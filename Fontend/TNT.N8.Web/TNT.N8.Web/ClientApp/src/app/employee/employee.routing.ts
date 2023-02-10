import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { EmployeeComponent } from "./employee.component";
import { AuthGuard } from "../shared/guards/auth.guard";

import { ListComponent } from "./components/list/list.component";
import { EmployeeCreateComponent } from "./components/employee-profile/employee-create/employee-create.component";
import { CreateEmployeeRequestComponent } from "./components/employee-request/create-employee-request/create-employee-request.component";
import { ListEmployeeRequestComponent } from "./components/employee-request/list-employee-request/list-employee-request.component";
import { EmployeeSalaryListComponent } from "./components/employee-salary/employee-salary-list/employee-salary-list.component";
import { EmployeeTimesheetImportComponent } from "./components/employee-salary/employee-timesheet-import/employee-timesheet-import.component";
import { TeacherSalaryListComponent } from "./components/employee-salary/teacher-salary-list/teacher-salary-list.component";
import { AssistantSalaryListComponent } from "./components/employee-salary/assistant-salary-list/assistant-salary-list.component";
import { DetailEmployeeRequestComponent } from "./components/employee-request/detail-employee-request/detail-employee-request.component";
import { EmployeeDashboardComponent } from "./components/employee-dashboard/employee-dashboard.component";
import { EmployeeQuitWorkComponent } from "./components/employee-quit-work/employee-quit-work.component";
import { TongQuanNvComponent } from "./components/employee-profile/employee-details/tong-quan-nv/tong-quan-nv.component";
import { DeXuatTangLuongComponent } from './components/de-xuat-tang-luongGroup/de-xuat-tang-luong/de-xuat-tang-luong.component';
import { DeXuatTangLuongDetailComponent } from './components/de-xuat-tang-luongGroup/de-xuat-tang-luong-detail/de-xuat-tang-luong-detail.component';
import { DeXuatTangLuongListComponent } from './components/de-xuat-tang-luongGroup/de-xuat-tang-luong-list/de-xuat-tang-luong-list.component';


import { ChienDichCreateComponent } from "./components/chien-dich-create/chien-dich-create.component";
import { ChienDichDashboardComponent } from "./components/chien-dich-dashboard/chien-dich-dashboard.component";
import { ChienDichDetailComponent } from "./components/chien-dich-detail/chien-dich-detail.component";
import { ChienDichListComponent } from "./components/chien-dich-list/chien-dich-list.component";
import { TuyenDungDashboardComponent } from "./components/tuyen-dung-dashboard/tuyen-dung-dashboard.component";
import { TuyenDungCreateComponent } from "./components/tuyen-dung-create/tuyen-dung-create.component";
import { TuyenDungDetailComponent } from "./components/tuyen-dung-detail/tuyen-dung-detail.component";
import { TuyenDungListComponent } from "./components/tuyen-dung-list/tuyen-dung-list.component";
import { TuyenDungBaoCaoComponent } from "./components/tuyen-dung-bao-cao/tuyen-dung-bao-cao.component";
import { UngVienDashboardComponent } from "./components/ung-vien-dashboard/ung-vien-dashboard.component";
import { UngVienCreateComponent } from "./components/ung-vien-create/ung-vien-create.component";
import { UngVienDetailComponent } from "./components/ung-vien-detail/ung-vien-detail.component";
import { UngVienListComponent } from "./components/ung-vien-list/ung-vien-list.component";
import { CauHinhBaoHiemComponent } from "./components/cauhinh-baohiem/cauhinh-baohiem/cauhinh-baohiem.component";
import { KeHoachOtCreateComponent } from "./components/quan-ly-ot/kehoach-ot-create/kehoach-ot-create.component";
import { DanhSachOtComponent } from "./components/quan-ly-ot/danh-sach-ot/danh-sach-ot.component";
import { DeXuatChucVuComponent } from "./components/de-xuat-chuc-vuGroup/de-xuat-chuc-vu/de-xuat-chuc-vu.component";
import { DeXuatChucVuDetailComponent } from "./components/de-xuat-chuc-vuGroup/de-xuat-chuc-vu-detail/de-xuat-chuc-vu-detail.component";
import { DeXuatChucVuListComponent } from "./components/de-xuat-chuc-vuGroup/de-xuat-chuc-vu-list/de-xuat-chuc-vu-list.component";
import { TaoHoSoCongTacComponent } from "./components/quan-ly-cong-tac/ho-so-cong-tac/ho-so-cong-tac-create/ho-so-cong-tac-create.component";
import { KeHoachOtDetailComponent } from './components/quan-ly-ot/kehoach-ot-detail/kehoach-ot-detail.component';
import { DeXuatCongTacChiTietComponent } from './components/quan-ly-cong-tac/de-xuat-cong-tac/de-xuat-cong-tac-chi-tiet/de-xuat-cong-tac-chi-tiet.component';
import { HoSoCongTacListComponent } from './components/quan-ly-cong-tac/ho-so-cong-tac/ho-so-cong-tac-list/ho-so-cong-tac-list.component';
import { ChiTietHoSoCongTacComponent } from './components/quan-ly-cong-tac/ho-so-cong-tac/ho-so-cong-tac-detail/ho-so-cong-tac-detail.component';
import { TaoDeNghiTamHoanUngComponent } from './components/quan-ly-cong-tac/de-nghi-tam-hoan-ung/tao-de-nghi-tam-hoan-ung/tao-de-nghi-tam-hoan-ung.component';
import { ChiTietDeNghiTamHoanUngComponent } from './components/quan-ly-cong-tac/de-nghi-tam-hoan-ung/chi-tiet-de-nghi-tam-hoan-ung/chi-tiet-de-nghi-tam-hoan-ung.component';
import { DanhSachDeNghiTamUngComponent } from './components/quan-ly-cong-tac/de-nghi-tam-hoan-ung/danh-sach-de-nghi-tam-ung/danh-sach-de-nghi-tam-ung.component';
import { DanhSachDeNghiHoanUngComponent } from './components/quan-ly-cong-tac/de-nghi-tam-hoan-ung/danh-sach-de-nghi-hoan-ung/danh-sach-de-nghi-hoan-ung.component';
import { QuyLuongComponent } from './components/quan-ly-danh-gia/quy-luong/quy-luong.component';
import { TaoDeXuatCongTacComponent } from './components/quan-ly-cong-tac/de-xuat-cong-tac/tao-de-xuat-cong-tac/tao-de-xuat-cong-tac.component';
import { ListCauhinhChecklistComponent } from "./components/cauhinh-checklist/list-cauhinh-checklist/list-cauhinh-checklist.component";
import { TaoPhieuDanhGiaComponent } from "./components/quan-ly-danh-gia/tao-phieu-danh-gia/tao-phieu-danh-gia.component";
import { DanhSachPhieuDanhGiaComponent } from "./components/quan-ly-danh-gia/danh-sach-phieu-danh-gia/danh-sach-phieu-danh-gia.component";
// import { TaoPhieuDanhGiaComponent } from "./components/tao-phieu-danh-gia/tao-phieu-danh-gia.component";
// import { DanhSachKyDanhGiaComponent } from "./components/quan-ly-danh-gia/danh-sach-ky-danh-gia/danh-sach-ky-danh-gia.component";
import { ChiTietKyDanhGiaComponent } from "./components/quan-ly-danh-gia/chi-tiet-ky-danh-gia/chi-tiet-ky-danh-gia.component";
import { TaoMoiKyDanhGiaComponent } from "./components/quan-ly-danh-gia/tao-moi-ky-danh-gia/tao-moi-ky-danh-gia.component";


import { DanhSachDeXuatCongTacComponent } from "./components/quan-ly-cong-tac/de-xuat-cong-tac/danh-sach-de-xuat-cong-tac/danh-sach-de-xuat-cong-tac.component";
import { ChiTietPhieuDanhGiaComponent } from "./components/quan-ly-danh-gia/chi-tiet-phieu-danh-gia/chi-tiet-phieu-danh-gia.component";
import { DanhSachKyDanhGiaComponent } from "./components/quan-ly-danh-gia/danh-sach-ky-danh-gia/danh-sach-ky-danh-gia.component";
import { ThucHienDanhGiaComponent } from "./components/quan-ly-danh-gia/thuc-hien-danh-gia/thuc-hien-danh-gia.component";
import { BaoCaoNhanSuComponent } from "./components/bao-cao-nhan-su/bao-cao-nhan-su.component";

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: "",
        component: EmployeeComponent,
        children: [
          {
            path: "dashboard",
            component: EmployeeDashboardComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "list",
            component: ListComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "create",
            component: EmployeeCreateComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "detail",
            component: TongQuanNvComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "request/list",
            component: ListEmployeeRequestComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "request/create",
            component: CreateEmployeeRequestComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "request/detail",
            component: DetailEmployeeRequestComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "employee-salary/list",
            component: EmployeeSalaryListComponent,
            canActivate: [AuthGuard],
          },
          //,
          //{
          //  path: 'employee-salary/import',
          //  component: EmployeeTimesheetImportComponent,
          //  canActivate: [AuthGuard]
          //}
          //,
          {
            path: "teacher-salary/list",
            component: TeacherSalaryListComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "assistant-salary/list",
            component: AssistantSalaryListComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "employee-quit-work",
            component: EmployeeQuitWorkComponent,
            canActivate: [AuthGuard],
          },
          {
            path: 'cauhinh-baohiem',
            component: CauHinhBaoHiemComponent,
            canActivate: [AuthGuard]
          },
          // {
          //   path: 'employee-detail',
          //   component: TongQuanNvComponent,
          //   canActivate: [AuthGuard]
          // },
          {
            path: "tao-chien-dich",
            component: ChienDichCreateComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "dashboard-chien-dich",
            component: ChienDichDashboardComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "chi-tiet-chien-dich",
            component: ChienDichDetailComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "danh-sach-chien-dich",
            component: ChienDichListComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "dashboard-tuyen-dung",
            component: TuyenDungDashboardComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "tao-cong-viec-tuyen-dung",
            component: TuyenDungCreateComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "chi-tiet-cong-viec-tuyen-dung",
            component: TuyenDungDetailComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "danh-sach-cong-viec-tuyen-dung",
            component: TuyenDungListComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "bao-cao-cong-viec-tuyen-dung",
            component: TuyenDungBaoCaoComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "dashboard-ung-vien",
            component: UngVienDashboardComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "tao-ung-vien",
            component: UngVienCreateComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "chi-tiet-ung-vien",
            component: UngVienDetailComponent,
            canActivate: [AuthGuard],
          },
          {
            path: "danh-sach-ung-vien",
            component: UngVienListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'tao-de-xuat-tang-luong',
            component: DeXuatTangLuongComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'de-xuat-tang-luong-detail',
            component: DeXuatTangLuongDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'danh-sach-de-xuat-tang-luong',
            component: DeXuatTangLuongListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: "kehoach-ot-create",
            component: KeHoachOtCreateComponent,
            canActivate: [AuthGuard],
          },
          {
            path: 'danh-sach-de-xuat-ot',
            component: DanhSachOtComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'tao-de-xuat-chuc-vu',
            component: DeXuatChucVuComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'danh-sach-de-xuat-chuc-vu',
            component: DeXuatChucVuListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'de-xuat-chuc-vu-detail',
            component: DeXuatChucVuDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'de-xuat-cong-tac-chi-tiet',
            component: DeXuatCongTacChiTietComponent,
            canActivate: [AuthGuard]
          },
          {
            path: "kehoach-ot-detail",
            component: KeHoachOtDetailComponent,
            canActivate: [AuthGuard],
          },

          {
            path: 'tao-ho-so-cong-tac',
            component: TaoHoSoCongTacComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'danh-sach-ho-so-cong-tac',
            component: HoSoCongTacListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'chi-tiet-ho-so-cong-tac',
            component: ChiTietHoSoCongTacComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'tao-de-nghi-tam-hoan-ung',
            component: TaoDeNghiTamHoanUngComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'chi-tiet-de-nghi-tam-hoan-ung',
            component: ChiTietDeNghiTamHoanUngComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'danh-sach-de-nghi-tam-ung',
            component: DanhSachDeNghiTamUngComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'danh-sach-de-nghi-hoan-ung',
            component: DanhSachDeNghiHoanUngComponent,
            canActivate: [AuthGuard]
          },

          {
            path: 'quy-luong',
            component: QuyLuongComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'tao-de-xuat-cong-tac',
            component: TaoDeXuatCongTacComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'tao-phieu-danh-gia',
            component: TaoPhieuDanhGiaComponent,
            canActivate: [AuthGuard]
          },


          {
            path: 'cauhinh-checklist',
            component: ListCauhinhChecklistComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'danh-sach-phieu-danh-gia',
            component: DanhSachPhieuDanhGiaComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'danh-sach-de-xuat-cong-tac',
            component: DanhSachDeXuatCongTacComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'chi-tiet-ky-danh-gia',
            component: ChiTietKyDanhGiaComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'chi-tiet-phieu-danh-gia',
            component: ChiTietPhieuDanhGiaComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'tao-moi-ky-danh-gia',
            component: TaoMoiKyDanhGiaComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'danh-sach-ky-danh-gia',
            component: DanhSachKyDanhGiaComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'thuc-hien-danh-gia',
            component: ThucHienDanhGiaComponent,
            canActivate: [AuthGuard]
          },

          {
            path: 'bao-cao-nhan-su',
            component: BaoCaoNhanSuComponent,
            canActivate: [AuthGuard]
          },
        ]
      }
    ])
  ],
  exports: [RouterModule],
})
export class EmployeeRouting { }
