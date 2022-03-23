using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Text;
using System.Xml;
using System.Diagnostics;
using DJCRYPTLib;
using System.IO;
using System.Security.Cryptography;

namespace XQLiteMgm.Helper
{
    public class Crypt
    {
        public Crypt()
        {
        }
        private static string strKey = "just1234";
        private static string strIV = "just1234";

        /// <summary>信用卡字串編碼</summary>
        /// <param name="strSource">原始字串</param>
        /// <returns>編碼後的結果字串</returns>
        public static string enCryptCredit(string strSource)
        {
            if (strSource != null)
            {
                try
                {
                    MemoryStream ms = new MemoryStream();
                    DESCryptoServiceProvider key = new DESCryptoServiceProvider();
                    CryptoStream encStream = new CryptoStream(ms, key.CreateEncryptor(Encoding.Default.GetBytes(strKey), Encoding.Default.GetBytes(strIV)), CryptoStreamMode.Write);
                    StreamWriter sw = new StreamWriter(encStream);
                    sw.WriteLine(strSource);
                    sw.Close();
                    encStream.Close();
                    byte[] buffer = ms.ToArray();
                    ms.Close();
                    return Convert.ToBase64String(buffer).Replace("+", "@");
                }
                catch
                {
                    return "fail";
                }

            }
            else
            {
                return "fail";
            }
        }

        /// <summary>信用卡字串解碼</summary>
        /// <param name="strSource">加密過的字串</param>
        /// <returns>解碼後的結果字串</returns>
        public static string deCryptCredit(string strSource)
        {
            string val = null;
            strSource = strSource.Replace("@", "+");
            if (strSource != null)
            {
                try
                {
                    MemoryStream ms = new MemoryStream(Convert.FromBase64String(strSource));
                    DESCryptoServiceProvider key = new DESCryptoServiceProvider();
                    CryptoStream encStream = new CryptoStream(ms, key.CreateDecryptor(Encoding.Default.GetBytes(strKey), Encoding.Default.GetBytes(strIV)), CryptoStreamMode.Read);
                    StreamReader sr = new StreamReader(encStream);
                    val = sr.ReadLine();
                    sr.Close();
                    encStream.Close();
                    ms.Close();
                }
                catch
                {
                    return "fail";
                }
            }

            return val;
        }

        // Encrypt password
        //
        static ASCIIEncoding encoding = new ASCIIEncoding();
        static string[] _SecretTable = new string[]
              {
              "When the holy Bodhisattva Avalokitesvara had truly grasped the transcendent wisdom",
              "he realized that visible form is only illusion.",
              "The same applies to its perception, to its names and categories",
              "to discriminative intellect and finally even to our consciousness.",
              "They are all illusion. With this realizaton he was beyond all sorrow and bitterness.",
              "Disciple Sariputra!",
              "The material is not different from the immaterial.",
              "The immaterial and the material are in fact one and the same thing.",
              "The same applies to perception, concepts, discriminative thinking and consciousness.",
              "They are neither existing nor not existing.",
              "Sariputra! All things therefore they are in themselves not good and not bad",
              "they are not increasing and not decreasing.",
              "Therefore one may say there are no such things as form, perception, concepts,",
              "thinking process, and consciousness.",
              "Our senses such as eye, ear, nose, tongue, body and mind are misleading us to illusion;",
              "thus one may also say there is no reality in visible form, sound, smell, taste, touch and mindknowledge.",
              "There are also no such things as the realms of sense from sight up to mind,",
              "and no such things as the links of existence from ignorance and its end to old age and death and their end",
              "Also the Four Noble Truths* are nonexistent",
              "just as there is no such thing as wisdom and also no gain.",
              "Because the holy Bodhisattva who relies on transcendent wisdom knows that there is no gain",
              "he has no worries and also no fear. Beyond all illusion he has reached the space of highest Nirvana",
              "All Buddhas of the past, present and future,",
              "found highest perfect knowedge because they relied on transcendental wisdom.",
              "Therefore we ought to know that the great verse of the transcendent wisdom is unsurpassed in its splendor",
              "and that it appeases truly all pain",
              "GATE, GATE, PARAGATE, PARASAMGATE BODHISVAHA",
              "Go, do it, make the step, you all must break through to the Absolute,",
              "Four Noble Truths: Life means suffering",
              "The origin of suffering is attachment",
              "The cessation of suffering is attainable; The path to the cessation of suffering",
              "pain: The more appropriate word may be ",
            };

        public static string DJEncrypt(string str, string sTime, string sKey = "6384E9BA-DEAE-4f50-BF55-F56B45D79D81")
        {
            string sResult = "";
            try
            {
                EncryptClass oCrypt = new EncryptClass();
                oCrypt.Init();

                string sSeed = oCrypt.GenSeed(sTime + sKey);
                sResult = oCrypt.EncryptMessage(sSeed, str);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return sResult;
        }
        public static string DJDecrypt(string str, string sTime, string sKey = "6384E9BA-DEAE-4f50-BF55-F56B45D79D81")
        {
            string sResult = "";
            try
            {
                EncryptClass oCrypt = new EncryptClass();
                oCrypt.Init();

                string sSeed = oCrypt.GenSeed(sTime + sKey);
                sResult = oCrypt.DecryptMessage(sSeed, str);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return sResult;
        }

        public static string EncryptPwdIC(string sPwd, string sTime)
        {
            string sResult = "";
            try
            {
                EncryptClass oCrypt = new EncryptClass();
                oCrypt.Init();



                string sSeed = oCrypt.GenSeed("IC" + sTime);
                sResult = oCrypt.EncryptMessage(sSeed, sPwd);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return sResult;
        }

        public static string EncryptPwd(string sPwd, string sTime)
        {
            string sResult = "";
            try
            {
                EncryptClass oCrypt = new EncryptClass();
                oCrypt.Init();

                string sKey = ConfigurationManager.AppSettings["CryptLoginKey"];
                if (sKey.Length <= 0)
                    sKey = "3CBC1A2B-7E72-4b38-A8C8-7F079DEE736D";

                string sSeed = oCrypt.GenSeed(sTime + sKey);
                sResult = oCrypt.EncryptMessage(sSeed, sPwd);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return sResult;
        }

        public static string DecryptPwd(string sPwd, string sTime, string sCryptSeed)
        {
            string sResult = "";
            try
            {
                EncryptClass oCrypt = new EncryptClass();
                oCrypt.Init();

                string sKey = ConfigurationManager.AppSettings["CryptLoginKey"];
                if (sKey.Length <= 0)
                    sKey = "3CBC1A2B-7E72-4b38-A8C8-7F079DEE736D";

                string sSeed = oCrypt.GenSeed(sTime + sKey);
                if (sCryptSeed.Length > 0)
                {
                    //Debug.Assert(sCryptSeed == sSeed);
                    if (sCryptSeed != sSeed)
                    {
                        Debug.WriteLine("Crypt Different Seed");
                    }
                }
                sResult = oCrypt.DecryptMessage(sSeed, sPwd);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return sResult;
        }

        public static string DecryptData(byte[] arrData, string strKey, string strMethod)
        {
            // strKey = TDate - Number
            //	e.g. "20070911-105"
            // 

            string sResult = "";
            try
            {
                EncryptClass oCrypt = new EncryptClass();
                oCrypt.Init();

                string[] arrToken = strKey.Split('-');
                if (arrToken.Length != 2)
                    throw new Exception("Invalid KeyFormat");

                DateTime date;
                int nSecretIndex;
                try
                {
                    date = DateTime.ParseExact(arrToken[0], "yyyyMMdd", null);
                    nSecretIndex = Int32.Parse(arrToken[1]);
                }
                catch
                {
                    throw new Exception("Invalid KeyFormat: format incorrect");
                }

                TimeSpan ts = date - DateTime.Today;
                if (ts.TotalDays > 2)
                    throw new Exception("Invalid KeyFormat: Date is invalid");

                string strSecred = _SecretTable[nSecretIndex % _SecretTable.Length];

                string strSeed = oCrypt.GenSeed(date.ToString("yyyyMMdd") + "-" + nSecretIndex.ToString() + "-" + strSecred);

                //string strData = encoding.GetString(arrData);
                sResult = oCrypt.DecryptData(strSeed, arrData);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                // Error handling
                //
            }
            return sResult;
        }

        //public static string EnCryptData(byte[] sOriData, string strKey)
        //{
        //    ASCIIEncoding encoding = new ASCIIEncoding();
        //    string sOriStr = encoding.GetString(sOriData);
        //    return EnCryptData(sOriStr, strKey);
        //}
        public static byte[] EnCryptData(byte[] arrData, string strKey, string strMethod)
        {
            // strKey = TDate - Number
            //	e.g. "20070911-105"
            // 
            //ASCIIEncoding encoding=new ASCIIEncoding();
            //byte[] arrData = encoding.GetBytes(sOriData);

            //string sResult = "";
            byte[] dataResult = null;
            try
            {
                EncryptClass oCrypt = new EncryptClass();
                oCrypt.Init();

                string[] arrToken = strKey.Split('-');
                if (arrToken.Length != 2)
                    throw new Exception("Invalid KeyFormat");

                DateTime date;
                int nSecretIndex;
                try
                {
                    date = DateTime.ParseExact(arrToken[0], "yyyyMMdd", null);
                    nSecretIndex = Int32.Parse(arrToken[1]);
                }
                catch
                {
                    throw new Exception("Invalid KeyFormat: format incorrect");
                }

                TimeSpan ts = date - DateTime.Today;
                if (ts.TotalDays > 2)
                    throw new Exception("Invalid KeyFormat: Date is invalid");

                string strSecred = _SecretTable[nSecretIndex % _SecretTable.Length];

                string strSeed = oCrypt.GenSeed(date.ToString("yyyyMMdd") + "-" + nSecretIndex.ToString() + "-" + strSecred);
                string strData = encoding.GetString(arrData);

                dataResult = (byte[])oCrypt.EncryptData(strSeed, strData);



            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                //LogSvc.LogInfo("[EnCryptData]:" + encoding.GetString(arrData) + " [Key]:" + strKey);
                // Error handling
                //
            }
            return dataResult;

        }
    }
}