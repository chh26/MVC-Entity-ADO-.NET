using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using XQLiteMgm.Models.UnitOfWork.Interface;

namespace XQLiteMgm.Models.UnitOfWork
{
    public class UnitOfWorkFactory
    {
        public static IUnitOfWork Create(string connString = null)
        {
            if (string.IsNullOrEmpty(connString))
                connString = ConfigurationManager.ConnectionStrings["DSN_DASDB"].ConnectionString;

            var connection = new SqlConnection(connString);

            connection.Open();

            return new SQLUnitOfWork(connection, true);
        }
    }
}