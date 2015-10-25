using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Codeplex.Data;
using Fiddler;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom
{
    public static class APIHelper
    {
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
                    _svdata = new svdata();
                }
            }
            return new APIData(_svdata, oSession.GetRequestBodyAsString());
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
                    App.Current.MainWindow.Dispatcher.Invoke(() => System.Windows.MessageBox.Show(ex.Message));
                    svdata = new svdata<T>();
                }
            }
            return new APIData<T>(svdata, oSession.GetRequestBodyAsString());
        }
        public static int GetInt(this NameValueCollection req, string name)
        {
            return int.Parse(req[name]);
        }
        public static IEnumerable<int> GetInts(this NameValueCollection req, string name)
        {
            var str = req[name];
            return str.Split(',').ArrayOperation(x => int.Parse(x));
        }
        public static APIData<getmember_questlist> ParseQuest(this Session oSession)
        {
            dynamic res;
            svdata<getmember_questlist> svdata = new svdata<getmember_questlist>();
            using (var mms = new MemoryStream(oSession.ResponseBody, 7, oSession.ResponseBody.Length - 7, false))
            {
                res = DynamicJson.Parse(mms, new UTF8Encoding());
            }
            svdata.api_result = Convert.ToInt32(res.api_result);
            svdata.api_result_msg = Convert.ToString(res.api_result_msg);
            getmember_questlist data = new getmember_questlist
            {
                api_count = Convert.ToInt32(res.api_data.api_count),
                api_disp_page = Convert.ToInt32(res.api_data.api_disp_page),
                api_page_count = Convert.ToInt32(res.api_data.api_page_count),
                api_exec_count = Convert.ToInt32(res.api_data.api_exec_count),
            };
            try
            {
                var list = new List<api_quest>();
                var serializer = new DataContractJsonSerializer(typeof(api_quest));
                foreach (var x in (object[])res.api_data.api_list)
                {
                    var mms = new MemoryStream(Encoding.UTF8.GetBytes(x.ToString()));
                    try
                    {
                        list.Add(serializer.ReadObject(mms) as api_quest);
                    }
                    catch { }
                    finally { mms.Dispose(); }
                }
                data.api_list = list.ToArray();
            }
            catch { }
            svdata.api_data = data;
            return new APIData<getmember_questlist>(svdata, oSession.GetRequestBodyAsString());
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
            svdata.api_result = res.api_result;
            svdata.api_result_msg = res.api_result_msg;
            return new APIData<dynamic>(svdata, oSession.GetRequestBodyAsString());
        }
    }
}
