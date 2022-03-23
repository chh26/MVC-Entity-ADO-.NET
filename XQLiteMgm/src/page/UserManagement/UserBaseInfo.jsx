import React from 'react';
import {  Button, Spin, Form, Input } from 'antd';

var _common = require('../../shared/script.js');

export class UserBaseInfo extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            loading:false,
            isValuesSetting: false,
            userid:''
        };
    }

    componentDidMount() {

        let { userBaseInfo, isValuesSetting } = this.props;

        if (!isValuesSetting) {

            this.props.form.setFieldsValue({
                userid: userBaseInfo.name,
                mobile: userBaseInfo.mobile,
                email: userBaseInfo.email,
            });

            this.props.onValuesSetting(true);
        }
    }

    componentDidUpdate(prevProps, prevState) {
        let { userBaseInfo,isValuesSetting  } = this.props;
        if (!isValuesSetting) {

            this.props.form.setFieldsValue({
                userid: userBaseInfo.name,
                mobile: userBaseInfo.mobile,
                email: userBaseInfo.email,
            });

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
        let currentInfo = this.props.userBaseInfo;
        let param = {
            userid : fromValues.userid,
            currentmobile : currentInfo.mobile,
            newmobile : fromValues.mobile,
            currentemail : currentInfo.email,
            newemail : fromValues.email
        }
        // console.log('Update param of form: ', param);

        $.ajax("../api/User/MaintenBaseInfo", {
            type: "POST",
            data: JSON.stringify(param),
            contentType: "application/json",
            processData: false,
            success: data => {
                try {
                    data = JSON.parse(data);
                    // console.log("MaintenBaseInfoReturnData:", data);
                    
                    this.alertUpdateResult(data.StatusCode);

                } catch (ex) {
                    _common.ShowMessage("error", "更新使用者資訊無法解析");
                    this.setState({ loading: false });
                    console.log(ex);
                }
            },
            complete: () => {
            },
            error: err => {
                _common.ShowMessage("error", "MaintenBaseInfo 無法查詢");
                this.setState({ loading: false });
                console.log(err);
            }
        });

    }

    onCloseDlg = (s) => {
        this.props.onCloseDlg(s);
    }

    alertUpdateResult = (statusCode) => {
        this.setState({ loading: false });
        switch (statusCode) {
            case "00":
                _common.ShowMessage("success", "更新成功。");
                this.onCloseDlg("update");
                break;
            case "10":
                _common.ShowMessage("error", "更新失敗，userid 不可為空值。");
                break;
            case "20":
                _common.ShowMessage("error", "更新失敗，原mobile 或 新mobile 為空值。");
                break;
            case "21":
                _common.ShowMessage("error", "更新失敗，mobile 重覆。");
                break;
            case "22":
                _common.ShowMessage("error", "更新失敗，確認mobile是否重覆時，發生錯誤。");
                break;
            case "30":
                _common.ShowMessage("error", "更新失敗，原email 或 新email 為空值。");
                break;
            case "31":
                _common.ShowMessage("error", "更新失敗，Email 重覆。");
                break;
            case "32":
                _common.ShowMessage("error", "更新失敗，確認Email是否重覆時，發生錯誤。");
                break;
            case "40":
                _common.ShowMessage("error", "更新失敗，更新mobile、email時，發生錯誤。");
                break;
            case "41":
                _common.ShowMessage("error", "更新失敗，將舊mail 失效時，發生錯誤。");
                break;
            case "42":
                _common.ShowMessage("error", "更新失敗，新增新的mail時，發生錯誤。");
                break;
            case "43":
                _common.ShowMessage("error", "更新失敗，寫入UserActionMsg時，發生錯誤。");
                break;
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

        let {userid} = this.state;
        return (
            <Spin spinning={false}>
                <Form {...formItemLayout} onSubmit={this.handleSubmit}>
                    <Form.Item label="帳號">
                        {getFieldDecorator('userid', {})(<div>{getFieldValue('userid')}</div>)}
                    </Form.Item>
                    <Form.Item label="Mobile">
                        {getFieldDecorator('mobile', {
                            rules: [
                                {
                                    max: 50,
                                    message: '帳號字數不可以大於50',
                                },
                                {
                                    required: true,
                                    message: 'Mobile欄位為必填',
                                },
                            ],
                        })(<Input/>)}
                    </Form.Item>
                    <Form.Item label="email">
                        {getFieldDecorator('email', {
                            rules: [
                                {
                                  type: 'email',
                                  message: '不符合E-mail格式',
                                },
                                {
                                  required: true,
                                  message: 'E-mail欄位為必填',
                                },
                              ],
                        })(<Input/>)}
                    </Form.Item>
                    <Form.Item {...tailFormItemLayout}>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <Button key="取消" onClick={this.onCloseDlg}>
                            取消
                        </Button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <Button key="確認" type="primary" htmlType="submit" >
                            確認
                        </Button>
                    </Form.Item>
                </Form>
            </Spin>
        )
    }
}