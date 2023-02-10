import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService, TreeNode } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { AssetService } from '../../services/asset.service';
import { Paginator, Table } from 'primeng';
import { DatePipe } from '@angular/common';
import { YeuCauCapPhatModel } from '../../models/yeu-cau-cap-phat';
import { YeuCauCapPhatChiTietModel } from '../../models/yeu-cau-cap-phat-chi-tiet';
import { EmployeeService } from '../../../../app/employee/services/employee.service';
import { NoteService } from '../../../../app/shared/services/note.service';
import { QuyTrinhService } from '../../../../app/admin/services/quy-trinh.service';
import * as XLSX from 'xlsx';
import { YeuCauCapPhatImportDetailComponent } from '../yeu-cau-cap-phat-import-detail/yeu-cau-cap-phat-import-detail.component';
import { TaiSanModel } from '../../models/taisan.model';
import { EncrDecrService } from '../../../shared/services/encrDecr.service';
import { async } from '@angular/core/testing';

class CategoryModel {
  categoryId: string;
  categoryCode: string;
  categoryName: string;

  constructor() {
    this.categoryId = '00000000-0000-0000-0000-000000000000';
    this.categoryCode = '';
    this.categoryName = '';
  }
}

class EmployeeModel {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  employeeLastname: string;
  employeeFirstname: string;
  startedDate: Date;
  organizationId: string;
  positionId: string;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  active: Boolean;
  username: string;
  identity: string;
  organizationName: string;
  avatarUrl: string;
  positionName: string;
  contactId: string;
  isManager: boolean;
  permissionSetId: string;
  probationEndDate: Date;
  probationStartDate: Date;
  trainingStartDate: Date;
  contractType: string;
  contractEndDate: Date;
  isTakeCare: boolean;
  isXuLyPhanHoi: boolean;
  organizationLevel: number;
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

class importAssetByExcelModel {
  // Loại tài sản
  // Số lượng
  // Mô tả
  // mã Nhân viên
  // Tên nhân viên
  // Mã mục đích sử dụng
  // Sử dụng từ ngày
  // Sử dụng đến ngày
  // Lý do
  maLoaiTaiSan: String;
  soLuong: String;
  moTa: String;
  employeeCode: String;
  tenNhanVien: String;
  maMucDichSuDung: String;
  mucDichSuDung: String;
  ngayBatDau: Date;
  ngayKetThuc: Date;
  lyDo: String;
}

@Component({
  selector: 'app-yeu-cau-cap-phat-detail',
  templateUrl: './yeu-cau-cap-phat-detail.component.html',
  styleUrls: ['./yeu-cau-cap-phat-detail.component.css']
})
export class YeuCauCapPhatDetailComponent implements OnInit {
  /*Điều kiện hiển thị các button*/
  isShowGuiPheDuyet: boolean = false;
  isShowPheDuyet: boolean = false;
  isShowTuChoi: boolean = false;
  isShowHuyYeuCauPheDuyet: boolean = false;
  isShowLuu: boolean = false;
  isShowXoa: boolean = false;
  isShowHuy: boolean = false;
  isShowPhanBo: boolean = false;
  isShowDatVeMoi: boolean = false;
  isShowHoanThanh: boolean = false;
  trangThai: number = 1;
  /*End*/

  today: Date = new Date();
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  @ViewChild('paginator', { static: true }) paginator: Paginator
  @ViewChild('myTable') myTable: Table;
  filterGlobal: string = '';

  /* End */
  /*Khai báo biến*/
  auth: any = JSON.parse(localStorage.getItem("auth"));
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;
  /* Action*/
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionCapPhat: boolean = true;
  /*END*/
  // Label trên form
  nguoiDeXuat: string = '';
  maYeuCau: string = '';
  /* Form */
  yeuCauCapPhatForm: FormGroup;

  loaiTaiSanControl: FormControl;
  moTaControl: FormControl;
  nhanVienControl: FormControl;
  mucDichSuDungControl: FormControl;
  ngayBatDauControl: FormControl;
  ngayKetThucControl: FormControl;
  lyDoControl: FormControl;
  soLuongControl: FormControl;

  // Thông tin chung
  ngayDeXuatControl: FormControl;
  // Dữ liệu masterdata
  listLoaiTS: Array<CategoryModel> = new Array<CategoryModel>();
  listMucDichSuDung: Array<CategoryModel> = new Array<CategoryModel>();
  listEmployee: Array<EmployeeModel> = new Array<EmployeeModel>();

  // notification
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;

  // Tài liệu liên quan
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];
  colsFile: any[];
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  employeeModel: EmployeeModel = new EmployeeModel();
  isUpdate: boolean = false;

  selectedColumns: any[];
  cols: any[];

  listTaiSanYeuCauTemp: Array<YeuCauCapPhatChiTietModel> = new Array<YeuCauCapPhatChiTietModel>();
  yeuCauCapPhatTaiSanId: number = 0;
  yeuCauCapPhat: YeuCauCapPhatModel = new YeuCauCapPhatModel();

  trangThaiYC: string = 'Mới';
  yeuCauCapPhatTaiSanChiTietId: number = 0;
  disableForm: boolean = false;
  statusCode: string = null;

  ngayDeXuat = new Date();
  awaitResponseApprove: boolean = false;
  displayReject: boolean = false;
  descriptionReject: string = '';

  //#region   QUY TRÌNH PHÊ DUYỆT
  listFormatStatusSupport: Array<any> = []; // Thanh trạng thái
  showLyDoTuChoi: boolean = false;
  tuChoiForm: FormGroup;
  lyDoTuChoiControl: FormControl;
  //#endregion

  // Excel
  displayChooseFileImportDialog: boolean = false;
  fileName: string = '';
  importFileExcel: any = null;

  // Phân bổ tài sản
  taoPhanBoForm: FormGroup;
  taiSanPhanBoControl: FormControl;
  ngayBatDauPhanBoControl: FormControl;
  ngayKetThucPhanBoControl: FormControl;
  yeuCauCapPhatChiTietModel: YeuCauCapPhatChiTietModel = new YeuCauCapPhatChiTietModel();
  listTaiSanChuaPhanBo: Array<TaiSanModel> = new Array<TaiSanModel>();
  listTaiSanChuaPhanBoDefault: Array<TaiSanModel> = new Array<TaiSanModel>();

  listDonVi: Array<CategoryModel> = new Array<CategoryModel>();

  qrCode: any = "Loft3Di";
  displayCreateAsset: boolean = false;
  taoNhanhTSForm: FormGroup;
  maTaiSanControl: FormControl;
  tenTaiSanControl: FormControl;
  donViControl: FormControl;

  colsItem: any;
  dataTree: Array<TreeNode> = [];
  selectedColumnsPB: any[];
  isAddAsset: boolean = true;
  displayQRCode: boolean = false;

  refreshNote: number = 0;

  constructor(
    private assetService: AssetService,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private getPermission: GetPermission,
    private el: ElementRef,
    private ref: ChangeDetectorRef,
    private employeeService: EmployeeService,
    private noteService: NoteService,
    private quyTrinhService: QuyTrinhService,
    private encrDecrService: EncrDecrService,
  ) {

  }

  async ngOnInit() {
    this.setTable();
    this.setForm();
    let resource = "ass/asset/chi-tiet-yeu-cau-cap-phat";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
      this.showToast('warn', 'Thông báo', 'Bạn không có quyền truy cập');
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("cap-phat") == -1) {
        this.actionCapPhat = false;
      }
      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }

      this.route.params.subscribe(params => {
        this.yeuCauCapPhatTaiSanId = Number(this.encrDecrService.get(params['requestId']));
      });


      this.getMasterData();
    }
  }

  setForm() {
    this.ngayDeXuatControl = new FormControl(null, [Validators.required]);
    this.loaiTaiSanControl = new FormControl(null, [Validators.required]);
    this.nhanVienControl = new FormControl(null, [Validators.required]);
    this.mucDichSuDungControl = new FormControl(null, [Validators.required]);
    this.ngayBatDauControl = new FormControl(null, [Validators.required]);
    this.ngayKetThucControl = new FormControl(null);
    this.lyDoControl = new FormControl('');
    this.moTaControl = new FormControl('');
    this.soLuongControl = new FormControl(null, [Validators.required, Validators.min(0.00000000001)]);

    this.yeuCauCapPhatForm = new FormGroup({
      loaiTaiSanControl: this.loaiTaiSanControl,
      nhanVienControl: this.nhanVienControl,
      mucDichSuDungControl: this.mucDichSuDungControl,
      ngayBatDauControl: this.ngayBatDauControl,
      ngayKetThucControl: this.ngayKetThucControl,
      lyDoControl: this.lyDoControl,
      moTaControl: this.moTaControl,
      soLuongControl: this.soLuongControl
    });

    this.lyDoTuChoiControl = new FormControl(null, [Validators.required]);
    this.tuChoiForm = new FormGroup({
      lyDoTuChoiControl: this.lyDoTuChoiControl,
    });

    // Phân bổ tài sản
    this.taiSanPhanBoControl = new FormControl(null, [Validators.required]);
    this.ngayBatDauPhanBoControl = new FormControl(null, [Validators.required]);
    this.ngayKetThucPhanBoControl = new FormControl(null);

    this.taoPhanBoForm = new FormGroup({
      taiSanPhanBoControl: this.taiSanPhanBoControl,
      ngayBatDauPhanBoControl: this.ngayBatDauPhanBoControl,
      ngayKetThucPhanBoControl: this.ngayKetThucPhanBoControl,
    });

    // Tạo nhanh tài sản
    this.maTaiSanControl = new FormControl(null, [Validators.required]);
    this.tenTaiSanControl = new FormControl(null, [Validators.required]);
    this.donViControl = new FormControl(null, [Validators.required]);
    this.taoNhanhTSForm = new FormGroup({
      maTaiSanControl: this.maTaiSanControl,
      tenTaiSanControl: this.tenTaiSanControl,
      donViControl: this.donViControl
    });
  }

  async getMasterData() {
    this.loading = true;
    let [result, resultDetail]: any = await Promise.all([
      this.assetService.getMasterDataYeuCauCapPhatForm(),
      this.assetService.getDataYeuCauCapPhatDetail(this.yeuCauCapPhatTaiSanId, this.auth.UserId)
    ]);

    await this.getDuLieuQuyTrinh();

    if (result.statusCode === 200 && resultDetail.statusCode == 200) {
      this.loading = false;

      this.listLoaiTS = result.listLoaiTSPB;
      this.listMucDichSuDung = result.listMucDichSuDung;
      this.listEmployee = result.listEmployee;
      this.listDonVi = result.listDonVi;
      this.listTaiSanChuaPhanBoDefault = result.listTaiSanChuaPhanBo;
      this.listTaiSanChuaPhanBo = result.listTaiSanChuaPhanBo;
      this.trangThaiYC = resultDetail.yeuCauCapPhat.trangThaiString
      this.mapDataFromModelToForm(resultDetail);

      this.arrayDocumentModel = resultDetail.listFileInFolder;
      this.ref.detectChanges();
    }
    else
      this.loading = false;
  }

  mapDataFromModelToForm(resultDetail: any) {

    this.isShowGuiPheDuyet = resultDetail.isShowGuiPheDuyet;
    this.isShowPheDuyet = resultDetail.isShowPheDuyet;
    this.isShowTuChoi = resultDetail.isShowTuChoi;
    this.isShowHuyYeuCauPheDuyet = resultDetail.isShowHuyYeuCauPheDuyet;
    this.isShowLuu = resultDetail.isShowLuu;
    this.isShowXoa = resultDetail.isShowXoa;
    this.isShowHuy = resultDetail.isShowHuy;
    this.isShowPhanBo = resultDetail.isShowPhanBo;
    this.isShowDatVeMoi = resultDetail.isShowDatVeMoi;
    this.trangThai = resultDetail.yeuCauCapPhat.trangThai;
    this.isShowHoanThanh = resultDetail.isShowHoanThanh;

    if (this.trangThai == 1)
      this.selectedColumns = this.cols.filter(e => e.field == "loaiTaiSan" || e.field == "soLuong" || e.field == "moTa"
        || e.field == "tenNhanVien" || e.field == "phongBan" || e.field == "viTriLamViec"
        || e.field == "mucDichSuDung" || e.field == "ngayBatDau" || e.field == "ngayKetThuc" || e.field == "lyDo"
      );
    else {
      this.selectedColumns = this.cols.filter(e => e.field == "loaiTaiSan" || e.field == "soLuong" || e.field == "moTa"
        || e.field == "tenNhanVien" || e.field == "phongBan" || e.field == "viTriLamViec"
        || e.field == "mucDichSuDung" || e.field == "soLuongPheDuyet" || e.field == "ngayBatDau" || e.field == "ngayKetThuc" || e.field == "lyDo"
      );
    }
    // Người đề xuất
    this.yeuCauCapPhat.nguoiDeXuat = resultDetail.yeuCauCapPhat.nguoiDeXuat;
    this.yeuCauCapPhat.nguoiDeXuatId = resultDetail.yeuCauCapPhat.nguoiDeXuatId;

    // Mã yêu cầu
    this.yeuCauCapPhat.maYeuCau = resultDetail.yeuCauCapPhat.maYeuCau;

    // Ngày đề xuất
    let ngayDeXuat = resultDetail.yeuCauCapPhat.ngayDeXuat ? new Date(resultDetail.yeuCauCapPhat.ngayDeXuat) : null;
    this.yeuCauCapPhat.ngayDeXuat = ngayDeXuat;

    // Yêu cầu cấp phát
    this.yeuCauCapPhat.yeuCauCapPhatTaiSanId = resultDetail.yeuCauCapPhat.yeuCauCapPhatTaiSanId;

    // Danh sách tài sản yêu cầu
    this.listTaiSanYeuCauTemp = resultDetail.yeuCauCapPhat.listYeuCauCapPhatTaiSanChiTiet;

    if (this.isShowPhanBo || this.isShowHoanThanh) {
      this.rebuildYeuCauCPTree();
    }
    if (this.isShowHoanThanh) {
      this.taoPhanBoForm.disable();
    }

    if (this.trangThai != 1)
      this.yeuCauCapPhatForm.disable();
    this.ref.detectChanges();
  }

  setTable() {
    this.colsFile = [
      { field: 'fileFullName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', color: '#f44336' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'right', color: '#f44336' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'center', color: '#f44336' },
      { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'left', color: '#f44336' },
    ];

    this.cols = [
      { field: 'loaiTaiSan', header: 'Loại tài sản', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'soLuong', header: 'SL', width: '50px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'soLuongPheDuyet', header: 'SL phê duyệt', width: '70px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'moTa', header: 'Mô tả', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'tenNhanVien', header: 'NV yêu cầu', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'phongBan', header: 'Phòng ban', width: '100px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'viTriLamViec', header: 'Vị trí làm việc', width: '100px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'mucDichSuDung', header: 'Mục đích sử dụng', width: '100px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'ngayBatDau', header: 'Từ ngày', width: '100px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'ngayKetThuc', header: 'Đến ngày', width: '100px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'lyDo', header: 'Lý do', width: '100px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
    ];

    this.selectedColumns = this.cols;

    this.colsItem = [
      // { field: '#', header: '#', width: '30px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'loaiTaiSan', header: 'Loại tài sản', width: '100px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'maTaiSan', header: 'Mã tài sản', width: '120px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'soLuong', header: 'SL yêu cầu', width: '50px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'soLuongPheDuyet', header: 'SL phê duyệt', width: '70px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'moTa', header: 'Mô tả', width: '120px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'tenNhanVien', header: 'NV yêu cầu', width: '140px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'phongBan', header: 'Phòng ban', width: '100px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'viTriLamViec', header: 'Vị trí làm việc', width: '100px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'mucDichSuDung', header: 'Mục đích sử dụng', width: '100px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'ngayBatDau', header: 'Từ ngày', width: '100px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'ngayKetThuc', header: 'Đến ngày', width: '100px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'lyDo', header: 'Lý do', width: '60px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
    ];
    this.selectedColumnsPB = this.colsItem;
  }

  //Function reset toàn bộ các value đã nhập trên form
  resetFieldValue() {
    this.listTaiSanYeuCauTemp = [];
    this.yeuCauCapPhatForm.reset();
    this.clearAllFile();
    this.arrayDocumentModel = [];
    this.ngayDeXuat = new Date();
  }

  mapFormDataToModel(): Array<YeuCauCapPhatChiTietModel> {
    let lstPhanBo = new Array<YeuCauCapPhatChiTietModel>();
    this.listTaiSanYeuCauTemp.forEach(taisan => {
      lstPhanBo.push(taisan)
    });

    return lstPhanBo;
  }

  mapFormToYCCapPhatModelTemp(isUpdate: boolean = false): YeuCauCapPhatChiTietModel {

    let yeuCauChiTietModel = new YeuCauCapPhatChiTietModel();
    if (!isUpdate) {
      const maxValueOfY = Math.max(...this.listTaiSanYeuCauTemp.map(o => o.yeuCauCapPhatTaiSanId), 0);
      yeuCauChiTietModel.yeuCauCapPhatTaiSanId = maxValueOfY + 1;
    }
    // loại Tài sản
    let loaiTS = this.yeuCauCapPhatForm.get('loaiTaiSanControl').value;
    if (loaiTS != null && loaiTS != undefined) {
      yeuCauChiTietModel.loaiTaiSanId = loaiTS ? loaiTS.categoryId : 0;
      yeuCauChiTietModel.loaiTaiSan = loaiTS.categoryName;
    }

    // Nhân viên
    let nhanVien = this.yeuCauCapPhatForm.get('nhanVienControl').value
    if (nhanVien != null && nhanVien != undefined) {
      yeuCauChiTietModel.nhanVienYeuCauId = nhanVien ? nhanVien.employeeId : this.emptyGuid;
      yeuCauChiTietModel.tenNhanVien = nhanVien.employeeName;
      yeuCauChiTietModel.phongBan = nhanVien.organizationName;
      yeuCauChiTietModel.viTriLamViec = nhanVien.positionName;
    }

    // Mục đích sử dụng
    let mucDichSD = this.yeuCauCapPhatForm.get('mucDichSuDungControl').value

    if (mucDichSD != null && mucDichSD != undefined) {
      yeuCauChiTietModel.mucDichSuDungId = mucDichSD ? mucDichSD.categoryId : this.emptyGuid;
      yeuCauChiTietModel.mucDichSuDung = mucDichSD.categoryName;
    }

    var datePipe = new DatePipe("en-US");
    // Từ ngày
    let ngayBatDau = this.yeuCauCapPhatForm.get('ngayBatDauControl').value ? convertToUTCTime(this.yeuCauCapPhatForm.get('ngayBatDauControl').value) : null;
    yeuCauChiTietModel.ngayBatDau = ngayBatDau;
    if (this.yeuCauCapPhatForm.get('ngayBatDauControl').value != null) {

      yeuCauChiTietModel.ngayBatDauString = datePipe.transform(this.yeuCauCapPhatForm.get('ngayBatDauControl').value, 'dd/MM/yyyy');
    }

    // Đến ngày
    let ngayKetThuc = this.yeuCauCapPhatForm.get('ngayKetThucControl').value ? convertToUTCTime(this.yeuCauCapPhatForm.get('ngayKetThucControl').value) : null;
    yeuCauChiTietModel.ngayKetThuc = ngayKetThuc;
    if (this.yeuCauCapPhatForm.get('ngayKetThucControl').value != null) {

      yeuCauChiTietModel.ngayKetThucString = datePipe.transform(this.yeuCauCapPhatForm.get('ngayKetThucControl').value, 'dd/MM/yyyy');
    }

    // So luong
    yeuCauChiTietModel.soLuong = this.soLuongControl.value;

    // Mô tả
    yeuCauChiTietModel.moTa = this.moTaControl.value == null ? "" : this.moTaControl.value;

    // Lý do
    yeuCauChiTietModel.lyDo = this.lyDoControl.value == null ? "" : this.lyDoControl.value;

    yeuCauChiTietModel.createdById = this.auth.UserId;
    yeuCauChiTietModel.createdDate = convertToUTCTime(new Date());
    yeuCauChiTietModel.updatedById = null;
    yeuCauChiTietModel.updatedDate = null;
    return yeuCauChiTietModel;
  }

  cancel() {
    this.router.navigate(['/asset/danh-sach-yeu-cau-cap-phat']);
  }

  async createYeuCauTemp() {

    if (!this.yeuCauCapPhatForm.valid) {
      Object.keys(this.yeuCauCapPhatForm.controls).forEach(key => {
        if (!this.yeuCauCapPhatForm.controls[key].valid) {
          this.yeuCauCapPhatForm.controls[key].markAsTouched();
        }
      });

      this.emitStatusChangeForm = this.yeuCauCapPhatForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });

      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    }
    else {
      this.loading = true;
      let yeuCauCPModel = this.mapFormToYCCapPhatModelTemp();
      yeuCauCPModel.yeuCauCapPhatTaiSanChiTietId = 0;
      yeuCauCPModel.yeuCauCapPhatTaiSanId = this.yeuCauCapPhatTaiSanId;
      let result: any = await this.assetService.createOrUpdateChiTietYeuCauCapPhat(yeuCauCPModel, this.auth.UserId);
      this.loading = false;
      if (result.statusCode === 200) {
        this.refreshForm();
        this.listTaiSanYeuCauTemp = result.listTaiSanYeuCau;

      }
      else
        this.showToast('error', 'Thông báo', result.message);
    }
  }

  async updateYeuCauTemp() {
    if (!this.yeuCauCapPhatForm.valid) {
      Object.keys(this.yeuCauCapPhatForm.controls).forEach(key => {
        if (!this.yeuCauCapPhatForm.controls[key].valid) {
          this.yeuCauCapPhatForm.controls[key].markAsTouched();
        }
      });

      this.emitStatusChangeForm = this.yeuCauCapPhatForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });

      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    }
    else {
      this.loading = true;
      let yeuCauCPModel = this.mapFormToYCCapPhatModelTemp();
      yeuCauCPModel.yeuCauCapPhatTaiSanChiTietId = this.yeuCauCapPhatTaiSanChiTietId;
      yeuCauCPModel.yeuCauCapPhatTaiSanId = this.yeuCauCapPhatTaiSanId;
      let result: any = await this.assetService.createOrUpdateChiTietYeuCauCapPhat(yeuCauCPModel, this.auth.UserId);
      this.loading = false;
      if (result.statusCode === 200) {
        this.refreshForm();
        this.listTaiSanYeuCauTemp = result.listTaiSanYeuCau;
      }
      else
        this.showToast('error', 'Thông báo', result.message);
    }
  }

  delAssetTemp(rowData: any) {
    if (!rowData.isNewLine) {
      this.confirmationService.confirm({
        message: `Bạn có chắc chắn xóa dòng này?`,
        accept: async () => {
          this.loading = true;
          let result: any = await this.assetService.deleteChiTietYeuCauCapPhat(rowData.yeuCauCapPhatTaiSanChiTietId);
          this.loading = false;
          if (result.statusCode === 200) {
            this.listTaiSanYeuCauTemp = this.listTaiSanYeuCauTemp.filter(x => x != rowData);
            this.refreshForm();
            this.showToast('success', 'Thông báo', 'Xóa dữ liệu thành công');
          }
          else
            this.showToast('error', 'Thông báo', result.message);
        }
      });
    }
    else {
      this.listTaiSanYeuCauTemp = this.listTaiSanYeuCauTemp.filter(e => e != rowData);
    }
  }

  /*Hiển thị lại thông tin tài sản*/
  reShowAsset(rowData: any) {
    this.isUpdate = true;

    this.yeuCauCapPhatTaiSanChiTietId = rowData.yeuCauCapPhatTaiSanChiTietId;

    // Loại tài sản phân bổ
    let phanLoai = this.listLoaiTS.find(c => c.categoryId == rowData.loaiTaiSanId);
    this.yeuCauCapPhatForm.controls['loaiTaiSanControl'].setValue(phanLoai);

    // Số lượng
    this.yeuCauCapPhatForm.controls['soLuongControl'].setValue(rowData.soLuong);

    // Mô tả
    this.yeuCauCapPhatForm.controls['moTaControl'].setValue(rowData.moTa);

    // Mã nhân viên
    let nhanVien = this.listEmployee.find(c => c.employeeId == rowData.nhanVienYeuCauId);
    this.yeuCauCapPhatForm.controls['nhanVienControl'].setValue(nhanVien);

    this.employeeModel = nhanVien;

    // Mục đích sử dụng
    let mucDich = this.listMucDichSuDung.find(c => c.categoryId == rowData.mucDichSuDungId);
    this.yeuCauCapPhatForm.controls['mucDichSuDungControl'].setValue(mucDich);

    // Sử dụng từ ngày
    this.yeuCauCapPhatForm.controls['ngayBatDauControl'].setValue(new Date(rowData.ngayBatDau));

    if (rowData.ngayKetThuc) {
      this.yeuCauCapPhatForm.controls['ngayKetThucControl'].setValue(new Date(rowData.ngayKetThuc));
    }

    this.yeuCauCapPhatForm.controls['lyDoControl'].setValue(rowData.lyDo);

    this.yeuCauCapPhatForm.controls['loaiTaiSanControl'].updateValueAndValidity();
    this.yeuCauCapPhatForm.controls['mucDichSuDungControl'].updateValueAndValidity();
    this.yeuCauCapPhatForm.controls['nhanVienControl'].updateValueAndValidity();
    this.yeuCauCapPhatForm.controls['mucDichSuDungControl'].updateValueAndValidity();
  }

  thayDoiNhanVien(event: any) {
    let nhanVien = this.listEmployee.find(x => x.employeeId == event.value.employeeId);
    this.employeeModel = nhanVien;
  }

  refreshForm() {
    this.yeuCauCapPhatForm.reset();
    this.soLuongControl.setValue(0);
    this.employeeModel = new EmployeeModel();
    this.isUpdate = false;
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
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
        this.employeeService.deleteFile(file.fileInFolderId).subscribe(res => {
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
      fileUpload.FileInFolder.objectNumber = this.yeuCauCapPhatTaiSanId;
      fileUpload.FileInFolder.objectType = 'YCCAPPHAT';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });


    this.assetService.uploadFile("YCCAPPHAT", listFileUploadModel, this.yeuCauCapPhatTaiSanId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.uploadedFiles = [];

        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }

        this.arrayDocumentModel = result.listFileInFolder;

        this.showToast('success', 'Thông báo', "Thêm file thành công");
      } else {
        this.showToast('error', 'Thông báo', result.message);
      }
    });
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  // tạo yêu cầu
  async taoYeuCauCapPhat() {
    if (this.listTaiSanYeuCauTemp.length > 0) {
      {
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
          fileUpload.FileInFolder.objectType = 'YCCAPPHAT';
          fileUpload.FileSave = item;
          listFileUploadModel.push(fileUpload);
        });

        this.loading = true;
        this.awaitResult = true;

        this.listTaiSanYeuCauTemp.forEach(chitiet => {
          chitiet.ngayBatDau = new Date(chitiet.ngayBatDau);
          if (chitiet.ngayKetThuc != null)
            chitiet.ngayKetThuc = new Date(chitiet.ngayKetThuc);
          chitiet.createdDate = new Date(chitiet.createdDate);
        });

        this.assetService.createOrYeuCauCapPhat(this.yeuCauCapPhat, this.listTaiSanYeuCauTemp, 'YCCAPPHAT', listFileUploadModel, this.auth.UserId).subscribe(async response => {

          this.loading = false;
          let result = <any>response;

          if (result.statusCode == 200) {
            this.showToast('success', 'Thông báo', "Cập nhập yêu cầu cấp phát thành công.");
            this.resetFieldValue();
            await this.getMasterData();
            this.awaitResult = false;

            return true;
          }
          else {
            this.awaitResult = false;
            this.clearToast();
            this.showToast('error', 'Thông báo', "Đã xả ra lỗi trong quá trình lưu thông tin.");
            return false;
          }
        });
      }
    }
    else {
      this.showToast('error', 'Thông báo', 'Danh sách tài sản yêu cầu không được trống.');
      return false;
    }
  }

  //#region QUY TRÌNH PHÊ DUYỆT

  async getDuLieuQuyTrinh() {
    this.quyTrinhService.getDuLieuQuyTrinh(this.emptyGuid, 20, this.yeuCauCapPhatTaiSanId).subscribe(res => {
      let result: any = res;

      if (result.statusCode == 200) {

        this.listFormatStatusSupport = result.listDuLieuQuyTrinh;
      }
      else {
        this.showToast('error', 'Thông báo:', result.message);
      }
    });
  }

  // Chuyển yêu cầu về trạng thái mới sau khi bị từ chối phê duyệt
  async datVeMoi() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn sử dụng lại Yêu cầu cấp phát này?',
      accept: async () => {

        let res: any = await this.assetService.datVeMoiYeuCauCapPhatTS(this.yeuCauCapPhatTaiSanId);
        if (res.statusCode === 200) {
          this.showToast('success', 'Thông báo:', "Đặt về mới yêu cầu cấp phát tài sản thành công.");
          this.getMasterData();
        } else {
          this.showToast('success', 'Thông báo:', res.message);
        }
      }
    });
  }

  // Gửi phê duyệt
  async guiPheDuyet() {

    this.loading = true;

    this.quyTrinhService.guiPheDuyet(this.emptyGuid, 20, this.yeuCauCapPhatTaiSanId).subscribe(async res => {
      let result: any = res;

      if (result.statusCode == 200) {
        this.showToast('success', 'Thông báo:', "Gửi phê duyệt thành công.");
        await this.getMasterData();
        this.ref.detectChanges();
      }
      else {
        this.loading = false;
        this.showToast('error', 'Thông báo:', result.message);
      }
    });
  }

  // Phê duyệt
  async pheDuyet() {
    var isError = false;
    let tongSLPD = 0;

    let currentBuoc = this.listFormatStatusSupport.findIndex(x => x.isCurrent);
    if (this.listFormatStatusSupport.length - 1 == currentBuoc) {
      for (let i = 0; i < this.listTaiSanYeuCauTemp.length; i++) {
        let soLuongPD = this.listTaiSanYeuCauTemp[i].soLuongPheDuyet;
        let soLuong = this.listTaiSanYeuCauTemp[i].soLuong;

        tongSLPD += soLuongPD;
        if (soLuongPD > soLuong || soLuongPD <= 0 || soLuongPD == null) {
          isError = true;
        }
      }
      if (tongSLPD == 0) {
        this.showToast('error', 'Thông báo:', 'Tổng số lượng tài sản phê duyệt phải lớn hơn 0.');
        return;
      }

      if (isError == true) {
        this.showToast('error', 'Thông báo:', 'Số lượng phê duyệt lớn hơn số lượng yêu cầu.');
        return;
      }
    }

    this.confirmationService.confirm({
      message: 'Bạn chắc chắc muốn phê duyệt yêu cầu cấp phát này?',
      accept: async () => {
        await this.taoYeuCauCapPhat();
        this.loading = true;

        this.quyTrinhService.pheDuyet(this.emptyGuid, 20, '', this.yeuCauCapPhatTaiSanId).subscribe(async res => {
          let result: any = res;

          if (result.statusCode == 200) {
            this.showToast('success', 'Thông báo:', "Phê duyệt yêu cầu thành công.");
            await this.getMasterData();
            this.ref.detectChanges();
          }
          else {
            this.loading = false;
            this.showToast('error', 'Thông báo:', result.message);
          }
        });
      }
    });
  }

  openDialogReject() {
    this.showLyDoTuChoi = true;
    this.tuChoiForm.reset();
  }

  // Từ chối phê duyệt
  async tuChoi() {

    if (!this.tuChoiForm.valid) {
      Object.keys(this.tuChoiForm.controls).forEach(key => {
        if (!this.tuChoiForm.controls[key].valid) {
          this.tuChoiForm.controls[key].markAsTouched();
        }
      });

      this.emitStatusChangeForm = this.tuChoiForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });

      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    }
    else {
      this.loading = true;
      this.quyTrinhService.tuChoi(this.emptyGuid, 20, this.lyDoTuChoiControl.value, this.yeuCauCapPhatTaiSanId).subscribe(res => {
        let result: any = res;

        if (result.statusCode == 200) {
          this.showToast('success', 'Thông báo:', "Từ chối yêu cầu thành công.");

          this.getMasterData();
        }
        else {
          this.loading = false;
          this.showToast('error', 'Thông báo:', result.message);
        }
      });
      this.showLyDoTuChoi = false;
    }
  }

  huyYeuCacXacNhan() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn hủy yêu cầu xác nhận yêu cầu này không?',
      accept: async () => {
        // await this.createOrUpdate(false);

        this.loading = true;
        this.quyTrinhService.huyYeuCauPheDuyet(this.emptyGuid, 20, this.yeuCauCapPhatTaiSanId).subscribe(res => {
          let result: any = res;

          if (result.statusCode != 200) {
            this.loading = false;
            this.showToast('success', 'Thông báo:', "Hủy xác nhận yêu cầu thành công.");
            return;
          }

          this.showToast('error', 'Thông báo:', result.message);
          this.getMasterData();
        });
      }
    });
  }

  kiemTraSoLuong(rowdata: any) {
    let soLuongPD = 0;
    soLuongPD = rowdata.soLuongPheDuyet ? parseFloat(rowdata.soLuongPheDuyet.replace(/,/g, '')) : null;

    if (soLuongPD > rowdata.soLuong || soLuongPD <= 0 || soLuongPD == null) {
      rowdata.error = true;
      this.showToast('error', 'Thông báo:', 'Số lượng thực xuất cần lớn hơn 0 và nhỏ hơn hoặc bằng số lượng cần xuất');
      return;
    }
    else {
      rowdata.error = false;
    }
  }

  closeDialog() {
    this.showLyDoTuChoi = false;
  }

  //#endregion


  //#region FUNCTION EXCEL
  importAsset() {
    this.displayChooseFileImportDialog = true;
  }

  importExcel() {
    if (this.fileName == "") {
      this.showToast('error', 'Thông báo:', "Chưa chọn file cần nhập");
      return;
    }
    this.loading = true
    const targetFiles: DataTransfer = <DataTransfer>(this.importFileExcel);
    const reader: FileReader = new FileReader();
    reader.readAsBinaryString(targetFiles.files[0]);

    reader.onload = (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary', cellDates: true });
      let code = 'YeuCauTaiSan';
      if (!workbook.Sheets[code]) {
        this.showToast('error', 'Thông báo:', "File không hợp lệ");
        return;
      }

      //lấy data từ file excel
      const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[code];
      /* save data */
      let listAssetRawData: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
      /* remove header */
      listAssetRawData = listAssetRawData.filter((e, index) => index != 0);
      /* nếu không nhập  trường required thì loại bỏ */
      listAssetRawData = listAssetRawData.filter(e => (e[0] && e[1]));

      /* chuyển từ raw data sang model */
      let listAssetImport: Array<importAssetByExcelModel> = [];
      listAssetRawData ?.forEach(_rawData => {
        /*
        Loại tài sản
        Số lượng
        Mô tả
        mã Nhân viên
        Tên nhân viên
        Mã mục đích sử dụng
        Sử dụng từ ngày
        Sử dụng đến ngày
        Lý do
       */
        let asset = new importAssetByExcelModel();
        asset.maLoaiTaiSan = _rawData[0] ? _rawData[0].toString().trim() : '';
        asset.soLuong = _rawData[1] ? _rawData[1].toString().trim() : '';
        asset.moTa = _rawData[2] ? _rawData[2].toString().trim() : '';
        asset.employeeCode = _rawData[3] ? _rawData[3].toString().trim() : '';
        asset.maMucDichSuDung = _rawData[4] ? _rawData[4].toString().trim() : '';

        let ngayBD = new Date(_rawData[5].replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
        asset.ngayBatDau = convertToUTCTime(ngayBD);

        if (_rawData[6] != undefined) {
          let ngayKT = new Date(_rawData[6].replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
          asset.ngayKetThuc = convertToUTCTime(ngayKT);
        }

        asset.lyDo = _rawData[7] ? _rawData[7].toString().trim() : '';
        listAssetImport = [...listAssetImport, asset];
      });
      /* tắt dialog import file, bật dialog chi tiết tài sản import */
      this.displayChooseFileImportDialog = false;
      this.loading = false
      this.openDetailImportDialog(listAssetImport);
    }
  }

  openDetailImportDialog(listAssetImport) {
    let ref = this.dialogService.open(YeuCauCapPhatImportDetailComponent, {
      data: {
        listAssetImport: listAssetImport
      },
      header: 'Nhập excel danh sách yêu cầu tài sản',
      width: '85%',
      baseZIndex: 1050,
      contentStyle: {
        "max-height": "800px",
      }
    });
    ref.onClose.subscribe((result: any) => {
      if (result ?.status) {
        result.listAsset.forEach(item => {

          let taiSanYCTemp = new YeuCauCapPhatChiTietModel();

          // loại Tài sản
          let loaiTS = this.listLoaiTS.find(x => x.categoryCode == item.maLoaiTaiSan);
          if (loaiTS != null && loaiTS != undefined) {
            taiSanYCTemp.loaiTaiSanId = loaiTS ? loaiTS.categoryId : '';
            taiSanYCTemp.loaiTaiSan = loaiTS.categoryName;
          }

          // Nhân viên
          let nhanVien = this.listEmployee.find(x => x.employeeCode == item.employeeCode)
          if (nhanVien != null && nhanVien != undefined) {
            taiSanYCTemp.nhanVienYeuCauId = nhanVien ? nhanVien.employeeId : this.emptyGuid;
            taiSanYCTemp.tenNhanVien = nhanVien.employeeName;
            taiSanYCTemp.phongBan = nhanVien.organizationName;
            taiSanYCTemp.viTriLamViec = nhanVien.positionName;
          }

          // Mục đích sử dụng
          let mucDichSD = this.listMucDichSuDung.find(x => x.categoryCode == item.maMucDichSuDung)

          if (mucDichSD != null && mucDichSD != undefined) {
            taiSanYCTemp.mucDichSuDungId = mucDichSD ? mucDichSD.categoryId : this.emptyGuid;
            taiSanYCTemp.mucDichSuDung = mucDichSD.categoryName;
          }

          var datePipe = new DatePipe("en-US");
          // Từ ngày
          let ngayBatDau = item.ngayBatDau ? convertToUTCTime(item.ngayBatDau) : null;
          taiSanYCTemp.ngayBatDau = ngayBatDau;

          if (ngayBatDau != null) {
            taiSanYCTemp.ngayBatDauString = datePipe.transform(ngayBatDau, 'dd/MM/yyyy');
          }

          // Đến ngày
          let ngayKetThuc = item.ngayKetThuc ? convertToUTCTime(item.ngayKetThuc) : null;
          taiSanYCTemp.ngayKetThuc = ngayKetThuc;

          if (ngayKetThuc != null) {
            taiSanYCTemp.ngayKetThucString = datePipe.transform(ngayKetThuc, 'dd/MM/yyyy');
          }

          // So luong
          taiSanYCTemp.soLuong = Number(item.soLuong);

          // Mô tả
          taiSanYCTemp.moTa = item.moTa;

          // Lý do
          taiSanYCTemp.lyDo = item.lyDo;

          taiSanYCTemp.createdById = this.auth.UserId;
          taiSanYCTemp.createdDate = convertToUTCTime(new Date());
          taiSanYCTemp.updatedById = null;
          taiSanYCTemp.updatedDate = null;

          this.listTaiSanYeuCauTemp.push(taiSanYCTemp)
        });
      }
    });
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

  async downloadTemplateExcel() {
    let result: any = await this.assetService.downloadTemplateAsset(2);

    if (result.excelFile != null && result.statusCode === 200) {
      const binaryString = window.atob(result.excelFile);
      const binaryLen = binaryString.length;
      const bytes = new Uint8Array(binaryLen);
      for (let idx = 0; idx < binaryLen; idx++) {
        const ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      const fileName = result.nameFile + ".xlsx";
      link.download = fileName;
      link.click();
    } else {
      this.displayChooseFileImportDialog = false;

      this.showToast('error', 'Thông báo:', 'Download file thất bại');
    }
  }

  chooseFile(event: any) {
    this.fileName = event.target ?.files[0] ?.name;
    this.importFileExcel = event.target;
  }

  //#endregion

  //#region PHÂN BỔ TÀI SẢN
  showQRCode() {
    this.displayQRCode = true;
    this.qrCode = this.yeuCauCapPhatChiTietModel.maTaiSan;
  }

  closeQRCode() {
    this.displayQRCode = false;
  }
  openAddAssetDialog() {
    this.taoNhanhTSForm.reset();
    this.displayCreateAsset = true;
  }

  closeTaoTS() {
    this.displayCreateAsset = false;
  }

  taoNhanhTS() {
    if (!this.taoNhanhTSForm.valid) {
      Object.keys(this.taoNhanhTSForm.controls).forEach(key => {
        if (!this.taoNhanhTSForm.controls[key].valid) {
          this.taoNhanhTSForm.controls[key].markAsTouched();
        }
      });

      this.emitStatusChangeForm = this.taoNhanhTSForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });

      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    }
    else {
      this.loading = true;
      this.awaitResult = true;
      let donVi = this.taoNhanhTSForm.get('donViControl').value;

      let taiSanModel = new TaiSanModel();
      taiSanModel.maTaiSan = this.maTaiSanControl.value.trim()
      taiSanModel.tenTaiSan = this.tenTaiSanControl.value.trim()
      // Đơn vị
      taiSanModel.donViTinhId = donVi ? donVi.categoryId : this.emptyGuid
      taiSanModel.soLuong = 1,
        // Phân loại tài sản
        taiSanModel.phanLoaiTaiSanId = this.yeuCauCapPhatChiTietModel.loaiTaiSanId

      this.assetService.createOrUpdateAsset(taiSanModel, this.auth.UserId, true).subscribe(response => {
        this.loading = false;
        this.awaitResult = false;
        let result = <any>response;
        if (result.statusCode == 200) {
          this.displayCreateAsset = false;
          this.clearToast();
          this.showToast('success', 'Thông báo', 'Tạo nhanh tài sản thành công.');
          // Thêm vào danh sách sản phẩm phân bổ


          this.listTaiSanChuaPhanBo = result.listTaiSanChuaPhanBo;
          this.listTaiSanChuaPhanBoDefault = result.listTaiSanChuaPhanBo;

          this.ref.detectChanges();
        }
        else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.message);
        }
      });
    }
  }

  capPhatTaiSanOnRow(rowData: any, isParent: boolean) {

    this.yeuCauCapPhatChiTietModel = this.listTaiSanYeuCauTemp.find(x => x.yeuCauCapPhatTaiSanChiTietId == rowData.yeuCauCapPhatTaiSanChiTietId);
    this.yeuCauCapPhatChiTietModel.maTaiSan = '';
    // Sử dụng từ ngày
    this.taoPhanBoForm.controls['ngayBatDauPhanBoControl'].setValue(new Date(rowData.ngayBatDau));

    if (rowData.ngayKetThuc) {
      this.taoPhanBoForm.controls['ngayKetThucPhanBoControl'].setValue(new Date(rowData.ngayKetThuc));
    }

    this.isAddAsset = isParent;

    this.ref.detectChanges();
  }

  capNhapPhanBo() {

    if (this.taiSanPhanBoControl.value != null) {
      let taisan = this.listTaiSanChuaPhanBoDefault.find(x => x.taiSanId == this.taiSanPhanBoControl.value.taiSanId)

      let indexTSEdit = this.listTaiSanYeuCauTemp.findIndex(x => x.isUpdate == true)
      if (indexTSEdit != -1) {
        this.listTaiSanYeuCauTemp[indexTSEdit].taiSanId = taisan.taiSanId;
        this.listTaiSanYeuCauTemp[indexTSEdit].maTaiSan = taisan.maTaiSan;
        this.listTaiSanYeuCauTemp[indexTSEdit].isUpdate = false;
      }
      else {
        let taiSanPB = new YeuCauCapPhatChiTietModel();
        taiSanPB.taiSanId = taisan.taiSanId;
        taiSanPB.capPhatTaiSanId = 0;
        taiSanPB.maTaiSan = taisan.maTaiSan;
        taiSanPB.isUpdate = false;
        taiSanPB.loaiTaiSanId = taisan.phanLoaiTaiSanId;
        taiSanPB.mucDichSuDungId = this.yeuCauCapPhatChiTietModel.mucDichSuDungId;
        taiSanPB.nhanVienYeuCauId = this.yeuCauCapPhatChiTietModel.nhanVienYeuCauId;
        taiSanPB.lyDo = this.yeuCauCapPhatChiTietModel.lyDo;
        taiSanPB.ngayBatDau = this.yeuCauCapPhatChiTietModel.ngayBatDau;
        taiSanPB.ngayKetThuc = this.yeuCauCapPhatChiTietModel.ngayKetThuc;
        taiSanPB.createdById = this.auth.UserId;
        taiSanPB.parentPartId = this.yeuCauCapPhatChiTietModel.yeuCauCapPhatTaiSanChiTietId;
        taiSanPB.yeuCauCapPhatTaiSanChiTietId = this.yeuCauCapPhatChiTietModel.yeuCauCapPhatTaiSanChiTietId;
        this.listTaiSanYeuCauTemp.push(taiSanPB)

        // Remove tài sản đã chọn ra khỏi danh sách tài sản phân bổ
        this.listTaiSanChuaPhanBo = this.listTaiSanChuaPhanBo.filter(x => x.taiSanId != taisan.taiSanId);
      }
      this.rebuildYeuCauCPTree();
      this.taoPhanBoForm.reset();
      this.yeuCauCapPhatChiTietModel = new YeuCauCapPhatChiTietModel();
    }
  }

  suaTaiSanOnRow(rowData: any, isParent: boolean) {
    let taisan = this.listTaiSanChuaPhanBoDefault.find(x => x.taiSanId == rowData.taiSanId)
    this.listTaiSanChuaPhanBo.push(taisan);
    this.taiSanPhanBoControl.setValue(taisan);

    this.yeuCauCapPhatChiTietModel = this.listTaiSanYeuCauTemp.find(x => x.yeuCauCapPhatTaiSanChiTietId == rowData.parentPartId);

    // Sử dụng từ ngày
    this.taoPhanBoForm.controls['ngayBatDauPhanBoControl'].setValue(new Date(rowData.ngayBatDau));

    if (rowData.ngayKetThuc) {
      this.taoPhanBoForm.controls['ngayKetThucPhanBoControl'].setValue(new Date(rowData.ngayKetThuc));
    }
    rowData.isUpdate = true;

    this.ref.detectChanges();
  }

  thayDoiTaiSanPhanBo(event: any) {
    this.yeuCauCapPhatChiTietModel.maTaiSan = event.value.maTaiSan
    this.yeuCauCapPhatChiTietModel.taiSanId = event.value.taiSanId
  }
  // phân bổ tài sản sau khi hoàn thành quy trình phê duyệt
  async phanBoYeuCauCP() {
    if (this.listTaiSanYeuCauTemp.length > 0) {
      this.loading = true;
      this.awaitResult = true;
      let yeuCauCapPhat = this.listTaiSanYeuCauTemp.filter(x => x.capPhatTaiSanId === 0)
      this.assetService.taoPhanBoTaiSan(yeuCauCapPhat, this.auth.UserId).subscribe(async response => {
        this.loading = false;
        let result = <any>response;
        if (result.statusCode == 200) {
          let lstParent = this.listTaiSanYeuCauTemp.filter(a => a.yeuCauCapPhatTaiSanId !== 0);
          let totalChild = 0;
          let totalPheDuyet = 0;
          lstParent ?.forEach(chitiet => {
            totalChild += chitiet.totalChild;
            totalPheDuyet += chitiet.soLuongPheDuyet;
          });
          // Số lượng tài sản cấp phát = số lượng duyệt => Hoàn thành
          if (totalChild == totalPheDuyet) {
            let res: any = await this.assetService.capNhapTrangThaiYeuCauCapPhat(this.yeuCauCapPhatTaiSanId, 5);
            if (res.statusCode === 200) {
              this.showToast('success', 'Thông báo', "Hoàn thành phiếu cấp phát tài sản.");
              this.getMasterData();
            }
          }
          else {
            this.showToast('success', 'Thông báo', "Cập nhập phiếu cấp phát thành công.");
          }
          this.awaitResult = false;
        }
        else {
          let lstTaiSanId = result.listAssetId;
          let mes = 'Tài sản đã được phân bổ: ';
          lstTaiSanId.forEach(id => {
            var taisan = this.listTaiSanChuaPhanBo.filter(x => x.taiSanId == id)[0];
            mes += + ' ' + taisan.maTaiSan + ' - ' + taisan.tenTaiSan;
          });
          this.clearToast();
          this.showToast('error', 'Thông báo', mes);
        }
      });
    }
    else {
      this.clearToast();
      this.showToast('error', 'Thông báo', "Vui lòng chọn tài sản để cấp phát!");
    }

  }

  rebuildYeuCauCPTree() {

    this.dataTree = [];
    let newDataTree = [];
    this.listTaiSanYeuCauTemp.forEach((item: YeuCauCapPhatChiTietModel) => {
      item.totalChild = 0;
      let parent_item: TreeNode = {
        data: {},
        children: []
      };

      //Lấy các item cha
      if (item.parentPartId == null) {
        parent_item.data = item;

        //Thêm vào dataTree
        newDataTree.push(parent_item);
        let count = 0;
        let listItemLv1 = this.listTaiSanYeuCauTemp.filter(x => x.parentPartId == item.yeuCauCapPhatTaiSanChiTietId);
        listItemLv1.forEach((item_lv1: YeuCauCapPhatChiTietModel) => {
          let _item_lv1: TreeNode = {
            data: {},
            children: []
          };
          count++;
          _item_lv1.data = item_lv1;

          //Thêm vào dataTree
          let find_item_lv0 = newDataTree.find(x => x.data.yeuCauCapPhatTaiSanChiTietId == item_lv1.parentPartId);
          find_item_lv0.children.push(_item_lv1);
        });

        parent_item.data.totalChild = count;
      }
    });
    this.dataTree = [...newDataTree];

  }

  xemChiTietTaiSan(rowData: any) {

    let url = this.router.serializeUrl(this.router.createUrlTree(['/asset/detail', { assetId: this.encrDecrService.set(rowData.taiSanId) }]));
    window.open(url, '_blank');
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
