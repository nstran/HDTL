import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ManufactureService } from '../../services/manufacture.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DialogService } from 'primeng';

class TotalProductionOrder {
  totalProductionOrderId: string;
  code: string;
  statusId: string;
  statusName: string;
  periodId: string;
  startDate: Date;
  totalQuantity: number;
  totalArea: number;
  minEndDate: Date;
  maxEndDate: Date;
  createdDate: Date;
  createdById: string;
}

class Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}

@Component({
  selector: 'app-total-production-order-list',
  templateUrl: './total-production-order-list.component.html',
  styleUrls: ['./total-production-order-list.component.css']
})
export class TotalProductionOrderListComponent implements OnInit {
  loading: boolean = false;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;

  cols: any;
  selectedColumns: any[];

  @ViewChild('myTable') myTable: Table;

  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  innerWidth: number = 0; //number window size first

  listTotalProductionOrder: Array<TotalProductionOrder> = [];

  code: string = '';
  startDate: Date = null;
  totalQuantity: any = null;
  totalArea: any = null;
  minEndDate: Date = null;
  maxEndDate: Date = null;
  listStatus: Array<Category> = [];
  selectedStatus: Array<Category> = [];

  firstNumber: number = 0;
  totalRecords: number = 0;
  rows: number = 10;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private manufactureService: ManufactureService,
    private dialogService: DialogService,
    private messageService: MessageService,
  ) {
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    let resource = "man/manufacture/total-production-order/list";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    this.initTable();
    this.getMasterData();
  }

  initTable() {
    this.cols = [
      { field: 'code', header: 'Lệnh số', textAlign: 'left', display: 'table-cell' },
      { field: 'startDate', header: 'Ngày sx', textAlign: 'left', display: 'table-cell' },
      { field: 'totalQuantity', header: 'Tổng số tấm', textAlign: 'center', display: 'table-cell' },
      { field: 'totalArea', header: 'Tổng số m2', textAlign: 'center', display: 'table-cell' },
      { field: 'minEndDate', header: 'Ngày trả gần nhất', textAlign: 'right', display: 'table-cell' },
      { field: 'maxEndDate', header: 'Ngày trả xa nhất', textAlign: 'right', display: 'table-cell' },
    ];
    this.selectedColumns = this.cols;
  }

  getMasterData() {
    this.manufactureService.getMasterDataSearchTotalProductionOrder().subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listStatus = result.listStatus;
        this.searchTotalProductionOrder();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  searchTotalProductionOrder() {
    let listStatusId: Array<string> = this.selectedStatus.map(x => x.categoryId);

    let startDate = this.startDate;
    if (startDate) {
      startDate = convertToUTCTime(startDate);
    }

    let minEndDate = this.minEndDate;
    if (minEndDate) {
      minEndDate = convertToUTCTime(minEndDate);
    }

    let maxEndDate = this.maxEndDate;
    if (maxEndDate) {
      maxEndDate = convertToUTCTime(maxEndDate);
    }

    let totalQuantity = this.totalQuantity;
    if (totalQuantity) {
      totalQuantity = ParseStringToFloat(totalQuantity);
    }

    let totalArea = this.totalArea;
    if (totalArea) {
      totalArea = ParseStringToFloat(totalArea);
    }

    this.loading = true;
    this.manufactureService.searchTotalProductionOrder(this.code.trim(), startDate, totalQuantity,
      totalArea, minEndDate, maxEndDate, listStatusId, this.firstNumber, this.rows).subscribe(response => {
        let result: any = response;
        this.loading = false;

        if (result.statusCode == 200) {
          this.totalRecords = result.totalRecords;
          this.listTotalProductionOrder = result.listTotalProductionOrder;
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
  }

  paginate(event: any) {
    //event.first = Index of the first record
    //event.rows = Number of rows to display in new page
    //event.page = Index of the new page
    //event.pageCount = Total number of pages
    this.firstNumber = event.first;
    this.searchTotalProductionOrder();
  }

  refreshFilter() {
    this.code = '';
    this.startDate = null;
    this.totalQuantity = null;
    this.totalArea = null;
    this.minEndDate = null;
    this.maxEndDate = null;
    this.selectedStatus = [];

    this.searchTotalProductionOrder();
  }

  goToCreate() {
    this.router.navigate(['/manufacture/total-production-order/create']);
  }

  goToDetail(id: string) {
    //this.router.navigate(['/manufacture/total-production-order/detail', {totalProductionOrderId: id}]);
    let url = '/manufacture/total-production-order/detail' + ';totalProductionOrderId=' + id;
    window.open(url, "_blank");
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

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
};
