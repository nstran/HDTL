import { Component, Injector, OnInit } from '@angular/core';
import firebase from 'firebase';
import { AbstractBase } from '../../../shared/abstract-base.component';

@Component({
  selector: 'app-notification-list',
  templateUrl: './notification-list.component.html',
  styleUrls: ['./notification-list.component.css']
})
export class NotificationListComponent extends AbstractBase implements OnInit {
  first: number = 0;
  loading: boolean = false;
  rows: number = 10;
  cols: any[];
  filterText: string;
  listNotification: any[] = [];
  listCurrentNotification: any[] = [];
  listNotificationSelected : any[] = [];
  auth: any = JSON.parse(localStorage.getItem("auth"));
  employeeId = this.auth.EmployeeId;

  constructor(
    injector : Injector
  ) {
    super(injector)
  }

  ngOnInit(): void {
    this.initTable();
    this.getListNotification();
    this.getLastNotification()
  }

  initTable(): void {
    this.cols = [
      { field: 'content', header: 'Thông báo', textAlign: 'left', display: 'table-cell' },
      { field: 'status', header: 'Trạng thái', textAlign: 'left', display: 'table-cell' },
      { field: 'date', header: 'Thời gian', textAlign: 'left', display: 'table-cell' },
    ];
  }

  getListNotification(): void {
    firebase.database().ref('notification/').child(this.employeeId).once('value', resp => {
      this.listNotification = this.snapshotToArray(resp).sort((a, b): any => { return b.date.localeCompare(a.date) });
      this.listCurrentNotification = this.listNotification;
    })
  }

  getLastNotification(): void {
    firebase.database().ref('notification/').child(this.auth.EmployeeId).limitToLast(1).on('value', resp => {
      this.getListNotification();
    })
  }

  filterNotification(text: string): void {
    if(text){
      this.listNotification = this.listNotification.filter(x => x.content.toLocaleLowerCase().includes(text.toLocaleLowerCase()));
    }else{
      this.listNotification = this.listCurrentNotification;
    }
  }

  goToNotiUrl(item): void {
    if(item.url) {
      window.location.href = window.location.protocol + '//' + window.location.host + item.url;
      const notificationRef = firebase.database().ref('notification').child(this.auth.EmployeeId + '/' + item.key);
      notificationRef.update({status: true});
    }
  }

  deleteNotification(item): void {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        const notificationRef = firebase.database().ref('notification').child(this.auth.EmployeeId + '/' + item.key);
        notificationRef.remove();
        this.listNotification = this.listNotification.filter(x => x.key != item.key);
      }
    });
  }

  deleteNotificationSelected(): void {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.listNotificationSelected.forEach(x => {
          const notificationRef : any = firebase.database().ref('notification').child(this.auth.EmployeeId + '/' + x.key);
          notificationRef.on('value', (snap) => {
            notificationRef.remove();
            this.listNotification = this.listNotification.filter(x => x.key != x.key);              
          })
        })
      }
    });
  }

  snapshotToArray(snapshot: any): any[] {
    const returnArr = [];
    snapshot.forEach((childSnapshot: any) => {
        const item = childSnapshot.val();
        item.key = childSnapshot.key;
        returnArr.push(item);
    });
    return returnArr;
  };

}
