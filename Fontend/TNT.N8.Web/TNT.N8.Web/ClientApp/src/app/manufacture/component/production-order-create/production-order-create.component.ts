import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { ManufactureService } from '../../services/manufacture.service';
import { MessageService, TreeNode } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import 'moment/locale/pt-br';
import * as $ from 'jquery';
import { element } from 'protractor';

class ProductionOrder {
  productionOrderId: string;
  productionOrderCode: string;
  orderId: string;
  customerName: string;
  orderCustomerCode: string;
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
  parentId: string;

  constructor() {
    this.productionOrderId = '00000000-0000-0000-0000-000000000000';
    this.productionOrderCode = null;
    this.orderId = '00000000-0000-0000-0000-000000000000';
    this.customerName = null;
    this.orderCustomerCode = null;
    this.placeOfDelivery = null;
    this.receivedDate = null;
    this.startDate = null;
    this.endDate = null;
    this.note = null;
    this.noteTechnique = null;
    this.especially = false;
    this.statusId = '00000000-0000-0000-0000-000000000000';
    this.createdDate = convertToUTCTime(new Date());
    this.createdById = '00000000-0000-0000-0000-000000000000';
    this.customerNumber = null;
    this.parentId = null;
  }
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
class MappingOrderTechnique {
  productOrderWorkflowId: string;
  name: string;
  isDefault: boolean;
  listTechniqueRequest: Array<Technique>;
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
  productCode: string;
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
  productOrderWorkflowId: string;
  listTechnique: Array<Technique>;
  parentIndex: string;
  borehole: number;
  hole: number;
  productGroupCode: string;
  description: string;// Mô tả lỗi
  nameNest: string;  // Tên tổ
  productionOrderHistoryId: string // Id bản lịch sử
  techniqueOrder: number // Order tiến trình lỗi
  originalId: string; // Tiến trình gây lỗi
  grind: number;  //Mài
  stt: number;  //Số thứ tự
  note: string;
}

@Component({
  selector: 'app-production-order-create',
  templateUrl: './production-order-create.component.html',
  styleUrls: ['./production-order-create.component.css']
})
export class ProductionOrderCreateComponent implements OnInit {
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;

  @ViewChild('myTable') myTable: Table;
  colsListProduct: any;
  listProduct: TreeNode[] = [];
  selectedProduct: TreeNode[];
  cities: any[];
  orderId: string;
  loading: boolean = false;
  orderCode: string;
  listMappingOrderTechnique: Array<MappingOrderTechnique>;
  selectedProductOrderWorkflow: MappingOrderTechnique;
  defaultProductOrderWorkflow: MappingOrderTechnique;
  // Hiện yêu cầu kỹ thuật đặc biệt
  showCut: boolean = false; // Cắt
  showPase: boolean = false; // Dán
  showGrinding: boolean = false; // Mài
  showDrill: boolean = false; // Khoan
  showI: boolean = false; // Tôi
  showBox: boolean = false; // Hộp
  rowSelected: any;
  isAddAll: boolean = false;
  parentNode: any;
  isSession: boolean = false;
  isShowDialog: boolean = false;
  isOverFlow: boolean = true;
  // Form
  productOrderForm: FormGroup;
  endDateControl: FormControl;
  startDateControl: FormControl;
  especiallyControl: FormControl;
  placeOfDeliveryControl: FormControl;
  noteTechniqueControl: FormControl;

  // Form dialog
  dialogForm: FormGroup;
  productName11Control: FormControl;
  productName12Control: FormControl;
  productName111Control: FormControl;
  productName112Control: FormControl;
  productName121Control: FormControl;
  productName122Control: FormControl;
  productThickness11Control: FormControl;
  productThickness12Control: FormControl;
  productThickness111Control: FormControl;
  productThickness112Control: FormControl;
  productThickness121Control: FormControl;
  productThickness122Control: FormControl;
  productOrderWorkflowBTPControl: FormControl;
  productOrderWorkflowBTP11Control: FormControl;
  productOrderWorkflowBTP12Control: FormControl;
  productOrderWorkflowBTP111Control: FormControl;
  productOrderWorkflowBTP112Control: FormControl;
  productOrderWorkflowBTP121Control: FormControl;
  productOrderWorkflowBTP122Control: FormControl;
  productGroupCode11Control: FormControl;
  productGroupCode12Control: FormControl;
  productGroupCode111Control: FormControl;
  productGroupCode112Control: FormControl;
  productGroupCode121Control: FormControl;
  productGroupCode122Control: FormControl;
  selectedBTPControl: FormControl;
  isChildren1Control: FormControl;
  isChildren2Control: FormControl;

  // End

  // Danh sách quy trình
  listTechniqueName11: string;
  listTechniqueName12: string;
  listTechniqueName111: string;
  listTechniqueName112: string;
  listTechniqueName121: string;
  listTechniqueName122: string;
  // Show button add thêm tiến trình đặc biệt
  showButton: boolean = false;
  productionOrderModel: ProductionOrder = new ProductionOrder();

  totalQuantity: number = 0;
  totalArea: number = 0;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private manufactureService: ManufactureService,
    private messageService: MessageService,
    private getPermission: GetPermission,
  ) { }

  async ngOnInit() {
    this.initTable();
    this.setForm();
    let resource = "man/manufacture/production-order/create";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
    }


    this.route.params.subscribe(params => { this.orderId = params['customerOrderID'] });
    this.loading = true;
    this.manufactureService.getMasterDataCreateProductionOrder(this.orderId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        // Lấy danh sách tiến trình đặc biệt
        let customerOrderObject = result.customerOrderObject;
        let listCustomerOrderDetail = result.listCustomerOrderDetail;

        this.productionOrderModel.orderId = customerOrderObject.orderId;
        this.productionOrderModel.receivedDate = customerOrderObject.orderDate == null ? null : new Date(customerOrderObject.orderDate);
        // Số ĐH của KH
        this.productionOrderModel.customerNumber = customerOrderObject.customerNumber;
        this.productionOrderModel.placeOfDelivery = customerOrderObject.placeOfDelivery;
        this.productionOrderModel.customerName = customerOrderObject.customerName;
        let receivedDate = customerOrderObject.receivedDate == null ? null : new Date(customerOrderObject.receivedDate);
        let manufactureDate = customerOrderObject.manufactureDate == null ? null : new Date(customerOrderObject.manufactureDate);
        this.endDateControl.setValue(receivedDate);
        this.startDateControl.setValue(manufactureDate);
        this.placeOfDeliveryControl.setValue(customerOrderObject.placeOfDelivery);
        this.noteTechniqueControl.setValue(customerOrderObject.noteTechnique)
        this.productionOrderModel.noteTechnique = customerOrderObject.noteTechnique;
        this.orderCode = customerOrderObject.orderCode;

        this.listMappingOrderTechnique = [];
        result.listMappingOrder.forEach(element => {
          let map = new MappingOrderTechnique();
          map.isDefault = element.isDefault;
          map.name = element.name;
          map.productOrderWorkflowId = element.productOrderWorkflowId;
          map.listTechniqueRequest = element.listTechniqueRequest;
          this.listMappingOrderTechnique.push(map);

          if (element.isDefault == true) {
            this.selectedProductOrderWorkflow = element;
            this.defaultProductOrderWorkflow = element;

          }
        });
        let index = 0;
        listCustomerOrderDetail.forEach(element => {
          let techniqueRequestMaping = new TechniqueRequestMaping();
          let productOrderWorkflowName = ""
          let productOrderWorkflowId = "";
          let listTechnique = new Array<Technique>();
          if (this.defaultProductOrderWorkflow != null) {
            productOrderWorkflowName = this.defaultProductOrderWorkflow.name;
            productOrderWorkflowId = this.defaultProductOrderWorkflow.productOrderWorkflowId;
            this.defaultProductOrderWorkflow.listTechniqueRequest.forEach(item => {
              listTechnique.push(item);

            });

          }
          techniqueRequestMaping.index = index.toString();
          techniqueRequestMaping.productColorCode = element.productColorCode;
          techniqueRequestMaping.productColor = element.productColor;
          techniqueRequestMaping.productLength = element.productLength == null ? 0 : element.productLength;
          techniqueRequestMaping.productName = element.productName;
          techniqueRequestMaping.productThickness = element.productThickness == null ? 0 : element.productThickness;
          techniqueRequestMaping.productWidth = element.productWidth == null ? 0 : element.productWidth;
          techniqueRequestMaping.quantity = element.quantity == null ? 0 : element.quantity;
          let width = techniqueRequestMaping.productWidth / 1000;
          let length = techniqueRequestMaping.productLength / 1000;
          let number = techniqueRequestMaping.quantity;
          let total: number = (width * length * number);
          techniqueRequestMaping.techniqueDescription = element.techniqueDescription + "; " + this.productionOrderModel.noteTechnique;
          techniqueRequestMaping.totalArea = this.roundNumber(total, 4);
          techniqueRequestMaping.productOrderWorkflowName = productOrderWorkflowName;
          techniqueRequestMaping.productOrderWorkflowId = productOrderWorkflowId;
          techniqueRequestMaping.listTechnique = listTechnique;
          techniqueRequestMaping.productCode = element.productCode;
          techniqueRequestMaping.borehole = element.borehole;
          techniqueRequestMaping.hole = element.hole;
          techniqueRequestMaping.productGroupCode = element.productGroupCode;
          //add by Giang
          techniqueRequestMaping.grind = element.grind;
          techniqueRequestMaping.stt = element.stt;
          //end

          let node: TreeNode = { data: techniqueRequestMaping, children: [] };
          index = index + 1;
          this.listProduct.push(node);
        });
        this.listProduct = [...this.listProduct];

        //Tính tổng số tấm và Tổng số m2
        this.calculatorTotal();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setForm() {
    this.endDateControl = new FormControl(new Date(), [Validators.required]);
    this.startDateControl = new FormControl(null);
    this.placeOfDeliveryControl = new FormControl(null);
    this.noteTechniqueControl = new FormControl(null);
    this.especiallyControl = new FormControl(false);

    this.productName11Control = new FormControl(null, [Validators.required]);
    this.productName12Control = new FormControl(null, [Validators.required]);
    this.productName111Control = new FormControl(null, [Validators.required]);
    this.productName112Control = new FormControl(null, [Validators.required]);
    this.productName121Control = new FormControl(null, [Validators.required]);
    this.productName122Control = new FormControl(null, [Validators.required]);
    this.productThickness11Control = new FormControl(0, [Validators.required]);
    this.productThickness12Control = new FormControl(0, [Validators.required]);
    this.productThickness111Control = new FormControl(0, [Validators.required]);
    this.productThickness112Control = new FormControl(0, [Validators.required]);
    this.productThickness121Control = new FormControl(0, [Validators.required]);
    this.productThickness122Control = new FormControl(0, [Validators.required]);
    this.productOrderWorkflowBTPControl = new FormControl(null, [Validators.required]);
    this.productOrderWorkflowBTP11Control = new FormControl(null, [Validators.required]);
    this.productOrderWorkflowBTP12Control = new FormControl(null, [Validators.required]);
    this.productOrderWorkflowBTP111Control = new FormControl(null, [Validators.required]);
    this.productOrderWorkflowBTP112Control = new FormControl(null, [Validators.required]);
    this.productOrderWorkflowBTP121Control = new FormControl(null, [Validators.required]);
    this.productOrderWorkflowBTP122Control = new FormControl(null, [Validators.required]);
    this.productGroupCode11Control = new FormControl("");
    this.productGroupCode12Control = new FormControl("");
    this.productGroupCode111Control = new FormControl("");
    this.productGroupCode112Control = new FormControl("");
    this.productGroupCode121Control = new FormControl("");
    this.productGroupCode122Control = new FormControl("");
    this.selectedBTPControl = new FormControl("BTP1", [Validators.required]);
    this.isChildren1Control = new FormControl(false);
    this.isChildren2Control = new FormControl(false);


    this.productOrderForm = new FormGroup({
      endDateControl: this.endDateControl,
      startDateControl: this.startDateControl,
      placeOfDeliveryControl: this.placeOfDeliveryControl,
      noteTechniqueControl: this.noteTechniqueControl,
      especiallyControl: this.especiallyControl
    });


    this.dialogForm = new FormGroup({
      productName11Control: this.productName11Control,
      productName12Control: this.productName12Control,
      productName111Control: this.productName111Control,
      productName112Control: this.productName112Control,
      productName121Control: this.productName121Control,
      productName122Control: this.productName122Control,
      productThickness11Control: this.productThickness11Control,
      productThickness12Control: this.productThickness12Control,
      productThickness111Control: this.productThickness111Control,
      productThickness112Control: this.productThickness112Control,
      productThickness121Control: this.productThickness121Control,
      productThickness122Control: this.productThickness122Control,
      productOrderWorkflowBTPControl: this.productOrderWorkflowBTPControl,
      productOrderWorkflowBTP11Control: this.productOrderWorkflowBTP11Control,
      productOrderWorkflowBTP12Control: this.productOrderWorkflowBTP12Control,
      productOrderWorkflowBTP111Control: this.productOrderWorkflowBTP111Control,
      productOrderWorkflowBTP112Control: this.productOrderWorkflowBTP112Control,
      productOrderWorkflowBTP121Control: this.productOrderWorkflowBTP121Control,
      productOrderWorkflowBTP122Control: this.productOrderWorkflowBTP122Control,
      productGroupCode11Control: this.productGroupCode11Control,
      productGroupCode12Control: this.productGroupCode12Control,
      productGroupCode111Control: this.productGroupCode111Control,
      productGroupCode112Control: this.productGroupCode112Control,
      productGroupCode121Control: this.productGroupCode121Control,
      productGroupCode122Control: this.productGroupCode122Control,
      selectedBTPControl: this.selectedBTPControl,
      isChildren1Control: this.isChildren1Control,
      isChildren2Control: this.isChildren2Control
    });
  }

  initTable() {
    this.colsListProduct = [
      { field: 'stt', header: 'STT', width: '150px' },
      { field: 'productName', header: 'Chủng Loại', textAlign: 'left', display: 'table-cell', width: '200px' },
      { field: 'productColor', header: 'Màu sắc', textAlign: 'left', display: 'table-cell', width: '100px' },
      { field: 'productThickness', header: 'Độ dày(mm)', textAlign: 'right', display: 'table-cell', width: '100px' },
      { field: 'productLength', header: 'Chiều dài(mm)', textAlign: 'right', display: 'table-cell', width: '150px' },
      { field: 'productWidth', header: 'Chiều rộng(mm)', textAlign: 'right', display: 'table-cell', width: '150px' },
      { field: 'quantity', header: 'Số tấm', textAlign: 'right', display: 'table-cell', width: '100px' },
      { field: 'totalArea', header: 'Tổng số(m2)', textAlign: 'right', display: 'table-cell', width: '150px' },
      { field: 'borehole', header: 'Khoan', textAlign: 'right', display: 'table-cell', width: '100px' },
      { field: 'hole', header: 'Khoét', textAlign: 'right', display: 'table-cell', width: '100px' },
      { field: 'techniqueDescription', header: 'Mã hiệu', textAlign: 'left', display: 'table-cell', width: '100px' },
      { field: 'productOrderWorkflowName', header: 'Tên quy trình', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'left', display: 'table-cell', width: '100px' },
      { field: 'productGroupCode', header: 'Mã nhóm', textAlign: 'center', display: 'table-cell', width: '100px' },
      { field: 'to', header: 'Thao tác', display: 'table-cell', width: '150px' },
    ];
  }

  calculatorTotal() {
    this.totalQuantity = 0;
    this.totalArea = 0;

    this.listProduct.forEach(item => {
      let quantityString = item.data.quantity.toString();
      let quantity = ParseStringToFloat(quantityString.toString());

      let areaString = item.data.totalArea.toString();
      let area = ParseStringToFloat(areaString.toString());

      this.totalQuantity += quantity;
      this.totalArea += area;
    });

    this.totalArea = this.roundNumber(this.totalArea, 4);
  }

  changeNoteTechnique() {
    this.listProduct.forEach(item => {
      item.children.forEach(element => {
        let description1: string = element.data.techniqueDescription;
        let index = description1.lastIndexOf(";");
        element.data.techniqueDescription = description1.substring(0, index) + ";" + this.noteTechniqueControl.value;
        element.children.forEach(children => {
          let description2: string = children.data.techniqueDescription;
          let index = description2.lastIndexOf(";");
          children.data.techniqueDescription = description2.substring(0, index) + ";" + this.noteTechniqueControl.value;
        });
      });
      let description = item.data.techniqueDescription;
      let index = description.lastIndexOf(";");
      item.data.techniqueDescription = description.substring(0, index) + ";" + this.noteTechniqueControl.value;
    });
  }

  changeInput(event) {
    let data: TechniqueRequestMaping = event.data;
    if (event.data.productLength == "") {
      data.productLength = 0;
    }

    let length: number = ParseStringToFloat(data.productLength.toString());
    let width: number = ParseStringToFloat(data.productWidth.toString());
    let quantity: number = ParseStringToFloat(data.quantity.toString());
    let area: number = length / 1000 * width / 1000 * quantity;
    let hole: number = data.hole;
    let borehole: number = data.borehole;
    area = this.roundNumber(area, 4);
    data.totalArea = area;
    let listChildren = event.children;
    if (listChildren.length > 0) {
      listChildren.forEach(element => {
        element.productLength = length;
        element.quantity = quantity.toString();
        element.productWidth = width.toString();
        element.totalArea = data.totalArea;
        element.hole = hole.toString();
        element.borehole = borehole.toString();
      });

      this.listProduct.forEach(item => {
        if (item.data.index == data.index) {
          item.children.forEach(x => {
            x.data.productLength = length;
            x.data.quantity = quantity;
            x.data.productWidth = width;
            x.data.totalArea = data.totalArea;
            x.data.hole = hole.toString();
            x.data.borehole = borehole.toString();
            x.children.forEach(z => {
              z.data.productLength = length;
              z.data.quantity = quantity;
              z.data.productWidth = width;
              z.data.totalArea = data.totalArea;
              z.data.hole = hole.toString();
              z.data.borehole = borehole.toString();
            });
          });

        }
      });
      this.listProduct = [...this.listProduct];
    }

    //Tính lại tổng số tấm và tổng số m2
    this.calculatorTotal();
  }


  onEditComplete(event) {
    let data: TechniqueRequestMaping = event.data;
    if (event.data.productLength == "") {
      data.productLength = 0;
    }

    let length: number = ParseStringToFloat(data.productLength.toString());
    let width: number = ParseStringToFloat(data.productWidth.toString());
    let quantity: number = ParseStringToFloat(data.quantity.toString());
    let area: number = length / 1000 * width / 1000 * quantity;
    let hole: number = data.hole;
    let borehole: number = data.borehole;
    area = this.roundNumber(area, 4);
    data.totalArea = area;
    let listChildren = event.children;
    if (listChildren.length > 0) {
      listChildren.forEach(element => {
        element.productLength = length;
        element.quantity = quantity.toString();
        element.productWidth = width.toString();
        element.totalArea = data.totalArea;
        element.hole = hole.toString();
        element.borehole = borehole.toString();
      });

      this.listProduct.forEach(item => {
        if (item.data.index == data.index) {
          item.children.forEach(x => {
            x.data.productLength = length;
            x.data.quantity = quantity;
            x.data.productWidth = width;
            x.data.totalArea = data.totalArea;
            x.data.hole = hole.toString();
            x.data.borehole = borehole.toString();
            x.children.forEach(z => {
              z.data.productLength = length;
              z.data.quantity = quantity;
              z.data.productWidth = width;
              z.data.totalArea = data.totalArea;
              z.data.hole = hole.toString();
              z.data.borehole = borehole.toString();
            });
          });
        }
      });
      this.listProduct = [...this.listProduct];
    }

    //Tính lại tổng số tấm và tổng số m2
    this.calculatorTotal();
  }

  // Lưu lệnh sản xuất
  saveProductionOrder() {
    if (!this.productOrderForm.valid) {
      Object.keys(this.productOrderForm.controls).forEach(key => {
        if (this.productOrderForm.controls[key].valid == false) {
          this.productOrderForm.controls[key].markAsTouched();
        }
      });
    } else {
      let endDate = this.endDateControl.value;
      if (endDate) {
        endDate = convertToUTCTime(endDate);
      }
      this.productionOrderModel.endDate = endDate;
      let startDate = this.startDateControl.value;

      if (startDate) {
        startDate = convertToUTCTime(startDate);
      }

      let reviewDate = this.productionOrderModel.receivedDate;
      if (reviewDate) {
        reviewDate = convertToUTCTime(reviewDate);
      }
      let placeOfDelivery = this.placeOfDeliveryControl.value;
      let noteTechnique = this.noteTechniqueControl.value;
      let especially = this.especiallyControl.value;
      this.productionOrderModel.receivedDate = reviewDate;
      this.productionOrderModel.startDate = startDate;
      this.productionOrderModel.placeOfDelivery = placeOfDelivery;
      this.productionOrderModel.noteTechnique = noteTechnique;
      this.productionOrderModel.especially = especially;
      this.productionOrderModel.productionOrderCode = this.orderCode;
      let listProduct: Array<TechniqueRequestMaping> = [];
      listProduct = this.listProduct.map(item => item.data);
      let isSave: boolean = true;
      listProduct.forEach(item => {
        if (item.listTechnique == null || item.listTechnique.length == 0) {
          isSave = false;
        }
      });
      let listProductChildren: Array<TechniqueRequestMaping> = [];
      let listProductChildrenChildren: Array<TechniqueRequestMaping> = [];
      this.listProduct.forEach(item => {
        if (item.children.length > 0) {
          item.children.forEach(x => {
            if (x.data.listTechnique == null || x.data.listTechnique.length == 0) {
              isSave = false;
            }
            let techniqueRequestMaping: TechniqueRequestMaping = new TechniqueRequestMaping();
            techniqueRequestMaping = x.data;
            techniqueRequestMaping.parentIndex = item.data.index;
            listProductChildren.push(techniqueRequestMaping);
            if (x.children.length > 0) {
              x.children.forEach(z => {
                if (z.data.listTechnique == null || z.data.listTechnique.length == 0) {
                  isSave = false;
                }
                let techniqueRequestMaping2: TechniqueRequestMaping = new TechniqueRequestMaping();
                techniqueRequestMaping2 = z.data;
                techniqueRequestMaping2.parentIndex = x.data.index;
                listProductChildrenChildren.push(techniqueRequestMaping2);
              })
            }
          });
        }
      })
      if (isSave) {
        this.loading = true;
        this.manufactureService.createProductionOrder(this.productionOrderModel, listProduct, listProductChildren, listProductChildrenChildren).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: "Thêm thành công" };

            this.router.navigate(['/manufacture/production-order/detail', { productionOrderId: result.productionOrderId }]);

            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: "Phải chọn tiến trình cho tất cả các sản phầm" };
        this.showMessage(msg);
      }

    }
  }

  // Thêm bán thành phẩm
  AddProduct(data) {
    if (this.isSession) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Bạn chưa lưu lại tiến trình!" };
      this.showMessage(msg);
    }
    else {
      let index = "";
      let numberchildren = data.node.children.length + 1;
      index = data.node.data.index + '.' + numberchildren;
      if (data.node.parent == null) {
        for (let i = 0; i < this.listProduct.length; i++) {
          if (this.listProduct[i] == <TreeNode>data.node) {
            if (this.listProduct[i].children == null) {
              this.listProduct[i].children = [];
            }
            let demo: TreeNode = { data: {}, children: [] }
            let treeNode: TechniqueRequestMaping = new TechniqueRequestMaping();
            treeNode.productOrderWorkflowId = data.node.data.productOrderWorkflowId;
            treeNode.index = index;
            treeNode.productColorCode = data.node.data.productColorCode;
            treeNode.productLength = data.node.data.productLength;
            treeNode.productName = data.node.data.productName;
            treeNode.productThickness = data.node.data.productThickness;
            treeNode.productWidth = data.node.data.productWidth;
            treeNode.quantity = data.node.data.quantity;
            treeNode.productColor = data.node.data.productColor;
            treeNode.techniqueDescription = data.node.data.techniqueDescription;
            treeNode.totalArea = data.node.data.totalArea;
            treeNode.hole = data.node.data.hole;
            treeNode.borehole = data.node.data.borehole;
            treeNode.productGroupCode = data.node.data.productGroupCode;

            if (data.node.data != null) {
              treeNode.productOrderWorkflowName = data.node.data.productOrderWorkflowName;
              treeNode.productOrderWorkflowId = data.node.data.productOrderWorkflowId;
              treeNode.listTechnique = data.node.data.listTechnique;
            }
            demo.data = treeNode;
            this.listProduct[i].children.push(demo);
          }
        }
        this.listProduct = [...this.listProduct];
      }
      else {
        for (let i = 0; i < this.listProduct.length; i++) {
          if (this.listProduct[i].data.index == data.node.parent.data.index) {
            for (let j = 0; j < this.listProduct[i].children.length; j++) {
              if (this.listProduct[i].children[j].data.index == data.node.data.index) {
                if (this.listProduct[i].children[j].children == null) {
                  this.listProduct[i].children[j].children = [];
                }
                let demo: TreeNode = { data: {}, children: [] }
                let treeNode: TechniqueRequestMaping = new TechniqueRequestMaping();
                treeNode.productOrderWorkflowId = data.node.data.productOrderWorkflowId;
                treeNode.index = index;
                treeNode.productColorCode = data.node.data.productColorCode;
                treeNode.productLength = data.node.data.productLength;
                treeNode.productName = data.node.data.productName;
                treeNode.productThickness = data.node.data.productThickness;
                treeNode.productWidth = data.node.data.productWidth;
                treeNode.quantity = data.node.data.quantity;
                treeNode.productColor = data.node.data.productColor;
                treeNode.techniqueDescription = data.node.data.techniqueDescription;
                treeNode.totalArea = data.node.data.totalArea;
                treeNode.hole = data.node.data.hole;
                treeNode.borehole = data.node.data.borehole;
                treeNode.productGroupCode = data.node.data.productGroupCode;

                if (data.node.data != null) {
                  treeNode.productOrderWorkflowName = data.node.data.productOrderWorkflowName;
                  treeNode.productOrderWorkflowId = data.node.data.productOrderWorkflowId;
                  treeNode.listTechnique = data.node.data.listTechnique;
                }
                demo.data = treeNode;

                this.listProduct[i].children[j].children.push(demo);
              }
            }
          }
        }
        this.listProduct = [...this.listProduct];

      }
      this.showTreeTable();
    }
  }

  showTreeTable() {
    this.listProduct.forEach(item => {
      item.expanded = true;
      item.children.forEach(children => {
        children.expanded = true;
        children.children.forEach(x => {
          x.expanded = true;
        })
      });
    });
  }

  resize(event) {
    if (event.target.className.includes("pi pi-window-maximize")) {
      this.isOverFlow = true;
    } else if (event.target.className.includes("pi pi-window-minimize")) {
      this.isOverFlow = false;
    }
  }

  // Thêm nhiều bán thành phẩm
  applyAll() {
    let isAdd = true;
    let btp = this.selectedBTPControl.value;
    if (!this.dialogForm.valid) {
      Object.keys(this.dialogForm.controls).forEach(key => {
        if (this.dialogForm.controls[key].valid == false) {
          if (key == "productOrderWorkflowBTPControl") {
            isAdd = false;
          }
          if (btp == 'BTP1') {
            if (key == 'productName11Control' || key == 'productName12Control' ||
              key == 'productThickness11Control' || key == 'productThickness12Control' ||
              key == 'productOrderWorkflowBTP11Control' || key == 'productOrderWorkflowBTP12Control') {
              isAdd = false;
            }
            else {
              isAdd = true;
            }
          }
          else {
            if (!this.isChildren1Control.value && !this.isChildren2Control.value) {
              isAdd = false;
            }
            else {
              if (this.isChildren1Control.value &&
                (key == 'productName111Control' || key == 'productName112Control' ||
                  key == 'productThickness111Control' || key == 'productThickness112Control' ||
                  key == 'productOrderWorkflowBTP111Control' || key == 'productOrderWorkflowBTP112Control')) {
                isAdd = false;
              }
              else {
                isAdd = true;
              }

              if (isAdd) {
                if (this.isChildren2Control.value &&
                  (key == 'productName121Control' || key == 'productName122Control' ||
                    key == 'productThickness121Control' || key == 'productThickness122Control' ||
                    key == 'productOrderWorkflowBTP121Control' || key == 'productOrderWorkflowBTP122Control')) {
                  isAdd = false;
                }
                else {
                  isAdd = true;
                }
              }
            }
          }
          this.dialogForm.controls[key].markAsTouched();
        }
      });
    }

    if (btp == "BTP2" && !this.isChildren2Control.value && !this.isChildren1Control.value) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Phải có ít nhất 1 bán thành phẩm con!" };
      this.showMessage(msg);
    }

    if (isAdd) {
      let wflBTP = this.productOrderWorkflowBTPControl.value;
      let wflBTP11 = this.productOrderWorkflowBTP11Control.value;
      let wflBTP12 = this.productOrderWorkflowBTP12Control.value;
      let wflBTP111 = this.productOrderWorkflowBTP111Control.value;
      let wflBTP112 = this.productOrderWorkflowBTP112Control.value;
      let wflBTP121 = this.productOrderWorkflowBTP121Control.value;
      let wflBTP122 = this.productOrderWorkflowBTP122Control.value;

      if (this.productOrderWorkflowBTPControl.value) {
        for (let i = 0; i < this.listProduct.length; i++) {
          this.listProduct[i].children = [];
          this.listProduct[i].data.productOrderWorkflowName = wflBTP.name;
          this.listProduct[i].data.productOrderWorkflowId = wflBTP.productOrderWorkflowId;
          this.listProduct[i].data.listTechnique = wflBTP.listTechniqueRequest;
          let index = "";
          let numberchildren = this.listProduct[i].children.length + 1;
          index = this.listProduct[i].data.index + '.' + numberchildren;
          let demo: TreeNode = { data: {}, children: [] }
          let treeNode: TechniqueRequestMaping = new TechniqueRequestMaping();
          treeNode.index = index;
          treeNode.productColorCode = this.listProduct[i].data.productColorCode;
          treeNode.productLength = this.listProduct[i].data.productLength;
          treeNode.productName = this.productName11Control.value;
          treeNode.productThickness = this.productThickness11Control.value;
          treeNode.productWidth = this.listProduct[i].data.productWidth;
          treeNode.quantity = this.listProduct[i].data.quantity;
          treeNode.productColor = this.listProduct[i].data.productColor;
          treeNode.techniqueDescription = this.listProduct[i].data.techniqueDescription;
          treeNode.totalArea = this.listProduct[i].data.totalArea;
          treeNode.hole = this.listProduct[i].data.hole;
          treeNode.borehole = this.listProduct[i].data.borehole;
          treeNode.productGroupCode = this.productGroupCode11Control.value == null ? "" : this.productGroupCode11Control.value.trim(); //this.listProduct[i].data.productGroupCode;
          treeNode.productOrderWorkflowName = wflBTP11.name;
          treeNode.productOrderWorkflowId = wflBTP11.productOrderWorkflowId;
          treeNode.listTechnique = wflBTP11.listTechniqueRequest;
          demo.data = treeNode;

          if (this.isChildren1Control.value && btp == 'BTP2') {
            let index1 = "";
            let numberchildren1 = demo.children.length + 1;
            index1 = demo.data.index + '.' + numberchildren1;
            let children1: TreeNode = { data: {}, children: [] }
            let treeNodeChildren1: TechniqueRequestMaping = new TechniqueRequestMaping();
            treeNodeChildren1.index = index1;
            treeNodeChildren1.productColorCode = this.listProduct[i].data.productColorCode;
            treeNodeChildren1.productLength = this.listProduct[i].data.productLength;
            treeNodeChildren1.productName = this.productName111Control.value;
            treeNodeChildren1.productThickness = this.productThickness111Control.value;
            treeNodeChildren1.productWidth = this.listProduct[i].data.productWidth;
            treeNodeChildren1.quantity = this.listProduct[i].data.quantity;
            treeNodeChildren1.productColor = this.listProduct[i].data.productColor;
            treeNodeChildren1.techniqueDescription = this.listProduct[i].data.techniqueDescription;
            treeNodeChildren1.totalArea = this.listProduct[i].data.totalArea;
            treeNodeChildren1.hole = this.listProduct[i].data.hole;
            treeNodeChildren1.borehole = this.listProduct[i].data.borehole;
            treeNodeChildren1.productGroupCode = this.productGroupCode111Control.value == null ? "" : this.productGroupCode111Control.value.trim(); //this.listProduct[i].data.productGroupCode;
            treeNodeChildren1.productOrderWorkflowName = wflBTP111.name;
            treeNodeChildren1.productOrderWorkflowId = wflBTP111.productOrderWorkflowId;
            treeNodeChildren1.listTechnique = wflBTP111.listTechniqueRequest;
            children1.data = treeNodeChildren1;
            demo.children.push(children1);

            numberchildren1 = demo.children.length + 1;
            index1 = demo.data.index + '.' + numberchildren1;
            let children2: TreeNode = { data: {}, children: [] }
            let treeNodeChildren2: TechniqueRequestMaping = new TechniqueRequestMaping();
            treeNodeChildren2.index = index1;
            treeNodeChildren2.productColorCode = this.listProduct[i].data.productColorCode;
            treeNodeChildren2.productLength = this.listProduct[i].data.productLength;
            treeNodeChildren2.productName = this.productName112Control.value;
            treeNodeChildren2.productThickness = this.productThickness112Control.value;
            treeNodeChildren2.productWidth = this.listProduct[i].data.productWidth;
            treeNodeChildren2.quantity = this.listProduct[i].data.quantity;
            treeNodeChildren2.productColor = this.listProduct[i].data.productColor;
            treeNodeChildren2.techniqueDescription = this.listProduct[i].data.techniqueDescription;
            treeNodeChildren2.totalArea = this.listProduct[i].data.totalArea;
            treeNodeChildren2.hole = this.listProduct[i].data.hole;
            treeNodeChildren2.borehole = this.listProduct[i].data.borehole;
            treeNodeChildren2.productGroupCode = this.productGroupCode112Control.value == null ? "" : this.productGroupCode112Control.value.trim(); //this.listProduct[i].data.productGroupCode;
            treeNodeChildren2.productOrderWorkflowName = wflBTP112.name;
            treeNodeChildren2.productOrderWorkflowId = wflBTP112.productOrderWorkflowId;
            treeNodeChildren2.listTechnique = wflBTP112.listTechniqueRequest;
            children2.data = treeNodeChildren2;
            demo.children.push(children2);
          }
          this.listProduct[i].children.push(demo);

          numberchildren = this.listProduct[i].children.length + 1;
          index = this.listProduct[i].data.index + '.' + numberchildren;

          let demo1: TreeNode = { data: {}, children: [] }
          let treeNode2: TechniqueRequestMaping = new TechniqueRequestMaping();
          treeNode2.index = index;
          treeNode2.productColorCode = this.listProduct[i].data.productColorCode;
          treeNode2.productLength = this.listProduct[i].data.productLength;
          treeNode2.productName = this.productName12Control.value;
          treeNode2.productThickness = this.productThickness12Control.value;
          treeNode2.productWidth = this.listProduct[i].data.productWidth;
          treeNode2.quantity = this.listProduct[i].data.quantity;
          treeNode2.productColor = this.listProduct[i].data.productColor;
          treeNode2.techniqueDescription = this.listProduct[i].data.techniqueDescription;
          treeNode2.totalArea = this.listProduct[i].data.totalArea;
          treeNode2.hole = this.listProduct[i].data.hole;
          treeNode2.borehole = this.listProduct[i].data.borehole;
          treeNode2.productGroupCode = this.productGroupCode12Control.value == null ? "" : this.productGroupCode12Control.value.trim(); //this.listProduct[i].data.productGroupCode;
          treeNode2.productOrderWorkflowName = wflBTP12.name;
          treeNode2.productOrderWorkflowId = wflBTP12.productOrderWorkflowId;
          treeNode2.listTechnique = wflBTP12.listTechniqueRequest;
          demo1.data = treeNode2;

          if (this.isChildren2Control.value && btp == 'BTP2') {
            let index1 = "";
            let numberchildren1 = demo.children.length + 1;
            index1 = demo.data.index + '.' + numberchildren1;
            let children1: TreeNode = { data: {}, children: [] }
            let treeNodeChildren1: TechniqueRequestMaping = new TechniqueRequestMaping();
            treeNodeChildren1.index = index1;
            treeNodeChildren1.productColorCode = this.listProduct[i].data.productColorCode;
            treeNodeChildren1.productLength = this.listProduct[i].data.productLength;
            treeNodeChildren1.productName = this.productName121Control.value;
            treeNodeChildren1.productThickness = this.productThickness121Control.value;
            treeNodeChildren1.productWidth = this.listProduct[i].data.productWidth;
            treeNodeChildren1.quantity = this.listProduct[i].data.quantity;
            treeNodeChildren1.productColor = this.listProduct[i].data.productColor;
            treeNodeChildren1.techniqueDescription = this.listProduct[i].data.techniqueDescription;
            treeNodeChildren1.totalArea = this.listProduct[i].data.totalArea;
            treeNodeChildren1.hole = this.listProduct[i].data.hole;
            treeNodeChildren1.borehole = this.listProduct[i].data.borehole;
            treeNodeChildren1.productGroupCode = this.productGroupCode121Control.value == null ? "" : this.productGroupCode121Control.value.trim(); //this.listProduct[i].data.productGroupCode;
            treeNodeChildren1.productOrderWorkflowName = wflBTP121.name;
            treeNodeChildren1.productOrderWorkflowId = wflBTP121.productOrderWorkflowId;
            treeNodeChildren1.listTechnique = wflBTP121.listTechniqueRequest;
            children1.data = treeNodeChildren1;
            demo1.children.push(children1);

            numberchildren1 = demo1.children.length + 1;
            index1 = demo1.data.index + '.' + numberchildren1;
            let children2: TreeNode = { data: {}, children: [] }
            let treeNodeChildren2: TechniqueRequestMaping = new TechniqueRequestMaping();
            treeNodeChildren2.index = index1;
            treeNodeChildren2.productColorCode = this.listProduct[i].data.productColorCode;
            treeNodeChildren2.productLength = this.listProduct[i].data.productLength;
            treeNodeChildren2.productName = this.productName122Control.value;
            treeNodeChildren2.productThickness = this.productThickness122Control.value;
            treeNodeChildren2.productWidth = this.listProduct[i].data.productWidth;
            treeNodeChildren2.quantity = this.listProduct[i].data.quantity;
            treeNodeChildren2.productColor = this.listProduct[i].data.productColor;
            treeNodeChildren2.techniqueDescription = this.listProduct[i].data.techniqueDescription;
            treeNodeChildren2.totalArea = this.listProduct[i].data.totalArea;
            treeNodeChildren2.hole = this.listProduct[i].data.hole;
            treeNodeChildren2.borehole = this.listProduct[i].data.borehole;
            treeNodeChildren2.productGroupCode = this.productGroupCode122Control.value == null ? "" : this.productGroupCode122Control.value.trim(); //this.listProduct[i].data.productGroupCode;
            treeNodeChildren2.productOrderWorkflowName = wflBTP122.name;
            treeNodeChildren2.productOrderWorkflowId = wflBTP122.productOrderWorkflowId;
            treeNodeChildren2.listTechnique = wflBTP122.listTechniqueRequest;
            children2.data = treeNodeChildren2;
            demo1.children.push(children2);
          }
          this.listProduct[i].children.push(demo1);
        }

        this.showTreeTable();
        this.listProduct = [... this.listProduct];
        let msg = { severity: 'success', summary: 'Thông báo:', detail: "Thêm thành công!" };
        this.showMessage(msg);
        this.isShowDialog = false;
      }
    }
  }

  changeWFL11() {
    let name = "";
    for (let index = 0; index < this.productOrderWorkflowBTP11Control.value.listTechniqueRequest.length; index++) {
      if (index == this.productOrderWorkflowBTP11Control.value.listTechniqueRequest.length - 1) {
        name = name + this.productOrderWorkflowBTP11Control.value.listTechniqueRequest[index].techniqueName;
      } else {
        name = name + this.productOrderWorkflowBTP11Control.value.listTechniqueRequest[index].techniqueName + '-->';
      }
    }
    this.listTechniqueName11 = name;
  }

  changeWFL12() {
    let name = "";
    for (let index = 0; index < this.productOrderWorkflowBTP12Control.value.listTechniqueRequest.length; index++) {
      if (index == this.productOrderWorkflowBTP12Control.value.listTechniqueRequest.length - 1) {
        name = name + this.productOrderWorkflowBTP12Control.value.listTechniqueRequest[index].techniqueName;
      } else {
        name = name + this.productOrderWorkflowBTP12Control.value.listTechniqueRequest[index].techniqueName + '-->';
      }
    }
    this.listTechniqueName12 = name;
  }


  changeWFL111() {
    let name = "";
    for (let index = 0; index < this.productOrderWorkflowBTP111Control.value.listTechniqueRequest.length; index++) {
      if (index == this.productOrderWorkflowBTP111Control.value.listTechniqueRequest.length - 1) {
        name = name + this.productOrderWorkflowBTP111Control.value.listTechniqueRequest[index].techniqueName;
      } else {
        name = name + this.productOrderWorkflowBTP111Control.value.listTechniqueRequest[index].techniqueName + '-->';
      }
    }
    this.listTechniqueName111 = name;
  }

  changeWFL112() {
    let name = "";
    for (let index = 0; index < this.productOrderWorkflowBTP112Control.value.listTechniqueRequest.length; index++) {
      if (index == this.productOrderWorkflowBTP112Control.value.listTechniqueRequest.length - 1) {
        name = name + this.productOrderWorkflowBTP112Control.value.listTechniqueRequest[index].techniqueName;
      } else {
        name = name + this.productOrderWorkflowBTP112Control.value.listTechniqueRequest[index].techniqueName + '-->';
      }
    }
    this.listTechniqueName112 = name;
  }


  changeWFL121() {
    let name = "";
    for (let index = 0; index < this.productOrderWorkflowBTP121Control.value.listTechniqueRequest.length; index++) {
      if (index == this.productOrderWorkflowBTP121Control.value.listTechniqueRequest.length - 1) {
        name = name + this.productOrderWorkflowBTP121Control.value.listTechniqueRequest[index].techniqueName;
      } else {
        name = name + this.productOrderWorkflowBTP121Control.value.listTechniqueRequest[index].techniqueName + '-->';
      }
    }
    this.listTechniqueName121 = name;
  }

  changeWFL122() {
    let name = "";
    for (let index = 0; index < this.productOrderWorkflowBTP122Control.value.listTechniqueRequest.length; index++) {
      if (index == this.productOrderWorkflowBTP122Control.value.listTechniqueRequest.length - 1) {
        name = name + this.productOrderWorkflowBTP122Control.value.listTechniqueRequest[index].techniqueName;
      } else {
        name = name + this.productOrderWorkflowBTP122Control.value.listTechniqueRequest[index].techniqueName + '-->';
      }
    }
    this.listTechniqueName122 = name;
  }

  showDiaLog() {
    this.isShowDialog = true;
  }
  // Xóa bán thành phẩm
  RemoveProduct(data) {
    if (this.isSession) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Bạn chưa lưu lại tiến trình!" };
      this.showMessage(msg);
    }
    else {

      for (var index = 0; index < this.listProduct.length; index++) {

        if (this.listProduct[index] == data.node.parent) {
          let len = this.listProduct[index].children.length;
          for (let j = 0; j < len; j++) {
            if (this.listProduct[index].children[j] == data.node) {
              this.listProduct[index].children = [...this.listProduct[index].children.filter(x => x != data.node)];
            }
          }
        }
        let len = this.listProduct[index].children.length;
        let isRemove = false;
        for (let j = 0; j < len; j++) {
          if (isRemove == false) {
            if (this.listProduct[index].children[j] == data.node.parent) {
              for (let z = 0; z < this.listProduct[index].children[j].children.length; z++) {
                if (this.listProduct[index].children[j].children[z] == data.node) {
                  this.listProduct[index].children[j].children = [...this.listProduct[index].children[j].children.filter(x => x != data.node)];
                  isRemove = true;
                  break;
                }
              }
            }
          } else {
            break;
          }
        }
      }
      this.listProduct = [...this.listProduct];
      this.showTreeTable();
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  // Đóng session thêm quy trình
  cancelSession() {
    this.isSession = false;
    if (this.selectedProduct.length > 1) {
      this.isAddAll = true;
    }
  }

  // Hiện session
  showSession(data) {
    if (this.isSession) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Bạn chưa lưu lại tiến trình!" };
      this.showMessage(msg);
    }
    else {
      this.isSession = true;
      this.rowSelected = data;
      this.parentNode = null;
      this.selectedProduct = [];
      let treeNode: TreeNode = { data: {}, children: [] };
      treeNode.data = data.data;
      treeNode.partialSelected = true;
      this.selectedProduct.push(treeNode);
      this.isAddAll = false;
      this.selectedProductOrderWorkflow = this.listMappingOrderTechnique.find(x => x.productOrderWorkflowId == data.data.productOrderWorkflowId);
    }
  }

  showSessionChildren(childrenNode, node) {
    if (this.isSession) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Bạn chưa lưu lại tiến trình!" };
      this.showMessage(msg);
    }
    else {
      this.isSession = true;
      this.rowSelected = childrenNode;
      let treeNode: TreeNode = { data: {}, children: [] };
      this.parentNode = node;
      this.selectedProduct = [];
      treeNode.data = childrenNode.data;
      treeNode.partialSelected = true;
      this.selectedProduct.push(treeNode);
      this.isAddAll = false;
      this.selectedProductOrderWorkflow = this.listMappingOrderTechnique.find(x => x.productOrderWorkflowId == childrenNode.data.productOrderWorkflowId);
    }
  }

  addOrSaveAll() {
    this.isSession = true;
    if (this.defaultProductOrderWorkflow != null) {
      this.selectedProductOrderWorkflow = this.defaultProductOrderWorkflow;
    }
    this.isAddAll = false;
  }

  // Lưu lại thông tin tiến trình và quy trình vào item
  saveSession() {
    // Khi mà không chọn tiến trình nào báo lỗi
    if (this.selectedProductOrderWorkflow == null) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Chưa chọn quy trình cho sản phẩm!" };
      this.showMessage(msg);
    }
    else {
      // Lấy danh sách tất cả các tiến trình đặc biệt

      this.selectedProduct.forEach(item => {
        item.data.productOrderWorkflowId = this.selectedProductOrderWorkflow.productOrderWorkflowId;
        item.data.productOrderWorkflowName = this.selectedProductOrderWorkflow.name;
        item.data.listTechnique = this.selectedProductOrderWorkflow.listTechniqueRequest;
      });

      let msg = { severity: 'success', summary: 'Thông báo:', detail: "Lưu thành công" };
      this.showMessage(msg);
      this.cancelSession();
    }

    if (this.selectedProduct.length > 1) {
      this.isAddAll = true;
    }
  }

  // Tick check box item ẩn và hiện nút thêm nhiều
  selectedListProduct(event) {
    if (this.selectedProduct.length == 1 && this.listProduct.length == 1) {
      this.showSession(this.selectedProduct[0]);
    }
    else if (this.selectedProduct.length > 1) {
      this.isAddAll = true;
      this.cancelSession();
    }
    else {
      this.isAddAll = false;
    }
  }


  unSelectedListProduct(event) {
    if (this.selectedProduct.length > 1) {
      this.isAddAll = true;
    }
    else {
      this.isAddAll = false;
    }
  }

  selectedAll() {
    if (this.selectedProduct.length == 1 && this.listProduct.length == 1) {
      this.showSession(this.selectedProduct[0]);
    }
    else if (this.selectedProduct.length > 1) {
      this.isAddAll = true;
      this.cancelSession();
      this.rowSelected = null;
    }
    else {
      this.isAddAll = false;
      this.cancelSession();
      this.rowSelected = null;
    }

  }


  goToProductionOrderList() {
    this.router.navigate(['/manufacture/production-order/list']);
  }

  goToOrderList() {
    this.router.navigate(['/manufacture/production-order/list']);
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
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
