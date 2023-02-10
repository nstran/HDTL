import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormArray, FormBuilder, Validators } from '@angular/forms'
//SERVICES
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { AssetService } from '../../services/asset.service';
import { PhanBoTaiSanModel } from '../../models/phanbotaisan.model';
import { DatePipe } from '@angular/common';


interface ResultDialog {
  status: boolean,
  statusImport: boolean
}

class Note {
  public code: string;
  public name: string;
}
class CategoryModel {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}

class importAssetByExcelModel {
  maTaiSan: string;
  tenTaiSan: string;
  maCode: string;
  employeeCode: string;
  employeeName: string;
  maMucDichSuDung: string;
  mucDichSuDung: string;
  organizationName: string;
  positionName: string;
  ngayBatDau: Date;
  ngayKetThuc: Date;
  lyDo: string;

  listStatus: Array<Note>;
  isValid: boolean;
}

@Component({
  selector: 'app-phan-bo-import-detail',
  templateUrl: './phan-bo-import-detail.component.html',
  styleUrls: ['./phan-bo-import-detail.component.css']
})
export class PhanBoImportDetailComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;

  listNote: Array<Note> = [
    /* required fields */
    { code: "required_mataisan", name: "Nhập mã mã tài sản" },
    { code: "required_manhanvien", name: "Nhập mã nhân viên" },
    { code: "required_mamucdich", name: "Nhập mã mục đích" },
    { code: "required_ngaybatdau", name: "Nhập sử dụng từ ngày" },
    { code: "error_mamucdich", name: "Mã mục đích sai" },
    { code: "error_manhanvien", name: "Mã nhân viên không tồn tại" },
    { code: "error_mataisan", name: "Mã tài sản không tồn tại" },
  ]

  //dialog data
  listAssetImport: Array<importAssetByExcelModel> = [];

  //table
  rows: number = 10;
  columns: Array<any> = [];
  selectedColumns: Array<any> = [];
  selectedAssetImport: Array<importAssetByExcelModel> = [];
  customerForm: FormGroup;
  //master data
  listEmployee: Array<any> = [];
  listTaiSan: Array<any> = [];
  listMucDichSuDung: Array<CategoryModel> = [];

  listTaiSanPB: Array<PhanBoTaiSanModel> = new Array<PhanBoTaiSanModel>();

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private formBuilder: FormBuilder,
    private assetService: AssetService,
  ) {

  }

  async ngOnInit() {
    this.initTable();
    await this.getMasterdata();

  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  getDataFromAssetImportComponent() {
    this.listAssetImport = this.config.data.listAssetImport;
    //mapping các giá trị nếu từ excel
    this.listAssetImport.forEach(item => {
      let batDau = new Date(item.ngayBatDau)
      if (item.ngayBatDau != null) item.ngayBatDau = batDau;

      if (item.ngayKetThuc != undefined) {
        let ketThuc = new Date(item.ngayKetThuc)
        if (item.ngayKetThuc != null) item.ngayKetThuc = ketThuc;
      }

      let taisan = this.listTaiSan.find(x => x.maTaiSan == item.maTaiSan);
      if (taisan != undefined) {
        item.maCode = taisan.maCode;
      }

      let nhanvien = this.listEmployee.find(x => x.employeeCode == item.employeeCode);
      if (nhanvien != undefined) {
        item.organizationName = nhanvien.organizationName;
        item.positionName = nhanvien.positionName;
      }

      let mucdich = this.listMucDichSuDung.find(x => x.categoryCode == item.maMucDichSuDung);
      if (mucdich != undefined)
        item.mucDichSuDung = mucdich.categoryName;
    })
  }

  initTable() {
    this.columns = [
      { field: 'maTaiSan', header: 'Mã tài sản', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'tenTaiSan', header: 'Tên tài sản', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'maCode', header: ' Mã Code', textAlign: 'center', display: 'table-cell', width: '120px' },
      { field: 'employeeCode', header: 'Mã nhân viên', textAlign: 'center', display: 'table-cell', width: '100px' },
      { field: 'employeeName', header: 'Tên nhân viên', textAlign: 'center', display: 'table-cell', width: '100px' },
      { field: 'organizationName', header: 'Phòng ban', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'positionName', header: 'Vị trí làm việc', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'mucDichSuDung', header: 'Mục đích sử dụng', textAlign: 'center', display: 'table-cell', width: '120px' },
      { field: 'ngayBatDau', header: 'Sử dụng từ ngày', textAlign: 'center', display: 'table-cell', width: '100px' },
      { field: 'ngayKetThuc', header: 'Sử dụng đến ngày', textAlign: 'center', display: 'table-cell', width: '100px' },
      { field: 'lyDo', header: 'Lý do', textAlign: 'center', display: 'table-cell', width: '200px' },
      { field: 'listStatus', header: 'Ghi chú', textAlign: 'left', display: 'table-cell', width: '200px' },
    ];
  }

  async getMasterdata() {
    this.loading = true;
    this.assetService.getMasterDataPhanBoTSForm().subscribe(response => {
      let result: any = response;

      this.loading = false;
      if (result.statusCode == 200) {
        this.listTaiSan = result.listTaiSan;
        this.listMucDichSuDung = result.listMucDichSuDung;
        this.listEmployee = result.listEmployee;

        this.getDataFromAssetImportComponent();
        this.checkStatus(true);
      }
    });
  }

  checkStatus(autoAdd: boolean) {

    let listAssetCode = []
    let listEmployeeCode = []
    let listMucDichSuDungCode = []

    this.listTaiSan.forEach(e => { listAssetCode.push(e.maTaiSan) })
    this.listEmployee.forEach(e => { listEmployeeCode.push(e.employeeCode) })
    this.listMucDichSuDung.forEach(e => { listMucDichSuDungCode.push(e.categoryCode) })

    this.listAssetImport.forEach(asset => {
      asset.listStatus = [];
      asset.isValid = true;
      /* required fields */
      if (!asset.maTaiSan ?.trim()) {
        asset.listStatus = [...asset.listStatus, this.listNote.find(e => e.code == "required_mataisan")];
        asset.isValid = false;
      }

      if (!asset.employeeCode ?.trim()) {
        asset.listStatus = [...asset.listStatus, this.listNote.find(e => e.code == "required_manhanvien")];
        asset.isValid = false;
      }

      if (!asset.maMucDichSuDung ?.trim()) {
        asset.listStatus = [...asset.listStatus, this.listNote.find(e => e.code == "required_mamucdich")];
        asset.isValid = false;
      }

      if (asset.ngayBatDau == null) {
        asset.listStatus = [...asset.listStatus, this.listNote.find(e => e.code == "required_ngaybatdau")];
        asset.isValid = false;
      }

      let existAssetCode = listAssetCode.find(e => (e ?.trim() ?.toLowerCase() === asset.maTaiSan ?.trim() ?.toLowerCase()));
      if (!existAssetCode) {
        asset.listStatus = [...asset.listStatus, this.listNote.find(e => e.code == "error_mataisan")];
        asset.isValid = false;
      }

      let existEmpCode = listEmployeeCode.find(e => (e ?.trim() ?.toLowerCase() === asset.employeeCode ?.trim() ?.toLowerCase()));
      if (!existEmpCode) {
        asset.listStatus = [...asset.listStatus, this.listNote.find(e => e.code == "error_manhanvien")];
        asset.isValid = false;
      }

      let existMucDichCode = listMucDichSuDungCode.find(e => (e ?.trim() ?.toLowerCase() === asset.maMucDichSuDung ?.trim() ?.toLowerCase()));
      if (!existMucDichCode) {
        asset.listStatus = [...asset.listStatus, this.listNote.find(e => e.code == "error_mamucdich")];
        asset.isValid = false;
      }
    });

    /* auto add to valid list */
    if (autoAdd) this.selectedAssetImport = this.listAssetImport.filter(e => e.isValid);
  }

  onCancel() {
    let result: ResultDialog = {
      status: false,
      statusImport: false
    };
    this.ref.close(result);
  }

  async importAsset() {
    /* check valid list selected */
    if (this.selectedAssetImport.length == 0) {
      let msg = { severity: 'warn', summary: 'Thông báo', detail: 'Chọn danh sách cần import' };
      this.showMessage(msg);
      return;
    }

    let inValidRecord = this.selectedAssetImport.find(e => !e.isValid);
    if (inValidRecord) {
      let msg = { severity: 'error', summary: 'Thông báo', detail: 'Danh sách không hợp lệ' };
      this.showMessage(msg);
      return;
    }
    this.checkStatus(false);
    this.standardizedListAsset();

    this.listTaiSanPB = this.mapFormToPhanBoTaiSanModel();
    this.loading = true;
    this.assetService.taoPhanBoTaiSan(this.listTaiSanPB, this.auth.UserId).subscribe(response => {
      this.loading = false;
      let result = <any>response;
      if (result.statusCode === 200) {
        let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Nhập excel thành công' };
        this.showMessage(mgs);
        let result: ResultDialog = {
          status: true,
          statusImport: true
        };
        this.ref.close(result);
      } else {
        let mgs = { severity: 'error', summary: 'Thông báo', detail: 'Nhập excel thất bại' };
        this.showMessage(mgs);
        let result: ResultDialog = {
          status: false,
          statusImport: false
        };
        this.ref.close(result);
      }
    });
  }

  mapFormToPhanBoTaiSanModel(): Array<PhanBoTaiSanModel> {
    let lstPhanBo = new Array<PhanBoTaiSanModel>();

    this.selectedAssetImport.forEach(taiSanImport => {
      let phanBoTSModel = new PhanBoTaiSanModel();

      phanBoTSModel.capPhatTaiSanId = 0;
      // Tài sản
      let taisan = this.listTaiSan.find(x => x.maTaiSan == taiSanImport.maTaiSan);

      if (taisan != null && taisan != undefined) {
        phanBoTSModel.taiSanId = taisan ? taisan.taiSanId : 0;
        phanBoTSModel.tenTaiSan = taisan.tenTaiSan;
      }

      // Nhân viên
      let nhanVien = this.listEmployee.find(x => x.employeeCode == taiSanImport.employeeCode);
      phanBoTSModel.nguoiSuDungId = nhanVien ? nhanVien.employeeId : this.emptyGuid;
      phanBoTSModel.employeeName = nhanVien.employeeName;
      phanBoTSModel.organizationName = nhanVien.organizationName;
      phanBoTSModel.positionName = nhanVien.positionName;

      // Mục đích sử dụng
      let mucDichSD = this.listMucDichSuDung.find(x => x.categoryCode == taiSanImport.maMucDichSuDung)

      if (mucDichSD != null && mucDichSD != undefined) {
        phanBoTSModel.mucDichSuDungId = mucDichSD ? mucDichSD.categoryId : this.emptyGuid;
        phanBoTSModel.mucDichSuDung = mucDichSD.categoryName;
      }

      var datePipe = new DatePipe("en-US");
      // Từ ngày
      let ngayBatDau = convertToUTCTime(new Date(taiSanImport.ngayBatDau));
      phanBoTSModel.ngayBatDau = ngayBatDau;

      // Đến ngày
      let ngayKetThuc = convertToUTCTime(new Date(taiSanImport.ngayKetThuc));
      phanBoTSModel.ngayKetThuc = ngayKetThuc;

      // Lý do
      phanBoTSModel.lyDo = taiSanImport.lyDo;

      phanBoTSModel.nguoiCapPhatId = this.auth.UserId;
      phanBoTSModel.createdById = this.auth.UserId;
      phanBoTSModel.createdDate = convertToUTCTime(new Date());
      phanBoTSModel.updatedById = null;
      phanBoTSModel.updatedDate = null;
      lstPhanBo.push(phanBoTSModel);
    });

    return lstPhanBo;
  }
  standardizedListAsset() {
    this.listAssetImport.forEach(asset => {
      asset.maTaiSan = asset.maTaiSan ?.trim() ?? "";
      asset.tenTaiSan = asset.tenTaiSan ?.trim() ?? "";
      asset.maCode = asset.maCode ?.trim() ?? "";
      asset.employeeCode = asset.employeeCode ?.trim() ?? "";
      asset.employeeName = asset.employeeName ?.trim() ?? "";
      asset.mucDichSuDung = asset.mucDichSuDung ?.trim() ?? "";
      asset.maMucDichSuDung = asset.maMucDichSuDung ?.trim() ?? "";
      asset.organizationName = asset.organizationName ?.trim() ?? "";
      asset.positionName = asset.positionName ?.trim() ?? "";
      asset.ngayBatDau = asset.ngayBatDau;
      asset.ngayKetThuc = asset.ngayKetThuc;
      asset.lyDo = asset.lyDo ?.trim() ?? "";

    });
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

