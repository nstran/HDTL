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
import { PhanBoTaiSanModel } from '../../models/phanbotaisan.model';
import * as XLSX from 'xlsx';
import { PhanBoImportDetailComponent } from '../phan-bo-import-detail/phan-bo-import-detail.component';
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
    // Mã tài sản
  // Tên tài sản
  // Mã Code
  // Mã nhân viên
  // Tên nhân viên
  // Phòng ban
  // Vị trí làm việc
  // Mã mục đích sử dụng
  // Sử dụng từ ngày
  // Sử dụng đến ngày
  // Lý do
  maTaiSan: String;
  tenTaiSan: String;
  maCode: String;
  employeeCode: String;
  employeeName: String;
  maMucDichSuDung: String;
  mucDichSuDung: String;
  organizationName: String;
  positionName: String;
  ngayBatDau: Date;
  ngayKetThuc: Date;
  lyDo: String;
}


@Component({
  selector: 'app-phan-bo-tai-san',
  templateUrl: './phan-bo-tai-san.component.html',
  styleUrls: ['./phan-bo-tai-san.component.css']
})
export class PhanBoTaiSanComponent implements OnInit {
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

  nhanVienId: string = null; //employeeId khi click từ tab Cấp phát tài sản trong Chi tiết nhân viên

  /* Form */
  phanBoTaiSanForm: FormGroup;
  // Thông tin chung
  loaiTaiSanPBControl: FormControl;
  taiSanPBControl: FormControl;
  nhanVienControl: FormControl;
  mucDichSuDungControl: FormControl;
  ngayBatDauControl: FormControl;
  ngayKetThucControl: FormControl;
  lyDoPBControl: FormControl;

  // Dữ liệu masterdata
  listLoaiTSPB: Array<CategoryModel> = new Array<CategoryModel>();
  listMucDichSuDung: Array<CategoryModel> = new Array<CategoryModel>();

  // Dữ liệu trả về
  listTaiSanDefault: Array<TaiSanModel> = new Array<TaiSanModel>();
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

  listTaiSanPBTemp: Array<PhanBoTaiSanModel> = new Array<PhanBoTaiSanModel>();
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

      this.route.params.subscribe(params => {
        this.nhanVienId = params['employeeId'];
      });

      // this.setTable();
      // this.setForm();
      this.getMasterData();
    }
  }

  setForm() {
    this.loaiTaiSanPBControl = new FormControl(null, [Validators.required]);
    this.taiSanPBControl = new FormControl(null, [Validators.required]);
    this.nhanVienControl = new FormControl(null, [Validators.required]);
    this.mucDichSuDungControl = new FormControl(null, [Validators.required]);
    this.ngayBatDauControl = new FormControl(null, [Validators.required]);
    this.ngayKetThucControl = new FormControl(null);
    this.lyDoPBControl = new FormControl(null);


    this.phanBoTaiSanForm = new FormGroup({
      loaiTaiSanPBControl: this.loaiTaiSanPBControl,
      taiSanPBControl: this.taiSanPBControl,
      nhanVienControl: this.nhanVienControl,
      mucDichSuDungControl: this.mucDichSuDungControl,
      ngayBatDauControl: this.ngayBatDauControl,
      ngayKetThucControl: this.ngayKetThucControl,
      lyDoPBControl: this.lyDoPBControl,
    });
  }

  getMasterData() {
    this.loading = true;
    this.assetService.getMasterDataPhanBoTSForm().subscribe(response => {
      let result: any = response;

      this.loading = false;
      if (result.statusCode == 200) {
        this.listLoaiTSPB = result.listLoaiTSPB;
        this.listMucDichSuDung = result.listMucDichSuDung;
        this.listEmployee = result.listEmployee;

        this.listTaiSanDefault = result.listTaiSan;
        if (this.taiSanId != 0 && this.taiSanId != undefined) {
          this.setDefaultForm();
          // this.disableForm = true;
        }

        if (this.nhanVienId) {
          let employee = this.listEmployee.find(x => x.employeeId == this.nhanVienId);
          if (employee) {
            this.nhanVienControl.setValue(employee)
          }
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
      { field: 'mucDichSuDung', header: 'Mục đích sử dụng', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'ngayBatDauString', header: 'Từ ngày', width: '100px', textAlign: 'left', color: '#f44336' },
      { field: 'ngayKetThucString', header: 'Đến ngày', width: '100px', textAlign: 'left', color: '#f44336' },
      { field: 'lyDo', header: 'Lý do', width: '150px', textAlign: 'left', color: '#f44336' },
    ];

    this.selectedColumns = this.cols;
  }

  // tạo tài sản
  taoPhanBoTaiSan(isSaveNew) {
    if (this.listTaiSanPBTemp.length > 0) {
      this.loading = true;
      this.awaitResult = true;
      this.assetService.taoPhanBoTaiSan(this.listTaiSanPBTemp, this.auth.UserId).subscribe(response => {
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
          let mes = 'Tài sản đã được phân bổ: ';
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
    this.phanBoTaiSanForm.reset();
  }

  mapFormToPhanBoTaiSanModel(): Array<PhanBoTaiSanModel> {
    let lstPhanBo = new Array<PhanBoTaiSanModel>();
    this.listTaiSanPBTemp.forEach(taisan => {
      lstPhanBo.push(taisan)
    });

    return lstPhanBo;
  }

  mapFormToPhanBoTaiSanModelTemp(isUpdate: boolean = false): PhanBoTaiSanModel {

    let phanBoTSModel = new PhanBoTaiSanModel();
    if (!isUpdate) {
      const maxValueOfY = Math.max(...this.listTaiSanPBTemp.map(o => o.capPhatTaiSanId), 0);
      phanBoTSModel.capPhatTaiSanId = maxValueOfY + 1;
    }
    // Tài sản
    let taiSan = this.phanBoTaiSanForm.get('taiSanPBControl').value;
    if (taiSan != null && taiSan != undefined) {
      phanBoTSModel.taiSanId = taiSan ? taiSan.taiSanId : 0;
      phanBoTSModel.tenTaiSan = taiSan.tenTaiSan;
    }

    // Nhân viên
    let nhanVien = this.phanBoTaiSanForm.get('nhanVienControl').value
    phanBoTSModel.nguoiSuDungId = nhanVien ? nhanVien.employeeId : this.emptyGuid;
    phanBoTSModel.employeeName = nhanVien.employeeName;
    phanBoTSModel.organizationName = nhanVien.organizationName;
    phanBoTSModel.positionName = nhanVien.positionName;

    // Mục đích sử dụng
    let mucDichSD = this.phanBoTaiSanForm.get('mucDichSuDungControl').value

    if (mucDichSD != null && mucDichSD != undefined) {
      phanBoTSModel.mucDichSuDungId = mucDichSD ? mucDichSD.categoryId : this.emptyGuid;
      phanBoTSModel.mucDichSuDung = mucDichSD.categoryName;
    }

    var datePipe = new DatePipe("en-US");
    // Từ ngày
    let ngayBatDau = this.phanBoTaiSanForm.get('ngayBatDauControl').value ? convertToUTCTime(this.phanBoTaiSanForm.get('ngayBatDauControl').value) : null;
    phanBoTSModel.ngayBatDau = ngayBatDau;
    if (this.phanBoTaiSanForm.get('ngayBatDauControl').value != null) {

      phanBoTSModel.ngayBatDauString = datePipe.transform(this.phanBoTaiSanForm.get('ngayBatDauControl').value, 'dd/MM/yyyy');
    }

    // Đến ngày
    let ngayKetThuc = this.phanBoTaiSanForm.get('ngayKetThucControl').value ? convertToUTCTime(this.phanBoTaiSanForm.get('ngayKetThucControl').value) : null;
    phanBoTSModel.ngayKetThuc = ngayKetThuc;
    if (this.phanBoTaiSanForm.get('ngayKetThucControl').value != null) {

      phanBoTSModel.ngayKetThucString = datePipe.transform(this.phanBoTaiSanForm.get('ngayKetThucControl').value, 'dd/MM/yyyy');
    }

    // Lý do
    phanBoTSModel.lyDo = this.lyDoPBControl.value;

    phanBoTSModel.createdById = this.auth.UserId;
    phanBoTSModel.createdDate = convertToUTCTime(new Date());
    phanBoTSModel.updatedById = null;
    phanBoTSModel.updatedDate = null;
    return phanBoTSModel;
  }

  cancel() {
    this.router.navigate(['/asset/list']);
  }

  createPBTSTemp() {
    if (!this.phanBoTaiSanForm.valid) {
      Object.keys(this.phanBoTaiSanForm.controls).forEach(key => {
        if (!this.phanBoTaiSanForm.controls[key].valid) {
          this.phanBoTaiSanForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.phanBoTaiSanForm.statusChanges.subscribe((validity: string) => {
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
      let phanBoModel = this.mapFormToPhanBoTaiSanModelTemp();
      this.listTaiSanPBTemp.push(phanBoModel);
      this.listTaiSan = this.listTaiSan.filter(x => x.taiSanId != phanBoModel.taiSanId);
      this.def.detectChanges();
      this.refreshForm();
    }
  }

  updatePBTSTemp() {
    if (!this.phanBoTaiSanForm.valid) {
      Object.keys(this.phanBoTaiSanForm.controls).forEach(key => {
        if (!this.phanBoTaiSanForm.controls[key].valid) {
          this.phanBoTaiSanForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.phanBoTaiSanForm.statusChanges.subscribe((validity: string) => {
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
      let phanBoModelUpdate = this.mapFormToPhanBoTaiSanModelTemp(true);
      const index = this.listTaiSanPBTemp.findIndex(obj => obj.capPhatTaiSanId === this.capPhatTaiSanId);
      this.listTaiSanPBTemp[index] = phanBoModelUpdate;
      this.def.detectChanges();
      this.refreshForm();
    }
  }

  delAssetTemp(rowData: any) {
    this.listTaiSanPBTemp = this.listTaiSanPBTemp.filter(x => x.taiSanId != rowData.taiSanId);
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
    this.phanBoTaiSanForm.controls['taiSanPBControl'].setValue(taiSan);

    this.taiSanModel = taiSan;

    // Loại tài sản phân bổ
    let phanLoai = this.listLoaiTSPB.find(c => c.categoryId == taiSan.phanLoaiTaiSanId);
    this.phanBoTaiSanForm.controls['loaiTaiSanPBControl'].setValue(phanLoai);

    // Mã nhân viên
    let nhanVien = this.listEmployee.find(c => c.employeeId == rowData.nguoiSuDungId);
    this.phanBoTaiSanForm.controls['nhanVienControl'].setValue(nhanVien);

    this.employeeModel = nhanVien;

    // Mục đích sử dụng
    let mucDich = this.listMucDichSuDung.find(c => c.categoryId == rowData.mucDichSuDungId);
    this.phanBoTaiSanForm.controls['mucDichSuDungControl'].setValue(mucDich);

    this.phanBoTaiSanForm.controls['ngayBatDauControl'].setValue(new Date(rowData.ngayBatDau));

    if (rowData.ngayKetThuc) {
      this.phanBoTaiSanForm.controls['ngayKetThucControl'].setValue(new Date(rowData.ngayKetThuc));
    }

    this.phanBoTaiSanForm.controls['lyDoPBControl'].setValue(rowData.lyDo);

    this.phanBoTaiSanForm.controls['taiSanPBControl'].updateValueAndValidity();
    this.phanBoTaiSanForm.controls['loaiTaiSanPBControl'].updateValueAndValidity();
    this.phanBoTaiSanForm.controls['nhanVienControl'].updateValueAndValidity();
    this.phanBoTaiSanForm.controls['mucDichSuDungControl'].updateValueAndValidity();
  }

  thayDoiLoaiTaiSan(event: any) {
    let lstTSId: any[] = []
    this.listTaiSanPBTemp?.forEach(item => {
      lstTSId.push(item.taiSanId);
    });

    this.listTaiSan = this.listTaiSanDefault.filter(x => x.phanLoaiTaiSanId == event.categoryId);
    // Loại bỏ các tài sản đã nằm trong listPhanBoTSTemp
    if (this.taiSanId != 0 && this.taiSanId != undefined) {
      this.taiSanPBControl.setValue(this.listTaiSan[0])
      this.thayDoiTaiSan(this.listTaiSan[0])
    }

    if (lstTSId.length > 0) {
      this.listTaiSan = this.listTaiSan.filter(x => !lstTSId.includes(x.taiSanId));
    }
  }

  thayDoiTaiSan(event: any) {
    let taiSan = this.listTaiSan.find(x => x.taiSanId == event.taiSanId);
    this.taiSanModel = taiSan;
  }

  thayDoiNhanVien(event: any) {
    let nhanVien = this.listEmployee.find(x => x.employeeId == event.value.employeeId);
    this.employeeModel = nhanVien;
  }

  refreshForm() {
    this.capPhatTaiSanId = 0;
    this.phanBoTaiSanForm.reset();
    this.employeeModel = new EmployeeModel();
    this.taiSanModel = new TaiSanModel();
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
        Mã Code
        Mã nhân viên
        Tên nhân viên
        Phòng ban
        Vị trí làm việc
        Mã mục đích sử dụng
        Sử dụng từ ngày
        Sử dụng đến ngày
        Lý do
       */
        let asset = new importAssetByExcelModel();
        asset.maTaiSan = _rawData[0] ? _rawData[0].toString().trim() : '';
        asset.tenTaiSan = _rawData[1] ? _rawData[1].toString().trim() : '';
        // asset.maCode = _rawData[2] ? _rawData[2].toString().trim() : '';
        asset.employeeCode = _rawData[2] ? _rawData[2].toString().trim() : '';
        asset.employeeName = _rawData[3] ? _rawData[3].toString().trim() : '';
        // asset.organizationName = _rawData[5] ? _rawData[5].toString().trim() : '';
        // asset.positionName = _rawData[6] ? _rawData[6] : 0;
        asset.maMucDichSuDung = _rawData[4] ? _rawData[4] : null;

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
    let ref = this.dialogService.open(PhanBoImportDetailComponent, {
      data: {
        listAssetImport: listAssetImport
      },
      header: 'Nhập excel danh sách tài sản',
      width: '85%',
      baseZIndex: 1050,
      contentStyle: {
        "max-height": "800px",
      }
    });
    ref.onClose.subscribe((result: any) => {
      if (result?.status) {
        //   this.getListAssetAndEmployee();
      }
    });
    // this.getListAssetAndEmployee();
  }

  //lấy danh sách tài sản - nhân viên
  // getListAssetAndEmployee() {

  //   this.loading = true
  //   this.assetService.getListAssetAndEmployee().subscribe(response => {
  //
  //     this.loading = false
  //   }, error => { });
  // }

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
    let result: any = await this.assetService.downloadTemplateAsset(1);
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

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }
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
