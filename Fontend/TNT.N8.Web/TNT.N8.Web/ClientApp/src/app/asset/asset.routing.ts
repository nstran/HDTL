import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../shared/guards/auth.guard';
import { AssetComponent } from './asset.component';
import { AssetCreateComponent } from './components/asset-create/asset-create.component';
import { AssetListComponent } from './components/asset-list/asset-list.component';
import { PhanBoTaiSanComponent } from './components/phan-bo-tai-san/phan-bo-tai-san.component';
import { ThuHoiTaiSanComponent } from './components/thu-hoi-tai-san/thu-hoi-tai-san.component';
import { AssetDetailComponent } from './components/asset-detail/asset-detail.component';
import { YeuCauCapPhatCreateComponent } from './components/yeu-cau-cap-phat-create/yeu-cau-cap-phat-create.component';
import { DanhSachCapPhatListComponent } from './components/yeu-cau-cap-phat-list/yeu-cau-cap-phat-list.component';
import { YeuCauCapPhatDetailComponent } from './components/yeu-cau-cap-phat-detail/yeu-cau-cap-phat-detail.component';
import { BaoCaoPhanBoComponent } from './components/bao-cao-phan-bo/bao-cao-phan-bo.component';
import { BaoCaoKhauHaoComponent } from './components/bao-cao-khau-hao/bao-cao-khau-hao.component';
import { DotKiemKeListComponent } from './components/dot-kiem-ke-list/dot-kiem-ke-list.component';
import { DotKiemKeDetailComponent } from './components/dot-kiem-ke-detail/dot-kiem-ke-detail.component';
@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: AssetComponent,
        children: [
          {
            path: 'list',
            component: AssetListComponent,
            canActivate: [AuthGuard]
          }
          ,
          {
            path: 'create',
            component: AssetCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'phan-bo-tai-san',
            component: PhanBoTaiSanComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'thu-hoi-tai-san',
            component: ThuHoiTaiSanComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'detail',
            component: AssetDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'yeu-cau-cap-phat-tai-san',
            component: YeuCauCapPhatCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'danh-sach-yeu-cau-cap-phat',
            component: DanhSachCapPhatListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'chi-tiet-yeu-cau-cap-phat',
            component: YeuCauCapPhatDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'bao-cao-phan-bo',
            component: BaoCaoPhanBoComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'bao-cao-khau-hao',
            component: BaoCaoKhauHaoComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'danh-sach-dot-kiem-ke',
            component: DotKiemKeListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'chi-tiet-dot-kiem-ke',
            component: DotKiemKeDetailComponent,
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
export class AssetRouting { }
