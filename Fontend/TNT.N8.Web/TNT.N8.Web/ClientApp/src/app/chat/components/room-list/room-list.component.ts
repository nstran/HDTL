import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as firebase from 'firebase';
import { AbstractBase } from '../../../shared/abstract-base.component';
import { ChatService } from '../../services/chat.service';
import { ChatroomComponent } from '../chatroom/chatroom.component';
import { FormControl } from '@angular/forms';
import { debounceTime, startWith, switchMap, tap } from 'rxjs/operators';
import { EmployeeEntityModel } from '../../models/chat.models';

@Component({
  selector: 'app-room-list',
  templateUrl: './room-list.component.html',
  styleUrls: ['./room-list.component.css']
})
export class RoomListComponent extends AbstractBase implements OnInit {
  @ViewChild('chatroom', { static: true }) chatroomComponent: ChatroomComponent;
  loading : boolean = false;
  rooms = [];
  auth: any = JSON.parse(localStorage.getItem('auth'));
  filterText: string = '';
  userId = this.auth.UserId;
  queryControl = new FormControl("");
  isShowFilter : boolean = false;
  listEmployee : EmployeeEntityModel[] = [];
  employeeName : string = localStorage.getItem('EmployeeName');

  constructor(
    injector : Injector,
    private _router: Router,
    private _chatService : ChatService,
    public _activatedRoute : ActivatedRoute
    ) {
    super(injector)
  }

  ngOnInit(): void {
    this.getDataFireBase();
    this.takeListEmployeeByUsername();
  }

  getDataFireBase(): void {
    let fireBase = firebase.database();
    fireBase.ref('rooms/').on('value', async resp => {
      this.rooms = [];
      this.rooms = this.snapshotToArrayRoomName(resp);
      let listAvatarUrl : any = await this.getUserProfileByRoomName(this.rooms.map(x => x.roomname));
      this.rooms.forEach(r => {
        fireBase.ref('chats/').child(r.roomname).limitToLast(5).on('value', resp => {
          const listMessageUnread = this.snapshotToArrayMessage(resp, r.roomname);
          r.messageUnreadCount = listMessageUnread.length;
          r.userChat = (r.receiver == this.userId ? r.userCreateName : r.receiverName);
          let userInfor = listAvatarUrl.listUserProfileResult.find(y => y.objectId == (r.roomname.split('_')[1] == this.userId ? r.roomname.split('=').pop().split('')[0] : r.roomname.split('_')[1]));
          r.avatarUrl = userInfor?.avatarUrl;
          let listLastMessage = this.snapshotToArrayLastMessage(resp, r.roomname);
          if(listLastMessage.length > 0){
            r.lastMessage = listLastMessage[listLastMessage.length - 1].message;
            r.timeLastMessage = listLastMessage[listLastMessage.length - 1].date;
          }
        });
      })
      this.rooms = this.rooms.sort((a, b) => (b.timeLastMessage > a.timeLastMessage) ? 1 : -1);
    });
  };

  async getUserProfileByRoomName(listRoomName : string[]) {
    return await this._chatService.getUserProfileByRoomName(listRoomName);
  }

  showFilterListEmployee(): void {
    this.isShowFilter = true;
  }

  closeFilterListEmployee(): void {
    this.isShowFilter = false;
    this.filterText = '';
  }

  takeListEmployeeByUsername(): void {
    this.queryControl.valueChanges
    .pipe(
      debounceTime(1000),
      tap(() => { this.loading = true; }),
      startWith(this.filterText != "" ? this.filterText : ""),
      switchMap((query: string) =>
        this._chatService.takeListEmployeeByUsername(
          query
        ).pipe(tap(() => {
          this.loading = false;
          this.filterText = query;
        }))
      )
    )
    .subscribe((result) => {
      this.listEmployee = result.listEmployeeEntityModel;
    });
  }

  async createRoomChat(event: EmployeeEntityModel){
    let result : any = await this._chatService.createDataFireBase(this.userId + '_' + event.userId, event.userId)
    if(result.statusCode == 200){
      let fireBase = firebase.database();
      fireBase.ref('rooms/').orderByChild('roomname').equalTo(result.roomname).once('value', (snapshot: any) => {
        if (snapshot.exists()) {
          let room = {
            roomname : result.roomname,
          }
          this.joinRoom(room);
          this.closeFilterListEmployee();
        } else {
          const newRoom = fireBase.ref('rooms/').push();
          let room = {
            roomname : this.userId + '_' + event.userId,
            userCreate : this.userId,
            userCreateName: this.employeeName,
            receiver : event.userId,
            receiverName: event.employeeName
          }
          newRoom.set(room);
          this.joinRoom(room);
          this.closeFilterListEmployee();
        }
        this.getDataFireBase();
      });
    }
  }

  joinRoom(room: any): void {
    this._router.navigate(['/chat/room-list', { id: room.roomname }]);
    this.chatroomComponent.getIdRoomName(false);
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

  snapshotToArrayLastMessage(snapshot: any, roomName: string): any[] {
    const returnArr = [];
    snapshot.forEach((childSnapshot: any) => {
        const item = childSnapshot.val();
        item.key = childSnapshot.key;
        if(item.roomname == roomName){
          returnArr.push(item);
        }
    });
    return returnArr;
  };

  snapshotToArrayRoomName(snapshot: any): any[] {
    const returnArr = [];
    snapshot.forEach((childSnapshot: any) => {
        const item = childSnapshot.val();
        item.key = childSnapshot.key;
        if ((item.receiver == this.userId || item.userCreate == this.userId)) {
          returnArr.push(item);
        }
    });
    return returnArr;
  };

  snapshotToArrayMessage(snapshot: any, roomName: string): any[] {
    const returnArr = [];
    snapshot.forEach((childSnapshot: any) => {
        const item = childSnapshot.val();
        item.key = childSnapshot.key;
        if (item.roomname == roomName && item.senderId != this.userId && item.isSeen == false) {
          returnArr.push(item);
        }
    });
    return returnArr;
  };

  snapshotToArrayFindRoomName(snapshot: any): any[] {
    const returnArr = [];
    snapshot.forEach((childSnapshot: any) => {
      const item = childSnapshot.val();
      item.key = childSnapshot.key;
      returnArr.push(item);
    });
    return returnArr;
  };

}
