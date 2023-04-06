import { DatePipe } from '@angular/common';
import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as firebase from 'firebase';

@Component({
  selector: 'app-chatroom',
  templateUrl: './chatroom.component.html',
  styleUrls: ['./chatroom.component.css'],
})
export class ChatroomComponent implements OnInit {
  @ViewChild('chatcontent') chatcontent: ElementRef;
  @Input() roomName : string = '';
  scrolltop: number = null;
  nickname: string = localStorage.getItem('EmployeeName');
  message = '';
  users = [];
  chats = [];
  currentScrollTop : number;
  count : number = 20;
  idChat : string = '';
  countMessage : number;
  firstKey : string;
  scrollHeight: number;
  currentScrollHeight: number;
  isRunScroll: boolean = true;
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  employeeName : string = localStorage.getItem('EmployeeName');
  isChat: boolean = false;

  constructor(
    public _datepipe: DatePipe,
    public _activatedRoute : ActivatedRoute
  ) {}

  ngOnInit(): void {
    this._activatedRoute.params.subscribe(params => {
      this.roomName = params['id'];
      if(this.roomName){
        this.getChatByRoomName(this.roomName);
        this.getLastMessage();
      }
    });
    localStorage.setItem('scroll', '0');
  }

  scroll(): void {
    if(this.chatcontent.nativeElement.scrollTop == 0 && document.getElementById("chat") && this.chats.length > 0 && this.count == this.chats.length && this.isRunScroll == true){
      if(this.roomName){
        firebase.database().ref('chats/').child(this.roomName).orderByKey().endAt(this.firstKey).limitToLast(20).once('value', resp => {
          this.chats.unshift(... this.snapshotToArrayMessage(resp));
          this.firstKey = this.chats[0].key;
          this.count += 20;
        });
      }
      this.isChat = false;
    }
    this.isRunScroll = true;
  }

  getLastMessage(): void {
    firebase.database().ref('chats/').child(this.roomName).limitToLast(1).on('value', resp => {
      this.chats.push(this.snapshotToArrayMessageOn(resp, this.roomName)[0]);
      if(this.chats && this.chats.length > 0){
        this.chats = this.chats.filter(x => x != undefined);
        this.chats = [...new Map(this.chats.map(item =>[item['date'], item])).values()];
        setTimeout(() => this.scrolltop = this.chatcontent.nativeElement.scrollHeight, 0);
        this.chats.forEach(c => {
          if(window.location.pathname.substr(window.location.pathname.lastIndexOf('=') + 1) == this.roomName){
            if(c && c.senderId != this.userId && c.isSeen == false){
              if(c.key){
                const chatRef = firebase.database().ref('chats/').child(this.roomName + '/' + c.key);
                chatRef.update({isSeen: true});
              }
            }
          }
        })
      }
    })
  }

  getScroll(): void {
    if(this.chatcontent){
      this.scrollHeight = this.chatcontent.nativeElement.scrollHeight;
      if(this.currentScrollHeight != this.scrollHeight && document.getElementById("chat") && this.chats.length > 0 && this.isChat == false){
        this.currentScrollTop = this.chatcontent.nativeElement.scrollHeight - this.chatcontent.nativeElement.clientHeight;
        localStorage.getItem('scroll');
        let number = Number(localStorage.getItem('scroll'));
        if(number == 0){
          this.scrolltop = this.currentScrollTop;
        } else {
          this.scrolltop = this.currentScrollTop - number;
        }
        localStorage.setItem('scroll', this.currentScrollTop.toString());
        this.currentScrollHeight = this.scrollHeight;
      }
    }
  }

  getIdRoomName(isRunScroll : boolean): void {
    this.isRunScroll = isRunScroll;
    if(this.roomName != this._activatedRoute.snapshot.params.id){
      this.getChatByRoomName(this._activatedRoute.snapshot.params.id);
    }
    localStorage.setItem('scroll', '0');
  }

  getChatByRoomName(roomName: string): void {
    this.count = 20;
    if(roomName){
      firebase.database().ref('chats/').child(roomName).limitToLast(20).once('value', resp => {
        this.chats = [];
        this.chats = this.snapshotToArrayMessage(resp);
        if(this.chats[0]){
          this.firstKey = this.chats[0].key;
        }
        this.chats.forEach(c => {
          if(c.senderId != this.userId && c.isSeen == false){
            if(c.key){
              const chatRef = firebase.database().ref('chats/').child(this.roomName + '/' + c.key);
              chatRef.update({isSeen: true});
            }
          }
        })
        localStorage.setItem('scroll', '0');
        setTimeout(() => this.scrolltop = this.chatcontent.nativeElement.scrollHeight, 0);
      });
    }
  }

  chat(value: string): void {
    let re1 = /\<p>/gi;
    let re2 = /\<\/p>/gi;
    let re3 = /\<br>/gi;
    value = value.replace(re1, "");
    value = value.replace(re2, "");
    value = value.replace(re3, "");
    
    if(value){
      let chat : any = {
        message : value
      }
      chat.roomname = this.roomName;
      chat.nickname = this.employeeName;
      chat.senderId = this.userId;
      chat.date = new Date().getTime();
      chat.isSeen = false;
      const newMessage = firebase.database().ref('chats/').child(this.roomName);
      newMessage.push(chat);
      this.message = null;
      this.count = 20;
      localStorage.setItem('scroll', '0');
      setTimeout(() => this.scrolltop = this.chatcontent.nativeElement.scrollHeight, 0);
      this.isChat = true;
    }
  };

  snapshotToArrayMessage(snapshot: any): any[] {
    const returnArr = [];
    snapshot.forEach((childSnapshot: any) => {
      const item = childSnapshot.val();
      item.key = childSnapshot.key;
      if (item.message) {
        returnArr.push(item);
      }
    });
    return returnArr;
  };

  snapshotToArrayMessageOn(snapshot: any, roomName: string): any[] {
    const returnArr = [];
    snapshot.forEach((childSnapshot: any) => {
      const item = childSnapshot.val();
      item.key = childSnapshot.key;
      if (item.message && item.roomname == roomName) {
        returnArr.push(item);
      }
    });
    return returnArr;
  };

  snapshotToArrayMessageLastMessage(snapshot: any): any[] {
    const returnArr = [];
    snapshot.forEach((childSnapshot: any) => {
      const item = childSnapshot.val();
      item.key = childSnapshot.key;
      if (item.senderId != this.userId) {
        returnArr.push(item);
      }
    });
    return returnArr;
  };

  snapshotToArrayMessageUnread(snapshot: any): any[] {
    const returnArr = [];
    snapshot.forEach((childSnapshot: any) => {
      const item = childSnapshot.val();
      item.key = childSnapshot.key;
      if (item.senderId != this.userId && item.isSeen == false) {
        returnArr.push(item);
      }
    });
    return returnArr;
  };

}
