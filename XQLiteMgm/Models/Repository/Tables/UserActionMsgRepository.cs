using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using XQLiteMgm.Helper;
using XQLiteMgm.Models.Interface;
using XQLiteMgm.Models.UnitOfWork;
using XQLiteMgm.Models.UnitOfWork.Interface;

namespace XQLiteMgm.Models.Repository.Tables
{
    public class UserActionMsgRepository : IRepository
    {
        private SQLUnitOfWork _unitOfWork;

        public UserActionMsgRepository(IUnitOfWork iuow)
        {
            if (iuow == null)
                throw new ArgumentNullException("IUnitOfWork Factory is null.");

            _unitOfWork = iuow as SQLUnitOfWork;

            if (_unitOfWork == null)
            {
                throw new NotSupportedException("UnitOfWork Factory is null.");
            }
        }

        public bool UserActionMsg_INS(UserActionMsgViewModel model)
        {
            try
            {
                using (var cmd = _unitOfWork.CreateCommand())
                {
                    cmd.Parameters.Clear();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = @"UserActionMsg_INS";

                    var paramAppid = cmd.CreateParameter();
                    paramAppid.ParameterName = "@Appid";
                    paramAppid.Value = model.Appid;
                    cmd.Parameters.Add(paramAppid);

                    var paramUserid = cmd.CreateParameter();
                    paramUserid.ParameterName = "@userid";
                    paramUserid.Value = model.userid;
                    cmd.Parameters.Add(paramUserid);

                    var paramAction = cmd.CreateParameter();
                    paramAction.ParameterName = "@action";
                    paramAction.Value = model.Action;
                    cmd.Parameters.Add(paramAction);

                    var paramMsg = cmd.CreateParameter();
                    string userOptName = string.IsNullOrEmpty(UserHelper.GetUserAccount()) ? "admin_API" : UserHelper.GetUserAccount();

                    paramMsg.ParameterName = "@Msg";
                    paramMsg.Value = $"[{userOptName}]{model.Msg}";
                    cmd.Parameters.Add(paramMsg);

                    var paramIpAddress = cmd.CreateParameter();
                    paramIpAddress.ParameterName = "@IpAddress";
                    paramIpAddress.Value = AppHelper.GetIPAddress();
                    cmd.Parameters.Add(paramIpAddress);

                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<UserActionMsgViewModel> UserActoinMsg_SEL(UserActionMsgViewModel model)
        {
            /* action：NEW_ACCOUNT（查詢代號）!normal（查詢類型） */
            string actionCode = model.Action.Split('!')[0];
            string actionType = model.Action.Split('!')[1];

            List<UserActionMsgViewModel> list = new List<UserActionMsgViewModel>();

            using (var cmd = _unitOfWork.CreateCommand())
            {
                if (actionCode == "Model")
                {
                    string queryStr = @"
                                SELECT 
                                        userid,
                                        Action,
                                        Msg,
                                        CreateTime,
                                        CustomerMaster.email,
                                        Active
                                FROM UserActionMsg
                                Left join CustomerMaster on UserActionMsg.userid = CustomerMaster.name
                                Left join EmailCheck on EmailCheck.email = CustomerMaster.email
                                WHERE 
                                    (action like '%BUY%' or action like '%CANCELSUB%' or action like '%RESUB%' or action like '%REBUY%' or action like '%PAYERROR%') 
                                AND 
                                    action <> 'BUY_MAIL' 
                                AND 
                                    userid LIKE '%' + @userid +'%'
                                AND 
                                    CreateTime BETWEEN @StartDate AND @EndDate
                                ORDER BY CreateTime DESC, action ASC, Msg ASC
                                ";
                    cmd.CommandText = queryStr;

                }
                else
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = @"UserActoinMsg_SEL";
                }

                #region Query Parameters

                cmd.Parameters.Clear();
                /*userid*/
                var paramUserid = cmd.CreateParameter();
                paramUserid.ParameterName = "@userid";
                if (string.IsNullOrEmpty(model.userid))
                    paramUserid.Value = DBNull.Value;
                else
                    paramUserid.Value = model.userid;
                cmd.Parameters.Add(paramUserid);

                /*action*/
                var paramAction = cmd.CreateParameter();
                paramAction.ParameterName = "@action";
                string actionParam = actionCode.Replace("_", "[_]");//底線為萬用字完，所以要用括號括起來
                if (string.IsNullOrEmpty(actionCode))
                    paramAction.Value = DBNull.Value;
                else
                    paramAction.Value = actionParam;
                cmd.Parameters.Add(paramAction);

                /*StartDate*/
                var paramStartDate = cmd.CreateParameter();
                paramStartDate.ParameterName = "@StartDate";
                if (model.StartDate == null)
                    paramStartDate.Value = DBNull.Value;
                else
                    paramStartDate.Value = model.StartDate;
                cmd.Parameters.Add(paramStartDate);

                /*EndDate*/
                var paramEndDate = cmd.CreateParameter();
                paramEndDate.ParameterName = "@EndDate";
                if (model.EndDate == null)
                    paramEndDate.Value = DBNull.Value;
                else
                    paramEndDate.Value = model.EndDate;
                cmd.Parameters.Add(paramEndDate);
                #endregion

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UserActionMsgViewModel userAction = new UserActionMsgViewModel();

                        userAction.userid = reader["userid"].ToString();

                        #region 組合 action string
                        string actionStr = string.Empty;
                        string switchAcion = string.Empty;

                        if (actionType == "product")
                        {
                            #region 如為組模查詢，則將查詢出來 reader["Action"] 來組合 action string

                            switchAcion = reader["Action"].ToString().Split('_')[0];

                            switch (switchAcion)
                            {
                                case "BUY":
                                    if (reader["Action"].ToString() != "BUY_MAIL")
                                        actionStr = "購買";
                                    else
                                        actionStr = reader["Action"].ToString();
                                    break;
                                case "BUYFAIL":
                                    actionStr = "購買失敗";
                                    break;
                                case "RESUB":
                                    actionStr = "重新購買";
                                    break;
                                case "REBUYSUCCESS":
                                    actionStr = "續訂成功";
                                    break;
                                case "REBUYFAIL":
                                    actionStr = "續訂失敗";
                                    break;
                                case "CANCELSUB":
                                    actionStr = "取消訂閱";
                                    break;
                                case "PAYERROR":
                                    actionStr = "付款失敗";
                                    break;
                                case "SYS":
                                    actionStr = "後台模組開通";
                                    break;
                                case "COST":
                                    actionStr = "模組售價";
                                    break;
                                
                                default:
                                    actionStr = reader["Action"].ToString();
                                    break;
                            }

                            switch (reader["Action"].ToString().Replace(switchAcion, ""))
                            {
                                case "_SENSOR":
                                    actionStr += "-策略模組";
                                    break;
                                case "_STRATEGY":
                                    actionStr += "-選股模組";
                                    break;
                                case "_WS":
                                    actionStr += "-權證模組";
                                    break;
                                case "_BS":
                                    actionStr += "-籌碼模組";
                                    break;
                                case "_INDU":
                                    actionStr += "-產業模組";
                                    break;
                                case "_CN":
                                    actionStr += "-陸股即時模組";
                                    break;
                                case "_HK":
                                    actionStr += "-港股即時模組";
                                    break;
                                case "_TW":
                                    actionStr += "-台股即時模組";
                                    break;
                                case "_TW300":
                                    actionStr += "-新台股即時模組（300）";
                                    break;
                                case "_US":
                                    actionStr += "-美股即時模組";
                                    break;
                            }
                            #endregion
                        }
                        else
                        {
                            #region 如為一般查詢，則直接用 actionCode 來組合 action string

                            switchAcion = actionCode;

                            switch (switchAcion)
                            {
                                case "ADD":
                                    //特殊案例目前只會有權證模組
                                    actionStr = "新增權限-權證模組";
                                    break;
                                case "DEL_":
                                    //特殊案例目前只會有權證模組
                                    actionStr = "刪除權限-權證模組";
                                    break;
                                case "SEND_SMS":
                                    actionStr = "發送認證信";
                                    break;
                                case "LOGIN":
                                    actionStr = "登入";
                                    break;
                                case "NEW_ACCOUNT_Agree":
                                    actionStr = "同意免責聲明";
                                    break;
                                case "NEW_ACCOUNT":
                                    actionStr = "新註冊";
                                    break;
                                case "SEND_CHANGE_EMAIL_CHECK":
                                    actionStr = "寄送認證信";
                                    break;
                                case "SEND_EMAIl_EXPIRE":
                                    actionStr = "通知權限到期";
                                    break;
                                case "CHANGE_MOBILE":
                                    actionStr = "修改手機號碼";
                                    break;
                                case "RE_SEND_EMAIL_CHECK":
                                    actionStr = "重新發送驗證信";
                                    break;
                                case "VerifyBroker":
                                    actionStr = "券商驗證";
                                    break;
                                default:
                                    actionStr = reader["Action"].ToString();
                                    break;
                            }

                            /**在前面的action 判斷完之後，才會做美股Action判斷**/
                            if (!string.IsNullOrEmpty(actionCode) && actionCode == "FreeTrial")
                            {
                                switch (reader["Action"].ToString())
                                {
                                    case "FreeTrial_US":
                                        actionStr = "美股贈送";
                                        break;
                                    case "extendFreeTrial_MAil":
                                        actionStr = "美股延展紀錄寄送";
                                        break;
                                    case "FreeTrial_USD":
                                        actionStr = "美股延遲模組贈送";
                                        break;
                                }
                            }
                            #endregion
                        }
                        #endregion

                        userAction.Action = actionStr;
                        userAction.Msg = reader["Msg"].ToString();
                        userAction.CreateTime = Convert.ToDateTime(reader["CreateTime"].ToString()).ToString("yyyy/MM/dd hh:mm:ss");
                        userAction.email = reader["email"].ToString();

                        if (string.IsNullOrEmpty(reader["Active"].ToString()))
                            userAction.Active = true;
                        else
                            userAction.Active = Convert.ToBoolean(reader["Active"].ToString());

                        list.Add(userAction);
                    }
                }

                return list;
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

    public class UserActionMsgViewModel
    {
        public string Appid { get; set; }
        public string userid { get; set; }
        public string ParentsAction { get; set; }
        public string Action { get; set; }
        public string Msg { get; set; }
        public string CreateTime { get; set; }
        public string IpAddress { get; set; }
        public string email { get; set; }
        public bool Active { get; set; }
        public System.DateTime? StartDate { get; set; }
        public System.DateTime? EndDate { get; set; }

    }
}