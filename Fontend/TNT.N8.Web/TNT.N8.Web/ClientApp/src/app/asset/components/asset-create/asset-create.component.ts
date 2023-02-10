import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { AssetService } from '../../services/asset.service';
import { TaiSanModel } from '../../models/taisan.model';
import { Paginator } from 'primeng';
import { DatePipe } from '@angular/common';
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
class Province {
  public provinceId: string;
  public provinceCode: string;
  public provinceName: string;
  public provinceType: string;
}

@Component({
  selector: 'app-asset-create',
  templateUrl: './asset-create.component.html',
  styleUrls: ['./asset-create.component.css']
})
export class AssetCreateComponent implements OnInit {
  today: Date = new Date();
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  @ViewChild('paginator', { static: true }) paginator: Paginator

  /* End */
  /*Khai báo biến*/
  auth: any = JSON.parse(localStorage.getItem("auth"));
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  /* Action*/
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  /*END*/
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;

  /* Form */
  createAssetForm: FormGroup;
  // Thông tin chung
  maTaiSanControl: FormControl;
  phanLoaiTSControl: FormControl;
  tenTaiSanControl: FormControl;
  ngayVaoSoControl: FormControl;
  donViControl: FormControl;
  maCodeControl: FormControl;
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
  thoiDiemTinhKhauHaoControl: FormControl;
  phuongPhapTinhKhauHaoControl: FormControl;
  thoiDiemKetThucKhauHao: string = '';
  giaTriTinhKhauHaoControl: FormControl;
  /* End */

  colLeft: number = 8;
  isShow: boolean = true;

  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();
  colsKhauHao: any;

  // Dữ liệu masterdata
  listPhanLoaiTS: Array<CategoryModel> = new Array<CategoryModel>();
  listKhuVuc: Array<Province> = [];
  listDonVi: Array<CategoryModel> = new Array<CategoryModel>();
  listHienTrangTS = [
    {
      id: 1, name: 'Đang sử dụng'
    },
    {
      id: 0, name: 'Không sử dụng'
    }];

  listNuocSX: Array<CategoryModel> = new Array<CategoryModel>();
  listHangSX: Array<CategoryModel> = new Array<CategoryModel>();
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

  tiLeKhauHaoTheoThang: number = 0;
  giaTriKhauHaoTheoThang: number = 0;
  tiLeKhauHaoTheoNam: number = 0;
  giaTriKhauHaoTheoNam: number = 0;
  giaTriKhauHaoLuyKe: number = 0;
  giaTriConLai: number = 0;
  defaultNumberType: number = 2;
  constructor(
    private assetService: AssetService,
    private messageService: MessageService,
    private router: Router,
    private getPermission: GetPermission,
    private el: ElementRef,
    private ref: ChangeDetectorRef,
    private encrDecrService: EncrDecrService,
  ) {

  }

  async ngOnInit() {
    this.setTable();
    this.setForm();
    let resource = "ass/asset/create/";
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


      //this.setForm();
      this.getMasterData();
      this.setDefaultValueToForm();
    }
  }
  setDefaultValueToForm() {
    this.soLuongControl.setValue(1);
    this.ngayVaoSoControl.setValue(this.today)
  }

  setForm() {
    this.maTaiSanControl = new FormControl(null, [Validators.required]); // Mã tài sản
    this.khuVucTSControl = new FormControl(null, [Validators.required]); // Địa điểm phân bổ tài sản
    this.phanLoaiTSControl = new FormControl(null, [Validators.required]); // Phân loại tài sản
    this.tenTaiSanControl = new FormControl(null, [Validators.required]); // Tên tài sản
    this.ngayVaoSoControl = new FormControl(null, [Validators.required]); // Ngày vào sổ
    this.donViControl = new FormControl(null, [Validators.required]); // Đơn vị
    this.maCodeControl = new FormControl(null); // Mã code
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
    this.thoiGianKhauHaoControl = new FormControl(0, [Validators.required, Validators.min(0)]); // Thời gian khấu hao (tháng)
    this.giaTriTinhKhauHaoControl = new FormControl(0, [Validators.required, Validators.min(0)]);
    this.thoiDiemTinhKhauHaoControl = new FormControl(null, [Validators.required]); // Thời điểm bắt đầu tính khấu hao
    this.phuongPhapTinhKhauHaoControl = new FormControl(null, [Validators.required]); // Phương pháp khấu khao

    this.createAssetForm = new FormGroup({
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
    let result: any = await this.assetService.getMasterDataAssetForm();
    this.loading = false;
    if (result.statusCode == 200) {
      this.listPhanLoaiTS = result.listPhanLoaiTS;
      this.listDonVi = result.listDonVi;
      this.listNuocSX = result.listNuocSX;
      this.listHangSX = result.listHangSX;
      this.listKhuVuc = result.listProvinceModel;
      let phuongphap = this.listPhuongPhapKhauHao.find(x => x.id == 1);
      this.phuongPhapTinhKhauHaoControl.setValue(phuongphap);
    }
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
  }

  // tạo tài sản
  createAsset(isSaveNew) {
    if (!this.createAssetForm.valid) {
      Object.keys(this.createAssetForm.controls).forEach(key => {
        if (!this.createAssetForm.controls[key].valid) {
          this.createAssetForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.createAssetForm.statusChanges.subscribe((validity: string) => {
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
          this.showToast('success', 'Thông báo', 'Thêm mới tài sản thành công!');
          //Lưu và thêm mới
          if (isSaveNew) {
            this.resetFieldValue();
            let phuongphap = this.listPhuongPhapKhauHao.find(x => x.id == 1);
            this.phuongPhapTinhKhauHaoControl.setValue(phuongphap);
            this.createAssetForm.controls['giaTriTinhKhauHaoControl'].setValue(0);
            this.createAssetForm.controls['thoiGianKhauHaoControl'].setValue(0);
            this.createAssetForm.controls['giaTriTinhKhauHaoControl'].updateValueAndValidity();
            this.createAssetForm.controls['thoiGianKhauHaoControl'].updateValueAndValidity();
            this.ref.detectChanges();
          }
          //Lưu
          else {
            setTimeout(() => {
              this.router.navigate(['/asset/detail', { assetId: this.encrDecrService.set(result.assetId) }]);
            }, 1000)
          }
        }
        else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.message);
        }
        this.awaitResult = false;
      });
    }
  }
  //Function reset toàn bộ các value đã nhập trên form
  resetFieldValue() {
    this.createAssetForm.reset();
    Object.keys(this.createAssetForm.controls).forEach(key => {
      this.createAssetForm.controls[key].markAsUntouched();
    });
    this.isInvalidForm = false;
    this.isOpenNotifiError = false;
    this.thoiDiemKetThucKhauHao = "";
    this.tiLeKhauHaoTheoThang = 0;
    this.giaTriKhauHaoTheoThang = 0;
    this.tiLeKhauHaoTheoNam = 0;
    this.giaTriKhauHaoTheoNam = 0;
    this.giaTriKhauHaoLuyKe = 0;
    this.giaTriConLai = 0;
    this.setDefaultValueToForm();
  }

  mapFormToTaiSanModel(): TaiSanModel {
    let taiSanModel = new TaiSanModel();

    // Mã tài sản
    taiSanModel.maTaiSan = this.maTaiSanControl.value;

    // Phân loại tài sản
    let phanLoaiTS = this.createAssetForm.get('phanLoaiTSControl').value
    taiSanModel.phanLoaiTaiSanId = phanLoaiTS ? phanLoaiTS.categoryId : this.emptyGuid;

    // Khu vục tài sản
    let khuVucTS = this.createAssetForm.get('khuVucTSControl').value
    taiSanModel.khuVucTaiSanId = khuVucTS ? khuVucTS.provinceId : this.emptyGuid;

    // Đơn vị
    let donVi = this.createAssetForm.get('donViControl').value
    taiSanModel.donViTinhId = donVi ? donVi.categoryId : this.emptyGuid;

    // Số lượng
    taiSanModel.soLuong = this.soLuongControl.value;
    // Tên tài sản
    taiSanModel.tenTaiSan = this.tenTaiSanControl.value;

    // Ngày vào sổ
    let ngayVS = this.createAssetForm.get('ngayVaoSoControl').value ? convertToUTCTime(this.createAssetForm.get('ngayVaoSoControl').value) : null;
    taiSanModel.ngayVaoSo = ngayVS;

    // Mô tả
    taiSanModel.moTa = this.moTaControl.value;

    // Mã code
    taiSanModel.maCode = this.maTaiSanControl.value;

    // Hiện trạng tài sản
    ///let hientrang = this.createAssetForm.get('hienTrangTaiSanControl').value
    taiSanModel.hienTrangTaiSan = 0;//hientrang ? hientrang.id : 0;

    taiSanModel.soSerial = this.serialControl.value;
    taiSanModel.namSX = this.namSXControl.value;

    // Ngày mua
    let ngayMua = this.createAssetForm.get('ngayMuaControl').value ? convertToUTCTime(this.createAssetForm.get('ngayMuaControl').value) : null;
    taiSanModel.ngayMua = ngayMua;

    // Model
    taiSanModel.model = this.modelControl.value;

    // Nước sản xuất
    let nuocSX = this.createAssetForm.get('nuocSXControl').value
    taiSanModel.nuocSXId = nuocSX ? nuocSX.categoryId : this.emptyGuid;

    // Số hiệu
    taiSanModel.soHieu = this.soHieuControl.value;

    // Hãng sản xuất
    let hangSX = this.createAssetForm.get('hangSXControl').value
    taiSanModel.hangSXId = hangSX ? hangSX.categoryId : this.emptyGuid;

    // Thời hạn bảo hành
    taiSanModel.thoiHanBaoHanh = this.thoiHanBHControl.value;

    // Thông tin nơi mua
    taiSanModel.thongTinNoiMua = this.thongTinNoiMuaControl.value;

    // Thông tin nơi bảo hành/ bảo trì
    taiSanModel.thongTinNoiBaoHanh = this.thongTinNoiBaoHanhControl.value;

    // Bảo dưỡng định kỳ
    taiSanModel.baoDuongDinhKy = this.baoDuongDinhKyControl.value;

    // KHẤU HAO
    //Thời điểm bắt đầu tính khấu hao

    let ngayTinhKH = this.createAssetForm.get('thoiDiemTinhKhauHaoControl').value ? convertToUTCTime(this.createAssetForm.get('thoiDiemTinhKhauHaoControl').value) : null;
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

  tinhThoiGianKhauHao() {

    if (this.thoiGianKhauHaoControl.value != null && this.thoiDiemTinhKhauHaoControl.value != null) {
      let ngayTinhKH = this.createAssetForm.get('thoiDiemTinhKhauHaoControl').value ? convertToUTCTime(this.createAssetForm.get('thoiDiemTinhKhauHaoControl').value) : null;

      var datePipe = new DatePipe("en-US");
      const result = addMonths(this.thoiGianKhauHaoControl.value, ngayTinhKH);
      this.thoiDiemKetThucKhauHao = datePipe.transform(result.setDate(result.getDate() - 1), 'MM/yyyy');
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

    //Giá trị khấu hao lũy kế = Gía trị tính khấu hao  - [ giá trị khấu hao theo tháng * (tháng hiện tại - tháng bắt đầu tính khấu hao)]
    this.giaTriKhauHaoLuyKe = Number((this.giaTriKhauHaoTheoThang * getCountMonth(this.thoiDiemTinhKhauHaoControl.value, new Date())).toFixed(2)) < 0 ? 0 : Number((this.giaTriKhauHaoTheoThang * getCountMonth(this.thoiDiemTinhKhauHaoControl.value, new Date())).toFixed(2));

    //Giá trị còn lại = Giá trị tính khấu hao - Giá trị khấu hao lũy kế
    this.giaTriConLai = Number((giaTriTinhKHao - this.giaTriKhauHaoLuyKe).toFixed(2));
    this.ref.detectChanges();
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
