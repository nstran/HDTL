import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ManufactureService } from '../../services/manufacture.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import * as $ from 'jquery';
import { DescriptionErrorDialogComponent } from '../../component/description-error-dialog/description-error-dialog.component';
import { ExportTrackProductionDialogComponent } from '../../component/export-track-production-dialog/export-track-production-dialog.component';
import { ViewRememberItemDialogComponent } from '../../component/view-remember-item-dialog/view-remember-item-dialog.component';
import { DialogService } from 'primeng';
//import { TIME_REFRESH } from '../../../../assets/config/setTimeRefresh';

class ProductionOrderHistory {
  productionOrderHistoryId: string;
  totalProductionOrderId: string;
  parentId: string;
  parentType: boolean;
  productionOrderId: string;
  productionOrderMappingId: string;
  techniqueRequestId: string;
  calculatorType: boolean;
  isError: boolean;
  description: string;
  rate: number;
  quantityUnitErr: number;
  isErrorPre: boolean;
  originalId: string;
  isParent: string;
  isSubParent: string;
  idChildrent: string;
  parentPartId: string;
  parentExtendId: string;
  isChildren: boolean;
  isAddPart: boolean;
  present: boolean;
  remainQuantity: number;
  createdDate: Date;
  createdById: string;

  constructor() {
    this.productionOrderHistoryId = '00000000-0000-0000-0000-000000000000';
    this.totalProductionOrderId = '00000000-0000-0000-0000-000000000000';
    this.parentId = null;
    this.parentType = null;
    this.productionOrderId = '00000000-0000-0000-0000-000000000000';
    this.productionOrderMappingId = '00000000-0000-0000-0000-000000000000';
    this.techniqueRequestId = '00000000-0000-0000-0000-000000000000';
    this.calculatorType = null;
    this.isError = null;
    this.isErrorPre = null;
    this.originalId = null;
    this.description = null;
    this.rate = null;
    this.quantityUnitErr = 1;
    this.isParent = null;
    this.isSubParent = null;
    this.idChildrent = null;
    this.parentPartId = null;
    this.isChildren = null;
    this.isAddPart = null;
    this.remainQuantity = 0;
    this.createdDate = new Date();
    this.createdById = '00000000-0000-0000-0000-000000000000';
  }
}

class RememberItem {
  rememberItemId: string;
  productionOrderId: string;
  productionOrderMappingId: string;
  quantity: number;
  description: string;
  isCheck: boolean;
  createdDate: Date;
  createdById: string;

  constructor() {
    this.rememberItemId = '00000000-0000-0000-0000-000000000000';
    this.productionOrderId = null;
    this.productionOrderMappingId = null;
    this.quantity = null;
    this.description = null;
    this.isCheck = false;
    this.createdDate = new Date();
    this.createdById = '00000000-0000-0000-0000-000000000000';
  }
}

class ProductionOrder {
  productionOrderId: string;
  productionOrderCode: string;
}

@Component({
  selector: 'app-track-production',
  templateUrl: './track-production.component.html',
  styleUrls: ['./track-production.component.css']
})
export class TrackProductionComponent implements OnInit {
  id_page: any;
  loading: boolean = false;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;

  cols: any;
  selectedColumns: any[];

  @ViewChild('myTable') myTable: Table;

  organizationCode: string = null;
  organizationName: string = null;
  code: string = null;
  currentTime: string = null;
  listTrackProduction: Array<any> = [];

  listProductLengthValue: Array<any> = [];
  listProductWidthValue: Array<any> = [];

  displayViewDialog: boolean = false;
  listTechniqueRequest: Array<any> = [];

  endDate: Date = null;
  productThickness: string = null;
  productName: string = null;
  selectedProductLengthValue: any = null;
  selectedProductWidthValue: any = null;
  fromDate: Date = null; //this.findFromDate();
  toDate: Date = new Date();
  listStatusItem: Array<any> = [];
  selectedStatus: Array<any> = [];
  listProductionOrder: Array<ProductionOrder> = [];
  selectedProductionOrder: Array<ProductionOrder> = [];

  isDisplayExportButton: boolean = false;

  totalRecords: number = 0;
  firstNumber: number = 0;
  rows: number = 10;

  listCheckbox: Array<any> = [];
  awaitResponse: boolean = false;
  checkAll: boolean = false;

  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private manufactureService: ManufactureService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) { }

  async ngOnInit() {
    let resource = "man/manufacture/track-production/track";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
    }

    $("body").addClass("sidebar-collapse");
    this.initTable();

    this.loading = true;
    this.manufactureService.getMasterDataTrackProduction().subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.organizationCode = result.organizationCode;
        this.organizationName = result.organizationName;
        this.code = result.code;
        this.currentTime = result.currentTime;
        //this.listProductionOrder = result.listProductionOrder;
        this.listStatusItem = result.listStatusItem;

        //Lấy giá trị mặc định của list trạng thái lsx
        this.selectedStatus = this.listStatusItem.filter(x => x.categoryCode == "NEW" || x.categoryCode == "PROC" || x.categoryCode == "COMP");

        this.getListTrackProduction(true, this.firstNumber, this.rows);

        this.id_page = setInterval(() => {
          if (this.loading == false && this.listCheckbox.length == 0) {
            this.getListTrackProduction(true, this.firstNumber, this.rows);
          }
        }, 180000);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  initTable() {
    this.cols = [
      { field: 'checkbox', header: '', width: '12px', textAlign: 'center', display: 'table-cell' },
      { field: 'endDate', header: 'Ngày GH', width: '28px', textAlign: 'center', display: 'table-cell' },
      { field: 'codeShow', header: 'Lệnh số', width: '25px', textAlign: 'center', display: 'table-cell' },
      { field: 'productName', header: 'Chủng loại', width: '38px', textAlign: 'left', display: 'table-cell' },
      { field: 'productColor', header: 'Màu sắc', width: '25px', textAlign: 'center', display: 'table-cell' },
      { field: 'productThickness', header: 'Độ dày', width: '23px', textAlign: 'center', display: 'table-cell' },
      { field: 'productLength', header: 'Chiều dài', width: '25px', textAlign: 'center', display: 'table-cell' },
      { field: 'productWidth', header: 'Chiều rộng', width: '25px', textAlign: 'center', display: 'table-cell' },
      { field: 'borehole', header: 'Khoan', width: '23px', textAlign: 'center', display: 'table-cell' },
      { field: 'hole', header: 'Khoét', width: '23px', textAlign: 'center', display: 'table-cell' },
      { field: 'preCompleteQuantity', header: 'HTCĐT', width: '25px', textAlign: 'center', display: 'table-cell' },
      { field: 'techniqueDescription', header: 'Mã hiệu', width: '30px', textAlign: 'left', display: 'table-cell' },
      { field: 'statusItemName', header: 'Trạng thái', width: '31px', textAlign: 'left', display: 'table-cell' },
      { field: 'allNumber', header: 'Số tổng hợp', width: '31px', textAlign: 'center', display: 'table-cell' },
      { field: 'to', header: 'Thao tác', width: '40px', textAlign: 'center', display: 'table-cell' }
    ];
    this.selectedColumns = this.cols;
  }

  paginate(event: any) {
    //event.first = Index of the first record
    //event.rows = Number of rows to display in new page
    //event.page = Index of the new page
    //event.pageCount = Total number of pages
    this.firstNumber = event.first;
    this.getListTrackProduction(true, this.firstNumber, this.rows);
  }

  chekedItem(event: boolean, data: any) {
    if (event) {
      let hasValue = this.listCheckbox.find(x => x.productionOrderMappingId == data.productionOrderMappingId);
      if (!hasValue) this.listCheckbox = [...this.listCheckbox, data];
    } else {
      this.listCheckbox = this.listCheckbox.filter(x => x.productionOrderMappingId != data.productionOrderMappingId);
    }

    let total = this.listTrackProduction.length;
    let totalCheck = this.listTrackProduction.filter(x => x.checked == true).length;
    let totalUnCheck = this.listTrackProduction.filter(x => x.checked == false).length;

    if (total != 0 && total == totalCheck) {
      this.checkAll = true;
    }
    else if (total != 0 && total == totalUnCheck) {
      this.checkAll = false;
    }
  }

  /*Event: Nhấn nút Tìm kiếm*/
  search(first: boolean, firstNumber: number, rows: number) {
    this.listCheckbox = [];
    this.getListTrackProduction(first, firstNumber, rows);
  }

  getListTrackProduction(first: boolean, firstNumber: number, rows: number) {
    if (first) {
      this.loading = true;
    }

    let listProductionOrderId = this.selectedProductionOrder.map(x => x.productionOrderId);

    let endDate = null;
    if (this.endDate) {
      endDate = this.endDate;
      endDate.setHours(23);
      endDate.setMinutes(59);
      endDate.setSeconds(59);
      endDate = convertToUTCTime(endDate);
    }

    let productThickness = null;
    if (this.productThickness == null || this.productThickness == '') {
      productThickness = null;
    } else {
      productThickness = ParseStringToFloat(this.productThickness);
    }

    let productName = this.productName;
    if (productName) {
      productName = productName.trim();
    }

    let fromDate = null;
    if (this.fromDate) {
      fromDate = this.fromDate;
      fromDate.setHours(0);
      fromDate.setMinutes(0);
      fromDate.setSeconds(0);
      fromDate = convertToUTCTime(fromDate);
    }

    let toDate = null;
    if (this.toDate) {
      toDate = this.toDate;
      toDate.setHours(23);
      toDate.setMinutes(59);
      toDate.setSeconds(59);
      toDate = convertToUTCTime(toDate);
    }

    let listStatusItem = this.selectedStatus.map(x => x.categoryId);

    let productLength = null;
    if (this.selectedProductLengthValue) {
      productLength = this.selectedProductLengthValue.value;
    }

    let productWidth = null;
    if (this.selectedProductWidthValue) {
      productWidth = this.selectedProductWidthValue.value;
    }

    this.manufactureService.getTrackProduction(listProductionOrderId, endDate, productThickness, productName,
      fromDate, toDate, listStatusItem, productLength, productWidth, firstNumber, rows)
      .subscribe(response => {
        let result: any = response;
        this.loading = false;
        this.awaitResponse = false;
        if (result.statusCode == 200) {
          this.totalRecords = result.totalRecords;
          this.mapData(result);
          //hiện button export excel cho tổ dán và tổ hộp
          if (this.organizationCode == "TDAN" || this.organizationCode == "THOP") this.isDisplayExportButton = true;
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
  }

  mapData(result: any) {
    this.listTrackProduction = result.listTrackProduction;
    this.listProductLengthValue = [];
    this.listProductWidthValue = [];
    this.selectedProductLengthValue = null;
    this.selectedProductWidthValue = null;
    this.listProductionOrder = [];

    this.listTrackProduction.forEach(item => {
      item.allNumber = item.quantity + ' - ' + item.completeQuantity + '/' + item.unitQuantity + ' - ' + item.actionQuantity;

      let length = {
        label: item.productLength.toString(),
        value: item.productLength
      }
      if (!this.listProductLengthValue.find(x => x.value == item.productLength)) {
        this.listProductLengthValue.push(length);
      }

      let width = {
        label: item.productWidth.toString(),
        value: item.productWidth
      }

      if (!this.listProductWidthValue.find(x => x.value == item.productWidth)) {
        this.listProductWidthValue.push(width);
      }

      if (item.code.trim().length > 10) {
        item.codeShow = item.code.substring(0, 8) + '...';
      } else {
        item.codeShow = item.code;
      }

      item.checked = false;

      //Hiển thị lại các item đã cheked
      let hasChecked = this.listCheckbox.find(x => x.productionOrderMappingId == item.productionOrderMappingId);
      if (hasChecked) {
        item.checked = true;
      }

      //Kiểm tra để disable button cộng
      item.isDisable = false;

      if (item.type == 1) {
        let preCompleteQuantity = Math.min(...item.listRateChild);
        if (item.actionQuantity >= item.unitQuantity || item.completeQuantity >= preCompleteQuantity) {
          item.isDisable = true;
        }
      } else if (item.type == 0) {
        if (
          (item.actionQuantity < item.unitQuantity && item.preCompleteQuantity >= item.actionQuantity) ||
          (item.actionQuantity < item.unitQuantity && item.preCompleteQuantity == null)
        ) {
          if (item.preCompleteQuantity == null ||
            item.completeQuantity < (Math.floor((item.preCompleteQuantity) / item.rate - item.totalErrPre))
          ) {
            if (item.statusCode == "CANC") {
              item.isDisable = true;
            }
          } else {
            item.isDisable = true;
          }
        } else {
          item.isDisable = true;
        }
      }
    });

    // var unique = [...Array.from(new Set(this.listProductionOrder.map(p => p.productionOrderId)))];

    // let newList: any = [];
    // this.listProductionOrder.forEach((_item, _index) => {
    //   if (unique.includes(_item.productionOrderId)) {
    //     newList.push(_item);
    //     unique = unique.filter(x => x != _item.productionOrderId);
    //   }
    // });

    // this.listProductionOrder = [...newList];

    let total = this.listTrackProduction.length;
    let totalCheck = this.listTrackProduction.filter(x => x.checked == true).length;
    if (total != 0 && total == totalCheck) {
      this.checkAll = true;
    }
    else {
      this.checkAll = false;
    }

    this.listProductionOrder = result.listProductionOrder;

    if (this.listProductionOrder.length == 0) {
      this.selectedProductionOrder = [];
    }
  }

  setCurrentPage(n: number) {
    let paging = {
      first: ((n - 1) * this.myTable.rows),
      rows: this.myTable.rows
    };
    this.myTable.onPageChange(paging);
  }

  refreshFilter() {
    this.endDate = null;
    this.selectedProductionOrder = [];
    this.productThickness = null;
    this.productName = null;
    this.fromDate = null; //this.findFromDate();
    this.toDate = new Date();
    this.selectedStatus = this.listStatusItem.filter(x => x.categoryCode == "NEW" || x.categoryCode == "PROC" || x.categoryCode == "COMP");
    this.listProductLengthValue = [];
    this.listProductWidthValue = [];
    this.selectedProductLengthValue = null;
    this.selectedProductWidthValue = null;

    this.firstNumber = 0;
    this.listCheckbox = [];
    this.getListTrackProduction(true, this.firstNumber, this.rows);
  }

  openViewDialog(data: any) {
    if (!this.displayViewDialog) {
      this.listTechniqueRequest = data.listTechniqueRequest;
      this.displayViewDialog = true;
    }
  }

  congItem(data: any) {
    this.awaitResponse = true;
    this.listCheckbox = [];
    this.selectedProductLengthValue = null;
    this.selectedProductWidthValue = null;

    if (data.type == 1) {
      //Nếu là cộng cho Item cha

      let preCompleteQuantity = Math.min(...data.listRateChild);
      if (data.actionQuantity < data.unitQuantity && data.completeQuantity < preCompleteQuantity) {
        let productionOrderHistory = new ProductionOrderHistory();
        productionOrderHistory.totalProductionOrderId = data.totalProductionOrderId;
        productionOrderHistory.parentId = data.parentId;
        productionOrderHistory.parentType = data.parentType;
        productionOrderHistory.productionOrderId = data.productionOrderId;
        productionOrderHistory.productionOrderMappingId = data.productionOrderMappingId;
        productionOrderHistory.techniqueRequestId = data.techniqueRequestId;
        productionOrderHistory.calculatorType = true;
        productionOrderHistory.isError = null;
        productionOrderHistory.originalId = data.originalId;
        productionOrderHistory.description = null;
        productionOrderHistory.quantityUnitErr = null;
        productionOrderHistory.isParent = data.isParent;
        productionOrderHistory.isSubParent = data.isSubParent;
        productionOrderHistory.idChildrent = data.idChildrent;
        productionOrderHistory.parentPartId = data.parentPartId;
        productionOrderHistory.parentExtendId = data.parentExtendId;
        productionOrderHistory.isChildren = data.isChildren;
        productionOrderHistory.isAddPart = data.isAddPart;
        productionOrderHistory.present = data.present;

        this.loading = true;
        this.manufactureService.plusItem(productionOrderHistory).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            this.getListTrackProduction(false, this.firstNumber, this.rows);
          } else {
            this.loading = false;
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      } else {
        this.getListTrackProduction(false, this.firstNumber, this.rows);
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Đã vượt quá số lượng phải hoàn thành' };
        this.showMessage(msg);
      }
    } else if (data.type == 0) {
      //Nếu là cộng cho Item không phải Item cha
      if ((data.actionQuantity < data.unitQuantity && data.preCompleteQuantity >= data.actionQuantity) ||
        (data.actionQuantity < data.unitQuantity && data.preCompleteQuantity == null)) {
        if (data.preCompleteQuantity == null ||
          data.completeQuantity < (Math.floor((data.preCompleteQuantity) / data.rate - data.totalErrPre))) {
          let productionOrderHistory = new ProductionOrderHistory();
          productionOrderHistory.totalProductionOrderId = data.totalProductionOrderId;
          productionOrderHistory.parentId = data.parentId;
          productionOrderHistory.parentType = data.parentType;
          productionOrderHistory.productionOrderId = data.productionOrderId;
          productionOrderHistory.productionOrderMappingId = data.productionOrderMappingId;
          productionOrderHistory.techniqueRequestId = data.techniqueRequestId;
          productionOrderHistory.calculatorType = true;
          productionOrderHistory.isError = null;
          productionOrderHistory.originalId = data.originalId;
          productionOrderHistory.description = null;
          productionOrderHistory.quantityUnitErr = null;
          productionOrderHistory.isParent = data.isParent;
          productionOrderHistory.isSubParent = data.isSubParent;
          productionOrderHistory.idChildrent = data.idChildrent;
          productionOrderHistory.parentPartId = data.parentPartId;
          productionOrderHistory.parentExtendId = data.parentExtendId;
          productionOrderHistory.isChildren = data.isChildren;
          productionOrderHistory.isAddPart = data.isAddPart;
          productionOrderHistory.present = data.present;

          this.loading = true;
          this.manufactureService.plusItem(productionOrderHistory).subscribe(response => {
            let result: any = response;
            this.loading = false;

            if (result.statusCode == 200) {
              this.getListTrackProduction(false, this.firstNumber, this.rows);
            } else {
              this.loading = false;
              let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
          });
        } else {
          this.getListTrackProduction(false, this.firstNumber, this.rows);
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Đã vượt quá số lượng phải hoàn thành' };
          this.showMessage(msg);
        }
      } else {
        this.getListTrackProduction(false, this.firstNumber, this.rows);
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Đã vượt quá số lượng phải hoàn thành' };
        this.showMessage(msg);
      }
    }
  }

  truItem(data: any) {
    if (data.completeQuantity == 0) {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không thể thao tác' };
      this.showMessage(msg);
    } else {
      this.listCheckbox = [];
      this.selectedProductLengthValue = null;
      this.selectedProductWidthValue = null;

      if (data.type == 1) {
        //Item cha

        let preCompleteQuantity = Math.min(...data.listRateChild);
        if (preCompleteQuantity > 0) {
          let ref = this.dialogService.open(DescriptionErrorDialogComponent, {
            data: {
              productionOrderMappingId: data.productionOrderMappingId,
              rate: data.rate,
              techniqueOrder: data.techniqueOrder,
              productName: data.productName,
              productLength: data.productLength,
              productWidth: data.productWidth,
              productThickness: data.productThickness,
              type: data.type,
              organizationCode: this.organizationCode
            },
            header: 'Chi tiết lỗi',
            width: '50%',
            baseZIndex: 1030,
            contentStyle: {
              "min-height": "240px",
              "max-height": "600px",
              "overflow": "auto"
            }
          });

          ref.onClose.subscribe((result: any) => {
            if (result) {
              let productionOrderHistory = new ProductionOrderHistory();
              productionOrderHistory.totalProductionOrderId = data.totalProductionOrderId;
              productionOrderHistory.parentId = data.parentId;
              productionOrderHistory.parentType = data.parentType;
              productionOrderHistory.productionOrderId = data.productionOrderId;
              productionOrderHistory.productionOrderMappingId = data.productionOrderMappingId;
              productionOrderHistory.techniqueRequestId = data.techniqueRequestId;
              productionOrderHistory.calculatorType = false;
              productionOrderHistory.isError = result.isError;
              productionOrderHistory.originalId = null;
              productionOrderHistory.description = result.description;
              productionOrderHistory.quantityUnitErr = result.quantityUnitErr;
              productionOrderHistory.isErrorPre = result.isErrorPre;
              productionOrderHistory.isParent = data.isParent;
              productionOrderHistory.isSubParent = data.isSubParent;
              productionOrderHistory.idChildrent = result.idChildrent;
              productionOrderHistory.parentPartId = data.parentPartId;
              productionOrderHistory.parentExtendId = data.parentExtendId;
              productionOrderHistory.isChildren = result.isChildren;
              productionOrderHistory.isAddPart = data.isAddPart;
              productionOrderHistory.present = data.present;

              this.loading = true;
              this.manufactureService.minusItem(productionOrderHistory, result.listItemId).subscribe(response => {
                let result: any = response;

                if (result.statusCode == 200) {
                  this.getListTrackProduction(false, this.firstNumber, this.rows);
                } else {
                  this.getListTrackProduction(false, this.firstNumber, this.rows);
                  let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
                  this.showMessage(msg);
                }
              });
            }
          });
        } else {
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không thể thao tác' };
          this.showMessage(msg);
        }
      } else {
        let ref = this.dialogService.open(DescriptionErrorDialogComponent, {
          data: {
            productionOrderMappingId: data.productionOrderMappingId,
            rate: data.rate,
            techniqueOrder: data.techniqueOrder,
            productName: data.productName,
            productLength: data.productLength,
            productWidth: data.productWidth,
            productThickness: data.productThickness,
            type: data.type,
            organizationCode: this.organizationCode
          },
          header: 'Chi tiết lỗi',
          width: '50%',
          baseZIndex: 1030,
          contentStyle: {
            "min-height": "240px",
            "max-height": "600px",
            "overflow": "auto"
          }
        });

        ref.onClose.subscribe((result: any) => {
          if (result) {
            let productionOrderHistory = new ProductionOrderHistory();
            productionOrderHistory.totalProductionOrderId = data.totalProductionOrderId;
            productionOrderHistory.parentId = data.parentId;
            productionOrderHistory.parentType = data.parentType;
            productionOrderHistory.productionOrderId = data.productionOrderId;
            productionOrderHistory.productionOrderMappingId = data.productionOrderMappingId;
            productionOrderHistory.techniqueRequestId = data.techniqueRequestId;
            productionOrderHistory.calculatorType = false;
            productionOrderHistory.isError = result.isError;
            productionOrderHistory.originalId = null;
            productionOrderHistory.description = result.description;
            productionOrderHistory.quantityUnitErr = result.quantityUnitErr;
            productionOrderHistory.isErrorPre = result.isErrorPre;
            productionOrderHistory.isParent = data.isParent;
            productionOrderHistory.isSubParent = data.isSubParent;
            productionOrderHistory.idChildrent = data.idChildrent;
            productionOrderHistory.parentPartId = data.parentPartId;
            productionOrderHistory.parentExtendId = data.parentExtendId;
            productionOrderHistory.isChildren = result.isChildren;
            productionOrderHistory.isAddPart = data.isAddPart;
            productionOrderHistory.present = data.present;

            this.loading = true;
            this.manufactureService.minusItem(productionOrderHistory, result.listItemId).subscribe(response => {
              let result: any = response;

              if (result.statusCode == 200) {
                this.getListTrackProduction(false, this.firstNumber, this.rows);
              } else {
                this.getListTrackProduction(false, this.firstNumber, this.rows);
                let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
                this.showMessage(msg);
              }
            });
          }
        });
      }
    }
  }

  logItem(data: any) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn chọn lỗi khác?',
      accept: () => {
        let rememberItem = new RememberItem();
        rememberItem.productionOrderId = data.productionOrderId;
        rememberItem.productionOrderMappingId = data.productionOrderMappingId;

        this.manufactureService.createRememberItem(rememberItem).subscribe(response => {
          let result: any = response;

          if (result.statusCode == 200) {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Lưu thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }

  cong_tat_ca() {
    this.selectedProductLengthValue = null;
    this.selectedProductWidthValue = null;
    let selectedItem = this.listCheckbox.filter(x => x.isDisable == false);

    //Chỉ lấy những Item nào có thể cộng được
    if (selectedItem.length > 0) {
      let listItem: Array<ProductionOrderHistory> = [];

      selectedItem.forEach(item => {
        if (item.type == 1) {
          //Item là item cha và ở tiến trình đầu tiên
          let preCompleteQuantity = Math.min(...item.listRateChild);

          //Trường hợp item là item gốc và không có bán thành phẩm (chỉ Dán)
          if (item.listRateChild.length == 0) {
            preCompleteQuantity = item.unitQuantity;
          }

          let productionOrderHistory = new ProductionOrderHistory();
          productionOrderHistory.totalProductionOrderId = item.totalProductionOrderId;
          productionOrderHistory.parentId = item.parentId;
          productionOrderHistory.parentType = item.parentType;
          productionOrderHistory.productionOrderId = item.productionOrderId;
          productionOrderHistory.productionOrderMappingId = item.productionOrderMappingId;
          productionOrderHistory.techniqueRequestId = item.techniqueRequestId;
          productionOrderHistory.calculatorType = true;
          productionOrderHistory.isError = null;
          productionOrderHistory.originalId = item.originalId;
          productionOrderHistory.description = null;
          productionOrderHistory.quantityUnitErr = null;
          productionOrderHistory.isParent = item.isParent;
          productionOrderHistory.isSubParent = item.isSubParent;
          productionOrderHistory.idChildrent = item.idChildrent;
          productionOrderHistory.parentPartId = item.parentPartId;
          productionOrderHistory.parentExtendId = item.parentExtendId;
          productionOrderHistory.isChildren = item.isChildren;
          productionOrderHistory.isAddPart = item.isAddPart;
          productionOrderHistory.present = item.present;
          productionOrderHistory.remainQuantity = preCompleteQuantity - item.completeQuantity;

          listItem.push(productionOrderHistory);
        } else if (item.type == 0) {
          //Item không phải item cha và hoặc là item cha nhưng không ở tiến trình đầu tiên

          if (item.preCompleteQuantity == null) {
            //Trường hợp là tiến trình đầu tiên

            let productionOrderHistory = new ProductionOrderHistory();
            productionOrderHistory.totalProductionOrderId = item.totalProductionOrderId;
            productionOrderHistory.parentId = item.parentId;
            productionOrderHistory.parentType = item.parentType;
            productionOrderHistory.productionOrderId = item.productionOrderId;
            productionOrderHistory.productionOrderMappingId = item.productionOrderMappingId;
            productionOrderHistory.techniqueRequestId = item.techniqueRequestId;
            productionOrderHistory.calculatorType = true;
            productionOrderHistory.isError = null;
            productionOrderHistory.originalId = item.originalId;
            productionOrderHistory.description = null;
            productionOrderHistory.quantityUnitErr = null;
            productionOrderHistory.isParent = item.isParent;
            productionOrderHistory.isSubParent = item.isSubParent;
            productionOrderHistory.idChildrent = item.idChildrent;
            productionOrderHistory.parentPartId = item.parentPartId;
            productionOrderHistory.parentExtendId = item.parentExtendId;
            productionOrderHistory.isChildren = item.isChildren;
            productionOrderHistory.isAddPart = item.isAddPart;
            productionOrderHistory.present = item.present;
            productionOrderHistory.remainQuantity = item.unitQuantity - item.actionQuantity;

            listItem.push(productionOrderHistory);
          } else {
            //Trường hợp không phải tiến trình đầu tiên

            let productionOrderHistory = new ProductionOrderHistory();
            productionOrderHistory.totalProductionOrderId = item.totalProductionOrderId;
            productionOrderHistory.parentId = item.parentId;
            productionOrderHistory.parentType = item.parentType;
            productionOrderHistory.productionOrderId = item.productionOrderId;
            productionOrderHistory.productionOrderMappingId = item.productionOrderMappingId;
            productionOrderHistory.techniqueRequestId = item.techniqueRequestId;
            productionOrderHistory.calculatorType = true;
            productionOrderHistory.isError = null;
            productionOrderHistory.originalId = item.originalId;
            productionOrderHistory.description = null;
            productionOrderHistory.quantityUnitErr = null;
            productionOrderHistory.isParent = item.isParent;
            productionOrderHistory.isSubParent = item.isSubParent;
            productionOrderHistory.idChildrent = item.idChildrent;
            productionOrderHistory.parentPartId = item.parentPartId;
            productionOrderHistory.parentExtendId = item.parentExtendId;
            productionOrderHistory.isChildren = item.isChildren;
            productionOrderHistory.isAddPart = item.isAddPart;
            productionOrderHistory.present = item.present;
            productionOrderHistory.remainQuantity = item.preCompleteQuantity - item.completeQuantity;

            listItem.push(productionOrderHistory);
          }
        }
      });

      this.loading = true;
      this.awaitResponse = true;
      this.listCheckbox = [];
      this.manufactureService.plusListItem(listItem).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.awaitResponse = false;
          this.getListTrackProduction(false, this.firstNumber, this.rows);
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Lưu thành công' };
          this.showMessage(msg);
        } else {
          this.awaitResponse = false;
          this.getListTrackProduction(false, this.firstNumber, this.rows);
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    } else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Phải có ít nhất một sản phẩm hợp lệ' };
      this.showMessage(msg);
    }
  }

  toggleCheckAll(event: boolean) {
    //Nếu chọn tất cả
    if (event) {
      let listCheck: Array<any> = [];
      this.listTrackProduction.forEach(item => {
        item.checked = true;

        let hasValue = this.listCheckbox.find(x => x.productionOrderMappingId == item.productionOrderMappingId);
        if (!hasValue) {
          listCheck.push(item);
        }
      });

      this.listCheckbox = [...this.listCheckbox, ...listCheck];
    }
    //Nếu bỏ chọn tất cả
    else {
      let listUnCheck: Array<any> = [];
      this.listTrackProduction.forEach(item => {
        item.checked = false;

        listUnCheck.push(item);
      });
      listUnCheck.forEach(item => {
        this.listCheckbox = this.listCheckbox.filter(x => x.productionOrderMappingId != item.productionOrderMappingId);
      });
    }
  }

  bao_het_kinh(data: any) {
    this.selectedProductLengthValue = null;
    this.selectedProductWidthValue = null;
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn báo hết kính cắt hạ?',
      accept: () => {
        //Kiểm tra điều kiện để được báo hết kính cắt hạ

        if ((data.quantity == 1) ||
          (data.quantity - data.actionQuantity == data.quantity)) {
          //Trường hợp 1: item có số tấm phải làm là 1
          //Trường hợp 2: item chưa làm tấm nào
          //==> Sửa quy trình từ Dán --> Cắt - Dán
          this.loading = true;
          this.manufactureService.minusQuantityForItem(data.productionOrderMappingId, 2, data.quantity).subscribe(response => {
            let result: any = response;
            this.loading = false;

            if (result.statusCode == 200) {
              this.getListTrackProduction(false, this.firstNumber, this.rows);
              let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Lưu thành công' };
              this.showMessage(msg);
            } else {
              this.getListTrackProduction(false, this.firstNumber, this.rows);
              let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
          });
        } else if (data.quantity - data.actionQuantity >= 1 && data.actionQuantity != 0) {
          //==> Trừ số tấm phải làm của Item và Tạo Item mới với quy trình Cắt - Dán
          this.loading = true;
          this.manufactureService.minusQuantityForItem(data.productionOrderMappingId, 1, (data.quantity - data.actionQuantity)).subscribe(response => {
            let result: any = response;
            this.loading = false;

            if (result.statusCode == 200) {
              this.getListTrackProduction(false, this.firstNumber, this.rows);
              let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Lưu thành công' };
              this.showMessage(msg);
            } else {
              this.getListTrackProduction(false, this.firstNumber, this.rows);
              let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
          });
        } else {
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Sản phẩm này không hợp lệ' };
          this.showMessage(msg);
        }
      }
    });
  }

  exportExcel() {
    let ref = this.dialogService.open(ExportTrackProductionDialogComponent, {
      data: {
        organizationCode: this.organizationCode
      },
      header: 'Xuất Excel',
      width: '40%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "350px",
        "max-height": "700px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
      }
    });
  }

  viewRememberItemDialog() {
    let ref = this.dialogService.open(ViewRememberItemDialogComponent, {
      data: {
      },
      header: 'Xem lỗi khác',
      width: '75%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "450px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });
  }

  findFromDate() {
    //Từ ngày sẽ là từ 3 ngày trước đến thời điểm hiện tại
    let milisecon = 2 * 24 * 60 * 60 * 1000;
    let nowDate = new Date();
    let miliseconFromDate = nowDate.getTime() - milisecon;
    let fromDate = new Date(miliseconFromDate);

    return fromDate;
  }

  goToDetail(id: string) {
    let url = '/manufacture/production-order/detail' + ';productionOrderId=' + id;
    window.open(url, "_blank");
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  ngOnDestroy() {
    $("body").removeClass("sidebar-collapse");
    if (this.id_page) {
      clearInterval(this.id_page);
    }
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
