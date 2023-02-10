import { SalaryService } from './../../../../services/salary.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { CommonService } from '../../../../../shared/services/common.service';
import { ConfirmationService, MessageService } from "primeng/api";

@Component({
  selector: 'app-update-dieu-kien-tro-cap-co-dinh',
  templateUrl: './update-dieu-kien-tro-cap-co-dinh.component.html',
  styleUrls: ['./update-dieu-kien-tro-cap-co-dinh.component.css']
})
export class UpdateDieuKienTroCapCoDinhComponent implements OnInit {
  awaitResponse: boolean = false;

  trangThai: number = null;
  employeeName: string = null;
  loaiTroCap: string = null;
  listLuongCtDieuKienTroCapCoDinh: Array<any> = [];
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
    this.trangThai = this.config.data.trangThai;
    this.employeeName = this.config.data.employeeName;
    this.loaiTroCap = this.config.data.loaiTroCap;
    this.listLuongCtDieuKienTroCapCoDinh = this.config.data.listLuongCtDieuKienTroCapCoDinh;
  }

  ngOnInit(): void {

  }

  async update(data: any) {
    this.awaitResponse = true;
    let result: any = await this.salaryService.updateDieuKienTroCapCoDinh(data);
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
