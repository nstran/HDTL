import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { EmployeeService } from "../../../services/employee.service";
import { GetPermission } from '../../../../shared/permission/get-permission';
import { Row, Workbook, Worksheet } from 'exceljs';
import { saveAs } from "file-saver";
import { ConfirmationService, MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { EncrDecrService } from '../../../../shared/services/encrDecr.service';
import 'moment/locale/pt-br';

@Component({
  selector: 'app-danh-sach-ot',
  templateUrl: './danh-sach-ot.component.html',
  styleUrls: ['./danh-sach-ot.component.css']
})
export class DanhSachOtComponent implements OnInit {
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  listKeHoachOT: Array<any> = [];
  isManager: boolean = null;
  employeeId: string = null;
  messages: any;
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  loading: boolean = false;
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  currentYear: number = (new Date()).getFullYear();
  leftColNumber: number = 12;
  rightColNumber: number = 0;
  searchValue: Array<any> = null;
  abc: Array<any> = [];
  trangThai = []
  filterGlobal = ''
  nowDate: Date = new Date();
  colsList: any;
  selectedColumns: any[];
  mod = '';

  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;

  companyConfig: any = null; //thông tin công ty
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  @ViewChild('myTable') myTable: Table;

  actionDelete: boolean = true;
  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private router: Router,
    private messageService: MessageService,
    public dialogService: DialogService,
    private employeeService: EmployeeService,
    private confirmationService: ConfirmationService,
    private encrDecrService: EncrDecrService,
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    let resource0 = "hrm/employee/kehoach-ot-detail/";
    let permission0: any = await this.getPermission.getPermission(resource0);
    if (permission0.status == false) {
      this.actionDelete = false;
    }
    else {
      let listCurrentActionResource = permission0.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
    }

    let resource = "hrm/employee/danh-sach-de-xuat-ot/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("view") == -1) {
        let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
        this.showMessage(mgs);
      }
      this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
      this.initTable();
    }
    await this.getData();
    this.employeeId = JSON.parse(localStorage.getItem('auth')).EmployeeId;
    this.initTable();
  }

  async getData() {
    this.loading = true;
    let listSearchTrangThai = null;
    if (this.searchValue != null) {
      if (this.searchValue.length > 0) {
        listSearchTrangThai = this.searchValue ? this.searchValue.map(x => x.value) : null;
      }
    }

    let result: any = await this.employeeService.getMasterSearchKeHoachOtAsync(listSearchTrangThai);
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
    this.loading = false;

    this.companyConfig = result.companyConfig;
    this.trangThai = result.listTrangThaiKeHoach;
    this.listKeHoachOT = result.listKeHoachOt;
    this.setIndex();
  }

  initTable() {
    this.colsList = [
      { field: 'tenKeHoach', header: 'Tên kế hoạch', textAlign: 'left', display: 'table-cell' },
      { field: 'loaiOTName', header: 'Loại OT', textAlign: 'left', display: 'table-cell' },
      { field: 'thoiGian', header: 'Thời gian', textAlign: 'left', display: 'table-cell' },
      { field: 'trangThai', header: 'Trạng thái', textAlign: 'left', display: 'table-cell' },
    ];

    this.selectedColumns = this.colsList;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  setIndex() {
    this.listKeHoachOT ?.forEach((item, index) => {
      item.index = index + 1;
      item.gioBatDau = item.gioBatDau.slice(0, item.gioBatDau.lastIndexOf(':'));
      item.gioKetThuc = item.gioKetThuc.slice(0, item.gioKetThuc.lastIndexOf(':'));
    });
  }

  async refreshFilter() {
    this.myTable.reset();
    this.filterGlobal = '';

    this.loading = true;
    this.searchValue = null;
    await this.getData();
    this.loading = false;
  }

  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 9;
        this.rightColNumber = 3;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo ?.systemValueString;
  }

  exportExcel() {
    let title = `Danh sách kế hoạch OT`;

    let workBook = new Workbook();
    let worksheet = workBook.addWorksheet(title);

    //LOGO - infor section
    let imgBase64 = this.getBase64Logo();
    var imgLogo = workBook.addImage({
      base64: imgBase64,
      extension: 'png',
    });
    worksheet.addImage(imgLogo, {
      tl: { col: 0.5, row: 0.5 },
      ext: { width: 140, height: 95 }
    });

    let dataInforRow_1 = `${this.companyConfig.companyName}`
    let dataInforRow_2 = `Địa chỉ: ${this.companyConfig.companyAddress}`;
    let dataInforRow_3 = `Điện thoại: ${this.companyConfig.phone}`;
    let dataInforRow_4 = `Email: ${this.companyConfig.email}`;
    let dataInforRow_5 = `Website dịch vụ: `

    let inforRow_1 = worksheet.addRow(['', '', dataInforRow_1]);
    let inforRow_2 = worksheet.addRow(['', '', dataInforRow_2]);
    let inforRow_3 = worksheet.addRow(['', '', dataInforRow_3]);
    let inforRow_4 = worksheet.addRow(['', '', dataInforRow_4]);
    let inforRow_5 = worksheet.addRow(['', '', dataInforRow_5]);

    inforRow_1.font = { size: 14, bold: true };
    worksheet.mergeCells(`C${1}:E${1}`);
    worksheet.mergeCells(`C${2}:${2}`);
    worksheet.mergeCells(`C${3}:E${3}`);
    worksheet.mergeCells(`C${4}:E${4}`);
    worksheet.mergeCells(`C${5}:E${5}`);

    worksheet.addRow([]);
    /* title */
    let dataHeaderMain = "Danh sách kế hoạch OT".toUpperCase();
    let headerMain = worksheet.addRow([dataHeaderMain]);
    headerMain.font = { size: 18, bold: true };
    worksheet.mergeCells(`A${7}:G${7}`);
    headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    worksheet.addRow([]);

    let dataHeaderRow1: Array<string> = [];
    dataHeaderRow1 = ["", "Tên kế hoạch", "Loại OT", "Thời gian", "Trạng thái"];

    let headerRow1 = worksheet.addRow(dataHeaderRow1);
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };

    //merge header column
    worksheet.mergeCells(`A${9}:A${9}`);
    worksheet.mergeCells(`B${9}:B${9}`);
    worksheet.mergeCells(`C${9}:C${9}`);
    worksheet.mergeCells(`D${9}:D${9}`);
    worksheet.mergeCells(`E${9}:E${9}`);

    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
    dataHeaderRow1.forEach((item, index) => {
      if (index + 2 < dataHeaderRow1.length + 1) {
        headerRow1.getCell(index + 2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow1.getCell(index + 2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow1.getCell(index + 2).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      }
    });
    let data: Array<any> = [];
    this.listKeHoachOT.forEach((item, index) => {
      let row: Array<any> = [];
      row[0] = '';
      row[1] = item.tenKeHoach;
      row[2] = item.loaiOTName;
      row[3] = formatDate(new Date(item.ngayBatDau)) + " " + item.gioBatDau + " - " + formatDate(new Date(item.ngayKetThuc)) + " " + item.gioKetThuc;

      switch (item.trangThai) {
        case 1: {
          row[4] = "Mới tạo";
          break;
        }
        case 2: {
          row[4] = "Chờ phê duyệt";
          break;
        }
        case 3: {
          row[4] = "Đăng ký OT";
          break;
        }
        case 4: {
          row[4] = "Chờ phê duyệt đăng ký OT";
          break;
        }
        case 5: {
          row[4] = "Đã duyệt";
          break;
        }
        case 6: {
          row[4] = "Đang thực hiện";
          break;
        }
        case 7: {
          row[4] = "Hoàn thành";
          break;
        }
        case 8: {
          row[4] = "Từ chối kế hoạch OT";
          break;
        }
        case 9: {
          row[4] = "Từ chối đăng ký OT";
          break;
        }
        case 10: {
          row[4] = "Hết hạn phê duyệt kế hoạch OT";
          break;
        }
        case 11: {
          row[4] = "Hết hạn phê duyệt đăng ký OT";
          break;
        }

        default: {
          row[4] = '';
          break;
        }
      }
      data.push(row);
    });

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman', size: 11 };

      row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(5).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    });

    /* fix with for column */
    worksheet.getColumn(1).width = 20;
    worksheet.getColumn(2).width = 20;
    worksheet.getColumn(3).width = 20;
    worksheet.getColumn(4).width = 40;
    worksheet.getColumn(5).width = 28;

    this.exportToExel(workBook, title);
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  /*End*/
  onViewDetail(rowData: any) {
    this.router.navigate(['/employee/kehoach-ot-detail', { deXuatOTId: this.encrDecrService.set(rowData.keHoachOtId) }]);
  }

  goToCreate() {
    this.router.navigate(['/employee/kehoach-ot-create']);
  }


  deleteDeXuat(rowData: any) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.employeeService.deleteKehoachOT(rowData.keHoachOtId).subscribe(response => {
          let result = <any>response;
          this.loading = false;
          if (result.statusCode == 200) {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: result.message };
            this.listKeHoachOT = this.listKeHoachOT.filter(item => item.keHoachOtId != rowData.keHoachOtId);
            this.showMessage(msg);
          } else {

            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
            this.showMessage(msg);
          }
          this.setIndex();
        }, error => { });
      }
    });
  }
}

function formatDate(date) {
  return [
    padTo2Digits(date.getDate()),
    padTo2Digits(date.getMonth() + 1),
    date.getFullYear(),
  ].join('/');
}

function padTo2Digits(num) {
  return num.toString().padStart(2, '0');
}

function formatHour(date) {
  return [
    padTo2Digits(date.getHours()),
    padTo2Digits(date.getMinutes()),
  ].join(':');
}

