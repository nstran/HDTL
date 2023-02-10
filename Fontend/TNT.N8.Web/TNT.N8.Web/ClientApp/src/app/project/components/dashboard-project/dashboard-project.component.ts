import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission';
import { Chart } from 'angular-highcharts';
import * as Highcharts from 'highcharts/highstock';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DialogService } from 'primeng/dynamicdialog';
import { DatePipe, DecimalPipe } from '@angular/common';
import { TaskModel } from '../../models/ProjectTask.model';
import { ReSearchService } from '../../../services/re-search.service';
import { ProjectService } from '../../services/project.service';
import 'moment/locale/pt-br';
import { ProjectModel } from '../../models/project.model';
import { SetIconTaskType } from '../../setIcon/setIcon';

// Full canlendar
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { FullCalendar } from 'primeng';


class dataChart {
  name: string;
  y: number;
  color: string;
}

class ChooseTime {
  label: string;
  value: string;
}

class ChartTaskFollowStatus {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  countTask: number;
  color: string;

  percentValue: number;
}

class ChartTaskFollowTime {
  timeCode: string;
  timeName: string;
  countTask: number;
  color: string;

  percentValue: number;
}

class CharFollowTaskType {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  countTask: number;
}

class ChartTaskFollowResource {
  resouceId: string;
  employeeId: string;
  employeeCodeName: string;
  countTaskComplete: number;
  countTaskNotComplete: number;
  total: number;
}

class ChartTimeFollowResource {
  employeeId: string;
  employeeCodeName: string;
  totalHour: number;
  hourUsed: number;
  hourNotUsed: number;
}

class ChartEvn {
  dateStr: string;
  pv: number;
  ac: number;
  ev: number;
}

class ChartProjectFollowResource {
  projectCode: string;
  projectName: string;
  color: string;
  allowcation: number;
}

class PerformanceCost {
  dateStr: string;
  cpi: number;
  spi: number;
  maxYAxis: number;
}

class Calendar {
  id: string;
  title: string;
  start: Date;
  end: Date;
  backgroundColor: string;
  allDay: boolean;
}

class ChartBudget {
  budgetName: string;
  budgetValue: number;
}

@Component({
  selector: 'app-dashboard-project',
  templateUrl: './dashboard-project.component.html',
  styleUrls: ['./dashboard-project.component.css'],
  providers: [DialogService, DatePipe, DecimalPipe]
})
export class DashboardProjectComponent implements OnInit {
  @ViewChild('myTable') myTable: Table;
  @ViewChild('calendar') calendar: FullCalendar;
  auth = JSON.parse(localStorage.getItem("auth"));

  filterGlobal: string;
  innerWidth: number = 0; //number window size first
  loading: boolean = false;
  startDate: Date = new Date();
  minYear: number = 2015;
  currentYear: number = (new Date()).getFullYear();
  currentTimeString = this.convertTimeToString();

  emptyGuid: string = '00000000-0000-0000-0000-000000000000';

  projectId: string = '00000000-0000-0000-0000-000000000000';

  today: Date = new Date();
  isShow: boolean = true;

  isManager: boolean = null;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  paramUrl: any;

  //START: BIẾN ĐIỀU KIỆN
  fixed: boolean = false;
  withFiexd: string = "";
  //END : BIẾN ĐIỀU KIỆN

  /*START: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  listTaskOverdue: Array<TaskModel> = []; // Danh sách công việc quá hạn - order by : priority => thời gian
  listTaskComplete: Array<TaskModel> = []; // Danh sách công việc cần hoàn thành trong ngày - order by : priority => thời gian

  project: ProjectModel = null;
  listProject: Array<ProjectModel> = [];
  selectedProject: ProjectModel = null;

  totalPercent: number = 0; // Tiến độ dự án
  totalEstimateTime: string = '0';
  totalHourUsed: string = '0';
  totalEE: number = 0;
  isContainResource: boolean = false;

  listTaskFollowTime: Array<ChartTaskFollowTime> = [];
  listProjectFollowResource: Array<ChartProjectFollowResource> = [];
  listTaskFollowStatus: Array<ChartTaskFollowStatus> = [];
  listTaskFollowTaskType: Array<CharFollowTaskType> = [];
  listTaskFollowResource: Array<ChartTaskFollowResource> = [];
  listChartTimeFollowResource: Array<ChartTimeFollowResource> = [];
  listChartBudget: Array<ChartBudget> = [];

  listChartEvn: Array<ChartEvn> = [];
  listPerformanceCost: Array<PerformanceCost> = [];
  /*END: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/

  /*TABLE*/
  selectedColumnsOverdue: any[];
  colsTaskOverdue: any[];

  selectedColumnsTaskCompleteInDay: any[];
  colsTaskCompleteInDay: any[];

  /*Declare chart*/
  doughnutChartStatus: Chart;
  doughnutChartFollowTime: Chart;



  listTime: Array<ChooseTime> = [
    { label: "Ngày", value: "Day" },
    { label: "Tuần", value: "Week" },
    { label: "Tháng", value: "Month" },
    { label: "Năm", value: "Year" },
  ]

  selectedTime: ChooseTime = this.listTime[1];

  // THÔNG TIN LỊCH LÀM VIỆC
  events: Array<Calendar> = [];
  options: any;

  listEmployee: Array<any> = [];
  selectedEmployee: Array<any> = [];
  totalEvents: Array<Calendar> = [];

  listTaskFollowRerouse: Array<TaskModel> = [];
  listAllTask: Array<TaskModel> = [];

  employee: any;
  isReadonly: boolean = false;
  isShowClear: boolean = true;
  listPermissionResourceActive: string = localStorage.getItem("ListPermissionResource");

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

  async ngOnInit() {
    this.initTable();
    this.setOptionCalendar();
    this.route.params.subscribe(async params => {
      this.projectId = params['projectId'];

      let resource = "pro/project/dashboard";
      let permission: any = await this.getPermission.getPermissionLocal(this.listPermissionResourceActive, resource);
      if (permission.status == false) {
        this.router.navigate(['/home']);
      }
      else {

        this.getMasterData();
      }
    });
  }

  initTable() {
    this.colsTaskOverdue = [
      { field: 'taskCode', header: 'Mã #', textAlign: 'center', display: 'table-cell', width: '140px', excelWidth: 20, },
      { field: 'taskName', header: 'Tên công việc', textAlign: 'left', display: 'table-cell', width: '200px', excelWidth: 30, },
      { field: 'taskTypeName', header: 'Loại công việc', textAlign: 'left', display: 'table-cell', width: '150px', excelWidth: 25, },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '120px', excelWidth: 25, },
      { field: 'priorityName', header: 'Mức độ ưu tiên', textAlign: 'center', display: 'table-cell', width: '160px', excelWidth: 20, },
      { field: 'persionInChargedName', header: 'Người phụ trách', textAlign: 'left', display: 'table-cell', width: '160px', excelWidth: 30, },
      { field: 'planEndTimeStr', header: 'Ngày dự kiến hết hạn', textAlign: 'right', display: 'table-cell', width: '180px', excelWidth: 25, },
      { field: 'actualEndTimeStr', header: 'Ngày kết thúc thực tế', textAlign: 'right', display: 'table-cell', width: '180px', excelWidth: 25, },
    ];
    this.selectedColumnsOverdue = this.colsTaskOverdue;

    this.colsTaskCompleteInDay = [
      { field: 'taskCode', header: 'Mã #', textAlign: 'center', display: 'table-cell', width: '140px', excelWidth: 20, },
      { field: 'taskName', header: 'Tên công việc', textAlign: 'left', display: 'table-cell', width: '200px', excelWidth: 30, },
      { field: 'taskTypeName', header: 'Loại công việc', textAlign: 'left', display: 'table-cell', width: '150px', excelWidth: 25, },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '120px', excelWidth: 25, },
      { field: 'priorityName', header: 'Mức độ ưu tiên', textAlign: 'center', display: 'table-cell', width: '160px', excelWidth: 20, },
      { field: 'persionInChargedName', header: 'Người phụ trách', textAlign: 'left', display: 'table-cell', width: '160px', excelWidth: 30, },
      { field: 'planEndTimeStr', header: 'Ngày dự kiến hết hạn', textAlign: 'right', display: 'table-cell', width: '180px', excelWidth: 25, },
    ];

    this.selectedColumnsTaskCompleteInDay = this.colsTaskCompleteInDay;
  }

  getMasterData() {
    this.loading = true;
    this.projectService.getMasterDataCommonDashboardProject(this.projectId, this.auth.UserId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode != 200) {
        this.showMessage('error', result.messageCode);
        return;
      }

      this.isManager = result.isManager;
      this.listTaskOverdue = this.setIconTaskType.setIconTaskTypeInListTask(result.listTaskOverdue);
      this.listTaskComplete = this.setIconTaskType.setIconTaskTypeInListTask(result.listTaskComplete);
      this.project = result.project;
      this.totalEstimateTime = result.totalEstimateHour;
      this.totalHourUsed = result.totalHourUsed;
      this.totalEE = result.totalEE;
      this.totalPercent = result.projectTaskComplete.toFixed(2);
      this.listProject = result.listProject;
      this.selectedProject = this.listProject.find(c => c.projectId == this.projectId);
      this.listAllTask = result.listAllTask;
      this.listEmployee = result.listEmployee;
      this.listTaskFollowRerouse = this.listAllTask;
      this.listChartBudget = result.listChartBudget;

      if (this.listChartBudget.length > 0 && this.isManager) {
        setTimeout(() => {
          this.createBarChartBudget(this.listChartBudget);
        }, 0);
      }

      this.setCalendar();
      // API của quản lý
      if (this.isManager) {
        this.projectService.getDataDashboardProjectFollowManager(this.projectId, this.selectedTime.value, this.auth.UserId).subscribe(response => {
          let result: any = response;
          if (result.statusCode == 200) {
            this.listTaskFollowStatus = result.listTaskFollowStatus ?? [];
            this.listTaskFollowTime = result.listTaskFollowTime ?? [];
            this.listTaskFollowTaskType = result.listTaskFollowTaskType ?? [];
            this.listTaskFollowResource = result.listTaskFollowResource ?? [];
            this.listChartTimeFollowResource = result.listChartTimeFollowResource ?? [];

            setTimeout(() => {
              if (this.listTaskFollowStatus.length > 0) this.createTaskFollowStatusDoughnutChart(this.listTaskFollowStatus);
              if (this.listTaskFollowTime.length > 0) this.createTaskFollowTimeDoughnutChart(this.listTaskFollowTime);
              if (this.listTaskFollowTaskType.length > 0) this.createBarChart(this.listTaskFollowTaskType);
              if (this.listTaskFollowResource.length > 0) this.createBarChartResource(this.listTaskFollowResource);
              if (this.listChartTimeFollowResource.length > 0) this.createBarChartTimeNotUsedOfResource(this.listChartTimeFollowResource);
            }, 0);
          }
        });

        this.projectService.getDataEVNProjectDashboard(this.projectId, this.selectedTime.value, this.auth.UserId).subscribe(response => {
          let result: any = response;
          if (result.statusCode == 200) {
            this.listChartEvn = result.listChartEvn ?? [];
            this.listPerformanceCost = result.listPerformanceCost ?? [];
            setTimeout(() => {
              if (this.listChartEvn.length > 0) this.createLineChartEvn(this.listChartEvn);
              if (this.listPerformanceCost.length > 0) this.createLineChartPerformance(this.listPerformanceCost)
            }, 0);
          }
        });
      }
      // API của nhân viên
      else {
        this.employee = this.listEmployee.find(x => x.employeeId == this.auth.EmployeeId);
        this.isReadonly = true;
        this.isShowClear = false;

        this.projectService.getDataDashboardProjectFollowEmployee(this.projectId, this.selectedTime.value, this.auth.UserId).subscribe(response => {
          let result: any = response;
          if (result.statusCode == 200) {
            this.listTaskFollowTime = result.listTaskFollowTime ?? [];
            this.listProjectFollowResource = result.listProjectFollowResource ?? [];

            setTimeout(() => {
              if (this.listTaskFollowTime.length > 0) this.createTaskFollowTimeDoughnutChart(this.listTaskFollowTime);
              if (this.listProjectFollowResource.length > 0) this.createAuthorizedProjectsBarChart(this.listProjectFollowResource);
            }, 0);
          }
        });
      }
    });
  }

  showMessage(severity: string, detail: string) {
    this.messageService.add({ severity: severity, summary: 'Thông báo', detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  convertTimeToString(): string {
    let now = new Date();
    let day = now.getDay() + 1;
    let stringDay = '';
    let date = now.getDate();
    let stringDate = '';
    let month = now.getMonth() + 1;
    let stringMonth = '';
    let year = now.getFullYear();
    let stringYear = '';

    if (day == 1) {
      stringDay = "Chủ nhật"
    } else {
      stringDay = "Thứ " + day;
    }
    stringDate = "ngày " + date;
    stringMonth = "tháng " + month;
    stringYear = "năm " + year;

    return stringDay + ", " + stringDate + " " + stringMonth + " " + stringYear;
  }

  goToListAllTask() {
    this.router.navigate(['/project/list-project-task', { 'projectId': this.projectId }]);
  }

  goToDetail(taskId: string) {
    this.router.navigate(['/project/detail-project-task', { 'taskId': taskId, 'projectId': this.projectId }]);
  }

  onChangeProject(projectId: string) {
    this.router.navigate(['/project/dashboard', { 'projectId': projectId }]);
  }

  /*START: TẠO CHART CHO PM*/
  // Tạo Doughnut Chart cho tổng quan tình trạng công việc
  createTaskFollowStatusDoughnutChart(array: Array<ChartTaskFollowStatus>) {
    let height = '';
    if (this.innerWidth <= 1370) {
      height = '100%';
    } else {
      height = '70%';
    }

    let data: Array<dataChart> = [];
    array.forEach((e, index) => {
      let item = new dataChart();
      item.name = `${e.categoryName}`;
      item.y = e.countTask;
      item.color = e.color;

      data = [item, ...data];
    });
    let chart = new Chart({
      chart: {
        type: 'pie',
        alignTicks: false,
        height: height
      },
      title: {
        text: '', //text in center
        align: 'center',
        verticalAlign: 'middle',
        style: { 'color': '#6D98E7', 'font-size': '20px', 'line-height': '10px' }
      },
      credits: {
        enabled: false
      },
      tooltip: {
        pointFormat: '{series.name}: {point.percentage:.2f}%'
      },
      series: [{
        name: 'Tỉ lệ',
        innerSize: '50%',
        data: data,
      }],
      plotOptions: {
        pie: {
          allowPointSelect: true,
          cursor: 'pointer',
          dataLabels: {
            enabled: false
          }
        }
      }
    });
    this.doughnutChartStatus = chart;
  }

  // Tạo Doughnut Chart cho tiến trình công việc
  createTaskFollowTimeDoughnutChart(array: Array<ChartTaskFollowTime>) {
    let height = '';
    if (this.innerWidth <= 1370) {
      height = '100%';
    } else {
      height = '70%';
    }

    let data: Array<dataChart> = [];
    array.forEach((e, index) => {
      let item = new dataChart();
      item.name = `${e.timeName}`;
      item.y = e.countTask;
      item.color = e.color;

      data = [item, ...data];
    });
    let chart = new Chart({
      chart: {
        type: 'pie',
        height: height
      },
      title: {
        text: '', //text in center
        align: 'center',
        verticalAlign: 'middle',
        style: { 'color': '#6D98E7', 'font-size': '20px', 'line-height': '10px' }
      },
      credits: {
        enabled: false
      },
      tooltip: {
        pointFormat: '{series.name}: {point.percentage:.2f}%'
      },
      series: [{
        name: 'Tỉ lệ',
        innerSize: '50%',
        data: data,
      }],
      plotOptions: {
        pie: {
          allowPointSelect: true,
          cursor: 'pointer',
          dataLabels: {
            enabled: false
          }
        }
      }
    });
    this.doughnutChartFollowTime = chart;
  }

  // Tạo Bar Chart phân loại công việc
  createBarChart(array: Array<CharFollowTaskType>) {
    let arrayCate: Array<string> = array.map(x => x.categoryName);
    let data: Array<any> = array.map(x => x.countTask);
    Highcharts.setOptions({
      chart: {
        style: {
          fontFamily: 'Inter UI'
        }
      }
    });

    Highcharts.chart('container', {
      chart: {
        type: 'column'
      },
      title: {
        style: {
          display: 'none'
        }
      },

      xAxis: {
        categories: arrayCate,
        crosshair: true,
        title: {
          text: ''
        }
      },
      yAxis: {
        min: 0,
        title: {
          text: '',
        }
      },
      tooltip: {
        headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
        pointFormat: '<tr><td style="color:{series.color};padding:0;font-family:Inter UI">{series.name}: </td>' +
          '<td style="padding:0;font-family:Inter UI"><b> {point.y}</b></td></tr>',
        footerFormat: '</table>',
        shared: true,
        useHTML: true
      },
      plotOptions: {
        column: {
          pointPadding: 0.2,
          borderWidth: 0
        }
      },
      legend: {
        enabled: false
      },
      series: [{
        name: "Công việc",
        data: data,
      }]
    });
  }

  // Tạo Bar Chart tiến độ công việc theo nhân viên
  createBarChartResource(array: Array<ChartTaskFollowResource>) {
    let arrayCate: Array<string> = array.map(x => x.employeeCodeName);
    let dataComplete: Array<any> = array.map(x => x.countTaskComplete);
    let dataNotComplete: Array<any> = array.map(x => x.countTaskNotComplete);

    let yMax = ParseStringToFloat(array[0].total.toString()) + 10;

    Highcharts.setOptions({
      chart: {
        style: {
          fontFamily: 'Inter UI'
        }
      }
    });

    (Highcharts as any).chart('container2', {
      chart: {
        type: 'bar'
      },
      title: {
        style: {
          display: 'none'
        }
      },
      xAxis: {
        type: 'category',
        categories: arrayCate,
        tickLength: 0,
        min: 0,
        max: 3,
        scrollbar: {
          enabled: true
        },
      },
      yAxis: {
        min: 0,
        max: yMax,
        title: {
          text: '',
        },
      },
      colors: ['#d7d7d7', '#71b603'],
      plotOptions: {
        bar: {
          allowPointSelect: false,
          cursor: 'pointer',
          dataLabels: {
            enabled: true,
          }
        },
        series: {
          stacking: 'normal'
        },
      },
      legend: {
        reversed: true
      },
      series: [
        {
          name: "Chưa hoàn thành",
          data: dataNotComplete,
        },
        {
          name: "Hoàn thành",
          data: dataComplete,
        },
      ]
    });
  }

  // Tạo Bar Chart thời gian nhàn rỗi của nhân viên
  createBarChartTimeNotUsedOfResource(array: Array<ChartTimeFollowResource>) {
    let arrayCate: Array<string> = array.map(x => x.employeeCodeName);
    let data: Array<any> = array.map(x => x.hourNotUsed);
    Highcharts.setOptions({
      chart: {
        style: {
          fontFamily: 'Inter UI'
        }
      }
    });

    Highcharts.chart('container5', {
      chart: {
        type: 'column'
      },
      title: {
        style: {
          display: 'none'
        }
      },
      xAxis: {
        categories: arrayCate,
        crosshair: true,
        title: {
          text: ''
        }
      },
      yAxis: {
        min: 0,
        title: {
          text: '',
        }
      },
      tooltip: {
        headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
        pointFormat: '<tr><td style="color:{series.color};padding:0;font-family:Inter UI">{series.name}: </td>' +
          '<td style="padding:0;font-family:Inter UI"><b> {point.y}</b></td></tr>',
        footerFormat: '</table>',
        shared: true,
        useHTML: true
      },
      plotOptions: {
        column: {
          pointPadding: 0.2,
          borderWidth: 0
        }
      },
      legend: {
        enabled: false
      },
      series: [{
        name: "Thời gian rảnh",
        data: data,
      }]
    });
  }

  // Tạo Line chart, đồ thị tiến độ - chi phí theo evn
  createLineChartEvn(array: Array<ChartEvn>) {
    let arrayCate: Array<string> = array.map(x => x.dateStr);
    let lstPV: Array<any> = array.map(x => x.pv);
    let lstAC: Array<any> = array.map(x => x.ac);
    let lstEV: Array<any> = array.map(x => x.ev);

    Highcharts.setOptions({
      chart: {
        style: {
          fontFamily: 'Inter UI'
        }
      }
    });

    Highcharts.chart('container3', {
      title: {
        style: {
          display: 'none'
        }
      },
      xAxis: {
        categories: arrayCate,
        crosshair: true,
        title: {
          text: ''
        }
      },
      yAxis: {
        title: {
          text: '',
        },
      },
      legend: {
        layout: 'horizontal',
        align: 'center',
        verticalAlign: 'bottom'
      },
      plotOptions: {
        series: {
          // label: {
          //   connectorAllowed: false
          // },
          // pointStart: 2010
        }
      },
      series: [{
        name: 'Giá trị dự kiến (PV)',
        data: lstPV
      }, {
        name: 'Chi phí thực tế (AC)',
        data: lstAC
      }, {
        name: 'Giá trị thu được(EV)',
        data: lstEV
      }],
    });
  }

  // Tạo line chart, chỉ số hiệu suất chi phí
  createLineChartPerformance(array: Array<PerformanceCost>) {
    let arrayCate: Array<string> = array.map(x => x.dateStr);
    let lstCPI: Array<any> = array.map(x => x.cpi);
    let lstSPI: Array<any> = array.map(x => x.spi);
    let maxYAxis: number = array.map(x => x.maxYAxis)[0];

    Highcharts.setOptions({
      chart: {
        style: {
          fontFamily: 'Inter UI'
        }
      }
    });

    Highcharts.chart('container4', {
      title: {
        style: {
          display: 'none'
        }
      },
      xAxis: {
        categories: arrayCate,
        crosshair: true,
        title: {
          text: ''
        }
      },
      yAxis: {
        min: 0,
        max: maxYAxis,
        title: {
          text: '',
        },

      },
      legend: {
        layout: 'horizontal',
        align: 'center',
        verticalAlign: 'bottom'
      },
      plotOptions: {
        series: {
          // general options for all series
        },
        line: {
          // shared options for all line series
        }
      },
      series: [{
        name: 'CPI',
        data: lstCPI,
        type: 'line'
      }, {
        name: 'SPI',
        data: lstSPI,
        type: 'line'
      }],
    });
  }

  changeChooseTime(time: ChooseTime) {
    if (this.isManager) {
      this.loading = true;
      this.projectService.getDataDashboardProjectFollowManager(this.projectId, time.value, this.auth.UserId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.listTaskFollowStatus = result.listTaskFollowStatus ?? [];
          this.listTaskFollowTime = result.listTaskFollowTime ?? [];
          this.listTaskFollowTaskType = result.listTaskFollowTaskType ?? [];
          this.listTaskFollowResource = result.listTaskFollowResource ?? [];
          this.listChartTimeFollowResource = result.listChartTimeFollowResource ?? [];

          setTimeout(() => {
            if (this.listTaskFollowStatus.length > 0) this.createTaskFollowStatusDoughnutChart(this.listTaskFollowStatus);
            if (this.listTaskFollowTime.length > 0) this.createTaskFollowTimeDoughnutChart(this.listTaskFollowTime);
            if (this.listTaskFollowTaskType.length > 0) this.createBarChart(this.listTaskFollowTaskType);
            if (this.listTaskFollowResource.length > 0) this.createBarChartResource(this.listTaskFollowResource);
            if (this.listChartTimeFollowResource.length > 0) this.createBarChartTimeNotUsedOfResource(this.listChartTimeFollowResource);
          }, 0);
        }
      });

      this.projectService.getDataEVNProjectDashboard(this.projectId, time.value, this.auth.UserId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.listChartEvn = result.listChartEvn ?? [];
          this.listPerformanceCost = result.listPerformanceCost ?? [];
          setTimeout(() => {
            if (this.listChartEvn.length > 0) this.createLineChartEvn(this.listChartEvn);
            if (this.listPerformanceCost.length > 0) this.createLineChartPerformance(this.listPerformanceCost)
          }, 0);
        }
      });
    }
    else {
      this.employee = this.listEmployee.find(x => x.employeeId == this.auth.EmployeeId);
      this.isReadonly = true;
      this.isShowClear = false;

      this.projectService.getDataDashboardProjectFollowEmployee(this.projectId, this.selectedTime.value, this.auth.UserId).subscribe(response => {
        let result: any = response;
        if (result.statusCode == 200) {
          this.listTaskFollowTime = result.listTaskFollowTime ?? [];
          this.listProjectFollowResource = result.listProjectFollowResource ?? [];

          setTimeout(() => {
            if (this.listTaskFollowTime.length > 0) this.createTaskFollowTimeDoughnutChart(this.listTaskFollowTime);
            if (this.listProjectFollowResource.length > 0) this.createAuthorizedProjectsBarChart(this.listProjectFollowResource);
          }, 0);
        }
      });
    }
  }
  /*END: TẠO CHART CHO PM*/

  /*START: THÔNG TIN LỊCH LÀM VIỆC*/
  setOptionCalendar() {
    this.options = {
      plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
      locale: 'vi',
      defaultDate: new Date(),
      defaultView: 'timeGridWeek',
      eventMinHeight: 50,
      weekends: false,
      // height: 350,
      header: {
        left: 'prev,next',
        center: 'title',
        right: 'dayGridMonth,timeGridWeek,timeGridDay'
      },
      editable: true,
      buttonText: {
        dayGridMonth: 'Tháng',
        timeGridWeek: 'Tuần',
        timeGridDay: 'Ngày',
      },
      eventClick: (event) => {
        this.goToDetail(event.event.id);
      }

    }
  }

  // Tạo Bar Chart cho các dự án đc giao
  createAuthorizedProjectsBarChart(array: Array<ChartProjectFollowResource>) {
    let arrayCate: Array<string> = array.map(x => x.projectCode);
    let data: Array<any> = array.map(x => x.allowcation);
    let total = data.reduce((total, currentValue) => total + currentValue);
    let color = '';

    arrayCate.push('Tổng');
    data.push(total);

    if (total > 100) {
      color = '#E30B0B';
    }
    else if (85 <= total && total <= 100) {
      color = '#70B603';
    }
    else {
      color = '#ffce3b';
    }

    Highcharts.setOptions({
      chart: {
        style: {
          fontFamily: 'Inter UI'
        }
      }
    });

    Highcharts.chart('container7', {
      chart: {
        type: 'column'
      },
      title: {
        style: {
          display: 'none'
        }
      },
      colors: [color],
      xAxis: {
        categories: arrayCate,
        crosshair: true,
        title: {
          text: ''
        }
      },
      yAxis: {
        min: 0,
        title: {
          text: '',
        }
      },
      tooltip: {
        headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
        pointFormat: '<tr><td style="color:{series.color};padding:0;font-family:Inter UI">{series.name}: </td>' +
          '<td style="padding:0;font-family:Inter UI"><b> {point.y}</b></td></tr>',
        footerFormat: '</table>',
        shared: true,
        useHTML: true
      },
      plotOptions: {
        column: {
          pointPadding: 0.2,
          borderWidth: 0
        }
      },
      legend: {
        enabled: false
      },
      series: [{
        name: "% Phân bổ",
        data: data,
      }]
    });
  }

  setCalendar() {
    this.events = [];
    if (this.listTaskFollowRerouse) {
      this.listTaskFollowRerouse.forEach(item => {
        let meeting = new Calendar();
        meeting.id = item.taskId;
        meeting.start = new Date(item.planStartTime);
        meeting.end = new Date(new Date(item.planEndTime).setDate(new Date(item.planEndTime).getDate() + 1));
        meeting.allDay = true;
        meeting.backgroundColor = item.backgroundColorForStatus;
        if (item.includeWeekend) {
          if (item.persionInChargedName) {
            meeting.title = ` ${item.persionInChargedName} - ${item.taskCode} - ${item.taskName}, ${item.estimateHour} giờ, Tính cuối tuần`;
          }
          else {
            meeting.title = ` ${item.taskCode} - ${item.taskName}, ${item.estimateHour} giờ, Tính cuối tuần`;
          }
        }
        else {
          if (item.persionInChargedName) {
            meeting.title = ` ${item.persionInChargedName} - ${item.taskCode} - ${item.taskName}, ${item.estimateHour} giờ`;
          }
          else {
            meeting.title = ` ${item.taskCode} - ${item.taskName}, ${item.estimateHour} giờ`;
          }
        }

        this.events = [...this.events, meeting];
      });
    }
    this.totalEvents = this.events;
  }

  changeEventFullCalendar(els: any) {
    if (els) {
      let task = this.listAllTask.find(c => c.taskId == els.event.id);
      if (task.statusCode == "NEW") {
        let startDate = null
        if (els.event.start) {
          startDate = convertToUTCTime(els.event.start)
        }
        let endDate = null;
        if (els.event.end) {
          endDate = convertToUTCTime(els.event.end);
        }
        let message: string = '';
        if (els.event.end) {
          // message = "Bạn có muốn chỉnh sửa thời gian lịch hẹn bắt đầu từ : " + this.datePipe.transform(els.event.start, "h:mm dd/MM/yyyy ") + " đến " + this.datePipe.transform(els.event.end, "h:mm dd/MM/yyyy");
        } else {
          // message = "Bạn có muốn chỉnh sửa thời gian lịch hẹn bắt đầu từ : " + this.datePipe.transform(els.event.start, "h:mm dd/MM/yyyy ");
        }
      } else {
        this.loading = true;
        this.projectService.getMasterDataCommonDashboardProject(this.projectId, this.auth.UserId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          this.isManager = result.isManager;
          this.listTaskOverdue = this.setIconTaskType.setIconTaskTypeInListTask(result.listTaskOverdue);
          this.project = result.project;
          this.totalEstimateTime = result.totalEstimateHour;
          this.totalHourUsed = result.totalHourUsed;
          this.totalEE = result.totalEE;
          this.totalPercent = result.projectTaskComplete.toFixed(2);
          this.listProject = result.listProject;
          this.selectedProject = this.listProject.find(c => c.projectId == this.projectId);
          this.listAllTask = result.listAllTask;
          this.listEmployee = result.listEmployee;
          this.listTaskFollowRerouse = this.listAllTask;
          this.setCalendar();
          // API của quản lý
        });
      }
    }
  }

  handleEventClick(eventClick: any) {
    if (eventClick) {
    }
  }

  changeEmployee(employee: any) {
    if (employee) {
      this.listTaskFollowRerouse = this.listAllTask.filter(c => employee.listTaskId.includes(c.taskId));
      this.setCalendar();
    } else {
      this.listTaskFollowRerouse = this.listAllTask;
      this.setCalendar();
    }
  }

  /*END: THÔNG TIN LỊCH LÀM VIỆC*/

  // Tạo Bar Chart ngân sách dự án
  createBarChartBudget(array: Array<ChartBudget>) {
    let arrayCate: Array<string> = array.map(x => x.budgetName);
    let dataBudget: Array<any> = array.map(x => x.budgetValue);

    Highcharts.setOptions({
      chart: {
        style: {
          fontFamily: 'Inter UI',
        },
      }
    });

    Highcharts.chart('container6', {
      chart: {
        type: 'bar',
        height: 200
      },
      title: {
        style: {
          display: 'none'
        }
      },
      xAxis: {
        type: 'category',
        categories: arrayCate,
        tickLength: 0,
        min: 0,
      },
      yAxis: {
        min: 0,
        title: {
          text: '',
        },
      },
      // colors: ['#d7d7d7', '#71b603'],
      plotOptions: {
        bar: {
          allowPointSelect: false,
          cursor: 'pointer',
          dataLabels: {
            enabled: true,
          }
        },
        series: {
          stacking: 'normal'
        },
      },
      legend: {
        reversed: true
      },
      series: [
        {
          name: "Ngân sách",
          data: dataBudget,
        },
      ]
    });
  }
}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
