using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Fiddler;
using Huoyaoyuan.AdmiralRoom.API;
using Newtonsoft.Json;

namespace Huoyaoyuan.AdmiralRoom
{
    public class CachedSession
    {
        public Session Session { get; }
        public string Request { get; }
        public string Response { get; }
        public string JsonResponse { get; }
        public APIData CachedAPI { get; private set; }
        public CachedSession(Session oSession)
        {
            Session = oSession;
            Request = oSession.GetRequestBodyAsString();
            Response = oSession.GetResponseBodyAsString();
            JsonResponse = Response.Substring(7);
        }
        public bool TryParse(out APIData result)
        {
            if (CachedAPI != null)
            {
                result = CachedAPI;
                return result.IsSuccess;
            }
            try
            {
                result = this.Parse();
                CachedAPI = result;
                return result.IsSuccess;
            }
            catch
            {
                result = null;
                return false;
            }
        }
        public bool TryParse<T>(out APIData<T> result)
        {
            if (CachedAPI is APIData<T> data)
            {
                result = data;
                return result.IsSuccess;
            }
            try
            {
                result = this.Parse<T>();
                CachedAPI = result;
                return result.IsSuccess;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
    public static class APIHelper
    {
        private static void APIError(Exception ex) => DispatcherHelper.UIDispatcher.Invoke(() => System.Windows.MessageBox.Show(ex.Message));
        public static int GetInt(this NameValueCollection req, string name) => int.Parse(req[name]);
        public static IEnumerable<int> GetInts(this NameValueCollection req, string name)
        {
            var str = req[name];
            return str.Split(',').Select(int.Parse);
        }
        private static readonly JsonSerializer JSerializer = new JsonSerializer
        {
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        static APIHelper()
        {
            JSerializer.Error += (s, e) => e.ErrorContext.Handled = true;
        }
        public static APIData Parse(this CachedSession oSession)
        {
            svdata _svdata = null;
            var reader = new StringReader(oSession.JsonResponse);
            using (var jreader = new JsonTextReader(reader))
                _svdata = JSerializer.Deserialize<svdata>(jreader);
            return new APIData(_svdata, oSession.Request);
        }
        public static APIData<T> Parse<T>(this CachedSession oSession)
        {
            var reader = new StringReader(oSession.JsonResponse);
            return new APIData<T>(Parse<T>(reader), oSession.Request);
        }
        public static svdata<T> Parse<T>(TextReader reader)
        {
            using (var jreader = new JsonTextReader(reader))
                return JSerializer.Deserialize<svdata<T>>(jreader);
        }
    }
}
