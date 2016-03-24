using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Equipment : GameObject<getmember_slotitem>
    {
        public override int Id => rawdata.api_id;
        public EquipInfo EquipInfo => Staff.Current.MasterData.EquipInfo[rawdata.api_slotitem_id];
        public bool IsLocked => rawdata.api_locked != 0;
        /// <summary>
        /// 改修等级
        /// </summary>
        public int ImproveLevel => rawdata.api_level;
        /// <summary>
        /// 熟练度
        /// </summary>
        public int AirProficiency => rawdata.api_alv;
        public ImageSource ProfIcon
            => AirProficiency == 0 ? null :
            new BitmapImage(new Uri($"pack://application:,,,/AdmiralRoom;component/Images/AirProficiency/{AirProficiency}.png", UriKind.Absolute));
        public Ship OnShip { get; set; }
        public void SetNotOnShip() => OnShip = null;
        public Equipment(getmember_slotitem api) : base(api) { }
        public override string ToString()
        {
            string s = EquipInfo.Name;
            if (ImproveLevel > 0 && ImproveLevel < 10) s += $" ★+{ImproveLevel}";
            if (ImproveLevel == 10) s += " ★max";
            if (AirProficiency > 0) s += $" +{AirProficiency}";
            return s;
        }
        protected override void UpdateProp()
        {
            if (OnShip != null && !OnShip.FindEquipment(Id)) OnShip = null;
        }
    }
}
