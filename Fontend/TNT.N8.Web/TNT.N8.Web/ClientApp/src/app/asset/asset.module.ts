import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TabMenuModule } from 'primeng/tabmenu';
import { StepsModule } from 'primeng/steps';
import { DatePipe } from '@angular/common';

import { AssetRouting } from './asset.routing';
import { AssetComponent } from './asset.component';
import { AssetCreateComponent } from './components/asset-create/asset-create.component';
import { AssetDetailComponent } from './components/asset-detail/asset-detail.component';
import { AssetListComponent } from './components/asset-list/asset-list.component';
import { AssetService } from '../asset/services/asset.service';
import { QRCodeModule } from 'angularx-qrcode';
import { PhanBoTaiSanComponent } from './components/phan-bo-tai-san/phan-bo-tai-san.component';
import { PhanBoImportDetailComponent } from './components/phan-bo-import-detail/phan-bo-import-detail.component';
import { ThuHoiTaiSanComponent } from './components/thu-hoi-tai-san/thu-hoi-tai-san.component';
import { ThuHoiImportDetailComponent } from './components/thu-hoi-import-detail/thu-hoi-import-detail.component';
import { YeuCauCapPhatCreateComponent } from './components/yeu-cau-cap-phat-create/yeu-cau-cap-phat-create.component';
import { YeuCauCapPhatImportDetailComponent } from './components/yeu-cau-cap-phat-import-detail/yeu-cau-cap-phat-import-detail.component';
import { DanhSachCapPhatListComponent } from './components/yeu-cau-cap-phat-list/yeu-cau-cap-phat-list.component';
import { YeuCauCapPhatDetailComponent } from './components/yeu-cau-cap-phat-detail/yeu-cau-cap-phat-detail.component';
import { BaoCaoPhanBoComponent } from './components/bao-cao-phan-bo/bao-cao-phan-bo.component';
import { BaoCaoKhauHaoComponent } from './components/bao-cao-khau-hao/bao-cao-khau-hao.component';
import { NoteService } from '../shared/services/note.service';
import { EmployeeService } from '../employee/services/employee.service';
import { AssetImportDetailComponent } from '../asset/components/asset-import-detail/asset-import-detail.component';
import { DotKiemKeListComponent } from './components/dot-kiem-ke-list/dot-kiem-ke-list.component';
import { DotKiemKeDetailComponent } from './components/dot-kiem-ke-detail/dot-kiem-ke-detail.component';

@NgModule({
  imports: [
    CommonModule,
    AssetRouting,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    TabMenuModule,
    StepsModule,
    QRCodeModule
  ],
  declarations: [
    AssetComponent,
    AssetCreateComponent,
    AssetDetailComponent,
    AssetListComponent,
    PhanBoTaiSanComponent,
    PhanBoImportDetailComponent,
    ThuHoiTaiSanComponent,
    ThuHoiImportDetailComponent,
    YeuCauCapPhatCreateComponent,
    YeuCauCapPhatImportDetailComponent,
    DanhSachCapPhatListComponent,
    YeuCauCapPhatDetailComponent,
    BaoCaoPhanBoComponent,
    BaoCaoKhauHaoComponent,
    AssetImportDetailComponent,
    DotKiemKeListComponent,
    DotKiemKeDetailComponent
  ],
  bootstrap: [],
  entryComponents: [
    //AssetDialogComponent
  ],
  providers: [
    AssetComponent,
    AssetService,
    DatePipe,
    NoteService,
    EmployeeService
  ]
})
export class AssetModule { }
