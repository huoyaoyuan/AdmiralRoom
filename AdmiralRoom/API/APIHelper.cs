using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Codeplex.Data;
using Fiddler;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom
{
    public static class APIHelper
    {
        private static void APIError(Exception ex) => Officer.Staff.Current.Dispatcher.Invoke(() => System.Windows.MessageBox.Show(ex.Message));
        public static int GetInt(this NameValueCollection req, string name)
        {
            return int.Parse(req[name]);
        }
        public static IEnumerable<int> GetInts(this NameValueCollection req, string name)
        {
            var str = req[name];
            return str.Split(',').Select(x => int.Parse(x));
        }
        public static APIData Parse(this Session oSession)
        {
            var serializer = svdata.Serializer;
            svdata _svdata = null;
            using (var mms = new MemoryStream(oSession.ResponseBody, 7, oSession.ResponseBody.Length - 7, false))
            {
                try
                {
                    _svdata = serializer.ReadObject(mms) as svdata;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    var apistring = (new UTF8Encoding()).GetString(mms.ToArray());
                    Debug.WriteLine(apistring);
                    Debugger.Break();
                    throw;
                }
            }
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
            var serializer = svdata<T>.Serializer;
            svdata<T> svdata = null;
            using (var mms = new MemoryStream(oSession.ResponseBody, 7, oSession.ResponseBody.Length - 7, false))
            {
                try
                {
                    svdata = serializer.ReadObject(mms) as svdata<T>;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    var apistring = (new UTF8Encoding()).GetString(mms.ToArray());
                    Debug.WriteLine(apistring);
                    Debugger.Break();
                    APIError(ex);
                    throw ex;
                }
            }
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
        public static APIData<dynamic> ParseDynamic(this Session oSession)
        {
            dynamic res;
            svdata<dynamic> svdata = new svdata<dynamic>();
            using (var mms = new MemoryStream(oSession.ResponseBody, 7, oSession.ResponseBody.Length - 7, false))
            {
                res = DynamicJson.Parse(mms);
            }
            svdata.api_data = res.api_data;
            svdata.api_result = (int)res.api_result;
            svdata.api_result_msg = res.api_result_msg;
            return new APIData<dynamic>(svdata, oSession.GetRequestBodyAsString());
        }
        public static bool TryParseDynamic(this Session oSession, out APIData<dynamic> result)
        {
            try
            {
                result = oSession.ParseDynamic();
                return result.IsSuccess;
            }
            catch
            {
                result = null;
                return false;
            }
        }
        public static APIData<T> ParseDummy<T>(this Session oSession)
        {
            var serializer = svdata<T>.Serializer;
            svdata<T> svdata = null;
            string dummystr = oSession.GetResponseBodyAsString().Substring(7).Replace("[-1,", "[").Replace("[-1]", "null");
            using (var mms = new MemoryStream((new UTF8Encoding()).GetBytes(dummystr), false))
            {
                try
                {
                    svdata = serializer.ReadObject(mms) as svdata<T>;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    var apistring = (new UTF8Encoding()).GetString(mms.ToArray());
                    Debug.WriteLine(apistring);
                    Debugger.Break();
                    APIError(ex);
                    throw ex;
                }
            }
            return new APIData<T>(svdata, oSession.GetRequestBodyAsString());
        }
        public static bool TryParseDummy<T>(this Session oSession, out APIData<T> result)
        {
            try
            {
                result = oSession.ParseDummy<T>();
                return result.IsSuccess;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
