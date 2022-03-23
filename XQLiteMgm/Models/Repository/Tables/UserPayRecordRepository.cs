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
    public class UserPayRecordRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public UserPayRecordRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        public void UserPayRecord_UPD(UserPayRecordViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserPayRecord_UPD";
                cmd.Parameters.Clear();

                var paramSEQNO = cmd.CreateParameter();
                paramSEQNO.ParameterName = "@SEQNO";
                paramSEQNO.Value = model.SEQNO;
                cmd.Parameters.Add(paramSEQNO);

                var paramAutopay = cmd.CreateParameter();
                paramAutopay.ParameterName = "@autopay";
                paramAutopay.Value = model.autopay;
                cmd.Parameters.Add(paramAutopay);

                var paramPayEdate = cmd.CreateParameter();
                paramPayEdate.ParameterName = "@payEdate";
                paramPayEdate.Value = model.payEdate;
                cmd.Parameters.Add(paramPayEdate);

                var paramOthers = cmd.CreateParameter();
                paramOthers.ParameterName = "@others";
                paramOthers.Value = model.others;
                cmd.Parameters.Add(paramOthers);

                cmd.ExecuteNonQuery();

            }
        }

        public void UserPayRecord_INS(UserPayRecordViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserPayRecord_INS";
                cmd.Parameters.Clear();

                var paramSEQNO = cmd.CreateParameter();
                paramSEQNO.ParameterName = "@SEQNO";
                paramSEQNO.Value = model.SEQNO;
                cmd.Parameters.Add(paramSEQNO);

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = model.userid;
                cmd.Parameters.Add(paramUserid);

                var paramCardno = cmd.CreateParameter();
                paramCardno.ParameterName = "@cardno";
                paramCardno.Value = model.cardno;
                cmd.Parameters.Add(paramCardno);

                var paramValidateend = cmd.CreateParameter();
                paramValidateend.ParameterName = "@validateend";
                paramValidateend.Value = model.validateend;
                cmd.Parameters.Add(paramValidateend);

                var paramName = cmd.CreateParameter();
                paramName.ParameterName = "@name";
                paramName.Value = model.name;
                cmd.Parameters.Add(paramName);

                var paramUniForm = cmd.CreateParameter();
                paramUniForm.ParameterName = "@UniForm";
                paramUniForm.Value = model.UniForm;
                cmd.Parameters.Add(paramUniForm);

                var paramUniNum = cmd.CreateParameter();
                paramUniNum.ParameterName = "@UniNum";
                paramUniNum.Value = model.UniNum;
                cmd.Parameters.Add(paramUniNum);

                var paramAddress = cmd.CreateParameter();
                paramAddress.ParameterName = "@address";
                paramAddress.Value = model.address;
                cmd.Parameters.Add(paramAddress);

                var paramZipcode = cmd.CreateParameter();
                paramZipcode.ParameterName = "@zipcode";
                paramZipcode.Value = model.zipcode;
                cmd.Parameters.Add(paramZipcode);

                var paramOpseq = cmd.CreateParameter();
                paramOpseq.ParameterName = "@opseq";
                paramOpseq.Value = model.opseq;
                cmd.Parameters.Add(paramOpseq);

                var paramAmount = cmd.CreateParameter();
                paramAmount.ParameterName = "@amount";
                paramAmount.Value = model.amount;
                cmd.Parameters.Add(paramAmount);

                var paramMSRP = cmd.CreateParameter();
                paramMSRP.ParameterName = "@MSRP";
                paramMSRP.Value = model.MSRP;
                cmd.Parameters.Add(paramMSRP);

                var paramAutopay = cmd.CreateParameter();
                paramAutopay.ParameterName = "@autopay";
                paramAutopay.Value = model.autopay;
                cmd.Parameters.Add(paramAutopay);

                var paramPaySdate = cmd.CreateParameter();
                paramPaySdate.ParameterName = "@paySdate";
                paramPaySdate.Value = model.paySdate;
                cmd.Parameters.Add(paramPaySdate);

                var paramPayEdate = cmd.CreateParameter();
                paramPayEdate.ParameterName = "@payEdate";
                paramPayEdate.Value = model.payEdate;
                cmd.Parameters.Add(paramPayEdate);

                var paramIpaddress = cmd.CreateParameter();
                paramIpaddress.ParameterName = "@ipaddress";
                paramIpaddress.Value = model.ipaddress;
                cmd.Parameters.Add(paramIpaddress);

                var paramCardSEQNO = cmd.CreateParameter();
                paramCardSEQNO.ParameterName = "@cardSEQNO";
                paramCardSEQNO.Value = model.cardSEQNO;
                cmd.Parameters.Add(paramCardSEQNO);

                var paramOpid = cmd.CreateParameter();
                paramOpid.ParameterName = "@opid";
                paramOpid.Value = model.opid;
                cmd.Parameters.Add(paramOpid);

                var paramOthers = cmd.CreateParameter();
                paramOthers.ParameterName = "@others";
                paramOthers.Value = model.others;
                cmd.Parameters.Add(paramOthers);

                var paramReceiptSEQNO = cmd.CreateParameter();
                paramReceiptSEQNO.ParameterName = "@ReceiptSEQNO";
                paramReceiptSEQNO.Value = model.ReceiptSEQNO;
                cmd.Parameters.Add(paramReceiptSEQNO);

                cmd.ExecuteNonQuery();

            }
        }

        public void UserPayRecord_UPDAddress(UserPayRecordViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                            UPDATE  [UserPayRecord]
                            SET
                                zipcode = @zipcode
                                ,address = @address
                            WHERE
                                [SEQNO] = @SEQNO
                            ";
                cmd.Parameters.Clear();

                var paramZipcode = cmd.CreateParameter();
                paramZipcode.ParameterName = "@zipcode";
                paramZipcode.Value = model.zipcode;
                cmd.Parameters.Add(paramZipcode);

                var paramAddress = cmd.CreateParameter();
                paramAddress.ParameterName = "@address";
                paramAddress.Value = model.address;
                cmd.Parameters.Add(paramAddress);

                var paramSEQNO = cmd.CreateParameter();
                paramSEQNO.ParameterName = "@SEQNO";
                paramSEQNO.Value = model.SEQNO;
                cmd.Parameters.Add(paramSEQNO);

                cmd.ExecuteNonQuery();

            }
        }

        /// <summary>
        /// 取得客戶付款明細
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="opid"></param>
        /// <returns></returns>
        internal List<UserPayRecordViewModel> GetUserPayRecord(string userid, string opid)
        {
            List<UserPayRecordViewModel> list = new List<UserPayRecordViewModel>();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                                SELECT 
	                                p.SEQNO
	                                ,p.userid
	                                ,p.name
	                                ,p.address
	                                ,p.opid
	                                ,p.opseq
	                                ,p.paySdate
	                                ,p.payEdate
	                                ,p.others
                                    ,p.cardSEQNO
	                                ,o.userid as OrderUserid
                                FROM UserPayRecord p
	                                LEFT JOIN UserOrderRecord  o ON p.SEQNO = o.SEQNO
                                WHERE 
	                                p.userid = @userid
                                AND
	                                p.opid = @opid   
                                ORDER BY p.createtime desc                         
                                ";
                cmd.Parameters.Clear();

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = userid;
                cmd.Parameters.Add(paramUserid);

                var paramOpid = cmd.CreateParameter();
                paramOpid.ParameterName = "@opid";
                paramOpid.Value = opid;
                cmd.Parameters.Add(paramOpid);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UserPayRecordViewModel userPayRecord = new UserPayRecordViewModel();

                        userPayRecord.SEQNO = reader["SEQNO"].ToString();
                        userPayRecord.userid = reader["userid"].ToString();
                        userPayRecord.name = reader["name"].ToString();
                        userPayRecord.address = reader["address"].ToString();
                        userPayRecord.opid = reader["opid"].ToString();
                        userPayRecord.opseq = reader["opseq"].ToString();
                        userPayRecord.cardSEQNO = reader["cardSEQNO"].ToString();
                        userPayRecord.paySdate = Convert.ToDateTime(reader["paySdate"].ToString());
                        userPayRecord.payEdate = Convert.ToDateTime(reader["payEdate"].ToString());
                        userPayRecord.others = reader["others"].ToString();
                        userPayRecord.OrderUserid = reader["OrderUserid"].ToString();

                        list.Add(userPayRecord);
                    }
                }
            }

            return list;
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

    public class UserPayRecordViewModel
    {
        public string SEQNO { get; set; }
        public string userid { get; set; }
        public string cardno { get; set; }
        public string validateend { get; set; }
        public string secure { get; set; }
        public string name { get; set; }
        public string UniForm { get; set; }
        public string UniNum { get; set; }
        public string county { get; set; }
        public string district { get; set; }
        public string zipcode { get; set; }
        public string address { get; set; }
        public string opid { get; set; }
        public string opseq { get; set; }
        public Nullable<double> amount { get; set; }
        public Nullable<double> MSRP { get; set; }
        public string autopay { get; set; }
        public System.DateTime? paySdate { get; set; }
        public System.DateTime? payEdate { get; set; }
        public string others { get; set; }
        public string ipaddress { get; set; }
        public string cardSEQNO { get; set; }
        public Nullable<System.Guid> ReceiptSEQNO { get; set; }
        public System.DateTime createtime { get; set; }
        public string recipient { get; set; }
        public string OrderUserid { get; set; }
    }
}