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
    public class UserActionQueryController : ApiController
    {
        [Route("api/User/GetUserActionRecord")]
        [HttpPost]
        public ResultViewModels<List<UserActionMsgViewModel>> GetUserActionRecord(UserActionMsgViewModel model)
        {
            ResultViewModels<List<UserActionMsgViewModel>> result = new ResultViewModels<List<UserActionMsgViewModel>>();

            try
            {
                using (var uow = UnitOfWorkFactory.Create())
                {
                    var userActionMsgRepos = new UserActionMsgRepository(uow);
                    result.ResultStatus = Status.success;
                    result.Result = userActionMsgRepos.UserActoinMsg_SEL(model).AsEnumerable().Where(p => p.Action != "BUY_MAIL").OrderByDescending(a => a.CreateTime).ThenBy(a => a.Action).ThenBy(a => a.Msg).ToList();
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
    }
}
