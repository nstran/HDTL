import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SalaryRoutingModule } from './salary-routing.module';

import { SalaryService } from './services/salary.service';
import { NoteService } from './../shared/services/note.service';

import { SalaryComponent } from './salary.component';
import { LuongComponent } from './components/cau-hinh/luong/luong.component';
import { CauHinhChungComponent } from './components/cau-hinh/cau-hinh-chung/cau-hinh-chung.component';
import { TkChamCongComponent } from './components/tk-cham-cong/tk-cham-cong.component';
import { UpdateChamCongDialogComponent } from './components/update-cham-cong-dialog/update-cham-cong-dialog.component';
import { ChamCongThuongComponent } from './components/tk-cham-cong/cham-cong-thuong/cham-cong-thuong.component';
import { ChamCongOtComponent } from './components/tk-cham-cong/cham-cong-ot/cham-cong-ot.component';
import { ChamCongOtChiTietDialogComponent } from './components/tk-cham-cong/cham-cong-ot-chi-tiet-dialog/cham-cong-ot-chi-tiet-dialog.component';
import { BaoCaoChiPhiComponent } from './components/bao-cao/bao-cao-chi-phi/bao-cao-chi-phi.component';
import { KyLuongListComponent } from './components/ky-luong/ky-luong-list/ky-luong-list.component';
import { TongQuanKyLuongComponent } from './components/ky-luong/ky-luong-detail/tong-quan-ky-luong/tong-quan-ky-luong.component';
import { BangLuongChiTietComponent } from './components/ky-luong/ky-luong-detail/bang-luong-chi-tiet/bang-luong-chi-tiet.component';
import { PhieuLuongListComponent } from './components/phieu-luong/phieu-luong-list/phieu-luong-list.component';
import { PhieuLuongDetailComponent } from './components/phieu-luong/phieu-luong-detail/phieu-luong-detail.component';
import { UpdateDieuKienTroCapCoDinhComponent } from './components/ky-luong/ky-luong-detail/update-dieu-kien-tro-cap-co-dinh/update-dieu-kien-tro-cap-co-dinh.component';
import { UpdateDieuKienTroCapKhacComponent } from './components/ky-luong/ky-luong-detail/update-dieu-kien-tro-cap-khac/update-dieu-kien-tro-cap-khac.component';

@NgModule({
  imports: [
    CommonModule,
    SalaryRoutingModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule
  ],
  declarations: [
    SalaryComponent,
    LuongComponent,
    CauHinhChungComponent,
    TkChamCongComponent,
    UpdateChamCongDialogComponent,
    ChamCongThuongComponent,
    ChamCongOtComponent,
    ChamCongOtChiTietDialogComponent,
    BaoCaoChiPhiComponent,
    KyLuongListComponent,
    TongQuanKyLuongComponent,
    BangLuongChiTietComponent,
    PhieuLuongListComponent,
    PhieuLuongDetailComponent,
    UpdateDieuKienTroCapCoDinhComponent,
    UpdateDieuKienTroCapKhacComponent
  ],
  providers: [
    SalaryService,
    NoteService
  ],
  entryComponents: [
    UpdateChamCongDialogComponent
  ]
})
export class SalaryModule { }
