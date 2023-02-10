import { Component, OnInit, ViewChild, Renderer2, ElementRef } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { ProjectService } from '../../services/project.service';
import { ProjectResourceModel } from '../../models/ProjectResource.model';
/* Primeng API */
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService, TreeNode } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
/* end PrimeNg API */
import { FileUpload, Table } from 'primeng';
import { ProjectModel } from '../../models/project.model';
import { ProjectScopeModel } from '../../models/ProjectScope.model';
import { TranslateService } from '@ngx-translate/core';

import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { TaskDocument, TaskModel } from '../../models/ProjectTask.model';
import * as $ from 'jquery';
import { TreeTable } from 'primeng';
import { TaskRelate } from '../../models/TaskRelate.model';

class Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}
interface DialogResult {
  status: boolean;
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
  selector: 'app-add-task-scope-dialog',
  templateUrl: './add-task-scope-dialog.component.html',
  styleUrls: ['./add-task-scope-dialog.component.css']
})
export class AddTaskScopeDialogComponent implements OnInit {
  loading: boolean = false;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  listDistributionRate: Array<any> = [];
  minDate: Date = new Date();

  @ViewChild('myTable') myTable: Table;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  defaultNumberType = this.systemParameterList.find((systemParameter: { systemKey: string; }) => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;

  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  colsFile: any[];
  //START: BIẾN ĐIỀU KIỆN
  isOpenNotifiError: boolean = false;
  isInvalidForm: boolean = false;
  //END : BIẾN ĐIỀU KIỆN

  listRelateTask: Array<TaskRelate> = [];


  selectedObjectType: string = '1';// Mức độ ưu tiên
  selecteTimeSetting: string = 'auto'; // Tủy chỉnh thời gian

  /*START: FORM CONTROL*/
  createTaskScopeForm: FormGroup;
  taskCodeControl: FormControl;
  taskNameControl: FormControl;
  scopeControl: FormControl; // Hạng mục
  descriptionControl: FormControl;
  statusControl: FormControl; // Trạng thái
  percentControl: FormControl; // Phần trăm hoàn thành

  startTimeControl: FormControl; // Thời gian bắt đầu
  endTimeControl: FormControl; //Thời gian kết thúc
  hourControl: FormControl; // Số giờ dự kiên
  isWeekendControl: FormControl; //Tính cuối tuần
  actualHourControl: FormControl; // Số giờ thực tế
  /*END: FORM CONTROL*/
  listTaskDocument: Array<TaskDocument> = [];
  listTaskDocumentDelete: Array<TaskDocument> = [];
  /*START: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  listTaskType: Array<Category> = [];
  listStatus: Array<Category> = [];
  listResource: Array<ProjectResourceModel> = [];
  listProjectScope: Array<ProjectScopeModel> = [];
  project: ProjectModel = null;
  projectScope: ProjectScopeModel = null;
  /*END: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  /*START: TABLE*/
  colsList: any;
  selectedColumns: any[];
  /*END: TABLE*/
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
  projectId: string = localStorage.getItem("Project_ID");
  taskId: string = null;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  display: boolean = false;
  selectedNode: TreeNode;
  columns: any;
  @ViewChild('treeTable') treeTable: TreeTable;
  listNoteProjectScope: TreeNode[];
  projectScopeId: string;
  minDateProject: Date = null;
  maxDateProject: Date = null;
  status: Category;

  tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink']
  };

  constructor(
    private messageService: MessageService,
    private translate: TranslateService,
    private renderer: Renderer2,
    private confirmationService: ConfirmationService,
    public ref: DynamicDialogRef,
    private el: ElementRef,
    private projectService: ProjectService,
    private imageService: ImageUploadService,
    public config: DynamicDialogConfig) {
    this.projectId = this.config.data.projectId;
    this.taskId = this.config.data.taskId;
    this.projectScope = this.config.data.projectScope;

    this.translate.setDefaultLang('vi');
    this.renderer.listen('window', 'click', (e: Event) => {
      if (this.toggleButton && this.notifi) {
        if (!this.toggleButton.nativeElement.contains(e.target) &&
          !this.notifi.nativeElement.contains(e.target) &&
          !this.save.nativeElement.contains(e.target) &&
          !this.saveAndCreate.nativeElement.contains(e.target)) {
          this.isOpenNotifiError = false;
        }
      }
    });
  }

  ngOnInit(): void {
    this.initForm();
    this.initTable();
    this.getMasterData();

  }

  initForm() {
    this.taskNameControl = new FormControl('', [Validators.required, forbiddenSpaceText]);
    this.scopeControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);

    this.taskCodeControl = new FormControl(null);
    this.descriptionControl = new FormControl('');
    this.statusControl = new FormControl();
    this.percentControl = new FormControl(null);
    this.startTimeControl = new FormControl(null, [Validators.required]);
    this.endTimeControl = new FormControl(null, [Validators.required]);
    this.hourControl = new FormControl('0', [Validators.required]);
    this.isWeekendControl = new FormControl(false);
    this.actualHourControl = new FormControl('0');
    this.createTaskScopeForm = new FormGroup({
      taskNameControl: this.taskNameControl,
      taskCodeControl: this.taskCodeControl,
      scopeControl: this.scopeControl,
      descriptionControl: this.descriptionControl,
      statusControl: this.statusControl,
      percentControl: this.percentControl,
      startTimeControl: this.startTimeControl,
      endTimeControl: this.endTimeControl,
      hourControl: this.hourControl,
      isWeekendControl: this.isWeekendControl,
      actualHourControl: this.actualHourControl,
    });
  }

  initTable() {
    this.colsList = [
      { field: 'nameResource', header: 'Người tham gia', textAlign: 'left', display: 'table-cell' },
    ];

    this.selectedColumns = this.colsList;

    this.colsFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'documentSize', header: 'Kích thước', width: '50%', textAlign: 'left', type: 'number' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'left', type: 'date' },
      { field: 'createByName', header: 'Người Upload', width: '50%', textAlign: 'left', type: 'string' },
    ];
  }

  getMasterData() {
    this.loading = true;
    this.projectService.getMasterDataCreateOrUpdateTask(this.projectId, this.taskId, this.auth.UserId).subscribe(response => {
      let result: any = response;

      if (result.statusCode === 200) {
        this.listStatus = result.listStatus;
        this.listTaskType = result.listTaskType;

        this.listResource = result.listProjectResource;
        this.listTaskDocument = result.listTaskDocument;
        this.project = result.project;

        this.minDateProject = this.project.projectStartDate ? new Date(this.project.projectStartDate) : null;
        this.maxDateProject = this.project.projectEndDate ? new Date(this.project.projectEndDate) : null;

        if (this.taskId != null) {
          this.mappingDataToForm(result.task);
          this.status = this.listStatus.find(x => x.categoryId == result.task.status);

          this.setDataFollowStatus();
        }
        else {
          // set default status
          let stt = this.listStatus.find(x => x.categoryCode == 'NEW');
          this.statusControl.setValue(stt);
          this.scopeControl.setValue(this.projectScope.projectScopeName);

          this.listResource.forEach(item => {
            item.startTime = new Date(item.startTime);
            item.endTime = new Date(item.endTime);
          });
        }

        this.loading = false;
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  mappingDataToForm(task: TaskModel) {

    this.taskCodeControl.setValue(task.taskCode);
    this.taskNameControl.setValue(task.taskName);
    this.scopeControl.setValue(task.projectScopeName);
    this.descriptionControl.setValue(task.description);
    let status = this.listStatus.find(c => c.categoryId == task.status);
    this.statusControl.setValue(status);
    this.selectedObjectType = task.priority.toString();
    let percent = this.listPhanTram.find(c => c.value == task.taskComplate);
    this.percentControl.setValue(percent);
    task.planStartTime ? this.startTimeControl.setValue(new Date(task.planStartTime)) : null;
    task.planEndTime ? this.endTimeControl.setValue(new Date(task.planEndTime)) : null;
    this.hourControl.setValue(task.estimateHour);
    this.actualHourControl.setValue(task.actualHour);
    this.isWeekendControl.setValue(task.includeWeekend);
    this.selecteTimeSetting = task.timeType;

    this.listResource.forEach(item => {
      let resouseMapping = task.listTaskResource.find(c => c.resourceId == item.projectResourceId);
      if (resouseMapping != null) {
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

  async createTask() {

    if (!this.createTaskScopeForm.valid) {
      Object.keys(this.createTaskScopeForm.controls).forEach(key => {
        if (!this.createTaskScopeForm.controls[key].valid) {
          this.createTaskScopeForm.controls[key].markAsTouched();
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
      if (this.hourControl.value == 0) {
        this.showToast('error', 'Thông báo', ' Số giờ dự kiến thực hiện cần lớn hơn 0');
      }
      else {
        this.isInvalidForm = false;
        this.isOpenNotifiError = false;

        let task = this.mappingDateFromFormToModel();

        let listResource = this.listResource.filter(c => c.isPic == true || c.isCheck == true);
        this.loading = true;

        let folderType = `${this.project.projectCode}_TASK_FILE`;

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
          if (result.statusCode == 200) {
            this.loading = false;
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Thêm công việc thành công');
            let result: DialogResult = {
              status: true
            }
            this.ref.close(result);
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    }
  }
  setDataFollowStatus() {
    switch (this.status.categoryCode) {
      case "NEW":
        this.listStatus = this.listStatus;
        this.createTaskScopeForm.enable();
        break;
      case "DL":
        this.listStatus = this.listStatus.filter(c => c.categoryCode != "NEW");
        this.createTaskScopeForm.enable();
        this.disableControl();
        this.endTimeControl.disable();
        break;
      case "HT":
        this.listStatus = this.listStatus.filter(c => c.categoryCode != "NEW");
        this.createTaskScopeForm.enable();
        this.endTimeControl.disable();
        this.disableControl();
        break;
      case "CLOSE":
        this.listStatus = this.listStatus.filter(c => c.categoryCode == "DL" || c.categoryCode == "CLOSE");
        this.createTaskScopeForm.disable();
        this.statusControl.enable();
        break;
    }
  }

  disableControl() {
    this.startTimeControl.disable();
    this.endTimeControl.disable();
    this.hourControl.disable();
  }

  async updateTask() {

    if (!this.createTaskScopeForm.valid) {

      Object.keys(this.createTaskScopeForm.controls).forEach(key => {
        if (!this.createTaskScopeForm.controls[key].valid) {
          this.createTaskScopeForm.controls[key].markAsTouched();
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
      if (this.hourControl.value == 0) {
        this.showToast('error', 'Thông báo', 'Số giờ dự kiến thực hiện cần lớn hơn 0');
      }
      else {

        this.isInvalidForm = false;
        this.isOpenNotifiError = false;
        let task = this.mappingDateFromFormToModel();
        let listResource = this.listResource.filter(c => c.isPic == true || c.isCheck == true);
        this.loading = true;
        // if (this.uploadedFiles.length > 0) {
        //   this.uploadFiles(this.uploadedFiles);
        //   this.converToListTaskDocument(this.uploadedFiles);
        // }

        let folderType = `${this.project.projectCode}_TASK_FILE`;

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

        try {
          this.projectService.createOrUpdateTask(task, folderType, listResource, listFileUploadModel, this.listTaskDocumentDelete, this.auth.UserId, this.listRelateTask).subscribe(response => {
            let result: any = response;
            if (result.statusCode == 200) {

              this.clearToast();
              this.showToast('success', 'Thông báo', 'Chỉnh sửa công việc thành công');
              let result: DialogResult = {
                status: true
              }
              this.ref.close(result);
            } else {
              this.clearToast();
              this.showToast('error', 'Thông báo', result.messageCode);
            }
            this.loading = false;
          });
        }
        catch
        {
          this.loading = false;
        }
      }
    }
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

  mappingDateFromFormToModel(): TaskModel {

    let task = new TaskModel();
    if (this.taskId != null)
      task.taskId = this.taskId;
    task.taskName = this.taskNameControl.value.trim();
    task.projectId = this.projectId;
    task.projectScopeId = this.projectScope ? this.projectScope.projectScopeId : null;
    task.description = this.descriptionControl.value ? this.descriptionControl.value.trim() : null;
    task.planStartTime = this.startTimeControl.value ? convertToUTCTime(this.startTimeControl.value) : null;
    task.planEndTime = this.endTimeControl.value ? convertToUTCTime(this.endTimeControl.value) : null;
    task.estimateHour = this.hourControl.value ? ParseStringToFloat(this.hourControl.value) : 0;

    task.actualHour = this.actualHourControl.value ? ParseStringToFloat(this.actualHourControl.value) : 0;
    task.priority = this.selectedObjectType ? ParseStringToFloat(this.selectedObjectType) : 1;
    task.status = this.statusControl.value ? this.statusControl.value.categoryId : null;
    task.includeWeekend = this.isWeekendControl.value;
    task.taskComplate = this.percentControl.value ? this.percentControl.value.value : 0;
    task.timeType = this.selecteTimeSetting;
    task.milestonesId = null;

    return task;
  }
  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: TaskDocument) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listTaskDocument.indexOf(file);
        this.listTaskDocument.splice(index, 1);
        this.confirmationService.close();
      },
      reject: () => {
        this.confirmationService.close();
      }
    });
  }

  changePic() {
    if (this.selecteTimeSetting == "auto") {
      let timeAll = this.hourControl.value ?? 0;
      this.numberPic = this.listResource.filter(c => c.isPic == true).length;
      var timeOfOneResource = timeAll.replace(',', '') / this.numberPic;
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

  uploadFiles(files: File[]) {
    this.imageService.uploadFile(files).subscribe(response => { });
  }

  clickHandTime() {
    this.listResource.forEach(item => {
      item.hours = 0;
    });
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  async openProjetScopeDialog() {
    this.display = true;
    this.initTableProjectResource();
    await this.getMasterDataProjectResource();
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
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    });
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

  cancel() {
    this.confirmationService.confirm({
      message: 'Hành động này không thể được hoàn tác, bạn có chắc chắn muốn hủy?',
      accept: () => {
        //this.updateTask();
        this.confirmationService.close();
        let result: DialogResult = {
          status: false
        }
        this.ref.close(result);

      },
      reject: () => {
        this.confirmationService.close();
      }
    });
  }

  showMessenger(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }
  clearToast() {
    this.messageService.clear();
  }
  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ key: "popup", severity: severity, summary: summary, detail: detail });
  }

  clearMessenger() {
    this.messageService.clear();
  }

  async getMasterDataProjectResource() {
    this.loading = true;

    await this.projectService.getProjectScopeByProjectId(this.projectId, this.auth.UserId).subscribe(response => {
      let result: any = response;

      this.loading = false;
      if (result.statusCode === 200) {
        this.listProjectScope = result.listProjectScrope;
        this.listNoteProjectScope = this.list_to_tree();
      } else {
        this.clearMessenger();
        this.showMessenger('error', 'Thông báo', result.Message);
      }
    });
  }

  list_to_tree() {
    let list: Array<TreeNode> = [];
    this.listProjectScope.forEach(item => {
      let node: TreeNode = {
        label: item.projectScopeName,
        expanded: true,
        data: {
          'Id': item.projectScopeId,
          'ParentId': item.parentId,
          "Name": item.projectScopeName,
        },
        children: []
      };

      list = [...list, node];
    });

    var map = {}, node: TreeNode, roots = [], i: number;

    for (i = 0; i < list.length; i += 1) {
      map[list[i].data.Id] = i; // initialize the map
      list[i].children = []; // initialize the children
    }

    for (i = 0; i < list.length; i += 1) {
      node = list[i];
      if (node.data.ParentId !== null) {
        // if you have dangling branches check that map[node.ParentId] exists
        list[map[node.data.ParentId]].children.push(node);
      } else {
        roots.push(node);
      }
    }
    return roots;
  }

  initTableProjectResource() {
    this.columns = [
      { field: 'Name', header: 'Hạng mục', textAlign: 'left' },
    ];
  }

  selectedScope(rowData: any) {

    let projectScope = this.listProjectScope.find(c => c.projectScopeId == rowData.Id);
    this.projectScope = projectScope;
    this.scopeControl.setValue(this.projectScope.projectScopeName);
    this.display = false;
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