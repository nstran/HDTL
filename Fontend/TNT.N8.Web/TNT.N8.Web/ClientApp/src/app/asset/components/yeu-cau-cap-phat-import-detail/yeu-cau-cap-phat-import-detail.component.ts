import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormArray, FormBuilder, Validators } from '@angular/forms'
//SERVICES
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { AssetService } from '../../services/asset.service';
import { DatePipe } from '@angular/common';
import { YeuCauCapPhatChiTietModel } from '../../models/yeu-cau-cap-phat-chi-tiet';


interface ResultDialog {
  status: boolean,
  listAsset: Array<importAssetByExcelModel>;
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

interface importAssetByExcelModel {
  maLoaiTaiSan: string;
  tenLoaiTaiSan: string;
  moTa: string;
  soLuong: string;
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
  selector: 'app-yeu-cau-cap-phat-import-detail',
  templateUrl: './yeu-cau-cap-phat-import-detail.component.html',
  styleUrls: ['./yeu-cau-cap-phat-import-detail.component.css']
})
export class YeuCauCapPhatImportDetailComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;

  listNote: Array<Note> = [
    /* required fields */
    { code: "required_mataisan", name: "Nhập mã loại tài sản" },
    { code: "required_soluong", name: "Nhập mã loại tài sản" },
    { code: "required_manhanvien", name: "Nhập mã nhân viên" },
    { code: "required_mamucdich", name: "Nhập mã mục đích" },
    { code: "required_ngaybatdau", name: "Nhập sử dụng từ ngày" },
    { code: "error_mamucdich", name: "Mã mục đích sai" },
    { code: "error_manhanvien", name: "Mã nhân viên không tồn tại" },
    { code: "error_soluong", name: "Số lượng sai định dạng" },
    { code: "error_ngay", name: "Sai định dạng ngày" },
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
  listLoaiTS: Array<any> = [];
  listMucDichSuDung: Array<CategoryModel> = [];

  listYeuCauCP: Array<YeuCauCapPhatChiTietModel> = new Array<YeuCauCapPhatChiTietModel>();

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

      let loaiTS = this.listLoaiTS.find(x => x.categoryCode == item.maLoaiTaiSan);
      if (loaiTS != undefined) {
        item.maLoaiTaiSan = loaiTS.categoryCode;
        item.tenLoaiTaiSan = loaiTS.categoryName;
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
      { field: 'maLoaiTaiSan', header: 'Mã loại tài sản', textAlign: 'center', display: 'table-cell', width: '100px' },
      { field: 'tenLoaiTaiSan', header: 'Tên loại tài sản', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'moTa', header: 'Mô tả', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'soLuong', header: 'Số lượng', textAlign: 'center', display: 'table-cell', width: '100px' },
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
        this.listMucDichSuDung = result.listMucDichSuDung;
        this.listEmployee = result.listEmployee;
        this.listLoaiTS = result.listLoaiTSPB;

        this.getDataFromAssetImportComponent();
        this.checkStatus(true);
      }
    });
  }

  checkStatus(autoAdd: boolean) {

    let listAssetCode = []
    let listEmployeeCode = []
    let listMucDichSuDungCode = []

    this.listLoaiTS.forEach(e => { listAssetCode.push(e.maTaiSan) })
    this.listEmployee.forEach(e => { listEmployeeCode.push(e.employeeCode) })
    this.listMucDichSuDung.forEach(e => { listMucDichSuDungCode.push(e.categoryCode) })

    this.listAssetImport.forEach(asset => {
      asset.listStatus = [];
      asset.isValid = true;

      /* required fields */
      if (!asset.maLoaiTaiSan ?.trim()) {
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

      if (!asset.soLuong.match('^[0-9]*(\.[0-9])?$')) {
        asset.listStatus = [...asset.listStatus, this.listNote.find(e => e.code == "error_soluong")];
        asset.isValid = false;
      }

    });

    /* auto add to valid list */
    if (autoAdd) this.selectedAssetImport = this.listAssetImport.filter(e => e.isValid);
  }

  onCancel() {
    let result: ResultDialog = {
      status: false,
      listAsset: []
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

    this.listYeuCauCP = this.mapFormToPhanBoTaiSanModel();
    this.loading = true;
    let resultImport: ResultDialog = {
      status: true,
      listAsset: this.selectedAssetImport.filter(e => e.isValid == true)
    };
    this.ref.close(resultImport);
  }

  mapFormToPhanBoTaiSanModel(): Array<YeuCauCapPhatChiTietModel> {
    let lstPhanBo = new Array<YeuCauCapPhatChiTietModel>();

    this.selectedAssetImport.forEach(taiSanImport => {
      let yeuCauTSModel = new YeuCauCapPhatChiTietModel();

      yeuCauTSModel.yeuCauCapPhatTaiSanChiTietId = 0;
      // Loại tài sản
      let loaiTS = this.listLoaiTS.find(x => x.categoryId == taiSanImport.maLoaiTaiSan);

      if (loaiTS != null && loaiTS != undefined) {
        yeuCauTSModel.loaiTaiSan = loaiTS.categoryCode;
      }

      // Nhân viên
      let nhanVien = this.listEmployee.find(x => x.employeeCode == taiSanImport.employeeCode);
      if (nhanVien != null && nhanVien != undefined) {
        yeuCauTSModel.employeeName = nhanVien.employeeName;
        yeuCauTSModel.organizationName = nhanVien.organizationName;
        yeuCauTSModel.positionName = nhanVien.positionName;
      }
      // Mục đích sử dụng
      let mucDichSD = this.listMucDichSuDung.find(x => x.categoryCode == taiSanImport.maMucDichSuDung)

      if (mucDichSD != null && mucDichSD != undefined) {
        yeuCauTSModel.mucDichSuDungId = mucDichSD ? mucDichSD.categoryId : this.emptyGuid;
        yeuCauTSModel.mucDichSuDung = mucDichSD.categoryName;
      }

      var datePipe = new DatePipe("en-US");
      // Từ ngày
      let ngayBatDau = convertToUTCTime(new Date(taiSanImport.ngayBatDau));
      yeuCauTSModel.ngayBatDau = ngayBatDau;

      // Đến ngày
      let ngayKetThuc = convertToUTCTime(new Date(taiSanImport.ngayKetThuc));
      yeuCauTSModel.ngayKetThuc = ngayKetThuc;

      // Lý do
      yeuCauTSModel.lyDo = taiSanImport.lyDo;

      yeuCauTSModel.createdById = this.auth.UserId;
      yeuCauTSModel.createdDate = convertToUTCTime(new Date());
      yeuCauTSModel.updatedById = null;
      yeuCauTSModel.updatedDate = null;
      lstPhanBo.push(yeuCauTSModel);
    });

    return lstPhanBo;
  }

  standardizedListAsset() {
    this.listAssetImport.forEach(asset => {
      asset.maLoaiTaiSan = asset.maLoaiTaiSan ?.trim() ?? "";
      asset.tenLoaiTaiSan = asset.tenLoaiTaiSan ?.trim() ?? "";
      asset.moTa = asset.moTa ?.trim() ?? "";
      asset.soLuong = asset.soLuong ?.trim() ?? "";
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

