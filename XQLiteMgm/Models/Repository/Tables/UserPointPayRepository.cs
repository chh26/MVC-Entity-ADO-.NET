using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using XQLiteMgm.Models.UnitOfWork;
using XQLiteMgm.Models.UnitOfWork.Interface;

namespace XQLiteMgm.Models.Repository.Tables
{
    public class UserPointPayRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public UserPointPayRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        public void UserPointPay_UPD_STATUS(UserPointPayViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserPointPay_UPD_STATUS";
                cmd.Parameters.Clear();

                var paramCardSEQNO = cmd.CreateParameter();
                paramCardSEQNO.ParameterName = "@cardSEQNO";
                paramCardSEQNO.Value = model.cardSEQNO;
                cmd.Parameters.Add(paramCardSEQNO);

                var paramStatus = cmd.CreateParameter();
                paramStatus.ParameterName = "@Status";
                paramStatus.Value = model.Status;
                cmd.Parameters.Add(paramStatus);

                cmd.ExecuteNonQuery();

            }
        }

        public void UserPointPay_INS(UserPointPayViewModel model)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserPointPay_INS";
                cmd.Parameters.Clear();

                var paramUserID = cmd.CreateParameter();
                paramUserID.ParameterName = "@userid";
                paramUserID.Value = model.UserID;
                cmd.Parameters.Add(paramUserID);

                var paramCardSEQNO = cmd.CreateParameter();
                paramCardSEQNO.ParameterName = "@cardSEQNO";
                paramCardSEQNO.Value = model.cardSEQNO;
                cmd.Parameters.Add(paramCardSEQNO);

                var paramPoint = cmd.CreateParameter();
                paramPoint.ParameterName = "@point";
                paramPoint.Value = model.Point;
                cmd.Parameters.Add(paramPoint);

                var paramPayTime = cmd.CreateParameter();
                paramPayTime.ParameterName = "@paytime";
                paramPayTime.Value = model.PayTime;
                cmd.Parameters.Add(paramPayTime);

                var paramStatus = cmd.CreateParameter();
                paramStatus.ParameterName = "@Status";
                paramStatus.Value = model.Status;
                cmd.Parameters.Add(paramStatus);

                var paramEndDate = cmd.CreateParameter();
                paramEndDate.ParameterName = "@enddate";
                paramEndDate.Value = model.EndDate;
                cmd.Parameters.Add(paramEndDate);

                cmd.ExecuteNonQuery();
            }
        }
    }

    public class UserPointPayViewModel
    {
        public string cardSEQNO { get; set; }
        public string UserID { get; set; }
        public int Point { get; set; }
        public DateTime PayTime { get; set; }
        public string Status { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateTime { get; set; }
        public string StatusMemo { get; set; }
        public string Company { get; set; }
        public string CompanyName { get; set; }
    }
}