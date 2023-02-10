import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ConfirmationService, MessageService } from "primeng/api";
import { SalaryService } from '../../../services/salary.service';
import { CommonService } from '../../../../shared/services/common.service';
import { GetPermission } from "../../../../shared/permission/get-permission";
import { Router } from "@angular/router";
import { TroCap } from '../../../models/tro-cap.model';
import { TroCapChucVuMapping } from '../../../models/tro-cap-chuc-vu-mapping.model';
import { TroCapLoaiHopDongMapping } from '../../../models/tro-cap-loai-hop-dong-mapping.model';
import { TroCapDieuKienHuongMapping } from '../../../models/tro-cap-dieu-kien-huong-mapping.model';
import { MucHuongTheoNgayNghi } from '../../../models/muc-huong-theo-ngay-nghi.model';
import { MucHuongDmvs } from '../../../models/muc-huong-dmvs.model';

@Component({
  selector: 'app-cau-hinh-chung',
  templateUrl: './cau-hinh-chung.component.html',
  styleUrls: ['./cau-hinh-chung.component.css']
})
export class CauHinhChungComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;
  tabIndex: number = 0;

  actionAdd: boolean = false;
  actionEdit: boolean = false;
  actionDelete: boolean = false;

  showTroCapForm: boolean = false;
  showMucHuongNgayNghi: boolean = false;
  showMucHuongDmvs: boolean = false;

  /* typeId = 1 || typeId = 4 */
  troCapForm: FormGroup;
  troCapIdControl: FormControl;
  loaiTroCapControl: FormControl;
  mucTroCapControl: FormControl;
  chucVuControl: FormControl;
  loaiHopDongControl: FormControl;
  dieuKienHuongControl: FormControl;

  /* typeId = 2 */
  troCapNgayNghiForm: FormGroup;
  troCapNgayNghiIdControl: FormControl;
  loaiTroCapNgayNghiControl: FormControl;
  mucTroCapNgayNghiControl: FormControl;
  chucVuNgayNghiControl: FormControl;
  loaiHopDongNgayNghiControl: FormControl;
  dieuKienHuongNgayNghiControl: FormControl;

  mucHuongNgayNghiForm: FormGroup;
  mucHuongNgayNghiTroCapIdControl: FormControl;
  mucHuongPhanTramControl: FormControl;
  loaiNgayNghiControl: FormControl;
  soNgayTuControl: FormControl;
  soNgayDenControl: FormControl;

  /* typeId = 3 */
  troCapDmvsForm: FormGroup;
  troCapDmvsIdControl: FormControl;
  loaiTroCapDmvsControl: FormControl;
  mucTroCapDmvsControl: FormControl;
  chucVuDmvsControl: FormControl;
  loaiHopDongDmvsControl: FormControl;
  dieuKienHuongDmvsControl: FormControl;

  mucHuongDmvsForm: FormGroup;
  mucHuongDmvsTroCapIdControl: FormControl;
  hinhThucTruControl: FormControl;
  mucTruControl: FormControl;
  soLanTuControl: FormControl;
  soLanDenControl: FormControl;

  cols: Array<any> = [];
  colsMucHuongNgayNghi: Array<any> = [];
  colsMucHuongDmvs: Array<any> = [];

  listLoaiTroCapCoDinh: Array<any> = [];
  listLoaiTroCapChuyenCanNgayCong: Array<any> = [];
  listLoaiTroCapChuyenCanDmvs: Array<any> = [];
  listLoaiTroCapKhac: Array<any> = [];
  listPosition: Array<any> = [];
  listLoaiHopDong: Array<any> = [];
  listDieuKienHuong: Array<any> = [];
  listLoaiNgayNghi: Array<any> = [];
  listHinhThucTru: Array<any> = [];

  listTroCapCoDinh: Array<TroCap> = [];
  troCapTheoChuyenCanNgayCong: TroCap = new TroCap;
  listMucHuongTheoNgayNghi: Array<MucHuongTheoNgayNghi> = [];
  troCapTheoChuyenCanDmvs: TroCap = new TroCap;
  listMucHuongDmvs: Array<MucHuongDmvs> = [];
  listTroCapKhac: Array<TroCap> = [];

  typeId: number = null;
  troCapTheoChuyenCanNgayCongClone: TroCap = new TroCap;
  mucHuongNgayNghiId: number = null;
  troCapTheoChuyenCanDmvsClone: TroCap = new TroCap;
  mucHuongDmvsId: number = null;

  refreshNote = 0;

  constructor(
    private router: Router,
    private messageService: MessageService,
    private salaryService: SalaryService,
    private confirmationService: ConfirmationService,
    private getPermission: GetPermission,
    private commonService: CommonService,
  ) { }

  async ngOnInit() {
    this.initForm();
    this.initTable();
    await this._getPermission();
    this.getMasterData();
  }

  initForm() {
    /* Trợ cấp theo ngày công thực tế */
    this.troCapIdControl = new FormControl(null);
    this.loaiTroCapControl = new FormControl(null, [Validators.required]);
    this.mucTroCapControl = new FormControl(0, [Validators.required]);
    this.chucVuControl = new FormControl([], [Validators.required]);
    this.loaiHopDongControl = new FormControl([], [Validators.required]);
    this.dieuKienHuongControl = new FormControl([]);

    this.troCapForm = new FormGroup({
      troCapIdControl: this.troCapIdControl,
      loaiTroCapControl: this.loaiTroCapControl,
      mucTroCapControl: this.mucTroCapControl,
      chucVuControl: this.chucVuControl,
      loaiHopDongControl: this.loaiHopDongControl,
      dieuKienHuongControl: this.dieuKienHuongControl
    });

    /* Trợ cấp theo ngày nghỉ */
    this.troCapNgayNghiIdControl = new FormControl(null);
    this.loaiTroCapNgayNghiControl = new FormControl(null, [Validators.required]);
    this.mucTroCapNgayNghiControl = new FormControl(0, [Validators.required]);
    this.chucVuNgayNghiControl = new FormControl([], [Validators.required]);
    this.loaiHopDongNgayNghiControl = new FormControl([], [Validators.required]);
    this.dieuKienHuongNgayNghiControl = new FormControl([]);

    this.troCapNgayNghiForm = new FormGroup({
      troCapNgayNghiIdControl: this.troCapNgayNghiIdControl,
      loaiTroCapNgayNghiControl: this.loaiTroCapNgayNghiControl,
      mucTroCapNgayNghiControl: this.mucTroCapNgayNghiControl,
      chucVuNgayNghiControl: this.chucVuNgayNghiControl,
      loaiHopDongNgayNghiControl: this.loaiHopDongNgayNghiControl,
      dieuKienHuongNgayNghiControl: this.dieuKienHuongNgayNghiControl
    });

    this.troCapNgayNghiForm.disable();

    this.mucHuongNgayNghiTroCapIdControl = new FormControl(null);
    this.mucHuongPhanTramControl = new FormControl(0, [Validators.required]);
    this.loaiNgayNghiControl = new FormControl(null);
    this.soNgayTuControl = new FormControl(null, [Validators.required]);
    this.soNgayDenControl = new FormControl(null);

    this.mucHuongNgayNghiForm = new FormGroup({
      mucHuongNgayNghiTroCapIdControl: this.mucHuongNgayNghiTroCapIdControl,
      mucHuongPhanTramControl: this.mucHuongPhanTramControl,
      loaiNgayNghiControl: this.loaiNgayNghiControl,
      soNgayTuControl: this.soNgayTuControl,
      soNgayDenControl: this.soNgayDenControl
    });

    /* Trợ cấp theo số lần đi muộn về sớm */
    this.troCapDmvsIdControl = new FormControl(null);
    this.loaiTroCapDmvsControl = new FormControl(null, [Validators.required]);
    this.mucTroCapDmvsControl = new FormControl(0, [Validators.required]);
    this.chucVuDmvsControl = new FormControl([], [Validators.required]);
    this.loaiHopDongDmvsControl = new FormControl([], [Validators.required]);
    this.dieuKienHuongDmvsControl = new FormControl([]);

    this.troCapDmvsForm = new FormGroup({
      troCapDmvsIdControl: this.troCapDmvsIdControl,
      loaiTroCapDmvsControl: this.loaiTroCapDmvsControl,
      mucTroCapDmvsControl: this.mucTroCapDmvsControl,
      chucVuDmvsControl: this.chucVuDmvsControl,
      loaiHopDongDmvsControl: this.loaiHopDongDmvsControl,
      dieuKienHuongDmvsControl: this.dieuKienHuongDmvsControl
    });

    this.troCapDmvsForm.disable();

    this.mucHuongDmvsTroCapIdControl = new FormControl(null);
    this.mucTruControl = new FormControl(0, [Validators.required]);
    this.hinhThucTruControl = new FormControl(null, [Validators.required]);
    this.soLanTuControl = new FormControl(null, [Validators.required]);
    this.soLanDenControl = new FormControl(null);

    this.mucHuongDmvsForm = new FormGroup({
      mucHuongDmvsTroCapIdControl: this.mucHuongDmvsTroCapIdControl,
      mucTruControl: this.mucTruControl,
      hinhThucTruControl: this.hinhThucTruControl,
      soLanTuControl: this.soLanTuControl,
      soLanDenControl: this.soLanDenControl,
    });
  }

  initTable() {
    this.cols = [
      { field: "stt", header: "#", width: "10%", textAlign: "center" },
      { field: "loaiTroCap", header: "Loại trợ cấp", width: "10%", textAlign: "left" },
      { field: "mucTroCap", header: "Mức trợ cấp", width: "10%", textAlign: "right" },
      { field: "chucVuText", header: "Chức vụ hưởng", width: "20%", textAlign: "left" },
      { field: "loaiHopDongText", header: "Loại HĐ hưởng", width: "20%", textAlign: "left" },
      { field: "dieuKienHuongText", header: "Điều kiện hưởng khác", width: "20%", textAlign: "left" },
      { field: "action", header: "Thao tác", width: "10%", textAlign: "center" },
    ];

    this.colsMucHuongNgayNghi = [
      { field: "stt", header: "#", width: "10%", textAlign: "center" },
      { field: "mucHuongPhanTram", header: "Mức hưởng (%)", width: "20%", textAlign: "right" },
      { field: "cachTinh", header: "Cách tính", width: "60%", textAlign: "left" },
      { field: "action", header: "Thao tác", width: "10%", textAlign: "center" },
    ];

    this.colsMucHuongDmvs = [
      { field: "stt", header: "#", width: "10%", textAlign: "center" },
      { field: "hinhThucTruText", header: "Hình thức trừ", width: "20%", textAlign: "left" },
      { field: "mucTru", header: "Mức trừ", width: "20%", textAlign: "right" },
      { field: "cachTinh", header: "Cách tính", width: "60%", textAlign: "left" },
      { field: "action", header: "Thao tác", width: "10%", textAlign: "center" },
    ];
  }

  async _getPermission() {

    let resource = "salary/salary/cau-hinh-chung/";
    this.loading = true;
    let permission: any = await this.getPermission.getPermission(resource);
    this.loading = false;

    if (permission.status == false) {
      this.router.navigate(["/home"]);
      return;
    }

    if (permission.listCurrentActionResource.indexOf("add") != -1) {
      this.actionAdd = true;
    }

    if (permission.listCurrentActionResource.indexOf("edit") != -1) {
      this.actionEdit = true;
    }

    if (permission.listCurrentActionResource.indexOf("delete") != -1) {
      this.actionDelete = true;
    }
  }

  async getMasterData() {
    this.loading = true;
    this.awaitResult = true;

    let result: any = await this.salaryService.getMasterDataTroCap();
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.listLoaiTroCapCoDinh = result.listLoaiTroCapCoDinh;
    this.listLoaiTroCapChuyenCanNgayCong = result.listLoaiTroCapChuyenCanNgayCong;
    this.listLoaiTroCapChuyenCanDmvs = result.listLoaiTroCapChuyenCanDmvs;
    this.listLoaiTroCapKhac = result.listLoaiTroCapKhac;
    this.listPosition = result.listPosition;
    this.listLoaiHopDong = result.listLoaiHopDong;
    this.listDieuKienHuong = result.listDieuKienHuong;
    this.listLoaiNgayNghi = result.listLoaiNgayNghi;
    this.listHinhThucTru = result.listHinhThucTru;

    this.listTroCapCoDinh = result.listTroCapCoDinh;
    this.troCapTheoChuyenCanNgayCong = result.troCapTheoChuyenCanNgayCong;
    this.troCapTheoChuyenCanNgayCongClone = Object.assign({}, result.troCapTheoChuyenCanNgayCong);
    this.troCapTheoChuyenCanDmvs = result.troCapTheoChuyenCanDmvs;
    this.troCapTheoChuyenCanDmvsClone = Object.assign({}, result.troCapTheoChuyenCanDmvs);
    this.listTroCapKhac = result.listTroCapKhac;

    this.mapDataToTroCapNgayNghiForm(this.troCapTheoChuyenCanNgayCong);
    this.mapDataToTroCapDmvsForm(this.troCapTheoChuyenCanDmvs);
  }

  mapDataToTroCapNgayNghiForm(troCap: TroCap) {
    this.troCapNgayNghiIdControl.setValue(troCap.troCapId);

    let loaiTroCap = this.listLoaiTroCapChuyenCanNgayCong.find(x => x.categoryId == troCap.loaiTroCapId);
    this.loaiTroCapNgayNghiControl.setValue(loaiTroCap);

    this.mucTroCapNgayNghiControl.setValue(troCap.mucTroCap);

    let listChucVu = this.listPosition.filter(x => troCap.listChucVu.map(x => x.positionId).includes(x.positionId));
    this.chucVuNgayNghiControl.setValue(listChucVu);

    let listLoaiHopDong = this.listLoaiHopDong.filter(x => troCap.listLoaiHopDong.map(x => x.loaiHopDongId).includes(x.categoryId));
    this.loaiHopDongNgayNghiControl.setValue(listLoaiHopDong);

    let listDieuKienHuong = this.listDieuKienHuong.filter(x => troCap.listDieuKienHuong.map(x => x.dieuKienHuongId).includes(x.categoryId));
    this.dieuKienHuongNgayNghiControl.setValue(listDieuKienHuong);

    this.listMucHuongTheoNgayNghi = [];
    troCap.listMucHuongTheoNgayNghi.forEach((item, index) => {
      item.id = index + 1;
      item.mucHuongTheoNgayNghiId = null;

      this.listMucHuongTheoNgayNghi = [...this.listMucHuongTheoNgayNghi, item];
    });
  }

  mapDataToTroCapDmvsForm(troCap: TroCap) {
    this.troCapDmvsIdControl.setValue(troCap.troCapId);

    let loaiTroCap = this.listLoaiTroCapChuyenCanDmvs.find(x => x.categoryId == troCap.loaiTroCapId);
    this.loaiTroCapDmvsControl.setValue(loaiTroCap);

    this.mucTroCapDmvsControl.setValue(troCap.mucTroCap);

    let listChucVu = this.listPosition.filter(x => troCap.listChucVu.map(x => x.positionId).includes(x.positionId));
    this.chucVuDmvsControl.setValue(listChucVu);

    let listLoaiHopDong = this.listLoaiHopDong.filter(x => troCap.listLoaiHopDong.map(x => x.loaiHopDongId).includes(x.categoryId));
    this.loaiHopDongDmvsControl.setValue(listLoaiHopDong);

    let listDieuKienHuong = this.listDieuKienHuong.filter(x => troCap.listDieuKienHuong.map(x => x.dieuKienHuongId).includes(x.categoryId));
    this.dieuKienHuongDmvsControl.setValue(listDieuKienHuong);

    this.listMucHuongDmvs = [];
    troCap.listMucHuongDmvs.forEach((item, index) => {
      item.id = index + 1;
      item.mucHuongDmvsId = null;

      this.listMucHuongDmvs = [...this.listMucHuongDmvs, item];
    });
  }

  themTroCap(typeId: number) {
    this.typeId = typeId;
    this.troCapForm.reset();
    this.mucTroCapControl.setValue(0);
    this.showTroCapForm = true;
  }

  suaTroCap(typeId: number, rowData: TroCap) {
    this.typeId = typeId;
    this.troCapForm.reset();

    this.troCapIdControl.setValue(rowData.troCapId);

    if (typeId == 1) {
      let loaiTroCap = this.listLoaiTroCapCoDinh.find(x => x.categoryId == rowData.loaiTroCapId);
      this.loaiTroCapControl.setValue(loaiTroCap);
    }
    else if (typeId == 4) {
      let loaiTroCap = this.listLoaiTroCapKhac.find(x => x.categoryId == rowData.loaiTroCapId);
      this.loaiTroCapControl.setValue(loaiTroCap);
    }

    this.mucTroCapControl.setValue(rowData.mucTroCap);

    let listChucVu = this.listPosition.filter(x => rowData.listChucVu.map(x => x.positionId).includes(x.positionId));
    this.chucVuControl.setValue(listChucVu);

    let listLoaiHopDong = this.listLoaiHopDong.filter(x => rowData.listLoaiHopDong.map(x => x.loaiHopDongId).includes(x.categoryId));
    this.loaiHopDongControl.setValue(listLoaiHopDong);

    let listDieuKienHuong = this.listDieuKienHuong.filter(x => rowData.listDieuKienHuong.map(x => x.dieuKienHuongId).includes(x.categoryId));
    this.dieuKienHuongControl.setValue(listDieuKienHuong);

    this.showTroCapForm = true;
  }

  xoaTroCap(typeId: number, rowData: TroCap) {
    this.confirmationService.confirm({
      message: `Dữ liệu không thể hoàn tác, bạn chắc chắn muốn xóa cấu hình này?`,
      accept: async () => {
        this.loading = true;
        this.awaitResult = true;
        let result: any = await this.salaryService.deleteTroCap(typeId, rowData.troCapId);

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

  async saveTroCap() {
    if (!this.troCapForm.valid) {
      Object.keys(this.troCapForm.controls).forEach(key => {
        if (!this.troCapForm.controls[key].valid) {
          this.troCapForm.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let troCap = this.mapDataToModel();
    await this.createOrUpdateTroCap(troCap);
    this.showTroCapForm = false;
  }

  async createOrUpdateTroCap(troCap: TroCap) {
    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.createOrUpdateTroCap(troCap);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showMessage("success", result.messageCode);
    await this.getMasterData();
    return;
  }

  mapDataToModel() {
    let troCap = new TroCap();
    troCap.troCapId = this.troCapIdControl.value;
    troCap.typeId = this.typeId;
    troCap.loaiTroCapId = this.loaiTroCapControl.value.categoryId;
    troCap.mucTroCap = this.commonService.convertStringToNumber(this.mucTroCapControl.value.toString());

    let listChucVu = this.chucVuControl.value;
    listChucVu ?.forEach(item => {
      let newItem = new TroCapChucVuMapping();
      newItem.troCapId = troCap.troCapId;
      newItem.positionId = item.positionId;

      troCap.listChucVu.push(newItem);
    });

    let listLoaiHopDong = this.loaiHopDongControl.value;
    listLoaiHopDong ?.forEach(item => {
      let newItem = new TroCapLoaiHopDongMapping();
      newItem.troCapId = troCap.troCapId;
      newItem.loaiHopDongId = item.categoryId;

      troCap.listLoaiHopDong.push(newItem);
    });

    let listDieuKienHuong = this.dieuKienHuongControl.value;
    listDieuKienHuong ?.forEach(item => {
      let newItem = new TroCapDieuKienHuongMapping();
      newItem.troCapId = troCap.troCapId;
      newItem.dieuKienHuongId = item.categoryId;

      troCap.listDieuKienHuong.push(newItem);
    });

    return troCap;
  }

  closeTroCap() {
    this.typeId = null;
    this.showTroCapForm = false;
  }

  suaTroCapNgayNghi() {
    this.troCapNgayNghiForm.enable();
  }

  async saveTroCapNgayNghi() {
    if (!this.troCapNgayNghiForm.valid) {
      Object.keys(this.troCapNgayNghiForm.controls).forEach(key => {
        if (!this.troCapNgayNghiForm.controls[key].valid) {
          this.troCapNgayNghiForm.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let troCap = this.mapDataTroCapNgayNghiToModel();
    await this.createOrUpdateTroCap(troCap);
    this.troCapNgayNghiForm.disable();
  }

  mapDataTroCapNgayNghiToModel(): TroCap {
    let troCap = new TroCap;
    troCap.troCapId = this.troCapNgayNghiIdControl.value;
    troCap.typeId = 2;
    troCap.loaiTroCapId = this.loaiTroCapNgayNghiControl.value.categoryId;
    troCap.mucTroCap = this.commonService.convertStringToNumber(this.mucTroCapNgayNghiControl.value.toString());

    let listChucVu = this.chucVuNgayNghiControl.value;
    listChucVu ?.forEach(item => {
      let newItem = new TroCapChucVuMapping();
      newItem.troCapId = troCap.troCapId;
      newItem.positionId = item.positionId;

      troCap.listChucVu.push(newItem);
    });

    let listLoaiHopDong = this.loaiHopDongNgayNghiControl.value;
    listLoaiHopDong ?.forEach(item => {
      let newItem = new TroCapLoaiHopDongMapping();
      newItem.troCapId = troCap.troCapId;
      newItem.loaiHopDongId = item.categoryId;

      troCap.listLoaiHopDong.push(newItem);
    });

    let listDieuKienHuong = this.dieuKienHuongNgayNghiControl.value;
    listDieuKienHuong ?.forEach(item => {
      let newItem = new TroCapDieuKienHuongMapping();
      newItem.troCapId = troCap.troCapId;
      newItem.dieuKienHuongId = item.categoryId;

      troCap.listDieuKienHuong.push(newItem);
    });

    troCap.listMucHuongTheoNgayNghi = this.listMucHuongTheoNgayNghi;

    return troCap;
  }

  huySuaTroCapNgayNghi() {
    this.mapDataToTroCapNgayNghiForm(this.troCapTheoChuyenCanNgayCongClone);
    this.troCapNgayNghiForm.disable();
  }

  openAddMucHuongNgayNghi() {
    this.mucHuongNgayNghiForm.reset();
    this.mucHuongPhanTramControl.setValue(0);
    this.soNgayTuControl.setValue(0);
    this.mucHuongNgayNghiId = null;
    this.showMucHuongNgayNghi = true;
  }

  suaMucHuongNgayNghi(rowData: MucHuongTheoNgayNghi) {
    this.troCapNgayNghiIdControl.setValue(rowData.troCapId);

    let loaiNgayNghi = this.listLoaiNgayNghi.find(x => x.value == rowData.loaiNgayNghi) ?? null;
    this.loaiNgayNghiControl.setValue(loaiNgayNghi);

    this.mucHuongPhanTramControl.setValue(rowData.mucHuongPhanTram);
    this.soNgayTuControl.setValue(rowData.soNgayTu);
    this.soNgayDenControl.setValue(rowData.soNgayDen);

    this.mucHuongNgayNghiId = rowData.id;
    this.showMucHuongNgayNghi = true;
  }

  addMucHuongNgayNghi() {
    if (!this.mucHuongNgayNghiForm.valid) {
      Object.keys(this.mucHuongNgayNghiForm.controls).forEach(key => {
        if (!this.mucHuongNgayNghiForm.controls[key].valid) {
          this.mucHuongNgayNghiForm.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let soNgayTu = this.commonService.convertStringToNumber(this.soNgayTuControl.value.toString());
    let soNgayDen = this.soNgayDenControl.value ? this.commonService.convertStringToNumber(this.soNgayDenControl.value.toString()) : null;

    if (soNgayDen != null && soNgayTu > soNgayDen) {
      this.showMessage('warn', 'Số ngày từ phải nhỏ hơn Số ngày đến');
      return;
    }

    //Create
    if (!this.mucHuongNgayNghiId) {
      let maxId = 0;
      if (this.listMucHuongTheoNgayNghi.length > 0) maxId = Math.max(...this.listMucHuongTheoNgayNghi.map(x => x.id));

      let item = new MucHuongTheoNgayNghi;
      item.id = maxId + 1;

      item = this.mapDataMucHuongNgayNghiToModel(item);

      this.listMucHuongTheoNgayNghi = [...this.listMucHuongTheoNgayNghi, item];
    }
    //Update
    else {
      let item = this.listMucHuongTheoNgayNghi.find(x => x.id == this.mucHuongNgayNghiId);

      item = this.mapDataMucHuongNgayNghiToModel(item);
    }

    this.showMucHuongNgayNghi = false;
  }

  mapDataMucHuongNgayNghiToModel(item: MucHuongTheoNgayNghi) {
    item.troCapId = this.troCapNgayNghiIdControl.value;
    item.loaiNgayNghi = this.loaiNgayNghiControl.value ?.value ?? null;
    item.mucHuongPhanTram = this.commonService.convertStringToNumber(this.mucHuongPhanTramControl.value.toString());
    item.soNgayTu = this.commonService.convertStringToNumber(this.soNgayTuControl.value.toString());
    item.soNgayDen = this.soNgayDenControl.value ? this.commonService.convertStringToNumber(this.soNgayDenControl.value.toString()) : null;

    let text = item.loaiNgayNghi == null ? '' : this.loaiNgayNghiControl.value.name;
    if (item.soNgayDen != null && item.soNgayTu == item.soNgayDen) item.cachTinh = "Ngày nghỉ: " + item.soNgayTu + " ngày " + text;
    else if (item.soNgayDen == null) item.cachTinh = "Ngày nghỉ: Trên " + item.soNgayTu + " ngày " + text;
    else if (item.soNgayDen != null) item.cachTinh = "Ngày nghỉ: Trên " + item.soNgayTu + " ngày đến " + item.soNgayDen + " ngày " + text;

    return item;
  }

  xoaMucHuongNgayNghi(rowData: MucHuongTheoNgayNghi) {
    this.listMucHuongTheoNgayNghi = this.listMucHuongTheoNgayNghi.filter(x => x != rowData);
  }

  closeMucHuongNgayNghi() {
    this.showMucHuongNgayNghi = false;
  }

  changeMucHuongPhanTram() {
    let mucHuongPhanTram = this.commonService.convertStringToNumber(this.mucHuongPhanTramControl.value.toString());

    if (mucHuongPhanTram > 100) this.mucHuongPhanTramControl.setValue(100);
  }

  suaTroCapDmvs() {
    this.troCapDmvsForm.enable();
  }

  async saveTroCapDmvs() {
    if (!this.troCapDmvsForm.valid) {
      Object.keys(this.troCapDmvsForm.controls).forEach(key => {
        if (!this.troCapDmvsForm.controls[key].valid) {
          this.troCapDmvsForm.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let troCap = this.mapDataTroCapDmvsToModel();
    await this.createOrUpdateTroCap(troCap);
    this.troCapDmvsForm.disable();
  }

  mapDataTroCapDmvsToModel() {
    let troCap = new TroCap;
    troCap.troCapId = this.troCapDmvsIdControl.value;
    troCap.typeId = 3;
    troCap.loaiTroCapId = this.loaiTroCapDmvsControl.value.categoryId;
    troCap.mucTroCap = this.commonService.convertStringToNumber(this.mucTroCapDmvsControl.value.toString());

    let listChucVu = this.chucVuDmvsControl.value;
    listChucVu ?.forEach(item => {
      let newItem = new TroCapChucVuMapping();
      newItem.troCapId = troCap.troCapId;
      newItem.positionId = item.positionId;

      troCap.listChucVu.push(newItem);
    });

    let listLoaiHopDong = this.loaiHopDongDmvsControl.value;
    listLoaiHopDong ?.forEach(item => {
      let newItem = new TroCapLoaiHopDongMapping();
      newItem.troCapId = troCap.troCapId;
      newItem.loaiHopDongId = item.categoryId;

      troCap.listLoaiHopDong.push(newItem);
    });

    let listDieuKienHuong = this.dieuKienHuongDmvsControl.value;
    listDieuKienHuong ?.forEach(item => {
      let newItem = new TroCapDieuKienHuongMapping();
      newItem.troCapId = troCap.troCapId;
      newItem.dieuKienHuongId = item.categoryId;

      troCap.listDieuKienHuong.push(newItem);
    });

    troCap.listMucHuongDmvs = this.listMucHuongDmvs;

    return troCap;
  }

  huySuaTroCapDmvs() {
    this.mapDataToTroCapDmvsForm(this.troCapTheoChuyenCanDmvsClone);
    this.troCapDmvsForm.disable();
  }

  openAddMucHuongDmvs() {
    this.mucHuongDmvsForm.reset();
    this.hinhThucTruControl.setValue(this.listHinhThucTru[0]);
    this.mucTruControl.setValue(0);
    this.soLanTuControl.setValue(0);
    this.mucHuongDmvsId = null;
    this.showMucHuongDmvs = true;
  }

  suaMucHuongDmvs(rowData: MucHuongDmvs) {
    this.troCapDmvsIdControl.setValue(rowData.troCapId);

    let hinhThucTru = this.listHinhThucTru.find(x => x.value == rowData.hinhThucTru) ?? null;
    this.hinhThucTruControl.setValue(hinhThucTru);

    this.mucTruControl.setValue(rowData.mucTru);
    this.soLanTuControl.setValue(rowData.soLanTu);
    this.soLanDenControl.setValue(rowData.soLanDen);

    this.mucHuongDmvsId = rowData.id;
    this.showMucHuongDmvs = true;
  }

  xoaMucHuongDmvs(rowData: MucHuongDmvs) {
    this.listMucHuongDmvs = this.listMucHuongDmvs.filter(x => x != rowData);
  }

  addMucHuongDmvs() {
    if (!this.mucHuongDmvsForm.valid) {
      Object.keys(this.mucHuongDmvsForm.controls).forEach(key => {
        if (!this.mucHuongDmvsForm.controls[key].valid) {
          this.mucHuongDmvsForm.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let soLanTu = this.commonService.convertStringToNumber(this.soLanTuControl.value.toString());
    let soLanDen = this.soLanDenControl.value ? this.commonService.convertStringToNumber(this.soLanDenControl.value.toString()) : null;

    if (soLanDen != null && soLanTu > soLanDen) {
      this.showMessage('warn', 'Số lần đi muộn về sớm từ phải nhỏ hơn Số lần đi muộn về sớm đến');
      return;
    }

    //Create
    if (!this.mucHuongDmvsId) {
      let maxId = 0;
      if (this.listMucHuongDmvs.length > 0) maxId = Math.max(...this.listMucHuongDmvs.map(x => x.id));

      let item = new MucHuongDmvs;
      item.id = maxId + 1;

      item = this.mapDataMucHuongDmvsToModel(item);

      this.listMucHuongDmvs = [...this.listMucHuongDmvs, item];
    }
    //Update
    else {
      let item = this.listMucHuongDmvs.find(x => x.id == this.mucHuongDmvsId);

      item = this.mapDataMucHuongDmvsToModel(item);
    }

    this.showMucHuongDmvs = false;
  }

  mapDataMucHuongDmvsToModel(item: MucHuongDmvs) {
    item.troCapId = this.troCapDmvsIdControl.value;
    item.hinhThucTru = this.hinhThucTruControl.value ?.value ?? null;
    item.mucTru = this.commonService.convertStringToNumber(this.mucTruControl.value.toString());
    item.soLanTu = this.commonService.convertStringToNumber(this.soLanTuControl.value.toString());
    item.soLanDen = this.soLanDenControl.value ? this.commonService.convertStringToNumber(this.soLanDenControl.value.toString()) : null;

    item.hinhThucTruText = this.hinhThucTruControl.value.name;

    if (item.soLanDen != null && item.soLanTu == item.soLanDen) item.cachTinh = "Số lần đi muộn về sớm: " + item.soLanTu + " lần/tháng";
    else if (item.soLanDen != null) item.cachTinh = "Số lần đi muộn về sớm: Trên " + item.soLanTu + " lần đến " + item.soLanDen + " lần/tháng";
    else if (item.soLanDen == null) item.cachTinh = "Số lần đi muộn về sớm: Trên " + item.soLanTu + " lần/tháng";

    return item;
  }

  closeMucHuongDmvs() {
    this.showMucHuongDmvs = false;
  }

  checkMucTru() {
    let hinhThucTru = this.hinhThucTruControl.value.value;

    //Nếu theo % mức trợ cấp
    if (hinhThucTru == 2) {
      let mucTru = this.commonService.convertStringToNumber(this.mucTruControl.value.toString());

      if (mucTru > 100) this.mucTruControl.setValue(100);
    }
  }

  convertNumber(number: string) {
    if (!isNaN(parseFloat(number))) {
      return parseFloat(number.toString().replace(/,/g, ''));
    }

    return number;
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }
}
