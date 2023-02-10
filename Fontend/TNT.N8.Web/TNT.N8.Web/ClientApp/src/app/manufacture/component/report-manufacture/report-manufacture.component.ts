import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { ManufactureService } from '../../services/manufacture.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService, SortEvent } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import * as moment from 'moment';
import * as $ from 'jquery';
import 'moment/locale/pt-br';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { DatePipe } from '@angular/common';
import { formatDate } from '@angular/common';
import { formatNumber } from '@angular/common';
import { DialogService } from 'primeng';

interface Month {
  label: string,
  value: number
}

class resultMonthModel {
  startMonth: number;
  endMonth: number;
}

class techniqueRequest {
  techniqueRequestId: string;
  techniqueName: string;
  techniqueRequestCode: string;
}

class reportType {
  label: string;
  value: number;
}

//ca ngay/dem
class shiftType {
  label: string;
  value: number;
}

class yearModel {
  label: string;
  value: number;
}

class periodModel {
  id: string;
  name: string;
}

//model dùng để build template excel
class columnModel {
  column1: string;
  column2: string;
}

class searchFormModel {
  techniqueRequestId: string;
  shift: number;
  fromDate: Date;
  toDate: Date;
}

class reportManuFactureByDayModel {
  productionOrderCode: string;//Lệnh số
  customerName: string; //khách hàng
  productName: string; //sản phẩm
  productThickness: number; //độ dày
  quantity: number; //số lượng
  totalAreaThick: number; //tổng m2 dày
  totalAreaThin: number; //tổng m2 mỏng
  totalAreaEspeciallyThick: number; //tổng m2 dày đặc biệt
  totalAreaEspeciallyThin: number; //tổng m2 mỏng đặc biệt
  totalAreaOriginalThick: number; //tổng m2 nguyên khối dày
  totalAreaOriginalThin: number; //tổng m2 nguyên khối đặc biệt
  totalAreaBoreholeThick: number; //tổng khoan khoét dày
  totalAreaBoreholeThin: number; //tổng khoan khoét mỏng
  sandblasting: string; //phun cat - free text
  washglasses: string; //rua kinh - free text
  endDate: Date;
  note: string;
}

class sumaryReportManuFactureByDayModel {
  quantity: number; //số lượng
  totalAreaThick: number; //tổng m2 dày
  totalAreaThin: number; //tổng m2 mỏng
  totalAreaEspeciallyThick: number; //tổng m2 dày đặc biệt
  totalAreaEspeciallyThin: number; //tổng m2 mỏng đặc biệt
  totalAreaBoreholeThick: number; //tổng số m2 dày khoan khoét
  totalAreaBoreholeThin: number; //tổng số m2 mỏng khoan khoét
  totalAreaOriginalThick: number; //tong so m2 nguyen khoi day
  totalAreaOriginalThin: number; //tong so m2 nguyen khoi mong
  totalArea: number;

  percentCompleteToday: number; //tỉ lệ % hoành thành trong ngày
  percentCompleteYesterday: number; //tỉ lệ % hoành thành hôm trước

  constructor() {
    this.quantity = 0;
    this.totalAreaThick = 0;
    this.totalAreaThin = 0;
    this.totalAreaEspeciallyThick = 0;
    this.totalAreaEspeciallyThin = 0;
    this.totalAreaBoreholeThick = 0;
    this.totalAreaBoreholeThin = 0;
    this.totalAreaOriginalThick = 0;
    this.totalAreaOriginalThin = 0;
    this.totalArea = 0;

    this.percentCompleteToday = 0;
    this.percentCompleteYesterday = 0;
  }
}

class reportManuFactureByMonthModel {
  day: Date; //ngay
  shift: yearModel; //ca
  shiftValue: number;
  totalAreaThick: number; //tong m2 day
  totalAreaThin: number; //tong m2 mong
  totalAreaEspeciallyThick: number; //tổng m2 dày đặc biệt
  totalAreaEspeciallyThin: number; //tổng m2 mỏng đặc biệt
  totalAreaBoreholeThick: number;
  totalAreaBoreholeThin: number;
  totalAreaOriginalThick: number;
  totalAreaOriginalThin: number;
  totalDayShift: number; //tong ca ngay
  totalNightShift: number; //tong ca dem
  labor: number; //nhan cong

  constructor() {
    this.totalAreaThick = 0;
    this.totalAreaThin = 0;
    this.totalAreaEspeciallyThick = 0;
    this.totalAreaEspeciallyThin = 0;
    this.totalAreaBoreholeThick = 0;
    this.totalAreaBoreholeThin = 0;
    this.totalAreaOriginalThick = 0;
    this.totalAreaOriginalThin = 0;
    this.totalDayShift = 0;
    this.totalNightShift = 0;
    this.labor = 0;
  }
}

class sumaryReportManuFactureByMonthModel {
  totalAreaThick: number; //tong m2 day
  totalAreaThin: number; //tong m2 mong
  totalAreaEspeciallyThick: number; //tổng m2 dày đặc biệt
  totalAreaEspeciallyThin: number; //tổng m2 mỏng đặc biệt
  totalAreaBoreholeThick: number;
  totalAreaBoreholeThin: number;
  totalAreaOriginalThick: number;
  totalAreaOriginalThin: number;
  totalDayShift: number; //tong ca ngay
  totalNightShift: number; //tong ca dem
  totalLabor: number; //nhan cong

  totalArea: number; //tong m2
  totalAreaPerLabor: number; //tong m2 trung binh/nguoi
  constructor() {
    this.totalAreaThick = 0;
    this.totalAreaThin = 0;
    this.totalAreaEspeciallyThick = 0;
    this.totalAreaEspeciallyThin = 0;
    this.totalAreaBoreholeThick = 0;
    this.totalAreaBoreholeThin = 0;
    this.totalAreaOriginalThick = 0;
    this.totalAreaOriginalThin = 0;
    this.totalDayShift = 0;
    this.totalNightShift = 0;
    this.totalLabor = 0;
    this.totalArea = 0;
    this.totalAreaPerLabor = 0;
  }
}

class reportManuFactureByYearModel {
  month: Month;
  monthValue: number;
  totalAreaThick: number; //tong m2 day
  totalAreaThin: number; //tong m2 mong
  totalAreaEspeciallyThick: number; //tổng m2 dày đặc biệt
  totalAreaEspeciallyThin: number; //tổng m2 mỏng đặc biệt
  totalAreaBoreholeThick: number;
  totalAreaBoreholeThin: number;
  totalAreaOriginalThick: number;
  totalAreaOriginalThin: number;
  totalArea: number;
  constructor() {
    this.totalAreaThick = 0;
    this.totalAreaThin = 0;
    this.totalAreaEspeciallyThick = 0;
    this.totalAreaEspeciallyThin = 0;
    this.totalAreaBoreholeThick = 0;
    this.totalAreaBoreholeThin = 0;
    this.totalAreaOriginalThick = 0;
    this.totalAreaOriginalThin = 0;
    this.totalArea = 0;
  }
}

class sumaryReportManuFactureByYearModel {
  totalAreaThick: number; //tong m2 day
  totalAreaThin: number; //tong m2 mong

  totalAreaEspeciallyThick: number; //tổng m2 dày đặc biệt
  totalAreaEspeciallyThin: number; //tổng m2 mỏng đặc biệt

  totalAreaBoreholeThick: number;
  totalAreaBoreholeThin: number;

  totalAreaOriginalThick: number;
  totalAreaOriginalThin: number;

  totalArea: number; //tong m2
  constructor() {
    this.totalAreaThick = 0;
    this.totalAreaThin = 0;
    this.totalAreaEspeciallyThick = 0;
    this.totalAreaEspeciallyThin = 0;
    this.totalAreaBoreholeThick = 0;
    this.totalAreaBoreholeThin = 0;
    this.totalAreaOriginalThick = 0;
    this.totalAreaOriginalThin = 0;
    this.totalArea = 0;
  }
}
@Component({
  selector: 'app-report-manufacture',
  templateUrl: './report-manufacture.component.html',
  styleUrls: ['./report-manufacture.component.css'],
  providers: [DatePipe]
})
export class ReportManufactureComponent implements OnInit {
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  loading: boolean = false;
  listMonth: Array<Month> = [
    {
      label: 'Tháng 1', value: 1
    },
    {
      label: 'Tháng 2', value: 2
    },
    {
      label: 'Tháng 3', value: 3
    },
    {
      label: 'Tháng 4', value: 4
    },
    {
      label: 'Tháng 5', value: 5
    },
    {
      label: 'Tháng 6', value: 6
    },
    {
      label: 'Tháng 7', value: 7
    },
    {
      label: 'Tháng 8', value: 8
    },
    {
      label: 'Tháng 9', value: 9
    },
    {
      label: 'Tháng 10', value: 10
    },
    {
      label: 'Tháng 11', value: 11
    },
    {
      label: 'Tháng 12', value: 12
    }
  ];
  //hardcode
  listReportType: Array<reportType> = [
    { label: "Báo cáo ngày", value: 1 },
    { label: "Báo cáo tháng", value: 2 },
    { label: "Báo cáo năm", value: 3 },
    { label: "Báo cáo tháng kính bổ sung", value: 4 },
  ];
  listShiftType: Array<shiftType> = [
    { label: "Ca ngày", value: 1 },
    { label: "Ca đêm", value: 2 }
  ];
  // table
  innerWidth: number = 0;
  //masterdata;
  listTechniqueRequest: Array<techniqueRequest> = [];
  //export data
  //bao cao theo ngay
  listReportManuFactureByDay: Array<reportManuFactureByDayModel> = [];
  // listReportManuFactureByDayRemain:  Array<reportManuFactureByDayModel> = [];
  sumaryReportManuFactureByDay: sumaryReportManuFactureByDayModel = new sumaryReportManuFactureByDayModel();
  // sumaryReportManuFactureByDayRemain: sumaryReportManuFactureByDayModel = new sumaryReportManuFactureByDayModel();
  //bao cao theo thang
  listReportManuFactureByMonth: Array<reportManuFactureByMonthModel> = [];
  sumaryReportManuFactureByMonth: sumaryReportManuFactureByMonthModel = new sumaryReportManuFactureByMonthModel();
  //bao cao theo nam
  listReportManuFactureByYear: Array<reportManuFactureByYearModel> = [];
  sumaryReportManuFactureByYear: sumaryReportManuFactureByYearModel = new sumaryReportManuFactureByYearModel();
  //search
  searchType: number = 1; //lưu loại báo cáo để hiện filter phù hợp - mặc định báo cáo ngày
  // searchForm: FormGroup;
  searchByDayForm: FormGroup;
  searchByMonthForm: FormGroup;
  searchByYearForm: FormGroup;
  searchByMonthAdditionalForm: FormGroup;

  currentYear: number = new Date().getFullYear();
  minYear: number = this.currentYear - 10;
  listyears: Array<yearModel> = [];
  periods: { id: string, name: string, value: number }[] = [
    // { id: "Q1", name: "Quý 1" },
    // { id: "Q2", name: "Quý 2" },
    // { id: "Q3", name: "Quý 3" },
    // { id: "Q4", name: "Quý 4" },
    { id: "T1", name: "Tháng 1", value: 1 },
    { id: "T2", name: "Tháng 2", value: 2 },
    { id: "T3", name: "Tháng 3", value: 3 },
    { id: "T4", name: "Tháng 4", value: 4 },
    { id: "T5", name: "Tháng 5", value: 5 },
    { id: "T6", name: "Tháng 6", value: 6 },
    { id: "T7", name: "Tháng 7", value: 7 },
    { id: "T8", name: "Tháng 8", value: 8 },
    { id: "T9", name: "Tháng 9", value: 9 },
    { id: "T10", name: "Tháng 10", value: 10 },
    { id: "T11", name: "Tháng 11", value: 11 },
    { id: "T12", name: "Tháng 12", value: 12 },
    { id: "CUS", name: "Tùy chọn", value: 13 }
  ];

  //template excel
  //listExcelColumn: Array<columnModel> = [];
  constructor(private datePipe: DatePipe,
    private router: Router,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private manufactureService: ManufactureService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private renderer: Renderer2,
    private confirmationService: ConfirmationService,) {
    this.innerWidth = window.innerWidth;
    $("body").addClass("sidebar-collapse");
  }

  async  ngOnInit() {
    let resource = "man/manufacture/report/manufacture";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }

    this.getYears();
    this.initForm();
    this.getMasterdata();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  initForm() {
    this.searchByDayForm = new FormGroup({
      'ReportType': new FormControl(null, [Validators.required]),
      'TechniqueRequest': new FormControl(null, [Validators.required]),
      'ShiftType': new FormControl(null, [Validators.required]),
      'Date': new FormControl(null, [Validators.required]),
    });
    this.searchByMonthForm = new FormGroup({
      'ReportType': new FormControl(null, [Validators.required]),
      'TechniqueRequest': new FormControl(null, [Validators.required]),
      'Year': new FormControl(null),
      'Period': new FormControl(null),
      'FromDate': new FormControl(null, [Validators.required]),
      'ToDate': new FormControl(null, [Validators.required]),
    });
    this.searchByYearForm = new FormGroup({
      'ReportType': new FormControl(null, [Validators.required]),
      'TechniqueRequest': new FormControl(null, [Validators.required]),
      'Year': new FormControl(null, [Validators.required]),
    });

    this.searchByMonthAdditionalForm = new FormGroup({
      'ReportType': new FormControl(null, [Validators.required]),
      'TechniqueRequest': new FormControl(null, [Validators.required]),
      'Year': new FormControl(null),
      'Period': new FormControl(null),
      'FromDate': new FormControl(null, [Validators.required]),
      'ToDate': new FormControl(null, [Validators.required]),
    });

    // this.searchByDayForm.get("ReportType").setValue(this.listReportType[0]);
    this.resetSearchByDayForm();
  }

  resetSearchByDayForm() {
    this.searchByDayForm.reset();
    let currentSearchType = this.listReportType.find(e => e.value == this.searchType);
    this.searchByDayForm.get("ReportType").setValue(currentSearchType);
    let currentTime = new Date();
    this.searchByDayForm.get("Date").setValue(currentTime);
  }

  resetSearchByMonthForm() {
    this.searchByMonthForm.reset();
    let currentSearchType = this.listReportType.find(e => e.value == this.searchType);
    this.searchByMonthForm.get("ReportType").setValue(currentSearchType);
    let currentTime = new Date();
    let currentYear = this.listyears.find(e => e.value == currentTime.getFullYear());
    this.searchByMonthForm.get("Year").setValue(currentYear);

    // let currentMonth = this.periods.find(e => e.value == (currentTime.getMonth() + 1));
    // this.searchByMonthForm.get("Period").setValue(currentMonth);
  }

  resetSearchByMonthAdditionalForm() {
    this.searchByMonthAdditionalForm.reset();
    let currentSearchType = this.listReportType.find(e => e.value == this.searchType);
    this.searchByMonthAdditionalForm.get("ReportType").setValue(currentSearchType);

    let currentTime = new Date();
    let currentYear = this.listyears.find(e => e.value == currentTime.getFullYear());
    this.searchByMonthAdditionalForm.get("Year").setValue(currentYear);

    // let currentMonth = this.periods.find(e => e.value == (currentTime.getMonth() + 1));
    // this.searchByMonthAdditionalForm.get("Period").setValue(currentMonth);
  }

  resetSearchByYearForm() {
    this.searchByYearForm.reset();
    let currentSearchType = this.listReportType.find(e => e.value == this.searchType);
    this.searchByYearForm.get("ReportType").setValue(currentSearchType);

    let currentTime = new Date();
    let currentYear = this.listyears.find(e => e.value == currentTime.getFullYear());
    this.searchByYearForm.get("Year").setValue(currentYear);
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.manufactureService.getDataReportManufacture();
    this.loading = false;
    if (result.statusCode == 200) {
      this.listTechniqueRequest = result.listTechniqueRequest;
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  changeReportType(event: any) {
    let selectedReportType: reportType = event.value;
    if (!selectedReportType) return;
    this.searchType = selectedReportType.value;
    //reset all form
    this.resetSearchByDayForm();
    this.resetSearchByMonthForm();
    this.resetSearchByYearForm();
    this.resetSearchByMonthAdditionalForm();
  }

  changeYearReportByMonth(event: any) {
    this.searchByMonthForm.get('Period').reset();
    this.searchByMonthForm.get('FromDate').reset();
    this.searchByMonthForm.get('ToDate').reset();

    let selectedYear: yearModel = event.value;
    if (!selectedYear) return;

    let startDate = new Date(selectedYear.value, 0, 1);
    let endDate = new Date(selectedYear.value, 12, 0);
    this.searchByMonthForm.get('FromDate').setValue(startDate);
    this.searchByMonthForm.get('ToDate').setValue(endDate);
  }

  changeYearReportByMonthAdditional(event: any) {
    this.searchByMonthAdditionalForm.get('Period').reset();
    this.searchByMonthAdditionalForm.get('FromDate').reset();
    this.searchByMonthAdditionalForm.get('ToDate').reset();

    let selectedYear: yearModel = event.value;
    if (!selectedYear) return;

    let startDate = new Date(selectedYear.value, 0, 1);
    let endDate = new Date(selectedYear.value, 12, 0);
    this.searchByMonthAdditionalForm.get('FromDate').setValue(startDate);
    this.searchByMonthAdditionalForm.get('ToDate').setValue(endDate);
  }

  changePeriodReportByMonth(event: any) {
    let year: yearModel = this.searchByMonthForm.get('Year').value;
    this.searchByMonthForm.get('FromDate').reset();
    this.searchByMonthForm.get('ToDate').reset();
    if (!year) {
      this.showToast('error', 'Thông báo', 'Chọn năm');
      return;
    }
    let selectedPeriod: periodModel = event.value;
    if (!selectedPeriod) return;

    if (selectedPeriod.id === "CUS") {
      this.searchByMonthForm.get('Year').reset();
      return;
    }

    let _getMonth = this.getMonth(selectedPeriod);

    let startDate = new Date(year.value, _getMonth.startMonth, 1);
    let endDate = new Date(year.value, _getMonth.endMonth + 1, 0);

    this.searchByMonthForm.get('FromDate').setValue(startDate);
    this.searchByMonthForm.get('ToDate').setValue(endDate);
  }

  changePeriodReportByMonthAdditional(event: any) {
    let year: yearModel = this.searchByMonthAdditionalForm.get('Year').value;
    this.searchByMonthAdditionalForm.get('FromDate').reset();
    this.searchByMonthAdditionalForm.get('ToDate').reset();
    if (!year) {
      this.showToast('error', 'Thông báo', 'Chọn năm');
      return;
    }
    let selectedPeriod: periodModel = event.value;
    if (!selectedPeriod) return;

    if (selectedPeriod.id === "CUS") {
      this.searchByMonthAdditionalForm.get('Year').reset();
      return;
    }

    let _getMonth = this.getMonth(selectedPeriod);

    let startDate = new Date(year.value, _getMonth.startMonth, 1);
    let endDate = new Date(year.value, _getMonth.endMonth + 1, 0);

    this.searchByMonthAdditionalForm.get('FromDate').setValue(startDate);
    this.searchByMonthAdditionalForm.get('ToDate').setValue(endDate);
  }

  getMonth(period: any): resultMonthModel {
    let result = new resultMonthModel();
    switch (period.id) {
      case "Q1":
        result.startMonth = 0;
        result.endMonth = 2;
        break;
      case "Q2":
        result.startMonth = 3;
        result.endMonth = 5;
        break;
      case "Q3":
        result.startMonth = 6;
        result.endMonth = 8;
        break;
      case "Q4":
        result.startMonth = 9;
        result.endMonth = 11;
        break;
      case "T1":
        result.startMonth = 0;
        result.endMonth = 0;
        break;
      case "T2":
        result.startMonth = 1;
        result.endMonth = 1;
        break;
      case "T3":
        result.startMonth = 2;
        result.endMonth = 2;
        break;
      case "T4":
        result.startMonth = 3;
        result.endMonth = 3;
        break;
      case "T5":
        result.startMonth = 4;
        result.endMonth = 4;
        break;
      case "T6":
        result.startMonth = 5;
        result.endMonth = 5;
        break;
      case "T7":
        result.startMonth = 6;
        result.endMonth = 6;
        break;
      case "T8":
        result.startMonth = 7;
        result.endMonth = 7;
        break;
      case "T9":
        result.startMonth = 8;
        result.endMonth = 8;
        break;
      case "T10":
        result.startMonth = 9;
        result.endMonth = 9;
        break;
      case "T11":
        result.startMonth = 10;
        result.endMonth = 10;
        break;
      case "T12":
        result.startMonth = 11;
        result.endMonth = 11;
        break;
    }

    return result;
  }

  async getReport() {
    let reportType: reportType;
    switch (this.searchType) {
      case 1:
        reportType = this.searchByDayForm.get('ReportType').value;
        break;
      case 2:
        reportType = this.searchByMonthForm.get('ReportType').value;
        break;
      case 3:
        reportType = this.searchByYearForm.get('ReportType').value;
        break;
      case 4:
        reportType = this.searchByMonthForm.get('ReportType').value;
        break;
      default:
        break;
    }

    switch (reportType.value) {
      case 1:
        if (!this.searchByDayForm.valid) {
          Object.keys(this.searchByDayForm.controls).forEach(key => {
            if (!this.searchByDayForm.controls[key].valid) {
              this.searchByDayForm.controls[key].markAsTouched();
            }
          });
          return;
        }
        //bao cao theo ngay
        let searchFormByDay: searchFormModel = this.getSearchFormByDay();
        this.loading = true;
        let result: any = await this.manufactureService.getReportManuFactureByDay(searchFormByDay.techniqueRequestId, searchFormByDay.shift, searchFormByDay.fromDate, searchFormByDay.toDate);
        this.loading = false;
        if (result.statusCode == 200) {
          this.listReportManuFactureByDay = result.listReportManuFactureByDay;
          this.sumaryReportManuFactureByDay = result.sumaryReportManuFactureByDay;
          // this.listReportManuFactureByDayRemain = result.listReportManuFactureByDayRemain;
          // this.sumaryReportManuFactureByDayRemain = result.sumaryReportManuFactureByDayRemain;
          let techniqueRequest = this.listTechniqueRequest.find(e => e.techniqueRequestId == searchFormByDay.techniqueRequestId);
          let currentShift: shiftType = this.searchByDayForm.get('ShiftType').value;
          let shift = this.listShiftType.find(e => e == currentShift);
          let shiftName = "";
          if (shift.value == 1) {
            shiftName = "NGÀY";
          } else if (shift.value == 2) {
            shiftName = "ĐÊM";
          }
          this.exportReport(techniqueRequest, shiftName, searchFormByDay);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
        break;
      case 2:
        if (!this.searchByMonthForm.valid) {
          Object.keys(this.searchByMonthForm.controls).forEach(key => {
            if (!this.searchByMonthForm.controls[key].valid) {
              this.searchByMonthForm.controls[key].markAsTouched();
            }
          });
          return;
        }
        // //bao cao theo thang
        this.listReportManuFactureByMonth = [];
        let searchFormByMonth: searchFormModel = this.getSearchFormByMonth();
        this.loading = true;
        let resultMonth: any = await this.manufactureService.getReportManuFactureByMonth(searchFormByMonth.techniqueRequestId, searchFormByMonth.fromDate, searchFormByMonth.toDate);
        this.loading = false;
        if (resultMonth.statusCode == 200) {
          this.listReportManuFactureByMonth = resultMonth.listReportManuFactureByMonth;
          //map shiftValue với model shift
          this.listReportManuFactureByMonth.forEach(e => {
            e.shift = this.listShiftType.find(shift => shift.value == e.shiftValue);
          });
          this.sumaryReportManuFactureByMonth = resultMonth.sumaryReportManuFactureByMonth;
          let techniqueRequestByMonth = this.listTechniqueRequest.find(e => e.techniqueRequestId == searchFormByMonth.techniqueRequestId);
          this.exportReportByMonth(techniqueRequestByMonth, searchFormByMonth);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: resultMonth.messageCode };
          this.showMessage(msg);
        }
        break;
      case 3:
        if (!this.searchByYearForm.valid) {
          Object.keys(this.searchByYearForm.controls).forEach(key => {
            if (!this.searchByYearForm.controls[key].valid) {
              this.searchByYearForm.controls[key].markAsTouched();
            }
          });
          return;
        }
        // //bao cao theo nam
        this.listReportManuFactureByYear = [];
        let searchFormByYear: searchFormModel = this.getSearchFormByYear();
        this.loading = true;
        let resultYear: any = await this.manufactureService.getReportManuFactureByYear(searchFormByYear.techniqueRequestId, searchFormByYear.fromDate, searchFormByYear.toDate);
        this.loading = false;
        if (resultYear.statusCode == 200) {
          this.listReportManuFactureByYear = resultYear.listReportManuFactureByYear;
          //map shiftValue với model shift
          this.listReportManuFactureByYear.forEach(e => {
            e.month = this.listMonth.find(month => month.value == e.monthValue);
          });
          this.sumaryReportManuFactureByYear = resultYear.sumaryReportManuFactureByYear;
          let techniqueRequestByYear = this.listTechniqueRequest.find(e => e.techniqueRequestId == searchFormByYear.techniqueRequestId);
          this.exportReportByYear(techniqueRequestByYear, searchFormByYear);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: resultYear.messageCode };
          this.showMessage(msg);
        }
        break;
      case 4:
        if (!this.searchByMonthAdditionalForm.valid) {
          Object.keys(this.searchByMonthAdditionalForm.controls).forEach(key => {
            if (!this.searchByMonthAdditionalForm.controls[key].valid) {
              this.searchByMonthAdditionalForm.controls[key].markAsTouched();
            }
          });
          return;
        }
        // //bao cao theo nam
        this.listReportManuFactureByYear = [];
        let searchFormByMonthAdditional: searchFormModel = this.getSearchFormByMonthAdditional();
        this.loading = true;
        let resultMonthAdditional: any = await this.manufactureService.getReportManuFactureByMonthAdditional(searchFormByMonthAdditional.techniqueRequestId, searchFormByMonthAdditional.fromDate, searchFormByMonthAdditional.toDate);
        this.loading = false;
        if (resultMonthAdditional.statusCode == 200) {
          this.listReportManuFactureByMonth = resultMonthAdditional.listReportManuFactureByMonth;
          //map shiftValue với model shift
          this.listReportManuFactureByMonth.forEach(e => {
            e.shift = this.listShiftType.find(shift => shift.value == e.shiftValue);
          });
          this.sumaryReportManuFactureByMonth = resultMonthAdditional.sumaryReportManuFactureByMonth;
          let techniqueRequestByMonthAdditional = this.listTechniqueRequest.find(e => e.techniqueRequestId == searchFormByMonthAdditional.techniqueRequestId);
          this.exportReportByMonth(techniqueRequestByMonthAdditional, searchFormByMonthAdditional);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: resultMonthAdditional.messageCode };
          this.showMessage(msg);
        }
        break;
      default:
        break;
    }
  }

  getSearchFormByDay(): searchFormModel {
    let searchForm: searchFormModel = new searchFormModel();
    let techniqueRequest: techniqueRequest = this.searchByDayForm.get('TechniqueRequest').value;
    let shift: shiftType = this.searchByDayForm.get('ShiftType').value;
    let date: Date = this.searchByDayForm.get('Date').value;

    searchForm.techniqueRequestId = techniqueRequest ? techniqueRequest.techniqueRequestId : null;
    searchForm.shift = shift ? shift.value : null;
    searchForm.fromDate = GetStartDay(date);
    searchForm.toDate = GetEndDay(date);
    return searchForm;
  }

  getSearchFormByMonth(): searchFormModel {
    let searchForm: searchFormModel = new searchFormModel();
    let techniqueRequest: techniqueRequest = this.searchByMonthForm.get('TechniqueRequest').value;
    let fromDate: Date = this.searchByMonthForm.get('FromDate').value;
    let toDate: Date = this.searchByMonthForm.get('ToDate').value;
    searchForm.techniqueRequestId = techniqueRequest ? techniqueRequest.techniqueRequestId : null;
    searchForm.fromDate = fromDate ? ConvertToUTCTime(fromDate) : null;
    searchForm.toDate = toDate ? ConvertToUTCTime(toDate) : null;
    return searchForm;
  }

  getSearchFormByMonthAdditional(): searchFormModel {
    let searchForm: searchFormModel = new searchFormModel();
    let techniqueRequest: techniqueRequest = this.searchByMonthAdditionalForm.get('TechniqueRequest').value;
    let fromDate: Date = this.searchByMonthAdditionalForm.get('FromDate').value;
    let toDate: Date = this.searchByMonthAdditionalForm.get('ToDate').value;
    searchForm.techniqueRequestId = techniqueRequest ? techniqueRequest.techniqueRequestId : null;
    searchForm.fromDate = fromDate ? ConvertToUTCTime(fromDate) : null;
    searchForm.toDate = toDate ? ConvertToUTCTime(toDate) : null;
    return searchForm;
  }

  getSearchFormByYear(): searchFormModel {
    let searchForm: searchFormModel = new searchFormModel();
    let techniqueRequest: techniqueRequest = this.searchByYearForm.get('TechniqueRequest').value;

    let year: yearModel = this.searchByYearForm.get('Year').value;
    let fromDate = new Date(year.value, 0, 1);
    let toDate = new Date(year.value, 11, 31);

    searchForm.techniqueRequestId = techniqueRequest ? techniqueRequest.techniqueRequestId : null;
    searchForm.fromDate = fromDate ? ConvertToUTCTime(fromDate) : null;
    searchForm.toDate = toDate ? ConvertToUTCTime(toDate) : null;
    return searchForm;
  }

  exportReport(techRequest: techniqueRequest, shiftName: string, searchFormByDay: searchFormModel) {
    let title = `BÁO CÁO SẢN LƯỢNG ${shiftName}`;
    let workBook = new Workbook();
    let worksheet = workBook.addWorksheet(title);
    /* title */
    let titleRow = worksheet.addRow([title]);
    titleRow.font = { family: 4, size: 16, bold: true };
    titleRow.height = 25;
    titleRow.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A${titleRow.number}:L${titleRow.number}`);
    titleRow.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, right: { style: "thin" } };
    //desc row
    let date = this.transformDate(searchFormByDay.fromDate);

    let desc = `Ngày ${searchFormByDay.fromDate.getDate()} tháng ${searchFormByDay.fromDate.getMonth() + 1} năm ${searchFormByDay.fromDate.getFullYear()}`;
    let descRow = worksheet.addRow([desc]);
    descRow.font = { family: 4, size: 12, italic: true };
    descRow.alignment = { vertical: 'middle', horizontal: 'right' };
    worksheet.mergeCells(`A${descRow.number}:L${descRow.number}`);
    //send row
    let send = `Gửi Tổ ${techRequest.techniqueName}`;
    let sendRow = worksheet.addRow([send]);
    sendRow.font = { family: 4, size: 12, bold: true };
    sendRow.height = 18;
    sendRow.alignment = { vertical: 'middle', horizontal: 'left' };
    worksheet.mergeCells(`A${sendRow.number}:L${sendRow.number}`);
    let listExcelColumns = this.getListExcelColumns(techRequest);
    let dataHeaderRow1: Array<string> = listExcelColumns.map(e => e.column1);
    let dataHeaderRow2: Array<string> = listExcelColumns.map(e => e.column2);
    let headerRow1 = worksheet.addRow(dataHeaderRow1);
    let headerRow2 = worksheet.addRow(dataHeaderRow2);
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
    headerRow2.font = { name: 'Time New Roman', size: 10, bold: true };
    //merge column
    switch (techRequest.techniqueRequestCode) {
      case "CAT":
        worksheet.mergeCells(`A${4}:A${5}`);
        worksheet.mergeCells(`B${4}:B${5}`);
        worksheet.mergeCells(`C${4}:C${5}`);
        worksheet.mergeCells(`D${4}:D${5}`);
        worksheet.mergeCells(`E${4}:E${5}`);
        worksheet.mergeCells(`F${4}:F${5}`);
        worksheet.mergeCells(`K${4}:K${5}`);
        worksheet.mergeCells(`L${4}:L${5}`);
        worksheet.mergeCells(`G${4}:H${4}`);
        worksheet.mergeCells(`I${4}:J${4}`);
        break;
      case "MAI":
        worksheet.mergeCells(`A${4}:A${5}`);
        worksheet.mergeCells(`B${4}:B${5}`);
        worksheet.mergeCells(`C${4}:C${5}`);
        worksheet.mergeCells(`D${4}:D${5}`);
        worksheet.mergeCells(`E${4}:E${5}`);
        worksheet.mergeCells(`F${4}:F${5}`);
        worksheet.mergeCells(`K${4}:K${5}`);
        worksheet.mergeCells(`L${4}:L${5}`);
        worksheet.mergeCells(`G${4}:H${4}`);
        worksheet.mergeCells(`I${4}:J${4}`);
        break;
      case "KHOAN":
        worksheet.mergeCells(`A${4}:A${5}`);
        worksheet.mergeCells(`B${4}:B${5}`);
        worksheet.mergeCells(`C${4}:C${5}`);
        worksheet.mergeCells(`D${4}:D${5}`);
        worksheet.mergeCells(`E${4}:E${5}`);
        worksheet.mergeCells(`F${4}:F${5}`);
        worksheet.mergeCells(`M${4}:M${5}`);
        worksheet.mergeCells(`N${4}:N${5}`);
        worksheet.mergeCells(`G${4}:H${4}`);
        worksheet.mergeCells(`I${4}:J${4}`);
        worksheet.mergeCells(`K${4}:L${4}`);
        break;
      case "TOI":
        worksheet.mergeCells(`A${4}:A${5}`);
        worksheet.mergeCells(`B${4}:B${5}`);
        worksheet.mergeCells(`C${4}:C${5}`);
        worksheet.mergeCells(`D${4}:D${5}`);
        worksheet.mergeCells(`E${4}:E${5}`);
        worksheet.mergeCells(`F${4}:F${5}`);
        worksheet.mergeCells(`K${4}:K${5}`);
        worksheet.mergeCells(`L${4}:L${5}`);
        worksheet.mergeCells(`M${4}:M${5}`);
        worksheet.mergeCells(`N${4}:N${5}`);
        worksheet.mergeCells(`G${4}:H${4}`);
        worksheet.mergeCells(`I${4}:J${4}`);
        break;
      case "HOP":
        worksheet.mergeCells(`A${4}:A${5}`);
        worksheet.mergeCells(`B${4}:B${5}`);
        worksheet.mergeCells(`C${4}:C${5}`);
        worksheet.mergeCells(`D${4}:D${5}`);
        worksheet.mergeCells(`E${4}:E${5}`);
        worksheet.mergeCells(`F${4}:F${5}`);
        worksheet.mergeCells(`K${4}:K${5}`);
        worksheet.mergeCells(`L${4}:L${5}`);
        worksheet.mergeCells(`M${4}:M${5}`);
        worksheet.mergeCells(`N${4}:N${5}`);
        worksheet.mergeCells(`G${4}:H${4}`);
        worksheet.mergeCells(`I${4}:J${4}`);
        break;
      case "DAN":
        worksheet.mergeCells(`A${4}:A${5}`);
        worksheet.mergeCells(`B${4}:B${5}`);
        worksheet.mergeCells(`C${4}:C${5}`);
        worksheet.mergeCells(`D${4}:D${5}`);
        worksheet.mergeCells(`E${4}:E${5}`);
        worksheet.mergeCells(`F${4}:F${5}`);
        worksheet.mergeCells(`O${4}:O${5}`);
        worksheet.mergeCells(`P${4}:P${5}`);
        worksheet.mergeCells(`G${4}:H${4}`);
        worksheet.mergeCells(`I${4}:J${4}`);
        worksheet.mergeCells(`K${4}:L${4}`);
        worksheet.mergeCells(`M${4}:N${4}`);
        break;
      default:
        break;
    }
    dataHeaderRow1.forEach((item, index) => {
      headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow1.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
    });
    headerRow1.height = 40;
    dataHeaderRow2.forEach((item, index) => {
      headerRow2.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow2.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
    });
    headerRow2.height = 40;
    //handle data for single report type
    let data: Array<any> = this.getDataReport(techRequest);
    data.forEach(e => {
      let row = worksheet.addRow(e);
      let totalColumns = listExcelColumns.length;
      for (let i = 1; i <= totalColumns; i++) {
        row.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }

      switch (techRequest.techniqueRequestCode) {
        case "CAT":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //index
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left' };//lệnh số
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left' };//khach hang
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left' };//san pham
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
          row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
          row.getCell(11).alignment = { vertical: 'middle', horizontal: 'center' };//ngay tra
          row.getCell(12).alignment = { vertical: 'middle', horizontal: 'left' };//note
          break;
        case "MAI":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //index
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left' };//lệnh số
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left' };//khach hang
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left' };//san pham
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
          row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
          row.getCell(11).alignment = { vertical: 'middle', horizontal: 'center' };//ngay tra
          row.getCell(12).alignment = { vertical: 'middle', horizontal: 'left' };//note
          break;
        case "KHOAN":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //index
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left' };//lệnh số
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left' };//khach hang
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left' };//san pham
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
          row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
          row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' };//khoan khoet day
          row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right' };//khoan khoet mong
          row.getCell(13).alignment = { vertical: 'middle', horizontal: 'center' };//ngay tra
          row.getCell(14).alignment = { vertical: 'middle', horizontal: 'left' };//note
          break;
        case "TOI":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //index
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left' };//lệnh số
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left' };//khach hang
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left' };//san pham
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
          row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
          row.getCell(11).alignment = { vertical: 'middle', horizontal: 'left' };//phun cat
          row.getCell(12).alignment = { vertical: 'middle', horizontal: 'left' };//rua kinh
          row.getCell(13).alignment = { vertical: 'middle', horizontal: 'center' };//ngay tra
          row.getCell(14).alignment = { vertical: 'middle', horizontal: 'left' };//note
          break;
        case "DAN":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //index
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left' };//lệnh số
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left' };//khach hang
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left' };//san pham
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
          row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
          row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 nguyen khoi day
          row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 nguyen khoi mong
          row.getCell(13).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 cat ha day
          row.getCell(14).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 cat ha mong
          row.getCell(15).alignment = { vertical: 'middle', horizontal: 'center' };//ngay tra
          row.getCell(16).alignment = { vertical: 'middle', horizontal: 'left' };//note
          break;
        case "HOP":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //index
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left' };//lệnh số
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left' };//khach hang
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left' };//san pham
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
          row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
          row.getCell(11).alignment = { vertical: 'middle', horizontal: 'left' };//phun cat
          row.getCell(12).alignment = { vertical: 'middle', horizontal: 'left' };//rua kinh
          row.getCell(13).alignment = { vertical: 'middle', horizontal: 'center' };//ngay tra
          row.getCell(14).alignment = { vertical: 'middle', horizontal: 'left' };//note
          break;
        default:
          break;
      }
    });


    /* sumary section */
    let sumaryRow: Array<any> = this.getSumaryDataReport(techRequest);
    sumaryRow.forEach((e, index) => {
      let row = worksheet.addRow(e);
      row.font = { family: 4, size: 12, bold: true };
      let totalColumns = listExcelColumns.length;
      for (let i = 1; i <= totalColumns; i++) {
        row.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }

      switch (techRequest.techniqueRequestCode) {
        case "CAT":
        case "MAI":
          worksheet.mergeCells(`A${row.number}:D${row.number}`);
          if (index == 1 || index == 2) worksheet.mergeCells(`F${row.number}:J${row.number}`);
          if (index == 0) {
            row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
            row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
            row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center' };//khach hang
            row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center' };//san pham

            row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
            row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
            row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
            row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
            row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
            row.getCell(11).alignment = { vertical: 'middle', horizontal: 'center' }; //tong
            row.getCell(12).alignment = { vertical: 'middle', horizontal: 'center' };//
            worksheet.mergeCells(`K${row.number}:L${row.number}`);
          }
          break;
        case "KHOAN":
          worksheet.mergeCells(`A${row.number}:D${row.number}`);
          if (index == 1 || index == 2) worksheet.mergeCells(`F${row.number}:L${row.number}`);
          if (index == 0) {
            row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
            row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
            row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center' };//khach hang
            row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center' };//san pham

            row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
            row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
            row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
            row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
            row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
            row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 day khoan khoet
            row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong khoan khoet
            row.getCell(13).alignment = { vertical: 'middle', horizontal: 'center' }; //tong
            row.getCell(14).alignment = { vertical: 'middle', horizontal: 'center' };//
            worksheet.mergeCells(`M${row.number}:N${row.number}`);
          }
          break;
        case "TOI":
        case "HOP":
          worksheet.mergeCells(`A${row.number}:D${row.number}`);
          if (index == 1 || index == 2) worksheet.mergeCells(`F${row.number}:L${row.number}`);
          if (index == 0) {
            row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
            row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
            row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center' };//khach hang
            row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center' };//san pham

            row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
            row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
            row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
            row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
            row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
            row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 day khoan khoet
            row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong khoan khoet
            row.getCell(13).alignment = { vertical: 'middle', horizontal: 'center' }; //tong
            row.getCell(14).alignment = { vertical: 'middle', horizontal: 'center' };//
            worksheet.mergeCells(`M${row.number}:N${row.number}`);
          }
          break;
        case "DAN":
          worksheet.mergeCells(`A${row.number}:D${row.number}`);
          if (index == 1 || index == 2) worksheet.mergeCells(`F${row.number}:N${row.number}`);
          if (index == 0) {
            row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
            row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
            row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center' };//khach hang
            row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center' };//san pham

            row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
            row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
            row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
            row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
            row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
            row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 day khoan khoet
            row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong khoan khoet
            row.getCell(13).alignment = { vertical: 'middle', horizontal: 'right' };
            row.getCell(14).alignment = { vertical: 'middle', horizontal: 'right' };
            row.getCell(15).alignment = { vertical: 'middle', horizontal: 'center' }; //tong
            row.getCell(16).alignment = { vertical: 'middle', horizontal: 'center' };//
            worksheet.mergeCells(`O${row.number}:P${row.number}`);
          }
          break;
        default:
          break;
      }
    });
    // worksheet.addRow([]);
    // let title2 = `ĐH TỒN`;
    // let titleRow2 = worksheet.addRow([title2]);
    // titleRow2.font = { family: 4, size: 16, bold: true };
    // titleRow2.height = 25;
    // titleRow2.alignment = { vertical: 'middle', horizontal: 'center' };
    // worksheet.mergeCells(`A${titleRow2.number}:L${titleRow2.number}`);
    // titleRow2.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, right: { style: "thin" } };
    // let dataRemain: Array<any> = this.getDataRemainReport(techRequest);
    // dataRemain.forEach(e => {
    //   let row = worksheet.addRow(e);
    //   let totalColumns = listExcelColumns.length;
    //   for (let i = 1; i <= totalColumns; i++) {
    //     row.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    //   }

    //   switch (techRequest.techniqueRequestCode) {
    //     case "CAT":
    //       row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //index
    //       row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left' };//lệnh số
    //       row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left' };//khach hang
    //       row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left' };//san pham
    //       row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
    //       row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
    //       row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
    //       row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
    //       row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
    //       row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
    //       row.getCell(11).alignment = { vertical: 'middle', horizontal: 'center' };//ngay tra
    //       row.getCell(12).alignment = { vertical: 'middle', horizontal: 'left' };//note
    //       break;
    //     case "MAI":
    //       row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //index
    //       row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left' };//lệnh số
    //       row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left' };//khach hang
    //       row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left' };//san pham
    //       row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
    //       row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
    //       row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
    //       row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
    //       row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
    //       row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
    //       row.getCell(11).alignment = { vertical: 'middle', horizontal: 'center' };//ngay tra
    //       row.getCell(12).alignment = { vertical: 'middle', horizontal: 'left' };//note
    //       break;
    //     case "KHOAN":
    //       row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //index
    //       row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left' };//lệnh số
    //       row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left' };//khach hang
    //       row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left' };//san pham
    //       row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
    //       row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
    //       row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
    //       row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
    //       row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
    //       row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
    //       row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' };//khoan khoet day
    //       row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right' };//khoan khoet mong
    //       row.getCell(13).alignment = { vertical: 'middle', horizontal: 'center' };//ngay tra
    //       row.getCell(14).alignment = { vertical: 'middle', horizontal: 'left' };//note
    //       break;
    //     case "TOI":
    //       row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //index
    //       row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left' };//lệnh số
    //       row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left' };//khach hang
    //       row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left' };//san pham
    //       row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
    //       row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
    //       row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
    //       row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
    //       row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
    //       row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
    //       row.getCell(11).alignment = { vertical: 'middle', horizontal: 'left' };//phun cat
    //       row.getCell(12).alignment = { vertical: 'middle', horizontal: 'left' };//rua kinh
    //       row.getCell(13).alignment = { vertical: 'middle', horizontal: 'center' };//ngay tra
    //       row.getCell(14).alignment = { vertical: 'middle', horizontal: 'left' };//note
    //       break;
    //     case "DAN":
    //       row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //index
    //       row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left' };//lệnh số
    //       row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left' };//khach hang
    //       row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left' };//san pham
    //       row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
    //       row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
    //       row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
    //       row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
    //       row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
    //       row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
    //       row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 nguyen khoi day
    //       row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 nguyen khoi mong
    //       row.getCell(13).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 cat ha day
    //       row.getCell(14).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 cat ha mong
    //       row.getCell(15).alignment = { vertical: 'middle', horizontal: 'center' };//ngay tra
    //       row.getCell(16).alignment = { vertical: 'middle', horizontal: 'left' };//note
    //       break;
    //     case "HOP":
    //       row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //index
    //       row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left' };//lệnh số
    //       row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left' };//khach hang
    //       row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left' };//san pham
    //       row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
    //       row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
    //       row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
    //       row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
    //       row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
    //       row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
    //       row.getCell(11).alignment = { vertical: 'middle', horizontal: 'left' };//phun cat
    //       row.getCell(12).alignment = { vertical: 'middle', horizontal: 'left' };//rua kinh
    //       row.getCell(13).alignment = { vertical: 'middle', horizontal: 'center' };//ngay tra
    //       row.getCell(14).alignment = { vertical: 'middle', horizontal: 'left' };//note
    //       break;
    //     default:
    //       break;
    //   }
    // });

    /* sumary section */
    // let sumaryRow2: Array<any> = this.getSumaryDataReportRemain(techRequest);
    // sumaryRow2.forEach((e, index) => {
    //   let row = worksheet.addRow(e);
    //   row.font = { family: 4, size: 12, bold: true };
    //   let totalColumns = listExcelColumns.length;
    //   for (let i = 1; i <= totalColumns; i++) {
    //     row.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    //   }

    //   switch (techRequest.techniqueRequestCode) {
    //     case "CAT":
    //     case "MAI":
    //       worksheet.mergeCells(`A${row.number}:D${row.number}`);
    //       if (index == 1 || index == 2) worksheet.mergeCells(`F${row.number}:J${row.number}`);
    //       if (index == 0) {
    //         row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
    //         row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
    //         row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center' };//khach hang
    //         row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center' };//san pham

    //         row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
    //         row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
    //         row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
    //         row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
    //         row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
    //         row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
    //         row.getCell(11).alignment = { vertical: 'middle', horizontal: 'center' }; //tong
    //         row.getCell(12).alignment = { vertical: 'middle', horizontal: 'center' };//
    //         worksheet.mergeCells(`K${row.number}:L${row.number}`);
    //       }
    //       break;
    //     case "KHOAN":
    //       worksheet.mergeCells(`A${row.number}:D${row.number}`);
    //       if (index == 1 || index == 2) worksheet.mergeCells(`F${row.number}:L${row.number}`);
    //       if (index == 0) {
    //         row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
    //         row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
    //         row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center' };//khach hang
    //         row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center' };//san pham

    //         row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
    //         row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
    //         row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
    //         row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
    //         row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
    //         row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
    //         row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 day khoan khoet
    //         row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong khoan khoet
    //         row.getCell(13).alignment = { vertical: 'middle', horizontal: 'center' }; //tong
    //         row.getCell(14).alignment = { vertical: 'middle', horizontal: 'center' };//
    //         worksheet.mergeCells(`M${row.number}:N${row.number}`);
    //       }
    //       break;
    //     case "TOI":
    //     case "HOP":
    //       worksheet.mergeCells(`A${row.number}:D${row.number}`);
    //       if (index == 1 || index == 2) worksheet.mergeCells(`F${row.number}:L${row.number}`);
    //       if (index == 0) {
    //         row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
    //         row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
    //         row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center' };//khach hang
    //         row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center' };//san pham

    //         row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
    //         row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
    //         row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
    //         row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
    //         row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
    //         row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
    //         row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 day khoan khoet
    //         row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong khoan khoet
    //         row.getCell(13).alignment = { vertical: 'middle', horizontal: 'center' }; //tong
    //         row.getCell(14).alignment = { vertical: 'middle', horizontal: 'center' };//
    //         worksheet.mergeCells(`M${row.number}:N${row.number}`);
    //       }
    //       break;
    //     case "DAN":
    //       worksheet.mergeCells(`A${row.number}:D${row.number}`);
    //       if (index == 1 || index == 2) worksheet.mergeCells(`F${row.number}:N${row.number}`);
    //       if (index == 0) {
    //         row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
    //         row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
    //         row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center' };//khach hang
    //         row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center' };//san pham

    //         row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
    //         row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
    //         row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
    //         row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
    //         row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
    //         row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong dac biet
    //         row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 day khoan khoet
    //         row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong khoan khoet
    //         row.getCell(13).alignment = { vertical: 'middle', horizontal: 'right' };
    //         row.getCell(14).alignment = { vertical: 'middle', horizontal: 'right' };
    //         row.getCell(15).alignment = { vertical: 'middle', horizontal: 'center' }; //tong
    //         row.getCell(16).alignment = { vertical: 'middle', horizontal: 'center' };//
    //         worksheet.mergeCells(`O${row.number}:P${row.number}`);
    //       }
    //       break;
    //     default:
    //       break;
    //   }
    // });

    /* set width */
    let width = dataHeaderRow1.map((e: any) => e.length);
    dataHeaderRow2.forEach((e, index) => {
      if (e != null) {
        if (e.length > width[index]) {
          width[index] = e.length;
        }
      }
    });
    data.forEach(row => {
      row.forEach((cell, index) => {
        if (cell != null) {
          if (cell.length > width[index])
            width[index] = cell.length;
        }
      });
    });

    // dataRemain.forEach(row => {
    //   row.forEach((cell, index) => {
    //     if (cell != null) {
    //       if (cell.length > width[index])
    //         width[index] = cell.length;
    //     }
    //   });
    // });

    width.forEach((el, index) => {
      worksheet.getColumn(index + 1).width = el + 5;
    });

    /* note row */
    //worksheet.addRow(['']);
    let noteData = "Ghi chú";
    let noteRow = worksheet.addRow([noteData]);
    noteRow.font = { family: 4, size: 12, bold: true, italic: true };
    noteRow.alignment = { vertical: 'top', horizontal: 'left' };
    //noteRow.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    noteRow.height = 70;
    worksheet.mergeCells(`A${noteRow.number}:L${noteRow.number}`);
    /* sign row */
    let text1 = "Kế hoạch sản xuất".padEnd(80);
    let text2 = "Xác nhận hoàn thành".padEnd(80);
    let text3 = "Sản xuất ký nhận".padEnd(80);
    let text4 = "Tổ trưởng".padEnd(80);
    let signData = `${text1}${text2}${text3}${text4}`;
    let signRow = worksheet.addRow([signData]);
    signRow.font = { family: 4, size: 12, bold: true };
    signRow.alignment = { vertical: 'top', horizontal: 'left' };
    signRow.height = 120;
    worksheet.mergeCells(`A${signRow.number}:L${signRow.number}`);
    //export
    this.exportToExel(workBook, title);
  }

  exportReportByMonth(techRequest: techniqueRequest, searchFormByMonth: searchFormModel) {
    let title = `BÁO CÁO SẢN XUẤT THEO THÁNG`;
    let workBook = new Workbook();
    let worksheet = workBook.addWorksheet(title);
    /* title */
    let titleRow = worksheet.addRow([title]);
    titleRow.font = { family: 4, size: 16, bold: true };
    titleRow.height = 25;
    titleRow.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A${titleRow.number}:I${titleRow.number}`);
    titleRow.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, right: { style: "thin" } };
    //desc row
    let fromDate = `Từ ngày ${searchFormByMonth.fromDate.getDate()} tháng ${searchFormByMonth.fromDate.getMonth() + 1} năm ${searchFormByMonth.fromDate.getFullYear()}`;
    let toDate = `đến ${searchFormByMonth.toDate.getDate()} tháng ${searchFormByMonth.toDate.getMonth() + 1} năm ${searchFormByMonth.toDate.getFullYear()}`;
    let desc = `${fromDate} ${toDate}`
    let descRow = worksheet.addRow([desc]);
    descRow.font = { family: 4, size: 12, italic: true };
    descRow.alignment = { vertical: 'middle', horizontal: 'right' };
    worksheet.mergeCells(`A${descRow.number}:I${descRow.number}`);
    //send row
    let send = `Gửi Tổ ${techRequest.techniqueName}`;
    let sendRow = worksheet.addRow([send]);
    sendRow.font = { family: 4, size: 12, bold: true };
    sendRow.height = 18;
    sendRow.alignment = { vertical: 'middle', horizontal: 'left' };
    worksheet.mergeCells(`A${sendRow.number}:I${sendRow.number}`);

    let listExcelColumns = this.getListExcelColumnsByMonth(techRequest);

    let dataHeaderRow1: Array<string> = listExcelColumns.map(e => e.column1);
    let dataHeaderRow2: Array<string> = listExcelColumns.map(e => e.column2);
    let headerRow1 = worksheet.addRow(dataHeaderRow1);
    let headerRow2 = worksheet.addRow(dataHeaderRow2);
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
    headerRow2.font = { name: 'Time New Roman', size: 10, bold: true };

    //merge column
    switch (techRequest.techniqueRequestCode) {
      case "CAT":
      case "MAI":
        worksheet.mergeCells(`A${4}:A${5}`);
        worksheet.mergeCells(`B${4}:B${5}`);
        worksheet.mergeCells(`C${4}:D${4}`);
        worksheet.mergeCells(`E${4}:F${4}`);
        worksheet.mergeCells(`G${4}:G${5}`);
        worksheet.mergeCells(`H${4}:H${5}`);
        worksheet.mergeCells(`I${4}:I${5}`);
        break;
      case "KHOAN":
        worksheet.mergeCells(`A${4}:A${5}`);
        worksheet.mergeCells(`B${4}:B${5}`);
        worksheet.mergeCells(`C${4}:D${4}`);
        worksheet.mergeCells(`E${4}:F${4}`);
        worksheet.mergeCells(`G${4}:H${4}`);
        worksheet.mergeCells(`I${4}:I${5}`);
        worksheet.mergeCells(`J${4}:J${5}`);
        worksheet.mergeCells(`K${4}:K${5}`);
        break;
      case "TOI":
      case "HOP":
        worksheet.mergeCells(`A${4}:A${5}`);
        worksheet.mergeCells(`B${4}:B${5}`);
        worksheet.mergeCells(`C${4}:D${4}`);
        worksheet.mergeCells(`E${4}:F${4}`);
        worksheet.mergeCells(`G${4}:G${5}`);
        worksheet.mergeCells(`H${4}:H${5}`);
        worksheet.mergeCells(`I${4}:I${5}`);
        worksheet.mergeCells(`J${4}:J${5}`);
        worksheet.mergeCells(`K${4}:K${5}`);
        break;
      case "DAN":
        worksheet.mergeCells(`A${4}:A${5}`);
        worksheet.mergeCells(`B${4}:B${5}`);
        worksheet.mergeCells(`C${4}:D${4}`);
        worksheet.mergeCells(`E${4}:F${4}`);
        worksheet.mergeCells(`G${4}:H${4}`);
        worksheet.mergeCells(`I${4}:J${4}`);
        worksheet.mergeCells(`K${4}:K${5}`);
        worksheet.mergeCells(`L${4}:L${5}`);
        worksheet.mergeCells(`M${4}:M${5}`);
        break;
      default:
        break;
    }

    dataHeaderRow1.forEach((item, index) => {
      headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow1.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
    });
    headerRow1.height = 40;
    dataHeaderRow2.forEach((item, index) => {
      headerRow2.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow2.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
    });
    headerRow2.height = 40;

    //handle data for single report type
    let data: Array<any> = this.getDataReportByMonth(techRequest);
    data.forEach((e, index) => {
      let row = worksheet.addRow(e);

      let totalColumns = listExcelColumns.length;
      for (let i = 1; i <= totalColumns; i++) {
        row.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }

      switch (techRequest.techniqueRequestCode) {
        case "CAT":
        case "MAI":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //thoi gian
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//theo ca
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 day
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 dac biet day
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 dac biet mong
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //tong ngay
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //tong dem
          row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //nhan cong
          break;
        case "KHOAN":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //thoi gian
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//theo ca
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 day
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 dac biet day
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 dac biet mong

          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 khoan khoet day
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 khoan khoet mong

          row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' };//tong ngay
          row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//tong dem
          row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' }; //nhan cong
          break;
        case "TOI":
        case "HOP":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //thoi gian
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//theo ca
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 day
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 dac biet day
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 dac biet mong

          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 khoan khoet day
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 khoan khoet mong

          row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' };//tong ngay
          row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//tong dem
          row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' }; //nhan cong
          break;
        case "DAN":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //thoi gian
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//theo ca
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 day
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 mong
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 dac biet day
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 dac biet mong
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 khoan khoet day
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' };//so m2 khoan khoet mong
          row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' };//tong ngay
          row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };//tong dem
          row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' }; //nhan cong
          row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right' }; //nhan cong
          row.getCell(13).alignment = { vertical: 'middle', horizontal: 'right' }; //nhan cong
          break;
        default:
          break;
      }
    });

    data.forEach((e, index) => {
      if (index % 2 == 0) {
        worksheet.mergeCells(`A${6 + index}: A${6 + 1 + index}`);
      }
    });

    let width = dataHeaderRow1.map((e: any) => e.length);
    dataHeaderRow2.forEach((e, index) => {
      if (e != null) {
        if (e.length > width[index]) {
          width[index] = e.length;
        }
      }
    });
    data.forEach(row => {
      row.forEach((cell, index) => {
        if (cell != null) {
          if (cell.length > width[index])
            width[index] = cell.length;
        }
      });
    });
    width.forEach((el, index) => {
      worksheet.getColumn(index + 1).width = el + 5;
    });

    /* sumary section */
    let sumaryRow: Array<any> = this.getSumaryDataReportByMonth(techRequest);

    sumaryRow.forEach((e, index) => {
      let row = worksheet.addRow(e);
      row.font = { family: 4, size: 12, bold: true };
      let totalColumns = listExcelColumns.length;
      for (let i = 1; i <= totalColumns; i++) {
        row.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }

      switch (techRequest.techniqueRequestCode) {
        case "CAT":
        case "MAI":
          if (index == 0) {
            worksheet.mergeCells(`A${row.number}:B${row.number}`);
            row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
            row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
            row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center' };//khach hang
            row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center' };//san pham
            row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
            row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
            row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
            row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
            row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          }
          if (index == 1 || index == 2) {
            worksheet.mergeCells(`A${row.number}:F${row.number}`);
            worksheet.mergeCells(`G${row.number}:H${row.number}`);
            row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' }; //index
            row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
            row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center' };//khach hang
            row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center' };//san pham
            row.getCell(5).alignment = { vertical: 'middle', horizontal: 'center' };//do day
            row.getCell(6).alignment = { vertical: 'middle', horizontal: 'center' };//so tam

            row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
            row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
            row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          }
          break;
        case "KHOAN":
          if (index == 0) {
            worksheet.mergeCells(`A${row.number}:B${row.number}`);
            row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
            row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
            row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };//khach hang
            row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };//san pham
            row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
            row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
            row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
            row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
            row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          }
          if (index == 1 || index == 2) {
            worksheet.mergeCells(`A${row.number}:H${row.number}`);
            worksheet.mergeCells(`I${row.number}:J${row.number}`);
            row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
            row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
            row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center' };//khach hang
            row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center' };//san pham
            row.getCell(5).alignment = { vertical: 'middle', horizontal: 'center' };//do day
            row.getCell(6).alignment = { vertical: 'middle', horizontal: 'center' };//so tam
            row.getCell(7).alignment = { vertical: 'middle', horizontal: 'center' }; //so m2 day
            row.getCell(8).alignment = { vertical: 'middle', horizontal: 'center' }; //so m2 mong

            row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          }
          break;
        case "TOI":
        case "HOP":
          if (index == 0) {
            worksheet.mergeCells(`A${row.number}:B${row.number}`);
            row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
            row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
            row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };//khach hang
            row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };//san pham
            row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
            row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
            row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
            row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
            row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          }
          if (index == 1 || index == 2) {
            worksheet.mergeCells(`A${row.number}:H${row.number}`);
            worksheet.mergeCells(`I${row.number}:J${row.number}`);
            row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
            row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
            row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center' };//khach hang
            row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center' };//san pham
            row.getCell(5).alignment = { vertical: 'middle', horizontal: 'center' };//do day
            row.getCell(6).alignment = { vertical: 'middle', horizontal: 'center' };//so tam
            row.getCell(7).alignment = { vertical: 'middle', horizontal: 'center' }; //so m2 day
            row.getCell(8).alignment = { vertical: 'middle', horizontal: 'center' }; //so m2 mong

            row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          }
          break;
        case "DAN":
          if (index == 0) {
            worksheet.mergeCells(`A${row.number}:B${row.number}`);
            row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
            row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
            row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };//khach hang
            row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };//san pham
            row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
            row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
            row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
            row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
            row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(13).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          }
          if (index == 1 || index == 2) {
            worksheet.mergeCells(`A${row.number}:J${row.number}`);
            worksheet.mergeCells(`K${row.number}:L${row.number}`);
            row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
            row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };//lệnh số
            row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center' };//khach hang
            row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center' };//san pham
            row.getCell(5).alignment = { vertical: 'middle', horizontal: 'center' };//do day
            row.getCell(6).alignment = { vertical: 'middle', horizontal: 'center' };//so tam
            row.getCell(7).alignment = { vertical: 'middle', horizontal: 'center' }; //so m2 day
            row.getCell(8).alignment = { vertical: 'middle', horizontal: 'center' }; //so m2 mong
            row.getCell(9).alignment = { vertical: 'middle', horizontal: 'center' }; //so m2 day dac biet
            row.getCell(10).alignment = { vertical: 'middle', horizontal: 'center' }; //so m2 day dac biet

            row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
            row.getCell(13).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          }
          break;
        default:
          break;
      }
    });

    //export
    this.exportToExel(workBook, title);
  }

  exportReportByYear(techRequest: techniqueRequest, searchFormByYear: searchFormModel) {
    let title = `BÁO CÁO SẢN XUẤT THEO NĂM`;
    let workBook = new Workbook();
    let worksheet = workBook.addWorksheet(title);
    /* title */
    let titleRow = worksheet.addRow([title]);
    titleRow.font = { family: 4, size: 16, bold: true };
    titleRow.height = 25;
    titleRow.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A${titleRow.number}:I${titleRow.number}`);
    titleRow.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, right: { style: "thin" } };
    //desc row
    let fromDate = `Từ ngày ${searchFormByYear.fromDate.getDate()} tháng ${searchFormByYear.fromDate.getMonth() + 1} năm ${searchFormByYear.fromDate.getFullYear()}`;
    let toDate = `đến ${searchFormByYear.toDate.getDate()} tháng ${searchFormByYear.toDate.getMonth() + 1} năm ${searchFormByYear.toDate.getFullYear()}`;
    let desc = `${fromDate} ${toDate}`
    let descRow = worksheet.addRow([desc]);
    descRow.font = { family: 4, size: 12, italic: true };
    descRow.alignment = { vertical: 'middle', horizontal: 'right' };
    worksheet.mergeCells(`A${descRow.number}:I${descRow.number}`);
    //send row
    let send = `Gửi Tổ ${techRequest.techniqueName}`;
    let sendRow = worksheet.addRow([send]);
    sendRow.font = { family: 4, size: 12, bold: true };
    sendRow.height = 18;
    sendRow.alignment = { vertical: 'middle', horizontal: 'left' };
    worksheet.mergeCells(`A${sendRow.number}:I${sendRow.number}`);

    let listExcelColumns = this.getListExcelColumnsByYear(techRequest);

    let dataHeaderRow1: Array<string> = listExcelColumns.map(e => e.column1);
    let dataHeaderRow2: Array<string> = listExcelColumns.map(e => e.column2);
    let headerRow1 = worksheet.addRow(dataHeaderRow1);
    let headerRow2 = worksheet.addRow(dataHeaderRow2);
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
    headerRow2.font = { name: 'Time New Roman', size: 10, bold: true };

    //merge column
    switch (techRequest.techniqueRequestCode) {
      case "CAT":
      case "MAI":
        worksheet.mergeCells(`A${4}:A${5}`);
        worksheet.mergeCells(`B${4}:C${4}`);
        worksheet.mergeCells(`D${4}:E${4}`);
        break;
      case "KHOAN":
        worksheet.mergeCells(`A${4}:A${5}`);
        worksheet.mergeCells(`B${4}:C${4}`);
        worksheet.mergeCells(`D${4}:E${4}`);
        worksheet.mergeCells(`F${4}:G${4}`);
        break;
      case "TOI":
      case "HOP":
        worksheet.mergeCells(`A${4}:A${5}`);
        worksheet.mergeCells(`B${4}:C${4}`);
        worksheet.mergeCells(`D${4}:E${4}`);
        worksheet.mergeCells(`F${4}:F${5}`);
        worksheet.mergeCells(`G${4}:G${5}`);
        break;
      case "DAN":
        worksheet.mergeCells(`A${4}:A${5}`);
        worksheet.mergeCells(`B${4}:C${4}`);
        worksheet.mergeCells(`D${4}:E${4}`);
        worksheet.mergeCells(`F${4}:G${4}`);
        worksheet.mergeCells(`H${4}:I${4}`);
        break;
      default:
        break;
    }

    dataHeaderRow1.forEach((item, index) => {
      headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow1.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
    });
    headerRow1.height = 40;
    dataHeaderRow2.forEach((item, index) => {
      headerRow2.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow2.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
    });
    headerRow2.height = 40;

    //handle data for single report type
    let data: Array<any> = this.getDataReportByYear(techRequest);
    data.forEach((e, index) => {
      let row = worksheet.addRow(e);

      let totalColumns = listExcelColumns.length;
      for (let i = 1; i <= totalColumns; i++) {
        row.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }

      switch (techRequest.techniqueRequestCode) {
        case "CAT":
        case "MAI":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };
          break;
        case "KHOAN":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' };
          break;
        case "TOI":
        case "HOP":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' };
          break;
        case "DAN":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' };
          row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' };
          break;
        default:
          break;
      }
    });

    let width = dataHeaderRow1.map((e: any) => e.length);
    dataHeaderRow2.forEach((e, index) => {
      if (e != null) {
        if (e.length > width[index]) {
          width[index] = e.length;
        }
      }
    });
    data.forEach(row => {
      row.forEach((cell, index) => {
        if (cell != null) {
          if (cell.length > width[index])
            width[index] = cell.length;
        }
      });
    });
    width.forEach((el, index) => {
      worksheet.getColumn(index + 1).width = el + 5;
    });

    /* sumary section */
    let sumaryRow: Array<any> = this.getSumaryDataReportByYear(techRequest);

    sumaryRow.forEach((e, index) => {
      let row = worksheet.addRow(e);
      row.font = { family: 4, size: 12, bold: true };
      let totalColumns = listExcelColumns.length;
      for (let i = 1; i <= totalColumns; i++) {
        row.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }

      switch (techRequest.techniqueRequestCode) {
        case "CAT":
        case "MAI":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'right' };//lệnh số
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };//khach hang
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };//san pham
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
          break;
        case "KHOAN":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'right' };//lệnh số
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };//khach hang
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };//san pham
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
          break;
        case "TOI":
        case "HOP":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'right' };//lệnh số
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };//khach hang
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };//san pham
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
          break;
        case "DAN":
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };//Stt
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'right' };//lệnh số
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'right' };//khach hang
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right' };//san pham
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right' };//do day
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right' };//so tam
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 mong
          row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right' }; //so m2 day dac biet
          break;
        default:
          break;
      }
    });

    //export
    this.exportToExel(workBook, title);
  }

  getListExcelColumns(techniqueRequest: techniqueRequest): Array<columnModel> {
    let listColumns: Array<columnModel> = [];
    /* NHỮNG CỘT DÙNG CHUNG CHO TẤT CẢ TEMPLATE */
    //cột số thứ tự
    let indexColumn: columnModel = new columnModel();
    indexColumn.column1 = "STT";
    indexColumn.column2 = "";
    //cột Lệnh số
    let orderCodeColumn: columnModel = new columnModel();
    orderCodeColumn.column1 = "Lệnh số";
    orderCodeColumn.column2 = "";
    //Khách hàng
    let customerNameColumn: columnModel = new columnModel();
    customerNameColumn.column1 = "Khách hàng";
    customerNameColumn.column2 = "";
    //Chủng loại
    let productNameColumn: columnModel = new columnModel();
    productNameColumn.column1 = "Chủng loại";
    productNameColumn.column2 = "";
    //Độ dày
    let thichnessColumn: columnModel = new columnModel();
    thichnessColumn.column1 = "Độ dày";
    thichnessColumn.column2 = "";
    //Số tấm
    let quantityColumn: columnModel = new columnModel();
    quantityColumn.column1 = "Số tấm";
    quantityColumn.column2 = "";
    //Số m2
    let totalAreaColumn1: columnModel = new columnModel();
    totalAreaColumn1.column1 = "Số m2";
    totalAreaColumn1.column2 = "Dày";
    let totalAreaColumn2: columnModel = new columnModel();
    totalAreaColumn2.column1 = "";
    totalAreaColumn2.column2 = "Mỏng";
    //Số m2 đặc biệt
    let totalAreaSpecColumn1: columnModel = new columnModel();
    totalAreaSpecColumn1.column1 = "Số m2 đặc biệt";
    totalAreaSpecColumn1.column2 = "Dày";
    let totalAreaSpecColumn2: columnModel = new columnModel();
    totalAreaSpecColumn2.column1 = "";
    totalAreaSpecColumn2.column2 = "Mỏng";
    //Ngày trả
    let receiveDate: columnModel = new columnModel();
    receiveDate.column1 = "Ngày trả";
    receiveDate.column2 = "";
    //Ghi chú
    let note: columnModel = new columnModel();
    note.column1 = "Ghi chú";
    note.column2 = "";
    /* NHỮNG CỘT ĐẶC BIỆT CHO 1 SỐ TEMPLATE */
    //Tổ khoan - cột khoan khoét
    let hole1: columnModel = new columnModel();
    hole1.column1 = "Khoan khoét";
    hole1.column2 = "Dày";
    let hole2: columnModel = new columnModel();
    hole2.column1 = "";
    hole2.column2 = "Mỏng";
    //Tổ tôi - phun cát - rửa kính - free text
    let phutcat: columnModel = new columnModel();
    phutcat.column1 = "Phun cát";
    phutcat.column2 = "";
    let ruakinh: columnModel = new columnModel();
    ruakinh.column1 = "Rửa kính";
    ruakinh.column2 = "";
    //Tổ dán - nguyên khổ
    let original1: columnModel = new columnModel();
    original1.column1 = "Số m2 nguyên khổ";
    original1.column2 = "Dày";
    let original2: columnModel = new columnModel();
    original2.column1 = "";
    original2.column2 = "Mỏng";
    //Tổ dán - cột cắt hạ - free text
    let catha1: columnModel = new columnModel();
    catha1.column1 = "Số m2 cắt hạ";
    catha1.column2 = "Dày";
    let catha2: columnModel = new columnModel();
    catha2.column1 = "";
    catha2.column2 = "Mỏng";

    switch (techniqueRequest.techniqueRequestCode) {
      case "CAT":
        listColumns = [indexColumn, orderCodeColumn, customerNameColumn, productNameColumn, thichnessColumn,
          quantityColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1, totalAreaSpecColumn2, receiveDate, note];
        break;
      case "DAN":
        listColumns = [indexColumn, orderCodeColumn, customerNameColumn, productNameColumn, thichnessColumn,
          quantityColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1, totalAreaSpecColumn2, original1, original2, catha1, catha2, receiveDate, note];
        break;
      case "HOP":
        listColumns = [indexColumn, orderCodeColumn, customerNameColumn, productNameColumn, thichnessColumn,
          quantityColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1, totalAreaSpecColumn2, phutcat, ruakinh, receiveDate, note];
        break;
      case "KHOAN":
        listColumns = [indexColumn, orderCodeColumn, customerNameColumn, productNameColumn, thichnessColumn,
          quantityColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1, totalAreaSpecColumn2, hole1, hole2, receiveDate, note];
        break;
      case "MAI":
        listColumns = [indexColumn, orderCodeColumn, customerNameColumn, productNameColumn, thichnessColumn,
          quantityColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1, totalAreaSpecColumn2, receiveDate, note];
        break;
      case "TOI":
        listColumns = [indexColumn, orderCodeColumn, customerNameColumn, productNameColumn, thichnessColumn,
          quantityColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1, totalAreaSpecColumn2, phutcat, ruakinh, receiveDate, note];
        break;
      default:
        break;
    }

    return listColumns;
  }

  getListExcelColumnsByMonth(techniqueRequest: techniqueRequest): Array<columnModel> {
    let listColumns: Array<columnModel> = [];
    /* NHỮNG CỘT DÙNG CHUNG CHO TẤT CẢ TEMPLATE */
    //cột số thứ tự
    // let indexColumn: columnModel = new columnModel();
    // indexColumn.column1 = "STT";
    // indexColumn.column2 = "";
    //cột ngày
    let dayColumn: columnModel = new columnModel();
    dayColumn.column1 = "Thời gian";
    dayColumn.column2 = "";
    //cột ca
    let shiftColumn: columnModel = new columnModel();
    shiftColumn.column1 = "Theo ca";
    shiftColumn.column2 = "";
    //Số m2
    let totalAreaColumn1: columnModel = new columnModel();
    totalAreaColumn1.column1 = "Số m2";
    totalAreaColumn1.column2 = "Dày";
    let totalAreaColumn2: columnModel = new columnModel();
    totalAreaColumn2.column1 = "";
    totalAreaColumn2.column2 = "Mỏng";
    //Số m2 đặc biệt
    let totalAreaSpecColumn1: columnModel = new columnModel();
    totalAreaSpecColumn1.column1 = "Số m2 đặc biệt";
    totalAreaSpecColumn1.column2 = "Dày";
    let totalAreaSpecColumn2: columnModel = new columnModel();
    totalAreaSpecColumn2.column1 = "";
    totalAreaSpecColumn2.column2 = "Mỏng";
    //tong ngay
    let dayShiftColumn: columnModel = new columnModel();
    dayShiftColumn.column1 = "Tổng ngày";
    dayShiftColumn.column2 = "";
    //tong dem
    let nightShiftColumn: columnModel = new columnModel();
    nightShiftColumn.column1 = "Tổng đêm";
    nightShiftColumn.column2 = "";
    //nhan cong
    let laborColumn: columnModel = new columnModel();
    laborColumn.column1 = "Nhân công";
    laborColumn.column2 = "";
    /* NHỮNG CỘT ĐẶC BIỆT CHO 1 SỐ TEMPLATE */
    //Tổ khoan - cột khoan khoét
    let hole1: columnModel = new columnModel();
    hole1.column1 = "Khoan khoét";
    hole1.column2 = "Dày";
    let hole2: columnModel = new columnModel();
    hole2.column1 = "";
    hole2.column2 = "Mỏng";
    //Tổ tôi - phun cát - rửa kính - free text
    let phutcat: columnModel = new columnModel();
    phutcat.column1 = "Phun cát";
    phutcat.column2 = "";
    let ruakinh: columnModel = new columnModel();
    ruakinh.column1 = "Rửa kính";
    ruakinh.column2 = "";
    //Tổ dán - nguyên khổ
    let original1: columnModel = new columnModel();
    original1.column1 = "Số m2 nguyên khổ";
    original1.column2 = "Dày";
    let original2: columnModel = new columnModel();
    original2.column1 = "";
    original2.column2 = "Mỏng";
    //Tổ dán - cột cắt hạ - free text
    let catha1: columnModel = new columnModel();
    catha1.column1 = "Số m2 cắt hạ";
    catha1.column2 = "Dày";
    let catha2: columnModel = new columnModel();
    catha2.column1 = "";
    catha2.column2 = "Mỏng";

    switch (techniqueRequest.techniqueRequestCode) {
      case "CAT":
        listColumns = [dayColumn, shiftColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1,
          totalAreaSpecColumn2, dayShiftColumn, nightShiftColumn, laborColumn];
        break;
      case "DAN":
        listColumns = [dayColumn, shiftColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1,
          totalAreaSpecColumn2, original1, original2, catha1, catha2, dayShiftColumn, nightShiftColumn, laborColumn];
        break;
      case "KHOAN":
        listColumns = [dayColumn, shiftColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1,
          totalAreaSpecColumn2, hole1, hole2, dayShiftColumn, nightShiftColumn, laborColumn];
        break;
      case "MAI":
        listColumns = [dayColumn, shiftColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1,
          totalAreaSpecColumn2, dayShiftColumn, nightShiftColumn, laborColumn];
        break;
      case "HOP":
      case "TOI":
        listColumns = [dayColumn, shiftColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1,
          totalAreaSpecColumn2, phutcat, ruakinh, dayShiftColumn, nightShiftColumn, laborColumn];
        break;
      default:
        break;
    }

    return listColumns;
  }

  getListExcelColumnsByYear(techniqueRequest: techniqueRequest): Array<columnModel> {
    let listColumns: Array<columnModel> = [];
    /* NHỮNG CỘT DÙNG CHUNG CHO TẤT CẢ TEMPLATE */
    //cột thoi gian
    let timeColumn: columnModel = new columnModel();
    timeColumn.column1 = "Thời gian";
    timeColumn.column2 = "";
    //cột ca
    // let shiftColumn: columnModel = new columnModel();
    // shiftColumn.column1 = "Theo ca";
    // shiftColumn.column2 = "";
    //Số m2
    let totalAreaColumn1: columnModel = new columnModel();
    totalAreaColumn1.column1 = "Số m2";
    totalAreaColumn1.column2 = "Dày";
    let totalAreaColumn2: columnModel = new columnModel();
    totalAreaColumn2.column1 = "";
    totalAreaColumn2.column2 = "Mỏng";
    //Số m2 đặc biệt
    let totalAreaSpecColumn1: columnModel = new columnModel();
    totalAreaSpecColumn1.column1 = "Số m2 đặc biệt";
    totalAreaSpecColumn1.column2 = "Dày";
    let totalAreaSpecColumn2: columnModel = new columnModel();
    totalAreaSpecColumn2.column1 = "";
    totalAreaSpecColumn2.column2 = "Mỏng";
    let totalColumn: columnModel = new columnModel();
    totalColumn.column1 = "Tổng";
    totalColumn.column2 = "";
    /* NHỮNG CỘT ĐẶC BIỆT CHO 1 SỐ TEMPLATE */
    //Tổ khoan - cột khoan khoét
    let hole1: columnModel = new columnModel();
    hole1.column1 = "Khoan khoét";
    hole1.column2 = "Dày";
    let hole2: columnModel = new columnModel();
    hole2.column1 = "";
    hole2.column2 = "Mỏng";
    //Tổ tôi - phun cát - rửa kính - free text
    let phutcat: columnModel = new columnModel();
    phutcat.column1 = "Phun cát";
    phutcat.column2 = "";
    let ruakinh: columnModel = new columnModel();
    ruakinh.column1 = "Rửa kính";
    ruakinh.column2 = "";
    //Tổ dán - nguyên khổ
    let original1: columnModel = new columnModel();
    original1.column1 = "Số m2 nguyên khổ";
    original1.column2 = "Dày";
    let original2: columnModel = new columnModel();
    original2.column1 = "";
    original2.column2 = "Mỏng";
    //Tổ dán - cột cắt hạ - free text
    let catha1: columnModel = new columnModel();
    catha1.column1 = "Số m2 cắt hạ";
    catha1.column2 = "Dày";
    let catha2: columnModel = new columnModel();
    catha2.column1 = "";
    catha2.column2 = "Mỏng";

    switch (techniqueRequest.techniqueRequestCode) {
      case "CAT":
      case "MAI":
        listColumns = [timeColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1, totalAreaSpecColumn2, totalColumn];
        break;
      case "DAN":
        listColumns = [timeColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1, totalAreaSpecColumn2, original1, original2, catha1, catha2, totalColumn];
        break;
      case "KHOAN":
        listColumns = [timeColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1, totalAreaSpecColumn2, hole1, hole2, totalColumn];
        break;
      case "HOP":
      case "TOI":
        listColumns = [timeColumn, totalAreaColumn1, totalAreaColumn2, totalAreaSpecColumn1, totalAreaSpecColumn2, phutcat, ruakinh, totalColumn];
        break;
      default:
        break;
    }

    return listColumns;
  }

  // getDataRemainReport(techRequest: techniqueRequest): Array<any> {
  //   let data: Array<any> = [];
  //   switch (techRequest.techniqueRequestCode) {
  //     case "CAT":
  //       this.listReportManuFactureByDayRemain.forEach((item, index) => {
  //         let row: Array<any> = [];
  //         row[0] = index + 1;
  //         row[1] = ConvertToString(item.productionOrderCode); //lệnh số
  //         row[2] = ConvertToString(item.customerName); //khach hang
  //         row[3] = ConvertToString(item.productName); //san pham
  //         row[4] = ConvertToString(item.productThickness); //do day
  //         row[5] = ConvertToString(item.quantity); //so tam
  //         row[6] = ConvertToString(item.totalAreaThick); //so m2 day
  //         row[7] = ConvertToString(item.totalAreaThin); //so m2 mong
  //         row[8] = ConvertToString(item.totalAreaEspeciallyThick); //so m2 day dac biet
  //         row[9] = ConvertToString(item.totalAreaEspeciallyThin); //so m2 mong dac biet
  //         row[10] = this.transformDate(item.endDate); //ngay tra
  //         row[11] = ConvertToString(item.note); //note
  //         data.push(row);
  //       });
  //       break;
  //     case "KHOAN":
  //       this.listReportManuFactureByDayRemain.forEach((item, index) => {
  //         let row: Array<any> = [];
  //         row[0] = index + 1;
  //         row[1] = ConvertToString(item.productionOrderCode); //lệnh số
  //         row[2] = ConvertToString(item.customerName); //khach hang
  //         row[3] = ConvertToString(item.productName); //san pham
  //         row[4] = ConvertToString(item.productThickness); //do day
  //         row[5] = ConvertToString(item.quantity); //so tam
  //         row[6] = ConvertToString(item.totalAreaThick); //so m2 day
  //         row[7] = ConvertToString(item.totalAreaThin); //so m2 mong
  //         row[8] = ConvertToString(item.totalAreaEspeciallyThick); //so m2 day dac biet
  //         row[9] = ConvertToString(item.totalAreaEspeciallyThin); //so m2 mong dac biet
  //         row[10] = ConvertToString(item.totalBoreHoleThick); //khoan khoet day
  //         row[11] = ConvertToString(item.totalBoreHoleThin); //khoan khoet mong
  //         row[12] = this.transformDate(item.endDate); //ngay tra
  //         row[13] = ConvertToString(item.note); //note
  //         data.push(row);
  //       });
  //       break;
  //     case "MAI":
  //       this.listReportManuFactureByDayRemain.forEach((item, index) => {
  //         let row: Array<any> = [];
  //         row[0] = index + 1;
  //         row[1] = ConvertToString(item.productionOrderCode); //lệnh số
  //         row[2] = ConvertToString(item.customerName); //khach hang
  //         row[3] = ConvertToString(item.productName); //san pham
  //         row[4] = ConvertToString(item.productThickness); //do day
  //         row[5] = ConvertToString(item.quantity); //so tam
  //         row[6] = ConvertToString(item.totalAreaThick); //so m2 day
  //         row[7] = ConvertToString(item.totalAreaThin); //so m2 mong
  //         row[8] = ConvertToString(item.totalAreaEspeciallyThick); //so m2 day dac biet
  //         row[9] = ConvertToString(item.totalAreaEspeciallyThin); //so m2 mong dac biet
  //         row[10] = this.transformDate(item.endDate); //ngay tra
  //         row[11] = ConvertToString(item.note); //note
  //         data.push(row);
  //       });
  //       break;
  //     case "TOI":
  //       this.listReportManuFactureByDayRemain.forEach((item, index) => {
  //         let row: Array<any> = [];
  //         row[0] = index + 1;
  //         row[1] = ConvertToString(item.productionOrderCode); //lệnh số
  //         row[2] = ConvertToString(item.customerName); //khach hang
  //         row[3] = ConvertToString(item.productName); //san pham
  //         row[4] = ConvertToString(item.productThickness); //do day
  //         row[5] = ConvertToString(item.quantity); //so tam
  //         row[6] = ConvertToString(item.totalAreaThick); //so m2 day
  //         row[7] = ConvertToString(item.totalAreaThin); //so m2 mong
  //         row[8] = ConvertToString(item.totalAreaEspeciallyThick); //so m2 day dac biet
  //         row[9] = ConvertToString(item.totalAreaEspeciallyThin); //so m2 mong dac biet
  //         row[10] = ''; //phun cat
  //         row[11] = ''; //rua kinh
  //         row[12] = this.transformDate(item.endDate); //ngay tra
  //         row[13] = ConvertToString(item.note); //note
  //         data.push(row);
  //       });
  //       break;
  //     case "DAN":
  //       this.listReportManuFactureByDayRemain.forEach((item, index) => {
  //         let row: Array<any> = [];
  //         row[0] = index + 1;
  //         row[1] = ConvertToString(item.productionOrderCode); //lệnh số
  //         row[2] = ConvertToString(item.customerName); //khach hang
  //         row[3] = ConvertToString(item.productName); //san pham
  //         row[4] = ConvertToString(item.productThickness); //do day
  //         row[5] = ConvertToString(item.quantity); //so tam
  //         row[6] = ConvertToString(item.totalAreaThick); //so m2 day
  //         row[7] = ConvertToString(item.totalAreaThin); //so m2 mong
  //         row[8] = ConvertToString(item.totalAreaEspeciallyThick); //so m2 day dac biet
  //         row[9] = ConvertToString(item.totalAreaEspeciallyThin); //so m2 mong dac biet
  //         row[10] = ConvertToString(item.totalAreaOriginalThick); //so m2 nguyen kho day
  //         row[11] = ConvertToString(item.totalAreaOriginalThin); //so m2 nguyen kho mong
  //         row[12] = ''; //so m2 cat ha day
  //         row[13] = ''; //so m2 cat ha mong
  //         row[14] = this.transformDate(item.endDate); //ngay tra
  //         row[15] = ConvertToString(item.note); //note
  //         data.push(row);
  //       });
  //       break;
  //     case "HOP":
  //       this.listReportManuFactureByDayRemain.forEach((item, index) => {
  //         let row: Array<any> = [];
  //         row[0] = index + 1;
  //         row[1] = ConvertToString(item.productionOrderCode); //lệnh số
  //         row[2] = ConvertToString(item.customerName); //khach hang
  //         row[3] = ConvertToString(item.productName); //san pham
  //         row[4] = ConvertToString(item.productThickness); //do day
  //         row[5] = ConvertToString(item.quantity); //so tam
  //         row[6] = ConvertToString(item.totalAreaThick); //so m2 day
  //         row[7] = ConvertToString(item.totalAreaThin); //so m2 mong
  //         row[8] = ConvertToString(item.totalAreaEspeciallyThick); //so m2 day dac biet
  //         row[9] = ConvertToString(item.totalAreaEspeciallyThin); //so m2 mong dac biet
  //         row[10] = ''; //phun cat
  //         row[11] = ''; //rua kinh
  //         row[12] = this.transformDate(item.endDate); //ngay tra
  //         row[13] = ConvertToString(item.note); //note
  //         data.push(row);
  //       });
  //       break;
  //     default:
  //       break;
  //   }

  //   return data;
  // }

  getDataReport(techRequest: techniqueRequest): Array<any> {
    let data: Array<any> = [];
    switch (techRequest.techniqueRequestCode) {
      case "CAT":
        this.listReportManuFactureByDay.forEach((item, index) => {
          let row: Array<any> = [];
          row[0] = index + 1;
          row[1] = ConvertToString(item.productionOrderCode); //lệnh số
          row[2] = ConvertToString(item.customerName); //khach hang
          row[3] = ConvertToString(item.productName); //san pham
          row[4] = ConvertToString(item.productThickness); //do day
          row[5] = ConvertToString(item.quantity); //so tam
          row[6] = ConvertToString(item.totalAreaThick); //so m2 day
          row[7] = ConvertToString(item.totalAreaThin); //so m2 mong
          row[8] = ConvertToString(item.totalAreaEspeciallyThick); //so m2 day dac biet
          row[9] = ConvertToString(item.totalAreaEspeciallyThin); //so m2 mong dac biet
          row[10] = this.transformDate(item.endDate); //ngay tra
          row[11] = ConvertToString(item.note); //note
          data.push(row);
        });
        break;
      case "KHOAN":
        this.listReportManuFactureByDay.forEach((item, index) => {
          let row: Array<any> = [];
          row[0] = index + 1;
          row[1] = ConvertToString(item.productionOrderCode); //lệnh số
          row[2] = ConvertToString(item.customerName); //khach hang
          row[3] = ConvertToString(item.productName); //san pham
          row[4] = ConvertToString(item.productThickness); //do day
          row[5] = ConvertToString(item.quantity); //so tam
          row[6] = ConvertToString(item.totalAreaThick); //so m2 day
          row[7] = ConvertToString(item.totalAreaThin); //so m2 mong
          row[8] = ConvertToString(item.totalAreaEspeciallyThick); //so m2 day dac biet
          row[9] = ConvertToString(item.totalAreaEspeciallyThin); //so m2 mong dac biet
          row[10] = ConvertToString(item.totalAreaBoreholeThick); //khoan khoet day
          row[11] = ConvertToString(item.totalAreaBoreholeThin); //khoan khoet mong
          row[12] = this.transformDate(item.endDate); //ngay tra
          row[13] = ConvertToString(item.note); //note
          data.push(row);
        });
        break;
      case "MAI":
        this.listReportManuFactureByDay.forEach((item, index) => {
          let row: Array<any> = [];
          row[0] = index + 1;
          row[1] = ConvertToString(item.productionOrderCode); //lệnh số
          row[2] = ConvertToString(item.customerName); //khach hang
          row[3] = ConvertToString(item.productName); //san pham
          row[4] = ConvertToString(item.productThickness); //do day
          row[5] = ConvertToString(item.quantity); //so tam
          row[6] = ConvertToString(item.totalAreaThick); //so m2 day
          row[7] = ConvertToString(item.totalAreaThin); //so m2 mong
          row[8] = ConvertToString(item.totalAreaEspeciallyThick); //so m2 day dac biet
          row[9] = ConvertToString(item.totalAreaEspeciallyThin); //so m2 mong dac biet
          row[10] = this.transformDate(item.endDate); //ngay tra
          row[11] = ConvertToString(item.note); //note
          data.push(row);
        });
        break;
      case "TOI":
        this.listReportManuFactureByDay.forEach((item, index) => {
          let row: Array<any> = [];
          row[0] = index + 1;
          row[1] = ConvertToString(item.productionOrderCode); //lệnh số
          row[2] = ConvertToString(item.customerName); //khach hang
          row[3] = ConvertToString(item.productName); //san pham
          row[4] = ConvertToString(item.productThickness); //do day
          row[5] = ConvertToString(item.quantity); //so tam
          row[6] = ConvertToString(item.totalAreaThick); //so m2 day
          row[7] = ConvertToString(item.totalAreaThin); //so m2 mong
          row[8] = ConvertToString(item.totalAreaEspeciallyThick); //so m2 day dac biet
          row[9] = ConvertToString(item.totalAreaEspeciallyThin); //so m2 mong dac biet
          row[10] = ''; //phun cat
          row[11] = ''; //rua kinh
          row[12] = this.transformDate(item.endDate); //ngay tra
          row[13] = ConvertToString(item.note); //note
          data.push(row);
        });
        break;
      case "DAN":
        this.listReportManuFactureByDay.forEach((item, index) => {
          let row: Array<any> = [];
          row[0] = index + 1;
          row[1] = ConvertToString(item.productionOrderCode); //lệnh số
          row[2] = ConvertToString(item.customerName); //khach hang
          row[3] = ConvertToString(item.productName); //san pham
          row[4] = ConvertToString(item.productThickness); //do day
          row[5] = ConvertToString(item.quantity); //so tam
          row[6] = ConvertToString(item.totalAreaThick); //so m2 day
          row[7] = ConvertToString(item.totalAreaThin); //so m2 mong
          row[8] = ConvertToString(item.totalAreaEspeciallyThick); //so m2 day dac biet
          row[9] = ConvertToString(item.totalAreaEspeciallyThin); //so m2 mong dac biet
          row[10] = ConvertToString(item.totalAreaOriginalThick); //so m2 nguyen kho day
          row[11] = ConvertToString(item.totalAreaOriginalThin); //so m2 nguyen kho mong
          row[12] = ''; //so m2 cat ha day
          row[13] = ''; //so m2 cat ha mong
          row[14] = this.transformDate(item.endDate); //ngay tra
          row[15] = ConvertToString(item.note); //note
          data.push(row);
        });
        break;
      case "HOP":
        this.listReportManuFactureByDay.forEach((item, index) => {
          let row: Array<any> = [];
          row[0] = index + 1;
          row[1] = ConvertToString(item.productionOrderCode); //lệnh số
          row[2] = ConvertToString(item.customerName); //khach hang
          row[3] = ConvertToString(item.productName); //san pham
          row[4] = ConvertToString(item.productThickness); //do day
          row[5] = ConvertToString(item.quantity); //so tam
          row[6] = ConvertToString(item.totalAreaThick); //so m2 day
          row[7] = ConvertToString(item.totalAreaThin); //so m2 mong
          row[8] = ConvertToString(item.totalAreaEspeciallyThick); //so m2 day dac biet
          row[9] = ConvertToString(item.totalAreaEspeciallyThin); //so m2 mong dac biet
          row[10] = ''; //phun cat
          row[11] = ''; //rua kinh
          row[12] = this.transformDate(item.endDate); //ngay tra
          row[13] = ConvertToString(item.note); //note
          data.push(row);
        });
        break;
      default:
        break;
    }

    return data;
  }

  getDataReportByMonth(techRequest: techniqueRequest): Array<any> {
    let data: Array<any> = [];
    switch (techRequest.techniqueRequestCode) {
      case "CAT":
      case "MAI":
        this.listReportManuFactureByMonth.forEach((item, index) => {
          let row: Array<any> = [];
          row[0] = this.transformDate(item.day); //thời gian
          row[1] = ConvertToString(item.shift.label); //theo ca
          row[2] = ConvertToString(item.totalAreaThick);//so m2 day
          row[3] = ConvertToString(item.totalAreaThin);//so m2 mong
          row[4] = ConvertToString(item.totalAreaEspeciallyThick);//so m2 day dac biet
          row[5] = ConvertToString(item.totalAreaEspeciallyThin);//so m2 day dac biet
          row[6] = ConvertToString(item.totalDayShift);//tong ngay
          row[7] = ConvertToString(item.totalNightShift);//tong dem
          row[8] = ConvertToString(item.labor);//nhan cong
          data.push(row);
        });
        break;
      case "KHOAN":
        this.listReportManuFactureByMonth.forEach((item, index) => {
          let row: Array<any> = [];
          row[0] = this.transformDate(item.day); //thời gian
          row[1] = ConvertToString(item.shift.label); //theo ca
          row[2] = ConvertToString(item.totalAreaThick);//so m2 day
          row[3] = ConvertToString(item.totalAreaThin);//so m2 mong
          row[4] = ConvertToString(item.totalAreaEspeciallyThick);//so m2 day dac biet
          row[5] = ConvertToString(item.totalAreaEspeciallyThin);//so m2 day dac biet
          row[6] = ConvertToString(item.totalAreaBoreholeThick); //so m2 khoan khoet day
          row[7] = ConvertToString(item.totalAreaBoreholeThin); //so m2 khoan khoet mong
          row[8] = ConvertToString(item.totalDayShift);//tong ngay
          row[9] = ConvertToString(item.totalNightShift);//tong dem
          row[10] = ConvertToString(item.labor);//nhan cong
          data.push(row);
        });
        break;
      case "TOI":
      case "HOP":
        this.listReportManuFactureByMonth.forEach((item, index) => {
          let row: Array<any> = [];
          row[0] = this.transformDate(item.day); //thời gian
          row[1] = ConvertToString(item.shift.label); //theo ca

          row[2] = ConvertToString(item.totalAreaThick);//so m2 day
          row[3] = ConvertToString(item.totalAreaThin);//so m2 mong

          row[4] = ConvertToString(item.totalAreaEspeciallyThick);//so m2 day dac biet
          row[5] = ConvertToString(item.totalAreaEspeciallyThin);//so m2 day dac biet

          row[6] = ConvertToString(""); //phun cat
          row[7] = ConvertToString(""); //rua kinh

          row[8] = ConvertToString(item.totalDayShift);//tong ngay
          row[9] = ConvertToString(item.totalNightShift);//tong dem
          row[10] = ConvertToString(item.labor);//nhan cong
          data.push(row);
        });
        break;
      case "DAN":
        this.listReportManuFactureByMonth.forEach((item, index) => {
          let row: Array<any> = [];
          row[0] = this.transformDate(item.day); //thời gian
          row[1] = ConvertToString(item.shift.label); //theo ca

          row[2] = ConvertToString(item.totalAreaThick);//so m2 day
          row[3] = ConvertToString(item.totalAreaThin);//so m2 mong

          row[4] = ConvertToString(item.totalAreaEspeciallyThick);//so m2 day dac biet
          row[5] = ConvertToString(item.totalAreaEspeciallyThin);//so m2 day dac biet

          row[6] = ConvertToString(item.totalAreaOriginalThick);//nguyen kho
          row[7] = ConvertToString(item.totalAreaOriginalThin);//nguyen kho

          row[8] = ConvertToString(""); //cat ha day
          row[9] = ConvertToString(""); //cat ha mong

          row[10] = ConvertToString(item.totalDayShift);//tong ngay
          row[11] = ConvertToString(item.totalNightShift);//tong dem
          row[12] = ConvertToString(item.labor);//nhan cong
          data.push(row);
        });
        break;
      default:
        break;
    }
    return data;
  }

  getDataReportByYear(techRequest: techniqueRequest): Array<any> {
    let data: Array<any> = [];
    switch (techRequest.techniqueRequestCode) {
      case "CAT":
      case "MAI":
        this.listReportManuFactureByYear.forEach((item, index) => {
          let row: Array<any> = [];
          row[0] = ConvertToString(this.listMonth.find(e => e == item.month).label); //thời gian
          row[1] = ConvertToString(item.totalAreaThick);//so m2 day
          row[2] = ConvertToString(item.totalAreaThin);//so m2 mong
          row[3] = ConvertToString(item.totalAreaEspeciallyThick);//so m2 day dac biet
          row[4] = ConvertToString(item.totalAreaEspeciallyThin);//so m2 day dac biet
          row[5] = ConvertToString(item.totalArea);//tong
          data.push(row);
        });
        break;
      case "KHOAN":
        this.listReportManuFactureByYear.forEach((item, index) => {
          let row: Array<any> = [];
          row[0] = ConvertToString(this.listMonth.find(e => e == item.month).label); //thời gian
          row[1] = ConvertToString(item.totalAreaThick);//so m2 day
          row[2] = ConvertToString(item.totalAreaThin);//so m2 mong
          row[3] = ConvertToString(item.totalAreaEspeciallyThick);//so m2 day dac biet
          row[4] = ConvertToString(item.totalAreaEspeciallyThin);//so m2 day dac biet
          row[5] = ConvertToString(item.totalAreaBoreholeThick);
          row[6] = ConvertToString(item.totalAreaBoreholeThin);
          row[7] = ConvertToString(item.totalArea);//tong
          data.push(row);
        });
        break;
      case "TOI":
      case "HOP":
        this.listReportManuFactureByYear.forEach((item, index) => {
          let row: Array<any> = [];
          row[0] = ConvertToString(this.listMonth.find(e => e == item.month).label); //thời gian
          row[1] = ConvertToString(item.totalAreaThick);//so m2 day
          row[2] = ConvertToString(item.totalAreaThin);//so m2 mong
          row[3] = ConvertToString(item.totalAreaEspeciallyThick);//so m2 day dac biet
          row[4] = ConvertToString(item.totalAreaEspeciallyThin);//so m2 day dac biet
          row[5] = ConvertToString("");
          row[6] = ConvertToString("");
          row[7] = ConvertToString(item.totalArea);//tong

          data.push(row);
        });
        break;
      case "DAN":
        this.listReportManuFactureByYear.forEach((item, index) => {
          let row: Array<any> = [];
          row[0] = ConvertToString(this.listMonth.find(e => e == item.month).label); //thời gian
          row[1] = ConvertToString(item.totalAreaThick);//so m2 day
          row[2] = ConvertToString(item.totalAreaThin);//so m2 mong
          row[3] = ConvertToString(item.totalAreaEspeciallyThick);//so m2 day dac biet
          row[4] = ConvertToString(item.totalAreaEspeciallyThin);//so m2 day dac biet
          row[5] = ConvertToString(item.totalAreaOriginalThick);
          row[6] = ConvertToString(item.totalAreaOriginalThin);
          row[7] = ConvertToString("");
          row[8] = ConvertToString("");
          row[9] = ConvertToString(item.totalArea);//tong
          data.push(row);
        });
        break;
      default:
        break;
    }
    return data;
  }

  getSumaryDataReport(techRequest: techniqueRequest): Array<any> {
    let data: Array<any> = [];
    switch (techRequest.techniqueRequestCode) {
      case "CAT":
      case "MAI":
        let row1: Array<any> = [];
        row1[0] = "TỔNG";
        row1[1] = "";
        row1[2] = "";
        row1[3] = "";
        row1[4] = "";
        row1[5] = ConvertToString(this.sumaryReportManuFactureByDay.quantity); //so tam
        row1[6] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaThick); //so m2 day
        row1[7] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaThin); //so m2 mong
        row1[8] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaEspeciallyThick);  //so m2 day dac biet
        row1[9] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaEspeciallyThin);  //so m2 mong dac biet
        row1[10] = ConvertToString(this.sumaryReportManuFactureByDay.totalArea);
        row1[11] = "";
        data.push(row1);
        break;
      case "KHOAN":
        let row1_KHOAN: Array<any> = [];
        row1_KHOAN[0] = "TỔNG";
        row1_KHOAN[1] = "";
        row1_KHOAN[2] = "";
        row1_KHOAN[3] = "";
        row1_KHOAN[4] = "";
        row1_KHOAN[5] = ConvertToString(this.sumaryReportManuFactureByDay.quantity); //so tam
        row1_KHOAN[6] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaThick); //so m2 day
        row1_KHOAN[7] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaThin); //so m2 mong
        row1_KHOAN[8] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaEspeciallyThick);  //so m2 day dac biet
        row1_KHOAN[9] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaEspeciallyThin);  //so m2 mong dac biet
        row1_KHOAN[10] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaBoreholeThick);  //so m2 day khoan khoet
        row1_KHOAN[11] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaBoreholeThin);  //so m2 mong khoan khoet
        row1_KHOAN[12] = ConvertToString(this.sumaryReportManuFactureByDay.totalArea);
        row1_KHOAN[13] = "";
        data.push(row1_KHOAN);
        break;
      case "TOI":
      case "HOP":
        let row1_TOI: Array<any> = [];
        row1_TOI[0] = "TỔNG";
        row1_TOI[1] = "";
        row1_TOI[2] = "";
        row1_TOI[3] = "";
        row1_TOI[4] = "";
        row1_TOI[5] = ConvertToString(this.sumaryReportManuFactureByDay.quantity); //so tam
        row1_TOI[6] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaThick); //so m2 day
        row1_TOI[7] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaThin); //so m2 mong
        row1_TOI[8] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaEspeciallyThick);  //so m2 day dac biet
        row1_TOI[9] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaEspeciallyThin);  //so m2 mong dac biet
        row1_TOI[10] = ConvertToString("");  //Phun cat
        row1_TOI[11] = ConvertToString("");  //Rua kinh
        row1_TOI[12] = ConvertToString(this.sumaryReportManuFactureByDay.totalArea);
        row1_TOI[13] = "";
        data.push(row1_TOI);
        break;
      case "DAN":
        let row1_DAN: Array<any> = [];
        row1_DAN[0] = "TỔNG";
        row1_DAN[1] = "";
        row1_DAN[2] = "";
        row1_DAN[3] = "";
        row1_DAN[4] = "";
        row1_DAN[5] = ConvertToString(this.sumaryReportManuFactureByDay.quantity); //so tam
        row1_DAN[6] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaThick); //so m2 day
        row1_DAN[7] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaThin); //so m2 mong
        row1_DAN[8] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaEspeciallyThick);  //so m2 day dac biet
        row1_DAN[9] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaEspeciallyThin);  //so m2 mong dac biet
        row1_DAN[10] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaOriginalThick);  //m2 nguyen kho day
        row1_DAN[11] = ConvertToString(this.sumaryReportManuFactureByDay.totalAreaOriginalThin);  //m2 nguyen kho mong
        row1_DAN[12] = ""; //m2 cat ha day
        row1_DAN[13] = ""; //m2 cat ha mong
        row1_DAN[14] = ConvertToString(this.sumaryReportManuFactureByDay.totalArea);
        row1_DAN[15] = "";
        data.push(row1_DAN);
        break;
      default:
        break;
    }
    return data;
  }

  // getSumaryDataReportRemain(techRequest: techniqueRequest): Array<any> {
  //   let data: Array<any> = [];
  //   switch (techRequest.techniqueRequestCode) {
  //     case "CAT":
  //     case "MAI":
  //       let row1: Array<any> = [];
  //       row1[0] = "TỔNG";
  //       row1[1] = "";
  //       row1[2] = "";
  //       row1[3] = "";
  //       row1[4] = "";
  //       row1[5] = ConvertToString(this.sumaryReportManuFactureByDayRemain.quantity); //so tam
  //       row1[6] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaThick); //so m2 day
  //       row1[7] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaThin); //so m2 mong
  //       row1[8] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaEspeciallyThick);  //so m2 day dac biet
  //       row1[9] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaEspeciallyThin);  //so m2 mong dac biet
  //       row1[10] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalArea);
  //       row1[11] = "";
  //       data.push(row1);
  //       break;
  //     case "KHOAN":
  //       let row1_KHOAN: Array<any> = [];
  //       row1_KHOAN[0] = "TỔNG";
  //       row1_KHOAN[1] = "";
  //       row1_KHOAN[2] = "";
  //       row1_KHOAN[3] = "";
  //       row1_KHOAN[4] = "";
  //       row1_KHOAN[5] = ConvertToString(this.sumaryReportManuFactureByDayRemain.quantity); //so tam
  //       row1_KHOAN[6] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaThick); //so m2 day
  //       row1_KHOAN[7] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaThin); //so m2 mong
  //       row1_KHOAN[8] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaEspeciallyThick);  //so m2 day dac biet
  //       row1_KHOAN[9] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaEspeciallyThin);  //so m2 mong dac biet
  //       row1_KHOAN[10] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaBoreholeThick);  //so m2 day khoan khoet
  //       row1_KHOAN[11] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaBoreholeThin);  //so m2 mong khoan khoet
  //       row1_KHOAN[12] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalArea);
  //       row1_KHOAN[13] = "";
  //       data.push(row1_KHOAN);
  //       break;
  //     case "TOI":
  //     case "HOP":
  //       let row1_TOI: Array<any> = [];
  //       row1_TOI[0] = "TỔNG";
  //       row1_TOI[1] = "";
  //       row1_TOI[2] = "";
  //       row1_TOI[3] = "";
  //       row1_TOI[4] = "";
  //       row1_TOI[5] = ConvertToString(this.sumaryReportManuFactureByDayRemain.quantity); //so tam
  //       row1_TOI[6] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaThick); //so m2 day
  //       row1_TOI[7] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaThin); //so m2 mong
  //       row1_TOI[8] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaEspeciallyThick);  //so m2 day dac biet
  //       row1_TOI[9] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaEspeciallyThin);  //so m2 mong dac biet
  //       row1_TOI[10] = ConvertToString("");  //Phun cat
  //       row1_TOI[11] = ConvertToString("");  //Rua kinh
  //       row1_TOI[12] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalArea);
  //       row1_TOI[13] = "";
  //       data.push(row1_TOI);
  //       break;
  //     case "DAN":
  //       let row1_DAN: Array<any> = [];
  //       row1_DAN[0] = "TỔNG";
  //       row1_DAN[1] = "";
  //       row1_DAN[2] = "";
  //       row1_DAN[3] = "";
  //       row1_DAN[4] = "";
  //       row1_DAN[5] = ConvertToString(this.sumaryReportManuFactureByDayRemain.quantity); //so tam
  //       row1_DAN[6] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaThick); //so m2 day
  //       row1_DAN[7] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaThin); //so m2 mong
  //       row1_DAN[8] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaEspeciallyThick);  //so m2 day dac biet
  //       row1_DAN[9] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaEspeciallyThin);  //so m2 mong dac biet
  //       row1_DAN[10] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaOriginalThick);  //m2 nguyen kho day
  //       row1_DAN[11] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalAreaOriginalThin);  //m2 nguyen kho mong
  //       row1_DAN[12] = ""; //m2 cat ha day
  //       row1_DAN[13] = ""; //m2 cat ha mong
  //       row1_DAN[14] = ConvertToString(this.sumaryReportManuFactureByDayRemain.totalArea);
  //       row1_DAN[15] = "";
  //       data.push(row1_DAN);
  //       break;
  //     default:
  //       break;
  //   }
  //   return data;
  // }

  getSumaryDataReportByMonth(techRequest: techniqueRequest): Array<any> {
    let data: Array<any> = [];
    switch (techRequest.techniqueRequestCode) {
      case "CAT":
      case "MAI":
        let row1: Array<any> = [];
        row1[0] = "TỔNG";
        row1[1] = "";
        row1[2] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaThick);
        row1[3] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaThin);
        row1[4] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaEspeciallyThick);
        row1[5] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaEspeciallyThin);
        row1[6] = ConvertToString(this.sumaryReportManuFactureByMonth.totalDayShift);
        row1[7] = ConvertToString(this.sumaryReportManuFactureByMonth.totalNightShift);
        row1[8] = ConvertToString(this.sumaryReportManuFactureByMonth.totalLabor);
        data.push(row1);
        let row2: Array<any> = [];
        row2[0] = "Tổng sản lượng";
        row2[1] = "";
        row2[2] = "";
        row2[3] = "";
        row2[4] = "";
        row2[5] = "";
        row2[6] = ConvertToString(this.sumaryReportManuFactureByMonth.totalArea);
        row2[7] = "";
        row2[8] = "";
        data.push(row2);
        let row3: Array<any> = [];
        row3[0] = "Tổng m2 trung bình/người";
        row3[1] = "";
        row3[2] = "";
        row3[3] = "";
        row3[4] = "";
        row3[5] = "";
        row3[6] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaPerLabor);
        row3[7] = "";
        row3[8] = "";
        data.push(row3);
        break;
      case "KHOAN":
        let row1_KHOAN: Array<any> = [];
        row1_KHOAN[0] = "TỔNG";
        row1_KHOAN[1] = "";
        row1_KHOAN[2] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaThick);
        row1_KHOAN[3] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaThin);
        row1_KHOAN[4] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaEspeciallyThick);
        row1_KHOAN[5] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaEspeciallyThin);

        row1_KHOAN[6] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaBoreholeThick);
        row1_KHOAN[7] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaBoreholeThin);

        row1_KHOAN[8] = ConvertToString(this.sumaryReportManuFactureByMonth.totalDayShift);
        row1_KHOAN[9] = ConvertToString(this.sumaryReportManuFactureByMonth.totalNightShift);
        row1_KHOAN[10] = ConvertToString(this.sumaryReportManuFactureByMonth.totalLabor);
        data.push(row1_KHOAN);
        let row2_KHOAN: Array<any> = [];
        row2_KHOAN[0] = "Tổng sản lượng";
        row2_KHOAN[1] = "";
        row2_KHOAN[2] = "";
        row2_KHOAN[3] = "";
        row2_KHOAN[4] = "";
        row2_KHOAN[5] = "";
        row2_KHOAN[6] = "";
        row2_KHOAN[7] = "";
        row2_KHOAN[8] = ConvertToString(this.sumaryReportManuFactureByMonth.totalArea);
        row2_KHOAN[9] = "";
        row2_KHOAN[10] = "";
        data.push(row2_KHOAN);
        let row3_KHOAN: Array<any> = [];
        row3_KHOAN[0] = "Tổng m2 trung bình/người";
        row3_KHOAN[1] = "";
        row3_KHOAN[2] = "";
        row3_KHOAN[3] = "";
        row3_KHOAN[4] = "";
        row3_KHOAN[5] = "";
        row3_KHOAN[6] = "";
        row3_KHOAN[7] = "";
        row3_KHOAN[8] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaPerLabor);
        row3_KHOAN[9] = "";
        row3_KHOAN[10] = "";
        data.push(row3_KHOAN);
        break;
      case "TOI":
      case "HOP":
        let row1_TOI: Array<any> = [];
        row1_TOI[0] = "TỔNG";
        row1_TOI[1] = "";

        row1_TOI[2] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaThick);
        row1_TOI[3] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaThin);

        row1_TOI[4] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaEspeciallyThick);
        row1_TOI[5] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaEspeciallyThin);

        row1_TOI[6] = ConvertToString(''); //phun cat
        row1_TOI[7] = ConvertToString(''); //rua kinh

        row1_TOI[8] = ConvertToString(this.sumaryReportManuFactureByMonth.totalDayShift);
        row1_TOI[9] = ConvertToString(this.sumaryReportManuFactureByMonth.totalNightShift);
        row1_TOI[10] = ConvertToString(this.sumaryReportManuFactureByMonth.totalLabor);
        data.push(row1_TOI);
        let row2_TOI: Array<any> = [];
        row2_TOI[0] = "Tổng sản lượng";
        row2_TOI[1] = "";
        row2_TOI[2] = "";
        row2_TOI[3] = "";
        row2_TOI[4] = "";
        row2_TOI[5] = "";
        row2_TOI[6] = "";
        row2_TOI[7] = "";

        row2_TOI[8] = ConvertToString(this.sumaryReportManuFactureByMonth.totalArea);
        row2_TOI[9] = "";
        row2_TOI[10] = "";
        data.push(row2_TOI);
        let row3_TOI: Array<any> = [];
        row3_TOI[0] = "Tổng m2 trung bình/người";
        row3_TOI[1] = "";
        row3_TOI[2] = "";
        row3_TOI[3] = "";
        row3_TOI[4] = "";
        row3_TOI[5] = "";
        row3_TOI[6] = "";
        row3_TOI[7] = "";
        row3_TOI[8] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaPerLabor);
        row3_TOI[9] = "";
        row3_TOI[10] = "";
        data.push(row3_TOI);
        break;
      case "DAN":
        let row1_DAN: Array<any> = [];
        row1_DAN[0] = "TỔNG";
        row1_DAN[1] = "";

        row1_DAN[2] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaThick);
        row1_DAN[3] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaThin);

        row1_DAN[4] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaEspeciallyThick);
        row1_DAN[5] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaEspeciallyThin);

        row1_DAN[6] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaOriginalThick);
        row1_DAN[7] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaOriginalThin);

        row1_DAN[8] = ConvertToString(''); //phun cat
        row1_DAN[9] = ConvertToString(''); //rua kinh

        row1_DAN[10] = ConvertToString(this.sumaryReportManuFactureByMonth.totalDayShift);
        row1_DAN[11] = ConvertToString(this.sumaryReportManuFactureByMonth.totalNightShift);
        row1_DAN[12] = ConvertToString(this.sumaryReportManuFactureByMonth.totalLabor);
        data.push(row1_DAN);
        let row2_DAN: Array<any> = [];
        row2_DAN[0] = "Tổng sản lượng";
        row2_DAN[1] = "";
        row2_DAN[2] = "";
        row2_DAN[3] = "";
        row2_DAN[4] = "";
        row2_DAN[5] = "";
        row2_DAN[6] = "";
        row2_DAN[7] = "";
        row2_DAN[8] = "";
        row2_DAN[9] = "";
        row2_DAN[10] = ConvertToString(this.sumaryReportManuFactureByMonth.totalArea);
        row2_DAN[11] = "";
        row2_DAN[12] = "";
        data.push(row2_DAN);
        let row3_DAN: Array<any> = [];
        row3_DAN[0] = "Tổng m2 trung bình/người";
        row3_DAN[1] = "";
        row3_DAN[2] = "";
        row3_DAN[3] = "";
        row3_DAN[4] = "";
        row3_DAN[5] = "";
        row3_DAN[6] = "";
        row3_DAN[7] = "";
        row3_DAN[8] = "";
        row3_DAN[9] = "";
        row3_DAN[10] = ConvertToString(this.sumaryReportManuFactureByMonth.totalAreaPerLabor);
        row3_DAN[11] = "";
        row3_DAN[12] = "";
        data.push(row3_DAN);
        break;
      default:
        break;
    }
    return data;
  }

  getSumaryDataReportByYear(techRequest: techniqueRequest): Array<any> {
    let data: Array<any> = [];
    switch (techRequest.techniqueRequestCode) {
      case "CAT":
      case "MAI":
        let row1: Array<any> = [];
        row1[0] = "TỔNG";
        row1[1] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaThick);
        row1[2] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaThin);
        row1[3] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaEspeciallyThick);
        row1[4] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaEspeciallyThin);
        row1[5] = ConvertToString(this.sumaryReportManuFactureByYear.totalArea);
        data.push(row1);
        break;
      case "KHOAN":
        let row1_KHOAN: Array<any> = [];
        row1_KHOAN[0] = "TỔNG";
        row1_KHOAN[1] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaThick);
        row1_KHOAN[2] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaThin);
        row1_KHOAN[3] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaEspeciallyThick);
        row1_KHOAN[4] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaEspeciallyThin);
        row1_KHOAN[5] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaBoreholeThick);
        row1_KHOAN[6] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaBoreholeThin);
        row1_KHOAN[7] = ConvertToString(this.sumaryReportManuFactureByYear.totalArea);
        data.push(row1_KHOAN);
        break;
      case "TOI":
      case "HOP":
        let row1_TOI: Array<any> = [];
        row1_TOI[0] = "TỔNG";
        row1_TOI[1] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaThick);
        row1_TOI[2] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaThin);
        row1_TOI[3] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaEspeciallyThick);
        row1_TOI[4] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaEspeciallyThin);
        row1_TOI[5] = ConvertToString("0");
        row1_TOI[6] = ConvertToString("0");
        row1_TOI[7] = ConvertToString(this.sumaryReportManuFactureByYear.totalArea);
        data.push(row1_TOI);
        break;
      case "DAN":
        let row1_DAN: Array<any> = [];
        row1_DAN[0] = "TỔNG";
        row1_DAN[1] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaThick);
        row1_DAN[2] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaThin);
        row1_DAN[3] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaEspeciallyThick);
        row1_DAN[4] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaEspeciallyThin);
        row1_DAN[5] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaOriginalThick);
        row1_DAN[6] = ConvertToString(this.sumaryReportManuFactureByYear.totalAreaOriginalThin);
        row1_DAN[7] = ConvertToString("0");
        row1_DAN[8] = ConvertToString("0");
        row1_DAN[9] = ConvertToString(this.sumaryReportManuFactureByYear.totalArea);
        data.push(row1_DAN);
        break;
      default:
        break;
    }
    return data;
  }

  getYears() {
    for (let i = this.currentYear; i >= this.minYear; i--) {
      let item: yearModel = new yearModel();
      item.label = String(i);
      item.value = i;
      this.listyears.push(item);
    }
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch (unit) {
      case 0: {
        result = result;
        break;
      }
      case 1: {
        result = Math.round(number * 10) / 10;
        break;
      }
      case 2: {
        result = Math.round(number * 100) / 100;
        break;
      }
      case 3: {
        result = Math.round(number * 1000) / 1000;
        break;
      }
      case 4: {
        result = Math.round(number * 10000) / 10000;
        break;
      }
      default: {
        result = result;
        break;
      }
    }
    return result;
  }

  transformDate(date: Date) {
    return formatDate(date, 'dd/MM/yyyy', 'EN');
  }

  transformNumber(input: any) {
    return formatNumber(input, 'EN', '1.0-4');
  }
}

function ConvertToString(str: any) {
  if (str == undefined || str == null) return '';
  return String(str).trim();
}

function GetStartDay(date: Date) {
  if (!date) return null;
  date.setHours(0, 0, 0, 0);
  return ConvertToUTCTime(date);
}

function GetEndDay(date: Date) {
  if (!date) return null;
  date.setHours(23, 59, 59, 999);
  return ConvertToUTCTime(date);
}

function ConvertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
