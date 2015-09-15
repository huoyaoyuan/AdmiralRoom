using System.IO;
using Fiddler;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom
{
    public static class APIHelper
    {
        public static APIData<T> Parse<T>(this Session oSession)
        {
            var resarr = oSession.ResponseBody;
            var mms = new MemoryStream(resarr, 7, resarr.Length - 7, false);
            var serializer = svdata<T>.Serializer;
            var svdata = serializer.ReadObject(mms) as svdata<T>;
            return new APIData<T>(svdata, oSession.GetRequestBodyAsString());
        }
    }
}
