import React from 'react';
import {Form, Button, DatePicker, Switch, Icon,Input } from 'antd';
import moment from 'moment';
var _common = require('../../shared/script.js');

export class MaintenAddress extends React.Component {
    constructor(porps){
        super(porps);

        this.state = {
            loading:false
        };
    }

    componentDidMount() {
        let { info, isValuesSetting } = this.props;

        if (!isValuesSetting) {
            this.props.form.setFieldsValue({
                zipcode: info.zipcode,
                address: info.address,
            });

            this.props.onValuesSetting(true);
        }
    }

    componentDidUpdate(prevProps, prevState) {
        let { info, isValuesSetting } = this.props;

        if (!isValuesSetting) {
            this.props.form.setFieldsValue({
                zipcode: info.zipcode,
                address: info.address,
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
        let {info} = this.props;
        let param = info;
        param.zipcode = fromValues.zipcode;
        param.address = fromValues.address;
        
        // console.log('Update param of form: ', param);

        this.setState({ loading: true });

        $.ajax("../api/User/UserPayRecord_UPDAddress", {
            type: "POST",
            data: JSON.stringify(param),
            contentType: "application/json",
            processData: false,
            success: data => {
                try {
                    // console.log("UserPayRecord_UPDAddressReturnData:", data);

                    this.alertUpdateResult(data.ResultStatus);

                } catch (ex) {
                    _common.ShowMessage("error", "維護訂單地址無法解析");
                    this.setState({ loading: false });
                    console.log(ex);
                }
            },
            complete: () => {
            },
            error: err => {
                _common.ShowMessage("error", "UserPayRecord_UPDAddress 無法查詢");
                this.setState({ loading: false });
                console.log(err);
            }
        });
    }

    alertUpdateResult = (ResultStatus) => {
        this.setState({ loading: false });
        switch (ResultStatus) {
            case 1:
                _common.ShowMessage("success", "更新成功。");
                this.onCloseDlg("");
                break;
            case 0:
                _common.ShowMessage("error", "更新失敗。");
        }
    }

    onCloseDlg = (s) => {
        this.props.onCloseDlg(s);
    }

    render() {
        let {info} = this.props;
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
                    <Form.Item label="郵遞區號">
                        {getFieldDecorator('zipcode', {rules: [{required: true, message: "請正確填寫發票寄送的郵遞區號"}]})(<Input />)}
                    </Form.Item>
                    <Form.Item label="地址">
                        {getFieldDecorator('address', {rules: [{required: true, message: "請填寫發票寄送的地址"}]})(<Input />)}
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