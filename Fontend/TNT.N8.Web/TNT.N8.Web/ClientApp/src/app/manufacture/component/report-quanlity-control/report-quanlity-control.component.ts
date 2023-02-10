import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { ManufactureService } from '../../services/manufacture.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService, SortEvent } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import * as moment from 'moment';
import * as $ from 'jquery';
import 'moment/locale/pt-br';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { DatePipe } from '@angular/common';
import { formatDate } from '@angular/common';
import { formatNumber } from '@angular/common';
import { DialogService } from 'primeng';

class techniqueRequest {
  techniqueRequestId: string;
  techniqueName: string;
}

class qualityControlNote {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}

class errorType {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}

class thicknessFilter {
  id: number;
  label: string;
}

class searchFormModel {
  listTechniqueRequestId: Array<string>;
  listThicknessOptionId: Array<number>;
  fromDate: Date;
  toDate: Date;
  constructor() {
    this.listTechniqueRequestId = [];
    this.listThicknessOptionId = [];
  }
}

class quanlityControlReport {
  productionOrderHistoryId: string;
  createdDate: Date;//Ngày tháng
  productionOrderCode: string;//Lệnh số
  productionOrderId: string;
  customerName: string; //Tên khách hàng
  productName: string; //Sản phẩm
  productLength: number;//Kích thước: Dài
  productWidth: number;//Kích thước: Rộng
  quantity: number; //Số tấm
  totalArea: number;//Số m2
  productThickness: number; //Độ dày
  productColor: string;//Màu
  //TODO: khoan khoet
  borehole: number;
  hole: number;
  description: string;//Ghi chú tổ SX
  noteQc: string;//Ghi chú QC
  errorType: string// Loại lỗi
  //bien table
  selectedQcNote: qualityControlNote;
  selectedErrorType: errorType;
  noteQcName: string;
  errorTypeName: string;
}

class techniqueRequestReport {
  techniqueName: string;
  techniqueRequestId: string;

  totalArea: number;
  totalAreaError: number;
  totalAreaErrorByManufacture: number;
  totalAreaErrorByMaterials: number;
  totalAreaErrorByTechEquipments: number;
  totalAreaErrorByThickGlass: number;
  totalAreaErrorByThinGlass: number;

  totalAreaByPercent: number;
  totalAreaErrorByPercent: number;
  totalAreaErrorByManufactureByPercent: number;
  totalAreaErrorByMaterialsByPercent: number;
  totalAreaErrorByTechEquipmentsByPercent: number;
  totalAreaErrorByThickGlassByPercent: number;
  totalAreaErrorByThinGlassByPercent: number;
}

class sumaryExcelModel {
  errorType: errorType;
  sumQuantity: number;
  sumArea: number;
  constructor() {
    this.sumQuantity = 0;
    this.sumArea = 0;
  }
}

@Component({
  selector: 'app-report-quanlity-control',
  templateUrl: './report-quanlity-control.component.html',
  styleUrls: ['./report-quanlity-control.component.css'],
  providers: [DatePipe]
})
export class ReportQuanlityControlComponent implements OnInit {
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  loading: boolean = false;
  //hardcode
  listThickNessFilter: Array<thicknessFilter> = [
    { id: 1, label: 'Dày' },
    { id: 2, label: 'Mỏng' }
  ];
  //search form
  currentYear: number = new Date().getFullYear();
  minYear = this.currentYear - 10;
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  searchForm: FormGroup;
  //download form
  downloadErrorProductForm: FormGroup;
  downloadReportForm: FormGroup;

  // table
  innerWidth: number = 0;
  @ViewChild('myTable') myTable: Table;
  cols: any[];
  selectedColumns: any[];
  bodyColumns: any[];
  frozenCols: any[];
  selectedReport: quanlityControlReport;
  //masterdata;
  listTechniqueRequest: Array<techniqueRequest> = [];
  listErrorType: Array<errorType> = [];
  listQualityControlNote: Array<qualityControlNote> = [];
  listTechniqueRequestReport: Array<techniqueRequestReport> = [];
  //search data
  listQuanlityControlReport: Array<quanlityControlReport> = [];
  //excel
  listSumaryError: Array<sumaryExcelModel> = [];
  constructor(
    private datePipe: DatePipe,
    private router: Router,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private manufactureService: ManufactureService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private renderer: Renderer2,
    private confirmationService: ConfirmationService,
  ) {
    this.innerWidth = window.innerWidth;
    $("body").addClass("sidebar-collapse");
  }

 async  ngOnInit() {
    let resource = "man/manufacture/report/quanlity-control";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }

    this.initForm();
    this.initTable();
    this.getMasterdata();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  initForm() {
    this.searchForm = new FormGroup({
      'TechniqueRequest': new FormControl(null),
      'Thickness': new FormControl([]),
      'FromDate': new FormControl(null),
      'ToDate': new FormControl(null)
    });
    this.downloadErrorProductForm = new FormGroup({
      'TechniqueRequest': new FormControl(null, [Validators.required]),
      'Thickness': new FormControl([]),
      'FromDate': new FormControl(null),
      'ToDate': new FormControl(null)
    });
    this.downloadReportForm = new FormGroup({
      'FromDate': new FormControl(null),
      'ToDate': new FormControl(null)
    });
  }

  setDefaultForm() {
    //patch data sau khi lấy master data
    this.searchForm.setValue({
      'TechniqueRequest': this.listTechniqueRequest[0],
      'Thickness': [],
      'FromDate': null,
      'ToDate': null
    });
  }

  resetDownloadErrorProductForm() {
    this.downloadErrorProductForm.reset();
    this.downloadErrorProductForm.setValue({
      'TechniqueRequest': null,
      'Thickness': [],
      'FromDate': null,
      'ToDate': null
    });
  }

  resetDownloadReportForm() {
    this.downloadReportForm.reset();
    this.downloadReportForm.setValue({
      'FromDate': null,
      'ToDate': null
    });
  }

  resetSearchForm() {
    this.searchForm.reset();
    this.setDefaultForm();
  }

  initTable() {
    // this.cols = [
    //   { field: 'createdDate', header: 'Ngày tháng', textAlign: 'left', width: '100px !important', rowspan: 2, colspan: 1, headerRow: 1 },
    //   { field: 'productionOrderCode', header: 'Lệnh số', textAlign: 'left', width: '100px !important', rowspan: 2, colspan: 1, headerRow: 1 },
    //   { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left', width: '50px !important', rowspan: 2, colspan: 1, headerRow: 1 },
    //   { field: 'productName', header: 'Tên sản phẩm', textAlign: 'left', width: '100px !important', rowspan: 2, colspan: 1, headerRow: 1 },

    //   { field: 'size', header: 'Kích thước', textAlign: 'center', width: '30px !important', rowspan: 1, colspan: 2, headerRow: 1 },

    //   { field: 'productLength', header: 'Dài', textAlign: 'center', width: '15px !important', rowspan: 1, colspan: 1, headerRow: 2 },
    //   { field: 'productWidth', header: 'Rộng', textAlign: 'center', width: '15px !important', rowspan: 1, colspan: 1, headerRow: 2 },

    //   { field: 'quantity', header: 'Số tấm', textAlign: 'center', width: '30px !important', rowspan: 2, colspan: 1, headerRow: 1 },
    //   { field: 'totalArea', header: 'Số m2', textAlign: 'center', width: '30px !important', rowspan: 2, colspan: 1, headerRow: 1 },
    //   { field: 'productThickness', header: 'Độ dày', textAlign: 'center', width: '30px !important', rowspan: 2, colspan: 1, headerRow: 1 },
    //   { field: 'productColor', header: 'Màu sắc', textAlign: 'center', width: '40px !important', rowspan: 2, colspan: 1, headerRow: 1 },
    //   { field: 'drill', header: 'Khoan khoét', textAlign: 'center', width: '30px !important', rowspan: 2, colspan: 1, headerRow: 1 },
    //   { field: 'description', header: 'Ghi chú tổ SX', textAlign: 'center', width: '30px !important', rowspan: 2, colspan: 1, headerRow: 1 },
    //   { field: 'noteQcName', header: 'Ghi chú QC', textAlign: 'center', width: '200px !important', rowspan: 2, colspan: 1, headerRow: 1 },
    //   { field: 'errorTypeName', header: 'Loại lỗi', textAlign: 'center', width: '100px !important', rowspan: 2, colspan: 1, headerRow: 1 },
    // ];
    // this.selectedColumns = this.cols;

    // this.bodyColumns = this.cols.filter(e => e.field != 'size');
    // this.frozenCols = this.cols.filter(e => e.field == 'noteQc');


    this.cols = [
       { field: 'createdDate', header: 'Ngày tháng', textAlign: 'left', width: '100px' },
      { field: 'productionOrderCode', header: 'Lệnh số', textAlign: 'left', width: '140px' },
      { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left', width: '280px' },
      { field: 'productName', header: 'Tên sản phẩm', textAlign: 'left', width: '300px' },
      //  { field: 'size', header: 'Kích thước', textAlign: 'center', width: '200px' },
      { field: 'productLength', header: 'Dài', textAlign: 'center', width: '150px' },
      { field: 'productWidth', header: 'Rộng', textAlign: 'center', width: '150px' },

      { field: 'quantity', header: 'Số tấm', textAlign: 'center', width: '100px' },
      { field: 'totalArea', header: 'Số m2', textAlign: 'center', width: '150px' },
      { field: 'productThickness', header: 'Độ dày', textAlign: 'center', width: '100px' },
      { field: 'productColor', header: 'Màu sắc', textAlign: 'center', width: '100px' },
      { field: 'borehole', header: 'Khoan', textAlign: 'center', width: '150px' },
      { field: 'hole', header: 'Khoét', textAlign: 'center', width: '150px' },
      { field: 'description', header: 'Ghi chú tổ SX', textAlign: 'center', width: '200px' },
      { field: 'noteQcName', header: 'Ghi chú QC', textAlign: 'center', width: '200px' },
      { field: 'errorTypeName', header: 'Loại lỗi', textAlign: 'center', width: '200px' },
    ];
    // this.frozenCols = this.cols.filter(e => e.field == 'createdDate');
    this.selectedColumns = this.cols;


    // this.frozenCols = [
    //   { field: 'createdDate', header: 'Ngày tháng', textAlign: 'left', width: '100px' },
    // ];
  }

  // Show filter
  leftColNumber: number = 12;
  rightColNumber: number = 0;
  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.rightColNumber = 4;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

  refreshFilter() {
    this.listQuanlityControlReport = [];
    this.resetSearchForm();
    this.search();
  }

  displayErrorProductDialog: boolean = false;
  showErrorProductDialog() {
    this.displayErrorProductDialog = true;
    this.resetDownloadErrorProductForm();
  }

  displayTotalReportDialog: boolean = false;
  showTotalReportDialog() {
    this.displayTotalReportDialog = true;
    this.resetDownloadReportForm();
  }

  async exportErrorProduct() {
    if (!this.downloadErrorProductForm.valid) {
      Object.keys(this.downloadErrorProductForm.controls).forEach(key => {
        if (this.downloadErrorProductForm.controls[key].valid == false) {
          this.downloadErrorProductForm.controls[key].markAsTouched();
        }
      });
    } else {
      let searchModel = new searchFormModel();
      let techiqueRequest: techniqueRequest = this.downloadErrorProductForm.get('TechniqueRequest').value;
      let listThicknessFilter: Array<thicknessFilter> = this.downloadErrorProductForm.get('Thickness').value;
      let fromDate: Date = this.downloadErrorProductForm.get('FromDate').value;
      let toDate: Date = this.downloadErrorProductForm.get('ToDate').value;
      let listThicknessFilterId = listThicknessFilter.map(e => e.id);
      searchModel.listTechniqueRequestId.push(techiqueRequest.techniqueRequestId);
      searchModel.listThicknessOptionId = listThicknessFilterId;
      if (fromDate) {
        fromDate.setHours(0, 0, 0, 0);
      }
      if (toDate) {
        toDate.setHours(23, 59, 59, 999);
      }
      searchModel.fromDate = fromDate ? convertToUTCTime(fromDate) : null;
      searchModel.toDate = toDate ? convertToUTCTime(toDate) : null;

      this.loading = true;
      let result: any = await this.manufactureService.searchQuanlityControlReport(searchModel.listTechniqueRequestId, searchModel.listThicknessOptionId, searchModel.fromDate, searchModel.toDate);
      this.loading = false;

      let listQuanlityControlReport: Array<quanlityControlReport> = [];

      if (result.statusCode == 200) {

        listQuanlityControlReport = result.listQuanlityControlReport;

        if (listQuanlityControlReport.length === 0) {
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: "Không có dữ liệu" };
          this.showMessage(msg);
          return;
        }
        let techiqueRequest: techniqueRequest = this.downloadErrorProductForm.get('TechniqueRequest').value;
        let techiqueRequestName = techiqueRequest ? techiqueRequest.techniqueName.toString().toLocaleUpperCase() : '';

        listQuanlityControlReport.forEach(qcreport => {
          let qcnote = this.listQualityControlNote.find(cate => cate.categoryId == qcreport.noteQc);
          if (qcnote) {
            qcreport.noteQcName = qcnote.categoryName;
            qcreport.selectedQcNote = qcnote;
          }
          let errorNote = this.listErrorType.find(cate => cate.categoryId == qcreport.errorType);
          if (errorNote) {
            qcreport.errorTypeName = errorNote.categoryName;
            qcreport.selectedErrorType = errorNote;
          }
        });

        this.displayErrorProductDialog = false;
        this.exportErrorProductReport(listQuanlityControlReport, techiqueRequestName, searchModel);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    }
  }

  async exportTotalReport() {
    let searchModel = new searchFormModel();
    let fromDate: Date = this.downloadReportForm.get('FromDate').value;
    let toDate: Date = this.downloadReportForm.get('ToDate').value;
    searchModel.fromDate = fromDate;
    searchModel.toDate = toDate;
    if (fromDate) {
      fromDate.setHours(0, 0, 0, 0);
    }
    if (toDate) {
      toDate.setHours(23, 59, 59, 999);
    }

    searchModel.fromDate = fromDate ? convertToUTCTime(fromDate) : null;
    searchModel.toDate = toDate ? convertToUTCTime(toDate) : null;

    this.loading = true;
    let result: any = await this.manufactureService.exportManufactureReport(searchModel.fromDate, searchModel.toDate);
    this.loading = false;
    if (result.statusCode == 200) {
      this.listTechniqueRequestReport = result.listTechniqueRequestReport; //báo cáo tổng
      if (this.listTechniqueRequestReport.length === 0) {
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: "Không có dữ liệu" };
        this.showMessage(msg);
        return;
      }

      this.displayTotalReportDialog = false;
      this.exportReport(searchModel);
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  async changeNoteQc(event: any, rowData: quanlityControlReport) {
    let selectedQcNote = rowData.selectedQcNote;
    var updateQcNoteId: string = null;
    if (selectedQcNote) {
      updateQcNoteId = selectedQcNote.categoryId;
    }
    this.loading = true;
    let result: any = await this.manufactureService.updateProductionOrderHistoryNoteQcAndErroType(rowData.productionOrderHistoryId, updateQcNoteId, rowData.errorType, true, false);
    this.loading = false;
    if (result.statusCode == 200) {
      let msg = { severity: 'success', summary: 'Thông báo:', detail: "Thay đổi thành công" };
      this.showMessage(msg);
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  async changeErrorType(event: any, rowData: quanlityControlReport) {
    let selectedErrorType = rowData.selectedErrorType;
    var updateErorTypeId: string = null;
    if (selectedErrorType) {
      updateErorTypeId = selectedErrorType.categoryId;
    }
    this.loading = true;
    let result: any = await this.manufactureService.updateProductionOrderHistoryNoteQcAndErroType(rowData.productionOrderHistoryId, rowData.noteQc, updateErorTypeId, false, true);
    this.loading = false;
    if (result.statusCode == 200) {
      let msg = { severity: 'success', summary: 'Thông báo:', detail: "Thay đổi thành công" };
      this.showMessage(msg);
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.manufactureService.getDataReportQuanlityControl();
    this.loading = false;
    if (result.statusCode == 200) {
      this.listTechniqueRequest = result.listTechniqueRequest;
      this.listQualityControlNote = result.listQualityControlNote;
      this.listErrorType = result.listErrorType;
      this.setDefaultForm();
      this.search();
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  exportErrorProductReport(listQuanlityControlReport: Array<quanlityControlReport>, techniqueRequestName: string, searchModel: searchFormModel) {
    let title = `THEO DÕI KÍNH VỠ TỔ ${techniqueRequestName}`;
    let workBook = new Workbook();
    let worksheet = workBook.addWorksheet(title);
    /* title */
    let titleRow = worksheet.addRow([title]);
    titleRow.font = { family: 4, size: 16, bold: true };
    titleRow.height = 25;
    titleRow.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A${titleRow.number}:L${titleRow.number}`);
    //titleRow.border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    titleRow.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, right: { style: "thin" } };
    /* desc */
    // let fromDate = `Từ ngày ${searchModel.fromDate.getDate()} tháng ${searchModel.fromDate.getMonth() + 1} năm ${searchModel.fromDate.getFullYear()}`;
    // let toDate = `đến ${searchModel.toDate.getDate()} tháng ${searchModel.toDate.getMonth() + 1} năm ${searchModel.toDate.getFullYear()}`;
    // let dateDesc = `Liên Ninh, ${fromDate} ${toDate}`;
    // let descRow = worksheet.addRow([dateDesc]);
    let fromDate = `Từ ngày ${searchModel.fromDate == null ? '' : searchModel.fromDate.getDate()} tháng ${searchModel.fromDate == null ? '' : searchModel.fromDate.getMonth() + 1} năm ${searchModel.fromDate == null ? '' : searchModel.fromDate.getFullYear()}`;
    let toDate = `đến ${this.downloadErrorProductForm.get('ToDate').value == null ? '' : this.downloadErrorProductForm.get('ToDate').value.getDate()} tháng ${this.downloadErrorProductForm.get('ToDate').value == null ? '' : this.downloadErrorProductForm.get('ToDate').value.getMonth() + 1} năm ${this.downloadErrorProductForm.get('ToDate').value == null ? '' : this.downloadErrorProductForm.get('ToDate').value.getFullYear()}`;
    let dateDesc = `Liên Ninh, ${fromDate} ${toDate}`;
    let descRow = worksheet.addRow([dateDesc]);

    worksheet.mergeCells(`A${descRow.number}:L${descRow.number}`);
    descRow.alignment = { vertical: 'middle', horizontal: 'right' };
    descRow.getCell(1).border = { left: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    /* header */
    let dataHeaderRow1 = ['STT', 'Ngày tháng', 'Lệnh số', 'Khách hàng', 'Kích thước', '', 'Số tấm', 'Số m2', 'Độ dày', 'Khoan', 'Khoét', 'Ghi chú', 'Lỗi'];
    let dataHeaderRow2 = ['', '', '', '', 'Dài', 'Rộng', '', '', '', '', '', '', ''];

    let headerRow1 = worksheet.addRow(dataHeaderRow1);
    let headerRow2 = worksheet.addRow(dataHeaderRow2);
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
    headerRow2.font = { name: 'Time New Roman', size: 10, bold: true };

    /* merge header */
    worksheet.mergeCells(`A${3}:A${4}`);
    worksheet.mergeCells(`B${3}:B${4}`);
    worksheet.mergeCells(`C${3}:C${4}`);
    worksheet.mergeCells(`D${3}:D${4}`);
    worksheet.mergeCells(`E${3}:F${3}`); //merge cell kich thuoc
    worksheet.mergeCells(`G${3}:G${4}`);
    worksheet.mergeCells(`H${3}:H${4}`);
    worksheet.mergeCells(`I${3}:I${4}`);
    worksheet.mergeCells(`J${3}:J${4}`);
    worksheet.mergeCells(`K${3}:K${4}`);
    worksheet.mergeCells(`L${3}:L${4}`);
    worksheet.mergeCells(`M${3}:M${4}`);

    dataHeaderRow1.forEach((item, index) => {
      headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow1.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };

    });
    headerRow1.height = 40;

    dataHeaderRow2.forEach((item, index) => {
      headerRow2.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow2.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
    });
    headerRow2.height = 40;

    let data: Array<any> = [];

    listQuanlityControlReport.forEach((item, index) => {
      let row: Array<any> = [];
      row[0] = index + 1;
      row[1] = this.transformDate(item.createdDate);
      row[2] = ConvertToString(item.productionOrderCode);
      row[3] = ConvertToString(item.customerName);
      row[4] = ConvertToString(item.productLength);
      row[5] = ConvertToString(item.productWidth);
      row[6] = ConvertToString(item.quantity);
      row[7] = ConvertToString(item.totalArea);
      row[8] = ConvertToString(item.productThickness);
      row[9] = ConvertToString(item.borehole);
      row[10] = ConvertToString(item.hole);
      row[11] = ConvertToString(item.noteQcName);
      row[12] = ConvertToString(item.errorTypeName);

      data.push(row);
    });

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman', size: 11 };

      row.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };

      row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(5).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(6).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(7).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(7).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(8).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(8).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(9).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(9).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(10).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(10).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(11).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(11).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(12).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(12).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(13).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(13).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    });

    /* sumary section */
    worksheet.addRow([]); //row rong~
    let sumaryHeaderRow = worksheet.addRow(['Tổng kết loại lỗi']);
    sumaryHeaderRow.font = { family: 4, size: 16, bold: true };
    sumaryHeaderRow.height = 25;
    sumaryHeaderRow.alignment = { vertical: 'middle', horizontal: 'center' };

    worksheet.mergeCells(`A${sumaryHeaderRow.number}:D${sumaryHeaderRow.number}`);
    sumaryHeaderRow.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

    let dataErrorListHeader = ['STT', 'Loại lỗi', 'Số tấm', 'Số m2'];
    let erorrListHeader = worksheet.addRow(dataErrorListHeader);
    erorrListHeader.font = { name: 'Time New Roman', size: 10, bold: true };

    dataErrorListHeader.forEach((item, index) => {
      erorrListHeader.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      erorrListHeader.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      erorrListHeader.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
    });
    headerRow2.height = 40;

    let sumaryData: Array<any> = [];

    let listErrorType = this.calculateSumErrorType(listQuanlityControlReport);
    //get summary
    let totalQuantity = 0;
    let totalArea = 0;
    listErrorType.forEach(e => {
      totalQuantity += e.sumQuantity;
      totalArea += e.sumArea;
    })
    totalQuantity = this.roundNumber(totalQuantity,2);
    totalArea = this.roundNumber(totalArea, 2);

    listErrorType.forEach((item, index) => {
      let row: Array<any> = [];
      row[0] = index + 1;
      row[1] = ConvertToString(item.errorType.categoryName);
      row[2] = ConvertToString(item.sumQuantity);
      row[3] = ConvertToString(item.sumArea);
      sumaryData.push(row);
    });

     let row: Array<any> = [];
     row[0] = '';
     row[1] = ConvertToString("Tổng lỗi");
     row[2] = ConvertToString(totalQuantity);
     row[3] = ConvertToString(totalArea);
     sumaryData.push(row);

    sumaryData.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman', size: 11 };

      row.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };

      row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    });

    /* fix with for column */
    worksheet.getColumn(1).width = 5;
    worksheet.getColumn(2).width = 15;
    worksheet.getColumn(3).width = 15;
    worksheet.getColumn(4).width = 25;
    worksheet.getColumn(5).width = 10;
    worksheet.getColumn(6).width = 10;
    worksheet.getColumn(7).width = 10;
    worksheet.getColumn(8).width = 10;
    worksheet.getColumn(9).width = 10;
    worksheet.getColumn(10).width = 15;
    worksheet.getColumn(11).width = 15;
    worksheet.getColumn(12).width = 25;
    worksheet.getColumn(13).width = 15;

    this.exportToExel(workBook, title);
  }

  exportReport(searchModel: searchFormModel) {
    let workBook = new Workbook();
    let title = `BÁO CÁO SAI HỎNG TỔ SẢN XUẤT`;
    let worksheet = workBook.addWorksheet(title);

    let src1 = 'data:image/png;base64,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'
    let src2 = 'data:image/jpeg;base64,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'

    let imgLogo1 = workBook.addImage({
      base64: src1,
      extension: 'jpeg',
    });

    let imgLogo2 = workBook.addImage({
      base64: src2,
      extension: 'png',
    });

    worksheet.addImage(imgLogo1, 'A1:C7');
    worksheet.addImage(imgLogo2, 'K1:N7');

    let companyName = 'CÔNG TY CP KÍNH KALA';
    let dataRow1 = [];
    dataRow1[4] = companyName;
    let row1 = worksheet.addRow(dataRow1);
    row1.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`D${row1.number}:I${row1.number}`);
    row1.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let address = 'Km 15+300,Quốc lộ 1A (cũ),';
    let dataRow2 = [];
    dataRow2[4] = address;
    let row2 = worksheet.addRow(dataRow2);
    row2.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`D${row2.number}:I${row2.number}`);
    row2.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let region = 'Liên Ninh, Thanh Trì, Hà Nội.';
    let dataRow3 = [];
    dataRow3[4] = region;
    let row3 = worksheet.addRow(dataRow3);
    row3.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`D${row3.number}:I${row3.number}`);
    row3.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let phone = 'Tel: 0243.689.0198   Fax: 0243.686.3184';
    let dataRow4 = [];
    dataRow4[4] = phone;
    let row4 = worksheet.addRow(dataRow4);
    row4.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`D${row4.number}:I${row4.number}`);
    row4.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    worksheet.addRow([]);
    worksheet.addRow([]);
    worksheet.addRow([]);
    worksheet.addRow([]);

    /* desc */
    let currentTime = new Date();
    let fromDate1 = `Từ ngày ${searchModel.fromDate == null ? '' : searchModel.fromDate.getDate()} tháng ${searchModel.fromDate == null ? '' : searchModel.fromDate.getMonth() + 1} năm ${searchModel.fromDate == null ? '' : searchModel.fromDate.getFullYear()}`;
    let toDate1 = `đến ${this.downloadReportForm.get('ToDate').value == null ? '' : this.downloadReportForm.get('ToDate').value.getDate()} tháng ${this.downloadReportForm.get('ToDate').value == null ? '' : this.downloadReportForm.get('ToDate').value.getMonth() + 1} năm ${this.downloadReportForm.get('ToDate').value == null ? '' : this.downloadReportForm.get('ToDate').value.getFullYear()}`;
    let dateDesc = `Liên Ninh, ${fromDate1} ${toDate1}`;
    let descRow = worksheet.addRow([dateDesc]);
    worksheet.mergeCells(`A${descRow.number}:N${descRow.number}`);
    descRow.alignment = { vertical: 'middle', horizontal: 'right' };
    // descRow.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

    let titleRow = worksheet.addRow([title]);
    titleRow.font = { family: 4, size: 16, bold: true };
    titleRow.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A${titleRow.number}:N${titleRow.number}`);
    // titleRow.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, right: { style: "thin" } };
    titleRow.height = 25;
    //date time
    let fromDate: Date = this.searchForm.get('FromDate').value;
    let toDate: Date = this.searchForm.get('ToDate').value;

    let fromDateDesc = '';
    let toDateDesc = '';

    if (fromDate) {
      fromDateDesc = `Từ ngày ${this.transformDate(fromDate)}`
    }

    if (toDate) {
      toDateDesc = `đến ${this.transformDate(toDate)}`
    }

    let dataDesc2 = `${fromDateDesc} ${toDateDesc}`;
    let descRow2 = worksheet.addRow([dataDesc2]);
    worksheet.mergeCells(`A${descRow2.number}:N${descRow2.number}`);
    descRow2.alignment = { vertical: 'middle', horizontal: 'center' };
    // descRow2.getCell(1).border = { left: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

    /* table reagion */
    worksheet.addRow([]);

    /* header */
    let dataHeaderRow1 = ['STT', 'Nội dung', 'Tổ cắt', '', 'Tổ Mài', '', 'Tổ Khoan', '', 'Tổ Tôi', '', 'Tổ Dán', '', 'Tổ Hộp', ''];
    let dataHeaderRow2 = ['', '', 'm2', '%', 'm2', '%', 'm2', '%', 'm2', '%', 'm2', '%', 'm2', '%'];

    // let dataHeaderRow1 = ['STT', 'Nội dung'];
    // let dataHeaderRow2 = ['', ''];

    // this.listTechniqueRequestReport.forEach(e => {
    //   dataHeaderRow1 = [...dataHeaderRow1, e.techniqueName, ''];
    //   dataHeaderRow2 = [...dataHeaderRow2, 'm2', '%'];
    // });

    let headerRow1 = worksheet.addRow(dataHeaderRow1);
    let headerRow2 = worksheet.addRow(dataHeaderRow2);
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
    headerRow2.font = { name: 'Time New Roman', size: 10, bold: true };

    /* merge header */
    worksheet.mergeCells(`A${13}:A${14}`);
    worksheet.mergeCells(`B${13}:B${14}`);

    worksheet.mergeCells(`C${13}:D${13}`);
    worksheet.mergeCells(`E${13}:F${13}`);
    worksheet.mergeCells(`G${13}:H${13}`);
    worksheet.mergeCells(`I${13}:J${13}`);
    worksheet.mergeCells(`K${13}:L${13}`);
    worksheet.mergeCells(`M${13}:N${13}`);

    dataHeaderRow1.forEach((item, index) => {
      headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow1.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };

    });
    headerRow1.height = 40;

    dataHeaderRow2.forEach((item, index) => {
      headerRow2.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow2.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
    });
    headerRow2.height = 40;

    // /* body of table */
    let data: Array<any> = [];

    //san luong
    let totalAreaData: Array<any> = [];
    let totalAreaErrorData: Array<any> = [];
    let totalAreaErrorByThickGlass: Array<any> = [];
    let totalAreaErrorByThinGlass: Array<any> = [];
    let totalAreaErrorByTechEquipments: Array<any> = [];
    let totalAreaErrorByManufacture: Array<any> = [];
    let totalAreaErrorByMaterials: Array<any> = [];

    //tổ cắt
    let toCat = this.listTechniqueRequestReport.find(e => e.techniqueName.toLowerCase().trim() === 'cắt');
    //tổ mài
    let toDan = this.listTechniqueRequestReport.find(e => e.techniqueName.toLowerCase().trim() === 'dán');
    //tổ khoan
    let toKhoan = this.listTechniqueRequestReport.find(e => e.techniqueName.toLowerCase().trim() === 'khoan');
    //tổ tôi
    let toToi = this.listTechniqueRequestReport.find(e => e.techniqueName.toLowerCase().trim() === 'tôi');
    //tổ dán
    let toMai = this.listTechniqueRequestReport.find(e => e.techniqueName.toLowerCase().trim() === 'mài');
    //tổ hộp
    let toHop = this.listTechniqueRequestReport.find(e => e.techniqueName.toLowerCase().trim() === 'hộp');

    totalAreaData[0] = 1;
    totalAreaData[1] = "Sản lượng";
    totalAreaErrorData[0] = 2;
    totalAreaErrorData[1] = "Sai hỏng";
    totalAreaErrorByThickGlass[0] = 3;
    totalAreaErrorByThickGlass[1] = "Sai hỏng kính dày";
    totalAreaErrorByThinGlass[0] = 4;
    totalAreaErrorByThinGlass[1] = "Sai hỏng kính mỏng";
    totalAreaErrorByTechEquipments[0] = 5;
    totalAreaErrorByTechEquipments[1] = "Lỗi TBCN";
    totalAreaErrorByManufacture[0] = 6;
    totalAreaErrorByManufacture[1] = "Lỗi tổ Sản xuất";
    totalAreaErrorByMaterials[0] = 7;
    totalAreaErrorByMaterials[1] = "Lỗi Vật tư";

    if (toCat) {
      totalAreaData[2] = toCat.totalArea;
      totalAreaData[3] = toCat.totalAreaByPercent;
      totalAreaErrorData[2] = toCat.totalAreaError;
      totalAreaErrorData[3] = toCat.totalAreaErrorByPercent;
      totalAreaErrorByThickGlass[2] = toCat.totalAreaErrorByThickGlass;
      totalAreaErrorByThickGlass[3] = toCat.totalAreaErrorByThickGlassByPercent;
      totalAreaErrorByThinGlass[2] = toCat.totalAreaErrorByThinGlass;
      totalAreaErrorByThinGlass[3] = toCat.totalAreaErrorByThinGlassByPercent;
      totalAreaErrorByTechEquipments[2] = toCat.totalAreaErrorByTechEquipments;
      totalAreaErrorByTechEquipments[3] = toCat.totalAreaErrorByTechEquipmentsByPercent;
      totalAreaErrorByManufacture[2] = toCat.totalAreaErrorByManufacture;
      totalAreaErrorByManufacture[3] = toCat.totalAreaErrorByManufactureByPercent;
      totalAreaErrorByMaterials[2] = toCat.totalAreaErrorByMaterials;
      totalAreaErrorByMaterials[3] = toCat.totalAreaErrorByMaterials;
    }
    if (toMai) {
      totalAreaData[4] = toMai.totalArea;
      totalAreaData[5] = toMai.totalAreaByPercent;
      totalAreaErrorData[4] = toMai.totalAreaError;
      totalAreaErrorData[5] = toMai.totalAreaErrorByPercent;
      totalAreaErrorByThickGlass[4] = toMai.totalAreaErrorByThickGlass;
      totalAreaErrorByThickGlass[5] = toMai.totalAreaErrorByThickGlassByPercent;
      totalAreaErrorByThinGlass[4] = toMai.totalAreaErrorByThinGlass;
      totalAreaErrorByThinGlass[5] = toMai.totalAreaErrorByThinGlassByPercent;
      totalAreaErrorByTechEquipments[4] = toMai.totalAreaErrorByTechEquipments;
      totalAreaErrorByTechEquipments[5] = toMai.totalAreaErrorByTechEquipmentsByPercent;
      totalAreaErrorByManufacture[4] = toMai.totalAreaErrorByManufacture;
      totalAreaErrorByManufacture[5] = toMai.totalAreaErrorByManufactureByPercent;
      totalAreaErrorByMaterials[4] = toMai.totalAreaErrorByMaterials;
      totalAreaErrorByMaterials[5] = toMai.totalAreaErrorByMaterials;
    }

    if (toKhoan) {
      totalAreaData[6] = toKhoan.totalArea;
      totalAreaData[7] = toKhoan.totalAreaByPercent;
      totalAreaErrorData[6] = toKhoan.totalAreaError;
      totalAreaErrorData[7] = toKhoan.totalAreaErrorByPercent;
      totalAreaErrorByThickGlass[6] = toKhoan.totalAreaErrorByThickGlass;
      totalAreaErrorByThickGlass[7] = toKhoan.totalAreaErrorByThickGlassByPercent;
      totalAreaErrorByThinGlass[6] = toKhoan.totalAreaErrorByThinGlass;
      totalAreaErrorByThinGlass[7] = toKhoan.totalAreaErrorByThinGlassByPercent;
      totalAreaErrorByTechEquipments[6] = toKhoan.totalAreaErrorByTechEquipments;
      totalAreaErrorByTechEquipments[7] = toKhoan.totalAreaErrorByTechEquipmentsByPercent;
      totalAreaErrorByManufacture[6] = toKhoan.totalAreaErrorByManufacture;
      totalAreaErrorByManufacture[7] = toKhoan.totalAreaErrorByManufactureByPercent;
      totalAreaErrorByMaterials[6] = toKhoan.totalAreaErrorByMaterials;
      totalAreaErrorByMaterials[7] = toKhoan.totalAreaErrorByMaterials;
    }

    if (toToi) {
      totalAreaData[8] = toToi.totalArea;
      totalAreaData[9] = toToi.totalAreaByPercent;
      totalAreaErrorData[8] = toToi.totalAreaError;
      totalAreaErrorData[9] = toToi.totalAreaErrorByPercent;
      totalAreaErrorByThickGlass[8] = toToi.totalAreaErrorByThickGlass;
      totalAreaErrorByThickGlass[9] = toToi.totalAreaErrorByThickGlassByPercent;
      totalAreaErrorByThinGlass[8] = toToi.totalAreaErrorByThinGlass;
      totalAreaErrorByThinGlass[9] = toToi.totalAreaErrorByThinGlassByPercent;
      totalAreaErrorByTechEquipments[8] = toToi.totalAreaErrorByTechEquipments;
      totalAreaErrorByTechEquipments[9] = toToi.totalAreaErrorByTechEquipmentsByPercent;
      totalAreaErrorByManufacture[8] = toToi.totalAreaErrorByManufacture;
      totalAreaErrorByManufacture[9] = toToi.totalAreaErrorByManufactureByPercent;
      totalAreaErrorByMaterials[8] = toToi.totalAreaErrorByMaterials;
      totalAreaErrorByMaterials[9] = toToi.totalAreaErrorByMaterials;
    }

    if (toDan) {
      totalAreaData[10] = toDan.totalArea;
      totalAreaData[11] = toDan.totalAreaByPercent;
      totalAreaErrorData[10] = toDan.totalAreaError;
      totalAreaErrorData[11] = toDan.totalAreaErrorByPercent;
      totalAreaErrorByThickGlass[10] = toDan.totalAreaErrorByThickGlass;
      totalAreaErrorByThickGlass[11] = toDan.totalAreaErrorByThickGlassByPercent;
      totalAreaErrorByThinGlass[10] = toDan.totalAreaErrorByThinGlass;
      totalAreaErrorByThinGlass[11] = toDan.totalAreaErrorByThinGlassByPercent;
      totalAreaErrorByTechEquipments[10] = toDan.totalAreaErrorByTechEquipments;
      totalAreaErrorByTechEquipments[11] = toDan.totalAreaErrorByTechEquipmentsByPercent;
      totalAreaErrorByManufacture[10] = toDan.totalAreaErrorByManufacture;
      totalAreaErrorByManufacture[11] = toDan.totalAreaErrorByManufactureByPercent;
      totalAreaErrorByMaterials[10] = toDan.totalAreaErrorByMaterials;
      totalAreaErrorByMaterials[11] = toDan.totalAreaErrorByMaterials;
    }

    if (toHop) {
      totalAreaData[12] = toHop.totalArea;
      totalAreaData[13] = toHop.totalAreaByPercent;
      totalAreaErrorData[12] = toHop.totalAreaError;
      totalAreaErrorData[13] = toHop.totalAreaErrorByPercent;
      totalAreaErrorByThickGlass[12] = toHop.totalAreaErrorByThickGlass;
      totalAreaErrorByThickGlass[13] = toHop.totalAreaErrorByThickGlassByPercent;
      totalAreaErrorByThinGlass[12] = toHop.totalAreaErrorByThinGlass;
      totalAreaErrorByThinGlass[13] = toHop.totalAreaErrorByThinGlassByPercent;
      totalAreaErrorByTechEquipments[12] = toHop.totalAreaErrorByTechEquipments;
      totalAreaErrorByTechEquipments[13] = toHop.totalAreaErrorByTechEquipmentsByPercent;
      totalAreaErrorByManufacture[12] = toHop.totalAreaErrorByManufacture;
      totalAreaErrorByManufacture[13] = toHop.totalAreaErrorByManufactureByPercent;
      totalAreaErrorByMaterials[12] = toHop.totalAreaErrorByMaterials;
      totalAreaErrorByMaterials[13] = toHop.totalAreaErrorByMaterials;
    }

    let totalAreaDataRow = worksheet.addRow(totalAreaData);
    let totalAreaErrorDataRow = worksheet.addRow(totalAreaErrorData);
    let totalAreaErrorByThickGlassRow = worksheet.addRow(totalAreaErrorByThickGlass);
    let totalAreaErrorByThinGlassRow = worksheet.addRow(totalAreaErrorByThinGlass);
    let totalAreaErrorByTechEquipmentsRow = worksheet.addRow(totalAreaErrorByTechEquipments);
    let totalAreaErrorByManufactureRow = worksheet.addRow(totalAreaErrorByManufacture);
    let totalAreaErrorByMaterialsRow = worksheet.addRow(totalAreaErrorByMaterials);


    totalAreaDataRow.eachCell(e => {
      e.border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });
    totalAreaErrorDataRow.eachCell(e => {
      e.border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });
    totalAreaErrorByThickGlassRow.eachCell(e => {
      e.border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });
    totalAreaErrorByThinGlassRow.eachCell(e => {
      e.border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });
    totalAreaErrorByTechEquipmentsRow.eachCell(e => {
      e.border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });
    totalAreaErrorByManufactureRow.eachCell(e => {
      e.border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });
    totalAreaErrorByMaterialsRow.eachCell(e => {
      e.border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });

    /* fix with for column */
    worksheet.getColumn(1).width = 5;
    // worksheet.getColumn(1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
    worksheet.getColumn(2).width = 30;
    //  worksheet.getColumn(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
    worksheet.getColumn(3).width = 10;
    //  worksheet.getColumn(3).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    worksheet.getColumn(4).width = 10;
    //worksheet.getColumn(4).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    worksheet.getColumn(5).width = 10;
    // worksheet.getColumn(5).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    worksheet.getColumn(6).width = 10;
    worksheet.getColumn(7).width = 10;
    worksheet.getColumn(8).width = 10;
    worksheet.getColumn(9).width = 10;
    worksheet.getColumn(10).width = 10;
    worksheet.getColumn(11).width = 10;
    worksheet.getColumn(12).width = 10;
    worksheet.getColumn(13).width = 10;
    worksheet.getColumn(14).width = 10;

    this.exportToExel(workBook, title);
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  async search() {
    let searchModel = new searchFormModel();
    let techiqueRequest: techniqueRequest = this.searchForm.get('TechniqueRequest').value;
    let listThicknessFilter: Array<thicknessFilter> = this.searchForm.get('Thickness').value;
    let fromDate: Date = this.searchForm.get('FromDate').value;
    let toDate: Date = this.searchForm.get('ToDate').value;
    let listThicknessFilterId = listThicknessFilter.map(e => e.id);
    searchModel.listTechniqueRequestId.push(techiqueRequest.techniqueRequestId);
    searchModel.listThicknessOptionId = listThicknessFilterId;

    if (fromDate) {
      fromDate.setHours(0);
      fromDate.setMinutes(0);
      fromDate.setSeconds(0);
    }
    searchModel.fromDate = fromDate ? convertToUTCTime(fromDate) : null;

    if (toDate) {
      toDate.setHours(23);
      toDate.setMinutes(59);
      toDate.setSeconds(59);
    }
    searchModel.toDate = toDate? convertToUTCTime(toDate) : null;

    this.loading = true;
    let result: any = await this.manufactureService.searchQuanlityControlReport(searchModel.listTechniqueRequestId, searchModel.listThicknessOptionId, searchModel.fromDate, searchModel.toDate);
    this.loading = false;
    if (result.statusCode == 200) {
      this.listQuanlityControlReport = result.listQuanlityControlReport; //báo cáo theo từng tổ
      this.listTechniqueRequestReport = result.listTechniqueRequestReport; //báo cáo tổng
      this.handleNoteReport();
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  handleNoteReport() {
    this.listQuanlityControlReport.forEach(qcreport => {
      let qcnote = this.listQualityControlNote.find(cate => cate.categoryId == qcreport.noteQc);
      if (qcnote) {
        qcreport.noteQcName = qcnote.categoryName;
        qcreport.selectedQcNote = qcnote;
      }
      let errorNote = this.listErrorType.find(cate => cate.categoryId == qcreport.errorType);
      if (errorNote) {
        qcreport.errorTypeName = errorNote.categoryName;
        qcreport.selectedErrorType = errorNote;
      }
    });
  }

  transformDate(date: Date) {
    return formatDate(date, 'dd/MM/yyyy', 'EN');
  }

  transformNumber(input: any) {
    return formatNumber(input, 'EN',);
  }

  calculateSumErrorType(listQuanlityControlReport: Array<quanlityControlReport>): Array<sumaryExcelModel> {
    let result: Array<sumaryExcelModel> = new Array<sumaryExcelModel>();
    this.listErrorType.forEach(e => {
      let errorType = new sumaryExcelModel();
      errorType.errorType = e;
      errorType.sumArea = 0;
      errorType.sumQuantity = 0;
      listQuanlityControlReport.forEach(qcreport => {
        if (qcreport.selectedErrorType == e) {
          errorType.sumArea += qcreport.totalArea;
          errorType.sumQuantity += qcreport.quantity;
        }
      });

      //round number
      errorType.sumArea = this.roundNumber(errorType.sumArea, 2);
      errorType.sumQuantity = this.roundNumber(errorType.sumQuantity, 2);
      result = [...result, errorType];
    });
    return result;
  }

  getQCNoteDesc(selectedNote: qualityControlNote): string {
    if (!selectedNote) return '';
    return selectedNote.categoryName;
  }

  getErrorTypeDesc(selectedErrorType: errorType): string {
    if (!selectedErrorType) return '';
    return selectedErrorType.categoryName;
  }

  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch (unit) {
      case 0: {
        result = result;
        break;
      }
      case 1: {
        result = Math.round(number * 10) / 10;
        break;
      }
      case 2: {
        result = Math.round(number * 100) / 100;
        break;
      }
      case 3: {
        result = Math.round(number * 1000) / 1000;
        break;
      }
      case 4: {
        result = Math.round(number * 10000) / 10000;
        break;
      }
      default: {
        result = result;
        break;
      }
    }
    return result;
  }

  dateFieldFormat: string = 'DD/MM/YYYY';
  customSort(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];

      /**Customize sort date */
      if (event.field == 'endDate') {
        const date1 = moment(value1, this.dateFieldFormat);
        const date2 = moment(value2, this.dateFieldFormat);

        let result: number = -1;
        if (moment(date2).isBefore(date1, 'day')) { result = 1; }

        return result * event.order;
      }

      /**Customize sort date */
      if (event.field == 'startDate') {
        const date1 = moment(value1, this.dateFieldFormat);
        const date2 = moment(value2, this.dateFieldFormat);

        let result: number = -1;
        if (moment(date2).isBefore(date1, 'day')) { result = 1; }

        return result * event.order;
      }
      /**End */

      let result = null;

      if (value1 == null && value2 != null)
        result = -1;
      else if (value1 != null && value2 == null)
        result = 1;
      else if (value1 == null && value2 == null)
        result = 0;
      else if (typeof value1 === 'string' && typeof value2 === 'string')
        result = value1.localeCompare(value2);
      else
        result = (value1 < value2) ? -1 : (value1 > value2) ? 1 : 0;

      return (event.order * result);
    });
  }
}

function ConvertToString(str: any) {
  if (str === null || str === undefined) return '';
  return String(str);
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

