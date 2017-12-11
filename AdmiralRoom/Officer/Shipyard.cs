using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;
using Meowtrix.Collections.Generic;
using Meowtrix.ComponentModel;
using Newtonsoft.Json.Linq;

#pragma warning disable CC0091

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Shipyard : NotificationObject
    {
        public Shipyard()
        {
            Staff.API("api_get_member/ndock").Subscribe<getmember_ndock[]>(NDockHandler);
            Staff.API("api_req_nyukyo/start").Subscribe(NStartHandler);
            Staff.API("api_get_member/kdock").Subscribe<getmember_kdock[]>(KDockHandler);
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
            Staff.API("api_req_kaisou/remodeling").Subscribe(x =>
            {
                var ship = Staff.Current.Homeport.Ships[x.GetInt("api_id")];
                ship.IgnoreNextCondition();
                ship.InvalidEquip();
            });
        }

        public IDTable<int, RepairDock> RepairDocks { get; } = new IDTable<int, RepairDock>().WithSyncBindingEnabled();
        public IDTable<int, BuildingDock> BuildingDocks { get; } = new IDTable<int, BuildingDock>().WithSyncBindingEnabled();
        public ObservableCollection<DevelopmentInfo> Development { get; } = new ObservableCollection<DevelopmentInfo>().WithSyncBindingEnabled();

        private int[] inndock = { };
        private int lastcreatedock = -1;
        public void NDockHandler(getmember_ndock[] api)
        {
            RepairDocks.UpdateAll(api, x => x.api_id);
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
            Logger.Loggers.MaterialLogger.ForceLog = true;
        }
        private void NSpeedChangeHandler(NameValueCollection req)
        {
            RepairDocks[req.GetInt("api_ndock_id")].Ship.SetRepaired();
            Staff.Current.Homeport.Material.InstantRepair--;
        }
        private void CreateShipHandler(NameValueCollection req)
        {
            int dockid = lastcreatedock = req.GetInt("api_kdock_id");
            BuildingDocks[dockid].IsLSC = req.GetInt("api_large_flag") != 0;
            BuildingDocks[dockid].Secretary = Staff.Current.Homeport.Secretary;
            BuildingDocks[dockid].HighSpeed = req.GetInt("api_highspeed");
            Logger.Loggers.MaterialLogger.ForceLog = true;
        }
        public void KDockHandler(getmember_kdock[] api)
        {
            BuildingDocks.UpdateAll(api, x => x.api_id);
            var dock = BuildingDocks[lastcreatedock];
            if (dock?.CreatedShip != null)
            {
                Logger.Loggers.CreateShipLogger.Log(new Logger.CreateShipLog
                {
                    DateTime = DateTime.UtcNow,
                    AdmiralLevel = Staff.Current.Admiral.Level,
                    EmptyDocks = BuildingDocks.Count(x => x.State == DockState.Empty),
                    Item1 = dock.UseFuel,
                    Item2 = dock.UseBull,
                    Item3 = dock.UseSteel,
                    Item4 = dock.UseBauxite,
                    Item5 = dock.UseDevelopment,
                    IsLSC = dock.IsLSC,
                    SecretaryId = Staff.Current.Homeport.Secretary.ShipInfo.Id,
                    SecretaryLevel = Staff.Current.Homeport.Secretary.Level,
                    ShipId = dock.CreatedShip.Id
                });
                Reporter.PoiDBReporter.ReportAsync(new JObject
                {
                    ["items"] = new JArray
                    {
                        dock.UseFuel,
                        dock.UseBull,
                        dock.UseSteel,
                        dock.UseBauxite,
                        dock.UseDevelopment
                    },
                    ["kdockId"] = lastcreatedock - 1,
                    ["secretary"] = Staff.Current.Homeport.Secretary.ShipInfo.Id,
                    ["teitokuLv"] = Staff.Current.Admiral.Level,
                    ["largeFlag"] = dock.IsLSC,
                    ["highspeed"] = dock.HighSpeed,
                    ["shipId"] = dock.CreatedShip.Id
                }, "create_ship");
            }
            lastcreatedock = -1;
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
            foreach (int id in req.GetInts("api_ship_id"))
                Staff.Current.Homeport.RemoveShip(Staff.Current.Homeport.Ships[id]);
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
        private void CreateItemHandler(NameValueCollection req, kousyou_createitem api)
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
            Development.Insert(0, dev);
            Staff.Current.Homeport.Material.UpdateMaterial(api.api_material);
            Logger.Loggers.CreateItemLogger.Log(new Logger.CreateItemLog
            {
                DateTime = DateTime.UtcNow,
                SecretaryId = Staff.Current.Homeport.Secretary.ShipInfo.Id,
                SecretaryLevel = Staff.Current.Homeport.Secretary.Level,
                IsSuccess = dev.IsSuccess,
                AdmiralLevel = Staff.Current.Admiral.Level,
                EquipId = dev.Equip.Id,
                Item1 = req.GetInt("api_item1"),
                Item2 = req.GetInt("api_item2"),
                Item3 = req.GetInt("api_item3"),
                Item4 = req.GetInt("api_item4")
            });
            Logger.Loggers.MaterialLogger.ForceLog = true;
            Reporter.PoiDBReporter.ReportAsync(new JObject
            {
                ["items"] = new JArray
                {
                    req.GetInt("api_item1"),
                    req.GetInt("api_item2"),
                    req.GetInt("api_item3"),
                    req.GetInt("api_item4")
                },
                ["teitokuLv"] = Staff.Current.Admiral.Level,
                ["itemId"] = dev.Equip.Id,
                ["secretary"] = Staff.Current.Homeport.Secretary.ShipInfo.Id,
                ["successful"] = dev.IsSuccess
            }, "create_item");
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
