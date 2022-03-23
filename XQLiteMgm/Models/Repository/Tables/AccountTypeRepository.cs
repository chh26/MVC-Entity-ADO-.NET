using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using XQLiteMgm.Models.Interface;
using XQLiteMgm.Models.UnitOfWork;
using XQLiteMgm.Models.UnitOfWork.Interface;
using XQLiteMgm.Helper;

namespace XQLiteMgm.Models.Repository.Tables
{
    public class AccountTypeRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public AccountTypeRepository()
        {

        }

        public AccountTypeRepository(IUnitOfWork iuow, string _userid)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }

            this.userid = _userid;
        }

        string ICAuth = string.Empty;
        string userid = string.Empty;
        //券商驗證用
        /** accountType:
        * for XQ: null
        * for XQLite "": 沒有券商驗證或券商驗證失敗,"1": 付費, "2": 校園, "3": XQ Trade, "4":"驗證成功但非前三種case"
        */
        public string accountType = string.Empty;
        /** accountOrderType:
        * for XQ: null
        * for XQLite "": 沒有券商驗證或券商驗證失敗, 1": 付費, "2": 校園, "3": XQ Trade, "4":"驗證成功但非前三種case", "5":只購買台股
        */
        public string accountOrderType = string.Empty;

        public UserVerifyBroker UserVerifyBroker = new UserVerifyBroker();

        public class ICResult
        {
            public Boolean Success { get; set; }
            public string Msg { get; set; }
        }

        public void GetAccountType(bool isXQLite)
        {
            string verifyBroker_isEnable = ConfigurationManager.AppSettings["VerifyBroker_isEnable"];

            if (isXQLite && verifyBroker_isEnable == "Y")
            {
                UserVerifyBroker = GetUserVerifyBroker(userid);

                //檢查用戶是否有VerifyBroker
                if (!string.IsNullOrEmpty(UserVerifyBroker.TSID))
                {
                    //檢查日期是否在有效之內
                    if (UserVerifyBroker.STATUS.ToUpper() == "SUCCESS")
                    {
                        if (UserVerifyBroker.TSID == "SCHOOL")
                        {
                            accountType = "2";
                        }
                        else if (UserVerifyBroker.TSID == "XQTRADE")
                        {
                            accountType = "3";
                        }
                        else
                        {
                            accountType = "4";
                        }

                    }
                }

                //檢查用戶是否有付費          
                List<string> PayOpidList = GetPayOpidForXQLite(userid);

                if (PayOpidList.Count > 0)
                {
                    if (ConfigurationManager.AppSettings["marketing_Date"] != "")
                    {
                        if (Convert.ToDateTime(ConfigurationManager.AppSettings["marketing_Date"]) > DateTime.Now)
                        {
                            if (PayOpidList.Count == 1 && PayOpidList.Contains("US"))
                            {
                                DataSet freeDS = UserXQLitePayRecord_FreeTrial(userid);
                                if (freeDS != null && freeDS.Tables.Count > 0)
                                {
                                    if (freeDS.Tables[0].Rows.Count > 0)
                                    {
                                        var dr = freeDS.Tables[0].Select("cardSEQNO='888'");
                                        if (dr.Length <= 0)
                                        {
                                            dr = freeDS.Tables[0].Select("cardSEQNO='889'");
                                            if (dr.Length > 0)
                                            {
                                                if (accountType != "")
                                                {
                                                    accountOrderType = accountType;
                                                    accountType = "999";
                                                }
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    accountOrderType = accountType;
                    accountType = "1";

                    string buyTWOpid = string.Join(",", PayOpidList.FindAll(item => item.Contains("TW")).ToArray());
                    if (!string.IsNullOrEmpty(buyTWOpid))
                    {
                        List<string> withoutTWList = PayOpidList.FindAll(item => !item.Contains("TW") && !item.Contains("USD"));

                        if (withoutTWList.Count > 0)
                        {
                            accountOrderType = accountType + "@" + buyTWOpid;
                        }
                        else
                        {
                            accountOrderType += "@" + buyTWOpid; //新舊台股並存
                        }
                    }
                    else if (PayOpidList.Count == 1 && PayOpidList.Contains("USD"))
                    {
                        //只存在美股延遲模組
                        if (PayOpidList.Count == 1 && PayOpidList.Contains("USD"))
                        {
                            accountOrderType = accountType + "@USD";
                        }
                    }
                }
                else if (ConfigurationManager.AppSettings["ICGetTW"] == "Y")
                {
                    //如果都沒購買模組，要判斷是不是IC用戶
                    if (ICAuth == "2")
                    {
                        accountType = "5";
                        accountOrderType = accountType;
                    }
                }
            }
        }

        public UserVerifyBroker GetUserVerifyBroker(string UserID)
        {
            UserVerifyBroker userverifybroker = new UserVerifyBroker();

            using (var uow = UnitOfWorkFactory.Create())
            {
                UserMaintenanceRepository userMaintenRepos = new UserMaintenanceRepository(uow);
                DataSet ds = userMaintenRepos.UserXQLiteVerifyBroker_SEL(UserID);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string userid = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    string tsid = ds.Tables[0].Rows[0].ItemArray[1].ToString();
                    string verifyDate = ds.Tables[0].Rows[0].ItemArray[2].ToString();
                    string status = ds.Tables[0].Rows[0].ItemArray[3].ToString();
                    string name = ds.Tables[0].Rows[0].ItemArray[4].ToString();

                    if (ds.Tables[0].Rows.Count > 1 &&
                        ((ds.Tables[0].Rows[0].ItemArray[1].ToString() != "SCHOOL") && (ds.Tables[0].Rows[0].ItemArray[1].ToString() != "XQTRADE")))
                    {
                        DataRow[] dr = ds.Tables[0].Select("TSID='SCHOOL' AND Status='Success'");
                        DataRow[] dr2 = ds.Tables[0].Select("TSID='XQTRADE' AND Status='Success'");
                        if (dr.Length > 0)//校園權限
                        {
                            tsid = dr[0].ItemArray[1].ToString();
                            verifyDate = dr[0].ItemArray[2].ToString();
                            status = dr[0].ItemArray[3].ToString();
                            name = dr[0].ItemArray[4].ToString();
                        }
                        else if (dr2.Length > 0)//XQTrade權限
                        {
                            tsid = dr2[0].ItemArray[1].ToString();
                            verifyDate = dr2[0].ItemArray[2].ToString();
                            status = dr2[0].ItemArray[3].ToString();
                            name = dr2[0].ItemArray[4].ToString();
                        }
                        else//有成功的券商驗證，沒有的話預設是最後一筆資料
                        {
                            DataRow[] dr3 = ds.Tables[0].Select("Status='Success'");
                            if (dr3.Length > 0)
                            {
                                tsid = dr3[0].ItemArray[1].ToString();
                                verifyDate = dr3[0].ItemArray[2].ToString();
                                status = dr3[0].ItemArray[3].ToString();
                                name = dr3[0].ItemArray[4].ToString();
                            }
                        }

                    }
                    userverifybroker.USERID = userid;
                    userverifybroker.TSID = tsid;
                    userverifybroker.VERIFYBROKERDATE = verifyDate;
                    userverifybroker.STATUS = status;
                    userverifybroker.NAME = name;
                }
            }
            return userverifybroker;
        }

        internal void GetICAuth()
        {
            Dictionary<string, string> cardServiceData = new Dictionary<string, string>();

            string result = "";
            string datetimex = DateTime.Now.ToString("yyyyMMddHHmmss");

            string id = Crypt.EncryptPwdIC(userid, datetimex);
            cardServiceData.Add("id", id);
            cardServiceData.Add("datetime", datetimex);
            string data = JsonConvert.SerializeObject(cardServiceData);
            var webAddr = ConfigurationManager.AppSettings["ICUserAuthor"];
            try
            {
                if (webAddr == "")
                {
                    ICAuth = "0";
                }
                else
                {
                    result = PostRequest(webAddr, data);
                    ICResult icresult = JsonConvert.DeserializeObject<ICResult>(result);
                    if (icresult.Success && icresult.Msg != "0")
                    {
                        ICAuth = icresult.Msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得所有購買模組
        /// </summary>
        /// <param name="UserID"></param>
        public List<string> GetPayOpidForXQLite(string UserID)
        {
            List<string> opidList = new List<string>();


            DataSet ds = UserXQLiteOrderAuth_SEL(UserID);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Select("IsValid='Y'"))
                {
                    opidList.Add(dr["authid"].ToString());
                }
            }
            if (opidList.Count < 1)
            {

                ds = UserXQLiteExtraAuthNoDefault_SEL(UserID);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    opidList.Add(dr["authid"].ToString());
                }

            }

            return opidList;
        }

        private DataSet UserXQLiteOrderAuth_SEL(string userID)
        {
            DataSet ds = new DataSet();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"UserXQLiteOrderAuth_SEL";

                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@userid";
                paramText.Value = userID;
                cmd.Parameters.Add(paramText);

                ds.Load(cmd.ExecuteReader(), LoadOption.OverwriteChanges, "");
            }

            return ds;
        }

        public DataSet UserXQLiteExtraAuthNoDefault_SEL(string userID)
        {
            DataSet ds = new DataSet();
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"UserXQLiteExtraAuthNoDefault_SEL";

                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@userid";
                paramText.Value = userID;
                cmd.Parameters.Add(paramText);

                ds.Load(cmd.ExecuteReader(), LoadOption.OverwriteChanges, "");
            }

            return ds;
        }

        private DataSet UserXQLitePayRecord_FreeTrial(string userID)
        {
            DataSet ds = new DataSet();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"UserXQLitePayRecord_FreeTrial";

                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@userid";
                paramText.Value = userID;
                cmd.Parameters.Add(paramText);

                ds.Load(cmd.ExecuteReader(), LoadOption.OverwriteChanges, "");
            }

            return ds;
        }

        /// <summary>
        /// post 網頁+資料
        /// </summary>
        /// <param name="webAddr"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string PostRequest(string webAddr, string data)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 5000;


            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }

        }

        

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public void SaveChanges()
        {
            _unitOfWork.SaveChanges();
        }
    }
}