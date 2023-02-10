import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from "../shared/guards/auth.guard";

import { SalaryComponent } from './salary.component';
import { CauHinhChungComponent } from './components/cau-hinh/cau-hinh-chung/cau-hinh-chung.component';
import { TkChamCongComponent } from './components/tk-cham-cong/tk-cham-cong.component';
import { BaoCaoChiPhiComponent } from './components/bao-cao/bao-cao-chi-phi/bao-cao-chi-phi.component';
import { KyLuongListComponent } from './components/ky-luong/ky-luong-list/ky-luong-list.component';
import { TongQuanKyLuongComponent } from './components/ky-luong/ky-luong-detail/tong-quan-ky-luong/tong-quan-ky-luong.component';
import { PhieuLuongListComponent } from './components/phieu-luong/phieu-luong-list/phieu-luong-list.component';
import { PhieuLuongDetailComponent } from './components/phieu-luong/phieu-luong-detail/phieu-luong-detail.component';

@NgModule({
  imports: [RouterModule.forChild([
    {
      path: "",
      component: SalaryComponent,
      children: [
        {
          path: 'cau-hinh-chung',
          component: CauHinhChungComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'tk-cham-cong',
          component: TkChamCongComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'bao-cao-chi-phi',
          component: BaoCaoChiPhiComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'ky-luong-list',
          component: KyLuongListComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'ky-luong-detail',
          component: TongQuanKyLuongComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'phieu-luong-list',
          component: PhieuLuongListComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'phieu-luong-detail',
          component: PhieuLuongDetailComponent,
          canActivate: [AuthGuard]
        },
      ]
    }
  ])],
  exports: [RouterModule]
})
export class SalaryRoutingModule { }
