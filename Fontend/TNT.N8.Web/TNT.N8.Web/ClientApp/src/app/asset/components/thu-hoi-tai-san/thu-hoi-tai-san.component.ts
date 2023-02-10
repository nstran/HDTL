import { Component, OnInit, ViewChild, ElementRef, Renderer2, HostListener, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { AssetService } from '../../services/asset.service';
import { TaiSanModel } from '../../models/taisan.model';
import { Paginator, Table } from 'primeng';
import { DatePipe } from '@angular/common';
import * as XLSX from 'xlsx';
import { ThuHoiImportDetailComponent } from '../thu-hoi-import-detail/thu-hoi-import-detail.component';
import { ThuHoiTaiSanModel } from '../../models/thu-hoi-tai-san.model';
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
  /*
       Mã tài sản
       Tên tài sản
       Thu hồi ngày
       Lý do
      */
  maTaiSan: String;
  tenTaiSan: String;
  ngayBatDau: Date;
  lyDo: String;
}


@Component({
  selector: 'app-thu-hoi-tai-san',
  templateUrl: './thu-hoi-tai-san.component.html',
  styleUrls: ['./thu-hoi-tai-san.component.css']
})
export class ThuHoiTaiSanComponent implements OnInit {
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
  actionAdd: boolean = true;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;

  /* Form */
  thuHoiTaiSanForm: FormGroup;
  // Thông tin chung
  loaiTaiSanPBControl: FormControl;
  taiSanThuHoiControl: FormControl;
  lyDoThuHoiControl: FormControl;
  ngayThuHoiControl: FormControl;

  // Dữ liệu masterdata
  listLoaiTSPB: Array<CategoryModel> = new Array<CategoryModel>();

  // Dữ liệu trả về
  listTaiSanDefault: any[];
  listTaiSan: Array<TaiSanModel> = new Array<TaiSanModel>();
  listEmployee: Array<EmployeeModel> = new Array<EmployeeModel>();

  // notification
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;

  taiSanModel: TaiSanModel = new TaiSanModel();
  employeeModel: EmployeeModel = new EmployeeModel();
  isUpdate: boolean = false;

  selectedColumns: any[];
  cols: any[];

  listTaiSanThuHoiTemp: Array<ThuHoiTaiSanModel> = new Array<ThuHoiTaiSanModel>();
  capPhatTaiSanId: number = 0;

  // Excel
  displayChooseFileImportDialog: boolean = false;
  fileName: string = '';
  importFileExcel: any = null;

  taiSanId: number = 0;
  disableForm: boolean = false;
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
    let resource = "ass/asset/create";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
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

      this.route.params.subscribe(params => {
        this.taiSanId = Number(this.encrDecrService.get(params['assetId']));
      });
      // this.setTable();
      // this.setForm();
      this.getMasterData();
      this.ngayThuHoiControl.setValue(new Date());
    }
  }

  setForm() {
    this.loaiTaiSanPBControl = new FormControl(null, [Validators.required]);
    this.taiSanThuHoiControl = new FormControl(null, [Validators.required]);
    this.ngayThuHoiControl = new FormControl(null, [Validators.required]);
    this.lyDoThuHoiControl = new FormControl(null);

    this.thuHoiTaiSanForm = new FormGroup({
      loaiTaiSanPBControl: this.loaiTaiSanPBControl,
      taiSanThuHoiControl: this.taiSanThuHoiControl,
      ngayThuHoiControl: this.ngayThuHoiControl,
      lyDoThuHoiControl: this.lyDoThuHoiControl,
    });
  }

  getMasterData() {
    this.loading = true;
    this.assetService.getMasterDataThuHoiTSForm().subscribe(response => {
      let result: any = response;

      this.loading = false;
      if (result.statusCode == 200) {
        this.listLoaiTSPB = result.listLoaiTSPB;
        this.listEmployee = result.listEmployee;
        this.listTaiSanDefault = result.listTaiSan;

        if (this.taiSanId != 0 && this.taiSanId != undefined) {
          this.setDefaultForm();
          // this.disableForm = true;
        }
      }
    });
  }

  setDefaultForm() {
    let taiSan = this.listTaiSanDefault.find(x => x.taiSanId == this.taiSanId)
    if (taiSan != null && taiSan != undefined) {
      let phanLoai = this.listLoaiTSPB.find(x => x.categoryId == taiSan.phanLoaiTaiSanId)
      this.loaiTaiSanPBControl.setValue(phanLoai)
      this.thayDoiLoaiTaiSan(phanLoai);
    }
  }

  setTable() {
    this.cols = [
      { field: 'tenTaiSan', header: 'Tên tài sản', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'employeeName', header: 'Họ tên nhân viên sử dụng', width: '250px', textAlign: 'left', color: '#f44336' },
      { field: 'organizationName', header: 'Phòng ban', width: '200px', textAlign: 'left', color: '#f44336' },
      { field: 'positionName', header: 'Vị trí làm việc', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'ngayBatDauString', header: 'Ngày thu hồi', width: '100px', textAlign: 'left', color: '#f44336' },
      { field: 'lyDo', header: 'Lý do', width: '150px', textAlign: 'left', color: '#f44336' },
    ];

    this.selectedColumns = this.cols;
  }

  // tạo thu hồi tài sản
  thuHoiTaiSan() {
    if (this.listTaiSanThuHoiTemp.length > 0) {
      this.loading = true;
      this.awaitResult = true;
      this.assetService.taoThuHoiTaiSan(this.listTaiSanThuHoiTemp, this.auth.UserId).subscribe(response => {
        this.loading = false;
        let result = <any>response;
        if (result.statusCode == 200) {
          this.showToast('success', 'Thông báo', result.message);
          setTimeout(() => {
            this.router.navigate(['/asset/list']);
          }, 1000);
          this.awaitResult = false;
        }
        else {
          let lstTaiSanId = result.listAssetId;
          let mes = 'Tài sản đã được thu hồi: ';
          lstTaiSanId.forEach(id => {
            var taisan = this.listTaiSan.filter(x => x.taiSanId == id)[0];
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

  //Function reset toàn bộ các value đã nhập trên form
  resetFieldValue() {
    this.thuHoiTaiSanForm.reset();
  }

  mapFormToThuHoiTaiSanModel(): Array<ThuHoiTaiSanModel> {
    let lstThuHoi = new Array<ThuHoiTaiSanModel>();
    this.listTaiSanThuHoiTemp.forEach(taisan => {
      lstThuHoi.push(taisan)
    });

    return lstThuHoi;
  }

  mapFormToThuHoiTaiSanModelTemp(isUpdate: boolean = false): ThuHoiTaiSanModel {
    let thuHoiTSModel = new ThuHoiTaiSanModel();
    if (!isUpdate) {
      const maxValueOfY = Math.max(...this.listTaiSanThuHoiTemp.map(o => o.capPhatTaiSanId), 0);
      thuHoiTSModel.capPhatTaiSanId = maxValueOfY + 1;
    }

    // Tài sản
    let taiSan = this.thuHoiTaiSanForm.get('taiSanThuHoiControl').value;
    if (taiSan != null && taiSan != undefined) {
      thuHoiTSModel.taiSanId = taiSan ? taiSan.taiSanId : 0;
      thuHoiTSModel.tenTaiSan = taiSan.tenTaiSan;
      thuHoiTSModel.nguoiSuDungId = taiSan ? taiSan.employeeId : this.emptyGuid;
      thuHoiTSModel.employeeName = taiSan.hoVaTen;
      thuHoiTSModel.employeeCode = taiSan.maNV;
      thuHoiTSModel.organizationName = taiSan.phongBan;
      thuHoiTSModel.positionName = taiSan.viTriLamViec;
    }

    var datePipe = new DatePipe("en-US");
    // Ngày thu hồi
    let ngayThuHoi = this.thuHoiTaiSanForm.get('ngayThuHoiControl').value ? convertToUTCTime(this.thuHoiTaiSanForm.get('ngayThuHoiControl').value) : null;
    thuHoiTSModel.ngayBatDau = ngayThuHoi;
    if (this.thuHoiTaiSanForm.get('ngayThuHoiControl').value != null) {
      thuHoiTSModel.ngayBatDauString = datePipe.transform(this.thuHoiTaiSanForm.get('ngayThuHoiControl').value, 'dd/MM/yyyy');
    }

    // Lý do
    thuHoiTSModel.lyDo = this.lyDoThuHoiControl.value;

    thuHoiTSModel.createdById = this.auth.UserId;
    thuHoiTSModel.createdDate = convertToUTCTime(new Date());
    thuHoiTSModel.updatedById = null;
    thuHoiTSModel.updatedDate = null;
    return thuHoiTSModel;
  }

  cancel() {
    this.router.navigate(['/asset/list']);
  }

  createThuHoiTemp() {
    if (!this.thuHoiTaiSanForm.valid) {
      Object.keys(this.thuHoiTaiSanForm.controls).forEach(key => {
        if (!this.thuHoiTaiSanForm.controls[key].valid) {
          this.thuHoiTaiSanForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.thuHoiTaiSanForm.statusChanges.subscribe((validity: string) => {
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
      let thuHoiModel = this.mapFormToThuHoiTaiSanModelTemp();
      this.listTaiSanThuHoiTemp.push(thuHoiModel);
      this.listTaiSan = this.listTaiSan.filter(x => x.taiSanId != thuHoiModel.taiSanId);
      this.def.detectChanges();
      this.refreshForm();
    }
  }

  updateThuHoiTSTemp() {
    if (!this.thuHoiTaiSanForm.valid) {
      Object.keys(this.thuHoiTaiSanForm.controls).forEach(key => {
        if (!this.thuHoiTaiSanForm.controls[key].valid) {
          this.thuHoiTaiSanForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.thuHoiTaiSanForm.statusChanges.subscribe((validity: string) => {
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
      let thuHoiModelUpdate = this.mapFormToThuHoiTaiSanModelTemp(true);
      const index = this.listTaiSanThuHoiTemp.findIndex(obj => obj.capPhatTaiSanId === this.capPhatTaiSanId);
      this.listTaiSanThuHoiTemp[index] = thuHoiModelUpdate;
      this.def.detectChanges();
      this.refreshForm();
    }
  }

  findIndexToUpdate(newItem) {
    return newItem.id === this;
  }

  delAssetTemp(rowData: any) {
    this.listTaiSanThuHoiTemp = this.listTaiSanThuHoiTemp.filter(x => x.taiSanId != rowData.taiSanId);

    let taiSan = new TaiSanModel();
    taiSan = this.listTaiSanDefault.find(x => x.taiSanId == rowData.taiSanId);
    this.listTaiSan.push(taiSan);
    this.def.detectChanges();
    this.refreshForm();
  }

  /*Hiển thị lại thông tin tài sản*/
  reShowAsset(rowData: any) {
    this.isUpdate = true;
    this.capPhatTaiSanId = rowData.capPhatTaiSanId;
    // Tài sản
    let taiSan = this.listTaiSanDefault.find(c => c.taiSanId == rowData.taiSanId);
    this.thuHoiTaiSanForm.controls['taiSanThuHoiControl'].setValue(taiSan);

    this.taiSanModel = taiSan;

    // Loại tài sản phân bổ
    let phanLoai = this.listLoaiTSPB.find(c => c.categoryId == taiSan.phanLoaiTaiSanId);
    this.thuHoiTaiSanForm.controls['loaiTaiSanPBControl'].setValue(phanLoai);

    // Mã nhân viên
    let nhanVien = this.listEmployee.find(c => c.employeeId == taiSan.nguoiSuDungId);
    this.employeeModel = nhanVien;

    this.thuHoiTaiSanForm.controls['ngayThuHoiControl'].setValue(new Date(rowData.ngayBatDau));

    this.thuHoiTaiSanForm.controls['lyDoThuHoiControl'].setValue(rowData.lyDo);

    this.thuHoiTaiSanForm.controls['taiSanThuHoiControl'].updateValueAndValidity();
    this.thuHoiTaiSanForm.controls['loaiTaiSanPBControl'].updateValueAndValidity();
    this.thuHoiTaiSanForm.controls['ngayThuHoiControl'].updateValueAndValidity();
  }

  thayDoiLoaiTaiSan(event: any) {
    let lstTSId: any[] = []
    this.listTaiSanThuHoiTemp?.forEach(item => {
      lstTSId.push(item.taiSanId);
    });

    this.listTaiSan = this.listTaiSanDefault.filter(x => x.phanLoaiTaiSanId == event.categoryId);
    // Loại bỏ các tài sản đã nằm trong listTaiSanThuHoiTemp

    if (this.taiSanId != 0 && this.taiSanId != undefined) {
      this.taiSanThuHoiControl.setValue(this.listTaiSan[0])
      this.thayDoiTaiSan(this.listTaiSan[0])
    }
    if (lstTSId.length > 0) {
      this.listTaiSan = this.listTaiSan.filter(x => !lstTSId.includes(x.taiSanId));
    }
  }

  thayDoiTaiSan(event: any) {
    let taiSan = this.listTaiSan.find(x => x.taiSanId == event.taiSanId);
    this.taiSanModel = taiSan;
    let nhanVien = new EmployeeModel();
    nhanVien.employeeCode = taiSan.maNV;
    nhanVien.employeeName = taiSan.hoVaTen;
    nhanVien.organizationName = taiSan.phongBan;
    nhanVien.positionName = taiSan.viTriLamViec;
    this.employeeModel = nhanVien;
  }

  refreshForm() {
    this.capPhatTaiSanId = 0;
    this.thuHoiTaiSanForm.reset();
    this.employeeModel = new EmployeeModel();
    this.taiSanModel = new TaiSanModel();
    this.isUpdate = false;
    this.ngayThuHoiControl.setValue(new Date());
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
      let code = 'DanhMucTaiSan';
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
      listAssetRawData?.forEach(_rawData => {
        /*
         Mã tài sản
         Tên tài sản
         Thu hồi ngày
         Lý do
        */
        let asset = new importAssetByExcelModel();
        asset.maTaiSan = _rawData[0] ? _rawData[0].toString().trim() : '';
        asset.tenTaiSan = _rawData[1] ? _rawData[1].toString().trim() : '';
        if (_rawData[2] != undefined) {
          let ngayBD = new Date(_rawData[2].replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
          asset.ngayBatDau = convertToUTCTime(ngayBD);
        }

        asset.lyDo = _rawData[3] ? _rawData[3].toString().trim() : '';
        listAssetImport = [...listAssetImport, asset];
      });
      /* tắt dialog import file, bật dialog chi tiết tài sản import */
      this.displayChooseFileImportDialog = false;
      this.loading = false
      this.openDetailImportDialog(listAssetImport);
    }
  }

  openDetailImportDialog(listAssetImport) {
    let ref = this.dialogService.open(ThuHoiImportDetailComponent, {
      data: {
        listAssetImport: listAssetImport
      },
      header: 'Nhập excel danh sách tài sản thu hồi',
      width: '85%',
      baseZIndex: 1050,
      contentStyle: {
        "max-height": "800px",
      }
    });
    ref.onClose.subscribe((result: any) => {
      if (result?.status) {
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
    let result: any = await this.assetService.downloadTemplateAsset(0);
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
    this.fileName = event.target?.files[0]?.name;
    this.importFileExcel = event.target;
  }

  //#endregion
}

function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
      }
    }
  }
  return null;
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function addMonths(numOfMonths, date) {
  date.setMonth(date.getMonth() + numOfMonths);

  return date;
}
