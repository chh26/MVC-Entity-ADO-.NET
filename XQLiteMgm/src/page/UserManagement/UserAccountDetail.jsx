import React from 'react';
import { Table, Button, Modal } from 'antd';
import { SubProductsList } from './SubProductsList.jsx';

var _common = require('../../shared/script.js');

export class UserAccountDetail extends React.Component {
    constructor(props) {
        super(props);
        let demoData = {
            productList: [{ product: "XQ策略模組" }, { product: "XQ選股模組" }],
            brokerAccVerification: "加值服務用戶",
            freeTrial: {
                qualified: true,
                brokerName: "永豐證券",
                freeTrialList: [
                    {
                        account: "A123456987A123456987A123456987",
                        sDate: "2019/7/4A123456987A123456987A123456987",
                        eDate: "2019/8/3A123456987A123456987",
                        lastDate: "2019/8/1A123456987A123456987A123456987"
                    }
                ]
            },
            tradeAccountSettingList: [
                {
                    broker: "永豐證券",
                    settingAccountList: [
                        { account: "9A005662850" },
                        { account: "9A005661265" }
                    ]

                },
                {
                    broker: "康和證券",
                    settingAccountList: [
                        { account: "84501123658" }
                    ]
                },
                {
                    broker: "凱基證券",
                    settingAccountList: [
                        { account: "92009874565" }
                    ]
                }
            ],
            pointTradeAccList: [
                {
                    broker: "【永豐證券】",
                    accountList: [
                        { account: "9A005662850" },
                        { account: "9A005661265" }
                    ]

                },
                {
                    broker: "【康和證券】",
                    accountList: [
                        { account: "84501123658" }
                    ]
                }
            ]
        };

        this.StatisticsCols = [
            { title: "券商帳號", dataIndex: "account", align: 'center' },
            { title: "開始日", dataIndex: "sDate", align: 'center' },
            { title: "到期日", dataIndex: "eDate", align: 'center' },
            { title: "最後下單日", dataIndex: "lastDate", align: 'center' },
        ];

        this.PointBrokersDetailCols = [
            { title: "交易券商", dataIndex: "broker", align: 'center', width: 150 },
            { title: "交易帳號", dataIndex: "accountList", render: p => p.map(a => <span style={{ paddingRight: "15px" }} key={a.account}>{a.account}</span>) },
        ];

        this.state = {
            load: false,
            accountDetail: demoData,
            subProductsListVisible: false,
            isQueryProducts: false

        };

    }

    componentDidMount() {
        this.getUserAccountDetail();
    }

    componentDidUpdate(prevProps, prevState) {
        let { isQueryDetail } = this.props;
        if (isQueryDetail) {
            this.getUserAccountDetail();
        }
    }

    getUserAccountDetail = () => {
        this.setState({ load: true });
        let { userid } = this.props;
        
        $.ajax("../api/User/GetUserAccountDetail", {
            type: "POST",
            contentType: "application/json",
            processData: false,
            async: false,
            data: JSON.stringify({ userid: userid }),
            processData: false,
            success: jdata => {
                try {
                    this.setState({ userInfoDetail: jdata });
                    this.props.setBalanceAmount(jdata.balance.amount);
                } catch (ex) {
                    _common.ShowMessage("error", "帳號明細無法解析");
                    console.log(ex);
                }
            },
            complete: () => {
                this.setState({ load: false });
                this.props.changeIsQueryDetailState(false);
            },
            error: err => {
                _common.ShowMessage("error", "GetUserAccountDetail 無法查詢");
                console.log(err);
            }
        });
    }

    onSubProductsList = () =>{
        this.setState({ subProductsListVisible: true, isQueryProducts: true });
    }

    changeisQueryProducts = status => {
        this.setState({ isQueryProducts: status });
    }
    
    onCloseDlg = () =>{
        this.setState({ subProductsListVisible: false });
    }
    render() {
        let { load, accountDetail, userInfoDetail, subProductsListVisible, isQueryProducts } = this.state;
        let { userid, userinfo } = this.props;

        return (
            <div>
                {(userInfoDetail == null || userInfoDetail == undefined) ? (null) : (
                    <div>
                        <div >
                            <div className="UserInfoDetailTitle">
                                <span>帳號狀態</span>
                            </div>
                            <div className="line"></div>
                            <div className="UserInfoDetailItem">
                                <div className="Title"><span>訂閱模組</span></div>
                                <div className="Content">
                                    {userInfoDetail.productList.map(p => <span key={p.product} >【{p.product}】</span>)}
                                    <Button onClick={this.onSubProductsList}>查看 / 維護</Button>
                                </div>
                            </div>
                            <div className="UserInfoDetailItem">
                                <div className="Title"><span>券商帳號驗證</span></div>
                                <div className="Content"><span >{userInfoDetail.brokerAccVerification}</span></div>
                            </div>
                            <div className="UserInfoDetailItem">
                                <div className="Title"><span>美股贈送</span></div>
                                {(userInfoDetail.freeTrial.qualified)?
                                    ( (userInfoDetail.freeTrial.cardSEQNO == "888") ?
                                        <div className="Content"><span >符合。是{userInfoDetail.freeTrial.brokerName}首開戶。</span></div> :
                                        <div className="Content"><span >符合。宅在家活動。</span></div> 
                                    ):
                                    (<div className="Content"><span >不符合。</span></div>)
                                }

                            </div>
                            <div className="UserInfoDetailItem">
                                <div className="Title"><span> </span></div>
                                <div className="Content">
                                    <Table
                                        loading={load}
                                        style={{ minWidth: "500px",maxWidth: "998px" }}
                                        columns={this.StatisticsCols}
                                        dataSource={userInfoDetail.freeTrial.freeTrialList}
                                        rowKey={(r, j) => "mon." + j}
                                        // pagination={{ showSizeChanger: true, showQuickJumper: true }}
                                        pagination={false}
                                        size="small" />
                                </div>
                            </div>
                            <div className="UserInfoDetailItem">
                                <div className="Title" style={{ verticalAlign: "top" }}><span>交易帳號設定</span></div>
                                <div className="Content">{userInfoDetail.tradeAccountSettingList.map(t => <div key={"div" + t.broker}><span key={t.broker} >【{t.broker}】</span>{t.settingAccountList.map(a => <span key={a.account}>{a.account}</span>)}</div>)}</div>
                            </div>
                        </div>
                        <div>
                            <div className="UserInfoDetailTitle">
                                <span>XQ點數</span>
                            </div>
                            <div className="line"></div>
                            <div>
                                <Table
                                    loading={load}
                                    style={{ width: "100%" }}
                                    columns={this.PointBrokersDetailCols}
                                    dataSource={userInfoDetail.pointTradeAccList}
                                    rowKey={(r, j) => "mon." + j}
                                    // pagination={{ showSizeChanger: true, showQuickJumper: true }}
                                    pagination={false}
                                    size="small" />
                            </div>
                            <div className="UserInfoDetailItem">
                                <div style={{ display: "inline-block", height: "102px", verticalAlign: "middle" }}>
                                    <div className="Title"><span>XQ點數餘額</span></div>
                                </div>
                                <div style={{ display: "inline-block" }}>
                                    <div className="Content" style={{ borderBottom: "solid 1px", marginRight: "20px", width: "150px", height: "78px", textAlign: "center", fontSize: "-webkit-xxx-large" }}>
                                        <span>{_common.convertAmount(userInfoDetail.balance.amount.toString())}</span>
                                    </div>
                                    <div style={{ width: "150px", textAlign: "center" }}><span>(計算至{userInfoDetail.balance.date})</span></div>
                                </div>
                                <div style={{ display: "inline-block" }}>
                                    <div >
                                        <div className="Content"><div><span>上月結餘點數</span></div></div>
                                        <div className="Content">
                                            <div style={{ borderBottom: "solid 1px", marginRight: "20px", width: "100px", height: "28px", textAlign: "center" }}>
                                                <span>{_common.convertAmount(userInfoDetail.balance.accu.toString())}</span>
                                            </div>
                                        </div>
                                    </div>
                                    <div >
                                        <div className="Content"><div><span>本月新增點數</span></div></div>
                                        <div className="Content">
                                            <div style={{ borderBottom: "solid 1px", marginRight: "20px", width: "100px", height: "28px", textAlign: "center" }}>
                                                <span>{_common.convertAmount(userInfoDetail.balance.Added.toString())}</span>
                                            </div>
                                        </div>
                                    </div>
                                    <div style={{ display: "inline-block" }}>
                                        <div className="Content"><div><span>本用使用點數</span></div></div>
                                        <div className="Content">
                                            <div style={{ borderBottom: "solid 1px", marginRight: "20px", width: "100px", height: "28px", textAlign: "center" }}>
                                                <span>{_common.convertAmount(userInfoDetail.balance.used.toString())}</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                {/* <div className="Content">
                                <div ><span>共累積點數</span></div>
                                <div ><span>已使用點數</span></div>
                            </div>
                            <div className="Content">
                                <div className="Content" style={{borderBottom:"solid 1px" , marginLeft:"20px", width:"100px", textAlign:"center"}}><span>56</span></div>
                                <div className="Content" style={{borderBottom:"solid 1px" , marginLeft:"20px", width:"100px", textAlign:"center"}}><span>56</span></div>
                            </div> */}
                            </div>
                        </div>
                    </div>
                )}
                <div>
                    <Modal title="訂閱模組權限 查詢 / 維護" width="1200px" bodyStyle={{ height: "680px" }}
                        visible={subProductsListVisible}
                        onCancel={this.onCloseDlg}
                        footer={[<Button key="back" onClick={this.onCloseDlg}>關閉</Button>]}
                    >
                        <div>
                            <SubProductsList userid={userid} userinfo={userinfo} isQueryProducts={isQueryProducts} changeisQueryProducts={this.changeisQueryProducts} />
                        </div>
                    </Modal>
                </div>
            </div>
        );
    }
}