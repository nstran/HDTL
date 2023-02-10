import { Component, OnInit, ChangeDetectorRef, Renderer2, Injector } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { CustomerOrder, CustomerOrderTaskEntityModel, ReportPointEntityModel } from '../../models/customer-order.model';
import { CustomerService } from '../../../customer/services/customer.service';
import { CustomerOrderService } from '../../services/customer-order.service';
import { BankService } from '../../../shared/services/bank.service';
import { ContactService } from '../../../shared/services/contact.service';
import { QuoteService } from '../../../customer/services/quote.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { EmailConfigService } from '../../../admin/services/email-config.service';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { CustomerOrderExtension } from '../../models/customer-order-detail.model';
import { TranslateService } from '@ngx-translate/core';
import { DatePipe } from '@angular/common';
import { CustomerOrderTaskSettingComponent } from '../customer-order-task-setting/customer-order-task-setting.component';
import { SettingReportPointComponent } from '../setting-report-point/setting-report-point.component';
import { SettingReportPointDetailComponent } from '../setting-report-point-detail/setting-report-point-detail.component';
import { AbstractBase } from '../../../shared/abstract-base.component';
import { NotificationFireBase } from '../../../shared/models/fire-base.model';

class ResultDialog {
  status: boolean;
  rowData: CustomerOrderTaskEntityModel;
  isChangStatus: boolean;
}

@Component({
  selector: 'app-orderAction',
  templateUrl: './orderAction.component.html',
  styleUrls: ['./orderAction.component.css']
})
export class OrderActionComponent extends AbstractBase implements OnInit {
  /*Khai báo biến*/
  auth: any = JSON.parse(localStorage.getItem("auth"));
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  loading: boolean = false;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;

  toDay: Date = new Date();

  /* Masterdata */
  //List phiếu yêu cầu < customerOrder>
  listCustomerOrder: Array<any> = [];
  /* End */

  editing: boolean = false;

  listColumns = [];

  listSettingTaskEmp: Array<CustomerOrderTaskEntityModel> = [];

  //List all thuộc tính của gói
  listAtrrOption: Array<CustomerOrderExtension> = [];

  title: string = "Tạo phiếu hỗ trợ dịch vụ";

  //form
  orderActionForm: FormGroup;
  //Thông tin phiếu
  customerOrderInfor: FormControl;
  orderActionCode: FormControl;
  creatorName: FormControl;
  createdDate: FormControl;
  orderActionStatus: FormControl;
  //Thông tin khách hàng
  cusName: FormControl;
  cusAddress: FormControl;
  cusPhone: FormControl;
  cusNote: FormControl;
  cusOrderDate: FormControl;
  //end form

  statusOrderAction: number = 1;
  orderActionId: string = null;
  orderId: string = null;
  customerOrderModel: CustomerOrder = new CustomerOrder();

  listReportPoint: Array<ReportPointEntityModel> = [];

  heightOfReportPoint: string = "0px";
  //Công thức: 210 * (số bản ghi - 1)

  customerOrderAction = null;
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
  isActionStep: boolean = true;
  isConfirmStep: boolean = true;

  constructor(private translate: TranslateService,
    injector: Injector,
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
  ) { 
    super(injector)
  }

  async ngOnInit() {
    this.setForm();
    this.setTable();
 
    let resource = "cusOrder/order/orderAction";
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
      this.orderActionId = params['OrderActionId'];
      this.orderId = params['OrderId'];
    });

    this.getMasterData();
  }

  setForm() {
    this.customerOrderInfor = new FormControl(null, [Validators.required]);
    this.orderActionCode = new FormControl("Hệ thống tự động tạo");
    this.creatorName = new FormControl(null);
    this.createdDate = new FormControl(new Date());
    this.orderActionStatus = new FormControl("Mới");
    this.cusName = new FormControl(null);
    this.cusAddress = new FormControl(null);
    this.cusPhone = new FormControl(null);
    this.cusNote = new FormControl(null);
    this.cusOrderDate = new FormControl(null);

    this.orderActionForm = new FormGroup({
      customerOrderInfor: this.customerOrderInfor,
      orderActionCode: this.orderActionCode,
      creatorName: this.creatorName,
      createdDate: this.createdDate,
      cusName: this.cusName,
      cusAddress: this.cusAddress,
      cusPhone: this.cusPhone,
      cusNote: this.cusNote,
      cusOrderDate: this.cusOrderDate,
    });
  }

  setTable() {
    this.listColumns = [
      { field: 'path', header: 'Dịch vụ', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'vendorName', header: 'Nhà cung cấp', width: '140px', textAlign: 'left', color: '#f44336' },
      { field: 'listEmpName', header: 'Người phụ trách', width: '200px', textAlign: 'left', color: '#f44336' },
      // { field: 'mission', header: 'Nhiệm vụ', width: '140px', textAlign: 'left', color: '#f44336' },
      { field: 'note', header: 'Ghi chú', width: '200px', textAlign: 'left', color: '#f44336' },
      { field: 'action', header: 'Tác vụ', width: '60px', textAlign: 'center', color: '#f44336' },
    ];
  }

  getMasterData() {
    this.loading = true;
    this.customerOrderService.getMasterDataCreateOrderAction().subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listCustomerOrder = result.listCustomerOrder;
        this.creatorName.setValue(result.creatorName);

        if (this.orderId != null && this.orderActionId == null) {
          let order = this.listCustomerOrder.find(x => x.orderId == this.orderId);
          this.customerOrderInfor.setValue(order);
          this.changeCustomerOrder(order, false);
        }

        if (this.orderActionId != null) this.setDefaultValue();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setDefaultValue() {
    this.title = "Chi tiết phiếu hỗ trợ dịch vụ";
    this.loading = true;
    this.customerOrderService.getMasterDataOrderActionDetail(this.orderActionId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.orderActionCode.setValue(result.customerOrderAction.orderCode);
        this.orderActionStatus.setValue(result.customerOrderAction.statusOrderName);
        this.creatorName.setValue(result.empNameCreator);
        this.createdDate.setValue(result.customerOrderAction.createdDate);
        this.statusOrderAction = result.customerOrderAction.statusOrder;
        this.orderId = result.customerOrderAction.objectId;
        this.customerOrderAction = result.customerOrderAction;
        let newArray = [];
        newArray.push(result.customerOrder);
        this.listCustomerOrder.forEach(item => {
          newArray.push(item);
        });

        this.listCustomerOrder = newArray;
        this.isActionStep = result.isActionStep;
        this.isConfirmStep = result.isConfirmStep;
        this.customerOrderInfor.setValue(result.customerOrder);
        this.changeCustomerOrder(result.customerOrder, true);
        this.listSettingTaskEmp = result.listCustomerOrderTask;

        this.listReportPoint = result.listReportPoint;

        //Set chiều cao của thanh nối các step
        //Công thức: 210 * (số bản ghi - 1)
        this.heightOfReportPoint = ((this.listReportPoint.length - 1) * 210).toString() + "px";
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  cancel() {
    this.router.navigate(['order/orderActionList']);
  }

  changeCustomerOrder(data, isDetailScreen: boolean = false) {
    //Lấy thông tin về các dịch vụ đã được chọn trong phiếu
    this.customerOrderService.changeCustomerOrder(data.orderId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listAtrrOption = result.listAtrrOption;
        this.cusName.setValue(result.cusName);
        this.cusAddress.setValue(result.cusAddress);
        this.cusPhone.setValue(result.cusPhone);
        this.cusNote.setValue(result.cusNote);
        this.cusOrderDate.setValue(result.cusOrderDate);

        //Nếu kp là màn detail thì gán lại
        if (!isDetailScreen) {
          this.listSettingTaskEmp = [];
          result.listOrderDetail.forEach(item => {
            let newTask = new CustomerOrderTaskEntityModel();
            newTask.servicePacketMappingOptionsId = item.optionId;
            newTask.path = item.optionName;
            newTask.optionId = item.optionTempId;
            newTask.servicePacketId = item.servicePacketId;
            newTask.isExtend = false; // có phải tùy chọn phát sinh hay không
            this.listSettingTaskEmp.push(newTask);
          });

          result.listOrderDetailExten.forEach(item => {
            let newTask = new CustomerOrderTaskEntityModel();
            newTask.servicePacketMappingOptionsId = item.id;
            newTask.path = item.name;
            newTask.servicePacketId = item.servicePacketId;
            newTask.isExtend = true; // có phải tùy chọn phát sinh hay không
            this.listSettingTaskEmp.push(newTask);
          });
        }
      } else {
        this.loading = false;
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });

  }

  /*Chỉnh sửa thông tin nhân viên phụ trách dịch vụ*/
  editTask(rowData: CustomerOrderTaskEntityModel): void {
    let listAttr = [];
    if (!rowData.isExtend) listAttr = this.listAtrrOption.filter(x => x.objectId == rowData.optionId && x.servicePacketMappingOptionsId == rowData.servicePacketMappingOptionsId);

    let ref = this.dialogService.open(CustomerOrderTaskSettingComponent, {
      data: {
        rowData: rowData,
        listAttr: listAttr,
        servicePacketId: rowData.servicePacketId,
      },
      header: 'Cài đặt thông tin nhân viên hỗ trợ',
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "550px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: ResultDialog) => {
      if (result.status) {
        rowData.vendorId = result.rowData.vendorId;
        rowData.vendorName = result.rowData.vendorName;
        rowData.empId = result.rowData.empId;
        rowData.empName = result.rowData.empName;
        rowData.mission = result.rowData.mission;
        rowData.note = result.rowData.note;
      }
    });
  }

  // SettingReportPointComponent: cấu hình điểm báo cáo
  // SettingReportPointDetailComponent: cập nhật báo cáo
  openSettingReport(rowData: ReportPointEntityModel): void {
    let ref = this.dialogService.open(SettingReportPointComponent, {
      data: {
        orderActionId: this.orderActionId,
        rowData: rowData,
      },
      header: 'Cài đặt điểm báo cáo',
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "550px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: ResultDialog) => {
      if (result.status) {
        this.setDefaultValue();
      }
    });
  }

  //Xem chi tiết hoặc báo cáo cho 1 điểm báo cáo
  openReport(isDetail, reportPoint: ReportPointEntityModel): void {
    let ref = this.dialogService.open(SettingReportPointDetailComponent, {
      data: {
        isDetail: isDetail,
        reportPoint: reportPoint,
        orderActionId: this.orderActionId,
        orderStatus: this.statusOrderAction,
        orderActionCode: this.orderActionCode.value,
      },
      header: isDetail ? 'Chi tiết điểm báo cáo' : "Báo cáo",
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "700px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: ResultDialog) => {
      if (result.isChangStatus) {
        this.getMasterData();
      }
    });
  }

  //Tạo hoặc cập nhật phiếu hỗ trợ
  createOrderAction(statusOrderAction: number) {
    if (!this.orderActionForm.valid) {
      Object.keys(this.orderActionForm.controls).forEach(key => {
        if (this.orderActionForm.controls[key].valid == false) {
          this.orderActionForm.controls[key].markAsTouched();
        }
      });
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Vui lòng chọn thông tin phiếu yêu cầu' };
      this.showMessage(msg);
      return;
    }

    let listSettingEmpToTask = this.listSettingTaskEmp;
    //Check validate các task cho nhân viên
    let checkEmpToTask = false;
    listSettingEmpToTask.forEach(key => {
      if (key.listEmpId == null || key.listEmpId.length == 0 || (key.isExtend == false && key.vendorId == null)) checkEmpToTask = true;
    });

    if (checkEmpToTask) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Vui lòng chọn đầy đủ thông tin cài đặt nhân viên hỗ trợ!' };
      this.showMessage(msg);
      return;
    }

    //Check validate các điểm báo cáo
    let checkReportPoint = false;
    this.listReportPoint.forEach(item => {
      if (
        (item.empId == null || item.empId == this.emptyGuid || item.empId == "") &&
        (item.content == null || item.content == "")
      ) {
        checkReportPoint = true;
      }
    });

    if (checkReportPoint) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Vui lòng cấu hình đầy đủ thông tin các điểm báo cáo!' };
      this.showMessage(msg);
      return;
    }

    let customrerOderId = this.customerOrderInfor.value.orderId;
    this.loading = true;
    this.customerOrderService.createOrUpdateCustomerOrderAction(this.orderActionId, customrerOderId, listSettingEmpToTask, statusOrderAction).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
        if (result.listEmpId.length > 0){
          result.listEmpId.forEach(e => {
            let notification: NotificationFireBase = {
              content: "Phiếu " + this.customerOrderInfor.value.orderCode + ": " + result.messageCode,
              status: false,
              url: '/order/create;OrderId=' + result.customerOrderActionId,
              orderId: this.orderId,
              date: this.datepipe.transform(new Date(), 'dd/MM/yyyy HH:mm:ss'),
              employeeId: e
            }
            this.createNotificationFireBase(notification, e);
          });
        };

        if (this.orderActionId == null) this.router.navigate(['order/orderAction', { OrderActionId: result.customerOrderActionId }]);
        this.getMasterData();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }


}

