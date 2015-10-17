using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class EquipInfo : GameObject<api_mst_slotitem>, IIdentifiable
    {
        public int Id => rawdata.api_id;
        public int SortNo => rawdata.api_sortno;
        public string Name => rawdata.api_name;
        public EquipType EquipType => Staff.Current.MasterData.EquipTypes[rawdata.api_type[2]];
        public int IconID => rawdata.api_type[3];
        private ImageSource _icon;
        public ImageSource Icon
        {
            get
            {
                if (_icon == null)
                {
                    try
                    {
                        _icon = new BitmapImage(new Uri($"pack://application:,,,/AdmiralRoom;component/Images/Equip/{IconID}.png", UriKind.Absolute));
                    }
                    catch
                    {
                        _icon = new BitmapImage(new Uri("pack://application:,,,/AdmiralRoom;component/Images/Equip/Misc.png", UriKind.Absolute));
                    }
                }
                return _icon;
            }
        }
        public int FirePower => rawdata.api_houg;
        public int Torpedo => rawdata.api_raig;
        public int Armor => rawdata.api_souk;
        public int AA => rawdata.api_tyku;
        public int Bomb => rawdata.api_baku;
        public int ASW => rawdata.api_tais;
        public int Accuracy => rawdata.api_houm;
        public int Evasion => rawdata.api_houk;
        public int ViewRange => rawdata.api_saku;
        public ShootRange Range => (ShootRange)rawdata.api_leng;
        public int Rare => rawdata.api_rare;
        public int[] DestroyMaterial => rawdata.api_broken;
        public string Info => rawdata.api_info;
        public EquipInfo() { }
        public EquipInfo(api_mst_slotitem api) : base(api) { }
    }
}
