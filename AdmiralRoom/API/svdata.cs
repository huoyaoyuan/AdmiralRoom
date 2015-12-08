using System.Runtime.Serialization.Json;

namespace Huoyaoyuan.AdmiralRoom.API
{
    public class svdata
    {
        public int api_result { get; set; }
        public string api_result_msg { get; set; }
    }
    public class svdata<T> : svdata
    {
        public T api_data { get; set; }
    }
}
