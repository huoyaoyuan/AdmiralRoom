using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Fiddler;
using Huoyaoyuan.AdmiralRoom.API;
using Newtonsoft.Json;

#pragma warning disable CC0022

namespace Huoyaoyuan.AdmiralRoom
{
    public static class APIHelper
    {
        private static void APIError(Exception ex) => DispatcherHelper.UIDispatcher.Invoke(() => System.Windows.MessageBox.Show(ex.Message));
        public static int GetInt(this NameValueCollection req, string name) => int.Parse(req[name]);
        public static IEnumerable<int> GetInts(this NameValueCollection req, string name)
        {
            var str = req[name];
            return str.Split(',').Select(x => int.Parse(x));
        }
        private static readonly JsonSerializer JSerializer = new JsonSerializer
        {
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        static APIHelper()
        {
            JSerializer.Error += (s, e) => e.ErrorContext.Handled = true;
        }
        public static APIData Parse(this Session oSession)
        {
            svdata _svdata = null;
            var reader = new StringReader(oSession.GetResponseBodyAsString().Substring(7));
            using (var jreader = new JsonTextReader(reader))
                _svdata = JSerializer.Deserialize<svdata>(jreader);
            return new APIData(_svdata, oSession.GetRequestBodyAsString());
        }
        public static bool TryParse(this Session oSession, out APIData result)
        {
            try
            {
                result = oSession.Parse();
                return result.IsSuccess;
            }
            catch
            {
                result = null;
                return false;
            }
        }
        public static APIData<T> Parse<T>(this Session oSession)
        {
            svdata<T> svdata = null;
            var reader = new StringReader(oSession.GetResponseBodyAsString().Substring(7));
            using (var jreader = new JsonTextReader(reader))
                svdata = JSerializer.Deserialize<svdata<T>>(jreader);
            return new APIData<T>(svdata, oSession.GetRequestBodyAsString());
        }
        public static bool TryParse<T>(this Session oSession, out APIData<T> result)
        {
            try
            {
                result = oSession.Parse<T>();
                return result.IsSuccess;
            }
            catch
            {
                result = null;
                return false;
            }
        }
        //public static APIData<dynamic> ParseDynamic(this Session oSession)
        //{
        //    dynamic res;
        //    svdata<dynamic> svdata = new svdata<dynamic>();
        //    using (var mms = new MemoryStream(oSession.ResponseBody, 7, oSession.ResponseBody.Length - 7, false))
        //    {
        //        res = DynamicJson.Parse(mms);
        //    }
        //    svdata.api_data = res.api_data;
        //    svdata.api_result = (int)res.api_result;
        //    svdata.api_result_msg = res.api_result_msg;
        //    return new APIData<dynamic>(svdata, oSession.GetRequestBodyAsString());
        //}
        //public static bool TryParseDynamic(this Session oSession, out APIData<dynamic> result)
        //{
        //    try
        //    {
        //        result = oSession.ParseDynamic();
        //        return result.IsSuccess;
        //    }
        //    catch
        //    {
        //        result = null;
        //        return false;
        //    }
        //}
    }
}
