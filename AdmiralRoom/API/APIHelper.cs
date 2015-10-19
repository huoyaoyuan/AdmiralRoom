﻿using System;
using System.Diagnostics;
using System.IO;
using Fiddler;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom
{
    public static class APIHelper
    {
        public static APIData Parse(this Session oSession)
        {
            var resarr = oSession.ResponseBody;
            var serializer = svdata.Serializer;
            svdata _svdata = null;
            using (var mms = new MemoryStream(resarr, 7, resarr.Length - 7, false))
            {
                try
                {
                    _svdata = serializer.ReadObject(mms) as svdata;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    var apistring = (new System.Text.UTF8Encoding()).GetString(mms.ToArray());
                    Debug.WriteLine(apistring);
                    Debugger.Break();
                    _svdata = new svdata();
                }
            }
            return new APIData(_svdata, oSession.GetRequestBodyAsString());
        }
        public static APIData<T> Parse<T>(this Session oSession)
        {
            var resarr = oSession.ResponseBody;
            var serializer = svdata<T>.Serializer;
            svdata<T> svdata = null;
            using (var mms = new MemoryStream(resarr, 7, resarr.Length - 7, false))
            {
                try
                {
                    svdata = serializer.ReadObject(mms) as svdata<T>;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    var apistring = (new System.Text.UTF8Encoding()).GetString(mms.ToArray());
                    Debug.WriteLine(apistring);
                    Debugger.Break();
                    svdata = new svdata<T>();
                }
            }
            return new APIData<T>(svdata, oSession.GetRequestBodyAsString());
        }
    }
}
