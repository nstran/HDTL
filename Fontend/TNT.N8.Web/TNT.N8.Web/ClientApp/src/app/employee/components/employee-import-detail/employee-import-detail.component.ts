import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, FormArray, FormBuilder, Validators } from '@angular/forms'

//SERVICES
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';

import { EmployeeService } from '../../services/employee.service';
import { CommonService } from '../../../shared/services/common.service';




class importEmployeeModel {
  EmployeeCode: string;
  EmployeeName: string;
  HoTenTiengAnh: string;
  FirstName: string;
  LastName: string;
  DateOfBirth: Date;
  Gender: string;
  OtherPhone: string;
  Phone: string;


  Email: string;
  model: string;
  QuocTich: string;
  DanToc: string;
  TonGiao: number;
  CodeMayChamCong: string;
  TenMayChamCong: string;


  MaTest: string;
  DiemTest: string;

  GradeTestingName: string;
  GradeTestingId: any;

  DeptCodeValueName: string;
  DeptCodeValueId: any;

  SubCode1ValueName: string;
  SubCode1ValueId: any;

  SubCode2ValueName: string;
  SubCode2ValueId: any;

  OrganizationIdName: string;
  OrganizationId: any;

  ProvinceIdName: string;
  ProvinceId: any;

  SocialInsuranceNumber: string;
  MaSoThueCaNhan: string;
  HealthInsuranceNumber: string;

  IdentityId: string;
  IdentityIddateOfIssue: Date;
  IdentityIdplaceOfIssue: string;
  NoiCapCmndtiengAnh: string;
  HoKhauThuongTruTv: string;
  HoKhauThuongTruTa: string;
  Address: string;
  AddressTiengAnh: string;

  BankCode: string;
  BankOwnerName: string;
  BankAccount: string;
  BankName: string;
  BienSo: string;
  LoaiXe: string;

  DienThoaiEmailLienHe: string;
  KinhNghiemLamViec: string;

  KyNangTayNgheName: string;
  KyNangTayNgheId: any;

  TomTatHocVan: string;
  ChuyenNganhHoc: string;
  TenTruongHocCaoNhat: string;

  BangCapCaoNhatDatDuocIdName: string;
  BangCapCaoNhatDatDuocId: any;

  PhuongThucTuyenDungIdName: string;
  PhuongThucTuyenDungId: any;

  CoPhi: string;
  MucPhi: number;

  NguonTuyenDungIdName: string;
  NguonTuyenDungId: any;

  SoPhepConLai: number;
  SoPhepDaSuDung: number;

  listStatus = [];
  isValid: boolean;
}

interface ResultDialog {
  status: boolean,
  statusImport: boolean
}

class Note {
  public code: string;
  public name: string;
}

class EmployeeEntityModel {
  EmployeeCode: string;
  EmployeeName: string;
  HoTenTiengAnh: string;
  FirstName: string;
  LastName: string;
  DateOfBirth: Date;
  Gender: string;
  OtherPhone: string;
  Phone: string;

  Email: string;
  model: string;
  QuocTich: string;
  DanToc: string;
  TonGiao: number;
  CodeMayChamCong: string;
  TenMayChamCong: string;


  MaTest: string;
  DiemTest: string;

  GradeTesting: any;

  DeptCodeValue: any;

  SubCode1: any;

  SubCode2: any;

  OrganizationId: any;

  ProvinceId: any;

  SocialInsuranceNumber: string;
  MaSoThueCaNhan: string;
  HealthInsuranceNumber: string;

  IdentityId: string;
  IdentityIddateOfIssue: Date;
  IdentityIdplaceOfIssue: string;
  NoiCapCmndtiengAnh: string;
  HoKhauThuongTruTv: string;
  HoKhauThuongTruTa: string;
  Address: string;
  AddressTiengAnh: string;

  BankCode: string;
  BankOwnerName: string;
  BankAccount: string;
  BankName: string;
  BienSo: string;
  LoaiXe: string;

  // DienThoaiEmailLienHe: string;
  // KinhNghiemLamViec: string;

  KyNangTayNghe: any;

  TomTatHocVan: string;
  ChuyenNganhHoc: string;
  TenTruongHocCaoNhat: string;

  BangCapCaoNhatDatDuocId: any;

  PhuongThucTuyenDungId: any;

  CoPhi: boolean;
  MucPhi: number;

  NguonTuyenDungId: any;

  SoPhepConLai: number;
  SoPhepDaSuDung: number;

}


@Component({
  selector: 'app-employee-import-detail',
  templateUrl: './employee-import-detail.component.html',
  styleUrls: ['./employee-import-detail.component.css']
})
export class EmployeeImportDetailComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;
  @ViewChild('myTable') myTable: Table;

  today: Date = new Date();

  listNote: Array<Note> = [
    /* required fields */
    { code: "required_UserName", name: "Nhập mã nhân viên" },
    { code: "required_nameTV", name: "Nhập họ tên tiếng việt" },
    { code: "required_nameTA", name: "Nhập họ tên tiếng anh" },
    { code: "required_firstName", name: "Nhập họ" },
    { code: "required_lastName", name: "Nhập tên" },
    { code: "required_birthDay", name: "Nhập ngày tháng năm sinh" },
    { code: "required_gioiTinh", name: "Nhập giới tính" },
    { code: "required_sdtCaNhan", name: "Nhập SDT cá nhân" },
    { code: "required_emailCaNhan", name: "Nhập email cá nhân" },

    { code: "required_deptCode", name: "Nhập DEPT-CODE" },
    { code: "required_SUB_CODE1", name: "Nhập SUB-CODE1" },
    { code: "required_SUB_CODE2", name: "Nhập SUB-CODE2" },

    // check mã code
    { code: "wrong_GradeTestingName", name: "Grade Testing không đúng " },
    { code: "wrong_DeptCodeValueName", name: "Dept Code không đúng " },
    { code: "wrong_SubCode1ValueName", name: "SubCode1 không đúng " },
    { code: "wrong_SubCode2ValueName", name: "SubCode2 không đúng " },
    { code: "wrong_OrganizationIdName", name: "Phòng ban không đúng " },
    { code: "wrong_ProvinceIdName", name: "Địa điểm làm việc không đúng " },
    { code: "wrong_BangCapCaoNhatDatDuocIdName", name: "Bằng cấp cao nhất không đúng " },
    { code: "wrong_PhuongThucTuyenDungIdName", name: "Phương thức tuyển dụng không đúng " },
    { code: "wrong_NguonTuyenDungIdName", name: "Nguồn tuyển dụng không đúng " },
    { code: "wrong_KyNangTayNghe", name: "Kỹ năng tài nghề không đúng " },

    //Check mã tài sản trong DB
    { code: "existEmpCode_inDB", name: "Mã nhân viên đã tồn tại trên hệ thống" },
    { code: "existPhone_inDB", name: "Số điện thoại cá nhân đã tồn tại trên hệ thống" },
    { code: "existEmail_inDB", name: "Email cá nhân đã tồn tại trên hệ thống" },
    { code: "existCodeMayChamCong_inDB", name: "Code máy chấm công đã tồn tại trên hệ thống" },

    //Check mã tài sản trong list
    { code: "existUserName_inList", name: "Tên tài khoản đã tồn tại trong danh sách nhập excel" },
    { code: "existPhone_inList", name: "Số điện thoại cá nhân đã tồn tại trong danh sách nhập excel" },
    { code: "existEmail_inList", name: "Email cá nhân đã tồn tại trong danh sách nhập excel" },
    { code: "existCodeMayChamCong_inList", name: "Code máy chấm công đã tồn tại trong danh sách nhập excel" },

    //Check số lơn hơn 0
    { code: "DiemTest_positive", name: "Điểm test phải lơn hơn 0" },
    { code: "SoPhepConLai_positive", name: "Số phép còn lại phải lơn hơn 0" },
    { code: "SoPhepDaSuDung_positive", name: "Số phép đã sử dụng phải lơn hơn 0" },
  ]

  listTaiSanImport: Array<importEmployeeModel> = [];

  //table
  rows: number = 10;
  columns: Array<any> = [];
  selectedColumns: Array<any> = [];
  selectedEmployeeImport: Array<importEmployeeModel> = [];

  ListCapBac: Array<any> = [];
  ListDeptCode: Array<any> = [];
  ListSubCode1: Array<any> = [];
  ListSubCode2: Array<any> = [];
  ListPhongBan: Array<any> = [];
  ListBangCap: Array<any> = [];
  ListPTTD: Array<any> = [];
  ListKenhTd: Array<any> = [];
  ListProvince: Array<any> = [];
  ListEmployeeExportDate: Array<any> = [];
  ListKyNangTayNghe: Array<any> = [];

  //In list
  listUserName: Array<any> = [];
  listPhone: Array<any> = [];
  listEmail: Array<any> = [];
  listCodeMayChamCong: Array<any> = [];

  //In DB
  listEmpCodeDB: Array<any> = [];
  listPhoneDB: Array<any> = [];
  listEmailDB: Array<any> = [];
  listCodeMayChamCongDB: Array<any> = [];

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private messageService: MessageService,
    public employeeService: EmployeeService,
    public commonService: CommonService,
  ) {
    if (this.config.data) {
      this.listTaiSanImport = this.config.data.listTaiSanImport;
    }
  }

  async ngOnInit() {
    this.initTable();
    await this.getMasterdata();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  initTable() {
    // numberInt
    // text
    this.columns = [
      { field: 'EmployeeCode', header: 'Mã nhân viên', textAlign: 'left', display: 'table-cell', width: '150px', type: 'text', isRequired: true, isList: false },
      { field: 'EmployeeName', header: 'Họ tên tiếng Việt', textAlign: 'left', display: 'table-cell', width: '150px', type: 'text', isRequired: true, isList: false },
      { field: 'HoTenTiengAnh', header: 'Họ và tên tiếng Anh', textAlign: 'left', display: 'table-cell', width: '150px', type: 'text', isRequired: true, isList: false },
      { field: 'FirstName', header: 'First Name', textAlign: 'center', display: 'table-cell', width: '120px', type: 'text', isRequired: true, isList: false },
      { field: 'LastName', header: 'Last Name', textAlign: 'center', display: 'table-cell', width: '100px', type: 'text', isRequired: true, isList: false },
      { field: 'DateOfBirth', header: 'Ngày tháng năm sinh', textAlign: 'center', display: 'table-cell', width: '100px', type: 'date', isRequired: true, isList: false },

      { field: 'Gender', header: 'Giới tính', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: true, isList: false },
      { field: 'OtherPhone', header: 'Số điện thoại nhà riêng', textAlign: 'left', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'Phone', header: 'Số Di động cá nhân', textAlign: 'left', display: 'table-cell', width: '120px', type: 'text', isRequired: true, isList: false },
      { field: 'Email', header: 'Email - Private', textAlign: 'left', display: 'table-cell', width: '100px', type: 'text', isRequired: true, isList: false },

      { field: 'QuocTich', header: 'Quốc tịch', textAlign: 'center', display: 'table-cell', width: '100px', type: 'text', isRequired: false, isList: false },
      { field: 'DanToc', header: 'Dân tộc', textAlign: 'center', display: 'table-cell', width: '100px', type: 'text', isRequired: false, isList: false },
      { field: 'TonGiao', header: 'Tôn giáo', textAlign: 'center', display: 'table-cell', width: '100px', type: 'text', isRequired: false, isList: false },


      { field: 'CodeMayChamCong', header: 'Code máy chấm công', textAlign: 'center', display: 'table-cell', width: '100px', type: 'text', isRequired: false, isList: false },
      { field: 'TenMayChamCong', header: 'Họ tên máy chấm công', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'MaTest', header: 'Mã testing', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'DiemTest', header: 'Điểm test', textAlign: 'center', display: 'table-cell', width: '120px', type: 'text', isRequired: false, isList: false },
      { field: 'GradeTestingName', header: 'GRADE TESTING', textAlign: 'center', display: 'table-cell', width: '100px', type: 'text', isRequired: false, isList: true },

      { field: 'DeptCodeValueName', header: 'DEPT-CODE', textAlign: 'left', display: 'table-cell', width: '150px', type: 'text', isRequired: true, isList: true },
      { field: 'SubCode1ValueName', header: 'SUB-CODE1', textAlign: 'left', display: 'table-cell', width: '150px', type: 'text', isRequired: true, isList: true },
      { field: 'SubCode2ValueName', header: 'SUB-CODE2', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: true, isList: true },

      { field: 'OrganizationIdName', header: 'Phòng ban', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: true },
      { field: 'ProvinceIdName', header: 'Địa điểm làm việc', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: true, isList: true },
      { field: 'SocialInsuranceNumber', header: 'Số sổ BHXH', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },


      { field: 'MaSoThueCaNhan', header: 'Mã số thuế', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'HealthInsuranceNumber', header: 'MÃ THẺ BHYT', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'IdentityId', header: 'Số CMTND/ Hộ chiếu', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'IdentityIddateOfIssue', header: 'Ngày - Tháng - Năm cấp', textAlign: 'center', display: 'table-cell', width: '150px', type: 'date', isRequired: false, isList: false },


      { field: 'IdentityIdplaceOfIssue', header: 'Nơi cấp_ Tiếng việt', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'NoiCapCmndtiengAnh', header: 'Nơi cấp tiếng Anh', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'HoKhauThuongTruTv', header: 'Hộ khẩu thường trú_Tiếng Việt', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'HoKhauThuongTruTa', header: 'Hộ khẩu thường trú_Tiếng Anh', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },

      { field: 'Address', header: 'Nơi ở hiện tại_Tiếng Việt', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'AddressTiengAnh', header: 'Nơi ở hiện tại_Tiếng Anh', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'BankCode', header: 'BANK CODE', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'BankOwnerName', header: 'Chủ tài khoản ngân hàng', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'BankAccount', header: 'Số tài khoản', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'BankName', header: 'Tên ngân hàng & Địa chỉ ngân hàng', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'BienSo', header: 'Biển số xe ', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'LoaiXe', header: 'Loại xe', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'DienThoaiEmailLienHe', header: 'Điện thoại và email liên hệ', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },

      { field: 'KinhNghiemLamViec', header: 'Kinh nghiệm làm việc', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'KyNangTayNgheName', header: 'Kỹ năng, tay nghề', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: true },
      { field: 'TomTatHocVan', header: 'Tóm tắt học vấn', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'ChuyenNganhHoc', header: 'Chuyên ngành học', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'TenTruongHocCaoNhat', header: 'Tên trường học cao nhất tiếng Việt', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'BangCapCaoNhatDatDuocIdName', header: 'BẰNG CẤP CAO NHẤT ĐẠT ĐƯỢC Tiếng Việt', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: true },

      { field: 'PhuongThucTuyenDungIdName', header: 'Phương thức tuyển dụng', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: true },
      { field: 'CoPhi', header: 'Có phí/ miễn phí', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'MucPhi', header: 'Mức phí', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'NguonTuyenDungIdName', header: 'Qua nguồn nào', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: true },
      { field: 'SoPhepConLai', header: 'Số  phép còn lại', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },
      { field: 'SoPhepDaSuDung', header: 'Số phép đã sử dụng', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false, isList: false },

      { field: 'listStatus', header: 'Trạng thái', textAlign: 'left', display: 'table-cell', width: '250px', type: 'listStatus' },
    ];
  }

  async getMasterdata() {
    let result: any = await this.employeeService.getMasterDateImportEmployee(1);
    this.loading = false;
    if (result.statusCode == 200) {
      this.ListCapBac = result.listCapBac;
      this.ListDeptCode = result.listDeptCode;
      this.ListSubCode1 = result.listSubCode1;
      this.ListSubCode2 = result.listSubCode2;
      this.ListPhongBan = result.listPhongBan;
      this.ListKyNangTayNghe = result.listKyNangTayNghe;
      this.ListBangCap = result.listBangCap;
      this.ListPTTD = result.listPTTD;
      this.ListKenhTd = result.listKenhTd;
      this.ListProvince = result.listProvince;

      this.listEmpCodeDB = result.listEmpCode;
      this.listPhoneDB = result.listPhone;
      this.listEmailDB = result.listEmail;
      this.listCodeMayChamCongDB = result.listCodeMayChamCong;

      this.listUserName = this.listTaiSanImport.map(x => x.EmployeeCode.toUpperCase().trim());
      this.listPhone = this.listTaiSanImport.map(x => x.Phone.toUpperCase().trim());
      this.listEmail = this.listTaiSanImport.map(x => x.Email.toUpperCase().trim());
      this.listCodeMayChamCong = this.listTaiSanImport.map(x => x.CodeMayChamCong.toUpperCase().trim());
      this.checkStatus(true);
    }
    else {
      let mgs = { severity: 'error', summary: 'Thông báo', detail: result.message };
      this.showMessage(mgs);
    }
  }

  selectRow(checkValue) {
    if (checkValue) {
      this.selectedEmployeeImport = this.listTaiSanImport.filter(e => e.isValid);
    } else {
      this.selectedEmployeeImport = [];
    }
  }

  checkStatus(autoAdd: boolean) {

    this.listUserName = this.listTaiSanImport.map(x => x.EmployeeCode.toUpperCase().trim());
    this.listPhone = this.listTaiSanImport.map(x => x.Phone.toUpperCase().trim());
    this.listEmail = this.listTaiSanImport.map(x => x.Email.toUpperCase().trim());
    this.listCodeMayChamCong = this.listTaiSanImport.map(x => x.CodeMayChamCong.toUpperCase().trim());

    this.listTaiSanImport.forEach(employee => {
      employee.listStatus = [];
      employee.isValid = true;

      employee.NguonTuyenDungIdName = employee.NguonTuyenDungId != null ? employee.NguonTuyenDungId.categoryName : employee.NguonTuyenDungIdName;
      employee.PhuongThucTuyenDungIdName = employee.PhuongThucTuyenDungId != null ? employee.PhuongThucTuyenDungId.categoryName : employee.PhuongThucTuyenDungIdName;
      employee.BangCapCaoNhatDatDuocIdName = employee.BangCapCaoNhatDatDuocId != null ? employee.BangCapCaoNhatDatDuocId.categoryName : employee.BangCapCaoNhatDatDuocIdName;
      employee.KyNangTayNgheName = employee.KyNangTayNgheId != null ? employee.KyNangTayNgheId.name : employee.KyNangTayNgheName;
      employee.ProvinceIdName = employee.ProvinceId != null ? employee.ProvinceId.provinceName : employee.ProvinceIdName;
      employee.OrganizationIdName = employee.OrganizationId != null ? employee.OrganizationId.organizationName : employee.OrganizationIdName;
      employee.SubCode2ValueName = employee.SubCode2ValueId != null ? employee.SubCode2ValueId.name : employee.SubCode2ValueName;
      employee.SubCode1ValueName = employee.SubCode1ValueId != null ? employee.SubCode1ValueId.name : employee.SubCode1ValueName;
      employee.DeptCodeValueName = employee.DeptCodeValueId != null ? employee.DeptCodeValueId.name : employee.DeptCodeValueName;
      employee.GradeTestingName = employee.GradeTestingId != null ? employee.GradeTestingId.categoryName : employee.GradeTestingName;

      //Check duplicate in list
      if (employee.EmployeeCode) {
        let check = this.listUserName.filter(x => x.toUpperCase().trim() == employee.EmployeeCode.toUpperCase().trim());
        if (check.length > 1) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "existUserName_inList")];
          employee.isValid = false;
        }
      }

      if (employee.Phone) {
        let check = this.listPhone.filter(x => x.toUpperCase().trim() == employee.Phone.toUpperCase().trim());
        if (check.length > 1) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "existPhone_inList")];
          employee.isValid = false;
        }
      }

      if (employee.Email) {
        let check = this.listEmail.filter(x => x.toUpperCase().trim() == employee.Email.toUpperCase().trim());
        if (check.length > 1) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "existEmail_inList")];
          employee.isValid = false;
        }
      }

      if (employee.CodeMayChamCong) {
        let check = this.listCodeMayChamCong.filter(x => x.toUpperCase().trim() == employee.CodeMayChamCong.toUpperCase().trim());
        if (check.length > 1) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "existCodeMayChamCong_inList")];
          employee.isValid = false;
        }
      }

      //Check duplicate in DB
      if (employee.EmployeeCode) {
        let check = this.listEmpCodeDB.filter(x => x.toUpperCase().trim() == employee.EmployeeCode.toUpperCase().trim());
        if (check.length != 0) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "existEmpCode_inDB")];
          employee.isValid = false;
        }
      }

      if (employee.Phone) {
        let check = this.listPhoneDB.filter(x => x.toUpperCase().trim() == employee.Phone.toUpperCase().trim());
        if (check.length != 0) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "existPhone_inDB")];
          employee.isValid = false;
        }
      }

      if (employee.Email) {
        let check = this.listEmailDB.filter(x => x.toUpperCase().trim() == employee.Email.toUpperCase().trim());
        if (check.length != 0) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "existEmail_inDB")];
          employee.isValid = false;
        }
      }

      if (employee.CodeMayChamCong) {
        let check = this.listCodeMayChamCongDB.filter(x => x.toUpperCase().trim() == employee.CodeMayChamCong.toUpperCase().trim());
        if (check.length != 0) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "existCodeMayChamCong_inDB")];
          employee.isValid = false;
        }
      }

      /* required fields */
      if (!employee?.EmployeeCode.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_UserName")];
        employee.isValid = false;
      }

      if (!employee?.EmployeeName.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_nameTV")];
        employee.isValid = false;
      }

      if (!employee?.HoTenTiengAnh.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_nameTA")];
        employee.isValid = false;
      }

      if (!employee?.FirstName.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_firstName")];
        employee.isValid = false;
      }

      if (!employee?.LastName.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_lastName")];
        employee.isValid = false;
      }


      if (!employee?.DateOfBirth) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_birthDay")];
        employee.isValid = false;
      }

      if (!employee?.Gender.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_gioiTinh")];
        employee.isValid = false;
      }

      if (!employee?.Phone.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_sdtCaNhan")];
        employee.isValid = false;
      }

      if (!employee?.Email.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_emailCaNhan")];
        employee.isValid = false;
      }

      if (!employee?.DeptCodeValueName.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_deptCode")];
        employee.isValid = false;
      }

      if (!employee?.SubCode1ValueName.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_SUB_CODE1")];
        employee.isValid = false;
      }

      if (!employee?.SubCode2ValueName.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_SUB_CODE2")];
        employee.isValid = false;
      }

      if (!employee?.ProvinceIdName.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_diaDiemLamViec")];
        employee.isValid = false;
      }

      if (!employee?.SubCode2ValueName.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_thoiGianKhauHao")];
        employee.isValid = false;
      }
      if (!employee?.SubCode2ValueName.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_thoiDiemBdTinhKhauHao")];
        employee.isValid = false;
      }

      //check tên
      if (employee.GradeTestingName) {
        let GradeTesting = this.ListCapBac.find(x => x.categoryName.toLowerCase().trim() == employee.GradeTestingName.toLowerCase().trim());
        if (!GradeTesting) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "wrong_GradeTestingName")];
          employee.isValid = false;
        } else {
          employee.GradeTestingId = GradeTesting;
        }
      }


      if (employee.DeptCodeValueName) {
        let DeptCodeValue = this.ListDeptCode.find(x => x.name.toLowerCase().trim() == employee.DeptCodeValueName.toLowerCase().trim());
        if (!DeptCodeValue) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "wrong_DeptCodeValueName")];
          employee.isValid = false;
        } else {
          employee.DeptCodeValueId = DeptCodeValue;
        }
      }


      if (employee.SubCode1ValueName) {
        let SubCode1Value = this.ListSubCode1.find(x => x.name.toLowerCase().trim() == employee.SubCode1ValueName.toLowerCase().trim());
        if (!SubCode1Value) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "wrong_SubCode1ValueName")];
          employee.isValid = false;
        } else {
          employee.SubCode1ValueId = SubCode1Value;
        }
      }


      if (employee.SubCode2ValueName) {
        let SubCode2Value = this.ListSubCode2.find(x => x.name.toLowerCase().trim() == employee.SubCode2ValueName.toLowerCase().trim());
        if (!SubCode2Value) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "wrong_SubCode2ValueName")];
          employee.isValid = false;
        } else {
          employee.SubCode2ValueId = SubCode2Value;
        }
      }


      if (employee.OrganizationIdName) {
        let organizationName = this.ListPhongBan.find(x => x.organizationName.toLowerCase().trim() == employee.OrganizationIdName.toLowerCase().trim());
        if (!organizationName) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "wrong_OrganizationIdName")];
          employee.isValid = false;
        } else {
          employee.OrganizationId = organizationName;
        }
      }

      if (employee.ProvinceIdName) {
        let Province = this.ListProvince.find(x => x.provinceName.toLowerCase().trim() == employee.ProvinceIdName.toLowerCase().trim());
        if (!Province) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "wrong_ProvinceIdName")];
          employee.isValid = false;
        } else {
          employee.ProvinceId = Province;
        }
      }

      if (employee.KyNangTayNgheName) {
        let KyNangTayNgheName = this.ListKyNangTayNghe.find(x => x.name.toLowerCase().trim() == employee.KyNangTayNgheName.toLowerCase().trim());
        if (!KyNangTayNgheName) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "wrong_KyNangTayNghe")];
          employee.isValid = false;
        } else {
          employee.KyNangTayNgheId = KyNangTayNgheName;
        }
      }

      if (employee.BangCapCaoNhatDatDuocIdName) {
        let BangCapCaoNhatDatDuoc = this.ListBangCap.find(x => x.categoryName.toLowerCase().trim() == employee.BangCapCaoNhatDatDuocIdName.toLowerCase().trim());
        if (!BangCapCaoNhatDatDuoc) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "wrong_BangCapCaoNhatDatDuocIdName")];
          employee.isValid = false;
        } else {
          employee.BangCapCaoNhatDatDuocId = BangCapCaoNhatDatDuoc;
        }
      }

      if (employee.PhuongThucTuyenDungIdName) {
        let PhuongThucTuyenDung = this.ListPTTD.find(x => x.categoryName.toLowerCase().trim() == employee.PhuongThucTuyenDungIdName.toLowerCase().trim());
        if (!PhuongThucTuyenDung) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "wrong_PhuongThucTuyenDungIdName")];
          employee.isValid = false;
        } else {
          employee.PhuongThucTuyenDungId = PhuongThucTuyenDung;
        }
      }

      if (employee.NguonTuyenDungIdName) {
        let NguonTuyenDung = this.ListKenhTd.find(x => x.categoryName.toLowerCase().trim() == employee.NguonTuyenDungIdName.toLowerCase().trim());
        if (!NguonTuyenDung) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "wrong_NguonTuyenDungIdName")];
          employee.isValid = false;
        } else {
          employee.NguonTuyenDungId = NguonTuyenDung;
        }
      }

      //Số phải lớn hơn 0
      if (employee.DiemTest) {
        if (Number(employee.DiemTest) != NaN) {
          if (Number(employee.DiemTest) < 0) {
            employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "DiemTest_positive")];
            employee.isValid = false;
          }
        }
      }

      if (employee.SoPhepConLai) {
        if (Number(employee.SoPhepConLai) != NaN) {
          if (Number(employee.SoPhepConLai) < 0) {
            employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "SoPhepConLai_positive")];
            employee.isValid = false;
          }
        }
      }

      if (employee.SoPhepDaSuDung) {
        if (Number(employee.SoPhepDaSuDung) != NaN) {
          if (Number(employee.SoPhepDaSuDung) < 0) {
            employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "SoPhepDaSuDung_positive")];
            employee.isValid = false;
          }
        }
      }

    });

    /* auto add to valid list */
    // if (autoAdd) this.selectedEmployeeImport = this.listTaiSanImport.filter(e => e.isValid);
  }

  onCancel() {
    let result: ResultDialog = {
      status: false,
      statusImport: false
    };
    this.ref.close(result);
  }

  async importCustomer() {
    /* check valid list selected */
    if (this.selectedEmployeeImport.length == 0) {
      let msg = { severity: 'warn', summary: 'Thông báo', detail: 'Chọn danh sách cần import' };
      this.showMessage(msg);
      return;
    }
    let inValidRecord = this.selectedEmployeeImport.find(e => !e.isValid);
    if (inValidRecord) {
      let msg = { severity: 'error', summary: 'Thông báo', detail: 'Danh sách không hợp lệ' };
      this.showMessage(msg);
      return;
    }
    this.checkStatus(false);
    this.standardizedListCustomer();
    let listEmpAdditional: Array<EmployeeEntityModel> = [];
    this.selectedEmployeeImport.forEach(item => {
      var newEmp = this.mapFormToEmployeeModel(item);
      listEmpAdditional.push(newEmp);
    });
    console.log('listEmpAdditional', listEmpAdditional)
    this.loading = true;
    let result: any = await this.employeeService.importEmployee(listEmpAdditional);
    this.loading = false;
    if (result.statusCode === 200) {
      let mgs = { severity: 'success', summary: 'Thông báo', detail: result.message };
      this.showMessage(mgs);
      this.ref.close(result);
    } else {
      let mgs = { severity: 'error', summary: 'Thông báo', detail: result.message };
      this.showMessage(mgs);
      this.getMasterdata();
      this.checkStatus(true);
    }
  }

  standardizedListCustomer() {
    this.listTaiSanImport.forEach(customer => {
      // customer.materialID = customer.materialID?.trim() ?? "";
    });
  }

  mapFormToEmployeeModel(employee): EmployeeEntityModel {
    let employeeModel = new EmployeeEntityModel();
    employeeModel.EmployeeCode = employee.EmployeeCode.trim();
    employeeModel.EmployeeName = employee.EmployeeName.trim();
    employeeModel.HoTenTiengAnh = employee.HoTenTiengAnh.trim();
    employeeModel.FirstName = employee.FirstName;
    employeeModel.LastName = employee.LastName;
    employeeModel.DateOfBirth = convertToUTCTime(new Date(employee.DateOfBirth));
    employeeModel.Gender = employee.Gender;
    employeeModel.OtherPhone = employee.OtherPhone;
    employeeModel.Phone = employee.Phone;
    employeeModel.model = employee.model;
    employeeModel.Email = employee.Email;
    employeeModel.model = employee.model;
    employeeModel.QuocTich = employee.QuocTich;
    employeeModel.DanToc = employee.DanToc;
    employeeModel.TonGiao = employee.TonGiao;
    employeeModel.CodeMayChamCong = employee.CodeMayChamCong;
    employeeModel.TenMayChamCong = employee.TenMayChamCong;
    employeeModel.MaTest = employee.MaTest;
    employeeModel.DiemTest = employee.DiemTest;
    employeeModel.GradeTesting = employee.GradeTesting != null ? employee.GradeTesting.categoryName : '';
    employeeModel.DeptCodeValue = employee.DeptCodeValueId != null ? employee.DeptCodeValueId.value : '';
    employeeModel.SubCode1 = employee.SubCode1ValueId != null ? employee.SubCode1ValueId.value : '';
    employeeModel.SubCode2 = employee.SubCode2ValueId != null ? employee.SubCode2ValueId.value : '';
    employeeModel.OrganizationId = employee.OrganizationId != null ? employee.OrganizationId.organizationId : '';
    employeeModel.ProvinceId = employee.ProvinceId != null ? employee.ProvinceId.provinceId : "";
    employeeModel.SocialInsuranceNumber = employee.SocialInsuranceNumber;
    employeeModel.MaSoThueCaNhan = employee.MaSoThueCaNhan;
    employeeModel.HealthInsuranceNumber = employee.HealthInsuranceNumber;
    employeeModel.IdentityId = employee.IdentityId;
    employeeModel.IdentityIddateOfIssue = employee.IdentityIddateOfIssue != null ? convertToUTCTime(new Date(employee.IdentityIddateOfIssue)) : null;
    employeeModel.IdentityIdplaceOfIssue = employee.IdentityIdplaceOfIssue;
    employeeModel.NoiCapCmndtiengAnh = employee.NoiCapCmndtiengAnh;
    employeeModel.HoKhauThuongTruTv = employee.HoKhauThuongTruTv;
    employeeModel.HoKhauThuongTruTa = employee.HoKhauThuongTruTa;

    employeeModel.Address = employee.Address;
    employeeModel.AddressTiengAnh = employee.AddressTiengAnh;
    employeeModel.BankCode = employee.BankCode;
    employeeModel.BankOwnerName = employee.BankOwnerName;
    employeeModel.BankAccount = employee.BankAccount;
    employeeModel.BankName = employee.BankName;
    employeeModel.BienSo = employee.BienSo;
    employeeModel.LoaiXe = employee.LoaiXe;
    // employeeModel.DienThoaiEmailLienHe = employee.DienThoaiEmailLienHe;
    // employeeModel.KinhNghiemLamViec = employee.KinhNghiemLamViec;
    employeeModel.KyNangTayNghe = employee.KyNangTayNghe != null ? employee.KyNangTayNgheId.value : '';
    employeeModel.TomTatHocVan = employee.TomTatHocVan;
    employeeModel.ChuyenNganhHoc = employee.ChuyenNganhHoc;
    employeeModel.TenTruongHocCaoNhat = employee.TenTruongHocCaoNhat;
    employeeModel.BangCapCaoNhatDatDuocId = employee.BangCapCaoNhatDatDuocId != null ? employee.BangCapCaoNhatDatDuocId.categoryId : '';
    employeeModel.PhuongThucTuyenDungId = employee.PhuongThucTuyenDungId != null ? employee.PhuongThucTuyenDungId.categoryId : '';
    employeeModel.CoPhi = employee.CoPhi == 'Có phí' ? true : false;
    employeeModel.MucPhi = this.commonService.convertStringToNumber(employee.MucPhi.toString());
    employeeModel.NguonTuyenDungId = employee.NguonTuyenDungId != null ? employee.NguonTuyenDungId.categoryId : '';
    employeeModel.SoPhepConLai = employee.SoPhepConLai;
    employeeModel.SoPhepDaSuDung = employee.SoPhepDaSuDung;
    return employeeModel;
  }
  //end
}

function validateString(str: string) {
  if (str === undefined) return "";
  return str.trim();
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

