import React from 'react';
import { Dropdown,Button, Input, Table, Spin, Modal, Select, Form, Menu } from 'antd';
import {UserDetailInfo} from './UserDetailInfo.jsx';
import {UserBaseInfo} from './UserBaseInfo.jsx';
import {AccountSetting} from './AccountSetting.jsx';
import {CreateAccountAandProductService} from './CreateAccountAandProductService.jsx';
import { SubProductsList } from './SubProductsList.jsx';

const UserBaseInfoForm = Form.create({ name: 'UserBaseInfo' })(UserBaseInfo);
const AccountSettingForm = Form.create({ name: 'AccountSetting' })(AccountSetting);
const CreateAccountAandProductServiceForm = Form.create({ name: 'AccountSetting' })(CreateAccountAandProductService);



var _common = require('../../shared/script.js');

export class UserInfoMaintenance extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            load: false,
            isQueryDetail: false,
            isQueryProducts:false
        };
        this._vs = {type:"user"};

        
        

        this.cols = [
            { title: "Name", dataIndex: "name" },
            {
                title: "UserID",
                dataIndex: "userid",
                render: (t, r) => "Y" === r.cardno5 ? 
                <span className="caution">{t}</span> : 
                <div style={{ color: "#00b0ff", cursor: "pointer"}} onClick={() => { this.openDetail(t,r) }}>
                    {t}
                </div>
            },
            { title: "Mobile", dataIndex: "mobile" },
            { title: "Email", dataIndex: "email" },
            {
                title: "帳號創建時間",
                dataIndex: "cratedate",
                render: t => {
                    try {
                        return t.replace("T", " ");
                    }
                    catch (ex) {
                        return null;
                    }
                }
            },
            { title: "經銷商代碼", dataIndex: "TSID" },
            { title: "操作", dataIndex: "123",
                render: (t, r) =>  
                <div>
                    <Dropdown overlay=
                                    {
                                        (
                                            <Menu>
                                            <Menu.Item>
                                                <div style={{ color: "#00b0ff", cursor: "pointer" }} onClick={() => { this.maintenBaseInfo(r) }}>
                                                    基本資料維護
                                                </div> 
                                            </Menu.Item>
                                            <Menu.Item>
                                                <div style={{ color: "#00b0ff", cursor: "pointer" }} onClick={() => { this.maintenAccountSetting(r)}}>
                                                    帳號停用/復權
                                                </div> 
                                            </Menu.Item>
                                            <Menu.Item>
                                                <div style={{ color: "#00b0ff", cursor: "pointer" }} onClick={() => { this.maintainPermission(r)}}>
                                                    權限查詢/維護
                                                </div> 
                                            </Menu.Item>
                                            </Menu>
                                        )
                                    } placement="bottomLeft">
                        <Button>維護</Button>
                    </Dropdown>
                </div>
                
                },
        ];
    }
    _go = async e => {
        $.extend(this._vs, { UserID: e.target.value, mydata: null })
    }
    _go2 = async () => {
        let { mydata = [] } = this._vs;
        let rows = [], filt = r => rows.push(r);

        mydata.map(r => filt(r));
        this.setState({ rows });
    }

    _go3 = async () => {
        let { lock, UserID, type } = this._vs;

        UserID = $.trim(UserID);
        UserID ? (
            lock || (
                this._vs.lock = true, // 以第一次查詢為主
                this.setState({ load: true }),
                $.ajax("../api/User/GetUserBaseInfo", {
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({ type: type, queryText: UserID }),
                    processData: false,
                    success: jdata => {
                        try {
                            let list = [];
                            jdata.map(r => list.push(r));
                            this._vs.mydata = list;
                            this._go2();
                        } catch (ex) {
                            _common.ShowMessage("error", "基本資料無法解析");
                            console.log(ex);
                        }
                    },
                    error: err => {
                        _common.ShowMessage("error", "GetUserBaseInfo 無法查詢");
                        console.log(err);
                    },
                    complete: () => {
                        this.setState({ load: false });
                        this._vs.lock = false;
                    }
                })
            )
        ) : _common.ShowMessage("warn", "請先輸入使用者ID以查詢");
    }
    _go4 = async () => {
        this._go3();
    }
    
    openDetail = (userid,r) => {
        this.setState({ detailVisible: true, userid, isQueryDetail: true, userBaseInfo: r });
    }

    maintenBaseInfo = (r) => {
        this.setState({infoMaintenVisible: true, userBaseInfo: r});
        this.setState({isValuesSetting:false});
    }

    maintenAccountSetting = (r) => {
        this.setState({accountSettingVisible: true, userid: r.name});
        this.setState({isAccountValuesSetting:false});
    }

    maintainPermission = (r) =>{
        this.setState({ subProductsListVisible: true, isQueryProducts: true,userBaseInfo: r, userid: r.name});

        // this.setState({createAccountVisible: true, userBaseInfo: r, accountDialogTitle:"新增權限"});
        this.setState({isValuesSetting:false});
    }

    onCreateAccount = () => {
        this.setState({createAccountVisible: true, userBaseInfo: null});
        this.setState({isValuesSetting:false});
    }

    onCloseDlg = (event) => {
        this.setState({ detailVisible: false });
        this.setState({ infoMaintenVisible: false });
        this.setState({ accountSettingVisible: false });
        this.setState({createAccountVisible: false});
        this.setState({ subProductsListVisible: false });

        if(event && event.update) {
            $.extend(this._vs, { UserID: event.userid })
            this._go3();
        }
    }

    changeIsQueryDetailState = state => {
        this.setState({ isQueryDetail: state });
    }

    onQueryTypeChange = e => {
        $.extend(this._vs, { type: e })
    }

    onValuesSetting = e =>{
        this.setState({isValuesSetting:true});
    }

    onAccountValuesSetting = e => {
        this.setState({isAccountValuesSetting:true});
    }

    onBaseInfoFormCloseDlg = (s) =>{
        this.onCloseDlg();
        if(s == "update") {
            this._go3();
        }
    }

    changeisQueryProducts = status => {
        this.setState({ isQueryProducts: status });
    }

    render() {
        let { rows = [], load, detailVisible = false, userid, isQueryDetail, infoMaintenVisible = false, userBaseInfo, isValuesSetting, accountSettingVisible = false, isAccountValuesSetting, createAccountVisible = false, isQueryProducts, subProductsListVisible } = this.state;
        let {type} = this._vs;
        return (
            <Spin spinning={load}>
                <div>
                    <div style={{display: "inline-block" }} >
                        <Input.Group compact>
                            <Select style={{ width: "80px" }} defaultValue={type} onChange={this.onQueryTypeChange}>
                                <Select.Option value="user">帳號</Select.Option>
                                <Select.Option value="mobile">電話</Select.Option>
                                <Select.Option value="email">E-Mail</Select.Option>
                            </Select>
                            <Input style={{ width: '230px' }} placeholder="請輸入查詢關鍵字" onChange={this._go} />
                            <Button type="primary" onClick={this._go4} >查詢</Button>
                        </Input.Group>
                    </div>
                    <div style={{ display: "inline-block", verticalAlign: "top" }} >
                        <Button type="primary" onClick={this.onCreateAccount} >新增帳號</Button>
                    </div>
                </div>
                <Table
                    columns={this.cols}
                    dataSource={rows}
                    rowKey={(r, j) => "info." + j}
                    pagination={{ showSizeChanger: true, showQuickJumper: true }}
                    size="small" />
                <Modal
                    title="客戶資料明細"
                    width="1200px"
                    visible={detailVisible}
                    onCancel={this.onCloseDlg}
                    footer={[<Button key="back" onClick={this.onCloseDlg}>關閉</Button>]}>
                    <UserDetailInfo userid={userid} userinfo={userBaseInfo} isQueryDetail={isQueryDetail} changeIsQueryDetailState={this.changeIsQueryDetailState} />
                </Modal>
                <Modal
                    title="基本資料維護"
                    width="400px"
                    visible={infoMaintenVisible}
                    onCancel={this.onCloseDlg}
                    footer="">
                        <UserBaseInfoForm userBaseInfo={userBaseInfo} onValuesSetting= {this.onValuesSetting} isValuesSetting={isValuesSetting} onCloseDlg={this.onBaseInfoFormCloseDlg}></UserBaseInfoForm>
                </Modal>
                <Modal
                    title="帳號停用/復權"
                    width="600px"
                    visible={accountSettingVisible}
                    onCancel={this.onCloseDlg}
                    footer="">
                        <AccountSettingForm userid={userid} onValuesSetting= {this.onAccountValuesSetting} isAccountValuesSetting={isAccountValuesSetting} onCloseDlg={this.onCloseDlg}></AccountSettingForm>
                </Modal>
                <Modal
                    title="新增帳號"
                    width="600px"
                    visible={createAccountVisible}
                    onCancel={this.onCloseDlg}
                    footer="">
                        <CreateAccountAandProductServiceForm userinfo={userBaseInfo} onValuesSetting= {this.onValuesSetting} isValuesSetting={isValuesSetting} onCloseDlg={this.onCloseDlg}></CreateAccountAandProductServiceForm>
                </Modal>
                <Modal title="訂閱模組權限 查詢 / 維護" width="1200px" bodyStyle={{ height: "680px" }}
                        visible={subProductsListVisible}
                        onCancel={this.onCloseDlg}
                        footer={[<Button key="back" onClick={this.onCloseDlg}>關閉</Button>]}
                    >
                        <div>
                            <SubProductsList userid={userid} userinfo={userBaseInfo} isQueryProducts={isQueryProducts} changeisQueryProducts={this.changeisQueryProducts} />
                        </div>
                </Modal>
            </Spin>
        );
    }
}
