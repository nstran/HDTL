import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as Highcharts from 'highcharts/highstock';
import { MessageService } from 'primeng/api';
import { Chart } from 'angular-highcharts';
import { EmployeeService } from '../../services/employee.service';

// Full canlendar
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { GetPermission } from '../../../shared/permission/get-permission';

class ChooseTime {
  label: string;
  value: number;
}

class dataChart {
  name: string;
  y: number;
  color: string;
}

/// <summary>
/// Tỉ lệ ứng viên theo trạng thái
/// </summary>
class ChartCandidateFollowStatus {
  status: number;
  statusName: string;
  count: number;
  percentValue: string;
  color: string;
}

/// <summary>
/// Số lượng ứng viên theo kênh tuyển dụng
/// </summary>
class NumberCandidateFollowRecruitmentChannel {
  recruitmentChannelId: string;
  recruitmentChannelName: string;
  totalCandidateCount: number;
  totalCandidateInterviewCount: number;
  totalCandidateEmployeeCount: number;
}

/// <summary>
/// SỐ LƯỢNG NV/THỬ VIỆC TUYỂN ĐƯỢC THEO NHÂN VIÊN
/// </summary>
class NumberCandidateTurnEmployeeFollowEmployee {
  personInChargeId: string;
  listVacanciesId: Array<string>;
  personInChargeName: string;
  count: number;
}

class Calendar {
  id: string;
  title: string;
  start: Date;
  end: Date;
  backgroundColor: string;
}

@Component({
  selector: 'app-chien-dich-dashboard',
  templateUrl: './chien-dich-dashboard.component.html',
  styleUrls: ['./chien-dich-dashboard.component.css']
})
export class ChienDichDashboardComponent implements OnInit {
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  loading: boolean = false;
  today: Date = new Date();
  currentTimeString = this.convertTimeToString();
  innerWidth: number = 0; //number window size first
  colsXeQuaHan: any[];
  colsBaoGiaCho: any[];

  // full calendar
  events: Array<Calendar> = [];
  options: any;

  listTime: Array<ChooseTime> = [
    { label: "Tháng 1", value: 1 },
    { label: "Tháng 2", value: 2 },
    { label: "Tháng 3", value: 3 },
    { label: "Tháng 4", value: 4 },
    { label: "Tháng 5", value: 5 },
    { label: "Tháng 6", value: 6 },
    { label: "Tháng 7", value: 7 },
    { label: "Tháng 8", value: 8 },
    { label: "Tháng 9", value: 9 },
    { label: "Tháng 10", value: 10 },
    { label: "Tháng 11", value: 11 },
    { label: "Tháng 12", value: 12 },
  ];

  selectedTime: ChooseTime;

  listChartCandidateFollowStatus: Array<ChartCandidateFollowStatus> = [];
  listNumberCandidateFollowRecruitmentChannel: Array<NumberCandidateFollowRecruitmentChannel> = [];
  listNumberCandidateTurnEmployeeFollowEmployee: Array<NumberCandidateTurnEmployeeFollowEmployee> = [];
  listInterview: Array<any> = [];

  doughnutChartStatus: Chart;

  constructor(
    private router: Router,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    private getPermission: GetPermission,
  ) {
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    let resource = "rec/employee/dashboard-chien-dich/";
    let permission: any = await this.getPermission.getPermission(resource);

    if (permission.status == false) {
      this.router.navigate(['/home']);
      return;
    }

    let thisMonth = this.today.getMonth();
    this.selectedTime = this.listTime.find(x => x.value == thisMonth + 1);
    this.options = {
      plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
      defaultDate: new Date(),
      header: {
        left: 'prev,next',
        center: 'title',
        right: 'dayGridMonth,timeGridWeek,timeGridDay'
      },
      buttonText: {
        dayGridMonth: 'Tháng',
        timeGridWeek: 'Tuần',
        timeGridDay: 'Ngày',
      },
      allDaySlot: false,
      selectOverlap: true,
    }
    this.getMasterData(this.selectedTime.value);
  }

  getMasterData(timeType: number) {
    this.loading = true;
    this.employeeService.getMasterDataDashboard(timeType).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        //Nếu không phải là nguoif phụ trách hoặc giám đốc hoặc trưởng nhân sự thì về trang home
        if (result.isNguoiPhuTrach || result.isGD || result.isManagerOfHR) {

        } else {
          this.router.navigate(['/home']);
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập' };
          this.showMessage(msg);
        }
        this.listChartCandidateFollowStatus = result.listChartCandidateFollowStatus;
        this.listNumberCandidateFollowRecruitmentChannel = result.listNumberCandidateFollowRecruitmentChannel;
        this.listNumberCandidateTurnEmployeeFollowEmployee = result.listNumberCandidateTurnEmployeeFollowEmployee;
        this.listInterview = result.listInterviewSchedule;

        this.listInterview.forEach(item => {
          item.interviewDate = new Date(item.interviewDate);
        });
        this.listChartCandidateFollowStatus.forEach(item => {
          switch (item.status) {
            case 1:
              item.color = '#FFCD56';
              break;
            case 2:
              item.color = '#4BC0C0';
              break;
            case 3:
              item.color = '#8BEB7A';
              break;
            case 4:
              item.color = '#36A2EB';
              break;
            case 5:
              item.color = '#FF6384';
              break;
            case 6:
              item.color = '#DD4895';
              break;
            case 7:
              item.color = '#2DC258';
              break;
          }
        });

        setTimeout(() => {
          if (this.listChartCandidateFollowStatus.length > 0) this.createChartCandidateFollowStatusDoughnutChart(this.listChartCandidateFollowStatus);
          if (this.listNumberCandidateFollowRecruitmentChannel.length > 0) this.createBarNumberCandidateFollowRecruitmentChannel(this.listNumberCandidateFollowRecruitmentChannel);
          if (this.listNumberCandidateTurnEmployeeFollowEmployee.length > 0) this.createBarNumberCandidateTurnEmployeeFollowEmployee(this.listNumberCandidateTurnEmployeeFollowEmployee);
        }, 0)
        this.setCalendar();
      }
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  // SỐ LƯỢNG NV/THỬ VIỆC TUYỂN ĐƯỢC THEO NHÂN VIÊN
  createChartCandidateFollowStatusDoughnutChart(array: Array<ChartCandidateFollowStatus>) {
    let height = '';
    if (this.innerWidth <= 1370) {
      height = '100%';
    } else {
      height = '70%';
    }

    let data: Array<dataChart> = [];
    array.forEach((e, index) => {
      let item = new dataChart();
      item.name = `${e.statusName}`;
      item.y = e.count;
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

  // SỐ LƯỢNG ỨNG VIÊN THEO KÊNH TUYỂN DỤNG
  createBarNumberCandidateFollowRecruitmentChannel(array: Array<NumberCandidateFollowRecruitmentChannel>) {
    let arrayCate: Array<string> = array.map(x => x.recruitmentChannelName);
    let total: Array<any> = array.map(x => x.totalCandidateCount);
    let interview: Array<any> = array.map(x => x.totalCandidateInterviewCount);
    let emp: Array<any> = array.map(x => x.totalCandidateEmployeeCount);

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
      series: [
        {
          name: "Tổng",
          data: total,
          color: "#FACD91"
        },
        {
          name: "Ứng viên hẹn PV",
          data: interview,
          color: "#78A9FF"
        },
        {
          name: "NV/Thử việc",
          data: emp,
          color: "#2DC258"
        },
      ]
    });
  }

  // Tạo Bar Chart DOANH THU BÁN VTPT THEO NV KINH DOANH PT
  createBarNumberCandidateTurnEmployeeFollowEmployee(array: Array<NumberCandidateTurnEmployeeFollowEmployee>) {
    let arrayCate: Array<string> = array.map(x => x.personInChargeName);
    let data: Array<any> = array.map(x => x.count);
    Highcharts.setOptions({
      chart: {
        style: {
          fontFamily: 'Inter UI'
        }
      }
    });

    Highcharts.chart('container1', {
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
        name: "NV/Thử việc",
        data: data,
      }]
    });
  }

  setCalendar() {
    this.events = [];
    if (this.listInterview) {
      this.listInterview.forEach(item => {
        let meeting = new Calendar();
        let end = new Date(item.interviewDate);
        meeting.id = item.interviewScheduleId;
        meeting.title = item.interviewTitle;
        meeting.start = item.interviewDate;
        meeting.end = new Date(end.setMinutes(end.getMinutes() + item.interviewTime));

        if (meeting.start < new Date()) {
          meeting.backgroundColor = "#DD0000";
        }
        this.events = [...this.events, meeting];
      });
    }
  }

  changeChooseTime(time: ChooseTime) {
    this.getMasterData(time.value);
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

}
