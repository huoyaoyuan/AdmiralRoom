using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;

namespace Huoyaoyuan.AdmiralRoom.API
{
    [Serializable]
    public class svdata<T>
    {
        public int api_result;
        public string api_result_msg;
        public T api_data = default(T);
        [NonSerialized]
        private static DataContractJsonSerializer _serializer = null;
        public static DataContractJsonSerializer Serializer
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
