import { Component, ElementRef, OnInit, ViewChild, Injector } from '@angular/core';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';

import { CustomerOrderService } from '../../services/customer-order.service';
import { AbstractBase } from '../../../shared/abstract-base.component';

import { ProductCategoryService } from '../../../admin/components/product-category/services/product-category.service';
import { CustomerOrderTaskEntityModel, ReportPointEntityModel } from '../../models/customer-order.model';
import { NotificationFireBase } from '../../../shared/models/fire-base.model';
import { DatePipe } from '@angular/common';

class ResultDialog {
  status: boolean;
  rowData: CustomerOrderTaskEntityModel;
  isChangStatus: boolean;
}

@Component({
  selector: 'app-setting-report-point-detail',
  templateUrl: './setting-report-point-detail.component.html',
  styleUrls: ['./setting-report-point-detail.component.css'],
  providers: [DialogService]
})


export class SettingReportPointDetailComponent extends AbstractBase implements OnInit {
  @ViewChild('priceInitial') priceInitialElement: ElementRef;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  toDay: Date = new Date();
  currentYear: number = this.toDay.getFullYear();
  cols: any[];

  /*Các biến điều kiện*/
  isDetail: boolean = true; //true: vào màn chi tiết, false: màn báo cáo
  /*End*/

  orderActionId: string = "";
  rowData: ReportPointEntityModel = null;

  isChangStatus: boolean = false;

  //binding data
  name: string = '';
  optionName: string = '';
  order: number = 1;
  empName: string = '';
  deadline: Date = null
  isCusView: boolean = true;
  content: string = '';
  status: number = 1;
  statusName: string = '';

  orderActionCode: any;

  isManagerOfHR: boolean = false;
  isGD: boolean = false;
  isNguoiPhuTrach: boolean = false;
  viewNote: boolean = true;
  viewTimeline: boolean = true;
  pageSize = 20;
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  isReportPoint: boolean = true;

  orderStatus: number = 1;


  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private customerOrderService: CustomerOrderService,
    public dialogService: DialogService,
    public injector: Injector,
    public datepipe: DatePipe,

  ) {
    super(injector)
    this.isDetail = this.config.data.isDetail;
    this.orderActionId = this.config.data.orderActionId;
    this.rowData = this.config.data.reportPoint;
    this.orderStatus = this.config.data.reportPoint.status;
    this.orderActionCode = this.config.data.orderActionCode;

    if (!this.isDetail) {
      //Nếu là sửa
    }
  }

  ngOnInit() {
    this.setDefaultData();
  }



  setDefaultData() {
    this.name = this.rowData.name;
    this.optionName = this.rowData.optionName;
    this.order = this.rowData.order;
    this.empName = this.rowData.empName;
    this.deadline = this.rowData.deadline;
    this.isCusView = this.rowData.isCusView;
    this.content = this.rowData.content;
    this.status = this.rowData.status;
    this.statusName = this.rowData.statusName;
  }

  /*Event Hủy dialog*/
  cancel() {
    let result = new ResultDialog();
    result.status = true;
    result.rowData = null;
    result.isChangStatus = this.isChangStatus;
    this.ref.close(result);
  }
  /*End*/

  /*Event Lưu dialog*/
  save(status: number) {
    debugger
    this.loading = true;
    this.customerOrderService.changeStatusReportPoint(this.rowData.id, status).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.status = status;
        this.rowData.status = status;
        this.orderStatus = status;
        this.isChangStatus = true;
        if (status == 1) {
          this.statusName = 'Mới';
          this.rowData.statusName = 'Mới';
        }
        if (status == 2) {
          this.statusName = 'Đang thực hiện';
          this.rowData.statusName = 'Đang thực hiện';
        }
        if (status == 3) {
          this.statusName = 'Hoàn thành';
          this.rowData.statusName = 'Hoàn thành';
        }
        if (result.listEmpId.length > 0){
          result.listEmpId.forEach(e => {
            let notification: NotificationFireBase = {
              content: "Phiếu " + this.orderActionCode + ": " + result.messageCode + " - " + this.name,
              status: false,
              url: '/order/orderAction;OrderActionId=' + this.orderActionId,
              orderId: this.orderActionId,
              date: this.datepipe.transform(new Date(), 'dd/MM/yyyy HH:mm:ss'),
              employeeId: e
            }
            this.createNotificationFireBase(notification, e);
          });
        };

        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }



}
