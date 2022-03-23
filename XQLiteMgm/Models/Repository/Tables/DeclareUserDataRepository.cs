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
    public class DeclareUserDataRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public DeclareUserDataRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        internal void DeclareUserData_INS(DeclareUserDataViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "DeclareUserData_INS";
                cmd.Parameters.Clear();

                var paramUserID = cmd.CreateParameter();
                paramUserID.ParameterName = "@userid";
                paramUserID.Value = model.userid;
                cmd.Parameters.Add(paramUserID);

                var paramAppid = cmd.CreateParameter();
                paramAppid.ParameterName = "@appid";
                paramAppid.Value = model.appid;
                cmd.Parameters.Add(paramAppid);


                var paramName = cmd.CreateParameter();
                paramName.ParameterName = "@name";
                paramName.Value = model.name;
                cmd.Parameters.Add(paramName);

                var paramMobile = cmd.CreateParameter();
                paramMobile.ParameterName = "@mobile";
                paramMobile.Value = model.mobile;
                cmd.Parameters.Add(paramMobile);

                var paramEmail = cmd.CreateParameter();
                paramEmail.ParameterName = "@email";
                paramEmail.Value = model.email;
                cmd.Parameters.Add(paramEmail);

                var paramAddress = cmd.CreateParameter();
                paramAddress.ParameterName = "@address";
                paramAddress.Value = model.address;
                cmd.Parameters.Add(paramAddress);

                var paramIdkind = cmd.CreateParameter();
                paramIdkind.ParameterName = "@idkind";
                paramIdkind.Value = model.idkind;
                cmd.Parameters.Add(paramIdkind);

                var paramCountry = cmd.CreateParameter();
                paramCountry.ParameterName = "@country";
                paramCountry.Value = model.country;
                cmd.Parameters.Add(paramCountry);

                var paramSales = cmd.CreateParameter();
                paramSales.ParameterName = "@sales";
                paramSales.Value = model.sales;
                cmd.Parameters.Add(paramSales);

                var paramInternal = cmd.CreateParameter();
                paramInternal.ParameterName = "@internal";
                paramInternal.Value = model.Internal;
                cmd.Parameters.Add(paramInternal);

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

    public class DeclareUserDataViewModel
    {
        public string userid { get; set; }
        public string appid { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string idkind { get; set; }
        public string country { get; set; }
        public string sales { get; set; }
        public string Internal { get; set; }
        public Nullable<DateTime> createdate { get; set; }
        public Nullable<DateTime> lastupdate { get; set; }
        public string isdeclare { get; set; }
        public string syscheck { get; set; }
        public string Operator { get; set; }
    }
}