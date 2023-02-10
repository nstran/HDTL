import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ManufactureService } from '../../services/manufacture.service';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { DatePipe } from '@angular/common';
import { formatDate } from '@angular/common';
import { formatNumber } from '@angular/common';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng';

class productionOrderModel {
  productionOrderId: string;
  productionOrderCode: string;
}

class columnModel {
  column1: string;
  column2: string;
}

class configDataModel {
  organizationCode: string;
}

class searchReportModel {
  OrganizationCode: string;
  ListProductionOrderId: Array<string>;
  constructor() {
    this.ListProductionOrderId = [];
  }
}

class trackProductionReportModel {
  stt: number;
  index: number;
  customerName: string;
  productColor: string;
  productLength: number;
  productMaterial: string;
  productThickness: number;
  productWidth: number;
  productionDate: Date;
  productionOrderCode: string;
  productionOrderId: string;
  quantity: number;
  techniqueDescription: string;
}

@Component({
  selector: 'app-export-track-production-dialog',
  templateUrl: './export-track-production-dialog.component.html',
  styleUrls: ['./export-track-production-dialog.component.css']
})

export class ExportTrackProductionDialogComponent implements OnInit {
  loading: boolean = false;
  listProductionOrder: Array<productionOrderModel> = [];
  //form
  exportForm: FormGroup;
  //router variable
  organizationCode: string;
  //data
  listTrackProductionReport: Array<trackProductionReportModel> = [];

  constructor(public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private manufactureService: ManufactureService) { }

  ngOnInit() {
    if (this.config.data) {
      let configData: configDataModel = this.config.data;
      this.organizationCode = configData.organizationCode;
    }

    this.initForm();
    this.getMasterdata();
  }

  initForm() {
    this.exportForm = new FormGroup({
      "ProductionOrderCode": new FormControl([], [Validators.required])
    });
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.manufactureService.getDataExportTrackProduction(this.organizationCode);
    this.loading = false;
    if (result.statusCode == 200) {
      this.listProductionOrder = result.listProductionOrder;
    } else {
    }
  }

  async exportReport() {
    if (!this.exportForm.valid) {
      Object.keys(this.exportForm.controls).forEach(key => {
        if (this.exportForm.controls[key].valid == false) {
          this.exportForm.controls[key].markAsTouched();
        }
      });
      return;
    }

    let searchReport = new searchReportModel();
    searchReport.OrganizationCode = this.organizationCode;
    let listProductionOrder: Array<productionOrderModel> = this.exportForm.get('ProductionOrderCode').value;
    let listProductionOrderId = listProductionOrder.map(e => e.productionOrderId);
    searchReport.ListProductionOrderId = listProductionOrderId;
    this.loading = true;
    let result: any = await this.manufactureService.getTrackProductionReport(searchReport.OrganizationCode, searchReport.ListProductionOrderId);
    this.loading = false;
    if (result.statusCode === 200) {
       this.listTrackProductionReport = result.listTrackProductionReport;
       this.setIndexTrackProductionReport();
       this.export(this.listTrackProductionReport);
    }
  }

  setIndexTrackProductionReport() {
    let temp = 0;
    for (let i = 0; i < this.listTrackProductionReport.length; i++) {
      this.listTrackProductionReport[i].index = temp +1;
      temp++;
      if (this.listTrackProductionReport[i + 1]) {
        if (this.listTrackProductionReport[i].productionOrderCode != this.listTrackProductionReport[i + 1].productionOrderCode) temp = 0;
      }
    }
  }

  export(listTrackProductionReport: Array<trackProductionReportModel>) {
    let title = `THEO DÕI SẢN XUẤT`;
    let workBook = new Workbook();
    let worksheet = workBook.addWorksheet(title);
    /* title */
    // let titleRow = worksheet.addRow([title]);
    // titleRow.font = { family: 4, size: 16, bold: true };
    // titleRow.height = 25;
    // titleRow.alignment = { vertical: 'middle', horizontal: 'center' };
    // worksheet.mergeCells(`A${titleRow.number}:K${titleRow.number}`);
    // titleRow.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, right: { style: "thin" } };
    //HEADER
    let header: Array<string> = ["STT", "MÃ ĐƠN", "CHIỀU DÀI", "CHIỀU RỘNG", "MÃ HIỆU", "SỐ LƯỢNG", "KHÁCH HÀNG", "MÀU SẮC", "CHỦNG LOẠI", "NGÀY SẢN XUẤT", "THÀNH PHẦN"];
    let headerRow = worksheet.addRow(header);
    headerRow.font = { name: 'Time New Roman', size: 10, bold: true };
    header.forEach((item, index) => {
      headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    });
    headerRow.height = 30;
    //handle data for single report type
    let data: Array<any> = this.getReportData(listTrackProductionReport);
    data.forEach(e => {
      let row = worksheet.addRow(e);
      let totalColumns = header.length;
      for (let i = 1; i <= totalColumns; i++) {
        row.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(i).alignment = { vertical: 'middle', horizontal: 'center' };
      }
    });
    /* set width */
    let width = header.map((e: any) => e.length);
    header.forEach((e, index) => {
      if (e != null) {
        if (e.length > width[index]) {
          width[index] = e.length;
        }
      }
    });
    data.forEach(row => {
      row.forEach((cell, index) => {
        if (cell != null) {
          if (cell.length > width[index])
            width[index] = cell.length;
        }
      });
    });
    width.forEach((el, index) => {
      worksheet.getColumn(index + 1).width = el + 5;
    });
    //export
    this.exportToExel(workBook, title);
  }

  getReportData(listTrackProductionReport: Array<trackProductionReportModel>): Array<any> {
    let data: Array<any> = [];

    listTrackProductionReport.forEach((e) => {
      let row: Array<any> = [];
      row[0] = ConvertToString(e.stt);
      row[1] = ConvertToString(e.productionOrderCode);
      row[2] = ConvertToString(e.productLength);
      row[3] = ConvertToString(e.productWidth);
      row[4] = ConvertToString(e.techniqueDescription);
      row[5] = ConvertToString(e.quantity);
      row[6] = ConvertToString(e.customerName);
      row[7] = ConvertToString(e.productColor);
      row[8] = ConvertToString(e.productThickness);
      row[9] = this.transformDate(e.productionDate);
      row[10] = ConvertToString(e.productMaterial);

      data.push(row);
    });

    return data;
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  transformDate(date: Date) {
    return formatDate(date, 'dd/MM/yyyy', 'EN');
  }

  transformNumber(input: any) {
    return formatNumber(input, 'EN',);
  }
}

function ConvertToString(str: any) {
  if (!str) return "";
  return String(str).trim();
}

function ConvertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

