import React from 'react';
import { Table, Select, Button, message } from 'antd';

var _common = require('../../shared/script.js');

export class UserPointDetail extends React.Component {
    constructor(props) {
        super(props);
        this.historyCols = [
            { title: "日期", dataIndex: "date", align:'center' },
            { title: "來源", dataIndex: "source", align:'center', width:"500px"
              , render: r => {

                return <div style={{paddingLeft:"100px",textAlign:"left"}}>{r}</div>
              }
            },
            { title: "預估點數", dataIndex: "reword", align:'center', render: r => _common.convertAmount(r.toString()) },
            { title: "兌換點數", dataIndex: "redeem", align:'center', render: r => _common.convertAmount(r.toString()) },
            // { title: "說明", dataIndex: "desc", align:'center' },
            // { title: "訂單編號", dataIndex: "order", align:'center' }
        ];

        this.historyRows = [
            {
                date: "2019-09-15",
                source: "加值模組抵扣",
                reword: 0,
                redeem: 50,
                desc: "訂購［策略雷達］",
                order: "100018549"
            },
            {
                date: "2019-09-05",
                source: "［永豐證券］8月份交易64,875元",
                reword: 12,
                redeem: 0,
                desc: "交易贈點",
                order: ""
            }
        ];

        this.state = 
        {
            tsid:'ALL',
            interval: '1',
            reword:0,
            redeem:0,
            historyRows: []
        }
    }

    componentDidUpdate(prevProps,prevState) {
        if(prevProps.userid != this.props.userid){
            this.setState({
                tsid:'ALL',
                interval: '1',
                reword:0,
                redeem:0,
                historyRows: []
            });
        }
    }

    onBrokerChange = (tsid) => {
        this.setState({tsid});
    }

    onIntervalChange = (interval) => {
        this.setState({interval});
    }

    onQueryClick = () => {
        let {userid} = this.props;
        let { tsid, interval } = this.state;

        $.ajax("../api/User/GetIntervalPointInfo", {
            type: "POST",
            contentType: "application/json",
            processData: false,
            async: false,
            data: JSON.stringify({userid, tsid, interval }),
            success: jdata => {
                try {
                    this.setState({ reword: jdata.reword });
                    this.setState({ redeem: jdata.redeem });

                } catch (ex) {
                    _common.ShowMessage("error", "查詢區間點數資訊無法解析");
                    console.log(ex);
                    this.setState({ load: false });
                }
            },
            complete: () => {
                this.setState({ load: false });
            },
            error: err => {
                _common.ShowMessage("error", "GetIntervalPointInfo 無法查詢");
                console.log(err);
                this.setState({ load: false });
            }
        });

        $.ajax("../api/User/GetUserPointHistory", {
            type: "POST",
            contentType: "application/json",
            processData: false,
            async: false,
            data: JSON.stringify({ userid,tsid,interval }),
            success: jdata => {
                try {
                    let historyRows = [];
                    jdata.map(r => historyRows.push(r));
                    this.setState({ historyRows });
                } catch (ex) {
                    _common.ShowMessage("error", "點數資訊無法解析");
                    console.log(ex);
                    this.setState({ load: false });
                }
            },
            complete: () => {
                this.setState({ load: false });
            },
            error: err => {
                _common.ShowMessage("error", "GetPointInfo 無法查詢");
                console.log(err);
                this.setState({ load: false });
            }
        });
    }

    render() {
        let {userid,balanceAmount,pointBrokerInfo} = this.props;
        let { historyRows, reword, redeem } = this.state;
        return(
            <div>
                <div>
                    <div style={{display:"inline-block"}}>
                        <div style={{display:"inline-block", paddingRight:"5px", paddingBottom:"5px"}}>客戶帳號：</div>
                        <div style={{display:"inline-block", paddingRight:"30px"}}>{userid}</div>
                    </div>
                    <div style={{display:"inline-block"}}>
                        <div style={{display:"inline-block", paddingRight:"5px"}}>XQ點數餘額：</div>
                        <div style={{display:"inline-block"}}>{_common.convertAmount(balanceAmount.toString())}</div>
                    </div>
                </div>
                <div>
                    <div style={{display:"inline-block"}}>
                        <div style={{display:"inline-block", paddingRight:"5px"}}>指定券商：</div>
                        <div style={{display:"inline-block", paddingRight:"30px"}}>
                            <Select style={{ width: 135 }} defaultValue="ALL" onChange={this.onBrokerChange}>
                                <Select.Option value="ALL">全部</Select.Option>
                                {pointBrokerInfo.map(item => {
                                    return (<Select.Option value={item.TSID} key={item.TSID}>{item.TSName}</Select.Option>)
                                })}
                            </Select>
                        </div>
                        
                    </div>
                    <div style={{display:"inline-block"}}>
                        <div style={{display:"inline-block", paddingRight:"5px"}}>查詢日期：</div>
                        <div style={{display:"inline-block", paddingRight:"10px"}}>
                            <Select style={{ width: 150 }} defaultValue="1" onChange={this.onIntervalChange}>
                                <Select.Option value="ALL">全部</Select.Option>
                                <Select.Option value="1">最近 一年</Select.Option>
                                <Select.Option value="2">最近 二年</Select.Option>
                                <Select.Option value="3">最近 三年</Select.Option>
                            </Select>
                        </div>
                    </div>
                    <div style={{display:"inline-block", paddingRight:"5px"}}>
                        <Button type="primary" onClick={this.onQueryClick}>查詢</Button>
                    </div>
                </div>
                <div>
                    <div style={{margin:"10px 0px"}}>
                        <div style={{display:"inline-block"}}>查詢區間內，預估點數總計</div>
                        <div style={{display:"inline-block"}}>
                            <div style={{borderBottom:"solid 1px", display:"inline-block", height: "19px"}}>{_common.convertAmount(reword.toString())}</div>
                            <div style={{display:"inline-block", height: "19px"}}>點</div>
                        </div>
                        <div style={{display:"inline-block"}}>，使用點數</div>
                        <div style={{display:"inline-block"}}>
                            <div style={{borderBottom:"solid 1px", display:"inline-block", height: "19px"}}>{_common.convertAmount(redeem.toString())}</div>
                            <div style={{display:"inline-block", height: "19px"}}>點</div>
                        </div>
                        <div style={{display:"inline-block"}}>。</div>
                    </div>
                </div>
                <div>
                <Table
                    // loading ={load}                                 
                    style={{ width: "100%" }}
                    columns={this.historyCols}
                    dataSource={historyRows}
                    rowKey={(r, j) => "mon." + j}
                    pagination={{ showSizeChanger: true, showQuickJumper: true }}
                    // pagination={false}
                    size="small" />
                </div>
            </div>
        );
    }
}