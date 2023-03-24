import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener, Injector } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { NumberToStringPipe } from '../../../shared/ConvertMoneyToString/numberToString.pipe';
import { CustomerOrder } from '../../models/customer-order.model';
import { OrderProductDetailProductAttributeValue } from '../../models/order-product-detail-product-attribute-value.model';
import { ContactModel } from '../../../shared/models/contact.model';
import { CustomerService } from '../../../customer/services/customer.service';
import { CustomerOrderService } from '../../services/customer-order.service';
import { BankService } from '../../../shared/services/bank.service';
import { ContactService } from '../../../shared/services/contact.service';
import { QuoteService } from '../../../customer/services/quote.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { EmailConfigService } from '../../../admin/services/email-config.service';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { NewTreeNode, OrderDetailDialogComponent } from '../order-detail-dialog/order-detail-dialog.component';
import { CustomerOrderDetail, CustomerOrderExtension, CustomerOrderDetailExten } from '../../models/customer-order-detail.model';
import * as $ from 'jquery';
import { OrderCostDetail } from '../../models/customer-order-cost-detail.model';
import { TranslateService } from '@ngx-translate/core';

import { ServicePacket } from '../../../../../src/app/product/models/productPacket.model';
import { TrangThaiGeneral } from '../../../../../src/app/product/models/product.model';
import { AbstractBase } from '../../../shared/abstract-base.component';
import { NotificationFireBase } from '../../../shared/models/fire-base.model';
import { DatePipe } from '@angular/common';
import { QuyTrinhService } from '../../../../../src/app/admin/services/quy-trinh.service';
import { PaymentMethodConfigure } from '../../../../../src/app/admin/models/mobile-app-configuraton.models';


interface Order {
  orderId: string,
  orderCode: string,
  createdDate: Date,
  customerName: string,
  supporterName: string,
  orderStatusName: string,

  statusOrder: number,
  orderDate: string,
  customerId: string,
  orderTypeName: string,
  orderType: number,
  backgroundColorForStatus: string,
}


class ResultDialog {
  status: boolean;
  //Lưu lại tất cả các thuộc tính của từng option đã điền
  listAtrrOption: CustomerOrderExtension[];
  //Thuộc tính gói dịch vụ
  listAttrPacket: Array<CustomerOrderExtension>;
  //Infor gói dịch vụ
  packetService: ServicePacket;
  //Các tùy chọn dịch vụ đã chọn ở level cuối cùng
  listOptionSave: any;
}

interface DiscountType {
  name: string;
  code: string;
  value: boolean;
}

class rowDataAddedOption {
  serviceName: string;
  optionName: string;
  optionId: string;
  number: number;
  cost: number;
  vat: number;
  //Lưu lại tất cả các thuộc tính của từng option đã điền
  listAtrrOption: CustomerOrderExtension[];
  //Thuộc tính gói dịch vụ
  listAttrPacket: Array<CustomerOrderExtension>;
  //Infor gói dịch vụ
  packetService: ServicePacket;
  //Các tùy chọn dịch vụ đã chọn ở level cuối cùng
  listOptionSave: any;
}

interface CustomerType {
  typeValue: number;
  typeName: string;
}

export class CustomList {
  value: number;
  name: string;
}

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent extends AbstractBase implements OnInit {
  /*Khai báo biến*/
  auth: any = JSON.parse(localStorage.getItem("auth"));
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  minimumProfit = this.systemParameterList.find(x => x.systemKey == 'MinimumProfitExpect').systemValueString;
  defaultNumberType = this.getDefaultNumberType();
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;


  toDay: Date = new Date();
  empLoginName: string = "Tên người tạo";

  fixed: boolean = false;
  withFiexd: string = "";
  withFiexdCol: string = "";
  withColCN: number = 0;
  withCol: number = 0;
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
      var colT = 0;
      if (this.withColCN != width) {
        colT = this.withColCN - width;
        this.withColCN = width;
        this.withCol = $('#parentTH').width();
      }
      this.withFiexdCol = (this.withCol) + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
      this.withCol = $('#parentTH').width();
      this.withColCN = $('#parent').width();
      this.withFiexdCol = "";
    }
  }

  /* Form */
  colLeft: number = 8;
  isShow: boolean = true;
  createOrderForm: FormGroup;
  customerControl: FormControl;
  customerEmailControl: FormControl;
  customerPhoneControl: FormControl;
  fullAddressControl: FormControl;
  orderTypeControl: FormControl;
  objectControl: FormControl; //Bổ sung từ phiếu nào ( order )
  /* End */

  /* Valid Form */
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  /* End */

  editing: boolean = false;
  isCurrentStep: boolean = true;


  isShowAddOptionIncurred: boolean = false; //show dialog thêm dịch vụ phát sinh

  listProduct: Array<any> = [];
  listCustomer: Array<any> = [];

  listCustomerCode: Array<string> = [];

  listCustomerType: Array<CustomerType> = [
    {
      typeValue: 2,
      typeName: 'Khách hàng cá nhân'
    },
    {
      typeValue: 1,
      typeName: 'Khách hàng doanh nghiệp'
    }
  ];

  orderCode: string = "Hệ thống tự động tạo"
  selectedColumns: any[];
  selectedColumns2: any[];
  selectedColumns3: any[];
  selectedColumns4: any[];


  selectedItem: any;
  //Dịch vụ đã có
  listCustomerOrderDetailModel: Array<rowDataAddedOption> = [];
  //Dịch vụ phát sinh
  listCustomerOrderDetailModel2: Array<CustomerOrderDetailExten> = [];
  //list phiếu hỗ trợ
  listCustomerOrderAction: Array<Order> = [];
  //list yêu cầu bổ sung
  listCustomerOrderExten: Array<Order> = [];


  discountTypeList: Array<CustomList> = [
    { value: 1, name: "Theo %" },
    { value: 2, name: "Theo số tiền" },
  ];


  listTax: Array<CustomList> = [
    { value: -1, name: "Không chịu thuế" },
    { value: 0, name: "0%" },
    { value: 5, name: "5%" },
    { value: 8, name: "8%" },
    { value: 10, name: "10%" },
  ];

  orderId: string = this.emptyGuid;
  cusId: string = this.emptyGuid;
  packId: string = this.emptyGuid;
  orderProcessId: string = this.emptyGuid;

  //disable trường chọn KH và gói dịch vụ 
  disableCusAndPack: boolean = true;

  statusOrder: number = 1; //Mới

  listFormatStatusSupport: Array<any> = []; // Thanh trạng thái

  listPaymentMethod: Array<PaymentMethodConfigure> = [];
  selectedPayMentMethod: any;
  payContent: string = '';

  typeAccount: number = 2;

  contactModel = new ContactModel();

  arrayCustomerOrderDetailModel: Array<CustomerOrderDetail> = [];
  arrayOrderProductDetailProductAttributeValueModel: Array<OrderProductDetailProductAttributeValue> = [];

  listAllCustomerOrder: Array<Order> = [];

  //Tổng tiền trước thuế
  totalCostBeforeTax: number = 0;
  //Thuế GTGT
  vat: CustomList = { value: -1, name: "Không chịu thuế" };
  //Chiết khấu Type
  discountType: CustomList = { value: 1, name: "Theo %" };
  //Số chiết khấu: % or money
  discountNumber: number = 0;
  //Tổng tiền thuế
  totalCostTax: number = 0;
  //Tổng tiền chiết khấu
  totalDiscountCost: number = 0;
  // Tổng tiền phải trả
  totalCostPay: number = 0;
  messageConfirm: string = '';

  //Detail
  empNameCreator: string = "";
  customerOrder = null;

  listOrderType: Array<TrangThaiGeneral> = [];

  orderExtenId: string;
  orderActionId: string = null;
  //Note
  isManagerOfHR: boolean = false;
  isGD: boolean = false;
  isNguoiPhuTrach: boolean = false;
  viewNote: boolean = true;
  viewTimeline: boolean = true;
  pageSize = 20;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  isReportPoint: boolean = false; // là điểm báo cáo ?

  isShowXacNhan: boolean = false;
  isShowTuChoi: boolean = false;
  isShowButtonDelete: boolean = false;

  listStatusOrderExtenDetail = [
    { value: 3, name: "Chờ phê duyệt" },
    { value: 1, name: "Từ chối" },
  ];
  isShowNote = false;
  paymentMethodNote = "";

  constructor(
    injector: Injector,
    private translate: TranslateService,
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private customerService: CustomerService,
    private bankService: BankService,
    private customerOrderService: CustomerOrderService,
    private quoteService: QuoteService,
    private contactService: ContactService,
    public cdRef: ChangeDetectorRef,
    private emailConfigService: EmailConfigService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private renderer: Renderer2,
    public datepipe: DatePipe,
    private quyTrinhService: QuyTrinhService,

  ) {
    super(injector)
    this.renderer.listen('window', 'click', (e: Event) => {
      /**
       * Only run when toggleButton is not clicked
       * If we don't check this, all clicks (even on the toggle button) gets into this
       * section which in the result we might never see the menu open!
       * And the menu itself is checked here, and it's where we check just outside of
       * the menu and button the condition abbove must close the menu
       */
      if (this.toggleButton && this.notifi) {
        if (this.saveAndCreate) {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target) &&
            !this.saveAndCreate.nativeElement.contains(e.target)) {
            this.isOpenNotifiError = false;
          }
        } else {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target)) {
            this.isOpenNotifiError = false;
          }
        }
      }
    });
  }

  async ngOnInit() {
    this.setForm();
    this.setTable();
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;

    let resource = "cusOrder/order/create";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.getMasterData();
    }

  }

  getParam() {
    this.route.params.subscribe(params => {
      this.orderId = params['OrderId'];
      this.orderExtenId = params['OrderExtenId'];
      this.cusId = params['CusId'];
      this.packId = params['PackId'];
      this.orderProcessId = params['OrderProcessId'];

      if (this.cusId == null || this.packId == null) {
        this.disableCusAndPack = false;
      }
    });
  }


  setForm() {
    this.customerControl = new FormControl(null, [Validators.required]);
    this.customerEmailControl = new FormControl(null);
    this.customerPhoneControl = new FormControl(null);
    this.fullAddressControl = new FormControl(null);
    this.orderTypeControl = new FormControl(null, [Validators.required]);
    this.objectControl = new FormControl(null);

    this.createOrderForm = new FormGroup({
      customerControl: this.customerControl,
      customerEmailControl: this.customerEmailControl,
      customerPhoneControl: this.customerPhoneControl,
      fullAddressControl: this.fullAddressControl,
      orderTypeControl: this.orderTypeControl,
      objectControl: this.objectControl,
    });

  }

  setTable() {

    this.selectedColumns = [
      { field: 'Move', header: '#', width: '40px', textAlign: 'center', color: '#f44336' },
      { field: 'serviceName', header: 'Tên gói dịch vụ', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'optionName', header: 'Tên dịch vụ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'number', header: 'Số lượng', width: '80px', textAlign: 'center', color: '#f44336' },
      { field: 'vat', header: 'VAT (%)', width: '80px', textAlign: 'center', color: '#f44336' },
      { field: 'cost', header: 'Giá tiền', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'delete', header: 'Thao tác', width: '170px', textAlign: 'center', color: '#f44336' },
    ];

    this.selectedColumns2 = [
      { field: 'Move', header: '#', width: '40px', textAlign: 'center', color: '#f44336' },
      { field: 'name', header: 'Tên dịch vụ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', width: '120px', textAlign: 'left', color: '#f44336' },
      { field: 'price', header: 'Đơn giá', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'statusName', header: 'Phê duyệt', width: '200px', textAlign: 'left', color: '#f44336' },
      { field: 'action', header: 'Thao tác', width: '170px', textAlign: 'center', color: '#f44336' },
    ];


    this.selectedColumns3 = [
      { field: 'Move', header: '#', width: '40px', textAlign: 'center', color: '#f44336' },
      { field: 'orderCode', header: 'Mã phiếu', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'createdDate', header: 'Ngày yêu cầu', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'customerName', header: 'Tên khách hàng', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'orderStatusName', header: 'Trạng thái', width: '170px', textAlign: 'center', color: '#f44336' },
    ];

    this.selectedColumns4 = [
      { field: 'Move', header: '#', width: '40px', textAlign: 'center', color: '#f44336' },
      { field: 'orderCode', header: 'Mã phiếu', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'createdDate', header: 'Ngày yêu cầu', width: '120px', textAlign: 'left', color: '#f44336' },
      { field: 'customerName', header: 'Tên khách hàng', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'supporterName', header: 'Nhân viên hỗ trợ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'orderStatusName', header: 'Trạng thái', width: '170px', textAlign: 'center', color: '#f44336' },
    ];
  }

  getMasterData() {
    this.getParam();
    this.loading = true;
    this.customerOrderService.getMasterDataOrderCreate(this.cusId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listCustomer = result.listCustomer;
        this.listCustomerCode = result.listCustomerCode;
        this.empLoginName = result.empLoginName;
        this.listOrderType = result.listOrderType;
        this.listPaymentMethod = result.listPaymentMethod;
        this.listAllCustomerOrder = result.listAllCustomerOrder;
        //Nếu đã có thông tin KH và gói => phiếu yêu cầu
        if (this.disableCusAndPack) {
          //Thông tin KH
          let customer = this.listCustomer.find(x => x.customerId == this.cusId);
          this.customerControl.setValue(customer);
          this.changeCustomer(customer);

          //Loại phiếu: yêu cầu (1) hoặc bổ sung (2)
          let orderType = this.listOrderType.find(x => x.value == 1);
          this.orderTypeControl.setValue(orderType);
          this.changeOrderType(orderType)

        }
        //Phiếu bổ sung
        else {
          //Loại phiếu: yêu cầu (1) hoặc bổ sung (2)
          let orderType = this.listOrderType.find(x => x.value == 2);
          this.orderTypeControl.setValue(orderType);
          this.changeOrderType(orderType);

          if (this.orderExtenId) {
            let orderExten = this.listAllCustomerOrder.find(x => x.orderId == this.orderExtenId);
            this.objectControl.setValue(orderExten);
            this.choseRootOrder(orderExten);
          }
        }

        if (this.orderId != null) this.setDefaultValue();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  changeOrderType(data) {
    //Phiếu thường
    if (data.value == 1) {
      this.objectControl.setValidators(null);
      this.objectControl.updateValueAndValidity();
    }
    //Phiếu bổ sung
    else {
      this.objectControl.setValidators([Validators.required]);
      this.objectControl.updateValueAndValidity();
    }
  }

  setDefaultValue() {
    this.customerOrderService.getMasterDataOrderDetail(this.orderId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listAllCustomerOrder = this.listAllCustomerOrder.filter(item => item.orderId != this.orderId);
        let customer = this.listCustomer.find(x => x.customerId == result.customerOrder.customerId);
        this.customerControl.setValue(customer);
        this.changeCustomer(customer);

        this.customerOrder = result.customerOrder;
        this.orderActionId = result.orderActionId;

        this.isCurrentStep = result.isCurrentStep; //Kiểm tra xem có phải xác nhận thanh toán là bước hiện tại trong orderProcess hay k
        this.isShowTuChoi = result.isShowTuChoi; //Hiển thị nút từ chối
        this.isShowXacNhan = result.isShowXacNhan; //Hiển thị nút xác nhận
        this.isShowButtonDelete = result.isShowButtonDelete //Hiển thị nút xóa

        this.packId = result.customerOrder.servicePacketId; //Gói dịch vụ gán với phiếu

        this.payContent = result.customerOrder.paymentContent;
        this.selectedPayMentMethod = this.listPaymentMethod.find(x => x.id == result.customerOrder.paymentMethod);
        if(this.selectedPayMentMethod && this.selectedPayMentMethod.categoryCode == "TM"){
          this.isShowNote = true;
          this.paymentMethodNote = this.customerOrder.paymentMethodNote;
        }

        this.vat = this.listTax.find(x => x.value == result.customerOrder.vat);
        this.discountType = result.customerOrder.discountType == true ? this.discountTypeList.find(x => x.value == 2) : this.discountTypeList.find(x => x.value == 1);
        this.discountNumber = result.customerOrder.discountValue;
        this.empLoginName = result.empNameCreator;
        this.toDay = new Date(result.customerOrder.createdDate);
        this.orderCode = result.customerOrder.orderCode;
        this.statusOrder = result.customerOrder.statusOrder;
        this.orderProcessId = result.customerOrder.orderProcessId;
        let orderType = this.listOrderType.find(x => x.value == result.customerOrder.orderType);
        this.orderTypeControl.setValue(orderType);
        this.changeOrderType(orderType)
        let listDetail: Array<CustomerOrderDetail> = result.listDetail;

        let object = this.listAllCustomerOrder.find(x => x.orderId == result.customerOrder.objectId);
        this.objectControl.setValue(object);
        //set lại số cột theo trạng thái
        this.setTable();
        if (this.statusOrder != 1 && this.statusOrder != 3) this.selectedColumns = this.selectedColumns.filter(x => x.field != 'delete');
        if (this.statusOrder != 1 && this.statusOrder != 2 && this.statusOrder != 10 && this.statusOrder != 11 && this.statusOrder != 12 && this.statusOrder != 3) this.selectedColumns2 = this.selectedColumns2.filter(x => x.field != 'action');

        this.listCustomerOrderDetailModel2 = result.listOptionExten;
        this.listCustomerOrderDetailModel2.forEach(x => {
          x.statusObject = this.listStatusOrderExtenDetail.find(item => item.value == x.status);
        });



        this.listCustomerOrderDetailModel = [];
        listDetail.forEach(item => {
          let newRow = new rowDataAddedOption();
          newRow.serviceName = result.listServicePacket.find(x => x.id == item.servicePacketId).name;
          newRow.number = item.quantity;
          newRow.optionName = item.optionName ?? "Tên dịch vụ";
          newRow.optionId = item.optionId;
          newRow.cost = item.priceInitial;
          newRow.vat = item.vat;
          //Lưu lại tất cả các thuộc tính của từng option đã điền
          newRow.listAtrrOption = result.listAtrrOption.filter(x => x.objectType == "1");
          //Thuộc tính gói dịch vụ
          newRow.listAttrPacket = result.listAtrrPacket.filter(x => x.objectType == "2");
          //Infor gói dịch vụ
          newRow.packetService = result.listServicePacket.find(x => x.id == item.servicePacketId);
          //Các tùy chọn dịch vụ đã chọn ở level cuối cùng
          newRow.listOptionSave = []
          let selectedOption = listDetail.filter(x => x.servicePacketId == item.servicePacketId);
          selectedOption.forEach(selectedOp => {
            let newSelected: NewTreeNode = {
              number: selectedOp.quantity.toString(),
              data: { id: selectedOp.optionId, price: selectedOp.priceInitial, vat: selectedOp.vat },
              listAttr: newRow.listAtrrOption,
              path: "",
              margin: "",
            };
            newRow.listOptionSave.push(newSelected);
          });
          this.listCustomerOrderDetailModel.push(newRow);
        });

        //list phiếu hỗ trợ
        this.listCustomerOrderAction = result.listCustomerOrderAction;
        //list yêu cầu bổ sung
        this.listCustomerOrderExten = result.listAllOrderExten;
        this.handleBackGroundColorForStatus();

        this.getDuLieuQuyTrinh();
        this.calculatorAll();
      } else {
        this.loading = false;
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  handleBackGroundColorForStatus() {
    this.listCustomerOrderAction.forEach(item => {
      switch (item.statusOrder) {
        case 1://bi tra lai
          item.backgroundColorForStatus = "#BB0000";
          break;
        case 2://da dong
          item.backgroundColorForStatus = "#6D98E7";
          break;
        case 3://da giao hang
          item.backgroundColorForStatus = "#66CC00";
          break;
        case 4://da thanh toan
          item.backgroundColorForStatus = "#9C00FF";
          break;
        case 5://dang xu ly
          item.backgroundColorForStatus = "#34c759";
          break;
        case 6://hoan
          item.backgroundColorForStatus = "#666666";
          break;
        default:
          item.backgroundColorForStatus = "#ffcc00";
          break;
      }
    });

    this.listCustomerOrderExten.forEach(item => {
      switch (item.statusOrder) {
        case 1://bi tra lai
          item.backgroundColorForStatus = "#BB0000";
          break;
        case 2://da dong
          item.backgroundColorForStatus = "#6D98E7";
          break;
        case 3://da giao hang
          item.backgroundColorForStatus = "#66CC00";
          break;
        case 4://da thanh toan
          item.backgroundColorForStatus = "#9C00FF";
          break;
        case 5://dang xu ly
          item.backgroundColorForStatus = "#34c759";
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

  goToDetail(orderId: string, isOrderAction: boolean) {
    if (isOrderAction) {
      this.router.navigate(['order/orderAction', { OrderActionId: orderId }]);
    } else {
      this.router.navigate(['order/create', { OrderId: orderId }]);
    }
  }

  changeCustomer(customer: any, clear?: string) {
    //Nếu thay đổi khách hàng trên giao diện thì clear hết sản phẩm dịch vụ
    if (clear) {
      this.listCustomerOrderDetailModel = [];
      this.calculatorAll();
    }


    this.customerControl.setValue("");
    this.customerEmailControl.setValue("");
    this.customerPhoneControl.setValue("");
    this.fullAddressControl.setValue("");

    if (customer) {
      this.customerControl.setValue(customer);
      this.customerEmailControl.setValue(customer.customerEmail);
      this.customerPhoneControl.setValue(customer.customerPhone);
      this.fullAddressControl.setValue("");

      //Lấy địa chỉ của khách hàng
      this.contactService.getAddressByObject(customer.customerId, "CUS").subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.fullAddressControl.setValue(result.address);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  /*Thêm sản phẩm dịch vụ*/
  openServiceOrderDetail(isAdd: boolean): void {
    let ref = this.dialogService.open(OrderDetailDialogComponent, {
      data: {
        isCreate: isAdd,
        statusOrder: this.statusOrder,
        packId: this.packId,
      },
      header: 'Thêm dịch vụ',
      width: '90%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "700px",
        "overflow": "auto"
      }
    });
    ref.onClose.subscribe((result: ResultDialog) => {
      if (result) {
        if (result.status) {
          //Chỉ được thêm 1 gói dịch vụ trong 1 phiếu.
          let listPack = this.listCustomerOrderDetailModel.map(x => x.packetService);
          if (listPack.filter(x => x.id == result.packetService.id).length != listPack.length) {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: "Mỗi phiếu chỉ được thêm 1 gói dịch vụ!" };
            this.showMessage(msg);
            return;
          }
          //Các tùy chọn dịch vụ đã chọn ở level cuối cùng
          let listOptionSave = result.listOptionSave;
          //Thêm vào bảng
          this.listCustomerOrderDetailModel = [];
          listOptionSave.forEach(item => {
            let newRow = new rowDataAddedOption();
            newRow.serviceName = result.packetService.name;
            newRow.optionName = item.path;
            newRow.optionId = item.optionId;
            newRow.number = item.number;
            newRow.cost = item.data.price;
            newRow.vat = item.data.vat;
            //Lưu lại tất cả các thuộc tính của từng option đã điền
            newRow.listAtrrOption = result.listAtrrOption;
            //Thuộc tính gói dịch vụ
            newRow.listAttrPacket = result.listAttrPacket;
            //Infor gói dịch vụ
            newRow.packetService = result.packetService;
            //Các tùy chọn dịch vụ đã chọn ở level cuối cùng
            newRow.listOptionSave = listOptionSave;
            this.listCustomerOrderDetailModel.push(newRow);
          });
          this.calculatorAll();
        }
      }
    });
  }

  /*Sửa một sản phẩm dịch vụ*/
  onRowSelect(dataRow) {
    let ref = this.dialogService.open(OrderDetailDialogComponent, {
      data: {
        isCreate: false,
        dataRow: dataRow,
        statusOrder: this.statusOrder,
        packId: this.packId,
      },
      header: 'Sửa dịch vụ',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });
    ref.onClose.subscribe((result: ResultDialog) => {
      if (result) {
        if (result.status) {
          //Chỉ được thêm 1 gói dịch vụ trong 1 phiếu.
          let listPack = this.listCustomerOrderDetailModel.map(x => x.packetService);
          if (listPack.filter(x => x.id == result.packetService.id).length != listPack.length) {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: "Mỗi phiếu chỉ được thêm 1 gói dịch vụ!" };
            this.showMessage(msg);
            return;
          }
          this.listCustomerOrderDetailModel = [];
          //Các tùy chọn dịch vụ đã chọn ở level cuối cùng
          let listOptionSave = result.listOptionSave;
          //Thêm vào bảng
          listOptionSave.forEach(item => {
            let newRow = new rowDataAddedOption();
            newRow.serviceName = result.packetService.name;
            newRow.optionName = item.path;
            newRow.optionId = item.optionId;
            newRow.number = item.number;
            newRow.cost = item.data.price;
            newRow.vat = item.data.vat;
            //Lưu lại tất cả các thuộc tính của từng option đã điền
            newRow.listAtrrOption = result.listAtrrOption;
            //Thuộc tính gói dịch vụ
            newRow.listAttrPacket = result.listAttrPacket;
            //Infor gói dịch vụ
            newRow.packetService = result.packetService;
            //Các tùy chọn dịch vụ đã chọn ở level cuối cùng
            newRow.listOptionSave = listOptionSave;
            this.listCustomerOrderDetailModel.push(newRow);
          });
          this.calculatorAll();
        }
      }
    });
  }

  /*Xóa một sản phẩm dịch vụ*/
  deleteItem(dataRow, event: Event) {
    //this.translate.get('order.messages_confirm.delete_confirm').subscribe(value => { this.messageConfirm = value; });
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listCustomerOrderDetailModel = this.listCustomerOrderDetailModel.filter(x => x != dataRow);
        this.listCustomerOrderDetailModel.forEach(item => {
          item.listOptionSave = item.listOptionSave.filter(x => x.data.id != dataRow.optionId);
          item.listAtrrOption = item.listAtrrOption.filter(x => x.objectId != dataRow.optionId && x.objectType == "1");
        });
        this.calculatorAll();
      }
    });
  }

  /*Tính lại tất cả các số phần tổng hợp đơn hàng*/
  calculatorAll() {
    //Tổng tiền trước thuế
    this.totalCostBeforeTax = 0;
    //Tổng tiền thuế
    this.totalCostTax = 0;
    //Tổng tiền chiết khấu
    this.totalDiscountCost = 0;
    // Tổng tiền phải trả
    this.totalCostPay = 0;

    this.listCustomerOrderDetailModel.forEach(item => {
      this.totalCostBeforeTax += (ParseStringToFloat(item.number.toString()) * item.cost);
      this.totalCostTax += (ParseStringToFloat(item.number.toString()) * item.cost) / 100 * ((item.vat == -1 || item.vat == 0) ? 0 : item.vat);
    });

    //Nếu có phát sinh
    if (this.listCustomerOrderDetailModel2.length > 0 && this.statusOrder != 1) {
      this.listCustomerOrderDetailModel2.forEach(item => {
        if (item.price && item.status == 2) {//Nếu là phê duyệt và có nhập tiền
          this.totalCostBeforeTax += (ParseStringToFloat(item.quantity.toString()) * ParseStringToFloat(item.price.toString()));
        }
      });
    }

    // //Chiết khấu theo %
    // if (this.discountType.value == 1) {
    //   this.totalDiscountCost = this.totalCostBeforeTax * this.discountNumber / 100;
    // }
    // //Chiết khấu theo số tiền
    // else if (this.discountType.value == 2) {
    //   this.totalDiscountCost = ParseStringToFloat(this.discountNumber.toString());
    // }
    this.totalCostPay = this.totalCostBeforeTax + this.totalCostTax - this.totalDiscountCost;
  }

  createOrder(statusOrder: number) {
    if (!this.createOrderForm.valid) {
      Object.keys(this.createOrderForm.controls).forEach(key => {
        if (this.createOrderForm.controls[key].valid == false) {
          this.createOrderForm.controls[key].markAsTouched();
        }
      });
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Vui lòng nhập đầy đủ thông tin!" };
      this.showMessage(msg);
      return;
    }

    let listOrderExten: Array<CustomerOrderDetailExten> = this.listCustomerOrderDetailModel2;
    if (listOrderExten.find(x => x.edit == true || (x.price == null && this.statusOrder != 1))) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Vui lòng nhập đầy đủ thông tin dịch vụ phát sinh!" };
      this.showMessage(msg);
      return;
    }

    listOrderExten.forEach(item => {
      item.id = '00000000-0000-0000-0000-000000000000';
      item.quantity = ParseStringToFloat(item.quantity.toString());
      if (this.statusOrder == 2) item.price = ParseStringToFloat(item.price.toString());

    });

    //phương thức thanh toán
    if (!this.selectedPayMentMethod) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Vui lòng chọn phương thức thanh toán!" };
      this.showMessage(msg);
      return;
    }

    let vat: CustomList = this.vat;
    let discountType: number = this.discountType.value;
    let discountValue: number = this.discountNumber ? ParseStringToFloat(this.discountNumber.toString()) : 0;
    let cusOrder = new CustomerOrder();
    cusOrder.OrderId = this.orderId;
    cusOrder.CustomerId = this.customerControl.value.customerId;
    cusOrder.StatusOrder = this.statusOrder;
    cusOrder.OrderType = this.orderTypeControl.value.value;
    cusOrder.PaymentMethod = this.selectedPayMentMethod.id;
    cusOrder.PaymentContent = this.selectedPayMentMethod.content;
    cusOrder.PaymentMethodNote = this.paymentMethodNote;

    if (cusOrder.OrderType == 2) cusOrder.ObjectId = this.objectControl.value.orderId;

    //Các gói dịch vụ được thêm 
    let listPackAdded: Array<ServicePacket> = [];
    //Lưu thông tin các tùy chọn được chọn;
    let listCustomerDetail: Array<CustomerOrderDetail> = [];
    //Lưu thuộc tính của gói và tùy chọn dịch vụ
    let listAttrPackAndOption: Array<CustomerOrderExtension> = [];

    this.listCustomerOrderDetailModel.forEach(rowData => {
      if (listPackAdded.indexOf(rowData.packetService) == -1) {
        listPackAdded.push(rowData.packetService);

        //Lưu các tùy chọn được thêm
        rowData.listOptionSave.forEach(item => {
          let newDetail = new CustomerOrderDetail();
          newDetail.servicePacketId = rowData.packetService.id;
          newDetail.optionId = item.data.id;
          newDetail.vat = item.data.vat;
          newDetail.priceInitial = ParseStringToFloat(item.data.price.toString());
          newDetail.quantity = ParseStringToFloat(item.number.toString());
          listCustomerDetail.push(newDetail);
        });

        //Thuộc tính của tùy chọn
        rowData.listAtrrOption.forEach(item => {
          let newAttr = this.genNewAttr(item, 1);
          listAttrPackAndOption.push(newAttr);
        });

        //Thuộc tính của gói
        rowData.listAttrPacket.forEach(item => {
          let newAttr = this.genNewAttr(item, 2);
          listAttrPackAndOption.push(newAttr);
        });

      }
    });

    //Nếu là phiếu yêu cầu thì cần ít nhất 1 gói
    if (listCustomerDetail.length == 0 && this.orderTypeControl.value.value == 1) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Vui lòng chọn ít nhất 1 gói dịch vụ!" };
      this.showMessage(msg);
      return;
    }
    this.loading = true;
    //Lấy địa chỉ của khách hàng
    this.customerOrderService.CreateCustomerOrder(this.orderId, vat, discountType, discountValue,
      cusOrder, listCustomerDetail, listAttrPackAndOption, listOrderExten, this.orderProcessId, this.packId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          let msg = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
          if (this.orderId == null) {
            this.router.navigate(['order/create', { OrderId: result.customerOrderID }]);
          }
          if (statusOrder == null) {
            this.getMasterData();
          } else {
            this.changeStatusCustomerOrder(statusOrder);
          }
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
  }

  changeStatusCustomerOrder(statusOrder) {
    this.customerOrderService.changeStatusCustomerOrder(this.orderId, statusOrder).subscribe(response => {
      let resultUpdate: any = response;
      this.loading = false;
      if (resultUpdate.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: resultUpdate.messageCode };
        this.showMessage(msg);
        if (resultUpdate.listEmpId.length > 0){
          resultUpdate.listEmpId.forEach(e => {
            let notification: NotificationFireBase = {
              content: "Phiếu " + this.customerOrder.orderCode + ": " + resultUpdate.messageCode,
              status: false,
              url: '/order/create;OrderId=' + this.orderId,
              orderId: this.orderId,
              date: this.datepipe.transform(new Date(), 'dd/MM/yyyy HH:mm:ss'),
              employeeId: e
            }
            this.createNotificationFireBase(notification, e);
          });
        } 

        if (statusOrder == 2) {
          this.guiXacNhan();
        }
        this.getMasterData();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: resultUpdate.messageCode };
        this.showMessage(msg);
      }
    });
  }


  genNewAttr(item: CustomerOrderExtension, type) {
    // type: 1 là tùy chọn, 2 là dịch vụ
    let newAttr = new CustomerOrderExtension();
    newAttr.attributeConfigurationId = item.attributeConfigurationId;
    newAttr.objectId = item.objectId;
    newAttr.objectType = type;
    newAttr.value = item.dataType == 3 ? item.value.toString() : item.value.replace(",", ""); // kiểu datatime thì chuyển thành chuỗi string
    newAttr.dataType = item.dataType;
    newAttr.servicePacketMappingOptionsId = item.servicePacketMappingOptionsId;
    return newAttr;
  }


  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  cancel() {
    this.router.navigate(['order/list']);
  }

  ngOnDestroy() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
    }
  }

  showTotalOrder() {
    this.isShow = !this.isShow;
    this.colLeft = this.isShow ? 8 : 12;
    if (this.isShow) {
      window.scrollTo(0, 0)
    }
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  //data: CustomerOrder
  choseRootOrder(data) {
    //Thông tin KH
    let customer = this.listCustomer.find(x => x.customerId == data.customerId);
    this.customerControl.setValue(customer);
    this.changeCustomer(customer);

    //Quy trình
    this.orderProcessId = data.orderProcessId;
  }

  addOrderDetailExten() {
    var newOrderExten = new CustomerOrderDetailExten();
    this.listCustomerOrderDetailModel2.push(newOrderExten);
    this.listCustomerOrderDetailModel2.forEach((item, index) => {
      item.id = index.toString();
    });
  }

  onRowEditInitChild(rowData: CustomerOrderDetailExten) {
    rowData.edit = !rowData.edit;
  }

  async onRowRemoveChild(rowData: CustomerOrderDetailExten) {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn xóa dòng này?`,
      accept: async () => {
        this.listCustomerOrderDetailModel2 = this.listCustomerOrderDetailModel2.filter(e => e != rowData);
      }
    });
  }

  /** Xử lý row con */
  onRowEditSaveChild(rowData: CustomerOrderDetailExten, event) {
    //Xác nhận phát sinh thì cần phải nhập giá
    if (this.statusOrder == 11 || this.statusOrder == 12 || this.statusOrder == 2 || this.statusOrder == 10) {
      if (!rowData.name || rowData.name == '' || !rowData.quantity || !rowData.price) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Hãy nhập đầy đủ thông tin!' };
        this.showMessage(msg);
        return;
      }
    }
    rowData.edit = !rowData.edit;
  }

  onRowEditCancelChild(rowData: any) {
    //Xác nhận phát sinh thì cần phải nhập giá
    if (this.statusOrder == 11 || this.statusOrder == 12 || this.statusOrder == 2 || this.statusOrder == 10) {
      if (!rowData.name || rowData.name == '' || !rowData.quantity || !rowData.price) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Hãy nhập đầy đủ thông tin!' };
        this.showMessage(msg);
        return;
      }
    }
    rowData.edit = !rowData.edit;
  }

  choseStatusForDetailExten(data, rowData) {
    rowData.status = data.value;
    rowData.statusName = data.name;
    if (!rowData.price) rowData.price = 0;
  }

  viewOrderAction() {
    if (this.orderActionId == null) {
      this.router.navigate(['order/orderAction', { OrderId: this.orderId }]);
    } else {
      this.router.navigate(['order/orderAction', { OrderActionId: this.orderActionId }]);
    }
  }

  async getDuLieuQuyTrinh() {
    var doiTuongApDung = 31;
    this.quyTrinhService.getDuLieuQuyTrinh(this.orderId, doiTuongApDung, null, this.packId).subscribe(res => {
      let result: any = res;

      if (result.statusCode == 200) {
        this.listFormatStatusSupport = result.listDuLieuQuyTrinh;
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }


  async guiXacNhan() {
    let listOrderExten: Array<CustomerOrderDetailExten> = this.listCustomerOrderDetailModel2;
    if (listOrderExten.find(x => x.edit == true || (x.price == null && this.statusOrder != 1))) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Vui lòng nhập đầy đủ thông tin dịch vụ phát sinh!" };
      this.showMessage(msg);
      return;
    }
    var doiTuongApDung = 31;
    this.loading = true;
    this.quyTrinhService.guiPheDuyet(this.orderId, doiTuongApDung, null, this.packId).subscribe(res => {
      let result: any = res;
      if (result.statusCode == 200) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);

        if (result.listEmpId.length > 0){
          result.listEmpId.forEach(e => {
            let notification: NotificationFireBase = {
              content: "Phiếu " + this.customerOrder.orderCode + ": " + result.messageCode,
              status: false,
              url: '/order/create;OrderId=' + this.orderId,
              orderId: this.orderId,
              date: this.datepipe.transform(new Date(), 'dd/MM/yyyy HH:mm:ss'),
              employeeId: e
            }
            this.createNotificationFireBase(notification, e);
          });
        } 
        
        this.getMasterData();
      }
      else {
        this.loading = false;
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  async updateCustomerOrderDetailExtend() {
    let listOrderExten: Array<CustomerOrderDetailExten> = this.listCustomerOrderDetailModel2;
    if (listOrderExten.find(x => x.edit == true || (x.price == null && this.statusOrder != 1))) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Vui lòng nhập đầy đủ thông tin dịch vụ phát sinh!" };
      this.showMessage(msg);
      return;
    }

    this.loading = true;
    this.quyTrinhService.updateCustomerOrderDetailExtend(this.orderId, this.listCustomerOrderDetailModel2).subscribe(res => {
      let result: any = res;
      this.loading = false;
      if (result.statusCode == 200) {
        this.xacNhan(31);
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
        return;
      }
    });
  }

  async xacNhan(doiTuongApDung) {
    this.loading = true;
    this.quyTrinhService.pheDuyet(this.orderId, doiTuongApDung, '', null, this.packId).subscribe(res => {
      let result: any = res;
      if (result.statusCode == 200) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);

        let content = "Phiếu " + this.customerOrder.orderCode + ": Phê duyệt " + (this.customerOrder.objectType == 1 ? "đơn giá phát sinh" : "yêu cầu bổ sung") + " thành công!";
        if (result.listEmpId.length > 0){
          result.listEmpId.forEach(e => {
            let notification: NotificationFireBase = {
              content: content,
              status: false,
              url: '/order/create;OrderId=' + this.orderId,
              orderId: this.orderId,
              date: this.datepipe.transform(new Date(), 'dd/MM/yyyy HH:mm:ss'),
              employeeId: e
            }
            this.createNotificationFireBase(notification, e);
          });
        };

        this.getMasterData();
      }
      else {
        this.loading = false;
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }


  async tuChoi(doiTuongApDung) {
    this.loading = true;
    this.quyTrinhService.tuChoi(this.orderId, doiTuongApDung, "Từ chối", null, this.packId).subscribe(res => {
      let result: any = res;

      if (result.statusCode == 200) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
        let content = "Phiếu " + this.customerOrder.orderCode + ": Từ chối " + (this.customerOrder.objectType == 1 ? "đơn giá phát sinh" : "yêu cầu bổ sung") + " thành công!";

        if (result.listEmpId.length > 0){
          result.listEmpId.forEach(e => {
            let notification: NotificationFireBase = {
              content: content,
              status: false,
              url: '/order/create;OrderId=' + this.orderId,
              orderId: this.orderId,
              date: this.datepipe.transform(new Date(), 'dd/MM/yyyy HH:mm:ss'),
              employeeId: e
            }
            this.createNotificationFireBase(notification, e);
          });
        };
        this.getMasterData();
      }
      else {
        this.loading = false;
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  deleteCustomerOrder() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.customerOrderService.deleteCustomerOrder(this.orderId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.router.navigate(['order/list']);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }

  changePaymentMethod(event: PaymentMethodConfigure): void {
    this.isShowNote = event.categoryCode == "TM" ? true : false;
  }

}


function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}


