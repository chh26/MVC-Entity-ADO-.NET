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
    public class UserNewsAuthRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public UserNewsAuthRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        public void UserXQLiteNewsAuth_INS(UserNewsAuthViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserXQLiteNewsAuth_INS";
                cmd.Parameters.Clear();

                var paramUserId = cmd.CreateParameter();
                paramUserId.ParameterName = "@userid";
                paramUserId.Value = model.userid;
                cmd.Parameters.Add(paramUserId);

                var paramNewsid = cmd.CreateParameter();
                paramNewsid.ParameterName = "@newsid";
                paramNewsid.Value = model.newsid;
                cmd.Parameters.Add(paramNewsid);

                var paramEnddate = cmd.CreateParameter();
                paramEnddate.ParameterName = "@enddate";
                paramEnddate.Value = model.enddate;
                cmd.Parameters.Add(paramEnddate);

                cmd.ExecuteNonQuery();
            }
        }

        public List<UserNewsAuthViewModel> UserNewsAuth_SEL(UserNewsAuthViewModel model)
        {
            List<UserNewsAuthViewModel> list = new List<UserNewsAuthViewModel>();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserNewsAuth_SEL";

                cmd.Parameters.Clear();

                var paramUserId = cmd.CreateParameter();
                paramUserId.ParameterName = "@userid";
                paramUserId.Value = model.userid;
                cmd.Parameters.Add(paramUserId);

                var paramExtraid = cmd.CreateParameter();
                paramExtraid.ParameterName = "@newsid";
                paramExtraid.Value = model.newsid;
                cmd.Parameters.Add(paramExtraid);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                list = dt.AsEnumerable().Select(u =>
                new UserNewsAuthViewModel()
                {
                    userid = u["userid"].ToString(),
                    newsid = u["authid"].ToString(),
                    startdate = u["startdate"] == DBNull.Value ? null : (DateTime?)u["startdate"],
                    enddate = u["enddate"] == DBNull.Value ? null : (DateTime?)u["enddate"],
                    lastupdate = u["LastAuthTime"] == DBNull.Value ? null : (DateTime?)u["LastAuthTime"],
                }).ToList();
            }

            return list;
        }

        public void UserNewsAuth_UPD(UserNewsAuthViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserNewsAuth_UPD";
                cmd.Parameters.Clear();

                var paramUserId = cmd.CreateParameter();
                paramUserId.ParameterName = "@userid";
                paramUserId.Value = model.userid;
                cmd.Parameters.Add(paramUserId);

                var paramExtraid = cmd.CreateParameter();
                paramExtraid.ParameterName = "@newsid";
                paramExtraid.Value = model.newsid;
                cmd.Parameters.Add(paramExtraid);

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

        public void Dispose()
        {
            _unitOfWork.SaveChanges();
        }

        public void SaveChanges()
        {
            _unitOfWork.Dispose();
        }
    }

    public class UserNewsAuthViewModel
    {
        public string userid { get; set; }
        public string newsid { get; set; }
        public Nullable<System.DateTime> startdate { get; set; }
        public Nullable<System.DateTime> enddate { get; set; }
        public Nullable<System.DateTime> lastupdate { get; set; }
    }
}