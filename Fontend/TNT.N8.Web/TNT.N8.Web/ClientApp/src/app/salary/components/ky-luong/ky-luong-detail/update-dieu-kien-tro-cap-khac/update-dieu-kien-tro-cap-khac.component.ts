import { SalaryService } from './../../../../services/salary.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { CommonService } from '../../../../../shared/services/common.service';
import { ConfirmationService, MessageService } from "primeng/api";

@Component({
  selector: 'app-update-dieu-kien-tro-cap-khac',
  templateUrl: './update-dieu-kien-tro-cap-khac.component.html',
  styleUrls: ['./update-dieu-kien-tro-cap-khac.component.css']
})
export class UpdateDieuKienTroCapKhacComponent implements OnInit {
  awaitResponse: boolean = false;

  luongCtLoaiTroCapKhacId: number = null;
  trangThai: number = null;
  employeeId: string = null;
  employeeName: string = null;
  loaiTroCap: string = null;
  isCoCauHinhLoaiTroCap: boolean = null;
  mucTroCap: any = null;
  listLuongCtDieuKienTroCapKhac: Array<any> = [];
  cols: Array<any> = [
    { field: "stt", header: "#", width: "60px", textAlign: "center" },
    { field: "dieuKienHuong", header: "Điều kiện hưởng", width: "200px", textAlign: "left" },
    { field: "duDieuKien", header: "Xác nhận", width: "100px", textAlign: "center" },
  ];

  constructor(
    private ref: DynamicDialogRef, 
    private config: DynamicDialogConfig,
    private commonService: CommonService,
    private salaryService: SalaryService,
    private messageService: MessageService
  ) { 
    this.luongCtLoaiTroCapKhacId = this.config.data.luongCtLoaiTroCapKhacId;
    this.trangThai = this.config.data.trangThai;
    this.employeeId = this.config.data.employeeId;
    this.employeeName = this.config.data.employeeName;
    this.loaiTroCap = this.config.data.loaiTroCap;
    this.mucTroCap = this.config.data.mucTroCap;
    this.isCoCauHinhLoaiTroCap = this.config.data.isCoCauHinhLoaiTroCap;
    this.listLuongCtDieuKienTroCapKhac = this.config.data.listLuongCtDieuKienTroCapKhac;
  }

  ngOnInit(): void {

  }

  async updateMucTroCapKhac() {
    let mucTroCap = 0;
    if (this.mucTroCap && this.mucTroCap != '') mucTroCap = this.commonService.convertStringToNumber(this.mucTroCap);

    this.awaitResponse = true;
    let result: any = await this.salaryService.updateMucTroCapKhac(mucTroCap, this.luongCtLoaiTroCapKhacId);
    this.awaitResponse = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showMessage("success", result.messageCode);
  }

  async update(data: any) {
    this.awaitResponse = true;
    let result: any = await this.salaryService.updateDieuKienTroCapKhac(this.employeeId, data);
    this.awaitResponse = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showMessage("success", result.messageCode);
  }

  close() {
    this.ref.close();
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }

}
