using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class BattleState : NotificationObject
    {
        public BattleState()
        {
            Staff.API("api_port/port").Subscribe((Fiddler.Session x) =>
            {
                InSortie = false;
                CurrentMap = null;
                CurrentNode = null;
                if (sortiefleet1 != null) sortiefleet1.InSortie = false;
                if (sortiefleet2 != null) sortiefleet2.InSortie = false;
                sortiefleet1 = null;
                sortiefleet2 = null;
            });
            Staff.API("api_req_map/start").Subscribe<map_start_next>(StartNextHandler);
            Staff.API("api_req_map/next").Subscribe<map_start_next>(StartNextHandler);
            Staff.API("api_req_sortie/battleresult").Subscribe<sortie_battleresult>(BattleResultHandler);
            Staff.API("api_req_map/start").Subscribe((System.Collections.Specialized.NameValueCollection x) =>
            {
                InSortie = true;
                sortiefleet1 = Staff.Current.Homeport.Fleets[x.GetInt("api_deck_id")];
                sortiefleet1.InSortie = true;
                if (Staff.Current.Homeport.CombinedFleet > 0)
                {
                    sortiefleet2 = Staff.Current.Homeport.Fleets[2];
                    sortiefleet2.InSortie = true;
                }
            });
        }
        private Fleet sortiefleet1, sortiefleet2;

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

        void StartNextHandler(map_start_next api)
        {
            CurrentMap = Staff.Current.MasterData.MapAreas[api.api_maparea_id][api.api_mapinfo_no];
            CurrentNode = new MapNode(api);
            sortiefleet1?.Ships.ArrayOperation(y => y.IgnoreNextCondition());
            sortiefleet2?.Ships.ArrayOperation(y => y.IgnoreNextCondition());
        }
        void BattleResultHandler(sortie_battleresult api)
        {
            sortiefleet1?.Ships.ArrayOperation(y => y.IgnoreNextCondition());
            sortiefleet2?.Ships.ArrayOperation(y => y.IgnoreNextCondition());
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
