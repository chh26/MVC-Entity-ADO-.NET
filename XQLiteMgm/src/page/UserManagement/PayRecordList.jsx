import React from 'react';
import { Table, Modal, Form, Dropdown, Menu, Button} from 'antd';
import moment from 'moment';

var _common = require('../../shared/script.js');

export class PayRecordList extends React.Component{
    constructor(props){
        super(props);

        this.Cols = [
            { title: "持卡人姓名", dataIndex: "name", align:'center' ,
            sorter: (a, b) => { return a.name.localeCompare(b.name)}},
            { title: "地址", dataIndex: "address", align:'center' },
            { title: "訂單號碼", dataIndex: "cardSEQNO", align:'center' },
            { 
                title: "購買日", dataIndex: "paySdate", align:'center' ,
                sorter: (a, b) => moment(a.paySdate).unix() - moment(b.paySdate).unix(), 
                render: (t,r) => <div>{moment(t).format('YYYY-MM-DD hh:mm:ss')}</div>
            },
            { 
                title: "到期日", dataIndex: "payEdate", align:'center' ,
                sorter: (a, b) => moment(a.payEdate).unix() - moment(b.payEdate).unix(),
                defaultSortOrder:"descend", 
                render: (t,r) => <div>{moment(t).format('YYYY-MM-DD hh:mm:ss')}</div>
            },
            { title: "備註", dataIndex: "others", align:'center', render: (t,r) => <div >{t}</div> }
        ];

        this.state = {
            loading : false,
        };
    }

    componentDidMount() {
        let {isQuery} = this.props;
        if (!isQuery) {
            // console.log("componentDidMount");
            this.getUserPayRecordList();
        }
    }

    componentDidUpdate(prevProps, prevState, snapshot) {
        let {isQuery} = this.props;

        if (!isQuery) {
            // console.log("componentDidUpdate");
            this.getUserPayRecordList();
        } 
    }

    getUserPayRecordList = () =>{

        let {info} = this.props;

        this.setState({ loading: true });

        let param = {
            userid:info.userid,
            opid:info.opid
        };


        $.ajax("../api/Order/GetUserPayRecord", {
            type: "POST",
            contentType: "application/json",
            processData: false,
            async: false,
            data: JSON.stringify(param),
            processData: false,
            success: jdata => {
                try {
                    if (jdata.ResultStatus == 1){
                        this.setState({ rows: jdata.Result});
                    }
                    else {
                        _common.ShowMessage("error", "購買紀錄列查詢失敗");
                        console.log(jdata.Msg);
                    }
                } catch (ex) {
                    _common.ShowMessage("error", "購買紀錄列表無法解析");
                    console.log(ex);
                }
            },
            complete: () => {
                this.setState({ loading: false });
                this.props.changeIsQuery(true);
            },
            error: err => {
                _common.ShowMessage("error", "GetUserPayRecord 無法查詢");
                console.log(err);
            }
        });
    }

    render() {
        let { loading, rows = [] } = this.state;
        return(
            <div>
                <Table
                loading={loading}
                style={{ width: "1130px" }}
                columns={this.Cols}
                dataSource={rows}
                rowKey={(r, j) => "mon." + j}
                pagination={{ showSizeChanger: true, showQuickJumper: true }}
                size="small" />
            </div>
        );
    }
}