using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class MapNode : GameObject<map_start_next>
    {
        public override int Id => rawdata.api_no;
        public int MapAreaId => rawdata.api_maparea_id;
        public MapArea MapArea => Staff.Current.MasterData.MapAreas[MapAreaId];
        public int MapNo => rawdata.api_mapinfo_no;
        public MapInfo Map => MapArea[MapNo];
        /// <summary>
        /// 后续分歧个数
        /// </summary>
        public int Forewards => rawdata.api_next;
        public MapNodeType Type { get; private set; }
        public bool LoSAlert => rawdata.api_production_kind == 1;
        public int ItemType { get; private set; }
        public int ItemCount { get; private set; }
        public bool ItemLostReduced { get; private set; }
        public int AirSearchType { get; private set; }
        public int AirSearchResult { get; private set; }
        public MapNode() { }
        public MapNode(map_start_next api) : base(api) { }
        protected override void UpdateProp()
        {
            switch (rawdata.api_event_id)
            {
                case 2:
                    Type = MapNodeType.ItemGet;
                    ItemType = rawdata.api_itemget.api_id;
                    ItemCount = rawdata.api_itemget.api_getcount;
                    break;
                case 3:
                    Type = MapNodeType.ItemLost;
                    ItemType = rawdata.api_happening.api_mst_id;
                    ItemCount = rawdata.api_happening.api_count;
                    ItemLostReduced = rawdata.api_happening.api_dentan != 0;
                    break;
                case 5:
                    Type = MapNodeType.BOSS;
                    break;
                case 6:
                    Type = MapNodeType.Imagination;
                    break;
                case 7:
                    if (rawdata.api_event_kind == 0)
                    {
                        Type = MapNodeType.AirSearch;
                        AirSearchType = rawdata.api_airsearch.api_plane_type;
                        AirSearchResult = rawdata.api_airsearch.api_result;
                    }
                    else if (rawdata.api_event_kind == 4) Type = MapNodeType.AirBattle;
                    break;
                case 8:
                    Type = MapNodeType.Guard;
                    ItemType = rawdata.api_itemget_eo_comment.api_id;
                    ItemCount = rawdata.api_itemget_eo_comment.api_getcount;
                    break;
                default:
                    switch (rawdata.api_event_kind)
                    {
                        case 1:
                            Type = MapNodeType.Battle;
                            break;
                        case 2:
                            Type = MapNodeType.NightBattle;
                            break;
                        case 3:
                            Type = MapNodeType.NightToDayBattle;
                            break;
                        default:
                            Type = MapNodeType.Unknown;
                            break;
                    }
                    break;
            }
        }
    }
    public enum MapNodeType { Unknown, ItemGet, ItemLost, Imagination, Battle, NightBattle, NightToDayBattle, AirBattle, BOSS, SelectRoute, AirSearch, Guard }
}
