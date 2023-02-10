import { Component, OnInit, ElementRef, HostListener, ViewChild, Renderer2 } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import * as $ from 'jquery';
import { Location } from '@angular/common';
import { NoteModel } from '../../../shared/models/note.model';

import { MessageService, ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission'
import { ProjectService } from '../../services/project.service';
import { ProjectModel } from '../../models/project.model';
import { TaskModel } from '../../models/ProjectTask.model';

import { ProjectMilestoneModel } from '../../models/milestone.model';
import { Table } from 'primeng';


@Component({
  selector: 'app-add-or-remove-task-milestone-dialog',
  templateUrl: './add-or-remove-task-milestone-dialog.component.html',
  styleUrls: ['./add-or-remove-task-milestone-dialog.component.css']
})
export class AddOrRemoveTaskMilestoneDialogComponent implements OnInit {
  @ViewChild('myTable') myTable: Table;
  auth = JSON.parse(localStorage.getItem("auth"));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();
  actionAdd: boolean = true;
  fixed: boolean = false;
  withFiexd: string = "";
  isOpenNotifiError: boolean = false;
  isInvalidForm: boolean = false;

  /*BIẾN LƯU DỮ LIỆU TRẢ VỀ*/
  listTask: Array<TaskModel> = [];
  /*END*/

  projectMilestoneId: string = this.emptyGuid;
  projectId: string = this.emptyGuid;
  type: number = 0;

  /*TABLE*/
  selectedColumns: any[];
  cols: any[];
  selectedTask: Array<TaskModel> = []
  isGlobalFilter: string = '';

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    public builder: FormBuilder,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private renderer: Renderer2,
    private projectService: ProjectService,
  ) {
    this.translate.setDefaultLang('vi');
    this.projectMilestoneId = this.config.data.projectMilestoneId;
    this.projectId = this.config.data.projectId;
    this.type = this.config.data.type;
  }

  ngOnInit(): void {
    this.initTable();
    this.getMasterData();
  }

  initTable() {
    this.cols = [
      { field: 'taskCode', header: 'Mã #', textAlign: 'center', display: 'table-cell', width: '80px', excelWidth: 20, },
      { field: 'taskName', header: 'Tên công việc', textAlign: 'left', display: 'table-cell', width: '200px', excelWidth: 30, },
      { field: 'taskTypeName', header: 'Loại công việc', textAlign: 'left', display: 'table-cell', width: '150px', excelWidth: 25, },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '150px', excelWidth: 25, },
      { field: 'priorityName', header: 'Mức độ ưu tiên', textAlign: 'center', display: 'table-cell', width: '150px', excelWidth: 20, },
      { field: 'planEndTimeStr', header: 'Ngày dự kiến hết hạn', textAlign: 'right', display: 'table-cell', width: '180px', excelWidth: 25, },
    ];
    this.selectedColumns = this.cols;
  }

  getMasterData() {
    this.loading = true;
    this.projectService.getMasterDataAddOrRemoveTaskToMilestone(this.projectMilestoneId, this.projectId, this.type, this.auth.UserId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listTask = result.listTask;
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  cancel() {
    this.ref.close();
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ key: "popup", severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  save(type: boolean) {
    if (this.selectedTask.length == 0) {
      this.clearToast();
      this.showToast('error', 'Thông báo', 'Chưa có bản ghi nào được chọn!');
    } else {
      let listTaskId = this.selectedTask.map(c => c.taskId);
      this.loading = true;
      this.projectService.addOrRemoveTaskMilestone(listTaskId, this.projectMilestoneId, this.type, this.auth.UserId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          if (type) {
            this.ref.close(true);
          } else {
            this.selectedTask = [];
            this.getMasterData();
          }
        } else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      });
    }
  }
}
