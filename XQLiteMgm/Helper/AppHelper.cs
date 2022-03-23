using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace XQLiteMgm.Helper
{
    public class AppHelper
    {
        public static string HttpWebRequestGet(string apiAddress, string data)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiAddress);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }

        }

        public static string HttpWebRequestPost(string apiAddress, string data)
        {
            string result = string.Empty;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiAddress);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Accept = "application/xml";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = false;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
                streamWriter.Flush();
                streamWriter.Close();
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    result = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                }
            }

            return result.Trim();
        }

        public static string GetIPAddress()
        {
            string sIP = "";
            sIP = GetHeader("DAC-IP");

            if (sIP == "")
                sIP = GetClientIP();
            if (sIP == "")
                sIP = "127.0.0.1";

            return sIP;
        }

        private static string GetHeader(string sKey)
        {
            HttpContext m_ctx = null;

            if (m_ctx == null)
            {
                return "";
            }
            string sValue = m_ctx.Request.Headers.Get(sKey);
            if (sValue == null)
            {
                // For debugging purpose, it's ok to send header value thru
                // query string
                //
                sValue = m_ctx.Request.QueryString[sKey];
                if (sValue == null)
                    return "";
            }
            return sValue;
        }

        private static string GetClientIP()
        {
            return GetClientIPv4();
        }

        /// <summary>
        /// 獲取訪問客戶端的IPV4地址
        /// </summary>
        /// <returns></returns>
        public static string GetClientIPv4()
        {
            string ipv4 = String.Empty;
            foreach (IPAddress ip in Dns.GetHostAddresses(GetClientIP_Puls()))
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    ipv4 = ip.ToString();
                    break;
                }
            }

            if (ipv4 != String.Empty)
            {
                return ipv4;
            }
            // 利用 Dns.GetHostEntry 方法,由獲取的 IPv6 位址反查 DNS 紀錄,
            // 再逐一判斷何者為 IPv4 協議,即可轉為 IPv4 位址。
            foreach (IPAddress ip in Dns.GetHostEntry(GetClientIP_Puls()).AddressList)
            //foreach (IPAddress ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    ipv4 = ip.ToString();
                    break;
                }
            }

            return ipv4;
        }
        private static string GetClientIP_Puls()
        {
            string sIP = string.Empty;
            if (null == HttpContext.Current.Request.ServerVariables["HTTP_VIA"])
            {
                sIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                sIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            if (string.IsNullOrEmpty(sIP))
                sIP = HttpContext.Current.Request.UserHostAddress;
            return sIP;
        }
    }
}