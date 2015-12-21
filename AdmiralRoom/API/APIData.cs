using System.Collections.Specialized;
using System.Web;

namespace Huoyaoyuan.AdmiralRoom.API
{
    public class APIData
    {
        public svdata SvData { get; }
        public NameValueCollection Request { get; }
        public bool IsSuccess => this.SvData.api_result == 1;
        public APIData(svdata data, string request)
        {
            SvData = data;
            Request = HttpUtility.ParseQueryString(request);
        }
    }
    public class APIData<T>
    {
        public svdata<T> SvData { get; }
        public T Data => this.SvData.api_data;
        public NameValueCollection Request { get; }
        public bool IsSuccess => this.SvData.api_result == 1;
        public APIData(svdata<T> data, string request)
        {
            SvData = data;
            Request = HttpUtility.ParseQueryString(request);
        }
    }
}
