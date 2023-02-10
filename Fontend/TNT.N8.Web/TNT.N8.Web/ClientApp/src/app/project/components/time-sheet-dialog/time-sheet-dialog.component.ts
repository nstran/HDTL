import { Component, OnInit, ElementRef, HostListener, ViewChild, Renderer2 } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import * as $ from 'jquery';


import { MessageService, ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { NoteService } from '../../../shared/services/note.service';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission'
import { ProjectService } from '../../services/project.service';
import { ProjectModel } from '../../models/project.model';
import { TaskModel } from '../../models/ProjectTask.model';
import { DatePipe, DecimalPipe } from '@angular/common';
import { TimeSheet, TimeSheetDetail } from '../../models/timeSheet.model';

class Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
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

@Component({
  selector: 'app-time-sheet-dialog',
  templateUrl: './time-sheet-dialog.component.html',
  styleUrls: ['./time-sheet-dialog.component.css'],
  providers: [DatePipe, DecimalPipe]
})
export class TimeSheetDialogComponent implements OnInit {
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
  projectId: string = '00000000-0000-0000-0000-000000000000';
  taskId: string = '00000000-0000-0000-0000-000000000000';
  timeSheetId: string = null;
  /*START: FORM CONTROL*/
  createTimeSheetForm: FormGroup;

  weekControl: FormControl; // Tuần
  statusControl: FormControl; // Trạng thái phê duyệt
  personInChargedControl: FormControl; // Người phụ trách
  projectControl: FormControl; // Dự án
  taskControl: FormControl; // Công việc
  timeTypeControl: FormControl; // Loại thời gian
  noteControl: FormControl; // Ghi chú
  /*END: FORM CONTROL*/

  /*START: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  listAllTask: Array<TaskModel> = [];
  listTask: Array<TaskModel> = [];
  listTimeType: Array<Category> = [];
  listStatus: Array<Category> = [];
  project: ProjectModel;
  employee: any;
  fromDate: Date;
  toDate: Date;
  listTimeSheet: Array<TimeSheet> = [];
  /*END: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/

  totalSpentHour: number = 0;
  listTimeSheetDetail: Array<TimeSheetDetail> = [];

  /*NOTE*/
  noteContent: string = '';
  noteId: string;
  listUpdateNoteDocument: Array<NoteDocument> = [];
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  /*END*/

  noteHistory: Array<Note> = [];
  isNotEdit: boolean = false;
  isShowSendApproval: boolean = false;
  isShowApproval: boolean = false;
  isCreateBy: boolean = false;
  timeSheet: TimeSheet;
  isAdminApproval: boolean = false;

  /*TỪ CHỐI*/
  displayReject: boolean = false;
  descriptionReject: string = '';

  /*PHÊ DUYỆT*/
  displayApproval: boolean = false;
  descriptionApproval: string = '';

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
    private translate: TranslateService,
    private getPermission: GetPermission,
    public builder: FormBuilder,
    private el: ElementRef,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private renderer: Renderer2,
    private projectService: ProjectService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private datePipe: DatePipe,
  ) {
    this.translate.setDefaultLang('vi');
    this.projectId = this.config.data.projectId;
    this.taskId = this.config.data.taskId;
    this.timeSheetId = this.config.data.timeSheetId;
  }

  ngOnInit(): void {
    this.initForm();
    this.getMasterData();
  }

  initForm() {
    this.weekControl = new FormControl('');
    this.statusControl = new FormControl(null);
    this.personInChargedControl = new FormControl('');
    this.projectControl = new FormControl('');
    this.taskControl = new FormControl(null, [Validators.required]);
    this.timeTypeControl = new FormControl(null, [Validators.required]);
    this.noteControl = new FormControl('');

    this.createTimeSheetForm = new FormGroup({
      weekControl: this.weekControl,
      statusControl: this.statusControl,
      personInChargedControl: this.personInChargedControl,
      projectControl: this.projectControl,
      taskControl: this.taskControl,
      timeTypeControl: this.timeTypeControl,
      noteControl: this.noteControl
    });
  }

  getMasterData() {
    this.loading = true;
    this.projectService.getMasterDataTimeSheet(this.projectId, this.auth.UserId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode === 200) {
        this.listAllTask = result.listTask;
        this.listTask = this.listAllTask.filter(c => c.isCreate == true);
        this.project = result.project;
        this.listTimeType = result.listTimeType;
        this.employee = result.employee;
        this.fromDate = result.fromDate;
        this.toDate = result.toDate;
        this.listTimeSheet = result.listTimeSheet;
        this.listStatus = result.listStatus;
        this.setDefaultData();
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  setDefaultData() {
    this.personInChargedControl.setValue(`${this.employee.employeeName}`);
    this.projectControl.setValue(`${this.project.projectCode} - ${this.project.projectName}`);

    let weekName = `Thứ 2 ngày ${this.datePipe.transform(this.fromDate, 'dd/MM/yyyy')} - Chủ nhật ngày ${this.datePipe.transform(this.toDate, 'dd/MM/yyyy')}`;
    this.weekControl.setValue(weekName);

    if (this.timeSheetId) {
      this.setDataTimeSheetFollowId();
    } else {
      this.setDataTimeSheet();
    }
  }

  setDataTimeSheetFollowId() {
    let timeSheet = this.listTimeSheet.find(c => c.timeSheetId == this.timeSheetId);
    if (timeSheet) {
      this.isCreateBy = true;
      this.timeSheet = timeSheet;
      this.fromDate = timeSheet.fromDate;
      this.toDate = timeSheet.toDate;
      let weekName = `Thứ 2 ngày ${this.datePipe.transform(this.fromDate, 'dd/MM/yyyy')} - Chủ nhật ngày ${this.datePipe.transform(this.toDate, 'dd/MM/yyyy')}`;
      this.weekControl.setValue(weekName);

      let task = this.listAllTask.find(c => c.taskId == timeSheet.taskId);
      this.taskControl.setValue(task);

      let status = this.listStatus.find(c => c.categoryId == timeSheet.status);
      this.statusControl.setValue(status);

      let timeType = this.listTimeType.find(c => c.categoryId == timeSheet.timeType);
      this.timeTypeControl.setValue(timeType);

      this.listTimeSheetDetail = timeSheet.listTimeSheetDetail;
      this.listTimeSheetDetail.forEach((item, index) => {
        if (index + 2 == 8) {
          item.dayOfWeek = `CN`;
        } else {
          item.dayOfWeek = `Thứ ${index + 2}`;
        }
      });
      this.totalSpentHour = timeSheet.spentHour;
      this.noteControl.setValue(timeSheet.note);

      this.noteHistory = timeSheet.listNote;
      this.handleNoteContent();

      if (timeSheet.statusCode == "NHAP" || timeSheet.statusCode == "TCKB") {
        this.isNotEdit = false;
        this.isShowSendApproval = true;
        this.isShowApproval = false;
      } else if (timeSheet.statusCode == "GPD") {
        this.isNotEdit = true;
        this.isShowSendApproval = false;
        this.isShowApproval = true;
      } else {
        this.isNotEdit = true;
        this.isShowSendApproval = false;
        this.isShowApproval = false;
      }
      // Nếu như là quản lý vào xem chi tiết - ko có quyền sửa, gửi phê duyệt
      this.isAdminApproval = timeSheet.createdById != this.auth.UserId;

    }
  }

  setDataTimeSheet() {
    //Ở màn chi tiết công việc sẽ lấy timesheet tại tuần hiện tại (Mỗi một tuần sẽ tương ứng vs 1 bản timesheet)
    let timeSheet = this.listTimeSheet.find(c => (new Date(c.fromDate).setHours(0, 0, 0, 0) == new Date(this.fromDate).setHours(0, 0, 0, 0)) &&
      (new Date(c.toDate).setHours(0, 0, 0, 0) == new Date(this.toDate).setHours(0, 0, 0, 0)) && c.taskId == this.taskId && c.personInChargeId == this.employee.employeeId);
    if (timeSheet) {
      this.isCreateBy = true;
      this.timeSheet = timeSheet;
      this.timeSheetId = timeSheet.timeSheetId;

      let task = this.listTask.find(c => c.taskId == timeSheet.taskId);
      this.taskControl.setValue(task);

      let status = this.listStatus.find(c => c.categoryId == timeSheet.status);
      this.statusControl.setValue(status);

      let timeType = this.listTimeType.find(c => c.categoryId == timeSheet.timeType);
      this.timeTypeControl.setValue(timeType);

      this.listTimeSheetDetail = timeSheet.listTimeSheetDetail;
      this.listTimeSheetDetail.forEach((item, index) => {
        if (index + 2 == 8) {
          item.dayOfWeek = `CN`;
        } else {
          item.dayOfWeek = `Thứ ${index + 2}`;
        }
      });
      this.totalSpentHour = timeSheet.spentHour;
      this.noteControl.setValue(timeSheet.note);

      this.noteHistory = timeSheet.listNote;
      this.handleNoteContent();

      if (timeSheet.statusCode == "NHAP" || timeSheet.statusCode == "TCKB") {
        this.isNotEdit = false;
        this.isShowSendApproval = true;
        this.isShowApproval = false;
      } else if (timeSheet.statusCode == "GPD") {
        this.isNotEdit = true;
        this.isShowSendApproval = false;
        this.isShowApproval = true;
      } else {
        this.isNotEdit = true;
        this.isShowSendApproval = false;
        this.isShowApproval = false;
      }

      // Nếu như là quản lý vào xem chi tiết - ko có quyền sửa, gửi phê duyệt
      this.isAdminApproval = timeSheet.createdById != this.auth.UserId;
    } else {
      this.isCreateBy = false;
      this.timeSheet = null;
      this.timeSheetId = '00000000-0000-0000-0000-000000000000';
      let task = this.listTask.find(c => c.taskId == this.taskId);
      if (task) {
        this.taskControl.setValue(task);
      }
      let nhapStatus = this.listStatus.find(c => c.categoryCode == "NHAP");
      this.statusControl.setValue(nhapStatus);

      this.timeTypeControl.setValue(null);
      this.settingDayOfWeek(this.fromDate, this.toDate);

      this.totalSpentHour = 0;
      this.noteControl.setValue("");

      this.isNotEdit = false;
      this.isShowSendApproval = false;
      this.isShowApproval = false;

      this.noteHistory = [];
    }
  }

  settingDayOfWeek(fromDate: Date, toDate: Date) {
    this.listTimeSheetDetail = [];
    let monday = new TimeSheetDetail("Thứ 2", fromDate);
    let tuesday = new TimeSheetDetail("Thứ 3", addDays(fromDate, 1));
    let wednesday = new TimeSheetDetail("Thứ 4", addDays(fromDate, 2));
    let thursday = new TimeSheetDetail("Thứ 5", addDays(fromDate, 3));
    let friday = new TimeSheetDetail("Thứ 6", addDays(fromDate, 4));
    let saturday = new TimeSheetDetail("Thứ 7", addDays(fromDate, 5));
    let sunday = new TimeSheetDetail("CN", toDate);
    this.listTimeSheetDetail.push(monday);
    this.listTimeSheetDetail.push(tuesday);
    this.listTimeSheetDetail.push(wednesday);
    this.listTimeSheetDetail.push(thursday);
    this.listTimeSheetDetail.push(friday);
    this.listTimeSheetDetail.push(saturday);
    this.listTimeSheetDetail.push(sunday);
  }

  save() {
    if (!this.createTimeSheetForm.valid) {
      Object.keys(this.createTimeSheetForm.controls).forEach(key => {
        if (!this.createTimeSheetForm.controls[key].valid) {
          this.createTimeSheetForm.controls[key].markAsTouched();
        }
      });
    } else {
      let timeSheet = this.mappingDataFromFormToModel();
      this.loading = true;
      this.projectService.createOrUpdateTimeSheet(timeSheet, this.auth.UserId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode === 200) {
          this.showToast('success', 'Thông báo', 'Khai báo thành công');
          this.timeSheetId = result.timeSheetId;
          this.getMasterData();
        } else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      });
    }
  }

  mappingDataFromFormToModel(): TimeSheet {
    let timeSheet = new TimeSheet();
    if (this.timeSheetId) {
      timeSheet.timeSheetId = this.timeSheetId;
    }
    timeSheet.taskId = this.taskControl.value ? this.taskControl.value.taskId : this.emptyGuid;
    timeSheet.fromDate = this.fromDate;
    timeSheet.toDate = this.toDate;
    timeSheet.status = this.statusControl.value ? this.statusControl.value.categoryId : null;
    timeSheet.spentHour = this.totalSpentHour;
    timeSheet.timeType = this.timeTypeControl.value ? this.timeTypeControl.value.categoryId : null;
    timeSheet.note = this.noteControl.value ? this.noteControl.value : null;
    timeSheet.personInChargeId = this.employee.employeeId;
    timeSheet.listTimeSheetDetail = this.listTimeSheetDetail;
    return timeSheet;
  }

  cancel() {
    this.ref.close(false);
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  changeSpentHour(timeSheetDetail: TimeSheetDetail) {
    if (!timeSheetDetail.spentHour || ParseStringToFloat(timeSheetDetail.spentHour.toString()) > 24) {
      timeSheetDetail.spentHour = 0;
    }
    this.totalSpentHour = 0;
    this.listTimeSheetDetail.forEach(item => {
      // Không tính trạng thái nh
      if(item.status != 0){
        let hour = ParseStringToFloat(item.spentHour.toString());
        this.totalSpentHour += hour;
      }
    });
  }

  nextWeek() {
    this.fromDate = addDays(this.fromDate, 7);
    this.toDate = addDays(this.toDate, 7);
    let weekName = `Thứ 2 ngày ${this.datePipe.transform(this.fromDate, 'dd/MM/yyyy')} - Chủ nhật ngày ${this.datePipe.transform(this.toDate, 'dd/MM/yyyy')}`;
    this.weekControl.setValue(weekName);
    this.setDataTimeSheet();
  }

  nextDoubleWeek() {
    this.fromDate = addDays(this.fromDate, 14);
    this.toDate = addDays(this.toDate, 14);
    let weekName = `Thứ 2 ngày ${this.datePipe.transform(this.fromDate, 'dd/MM/yyyy')} - Chủ nhật ngày ${this.datePipe.transform(this.toDate, 'dd/MM/yyyy')}`;
    this.weekControl.setValue(weekName);
    this.setDataTimeSheet();
  }

  previousWeek() {
    this.fromDate = addDays(this.fromDate, -7);
    this.toDate = addDays(this.toDate, -7);
    let weekName = `Thứ 2 ngày ${this.datePipe.transform(this.fromDate, 'dd/MM/yyyy')} - Chủ nhật ngày ${this.datePipe.transform(this.toDate, 'dd/MM/yyyy')}`;
    this.weekControl.setValue(weekName);
    this.setDataTimeSheet();
  }

  previousDoubleWeek() {
    this.fromDate = addDays(this.fromDate, -14);
    this.toDate = addDays(this.toDate, -14);
    let weekName = `Thứ 2 ngày ${this.datePipe.transform(this.fromDate, 'dd/MM/yyyy')} - Chủ nhật ngày ${this.datePipe.transform(this.toDate, 'dd/MM/yyyy')}`;
    this.weekControl.setValue(weekName);
    this.setDataTimeSheet();
  }

  changeTask(event: any) {
    if (event.value == null) return;
    this.taskId = event.value.taskId;
    this.setDataTimeSheet();
  }

  /*Kiểm tra noteText > 250 ký tự hoặc noteDocument > 3 thì ẩn đi một phần nội dung*/
  tooLong(note): boolean {
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

  sendApprovalTimeSheet() {
    if (!this.createTimeSheetForm.valid) {
      Object.keys(this.createTimeSheetForm.controls).forEach(key => {
        if (!this.createTimeSheetForm.controls[key].valid) {
          this.createTimeSheetForm.controls[key].markAsTouched();
        }
      });
    } else {
      // lưu thông tin mới nhất

      //Tính tổng thời gian của timesheet khi gửi phê duyệt luôn
      this.totalSpentHour = 0;
      this.listTimeSheetDetail.forEach(item => {
          let hour = ParseStringToFloat(item.spentHour.toString());
          this.totalSpentHour += hour;
      });

      let timeSheet = this.mappingDataFromFormToModel();
      this.loading = true;
      this.projectService.createOrUpdateTimeSheet(timeSheet, this.auth.UserId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode === 200) {
          this.ref.close(true);
          //tiến hành gửi phê duyệt
          let listTimeSheetSelectedId: Array<string> = [result.timeSheetId];
          this.loading = true;
          this.projectService.updateStatusTimeSheet(listTimeSheetSelectedId, 'SEND_APPROVAL', '', this.auth.UserId, timeSheet).subscribe(response => {
            let result: any = response;
            this.loading = false;
            if (result.statusCode == 200) {
              this.clearToast();
              this.showToast('success', 'Thông báo', "Gửi phê duyệt thành công.");
              this.ref.close(true);
            } else {
              this.clearToast();
              this.showToast('error', 'Thông báo', "Gửi phê duyệt thất bại!");
            }
          });
        } else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      });
    }
  }

  approvalOrRejectTimeSheet(type: string) {
    if (type == "APPROVAL") {
      this.displayApproval = true;
    } else if (type == "REJECT") {
      this.displayReject = true;
    }
  }

  approvalSingleTimeSheet() {
    let timeSheet = this.mappingDataFromFormToModel();
    let listTimeSheetSelectedId: Array<string> = [this.timeSheet.timeSheetId]
    this.loading = true;
    this.projectService.updateStatusTimeSheet(listTimeSheetSelectedId, 'APPROVAL', this.descriptionApproval, this.auth.UserId,timeSheet).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.clearToast();
        this.showToast('success', 'Thông báo', "Phê duyệt khai báo thành công.");
        this.ref.close(true);
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', "Phê duyệt khai báo thất bại!");
      }
    });
  }

  rejectSingleTimeSheet() {
    let timeSheet = this.mappingDataFromFormToModel();
    let listTimeSheetSelectedId: Array<string> = [this.timeSheet.timeSheetId]
    this.loading = true;
    this.projectService.updateStatusTimeSheet(listTimeSheetSelectedId, 'REJECT', this.descriptionReject, this.auth.UserId,timeSheet).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.clearToast();
        this.showToast('success', 'Thông báo', "Từ chối khai báo thành công.");
        this.ref.close(true);
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', "Từ chối khai báo thất bại!");
      }
    });
  }
}

function addDays(dates: Date, days: number) {
  const date = new Date(dates);
  date.setDate(date.getDate() + days);
  return date;
};

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
