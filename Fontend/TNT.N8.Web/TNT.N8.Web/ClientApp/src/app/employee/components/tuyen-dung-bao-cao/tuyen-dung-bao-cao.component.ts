import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { EmployeeService } from '../../services/employee.service';
import { Row, Workbook, Worksheet } from 'exceljs';
import { saveAs } from "file-saver";

@Component({
  selector: 'app-tuyen-dung-bao-cao',
  templateUrl: './tuyen-dung-bao-cao.component.html',
  styleUrls: ['./tuyen-dung-bao-cao.component.css']
})
export class TuyenDungBaoCaoComponent implements OnInit {

  auth = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  projectId: string = null;

  cols1: Array<any> = [];

  listRecruitmentCampaign: Array<any> = [];
  listRecruitmentCampaignDefault: Array<any> = [];
  selectedRecruitmentCampaign: Array<any> = [];

  listVacancies: Array<any> = [];
  listVacanciesDefault: Array<any> = [];
  selectedVacancies: Array<any> = [];

  listEmployee: Array<any> = [];
  listEmployeeDefault: Array<any> = [];
  selectedEmployee: Array<any> = [];

  listDefineData: Array<any> = [];
  listData: Array<any> = [];

  listHeaderRow1: Array<any> = [];
  listHeaderRow2: Array<any> = [];
  listDataFooter: Array<any> = [];


  rowGroupMetadata;

  listThang: Array<any> = [
    { name: 'Tháng 1', value: 1 },
    { name: 'Tháng 2', value: 2 },
    { name: 'Tháng 3', value: 3 },
    { name: 'Tháng 4', value: 4 },
    { name: 'Tháng 5', value: 5 },
    { name: 'Tháng 6', value: 6 },
    { name: 'Tháng 7', value: 7 },
    { name: 'Tháng 8', value: 8 },
    { name: 'Tháng 9', value: 9 },
    { name: 'Tháng 10', value: 10 },
    { name: 'Tháng 11', value: 11 },
    { name: 'Tháng 12', value: 12 },
  ];
  selectedThang: any = this.listThang.find(x => x.value == (new Date().getMonth() + 1));

  listNam: Array<any> = this.setListNam();
  selectedNam: any = this.listNam.find(x => x.value == new Date().getFullYear());

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private employeeService: EmployeeService,
  ) { }

  ngOnInit(): void {
    this.getMasterData();
    // this.route.params.subscribe(params => {
    //   this.projectId = params['projectId'];
    // });
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.employeeService.getMasterDataBaoCaoTuyenDung();
    this.loading = false;
    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }

    this.listRecruitmentCampaignDefault = result.listRecruitmentCampaign;
    this.listVacanciesDefault = result.listVacancies;
    this.listEmployeeDefault = result.listEmployee;

    this.listRecruitmentCampaign = result.listRecruitmentCampaign;
    this.listVacancies = result.listVacancies;
    this.listEmployee = result.listEmployee;

    this.getBaoCaoTuyenDung();
  }

  async getBaoCaoTuyenDung() {
    let nam = this.selectedNam.value;
    let thang = this.selectedThang.value;

    let listRecruitmentCampaignId = this.selectedRecruitmentCampaign.map(x => x.recruitmentCampaignId);
    let listVacansiesId = this.selectedVacancies.map(x => x.vacanciesId);
    let listEmployeeId = this.selectedEmployee.map(x => x.employeeId);
    this.loading = true;

    let result: any = await this.employeeService.getBaoCaoBaoCaoTuyenDung(thang, nam, listRecruitmentCampaignId, listVacansiesId, listEmployeeId);
    this.loading = false;
    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }

    this.listHeaderRow1 = result.listHeaderRow1;
    this.listHeaderRow2 = result.listHeaderRow2;
    this.listDefineData = result.listData;
    this.listDataFooter = result.listDataFooter;

    this.buildTable();

  }

  buildTable() {
    this.cols1 = [];
    this.listData = [];

    /* build header */
    let temp = this.listDefineData[0];

    temp ?.forEach((item, index) => {
      let header = { field: item.columnKey, header: item.columnValue, width: item.width, textAlign: item.textAlign };

      this.cols1 = [...this.cols1, header];
    });

    /* build data */

    this.listDefineData.forEach(item => {
      let data = {};

      item.forEach(_data => {
        if (_data.columnValue != null && _data.valueType == 1) {
          data[_data.columnKey] = ParseStringToFloat(_data.columnValue);
        }
        else {
          data[_data.columnKey] = _data.columnValue;
        }
      });

      this.listData = [...this.listData, data];
    });
    //test
    this.rowGroupMetadata = {};
    if (this.listData) {
      this.listData.forEach((item, index) => {
        let brand = item.recruitName;
        let brand1 = item.employeeName;
        if (index == 0) {
          this.rowGroupMetadata[brand] = { index: 0, size: 1 };
          this.rowGroupMetadata[brand][brand1] = { index: 0, size: 1 };
        } else {
          let previousRowData = this.listData[index - 1];

          let previousRowGroup = previousRowData.recruitName;
          let previousRowGroup1 = previousRowData.employeeName;

          if (brand === previousRowGroup) {
            this.rowGroupMetadata[brand].size++;
            if (brand1 === previousRowGroup1) {
              this.rowGroupMetadata[brand][brand1].size++;
            } else {
              this.rowGroupMetadata[brand][brand1] = { index: index, size: 1 };
            }
          }
          else {
            this.rowGroupMetadata[brand] = { index: index, size: 1 };
            this.rowGroupMetadata[brand][brand1] = { index: index, size: 1 };
          }
        }
      });
    }

    // /* build footer */
    // this.listDataFooter.forEach(item => {
    //   if (item.columnValue != null && item.valueType == 1) {
    //     item.columnValue = ParseStringToFloat(item.columnValue);
    //   }
    // });
  }

  resetFilter() {
    this.selectedThang = this.listThang.find(x => x.value == (new Date().getMonth() + 1));
    this.selectedNam = this.listNam.find(x => x.value == new Date().getFullYear());
    this.selectedRecruitmentCampaign = [];
    this.selectedEmployee = [];
    this.selectedVacancies = [];

    this.getBaoCaoTuyenDung();
  }

  exportExcel() {
    let titleQuote = "BÁO CÁO TUYỂN DUNG";
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet(titleQuote, { views: [{ showGridLines: false }] });
    worksheet.pageSetup.margins = {
      top: 0.25, bottom: 0.75,
      left: 0.25, right: 0.5,
      header: 0.3, footer: 0.05
    };

    worksheet.pageSetup.paperSize = 9;  //A4 : 9

    let colA = worksheet.getColumn('A');
    colA.width = 5;

    let colB = worksheet.getColumn('B');
    colB.width = 15;

    let colC = worksheet.getColumn('C');
    colC.width = 20;

    let colD = worksheet.getColumn('D');
    colD.width = 25;

    let colE = worksheet.getColumn('E');
    colE.width = 10;

    let colF = worksheet.getColumn('F');
    colF.width = 10;

    let colG = worksheet.getColumn('G');
    colG.width = 10;

    let colH = worksheet.getColumn('H');
    colH.width = 10;

    let colI = worksheet.getColumn('I');
    colI.width = 10;

    let colK = worksheet.getColumn('K');
    colK.width = 10;

    let colL = worksheet.getColumn('L');
    colL.width = 10;

    let colM = worksheet.getColumn('M');
    colM.width = 10;

    let colN = worksheet.getColumn('N');
    colN.width = 10;

    let colO = worksheet.getColumn('O');
    colO.width = 10;

    let colP = worksheet.getColumn('P');
    colP.width = 10;

    //set header1
    let row1 = worksheet.getRow(1);
    row1.getCell('A').value = 'STT';
    row1.getCell('B').value = 'Tên chiến dịch';
    row1.getCell('C').value = 'Vị trí tuyển dụng';
    row1.getCell('D').value = 'Người phụ trách vị trí';
    row1.getCell('E').value = 'SL tuyển';
    row1.getCell('F').value = 'Tổng cv';
    row1.getCell('G').value = 'NV/ thử việc';
    row1.getCell('H').value = 'Trạng thái ứng viên';
    // row1.getCell('I').value = 'STT';
    // row1.getCell('J').value = 'STT';
    // row1.getCell('K').value = 'STT';
    // row1.getCell('L').value = 'STT';
    // row1.getCell('M').value = 'STT';
    row1.getCell('N').value = 'Ưu tiên';
    row1.getCell('O').value = '% Đạt';
    row1.getCell('P').value = '% Hiệu suất';
    row1.font = { name: 'Arial', size: 9, bold: true };
    row1.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    this.setBorderAll(worksheet, row1);
    row1.height = 30;

    //Set header2
    let row2 = worksheet.getRow(2);
    row2.getCell('H').value = 'Mới';
    row2.getCell('I').value = 'Hẹn PV';
    row2.getCell('J').value = 'Đạt PV';
    row2.getCell('K').value = 'Gửi offer';
    row2.getCell('L').value = 'Từ chối offer';
    row2.getCell('M').value = 'Không đạt';
    row2.font = { name: 'Arial', size: 9, bold: true };
    row2.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    this.setBorderAll(worksheet, row2);
    row2.height = 30;

    //merge cell header
    worksheet.mergeCells(`A${row1.number}:A${row2.number}`);
    worksheet.mergeCells(`B${row1.number}:B${row2.number}`);
    worksheet.mergeCells(`C${row1.number}:C${row2.number}`);
    worksheet.mergeCells(`D${row1.number}:D${row2.number}`);
    worksheet.mergeCells(`E${row1.number}:E${row2.number}`);
    worksheet.mergeCells(`F${row1.number}:F${row2.number}`);
    worksheet.mergeCells(`G${row1.number}:G${row2.number}`);
    worksheet.mergeCells(`N${row1.number}:N${row2.number}`);
    worksheet.mergeCells(`O${row1.number}:O${row2.number}`);
    worksheet.mergeCells(`P${row1.number}:P${row2.number}`);

    worksheet.mergeCells(`H${row1.number}:M${row1.number}`);

    //list data
    let currentRow = 3;
    let preRecuit = this.listData[0].recruitName;
    let preRecuitRow = 3;

    let preEmpl = this.listData[0].employeeName;
    let preEmplRow = 3;

    this.listData.forEach((item, index) => {
      let rowVT = worksheet.getRow(currentRow);
      rowVT.getCell('A').value = index + 1/*  */;
      rowVT.getCell('B').value = item.recruitName;
      rowVT.getCell('C').value = item.vacanciesName;
      rowVT.getCell('D').value = item.employeeName;
      rowVT.getCell('E').value = item.quantity;
      rowVT.getCell('F').value = item.totalCV;
      rowVT.getCell('G').value = item.totalConvert;
      rowVT.getCell('H').value = item.new;
      rowVT.getCell('I').value = item.henPV;
      rowVT.getCell('J').value = item.datPV;
      rowVT.getCell('K').value = item.guiOff;
      rowVT.getCell('L').value = item.tuChoiOff;
      rowVT.getCell('M').value = item.khongDat;
      rowVT.getCell('N').value = item.uuTien;
      rowVT.getCell('O').value = item.ptDat;
      rowVT.getCell('P').value = item.ptHieuSuat;
      //merge trùng teen chiến dịch

      if (preRecuit != item.recruitName) {
        let endRow = worksheet.getRow(currentRow - 1);
        let startRow = worksheet.getRow(preRecuitRow);
        worksheet.mergeCells(`B${startRow.number}:B${endRow.number}`);
        preRecuitRow = currentRow;
        preRecuit = item.recruitName;
        if (preEmpl != item.employeeName) {
          let endRow = worksheet.getRow(currentRow - 1);
          let startRow = worksheet.getRow(preEmplRow);
          worksheet.mergeCells(`D${startRow.number}:D${endRow.number}`);
        }
        preEmplRow = currentRow;
        preEmpl = item.employeeName;
      }


      if (preRecuit == item.recruitName) {
        if (preEmpl != item.employeeName) {
          let endRow = worksheet.getRow(currentRow - 1);
          let startRow = worksheet.getRow(preEmplRow);
          worksheet.mergeCells(`D${startRow.number}:D${endRow.number}`);
          preEmplRow = currentRow;
          preEmpl = item.employeeName;
        }
      }

      //trường hợp trùng đến hết bảng của tên chiến dịch
      if (preRecuit == item.recruitName && index == this.listData.length - 1) {
        let endRow = worksheet.getRow(currentRow);
        let startRow = worksheet.getRow(preRecuitRow);
        worksheet.mergeCells(`B${startRow.number}:B${endRow.number}`);
        if (preEmpl == item.employeeName) {
          let endRow = worksheet.getRow(currentRow);
          let startRow = worksheet.getRow(preEmplRow);
          worksheet.mergeCells(`D${startRow.number}:D${endRow.number}`);
        }
      }

      rowVT.font = { name: 'Arial', size: 9 };
      rowVT.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      rowVT.height = 20;
      // Tao border
      this.setBorderAll(worksheet, rowVT);
      currentRow++;
    });
    this.exportToExel(workbook, titleQuote);

  }

  setBorderAll(worksheet, row) {
    worksheet.getCell(`A${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`B${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`C${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`D${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`E${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`F${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`G${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`H${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`I${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`J${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`K${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`L${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`M${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`N${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`O${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    worksheet.getCell(`P${row.number}`).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
  }



  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  changeRecruitmentCampaign() {
    this.listEmployee = []
    this.listVacancies = []
    let lstRecruiId = this.selectedRecruitmentCampaign.map(x => x.recruitmentCampaignId);
    // Lọc vị trí theo chiến dịch
    this.listVacancies = this.listVacanciesDefault.filter(x => lstRecruiId.includes(x.recruitmentCampaignId));

    let lstEmployeeId = this.listVacancies.map(x => x.personInChargeId);
    this.listEmployee = this.listEmployeeDefault.filter(x => lstEmployeeId.includes(x.employeeId));
  }
  changeVacancies() {
    this.listEmployee = []

    let lstEmployeeId = this.selectedVacancies.map(x => x.personInChargeId);
    this.listEmployee = this.listEmployeeDefault.filter(x => lstEmployeeId.includes(x.employeeId))
  }
  setListNam() {
    let listNam = [];
    let currentYear = new Date().getFullYear();
    let start = currentYear - 5;
    let end = currentYear + 1;

    for (let i = start; i <= end; i++) {
      let option = { name: i.toString(), value: i };
      listNam.push(option);
    }

    return listNam;
  }

  showMessage(severity: string, detail: string) {
    this.messageService.add({ severity: severity, summary: 'Thông báo', detail: detail });
  }
}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
