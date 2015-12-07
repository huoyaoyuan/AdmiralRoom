using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class MapInfo : GameObject<api_mst_mapinfo>
    {
        public override int Id => rawdata.api_id;
        public int AreaNo => rawdata.api_maparea_id;
        public MapArea Area => Staff.Current.MasterData.MapAreas[AreaNo];
        public int No => rawdata.api_no;
        public string Name => rawdata.api_name;
        public int Level => rawdata.api_level;
        public string OperationName => rawdata.api_opetext;
        public string Info => rawdata.api_infotext.Replace("<br>", "");
        public int[] GetItem => rawdata.api_item;
        public int MapHP => rawdata.api_max_maphp ?? 0;
        public int RequiredDefeatCount => rawdata.api_required_defeat_count ?? 1;
        public bool IsNormalFleet => rawdata.api_sally_flag[0] == 1;
        public bool CanCombinedCarrier => (rawdata.api_sally_flag[1] & 1) != 0;
        public bool CanCombinedSurface => (rawdata.api_sally_flag[1] & 2) != 0;

        #region DefeatedCount
        private int _defeatedcount;
        public int DefeatedCount
        {
            get { return _defeatedcount; }
            set
            {
                if (_defeatedcount != value)
                {
                    _defeatedcount = value;
                    OnAllPropertyChanged();
                }
            }
        }
        #endregion

        #region NowHP
        private int _nowhp;
        public int NowHP
        {
            get { return _nowhp; }
            set
            {
                if (_nowhp != value)
                {
                    _nowhp = value;
                    OnAllPropertyChanged();
                }
            }
        }
        #endregion

        #region MaxHP
        private int _maxhp;
        public int MaxHP
        {
            get { return _maxhp; }
            set
            {
                if (_maxhp != value)
                {
                    _maxhp = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region IsClear
        private bool _isclear;
        public bool IsClear
        {
            get { return _isclear; }
            set
            {
                if (_isclear != value)
                {
                    _isclear = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public bool IsFinal
        {
            get
            {
                if (MaxHP != 0) return NowHP <= Staff.Current.MasterData.QueryFinalHP(this);
                else if (RequiredDefeatCount > 1) return RequiredDefeatCount - DefeatedCount == 1;
                else if (!IsClear) return true;
                else return false;
            }
        }
        public LimitedValue HPMeter
        {
            get
            {
                if (MaxHP != 0) return new LimitedValue(NowHP, MaxHP);
                else if (IsClear) return new LimitedValue(0, RequiredDefeatCount);
                else if (RequiredDefeatCount > 1) return new LimitedValue(RequiredDefeatCount - DefeatedCount, RequiredDefeatCount);
                else return new LimitedValue(1, 1);
            }
        }
        public EventMapDifficulty Difficulty { get; set; }
        public MapInfo() { }
        public MapInfo(api_mst_mapinfo api) : base(api) { }
    }
    public enum EventMapDifficulty { None = 0, Easy = 1, Medium = 2, Hard = 3 }
}
