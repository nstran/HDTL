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
import { FileUpload, Table } from 'primeng';
import { TimeSheetDialogComponent } from '../time-sheet-dialog/time-sheet-dialog.component';
import { ProjectService } from '../../services/project.service';
import { ProjectModel } from '../../models/project.model';
import { TaskProjectScopeDialogComponent } from '../task-project-scope-dialog/task-project-scope-dialog.component';
import { ProjectScopeModel } from '../../models/ProjectScope.model';
import { ProjectResourceModel } from '../../models/ProjectResource.model';
import { ProjectTaskModel, TaskDocument, TaskModel } from '../../models/ProjectTask.model';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { TaskContraint } from '../../models/TaskConstraint.model';
import { AddConstaintDialogComponent } from '../add-constaint-dialog/add-constaint-dialog.component';
import { SetIconTaskType } from '../../setIcon/setIcon';
import { Paginator } from 'primeng/paginator';
import { TaskRelate } from '../../models/TaskRelate.model';
import { AddRelateDialogComponent } from '../add-relate-dialog/add-relate-dialog.component';
class Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}

class PhanTram {
  label: string;
  value: number;
}

class Note {
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
  constructor() {
    this.noteDocList = [];
  }
}

class NoteDocument {
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
  selector: 'app-detail-project-task',
  templateUrl: './detail-project-task.component.html',
  styleUrls: ['./detail-project-task.component.css']
})
export class DetailProjectTaskComponent implements OnInit {
  @ViewChild('myTable') myTable: Table;
  @ViewChild('dt') dt: Table;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  @ViewChild('fileUploadNote') fileUploadNote: FileUpload;
  @ViewChild('tableBefore') tableBefore: Table;
  @ViewChild('tableAfter') tableAfter: Table;
  @ViewChild('tableRelateTask') tableRelateTask: Table;

  tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink', '|', 'ClearFormat', '|', 'Undo', 'Redo']
  };

  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  auth = JSON.parse(localStorage.getItem("auth"));
  defaultNumberType = this.systemParameterList.find((systemParameter: { systemKey: string; }) => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;

  minDateProject: Date = null;
  maxDateProject: Date = null;

  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  listNoteDocumentModel: Array<NoteDocumentModel> = [];

  //START: BIẾN ĐIỀU KIỆN
  actionAdd: boolean = true;
  fixed: boolean = false;
  withFiexd: string = "";
  isOpenNotifiError: boolean = false;
  isInvalidForm: boolean = false;
  //END : BIẾN ĐIỀU KIỆN

  selectedObjectType: string = '1';// Mức độ ưu tiên
  selecteTimeSetting: string = 'auto'; // Tủy chỉnh thời gian

  /*START: FORM CONTROL*/
  updateProjectTaskFrom: FormGroup;

  taskCodeControl: FormControl;
  taskNameControl: FormControl;
  scopeControl: FormControl; // Hạng mục
  projectNameControl: FormControl;
  descriptionControl: FormControl;
  listRelateTaskChose: FormControl; // công việc liên quan
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

  soLanMoLaiControl: FormControl; // Số lần mở lại

  isSendEmailControl: FormControl;

  listRelateTask: Array<TaskRelate> = []; //Công việc lquan

  listRelateTaskShow: Array<any> = [];

  listRelateTaskUpdate: Array<TaskRelate> = [];

  /*END: FORM CONTROL*/

  /*START: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  listTaskType: Array<Category> = [];
  listStatus: Array<Category> = [];
  listMilestones: Array<any> = [];
  listResource: Array<ProjectResourceModel> = [];
  project: ProjectModel = null;
  projectScope: ProjectScopeModel = null;
  task: TaskModel;
  noteHistory: Array<Note> = [];
  status: Category;

  listTaskContraintBefore: Array<TaskContraint> = [];
  listTaskContraintAfter: Array<TaskContraint> = [];

  /*END: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/

  /*NOTE*/
  noteContent: string = '';
  noteId: string;
  listUpdateNoteDocument: Array<NoteDocument> = [];
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  /*END*/

  /*START: TABLE*/
  colsList: any;
  selectedColumns: any[];

  colsFile: any[];
  isGlobalFilter: any = '';
  colsConstraintBefore: any[];


  selectedColumnsRelateTask: any = [];
  selectedColumnsConstraintBefore: any = [];
  isGlobalFilterOne: any = '';

  colsRelateTask: any[];
  colsConstrantAfter: any[];
  selectedColumnsConstrantAfter: any = [];
  isGlobalFilterTwo: any = [];
  /*END: TABLE*/

  listTaskDocument: Array<TaskDocument> = [];
  listTaskDocumentDelete: Array<TaskDocument> = [];
  actionDelete: boolean = true;
  actionEdit: boolean = true;
  actionEditNote: boolean = true;
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

  projectId: string = this.emptyGuid;
  taskId: string = '';
  projectCodeName: string = '';
  totalRecordsNote: number = 0;
  emitStatusChangeForm: any;
  statusCode: string = '';
  isManager: boolean = false;
  isPresident: boolean = false;
  isDisable: boolean = false;
  isCreateTask: boolean = true;

  @ViewChild('toggleButton') toggleButton: ElementRef;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('paginator', { static: true }) paginator: Paginator
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
    public location: Location,
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

  async ngOnInit() {
    this.initForm();
    this.initTable();
    this.route.params.subscribe(async params => {
      let resource = "pro/project/detail";
      this.taskId = params['taskId'];
      this.projectId = params['projectId'];
      let permission: any = await this.getPermission.getPermission(resource);
      if (permission.status == false) {
        this.router.navigate(['/home']);
      }
      else {

        let listCurrentActionResource = permission.listCurrentActionResource;

        if (listCurrentActionResource.indexOf("editnote") == -1) {
          this.actionEditNote = false;
        }
        this.getMasterData();
      }

    });
  }

  initForm() {
    this.taskNameControl = new FormControl('', [Validators.required, forbiddenSpaceText]);
    this.projectNameControl = new FormControl('');
    this.scopeControl = new FormControl(null);
    this.descriptionControl = new FormControl('');

    this.listRelateTaskChose = new FormControl(null);
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
    this.actualHourControl = new FormControl([{ value: '0', disabled: true }]);
    this.isSendEmailControl = new FormControl(false);
    this.soLanMoLaiControl = new FormControl(null);

    this.updateProjectTaskFrom = new FormGroup({
      taskNameControl: this.taskNameControl,
      projectNameControl: this.projectNameControl,
      scopeControl: this.scopeControl,
      descriptionControl: this.descriptionControl,
      listRelateTaskChose: this.listRelateTaskChose,
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
      isSendEmailControl: this.isSendEmailControl,
      soLanMoLaiControl: this.soLanMoLaiControl
    });
  }

  initTable() {
    this.colsList = [
      { field: 'nameResource', header: 'Người tham gia', textAlign: 'left', display: 'table-cell' },
      { field: 'employeeRoleName', header: 'Vai trò', textAlign: 'left', display: 'table-cell' },
    ];
    this.selectedColumns = this.colsList;

    this.colsFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'documentSize', header: 'Kích thước', width: '50%', textAlign: 'left', type: 'number' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'left', type: 'date' },
      { field: 'createByName', header: 'Người Upload', width: '50%', textAlign: 'left', type: 'string' },
    ];

    this.colsConstraintBefore = [
      { field: 'taskCode', header: 'Mã công việc', textAlign: 'left', display: 'table-cell', width: '120px', },
      { field: 'taskName', header: 'Tên công việc', textAlign: 'left', display: 'table-cell', width: '180px', },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '120px', },
      { field: 'planStartTime', header: 'Ngày bắt đầu dự kiến', textAlign: 'right', display: 'table-cell', width: '180px', },
      { field: 'planEndTime', header: 'Ngày kết thúc dự kiến', textAlign: 'right', display: 'table-cell', width: '180px', },
      { field: 'delayTime', header: 'Độ trễ', textAlign: 'right', display: 'table-cell', width: '120px', },
    ];
    this.selectedColumnsConstraintBefore = this.colsConstraintBefore;

    this.colsConstrantAfter = [
      { field: 'taskCode', header: 'Mã công việc', textAlign: 'left', display: 'table-cell', width: '120px' },
      { field: 'taskName', header: 'Tên công việc', textAlign: 'left', display: 'table-cell', width: '200px' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '120px' },
      { field: 'planStartTime', header: 'Ngày bắt đầu dự kiến', textAlign: 'right', display: 'table-cell', width: '180px', },
      { field: 'planEndTime', header: 'Ngày kết thúc dự kiến', textAlign: 'right', display: 'table-cell', width: '180px', },
      { field: 'delayTime', header: 'Độ trễ', textAlign: 'right', display: 'table-cell', width: '120px', },
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
    this.projectService.getMasterDataCreateOrUpdateTask(this.projectId, this.taskId, this.auth.UserId).subscribe(response => {
      let result: any = response;
      if (result.statusCode === 200) {
        this.listTaskType = this.setIconTaskType.setIconTaskType(result.listTaskType);
        this.listMilestones = result.listMilestone;
        this.listTaskDocument = result.listTaskDocument;
        this.listResource = result.listProjectResource;
        this.project = result.project;
        this.projectCodeName = `${this.project.projectCode} - ${this.project.projectName}`;

        //Công việc liên quan con
        this.listRelateTask = result.listRelateTask;

        //Công việc liên quan parent
        let parentTaskRelate = result.listRelateTaskParent;


        this.listTaskDocument.forEach(item => {
          // item.documentExtension = item.documentName.substring(item.documentName.lastIndexOf('.') + 1);
          item.documentName = item.documentName.substring(0, item.documentName.lastIndexOf('_')).toLocaleLowerCase();
        })

        this.task = result.task;

        this.projectScope = result.scope;
        this.projectScope ? this.scopeControl.setValue(this.projectScope.projectScopeName) : '';
        if (this.project) {
          this.minDateProject = this.project.projectStartDate ? new Date(this.project.projectStartDate) : null;
          this.maxDateProject = this.project.projectEndDate ? new Date(this.project.projectEndDate) : null;
          let name = `${this.project.projectCode} - ${this.project.projectName}`;
          this.projectNameControl.setValue(name);
        }
        this.listTaskContraintBefore = result.listTaskConstraintBefore;
        this.listTaskContraintAfter = result.listTaskConstraintAfter;

        this.isManager = result.isManager;
        this.isPresident = result.isPresident;
        this.setDataFollowStatus(result.task, result.listStatus);
        this.projectNameControl.disable();
        this.scopeControl.disable();

        this.mappingDataToForm(result.task);
        this.loading = false;

        this.totalRecordsNote = result.totalRecordsNote;
        this.noteHistory = result.listNote;
        this.handleNoteContent();

        this.projectService.GetMasterDataCreateRelateTask(this.taskId, this.projectId, this.auth.UserId).subscribe(response => {
          let result1: any = response;
          this.loading = false;
          if (result1.statusCode == 200) {


            this.listRelateTaskShow = result1.listRelateTask;
            var setValueRelateTask = [];
            parentTaskRelate.forEach(item => {
              let task = this.listRelateTaskShow.find(x => x.taskId == item.relateTaskId);
              setValueRelateTask.push(task);
            });
            this.listRelateTaskChose.setValue(setValueRelateTask);


          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result1.messageCode);
          }
        });


      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });



  }

  AddRelateTask(data) {
    this.listRelateTaskUpdate = [];
    if (data.value) {
      data.value.forEach(element => {
        let relateTask = new TaskRelate();
        relateTask.relateTaskId = element.taskId;
        relateTask.ProjectId = this.projectId;
        this.listRelateTaskUpdate.push(relateTask);
      });
    }
  }

  setDataFollowStatus(task: TaskModel, listStatus: Array<Category>) {
    this.status = listStatus.find(c => c.categoryId == task.status);
    switch (this.status.categoryCode) {
      case "NEW":
        this.listStatus = listStatus;
        this.actualHourControl.disable();
        // check nếu không phải là người tạo thì disable hết
        if (task.createBy != this.auth.UserId && !this.isManager) {
          // this.isDisable = true;
          this.isCreateTask = false;
          this.disableControl();
          this.scopeControl.disable();
          this.taskNameControl.disable();
          this.projectNameControl.disable();
          this.isWeekendControl.disable();
          this.startTimeControl.enable();
          this.endTimeControl.enable();
          this.hourControl.enable();
          this.isWeekendControl.enable();

          // this.descriptionControl.disable();
        }
        break;
      case "DL":
        this.listStatus = listStatus.filter(c => c.categoryCode != "NEW");

        if (!this.isPresident) {
          this.updateProjectTaskFrom.enable();
          this.taskNameControl.disable();
          this.projectNameControl.disable();
          this.disableControl();
          this.actualStartTimeControl.disable();
          this.isDisable = true;
          if (!this.isManager) {
            this.scopeControl.disable();
            this.descriptionControl.enable();
            this.startTimeControl.disable();
            this.endTimeControl.disable();
            this.hourControl.disable();
            this.isWeekendControl.disable();
            this.milestonesControl.disable();
          }
          else {
            this.startTimeControl.enable();
            this.endTimeControl.enable();
            this.hourControl.enable();
            this.isWeekendControl.enable();
          }
        }
        break;
      case "HT":
        this.listStatus = listStatus.filter(c => c.categoryCode != "NEW");
        this.updateProjectTaskFrom.enable();
        this.taskNameControl.disable();
        this.isDisable = true;
        this.actualStartTimeControl.disable();
        this.actualEndTimeControl.disable();
        this.milestonesControl.disable();
        this.isWeekendControl.disable();

        this.disableControl();
        break;
      case "CLOSE":
        this.listStatus = listStatus.filter(c => c.categoryCode == "DL" || c.categoryCode == "CLOSE");
        this.updateProjectTaskFrom.disable();
        this.statusControl.enable();
        this.descriptionControl.enable();
        this.isDisable = true;
        break;
    }
  }

  disableControl() {
    this.startTimeControl.disable();
    this.endTimeControl.disable();
    this.hourControl.disable();
    this.actualHourControl.disable();
  }

  mappingDataToForm(task: TaskModel) {
    this.taskNameControl.setValue(task.taskName);
    this.projectNameControl.setValue(this.project.projectName);
    this.descriptionControl.setValue(task.description);

    let taskType = this.listTaskType.find(c => c.categoryId == task.taskTypeId);
    this.taskTypeControl.setValue(taskType);

    let status = this.listStatus.find(c => c.categoryId == task.status);
    this.statusControl.setValue(status);

    let milestone = this.listMilestones.find(c => c.projectMilestonesId == task.milestonesId);
    this.milestonesControl.setValue(milestone);
    this.selectedObjectType = task.priority.toString();
    let percent = this.listPhanTram.find(c => c.value == task.taskComplate);
    this.percentControl.setValue(percent);
    task.planStartTime ? this.startTimeControl.setValue(new Date(task.planStartTime)) : null;
    task.planEndTime ? this.endTimeControl.setValue(new Date(task.planEndTime)) : null;
    this.hourControl.setValue(task.estimateHour);

    task.actualStartTime ? this.actualStartTimeControl.setValue(new Date(task.actualStartTime)) : null;
    task.actualEndTime ? this.actualEndTimeControl.setValue(new Date(task.actualEndTime)) : null;
    this.actualHourControl.setValue(task.actualHour);
    this.soLanMoLaiControl.setValue(task.soLanMoLai);
    this.soLanMoLaiControl.disable();



    this.isWeekendControl.setValue(task.includeWeekend);
    this.isSendEmailControl.setValue(task.isSendMail);
    this.selecteTimeSetting = task.timeType;
    this.listResource.forEach(item => {
      let resouseMapping = task.listTaskResource.find(c => c.resourceId == item.projectResourceId);
      if (resouseMapping) {
        item.hours = resouseMapping.hours;
        item.isPic = resouseMapping.isPersonInCharge;
        item.isCheck = resouseMapping.isChecker;
      }
      item.startTime = new Date(item.startTime);
      item.endTime = new Date(item.endTime);
    });
    this.numberPic = this.listResource.filter(c => c.isPic == true).length;
    this.numerChecker = this.listResource.filter(c => c.isCheck == true).length;

  }

  setDefaultValue() {
    let newStatus = this.listStatus.find(c => c.categoryCode == "NEW");
    this.statusControl.setValue(newStatus);
    let zezoPhanTram = this.listPhanTram[0];
    this.percentControl.setValue(zezoPhanTram);
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
        this.location.back();
      }
    });
  }

  async updateProjectTask(type: boolean) {
    if (!this.updateProjectTaskFrom.valid) {
      Object.keys(this.updateProjectTaskFrom.controls).forEach(key => {
        if (!this.updateProjectTaskFrom.controls[key].valid) {
          this.updateProjectTaskFrom.controls[key].markAsTouched();
        }
      });
      this.emitStatusChangeForm = this.updateProjectTaskFrom.statusChanges.subscribe((validity: string) => {
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
      if (this.status.categoryCode == "CLOSE" && this.statusControl.value.categoryCode == "DL" &&
        this.actualEndTimeControl.value && CompareDateNow(this.actualEndTimeControl.value) == true) {
        this.confirmationService.confirm({
          message: 'Dự án / công việc này thực tế đã kết thúc, nếu mở lại thông tin kết thúc thực tế sẽ bị xóa',
          accept: () => {
            this.saveTask();
          },
        });
      } else {
        this.saveTask();
      }
      // }
    }
  }

  saveTask() {
    this.isInvalidForm = false;
    this.isOpenNotifiError = false;
    let task = this.mappingDateFromFormToModel();
    let listResource = this.listResource.filter(c => c.isPic == true || c.isCheck == true);
    this.loading = true;
    let folderType = `${this.project.projectCode}_TASK_FILE`;

    // if (this.uploadedFiles.length > 0) {
    //   this.uploadFiles(this.uploadedFiles);
    //   this.converToListTaskDocument(this.uploadedFiles);
    // }

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

    this.projectService.createOrUpdateTask(task, folderType, listResource, listFileUploadModel, this.listTaskDocumentDelete, this.auth.UserId, this.listRelateTaskUpdate).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.clearToast();
        this.showToast('success', 'Thông báo', 'Chỉnh sửa công việc thành công');
        this.clearAllFile();
        this.fileUpload.clear();
        this.resetForm();
        this.getMasterData();
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });

  }

  mappingDateFromFormToModel(): TaskModel {

    let task = new TaskModel();
    task.taskId = this.taskId;
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
    this.updateProjectTaskFrom.reset();
    this.hourControl.setValue('0')
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
      width = '60%';
    }

    let ref = this.dialogService.open(TimeSheetDialogComponent, {
      data: {
        projectId: this.projectId,
        taskId: this.taskId
      },
      styleClass: 'timesheet-dialog',
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
      if (result == true) {
        this.getMasterData();
      }
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

  async uploadFilesAsync(files: File[]) {
    await this.imageService.uploadFileAsync(files);
  }

  /*Event thay đổi nội dung ghi chú*/
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  /*Xử lý và hiển thị lại nội dung ghi chú*/
  async handleNoteContent() {
    this.noteHistory.forEach(element => {
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

  /*Event Sửa ghi chú*/
  onClickEditNote(noteId: string, noteDes: string) {
    this.noteContent = noteDes;
    this.noteId = noteId;
    this.listUpdateNoteDocument = this.noteHistory.find(x => x.noteId == this.noteId).noteDocList;
    this.isEditNote = true;
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
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Xóa ghi chú thành công');
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    });
  }
  /*End*/

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

  /*Hủy sửa ghi chú*/
  cancelNote() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn hủy ghi chú này?',
      accept: () => {
        this.noteId = null;
        this.noteContent = null;
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
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
    /*Upload file mới nếu có*/
    if (this.uploadedFiles.length > 0) {
      await this.uploadFilesAsync(this.uploadedFiles);
      for (var x = 0; x < this.uploadedFiles.length; ++x) {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = this.uploadedFiles[x].name;
        noteDocument.DocumentSize = this.uploadedFiles[x].size.toString();
        this.listNoteDocumentModel.push(noteDocument);
      }
    }
    let noteModel = new NoteModel();
    if (!this.noteId) {
      /*Tạo mới ghi chú*/
      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.taskId;
      noteModel.ObjectType = 'TAS';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/
      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'EDT';
      noteModel.ObjectId = this.taskId;
      noteModel.ObjectType = 'TAS';
      noteModel.NoteTitle = 'đã sửa ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
      //Thêm file cũ đã lưu nếu có
      // this.listUpdateNoteDocument.forEach(item => {
      //   let noteDocument = new NoteDocumentModel();
      //   noteDocument.DocumentName = item.documentName;
      //   noteDocument.DocumentSize = item.documentSize;
      //   this.listNoteDocumentModel.push(noteDocument);
      // });
    }
    this.noteService.createNoteTask(noteModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.noteContent = null;
        this.listUpdateNoteDocument = [];
        this.noteId = null;
        this.isEditNote = false;
        /*Reshow Time Line */
        // this.noteHistory = result.listNote;
        //this.getMasterData();

        //this.fileUploadNote.clear();
        this.paginator.changePage(0);
        this.clearToast();
        this.showToast('success', 'Thông báo', "Lưu ghi chú thành công!");

      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  // async createNoteAfterEdit(type: string, noteTitle: string, noteBody: string) {
  //   let noteModel = new NoteModel();
  //   noteModel.NoteId = this.emptyGuid;
  //   noteModel.Description = noteBody;
  //   noteModel.Type = 'ADD';
  //   noteModel.ObjectId = this.taskId;
  //   noteModel.ObjectType = 'TAS';
  //   noteModel.NoteTitle = noteTitle;
  //   noteModel.Active = true;
  //   noteModel.CreatedById = this.emptyGuid;
  //   noteModel.CreatedDate = new Date();
  //   this.noteService.createNoteTask(noteModel).subscribe(response => {
  //     let result: any = response;
  //     if (result.statusCode == 200) {
  //       this.uploadedFiles = [];
  //       if (this.fileUpload) {
  //         this.fileUpload.clear();  //Xóa toàn bộ file trong control
  //       }
  //       this.noteContent = null;
  //       this.listUpdateNoteDocument = [];
  //       this.noteId = null;
  //       this.isEditNote = false;
  //       /*Reshow Time Line */
  //       this.noteHistory = result.listNote;
  //       this.handleNoteContent();
  //     } else {
  //       this.clearToast();
  //       this.showToast('error', 'Thông báo', result.messageCode);
  //     }
  //   });
  // }

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: TaskDocument) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let fileDelete = this.listTaskDocument.find(x => x.taskDocumentId == file.taskDocumentId);
        this.listTaskDocumentDelete.push(fileDelete);
        let index = this.listTaskDocument.indexOf(file);
        this.listTaskDocument.splice(index, 1);
      }
    });
  }

  /*Event upload list file*/
  myUploader(event: any) {
    if (this.uploadedFiles.length > 0) {
      this.uploadFiles(this.uploadedFiles);
      this.converToListTaskDocument(this.uploadedFiles);

      // Xóa file trong control
      this.uploadedFiles = [];
      if (this.fileUpload) {
        this.fileUpload.clear();
      }
    }

    // this.projectService.UploadQuoteDocument(this.quoteId, this.arrayQuoteDocumentModel).subscribe(response => {
    //   var result = <any>response;
    //   if (result.statusCode == 200) {
    //     let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
    //     this.showMessage(mgs);
    //   } else {
    //     let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
    //     this.showMessage(mgs);
    //   }
    // });
  }

  /*Event khi xóa 1 file trong comment đã lưu trên server*/
  deleteNoteFile(file: NoteDocument) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listUpdateNoteDocument.indexOf(file);
        this.listUpdateNoteDocument.splice(index, 1);
      }
    });
  }

  /*Event khi download 1 file đã lưu trên server*/
  downloadFile(fileInfor: TaskDocument) {

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
          anchor.download = name + result.extension;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    });
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
  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }
  checkPicTask(): boolean {
    let list = this.listResource.filter(c => c.isPic == true);
    return list.length == 0;
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
        taskId: this.taskId,
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
        "overflow": "hidden"
      }
    });

    ref.onClose.subscribe((result: boolean) => {
      if (result) {
        this.clearToast();
        this.showToast('success', 'Thông báo', "Tạo ràng buộc thành công!");
        this.getMasterData();
      }
    });
  }

  changeRequired(rowData: TaskContraint) {
    // this.loading = true;
    this.projectService.updateRequiredConstrant(rowData.taskConstraintId, this.auth.UserId).subscribe(response => {
      let result: any = response;
    });
  }

  deleteConstraint(rowData: TaskContraint) {
    this.confirmationService.confirm({
      message: `Bạn có chắc là muốn xóa ràng buộc?`,
      accept: () => {
        this.loading = true;
        this.projectService.deleteTaskConstraint(rowData.taskConstraintId, this.auth.UserId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.clearToast();
            this.showToast('success', 'Thông báo', "Xóa ràng buộc thành công!");
            this.getMasterData();
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    });
  }

  deleteRelateTask(rowData) {
    this.confirmationService.confirm({
      message: `Bạn có chắc là muốn xóa công việc liên quan?`,
      accept: () => {
        this.loading = true;
        this.projectService.deleteRelateTask(rowData.relateTaskMappingId, this.auth.UserId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.clearToast();
            this.showToast('success', 'Thông báo', "Xóa công việc liên quan thành công!");
            this.getMasterData();
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    });
  }

  //Thêm mới công việc lquan
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
        taskId: this.taskId,
        isCreate: false,
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
      if (result.RelateTaskMappingId != "00000000-0000-0000-0000-000000000000") this.listRelateTask.push(result);
    });
  }

  //xem công việc lquan
  viewJobDetailRelateTask(data) {
    this.router.navigate(['/project/detail-project-task', { taskId: data.taskId, projectId: data.projectId }]);
  }

  async paginate(event) {
    //event.first = Index of the first record
    //event.rows = Number of rows to display in new page
    //event.page = Index of the new page
    //event.pageCount = Total number of pages

    let result: any = await this.projectService.pagingProjectNote(this.taskId, event.rows, event.page, "TAS");
    if (result.statusCode == 200) {
      this.noteHistory = result.noteEntityList;
      this.totalRecordsNote = result.totalRecordsNote
      await this.handleNoteContent();
    }
  }

  viewJobDetail(data) {
  }
}


function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours() - 7, time.getMinutes(), time.getSeconds()));
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

function CompareDateNow(date1: Date) {
  let today = new Date();
  let leftDate = date1.setHours(0, 0, 0, 0);
  let rightDate = today.setHours(0, 0, 0, 0);

  return leftDate <= rightDate;
}
