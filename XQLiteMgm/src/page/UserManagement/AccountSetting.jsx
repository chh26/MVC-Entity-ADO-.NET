import React from 'react';
import {  Button, Spin, Form, Input, Switch, Modal, Table } from 'antd';
const { TextArea } = Input;

var _common = require('../../shared/script.js');

export class AccountSetting extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            loading:false,
            isAccountValuesSetting: false,
            historyRecordVisible: false,
            historyData:[]
        };

        this.Cols = [
            { 
                title: "事件", 
                dataIndex: "Action", 
                align:'center',
                width:180,
                render: (t, r) => t.toString() === "Account_Disabled" ? 
                <div style={{color:'#FF0000'}}>帳號停用</div> : <div style={{color:'#32CD32'}}>帳號啟用</div>
            },
            { title: "原因", dataIndex: "Msg"},
            { title: "時間", dataIndex: "CreateTime", align:'center' }
        ];
    }

    componentDidMount() {
        let { userid, isAccountValuesSetting } = this.props;

        if (!isAccountValuesSetting) {
            this.setUserPWD(userid);
            this.props.onValuesSetting(true);
        }
    }

    componentDidUpdate(prevProps, prevState) {
        let { userid,isAccountValuesSetting  } = this.props;
        if (!isAccountValuesSetting) {
            this.setUserPWD(userid);
            this.props.onValuesSetting(true);
        }
    }

    /* form */
    handleSubmit = e => {
        e.preventDefault();
        this.props.form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                // console.log('Received values of form: ', values);
                this.Update(values);

            }
        });
    };

    Update = (fromValues) => {
        let param = {
            userid : fromValues.userid,
            deleteflag : !fromValues.enable,
            Msg : fromValues.msg
        }
        // console.log('Update param of form: ', param);

        this.setState({ loading: true });

        $.ajax("../api/User/MaintenUserPWD", {
            type: "POST",
            data: JSON.stringify(param),
            contentType: "application/json",
            processData: false,
            success: data => {
                try {
                    // console.log("MaintenUserPWD:", data);

                    this.alertUpdateResult(data.ResultStatus);

                } catch (ex) {
                    _common.ShowMessage("error", "更新使用者帳號資訊無法解析");
                    this.setState({ loading: false });
                    console.log(ex);
                }
            },
            complete: () => {
            },
            error: err => {
                _common.ShowMessage("error", "MaintenUserPWD 無法查詢");
                this.setState({ loading: false });
                console.log(err);
            }
        });

    }

    setUserPWD =  (userid) =>{
        let param = {
            userid : userid
        }

        this.setState({ loading: true });

        $.ajax("../api/User/GetUserPWDByUser", {
            type: "POST",
            data: JSON.stringify(param),
            contentType: "application/json",
            processData: false,
            success: data => {
                try {
                    // console.log("GetUserPWDByUserReturnData:", data);
                    if (data.ResultStatus == 1) {
                        data = data.Result;

                        this.props.form.setFieldsValue({
                            userid: data.userid,
                            enable: !data.deleteflag,//deleteflag=false為啟用
                            msg: "",
                        });
            
                        this.props.onValuesSetting(true);
                    }
                    else {
                        Modal.error({
                            title: "",
                            content: "資料取得發生錯誤",
                            onOk: () => this.onCloseDlg("")
                          });

                        console.log("GetUserPWDByUserError:", data.Msg);

                    }
                } catch (ex) {
                    _common.ShowMessage("error", "取得使用者帳號資訊無法解析");
                    console.log(ex);
                }

                this.setState({ loading: false });

            },
            complete: () => {
            },
            error: err => {
                _common.ShowMessage("error", "GetUserPWDByUser 無法查詢");
                this.setState({ loading: false });
                console.log(err);
            }
        });
    }

    openHistoryRecord = () => {
        this.setState({ historyRecordVisible: true });

        let { userid} = this.props;

        let param = {
            userid : userid,
            Action:"Account_!normal"
        }

        $.ajax("../api/User/GetUserAccountAction", {
            type: "POST",
            data: JSON.stringify(param),
            contentType: "application/json",
            processData: false,
            success: data => {
                try {
                    // console.log("GetUserAccountAction:", data);
                    if (data.ResultStatus == 1) {
                        data = data.Result;

                        this.setState({historyData:data});
                    }
                    else {
                        Modal.error({
                            title: "",
                            content: "取得歷史紀錄發生錯誤",
                            onOk: () => this.onCloseDlg("")
                          });

                        console.log("GetUserAccountAction:", data.Msg);

                    }
                } catch (ex) {
                    _common.ShowMessage("error", "取得歷史紀錄資訊無法解析");
                    console.log(ex);
                }

                this.setState({ loading: false });

            },
            complete: () => {
            },
            error: err => {
                _common.ShowMessage("error", "GetUserAccountAction 無法查詢");
                this.setState({ loading: false });
                console.log(err);
            }
        });
        
    }

    onPropsCloseDlg = () => {
        this.props.onCloseDlg();
    }

    onCloseDlg = () => {
        this.setState({ historyRecordVisible: false });
    }

    alertUpdateResult = (ResultStatus) => {
        this.setState({ loading: false });
        switch (ResultStatus) {
            case 1:
                _common.ShowMessage("success", "更新成功。");
                this.onPropsCloseDlg("");
                break;
            case 0:
                _common.ShowMessage("error", "更新失敗。");
        }
    }

    render() {
        const { getFieldDecorator, getFieldValue } = this.props.form;
        const formItemLayout = {
            labelCol: {
                xs: { span: 24 },
                sm: { span: 6 },
            },
            wrapperCol: {
                xs: { span: 1 },
                sm: { span: 14 },
            },
        };
        const tailFormItemLayout = {
            wrapperCol: {
                xs: {
                    span: 24,
                    offset: 0,
                },
                sm: {
                    span: 16,
                    offset: 8,
                },
            },
        };

        let {loading,historyRecordVisible, historyData} = this.state;
        return (
            <Spin spinning={loading}>
                <Form {...formItemLayout} onSubmit={this.handleSubmit}>
                    <Form.Item label="帳號">
                        <div></div>
                        {getFieldDecorator('userid', {})(
                        <div>
                            {getFieldValue('userid')}
                            <Button key="historyRecord" style={{marginLeft:"10px"}} onClick={this.openHistoryRecord}>
                                歷史紀錄
                            </Button>
                        </div>)}
                    </Form.Item>
                    <Form.Item label="狀態">
                         {getFieldDecorator('enable')(
                            <Switch
                                checkedChildren="啟用"
                                unCheckedChildren="停用"
                                checked={getFieldValue('enable')}
                                onChange={this.onSubChange}
                            />  
                        )}
                    </Form.Item>
                    <Form.Item label="停/復權原因">
                        {getFieldDecorator('msg', {})(<TextArea rows={4} />)}
                    </Form.Item>
                    <Form.Item {...tailFormItemLayout}>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <Button key="取消" onClick={this.onPropsCloseDlg}>
                            取消
                        </Button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <Button key="確認" type="primary" htmlType="submit" >
                            確認
                        </Button>
                    </Form.Item>
                </Form>
                <div>
                    <Modal title="歷史紀錄" width="600px" bodyStyle={{ height: "500px" }}
                        visible={historyRecordVisible}
                        onCancel={this.onCloseDlg}
                        footer={[<Button key="back" onClick={this.onCloseDlg}>關閉</Button>]}
                    >
                        <div>
                        <Table
                            columns={this.Cols}
                            dataSource={historyData}
                            rowKey={(r, j) => "info." + j}
                            pagination={{ showSizeChanger: true, showQuickJumper: true }}
                            size="small" />
                        </div>
                    </Modal>
                </div>
            </Spin>
        )
    }
}