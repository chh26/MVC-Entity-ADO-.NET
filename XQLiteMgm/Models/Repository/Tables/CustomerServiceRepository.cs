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
    public class CustomerServiceRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public CustomerServiceRepository(IUnitOfWork uow)
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
        /// 新增 CustomerService
        /// </summary>
        /// <param name="model"></param>
        public void CustomerServiceInsert(CustomerServiceViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"CustomerServiceInsert";

                cmd.Parameters.Clear();

                var paramCustno = cmd.CreateParameter();
                paramCustno.ParameterName = "@custno";
                paramCustno.Value = model.custno;
                cmd.Parameters.Add(paramCustno);

                var paramServicedate = cmd.CreateParameter();
                paramServicedate.ParameterName = "@servicedate";
                paramServicedate.Value = model.servicedate;
                cmd.Parameters.Add(paramServicedate);

                var paramServiceitem = cmd.CreateParameter();
                paramServiceitem.ParameterName = "@serviceitem";
                paramServiceitem.Value = model.serviceitem;
                cmd.Parameters.Add(paramServiceitem);

                var paramServicedetail = cmd.CreateParameter();
                paramServicedetail.ParameterName = "@servicedetail";
                paramServicedetail.Value = model.servicedetail;
                cmd.Parameters.Add(paramServicedetail);

                var paramOpseq = cmd.CreateParameter();
                paramOpseq.ParameterName = "@opseq";
                paramOpseq.Value = model.opseq;
                cmd.Parameters.Add(paramOpseq);

                var paramOperip = cmd.CreateParameter();
                paramOperip.ParameterName = "@operip";
                paramOperip.Value = model.operIP;
                cmd.Parameters.Add(paramOperip);

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
    public class CustomerServiceViewModel
    {
        public int csSeq { get; set; }
        public string custno { get; set; }
        public Nullable<System.DateTime> servicedate { get; set; }
        public string serviceitem { get; set; }
        public string servicedetail { get; set; }
        public int opseq { get; set; }
        public string operIP { get; set; }
    }

    #endregion
}