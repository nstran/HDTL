import { Injector } from "@angular/core";
import { ConfirmationService, Message, MessageService } from "primeng/api";
import * as firebase from 'firebase/app';
import { NotificationFireBase } from "./models/fire-base.model";

export abstract class AbstractBase {
    message: MessageService;
    confirmationService: ConfirmationService;

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