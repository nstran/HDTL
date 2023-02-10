import { Component, OnInit, ViewChild, ElementRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { AssetService } from '../../services/asset.service';
//import { NoteService } from '../../../shared/services/note.service';
import { TaiSanModel } from '../../models/taisan.model';
import { Paginator } from 'primeng';
import { DatePipe } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { EmployeeService } from '../../../employee/services/employee.service';
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

class BaoDuongModel {
  baoDuongTaiSanId: number;
  taiSanId: number;
  tuNgay: Date;
  denNgay: Date;
  nguoiPhuTrachId: string;
  moTa: string;
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
class Province {
  public provinceId: string;
  public provinceCode: string;
  public provinceName: string;
  public provinceType: string;
}


@Component({
  selector: 'app-asset-detail',
  templateUrl: './asset-detail.component.html',
  styleUrls: ['./asset-detail.component.css']
})
export class AssetDetailComponent implements OnInit {
  today: Date = new Date();
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('paginator', { static: true }) paginator: Paginator

  /* End */
  /*Khai báo biến*/
  auth: any = JSON.parse(localStorage.getItem("auth"));
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  /* Action*/
  actionThuHoi: boolean = true;
  actionCapPhat: boolean = true;
  /*END*/
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;

  /* Form */
  updateAssetForm: FormGroup;
  // Thông tin chung
  maTaiSanControl: FormControl;
  phanLoaiTSControl: FormControl;
  tenTaiSanControl: FormControl;
  ngayVaoSoControl: FormControl;
  donViControl: FormControl;
  maCodeControl: FormControl;
  //hienTrangTaiSanControl: FormControl;
  soLuongControl: FormControl;
  moTaControl: FormControl;
  khuVucTSControl: FormControl;

  // Thông tin chi tiết
  serialControl: FormControl;
  namSXControl: FormControl;
  ngayMuaControl: FormControl;
  modelControl: FormControl;
  nuocSXControl: FormControl;
  thoiHanBHControl: FormControl;
  soHieuControl: FormControl;
  hangSXControl: FormControl;
  baoDuongDinhKyControl: FormControl;
  thongTinNoiMuaControl: FormControl;
  thongTinNoiBaoHanhControl: FormControl;

  // Khấu hao
  giaTriNguyenGiaControl: FormControl;
  thoiGianKhauHaoControl: FormControl;
  giaTriTinhKhauHaoControl: FormControl;
  thoiDiemTinhKhauHaoControl: FormControl;
  phuongPhapTinhKhauHaoControl: FormControl;
  thoiDiemKetThucKhauHao: string = '';
  /* End */

  colLeft: number = 8;
  isShow: boolean = true;
  taiSanId: number = 0;

  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();
  colsKhauHao: any;
  colsPhanBoTS: any;
  // Dữ liệu masterdata
  listPhanLoaiTS: Array<CategoryModel> = new Array<CategoryModel>();
  listDonVi: Array<CategoryModel> = new Array<CategoryModel>();
  listKhuVuc: Array<Province> = [];
  listNuocSX: Array<CategoryModel> = new Array<CategoryModel>();
  listHangSX: Array<CategoryModel> = new Array<CategoryModel>();
  listEmployee: any[] = [];
  listTaiSanPhanBo: any[] = [];
  listPhuongPhapKhauHao = [
    {
      id: 1, name: 'Khấu hao đường thẳng'
    }];

  tyLeKhauHao = [
    {
      id: 1, name: 'Theo tháng'
    },
    {
      id: 2, name: 'Theo năm'
    }];
  // notification
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;

  // Bảo dưỡng
  selectedColumns: any[];
  listBaoDuongBaoTri: Array<BaoDuongModel> = [];

  baoDuongFormGroup: FormGroup;
  tuNgayFormControl: FormControl;
  denNgayFormControl: FormControl;
  //nguoiPhuTrachFormControl: FormControl;
  moTaFormControl: FormControl;

  tiLeKhauHaoTheoThang: number = 0;
  giaTriKhauHaoTheoThang: number = 0;
  tiLeKhauHaoTheoNam: number = 0;
  giaTriKhauHaoTheoNam: number = 0;
  giaTriKhauHaoLuyKe: number = 0;
  giaTriConLai: number = 0;

  clonedData: { [s: string]: any; } = {}
  isNewLine: boolean;

  assetDetailModel: TaiSanModel;
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
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];

  hienTrangTaiSan: number = 0;

  displayThuHoi: boolean = false;
  lyDoThuHoi: string = null;
  ngayThuHoi: Date = null;
  minNgayThuHoi: Date = new Date();



  displayCapPhat: boolean = false;
  minNgayCapPhat: Date = new Date();
  nhanVienPB: any;
  mucDichSuDung: any;
  thoiGianBD: Date;
  thoiGianKT: Date;
  lyDoCapPhat: string = null;
  listMucDichSuDung: Array<CategoryModel> = new Array<CategoryModel>();
  /*End : Note*/

    //note
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionDownLoad: boolean = true;
  actionUpLoad: boolean = true;
  viewNote: boolean = true;
  viewTimeline: boolean = true;
  pageSize = 20;

  isEdit: boolean = true;

  constructor(
    private assetService: AssetService,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private getPermission: GetPermission,
    //private noteService: NoteService,
    private el: ElementRef,
    private ref: ChangeDetectorRef,
    private employeeService: EmployeeService,
    private encrDecrService: EncrDecrService,
  ) {

  }

  async ngOnInit() {
    this.setTable();
    this.setForm();

    //Lấy action cấp phát bên màn chi tiết yêu cầu cấp phát
    let resourceDetail = "ass/asset/chi-tiet-yeu-cau-cap-phat";
    let permissionDetail: any = await this.getPermission.getPermission(resourceDetail);
    if (permissionDetail.status == false) {
      this.router.navigate(['/home']);
      this.showToast('warn', 'Thông báo', 'Bạn không có quyền truy cập');
    }
    else {
      let listCurrentActionResource = permissionDetail.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("cap-phat") == -1) {
        this.actionCapPhat = false;
      }
    }

    let resource = "ass/asset/detail";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
      this.showToast('warn', 'Thông báo', 'Bạn không có quyền truy cập');
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;

      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      if (listCurrentActionResource.indexOf("thu-hoi") == -1) {
        this.actionThuHoi = false;
      }
      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }

      this.route.params.subscribe(params => {
        this.taiSanId = Number(this.encrDecrService.get(params['assetId']));
      });

      this.getMasterData();
      this.soLuongControl.setValue(1);
    }
  }

  setForm() {
    this.maTaiSanControl = new FormControl(null, [Validators.required]); // Mã tài sản
    this.phanLoaiTSControl = new FormControl(null, [Validators.required]); // Phân loại tài sản
    this.khuVucTSControl = new FormControl(null, [Validators.required]); // Địa điểm phân bổ tài sản
    this.tenTaiSanControl = new FormControl(null, [Validators.required]); // Tên tài sản
    this.ngayVaoSoControl = new FormControl(null, [Validators.required]); // Ngày vào sổ
    this.donViControl = new FormControl(null, [Validators.required]); // Đơn vị
    this.maCodeControl = new FormControl(null, [Validators.required]); // Mã code
    //this.hienTrangTaiSanControl = new FormControl(null, [Validators.required]); // Hiện trạng tài sản
    this.soLuongControl = new FormControl(null, [Validators.required]); // Số lượng
    this.moTaControl = new FormControl(null); // Mô tả

    // Thông tin chi tiết
    this.serialControl = new FormControl(null, [Validators.required]); // Serial
    this.namSXControl = new FormControl(null); // Năm sản xuất
    this.ngayMuaControl = new FormControl(null); // Ngày mua
    this.modelControl = new FormControl(null, [Validators.required]); // Model
    this.nuocSXControl = new FormControl(null); // Nước sản xuất
    this.thoiHanBHControl = new FormControl(null); // Thời hạn bảo hành (tháng)
    this.soHieuControl = new FormControl(null); // Số hiệu
    this.hangSXControl = new FormControl(null); // Hãng sản xuất
    this.baoDuongDinhKyControl = new FormControl(null); // Bảo dưỡng định kỳ (tháng)
    this.thongTinNoiMuaControl = new FormControl(null); // Thông tin nơi mua
    this.thongTinNoiBaoHanhControl = new FormControl(null); // Thông tin nơi bảo trì/bảo dưỡng

    // Khấu hao
    this.giaTriNguyenGiaControl = new FormControl(null); // Giá trị nguyên giá
    this.thoiGianKhauHaoControl = new FormControl(null); // Thời gian khấu hao (tháng)
    this.giaTriTinhKhauHaoControl = new FormControl(null, [Validators.required]);
    this.thoiDiemTinhKhauHaoControl = new FormControl(null); // Thời điểm bắt đầu tính khấu hao
    this.phuongPhapTinhKhauHaoControl = new FormControl(null, [Validators.required]); // Phương pháp khấu khao

    // Bảo dưỡng
    this.tuNgayFormControl = new FormControl(null, [Validators.required]);
    this.denNgayFormControl = new FormControl(null);
    //this.nguoiPhuTrachFormControl = new FormControl(null, [Validators.required]);
    this.moTaFormControl = new FormControl('');

    this.baoDuongFormGroup = new FormGroup({
      tuNgayFormControl: this.tuNgayFormControl,
      denNgayFormControl: this.denNgayFormControl,
      moTaFormControl: this.moTaFormControl,
      // nguoiPhuTrachFormControl: this.nguoiPhuTrachFormControl,
    });

    this.updateAssetForm = new FormGroup({
      maTaiSanControl: this.maTaiSanControl,
      phanLoaiTSControl: this.phanLoaiTSControl,
      khuVucTSControl: this.khuVucTSControl,
      tenTaiSanControl: this.tenTaiSanControl,
      ngayVaoSoControl: this.ngayVaoSoControl,
      donViControl: this.donViControl,
      maCodeControl: this.maCodeControl,
      //hienTrangTaiSanControl: this.hienTrangTaiSanControl,
      soLuongControl: this.soLuongControl,
      moTaControl: this.moTaControl,

      serialControl: this.serialControl,
      namSXControl: this.namSXControl,
      ngayMuaControl: this.ngayMuaControl,
      modelControl: this.modelControl,
      nuocSXControl: this.nuocSXControl,
      thoiHanBHControl: this.thoiHanBHControl,
      soHieuControl: this.soHieuControl,
      hangSXControl: this.hangSXControl,
      baoDuongDinhKyControl: this.baoDuongDinhKyControl,
      thongTinNoiMuaControl: this.thongTinNoiMuaControl,
      thongTinNoiBaoHanhControl: this.thongTinNoiBaoHanhControl,

      giaTriNguyenGiaControl: this.giaTriNguyenGiaControl,
      thoiGianKhauHaoControl: this.thoiGianKhauHaoControl,
      giaTriTinhKhauHaoControl: this.giaTriTinhKhauHaoControl,
      thoiDiemTinhKhauHaoControl: this.thoiDiemTinhKhauHaoControl,
      phuongPhapTinhKhauHaoControl: this.phuongPhapTinhKhauHaoControl,
    });
  }

  async getMasterData() {
    this.loading = true;
    let [result, resultDetail]: any = await Promise.all([
      this.assetService.getMasterDataAssetForm(),
      this.assetService.getDataAssetDetail(this.taiSanId, this.auth.UserId)
    ]);

    if (result.statusCode === 200 && resultDetail.statusCode == 200) {

      this.loading = false;
      this.listPhanLoaiTS = result.listPhanLoaiTS;
      this.listDonVi = result.listDonVi;
      this.listNuocSX = result.listNuocSX;
      this.listHangSX = result.listHangSX;
      this.listEmployee = result.listEmployee;
      this.listMucDichSuDung = result.listMucDichSuDung;
      this.listKhuVuc = result.listProvinceModel;
      // Chi tiết tài sản
      this.listTaiSanPhanBo = resultDetail.listTaiSanPhanBo;

      this.listBaoDuongBaoTri = resultDetail.listBaoDuong;
      this.listBaoDuongBaoTri.forEach(bd => {
        bd.tuNgay = new Date(bd.tuNgay);
        bd.denNgay = new Date(bd.denNgay);
      })

      this.hienTrangTaiSan = resultDetail.assetDetail.hienTrangTaiSan; // 1 Đang sử dụng --- 0 Không sử dụng
      this.assetDetailModel = this.mappingAssetModel(resultDetail.assetDetail);
      this.mappingModelToFrom();

      this.arrayDocumentModel = resultDetail.listFileInFolder;
      // this.noteHistory = resultDetail.listNote;
      // this.handleNoteContent();
      this.ref.detectChanges();
    }
    else
      this.loading = false;
  }

  setTable() {
    this.colsKhauHao = [
      { field: 'stt', header: '#', width: '95px', textAlign: 'left', color: '#f44336' },
      { field: 'fromDate', header: 'Từ ngày', width: '120px', textAlign: 'left', color: '#f44336' },
      { field: 'toDate', header: 'Đến ngày', width: '120px', textAlign: 'left', color: '#f44336' },
      { field: 'giaTriTruocKH', header: 'Giá trị trước khấu hao', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'giaTriSauKH', header: 'Giá trị sau khấu hao', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'giaTriConLaiSauKH', header: 'Giá trị còn lại sau khấu hao', width: '250px', textAlign: 'left', color: '#f44336' },
    ];

    this.selectedColumns = [
      { field: 'index', header: 'STT', width: '30px', textAlign: 'center' },
      { field: 'tuNgay', header: 'Từ ngày', width: '100px', textAlign: 'left' },
      { field: 'denNgay', header: 'Đến ngày', width: '100px', textAlign: 'left' },
      { field: 'nguoiPhuTrach', header: 'Người phụ trách', width: '200px', textAlign: 'left' },
      { field: 'moTa', header: 'Mô tả', width: '150px', textAlign: 'left' },
      { field: 'action', header: 'Thao tác', width: '80px', textAlign: 'center' },
    ];

    this.colsPhanBoTS = [
      { field: 'loaiCapPhat', header: 'Phân loại', width: '100px', textAlign: 'center' },
      { field: 'ngayBatDau', header: 'Từ ngày', width: '100px', textAlign: 'center' },
      { field: 'ngayKetThuc', header: 'Đến ngày', width: '100px', textAlign: 'center' },
      { field: 'maNV', header: 'Mã nhân viên', width: '100px', textAlign: 'center' },
      { field: 'hoVaTen', header: 'Tên nhân viên', width: '200px', textAlign: 'left' },
      { field: 'phongBan', header: 'Phòng ban', width: '150px', textAlign: 'left' },
      { field: 'viTriLamViec', header: 'Vị trí làm việc', width: '150px', textAlign: 'left' },
      { field: 'nguoiCapPhat', header: 'Người cấp phát', width: '200px', textAlign: 'left' },
      { field: 'lyDo', header: 'Lý do', width: '150px', textAlign: 'left' },
    ];
  }

  mappingAssetModel(model: any): TaiSanModel {
    let taiSanModel = new TaiSanModel();

    // Tài sản Id
    taiSanModel.taiSanId = model.taiSanId;
    // Mã tài sản
    taiSanModel.maTaiSan = model.maTaiSan;
    // Tên tài sản
    taiSanModel.tenTaiSan = model.tenTaiSan;
    // khu vực tài sản
    taiSanModel.khuVucTaiSanId = model.khuVucTaiSanId;

    // Phân loại tài sản
    taiSanModel.phanLoaiTaiSanId = model.phanLoaiTaiSanId;

    // Đơn vị
    taiSanModel.donViTinhId = model.donViTinhId;

    // Số lượng
    taiSanModel.soLuong = model.soLuong;

    // Ngày vào sổ
    taiSanModel.ngayVaoSo = model.ngayVaoSo;

    // Mô tả
    taiSanModel.moTa = model.moTa;

    // Mã code
    taiSanModel.maCode = model.maCode;

    // Hiện trạng tài sản
    taiSanModel.hienTrangTaiSan = model.hienTrangTaiSan;

    taiSanModel.soSerial = model.soSerial;
    taiSanModel.namSX = model.namSX;

    // Ngày mua
    taiSanModel.ngayMua = model.ngayMua;

    // Model
    taiSanModel.model = model.model;

    // Nước sản xuất
    taiSanModel.nuocSXId = model.nuocSXId;

    // Số hiệu
    taiSanModel.soHieu = model.soHieu;

    // Hãng sản xuất
    taiSanModel.hangSXId = model.hangSXId;

    // Thời hạn bảo hành
    taiSanModel.thoiHanBaoHanh = model.thoiHanBaoHanh;

    // Bảo dưỡng định kỳ (tháng)
    taiSanModel.baoDuongDinhKy = model.baoDuongDinhKy;

    // Thông tin nơi mua
    taiSanModel.thongTinNoiMua = model.thongTinNoiMua;

    // Thông tin nơi bảo hành/ bảo trì
    taiSanModel.thongTinNoiBaoHanh = model.thongTinNoiBaoHanh;


    // KHẤU HAO
    //Thời điểm bắt đầu tính khấu hao
    taiSanModel.thoiDiemBDTinhKhauHao = model.thoiDiemBDTinhKhauHao;

    taiSanModel.giaTriNguyenGia = model.giaTriNguyenGia;
    taiSanModel.giaTriTinhKhauHao = model.giaTriTinhKhauHao;
    taiSanModel.tiLeKhauHao = model.tiLeKhauHao;
    taiSanModel.thoiGianKhauHao = model.thoiGianKhauHao;
    taiSanModel.phuongPhapTinhKhauHao = model.phuongPhapTinhKhauHao;
    taiSanModel.nguoiSuDungId = model.nguoiSuDungId;

    taiSanModel.createdById = model.createdById;
    taiSanModel.createdDate = model.createdDate;
    taiSanModel.updatedById = this.auth.UserId;
    taiSanModel.updatedDate = convertToUTCTime(new Date());

    return taiSanModel;
  }

  mappingModelToFrom() {

    // mã tài sản
    this.maTaiSanControl.setValue(this.assetDetailModel.maTaiSan);

    // Tên tài sản
    this.tenTaiSanControl.setValue(this.assetDetailModel.tenTaiSan);

    // mã code
    this.maCodeControl.setValue(this.assetDetailModel.maCode);

    //phanLoaiTSControl
    let phanLoai = this.listPhanLoaiTS.find(x => x.categoryId == this.assetDetailModel.phanLoaiTaiSanId)
    this.phanLoaiTSControl.setValue(phanLoai);

    //khuVucTSControl
    let khuVuc = this.listKhuVuc.find(x => x.provinceId == this.assetDetailModel.khuVucTaiSanId)
    this.khuVucTSControl.setValue(khuVuc);

    // Ngày vào sổ
    let ngayVS = this.assetDetailModel.ngayVaoSo ? new Date(this.assetDetailModel.ngayVaoSo) : null;
    this.ngayVaoSoControl.setValue(ngayVS);

    //donViControl
    let donVi = this.listDonVi.find(x => x.categoryId == this.assetDetailModel.donViTinhId)
    this.donViControl.setValue(donVi);

    //hienTrangTaiSanControl
    // let hientrang = this.listHienTrangTS.find(x => x.id == this.assetDetailModel.hienTrangTaiSan)
    // this.hienTrangTaiSanControl.setValue(hientrang);

    // Số lượng
    this.soLuongControl.setValue(this.assetDetailModel.soLuong);

    // Mô tả
    this.moTaControl.setValue(this.assetDetailModel.moTa);


    //#region  Thông tin chi tiết
    // serialControl
    this.serialControl.setValue(this.assetDetailModel.soSerial);

    // namSXControl
    this.namSXControl.setValue(this.assetDetailModel.namSX);

    // Ngày mua
    let ngayMua = this.assetDetailModel.ngayMua ? new Date(this.assetDetailModel.ngayMua) : null;
    this.ngayMuaControl.setValue(ngayMua);

    // modelControl
    this.modelControl.setValue(this.assetDetailModel.model);

    // nuocSXControl
    let nuocSx = this.listNuocSX.find(x => x.categoryId == this.assetDetailModel.nuocSXId)
    this.nuocSXControl.setValue(nuocSx);

    //thoiHanBHControl
    this.thoiHanBHControl.setValue(this.assetDetailModel.thoiHanBaoHanh);

    // soHieuControl
    this.soHieuControl.setValue(this.assetDetailModel.soHieu);

    // hangSXControl
    let hangSX = this.listHangSX.find(x => x.categoryId == this.assetDetailModel.hangSXId)
    this.hangSXControl.setValue(hangSX);


    // baoDuongDinhKyControl
    this.baoDuongDinhKyControl.setValue(this.assetDetailModel.baoDuongDinhKy);

    // thongTinNoiMuaControl
    this.thongTinNoiMuaControl.setValue(this.assetDetailModel.thongTinNoiMua);

    // thongTinNoiBaoHanhControl
    this.thongTinNoiBaoHanhControl.setValue(this.assetDetailModel.thongTinNoiBaoHanh);

    //#endregion

    //#region Khấu hao
    // giaTriNguyenGiaControl
    this.giaTriNguyenGiaControl.setValue(this.assetDetailModel.giaTriNguyenGia);

    // giaTriTinhKhauHaoControl
    this.giaTriTinhKhauHaoControl.setValue(this.assetDetailModel.giaTriTinhKhauHao);

    // phuongPhapTinhKhauHaoControl
    let khauHao = this.listPhuongPhapKhauHao.find(x => x.id == this.assetDetailModel.phuongPhapTinhKhauHao)
    this.phuongPhapTinhKhauHaoControl.setValue(khauHao);

    //thoiDiemTinhKhauHaoControl
    let thoiDiemBD = this.assetDetailModel.thoiDiemBDTinhKhauHao ? new Date(this.assetDetailModel.thoiDiemBDTinhKhauHao) : null;
    this.thoiDiemTinhKhauHaoControl.setValue(thoiDiemBD);

    //thoiGianKhauHaoControl
    this.thoiGianKhauHaoControl.setValue(this.assetDetailModel.thoiGianKhauHao);

    this.tinhThoiGianKhauHao();
    //#endregion
  }

  // Lưu tài sản
  updateAsset() {
    if (!this.updateAssetForm.valid) {
      Object.keys(this.updateAssetForm.controls).forEach(key => {
        if (!this.updateAssetForm.controls[key].valid) {
          this.updateAssetForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.updateAssetForm.statusChanges.subscribe((validity: string) => {
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
      let taiSanModel: TaiSanModel = this.mapFormToTaiSanModel();
      this.loading = true;
      this.awaitResult = true;
      this.assetService.createOrUpdateAsset(taiSanModel, this.auth.UserId).subscribe(response => {
        this.loading = false;
        let result = <any>response;

        if (result.statusCode == 200) {
          this.showToast('success', 'Thông báo', result.message);
          this.getMasterData();
          this.awaitResult = false;
        }
        else {

          this.clearToast();
          this.showToast('error', 'Thông báo', result.message);
        }
      });
    }
  }

  //Function reset toàn bộ các value đã nhập trên form
  resetFieldValue() {
    this.updateAssetForm.reset();
    this.thoiDiemKetThucKhauHao = "";
  }

  mapFormToTaiSanModel(): TaiSanModel {
    let taiSanModel = new TaiSanModel();

    // Mã tài sản
    taiSanModel.maTaiSan = this.maTaiSanControl.value;
    taiSanModel.taiSanId = this.taiSanId;

    // Phân loại tài sản
    let phanLoaiTS = this.updateAssetForm.get('phanLoaiTSControl').value
    taiSanModel.phanLoaiTaiSanId = phanLoaiTS ? phanLoaiTS.categoryId : this.emptyGuid;

    // Khu vực tài sản
    let khuVucTS = this.updateAssetForm.get('khuVucTSControl').value
    taiSanModel.khuVucTaiSanId = khuVucTS ? khuVucTS.provinceId : this.emptyGuid;

    // Đơn vị
    let donVi = this.updateAssetForm.get('donViControl').value
    taiSanModel.donViTinhId = donVi ? donVi.categoryId : this.emptyGuid;

    // Số lượng
    taiSanModel.soLuong = this.soLuongControl.value;
    // Tên tài sản
    taiSanModel.tenTaiSan = this.tenTaiSanControl.value;

    // Ngày vào sổ
    let ngayVS = this.updateAssetForm.get('ngayVaoSoControl').value ? convertToUTCTime(this.updateAssetForm.get('ngayVaoSoControl').value) : null;
    taiSanModel.ngayVaoSo = ngayVS;

    // Mô tả
    taiSanModel.moTa = this.moTaControl.value;

    // Mã code
    taiSanModel.maCode = this.maCodeControl.value;

    taiSanModel.soSerial = this.serialControl.value;
    taiSanModel.namSX = this.namSXControl.value;

    // Ngày mua
    let ngayMua = this.updateAssetForm.get('ngayMuaControl').value ? convertToUTCTime(this.updateAssetForm.get('ngayMuaControl').value) : null;
    taiSanModel.ngayMua = ngayMua;

    // Model
    taiSanModel.model = this.modelControl.value;

    // Nước sản xuất
    let nuocSX = this.updateAssetForm.get('nuocSXControl').value
    taiSanModel.nuocSXId = nuocSX ? nuocSX.categoryId : this.emptyGuid;

    // Số hiệu
    taiSanModel.soHieu = this.soHieuControl.value;

    // Hãng sản xuất
    let hangSX = this.updateAssetForm.get('hangSXControl').value
    taiSanModel.hangSXId = hangSX ? hangSX.categoryId : this.emptyGuid;

    // Thời hạn bảo hành
    taiSanModel.thoiHanBaoHanh = this.thoiHanBHControl.value;

    // Thông tin nơi mua
    taiSanModel.thongTinNoiMua = this.thongTinNoiMuaControl.value;

    // Thông tin nơi bảo hành/ bảo trì
    taiSanModel.thongTinNoiBaoHanh = this.thongTinNoiBaoHanhControl.value;


    // KHẤU HAO
    //Thời điểm bắt đầu tính khấu hao
    let ngayTinhKH = this.updateAssetForm.get('thoiDiemTinhKhauHaoControl').value ? convertToUTCTime(this.updateAssetForm.get('thoiDiemTinhKhauHaoControl').value) : null;
    taiSanModel.thoiDiemBDTinhKhauHao = ngayTinhKH;

    taiSanModel.giaTriNguyenGia = ParseStringToFloat(this.giaTriNguyenGiaControl.value);
    taiSanModel.giaTriTinhKhauHao = ParseStringToFloat(this.giaTriTinhKhauHaoControl.value);

    taiSanModel.thoiGianKhauHao = ParseStringToFloat(this.thoiGianKhauHaoControl.value);
    taiSanModel.baoDuongDinhKy = ParseStringToFloat(this.baoDuongDinhKyControl.value);

    taiSanModel.phuongPhapTinhKhauHao = this.phuongPhapTinhKhauHaoControl.value.id;

    taiSanModel.createdById = this.auth.UserId;
    taiSanModel.createdDate = convertToUTCTime(new Date());
    taiSanModel.updatedById = null;
    taiSanModel.updatedDate = null;
    return taiSanModel;
  }

  // // Phân bổ tài sản
  // phanBoTaiSan() {
  //   this.router.navigate(['/asset/phan-bo-tai-san', { assetId: this.taiSanId }]);
  // }

  themMoiBaoTri() {

    let baoDuong = {
      baoDuongTaiSanId: 0,
      taiSanId: this.taiSanId,
      tuNgay: new Date(),
      denNgay: new Date(),
      nguoiPhuTrachId: this.emptyGuid,
      moTa: '',
      isNewLine: true
    };
    this.listBaoDuongBaoTri.push(baoDuong);
    this.ref.detectChanges();
  }

  //#region Bảo dưỡng
  onRowEditInit(rowData: any) {
    this.clonedData[rowData.baoDuongTaiSanId] = { ...rowData };
    this.ref.detectChanges();
  }


  changeNguoiPhuTrach(event, rowData) {
    rowData.nguoiPhuTrachId = event.employeeId;
  }

  changeTuNgay(rowData) {
    rowData.tuNgay = this.tuNgayFormControl.value;
  }

  changeDenNgay(rowData) {
    rowData.denNgay = this.denNgayFormControl.value;
  }
  changeDes(rowData) {
    rowData.moTa = this.moTaFormControl.value;
  }

  onRowEditSave(rowData: any) {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn muốn thay đổi`,
      accept: async () => {
        this.loading = true;
        let baoDuong: any = await this.assetService.createOrUpdateBaoDuong(rowData);
        this.loading = false;
        if (baoDuong.statusCode == 200) {

          this.listBaoDuongBaoTri = baoDuong.listBaoDuong;
          // if (rowData.isNewLine) {
          //   rowData.baoDuongTaiSanId = baoDuong.baoDuongTaiSanId;
          //   rowData.isNewLine = false;
          this.listBaoDuongBaoTri.forEach(bd => {
            bd.tuNgay = new Date(bd.tuNgay);
            bd.denNgay = new Date(bd.denNgay);
          })

          this.ref.detectChanges();
          this.showToast('success', 'Thông báo', 'Cập nhật thành công');
        } else {
          this.showToast('error', 'Thông báo', baoDuong.message);
        }
      }
    });
  }

  onRowEditCancel(rowData: any) {

    Object.keys(this.clonedData).forEach(key => {
      if (key == rowData.baoDuongTaiSanId && rowData.baoDuongTaiSanId != 0) {
        rowData.baoDuongTaiSanId = this.clonedData[key].baoDuongTaiSanId;
        rowData.tuNgay = this.clonedData[key].tuNgay;
        rowData.denNgay = this.clonedData[key].denNgay;
        rowData.nguoiPhuTrachId = this.clonedData[key].nguoiPhuTrachId;
        rowData.moTa = this.clonedData[key].moTa;
      }
      else {
        this.listBaoDuongBaoTri = this.listBaoDuongBaoTri.filter(e => e != rowData);
      }
    });
  }

  async onRowRemove(rowData: any) {
    if (!rowData.isNewLine) {
      this.confirmationService.confirm({
        message: `Bạn có chắc chắn xóa dòng này?`,
        accept: async () => {
          this.loading = true;
          let result: any = await this.assetService.deleteBaoDuong(rowData.baoDuongTaiSanId);
          this.loading = false;
          if (result.statusCode === 200) {
            this.listBaoDuongBaoTri = this.listBaoDuongBaoTri.filter(e => e != rowData);
            this.showToast('success', 'Thông báo', 'Xóa dữ liệu thành công');
          }
          else
            this.showToast('error', 'Thông báo', result.messageCode);
        }
      });
    }
    else {
      this.listBaoDuongBaoTri = this.listBaoDuongBaoTri.filter(e => e != rowData);
    }
  }

  //#endregion

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
      fileUpload.FileInFolder.objectNumber = this.taiSanId;
      fileUpload.FileInFolder.objectType = 'ASSET';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });


    this.assetService.uploadFile("ASSET", listFileUploadModel, this.taiSanId).subscribe(response => {
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

  // Event thay đổi nội dung ghi chú
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  /*Event Sửa ghi chú*/
  onClickEditNote(noteId: string, noteDes: string) {
    this.noteContent = noteDes;
    this.noteId = noteId;
    this.listUpdateNoteDocument = this.noteHistory.find(x => x.noteId == this.noteId).noteDocList;
    this.isEditNote = true;
  }
  /*End*/

  /*Event Xóa ghi chú*/
  // onClickDeleteNote(noteId: string) {
  //   this.confirmationService.confirm({
  //     message: 'Bạn chắc chắn muốn xóa ghi chú này?',
  //     accept: () => {
  //       this.loading = true;
  //       this.noteService.disableNote(noteId).subscribe(response => {
  //         let result: any = response;
  //         this.loading = false;

  //         if (result.statusCode == 200) {
  //           let note = this.noteHistory.find(x => x.noteId == noteId);
  //           let index = this.noteHistory.lastIndexOf(note);
  //           this.noteHistory.splice(index, 1);

  //           this.showToast('success', 'Thông báo', 'Xóa ghi chú thành công');
  //         } else {
  //           this.showToast('error', 'Thông báo', result.messageCode);
  //         }
  //       });
  //     }
  //   });
  // }

  /* Event thêm file dược chọn vào list file note */
  handleNoteFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= this.defaultLimitedFileSize) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedNoteFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedNoteFiles.push(file);
          }
        }
      }
    }
  }

  /*Event khi click xóa từng file */
  removeNoteFile(event) {
    let index = this.uploadedNoteFiles.indexOf(event.file);
    this.uploadedNoteFiles.splice(index, 1);
  }
  /*End*/

  /*Event khi click xóa toàn bộ file */
  clearAllNoteFile() {
    this.uploadedNoteFiles = [];
  }

  // cancelNote() {
  //   this.confirmationService.confirm({
  //     message: 'Bạn có chắc muốn hủy ghi chú này?',
  //     accept: () => {
  //       this.noteId = null;
  //       this.noteContent = null;
  //       this.uploadedNoteFiles = [];
  //       if (this.fileNoteUpload) {
  //         this.fileNoteUpload.clear();
  //       }
  //       this.listUpdateNoteDocument = [];
  //       this.isEditNote = false;
  //     }
  //   });
  // }

  /*Lưu file và ghi chú vào Db*/
  // async saveNote() {
  //   this.loading = true;
  //   let objectType = 'ASSET';
  //   this.listNoteDocumentModel = [];

  //   let listFileUploadModel: Array<FileUploadModel> = [];
  //   this.uploadedNoteFiles.forEach(item => {
  //     let fileUpload: FileUploadModel = new FileUploadModel();
  //     fileUpload.FileInFolder = new FileInFolder();
  //     fileUpload.FileInFolder.active = true;
  //     let index = item.name.lastIndexOf(".");
  //     let name = item.name.substring(0, index);
  //     fileUpload.FileInFolder.fileName = name;
  //     fileUpload.FileInFolder.fileExtension = item.name.substring(index + 1);
  //     fileUpload.FileInFolder.size = item.size;
  //     fileUpload.FileInFolder.objectType = objectType;
  //     fileUpload.FileInFolder.objectNumber = this.taiSanId;
  //     fileUpload.FileSave = item;
  //     listFileUploadModel.push(fileUpload);
  //   });
  //   let noteModel = new NoteModel();
  //   if (!this.noteId) {
  //     /*Tạo mới ghi chú*/
  //     noteModel.NoteId = this.emptyGuid;
  //     noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
  //     noteModel.Type = 'ADD';
  //     noteModel.ObjectNumber = this.taiSanId;
  //     noteModel.ObjectType = objectType;
  //     noteModel.NoteTitle = 'đã thêm ghi chú';
  //     noteModel.Active = true;
  //     noteModel.CreatedById = this.emptyGuid;
  //     noteModel.CreatedDate = new Date();
  //   } else {
  //     /*Update ghi chú*/
  //     noteModel.NoteId = this.noteId;
  //     noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
  //     noteModel.Type = 'EDT';
  //     noteModel.ObjectNumber = this.taiSanId;
  //     noteModel.ObjectType = objectType;
  //     noteModel.NoteTitle = 'đã thêm cập nhật';
  //     noteModel.Active = true;
  //     noteModel.CreatedById = this.emptyGuid;
  //     noteModel.CreatedDate = new Date();
  //   }

  //   this.noteHistory = [];
  //   this.noteService.createNoteForAsset(noteModel, objectType, listFileUploadModel).subscribe(response => {
  //     let result: any = response;
  //     this.loading = false;
  //     if (result.statusCode == 200) {
  //       this.uploadedNoteFiles = [];
  //       if (this.fileNoteUpload) {
  //         this.fileNoteUpload.clear();  //Xóa toàn bộ file trong control
  //       }
  //       this.noteContent = null;
  //       this.noteId = null;
  //       this.isEditNote = false;

  //       /*Reshow Time Line*/
  //       //this.arrayDocumentModel = result.listFileInFolder;
  //       this.noteHistory = result.listNote;
  //       this.handleNoteContent();
  //       this.ref.detectChanges();
  //       this.clearToast();
  //       this.showToast('success', 'Thông báo', 'Thêm ghi chú thành công');
  //     } else {
  //       this.showToast('error', 'Thông báo', result.messageCode);
  //     }
  //   });
  // }

  /** Xử lý và hiển thị lại nội dung ghi chú */
  handleNoteContent() {
    this.noteHistory.forEach(element => {
      setTimeout(() => {
        let count = 0;
        if (element.description == null) {
          element.description = "";
        }

        let des = $.parseHTML(element.description);
        let newTextContent = '';
        for (let i = 0; i < des.length; i++) {
          count += des[i].textContent.length;
          newTextContent += des[i].textContent;
        }

        if (count > 250) {
          newTextContent = newTextContent.substr(0, 250) + '<b>...</b>';
          $('#' + element.noteId).find('.short-content').append($.parseHTML(newTextContent));
        } else {
          $('#' + element.noteId).find('.short-content').append($.parseHTML(element.description));
        }

        // $('#' + element.noteId).find('.note-title').append($.parseHTML(element.noteTitle));
        $('#' + element.noteId).find('.full-content').append($.parseHTML(element.description));
      }, 1000);
    });
  }
  /*End*/

  /*Event Mở rộng/Thu gọn nội dung của ghi chú*/
  toggle_note_label: string = 'Mở rộng';
  trigger_node(nodeid: string, event) {
    // noteContent
    let shortcontent_ = $('#' + nodeid).find('.short-content');
    let fullcontent_ = $('#' + nodeid).find('.full-content');
    if (shortcontent_.css("display") === "none") {
      fullcontent_.css("display", "none");
      shortcontent_.css("display", "block");
    } else {
      fullcontent_.css("display", "block");
      shortcontent_.css("display", "none");
    }
    // noteFile
    let shortcontent_file = $('#' + nodeid).find('.short-content-file');
    let fullcontent_file = $('#' + nodeid).find('.full-content-file');
    let continue_ = $('#' + nodeid).find('.continue')
    if (shortcontent_file.css("display") === "none") {
      continue_.css("display", "block");
      fullcontent_file.css("display", "none");
      shortcontent_file.css("display", "block");
    } else {
      continue_.css("display", "none");
      fullcontent_file.css("display", "block");
      shortcontent_file.css("display", "none");
    }
    let curr = $(event.target);

    if (curr.attr('class').indexOf('pi-chevron-right') != -1) {
      this.toggle_note_label = 'Thu gọn';
      curr.removeClass('pi-chevron-right');
      curr.addClass('pi-chevron-down');
    } else {
      this.toggle_note_label = 'Mở rộng';
      curr.removeClass('pi-chevron-down');
      curr.addClass('pi-chevron-right');
    }
  }
  /*End */

  /*Kiểm tra noteText > 250 ký tự hoặc noteDocument > 3 thì ẩn đi một phần nội dung*/
  tooLong(note): boolean {
    if (note.noteDocList.length > 3) return true;
    var des = $.parseHTML(note.description);
    var count = 0;
    for (var i = 0; i < des.length; i++) {
      count += des[i].textContent.length;
      if (count > 250) return true;
    }
    return false;
  }

  //#endregion

  // Xem khấu hao
  xemKhauHao() {

  }

  tinhThoiGianKhauHao() {
    if (!this.thoiGianKhauHaoControl.value || !this.thoiDiemTinhKhauHaoControl.value) {
      this.thoiDiemKetThucKhauHao = "-";
      this.tiLeKhauHaoTheoThang = null;
      this.giaTriKhauHaoTheoThang = null;
      this.tiLeKhauHaoTheoNam = null;

      this.giaTriKhauHaoTheoNam = null;
      this.giaTriKhauHaoLuyKe = null;
      this.giaTriConLai = null;
      return;
    }

    if (this.thoiGianKhauHaoControl.value != null && this.thoiDiemTinhKhauHaoControl.value != null) {
      let ngayTinhKH = this.updateAssetForm.get('thoiDiemTinhKhauHaoControl').value ? convertToUTCTime(this.updateAssetForm.get('thoiDiemTinhKhauHaoControl').value) : null;

      var datePipe = new DatePipe("en-US");
      const result = addMonths(this.thoiGianKhauHaoControl.value, new Date(ngayTinhKH));
      this.thoiDiemKetThucKhauHao = datePipe.transform(result.setDate(result.getDate() - 1), 'dd/MM/yyyy');
    }
    //Giá trị tính khấu hao
    let giaTriTinhKHao = this.giaTriTinhKhauHaoControl.value == null || this.giaTriTinhKhauHaoControl.value == undefined ? 0 : Math.round(ParseStringToFloat(this.giaTriTinhKhauHaoControl.value));

    let thoiGianKhauHao = ParseStringToFloat(this.thoiGianKhauHaoControl.value);

    this.tiLeKhauHaoTheoThang = Number((100 / thoiGianKhauHao).toFixed(2));
    //Giá trị khấu hao theo tháng = (Giá trị tính khấu hao* tỉ lệ khấu hao theo tháng)/100
    this.giaTriKhauHaoTheoThang = Number(((giaTriTinhKHao * this.tiLeKhauHaoTheoThang) / 100).toFixed(2));

    //Tỉ lệ khấu hao theo năm = (100 / (Thời gian khấu hao / 12))
    this.tiLeKhauHaoTheoNam = Number((100 / (thoiGianKhauHao / 12)).toFixed(2));

    //Giá trị khấu hao theo năm = (Giá trị tính khấu hao* tỉ lệ khấu hao theo năm)/100
    this.giaTriKhauHaoTheoNam = Number(((giaTriTinhKHao * this.tiLeKhauHaoTheoNam) / 100).toFixed(2));

    //Giá trị khấu hao lũy kế = [ giá trị khấu hao theo tháng * (tháng hiện tại - tháng bắt đầu tính khấu hao)]
    this.giaTriKhauHaoLuyKe = Number((this.giaTriKhauHaoTheoThang * getCountMonth(this.thoiDiemTinhKhauHaoControl.value, new Date())).toFixed(2)) < 0 ? 0 : Number((this.giaTriKhauHaoTheoThang * getCountMonth(this.thoiDiemTinhKhauHaoControl.value, new Date())).toFixed(2));

    if (this.giaTriKhauHaoLuyKe >= giaTriTinhKHao) this.giaTriKhauHaoLuyKe = giaTriTinhKHao;

    //Giá trị còn lại = Giá trị tính khấu hao - Giá trị khấu hao lũy kế
    this.giaTriConLai = Number((giaTriTinhKHao - this.giaTriKhauHaoLuyKe).toFixed(2));
    this.ref.detectChanges();
  }

  // Thu hồi tài sản
  thuHoiTaiSan() {
    // this.router.navigate(['/asset/thu-hoi-tai-san', { assetId: this.encrDecrService.set(this.taiSanId) }]);
    this.displayThuHoi = true;
    this.lyDoThuHoi = null;
    this.ngayThuHoi = null;
    /*
    1. Ngày kết thúc gần nhất có giá trị
     => Giá trị nhỏ nhất ngày thu hồi >= ngày kết thúc gần nhất
    2. Ngày kết thúc không có giá trị
     => Giá trị nhỏ nhất ngày thu hồi >= ngày bắt đầu gần nhất
    */

    let obj = this.listTaiSanPhanBo[this.listTaiSanPhanBo.length - 1];
    if (obj.ngayKetThuc == null && obj.ngayKetThuc == undefined)
      this.minNgayThuHoi = new Date(obj.ngayBatDau)
    else
      this.minNgayThuHoi = new Date(obj.ngayKetThuc)
  }

  closeThuHoi() {
    this.displayThuHoi = false;
  }

  xacNhanThuHoi() {
    if (!this.ngayThuHoi) {
      this.clearToast();
      this.showToast('warn', 'Thông báo', 'Ngày thu hồi không được để trống');
      return;
    }

    var listThuHoi = [
      {
        CapPhatTaiSanId: 0,
        TaiSanId: this.assetDetailModel.taiSanId,
        NguoiSuDungId: this.assetDetailModel.nguoiSuDungId,
        MucDichSuDungId: this.emptyGuid,
        NgayBatDau: convertToUTCTime(this.ngayThuHoi),
        NgayKetThuc: null,
        NguoiCapPhatId: this.emptyGuid,
        LyDo: this.lyDoThuHoi,
        CreatedById: this.emptyGuid,
        CreatedDate: new Date,
      }
    ];

    if (listThuHoi.length > 0) {
      this.loading = true;
      this.awaitResult = true;
      this.assetService.taoThuHoiTaiSan(listThuHoi, this.auth.UserId).subscribe(async response => {
        this.loading = false;
        let result = <any>response;
        if (result.statusCode == 200) {
          this.displayThuHoi = false;
          this.showToast('success', 'Thông báo', result.message);
          await this.getMasterData();
          this.awaitResult = false;
        }
        else {
          let lstTaiSanId = result.listAssetId;
          let mes = 'Tài sản đã được thu hồi: ';
          lstTaiSanId.forEach(id => {
            var taisan = this.listTaiSanPhanBo.filter(x => x.taiSanId == id)[0];
            mes += + ' ' + taisan.maTaiSan + ' - ' + taisan.tenTaiSan;
          });
          this.clearToast();
          this.showToast('error', 'Thông báo', mes);
        }
      });
    }
    else {
      this.clearToast();
      this.showToast('error', 'Thông báo', "Không có tài sản thu hồi");
    }
  }

  // Cấp phát tài sản
  closeCapPhat() {
    this.displayCapPhat = false;
  }

  xacNhanCapPhat() {
    if (!this.nhanVienPB) {
      this.clearToast();
      this.showToast('warn', 'Thông báo', 'Vui lòng chọn nhân viên');
      return;
    }
    if (!this.mucDichSuDung) {
      this.clearToast();
      this.showToast('warn', 'Thông báo', 'Vui lòng chọn mục đích sử dụng');
      return;
    }
    if (!this.thoiGianBD) {
      this.clearToast();
      this.showToast('warn', 'Thông báo', 'Ngày sử dụng từ không được để trống');
      return;
    }
    let emp = this.listEmployee.find(x => x.employeeId == this.nhanVienPB.employeeId);
    let mucDichSuDung = this.listMucDichSuDung.find(x => x.categoryId == this.mucDichSuDung.categoryId)
    var listCapPhat = [
      {
        CapPhatTaiSanId: 0,
        TaiSanId: this.assetDetailModel.taiSanId,
        NguoiSuDungId: emp.employeeId,
        MucDichSuDungId: mucDichSuDung.categoryId,
        NgayBatDau: convertToUTCTime(this.thoiGianBD),
        NgayKetThuc: this.thoiGianKT == null ? null : convertToUTCTime(this.thoiGianKT),
        NguoiCapPhatId: this.emptyGuid,
        LyDo: this.lyDoCapPhat,
        CreatedById: this.emptyGuid,
        CreatedDate: new Date,
      }
    ];
    if (listCapPhat.length > 0) {
      this.loading = true;
      this.awaitResult = true;
      this.assetService.taoPhanBoTaiSan(listCapPhat, this.auth.UserId).subscribe(async response => {
        this.loading = false;
        let result = <any>response;
        if (result.statusCode == 200) {
          this.displayCapPhat = false;
          this.showToast('success', 'Thông báo', "Cấp phát thành công.");
          await this.getMasterData();
          this.awaitResult = false;
        }
        else {
          let lstTaiSanId = result.listAssetId;
          let mes = 'Tài sản đã được cấp phát: ';
          lstTaiSanId.forEach(id => {
            var taisan = this.listTaiSanPhanBo.filter(x => x.taiSanId == id)[0];
            mes += + ' ' + taisan.maTaiSan + ' - ' + taisan.tenTaiSan;
          });
          this.clearToast();
          this.showToast('error', 'Thông báo', mes);
        }
      });
    }
    else {
      this.clearToast();
      this.showToast('error', 'Thông báo', "Không có tài sản cấp phát");
    }
  }

  phanBoTaiSan() {
    this.displayCapPhat = true;
    this.nhanVienPB = null;
    this.mucDichSuDung = null;
    this.lyDoCapPhat = null;
    this.minNgayCapPhat = null;
    let obj = this.listTaiSanPhanBo[this.listTaiSanPhanBo.length - 1];
    if (obj != null && obj != undefined) {
      this.minNgayCapPhat = new Date(obj.ngayBatDau);
      this.thoiGianBD = new Date(obj.ngayBatDau);
    }
  }

  cancel() {
    this.router.navigate(['/asset/list']);
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
  date.setMonth(date.getMonth() + Number.parseInt(numOfMonths));

  return date;
}

function getCountMonth(startDate, endDate) {
  if (startDate == null || startDate == undefined)
    return 0;
  else
    return (
      endDate.getMonth() -
      startDate.getMonth() +
      12 * (endDate.getFullYear() - startDate.getFullYear())
    );
}

function ParseStringToFloat(str: string) {
  if (str === "" || str == null) return 0;
  str = str.toString().replace(/,/g, '');
  return parseFloat(str);
}
