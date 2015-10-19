using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Shipyard : NotifyBase
    {
        public Shipyard()
        {
            Staff.RegisterHandler("api_get_member/ndock", x => NDockHandler(x.Parse<getmember_ndock[]>().Data));
            Staff.RegisterHandler("api_req_nyukyo/start", x => NStartHandler(x.Parse().Request));
            Staff.RegisterHandler("api_get_member/kdock", x =>
                Staff.Current.Dispatcher.Invoke(() =>
                BuildingDocks.UpdateAll(x.Parse<getmember_kdock[]>().Data, api => api.api_id)));
            Staff.RegisterHandler("api_req_kousyou/createship", x => CreateShipHandler(x.Parse().Request));
            Staff.RegisterHandler("api_req_kousyou/getship", x => GetShipHandler(x.Parse<req_getship>().Data));
            Staff.RegisterHandler("api_req_kousyou/destroyship", x => DestroyShipHandler(x.Parse().Request, x.Parse<kousyou_destroyship>().Data));
            Staff.RegisterHandler("api_req_kousyou/destroyitem2", x => DestroyItemHandler(x.Parse().Request, x.Parse<kousyou_destroyitem2>().Data));
            Staff.RegisterHandler("api_req_kaisou/powerup", x => PowerUpHandler(x.Parse().Request, x.Parse<kaisou_powerup>().Data));
            Staff.RegisterHandler("api_req_kousyou/createitem", x => CreateItemHandler(x.Parse().Request, x.Parse<kousyou_createitem>().Data));
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

        private int[] inndock = { };
        public void NDockHandler(getmember_ndock[] api)
        {
            Staff.Current.Dispatcher.Invoke(() => RepairDocks.UpdateAll(api, x => x.api_id));
            var newindock = api.ArrayOperation(x => x.api_ship_id).ToArray();
            foreach(var ship in Staff.Current.Homeport.Ships)
            {
                if (newindock.Contains(ship.Id))
                    ship.IsRepairing = true;
                else if (inndock.Contains(ship.Id))
                {
                    ship.IsRepairing = false;
                    ship.RepairingHP = ship.HP.Max;
                }
                else ship.IsRepairing = false;
            }
            inndock = newindock;
        }
        void NStartHandler(NameValueCollection req)
        {
            inndock[req.GetInt("api_ndock_id") - 1] = req.GetInt("api_ship_id");
        }
        void CreateShipHandler(NameValueCollection req)
        {
            int dockid = req.GetInt("api_kdock_id");
            BuildingDocks[dockid].IsLSC = req.GetInt("api_large_flag") != 0;
            BuildingDocks[dockid].Secratary = Staff.Current.Homeport.Secratary;
        }
        void GetShipHandler(req_getship api)
        {
            if (api.api_slotitem != null)
                foreach (var item in api.api_slotitem)
                    Staff.Current.Homeport.Equipments.Add(new Equipment(item));
            Staff.Current.Homeport.Ships.Add(new Ship(api.api_ship));
            Staff.Current.Homeport.UpdateCounts();
        }
        void DestroyShipHandler(NameValueCollection req, kousyou_destroyship api)
        {
            Staff.Current.Homeport.RemoveShip(Staff.Current.Homeport.Ships[req.GetInt("api_ship_id")]);
            Staff.Current.Homeport.Material.Fuel = api.api_material[0];
            Staff.Current.Homeport.Material.Bull = api.api_material[1];
            Staff.Current.Homeport.Material.Steel = api.api_material[2];
            Staff.Current.Homeport.Material.Bauxite = api.api_material[3];
        }
        void DestroyItemHandler(NameValueCollection req, kousyou_destroyitem2 api)
        {
            foreach(int id in req.GetInts("api_slotitem_ids"))
            {
                Staff.Current.Homeport.Equipments.Remove(Staff.Current.Homeport.Equipments[id]);
            }
            Staff.Current.Homeport.UpdateCounts();
            Staff.Current.Homeport.Material.Fuel += api.api_get_material[0];
            Staff.Current.Homeport.Material.Bull += api.api_get_material[1];
            Staff.Current.Homeport.Material.Steel += api.api_get_material[2];
            Staff.Current.Homeport.Material.Bauxite += api.api_get_material[3];
        }
        void PowerUpHandler(NameValueCollection req, kaisou_powerup api)
        {
            foreach(int id in req.GetInts("api_id_items"))
            {
                Staff.Current.Homeport.RemoveShip(Staff.Current.Homeport.Ships[id]);
            }
            Staff.Current.Homeport.Ships.UpdateWithoutRemove(api.api_ship, x => x.api_id);
            Staff.Current.Homeport.Fleets.UpdateWithoutRemove(api.api_deck, x => x.api_id);
        }
        void CreateItemHandler(NameValueCollection req, kousyou_createitem api)
        {
            if (api.api_create_flag == 1)
            {
                Staff.Current.Homeport.Equipments.Add(new Equipment(api.api_slot_item));
                Staff.Current.Homeport.UpdateCounts();
            }
            Staff.Current.Homeport.Material.Fuel = api.api_material[0];
            Staff.Current.Homeport.Material.Bull = api.api_material[1];
            Staff.Current.Homeport.Material.Steel = api.api_material[2];
            Staff.Current.Homeport.Material.Bauxite = api.api_material[3];
        }
    }
}
