using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XQLiteMgm.Helper;
using XQLiteMgm.Models;
using XQLiteMgm.Models.Repository;
using XQLiteMgm.Models.Repository.Tables;
using XQLiteMgm.Models.UnitOfWork;

namespace XQLiteMgm.Controllers.WebAPI
{
    public class CreateAccountAndProductsController : ApiController
    {
        /// <summary>
        /// 取得使用者目前可訂閱的模組，其中display = true是已訂閱的
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/CreateAcount/GetUserProductList")]
        [HttpPost]
        public List<ProductSetViewModel> GetUserProductList(UserOrderRecordViewModel model)
        {
            List<ProductSetViewModel> productSetList = new List<ProductSetViewModel>();
            List<UserOrderRecordViewModel> userOrderRecordList = new List<UserOrderRecordViewModel>();

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["BrokerID"]))
            {
                if (!model.userid.ToUpper().Contains(ConfigurationManager.AppSettings["BrokerID"]))
                {
                    model.userid = ConfigurationManager.AppSettings["BrokerID"] + model.userid;
                }
            }

            using (var uow = UnitOfWorkFactory.Create())
            {
                var productSetRepo = new ProductSetRepository(uow);
                productSetList = productSetRepo.GetProductSetList();
                productSetRepo.Dispose();

                var userOrderRecordRepo = new UserOrderRecordRepository(uow);
                userOrderRecordList = userOrderRecordRepo.UserOrderRecord_SEL_Sub(model.userid);
            }

            productSetList.ForEach(product => {
                var userOrder = userOrderRecordList.Where(o => o.opid == product.PID).ToList();

                if (userOrder.Count > 0)
                    product.disabled = true;//user如果有訂閱該模組，則display = true
            });

            return productSetList;
        }

        /// <summary>
        /// 取得使用者目前可訂閱的模組，其中display = true是已訂閱的
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/CreateAcount/CreateAccountAndService")]
        [HttpPost]
        public ResultViewModels<string> CreateAccountAndService(CreateAccountViewModel model)
        {
            ResultViewModels<string> result = new ResultViewModels<string>();

            if (string.IsNullOrEmpty(model.userid))
            {
                result.ResultStatus = Status.fail;
                result.Msg = "UserID為必填";
                result.Result = "UserIDEmpty";
                return result;
            }

            if (string.IsNullOrEmpty(model.pwd))
            {
                result.ResultStatus = Status.fail;
                result.Msg = "密碼為必填";
                result.Result = "PWDEmpty";
                return result;
            }

            if (string.IsNullOrEmpty(model.mobile))
            {
                result.ResultStatus = Status.fail;
                result.Msg = "行動電話為必填";
                result.Result = "MobileEmpty";
                return result;
            }

            if (string.IsNullOrEmpty(model.email))
            {
                result.ResultStatus = Status.fail;
                result.Msg = "電子信箱為必填";
                result.Result = "EMailEmpty";
                return result;
            }

            using (var uow = UnitOfWorkFactory.Create())
            {
                var createAccRepo = new CreateAccountRepository(uow);
                var createServiceRepo = new CreateServiceRepository(uow);

                model.xquserid = model.userid;
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["BrokerID"]))
                {
                    if (!model.xquserid.Contains(ConfigurationManager.AppSettings["BrokerID"]))
                    {
                        model.xquserid = ConfigurationManager.AppSettings["BrokerID"] + model.userid;
                    }
                }

                #region CreateAccount

                try
                {
                    result = createAccRepo.CreateAccount(model);

                    if (result.ResultStatus == Status.fail)
                    {
                        uow.Dispose();
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = $@"發生錯誤:{Environment.NewLine}新建客戶帳號發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                    LogHelper.WriteLog(string.Empty, errMsg);

                    result.ResultStatus = Status.fail;
                    result.Result = "CreateAccountError";
                    result.Msg = errMsg;

                    uow.Dispose();
                    return result;
                }
                #endregion

                #region CreateService
                try
                {
                    if (model.opidList.Count > 0)
                    {
                        foreach (string opid in model.opidList)
                        {
                            string userOptName = string.IsNullOrEmpty(UserHelper.GetUserAccount()) ? "admin_API" : UserHelper.GetUserAccount();

                            result = createServiceRepo.CreateService(model, opid, "SYS", userOptName);

                            if (result.ResultStatus == Status.fail)
                            {
                                uow.Dispose();
                                return result;
                            }
                        }
                    }
                    

                }
                catch (Exception ex)
                {
                    string errMsg = $@"發生錯誤:{Environment.NewLine}新建產品服務發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                    LogHelper.WriteLog(string.Empty, errMsg);

                    result.ResultStatus = Status.fail;
                    result.Result = "CreateServiceError";
                    result.Msg = errMsg;

                    uow.Dispose();
                    return result;
                }
                #endregion

                #region CreateMoneyDJAccount

                try
                {
                    result = createAccRepo.CreateMoneyDJAccount(model);

                    if (result.ResultStatus == Status.fail)
                    {
                        uow.Dispose();
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = $@"發生錯誤:{Environment.NewLine}新建MoneyDJ發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                    LogHelper.WriteLog(string.Empty, errMsg);

                    result.ResultStatus = Status.fail;
                    result.Result = "CreateMoneyDJAccountError";
                    result.Msg = errMsg;

                    uow.Dispose();
                    return result;
                }
                #endregion

                uow.SaveChanges();
                uow.Dispose();
            }

            result.ResultStatus = Status.success;
            result.Msg = "0";//成功

            return result;
        }

        /// <summary>
        /// 取得使用者目前可訂閱的模組，其中display = true是已訂閱的
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/CreateAcount/CreateService")]
        [HttpPost]
        public ResultViewModels<string> CreateService(CreateAccountViewModel model)
        {
            ResultViewModels<string> result = new ResultViewModels<string>();

            using (var uow = UnitOfWorkFactory.Create())
            {
                var createAccRepo = new CreateAccountRepository(uow);
                var createServiceRepo = new CreateServiceRepository(uow);

                model.xquserid = model.userid;
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["BrokerID"]))
                {
                    if (!model.xquserid.Contains(ConfigurationManager.AppSettings["BrokerID"]))
                    {
                        model.xquserid = ConfigurationManager.AppSettings["BrokerID"] + model.userid;
                    }
                }

                #region CreateService
                try
                {
                    if (model.opidList.Count > 0)
                    {
                        foreach (string opid in model.opidList)
                        {
                            string userOptName = string.IsNullOrEmpty(UserHelper.GetUserAccount()) ? "admin_API" : UserHelper.GetUserAccount();

                            result = createServiceRepo.CreateService(model, opid, "MARKET", userOptName);

                            if (result.ResultStatus == Status.fail)
                            {
                                uow.Dispose();
                                return result;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = $@"發生錯誤:{Environment.NewLine}新建產品服務發生錯誤。 {Environment.NewLine}ErrorMsg：{ex.Message}{Environment.NewLine}ErrorStackTrace：{ex.StackTrace}";

                    LogHelper.WriteLog(string.Empty, errMsg);

                    result.ResultStatus = Status.fail;
                    result.Result = "CreateServiceError";
                    result.Msg = errMsg;

                    uow.Dispose();
                    return result;
                }
                #endregion

                uow.SaveChanges();
                uow.Dispose();
            }

            result.ResultStatus = Status.success;
            result.Msg = "0";//成功

            return result;
        }
    }
}
