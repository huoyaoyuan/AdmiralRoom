using System;
using System.Diagnostics;
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
            svdata<T> svdata = null;
            try
            {
                svdata = serializer.ReadObject(mms) as svdata<T>;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debugger.Break();
                svdata = new svdata<T>();
            }
            return new APIData<T>(svdata, oSession.GetRequestBodyAsString());
        }
    }
}
