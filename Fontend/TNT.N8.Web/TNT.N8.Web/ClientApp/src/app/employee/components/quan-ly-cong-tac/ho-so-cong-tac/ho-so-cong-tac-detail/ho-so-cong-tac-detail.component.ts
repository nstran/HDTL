import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { GetPermission } from '../../../../../../app/shared/permission/get-permission';
import { MessageService, MenuItem } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { Paginator, Table } from 'primeng';
import { QuanLyCongTacService } from '../../../../services/quan-ly-cong-tac/quan-ly-cong-tac.service';
import { HoSoCongTacModel } from '../../../../models/ho-so-cong-tac';
import { NoteDocumentModel } from '../../../../../shared/models/note-document.model';
import { NoteService } from '../../../../../../app/shared/services/note.service';
import { EncrDecrService } from '../../../../../shared/services/encrDecr.service';

export class CategoryModel {
  categoryId: string;
  categoryCode: string;
  categoryName: string;

  constructor() {
    this.categoryId = '00000000-0000-0000-0000-000000000000';
    this.categoryCode = '';
    this.categoryName = '';
  }
}
export class EmployeeModel {
  EmployeeId: string;
  EmployeeName: string;
  StartedDate: Date;
  OrganizationId: string;
  PositionId: string;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: Boolean;
  deXuatCongTacId: number;
  constructor() { }
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

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
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
@Component({
  selector: 'app-ho-so-cong-tac-detail',
  templateUrl: './ho-so-cong-tac-detail.component.html',
  styleUrls: ['./ho-so-cong-tac-detail.component.css']
})
export class ChiTietHoSoCongTacComponent implements OnInit {
  today: Date = new Date();
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('paginator', { static: true }) paginator: Paginator
  @ViewChild('myTable') myTable: Table;
  /* End */
  /*Khai báo biến*/
  auth: any = JSON.parse(localStorage.getItem("auth"));
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;
  /** Action */
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  /**End */

  /* Form */
  hoSoCongTacForm: FormGroup;
  ketQuaCTControl: FormControl;
  /* End */

  isShow: boolean = true;
  fixed: boolean = false;
  withFiexd: string = "";
  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();

  // Dữ liệu masterdata
  listQuyetDinhCT: Array<any> = new Array<any>();
  listNhanVienCT: Array<EmployeeModel> = new Array<EmployeeModel>();

  listDeNghiTamUng: Array<any> = new Array<any>();
  listDeNghiHoanUng: Array<any> = new Array<any>();

  listTamHoanUng: Array<EmployeeModel> = new Array<EmployeeModel>();
  deXuatCTModel: any;

  // notification
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;

  colsList: any[];
  colsHTU: any[];

  // Tài liệu liên quan
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];
  colsFile: any[];
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  employeeModel: EmployeeModel = new EmployeeModel();
  isUpdate: boolean = false;

  hoSoCongTacId: number;

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
  @ViewChild('fileUpload') fileUpload: FileUpload;

  hoSoCongTacModel: HoSoCongTacModel = new HoSoCongTacModel();

  refreshNote: number = 0;

  constructor(
    private quanLyCongtacService: QuanLyCongTacService,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private getPermission: GetPermission,
    private el: ElementRef,
    private ref: ChangeDetectorRef,
    private noteService: NoteService,
    private encrDecrService: EncrDecrService,
  ) { }

  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }

  async ngOnInit() {
    this.setTable();
    this.setForm();
    let resource = "hrm/employee/chi-tiet-ho-so-cong-tac";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }
      this.route.params.subscribe(params => { this.hoSoCongTacId = Number(this.encrDecrService.get(params['hoSoCongTacId'])) });
      this.getMasterData();
    }
  }

  setForm() {
    this.ketQuaCTControl = new FormControl(null);

    this.hoSoCongTacForm = new FormGroup({
      ketQuaCTControl: this.ketQuaCTControl,
    });
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.quanLyCongtacService.getDataHoSoCTDetail(this.hoSoCongTacId, this.auth.UserId);
    this.loading = false;
    if (result.statusCode == 200) {
      this.mappingHoSoCongTacModel(result.hoSoCongTac);
      // Danh sách nhân viên
      this.listNhanVienCT = result.hoSoCongTac.listNhanVienCT;

      // Hoàn ứng
      this.listDeNghiHoanUng = result.listDeNghiTamHoanUng.filter(x => x.loaiDeNghi == 1)

      // Tạm ứng
      this.listDeNghiTamUng = result.listDeNghiTamHoanUng.filter(x => x.loaiDeNghi == 0);

      this.arrayDocumentModel = result.listFileInFolder;

      this.ref.detectChanges();
    }
  }

  mappingHoSoCongTacModel(model: any) {
    this.hoSoCongTacModel.maHoSoCongTac = model.maHoSoCongTac;

    this.hoSoCongTacModel.deXuatCongTac = model.tenDeXuat;

    this.hoSoCongTacModel.donViDen = model.donVi;

    this.hoSoCongTacModel.ngayBatDau = model.thoiGianBatDau;

    this.hoSoCongTacModel.ngayKetThuc = model.thoiGianKetThuc;

    this.hoSoCongTacModel.diaDiem = model.diaDiem;

    this.hoSoCongTacModel.phuongTien = model.phuongTien;

    this.hoSoCongTacModel.nhiemVu = model.nhiemVu;

    this.hoSoCongTacModel.ketQuaCT = model.ketQua;

    this.ketQuaCTControl.setValue(this.hoSoCongTacModel.ketQuaCT);

    this.hoSoCongTacModel.trangThai = model.trangThai;

    this.hoSoCongTacModel.trangThaiText = model.trangThaiText;

    if (this.hoSoCongTacModel.trangThai == 1) {
      this.hoSoCongTacForm.disable();
    }

    this.ref.detectChanges();
  }

  onViewDetail(rowData: any) {
    let url = this.router.serializeUrl(this.router.createUrlTree(['/employee/detail', { employeeId: rowData.employeeId, contactId: rowData.contactId }]));
    window.open(url, '_blank');
  }

  setTable() {
    this.colsList = [
      { field: 'employeeName', header: 'Họ tên', width: '200px', textAlign: 'left', color: '#f44336' },
      { field: 'employeeCode', header: 'Mã nhân viên', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'organizationName', header: 'Phòng ban', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'positionName', header: 'Chức vụ', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'dateOfBirth', header: 'Ngày sinh', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'identity', header: 'Số CMT/CCCD', width: '100px', textAlign: 'left', color: '#f44336' },
    ];

    this.colsFile = [
      { field: 'fileFullName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', color: '#f44336' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'right', color: '#f44336' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'center', color: '#f44336' },
      { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'left', color: '#f44336' },
    ];

    this.colsHTU = [
      { field: 'maDeNghi', header: 'Mã đề nghị', width: '130px', textAlign: 'center', color: '#f44336' },
      { field: 'nguoiThuHuong', header: 'Người thụ hưởng', width: '250px', textAlign: 'left', color: '#f44336' },
      { field: 'tongTienThanhToan', header: 'Tổng tiền thanh toán', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'trangThaiString', header: 'Trạng thái', width: '80px', textAlign: 'center', color: '#f44336' },
    ];
  }

  capNhapHoSoCongTac() {
    this.loading = true;
    this.awaitResult = true;

    let hoSoCTModel: HoSoCongTacModel = this.mapFormToHoSoCTModel();

    this.quanLyCongtacService.createOrUpdateHoSoCT(hoSoCTModel, 'HOSOCT', [], this.auth.UserId).subscribe(response => {
      this.loading = false;
      let result = <any>response;
      if (result.statusCode == 200) {
        this.showToast('success', 'Thông báo', "Cập nhập hồ sơ công tác thành công");
        this.getMasterData();
      }
      else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.message);
      }
      this.awaitResult = false;
    });
  }

  taoDeNghiTamUng() {
    let url = this.router.serializeUrl(this.router.createUrlTree(['/employee/tao-de-nghi-tam-hoan-ung', { hoSoCongTacId: this.encrDecrService.set(this.hoSoCongTacId), loaiDeNghi: 0 }]));
    window.open(url, '_blank');
  }

  taoDeNghiHoanUng() {
    let url = this.router.serializeUrl(this.router.createUrlTree(['/employee/tao-de-nghi-tam-hoan-ung', { hoSoCongTacId: this.encrDecrService.set(this.hoSoCongTacId), loaiDeNghi: 1 }]));
    window.open(url, '_blank');
  }

  hoanThanh() {
    let hoSoCTModel: HoSoCongTacModel = this.mapFormToHoSoCTModel();

    this.loading = true;
    this.awaitResult = true;
    this.quanLyCongtacService.createOrUpdateHoSoCT(hoSoCTModel, 'HOSOCT', [], this.auth.UserId).subscribe(async response => {
      let result = <any>response;
      
      if (result.statusCode != 200) {
        this.loading = false;
        this.awaitResult = false;
        this.clearToast();
        this.showToast('error', 'Thông báo', result.message);
        return;
      }

      let _result: any = await this.quanLyCongtacService.hoanThanhHoSoCongTac(this.hoSoCongTacId);

      if (_result.statusCode != 200) {
        this.loading = false;
        this.awaitResult = false;
        this.clearToast();
        this.showToast('error', 'Thông báo', _result.message);
        return;
      }

      this.getMasterData();
    });
  }

  mapFormToHoSoCTModel(): HoSoCongTacModel {
    let hoSoCTModel = new HoSoCongTacModel();

    hoSoCTModel.hoSoCongTacId = this.hoSoCongTacId;
    hoSoCTModel.ketQuaCT = this.ketQuaCTControl.value == null ? '' : this.ketQuaCTControl.value;

    hoSoCTModel.createdById = this.auth.UserId;
    hoSoCTModel.createdDate = convertToUTCTime(new Date());
    hoSoCTModel.updatedById = null;
    hoSoCTModel.updatedDate = null;
    return hoSoCTModel;
  }

  cancel() {
    this.router.navigate(['/employee/danh-sach-ho-so-cong-tac']);
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  chiTietDeNghi(rowData: any) {
    let url = this.router.serializeUrl(this.router.createUrlTree(['/employee/chi-tiet-de-nghi-tam-hoan-ung', { deNghiTamHoanUngId: this.encrDecrService.set(rowData.deNghiTamHoanUngId), hoSoCongTacId: this.encrDecrService.set(this.hoSoCongTacId) }]));
    window.open(url, '_blank');
  }

  //#region  GHI CHÚ

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

  /*Event Khi click xóa toàn bộ file*/
  clearAllFile() {
    this.uploadedFiles = [];
  }

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: any) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa tài liệu?',
      accept: () => {
        this.quanLyCongtacService.deleteFile(file.fileInFolderId).subscribe(res => {
          let result: any = res;
          if (result.statusCode == 200) {
            let index = this.arrayDocumentModel.indexOf(file);
            this.arrayDocumentModel.splice(index, 1);
            this.showToast('success', 'Thông báo', 'Xóa file thành công');
          } else {
            this.showToast('success', 'Thông báo', result.message);
          }
        })
      }
    });
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
      fileUpload.FileInFolder.objectNumber = this.hoSoCongTacId;
      fileUpload.FileInFolder.objectType = 'HOSOCT';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    this.quanLyCongtacService.uploadFile("HOSOCT", listFileUploadModel, this.hoSoCongTacId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.uploadedFiles = [];

        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }

        this.arrayDocumentModel = result.listFileInFolder;
        this.ref.detectChanges();
        this.showToast('success', 'Thông báo', "Thêm file thành công");
      } else {
        this.showToast('error', 'Thông báo', result.message);
      }
    });
  }

  //#endregion
  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function ParseStringToFloat(str: string) {
  if (str === "" || str == null) return 0;
  str = str.toString().replace(/,/g, '');
  return parseFloat(str);
}
