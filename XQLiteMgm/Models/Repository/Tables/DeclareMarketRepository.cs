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
    public class DeclareMarketRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public DeclareMarketRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        internal void DeclareMarket_INS(DeclareMarketViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "DeclareMarket_INS";
                cmd.Parameters.Clear();

                var paramUserID = cmd.CreateParameter();
                paramUserID.ParameterName = "@userid";
                paramUserID.Value = model.userid;
                cmd.Parameters.Add(paramUserID);

                var paramAppid = cmd.CreateParameter();
                paramAppid.ParameterName = "@appid";
                paramAppid.Value = model.appid;
                cmd.Parameters.Add(paramAppid);

                var paramMarket = cmd.CreateParameter();
                paramMarket.ParameterName = "@market";
                paramMarket.Value = model.market;
                cmd.Parameters.Add(paramMarket);


                var paramStartdate = cmd.CreateParameter();
                paramStartdate.ParameterName = "@startdate";
                paramStartdate.Value = model.startdate;
                cmd.Parameters.Add(paramStartdate);

                var paramEnddate = cmd.CreateParameter();
                paramEnddate.ParameterName = "@enddate";
                paramEnddate.Value = model.enddate;
                cmd.Parameters.Add(paramEnddate);

                var paramCreatedate = cmd.CreateParameter();
                paramCreatedate.ParameterName = "@createdate";
                paramCreatedate.Value = model.createdate;
                cmd.Parameters.Add(paramCreatedate);

                var paramLastupdate = cmd.CreateParameter();
                paramLastupdate.ParameterName = "@lastupdate";
                paramLastupdate.Value = model.lastupdate;
                cmd.Parameters.Add(paramLastupdate);

                var paramIsdeclare = cmd.CreateParameter();
                paramIsdeclare.ParameterName = "@isdeclare";
                paramIsdeclare.Value = model.isdeclare;
                cmd.Parameters.Add(paramIsdeclare);

                var paramSyscheck = cmd.CreateParameter();
                paramSyscheck.ParameterName = "@syscheck";
                paramSyscheck.Value = model.syscheck;
                cmd.Parameters.Add(paramSyscheck);

                var paramOperator = cmd.CreateParameter();
                paramOperator.ParameterName = "@operator";
                paramOperator.Value = model.Operator;
                cmd.Parameters.Add(paramOperator);

                cmd.ExecuteNonQuery();
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

    public class DeclareMarketViewModel
    {
        public string userid { get; set; }
        public string appid { get; set; }
        public string market { get; set; }
        public Nullable<DateTime> startdate { get; set; }
        public Nullable<DateTime> enddate { get; set; }
        public Nullable<DateTime> createdate { get; set; }
        public Nullable<DateTime> lastupdate { get; set; }
        public string isdeclare { get; set; }
        public string syscheck { get; set; }
        public string Operator { get; set; }
        public int Seq { get; set; }
    }
}