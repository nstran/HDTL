import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Table } from 'primeng/table';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { MessageService, ConfirmationService } from 'primeng/api';
import { EncrDecrService } from '../../../../shared/services/encrDecr.service';
import { SalaryService } from '../../../services/salary.service';
import { CommonService } from '../../../../shared/services/common.service';
import { FormatDateService } from '../../../../shared/services/formatDate.services';
import { KyLuong } from '../../../models/ky-luong.model';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";

//model dùng để build template excel
class columnModel {
  column1: string;
  column2: string;
}

@Component({
  selector: 'app-ky-luong-list',
  templateUrl: './ky-luong-list.component.html',
  styleUrls: ['./ky-luong-list.component.css']
})
export class KyLuongListComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;
  nowDate = new Date();

  actionAdd: boolean = false;
  actionDelete: boolean = false;

  filterGlobal: string = null;
  isShowFilterRight: boolean = false;
  leftColNumber: number = 12;
  rightColNumber: number = 4;

  listData: Array<any> = [];
  tenKyLuong: string = null;
  listStatus: Array<any> = [];
  listSelectedStatus: Array<any> = [];

  @ViewChild('myTable') myTable: Table;
  colsList: any;

  showCreate: boolean = false;

  createForm: FormGroup;
  tenKyLuongControl: FormControl;
  soNgayLamViecControl: FormControl;
  tuNgayControl: FormControl;
  denNgayControl: FormControl;

  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private encrDecrService: EncrDecrService,
    private salaryService: SalaryService,
    private commonService: CommonService,
    private formatDateService: FormatDateService,
  ) { }

  async ngOnInit() {
    this.initTable();
    this.initForm();

    await this._getPermission();

    this.search();
  }

  initTable() {
    this.colsList = [
      { field: 'index', header: '#', textAlign: 'center', display: 'table-cell' },
      { field: 'tenKyLuong', header: 'Tên kỳ lương', textAlign: 'left', display: 'table-cell' },
      { field: 'tuNgay', header: 'Ngày bắt đầu', textAlign: 'center', display: 'table-cell' },
      { field: 'denNgay', header: 'Ngày kết thúc', textAlign: 'center', display: 'table-cell' },
      { field: 'soNgayLamViec', header: 'Số ngày làm việc', textAlign: 'center', display: 'table-cell' },
      { field: 'tenTrangThai', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
      { field: 'createdDate', header: 'Ngày tạo', textAlign: 'center', display: 'table-cell' },
      { field: 'actions', header: 'Thao tác', textAlign: 'center', display: 'table-cell' },
    ];
  }

  initForm() {
    this.tenKyLuongControl = new FormControl(null, [Validators.required, Validators.maxLength(500)]);
    this.soNgayLamViecControl = new FormControl(0, [Validators.required]);
    this.tuNgayControl = new FormControl(null, [Validators.required]);
    this.denNgayControl = new FormControl(null, [Validators.required]);

    this.createForm = new FormGroup({
      tenKyLuongControl: this.tenKyLuongControl,
      soNgayLamViecControl: this.soNgayLamViecControl,
      tuNgayControl: this.tuNgayControl,
      denNgayControl: this.denNgayControl
    });
  }

  async _getPermission() {
    let resource = "salary/salary/ky-luong-list/";
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

    if (permission.listCurrentActionResource.indexOf("delete") != -1) {
      this.actionDelete = true;
    }
  }

  async search() {
    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.getListKyLuong(this.tenKyLuong?.trim(), this.listSelectedStatus.map(x => x.value));
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.listStatus = result.listStatus;
    this.listData = result.listData;
  }

  openCreate() {
    this.createForm.reset();
    this.soNgayLamViecControl.setValue(0);
    this.showCreate = true;
  }

  closeCreate() {
    this.showCreate = false;
  }

  changeSoNgay() {
    if (this.soNgayLamViecControl.value == null || this.soNgayLamViecControl.value == '')
      this.soNgayLamViecControl.setValue('0');
  }

  async save() {
    if (!this.createForm.valid) {
      Object.keys(this.createForm.controls).forEach(key => {
        if (!this.createForm.controls[key].valid) {
          this.createForm.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let kyLuong = new KyLuong();
    kyLuong.tenKyLuong = this.tenKyLuongControl.value.trim();
    kyLuong.soNgayLamViec = this.commonService.convertStringToNumber(this.soNgayLamViecControl.value.toString());
    kyLuong.tuNgay = this.formatDateService.convertToUTCTime(this.tuNgayControl.value);
    kyLuong.denNgay = this.formatDateService.convertToUTCTime(this.denNgayControl.value);

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.createOrUpdateKyLuong(kyLuong);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showMessage("error", result.messageCode);
      return;
    }

    await this.search();
    this.showMessage("success", result.messageCode);
    this.showCreate = false;
  }

  async delete(data: KyLuong) {
    this.confirmationService.confirm({
      message: `Dữ liệu không thể hoàn tác, bạn chắc chắn muốn xóa?`,
      accept: async () => {
        this.loading = true;
        this.awaitResult = true;
        let result: any = await this.salaryService.deleteKyLuong(data.kyLuongId);

        if (result.statusCode != 200) {
          this.loading = false;
          this.awaitResult = false;
          this.showMessage("error", result.messageCode);
          return;
        }

        await this.search();
        this.showMessage("success", result.messageCode);
      },
    });
  }

  goToDetail(data: KyLuong) {
    this.router.navigate(['/salary/ky-luong-detail', { kyLuongId: this.encrDecrService.set(data.kyLuongId) }]);
  }

  refreshFilter() {
    this.filterGlobal = null;
    this.myTable.reset();
    this.tenKyLuong = null;
    this.listSelectedStatus = [];
    this.search();
  }

  showFilter() {
    this.isShowFilterRight = !this.isShowFilterRight;

    if (this.isShowFilterRight) this.leftColNumber = 8;
    else this.leftColNumber = 12;
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }

  getDataExportExcel(id) {

  }

  excel5() {
    let title = `Allowances`;
    let workBook = new Workbook();
    let worksheet = workBook.addWorksheet(title);

    //Cột hàng 1
    let listExcelColumns1 = this.getListExcelColumns(1);
    let dataHeaderRow1: Array<string> = listExcelColumns1.map(e => e.column1);
    let headerRow1 = worksheet.addRow(dataHeaderRow1);
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
    //merge header column
    worksheet.mergeCells(`J${1}:O${1}`);
    worksheet.mergeCells(`P${1}:U${1}`);
    worksheet.mergeCells(`AF${1}:AK${1}`);

    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
    dataHeaderRow1.forEach((item, index) => {
      if (index + 1 < dataHeaderRow1.length + 1) {
        headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow1.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      }

      // chỉnh độ rộng cột theo độ dài ký tự
      if (item.length >= 30) {
        worksheet.getColumn(index + 1).width = 22;
      } else if (item.length <= 30) {
        worksheet.getColumn(index + 1).width = 15;
      }
    });
    headerRow1.height = 27;

    //Cột hàng 2
    let listExcelColumns2 = this.getListExcelColumns(2);
    let dataHeaderRow2: Array<string> = listExcelColumns2.map(e => e.column1);
    let headerRow2 = worksheet.addRow(dataHeaderRow2);
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };

    headerRow2.font = { name: 'Time New Roman', size: 10, bold: true };
    dataHeaderRow2.forEach((item, index) => {
      //merge header column row1 vs row2
      if (item == "") {
        // merge by start row, start column, end row, end column
        worksheet.mergeCells(1, index + 1, 2, index + 1);
      }

      // chỉnh độ rộng cột theo độ dài ký tự
      if (item.length >= 30) {
        worksheet.getColumn(index + 1).width = 22;
      } else if (item.length >= 60) {
        worksheet.getColumn(index + 1).width = 27;
      } else if (item.length >= 80) {
        worksheet.getColumn(index + 1).width = 30;
      }

      if (index + 1 < dataHeaderRow2.length + 1) {
        headerRow2.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow2.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      }
    });

    let data: Array<any> = [];

    // let dataExportExcel: Array<TaiSanModel> = this.getDataExportExcel(this.listAsset);

    let dataExportExcel = [
      {
        "0": 0, "1": 1, "2": 2, "3": 3, "4": 4, "5": 5, "6": 6, "7": 7, "8": 8, "9": 9, "10": 10,
        "11": 11, "12": 12, "13": 13, "14": 14, "15": 15, "16": 16, "17": 17, "18": 18, "19": 19, "20": 20,
        "21": 21, "22": 22, "23": 23, "24": 24, "25": 25, "26": 26, "27": 27, "28": 28, "29": 29, "30": 30,
        "31": 31, "32": 32, "33": 33, "34": 34, "35": 35, "36": 36, "37": 37, "38": 38, "39": 39
      },

      {
        "0": 0, "1": 1, "2": 2, "3": 3, "4": 4, "5": 5, "6": 6, "7": 7, "8": 8, "9": 9, "10": 10,
        "11": 11, "12": 12, "13": 13, "14": 14, "15": 15, "16": 16, "17": 17, "18": 18, "19": 19, "20": 20,
        "21": 21, "22": 22, "23": 23, "24": 24, "25": 25, "26": 26, "27": 27, "28": 28, "29": 29, "30": 30,
        "31": 31, "32": 32, "33": 33, "34": 34, "35": 35, "36": 36, "37": 37, "38": 38, "39": 39
      },

      {
        "0": 0, "1": 1, "2": 2, "3": 3, "4": 4, "5": 5, "6": 6, "7": 7, "8": 8, "9": 9, "10": 10,
        "11": 11, "12": 12, "13": 13, "14": 14, "15": 15, "16": 16, "17": 17, "18": 18, "19": 19, "20": 20,
        "21": 21, "22": 22, "23": 23, "24": 24, "25": 25, "26": 26, "27": 27, "28": 28, "29": 29, "30": 30,
        "31": 31, "32": 32, "33": 33, "34": 34, "35": 35, "36": 36, "37": 37, "38": 38, "39": 39
      },
    ];


    dataExportExcel?.forEach((item, index) => {
      let row: Array<any> = [];
      listExcelColumns1.forEach((col, colIndex) => {
        row[colIndex] = item[col.column2];
      })
      data.push(row);
    });


    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      //Chỉnh bordor và thuộc tính của từng cột của dòng
      listExcelColumns1.forEach((col, colIndex) => {
        row.font = { name: 'Times New Roman', size: 11 };
        row.getCell(colIndex + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(colIndex + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      })
    });

    this.exportToExel(workBook, title);
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  getListExcelColumns(row): Array<columnModel> {
    let listColumns: Array<columnModel> = [];
    if (row == 1) {
      let columGroup_0 = new columnModel();
      columGroup_0.column1 = "No.";
      columGroup_0.column2 = "0";

      let columGroup_1 = new columnModel();
      columGroup_1.column1 = "HR Filing Code";
      columGroup_1.column2 = "1";

      let columGroup_2 = new columnModel();
      columGroup_2.column1 = "Vietnamesename";
      columGroup_2.column2 = "2";

      let columGroup_3 = new columnModel();
      columGroup_3.column1 = "Team";
      columGroup_3.column2 = "3";

      let columGroup_4 = new columnModel();
      columGroup_4.column1 = "DEPT";
      columGroup_4.column2 = "4";

      let columGroup_5 = new columnModel();
      columGroup_5.column1 = "TITLE";
      columGroup_5.column2 = "5";

      let columGroup_6 = new columnModel();
      columGroup_6.column1 = "ELIGIBLE FOR LEADER/MENTOR BONUS OR NOT?";
      columGroup_6.column2 = "6";

      let columGroup_7 = new columnModel();
      columGroup_7.column1 = "Contract  Type";
      columGroup_7.column2 = "7";

      let columGroup_8 = new columnModel();
      columGroup_8.column1 = "Thưởng tuân thủ nội quy lao động Theo HĐ";
      columGroup_8.column2 = "8";

      let columGroup_9 = new columnModel();
      columGroup_9.column1 = "LEADER/ TEAM KPI OF … 2021";
      columGroup_9.column2 = "9";

      let columGroup_10 = new columnModel();
      columGroup_10.column1 = "";
      columGroup_10.column2 = "10";

      let columGroup_11 = new columnModel();
      columGroup_11.column1 = "";
      columGroup_11.column2 = "11";

      let columGroup_12 = new columnModel();
      columGroup_12.column1 = "";
      columGroup_12.column2 = "12";

      let columGroup_13 = new columnModel();
      columGroup_13.column1 = "";
      columGroup_13.column2 = "13";

      let columGroup_14 = new columnModel();
      columGroup_14.column1 = "";
      columGroup_14.column2 = "14";

      let columGroup_15 = new columnModel();
      columGroup_15.column1 = "LEADER/ TEAM KPI OF MAR 2022";
      columGroup_15.column2 = "15";

      let columGroup_16 = new columnModel();
      columGroup_16.column1 = "";
      columGroup_16.column2 = "16";

      let columGroup_17 = new columnModel();
      columGroup_17.column1 = "";
      columGroup_17.column2 = "17";

      let columGroup_18 = new columnModel();
      columGroup_18.column1 = "";
      columGroup_18.column2 = "18";

      let columGroup_19 = new columnModel();
      columGroup_19.column1 = "";
      columGroup_19.column2 = "19";

      let columGroup_20 = new columnModel();
      columGroup_20.column1 = "";
      columGroup_20.column2 = "20";

      let columGroup_21 = new columnModel();
      columGroup_21.column1 = "Actual working days";
      columGroup_21.column2 = "21";

      let columGroup_22 = new columnModel();
      columGroup_22.column1 = "Actual working day with old salary/ Probation salary";
      columGroup_22.column2 = "22";

      let columGroup_23 = new columnModel();
      columGroup_23.column1 = "AL";
      columGroup_23.column2 = "23";

      let columGroup_24 = new columnModel();
      columGroup_24.column1 = "Holiday";
      columGroup_24.column2 = "24";

      let columGroup_25 = new columnModel();
      columGroup_25.column1 = "Other paid leave with full paid";
      columGroup_25.column2 = "25";

      let columGroup_26 = new columnModel();
      columGroup_26.column1 = "Unpaid Leave with reason";
      columGroup_26.column2 = "26";

      let columGroup_27 = new columnModel();
      columGroup_27.column1 = "Leave without approval_Unpaid";
      columGroup_27.column2 = "27";

      let columGroup_28 = new columnModel();
      columGroup_28.column1 = "Leave early and come late - Days w/o pay";
      columGroup_28.column2 = "28";

      let columGroup_29 = new columnModel();
      columGroup_29.column1 = "Days w/o attenance award";
      columGroup_29.column2 = "29";

      let columGroup_30 = new columnModel();
      columGroup_30.column1 = "Total payable day for this month, including Holiday (Mon-Fri)";
      columGroup_30.column2 = "30";

      let columGroup_31 = new columnModel();
      columGroup_31.column1 = "";
      columGroup_31.column2 = "31";

      let columGroup_32 = new columnModel();
      columGroup_32.column1 = "";
      columGroup_32.column2 = "32";

      let columGroup_33 = new columnModel();
      columGroup_33.column1 = "";
      columGroup_33.column2 = "33";

      let columGroup_34 = new columnModel();
      columGroup_34.column1 = "";
      columGroup_34.column2 = "34";

      let columGroup_35 = new columnModel();
      columGroup_35.column1 = "";
      columGroup_35.column2 = "35";

      let columGroup_36 = new columnModel();
      columGroup_36.column1 = "";
      columGroup_36.column2 = "36";

      let columGroup_37 = new columnModel();
      columGroup_37.column1 = "MONTHLY KPI BONUS (R5, R4 or Top 10% over 100%)";
      columGroup_37.column2 = "37";

      let columGroup_38 = new columnModel();
      columGroup_38.column1 = "LEADER BONUS";
      columGroup_38.column2 = "38";

      let columGroup_39 = new columnModel();
      columGroup_39.column1 = "GHI CHÚ";
      columGroup_39.column2 = "39";

      listColumns = [
        columGroup_0, columGroup_1, columGroup_2, columGroup_3, columGroup_4, columGroup_5, columGroup_6, columGroup_7, columGroup_8, columGroup_9, columGroup_10,
        columGroup_11, columGroup_12, columGroup_13, columGroup_14, columGroup_15, columGroup_16, columGroup_17, columGroup_18, columGroup_19, columGroup_20,
        columGroup_21, columGroup_22, columGroup_23, columGroup_24, columGroup_25, columGroup_26, columGroup_27, columGroup_28, columGroup_29, columGroup_30,
        columGroup_31, columGroup_32, columGroup_33, columGroup_34, columGroup_35, columGroup_36, columGroup_37, columGroup_38, columGroup_39,
      ];
    } else {
      let columGroup_0 = new columnModel();
      columGroup_0.column1 = "";

      let columGroup_1 = new columnModel();
      columGroup_1.column1 = "";

      let columGroup_2 = new columnModel();
      columGroup_2.column1 = "";

      let columGroup_3 = new columnModel();
      columGroup_3.column1 = "";

      let columGroup_4 = new columnModel();
      columGroup_4.column1 = "";

      let columGroup_5 = new columnModel();
      columGroup_5.column1 = "";

      let columGroup_6 = new columnModel();
      columGroup_6.column1 = "";

      let columGroup_7 = new columnModel();
      columGroup_7.column1 = "";

      let columGroup_8 = new columnModel();
      columGroup_8.column1 = "";

      let columGroup_9 = new columnModel();
      columGroup_9.column1 = "Personal Ranking";

      let columGroup_10 = new columnModel();
      columGroup_10.column1 = "Rank";

      let columGroup_11 = new columnModel();
      columGroup_11.column1 = "Trợ cấp 1.5 triệu VNĐ/ tháng (ĐT 200k, Đi lại 300k, Nhà ở 1.000k)";

      let columGroup_12 = new columnModel();
      columGroup_12.column1 = "Team Ranking";

      let columGroup_13 = new columnModel();
      columGroup_13.column1 = "Rank";

      let columGroup_14 = new columnModel();
      columGroup_14.column1 = "Leader Bonus?";

      let columGroup_15 = new columnModel();
      columGroup_15.column1 = "Final Ranking";

      let columGroup_16 = new columnModel();
      columGroup_16.column1 = "Rank";

      let columGroup_17 = new columnModel();
      columGroup_17.column1 = "Thưởng trợ cấp nếu đạt KPI, bottom 10% no allowance";

      let columGroup_18 = new columnModel();
      columGroup_18.column1 = "Team Ranking";

      let columGroup_19 = new columnModel();
      columGroup_19.column1 = "Rank";

      let columGroup_20 = new columnModel();
      columGroup_20.column1 = "Leader or Mentor Bonus?";

      let columGroup_21 = new columnModel();
      columGroup_21.column1 = "";

      let columGroup_22 = new columnModel();
      columGroup_22.column1 = "";

      let columGroup_23 = new columnModel();
      columGroup_23.column1 = "";

      let columGroup_24 = new columnModel();
      columGroup_24.column1 = "";

      let columGroup_25 = new columnModel();
      columGroup_25.column1 = "";

      let columGroup_26 = new columnModel();
      columGroup_26.column1 = "";

      let columGroup_27 = new columnModel();
      columGroup_27.column1 = "";

      let columGroup_28 = new columnModel();
      columGroup_28.column1 = "";

      let columGroup_29 = new columnModel();
      columGroup_29.column1 = "";

      let columGroup_30 = new columnModel();
      columGroup_30.column1 = "";

      let columGroup_31 = new columnModel();
      columGroup_31.column1 = "Bus/ Travelling support (Only applied from effective date of labor contract, based on actual working day)_Trả cho ngày làm việc thực tế từ thử việc";

      let columGroup_32 = new columnModel();
      columGroup_32.column1 = "Cell phone allowance (Only applied from effective date of labor contract, based on actual working day)_Trả cho ngày làm việc thực tế từ thử việc";

      let columGroup_33 = new columnModel();
      columGroup_33.column1 = "Trợ cấp chuyên cần ngày công/ Workingday attendance allowance";

      let columGroup_34 = new columnModel();
      columGroup_34.column1 = "Lunch allowance _Tính ngày làm việc thực tế";

      let columGroup_35 = new columnModel();
      columGroup_35.column1 = "Trợ cấp chuyên cần đi muộn  về sớm/ Early _Late attendance allowance";

      let columGroup_36 = new columnModel();
      columGroup_36.column1 = "TOTAL GROSS ALLOWANCE (Cell phone, Housing allowance, lunch, attendance reward)";

      let columGroup_37 = new columnModel();
      columGroup_37.column1 = "";

      let columGroup_38 = new columnModel();
      columGroup_38.column1 = "";

      let columGroup_39 = new columnModel();
      columGroup_39.column1 = "";

      listColumns = [
        columGroup_0, columGroup_1, columGroup_2, columGroup_3, columGroup_4, columGroup_5, columGroup_6, columGroup_7, columGroup_8, columGroup_9, columGroup_10,
        columGroup_11, columGroup_12, columGroup_13, columGroup_14, columGroup_15, columGroup_16, columGroup_17, columGroup_18, columGroup_19, columGroup_20,
        columGroup_21, columGroup_22, columGroup_23, columGroup_24, columGroup_25, columGroup_26, columGroup_27, columGroup_28, columGroup_29, columGroup_30,
        columGroup_31, columGroup_32, columGroup_33, columGroup_34, columGroup_35, columGroup_36, columGroup_37, columGroup_38, columGroup_39,
      ];
    }
    return listColumns;
  }

}
