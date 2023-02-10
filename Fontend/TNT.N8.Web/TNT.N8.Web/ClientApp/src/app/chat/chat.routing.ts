import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthGuard } from '../shared/guards/auth.guard';
import { ChatComponent } from './chat.component';
import { NotificationListComponent } from './components/notification-list/notification-list.component';
import { RoomListComponent } from './components/room-list/room-list.component';
@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: ChatComponent,
        children: [
          {
            path: 'room-list',
            component: RoomListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'notificaton-list',
            component: NotificationListComponent,
            canActivate: [AuthGuard]
          }
        ]
      }
    ])
  ],
  exports: [
    RouterModule
  ]
 
})
export class ChatRouting { }
