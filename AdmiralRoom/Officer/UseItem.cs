using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class UseItem : GameObject<api_mst_useitem>
    {
        public UseItem() { }
        public UseItem(api_mst_useitem api) : base(api) { }
        public override int Id => rawdata.api_id;
        public int UseType => rawdata.api_usetype;
        public int Category => rawdata.api_category;
        public string Name => rawdata.api_name;
        public string Description => rawdata.api_description[0];
    }
}
