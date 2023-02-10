import { CauHinhOtCaNgay } from './../../../models/cau-hinh-ot-ca-ngay.model';
import { Router } from "@angular/router";
import { Component, OnInit, ChangeDetectorRef, ViewChild, Input } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ConfirmationService, MessageService } from "primeng/api";

import { SalaryService } from '../../../services/salary.service';
import { CommonService } from '../../../../shared/services/common.service';
import { FormatDateService } from '../../../../shared/services/formatDate.services';

import { GetPermission } from "../../../../shared/permission/get-permission";
import { CaLamViec } from '../../../models/ca-lam-viec.model';
import { CaLamViecChiTiet } from '../../../models/ca-lam-viec-chi-tiet.model';
import { CauHinhNghiLe } from '../../../models/cau-hinh-nghi-le.model';
import { CauHinhNghiLeChiTiet } from '../../../models/cau-hinh-nghi-le-chi-tiet.model';
import { CauHinhOt } from '../../../models/cau-hinh-ot.model';
import { CauHinhGiamTru } from '../../../models/cau-hinh-giam-tru.model';
import { KinhPhiCongDoan } from '../../../models/kinh-phi-cong-doan.model';
import { CongThucTinhLuong } from '../../../models/cong-thuc-tinh-luong.model';
import { CauHinhChamCongOt } from '../../../models/cau-hinh-cham-cong-ot.model';

@Component({
  selector: "app-luong",
  templateUrl: "./luong.component.html",
  styleUrls: ["./luong.component.css"],
})
export class LuongComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới
  systemParameterList = JSON.parse(localStorage.getItem("systemParameterList"));
  defaultLimitedFileSize = Number(
    this.systemParameterList.find((systemParameter) => systemParameter.systemKey == "LimitedFileSize").systemValueString
  ) * 1024 * 1024;
  strAcceptFile: string = "image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt";
  emptyGuid: string = "00000000-0000-0000-0000-000000000000";

  @Input() actionAdd: boolean;
  @Input() actionEdit: boolean;
  @Input() actionDelete: boolean;

  showCaLamViec: boolean = false;
  showCauHinhOt: boolean = false;
  showCauHinhGiamTru: boolean = false;
  showCauHinhNghiLe: boolean = false;
  showCongThucTinhLuong: boolean = false;
  showCauHinhTtncn: boolean = false;

  /* Ca làm việc */
  colsCaLamViec: Array<any> = [];
  listCaLamViec: Array<any> = [];
  listLoaiCaLamViec: Array<any> = [];
  listNgayLamViecTrongTuan: Array<any> = [];

  caLamViecFormGroup: FormGroup;
  caLamViecIdControl: FormControl;
  loaiCaLamViecControl: FormControl;
  gioVaoControl: FormControl;
  gioRaControl: FormControl;
  thoiGianKetThucCaControl: FormControl;
  ngayLamViecTrongTuanControl: FormControl;

  /* Nghỉ lễ */
  colsNghiLe: Array<any> = [];
  listCauHinhNghiLe: Array<any> = [];

  cauHinhNghiLeFormGroup: FormGroup;
  nghiLeIdControl: FormControl;
  soNamControl: FormControl;
  ngayNghiLeControl: FormControl;
  ngayNghiBuControl: FormControl;
  ngayLamBuControl: FormControl;

  /* Cấu hình Ot */
  colsCauHinhOt: Array<any> = [];
  listLoaiOt: Array<any> = [];
  listCauHinhOt: Array<any> = [];

  cauHinhOtFormGroup: FormGroup;
  cauHinhOtIdControl: FormControl;
  loaiOtControl: FormControl;
  tyLeHuongControl: FormControl;

  /* Cấu hình giảm trừ */
  colsGiamTru: Array<any> = [];
  listLoaiGiamTru: Array<any> = [];
  listCauHinhGiamTru: Array<any> = [];

  cauHinhGiamTruFormGroup: FormGroup;
  cauHinhGiamTruIdControl: FormControl;
  loaiGiamTruControl: FormControl;
  mucGiamTruControl: FormControl;

  /* Kinh phí công đoàn */
  kinhPhiCongDoanClone = new KinhPhiCongDoan();
  kinhPhiCongDoanFormGroup: FormGroup;
  kinhPhiCongDoanIdControl: FormControl;
  mucDongNhanVienControl: FormControl;
  mucDongCongTyControl: FormControl;

  /* Công thức tính lương */
  congThucTinhLuongClone = new CongThucTinhLuong();
  listTokenTinhLuong: Array<any> = [];

  congThucTinhLuongFormGroup: FormGroup;
  congThucTinhLuongIdControl: FormControl;
  congThucControl: FormControl;

  /* Cấu hình chấm công OT */
  cauHinhChamCongOtClone = new CauHinhChamCongOt();

  cauHinhChamCongOtFormGroup: FormGroup;
  cauHinhChamCongOtId: FormControl;
  soPhutControl: FormControl;

  /* Cấu hình OT cả ngày */
  cauHinhOtCaNgayClone = new CauHinhOtCaNgay();

  cauHinhOtCaNgayFormGroup: FormGroup;
  cauHinhOtCaNgayIdControl: FormControl;
  gioVaoSangControl: FormControl;
  gioRaSangControl: FormControl;
  gioKetThucSangControl: FormControl;
  gioVaoChieuControl: FormControl;
  gioRaChieuControl: FormControl;
  gioKetThucChieuControl: FormControl;

  /* Cấu hình thuế TNCN */
  colsTtncn: Array<any> = [];
  listCauHinhThueTncn: Array<any> = [];

  ttncnFormGroup: FormGroup;
  cauHinhThueTncnIdControl: FormControl;
  soTienTuControl: FormControl;
  soTienDenControl: FormControl;
  phanTramThueControl: FormControl;
  soBiTruTheoCongThucControl: FormControl;

  refreshNote: number = 0;

  constructor(
    private router: Router,
    private messageService: MessageService,
    private salaryService: SalaryService,
    private confirmationService: ConfirmationService,
    private def: ChangeDetectorRef,
    private getPermission: GetPermission,
    private commonService: CommonService,
    private formatDateService: FormatDateService
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.setTable();
    this.getMasterData();
  }

  initForm() {
    /* Ca làm việc */
    this.caLamViecIdControl = new FormControl(null);
    this.loaiCaLamViecControl = new FormControl(null, [Validators.required]);
    this.gioVaoControl = new FormControl(null, [Validators.required]);
    this.gioRaControl = new FormControl(null, [Validators.required]);
    this.thoiGianKetThucCaControl = new FormControl(null, [Validators.required]);
    this.ngayLamViecTrongTuanControl = new FormControl([], [Validators.required]);

    this.caLamViecFormGroup = new FormGroup({
      caLamViecIdControl: this.caLamViecIdControl,
      loaiCaLamViecControl: this.loaiCaLamViecControl,
      gioVaoControl: this.gioVaoControl,
      gioRaControl: this.gioRaControl,
      thoiGianKetThucCaControl: this.thoiGianKetThucCaControl,
      ngayLamViecTrongTuanControl: this.ngayLamViecTrongTuanControl,
    });

    /* Cấu hình nghỉ lễ */
    this.nghiLeIdControl = new FormControl(null);
    this.soNamControl = new FormControl(null, [Validators.required]);
    this.ngayNghiLeControl = new FormControl(null);
    this.ngayNghiBuControl = new FormControl(null);
    this.ngayLamBuControl = new FormControl(null);

    this.cauHinhNghiLeFormGroup = new FormGroup({
      nghiLeIdControl: this.nghiLeIdControl,
      soNamControl: this.soNamControl,
      ngayNghiLeControl: this.ngayNghiLeControl,
      ngayNghiBuControl: this.ngayNghiBuControl,
      ngayLamBuControl: this.ngayLamBuControl
    });

    /* Cấu hình OT */
    this.cauHinhOtIdControl = new FormControl(null);
    this.loaiOtControl = new FormControl(null, [Validators.required]);
    this.tyLeHuongControl = new FormControl(null, [Validators.required]);

    this.cauHinhOtFormGroup = new FormGroup({
      cauHinhOtIdControl: this.cauHinhOtIdControl,
      loaiOtControl: this.loaiOtControl,
      tyLeHuongControl: this.tyLeHuongControl,
    });

    /* Cấu hình Giảm trừ */
    this.cauHinhGiamTruIdControl = new FormControl(null);
    this.loaiGiamTruControl = new FormControl(null, [Validators.required]);
    this.mucGiamTruControl = new FormControl(null, [Validators.required]);

    this.cauHinhGiamTruFormGroup = new FormGroup({
      cauHinhGiamTruIdControl: this.cauHinhGiamTruIdControl,
      loaiGiamTruControl: this.loaiGiamTruControl,
      mucGiamTruControl: this.mucGiamTruControl,
    });

    /* Kinh phí công đoàn */
    this.kinhPhiCongDoanIdControl = new FormControl(null);
    this.mucDongNhanVienControl = new FormControl(0, [Validators.required]);
    this.mucDongCongTyControl = new FormControl(0, [Validators.required]);

    this.kinhPhiCongDoanFormGroup = new FormGroup({
      kinhPhiCongDoanIdControl: this.kinhPhiCongDoanIdControl,
      mucDongNhanVienControl: this.mucDongNhanVienControl,
      mucDongCongTyControl: this.mucDongCongTyControl
    });

    this.kinhPhiCongDoanFormGroup.disable();

    /* Công thức tính lương */
    this.congThucTinhLuongIdControl = new FormControl(null);
    this.congThucControl = new FormControl(null, [Validators.required]);

    this.congThucTinhLuongFormGroup = new FormGroup({
      congThucTinhLuongIdControl: this.congThucTinhLuongIdControl,
      congThucControl: this.congThucControl
    });

    /* Cấu hình chấm công OT */
    this.cauHinhChamCongOtId = new FormControl(null);
    this.soPhutControl = new FormControl(0, [Validators.required]);

    this.cauHinhChamCongOtFormGroup = new FormGroup({
      cauHinhChamCongOtId: this.cauHinhChamCongOtId,
      soPhutControl: this.soPhutControl
    });

    this.cauHinhChamCongOtFormGroup.disable();

    /* Cấu hình OT cả ngày */
    this.cauHinhOtCaNgayIdControl = new FormControl(null);
    this.gioVaoSangControl = new FormControl(null, [Validators.required]);
    this.gioRaSangControl = new FormControl(null, [Validators.required]);
    this.gioKetThucSangControl = new FormControl(null, [Validators.required]);
    this.gioVaoChieuControl = new FormControl(null, [Validators.required]);
    this.gioRaChieuControl = new FormControl(null, [Validators.required]);
    this.gioKetThucChieuControl = new FormControl(null, [Validators.required]);

    this.cauHinhOtCaNgayFormGroup = new FormGroup({
      cauHinhOtCaNgayIdControl: this.cauHinhOtCaNgayIdControl,
      gioVaoSangControl: this.gioVaoSangControl,
      gioRaSangControl: this.gioRaSangControl,
      gioKetThucSangControl: this.gioKetThucSangControl,
      gioVaoChieuControl: this.gioVaoChieuControl,
      gioRaChieuControl: this.gioRaChieuControl,
      gioKetThucChieuControl: this.gioKetThucChieuControl
    });

    this.cauHinhOtCaNgayFormGroup.disable();

    /* Thuế TNCN */
    this.cauHinhThueTncnIdControl = new FormControl(null);
    this.soTienTuControl = new FormControl(0, [Validators.required]);
    this.soTienDenControl = new FormControl(0, [Validators.required]);
    this.phanTramThueControl = new FormControl(0, [Validators.required]);
    this.soBiTruTheoCongThucControl = new FormControl(0, [Validators.required]);

    this.ttncnFormGroup = new FormGroup({
      cauHinhThueTncnIdControl: this.cauHinhThueTncnIdControl,
      soTienTuControl: this.soTienTuControl,
      soTienDenControl: this.soTienDenControl,
      phanTramThueControl: this.phanTramThueControl,
      soBiTruTheoCongThucControl: this.soBiTruTheoCongThucControl
    });
  }

  setTable() {
    this.colsCaLamViec = [
      { field: "tenLoaiCaLamViec", header: "Ca", width: "10%", textAlign: "left" },
      { field: "gioVao", header: "Giờ vào", width: "10%", textAlign: "center" },
      { field: "gioRa", header: "Giờ ra", width: "10%", textAlign: "center" },
      { field: "thoiGianKetThucCa", header: "Thời gian kết thúc ca", width: "20%", textAlign: "center" },
      { field: "ngayLamViecTrongTuanText", header: "Ngày làm việc trong tuần", width: "30%", textAlign: "left" },
      { field: "action", header: "Thao tác", width: "20%", textAlign: "center" },
    ];

    this.colsNghiLe = [
      { field: "soNam", header: "Năm", width: "20%", textAlign: "center" },
      { field: "ngayNghiLe", header: "Ngày nghỉ lễ", width: "20%", textAlign: "left" },
      { field: "ngayNghiBu", header: "Ngày nghỉ bù", width: "20%", textAlign: "left" },
      { field: "ngayLamBu", header: "Ngày làm bù", width: "20%", textAlign: "left" },
      { field: "action", header: "Thao tác", width: "20%", textAlign: "center" },
    ];

    this.colsCauHinhOt = [
      { field: "stt", header: "STT", width: "20%", textAlign: "center" },
      { field: "tenLoaiOt", header: "Loại OT", width: "40%", textAlign: "left" },
      { field: "tyLeHuong", header: "Mức lương (%)", width: "20%", textAlign: "right" },
      { field: "action", header: "Thao tác", width: "20%", textAlign: "center" },
    ];

    this.colsGiamTru = [
      { field: "stt", header: "STT", width: "20%", textAlign: "center" },
      { field: "tenLoaiGiamTru", header: "Loại giảm trừ", width: "40%", textAlign: "left" },
      { field: "mucGiamTru", header: "Mức giảm trừ (VNĐ)", width: "20%", textAlign: "right" },
      { field: "action", header: "Thao tác", width: "20%", textAlign: "center" },
    ];

    this.colsTtncn = [
      { field: "stt", header: "STT", width: "20%", textAlign: "center" },
      { field: "tuDen", header: "Phần thu nhập tính thuế/tháng (VNĐ)", width: "20%", textAlign: "left" },
      { field: "phanTramThue", header: "Thuế suất (%)", width: "20%", textAlign: "right" },
      { field: "soBiTruTheoCongThuc", header: "Tiền thuế phần thu nhập lũy tiến", width: "20%", textAlign: "right" },
      { field: "action", header: "Thao tác", width: "20%", textAlign: "center" },
    ];
  }

  // Lấy dữ liệu ban đầu
  async getMasterData() {
    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.getMasterCauHinhLuong();
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.listCaLamViec = result.listCaLamViec.map(x => {
      x.gioVao = x.gioVao.slice(0, x.gioVao.lastIndexOf(':'));
      x.gioRa = x.gioRa.slice(0, x.gioRa.lastIndexOf(':'));
      x.thoiGianKetThucCa = x.thoiGianKetThucCa.slice(0, x.thoiGianKetThucCa.lastIndexOf(':'));

      return x;
    });
    this.listLoaiCaLamViec = result.listLoaiCaLamViec;
    this.listNgayLamViecTrongTuan = result.listNgayLamViecTrongTuan;
    this.listCauHinhNghiLe = result.listCauHinhNghiLe;
    this.listLoaiOt = result.listLoaiOt;
    this.listCauHinhOt = result.listCauHinhOt;
    this.listLoaiGiamTru = result.listLoaiGiamTru;
    this.listCauHinhGiamTru = result.listCauHinhGiamTru;

    if (result.kinhPhiCongDoan) {
      this.kinhPhiCongDoanClone = result.kinhPhiCongDoan;

      this.kinhPhiCongDoanIdControl.setValue(this.kinhPhiCongDoanClone.kinhPhiCongDoanId);
      this.mucDongNhanVienControl.setValue(this.kinhPhiCongDoanClone.mucDongNhanVien);
      this.mucDongCongTyControl.setValue(this.kinhPhiCongDoanClone.mucDongCongTy);
    }

    this.listTokenTinhLuong = result.listTokenTinhLuong;
    if (result.congThucTinhLuong) {
      this.congThucTinhLuongClone = result.congThucTinhLuong;

      this.congThucTinhLuongIdControl.setValue(this.congThucTinhLuongClone.congThucTinhLuongId);
      this.congThucControl.setValue(this.congThucTinhLuongClone.congThuc);
    }

    if (result.cauHinhChamCongOt) {
      this.cauHinhChamCongOtClone = result.cauHinhChamCongOt;

      this.cauHinhChamCongOtId.setValue(this.cauHinhChamCongOtClone.cauHinhChamCongOtId);
      this.soPhutControl.setValue(this.cauHinhChamCongOtClone.soPhut);
    }

    if (result.cauHinhOtCaNgay) {
      this.cauHinhOtCaNgayClone = result.cauHinhOtCaNgay;

      this.cauHinhOtCaNgayIdControl.setValue(this.cauHinhOtCaNgayClone.cauHinhOtCaNgayId);

      let gioVaoSang = this.cauHinhOtCaNgayClone.gioVaoSang.slice(0, this.cauHinhOtCaNgayClone.gioVaoSang.lastIndexOf(':'));
      this.gioVaoSangControl.setValue(this.commonService.convertTimeSpanToDate(gioVaoSang));

      let gioRaSang = this.cauHinhOtCaNgayClone.gioRaSang.slice(0, this.cauHinhOtCaNgayClone.gioRaSang.lastIndexOf(':'));
      this.gioRaSangControl.setValue(this.commonService.convertTimeSpanToDate(gioRaSang));

      let gioKetThucSang = this.cauHinhOtCaNgayClone.gioKetThucSang.slice(0, this.cauHinhOtCaNgayClone.gioKetThucSang.lastIndexOf(':'));
      this.gioKetThucSangControl.setValue(this.commonService.convertTimeSpanToDate(gioKetThucSang));

      let gioVaoChieu = this.cauHinhOtCaNgayClone.gioVaoChieu.slice(0, this.cauHinhOtCaNgayClone.gioVaoChieu.lastIndexOf(':'));
      this.gioVaoChieuControl.setValue(this.commonService.convertTimeSpanToDate(gioVaoChieu));

      let gioRaChieu = this.cauHinhOtCaNgayClone.gioRaChieu.slice(0, this.cauHinhOtCaNgayClone.gioRaChieu.lastIndexOf(':'));
      this.gioRaChieuControl.setValue(this.commonService.convertTimeSpanToDate(gioRaChieu));

      let gioKetThucChieu = this.cauHinhOtCaNgayClone.gioKetThucChieu.slice(0, this.cauHinhOtCaNgayClone.gioKetThucChieu.lastIndexOf(':'));
      this.gioKetThucChieuControl.setValue(this.commonService.convertTimeSpanToDate(gioKetThucChieu));
    }

    this.listCauHinhThueTncn = result.listCauHinhThueTncn;
  }

  addRow(mode: string) {
    switch (mode) {
      case "CLV":
        this.showCaLamViec = true;
        this.caLamViecFormGroup.reset();
        this.def.detectChanges();
        break;
      case "NL":
        this.showCauHinhNghiLe = true;
        this.cauHinhNghiLeFormGroup.reset();
        this.def.detectChanges();
        break;
      case "OT":
        this.showCauHinhOt = true;
        this.cauHinhOtFormGroup.reset();
        this.def.detectChanges();
        break;
      case "GT":
        this.showCauHinhGiamTru = true;
        this.cauHinhGiamTruFormGroup.reset();
        this.def.detectChanges();
        break;
      case "CTTL":
        this.showCongThucTinhLuong = true;
        this.congThucTinhLuongFormGroup.reset();
        this.congThucTinhLuongIdControl.setValue(this.congThucTinhLuongClone.congThucTinhLuongId);
        this.congThucControl.setValue(this.congThucTinhLuongClone.congThuc);
        this.def.detectChanges();
        break;
      case "TTNCN":
        this.showCauHinhTtncn = true;
        this.ttncnFormGroup.reset();
        this.def.detectChanges();
        break;
    }
  }

  closeDetailItem(type: any) {
    switch (type) {
      case "CLV":
        this.showCaLamViec = false;
        this.def.detectChanges();
        break;
      case "NL":
        this.showCauHinhNghiLe = false;
        this.def.detectChanges();
        break;
      case "OT":
        this.showCauHinhOt = false;
        this.def.detectChanges();
        break;
      case "GT":
        this.showCauHinhGiamTru = false;
        this.def.detectChanges();
        break;
      case "CTTL":
        this.showCongThucTinhLuong = false;
        this.def.detectChanges();
        break;
      case "TTNCN":
        this.showCauHinhTtncn = false;
        this.def.detectChanges();
        break;
    }
  }

  async saveCaLamViec() {
    if (!this.caLamViecFormGroup.valid) {
      Object.keys(this.caLamViecFormGroup.controls).forEach(key => {
        if (!this.caLamViecFormGroup.controls[key].valid) {
          this.caLamViecFormGroup.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let caLamViec = new CaLamViec();
    caLamViec.caLamViecId = this.caLamViecIdControl.value;
    caLamViec.loaiCaLamViecId = this.loaiCaLamViecControl.value.value;
    caLamViec.gioVao = this.commonService.convertDateToTimeSpan(this.gioVaoControl.value);
    caLamViec.gioRa = this.commonService.convertDateToTimeSpan(this.gioRaControl.value);
    caLamViec.thoiGianKetThucCa = this.commonService.convertDateToTimeSpan(this.thoiGianKetThucCaControl.value);

    let listCaLamViecChiTiet = [];
    let listSelectedNgayLamViec = this.ngayLamViecTrongTuanControl.value;
    listSelectedNgayLamViec.forEach(item => {
      let caLamViecChiTiet = new CaLamViecChiTiet();
      caLamViecChiTiet.ngayTrongTuan = item.value;

      listCaLamViecChiTiet.push(caLamViecChiTiet);
    });

    caLamViec.listCaLamViecChiTiet = listCaLamViecChiTiet;

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.createOrUpdateCaLamViec(caLamViec);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showCaLamViec = false;
    this.showMessage("success", result.messageCode);
    this.getMasterData();
  }

  suaCaLamViec(data: CaLamViec) {
    this.showCaLamViec = true;
    this.caLamViecFormGroup.reset();
    this.def.detectChanges();

    this.caLamViecIdControl.setValue(data.caLamViecId);

    let loaiCa = this.listLoaiCaLamViec.find(x => x.value == data.loaiCaLamViecId);
    this.loaiCaLamViecControl.setValue(loaiCa);

    this.gioVaoControl.setValue(this.commonService.convertTimeSpanToDate(data.gioVao));
    this.gioRaControl.setValue(this.commonService.convertTimeSpanToDate(data.gioRa));
    this.thoiGianKetThucCaControl.setValue(this.commonService.convertTimeSpanToDate(data.thoiGianKetThucCa));

    let listSelectedNgayLamViec = this.listNgayLamViecTrongTuan.filter(x =>
      data.listCaLamViecChiTiet.map(y => y.ngayTrongTuan).includes(x.value));

    this.ngayLamViecTrongTuanControl.setValue(listSelectedNgayLamViec);
  }

  xoaCaLamViec(data: CaLamViec) {
    this.confirmationService.confirm({
      message: `Dữ liệu không thể hoàn tác, bạn chắc chắn muốn xóa cấu hình này?`,
      accept: async () => {
        this.loading = true;
        this.awaitResult = true;
        let result: any = await this.salaryService.deleteCauHinhCaLamViec(data.caLamViecId);

        if (result.statusCode != 200) {
          this.loading = false;
          this.awaitResult = false;
          this.showMessage("error", result.messageCode);
          return;
        }

        this.showMessage("success", result.messageCode);
        this.getMasterData();
      },
    });
  }

  async saveNghiLe() {
    if (!this.cauHinhNghiLeFormGroup.valid) {
      Object.keys(this.cauHinhNghiLeFormGroup.controls).forEach(key => {
        if (!this.cauHinhNghiLeFormGroup.controls[key].valid) {
          this.cauHinhNghiLeFormGroup.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let listSelectedNghiLe = this.ngayNghiLeControl.value;
    let listSelectedNghiBu = this.ngayNghiBuControl.value;
    let listSelectedLamBu = this.ngayLamBuControl.value;

    if (!listSelectedNghiLe && !listSelectedNghiBu && !listSelectedLamBu) {
      this.showMessage('warn', 'Bạn cần nhập ít nhất 1 ngày');
      return;
    }

    let cauHinhNghiLe = new CauHinhNghiLe();
    cauHinhNghiLe.nghiLeId = this.nghiLeIdControl.value;
    cauHinhNghiLe.soNam = this.soNamControl.value;

    let listCauHinhNghiLeChiTiet = [];

    //Nghỉ lễ
    listSelectedNghiLe?.forEach(item => {
      let cauHinhNghiLeChiTiet = new CauHinhNghiLeChiTiet();
      cauHinhNghiLeChiTiet.loaiNghiLe = 1;
      cauHinhNghiLeChiTiet.ngay = this.formatDateService.convertToUTCTime(item);

      listCauHinhNghiLeChiTiet.push(cauHinhNghiLeChiTiet);
    });

    //Nghỉ bù
    listSelectedNghiBu?.forEach(item => {
      let cauHinhNghiLeChiTiet = new CauHinhNghiLeChiTiet();
      cauHinhNghiLeChiTiet.loaiNghiLe = 2;
      cauHinhNghiLeChiTiet.ngay = this.formatDateService.convertToUTCTime(item);

      listCauHinhNghiLeChiTiet.push(cauHinhNghiLeChiTiet);
    });

    //Làm bù
    listSelectedLamBu?.forEach(item => {
      let cauHinhNghiLeChiTiet = new CauHinhNghiLeChiTiet();
      cauHinhNghiLeChiTiet.loaiNghiLe = 3;
      cauHinhNghiLeChiTiet.ngay = this.formatDateService.convertToUTCTime(item);

      listCauHinhNghiLeChiTiet.push(cauHinhNghiLeChiTiet);
    });

    cauHinhNghiLe.listCauHinhNghiLeChiTiet = listCauHinhNghiLeChiTiet;

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.createOrUpdateCauHinhNghiLe(cauHinhNghiLe);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showCauHinhNghiLe = false;
    this.showMessage("success", result.messageCode);
    this.getMasterData();
  }

  suaCauHinhNghiLe(data: CauHinhNghiLe) {
    this.showCauHinhNghiLe = true;
    this.cauHinhNghiLeFormGroup.reset();
    this.def.detectChanges();

    this.nghiLeIdControl.setValue(data.nghiLeId);
    this.soNamControl.setValue(data.soNam);

    let listSelectedNghiLe = data.listCauHinhNghiLeChiTiet.filter(x => x.loaiNghiLe == 1).map(x => new Date(x.ngay));
    let listSelectedNghiBu = data.listCauHinhNghiLeChiTiet.filter(x => x.loaiNghiLe == 2).map(x => new Date(x.ngay));
    let listSelectedLamBu = data.listCauHinhNghiLeChiTiet.filter(x => x.loaiNghiLe == 3).map(x => new Date(x.ngay));

    this.ngayNghiLeControl.setValue(listSelectedNghiLe.length ? listSelectedNghiLe : null);
    this.ngayNghiBuControl.setValue(listSelectedNghiBu.length ? listSelectedNghiBu : null);
    this.ngayLamBuControl.setValue(listSelectedLamBu.length ? listSelectedLamBu : null);
  }

  xoaCauHinhNghiLe(data: CauHinhNghiLe) {
    this.confirmationService.confirm({
      message: `Dữ liệu không thể hoàn tác, bạn chắc chắn muốn xóa cấu hình này?`,
      accept: async () => {
        this.loading = true;
        this.awaitResult = true;
        let result: any = await this.salaryService.deleteCauHinhNghiLe(data.nghiLeId);

        if (result.statusCode != 200) {
          this.loading = false;
          this.awaitResult = false;
          this.showMessage("error", result.messageCode);
          return;
        }

        this.showMessage("success", result.messageCode);
        this.getMasterData();
      },
    });
  }

  async saveCauHinhOt() {
    if (!this.cauHinhOtFormGroup.valid) {
      Object.keys(this.cauHinhOtFormGroup.controls).forEach(key => {
        if (!this.cauHinhOtFormGroup.controls[key].valid) {
          this.cauHinhOtFormGroup.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let cauHinhOt = new CauHinhOt();
    cauHinhOt.cauHinhOtId = this.cauHinhOtIdControl.value;
    cauHinhOt.loaiOtId = this.loaiOtControl.value.categoryId;
    cauHinhOt.tyLeHuong = this.commonService.convertStringToNumber(this.tyLeHuongControl.value);

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.createOrUpdateCauHinhOt(cauHinhOt);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showCauHinhOt = false;
    this.showMessage("success", result.messageCode);
    this.getMasterData();
  }

  suaCauHinhOt(data: CauHinhOt) {
    this.showCauHinhOt = true;
    this.cauHinhOtFormGroup.reset();
    this.def.detectChanges();

    this.cauHinhOtIdControl.setValue(data.cauHinhOtId);

    let loaiOt = this.listLoaiOt.find(x => x.categoryId == data.loaiOtId);
    this.loaiOtControl.setValue(loaiOt);

    this.tyLeHuongControl.setValue(data.tyLeHuong.toString());
  }

  xoaCauHinhOt(data: CauHinhOt) {
    this.confirmationService.confirm({
      message: `Dữ liệu không thể hoàn tác, bạn chắc chắn muốn xóa cấu hình này?`,
      accept: async () => {
        this.loading = true;
        this.awaitResult = true;
        let result: any = await this.salaryService.deleteCauHinhOt(data.cauHinhOtId);

        if (result.statusCode != 200) {
          this.loading = false;
          this.awaitResult = false;
          this.showMessage("error", result.messageCode);
          return;
        }

        this.showMessage("success", result.messageCode);
        this.getMasterData();
      },
    });
  }

  async saveCauHinhGiamTru() {
    if (!this.cauHinhGiamTruFormGroup.valid) {
      Object.keys(this.cauHinhGiamTruFormGroup.controls).forEach(key => {
        if (!this.cauHinhGiamTruFormGroup.controls[key].valid) {
          this.cauHinhGiamTruFormGroup.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let cauHinhGiamTru = new CauHinhGiamTru();
    cauHinhGiamTru.cauHinhGiamTruId = this.cauHinhGiamTruIdControl.value;
    cauHinhGiamTru.loaiGiamTruId = this.loaiGiamTruControl.value.value;
    cauHinhGiamTru.mucGiamTru = this.commonService.convertStringToNumber(this.mucGiamTruControl.value);

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.createOrUpdateCauHinhGiamTru(cauHinhGiamTru);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showCauHinhGiamTru = false;
    this.showMessage("success", result.messageCode);
    this.getMasterData();
  }

  suaCauHinhGiamTru(data: CauHinhGiamTru) {
    this.showCauHinhGiamTru = true;
    this.cauHinhGiamTruFormGroup.reset();
    this.def.detectChanges();

    this.cauHinhGiamTruIdControl.setValue(data.cauHinhGiamTruId);

    let loaiGiamTru = this.listLoaiGiamTru.find(x => x.value == data.loaiGiamTruId);
    this.loaiGiamTruControl.setValue(loaiGiamTru);

    this.mucGiamTruControl.setValue(data.mucGiamTru.toString());
  }

  xoaCauHinhGiamTru(data: CauHinhGiamTru) {
    this.confirmationService.confirm({
      message: `Dữ liệu không thể hoàn tác, bạn chắc chắn muốn xóa cấu hình này?`,
      accept: async () => {
        this.loading = true;
        this.awaitResult = true;
        let result: any = await this.salaryService.deleteCauHinhGiamTru(data.cauHinhGiamTruId);

        if (result.statusCode != 200) {
          this.loading = false;
          this.awaitResult = false;
          this.showMessage("error", result.messageCode);
          return;
        }

        this.showMessage("success", result.messageCode);
        this.getMasterData();
      },
    });
  }

  suaKinhPhiCongDoan() {
    this.kinhPhiCongDoanFormGroup.enable();
  }

  async saveKinhPhiCongDoan() {
    if (!this.kinhPhiCongDoanFormGroup.valid) {
      Object.keys(this.kinhPhiCongDoanFormGroup.controls).forEach(key => {
        if (!this.kinhPhiCongDoanFormGroup.controls[key].valid) {
          this.kinhPhiCongDoanFormGroup.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let kinhPhiCongDoan = new KinhPhiCongDoan();
    kinhPhiCongDoan.kinhPhiCongDoanId = this.kinhPhiCongDoanIdControl.value;
    kinhPhiCongDoan.mucDongNhanVien = this.commonService.convertStringToNumber(this.mucDongNhanVienControl.value.toString());
    kinhPhiCongDoan.mucDongCongTy = this.commonService.convertStringToNumber(this.mucDongCongTyControl.value.toString());

    if (kinhPhiCongDoan.mucDongNhanVien > 100 || kinhPhiCongDoan.mucDongCongTy > 100) {
      this.showMessage('warn', 'Số phần trăm không được vượt quá 100%');
      return;
    }

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.createOrUpdateKinhPhi(kinhPhiCongDoan);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showMessage("error", result.messageCode);
      return;
    }

    this.kinhPhiCongDoanFormGroup.disable();
    this.showMessage("success", result.messageCode);
    this.getMasterData();
  }

  huySuaKinhPhiCongDoan() {
    this.kinhPhiCongDoanIdControl.setValue(this.kinhPhiCongDoanClone.kinhPhiCongDoanId);
    this.mucDongNhanVienControl.setValue(this.kinhPhiCongDoanClone.mucDongNhanVien);
    this.mucDongCongTyControl.setValue(this.kinhPhiCongDoanClone.mucDongCongTy);

    this.kinhPhiCongDoanFormGroup.disable();
  }

  xoaCongThuc() {
    this.congThucControl.setValue(null);
  }

  addToken(token: any) {
    let congThuc = this.congThucControl.value ?? '';
    this.congThucControl.setValue(congThuc + token.valueText);
  }

  async saveCongThucTinhLuong() {
    if (!this.congThucTinhLuongFormGroup.valid) {
      Object.keys(this.congThucTinhLuongFormGroup.controls).forEach(key => {
        if (!this.congThucTinhLuongFormGroup.controls[key].valid) {
          this.congThucTinhLuongFormGroup.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let congThucTinhLuong = new CongThucTinhLuong();
    congThucTinhLuong.congThucTinhLuongId = this.congThucTinhLuongIdControl.value;
    congThucTinhLuong.congThuc = this.congThucControl.value;

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.createOrUpdateCongThucTinhLuong(congThucTinhLuong);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showCongThucTinhLuong = false;
    this.showMessage("success", result.messageCode);
    this.getMasterData();
  }

  suaCauHinhChamCongOt() {
    this.cauHinhChamCongOtFormGroup.enable();
  }

  async saveCauHinhChamCongOt() {
    if (!this.cauHinhChamCongOtFormGroup.valid) {
      Object.keys(this.cauHinhChamCongOtFormGroup.controls).forEach(key => {
        if (!this.cauHinhChamCongOtFormGroup.controls[key].valid) {
          this.cauHinhChamCongOtFormGroup.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let cauHinhChamCongOt = new CauHinhChamCongOt();
    cauHinhChamCongOt.cauHinhChamCongOtId = this.cauHinhChamCongOtId.value;
    cauHinhChamCongOt.soPhut = this.commonService.convertStringToNumber(this.soPhutControl.value.toString());

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.createOrUpdateCauHinhChamCongOt(cauHinhChamCongOt);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showMessage("error", result.messageCode);
      return;
    }

    this.cauHinhChamCongOtFormGroup.disable();
    this.showMessage("success", result.messageCode);
    this.getMasterData();
  }

  huySuaCauHinhChamCongOt() {
    this.soPhutControl.setValue(this.cauHinhChamCongOtClone.soPhut);
    this.cauHinhChamCongOtFormGroup.disable();
  }

  suaCauHinhOtCaNgay() {
    this.cauHinhOtCaNgayFormGroup.enable();
  }

  huySuaCauHinhOtCaNgay() {
    let gioVaoSang = this.cauHinhOtCaNgayClone.gioVaoSang?.slice(0, this.cauHinhOtCaNgayClone.gioVaoSang.lastIndexOf(':'));
    this.gioVaoSangControl.setValue(this.commonService.convertTimeSpanToDate(gioVaoSang));

    let gioRaSang = this.cauHinhOtCaNgayClone.gioRaSang?.slice(0, this.cauHinhOtCaNgayClone.gioRaSang.lastIndexOf(':'));
    this.gioRaSangControl.setValue(this.commonService.convertTimeSpanToDate(gioRaSang));

    let gioKetThucSang = this.cauHinhOtCaNgayClone.gioKetThucSang?.slice(0, this.cauHinhOtCaNgayClone.gioKetThucSang.lastIndexOf(':'));
    this.gioKetThucSangControl.setValue(this.commonService.convertTimeSpanToDate(gioKetThucSang));

    let gioVaoChieu = this.cauHinhOtCaNgayClone.gioVaoChieu?.slice(0, this.cauHinhOtCaNgayClone.gioVaoChieu.lastIndexOf(':'));
    this.gioVaoChieuControl.setValue(this.commonService.convertTimeSpanToDate(gioVaoChieu));

    let gioRaChieu = this.cauHinhOtCaNgayClone.gioRaChieu?.slice(0, this.cauHinhOtCaNgayClone.gioRaChieu.lastIndexOf(':'));
    this.gioRaChieuControl.setValue(this.commonService.convertTimeSpanToDate(gioRaChieu));

    let gioKetThucChieu = this.cauHinhOtCaNgayClone.gioKetThucChieu?.slice(0, this.cauHinhOtCaNgayClone.gioKetThucChieu.lastIndexOf(':'));
    this.gioKetThucChieuControl.setValue(this.commonService.convertTimeSpanToDate(gioKetThucChieu));

    this.cauHinhOtCaNgayFormGroup.disable();
  }

  async saveCauHinhOtCaNgay() {
    if (!this.cauHinhOtCaNgayFormGroup.valid) {
      Object.keys(this.cauHinhOtCaNgayFormGroup.controls).forEach(key => {
        if (!this.cauHinhOtCaNgayFormGroup.controls[key].valid) {
          this.cauHinhOtCaNgayFormGroup.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let cauHinhOtCaNgay = new CauHinhOtCaNgay();
    cauHinhOtCaNgay.cauHinhOtCaNgayId = this.cauHinhOtCaNgayIdControl.value;
    cauHinhOtCaNgay.gioVaoSang = this.commonService.convertDateToTimeSpan(this.gioVaoSangControl.value);
    cauHinhOtCaNgay.gioRaSang = this.commonService.convertDateToTimeSpan(this.gioRaSangControl.value);
    cauHinhOtCaNgay.gioKetThucSang = this.commonService.convertDateToTimeSpan(this.gioKetThucSangControl.value);
    cauHinhOtCaNgay.gioVaoChieu = this.commonService.convertDateToTimeSpan(this.gioVaoChieuControl.value);
    cauHinhOtCaNgay.gioRaChieu = this.commonService.convertDateToTimeSpan(this.gioRaChieuControl.value);
    cauHinhOtCaNgay.gioKetThucChieu = this.commonService.convertDateToTimeSpan(this.gioKetThucChieuControl.value);

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.createOrUpdateCauHinhOtCaNgay(cauHinhOtCaNgay);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showMessage("error", result.messageCode);
      return;
    }

    this.cauHinhOtCaNgayFormGroup.disable();
    this.showMessage("success", result.messageCode);
    this.getMasterData();
  }

  suaCauHinhTtncn(data: any) {
    this.cauHinhThueTncnIdControl.setValue(data.cauHinhThueTncnId);
    this.soTienTuControl.setValue(data.soTienTu);
    this.soTienDenControl.setValue(data.soTienDen);
    this.phanTramThueControl.setValue(data.phanTramThue);
    this.soBiTruTheoCongThucControl.setValue(data.soBiTruTheoCongThuc);

    this.showCauHinhTtncn = true;
  }

  xoaCauHinhTtncn(data: any) {
    this.confirmationService.confirm({
      message: `Dữ liệu không thể hoàn tác, bạn chắc chắn muốn xóa cấu hình này?`,
      accept: async () => {
        this.loading = true;
        this.awaitResult = true;
        let result: any = await this.salaryService.deleteCauHinhThueTncn(data.cauHinhThueTncnId);

        if (result.statusCode != 200) {
          this.loading = false;
          this.awaitResult = false;
          this.showMessage("error", result.messageCode);
          return;
        }

        this.showMessage("success", result.messageCode);
        this.getMasterData();
      },
    });
  }

  async saveCauHinhTtncn() {
    if (!this.ttncnFormGroup.valid) {
      Object.keys(this.ttncnFormGroup.controls).forEach(key => {
        if (!this.ttncnFormGroup.controls[key].valid) {
          this.ttncnFormGroup.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let phanTramThue = this.phanTramThueControl.value;

    if (phanTramThue > 100) {
      this.showMessage('error', 'Thuế suất không được quá 100%');
      return;
    }

    let cauHinh = {
      cauHinhThueTncnId: this.cauHinhThueTncnIdControl.value,
      soTienTu: this.commonService.convertStringToNumber(this.soTienTuControl.value.toString()),
      soTienDen: this.commonService.convertStringToNumber(this.soTienDenControl.value.toString()),
      phanTramThue: this.commonService.convertStringToNumber(this.phanTramThueControl.value.toString()),
      soBiTruTheoCongThuc: this.commonService.convertStringToNumber(this.soBiTruTheoCongThucControl.value.toString())
    };

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.createOrUpdateCauHinhThueTncn(cauHinh);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showMessage("success", result.messageCode);
    await this.getMasterData();
    this.showCauHinhTtncn = false;
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }

}
