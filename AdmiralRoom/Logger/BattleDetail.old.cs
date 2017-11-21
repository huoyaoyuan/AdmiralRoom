using System;
using System.Collections.Generic;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;
using Huoyaoyuan.AdmiralRoom.Officer;
using Huoyaoyuan.AdmiralRoom.Officer.Battle;
using Huoyaoyuan.AdmiralRoom.Officer.Battle.Old;
using Meowtrix;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    class OldBattleDetail : IBattleDetail
    {
        public string time { get; set; }
        private DateTime? _utctime;
        public DateTime GetTimeStamp()
        {
            if (_utctime == null)
            {
                var utc = DateTime.Parse(time);
                _utctime = DateTime.SpecifyKind(utc, DateTimeKind.Unspecified);
            }
            return _utctime.Value;
        }
        bool IBattleDetail.IsValid => battle != null;
        DateTime IIdentifiable<DateTime>.Id => GetTimeStamp();
        public class APILog<T>
        {
            public string api { get; set; }
            public svdata<T> data { get; set; }
        }
        public APILog<map_start_next> startnext { get; set; }
        public APILog<sortie_battle_old> battle { get; set; }
        public APILog<sortie_battle_old> nightbattle { get; set; }
        public class ShipInfo
        {
            public int id { get; set; }
            public int shipid { get; set; }
            public int lv { get; set; }
            public int karyoku { get; set; }
            public int raisou { get; set; }
            public int taiku { get; set; }
            public int soukou { get; set; }
            public int kaihi { get; set; }
            public int taisen { get; set; }
            public int sakuteki { get; set; }
            public int lucky { get; set; }
            public EquipInfo[] slots { get; set; }
            public EquipInfo slotex { get; set; }
        }
        public class EquipInfo
        {
            public int itemid { get; set; }
            public int level { get; set; }
            public int alv { get; set; }
        }
        public ShipInfo[] fleet1 { get; set; }
        public ShipInfo[] fleet2 { get; set; }
        private static Dictionary<string, CombinedFleetType> apimap = new Dictionary<string, CombinedFleetType>
        {
            ["api_req_sortie/battle"] = CombinedFleetType.None,
            ["api_req_battle_midnight/sp_midnight"] = CombinedFleetType.None,
            ["api_req_sortie/airbattle"] = CombinedFleetType.None,
            ["api_req_sortie/ld_airbattle"] = CombinedFleetType.None,
            ["api_req_combined_battle/airbattle"] = CombinedFleetType.Carrier,
            ["api_req_combined_battle/battle"] = CombinedFleetType.Carrier,
            ["api_req_combined_battle/sp_midnight"] = CombinedFleetType.Carrier,
            ["api_req_combined_battle/battle_water"] = CombinedFleetType.Surface,
            ["api_req_combined_battle/ld_airbattle"] = CombinedFleetType.Carrier,
            ["api_req_combined_battle/ec_battle"] = CombinedFleetType.None,
            ["api_req_combined_battle/each_battle"] = CombinedFleetType.Carrier,
            ["api_req_combined_battle/each_battle_water"] = CombinedFleetType.Surface
        };

        private static readonly DateTime idChangeStamp = new DateTime(2017, 4, 5, 3, 0, 0);
        public BattleDetailViewModel ToViewModel(BattleDropLog log)
        {
            ShipInBattle[] fleet1, fleet2;
            EquipInBattle EquipSelector(EquipInfo equip)
                => equip != null ?
                new EquipInBattle(Staff.Current.MasterData.EquipInfo[equip.itemid])
                {
                    ImproveLevel = equip.level,
                    AirProficiency = equip.alv
                } : null;
            fleet1 = this.fleet1?.Select((ship, i) => new ShipInBattle
            {
                Index = i + 1,
                Level = ship.lv,
                ShipInfo = Staff.Current.MasterData.ShipInfo[ship.shipid],
                Firepower = ship.karyoku,
                Torpedo = ship.raisou,
                AA = ship.taiku,
                Armor = ship.soukou,
                Equipments = new[] { ship.slotex }.Concat(ship.slots).Where(x => x != null)
                        .Select(EquipSelector).ToArray(),
                EquipmentEx = EquipSelector(ship.slotex)
            }).ToArray();
            fleet2 = this.fleet2?.Select((ship, i) => new ShipInBattle
            {
                Index = i + 7,
                Level = ship.lv,
                ShipInfo = Staff.Current.MasterData.ShipInfo[ship.shipid],
                Firepower = ship.karyoku,
                Torpedo = ship.raisou,
                AA = ship.taiku,
                Armor = ship.soukou,
                Equipments = new[] { ship.slotex }.Concat(ship.slots).Where(x => x != null)
                            .Select(EquipSelector).ToArray(),
                EquipmentEx = EquipSelector(ship.slotex)
            }).ToArray();
            var node = new MapNode(startnext.data.api_data);
            //CombinedFleetType type;
            //if (!apimap.TryGetValue(battle.api, out type) && fleet2 != null)//broken log
            //{
            //    if (DispatcherHelper.UIDispatcher.Invoke(() =>
            //        MessageBox.Show("记录损坏。请问要将该场战斗作为机动/运输部队解析吗？", "记录损坏", MessageBoxButton.YesNo)) == MessageBoxResult.Yes)
            //        type = CombinedFleetType.Carrier;
            //    else type = CombinedFleetType.Surface;
            //}

            void PatchEnemyId(int[] enemy)
            {
                if (enemy == null) return;
                for (int i = 0; i < enemy.Length; i++)
                    if (enemy[i] > 500 && enemy[i] <= 1500)
                        enemy[i] += 1000;
            }

            if (log.DateTime < idChangeStamp)
            {
                PatchEnemyId(battle.data.api_data.api_ship_ke);
                PatchEnemyId(battle.data.api_data.api_ship_ke_combined);
            }

            apimap.TryGetValue(battle.api, out var type);
            var result = new OldBattle(battle.data.api_data, type, node.Type, fleet1, fleet2);
            if (nightbattle != null) result.NightBattle(nightbattle.data.api_data);
            return new BattleDetailViewModel
            {
                Log = log,
                Node = node,
                Battle = result,
                Time = new DateTimeOffset(GetTimeStamp(), TimeSpan.Zero)
            };
        }
    }
}
