using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XQLiteMgm.Models;
using XQLiteMgm.Models.Repository.Tables;
using XQLiteMgm.Models.UnitOfWork;

namespace XQLiteMgm.Controllers.WebAPI
{
    public class UserLogQueryController : ApiController
    {
        [Route("api/UserLog/GetUserIdList")]
        [HttpPost]
        public ResultViewModels<List<CustomerDetailViewModel>> GetUserIdList(CustomerDetailViewModel model)
        {
            ResultViewModels<List<CustomerDetailViewModel>> result = new ResultViewModels<List<CustomerDetailViewModel>>();

            try
            {
                using (var uow = UnitOfWorkFactory.Create())
                {
                    var customerDetailRepos = new CustomerDetailRepository(uow);
                    result.Result = customerDetailRepos.GetUserIdList(model.userid);
                    customerDetailRepos.Dispose();
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

        [Route("api/UserLog/GetUserLogList")]
        [HttpPost]
        public ResultViewModels<List<UserLogViewModel>> GetUserLogList(UserLogViewModel model)
        {
            ResultViewModels<List<UserLogViewModel>> result = new ResultViewModels<List<UserLogViewModel>>();

            try
            {
                using (var uow = UnitOfWorkFactory.Create())
                {
                    var userLogRepos = new UserLogRepository(uow);
                    result.Result = userLogRepos.GetUserLogList(model.userid);
                    userLogRepos.Dispose();
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
