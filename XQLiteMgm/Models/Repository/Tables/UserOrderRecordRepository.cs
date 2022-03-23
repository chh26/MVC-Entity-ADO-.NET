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
    public class UserOrderRecordRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public UserOrderRecordRepository(IUnitOfWork iuow)
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
        /// Get user order record by userid and opid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<UserOrderRecordViewModel> GetUserOrderRecord(MaintenOrderParamViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"
                                    SELECT
	                                    o.userid,
	                                    o.opseq,
	                                    p.opid,
	                                    p.cardSEQNO AS orderNo,
	                                    o.amount,
	                                    o.MSRP,
	                                    payFdate,
	                                    o.paySdate,
	                                    payCdate,
	                                    o.payEdate,
	                                    o.autopay,
	                                    o.SEQNO,
	                                    SystemMsg,
	                                    SystemStatus,
	                                    lastUpdate
                                    FROM
	                                    UserOrderRecord o
                                    INNER JOIN UserPayRecord p on o.SEQNO = p.SEQNO
                                    WHERE 
	                                    o.userid = @USERID 
                                    AND
	                                    o.autopay = 'true' 
                                    AND
	                                    getdate() < o.payEdate
                                    AND
	                                    p.opid = @opid";

                cmd.Parameters.Clear();

                var paramUserId = cmd.CreateParameter();
                paramUserId.ParameterName = "@userid";
                paramUserId.Value = model.userid;
                cmd.Parameters.Add(paramUserId);

                var paramOrderNo = cmd.CreateParameter();
                paramOrderNo.ParameterName = "@opid";
                paramOrderNo.Value = model.opid;
                cmd.Parameters.Add(paramOrderNo);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                List<UserOrderRecordViewModel> list = new List<UserOrderRecordViewModel>();

                list = dt.AsEnumerable().Select(u =>
                new UserOrderRecordViewModel()
                {
                    userid = u["userid"].ToString(),
                    opseq = u["opseq"].ToString(),
                    opid = u["opid"].ToString(),
                    orderNo = u["orderNo"].ToString(),
                    amount = u["amount"] == DBNull.Value ? null : (double?)u["amount"],
                    MSRP = u["MSRP"] == DBNull.Value ? null : (double?)u["MSRP"],
                    payFdate = Convert.ToDateTime(u["payFdate"].ToString()),
                    paySdate = Convert.ToDateTime(u["paySdate"].ToString()),
                    payCdate = u["payCdate"] == DBNull.Value ? null : (DateTime?)u["payCdate"],
                    payEdate = Convert.ToDateTime(u["payEdate"].ToString()),
                    autopay = u["autopay"].ToString(),
                    SEQNO = u["SEQNO"].ToString(),
                    SystemMsg = u["SystemMsg"].ToString(),
                    SystemStatus = u["SystemStatus"].ToString(),
                    lastUpdate = Convert.ToDateTime(u["lastUpdate"].ToString())
                }).ToList();

                return list;
            }
        }

        /// <summary>
        /// 取得user訂閱模組
        /// </summary>
        public List<UserOrderRecordViewModel> UserOrderRecord_SEL_Sub(string userid)
        {
            List<UserOrderRecordViewModel> list = new List<UserOrderRecordViewModel>();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                //cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"SELECT * from UserOrderRecord where userid=@USERID and (autopay='true' or autopay='MARKET' or autopay='sys' or autopay = 'FreeTrial') and payEdate>GETDATE()  order by opid";
                cmd.Parameters.Clear();

                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@USERID";
                paramText.Value = userid;
                cmd.Parameters.Add(paramText);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UserOrderRecordViewModel orderRecord = new UserOrderRecordViewModel();
                        orderRecord.userid = reader["userid"].ToString();
                        orderRecord.opseq = reader["opseq"].ToString();
                        orderRecord.opid = reader["opid"].ToString();
                        orderRecord.amount = reader["amount"] == DBNull.Value ? null : (double?)reader["amount"];
                        orderRecord.MSRP = reader["MSRP"] == DBNull.Value ? null : (double?)reader["MSRP"];
                        orderRecord.payFdate = Convert.ToDateTime(reader["payFdate"].ToString());
                        orderRecord.paySdate = Convert.ToDateTime(reader["paySdate"].ToString());
                        orderRecord.payCdate = reader["payCdate"] == DBNull.Value ? null : (DateTime?)reader["payCdate"];
                        orderRecord.payEdate = Convert.ToDateTime(reader["payEdate"].ToString());
                        orderRecord.autopay = reader["autopay"].ToString();
                        orderRecord.SEQNO = reader["SEQNO"].ToString();
                        orderRecord.SystemMsg = reader["SystemMsg"].ToString();
                        orderRecord.SystemStatus = reader["SystemStatus"].ToString();
                        orderRecord.lastUpdate = Convert.ToDateTime(reader["lastUpdate"].ToString());

                        list.Add(orderRecord);
                    }
                }
            }

            return list;
        }

        internal void UserOrderRecord_UPD(UserOrderRecordViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserOrderRecord_UPD";
                cmd.Parameters.Clear();

                var paramSEQNO = cmd.CreateParameter();
                paramSEQNO.ParameterName = "@SEQNO";
                paramSEQNO.Value = model.SEQNO;
                cmd.Parameters.Add(paramSEQNO);

                var paramOpid = cmd.CreateParameter();
                paramOpid.ParameterName = "@opid";
                paramOpid.Value = model.opid;
                cmd.Parameters.Add(paramOpid);

                var paramPayCdate = cmd.CreateParameter();
                paramPayCdate.ParameterName = "@payCdate";
                paramPayCdate.Value = model.payCdate;
                cmd.Parameters.Add(paramPayCdate);

                var paramPayEdate = cmd.CreateParameter();
                paramPayEdate.ParameterName = "@payEdate";
                paramPayEdate.Value = model.payEdate;
                cmd.Parameters.Add(paramPayEdate);

                var paramAutopay = cmd.CreateParameter();
                paramAutopay.ParameterName = "@autopay";
                paramAutopay.Value = model.autopay.ToLower();
                cmd.Parameters.Add(paramAutopay);

                cmd.ExecuteNonQuery();
            }
        }

        internal void UserOrderRecord_INS(UserOrderRecordViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserOrderRecord_INS";
                cmd.Parameters.Clear();

                var paramSEQNO = cmd.CreateParameter();
                paramSEQNO.ParameterName = "@SEQNO";
                paramSEQNO.Value = model.SEQNO;
                cmd.Parameters.Add(paramSEQNO);

                var paramUserID = cmd.CreateParameter();
                paramUserID.ParameterName = "@userid";
                paramUserID.Value = model.userid;
                cmd.Parameters.Add(paramUserID);

                var paramOpseq = cmd.CreateParameter();
                paramOpseq.ParameterName = "@opseq";
                paramOpseq.Value = model.opseq;
                cmd.Parameters.Add(paramOpseq);

                var paramOpid = cmd.CreateParameter();
                paramOpid.ParameterName = "@opid";
                paramOpid.Value = model.opid;
                cmd.Parameters.Add(paramOpid);

                var paramAmount = cmd.CreateParameter();
                paramAmount.ParameterName = "@amount";
                paramAmount.Value = model.amount;
                cmd.Parameters.Add(paramAmount);

                var paramMSRP = cmd.CreateParameter();
                paramMSRP.ParameterName = "@MSRP";
                paramMSRP.Value = model.MSRP;
                cmd.Parameters.Add(paramMSRP);

                var paramPaySdate = cmd.CreateParameter();
                paramPaySdate.ParameterName = "@paySdate";
                paramPaySdate.Value = model.paySdate;
                cmd.Parameters.Add(paramPaySdate);

                var paramPayFdate = cmd.CreateParameter();
                paramPayFdate.ParameterName = "@payFdate";
                paramPayFdate.Value = model.payFdate;
                cmd.Parameters.Add(paramPayFdate);

                var paramPayCdate = cmd.CreateParameter();
                paramPayCdate.ParameterName = "@payCdate";
                paramPayCdate.Value = model.payCdate;
                cmd.Parameters.Add(paramPayCdate);

                var paramPayEdate = cmd.CreateParameter();
                paramPayEdate.ParameterName = "@payEdate";
                paramPayEdate.Value = model.payEdate;
                cmd.Parameters.Add(paramPayEdate);

                var paramAutopay = cmd.CreateParameter();
                paramAutopay.ParameterName = "@autopay";
                paramAutopay.Value = model.autopay;
                cmd.Parameters.Add(paramAutopay);

                var paramSystemStatus = cmd.CreateParameter();
                paramSystemStatus.ParameterName = "@systemStatus";
                paramSystemStatus.Value = model.SystemStatus;
                cmd.Parameters.Add(paramSystemStatus);

                var paramSystemMsg = cmd.CreateParameter();
                paramSystemMsg.ParameterName = "@systemMsg";
                paramSystemMsg.Value = model.SystemMsg;
                cmd.Parameters.Add(paramSystemMsg);

                cmd.ExecuteNonQuery();
            }
        }
    }

    #region ViewModel
    public class UserOrderRecordViewModel
    {
        public string userid { get; set; }
        public string opseq { get; set; }
        public string opid { get; set; }
        public string orderNo { get; set; }
        public Nullable<double> amount { get; set; }
        public Nullable<double> MSRP { get; set; }
        public System.DateTime? payFdate { get; set; }
        public System.DateTime? paySdate { get; set; }
        public Nullable<System.DateTime> payCdate { get; set; }
        public System.DateTime? payEdate { get; set; }
        public string autopay { get; set; }
        public string SEQNO { get; set; }
        public string SystemMsg { get; set; }
        public string SystemStatus { get; set; }
        public System.DateTime lastUpdate { get; set; }
    }

    #endregion
}