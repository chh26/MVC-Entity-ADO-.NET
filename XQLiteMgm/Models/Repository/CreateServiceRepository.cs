using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using XQLiteMgm.Helper;
using XQLiteMgm.Models.Interface;
using XQLiteMgm.Models.Repository.Tables;
using XQLiteMgm.Models.UnitOfWork;
using XQLiteMgm.Models.UnitOfWork.Interface;

namespace XQLiteMgm.Models.Repository
{
    public class CreateServiceRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public CreateServiceRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        internal ResultViewModels<string> CreateService(CreateAccountViewModel model, string opid, string autopay, string adminName)
        {
            string userid = model.xquserid;
            DateTime payEdate = model.payEdate.Date.AddHours(8);

            ResultViewModels<string> result = new ResultViewModels<string>();
            ProductSetRepository productSetRepos = new ProductSetRepository(_unitOfWork);
            UserActionMsgRepository actionMsgRepos = new UserActionMsgRepository(_unitOfWork);

            OrderViewModel o1 = new OrderViewModel();

            try
            {
                DateTime dt = DateTime.Now;
                o1.userid = userid;
                o1.CreditCard = Crypt.enCryptCredit("4222222222222");
                o1.CardValidDate = Crypt.enCryptCredit("202010");
                o1.Name = o1.userid;
                o1.Address = "後門程式";
                o1.opid = opid;

                o1.Ip = AppHelper.GetIPAddress();
                o1.UniForm = "2";
                o1.SEQNO = Guid.NewGuid().ToString();                             //訂單編號
                o1.ReceiptSEQNO = Guid.NewGuid();
                o1.paySdate = dt;
                o1.payEdate = payEdate;
                o1.payFdate = dt;

                //根據PID 查詢商品資訊
                ProductSetViewModel productModel = new ProductSetViewModel();
                productModel.PID = o1.opid;
                List<ProductSetViewModel> list = productSetRepos.ProductSet_SEL_PID(productModel);

                if (list.Count > 0)
                    productModel = list[0];

                o1.MSRP = productModel.MSRP;          //購買項目原價

                if (productModel.Status == "onsale")
                {
                    //DB上存double 轉成 int (for 信用卡格式) 
                    o1.Price = productModel.Bargain_Price;
                }
                else
                {
                    o1.Price = productModel.MSRP;       //購買項目特價
                }

                o1.opseq = productModel.PNAME;                      //訂單項目
                o1.autopay = (string.IsNullOrEmpty(autopay)) ? "SYS" : autopay;   //是否訂閱   
                o1.ProductID = productModel.ECPID;                  //信用卡用訂單編號
                o1.payCdate = null;                                      //取消訂閱時間，可設為null  
                o1.Email = model.email;
                o1.Phone = model.mobile;

                //刷卡服務要用MoneyDJ的userid，且不包含JUSTB-
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["BrokerID"]))
                    o1.MemberID = o1.userid.Replace(ConfigurationManager.AppSettings["BrokerID"], "");
                else
                    o1.MemberID = o1.userid;    //MoneyDJ帳號

                o1.cardSEQNO = "111";
                o1.systemStatus = "0";
                o1.systemMsg = "後門程式訂閱";
                result.ResultStatus = Status.success;
            }
            catch (Exception ex)
            {
                string errMsg = $@"發生錯誤:{Environment.NewLine} 準備刷卡資料時發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                LogHelper.WriteLog(string.Empty, errMsg);

                result.ResultStatus = Status.fail;
                result.Result = "SettingCreditCardDataError";
                result.Msg = errMsg;

                return result;
            }

            try
            {
                result = CreateOrder(o1);

                //建立訂單
                if (result.ResultStatus == Status.success)
                {
                    UserActionMsgViewModel actionMsg = new UserActionMsgViewModel();
                    actionMsg.Appid = "DAQXQLITE";
                    actionMsg.userid = o1.userid;
                    actionMsg.Action = $"{autopay}_{o1.opid}";
                    actionMsg.Msg = $"{adminName} 開通 {o1.opseq} ，有效期間：{o1.paySdate} 到 {o1.payEdate}";
                    actionMsg.IpAddress = AppHelper.GetIPAddress();
                    actionMsgRepos.UserActionMsg_INS(actionMsg);
                }
                else
                {
                    return result;
                }

                result.ResultStatus = Status.success;
            }
            catch (Exception ex)
            {
                string errMsg = $@"發生錯誤:{Environment.NewLine} 建立訂單發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                LogHelper.WriteLog(string.Empty, errMsg);

                result.ResultStatus = Status.fail;
                result.Result = "CreateOrderError";
                result.Msg = errMsg;

                return result;
            }

            return result;
        }

        public ResultViewModels<string> AddWsPower(string strUID)
        {
            ResultViewModels<string> resultModel = new Models.ResultViewModels<string>();
            string Url = ConfigurationManager.AppSettings["AddWsPower"];
            Url = string.Format(Url, "DAQXQLITE", strUID);

            HttpWebRequest request = HttpWebRequest.Create(Url) as HttpWebRequest;
            request.Method = "POST";    // 方法
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
            catch (Exception ex)
            {
                string errMsg = $@"發生錯誤:{Environment.NewLine} AddWsPowerAPI發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                LogHelper.WriteLog(string.Empty, errMsg);

                resultModel.ResultStatus = Status.fail;
                resultModel.Msg = errMsg;
                resultModel.Result = "AddWsPowerAPIError";

                return resultModel;
            }

            resultModel.ResultStatus = Status.success;
            resultModel.Result = result;
            return resultModel;
        }

        /// <summary>
        /// 建立訂單
        /// <para>參數: 使用者帳戶,卡號,卡號有效日期MMYY,姓名,縣市,鄉鎮,郵遞區號,地址,購買項目ID,ip位址</para>
        /// </summary>
        /// <param name="o1"></param>
        public ResultViewModels<string> CreateOrder(OrderViewModel o1)
        {
            ResultViewModels<string> resultModel = new Models.ResultViewModels<string>();
            UserOrderRecordRepository uerOderRecordRepos = new UserOrderRecordRepository(_unitOfWork);
            UserPayRecordRepository userPayRecordRepos = new UserPayRecordRepository(_unitOfWork);
            DeclareMarketRepository declareMaketRepos = new DeclareMarketRepository(_unitOfWork);
            DeclareUserDataRepository declareUserData = new DeclareUserDataRepository(_unitOfWork);
            ProductSetRepository productSetRepos = new ProductSetRepository(_unitOfWork);
            UserActionMsgRepository actionMsgRepos = new UserActionMsgRepository(_unitOfWork);

            try
            {
                #region UserOrderRecord_INS
                UserOrderRecordViewModel uerOderRecordModel = new UserOrderRecordViewModel();
                uerOderRecordModel.userid = o1.userid;
                uerOderRecordModel.opseq = o1.opseq;
                uerOderRecordModel.opid = o1.opid;
                uerOderRecordModel.amount = Convert.ToDouble(o1.Price);
                uerOderRecordModel.MSRP = Convert.ToDouble(o1.MSRP);
                uerOderRecordModel.paySdate = o1.paySdate;
                uerOderRecordModel.payFdate = o1.payFdate;
                uerOderRecordModel.payCdate = o1.payCdate;
                uerOderRecordModel.payEdate = o1.payEdate;
                uerOderRecordModel.autopay = o1.autopay;
                uerOderRecordModel.SEQNO = o1.SEQNO;
                uerOderRecordModel.SystemStatus = o1.systemStatus;
                uerOderRecordModel.SystemMsg = o1.systemMsg;

                uerOderRecordRepos.UserOrderRecord_INS(uerOderRecordModel);
                #endregion

                #region UserPayRecord_INS
                UserPayRecordViewModel userPayRecordModel = new UserPayRecordViewModel();

                userPayRecordModel.SEQNO = o1.SEQNO;
                userPayRecordModel.userid = o1.userid;
                userPayRecordModel.cardno = o1.CreditCard;
                userPayRecordModel.validateend = o1.CardValidDate;
                userPayRecordModel.name = o1.Name;
                userPayRecordModel.UniForm = o1.UniForm;
                userPayRecordModel.UniNum = o1.CompanyTaxID;
                userPayRecordModel.address = o1.Address;
                userPayRecordModel.zipcode = o1.Zip;
                userPayRecordModel.opseq = o1.opseq;
                userPayRecordModel.amount = Convert.ToDouble(o1.Price);
                userPayRecordModel.MSRP = Convert.ToDouble(o1.MSRP);
                userPayRecordModel.autopay = o1.autopay;
                userPayRecordModel.paySdate = o1.paySdate;
                userPayRecordModel.payEdate = o1.payEdate;
                userPayRecordModel.ipaddress = o1.Ip;
                userPayRecordModel.cardSEQNO = o1.cardSEQNO;
                userPayRecordModel.opid = o1.opid;
                userPayRecordModel.others = o1.others;
                userPayRecordModel.ReceiptSEQNO = o1.ReceiptSEQNO;
                userPayRecordRepos.UserPayRecord_INS(userPayRecordModel);
                #endregion

                #region 建立海外申報系統

                if (o1.cardSEQNO != "111" && o1.cardSEQNO != "404" && o1.cardSEQNO != "300")
                {
                    if (o1.opid == "TW300" || o1.opid == "TW" || o1.opid == "CN" || o1.opid == "HK" || o1.opid == "US")
                    {
                        if (o1.opid == "TW300") o1.opid = "TW";
                        try
                        {
                            // 7 HK  8 CN  9 CN  12 US
                            #region 建立海外申報系統 - 申報模組
                            DeclareMarketViewModel declareMarketModel = new DeclareMarketViewModel();

                            declareMarketModel.userid = o1.userid;
                            declareMarketModel.appid = "DAQXQLITE";
                            declareMarketModel.market = o1.opid;
                            declareMarketModel.startdate = Convert.ToDateTime(DateTime.Now);
                            declareMarketModel.enddate = Convert.ToDateTime(o1.payEdate);
                            declareMarketModel.createdate = Convert.ToDateTime(DateTime.Now);
                            declareMarketModel.lastupdate = Convert.ToDateTime(DateTime.Now);

                            if (o1.cardSEQNO == "888")
                                declareMarketModel.isdeclare = "N";
                            else
                                declareMarketModel.isdeclare = "Y";

                            declareMarketModel.syscheck = "0";
                            declareMarketModel.Operator = "sysjustadmin";

                            declareMaketRepos.DeclareMarket_INS(declareMarketModel);
                            #endregion

                            #region 建立海外申報系統 - 個人資料
                            DeclareUserDataViewModel declareUserDataModel = new DeclareUserDataViewModel();

                            declareUserDataModel.userid = o1.userid;
                            declareUserDataModel.appid = "DAQXQLITE";
                            declareUserDataModel.name = o1.Name;
                            declareUserDataModel.mobile = o1.Phone;
                            declareUserDataModel.email = o1.Email;
                            declareUserDataModel.address = o1.Address;
                            declareUserDataModel.idkind = "2";
                            declareUserDataModel.country = "TWN";
                            declareUserDataModel.sales = "張淑杏";
                            declareUserDataModel.Internal = "0"; //外部使用者
                            declareUserDataModel.createdate = Convert.ToDateTime(DateTime.Now);
                            declareUserDataModel.lastupdate = Convert.ToDateTime(DateTime.Now);
                            declareUserDataModel.isdeclare = "Y";    //要申報
                            declareUserDataModel.syscheck = "0";
                            declareUserDataModel.Operator = "sysjustadmin";
                            declareUserData.DeclareUserData_INS(declareUserDataModel);
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            string errMsg = $@"發生錯誤:{Environment.NewLine} 新增海外申報系統發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                            LogHelper.WriteLog(string.Empty, errMsg);

                            resultModel.ResultStatus = Status.fail;
                            resultModel.Msg = errMsg;
                            resultModel.Result = "AddDeclareError";

                            return resultModel;
                        }
                    }
                }
                #endregion

                #region 建立點數

                if (o1.Point >= 50)
                {
                    try
                    {
                        UserPointPayRepository userPointPayRepos = new UserPointPayRepository(_unitOfWork);
                        UserPointPayViewModel userPointPayModel = new UserPointPayViewModel();

                        userPointPayModel.UserID = o1.userid;
                        userPointPayModel.cardSEQNO = o1.cardSEQNO;
                        userPointPayModel.Point = o1.Point;
                        userPointPayModel.PayTime = DateTime.Now;
                        userPointPayModel.Status = "0";
                        userPointPayModel.EndDate = Convert.ToDateTime("2038-01-01");

                        userPointPayRepos.UserPointPay_INS(userPointPayModel);
                    }
                    catch (Exception ex)
                    {
                        string errMsg = $@"發生錯誤:{Environment.NewLine} 新增點數資訊發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                        LogHelper.WriteLog(string.Empty, errMsg);

                        resultModel.ResultStatus = Status.fail;
                        resultModel.Msg = errMsg;
                        resultModel.Result = "ADDUserPointPayError";

                        return resultModel;
                    }
                }
                #endregion

                #region 建立權限

                ProductSetViewModel productSetModel = new ProductSetViewModel();
                productSetModel.PID = o1.opid;
                #region 建立模組權限
                var UserExtraAuthList = productSetRepos.serach_Permission(productSetModel);
                if (UserExtraAuthList != null)
                {
                    foreach (var item in UserExtraAuthList)
                    {
                        try
                        {
                            UserExtraAuthRepository userExtraAuthRepos = new UserExtraAuthRepository(_unitOfWork);
                            UserExtraAuthViewModel userExtraAuthModel = new UserExtraAuthViewModel();

                            var extraList = item.Split('@');
                            string extraid = extraList[0];
                            string extraval = extraList[1];
                            DateTime dt = DateTime.Now;
                            userExtraAuthModel.userid = o1.userid;
                            userExtraAuthModel.extraid = extraid;
                            userExtraAuthModel.extraval = extraval;
                            userExtraAuthModel.startdate = Convert.ToDateTime(o1.paySdate);
                            userExtraAuthModel.enddate = Convert.ToDateTime(o1.payEdate);
                            userExtraAuthModel.lastupdate = dt;

                            userExtraAuthRepos.UserExtraAuth_INS(userExtraAuthModel);
                        }
                        catch (Exception ex)
                        {
                            string errMsg = $@"發生錯誤:{Environment.NewLine} 新增權限發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                            LogHelper.WriteLog(string.Empty, errMsg);

                            resultModel.ResultStatus = Status.fail;
                            resultModel.Msg = errMsg;
                            resultModel.Result = "ADDUserExtraAuthError";

                            return resultModel;
                        }
                    }
                }
                #endregion

                #region 建立行情權限
                var UserExchAuthList = productSetRepos.serach_exch_Permission(productSetModel);
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
                                UserExchAuthRepository userExchAuthRepos = new UserExchAuthRepository(_unitOfWork);
                                UserExchAuthViewModel userExchAuthModel = new UserExchAuthViewModel();
                                var extraList = item.Split(':');
                                string exchid = extraList[0];
                                string attrvalue = extraList[1];
                                string description = extraList[2];
                                DateTime dt = DateTime.Now;
                                userExchAuthModel.userid = o1.userid;
                                userExchAuthModel.exchid = exchid;
                                userExchAuthModel.attrvalue = attrvalue;
                                userExchAuthModel.description = description;
                                userExchAuthModel.enddate = Convert.ToDateTime(o1.payEdate);
                                userExchAuthRepos.UserXQLiteExchAuth_INS(userExchAuthModel);
                            }
                            catch (Exception ex)
                            {
                                string errMsg = $@"發生錯誤:{Environment.NewLine} 新增行情模組權限發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                                LogHelper.WriteLog(string.Empty, errMsg);

                                resultModel.ResultStatus = Status.fail;
                                resultModel.Msg = errMsg;
                                resultModel.Result = "ADDUserExchAuthError";

                                return resultModel;
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
                                UserNewsAuthRepository userNewsAuthRepos = new UserNewsAuthRepository(_unitOfWork);
                                UserNewsAuthViewModel userNewsAuthModel = new UserNewsAuthViewModel();
                                DateTime dt = DateTime.Now;
                                userNewsAuthModel.userid = o1.userid;
                                userNewsAuthModel.newsid = newsid;
                                userNewsAuthModel.enddate = Convert.ToDateTime(o1.payEdate);
                                userNewsAuthRepos.UserXQLiteNewsAuth_INS(userNewsAuthModel);
                            }
                            catch (Exception ex)
                            {
                                string errMsg = $@"發生錯誤:{Environment.NewLine} 新增新聞權限發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                                LogHelper.WriteLog(string.Empty, errMsg);

                                resultModel.ResultStatus = Status.fail;
                                resultModel.Msg = errMsg;
                                resultModel.Result = "ADDNewsPowerError";

                                return resultModel;
                            }
                        }
                    }

                }
                #endregion

                #endregion

                #region Add Ws Power

                try
                {
                    if (o1.opid == "WS")
                    {
                        resultModel = AddWsPower(o1.userid);

                        if (resultModel.ResultStatus == Status.fail)
                            return resultModel;

                        UserActionMsgViewModel actionMsg = new UserActionMsgViewModel();
                        actionMsg.Appid = "DAQXQLITE";
                        actionMsg.userid = o1.userid;
                        actionMsg.Action = "ADDWS";
                        actionMsg.Msg = $"ADD WS [{o1.userid}]";
                        actionMsg.IpAddress = AppHelper.GetIPAddress();
                        actionMsgRepos.UserActionMsg_INS(actionMsg);
                    }
                }
                catch (Exception ex)
                {

                    string errMsg = $@"發生錯誤:{Environment.NewLine} AddWsPower發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                    LogHelper.WriteLog(string.Empty, errMsg);

                    resultModel.ResultStatus = Status.fail;
                    resultModel.Result = "AddWsPowerError";
                    resultModel.Msg = errMsg;

                    return resultModel;
                }
                #endregion

            }
            catch (Exception ex)
            {

                string errMsg = $@"發生錯誤:{Environment.NewLine} 新增模組權限發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                LogHelper.WriteLog(string.Empty, errMsg);

                resultModel.ResultStatus = Status.fail;
                resultModel.Msg = errMsg;
                resultModel.Result = "ADDProductServiceError";

                return resultModel;
            }

            resultModel.ResultStatus = Status.success;

            return resultModel;
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

    

    /// <summary>
    /// 訂單
    /// </summary>
    public class OrderViewModel : creditcard
    {
        /// <summary>
        /// 刷卡紀錄編號
        /// </summary>
        public string SEQNO { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 訂單項目ID
        /// </summary>
        public string opid { get; set; }
        /// <summary>
        /// 訂單項目名稱
        /// </summary>
        public string opseq { get; set; }
        /// <summary>
        /// 訂單項目原始價格
        /// </summary>
        public decimal MSRP { get; set; }
        /// <summary>
        /// 是否訂閱
        /// </summary>
        public string autopay { get; set; }
        /// <summary>
        /// 付費日期
        /// </summary>
        public Nullable<DateTime> paySdate { get; set; }
        /// <summary>
        /// 7天鑑賞期
        /// </summary>
        public Nullable<DateTime> payFdate { get; set; }
        /// <summary>
        /// 7天鑑賞期
        /// </summary>
        public Nullable<DateTime> payCdate { get; set; }
        /// <summary>
        /// 付費一個月期限
        /// </summary>
        public Nullable<DateTime> payEdate { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string others { get; set; }
        /// <summary>
        /// 信用卡訂單編號
        /// </summary>
        public string cardSEQNO { get; set; }
        /// <summary>
        /// 發票種類
        /// </summary>
        public string UniForm { get; set; }
        /// <summary>
        /// MoneyDJ刷卡錯誤訊息
        /// </summary>
        public string systemMsg { get; set; }
        /// <summary>
        /// MoneyDJ刷卡錯誤編號
        /// </summary>
        public string systemStatus { get; set; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Guid ReceiptSEQNO { get; set; }
        #region 目前沒用到
        /// <summary>
        /// 信用卡驗證碼
        /// </summary>
        ///public string secure { get; set; }
        #endregion
    }

    /// <summary>
    /// 信用卡付款
    /// </summary>
    public class creditcard
    {
        /// <summary>
        /// MoneyDJ帳號
        /// </summary>
        public string MemberID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 付費價格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CreditCard { get; set; }
        /// <summary>
        /// 到期日
        /// </summary>
        public string CardValidDate { get; set; }
        /// <summary>
        /// 郵遞區號
        /// </summary>
        public string Zip { get; set; }
        /// <summary>
        /// 縣市
        /// </summary>
        public string County { get; set; }
        /// <summary>
        /// 鄉區
        /// </summary>
        public string District { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 購物ID productset ECPID
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 使用者IP
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 統編
        /// </summary>
        public string CompanyTaxID { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 訂單項目名稱
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 載具類型，當發票為二聯式時，0:電子發票，1:手機載具
        /// </summary>
        public string CarrierType { get; set; }
        /// <summary>
        /// 手機載具條碼
        /// </summary>
        public string CarrierId { get; set; }
        /// <summary>
        /// 捐贈代碼
        /// </summary>
        public string Npo { get; set; }
        /// <summary>
        /// 購買數量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 點數
        /// </summary>
        public int Point { get; set; }
        /// <summary>
        /// 點數說明
        /// </summary>
        public string PointInfo { get; set; }

    }
}