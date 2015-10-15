using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Huoyaoyuan.AdmiralRoom.API
{
    public class APIData<T>
    {
        public svdata<T> SvData { get; private set; }
        public T Data => this.SvData.api_data;
        public NameValueCollection Request { get; private set; }
        public bool IsSuccess => this.SvData.api_result == 1;
        public APIData(svdata<T> data, string request)
        {
            SvData = data;
            Request = HttpUtility.ParseQueryString(request);
        }
    }
}
