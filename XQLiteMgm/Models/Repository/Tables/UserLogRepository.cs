using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XQLiteMgm.Models.Interface;
using XQLiteMgm.Models.UnitOfWork;
using XQLiteMgm.Models.UnitOfWork.Interface;

namespace XQLiteMgm.Models.Repository.Tables
{
    public class UserLogRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public UserLogRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        public List<UserLogViewModel> GetUserLogList(string userid)
        {
            List<UserLogViewModel> list = new List<UserLogViewModel>();
            string sqlcmd = string.Empty;

            if (string.IsNullOrEmpty(userid))
            {
                sqlcmd = @"
                        SELECT 
                                TOP 50 
                                userid 
                                ,createtime
                                ,eventid
                                ,ip
                                ,status
                        FROM 
                                UserLog 
                        WHERE 
                                userid =userid 
                        order by createtime desc";
            }
            else
            {
                sqlcmd = @"
                        SELECT 
                                TOP 50 
                                userid 
                                ,createtime
                                ,eventid
                                ,ip
                                ,status
                        FROM 
                                UserLog 
                        WHERE 
                                userid = @userid 
                        order by createtime desc";
            }

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = sqlcmd;

                cmd.Parameters.Clear();

                if (!string.IsNullOrEmpty(userid))
                {
                    var paramUserid = cmd.CreateParameter();
                    paramUserid.ParameterName = "@userid";
                    paramUserid.Value = userid;
                    cmd.Parameters.Add(paramUserid);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UserLogViewModel viewModel = new UserLogViewModel();

                        viewModel.userid = reader["userid"].ToString();
                        viewModel.createtime = Convert.ToDateTime(reader["createtime"].ToString());
                        viewModel.eventid = Convert.ToInt32(reader["eventid"].ToString());
                        viewModel.ip = reader["ip"].ToString();
                        viewModel.status = Convert.ToInt16(reader["status"].ToString());


                        list.Add(viewModel);
                    }
                }
            }

            return list;
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

    public class UserLogViewModel
    {
        public string userid { get; set; }
        public System.DateTime createtime { get; set; }
        public int eventid { get; set; }
        public string ip { get; set; }
        public int status { get; set; }
    }
}