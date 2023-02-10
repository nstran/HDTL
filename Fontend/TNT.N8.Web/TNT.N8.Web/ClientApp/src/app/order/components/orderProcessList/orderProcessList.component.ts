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
import { EmployeeEntityModel, OrderProcessDetailEntityModel, OrderProcessEntityModel, ProductCategoryEntityModel, ServicePacketEntityModel, TrangThaiGeneral } from '../../../product/models/product.model';


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

export interface CustomerModel {
  customerId: string,
  customerCodeName: string,
}

@Component({
  selector: 'app-orderProcessList',
  templateUrl: './orderProcessList.component.html',
  styleUrls: ['./orderProcessList.component.css'],
  providers: [
    DecimalPipe,
    DatePipe,

  ]
})
export class OrderProcessListComponent implements OnInit {
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  loading: boolean = false;
  innerWidth: number = 0; //number window size first
  toDay: Date = new Date();

  listOrderProcess: Array<OrderProcessEntityModel> = [];
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
  listCus: Array<CustomerModel> = [];
  listAllProductCategory: Array<ProductCategoryEntityModel> = [];


  packetSelected: Array<ServicePacketEntityModel> = [];
  creatorSelected: Array<EmployeeEntityModel> = [];
  statusSelected: Array<TrangThaiGeneral> = [];
  fromDate: Date = null;
  toDate: Date = null;
  cusSelected: Array<CustomerModel> = [];
  productCategorySelected: Array<ProductCategoryEntityModel> = [];

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
    let resource = "cusOrder/order/orderProcessList";
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
      this.customerOrderService.getMasterDataOrderSearch(false).subscribe(response => {
        let result: any = response;
        this.loading = true;
        if (result.statusCode == 200) {
          this.listCus = result.listCus;
          this.listStatus = result.listStatus;
          this.searchOrder();
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }

    let isOrderAction = false;
    let isOrderProcess = true;
    this.customerOrderService.getMasterDataOrderSearch(isOrderAction, isOrderProcess).subscribe(response => {
      let result: any = response;
      this.loading = true;
      if (result.statusCode == 200) {
        this.listPacketService = result.listPacketService;
        this.listCreator = result.listCreator;
        this.listStatus = result.listStatus;
        this.listAllProductCategory = result.listAllProductCategory;
        this.listCus = result.listCus;
        this.searchOrder();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  initTable() {
    this.colsListOrder = [
      { field: 'orderProcessCode', header: 'Mã phiếu', textAlign: 'left', display: 'table-cell' },
      { field: 'productCategoryName', header: 'Loại dịch vụ', textAlign: 'left', display: 'table-cell' },
      { field: 'customerName', header: 'Khách hàng', textAlign: 'left', display: 'table-cell' },
      { field: 'servicePacketName', header: 'Gói hàng', textAlign: 'left', display: 'table-cell' },
      { field: 'createdDate', header: 'Ngày tạo', textAlign: 'center', display: 'table-cell' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
    ];
    this.selectedColumns = this.colsListOrder;
  }

  searchOrder() {
    let listPacketIdSelected = this.packetSelected.map(x => x.id);
    let listCreatorIdSelected = this.creatorSelected.map(x => x.employeeId);
    let listStatusSelected = this.statusSelected.map(x => x.value);

    let listCusSelected = this.cusSelected.map(x => x.customerId);
    let listProductCateSelected = this.productCategorySelected.map(x => x.productCategoryId);


    let fromDate = null;
    let toDate = null;
    if (this.fromDate) fromDate = convertToUTCTime(this.fromDate);
    if (this.toDate) toDate = convertToUTCTime(this.toDate);

    let IsOrderAction = false;
    let IsOrderProcess = true;
    this.loading = true;
    this.customerOrderService.searchOrder(listPacketIdSelected, listCreatorIdSelected, listStatusSelected, fromDate, toDate, null, IsOrderAction, IsOrderProcess, listCusSelected, listProductCateSelected).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listOrderProcess = result.listOrderProcess;
        if (this.listOrderProcess.length == 0) {
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy phiếu yêu cầu hỗ trợ nào!' };
          this.showMessage(msg);
        }
        else {
          this.handleBackGroundColorForStatus();
          // this.listOrderProcess.forEach(item => {
          //   item.orderDate = this.datePipe.transform(item.orderDate, 'dd/MM/yyyy');
          // });
        }
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  handleBackGroundColorForStatus() {
    this.listOrderProcess.forEach(item => {
      switch (item.status) {
        case 1://bi tra lai
          item.backgroundColorForStatus = "#95CEFF";
          break;
        case 2://da dong
          item.backgroundColorForStatus = "#1994FF";
          break;
        case 3://da giao hang
          item.backgroundColorForStatus = "#66CC00";
          break;
        case 4://da thanh toan
          item.backgroundColorForStatus = "#9C00FF";
          break;
        case 5://dang xu ly
          item.backgroundColorForStatus = "#F54E5E";
          break;
        case 6://hoan
          item.backgroundColorForStatus = "#666666";
          break;
        default:
          item.backgroundColorForStatus = "#ffcc00";
          break;
      }
    });
  }

  refreshFilter() {
    this.packetSelected = [];
    this.cusSelected = [];
    this.listAllProductCategory = [];
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
    this.router.navigate(['/order/orderProcess']);
  }

  goToOrderProcessDetail(orderProcessId: string) {
    this.router.navigate(['order/orderProcess', { OrderProcessId: orderProcessId }]);
  }



  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  // deleteOrder(rowData: Order) {
  //   this.confirmationService.confirm({
  //     message: 'Bạn có chắc chắn muốn xóa?',
  //     accept: () => {
  //       this.loading = true;
  //       this.customerOrderService.deleteOrder(rowData.orderId).subscribe(response => {
  //         this.loading = false;
  //         const result = <any>response;
  //         if (result.statusCode === 202 || result.statusCode === 200) {
  //           this.listOrderProcess = this.listOrderProcess.filter(e => e != rowData);
  //           this.clearToast();
  //           this.showToast('success', 'Thông báo', 'Xóa phiếu yêu cầu thành công');
  //         } else {
  //           this.clearToast();
  //           this.showToast('error', 'Thông báo', result.messageCode);
  //         }
  //       });
  //     }
  //   });
  // }

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
