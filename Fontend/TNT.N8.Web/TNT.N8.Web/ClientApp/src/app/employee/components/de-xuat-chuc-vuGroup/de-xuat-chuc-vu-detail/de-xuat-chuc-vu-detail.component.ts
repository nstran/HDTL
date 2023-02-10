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
import { ImportNvDeXuatTangLuongComponent } from '../../de-xuat-tang-luongGroup/importNv-de-xuat-tang-luong/importNv-de-xuat-tang-luong.component';
import { QuyTrinhService } from '../../../../admin/services/quy-trinh.service';
import { NoteService } from '../../../../shared/services/note.service';
import { DeXuatChucVuModel, NhanVienDeXuatThayDoiChucVu } from '../../../models/de-xuat-chuc-vu';
import { DateAdapter } from '@angular/material/core';
import { ImportNvDeXuatChucVuComponent } from '../importNv-de-xuat-chuc-vu/importNv-de-xuat-chuc-vu.component';
import { ForderConfigurationService } from '../../../../admin/components/folder-configuration/services/folder-configuration.service';
import { ImageUploadService } from '../../../../shared/services/imageupload.service';
import { EncrDecrService } from '../../../../shared/services/encrDecr.service';
import { ExportFileWordService } from '../../../../shared/services/exportFileWord.services'



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
  constructor() { }
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
  empId: string;
  empCode: string;
  empName: string;
  oranganizationName: string;
  positionName: string;
  chucVuDeXuat: string;
  chucVuDeXuatId: string;
  lydo: string;
}

class EmployeeInterface {
  nhanVienDeXuatThayDoiChucVuId: number;
  deXuatThayDoiChucVuId: number;
  employeeId: string;
  employeeName: string;
  organizationName: string;
  PhongBanId: string;
  employeeCode: string;
  DateOfBirth: Date;
  lyDoDeXuat: string;
  positionName: string;
  chucVuHienTaiId: string;
  chucVuDeXuatId: string;
  positionNameDx: string;
  trangThai: number;
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
  selector: 'app-de-xuat-chuc-vu-detail',
  templateUrl: './de-xuat-chuc-vu-detail.component.html',
  styleUrls: ['./de-xuat-chuc-vu-detail.component.css']
})



export class DeXuatChucVuDetailComponent implements OnInit {


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


  //Form của phiếu đề xuất chức vụ
  deXuatTangLuongFormGroup: FormGroup;
  tenDeXuatFormControl: FormControl;
  ngayDeXuatFormControl: FormControl;
  nguoiDeXuatFormControl: FormControl;


  //Ds nhân viên được đề xuất
  nhanVienFormGroup: FormGroup;
  EmployeeIdFormControl: FormControl;
  EmployeeNameFormControl: FormControl;
  OranganizationIdFormControl: FormControl;
  OranganizationNameFormControl: FormControl;
  EmployeeCodeFormControl: FormControl;
  PositisionIdFormControl: FormControl;
  PositisionNameFormControl: FormControl;
  DateOfBirthFormControl: FormControl;

  ChucVuCuFormControl: FormControl;
  ChucVuDXFormControl: FormControl;
  LyDoDXFormControl: FormControl;

  //Form của ngày áp dụng chức vụ
  ngayApDungMoiFormGroup: FormGroup;
  ngayApDungCVFormControl: FormControl;

  IsEditNV: boolean = false;
  loginEmpId: string = '';

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
  thongTinNhanVienDuocDeXuat: any

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

  isAprovalQuote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  @ViewChild('fileNoteUpload') fileNoteUpload: FileUpload;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];
  colsFile: any[];
  /*End : Note*/
  file: File[];

  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionDownLoad: boolean = true;
  actionUpLoad: boolean = true;
  isManagerOfHR: boolean = false;
  isGD: boolean = false;
  viewNote: boolean = true;
  viewTimeline: boolean = true;
  statusCode: string = null;
  pageSize = 20;


  isNguoiPhuTrach: boolean = false;

  chucVuList: any = [];
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
    private folderService: ForderConfigurationService,
    private imageService: ImageUploadService,
    private encrDecrService: EncrDecrService,
    public exportFileWordService: ExportFileWordService,
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
    let resource = "hrm/employee/de-xuat-chuc-vu-detail/";
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
    this.ngayApDungCVFormControl = new FormControl(null, [Validators.required]);

    this.ngayApDungMoiFormGroup = new FormGroup({
      ngayApDungCVFormControl: this.ngayApDungCVFormControl
    });



    this.tenDeXuatFormControl = new FormControl(null, [Validators.required]);
    this.ngayDeXuatFormControl = new FormControl(null, [Validators.required]);
    this.nguoiDeXuatFormControl = new FormControl(null, [Validators.required]);

    this.deXuatTangLuongFormGroup = new FormGroup({
      tenDeXuatFormControl: this.tenDeXuatFormControl,
      ngayDeXuatFormControl: this.ngayDeXuatFormControl,
      nguoiDeXuatFormControl: this.nguoiDeXuatFormControl
    });


    this.EmployeeIdFormControl = new FormControl(null, [Validators.required]);
    this.EmployeeNameFormControl = new FormControl(null);
    this.OranganizationIdFormControl = new FormControl();
    this.OranganizationNameFormControl = new FormControl(null);
    this.ChucVuCuFormControl = new FormControl(null);
    this.ChucVuDXFormControl = new FormControl(null, [Validators.required]);
    this.EmployeeCodeFormControl = new FormControl(null);
    this.PositisionIdFormControl = new FormControl(null);
    this.PositisionNameFormControl = new FormControl(null);
    this.DateOfBirthFormControl = new FormControl(null);
    this.LyDoDXFormControl = new FormControl(null, [Validators.required]);

    this.nhanVienFormGroup = new FormGroup({
      EmployeeIdFormControl: this.EmployeeIdFormControl,
      EmployeeNameFormControl: this.EmployeeNameFormControl,
      OranganizationIdFormControl: this.OranganizationIdFormControl,
      OranganizationNameFormControl: this.OranganizationNameFormControl,
      ChucVuDXFormControl: this.ChucVuDXFormControl,
      EmployeeCodeFormControl: this.EmployeeCodeFormControl,
      PositisionIdFormControl: this.PositisionIdFormControl,
      PositisionNameFormControl: this.PositisionNameFormControl,
      DateOfBirthFormControl: this.DateOfBirthFormControl,
      ChucVuCuFormControl: this.ChucVuCuFormControl,
      LyDoDXFormControl: this.LyDoDXFormControl,
    });
    this.ngayDeXuatFormControl.setValue(new Date());
  }


  setCols() {
    this.colsListEmp = [
      { field: 'employeeCode', header: 'Mã nhân viên', textAlign: 'left', display: 'table-cell', width: "125px" },
      { field: 'employeeName', header: 'Họ tên', textAlign: 'left', display: 'table-cell', width: "150px" },
      { field: 'organizationName', header: 'Phòng ban', textAlign: 'left', display: 'table-cell', width: '200px' },
      { field: 'positionName', header: 'Chức vụ', textAlign: 'center', display: 'table-cell', width: '95px' },
      { field: 'positionNameDx', header: 'Chức vụ đề xuất', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'lyDoDeXuat', header: 'Lý do đề xuất', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'trangThai', header: 'Trạng thái', textAlign: 'left', display: 'table-cell', width: '100px' },
      { field: 'lyDo', header: 'Ghi chú', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'nghiaVu', header: 'Quyền và nghĩa vụ', textAlign: 'left', display: 'table-cell', width: '150px' },
    ];

    this.colsFile = [
      { field: 'fileName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'right', type: 'number' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'right', type: 'date' },
      { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'left', type: 'string' },
    ];

    if (this.trangThaiDeXuat == 1) {
      this.selectedColumns = this.colsListEmp.filter(x => x.header != "Lý do đề xuất" && x.header != "Trạng thái" && x.header != "Ghi chú" && x.header != "Nghĩa vụ");
    } else {
      this.selectedColumns = this.colsListEmp;
    }
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.employeeService.getMasterDataCreateDeXuatChucVu();
    this.loading = false;
    await this.getDuLieuQuyTrinh();
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Lấy thông tin không thành công' };
      this.showMessage(msg);
      return;
    }
    this.listEmpAdd = result.listEmp;
    this.chucVuList = result.listPosition;


    this.loading = true;

    let resultDetail: any = await this.employeeService.deXuatChucVuDetail(this.deXuatTLId);
    this.loading = false;
    if (resultDetail.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Lấy thông tin không thành công' };
      this.showMessage(msg);
      return;
    }

    this.thongTinNhanVienDuocDeXuat = resultDetail.nhanVienDuocDeXuats

    this.selectedEmpCheckBox = [];

    this.companyConfig = resultDetail.companyConfig;

    this.setDefaultValue(resultDetail);

    this.setCols();

    this.arrayDocumentModel = resultDetail.listFileInFolder;
    this.uploadedFiles = [];

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
      this.EmployeeCodeFormControl.setValue(emp.value.employeeCode);
      this.PositisionIdFormControl.setValue(emp.value.positionId);
      this.PositisionNameFormControl.setValue(emp.value.positionName);
      this.DateOfBirthFormControl.setValue(new Date(emp.value.dateOfBirth));
      this.LyDoDXFormControl.setValue("");
      this.ChucVuCuFormControl.setValue(emp.value.positionId);
      // this.ChucVuDXFormControl.setValue(this.chucVuList[0]);
    }
  }

  setDefaultValue(resultDetail) {
    this.tenDeXuatFormControl.setValue(resultDetail.deXuatChucVu.tenDeXuat);
    this.ngayDeXuatFormControl.setValue(new Date(resultDetail.deXuatChucVu.ngayDeXuat));
    this.nguoiDeXuatFormControl.setValue(this.listEmpAdd.find(x => x.employeeId == resultDetail.deXuatChucVu.nguoiDeXuatId));
    this.nguoiDeXuatFormControl.disable();

    //danh sách nhân viên
    this.listEmpSelected = resultDetail.nhanVienDuocDeXuats;
    this.listEmpSelectedFilter = resultDetail.nhanVienDuocDeXuats;

    this.trangThaiDeXuat = resultDetail.deXuatChucVu.trangThai;
    this.loaiDeXuatType = resultDetail.deXuatChucVu.loaiDeXuat;

    this.listChucVu = resultDetail.listPosition;
    this.listPhongBan = resultDetail.listOrganization;

    //Ngày áp dụng mức lương mới
    if (this.trangThaiDeXuat == 3) {
      if (resultDetail.deXuatChucVu.ngayApDung) this.ngayApDungCVFormControl.setValue(new Date(resultDetail.deXuatChucVu.ngayApDung));
    }

    //Nếu trạng thái là mới thì disable from
    if (this.trangThaiDeXuat == 1) {
      this.deXuatTangLuongFormGroup.enable();
    } else {
      this.deXuatTangLuongFormGroup.disable();
    }

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
    newEmp.chucVuHienTaiId = this.PositisionIdFormControl.value;
    newEmp.positionName = this.PositisionNameFormControl.value;
    newEmp.lyDoDeXuat = this.LyDoDXFormControl.value;
    newEmp.positionNameDx = this.ChucVuDXFormControl.value.positionName;
    newEmp.chucVuDeXuatId = this.ChucVuDXFormControl.value.positionId;
    newEmp.trangThai = 1; //trạng thái mới
    var checkExist = this.listEmpSelected.filter(x => x.employeeId == this.EmployeeIdFormControl.value.employeeId);
    if (checkExist.length != 0) {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Nhân viên đã tồn tại trong danh sách đề xuất' };
      this.showMessage(msg);
      return;
    }

    this.listEmpSelected.push(newEmp);
    this.def.detectChanges();
    this.refreshFilter();
    this.clearEmp();
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

    this.loading = true;
    let resultDetail: any = await this.employeeService.tuChoiOrPheDuyetNhanVienDeXuatCV(this.selectedEmpCheckBox, this.deXuatTLId, IsXacNhan, this.lyDoTuChoi);
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
    this.router.navigate(['/employee/danh-sach-de-xuat-chuc-vu']);
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

    let newEmp = new EmployeeInterface();
    newEmp.employeeId = this.EmployeeIdFormControl.value.employeeId;
    newEmp.employeeName = this.EmployeeIdFormControl.value.employeeName;
    newEmp.PhongBanId = this.OranganizationIdFormControl.value;
    newEmp.organizationName = this.OranganizationNameFormControl.value;
    newEmp.employeeCode = this.EmployeeCodeFormControl.value;
    newEmp.chucVuHienTaiId = this.PositisionIdFormControl.value;
    newEmp.positionName = this.PositisionNameFormControl.value;
    newEmp.lyDoDeXuat = this.LyDoDXFormControl.value;
    newEmp.positionNameDx = this.ChucVuDXFormControl.value.positionName;
    newEmp.chucVuDeXuatId = this.ChucVuDXFormControl.value.positionId;
    newEmp.trangThai = 1; // trạng thái mới
    this.listEmpSelected = await this.listEmpSelected.filter(x => x.employeeId != newEmp.employeeId);
    this.listEmpSelected.push(newEmp);
    this.def.detectChanges();
    this.clearEmp();
    this.refreshFilter();
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
    this.ChucVuDXFormControl.setValue(this.chucVuList.find(x => x.positionId == rowData.chucVuDeXuatId));
    this.EmployeeCodeFormControl.setValue(rowData.employeeCode);
    this.PositisionIdFormControl.setValue(rowData.PositionId);
    this.PositisionNameFormControl.setValue(rowData.positionName);
    this.DateOfBirthFormControl.setValue(rowData.DateOfBirth);
    this.ChucVuCuFormControl.setValue(rowData.positionId);
    this.LyDoDXFormControl.setValue(rowData.lyDoDeXuat);
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
    let title = `Danh sách nhân viên đề xuất thay đổi chức vụ`;

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
    let dataHeaderMain = "Danh sách nhân viên đề xuất thay đổi chức vụ".toUpperCase();
    let headerMain = worksheet.addRow([dataHeaderMain]);
    headerMain.font = { size: 18, bold: true };
    worksheet.mergeCells(`A${7}:G${7}`);
    headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    worksheet.addRow([]);

    let dataHeaderRow1: Array<string> = [];
    dataHeaderRow1 = ["", "Mã nhân viên", "Tên nhân viên", "Phòng ban", "Chức vụ", "Chức vụ đề xuất", "Ly do đề xuất", "Trạng thái", "Ghi chú"];

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
      row[0] = '';
      row[1] = item.employeeCode;
      row[2] = item.employeeName;
      row[3] = item.organizationName;
      row[4] = item.positionName;
      row[5] = item.positionNameDx;
      row[6] = item.lyDoDeXuat;
      if (item.trangThai == 1) row[7] = "Mới";
      if (item.trangThai == 2) row[7] = "Chờ phê duyệt";
      if (item.trangThai == 3) row[7] = "Phê duyệt";
      if (item.trangThai == 4) row[7] = "Từ chối";
      row[8] = "Ghi chú này";
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

    this.exportToExel(workBook, title);

  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }



  async getDuLieuQuyTrinh() {
    this.quyTrinhService.getDuLieuQuyTrinh(this.emptyGuid, 11, this.deXuatTLId).subscribe(res => {
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
    this.quyTrinhService.guiPheDuyet(this.emptyGuid, 11, this.deXuatTLId).subscribe(res => {
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
      message: 'Bạn chắc chắn muốn đặt về mới đề xuất này?',
      accept: async () => {
        let res: any = await this.employeeService.datVeMoiDeXuatChucVu(this.deXuatTLId);
        if (res.statusCode === 200) {
          let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Đặt về mới đề xuất thay dổi chức vụ thành công" };
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
      if (!this.ngayApDungMoiFormGroup.valid) {
        Object.keys(this.ngayApDungMoiFormGroup.controls).forEach(key => {
          if (this.ngayApDungMoiFormGroup.controls[key].valid == false) {
            this.ngayApDungMoiFormGroup.controls[key].markAsTouched();
          }
        });
        let mgs = { severity: 'warn', summary: 'Thông báo:', detail: "Nhập ngày áp dụng chức vụ mới!" };
        this.showMessage(mgs);
        return;
      }

      let ngayApDung = null;
      if (this.IsShowNgayApDung) {
        ngayApDung = convertToUTCTime(new Date(this.ngayApDungCVFormControl.value));
      }
      let result: any = await this.employeeService.capNhatNgayApDungDeXuat(ngayApDung, this.deXuatTLId, 2);
      if (result.statusCode != 200) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
        return;
      }
    }

    this.loading = true;
    this.quyTrinhService.pheDuyet(this.emptyGuid, 11, '', this.deXuatTLId).subscribe(res => {
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

    this.loading = true;
    this.quyTrinhService.tuChoi(this.emptyGuid, 11, this.lyDoTuChoi, this.deXuatTLId).subscribe(res => {
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
      message: 'Bạn có chắc chắn muốn hủy yêu cầu xác nhận đề xuất này không?',
      accept: async () => {
        // await this.createOrUpdate(false);

        this.loading = true;
        this.quyTrinhService.huyYeuCauPheDuyet(this.emptyGuid, 11, this.deXuatTLId).subscribe(res => {
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
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Hãy chọn nhân viên được đề xuất thay đổi chức vụ!' };
      this.showMessage(msg);
      return;
    }

    let deXuatTangLuong = new DeXuatChucVuModel();
    deXuatTangLuong.deXuatThayDoiChucVuId = this.deXuatTLId;
    deXuatTangLuong.ngayDeXuat = this.ngayDeXuatFormControl.value;
    deXuatTangLuong.tenDeXuat = this.tenDeXuatFormControl.value;
    deXuatTangLuong.nguoiDeXuatId = this.nguoiDeXuatFormControl.value.employeeId;
    deXuatTangLuong.trangThai = this.trangThaiDeXuat;


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
      fileUpload.FileInFolder.objectType = 'DXCV';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });


    this.loading = true;
    let result: any = await this.employeeService.updateDeXuatChucVu(deXuatTangLuong, this.listEmpSelected, listFileUploadModel, "DXCV");
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


  async lyDoPheDuyetOrTuChoi(rowData, type) {
    if (type == 1 && (rowData.lyDo.trim() == '' || rowData.lyDo.trim() == null)) return;
    if (type == 2 && (rowData.nghiaVu.trim() == '' || rowData.nghiaVu.trim() == null)) return;
    let result: any = await this.employeeService.capNhatLyDoPheDuyetOrTuChoiDeXuatNV(rowData.nhanVienDeXuatThayDoiChucVuId, rowData.lyDo, 2, rowData.nghiaVu);
    this.loading = false;
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
      this.showMessage(msg);
      return;
    }
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
      let sheetName = 'DoiChucVuNV_DeXuat';
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
        empId: number;
        empCode: string;
        empName: string;
        oranganizationName: string;
        positionName: string;
        chucVuDeXuat: number;
        chucVuDeXuatId: number;
        lydo: string;
       */
        let customer = new importNVByExcelModel();
        customer.empCode = _rawData[0] ? _rawData[0].toString().trim() : '';
        customer.chucVuDeXuat = _rawData[1] ? _rawData[1].toString().trim() : '';
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
    let ref = this.dialogService.open(ImportNvDeXuatChucVuComponent, {
      data: {
        listEmpImport: listCustomerImport,
        listAdded: this.listEmpSelected,
        listAllEmp: this.listEmpAdd,
        listPosition: this.chucVuList,
      },
      header: 'Nhập excel danh sách nhân viên đề xuất thay đổi chức vụ',
      width: '85%',
      baseZIndex: 1050,
      contentStyle: {
        "max-height": "800px",
        // "over-flow": "hidden"
      }
    });
    ref.onClose.subscribe((result: any) => {
      if (result.status) {
        this.listEmpSelected = this.listEmpSelected.concat(result.returnList);
        this.refreshFilter();
      }
    });

  }


  /*Event Khi click xóa toàn bộ file*/
  clearAllFile() {
    this.uploadedFiles = [];
  }

  /*Event Khi click xóa từng file*/
  removeFile(event) {
    let index = this.uploadedFiles.indexOf(event.file);
    this.uploadedFiles.splice(index, 1);
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
  async exportWord() {
    for (let i = 0; i < this.thongTinNhanVienDuocDeXuat.length; i++) {

      let data = {
        template: 3,
        positionNameDx: this.thongTinNhanVienDuocDeXuat[i].positionNameDx,
        employeeName: this.thongTinNhanVienDuocDeXuat[i].employeeName,
        employeeCode: this.thongTinNhanVienDuocDeXuat[i].employeeCode,
        dateOfBirth: convertDate(new Date(this.thongTinNhanVienDuocDeXuat[i].dateOfBirth)),
        identity: this.thongTinNhanVienDuocDeXuat[i].identityId,
        identityIddateOfIssue: convertDate(new Date(this.thongTinNhanVienDuocDeXuat[i].identityIddateOfIssue)),
        identityIdplaceOfIssue: this.thongTinNhanVienDuocDeXuat[i].identityIdplaceOfIssue,
        hoKhauThuongTruTv: this.thongTinNhanVienDuocDeXuat[i].hoKhauThuongTruTv,
        address: this.thongTinNhanVienDuocDeXuat[i].address,
        nghiaVu: this.thongTinNhanVienDuocDeXuat[i].nghiaVu,

        date: new Date().getDate(),
        month: new Date().getMonth() + 1,
        year: new Date().getFullYear(),
      }
      this.exportFileWordService.saveFileWord(data, `Quyết định bổ nhiệm - ${data.employeeCode}_${data.employeeName}.docx`)
    }

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
