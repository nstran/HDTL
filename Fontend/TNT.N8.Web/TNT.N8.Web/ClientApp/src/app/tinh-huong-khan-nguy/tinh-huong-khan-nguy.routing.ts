import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthGuard } from '../shared/guards/auth.guard';

import { TinhHuongKhanNguyComponent } from './tinh-huong-khan-nguy.component';
import { ActionsComponent } from './components/actions/actions.component'

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: TinhHuongKhanNguyComponent,
        children: [
          {
            path: 'actions',
            component: ActionsComponent,
            canActivate: [AuthGuard]
          },
        ]
      }
    ])
  ],
  exports: [
    RouterModule
  ]
})
export class TinhHuongKhanNguyRouting {
}