using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using XQLiteMgm.Models.Interface;
using XQLiteMgm.Models.UnitOfWork.Interface;

namespace XQLiteMgm.Models.UnitOfWork
{
    public class SQLUnitOfWork : IUnitOfWork
    {
        public bool _hasConnection { get; set; }

        public IDbTransaction _transaction { get; set; }

        public IDbConnection _connection { get; set; }

        public SQLUnitOfWork(IDbConnection connection, bool hasConnection)
        {
            _connection = connection;
            _hasConnection = hasConnection;
            _transaction = connection.BeginTransaction();
        }

        public IDbCommand CreateCommand()
        {
            var command = _connection.CreateCommand();
            command.Transaction = _transaction;
            return command;
        }

        public void SaveChanges()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction have already been already been commited. Check your transaction handling.");
            }

            _transaction.Commit();
            _transaction = null;
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction = null;
            }

            if (_connection != null && _hasConnection)
            {
                _connection.Close();
                _connection = null;
            }
        }
    }
}