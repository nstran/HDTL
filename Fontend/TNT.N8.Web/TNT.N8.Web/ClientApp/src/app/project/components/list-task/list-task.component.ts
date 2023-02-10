import { Component, OnInit, ViewChild, ElementRef, HostListener, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission';
import { SelectItem, SortEvent } from 'primeng/api';
import { MenuItem } from "primeng/primeng";
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DialogService } from 'primeng/dynamicdialog';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { DatePipe, DecimalPipe, formatNumber } from '@angular/common';
import { TaskModel } from '../../models/ProjectTask.model';
import { ReSearchService } from '../../../services/re-search.service';
import { ProjectService } from '../../services/project.service';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { ProjectModel } from '../../models/project.model';
import { ListTaskSearch } from '../../../shared/models/re-search/list-task.search.model';
import { SetIconTaskType } from '../../setIcon/setIcon';

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
}

class ItemStatus {
  label: string;
  value: string;
  isActive: boolean;
  isComplete: boolean;

  constructor() {
    this.isActive = false;
    this.isComplete = false;
  }
}

class Employee {
  public employeeId: string;
  public employeeName: string;
  public employeeCode: string;
  public active: boolean;
}

class Priority {
  label: string;
  value: number;
}

class InforExportExcel {
  companyName: string;
  address: string;
  phone: string;
  website: string;
  email: string;
  textTotalMoney: string
}

class SearchTaskModel {
  public ListTaskTypeId: Array<string>;
  public ListStatusId: Array<string>;
  public ListPersonInChargedId: Array<string>;
  public listCreateById: Array<string>;
  public ListPriority: Array<number>;
  public FromDate: Date;
  public ToDate: Date;
  public SelectedOption: string;
  public SelectedOptionStatus: string;
  public UserId: string;

  constructor() {
    this.ListTaskTypeId = [];
    this.ListStatusId = [];
    this.ListPersonInChargedId = [];
    this.listCreateById = [];
    this.ListPriority = [];
  }
}

@Component({
  selector: 'app-list-task',
  templateUrl: './list-task.component.html',
  styleUrls: ['./list-task.component.css'],
  providers: [DialogService, DatePipe, DecimalPipe]
})
export class ListTaskComponent implements OnInit {
  @ViewChild('myTable') myTable: Table;
  auth = JSON.parse(localStorage.getItem("auth"));
  filterGlobal: string;
  innerWidth: number = 0; //number window size first
  loading: boolean = false;
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  customerCode: string = '';
  startDate: Date = new Date();
  minYear: number = 2015;
  currentYear: number = (new Date()).getFullYear();
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  isManager: boolean = null;
  isGlobalFilter: string = '';
  actions: MenuItem[] = [];
  projectId: string = '00000000-0000-0000-0000-000000000000';
  selectedTask: Array<TaskModel> = [];
  today: Date = new Date();
  isShow: boolean = true;

  listProjectTitle: Array<Project> = [];
  selectedProject: Project = null;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  paramUrl: any;
  inforExportExcel: InforExportExcel;

  //START: BIẾN ĐIỀU KIỆN
  fixed: boolean = false;
  withFiexd: string = "";
  //END : BIẾN ĐIỀU KIỆN

  /* Action*/
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  actionDelete: boolean = true;
  actionEdit: boolean = true;
  /*END*/

  /*TABLE*/
  selectedColumns: any[];
  cols: any[];

  /*START: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  listTaskType: Array<Category> = [];
  listStatus: Array<Category> = [];
  listPersionInCharged: Array<Employee> = [];
  listEmployeeCreated: Array<Employee> = [];
  employee: Employee;
  listPriority: Array<Priority> = [
    { label: "Thấp", value: 0 },
    { label: "Trung bình", value: 1 },
    { label: "Cao", value: 2 },
  ]
  listTask: Array<TaskModel> = [];
  numberTaskNew: number = 0;
  numberTaskDL: number = 0;
  numberTaskHT: number = 0;
  numberTaskClose: number = 0;
  numberTotalTask: number = 0;
  project: ProjectModel = null;
  totalPercent: number = 0; // Tiến độ dự án
  totalEstimateTime: string = '0';
  isContainResource: boolean = false;
  rowGroupMetadata: any;
  /*END: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/

  SelectedTask: Array<TaskModel> = [];
  selectedOption: string = "all";
  stateOptions: SelectItem[];
  stateOptionStatus: Array<ItemStatus> = [
    { label: `Tất cả(${this.numberTotalTask})`, value: "TEMP", isActive: false, isComplete: false },
    { label: `Mới(${this.numberTaskNew})`, value: "NEW", isActive: false, isComplete: false },
    { label: `Đang thực hiện(${this.numberTaskDL})`, value: "DL", isActive: false, isComplete: false },
    { label: `Hoàn thành(${this.numberTaskHT})`, value: "HT", isActive: false, isComplete: false },
    { label: `Đóng(${this.numberTaskClose})`, value: "CLOSE", isActive: false, isComplete: false }
  ];
  selectedOptionStatus: ItemStatus;
  /*SEARCH VALUE*/
  selectedStatus: Array<Category> = [];
  selectedTaskType: Array<Category> = [];
  selectedPriority: Array<Priority> = [];
  selectedPic: Array<Employee> = [];
  selectedCreate: Array<Employee> = [];
  fromDate: Date = null;
  toDate: Date = null;

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
    private setIconTaskType: SetIconTaskType
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit(): void {
    this.initTable();
    this.route.params.subscribe(params => {
      this.projectId = params['projectId'];
      this.stateOptions = [
        { label: "Tất cả công việc", value: "all" },
        { label: "Công việc của tôi", value: "me" },
        { label: "Công việc quá hạn", value: "overdue" },
        { label: "Chưa được giao", value: "notassign" }
      ];
      this.getMasterData();
    });
  }

  ngAfterViewInit() {
    this.ref.detectChanges();
  }

  initTable() {
    this.cols = [
      { field: 'taskCode', header: 'Mã #', textAlign: 'center', display: 'table-cell', width: '140px', excelWidth: 20, },
      { field: 'taskName', header: 'Tên công việc', textAlign: 'left', display: 'table-cell', width: '200px', excelWidth: 30, },
      { field: 'taskTypeName', header: 'Loại công việc', textAlign: 'left', display: 'table-cell', width: '150px', excelWidth: 25, },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '120px', excelWidth: 25, },
      { field: 'priorityName', header: 'Mức độ ưu tiên', textAlign: 'center', display: 'table-cell', width: '160px', excelWidth: 20, },
      { field: 'persionInChargedName', header: 'Người phụ trách', textAlign: 'left', display: 'table-cell', width: '160px', excelWidth: 30, },
      { field: 'planEndTimeStr', header: 'Ngày dự kiến hết hạn', textAlign: 'right', display: 'table-cell', width: '180px', excelWidth: 25, },
      { field: 'actualEndTimeStr', header: 'Ngày kết thúc thực tế', textAlign: 'right', display: 'table-cell', width: '180px', excelWidth: 25, },
    ];
    this.selectedColumns = this.cols;
  }

  getMasterData() {
    this.loading = true;
    this.projectService.getMasterDataSearchTask(this.projectId, this.auth.UserId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode === 200) {
        this.listProjectTitle = result.listProject;
        this.listProjectTitle.forEach(item => {
          item.projectCodeName = `[${item.projectCode}]: ${item.projectName}`;
        });
        this.selectedProject = this.listProjectTitle.find(c => c.projectId == this.projectId);
        this.listStatus = result.listStatus;
        this.listTaskType = this.setIconTaskType.setIconTaskType(result.listTaskType);
        this.listPersionInCharged = result.listPersionInCharged;
        this.inforExportExcel = result.inforExportExcel;
        this.employee = result.employee;
        this.project = result.project;
        this.totalPercent = result.projectTaskComplete.toFixed(2);
        this.totalEstimateTime = result.totalEstimateHour;
        this.isContainResource = result.isContainResource;
        let listTaskSearch: ListTaskSearch = JSON.parse(localStorage.getItem("listTaskSearch"));
        if (listTaskSearch) {
          this.mapDataToNgModel(listTaskSearch);
        }
        this.searchTask();
        this.setDataOption();
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  searchTask() {
    this.resetTable();
    let searchModel = this.mappingNgModelToModel();
    this.saveParameterSearch(searchModel);

    this.loading = true;
    this.projectService.searchTask(this.projectId, searchModel.ListTaskTypeId, searchModel.ListStatusId, searchModel.ListPriority,
      searchModel.ListPersonInChargedId, searchModel.listCreateById, searchModel.FromDate, searchModel.ToDate, this.selectedOption, searchModel.SelectedOptionStatus, this.auth.UserId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.listTask = this.setIconTaskType.setIconTaskTypeInListTask(result.listTask);
          this.numberTotalTask = result.numberTotalTask;
          this.numberTaskNew = result.numberTaskNew;
          this.numberTaskDL = result.numberTaskDL;
          this.numberTaskHT = result.numberTaskHT;
          this.numberTaskClose = result.numberTaskClose;
          this.totalPercent = result.projectTaskComplete.toFixed(2);
          this.totalEstimateTime = result.totalEstimateHour;
          this.updateRowGroupMetaData();
          this.setDataOption();
          if (this.listTask.length == 0) {
            this.clearToast();
            this.showToast('warn', 'Thông báo', "Không tìm thấy bản ghi nào!");
          }
        } else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      });
  }

  onSort() {
    this.updateRowGroupMetaData();
  }

  updateRowGroupMetaData() {
    this.rowGroupMetadata = {};
    if (this.listTask) {
      for (let i = 0; i < this.listTask.length; i++) {
        let rowData = this.listTask[i];
        const totalRowObj = {
          index: 0,
          size: 1,
          taskCode: rowData.taskCode,
          taskName: rowData.taskName,
          taskTypeName: rowData.taskTypeName,
          statusName: rowData.statusName,
          priorityName: rowData.priorityName,
          persionInChargedName: rowData.persionInChargedName,
          planEndTimeStr: rowData.planEndTimeStr,
          actualEndTimeStr: rowData.actualEndTimeStr,
        };

        let updateDateStr = rowData.updateDateStr.toString();
        if (i == 0) {
          this.rowGroupMetadata[updateDateStr] = totalRowObj;
        }
        else {
          let previousRowData = this.listTask[i - 1];
          let previousRowGroup = previousRowData.updateDateStr.toString();
          if (updateDateStr === previousRowGroup)
            this.rowGroupMetadata[updateDateStr].size++;
          else
            this.rowGroupMetadata[updateDateStr] = {
              ...totalRowObj,
              index: i,
            };
        }
      }

    }
  }

  mappingNgModelToModel(): SearchTaskModel {
    var searchModel = new SearchTaskModel();
    let listStatusId: Array<string> = [];
    if (this.selectedStatus) {
      listStatusId = this.selectedStatus.map(c => c.categoryId);
    }
    let listTaskTypeId: Array<string> = [];
    if (this.selectedTaskType) {
      listTaskTypeId = this.selectedTaskType.map(c => c.categoryId);
    }
    let listPersonInChargedId: Array<string> = [];
    if (this.selectedPic) {
      listPersonInChargedId = this.selectedPic.map(c => c.employeeId);
    }
    let listCreateById: Array<string> = [];
    if (this.selectedCreate) {
      listCreateById = this.selectedCreate.map(c => c.employeeId);
    }
    let listPriorityValue: Array<number> = [];
    if (this.selectedPriority) {
      listPriorityValue = this.selectedPriority.map(c => c.value);
    }
    let fromDate = this.fromDate;
    if (fromDate) {
      fromDate.setHours(0, 0, 0, 0);
      fromDate = convertToUTCTime(fromDate);
    }
    let toDate = this.toDate;
    if (toDate) {
      toDate.setHours(23, 59, 59, 999);
      toDate = convertToUTCTime(toDate);
    }

    searchModel.ListTaskTypeId = listTaskTypeId;
    searchModel.ListStatusId = listStatusId;
    searchModel.ListPriority = listPriorityValue;
    searchModel.ListPersonInChargedId = listPersonInChargedId;
    searchModel.listCreateById = listCreateById;
    searchModel.FromDate = fromDate;
    searchModel.ToDate = toDate;

    searchModel.SelectedOption = this.selectedOption;
    if (this.selectedOptionStatus) {
      searchModel.SelectedOptionStatus = this.selectedOptionStatus.value;
    }
    else {
      searchModel.SelectedOptionStatus = "TEMP";
    }

    return searchModel;
  }

  mapDataToNgModel(listTaskSearch: ListTaskSearch) {
    this.selectedTaskType = listTaskSearch.listTaskTypeId ? this.listTaskType.filter(c => listTaskSearch.listTaskTypeId.includes(c.categoryId)) : [];
    this.selectedStatus = listTaskSearch.listStatusId ? this.listStatus.filter(c => listTaskSearch.listStatusId.includes(c.categoryId)) : [];
    this.selectedPic = listTaskSearch.listPersonInChargedId ? this.listPersionInCharged.filter(c => listTaskSearch.listPersonInChargedId.includes(c.employeeId)) : [];
    this.selectedCreate = listTaskSearch.listCreatedId ? this.listEmployeeCreated.filter(c => listTaskSearch.listCreatedId.includes(c.employeeId)) : [];
    this.selectedPriority = listTaskSearch.listPriority ? this.listPriority.filter(c => listTaskSearch.listPriority.includes(c.value)) : [];

    this.selectedOption = listTaskSearch.selectedOption;
    this.selectedOptionStatus = this.stateOptionStatus.find(c => c.value == listTaskSearch.selectedOptionStatus);
    this.fromDate = listTaskSearch.fromDate ? new Date(listTaskSearch.fromDate) : null;
    this.toDate = listTaskSearch.toDate ? new Date(listTaskSearch.toDate) : null;
  }

  saveParameterSearch(searchTaskModel: SearchTaskModel) {
    var listTaskSearch = new ListTaskSearch();
    listTaskSearch.listTaskTypeId = searchTaskModel.ListTaskTypeId;
    listTaskSearch.listStatusId = searchTaskModel.ListStatusId;
    listTaskSearch.listPersonInChargedId = searchTaskModel.ListPersonInChargedId;
    listTaskSearch.listCreatedId = searchTaskModel.listCreateById;
    listTaskSearch.listPriority = searchTaskModel.ListPriority;
    listTaskSearch.selectedOption = searchTaskModel.SelectedOption;
    listTaskSearch.selectedOptionStatus = searchTaskModel.SelectedOptionStatus;
    listTaskSearch.fromDate = searchTaskModel.FromDate ? searchTaskModel.FromDate.toISOString().split('T')[0] : null;
    listTaskSearch.toDate = searchTaskModel.ToDate ? searchTaskModel.ToDate.toISOString().split('T')[0] : null;

    this.reSearchService.updatedSearchModel('listTaskSearch', listTaskSearch);
  }

  setDataOption() {
    this.stateOptionStatus = [];
    this.stateOptionStatus = [
      { label: `Tất cả(${this.numberTotalTask})`, value: "TEMP", isActive: false, isComplete: false },
      { label: `Mới(${this.numberTaskNew})`, value: "NEW", isActive: false, isComplete: false },
      { label: `Đang thực hiện(${this.numberTaskDL})`, value: "DL", isActive: false, isComplete: false },
      { label: `Hoàn thành(${this.numberTaskHT})`, value: "HT", isActive: false, isComplete: false },
      { label: `Đóng(${this.numberTaskClose})`, value: "CLOSE", isActive: false, isComplete: false }
    ];
    if (this.selectedOptionStatus) {
      this.stateOptionStatus.forEach(item => {
        item.isActive = false;
        if (item.value === this.selectedOptionStatus.value) {
          item.isActive = true;
        }
      });
    }
    else {
      this.stateOptionStatus[0].isActive = true;
    }
  }

  resetTable() {
    this.filterGlobal = '';
    if (!this.myTable) return;
    this.myTable.sortOrder = 0;
    this.myTable.sortField = '';
    this.myTable.reset();
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  goToCreate() {
    this.router.navigate(['/project/create-project-task']);
  }

  leftColNumber: number = 12;
  rightColNumber: number = 2;
  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 9;
        this.rightColNumber = 3;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

  refreshFilter() {
    this.selectedTaskType = [];
    this.selectedStatus = [];
    this.selectedPic = [];
    this.selectedCreate = [];
    this.selectedPriority = [];
    this.fromDate = null;
    this.toDate = null;
    this.isGlobalFilter = '';

    this.resetTable();
    this.listTask = [];

    this.searchTask();
  }

  dateFieldFormat: string = 'DD/MM/YYYY';
  customSort(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];

      /**Customize sort date */
      if (event.field == 'planEndTime') {
        const date1 = moment(value1, this.dateFieldFormat);
        const date2 = moment(value2, this.dateFieldFormat);

        let result: number = -1;
        if (moment(date2).isBefore(date1, 'day')) { result = 1; }

        return result * event.order;
      }
      /**End */

      let result = null;

      if (value1 == null && value2 != null)
        result = -1;
      else if (value1 != null && value2 == null)
        result = 1;
      else if (value1 == null && value2 == null)
        result = 0;
      else if (typeof value1 === 'string' && typeof value2 === 'string')
        result = value1.localeCompare(value2);
      else
        result = (value1 < value2) ? -1 : (value1 > value2) ? 1 : 0;

      return (event.order * result);
    });
    this.updateRowGroupMetaData();
  }

  goToDetail(taskId: string) {
    this.router.navigate(['/project/detail-project-task', { 'taskId': taskId, 'projectId': this.projectId }]);
  }

  changeActive(item: any) {
    this.selectedOptionStatus = item;
    this.searchTask();
  }

  onFilter(event: any) {
    this.updateRowGroupMetaDataFilter(event.filteredValue);
  }

  updateRowGroupMetaDataFilter(data: Array<TaskModel>) {
    this.rowGroupMetadata = {};
    if (data) {
      for (let i = 0; i < data.length; i++) {
        let rowData = data[i];
        let updateDateStr = rowData.updateDateStr.toString();
        if (i == 0) {
          this.rowGroupMetadata[updateDateStr] = { index: 0, size: 1 };
        }
        else {
          let previousRowData = data[i - 1];
          let previousRowGroup = previousRowData.updateDateStr.toString();
          if (updateDateStr === previousRowGroup)
            this.rowGroupMetadata[updateDateStr].size++;
          else
            this.rowGroupMetadata[updateDateStr] = { index: i, size: 1 };
        }
      }
    }
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
      id: '6', label: 'Xóa', icon: 'pi pi-trash', command: () => {
        this.deleteTask(rowData);
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
        break;
      case "HT":
        this.actions.push(close);
        this.actions.push(update);
        this.actions.push(reOpen);
        break;
      case "CLOSE":
        this.actions.push(update);
        this.actions.push(reOpen);
        this.actions.push(goToMilestone);
        break;
    }
  }

  chooseOption() {
    this.searchTask();
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
        this.searchTask();
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
              this.searchTask();
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
          this.searchTask();
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
            this.searchTask();
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
            this.searchTask();
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
            this.searchTask();
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    });
  }

  openPopupCreateTask() {
    this.router.navigate(['/project/create-project-task', { 'projectId': this.projectId }]);
  }
  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo?.systemValueString;
  }

  exportExcel() {
    let title = 'DANH SÁCH CÔNG VIỆC';
    switch (this.selectedOption) {
      case "all":
        title = `DANH SÁCH CÔNG VIỆC_${this.employee.employeeCode}_${this.employee.employeeName}`;
        break;
      case "me":
        title = `DANH SÁCH CÔNG VIỆC_${this.employee.employeeCode}_${this.employee.employeeName}`;
        break;
      case "overdue":
        title = `DANH SÁCH CÔNG VIỆC QUÁ HẠN_${this.employee.employeeCode}_${this.employee.employeeName}`;
        break;
      case "notassign":
        title = `DANH SÁCH CÔNG VIỆC CHƯA GIAO_${this.employee.employeeCode}_${this.employee.employeeName}`;
        break;
    }

    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('BAO_CAO');

    let imgBase64 = this.getBase64Logo();
    /* Image */
    var imgLogo = workbook.addImage({
      base64: imgBase64,
      extension: 'png',
    });
    worksheet.addImage(imgLogo, {
      tl: { col: 0, row: 0 },
      ext: { width: 155, height: 100 }
    });

    let dataRow1 = [];
    dataRow1[3] = this.inforExportExcel.companyName.toUpperCase();  //Tên công ty
    let row1 = worksheet.addRow(dataRow1);
    row1.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`C${row1.number}:G${row1.number}`);
    row1.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow2 = [];
    dataRow2[3] = 'Địa chỉ: ' + this.inforExportExcel.address;  //Địa chỉ
    let row2 = worksheet.addRow(dataRow2);
    row2.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`C${row2.number}:G${row2.number}`);
    row2.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow3 = [];
    dataRow3[3] = 'Điện thoại: ' + this.inforExportExcel.phone;  //Số điện thoại
    let row3 = worksheet.addRow(dataRow3);
    row3.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`C${row3.number}:G${row3.number}`);
    row3.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow4 = [];
    dataRow4[3] = 'Email: ' + this.inforExportExcel.email;
    let row4 = worksheet.addRow(dataRow4);
    row4.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`C${row4.number}:G${row4.number}`);
    row4.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow5 = [];
    dataRow5[3] = 'Website dịch vụ: ' + this.inforExportExcel.website;  //Địa chỉ website
    let row5 = worksheet.addRow(dataRow5);
    row5.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`C${row5.number}:G${row5.number}`);
    row5.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    worksheet.addRow([]);
    worksheet.addRow([]);
    worksheet.addRow([]);

    /* title */
    let titleRow = worksheet.addRow([title]);
    titleRow.font = { family: 4, size: 16, bold: true };
    titleRow.height = 25;
    titleRow.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A${titleRow.number}:G${titleRow.number}`);

    /* header */
    const header = this.cols.map(e => e.header);
    let headerRow = worksheet.addRow(header);
    headerRow.font = { bold: true };
    header.forEach((item, index) => {
      headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });

    /* data table */
    let data = this.listTask.map((item, index) => {
      let row = [];

      row.push(item.taskCode); /* Mã công việc */
      row.push(item.taskName); /* Tên công việc*/
      row.push(item.taskTypeName); /* Loại công việc*/
      row.push(item.statusName); /* Trạng thái công việc*/
      row.push(item.priorityName); /* mức độ ưu tiên công việc*/
      row.push(item.persionInChargedName); /* Người phục trách công việc*/
      row.push(item.planEndTime ? this.datePipe.transform(item.planEndTime, 'dd/MM/yyyy') : ''); /* Ngày dự kiến kết thúc*/
      return row;
    });

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      this.cols.forEach((item, index) => {
        row.getCell(index + 1).alignment = { vertical: 'bottom', horizontal: item.textAlign, wrapText: true };
        row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      });
    });

    /* set width */
    this.cols.forEach((item, index) => {
      worksheet.getColumn(index + 1).width = item.excelWidth;
    });

    /* ký tên */

    worksheet.addRow([]);
    worksheet.addRow([]);

    let footer1 = [];
    footer1[5] = 'Ngày.....tháng.....năm..........';
    let rowFooter1 = worksheet.addRow(footer1);
    worksheet.mergeCells(`E${rowFooter1.number}:F${rowFooter1.number}`);
    rowFooter1.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    this.exportToExel(workbook, title);
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    });
  }

  showProjectInfor() {
    this.isShow = !this.isShow;
    if (this.isShow) {
      window.scrollTo(0, 0)
    }
  }

  onChangeProject(projectId: string) {
    this.router.navigate(['/project/list-project-task', { 'projectId': projectId }]);
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function CompareDateNow(date1: Date) {
  let today = new Date();
  let leftDate = date1.setHours(0, 0, 0, 0);
  let rightDate = today.setHours(0, 0, 0, 0);

  return leftDate <= rightDate;
}
