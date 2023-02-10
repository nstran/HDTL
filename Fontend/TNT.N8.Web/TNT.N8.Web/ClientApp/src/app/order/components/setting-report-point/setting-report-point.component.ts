import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService, TreeNode } from 'primeng/api';

import { CustomerOrderService } from '../../services/customer-order.service';

import { ProductCategoryService } from '../../../admin/components/product-category/services/product-category.service';
import { ServicePacket } from '../../../product/models/productPacket.model';
import { AttributeConfigurationEntityModel, CustomerOrderExtension, EmployeeEntityModel, OptionsEntityModel } from '../../../product/models/product.model';
import { VendorListModel, VendorModel } from '../../../product/components/product-options/model/list-vendor';
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
  isCreate: boolean = true; //true: Tạo mới sản phẩm dịch vụ(hoặc chi phí phát sinh), false: Sửa sản phẩm dịch vụ(hoặc chi phí phát sinh)
  selectedOrderDetailType: number = 0;  //0: Sản phẩm dịch vụ, 1: Chi phí phát sinh
  isShowRadioProduct: boolean = true;
  isShowRadioOC: boolean = true;
  /*End*/

  orderActionId: string = "";

  //List các tùy chọn dịch vụ
  listOption: Array<CustomerOrderTaskEntityModel> = [];

  //List nhân viên hỗ trợ
  listSupporter: Array<EmployeeEntityModel> = [];

  //Databinding
  settingFormGroup: FormGroup;
  nameControl: FormControl;
  optionControl: FormControl;
  orderControl: FormControl; // Thứ tự
  reporterControl: FormControl;
  deadlineControl: FormControl;
  isCusViewControl: FormControl;
  contentControl: FormControl;

  rowData: ReportPointEntityModel = new ReportPointEntityModel();

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private customerOrderService: CustomerOrderService,
    public dialogService: DialogService,
    private productCategoryService: ProductCategoryService,
  ) {
    this.orderActionId = this.config.data.orderActionId;
    this.rowData = this.config.data.rowData;
  }

  ngOnInit() {
    this.setForm();
    this.getMasterData();
  }


  setForm() {
    this.nameControl = new FormControl(null, [Validators.required]);
    this.optionControl = new FormControl(null);
    this.orderControl = new FormControl("1", [Validators.required]);
    this.reporterControl = new FormControl(null, [Validators.required]);
    this.deadlineControl = new FormControl(null);
    this.isCusViewControl = new FormControl(null);
    this.contentControl = new FormControl(null, [Validators.required]);

    this.settingFormGroup = new FormGroup({
      nameControl: this.nameControl,
      optionControl: this.optionControl,
      orderControl: this.orderControl,
      reporterControl: this.reporterControl,
      deadlineControl: this.deadlineControl,
      isCusViewControl: this.isCusViewControl,
      contentControl: this.contentControl
    });

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
    this.nameControl.setValue(this.rowData.name);
    let option = this.listOption.find(x => x.servicePacketMappingOptionsId == this.rowData.servicePacketMappingOptionsId);
    if(option != null ){
      this.optionControl.setValue(option);
      this.changeOption(option)
    }
    this.orderControl.setValue(this.rowData.order);
    this.reporterControl.setValue(this.listSupporter.find(x => x.employeeId == this.rowData.empId));
    this.contentControl.setValue(this.rowData.content);
    this.isCusViewControl.setValue(this.rowData.isCusView);
    this.deadlineControl.setValue(this.rowData.deadline != null ?  convertToUTCTime(new Date(this.rowData.deadline)) : null)

  }

  //Đổi listemp theo dịch vụ
  changeOption(option){
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
    if (!this.settingFormGroup.valid) {
      Object.keys(this.settingFormGroup.controls).forEach(key => {
        if (this.settingFormGroup.controls[key].valid == false) {
          this.settingFormGroup.controls[key].markAsTouched();
        }
      });
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Vui lòng nhập đầy đủ thông tin các trường dữ liệu.' };
      this.showMessage(msg);
      return;
    }

    var newReportPoint = new ReportPointEntityModel();
    if(this.rowData){
      newReportPoint.id = this.rowData.id;
      newReportPoint.status = this.rowData.status;
    } 
    newReportPoint.name = this.nameControl.value;
    newReportPoint.optionId = this.optionControl.value ? this.optionControl.value.id : this.emptyGuid;
    newReportPoint.order = this.orderControl.value;
    newReportPoint.empId = this.reporterControl.value.employeeId;
    newReportPoint.content = this.contentControl.value;
    newReportPoint.servicePacketMappingOptionsId = this.optionControl.value != null ? this.optionControl.value.servicePacketMappingOptionsId : null;
    newReportPoint.isCusView = this.isCusViewControl.value;
    newReportPoint.deadline = this.deadlineControl.value != null ? convertToUTCTime(new Date(this.deadlineControl.value)) : null;
    newReportPoint.isExtend = this.optionControl.value ? this.optionControl.value.isExtend : false;

    this.customerOrderService.createOrUpdateCustomerReportPoint(newReportPoint, this.orderActionId).subscribe(response => {
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

