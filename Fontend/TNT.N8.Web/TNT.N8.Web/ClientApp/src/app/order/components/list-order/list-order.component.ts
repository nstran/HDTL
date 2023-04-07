import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CustomerOrderService } from '../../services/customer-order.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DatePipe } from '@angular/common';
import 'moment/locale/pt-br';
import { DecimalPipe } from '@angular/common';
import { ReSearchService } from '../../../services/re-search.service';
import { EmployeeEntityModel, ServicePacketEntityModel, TrangThaiGeneral } from '../../../../../src/app/product/models/product.model';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";

interface Vat {
  value: number,
  name: string
}

interface Order {
  orderId: string,
  orderCode: string,
  statusOrder: number,
  orderDate: string,
  seller: string,
  sellerName: string,
  description: string,
  note: string,
  customerId: string,
  customerName: string,
  customerContactId: string,
  paymentMethod: string,
  daysAreOwed: number,
  maxDebt: number,
  receivedDate: Date,
  receivedHour: Date,
  recipientName: string,
  locationOfShipment: string,
  shippingNote: string,
  recipientPhone: string,
  recipientEmail: string,
  placeOfDelivery: string,
  amount: number,
  discountType: boolean,
  discountValue: number,
  statusId: string,
  statusCode: string,
  orderStatusName: string,
  sellerFirstName: string,
  sellerLastName: string,
  listOrderDetail: string,
  backgroundColorForStatus: string,
  canDelete: boolean,
  orderTypeName: string,
  orderType: number,
  listPacketServiceName: string,
  createdDate: string,
  creatorname: string
}

@Component({
  selector: 'app-list-order',
  templateUrl: './list-order.component.html',
  styleUrls: ['./list-order.component.css'],
  providers: [
    DecimalPipe,
    DatePipe,

  ]
})
export class ListOrderComponent implements OnInit {
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  loading: boolean = false;
  innerWidth: number = 0; //number window size first
  toDay: Date = new Date();

  listOrder: Array<Order> = [];
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxStartDate: Date = new Date();
  isGlobalFilter: string = '';

  //Bộ lọc
  listPacketService: Array<ServicePacketEntityModel> = [];
  listCreator: Array<EmployeeEntityModel> = [];
  listStatus: Array<TrangThaiGeneral> = [];


  packetSelected: Array<ServicePacketEntityModel> = [];
  creatorSelected: Array<EmployeeEntityModel> = [];
  statusSelected: Array<TrangThaiGeneral> = [];
  fromDate: Date = null;
  toDate: Date = null;

  @ViewChild('myTable') myTable: Table;
  colsListOrder: any;
  selectedColumns: any[];

  public Seller: string = '';   //id nhân viên bán hàng nhận từ dashboard bán hàng

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private customerOrderService: CustomerOrderService,
    private messageService: MessageService,
    private decimalPipe: DecimalPipe,
    private confirmationService: ConfirmationService,
    private datePipe: DatePipe,
    public reSearchService: ReSearchService
  ) {
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    this.initTable();
    let resource = "cusOrder/order/list";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("download") == -1) {
        this.actionDownload = false;
      }
      let isOrderAction = false;
      this.customerOrderService.getMasterDataOrderSearch(isOrderAction).subscribe(response => {
        let result: any = response;
        this.loading = true;
        if (result.statusCode == 200) {
          this.listPacketService = result.listPacketService;
          this.listCreator = result.listCreator;
          this.listStatus = result.listStatus;

          this.searchOrder();
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  initTable() {
    this.colsListOrder = [
      { field: 'orderCode', header: 'Mã phiếu', textAlign: 'left', display: 'table-cell' },
      { field: 'orderTypeName', header: 'Loại phiếu phiếu', textAlign: 'left', display: 'table-cell' },
      { field: 'createdDate', header: 'Ngày yêu cầu', textAlign: 'center', display: 'table-cell' },
      { field: 'creatorname', header: 'Người tạo yêu cầu', textAlign: 'left', display: 'table-cell' },
      { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left', display: 'table-cell' },
      { field: 'listPacketServiceName', header: 'Gói dịch vụ yêu cầu', textAlign: 'left', display: 'table-cell' },
      { field: 'amount', header: 'Tổng tiền', textAlign: 'left', display: 'table-cell' },
      { field: 'orderStatusName', header: 'Trạng thái', textAlign: 'left', display: 'table-cell' },
    ];
    this.selectedColumns = this.colsListOrder;
  }

  searchOrder() {
    let listPacketIdSelected = this.packetSelected.map(x => x.id);
    let listCreatorIdSelected = this.creatorSelected.map(x => x.employeeId);
    let listStatusSelected = this.statusSelected.map(x => x.value);

    let fromDate = null;
    let toDate = null;
    if (this.fromDate) fromDate = convertToUTCTime(this.fromDate);
    if (this.toDate) toDate = convertToUTCTime(this.toDate);

    let IsOrderAction = false;
    this.loading = true;
    this.customerOrderService.searchOrder(listPacketIdSelected, listCreatorIdSelected, listStatusSelected, fromDate, toDate, null ,IsOrderAction).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listOrder = result.listOrder;
        if (this.listOrder.length == 0) {
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy phiếu yêu cầu hỗ trợ nào!' };
          this.showMessage(msg);
        }
        else {
          this.handleBackGroundColorForStatus();
          this.listOrder.forEach(item => {
            item.orderDate = this.datePipe.transform(item.orderDate, 'dd/MM/yyyy');
          });
        }
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  handleBackGroundColorForStatus() {
    this.listOrder.forEach(item => {
      switch (item.statusOrder) {
        case 1://bi tra lai
          item.backgroundColorForStatus = "#95CEFF";
          break;
        case 2://da dong
          item.backgroundColorForStatus = "#6D98E7";
          break;
        case 3://da giao hang
          item.backgroundColorForStatus = "#66CC00";
          break;
        case 4://da thanh toan
          item.backgroundColorForStatus = "#DDDD3B";
          break;
        case 5://dang xu ly
          item.backgroundColorForStatus = "#34c759";
          break;
        case 6://hoan
          item.backgroundColorForStatus = "#F54E5E";
          break;
        default:
          item.backgroundColorForStatus = "#F54E5E";
          break;
      }
    });
  }

  refreshFilter() {
    this.packetSelected = [];
    this.creatorSelected = [];
    this.statusSelected = [];
    this.fromDate = null;
    this.toDate = null;
    this.resetTable();
    this.searchOrder();
  }

  resetTable() {
    if (this.myTable) {
      this.myTable.reset();
    }
  }

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

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  goToCreateOrder() {
    this.router.navigate(['/order/create']);
  }

  goToOrderDetail(orderId: string) {
    this.router.navigate(['order/create', { OrderId: orderId }]);
  }

  goToCustomerDetail(customerId: string) {
    this.router.navigate(['/customer/detail', { customerId: customerId }]);
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  deleteOrder(rowData: Order) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.customerOrderService.deleteOrder(rowData.orderId).subscribe(response => {
          this.loading = false;
          const result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.listOrder = this.listOrder.filter(e => e != rowData);
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Xóa phiếu yêu cầu thành công');
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    });
  }

  exportExcel() {
    let pdfOrder = this.listOrder;
    let title = "Danh sách phiếu yêu cầu dịch vụ";
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet(title);
    worksheet.pageSetup.margins = {
      left: 0.25, right: 0.25,
      top: 0.75, bottom: 0.75,
      header: 0.3, footer: 0.3
    };
    worksheet.pageSetup.paperSize = 9;  //A4 : 9

    /* Header row */
    // let dataHeaderRow = ['Mã phiếu', '', '','Loại phiếu','', 'Tên khách hàng', '', '', '', 'Ngày đặt hàng', '', 'Trạng thái', '', 'Tổng giá trị', '', 'Nhân viên bán hàng', '', 'Chi tiết', ''];
    let dataHeaderRow = ['Mã phiếu', '', '', 'Loại phiếu', '', 'Ngày yêu cầu', '', 'Người tạo yêu cầu', '', 'Tên khách hàng', '', 'Gói dịch vụ yêu cầu', '', 'Tổng tiền', '', 'Trạng thái', ''];
    
    let headerRow = worksheet.addRow(dataHeaderRow);
    worksheet.mergeCells(`A${headerRow.number}:C${headerRow.number}`);
    worksheet.mergeCells(`D${headerRow.number}:E${headerRow.number}`);
    worksheet.mergeCells(`F${headerRow.number}:G${headerRow.number}`);
    worksheet.mergeCells(`H${headerRow.number}:I${headerRow.number}`);
    worksheet.mergeCells(`J${headerRow.number}:K${headerRow.number}`);
    worksheet.mergeCells(`L${headerRow.number}:M${headerRow.number}`);
    worksheet.mergeCells(`N${headerRow.number}:O${headerRow.number}`);
    worksheet.mergeCells(`P${headerRow.number}:Q${headerRow.number}`);
    // worksheet.mergeCells(`R${headerRow.number}:S${headerRow.number}`);
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
    headerRow.height = 35;

    /* Data table */
    let data: Array<any> = []; //[1, 'Dịch vụ CNTT', 'Gói', '2', '6.000.000', '12.000.000']

    let rg = /\, /gi;
    pdfOrder.forEach(item => {
      let row: Array<any> = [];
      row[0] = item.orderCode;
      row[3] = item.orderTypeName;
      row[5] = this.datePipe.transform(item.createdDate, 'dd/MM/yyyy');;
      row[7] = item.creatorname;
      row[9] = item.customerName;
      row[11] = item.listPacketServiceName.replace(rg, "");
      row[13] = this.decimalPipe.transform(item.amount).toString(); // item.amount;
      row[15] = item.orderStatusName;

      data.push(row);
    });

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      worksheet.mergeCells(`A${row.number}:C${row.number}`);
      worksheet.mergeCells(`D${row.number}:E${row.number}`);
      worksheet.mergeCells(`F${row.number}:G${row.number}`);
      worksheet.mergeCells(`H${row.number}:I${row.number}`);
      worksheet.mergeCells(`J${row.number}:K${row.number}`);
      worksheet.mergeCells(`L${row.number}:M${row.number}`);
      worksheet.mergeCells(`N${row.number}:O${row.number}`);
      worksheet.mergeCells(`P${row.number}:Q${row.number}`);
      // worksheet.mergeCells(`R${row.number}:S${row.number}`);

      row.font = { name: 'Times New Roman', size: 11 };

      row.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(5).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(6).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      row.getCell(7).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(7).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(8).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(8).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      row.getCell(9).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(9).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(10).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(10).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      row.getCell(11).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(11).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(12).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      row.getCell(13).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(13).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(14).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(14).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(15).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(15).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      row.getCell(16).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(16).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      row.getCell(17).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(17).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
    });

    this.exportToExel(workbook, title);
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
