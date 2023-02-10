import { Component, OnInit, ViewChild, ElementRef, Renderer2, HostListener, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { AssetService } from '../../services/asset.service';
import { Paginator, Table } from 'primeng';
import { DatePipe } from '@angular/common';
import { YeuCauCapPhatModel } from '../../models/yeu-cau-cap-phat';
import * as XLSX from 'xlsx';
import { YeuCauCapPhatChiTietModel } from '../../models/yeu-cau-cap-phat-chi-tiet';
import { YeuCauCapPhatImportDetailComponent } from '../yeu-cau-cap-phat-import-detail/yeu-cau-cap-phat-import-detail.component';
import { EncrDecrService } from '../../../shared/services/encrDecr.service';

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

@Component({
  selector: 'app-yeu-cau-cap-phat-create',
  templateUrl: './yeu-cau-cap-phat-create.component.html',
  styleUrls: ['./yeu-cau-cap-phat-create.component.css']
})
export class YeuCauCapPhatCreateComponent implements OnInit {
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
  /*END*/
  // Label trên form
  nguoiDeXuat: string = '';

  /* Form */
  yeuCauCapPhatForm: FormGroup;
  // Thông tin chung
  ngayDeXuatControl: FormControl;
  loaiTaiSanControl: FormControl;
  moTaControl: FormControl;
  nhanVienControl: FormControl;
  mucDichSuDungControl: FormControl;
  ngayBatDauControl: FormControl;
  ngayKetThucControl: FormControl;
  lyDoControl: FormControl;
  soLuongControl: FormControl;

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
  // Excel
  displayChooseFileImportDialog: boolean = false;
  fileName: string = '';
  importFileExcel: any = null;

  disableForm: boolean = false;
  statusCode: string = null;

  ngayDeXuat = new Date();

  constructor(
    private assetService: AssetService,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private getPermission: GetPermission,
    private el: ElementRef,
    private def: ChangeDetectorRef,
    private encrDecrService: EncrDecrService,
  ) {

  }

  async ngOnInit() {
    this.setTable();
    this.setForm();
    let resource = "ass/asset/yeu-cau-cap-phat-tai-san/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.showToast('warn', 'Thông báo', 'Bạn không có quyền truy cập');
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }

      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      // this.setTable();
      // this.setForm();
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
    this.lyDoControl = new FormControl(null);
    this.moTaControl = new FormControl(null);
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
  }

  async getMasterData() {
    this.loading = true;

    let result: any = await this.assetService.getMasterDataYeuCauCapPhatForm();
    this.loading = false;
    if (result.statusCode == 200) {
      this.listLoaiTS = result.listLoaiTSPB;
      this.listMucDichSuDung = result.listMucDichSuDung;
      this.listEmployee = result.listEmployee;

      this.nguoiDeXuat = this.listEmployee.find(x => x.employeeId == this.auth.EmployeeId).employeeName;
    }
  }

  setTable() {
    this.cols = [
      { field: 'loaiTaiSan', header: 'Loại tài sản', width: '120px', textAlign: 'center', color: '#f44336' },
      { field: 'soLuong', header: 'Số lượng', width: '80px', textAlign: 'center', color: '#f44336' },
      { field: 'moTa', header: 'Mô tả', width: '200px', textAlign: 'left', color: '#f44336' },
      { field: 'tenNhanVien', header: 'Họ tên NV yêu cầu', width: '200px', textAlign: 'left', color: '#f44336' },
      { field: 'phongBan', header: 'Phòng ban', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'viTriLamViec', header: 'Vị trí làm việc', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'mucDichSuDung', header: 'Mục đích sử dụng', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'ngayBatDauString', header: 'Từ ngày', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'ngayKetThucString', header: 'Đến ngày', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'lyDo', header: 'Lý do', width: '150px', textAlign: 'left', color: '#f44336' },
    ];

    this.selectedColumns = this.cols;
  }
  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  // tạo tài sản
  taoYeuCauCapPhat(isSaveNew) {
    if (this.ngayDeXuat == null) {

      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi

      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    }
    else {
      if (this.listTaiSanYeuCauTemp.length > 0) {
        {
          this.isInvalidForm = false;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = false;  //Hiển thị message lỗi
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
          this.yeuCauCapPhat.ngayDeXuat = this.ngayDeXuat;
          this.yeuCauCapPhat.nguoiDeXuatId = this.auth.EmployeeId;

          this.assetService.createOrYeuCauCapPhat(this.yeuCauCapPhat, this.listTaiSanYeuCauTemp, 'YCCAPPHAT', listFileUploadModel, this.auth.UserId).subscribe(response => {

            this.loading = false;
            let result = <any>response;

            if (result.statusCode == 200) {
              this.showToast('success', 'Thông báo', "Tạo yêu cầu cấp phát thành công.");
              //Lưu và thêm mới
              if (isSaveNew) {
                this.resetFieldValue();
                this.listTaiSanYeuCauTemp = new Array<YeuCauCapPhatChiTietModel>();
                this.awaitResult = false;
              }
              //Lưu
              else {
                this.resetFieldValue();
                setTimeout(() => {

                  this.router.navigate(['/asset/chi-tiet-yeu-cau-cap-phat', { requestId: this.encrDecrService.set(result.yeuCauCapPhatTaiSanId) }]);
                }, 1000)
                this.awaitResult = false;
              }
            }
            else {

              this.awaitResult = false;
              this.clearToast();
              this.showToast('error', 'Thông báo', "Có lỗi trong quá trình lưu dũ liệu.");
            }
          });
        }
      }
      else {
        this.showToast('error', 'Thông báo', 'Vui lòng chọn tài sản cần yêu cầu.');
      }
    }
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
    yeuCauChiTietModel.moTa = this.moTaControl.value;

    // Lý do
    yeuCauChiTietModel.lyDo = this.lyDoControl.value;

    yeuCauChiTietModel.createdById = this.auth.UserId;
    yeuCauChiTietModel.createdDate = convertToUTCTime(new Date());
    yeuCauChiTietModel.updatedById = null;
    yeuCauChiTietModel.updatedDate = null;
    return yeuCauChiTietModel;
  }

  cancel() {
    this.router.navigate(['/asset/danh-sach-yeu-cau-cap-phat']);
  }

  createYeuCauTemp() {
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
      let phanBoModel = this.mapFormToYCCapPhatModelTemp();
      this.listTaiSanYeuCauTemp.push(phanBoModel);
      this.def.detectChanges();
      this.refreshForm();
    }
  }

  updateYeuCauTemp() {
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
      let yeuCauCPModel = this.mapFormToYCCapPhatModelTemp(true);
      const index = this.listTaiSanYeuCauTemp.findIndex(obj => obj.yeuCauCapPhatTaiSanId === this.yeuCauCapPhatTaiSanId);
      this.listTaiSanYeuCauTemp[index] = yeuCauCPModel;
      this.def.detectChanges();
      this.refreshForm();
    }
  }

  delAssetTemp(rowData: any) {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn xóa dòng này?`,
      accept: async () => {
        this.listTaiSanYeuCauTemp = this.listTaiSanYeuCauTemp.filter(x => x != rowData);
        this.def.detectChanges();
        this.refreshForm();
      }
    });
  }

  /*Hiển thị lại thông tin tài sản*/
  reShowAsset(rowData: any) {
    this.isUpdate = true;
    this.yeuCauCapPhatTaiSanId = rowData.yeuCauCapPhatTaiSanId;

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
    this.yeuCauCapPhatTaiSanId = 0;
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

  //#region Excel
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

  //#region TÀI LIỆU LIÊN QUAN

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
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.arrayDocumentModel.indexOf(file);
        this.arrayDocumentModel.splice(index, 1);
      }
    });
  }
  //#endregion

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
