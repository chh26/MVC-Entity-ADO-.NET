import React from 'react';
import { Select, Input, Button, Table } from 'antd';
import moment from 'moment';

var _common = require('../../shared/script.js');

const { Option } = Select;

export class UserLogQuery extends React.Component {
    constructor(prop) {
        super(prop);

        this.Cols = [

            { title: "使用者名稱", dataIndex: "userid", align: 'center' },
            { title: "建立時間", dataIndex: "createtime", align: 'center', render: (t, r) => <div>{moment(t).format('YYYY-MM-DD hh:mm:ss')}</div> },
            { title: "IP", dataIndex: "ip", align: 'center' },
            {
                title: "登入狀態",
                dataIndex: "status",
                align: 'center',
                sorter: (a, b) => a.status - b.status,
                render: (t, r) =>{
                    return t.toString() == "0" ?
                    <div style={{ color: '#32CD32' }}>成功</div> : <div style={{ color: '#FF0000' }}>失敗</div>
                }
            }
        ];

        this.state = {
            optionData: [],
            queryText: undefined,
        };
    }

    handleSearch = value => {
        if (value) {
            let param = { userid: value };
            $.ajax("../api/UserLog/GetUserIdList", {
                type: "POST",
                data: JSON.stringify(param),
                contentType: "application/json",
                processData: false,
                success: data => {
                    try {
                        // console.log("GetUserIdList:", data);
                        if (data.ResultStatus == 1) {
                            data = data.Result;
                            this.setState({ optionData: data });
                        }
                        else {
                            Modal.error({
                                title: "",
                                content: "取得USER ID發生錯誤"
                            });

                            console.log("GetUserIdListErrMsg:", data.Msg);

                        }
                    } catch (ex) {
                        _common.ShowMessage("error", "取得取得USER ID無法解析");
                        console.log(ex);
                    }
                },
                complete: () => {
                    this.setState({ loading: false });
                },
                error: err => {
                    _common.ShowMessage("error", "GetUserIdList 無法查詢");
                    console.log(err);
                }
            })
        } else {
            this.setState({ optionData: [] });
        }
    };

    handleChange = value => {
        this.setState({ queryText: value });

    };

    onSearch = () => {
        let searchParam;
        this.state.queryText ? searchParam = { userid: this.state.queryText } : searchParam = { userid: '' };

        $.ajax("../api/UserLog/GetUserLogList", {
            type: "POST",
            data: JSON.stringify(searchParam),
            contentType: "application/json",
            processData: false,
            success: data => {
                try {
                    // console.log("GetUserLogList:", data);
                    if (data.ResultStatus == 1) {
                        data = data.Result;
                        this.setState({ rows: data });
                    }
                    else {
                        Modal.error({
                            title: "",
                            content: "取得使用者登入LOG發生錯誤"
                        });

                        console.log("GetUserLogListErrMsg:", data.Msg);

                    }
                } catch (ex) {
                    _common.ShowMessage("error", "取得取得使用者登入無法解析");
                    console.log(ex);
                }
            },
            complete: () => {
                this.setState({ loading: false });
            },
            error: err => {
                _common.ShowMessage("error", "GetUserLogList 無法查詢");
                console.log(err);
            }
        })
    }

    render() {
        let { loading = false, rows = [] } = this.state

        const options = this.state.optionData.map(d => <Option key={d.userid}>{d.userid}</Option>);

        return (
            <div>
                <div>
                    <Input.Group compact>
                        <div style={{ width: "110px", height: "32px", textAlign: "center", border: "1px solid #d9d9d9", borderTopLeftRadius: "4px", borderBottomLeftRadius: "4px" }}>
                            <div style={{ paddingTop: "5px" }}>帳號</div>
                        </div>
                        <Select
                            showSearch
                            value={this.state.queryText}
                            placeholder="輸入 User ID"
                            defaultActiveFirstOption={false}
                            showArrow={false}
                            filterOption={false}
                            onSearch={this.handleSearch}
                            onChange={this.handleChange}
                            notFoundContent={null}
                            style={{ width: "300px" }}
                        >
                            {options}
                        </Select>
                        <Button type="primary" onClick={this.onSearch} >查詢</Button>
                    </Input.Group>
                </div>
                <div>
                    <Table
                        loading={loading}
                        style={{ width: "1130px" }}
                        columns={this.Cols}
                        dataSource={rows}
                        rowKey={(r, j) => "mon." + j}
                        pagination={{ showSizeChanger: true, showQuickJumper: true }}
                        size="small" />
                </div>
            </div>
        )
    }
}