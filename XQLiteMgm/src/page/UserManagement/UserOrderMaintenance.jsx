import React from 'react';
import { Button, Input, Select } from 'antd';
import { SubProductsList } from './SubProductsList.jsx';

export class UserOrderMaintenance extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            type:"user",
            isQueryProducts: false

        };
    }

    onQueryTypeChange = e => {
        this.setState({ type: e });
    }

    onSubProductsList = () =>{
        this.setState({ isQueryProducts: true });
    }

    changeisQueryProducts = status => {
        this.setState({ isQueryProducts: status });
    }

    onInputChange = (e) => {
        this.setState({ userid: e.target.value });

    }

    render() {
        let { userid = '', isQueryProducts } = this.state;

        return (
            <div>
                <div>
                    <Input.Group compact>
                        <div style={{width:"110px", height:"32px", textAlign:"center",border:"1px solid #d9d9d9", borderTopLeftRadius:"4px", borderBottomLeftRadius:"4px"}}>
                            <div style={{paddingTop:"5px"}}>帳號</div>
                        </div>
                        <Input style={{ width: '230px' }} placeholder="請輸入查詢帳號" onChange={this.onInputChange} />
                        <Button type="primary" onClick={this.onSubProductsList} >查詢</Button>
                    </Input.Group>
                </div>
                <div>
                    <SubProductsList userid={userid} fuzzy={true} isQueryProducts={isQueryProducts} changeisQueryProducts={this.changeisQueryProducts} />
                </div>
            </div>
        )
    }
}