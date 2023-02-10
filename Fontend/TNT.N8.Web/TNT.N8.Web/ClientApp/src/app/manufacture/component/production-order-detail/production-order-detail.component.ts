import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { ManufactureService } from '../../services/manufacture.service';
import { MessageService, TreeNode } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { formatDate } from '@angular/common';
import { formatNumber } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import * as $ from 'jquery';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { NoteModel } from '../../../shared/models/note.model';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { NoteService } from '../../../shared/services/note.service';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { FileUpload } from 'primeng/fileupload';

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
  nameOrganization: string  // Tên tổ gây lỗi
  descriptionError: string  // Mô tả lỗi

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

// Quy trình và danh sách tiến trình trong quy trình
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
  statusCode: string;
  statusName: string;
  createdDate: Date;
  createdById: string;
  productOrderWorkflowName: string;
  productOrderWorkflowId: string;
  listTechnique: Array<Technique>;
  numberTechniqueSpecial: number;
  parentIndex: string;
  borehole: number;
  hole: number;
  productGroupCode: string;
  description: string;// Mô tả lỗi
  nameNest: string;  // Tên tổ
  productionOrderHistoryId: string // Id bản lịch sử
  techniqueOrder: number // Order tiến trình lỗi
  originalId: string; // Tiến trình gây lỗi
  parentPartId: string; // Id item cha của bán thành phẩm
  grind: number;  //Mài
  stt: number;  //Số thứ tự
  note: string;
}

interface NoteDocument {
  active: boolean;
  base64Url: string;
  createdById: string;
  createdDate: Date;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  noteDocumentId: string;
  noteId: string;
  updatedById: string;
  updatedDate: Date;
}

interface Note {
  active: boolean;
  createdById: string;
  createdDate: Date;
  description: string;
  noteDocList: Array<NoteDocument>;
  noteId: string;
  noteTitle: string;
  objectId: string;
  objectType: string;
  responsibleAvatar: string;
  responsibleName: string;
  type: string;
  updatedById: string;
  updatedDate: Date;
}

class ProductionOrderStatus {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
}

interface FileNameExists {
  oldFileName: string;
  newFileName: string
}

class ExportExcelModel {
  productionOrderCode: string;
  customerInfor: CustomerInforModel;
  listProductInfor: Array<ProductInfor>;
  sumaryProduct: SumaryProductModel;
  constructor() {
    this.listProductInfor = [];

  }
}

class CustomerInforModel {
  customerName: string;
  customerNumber: string;
  placeOfDelivery: string; //địa điểm trả hàng
  receivedDate: string; //ngày nhận
  startDate: string; //ngày sản xuất
  endDate: string; //ngày dự kiến trả
  noteTechnique: string; //ghi chu
}

class ProductInfor {
  index: number;
  productName: string; //chủng loại
  productColor: string; //màu sắc
  productThickness: string; //độ dày
  productLength: string; //chiều dài
  productWidth: string; //chiều rộng
  quantity: string; //số tấm
  totalArea: string; //tổng số
  grind: string; //Mài
  borehole: string; //khoan
  hole: string; //khoét
  techniqueDescription: string; //mã hiệu
}

class SumaryProductModel {
  quantity: number;
  totalArea: number;
  grind: number;
  borehole: number;
  hole: number;
  constructor() {
    this.quantity = 0;
    this.totalArea = 0;
    this.grind = 0;
    this.borehole = 0;
    this.hole = 0;
  }
}

@Component({
  selector: 'app-production-order-detail',
  templateUrl: './production-order-detail.component.html',
  styleUrls: ['./production-order-detail.component.css']
})
export class ProductionOrderDetailComponent implements OnInit {
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionEdit: boolean = true;

  @ViewChild('myTable') myTable: Table;
  colsListProduct: any;
  frozenCols: any;
  listProduct: TreeNode[] = [];
  selectedProduct: TreeNode[];
  cities: any[];
  displayDialog: boolean = false;
  productionOrderId: string;
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
  listOrderStatus: Array<ProductionOrderStatus> = [];
  listStatusDefaul: Array<ProductionOrderStatus> = [];
  listStatusItem: Array<ProductionOrderStatus> = [];

  // Ghi chú
  listUpdateNoteDocument: Array<NoteDocument> = [];
  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  noteHistory: Array<Note> = [];

  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  noteId: string = null;
  noteContent: string = '';
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  colsFile: any;
  isEditWorkFlow: boolean = true;

  // Các trạng thái được sửa hoặc không được sửa thông tin và các sự kiện của các item
  isButtonItem: boolean = false;
  isDisabled: boolean = false;
  isDisabledStatus: boolean = false;
  isDisabledEspecially: boolean = false;
  isStop: boolean = true;
  isWorking: boolean = false;
  isNew: boolean = true;
  isEditItem: boolean = true;
  isSaveSession: boolean = true;
  isOverFlow: boolean = true;
  isShowDialog: boolean = false;
  isShowChangeGroupCode: boolean = false;

  // Form
  productOrderForm: FormGroup;
  endDateControl: FormControl;
  startDateControl: FormControl;
  especiallyControl: FormControl;
  placeOfDeliveryControl: FormControl;
  noteTechniqueControl: FormControl;
  productionOrderStatusControl: FormControl;
  // End

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
  selectedBTPControl: FormControl;
  isChildren1Control: FormControl;
  isChildren2Control: FormControl;
  // End

  //Form sửa mã nhóm cho bán thành phẩm
  dialogChangeGroupCodeForm: FormGroup;
  productGroupCode11Control: FormControl;
  productGroupCode111Control: FormControl;
  productGroupCode112Control: FormControl;
  productGroupCode12Control: FormControl;
  productGroupCode121Control: FormControl;
  productGroupCode122Control: FormControl;
  //End

  // Danh sách quy trình
  listTechniqueName11: string;
  listTechniqueName12: string;
  listTechniqueName111: string;
  listTechniqueName112: string;
  listTechniqueName121: string;
  listTechniqueName122: string;

  actionDelete: boolean = true;

  emptyGuid: string = '00000000-0000-0000-0000-000000000000';

  @ViewChild('fileUpload') fileUpload: FileUpload;

  // Show button add thêm tiến trình đặc biệt
  showButton: boolean = false;
  actionImport: boolean = true;
  productionOrderModel: ProductionOrder = new ProductionOrder();

  totalQuantity: number = 0;
  totalArea: number = 0;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private manufactureService: ManufactureService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private imageService: ImageUploadService,
    private noteService: NoteService,
    private getPermission: GetPermission,
  ) { }

  async  ngOnInit() {
    this.initTable();
    this.setForm();
    let resource = "man/manufacture/production-order/detail";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
    }


    this.route.params.subscribe(params => { this.productionOrderId = params['productionOrderId'] });
    this.loading = true;
    this.getMasterData();
  }

  getMasterData() {
    this.manufactureService.getMasterDataProductionOrderDetail(this.productionOrderId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        // Lấy thông tin lệnh sản xuất
        let productionOrder = result.productionOrder;
        this.productionOrderModel.orderId = productionOrder.orderId;
        this.productionOrderModel.receivedDate = productionOrder.receivedDate == null ? null : new Date(productionOrder.receivedDate);
        // Số ĐH của KH
        this.productionOrderModel.customerNumber = productionOrder.customerNumber;
        this.productionOrderModel.placeOfDelivery = productionOrder.placeOfDelivery;
        this.productionOrderModel.customerName = productionOrder.customerName;
        let endDate = productionOrder.endDate == null ? null : new Date(productionOrder.endDate);
        let startDate = productionOrder.startDate == null ? null : new Date(productionOrder.startDate);
        this.endDateControl.setValue(endDate);
        this.startDateControl.setValue(startDate);
        this.placeOfDeliveryControl.setValue(productionOrder.placeOfDelivery);
        this.noteTechniqueControl.setValue(productionOrder.noteTechnique)
        this.productionOrderModel.noteTechnique = productionOrder.noteTechnique;
        this.productionOrderModel.productionOrderCode = productionOrder.productionOrderCode;
        this.especiallyControl.setValue(productionOrder.especially);
        this.productionOrderModel.parentId = productionOrder.parentId;
        this.productionOrderModel.nameOrganization = productionOrder.nameOrganization;
        this.productionOrderModel.descriptionError = productionOrder.descriptionError;
        // Lấy danh sách status
        result.listStatus.forEach(s => {
          let status: ProductionOrderStatus = new ProductionOrderStatus();
          status.categoryCode = s.categoryCode;
          status.categoryId = s.categoryId;
          status.categoryName = s.categoryName;

          this.listStatusDefaul.push(status)
        });

        this.listStatusItem = result.listStatusItem;
        this.listOrderStatus = this.listStatusDefaul.filter(x => x != null);
        // Nếu là lệnh bổ sung
        if (this.productionOrderModel.parentId != null) {
          this.listOrderStatus = this.listStatusDefaul.filter(x => x != null);
          let status: ProductionOrderStatus = this.listStatusDefaul.find(x => x.categoryId == productionOrder.statusId);
          this.productionOrderStatusControl.setValue(status);
          // Danh sách quy trình và tiến trình của nó
          this.listMappingOrderTechnique = [];
          result.listMappingOrder.forEach(element => {
            let map = new MappingOrderTechnique();
            map.isDefault = element.isDefault;
            map.name = element.name;
            map.productOrderWorkflowId = element.productOrderWorkflowId;
            map.listTechniqueRequest = element.listTechniqueRequest;
            this.listMappingOrderTechnique.push(map);
          });
          this.isDisabled = true;
          this.isDisabledStatus = true;
          this.isDisabledEspecially = true;
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
            { field: 'to', header: 'Thao tác', display: 'table-cell', width: '200px' },
          ];
          // Lấy danh sách item
          let listProduct = result.listProductItem;
          let listProductParent = listProduct.filter(x => x.parentPartId == null);

          let listProductChildren = listProduct.filter(x => x.parentPartId != null);
          if (listProductParent.length > 0) {
            listProductParent.forEach(element => {
              let productOrderWorkflow: MappingOrderTechnique = result.listMappingOrder.find(x => x.productOrderWorkflowId == element.productOrderWorkflowId);
              let productOrderWorkflowName = productOrderWorkflow.name;
              let productOrderWorkflowId = element.productOrderWorkflowId;
              element.productOrderWorkflowName = productOrderWorkflowName;
              element.productOrderWorkflowId = productOrderWorkflowId;
              element.statusName = this.listStatusItem.find(x => x.categoryId == element.statusId).categoryName;

              let listTechniqueRequestMapingChildren: TreeNode[] = [];
              listProductChildren.forEach(children => {
                if (children.parentPartId == element.productionOrderMappingId) {
                  let listTechniqueRequestMapingChildrenChildren: TreeNode[] = [];
                  let productOrderWorkflowChildren: MappingOrderTechnique = result.listMappingOrder.find(x => x.productOrderWorkflowId == children.productOrderWorkflowId);
                  let productOrderWorkflowNameChildren = productOrderWorkflowChildren.name;
                  let productOrderWorkflowIdChildren = children.productOrderWorkflowId;
                  children.productOrderWorkflowName = productOrderWorkflowNameChildren;
                  children.productOrderWorkflowId = productOrderWorkflowIdChildren;
                  children.statusName = this.listStatusItem.find(x => x.categoryId == children.statusId).categoryName;

                  //add by Giang
                  children.grind = children.grind;
                  children.stt = children.stt;
                  //end

                  listProductChildren.forEach(childrenChildren => {
                    if (childrenChildren.parentPartId == children.productionOrderMappingId) {
                      let productOrderWorkflowChildrenChildren: MappingOrderTechnique = result.listMappingOrder.find(x => x.productOrderWorkflowId == childrenChildren.productOrderWorkflowId);
                      let productOrderWorkflowNameChildrenChildren = productOrderWorkflowChildrenChildren.name;
                      let productOrderWorkflowIdChildrenChildren = childrenChildren.productOrderWorkflowId;
                      childrenChildren.productOrderWorkflowName = productOrderWorkflowNameChildrenChildren;
                      childrenChildren.productOrderWorkflowId = productOrderWorkflowIdChildrenChildren;
                      childrenChildren.statusName = this.listStatusItem.find(x => x.categoryId == childrenChildren.statusId).categoryName;

                      let nodeChidrenChildren: TreeNode = { data: childrenChildren, children: [] };
                      listTechniqueRequestMapingChildrenChildren.push(nodeChidrenChildren);
                    }
                  });

                  let nodeChidren: TreeNode = { data: children, children: listTechniqueRequestMapingChildrenChildren };
                  listTechniqueRequestMapingChildren.push(nodeChidren);
                }
              });
              let node: TreeNode = { data: element, children: listTechniqueRequestMapingChildren };
              this.listProduct.push(node);
            });
          } else {
            listProductChildren.forEach(children => {
              if (children.parentId != null) {
                let listTechniqueRequestMapingChildrenChildren: TreeNode[] = [];
                let productOrderWorkflowChildren: MappingOrderTechnique = result.listMappingOrder.find(x => x.productOrderWorkflowId == children.productOrderWorkflowId);
                let productOrderWorkflowNameChildren = productOrderWorkflowChildren.name;
                let productOrderWorkflowIdChildren = children.productOrderWorkflowId;
                children.productOrderWorkflowName = productOrderWorkflowNameChildren;
                children.productOrderWorkflowId = productOrderWorkflowIdChildren;
                children.statusName = this.listStatusItem.find(x => x.categoryId == children.statusId).categoryName;

                //add by Giang
                children.grind = children.grind;
                children.stt = children.stt;
                //end

                listProductChildren.forEach(childrenChildren => {
                  if (childrenChildren.parentPartId == children.productionOrderMappingId) {
                    let productOrderWorkflowChildrenChildren: MappingOrderTechnique = result.listMappingOrder.find(x => x.productOrderWorkflowId == childrenChildren.productOrderWorkflowId);
                    let productOrderWorkflowNameChildrenChildren = productOrderWorkflowChildrenChildren.name;
                    let productOrderWorkflowIdChildrenChildren = childrenChildren.productOrderWorkflowId;
                    childrenChildren.productOrderWorkflowName = productOrderWorkflowNameChildrenChildren;
                    childrenChildren.productOrderWorkflowId = productOrderWorkflowIdChildrenChildren;
                    childrenChildren.statusName = this.listStatusItem.find(x => x.categoryId == childrenChildren.statusId).categoryName;
                    let nodeChidrenChildren: TreeNode = { data: childrenChildren, children: [] };
                    listTechniqueRequestMapingChildrenChildren.push(nodeChidrenChildren);
                  }
                });

                let nodeChidren: TreeNode = { data: children, children: listTechniqueRequestMapingChildrenChildren };
                this.listProduct.push(nodeChidren);
              }
            });
          }
        } else {
          let status: ProductionOrderStatus = this.listStatusDefaul.find(x => x.categoryId == productionOrder.statusId);
          this.productionOrderStatusControl.setValue(status);
          if (status.categoryCode == "COMP" || status.categoryCode == "CANC") {
            this.isDisabled = true;
            this.isDisabledStatus = true;
            this.isDisabledEspecially = true;
          }
          else {
            if (status.categoryCode == 'PEND') {
              this.isDisabledEspecially = true;
              this.listOrderStatus = this.listStatusDefaul.filter(x => x.categoryCode == 'CANC' || x.categoryCode == 'PROC' || x.categoryCode == 'PEND');
            } else {
              this.listOrderStatus = this.listStatusDefaul.filter(x => x.categoryCode == 'CANC' || x.categoryCode == status.categoryCode || x.categoryCode == 'PEND');
            }
            if (status.categoryCode != 'NEW') {
              this.isDisabled = true;
              this.isNew = false;
            }
          }
          // End

          // Danh sách quy trình và tiến trình của nó
          this.listMappingOrderTechnique = [];
          result.listMappingOrder.forEach(element => {
            if (element.parentId == null) {
              {
                let map = new MappingOrderTechnique();
                map.isDefault = element.isDefault;
                map.name = element.name;
                map.productOrderWorkflowId = element.productOrderWorkflowId;
                map.listTechniqueRequest = element.listTechniqueRequest;
                this.listMappingOrderTechnique.push(map);
              }
            }
          });

          // Lấy danh sách item
          let index = 0;
          let listProduct = result.listProductItem;
          let listProductParent = listProduct.filter(x => x.parentPartId == null);
          let listProductChildren = listProduct.filter(x => x.parentPartId != null);

          listProductParent.forEach(element => {
            let techniqueRequestMaping = null;
            let listTechniqueRequestMapingChildren: TreeNode[] = [];
            techniqueRequestMaping = new TechniqueRequestMaping();
            let productOrderWorkflow: MappingOrderTechnique = result.listMappingOrder.find(x => x.productOrderWorkflowId == element.productOrderWorkflowId);
            let productOrderWorkflowName = productOrderWorkflow.name;
            let productOrderWorkflowId = element.productOrderWorkflowId;
            let listTechnique = new Array<Technique>();

            element.listTechnique.forEach(item => {
              listTechnique.push(item);
            });

            // Lấy thông tín bán item
            techniqueRequestMaping.productionOrderMappingId = element.productionOrderMappingId;
            techniqueRequestMaping.index = index.toString();
            techniqueRequestMaping.productColorCode = element.productColorCode;
            techniqueRequestMaping.productColor = element.productColor;
            techniqueRequestMaping.productLength = element.productLength == null ? 0 : element.productLength;
            techniqueRequestMaping.productName = element.productName;
            techniqueRequestMaping.productThickness = element.productThickness == null ? 0 : element.productThickness;
            techniqueRequestMaping.productWidth = element.productWidth == null ? 0 : element.productWidth;
            techniqueRequestMaping.quantity = element.quantity == null ? 0 : element.quantity;
            techniqueRequestMaping.techniqueDescription = element.techniqueDescription;
            techniqueRequestMaping.totalArea = this.roundNumber(element.totalArea, 4)
            techniqueRequestMaping.productOrderWorkflowName = productOrderWorkflowName;
            techniqueRequestMaping.productOrderWorkflowId = productOrderWorkflowId;
            techniqueRequestMaping.listTechnique = listTechnique;
            techniqueRequestMaping.productCode = element.productCode;
            techniqueRequestMaping.statusId = element.statusId;
            techniqueRequestMaping.statusCode = element.statusCode;
            techniqueRequestMaping.parentPartId = element.parentPartId;
            techniqueRequestMaping.productionOrderId = element.productionOrderId;
            techniqueRequestMaping.productGroupCode = element.productGroupCode;
            techniqueRequestMaping.borehole = element.borehole;
            techniqueRequestMaping.hole = element.hole;

            //add by Giang
            techniqueRequestMaping.grind = element.grind;
            techniqueRequestMaping.stt = element.stt;
            //end

            techniqueRequestMaping.statusName = this.listStatusItem.find(x => x.categoryId == element.statusId).categoryName;

            if (techniqueRequestMaping.statusCode == 'COMP' || techniqueRequestMaping.statusCode == 'CANC') {
              this.isButtonItem = true;
            }
            if (techniqueRequestMaping.statusCode != 'NEW') {
              this.isEditItem = false;
            }
            listProductChildren.forEach(product => {

              if (element.productionOrderMappingId == product.parentPartId) {
                let techniqueRequestMapingChildren = null;
                techniqueRequestMapingChildren = new TechniqueRequestMaping();
                let listTechniqueRequestMapingChildrenChildren: TreeNode[] = [];
                let productOrderWorkflowChildren: MappingOrderTechnique = result.listMappingOrder.find(x => x.productOrderWorkflowId == product.productOrderWorkflowId);
                let productOrderWorkflowNameChildren = productOrderWorkflowChildren.name;
                let productOrderWorkflowIdChildren = productOrderWorkflowChildren.productOrderWorkflowId;
                let listTechnique = new Array<Technique>();

                product.listTechnique.forEach(item => {
                  listTechnique.push(item);
                });

                // Lấy danh sách tiên trình đặc biệt của tiến trình
                techniqueRequestMapingChildren.productionOrderMappingId = product.productionOrderMappingId;
                techniqueRequestMapingChildren.index = index.toString();
                techniqueRequestMapingChildren.productColorCode = product.productColorCode;
                techniqueRequestMapingChildren.productColor = product.productColor;
                techniqueRequestMapingChildren.productLength = product.productLength == null ? 0 : product.productLength;
                techniqueRequestMapingChildren.productName = product.productName;
                techniqueRequestMapingChildren.productThickness = product.productThickness == null ? 0 : product.productThickness;
                techniqueRequestMapingChildren.productWidth = product.productWidth == null ? 0 : product.productWidth;
                techniqueRequestMapingChildren.quantity = product.quantity == null ? 0 : product.quantity;

                techniqueRequestMapingChildren.techniqueDescription = product.techniqueDescription;
                techniqueRequestMapingChildren.totalArea = this.roundNumber(product.totalArea, 4)
                techniqueRequestMapingChildren.productOrderWorkflowName = productOrderWorkflowNameChildren;
                techniqueRequestMapingChildren.productOrderWorkflowId = productOrderWorkflowIdChildren;
                techniqueRequestMapingChildren.listTechnique = listTechnique;
                techniqueRequestMapingChildren.productCode = product.productCode;
                techniqueRequestMapingChildren.statusId = product.statusId;
                techniqueRequestMapingChildren.statusCode = product.statusCode;
                techniqueRequestMapingChildren.numberTechniqueSpecial = product.numberTechniqueSpecial;
                techniqueRequestMapingChildren.parentPartId = product.parentPartId;
                techniqueRequestMapingChildren.productionOrderId = product.productionOrderId;
                techniqueRequestMapingChildren.productGroupCode = product.productGroupCode;
                techniqueRequestMapingChildren.borehole = product.borehole;
                techniqueRequestMapingChildren.hole = product.hole;
                techniqueRequestMapingChildren.statusName = this.listStatusItem.find(x => x.categoryId == product.statusId).categoryName;

                let listChildrenChildren = listProduct.filter(x => x.parentPartId == product.productionOrderMappingId);
                if (listChildrenChildren.length > 0) {
                  listChildrenChildren.forEach(children => {
                    if (children.parentPartId == product.productionOrderMappingId) {
                      let techniqueRequestMapingChildrenChildren: TechniqueRequestMaping = new TechniqueRequestMaping();
                      let productOrderWorkflowChildrenChildren: MappingOrderTechnique = result.listMappingOrder.find(x => x.productOrderWorkflowId == children.productOrderWorkflowId);
                      let productOrderWorkflowNameChildrenChildren = productOrderWorkflowChildrenChildren.name;
                      let productOrderWorkflowIdChildrenChildren = productOrderWorkflowChildrenChildren.productOrderWorkflowId;
                      let listTechnique = new Array<Technique>();

                      children.listTechnique.forEach(item => {
                        listTechnique.push(item);
                      });

                      // Lấy danh sách tiên trình đặc biệt của tiến trình
                      techniqueRequestMapingChildrenChildren.productionOrderMappingId = children.productionOrderMappingId;
                      techniqueRequestMapingChildrenChildren.index = index.toString();
                      techniqueRequestMapingChildrenChildren.productColorCode = children.productColorCode;
                      techniqueRequestMapingChildrenChildren.productColor = children.productColor;
                      techniqueRequestMapingChildrenChildren.productLength = children.productLength == null ? 0 : children.productLength;
                      techniqueRequestMapingChildrenChildren.productName = children.productName;
                      techniqueRequestMapingChildrenChildren.productThickness = children.productThickness == null ? 0 : children.productThickness;
                      techniqueRequestMapingChildrenChildren.productWidth = children.productWidth == null ? 0 : children.productWidth;
                      techniqueRequestMapingChildrenChildren.quantity = children.quantity == null ? 0 : children.quantity;

                      techniqueRequestMapingChildrenChildren.techniqueDescription = children.techniqueDescription;
                      techniqueRequestMapingChildrenChildren.totalArea = this.roundNumber(children.totalArea, 4)
                      techniqueRequestMapingChildrenChildren.productOrderWorkflowName = productOrderWorkflowNameChildrenChildren;
                      techniqueRequestMapingChildrenChildren.productOrderWorkflowId = productOrderWorkflowIdChildrenChildren;
                      techniqueRequestMapingChildrenChildren.listTechnique = listTechnique;
                      techniqueRequestMapingChildrenChildren.productCode = children.productCode;
                      techniqueRequestMapingChildrenChildren.statusId = children.statusId;
                      techniqueRequestMapingChildrenChildren.statusCode = children.statusCode;
                      techniqueRequestMapingChildrenChildren.numberTechniqueSpecial = children.numberTechniqueSpecial;
                      techniqueRequestMapingChildrenChildren.parentPartId = children.parentPartId;
                      techniqueRequestMapingChildrenChildren.productionOrderId = children.productionOrderId;
                      techniqueRequestMapingChildrenChildren.productGroupCode = children.productGroupCode;
                      techniqueRequestMapingChildrenChildren.borehole = children.borehole;
                      techniqueRequestMapingChildrenChildren.hole = children.hole;
                      techniqueRequestMapingChildrenChildren.statusName = this.listStatusItem.find(x => x.categoryId == children.statusId).categoryName;

                      let childrenChildrenNode: TreeNode = { data: techniqueRequestMapingChildrenChildren, children: [], parent: techniqueRequestMapingChildren };
                      listTechniqueRequestMapingChildrenChildren.push(childrenChildrenNode);
                    }
                  });
                }
                let childrenNode: TreeNode = { data: techniqueRequestMapingChildren, children: listTechniqueRequestMapingChildrenChildren, parent: techniqueRequestMaping };
                listTechniqueRequestMapingChildren.push(childrenNode);
              }
            });
            let node: TreeNode = { data: techniqueRequestMaping, children: listTechniqueRequestMapingChildren };
            index = index + 1;
            this.listProduct.push(node);
          });
        }


        this.listProduct = [...this.listProduct];

        //Tính tổng số tấm và tổng số m2
        this.calculatorTotal();

        this.noteHistory = result.listNote;
        this.handleNoteContent();
        this.showTreeTable();
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
    this.productionOrderStatusControl = new FormControl(null, [Validators.required]);

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
    this.selectedBTPControl = new FormControl("BTP1", [Validators.required]);
    this.isChildren1Control = new FormControl(false);
    this.isChildren2Control = new FormControl(false);

    this.productOrderForm = new FormGroup({
      endDateControl: this.endDateControl,
      startDateControl: this.startDateControl,
      placeOfDeliveryControl: this.placeOfDeliveryControl,
      noteTechniqueControl: this.noteTechniqueControl,
      especiallyControl: this.especiallyControl,
      productionOrderStatusControl: this.productionOrderStatusControl
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
      selectedBTPControl: this.selectedBTPControl,
      isChildren1Control: this.isChildren1Control,
      isChildren2Control: this.isChildren2Control
    });

    //Form thay đổi mã nhóm
    this.productGroupCode11Control = new FormControl("");
    this.productGroupCode111Control = new FormControl("");
    this.productGroupCode112Control = new FormControl("");
    this.productGroupCode12Control = new FormControl("");
    this.productGroupCode121Control = new FormControl("");
    this.productGroupCode122Control = new FormControl("");

    this.dialogChangeGroupCodeForm = new FormGroup({
      productGroupCode11Control: this.productGroupCode11Control,
      productGroupCode111Control: this.productGroupCode111Control,
      productGroupCode112Control: this.productGroupCode112Control,
      productGroupCode12Control: this.productGroupCode12Control,
      productGroupCode121Control: this.productGroupCode121Control,
      productGroupCode122Control: this.productGroupCode122Control
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
      { field: 'to', header: 'Thao tác', display: 'table-cell', width: '200px' },
    ];

    this.colsFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left' },
      { field: 'documentSize', header: 'Kích thước', width: '50%', textAlign: 'left' },
      { field: 'updatedDate', header: 'Ngày tạo', width: '50%', textAlign: 'left' },
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

  changeInput(event) {
    if (this.actionEdit == true) {
      let data: TechniqueRequestMaping = event.data;
      this.updateData(event);
      this.updateItem(data)
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền thay đổi' };
      this.showMessage(msg);
    }
  }

  /* Sửa mã nhóm của 1 bán thành phẩm */
  changeProductGroupCodeByItem(item: any) {

    this.loading = true;
    this.manufactureService.changeProductGroupCodeByItem(item.data.productionOrderMappingId, item.data.productGroupCode).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: "Sửa thành công!" };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /* Mở dialog Sửa mã nhóm cho toàn bộ bán thành phẩm */
  changeProductGroupCode() {
    //Mở dialog
    this.isShowChangeGroupCode = true;
  }

  /* Lưu thay đổi mã nhóm */
  saveChangeGroupCode() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn thay đổi không?',
      accept: () => {
        let code_11: string = this.productGroupCode11Control.value == null ?
                                "" : this.productGroupCode11Control.value.trim();
        let code_111: string = this.productGroupCode111Control.value == null ?
                                "" : this.productGroupCode111Control.value.trim();
        let code_112: string = this.productGroupCode112Control.value == null ?
                                "" : this.productGroupCode112Control.value.trim();
        let code_12: string = this.productGroupCode12Control.value == null ?
                                "" : this.productGroupCode12Control.value.trim();
        let code_121: string = this.productGroupCode121Control.value == null ?
                                "" : this.productGroupCode121Control.value.trim();
        let code_122: string = this.productGroupCode122Control.value == null ?
                                "" : this.productGroupCode122Control.value.trim();

        this.manufactureService.changeGroupCodeForListItem(this.productionOrderId, code_11,
          code_111, code_112, code_12, code_121, code_122).subscribe(response => {
            let result: any = response;

            if (result.statusCode == 200) {
              //Load lại data của page
              this.productionOrderModel = new ProductionOrder();
              this.listOrderStatus = [];
              this.listStatusDefaul = [];
              this.listStatusItem = [];
              this.listProduct = [];
              this.getMasterData();
              //End

              let msg = { severity: 'success', summary: 'Thông báo:', detail: "Lưu thành công!" };
              this.showMessage(msg);

              //Đóng dialog
              this.isShowChangeGroupCode = false;
            } else {
              let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
        });
      }
    });
  }

  /* Hủy không lưu thay đổi mã nhóm */
  cancelChangeGroupCode() {

    //Đóng dialog
    this.isShowChangeGroupCode = false;
  }

  updateData(event) {
    if (this.actionEdit == true) {
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
                z.children.forEach(k => {
                  k.data.productLength = length;
                  k.data.quantity = quantity;
                  k.data.productWidth = width;
                  k.data.totalArea = data.totalArea;
                  k.data.hole = hole.toString();
                  k.data.borehole = borehole.toString();

                });
              });
            });
          }
        });
        this.listProduct = [...this.listProduct];
      }

      //Tính tổng số tấm và tổng số m2
      this.calculatorTotal();
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền thay đổi' };
      this.showMessage(msg);
    }
  }

  onEditComplete(event) {
    let data: TechniqueRequestMaping = event.data;
    this.updateData(event);
    this.updateItem(data);
  }

  updateItem(data: TechniqueRequestMaping) {
    if (this.actionEdit == true) {
      this.loading = true;
      this.manufactureService.updateItemInProduction(data).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          let msg = { severity: 'success', summary: 'Thông báo:', detail: "Sửa thành công!" };
          this.showMessage(msg);

          return true;
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
      return false;
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền thay đổi' };
      this.showMessage(msg);
    }
  }

  confirm(data) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa bán thành phẩm này không?',
      accept: () => {
        this.RemoveProduct(data);
      }
    });
  }

  // Lưu lệnh sản xuất
  saveProductionOrder() {
    this.listOrderStatus = this.listStatusDefaul.filter(x => x != null);
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
      this.productionOrderModel.productionOrderId = this.productionOrderId;
      let status = this.productionOrderStatusControl.value;
      this.productionOrderModel.statusId = status.categoryId;
      this.loading = true;
      this.manufactureService.updateProductionOrder(this.productionOrderModel).subscribe(response => {
        let result: any = response;
        this.loading = false;
        this.isDisabledEspecially = false;
        if (result.statusCode == 200) {
          if (result.statusProdutionCode == 'CANC' || result.statusProdutionCode == 'COMP') {
            this.isDisabled = true;
            this.isDisabledStatus = true;
            this.isDisabledEspecially = true;
          }
          else {
            if (status.categoryCode == 'PEND') {
              this.isDisabledEspecially = true;
              this.listOrderStatus = this.listStatusDefaul.filter(x => x.categoryCode == 'CANC' || x.categoryCode == 'PROC' || x.categoryCode == 'PEND');
            } else {
              this.listOrderStatus = this.listStatusDefaul.filter(x => x.categoryCode == 'CANC' || x.categoryCode == status.categoryCode || x.categoryCode == 'PEND');
            }
            if (status.categoryCode != 'NEW') {
              this.isDisabled = true;
              this.isNew = false;
            }
          }
          // Đổi trạng thái của item khi trạng thái của lệnh sản xuất thay đổi
          this.listProduct.forEach(item => {
            let description: string = item.data.techniqueDescription;
            let index = description.lastIndexOf(";");
            item.data.techniqueDescription = description.substring(0, index) + ";" + this.noteTechniqueControl.value;
            item.children.forEach(children => {
              let description1: string = children.data.techniqueDescription;
              let index1 = description1.lastIndexOf(";");
              children.data.techniqueDescription = description1.substring(0, index1) + ";" + this.noteTechniqueControl.value;
              if (children.data.statusCode != 'COMP' && children.data.statusCode != 'CANC') {
                children.data.statusCode = result.statusProdutionCode;
                children.data.statusName = this.listStatusItem.find(x => x.categoryCode == result.statusProdutionCode).categoryName;
                children.data.statusId = this.listStatusItem.find(x => x.categoryCode == result.statusProdutionCode).categoryId;
                children.children.forEach(childrenChildren => {
                  childrenChildren.data.statusCode = result.statusProdutionCode;
                  childrenChildren.data.statusName = this.listStatusItem.find(x => x.categoryCode == result.statusProdutionCode).categoryName;
                  childrenChildren.data.statusId = this.listStatusItem.find(x => x.categoryCode == result.statusProdutionCode).categoryId;

                });
              }
              children.children.forEach(childrenChildren => {
                childrenChildren.data.techniqueDescription = noteTechnique;
                let description2: string = children.data.techniqueDescription;
                let index2 = description2.lastIndexOf(";");
                children.data.techniqueDescription = description2.substring(0, index2) + ";" + this.noteTechniqueControl.value;
              });
            });
            if (item.data.statusCode != 'COMP' && item.data.statusCode != 'CANC') {
              item.data.statusCode = result.statusProdutionCode;
              item.data.statusName = this.listStatusItem.find(x => x.categoryCode == result.statusProdutionCode).categoryName;
              item.data.statusId = this.listStatusItem.find(x => x.categoryCode == result.statusProdutionCode).categoryId;
            }
          });
          this.listProduct = [...this.listProduct]
          let msg = { severity: 'success', summary: 'Thông báo:', detail: "Sửa thành công" };
          this.showMessage(msg);
        } else {
          this.productionOrderModel = new ProductionOrder();
          this.listOrderStatus = [];
          this.listStatusDefaul = [];
          this.listStatusItem = [];
          this.listProduct = [];
          this.getMasterData();
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  cancelTechniqueSpecial() {
    this.displayDialog = false;
  }

  // Thêm bán thành phẩm
  AddProduct(data) {
    let productionOrderMapping: TechniqueRequestMaping = new TechniqueRequestMaping();
    productionOrderMapping = data.node.data;
    productionOrderMapping.parentPartId = data.node.data.productionOrderMappingId;
    productionOrderMapping.productionOrderId = this.productionOrderId;
    this.loading = true;
    this.manufactureService.createItemInProduction(productionOrderMapping).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: "Thêm thành công!" };
        this.showMessage(msg);
        // this.getMasterData(); sẽ làm loading lâu hơn
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
              treeNode.productionOrderMappingId = result.productionOrderMappingId;
              treeNode.statusCode = data.node.data.statusCode;
              treeNode.statusId = data.node.data.statusId;
              treeNode.statusName = data.node.data.statusName;

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
            if (this.listProduct[i].data.productionOrderMappingId == data.node.parent.data.productionOrderMappingId) {
              for (let j = 0; j < this.listProduct[i].children.length; j++) {
                if (this.listProduct[i].children[j].data.productionOrderMappingId == data.node.data.productionOrderMappingId) {
                  if (this.listProduct[i].children[j].children == null) {
                    this.listProduct[i].children[j].children = [];
                  }
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
                  treeNode.productionOrderMappingId = result.productionOrderMappingId;
                  treeNode.statusCode = data.node.data.statusCode;
                  treeNode.statusName = data.node.data.statusName;
                  treeNode.statusId = data.node.data.statusId;
                  if (data.node.data != null) {
                    treeNode.productOrderWorkflowName = data.node.data.productOrderWorkflowName;
                    treeNode.productOrderWorkflowId = data.node.data.productOrderWorkflowId;
                    treeNode.listTechnique = data.node.data.listTechnique;
                  }
                  let demo: TreeNode = { data: {}, children: [] }
                  demo.data = treeNode;
                  this.listProduct[i].children[j].children.push(demo);
                }
              }
            }
          }
          this.listProduct = [...this.listProduct];
        }
        this.showTreeTable();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
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

  // Xóa bán thành phẩm
  RemoveProduct(data) {
    if (this.actionEdit) {
      let productionOrderMappingId = data.node.data.productionOrderMappingId;
      this.manufactureService.deleteItemInProduction(productionOrderMappingId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          let msg = { severity: 'success', summary: 'Thông báo:', detail: "Xóa thành công!" };
          this.showMessage(msg);
          for (var index = 0; index < this.listProduct.length; index++) {
            let isRemove = false;
            if (this.listProduct[index] == data.node.parent) {
              let len = this.listProduct[index].children.length;
              for (let j = 0; j < len; j++) {
                if (this.listProduct[index].children[j] == data.node && isRemove == false) {
                  this.listProduct[index].children = this.listProduct[index].children.filter(x => x != data.node);
                  isRemove = true;
                  break;
                }
              }
            }
            else {
              let len = this.listProduct[index].children.length;
              for (let j = 0; j < len; j++) {
                if (isRemove == false) {
                  if (this.listProduct[index].children[j] == data.node.parent) {
                    for (let z = 0; z < this.listProduct[index].children[j].children.length; z++) {
                      if (this.listProduct[index].children[j].children[z] == data.node) {
                        this.listProduct[index].children[j].children = this.listProduct[index].children[j].children.filter(x => x != data.node);
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
            if (isRemove) {
              break;
            }
          }
          this.listProduct = [...this.listProduct];
          this.showTreeTable();
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền thay đổi' };
      this.showMessage(msg);
    }
  }

  itemCancel(rowNode) {
    if (this.actionEdit) {
      let id = rowNode.node.data.productionOrderMappingId;
      this.loading = true;
      this.listOrderStatus = this.listStatusDefaul.filter(x => x != null);
      this.manufactureService.updateStatusItemCancel(id).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          if (result.productionOrderStatusId != null) {
            let status: ProductionOrderStatus = this.listStatusDefaul.find(x => x.categoryId == result.productionOrderStatusId);
            this.productionOrderStatusControl.setValue(status);
            if (status.categoryCode == "COMP" || status.categoryCode == "CANC") {
              this.isDisabled = true;
              this.isDisabledStatus = true;
            }
            else {
              if (status.categoryCode == 'PEND') {
                this.listOrderStatus = this.listStatusDefaul.filter(x => x.categoryCode == 'CANC' || x.categoryCode == 'PROC' || x.categoryCode == 'PEND');
              } else {
                this.listOrderStatus = this.listStatusDefaul.filter(x => x.categoryCode == 'CANC' || x.categoryCode == status.categoryCode || x.categoryCode == 'PEND');
              }
              if (status.categoryCode != 'NEW') {
                this.isDisabled = true;
                this.isNew = false;
              }
            }
          }
          if (rowNode.node.children.length > 0) {
            this.listProduct.forEach(item => {
              if (item.data.productionOrderMappingId == id) {
                item.children.forEach(element => {
                  element.data.statusCode = 'CANC';
                  element.data.statusName = this.listStatusItem.find(x => x.categoryCode == 'CANC').categoryName;
                  element.data.statusId = this.listStatusItem.find(x => x.categoryCode == 'CANC').categoryId;
                  element.children.forEach(children => {
                    children.data.statusCode = 'CANC';
                    children.data.statusName = this.listStatusItem.find(x => x.categoryCode == 'CANC').categoryName;
                    children.data.statusId = this.listStatusItem.find(x => x.categoryCode == 'CANC').categoryId;
                    children.children.forEach(childrenChildren => {
                      childrenChildren.data.statusCode = 'CANC';
                      childrenChildren.data.statusName = this.listStatusItem.find(x => x.categoryCode == 'CANC').categoryName;
                      childrenChildren.data.statusId = this.listStatusItem.find(x => x.categoryCode == 'CANC').categoryId;
                    });
                  });
                });
              }
            });
          }

          rowNode.node.data.statusCode = 'CANC';
          rowNode.node.data.statusName = this.listStatusItem.find(x => x.categoryCode == 'CANC').categoryName;
          rowNode.node.data.statusId = this.listStatusItem.find(x => x.categoryCode == 'CANC').categoryId;
          this.isEditItem = false;
          this.listProduct = [...this.listProduct]
          let msg = { severity: 'success', summary: 'Thông báo:', detail: "Sửa thành công!" };
          this.showMessage(msg);
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền thay đổi' };
      this.showMessage(msg);
    }
  }

  itemWorking(rowNode) {
    if (this.actionEdit) {
      this.listOrderStatus = this.listStatusDefaul.filter(x => x != null);
      let id = rowNode.node.data.productionOrderMappingId;
      this.loading = true;
      this.manufactureService.updateStatusItemWorking(id).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          if (result.productionOrderStatusId != null) {
            let status: ProductionOrderStatus = this.listStatusDefaul.find(x => x.categoryId == result.productionOrderStatusId);

            if (status.categoryCode == "COMP" || status.categoryCode == "CANC") {
              this.isDisabled = true;
              this.isDisabledStatus = true;
            }
            else {
              if (status.categoryCode == 'PEND') {
                this.listOrderStatus = this.listStatusDefaul.filter(x => x.categoryCode == 'CANC' || x.categoryCode == 'PROC' || x.categoryCode == 'PEND');
              } else {
                this.listOrderStatus = this.listStatusDefaul.filter(x => x.categoryCode == 'CANC' || x.categoryCode == status.categoryCode || x.categoryCode == 'PEND');
              }
              if (status.categoryCode != 'NEW') {
                this.isDisabled = true;
                this.isNew = false;
              }
            }
            this.productionOrderStatusControl.setValue(status);
          }

          if (rowNode.node.children.length > 0) {
            this.listProduct.forEach(item => {
              if (item.data.productionOrderMappingId == id) {
                item.children.forEach(element => {
                  element.data.statusCode = 'PROC';
                  element.data.statusName = this.listStatusItem.find(x => x.categoryCode == 'PROC').categoryName;
                  element.data.statusId = this.listStatusItem.find(x => x.categoryCode == 'PROC').categoryId;
                  element.children.forEach(children => {
                    children.data.statusCode = 'PROC';
                    children.data.statusName = this.listStatusItem.find(x => x.categoryCode == 'PROC').categoryName;
                    children.data.statusId = this.listStatusItem.find(x => x.categoryCode == 'PROC').categoryId;
                    children.children.forEach(childrenChildren => {
                      childrenChildren.data.statusCode = 'PROC';
                      childrenChildren.data.statusName = this.listStatusItem.find(x => x.categoryCode == 'PROC').categoryName;
                      childrenChildren.data.statusId = this.listStatusItem.find(x => x.categoryCode == 'PROC').categoryId;
                    });
                  });
                });
              }
            });
          }
          rowNode.node.data.statusCode = 'PROC';
          rowNode.node.data.statusName = this.listStatusItem.find(x => x.categoryCode == 'PROC').categoryName;
          rowNode.node.data.statusId = this.listStatusItem.find(x => x.categoryCode == 'PROC').categoryId;
          this.isEditItem = false;
          this.listProduct = [...this.listProduct]
          let msg = { severity: 'success', summary: 'Thông báo:', detail: "Sửa thành công!" };
          this.showMessage(msg);
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền thay đổi' };
      this.showMessage(msg);
    }
  }

  itemStop(rowNode) {
    if (this.actionEdit) {
      this.listOrderStatus = this.listStatusDefaul.filter(x => x != null);
      let id = rowNode.node.data.productionOrderMappingId;
      this.loading = true;
      this.manufactureService.updateStatusItemStop(id).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          // Đổi trạng thái của các item con nếu có
          if (rowNode.node.children.length > 0) {
            this.listProduct.forEach(item => {
              if (item.data.productionOrderMappingId == id) {
                item.children.forEach(element => {
                  element.data.statusCode = 'PEND';
                  element.data.statusName = this.listStatusItem.find(x => x.categoryCode == 'PEND').categoryName;
                  element.data.statusId = this.listStatusItem.find(x => x.categoryCode == 'PEND').categoryId;
                  element.children.forEach(children => {
                    children.data.statusCode = 'PEND';
                    children.data.statusName = this.listStatusItem.find(x => x.categoryCode == 'PEND').categoryName;
                    children.data.statusId = this.listStatusItem.find(x => x.categoryCode == 'PEND').categoryId;
                    children.children.forEach(childrenChildren => {
                      childrenChildren.data.statusCode = 'PEND';
                      childrenChildren.data.statusName = this.listStatusItem.find(x => x.categoryCode == 'PEND').categoryName;
                      childrenChildren.data.statusId = this.listStatusItem.find(x => x.categoryCode == 'PEND').categoryId;
                    });
                  });
                });
              }
            });
          }
          rowNode.node.data.statusCode = 'PEND';
          rowNode.node.data.statusName = this.listStatusItem.find(x => x.categoryCode == 'PEND').categoryName;
          rowNode.node.data.statusId = this.listStatusItem.find(x => x.categoryCode == 'PEND').categoryId;
          this.listProduct = [...this.listProduct]

          if (result.productionOrderStatusId != null) {
            let status: ProductionOrderStatus = this.listStatusDefaul.find(x => x.categoryId == result.productionOrderStatusId);

            if (status.categoryCode == "COMP" || status.categoryCode == "CANC") {
              this.isDisabled = true;
              this.isDisabledStatus = true;
            }
            else {
              if (status.categoryCode == 'PEND') {
                this.listOrderStatus = this.listStatusDefaul.filter(x => x.categoryCode == 'CANC' || x.categoryCode == 'PROC' || x.categoryCode == 'PEND');
              } else {
                this.listOrderStatus = this.listStatusDefaul.filter(x => x.categoryCode == 'CANC' || x.categoryCode == status.categoryCode || x.categoryCode == 'PEND');
              }
            }
            this.productionOrderStatusControl.setValue(status);
          }
          this.isEditItem = false;
          let msg = { severity: 'success', summary: 'Thông báo:', detail: "Sửa thành công!" };
          this.showMessage(msg);
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền thay đổi' };
      this.showMessage(msg);
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
    if (data.data.statusCode == 'NEW') {
      this.isSaveSession = true;
    } else {
      this.isSaveSession = false;
    }
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

  showSessionChildren(childrenNode, node) {
    this.isSession = true;
    this.rowSelected = childrenNode;
    let treeNode: TreeNode = { data: {}, children: [] };
    this.parentNode = node;
    this.selectedProduct = [];
    treeNode.data = childrenNode.data;
    if (node.data.statusCode == 'NEW') {
      this.isSaveSession = true;
    }
    else {
      this.isSaveSession = false;
    }
    treeNode.partialSelected = true;
    this.selectedProduct.push(treeNode);
    this.isAddAll = false;
    this.selectedProductOrderWorkflow = this.listMappingOrderTechnique.find(x => x.productOrderWorkflowId == childrenNode.data.productOrderWorkflowId);
  }

  addOrSaveAll() {
    this.isSession = true;
    if (this.defaultProductOrderWorkflow != null) {
      this.selectedProductOrderWorkflow = this.defaultProductOrderWorkflow;
    }
    this.isAddAll = false;
  }
  // Dialog
  showDiaLog() {
    this.isShowDialog = true;
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

  applyAll() {
    let isAdd = true;
    let btp = this.selectedBTPControl.value;
    if (!this.dialogForm.valid) {
      Object.keys(this.dialogForm.controls).forEach(key => {
        if (this.dialogForm.controls[key].valid == false)
        {
          if (key == "productOrderWorkflowBTPControl")
          {
            isAdd = false;
          }
          if (btp == 'BTP1')
          {
            if (key == 'productName11Control' || key == 'productName12Control' ||
              key == 'productThickness11Control' || key == 'productThickness12Control' ||
              key == 'productOrderWorkflowBTP11Control' || key == 'productOrderWorkflowBTP12Control')
            {
              isAdd = false;
            }
            else
            {
              isAdd = true;
            }
          }
          else
          {
            if (!this.isChildren1Control.value && !this.isChildren2Control.value)
            {
              isAdd = false;
            }
            else
            {
              if (this.isChildren1Control.value &&
                  (key == 'productName111Control' || key == 'productName112Control' ||
                  key == 'productThickness111Control' || key == 'productThickness112Control' ||
                  key == 'productOrderWorkflowBTP111Control' || key == 'productOrderWorkflowBTP112Control'))
              {
                isAdd = false;
              }
              else
              {
                isAdd = true;
              }

              if (isAdd)
              {
                if (this.isChildren2Control.value &&
                  (key == 'productName121Control' || key == 'productName122Control' ||
                  key == 'productThickness121Control' || key == 'productThickness122Control' ||
                  key == 'productOrderWorkflowBTP121Control' || key == 'productOrderWorkflowBTP122Control'))
                {
                  isAdd = false;
                }
                else
                {
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
      let btp1:TechniqueRequestMaping = new TechniqueRequestMaping();
      btp1.listTechnique = this.productOrderWorkflowBTPControl.value.listTechniqueRequest;
      btp1.productOrderWorkflowId = this.productOrderWorkflowBTPControl.value.productOrderWorkflowId;
      btp1.productOrderWorkflowName = this.productOrderWorkflowBTPControl.value.name;

      let listBTP1: Array<TechniqueRequestMaping> = [];
      let listBTP2: Array<TechniqueRequestMaping> = [];
      let btp11: TechniqueRequestMaping = new TechniqueRequestMaping();
      btp11.listTechnique = this.productOrderWorkflowBTP11Control.value.listTechniqueRequest;
      btp11.productName = this.productName11Control.value;
      btp11.productThickness = this.productThickness11Control.value;
      btp11.productOrderWorkflowId = this.productOrderWorkflowBTP11Control.value.productOrderWorkflowId;
      btp11.productOrderWorkflowName = this.productOrderWorkflowBTP11Control.value.productOrderWorkflowId;
      btp11.index = "btp11";
      listBTP1.push(btp11);

      let btp12: TechniqueRequestMaping = new TechniqueRequestMaping();
      btp12.listTechnique = this.productOrderWorkflowBTP12Control.value.listTechniqueRequest;
      btp12.productName = this.productName12Control.value;
      btp12.productThickness = this.productThickness12Control.value;
      btp12.productOrderWorkflowId = this.productOrderWorkflowBTP12Control.value.productOrderWorkflowId;
      btp12.productOrderWorkflowName = this.productOrderWorkflowBTP12Control.value.name;
      btp12.index = "btp12";
      listBTP1.push(btp12);
      if (btp == "BTP2" && this.isChildren1Control.value) {
        let btp111: TechniqueRequestMaping = new TechniqueRequestMaping();
        btp111.listTechnique = this.productOrderWorkflowBTP111Control.value.listTechniqueRequest;
        btp111.productName = this.productName111Control.value;
        btp111.productThickness = this.productThickness111Control.value;
        btp111.productOrderWorkflowId = this.productOrderWorkflowBTP111Control.value.productOrderWorkflowId;
        btp111.productOrderWorkflowName = this.productOrderWorkflowBTP111Control.value.name;
        btp111.parentIndex ="btp11";
        listBTP2.push(btp111);

        let btp112: TechniqueRequestMaping = new TechniqueRequestMaping();
        btp112.listTechnique = this.productOrderWorkflowBTP112Control.value.listTechniqueRequest;
        btp112.productName = this.productName112Control.value;
        btp112.productThickness = this.productThickness112Control.value;
        btp112.productOrderWorkflowId = this.productOrderWorkflowBTP112Control.value.productOrderWorkflowId;
        btp112.productOrderWorkflowName = this.productOrderWorkflowBTP112Control.value.name;
        btp112.parentIndex ="btp11";
        listBTP2.push(btp112);
      }

      if (btp == "BTP2" && this.isChildren2Control.value) {
        let btp121: TechniqueRequestMaping = new TechniqueRequestMaping();
        btp121.listTechnique = this.productOrderWorkflowBTP121Control.value.listTechniqueRequest;
        btp121.productName = this.productName121Control.value;
        btp121.productThickness = this.productThickness121Control.value;
        btp121.productOrderWorkflowId = this.productOrderWorkflowBTP121Control.value.productOrderWorkflowId;
        btp121.productOrderWorkflowName = this.productOrderWorkflowBTP121Control.value.name;
        btp121.parentIndex ="btp12";
        listBTP2.push(btp121);

        let btp122: TechniqueRequestMaping = new TechniqueRequestMaping();
        btp122.listTechnique = this.productOrderWorkflowBTP122Control.value.listTechniqueRequest;
        btp122.productName = this.productName122Control.value;
        btp122.productThickness = this.productThickness122Control.value;
        btp122.productOrderWorkflowId = this.productOrderWorkflowBTP122Control.value.productOrderWorkflowId;
        btp122.productOrderWorkflowName = this.productOrderWorkflowBTP122Control.value.name;
        btp122.parentIndex ="btp12";
        listBTP2.push(btp122);
      }
      this.loading = true;
      this.manufactureService.createAllBTP(this.productionOrderId,btp1,listBTP1,listBTP2).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          let msg = { severity: 'success', summary: 'Thông báo:', detail: "Thêm thành công!" };
          this.showMessage(msg);
          this.isShowDialog = false;
          this.productionOrderModel = new ProductionOrder();
          this.listOrderStatus = [];
          this.listStatusDefaul = [];
          this.listStatusItem = [];
          this.listProduct = [];
          this.getMasterData();
        }
        else{
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });

    }
  }

  resize(event) {
    if (event.target.className.includes("pi pi-window-maximize")) {
      this.isOverFlow = true;
    } else if (event.target.className.includes("pi pi-window-minimize")) {
      this.isOverFlow = false;
    }
  }
  // End

  // Lưu lại thông tin tiến trình và quy trình vào item
  saveSession() {
    if (this.actionEdit == true) {
      // Khi mà không chọn tiến trình nào báo lỗi
      // Lấy danh sách tất cả các tiến trình đặc biệt
      var listProduct: Array<TechniqueRequestMaping> = this.selectedProduct.map(item => item.data);
      listProduct.forEach(item => {
        item.productOrderWorkflowId = this.selectedProductOrderWorkflow.productOrderWorkflowId;
        item.productOrderWorkflowName = this.selectedProductOrderWorkflow.name;
        item.listTechnique = this.selectedProductOrderWorkflow.listTechniqueRequest;
      });
      this.manufactureService.updateWorkFlowForProductionOrder(listProduct).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.isEditItem = false;
          let msg = { severity: 'success', summary: 'Thông báo:', detail: "Sửa thành công!" };
          this.showMessage(msg);
          this.selectedProduct.forEach(item => {
            item.data.productOrderWorkflowId = this.selectedProductOrderWorkflow.productOrderWorkflowId;
            item.data.productOrderWorkflowName = this.selectedProductOrderWorkflow.name;
            item.data.listTechnique = this.selectedProductOrderWorkflow.listTechniqueRequest;
          });
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
          this.productionOrderModel = new ProductionOrder();
          this.listOrderStatus = [];
          this.listStatusDefaul = [];
          this.listStatusItem = [];
          this.listProduct = [];
          this.getMasterData();
        }
      });
      this.cancelSession();

      if (this.selectedProduct.length > 1) {
        this.isAddAll = true;
      }
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền thay đổi' };
      this.showMessage(msg);
    }
  }

  // Tick check box item ẩn và hiện nút thêm nhiều
  selectedListProduct() {
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

  unSelectedListProduct() {
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

  /*Event thay đổi nội dung ghi chú*/
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  /*Event Thêm các file được chọn vào list file*/
  handleFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= 10000000) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedFiles.push(file);
          }
        }
      }
    }
  }

  /*Event Khi click xóa từng file*/
  removeFile(event) {
    let index = this.uploadedFiles.indexOf(event.file);
    this.uploadedFiles.splice(index, 1);
  }

  /*Event Khi click xóa toàn bộ file*/
  clearAllFile() {
    this.uploadedFiles = [];
  }

  /*Hủy sửa ghi chú*/
  cancelNote() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn hủy ghi chú này?',
      accept: () => {
        this.noteId = null;
        this.noteContent = null;
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.listUpdateNoteDocument = [];
        this.isEditNote = false;
      }
    });
  }

  /*Lưu file và ghi chú vào Db*/
  async saveNote() {
    this.loading = true;
    this.listNoteDocumentModel = [];

    /*Upload file mới nếu có*/
    if (this.uploadedFiles.length > 0) {
      let listFileNameExists: Array<FileNameExists> = [];
      let result: any = await this.imageService.uploadFileForOptionAsync(this.uploadedFiles, 'ProductionOrder');

      listFileNameExists = result.listFileNameExists;

      for (var x = 0; x < this.uploadedFiles.length; ++x) {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = this.uploadedFiles[x].name;
        let fileExists = listFileNameExists.find(f => f.oldFileName == this.uploadedFiles[x].name);
        if (fileExists) {
          noteDocument.DocumentName = fileExists.newFileName;
        }
        noteDocument.DocumentSize = this.uploadedFiles[x].size.toString();
        this.listNoteDocumentModel.push(noteDocument);
      }
    }

    let noteModel = new NoteModel();

    if (!this.noteId) {
      /*Tạo mới ghi chú*/
      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.productionOrderId;
      noteModel.ObjectType = 'PROOR';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/

      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.productionOrderId;
      noteModel.ObjectType = 'PROOR';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();

      //Thêm file cũ đã lưu nếu có
      this.listUpdateNoteDocument.forEach(item => {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = item.documentName;
        noteDocument.DocumentSize = item.documentSize;
        noteDocument.UpdatedById = item.updatedById;
        noteDocument.UpdatedDate = item.updatedDate;

        this.listNoteDocumentModel.push(noteDocument);
      });
    }

    this.noteService.createNoteForProductionOrderDetail(noteModel, this.listNoteDocumentModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.noteContent = null;
        this.listUpdateNoteDocument = [];
        this.noteId = null;
        this.isEditNote = false;

        /*Reshow Time Line*/
        this.noteHistory = result.listNote;
        this.handleNoteContent();

        let msg = { severity: 'success', summary: 'Thông báo:', detail: "Lưu ghi chú thành công" };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Xử lý và hiển thị lại nội dung ghi chú*/
  handleNoteContent() {
    this.noteHistory.forEach(element => {
      setTimeout(() => {
        let count = 0;
        if (element.description == null) {
          element.description = "";
        }

        let des = $.parseHTML(element.description);
        let newTextContent = '';
        for (let i = 0; i < des.length; i++) {
          count += des[i].textContent.length;
          newTextContent += des[i].textContent;
        }

        if (count > 250) {
          newTextContent = newTextContent.substr(0, 250) + '<b>...</b>';
          $('#' + element.noteId).find('.short-content').append($.parseHTML(newTextContent));
        } else {
          $('#' + element.noteId).find('.short-content').append($.parseHTML(element.description));
        }

        // $('#' + element.noteId).find('.note-title').append($.parseHTML(element.noteTitle));
        $('#' + element.noteId).find('.full-content').append($.parseHTML(element.description));
      }, 1000);
    });
  }
  /*End*/

  /*Event Mở rộng/Thu gọn nội dung của ghi chú*/
  toggle_note_label: string = 'Mở rộng';
  trigger_node(nodeid: string, event) {
    // noteContent
    let shortcontent_ = $('#' + nodeid).find('.short-content');
    let fullcontent_ = $('#' + nodeid).find('.full-content');
    if (shortcontent_.css("display") === "none") {
      fullcontent_.css("display", "none");
      shortcontent_.css("display", "block");
    } else {
      fullcontent_.css("display", "block");
      shortcontent_.css("display", "none");
    }
    // noteFile
    let shortcontent_file = $('#' + nodeid).find('.short-content-file');
    let fullcontent_file = $('#' + nodeid).find('.full-content-file');
    let continue_ = $('#' + nodeid).find('.continue')
    if (shortcontent_file.css("display") === "none") {
      continue_.css("display", "block");
      fullcontent_file.css("display", "none");
      shortcontent_file.css("display", "block");
    } else {
      continue_.css("display", "none");
      fullcontent_file.css("display", "block");
      shortcontent_file.css("display", "none");
    }
    let curr = $(event.target);

    if (curr.attr('class').indexOf('pi-chevron-right') != -1) {
      this.toggle_note_label = 'Thu gọn';
      curr.removeClass('pi-chevron-right');
      curr.addClass('pi-chevron-down');
    } else {
      this.toggle_note_label = 'Mở rộng';
      curr.removeClass('pi-chevron-down');
      curr.addClass('pi-chevron-right');
    }
  }
  /*End */

  /*Kiểm tra noteText > 250 ký tự hoặc noteDocument > 3 thì ẩn đi một phần nội dung*/
  tooLong(note): boolean {
    if (note.noteDocList.length > 3) return true;
    var des = $.parseHTML(note.description);
    var count = 0;
    for (var i = 0; i < des.length; i++) {
      count += des[i].textContent.length;
      if (count > 250) return true;
    }
    return false;
  }

  openItem(name, url) {
    this.imageService.downloadFile(name, url).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(file);
      } else {
        var fileURL = URL.createObjectURL(file);
        if (fileType.indexOf('image') !== -1) {
          window.open(fileURL);
        } else {
          var anchor = document.createElement("a");
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    }, error => { });
  }

  /*Event Sửa ghi chú*/
  onClickEditNote(noteId: string, noteDes: string) {
    this.noteContent = noteDes;
    this.noteId = noteId;
    this.listUpdateNoteDocument = this.noteHistory.find(x => x.noteId == this.noteId).noteDocList;
    this.isEditNote = true;
  }
  /*End*/

  /*Event Xóa ghi chú*/
  onClickDeleteNote(noteId: string) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa ghi chú này?',
      accept: () => {
        this.loading = true;
        this.noteService.disableNote(noteId).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            let note = this.noteHistory.find(x => x.noteId == noteId);
            let index = this.noteHistory.lastIndexOf(note);
            this.noteHistory.splice(index, 1);

            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa ghi chú thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }
  /*End*/

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: NoteDocument) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listUpdateNoteDocument.indexOf(file);
        this.listUpdateNoteDocument.splice(index, 1);
      }
    });
  }

  /*Event khi download 1 file đã lưu trên server*/
  downloadFile(fileInfor: NoteDocument) {
    this.imageService.downloadFile(fileInfor.documentName, fileInfor.documentUrl).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;
      var name = fileInfor.documentName;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(file);
      } else {
        var fileURL = URL.createObjectURL(file);
        if (fileType.indexOf('image') !== -1) {
          window.open(fileURL);
        } else {
          var anchor = document.createElement("a");
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    });
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  goToProductionOrderList() {
    this.router.navigate(['/manufacture/production-order/list']);
  }

  goToOrderList() {
    this.router.navigate(['/manufacture/production-order/list']);
  }

  exportExcel() {
    let result = this.getInforExportExcel();

    let workBook = new Workbook();
    let title = `LỆNH SẢN XUẤT`;
    let worksheet = workBook.addWorksheet(title);

    let src1 = 'data:image/png;base64,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'

    let imgLogo1 = workBook.addImage({
      base64: src1,
      extension: 'jpeg',
    });

    worksheet.addImage(imgLogo1, 'A1:B7');

    let companyName = 'CÔNG TY CP KÍNH KALA';
    let dataRow1 = [];
    dataRow1[3] = companyName;
    let row1 = worksheet.addRow(dataRow1);
    row1.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`C${row1.number}:I${row1.number}`);
    row1.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let address = 'Km 15+300,Quốc lộ 1A (cũ),';
    let dataRow2 = [];
    dataRow2[3] = address;
    let row2 = worksheet.addRow(dataRow2);
    row2.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`C${row2.number}:I${row2.number}`);
    row2.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let region = 'Liên Ninh, Thanh Trì, Hà Nội.';
    let dataRow3 = [];
    dataRow3[3] = region;
    let row3 = worksheet.addRow(dataRow3);
    row3.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`C${row3.number}:I${row3.number}`);
    row3.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let phone = 'Tel: 0243.689.0198   Fax: 0243.686.3184';
    let dataRow4 = [];
    dataRow4[3] = phone;
    let row4 = worksheet.addRow(dataRow4);
    row4.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`C${row4.number}:I${row4.number}`);
    row4.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
    worksheet.addRow([]);
    worksheet.addRow([]);
    worksheet.addRow([]);
    worksheet.addRow([]);
    /* title */
    let titleRow = worksheet.addRow([title]);
    titleRow.font = { family: 4, size: 16, bold: true };
    titleRow.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A${titleRow.number}:K${titleRow.number}`);
    titleRow.height = 25;
    worksheet.addRow([]);
    //lệnh số
    let codeRowData = `Lệnh số:   ` + result.productionOrderCode;
    let descRow2 = worksheet.addRow([codeRowData]);
    worksheet.mergeCells(`A${descRow2.number}:K${descRow2.number}`);
    descRow2.alignment = { vertical: 'middle', horizontal: 'right' };
    worksheet.addRow([]);
    //bang thong tin khach hang
    let customerData_row1 = ["Khách hàng", "Khách hàng", result.customerInfor.customerName, "Ngày nhận", "Ngày nhận", result.customerInfor.receivedDate, result.customerInfor.receivedDate];
    let customerData_row2 = ["Số ĐH của khách hàng", "Số ĐH của khách hàng", result.customerInfor.customerNumber, "Ngày sản xuất", "Ngày sản xuất", result.customerInfor.startDate, result.customerInfor.startDate];
    let customerData_row3 = ["Địa điểm trả hàng", "Địa điểm trả hàng", result.customerInfor.placeOfDelivery, "Ngày dự kiến trả", "Ngày dự kiến trả", result.customerInfor.endDate, result.customerInfor.endDate];

    let customer_row1 = worksheet.addRow(customerData_row1);
    let customer_row2 = worksheet.addRow(customerData_row2);
    let customer_row3 = worksheet.addRow(customerData_row3);

    customerData_row1.forEach((item, index) => {
      customer_row1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      customer_row1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
    });
    customerData_row2.forEach((item, index) => {
      customer_row2.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      customer_row2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
    });
    customerData_row3.forEach((item, index) => {
      customer_row3.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      customer_row3.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
    });

    worksheet.mergeCells(`A${customer_row1.number}:B${customer_row1.number}`);
    worksheet.mergeCells(`A${customer_row2.number}:B${customer_row2.number}`);
    worksheet.mergeCells(`A${customer_row3.number}:B${customer_row3.number}`);

    worksheet.mergeCells(`C${customer_row1.number}:D${customer_row1.number}`);
    worksheet.mergeCells(`C${customer_row2.number}:D${customer_row2.number}`);
    worksheet.mergeCells(`C${customer_row3.number}:D${customer_row3.number}`);

    worksheet.mergeCells(`F${customer_row1.number}:G${customer_row1.number}`);
    worksheet.mergeCells(`F${customer_row2.number}:G${customer_row2.number}`);
    worksheet.mergeCells(`F${customer_row3.number}:G${customer_row3.number}`);

    worksheet.addRow([]);
    worksheet.addRow([]);
    //ghi chu
    let noteRowData = `Ghi chú KT:   ` + result.customerInfor.noteTechnique;
    let noteRow = worksheet.addRow([noteRowData]);
    worksheet.mergeCells(`A${noteRow.number}:K${noteRow.number}`);
    noteRow.alignment = { vertical: 'middle', horizontal: 'left' };
    worksheet.addRow([]);
    //product infor
    /* header */
    let productHeader1 = ['STT', 'Chủng Loại', 'Màu sắc', 'Độ dày(mm)', 'Kích thước(mm)', '',
     'Số tấm', 'Tổng số(m2)', 'Mài','Khoan', 'Khoét', 'Mã hiệu'];
    let productHeader2 = ['', '', '', '', 'Dài', 'Rộng', '', '', '', '', '', ''];
    let headerRow1 = worksheet.addRow(productHeader1);
    let headerRow2 = worksheet.addRow(productHeader2);
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
    headerRow2.font = { name: 'Time New Roman', size: 10, bold: true };
    /* merge header */
    worksheet.mergeCells(`A${20}:A${21}`);
    worksheet.mergeCells(`B${20}:B${21}`);
    worksheet.mergeCells(`C${20}:C${21}`);
    worksheet.mergeCells(`D${20}:D${21}`);
    worksheet.mergeCells(`E${20}:F${20}`);
    worksheet.mergeCells(`G${20}:G${21}`);
    worksheet.mergeCells(`H${20}:H${21}`);
    worksheet.mergeCells(`I${20}:I${21}`);
    worksheet.mergeCells(`J${20}:J${21}`);
    worksheet.mergeCells(`K${20}:K${21}`);
    worksheet.mergeCells(`L${20}:L${21}`);

    productHeader1.forEach((item, index) => {
      headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    });
    headerRow1.height = 40;

    productHeader2.forEach((item, index) => {
      headerRow2.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    });
    headerRow2.height = 40;
    //data of product infor
    let data: Array<any> = [];
    result.listProductInfor.forEach(e => {
      let row: Array<any> = [];
      row[0] = e.index
      row[1] = e.productName;
      row[2] = e.productColor;
      row[3] = e.productThickness
      row[4] = e.productLength;
      row[5] = e.productWidth;
      row[6] = e.quantity;
      row[7] = e.totalArea;
      row[8] = e.grind;
      row[9] = e.borehole;
      row[10] = e.hole;
      row[11] = e.techniqueDescription;
      data.push(row);
    });
    data.forEach(e => {
      let row = worksheet.addRow(e);
      let totalColumns = productHeader1.length;
      for (let i = 1; i <= totalColumns; i++) {
        row.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }

      row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      row.getCell(12).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
    });
    //sumary section
    let sumaryData: Array<any> = [];
    sumaryData[0] = "Tổng";
    sumaryData[1] = '';
    sumaryData[2] = '';
    sumaryData[3] = '';
    sumaryData[4] = '';
    sumaryData[5] = '';
    sumaryData[6] = this.transformNumber(result.sumaryProduct.quantity);
    sumaryData[7] = this.transformNumber(result.sumaryProduct.totalArea);
    sumaryData[8] = this.transformNumber(result.sumaryProduct.grind);
    sumaryData[9] = this.transformNumber(result.sumaryProduct.borehole);
    sumaryData[10] = this.transformNumber(result.sumaryProduct.hole);
    sumaryData[11] = '';
    let sumaryRow = worksheet.addRow(sumaryData);
    let totalColumns = productHeader1.length;
    for (let i = 1; i <= totalColumns; i++) {
      sumaryRow.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    }

    sumaryRow.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    sumaryRow.getCell(2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    sumaryRow.getCell(3).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    sumaryRow.getCell(4).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    sumaryRow.getCell(5).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    sumaryRow.getCell(6).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    sumaryRow.getCell(7).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    sumaryRow.getCell(8).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    sumaryRow.getCell(9).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    sumaryRow.getCell(10).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    sumaryRow.getCell(11).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

    worksheet.mergeCells(`A${sumaryRow.number}:F${sumaryRow.number}`);

    let width = productHeader1.map((e: any) => e.length);
    productHeader1.forEach((e, index) => {
      if (e != null) {
        if (e.length > width[index]) {
          width[index] = e.length;
        }
      }
    });
    data.forEach(row => {
      row.forEach((cell, index) => {
        if (cell != null) {
          if (cell.length > width[index])
            width[index] = cell.length;
        }
      });
    });
    width.forEach((el, index) => {
      worksheet.getColumn(index + 1).width = el + 5;
    });

    this.exportToExel(workBook, title);
  }

  getInforExportExcel(): ExportExcelModel {
    let result = new ExportExcelModel();
    //sumary
    result.sumaryProduct = new SumaryProductModel();
    result.productionOrderCode = ConvertToString(this.productionOrderModel.productionOrderCode);

    let placeOfDelivery: string = this.placeOfDeliveryControl.value;
    let startDate: Date = this.startDateControl.value;
    let endDate: Date = this.endDateControl.value;
    let noteTechnique: string = this.noteTechniqueControl.value;

    result.customerInfor = new CustomerInforModel();
    result.customerInfor.customerName = ConvertToString(this.productionOrderModel.customerName);
    result.customerInfor.customerNumber = ConvertToString(this.productionOrderModel.customerNumber);
    result.customerInfor.placeOfDelivery = ConvertToString(placeOfDelivery);
    result.customerInfor.receivedDate = this.transformDate(this.productionOrderModel.receivedDate);
    result.customerInfor.startDate = this.transformDate(startDate);
    result.customerInfor.endDate = this.transformDate(endDate);
    result.customerInfor.noteTechnique = ConvertToString(noteTechnique);

    this.listProduct.forEach((e, index) => {
      let data: TechniqueRequestMaping = e.data;
      let productInfor = new ProductInfor();
      productInfor.index = data.stt;
      productInfor.productName = ConvertToString(data.productName);
      productInfor.productColor = ConvertToString(data.productColor);
      productInfor.productThickness = this.transformNumber(ParseStringToFloat(data.productThickness));
      productInfor.productLength = this.transformNumber(ParseStringToFloat(data.productLength));
      productInfor.productWidth = this.transformNumber(ParseStringToFloat(data.productWidth));
      productInfor.quantity = this.transformNumber(ParseStringToFloat(data.quantity));
      productInfor.totalArea = this.transformNumber(ParseStringToFloat(data.totalArea));
      productInfor.grind = this.transformNumber(ParseStringToFloat(data.grind == null ? 0: data.grind));
      productInfor.borehole = this.transformNumber(ParseStringToFloat(data.borehole));
      productInfor.hole = this.transformNumber(ParseStringToFloat(data.hole));
      productInfor.techniqueDescription = this.getTechniqueDescription(data.techniqueDescription);
      result.listProductInfor = [...result.listProductInfor, productInfor];

      result.sumaryProduct.quantity += ParseStringToFloat(data.quantity);
      result.sumaryProduct.totalArea += ParseStringToFloat(data.totalArea);
      result.sumaryProduct.grind += ParseStringToFloat(data.grind == null ? 0 : data.grind);
      result.sumaryProduct.borehole += ParseStringToFloat(data.borehole);
      result.sumaryProduct.hole += ParseStringToFloat(data.hole);
    });
    return result;
  }

  getTechniqueDescription(techniqueDescription: any){
    if (!techniqueDescription) techniqueDescription = '';
    let listStr: Array<string> = techniqueDescription.toString().split(';');
    return listStr[0];
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
      case 5: {
        result = Math.round(number * 100000) / 100000;
        break;
      }
      default: {
        result = result;
        break;
      }
    }
    return result;
  }

  transformDate(date: Date) {
    if (!date) return '';
    return formatDate(date, 'dd/MM/yyyy', 'EN');
  }

  transformNumber(input: any) {
    return formatNumber(input, 'EN', '1.0-4');
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

function ParseStringToFloat(str: any) {
  if (str === "") return 0;
  str = str.toString().replace(/,/g, '');
  return parseFloat(str);
}

function ConvertToString(str: any) {
  if (str === null || str === undefined) return '';
  return String(str).trim();
}
