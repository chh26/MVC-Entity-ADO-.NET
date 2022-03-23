import React from 'react';
import { Table, message, Select } from 'antd';

var _common = require('../../shared/script.js');

export class UserPointRule extends React.Component {
    constructor(props){
        super(props);

        this.StatisticsCols = [
            { title: "交易金額", dataIndex: "TradeCost", align:'center', width:"282px", render: r => <div style={{  width: "142px",textAlign: "right"}}>{r}</div> },
            { title: "XQ點數", dataIndex: "point", align:'center', width:"282px", render: r => <div style={{  textAlign: "center"}}>{r}</div> },
            { title: "活動期間（起）", dataIndex: "sDate", align:'center', width:"282px", render: r => <div style={{  textAlign: "center"}}>{r}</div> },
            { title: "活動期間（迄）", dataIndex: "eDate", align:'center', width:"282px", render: r => <div style={{  textAlign: "center"}}>{r}</div> },
            { title: "說明", dataIndex: "memo", align:'center', width:"282px", render: r => <div style={{  textAlign: "center"}}>{r}</div> },
        ];

        this.state = {
            load:false,
            tsid: 'ALL'
        }
    }

    componentDidMount() {
        let {tsid} = this.state;
        this.onOptionChange(tsid);
    }

    componentWillReceiveProps(nextProps) {
        if (nextProps.userid !== this.props.userid) {
            this.onOptionChange('ALL');
        }
    }
      
    onOptionChange = (tsid) => {
        this.setState({tsid: tsid});

        $.ajax("../api/User/GetPointRuleInfo", {
            type: "POST",
            contentType: "application/json",
            processData: false,
            async: false,
            data: JSON.stringify({ tsid }),
            processData: false,
            success: jdata => {
                try {
                    let StatisticsRows = [];
                    jdata.map(r => StatisticsRows.push(r));
                    this.setState({ StatisticsRows });
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
        let {pointBrokerInfo} = this.props;

        let { load, StatisticsRows, tsid } = this.state;
        return(
            <div>
                <div style={{paddingBottom:"5px"}}>
                    <div style={{ display: "inline-block", paddingRight: "5px" }}>合作券商：</div>
                    <div style={{ display: "inline-block", paddingRight: "30px" }}>
                        <Select style={{ width: 135 }} value={tsid} onChange={this.onOptionChange}>
                            <Select.Option value="ALL">全部</Select.Option>
                            {pointBrokerInfo.map(item => {
                                return (<Select.Option value={item.TSID} key={item.TSID}>{item.TSName}</Select.Option>)
                            })}
                        </Select>
                    </div>
                </div>
                <div>
                    <Table
                        loading={load}
                        style={{ width: "1130px" }}
                        columns={this.StatisticsCols}
                        dataSource={StatisticsRows}
                        rowKey={(r, j) => "mon." + j}
                        pagination={{ showSizeChanger: true, showQuickJumper: true }}
                        size="small" />
                </div>
            </div>
        )
    }
}