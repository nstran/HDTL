import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms'
//SERVICES
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { AssetService } from '../../services/asset.service';
import { DatePipe } from '@angular/common';
import { ThuHoiTaiSanModel } from '../../models/thu-hoi-tai-san.model';


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
  employeeCode: string;
  employeeName: string;
  organizationName: string;
  positionName: string;
  ngayBatDau: Date;
  lyDo: string;

  listStatus: Array<Note>;
  isValid: boolean;
}

@Component({
  selector: 'app-thu-hoi-import-detail',
  templateUrl: './thu-hoi-import-detail.component.html',
  styleUrls: ['./thu-hoi-import-detail.component.css']
})
export class ThuHoiImportDetailComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;

  listNote: Array<Note> = [
    /* required fields */
    { code: "required_mataisan", name: "Nhập mã mã tài sản" },
    { code: "required_ngaybatdau", name: "Nhập ngày thu hồi" },
    { code: "error_mataisan", name: "Mã tài sản không được phân bổ" },
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

  listTaiSanTHU: Array<ThuHoiTaiSanModel> = new Array<ThuHoiTaiSanModel>();

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private messageService: MessageService,
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

      let taiSan = this.listTaiSan.find(x => x.maTaiSan == item.maTaiSan);
      let ngayThuHoi = new Date(item.ngayBatDau)
      if (item.ngayBatDau != null) item.ngayBatDau = ngayThuHoi;
      if (taiSan != undefined) {
        item.employeeCode = taiSan.maNV;
        item.employeeName = taiSan.hoVaTen;
        item.positionName = taiSan.viTriLamViec;
      }
    });
  }

  initTable() {
    this.columns = [
      { field: 'maTaiSan', header: 'Mã tài sản', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'tenTaiSan', header: 'Tên tài sản', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'employeeCode', header: 'Mã nhân viên', textAlign: 'center', display: 'table-cell', width: '100px' },
      { field: 'employeeName', header: 'Tên nhân viên', textAlign: 'center', display: 'table-cell', width: '100px' },
      { field: 'positionName', header: 'Vị trí làm việc', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'ngayBatDau', header: 'Ngày thu hồi', textAlign: 'center', display: 'table-cell', width: '100px' },
      { field: 'lyDo', header: 'Lý do', textAlign: 'center', display: 'table-cell', width: '200px' },
      { field: 'listStatus', header: 'Ghi chú', textAlign: 'left', display: 'table-cell', width: '200px' },
    ];
  }

  async getMasterdata() {
    this.loading = true;
    this.assetService.getMasterDataThuHoiTSForm().subscribe(response => {
      let result: any = response;

      this.loading = false;
      if (result.statusCode == 200) {
        this.listTaiSan = result.listTaiSan;

        this.getDataFromAssetImportComponent();
        this.checkStatus(true);
      }
    });
  }

  checkStatus(autoAdd: boolean) {
    let listAssetCode = []
    let listEmployeeCode = []

    this.listTaiSan.forEach(e => { listAssetCode.push(e.maTaiSan) })

    this.listAssetImport.forEach(asset => {
      asset.listStatus = [];
      asset.isValid = true;
      /* required fields */
      if (!asset.maTaiSan?.trim()) {
        asset.listStatus = [...asset.listStatus, this.listNote.find(e => e.code == "required_mataisan")];
        asset.isValid = false;
      }

      if (asset.ngayBatDau == null) {
        asset.listStatus = [...asset.listStatus, this.listNote.find(e => e.code == "required_ngaybatdau")];
        asset.isValid = false;
      }

      let existAssetCode = listAssetCode.find(e => (e?.trim()?.toLowerCase() === asset.maTaiSan?.trim()?.toLowerCase()));
      if (!existAssetCode) {
        asset.listStatus = [...asset.listStatus, this.listNote.find(e => e.code == "error_mataisan")];
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

    this.listTaiSanTHU = this.mapFormToPhanBoTaiSanModel();
    this.loading = true;

    this.assetService.taoThuHoiTaiSan(this.listTaiSanTHU, this.auth.UserId).subscribe(response => {
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

  mapFormToPhanBoTaiSanModel(): Array<ThuHoiTaiSanModel> {
    let lstThuHoi = new Array<ThuHoiTaiSanModel>();

    this.selectedAssetImport.forEach(taiSanImport => {
      let thuHoiTSModel = new ThuHoiTaiSanModel();

      thuHoiTSModel.capPhatTaiSanId = 0;
      // Tài sản
      let taisan = this.listTaiSan.find(x => x.maTaiSan == taiSanImport.maTaiSan);

      if (taisan != null && taisan != undefined) {
        thuHoiTSModel.taiSanId = taisan ? taisan.taiSanId : 0;
        thuHoiTSModel.tenTaiSan = taisan.tenTaiSan;
      }

      // Ngày thu hồi
      let ngayBatDau = convertToUTCTime(new Date(taiSanImport.ngayBatDau));
      thuHoiTSModel.ngayBatDau = ngayBatDau;

      // Lý do
      thuHoiTSModel.lyDo = taiSanImport.lyDo;
      thuHoiTSModel.loaiCapPhat = 0;

      thuHoiTSModel.nguoiCapPhatId = this.auth.UserId;
      thuHoiTSModel.createdById = this.auth.UserId;
      thuHoiTSModel.createdDate = convertToUTCTime(new Date());
      thuHoiTSModel.updatedById = null;
      thuHoiTSModel.updatedDate = null;
      lstThuHoi.push(thuHoiTSModel);
    });

    return lstThuHoi;
  }
  standardizedListAsset() {
    this.listAssetImport.forEach(asset => {
      asset.maTaiSan = asset.maTaiSan?.trim() ?? "";
      asset.tenTaiSan = asset.tenTaiSan?.trim() ?? "";
      asset.ngayBatDau = asset.ngayBatDau;
      asset.lyDo = asset.lyDo?.trim() ?? "";

    });
  }


  //end
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

