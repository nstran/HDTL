import { Component, ElementRef, OnInit, Renderer2, setTestabilityGetter, ViewChild } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { DialogService } from 'primeng/dynamicdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import * as $ from 'jquery';
import { TranslateService } from '@ngx-translate/core';
import { ProjectService } from '../../services/project.service';
import { ProjectModel } from '../../models/project.model';
import { AddProjectTargetDialogComponent } from '../add-project-target-dialog/add-project-target-dialog.component';

class ProjectTargetModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
}

@Component({
  selector: 'app-project-create',
  templateUrl: './project-create.component.html',
  styleUrls: ['./project-create.component.css']
})
export class ProjectCreateComponent implements OnInit {
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  loading: boolean = false;
  actionAdd: boolean = true;
  minDate: Date = new Date();
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));

  createProjectForm: FormGroup;
  customerControl: FormControl;
  projectCodeControl: FormControl;
  projectNameControl: FormControl;
  projectContractControl: FormControl;
  projectEmployeeControl: FormControl;
  projectSMControl: FormControl;
  projectSubControl: FormControl;
  projectDescriptionControl: FormControl;
  projectTypeControl: FormControl;
  projectScopeControl: FormControl;
  projectStatusControl: FormControl;
  projectBudgetVNDControl: FormControl;
  projectBudgetUSDControl: FormControl;
  projectBudgetNgayCongControl: FormControl;
  projectPriorityControl: FormControl;
  projectStartTimeControl: FormControl;
  projectEndTimeControl: FormControl;
  includeWeekendControl: FormControl;
  giaBanTheoGioControl: FormControl;
  projectStatusPlanControl: FormControl;

  listCustomer: Array<any> = [];
  listContract: Array<any> = [];
  listAllContract: Array<any> = [];
  listEmployee: Array<any> = [];
  listEmployeeManager: Array<any> = [];
  listEmployeePM: Array<any> = [];
  listProjectType: Array<any> = [];
  listProjectScope: Array<any> = [];
  listProjectStatus: Array<any> = [];
  listTargetType: Array<any> = [];
  listTargetUnit: Array<any> = [];

  listButgetType = [{
    code: '1', name: 'Ngày công'
  },
  {
    code: '2', name: 'Tiền'
  }];

  listProjectStatusPlan: Array<any> = [
    { name: 'Chờ phê duyệt', value: false },
    { name: 'Đã duyệt', value: true }
  ]

  columns: Array<any> = [];
  selectedColumns: Array<any> = [];
  listProjectTarget: Array<any> = [];
  selectedProjectTarget: ProjectTargetModel;
  rows: number = 10;
  budgetRequied: boolean = false;

  // notification
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('toggleButton') toggleButton: ElementRef;

  tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink']
  };


  constructor(private projectService: ProjectService,
    private messageService: MessageService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private renderer: Renderer2,
    private getPermission: GetPermission,
    private el: ElementRef) {
    this.renderer.listen('window', 'click', (e: Event) => {
      /**
       * Only run when toggleButton is not clicked
       * If we don't check this, all clicks (even on the toggle button) gets into this
       * section which in the result we might never see the menu open!
       * And the menu itself is checked here, and it's where we check just outside of
       * the menu and button the condition abbove must close the menu
       */
      if (this.toggleButton && this.notifi) {
        if (!this.toggleButton.nativeElement.contains(e.target) &&
          !this.notifi.nativeElement.contains(e.target) &&
          !this.save.nativeElement.contains(e.target)) {
          this.isOpenNotifiError = false;
        }
      }
    });
  }

  async  ngOnInit() {
    let resource = "pro/project/create";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.initForm();
      this.initTable();
      this.getMasterData();
    }
  }

  getMasterData() {
    this.loading = true;
    this.projectService.getMasterProjectCreate().subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listCustomer = result.listCustomer;
        this.listAllContract = result.listContract;
        this.listEmployee = result.listEmployee;
        this.listProjectType = result.listProjectType;
        this.listProjectScope = result.listProjectScope;
        this.listProjectStatus = result.listProjectStatus;
        this.listTargetType = result.listTargetType;
        this.listTargetUnit = result.listTargetUnit;
        //this.setDefaultValue();

        // set default status
        let stt = this.listProjectStatus.find(x => x.categoryCode == 'MOI');
        this.projectStatusControl.setValue(stt);

        this.listEmployeePM = this.listEmployee;
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  initTable() {
    this.columns = [
      { field: 'orderNumber', header: 'STT', width: '5%', textAlign: 'center', color: '#f44336' },
      { field: 'targetTypeDisplay', header: 'Loại mục tiêu', width: '15%', textAlign: 'left', color: '#f44336' },
      { field: 'projectObjectName', header: 'Tên mục tiêu', width: '15%', textAlign: 'left', color: '#f44336' },
      { field: 'targetUnitDisplay', header: 'Đơn vị', width: '10%', textAlign: 'center', color: '#f44336' },
      { field: 'projectObjectValue', header: 'Giá trị', width: '5%', textAlign: 'center', color: '#f44336' },
      { field: 'projectObjectDescriptionShow', header: 'Ghi chú', width: '50%', textAlign: 'left', color: '#f44336' }
    ];
    this.selectedColumns = this.columns;
  }

  initForm() {

    const prt = '[a-zA-Z0-9]*';
    this.customerControl = new FormControl(null);
    this.projectCodeControl = new FormControl(null, [Validators.required, Validators.pattern(prt)]);
    this.projectNameControl = new FormControl(null, [Validators.required]);
    this.projectContractControl = new FormControl(null);
    this.projectEmployeeControl = new FormControl(null);
    this.projectSMControl = new FormControl(null);
    this.projectSubControl = new FormControl(null);
    this.projectDescriptionControl = new FormControl(null);
    this.projectTypeControl = new FormControl(null);
    this.projectScopeControl = new FormControl(null);
    this.projectStatusControl = new FormControl(null);
    this.projectBudgetVNDControl = new FormControl('');
    this.projectBudgetUSDControl = new FormControl('');
    this.projectBudgetNgayCongControl = new FormControl('');
    this.projectPriorityControl = new FormControl(null, Validators.required);
    this.projectStartTimeControl = new FormControl(null, Validators.required);
    this.projectEndTimeControl = new FormControl(null, Validators.required);
    this.includeWeekendControl = new FormControl(false);
    this.giaBanTheoGioControl = new FormControl('0');
    this.projectStatusPlanControl = new FormControl(null);

    this.createProjectForm = new FormGroup({

      customerControl: this.customerControl,
      projectCodeControl: this.projectCodeControl,
      projectNameControl: this.projectNameControl,
      projectContractControl: this.projectContractControl,
      projectEmployeeControl: this.projectEmployeeControl,
      projectSMControl: this.projectSMControl,
      projectSubControl: this.projectSubControl,
      projectDescriptionControl: this.projectDescriptionControl,
      projectTypeControl: this.projectTypeControl,
      projectScopeControl: this.projectScopeControl,
      projectStatusControl: this.projectStatusControl,
      projectBudgetVNDControl: this.projectBudgetVNDControl,
      projectBudgetUSDControl: this.projectBudgetUSDControl,
      projectBudgetNgayCongControl: this.projectBudgetNgayCongControl,
      projectPriorityControl: this.projectPriorityControl,
      projectStartTimeControl: this.projectStartTimeControl,
      projectEndTimeControl: this.projectEndTimeControl,
      includeWeekendControl: this.includeWeekendControl,
      giaBanTheoGioControl: this.giaBanTheoGioControl,
      projectStatusPlanControl: this.projectStatusPlanControl,
    });
  }


  changeCustomer(event: any) {
    let _listContract = this.listAllContract.filter(c => c.customerId == event.value.customerId);
    this.listContract = _listContract;
  }

  createProject() {
    if (!this.createProjectForm.valid) {
      Object.keys(this.createProjectForm.controls).forEach(key => {
        if (!this.createProjectForm.controls[key].valid) {
          this.createProjectForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.createProjectForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });

      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }

      if ((this.projectBudgetVNDControl.value == null && this.projectBudgetUSDControl.value == null && this.projectBudgetNgayCongControl.value == null) ||
        (this.projectBudgetVNDControl.value == "" && this.projectBudgetUSDControl.value == "" && this.projectBudgetNgayCongControl.value == "")) {
        this.budgetRequied = true;
      }

    }
    else if ((this.projectBudgetVNDControl.value == null && this.projectBudgetUSDControl.value == null && this.projectBudgetNgayCongControl.value == null) ||
      (this.projectBudgetVNDControl.value == "" && this.projectBudgetUSDControl.value == "" && this.projectBudgetNgayCongControl.value == "")) {
      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.budgetRequied = true
    }
    else {
      this.budgetRequied = false;
      this.isInvalidForm = false;  //Ẩn icon-warning-active
      this.isOpenNotifiError = false;  //Ẩn message lỗi

      let status = this.createProjectForm.get('projectStatusControl').value;
      let projectManager = this.projectEmployeeControl.value;
      if ((status.categoryCode == "DTK" || status.categoryCode == "HTH" || status.categoryCode == "DON") && projectManager == null) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Dự án hiện chưa có người quản lý, bạn phải chọn quản lý mới có thể tiếp thục hành động này?' };
        this.showMessage(msg);
      }
      else {

        let projectModel: ProjectModel = this.mapFormToProjectModel();

        this.loading = true;

        this.projectService.createProject(projectModel, this.listProjectTarget, this.auth.UserId).subscribe(response => {
          this.loading = false;
          let result = <any>response;
          if (result.statusCode == 200) {
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
            localStorage.setItem("Project_ID", result.projectId);
            setTimeout(() => {
              this.router.navigate(['/project/detail', { projectId: result.projectId }]);
            }, 1000);
          }
          else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          }
        });
      }
    }
  }

  mapFormToProjectModel(): ProjectModel {

    let projectModel = new ProjectModel();

    // ten du an
    projectModel.projectName = this.projectNameControl.value;

    // ma du an
    projectModel.projectCode = this.createProjectForm.get('projectCodeControl').value;

    // khach hang
    let customer = this.createProjectForm.get('customerControl').value
    projectModel.customerId = customer ? customer.customerId : this.emptyGuid;

    // hop dong
    let contract = this.createProjectForm.get('projectContractControl').value
    projectModel.contractId = contract ? contract.contractId : this.emptyGuid;

    // quan ly
    let employee = this.createProjectForm.get('projectEmployeeControl').value
    projectModel.projectManagerId = employee ? employee.employeeId : this.emptyGuid;

    // quản lý cấp cao
    projectModel.employeeSM = [];
    let listSelectedemployeeSm = this.projectSMControl.value;
    if (listSelectedemployeeSm != null) {
      listSelectedemployeeSm.forEach(item => {
        projectModel.employeeSM.push(item.employeeId);
      });
    }

    // đồng quản lý
    projectModel.employeeSub = [];
    let listSelectedemployeeSub = this.projectSubControl.value;
    if (listSelectedemployeeSub != null) {
      listSelectedemployeeSub.forEach(item => {
        projectModel.employeeSub.push(item.employeeId);
      });
    }

    let type = this.createProjectForm.get('projectTypeControl').value
    projectModel.projectType = type !== null ? type.categoryId : this.emptyGuid;

    let scope = this.createProjectForm.get('projectScopeControl').value
    projectModel.projectSize = scope !== null ? scope.categoryId : this.emptyGuid;

    let status = this.createProjectForm.get('projectStatusControl').value
    projectModel.projectStatus = status !== null ? status.categoryId : this.emptyGuid;

    // let bugetType = this.createProjectForm.get('projectBugetTypeControl').value


    projectModel.budgetVND = this.projectBudgetVNDControl.value ? this.projectBudgetVNDControl.value : null;
    projectModel.budgetUSD = this.projectBudgetUSDControl.value ? this.projectBudgetUSDControl.value : null;
    projectModel.budgetNgayCong = this.projectBudgetNgayCongControl.value ? this.projectBudgetNgayCongControl.value : null;
    projectModel.giaBanTheoGio = this.giaBanTheoGioControl.value ? this.giaBanTheoGioControl.value : null;
    projectModel.projectStatusPlan = this.projectStatusPlanControl.value ? this.projectStatusPlanControl.value.value : null;

    projectModel.priority = this.createProjectForm.get('projectPriorityControl').value;

    // ngay bat dau du kien
    let startDate = this.createProjectForm.get('projectStartTimeControl').value ? convertToUTCTime(this.createProjectForm.get('projectStartTimeControl').value) : null;
    projectModel.projectStartDate = startDate;

    // ngay ket thuc du kien
    let endDate = this.createProjectForm.get('projectEndTimeControl').value ? convertToUTCTime(this.createProjectForm.get('projectEndTimeControl').value) : null;
    projectModel.projectEndDate = endDate;

    let projectDescription = this.projectDescriptionControl.value ? this.projectDescriptionControl.value : null;
    projectModel.description = projectDescription;

    projectModel.createdById = this.auth.UserId;
    projectModel.createdDate = convertToUTCTime(new Date());
    projectModel.updatedById = null;
    projectModel.updatedDate = null;
    return projectModel;
  }

  changePrioriry() {
  }

  goBackToList() {
    if (this.createProjectForm.untouched && this.listProjectTarget.length == 0) {
      this.router.navigate(['/project/list']);
    } else {
      //confirm dialog
      this.confirmationService.confirm({
        message: 'Các thay đổi sẽ không được lưu lại.Hành động này không thể được hoàn tác, bạn có chắc chắn muốn hủy?',
        accept: () => {
          setTimeout(() => {
            this.router.navigate(['/project/list']);
          }, 500);
        }
      });
    }
  }

  addProjectTarget() {
    let ref = this.dialogService.open(AddProjectTargetDialogComponent, {
      data: {
        listTargetType: this.listTargetType,
        listTargetUnit: this.listTargetUnit
      },
      header: 'Thêm mới mục tiêu',
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px",
        "max-height": "600px",
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status === true) {
          let newTarget = result.targetModel;
          newTarget.projectObjectDescriptionShow = convertToPlain(newTarget.projectObjectDescription)
          newTarget.orderNumber = this.listProjectTarget.length + 1;
          this.listProjectTarget = [...this.listProjectTarget, newTarget];
        }
      }
    });
  }

  deleteProjectTarget(rowData: any) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.listProjectTarget = this.listProjectTarget.filter(e => e != rowData);
        let index = 1;
        this.listProjectTarget.forEach(item => { item.Stt = index; index++; })
      }
    });
  }

  editProjectTarget(rowData: any) {
    let ref = this.dialogService.open(AddProjectTargetDialogComponent, {
      header: 'Chỉnh sửa mục tiêu',
      data: {
        isEdit: true,
        listTargetType: this.listTargetType,
        listTargetUnit: this.listTargetUnit,
        target: rowData
      },
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px",
        "max-height": "600px",
      }
    });

    ref.onClose.subscribe(async (result: any) => {
      if (result) {
        if (result.status === true) {
          //replace rowdata cũ
          let newtarget = result.targetModel;
          newtarget.projectObjectDescriptionShow = convertToPlain(newtarget.projectObjectDescription)
          newtarget.orderNumber = rowData.orderNumber; //skip Stt old
          const index = this.listProjectTarget.indexOf(rowData);
          this.listProjectTarget[index] = newtarget;
          this.listProjectTarget = [...this.listProjectTarget];
        }
      }
    });
  }

  ChangeManager(event) {
    let emp = event.value;
    if (emp) {
      // quan ly cap cao
      this.projectSMControl.setValue(null);
      if (emp.isManager) {
        this.listEmployeeManager = this.listEmployee.filter(x => x.isManager && x.organizationLevel < emp.organizationLevel);
      } else {
        this.listEmployeeManager = this.listEmployee.filter(x => x.isManager && x.organizationLevel <= emp.organizationLevel);
      }

      // đồng quản lý
      this.projectSubControl.setValue(null);
      this.listEmployeePM = this.listEmployee.filter(x => x.employeeId != emp.employeeId);

    } else {
      this.projectSMControl.setValue(null);
      this.projectSubControl.setValue(null);
      this.listEmployeeManager = [];
      this.listEmployeePM = this.listEmployee;
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  ChangeBudget() {
    if (this.projectBudgetVNDControl.value == '' && this.projectBudgetUSDControl.value == '' && this.projectBudgetNgayCongControl.value == '') {
      this.budgetRequied = true;
    }
    else {
      this.budgetRequied = false;
    }
  }
}
function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function convertToPlain(html){

  // Create a new div element
  var tempDivElement = document.createElement("div");

  // Set the HTML content with the given value
  tempDivElement.innerHTML = html;

  // Retrieve the text property of the element
  return tempDivElement.textContent || tempDivElement.innerText || "";
}
