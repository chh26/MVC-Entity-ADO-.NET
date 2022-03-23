using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XQLiteMgm.Models;
using XQLiteMgm.Models.Repository.Tables;
using XQLiteMgm.Models.UnitOfWork;

namespace XQLiteMgm.Controllers.WebAPI
{
    public class SysAccManagementController : ApiController
    {
        string connString = ConfigurationManager.ConnectionStrings["UserDB_Just"].ConnectionString;

        [Route("api/System/GetSysUserList")]
        [HttpGet]
        public List<OperatorViewModel> GetSysUserList()
        {
            List<OperatorViewModel> list = new List<OperatorViewModel>();
            using (var uow = UnitOfWorkFactory.Create(connString))
            {
                var userRepo = new OperatorRepository(uow);
                list = userRepo.GetSysUserList();
                userRepo.Dispose();
            }

            return list;
        }

        [Route("api/System/ResetSysUserPW")]
        [HttpPost]
        public void ResetSysUserPW(OperatorViewModel model)
        {
            using (var uow = UnitOfWorkFactory.Create(connString))
            {
                string newPassword = "123456";

                var userRepo = new OperatorRepository(uow);
                userRepo.UpdateSysUserPW(model.name, newPassword);
                userRepo.SaveChanges();
                userRepo.Dispose();
            }

        }

        [Route("api/System/CheckIsAccountDuplicate")]
        [HttpPost]
        public bool CheckIsAccountDuplicate(OperatorViewModel model)
        {
            bool isDuplicate = false;//帳號不重覆

            using (var uow = UnitOfWorkFactory.Create(connString))
            {
                var userRepo = new OperatorRepository(uow);

                isDuplicate = userRepo.CheckIsAccountDuplicate(model.name);
                userRepo.Dispose();
            }

            return isDuplicate;
        }

        [Route("api/System/InsertOperator")]
        [HttpPost]
        public void InsertOperator(OperatorViewModel model)
        {
            using (var uow = UnitOfWorkFactory.Create(connString))
            {
                var userRepo = new OperatorRepository(uow);

                userRepo.InsertSysUser(model.name, model.password);
                userRepo.SaveChanges();
                userRepo.Dispose();
            }
        }

        [Route("api/System/DeleteSysUser")]
        [HttpPost]
        public void DeleteSysUser(OperatorViewModel model)
        {
            using (var uow = UnitOfWorkFactory.Create(connString))
            {
                var userRepo = new OperatorRepository(uow);
                userRepo.DeleteSysUser(model.name);
                userRepo.SaveChanges();
                userRepo.Dispose();
            }
        }
    }
}
