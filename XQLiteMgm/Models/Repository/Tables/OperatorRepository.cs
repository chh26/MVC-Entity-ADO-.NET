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
    public class OperatorRepository : IRepository<OperatorViewModel>
    {
        private SQLUnitOfWork _unitOfWork;

        public OperatorRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        public OperatorViewModel GetSysUser(string account, string pw)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"OperatorLogin";
                cmd.Parameters.Clear();

                var paramAccount = cmd.CreateParameter();
                paramAccount.ParameterName = "@opname";
                paramAccount.Value = account;
                cmd.Parameters.Add(paramAccount);

                //var paramPW = cmd.CreateParameter();
                //paramPW.ParameterName = "@pw";
                //paramPW.Value = pw;
                //cmd.Parameters.Add(paramPW);

                OperatorViewModel user = new OperatorViewModel();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user.opseq = reader["opseq"].ToString();
                        user.name = reader["name"].ToString();
                        user.password = reader["password"].ToString();
                        user.privelege = reader["privelege"].ToString();
                        user.crdate = reader["crdate"].ToString();
                        user.remark = reader["remark"].ToString();
                    }
                }

                return user;
            }
        }

        public bool UpdateSysUserPW(string account, string newPassword)
        {
            try
            {
                using (var cmd = _unitOfWork.CreateCommand())
                {
                    //cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = @"
                                    UPDATE [dbo].[Operator] 
	                                SET 
		                                [password] = @pw
	                                WHERE 
		                                [name] = @userid";
                    cmd.Parameters.Clear();

                    var paramAccount = cmd.CreateParameter();
                    paramAccount.ParameterName = "@userid";
                    paramAccount.Value = account;
                    cmd.Parameters.Add(paramAccount);

                    var paramPW = cmd.CreateParameter();
                    paramPW.ParameterName = "@pw";
                    paramPW.Value = newPassword;
                    cmd.Parameters.Add(paramPW);

                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public List<OperatorViewModel> GetSysUserList()
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"OperatorSelectAll";

                cmd.Parameters.Clear();

                List<OperatorViewModel> userList = new List<OperatorViewModel>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OperatorViewModel user = new OperatorViewModel();

                        user.opseq = reader["opseq"].ToString();
                        user.name = reader["name"].ToString();
                        user.password = reader["password"].ToString();
                        user.privelege = reader["privelege"].ToString();
                        user.crdate = reader["crdate"].ToString();
                        user.remark = reader["remark"].ToString();

                        userList.Add(user);
                    }
                }

                return userList;
            }
        }

        public bool CheckIsAccountDuplicate(string userid)
        {
            bool isDuplicate = false;//帳號不重覆
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"OperatorLogin";

                cmd.Parameters.Clear();

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@opname";
                paramUserid.Value = userid;
                cmd.Parameters.Add(paramUserid);

                OperatorViewModel user = new OperatorViewModel();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user.name = reader["UserID"].ToString();
                    }
                }

                if (user.name != null)
                {
                    isDuplicate = true;
                }
            }

            return isDuplicate;
        }

        public void InsertSysUser(string userid, string pw)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"OperatorInsert";

                cmd.Parameters.Clear();

                var paramUserID = cmd.CreateParameter();
                paramUserID.ParameterName = "@name";
                paramUserID.Value = userid;
                cmd.Parameters.Add(paramUserID);

                var paramPW = cmd.CreateParameter();
                paramPW.ParameterName = "@password";
                paramPW.Value = pw;
                cmd.Parameters.Add(paramPW);

                var paramPrivelege = cmd.CreateParameter();
                paramPrivelege.ParameterName = "@privelege";
                paramPrivelege.Value = 0;
                cmd.Parameters.Add(paramPrivelege);

                var paramCrdate = cmd.CreateParameter();
                paramCrdate.ParameterName = "@crdate";
                paramCrdate.Value = DateTime.Now;
                cmd.Parameters.Add(paramCrdate);

                var paramRemark = cmd.CreateParameter();
                paramRemark.ParameterName = "@remark";
                paramRemark.Value = string.Empty;
                cmd.Parameters.Add(paramRemark);

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteSysUser(string opseq)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"OperatorDelete";

                cmd.Parameters.Clear();

                var paramOpseq = cmd.CreateParameter();
                paramOpseq.ParameterName = "@opseq";
                paramOpseq.Value = opseq;
                cmd.Parameters.Add(paramOpseq);

                cmd.ExecuteNonQuery();
            }
        }

        public void SaveChanges()
        {
            _unitOfWork.SaveChanges();
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        #region 用不到的
        public void Create(OperatorViewModel instance)
        {
            throw new NotImplementedException();
        }

        public void Delete(OperatorViewModel instance)
        {
            throw new NotImplementedException();
        }

        public OperatorViewModel Get(int primaryID)
        {
            throw new NotImplementedException();
        }

        public IQueryable<OperatorViewModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(OperatorViewModel instance)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    #region Model
    public class OperatorViewModel
    {
        public string opseq { get; set; }
        /// <summary>
        /// 帳號
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 權限
        /// </summary>
        public string privelege { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public string crdate { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string remark { get; set; }

    }
    #endregion

}