import { Component, OnInit, ChangeDetectorRef, Renderer2 ,Injector} from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { CustomerOrderTaskEntityModel, } from '../../models/customer-order.model';
import { CustomerService } from '../../../customer/services/customer.service';
import { CustomerOrderService } from '../../services/customer-order.service';
import { BankService } from '../../../shared/services/bank.service';
import { ContactService } from '../../../shared/services/contact.service';
import { QuoteService } from '../../../customer/services/quote.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { EmailConfigService } from '../../../admin/services/email-config.service';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';
import { OrderProcessDetailEntityModel, OrderProcessEntityModel, PermissionConfigurationEntityModel, ProductCategoryEntityModel, ServicePacketEntityModel } from '../../../../../src/app/product/models/product.model';
import { NotificationFireBase } from '../../../shared/models/fire-base.model';
import { DatePipe } from '@angular/common';
import { AbstractBase } from '../../../shared/abstract-base.component';

class ResultDialog {
  status: boolean;
  rowData: CustomerOrderTaskEntityModel;
}

@Component({
  selector: 'app-orderProcess',
  templateUrl: './orderProcess.component.html',
  styleUrls: ['./orderProcess.component.css']
})
export class OrderProcessComponent extends AbstractBase implements OnInit {
  /*Khai báo biến*/
  auth: any = JSON.parse(localStorage.getItem("auth"));
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  loading: boolean = false;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;

  toDay: Date = new Date();

  editing: boolean = false;


  title: string = "Tạo đơn đặt dịch vụ";

  orderProcessId: string = null;

  //form
  processForm: FormGroup;
  processCodeForm: FormControl;
  serviceTypeControl: FormControl;
  servicePacketControl: FormControl;
  customerControl: FormControl;
  customerPhoneControl: FormControl;
  customerEmailControl: FormControl;
  customerAddressControl: FormControl;
  //end form

  heightOfReportPoint: string = "0px";
  //Công thức: 150 * (số bản ghi - 1)

  //note
  isManagerOfHR: boolean = false;
  isGD: boolean = false;
  isNguoiPhuTrach: boolean = false;
  viewNote: boolean = true;
  viewTimeline: boolean = true;
  pageSize = 20;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  isReportPoint: boolean = false; // là điểm báo cáo ?

  rateStar:number = 5;
  rateContent:string = "";
  
  listCus: Array<any> = [];
  listServiceType: Array<ProductCategoryEntityModel> = [];
  listServicePacket: Array<ServicePacketEntityModel> = []; //list root

  listServicePacketChose: Array<ServicePacketEntityModel> = [];//list display

  //Cấu hình quy trình của gói
  listProcessOfPack: Array<PermissionConfigurationEntityModel> = [];

  //Quy trình của gói gắn với phiếu kèm theo status
  listProcessOfOder: Array<OrderProcessDetailEntityModel> = [];


  orderProces: OrderProcessEntityModel;

  processStatus: number = 1;//Mới

  listOrderExten: any = [];
  showOrderExten: boolean = false;
  colOrderExten: any;
  constructor(
    private translate: TranslateService,
    private injector: Injector,
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
    private renderer: Renderer2,
    public datepipe: DatePipe,
  ) { 
    super(injector)
  }

  async ngOnInit() {
    this.setForm();
    this.setCol();
   
    let resource = "cusOrder/order/orderProcess";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
    }

    this.route.params.subscribe(params => {
      this.orderProcessId = params['OrderProcessId']; 
    });

    this.getMasterData();
  }


  setCol() {
    this.colOrderExten = [
      { field: 'stt', header: '#', width: '50px', textAlign: 'center' },
      { field: 'orderType', header: 'Loại phiếu', width: '170px', textAlign: 'left' },
      { field: 'orderCode', header: 'Phiếu yêu cầu dịch vụ', width: '170px', textAlign: 'left' },
      { field: 'orderActionCode', header: 'Phiếu hỗ trợ dịch vụ', width: '170px', textAlign: 'left' },
    ];
  }

  setForm() {
    this.serviceTypeControl = new FormControl(null, [Validators.required]);
    this.processCodeForm = new FormControl("Hệ thống tự động tạo");
    this.servicePacketControl = new FormControl(null);
    this.customerControl = new FormControl(null, [Validators.required]);
    this.customerPhoneControl = new FormControl(null);
    this.customerEmailControl = new FormControl(null);
    this.customerAddressControl = new FormControl(null);


    this.processForm = new FormGroup({
      serviceTypeControl: this.serviceTypeControl,
      servicePacketControl: this.servicePacketControl,
      processCodeForm: this.processCodeForm,
      customerControl: this.customerControl,
      customerPhoneControl: this.customerPhoneControl,
      customerEmailControl: this.customerEmailControl,
      customerAddressControl: this.customerAddressControl
    });
  }



  getMasterData() {
    this.loading = true;
    this.customerOrderService.getMasterDataCreateOrderProcess().subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listCus = result.listCustomer;
        this.listServiceType = result.listServiceType;
        this.listServicePacket = result.listServicePacket;
        this.listProcessOfPack = result.listProcessOfPack;
        if (this.orderProcessId != null) this.setDefaultValue();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setDefaultValue() {
    this.customerOrderService.getDetailOrderProcess(this.orderProcessId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.title = "Chi tiết đơn đặt dịch vụ";
        this.orderProces = result.orderProcess;

        this.processStatus = result.orderProcess.status;

        this.processCodeForm.setValue(this.orderProces.orderProcessCode);

        let serviceType = this.listServiceType.find(x => x.productCategoryId == this.orderProces.productCategoryId);
        this.serviceTypeControl.setValue(serviceType);
        this.changeSeviceType(serviceType);

        let packet = this.listServicePacket.find(x => x.id == this.orderProces.servicePacketId);
        this.servicePacketControl.setValue(packet);

        let cus = this.listCus.find(x => x.customerId == this.orderProces.customerId);
        this.customerControl.setValue(cus);
        this.changeCustomer(cus);

        this.listProcessOfOder = result.listOrderProcessDetail;

        this.heightOfReportPoint = (150 * (this.listProcessOfOder.length - 1)).toString() + "px";

        this.rateStar =  result.orderProcess.rateStar;
        this.rateContent = result.orderProcess.rateContent;

      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });

  }

  cancel() {
    this.router.navigate(['order/orderProcessList']);
  }

  changeCustomer(data) {
    this.customerAddressControl.setValue(data.fullAddress);
    this.customerPhoneControl.setValue(data.customerPhone);
    this.customerEmailControl.setValue(data.customerEmail);
  }

  changeSeviceType(data: ProductCategoryEntityModel) {
    this.listServicePacketChose = this.listServicePacket.filter(x => x.productCategoryId == data.productCategoryId);
  }

  changePack(data: ServicePacketEntityModel) {
    let listStep = this.listProcessOfPack.filter(x => x.servicePacketId == data.id);
    this.listProcessOfOder = [];
    listStep.forEach(item => {
      let newObj = new OrderProcessDetailEntityModel();
      newObj.stepId = item.stepId;
      newObj.roleId = item.roleId;
      newObj.servicePackageId = item.servicePacketId;
      newObj.employeeId = item.employeeId;
      newObj.categoryId = item.categoryId;
      newObj.categoryName = item.categoryName;
      newObj.categoryCode = item.categoryCode;
      newObj.isEdit = item.isEdit;
      newObj.objectRole = item.objectRole;
      newObj.status = 1; // mới
      newObj.statusName = "Mới";
      this.listProcessOfOder.push(newObj);
    });

    this.heightOfReportPoint = (150 * (this.listProcessOfOder.length - 1)).toString() + "px"
    //Công thức: 150 * (số bản ghi - 1)
  }

  //Tạo hoặc cập nhật phiếu hỗ trợ
  createOrderAction(statusOrderAction: number) {
    if (!this.processForm.valid) {
      Object.keys(this.processForm.controls).forEach(key => {
        if (this.processForm.controls[key].valid == false) {
          this.processForm.controls[key].markAsTouched();
        }
      });
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Vui lòng chọn thông tin phiếu yêu cầu' };
      this.showMessage(msg);
      return;
    }

    var orderProcess = new OrderProcessEntityModel();
    orderProcess.id = this.orderProcessId;
    orderProcess.servicePacketId = this.servicePacketControl.value.id;
    orderProcess.customerId = this.customerControl.value.customerId;
    orderProcess.productCategoryId = this.serviceTypeControl.value.productCategoryId;

    this.loading = true;
    this.customerOrderService.createOrUpdateCustomerOrderProcess(orderProcess).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
        if (this.orderProcessId == null) this.router.navigate(['order/orderProcess', { OrderProcessId: result.id }]);
        this.getMasterData();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  viewOrder(orderId: string) {
    if (orderId == null) {
      const url = this.router.serializeUrl(
        this.router.createUrlTree(['order/create', { CusId: this.customerControl.value.customerId, PackId: this.servicePacketControl.value.id, OrderProcessId: this.orderProcessId }])
      );
      window.open(url, '_blank');
    } else {
      const url = this.router.serializeUrl(
        this.router.createUrlTree(['order/create', { OrderId: orderId }])
      );
      window.open(url, '_blank');
    }

  

  }

  viewOrderAction(orderId: string, orderActionId: string) {
    if (orderActionId == null) {
      const url = this.router.serializeUrl(
        this.router.createUrlTree(['order/orderAction', { OrderId: orderId }])
      );
      window.open(url, '_blank');

    } else {
      const url = this.router.serializeUrl(
        this.router.createUrlTree(['order/orderAction', { OrderActionId: orderActionId }])
      );
      window.open(url, '_blank');
    }
  }

  updateOrderProcess(orderProcessDetailId: string) {
    this.loading = true;
    this.customerOrderService.updateOrderProcess(orderProcessDetailId, this.orderProcessId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);

        if (result.listEmpId.length > 0){
          result.listEmpId.forEach(e => {
            let notification: NotificationFireBase = {
              content: "Phiếu " + this.processCodeForm.value + ": " + result.messageCode,
              status: false,
              url: '/order/orderProcess;OrderProcessId=' + this.orderProcessId,
              orderId: this.orderProcessId,
              date: this.datepipe.transform(new Date(), 'dd/MM/yyyy HH:mm:ss'),
              employeeId: e
            }
            this.createNotificationFireBase(notification, e);
          });
        };

        this.getMasterData();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  createOrderExten(orderId) {
    const url = this.router.serializeUrl(
      this.router.createUrlTree(['order/create', { OrderExtenId: orderId, PackId: this.servicePacketControl.value.id }])
    );
    window.open(url, '_blank');
  }

  viewOrderExten(orderId: string) {
    const url = this.router.serializeUrl(
      this.router.createUrlTree(['order/create', { OrderId: orderId }])
    );
    window.open(url, '_blank');
  }


  viewListOrderExten(attr) {
    this.listOrderExten = attr.listExtenOrder;
    this.showOrderExten = true;
  }

}

