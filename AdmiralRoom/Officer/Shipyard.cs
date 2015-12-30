using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;

#pragma warning disable CC0091

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Shipyard : NotificationObject
    {
        public Shipyard()
        {
            Staff.API("api_get_member/ndock").Subscribe<getmember_ndock[]>(NDockHandler);
            Staff.API("api_req_nyukyo/start").Subscribe(NStartHandler);
            Staff.API("api_get_member/kdock").Subscribe<getmember_kdock[]>(x =>
                Staff.Current.Dispatcher.Invoke(() =>
                BuildingDocks.UpdateAll(x, api => api.api_id)));
            Staff.API("api_req_kousyou/createship").Subscribe(CreateShipHandler);
            Staff.API("api_req_kousyou/getship").Subscribe<req_getship>(GetShipHandler);
            Staff.API("api_req_kousyou/destroyship").Subscribe<kousyou_destroyship>(DestroyShipHandler);
            Staff.API("api_req_kousyou/destroyitem2").Subscribe<kousyou_destroyitem2>(DestroyItemHandler);
            Staff.API("api_req_kaisou/powerup").Subscribe<kaisou_powerup>(PowerUpHandler);
            Staff.API("api_req_kousyou/createitem").Subscribe<kousyou_createitem>(CreateItemHandler);
            Staff.API("api_req_kousyou/createship_speedchange").Subscribe(KSpeedChangeHandler);
            Staff.API("api_req_kaisou/open_exslot").Subscribe(x =>
                Staff.Current.Homeport.Ships[x.GetInt("api_id")].SlotEx.IsLocked = false);
            Staff.API("api_req_kousyou/remodel_slot").Subscribe<kousyou_remodel_slot>(RemodelItemHandler);
            Staff.API("api_req_nyukyo/speedchange").Subscribe(NSpeedChangeHandler);
            Staff.API("api_req_kaisou/remodeling").Subscribe(x => Staff.Current.Homeport.Ships[x.GetInt("api_id")].IgnoreNextCondition());
        }

        #region RepairDocks
        private IDTable<RepairDock> _repairdocks = new IDTable<RepairDock>();
        public IDTable<RepairDock> RepairDocks
        {
            get { return _repairdocks; }
            set
            {
                if (_repairdocks != value)
                {
                    _repairdocks = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region BuildingDocks
        private IDTable<BuildingDock> _buildingdocks = new IDTable<BuildingDock>();
        public IDTable<BuildingDock> BuildingDocks
        {
            get { return _buildingdocks; }
            set
            {
                if (_buildingdocks != value)
                {
                    _buildingdocks = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Development
        private ObservableCollection<DevelopmentInfo> _development = new ObservableCollection<DevelopmentInfo>();
        public ObservableCollection<DevelopmentInfo> Development
        {
            get { return _development; }
            set
            {
                if (_development != value)
                {
                    _development = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        private int[] inndock = { };
        public void NDockHandler(getmember_ndock[] api)
        {
            Staff.Current.Dispatcher.Invoke(() => RepairDocks.UpdateAll(api, x => x.api_id));
            var newindock = api.Select(x => x.api_ship_id).ToArray();
            foreach (var ship in Staff.Current.Homeport.Ships)
            {
                if (newindock.Contains(ship.Id))
                    ship.IsRepairing = true;
                else if (inndock.Contains(ship.Id))
                {
                    ship.SetRepaired();
                }
                else ship.IsRepairing = false;
            }
            inndock = newindock;
            Staff.Current.Homeport.Fleets?.ForEach(x => x.UpdateStatus());
        }
        private void NStartHandler(NameValueCollection req)
        {
            int shipid = req.GetInt("api_ship_id");
            inndock[req.GetInt("api_ndock_id") - 1] = shipid;
            if (req.GetInt("api_highspeed") != 0)
            {
                Staff.Current.Homeport.Ships[shipid].SetRepaired();
                Staff.Current.Homeport.Material.InstantRepair--;
            }
        }
        private void NSpeedChangeHandler(NameValueCollection req)
        {
            RepairDocks[req.GetInt("api_ndock_id")].Ship.SetRepaired();
            Staff.Current.Homeport.Material.InstantRepair--;
        }
        private void CreateShipHandler(NameValueCollection req)
        {
            int dockid = req.GetInt("api_kdock_id");
            BuildingDocks[dockid].IsLSC = req.GetInt("api_large_flag") != 0;
            BuildingDocks[dockid].Secratary = Staff.Current.Homeport.Secratary;
        }
        private void GetShipHandler(req_getship api)
        {
            if (api.api_slotitem != null)
                foreach (var item in api.api_slotitem)
                    Staff.Current.Homeport.Equipments.Add(new Equipment(item));
            Staff.Current.Homeport.Ships.Add(new Ship(api.api_ship));
            Staff.Current.Homeport.UpdateCounts();
            BuildingDocks.UpdateAll(api.api_kdock, x => x.api_id);
        }
        private void DestroyShipHandler(NameValueCollection req, kousyou_destroyship api)
        {
            Staff.Current.Homeport.RemoveShip(Staff.Current.Homeport.Ships[req.GetInt("api_ship_id")]);
            Staff.Current.Homeport.Material.UpdateMaterial(api.api_material);
        }
        private void DestroyItemHandler(NameValueCollection req, kousyou_destroyitem2 api)
        {
            foreach (int id in req.GetInts("api_slotitem_ids"))
                Staff.Current.Homeport.Equipments.Remove(id);
            Staff.Current.Homeport.UpdateCounts();
            Staff.Current.Homeport.Material.Fuel += api.api_get_material[0];
            Staff.Current.Homeport.Material.Bull += api.api_get_material[1];
            Staff.Current.Homeport.Material.Steel += api.api_get_material[2];
            Staff.Current.Homeport.Material.Bauxite += api.api_get_material[3];
        }
        private void PowerUpHandler(NameValueCollection req, kaisou_powerup api)
        {
            foreach (int id in req.GetInts("api_id_items"))
            {
                Staff.Current.Homeport.RemoveShip(Staff.Current.Homeport.Ships[id]);
            }
            Staff.Current.Homeport.Ships[req.GetInt("api_id")].Update(api.api_ship);
            Staff.Current.Homeport.Fleets.UpdateWithoutRemove(api.api_deck, x => x.api_id);
        }
        private void CreateItemHandler(kousyou_createitem api)
        {
            var dev = new DevelopmentInfo { IsSuccess = api.api_create_flag != 0 };
            if (dev.IsSuccess)
            {
                Staff.Current.Homeport.Equipments.Add(new Equipment(api.api_slot_item));
                Staff.Current.Homeport.UpdateCounts();
                dev.Equip = Staff.Current.MasterData.EquipInfo[api.api_slot_item.api_slotitem_id];
            }
            else
            {
                dev.Equip = Staff.Current.MasterData.EquipInfo[int.Parse(api.api_fdata.Split(',')[1])];
            }
            Staff.Current.Dispatcher.Invoke(() => Development.Add(dev));
            Staff.Current.Homeport.Material.UpdateMaterial(api.api_material);
        }
        private void KSpeedChangeHandler(NameValueCollection req)
        {
            var dock = BuildingDocks[req.GetInt("api_kdock_id")];
            dock.State = DockState.BuildComplete;
            dock.CompleteTime = DateTime.UtcNow;
            Staff.Current.Homeport.Material.InstantBuild -= dock.IsLSC ? 10 : 1;
        }

        private void RemodelItemHandler(kousyou_remodel_slot api)
        {
            if (api.api_remodel_flag != 0)
                Staff.Current.Homeport.Equipments[api.api_after_slot.api_id].Update(api.api_after_slot);
            if (api.api_use_slot_id != null)
                foreach (int id in api.api_use_slot_id)
                    Staff.Current.Homeport.Equipments.Remove(id);
            Staff.Current.Homeport.UpdateCounts();
            Staff.Current.Homeport.Material.UpdateMaterial(api.api_after_material);
        }
    }
}
