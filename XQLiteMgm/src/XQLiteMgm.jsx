import React from 'react';
import ReactDOM from 'react-dom';
import {Form} from 'antd';
import { SysAccountMaintenance } from './page/SystemManagement/SysAccountMaintenance.jsx';
import { UserInfoMaintenance } from './page/UserManagement/UserInfoMaintenance.jsx';
import { UserOrderMaintenance } from './page/UserManagement/UserOrderMaintenance.jsx';
import { UserActionMsg } from './page/UserManagement/UserActionMsg.jsx';
import { UserLogQuery } from './page/UserManagement/UserLogQuery.jsx'

import './shared/css/style.css';

const SysAccountMaintenanceForm = Form.create({ name: 'SysAccountMaintenance' })(SysAccountMaintenance);
const UserInfoMaintenanceForm = Form.create({ name: 'UserInfoMaintenance' })(UserInfoMaintenance);



class XQLiteMgmPage extends React.Component {
    render() {
        let {pid} = this.props;
        switch (pid) {
            case "SysAccountMaintenance":
                return <SysAccountMaintenanceForm ></SysAccountMaintenanceForm>
            case "UserInfoMaintenance":
                return <UserInfoMaintenance ></UserInfoMaintenance>
            case "UserOrderMaintenance":
                return <UserOrderMaintenance ></UserOrderMaintenance>
            case "UserActionQuery":
                return <UserActionMsg ></UserActionMsg>
            case "UserLogQuery":
                return <UserLogQuery></UserLogQuery>
            default:
                return <div>建置中……</div>;
        }
    }
}

var app = document.getElementById('APP');
var page = $(app).attr("page");

ReactDOM.render(
<XQLiteMgmPage pid={page} />, 
document.getElementById('APP')
)