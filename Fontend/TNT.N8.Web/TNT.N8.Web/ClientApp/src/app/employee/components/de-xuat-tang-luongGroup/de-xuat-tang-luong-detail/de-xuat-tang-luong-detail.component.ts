import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { EmployeeService } from '../../../services/employee.service';
import { Row, Workbook, Worksheet } from 'exceljs';
import { saveAs } from "file-saver";
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { interactionSettingsStore } from '@fullcalendar/core';
import * as XLSX from 'xlsx';
import { DialogService, FileUpload } from 'primeng';
import { ImportNvDeXuatTangLuongComponent } from '../importNv-de-xuat-tang-luong/importNv-de-xuat-tang-luong.component';
import { QuyTrinhService } from '../../../../admin/services/quy-trinh.service';
import { NoteDocumentModel } from '../../../../../../src/app/shared/models/note-document.model';
import { NoteService } from '../../../../shared/services/note.service';
import { TeacherSalaryListComponent } from '../../employee-salary/teacher-salary-list/teacher-salary-list.component';
import { ImageUploadService } from '../../../../shared/services/imageupload.service';
import { ForderConfigurationService } from '../../../../admin/components/folder-configuration/services/folder-configuration.service';
import { EncrDecrService } from '../../../../shared/services/encrDecr.service';
import { ExportFileWordService } from '../../../../shared/services/exportFileWord.services'
import { Item } from '@syncfusion/ej2-angular-navigations';


//ListNV trả về cùng với lý do được phê duyệt
class ListNV {
  EmpId: string;
  LyDo: string;
}


class NoteModel {
  NoteId: string;
  NoteTitle: string;
  Description: string;
  Type: string;
  ObjectId: string;
  ObjectNumber: number;
  ObjectType: string;
  Active: Boolean;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  constructor(
  ) { }
}


class Note {
  active: boolean;
  createdById: string;
  createdDate: Date;
  description: string;
  noteDocList: Array<NoteDocument>;
  noteId: string;
  noteTitle: string;
  objectId: string;
  objectType: string;
  responsibleAvatar: string;
  responsibleName: string;
  type: string;
  updatedById: string;
  updatedDate: Date;
  constructor() {
    this.noteDocList = [];
  }
}

class NoteDocument {
  active: boolean;
  base64Url: string;
  createdById: string;
  createdDate: Date;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  noteDocumentId: string;
  noteId: string;
  updatedById: string;
  updatedDate: Date;
}



class importNVByExcelModel {
  empId: number;
  empCode: string;
  empName: string;
  oranganizationName: string;
  positionName: string;
  mucLuongCu: number;
  luongDeXuat: number;
  luongChenhLech: number;
  phongBanId: string;
  chucVuId: string;;
  lydo: string;

}

class EmployeeInterface {
  employeeId: number;
  employeeName: string;
  chucVuId: string;
  organizationName: string;
  employeeCode: string;
  PhongBanId: string;
  positionName: string;
  DateOfBirth: string;
  lyDoDeXuat: string;
  luongDeXuat: number;
  luongHienTai: number;
  mucChechLech: number;
  trangThai: number;
  lyDo: number;
  diemDanhGia: string;
  phieuDanhGia: string;

  nguoiDanhGiaName: string;
  tongDiemTuDanhGia: number;
  tongDiemDanhGia: number;
  mucLuongDeXuatQuanLy: number;
}

class DeXuatTangLuongModel {
  DeXuatTangLuongId: number;
  TenDeXuat: string;
  LoaiDeXuat: number;
  NgayDeXuat: Date;
  NguoiDeXuatId: string;
  TrangThai: number;
  NgayApDung: Date;
}


class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}
class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileFullName: string;
  fileName: string;
  objectId: string;
  objectNumber: number;
  fileUrl: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
  createdById: string;
  createdDate: Date;
  uploadByName: string;
}


@Component({
  selector: 'app-de-xuat-tang-luong-detail',
  templateUrl: './de-xuat-tang-luong-detail.component.html',
  styleUrls: ['./de-xuat-tang-luong-detail.component.css']
})



export class DeXuatTangLuongDetailComponent implements OnInit {


  auth = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;

  filterGlobal: string = '';


  displayChooseFileImportDialog: boolean = false;
  fileName: string = '';
  importFileExcel: any = null;

  colsListEmp: any[];
  selectedColumns: any[];

  listEmpAdd: any[]; // List nhân viên chọn để thêm đề xuất
  listEmpSelected: Array<EmployeeInterface> = new Array<EmployeeInterface>();  // List nhân viên được chọn thêm đề xuất
  listEmpSelectedFilter: Array<EmployeeInterface> = new Array<EmployeeInterface>(); //Show ở table để lọc
  statusCode: string = null;

  listMucLuongDeXuatRoot = [];
  today = new Date();

  listChucVu = [];
  listPhongBan = [];


  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;


  leftColNumber: number = 12;
  rightColNumber: number = 0;

  //seach table nhân viên được đề xuất
  phongBanSearch: any;
  chucVuSearch: any;
  trangThaiSearch: any;


  //Form của phiếu đề xuất tăng lương
  deXuatTangLuongFormGroup: FormGroup;
  tenDeXuatFormControl: FormControl;
  phanLoaiDeXuatFormControl: FormControl;
  ngayDeXuatFormControl: FormControl;
  nguoiDeXuatFormControl: FormControl;


  //Ds nhân viên được đề xuất
  nhanVienFormGroup: FormGroup;
  EmployeeIdFormControl: FormControl;
  EmployeeNameFormControl: FormControl;
  OranganizationIdFormControl: FormControl;
  OranganizationNameFormControl: FormControl;
  MucLuongDeXuatFormControl: FormControl;
  EmployeeCodeFormControl: FormControl;
  PositisionIdFormControl: FormControl;
  PositisionNameFormControl: FormControl;
  DateOfBirthFormControl: FormControl;
  MucLuongCuFormControl: FormControl;
  LyDoFormControl: FormControl;



  //Form của ngày áp dụng mức lương mới
  ngayApDungLuongMoiFormGroup: FormGroup;
  ngayApDungLuongFormControl: FormControl;

  IsEditNV: boolean = false;

  tongTang: number = 0;
  quyLuongConLai: number = 0;
  quyLuongConLaiRoot: number = 0;
  loginEmpId: string = '';

  //list phân loại đều xuất có 2 loại => fixx cứng
  listLoaiDeXuat = [
    { value: 1, Name: "Đề xuất tăng lương adhoc" },
    { value: 2, Name: "Đề xuất tăng lương sau đánh giá" },
  ]


  listTrangThai = [
    { value: 1, name: "Mới" },
    { value: 2, name: "Chờ phê duyệt" },
    { value: 3, name: "Đã duyệt" },
    { value: 4, name: "Từ chối" },
  ]

  deXuatTLId: number = 0;
  loaiDeXuatType = 1;
  trangThaiDeXuat = 0;

  lyDoTuChoi: string = '';
  showLyDoTuChoi: boolean = false;

  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));


  companyConfig: any; // Thông tin vê cty

  //Loại từ chối.
  //1: từ chối đề xuất
  //2: từ chối nvien
  tuChoiType: number = 1;

  listFormatStatusSupport: Array<any> = []; // Thanh trạng thái

  isShowWorkFollowContract: boolean = true;
  //Hiển thị trạng thái các nút
  IsShowGuiXacNhan: boolean = false;
  IsShowXacNhan: boolean = false;
  IsShowTuChoi: boolean = false;
  IsShowLuu: boolean = false;
  IsShowXoa: boolean = false;
  IsShowHuy: boolean = false;
  IsShowHuyYeuCauXacNhan: boolean = false;
  IsShowHoanThanh: boolean = false;
  IsShowDatVeMoi: boolean = false;
  IsShowNgayApDung: boolean = false;

  NgayCuoiCungKyLuong: Date = null;


  emptyGuid = '00000000-0000-0000-0000-000000000000';

  selectedEmpCheckBox: Array<any>;


  //Note
  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  listUpdateNoteDocument: Array<NoteDocument> = [];
  noteHistory: Array<Note> = [];
  isAprovalQuote: boolean = false;
  noteId: string = null;
  noteContent: string = '';
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  uploadedNoteFiles: any[] = [];
  @ViewChild('fileNoteUpload') fileNoteUpload: FileUpload;
  @ViewChild('myTable') myTable: Table;


  @ViewChild('fileUpload') fileUpload: FileUpload;
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];
  colsFile: any[];
  /*End : Note*/
  file: File[];
  listFile: Array<FileInFolder> = [];
  colsNoteFile = [];

  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionDownLoad: boolean = true;
  actionUpLoad: boolean = true;


  isManagerOfHR: boolean = false;
  isGD: boolean = false;
  isNguoiPhuTrach: boolean = false;
  viewNote: boolean = true;
  viewTimeline: boolean = true;
  pageSize = 20;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    private def: ChangeDetectorRef,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private quyTrinhService: QuyTrinhService,
    private ref: ChangeDetectorRef,
    private noteService: NoteService,
    private imageService: ImageUploadService,
    private folderService: ForderConfigurationService,
    public exportFileWordService: ExportFileWordService,
    private encrDecrService: EncrDecrService,
    private getPermission: GetPermission,

  ) {
    this.innerWidth = window.innerWidth;
    if (this.innerWidth < 768) {
      this.isShowWorkFollowContract = false;
    }
  }

  async ngOnInit() {
    this.route.params.subscribe(params => {
      if (params['deXuatTLId']) {
        this.deXuatTLId = Number(this.encrDecrService.get(params['deXuatTLId']));
      }
    });
    this.setForm();
    let resource = "hrm/employee/de-xuat-tang-luong-detail/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(msg);

      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("view") == -1) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
        this.showMessage(msg);
        this.router.navigate(['/home']);      
      }
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
    }

    this.getMasterData();
  }


  setForm() {
    this.ngayApDungLuongFormControl = new FormControl(null, [Validators.required]);

    this.ngayApDungLuongMoiFormGroup = new FormGroup({
      ngayApDungLuongFormControl: this.ngayApDungLuongFormControl
    });


    this.tenDeXuatFormControl = new FormControl(null, [Validators.required]);
    this.phanLoaiDeXuatFormControl = new FormControl(null, [Validators.required]);
    this.ngayDeXuatFormControl = new FormControl(null, [Validators.required]);
    this.nguoiDeXuatFormControl = new FormControl(null, [Validators.required]);

    this.deXuatTangLuongFormGroup = new FormGroup({
      tenDeXuatFormControl: this.tenDeXuatFormControl,
      phanLoaiDeXuatFormControl: this.phanLoaiDeXuatFormControl,
      ngayDeXuatFormControl: this.ngayDeXuatFormControl,
      nguoiDeXuatFormControl: this.nguoiDeXuatFormControl
    });


    this.EmployeeIdFormControl = new FormControl(null);
    this.EmployeeNameFormControl = new FormControl(null);
    this.OranganizationIdFormControl = new FormControl();
    this.OranganizationNameFormControl = new FormControl(null);
    this.MucLuongCuFormControl = new FormControl(null);
    this.MucLuongDeXuatFormControl = new FormControl(null, [Validators.required, Validators.min(this.MucLuongCuFormControl.value)]);
    this.EmployeeCodeFormControl = new FormControl(null);
    this.PositisionIdFormControl = new FormControl(null);
    this.PositisionNameFormControl = new FormControl(null);
    this.DateOfBirthFormControl = new FormControl(null);
    this.LyDoFormControl = new FormControl(null, [Validators.required]);

    this.nhanVienFormGroup = new FormGroup({
      EmployeeIdFormControl: this.EmployeeIdFormControl,
      EmployeeNameFormControl: this.EmployeeNameFormControl,
      OranganizationIdFormControl: this.OranganizationIdFormControl,
      OranganizationNameFormControl: this.OranganizationNameFormControl,
      MucLuongDeXuatFormControl: this.MucLuongDeXuatFormControl,
      EmployeeCodeFormControl: this.EmployeeCodeFormControl,
      PositisionIdFormControl: this.PositisionIdFormControl,
      PositisionNameFormControl: this.PositisionNameFormControl,
      DateOfBirthFormControl: this.DateOfBirthFormControl,
      MucLuongCuFormControl: this.MucLuongCuFormControl,
      LyDoFormControl: this.LyDoFormControl,
    });
  }


  setCols() {

    this.colsFile = [
      { field: 'fileName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'right', type: 'number' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'right', type: 'date' },
      { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'left', type: 'string' },
    ];

    this.colsNoteFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'documentSize', header: 'Kích thước', width: '50%', textAlign: 'right', type: 'number' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'right', type: 'date' },
    ]

    this.colsListEmp = [
      { field: 'employeeCode', header: 'Mã Nhân viên', textAlign: 'left', display: 'table-cell', width: "150px", type: 1 },
      { field: 'employeeName', header: 'Tên Nhân viên', textAlign: 'left', display: 'table-cell', width: "150px", type: 1 },
      { field: 'employeeCodeName', header: 'Nhân viên', textAlign: 'left', display: 'table-cell', width: "200px", type: 1 },
      { field: 'organizationName', header: 'Phòng ban', textAlign: 'left', display: 'table-cell', width: '180px', type: 1 },
      { field: 'positionName', header: 'Chức vụ', textAlign: 'center', display: 'table-cell', width: '180px', type: 1 },
      { field: 'nguoiDanhGiaName', header: 'Người đánh giá', textAlign: 'left', display: 'table-cell', width: '200px', type: 2 },//
      { field: 'tongDiemTuDanhGia', header: 'Tự đánh giá', textAlign: 'center', display: 'table-cell', width: '150px', type: 2 },//
      { field: 'tongDiemDanhGia', header: 'Phụ trách đánh giá', textAlign: 'center', display: 'table-cell', width: '160px', type: 2 },//
      { field: 'diemDanhGia', header: 'Mức đánh giá cuối', textAlign: 'center', display: 'table-cell', width: '160px', type: 2 },//
      { field: 'luongHienTai', header: 'Mức lương cũ', textAlign: 'left', display: 'table-cell', width: '150px', type: 1 },
      { field: 'mucLuongDeXuatQuanLy', header: 'Mức lương phụ trách đề xuất', textAlign: 'left', display: 'table-cell', width: '160px', type: 2 },
      { field: 'luongDeXuat', header: 'Mức đề xuất tăng', textAlign: 'left', display: 'table-cell', width: '160px', type: 1 },
      { field: 'mucChechLech', header: 'Mức chênh lệch', textAlign: 'left', display: 'table-cell', width: '150px', type: 1 },
      { field: 'lyDoDeXuat', header: 'Lý do đề xuất', textAlign: 'left', display: 'table-cell', width: '150px', type: 1 },//
      { field: 'trangThai', header: 'Trạng thái', textAlign: 'left', display: 'table-cell', width: '120px', type: 1 },
      { field: 'lyDo', header: 'Ghi chú', textAlign: 'left', display: 'table-cell', width: '100px', type: 1 },
    ];
    if (this.loaiDeXuatType == 2) {
      if (this.trangThaiDeXuat == 1) {
        this.selectedColumns = this.colsListEmp.filter(x => x.header != "Mã Nhân viên" && x.header != "Tên Nhân viên" && x.header != "Ghi chú" && x.header != "Lý do đề xuất");;
      } else {
        this.selectedColumns = this.colsListEmp.filter(x => x.header != "Mã Nhân viên" && x.header != "Tên Nhân viên" && x.header != "Lý do đề xuất");;
      }
    } else {
      if (this.trangThaiDeXuat == 1) {
        this.selectedColumns = this.colsListEmp.filter(x => x.type == 1 && x.header != "Lý do đề xuất" && x.header != "Trạng thái" && x.header != "Ghi chú");
      } else {
        this.selectedColumns = this.colsListEmp.filter(x => x.type == 1);
      }
    }
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.employeeService.getMasterDataCreateDeXuatTangLuong();
    this.loading = false;

    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Lấy thông tin không thành công' };
      this.showMessage(msg);
      return;
    }
    this.listEmpAdd = result.listEmp;

    this.loading = true;
    let resultDetail: any = await this.employeeService.deXuatTangLuongDetail(this.deXuatTLId);
    this.loading = false;
    if (resultDetail.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: resultDetail.message };
      this.showMessage(msg);
      return;
    }
    this.selectedEmpCheckBox = [];

    this.companyConfig = resultDetail.companyConfig;

    this.setDefaultValue(resultDetail);
    this.getDuLieuQuyTrinh();

    this.uploadedFiles = [];

    this.arrayDocumentModel = resultDetail.listFileInFolder;
    this.ref.detectChanges();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  refreshFilter() {
    this.phongBanSearch = null;
    this.chucVuSearch = null;
    this.trangThaiSearch = null;
    this.filterGlobal = '';
    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.myTable?.reset();
    //Dsach lọc = danh sách chính
    this.listEmpSelectedFilter = this.listEmpSelected;

    this.searchEmployee();
  }

  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.rightColNumber = 4;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

  changeEmplyee(emp) {
    //check xem nhân viên có tồn tại trong list chưa
    var checkExist = this.listEmpSelected.find(x => x.employeeId == emp.value.employeeId);
    if (checkExist) {
      this.updateEmp(checkExist);
    } else {
      this.IsEditNV = false;
      this.EmployeeIdFormControl.setValue(emp.value);
      this.OranganizationIdFormControl.setValue(emp.value.organizationId);
      this.OranganizationNameFormControl.setValue(emp.value.organizationName);
      this.MucLuongDeXuatFormControl.setValue("");
      this.EmployeeCodeFormControl.setValue(emp.value.employeeCode);
      this.PositisionIdFormControl.setValue(emp.value.positionId);
      this.PositisionNameFormControl.setValue(emp.value.positionName);
      this.DateOfBirthFormControl.setValue(new Date(emp.value.dateOfBirth));
      this.LyDoFormControl.setValue("");
      this.MucLuongCuFormControl.setValue(emp.value.mucLuongHienTai);
    }
  }

  setDefaultValue(resultDetail) {
    this.tenDeXuatFormControl.setValue(resultDetail.deXuatTangLuong.tenDeXuat);
    this.phanLoaiDeXuatFormControl.setValue(this.listLoaiDeXuat.find(x => x.value == resultDetail.deXuatTangLuong.loaiDeXuat));
    this.ngayDeXuatFormControl.setValue(new Date(resultDetail.deXuatTangLuong.ngayDeXuat));
    this.nguoiDeXuatFormControl.setValue(this.listEmpAdd.find(x => x.employeeId == resultDetail.deXuatTangLuong.nguoiDeXuatId));
    this.nguoiDeXuatFormControl.disable();
    //danh sách nhân viên
    let data = resultDetail.nhanVienDuocDeXuats;
    this.listEmpSelected = data;
    this.listEmpSelectedFilter = data;
    data.forEach(item => {
      this.listMucLuongDeXuatRoot.push({
        employeeId: item.employeeId,
        luongDeXuat: item.luongDeXuat
      })
    })


    this.trangThaiDeXuat = resultDetail.deXuatTangLuong.trangThai;
    this.loaiDeXuatType = resultDetail.deXuatTangLuong.loaiDeXuat;
    this.tinhTongTang();
    this.setCols();

    this.quyLuongConLaiRoot = resultDetail.quyLuongConLai;
    this.quyLuongConLai = resultDetail.quyLuongConLai;

    this.listChucVu = resultDetail.listPosition;
    this.listPhongBan = resultDetail.listOrganization;
    if (resultDetail.ngayCuoiCungKyLuong != null) this.NgayCuoiCungKyLuong = resultDetail.ngayCuoiCungKyLuong;

    //Ngày áp dụng mức lương mới
    if (this.trangThaiDeXuat == 3) {
      if (resultDetail.deXuatTangLuong.ngayApDung) this.ngayApDungLuongFormControl.setValue(new Date(resultDetail.deXuatTangLuong.ngayApDung));
    }
    //Nếu trạng thái là mới thì disable from
    if (this.trangThaiDeXuat == 1) {
      this.deXuatTangLuongFormGroup.enable();
    } else {
      this.deXuatTangLuongFormGroup.disable();
    }
    this.phanLoaiDeXuatFormControl.disable();

    //trạng thái nút
    this.IsShowGuiXacNhan = resultDetail.isShowGuiXacNhan;
    this.IsShowXacNhan = resultDetail.isShowXacNhan;
    this.IsShowTuChoi = resultDetail.isShowTuChoi;
    this.IsShowLuu = resultDetail.isShowLuu;
    this.IsShowXoa = resultDetail.isShowXoa;
    this.IsShowHuy = resultDetail.isShowHuy;
    this.IsShowHuyYeuCauXacNhan = resultDetail.isShowHuyYeuCauXacNhan;
    this.IsShowHoanThanh = resultDetail.isShowHoanThanh;
    this.IsShowDatVeMoi = resultDetail.isShowDatVeMoi;
    this.IsShowNgayApDung = resultDetail.isShowNgayApDung;
  }

  AddEmp() {
    if (!this.nhanVienFormGroup.valid) {
      Object.keys(this.nhanVienFormGroup.controls).forEach(key => {
        if (this.nhanVienFormGroup.controls[key].valid == false) {
          this.nhanVienFormGroup.controls[key].markAsTouched();
        }
      });
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Vui lòng nhập đầy đủ thông tin các trường dữ liệu.' };
      this.showMessage(msg);
      return;
    }

    let newEmp = new EmployeeInterface();

    newEmp.employeeId = this.EmployeeIdFormControl.value.employeeId;
    newEmp.employeeName = this.EmployeeIdFormControl.value.employeeName;
    newEmp.PhongBanId = this.OranganizationIdFormControl.value;
    newEmp.organizationName = this.OranganizationNameFormControl.value;
    newEmp.employeeCode = this.EmployeeCodeFormControl.value;
    newEmp.chucVuId = this.PositisionIdFormControl.value;
    newEmp.positionName = this.PositisionNameFormControl.value;
    newEmp.lyDoDeXuat = this.LyDoFormControl.value;
    newEmp.luongDeXuat = this.MucLuongDeXuatFormControl.value;
    newEmp.luongHienTai = this.MucLuongCuFormControl.value;
    newEmp.mucChechLech = this.MucLuongDeXuatFormControl.value - this.MucLuongCuFormControl.value;
    newEmp.trangThai = 1; //trạng thái mới
    var checkExist = this.listEmpSelected.filter(x => x.employeeId == this.EmployeeIdFormControl.value.employeeId);
    if (checkExist.length != 0) {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Nhân viên đã tồn tại trong danh sách đề xuất' };
      this.showMessage(msg);
      return;
    }

    this.listEmpSelected.push(newEmp);
    this.def.detectChanges();
    this.clearEmp();
    this.tinhTongTang();
    this.refreshFilter();
  }

  async updateStatusNv(IsXacNhan) {

    if (this.selectedEmpCheckBox == null) {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: "Chưa có nhân viên nào được chọn!" };
      this.showMessage(msg);
      return;
    }

    if (this.selectedEmpCheckBox.length == 0) {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: "Chưa có nhân viên nào được chọn!" };
      this.showMessage(msg);
      return;
    }

    if (this.lyDoTuChoi == null || this.lyDoTuChoi.trim().length == 0) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: "Nhập lý do từ chối" };
      this.showMessage(mgs);
      return;
    }

    this.loading = true;
    let resultDetail: any = await this.employeeService.tuChoiOrPheDuyetNhanVienDeXuatTL(this.selectedEmpCheckBox, this.deXuatTLId, IsXacNhan, this.lyDoTuChoi);
    this.showLyDoTuChoi = false;
    this.lyDoTuChoi = ''
    this.loading = false;
    if (resultDetail.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: resultDetail.message };
      this.showMessage(msg);
      return;
    }

    let msg = { severity: 'success', summary: 'Thông báo:', detail: resultDetail.message };
    this.showMessage(msg);
    let listChangeStatus = this.selectedEmpCheckBox.map(i => i.employeeId);
    this.listEmpSelected.forEach(i => {
      if (listChangeStatus.indexOf(i.employeeId) > -1) {
        if (IsXacNhan) {
          i.trangThai = 3;
        } else {
          i.trangThai = 4;
        }
      }
    });
    this.selectedEmpCheckBox = [];
    this.refreshFilter();
  }

  thoat() {
    this.router.navigate(['/employee/danh-sach-de-xuat-tang-luong']);
  }

  // Event thay đổi nội dung ghi chú
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  async EditEmp() {
    if (!this.nhanVienFormGroup.valid) {
      Object.keys(this.nhanVienFormGroup.controls).forEach(key => {
        if (this.nhanVienFormGroup.controls[key].valid == false) {
          this.nhanVienFormGroup.controls[key].markAsTouched();
        }
      });
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Vui lòng nhập đầy đủ thông tin các trường dữ liệu.' };
      this.showMessage(msg);
      return;
    }
    this.IsEditNV = false;
        //Gán thông tin mới + thông tin cũ
    let newEmp = new EmployeeInterface();
    newEmp.employeeId = this.EmployeeIdFormControl.value.employeeId;
    newEmp.employeeName = this.EmployeeIdFormControl.value.employeeName;
    newEmp.PhongBanId = this.OranganizationIdFormControl.value;
    newEmp.organizationName = this.OranganizationNameFormControl.value;
    newEmp.employeeCode = this.EmployeeCodeFormControl.value;
    newEmp.chucVuId = this.PositisionIdFormControl.value;
    newEmp.positionName = this.PositisionNameFormControl.value;
    newEmp.lyDoDeXuat = this.LyDoFormControl.value;
    newEmp.luongDeXuat = this.MucLuongDeXuatFormControl.value;
    newEmp.luongHienTai = this.MucLuongCuFormControl.value
    newEmp.mucChechLech = this.MucLuongDeXuatFormControl.value - this.MucLuongCuFormControl.value;
    //Gán thêm thông tin về kỳ đánh giá
    if (this.loaiDeXuatType == 2) {
      let nhanVienInfor = await this.listEmpSelected.find(x => x.employeeId == newEmp.employeeId);
      newEmp.nguoiDanhGiaName = nhanVienInfor.nguoiDanhGiaName;
      newEmp.tongDiemTuDanhGia = nhanVienInfor.tongDiemTuDanhGia;
      newEmp.tongDiemDanhGia = nhanVienInfor.tongDiemDanhGia;
      newEmp.mucLuongDeXuatQuanLy = nhanVienInfor.mucLuongDeXuatQuanLy;
      newEmp.diemDanhGia = nhanVienInfor.diemDanhGia;

      //Nếu quỹ lương còn lại nhỏ hơn 0 => thông báo và k cho cập nhật
      this.tinhTongTang();
      this.quyLuongConLai = this.quyLuongConLai - this.tongTang;
      if (this.quyLuongConLai < 0) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Mức tăng đã vượt quá quỹ lương cho phép' };
        this.showMessage(msg);
        return;
      }
    }

    newEmp.trangThai = 1; // trạng thái mới
    this.listEmpSelected = await this.listEmpSelected.filter(x => x.employeeId != newEmp.employeeId);
    this.listEmpSelected.push(newEmp);
    this.def.detectChanges();
    this.clearEmp();
    this.tinhTongTang();
    this.refreshFilter();
  }

  async changeMucLuongDeXuat(rowData) {
    rowData.mucChechLech = rowData.luongDeXuat - rowData.luongHienTai;
    //Nếu quỹ lương còn lại nhỏ hơn 0 => thông báo và k cho cập nhật
    this.tinhTongTang();
    if (this.quyLuongConLai < 0) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Mức tăng đã vượt quá quỹ lương cho phép' };
      this.showMessage(msg);
      // đặt về mức lương cũ
      let mucDeXuatCu = this.listMucLuongDeXuatRoot.find(x => x.employeeId == rowData.employeeId);
      this.listEmpSelectedFilter.forEach(x => {
        if (x.employeeId == rowData.employeeId) {

          x.luongDeXuat = mucDeXuatCu.luongDeXuat;
          x.mucChechLech = mucDeXuatCu.luongDeXuat - rowData.luongHienTai;
        }
      })
      this.tinhTongTang();
      return;
    }
    // cập nhật mức đề xuất mới ở list root
    this.listEmpSelected.forEach(x => {
      if (x.employeeId == rowData.employeeId) {
        x.luongDeXuat = rowData.luongDeXuat;
      }
    })
    this.tinhTongTang();
  }

  removeEmp(rowData) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.listEmpSelected = this.listEmpSelected.filter(x => x.employeeId != rowData.employeeId);
        this.refreshFilter();
        this.loading = false;
      }
    });

  }

  updateEmp(rowData) {
    this.IsEditNV = true;
    this.EmployeeIdFormControl.setValue(this.listEmpAdd.find(x => x.employeeId == rowData.employeeId));
    this.OranganizationIdFormControl.setValue(rowData.OrganizationId);
    this.OranganizationNameFormControl.setValue(rowData.organizationName);
    this.MucLuongDeXuatFormControl.setValue(rowData.luongDeXuat);
    this.EmployeeCodeFormControl.setValue(rowData.employeeCode);
    this.PositisionIdFormControl.setValue(rowData.PositionId);
    this.PositisionNameFormControl.setValue(rowData.positionName);
    this.DateOfBirthFormControl.setValue(rowData.DateOfBirth);
    this.MucLuongCuFormControl.setValue(rowData.luongHienTai);
    this.LyDoFormControl.setValue(rowData.lyDoDeXuat);
    this.MucLuongDeXuatFormControl.setValidators([Validators.required, Validators.min(this.MucLuongCuFormControl.value)]);
    this.MucLuongDeXuatFormControl.updateValueAndValidity();
  }

  clearEmp() {
    this.IsEditNV = false;
    this.nhanVienFormGroup.reset();
  }

  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo?.systemValueString;
  }

  exportExcel() {
    let title = `Danh sách nhân viên đề xuất tăng lương`;

    let workBook = new Workbook();
    let worksheet = workBook.addWorksheet(title);

    //LOGO - infor section
    let imgBase64 = this.getBase64Logo();
    var imgLogo = workBook.addImage({
      base64: imgBase64,
      extension: 'png',
    });
    worksheet.addImage(imgLogo, {
      tl: { col: 0.5, row: 0.5 },
      ext: { width: 140, height: 95 }
    });

    let dataInforRow_1 = `${this.companyConfig.companyName}`
    let dataInforRow_2 = `Địa chỉ: ${this.companyConfig.companyAddress}`;
    let dataInforRow_3 = `Điện thoại: ${this.companyConfig.phone}`;
    let dataInforRow_4 = `Email: ${this.companyConfig.email}`;
    let dataInforRow_5 = `Website dịch vụ: `

    let inforRow_1 = worksheet.addRow(['', '', dataInforRow_1]);
    let inforRow_2 = worksheet.addRow(['', '', dataInforRow_2]);
    let inforRow_3 = worksheet.addRow(['', '', dataInforRow_3]);
    let inforRow_4 = worksheet.addRow(['', '', dataInforRow_4]);
    let inforRow_5 = worksheet.addRow(['', '', dataInforRow_5]);

    inforRow_1.font = { size: 14, bold: true };
    worksheet.mergeCells(`C${1}:E${1}`);
    worksheet.mergeCells(`C${2}:${2}`);
    worksheet.mergeCells(`C${3}:E${3}`);
    worksheet.mergeCells(`C${4}:E${4}`);
    worksheet.mergeCells(`C${5}:E${5}`);

    worksheet.addRow([]);
    /* title */
    let dataHeaderMain = "Danh sách nhân viên đề xuất tăng lương".toUpperCase();
    let headerMain = worksheet.addRow([dataHeaderMain]);
    headerMain.font = { size: 18, bold: true };
    worksheet.mergeCells(`A${7}:G${7}`);
    headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    worksheet.addRow([]);

    let dataHeaderRow1: Array<string> = [];
    if (this.loaiDeXuatType == 1) dataHeaderRow1 = ["", "Mã nhân viên", "Tên nhân viên", "Phòng ban", "Chức vụ", "Mức lương cũ", "Mức đề xuất tăng", "Mức chênh lệch", "Lý do đề xuất", "Trạng thái", "Ghi chú"];
    if (this.loaiDeXuatType == 2) dataHeaderRow1 = ["", "Mã nhân viên", "Tên nhân viên", "Phòng ban", "Chức vụ", "Điểm đánh giá", "Phiếu đánh giá", "Mức lương cũ", "Mức đề xuất tăng", "Mức chênh lệch", "Lý do đề xuất", "Trạng thái", "Ghi chú"];

    let headerRow1 = worksheet.addRow(dataHeaderRow1);
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };

    //merge header column
    worksheet.mergeCells(`A${9}:A${9}`);
    worksheet.mergeCells(`B${9}:B${9}`);
    worksheet.mergeCells(`C${9}:C${9}`);
    worksheet.mergeCells(`D${9}:D${9}`);
    worksheet.mergeCells(`E${9}:E${9}`);

    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
    dataHeaderRow1.forEach((item, index) => {
      if (index + 2 < dataHeaderRow1.length + 1) {
        headerRow1.getCell(index + 2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow1.getCell(index + 2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow1.getCell(index + 2).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      }
    });

    let data: Array<any> = [];
    this.listEmpSelected?.forEach((item, index) => {
      let row: Array<any> = [];
      if (this.loaiDeXuatType == 1) {
        row[0] = '';
        row[1] = item.employeeCode;
        row[2] = item.employeeName;
        row[3] = item.organizationName;
        row[4] = item.positionName;
        row[5] = new Intl.NumberFormat().format(item.luongHienTai);
        row[6] = new Intl.NumberFormat().format(item.luongDeXuat);
        row[7] = new Intl.NumberFormat().format(item.mucChechLech);
        row[8] = item.lyDoDeXuat;
        if (item.trangThai == 1) row[9] = "Mới";
        if (item.trangThai == 2) row[9] = "Chờ phê duyệt";
        if (item.trangThai == 3) row[9] = "Phê duyệt";
        if (item.trangThai == 4) row[9] = "Từ chối";
        row[10] = item.lyDo ?? "";
      } else {
        row[0] = '';
        row[1] = item.employeeCode;
        row[2] = item.employeeName;
        row[3] = item.organizationName;
        row[4] = item.positionName;
        row[5] = item.diemDanhGia;
        row[6] = item.phieuDanhGia;
        row[7] = new Intl.NumberFormat().format(item.luongHienTai);
        row[8] = new Intl.NumberFormat().format(item.luongDeXuat);
        row[9] = new Intl.NumberFormat().format(item.mucChechLech);
        row[10] = item.lyDoDeXuat;
        if (item.trangThai == 1) row[11] = "Mới";
        if (item.trangThai == 2) row[11] = "Chờ phê duyệt";
        if (item.trangThai == 3) row[11] = "Phê duyệt";
        if (item.trangThai == 4) row[11] = "Từ chối";
        row[12] = item.lyDo ?? "";
      }
      data.push(row);
    });

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman', size: 11 };

      row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(5).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(6).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(7).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(7).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(8).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(8).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(9).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(9).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(10).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(10).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(11).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(11).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      if (this.loaiDeXuatType == 2) {

        row.getCell(12).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(12).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

        row.getCell(13).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(13).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      }

    });

    /* fix with for column */
    worksheet.getColumn(1).width = 15;
    worksheet.getColumn(2).width = 20;
    worksheet.getColumn(3).width = 25;
    worksheet.getColumn(4).width = 30;
    worksheet.getColumn(5).width = 15;

    worksheet.getColumn(6).width = 15;
    worksheet.getColumn(7).width = 15;
    worksheet.getColumn(8).width = 15;
    worksheet.getColumn(9).width = 20;
    worksheet.getColumn(10).width = 30;
    worksheet.getColumn(11).width = 20;

    if (this.loaiDeXuatType == 2) {
      worksheet.getColumn(12).width = 15;
      worksheet.getColumn(12).width = 15;
    }

    this.exportToExel(workBook, title);

  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  getDuLieuQuyTrinh() {
    this.quyTrinhService.getDuLieuQuyTrinh(this.emptyGuid, 10, this.deXuatTLId).subscribe(res => {
      let result: any = res;

      if (result.statusCode == 200) {
        this.listFormatStatusSupport = result.listDuLieuQuyTrinh;
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  tickAllNv(checkValue) {
    this.selectedEmpCheckBox = this.listEmpSelectedFilter;
    if (checkValue) {
      this.selectedEmpCheckBox = this.selectedEmpCheckBox.filter(x => x.trangThai != 3 && x.trangThai != 4);
    } else {
      this.selectedEmpCheckBox = [];
    }
  }

  async guiXacNhan() {
    await this.updateStatusDeXuatTL();
    this.loading = true;
    this.quyTrinhService.guiPheDuyet(this.emptyGuid, 10, this.deXuatTLId).subscribe(res => {
      let result: any = res;

      if (result.statusCode == 200) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
        this.getMasterData();
      }
      else {
        this.loading = false;
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  async datVeMoi() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn sử dụng lại Hợp đồng này?',
      accept: async () => {

        let res: any = await this.employeeService.datVeMoiDeXuatTangLuong(this.deXuatTLId);

        if (res.statusCode === 200) {
          let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Đặt về mới đề xuất tăng lương thành công" };
          this.showMessage(mgs);
          this.getMasterData();
        } else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: res.messageCode };
          this.showMessage(mgs);
        }
      }
    });
  }

  async xacNhan() {
    if (this.IsShowNgayApDung) {
      if (!this.ngayApDungLuongMoiFormGroup.valid) {
        Object.keys(this.ngayApDungLuongMoiFormGroup.controls).forEach(key => {
          if (this.ngayApDungLuongMoiFormGroup.controls[key].valid == false) {
            this.ngayApDungLuongMoiFormGroup.controls[key].markAsTouched();
          }
        });
        let mgs = { severity: 'warn', summary: 'Thông báo:', detail: "Nhập ngày áp dụng lương mới!" };
        this.showMessage(mgs);
        return;
      }

      let ngayApDung = null;
      if (this.IsShowNgayApDung) {
        ngayApDung = convertToUTCTime(new Date(this.ngayApDungLuongFormControl.value));
      }
      let result: any = await this.employeeService.capNhatNgayApDungDeXuat(ngayApDung, this.deXuatTLId, 1);
      if (result.statusCode != 200) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
        return;
      }
    }

    this.loading = true;
    this.quyTrinhService.pheDuyet(this.emptyGuid, 10, '', this.deXuatTLId).subscribe(res => {
      let result: any = res;
      if (result.statusCode == 200) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
        this.getMasterData();
      }
      else {
        this.loading = false;
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  async tuChoi() {
    if (this.lyDoTuChoi == null || this.lyDoTuChoi.trim().length == 0) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: "Nhập lý do từ chối" };
      this.showMessage(mgs);
      return;
    }

    this.loading = true;
    this.quyTrinhService.tuChoi(this.emptyGuid, 10, this.lyDoTuChoi, this.deXuatTLId).subscribe(res => {
      let result: any = res;

      if (result.statusCode == 200) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);

        this.getMasterData();
      }
      else {
        this.loading = false;
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
    this.showLyDoTuChoi = false;
    this.lyDoTuChoi = "";
  }

  huyYeuCacXacNhan() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn hủy yêu cầu xác nhận hợp đồng này không?',
      accept: async () => {
        this.loading = true;
        this.quyTrinhService.huyYeuCauPheDuyet(this.emptyGuid, 10, this.deXuatTLId).subscribe(res => {
          let result: any = res;

          if (result.statusCode != 200) {
            this.loading = false;
            let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
            this.showMessage(msg);
            return;
          }

          let msg = { severity: 'success', summary: 'Thông báo', detail: result.messageCode };
          this.showMessage(msg);
          this.getMasterData();
        });
      }
    });
  }

  async lyDoPheDuyetOrTuChoi(rowData) {
    if (rowData.lyDo.trim() == '' || rowData.lyDo.trim() == null) return;
    let result: any = await this.employeeService.capNhatLyDoPheDuyetOrTuChoiDeXuatNV(rowData.deXuatTangLuongNhanVienId, rowData.lyDo, 1);
    this.loading = false;
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
      this.showMessage(msg);
      return;
    }
  }

  async updateStatusDeXuatTL() {
    if (!this.deXuatTangLuongFormGroup.valid) {
      Object.keys(this.deXuatTangLuongFormGroup.controls).forEach(key => {
        if (this.deXuatTangLuongFormGroup.controls[key].valid == false) {
          this.deXuatTangLuongFormGroup.controls[key].markAsTouched();
        }
      });

      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Vui lòng nhập đầy đủ thông tin các trường dữ liệu.' };
      this.showMessage(msg);
      return;
    }
    if (this.listEmpSelected.length == 0) {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Hãy chọn nhân viên được đề xuất tăng lương!' };
      this.showMessage(msg);
      return;
    }

    let deXuatTangLuong = new DeXuatTangLuongModel();
    deXuatTangLuong.DeXuatTangLuongId = this.deXuatTLId;
    deXuatTangLuong.TenDeXuat = this.tenDeXuatFormControl.value;
    deXuatTangLuong.LoaiDeXuat = this.phanLoaiDeXuatFormControl.value.value;
    deXuatTangLuong.NgayDeXuat = this.ngayDeXuatFormControl.value;
    deXuatTangLuong.NguoiDeXuatId = this.nguoiDeXuatFormControl.value.employeeId;
    deXuatTangLuong.TrangThai = this.trangThaiDeXuat;
    deXuatTangLuong.NgayApDung = null;

    // File tài liệu
    let listFileUploadModel: Array<FileUploadModel> = [];
    this.uploadedFiles.forEach(item => {
      let fileUpload: FileUploadModel = new FileUploadModel();
      fileUpload.FileInFolder = new FileInFolder();
      fileUpload.FileInFolder.active = true;
      let index = item.name.lastIndexOf(".");
      let name = item.name.substring(0, index);
      fileUpload.FileInFolder.fileName = name;
      fileUpload.FileInFolder.fileExtension = item.name.substring(index + 1);
      fileUpload.FileInFolder.size = item.size;
      fileUpload.FileInFolder.objectId = '';
      fileUpload.FileInFolder.objectNumber = 0;
      fileUpload.FileInFolder.objectType = 'DXTL';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    this.loading = true;
    let result: any = await this.employeeService.updateDeXuatTangLuong(deXuatTangLuong, this.listEmpSelected, listFileUploadModel, "DXTL");
    this.loading = false;
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
      this.showMessage(msg);
      return;
    }
    let msg = { severity: 'success', summary: 'Thông báo:', detail: result.message };
    this.showMessage(msg);
    this.getMasterData();
  }

  importExcel() {
    if (this.fileName == "") {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Chưa chọn file cần nhập" };
      this.showMessage(mgs);
      return;
    }
    const targetFiles: DataTransfer = <DataTransfer>(this.importFileExcel);
    const reader: FileReader = new FileReader();
    reader.readAsBinaryString(targetFiles.files[0]);
    reader.onload = (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary', cellDates: true });
      let sheetName = 'TangLuongNV_DeXuat';
      if (!workbook.Sheets[sheetName]) {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: "File không hợp lệ" };
        this.showMessage(mgs);
        return;
      }

      //lấy data từ file excel
      const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[sheetName];
      /* save data */
      let listCustomerRawData: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
      /* remove header */
      listCustomerRawData = listCustomerRawData.filter((e, index) => index != 0 && index != 1);
      /* nếu không nhập  trường required thì loại bỏ */
      listCustomerRawData = listCustomerRawData.filter(e => (e[0] && e[1]));
      /* chuyển từ raw data sang model */
      let listEmpImport: Array<importNVByExcelModel> = [];
      listCustomerRawData?.forEach(_rawData => {
        /*
        empCode: string;
        empName: string;
        oranganizationName: string;
        positionName: string;
        mucLuongCu: string;
        luongDeXuat: string;
       */
        let customer = new importNVByExcelModel();
        customer.empCode = _rawData[0] ? _rawData[0].toString().trim() : '';
        customer.luongDeXuat = _rawData[1] ? _rawData[1] : null;
        customer.lydo = _rawData[2] ? _rawData[2] : "";
        listEmpImport = [...listEmpImport, customer];
      });
      /* tắt dialog import file, bật dialog chi tiết khách hàng import */
      this.displayChooseFileImportDialog = false;
      this.openDetailImportDialog(listEmpImport);
    }
  }

  importEmp() {
    this.displayChooseFileImportDialog = true;
  }

  openDetailImportDialog(listCustomerImport) {
    let ref = this.dialogService.open(ImportNvDeXuatTangLuongComponent, {
      data: {
        listEmpImport: listCustomerImport,
        listAdded: this.listEmpSelected,
        listAllEmp: this.listEmpAdd
      },
      header: 'Nhập excel danh sách nhân viên đề xuất thay tăng lương',
      width: '85%',
      baseZIndex: 1050,
      contentStyle: {
        "max-height": "800px",
        // "over-flow": "hidden"
      }
    });
    ref.onClose.subscribe((result: any) => {
      if (result.status) {
        let listImport = this.mapLuongCu(result.returnList);
        this.listEmpSelected = this.listEmpSelected.concat(listImport);
        this.tinhTongTang();
        this.refreshFilter();
      }
    });
  }

  //map mức lương cũ với nhân viên
  mapLuongCu(returnList) {
    returnList.forEach((item) => {
      let empInfor = this.listEmpAdd.find(x => x.employeeCode == item.employeeCode);
      if (empInfor) {
        item.luongHienTai = empInfor.mucLuongHienTai != null ? empInfor.mucLuongHienTai : 0;
        item.mucChechLech = item.luongDeXuat - item.luongHienTai;
      }
    });
    return returnList;
  }



  tinhTongTang() {
    this.tongTang = 0;
    this.listEmpSelected.forEach(item => {
      this.tongTang = this.tongTang + item.mucChechLech;
    });
    this.quyLuongConLai = this.quyLuongConLaiRoot - this.tongTang;
  }

  closeChooseFileImportDialog() {
    this.cancelFile();
  }

  cancelFile() {
    let fileInput = $("#importFileProduct")
    fileInput.replaceWith(fileInput.val('').clone(true));
    this.fileName = "";
  }

  onClickImportBtn(event: any) {
    /* clear value của file input */
    event.target.value = ''
  }

  chooseFile(event: any) {
    this.fileName = event.target?.files[0]?.name;
    this.importFileExcel = event.target;
  }


  searchEmployee() {
    let listPhongBan: Array<string> = [];
    let listChucVu = [];
    let listTrangThai = [];

    if (this.phongBanSearch) listPhongBan = this.phongBanSearch.map(x => x.organizationName);
    if (this.chucVuSearch) listChucVu = this.chucVuSearch.map(x => x.positionName);
    if (this.trangThaiSearch) listTrangThai = this.trangThaiSearch.map(x => x.value);
    this.listEmpSelectedFilter = this.listEmpSelected.filter(x => (this.phongBanSearch == null || listPhongBan.indexOf(x.organizationName) > -1) &&
      (this.chucVuSearch == null || listChucVu.indexOf(x.positionName) > -1) &&
      (this.trangThaiSearch == null || listTrangThai.indexOf(x.trangThai) > -1));
  }



  openItem(name, url) {
    this.imageService.downloadFile(name, url).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if ((window.navigator as any) && (window.navigator as any).msSaveOrOpenBlob) {
        (window.navigator as any).msSaveOrOpenBlob(file);
      } else {
        var fileURL = URL.createObjectURL(file);
        if (fileType.indexOf('image') !== -1) {
          window.open(fileURL);
        } else {
          var anchor = document.createElement("a");
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    }, error => { });
  }

  downloadFile(data: FileInFolder) {
    this.folderService.downloadFile(data.fileInFolderId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        var binaryString = atob(result.fileAsBase64);
        var fileType = result.fileType;
        var binaryLen = binaryString.length;
        var bytes = new Uint8Array(binaryLen);
        for (var idx = 0; idx < binaryLen; idx++) {
          var ascii = binaryString.charCodeAt(idx);
          bytes[idx] = ascii;
        }
        var file = new Blob([bytes], { type: fileType });
        if ((window.navigator as any) && (window.navigator as any).msSaveOrOpenBlob) {
          (window.navigator as any).msSaveOrOpenBlob(file);
        } else {
          var fileURL = URL.createObjectURL(file);
          if (fileType.indexOf('image') !== -1) {
            window.open(fileURL);
          } else {
            var anchor = document.createElement("a");
            anchor.download = data.fileName.substring(0, data.fileName.lastIndexOf('_')) + "." + data.fileExtension;
            anchor.href = fileURL;
            anchor.click();
          }
        }
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  /*Event Lưu các file được chọn*/
  handleFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;
      if (size <= this.defaultLimitedFileSize) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedFiles.push(file);
          }
        }
      }
    }
  }

  /*Event Khi click xóa từng file*/
  removeFile(event) {
    let index = this.uploadedFiles.indexOf(event.file);
    this.uploadedFiles.splice(index, 1);
  }


  /*Event upload list file*/
  myUploader(event) {
    let listFileUploadModel: Array<FileUploadModel> = [];
    this.uploadedFiles.forEach(item => {
      let fileUpload: FileUploadModel = new FileUploadModel();
      fileUpload.FileInFolder = new FileInFolder();
      fileUpload.FileInFolder.active = true;
      let index = item.name.lastIndexOf(".");
      let name = item.name.substring(0, index);
      fileUpload.FileInFolder.fileName = name;
      fileUpload.FileInFolder.fileExtension = item.name.substring(index, item.name.length - index);
      fileUpload.FileInFolder.size = item.size;
      fileUpload.FileInFolder.objectNumber = this.deXuatTLId;
      fileUpload.FileInFolder.objectType = 'DXTL';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });


    this.employeeService.uploadFile("DXTL", listFileUploadModel, null, this.deXuatTLId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.uploadedFiles = [];

        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }

        this.arrayDocumentModel = result.listFileInFolder;
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Thêm file thành công" };
        this.showMessage(mgs);
      } else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.message };
        this.showMessage(mgs);
      }
    });
  }

  /*Event Khi click xóa toàn bộ file*/
  clearAllFile() {
    this.uploadedFiles = [];
  }

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: any) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.employeeService.deleteFile(file.fileInFolderId).subscribe(res => {
          let result: any = res;
          if (result.statusCode == 200) {
            let index = this.arrayDocumentModel.indexOf(file);
            this.arrayDocumentModel.splice(index, 1);
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Xóa file thành công" };
            this.showMessage(mgs);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          }
        })
      }
    });
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  onViewPhieuTuDanhGia(danhGiaNhanVienId) {
    this.router.navigate(['/employee/thuc-hien-danh-gia', { danhGiaNhanVienId: this.encrDecrService.set(danhGiaNhanVienId) }]);
  }

  async exportWord() {
    let ngayApDungLuong = convertDate(new Date(this.ngayApDungLuongFormControl.value))
    let data = {
      template: 2,
      ngayApDungLuong: ngayApDungLuong,
      date: new Date().getDate(),
      month: new Date().getMonth() + 1,
      year: new Date().getFullYear()
    }
    this.exportFileWordService.saveFileWord(data, 'Quyết định tăng lương.docx');
    this.exportExcel();
  }
}

function convertDate(time: any) {
  let ngay = time.getDate()
  let thang = time.getMonth() + 1
  let nam = time.getFullYear()
  return `${ngay}/${thang}/${nam}`
};

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

