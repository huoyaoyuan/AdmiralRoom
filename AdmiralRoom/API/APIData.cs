using System.Collections.Specialized;
using System.Web;

namespace Huoyaoyuan.AdmiralRoom.API
{
    public class APIData
    {
        public svdata SvData { get; private set; }
        public NameValueCollection Request { get; private set; }
        public bool IsSuccess => this.SvData.api_result == 1;
        public APIData(svdata data, string request)
        {
            SvData = data;
            Request = HttpUtility.ParseQueryString(request);
        }
    }
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
