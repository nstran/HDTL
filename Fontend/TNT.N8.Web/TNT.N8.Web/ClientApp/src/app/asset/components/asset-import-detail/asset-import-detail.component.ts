import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, FormArray, FormBuilder, Validators } from '@angular/forms'

//SERVICES
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { AssetService } from '../../services/asset.service';
//MODELS
import { TaiSanModel } from '../../models/taisan.model';



interface ResultDialog {
  status: boolean,
  statusImport: boolean
}

class Note {
  public code: string;
  public name: string;
}

class importTaiSanModel {
  maTaiSan: string;
  tenTaiSan: string;
  phanLoaiTaiSanId: string;
  donViTinhId: string;
  khuVucTaiSanId: string;
  ngayVaoSo: Date;
  moTa: string;
  soSerial: string;
  model: string;
  soHieu: string;
  thongTinNoiMua: string;
  namSx: number;
  nuocSxid: string;
  hangSxid: string;
  ngayMua: Date;
  thoiHanBaoHanh: number;
  baoDuongDinhKy: number;
  thongTinNoiBaoHanh: string;
  giaTriNguyenGia: number;
  giaTriTinhKhauHao: number;
  thoiGianKhauHao: number;
  thoiDiemBdtinhKhauHao: Date;

  nuocSxCode: string;
  hangSxCode: string;
  phanLoaiTaiSanCode: string;
  donViTinhCode: string;
  khuVucTaiSanCode: string;

  listStatus: Array<Note>;
  isValid: boolean;
}


@Component({
  selector: 'app-asset-import-detail',
  templateUrl: './asset-import-detail.component.html',
  styleUrls: ['./asset-import-detail.component.css']
})
export class AssetImportDetailComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;
  @ViewChild('myTable') myTable: Table;

  today: Date = new Date();

  listNote: Array<Note> = [
    /* required fields */
    { code: "required_code", name: "Nhập mã tài sản" },
    { code: "required_name", name: "Nhập tên tài sản" },
    { code: "required_phanLoai", name: "Nhập phân loại tài sản" },
    { code: "required_donVi", name: "Nhập đơn vị tính tài sản" },
    { code: "required_khuVuc", name: "Nhập khu vực" },
    { code: "required_ngayVaoSo", name: "Nhập ngày vào sổ" },
    { code: "required_soSerial", name: "Nhập số serial" },
    { code: "required_model", name: "Nhập model" },
    { code: "required_giaTriTinhKhauHao", name: "Nhập giá trị tính khấu hao" },
    { code: "required_thoiGianKhauHao", name: "Nhập thời gian khấu hao" },
    { code: "required_thoiDiemBdTinhKhauHao", name: "Nhập thời điểm bắt đầu tính khấu hao" },

    // check mã code
    { code: "wrong_KhuVucCode", name: "Mã khu vực không tồn tại" },
    { code: "wrong_phanLoaiTsCode", name: "Mã phân loại tài sản không tồn tại" },
    { code: "wrong_donViTinhCode", name: "Mã đơn vị tính không tồn tại" },
    { code: "wrong_nuocSx", name: "Mã nước sản xuất không tồn tại" },
    { code: "wrong_hangSxCode", name: "Mã hãng sản xuất không tồn tại" },


    //Check mã tài sản trong DB
    { code: "exist_inDB", name: "Mã tài sản đã tồn tại trên hệ thống" },

    //Check số lơn hơn 0
    { code: "thoiGianKhauHao_positive", name: "Thời gian khấu hao phải lơn hơn 0" },
    { code: "baoDuongDinhKy_positive", name: "Bảo dưỡng định kỳ phải lơn hơn 0" },
    { code: "namSx_positive", name: "Năm sản xuất phải lơn hơn 0" },
    { code: "namSx_mustBeInt", name: "Năm sản xuất phải là số nguyên dương" },
    { code: "thoiHanBaoHanh_positive", name: "Thời hạn bảo hành phải lơn hơn 0" },
  ]

  listTaiSanImport: Array<importTaiSanModel> = [];

  //table
  rows: number = 10;
  columns: Array<any> = [];
  selectedColumns: Array<any> = [];
  selectedTaiSanImport: Array<importTaiSanModel> = [];

  listPhanLoaiTS: Array<any> = [];
  listDonVi: Array<any> = [];
  listNuocSX: Array<any> = [];
  listHangSX: Array<any> = [];
  listKhuVuc: Array<any> = [];
  listMaTaiSan: Array<any> = [];
  listPhuongPhapKhauHao = [
    {
      id: 1, name: 'Khấu hao đường thẳng'
    }];

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private messageService: MessageService,
    private assetService: AssetService,
  ) {
    if (this.config.data) {
      this.listTaiSanImport = this.config.data.listTaiSanImport;
      console.log('sangcompotnent mới', this.listTaiSanImport)
    }
  }

  async ngOnInit() {
    this.initTable();
    await this.getMasterdata();
    this.checkStatus(true);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  initTable() {
    this.columns = [
      { field: 'maTaiSan', header: 'Mã tài sản', textAlign: 'left', display: 'table-cell', width: '150px', type: 'text', isRequired: true },
      { field: 'tenTaiSan', header: 'Tên tài sản', textAlign: 'left', display: 'table-cell', width: '150px', type: 'text', isRequired: true },
      { field: 'phanLoaiTaiSanCode', header: 'Phân loại tài sản', textAlign: 'center', display: 'table-cell', width: '120px', type: 'text', isRequired: true },
      { field: 'donViTinhCode', header: 'Đơn vị tính', textAlign: 'center', display: 'table-cell', width: '100px', type: 'text', isRequired: true },
      { field: 'khuVucTaiSanCode', header: 'Khu vực', textAlign: 'center', display: 'table-cell', width: '100px', type: 'text', isRequired: true },
      { field: 'ngayVaoSo', header: 'Ngày vào sổ', textAlign: 'center', display: 'table-cell', width: '150px', type: 'date', isRequired: true },
      { field: 'moTa', header: 'Mô tả', textAlign: 'left', display: 'table-cell', width: '150px', type: 'text', isRequired: false },
      { field: 'soSerial', header: 'Số serial', textAlign: 'left', display: 'table-cell', width: '120px', type: 'text', isRequired: true },
      { field: 'model', header: 'Model', textAlign: 'left', display: 'table-cell', width: '100px', type: 'text', isRequired: true },
      { field: 'soHieu', header: 'Số hiệu', textAlign: 'left', display: 'table-cell', width: '100px', type: 'text', isRequired: false },
      { field: 'thongTinNoiMua', header: 'Thông tin nơi mua', textAlign: 'left', display: 'table-cell', width: '200px', type: 'text', isRequired: false },
      { field: 'namSx', header: 'Năm sản xuất', textAlign: 'center', display: 'table-cell', width: '100px', type: 'numberInt', isRequired: false },
      { field: 'nuocSxCode', header: 'Nước sản xuất', textAlign: 'center', display: 'table-cell', width: '100px', type: 'text', isRequired: false },
      { field: 'hangSxCode', header: 'Hãng sản xuất', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: false },
      { field: 'ngayMua', header: 'Ngày mua', textAlign: 'center', display: 'table-cell', width: '150px', type: 'date', isRequired: false },
      { field: 'thoiHanBaoHanh', header: 'Thời hạn bảo hành', textAlign: 'center', display: 'table-cell', width: '120px', type: 'number', isRequired: false },
      { field: 'baoDuongDinhKy', header: 'Bảo dưỡng định kỳ', textAlign: 'center', display: 'table-cell', width: '100px', type: 'number', isRequired: false },
      { field: 'thongTinNoiBaoHanh', header: 'Thông tin nơi bảo hành', textAlign: 'left', display: 'table-cell', width: '150px', type: 'text', isRequired: false },
      { field: 'giaTriNguyenGia', header: 'Giá trị nguyên giá', textAlign: 'center', display: 'table-cell', width: '150px', type: 'number', isRequired: false },
      { field: 'giaTriTinhKhauHao', header: 'Giá trị tính khấu hao', textAlign: 'center', display: 'table-cell', width: '150px', type: 'number', isRequired: true },
      { field: 'thoiGianKhauHao', header: 'Thời gian khấu hao', textAlign: 'center', display: 'table-cell', width: '150px', type: 'number', isRequired: true },
      { field: 'thoiDiemBdtinhKhauHao', header: 'Thời điểm bắt đầu tính khấu hao', textAlign: 'center', display: 'table-cell', width: '150px', type: 'dateMonth', isRequired: true },
      { field: 'listStatus', header: 'Trạng thái', textAlign: 'left', display: 'table-cell', width: '250px', type: 'listStatus' },
    ];
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.assetService.getMasterDataAssetForm();
    this.loading = false;
    if (result.statusCode == 200) {
      console.log(result);
      this.listPhanLoaiTS = result.listPhanLoaiTS;
      this.listDonVi = result.listDonVi;
      this.listNuocSX = result.listNuocSX;
      this.listHangSX = result.listHangSX;
      this.listKhuVuc = result.listProvinceModel;
      this.listMaTaiSan = result.listMaTS;
    }
  }

  checkStatus(autoAdd: boolean) {
    this.listTaiSanImport.forEach(taiSan => {
      taiSan.listStatus = [];
      taiSan.isValid = true;

      /* required fields */
      if (!taiSan.maTaiSan?.trim()) {
        taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "required_code")];
        taiSan.isValid = false;
      }

      //Check mã tồn tại trong db
      if (taiSan.maTaiSan) {
        if (this.listMaTaiSan.indexOf(taiSan.maTaiSan.toUpperCase().trim()) != -1) {
          taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "exist_inDB")];
          taiSan.isValid = false;
        }
      }

      if (!taiSan.tenTaiSan?.trim()) {
        taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "required_name")];
        taiSan.isValid = false;
      }

      if (!taiSan.phanLoaiTaiSanCode?.trim()) {
        taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "required_phanLoai")];
        taiSan.isValid = false;
      }

      if (!taiSan.donViTinhCode?.trim()) {
        taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "required_donVi")];
        taiSan.isValid = false;
      }

      if (!taiSan.khuVucTaiSanCode?.trim()) {
        taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "required_khuVuc")];
        taiSan.isValid = false;
      }

      if (!taiSan.ngayVaoSo) {
        taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "required_ngayVaoSo")];
        taiSan.isValid = false;
      }

      if (!taiSan.soSerial) {
        taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "required_soSerial")];
        taiSan.isValid = false;
      }

      if (!taiSan.model) {
        taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "required_model")];
        taiSan.isValid = false;
      }

      if (!taiSan.giaTriTinhKhauHao) {
        taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "required_giaTriTinhKhauHao")];
        taiSan.isValid = false;
      }

      if (!taiSan.thoiGianKhauHao) {
        taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "required_thoiGianKhauHao")];
        taiSan.isValid = false;
      }

      if (!taiSan.thoiDiemBdtinhKhauHao) {
        taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "required_thoiDiemBdTinhKhauHao")];
        taiSan.isValid = false;
      }
      //check code

      //Đơn vị
      if (taiSan.donViTinhCode) {
        let donVi = this.listDonVi.find(x => x.categoryCode.toLowerCase().trim() == taiSan.donViTinhCode.toLowerCase().trim());
        if (!donVi) {
          taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "wrong_donViTinhCode")];
          taiSan.isValid = false;
        } else {
          taiSan.donViTinhId = donVi.categoryId;
        }
      }

      //Khu vực
      if (taiSan.khuVucTaiSanCode) {
        let khuVuc = this.listKhuVuc.find(x => x.provinceCode.toLowerCase().trim() == taiSan.khuVucTaiSanCode.toLowerCase().trim());
        if (!khuVuc) {
          taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "wrong_KhuVucCode")];
          taiSan.isValid = false;
        } else {
          taiSan.khuVucTaiSanId = khuVuc.provinceId;
        }
      }

      //Nước sản xuất
      if (taiSan.nuocSxCode) {
        let nuocSx = this.listNuocSX.find(x => x.categoryCode.toLowerCase().trim() == taiSan.nuocSxCode.toLowerCase().trim());
        if (!nuocSx) {
          taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "wrong_nuocSx")];
          taiSan.isValid = false;
        } else {
          taiSan.nuocSxid = nuocSx.categoryId;
        }
      }

      //Hãng sản xuất
      if (taiSan.hangSxCode) {
        let hangSx = this.listHangSX.find(x => x.categoryCode.toLowerCase().trim() == taiSan.hangSxCode.toLowerCase().trim());
        if (!hangSx) {
          taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "wrong_hangSxCode")];
          taiSan.isValid = false;
        } else {
          taiSan.hangSxid = hangSx.categoryId;
        }
      }

      //Phân loại tài sản
      if (taiSan.phanLoaiTaiSanCode) {
        let loaiTs = this.listPhanLoaiTS.find(x => x.categoryCode.toLowerCase().trim() == taiSan.phanLoaiTaiSanCode.toLowerCase().trim());
        if (!loaiTs) {
          taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "wrong_phanLoaiTsCode")];
          taiSan.isValid = false;
        } else {
          taiSan.phanLoaiTaiSanId = loaiTs.categoryId;
        }
      }

      //Số phải lớn hơn 0
      if (taiSan.thoiGianKhauHao) {
        if (Number(taiSan.thoiGianKhauHao) != NaN) {
          if (Number(taiSan.thoiGianKhauHao) < 0) {
            taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "thoiGianKhauHao_positive")];
            taiSan.isValid = false;
          }
        }
      }

      if (taiSan.baoDuongDinhKy) {
        if (Number(taiSan.baoDuongDinhKy) != NaN) {
          if (Number(taiSan.baoDuongDinhKy) < 0) {
            taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "baoDuongDinhKy_positive")];
            taiSan.isValid = false;
          }
        }
      }

      if (taiSan.namSx) {
        if (Number(taiSan.namSx) != NaN) {
          if (Number(taiSan.namSx) < 0) {
            taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "namSx_positive")];
            taiSan.isValid = false;
          }

          if (!Number.isInteger(Number(taiSan.namSx))) {
            taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "namSx_mustBeInt")];
            taiSan.isValid = false;
          }

        }
      }

      if (taiSan.thoiHanBaoHanh) {
        if (Number(taiSan.thoiHanBaoHanh) != NaN) {
          if (Number(taiSan.thoiHanBaoHanh) < 0) {
            taiSan.listStatus = [...taiSan.listStatus, this.listNote.find(e => e.code == "thoiHanBaoHanh_positive")];
            taiSan.isValid = false;
          }
        }
      }

    });



    /* auto add to valid list */
    if (autoAdd) this.selectedTaiSanImport = this.listTaiSanImport.filter(e => e.isValid);
  }

  onCancel() {
    let result: ResultDialog = {
      status: false,
      statusImport: false
    };
    this.ref.close(result);
  }

  async importCustomer() {
    /* check valid list selected */
    if (this.selectedTaiSanImport.length == 0) {
      let msg = { severity: 'warn', summary: 'Thông báo', detail: 'Chọn danh sách cần import' };
      this.showMessage(msg);
      return;
    }
    let inValidRecord = this.selectedTaiSanImport.find(e => !e.isValid);
    if (inValidRecord) {
      let msg = { severity: 'error', summary: 'Thông báo', detail: 'Danh sách không hợp lệ' };
      this.showMessage(msg);
      return;
    }
    this.checkStatus(false);
    this.standardizedListCustomer();
    let listTaiSanAdditional: Array<TaiSanModel> = [];
    this.selectedTaiSanImport.forEach(item => {
      var newTaiSan = this.mapFormToTaiSanModel(item);
      listTaiSanAdditional.push(newTaiSan);
    });
    this.loading = true;
    let result: any = await this.assetService.importAsset(listTaiSanAdditional);
    this.loading = false;
    if (result.statusCode === 200) {
      let mgs = { severity: 'success', summary: 'Thông báo', detail: result.message };
      this.showMessage(mgs);
      this.ref.close(result);
    } else {
      let mgs = { severity: 'error', summary: 'Thông báo', detail: result.message };
      this.showMessage(mgs);
      console.log(result)
      // this.ref.close(result);
    }
  }

  standardizedListCustomer() {
    this.listTaiSanImport.forEach(customer => {
      // customer.materialID = customer.materialID?.trim() ?? "";
    });
  }

  mapFormToTaiSanModel(taiSan): TaiSanModel {
    let taiSanModel = new TaiSanModel();
    taiSanModel.maTaiSan = taiSan.maTaiSan.trim();
    taiSanModel.tenTaiSan = taiSan.tenTaiSan.trim();
    taiSanModel.phanLoaiTaiSanId = taiSan.phanLoaiTaiSanId;
    taiSanModel.donViTinhId = taiSan.donViTinhId;
    taiSanModel.khuVucTaiSanId = taiSan.khuVucTaiSanId;
    taiSanModel.ngayVaoSo = taiSan.ngayVaoSo != null ? convertToUTCTime(new Date(taiSan.ngayVaoSo)) : null;
    taiSanModel.moTa = taiSan.moTa;
    taiSanModel.soSerial = taiSan.soSerial;
    taiSanModel.model = taiSan.model;
    taiSanModel.soHieu = taiSan.soHieu;
    taiSanModel.thongTinNoiMua = taiSan.thongTinNoiMua;
    taiSanModel.namSX = taiSan.namSx;
    taiSanModel.phuongPhapTinhKhauHao = 1;
    taiSanModel.soLuong = 1;
    taiSanModel.nuocSXId = taiSan.nuocSxid;
    taiSanModel.hangSXId = taiSan.hangSxid;
    taiSanModel.ngayMua = taiSan.ngayMua != null ? convertToUTCTime(new Date(taiSan.ngayMua)) : null;
    taiSanModel.thoiHanBaoHanh = taiSan.thoiHanBaoHanh ? parseInt(taiSan.thoiHanBaoHanh) : null;
    taiSanModel.baoDuongDinhKy = taiSan.baoDuongDinhKy ? parseInt(taiSan.baoDuongDinhKy) : null;
    taiSanModel.thongTinNoiBaoHanh = taiSan.thongTinNoiBaoHanh;
    taiSanModel.giaTriNguyenGia = taiSan.giaTriNguyenGia;
    taiSanModel.giaTriTinhKhauHao = taiSan.giaTriTinhKhauHao;
    taiSanModel.thoiGianKhauHao = taiSan.thoiGianKhauHao;
    taiSanModel.thoiDiemBDTinhKhauHao = taiSan.thoiDiemBdtinhKhauHao != null ? convertToUTCTime(new Date(taiSan.thoiDiemBdtinhKhauHao)) : null;
    taiSanModel.createdById = this.auth.UserId;
    taiSanModel.createdDate = convertToUTCTime(new Date());
    taiSanModel.updatedById = null;
    taiSanModel.updatedDate = null;
    return taiSanModel;
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

