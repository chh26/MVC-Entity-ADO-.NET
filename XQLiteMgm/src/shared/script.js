import { message } from 'antd';

export function ShowMessage(type,info){
    message.config({
        top: 100
    });
    switch(type) {
        case "success":
            message.success(info);
            break;
        case "warn":
            message.warning(info);
            break;
        case "error":
            message.error(info);
            break;
    }
}

//數字加入千分號
export function convertAmount(t) {
    if (checkNegative(t)){
        if (t.length > 3)
            t = t.substring(0, t.length - 3) + "," + t.substring(t.length - 3, t.length)
    }
    else {
        if (t.length > 4)
            t = t.substring(0, t.length - 3) + "," + t.substring(t.length - 3, t.length)
    }
    return t;
};

//數字確認是否為負數，return true 為正數，反之為負數
export function checkNegative(n) {
    if (typeof n == "string") {
        return !!(n.match(/^\d+((\.\d+){0,})?$/) && parseFloat(n) > 0)
    } else if (typeof n == "number") {
        return !isNaN(n) && n > 0;
    }
}