import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSnackBarConfig } from '@angular/material/snack-bar';
import { EmployeeRouting } from './employee.routing';
import { DatePipe } from '@angular/common';

import { EmployeeComponent } from './employee.component';
import { ListComponent } from './components/list/list.component';
import { EmployeeQuitWorkComponent } from './components/employee-quit-work/employee-quit-work.component';
import { CommonService } from '../shared/services/common.service';
import { OrganizationService } from '../shared/services/organization.service';
import { EmployeeService } from './services/employee.service';
import { EmployeeRequestService } from './services/employee-request/employee-request.service';
import { CategoryService } from '../shared/services/category.service';
import { PositionService } from '../shared/services/position.service';
import { PermissionService } from '../shared/services/permission.service';
import { EmployeeTimesheetService } from './services/employee-salary/employee-timesheet.service';
import { EmployeeSalaryHandmadeImportService } from './services/employee-salary/employee-salary-handmade-import.service';
import { TeacherSalaryHandmadeImportService } from './services/teacher-salary/teacher-salary-handmade-import.service';
import { AssistantExportListService } from './services/assistant-salary/assistant-export-list.service';
import { AssistantSalaryHandmadeImportService } from './services/assistant-salary/assistant-salary-handmade-import.service';
import { AssistantSalaryFindService } from './services/assistant-salary/assistant-salary-find.service';
import { EmployeeListService } from './services/employee-list/employee-list.service';
import { EmailConfigService } from '../admin/services/email-config.service';
import { ForderConfigurationService } from '../admin/components/folder-configuration/services/folder-configuration.service';

import { OrgSelectDialogComponent } from './components/org-select-dialog/org-select-dialog.component';
import { UnitSelectDialogComponent } from './components/unit-select-dialog/unit-select-dialog.component';
import { EmployeeSalaryListComponent } from './components/employee-salary/employee-salary-list/employee-salary-list.component';
import { CreateEmployeeRequestComponent } from './components/employee-request/create-employee-request/create-employee-request.component';
import { EmployeeTimesheetImportComponent } from './components/employee-salary/employee-timesheet-import/employee-timesheet-import.component';
import { EmployeeCreateComponent } from './components/employee-profile/employee-create/employee-create.component';
import { ListEmployeeRequestComponent } from './components/employee-request/list-employee-request/list-employee-request.component';
import { BankService } from '../shared/services/bank.service';
import { EmployeeSalaryService } from './services/employee-salary/employee-salary.service';
import { EmployeeCreateSalaryPopupComponent } from './components/employee-profile/employee-create-salary-popup/employee-create-salary-popup.component'
import { EmployeeAllowanceService } from './services/employee-allowance/employee-allowance.service';
import { EmployeeInsuranceService } from './services/employee-insurance/employee-insurance.service';
import { EmployeeSalaryHandmadeImportComponent } from './components/employee-salary/employee-salary-handmade-import/employee-salary-handmade-import.component';
import { EmployeeMonthySalaryService } from './services/employee-salary/employee-monthy-salary.service';
import { EmployeeAssessmentService } from './services/employee-assessment/employee-assessment.service';
import { ImageUploadService } from '../shared/services/imageupload.service';
import { NoteService } from '../shared/services/note.service';
import { PopupComponent } from '../shared/components/popup/popup.component';
import { NgxLoadingModule } from 'ngx-loading';
import { ContactService } from '../shared/services/contact.service';
import { TeacherSalaryListComponent } from './components/employee-salary/teacher-salary-list/teacher-salary-list.component';
import { TeacherSapiGetComponent } from './components/employee-salary/teacher-sapi-get/teacher-sapi-get.component';
import { TeacherSalaryService } from './services/teacher-salary/teacher-salary.service';
import { TeacherSalaryHandmadeImportComponent } from './components/employee-salary/teacher-salary-handmade-import/teacher-salary-handmade-import.component';
import { AssistantSalaryListComponent } from './components/employee-salary/assistant-salary-list/assistant-salary-list.component';
import { AssistantSalaryExportComponent } from './components/employee-salary/assistant-salary-export/assistant-salary-export.component';
import { AssistantSalaryHandmadeImportComponent } from './components/employee-salary/assistant-salary-handmade-import/assistant-salary-handmade-import.component';
import { WorkflowService } from '../admin/components/workflow/services/workflow.service';
import { MatChipsModule } from '@angular/material/chips';
import { DetailEmployeeRequestComponent } from './components/employee-request/detail-employee-request/detail-employee-request.component';
import { EmployeeDashboardComponent } from './components/employee-dashboard/employee-dashboard.component';
import { OrgSelectMultiDialogComponent } from './components/org-select-multi-dialog/org-select-multi-dialog.component';
import { GetPermission } from '../shared/permission/get-permission';
import { EmailService } from '../shared/services/email.service';
import { TongQuanNvComponent } from './components/employee-profile/employee-details/tong-quan-nv/tong-quan-nv.component';
import { ThongTinChungComponent } from './components/employee-profile/employee-details/thong-tin-chung/thong-tin-chung.component';
import { ThongTinNhanSuComponent } from './components/employee-profile/employee-details/thong-tin-nhan-su/thong-tin-nhan-su.component';
import { CauHinhPhanQuyenComponent } from './components/employee-profile/employee-details/cau-hinh-phan-quyen/cau-hinh-phan-quyen.component';
import { LuongVaTroCapComponent } from './components/employee-profile/employee-details/luong-va-tro-cap/luong-va-tro-cap.component';
import { ThongTinDanhGiaComponent } from './components/employee-profile/employee-details/thong-tin-danh-gia/thong-tin-danh-gia.component';
import { NvGhiChuComponent } from './components/employee-profile/employee-details/nv-ghi-chu/nv-ghi-chu.component';
import { ChonNhieuDvDialogComponent } from './../shared/components/chon-nhieu-dv-dialog/chon-nhieu-dv-dialog.component';

import { CandidateImportDetailComponent } from '../employee/components/candidate-import-detail/candidate-import-detail.component'
import { ChienDichCreateComponent } from './components/chien-dich-create/chien-dich-create.component';
import { ChienDichDashboardComponent } from './components/chien-dich-dashboard/chien-dich-dashboard.component';
import { ChienDichDetailComponent } from './components/chien-dich-detail/chien-dich-detail.component';
import { RecruitmentCampaignInforPanelComponent } from '../employee/components/chien-dich-information-panel/chien-dich-information-panel.component'
import { ChienDichListComponent } from './components/chien-dich-list/chien-dich-list.component';



import { TuyenDungDashboardComponent } from './components/tuyen-dung-dashboard/tuyen-dung-dashboard.component';
import { TuyenDungCreateComponent } from './components/tuyen-dung-create/tuyen-dung-create.component';
import { TuyenDungDetailComponent } from './components/tuyen-dung-detail/tuyen-dung-detail.component';
import { TuyenDungListComponent } from './components/tuyen-dung-list/tuyen-dung-list.component';
import { TuyenDungBaoCaoComponent } from './components/tuyen-dung-bao-cao/tuyen-dung-bao-cao.component';

import { UngVienDashboardComponent } from './components/ung-vien-dashboard/ung-vien-dashboard.component';
import { UngVienCreateComponent } from './components/ung-vien-create/ung-vien-create.component';
import { UngVienDetailComponent } from './components/ung-vien-detail/ung-vien-detail.component';
import { UngVienListComponent } from './components/ung-vien-list/ung-vien-list.component';

import { BaoHiemVaThueComponent } from './components/employee-profile/employee-details/bao-hiem--thue/bao-hiem-va-thue/bao-hiem-va-thue.component';
import { HocVanVaTuyenDungComponent } from './components/employee-profile/employee-details/hoc-van-va-tuyen-dung/hoc-van-va-tuyen-dung.component';
import { TaiLieuComponent } from './components/employee-profile/employee-details/checklist-tai-lieu/tai-lieu/tai-lieu.component';
import { ThemMoiTaiLieuComponent } from './components/employee-profile/employee-details/checklist-tai-lieu/them-moi-tai-lieu/them-moi-tai-lieu.component';
import { ThongTinGiaDinhComponent } from './components/employee-profile/employee-details/thong-tin-gia-dinhs/thong-tin-gia-dinh/thong-tin-gia-dinh.component';
import { ThongTinHopDongComponent } from './components/employee-profile/employee-details/hop-dong/thong-tin-hop-dong/thong-tin-hop-dong.component';
import { ThongTinKhacComponent } from './components/employee-profile/employee-details/thong-tin-khac/thong-tin-khac.component';
import { DeXuatTangLuongComponent } from './components/de-xuat-tang-luongGroup/de-xuat-tang-luong/de-xuat-tang-luong.component';
import { ThemMoiHopDongComponent } from './components/employee-profile/employee-details/hop-dong/them-moi-hop-dong/them-moi-hop-dong.component';
import { ChiTietHopDongComponent } from './components/employee-profile/employee-details/hop-dong/chi-tiet-hop-dong/chi-tiet-hop-dong.component';
import { TheoDoiChucVuComponent } from './components/employee-profile/employee-details/theo-doi-chuc-vu/theo-doi-chuc-vu.component';
import { CauHinhBaoHiemComponent } from './components/cauhinh-baohiem/cauhinh-baohiem/cauhinh-baohiem.component';
import { ImportNvDeXuatTangLuongComponent } from './components/de-xuat-tang-luongGroup/importNv-de-xuat-tang-luong/importNv-de-xuat-tang-luong.component';
import { DeXuatTangLuongDetailComponent } from './components/de-xuat-tang-luongGroup/de-xuat-tang-luong-detail/de-xuat-tang-luong-detail.component';
import { DeXuatTangLuongListComponent } from './components/de-xuat-tang-luongGroup/de-xuat-tang-luong-list/de-xuat-tang-luong-list.component';
import { ThemMoiGiaDinhComponent } from './components/employee-profile/employee-details/thong-tin-gia-dinhs/them-moi-gia-dinh/them-moi-gia-dinh.component';
import { KeHoachOtCreateComponent } from './components/quan-ly-ot/kehoach-ot-create/kehoach-ot-create.component';
import { DanhSachOtComponent } from "./components/quan-ly-ot/danh-sach-ot/danh-sach-ot.component";
import { ConfirmationService } from 'primeng/api';
import { DeXuatChucVuComponent } from './components/de-xuat-chuc-vuGroup/de-xuat-chuc-vu/de-xuat-chuc-vu.component';
import { ImportNvDeXuatChucVuComponent } from './components/de-xuat-chuc-vuGroup/importNv-de-xuat-chuc-vu/importNv-de-xuat-chuc-vu.component';
import { DeXuatChucVuDetailComponent } from './components/de-xuat-chuc-vuGroup/de-xuat-chuc-vu-detail/de-xuat-chuc-vu-detail.component';
import { DeXuatChucVuListComponent } from './components/de-xuat-chuc-vuGroup/de-xuat-chuc-vu-list/de-xuat-chuc-vu-list.component';
import { TaoHoSoCongTacComponent } from './components/quan-ly-cong-tac/ho-so-cong-tac/ho-so-cong-tac-create/ho-so-cong-tac-create.component'
import { QuanLyCongTacService } from './services/quan-ly-cong-tac/quan-ly-cong-tac.service'
import { HoSoCongTacListComponent } from './components/quan-ly-cong-tac/ho-so-cong-tac/ho-so-cong-tac-list/ho-so-cong-tac-list.component'
import { ChiTietHoSoCongTacComponent } from './components/quan-ly-cong-tac/ho-so-cong-tac/ho-so-cong-tac-detail/ho-so-cong-tac-detail.component'
import { KeHoachOtDetailComponent } from './components/quan-ly-ot/kehoach-ot-detail/kehoach-ot-detail.component';
import { TaoDeNghiTamHoanUngComponent } from './components/quan-ly-cong-tac/de-nghi-tam-hoan-ung/tao-de-nghi-tam-hoan-ung/tao-de-nghi-tam-hoan-ung.component';
import { ChiTietDeNghiTamHoanUngComponent } from './components/quan-ly-cong-tac/de-nghi-tam-hoan-ung/chi-tiet-de-nghi-tam-hoan-ung/chi-tiet-de-nghi-tam-hoan-ung.component';
import { DanhSachDeNghiTamUngComponent } from './components/quan-ly-cong-tac/de-nghi-tam-hoan-ung/danh-sach-de-nghi-tam-ung/danh-sach-de-nghi-tam-ung.component';
import { DanhSachDeNghiHoanUngComponent } from './components/quan-ly-cong-tac/de-nghi-tam-hoan-ung/danh-sach-de-nghi-hoan-ung/danh-sach-de-nghi-hoan-ung.component';
import { TaoDeXuatCongTacComponent } from './components/quan-ly-cong-tac/de-xuat-cong-tac/tao-de-xuat-cong-tac/tao-de-xuat-cong-tac.component';
import { TaoPhieuDanhGiaComponent } from './components/quan-ly-danh-gia/tao-phieu-danh-gia/tao-phieu-danh-gia.component';
import { ListCauhinhChecklistComponent } from './components/cauhinh-checklist/list-cauhinh-checklist/list-cauhinh-checklist.component';
import { ThemMoiCauhinhChecklistComponent } from './components/cauhinh-checklist/them-moi-cauhinh-checklist/them-moi-cauhinh-checklist.component';
import { QuyLuongComponent } from './components/quan-ly-danh-gia/quy-luong/quy-luong.component';
import { DeXuatCongTacChiTietComponent } from './components/quan-ly-cong-tac/de-xuat-cong-tac/de-xuat-cong-tac-chi-tiet/de-xuat-cong-tac-chi-tiet.component';
import { DanhSachPhieuDanhGiaComponent } from './components/quan-ly-danh-gia/danh-sach-phieu-danh-gia/danh-sach-phieu-danh-gia.component';
import { DanhSachDeXuatCongTacComponent } from "./components/quan-ly-cong-tac/de-xuat-cong-tac/danh-sach-de-xuat-cong-tac/danh-sach-de-xuat-cong-tac.component";
import { ChiTietKyDanhGiaComponent } from './components/quan-ly-danh-gia/chi-tiet-ky-danh-gia/chi-tiet-ky-danh-gia.component';
import { TaoMoiKyDanhGiaComponent } from './components/quan-ly-danh-gia/tao-moi-ky-danh-gia/tao-moi-ky-danh-gia.component';
import { CapPhatTaiSanComponent } from './components/employee-profile/employee-details/cap-phat-tai-san/cap-phat-tai-san.component';
import { ChiTietPhieuDanhGiaComponent } from './components/quan-ly-danh-gia/chi-tiet-phieu-danh-gia/chi-tiet-phieu-danh-gia.component';
import { DanhSachKyDanhGiaComponent } from './components/quan-ly-danh-gia/danh-sach-ky-danh-gia/danh-sach-ky-danh-gia.component';
import { ThucHienDanhGiaComponent } from './components/quan-ly-danh-gia/thuc-hien-danh-gia/thuc-hien-danh-gia.component';
import { BaoCaoNhanSuComponent } from './components/bao-cao-nhan-su/bao-cao-nhan-su.component';
import { ThemMoiLichsuThanhtoanBaohiemComponent } from './components/employee-profile/employee-details/bao-hiem--thue/them-moi-lichsu-thanhtoan-baohiem/them-moi-lichsu-thanhtoan-baohiem.component';
import { BaoHiemXaHoiComponent } from './components/cauhinh-baohiem/bao-hiem-xa-hoi/bao-hiem-xa-hoi.component';
import { BaoHiemLoftcareComponent } from './components/cauhinh-baohiem/bao-hiem-loftcare/bao-hiem-loftcare.component';
import { ThemNamCauHinhComponent } from "./components/cauhinh-baohiem/them-nam-cau-hinh/them-nam-cau-hinh.component";
import { ThemNhomBaoHiemComponent } from "./components/cauhinh-baohiem/them-nhom-bao-hiem/them-nhom-bao-hiem.component";
import { ThemQuyenLoiBaoHiemComponent } from "./components/cauhinh-baohiem/them-quyen-loi-bao-hiem/them-quyen-loi-bao-hiem.component";
import { EmployeeImportDetailComponent } from './components/employee-import-detail/employee-import-detail.component';
import { HttpClientModule } from '@angular/common/http';
import { HopDongImportDetailComponent } from './components/employee-profile/employee-details/hop-dong/hop-dong-import-detail/hop-dong-import-detail.component';


@NgModule({
  imports: [
    SharedModule,
    EmployeeRouting,
    FormsModule,
    ReactiveFormsModule,
    MatChipsModule,
    HttpClientModule,
    NgxLoadingModule.forRoot({})
  ],
  declarations: [
    EmployeeComponent,
    ListComponent,
    DeXuatTangLuongComponent,
    OrgSelectDialogComponent,
    UnitSelectDialogComponent,
    CreateEmployeeRequestComponent,
    EmployeeSalaryListComponent,
    EmployeeTimesheetImportComponent,
    EmployeeCreateComponent,
    ListEmployeeRequestComponent,
    EmployeeCreateSalaryPopupComponent,
    EmployeeSalaryHandmadeImportComponent,
    TeacherSalaryListComponent,
    TeacherSapiGetComponent,
    TeacherSalaryHandmadeImportComponent,
    AssistantSalaryListComponent,
    AssistantSalaryExportComponent,
    AssistantSalaryHandmadeImportComponent,
    DetailEmployeeRequestComponent,
    EmployeeDashboardComponent,
    OrgSelectMultiDialogComponent,
    EmployeeQuitWorkComponent,
    TongQuanNvComponent,
    ThongTinChungComponent,
    ThongTinNhanSuComponent,
    CauHinhPhanQuyenComponent,
    LuongVaTroCapComponent,
    ThongTinDanhGiaComponent,
    NvGhiChuComponent,
    CandidateImportDetailComponent,
    ChienDichCreateComponent,
    ChienDichDashboardComponent,
    ChienDichDetailComponent,
    RecruitmentCampaignInforPanelComponent,
    ChienDichListComponent,
    TuyenDungDashboardComponent,
    TuyenDungCreateComponent,
    TuyenDungDetailComponent,
    TuyenDungListComponent,
    TuyenDungBaoCaoComponent,
    UngVienDashboardComponent,
    UngVienCreateComponent,
    UngVienDetailComponent,
    UngVienListComponent,
    BaoHiemVaThueComponent,
    HocVanVaTuyenDungComponent,
    TaiLieuComponent,
    ThongTinGiaDinhComponent,
    ThongTinHopDongComponent,
    ThongTinKhacComponent,
    ThemMoiTaiLieuComponent,
    ThemMoiHopDongComponent,
    ChiTietHopDongComponent,
    CauHinhBaoHiemComponent,
    ChiTietHopDongComponent,
    TheoDoiChucVuComponent,
    ChiTietHopDongComponent,
    ImportNvDeXuatTangLuongComponent,
    DeXuatTangLuongDetailComponent,
    DeXuatTangLuongListComponent,
    ThemMoiGiaDinhComponent,
    KeHoachOtCreateComponent,
    DanhSachOtComponent,
    DeXuatChucVuComponent,
    ImportNvDeXuatChucVuComponent,
    DeXuatChucVuListComponent,
    DeXuatChucVuDetailComponent,
    TaoHoSoCongTacComponent,
    HoSoCongTacListComponent,
    ChiTietHoSoCongTacComponent,
    KeHoachOtDetailComponent,
    TaoDeNghiTamHoanUngComponent,
    ChiTietDeNghiTamHoanUngComponent,
    DanhSachDeNghiTamUngComponent,
    DanhSachDeNghiHoanUngComponent,
    QuyLuongComponent,
    DeXuatCongTacChiTietComponent,
    TaoDeXuatCongTacComponent,
    ListCauhinhChecklistComponent,
    ThemMoiCauhinhChecklistComponent,
    DanhSachDeXuatCongTacComponent,
    TaoPhieuDanhGiaComponent,
    DanhSachPhieuDanhGiaComponent,
    ChiTietPhieuDanhGiaComponent,
    ChiTietKyDanhGiaComponent,
    DanhSachPhieuDanhGiaComponent,
    CapPhatTaiSanComponent,
    TaoMoiKyDanhGiaComponent,
    DanhSachKyDanhGiaComponent,
    BaoCaoNhanSuComponent,
    DanhSachKyDanhGiaComponent,
    ThucHienDanhGiaComponent,
    ThemMoiLichsuThanhtoanBaohiemComponent,
    BaoHiemXaHoiComponent,
    BaoHiemLoftcareComponent,
    ThemNamCauHinhComponent,
    ThemNhomBaoHiemComponent,
    ThemQuyenLoiBaoHiemComponent,
    EmployeeImportDetailComponent,
    ThemMoiLichsuThanhtoanBaohiemComponent,
    HopDongImportDetailComponent
  ],
  providers: [
    ContactService,
    NoteService,
    ImageUploadService,
    EmployeeAssessmentService,
    EmployeeMonthySalaryService,
    EmployeeInsuranceService,
    EmployeeAllowanceService,
    EmployeeSalaryService,
    BankService,
    CommonService,
    EmployeeService,
    EmployeeRequestService,
    WorkflowService,
    GetPermission,
    OrganizationService,
    CategoryService,
    PositionService,
    PermissionService,
    MatSnackBarConfig,
    EmployeeTimesheetService,
    EmployeeSalaryHandmadeImportService,
    TeacherSalaryService,
    TeacherSalaryHandmadeImportService,
    AssistantExportListService,
    AssistantSalaryHandmadeImportService,
    AssistantSalaryFindService,
    EmployeeListService,
    EmailService,
    DatePipe,
    EmailConfigService,
    ForderConfigurationService,
    ConfirmationService,
    QuanLyCongTacService
  ],
  entryComponents: [
    PopupComponent,
    OrgSelectDialogComponent,
    UnitSelectDialogComponent,
    EmployeeCreateSalaryPopupComponent,
    EmployeeSalaryHandmadeImportComponent,
    TeacherSapiGetComponent,
    TeacherSalaryHandmadeImportComponent,
    AssistantSalaryExportComponent,
    AssistantSalaryHandmadeImportComponent,
    EmployeeTimesheetImportComponent,
    ChonNhieuDvDialogComponent,
    CandidateImportDetailComponent,
    ThemMoiHopDongComponent,
    ChiTietHopDongComponent,
    ThemMoiTaiLieuComponent,
    ThemMoiGiaDinhComponent,
    ThemMoiCauhinhChecklistComponent,
    ThemMoiLichsuThanhtoanBaohiemComponent,
    ThemNamCauHinhComponent,
    ThemNhomBaoHiemComponent,
    ThemQuyenLoiBaoHiemComponent
  ],
  exports: [
    OrgSelectDialogComponent
  ],
  bootstrap: [EmployeeTimesheetImportComponent]
})
export class EmployeeModule { }
