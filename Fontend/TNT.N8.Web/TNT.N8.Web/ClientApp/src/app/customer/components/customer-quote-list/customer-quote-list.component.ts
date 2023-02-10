import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { QuoteService } from '../../services/quote.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { SortEvent } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { CategoryService } from '../../../shared/services/category.service';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";

interface Category {
  categoryId: string,
  categoryName: string,
  categoryCode: string
}

interface Employee {
  employeeId: string,
  employeeName: string,
  employeeCode: string,
  employeeCodeName: string,
}

class ColumnExcel {
  code: string;
  name: string;
  width: number;
}

@Component({
  selector: 'app-customer-quote-list',
  templateUrl: './customer-quote-list.component.html',
  styleUrls: ['./customer-quote-list.component.css'],
  providers: [
    DatePipe
  ]
})
export class CustomerQuoteListComponent implements OnInit {
  innerWidth: number = 0; //number window size first

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    //if (this.innerWidth < )
  }

  emitParamUrl: any;
  @ViewChild('myTable') myTable: Table;

  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  applicationName = this.getDefaultApplicationName();

  loading: boolean = false;
  colsListQuote: any;
  first = 0;
  rows = 10;
  selectedColumns: any[];
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  today = new Date();
  minYear: number = 2015;
  currentYear: number = (new Date()).getFullYear();

  listStatus: Array<Category> = [];
  listPerSonInCharge: Array<Employee> = [];
  listSeller: Array<Employee> = [];
  listEmp: Array<Employee> = [];
  selectedStatus: Array<Category> = [];
  selectedPersonInCharge: Array<Employee> = [];

  selectedListSeller: Array<Employee> = [];
  selectedEmp: Array<Employee> = [];
  listQuote: Array<any> = [];
  SelectedQuote: Array<any> = [];
  quoteCode: string = '';
  quoteName: string = '';
  startDate: Date = null;
  maxEndDate: Date = new Date();
  endDate: Date = null;
  listStatusQuote: Array<any> = [];
  listEmpCreate: Array<any> = [];
  isOutOfDate: boolean = false; //Báo giá quá hạn
  isCompleteInWeek: boolean = false; //Báo giá phải hoàn thành trong tuần
  isGlobalFilter: string = '';
  isPotentialCustomer: boolean = false;
  isCustomer: boolean = false;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  userPermission: any = localStorage.getItem("UserPermission").split(',');
  createPermission: string = "order/create";
  viewPermission: string = "order/view";
  /*End*/

  /*Action*/
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  actionDelete: boolean = true;
  /*End*/

  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private quoteService: QuoteService,
    private messageService: MessageService,
    private datePipe: DatePipe,
    private confirmationService: ConfirmationService,
    private categoryService: CategoryService) {
    this.innerWidth = window.innerWidth;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  async ngOnInit() {

    //Check permission
    let resource = "crm/customer/quote-list";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("download") == -1) {
        this.actionDownload = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      this.declaireTable();

      await this.getMasterData();

      /**Nếu Link từ Dashboard báo giá sang */
      this.emitParamUrl = this.route.params.subscribe(params => {
        if (params['mode']) {
          if (params['mode'] == 'OutOfDate') {
            this.isOutOfDate = true;
            this.changeCompleteInWeek();
            this.searchQuote();
          } else if (params['mode'] == 'InWeek') {
            this.isCompleteInWeek = true;
            this.changeOutOfDate();
            this.searchQuote();
          }
        } else {
          this.isOutOfDate = false;
          this.isCompleteInWeek = false;
          this.searchQuote();
        }
      });
      /**End */
    }
  }

  declaireTable() {
    if (this.applicationName == 'VNS') {
      this.colsListQuote = [
        { field: 'quoteCode', header: 'Mã báo giá', textAlign: 'left', display: 'table-cell' },
        { field: 'quoteName', header: 'Tên báo giá', textAlign: 'left', display: 'table-cell' },
        { field: 'customerName', header: 'Khách hàng', textAlign: 'left', display: 'table-cell' },
        { field: 'createdByEmp', header: 'Người Tạo', textAlign: 'left', display: 'table-cell' },
        { field: 'quoteDate', header: 'Ngày báo giá', textAlign: 'right', display: 'table-cell' },
        { field: 'quoteStatusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
        { field: 'totalAmountAfterVat', header: 'Tổng giá trị', textAlign: 'right', display: 'table-cell' },
      ];
    }
    else {
      this.colsListQuote = [
        { field: 'quoteCode', header: 'Mã báo giá', textAlign: 'left', display: 'table-cell' },
        { field: 'quoteName', header: 'Tên báo giá', textAlign: 'left', display: 'table-cell' },
        { field: 'customerName', header: 'Khách hàng', textAlign: 'left', display: 'table-cell' },
        { field: 'sendQuoteDate', header: 'Ngày báo giá', textAlign: 'right', display: 'table-cell' },
        { field: 'intendedQuoteDate', header: 'Ngày gửi dự kiến', textAlign: 'right', display: 'table-cell' },
        { field: 'quoteStatusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
        { field: 'totalAmount', header: 'Tổng giá trị', textAlign: 'right', display: 'table-cell' },
        // { field: 'nguoiBanName', header: 'Người bán', textAlign: 'right', display: 'table-cell' },
        { field: 'nguoiPhuTrachName', header: 'Người phụ trách', textAlign: 'right', display: 'table-cell' },
      ];
    }
    this.selectedColumns = this.colsListQuote;
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.quoteService.getMasterDataSearchQuote();
    if (result.statusCode == 200) {
      this.listEmp = result.listEmp;
    }
    else {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(mgs);
    }
  }

  searchQuote() {
    this.loading = true;
    let startDate = this.startDate;
    if (startDate) {
      startDate.setHours(0, 0, 0, 0);
      startDate = convertToUTCTime(startDate);
    }

    let endDate = this.endDate;
    if (endDate) {
      endDate.setHours(23, 59, 59, 999);
      endDate = convertToUTCTime(endDate);
    }

    let listEmpCreateId = this.selectedEmp.map(x => x.employeeId);

    let listNhanVienPhuTrach = this.selectedPersonInCharge.map(x => x.employeeId);

    let listSeller = this.selectedListSeller.map(x => x.employeeId);
    this.listStatusQuote = this.selectedStatus.map(x => x.categoryId);
    this.quoteService.searchQuote(this.quoteCode, startDate, endDate,
      this.listStatusQuote, this.isOutOfDate, this.isCompleteInWeek, this.quoteName, this.isPotentialCustomer, this.isCustomer, listEmpCreateId, listNhanVienPhuTrach, listSeller).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          if (this.listStatus.length == 0) {
            this.listStatus = result.listStatus;
          }
          this.listQuote = result.listQuote;
          this.listPerSonInCharge = result.listNhanVienCapDuoi; // nhân viên phụ trách báo giá thuộc quyền quản lý của tài khoản đăng nhập
          this.listSeller = result.listNhanVienCapDuoi; // seller
          // this.listSeller = result.listNguoiBan; // seller

          this.isShowFilterLeft = false;
          this.leftColNumber = 12;
          this.rightColNumber = 0;
          if (this.listQuote.length == 0) {
            let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy báo giá nào!' };
            this.showMessage(mgs);
          } else {
            this.listQuote.forEach(item => {
              item.quoteDate = this.datePipe.transform(item.quoteDate, 'dd/MM/yyyy');
            });
          }
        } else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
        }
      });
  }

  resetTable() {
    if (this.myTable) {
      this.myTable.reset();
    }
  }

  // Refresh parameter search
  refreshFilter() {
    this.quoteCode = '';
    this.quoteName = '';
    this.startDate = null;
    this.endDate = null;
    this.selectedPersonInCharge = [];
    this.selectedListSeller = [];

    this.selectedStatus = [];
    this.selectedEmp = [];
    this.isOutOfDate = false;
    this.isCompleteInWeek = false;
    this.isGlobalFilter = '';
    this.isPotentialCustomer = false;
    this.isCustomer = false;
    this.resetTable();

    this.searchQuote();
  }

  pageChange(event: any) {
  }

  leftColNumber: number = 12;
  rightColNumber: number = 2;
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

  checkExpirationDate(expirationDate: Date) {
    if (expirationDate != null) {
      var formatedExpirationDate = new Date(expirationDate);
      formatedExpirationDate.setHours(this.today.getHours(), this.today.getMinutes(), this.today.getMilliseconds());
      if (formatedExpirationDate < this.today) {
        return true;
      }
      else {
        return false;
      }
    } else return false;
  }

  /**Xóa báo giá */
  del_quote(quoteId: any) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.quoteService.updateActiveQuote(quoteId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
            this.searchQuote();
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          }
        }, error => { });
      }
    });
  }

  dateFieldFormat: string = 'DD/MM/YYYY';
  customSort(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];

      /**Customize sort date */
      if (event.field == 'quoteDate') {
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

  changeOutOfDate() {
    if (this.isOutOfDate) {
      this.selectedStatus = this.listStatus.filter(item => item.categoryCode == 'MTA' || item.categoryCode == 'CHO' || item.categoryCode == 'DLY');
      this.isCompleteInWeek = false;
    }
  }

  changeCompleteInWeek() {
    if (this.isCompleteInWeek) {
      this.selectedStatus = this.listStatus.filter(item => item.categoryCode == 'MTA' || item.categoryCode == 'CHO' || item.categoryCode == 'DLY');
      this.isOutOfDate = false;
    }
  }

  setDafaultStartDate(): Date {
    let date = new Date();
    date.setDate(1);

    return date;
  }

  goToCreateQuote() {
    this.router.navigate(['/customer/quote-create']);
  }

  goToDetail(id: string) {
    this.router.navigate(['/customer/quote-detail', { quoteId: id }]);
  }

  ngOnDestroy() {
    if (this.emitParamUrl) {
      this.emitParamUrl.unsubscribe();
    }
  }

  getDefaultApplicationName() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "ApplicationName") ?.systemValueString;
  }

  exportExcel() {
    if (this.applicationName == 'VNS') {
      if (this.SelectedQuote.length > 0) {
        if (this.selectedColumns.length > 0) {
          let title = `Danh sách báo giá`;

          let workBook = new Workbook();
          let worksheet = workBook.addWorksheet(title);

          let dataHeaderMain = title.toUpperCase();
          let headerMain = worksheet.addRow([dataHeaderMain]);
          headerMain.font = { size: 18, bold: true };
          worksheet.mergeCells(`A${1}:G${1}`);
          headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          worksheet.addRow([]);

          /* Header row */
          let buildColumnExcel = this.buildColumnExcel();
          let dataHeaderRow = buildColumnExcel.map(x => x.name);
          let headerRow = worksheet.addRow(dataHeaderRow);
          headerRow.font = { name: 'Times New Roman', size: 10, bold: true };
          dataHeaderRow.forEach((item, index) => {
            headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
            headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
            headerRow.getCell(index + 1).fill = {
              type: 'pattern',
              pattern: 'solid',
              fgColor: { argb: '8DB4E2' }
            };
          });
          headerRow.height = 40;

          /* Data table */
          let data: Array<any> = [];
          this.SelectedQuote.forEach(item => {
            let row: Array<any> = [];
            buildColumnExcel.forEach((_item, _index) => {
              if (_item.code == 'quoteCode') {
                row[_index] = item.quoteCode;
              }
              else if (_item.code == 'quoteName') {
                row[_index] = item.quoteName;
              }
              else if (_item.code == 'customerName') {
                row[_index] = item.customerName;
              }
              else if (_item.code == 'createdByEmp') {
                row[_index] = item.createdByEmp;
              }
              else if (_item.code == 'quoteDate') {
                row[_index] = item.quoteDate
              }
              else if (_item.code == 'quoteStatusName') {
                row[_index] = item.quoteStatusName;
              }
              else if (_item.code == 'totalAmountAfterVat') {
                row[_index] = item.totalAmountAfterVat;
              }
            });

            data.push(row);
          });

          data.forEach((el, index, array) => {
            let row = worksheet.addRow(el);
            row.font = { name: 'Times New Roman', size: 11 };

            buildColumnExcel.forEach((_item, _index) => {
              row.getCell(_index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              if (_item.code == 'quoteCode' || _item.code == 'quoteName' || _item.code == 'customerName' || _item.code == 'createdByEmp') {
                row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
              }
              else if (_item.code == 'quoteDate' || _item.code == 'totalAmountAfterVat') {
                row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'right' };
              }
              else {
                row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'center' };
              }
            });
          });

          /* fix with for column */
          buildColumnExcel.forEach((item, index) => {
            worksheet.getColumn(index + 1).width = item.width;
          });

          this.exportToExel(workBook, title);
        }
        else {
          let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn phải chọn ít nhất 1 cột' };
          this.showMessage(mgs);
        }
      }
      else {
        let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn chưa chọn khách hàng' };
        this.showMessage(mgs);
      }
    }
    else {
      if (this.SelectedQuote.length > 0) {
        if (this.SelectedQuote.length > 0) {
          let title = `Danh sách báo giá`;

          let workBook = new Workbook();
          let worksheet = workBook.addWorksheet(title);

          let dataHeaderMain = "Danh sách báo giá".toUpperCase();
          let headerMain = worksheet.addRow([dataHeaderMain]);
          headerMain.font = { size: 18, bold: true };
          worksheet.mergeCells(`A${1}:F${1}`);
          headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          worksheet.addRow([]);

          /* Header row */
          let buildColumnExcel = this.buildColumnExcel();
          let dataHeaderRow = buildColumnExcel.map(x => x.name);
          let headerRow = worksheet.addRow(dataHeaderRow);
          headerRow.font = { name: 'Times New Roman', size: 12, bold: true };
          dataHeaderRow.forEach((item, index) => {
            headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
            headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
            headerRow.getCell(index + 1).fill = {
              type: 'pattern',
              pattern: 'solid',
              fgColor: { argb: '8DB4E2' }
            };
          });
          headerRow.height = 40;

          /* Data table */
          let data: Array<any> = [];
          this.SelectedQuote.forEach(item => {
            let row: Array<any> = [];
            buildColumnExcel.forEach((_item, _index) => {
              if (_item.code == 'quoteCode') {
                row[_index] = item.quoteCode;
              }
              else if (_item.code == 'quoteName') {
                row[_index] = item.quoteName;
              }
              else if (_item.code == 'customerName') {
                row[_index] = item.customerName;
              }
              else if (_item.code == 'quoteDate') {
                row[_index] = item.quoteDate;
              }
              else if (_item.code == 'quoteStatusName') {
                row[_index] = item.quoteStatusName;
              }
              else if (_item.code == 'totalAmount') {
                row[_index] = item.totalAmount;
              }
            });
            data.push(row);
          });

          data.forEach((el, index, array) => {
            let row = worksheet.addRow(el);
            row.font = { name: 'Times New Roman', size: 12 };

            buildColumnExcel.forEach((_item, _index) => {
              row.getCell(_index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              if (_item.code == 'quoteCode' || _item.code == 'quoteName' || _item.code == 'customerName') {
                row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
              }
              else if (_item.code == 'quoteStatusName') {
                row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
              }
              else {
                row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
              }
            });
          });

          /* fix with for column */
          buildColumnExcel.forEach((item, index) => {
            worksheet.getColumn(index + 1).width = item.width;
          });

          this.exportToExel(workBook, title);
        }
        else {
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn phải chọn ít nhất 1 cột' };
          this.showMessage(msg);
        }
      }
      else {
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn chưa chọn báo giá' };
        this.showMessage(msg);
      }
    }
  }

  buildColumnExcel(): Array<ColumnExcel> {
    if (this.applicationName == 'VNS') {
      let listColumn: Array<ColumnExcel> = [];

      this.selectedColumns.forEach(item => {
        let column = new ColumnExcel();
        column.code = item.field;
        column.name = item.header;

        if (item.field == 'quoteCode') {
          column.width = 30.71;
        }
        else if (item.field == 'quoteName') {
          column.width = 30.71;
        }
        else if (item.field == 'customerName') {
          column.width = 30.71;
        }
        else if (item.field == 'createdByEmp') {
          column.width = 15.71;
        }
        else if (item.field == 'quoteDate') {
          column.width = 25.71;
        }
        else if (item.field == 'quoteStatusName') {
          column.width = 30.71;
        }
        else if (item.field == 'totalAmountAfterVat') {
          column.width = 30.71;
        }

        listColumn.push(column);
      });

      return listColumn;
    }
    else {
      let listColumn: Array<ColumnExcel> = [];

      this.selectedColumns.forEach(item => {
        let column = new ColumnExcel();
        column.code = item.field;
        column.name = item.header;

        if (item.field == 'quoteCode') {
          column.width = 30.71;
        }
        else if (item.field == 'quoteName') {
          column.width = 35.71;
        }
        else if (item.field == 'customerName') {
          column.width = 35.71;
        }
        else if (item.field == 'quoteDate') {
          column.width = 25.71;
        }
        else if (item.field == 'quoteStatusName') {
          column.width = 20.71;
        }
        else if (item.field == 'totalAmount') {
          column.width = 20.71;
        }

        listColumn.push(column);
      });

      return listColumn;
    }
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
