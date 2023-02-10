import { SalaryService } from './../../services/salary.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { CommonService } from '../../../shared/services/common.service';
import { ConfirmationService, MessageService } from "primeng/api";

@Component({
  selector: 'app-update-cham-cong-dialog',
  templateUrl: './update-cham-cong-dialog.component.html',
  styleUrls: ['./update-cham-cong-dialog.component.css']
})
export class UpdateChamCongDialogComponent implements OnInit {
  awaitResponse: boolean = false;
  typeUpdate: string = null;
  ngay: string = null;
  ca: string = null;
  gio: string = null;
  name: string = null;
  employeeId: string = null;
  indexGio: number = null;
  indexCa: number = null;

  listEmployee: Array<any> = [];
  listKyHieuChamCong: Array<any> = [];

  chamCongGroup: FormGroup;
  thoiGianControl: FormControl;
  nhanVienControl: FormControl;
  loaiNghiControl: FormControl;

  constructor(
    private ref: DynamicDialogRef, 
    private config: DynamicDialogConfig,
    private commonService: CommonService,
    private salaryService: SalaryService,
    private messageService: MessageService
  ) 
  { 
    this.typeUpdate = this.config.data.typeUpdate;
    this.ngay = this.config.data.ngay;
    this.ca = this.config.data.ca;
    this.gio = this.config.data.gio;
    this.name = this.config.data.name;
    this.employeeId = this.config.data.employeeId;
    this.indexGio = this.config.data.indexGio;
    this.indexCa = this.config.data.indexCa;
    this.listEmployee = this.config.data.listEmployee;
    this.listKyHieuChamCong = this.config.data.listKyHieuChamCong;
  }

  ngOnInit() {
    if (this.typeUpdate == 'GIO') {
      this.thoiGianControl = new FormControl(this.commonService.convertTimeSpanToDate(this.gio));

      this.chamCongGroup = new FormGroup({
        thoiGianControl: this.thoiGianControl
      });
    }
    else if (this.typeUpdate == 'CA' || this.typeUpdate == 'NGAY') {
      this.nhanVienControl = new FormControl(null, [Validators.required]);
      this.loaiNghiControl = new FormControl(null, [Validators.required]);

      this.chamCongGroup = new FormGroup({
        nhanVienControl: this.nhanVienControl,
        loaiNghiControl: this.loaiNghiControl
      });
    }
  }

  getTenLoaiGio() {
    let loaiGio = this.indexGio % 4;

    if (loaiGio == 1 || loaiGio == 3) return 'Giờ vào';
    else if (loaiGio == 2 || loaiGio == 0) return 'Giờ ra';
  }

  getTypeField() {
    let typeField = 0;

    if (this.typeUpdate == 'GIO') {
      typeField = this.indexGio % 4;

      if (typeField == 0) typeField = 4;
    }
    else if (this.typeUpdate == 'CA') {
      typeField = this.indexCa % 2;

      if (typeField == 0) typeField = 2;
    }

    return typeField;
  }

  async save() {
    if (!this.chamCongGroup.valid) {
      Object.keys(this.chamCongGroup.controls).forEach(key => {
        if (!this.chamCongGroup.controls[key].valid) {
          this.chamCongGroup.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let typeField = this.getTypeField();
    let ngayChamCong = this.commonService.convertStringToDate(this.ngay, 'dd/MM/yyyy');

    if (this.typeUpdate == 'GIO') {
      let thoiGian = this.commonService.convertDateToTimeSpan(this.thoiGianControl.value);

      this.awaitResponse = true;
      let result: any = await this.salaryService.createOrUpdateChamCong(this.typeUpdate, typeField, 
        [this.employeeId], ngayChamCong, thoiGian, null);

      if (result.statusCode != 200) {
        this.showMessage("error", result.messageCode);
        this.awaitResponse = false;
        return;
      }

      this.showMessage("success", result.messageCode);
      this.ref.close(true);
    }
    else if (this.typeUpdate == 'CA' || this.typeUpdate == 'NGAY') {
      let listEmployeeId = this.nhanVienControl.value.map(x => x.employeeId);
      let kyHieuChamCong = this.loaiNghiControl.value.value == 0 ? null : this.loaiNghiControl.value.value;

      this.awaitResponse = true;
      let result: any = await this.salaryService.createOrUpdateChamCong(this.typeUpdate, typeField, 
        listEmployeeId, ngayChamCong, null, kyHieuChamCong);

      if (result.statusCode != 200) {
        this.showMessage("error", result.messageCode);
        this.awaitResponse = false;
        return;
      }

      this.showMessage("success", result.messageCode);
      this.ref.close(true);
    }
  }

  close() {
    this.ref.close();
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }
}
