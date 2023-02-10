import { Component, OnInit, ViewChild, ElementRef, HostListener, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission';
import { NoteModel } from '../../../shared/models/note.model';

import { MenuItem, MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DialogService } from 'primeng/dynamicdialog';
import { DatePipe, DecimalPipe, formatNumber } from '@angular/common';
import { ReSearchService } from '../../../services/re-search.service';
import { ProjectService } from '../../services/project.service';

import { ProjectMilestoneModel } from '../../models/milestone.model';
import { ProjectModel } from '../../models/project.model';
import { CreateOrUpdateMilestoneDialogComponent } from '../create-or-update-milestone-dialog/create-or-update-milestone-dialog.component';
import { AddOrRemoveTaskMilestoneDialogComponent } from '../add-or-remove-task-milestone-dialog/add-or-remove-task-milestone-dialog.component';
import { TaskModel } from '../../models/ProjectTask.model';
import * as $ from 'jquery';
import { NoteService } from '../../../shared/services/note.service';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { FileUpload } from 'primeng';
import { Paginator } from 'primeng/paginator';

class Project {
  projectId: string;
  projectCode: string;
  projectName: string;
  projectCodeName: string;
  projectStatusPlan: boolean;
}

class Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
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

@Component({
  selector: 'app-project-milestone',
  templateUrl: './project-milestone.component.html',
  styleUrls: ['./project-milestone.component.css'],
  providers: [DialogService, DatePipe, DecimalPipe]
})
export class ProjectMilestoneComponent implements OnInit {
  @ViewChild('myTable') myTable: Table;
  @ViewChild('fileUploadNote') fileUploadNote: FileUpload;
  @ViewChild('fileUpload') fileUpload: FileUpload;

  isGlobalFilter: string = '';

  listProjectTitle: Array<Project> = [];
  selectedProject: Project = null;
  isApproveProject: boolean = false;

  auth = JSON.parse(localStorage.getItem("auth"));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  projectId: string = this.emptyGuid;
  projectMilestoneId: string = this.emptyGuid;
  filterGlobal: string;
  innerWidth: number = 0; //number window size first
  loading: boolean = false;

  startDate: Date = new Date();
  minYear: number = 2015;
  currentYear: number = (new Date()).getFullYear();
  isManager: boolean = null;

  //START: BIẾN ĐIỀU KIỆN
  fixed: boolean = false;
  withFiexd: string = "";
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  //END : BIẾN ĐIỀU KIỆN

  percent: number = 15;

  /*BIẾN NHẬN GIÁ TRỊ TRẢ VỀ*/
  listProjectMilestoneInProgress: Array<ProjectMilestoneModel> = [new ProjectMilestoneModel()];
  listProjectMilestoneComplete: Array<ProjectMilestoneModel> = [new ProjectMilestoneModel()];
  project: ProjectModel = null;
  totalEstimateTime: string = '0';
  totalPercent: number = 0; // Tiến độ dự án

  listTaskNew: Array<TaskModel> = [];
  listTaskInProgress: Array<TaskModel> = [];
  listTaskComplete: Array<TaskModel> = [];
  listTaskClose: Array<TaskModel> = [];
  isContainResource: boolean = false;
  listStatus: Array<Category> = [];
  projectMilestone: ProjectMilestoneModel = null;
  noteHistory: Array<Note> = [];
  /*END*/

  colNumber: number = 3;
  draggedTask: TaskModel;
  actions: MenuItem[] = [];
  actionsMilestone: MenuItem[] = [];

  /*NOTE*/
  noteContent: string = '';
  noteId: string;
  listUpdateNoteDocument: Array<NoteDocument> = [];
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  /*END*/
  listTaskId: Array<string> = [];

  /*BIẾN ĐIỀU KIỆN PHÂN QUYỀN*/
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionEditNote: boolean = true;
  /*END*/
  isSendEmailControl: boolean = true;
  totalRecordsNote: number = 0;
  listPermissionResourceActive: string = localStorage.getItem("ListPermissionResource");
  @ViewChild('paginator', { static: true }) paginator: Paginator
  constructor(
    private ref: ChangeDetectorRef,
    private dialogService: DialogService,
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private translate: TranslateService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private datePipe: DatePipe,
    public reSearchService: ReSearchService,
    private projectService: ProjectService,
    private noteService: NoteService,
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
    if (this.innerWidth < 1600) {
      this.colNumber = 4;
    } else {
      this.colNumber = 3;
    }
  }

  async ngOnInit() {
    let resource = "pro/project/milestone/";
    this.route.params.subscribe(async params => {
      this.projectId = params['projectId'];

      let permission: any = await this.getPermission.getPermissionLocal(this.listPermissionResourceActive, resource);
      if (permission.status == false) {
        this.router.navigate(['/home']);
      }
      else {
        let listCurrentActionResource = permission.listCurrentActionResource;
        if (listCurrentActionResource.indexOf("add") == -1) {
          this.actionAdd = false;
        }
        if (listCurrentActionResource.indexOf("edit") == -1) {
          this.actionEdit = false;
        }
        if (listCurrentActionResource.indexOf("delete") == -1) {
          this.actionDelete = false;
        }
        if (listCurrentActionResource.indexOf("editnote") == -1) {
          this.actionEditNote = false;
        }
        this.getMasterData();
      }
    });
  }

  getMasterData() {
    this.loading = true;
    this.projectService.getMasterDataProjectMilestone(this.projectId, this.auth.UserId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        let listInProgress = result.listProjectMilestoneInProgress;

        this.listProjectTitle = result.listProject;
        this.listProjectTitle.forEach(item => {
          item.projectCodeName = `[${item.projectCode}]: ${item.projectName}`;
        });
        this.selectedProject = this.listProjectTitle.find(c => c.projectId == this.projectId);
        this.isApproveProject = this.selectedProject.projectStatusPlan;

        let listComplete = result.listProjectMilestoneComplete;
        listComplete.forEach((item, index) => {
          item.index = index + 1;
        });
        listInProgress.forEach((item, index) => {
          item.index = index + 1;
        });
        this.listProjectMilestoneComplete = [new ProjectMilestoneModel()];
        this.listProjectMilestoneInProgress = [new ProjectMilestoneModel()];

        this.listProjectMilestoneComplete = this.listProjectMilestoneComplete.concat(listComplete);
        this.listProjectMilestoneInProgress = this.listProjectMilestoneInProgress.concat(listInProgress);

        // lấy thông ngày cập nhập cuối cho mỗi mốc dự án
        this.listProjectMilestoneInProgress.forEach(mile => {

          // if (mile.updateDate != null)
          //   mile.dayOfWeek = mile.updateDate.getDay();
        });

        this.project = result.project;
        this.totalPercent = result.projectTaskComplete.toFixed(2);
        this.totalEstimateTime = result.totalEstimateHour;
        this.totalRecordsNote = result.totalRecordsNote;
        this.noteHistory = result.listNote;
        this.handleNoteContent();
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  // Thêm mới mốc dự án
  addMilestone() {
    let width = '';
    if (window.innerWidth < 500) {
      width = '100%';
    } else if (window.innerWidth < 1240) {
      width = '60%';
    } else {
      width = '45%';
    }

    let ref = this.dialogService.open(CreateOrUpdateMilestoneDialogComponent, {
      data: {
        projectId: this.projectId,
        isCreate: true,
      },
      header: 'Thêm mốc dự án',
      width: width,
      baseZIndex: 1030,
      contentStyle: {
        "overflow-x": "hidden",
        "min-height": "400px",
        "overflow-y": "auto"
      }
    });
    ref.onClose.subscribe(async (result: any) => {
      if (result == true) {
        this.getMasterData();
        this.clearToast();
        this.showToast('success', 'Thông báo', 'Thêm mới mốc dự án thành công');
      }
    });
  }

  editMilestone(item: ProjectMilestoneModel) {
    let width = '';
    if (window.innerWidth < 500) {
      width = '100%';
    } else if (window.innerWidth < 1240) {
      width = '60%';
    } else {
      width = '45%';
    }

    let ref = this.dialogService.open(CreateOrUpdateMilestoneDialogComponent, {
      data: {
        projectId: this.projectId,
        projectMilestoneId: item.projectMilestonesId,
        isCreate: false,
        isApproveProject: this.isApproveProject,
      },
      header: 'Chỉnh sửa mốc dự án',
      width: width,
      baseZIndex: 1030,
      contentStyle: {
        "overflow-x": "hidden",
        "min-height": "400px",
        "overflow-y": "auto"
      }
    });
    ref.onClose.subscribe(async (result: any) => {
      if (result == true) {
        this.getMasterData();
        this.clearToast();
        this.showToast('success', 'Thông báo', 'Chỉnh sửa mốc dự án thành công');
      }
    });
  }

  deleteMilestone(item: ProjectMilestoneModel) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn Xóa mốc dự án đang chọn?',
      accept: () => {
        this.loading = true;
        this.projectService.updateStatusProjectMilestone(item.projectMilestonesId, 2, this.auth.UserId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.projectMilestoneId = this.emptyGuid;
            this.getMasterData();
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Xóa mốc dự án thành công!');
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      },
    });
  }

  closeMilestone(item: ProjectMilestoneModel) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn Đóng mốc dự án đang chọn?',
      accept: () => {
        this.loading = true;
        this.projectService.updateStatusProjectMilestone(item.projectMilestonesId, 0, this.auth.UserId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.getMasterData();
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Đóng mốc dự án thành công!');
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      },
    });
  }

  addTaskToMilestone(item: ProjectMilestoneModel) {
    let width = '';
    if (window.innerWidth < 500) {
      width = '100%';
    } else {
      width = '85%';
    }

    let ref = this.dialogService.open(AddOrRemoveTaskMilestoneDialogComponent, {
      data: {
        projectId: this.projectId,
        projectMilestoneId: item.projectMilestonesId,
        type: 0,
      },
      header: `Thêm công việc > ${this.project.projectName} > ${item.name}`,
      width: width,
      baseZIndex: 1030,
      contentStyle: {
        "overflow-x": "hidden",
        "min-height": "auto",
        "overflow-y": "auto"
      }
    });
    ref.onClose.subscribe(async (result: any) => {
      if (result == true) {
        this.getMasterData();
        this.clearToast();
        this.showToast('success', 'Thông báo', 'Thêm công việc vào mốc dự án thành công');
        if (this.projectMilestoneId && this.projectMilestoneId != this.emptyGuid) {
          this.getDetailMileStone();
        }
      }
    });
  }

  deleteOneTaskFromMilestone(taskId: any) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa công việc khỏi mốc dự án',
      accept: () => {
        this.loading = true;
        this.listTaskId.push(taskId);
        this.projectService.addOrRemoveTaskMilestone(this.listTaskId, this.projectMilestoneId, 1, this.auth.UserId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Xóa công việc khỏi mốc dự án thành công');
            this.getMasterData();
            if (this.projectMilestoneId && this.projectMilestoneId != this.emptyGuid) {
              this.getDetailMileStone();
            }
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
          this.loading = false;
        });
      },
    });
  }

  deleteTaskFromMilestone(item: ProjectMilestoneModel) {

    let width = '';
    if (window.innerWidth < 500) {
      width = '100%';
    } else {
      width = '85%';
    }

    let ref = this.dialogService.open(AddOrRemoveTaskMilestoneDialogComponent, {
      data: {
        projectId: this.projectId,
        projectMilestoneId: item.projectMilestonesId,
        type: 1,
      },
      header: `Xóa công việc khỏi mốc dự án`,
      width: width,
      baseZIndex: 1030,
      contentStyle: {
        "overflow-x": "hidden",
        "min-height": "auto",
        "overflow-y": "auto"
      }
    });


    ref.onClose.subscribe(async (result: any) => {

      if (result == true) {
        this.clearToast();
        this.showToast('success', 'Thông báo', 'Xóa công việc khỏi mốc dự án thành công');
        this.getMasterData();
        if (this.projectMilestoneId && this.projectMilestoneId != this.emptyGuid) {
          this.getDetailMileStone();
        }
      }
    });
  }

  reOpenMilestone(item: ProjectMilestoneModel) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn mở lại mốc dự án đang chọn?',
      accept: () => {
        this.loading = true;
        this.projectService.updateStatusProjectMilestone(item.projectMilestonesId, 1, this.auth.UserId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.getMasterData();
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Mở lại mốc dự án thành công!');
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      },
    });
  }

  showDetailMilestone(item: ProjectMilestoneModel) {
    this.projectMilestoneId = item.projectMilestonesId;
    this.getDetailMileStone();
  }

  getDetailMileStone() {
    this.loading = true;
    this.projectService.getDataMilestoneById(this.projectId, this.projectMilestoneId, this.auth.UserId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {

        this.listTaskNew = result.listTaskNew;
        this.listTaskInProgress = result.listTaskInProgress;
        this.listTaskComplete = result.listTaskComplete;
        this.listTaskClose = result.listTaskClose;
        this.listStatus = result.listStatus;
        this.isContainResource = result.isContainResource;
        this.projectMilestone = result.projectMilestone;

      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  typeDragStart: string = '';
  typeDrop: string = '';
  typeEqual: boolean = false;
  isTypeStatusDrop: boolean = true;

  dragStart(event, task: TaskModel, type: string) {
    this.draggedTask = task;
    this.typeDragStart = type;
  }

  drop(event, type: string) {
    if (this.typeDragStart === type) {
      this.typeEqual = true;
      return;
    }
    this.typeDrop = type;
    this.isTypeStatusDrop = this.checkDropStatus();
    if (this.isTypeStatusDrop) return;
    if (this.draggedTask) {
      switch (type) {
        case "NEW":
          break;
        case "INPROGRESS":
          if (this.typeDragStart == "CLOSE" || this.typeDragStart == "COMPLETE") {
            this.reOpen(this.draggedTask);
          } else {
            this.startTaskConfirm(this.draggedTask);
          }
          break;
        case "COMPLETE":
          this.completeTask(this.draggedTask);
          break;
        case "CLOSE":
          this.closeTask(this.draggedTask);
          break;
      }
    }
    this.typeEqual = false;
    this.typeDrop = null;
    this.ref.detectChanges();
  }

  dragEnd(event, type: string) {
    if (this.typeEqual) {
      this.typeEqual = false;
      return;
    }
    if (this.isTypeStatusDrop) return;
    if (this.draggedTask) {
      switch (type) {
        case "NEW":
          break;
        case "INPROGRESS":
          break;
        case "COMPLETE":
          break;
        case "CLOSE":
          break;
      }
    }
    this.draggedTask = null;
    this.ref.detectChanges();
  }

  checkDropStatus(): boolean {
    if (!this.typeDrop) {
      return true;
    }
    if ((this.typeDragStart == "INPROGRESS" && this.typeDrop == "NEW") || (this.typeDragStart == "COMPLETE" && this.typeDrop == "NEW")
      || (this.typeDragStart == "CLOSE" && this.typeDrop == "NEW") || (this.typeDragStart == "CLOSE" && this.typeDrop == "COMPLETE")) {
      return true;
    }
    return false;
  }


  openPopupCreateTask() {
    this.router.navigate(['/project/create-project-task', { 'projectId': this.projectId, 'milestoneId': this.projectMilestoneId }]);
  }

  onChangeActionMilestone() {
    this.actionsMilestone = [];
    let createTask: MenuItem = {
      id: '1', label: 'Tạo công việc', icon: 'pi pi-plus', command: () => {
        this.openPopupCreateTask();
      }
    }

    let addTaskToMilestone: MenuItem = {
      id: '2', label: 'Thêm công việc', icon: 'pi pi-calendar-plus', command: () => {
        this.addTaskToMilestone(this.projectMilestone);
      }
    }

    let editMilestone: MenuItem = {
      id: '3', label: 'Chỉnh sửa mốc dự án', icon: 'pi pi-pencil', command: () => {
        this.editMilestone(this.projectMilestone);
      }
    }

    let deleteMilestone: MenuItem = {
      id: '4', label: 'Xóa mốc dự án', icon: 'pi pi-trash', command: () => {
        this.deleteMilestone(this.projectMilestone);
      }
    }

    let closeMilestone: MenuItem = {
      id: '5', label: 'Hoàn thành mốc dự án', icon: 'pi pi-check', command: () => {
        this.closeMilestone(this.projectMilestone);
      }
    }

    this.actionsMilestone.push(createTask);
    this.actionsMilestone.push(addTaskToMilestone);
    this.actionsMilestone.push(editMilestone);
    this.actionsMilestone.push(deleteMilestone);
    this.actionsMilestone.push(closeMilestone);
  }

  onChangeAction(rowData: TaskModel) {
    let status = this.listStatus.find(c => c.categoryId === rowData.status);
    this.actions = [];
    let start: MenuItem = {
      id: '1', label: 'Bắt đầu', icon: 'pi pi-play', command: () => {
        this.startTaskConfirm(rowData);
      }
    }

    let complete: MenuItem = {
      id: '2', label: 'Hoàn thành', icon: 'pi pi-check', command: () => {
        this.completeTask(rowData);
      }
    }
    let close: MenuItem = {
      id: '3', label: 'Đóng', icon: 'pi pi-times', command: () => {
        this.closeTask(rowData);
      }
    }
    let update: MenuItem = {
      id: '4', label: 'Cập nhật', icon: 'pi pi-pencil', command: () => {
        this.router.navigate(['/project/detail-project-task', { 'taskId': rowData.taskId, 'projectId': this.projectId }]);
      }
    }
    let goToMilestone: MenuItem = {
      id: '5', label: 'Di chuyển tới mốc', icon: 'pi pi-external-link', command: () => {
        // this.send_sms(rowData.customerId);
      }
    }
    let deteled: MenuItem = {
      id: '6', label: 'Xóa khỏi mốc', icon: 'pi pi-trash', command: () => {
        this.deleteOneTaskFromMilestone(rowData.taskId);
      }
    }
    let reOpen: MenuItem = {
      id: '7', label: 'Mở lại', icon: 'pi pi-replay', command: () => {
        this.reOpen(rowData);
      }
    }
    switch (status.categoryCode) {
      case "NEW":
        this.actions.push(start);
        this.actions.push(complete);
        this.actions.push(close);
        this.actions.push(update);
        this.actions.push(goToMilestone);
        this.actions.push(deteled);
        break;
      case "DL":
        this.actions.push(complete);
        this.actions.push(update);
        this.actions.push(close);
        this.actions.push(goToMilestone);
        this.actions.push(deteled);
        break;
      case "HT":
        this.actions.push(close);
        this.actions.push(update);
        this.actions.push(reOpen);
        this.actions.push(deteled);
        break;
      case "CLOSE":
        this.actions.push(update);
        this.actions.push(reOpen);
        this.actions.push(goToMilestone);
        this.actions.push(deteled);
        break;
    }
  }

  startTaskConfirm(task: TaskModel) {
    if (task.isHavePic) {
      this.startTask(task.taskId);
    } else {
      if (this.isContainResource) {
        this.confirmationService.confirm({
          message: 'Công việc này chưa được giao cho ai. Bạn có chắc chắn muốn nhận công việc này không?',
          accept: () => {
            this.startTask(task.taskId);
          },
        });
      } else {
        this.confirmationService.confirm({
          message: 'Công việc này chưa có người phụ trách!',
          accept: () => {
          },
        });
      }
    }
  }

  startTask(taskId: string) {
    let newStatus = this.listStatus.find(c => c.categoryCode == "DL");
    this.loading = true;
    this.projectService.changeStatusTask(taskId, newStatus.categoryId, 1, this.auth.UserId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.clearToast();
        this.showToast('success', 'Thông báo', "Thay đổi trạng thái công việc thành công!");
        this.getDetailMileStone();
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  reOpen(rowData: TaskModel) {
    let isCheck = false;
    if (rowData.actualEndTime) {
      isCheck = CompareDateNow(new Date(rowData.actualEndTime));
    }
    if (isCheck == true) {
      this.confirmationService.confirm({
        message: 'Dự án / công việc này thực tế đã kết thúc, nếu mở lại thông tin kết thúc thực tế sẽ bị xóa',
        accept: () => {
          let newStatus = this.listStatus.find(c => c.categoryCode == "DL");
          this.loading = true;
          this.projectService.changeStatusTask(rowData.taskId, newStatus.categoryId, 2, this.auth.UserId).subscribe(response => {
            let result: any = response;
            this.loading = false;
            if (result.statusCode == 200) {
              this.clearToast();
              this.showToast('success', 'Thông báo', "Thay đổi trạng thái công việc thành công!");
              this.getDetailMileStone();
              this.getMasterData();
            } else {
              this.clearToast();
              this.showToast('error', 'Thông báo', result.messageCode);
            }
          });
        },
      });
    } else {
      let newStatus = this.listStatus.find(c => c.categoryCode == "DL");
      this.loading = true;
      this.projectService.changeStatusTask(rowData.taskId, newStatus.categoryId, 2, this.auth.UserId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.clearToast();
          this.showToast('success', 'Thông báo', "Thay đổi trạng thái công việc thành công!");
          this.getDetailMileStone();
          this.getMasterData();
        } else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      });
    }
  }

  completeTask(rowData: TaskModel) {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn muốn Hoàn thành công việc số ${rowData.taskCode}`,
      accept: () => {
        let completeStatus = this.listStatus.find(c => c.categoryCode == "HT");
        this.loading = true;
        this.projectService.changeStatusTask(rowData.taskId, completeStatus.categoryId, 0, this.auth.UserId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.clearToast();
            this.showToast('success', 'Thông báo', "Thay dổi trạng thái công việc thành công!");
            this.getDetailMileStone();
            this.getMasterData();
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    });
  }

  closeTask(rowData: TaskModel) {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn muốn Đóng công việc số ${rowData.taskCode}`,
      accept: () => {
        let closeStatus = this.listStatus.find(c => c.categoryCode == "CLOSE");
        this.loading = true;
        this.projectService.changeStatusTask(rowData.taskId, closeStatus.categoryId, 0, this.auth.UserId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.clearToast();
            this.showToast('success', 'Thông báo', "Thay đổi trạng thái công việc thành công!");
            this.getDetailMileStone();
            this.getMasterData();
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    });
  }

  deleteTask(rowData: TaskModel) {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn muốn xóa công việc số ${rowData.taskCode}`,
      accept: () => {
        this.loading = true;
        this.projectService.deleteTask(rowData.taskId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.clearToast();
            this.showToast('success', 'Thông báo', "Xóa công việc thành công!");
            this.getDetailMileStone();
            this.getMasterData();
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    });
  }

  /*Event thay đổi nội dung ghi chú*/
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  /*Xử lý và hiển thị lại nội dung ghi chú*/
  handleNoteContent() {
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
    //if (note.noteDocList.length > 3) return true;
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
    let noteModel = new NoteModel();
    if (!this.noteId) {
      /*Tạo mới ghi chú*/
      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.projectId;
      noteModel.ObjectType = 'MILESTONE';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/
      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'EDT';
      noteModel.ObjectId = this.projectId;
      noteModel.ObjectType = 'MILESTONE';
      noteModel.NoteTitle = 'đã sửa ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();

    }

    this.noteService.createNoteMilestone(noteModel, this.isSendEmailControl).subscribe(response => {
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
        this.paginator.changePage(0);
        this.clearToast();
        this.showToast('success', 'Thông báo', "Lưu ghi chú thành công!");
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }


  cancel() {
    this.confirmationService.confirm({
      message: `Các thay đổi sẽ không được lưu lại. Hành động này không thể được hoàn tác, bạn có chắc chắn muốn thoát?`,
      accept: () => {
        this.router.navigate(['/project/detail', { projectId: this.projectId }]);
      }
    });
  }

  async paginate(event) {
    //event.first = Index of the first record
    //event.rows = Number of rows to display in new page
    //event.page = Index of the new page
    //event.pageCount = Total number of pages

    let result: any = await this.projectService.pagingProjectNote(this.projectId, event.rows, event.page, "MILESTONE");
    if (result.statusCode == 200) {
      this.noteHistory = result.noteEntityList;
      this.totalRecordsNote = result.totalRecordsNote
      await this.handleNoteContent();
    }
  }

  onChangeProject(projectId: string) {
    this.router.navigate(['/project/milestone', { 'projectId': projectId }]);
  }
}

function CompareDateNow(date1: Date) {
  let today = new Date();
  let leftDate = date1.setHours(0, 0, 0, 0);
  let rightDate = today.setHours(0, 0, 0, 0);

  return leftDate <= rightDate;
}
