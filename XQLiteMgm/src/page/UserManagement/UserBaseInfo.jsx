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
                    _common.ShowMessage("error", "?????????????????????????????????");
                    this.setState({ loading: false });
                    console.log(ex);
                }
            },
            complete: () => {
            },
            error: err => {
                _common.ShowMessage("error", "MaintenBaseInfo ????????????");
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
                _common.ShowMessage("success", "???????????????");
                this.onCloseDlg("update");
                break;
            case "10":
                _common.ShowMessage("error", "???????????????userid ??????????????????");
                break;
            case "20":
                _common.ShowMessage("error", "??????????????????mobile ??? ???mobile ????????????");
                break;
            case "21":
                _common.ShowMessage("error", "???????????????mobile ?????????");
                break;
            case "22":
                _common.ShowMessage("error", "?????????????????????mobile?????????????????????????????????");
                break;
            case "30":
                _common.ShowMessage("error", "??????????????????email ??? ???email ????????????");
                break;
            case "31":
                _common.ShowMessage("error", "???????????????Email ?????????");
                break;
            case "32":
                _common.ShowMessage("error", "?????????????????????Email?????????????????????????????????");
                break;
            case "40":
                _common.ShowMessage("error", "?????????????????????mobile???email?????????????????????");
                break;
            case "41":
                _common.ShowMessage("error", "?????????????????????mail ???????????????????????????");
                break;
            case "42":
                _common.ShowMessage("error", "???????????????????????????mail?????????????????????");
                break;
            case "43":
                _common.ShowMessage("error", "?????????????????????UserActionMsg?????????????????????");
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
                    <Form.Item label="??????">
                        {getFieldDecorator('userid', {})(<div>{getFieldValue('userid')}</div>)}
                    </Form.Item>
                    <Form.Item label="Mobile">
                        {getFieldDecorator('mobile', {
                            rules: [
                                {
                                    max: 50,
                                    message: '???????????????????????????50',
                                },
                                {
                                    required: true,
                                    message: 'Mobile???????????????',
                                },
                            ],
                        })(<Input/>)}
                    </Form.Item>
                    <Form.Item label="email">
                        {getFieldDecorator('email', {
                            rules: [
                                {
                                  type: 'email',
                                  message: '?????????E-mail??????',
                                },
                                {
                                  required: true,
                                  message: 'E-mail???????????????',
                                },
                              ],
                        })(<Input/>)}
                    </Form.Item>
                    <Form.Item {...tailFormItemLayout}>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <Button key="??????" onClick={this.onCloseDlg}>
                            ??????
                        </Button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <Button key="??????" type="primary" htmlType="submit" >
                            ??????
                        </Button>
                    </Form.Item>
                </Form>
            </Spin>
        )
    }
}