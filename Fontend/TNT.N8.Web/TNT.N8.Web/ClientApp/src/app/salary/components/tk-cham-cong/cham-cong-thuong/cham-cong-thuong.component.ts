import { UpdateChamCongDialogComponent } from './../../update-cham-cong-dialog/update-cham-cong-dialog.component';
import { FormatDateService } from './../../../../shared/services/formatDate.services';
import { Router } from '@angular/router';
import { CommonService } from './../../../../shared/services/common.service';
import { SalaryService } from './../../../services/salary.service';
import { Component, OnInit, Input, ViewChild, SimpleChanges } from '@angular/core';
import { ConfirmationService, MessageService } from "primeng/api";
import { DialogService } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import * as XLSX from 'xlsx';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";

@Component({
  selector: 'app-cham-cong-thuong',
  templateUrl: './cham-cong-thuong.component.html',
  styleUrls: ['./cham-cong-thuong.component.css']
})
export class ChamCongThuongComponent implements OnInit {
  loading: boolean = false;
  awaitResponse: boolean = false;

  @Input() actionEdit: boolean;
  @Input() actionDownload: boolean;
  @Input() actionImport: boolean;
  @Input() listEmployee: Array<any> = [];
  @Input() listKyHieuChamCong: Array<any> = [];
  @Input() tkChamCongThuongObjectId: string;

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
  frozenCols: Array<any> = [];
  trRow: Array<any> = [];
  trRowFix: Array<any> = [];
  trRowFrozen: Array<any> = [];
  listData: Array<any> = [];
  listColorChamCong: Array<any> = [];
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

    this.selectedRow = null;

    this.loading = true;
    this.awaitResponse = true;
    let result: any = await this.salaryService.getTkDiMuonVeSom(tuNgay, denNgay, listEmployeeId, isShowOption);
    this.loading = false;
    this.awaitResponse = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.trRow = result.listDataHeader;
    this.trRowFix[0] = this.trRow[0];
    this.trRowFrozen = [result.listDataHeader[0].filter(x => x.columnKey == "stt" || x.columnKey == "code" || x.columnKey == "name")];
    this.trRow[0] = this.trRow[0].filter(x => x.columnKey != "stt" && x.columnKey != "code" && x.columnKey != "name")

    this.listColorChamCong = result.listColorChamCong;
    this.buildData(result.listData);
  }

  buildData(list: Array<any>) {
    this.cols = [];
    this.frozenCols = [];

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
          if (item.columnKey == 'stt' || item.columnKey == 'name' || item.columnKey == 'code') {
            this.frozenCols.push(col);
          } else {
            this.cols.push(col);
          }
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

  getColorChamCong(data, key) {
    let indexGio = this.commonService.convertStringToNumber(key.slice(key.lastIndexOf('_') + 1));

    //Lấy ngày
    let indexNgay = 0;
    if (indexGio % 4 == 0) {
      indexNgay = indexGio / 4;
    }
    else {
      indexNgay = Math.floor(indexGio / 4) + 1;
    }

    let ngay = this.getNgay(this.trRow[0].find(x => x.columnKey == 'index_ngay_' + indexNgay).columnValue);

    let colorCode = this.listColorChamCong.find(x => x.code == data.code &&
      x.ngayChamCong == ngay && x.index == key)?.colorCode;

    if (!this.selectedRow) {
      return colorCode;
    }
    else {
      if (this.selectedRow == data && colorCode != '#f44336') {
        colorCode = '#fff';
      }

      return colorCode;
    }
  }

  changeKyHieu(item: any) {
    // Nếu chọn ngày
    if (item.columnKey.includes('index_ngay_')) {
      let ngay = this.getNgay(item.columnValue);
      let listEmpId = this.listData.map(x => x.employee_id);
      let listEmployee = this.listEmployee.filter(x => listEmpId.includes(x.employeeId));

      let ref = this.dialogService.open(UpdateChamCongDialogComponent, {
        header: 'Cập nhật chấm công',
        width: '30%',
        baseZIndex: 1030,
        closable: false,
        contentStyle: {
          "min-height": "auto",
          "max-height": "600px",
        },
        data: {
          typeUpdate: "NGAY",
          ngay: ngay,
          ca: null,
          gio: null,
          name: name,
          employeeId: null,
          indexGio: null,
          indexCa: null,
          listEmployee: listEmployee,
          listKyHieuChamCong: this.listKyHieuChamCong
        }
      });

      ref.onClose.subscribe((result: any) => {
        if (result) {
          this.getData();
          this.refreshNote++;
        }
        else if (result == false) {
          this.showMessage('error', "Lưu thất bại");
        }
      });
    }
    // Nếu chọn ca
    else if (item.columnKey.includes('index_ca')) {
      let indexCa = this.commonService.convertStringToNumber(item.columnKey.slice(item.columnKey.lastIndexOf('_') + 1));
      let indexNgay = 0;
      if (indexCa % 2 == 0) {
        indexNgay = indexCa / 2;
      }
      else {
        indexNgay = Math.floor(indexCa / 2) + 1;
      }

      let ngay = this.getNgay(this.trRow[0].find(x => x.columnKey == 'index_ngay_' + indexNgay).columnValue);
      let listEmpId = this.listData.map(x => x.employee_id);
      let listEmployee = this.listEmployee.filter(x => listEmpId.includes(x.employeeId));

      let ref = this.dialogService.open(UpdateChamCongDialogComponent, {
        header: 'Cập nhật chấm công',
        width: '30%',
        baseZIndex: 1030,
        closable: false,
        contentStyle: {
          "min-height": "auto",
          "max-height": "600px",
        },
        data: {
          typeUpdate: "CA",
          ngay: ngay,
          ca: item.columnValue,
          gio: null,
          name: name,
          employeeId: null,
          indexGio: null,
          indexCa: indexCa,
          listEmployee: listEmployee,
          listKyHieuChamCong: this.listKyHieuChamCong
        }
      });

      ref.onClose.subscribe((result: any) => {
        if (result) {
          this.getData();
          this.refreshNote++;
        }
        else if (result == false) {
          this.showMessage('error', "Lưu thất bại");
        }
      });
    }
  }

  checkKyHieu(kyHieu: string) {
    let exists = this.listKyHieuChamCong.find(x => x.valueText == kyHieu);
    return (exists ? true : false);
  }

  changeGio(data, key) {
    let gio = data[key];
    let isKyHieu = this.checkKyHieu(gio);

    //Nếu là ký hiệu thì không cho sửa thời gian
    if (isKyHieu) {
      return;
    }

    let indexGio = this.commonService.convertStringToNumber(key.slice(key.lastIndexOf('_') + 1));

    //Lấy ngày
    let indexNgay = 0;
    if (indexGio % 4 == 0) {
      indexNgay = indexGio / 4;
    }
    else {
      indexNgay = Math.floor(indexGio / 4) + 1;
    }

    let ngay = this.getNgay(this.trRow[0].find(x => x.columnKey == 'index_ngay_' + indexNgay).columnValue);

    //Lấy ca
    let indexCa = 0;
    if (indexGio % 2 == 0) {
      indexCa = indexGio / 2;
    }
    else {
      indexCa = Math.floor(indexGio / 2) + 1;
    }

    let ca = this.trRow[1].find(x => x.columnKey == 'index_ca_' + indexCa).columnValue;
    let name = data.name;
    let employeeId = data.employee_id;

    let ref = this.dialogService.open(UpdateChamCongDialogComponent, {
      header: 'Cập nhật chấm công',
      width: '30%',
      baseZIndex: 1030,
      closable: false,
      contentStyle: {
        "min-height": "auto",
        "max-height": "600px",
      },
      data: {
        typeUpdate: "GIO",
        ngay: ngay,
        ca: ca,
        gio: gio,
        name: name,
        employeeId: employeeId,
        indexGio: indexGio,
        indexCa: indexCa,
        listEmployee: [],
        listKyHieuChamCong: []
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        this.getData();
        this.refreshNote++;
      }
      else if (result == false) {
        this.showMessage('error', "Lưu thất bại");
      }
    });
  }

  exportExcel() {
    //Đi muộn về sớm
    if (this.listData.length > 0) {
      let title = 'THỐNG KÊ ĐI MUỘN VỀ SỚM';
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
      this.trRowFix[0].forEach(item => {
        if (item.columnKey.includes('index_ngay')) {
          for (let i = 1; i <= 4; i++) {
            if (i == 1) {
              header1.push(item.columnValue);
            }
            else {
              header1.push('');
            }
          }
        }
        else {
          header1.push(item.columnValue);
        }
      });

      let header2 = ['', '', '', '', '', '', ''];
      this.trRow[1].forEach(item => {
        for (let i = 1; i <= 2; i++) {
          if (i == 1) {
            header2.push(item.columnValue);
          }
          else {
            header2.push('');
          }
        }
      });

      let header3 = ['', '', '', '', '', '', ''];
      this.trRow[2].forEach(item => {
        header3.push(item.columnValue);
      });

      let headerRow1 = worksheet.addRow(header1);
      let headerRow2 = worksheet.addRow(header2);
      let headerRow3 = worksheet.addRow(header3);

      headerRow1.height = 20;
      headerRow1.font = { size: 10, bold: true };
      for (let i = 1; i <= header1.length; i++) {
        if (i <= 7) {
          headerRow1.getCell(i).worksheet.mergeCells(
            `${headerRow1.getCell(i).$col$row}:${headerRow3.getCell(i).$col$row}`
          );
        }
        else {
          if (header1[i - 1] != '') {
            headerRow1.getCell(i).worksheet.mergeCells(
              `${headerRow1.getCell(i).$col$row}:${headerRow1.getCell(i + 3).$col$row}`
            );
          }
        }
      }

      headerRow2.height = 20;
      headerRow2.font = { size: 10, bold: true };
      for (let i = 1; i <= header2.length; i++) {
        if (i > 7 && header2[i - 1] != '') {
          headerRow2.getCell(i).worksheet.mergeCells(
            `${headerRow2.getCell(i).$col$row}:${headerRow2.getCell(i + 1).$col$row}`
          );
        }
      }

      headerRow3.height = 20;
      headerRow3.font = { size: 10, bold: true };

      header1.forEach((item, index) => {
        headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow1.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      });

      header2.forEach((item, index) => {
        headerRow2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow2.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow2.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      });

      header3.forEach((item, index) => {
        headerRow3.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow3.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow3.getCell(index + 1).fill = {
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

          if (i == 0 || i == 1 || i > 6) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

            //highlight đỏ giờ đi sớm về muộn
            if (i > 6) {
              let indexGio = i - 6;
              let code = el[1];

              //Lấy ngày
              let indexNgay = 0;
              if (indexGio % 4 == 0) {
                indexNgay = indexGio / 4;
              }
              else {
                indexNgay = Math.floor(indexGio / 4) + 1;
              }

              let ngay = this.getNgay(this.trRow[0].find(x => x.columnKey == 'index_ngay_' + indexNgay).columnValue);
              let colorCode = this.listColorChamCong.find(x => x.code == code &&
                x.ngayChamCong == ngay && x.index == 'index_' + indexGio)?.colorCode;
              row.getCell(i + 1).font = { color: { argb: colorCode?.substring(1) } };
            }
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
      worksheet.getColumn(1).width = 5;
      worksheet.getColumn(2).width = 5;
      worksheet.getColumn(3).width = 25;
      worksheet.getColumn(4).width = 10;
      worksheet.getColumn(5).width = 10;
      worksheet.getColumn(6).width = 10;
      worksheet.getColumn(7).width = 10;

      for (let i = 1; i <= header3.length; i++) {
        if (i > 7) {
          worksheet.getColumn(i).width = 10;
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

  getNgay(data: string) {
    let result = '';
    let index = data.indexOf('-');

    if (index != -1) {
      result = data.slice(0, index).trim();
    }
    else {
      result = data;
    }

    return result;
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }

}
