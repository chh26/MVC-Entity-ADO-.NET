using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XQLiteMgm.Models
{
    public class UserBaseInfoViewModel
    {
        public string name { get; set; }
        public string userid { get; set; }

        public string mobile { get; set; }

        public string email { get; set; }

        public string cratedate { get; set; }

        public string opseq { get; set; }

        public string TSID { get; set; }

    }

    public class PointBrokerInfoViewModel
    {
        public string TSID { get; set; }
        public string TSName { get; set; }
        public string TSMemo { get; set; }
        public string AppID { get; set; }

    }

    public class AccountDetailViewModel
    {
        public List<Productlist> productList { get; set; }
        public string brokerAccVerification { get; set; }
        public Freetrial freeTrial { get; set; }
        public List<Tradeaccountsettinglist> tradeAccountSettingList { get; set; }
        public List<Pointtradeacclist> pointTradeAccList { get; set; }
        public Balance balance { get; set; }
    }

    public class Freetrial
    {
        public bool qualified { get; set; }
        public string cardSEQNO { get; set; }
        public string brokerName { get; set; }
        public List<Freetriallist> freeTrialList { get; set; }
    }

    public class Freetriallist
    {
        public string account { get; set; }
        public string sDate { get; set; }
        public string eDate { get; set; }
        public string lastDate { get; set; }
    }

    public class Productlist
    {
        public string product { get; set; }
    }

    public class SubProductlist
    {
        public string userid { get; set; }
        public string opseq { get; set; }
        public string opid { get; set; }
        public string orderNo { get; set; }
        public string paySdate { get; set; }
        public string payEdate { get; set; }
        public string payCdate { get; set; }
        public string autopay { get; set; }
        public string SEQNO { get; set; }
        public string others { get; set; }
        public string zipcode { get; set; }
        public string address { get; set; }
        public bool fuzzy { get; set; }
    }

    public class Tradeaccountsettinglist
    {
        public string broker { get; set; }
        public List<Settingaccountlist> settingAccountList { get; set; }
    }

    public class Settingaccountlist
    {
        public string account { get; set; }
    }

    public class Pointtradeacclist
    {
        public string broker { get; set; }
        public List<Accountlist> accountList { get; set; }
    }

    public class Accountlist
    {
        public string account { get; set; }
    }

    public class Balance
    {
        /// <summary>
        /// 查詢點數日期
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// 點數餘額
        /// </summary>
        public int amount { get; set; }
        /// <summary>
        /// 上月累積
        /// </summary>
        public int accu { get; set; }
        /// <summary>
        /// 本月使用
        /// </summary>
        public int used { get; set; }
        /// <summary>
        /// 本月累積
        /// </summary>
        public int Added { get; set; }

    }
    public class UserVerifyBroker
    {
        /// <summary>
        /// 
        /// </summary>
        public string USERID;
        /// <summary>
        /// 
        /// </summary>
        public string TSID;
        /// <summary>
        /// 
        /// </summary>
        public string VERIFYBROKERDATE;
        /// <summary>
        /// 
        /// </summary>
        public string STATUS;

        public string NAME;
    }

    public class IntervalPointInfoViewModel
    {
        public string reword { get; set; }
        public string redeem { get; set; }
    }

    public class PointHistoryViewModel
    {
        public string tsid { get; set; }
        public string date { get; set; }
        public string source { get; set; }
        public string reword { get; set; }
        public string redeem { get; set; }
        public string desc { get; set; }
        public string order { get; set; }
    }

    public class PointRuleInfoViewModel
    {
        public string TSID { get; set; }
        public string TradeCost { get; set; }
        public string point { get; set; }
        public string sDate { get; set; }
        public string eDate { get; set; }
        public string memo { get; set; }
    }

    public class MaintenResultViewModel
    {
        /// <summary>
        /// 是否取消成功: true： 成功；false：失敗
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 狀態資訊編碼
        /// </summary>
        public string StatusCode { get; set; }
    }

    public class MaintenOrderResult
    {
        /// <summary>
        /// 是否取消成功: true： 成功；false：失敗
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 狀態資訊編碼
        /// </summary>
        public string StatusCode { get; set; }
    }

    #region parameter
    public class UserBaseInfoParamViewModel
    {
        public string type { get; set; }

        public string queryText { get; set; }

    }

    public class AccountStatusParamViewModel
    {
        public string userid { get; set; }
    }

    public class PointHistoryParamViewModel
    {
        public string userid { get; set; }
        public string tsid { get; set; }
        public string interval { get; set; }

    }

    public class MaintenBaseInfoParamViewModel
    {
        public string userid { get; set; }
        public string currentmobile { get; set; }
        public string newmobile { get; set; }
        public string currentemail { get; set; }
        public string newemail { get; set; }

    }

    public class MaintenOrderParamViewModel
    {
        public string userid { get; set; }
        public string opseq { get; set; }
        public string opid { get; set; }
        public string orderNo { get; set; }
        /// <summary>
        /// 訂單購買日
        /// </summary>
        public Nullable<System.DateTime> paySdate { get; set; }
        public Nullable<System.DateTime> payEdate { get; set; }
        public Nullable<System.DateTime> payCdate { get; set; }
        public string autopay { get; set; }
        public string SEQNO { get; set; }

    }
    #endregion
}