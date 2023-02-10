import { CustomerService } from './../customer/services/customer.service';
import { TinhHuongKhanNguyRouting } from './tinh-huong-khan-nguy.routing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from './../shared/shared.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TinhHuongKhanNguyComponent } from './tinh-huong-khan-nguy.component';
import { ActionsComponent } from './components/actions/actions.component'

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    TinhHuongKhanNguyRouting,
    FormsModule,
    ReactiveFormsModule
  ],
  declarations: [
    TinhHuongKhanNguyComponent,
    ActionsComponent
  ],
  providers: [
    CustomerService
  ]
})
export class TinhHuongKhanNguyModule { }
