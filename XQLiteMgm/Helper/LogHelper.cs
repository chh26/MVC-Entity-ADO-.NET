using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace XQLiteMgm.Helper
{
    public class LogHelper
    {
        //應用程式目錄
        private static String logPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "log";//Log目錄

        /// <summary>
        /// 寫log
        /// </summary>
        /// <param name="logMsg">要寫入的log資訊</param>
        public static void WriteLog(string paramters, string logMsg)
        {
            String logFileName = DateTime.Now.Year.ToString() + int.Parse(DateTime.Now.Month.ToString()).ToString("00") + int.Parse(DateTime.Now.Day.ToString()).ToString("00") + ".txt";
            String nowTime = int.Parse(DateTime.Now.Hour.ToString()).ToString("00") + ":" + int.Parse(DateTime.Now.Minute.ToString()).ToString("00") + ":" + int.Parse(DateTime.Now.Second.ToString()).ToString("00");

            if (!Directory.Exists(logPath))
            {
                //建立資料夾
                Directory.CreateDirectory(logPath);
            }

            if (!File.Exists(logPath + "\\" + logFileName))
            {
                //建立檔案
                File.Create(logPath + "\\" + logFileName).Close();
            }

            using (StreamWriter sw = File.AppendText(logPath + "\\" + logFileName))
            {
                sw.WriteLine("--Tim： " + nowTime + "--");
                sw.WriteLine("Client IP：" + AppHelper.GetIPAddress());
                if (!string.IsNullOrEmpty(paramters))
                {
                    sw.WriteLine("Paramters：");
                    sw.WriteLine(paramters);
                }

                if (!string.IsNullOrEmpty(logMsg))
                {
                    sw.WriteLine("Result：");
                    sw.WriteLine(logMsg);
                }

                sw.WriteLine();
                sw.Close();
            }
        }
    }
}