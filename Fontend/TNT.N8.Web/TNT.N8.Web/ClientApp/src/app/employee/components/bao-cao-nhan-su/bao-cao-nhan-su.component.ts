import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import Highcharts from 'highcharts';
import { Table } from 'primeng/table';
import { MessageService } from 'primeng/api';
import { DatePipe } from '@angular/common';
import { Workbook } from 'exceljs';
import * as XLSX from 'xlsx';
import { saveAs } from "file-saver";
import { GetPermission } from '../../../shared/permission/get-permission';
import { ExportFileWordService } from '../../../shared/services/exportFileWord.services';
import { EmployeeService } from './../../services/employee.service';
import { FormatDateService } from './../../../shared/services/formatDate.services';
import { Router, ActivatedRoute } from '@angular/router';

require('highcharts/modules/exporting')(Highcharts);
require('highcharts/modules/export-data')(Highcharts);
require('highcharts/modules/accessibility')(Highcharts);

class NhanVienModel {
  empId: string;
  month: number;
  years: number;
  name: string;
  testingCode: string;
  testScore: string;
  gradeTesting: string;
  workingLocation: string;
  healthCareGroup: string;
  deptCode: string;
  subCode1: string;
  subCode2: string;
}

@Component({
  selector: 'app-bao-cao-nhan-su',
  templateUrl: './bao-cao-nhan-su.component.html',
  styleUrls: ['./bao-cao-nhan-su.component.css']
})
export class BaoCaoNhanSuComponent implements OnInit {
  selectedColumns: any;
  listNhanVien: Array<NhanVienModel>;
  filterGlobal: any;
  @ViewChild('myTable') myTable: Table;
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  leftColNumber: number = 12;
  rightColNumber: number = 0;

  //bộ lọc
  searchForm: FormGroup;
  searchKhuVucLamViec: FormControl;
  searchPhongBan: FormControl;
  searchChucVu: FormControl;
  searchSubCode1: FormControl;
  searchGioiTinh: FormControl;
  searchTrinhDoHocVan: FormControl;
  searchTrinhDoChuyenMon: FormControl;
  searchTrangThai: FormControl;
  searchNgayVaoTu: FormControl;
  searchNgayVaoDen: FormControl;
  searchTuoiTu: FormControl;
  searchTuoiDen: FormControl;
  searchThamNienTu: FormControl;
  searchThamNienDen: FormControl;

  khuVucLamViec: Array<any>;
  trinhDoChuyenMon: Array<any>;
  subCode1: Array<any>;
  gioiTinh: Array<any>;

  //biểu đồ thống kê
  thongTinBieuDo: FormGroup;
  searchLoaiBieuDo: FormControl;
  startTimeBieuDo: FormControl;
  endTimeBieuDo: FormControl;

  thongTinBieuDoChart12: FormGroup;
  searchLoaiBieuDoChart12: FormControl;
  startTimeBieuDoChart12: FormControl;
  endTimeBieuDoChart12: FormControl;

  checkChart12: boolean = false

  loaiBieuDo = [
    { key: 'chart2', name: 'Thống kê nhân sự theo SUB CODE 1' },
    { key: 'chart4', name: 'Thống kê nhân sự theo thâm niên' },
    { key: 'chart5', name: 'Thống kê nhân sự theo giới tính' },
    { key: 'chart7', name: 'Thống kê nhân sự theo độ tuổi' },
    { key: 'chart9', name: 'Thống kê nhân sự theo trình độ học vấn' },
    { key: 'chart11', name: 'Thống kê nhân sự theo chuyên môn' },
    { key: 'chart12', name: 'Thống kê nhân sự đã nghỉ việc' },
  ]

  showChart1: boolean = false
  showChart2: boolean = false
  showChart3: boolean = false
  showChart4: boolean = false
  showChart5: boolean = false
  showChart6: boolean = false
  showChart7: boolean = false
  showChart8: boolean = false
  showChart9: boolean = false
  showChart10: boolean = false
  showChart11: boolean = false
  showChart12: boolean = false
  showTime: boolean = true

  categoriesChartSubCode11: any
  seriesChartSubCode11: any

  categoriesChartSubCode12: any
  seriesChartSubCode12: any

  categoriesChartThamNien: any
  seriesChartThamNien: any

  categoriesChartGioiTinh1: any
  seriesChartGioiTinh1: any

  categoriesChartGioiTinh2: any
  seriesChartGioiTinh2: any

  categoriesChartDoTuoi1: any
  seriesChartDoTuoi1: any

  categoriesChartDoTuoi2: any
  seriesChartDoTuoi2: any

  categoriesChartHocVan1: any
  seriesChartHocVan1: any
  rangeFromChartHocVan1: any;
  rangeToChartHocVan1: any;

  categoriesChartHocVan2: any
  seriesChartHocVan2: any

  categoriesChartChuyenMon: any
  seriesChartChuyenMon: any

  categoriesChartNghiViec: any
  seriesChartNghiViec: any

  month = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

  tuThangTrongNam = 'Tháng 1 năm 2022'
  denThangTrongNam = 'Tháng 2 năm 2022'
  tu = 'Jan 2020'
  den = 'Jan 2022'
  loading: Boolean = false;

  listProvince: any = [];
  listOrganization: any = [];
  listPosition: any = [];
  listDeptCode: any = [];
  listSubCode1: any = [];
  listSubCode2: any = [];
  listCapBac: any = [];
  listBangCap: any = [];
  listKyNangTayNghe: any = [];

  listGioiTinh = [
    { code: 'NAM', name: 'Nam' },
    { code: 'NU', name: 'Nữ' },
  ];

  listTrangThai = [
    {
      id: 1, text: 'Đang hoạt động - Được phép truy cập'
    },
    {
      id: 2, text: 'Đang hoạt động - Không được phép truy cập'
    },
    {
      id: 3, text: 'Ngừng hoạt động'
    }
  ];

  listEmployee: any = [];

  actionDownload: boolean = false;

  constructor(
    public exportFileWordService: ExportFileWordService,
    public employeeService: EmployeeService,
    public messageService: MessageService,
    public formatDateService: FormatDateService,
    private getPermission: GetPermission,
    private router: Router,
  ) {
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    this.setForm();
    this.initTable();
    await this._getPermission();
    this.getMasterData();
  }

  setForm() {
    this.searchKhuVucLamViec = new FormControl;
    this.searchPhongBan = new FormControl;
    this.searchChucVu = new FormControl;
    this.searchSubCode1 = new FormControl;
    this.searchGioiTinh = new FormControl;
    this.searchTrinhDoHocVan = new FormControl;
    this.searchTrinhDoChuyenMon = new FormControl;
    this.searchTrangThai = new FormControl;
    this.searchNgayVaoTu = new FormControl;
    this.searchNgayVaoDen = new FormControl;
    this.searchTuoiTu = new FormControl;
    this.searchTuoiDen = new FormControl;
    this.searchThamNienTu = new FormControl;
    this.searchThamNienDen = new FormControl;

    this.searchForm = new FormGroup({
      searchKhuVucLamViec: this.searchKhuVucLamViec,
      searchPhongBan: this.searchPhongBan,
      searchChucVu: this.searchChucVu,
      searchSubCode1: this.searchSubCode1,
      searchGioiTinh: this.searchGioiTinh,
      searchTrinhDoHocVan: this.searchTrinhDoHocVan,
      searchTrinhDoChuyenMon: this.searchTrinhDoChuyenMon,
      searchTrangThai: this.searchTrangThai,
      searchNgayVaoTu: this.searchNgayVaoTu,
      searchNgayVaoDen: this.searchNgayVaoDen,
      searchTuoiTu: this.searchTuoiTu,
      searchTuoiDen: this.searchTuoiDen,
      searchThamNienTu: this.searchThamNienTu,
      searchThamNienDen: this.searchThamNienDen,
    });

    this.searchLoaiBieuDo = new FormControl(null, [Validators.required]);
    this.startTimeBieuDo = new FormControl(null, [Validators.required]);
    this.endTimeBieuDo = new FormControl(null, [Validators.required]);

    this.thongTinBieuDo = new FormGroup({
      searchLoaiBieuDo: this.searchLoaiBieuDo,
      startTimeBieuDo: this.startTimeBieuDo,
      endTimeBieuDo: this.endTimeBieuDo,
    },
      (formGroup: FormGroup) => {
        return ValidateTime(formGroup);
      })

    this.searchLoaiBieuDo.setValue({ key: 'chart2', name: 'Thống kê nhân sự theo SUB CODE 1' })

    this.searchLoaiBieuDoChart12 = new FormControl(null, [Validators.required]);
    this.startTimeBieuDoChart12 = new FormControl(null, [Validators.required]);
    this.endTimeBieuDoChart12 = new FormControl(null, [Validators.required]);

    this.thongTinBieuDoChart12 = new FormGroup({
      searchLoaiBieuDoChart12: this.searchLoaiBieuDoChart12,
      startTimeBieuDoChart12: this.startTimeBieuDoChart12,
      endTimeBieuDoChart12: this.endTimeBieuDoChart12,
    })
  }

  initTable() {
    this.selectedColumns = [
      { field: 'stt', header: 'STT', width: '50px', textAlign: 'center' },
      { field: 'employeeCode', header: 'Emp ID', width: '100px', textAlign: 'left' },
      { field: 'employeeName', header: 'Name', width: '100px', textAlign: 'left' },
      { field: 'capBacName', header: 'GRADE TESTING', width: '100px', textAlign: 'left' },
      { field: 'provinceName', header: 'Working location', width: '100px', textAlign: 'left' },
      { field: 'organizationName', header: 'Phòng ban', width: '100px', textAlign: 'left' },
      { field: 'positionName', header: 'Chức vụ', width: '100px', textAlign: 'left' },
      { field: 'deptCode', header: 'DEPT CODE', width: '100px', textAlign: 'left' },
      { field: 'subCode1Name', header: 'SUB-CODE1', width: '100px', textAlign: 'left' },
      { field: 'subCode2Name', header: 'SUB-CODE2', width: '100px', textAlign: 'left' },
      { field: 'genderName', header: 'Giới tính', width: '100px', textAlign: 'left' },
      { field: 'bangCapCaoNhatDatDuocName', header: 'Trình độ học vấn', width: '100px', textAlign: 'left' },
      { field: 'kyNangTayNghesName', header: 'Trình độ chuyên môn', width: '100px', textAlign: 'left' },
      { field: 'trangThaiName', header: 'Trạng thái', width: '100px', textAlign: 'left' },
      { field: 'startDateMayChamCong', header: 'Ngày vào', width: '100px', textAlign: 'left' },
      { field: 'dateOfBirth', header: 'Ngày sinh', width: '100px', textAlign: 'left' },
    ]
  }

  async _getPermission() {
    this.loading = true;
    let resource = "hrm/employee/bao-cao-nhan-su/";
    let permission: any = await this.getPermission.getPermission(resource);

    if (permission.status == false) {
      this.router.navigate(["/home"]);
      return;
    }

    if (permission.listCurrentActionResource.indexOf("download") != -1) {
      this.actionDownload = true;
    }
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.employeeService.getListBaoCaoNhanSu();
    this.loading = false;

    if (result.statusCode == 200) {
      this.listProvince = result.listProvince;
      this.listOrganization = result.listOrganization;
      this.listPosition = result.listPosition;
      this.listDeptCode = result.listDeptCode;
      this.listSubCode1 = result.listSubCode1;
      this.listSubCode2 = result.listSubCode2;
      this.listCapBac = result.listCapBac;
      this.listBangCap = result.listBangCap;
      this.listKyNangTayNghe = result.listKyNangTayNghe;

      this.listEmployee = result.listEmployee.map((item) =>
        Object.assign({
          genderName: item.gender ? this.listGioiTinh.find(x => x.code == item.gender) ?.name : '',
          trangThaiName: item.trangThaiId ? this.listTrangThai.find(x => x.id == item.trangThaiId) ?.text : '',
          startDateMayChamCongTu: item.startDateMayChamCong,
          startDateMayChamCongDen: item.startDateMayChamCong,
          doTuoiTu: item.age,
          doTuoiDen: item.age,
          thamNienTu: item.soNamLamViec,
          thamNienDen: item.soNamLamViec,
        }, item)
      );
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  checkLoaiBieuDo(data: any) {
    this.startTimeBieuDo.reset()
    this.endTimeBieuDo.reset()
    this.startTimeBieuDoChart12.reset()
    this.endTimeBieuDoChart12.reset()
    if (data.value.key == 'chart12') {
      this.showTime = true
      this.checkChart12 = true
      this.searchLoaiBieuDoChart12.setValue(data.value)
    } else {
      this.checkChart12 = false
      this.searchLoaiBieuDo.setValue(data.value)
      if (data.value.key == 'chart4' || data.value.key == 'chart11') {
        this.showTime = false
      } else {
        this.showTime = true
      }
    }
  }

  async showBieuDo() {
    this.showChart1 = false
    this.showChart2 = false
    this.showChart3 = false
    this.showChart4 = false
    this.showChart5 = false
    this.showChart6 = false
    this.showChart7 = false
    this.showChart8 = false
    this.showChart9 = false
    this.showChart10 = false
    this.showChart11 = false
    this.showChart12 = false

    //Type search:
    //2-Thống kê nhân sự theo SUB CODE 1: các tháng cùng kỳ các năm
    //3-Thống kê nhân sự theo SUB CODE 1: các tháng trong năm
    //4-Thống kê nhân sự theo thâm niên
    //5-Thống kê nhân sự theo giới tính: các tháng cùng kỳ các năm
    //6-Thống kê nhân sự theo giới tính: các tháng trong năm
    //7-Thống kê nhân sự theo độ tuổi: các tháng cùng kỳ các năm
    //8-Thống kê nhân sự theo độ tuổi: các tháng trong năm
    //9-Thống kê nhân sự theo trình độ học vấn: các tháng cùng kỳ các năm
    //10-Thống kê nhân sự theo trình độ học vấn: các tháng trong năm
    //11-Thống kê nhân sự theo chuyên môn
    //12-Thống kê nhân sự đã nghỉ việc

    if (this.searchLoaiBieuDo) {
      var data = this.loaiBieuDo.find(x => x.key == this.searchLoaiBieuDo.value.key);

      if (data.key == 'chart12') {
        if (!this.thongTinBieuDoChart12.valid) {
          Object.keys(this.thongTinBieuDoChart12.controls).forEach(key => {
            if (!this.thongTinBieuDoChart12.controls[key].valid) {
              this.thongTinBieuDoChart12.controls[key].markAsTouched();
            }
          });
        } else {
          // get data
          let dataSearch = {
            fromDate: this.formatDateService.convertToUTCTime(this.startTimeBieuDoChart12.value),
            toDate: this.formatDateService.convertToUTCTime(this.endTimeBieuDoChart12.value)
          }
          this.loading = true;
          let result: any = await this.employeeService.getBieuDoThongKeNhanSu(12, dataSearch.fromDate, dataSearch.toDate);
          this.loading = false;

          if (result.statusCode == 200) {
            this.categoriesChartNghiViec = result.categoriesChart;
            this.seriesChartNghiViec = result.chartThongKeNhanSu;
          }
          else {
            this.showMessage('error', result.messageCode);
          }

          // build the chart
          var datePipe = new DatePipe("en-US");
          var chart12 = Highcharts.chart('chart12', {
            chart: {
              type: 'column',
              backgroundColor: '#f2f4f8'
            },
            title: {
              text: `Turn Over Rate ${datePipe.transform(this.startTimeBieuDoChart12.value, 'M/yyyy')} - ${datePipe.transform(this.endTimeBieuDoChart12.value, 'M/yyyy')}`
            },
            xAxis: {
              categories: this.categoriesChartNghiViec
            },
            yAxis: {
              title: {
                text: ''
              }
            },
            credits: {
              enabled: false
            },
            series: this.seriesChartNghiViec,
            plotOptions: {
              series: {
                dataLabels: {
                  enabled: true,
                  format: '{point.y}%'
                }
              }
            },
            exporting: {
              buttons: {
                contextButton: {
                  menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                },
              },
              enabled: true,
            },

          });

          this.showChart12 = true
        }


      } else if (data.key == 'chart11') {
        // get data
        this.seriesChartChuyenMon = [{
          name: 'Series 1',
          data: []
        }];

        this.loading = true;
        let result: any = await this.employeeService.getBieuDoThongKeNhanSu(11);
        this.loading = false;

        if (result.statusCode == 200) {
          this.seriesChartChuyenMon[0].data = result.pieChartThongKeNhanSu;
        }
        else {
          this.showMessage('error', result.messageCode);
        }

        // build the chart
        var chart11 = (Highcharts as any).chart('chart11', {

          chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie',
            backgroundColor: '#f2f4f8'
          },
          title: {
            text: `Major Ratio ${this.tu} - ${this.den}`
          },
          tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
          },
          accessibility: {
            point: {
              valueSuffix: '%'
            }
          },
          plotOptions: {
            pie: {
              allowPointSelect: true,
              cursor: 'pointer',
              dataLabels: {
                enabled: true,
                format: '<b>{point.name}</b><br>{point.percentage:.1f} %',
                distance: -50,
                filter: {
                  property: 'percentage',
                  operator: '>',
                  value: 4
                }
              }
            }
          },
          series: this.seriesChartChuyenMon,
          credits: {
            enabled: false
          },
          exporting: {
            buttons: {
              contextButton: {
                menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
              },
            },
            enabled: true,
          },
        });

        this.showChart11 = true
      } else if (data.key == 'chart9') {
        if (!this.thongTinBieuDo.valid) {
          Object.keys(this.thongTinBieuDo.controls).forEach(key => {
            if (!this.thongTinBieuDo.controls[key].valid) {
              this.thongTinBieuDo.controls[key].markAsTouched();
            }
          });
        } else {
          let namTu = new Date(this.startTimeBieuDo.value).getFullYear()
          let namDen = new Date(this.endTimeBieuDo.value).getFullYear()
          if (namTu == namDen) {
            // get data
            let dataSearch = {
              fromDate: this.formatDateService.convertToUTCTime(this.startTimeBieuDo.value),
              toDate: this.formatDateService.convertToUTCTime(this.endTimeBieuDo.value)
            }
            this.loading = true;
            let result: any = await this.employeeService.getBieuDoThongKeNhanSu(10, dataSearch.fromDate, dataSearch.toDate);
            this.loading = false;

            if (result.statusCode == 200) {
              this.categoriesChartHocVan2 = result.categoriesChart;
              this.seriesChartHocVan2 = result.chartThongKeNhanSu;
            }
            else {
              this.showMessage('error', result.messageCode);
            }

            // this.categoriesChartHocVan2 = ['Tháng 1 năm 2022', 'Tháng 2 năm 2022']
            // this.seriesChartHocVan2 = [{
            //   name: 'Univercity',
            //   data: [65, 64]
            // }, {
            //   name: 'Training Center',
            //   data: [22, 23]
            // }, {
            //   name: 'High School',
            //   data: [13, 13]
            // }, {
            //   name: 'College',
            //   data: [36, 35]
            // }]

            // build the chart
            var chart10 = Highcharts.chart('chart10', {
              chart: {
                type: 'column',
                backgroundColor: '#f2f4f8'
              },
              credits: {
                enabled: false
              },
              title: {
                text: `Qualification ${this.categoriesChartHocVan2[0]} - ${this.categoriesChartHocVan2[this.categoriesChartHocVan2.length - 1]}`
              },
              xAxis: {
                categories: this.categoriesChartHocVan2
              },
              yAxis: {
                title: {
                  text: ''
                }

              },
              tooltip: {
                pointFormat: '<span style="color:{series.color}">{series.name}</span>: <b>{point.y}</b> ({point.percentage:.0f})<br/>',
                shared: true
              },
              legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle'
              },

              plotOptions: {
                column: {
                  stacking: 'normal',
                  dataLabels: {
                    enabled: true,
                    format: '{point.y}'
                  }
                }
              },
              series: this.seriesChartHocVan2,
              exporting: {
                buttons: {
                  contextButton: {
                    menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                  },
                },
                enabled: true,
              },
            });

            this.showChart10 = true
          } else {
            // get data
            let dataSearch = {
              fromDate: this.formatDateService.convertToUTCTime(this.startTimeBieuDo.value),
              toDate: this.formatDateService.convertToUTCTime(this.endTimeBieuDo.value)
            }
            this.loading = true;
            let result: any = await this.employeeService.getBieuDoThongKeNhanSu(9, dataSearch.fromDate, dataSearch.toDate);
            this.loading = false;

            if (result.statusCode == 200) {
              this.categoriesChartHocVan1 = result.categoriesChart;
              this.seriesChartHocVan1 = result.chartThongKeNhanSu;
            }
            else {
              this.showMessage('error', result.messageCode);
            }
            // this.categoriesChartHocVan1 = ['Tháng 1 năm 2018', 'Tháng 1 năm 2019', 'Tháng 1 năm 2020', 'Tháng 1 năm 2021'];
            // this.seriesChartHocVan1 = [{
            //   name: 'Univercity',
            //   data: [65, 64, 50, 14]
            // }, {
            //   name: 'Training Center',
            //   data: [22, 23, 14, 30]
            // }, {
            //   name: 'High School',
            //   data: [13, 13, 23, 40]
            // }, {
            //   name: 'College',
            //   data: [36, 35, 14, 33]
            // }];
            // this.rangeFromChartHocVan1 = 2018;
            // this.rangeToChartHocVan1 = 2021;

            // build the chart
            let seriesChart902 = JSON.parse(JSON.stringify(this.seriesChartHocVan1));

            this.categoriesChartHocVan1.forEach((itemCat, indexCat) => {
              if (indexCat > 0) {
                this.seriesChartHocVan1.forEach((itemSeries, indexSeries) => {
                  seriesChart902[indexSeries].data[indexCat] = itemSeries.data[indexCat] - itemSeries.data[indexCat - 1];
                })
              }
            });

            var chart901 = Highcharts.chart('chart901', {
              chart: {
                type: 'column',
                backgroundColor: '#f2f4f8'
              },
              credits: {
                enabled: false
              },
              title: {
                text: `Qualification ${this.categoriesChartHocVan1[0]} - ${this.categoriesChartHocVan1[this.categoriesChartHocVan1.length - 1]}`
              },
              xAxis: {
                categories: this.categoriesChartHocVan1
              },
              yAxis: {
                title: {
                  text: ''
                }
              },
              tooltip: {
                pointFormat: '<span style="color:{series.color}">{series.name}</span>: <b>{point.y}</b> ({point.percentage:.0f})<br/>',
                shared: true
              },
              legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle'
              },

              plotOptions: {
                column: {
                  stacking: 'normal',
                  dataLabels: {
                    enabled: true,
                    format: '{point.y}'
                  }
                }
              },
              series: this.seriesChartHocVan1,
              exporting: {
                buttons: {
                  contextButton: {
                    menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                  },
                },
                enabled: true,
              },
            });

            var chart902 = (Highcharts as any).chart('chart902', {
              chart: {
                backgroundColor: '#f2f4f8'
              },
              credits: {
                enabled: false
              },
              title: {
                text: `Compare (+/-) ${this.rangeFromChartHocVan1} to ${this.rangeToChartHocVan1}`
              },

              subtitle: {
                text: ''
              },

              yAxis: {
                title: {
                  text: ''
                }
              },

              xAxis: {
                // accessibility: {
                //   rangeDescription: `Range: ${this.rangeFromChart902} to ${this.rangeToChart902}`
                // }
                categories: this.categoriesChartHocVan1
              },

              legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle'
              },

              plotOptions: {
                line: {
                  dataLabels: {
                    enabled: true
                  },
                  enableMouseTracking: true
                },
                series: {
                  label: {
                    connectorAllowed: false
                  },
                  pointStart: this.rangeFromChartHocVan1
                }
              },

              series: seriesChart902,

              responsive: {
                rules: [{
                  condition: {
                    maxWidth: 300
                  },
                  chartOptions: {
                    legend: {
                      layout: 'horizontal',
                      align: 'center',
                      verticalAlign: 'bottom'
                    }
                  }
                }]
              },
              exporting: {
                buttons: {
                  contextButton: {
                    menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                  },
                },
                enabled: true,
              },

            });
            this.showChart9 = true
          }
        }
      } else if (data.key == 'chart7') {

        if (!this.thongTinBieuDo.valid) {
          Object.keys(this.thongTinBieuDo.controls).forEach(key => {
            if (!this.thongTinBieuDo.controls[key].valid) {
              this.thongTinBieuDo.controls[key].markAsTouched();
            }
          });
        } else {
          let namTu = new Date(this.startTimeBieuDo.value).getFullYear()
          let namDen = new Date(this.endTimeBieuDo.value).getFullYear()
          if (namTu == namDen) {
            // get data
            let dataSearch = {
              fromDate: this.formatDateService.convertToUTCTime(this.startTimeBieuDo.value),
              toDate: this.formatDateService.convertToUTCTime(this.endTimeBieuDo.value)
            }
            this.loading = true;
            let result: any = await this.employeeService.getBieuDoThongKeNhanSu(8, dataSearch.fromDate, dataSearch.toDate);
            this.loading = false;

            if (result.statusCode == 200) {
              this.categoriesChartDoTuoi2 = result.categoriesChart;
              this.seriesChartDoTuoi2 = result.chartThongKeNhanSu;
            }
            else {
              this.showMessage('error', result.messageCode);
            }
            // this.categoriesChartDoTuoi2 = ['Tháng 1 năm 2022', 'Tháng 2 năm 2022']
            // this.seriesChartDoTuoi2 = [
            //   {
            //     name: '39-43',
            //     data: [1, 2]
            //   }, {
            //     name: '34-38',
            //     data: [10, 15]
            //   }, {
            //     name: '29-33',
            //     data: [22, 23]
            //   }, {
            //     name: '24-28',
            //     data: [30, 28]
            //   }, {
            //     name: '19-23',
            //     data: [36, 35]
            //   }]

            // build the chart
            var chart8 = Highcharts.chart('chart8', {
              chart: {
                type: 'column',
                backgroundColor: '#f2f4f8'
              },
              credits: {
                enabled: false
              },
              title: {
                text: `Age Ratio ${this.categoriesChartDoTuoi2[0]} - ${this.categoriesChartDoTuoi2[this.categoriesChartDoTuoi2.length - 1]}`
              },
              xAxis: {
                categories: this.categoriesChartDoTuoi2
              },
              yAxis: {
                title: {
                  text: ''
                }
              },
              tooltip: {
                pointFormat: '<span style="color:{series.color}">{series.name}</span>: <b>{point.y}</b> ({point.percentage:.0f})<br/>',
                shared: true
              },
              legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle'
              },

              plotOptions: {
                column: {
                  stacking: 'normal',
                  dataLabels: {
                    enabled: true,
                    format: '{point.y}'
                  }
                }
              },
              series: this.seriesChartDoTuoi2,
              exporting: {
                buttons: {
                  contextButton: {
                    menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                  },
                },
                enabled: true,
              },
            });

            this.showChart8 = true
          } else {
            // get data
            let dataSearch = {
              fromDate: this.formatDateService.convertToUTCTime(this.startTimeBieuDo.value),
              toDate: this.formatDateService.convertToUTCTime(this.endTimeBieuDo.value)
            }
            this.loading = true;
            let result: any = await this.employeeService.getBieuDoThongKeNhanSu(7, dataSearch.fromDate, dataSearch.toDate);
            this.loading = false;

            if (result.statusCode == 200) {
              this.categoriesChartDoTuoi1 = result.categoriesChart;
              this.seriesChartDoTuoi1 = result.chartThongKeNhanSu;
            }
            else {
              this.showMessage('error', result.messageCode);
            }
            // this.categoriesChartDoTuoi1 = ['2/2020', '2/2021', '2/2022'];
            // this.seriesChartDoTuoi1 = [
            //   {
            //     name: '39-43',
            //     data: [2, 2, 1]
            //   }, {
            //     name: '34-38',
            //     data: [3, 3, 1]
            //   }, {
            //     name: '29-33',
            //     data: [7, 10, 15]
            //   }, {
            //     name: '24-28',
            //     data: [33, 42, 79]
            //   }, {
            //     name: '19-23',
            //     data: [2, 4, 39]
            //   }
            // ];

            // build the chart
            let seriesChart702 = JSON.parse(JSON.stringify(this.seriesChartDoTuoi1));

            this.categoriesChartDoTuoi1.forEach((itemCat, indexCat) => {
              if (indexCat > 0) {
                this.seriesChartDoTuoi1.forEach((itemSeries, indexSeries) => {
                  seriesChart702[indexSeries].data[indexCat] = itemSeries.data[indexCat] - itemSeries.data[indexCat - 1];
                })
              }
            });


            // chart 7-01
            Highcharts.chart('chart701', {
              chart: {
                type: 'column',
                backgroundColor: '#f2f4f8'
              },
              credits: {
                enabled: false
              },
              title: {
                text: `Age ratio ${this.categoriesChartDoTuoi1[0]} - ${this.categoriesChartDoTuoi1[this.categoriesChartDoTuoi1.length - 1]}`,
                align: 'center',
                style: {
                  fontWeight: 'bold',
                }
              },
              xAxis: {
                categories: this.categoriesChartDoTuoi1,
              },
              yAxis: {
                title: {
                  text: null
                },
                stackLabels: {
                  enabled: true,
                  style: {
                    fontWeight: 'bold',
                    color: 'gray',
                    textOutline: 'none'
                  }
                }
              },
              legend: {
                align: 'right',
                layout: 'vertical',
                verticalAlign: 'middle',
                backgroundColor: 'white',
                borderColor: '#CCC',
                borderWidth: 1,
                itemMarginTop: 4,
                itemMarginBottom: 4
              },
              tooltip: {
                headerFormat: '<b>{point.x}</b><br/>',
                pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
              },
              plotOptions: {
                column: {
                  stacking: 'normal',
                  dataLabels: {
                    enabled: true
                  }
                }
              },
              series: this.seriesChartDoTuoi1,
              exporting: {
                buttons: {
                  contextButton: {
                    menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                  },
                },
                enabled: true,
              },
            });

            // chart 7-02
            Highcharts.chart('chart702', {
              chart: {
                type: 'line',
                backgroundColor: '#f2f4f8'
              },
              credits: {
                enabled: false
              },
              title: {
                text: `Compare (+/-) from ${this.categoriesChartDoTuoi1[0]} to ${this.categoriesChartDoTuoi1[this.categoriesChartDoTuoi1.length - 1]}`,
                style: {
                  fontWeight: 'bold',
                }
              },
              xAxis: {
                categories: this.categoriesChartDoTuoi1
              },
              yAxis: {
                title: {
                  text: null
                }
              },
              plotOptions: {
                line: {
                  dataLabels: {
                    enabled: true
                  },
                  enableMouseTracking: true
                }
              },
              series: seriesChart702,
              exporting: {
                buttons: {
                  contextButton: {
                    menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                  },
                },
                enabled: true,
              },
            });

            this.showChart7 = true
          }
        }
      } else if (data.key == 'chart5') {
        if (!this.thongTinBieuDo.valid) {
          Object.keys(this.thongTinBieuDo.controls).forEach(key => {
            if (!this.thongTinBieuDo.controls[key].valid) {
              this.thongTinBieuDo.controls[key].markAsTouched();
            }
          });
        } else {
          let namTu = new Date(this.startTimeBieuDo.value).getFullYear()
          let namDen = new Date(this.endTimeBieuDo.value).getFullYear()
          if (namTu == namDen) {
            // get data
            let dataSearch = {
              fromDate: this.formatDateService.convertToUTCTime(this.startTimeBieuDo.value),
              toDate: this.formatDateService.convertToUTCTime(this.endTimeBieuDo.value)
            }
            this.loading = true;
            let result: any = await this.employeeService.getBieuDoThongKeNhanSu(6, dataSearch.fromDate, dataSearch.toDate);
            this.loading = false;

            if (result.statusCode == 200) {
              this.categoriesChartGioiTinh2 = result.categoriesChart;
              this.seriesChartGioiTinh2 = result.chartThongKeNhanSu;
            }
            else {
              this.showMessage('error', result.messageCode);
            }
            // this.categoriesChartGioiTinh2 = ['Tháng 1 năm 2022', 'Tháng 2 năm 2022']
            // this.seriesChartGioiTinh2 = [
            //   {
            //     name: 'Male',
            //     data: [95, 92]
            //   }, {
            //     name: 'Female',
            //     data: [25, 45]
            //   }
            // ]

            // build the chart
            var chart6 = Highcharts.chart('chart6', {
              chart: {
                type: 'column',
                backgroundColor: '#f2f4f8'
              },
              credits: {
                enabled: false
              },
              title: {
                text: `Gender ratio in ${this.categoriesChartGioiTinh2[0]} - ${this.categoriesChartGioiTinh2[this.categoriesChartGioiTinh2.length - 1]}`
              },
              xAxis: {
                categories: this.categoriesChartGioiTinh2
              },
              yAxis: {
                title: {
                  text: ''
                }
              },
              tooltip: {
                pointFormat: '<span style="color:{series.color}">{series.name}</span>: <b>{point.y}</b> ({point.percentage:.0f}%)<br/>',
                shared: true
              },
              legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle'
              },

              plotOptions: {
                column: {
                  stacking: 'normal',
                  dataLabels: {
                    enabled: true,
                    format: '{point.y}'
                  }
                }
              },
              series: this.seriesChartGioiTinh2,
              exporting: {
                buttons: {
                  contextButton: {
                    menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                  },
                },
                enabled: true,
              },
            });

            this.showChart6 = true
          } else {
            // get data
            let dataSearch = {
              fromDate: this.formatDateService.convertToUTCTime(this.startTimeBieuDo.value),
              toDate: this.formatDateService.convertToUTCTime(this.endTimeBieuDo.value)
            }
            this.loading = true;
            let result: any = await this.employeeService.getBieuDoThongKeNhanSu(5, dataSearch.fromDate, dataSearch.toDate);
            this.loading = false;

            if (result.statusCode == 200) {
              this.categoriesChartGioiTinh2 = result.categoriesChart;
              this.seriesChartGioiTinh2 = result.chartThongKeNhanSu;
            }
            else {
              this.showMessage('error', result.messageCode);
            }
            // this.categoriesChartGioiTinh1 = ['2/2020', '2/2021', '2/2022'];
            // this.seriesChartGioiTinh1 = [
            //   {
            //     name: 'Male',
            //     data: [35, 47, 98]
            //   }, {
            //     name: 'Female',
            //     data: [12, 14, 37]
            //   }
            // ];

            // build the chart
            let seriesChart502 = JSON.parse(JSON.stringify(this.seriesChartGioiTinh1));

            this.categoriesChartGioiTinh1.forEach((itemCat, indexCat) => {
              if (indexCat > 0) {
                this.seriesChartGioiTinh1.forEach((itemSeries, indexSeries) => {
                  seriesChart502[indexSeries].data[indexCat] = itemSeries.data[indexCat] - itemSeries.data[indexCat - 1];
                })
              }
            });

            // chart 5-01
            Highcharts.chart('chart501', {
              chart: {
                type: 'column',
                backgroundColor: '#f2f4f8'
              },
              credits: {
                enabled: false
              },
              title: {
                text: `Gender ratio in ${this.categoriesChartGioiTinh1[0]} - ${this.categoriesChartGioiTinh1[this.categoriesChartGioiTinh1.length - 1]}`,
                align: 'center',
                style: {
                  fontWeight: 'bold',
                }
              },
              xAxis: {
                categories: this.categoriesChartGioiTinh1,
              },
              yAxis: {
                title: {
                  text: null
                },
                stackLabels: {
                  enabled: true,
                  style: {
                    fontWeight: 'bold',
                    color: 'gray',
                    textOutline: 'none'
                  }
                }
              },
              legend: {
                align: 'right',
                layout: 'vertical',
                verticalAlign: 'middle',
                backgroundColor: 'white',
                borderColor: '#CCC',
                borderWidth: 1,
                itemMarginTop: 4,
                itemMarginBottom: 4
              },
              tooltip: {
                headerFormat: '<b>{point.x}</b><br/>',
                pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
              },
              plotOptions: {
                column: {
                  stacking: 'normal',
                  dataLabels: {
                    enabled: true
                  }
                }
              },
              series: this.seriesChartGioiTinh1,
              exporting: {
                buttons: {
                  contextButton: {
                    menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                  },
                },
                enabled: true,
              },

            });

            // chart 5-02
            Highcharts.chart('chart502', {
              chart: {
                type: 'line',
                backgroundColor: '#f2f4f8'
              },
              credits: {
                enabled: false
              },
              title: {
                text: `Compare (+/-) from ${this.categoriesChartGioiTinh1[0]} to ${this.categoriesChartGioiTinh1[this.categoriesChartGioiTinh1.length - 1]}`,
                style: {
                  fontWeight: 'bold',
                }
              },
              xAxis: {
                categories: this.categoriesChartGioiTinh1
              },
              yAxis: {
                title: {
                  text: null
                }
              },
              plotOptions: {
                line: {
                  dataLabels: {
                    enabled: true
                  },
                  enableMouseTracking: true
                }
              },
              series: seriesChart502,
              exporting: {
                buttons: {
                  contextButton: {
                    menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                  },
                },
                enabled: true,
              },
            });

            this.showChart5 = true
          }
        }
      } else if (data.key == 'chart4') {
        // get data
        this.seriesChartThamNien = [
          {
            name: 'Share',
            data: []
          }
        ];

        this.loading = true;
        let result: any = await this.employeeService.getBieuDoThongKeNhanSu(4);
        this.loading = false;

        if (result.statusCode == 200) {
          this.seriesChartThamNien[0].data = result.pieChartThongKeNhanSu;
        }
        else {
          this.showMessage('error', result.messageCode);
        }


        // build the chart
        let totalChart4 = 0;
        let seriesChart401 = JSON.parse(JSON.stringify(this.seriesChartThamNien));

        this.seriesChartThamNien[0].data.forEach(item => {
          totalChart4 += item.y;
        });
        seriesChart401[0].data.forEach(item => {
          item.y = item.y / totalChart4;
        });

        (Highcharts as any).chart('chart4', {
          chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie',
            backgroundColor: '#f2f4f8'
          },
          title: {
            text: 'Senority',
            style: {
              fontWeight: 'bold',
            }
          },
          credits: {
            enabled: false
          },
          legend: {
            align: 'center',
            verticalAlign: 'bottom'
          },
          tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
          },
          accessibility: {
            point: {
              valueSuffix: '%'
            }
          },
          plotOptions: {
            pie: {
              allowPointSelect: true,
              cursor: 'pointer',
              dataLabels: {
                enabled: true,
                format: '{point.percentage:.1f} %',
                distance: -50,
                filter: {
                  property: 'percentage',
                  operator: '>',
                  value: 4
                }
              },
              showInLegend: true,
            }
          },
          series: seriesChart401,
          exporting: {
            buttons: {
              contextButton: {
                menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
              },
            },
            enabled: true,
          },
        });

        this.showChart4 = true
      } else if (data.key == 'chart2') {

        if (!this.thongTinBieuDo.valid) {
          Object.keys(this.thongTinBieuDo.controls).forEach(key => {
            if (!this.thongTinBieuDo.controls[key].valid) {
              this.thongTinBieuDo.controls[key].markAsTouched();
            }
          });
        } else {
          let namTu = new Date(this.startTimeBieuDo.value).getFullYear()
          let namDen = new Date(this.endTimeBieuDo.value).getFullYear()
          if (namTu == namDen) {
            // get data
            let dataSearch = {
              fromDate: this.formatDateService.convertToUTCTime(this.startTimeBieuDo.value),
              toDate: this.formatDateService.convertToUTCTime(this.endTimeBieuDo.value)
            }
            this.loading = true;
            let result: any = await this.employeeService.getBieuDoThongKeNhanSu(3, dataSearch.fromDate, dataSearch.toDate);
            this.loading = false;

            if (result.statusCode == 200) {
              this.categoriesChartSubCode12 = result.categoriesChart;
              this.seriesChartSubCode12 = result.chartThongKeNhanSu;
            }
            else {
              this.showMessage('error', result.messageCode);
            }

            // build the chart
            Highcharts.chart('chart3', {
              chart: {
                type: 'column',
                backgroundColor: '#f2f4f8'
              },
              credits: {
                enabled: false
              },
              title: {
                text: `HC ${this.categoriesChartSubCode12[0]} - ${this.categoriesChartSubCode12[this.categoriesChartSubCode12.length - 1]}`,
                align: 'center',
                style: {
                  fontWeight: 'bold',
                }
              },
              xAxis: {
                categories: this.categoriesChartSubCode12,
              },
              yAxis: {
                title: {
                  text: null
                },
                stackLabels: {
                  enabled: true,
                  style: {
                    fontWeight: 'bold',
                    color: 'gray',
                    textOutline: 'none'
                  }
                }
              },
              legend: {
                align: 'right',
                layout: 'vertical',
                verticalAlign: 'middle',
                backgroundColor: 'white',
                borderColor: '#CCC',
                borderWidth: 1,
                itemMarginTop: 4,
                itemMarginBottom: 4
              },
              tooltip: {
                headerFormat: '<b>{point.x}</b><br/>',
                pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
              },
              plotOptions: {
                column: {
                  stacking: 'normal',
                  dataLabels: {
                    enabled: true
                  }
                }
              },
              series: this.seriesChartSubCode12,
              exporting: {
                buttons: {
                  contextButton: {
                    menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                  },
                },
                enabled: true,
              },

            });

            this.showChart3 = true
          } else {
            //get data
            let dataSearch = {
              fromDate: this.formatDateService.convertToUTCTime(this.startTimeBieuDo.value),
              toDate: this.formatDateService.convertToUTCTime(this.endTimeBieuDo.value)
            }
            this.loading = true;
            let result: any = await this.employeeService.getBieuDoThongKeNhanSu(2, dataSearch.fromDate, dataSearch.toDate);
            this.loading = false;

            if (result.statusCode == 200) {
              this.categoriesChartSubCode11 = result.categoriesChart;
              this.seriesChartSubCode11 = result.chartThongKeNhanSu;
            }
            else {
              this.showMessage('error', result.messageCode);
            }

            // this.categoriesChartSubCode11 = ['2/2020', '2/2021', '2/2022'];
            // this.seriesChartSubCode11 = [
            //   {
            //     name: 'OPS-DN',
            //     data: [14, 15, 68]
            //   }, {
            //     name: 'OPS-HN',
            //     data: [71, 56, 68]
            //   }, {
            //     name: 'G&A-DN',
            //     data: [31, 80, 41]
            //   }, {
            //     name: 'G&A-HN',
            //     data: [10, 23, 13]
            //   }, {
            //     name: 'COS-DN',
            //     data: [41, 42, 10]
            //   }, {
            //     name: 'COS-HN',
            //     data: [22, 19, 9]
            //   }];

            //build the chart
            let seriesChart202 = JSON.parse(JSON.stringify(this.seriesChartSubCode11));

            this.categoriesChartSubCode11.forEach((itemCat, indexCat) => {
              if (indexCat > 0) {
                this.seriesChartSubCode11.forEach((itemSeries, indexSeries) => {
                  seriesChart202[indexSeries].data[indexCat] = itemSeries.data[indexCat] - itemSeries.data[indexCat - 1];
                })
              }
            });

            // chart 2-01
            Highcharts.chart('chart201', {
              chart: {
                type: 'column',
                backgroundColor: '#f2f4f8'
              },
              credits: {
                enabled: false
              },
              title: {
                text: `HC ${this.categoriesChartSubCode11[0]} to ${this.categoriesChartSubCode11[this.categoriesChartSubCode11.length - 1]}`,
                align: 'center',
                style: {
                  fontWeight: 'bold',
                }
              },
              xAxis: {
                categories: this.categoriesChartSubCode11,
              },
              yAxis: {
                title: {
                  text: null
                },
                stackLabels: {
                  enabled: true,
                  style: {
                    fontWeight: 'bold',
                    color: 'gray',
                    textOutline: 'none'
                  }
                }
              },
              legend: {
                align: 'right',
                layout: 'vertical',
                verticalAlign: 'middle',
                backgroundColor: 'white',
                borderColor: '#CCC',
                borderWidth: 1,
                itemMarginTop: 4,
                itemMarginBottom: 4
              },
              tooltip: {
                headerFormat: '<b>{point.x}</b><br/>',
                pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
              },
              plotOptions: {
                column: {
                  stacking: 'normal',
                  dataLabels: {
                    enabled: true
                  }
                }
              },
              series: this.seriesChartSubCode11,
              exporting: {
                buttons: {
                  contextButton: {
                    menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                  },
                },
                enabled: true,
              },

            });

            // chart 2-02
            Highcharts.chart('chart202', {
              chart: {
                type: 'line',
                backgroundColor: '#f2f4f8'
              },
              credits: {
                enabled: false
              },
              title: {
                text: `Compare (+/-) from ${this.categoriesChartSubCode11[0]} to ${this.categoriesChartSubCode11[this.categoriesChartSubCode11.length - 1]}`,
                style: {
                  fontWeight: 'bold',
                }
              },
              xAxis: {
                categories: this.categoriesChartSubCode11
              },
              yAxis: {
                title: {
                  text: null
                }
              },
              plotOptions: {
                line: {
                  dataLabels: {
                    enabled: true
                  },
                  enableMouseTracking: true
                }
              },
              series: seriesChart202,
              exporting: {
                buttons: {
                  contextButton: {
                    menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                  },
                },
                enabled: true,
              },
            });
            this.showChart2 = true
          }
        }

      }
    }

  }

  searchThongTinNhanVien() {
    var data = {
      searchKhuVucLamViec: this.searchKhuVucLamViec.value,
      searchPhongBan: this.searchPhongBan.value,
      searchChucVu: this.searchChucVu.value,
      searchSubCode1: this.searchSubCode1.value,
      searchGioiTinh: this.searchGioiTinh.value,
      searchTrinhDoHocVan: this.searchTrinhDoHocVan.value,
      searchTrinhDoChuyenMon: this.searchTrinhDoChuyenMon.value,
      searchTrangThai: this.searchTrangThai.value,
      searchNgayVaoTu: this.searchNgayVaoTu.value,
      searchNgayVaoDen: this.searchNgayVaoDen.value,
      searchTuoiTu: this.searchTuoiTu.value,
      searchTuoiDen: this.searchTuoiDen.value,
      searchThamNienTu: this.searchThamNienTu.value,
      searchThamNienDen: this.searchThamNienDen.value,
    }

    this.myTable.filter(data.searchKhuVucLamViec ?.provinceId, 'provinceId', 'equals');
    this.myTable.filter(data.searchPhongBan ?.organizationId, 'organizationId', 'equals');
    this.myTable.filter(data.searchChucVu ?.positionId, 'positionId', 'equals');
    this.myTable.filter(data.searchSubCode1 ?.value, 'subCode1', 'equals');
    this.myTable.filter(data.searchGioiTinh ?.code, 'gender', 'equals');
    this.myTable.filter(data.searchTrinhDoHocVan ?.categoryId, 'bangCapCaoNhatDatDuocId', 'equals');
    this.myTable.filter(data.searchTrinhDoChuyenMon ?.value, 'kyNangTayNghes', 'equals');
    this.myTable.filter(data.searchTrangThai ?.id, 'trangThaiId', 'equals');
    this.myTable.filter(data.searchNgayVaoTu, 'startDateMayChamCongTu', 'gte');
    this.myTable.filter(data.searchNgayVaoDen, 'startDateMayChamCongDen', 'lte');
    this.myTable.filter(data.searchTuoiTu, 'doTuoiTu', 'gte');
    this.myTable.filter(data.searchTuoiDen, 'doTuoiDen', 'lte');
    this.myTable.filter(data.searchThamNienTu, 'thamNienTu', 'gte');
    this.myTable.filter(data.searchThamNienDen, 'thamNienDen', 'lte');

  }

  refreshFilter() {
    this.filterGlobal = '';
    this.searchForm.reset();
    this.searchThongTinNhanVien();
    this.myTable ?.reset();
    this.getMasterData();
  }


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



  async exportExcel() {
    let title = `Danh sách nhân sự`;
    let workBook = new Workbook();
    let worksheet = workBook.addWorksheet(title);
    worksheet.addRow([]);
    /* title */
    let dataHeaderMain = ["", "Danh sách nhân sự".toUpperCase()];
    let headerMain = worksheet.addRow(dataHeaderMain);
    headerMain.font = { size: 18, bold: true };
    headerMain.getCell(2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    worksheet.mergeCells(`B${2}:E${2}`);
    worksheet.addRow([]);
    let excel = [
      { name: 'STT', key: 'index' },
      { name: 'Emp ID', key: 'employeeCode' },
      { name: 'Name', key: 'employeeName' },
      { name: 'GRADE TESTING', key: 'capBacName' },
      { name: 'Working location', key: 'provinceName' },
      { name: 'Department', key: 'organizationName' },
      { name: 'Position', key: 'positionName' },
      { name: 'DEPT CODE', key: 'deptCode' },
      { name: 'SUB-CODE1', key: 'subCode1Name' },
      { name: 'SUB-CODE2', key: 'subCode2Name' },
      { name: 'Gender', key: 'genderName' },
      { name: 'Education', key: 'bangCapCaoNhatDatDuocName' },
      { name: 'Group Major', key: 'kyNangTayNghesName' },
      { name: 'State', key: 'trangThaiName' },
      { name: 'Start date', key: 'startDateMayChamCong' },
      { name: 'Birthday', key: 'dateOfBirth' },
    ]

    let dataHeaderRow1: Array<string> = excel.map(x => x.name);
    let headerRow1 = worksheet.addRow(dataHeaderRow1);
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
    headerRow1.height = 30;
    dataHeaderRow1.forEach((item, index) => {
      if (index + 1 < dataHeaderRow1.length + 1) {
        headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow1.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      }
    });

    //Đổ data
    let data: Array<any> = [];
    let key = excel.map(x => x.key);
    this.listEmployee ?.forEach((item, index) => {
      let array_Row: Array<any> = [];
      key.forEach((itemKey, indexKey) => {
        if (itemKey == 'index') {
          array_Row[indexKey + 1] = index + 1;
        } else {
          if (itemKey == 'dateOfBirth' || itemKey == 'startDateMayChamCong') {
            array_Row[indexKey + 1] = item[itemKey] != null ? new Date(item[itemKey]).toLocaleDateString("en-US") : '';
          } else {
            array_Row[indexKey + 1] = item[itemKey];
          }
        }
      })
      data.push(array_Row);
    });

    //Kẻ viền bảng
    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman', size: 11 };
      for (let i = 1; i <= key.length; i++) {
        row.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(i).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        worksheet.getColumn(i).width = 20;
      }
      row.height = 30;
    });
    this.exportToExel(workBook, title);
  }


  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}

function ValidateTime(ValidateTime: FormGroup) {
  let tuThang = new Date(ValidateTime.get('startTimeBieuDo').value).getMonth()
  let denThang = new Date(ValidateTime.get('endTimeBieuDo').value).getMonth()
  if (tuThang > denThang) {
    return { saiThongTin: true };
  }
  return null;
}
