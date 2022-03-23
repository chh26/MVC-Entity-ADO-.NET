using System;
using System.Collections.Generic;
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
    public class UserInfoMaintenanceController : ApiController
    {
        [Route("api/User/GetUserBaseInfo")]
        [HttpPost]
        public List<UserBaseInfoViewModel> GetUserBaseInfo(UserBaseInfoParamViewModel model)
        {
            List<UserBaseInfoViewModel> list = new List<UserBaseInfoViewModel>();
            using (var uow = UnitOfWorkFactory.Create())
            {
                var userMaintenRepo = new UserMaintenanceRepository(uow);
                list = userMaintenRepo.GetUserBaseInfo(model.type, model.queryText);
                userMaintenRepo.Dispose();
            }

            return list;
        }

        [Route("api/User/GetPointBrokerInfo")]
        [HttpGet]
        public List<PointBrokerInfoViewModel> GetPointBrokerInfo()
        {
            List<PointBrokerInfoViewModel> list = new List<PointBrokerInfoViewModel>();
            using (var uow = UnitOfWorkFactory.Create())
            {
                var userMaintenRepo = new UserMaintenanceRepository(uow);
                list = userMaintenRepo.GetPointBrokerInfo();
                userMaintenRepo.Dispose();
            }

            return list;
        }

        [Route("api/User/GetUserAccountDetail")]
        [HttpPost]
        public AccountDetailViewModel GetUserAccountDetail(AccountStatusParamViewModel model)
        {
            AccountDetailViewModel accountStatus = new AccountDetailViewModel();
            using (var uow = UnitOfWorkFactory.Create())
            {
                var userMaintenRepo = new UserMaintenanceRepository(uow);
                accountStatus = userMaintenRepo.GetUserAccountDetail(model.userid);
                userMaintenRepo.Dispose();
            }

            return accountStatus;
        }

        [Route("api/User/GetIntervalPointInfo")]
        [HttpPost]
        public IntervalPointInfoViewModel GetIntervalPointInfo(PointHistoryParamViewModel model)
        {
            IntervalPointInfoViewModel intervalPointInfo = new IntervalPointInfoViewModel();
            using (var uow = UnitOfWorkFactory.Create())
            {
                var userMaintenRepo = new UserMaintenanceRepository(uow);
                intervalPointInfo = userMaintenRepo.GetIntervalPointInfo(model.userid, model.tsid, model.interval);
                userMaintenRepo.Dispose();
            }

            return intervalPointInfo;
        }

        [Route("api/User/GetUserPointHistory")]
        [HttpPost]
        public List<PointHistoryViewModel> GetUserPointHistory(PointHistoryParamViewModel model)
        {
            List<PointHistoryViewModel> list = new List<PointHistoryViewModel>();
            using (var uow = UnitOfWorkFactory.Create())
            {
                var userMaintenRepo = new UserMaintenanceRepository(uow);
                list = userMaintenRepo.GetUserPointHistory(model.userid, model.tsid, model.interval);
                userMaintenRepo.Dispose();
            }

            return list;
        }

        [Route("api/User/GetPointRuleInfo")]
        [HttpPost]
        public List<PointRuleInfoViewModel> GetPointRuleInfo(PointHistoryParamViewModel model)
        {
            List<PointRuleInfoViewModel> list = new List<PointRuleInfoViewModel>();
            using (var uow = UnitOfWorkFactory.Create())
            {
                var userMaintenRepo = new UserMaintenanceRepository(uow);
                list = userMaintenRepo.GetPointRuleInfo(model.tsid);
                userMaintenRepo.Dispose();
            }

            return list;
        }

        [Route("api/User/MaintenBaseInfo")]
        [HttpPost]
        public string MaintenBaseInfo(MaintenBaseInfoParamViewModel model)
        {
            string result = string.Empty;
            using (var uow = UnitOfWorkFactory.Create())
            {
                var userMaintenRepo = new UserMaintenanceRepository(uow);
                result = userMaintenRepo.MaintenBaseInfo(model);
                userMaintenRepo.Dispose();
            }

            return result;
        }

        [Route("api/User/GetUserSubProducts")]
        [HttpPost]
        public List<SubProductlist> GetUserSubProducts(SubProductlist model)
        {
            List<SubProductlist> list = new List<SubProductlist>();
            using (var uow = UnitOfWorkFactory.Create())
            {
                var userMaintenRepo = new UserMaintenanceRepository(uow);
                list = userMaintenRepo.GetUserSubProducts(model.userid, model.fuzzy);
                userMaintenRepo.Dispose();
            }

            return list;
        }

        [Route("api/User/MaintenUserProdutPermission")]
        [HttpPost]
        public MaintenOrderResult MaintenUserProdutPermission(MaintenOrderParamViewModel model)
        {
            MaintenOrderResult result = new MaintenOrderResult();

            var userMaintenRepo = new UserMaintenanceRepository();
            result = userMaintenRepo.MaintenUserProdutPermission(model);

            return result;
        }

        [Route("api/User/GetUserPWDByUser")]
        [HttpPost]
        public ResultViewModels<UserPWDRecordViewModel> GetUserPWDByUser(UserPWDRecordViewModel model)
        {
            ResultViewModels<UserPWDRecordViewModel> result = new ResultViewModels<UserPWDRecordViewModel>();

            try
            {
                using (var uow = UnitOfWorkFactory.Create())
                {
                    var userPWDRepo = new UserPWDRepository(uow);
                    result.ResultStatus = Status.success;
                    result.Result = userPWDRepo.UserPWD_SEL(model.userid);
                    userPWDRepo.Dispose();
                }
            }
            catch (Exception e)
            {
                result.ResultStatus = Status.fail;
                result.Msg = $"ErrorMsg:{e.Message},{Environment.NewLine}{e.StackTrace}";
                return result;
            }

            return result;
        }

        [Route("api/User/MaintenUserPWD")]
        [HttpPost]
        public ResultViewModels<string> MaintenUserPWD(UserPWDRecordViewModel model)
        {
            ResultViewModels<string> result = new ResultViewModels<string>();

            using (var uow = UnitOfWorkFactory.Create())
            {
                var userPWDRepo = new UserPWDRepository(uow);
                var actionMsgRepos = new UserActionMsgRepository(uow);


                #region 更新UserPWD_UPD
                try
                {
                    userPWDRepo.UserPWD_UPD(model);
                }
                catch (Exception e)
                {
                    result.ResultStatus = Status.fail;
                    result.Msg = $"ErrorMsg:UserPWD_UPD Error {Environment.NewLine}{e.Message},{Environment.NewLine}{e.StackTrace}";//更新UserPWD_UPD Error

                    uow.Dispose();
                    return result;
                }
                #endregion

                #region 紀錄
                try
                {
                    UserActionMsgViewModel actionMsg = new UserActionMsgViewModel();

                    string action = string.Empty;
                    if (model.deleteflag)
                    {
                        action = "Account_Disabled";
                    }
                    else
                    {
                        action = "Account_Enable";
                    }
                    actionMsg.Appid = "DAQXQLITE";
                    actionMsg.userid = model.userid;
                    actionMsg.Action = action;
                    actionMsg.Msg = model.Msg;
                    actionMsg.IpAddress = AppHelper.GetIPAddress();
                    actionMsgRepos.UserActionMsg_INS(actionMsg);
                }
                catch (Exception e)
                {
                    result.ResultStatus = Status.fail;
                    result.Msg = $"ErrorMsg:UserActionMsg Error {Environment.NewLine}{e.Message},{Environment.NewLine}{e.StackTrace}"; ;//新增UserActionMsg Error

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

        [Route("api/User/GetUserAccountAction")]
        [HttpPost]
        public ResultViewModels<List<UserActionMsgViewModel>> GetUserAccountAction(UserActionMsgViewModel model)
        {
            ResultViewModels<List<UserActionMsgViewModel>> result = new ResultViewModels<List<UserActionMsgViewModel>>();

            try
            {
                using (var uow = UnitOfWorkFactory.Create())
                {
                    var userActionMsgRepos = new UserActionMsgRepository(uow);
                    result.ResultStatus = Status.success;
                    result.Result = userActionMsgRepos.UserActoinMsg_SEL(model).AsEnumerable().Where(p => p.Action == "Account_Disabled" || p.Action == "Account_Enable").ToList();
                    userActionMsgRepos.Dispose();
                }
            }
            catch (Exception e)
            {
                result.ResultStatus = Status.fail;
                result.Msg = $"ErrorMsg:{e.Message},{Environment.NewLine}{e.StackTrace}";
                return result;
            }

            return result;
        }

        [Route("api/User/UserPayRecord_UPDAddress")]
        [HttpPost]
        public ResultViewModels<string> UserPayRecord_UPDAddress(UserPayRecordViewModel model)
        {
            ResultViewModels<string> result = new ResultViewModels<string>();

            try
            {
                using (var uow = UnitOfWorkFactory.Create())
                {
                    var userPayRecordRepo = new UserPayRecordRepository(uow);
                    userPayRecordRepo.UserPayRecord_UPDAddress(model);
                    userPayRecordRepo.SaveChanges();
                    userPayRecordRepo.Dispose();
                }
            }
            catch (Exception e)
            {
                result.ResultStatus = Status.fail;
                result.Msg = $"ErrorMsg:{e.Message},{Environment.NewLine}{e.StackTrace}";
                return result;
            }

            result.ResultStatus = Status.success;
            result.Msg = "0";//成功

            return result;
        }

    }
}
