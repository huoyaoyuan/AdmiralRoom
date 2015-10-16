using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Equipment : GameObject<getmember_slotitem>, IIdentifiable
    {
        public int Id => rawdata.api_id;
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
        public Equipment(getmember_slotitem api) : base(api) { }
    }
}
