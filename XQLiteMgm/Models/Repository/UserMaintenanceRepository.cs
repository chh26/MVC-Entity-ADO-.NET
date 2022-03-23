using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using XQLiteMgm.Helper;
using XQLiteMgm.Models.Interface;
using XQLiteMgm.Models.Repository.Tables;
using XQLiteMgm.Models.UnitOfWork;
using XQLiteMgm.Models.UnitOfWork.Interface;

namespace XQLiteMgm.Models.Repository
{
    public class UserMaintenanceRepository : IRepository<UserBaseInfoViewModel>
    {
        private SQLUnitOfWork _unitOfWork;

        public UserMaintenanceRepository()
        {

        }

        public UserMaintenanceRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        /// <summary>
        /// 取得使用者資訊
        /// </summary>
        /// <param name="type"></param>
        /// <param name="queryText"></param>
        /// <returns></returns>
        public List<UserBaseInfoViewModel> GetUserBaseInfo(string type, string queryText)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                //cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"SELECT 
                                        distinct m.[name],
                                        d.userid,
                                        m.[mobile],
                                        [email],
                                        [cratedate],
                                        [opseq],
                                        ISNULL(TSID,'') AS TSID
                                    FROM 
                                        [CustomerMaster] m
                                    INNER JOIN CustomerDetail d ON m.custno = d.custno
                                    LEFT JOIN UserDealerBroker b ON d.userid = b.UserID
                                    WHERE 
                                        m.custno like 'XQLITE%' ";

                switch (type)
                {
                    case "user":
                        cmd.CommandText +=
                                    @"AND 
                                        (m.[name] LIKE '%' + @text + '%') ";
                        break;
                    case "mobile":
                        cmd.CommandText +=
                                    @"AND 
                                        (m.mobile LIKE '%' + @text + '%') ";
                        break;
                    case "email":
                        cmd.CommandText +=
                                    @"AND 
                                        (email LIKE '%' + @text + '%') ";
                        break;
                    default:
                        break;
                }

                cmd.CommandText += @"ORDER BY 
                                        [cratedate] desc";


                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@text";
                paramText.Value = queryText;
                cmd.Parameters.Add(paramText);

                List<UserBaseInfoViewModel> infoList = new List<UserBaseInfoViewModel>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UserBaseInfoViewModel info = new UserBaseInfoViewModel();

                        info.opseq = reader["opseq"].ToString();
                        info.name = reader["name"].ToString();
                        info.userid = reader["userid"].ToString();
                        info.mobile = reader["mobile"].ToString();
                        info.email = reader["email"].ToString();
                        info.cratedate = reader["cratedate"].ToString();
                        info.TSID = reader["TSID"].ToString();

                        infoList.Add(info);
                    }
                }

                return infoList;
            }
        }

        /// <summary>
        /// 取得點數券商資訊
        /// </summary>
        /// <returns></returns>
        public List<PointBrokerInfoViewModel> GetPointBrokerInfo()
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {

                cmd.CommandText = @"SELECT
                                        TSID
                                        ,TSName
                                        ,TSMemo
                                        ,AppID
                                        ,CreateTime
                                    FROM
                                        PointBrokerInfo ";

                List<PointBrokerInfoViewModel> infoList = new List<PointBrokerInfoViewModel>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PointBrokerInfoViewModel info = new PointBrokerInfoViewModel();

                        info.TSID = reader["TSID"].ToString();
                        info.TSName = reader["TSName"].ToString();
                        info.TSMemo = reader["TSMemo"].ToString();
                        info.AppID = reader["AppID"].ToString();

                        infoList.Add(info);
                    }
                }

                return infoList;
            }
        }

        #region 使用者帳號狀態明細
        public AccountDetailViewModel GetUserAccountDetail(string userid)
        {
            //UserOrderRecordRepository userOrderRecordRepos = new UserOrderRecordRepository(_unitOfWork);
            //List<UserOrderRecordViewModel> list = new List<UserOrderRecordViewModel>();
            //List<Productlist> productList = new List<Productlist>();

            //DataSet ds = UserXQLiteOrderAuth_SEL(userid);
            //if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //{
            //    foreach (DataRow dr in ds.Tables[0].Select("IsValid='Y'"))
            //    {
            //        Productlist product = new Productlist();
            //        product.product = dr["authname"].ToString();
            //        productList.Add(product);
            //    }
            //}

            AccountDetailViewModel status = new AccountDetailViewModel();

            status.productList = GetProductList(userid);
            status.brokerAccVerification = GetUserVerifyBroker(userid);
            status.freeTrial = GetUserFreeTrial(userid);
            status.tradeAccountSettingList = GetUserTradeAccSetting(userid);
            status.pointTradeAccList = GetUserPointTradeAcc(userid);
            status.balance = GetUserPointBlance(userid);
            return status;
        }

        public List<Productlist> GetProductList(string userid)
        {
            List<Productlist> productList = new List<Productlist>();
            List<UserOrderRecordViewModel> userOrderRecordList = new List<UserOrderRecordViewModel>();

            using (var uow = UnitOfWorkFactory.Create())
            {
                var userOrderRecordRepo = new UserOrderRecordRepository(uow);
                userOrderRecordList = userOrderRecordRepo.UserOrderRecord_SEL_Sub(userid);
            }

            foreach (var item in userOrderRecordList)
            {
                Productlist product = new Productlist();
                product.product = item.opseq;
                productList.Add(product);

            }

            return productList;
        }



        /// <summary>
        /// 取得user券商帳號驗證
        /// </summary>
        public string GetUserVerifyBroker(string userid)
        {
            string verifyBroker_isEnable = ConfigurationManager.AppSettings["VerifyBroker_isEnable"];
            string verifyBrokerAcc = "Y";
            string tsidName = string.Empty;

            //券商驗證用
            string accountType = string.Empty;
            string accountOrderType = string.Empty;
            string oldRole = string.Empty;
            string role = string.Empty;
            string userPowerText = string.Empty;
            DateTime? oldTWExpireTime = GetTWExpireTime();

            TWType twType = TWType.EOD;

            if (verifyBroker_isEnable == "Y")
            {
                UserVerifyBroker userverifybroker = new UserVerifyBroker();

                using (var uow = UnitOfWorkFactory.Create())
                {
                    AccountTypeRepository accType = new AccountTypeRepository(uow, userid);

                    if (ConfigurationManager.AppSettings["ICGetTW"] == "Y")
                        accType.GetICAuth();

                    accType.GetAccountType(true);
                    accountType = accType.accountType;
                    accountOrderType = accType.accountOrderType;
                    userverifybroker = accType.UserVerifyBroker;
                }

                #region 驗證券商是否要重新驗證
                if (!string.IsNullOrEmpty(userverifybroker.TSID)) //檢查用戶是否有VerifyBroker
                {
                    tsidName = userverifybroker.NAME;
                    //檢查日期是否在有效之內
                    if (userverifybroker.STATUS.ToUpper() == "SUCCESS")
                    {
                        //VerifyBrokerAcc="N"
                        verifyBrokerAcc = GetVerifyBrokerAcc(accountType);
                    }
                }
                #endregion

                if (accountType == "1" || accountType == "999")
                {
                    verifyBrokerAcc = "N";
                }
            }

            if (verifyBrokerAcc == "N")
            {
                switch (accountType)
                {
                    case "1":
                        role = "加值服務用戶";
                        twType = TWType.TW;
                        break;
                    case "2":
                        role = "校園帳號用戶";
                        twType = TWType.TW;
                        break;
                    case "3":
                        role = "已通過交易帳號驗證";
                        twType = TWType.TW5;
                        break;
                    case "4":
                        role = "已通過" + tsidName + "驗證";
                        twType = TWType.TW5;
                        break;
                    case "5":
                        role = "嘉實投顧勝率GO付費會員";
                        twType = TWType.TW;
                        break;
                    case "999":
                        role = "";
                        twType = TWType.EOD;
                        break;
                }

                oldRole = role;
            }

            string[] specialOrder = accountOrderType.Split('@');
            string specialOpidString = "";
            string[] specialOpidList = null;
            bool isOldTW = false;
            bool isNewTW = false;
            if (specialOrder.Length > 1)
            {
                specialOpidString = specialOrder[1];
                specialOpidList = specialOpidString.Split(',');
                isOldTW = Array.FindIndex(specialOpidList, item => item == "TW") > -1;
                isNewTW = specialOpidString.Contains("TW300");
                accountType = specialOrder[0]; //最原始的type
            }
            switch (specialOpidString)
            {
                case "TW300":
                    role = "台股即時模組用戶";
                    twType = TWType.TW;
                    break;
                case "USD":
                    role = "美股贈送已到期";
                    twType = TWType.TW5;
                    break;
                default:
                    if (isOldTW)
                    {
                        oldRole = "台股即時模組用戶";
                        if (accountType != "1" || isNewTW)
                            role = oldRole;
                        twType = TWType.TW;
                    }
                    break;
            }

            if (oldTWExpireTime == null || oldTWExpireTime > DateTime.Now)
            {
                //3.00.02前判斷 
                string freeTW = "";

                if (isOldTW)
                {
                    if (accountType == "1" && isOldTW)
                    {
                        freeTW = "TransPeriod";
                        oldRole = "已成為加值服務用戶";
                    }
                    else if (accountType == "3" && isOldTW)
                    {
                        freeTW = "TransPeriod";
                        oldRole = "已通過交易帳號驗證";
                    }
                }
                else
                {
                    if (accountType != "" && accountType != "999") freeTW = "FreeTW";
                }

                if (string.IsNullOrEmpty(oldRole))
                {
                    userPowerText = "!您尚未完成券商登入驗證，請您登出後再登入即可進行驗證。";
                }
                else if (freeTW == "FreeTW")
                {
                    userPowerText = $"{oldRole}，您已享有即時台股行情權限。";
                }
                else if (freeTW == "TransPeriod")
                {
                    userPowerText = $"台股即時模組用戶，{oldRole}，到期後免費享有即時台股行情權限。";
                }
                else
                {
                    userPowerText = oldRole;
                }
            }
            else
            {
                switch (twType)
                {
                    case TWType.EOD:
                        userPowerText = "!您尚未完成券商登入驗證，請您登出後再登入即可進行驗證。";
                        break;
                    case TWType.TW5:
                        userPowerText = role + "，台股行情為五秒更新版本";
                        break;
                    case TWType.TW:
                        if (isOldTW && accountType != "1")
                            userPowerText = role;
                        else if (isNewTW)
                        {
                            userPowerText = role;
                            if (accountType == "1") userPowerText += "，已成為加值服務用戶，到期後免費享有台股即時撮合行情";

                        }
                        else
                            userPowerText = role + "，您已享有台股即時撮合行情";
                        break;
                }
            }

            return userPowerText;
        }

        /// <summary>
        /// 取得美股贈送資訊
        /// 美股贈送有兩類
        /// 1.巴菲特專案-券商綁定，cardSEQNO：888
        /// 2.宅在家專案，cardSEQNO：889（預計8/1號結束）
        /// </summary>
        /// </summary>
        public Freetrial GetUserFreeTrial(string userid)
        {
            Freetrial freeTrial = new Freetrial();

            DataSet dsUserPayRecord_FreeTrial = UserXQLiteGetPayRecord_FreeTrial(userid);
            freeTrial.qualified = false;
            if (dsUserPayRecord_FreeTrial.Tables.Count > 0)
            {
                if (dsUserPayRecord_FreeTrial.Tables[0].Rows.Count > 0)
                {
                    freeTrial.qualified = true;
                    DataRow drUserPayRecord_FreeTrial = dsUserPayRecord_FreeTrial.Tables[0].Rows[0];

                    string cardSEQNO = drUserPayRecord_FreeTrial["cardSEQNO"].ToString();
                    freeTrial.cardSEQNO = cardSEQNO;

                    if (cardSEQNO == "888")
                    {
                        DataSet dsUserExchFreeTrial = UserXQLiteGetUserExchFreeTrial(userid);
                        if (dsUserExchFreeTrial.Tables.Count > 0)
                        {
                            if (dsUserExchFreeTrial.Tables[0].Rows.Count > 0)
                            {
                                DataRow drUserExchFreeTrial = dsUserExchFreeTrial.Tables[0].Rows[0];
                                freeTrial.brokerName = drUserExchFreeTrial["NAME"].ToString();

                                string tsid = drUserExchFreeTrial["TSID"].ToString();
                                string aid = drUserExchFreeTrial["AID"].ToString();

                                DataSet dsUserFreeTrial = UserXQLiteGetUserFreeTrial(userid, tsid, aid);
                                List<Freetriallist> freeTrialList = new List<Freetriallist>();

                                if (dsUserFreeTrial.Tables.Count > 0)
                                {
                                    if (dsUserFreeTrial.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow drFreeTrial in dsUserFreeTrial.Tables[0].Rows)
                                        {
                                            Freetriallist ft = new Freetriallist();
                                            ft.account = drFreeTrial["AID"].ToString();
                                            ft.sDate = drFreeTrial["paySdate"].ToString();
                                            ft.eDate = drFreeTrial["payEdate"].ToString();
                                            ft.lastDate = drFreeTrial["LastTrade"].ToString();

                                            freeTrialList.Add(ft);
                                        }
                                    }
                                }
                                freeTrial.freeTrialList = freeTrialList;
                            }
                        }
                    }
                }
            }

            return freeTrial;
        }

        public List<Tradeaccountsettinglist> GetUserTradeAccSetting(string userid)
        {
            List<Tradeaccountsettinglist> list = new List<Tradeaccountsettinglist>();
            DataSet dsTradeAccSetting = UserXQLiteGetUserTradeAccSetting(userid);
            foreach (DataRow drBroker in dsTradeAccSetting.Tables[0].Rows)
            {
                if (list.Count > 0)
                {
                    var checkBroker = list.AsEnumerable().Where(t => t.broker == drBroker["BrokerName"].ToString());

                    if (checkBroker.Count() > 0)
                        continue;
                }
                Tradeaccountsettinglist setting = new Tradeaccountsettinglist();
                setting.broker = drBroker["BrokerName"].ToString();

                List<Settingaccountlist> accountList = new List<Settingaccountlist>();
                foreach (DataRow account in dsTradeAccSetting.Tables[0].AsEnumerable().Where(b => b["BrokerName"].ToString() == setting.broker))
                {
                    Settingaccountlist settingAccount = new Settingaccountlist();
                    settingAccount.account = account["TradeAccount"].ToString();
                    accountList.Add(settingAccount);
                }

                setting.settingAccountList = accountList;
                list.Add(setting);
            }
            return list;
        }

        private List<Pointtradeacclist> GetUserPointTradeAcc(string userid)
        {
            List<Pointtradeacclist> pointTradeAccList = new List<Pointtradeacclist>();
            string yyyymm = DateTime.Now.ToString("yyyyMM");
            DataSet dsPointTradeAccList = UserXQLiteGetUserPointTradeAcc(userid);

            foreach (DataRow drBroker in dsPointTradeAccList.Tables[0].Rows)
            {
                if (pointTradeAccList.Count > 0)
                {
                    var checkBroker = pointTradeAccList.AsEnumerable().Where(t => t.broker == drBroker["TSName"].ToString());

                    if (checkBroker.Count() > 0)
                        continue;
                }
                Pointtradeacclist tradeAcc = new Pointtradeacclist();
                tradeAcc.broker = drBroker["TSName"].ToString();

                List<Accountlist> accountList = new List<Accountlist>();
                foreach (DataRow drAccount in dsPointTradeAccList.Tables[0].AsEnumerable().Where(b => b["TSName"].ToString() == tradeAcc.broker))
                {
                    Accountlist account = new Accountlist();
                    account.account = drAccount["TradeAccount"].ToString();
                    accountList.Add(account);
                }

                tradeAcc.accountList = accountList;
                pointTradeAccList.Add(tradeAcc);
            }

            return pointTradeAccList;
        }

        private Balance GetUserPointBlance(string userid)
        {
            DataSet dsbalance = null;

            dsbalance = UserXQLiteGetUserPointBlance(userid);

            Balance balance = new Balance();
            balance.date = DateTime.Now.ToString("yyyy-MM-dd");
            if (dsbalance != null && dsbalance.Tables[0].Rows.Count > 0)
            {
                balance.amount = Convert.ToInt32(dsbalance.Tables[0].Rows[0].ItemArray[0].ToString());
                balance.accu = Convert.ToInt32(dsbalance.Tables[0].Rows[0].ItemArray[1].ToString());
                balance.used = Convert.ToInt32(dsbalance.Tables[0].Rows[0].ItemArray[2].ToString());
                balance.Added = Convert.ToInt32(dsbalance.Tables[0].Rows[0].ItemArray[3].ToString());
            }

            return balance;
        }


        /// <summary>
        /// 取得所有購買模組
        /// </summary>
        /// <param name="UserID"></param>
        internal List<string> GetPayOpidForXQLite(string UserID)
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



        /// <summary>
        /// 驗證券商是否要重新驗證
        /// </summary>
        internal string GetVerifyBrokerAcc(string accountType)
        {
            string VerifyBrokerAcc = "Y";

            if (accountType == "1" || accountType != "")
            {
                VerifyBrokerAcc = "N";
            }
            return VerifyBrokerAcc;
        }

        public DataSet UserXQLiteVerifyBroker_SEL(string userid)
        {
            DataSet ds = new DataSet();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"UserXQLiteVerifyBroker_SEL";


                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@userid";
                paramText.Value = userid;
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

        private DataSet UserXQLiteGetUserFreeTrial(string userid, string tsid, string aid)
        {
            DataSet ds = new DataSet();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                //cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"SELECT  
                                        u.userid, u.TSID, u.AID, convert(varchar, o.paySdate, 111) paySdate, 
                                        ISNULL(convert(varchar, o.payEdate, 111) +'(延展下單日:'+ convert(varchar, extendDate, 111) +')',convert(varchar, o.payEdate, 111)) as payEdate, 
                                        convert(varchar, LastTrade, 111) LastTrade
                                    FROM UserExchFreeTrial u
                                    LEFT JOIN FreeTrialTradeRecord r ON r.TSID = u.TSID AND r.AID = u.AID
							        LEFT JOIN UserOrderRecord o ON o.userid = u.userid
                                    WHERE  
                                    (u.TSID = @TSID OR @TSID='')
                                    AND opid='US' AND SystemStatus=888
                                    AND u.userid = @userid
                                    AND u.TSID = @tsid
                                    AND u.AID = @aid
                        ";

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = userid;
                cmd.Parameters.Add(paramUserid);

                var paramTsid = cmd.CreateParameter();
                paramTsid.ParameterName = "@tsid";
                paramTsid.Value = tsid;
                cmd.Parameters.Add(paramTsid);

                var paramAid = cmd.CreateParameter();
                paramAid.ParameterName = "@aid";
                paramAid.Value = aid;
                cmd.Parameters.Add(paramAid);

                ds.Load(cmd.ExecuteReader(), LoadOption.OverwriteChanges, "");
            }
            return ds;
        }

        private DataSet UserXQLiteGetPayRecord_FreeTrial(string userid)
        {
            DataSet ds = new DataSet();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                //cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"    SELECT  
                                            [SEQNO]
                                            ,[userid]
                                            ,[opid]
                                            ,[opseq]
                                            ,convert(varchar, paySdate, 120) paySdate
                                            ,convert(varchar, payEdate, 120) payEdate
                                            ,[others]
                                            ,autopay
                                            ,convert(varchar, createTime, 120) createTime,
                                            cardSEQNO
                                        FROM 
                                            [dbo].[UserPayRecord]
                                        WHERE 
                                            opid IN ('US', 'USD') 
                                            AND cardSEQNO in(888,889)
                                            AND userid=@USERID
                                        AND 
                                            DATEDIFF(s,payEdate , @today) < 0
                        ";

                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@userid";
                paramText.Value = userid;
                cmd.Parameters.Add(paramText);

                var paramToday = cmd.CreateParameter();
                paramToday.ParameterName = "@today";
                paramToday.Value = DateTime.Now;
                cmd.Parameters.Add(paramToday);

                ds.Load(cmd.ExecuteReader(), LoadOption.OverwriteChanges, "");
            }
            return ds;
        }

        /// <summary>
        /// 取得券商綁定美股贈送資訊
        /// </summary>
        /// <param name="sDSN"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        private DataSet UserXQLiteGetUserExchFreeTrial(string userid)
        {
            DataSet ds = new DataSet();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                //cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"  SELECT 
                                            userid,
                                            u.TSID,
                                            ISNULL(v.Name, u.TSID) AS NAME,
                                            u.AID,
                                            lastUpdate,
                                            freeTrial,
                                            extendDate
                                        FROM UserExchFreeTrial u
                                        LEFT JOIN
                                            VerifyBrokerConfig v ON u.TSID = v.TSID
                                        WHERE freeTrial = '1' AND userid = @userid
                        ";

                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@userid";
                paramText.Value = userid;
                cmd.Parameters.Add(paramText);

                ds.Load(cmd.ExecuteReader(), LoadOption.OverwriteChanges, "");
            }
            return ds;
        }

        private DataSet UserXQLiteGetUserTradeAccSetting(string userid)
        {
            DataSet ds = new DataSet();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                //cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"    SELECT 
	                                        UserID,
	                                        u.TSID,
	                                        ISNULL(p.TSName,u.TSID) AS BrokerName, 
	                                        TradeAccount
                                        FROM UserPointBrokerMapping u
                                        LEFT JOIN 
	                                        PointBrokerInfo p ON u.TSID = p.TSID
                                        WHERE
	                                        UserID = @userid
                        ";

                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@userid";
                paramText.Value = userid;
                cmd.Parameters.Add(paramText);

                ds.Load(cmd.ExecuteReader(), LoadOption.OverwriteChanges, "");
            }
            return ds;
        }

        private DataSet UserXQLiteGetUserPointTradeAcc(string userid)
        {
            DataSet ds = new DataSet();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                //cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"    SELECT
                                            a.TSID,
                                            b.TSName,
                                            TradeAccount,
                                            userid 
                                        FROM
                                            UserPointInComeEstimate a
                                        INNER JOIN  PointBrokerInfo b ON a.TSID = b.TSID
                                        WHERE userid = @userid
                                        GROUP BY a.TSID,b.TSName,TradeAccount,userid
                        ";

                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@userid";
                paramText.Value = userid;
                cmd.Parameters.Add(paramText);

                ds.Load(cmd.ExecuteReader(), LoadOption.OverwriteChanges, "");
            }
            return ds;
        }

        private DataSet UserXQLiteGetUserPointBlance(string userid)
        {
            DataSet ds = new DataSet();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"UserPointPay_Balance2";


                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@userid";
                paramText.Value = userid;
                cmd.Parameters.Add(paramText);

                ds.Load(cmd.ExecuteReader(), LoadOption.OverwriteChanges, "");
            }

            return ds;
        }
        #endregion

        #region 查詢區間點數資訊
        public IntervalPointInfoViewModel GetIntervalPointInfo(string userid, string tsid, string interval)
        {
            IntervalPointInfoViewModel point = new IntervalPointInfoViewModel();

            DataSet dsReword = null;
            DataSet dsRedeem = null;

            string sDate = string.Empty;
            string eDate = string.Empty;

            if (interval != "ALL")
            {
                DateTime now = DateTime.Now;
                eDate = now.ToString("yyyyMMdd");
                sDate = now.AddYears(Convert.ToInt16(interval) * -1).ToString("yyyyMMdd");
            }

            dsReword = UserXQLiteGetIntervalPointEstimate(userid, tsid, sDate, eDate);
            dsRedeem = UserXQLiteGetIntervalPointRedeem(userid, sDate, eDate);
            if (dsReword != null)
            {
                if (dsReword.Tables.Count > 0)
                {
                    point.reword = dsReword.Tables[0].Rows[0][0].ToString();
                }
            }

            if (dsRedeem != null)
            {
                if (dsRedeem.Tables.Count > 0)
                {
                    point.redeem = dsRedeem.Tables[0].Rows[0][0].ToString();
                }
            }

            point.reword = string.IsNullOrEmpty(point.reword) ? "0" : point.reword;
            point.redeem = string.IsNullOrEmpty(point.redeem) ? "0" : point.redeem;

            return point;
        }

        private DataSet UserXQLiteGetIntervalPointEstimate(string userid, string tsid, string sDate, string eDate)
        {
            DataSet ds = new DataSet();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                //cmd.CommandType = CommandType.StoredProcedure;
                string cmdTxt = @"  
                            SELECT
                                ISNULL(sum(Point),0) as Point
                            FROM 
                                UserPointInComeEstimate a
                            INNER JOIN  PointBrokerInfo b ON a.TSID = b.TSID
                            WHERE 
                                a.userid =@userid ";

                if (tsid.ToLower() != "all")
                {
                    cmdTxt += @"AND
                                a.TSID = @tsid";
                }

                if (!string.IsNullOrEmpty(sDate) && !string.IsNullOrEmpty(eDate))
                {
                    cmdTxt += @"
                                AND 
                                    DATEDIFF(d, 0, @sDate) <= DATEDIFF(d, 0, a.CreateTime)
                                AND
                                    DATEDIFF(d, 0, a.CreateTime) <= DATEDIFF(d, 0, @eDate)
                                ";

                }

                cmd.CommandText = cmdTxt;

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = userid;
                cmd.Parameters.Add(paramUserid);

                var paramTsid = cmd.CreateParameter();
                paramTsid.ParameterName = "@tsid";
                paramTsid.Value = tsid;
                cmd.Parameters.Add(paramTsid);

                var paramSDate = cmd.CreateParameter();
                paramSDate.ParameterName = "@sDate";
                paramSDate.Value = sDate;
                cmd.Parameters.Add(paramSDate);

                var paramEDate = cmd.CreateParameter();
                paramEDate.ParameterName = "@eDate";
                paramEDate.Value = eDate;
                cmd.Parameters.Add(paramEDate);

                ds.Load(cmd.ExecuteReader(), LoadOption.OverwriteChanges, "");
            }
            return ds;
        }

        private DataSet UserXQLiteGetIntervalPointRedeem(string userid, string sDate, string eDate)
        {
            DataSet ds = new DataSet();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                //cmd.CommandType = CommandType.StoredProcedure;
                string cmdTxt = @"  
                            SELECT 
                                sum(a.point)
                            FROM 
                                UserPointPay a
                            INNER JOIN UserPayRecord b on a.cardSEQNO = b.cardSEQNO
                            WHERE
                                a.cardSEQNO not in ('404','888','111')
                            AND
                                a.userid =@userid";

                if (!string.IsNullOrEmpty(sDate) && !string.IsNullOrEmpty(eDate))
                {
                    cmdTxt += @"
                                AND 
                                    DATEDIFF(d, 0, @sDate) <= DATEDIFF(d, 0, a.CreateTime)
                                AND
                                    DATEDIFF(d, 0, a.CreateTime) <= DATEDIFF(d, 0, @eDate)
                                ";

                }

                cmd.CommandText = cmdTxt;

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = userid;
                cmd.Parameters.Add(paramUserid);

                var paramSDate = cmd.CreateParameter();
                paramSDate.ParameterName = "@sDate";
                paramSDate.Value = sDate;
                cmd.Parameters.Add(paramSDate);

                var paramEDate = cmd.CreateParameter();
                paramEDate.ParameterName = "@eDate";
                paramEDate.Value = eDate;
                cmd.Parameters.Add(paramEDate);

                ds.Load(cmd.ExecuteReader(), LoadOption.OverwriteChanges, "");
            }
            return ds;
        }
        #endregion

        #region 查詢使用者點數歷史資訊
        public List<PointHistoryViewModel> GetUserPointHistory(string userid, string tsid, string interval)
        {

            

            List<PointHistoryViewModel> list = new List<PointHistoryViewModel>();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserPoint_History";

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = userid;
                cmd.Parameters.Add(paramUserid);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PointHistoryViewModel model = new PointHistoryViewModel();

                        model.tsid = reader["TSID"].ToString();
                        model.date = Convert.ToDateTime(reader["CreateTime"].ToString()).ToString("yyyy-MM-dd");
                        if (reader["TSID"].ToString() == "Free")
                            model.source = reader["memo"].ToString() + "。訂單編號：" + reader["cardseqno"].ToString();
                        else
                        {
                            if (reader["memo"].ToString() == "開戶贈點")
                            {
                                model.source = "【" + reader["TSName"].ToString() + "】" + DateTime.ParseExact(reader["YYYYMMDD"].ToString(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("yyyy-MM-dd") + " 開戶贈點";
                            }
                            else
                            {
                                model.source = "【" + reader["TSName"].ToString() + "】" + DateTime.ParseExact(reader["YYYYMMDD"].ToString(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("yyyy-MM-dd") + " 交易" + Convert.ToInt64(reader["TradeCost"].ToString()).ToString("N0") + "元";
                            }
                        }

                        model.reword = reader["reword"].ToString() == "0" ? "-" : reader["reword"].ToString();
                        model.redeem = reader["redeem"].ToString() == "0" ? "-" : reader["redeem"].ToString();
                        model.desc = reader["memo"].ToString();
                        model.order = reader["cardseqno"].ToString();
                        list.Add(model);
                    }
                }
            }

            if (tsid != "ALL")
            {
                list = list.Where(h => h.tsid == tsid).ToList();
            }

            if (interval != "ALL")
            {
                DateTime now = DateTime.Now;
                DateTime sDate = now.AddYears(Convert.ToInt16(interval) * -1);

                list = list.Where(h => sDate.CompareTo(Convert.ToDateTime(h.date)) < 0).ToList();
            }



            list = list.OrderByDescending(h => h.date).ToList();
            return list;
        }

        private DataSet UserXQLiteGetPointHistory(string userid, string tsid, string sDate, string eDate)
        {
            DataSet ds = new DataSet();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                string cmdTxt = "UserPoint_History";
                #region MyRegion

                //string cmdTxt = @"  
                //            SELECT * FROM (
                //                SELECT
                //                    a.YYYYMMDD,
                //                    a.CreateTime,
                //                    a.TSID,
                //                    b.TSName,
                //                    ''as tradeaccount,
                //                    userid,
                //                    sum(Point) as reword,
                //                    '' as redeem,
                //                    sum(TradeCost) as TradeCost,'
                //                    ' as cardseqno,
                //                    '交易贈點' as memo 
                //                FROM 
                //                    UserPointInComeEstimate a
                //                INNER JOIN  PointBrokerInfo b ON a.TSID = b.TSID
                //                WHERE 
                //                    a.userid =@userid";

                //if (tsid != "ALL")
                //{
                //    cmdTxt += @"
                //                AND 
                //                    a.TSID = @tsid
                //                ";

                //}

                //if (!string.IsNullOrEmpty(sDate) && !string.IsNullOrEmpty(eDate))
                //{
                //    cmdTxt += @"
                //                AND 
                //                    DATEDIFF(d, 0, @sDate) <= DATEDIFF(d, 0, a.CreateTime)
                //                AND
                //                    DATEDIFF(d, 0, a.CreateTime) <= DATEDIFF(d, 0, @eDate)
                //                ";

                //}
                //cmdTxt += @"
                //                GROUP BY 
                //                    YYYYMMDD,a.CreateTime,a.TSID,b.TSName,userid
                //                UNION
                //                SELECT 
                //                    '',
                //                    a.CreateTime,
                //                    'Free' as tsid,
                //                    '加值模組抵扣' as tsname,
                //                    '' as tradeaccount,
                //                    a.userid,
                //                    '' as reword,
                //                    a.point,
                //                    0 as TradeCost,
                //                    a.cardSeqno ,
                //                    '購買【'+b.opseq+'】模組費用' as memo 
                //                FROM 
                //                    UserPointPay a
                //                INNER JOIN UserPayRecord b on a.cardSEQNO = b.cardSEQNO
                //                WHERE
                //                    a.cardSEQNO not in ('404','888','111')
                //                AND
                //                    a.userid =@userid";
                //if (!string.IsNullOrEmpty(sDate) && !string.IsNullOrEmpty(eDate))
                //{
                //    cmdTxt += @"
                //                AND 
                //                    DATEDIFF(d, 0, @sDate) <= DATEDIFF(d, 0, a.CreateTime)
                //                AND
                //                    DATEDIFF(d, 0, a.CreateTime) <= DATEDIFF(d, 0, @eDate)
                //                ";
                //}
                //cmdTxt += @"  
                //            ) a
                //            Order by CreateTime desc
                //        ";
                #endregion

                cmd.CommandText = cmdTxt;

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = userid;
                cmd.Parameters.Add(paramUserid);

                ds.Load(cmd.ExecuteReader(), LoadOption.OverwriteChanges, "");
            }
            return ds;
        }
        #endregion

        #region 點數規則查詢
        public List<PointRuleInfoViewModel> GetPointRuleInfo(string tsid)
        {
            List<PointRuleInfoViewModel> infoList = new List<PointRuleInfoViewModel>();
            string apiUrl = ConfigurationManager.AppSettings["PointInfoUrl"];

            string result = AppHelper.HttpWebRequestGet(apiUrl, string.Empty);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            var data = doc.GetElementsByTagName("Row");

            foreach (XmlNode node in data)
            {
                PointRuleInfoViewModel info = new PointRuleInfoViewModel();
                info.TSID = node.Attributes["TSID"].Value;
                info.TradeCost = node.Attributes["Cost"].Value;
                info.point = node.Attributes["Point"].Value;
                info.sDate = DateTime.ParseExact(node.Attributes["StartDate"].Value, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("yyyy/MM/dd");
                info.eDate = DateTime.ParseExact(node.Attributes["EndDate"].Value, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("yyyy/MM/dd");
                info.memo = node.Attributes["Memo"].Value;

                infoList.Add(info);
            }

            if (tsid == "ALL")
            {
                return infoList;
            }

            return infoList.AsEnumerable().Where(u => u.TSID == tsid).ToList();
        }
        #endregion


        /// <summary>
        /// 取得user目前訂閱及訂閱過的模組
        /// </summary>
        public List<SubProductlist> GetUserSubProducts(string userid, bool fuzzy)
        {
            List<SubProductlist> list = new List<SubProductlist>();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                            SELECT 
                                a.[userid]
                                ,a.[opseq]
                                ,a.[opid]
                                ,b.[cardSEQNO]
                                ,b.[paySdate]
                                ,[payCdate]
                                ,a.[payEdate]
                                ,a.[autopay]
                                ,a.[SEQNO]
                                ,b.[others]
                                ,b.[zipcode]
                                ,b.[address]
                            FROM
                                [UserOrderRecord] a
                            INNER JOIN
                                [UserPayRecord] b ON a.[SEQNO] = b.[SEQNO]
                            WHERE
                                
                            ";
                if (fuzzy)
                    cmd.CommandText += @" a.[userid] like '%' + @USERID + '%'";
                else
                    cmd.CommandText += @" a.[userid] = @USERID";
                cmd.CommandText += @" ORDER BY payEdate DESC";


                cmd.Parameters.Clear();

                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@USERID";
                paramText.Value = userid;
                cmd.Parameters.Add(paramText);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] productArray = reader["opid"].ToString().Split('_');
                        if (productArray.Length > 1)
                        {
                            //到期的贈送項目排除
                            if (productArray[1] == "MARKET")
                                continue;
                        }
                        SubProductlist product = new SubProductlist();
                        product.userid = reader["userid"].ToString();
                        product.opseq = reader["opseq"].ToString();
                        product.opid = reader["opid"].ToString();
                        product.orderNo = reader["cardSEQNO"].ToString();
                        product.paySdate = DateTime.Parse(reader["paySdate"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                        product.payEdate = DateTime.Parse(reader["payEdate"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                        product.payCdate = string.IsNullOrEmpty(reader["payCdate"].ToString()) ? string.Empty : DateTime.Parse(reader["payCdate"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                        product.autopay = reader["autopay"].ToString();
                        product.SEQNO = reader["SEQNO"].ToString();
                        product.others = reader["others"].ToString();
                        product.zipcode = reader["zipcode"].ToString();
                        product.address = reader["address"].ToString();

                        list.Add(product);
                    }
                }
            }

            return list;
        }

        public string MaintenBaseInfo(MaintenBaseInfoParamViewModel model)
        {
            MaintenResultViewModel result = new MaintenResultViewModel();
            //string sBrokerID = ConfigurationManager.AppSettings["JUSTB-"];

            if (string.IsNullOrEmpty(model.userid))
            {
                result.Status = false;
                result.StatusCode = "10";//userid 為空

                return JsonConvert.SerializeObject(result);
            }

            using (var uow = UnitOfWorkFactory.Create())
            {
                var customerMasterRepos = new CustomerMasterRepository(uow);
                var emailCheckRepos = new EmailCheckRepository(uow);
                var userActionMsgRepos = new UserActionMsgRepository(uow);
                var createAccountRepos = new CreateAccountRepository(uow);

                #region Mobile 驗證

                if (string.IsNullOrEmpty(model.currentmobile) || string.IsNullOrEmpty(model.newmobile))
                {
                    result.Status = false;
                    result.StatusCode = "20";//原mobile 或 新mobile 為空
                    uow.Dispose();
                    return JsonConvert.SerializeObject(result);
                }

                try
                {
                    //確認mobile是否重覆
                    if (model.newmobile != model.currentmobile && createAccountRepos.CheckMobile(model.newmobile))
                    {
                        result.Status = false;
                        result.StatusCode = "21";//新mobile 重覆
                        uow.Dispose();
                        return JsonConvert.SerializeObject(result);
                    }
                }
                catch (Exception e)
                {

                    result.Status = false;
                    result.StatusCode = "22";//確認mobile是否重覆時，發生錯誤
                    uow.Dispose();
                    return JsonConvert.SerializeObject(result);
                }
                #endregion

                #region Email 驗證
                if (string.IsNullOrEmpty(model.currentemail) || string.IsNullOrEmpty(model.newemail))
                {
                    result.Status = false;
                    result.StatusCode = "30";//原email 或 新email 為空
                    uow.Dispose();
                    return JsonConvert.SerializeObject(result);
                }

                try
                {
                    //確認Email是否重覆
                    if (model.newemail != model.currentemail && emailCheckRepos.CheckEmailExcludeUserid(model.newemail, model.userid))
                    {
                        result.Status = false;
                        result.StatusCode = "31";//新Email 重覆
                        uow.Dispose();
                        return JsonConvert.SerializeObject(result);
                    }
                }
                catch (Exception e)
                {

                    result.Status = false;
                    result.StatusCode = "32";//確認Email是否重覆時，發生錯誤
                    uow.Dispose();
                    return JsonConvert.SerializeObject(result);
                }

                #endregion

                //更新mobile、email
                if ((model.currentmobile != model.newmobile) || (model.currentemail != model.newemail))
                {
                    bool isUpdateCustomerMaster = customerMasterRepos.CustomerMasterUpdateByName(model.userid, model.newmobile, model.newemail);

                    if (!isUpdateCustomerMaster)
                    {
                        result.Status = false;
                        result.StatusCode = "40";//更新mobile、email時，發生錯誤
                        uow.Dispose();
                        return JsonConvert.SerializeObject(result);
                    }
                }

                if (model.currentemail != model.newemail)
                {
                    //將舊mail 失效
                    bool isUpdateCurrentEmail = emailCheckRepos.EmailCheck2_INS(model.currentemail, Guid.NewGuid(), "false", 0);
                    if (!isUpdateCurrentEmail)
                    {
                        result.Status = false;
                        result.StatusCode = "41";//將舊mail 失效時，發生錯誤
                        uow.Dispose();
                        return JsonConvert.SerializeObject(result);
                    }

                    //新增新的mail
                    bool isUpdateNewEmail = emailCheckRepos.EmailCheck_INS(model.newemail, Guid.NewGuid(), "true", 7);
                    if (!isUpdateNewEmail)
                    {
                        result.Status = false;
                        result.StatusCode = "42";//新增新的mail時，發生錯誤
                        uow.Dispose();
                        return JsonConvert.SerializeObject(result);
                    }
                }



                if ((model.currentmobile != model.newmobile) || (model.currentemail != model.newemail))
                {
                    //寫入UserActionMsg
                    string actionMsg = "由後台人員修改Mobile及E-mail資料，";
                    if (model.currentmobile != model.newmobile)
                        actionMsg += $"原Mobile：{model.currentmobile}，新Mobile：{model.newmobile}。";
                    if (model.currentemail != model.newemail)
                        actionMsg += $"原E-mail：{model.currentmobile}，新E-mail：{model.newmobile}。";

                    UserActionMsgViewModel actionModel = new UserActionMsgViewModel();
                    actionModel.Appid = "DAQXQLITE";
                    actionModel.userid = model.userid;
                    actionModel.Action = "CHANGE_MOBILE";
                    actionModel.Msg = actionMsg;

                    bool isUpdateUserActionMsg = userActionMsgRepos.UserActionMsg_INS(actionModel);
                    if (!isUpdateUserActionMsg)
                    {
                        result.Status = false;
                        result.StatusCode = "43";//寫入UserActionMsg時，發生錯誤
                        uow.Dispose();
                        return JsonConvert.SerializeObject(result);
                    }
                }

                uow.SaveChanges();
                uow.Dispose();
            }
            //改完是否寄給通知使用者

            result.Status = true;
            result.StatusCode = "00";//成功

            return JsonConvert.SerializeObject(result);
        }

        public MaintenOrderResult MaintenUserProdutPermission(MaintenOrderParamViewModel model)
        {
            MaintenOrderResult result = new MaintenOrderResult();

            /** to do 目前台股不做**/

            using (var uow = UnitOfWorkFactory.Create())
            {
                #region 檢查訂單資訊
                var orderRecordRepos = new UserOrderRecordRepository(uow);
                var orderRecordList = orderRecordRepos.GetUserOrderRecord(model);

                if (orderRecordList.Count <= 0)
                {
                    result.Status = false;
                    result.StatusCode = "1";//沒有 訂單資訊
                    uow.Dispose();

                    return result;
                }
                //else
                //{
                //    model.opid = orderRecordList[0].opid;
                //}
                #endregion

                #region 維護 XQ Lite 訂單
                result = CancelOrder(model, orderRecordList, uow);
                if (!result.Status)
                {
                    uow.Dispose();
                    return result;
                }
                #endregion

                string msg = string.Empty;
                string action = string.Empty;
                if (!Convert.ToBoolean(model.autopay))
                {
                    #region 更新發票狀態
                    UserInvoiceRecordRepository invoiceRecordRepos = new UserInvoiceRecordRepository(uow);
                    UserInvoiceRecordViewModel invoiceRecord = new UserInvoiceRecordViewModel();
                    List<UserInvoiceRecordViewModel> invoiceRecordList = new List<UserInvoiceRecordViewModel>();
                    try
                    {
                        invoiceRecordList = invoiceRecordRepos.UserInvoiceRecord_SEL_PayRecordSEQNO(orderRecordList[0].SEQNO);
                    }
                    catch (Exception)
                    {
                        result.Status = false;
                        result.StatusCode = "7";//取得UserInvoiceRecord資訊發生錯誤
                        uow.Dispose();

                        return result;
                    }

                    if (invoiceRecordList.Count > 0)
                    {
                        result = UpdateInvoiceStatus(model, invoiceRecordList[0], uow);

                        if (!result.Status)
                        {
                            uow.Dispose();
                            return result;
                        }
                    }
                    else
                    {
                        result.Status = false;
                        result.StatusCode = "8";//無發票資訊
                        uow.Dispose();

                        return result;
                    }
                    #endregion

                    #region 更新點數狀態
                    if (orderRecordList[0].orderNo != "404" && orderRecordList[0].orderNo != "888" && orderRecordList[0].orderNo != "111")
                    {
                        result = UpdatePointStatus(model, uow);
                        if (!result.Status)
                        {
                            uow.Dispose();
                            return result;
                        }
                    }
                    #endregion

                    action = "CANCELSUB_";
                    msg = $"由管理後台取消訂單，訂單編號：{orderRecordList[0].orderNo}，取消日期：{model.payEdate}。";
                }
                else
                {
                    action = "MAINTEN_";
                    msg = $"由管理後台維護訂單，訂單編號：{orderRecordList[0].orderNo}，paySdate：{model.paySdate.ToString()}，payEdate:{model.payEdate}。";
                }

                #region 紀錄
                try
                {
                    var actionMsgRepos = new UserActionMsgRepository(uow);
                    UserActionMsgViewModel actionMsg = new UserActionMsgViewModel();

                    actionMsg.Appid = "DAQXQLITE";
                    actionMsg.userid = model.userid;
                    actionMsg.Action = action + model.opid;
                    actionMsg.Msg = msg;
                    actionMsg.IpAddress = AppHelper.GetIPAddress();
                    actionMsgRepos.UserActionMsg_INS(actionMsg);
                }
                catch (Exception)
                {

                }
                #endregion

                uow.SaveChanges();
                uow.Dispose();
            }

            result.Status = true;
            result.StatusCode = "0";//成功

            return result;
        }

        private MaintenOrderResult UpdatePointStatus(MaintenOrderParamViewModel model, IUnitOfWork uow)
        {
            MaintenOrderResult result = new Models.MaintenOrderResult();

            UserPointPayRepository pointPayRepos = new UserPointPayRepository(uow);

            try
            {
                UserPointPayViewModel userPointPayModel = new UserPointPayViewModel();
                userPointPayModel.cardSEQNO = model.orderNo;
                userPointPayModel.Status = "-1";
                pointPayRepos.UserPointPay_UPD_STATUS(userPointPayModel);
            }
            catch (Exception)
            {
                result.Status = false;
                result.StatusCode = "9"; //UserPointPay更新失敗

                return result;
            }

            result.Status = true;
            result.StatusCode = "0";//成功

            return result;

        }

        private MaintenOrderResult UpdateInvoiceStatus(MaintenOrderParamViewModel model, UserInvoiceRecordViewModel invoiceRecord, IUnitOfWork uow)
        {
            MaintenOrderResult result = new Models.MaintenOrderResult();
            UserInvoiceRecordRepository invoiceRecordRepos = new UserInvoiceRecordRepository(uow);

            invoiceRecord.Status = "cancel";
            invoiceRecord.PayCdate = model.payEdate;

            try
            {
                invoiceRecordRepos.UserInvoiceRecord_Cancel_UPD(invoiceRecord);
            }
            catch (Exception)
            {
                result.Status = false;
                result.StatusCode = "43"; //發票紀錄更新失敗

                return result;
            }

            result.Status = true;
            result.StatusCode = "0";//成功

            return result;
        }

        private MaintenOrderResult CancelOrder(MaintenOrderParamViewModel model, List<UserOrderRecordViewModel> orderRecordList, IUnitOfWork uow)
        {
            MaintenOrderResult result = new Models.MaintenOrderResult();

            var orderRecordRepos = new UserOrderRecordRepository(uow);
            var payRecordRepos = new UserPayRecordRepository(uow);
            var exchAuthRepos = new UserExchAuthRepository(uow);
            var extraAuthRepos = new UserExtraAuthRepository(uow);
            var newsAuthRepos = new UserNewsAuthRepository(uow);
            var actionMsgRepos = new UserActionMsgRepository(uow);

            UserOrderRecordViewModel orderRecord = new UserOrderRecordViewModel();
            UserPayRecordViewModel payRecord = new UserPayRecordViewModel();
            UserExchAuthViewModel exchAuth = new UserExchAuthViewModel();
            UserExtraAuthViewModel extraAuth = new UserExtraAuthViewModel();
            UserNewsAuthViewModel newsAuth = new UserNewsAuthViewModel();
            UserActionMsgViewModel actionMsg = new UserActionMsgViewModel();

            string seqNo = string.Empty;// 訂單 SEQNO
            string msg = string.Empty;

            #region UserOrderRecord
            orderRecord = orderRecordList[0];
            seqNo = orderRecord.SEQNO;

            orderRecord.autopay = model.autopay;
            //orderRecord.paySdate = (DateTime)model.paySdate;
            orderRecord.payEdate = (DateTime)model.payEdate;

            if (!Convert.ToBoolean(model.autopay))
            {
                orderRecord.payCdate = model.payEdate;

                msg = $"由管理後台取消訂單，訂單編號：{orderRecordList[0].orderNo}，取消日期：{model.payEdate}。";
            }
            else
            {
                msg = $"由管理後台維護訂單，訂單編號：{orderRecordList[0].orderNo}，paySdate：{model.paySdate.ToString()}，payEdate:{model.payEdate}。";
            }

            orderRecord.SystemMsg = msg;

            try
            {
                orderRecordRepos.UserOrderRecord_UPD(orderRecord);
            }
            catch (Exception)
            {
                result.Status = false;
                result.StatusCode = "2";//UserOrderRecord更新失敗

                return result;
            }
            #endregion

            #region UserPayRecord
            payRecord.SEQNO = seqNo;
            payRecord.autopay = false.ToString().ToLower();
            payRecord.paySdate = (DateTime)model.paySdate;
            payRecord.payEdate = (DateTime)model.payEdate;
            payRecord.others = msg;

            try
            {
                payRecordRepos.UserPayRecord_UPD(payRecord);

            }
            catch (Exception)
            {
                result.Status = false;
                result.StatusCode = "3";//UserPayRecord更新失敗

                return result;
            }
            #endregion

            #region mark
            //#region UserExchAuth
            //exchAuth.userid = model.userid;
            //exchAuth.enddate = model.payEdate;

            //switch (model.opid)
            //{
            //    case "HK":
            //        exchAuth.exchid = "7";
            //        var exchAuthHKList = exchAuthRepos.UserExchAuth_SEL(exchAuth);
            //        if (exchAuthHKList.Count > 0)
            //            exchAuth.startdate = exchAuthHKList[0].startdate;

            //        try
            //        {
            //            exchAuthRepos.UserExchAuth_UPD(exchAuth);
            //        }
            //        catch (Exception)
            //        {
            //            result.Status = false;
            //            result.StatusCode = "4";//UserExchAuth更新失敗

            //            return result;
            //        }
            //        break;
            //    case "US":
            //        exchAuth.exchid = "12";
            //        var exchAuthUSList = exchAuthRepos.UserExchAuth_SEL(exchAuth);
            //        exchAuth.startdate = exchAuthUSList[0].startdate;

            //        try
            //        {
            //            exchAuthRepos.UserExchAuth_UPD(exchAuth);
            //        }
            //        catch (Exception)
            //        {
            //            result.Status = false;
            //            result.StatusCode = "4";//UserExchAuth更新失敗

            //            return result;
            //        }
            //        break;
            //    case "CN":
            //        exchAuth.exchid = "8";
            //        var exchAuthCNList = exchAuthRepos.UserExchAuth_SEL(exchAuth);
            //        exchAuth.startdate = exchAuthCNList[0].startdate;
            //        try
            //        {
            //            exchAuthRepos.UserExchAuth_UPD(exchAuth);
            //        }
            //        catch (Exception)
            //        {
            //            result.Status = false;
            //            result.StatusCode = "4";//UserExchAuth更新失敗

            //            return result;
            //        }

            //        exchAuth.exchid = "9";
            //        var exchAuthCN2List = exchAuthRepos.UserExchAuth_SEL(exchAuth);
            //        exchAuth.startdate = exchAuthCN2List[0].startdate;
            //        try
            //        {
            //            exchAuthRepos.UserExchAuth_UPD(exchAuth);
            //        }
            //        catch (Exception)
            //        {
            //            result.Status = false;
            //            result.StatusCode = "4";//UserExchAuth更新失敗

            //            return result;
            //        }
            //        break;
            //}
            //#endregion

            //#region UserExtraAuth
            //extraAuth.userid = model.userid;
            //switch (model.opid)
            //{
            //    case "BS":
            //    case "INDU":
            //    case "WS":
            //        extraAuth.extraid = "Features";
            //        extraAuth.extraval = model.opid;
            //        break;
            //    case "SENSOR":
            //    case "STRATEGY":
            //        extraAuth.extraid = "XSAuth";
            //        extraAuth.extraval = model.opid;
            //        break;
            //    default:
            //        break;
            //}
            //extraAuth.enddate = (DateTime)model.payEdate;

            //if (!string.IsNullOrEmpty(extraAuth.extraid) || !string.IsNullOrEmpty(extraAuth.extraval))
            //{
            //    try
            //    {
            //        extraAuthRepos.UserExtraAuth_UPD(extraAuth);
            //    }
            //    catch (Exception)
            //    {
            //        result.Status = false;
            //        result.StatusCode = "5";//UserExtraAuth更新失敗

            //        return result;
            //    }

            //    try
            //    {
            //        if (model.opid == "WS" && orderRecord.payFdate > DateTime.Now && !Convert.ToBoolean(model.autopay))
            //        {
            //            DelWsPower(model.userid);

            //            actionMsg.Appid = "DAQXQLITE";
            //            actionMsg.userid = model.userid;
            //            actionMsg.Action = "DEL_WS";
            //            actionMsg.Msg = $"DEL WS[{model.userid}]";
            //            actionMsg.IpAddress = AppHelper.GetIPAddress();
            //            actionMsgRepos.UserActionMsg_INS(actionMsg);
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        result.Status = false;
            //        result.StatusCode = "6"; //WS權限更新失敗

            //        return result;
            //    }

            //}
            //#endregion
            #endregion

            ProductSetViewModel productSetModel = new ProductSetViewModel();
            productSetModel.PID = model.opid;

            #region 取消模組權限 UserExtraAuth
            var UserExtraAuthList = serach_Permission(productSetModel, uow);
            if (UserExtraAuthList != null)
            {
                foreach (var item in UserExtraAuthList)
                {
                    try
                    {
                        var extraList = item.Split('@');
                        string extraid = extraList[0];
                        string extraval = extraList[1];

                        extraAuth.userid = model.userid;
                        extraAuth.extraid = extraid;
                        extraAuth.extraval = extraval;
                        extraAuth.enddate = (DateTime)model.payEdate;

                        extraAuthRepos.UserExtraAuth_UPD(extraAuth);
                    }
                    catch (Exception ex)
                    {
                        result.Status = false;
                        result.StatusCode = "34";

                        return result;
                    }
                }

                try
                {
                    if (model.opid == "WS" && orderRecord.payFdate > DateTime.Now)
                    {
                        DelWsPower(model.userid);

                        actionMsg.Appid = "DAQXQLITE";
                        actionMsg.userid = model.userid;
                        actionMsg.Action = "DEL_WS";
                        actionMsg.Msg = $"DEL WS[{model.userid}]";
                        actionMsg.IpAddress = AppHelper.GetIPAddress();
                        actionMsgRepos.UserActionMsg_INS(actionMsg);
                    }
                }
                catch (Exception ex)
                {
                    result.Status = false;
                    result.StatusCode = "35";

                    return result;
                }
            }
            #endregion

            #region 取消行情權限 UserExchAuth、UserNewsAuth
            var UserExchAuthList = serach_exch_Permission(productSetModel, uow);
            //EXCH@12:62:即時|NEWS@ET;IS
            if (UserExchAuthList != null)
            {
                string ExchPower = string.Empty;
                string NewsPower = string.Empty;
                foreach (string str in UserExchAuthList)
                {
                    if (str.IndexOf("EXCH@") > -1)
                    {
                        ExchPower = str;
                    }
                    if (str.IndexOf("NEWS@") > -1)
                    {
                        NewsPower = str;
                    }
                }

                if (ExchPower != string.Empty)
                {
                    ExchPower = ExchPower.Replace("EXCH@", "");
                    string[] ExchPowerList = null;
                    ExchPowerList = ExchPower.Split(';');

                    foreach (var item in ExchPowerList)
                    {
                        try
                        {
                            var extraList = item.Split(':');
                            string exchid = extraList[0];
                            string attrvalue = extraList[1];
                            string description = extraList[2];

                            exchAuth.userid = model.userid;
                            exchAuth.exchid = exchid;
                            exchAuth.enddate = model.payEdate;
                            var exchAuthList = exchAuthRepos.UserExchAuth_SEL(exchAuth);
                            if (exchAuthList.Count > 0)
                                exchAuth.startdate = exchAuthList[0].startdate;

                            exchAuthRepos.UserExchAuth_UPD(exchAuth);
                        }
                        catch (Exception ex)
                        {
                            result.Status = false;
                            result.StatusCode = "33";

                            return result;
                        }
                    }
                }

                if (NewsPower != string.Empty)
                {
                    NewsPower = NewsPower.Replace("NEWS@", "");
                    string[] NewsPowerList = null;
                    NewsPowerList = NewsPower.Split(';');

                    foreach (var newsid in NewsPowerList)
                    {
                        try
                        {
                            newsAuth.userid = model.userid;
                            newsAuth.newsid = newsid;
                            newsAuth.enddate = (DateTime)model.payEdate;
                            var newsAuthList = newsAuthRepos.UserNewsAuth_SEL(newsAuth);
                            if (newsAuthList.Count > 0)
                                newsAuth.startdate = newsAuthList[0].startdate;

                            newsAuthRepos.UserNewsAuth_UPD(newsAuth);
                        }
                        catch (Exception ex)
                        {
                            result.Status = false;
                            result.StatusCode = "36";

                            return result;
                        }
                    }
                }
            }
            #endregion

            result.Status = true;
            result.StatusCode = "0";//成功

            return result;
        }

        /// <summary>
        /// 刪除 權證模組 WS 權限
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        private string DelWsPower(string userid)
        {
            string Url = string.Format(ConfigurationManager.AppSettings["WS"], "DAQ", userid);
            HttpWebRequest request = HttpWebRequest.Create(Url) as HttpWebRequest;
            request.Method = "DELETE";    // 方法
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 30000;
            request.ContentLength = 0;
            request.KeepAlive = false; //是否保持連線
            string result = "";
            try
            {
                // 取得回應資料
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// 取得舊台股到期時間
        /// </summary>
        /// <returns></returns>
        public static DateTime? GetTWExpireTime()
        {
            DateTime? date = null;

            using (var uow = UnitOfWorkFactory.Create())
            {
                ProductSetRepository productSetRepos = new ProductSetRepository(uow);
                ProductSetViewModel model = new ProductSetViewModel();
                model.PID = "TW";
                List<ProductSetViewModel> list = productSetRepos.ProductSet_SEL_PID(model);

                if (list != null && list.Count > 0 )
                {
                    DateTime endDate = new DateTime();
                    ProductSetViewModel porductSet = list[0];
                    if (DateTime.TryParse(Convert.ToString(porductSet.EndDate), out endDate))
                    {
                        date = endDate;
                    }
                }
            }
            return date;

        }

        /// <summary>
        /// 查詢項目權限
        /// <para>參數: 項目ID</para>
        /// </summary>
        /// <param name="opid"></param>
        /// <returns></returns>
        public string[] serach_Permission(ProductSetViewModel model, IUnitOfWork uow)
        {
            List<ProductSetViewModel> productList = new List<ProductSetViewModel>();
            ProductSetRepository productRepos = new ProductSetRepository(uow);
            productList = productRepos.ProductSet_SEL_PID(model);

            ProductSetViewModel productset = productList[0];

            string[] UserExtraAuthList = null;

            if (productset != null)
            {
                string tmp = productset.Permissions;
                if (!string.IsNullOrEmpty(productset.PKind))
                {
                    string PKind = productset.PKind;
                    if (PKind.ToUpper() == "EXTRA")
                    {
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            UserExtraAuthList = tmp.Split(';');
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        UserExtraAuthList = tmp.Split(';');
                    }
                }

            }
            return UserExtraAuthList;
        }

        /// <summary>
        /// 查詢行情模組權限
        /// <para>參數: 項目ID</para>
        /// </summary>
        /// <param name="opid"></param>
        /// <returns></returns>
        public string[] serach_exch_Permission(ProductSetViewModel model, IUnitOfWork uow)
        {
            List<ProductSetViewModel> productList = new List<ProductSetViewModel>();
            ProductSetRepository productRepos = new ProductSetRepository(uow);
            productList = productRepos.ProductSet_SEL_PID(model);

            ProductSetViewModel productset = productList[0];

            string[] UserExtraAuthList = null;

            if (productset != null)
            {
                string tmp = productset.Permissions;
                if (!string.IsNullOrEmpty(productset.PKind))
                {
                    string PKind = productset.PKind;
                    if (PKind.ToUpper() == "EXCH")
                    {
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            UserExtraAuthList = tmp.Split('|');
                        }
                    }
                }

            }
            return UserExtraAuthList;
        }

        private enum TWType
        {
            /// <summary>
            /// 收盤後顯示
            /// </summary>
            EOD,
            /// <summary>
            /// 台股五秒更新
            /// </summary>
            TW5,
            /// <summary>
            /// 台股即時
            /// </summary>
            TW
        }

        public void SaveChanges()
        {
            _unitOfWork.SaveChanges();
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public void Create(UserBaseInfoViewModel instance)
        {
            throw new NotImplementedException();
        }

        public void Delete(UserBaseInfoViewModel instance)
        {
            throw new NotImplementedException();
        }

        public UserBaseInfoViewModel Get(int primaryID)
        {
            throw new NotImplementedException();
        }

        public IQueryable<UserBaseInfoViewModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(UserBaseInfoViewModel instance)
        {
            throw new NotImplementedException();
        }
    }
}

