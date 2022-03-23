using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using XQLiteMgm.Helper;
using XQLiteMgm.Models;
using XQLiteMgm.Models.Interface;
using XQLiteMgm.Models.UnitOfWork;
using XQLiteMgm.Models.UnitOfWork.Interface;

namespace XQLiteMgm.Models.Repository.Tables
{
    public class UserPWDRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public UserPWDRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        /// <summary>
        /// 新增 UserPWD
        /// </summary>
        /// <param name="model"></param>
        public void UserPWD_INS(UserPWDRecordViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"UserPWD_INS";

                cmd.Parameters.Clear();

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = model.userid;
                cmd.Parameters.Add(paramUserid);

                var paramPwd = cmd.CreateParameter();
                paramPwd.ParameterName = "@pwd";
                paramPwd.Value = model.pwd;
                cmd.Parameters.Add(paramPwd);

                cmd.ExecuteNonQuery();
            }
        }

        public UserPWDRecordViewModel UserPWD_SEL(string userid)
        {
            UserPWDRecordViewModel userPwd = null;

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"UserPWD_SEL";
                cmd.Parameters.Clear();

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = userid;
                cmd.Parameters.Add(paramUserid);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        userPwd = new UserPWDRecordViewModel();
                        userPwd.userid = reader["userid"].ToString();
                        userPwd.deleteflag = reader["deleteflag"].ToString().ToLower() == "n" ? false : true;
                        userPwd.pwd = reader["pwd"].ToString();
                        userPwd.attempt = Convert.ToInt16(reader["attempt"].ToString());
                        userPwd.islock = reader["lock"].ToString().ToLower() == "n" ? false : true;
                        userPwd.createdate = reader["createdate"] == DBNull.Value ? null : (DateTime?)reader["createdate"];
                        userPwd.lastupdate = reader["lastupdate"] == DBNull.Value ? null : (DateTime?)reader["lastupdate"];
                    }
                }

                return userPwd;
            }
            
        }

        public bool UserPWD_UPD(UserPWDRecordViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"UserPWD_UPD";

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = model.userid;
                cmd.Parameters.Add(paramUserid);

                var paramPwd = cmd.CreateParameter();
                paramPwd.ParameterName = "@pwd";
                paramPwd.Value = DBNull.Value;
                cmd.Parameters.Add(paramPwd);

                var paramDeleteflag = cmd.CreateParameter();
                paramDeleteflag.ParameterName = "@deleteflag";
                string deleteflag = model.deleteflag == true ? "y" : "n";
                paramDeleteflag.Value = deleteflag;
                cmd.Parameters.Add(paramDeleteflag);

                var paramAttempt = cmd.CreateParameter();
                paramAttempt.ParameterName = "@attempt";
                paramAttempt.Value = DBNull.Value;
                cmd.Parameters.Add(paramAttempt);

                var paramLock = cmd.CreateParameter();
                paramLock.ParameterName = "@lock";
                paramLock.Value = DBNull.Value;
                cmd.Parameters.Add(paramLock);

                cmd.ExecuteNonQuery();

                return true;
            }
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

    #region ViewModel
    public class UserPWDRecordViewModel
    {
        public string userid { get; set; }
        public bool deleteflag { get; set; }
        public string pwd { get; set; }
        public int attempt { get; set; }
        public bool islock { get; set; }
        public int noticeSeq { get; set; }
        public string Msg { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<System.DateTime> lastupdate { get; set; }
    }

    #endregion
}