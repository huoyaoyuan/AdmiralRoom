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
        public Equipment() { }
        public Equipment(getmember_slotitem api) : base(api) { }
        public override string ToString()
        {
            string s = EquipInfo.Name;
            if (ImproveLevel > 0) s += $" ★+{ImproveLevel}";
            if (AirProficiency > 0) s += $" +{AirProficiency}";
            return s;
        }
    }
}
