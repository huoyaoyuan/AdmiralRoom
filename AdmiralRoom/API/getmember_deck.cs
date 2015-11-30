namespace Huoyaoyuan.AdmiralRoom.API
{
    public class getmember_deck
    {
        public int api_member_id { get; set; }
        public int api_id { get; set; }
        public string api_name { get; set; }
        //public int api_name_id { get; set; }
        public long[] api_mission { get; set; }//[0]={0=未出撃, 1=遠征中, 2=遠征帰投, 3=強制帰投中}, [1]=遠征先ID, [2]=帰投時間, [3]=0
        public int api_flagship { get; set; }
        public int[] api_ship { get; set; }
    }
}
