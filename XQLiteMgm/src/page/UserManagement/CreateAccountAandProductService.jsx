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

    

    /* ?????? ?????????????????? */
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
            callback('?????????????????????');
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
            callback('??????????????????????????????????????????');
        } else {
            callback();
        }
    };

    handleConfirmBlur = e => {
        const { value } = e.target;
        this.setState({ confirmDirty: this.state.confirmDirty || !!value });
    };
    /* ?????? ?????????????????? */

    
    
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
                            _common.ShowMessage("success", "???????????????");

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
                        _common.ShowMessage("error", "???????????????????????????????????????");
                        console.log(ex);
                    }
                },
                complete: (a) => {
                    this.setState({ loading: false });
                },
                error: err => {
                    this.setState({ loading: false });
                    _common.ShowMessage("error", "CreateAccountAndService ????????????");
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
                    _common.ShowMessage("error", "????????????????????????");
                    console.log(ex);
                }
            },
            complete: () => {
            },
            error: err => {
                _common.ShowMessage("error", "GetUserProductList ????????????");
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
                _common.ShowMessage("error", "?????????????????????????????????");
                break;
            
            case "IsValidAccountDataError":
                _common.ShowMessage("error", "???????????????????????????????????????");
                break;
            case "MoneyDJCheckAccountAPIError":
                _common.ShowMessage("error", "MoneyDJ ?????????????????????????????????????????????");
                break;
            case "MoneyDJCreateAccountAPIError":
                _common.ShowMessage("error", "MoneyDJ ?????????????????????????????????");
                break;
            case "CreateServiceError":
                _common.ShowMessage("error", "???????????????????????????????????????");
                break;
            case "SettingCreditCardDataError":
                _common.ShowMessage("error", "???????????????????????????????????????????????????");
                break;
            case "CreateOrderError":
                _common.ShowMessage("error", "??????????????????????????????????????????");
                break;
            case "ADDProductServiceError":
                _common.ShowMessage("error", "????????????????????????????????????????????????");
                break;
            case "AddDeclareError":
                _common.ShowMessage("error", "??????????????????????????????????????????????????????");
                break;
            case "ADDUserPointPayError":
                _common.ShowMessage("error", "????????????????????????????????????????????????");
                break;
            case "ADDUserExtraAuthError":
                _common.ShowMessage("error", "??????????????????????????????????????????");
                break;
            case "ADDUserExchAuthError":
                _common.ShowMessage("error", "??????????????????????????????????????????????????????");
                break;
            case "ADDNewsPowerError":
                _common.ShowMessage("error", "????????????????????????????????????????????????");
                break;
            case "AddWsPowerError":
                _common.ShowMessage("error", "AddWsPower??????????????????????????????");
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
                <Divider orientation="left">????????????</Divider>
                <Form.Item label="??????">
                    {getFieldDecorator('userid', {
                        rules: [
                            {
                                min: 6,
                                message: '????????????????????????',
                            },
                            {
                                max: 50,
                                message: '???????????????????????????50',
                            },
                            {
                                required: true,
                                message: '???????????????',
                            },
                        ],
                    })(<Input />)}
                </Form.Item>
                <Form.Item label="??????" hasFeedback extra="????????????????????????">
                    {getFieldDecorator('password', {
                        rules: [
                            {
                                min: 6,
                                message: '????????????????????????',
                            }, {
                                required: true,
                                message: '???????????????',
                            },
                            {
                                validator: this.validateToNextPassword,
                            },
                        ],
                    })(<Input.Password />)}
                </Form.Item>
                <Form.Item label="????????????" hasFeedback>
                    {getFieldDecorator('confirmpassword', {
                        rules: [
                            {
                                required: true,
                                message: '?????????????????????',
                            },
                            {
                                validator: this.compareToFirstPassword,
                            },
                        ],
                    })(<Input.Password onBlur={this.handleConfirmBlur} />)}
                </Form.Item>
                <Form.Item label="????????????">
                    {getFieldDecorator('mobile', {
                        rules: [
                            {
                                required: true,
                                message: '?????????????????????',
                            },
                            {
                                type: 'integer',
                                message: '?????????????????????',
                                transform: (value) => { return Number(value) }//????????? Number ???????????????????????????????????? Input ??????????????????????????????????????????????????? Number
                            },
                        ],
                    })(<Input />)}
                </Form.Item>
                <Form.Item label="????????????">
                    {getFieldDecorator('email', {
                        rules: [
                            {
                                required: true,
                                message: '?????????email',
                            },
                            {
                                type: "email",
                                message: "????????? E-Mail ????????????",
                                disabled: true
                            }
                        ],
                    })(<Input />)}
                </Form.Item>
                <Divider orientation="left">????????????</Divider>
                <Form.Item label="??????">
                    {getFieldDecorator('model', {
                    })(
                        <Checkbox.Group options={productSetList} >
                        </Checkbox.Group>
                    )}
                </Form.Item>
                <Form.Item label="?????????" >
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
                    <Divider orientation="left">????????????</Divider>
                    <Form.Item label="??????">
                        {getFieldDecorator('model', {
                        })(
                            <Checkbox.Group options={productSetList} >
                            </Checkbox.Group>
                        )}
                    </Form.Item>
                    <Form.Item label="?????????" >
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
                        <Button key="??????" onClick={this.onPropsCloseDlg}>
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

