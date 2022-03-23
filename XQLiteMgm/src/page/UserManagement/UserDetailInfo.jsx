import React from 'react';
import { Tabs, message, Button, Modal } from 'antd';
import { UserAccountDetail } from './UserAccountDetail.jsx';
import {UserPointDetail} from './UserPointDetail.jsx';
import { UserPointRule } from './UserPointRule.jsx';

var _common = require('../../shared/script.js');

export class UserDetailInfo extends React.Component {
    constructor(props) {

        super(props);
        this.state = {
            tab: "accountDetailTab",
            xqPointDetailDialogVisible: false,
            xqPointRuleDialogVisible: false
        };
    }

    componentDidMount() {
        this.getPointBrokerInfo();
    }

    componentDidUpdate(prevProps, prevState) {
        if (prevProps != this.props) {
            this.setState({ tab: "accountDetailTab" });
        }
    }

    //tab 切換
    onTabsChange = e => {
        this.setState({ tab: e });
    }

    changeIsQueryDetailState = state => {
        this.props.changeIsQueryDetailState(state);
    }

    setBalanceAmount = balanceAmount => {
        this.setState({ balanceAmount });
    }

    getPointBrokerInfo = () => {
        this.setState({ load: true });

        $.ajax("../api/User/GetPointBrokerInfo", {
            type: "GET",
            contentType: "application/json",
            processData: false,
            success: jdata => {
                try {
                    this.setState({ pointBrokerInfo: jdata });
                } catch (ex) {
                    _common.ShowMessage("error", "點數券商資訊無法解析");
                    console.log(ex);
                }
            },
            complete: () => {
                this.setState({ load: false });
                this.props.changeIsQueryDetailState(false);
            },
            error: err => {
                _common.ShowMessage("error", "GetPointBrokerInfo 無法查詢");
                console.log(err);
            }
        });
    }

    onXQPointDetail = () => {
        this.setState({ xqPointDetailDialogVisible: true });
    }

    onXQPointDetailCloseDlg = () => {
        this.setState({ xqPointDetailDialogVisible: false });
    }

    onXQPointRule = () => {
        this.setState({ xqPointRuleDialogVisible: true });
    }

    onXQPointRuleCloseDlg = () => {
        this.setState({ xqPointRuleDialogVisible: false });
    }

    render() {
        let { userid, userinfo, isQueryDetail } = this.props;
        let { tab, pointBrokerInfo, balanceAmount, xqPointDetailDialogVisible, xqPointRuleDialogVisible } = this.state;
        return (
            <div>
                <div>
                    <UserAccountDetail
                        userid={userid}
                        userinfo={userinfo}
                        isQueryDetail={isQueryDetail}
                        changeIsQueryDetailState={this.changeIsQueryDetailState}
                        setBalanceAmount={this.setBalanceAmount} />
                </div>
                <div style={{ textAlign: "center" }}>
                    <div style={{ display: "inline-block", paddingRight: "15px", paddingTop: "15px" }}>
                        <Button onClick={this.onXQPointDetail}>用戶XQ點數明細查詢</Button>
                    </div>
                    <div style={{ display: "inline-block" }}>
                        <Button onClick={this.onXQPointRule}>合作券商點數計算基準查詢</Button>
                    </div>
                </div>
                <div>
                    <Modal title="客戶XQ點數明細查詢" width="1200px" bodyStyle={{ height: "600px" }}
                        visible={xqPointDetailDialogVisible}
                        onCancel={this.onXQPointDetailCloseDlg}
                        footer={[<Button key="back" onClick={this.onXQPointDetailCloseDlg}>關閉</Button>]}
                    >
                        <div>
                            <UserPointDetail
                                userid={userid}
                                pointBrokerInfo={pointBrokerInfo}
                                balanceAmount={balanceAmount}
                            />
                        </div>
                    </Modal>
                    <Modal title="XQ點數合作券商, 點數計算基準" width="1200px" bodyStyle={{ height: "550px" }}
                        visible={xqPointRuleDialogVisible}
                        onCancel={this.onXQPointRuleCloseDlg}
                        footer={[<Button key="back" onClick={this.onXQPointRuleCloseDlg}>關閉</Button>]}
                    >
                        <div>
                            <UserPointRule
                                userid={userid}
                                pointBrokerInfo={pointBrokerInfo}
                            />
                        </div>
                    </Modal>
                </div>
            </div>
        )
    }
}