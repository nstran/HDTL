import { Injector } from "@angular/core";
import { ConfirmationService, Message, MessageService } from "primeng/api";
import * as firebase from 'firebase/app';
import { NotificationFireBase } from "./models/fire-base.model";

export abstract class AbstractBase {
    message: MessageService;
    confirmationService: ConfirmationService;
    localeVi = {
        firstDayOfWeek: 0,
        dayNames: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
        dayNamesShort: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
        dayNamesMin: ["CN","T2","T3","T4","T5","T6","T7"],
        monthNames: [ "Tháng 1","Tháng 2","Tháng 3","Tháng 4","Tháng 5","Tháng 6","Tháng 7","Tháng 8","Tháng 9","Tháng 10","Tháng 11","Tháng 12" ],
        monthNamesShort: [ "Jan", "Feb", "Mar", "Apr", "May", "Jun","Jul", "Aug", "Sep", "Oct", "Nov", "Dec" ],
        today: 'Hôm nay',
        clear: 'Xóa',
        dateFormat: 'mm/dd/yy',
        weekHeader: 'Wk'
    };

    constructor(injector : Injector){
        this.message = injector.get(MessageService);
        this.confirmationService = injector.get(ConfirmationService);
    }

    showToast(severity: string, summary: string, detail: string): void {
        this.message.add({ severity: severity, summary: summary, detail: detail });
    }

    showMessage(msg: Message): void {
        this.message.add(msg);
    }

    createNotificationFireBase(notification : NotificationFireBase, employeeId: string): void {
        const noti = firebase.database().ref('notification/').child(employeeId).push();
        noti.set(notification);
    }

    convertToUTCTime(time: Date): Date {
        return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
    };
}