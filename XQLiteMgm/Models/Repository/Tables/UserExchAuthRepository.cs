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
    public class UserExchAuthRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public UserExchAuthRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        public void UserExchAuth_UPD(UserExchAuthViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserExchAuth_UPD";
                cmd.Parameters.Clear();

                var paramUserId = cmd.CreateParameter();
                paramUserId.ParameterName = "@userid";
                paramUserId.Value = model.userid;
                cmd.Parameters.Add(paramUserId);

                var paramExchid = cmd.CreateParameter();
                paramExchid.ParameterName = "@exchid";
                paramExchid.Value = model.exchid;
                cmd.Parameters.Add(paramExchid);

                var paramStartdate = cmd.CreateParameter();
                paramStartdate.ParameterName = "@startdate";
                paramStartdate.Value = model.startdate;
                cmd.Parameters.Add(paramStartdate);

                var paramEnddate = cmd.CreateParameter();
                paramEnddate.ParameterName = "@enddate";
                paramEnddate.Value = model.enddate;
                cmd.Parameters.Add(paramEnddate);

                cmd.ExecuteNonQuery();
            }
        }

        public void UserXQLiteExchAuth_INS(UserExchAuthViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserXQLiteExchAuth_INS";
                cmd.Parameters.Clear();

                var paramUserId = cmd.CreateParameter();
                paramUserId.ParameterName = "@userid";
                paramUserId.Value = model.userid;
                cmd.Parameters.Add(paramUserId);

                var paramExchid = cmd.CreateParameter();
                paramExchid.ParameterName = "@exchid";
                paramExchid.Value = model.exchid;
                cmd.Parameters.Add(paramExchid);

                var paramAttrvalue = cmd.CreateParameter();
                paramAttrvalue.ParameterName = "@attrvalue";
                paramAttrvalue.Value = model.attrvalue;
                cmd.Parameters.Add(paramAttrvalue);

                var paramDescription = cmd.CreateParameter();
                paramDescription.ParameterName = "@description";
                paramDescription.Value = model.description;
                cmd.Parameters.Add(paramDescription);

                var paramEnddate = cmd.CreateParameter();
                paramEnddate.ParameterName = "@enddate";
                paramEnddate.Value = model.enddate;
                cmd.Parameters.Add(paramEnddate);

                cmd.ExecuteNonQuery();
            }
        }

        public List<UserExchAuthViewModel> UserExchAuth_SEL(UserExchAuthViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserExchAuth_SEL";
                cmd.Parameters.Clear();

                var paramUserId = cmd.CreateParameter();
                paramUserId.ParameterName = "@userid";
                paramUserId.Value = model.userid;
                cmd.Parameters.Add(paramUserId);

                var paramExchid = cmd.CreateParameter();
                paramExchid.ParameterName = "@exchid";
                paramExchid.Value = model.exchid;
                cmd.Parameters.Add(paramExchid);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                List<UserExchAuthViewModel> list = new List<UserExchAuthViewModel>();

                list = dt.AsEnumerable().Select(u =>
                new UserExchAuthViewModel()
                {
                    userid = u["userid"].ToString(),
                    exchid = u["authid"].ToString(),
                    startdate = u["startdate"] == DBNull.Value ? null : (DateTime?)u["startdate"],
                    enddate = u["enddate"] == DBNull.Value ? null : (DateTime?)u["enddate"],
                    lastupdate = u["LastAuthTime"] == DBNull.Value ? null : (DateTime?)u["LastAuthTime"],
                    defaultExch = u["defaultExch"].ToString(),
                    IsPush = u["IsPush"].ToString()
                }).ToList();

                return list;
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

    public class UserExchAuthViewModel
    {
        public string userid { get; set; }
        public string exchid { get; set; }
        public Nullable<System.DateTime> startdate { get; set; }
        public Nullable<System.DateTime> enddate { get; set; }
        public Nullable<System.DateTime> lastupdate { get; set; }
        public string defaultExch { get; set; }
        public string IsPush { get; set; }
        public string attrvalue { get; set; }
        public string description { get; set; }
    }
}