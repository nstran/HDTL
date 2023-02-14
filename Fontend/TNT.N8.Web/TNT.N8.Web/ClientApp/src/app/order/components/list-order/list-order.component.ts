import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';

import { Router, ActivatedRoute } from '@angular/router';
import { CustomerOrderService } from '../../services/customer-order.service';
import { GetPermission } from '../../../shared/permission/get-permission';

import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { SortEvent } from 'primeng/api';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { DecimalPipe } from '@angular/common';
import { ListOrderSearch } from '../../../shared/models/re-search/list-order-search.model';
import { ReSearchService } from '../../../services/re-search.service';
import { EmployeeEntityModel, ServicePacketEntityModel, TrangThaiGeneral } from '../../../../../src/app/product/models/product.model';


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

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};