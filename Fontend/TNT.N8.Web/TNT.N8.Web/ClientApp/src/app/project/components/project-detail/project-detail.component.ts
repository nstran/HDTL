import { Component, ElementRef, ViewChild, OnInit, Renderer2, ChangeDetectorRef } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { DialogService } from 'primeng/dynamicdialog';
import { MessageService, ConfirmationService, MenuItem } from 'primeng/api';
import * as $ from 'jquery';
import { FileUpload } from 'primeng/fileupload';
import { ProjectService } from '../../services/project.service';
import { ProjectModel } from '../../models/project.model';
import { AddProjectTargetDialogComponent } from '../add-project-target-dialog/add-project-target-dialog.component';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { NoteModel } from '../../../shared/models/note.model';
import { NoteService } from '../../../shared/services/note.service';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { MatDialog } from '@angular/material/dialog';
import { AuthenticationService } from '../../../shared/services/authentication.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { ForderConfigurationService } from '../../../admin/components/folder-configuration/services/folder-configuration.service';
import { Paginator } from 'primeng/paginator';

class ProjectTargetModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
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

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
  createdById: string;
  createdDate: Date;
  uploadByName: string;
}

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

interface FileNameExists {
  oldFileName: string;
  newFileName: string
}

interface ProjectObjective {
  orderNumber: number;
  projectObjectDescription: string;
  projectObjectName: string;
  projectObjectType: string;
  projectObjectUnit: string;
  projectObjectValue: number;
  targetTypeDisplay: string;
  targetUnitDisplay: string;
}

@Component({
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.css']
})

export class ProjectDetailComponent implements OnInit {
  projectId: string = null;
  currentURl: string = '';
  listResource: Array<string> = [];
  listPermissionResourceActive: string = localStorage.getItem("ListPermissionResource");
  notificationList: Array<any> = [];
  NotificationContent: string;
  notificationNumber = 0;

  loading: boolean = false;
  minDate: Date = new Date();
  /*Check user permission*/

  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  actionAdd: boolean = true;
  actionImport: boolean = true;
  actionDelete: boolean = true;
  actionEdit: boolean = true;
  actionEditNote: boolean = true;
  actionDeleteScope: boolean = false;
  actionEditScope: boolean = false;
  /*End*/

  detailProjectForm: FormGroup;
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
  projectActualStartDateControl: FormControl;
  projectActualEndDateControl: FormControl;
  includeWeekendControl: FormControl;
  giaBanTheoGioControl: FormControl;
  projectStatusPlanControl: FormControl;
  projectNgayKyNghiemThuControl: FormControl;

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

  isApproveProject: boolean = false;

  role: string = '';

  columns: Array<any> = [];
  selectedColumns: Array<any> = [];
  colsFile: Array<any> = [];
  listProjectTarget: Array<any> = [];
  selectedProjectTarget: ProjectTargetModel;
  rows: number = 10;
  budgetRequied: boolean = false;
  hasTaskInProgress: boolean = false;
  /*NOTE*/
  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  listUpdateNoteDocument: Array<NoteDocument> = [];
  listDeleteNoteDocument: Array<NoteDocument> = [];
  noteHistory: Array<Note> = [];

  touched: boolean = false;

  noteId: string = null;
  noteContent: string = '';
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  uploadedNoteFiles: any[] = [];
  // listNoteFile: any[] = [];
  @ViewChild('fileNoteUpload') fileNoteUpload: FileUpload;
  /*End : Note*/

  statusCode: string = '';
  contractDefault: any;
  customerDefault: any;

  // notification
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('toggleButton') toggleButton: ElementRef;

  isHavePM: boolean = false;
  projectCodeName: string = '';
  @ViewChild('paginator', { static: true }) paginator: Paginator
  totalRecordsNote: number = 0;

  listProject: Array<ProjectModel> = [];
  selectedProject: ProjectModel = null;

  tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink']
  };
  isReadonly: boolean = false;
  isShowGiaBanTheoGio: boolean = false;

  constructor(
    private notiService: NotificationService,
    private authenticationService: AuthenticationService,
    private projectService: ProjectService,
    private messageService: MessageService,
    private folderService: ForderConfigurationService,
    private router: Router,
    private getPermission: GetPermission,
    private imageService: ImageUploadService,
    private noteService: NoteService,
    private route: ActivatedRoute,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    public dialog: MatDialog,
    private renderer: Renderer2,
    private def: ChangeDetectorRef,
    private el: ElementRef) {
    $("body").addClass('sidebar-collapse');
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



  async ngOnInit() {
    this.initForm();
    this.initTable();

    this.loading = true;

    this.route.params.subscribe(async params => {
      this.projectId = params['projectId'];

      let resource = "pro/project/detail";
      let permission: any = await this.getPermission.getPermissionLocal(this.listPermissionResourceActive, resource);
      if (permission.status == false) {
        this.router.navigate(['/home']);
      }
      else {
        let listCurrentActionResource = permission.listCurrentActionResource;
        if (listCurrentActionResource.indexOf("editnote") == -1) {
          this.actionEditNote = false;
        }
        this.getMasterData(this.projectId);
      }
    });

  }

  getMasterData(projectId: string) {
    this.loading = true;
    this.projectService.getMasterUpdateProjectCreate(projectId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.isShowGiaBanTheoGio = result.isShowGiaBanTheoGio;
        this.listCustomer = result.listCustomer;
        this.listAllContract = result.listContract;
        this.listEmployee = result.listEmployee;
        this.listProjectType = result.listProjectType;
        this.listProjectScope = result.listProjectScope;
        this.listProjectStatus = result.listProjectStatus;
        this.listTargetType = result.listTargetType;
        this.listTargetUnit = result.listTargetUnit;
        this.listProjectTarget = result.listProjectTarget;
        this.listProjectTarget.forEach(item => {
          item.projectObjectDescriptionShow = convertToPlain(item.projectObjectDescription);
        })


        this.listProject = result.listProject;
        this.selectedProject = this.listProject.find(c => c.projectId == this.projectId);

        this.totalRecordsNote = result.totalRecordsNote;
        this.noteHistory = result.listNote;
        this.listEmployeePM = this.listEmployee;
        this.role = result.role;
        this.handleNoteContent();

        if (result.hasTaskInProgress) {
          this.projectContractControl.disable();
          this.customerControl.disable();
        }
        else {
          this.projectContractControl.enable();
          this.customerControl.enable();
        }

        this.setDefaultValueForm(
          result.project
        );
        this.statusCode = result.project.projectStatusCode;
        switch (this.statusCode) {
          case 'MOI':
            this.listProjectStatus = this.listProjectStatus.filter(x => x.categoryCode == 'MOI' || x.categoryCode == 'DTK' || x.categoryCode == 'TDU' || x.categoryCode == 'HTH' || x.categoryCode == 'DON' || x.categoryCode == 'HUY');
            break;
          case 'DTK':
            this.listProjectStatus = this.listProjectStatus.filter(x => x.categoryCode == 'DTK' || x.categoryCode == 'TDU' || x.categoryCode == 'HTH');
            break;
          case 'TDU':
            this.listProjectStatus = this.listProjectStatus.filter(x => x.categoryCode == 'TDU' || x.categoryCode == 'DON' || x.categoryCode == 'HUY');
            break;
          case 'HTH':
            this.listProjectStatus = this.listProjectStatus.filter(x => x.categoryCode == 'HTH' || x.categoryCode == 'DON' || x.categoryCode == 'DTK');
            break;
          case 'DON':
            this.listProjectStatus = this.listProjectStatus.filter(x => x.categoryCode == 'DTK' || x.categoryCode == 'DON');
            break;
          case 'HUY':
            this.listProjectStatus = this.listProjectStatus.filter(x => x.categoryCode == 'HUY');
            break;
          default:
            break;
        }

        if (this.statusCode == 'TDU' || this.statusCode == 'HUY') {
          this.detailProjectForm.disable();
          this.projectDescriptionControl.enable();
          this.isReadonly = true
        } else {
          this.detailProjectForm.enable();
        }

        if (this.statusCode == 'DTK' && (result.project.actualStartDate != null && result.project.actualStartDate != undefined && result.project.actualStartDate != '')) {
          this.projectActualStartDateControl.disable();
        }

        if (this.statusCode != 'MOI') {
          this.customerControl.disable();
          this.projectContractControl.disable();
        }

        if (!this.isHavePM) {
          this.projectEmployeeControl.enable();
        }

        // if(this.isApproveProject){
        //   this.detailProjectForm.disable();
        //   this.projectDescriptionControl.enable();
        // }

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

    this.colsFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'documentSize', header: 'Kích thước', width: '50%', textAlign: 'left', type: 'number' },
      { field: 'updatedDate', header: 'Ngày tạo', width: '50%', textAlign: 'left', type: 'date' },
      // { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'left', type: 'string' },
    ];
  }

  initForm() {
    const prt = '[a-zA-Z0-9]*';
    this.customerControl = new FormControl(null);
    this.projectCodeControl = new FormControl(null, [Validators.required, Validators.pattern(prt)]);
    this.projectNameControl = new FormControl(null, [Validators.required]);
    this.projectContractControl = new FormControl;
    this.projectEmployeeControl = new FormControl;
    this.projectSMControl = new FormControl;
    this.projectSubControl = new FormControl;
    this.projectDescriptionControl = new FormControl;
    this.projectTypeControl = new FormControl;
    this.projectScopeControl = new FormControl;
    this.projectStatusControl = new FormControl;
    this.projectBudgetVNDControl = new FormControl;
    this.projectBudgetUSDControl = new FormControl;
    this.projectBudgetNgayCongControl = new FormControl;
    this.projectPriorityControl = new FormControl;
    this.projectStartTimeControl = new FormControl(null, Validators.required);
    this.projectEndTimeControl = new FormControl(null, Validators.required);
    this.projectActualStartDateControl = new FormControl(null);
    this.projectActualEndDateControl = new FormControl(null);
    this.includeWeekendControl = new FormControl(false);
    this.giaBanTheoGioControl = new FormControl('0');
    this.projectStatusPlanControl = new FormControl(null);
    this.projectNgayKyNghiemThuControl = new FormControl(null);

    this.detailProjectForm = new FormGroup({

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
      projectActualStartDateControl: this.projectActualStartDateControl,
      projectActualEndDateControl: this.projectActualEndDateControl,
      includeWeekendControl: this.includeWeekendControl,
      giaBanTheoGioControl: this.giaBanTheoGioControl,
      projectStatusPlanControl: this.projectStatusPlanControl,
      projectNgayKyNghiemThuControl: this.projectNgayKyNghiemThuControl
    });
  }

  async setDefaultValueForm(project) {
    this.projectNameControl.setValue(project.projectName);
    this.projectCodeControl.setValue(project.projectCode);
    this.includeWeekendControl.setValue(project.includeWeekend);
    let projectType = this.listProjectType.find(e => e.categoryId == project.projectType);
    if (projectType) this.projectTypeControl.setValue(projectType);

    let projectStatus = this.listProjectStatus.find(e => e.categoryId == project.projectStatus);
    if (projectStatus) this.projectStatusControl.setValue(projectStatus);

    let customer = this.listCustomer.find(c => c.customerId == project.customerId);
    this.customerDefault = customer;
    if (customer) this.customerControl.setValue(customer)


    this.projectDescriptionControl.setValue(project.description);

    let _listContract = this.listAllContract.filter(c => c.customerId == project.customerId);
    if (_listContract) this.listContract = _listContract;

    let employee = this.listEmployee.find(e => e.employeeId == project.projectManagerId);
    if (employee) {
      this.projectEmployeeControl.setValue(employee);
      this.ChangeManager(employee);
      this.isHavePM = true;
    }
    else {
      this.isHavePM = false;
    }

    let contract = this.listContract.find(e => e.contractId == project.contractId);
    if (contract) {
      this.projectContractControl.setValue(contract);
      this.contractDefault = contract;
    }

    let projectScope = this.listProjectScope.find(e => e.categoryId == project.projectSize);
    if (projectScope) this.projectScopeControl.setValue(projectScope);

    this.projectBudgetVNDControl.setValue(project.budgetVnd);
    this.projectBudgetUSDControl.setValue(project.budgetUsd);
    this.projectBudgetNgayCongControl.setValue(project.budgetNgayCong);
    this.giaBanTheoGioControl.setValue(project.giaBanTheoGio);

    this.projectStatusPlanControl.setValue(this.listProjectStatusPlan.find(x => x.value == project.projectStatusPlan));
    this.isApproveProject = project.projectStatusPlan

    this.projectPriorityControl.setValue(project.priority);

    let employeeSMList: Array<any> = [];
    project.employeeSM.forEach(item => {
      let empSM = this.listEmployee.find(em => em.employeeId == item);
      if (empSM) employeeSMList.push(empSM);
    })
    this.projectSMControl.setValue(employeeSMList);


    let employeeSub = this.listEmployee.filter(em => project.employeeSub.includes(em.employeeId));
    if (employeeSub) this.projectSubControl.setValue(employeeSub);

    let projectStart = project.projectStartDate ? new Date(project.projectStartDate) : null;
    if (projectStart) this.projectStartTimeControl.setValue(projectStart);

    let projectEnd = project.projectEndDate ? new Date(project.projectEndDate) : null;
    if (projectEnd) this.projectEndTimeControl.setValue(projectEnd);

    let actualStart = project.actualStartDate ? new Date(project.actualStartDate) : null;
    this.projectActualStartDateControl.setValue(actualStart);

    let actualEnd = project.actualEndDate ? new Date(project.actualEndDate) : null;
    this.projectActualEndDateControl.setValue(actualEnd);

    this.projectCodeName = project.projectCode + ' - ' + project.projectName;

    let ngayKyNghiemThu = project.ngayKyNghiemThu ? new Date(project.ngayKyNghiemThu) : null;
    this.projectNgayKyNghiemThuControl.setValue(ngayKyNghiemThu);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  changeCustomer(event: any) {
    if (this.customerDefault) {
      this.confirmationService.confirm({
        message: 'Tùy chọn này sẽ làm thay đổi dữ liệu thông tin khách hàng đã chọn trước đó. Bạn có chắc chắn muốn đổi?',
        accept: () => {
          if (event) {
            let _listContract = this.listAllContract.filter(c => c.customerId == event.customerId);
            this.listContract = _listContract;
          }
          else {
            this.listContract = []
          }
          this.projectContractControl.setValue(null);
        },
        reject: () => {
          this.customerControl.setValue(this.customerDefault);
        }
      });
    }
    else {
      let _listContract = this.listAllContract.filter(c => c.customerId == event.customerId);
      this.listContract = _listContract;
    }
  }

  changeContract(event: any) {
    if (this.contractDefault) {
      this.confirmationService.confirm({
        message: 'Tùy chọn này sẽ làm thay đổi dữ liệu thông tin hạng mục dự án liên quan đến báo giá của hợp đồng đã chọn trước đó. Bạn có chắc chắn muốn đổi?',
        reject: () => {
          this.projectContractControl.setValue(this.contractDefault);
        }
      });
    }
  }

  changePrioriry() {
  }

  goBackToList() {
    // this.router.navigate(['/project/list']);

    if (!this.detailProjectForm.touched) {
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

  saveProject() {
    if (!this.detailProjectForm.valid) {
      Object.keys(this.detailProjectForm.controls).forEach(key => {
        if (!this.detailProjectForm.controls[key].valid) {
          this.detailProjectForm.controls[key].markAsTouched();
        }
      });
      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.detailProjectForm.statusChanges.subscribe((validity: string) => {
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
        this.budgetRequied = true
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

      let status = this.detailProjectForm.get('projectStatusControl').value;
      let projectManager = this.projectEmployeeControl.value;
      if (this.statusCode == "MOI" && (status.categoryCode == "DTK" || status.categoryCode == "HTH" || status.categoryCode == "DON") && projectManager == null) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Dự án hiện chưa có người quản lý, bạn phải chọn quản lý mới có thể tiếp thục hành động này?' };
        this.showMessage(msg);
      }
      else {
        this.updateProject();
      }
    }
  }

  mapFormToProjectModel(): ProjectModel {
    let projectModel = new ProjectModel();
    projectModel.projectId = this.projectId;
    projectModel.projectName = this.projectNameControl.value;
    projectModel.description = this.projectDescriptionControl.value;
    projectModel.projectCode = this.detailProjectForm.get('projectCodeControl').value;
    let customer = this.detailProjectForm.get('customerControl').value
    projectModel.customerId = customer !== null ? customer.customerId : this.emptyGuid;

    let contract = this.detailProjectForm.get('projectContractControl').value
    projectModel.contractId = contract !== null ? contract.contractId : this.emptyGuid;
    let employee = this.detailProjectForm.get('projectEmployeeControl').value
    projectModel.projectManagerId = employee !== null ? employee.employeeId : this.emptyGuid;

    projectModel.employeeSM = [];
    //danh sách nhân viên cao cấp
    let listSelectedemployeeSm = this.projectSMControl.value;
    if (listSelectedemployeeSm != null) {
      listSelectedemployeeSm.forEach(item => {
        projectModel.employeeSM.push(item.employeeId);
      });
    }

    projectModel.employeeSub = [];
    //danh sách sub nhân viên
    let listSelectedemployeeSub = this.projectSubControl.value;
    if (listSelectedemployeeSub != null) {
      listSelectedemployeeSub.forEach(item => {
        projectModel.employeeSub.push(item.employeeId);
      });
    }

    let type = this.detailProjectForm.get('projectTypeControl').value
    projectModel.projectType = type !== null ? type.categoryId : this.emptyGuid;

    let scope = this.detailProjectForm.get('projectScopeControl').value
    projectModel.projectSize = scope !== null ? scope.categoryId : this.emptyGuid;

    let status = this.detailProjectForm.get('projectStatusControl').value
    projectModel.projectStatus = status !== null ? status.categoryId : this.emptyGuid;

    // let bugetType = this.detailProjectForm.get('projectBugetTypeControl').value
    // projectModel.butgetType = bugetType !== null ? bugetType.code : null;

    projectModel.budgetVND = this.projectBudgetVNDControl.value ? this.projectBudgetVNDControl.value : null;
    projectModel.budgetUSD = this.projectBudgetUSDControl.value ? this.projectBudgetUSDControl.value : null;
    projectModel.budgetNgayCong = this.projectBudgetNgayCongControl.value ? this.projectBudgetNgayCongControl.value : null;
    projectModel.giaBanTheoGio = this.giaBanTheoGioControl.value ? this.giaBanTheoGioControl.value : 0;
    projectModel.projectStatusPlan = this.projectStatusPlanControl.value ? this.projectStatusPlanControl.value.value : null;
    projectModel.ngayKyNghiemThu = convertToUTCTime(this.projectNgayKyNghiemThuControl.value);

    projectModel.priority = this.detailProjectForm.get('projectPriorityControl').value;
    // ngay bat dau du kien
    let startDate = this.detailProjectForm.get('projectStartTimeControl').value ? convertToUTCTime(this.detailProjectForm.get('projectStartTimeControl').value) : null
    projectModel.projectStartDate = startDate;

    // ngay ket thuc du kien
    let endDate = this.detailProjectForm.get('projectEndTimeControl').value ? convertToUTCTime(this.detailProjectForm.get('projectEndTimeControl').value) : null
    projectModel.projectEndDate = endDate;

    // ngày băt dau thuc te
    let actualStart = this.projectActualStartDateControl.value ? convertToUTCTime(this.projectActualStartDateControl.value) : null;
    projectModel.actualStartDate = actualStart;

    //ngày ket thuc thuc te
    let actualEnd = this.projectActualEndDateControl.value ? convertToUTCTime(this.projectActualEndDateControl.value) : null;
    projectModel.actualEndDate = actualEnd;

    projectModel.includeWeekend = this.detailProjectForm.get('includeWeekendControl').value;

    projectModel.createdById = this.auth.UserId;
    projectModel.createdDate = convertToUTCTime(new Date());
    projectModel.updatedById = null;
    projectModel.updatedDate = null;
    return projectModel;
  }

  updateProject() {
    let projectModel: ProjectModel = this.mapFormToProjectModel();
    this.loading = true;
    this.projectService.updateProject(projectModel, this.listProjectTarget, this.auth.UserId).subscribe(response => {
      this.loading = false;
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.clearToast();
        this.showToast('success', 'Thông báo', 'Cập nhật dự án thành công');
        this.getMasterData(this.projectId);
      }
      else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    }, error => { this.loading = false; });
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
          newTarget.orderNumber = this.listProjectTarget.length + 1;
          newTarget.projectObjectDescriptionShow = convertToPlain(newTarget.projectObjectDescription)

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
        statusCode: this.statusCode,
        isApproveProject: this.isApproveProject,
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

          newtarget.orderNumber = rowData.orderNumber; //skip Stt old
          newtarget.projectObjectDescriptionShow = convertToPlain(newtarget.projectObjectDescription)

          const index = this.listProjectTarget.indexOf(rowData);
          this.listProjectTarget[index] = newtarget;
          this.listProjectTarget = [...this.listProjectTarget];
        }
      }
    });
  }
  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }
  // Event thay đổi nội dung ghi chú
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  cancelNote() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn hủy ghi chú này?',
      accept: () => {
        this.noteId = null;
        this.noteContent = null;
        this.uploadedNoteFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();
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
    let folderType = `${this.projectCodeControl.value}_PROJECT_FILE`
    // if (this.uploadedNoteFiles.length > 0) {
    //   let listFileNameExists: Array<FileNameExists> = [];
    //   let result: any = await this.projectService.uploadFileForOptionAsync(this.uploadedNoteFiles, 'Note', this.projectCodeName);
    //   listFileNameExists = result.listFileNameExists;

    //   for (var x = 0; x < this.uploadedNoteFiles.length; ++x) {
    //     let noteDocument = new NoteDocumentModel();
    //     noteDocument.DocumentName = this.uploadedNoteFiles[x].name;
    //     let fileExists = listFileNameExists.find(f => f.oldFileName == this.uploadedNoteFiles[x].name);
    //     if (fileExists) {
    //       noteDocument.DocumentName = fileExists.newFileName;
    //     }
    //     noteDocument.DocumentSize = this.uploadedNoteFiles[x].size.toString();
    //     this.listNoteDocumentModel.push(noteDocument);
    //   }


    // this.uploadedNoteFiles.forEach(item => {
    //   let fileUpload: FileUploadModel = new FileUploadModel();
    //   fileUpload.FileInFolder = new FileInFolder();
    //   fileUpload.FileInFolder.active = true;
    //   let index = item.name.lastIndexOf(".");
    //   let name = item.name.substring(0, index);
    //   fileUpload.FileInFolder.fileName = name;
    //   fileUpload.FileInFolder.fileExtension = item.name.substring(index, item.name.length - index);
    //   fileUpload.FileInFolder.size = item.size;
    //   fileUpload.FileInFolder.objectId = this.projectId;
    //   fileUpload.FileInFolder.objectType = 'QLDA';
    //   fileUpload.FileSave = item;
    //   listFileUploadModel.push(fileUpload);
    // });

    // let result: any = this.folderService.uploadFileByFolderType(projectType, listFileUploadModel, this.projectId);
    // }


    let listFileUploadModel: Array<FileUploadModel> = [];
    this.uploadedNoteFiles.forEach(item => {
      let fileUpload: FileUploadModel = new FileUploadModel();
      fileUpload.FileInFolder = new FileInFolder();
      fileUpload.FileInFolder.active = true;
      let index = item.name.lastIndexOf(".");
      let name = item.name.substring(0, index);
      fileUpload.FileInFolder.fileName = name;
      fileUpload.FileInFolder.fileExtension = item.name.substring(index + 1);
      fileUpload.FileInFolder.size = item.size;
      fileUpload.FileInFolder.objectType = 'NOTE';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    let noteModel = new NoteModel();
    if (!this.noteId) {
      /*Tạo mới ghi chú*/
      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.projectId;
      noteModel.ObjectType = 'Project';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/
      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.projectId;
      noteModel.ObjectType = 'Project';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    }
    if (noteModel.Description == "" && this.listNoteDocumentModel.length == 0) {

      this.loading = false;
      return;
    }

    this.listDeleteNoteDocument.forEach(item => {
      // Xóa file vật lý
      let result: any = this.imageService.deleteFileForOptionAsync('Project', item.documentName);

      // xóa file trong DB
      this.noteService.deleteNoteDocument(item.noteId, item.noteDocumentId).subscribe(response => {
        let result: any = response;
        // this.loading = false;
        if (result.statusCode != 200) {
          let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    })

    this.noteHistory = [];
    this.noteService.createNoteForProjectDetail(noteModel, folderType, listFileUploadModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedNoteFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.noteContent = null;
        this.listUpdateNoteDocument = [];
        this.listDeleteNoteDocument = [];
        listFileUploadModel = [];
        this.noteId = null;
        this.isEditNote = false;

        /*Reshow Time Line*/
        this.paginator.changePage(0);
        let messageCode = "Thêm ghi chú thành công";
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: messageCode };
        this.showMessage(mgs);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Xử lý và hiển thị lại nội dung ghi chú*/
  handleNoteContent() {
    this.noteHistory.forEach(element => {
      $('#' + element.noteId).find('.short-content').append('');
      $('#' + element.noteId).find('.full-content').append('');
      this.def.detectChanges();
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
    // this.uploadedNoteFiles = this.listNoteFile;
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

  /* Event thêm file dược chọn vào list file note */
  handleNoteFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= 10000000) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedNoteFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedNoteFiles.push(file);
          }
        }
      }
    }
  }

  /*Event khi click xóa từng file */
  removeNoteFile(event) {
    let index = this.uploadedNoteFiles.indexOf(event.file);
    this.uploadedNoteFiles.splice(index, 1);
  }

  /*Event khi click xóa toàn bộ file */
  clearAllNoteFile() {
    this.uploadedNoteFiles = [];
  }

  clearToast() {
    this.messageService.clear();
  }

  goToNotiUrl(item: any, notificationId: string, id: string, code: string) {
    this.notiService.removeNotification(notificationId, this.auth.UserId).subscribe(response => {
      this.loading = true;
      if (code == "PRO_REQ") {
        this.router.navigate(['/procurement-request/view', { id: id }]);
      }
      if (code == "PAY_REQ") {
        this.router.navigate(['/accounting/payment-request-detail', { requestId: id }]);
      }
      if (code == "EMP_REQ") {
        this.router.navigate(['/employee/request/detail', { requestId: id }]);
      }
      if (code == "EMP_SLR") {
        this.NotificationContent = item.content;
        let month = this.NotificationContent.split(" ")[this.NotificationContent.split(" ").indexOf("tháng") + 1];
        this.router.navigate(['employee/employee-salary/list', { MonthSalaryRequestParam: month }]);
      }
      if (code == "TEA_SLR") {
        this.router.navigate(['/employee/teacher-salary/list']);
      }
      if (code == "AST_SLR") {
        this.router.navigate(['/employee/assistant-salary/list']);
      }
      this.loading = false;
    });
  }

  goToNotification() {
    this.router.navigate(['/notification']);
  }

  logout() {
    this.authenticationService.logout();
    this.router.navigate(['/login']);
  }

  ChangeManager(event) {
    if (event) {
      // quan ly cap cao
      this.projectSMControl.setValue(null);
      if (event.isManager) {
        this.listEmployeeManager = this.listEmployee.filter(x => x.isManager && x.organizationLevel < event.organizationLevel);
      } else {
        this.listEmployeeManager = this.listEmployee.filter(x => x.isManager && x.organizationLevel <= event.organizationLevel);
      }

      // đồng quản lý
      this.projectSubControl.setValue(null);
      this.listEmployeePM = this.listEmployee.filter(x => x.employeeId != event.employeeId);
    } else {
      this.projectSMControl.setValue(null);
      this.projectSubControl.setValue(null);
      this.listEmployeeManager = [];
      this.listEmployeePM = this.listEmployee;
    }
  }

  /* Tải file ở ghi chú (dòng thời gian) */
  downloadNoteFile(fileInfor: NoteDocument) {
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

  /*Event khi xóa 1 file trong comment đã lưu trên server*/
  deleteNoteFile(file: NoteDocument) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listUpdateNoteDocument.indexOf(file);
        this.listUpdateNoteDocument.splice(index, 1);
        this.listDeleteNoteDocument.push(file);
        // this.loading = true;
      }
    });
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
      this.isInvalidForm = false;
    }
  }


  async paginate(event) {
    //event.first = Index of the first record
    //event.rows = Number of rows to display in new page
    //event.page = Index of the new page
    //event.pageCount = Total number of pages

    let result: any = await this.projectService.pagingProjectNote(this.projectId, event.rows, event.page, "Project");
    if (result.statusCode == 200) {
      this.noteHistory = result.noteEntityList;
      this.totalRecordsNote = result.totalRecordsNote
      await this.handleNoteContent();
    }
  }

  onChangeProject(projectId: string) {
    this.router.navigate(['/project/detail', { 'projectId': projectId }]);
  }
}
function convertToUTCTime(time: any) {
  if (time)
    return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
  else
    return null;
};

function convertToPlain(html) {

  // Create a new div element
  var tempDivElement = document.createElement("div");

  // Set the HTML content with the given value
  tempDivElement.innerHTML = html;

  // Retrieve the text property of the element
  return tempDivElement.textContent || tempDivElement.innerText || "";
}
