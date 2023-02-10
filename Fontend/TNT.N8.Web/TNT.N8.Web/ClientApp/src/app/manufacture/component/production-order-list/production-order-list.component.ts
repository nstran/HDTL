import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';

import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { SortEvent } from 'primeng/api';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { ManufactureService } from '../../services/manufacture.service';
import { DialogModule } from 'primeng/dialog';
import { TreeNode } from 'primeng/api';

class ProductionOrder {
  productionOrderId: string;
  productionOrderCode: string;
  orderId: string;
  customerName: string;
  oderCustomerCode: string;
  placeOfDelivery: string;
  receivedDate: Date;
  startDate: Date;
  endDate: Date;
  note: string;
  noteTechnique: string;
  especially: boolean;
  statusId: string;
  createdDate: Date;
  createdById: string;
  customerNumber: string;
  totalProductionOrderCode: Array<string>;
  statusCode: string;
  isError: boolean;
  parentId: string;
  numberProductionOrder: number; // Số lệnh bổ sung cho lệnh sản xuất
}

class TechniqueRequestMaping {
  productionOrderMappingId: string;
  index: string;
  parentId: string;
  productionOrderId: string;
  productId: string;
  productName: string;
  productColor: string;
  productColorCode: string;
  productThickness: number;
  productLength: number;
  productWidth: number;
  quantity: number;
  totalArea: number;
  techniqueDescription: string;
  statusId: string;
  createdDate: Date;
  createdById: string;
  productOrderWorkflowName: string;
  productCode: string;
  productOrderWorkflowId: string;
  parentIndex: string;
  borehole: number;
  hole: number;
  productGroupCode: string;
  description: string;// Mô tả lỗi
  nameNest: string;  // Tên tổ
  productionOrderHistoryId: string // Id bản lịch sử
  techniqueOrder: number // Order tiến trình lỗi
  listTechnique: Array<Technique>;
  originalId: string; // Tiến trình gây lỗi
  grind: number;  //Mài
  stt: number;  //Số thứ tự
  note: string;
}

class Technique {
  techniqueRequestId: string;
  parentId: string;
  organizationId: string;
  techniqueName: string;
  description: string;
  active: boolean;
  createdDate: Date;
  createdById: string;
  techniqueOrder: number;
  techniqueValue: any;
  parentName: string;
  rate: number;
}

class ItemInfor {
  productionOrderMappingId: string;
  productionOrderId: string;
  productName: string;
  productColor: string;
  productThickness: any;
  productLength: number;
  productWidth: number;
  quantity: number;
  totalArea: number;
  hole: number;
  borehole: number;
  techniqueDescription: string;
  statusId: string;
  statusCode: string;
  statusName: string;
  productOrderWorkflowId: string;
  productOrderWorkflowName: string;
  productGroupCode: string;
  parentPartId: string;
  isAddItem: boolean;
  isCreated: boolean;
  isOriginal: boolean;
}

interface StatusProductionOrder {
  categoryId: string;
  categoryName: string;
}

@Component({
  selector: 'app-production-order-list',
  templateUrl: './production-order-list.component.html',
  styleUrls: ['./production-order-list.component.css']
})

export class ProductionOrderListComponent implements OnInit {
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  cols: any[];
  colsListProductionOrder: any;
  colsListProduct: any;
  colsItem: any;
  listItemInfor: Array<ItemInfor> = [];
  dataTree: Array<TreeNode> = [
    // {
    //   "data": {
    //     "name":"Vũ Bích Ngọc",
    //     "size":"max",
    //     "type":"abc",
    //     "level": 0
    //   }
    // },
    // {
    //   "data": {
    //     "name":"Documents",
    //     "size":"75kb",
    //     "type":"Folder",
    //     "level": 0
    //   },
    //   "children": [
    //     {
    //       "data": {
    //         "name":"Work",
    //         "size":"55kb",
    //         "type":"Folder",
    //         "level": 1
    //       },
    //       "children": [
    //         {
    //           "data": {
    //             "name":"Expenses.doc",
    //             "size":"30kb",
    //             "type":"Document",
    //             "level": 2
    //           }
    //         },
    //         {
    //           "data": {
    //             "name":"Resume.doc",
    //             "size":"25kb",
    //             "type":"Resume",
    //             "level": 2
    //           }
    //         }
    //       ]
    //     },
    //     {
    //       "data": {
    //         "name":"Home",
    //         "size":"20kb",
    //         "type":"Folder",
    //         "level": 1
    //       },
    //       "children": [
    //         {
    //           "data": {
    //             "name":"Invoices",
    //             "size":"20kb",
    //             "type":"Text",
    //             "level": 2
    //           }
    //         }
    //       ]
    //     }
    //   ]
    // },
    // {
    //   "data":{
    //     "name":"Pictures",
    //     "size":"150kb",
    //     "type":"Folder",
    //     "level": 0
    //   },
    //   "children":[
    //     {
    //       "data":{
    //         "name":"barcelona.jpg",
    //         "size":"90kb",
    //         "type":"Picture",
    //         "level": 1
    //       }
    //     },
    //     {
    //       "data":{
    //         "name":"primeui.png",
    //         "size":"30kb",
    //         "type":"Picture",
    //         "level": 1
    //       }
    //     },
    //     {
    //       "data":{
    //         "name":"optimus.jpg",
    //         "size":"30kb",
    //         "type":"Picture",
    //         "level": 1
    //       }
    //     }
    //   ]
    // }
  ];
  selectedColumns: any[];
  listProductOrder: Array<ProductionOrder> = [];
  productionOrder: ProductionOrder[];
  listStatus: Array<StatusProductionOrder> = [];
  listSelectedStatus: Array<StatusProductionOrder> = [];
  productionOrderCode: string = "";
  totalProductionOrderCode: string = "";
  selectedProduct: Array<TechniqueRequestMaping> = [];
  customerName: string = "";
  // Ngày sản xuất
  fromDate: Date = null;
  // Ngày dự kiến trả
  toDate: Date = null;
  noteTechnique: string = "";
  listTypeProductionOrder: Array<any> = [];
  selectedTypeProductionOrder: any = null;

  listOrganization: Array<any> = [];
  listSelectedOrganization: Array<any> = [];
  isCreateProductionOrderAdd: boolean = true;

  // Filter
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  //number window size first
  innerWidth: number = 0;
  dateTimeNow: Date = new Date();
  nextDateNow: Date = new Date();
  // Hiện dialog
  display: boolean = false;
  displayChangeTech: boolean = false;

  loading: boolean = false;

  listProduct: Array<TechniqueRequestMaping> = [];
  // My table
  @ViewChild('myTable') myTable: Table;

  isError: boolean = false;
  id_page: any;

  awaitResponse: boolean = false;

  constructor(
    private manufactureService: ManufactureService,
    private messageService: MessageService,
    private router: Router,
    private getPermission: GetPermission,
  ) {
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    let resource = "man/manufacture/production-order/list";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
    }

    this.nextDateNow.setDate(new Date().getDate() + 1);
    this.nextDateNow.setHours(23, 59, 59, 999);
    this.dateTimeNow.setHours(0, 0, 0, 0);
    this.initTable();
    this.loading = true;
    this.getMasterData();
    this.getAll();

    this.id_page = setInterval(() => {
      if (this.loading == false) {
        this.getAll();
      }
    }, 60000);
  }

  initTable() {
    this.colsListProductionOrder = [
      { field: 'productionOrderCode', header: 'Lệnh số', textAlign: 'left', display: 'table-cell', width: '8%' },
      { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left', display: 'table-cell', width: '15%' },
      { field: 'totalProductionOrderCode', header: 'Lệnh tổng', textAlign: 'left', display: 'table-cell', width: '9%' },
      { field: 'startDate', header: 'Ngày SX', textAlign: 'left', display: 'table-cell', width: '8%' },
      { field: 'endDate', header: 'Ngày GH', textAlign: 'center', display: 'table-cell', width: '8%' },
      { field: 'statusName', header: 'Trạng thái SX', textAlign: 'right', display: 'table-cell', width: '9%' },
      { field: 'noteTechnique', header: 'Ghi chú KT', textAlign: 'left', display: 'table-cell', width: '9%' },
      { field: 'especially', header: 'Ưu tiên', textAlign: 'left', display: 'table-cell', width: '6%' },
      { field: 'numberProductionOrder', header: 'Số lệnh BS', textAlign: 'left', display: 'table-cell', width: '8%' },
      { field: 'note', header: 'Ghi chú', textAlign: 'left', display: 'table-cell', width: '10%' },
      { field: 'xxx', header: 'Lỗi', textAlign: 'left', display: 'table-cell', width: '5%' },
      // { field: 'isChangeTech', header: 'Đổi QT', textAlign: 'left', display: 'table-cell', width: '5%' },
    ];
    this.selectedColumns = this.colsListProductionOrder;

    this.colsListProduct = [
      { field: 'productName', header: 'Chủng Loại', textAlign: 'left', display: 'table-cell', width: '8%' },
      { field: 'productColor', header: 'Màu sắc', textAlign: 'left', display: 'table-cell', width: '6%' },
      { field: 'productThickness', header: 'Độ dày(mm)', textAlign: 'right', display: 'table-cell', width: '6%' },
      { field: 'productLength', header: 'Chiều dài(mm)', textAlign: 'right', display: 'table-cell', width: '6%' },
      { field: 'productWidth', header: 'Chiều rộng(mm)', textAlign: 'right', display: 'table-cell', width: '6%' },
      { field: 'quantity', header: 'Số tấm', textAlign: 'right', display: 'table-cell', width: '4%' },
      { field: 'totalArea', header: 'Tổng số(m2)', textAlign: 'right', display: 'table-cell', width: '6%' },
      { field: 'borehole', header: 'Khoan', textAlign: 'right', display: 'table-cell', width: '5%' },
      { field: 'hole', header: 'Khoét', textAlign: 'right', display: 'table-cell', width: '5%' },
      { field: 'techniqueDescription', header: 'Mã hiệu', textAlign: 'left', display: 'table-cell', width: '5%' },
      { field: 'nameNest', header: 'Tổ', textAlign: 'left', display: 'table-cell', width: '5%' },
      { field: 'description', header: 'Mô tả lỗi', display: 'table-cell', width: '8%' },
      { field: 'note', header: 'Ghi chú', display: 'table-cell', width: '8%' },
    ];

    this.colsItem = [
      { field: 'productName', header: 'Chủng Loại' },
      { field: 'productColor', header: 'Màu sắc' },
      { field: 'productThickness', header: 'Độ dày(mm)' },
      { field: 'productLength', header: 'Chiều dài(mm)' },
      { field: 'productWidth', header: 'Chiều rộng(mm)' },
      { field: 'quantity', header: 'Số tấm' },
      { field: 'totalArea', header: 'Tổng số(m2)' },
      { field: 'borehole', header: 'Khoan' },
      { field: 'hole', header: 'Khoét' },
      { field: 'techniqueDescription', header: 'Mã hiệu' },
      // { field: 'statusName', header: 'Trạng thái' },
      { field: 'productOrderWorkflowName', header: 'Quy trình' },
      { field: 'productGroupCode', header: 'Mã nhóm' },
    ];

    this.listTypeProductionOrder = [
      {
        name: 'Lệnh bổ sung',
        value: true,
      },
      {
        name: 'Lệnh sản xuất',
        value: false,
      }
    ]
  }

  getMasterData() {
    this.loading = true;
    this.manufactureService.getMasterDataListSearchProductionOrder().subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listStatus = result.listStatus;
        this.listOrganization = result.organizations;
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  createAll() {
    if (this.selectedProduct.length == 0) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Phải chọn ít nhất 1 sản phẩm lỗi để tạo lệnh bổ sung!" };
      this.showMessage(msg);
    } else {
      this.createProductionOrder(this.selectedProduct, 2);
    }
  }

  createProduction(data: TechniqueRequestMaping) {
    this.isCreateProductionOrderAdd = false;
    this.selectedProduct = [];
    this.selectedProduct.push(data)
    this.createProductionOrder(this.selectedProduct, 1);
  }

  createProductionOrder(data: Array<TechniqueRequestMaping>, add: number) {
    this.loading = true;
    this.awaitResponse = true;
    this.manufactureService.createProductionOrderAdditional(data).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: "Thêm thành công" };
        this.showMessage(msg);
        this.refreshFilter();
        if (add == 1) {
          data.forEach(item => {
            this.listProduct = this.listProduct.filter(x => x != item);
          });
          this.listProduct = [...this.listProduct];
          this.awaitResponse = false;
          if (this.listProduct.length == 0) {
            this.display = false;
          }
          this.selectedProduct = [];
        }
        else {
          this.display = false;
          this.awaitResponse = false;
        }
        this.isCreateProductionOrderAdd = true;
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  getAll() {
    let listStatus: string[];
    // Ngày sản xuất
    let fromDate = null;
    if (this.fromDate) {
      fromDate = convertToUTCTime(this.fromDate);
    }

    // Ngày dự kiến trả
    let toDate = null;
    if (this.toDate) {
      toDate = convertToUTCTime(this.toDate);
    }

    listStatus = this.listSelectedStatus.map(x => x.categoryId);
    let type: boolean = null;
    if (this.selectedTypeProductionOrder == null) {
      type = null;
    } else {
      type = this.selectedTypeProductionOrder.value;

    }
    let listOrgan: Array<string> = this.listSelectedOrganization.map(x => x.organizationId);

    this.listProductOrder = [];
    this.manufactureService.getAllProductionOrder(this.productionOrderCode.trim(), this.customerName.trim(), this.totalProductionOrderCode.trim(),
      fromDate, toDate, listStatus, this.noteTechnique, type, listOrgan, this.isError).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          result.listProductionOrder.forEach(element => {
            let productOrder = new ProductionOrder();
            productOrder = element;
            productOrder.endDate = element.endDate == null ? null : new Date(element.endDate);
            this.listProductOrder.push(productOrder);
          });
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
  }

  showDialog(productionOrderId: string) {
    this.selectedProduct = [];
    this.loading = true;
    this.manufactureService.getMasterDataDialogListProductOrder(productionOrderId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listProduct = result.listProduct;
        var index = 0;
        this.listProduct.forEach(item => {
          item.index = (index + 1).toString();
          index++;
          item.totalArea = this.roundNumber(item.totalArea, 4);
        })
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
    this.display = true;
  }

  // Show filter
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

  refreshFilter() {
    this.listSelectedStatus = [];
    this.productionOrderCode = "";
    this.totalProductionOrderCode = "";
    this.customerName = "";
    this.listProductOrder = [];
    this.isShowFilterLeft = false;
    this.isShowFilterTop = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.selectedTypeProductionOrder = null;
    // Ngày sản xuất
    this.fromDate = null;
    // Ngày dự kiến trả
    this.toDate = null;
    this.noteTechnique = "";
    this.isError = false;

    this.loading = true;
    this.getMasterData();
    this.getAll()
  }

  gotoProductionOrderDetail(productionOrderId: string) {
    //this.router.navigate(['/manufacture/production-order/detail', { productionOrderId: productionOrderId }]);
    let url = '/manufacture/production-order/detail' + ';productionOrderId=' + productionOrderId;
    window.open(url, "_blank");
  }

  searchProductionOrder() {
    this.productionOrderCode = this.productionOrderCode;
    this.totalProductionOrderCode = this.totalProductionOrderCode;
    this.customerName = this.customerName;

    this.noteTechnique = this.noteTechnique;
    this.loading = true;
    this.listProductOrder = [];
    this.getAll()
  }

  dateFieldFormat: string = 'DD/MM/YYYY';
  customSort(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];

      /**Customize sort date */
      if (event.field == 'endDate') {
        const date1 = moment(value1, this.dateFieldFormat);
        const date2 = moment(value2, this.dateFieldFormat);

        let result: number = -1;
        if (moment(date2).isBefore(date1, 'day')) { result = 1; }

        return result * event.order;
      }

      /**Customize sort date */
      if (event.field == 'startDate') {
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

  updateEspecially(productionOrderId: string, especially: boolean) {
    this.loading = true;
    this.manufactureService.updateProductionOrderEspecially(productionOrderId, especially).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật thành công' };
        this.showMessage(msg);
        this.refreshFilter();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  changeNote(data: any) {
    this.manufactureService.updateProductionOrderNote(data.productionOrderId, data.note).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {

      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  changeTechForItem(data: any) {
    this.isDisableSaveCatHa = false;
    this.manufactureService.getListItemChange(data.productionOrderId).subscribe(response => {
      let result: any = response;
      if (result.statusCode == 200) {
        this.listItemInfor = [];
        this.listItemInfor = result.listItem;
        this.dataTree = [];
        let newDataTree = [];
        this.listItemInfor.forEach((item: ItemInfor) => {
          let parent_item: TreeNode = {
            data: {},
            children: []
          };

          //Lấy các item cha
          if (item.parentPartId == null) {
            parent_item.data = item;

            //Thêm vào dataTree
            newDataTree.push(parent_item);

            //Lấy các bán thành phẩm cấp 1 (nếu có) của item cha
            let listItemLv1 = this.listItemInfor.filter(x => x.parentPartId == item.productionOrderMappingId);
            listItemLv1.forEach((item_lv1: ItemInfor) => {
              let _item_lv1: TreeNode = {
                data: {},
                children: []
              };

              _item_lv1.data = item_lv1;

              //Thêm vào dataTree
              let find_item_lv0 = newDataTree.find(x => x.data.productionOrderMappingId == item_lv1.parentPartId);
              find_item_lv0.children.push(_item_lv1);

              //Lấy các bán thành phẩm cấp 2 (nếu có) của item lv1
              let listItemLv2 = this.listItemInfor.filter(x => x.parentPartId == item_lv1.productionOrderMappingId);
              listItemLv2.forEach((item_lv2: ItemInfor) => {
                let _item_lv2: TreeNode = {
                  data: {},
                  children: []
                };

                _item_lv2.data = item_lv2;

                //Thêm vào dataTree
                let find_item_lv1 = newDataTree.find(x => x.data.productionOrderMappingId == item_lv1.parentPartId).children.find(y => y.data.productionOrderMappingId == item_lv2.parentPartId);
                find_item_lv1.children.push(_item_lv2);
              });
            });
          }
        });

        this.dataTree = [...newDataTree];
        this.displayChangeTech = true;
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  checkKey(data: any) {
  }

  cancelDialogCatHa() {
    this.displayChangeTech = false;
    this.listWorkflow = '';
  }

  isDisableSaveCatHa: boolean = false;
  saveCatHa() {
    this.isDisableSaveCatHa = true;
    this.loading = true;

    //Lọc ra những Item cha
    let _listItemInfor = this.listItemInfor.filter(x => x.isOriginal == true);

    this.listItemInfor.forEach(item => {
      item.productThickness = ParseStringToFloat(item.productThickness);
    });

    this.manufactureService.saveCatHa(this.listItemInfor).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.displayChangeTech = false;
        this.listWorkflow = '';
        let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Lưu thành công' };
        this.showMessage(msg);
        this.getAll();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  listWorkflow: string = '';
  showWorkflow(productOrderWorkflowId: string) {
    this.manufactureService.getListWorkflowById(productOrderWorkflowId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listWorkflow = result.listWorkflow;
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
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

  ngOnDestroy() {
    if (this.id_page) {
      clearInterval(this.id_page);
    }
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function ParseStringToFloat(str: any) {
  if (str === "") return 0;
  str = str.toString().replace(/,/g, '');
  return parseFloat(str);
};