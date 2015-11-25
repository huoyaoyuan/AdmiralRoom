using System;
using System.Runtime.Serialization.Json;

namespace Huoyaoyuan.AdmiralRoom.API
{
    public class svdata
    {
        public int api_result { get; set; }
        public string api_result_msg { get; set; }
        private static DataContractJsonSerializer _serializer = null;
        public static DataContractJsonSerializer Serializer
        {
            get
            {
                if (_serializer == null)
                    _serializer = new DataContractJsonSerializer(typeof(svdata));
                return _serializer;
            }
        }
    }
    public class svdata<T> : svdata
    {
        public T api_data { get; set; }
        private static DataContractJsonSerializer _serializer = null;
        public new static DataContractJsonSerializer Serializer
        {
            get
            {
                if (_serializer == null)
                    _serializer = new DataContractJsonSerializer(typeof(svdata<T>));
                return _serializer;
            }
        }
    }
}
