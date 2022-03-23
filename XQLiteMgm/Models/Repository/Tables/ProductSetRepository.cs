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
    public class ProductSetRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public ProductSetRepository(IUnitOfWork uow)
        {
            if (uow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = uow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory  is null.");
            }
        }

        public List<ProductSetViewModel> GetProductSetList()
        {
            List<ProductSetViewModel> list = new List<ProductSetViewModel>();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"
                                    SELECT
                                            PID
                                            ,PNAME
                                            ,Discription
                                            ,MSRP
                                            ,Bargain_Price
                                            ,StartDate
                                            ,EndDate
                                            ,Permissions
                                            ,Status
                                            ,Beta_Permissins
                                            ,ECPID
                                            ,TryOut
                                            ,PKind
                                            ,SupportVer
                                            ,[Order]
                                    FROM
                                            ProductSet
                                    WHERE     
                                            (ECPID IS NOT NULL) 
                                    AND 
                                            [Status] <>'offline'
                                    ORDER BY [ORDER] ASC
                                ";

                cmd.Parameters.Clear();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ProductSetViewModel viewModel = new ProductSetViewModel();

                        viewModel.PID = reader["PID"].ToString();
                        viewModel.PNAME = reader["PID"].ToString() == "TW300" ? reader["PNAME"].ToString() + "(300)" : reader["PNAME"].ToString();
                        viewModel.Discription = reader["Discription"].ToString();
                        viewModel.MSRP = Math.Round(Convert.ToDecimal(reader["MSRP"].ToString()), 2, MidpointRounding.AwayFromZero);
                        viewModel.Bargain_Price = Math.Round(Convert.ToDecimal(reader["Bargain_Price"].ToString()), 2, MidpointRounding.AwayFromZero);

                        if (reader["StartDate"] != DBNull.Value)
                        {
                            if (!string.IsNullOrEmpty(reader["StartDate"].ToString()))
                            {
                                viewModel.StartDate = (DateTime?)Convert.ToDateTime(reader["StartDate"].ToString());
                            }
                        }

                        if (reader["EndDate"] != DBNull.Value)
                        {
                            if (!string.IsNullOrEmpty(reader["EndDate"].ToString()))
                            {
                                viewModel.EndDate = (DateTime?)Convert.ToDateTime(reader["EndDate"].ToString());
                            }
                        }
                        
                        viewModel.Permissions = reader["Permissions"].ToString();
                        viewModel.Status = reader["Status"].ToString();
                        viewModel.Beta_Permissins = reader["Beta_Permissins"].ToString();
                        viewModel.ECPID = reader["ECPID"].ToString();
                        viewModel.TryOut = reader["TryOut"].ToString();
                        viewModel.PKind = reader["PKind"].ToString();
                        viewModel.SupportVer = reader["SupportVer"].ToString();

                        if (reader["Order"] != DBNull.Value)
                        {
                            if (!string.IsNullOrEmpty(reader["Order"].ToString()))
                            {
                                viewModel.Order = Convert.ToInt16(reader["Order"].ToString());
                            }
                        }

                        list.Add(viewModel);
                    }
                }
            }

            return list;
        }


        public List<ProductSetViewModel> ProductSet_SEL_PID(ProductSetViewModel model)
        {
            List<ProductSetViewModel> list = new List<ProductSetViewModel>();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = @"ProductSet_SEL_PID";

                var paramPID = cmd.CreateParameter();
                paramPID.ParameterName = "@PID";
                paramPID.Value = model.PID;
                cmd.Parameters.Add(paramPID);


                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ProductSetViewModel viewModel = new ProductSetViewModel();

                        viewModel.PID = reader["PID"].ToString();
                        viewModel.PNAME = reader["PNAME"].ToString();
                        viewModel.Discription = reader["Discription"].ToString();
                        viewModel.MSRP = Math.Round(Convert.ToDecimal(reader["MSRP"].ToString()), 2, MidpointRounding.AwayFromZero);
                        viewModel.Bargain_Price = Math.Round(Convert.ToDecimal(reader["Bargain_Price"].ToString()), 2, MidpointRounding.AwayFromZero);

                        string aaa;
                        DateTime? aaa1;
                        if (reader["StartDate"] != DBNull.Value && !string.IsNullOrEmpty(reader["StartDate"].ToString()))
                        {
                            aaa = reader["StartDate"].ToString();

                            aaa1 = (DateTime?)Convert.ToDateTime(reader["StartDate"].ToString());

                        }

                        string bbb;
                        DateTime? bbb1;
                        if (reader["EndDate"] != DBNull.Value && !string.IsNullOrEmpty(reader["EndDate"].ToString()))
                        {

                            bbb = reader["EndDate"].ToString();

                            bbb1 = (DateTime?)Convert.ToDateTime(reader["EndDate"].ToString());
                        }

                        viewModel.StartDate = (reader["StartDate"] == DBNull.Value && string.IsNullOrEmpty(reader["StartDate"].ToString())) ? null : (DateTime?)Convert.ToDateTime(reader["StartDate"].ToString());
                        viewModel.EndDate = (reader["EndDate"] != DBNull.Value && !string.IsNullOrEmpty(reader["EndDate"].ToString())) ? (DateTime?)Convert.ToDateTime(reader["EndDate"].ToString()) : null;
                        viewModel.Permissions = reader["Permissions"].ToString();
                        viewModel.Status = reader["Status"].ToString();
                        viewModel.Beta_Permissins = reader["Beta_Permissins"].ToString();
                        viewModel.ECPID = reader["ECPID"].ToString();
                        viewModel.TryOut = reader["TryOut"].ToString();
                        viewModel.PKind = reader["PKind"].ToString();
                        viewModel.SupportVer = reader["SupportVer"].ToString();
                        viewModel.Order = Convert.ToInt16(reader["Order"].ToString());

                        list.Add(viewModel);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 查詢項目權限
        /// <para>參數: 項目ID</para>
        /// </summary>
        /// <param name="opid"></param>
        /// <returns></returns>
        public string[] serach_Permission(ProductSetViewModel model)
        {
            List<ProductSetViewModel> list = ProductSet_SEL_PID(model);
            string[] UserExtraAuthList = null;

            if (list.Count > 0)
            {
                ProductSetViewModel productSetModel = new ProductSetViewModel();
                productSetModel = list[0];

                string tmp = productSetModel.Permissions;
                string PKind = productSetModel.PKind;
                if (PKind.ToUpper() == "EXTRA")
                {
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        UserExtraAuthList = tmp.Split(';');
                    }
                }
            }
            return UserExtraAuthList;
        }

        /// <summary>
        /// 查詢行情模組權限
        /// <para>參數: 項目ID</para>
        /// </summary>
        /// <param name="opid"></param>
        /// <returns></returns>
        public string[] serach_exch_Permission(ProductSetViewModel model)
        {
            List<ProductSetViewModel> list = ProductSet_SEL_PID(model);
            string[] UserExtraAuthList = null;

            if (list.Count > 0)
            {
                ProductSetViewModel productSetModel = new ProductSetViewModel();
                productSetModel = list[0];

                string tmp = productSetModel.Permissions;
                string PKind = productSetModel.PKind;
                if (PKind.ToUpper() == "EXCH")
                {
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        UserExtraAuthList = tmp.Split('|');
                    }
                }

            }
            return UserExtraAuthList;
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

    public class ProductSetViewModel {
        public string PID { get; set; }
        public string PNAME { get; set; }
        public string Discription { get; set; }
        public decimal MSRP { get; set; }
        public decimal Bargain_Price { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Permissions { get; set; }
        public string Status { get; set; }
        public string Beta_Permissins { get; set; }
        public string ECPID { get; set; }
        public string TryOut { get; set; }
        public string PKind { get; set; }
        public string SupportVer { get; set; }
        public int? Order { get; set; }
        /// <summary>
        /// for 前端ui顯示用
        /// </summary>
        public bool disabled { get; set; }
    }
}