import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthGuard } from './shared/guards/auth.guard';
import { HomeComponent } from './home/home.component';
import { UserprofileComponent } from './userprofile/userprofile.component'

@NgModule({
  imports: [
    RouterModule.forRoot([
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
      { path: 'userprofile', component: UserprofileComponent, canActivate: [AuthGuard] },
      { path: 'forgot-pass', loadChildren: () => import('./forgot-pass/forgot-pass.module').then(m => m.ForgotPassModule) },
      { path: 'login', loadChildren: () => import('./login/login.module').then(m => m.LoginModule) },
      { path: 'admin', loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule), canActivate: [AuthGuard] },
      // { path: 'lead', loadChildren: () => import('./lead/lead.module').then(m => m.LeadModule), canActivate: [AuthGuard] },
      { path: 'employee', loadChildren: () => import('./employee/employee.module').then(m => m.EmployeeModule), canActivate: [AuthGuard] },
      { path: 'vendor', loadChildren: () => import('./vendor/vendor.module').then(m => m.VendorModule), canActivate: [AuthGuard] },
      { path: 'product', loadChildren: () => import('./product/product.module').then(m => m.ProductModule), canActivate: [AuthGuard] },
      { path: 'order', loadChildren: () => import('./order/order.module').then(m => m.OrderModule), canActivate: [AuthGuard] },
      { path: 'customer', loadChildren: () => import('./customer/customer.module').then(m => m.CustomerModule), canActivate: [AuthGuard] },
      // { path: 'accounting', loadChildren: () => import('./accounting/accounting.module').then(m => m.AccountingModule), canActivate: [AuthGuard] },
      { path: 'procurement-request', loadChildren: () => import('./procurement-request/procurement-request.module').then(m => m.ProcurementRequestModule), canActivate: [AuthGuard] },
      { path: 'sales', loadChildren: () => import('./sales/sales.module').then(m => m.SalesModule), canActivate: [AuthGuard] },
      { path: 'chat', loadChildren: () => import('./chat/chat.module').then(m => m.ChatModule), canActivate: [AuthGuard] },
      // { path: 'warehouse', loadChildren: () => import('./warehouse/warehouse.module').then(m => m.WarehouseModule), canActivate: [AuthGuard] },
      // { path: 'sale-bidding', loadChildren: () => import('./sale-bidding/sale-bidding.module').then(m => m.SaleBiddingModule), canActivate: [AuthGuard] },
      // { path: 'bill-sale', loadChildren: () => import('./bill-sale/bill-sale.module').then(m => m.BillSaleModule), canActivate: [AuthGuard] },
      // { path: 'promotion', loadChildren: () => import('./promotion/promotion.module').then(m => m.PromotionModule), canActivate: [AuthGuard] },
      // { path: 'project', loadChildren: () => import('./project/project.module').then(m => m.ProjectModule), canActivate: [AuthGuard] },
      // { path: 'manufacture', loadChildren: () => import('./manufacture/manufacture.module').then(m => m.ManufactureModule), canActivate: [AuthGuard] },
      // { path: 'tinhHuongKhanNguy', loadChildren: () => import('./tinh-huong-khan-nguy/tinh-huong-khan-nguy.module').then(m => m.TinhHuongKhanNguyModule), canActivate: [AuthGuard] },
      // { path: 'asset', loadChildren: () => import('./asset/asset.module').then(m => m.AssetModule), canActivate: [AuthGuard] },
      // { path: 'salary', loadChildren: () => import('./salary/salary.module').then(m => m.SalaryModule), canActivate: [AuthGuard] },
    ])],
  exports: [
    RouterModule
  ],
  providers: [AuthGuard]
})
export class AppRouting {
}
