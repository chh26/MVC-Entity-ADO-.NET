using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using XQLiteMgm.Helper;
using XQLiteMgm.Models.Interface;
using XQLiteMgm.Models.Repository.Tables;
using XQLiteMgm.Models.UnitOfWork;
using XQLiteMgm.Models.UnitOfWork.Interface;

namespace XQLiteMgm.Models.Repository
{
    public class CreateAccountRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public CreateAccountRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        internal ResultViewModels<string> CreateAccount(CreateAccountViewModel model)
        {
            ResultViewModels<string> result = new ResultViewModels<string>();
            

            //帳號資料驗證
            result = IsValidAccountData(model);

            //帳號資料驗證 有誤
            if (result.ResultStatus == Status.fail)
                return result;

            string custno = result.Result;//帳號資訊驗證成功，回傳最新的客戶編號（custno）

            try
            {
                string sysType = ConfigurationManager.AppSettings["SYS_Type"];

                CustomerMasterRepository custMasterRepos = new CustomerMasterRepository(_unitOfWork);
                CustomerDetailRepository custDetailRepos = new CustomerDetailRepository(_unitOfWork);
                CustomerServiceRepository custServiceRepos = new CustomerServiceRepository(_unitOfWork);
                UserPWDRepository userPWDRepos = new UserPWDRepository(_unitOfWork);
                UserAppAuthRepository userAppAuthRepos = new UserAppAuthRepository(_unitOfWork);
                UserSyncAccountRepository userSyncAccRepos = new UserSyncAccountRepository(_unitOfWork);

                #region CustomerMaster
                CustomerMasterViewModel custMasterModel = new CustomerMasterViewModel();

                custMasterModel.custno = custno;
                custMasterModel.name = model.xquserid;
                custMasterModel.invoiceTitle = string.Empty;
                custMasterModel.invoiceAddress = string.Empty;
                custMasterModel.ssn = string.Empty;
                custMasterModel.president = string.Empty;
                custMasterModel.contact = string.Empty;
                custMasterModel.tel = string.Empty;
                custMasterModel.fax = string.Empty;
                custMasterModel.mobile = model.mobile;
                custMasterModel.email = model.email;
                custMasterModel.remark = string.Empty;
                custMasterModel.cratedate = DateTime.Today;
                if (!string.IsNullOrEmpty(sysType) && sysType.ToLower() == "su")
                    custMasterModel.opseq = 0;
                else
                    custMasterModel.opseq = 999;

                custMasterRepos.CustomerMasterInsert(custMasterModel);
                #endregion

                #region CustomerDetail
                CustomerDetailViewModel custDetailModel = new CustomerDetailViewModel();
                custDetailModel.custno = custno;
                custDetailModel.name = model.xquserid;
                custDetailModel.userid = model.xquserid;
                custDetailModel.groupid = model.xquserid;
                custDetailModel.acctype = string.Empty;
                custDetailModel.hostaddress = string.Empty;
                custDetailModel.contact = string.Empty;
                custDetailModel.tel = string.Empty;
                custDetailModel.fax = string.Empty;
                custDetailModel.mobile = model.mobile;
                custDetailModel.connectype = "A";
                custDetailModel.leasedlineno = string.Empty;
                custDetailModel.pchardware = string.Empty;
                custDetailModel.slseq = 99;
                custDetailModel.lastupdate = DateTime.Today;
                custDetailModel.productype = "DAQXQLITE";
                custDetailModel.billtype = "免費";
                custDetailModel.paytype = "0";
                custDetailModel.initialdate = DateTime.Today;
                custDetailModel.billsdate = DateTime.Today;
                custDetailModel.ValidSDate = DateTime.Today;
                custDetailModel.ValidEDate = DateTime.Today;
                custDetailModel.money = 0;
                custDetailModel.Monitor = 0;

                custDetailRepos.CustomerDetailInsert(custDetailModel);
                #endregion

                #region CustomerService
                CustomerServiceViewModel custServiceModel = new CustomerServiceViewModel();
                custServiceModel.custno = custno;
                custServiceModel.servicedate = DateTime.Today;
                custServiceModel.serviceitem = "新增使用者";
                custServiceModel.servicedetail = string.Empty;
                custServiceModel.opseq = 11;
                custServiceModel.operIP = string.Empty;

                custServiceRepos.CustomerServiceInsert(custServiceModel);
                #endregion

                #region UserPWD
                UserPWDRecordViewModel userPWDModel = new UserPWDRecordViewModel();
                userPWDModel.userid = model.xquserid;
                userPWDModel.pwd = model.pwd;

                userPWDRepos.UserPWD_INS(userPWDModel);
                #endregion

                #region UserAppAuth
                UserAppAuthViewModel userAppAuthModel = new UserAppAuthViewModel();
                userAppAuthModel.userid = model.xquserid;
                userAppAuthModel.authid = "DAQXQLITE";
                userAppAuthModel.enddate = new DateTime(2038, 1, 1, 0, 0, 0);

                userAppAuthRepos.UserAppAuth_INS(userAppAuthModel);
                #endregion

            }
            catch (Exception ex)
            {
                string errMsg = $@"發生錯誤:{Environment.NewLine}新建客戶帳號發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";


                LogHelper.WriteLog(string.Empty, errMsg);

                result.ResultStatus = Status.fail;
                result.Result = "CreateAccountError";
                result.Msg = errMsg;

                return result;
            }

            result.ResultStatus = Status.success;

            return result;
        }

        internal ResultViewModels<string> CreateMoneyDJAccount(CreateAccountViewModel model)
        {
            ResultViewModels<string> result = new ResultViewModels<string>();
            try
            {
                UserSyncAccountRepository userSyncAccRepos = new UserSyncAccountRepository(_unitOfWork);

                result = CreateAccount(model.userid, model.pwd, model.email);

                if (result.ResultStatus == Status.fail)
                {
                    return result;
                }
                else
                {
                    UserSyncAccountViewModel userSyncAccModel = new UserSyncAccountViewModel();
                    userSyncAccModel.userid = model.xquserid;
                    userSyncAccModel.XQ = 1;
                    userSyncAccModel.Moneydj = 1;

                    userSyncAccRepos.UserSyncAccount_INS(userSyncAccModel);
                }
            }
            catch (Exception ex)
            {
                string errMsg = $@"發生錯誤:{Environment.NewLine}新建MoneyDJ帳號發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";


                LogHelper.WriteLog(string.Empty, errMsg);

                result.ResultStatus = Status.fail;
                result.Result = "CreateMoneyDJAccountError";
                result.Msg = errMsg;

                return result;
            }
            return result;
        }
        /// <summary>
        /// 帳號資料驗證
        /// </summary>
        /// <param name="model"></param>
        /// <returns> validResult.msg = custno;//帳號資訊驗證成功，回傳最新的客戶編號（custno）</returns>
        private ResultViewModels<string> IsValidAccountData(CreateAccountViewModel model)
        {
            ResultViewModels<string> validResult = new ResultViewModels<string>();
            string custno = string.Empty;

            validResult.ResultStatus = Status.success;

            //using (var uow = UnitOfWorkFactory.Create())
            //{
            try
            {
                CustomerMasterRepository customerMasterRepos = new CustomerMasterRepository(_unitOfWork);

                custno = customerMasterRepos.GetXQLiteMaxCustno();

                if (CheckCustno(custno))
                {
                    //客戶編號已存在
                    validResult.ResultStatus = Status.fail;
                    validResult.Msg = "客戶編號已存在";
                    validResult.Result = "CustnoExists";
                    return validResult;
                }

                if (CheckUserPWD(model.xquserid))
                {
                    // 客戶ID是否已存在
                    validResult.ResultStatus = Status.fail;
                    validResult.Msg = "客戶ID已存在（UserPWD）";
                    validResult.Result = "UserIDExists";
                    return validResult;
                }

                if (CheckUserAppAuth(model.xquserid))
                {
                    // 客戶ID是否已存在
                    validResult.ResultStatus = Status.fail;
                    validResult.Msg = "客戶ID已存在（UserAppAuth）";
                    validResult.Result = "UserIDExists";
                    return validResult;
                }

                if (CheckMobile(model.mobile))
                {
                    //MOBILE重覆
                    validResult.ResultStatus = Status.fail;
                    validResult.Msg = "行動電話重覆";
                    validResult.Result = "MobileExists";
                    return validResult;
                }

                if (CheckEmail(model.email))
                {
                    //EMAIL重覆
                    validResult.ResultStatus = Status.fail;
                    validResult.Msg = "電子信箱重覆";
                    validResult.Result = "EmailExists";
                    return validResult;
                }

                validResult = IsMemberIDAvailable(model.xquserid, model.userid, customerMasterRepos);
                if (validResult.ResultStatus == Status.success)
                {
                    if (validResult.Result != "0")
                    {
                        //客戶帳號已存在
                        validResult.ResultStatus = Status.fail;
                        validResult.Msg = "客戶帳號已存在";
                        validResult.Result = "MemberIDExists";
                        return validResult;
                    }
                }
                else if (validResult.ResultStatus == Status.fail)
                {
                    return validResult;
                }

                if (CheckCustomerDetail(custno, model.xquserid))
                {
                    // 客戶明細是否已存在
                    validResult.ResultStatus = Status.fail;
                    validResult.Msg = "客戶明細已存在";
                    validResult.Result = "CustomerDetailExists";
                    return validResult;
                }
            }
            catch (Exception ex)
            {
                string errMsg = $@"發生錯誤:{Environment.NewLine}新建客戶帳號發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                LogHelper.WriteLog(string.Empty, errMsg);

                validResult.ResultStatus = Status.fail;
                validResult.Msg = errMsg;
                validResult.Result = "IsValidAccountDataError";

                return validResult;
            }

            validResult.Result = custno;//帳號資訊驗證成功，回傳最新的客戶編號（custno）

            return validResult;
        }

        /// <summary>
        /// XQ客戶帳號是否已存在
        /// </summary>
        /// <param name="xquid"></param>
        /// <param name="uid"></param>
        /// <param name="customerMasterRepos"></param>
        /// <returns></returns>
        public ResultViewModels<string> IsMemberIDAvailable(string xquid, string uid, CustomerMasterRepository customerMasterRepos)
        {
            ResultViewModels<string> resultModel = new Models.ResultViewModels<string>();
            string Url = ConfigurationManager.AppSettings["IsMemberIDAvailable"];
            if (!string.IsNullOrEmpty(Url))
            {
                try
                {
                    string json = string.Format("{{\"P1\":\"{0}\"}}", uid);

                    string result = AppHelper.HttpWebRequestPost(Url, json);

                    JObject message = JsonConvert.DeserializeObject<JObject>(result);

                    if (result.IndexOf("\"Success\":false") > -1)
                    {
                        resultModel.ResultStatus = Status.success;
                        resultModel.Result = message["d"]["Message"].Value<string>();
                        return resultModel;
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = $@"發生錯誤:{Environment.NewLine}新建客戶帳號發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                    LogHelper.WriteLog(string.Empty, errMsg);

                    resultModel.ResultStatus = Status.fail;
                    resultModel.Msg = errMsg;
                    resultModel.Result = "MoneyDJCheckAccountAPIError";

                    return resultModel;
                }

            }

            if (CheckUserID(xquid, customerMasterRepos))
            {
                resultModel.ResultStatus = Status.success;
                resultModel.Result = "XQ客戶帳號已存在";
                return resultModel;
            }
            else
            {
                resultModel.ResultStatus = Status.success;
                resultModel.Result = "0";
                return resultModel;
            }
        }

        /// <summary>
        /// 新增 MoneyDJ 帳號
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public ResultViewModels<string> CreateAccount(string uid, string pwd, string email)
        {
            ResultViewModels<string> resultModel = new Models.ResultViewModels<string>();
            string Url = ConfigurationManager.AppSettings["CreateAccount"];
            //建立新帳號
            //需要帶Header AcceptTypes='application/json'回傳json格式 預設回傳XML格式資訊
            Url = string.Format(Url + "?uid={0}&pwd={1}&email={2}&verify=false", uid, pwd, email);
            HttpWebRequest request = HttpWebRequest.Create(Url) as HttpWebRequest;
            request.Method = "GET";
            request.Timeout = 30000;
            //request.ContentLength = 0;
            request.KeepAlive = false;
            request.CookieContainer = new CookieContainer();
            request.Accept = "application/xml";
            request.ContentType = "application/xml";

            try
            {
                string result = "";
                // 取得回應資料
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                    }
                }

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(result);
                XmlNodeList nodes = xml.SelectNodes("//Result");
                string reMsg = string.Empty;
                string status = string.Empty;
                reMsg = nodes[0].Attributes["Message"].Value;
                status = nodes[0].ChildNodes[0].InnerText;

                if (status != "0")
                {
                    resultModel.ResultStatus = Status.fail;
                    resultModel.Msg = "MoneyDJCreateAccountError";
                    resultModel.Result = reMsg;
                    return resultModel;
                }
            }
            catch (Exception ex)
            {
                string errMsg = $@"發生錯誤:{Environment.NewLine}新建客戶帳號發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                LogHelper.WriteLog(string.Empty, errMsg);

                resultModel.ResultStatus = Status.fail;
                resultModel.Msg = errMsg;
                resultModel.Result = "MoneyDJCreateAccountAPIError";

                return resultModel;
            }

            resultModel.ResultStatus = Status.success;

            return resultModel;
        }



        /// <summary>
        /// 確認客戶編號是否重覆
        /// </summary>
        /// <param name="custno"></param>
        /// <param name="customerMasterRepos"></param>
        /// <returns></returns>
        public bool CheckCustno(string custno)
        {
            CustomerMasterRepository customerMasterRepos = new CustomerMasterRepository(_unitOfWork);
            CustomerMasterViewModel custMaster = new CustomerMasterViewModel();

            custMaster = customerMasterRepos.CustomerMasterSelect(custno);

            if (custMaster != null)
            {
                return true;//客戶編號重複使用
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 確認email是否重覆
        /// </summary>
        /// <param name="email"></param>
        /// <param name="customerMasterRepos"></param>
        /// <returns></returns>
        public bool CheckEmail(string email)
        {
            CustomerMasterRepository customerMasterRepos = new CustomerMasterRepository(_unitOfWork);
            CustomerMasterViewModel custMaster = new CustomerMasterViewModel();

            custMaster = customerMasterRepos.CustomerMasterSelectEmail(email);

            if (custMaster != null)
            {
                return true;//email重複使用
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 確認mobile是否重覆
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="customerMasterRepos"></param>
        /// <returns></returns>
        public bool CheckMobile(string mobile)
        {
            CustomerMasterRepository customerMasterRepos = new CustomerMasterRepository(_unitOfWork);
            CustomerMasterViewModel custMaster = new CustomerMasterViewModel();

            custMaster = customerMasterRepos.CustomerMasterSelectMobile(mobile);

            if (custMaster != null)
            {
                return true;//手機重複使用
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 確認客戶帳號是否重覆
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="customerMasterRepos"></param>
        /// <returns></returns>
        public bool CheckUserID(string userid, CustomerMasterRepository customerMasterRepos)
        {
            CustomerMasterViewModel custMaster = new CustomerMasterViewModel();
            custMaster = customerMasterRepos.CustomerMasterSelectName(userid);

            if (custMaster != null)
            {
                return true;//客戶帳號重複使用
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 確認客戶明細是否已存在
        /// </summary>
        /// <param name="custno"></param>
        /// <param name="userid"></param>
        /// <param name="customerDetailRepos"></param>
        /// <returns></returns>
        public bool CheckCustomerDetail(string custno, string userid)
        {
            CustomerDetailRepository customerDetailRepos = new CustomerDetailRepository(_unitOfWork);
            CustomerDetailViewModel custDetail = new CustomerDetailViewModel();
            custDetail = customerDetailRepos.CustomerDetailSelect(custno, userid);

            if (custDetail != null)
            {
                return true;//客戶明細重複
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 確認客戶ID是否已存在
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="userPWDRepos"></param>
        /// <returns></returns>
        public bool CheckUserPWD(string userid)
        {
            UserPWDRepository userPWDRepos = new UserPWDRepository(_unitOfWork);
            UserPWDRecordViewModel userPWD = userPWDRepos.UserPWD_SEL(userid);

            if (userPWD != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 確認客戶ID是否已存在
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="userAppAuthRepos"></param>
        /// <returns></returns>
        public bool CheckUserAppAuth(string userid)
        {
            UserAppAuthRepository userAppAuthRepos = new UserAppAuthRepository(_unitOfWork);
            UserAppAuthViewModel appAuth = userAppAuthRepos.UserAppAuth_SEL(userid, "DAQXQLITE");

            if (appAuth != null)
                return true;
            else
                return false;
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

    public class CreateAccountViewModel
    {
        public string custno { get; set; }
        public string userid { get; set; }
        public string xquserid { get; set; }
        public string pwd { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public List<string> opidList { get; set; }
        public DateTime payEdate { get; set; }

    }

}