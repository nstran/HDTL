import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { ManufactureService } from '../../services/manufacture.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';

import { AddProductionOrderDialogComponent } from '../../component/add-production-order-dialog/add-production-order-dialog.component';
import { ViewListItemDialogComponent } from '../../component/view-list-item-dialog/view-list-item-dialog.component';
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
  isDelete: boolean;
  parentId: string;
  isIgnore: boolean;
}

@Component({
  selector: 'app-total-production-order-detail',
  templateUrl: './total-production-order-detail.component.html',
  styleUrls: ['./total-production-order-detail.component.css']
})
export class TotalProductionOrderDetailComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(sessionStorage.getItem("auth"));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionEdit: boolean = true;

  totalProductionOrderId: string = null;
  totalProductionOrder: TotalProductionOrder = new TotalProductionOrder();
  listStatusItem: Array<Category> = [];

  /* FORM */
  totalProductionOrderForm: FormGroup;
  startDateControl: FormControl;
  /* END FORM */

  cols: Array<any> = [];
  listProductionOrder: Array<ProductionOrder> = [];

  totalQuantity: number = 0;
  totalArea: number = 0;
  minEndDate: Date = null;
  maxEndDate: Date = null;

  disabled_status: boolean = true;
  disabled_startDate: boolean = true;

  oldStatusCodeFe: string = null;

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
    let resource = "man/manufacture/total-production-order/detail";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
    }

    this.route.params.subscribe(params => {
      this.totalProductionOrderId = params['totalProductionOrderId'];
    });

    //this.setForm();
    this.initTable();
    this.getMasterData(true);
  }

  setForm() {
    this.startDateControl = new FormControl(null, [Validators.required]);

    this.totalProductionOrderForm = new FormGroup({
      startDateControl: this.startDateControl
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
      { field: 'noteTechnique', header: 'Ghi chú KT', textAlign: 'left', display: 'table-cell' },
      { field: 'view', header: 'Chi tiết', textAlign: 'left', display: 'table-cell' },
      { field: 'delete', header: 'Xóa', textAlign: 'center', display: 'table-cell' }
    ];
  }

  getMasterData(first: boolean) {
    if (first) {
      this.loading = true;
    }

    this.manufactureService.getTotalProductionOrderById(this.totalProductionOrderId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.totalProductionOrder = result.totalProductionOrder;

        let startDate = this.totalProductionOrder.startDate == null ? null : new Date(this.totalProductionOrder.startDate);
        if (startDate) {
          this.startDateControl.setValue(startDate);
        }

        this.listStatusItem = result.listStatusItem;

        this.listProductionOrder = result.listProductionOrder;
        this.listProductionOrder.forEach(item => {
          if (item.endDate) {
            item.endDate = new Date(item.endDate);
          }

          let statusItemCode = this.listStatusItem.find(x => x.categoryId == item.statusId).categoryCode;
          //Nếu lệnh sx ở trạng thái Mới tạo mới đc xóa lệnh sx
          if (statusItemCode == "NEW" && item.parentId == null) {
            item.isDelete = true;
          } else {
            item.isDelete = false;
          }
        });

        this.statisticTotal();
        // this.checkStatus();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /* Thống kê lệnh sản xuất */
  statisticTotal() {
    let tmp_quantity = 0;
    this.listProductionOrder.forEach(item => {
      if (item.isIgnore == false) {
        tmp_quantity += item.remainQuantity;
      }
    });
    this.totalQuantity = this.roundNumber(tmp_quantity, 2);

    let tmp_area = 0;
    this.listProductionOrder.forEach(item => {
      if (item.isIgnore == false) {
        tmp_area += item.remainTotalArea;
      }
    });
    this.totalArea = this.roundNumber(tmp_area, 2);

    let listDate = this.listProductionOrder.map((x) => { if (x.endDate) return x.endDate });

    if (listDate.length > 0) {
      this.maxEndDate = new Date(Math.max.apply(null, listDate));
      this.minEndDate = new Date(Math.min.apply(null, listDate));
    } else {
      this.maxEndDate = null;
      this.minEndDate = null;
    }
  }

  /* Mở dialog thêm lệnh sản xuất */
  openAddProductionOrderDialog() {
    //Load lại dữ liệu để lấy những lệnh sản xuất đã được thêm vào lệnh tổng
    this.loading = true;
    this.manufactureService.getTotalProductionOrderById(this.totalProductionOrderId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.totalProductionOrder = result.totalProductionOrder;

        let startDate = this.totalProductionOrder.startDate == null ? null : new Date(this.totalProductionOrder.startDate);
        if (startDate) {
          this.startDateControl.setValue(startDate);
        }

        this.listStatusItem = result.listStatusItem;

        this.listProductionOrder = result.listProductionOrder;
        this.listProductionOrder.forEach(item => {
          if (item.endDate) {
            item.endDate = new Date(item.endDate);
          }

          let statusItemCode = this.listStatusItem.find(x => x.categoryId == item.statusId).categoryCode;
          //Nếu lệnh sx ở trạng thái Mới tạo mới đc xóa lệnh sx
          if (statusItemCode == "NEW" && item.parentId == null) {
            item.isDelete = true;
          } else {
            item.isDelete = false;
          }
        });

        this.statisticTotal();

        //Lọc data để đẩy vào dialog
        let listIgnore: Array<string> = this.listProductionOrder.map(x => x.productionOrderId);

        let ref = this.dialogService.open(AddProductionOrderDialogComponent, {
          data: {
            listIgnore: listIgnore
          },
          header: 'Thêm lệnh sản xuất',
          width: '70%',
          baseZIndex: 1030,
          contentStyle: {
            "min-height": "280px",
            "max-height": "600px",
            "overflow": "auto"
          }
        });

        ref.onClose.subscribe((result: any) => {
          if (result) {
            let listAddProductionOrder: Array<ProductionOrder> = result.listAddProductionOrder;
            if (listAddProductionOrder.length > 0) {
              listAddProductionOrder.forEach(item => {
                item.isDelete = true;
                if (item.endDate) {
                  item.endDate = new Date(item.endDate);
                }
                this.listProductionOrder = [...this.listProductionOrder, item];
              });

              this.statisticTotal();
            }
          }
        });
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  openViewItemDialog(data: ProductionOrder) {
    let ref = this.dialogService.open(ViewListItemDialogComponent, {
      data: {
        productionOrderId: data.productionOrderId
      },
      header: 'Lệnh sản xuất',
      width: '80%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });
  }

  /* Xóa lệnh sản xuất */
  deleteItem(data: ProductionOrder) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listProductionOrder = this.listProductionOrder.filter(x => x != data);
        this.listProductionOrder = [...this.listProductionOrder];

        this.statisticTotal();
      }
    });
  }

  updateTotalProductionOrder() {
    if (!this.totalProductionOrderForm.valid) {
      Object.keys(this.totalProductionOrderForm.controls).forEach(key => {
        if (this.totalProductionOrderForm.controls[key].valid == false) {
          this.totalProductionOrderForm.controls[key].markAsTouched();
        }
      });
    } else {
      let startDate = this.startDateControl.value;
      this.totalProductionOrder.startDate = convertToUTCTime(startDate);

      let listProductionOrderId: Array<string> = this.listProductionOrder.map(x => x.productionOrderId);

      this.loading = true;
      this.manufactureService.updateTotalProductionOrder(this.totalProductionOrder, listProductionOrderId, this.oldStatusCodeFe).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.getMasterData(false);
          let msg = { severity: 'success', summary: 'Thông báo:', detail: "Lưu thành công" };
          this.showMessage(msg);
        } else {
          this.loading = false;
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  cancel() {
    this.router.navigate(['/manufacture/total-production-order/list']);
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

  showMessage(msg: any) {
    this.messageService.add(msg);
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
