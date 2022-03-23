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
    public class CustomerDetailRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public CustomerDetailRepository(IUnitOfWork uow)
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
        /// 新增 CustomerDetail
        /// </summary>
        /// <param name="model"></param>
        public void CustomerDetailInsert(CustomerDetailViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"CustomerDetailInsert";

                cmd.Parameters.Clear();

                var paramCustno = cmd.CreateParameter();
                paramCustno.ParameterName = "@custno";
                paramCustno.Value = model.custno;
                cmd.Parameters.Add(paramCustno);

                var paramName = cmd.CreateParameter();
                paramName.ParameterName = "@name";
                paramName.Value = model.name;
                cmd.Parameters.Add(paramName);

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                paramUserid.Value = model.userid;
                cmd.Parameters.Add(paramUserid);

                var paramGroupid = cmd.CreateParameter();
                paramGroupid.ParameterName = "@groupid";
                paramGroupid.Value = model.groupid;
                cmd.Parameters.Add(paramGroupid);

                var paramAcctype = cmd.CreateParameter();
                paramAcctype.ParameterName = "@acctype";
                paramAcctype.Value = model.acctype;
                cmd.Parameters.Add(paramAcctype);

                var paramHostaddress = cmd.CreateParameter();
                paramHostaddress.ParameterName = "@hostaddress";
                paramHostaddress.Value = model.hostaddress;
                cmd.Parameters.Add(paramHostaddress);

                var paramContact = cmd.CreateParameter();
                paramContact.ParameterName = "@contact";
                paramContact.Value = model.contact;
                cmd.Parameters.Add(paramContact);

                var paramTel = cmd.CreateParameter();
                paramTel.ParameterName = "@tel";
                paramTel.Value = model.tel;
                cmd.Parameters.Add(paramTel);

                var paramFax = cmd.CreateParameter();
                paramFax.ParameterName = "@fax";
                paramFax.Value = model.fax;
                cmd.Parameters.Add(paramFax);

                var paramMobile = cmd.CreateParameter();
                paramMobile.ParameterName = "@mobile";
                paramMobile.Value = model.mobile;
                cmd.Parameters.Add(paramMobile);

                var paramConnectype = cmd.CreateParameter();
                paramConnectype.ParameterName = "@connectype";
                paramConnectype.Value = model.connectype;
                cmd.Parameters.Add(paramConnectype);

                var paramLeasedlineno = cmd.CreateParameter();
                paramLeasedlineno.ParameterName = "@leasedlineno";
                paramLeasedlineno.Value = model.leasedlineno;
                cmd.Parameters.Add(paramLeasedlineno);

                var paramPchardware = cmd.CreateParameter();
                paramPchardware.ParameterName = "@pchardware";
                paramPchardware.Value = model.pchardware;
                cmd.Parameters.Add(paramPchardware);

                var paramSlseq = cmd.CreateParameter();
                paramSlseq.ParameterName = "@slseq";
                paramSlseq.Value = model.slseq;
                cmd.Parameters.Add(paramSlseq);

                var paramLastupdate = cmd.CreateParameter();
                paramLastupdate.ParameterName = "@lastupdate";
                paramLastupdate.Value = model.lastupdate;
                cmd.Parameters.Add(paramLastupdate);

                var paramProductype = cmd.CreateParameter();
                paramProductype.ParameterName = "@productype";
                paramProductype.Value = model.productype;
                cmd.Parameters.Add(paramProductype);

                var paramBilltype = cmd.CreateParameter();
                paramBilltype.ParameterName = "@billtype";
                paramBilltype.Value = model.billtype;
                cmd.Parameters.Add(paramBilltype);

                var paramPaytype = cmd.CreateParameter();
                paramPaytype.ParameterName = "@paytype";
                paramPaytype.Value = model.paytype;
                cmd.Parameters.Add(paramPaytype);

                var paramInitialdate = cmd.CreateParameter();
                paramInitialdate.ParameterName = "@initialdate";
                paramInitialdate.Value = model.initialdate;
                cmd.Parameters.Add(paramInitialdate);

                var paramBillsdate = cmd.CreateParameter();
                paramBillsdate.ParameterName = "@billsdate";
                paramBillsdate.Value = model.billsdate;
                cmd.Parameters.Add(paramBillsdate);

                var paramValidSDate = cmd.CreateParameter();
                paramValidSDate.ParameterName = "@Validsdate";
                paramValidSDate.Value = model.ValidSDate;
                cmd.Parameters.Add(paramValidSDate);

                var paramValidEDate = cmd.CreateParameter();
                paramValidEDate.ParameterName = "@Validedate";
                paramValidEDate.Value = model.ValidEDate;
                cmd.Parameters.Add(paramValidEDate);

                var paramMoney = cmd.CreateParameter();
                paramMoney.ParameterName = "@money";
                paramMoney.Value = model.money;
                cmd.Parameters.Add(paramMoney);

                var paramMonitor = cmd.CreateParameter();
                paramMonitor.ParameterName = "@Monitor";
                paramMonitor.Value = model.Monitor;
                cmd.Parameters.Add(paramMonitor);

                cmd.ExecuteNonQuery();
            }
        }

        public List<CustomerDetailViewModel> GetUserIdList(string userid)
        {
            List<CustomerDetailViewModel> list = new List<CustomerDetailViewModel>();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"
                                SELECT 
                                        userid
                                From
                                        CustomerDetail
                                WHERE
                                        userid LIKE '%' + @userid +'%'
                                ";

                cmd.Parameters.Clear();

                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                if (string.IsNullOrEmpty(userid))
                    paramUserid.Value = DBNull.Value;
                else
                    paramUserid.Value = userid;
                cmd.Parameters.Add(paramUserid);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CustomerDetailViewModel viewModel = new CustomerDetailViewModel();

                        viewModel.userid = reader["userid"].ToString();

                        list.Add(viewModel);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// select CustomerMaster by custno  userid
        /// </summary>
        /// <param name="email"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public CustomerDetailViewModel CustomerDetailSelect(string custno, string userid)
        {
            CustomerDetailViewModel custDetail = null;

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"CustomerDetailSelect";


                var paramCustno = cmd.CreateParameter();
                paramCustno.ParameterName = "@custno";
                paramCustno.Value = custno;
                cmd.Parameters.Add(paramCustno);

                var paramGroupid = cmd.CreateParameter();
                paramGroupid.ParameterName = "@userid";
                paramGroupid.Value = userid;
                cmd.Parameters.Add(paramGroupid);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        custDetail = new CustomerDetailViewModel();
                        custDetail.custno = reader["custno"].ToString();
                        custDetail.name = reader["name"].ToString();
                        custDetail.userid = reader["userid"].ToString();
                        custDetail.groupid = reader["groupid"].ToString();
                        custDetail.acctype = reader["acctype"].ToString();
                        custDetail.hostaddress = reader["hostaddress"].ToString();
                        custDetail.contact = reader["contact"].ToString();
                        custDetail.tel = reader["tel"].ToString();
                        custDetail.fax = reader["fax"].ToString();
                        custDetail.mobile = reader["mobile"].ToString();
                        custDetail.connectype = reader["connectype"].ToString();
                        custDetail.leasedlineno = reader["leasedlineno"].ToString();

                        custDetail.pchardware = reader["pchardware"].ToString();
                        custDetail.slseq = (int)reader["slseq"];
                        custDetail.mobile = reader["mobile"].ToString();
                        custDetail.connectype = reader["connectype"].ToString();
                        custDetail.leasedlineno = reader["leasedlineno"].ToString();

                        custDetail.lastupdate = reader["lastupdate"] == DBNull.Value ? null : (DateTime?)reader["lastupdate"];

                        custDetail.productype = reader["productype"].ToString();
                        custDetail.billtype = reader["billtype"].ToString();
                        custDetail.paytype = reader["paytype"].ToString();

                        custDetail.initialdate = reader["initialdate"] == DBNull.Value ? null : (DateTime?)reader["initialdate"];
                        custDetail.billsdate = reader["billsdate"] == DBNull.Value ? null : (DateTime?)reader["billsdate"];
                    }
                }
            }

            return custDetail;
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

    public class CustomerDetailViewModel {
        public string custno { get; set; }
        public string name { get; set; }
        public string userid { get; set; }
        public string groupid { get; set; }
        public string acctype { get; set; }
        public string hostaddress { get; set; }
        public string contact { get; set; }
        public string tel { get; set; }
        public string fax { get; set; }
        public string mobile { get; set; }
        public string connectype { get; set; }
        public string leasedlineno { get; set; }
        public string pchardware { get; set; }
        public int slseq { get; set; }
        public Nullable<System.DateTime> lastupdate { get; set; }
        public string productype { get; set; }
        public string billtype { get; set; }
        public string paytype { get; set; }
        public Nullable<System.DateTime> initialdate { get; set; }
        public Nullable<System.DateTime> billsdate { get; set; }
        public Nullable<System.DateTime> ValidSDate { get; set; }
        public Nullable<System.DateTime> ValidEDate { get; set; }
        public int money { get; set; }
        public DateTime NextValidSDate { get; set; }
        public DateTime NextValidEDate { get; set; }
        public int Monitor { get; set; }




    }
}