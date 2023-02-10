import { CommonModule, DatePipe } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { SharedModule } from "../shared/shared.module";
import { ChatComponent } from "./chat.component";
import { ChatRouting } from "./chat.routing";
import { RoomListComponent } from "./components/room-list/room-list.component";
import { ChatroomComponent } from './components/chatroom/chatroom.component';
import { NotificationListComponent } from "./components/notification-list/notification-list.component";

@NgModule({
    imports: [
        CommonModule,
        ChatRouting,
        SharedModule,
        FormsModule,
        ReactiveFormsModule,
    ],
    declarations: [
        ChatComponent,
        RoomListComponent,
        ChatroomComponent,
        NotificationListComponent
    ],
    entryComponents: [
        ChatComponent,
    ],
    providers: [
        ChatComponent,
        DatePipe,
    ]
  })
  export class ChatModule { }
  