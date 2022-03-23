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
    public class OrderMaintenanceController : ApiController
    {
        [Route("api/Order/GetUserPayRecord")]
        [HttpPost]
        public ResultViewModels<List<UserPayRecordViewModel>> GetUserPayRecord(UserPayRecordViewModel model)
        {
            ResultViewModels<List<UserPayRecordViewModel>> result = new ResultViewModels<List<UserPayRecordViewModel>>();

            try
            {
                using (var uow = UnitOfWorkFactory.Create())
                {
                    var userPayRecordRepos = new UserPayRecordRepository(uow);
                    result.Result = userPayRecordRepos.GetUserPayRecord(model.userid, model.opid);
                    userPayRecordRepos.Dispose();
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
