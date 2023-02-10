import { Component, OnInit, ElementRef, HostListener, ViewChild, Renderer2 } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import * as $ from 'jquery';
import { Location } from '@angular/common';
import { NoteModel } from '../../../shared/models/note.model';

import { MessageService, ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { NoteService } from '../../../shared/services/note.service';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission'
import { FileUpload, Table, TTSortIcon } from 'primeng';
import { TimeSheetDialogComponent } from '../time-sheet-dialog/time-sheet-dialog.component';
import { ProjectService } from '../../services/project.service';
import { ProjectModel } from '../../models/project.model';
import { TaskProjectScopeDialogComponent } from '../task-project-scope-dialog/task-project-scope-dialog.component';
import { ProjectScopeModel } from '../../models/ProjectScope.model';
import { ProjectResourceModel } from '../../models/ProjectResource.model';
import { TaskDocument, TaskModel } from '../../models/ProjectTask.model';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { TaskContraint } from '../../models/TaskConstraint.model';
import { AddConstaintDialogComponent } from '../add-constaint-dialog/add-constaint-dialog.component';
import { SetIconTaskType } from '../../setIcon/setIcon';
import { AddRelateDialogComponent } from '../add-relate-dialog/add-relate-dialog.component';
import { TaskRelate } from '../../models/TaskRelate.model';

class Project {
  projectId: string;
  projectCode: string;
  projectName: string;
  projectCodeName: string;
}

class Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  icon: string;
}

class PhanTram {
  label: string;
  value: number;
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

@Component({
  selector: 'app-create-project-task',
  templateUrl: './create-project-task.component.html',
  styleUrls: ['./create-project-task.component.css']
})
export class CreateProjectTaskComponent implements OnInit {
  @ViewChild('myTable') myTable: Table;
  @ViewChild('tableBefore') tableBefore: Table;
  @ViewChild('tableAfter') tableAfter: Table;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink', '|', 'ClearFormat', '|', 'Undo', 'Redo']
  };

  listProjectTitle: Array<Project> = [];
  selectedProject: Project = null;

  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  auth = JSON.parse(localStorage.getItem("auth"));
  defaultNumberType = this.systemParameterList.find((systemParameter: { systemKey: string; }) => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;

  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  minDateProject: Date = null;
  maxDateProject: Date = null;

  //START: BIẾN ĐIỀU KIỆN
  actionAdd: boolean = true;
  fixed: boolean = false;
  withFiexd: string = "";
  isOpenNotifiError: boolean = false;
  isInvalidForm: boolean = false;
  //END : BIẾN ĐIỀU KIỆN


  listRelateTaskShow: Array<any> = [];


  selectedObjectType: string = '1';// Mức độ ưu tiên
  selecteTimeSetting: string = 'auto'; // Tủy chỉnh thời gian

  /*START: FORM CONTROL*/
  createProjectTaskFrom: FormGroup;

  taskCodeControl: FormControl;
  taskNameControl: FormControl;
  scopeControl: FormControl; // Hạng mục
  projectNameControl: FormControl;
  descriptionControl: FormControl;

  taskTypeControl: FormControl; // Loại công việc
  statusControl: FormControl; // Trạng thái
  priorityControl: FormControl; // Độ ưu tiên
  milestonesControl: FormControl; // Mốc dự án
  percentControl: FormControl; // Phần trăm hoàn thành

  startTimeControl: FormControl; // Thời gian bắt đầu
  endTimeControl: FormControl; //Thời gian kết thúc
  hourControl: FormControl; // Số giờ dự kiên
  isWeekendControl: FormControl; //Tính cuối tuần

  actualStartTimeControl: FormControl; // Thời gian bắt đầu thực tế
  actualEndTimeControl: FormControl; // Thời gian kết thúc thực tế
  actualHourControl: FormControl; // Số giờ thực tế

  isSendEmailControl: FormControl;
  /*END: FORM CONTROL*/
  listTaskDocument: Array<TaskDocument> = [];
  listTaskDocumentDelete: Array<TaskDocument> = [];

  /*START: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  listTaskType: Array<Category> = [];
  listStatus: Array<Category> = [];
  listMilestones: Array<any> = [];
  listResource: Array<ProjectResourceModel> = [];
  project: ProjectModel = null;
  projectScope: ProjectScopeModel = null;
  /*END: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/

  listTaskContraintBefore: Array<TaskContraint> = [];
  listTaskContraintAfter: Array<TaskContraint> = [];


  listRelateTask: Array<any> = [];
  taskId: string = '';

  /*START: TABLE*/
  colsList: any;
  selectedColumns: any[];
  isGlobalFilter: any = '';
  selectedColumnsRelateTask: any = [];

  colsConstraintBefore: any[];
  selectedColumnsConstraintBefore: any = [];
  isGlobalFilterOne: any = '';

  colsRelateTask: any[];
  colsConstrantAfter: any[];
  selectedColumnsConstrantAfter: any = [];
  isGlobalFilterTwo: any = [];
  /*END: TABLE*/

  /*START: DATA*/
  listPhanTram: Array<PhanTram> = [
    { label: '0%', value: 0 },
    { label: '5%', value: 5 },
    { label: '10%', value: 10 },
    { label: '15%', value: 15 },
    { label: '20%', value: 20 },
    { label: '25%', value: 25 },
    { label: '30%', value: 30 },
    { label: '35%', value: 35 },
    { label: '40%', value: 40 },
    { label: '45%', value: 45 },
    { label: '50%', value: 50 },
    { label: '55%', value: 55 },
    { label: '60%', value: 60 },
    { label: '65%', value: 65 },
    { label: '70%', value: 70 },
    { label: '75%', value: 75 },
    { label: '80%', value: 80 },
    { label: '85%', value: 85 },
    { label: '90%', value: 90 },
    { label: '95%', value: 95 },
    { label: '100%', value: 100 },
  ]
  numerChecker: number = 0;
  numberPic: number = 0;
  /*END: DATA*/

  projectId: string = '';
  milestoneId: string = '';
  emitStatusChangeForm: any;
  projectCodeName: string = '';

  @ViewChild('toggleButton') toggleButton: ElementRef;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private router: Router,
    private noteService: NoteService,
    public builder: FormBuilder,
    private el: ElementRef,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private renderer: Renderer2,
    private projectService: ProjectService,
    private imageService: ImageUploadService,
    private setIconTaskType: SetIconTaskType
  ) {
    this.translate.setDefaultLang('vi');
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
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }

  ngOnInit(): void {
    this.initForm();
    this.initTable();
    this.route.params.subscribe(params => {
      this.projectId = params['projectId'];
      this.milestoneId = params['milestoneId'];
      this.getMasterData();
    });
  }

  initForm() {
    this.taskNameControl = new FormControl('', [Validators.required, forbiddenSpaceText]);
    this.projectNameControl = new FormControl('');
    this.scopeControl = new FormControl(null);
    this.descriptionControl = new FormControl('');

    this.taskTypeControl = new FormControl(null);
    this.statusControl = new FormControl();
    this.priorityControl = new FormControl(null);
    this.milestonesControl = new FormControl(null);
    this.percentControl = new FormControl(null);

    this.startTimeControl = new FormControl(null, [Validators.required]);
    this.endTimeControl = new FormControl(null, [Validators.required]);
    this.hourControl = new FormControl('0', [Validators.required]);
    this.isWeekendControl = new FormControl(false);

    this.actualStartTimeControl = new FormControl(null);
    this.actualEndTimeControl = new FormControl(null);
    this.actualHourControl = new FormControl('0');
    this.isSendEmailControl = new FormControl(false);

    this.createProjectTaskFrom = new FormGroup({
      taskNameControl: this.taskNameControl,
      projectNameControl: this.projectNameControl,
      scopeControl: this.scopeControl,
      descriptionControl: this.descriptionControl,
      taskTypeControl: this.taskTypeControl,
      statusControl: this.statusControl,
      priorityControl: this.priorityControl,
      milestonesControl: this.milestonesControl,
      percentControl: this.percentControl,
      startTimeControl: this.startTimeControl,
      endTimeControl: this.endTimeControl,
      hourControl: this.hourControl,
      isWeekendControl: this.isWeekendControl,
      actualStartTimeControl: this.actualStartTimeControl,
      actualEndTimeControl: this.actualEndTimeControl,
      actualHourControl: this.actualHourControl,
      isSendEmailControl: this.isSendEmailControl
    });
  }

  initTable() {
    this.colsList = [
      { field: 'nameResource', header: 'Người tham gia', textAlign: 'left', display: 'table-cell' },
      { field: 'employeeRoleName', header: 'Vai trò', textAlign: 'left', display: 'table-cell' },
    ];
    this.selectedColumns = this.colsList;

    this.colsConstraintBefore = [
      { field: 'taskCode', header: 'Mã công việc', textAlign: 'left', display: 'table-cell' },
      { field: 'taskName', header: 'Tên công việc', textAlign: 'left', display: 'table-cell' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'left', display: 'table-cell' },
      { field: 'planStartTime', header: 'Ngày bắt đầu dự kiến', textAlign: 'left', display: 'table-cell' },
      { field: 'planEndTime', header: 'Ngày kết thúc dự kiến', textAlign: 'left', display: 'table-cell' },
      { field: 'delayTime', header: 'Độ trễ', textAlign: 'left', display: 'table-cell' },
    ];
    this.selectedColumnsConstraintBefore = this.colsConstraintBefore;

    this.colsConstrantAfter = [
      { field: 'taskCode', header: 'Mã công việc', textAlign: 'left', display: 'table-cell' },
      { field: 'taskName', header: 'Tên công việc', textAlign: 'left', display: 'table-cell' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'left', display: 'table-cell' },
      { field: 'planStartTime', header: 'Ngày bắt đầu dự kiến', textAlign: 'left', display: 'table-cell' },
      { field: 'planEndTime', header: 'Ngày kết thúc dự kiến', textAlign: 'left', display: 'table-cell' },
      { field: 'delayTime', header: 'Độ trễ', textAlign: 'left', display: 'table-cell' },
    ];
    this.selectedColumnsConstrantAfter = this.colsConstrantAfter;

    this.colsRelateTask = [
      { field: 'taskCode', header: 'Mã công việc', textAlign: 'left', display: 'table-cell', width: '120px' },
      { field: 'taskName', header: 'Tên công việc', textAlign: 'left', display: 'table-cell', width: '200px' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '120px' },
      { field: 'expectedEndDate', header: 'Ngày bắt đầu dự kiến', textAlign: 'right', display: 'table-cell', width: '180px', },
      { field: 'expectedStartDate', header: 'Ngày kết thúc dự kiến', textAlign: 'right', display: 'table-cell', width: '180px', },
    ];
    this.selectedColumnsRelateTask = this.colsRelateTask;
  }

  getMasterData() {
    this.loading = true;
    this.projectService.getMasterDataCreateOrUpdateTask(this.projectId, null, this.auth.UserId).subscribe(response => {
      let result: any = response;
      if (result.statusCode === 200) {
        this.listProjectTitle = result.listProject;
        this.listProjectTitle.forEach(item => {
          item.projectCodeName = `[${item.projectCode}]: ${item.projectName}`;
        });
        this.selectedProject = this.listProjectTitle.find(c => c.projectId == this.projectId);
        this.listStatus = result.listStatus;
        this.listTaskType = this.setIconTaskType.setIconTaskType(result.listTaskType);
        this.listMilestones = result.listMilestone;
        this.listResource = result.listProjectResource;
        this.project = result.project;
        this.listResource.forEach(item => {
          item.startTime = new Date(item.startTime);
          item.endTime = new Date(item.endTime);
        });
        this.loading = false;
        this.setDefaultValue();
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });

    this.projectService.GetMasterDataCreateRelateTask(this.taskId, this.projectId, this.auth.UserId).subscribe(response => {
      let result1: any = response;
      this.loading = false;
      if (result1.statusCode == 200) {
        this.listRelateTaskShow = result1.listRelateTask;

      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result1.messageCode);
      }
    });

  }

  setDefaultValue() {
    let newStatus = this.listStatus.find(c => c.categoryCode == "NEW");
    this.statusControl.setValue(newStatus);
    this.hourControl.setValue('0');
    this.actualHourControl.setValue('0');
    let zezoPhanTram = this.listPhanTram[0];
    this.percentControl.setValue(zezoPhanTram);
    if (this.project) {
      this.minDateProject = this.project.projectStartDate ? new Date(this.project.projectStartDate) : null;
      this.maxDateProject = this.project.projectEndDate ? new Date(this.project.projectEndDate) : null;
      let name = `${this.project.projectCode} - ${this.project.projectName}`;
      this.projectCodeName = `${this.project.projectCode} - ${this.project.projectName}`;
      this.projectNameControl.setValue(name);
    }
    if (this.milestoneId) {
      let milestone = this.listMilestones.find(x => x.projectMilestonesId == this.milestoneId);
      if (milestone) this.milestonesControl.setValue(milestone);
    }
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView(true);
  }

  cancel() {
    this.confirmationService.confirm({
      message: `Các thay đổi sẽ không được lưu lại. Hành động này không thể được hoàn tác, bạn có chắc chắn muốn huỷ?`,
      accept: () => {
        this.router.navigate(['/project/list-project-task', { 'projectId': this.projectId }]);
      }
    });
  }

  createProjectTask(type: boolean) {

    //type = true: luu va tao moi
    //type = false: luu
    if (!this.createProjectTaskFrom.valid) {
      Object.keys(this.createProjectTaskFrom.controls).forEach(key => {
        if (!this.createProjectTaskFrom.controls[key].valid) {
          this.createProjectTaskFrom.controls[key].markAsTouched();
        }
      });
      this.emitStatusChangeForm = this.createProjectTaskFrom.statusChanges.subscribe((validity: string) => {
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
      this.isInvalidForm = true;
      this.isOpenNotifiError = true;
    } else {
      // if (this.checkPicTask()) {
      //   this.clearToast();
      //   this.showToast('error', 'Thông báo', 'Công việc chưa có người phụ trách!');
      // } else {
      let hour = this.hourControl.value ? ParseStringToFloat(this.hourControl.value) : 0;
      if (hour <= 0) {
        this.clearToast();
        this.showToast('error', 'Thông báo', 'Số giờ dự kiến thực hiện cần lớn hơn 0');
        return;
      }
      this.isInvalidForm = false;
      this.isOpenNotifiError = false;
      let folderType = `${this.project.projectCode}_TASK_FILE`

      let task = this.mappingDateFromFormToModel();

      let listResource = this.listResource.filter(c => c.isPic == true || c.isCheck == true);

      this.loading = true;

      let listFileUploadModel: Array<FileUploadModel> = [];
      this.uploadedFiles.forEach(item => {
        let fileUpload: FileUploadModel = new FileUploadModel();
        fileUpload.FileInFolder = new FileInFolder();
        fileUpload.FileInFolder.active = true;
        let index = item.name.lastIndexOf(".");
        let name = item.name.substring(0, index);
        fileUpload.FileInFolder.fileName = name;
        fileUpload.FileInFolder.fileExtension = item.name.substring(index + 1);
        fileUpload.FileInFolder.size = item.size;
        fileUpload.FileInFolder.objectType = 'TASK';
        fileUpload.FileSave = item;
        listFileUploadModel.push(fileUpload);
      });

      // if (this.uploadedFiles.length > 0) {
      //   this.uploadFiles(this.uploadedFiles);
      //   this.converToListTaskDocument(this.uploadedFiles);
      // }
      this.projectService.createOrUpdateTask(task, folderType, listResource, listFileUploadModel, this.listTaskDocumentDelete, this.auth.UserId, this.listRelateTask).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          if (type === false) {
            this.showToast('success', 'Thông báo', 'Tạo công việc thành công');
            setTimeout(() => {
              this.router.navigate(['/project/detail-project-task', { 'taskId': result.taskId, 'projectId': this.projectId }]);
            }, 2000);
          } else {
            if (this.emitStatusChangeForm) {
              this.emitStatusChangeForm.unsubscribe();
              this.isInvalidForm = false; //Ẩn icon-warning-active
            }
            this.clearAllFile();
            this.fileUpload.clear();
            this.resetForm();
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Tạo công việc thành công');
            this.awaitResult = false;
          }
        } else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      });
      // }
    }
  }


  AddRelateTask(data){
    this.listRelateTask = [];
    if(data.value){
      data.value.forEach(element => {
        let relateTask = new TaskRelate();
        relateTask.relateTaskId = element.taskId;
        relateTask.ProjectId = this.projectId;
        this.listRelateTask.push(relateTask);
      });
    }
  }
  mappingDateFromFormToModel(): TaskModel {
    let task = new TaskModel();
    task.taskName = this.taskNameControl.value.trim();
    task.projectId = this.projectId;
    task.projectScopeId = this.projectScope ? this.projectScope.projectScopeId : null;
    task.description = this.descriptionControl.value ? this.descriptionControl.value.trim() : null;
    task.planStartTime = this.startTimeControl.value ? convertToUTCTime(this.startTimeControl.value) : null;
    task.planEndTime = this.endTimeControl.value ? convertToUTCTime(this.endTimeControl.value) : null;
    task.estimateHour = this.hourControl.value ? ParseStringToFloat(this.hourControl.value) : 0;
    task.actualStartTime = this.actualStartTimeControl.value ? convertToUTCTime(this.actualStartTimeControl.value) : null;
    task.actualEndTime = this.actualEndTimeControl.value ? convertToUTCTime(this.actualEndTimeControl.value) : null;
    task.actualHour = this.actualHourControl.value ? ParseStringToFloat(this.actualHourControl.value) : 0;
    task.priority = this.selectedObjectType ? ParseStringToFloat(this.selectedObjectType) : 1;
    task.milestonesId = this.milestonesControl.value ? this.milestonesControl.value.projectMilestonesId : null;
    task.status = this.statusControl.value ? this.statusControl.value.categoryId : null;
    task.includeWeekend = this.isWeekendControl.value;
    task.isSendMail = this.isSendEmailControl.value;
    task.taskComplate = this.percentControl.value ? this.percentControl.value.value : null;
    task.taskTypeId = this.taskTypeControl.value ? this.taskTypeControl.value.categoryId : null;
    task.timeType = this.selecteTimeSetting;

    return task;
  }

  resetForm() {
    this.createProjectTaskFrom.reset();
    this.listTaskDocument = [];
    this.setDefaultValue();
    this.selecteTimeSetting = "auto";
    this.listResource.forEach(item => {
      item.isCheck = false;
      item.isPic = false;
      item.hours = 0;
    });
    this.numberPic = 0;
    this.numerChecker = 0;
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  /*Event Lưu các file được chọn*/
  handleFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= this.defaultLimitedFileSize) {
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

  openAddTimeSheetDialog() {
    let width = '';
    if (window.innerWidth < 500) {
      width = '100%';
    } else {
      width = '50%';
    }

    let ref = this.dialogService.open(TimeSheetDialogComponent, {
      data: {
        projectId: this.projectId,
      },
      header: 'Thêm chi tiết thời gian',
      width: width,
      baseZIndex: 1030,
      closeOnEscape: true,
      closable: true,
      contentStyle: {
        "min-height": "300px",
        "max-height": "700px",
        "overflow": "auto !important",
      }
    });

    ref.onClose.subscribe((result: any) => {
    });
  }

  openProjetScopeDialog() {
    let width = '';
    if (window.innerWidth < 500) {
      width = '100%';
    } else {
      width = '40%';
    }

    let ref = this.dialogService.open(TaskProjectScopeDialogComponent, {
      data: {
        projectId: this.projectId
      },
      header: 'Hạng mục dự án',
      width: width,
      baseZIndex: 1030,
      closeOnEscape: true,
      closable: true,
      contentStyle: {
        "min-height": "300px",
        "max-height": "700px",
        "overflow-y": "auto"
      }
    });

    ref.onClose.subscribe((result: ProjectScopeModel) => {
      this.projectScope = result;
      this.scopeControl.setValue(this.projectScope.projectScopeName);
    });
  }

  openCreateContrantDialog() {
    let width = '';
    if (window.innerWidth < 500) {
      width = '100%';
    } else {
      width = '45%';
    }
    let ref = this.dialogService.open(AddConstaintDialogComponent, {
      data: {
        projectId: this.projectId,
        taskId: null,
        isCreate: true,
      },
      header: 'Thêm ràng buộc',
      width: width,
      baseZIndex: 1030,
      closeOnEscape: true,
      closable: true,
      contentStyle: {
        "min-height": "300px",
        "max-height": "700px",
        "overflow-y": "none"
      }
    });

    ref.onClose.subscribe((result: TaskContraint) => {

    });
  }

  // Tính toán lại thời gian của nguồn lực
  thayDoiThoiGianThucHien() {
    if (this.selecteTimeSetting === "auto") {
      let timeAll = ParseStringToFloat(this.hourControl.value ?? "0");
      let number = this.listResource.filter(c => c.isPic == true).length;
      if (number != 0) {
        var timeOfOneResource = timeAll / number;
        this.listResource.forEach(item => {
          if (item.isPic == true) {
            item.hours = timeOfOneResource;
          }
        });
      }
    }
  }

  clickAutoTime() {
    let timeAll = this.hourControl.value ?? 0;
    let number = this.listResource.filter(c => c.isPic == true).length;
    if (number != 0) {
      var timeOfOneResource = timeAll / number;
      this.listResource.forEach(item => {
        if (item.isPic == true) {
          item.hours = timeOfOneResource;
        }
      });
    }
  }

  clickHandTime() {
    this.listResource.forEach(item => {
      item.hours = 0;
    });
  }

  changePic() {
    if (this.selecteTimeSetting == "auto") {
      let timeAll = this.hourControl.value ?? 0;
      this.numberPic = this.listResource.filter(c => c.isPic == true).length;
      var timeOfOneResource = timeAll / this.numberPic;
      if (this.numberPic != 0) {
        this.listResource.forEach(item => {
          item.hours = 0;
          if (item.isPic == true) {
            item.hours = timeOfOneResource;
          }
        });
      } else {
        this.listResource.forEach(item => {
          item.hours = 0;
        });
      }
    }
  }

  changeChecker() {
    this.numerChecker = this.listResource.filter(c => c.isCheck == true).length;
  }

  async uploadFiles(files: File[]) {
    await this.projectService.uploadFileForOptionAsync(files, 'Task', this.projectCodeName);
  }

  converToListTaskDocument(fileList: File[]) {
    for (var x = 0; x < fileList.length; ++x) {
      let taskDocumentModel = new TaskDocument();
      taskDocumentModel.documentName = fileList[x].name;
      taskDocumentModel.documentSize = fileList[x].size + '';
      taskDocumentModel.createdDate = convertToUTCTime(new Date());
      taskDocumentModel.updatedDate = convertToUTCTime(new Date());
      this.listTaskDocument.push(taskDocumentModel);
    }
  }
  checkPicTask(): boolean {
    let list = this.listResource.filter(c => c.isPic == true);
    return list.length == 0;
  }


  onChangeProject(projectId: string) {
    this.router.navigate(['/project/create-project-task', { 'projectId': projectId }]);
  }


  openCreateRelateTaskDialog() {
    let width = '';
    if (window.innerWidth < 500) {
      width = '100%';
    } else {
      width = '45%';
    }
    let ref = this.dialogService.open(AddRelateDialogComponent, {
      data: {
        projectId: this.projectId,
        taskId: null,
        isCreate: true,
      },
      header: 'Thêm công việc liên quan',
      width: width,
      baseZIndex: 1030,
      closeOnEscape: true,
      closable: true,
      contentStyle: {
        "min-height": "300px",
        "max-height": "700px",
        "overflow-y": "none"
      }
    });

    ref.onClose.subscribe((result: TaskRelate) => {
      let checkExist = this.listRelateTask.filter(c => c.RelateTaskId == result.relateTaskId);
      if(checkExist.length == 0){
        this.listRelateTask.push(result);
      }else{
        this.showToast('error', 'Thông báo', 'Công việc liên quan đã tồn tại trong danh sách');
      }
    });
  }


  removeRelateTask(data){
    this.listRelateTask =  this.listRelateTask.filter(function (item) {
      return item.TaskId >= data.taskId;
    });
  }

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
      }
    }
  }
  return null;
}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
