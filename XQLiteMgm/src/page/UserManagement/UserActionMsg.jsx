import React from 'react';
import { Input, Table, Spin, DatePicker, Select, Modal } from 'antd';
import moment from 'moment';

var _common = require('../../shared/script.js');

const Search = Input.Search;
const RangePicker = DatePicker.RangePicker;
const Option = Select.Option;
const dtFmt = "YYYY/MM/DD";
const userActionData = require('json-loader!../../App_Data/userActionData.json');

export class UserActionMsg extends React.Component {
    constructor(props) {
        super(props);

        this.cols = [
            {
                title: "類別",
                dataIndex: "email",
                render: (t, r) => {
                    if (!t || "發送認證信" === r.Action) return "";

                    let ActiveMessage = true === r.Active ? "" : "*";

                    return /@moneydj\.com/i.test(t) ?
                        <span className="disgust">[{ActiveMessage}員工帳號]</span> :
                        <span className="secure">[{ActiveMessage}客戶帳號]</span>;
                }
            },
            { title: "UserID", dataIndex: "userid" },
            { title: "事件", dataIndex: "Action",
            sorter: (a, b) => a.Action.localeCompare(b.Action) },
            { title: "訊息", dataIndex: "Msg",width: 550,
            render: (text, record) => (
                <div style={{ wordWrap: 'break-word', wordBreak: 'break-word' }}>
                  {text}
                </div>
              )},
            {
                title: "時間",
                dataIndex: "CreateTime",
                sorter: (a, b) => moment(a.CreateTime).unix() - moment(b.CreateTime).unix(),
                render: t => {
                    try {
                        return t.replace("T", " ");
                    }
                    catch (ex) {
                        return null;
                    }
                }
            }
        ];

        this._default = { tdy: moment(), parentsAction: "Model!product"};//預設值
        this.state = { loading: false, dt1: this._default.tdy, dt2: this._default.tdy, parentsAction: "Model!product", Action: "Model!product" };
    }
    _go = async UserID => {
        let { dt1, dt2, Action } = this.state;

        Action = $.trim(Action);

        let param = {
            userid: UserID,
            ParentsAction:this._default.parentsAction,
            Action: Action,
            StartDate: dt1.format(dtFmt),
            EndDate: dt2.format(dtFmt)
        };

        UserID ? (
            this._default.lock || (
                this._default.lock = true, // 以第一次查詢為主
                this.setState({ load: true }),
                $.ajax("../api/User/GetUserActionRecord", {
                    type: "POST",
                    data: JSON.stringify(param),
                    contentType: "application/json",
                    processData: false,
                    success: data => {
                        try {
                            // console.log("GetUserAccountAction:", data);
                            if (data.ResultStatus == 1) {
                                data = data.Result;
                                let rows = [], filt = r => rows.push(r);

                                switch (Action) {
                                    case "NEW_ACCOUNT":
                                        filt = r => "新註冊" === r.Action && rows.push(r);
                                        break;
                                }

                                data.map(r => filt(r));

                                this.setState({ rows });
                            }
                            else {
                                Modal.error({
                                    title: "",
                                    content: "取得歷史紀錄發生錯誤"
                                });

                                // console.log("GetUserAccountAction:", data.Msg);

                            }
                        } catch (ex) {
                            _common.ShowMessage("error", "取得歷史紀錄資訊無法解析");
                            console.log(ex);
                        }


                    },
                    complete: () => {
                        this.setState({ loading: false });
                        this._default.lock = false;
                    },
                    error: err => {
                        _common.ShowMessage("error", "GetUserAccountAction 無法查詢");
                        console.log(err);
                    }
                })
            )
        ) : _common.ShowMessage("warn", "請先輸入使用者ID以查詢");
    }

    onDataRangeChange = async dts => this.setState({ dt1: dts[0], dt2: dts[1] })

    onOptionChange = async Action => {
        this.setState({subOptionVisible:false });

        let subOption = [];
        let subOptionVisible = false;

        subOption = userActionData[Action.split('!')[0]];//子acion

        if (subOption != undefined && subOption.length > 0)
            subOptionVisible = true;

        $.extend(this._default, { parentsAction: Action });//父 action
        this.setState({ Action, subOptionVisible, subOption, subOptionValue : " " });

    }

    onSubOptionChange = async Action => {
        this.setState({subOptionValue:Action});

        if (Action === " ")
            Action = this._default.parentsAction;//選擇全部時，將父acion塞回查詢acion

        this.setState({ Action });
    }
    render() {
        let { loading, rows = [], dt1, dt2, parentsAction, subOptionVisible = false, subOption = [], subOptionValue=" " } = this.state;
        return (
            <Spin spinning={loading}>
                <label>
                    日期：
                    <RangePicker
                        defaultValue={[dt1, dt2]}
                        format={dtFmt}
                        allowClear={false}
                        disabledDate={c => c > this._default.tdy}//只能選擇今天以前
                        onChange={this.onDataRangeChange} style={{ width: "230px" }} />
                </label> &nbsp;
                <label>
                    事件：
                    <Select defaultValue={parentsAction}  onChange={this.onOptionChange} style={{ width: "145px" }}>
                        {/* key的格式：NEW_ACCOUNT（查詢代號）!normal（查詢類型） */}
                        <Option key="Model!product">模組相關</Option>
                        <Option key="BUY_!product">購買</Option>
                        <Option key="BUYFAIL_!product">購買失敗</Option>
                        <Option key="RESUB_!product">重新購買</Option>
                        <Option key="REBUYSUCCESS_!product">續訂成功</Option>
                        <Option key="REBUYFAIL_!product">續訂失敗</Option>
                        <Option key="CANCELSUB_!product">取消訂閱</Option>
                        <Option key="PAYERROR_!product">付款失敗</Option>
                        <Option key="ADD!normal">新增權限</Option>
                        <Option key="DEL_!normal">刪除權限</Option>
                        <Option key="SYS_!product">後台模組開通</Option>
                        <Option key="COST_!product">模組售價</Option>
                        <Option key="NEW_ACCOUNT!normal">新註冊</Option>
                        <Option key="NEW_ACCOUNT_Agree!normal">同意免責聲明</Option>
                        <Option key="LOGIN!normal">登入</Option>
                        <Option key="SEND_SMS!normal">發送簡訊認證</Option>
                        <Option key="CHANGE_MOBILE!normal">修改手機號碼</Option>
                        <Option key="SEND_CHANGE_EMAIL_CHECK!normal">寄送認證信</Option>
                        <Option key="RE_SEND_EMAIL_CHECK!normal">重新發送驗證信</Option>
                        <Option key="SEND_EMAIl_EXPIRE!normal">通知權限到期</Option>
                        <Option key="FreeTrial!normal">美股贈送</Option>
                        <Option key="VerifyBroker!normal">券商驗證</Option>
                    </Select>&nbsp;
                    {
                        subOptionVisible ?
                            <Select defaultValue=" " value={subOptionValue} onChange={this.onSubOptionChange} style={{ width: "280px" }}>
                                <Option key=" ">全部</Option>
                                {subOption.map(option => <Option key={option.key+"!product"}>{option.text}</Option>)}
                            </Select>
                            : null
                    }

                </label> &nbsp;
                <div className="search-wrapper" style={{ height: "32px", verticalAlign: "top" }}>
                    <Search placeholder="請輸入使用者ID" onSearch={this._go} enterButton="查詢" style={{ width: "235px" }}/>
                </div>
                <span className="caution">＊表示未驗證E-Mail</span>
                <Table
                    columns={this.cols}
                    dataSource={rows}
                    rowKey={(r, j) => "evt." + j}
                    pagination={{ showSizeChanger: true, showQuickJumper: true }}
                    size="small" />
            </Spin>
        )
    }
}