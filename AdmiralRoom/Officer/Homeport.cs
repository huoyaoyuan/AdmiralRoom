using System;
using System.Collections.Specialized;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;
using Meowtrix.Collections.Generic;
using Meowtrix.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Homeport : NotificationObject
    {
        public Homeport()
        {
            Staff.API("api_get_member/require_info").Subscribe<getmember_require_info>(x =>
            {
                ItemsHandler(x.api_slot_item);
                Staff.Current.Shipyard.KDockHandler(x.api_kdock);
            });
            Staff.API("api_port/port").Subscribe<port_port>(PortHandler);
            Staff.API("api_get_member/deck").Subscribe<getmember_deck[]>(DecksHandler);
            Staff.API("api_get_member/slot_item").Subscribe<getmember_slotitem[]>(ItemsHandler);
            Staff.API("api_req_hensei/change").Subscribe(ChangeHandler);
            Staff.API("api_get_member/ship3").Subscribe<getmember_ship_deck>(Ship3Handler);
            Staff.API("api_get_member/ship2").Subscribe<api_ship[]>(x => Ships.UpdateAll(x, y => y.api_id));
            Staff.API("api_get_member/ship_deck").Subscribe<getmember_ship_deck>(Ship3Handler);
            Staff.API("api_req_hokyu/charge").Subscribe<hokyu_charge>(ChargeHandler);
            Staff.API("api_req_member/itemuse_cond").Subscribe(x => Fleets[x.GetInt("api_deck_id")].Ships.ForEach(y => y.IgnoreNextCondition()));
            Staff.API("api_req_hensei/preset_select").Subscribe<getmember_deck>(PresetHandler);
            Staff.API("api_req_kaisou/slot_exchange_index").Subscribe(ExchangeHandler);
            Staff.API("api_req_kaisou/slot_deprive").Subscribe<kaisou_slot_deprive>(DepriveHandler);
            Staff.API("api_get_member/mapinfo").Subscribe<getmembet_mapinfo[]>(RefreshMapInfo);
            Staff.API("api_req_map/select_eventmap_rank").Subscribe(SelectRank);
            Staff.API("api_req_mission/result").Subscribe<mission_result>(MissionHandler);
        }

        public Material Material { get; } = new Material();

        #region Fleets
        private IDTable<int, Fleet> _fleets;
        public IDTable<int, Fleet> Fleets
        {
            get { return _fleets; }
            set
            {
                if (_fleets != value)
                {
                    _fleets = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Ships
        private IDTable<int, Ship> _ships;
        public IDTable<int, Ship> Ships
        {
            get { return _ships; }
            set
            {
                if (_ships != value)
                {
                    _ships = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region SelectedFleet
        private int _selectedfleet;
        public int SelectedFleet
        {
            get { return _selectedfleet; }
            set
            {
                if (_selectedfleet != value)
                {
                    _selectedfleet = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Equipments
        private IDTable<int, Equipment> _equipments;
        public IDTable<int, Equipment> Equipments
        {
            get { return _equipments; }
            set
            {
                if (_equipments != value)
                {
                    _equipments = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region CombinedFleet
        private CombinedFleetType _combinedfleet;
        public CombinedFleetType CombinedFleet
        {
            get { return _combinedfleet; }
            set
            {
                if (_combinedfleet != value)
                {
                    _combinedfleet = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region MapsInProgress
        private MapInfo[] _mapsinprogress;
        public MapInfo[] MapsInProgress
        {
            get { return _mapsinprogress; }
            set
            {
                if (_mapsinprogress != value)
                {
                    _mapsinprogress = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public Ship Secretary => Fleets[1].Ships[0];

        public void UpdateCounts()
        {
            Staff.Current.Admiral.ShipCount = Ships.Count;
            Staff.Current.Admiral.EquipCount = Equipments.Count;
        }

        public void RemoveShip(Ship ship)
        {
            ship.InFleet?.Ships.Remove(ship);
            ship.InFleet?.UpdateStatus();
            foreach (var slot in ship.Slots)
                if (slot.HasItem)
                    Equipments.Remove(slot.Item);
            if (ship.SlotEx.HasItem)
                Equipments.Remove(ship.SlotEx.Item);
            Ships.Remove(ship);
            UpdateCounts();
        }

        private void PortHandler(port_port api)
        {
            System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
            Staff.Current.Admiral.BasicHandler(api.api_basic);
            ConditionHelper.Instance.BeginUpdate();
            Staff.Current.Shipyard.RepairDocks.ForEach(x => x.Ship?.IgnoreNextCondition());
            if (Ships == null)
                Ships = new IDTable<int, Ship>(api.api_ship.Select(x => new Ship(x)));
            else Ships.UpdateAll(api.api_ship, x => x.api_id);
            ConditionHelper.Instance.EndUpdate();
            Staff.Current.Admiral.ShipCount = api.api_ship.Length;
            Staff.Current.Shipyard.NDockHandler(api.api_ndock);
            DecksHandler(api.api_deck_port);
            if (Fleets.Any(x => x.MissionState == Fleet.FleetMissionState.Complete))
                Logger.Loggers.MaterialLogger.ForceLog = false;
            Material.MaterialHandler(api.api_material);
            CombinedFleet = (CombinedFleetType)api.api_combined_flag;
            Fleets.ForEach(x => x.CheckHomeportRepairingTime(false));
        }

        private void DecksHandler(getmember_deck[] api)
        {
            if (Fleets == null)
            {
                Fleets = new IDTable<int, Fleet>(api.Select(x => new Fleet(x))).WithSyncBindingEnabled();
                SelectedFleet = 0;
            }
            else
            {
                Fleets.UpdateAll(api, x => x.api_id);
            }
        }

        private void ItemsHandler(getmember_slotitem[] api)
        {
            if (Equipments == null)
                Equipments = new IDTable<int, Equipment>(api.Select(x => new Equipment(x)));
            else Equipments.UpdateAll(api, x => x.api_id);
            Staff.Current.Admiral.EquipCount = api.Length;
            Ships?.ForEach(x => x.TryUpdateEquip());
        }

        private void ChangeHandler(NameValueCollection api)
        {
            int idx = api.GetInt("api_ship_idx");
            int fleetid = api.GetInt("api_id");
            int shipid = api.GetInt("api_ship_id");
            var fleet = Fleets[fleetid];
            if (idx == -1)//旗艦以外全解除
            {
                for (int i = fleet.Ships.Count - 1; i > 0; i--)
                {
                    fleet.Ships[i].InFleet = null;
                    fleet.Ships.RemoveAt(i);
                }
            }
            else
            {
                if (shipid == -1)//はずす
                {
                    fleet.Ships[idx].InFleet = null;
                    fleet.Ships.RemoveAt(idx);
                }
                else
                {
                    var ship = Ships[shipid];
                    var destf = ship.InFleet;
                    if (idx < fleet.Ships.Count) fleet.Ships[idx].InFleet = destf;
                    if (destf != null)
                    {
                        if (idx < fleet.Ships.Count)
                            destf.Ships[destf.Ships.IndexOf(ship)] = fleet.Ships[idx];
                        else destf.Ships.Remove(ship);
                        destf.UpdateStatus();
                        if (destf != fleet && destf.Ships?.FirstOrDefault()?.ShipInfo.ShipType.Id == 19)
                            destf.CheckHomeportRepairingTime(true);
                    }
                    if (idx >= fleet.Ships.Count) fleet.Ships.Add(ship);
                    else fleet.Ships[idx] = ship;
                    ship.InFleet = fleet;
                }
                if (fleet.Ships?.FirstOrDefault()?.ShipInfo.ShipType.Id == 19)
                    fleet.CheckHomeportRepairingTime(true);
            }
            fleet.UpdateStatus();
        }

        private void PresetHandler(NameValueCollection req, getmember_deck api)
        {
            int deck = req.GetInt("api_deck_id");
            var fleet = Fleets[deck];
            fleet.Ships.ForEach(x => x.InFleet = null);
            fleet.Update(api);
        }

        private void Ship3Handler(getmember_ship_deck api)
        {
            Ships.UpdateWithoutRemove(api.api_ship_data, x => x.api_id);
            Fleets.UpdateWithoutRemove(api.api_deck_data, x => x.api_id);
        }

        private void ChargeHandler(hokyu_charge api)
        {
            foreach (var s in api.api_ship)
            {
                var ship = Ships[s.api_id];
                ship.Fuel = new LimitedValue(s.api_fuel, ship.Fuel.Max);
                ship.Bull = new LimitedValue(s.api_bull, ship.Bull.Max);
                for (int i = 0; i < ship.Slots.Count; i++)
                    ship.Slots[i].AirCraft = new LimitedValue(s.api_onslot[i], ship.Slots[i].AirCraft.Max);
                ship.UpdateStatus();
            }
            Material.Fuel = api.api_material[0];
            Material.Bull = api.api_material[1];
            Material.Steel = api.api_material[2];
            Material.Bauxite = api.api_material[3];
            Fleets.ForEach(x => x.UpdateStatus());
        }

        private void ExchangeHandler(NameValueCollection req)
        {
            var ship = Ships[req.GetInt("api_id")];
            int src = req.GetInt("api_src_idx");
            int dst = req.GetInt("api_dst_idx");
            var item1 = ship.Slots[src].Item;
            var item2 = ship.Slots[dst].Item;
            ship.Slots[src].Item = item2;
            ship.Slots[dst].Item = item1;
            ship.UpdateStatus();
        }

        private void DepriveHandler(kaisou_slot_deprive api)
        {
            Ships[api.api_ship_data.api_set_ship.api_id].Update(api.api_ship_data.api_set_ship);
            Ships[api.api_ship_data.api_unset_ship.api_id].Update(api.api_ship_data.api_unset_ship);
        }

        private void RefreshMapInfo(getmembet_mapinfo[] api)
        {
            foreach (var info in api)
            {
                var map = Staff.Current.MasterData.MapInfos[info.api_id];
                map.IsClear = info.api_cleared != 0;
                map.DefeatedCount = info.api_defeat_count;
                map.Difficulty = (EventMapDifficulty)(info.api_eventmap?.api_selected_rank ?? 0);
                map.MaxHP = info.api_eventmap?.api_max_maphp ?? 0;
                map.NowHP = info.api_eventmap?.api_now_maphp ?? 0;
            }
            MapsInProgress = Staff.Current.MasterData.MapInfos.Where(x => !x.IsClear).ToArray();
        }

        private void SelectRank(NameValueCollection req)
        {
            int mapid = req.GetInt("api_maparea_id") * 10 + req.GetInt("api_map_no");
            Staff.Current.MasterData.MapInfos[mapid].Difficulty = (EventMapDifficulty)req.GetInt("api_rank");
            Reporter.PoiDBReporter.ReportAsync(new JObject
            {
                ["teitokuLv"] = Staff.Current.Admiral.Level,
                ["teitokuId"] = Staff.Current.Admiral.MemberID,
                ["mapareaId"] = mapid,
                ["rank"] = req.GetInt("api_rank")
            }, "select_rank");
        }

        private void MissionHandler(mission_result api)
        {
            var log = new Logger.MissionLog
            {
                DateTime = DateTime.UtcNow,
                MissionName = api.api_quest_name,
                ResultRank = api.api_clear_result,
                UseItem1 = api.api_useitem_flag[0],
                UseItem1Count = api.api_get_item1?.api_useitem_count ?? 0,
                UseItem2 = api.api_useitem_flag[1],
                UseItem2Count = api.api_get_item2?.api_useitem_count ?? 0
            };
            if (api.api_useitem_flag[0] == 4) log.UseItem1 = api.api_get_item1.api_useitem_id;
            if (api.api_useitem_flag[1] == 4) log.UseItem2 = api.api_get_item2.api_useitem_id;
            if (api.api_get_material?.Length >= 1) log.Item1 = api.api_get_material[0];
            if (api.api_get_material?.Length >= 2) log.Item2 = api.api_get_material[1];
            if (api.api_get_material?.Length >= 3) log.Item3 = api.api_get_material[2];
            if (api.api_get_material?.Length >= 4) log.Item4 = api.api_get_material[3];
            Logger.Loggers.MissionLogger.Log(log);
            Logger.Loggers.MaterialLogger.ForceLog = true;
        }
    }
    public enum CombinedFleetType { None, Carrier, Surface, Transport }
}
