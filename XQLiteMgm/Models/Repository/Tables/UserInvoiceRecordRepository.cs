using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using XQLiteMgm.Models.UnitOfWork;
using XQLiteMgm.Models.UnitOfWork.Interface;

namespace XQLiteMgm.Models.Repository.Tables
{
    public class UserInvoiceRecordRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public UserInvoiceRecordRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        public List<UserInvoiceRecordViewModel> UserInvoiceRecord_SEL_PayRecordSEQNO(string payRecordSEQNO)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserInvoiceRecord_SEL_PayRecordSEQNO";
                cmd.Parameters.Clear();

                var paramPayRecordSEQNO = cmd.CreateParameter();
                paramPayRecordSEQNO.ParameterName = "@PayRecordSEQNO";
                paramPayRecordSEQNO.Value = payRecordSEQNO;
                cmd.Parameters.Add(paramPayRecordSEQNO);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                List<UserInvoiceRecordViewModel> list = new List<UserInvoiceRecordViewModel>();

                list = dt.AsEnumerable().Select(u =>
                new UserInvoiceRecordViewModel()
                {
                    SEQNO = u["SEQNO"].ToString(),
                    PayRecordSEQNO = u["PayRecordSEQNO"].ToString(),
                    userid = u["userid"].ToString(),
                    opid = u["opid"].ToString(),
                    opseq = u["opseq"].ToString(),
                    UniForm = u["UniForm"].ToString(),
                    UniNum = u["UniNum"].ToString(),
                    CarrierType = u["CarrierType"].ToString(),
                    CarrierId = u["CarrierId"].ToString(),
                    Npo = u["Npo"].ToString(),
                    Quantity = Convert.ToInt32(u["Quantity"]),
                    Status = u["Status"].ToString(),
                    InvoiceNo = u["InvoiceNo"].ToString(),
                    PayDate = Convert.ToDateTime(u["PayDate"].ToString()),
                    PayCdate = u["PayCdate"] == DBNull.Value ? null : (DateTime?)u["PayCdate"],
                    InvoiceSettingSEQNO = u["InvoiceSettingSEQNO"].ToString(),
                }).ToList();

                return list;
            }
        }

        public void UserInvoiceRecord_Cancel_UPD(UserInvoiceRecordViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserInvoiceRecord_Cancel_UPD";
                cmd.Parameters.Clear();

                var paramSEQNO = cmd.CreateParameter();
                paramSEQNO.ParameterName = "@SEQNO";
                paramSEQNO.Value = model.SEQNO;
                cmd.Parameters.Add(paramSEQNO);

                var paramInvoiceNo = cmd.CreateParameter();
                paramInvoiceNo.ParameterName = "@InvoiceNo";
                paramInvoiceNo.Value = model.InvoiceNo;
                cmd.Parameters.Add(paramInvoiceNo);

                var paramStatus = cmd.CreateParameter();
                paramStatus.ParameterName = "@Status";
                paramStatus.Value = model.Status;
                cmd.Parameters.Add(paramStatus);

                var paramPayCdate = cmd.CreateParameter();
                paramPayCdate.ParameterName = "@PayCdate";
                paramPayCdate.Value = model.PayCdate;
                cmd.Parameters.Add(paramPayCdate);

                cmd.ExecuteNonQuery();

            }
        }
    }

    public class UserInvoiceRecordViewModel
    {
        public string SEQNO { get; set; }
        public string PayRecordSEQNO { get; set; }
        public string OrderNO { get; set; }
        public string userid { get; set; }
        public string opid { get; set; }
        public string opseq { get; set; }
        public string UniForm { get; set; }
        public string UniNum { get; set; }
        public string CarrierType { get; set; }
        public string CarrierId { get; set; }
        public string Npo { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public string InvoiceNo { get; set; }
        public System.DateTime PayDate { get; set; }
        public Nullable<System.DateTime> PayCdate { get; set; }
        public string InvoiceSettingSEQNO { get; set; }
        public System.DateTime CreateTime { get; set; }
    }
}