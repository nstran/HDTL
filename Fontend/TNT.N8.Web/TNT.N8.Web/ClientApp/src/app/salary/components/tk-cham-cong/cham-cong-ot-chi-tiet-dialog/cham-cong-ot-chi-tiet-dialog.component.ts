import { SalaryService } from './../../../services/salary.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AfterViewInit, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { CommonService } from '../../../../shared/services/common.service';
import { ConfirmationService, MessageService } from "primeng/api";
import { FormatDateService } from '../../../../shared/services/formatDate.services'

@Component({
  selector: 'app-cham-cong-ot-chi-tiet-dialog',
  templateUrl: './cham-cong-ot-chi-tiet-dialog.component.html',
  styleUrls: ['./cham-cong-ot-chi-tiet-dialog.component.css']
})
export class ChamCongOtChiTietDialogComponent implements OnInit {
  awaitRes: boolean = false;
  tuNgay: Date = new Date();
  denNgay: Date = new Date();
  employeeId: string = null;
  name: string = null;
  code: string = null;

  cols: Array<any> = [];
  trRow: Array<any> = [];
  listData: Array<any> = [];
  listLoaiOt: Array<any> = [];

  displayUpdate: boolean = false;
  type: string = null;
  ngayChamCong: Date = null;
  loaiOtId: string = null;
  loaiOt: string = null;
  thoiGian: Date = null;
  ca: string = null;

  constructor(
    private ref: DynamicDialogRef, 
    private config: DynamicDialogConfig,
    private commonService: CommonService,
    private salaryService: SalaryService,
    private messageService: MessageService,
    private formatDateService: FormatDateService,
    private cdref: ChangeDetectorRef,
  ) { 
    this.employeeId = this.config.data.employeeId;
    this.tuNgay = this.config.data.tuNgay || null;
    this.denNgay = this.config.data.denNgay || null;
    this.name = this.config.data.name;
    this.code = this.config.data.code;
  }

  ngOnInit() {
    this.getData();
  }

  async getData() {
    if (this.tuNgay == null || this.denNgay == null) {
      this.showMessage('warn', 'Bạn chưa chọn đủ bộ lọc các trường Từ ngày, Đến ngày');
      return;
    }

    let tuNgay = this.formatDateService.convertToUTCTime(this.tuNgay);
    let denNgay = this.formatDateService.convertToUTCTime(this.denNgay);

    this.awaitRes = true;
    let result: any = await this.salaryService.getChamCongOtByEmpId(tuNgay, denNgay, this.employeeId);
    this.awaitRes = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.listLoaiOt = result.listLoaiOt;
    this.trRow = result.listDataHeader;
    this.buildData(result.listData);
  }

  buildData(list: Array<any>) {
    this.cols = [];
    this.listData = [];

    if (list.length) {
      list[0].forEach(item => {
        if (item.isShow) {
          let col = {
            field: item.columnKey,
            header: item.columnKey,
            width: item.width,
            textAlign: item.textAlign
          };
          this.cols.push(col);
        }
      });

      list.forEach(row => {
        let dataRow = {};
        row.forEach(item => {
          //Nếu là number
          if (item.valueType == 1) {
            dataRow[item.columnKey] = this.commonService.convertStringToNumber(item.columnValue);
          }
          else {
            dataRow[item.columnKey] = item.columnValue;
          }
        });

        this.listData = [...this.listData, dataRow];
      });
    }
  }

  changeGio(data: any, field: string) {
    this.displayUpdate = true;
    
    this.ngayChamCong = this.commonService.convertStringToDate(data.ngay, 'dd/MM/yyyy');
    this.type = field.split('_')[1];
    this.loaiOtId = field.split('_')[2];
    this.loaiOt = this.listLoaiOt.find(x => x.categoryId == this.loaiOtId)?.categoryName;
    let thoiGian = data[field] == '--' ? null : data[field];
    this.thoiGian = this.commonService.convertTimeSpanToDate(thoiGian);

    switch (this.type) {
      case 'vs': this.ca = 'vào sáng'; break;
      case 'rs': this.ca = 'ra sáng'; break;
      case 'vc': this.ca = 'vào chiều'; break;
      case 'rc': this.ca = 'ra chiều'; break;
      case 'vt': this.ca = 'vào tối'; break;
      case 'rt': this.ca = 'ra tối'; break;
      default: this.ca = null; break;
    }
  }

  async update() {
    let thoiGian = this.commonService.convertDateToTimeSpan(this.thoiGian);
    this.awaitRes = true;
    let result: any = await this.salaryService.createOrUpdateChamCongOt(this.employeeId, this.loaiOtId, this.ngayChamCong, this.type, thoiGian);
    this.awaitRes = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showMessage("success", result.messageCode);
    await this.getData();
    this.displayUpdate = false;
  }

  closeUpdate() {
    this.displayUpdate = false;
  }

  close() {
    this.ref.close(true);
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }
}
