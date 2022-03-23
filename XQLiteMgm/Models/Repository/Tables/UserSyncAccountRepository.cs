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
    public class UserSyncAccountRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public UserSyncAccountRepository(IUnitOfWork uow)
        {
            if (uow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = uow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory  is null.");
            }
        }

        /// <summary>
        /// 新增 UserSyncAccount
        /// </summary>
        /// <param name="model"></param>
        public void UserSyncAccount_INS(UserSyncAccountViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"UserSyncAccount_INS";

                cmd.Parameters.Clear();

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = model.userid;
                cmd.Parameters.Add(paramUserid);

                var paramXQ = cmd.CreateParameter();
                paramXQ.ParameterName = "@xq";
                paramXQ.Value = model.XQ;
                cmd.Parameters.Add(paramXQ);

                var paramMoneydj = cmd.CreateParameter();
                paramMoneydj.ParameterName = "@moneydj";
                paramMoneydj.Value = model.Moneydj;
                cmd.Parameters.Add(paramMoneydj);

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

    #region ViewModel
    public class UserSyncAccountViewModel
    {
        public string userid { get; set; }
        public int XQ { get; set; }
        public int Moneydj { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
    }

    #endregion
}