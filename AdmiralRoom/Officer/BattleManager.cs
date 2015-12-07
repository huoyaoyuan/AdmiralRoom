using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class BattleManager : NotificationObject
    {
        public BattleManager()
        {
            Staff.API("api_port/port").Subscribe((Fiddler.Session x) =>
            {
                InSortie = false;
                CurrentMap = null;
                CurrentNode = null;
                if (SortieFleet1 != null) SortieFleet1.InSortie = false;
                if (SortieFleet2 != null) SortieFleet2.InSortie = false;
                SortieFleet1 = null;
                SortieFleet2 = null;
                CurrentBattle = null;
            });
            Staff.API("api_req_map/start").Subscribe<map_start_next>(StartNextHandler);
            Staff.API("api_req_map/next").Subscribe<map_start_next>(StartNextHandler);
            Staff.API("api_req_sortie/battleresult").Subscribe<sortie_battleresult>(BattleResultHandler);
            Staff.API("api_req_map/start").Subscribe((System.Collections.Specialized.NameValueCollection x) =>
            {
                InSortie = true;
                SortieFleet1 = Staff.Current.Homeport.Fleets[x.GetInt("api_deck_id")];
                SortieFleet1.InSortie = true;
                if (SortieFleet1.Id == 1 && Staff.Current.Homeport.CombinedFleet != CombinedFleetType.None)
                {
                    SortieFleet2 = Staff.Current.Homeport.Fleets[2];
                    SortieFleet2.InSortie = true;
                }
                ItemsAfterShips = true;
            });
            Staff.API("api_req_sortie/battle").SubscribeDynamic(x => CurrentBattle = new Battle(x, CombinedFleetType.None, this, BattleType.Day));
            Staff.API("api_req_battle_midnight/battle").SubscribeDynamic(x => CurrentBattle = CurrentBattle.NightBattle(x));
            Staff.API("api_req_battle_midnight/sp_midnight").SubscribeDynamic(x => CurrentBattle = new Battle(x, CombinedFleetType.None, this, BattleType.Night));
            Staff.API("api_req_sortie/airbattle").SubscribeDynamic(x => CurrentBattle = new Battle(x, CombinedFleetType.None, this, BattleType.Air));
            Staff.API("api_req_combined_battle/airbattle").SubscribeDynamic(x => CurrentBattle = new Battle(x, Staff.Current.Homeport.CombinedFleet, this, BattleType.Air));
            Staff.API("api_req_combined_battle/battle").SubscribeDynamic(x => CurrentBattle = new Battle(x, Staff.Current.Homeport.CombinedFleet, this, BattleType.Day));
            Staff.API("api_req_combined_battle/midnight_battle").SubscribeDynamic(x => CurrentBattle = CurrentBattle.NightBattle(x));
            Staff.API("api_req_combined_battle/sp_midnight").SubscribeDynamic(x => CurrentBattle = new Battle(x, Staff.Current.Homeport.CombinedFleet, this, BattleType.Night));
            Staff.API("api_req_combined_battle/battle_water").SubscribeDynamic(x => CurrentBattle = new Battle(x, CombinedFleetType.Surface, this, BattleType.Day));
        }
        public Fleet SortieFleet1 { get; private set; }
        public Fleet SortieFleet2 { get; private set; }

        #region InSortie
        private bool _insortie;
        public bool InSortie
        {
            get { return _insortie; }
            set
            {
                if (_insortie != value)
                {
                    _insortie = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region CurrentMap
        private MapInfo _currentmap;
        public MapInfo CurrentMap
        {
            get { return _currentmap; }
            set
            {
                if (_currentmap != value)
                {
                    _currentmap = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region CurrentNode
        private MapNode _currentnode;
        public MapNode CurrentNode
        {
            get { return _currentnode; }
            set
            {
                if (_currentnode != value)
                {
                    _currentnode = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region CurrentBattle
        private Battle _currentbattle;
        public Battle CurrentBattle
        {
            get { return _currentbattle; }
            set
            {
                _currentbattle = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public bool ItemsAfterShips = false;
        private void StartNextHandler(map_start_next api)
        {
            CurrentMap = Staff.Current.MasterData.MapAreas[api.api_maparea_id][api.api_mapinfo_no];
            CurrentNode = new MapNode(api);
            SortieFleet1?.Ships.ForEach(y => y.IgnoreNextCondition());
            SortieFleet2?.Ships.ForEach(y => y.IgnoreNextCondition());
            CurrentBattle = null;
        }
        private void BattleResultHandler(sortie_battleresult api)
        {
            SortieFleet1?.Ships.ForEach(y => y.IgnoreNextCondition());
            SortieFleet2?.Ships.ForEach(y => y.IgnoreNextCondition());
            if (CurrentNode.Type == MapNodeType.BOSS)
            {
                StaticCounters.BossCounter.Increase();
                if (ConstData.RanksWin.Contains(api.api_win_rank))
                {
                    StaticCounters.BossWinCounter.Increase();
                    if (CurrentMap.AreaNo == 2) StaticCounters.Map2Counter.Increase();
                    else if (CurrentMap.AreaNo == 3 && CurrentMap.No >= 3) StaticCounters.Map3Counter.Increase();
                    else if (CurrentMap.AreaNo == 4) StaticCounters.Map4Counter.Increase();
                    else if (CurrentMap.Id == 15) StaticCounters.Map1_5Counter.Increase();
                }
            }
            Staff.Current.Quests.Save();
        }
    }
}
