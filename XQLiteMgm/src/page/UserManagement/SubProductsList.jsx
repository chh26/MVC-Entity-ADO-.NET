import React from 'react';
import { Table, Modal, Form, Dropdown, Menu, Button} from 'antd';
import {MaintenPermission} from './MaintenPermission.jsx';
import {MaintenAddress} from './MaintenAddress.jsx';
import {PayRecordList} from './PayRecordList.jsx';
import {CreateAccountAandProductService} from './CreateAccountAandProductService.jsx';

import moment from 'moment';

const MaintenPermissionForm = Form.create({ name: 'MaintenPermission' })(MaintenPermission);
const MaintenAddressForm = Form.create({ name: 'MaintenAddress' })(MaintenAddress);
const CreateAccountAandProductServiceForm = Form.create({ name: 'AccountSetting' })(CreateAccountAandProductService);


var _common = require('../../shared/script.js');

export class SubProductsList extends React.Component{
    constructor(props){
        super(props);

        if (this.props.fuzzy){
            this.Cols = [
                {
                    title: "UserID", dataIndex: "userid", align: 'center',
                    sorter: (a, b) => { return a.userid.localeCompare(b.userid) }
                },
                { title: "模組", dataIndex: "opseq", align: 'center' },
                {
                    title: "續訂狀態",
                    dataIndex: "autopay",
                    align: 'center',
                    width:115,
                    render: (t, r) => this.parserAutopay(t.toString(), r)
                },
                { title: "訂單號碼", dataIndex: "orderNo", align: 'center' },
                {
                    title: "購買日", dataIndex: "paySdate", align: 'center',
                    sorter: (a, b) => moment(a.paySdate).unix() - moment(b.paySdate).unix()
                },
                {
                    title: "到期日", dataIndex: "payEdate", align: 'center',
                    sorter: (a, b) => moment(a.payEdate).unix() - moment(b.payEdate).unix(),
                    defaultSortOrder: "descend"
                },
                {
                    title: "取消訂閱日", dataIndex: "payCdate", align: 'center',
                    sorter: (a, b) => moment(a.payCdate).unix() - moment(b.payCdate).unix()
                },
                { title: "備註", dataIndex: "others", align: 'center', render: (t, r) => <div style={{ textAlign: "left" }}>{t}</div> },
                {
                    title: "操作", dataIndex: "123",
                    render: (t, r) => this.maintainEvent(r)
                }
            ];
        }
        else {
            this.Cols = [
                { title: "模組", dataIndex: "opseq", align: 'center' },
                {
                    title: "續訂狀態",
                    dataIndex: "autopay",
                    align: 'center',
                    render: (t, r) => this.parserAutopay(t.toString(), r)
                },
                { title: "訂單號碼", dataIndex: "orderNo", align: 'center' },
                {
                    title: "購買日", dataIndex: "paySdate", align: 'center',
                    sorter: (a, b) => moment(a.paySdate).unix() - moment(b.paySdate).unix()
                },
                {
                    title: "到期日", dataIndex: "payEdate", align: 'center',
                    sorter: (a, b) => moment(a.payEdate).unix() - moment(b.payEdate).unix(),
                    defaultSortOrder: "descend"
                },
                {
                    title: "取消訂閱日", dataIndex: "payCdate", align: 'center',
                    sorter: (a, b) => moment(a.payCdate).unix() - moment(b.payCdate).unix()
                },
                { title: "備註", dataIndex: "others", align: 'center', render: (t, r) => <div style={{ textAlign: "left" }}>{t}</div> },
                {
                    title: "操作", dataIndex: "123",
                    render: (t, r) => this.maintainEvent(r)
                },
            ];
        }

        this.state = {
            loading : false,
            permissionVisible : false,
            maintenAddressVisible : false,
            payRecordVisible : false
        };
    }

    componentDidMount() {
        let {isQueryProducts} = this.props;
        if (isQueryProducts) {
            // console.log("componentDidMount");
            this.getUserSubProductsList();
        }
    }

    componentDidUpdate(prevProps, prevState, snapshot) {
        let {isQueryProducts} = this.props;

        if (isQueryProducts) {
            // console.log("componentDidUpdate");
            this.getUserSubProductsList();
        } 
    }

    getUserSubProductsList = () =>{
        let {userid, fuzzy} = this.props;

        // console.log(userid);
        this.setState({ loading: true });

        let param = {
            userid,fuzzy
        };

debugger
        $.ajax("../api/User/GetUserSubProducts", {
            type: "POST",
            contentType: "application/json",
            processData: false,
            async: false,
            data: JSON.stringify(param),
            processData: false,
            success: jdata => {
                try {
                    this.setState({ rows: jdata });
                    // console.log(jdata);
                } catch (ex) {
                    _common.ShowMessage("error", "訂閱模組列表無法解析");
                    console.log(ex);
                }
            },
            complete: () => {
                this.setState({ loading: false });
                this.props.changeisQueryProducts(false);
            },
            error: err => {
                _common.ShowMessage("error", "GetUserSubProducts 無法查詢");
                console.log(err);
            }
        });
    }

    parserAutopay(autopay, product){
        // console.log(product);
        let today = moment(new Date());//今天
        let payEdate = moment(product.payEdate.toString());//今天
        let color = '';
        let payText = '';
        switch(autopay.toLowerCase()){
            case "true":
                return (<div style={{color:'#32CD32'}}>訂閱中</div>);
            case "sys":
            case "market":
                if (today.isAfter(payEdate)){
                    color = '#FF0000';
                    payText = '取消訂閱'
                }
                else {
                    color = '#32CD32';
                    payText = '訂閱中'
                }
            return (<div style={{color:color}}>{payText}<br/>（由後台新增）</div>);
            case "freetrial":
                if (today.isAfter(payEdate)){
                    color = '#FF0000';
                    payText = '取消訂閱'
                }
                else {
                    color = '#096dd9';
                    payText = product.opid == 'CBOTD'? '簽署贈送' : '美股贈送';
                }
            return (<div style={{color:color}}>{payText}</div>);
            case "false":
                return (<div style={{color:'#FF0000'}}>取消訂閱</div>);
        }
    }

    maintainEvent(product) {
        let today = moment(new Date());//今天
        let payEdate = moment(product.payEdate.toString());//今天
        let isExpiration = today.isAfter(payEdate);

        if (product.autopay.toString() == "true" ){//由後台新增的模組先不考慮，有需要再做|| (product.autopay.toString().toLowerCase() == "sys" && !isExpiration) || (product.autopay.toString().toLowerCase() == "market" && !isExpiration)) {
            return (
                <div style={{ color: "#00b0ff", cursor: "pointer" }} >
                    <Dropdown overlay=
                        {
                            (
                            <Menu>
                                <Menu.Item>
                                    <div style={{ color: "#00b0ff", cursor: "pointer" }} onClick={() => { this.payRecord(product) }}>
                                        購買明細查詢
                                                    </div>
                                </Menu.Item>
                                <Menu.Item>
                                    <div style={{ color: "#00b0ff", cursor: "pointer" }} onClick={() => { this.maintainPermission(product) }}>
                                        權限查詢/維護
                                                    </div>
                                </Menu.Item>
                                <Menu.Item>
                                    <div style={{ color: "#00b0ff", cursor: "pointer" }} onClick={() => { this.maintenAddress(product) }}>
                                        地址修改
                                                    </div>
                                </Menu.Item>
                            </Menu>
                            )
                        } placement="bottomLeft">
                        <Button>維護</Button>
                    </Dropdown>
                </div>
            );
        }
        else {
            return (
                <div style={{ color: "#00b0ff", cursor: "pointer" }} >
                    <Dropdown overlay=
                        {
                            (
                            <Menu>
                                <Menu.Item>
                                    <div style={{ color: "#00b0ff", cursor: "pointer" }} onClick={() => { this.payRecord(product) }}>
                                        購買明細查詢
                                                    </div>
                                </Menu.Item>
                            </Menu>
                            )
                        } placement="bottomLeft">
                        <Button>維護</Button>
                    </Dropdown>
                </div>
            );
        }
    }

    maintainPermission = (data) =>{
        this.setState({permissionVisible : true, permissionInfo: data, isValuesSetting:false});
    }

    maintenAddress = (data) => {
        this.setState({maintenAddressVisible: true, permissionInfo: data, isValuesSetting:false});
    }

    payRecord = (data) => {
        this.setState({payRecordVisible: true, permissionInfo: data, isQuery:false});
    }

    addPermission = () =>{
        this.setState({createAccountVisible: true});
        this.setState({isValuesSetting:false});
    }

    onCloseDlg = () => {
        this.setState({permissionVisible : false});
        this.setState({maintenAddressVisible: false});
        this.setState({payRecordVisible : false});
    }

    onValuesSetting = e =>{
        this.setState({isValuesSetting:true});
    }

    changeIsQuery = e =>{
        this.setState({isQuery:true});
    }

    onFormCloseDlg = (s) =>{
        this.onCloseDlg();
        if(s == "update") {
            let {userid} = this.props;

            this.getUserSubProductsList(userid);
        }
    }

    onCreateServiceCloseDlg = (event) => {
        this.setState({createAccountVisible: false});

        if(event && event.update) {
            this.getUserSubProductsList(event.userid);
        }
    }

    render() {
        let {userinfo = null} = this.props;

        let { loading, rows, permissionVisible, permissionInfo, isValuesSetting, maintenAddressVisible, payRecordVisible, isQuery = false, createAccountVisible = false } = this.state;

        return(
            <div>
                {
                    (userinfo) ? (
                        <div style={{ display: "inline-block", verticalAlign: "top", paddingBottom:"5px" }} >
                            <Button type="primary" onClick={this.addPermission} >新增權限</Button>
                        </div>
                    ) : (null)
                }
                <Table
                loading={loading}
                style={{ width: "1130px" }}
                columns={this.Cols}
                dataSource={rows}
                rowKey={(r, j) => "mon." + j}
                pagination={{ showSizeChanger: true, showQuickJumper: true }}
                size="small" />
                <Modal
                    title="模組權限維護"
                    width="500px"
                    visible={permissionVisible}
                    onCancel={this.onCloseDlg}
                    footer="">
                        <MaintenPermissionForm permissionInfo={permissionInfo} isValuesSetting={isValuesSetting} onValuesSetting={this.onValuesSetting} onCloseDlg={this.onFormCloseDlg}></MaintenPermissionForm>
                </Modal>
                <Modal
                    title="地址修改"
                    width="500px"
                    visible={maintenAddressVisible}
                    onCancel={this.onCloseDlg}
                    footer="">
                        <MaintenAddressForm info={permissionInfo} isValuesSetting={isValuesSetting} onValuesSetting={this.onValuesSetting} onCloseDlg={this.onFormCloseDlg}></MaintenAddressForm>
                </Modal>
                <Modal
                    title="購買紀錄"
                    width="1200px"
                    visible={payRecordVisible}
                    onCancel={this.onCloseDlg}
                    footer="">
                        <PayRecordList info={permissionInfo} isQuery={isQuery} changeIsQuery={this.changeIsQuery} ></PayRecordList>
                </Modal>
                <Modal
                    title="新增權限"
                    width="600px"
                    visible={createAccountVisible}
                    onCancel={this.onCreateServiceCloseDlg}
                    footer="">
                        <CreateAccountAandProductServiceForm userinfo={userinfo} onValuesSetting= {this.onValuesSetting} isValuesSetting={isValuesSetting} onCloseDlg={this.onCreateServiceCloseDlg}></CreateAccountAandProductServiceForm>
                </Modal>
            </div>
        );
    }
}