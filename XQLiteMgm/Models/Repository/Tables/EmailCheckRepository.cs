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
    public class EmailCheckRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public EmailCheckRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        public bool CheckEmailExcludeUserid(string email, string userid)
        {
            DataSet ds = new DataSet();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"CustomerMasterSelectEmailExcludeUserid";


                var paramEmail = cmd.CreateParameter();
                paramEmail.ParameterName = "@email";
                paramEmail.Value = email;
                cmd.Parameters.Add(paramEmail);

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = userid;
                cmd.Parameters.Add(paramUserid);

                ds.Load(cmd.ExecuteReader(), LoadOption.OverwriteChanges, "");
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return true;//"EMail重複使用"
            }
            else
            {
                return false;
            }
        }

        public bool EmailCheck_INS(string Email, Guid SEQNO, string Active, int EmailValidDays)
        {
            try
            {
                using (var cmd = _unitOfWork.CreateCommand())
                {
                    cmd.Parameters.Clear();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = @"EmailCheck_INS";

                    var paramEmail = cmd.CreateParameter();
                    paramEmail.ParameterName = "@Email";
                    paramEmail.Value = Email;
                    cmd.Parameters.Add(paramEmail);

                    var paramSEQNO = cmd.CreateParameter();
                    paramSEQNO.ParameterName = "@SEQNO";
                    paramSEQNO.Value = SEQNO;
                    cmd.Parameters.Add(paramSEQNO);

                    var paramActive = cmd.CreateParameter();
                    paramActive.ParameterName = "@Active";
                    paramActive.Value = Active;
                    cmd.Parameters.Add(paramActive);

                    var paramEmailValidDays = cmd.CreateParameter();
                    paramEmailValidDays.ParameterName = "@EmailValidDays";
                    paramEmailValidDays.Value = EmailValidDays;
                    cmd.Parameters.Add(paramEmailValidDays);

                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EmailCheck2_INS(string Email, Guid SEQNO, string Active, int EmailValidDays)
        {
            try
            {
                using (var cmd = _unitOfWork.CreateCommand())
                {
                    cmd.Parameters.Clear();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = @"EmailCheck_INS";

                    var paramEmail = cmd.CreateParameter();
                    paramEmail.ParameterName = "@Email";
                    paramEmail.Value = Email;
                    cmd.Parameters.Add(paramEmail);

                    var paramSEQNO = cmd.CreateParameter();
                    paramSEQNO.ParameterName = "@SEQNO";
                    paramSEQNO.Value = SEQNO;
                    cmd.Parameters.Add(paramSEQNO);

                    var paramActive = cmd.CreateParameter();
                    paramActive.ParameterName = "@Active";
                    paramActive.Value = Active;
                    cmd.Parameters.Add(paramActive);

                    var paramEmailValidDays = cmd.CreateParameter();
                    paramEmailValidDays.ParameterName = "@EmailValidDays";
                    paramEmailValidDays.Value = EmailValidDays;
                    cmd.Parameters.Add(paramEmailValidDays);

                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
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
    }
}