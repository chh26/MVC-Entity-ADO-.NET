import React from 'react';
import {Form, Button, DatePicker, Switch, Icon } from 'antd';
import moment from 'moment';
var _common = require('../../shared/script.js');

export class MaintenPermission extends React.Component {
    constructor(porps){
        super(porps);

        this.state = {
            subStatusMemo : "none"
        }
    }

    componentDidMount() {

        let { permissionInfo, isValuesSetting } = this.props;

        if (!isValuesSetting) {
            this.props.form.setFieldsValue({
                opseq: permissionInfo.opseq,
                orderNo: permissionInfo.orderNo,
                paySdate: moment(permissionInfo.paySdate).format('YYYY-MM-DD'),
                payEdate: moment(moment(permissionInfo.payEdate).format('YYYY-MM-DD')),
                autopay: permissionInfo.autopay == "true"? true : false
            });

            this.props.onValuesSetting(true);
            this.setState({subStatusMemo:"none"});

        }
    }

    componentDidUpdate(prevProps, prevState) {
        let { permissionInfo,isValuesSetting  } = this.props;
        if (!isValuesSetting) {

            this.props.form.setFieldsValue({
                opseq: permissionInfo.opseq,
                orderNo: permissionInfo.orderNo,
                paySdate: moment(permissionInfo.paySdate).format('YYYY-MM-DD'),
                payEdate: moment(moment(permissionInfo.payEdate).format('YYYY-MM-DD')),
                autopay: permissionInfo.autopay == "true"? true : false
            });

            this.props.onValuesSetting(true);
            this.setState({subStatusMemo:"none"});

        }
    }

    onSubChange = (c,e) => {
        if(c) {
            let { permissionInfo  } = this.props;
            this.setState({subStatusMemo:"none"});
            this.props.form.setFieldsValue({
                payEdate: moment(permissionInfo.payEdate)
            });
        }
        else {
            this.setState({subStatusMemo:""});
            this.props.form.setFieldsValue({
                payEdate: moment()
            });
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
        let {permissionInfo} = this.props;
        let param = Object.assign({},permissionInfo);
        param.autopay = fromValues.autopay;
        param.payEdate = moment(fromValues.payEdate).format('YYYY-MM-DD HH:mm:ss')

        // console.log('Update param of form: ', param);

        $.ajax("../api/User/MaintenUserProdutPermission", {
            type: "POST",
            data: JSON.stringify(param),
            contentType: "application/json",
            processData: false,
            success: data => {
                try {
                    // console.log("MaintenUserProdutPermissionReturnData:", data);

                    this.alertUpdateResult(data.StatusCode);

                } catch (ex) {
                    _common.ShowMessage("error", "???????????????????????????????????????");
                    this.setState({ loading: false });
                    console.log(ex);
                }
            },
            complete: () => {
            },
            error: err => {
                _common.ShowMessage("error", "MaintenUserProdutPermission ????????????");
                this.setState({ loading: false });
                console.log(err);
            }
        });
    }

    alertUpdateResult = (statusCode) => {
        this.setState({ loading: false });
        switch (statusCode) {
            case "0":
                _common.ShowMessage("success", "???????????????");
                this.onCloseDlg("update");
                break;
            case "1":
                _common.ShowMessage("error", "?????????????????????");
                break;
            case "2":
                _common.ShowMessage("error", "UserOrderRecord???????????????");
                break;
            case "3":
                _common.ShowMessage("error", "UserPayRecord???????????????");
                break;
            case "4":
                _common.ShowMessage("error", "UserExchAuth???????????????");
                break;
            case "5":
                _common.ShowMessage("error", "UserExtraAuth???????????????");
                break;
            case "6":
                _common.ShowMessage("error", "WS?????????????????????");
                break;
            case "7":
                _common.ShowMessage("error", "??????UserInvoiceRecord?????????????????????");
                break;
            case "8":
                _common.ShowMessage("error", "??????????????????");
                break;
            case "9":
                _common.ShowMessage("error", "UserPointPay???????????????");
                break;
            default:
                _common.ShowMessage("error", "?????????????????????");
                break;
        }
    }

    onCloseDlg = (s) => {
        this.props.onCloseDlg(s);
    }

    render() {
        let {subStatusMemo} = this.state;

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

        const config = {
            rules: [{ type: 'object', required: true, message: 'Please select time!' }],
          };

        return(
            <div>
                <Form {...formItemLayout} onSubmit={this.handleSubmit}>
                    <Form.Item label="??????">
                        {getFieldDecorator('opseq', {})(<div>{getFieldValue('opseq')}</div>)}
                    </Form.Item>
                    <Form.Item label="????????????">
                        {getFieldDecorator('orderNo', {})(<div>{getFieldValue('orderNo')}</div>)}
                    </Form.Item>
                    <Form.Item label="????????????">

                        {getFieldDecorator('autopay')(
                            <Switch
                                checkedChildren="??????"
                                unCheckedChildren="??????"
                                checked={getFieldValue('autopay')}
                                onChange={this.onSubChange}
                            />  
                        )}
                        <div style={{color:'blue', display: subStatusMemo}}>
                            *?????????????????????????????????????????????????????????????????????????????????????????? {moment().format('YYYY-MM-DD')}
                        </div>
                    </Form.Item>
                    <Form.Item label="?????????">
                        {getFieldDecorator('paySdate')(<div>{getFieldValue('paySdate')}</div>)}
                    </Form.Item>
                    {/* <Form.Item label="?????????">
                        {getFieldDecorator('paySdate', config)(<DatePicker disabled={subStatusMemo=="none"? false:true} />)}
                    </Form.Item> */}
                    <Form.Item label="?????????">
                        {getFieldDecorator('payEdate', config)(<DatePicker disabled={subStatusMemo=="none"? false:true} />)}
                        <div style={{color:'blue'}}>
                                *???????????????????????????
                        </div>
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
            </div>
        )
    }
}