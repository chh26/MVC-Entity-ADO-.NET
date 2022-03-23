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
                    _common.ShowMessage("error", "維護使用者模組權限無法解析");
                    this.setState({ loading: false });
                    console.log(ex);
                }
            },
            complete: () => {
            },
            error: err => {
                _common.ShowMessage("error", "MaintenUserProdutPermission 無法查詢");
                this.setState({ loading: false });
                console.log(err);
            }
        });
    }

    alertUpdateResult = (statusCode) => {
        this.setState({ loading: false });
        switch (statusCode) {
            case "0":
                _common.ShowMessage("success", "更新成功。");
                this.onCloseDlg("update");
                break;
            case "1":
                _common.ShowMessage("error", "沒有訂單資訊。");
                break;
            case "2":
                _common.ShowMessage("error", "UserOrderRecord更新失敗。");
                break;
            case "3":
                _common.ShowMessage("error", "UserPayRecord更新失敗。");
                break;
            case "4":
                _common.ShowMessage("error", "UserExchAuth更新失敗。");
                break;
            case "5":
                _common.ShowMessage("error", "UserExtraAuth更新失敗。");
                break;
            case "6":
                _common.ShowMessage("error", "WS權限刪除失敗。");
                break;
            case "7":
                _common.ShowMessage("error", "取得UserInvoiceRecord資訊發生錯誤。");
                break;
            case "8":
                _common.ShowMessage("error", "無發票資訊。");
                break;
            case "9":
                _common.ShowMessage("error", "UserPointPay更新失敗。");
                break;
            default:
                _common.ShowMessage("error", "發生不明錯誤。");
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
                    <Form.Item label="模組">
                        {getFieldDecorator('opseq', {})(<div>{getFieldValue('opseq')}</div>)}
                    </Form.Item>
                    <Form.Item label="訂單號碼">
                        {getFieldDecorator('orderNo', {})(<div>{getFieldValue('orderNo')}</div>)}
                    </Form.Item>
                    <Form.Item label="續訂狀態">

                        {getFieldDecorator('autopay')(
                            <Switch
                                checkedChildren="訂閱"
                                unCheckedChildren="取消"
                                checked={getFieldValue('autopay')}
                                onChange={this.onSubChange}
                            />  
                        )}
                        <div style={{color:'blue', display: subStatusMemo}}>
                            *手動取消續訂，視為停用當前訂單的模組權限，並修改到期日為今日 {moment().format('YYYY-MM-DD')}
                        </div>
                    </Form.Item>
                    <Form.Item label="購買日">
                        {getFieldDecorator('paySdate')(<div>{getFieldValue('paySdate')}</div>)}
                    </Form.Item>
                    {/* <Form.Item label="購買日">
                        {getFieldDecorator('paySdate', config)(<DatePicker disabled={subStatusMemo=="none"? false:true} />)}
                    </Form.Item> */}
                    <Form.Item label="到期日">
                        {getFieldDecorator('payEdate', config)(<DatePicker disabled={subStatusMemo=="none"? false:true} />)}
                        <div style={{color:'blue'}}>
                                *僅維護當筆訂單權限
                        </div>
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
            </div>
        )
    }
}