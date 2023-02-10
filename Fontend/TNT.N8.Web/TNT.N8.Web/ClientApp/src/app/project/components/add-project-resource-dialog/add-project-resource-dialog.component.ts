import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { ProjectService } from '../../services/project.service';
import { ProjectResourceModel } from '../../models/ProjectResource.model';
/* Primeng API */
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
/* end PrimeNg API */
import { EmployeeService } from '../../../employee/services/employee.service';
import { EmailConfigService } from '../../../../app/admin/services/email-config.service';
import { ProjectTaskModel } from '../../models/ProjectTask.model';



interface DialogResult {
  status: boolean;
  projectResourceId: string;
}

@Component({
  selector: 'app-add-project-resource-dialog',
  templateUrl: './add-project-resource-dialog.component.html',
  styleUrls: ['./add-project-resource-dialog.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class AddProjectResourceDialogComponent implements OnInit {
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  projectId: string = null;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  listDistributionRate: Array<any> = [];
  listResourceType: Array<any> = [];
  listResourceSource: Array<any> = [];
  listEmployee: Array<any> = [];
  listVendorGroup: Array<any> = [];
  listVendor: Array<any> = [];
  listResourceRole: Array<any> = [];
  projectResource: ProjectResourceModel = new ProjectResourceModel();
  isInternal: boolean = true;
  isExitsVendorAccount: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  createProjectResourceInternalForm: FormGroup;
  createProjectResourceExternalForm: FormGroup;
  isCreateVendor: FormControl;
  projectResourceId: string;
  distributionRateControl: FormControl;
  distributionRateInternalControl: FormControl;
  resourceNameControl: FormControl;
  resourceTypeControl: FormControl;
  vendorGroupControl: FormControl;
  vendorControl: FormControl;
  projectStartTimeControl: FormControl;
  projectStartTimeInternalControl: FormControl;
  projectEndTimeControl: FormControl;
  isWeekendControlNoiBo: FormControl;
  isWeekendControlNgoai: FormControl;
  chiPhiTheoGioControl: FormControl;
  projectEndTimeInternalControl: FormControl;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  roleControl: FormControl;
  minDate: Date = null;
  maxDate: Date = null;

  constructor(private messageService: MessageService,
    public ref: DynamicDialogRef,
    private employeeService: EmployeeService,
    private emailConfigService: EmailConfigService,
    private confirmationService: ConfirmationService,
    private projectService: ProjectService,
    public config: DynamicDialogConfig) {
    this.projectId = this.config.data.projectId;
    this.projectResourceId = this.config.data.projectResourceId;
    this.isInternal = this.config.data.isInternal;
    this.minDate = new Date(this.config.data.projectStartDate);
    this.maxDate = new Date(this.config.data.projectEndDate);
  }

  async ngOnInit() {

    this.loading = true;
    this.initForm();
    this.isExitsVendorAccount = false;
    for (var i = 0; i <= 100; i += 5) {
      this.listDistributionRate.push({
        value: i, name: i.toString() + '%'
      })
    }
    await this.getMasterData();
    // set default status
    if (!this.isInternal) {
      let stt = this.listDistributionRate.find(x => x.value == 100);
      this.distributionRateControl.setValue(stt);
      this.isCreateVendor.setValue(true);
    }
    this.loading = false;
  }

  initForm() {
    // Nội bộ
    this.resourceNameControl = new FormControl(null, [Validators.required]);
    this.resourceTypeControl = new FormControl(null, [Validators.required]);
    this.roleControl = new FormControl(null);
    this.projectStartTimeInternalControl = new FormControl(null, [Validators.required]);
    this.projectEndTimeInternalControl = new FormControl(null, [Validators.required]);
    this.distributionRateInternalControl = new FormControl(null, [Validators.required]);
    this.isWeekendControlNoiBo = new FormControl(false);

    // thuê ngoài
    this.vendorGroupControl = new FormControl(null, [Validators.required]);
    this.vendorControl = new FormControl(null, [Validators.required]);
    this.isCreateVendor = new FormControl(false);
    this.isWeekendControlNgoai = new FormControl(false);
    this.projectStartTimeControl = new FormControl(null, [Validators.required]);
    this.projectEndTimeControl = new FormControl(null, [Validators.required]);
    this.distributionRateControl = new FormControl(null, [Validators.required]);
    this.chiPhiTheoGioControl = new FormControl(null);

    this.createProjectResourceInternalForm = new FormGroup({
      roleControl: this.roleControl,
      projectStartTimeInternalControl: this.projectStartTimeInternalControl,
      projectEndTimeInternalControl: this.projectEndTimeInternalControl,
      resourceTypeControl: this.resourceTypeControl,
      distributionRateInternalControl: this.distributionRateInternalControl,
      resourceNameControl: this.resourceNameControl,
      isWeekendControlNoiBo: this.isWeekendControlNoiBo
    });

    this.createProjectResourceExternalForm = new FormGroup({
      isCreateVendor: this.isCreateVendor,
      distributionRateControl: this.distributionRateControl,
      vendorControl: this.vendorControl,
      vendorGroupControl: this.vendorGroupControl,
      projectStartTimeControl: this.projectStartTimeControl,
      projectEndTimeControl: this.projectEndTimeControl,
      isWeekendControlNgoai: this.isWeekendControlNgoai,
      chiPhiTheoGioControl: this.chiPhiTheoGioControl
    });
    this.createProjectResourceExternalForm.get('distributionRateControl').disable();
    this.createProjectResourceInternalForm.get('distributionRateInternalControl').enable();
  }

  async getMasterData() {
    await this.projectService.getStatusResourceProject(this.projectId, this.projectResourceId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listResourceType = result.listResourceType;
        this.listResourceSource = result.listResourceSource || [];
        this.listResourceRole = result.listResourceRole;
        this.listVendorGroup = result.listVendorGroup;
        this.listEmployee = result.listEmployee;
        this.listVendor = result.listVendor;
        this.projectResource = result.projectResource;

        // Check xem nhà thầu đã có account hay chưa
        if (!this.isInternal) {
          if (this.projectResource.objectId != null) {
            if (this.listVendor.filter(x => x.vendorId.toLocaleLowerCase() == this.projectResource.objectId.toLocaleLowerCase()).length !== 0 && this.listVendor.filter(x => x.vendorId.toLocaleLowerCase() == this.projectResource.objectId.toLocaleLowerCase())[0].exitsAccount == true) {
              this.isExitsVendorAccount = true;
            }
          }
        }

        // Mode chinh sua nguon luc
        if (this.projectResourceId != null) {
          this.mappingDataToForm(this.projectResource);
        }
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  mappingDataToForm(projectResource: ProjectResourceModel) {
    // Nguon luc thue ngoai
    if (!this.isInternal) {
      // Nhóm nhà thầu
      let groupVendor = this.listVendorGroup.find(c => c.categoryId == projectResource.resourceType);
      this.vendorGroupControl.setValue(groupVendor);

      // Nhà thầu
      let vendor = this.listVendor.find(c => c.vendorId == projectResource.objectId);
      this.vendorControl.setValue(vendor);

      // Thời gian
      projectResource.startTime ? this.projectStartTimeControl.setValue(new Date(projectResource.startTime)) : null;

      projectResource.endTime ? this.projectEndTimeControl.setValue(new Date(projectResource.endTime)) : null;

      let percent = this.listDistributionRate.find(c => c.value == projectResource.allowcation);
      this.distributionRateControl.setValue(percent);

      this.isCreateVendor.setValue(projectResource.isCreateVendor);
      this.isWeekendControlNgoai.setValue(projectResource.includeWeekend);
      this.chiPhiTheoGioControl.setValue(projectResource.chiPhiTheoGio.toString());
    }
    // Nguon luc noi bo
    else {
      // Loại nguồn lực
      let resourceType = this.listResourceType.find(c => c.categoryId == projectResource.resourceType);
      this.resourceTypeControl.setValue(resourceType);

      // Tên nguồn lực
      let resourceName = this.listEmployee.find(c => c.employeeId == projectResource.objectId);
      this.resourceNameControl.setValue(resourceName);

      // Vai trò
      let role = this.listResourceRole.find(c => c.categoryId == projectResource.employeeRole);
      this.roleControl.setValue(role);

      // Thời gian
      projectResource.startTime ? this.projectStartTimeInternalControl.setValue(new Date(projectResource.startTime)) : null;
      projectResource.endTime ? this.projectEndTimeInternalControl.setValue(new Date(projectResource.endTime)) : null;

      let percent = this.listDistributionRate.find(c => c.value == projectResource.allowcation);
      this.distributionRateInternalControl.setValue(percent);
      this.isWeekendControlNoiBo.setValue(projectResource.includeWeekend);
    }
  }


  cancel() {
    this.ref.close();
  }

  async save() {
    this.loading = true;
    var isValid = true;
    var isDirty = false;
    if (this.isInternal == true) {
      // Check có sự thay đổi trên dialog
      if (this.createProjectResourceInternalForm.dirty) {
        isDirty = true;
      }
      // Check validation formcontrol
      if (!this.createProjectResourceInternalForm.valid) {
        Object.keys(this.createProjectResourceInternalForm.controls).forEach(key => {
          if (!this.createProjectResourceInternalForm.controls[key].valid) {
            this.createProjectResourceInternalForm.controls[key].markAsTouched();
            isValid = false;
          }
        });
      }
    }
    else {
      // Check có sự thay đổi trên dialog
      if (this.createProjectResourceExternalForm.dirty) {
        isDirty = true;
      }
      // Check validation formcontrol
      if (!this.createProjectResourceExternalForm.valid) {
        Object.keys(this.createProjectResourceExternalForm.controls).forEach(key => {
          if (!this.createProjectResourceExternalForm.controls[key].valid) {
            this.createProjectResourceExternalForm.controls[key].markAsTouched();
            isValid = false;
          }
        });
      }
    }
    if (isValid) {
      let resourceModel: ProjectResourceModel = this.mapFormToResourceModel();
      if (this.projectResourceId != null) {
        resourceModel.projectResourceId = this.projectResourceId;
      }
      this.projectService.checkAllowcateProjectResource(
        resourceModel.objectId, resourceModel.startTime, resourceModel.endTime, resourceModel.allowcation, resourceModel.projectResourceId).subscribe(response => {
          let result = <any>response;
          // Nếu overload thì cảnh báo
          if (result.totalAllowcation > 100) {
            resourceModel.isOverload = true;
            this.loading = false;
            this.confirmationService.confirm({
              header: "Xác nhận",
              message: 'Nguồn lực hiện tại đã vượt quá % phân bổ. Bạn có muốn thêm nguồn lực không?',
              accept: () => {
                if (isDirty)
                  this.createOrUpdateProjectResource(resourceModel, this.auth.userId);
                else {
                  let result: DialogResult = {
                    status: true,
                    projectResourceId: ''
                  }
                  this.ref.close(result);
                }
              }, reject: () => {
                this.confirmationService.close();
              }
            });
          }
          else {
            if (isDirty) {
              resourceModel.isOverload = false;
              this.createOrUpdateProjectResource(resourceModel, this.auth.userId);
            }
            else {
              let result: DialogResult = {
                status: true,
                projectResourceId: ''
              }
              this.ref.close(result);
            }
          }
        }, error => { this.loading = false; });
    }
    else
      this.loading = false;
  }

  createOrUpdateProjectResource(resourceModel: ProjectResourceModel, userId: string) {
    this.loading = true;
    this.projectService.createOrUpdateProjectResource(resourceModel, this.auth.UserId).subscribe(response => {
      let resultCreate = <any>response;
      if (resultCreate.statusCode === 202 || resultCreate.statusCode === 200) {
        // Check tạo tạo khoài nhà thầu không - Nếu có tài tk rồi thì thôi
        if (resourceModel.isCreateVendor) {
          // Gửi email thông báo tạo tài khoản
          var contractor = this.createProjectResourceExternalForm.get('vendorControl').value;
          contractor.Email
          let sendMailModel: any = resultCreate.sendEmailEntityModel;
          if (sendMailModel != null) {
            this.emailConfigService.sendEmail(7, sendMailModel).subscribe(reponse => {
            });
          }
          this.employeeService.sendEmail(contractor.Email, contractor.FirstName + " " + contractor.LastName, contractor.Email).subscribe(response => { }, error => { });
        }
        this.loading = false;
        this.clearToast();
        let result: DialogResult = {
          status: true,
          projectResourceId: resultCreate.projectResourceId
        }
        this.ref.close(result);
        this.showToast('success', 'Thông báo', 'Cập nhật nguồn lực thành công');
      }
      else {
        this.loading = false;
        this.clearToast();
        this.showToast('error', 'Thông báo', 'Cập nhật nguồn lực không thành công');
      }
    }, error => { this.loading = false; });
  }

  mapFormToResourceModel(): ProjectResourceModel {
    try {
      let projectResourceModel: ProjectResourceModel = new ProjectResourceModel();
      projectResourceModel.projectId = this.projectId;

      // Nguồn lực nội bộ
      if (this.isInternal == true) {
        projectResourceModel.resourceRole = this.listResourceSource.filter(x => x.categoryCode == "NB")[0].categoryId
        // EmployeeId or ResourceId lục tương ứng nhóm nhà thầu
        let resourceName = this.createProjectResourceInternalForm.get('resourceNameControl').value
        projectResourceModel.objectId = resourceName !== null ? resourceName.employeeId : this.emptyGuid;
        // Loại nguồn lực
        let resourceType = this.createProjectResourceInternalForm.get('resourceTypeControl').value;
        projectResourceModel.resourceType = resourceType !== null ? resourceType.categoryId : this.emptyGuid;
        // Vai trò
        let roleResource = this.createProjectResourceInternalForm.get('roleControl').value
        projectResourceModel.employeeRole = roleResource !== null && roleResource !== undefined ? roleResource.categoryId : this.emptyGuid;

        // Thời gian bắt đầu
        if (this.createProjectResourceInternalForm.get('projectStartTimeInternalControl').value != null)
          projectResourceModel.startTime = convertToUTCTime(this.createProjectResourceInternalForm.get('projectStartTimeInternalControl').value);
        // Thời gian kết thúc
        if (this.createProjectResourceInternalForm.get('projectEndTimeInternalControl').value != null)
          projectResourceModel.endTime = convertToUTCTime(this.createProjectResourceInternalForm.get('projectEndTimeInternalControl').value);
        // Phần trăm phân bổ
        projectResourceModel.allowcation = this.createProjectResourceInternalForm.get('distributionRateInternalControl').value.value;
        projectResourceModel.includeWeekend = this.isWeekendControlNoiBo.value;
      }
      // Nguồn lực thuê ngoài
      else {
        projectResourceModel.resourceRole = this.listResourceSource.filter(x => x.categoryCode == "TN")[0].categoryId
        // Nhóm nhà thầu
        let contractorGroup = this.createProjectResourceExternalForm.get('vendorGroupControl').value
        projectResourceModel.resourceType = contractorGroup !== null ? contractorGroup.categoryId : this.emptyGuid;
        // Nhà thầu
        let contractor = this.createProjectResourceExternalForm.get('vendorControl').value
        projectResourceModel.objectId = contractor !== null ? contractor.vendorId : this.emptyGuid;
        // Tạo tài khoản
        if (this.isExitsVendorAccount) {
          projectResourceModel.isCreateVendor = false;
        }
        else {
          let createAcc = this.createProjectResourceExternalForm.get('isCreateVendor').value
          projectResourceModel.isCreateVendor = createAcc;
        }
        // Thời gian bắt đầu
        if (this.createProjectResourceExternalForm.get('projectStartTimeControl').value != null)
          projectResourceModel.startTime = convertToUTCTime(this.createProjectResourceExternalForm.get('projectStartTimeControl').value);
        // Thời gian kết thúc
        if (this.createProjectResourceExternalForm.get('projectEndTimeControl').value != null)
          projectResourceModel.endTime = convertToUTCTime(this.createProjectResourceExternalForm.get('projectEndTimeControl').value);
        // Phần trăm phân bổ
        projectResourceModel.allowcation = this.createProjectResourceExternalForm.get('distributionRateControl').value.value;
        projectResourceModel.includeWeekend = this.isWeekendControlNgoai.value;

        //Chi phí theo giờ
        let chiPhiTheoGio = this.chiPhiTheoGioControl.value ?? 0;
        projectResourceModel.chiPhiTheoGio = chiPhiTheoGio == 0 ? 0 : ParseStringToFloat(chiPhiTheoGio.toString());
      }

      return projectResourceModel;
    } catch (error) {
      this.clearToast();
      this.showToast('error', 'Thông báo', error);
    }
  }
  // Thay đổi nhà thầu
  changeVendor(event: any) {
    this.isExitsVendorAccount = false;
    if (this.listVendor.filter(x => x.vendorId.toLocaleLowerCase() == event.value.vendorId.toLocaleLowerCase()).length !== 0 && this.listVendor.filter(x => x.vendorId.toLocaleLowerCase() == event.value.vendorId.toLocaleLowerCase())[0].exitsAccount == true) {
      this.isExitsVendorAccount = true;
    }
  }

  // Thay loại nguồn lực
  changeResourceType(event: any) {
    // // Danh sách nguồn lực và vai trò phải lọc theo
    // let resourceName = this.listEmployee.find(c => c.employeeId == event.value.categoryId);
    // this.resourceNameControl.setValue(resourceName);

    // // Vai trò
    // let role = this.listResourceRole.find(c => c.categoryId == event.value.employeeRole);
    // this.roleControl.setValue(role);



  }
  showMessage(msg: any) {
    this.messageService.add(msg);
  }
  clearToast() {
    this.messageService.clear();
  }
  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
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
