import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { ManufactureService } from '../../services/manufacture.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng';

class TotalProductionOrder {
  totalProductionOrderId: string;
  code: string;
  periodId: string;
  startDate: Date;
  createdDate: Date;
  createdById: string;

  constructor() {
    this.totalProductionOrderId = '00000000-0000-0000-0000-000000000000';
    this.code = null;
    this.periodId = null;
    this.startDate = null;
    this.createdDate = convertToUTCTime(new Date());
    this.createdById = '00000000-0000-0000-0000-000000000000';
  }
}

class Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefault: boolean;
}

class ProductionOrder {
  productionOrderId: string;
  productionOrderCode: string;
  customerName: string;
  remainQuantity: number;
  remainTotalArea: number;
  endDate: Date;
  statusId: string;
  statusName: string;
  noteTechnique: string;
  especially: boolean;
}

@Component({
  selector: 'app-total-production-order-create',
  templateUrl: './total-production-order-create.component.html',
  styleUrls: ['./total-production-order-create.component.css']
})
export class TotalProductionOrderCreateComponent implements OnInit {
  loading: boolean = false;
  actionAdd: boolean = true;
  awaitResult: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(sessionStorage.getItem("auth"));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  listPeriod: Array<Category> = [];
  listStatus: Array<Category> = [];

  /* FORM */
  totalProductionOrderForm: FormGroup;
  startDateControl: FormControl;
  statusControl: FormControl;
  /* END FORM */

  cols: Array<any> = [];
  listProductionOrder: Array<ProductionOrder> = [];
  selectedProductionOrder: Array<ProductionOrder> = [];

  totalQuantity: number = 0;
  totalArea: number = 0;
  minEndDate: Date = null;
  maxEndDate: Date = null;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private manufactureService: ManufactureService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private renderer: Renderer2,
    private confirmationService: ConfirmationService,
  ) { }

  async  ngOnInit() {
    this.setForm();
    let resource = "man/manufacture/total-production-order/create";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
    }

    this.initTable();
    this.getMasterData();
  }

  setForm() {
    this.startDateControl = new FormControl(new Date(), [Validators.required]);
    this.statusControl = new FormControl(null, [Validators.required]);

    this.totalProductionOrderForm = new FormGroup({
      startDateControl: this.startDateControl,
      statusControl: this.statusControl
    });
  }

  initTable() {
    this.cols = [
      { field: 'productionOrderCode', header: 'Lệnh số', textAlign: 'left', display: 'table-cell' },
      { field: 'customerName', header: 'Khách hàng', textAlign: 'left', display: 'table-cell' },
      { field: 'remainQuantity', header: 'Số tấm', textAlign: 'center', display: 'table-cell' },
      { field: 'remainTotalArea', header: 'Số m2', textAlign: 'center', display: 'table-cell' },
      { field: 'endDate', header: 'Ngày trả', textAlign: 'right', display: 'table-cell' },
      { field: 'statusName', header: 'Trạng thái SX', textAlign: 'left', display: 'table-cell' },
      { field: 'noteTechnique', header: 'Ghi chú KT', textAlign: 'left', display: 'table-cell' }
    ];
  }

  getMasterData() {
    this.loading = true;
    this.manufactureService.getMasterDataCreateTotalProductionOrder().subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listStatus = result.listStatus;
        let status_default = this.listStatus.find(x => x.isDefault == true);
        if (status_default) {
          this.statusControl.setValue(status_default);
        }

        this.listProductionOrder = result.listProductionOrder;
        this.listProductionOrder.forEach(item => {
          if (item.endDate) {
            item.endDate = new Date(item.endDate);
          }
        });
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /* Event Chọn tất cả lệnh sản xuất */
  changeCheckBoxAll() {
    this.statisticTotal();
  }

  /* Event Chọn item trong list lệnh sản xuất */
  changeCheckBox() {
    this.statisticTotal();
  }

  /* Thống kê lệnh sản xuất */
  statisticTotal() {
    let tmp_quantity = 0;
    this.selectedProductionOrder.forEach(item => {
      tmp_quantity += item.remainQuantity;
    });
    this.totalQuantity = this.roundNumber(tmp_quantity, 2);

    let tmp_area = 0;
    this.selectedProductionOrder.forEach(item => {
      tmp_area += item.remainTotalArea;
    });
    this.totalArea = this.roundNumber(tmp_area, 2);

    let listDate = this.selectedProductionOrder.map((x) => { if (x.endDate) return x.endDate });

    if (listDate.length > 0) {
      this.maxEndDate = new Date(Math.max.apply(null, listDate));
      this.minEndDate = new Date(Math.min.apply(null, listDate));
    } else {
      this.maxEndDate = null;
      this.minEndDate = null;
    }
  }

  createTotalProductionOrder(mode: boolean) {
    if (!this.totalProductionOrderForm.valid) {
      Object.keys(this.totalProductionOrderForm.controls).forEach(key => {
        if (this.totalProductionOrderForm.controls[key].valid == false) {
          this.totalProductionOrderForm.controls[key].markAsTouched();
        }
      });
    } else {
      this.loading = true;
      let totalProductionOrder = new TotalProductionOrder();

      let startDate = this.startDateControl.value;
      totalProductionOrder.startDate = convertToUTCTime(startDate);

      let listProductionOrderId: Array<string> = this.selectedProductionOrder.map(x => x.productionOrderId);

      this.manufactureService.createTotalProductionOrder(totalProductionOrder, listProductionOrderId).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.router.navigate(['/manufacture/total-production-order/detail', { totalProductionOrderId: result.totalProductionOrderId }]);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  cancel() {
    this.router.navigate(['/manufacture/total-production-order/list']);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
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

  goToDetail(id: string) {
    this.router.navigate(['/manufacture/production-order/detail', { productionOrderId: id }]);
  }

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
