import React from 'react';
import { Button, Spin, Form, Input, Divider, Checkbox, Row, Col, DatePicker } from 'antd';
import moment from 'moment';

var _common = require('../../shared/script.js');

export class CreateAccountAandProductService extends React.Component {
    constructor(porps) {
        super(porps);

        this.state = { loading: false };
    }

    componentDidMount() {
        let { isValuesSetting, userinfo } = this.props;

        if (!isValuesSetting) {

            if (!isValuesSetting) {
                if (userinfo == null) {
                    this.props.form.setFieldsValue({
                        userid: '',
                        password: '',
                        confirmpassword: '',
                        mobile: '',
                        email: '',
                        model: [],
                        expirationdate: moment()
                    });
                }
                else {
                    this.props.form.setFieldsValue({
                        model: [],
                        expirationdate: moment()
                    });
                }
                
    
                this.getProductList();
    
                this.props.onValuesSetting(true);
            }

            this.getProductList();

            this.props.onValuesSetting(true);
        }
    }

    componentDidUpdate(prevProps, prevState) {
        let { isValuesSetting, userinfo } = this.props;
        if (!isValuesSetting) {
            if (userinfo == null) {
                this.props.form.setFieldsValue({
                    userid: '',
                    password: '',
                    confirmpassword: '',
                    mobile: '',
                    email: '',
                    model: [],
                    expirationdate: moment()
                });
            }
            else {
                this.props.form.setFieldsValue({
                    model: [],
                    expirationdate: moment()
                });
            }
            

            this.getProductList();

            this.props.onValuesSetting(true);
        }
    }

    

    /* 驗證 ↓↓↓↓↓↓ */
    validateToNextPassword = (rule, value, callback) => {
        const { form } = this.props;
        if (value && this.state.confirmDirty) {
            form.validateFields(['confirm'], { force: true });
        }
        callback();
    };

    compareToFirstPassword = (rule, value, callback) => {
        const { form } = this.props;
        if (value && value !== form.getFieldValue('password')) {
            callback('密碼確認不相符');
        } else {
            callback();
        }
    };

    validateToRequired = (rule, value, callback) => {
        const { form } = this.props;
        let { productDefaultValueList } = this.state;
        let model = form.getFieldValue('model');
        let opidList = [];

        if (productDefaultValueList.length > 0) {
            if (model != []) {
                model.map(x => {
                    opidList.push(x);
                })
            }
            productDefaultValueList.map(x => {
                let index = opidList.indexOf(x);

                if (index > -1) {
                    opidList.splice(index, 1);
                }
            })
        }

        if (value == null && opidList.length > 0) {
            callback('如有新增模組，請選擇到期日。');
        } else {
            callback();
        }
    };

    handleConfirmBlur = e => {
        const { value } = e.target;
        this.setState({ confirmDirty: this.state.confirmDirty || !!value });
    };
    /* 驗證 ↑↑↑↑↑↑ */

    
    
    /* form */
    handleSubmit = e => {
        e.preventDefault();
        this.props.form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                
                // console.log('Received values of form1: ', values);

                this.add(values);
                // console.log('Received values of form: ', values);
            }
        });
    };

    add = (values) => {
        

        this.createAccountAndProduct(values)

    }

    createAccountAndProduct = (values) => {
        let { userinfo } = this.props;
        let { productDefaultValueList } = this.state;
        let opidList = [];

        if (values.model.length > 0) {

            values.model.map(x => {
                opidList.push(x);
            })
        }

        if (productDefaultValueList.length > 0) {

            productDefaultValueList.map(x => {
                let index = opidList.indexOf(x);

                if (index > -1) {
                    opidList.splice(index, 1);
                }
            })
        }

        let param = {
            userid: userinfo == null ? values.userid : userinfo.userid,
            pwd: values.password,
            mobile: userinfo == null ? values.mobile : userinfo.mobile,
            email:  userinfo == null ? values.email : userinfo.email,
            opidList: opidList,
            payEdate: values.expirationdate
        };

        let api = userinfo == null ? "CreateAccountAndService" : "CreateService";

        if (api == "CreateAccountAndService" || (api == "CreateService" && opidList.length > 0)) {
            this.setState({ loading: true });

            $.ajax("../api/CreateAcount/" + api, {
                type: "POST",
                data: JSON.stringify(param),
                contentType: "application/json",
                processData: false,
                success: data => {
                    try {
                        // console.log("CreateAccountAndServiceResult", data);
                        if (data.ResultStatus == 1) {
                            _common.ShowMessage("success", "新增成功。");

                            let resultParam = {
                                update: true,
                                userid: values.userid
                            };
                            this.props.onCloseDlg(resultParam);
                        }
                        else if (data.ResultStatus == 0) {
                            this.createResult(data);
                        }

                    } catch (ex) {
                        this.setState({ dialogLoading: false });
                        _common.ShowMessage("error", "新增帳號及模組權限無法解析");
                        console.log(ex);
                    }
                },
                complete: (a) => {
                    this.setState({ loading: false });
                },
                error: err => {
                    this.setState({ loading: false });
                    _common.ShowMessage("error", "CreateAccountAndService 發生錯誤");
                    console.log(err);
                }
            });
        }
        else
            this.onPropsCloseDlg();
    }

    getProductList = () => {
        let {userinfo} = this.props;

        $.ajax("../api/CreateAcount/GetUserProductList", {
            type: "POST",
            contentType: "application/json",
            processData: false,
            async: false,
            data: JSON.stringify({ userid: userinfo == null ? '' : userinfo.userid }),
            processData: false,
            success: jdata => {
                try {
                    let productSetList = [];
                    let productDefaultValueList = [];

                    jdata.map(p => {
                        productSetList.push({ label: p.PNAME, value: p.PID, style: "lineHeight:100px", disabled: p.disabled });
                        if (p.disabled)
                            productDefaultValueList.push(p.PID);
                    });
                    
                    if (productDefaultValueList != [])
                        this.props.form.setFieldsValue({ model: productDefaultValueList });

                    this.setState({ productSetList, productDefaultValueList });
                    // console.log('productDefaultValueList', productDefaultValueList);
                } catch (ex) {
                    _common.ShowMessage("error", "帳號明細無法解析");
                    console.log(ex);
                }
            },
            complete: () => {
            },
            error: err => {
                _common.ShowMessage("error", "GetUserProductList 無法查詢");
                console.log(err);
            }
        });
    }

    onPropsCloseDlg = () => {
        this.props.onCloseDlg();
    }

    createResult = (result) => {
        switch (result.Result) {
            case "CreateAccountError":
                _common.ShowMessage("error", "新增帳號時，發生錯誤。");
                break;
            
            case "IsValidAccountDataError":
                _common.ShowMessage("error", "帳號資料驗證時，發生錯誤。");
                break;
            case "MoneyDJCheckAccountAPIError":
                _common.ShowMessage("error", "MoneyDJ 確認帳號是否存在時，發生錯誤。");
                break;
            case "MoneyDJCreateAccountAPIError":
                _common.ShowMessage("error", "MoneyDJ 新增帳號時，發生錯誤。");
                break;
            case "CreateServiceError":
                _common.ShowMessage("error", "新增產品服務時，發生錯誤。");
                break;
            case "SettingCreditCardDataError":
                _common.ShowMessage("error", "準備刷卡資料時發生錯誤，發生錯誤。");
                break;
            case "CreateOrderError":
                _common.ShowMessage("error", "建立訂單發生錯誤，發生錯誤。");
                break;
            case "ADDProductServiceError":
                _common.ShowMessage("error", "新增模組權限發生錯誤，發生錯誤。");
                break;
            case "AddDeclareError":
                _common.ShowMessage("error", "新增海外申報系統發生錯誤，發生錯誤。");
                break;
            case "ADDUserPointPayError":
                _common.ShowMessage("error", "新增點數資訊發生錯誤，發生錯誤。");
                break;
            case "ADDUserExtraAuthError":
                _common.ShowMessage("error", "新增權限發生錯誤，發生錯誤。");
                break;
            case "ADDUserExchAuthError":
                _common.ShowMessage("error", "新增行情模組權限發生錯誤，發生錯誤。");
                break;
            case "ADDNewsPowerError":
                _common.ShowMessage("error", "新增新聞權限發生錯誤，發生錯誤。");
                break;
            case "AddWsPowerError":
                _common.ShowMessage("error", "AddWsPower發生錯誤，發生錯誤。");
                break;
            default:
                _common.ShowMessage("error", result.Msg);
        }
    }

    render() {
        let {userinfo} = this.props;
        let { productSetList, loading  } = this.state;
        const { getFieldDecorator } = this.props.form;

        const formItemLayout = {
            labelCol: {
                xs: { span: 24 },
                sm: { span: 6 },
            },
            wrapperCol: {
                xs: { span: 1 },
                sm: { span: 12 },
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

        let field = userinfo == null ? (
            <div key='accountAndProduct'>
                <Divider orientation="left">帳號資訊</Divider>
                <Form.Item label="帳號">
                    {getFieldDecorator('userid', {
                        rules: [
                            {
                                min: 6,
                                message: '不可以小於六個字',
                            },
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
                    {getFieldDecorator('confirmpassword', {
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
                <Form.Item label="行動電話">
                    {getFieldDecorator('mobile', {
                        rules: [
                            {
                                required: true,
                                message: '請輸入行動電話',
                            },
                            {
                                type: 'integer',
                                message: '請輸入數字格式',
                                transform: (value) => { return Number(value) }//不轉成 Number 的話，驗證無法通過，因為 Input 會當成你是輸入字串，所以要手動轉成 Number
                            },
                        ],
                    })(<Input />)}
                </Form.Item>
                <Form.Item label="電子信箱">
                    {getFieldDecorator('email', {
                        rules: [
                            {
                                required: true,
                                message: '請輸入email',
                            },
                            {
                                type: "email",
                                message: "請輸入 E-Mail 正確格式",
                                disabled: true
                            }
                        ],
                    })(<Input />)}
                </Form.Item>
                <Divider orientation="left">模組權限</Divider>
                <Form.Item label="模組">
                    {getFieldDecorator('model', {
                    })(
                        <Checkbox.Group options={productSetList} >
                        </Checkbox.Group>
                    )}
                </Form.Item>
                <Form.Item label="到期日" >
                    {getFieldDecorator('expirationdate', {
                        rules: [
                            , {
                                validator: this.validateToRequired,
                            }
                        ],
                    })(<DatePicker />)}
                </Form.Item>
            </div>
        ) : (
                <div key='product'>
                    <Divider orientation="left">模組權限</Divider>
                    <Form.Item label="模組">
                        {getFieldDecorator('model', {
                        })(
                            <Checkbox.Group options={productSetList} >
                            </Checkbox.Group>
                        )}
                    </Form.Item>
                    <Form.Item label="到期日" >
                        {getFieldDecorator('expirationdate', {
                            rules: [
                                , {
                                    validator: this.validateToRequired,
                                }
                            ],
                        })(<DatePicker />)}
                    </Form.Item>
                </div>
            );

        return (
            <Spin spinning={loading}>
                <Form {...formItemLayout} onSubmit={this.handleSubmit} >
                    {field}
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
            </Spin>
        )
    }
}

