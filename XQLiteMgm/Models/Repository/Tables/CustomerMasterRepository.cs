using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using XQLiteMgm.Models.Interface;
using XQLiteMgm.Models.UnitOfWork;
using XQLiteMgm.Models.UnitOfWork.Interface;

namespace XQLiteMgm.Models.Repository
{
    public class CustomerMasterRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public CustomerMasterRepository(IUnitOfWork uow)
        {
            if (uow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = uow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory  is null");
            }
        }

        /// <summary>
        /// 新增 CustomerMaster
        /// </summary>
        /// <param name="model"></param>
        public void CustomerMasterInsert(CustomerMasterViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"CustomerMasterInsert";

                cmd.Parameters.Clear();

                var paramCustno = cmd.CreateParameter();
                paramCustno.ParameterName = "@custno";
                paramCustno.Value = model.custno;
                cmd.Parameters.Add(paramCustno);

                var paramName = cmd.CreateParameter();
                paramName.ParameterName = "@name";
                paramName.Value = model.name;
                cmd.Parameters.Add(paramName);

                var paramInvoiceTitle = cmd.CreateParameter();
                paramInvoiceTitle.ParameterName = "@invoiceTitle";
                paramInvoiceTitle.Value = model.invoiceTitle;
                cmd.Parameters.Add(paramInvoiceTitle);

                var paramInvoiceAddress = cmd.CreateParameter();
                paramInvoiceAddress.ParameterName = "@invoiceAddress";
                paramInvoiceAddress.Value = model.invoiceAddress;
                cmd.Parameters.Add(paramInvoiceAddress);

                var paramSsn = cmd.CreateParameter();
                paramSsn.ParameterName = "@ssn";
                paramSsn.Value = model.ssn;
                cmd.Parameters.Add(paramSsn);

                var paramPresident = cmd.CreateParameter();
                paramPresident.ParameterName = "@president";
                paramPresident.Value = model.president;
                cmd.Parameters.Add(paramPresident);

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

                var paramEmail = cmd.CreateParameter();
                paramEmail.ParameterName = "@email";
                paramEmail.Value = model.email;
                cmd.Parameters.Add(paramEmail);

                var paramRemark = cmd.CreateParameter();
                paramRemark.ParameterName = "@remark";
                paramRemark.Value = model.remark;
                cmd.Parameters.Add(paramRemark);

                var paramCratedate = cmd.CreateParameter();
                paramCratedate.ParameterName = "@cratedate";
                paramCratedate.Value = model.cratedate;
                cmd.Parameters.Add(paramCratedate);

                var paramOpseq = cmd.CreateParameter();
                paramOpseq.ParameterName = "@opseq";
                paramOpseq.Value = model.opseq;
                cmd.Parameters.Add(paramOpseq);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// select CustomerMaster by Name
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public CustomerMasterViewModel CustomerMasterSelectName(string userid)
        {
            CustomerMasterViewModel custMaster = null;

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"CustomerMasterSelectName";


                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@name";
                paramText.Value = userid;
                cmd.Parameters.Add(paramText);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        custMaster = new CustomerMasterViewModel();
                        custMaster.custno = reader["custno"].ToString();
                        custMaster.name = reader["name"].ToString();
                        custMaster.invoiceTitle = reader["invoiceTitle"].ToString();
                        custMaster.invoiceAddress = reader["invoiceAddress"].ToString();
                        custMaster.ssn = reader["ssn"].ToString();
                        custMaster.president = reader["president"].ToString();
                        custMaster.contact = reader["contact"].ToString();
                        custMaster.tel = reader["tel"].ToString();
                        custMaster.fax = reader["fax"].ToString();
                        custMaster.mobile = reader["mobile"].ToString();
                        custMaster.email = reader["email"].ToString();
                        custMaster.remark = reader["remark"].ToString();
                        custMaster.cratedate = reader["cratedate"] == DBNull.Value ? null : (DateTime?)reader["cratedate"];
                        custMaster.opseq = Convert.ToInt16(reader["opseq"].ToString());
                    }
                }
            }

            return custMaster;
        }


        /// <summary>
        /// select CustomerMaster by custno
        /// </summary>
        /// <param name="custno"></param>
        /// <returns></returns>
        public CustomerMasterViewModel CustomerMasterSelect(string custno)
        {
            CustomerMasterViewModel custMaster = null;

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"CustomerMasterSelect";


                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@custno";
                paramText.Value = custno;
                cmd.Parameters.Add(paramText);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        custMaster = new CustomerMasterViewModel();
                        custMaster.custno = reader["custno"].ToString();
                        custMaster.name = reader["name"].ToString();
                        custMaster.invoiceTitle = reader["invoiceTitle"].ToString();
                        custMaster.invoiceAddress = reader["invoiceAddress"].ToString();
                        custMaster.ssn = reader["ssn"].ToString();
                        custMaster.president = reader["president"].ToString();
                        custMaster.contact = reader["contact"].ToString();
                        custMaster.tel = reader["tel"].ToString();
                        custMaster.fax = reader["fax"].ToString();
                        custMaster.mobile = reader["mobile"].ToString();
                        custMaster.email = reader["email"].ToString();
                        custMaster.remark = reader["remark"].ToString();
                        custMaster.cratedate = reader["cratedate"] == DBNull.Value ? null : (DateTime?)reader["cratedate"];
                        custMaster.opseq = (int)reader["opseq"];
                    }
                }
            }

            return custMaster;
        }

        /// <summary>
        /// select CustomerMaster by mobile
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public CustomerMasterViewModel CustomerMasterSelectMobile(string mobile)
        {
            CustomerMasterViewModel custMaster = null;

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"CustomerMasterSelectMobile";


                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@mobile";
                paramText.Value = mobile;
                cmd.Parameters.Add(paramText);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        custMaster = new CustomerMasterViewModel();
                        custMaster.custno = reader["custno"].ToString();
                        custMaster.name = reader["name"].ToString();
                        custMaster.mobile = reader["mobile"].ToString();
                        custMaster.email = reader["email"].ToString();
                    }
                }
            }

            return custMaster;
        }

        /// <summary>
        /// select CustomerMaster by Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public CustomerMasterViewModel CustomerMasterSelectEmail(string email)
        {
            CustomerMasterViewModel custMaster = null;

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"CustomerMasterSelectEmail";


                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@email";
                paramText.Value = email;
                cmd.Parameters.Add(paramText);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        custMaster = new CustomerMasterViewModel();
                        custMaster.custno = reader["custno"].ToString();
                        custMaster.name = reader["name"].ToString();
                        custMaster.invoiceTitle = reader["invoiceTitle"].ToString();
                        custMaster.invoiceAddress = reader["invoiceAddress"].ToString();
                        custMaster.ssn = reader["ssn"].ToString();
                        custMaster.president = reader["president"].ToString();
                        custMaster.contact = reader["contact"].ToString();
                        custMaster.tel = reader["tel"].ToString();
                        custMaster.fax = reader["fax"].ToString();
                        custMaster.mobile = reader["mobile"].ToString();
                        custMaster.email = reader["email"].ToString();
                        custMaster.remark = reader["remark"].ToString();
                        custMaster.cratedate = reader["cratedate"] == DBNull.Value ? null : (DateTime?)reader["cratedate"];
                        custMaster.opseq = Convert.ToInt16( reader["opseq"].ToString());
                    }
                }
            }

            return custMaster;
        }

        public string GetXQLiteMaxCustno()
        {
            DataSet ds = new DataSet();
            int myCustno = 1;

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"CustomerMasterSelectCustno";


                var paramText = cmd.CreateParameter();
                paramText.ParameterName = "@group";
                paramText.Value = "xqlite-";
                cmd.Parameters.Add(paramText);

                ds.Load(cmd.ExecuteReader(), LoadOption.OverwriteChanges, "");
            }

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                int maxnum = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    int thisnum = int.Parse(ds.Tables[0].Rows[i].ItemArray[0].ToString().Replace("XQLITE-", ""));
                    if (thisnum > maxnum)
                    {
                        maxnum = thisnum;
                    }
                }
                myCustno = maxnum + 1;
            }
            string custno = "XQLITE-" + myCustno;

            return custno;
        }

        public bool CustomerMasterUpdateByName(string userid, string mobile, string email)
        {
            try
            {
                using (var cmd = _unitOfWork.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = @"CustomerMasterUpdateByName";
                    cmd.Parameters.Clear();

                    var paramUserid = cmd.CreateParameter();
                    paramUserid.ParameterName = "@name";
                    paramUserid.Value = userid;
                    cmd.Parameters.Add(paramUserid);

                    var paramMobile = cmd.CreateParameter();
                    paramMobile.ParameterName = "@mobile";
                    paramMobile.Value = mobile;
                    cmd.Parameters.Add(paramMobile);

                    var paramEmail = cmd.CreateParameter();
                    paramEmail.ParameterName = "@email";
                    paramEmail.Value = email;
                    cmd.Parameters.Add(paramEmail);

                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
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

    #region ViewModel
    public class CustomerMasterViewModel
    {
        public string custno { get; set; }
        public string name { get; set; }
        public string invoiceTitle { get; set; }
        public string invoiceAddress { get; set; }
        public string ssn { get; set; }
        public string president { get; set; }
        public string contact { get; set; }
        public string tel { get; set; }
        public string fax { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string remark { get; set; }
        public Nullable<System.DateTime> cratedate { get; set; }
        public int opseq { get; set; }
        
    }

    #endregion
}