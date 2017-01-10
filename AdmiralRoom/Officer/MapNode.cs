using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Huoyaoyuan.AdmiralRoom.API;

#pragma warning disable CC0014

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
        public MapNodeType Type { get; private set; } = MapNodeType.Unknown;
        public bool LoSAlert => rawdata.api_production_kind == 1;
        public PathData AfterPath => MapData.SelectPath(MapAreaId, MapNo,Id);
        public class MaterialInfo
        {
            public int ItemType { get; set; }
            public int ItemCount { get; set; }
            private ImageSource _itemicon;
            public ImageSource ItemIcon
            {
                get
                {
                    try
                    {
                        if (_itemicon == null)
                            _itemicon = new BitmapImage(new Uri($"pack://application:,,,/AdmiralRoom;component/Images/Material/{ItemType}.png", UriKind.Absolute));
                        return _itemicon;
                    }
                    catch { return null; }
                }
            }
            public bool ItemLostReduced { get; set; }
        }
        public MaterialInfo[] Material { get; private set; }
        public int AirSearchType { get; private set; }
        public int AirSearchResult { get; private set; }
        public MapNode(map_start_next api) : base(api) { }
        protected override void UpdateProp()
        {
            switch (rawdata.api_event_id)
            {
                case 2:
                    Type = MapNodeType.ItemGet;
                    Material = rawdata.api_itemget.Select(x => new MaterialInfo
                    {
                        ItemType = x.api_id,
                        ItemCount = x.api_getcount
                    }).ToArray();
                    break;
                case 3:
                    Type = MapNodeType.ItemLost;
                    Material = new[] { new MaterialInfo
                    {
                        ItemType = rawdata.api_happening.api_mst_id,
                        ItemCount = rawdata.api_happening.api_count,
                        ItemLostReduced = rawdata.api_happening.api_dentan != 0
                    } };
                    break;
                case 4:
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
                        case 4:
                            Type = MapNodeType.AirBattle;
                            break;
                        case 5:
                            Type = MapNodeType.Combined;
                            break;
                        case 6:
                            Type = MapNodeType.AirDefence;
                            break;
                    }
                    break;
                case 5:
                    if (rawdata.api_event_kind == 5)
                        Type = MapNodeType.CombinedBOSS;
                    else Type = MapNodeType.BOSS;
                    break;
                case 6:
                    if (rawdata.api_event_kind == 2) Type = MapNodeType.SelectRoute;
                    else Type = MapNodeType.Imagination;
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
                    Material = new[] { new MaterialInfo
                    {
                        ItemType = rawdata.api_itemget_eo_comment.api_id,
                        ItemCount = rawdata.api_itemget_eo_comment.api_getcount
                    } };
                    break;
                case 9:
                    Type = MapNodeType.Transport;
                    break;
                case 10:
                    Type = MapNodeType.AirDefence;
                    break;
            }
        }
    }
    public enum MapNodeType { Unknown, ItemGet, ItemLost, Imagination, Battle, NightBattle, NightToDayBattle, AirBattle, BOSS, SelectRoute, AirSearch, Guard, Transport, AirDefence, Combined, CombinedBOSS }
    public static class MapNodeTypeExtension
    {
        public static bool IsBOSS(this MapNodeType node) => node == MapNodeType.BOSS || node == MapNodeType.CombinedBOSS;
    }
}
