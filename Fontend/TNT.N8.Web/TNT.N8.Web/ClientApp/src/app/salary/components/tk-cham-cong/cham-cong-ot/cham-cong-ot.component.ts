import { FormatDateService } from './../../../../shared/services/formatDate.services';
import { Router } from '@angular/router';
import { CommonService } from './../../../../shared/services/common.service';
import { SalaryService } from './../../../services/salary.service';
import { Component, OnInit, Input, ViewChild, SimpleChanges } from '@angular/core';
import { Table } from 'primeng/table';
import { ConfirmationService, MessageService } from "primeng/api";
import { DialogService } from 'primeng/dynamicdialog';
import { ChamCongOtChiTietDialogComponent } from '../cham-cong-ot-chi-tiet-dialog/cham-cong-ot-chi-tiet-dialog.component';
import * as XLSX from 'xlsx';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";

@Component({
  selector: 'app-cham-cong-ot',
  templateUrl: './cham-cong-ot.component.html',
  styleUrls: ['./cham-cong-ot.component.css']
})
export class ChamCongOtComponent implements OnInit {
  loading: boolean = false;
  awaitResponse: boolean = false;

  @Input() actionEdit: boolean;
  @Input() actionDownload: boolean;
  @Input() actionImport: boolean;
  @Input() listEmployee: Array<any> = [];
  @Input() tkChamCongOtObjectId: string;

  isShowFilter: boolean = false;
  leftColNumber: number = 12;
  rightColNumber: number = 0;

  listSelectedEmployee: Array<any> = [];
  listDieuKienHienThi: Array<any> = [
    { value: 1, name: 'Nhân viên đang làm việc' },
    { value: 2, name: 'Nhân viên đã nghỉ việc' },
  ];
  listSelectedDieuKienHienThi: Array<any> = [
    this.listDieuKienHienThi.find(x => x.value == 1)
  ];
  tuNgay: Date = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
  denNgay: Date = new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0);

  @ViewChild('myTable') myTable: Table;
  filterGlobal: string = null;
  cols: Array<any> = [];
  trRow: Array<any> = [];
  listData: Array<any> = [];
  selectedRow: any = null;

  refreshNote: number = 0;

  constructor(
    private salaryService: SalaryService,
    private commonService: CommonService,
    private router: Router,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private formatDateService: FormatDateService,
    private dialogService: DialogService
  ) { }

  ngOnInit(): void {
    this.getData();
  }

  /* Nếu Input có thay đổi */
  ngOnChanges(changes: SimpleChanges) {
    
  }

  async getData() {
    let tuNgay = this.formatDateService.convertToUTCTime(this.tuNgay);
    let denNgay = this.formatDateService.convertToUTCTime(this.denNgay);
    let listEmployeeId = this.listSelectedEmployee.map(x => x.employeeId);
    let isShowOption = 0;
    if (this.listSelectedDieuKienHienThi.length == 1) {
      isShowOption = this.listSelectedDieuKienHienThi[0].value;
    }

    if (!tuNgay || !denNgay) {
      this.showMessage("warn", 'Từ ngày, Đến ngày không được để trống');
      return;
    }

    let oneDay = 24 * 60 * 60 * 1000;
    let diffDays = Math.round(Math.abs((+tuNgay - +denNgay) / oneDay));
    if (diffDays > 31) {
      this.showMessage("warn", 'Giới hạn tìm kiếm chỉ trong một tháng');
      return;
    }

    this.loading = true;
    this.awaitResponse = true;
    let result: any = await this.salaryService.getTkThoiGianOt(tuNgay, denNgay, listEmployeeId, isShowOption);
    this.loading = false;
    this.awaitResponse = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

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

  viewChiTiet(data) {
    if (this.tuNgay == null || this.denNgay == null) {
      this.showMessage('warn', 'Bạn chưa chọn đủ bộ lọc các trường Từ ngày, Đến ngày');
      return;
    }
    
    let ref = this.dialogService.open(ChamCongOtChiTietDialogComponent, {
      header: 'Chi tiết chấm công OT',
      width: '50%',
      baseZIndex: 1030,
      closable: false,
      contentStyle: {
        "min-height": "auto",
        "max-height": "700px",
      },
      data: {
        tuNgay: this.tuNgay,
        denNgay: this.denNgay,
        employeeId: data.employee_id,
        name: data.name,
        code: data.code
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        this.getData();
        this.refreshNote++;
      }
    });
  }

  exportExcel() {
    //Đi muộn về sớm
    if (this.listData.length > 0) {
      let title = 'THỐNG KÊ CHẤM CÔNG OT';
      let workbook = new Workbook();
      let worksheet = workbook.addWorksheet('BAO_CAO');

      /* title */
      let titleRow = worksheet.addRow([title]);
      titleRow.font = { family: 4, size: 16, bold: true };
      titleRow.height = 25;
      worksheet.mergeCells(`A${titleRow.number}:T${titleRow.number}`)
      titleRow.alignment = { vertical: 'middle', horizontal: 'center' };

      worksheet.addRow([]);
      worksheet.addRow([]);

      /* header */
      let header1 = [];
      this.trRow[0].forEach(item => {
        header1.push(item.columnValue);
      });

      let headerRow1 = worksheet.addRow(header1);

      headerRow1.height = 20;
      headerRow1.font = { size: 10, bold: true };

      header1.forEach((item, index) => {
        headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow1.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      });

      /* data table */
      let data = this.listData.map((item, index) => {
        let row = [];

        Object.keys(item).forEach(key => {
          if (key != 'employee_id') {
            row.push(item[key]); 
          }
        });

        return row;
      });

      data.forEach((el, index) => {
        let row = worksheet.addRow(el);

        for (let i = 0; i < el.length; i++) {
          row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

          if (i == 0 || i == 1) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          }
          else if (i == 2) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
          }
          else {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          }
        }
      });

      /* set width */
      worksheet.getColumn(1).width = 10;
      worksheet.getColumn(2).width = 10;
      worksheet.getColumn(3).width = 30;

      for (let i = 1; i <= header1.length; i++) {
        if (i > 3) {
          worksheet.getColumn(i).width = 35;
        }
      }

      workbook.xlsx.writeBuffer().then((data) => {
        let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        saveAs.saveAs(blob, title);
      });
    }
  }

  refreshFilter() {
    this.selectedRow = null;
    this.tuNgay = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
    this.denNgay = new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0);
    this.listSelectedEmployee = [];
    this.listSelectedDieuKienHienThi = [this.listDieuKienHienThi.find(x => x.value == 1)];
    this.refreshNote++;

    this.getData();
  }

  showFilter() {
    this.isShowFilter = !this.isShowFilter;
    if (this.isShowFilter) {
      this.leftColNumber = 9;
      this.rightColNumber = 3;
    }
    else {
      this.leftColNumber = 12;
      this.rightColNumber = 0;
    }
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }

}
