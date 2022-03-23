import React from 'react';
import { message, Spin, Select, Table, DatePicker, Tabs, Button, Modal, Input, Form } from 'antd';

var _common = require('../../shared/script.js');

export class SysAccountMaintenance extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            load: false,
            dialogCreateAccVisible: false,
            dialogLoading: false,
            dlgDelconfirmVisible: false,
            dlgResetPWconfirmVisible: false
        }

        this.userListCols =
            [
                { title: "使用者帳號", dataIndex: "name", align: 'center' },
                // { title: "權限", dataIndex: "RankName", align: 'center' },
                {
                    title: "操作", dataIndex: "operat", align: 'center', render: (r, model) =>
                        <div>
                            <div style={{ color: "#00b0ff", cursor: "pointer", display: "inline-block" }}
                                onClick={this.resetPW.bind(this, model)}
                            >密碼重設</div>
                            &nbsp;|&nbsp;
                            <div style={{ color: "#00b0ff", cursor: "pointer", display: "inline-block" }}
                                onClick={this.openDeleteconfirmation.bind(this, model)}
                            >刪除</div>
                        </div>
                },
            ];
    }

    componentDidMount() {
        this.getSysUserList();
    }

    openCreateAccDlg = () => {
        this.setState({ dialogCreateAccVisible: true, dialogLoading: false });
    }

    resetPW = (model, e) => {
        this.setState({ dlgResetPWconfirmVisible: true, userID: model.name });
    }

    openDeleteconfirmation = (model, e) => {
        this.setState({ dlgDelconfirmVisible: true, userID: model.opseq });
    }

    closeDlg = () => {
        const { resetFields } = this.props.form;

        this.setState({ dialogCreateAccVisible: false, dlgDelconfirmVisible: false, dlgResetPWconfirmVisible: false });

        resetFields();
    }
    /* form */
    handleSubmit = e => {
        e.preventDefault();
        this.props.form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                this.add(values);
                // console.log('Received values of form: ', values);
            }
        });
    };

    add = (values) => {
        this.checkInsertOperatorData(values);
    }

    checkInsertOperatorData = (values) => {
        let param = { name: values.account };
        this.setState({ dialogLoading: true });

        //確認帳號重複
        $.ajax("../api/System/CheckIsAccountDuplicate", {
            type: "POST",
            data: JSON.stringify(param),
            contentType: "application/json",
            processData: false,
            success: data => {
                try {
                    if (data) {
                        _common.ShowMessage("error", "帳號重複，請重請輸入");
                        this.setState({ dialogLoading: false });
                    }
                    else
                        this.insertOperator(values);

                } catch (ex) {
                    _common.ShowMessage("error", "確認帳號重複無法解析");
                    this.setState({ dialogLoading: false });
                    console.log(ex);
                }
            },
            complete: () => {

            },
            error: err => {
                _common.ShowMessage("error", "CheckIsAccountDuplicate 無法查詢");
                this.setState({ dialogLoading: false });
                console.log(err);
            }
        });
    }

    insertOperator = (values) => {
        let param = { name: values.account, password: values.password };


        $.ajax("../api/System/InsertOperator", {
            type: "POST",
            data: JSON.stringify(param),
            contentType: "application/json",
            processData: false,
            success: data => {
                try {


                } catch (ex) {
                    _common.ShowMessage("error", "新增後台帳號無法解析");
                    this.setState({ dialogLoading: false });
                    console.log(ex);
                }
            },
            complete: () => {
                this.setState({ dialogCreateAccVisible: false, dialogLoading: false });
                this.closeDlg();
                this.getSysUserList();
            },
            error: err => {
                _common.ShowMessage("error", "InsertOperator 發生錯誤");
                this.setState({ dialogLoading: false });
                console.log(err);
            }
        });
    }

    checkRank = (rule, value, callback) => {
        if (value.accType == "1" && (value.tsid == '' || value.tsid == undefined)) {
            callback('請選擇券商');
            return;
        }

        callback();

    };

    validateToNextPassword = (rule, value, callback) => {
        const { form } = this.props;
        if (value && this.state.confirmDirty) {
            form.validateFields(['confirm'], { force: true });
        }
        callback();
    };

    handleConfirmBlur = e => {
        const { value } = e.target;
        this.setState({ confirmDirty: this.state.confirmDirty || !!value });
    };

    compareToFirstPassword = (rule, value, callback) => {
        const { form } = this.props;
        if (value && value !== form.getFieldValue('password')) {
            callback('密碼確認不相符');
        } else {
            callback();
        }
    };
    /* form */

    resetSysUserPW = () => {
        this.setState({ dialogLoading: true });

        let { userID } = this.state;

        let param = { name: userID };

        $.ajax("../api/System/ResetSysUserPW", {
            type: "POST",
            data: JSON.stringify(param),
            contentType: "application/json",
            processData: false,
            success: data => {
                try {

                } catch (ex) {
                    _common.ShowMessage("error", "變更密碼無法解析");
                    this.setState({ dialogLoading: false });
                    console.log(ex);
                }
            },
            complete: () => {
                this.setState({ dlgResetPWconfirmVisible: false, dialogLoading: false });
                _common.ShowMessage("success", "變更密碼成功。");

            },
            error: err => {
                _common.ShowMessage("ResetSysUserPW 無法查詢");
                this.setState({ dialogLoading: false });
                console.log(err);
            }
        });

    }

    deleteSysUser = () => {
        this.setState({ dialogLoading: true });

        let { userID } = this.state;

        let param = { name: userID };

        $.ajax("../api/System/DeleteSysUser", {
            type: "POST",
            data: JSON.stringify(param),
            contentType: "application/json",
            processData: false,
            success: data => {
                try {

                } catch (ex) {
                    _common.ShowMessage("error", "刪除系統使用者無法解析");
                    console.log(ex);
                }
            },
            complete: () => {
                this.setState({ dlgDelconfirmVisible: false, dialogLoading: false });
                this.getSysUserList();
            },
            error: err => {
                _common.ShowMessage("DeleteSysUser 無法查詢");
                console.log(err);
            }
        });

    }

    getSysUserList = () => {
        this.setState({ load: true });

        let userList = [];

        $.ajax("../api/System/GetSysUserList", {
            type: "GET",
            contentType: "application/json",
            processData: false,
            success: data => {
                try {
                    data.map(r => userList.push(r));
                } catch (ex) {
                    _common.ShowMessage("error", "系統使用者列表無法解析");
                    console.log(ex);
                }
            },
            complete: () => {
                this.setState({ userList: userList })
                this.setState({ load: false });
            },
            error: err => {
                _common.ShowMessage("error", "GetSysUserList Error");
                console.log(err);
            }
        });
    }


    render() {
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
        const { getFieldDecorator } = this.props.form;

        let { load, userList, userID, dialogLoading, dialogCreateAccVisible, dlgResetPWconfirmVisible, dlgDelconfirmVisible } = this.state;

        return (
            <div>
                <div style={{ padding: "15px 0px 15px 0px" }}>
                    <Button type="primary" onClick={this.openCreateAccDlg}>新增帳號</Button>
                </div>
                <div>
                    <Table
                        loading={load}
                        style={{ width: "1130px" }}
                        columns={this.userListCols}
                        dataSource={userList}
                        rowKey={(r, j) => "acc." + j}
                        pagination={{ showSizeChanger: true, showQuickJumper: true }}
                        size="small" />
                    <Modal title="新增帳號" width="450px"
                        visible={dialogCreateAccVisible}
                        onCancel={this.closeDlg}
                        footer="">
                        <Spin spinning={dialogLoading}>
                            <Form {...formItemLayout} onSubmit={this.handleSubmit}>
                                <Form.Item label="權限">
                                    {getFieldDecorator('rank', {
                                        initialValue: '0',
                                        rules: [{ validator: this.checkRank }],
                                    })(
                                        <Select style={{ width: "120px" }}>
                                            <Select.Option value="0">系統管理者</Select.Option>
                                            <Select.Option value="1">主管權限</Select.Option>
                                            <Select.Option value="2">行銷權限</Select.Option>
                                        </Select>
                                    )}
                                </Form.Item>
                                <Form.Item label="帳號">
                                    {getFieldDecorator('account', {
                                        rules: [
                                            {
                                                max: 50,
                                                message: '帳號字數不可以大於50',
                                            },
                                            {
                                                required: true,
                                                message: '請輸入帳號',
                                            },
                                        ],
                                    })(<Input />)}
                                </Form.Item>
                                <Form.Item label="密碼" hasFeedback extra="至少須六個字以上">
                                    {getFieldDecorator('password', {
                                        rules: [
                                            {
                                                min: 6,
                                                message: '不可以小於六個字',
                                            }, {
                                                required: true,
                                                message: '請輸入密碼',
                                            },
                                            {
                                                validator: this.validateToNextPassword,
                                            },
                                        ],
                                    })(<Input.Password />)}
                                </Form.Item>
                                <Form.Item label="確認密碼" hasFeedback>
                                    {getFieldDecorator('confirm', {
                                        rules: [
                                            {
                                                required: true,
                                                message: '請確認你的密碼',
                                            },
                                            {
                                                validator: this.compareToFirstPassword,
                                            },
                                        ],
                                    })(<Input.Password onBlur={this.handleConfirmBlur} />)}
                                </Form.Item>
                                <Form.Item {...tailFormItemLayout}>
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <Button key="取消" onClick={this.closeDlg}>
                                        取消
                        </Button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <Button key="確認新增" type="primary" htmlType="submit" >
                                        確認新增
                        </Button>
                                </Form.Item>
                            </Form>
                        </Spin>
                    </Modal>
                    <Modal title="密碼動設" width="300px"
                        visible={dlgResetPWconfirmVisible}
                        onCancel={this.closeDlg}
                        onOk={this.resetSysUserPW}
                        okText="確認"
                        cancelText="取消">
                        <Spin spinning={dialogLoading}>
                            確定要將帳號：{userID} 使用者的密碼重新設定為預計值 123456 嗎？
                        </Spin>
                    </Modal>
                    <Modal title="刪除確認" width="200px"
                        visible={dlgDelconfirmVisible}
                        onCancel={this.closeDlg}
                        onOk={this.deleteSysUser}
                        okText="確認"
                        cancelText="取消">
                        <Spin spinning={dialogLoading}>
                            確定要刪除嗎？
                        </Spin>
                    </Modal>
                </div>
            </div>
        );
    }
}