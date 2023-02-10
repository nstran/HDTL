import { DatePipe } from '@angular/common';
import { Component, ElementRef, Injector, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import * as firebase from 'firebase';
import { AbstractBase } from '../../abstract-base.component';
import { FireBaseService } from '../../services/fire-base.service';



@Component({
  selector: 'app-fire-base',
  templateUrl: './fire-base.component.html',
  styleUrls: ['./fire-base.component.css']
})
export class FireBaseComponent extends AbstractBase implements OnInit {
  @ViewChild('chatcontent', { static: true }) chatcontent: ElementRef;
  scrolltop: number = null;
  isShowChat = false;
  ref : any;
  chatForm: FormGroup;
  roomname : string;
  nickname : string;
  chats = [];
  // message : string = '';

  constructor(
    injector : Injector,
    private _fireBaseService : FireBaseService,
    public datepipe: DatePipe
    ) { 
      super(injector)
      // firebase.initializeApp(this.firebaseConfig);
      // this.ref = firebase.database().ref('rooms/');
      // firebase.database().ref('chats/').on('value', resp => {
      //   this.chats = [];
      //   this.chats = this.snapshotToArray(resp);
      //   setTimeout(() => this.scrolltop = this.chatcontent.nativeElement.scrollHeight, 500);
      // });
      // firebase.database().ref('roomusers/').orderByChild('roomname').equalTo(this.roomname).on('value', (resp2: any) => {
      //   const roomusers = this.snapshotToArray(resp2);
      //   // this.users = roomusers.filter(x => x.status === 'online');
      // });
  }

  ngOnInit(): void {
    
  };
  
  openChat(): void {
    this.isShowChat = true;
    this.getDataFireBase();
    setTimeout(() => this.scrolltop = this.chatcontent.nativeElement.scrollHeight);
  };

  closeChat(): void {
    this.isShowChat = false;
  };

  getDataFireBase(): void {
    // this._fireBaseService.getDataFireBase()
    //   .subscribe(result => {
    //     let roomName : Object = {
    //       roomname : result.fireBaseEntityModel.roomName
    //     };
    //     let userName : Object = {
    //       nickname : result.fireBaseEntityModel.userName
    //     }; 
    //     this.roomname = this.addRoom(roomName);
    //     this.nickname = this.addUser(userName);
    //     this.joinChatRoom(this.roomname, this.nickname);
    //   })
  };

  addUser(data: any): string {
    this.ref.orderByChild('nickname').equalTo(data.nickname).once('value', snapshot => {
      if (snapshot.exists()) {
        return data.nickname;
      } else {
        const newUser = firebase.database().ref('users/').push();
        newUser.set(data);
        return data.nickname;
      }
    });
    return data.nickname
  };

  addRoom(data: any): string {
    this.ref.orderByChild('roomname').equalTo(data.roomname).once('value', (snapshot: any) => {
      if (snapshot.exists()) {
        return data.roomname;
      } else {
        const newRoom = firebase.database().ref('rooms/').push();
        newRoom.set(data);
        return data.roomname;
      }
    });
    return data.roomname;
  };

  joinChatRoom(roomname: string, nickname: string): void {
    const chat = { roomname: '', nickname: '', message: '', date: '', type: '' };
    chat.roomname = roomname;
    chat.nickname = nickname;
    // chat.date = this.datepipe.transform(new Date(), 'dd/MM/yyyy HH:mm:ss');
    chat.message = `${nickname} enter the room`;
    chat.type = 'join';
    const newMessage = firebase.database().ref('chats/').push();
    newMessage.set(chat);

    firebase.database().ref('roomusers/').orderByChild('roomname').equalTo(roomname).on('value', (resp: any) => {
      let roomuser = [];
      roomuser = this.snapshotToArray(resp);
      const user = roomuser.find(x => x.nickname === nickname);
      if (user !== undefined) {
        const userRef = firebase.database().ref('roomusers/' + user.key);
        userRef.update({status: 'online'});
      } else {
        const newroomuser = { roomname: '', nickname: '', status: '' };
        newroomuser.roomname = roomname;
        newroomuser.nickname = nickname;
        newroomuser.status = 'online';
        const newRoomUser = firebase.database().ref('roomusers/').push();
        newRoomUser.set(newroomuser);
      }
    });
  };

  chat(value: string): void {
    if(value){
      let chat : any = {
        message : value
      }
      chat.roomname = this.roomname;
      chat.nickname = this.nickname;
      chat.date = this.datepipe.transform(new Date(), 'dd/MM/yyyy HH:mm:ss');
      chat.type = 'message';
      const newMessage = firebase.database().ref('chats/').push();
      newMessage.set(chat);
    }
  };

 snapshotToArray = (snapshot: any) => {
    const returnArr = [];
    snapshot.forEach((childSnapshot: any) => {
        const item = childSnapshot.val();
        item.key = childSnapshot.key;
        returnArr.push(item);
    });
    return returnArr;
  };

}
