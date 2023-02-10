
import { Component, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { range } from 'rxjs';

@Component({
  selector: 'app-demo-xuat-excel',
  templateUrl: './demo-xuat-excel.component.html',
  styleUrls: ['./demo-xuat-excel.component.css']
})
export class DemoXuatExcelComponent implements OnInit {
  listBaoCao: Array<any> = new Array<any>();
  loaiBaoCao: any;

  thoiGian: string;
  tenNhanVien: string;

  headerBaoCao2: Array<any> = [];
  headerBaoCao3: Array<any> = [];

  dataBaoCao1: Array<any> = [];

  dataBaoCao2bang1: Array<any> = [];
  dataBaoCao2bang2: Array<any> = [];
  dataBaoCao2bang3: Array<any> = [];
  dataBaoCao2bang3PhuLuc: Array<any> = [];
  dataBaoCao2bang4: Array<any> = [];
  dataBaoCao2bang5: Array<any> = [];

  dataBaoCao3bang1: Array<any> = [];
  dataBaoCao3bang2: Array<any> = [];
  dataBaoCao3bang3: Array<any> = [];
  dataBaoCao3bang4: Array<any> = [];
  dataBaoCao3bang5: Array<any> = [];

  constructor(private messageService: MessageService) {
    this.listBaoCao = [
      { key: 1, name: 'baocao1' },
      { key: 2, name: 'baocao2' },
      { key: 3, name: 'baocao3' },
    ];

    this.headerBaoCao2 = [
      [     // bang 1
        "Dept",
        "No of staff",
        "Basic Gross salary",
        "Bonus/ Allowances/ Others_AV",
        "Trade Union",
        "PIT",
        "PIT (Tax)",
        "Obligation insurance",
        "Net payable",
        "Total company gross payable before off-set",
        "Total company gross payable after off-set"
      ],
      [   // bang 2
        "Dept",
        "No of staff",
        "Gross salary",
        "Bonus/ Allowances/ Others_AV",
        "Trade Union - ER",
        "PIT",
        "PIT (Tax)",
        "Obligation insurance",
        "Net payroll",
        "Total company gross payable_DF"
      ],
      [   // bang 3
        "Grade",
        "Number of employees",
        "GROSS BASE SALARY",
        "AVERAGE-GROSS BASE SALARY",
        "TARGET Average BASIC SALARY STRUCTURE",
        "",
        "Actual vs Target"
      ],
      [   // bang 4
        "Grade",
        "Number of employees",
        "GROSS BASE + OTHER ALLOWANCE",
        "AVERAGE-GROSS BASE + OTHER ALLOWANCE",
        "TARGET_Average GROSS + OTHER ALLOWANCE",
        "",
        "Actual vs Target"
      ],
      [   // bang 5
        "Grade",
        "Number of employees",
        "TOTAL LUMPSUM GROSS PAYOUT",
        "AVERAGE TOTAL LUMPSUM GROSS PAYOUT",
        "TARGET_Average GROSS + OTHER ALLOWANCE",
        "",
        "Actual vs Target"
      ]
    ];

    this.headerBaoCao3 = [
      [     // bang 1
        "No.",
        "HR Filing Code",
        "Dept Code",
        "No. of staff",
        "Group Health Care",
        "Monthly Bonus/ Incentive Accrual Local Currency",
        "Actual working days",
        "Actual working days in vocational / Probation- 1.2020",
        "AL",
        "Holiday",
        "Other paid leave with full paid",
        "Unpaid Leave, paid by social innurance",
        "Unpaid leave without reason",
        "Unpaid leave",
        "Deduction days for attendance/ Deduction days for non- compliance of regulation",
        "Total payable day for this month, including Holiday (Mon-Fri)",
        "PAYBACK - GROSS BASIC IF ANY",
        "NEW GROSS BASED ON ACTUAL WORKING DAY",
        "GROSS OT",
        "GROSS ALLOWANCE (Cell phone, Housing allowance, lunch, attendance reward)",
        "GROSS BONUS",
        "NET SEVERANCE ALLOWANCE",
        "DEDUCTION FROM EMPLOYEE/ Refund company before Tax (Over paid AL) Các khoản giảm trừ trước thuế nếu có",
        "TOTAL PIT PAYMENT_Borne by employee",
        "PIT born by company &employee after off-set PIT liquidation",
        "UI",
        "UI",
        "SI",
        "SI",
        "HI",
        "HI",
        "Other HI - Additional",
        "Total company and employee_SI, AI, UI and HI _32%",
        "Total company and employee_SI, AI, UI and HI _32%",
        "Total company and employee_SI, AI, UI and HI _32%",
        "T.U_Company ",
        "Other deduction after Tax",
        "Other Additional after Tax for employee",
        "TOTAL NET INCOME PAYABLE",
        "Net pay before New Year",
        "Net_Paid 26th Jul",
        "TOTAL GROSS PAYABLE - BEFORE OFFSET PIT IF ANY",
        "TOTAL GROSS PAYABLE-AFTER OFFSET PIT IF ANY",
        "Bonus accrual from Jan-Dec",
        "Bonus accrual from Jan-Dec",
      ],
      [     // bang 2
        'Dept Code',
        'No. of HC',
      ],
      [     // bang 3
        'Description',
        'Before PIT adjustment',
        'PIT overpaid/under paid',
        'After PIT adjustment',
        'Diff',
      ],
      [     // bang 4
        "No.",
        "HR Filing Code",
        "Dept Code",
        "No. of staff",
        "Group Health Care",
        "Monthly Bonus/ Incentive Accrual Local Currency",
        "Actual working days",
        "Actual working days in vocational / Probation- 1.2020",
        "AL",
        "Holiday",
        "Other paid leave with full paid",
        "Unpaid Leave, paid by social innurance",
        "Unpaid leave without reason",
        "Unpaid leave",
        "Deduction days for attendance/ Deduction days for non- compliance of regulation",
        "Total payable day for this month, including Holiday (Mon-Fri)",
        "PAYBACK - GROSS BASIC IF ANY",
        "NEW GROSS BASED ON ACTUAL WORKING DAY",
        "GROSS OT",
        "GROSS ALLOWANCE (Cell phone, Housing allowance, lunch, attendance reward)",
        "GROSS BONUS",
        "NET SEVERANCE ALLOWANCE",
        "DEDUCTION FROM EMPLOYEE/ Refund company before Tax (Over paid AL) Các khoản giảm trừ trước thuế nếu có",
        "TOTAL PIT PAYMENT_Borne by employee",
        "PIT born by company &employee after off-set PIT liquidation",
        "UI",
        "UI",
        "SI",
        "SI",
        "HI",
        "HI",
        "Other HI - Additional",
        "Total company and employee_SI, AI, UI and HI _32%",
        "Total company and employee_SI, AI, UI and HI _32%",
        "Total company and employee_SI, AI, UI and HI _32%",
        "T.U_Company ",
        "Other deduction after Tax",
        "Other Additional after Tax",
        "TOTAL NET INCOME PAYABLE",
        "Net pay before New Year",
        "Net_Paid 10th Feb",
        "TOTAL GROSS PAYABLE - BEFORE OFFSET PIT LIQUIDATION",
        "TOTAL GROSS PAYABLE-AFTER OFFSET PIT LIQUIDATION",
      ],
      [     // bang 5
        "No.",
        "HR Filing Code",
        "Dept Code",
        "No. of staff",
        "Group Health Care",
        "Monthly Bonus/ Incentive Accrual Local Currency",
        "Actual working days",
        "Actual working days in vocational / Probation- 1.2020",
        "AL",
        "Holiday",
        "Other paid leave with full paid",
        "Unpaid Leave, paid by social innurance",
        "Unpaid leave without reason",
        "Unpaid leave",
        "Deduction days for attendance/ Deduction days for non- compliance of regulation",
        "Total payable day for this month, including Holiday (Mon-Fri)",
        "PAYBACK - GROSS BASIC IF ANY",
        "NEW GROSS BASED ON ACTUAL WORKING DAY",
        "GROSS OT",
        "GROSS ALLOWANCE (Cell phone, Housing allowance, lunch, attendance reward)",
        "GROSS BONUS",
        "NET SEVERANCE ALLOWANCE",
        "DEDUCTION FROM EMPLOYEE/ Refund company before Tax (Over paid AL) Các khoản giảm trừ trước thuế nếu có",
        "TOTAL PIT PAYMENT_Borne by employee",
        "PIT born by company &employee after off-set PIT liquidation",
        "UI",
        "UI",
        "SI",
        "SI",
        "HI",
        "HI",
        "Other HI - Additional",
        "Total company and employee_SI, AI, UI and HI _32%",
        "Total company and employee_SI, AI, UI and HI _32%",
        "Total company and employee_SI, AI, UI and HI _32%",
        "T.U_Company ",
        "Other deduction after Tax",
        "Other Additional after Tax",
        "TOTAL NET INCOME PAYABLE",
        "Net pay before New Year",
        "Net_Paid 26th Jul",
        "TOTAL GROSS PAYABLE - BEFORE OFFSET PIT LIQUIDATION",
        "TOTAL GROSS PAYABLE-AFTER OFFSET PIT LIQUIDATION",
      ],

    ];
  }

  ngOnInit(): void {
  }


  async getMasterData(reportType: number) {
    if (reportType == 1) {
      this.dataBaoCao1 = [
        {
          paymentItems: 'Salary',
          aprCDActual: 8575,
          mayForecast: 427554,
          mayCDActual: 542132,
          vsActualInApr: 42245,
          vsForecast: 521214,
          note: null
        },
        {
          paymentItems: 'Trade Union',
          aprCDActual: 21245864000,
          mayForecast: -786786,
          mayCDActual: 660000,
          vsActualInApr: 87658763,
          vsForecast: 732575,
          note: null
        },
        {
          paymentItems: 'Obligation insurance (SI, HI, UI, AI)',
          aprCDActual: 21864000,
          mayForecast: -22384000,
          mayCDActual: 66074752000,
          vsActualInApr: 72872,
          vsForecast: 4564,
          note: null
        },
        {
          paymentItems: 'PIT',
          aprCDActual: 275,
          mayForecast: 22384000,
          mayCDActual: 660000,
          vsActualInApr: -4654654,
          vsForecast: 7287287,
          note: null
        },
        {
          paymentItems: 'Total',
          aprCDActual: 754545,
          mayForecast: 22384000,
          mayCDActual: 660000,
          vsActualInApr: 4654654,
          vsForecast: 45654,
          note: null,
        },
      ];

    } else if (reportType == 2) {
      this.thoiGian = 'MARCH 2022';
      this.tenNhanVien = 'NGUYEN THI TRANG';
      this.dataBaoCao2bang1 = [   // length = 5
        {
          dept: 'G&A',
          noOfStaff: 1,
          basicGroupSalary: 10000000,
          bonus: 5510000,
          tradeUnion: null,
          pit: 1011000,
          pitTax: 1011000,
          obligationInsurance: null,
          netPayable: 14499000,
          totalBefore: 15510000,
          totalAfter: null
        },
        {
          dept: 'OPS',
          noOfStaff: 1,
          basicGroupSalary: 14212500,
          bonus: 15084258,
          tradeUnion: 300000,
          pit: 735201,
          pitTax: 735201,
          obligationInsurance: 4575000,
          netPayable: 26986557,
          totalBefore: 32596758,
          totalAfter: 83567,
        },
        {
          dept: 'COS',
          noOfStaff: 1,
          basicGroupSalary: 14212500,
          bonus: 15084258,
          tradeUnion: 300000,
          pit: 735201,
          pitTax: 735201,
          obligationInsurance: 4575000,
          netPayable: 26986557,
          totalBefore: 32596758,
          totalAfter: 54725,
        },
        {
          dept: 'TOTAL VND',
          noOfStaff: 1,
          basicGroupSalary: 14212500,
          bonus: 15084258,
          tradeUnion: 300000,
          pit: 735201,
          pitTax: 735201,
          obligationInsurance: 4575000,
          netPayable: 26986557,
          totalBefore: 32596758,
          totalAfter: null,
        },
        {
          dept: 'CHECK',
          noOfStaff: 1,
          basicGroupSalary: 14212500,
          bonus: 15084258,
          tradeUnion: 300000,
          pit: 735201,
          pitTax: 735201,
          obligationInsurance: 4575000,
          netPayable: 26986557,
          totalBefore: 32596758,
          totalAfter: null,
        }
      ];
      this.dataBaoCao2bang2 = [   // length = 5
        {
          dept: 'Junior',
          noOfStaff: 1,
          basicGroupSalary: 10000000,
          bonus: 5510000,
          tradeUnion: null,
          pit: 1011000,
          pitTax: 1011000,
          obligationInsurance: null,
          netPayable: 14499000,
          totalBefore: 15510000,
        },
        {
          dept: 'Mid',
          noOfStaff: 1,
          basicGroupSalary: 14212500,
          bonus: 15084258,
          tradeUnion: 300000,
          pit: 735201,
          pitTax: 735201,
          obligationInsurance: 4575000,
          netPayable: 26986557,
          totalBefore: 32596758,
        },
        {
          dept: 'Senior',
          noOfStaff: 1,
          basicGroupSalary: 14212500,
          bonus: 15084258,
          tradeUnion: 300000,
          pit: 735201,
          pitTax: 735201,
          obligationInsurance: 4575000,
          netPayable: 26986557,
          totalBefore: 32596758,
        },
        {
          dept: 'TOTAL VND',
          noOfStaff: 1,
          basicGroupSalary: 14212500,
          bonus: 15084258,
          tradeUnion: 300000,
          pit: 735201,
          pitTax: 735201,
          obligationInsurance: 4575000,
          netPayable: 26986557,
          totalBefore: 32596758,
        },
        {
          dept: 'CHECK',
          noOfStaff: 1,
          basicGroupSalary: 14212500,
          bonus: 15084258,
          tradeUnion: 300000,
          pit: 735201,
          pitTax: 735201,
          obligationInsurance: 4575000,
          netPayable: 26986557,
          totalBefore: 32596758,
        }
      ];
      this.dataBaoCao2bang3 = [   // length = 4
        {
          grade: 'Junior',
          numberEmployees: 1,
          grossBase: 10000000,
          average: 5510000,
          target: 5875532,
          actualVsTarget: 0.5,
        },
        {
          grade: 'Mid',
          numberEmployees: 1,
          grossBase: 10000000,
          average: 5510000,
          target: 43554354,
          actualVsTarget: 0.2,
        },
        {
          grade: 'Senior',
          numberEmployees: 1,
          grossBase: 10000000,
          average: 5510000,
          target: 43554354,
          actualVsTarget: 0.9,
        },
        {
          grade: 'Total',
          numberEmployees: 3,
          grossBase: 10000000,
          average: 5510000,
          target: 453453453,
          actualVsTarget: 1.8,
        },
      ];
      this.dataBaoCao2bang3PhuLuc = [
        {
          h30: 4646,
          h31: 4687468,
          i30: 6735,
          i31: 746546,
          j30: 65454,
        },
      ];
      this.dataBaoCao2bang4 = [   // length = 4
        {
          grade: 'Junior',
          numberEmployees: 1,
          grossBase: 10000000,
          average: 5510000,
          target: 43543453,
          actualVsTarget: 0.85,
        },
        {
          grade: 'Mid',
          numberEmployees: 1,
          grossBase: 10000000,
          average: 5510000,
          target: 27378287,
          actualVsTarget: 1.25,
        },
        {
          grade: 'Senior',
          numberEmployees: 1,
          grossBase: 10000000,
          average: 5510000,
          target: 2783275327,
          actualVsTarget: 1.01,
        },
        {
          grade: 'Total',
          numberEmployees: 3,
          grossBase: 10000000,
          average: 5510000,
          target: 753237527,
          actualVsTarget: 2.2,
        },
      ];
      this.dataBaoCao2bang5 = [   // length = 4
        {
          grade: 'Junior',
          numberEmployees: 1,
          grossBase: 10000000,
          average: 5510000,
          target: 56564,
          actualVsTarget: 1.2,
        },
        {
          grade: 'Mid',
          numberEmployees: 1,
          grossBase: 10000000,
          average: 5510000,
          target: 3453,
          actualVsTarget: 0.95,
        },
        {
          grade: 'Senior',
          numberEmployees: 1,
          grossBase: 10000000,
          average: 5510000,
          target: 435434,
          actualVsTarget: 0.25,
        },
        {
          grade: 'Total',
          numberEmployees: 3,
          grossBase: 10000000,
          average: 5510000,
          target: 345345,
          actualVsTarget: 1.25,
        },
      ];

    } else if (reportType == 3) {
      this.thoiGian = 'MARCH 2022';
      this.tenNhanVien = 'NGUYEN THI TRANG';
      this.dataBaoCao3bang1 = [   // length = 5
        {
          b: 'G&A',
          c: 'G&A',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
          ar: 4946,
          as: 6546,
        },
        {
          b: 'OPS',
          c: 'OPS',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
          ar: 4946,
          as: 6546,
        },
        {
          b: 'COS',
          c: 'COS',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
          ar: 4946,
          as: 6546,
        },
        {
          b: 'TOTAL',
          c: null,
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
          ar: 4946,
          as: 6546,
        },
        {
          b: 'HC note',
          c: 'Staffs',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
          ar: 4946,
          as: 6546,
        },
      ];
      this.dataBaoCao3bang2 = [   // length = 4
        {
          dept: 'G&A',
          noOfHC: 1,
        },
        {
          dept: 'OPS',
          noOfHC: 1,
        },
        {
          dept: 'COS',
          noOfHC: 1,
        },
        {
          dept: 'Total HC',
          noOfHC: 3,
        },
      ];
      this.dataBaoCao3bang3 = [   // length = 6
        {
          desc: 'Total gross-income',
          beforePIT: 5656485.2,
          overpaidPIT: 564565.0,
          afterPIT: 6446.5,
          diff: 54646,
        },
        {
          desc: 'PIT of the month (EE)',
          beforePIT: 5656485.2,
          overpaidPIT: 564565.0,
          afterPIT: 6446.5,
          diff: 54646,
        },
        {
          desc: 'PIT of the month (ER)',
          beforePIT: 5656485.2,
          overpaidPIT: 564565.0,
          afterPIT: 6446.5,
          diff: 54646,
        },
        {
          desc: 'SHUI (EE)',
          beforePIT: 5656485.2,
          overpaidPIT: 564565.0,
          afterPIT: 6446.5,
          diff: 54646,
        },
        {
          desc: 'Net receiving',
          beforePIT: 5656485.2,
          overpaidPIT: 564565.0,
          afterPIT: 6446.5,
          diff: 54646,
        },
        {
          desc: 'Diff',
          beforePIT: 0.4,
        }
      ];
      this.dataBaoCao3bang4 = [   // length = 11
        {
          b: 'G&A',
          c: 'CM',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
        {
          b: 'G&A',
          c: 'HRD',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
        {
          b: 'G&A',
          c: 'G&A-HR',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
        {
          b: 'G&A',
          c: 'G&A-ACC',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
        {
          b: 'G&A',
          c: 'G&A-AD',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
        {
          b: 'OPS',
          c: 'OPS-PM',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
        {
          b: 'OPS',
          c: 'OPS-IT',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
        {
          b: 'COS',
          c: 'COS-3D',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
        {
          b: 'COS',
          c: 'COS-QA',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
        {
          b: 'TOTAL',
          c: null,
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
        {
          b: null,
          c: null,
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
      ];
      this.dataBaoCao3bang5 = [   // length = 5
        {
          b: 'COS-3D',
          c: 'Mid',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
        {
          b: 'COS-3D',
          c: 'Junior',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
        {
          b: 'COS-3D',
          c: 'Senior',
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
        {
          b: 'TOTAL',
          c: null,
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
        {
          b: null,
          c: null,
          d: 27,
          e: 24,
          f: 542,
          g: 525,
          h: 65,
          i: 6,
          j: 131,
          k: 65416541,
          l: 465465,
          m: 65465,
          n: 465465,
          o: 4646,
          p: 465465,
          q: 46465,
          r: 646,
          s: 4646465,
          t: 56465,
          u: 4646,
          v: 4646,
          w: 4646,
          x: 646,
          y: 46621,
          z: 6546,
          aa: 6464,
          ab: 494,
          ac: 4949,
          ad: 46665,
          ae: 6562165,
          af: 549849,
          ag: 95565,
          ah: 94565465,
          ai: 95498,
          aj: 49465,
          ak: 46546,
          al: 64655,
          am: 498456,
          an: 945646,
          ao: 498498,
          ap: 9494,
          aq: 49465,
        },
      ]
    }
  }

  /* Chọn loại báo cáo và xuất file Excel theo yêu cầu */
  async exportExcel() {
    if (!this.loaiBaoCao) {
      this.showMessage("error", 'Bạn chưa chọn file');
      return;
    }
    this.showMessage("success", this.loaiBaoCao.name);

    if (this.loaiBaoCao.key == 1) {
      await this.getMasterData(1);
      this.exportExcel1();
    } else if (this.loaiBaoCao.key == 2) {
      await this.getMasterData(2);
      this.exportExcel2();
    } else if (this.loaiBaoCao.key == 3) {
      await this.getMasterData(3);
      this.exportExcel3();
    }
  }


  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }

  /* Xuat bao cao "In Comparison with forecast" */
  exportExcel1() {
    if (this.dataBaoCao1.length > 0) {
      let title = 'In Comparison with forecast';
      let workbook = new Workbook();
      let worksheet = workbook.addWorksheet(title);

      /* header table */
      let headerBaoCao1 = [
        "No.",
        "Payment items",
        "Apr. CD actual (163)",
        "May Forecast",
        "May. CD actual (196)",
        "Vs actual in Apr",
        "Vs forecast",
        "Note",
      ];

      let headerRow1 = worksheet.addRow(headerBaoCao1);

      headerRow1.height = 70;
      headerRow1.font = { name: 'Times New Roman', size: 11, bold: true };

      headerBaoCao1.forEach((item, index) => {
        headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

        if (index == 4 || index == 5) {
          headerRow1.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'FF0000' }
          };
        } else {
          headerRow1.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'FABF8F' }
          };
        }
      });

      /* data table */
      let data = this.dataBaoCao1.map((item, index) => {
        let row = [];

        if (index == 4) {
          row.push('');
          row.push('');
          row.push(item.aprCDActual);
          row.push(item.mayForecast);
          row.push(item.mayCDActual);
          row.push(item.vsActualInApr);
          row.push(item.vsForecast);
          row.push('');
        } else {
          row.push(index + 1);
          row.push(item.paymentItems);
          row.push(item.aprCDActual);
          row.push(item.mayForecast);
          row.push(item.mayCDActual);
          row.push(item.vsActualInApr);
          row.push(item.vsForecast);
          row.push(item.note);
        }

        return row;
      });

      data.forEach((el, index) => {
        let row = worksheet.addRow(el);
        row.font = { name: 'Times New Roman' };
        if (index == 4) {
          row.font = { bold: true };
          row.height = 30;
        }

        for (let i = 0; i < el.length; i++) {
          row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

          if (i == 0) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          }
          else if (i == 1 || i == 7) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
          }
          else {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).numFmt = '#,##0_);(#,##0)';
          }
        }
      });
      worksheet.mergeCells('A6:B6');

      /* set width */
      worksheet.getColumn(1).width = 10;
      worksheet.getColumn(2).width = 35;
      worksheet.getColumn(8).width = 100;

      for (let i = 1; i <= headerBaoCao1.length; i++) {
        if (i > 2 && i < 8) {
          worksheet.getColumn(i).width = 15;
        }
      }

      workbook.xlsx.writeBuffer().then((data) => {
        let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        saveAs.saveAs(blob, title);
      });
    }
    else {
      this.showMessage("error", 'Không có dữ liệu');
    }
  }

  /* Xuat bao cao "Cost Center_Loft" */
  exportExcel2() {
    if (this.dataBaoCao2bang1.length > 0 && this.dataBaoCao2bang2.length > 0 && this.dataBaoCao2bang3.length > 0 && this.dataBaoCao2bang4.length > 0 && this.dataBaoCao2bang5.length > 0) {
      let title = 'Cost Center_Loft';
      let workbook = new Workbook();
      let worksheet = workbook.addWorksheet(title);

      /* table 1 */
      /* title table 1 */
      let titleRow = worksheet.addRow(['SUMMARY BY CODE CENTER - LOFT']);
      titleRow.font = { name: 'Times New Roman', family: 4, size: 30, bold: true };
      titleRow.height = 40;
      worksheet.mergeCells(`A${titleRow.number}:K${titleRow.number}`)
      titleRow.alignment = { vertical: 'middle', horizontal: 'left' };

      titleRow = worksheet.addRow([`PAYROL OF ${this.thoiGian}`]);
      titleRow.font = { name: 'Times New Roman', family: 4, size: 15, bold: true };
      titleRow.height = 20;
      worksheet.mergeCells(`A${titleRow.number}:K${titleRow.number}`)
      titleRow.alignment = { vertical: 'middle', horizontal: 'left' };

      /* header table 1 */
      let headerRow1 = worksheet.addRow(this.headerBaoCao2[0]);

      headerRow1.height = 80;
      headerRow1.font = { name: 'Times New Roman', size: 15, bold: true };

      this.headerBaoCao2[0].forEach((item, index) => {
        headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

        if (index == 9) {
          headerRow1.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'FFFF00' }
          };
        } else if (index >= 4 && index <= 8) {
          headerRow1.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
          headerRow1.getCell(index + 1).font = {
            name: 'Times New Roman', size: 15, bold: true, color: { argb: 'C00000' }
          }
        } else {
          headerRow1.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
        }
      });

      /* data table 1*/
      let data1 = this.dataBaoCao2bang1.map((item, index) => {
        let row = [];

        row.push(item.dept);
        row.push(item.noOfStaff);
        row.push(item.basicGroupSalary);
        row.push(item.bonus);
        row.push(item.tradeUnion);
        row.push(item.pit);
        row.push(item.pitTax);
        row.push(item.obligationInsurance);
        row.push(item.netPayable);
        row.push(item.totalBefore);
        row.push(item.totalAfter);

        return row;
      });


      data1.forEach((el, index) => {
        let row = worksheet.addRow(el);
        row.height = 40;
        row.font = { name: 'Times New Roman', size: 13, bold: true };

        for (let i = 0; i < el.length; i++) {
          row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

          if (i == 0) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          } else if (i >= 4 && i <= 8) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
            row.getCell(i + 1).font = { name: 'Times New Roman', size: 13, bold: true, color: { argb: 'C00000' } };
            row.getCell(i + 1).numFmt = '#,##0_);(#,##0)';
          } else {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).numFmt = '#,##0_);(#,##0)';
          }
        }
      });

      for (var i = 0; i < 5; i++) {          // cach khoang 5 dong
        let space = worksheet.addRow(['']);
        space.height = 30;
      }

      /* table 2 */
      /* title table 2 */
      let titleRow2 = worksheet.addRow(['SUMMARY BY GRADE - 3D & QA']);
      titleRow2.font = { name: 'Times New Roman', family: 4, size: 30, bold: true };
      titleRow2.height = 40;
      worksheet.mergeCells(`A${titleRow2.number}:K${titleRow2.number}`)
      titleRow2.alignment = { vertical: 'middle', horizontal: 'left' };


      /* header table 2 */
      let headerRow2 = worksheet.addRow(this.headerBaoCao2[1]);

      headerRow2.height = 70;
      headerRow2.font = { name: 'Times New Roman', size: 15, bold: true };

      this.headerBaoCao2[1].forEach((item, index) => {
        headerRow2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow2.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

        if (index == 10) {
          headerRow2.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'FFFF00' }
          };
        } else {
          headerRow2.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
        }
      });

      /* data table 2*/
      let data2 = this.dataBaoCao2bang2.map((item, index) => {
        let row = [];

        row.push(item.dept);
        row.push(item.noOfStaff);
        row.push(item.basicGroupSalary);
        row.push(item.bonus);
        row.push(item.tradeUnion);
        row.push(item.pit);
        row.push(item.pitTax);
        row.push(item.obligationInsurance);
        row.push(item.netPayable);
        row.push(item.totalBefore);

        return row;
      });


      data2.forEach((el, index) => {
        let row = worksheet.addRow(el);
        row.height = 30;
        row.font = { name: 'Times New Roman', size: 13, bold: true };

        for (let i = 0; i < el.length; i++) {
          row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

          if (i == 0) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          } else {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).numFmt = '#,##0_);(#,##0)';
          }
        }

        if (index == 3) {
          row.font = { name: 'Times New Roman', size: 13, bold: true, color: { argb: 'C00000' } };
        }
      });

      worksheet.addRow(['']);
      worksheet.addRow(['']);
      worksheet.addRow(['']);
      worksheet.addRow(['']);
      worksheet.addRow(['']);
      worksheet.addRow(['']);

      /* table 3 */
      /* title table 3 */
      let titleRow3 = worksheet.addRow(['COMPARISION - BASIC PAYOUT VS TARGET']);
      titleRow3.font = { name: 'Calibri', family: 4, size: 14, bold: true };
      titleRow3.height = 25;
      worksheet.mergeCells(`A${titleRow3.number}:E${titleRow3.number}`)
      titleRow3.alignment = { vertical: 'middle', horizontal: 'left' };

      /* header table 3 */
      let headerRow3 = worksheet.addRow(this.headerBaoCao2[2]);

      headerRow3.height = 70;
      headerRow3.font = { name: 'Calibri', size: 11, bold: true };

      this.headerBaoCao2[2].forEach((item, index) => {

        if (index != 5) {
          headerRow3.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          headerRow3.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          headerRow3.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
        }
      });

      /* data table 3*/
      let data3 = this.dataBaoCao2bang3.map((item, index) => {
        let row = [];

        row.push(item.grade);
        row.push(item.numberEmployees);
        row.push(item.grossBase);
        row.push(item.average);
        row.push(item.target);
        row.push(null);
        row.push(item.actualVsTarget);

        return row;
      });


      data3.forEach((el, index) => {
        let row = worksheet.addRow(el);
        row.height = 20;
        row.font = { name: 'Arial', size: 11 };

        for (let i = 0; i < el.length; i++) {
          if (i != 5) {
            row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          }

          if (i == 0) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
            row.getCell(i + 1).font = { name: 'Calibri', size: 11, bold: true };
          } else if (i == 6) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).numFmt = '0%';
          } else {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).numFmt = '#,##0_);(#,##0)';
          }
        }

        if (index == 3) {
          row.font = { name: 'Calibri', size: 11, bold: true, color: { argb: 'C00000' } };
        }
      });

      /* phụ lục bảng 3 */
      var cell = worksheet.getCell('H30');
      cell.value = this.dataBaoCao2bang3PhuLuc[0].h30;
      cell.font = { name: 'Arial', size: 11, bold: true };
      cell.numFmt = '_(* #,##0_);_(* (#,##0);_(* "-"??_);_(@_)';
      cell.alignment = { vertical: 'middle', horizontal: 'right', wrapText: false };

      cell = worksheet.getCell('H31');
      cell.value = this.dataBaoCao2bang3PhuLuc[0].h31;
      cell.font = { name: 'Arial', size: 11, bold: true };
      cell.numFmt = '_(* #,##0_);_(* (#,##0);_(* "-"??_);_(@_)';
      cell.alignment = { vertical: 'middle', horizontal: 'right', wrapText: false };

      cell = worksheet.getCell('I30');
      cell.value = this.dataBaoCao2bang3PhuLuc[0].i30;
      cell.font = { name: 'Arial', size: 11, bold: true };
      cell.numFmt = '_($* #,##0.0_);_($* (#,##0.0);_($* "-"??_);_(@_)';
      cell.alignment = { vertical: 'middle', horizontal: 'right', wrapText: false };

      cell = worksheet.getCell('I31');
      cell.value = this.dataBaoCao2bang3PhuLuc[0].i31;
      cell.font = { name: 'Arial', size: 11, bold: true };
      cell.numFmt = '_($* #,##0.0_);_($* (#,##0.0);_($* "-"??_);_(@_)';
      cell.alignment = { vertical: 'middle', horizontal: 'right', wrapText: false };

      cell = worksheet.getCell('J30');
      cell.value = this.dataBaoCao2bang3PhuLuc[0].j30;
      cell.font = { name: 'Arial', size: 11, bold: true };
      cell.numFmt = '_($* #,##0.0_);_($* (#,##0.0);_($* "-"??_);_(@_)';
      cell.alignment = { vertical: 'middle', horizontal: 'right', wrapText: false };


      worksheet.addRow(['']);
      worksheet.addRow(['']);


      /* table 4 */
      /* title table 4 */
      let titleRow4 = worksheet.addRow(['COMPARISION - TOTAL GROSS BASE + BENEFITS PAYOUT VS TARGET']);
      titleRow4.font = { name: 'Calibri', family: 4, size: 14, bold: true };
      titleRow4.height = 25;
      worksheet.mergeCells(`A${titleRow4.number}:E${titleRow4.number}`)
      titleRow4.alignment = { vertical: 'middle', horizontal: 'left', wrapText: false };

      /* header table 4 */
      let headerRow4 = worksheet.addRow(this.headerBaoCao2[3]);

      headerRow4.height = 70;
      headerRow4.font = { name: 'Calibri', size: 11, bold: true };

      this.headerBaoCao2[3].forEach((item, index) => {

        if (index != 5) {
          headerRow4.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          headerRow4.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          headerRow4.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
        }
      });

      /* data table 4*/
      let data4 = this.dataBaoCao2bang4.map((item, index) => {
        let row = [];

        row.push(item.grade);
        row.push(item.numberEmployees);
        row.push(item.grossBase);
        row.push(item.average);
        row.push(item.target);
        row.push(null);
        row.push(item.actualVsTarget);

        return row;
      });


      data4.forEach((el, index) => {
        let row = worksheet.addRow(el);
        row.height = 20;
        row.font = { name: 'Arial', size: 11 };


        for (let i = 0; i < el.length; i++) {
          if (i != 5) {
            row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          }

          if (i == 0) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
            row.getCell(i + 1).font = { name: 'Calibri', size: 11, bold: true };
          } else if (i == 6) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).numFmt = '0%';
          } else {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).numFmt = '#,##0_);(#,##0)';
          }
        }
        if (index == 3) {
          row.font = { name: 'Calibri', size: 11, bold: true, color: { argb: 'C00000' } };
        }
      });

      worksheet.addRow(['']);
      worksheet.addRow(['']);


      /* table 5 */
      /* title table 5 */
      let titleRow5 = worksheet.addRow(['COMPARISION - TOTAL LUMPSUM GROSS (Net + PIT, Government fee) VS TARGET']);
      titleRow5.font = { name: 'Calibri', family: 4, size: 14, bold: true };
      titleRow5.height = 25;
      worksheet.mergeCells(`A${titleRow5.number}:E${titleRow5.number}`)
      titleRow5.alignment = { vertical: 'middle', horizontal: 'left', wrapText: false };

      /* header table 5 */
      let headerRow5 = worksheet.addRow(this.headerBaoCao2[4]);

      headerRow5.height = 70;
      headerRow5.font = { name: 'Calibri', size: 11, bold: true };

      this.headerBaoCao2[4].forEach((item, index) => {

        if (index != 5) {
          headerRow5.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          headerRow5.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          headerRow5.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
        }
      });

      /* data table 5*/
      let data5 = this.dataBaoCao2bang5.map((item, index) => {
        let row = [];

        row.push(item.grade);
        row.push(item.numberEmployees);
        row.push(item.grossBase);
        row.push(item.average);
        row.push(item.target);
        row.push(null);
        row.push(item.actualVsTarget);

        return row;
      });


      data5.forEach((el, index) => {
        let row = worksheet.addRow(el);
        row.height = 20;
        row.font = { name: 'Arial', size: 11 };


        for (let i = 0; i < el.length; i++) {
          if (i != 5) {
            row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          }

          if (i == 0) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
            row.getCell(i + 1).font = { name: 'Calibri', size: 11, bold: true };
          } else if (i == 6) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).numFmt = '0%';
          } else {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).numFmt = '#,##';
          }
        }

        if (index == 3) {
          row.font = { name: 'Calibri', size: 11, bold: true, color: { argb: 'C00000' } };
        }
      });

      worksheet.addRow(['']);
      worksheet.addRow(['']);
      worksheet.addRow(['']);


      /* end sheet */
      let sign = worksheet.addRow(['PREPARED BY:']);
      sign.font = { name: 'Times New Roman', size: 11, bold: true };
      sign.getCell(1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: false };
      worksheet.addRow(['']);
      worksheet.addRow(['']);
      worksheet.addRow(['']);
      worksheet.addRow(['']);
      sign = worksheet.addRow([`${this.tenNhanVien}`]);
      sign.font = { name: 'Times New Roman', size: 11, bold: true };
      sign.getCell(1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: false };

      /* set width */
      worksheet.getColumn(2).width = 15;

      for (let i = 1; i <= this.headerBaoCao2[0].length; i++) {
        if (i != 2) {
          worksheet.getColumn(i).width = 20;
        }
      }

      workbook.xlsx.writeBuffer().then((data) => {
        let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        saveAs.saveAs(blob, title);
      });
    } else {
      this.showMessage("error", 'Không có dữ liệu');
    }
  }


  /* Xuat bao cao "Cost Center_RSM" */
  exportExcel3() {
    let excelColumn = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'AA', 'AB', 'AC', 'AD', 'AE', 'AF', 'AG', 'AH', 'AI', 'AJ', 'AK', 'AL', 'AM', 'AN', 'AO', 'AP', 'AQ', 'AR', 'AS', 'AT', 'AU', 'AV', 'AW', 'AX', 'AY', 'AZ'];
    if (this.dataBaoCao3bang1.length > 0 && this.dataBaoCao3bang2.length > 0 && this.dataBaoCao3bang3.length > 0 && this.dataBaoCao3bang4.length > 0 && this.dataBaoCao3bang5.length > 0) {
      let title = 'Cost Center_RSM';
      let workbook = new Workbook();
      let worksheet = workbook.addWorksheet(title);

      /* title */
      let titleSheet = worksheet.addRow(['', `PAYROL OF ${this.thoiGian}`]);
      titleSheet.font = { name: 'Times New Roman', family: 4, size: 30, bold: true };
      titleSheet.height = 50;
      worksheet.mergeCells(`B${titleSheet.number}:T${titleSheet.number}`)
      titleSheet.alignment = { vertical: 'middle', horizontal: 'left' };
      // danh so thu tu
      let space = worksheet.addRow([]);
      for (var i = 1; i < 45; i++) {
        space.getCell(i + 1).value = i;
      }
      space.height = 30;
      space.font = { name: 'Times New Roman', size: 15, bold: true };
      space.alignment = { vertical: 'middle', horizontal: 'center' };

      /* table 1 */
      /* title table 1 */
      let titleRow = worksheet.addRow(['', 'SUMMARY BY CODE CENTER - RSM']);
      titleRow.font = { name: 'Times New Roman', family: 4, size: 30, bold: true };
      titleRow.height = 50;
      worksheet.mergeCells(`B${titleRow.number}:K${titleRow.number}`);
      titleRow.alignment = { vertical: 'middle', horizontal: 'left' };


      /* header table 1 */
      let headerRow1 = worksheet.addRow(this.headerBaoCao3[0]);
      let headerRow2 = worksheet.addRow([]);

      headerRow1.height = 50;
      headerRow1.font = { name: 'Times New Roman', size: 15, bold: true };
      headerRow2.height = 140;
      headerRow2.font = { name: 'Times New Roman', size: 15, bold: true };

      for (var i = 25; i <= 29; i += 2) {
        worksheet.mergeCells(`${excelColumn[i]}4:${excelColumn[i + 1]}4`);
      }
      worksheet.mergeCells(`${excelColumn[32]}4:${excelColumn[34]}4`);
      for (var i = 0; i <= 24; i++) {
        worksheet.mergeCells(`${excelColumn[i]}4:${excelColumn[i]}5`);
      }
      for (var i = 35; i <= 44; i++) {
        worksheet.mergeCells(`${excelColumn[i]}4:${excelColumn[i]}5`);
      }
      headerRow2.getCell(26).value = 'Company';
      headerRow2.getCell(27).value = 'Employee';
      headerRow2.getCell(28).value = 'Company';
      headerRow2.getCell(29).value = 'Employee';
      headerRow2.getCell(30).value = 'Company';
      headerRow2.getCell(31).value = 'Employee';
      headerRow2.getCell(32).value = 'Employee';
      headerRow2.getCell(33).value = 'Company';
      headerRow2.getCell(34).value = 'Employee';
      headerRow2.getCell(35).value = 'TOTAL';


      this.headerBaoCao3[0].forEach((item, index) => {
        headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

        if (index == 6 || index == 7 || index == 8 || index == 13 || index == 14) {
          headerRow1.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'F2DCDB' }
          };
        } else if (index == 1 || index == 15 || index == 18 || index == 24 || index == 36 || index == 37 || index == 42) {
          headerRow1.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'E4DFEC' }
          };
        } else if (index == 9 || index == 10) {
          headerRow1.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'F2F2F2' }
          };
        } else if (index == 11) {
          headerRow1.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'DAEEF3' }
          };
        } else if (index == 12) {
          headerRow1.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'FDE9D9' }
          };
        } else {
          headerRow1.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
        }

        if (index >= 25 && index <= 34) {
          headerRow2.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
          headerRow2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          headerRow2.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

        }
      });

      /* data table 1*/
      let dataRow1 = worksheet.addRow([null, null, null, null, null, 'Day', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'AG', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'BY', 'BZ', 'CB,CE', 'CC,CF', 'CI', 'CJ', null, 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'DE', 'DF', 'VND', 'VND', 'VND', 'VND']);
      dataRow1.height = 40;
      dataRow1.font = { name: 'Times New Roman', size: 15, bold: true };
      dataRow1.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      for (var i = 1; i <= 45; i++) {
        dataRow1.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }

      let data1 = this.dataBaoCao3bang1.map((item, index) => {
        let row = [];

        row.push(index + 1);
        row.push(item.b);
        row.push(item.c);
        row.push(item.d);
        row.push(item.e);
        row.push(item.f);
        row.push(item.g);
        row.push(item.h);
        row.push(item.i);
        row.push(item.j);
        row.push(item.k);
        row.push(item.l);
        row.push(item.m);
        row.push(item.n);
        row.push(item.o);
        row.push(item.p);
        row.push(item.q);
        row.push(item.r);
        row.push(item.s);
        row.push(item.t);
        row.push(item.u);
        row.push(item.v);
        row.push(item.w);
        row.push(item.x);
        row.push(item.y);
        row.push(item.z);
        row.push(item.aa);
        row.push(item.ab);
        row.push(item.ac);
        row.push(item.ad);
        row.push(item.ae);
        row.push(item.af);
        row.push(item.ag);
        row.push(item.ah);
        row.push(item.ai);
        row.push(item.aj);
        row.push(item.ak);
        row.push(item.al);
        row.push(item.am);
        row.push(item.an);
        row.push(item.ao);
        row.push(item.ap);
        row.push(item.aq);
        row.push(item.ar);
        row.push(item.as);


        return row;
      });


      data1.forEach((el, index) => {
        let row = worksheet.addRow(el);
        row.height = 30;
        row.font = { name: 'Times New Roman', size: 15 };

        for (let i = 0; i < el.length; i++) {
          if (index == 3) {
            row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
            row.getCell(i + 1).fill = {
              type: 'pattern',
              pattern: 'solid',
              fgColor: { argb: 'FDE9D9' }
            };
          } else if (index != 4) {
            row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "dotted" }, bottom: { style: "dotted" }, right: { style: "thin" } };
          }


          if (i == 0) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          } else if (i >= 1 && i <= 2) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
            row.getCell(i + 1).font = { name: 'Times New Roman', size: 20, bold: true };
          } else if (i == 3) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).font = { name: 'Times New Roman', size: 20, bold: true };
          } else {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).numFmt = '#,##0_);(#,##0)';
          }
        }

        if (index == 2) {
          row = worksheet.addRow([]);
          row.height = 30;
          for (let i = 0; i < el.length; i++) {
            row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "dotted" }, bottom: { style: "dotted" }, right: { style: "thin" } };
          }
        }

        if (index == 3) {
          row.getCell(1).value = '';
          row.height = 40;
          row.font = { name: 'Times New Roman', size: 16, bold: true, color: { argb: 'C00000' } };
        }
        if (index == 4) {
          row.height = 40;
          row.font = { name: 'Times New Roman', size: 18, bold: true };
          row.getCell(1).value = '';
          row.getCell(3).font = { name: 'Times New Roman', size: 18, bold: true, color: { argb: 'C00000' } };
        }
      });

      space = worksheet.addRow(['']);
      space.height = 30;


      /* table 2+3 */
      /* header table 2+3 */
      let headerRow23 = worksheet.addRow([]);

      headerRow23.height = 60;
      headerRow23.font = { name: 'Times New Roman', size: 13, bold: true };
      headerRow23.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      for (var i = 2; i <= 3; i++) {
        headerRow23.getCell(i).value = this.headerBaoCao3[1][i - 2];
        headerRow23.getCell(i).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'EBF1DE' }
        };
        headerRow23.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }
      for (var i = 19; i <= 23; i++) {
        headerRow23.getCell(i).value = this.headerBaoCao3[2][i - 19];
        headerRow23.getCell(i).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'EBF1DE' }
        };
        headerRow23.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }

      /* data table 2+3*/
      for (var i = 0; i < 5; i++) {
        let dataRow = worksheet.addRow(['']);
        dataRow.height = 40;
        dataRow.font = { name: 'Calibri', size: 13, bold: true };
        dataRow.alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

        // data table 2
        if (i != 0) {
          dataRow.getCell(2).value = this.dataBaoCao3bang2[i - 1].dept;
          dataRow.getCell(3).value = this.dataBaoCao3bang2[i - 1].noOfHC;
        }
        dataRow.getCell(2).border = { left: { style: "thin" }, top: { style: "dotted" }, bottom: { style: "dotted" }, right: { style: "thin" } };
        dataRow.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
        dataRow.getCell(3).border = { left: { style: "thin" }, top: { style: "dotted" }, bottom: { style: "dotted" }, right: { style: "thin" } };
        if (i == 4) {
          dataRow.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          dataRow.getCell(2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          dataRow.getCell(2).font = { name: 'Times New Roman', size: 13, bold: true };
          dataRow.getCell(2).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
          dataRow.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          dataRow.getCell(3).font = { name: 'Times New Roman', size: 13, bold: true };
          dataRow.getCell(3).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
        }

        // data table 3
        dataRow.getCell(19).value = this.dataBaoCao3bang3[i].desc;
        dataRow.getCell(20).value = this.dataBaoCao3bang3[i].beforePIT;
        dataRow.getCell(21).value = this.dataBaoCao3bang3[i].overpaidPIT;
        dataRow.getCell(22).value = this.dataBaoCao3bang3[i].afterPIT;
        dataRow.getCell(23).value = this.dataBaoCao3bang3[i].diff;

        dataRow.getCell(19).border = { left: { style: "thin" }, top: { style: "dotted" }, bottom: { style: "dotted" }, right: { style: "thin" } };
        dataRow.getCell(19).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
        dataRow.getCell(20).border = { left: { style: "thin" }, top: { style: "dotted" }, bottom: { style: "dotted" }, right: { style: "thin" } };
        dataRow.getCell(21).border = { left: { style: "thin" }, top: { style: "dotted" }, bottom: { style: "dotted" }, right: { style: "thin" } };
        dataRow.getCell(22).border = { left: { style: "thin" }, top: { style: "dotted" }, bottom: { style: "dotted" }, right: { style: "thin" } };
        dataRow.getCell(23).border = { left: { style: "thin" }, top: { style: "dotted" }, bottom: { style: "dotted" }, right: { style: "thin" } };
        if (i == 4) {
          dataRow.getCell(19).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          dataRow.getCell(19).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
          dataRow.getCell(19).font = { name: 'Times New Roman', size: 13, bold: true };
          dataRow.getCell(19).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
          dataRow.getCell(20).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          dataRow.getCell(20).font = { name: 'Times New Roman', size: 13, bold: true };
          dataRow.getCell(20).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
          dataRow.getCell(21).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          dataRow.getCell(21).font = { name: 'Times New Roman', size: 13, bold: true };
          dataRow.getCell(21).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
          dataRow.getCell(22).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          dataRow.getCell(22).font = { name: 'Times New Roman', size: 13, bold: true };
          dataRow.getCell(22).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
          dataRow.getCell(23).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          dataRow.getCell(23).font = { name: 'Times New Roman', size: 13, bold: true };
          dataRow.getCell(23).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };

          dataRow = worksheet.addRow([]);
          dataRow.height = 40;
          dataRow.alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          dataRow.font = { name: 'Times New Roman', size: 13, bold: true, color: { argb: 'C00000' } }
          dataRow.getCell(19).value = this.dataBaoCao3bang3[5].desc;
          dataRow.getCell(20).value = this.dataBaoCao3bang3[5].beforePIT;
        }

      }

      space = worksheet.addRow(['']);
      space.height = 100;
      worksheet.addRow(['']);

      /* table 4 */
      /* title table 4 */
      let titleRow4 = worksheet.addRow(['', 'SUMMARY BY SUB-COST CENTER']);
      titleRow4.font = { name: 'Times New Roman', family: 4, size: 30, bold: true };
      titleRow4.height = 70;
      worksheet.mergeCells(`B${titleRow4.number}:K${titleRow4.number}`);
      titleRow4.alignment = { vertical: 'middle', horizontal: 'left' };


      /* header table 4 */
      let headerRow41 = worksheet.addRow(this.headerBaoCao3[3]);
      let headerRow42 = worksheet.addRow([]);

      headerRow41.height = 50;
      headerRow41.font = { name: 'Times New Roman', size: 15, bold: true };
      headerRow42.height = 140;
      headerRow42.font = { name: 'Times New Roman', size: 15, bold: true };

      for (var i = 25; i <= 29; i += 2) {
        worksheet.mergeCells(`${excelColumn[i]}24:${excelColumn[i + 1]}24`);
      }
      worksheet.mergeCells(`${excelColumn[32]}24:${excelColumn[34]}24`);
      for (var i = 0; i <= 24; i++) {
        worksheet.mergeCells(`${excelColumn[i]}24:${excelColumn[i]}25`);
      }
      for (var i = 35; i <= 42; i++) {
        worksheet.mergeCells(`${excelColumn[i]}24:${excelColumn[i]}25`);
      }
      headerRow42.getCell(26).value = 'Company';
      headerRow42.getCell(27).value = 'Employee';
      headerRow42.getCell(28).value = 'Company';
      headerRow42.getCell(29).value = 'Employee';
      headerRow42.getCell(30).value = 'Company';
      headerRow42.getCell(31).value = 'Employee';
      headerRow42.getCell(32).value = 'Employee';
      headerRow42.getCell(33).value = 'Company';
      headerRow42.getCell(34).value = 'Employee';
      headerRow42.getCell(35).value = 'TOTAL';


      this.headerBaoCao3[3].forEach((item, index) => {
        headerRow41.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow41.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

        if (index == 6 || index == 7 || index == 8 || index == 13 || index == 14) {
          headerRow41.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'F2DCDB' }
          };
        } else if (index == 1 || (index >= 17 && index <= 20) || (index >= 23 && index <= 30) || (index >= 35 && index <= 38) || index == 41 || index == 42) {
          headerRow41.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'E4DFEC' }
          };
        } else if (index == 9 || index == 10) {
          headerRow41.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'F2F2F2' }
          };
        } else if (index == 11) {
          headerRow41.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'DAEEF3' }
          };
        } else if (index == 12) {
          headerRow41.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'FDE9D9' }
          };
        } else {
          headerRow41.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
        }

        if (index >= 25 && index <= 34) {
          if (index >= 25 && index <= 30) {
            headerRow42.getCell(index + 1).fill = {
              type: 'pattern',
              pattern: 'solid',
              fgColor: { argb: 'E4DFEC' }
            };
          } else {
            headerRow42.getCell(index + 1).fill = {
              type: 'pattern',
              pattern: 'solid',
              fgColor: { argb: 'EBF1DE' }
            };
          }

          headerRow42.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          headerRow42.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        }
      });

      /* data table 4*/
      let dataRow4 = worksheet.addRow([null, null, null, null, null, 'Day', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', null, 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', null, null, 'VND', 'VND']);
      dataRow4.height = 40;
      dataRow4.font = { name: 'Times New Roman', size: 15, bold: true };
      dataRow4.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      for (var i = 1; i <= 43; i++) {
        dataRow4.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }



      let data4 = this.dataBaoCao3bang4.map((item, index) => {
        let row = [];

        row.push(index + 1);
        row.push(item.b);
        row.push(item.c);
        row.push(item.d);
        row.push(item.e);
        row.push(item.f);
        row.push(item.g);
        row.push(item.h);
        row.push(item.i);
        row.push(item.j);
        row.push(item.k);
        row.push(item.l);
        row.push(item.m);
        row.push(item.n);
        row.push(item.o);
        row.push(item.p);
        row.push(item.q);
        row.push(item.r);
        row.push(item.s);
        row.push(item.t);
        row.push(item.u);
        row.push(item.v);
        row.push(item.w);
        row.push(item.x);
        row.push(item.y);
        row.push(item.z);
        row.push(item.aa);
        row.push(item.ab);
        row.push(item.ac);
        row.push(item.ad);
        row.push(item.ae);
        row.push(item.af);
        row.push(item.ag);
        row.push(item.ah);
        row.push(item.ai);
        row.push(item.aj);
        row.push(item.ak);
        row.push(item.al);
        row.push(item.am);
        row.push(item.an);
        row.push(item.ao);
        row.push(item.ap);
        row.push(item.aq);


        return row;
      });


      data4.forEach((el, index) => {
        let row = worksheet.addRow(el);
        row.height = 30;
        row.font = { name: 'Times New Roman', size: 15 };

        for (let i = 0; i < el.length; i++) {
          if (index == 9) {
            row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
            row.getCell(i + 1).fill = {
              type: 'pattern',
              pattern: 'solid',
              fgColor: { argb: 'FDE9D9' }
            };
          } else if (index != 10) {
            row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "dotted" }, bottom: { style: "dotted" }, right: { style: "thin" } };
          }


          if (i == 0) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          } else if (i >= 1 && i <= 2) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
            row.getCell(i + 1).font = { name: 'Times New Roman', size: 20, bold: true };
          } else if (i == 3) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).font = { name: 'Times New Roman', size: 20, bold: true };
          } else {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).numFmt = '#,##0_);(#,##0)';
          }
        }

        if (index == 8) {
          row = worksheet.addRow([]);
          row.height = 30;
          for (let i = 0; i < el.length; i++) {
            row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "dotted" }, bottom: { style: "dotted" }, right: { style: "thin" } };
          }
        }

        if (index == 9) {
          row.getCell(1).value = '';
          row.height = 40;
          row.font = { name: 'Times New Roman', size: 16, bold: true, color: { argb: 'C00000' } };
        }

        if (index == 10) {
          row.getCell(1).value = '';
          row.height = 60;
          row.font = { name: 'Times New Roman', size: 16, bold: true };
        }
      });
      worksheet.mergeCells('B27:B31');
      worksheet.mergeCells('B32:B33');


      /* table 5 */
      /* title table 5 */
      let titleRow5 = worksheet.addRow(['', 'SUMMARY BY GRADE - 3D']);
      titleRow5.font = { name: 'Times New Roman', family: 4, size: 30, bold: true };
      titleRow5.height = 70;
      worksheet.mergeCells(`B${titleRow5.number}:K${titleRow5.number}`);
      titleRow5.alignment = { vertical: 'middle', horizontal: 'left' };


      /* header table 5 */
      let headerRow51 = worksheet.addRow(this.headerBaoCao3[3]);
      let headerRow52 = worksheet.addRow([]);

      headerRow51.height = 50;
      headerRow51.font = { name: 'Times New Roman', size: 15, bold: true };
      headerRow52.height = 140;
      headerRow52.font = { name: 'Times New Roman', size: 15, bold: true };

      for (var i = 25; i <= 29; i += 2) {
        worksheet.mergeCells(`${excelColumn[i]}40:${excelColumn[i + 1]}40`);
      }
      worksheet.mergeCells(`${excelColumn[32]}40:${excelColumn[34]}40`);
      for (var i = 0; i <= 24; i++) {
        worksheet.mergeCells(`${excelColumn[i]}40:${excelColumn[i]}41`);
      }
      for (var i = 35; i <= 42; i++) {
        worksheet.mergeCells(`${excelColumn[i]}40:${excelColumn[i]}41`);
      }
      headerRow52.getCell(26).value = 'Company';
      headerRow52.getCell(27).value = 'Employee';
      headerRow52.getCell(28).value = 'Company';
      headerRow52.getCell(29).value = 'Employee';
      headerRow52.getCell(30).value = 'Company';
      headerRow52.getCell(31).value = 'Employee';
      headerRow52.getCell(32).value = 'Employee';
      headerRow52.getCell(33).value = 'Company';
      headerRow52.getCell(34).value = 'Employee';
      headerRow52.getCell(35).value = 'TOTAL';


      this.headerBaoCao3[3].forEach((item, index) => {
        headerRow51.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow51.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

        if (index == 6 || index == 7 || index == 8 || index == 13 || index == 14) {
          headerRow51.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'F2DCDB' }
          };
        } else if (index == 1 || (index >= 17 && index <= 20) || (index >= 23 && index <= 30) || (index >= 35 && index <= 38) || index == 41 || index == 42) {
          headerRow51.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'E4DFEC' }
          };
        } else if (index == 9 || index == 10) {
          headerRow51.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'F2F2F2' }
          };
        } else if (index == 11) {
          headerRow51.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'DAEEF3' }
          };
        } else if (index == 12) {
          headerRow51.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'FDE9D9' }
          };
        } else {
          headerRow51.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
        }

        if (index >= 25 && index <= 34) {
          if (index >= 25 && index <= 30) {
            headerRow52.getCell(index + 1).fill = {
              type: 'pattern',
              pattern: 'solid',
              fgColor: { argb: 'E4DFEC' }
            };
          } else {
            headerRow52.getCell(index + 1).fill = {
              type: 'pattern',
              pattern: 'solid',
              fgColor: { argb: 'EBF1DE' }
            };
          }

          headerRow52.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          headerRow52.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        }
      });

      /* data table 5*/
      let dataRow5 = worksheet.addRow([null, null, null, null, null, 'Day', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', null, 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', null, null, 'VND', 'VND']);
      dataRow5.height = 40;
      dataRow5.font = { name: 'Times New Roman', size: 15, bold: true };
      dataRow5.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      for (var i = 1; i <= 43; i++) {
        dataRow5.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }



      let data5 = this.dataBaoCao3bang5.map((item, index) => {
        let row = [];

        row.push(index + 1);
        row.push(item.b);
        row.push(item.c);
        row.push(item.d);
        row.push(item.e);
        row.push(item.f);
        row.push(item.g);
        row.push(item.h);
        row.push(item.i);
        row.push(item.j);
        row.push(item.k);
        row.push(item.l);
        row.push(item.m);
        row.push(item.n);
        row.push(item.o);
        row.push(item.p);
        row.push(item.q);
        row.push(item.r);
        row.push(item.s);
        row.push(item.t);
        row.push(item.u);
        row.push(item.v);
        row.push(item.w);
        row.push(item.x);
        row.push(item.y);
        row.push(item.z);
        row.push(item.aa);
        row.push(item.ab);
        row.push(item.ac);
        row.push(item.ad);
        row.push(item.ae);
        row.push(item.af);
        row.push(item.ag);
        row.push(item.ah);
        row.push(item.ai);
        row.push(item.aj);
        row.push(item.ak);
        row.push(item.al);
        row.push(item.am);
        row.push(item.an);
        row.push(item.ao);
        row.push(item.ap);
        row.push(item.aq);


        return row;
      });


      data5.forEach((el, index) => {
        let row = worksheet.addRow(el);
        row.height = 30;
        row.font = { name: 'Times New Roman', size: 15 };

        for (let i = 0; i < el.length; i++) {
          if (index == 3) {
            row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
            row.getCell(i + 1).fill = {
              type: 'pattern',
              pattern: 'solid',
              fgColor: { argb: 'FDE9D9' }
            };
          } else if (index != 4) {
            row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "dotted" }, bottom: { style: "dotted" }, right: { style: "thin" } };
          }


          if (i == 0) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          } else if (i >= 1 && i <= 2) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
            row.getCell(i + 1).font = { name: 'Times New Roman', size: 20, bold: true };
          } else if (i == 3) {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).font = { name: 'Times New Roman', size: 20, bold: true };
          } else {
            row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            row.getCell(i + 1).numFmt = '#,##0_);(#,##0)';
          }
        }

        if (index == 2) {
          row = worksheet.addRow([]);
          row.height = 30;
          for (let i = 0; i < el.length; i++) {
            row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "dotted" }, bottom: { style: "dotted" }, right: { style: "thin" } };
          }
        }

        if (index == 3) {
          row.getCell(1).value = '';
          row.height = 40;
          row.font = { name: 'Times New Roman', size: 16, bold: true, color: { argb: 'C00000' } };
        }

        if (index == 4) {
          row.getCell(1).value = '';
          row.height = 60;
          row.font = { name: 'Times New Roman', size: 20, bold: true };
        }
      });
      worksheet.mergeCells('B43:B45');


      /* end sheet */
      let sign = worksheet.addRow(['', 'PREPARED BY:']);
      sign.font = { name: 'Times New Roman', size: 20, bold: true };
      sign.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: false };
      worksheet.addRow(['']);
      worksheet.addRow(['']);
      worksheet.addRow(['']);
      worksheet.addRow(['']);
      worksheet.addRow(['']);
      worksheet.addRow(['']);
      sign = worksheet.addRow(['', `${this.tenNhanVien}`]);
      sign.font = { name: 'Times New Roman', size: 20, bold: true };
      sign.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: false };


      /* set width */
      for (var i = 1; i <= this.headerBaoCao3[0].length; i++) {
        if (i == 1) {
          worksheet.getColumn(i).width = 10;
        } else {
          worksheet.getColumn(i).width = 25;
        }
      }

      /* save file */
      workbook.xlsx.writeBuffer().then((data) => {
        let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        saveAs.saveAs(blob, title);
      });
    } else {
      this.showMessage("error", 'Không có dữ liệu');
    }
  };

}
