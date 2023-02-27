import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { CustomerOrderService } from '../../services/customer-order.service';
import { EmployeeEntityModel } from '../../../product/models/product.model';
import { CustomerOrderTaskEntityModel, ReportPointEntityModel } from '../../models/customer-order.model';

class ResultDialog {
  status: boolean;
  rowData: ReportPointEntityModel;
}

@Component({
  selector: 'app-setting-report-point',
  templateUrl: './setting-report-point.component.html',
  styleUrls: ['./setting-report-point.component.css'],
  providers: [DialogService]
})


export class SettingReportPointComponent implements OnInit {
  @ViewChild('priceInitial') priceInitialElement: ElementRef;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  toDay: Date = new Date();
  currentYear: number = this.toDay.getFullYear();
  cols: any[];

  /*Các biến điều kiện*/
  isShowRadioOC: boolean = true;
  /*End*/

  orderActionId: string = "";

  //List các tùy chọn dịch vụ
  listOption: Array<CustomerOrderTaskEntityModel> = [];

  //List nhân viên hỗ trợ
  listSupporter: Array<EmployeeEntityModel> = [];
  rowData: ReportPointEntityModel = new ReportPointEntityModel();
  employee = new EmployeeEntityModel();
  newReportPoint: ReportPointEntityModel = new ReportPointEntityModel();
  option : CustomerOrderTaskEntityModel;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private customerOrderService: CustomerOrderService,
    public dialogService: DialogService
  ) {
    this.orderActionId = this.config.data.orderActionId;
    this.rowData = this.config.data.rowData;
  }

  ngOnInit() {
    this.getMasterData();
  }

  /*Event set giá trị mặc định cho các control*/
  getMasterData() {
    this.loading = true;
    this.customerOrderService.getOptionAndEmpOfOrderAction(this.orderActionId, null).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listSupporter = result.listSupporter;
        this.listOption = result.listOption;
        if(this.rowData) this.setDefaultData();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }
  /*End*/

  setDefaultData() {
    this.newReportPoint = this.rowData;
    this.newReportPoint.deadline = this.newReportPoint.deadline ? new Date(this.newReportPoint.deadline) : null;
    this.employee = this.listSupporter.find(x => x.employeeId == this.rowData.empId);
    this.option = this.listOption.find(x => x.servicePacketMappingOptionsId == this.rowData.servicePacketMappingOptionsId);
    if(this.option != null ){
      // this.optionControl.setValue(this.option);
      // this.changeOption(this.option);
    }
  }

  //Đổi listemp theo dịch vụ
  changeOption(option: CustomerOrderTaskEntityModel): void {
    let servicePacketMappingOptionsId = option != null ?  option.servicePacketMappingOptionsId : null;
    this.customerOrderService.getOptionAndEmpOfOrderAction(this.orderActionId, servicePacketMappingOptionsId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listSupporter = result.listSupporter;
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Event Hủy dialog*/
  cancel() {
    let result = new ResultDialog();
    result.status = false;
    //Lưu lại tất cả các thuộc tính của từng option đã điền
    result.rowData = null;
    this.ref.close(result);
  }
  /*End*/

  /*Event Lưu dialog*/
  save() {
    if(this.rowData){
      this.newReportPoint.id = this.rowData.id;
      this.newReportPoint.status = this.rowData.status;
    } 
    this.newReportPoint.optionId = this.option ? this.option.id : this.emptyGuid;
    this.newReportPoint.servicePacketMappingOptionsId = this.option != null ? this.option.servicePacketMappingOptionsId : null;
    this.newReportPoint.deadline = this.newReportPoint.deadline != null ? convertToUTCTime(new Date(this.newReportPoint.deadline)) : null;
    this.newReportPoint.isExtend = this.option ? this.option.isExtend : false;

    this.customerOrderService.createOrUpdateCustomerReportPoint(this.newReportPoint, this.orderActionId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      } else {
        this.loading = false;
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
        return;
      }

      let resultDialog = new ResultDialog();
      resultDialog.status = true;
      this.ref.close(resultDialog);
    });
  }


  showMessage(msg: any) {
    this.messageService.add(msg);
  }

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

