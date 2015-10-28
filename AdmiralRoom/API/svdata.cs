using System;
using System.Runtime.Serialization.Json;

namespace Huoyaoyuan.AdmiralRoom.API
{
    [Serializable]
    public class svdata
    {
        public int api_result;
        public string api_result_msg;
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
    [Serializable]
    public class svdata<T> : svdata
    {
        public T api_data = default(T);
        [NonSerialized]
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
