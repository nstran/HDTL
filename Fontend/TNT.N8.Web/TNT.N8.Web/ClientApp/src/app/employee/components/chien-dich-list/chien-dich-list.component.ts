import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { EmployeeService } from '../../services/employee.service';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { DatePipe } from '@angular/common';
import { GetPermission } from '../../../shared/permission/get-permission';

class RecruitmentCampaignEntityModel {
  recruitmentCampaignId: string;
  recruitmentCampaignName: string;
  startDate: Date;
  endDateDate: Date;
  statusName: string;
  personInChargeId: string;
  personInChargeName: string;
  personInChargeCode: string;
  personInChargeCodeName: string;
  recruitmentCampaignDes: string;
  recruitmentQuantity: number;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
}

class ColumnExcel {
  code: string;
  name: string;
  width: number;
}

@Component({
  selector: 'app-chien-dich-list',
  templateUrl: './chien-dich-list.component.html',
  styleUrls: ['./chien-dich-list.component.css'],
  providers: [DatePipe]
})
export class ChienDichListComponent implements OnInit {

  @ViewChild('myTable') myTable: Table;

  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  today: Date = new Date();
  statusCode: string = null;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  currentEmployeeCodeName = localStorage.getItem('EmployeeCodeName');
  auth = JSON.parse(localStorage.getItem('auth'));
  chienDichId: string = '';
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  recruitmentCampaignModel: RecruitmentCampaignEntityModel;
  filterGlobal: string = '';
  innerWidth: number = 0; //number window size first

  leftColNumber: number = 12;
  rightColNumber: number = 0;

  //Search
  recruitmentCampaignName: string = '';
  startDateFrom: Date = null;
  startDateTo: Date = null;
  endDateFrom: Date = null;
  endDateTo: Date = null;
  listPersonInchange: Array<any> = [];
  recruitmentQuantityFrom: string = '';
  recruitmentQuantityTo: string = '';


  actionDelete: boolean = true;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  // danh sach
  listEmployee: Array<any> = [];
  listRecruitmentCampaign: Array<any> = [];
  selection: Array<any> = [];

  colsList: any[];
  selectedColumns: any[];

  constructor(
    private router: Router,
    private datePipe: DatePipe,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    private confirmationService: ConfirmationService,
    private getPermission: GetPermission,
  ) {
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    let resource = "rec/employee/danh-sach-chien-dich/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
      this.showMessage('warn', 'Bạn không có quyền truy cập');
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;

      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }
    }

    //Quyền xóa chiến dịch
    let resource1 = "rec/employee/chi-tiet-chien-dich/";
    let permission1: any = await this.getPermission.getPermission(resource1);
    if (permission1.status == false) {
      this.actionDelete = false;
    }
    else {
      let listCurrentActionResource = permission1.listCurrentActionResource;

      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
    }

    this.setTable();
    this.getMasterData();
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.employeeService.getMasterSearchRecruitmentCampaignAsync();
    this.loading = false;
    this.listEmployee = result.listEmployee;
    this.searchRecruitmentCampaign();
  }

  setTable() {
    this.colsList = [
      { field: 'index', header: '#', width: '20px', textAlign: 'center' },
      { field: 'recruitmentCampaignName', header: 'Tên chiến dịch', width: '250px', textAlign: 'left' },
      { field: 'startDate', header: 'Ngày bắt đầu', width: '100px', textAlign: 'right' },
      { field: 'endDateDate', header: 'Ngày kết thúc', width: '100px', textAlign: 'right' },
      { field: 'personInChargeName', header: 'Người phụ trách', width: '150px', textAlign: 'left' },
      { field: 'statusName', header: 'Trạng thái', width: '80px', textAlign: 'center' },
      { field: 'recruitmentQuantity', header: 'Kế hoạch tuyển dụng', width: '80px', textAlign: 'right' },
      { field: 'action', header: 'Thao tác', width: '100px', textAlign: 'center' },
    ];

    this.selectedColumns = this.colsList;
  }

  refreshFilter() {
    if (this.filterGlobal != '') {
      this.myTable.reset();
    }
    this.filterGlobal = '';
    this.recruitmentCampaignName = '';
    this.startDateFrom = null;
    this.startDateTo = null;
    this.endDateFrom = null;
    this.endDateTo = null;
    this.listPersonInchange = [];
    this.recruitmentQuantityFrom = '';
    this.recruitmentQuantityTo = '';

    this.searchRecruitmentCampaign();
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

  exportExcel() {
    if (this.selection.length > 0) {
      if (this.selectedColumns.length > 0) {
        let title = `Danh sách chiến dịch tuyển dụng`;

        let workBook = new Workbook();
        let worksheet = workBook.addWorksheet(title);

        let dataHeaderMain = "Danh sách chiến dịch tuyển dụng".toUpperCase();
        let headerMain = worksheet.addRow([dataHeaderMain]);
        headerMain.font = { size: 18, bold: true };
        worksheet.mergeCells(`A${1}:G${1}`);
        headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        worksheet.addRow([]);

        /* Header row */
        let buildColumnExcel = this.buildColumnExcel();
        let dataHeaderRow = buildColumnExcel.map(x => x.name);
        let headerRow = worksheet.addRow(dataHeaderRow);
        headerRow.font = { name: 'Times New Roman', size: 12, bold: true };
        dataHeaderRow.forEach((item, index) => {
          headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          headerRow.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: '8DB4E2' }
          };
        });
        headerRow.height = 22.50;

        /* Data table */
        let data: Array<any> = [];
        this.selection.forEach((item, index) => {
          let row: Array<any> = [];
          buildColumnExcel.forEach((_item, _index) => {

            if (_item.code == 'index') {
              row[_index] = index + 1;
            }
            else if (_item.code == 'recruitmentCampaignName') {
              row[_index] = item.recruitmentCampaignName;
            }
            else if (_item.code == 'startDate') {
              row[_index] = this.datePipe.transform(item.startDate, 'dd/MM/yyyy');
            }
            else if (_item.code == 'endDateDate') {
              row[_index] = this.datePipe.transform(item.endDateDate, 'dd/MM/yyyy');
            }
            else if (_item.code == 'personInChargeName') {
              row[_index] = item.personInChargeName;
            }
            else if (_item.code == 'statusName') {
              row[_index] = item.statusName;
            }
            else if (_item.code == 'recruitmentQuantity') {
              row[_index] = item.recruitmentQuantity;
            }
          });

          data.push(row);
        });

        data.forEach((el, index, array) => {
          let row = worksheet.addRow(el);
          row.font = { name: 'Times New Roman', size: 11 };

          buildColumnExcel.forEach((_item, _index) => {
            row.getCell(_index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
            if (_item.code == 'recruitmentCampaignName' || _item.code == 'personInChargeName') {
              row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'left' };
            }
            else if (_item.code == 'startDate' || _item.code == 'endDateDate') {
              row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'right' };
            }
            else {
              row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'center' };
            }
          });
        });

        /* fix with for column */
        buildColumnExcel.forEach((item, index) => {
          worksheet.getColumn(index + 1).width = item.width;
        });

        this.exportToExel(workBook, title);
      }
      else {
        this.showMessage('warn', 'Bạn phải chọn ít nhất 1 cột');
      }
    }
    else {
      this.showMessage('warn', 'Bạn chưa chọn chiến dịch');
    }
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  buildColumnExcel(): Array<ColumnExcel> {
    let listColumn: Array<ColumnExcel> = [];
    let cols = this.selectedColumns.filter(x => x.field != 'action');
    cols.forEach(item => {
      let column = new ColumnExcel();
      column.code = item.field;
      column.name = item.header;

      if (item.field == 'index') {
        column.width = 7.14;
      }
      else if (item.field == 'recruitmentCampaignName') {
        column.width = 30.71;
      }
      else if (item.field == 'startDate') {
        column.width = 30.71;
      }
      else if (item.field == 'endDateDate') {
        column.width = 30.71;
      }
      else if (item.field == 'personInChargeName') {
        column.width = 35.71;
      }
      else if (item.field == 'statusName') {
        column.width = 20.71;
      }
      else if (item.field == 'recruitmentQuantity') {
        column.width = 25.71;
      }

      listColumn.push(column);
    });

    return listColumn;
  }


  goToCreate() {
    this.router.navigate(['/employee/tao-chien-dich']);
  }

  onViewDetail(rowData) {
    this.router.navigate(['/employee/chi-tiet-chien-dich', { recruitmentCampaignId: rowData.recruitmentCampaignId }]);
  }

  deleteRow(rowData) {
    this.confirmationService.confirm({
      message: 'Bạn có muốn xác nhận xóa?',
      accept: () => {
        this.loading = true;
        this.employeeService.deleteRecruitmentCampaign(rowData.recruitmentCampaignId).subscribe(response => {
          let result = <any>response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.listRecruitmentCampaign = this.listRecruitmentCampaign.filter(x => x.recruitmentCampaignId != rowData.recruitmentCampaignId);
            this.showMessage('success', 'Xóa chiến dịch thành công.');
          } else {
            this.showMessage('error', result.messageCode);
          }
        });
      }
    });
  }

  searchRecruitmentCampaign() {

    let startDateFromUTC = null;
    if (this.startDateFrom) {
      startDateFromUTC = this.startDateFrom;
      startDateFromUTC.setHours(0, 0, 0, 0);
      startDateFromUTC = convertToUTCTime(startDateFromUTC);
    }

    let startDateToUTC = null;
    if (this.startDateTo) {
      startDateToUTC = this.startDateTo;
      startDateToUTC.setHours(0, 0, 0, 0);
      startDateToUTC = convertToUTCTime(startDateToUTC);
    }

    let endDateFromUTC = null;
    if (this.endDateFrom) {
      endDateFromUTC = this.endDateFrom;
      endDateFromUTC.setHours(0, 0, 0, 0);
      endDateFromUTC = convertToUTCTime(endDateFromUTC);
    }

    let endDateToUTC = null;
    if (this.endDateTo) {
      endDateToUTC = this.endDateTo;
      endDateToUTC.setHours(0, 0, 0, 0);
      endDateToUTC = convertToUTCTime(endDateToUTC);
    }

    let listPersonInchangeId = [];
    if (this.listPersonInchange.length > 0) {
      listPersonInchangeId = this.listPersonInchange.map(x => x.employeeId);
    }

    let recruitmentQuantityFrom = ParseStringToFloat(this.recruitmentQuantityFrom);

    let recruitmentQuantityTo = ParseStringToFloat(this.recruitmentQuantityTo);

    this.loading = true;
    this.employeeService.searchRecruitmentCampaign(this.recruitmentCampaignName, startDateFromUTC, startDateToUTC, endDateFromUTC, endDateToUTC, listPersonInchangeId, recruitmentQuantityFrom, recruitmentQuantityTo).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listRecruitmentCampaign = result.listRecruitmentCampaign;
        if (this.listRecruitmentCampaign.length === 0) {
          this.showMessage('warn', 'Không tìm thấy nhân viên nào!');
        }
      } else {
        this.showMessage('error', result.messageCode);
      }
    });
  };

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function ParseStringToFloat(str: string) {
  if (str === "") return null;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
