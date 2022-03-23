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
    public class UserAppAuthRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public UserAppAuthRepository(IUnitOfWork uow)
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
        /// 新增 UserAppAuth
        /// </summary>
        /// <param name="model"></param>
        public void UserAppAuth_INS(UserAppAuthViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"UserAppAuth_INS";

                cmd.Parameters.Clear();

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = model.userid;
                cmd.Parameters.Add(paramUserid);

                var paramAppid = cmd.CreateParameter();
                paramAppid.ParameterName = "@appid";
                paramAppid.Value = model.authid;
                cmd.Parameters.Add(paramAppid);

                var paramEnddate = cmd.CreateParameter();
                paramEnddate.ParameterName = "@enddate";
                paramEnddate.Value = model.enddate;
                cmd.Parameters.Add(paramEnddate);

                cmd.ExecuteNonQuery();
            }
        }

        public UserAppAuthViewModel UserAppAuth_SEL(string userid, string appid)
        {
            UserAppAuthViewModel appAuth = null;

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"UserAppAuth_SEL";
                cmd.Parameters.Clear();

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = userid;
                cmd.Parameters.Add(paramUserid);

                var paramAppid = cmd.CreateParameter();
                paramAppid.ParameterName = "@appid";
                paramAppid.Value = appid;
                cmd.Parameters.Add(paramAppid);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        appAuth = new UserAppAuthViewModel();
                        appAuth.userid = reader["userid"].ToString();
                        appAuth.authid = reader["authid"].ToString();
                        appAuth.startdate = reader["startdate"] == DBNull.Value ? null : (DateTime?)reader["startdate"];
                        appAuth.enddate = reader["enddate"] == DBNull.Value ? null : (DateTime?)reader["enddate"];
                        appAuth.LastAuthTime = reader["LastAuthTime"] == DBNull.Value ? null : (DateTime?)reader["LastAuthTime"];
                        appAuth.IsValid = reader["IsValid"].ToString();
                    }
                }

                return appAuth;
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
    public class UserAppAuthViewModel
    {
        public string userid { get; set; }
        public string authid { get; set; }
        public Nullable<System.DateTime> startdate { get; set; }
        public Nullable<System.DateTime> enddate { get; set; }
        public Nullable<System.DateTime> LastAuthTime { get; set; }
        public string IsValid { get; set; }
    }

    #endregion
}