using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using XQLiteMgm.Models.Interface;
using XQLiteMgm.Models.UnitOfWork;
using XQLiteMgm.Models.UnitOfWork.Interface;

namespace XQLiteMgm.Models.Repository.Tables
{
    public class UserExtraAuthRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public UserExtraAuthRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }
        public void UserExtraAuth_UPD(UserExtraAuthViewModel model)
        {
            UserExtraAuth_INS(model);
        }

        public void UserExtraAuth_INS(UserExtraAuthViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserExtraAuth_INS";
                cmd.Parameters.Clear();

                var paramUserId = cmd.CreateParameter();
                paramUserId.ParameterName = "@userid";
                paramUserId.Value = model.userid;
                cmd.Parameters.Add(paramUserId);

                var paramExtraid = cmd.CreateParameter();
                paramExtraid.ParameterName = "@extraid";
                paramExtraid.Value = model.extraid;
                cmd.Parameters.Add(paramExtraid);

                var paramExtraval = cmd.CreateParameter();
                paramExtraval.ParameterName = "@extraval";
                paramExtraval.Value = model.extraval;
                cmd.Parameters.Add(paramExtraval);

                var paramEnddate = cmd.CreateParameter();
                paramEnddate.ParameterName = "@enddate";
                paramEnddate.Value = model.enddate;
                cmd.Parameters.Add(paramEnddate);

                var paramLastupdate = cmd.CreateParameter();
                paramLastupdate.ParameterName = "@lastupdate";
                paramLastupdate.Value = DateTime.Now;
                cmd.Parameters.Add(paramLastupdate);

                cmd.ExecuteNonQuery();

            }
        }

        public void Dispose()
        {
            _unitOfWork.SaveChanges();
        }

        public void SaveChanges()
        {
            _unitOfWork.Dispose();
        }
    }

    public class UserExtraAuthViewModel
    {
        public string userid { get; set; }
        public string extraid { get; set; }
        public string extraval { get; set; }
        public System.DateTime startdate { get; set; }
        public System.DateTime enddate { get; set; }
        public System.DateTime lastupdate { get; set; }
    }
}