import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ProjectService } from '../../services/project.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MenuItem } from "primeng/primeng";
import { MessageService } from 'primeng/api';
import { SortEvent } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { CategoryService } from '../../../shared/services/category.service';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";


interface Category {
  categoryId: string,
  categoryName: string,
  categoryCode: string
}
interface Employee {
  employeeId: string,
  employeeName: string,
  employeeCode: string
}
class ColumnExcel {
  code: string;
  name: string;
  width: number;
}
@Component({
  selector: 'app-project-list',
  templateUrl: './project-list.component.html',
  styleUrls: ['./project-list.component.css'],
  providers: [
    DatePipe
  ]
})
export class ProjectListComponent implements OnInit {
  innerWidth: number = 0; //number window size first

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    //if (this.innerWidth < )
  }

  emitParamUrl: any;
  @ViewChild('myTableDtk') myTableDtk: Table;
  @ViewChild('myTableHuy') myTableHuy: Table;

  emptyGuild = "00000000-0000-0000-0000-000000000000";
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  applicationName = this.getDefaultApplicationName();
  auth: any = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  colsListProject: any;
  first = 0;
  rows = 10;
  selectedColumns: any[];
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  minYear: number = 2015;
  currentYear: number = (new Date()).getFullYear();
  actions: MenuItem[] = [];
  listStatus: Array<Category> = [];
  listProjectType: Array<Category> = [];
  listEmployee: Array<Employee> = [];
  selectedStatus: Array<Category> = []; // trạng thái
  selectedProjectType: Array<Category> = []; // loại dự án
  selectedEmployee: Array<Employee> = []; // người quản lý
  listProject: Array<any> = [];
  listProjectDTK: Array<any> = [];
  listProjectHUY: Array<any> = [];
  projectCode: string = ''; // mã dự án
  projectName: string = ''; // tên dự án
  projectStartS: Date = null; // ngày bắt đầu dự tính từ
  projectStartE: Date = null; // ngày bắt đầu dự tính đến
  projectEndS: Date = null; // ngày kết thúc dự tính từ
  projectEndE: Date = null; // ngày kết thúc dự tính đến
  actualStartS: Date = null;  // ngày bắt đầu thực tế từ
  actualStartE: Date = null; // ngày bắt đầu thực tế đến
  actualEndS: Date = null; // ngày kết thúc thưc tế từ
  actualEndE: Date = null; // ngày kết thúc thưc tế đến
  estimateCompleteTimeS: number = 0; // t/g du kien thuc hien tu
  estimateCompleteTimeE: number = 0; // t/g du kien thuc hien den
  completeRateS: number = 0; // uoc tinh ty le hoan thanh tu
  completeRateE: number = 0; // uoc tinh ty le hoan thanh den
  maxEndDate: Date = new Date();
  today: Date = new Date();

  isGlobalFilter: string = '';

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  userPermission: any = localStorage.getItem("UserPermission").split(',');
  createPermission: string = "order/create";
  viewPermission: string = "order/view";
  /*End*/

  /*Action*/
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  actionDelete: boolean = true;
  /*End*/

  activeIndex: number = 0;

  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private projectService: ProjectService,
    private messageService: MessageService,
    private datePipe: DatePipe,
    private confirmationService: ConfirmationService,
    private categoryService: CategoryService) {
    this.innerWidth = window.innerWidth;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  async ngOnInit() {
    //Check permission
    let resource = "pro/project/list";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("download") == -1) {
        this.actionDownload = false;
      }
      this.declaireTable();

      this.emitParamUrl = this.route.params.subscribe(params => {
        this.searchProject();
      });
    }
  }

  declaireTable() {
    this.colsListProject = [
      { field: 'projectName', header: 'Tên dự án', textAlign: 'left', width: '250px', display: 'table-cell' },
      { field: 'projectTypeName', header: 'Loại dự án', textAlign: 'left', width: '120px', display: 'table-cell' },
      { field: 'employeeName', header: 'Người quản lý', textAlign: 'left', width: '180px', display: 'table-cell' },
      { field: 'projectStatusName', header: 'Trạng thái', textAlign: 'center', width: '170px', display: 'table-cell' },
      { field: 'projectStartDate', header: 'Ngày bắt đầu dự tính', textAlign: 'right', width: '180px', display: 'table-cell' },
      { field: 'projectEndDate', header: 'Ngày kết thúc dự tính', textAlign: 'right', width: '180px', display: 'table-cell' },
      { field: 'actualStartDate', header: 'Ngày bắt đầu thực tế', textAlign: 'right', width: '180px', display: 'table-cell' },
      {
        field: 'estimateCompleteTime', header: 'Thời gian dự kiến thực hiện (Giờ)', textAlign: 'right', width: '90px', display: 'table-cell'
      },
      { field: 'completeRate', header: 'Ước tính tỉ lệ hoàn thành', textAlign: 'right', width: '100px', display: 'table-cell' }
    ];
    this.selectedColumns = this.colsListProject;
  }

  onChangeAction(rowData: any) {
    this.actions = [];

    let startProject: MenuItem = {
      id: '1', label: 'Bắt đầu', icon: 'fas fa-play-circle', command: () => {
        if (rowData.projectManagerId == null || rowData.projectManagerId == this.emptyGuild || rowData.projectManagerId == undefined) {
          this.confirmationService.confirm({
            message: 'Dự án hiện chưa có người quản lý, bạn phải chọn quản lý mới có thể tiếp thục hành động này?',
            accept: () => {
              let url = this.router.serializeUrl(this.router.createUrlTree(['/project/detail', { projectId: rowData.projectId }]));
              window.open(url, '_blank');
            }
          });
        }
        else {
          this.changeProjectStatus(rowData.projectId, "STA", 'Bạn có chắc chắn muốn thực hiện?');
        }
      }
    }

    let updateProject: MenuItem = {
      id: '2', label: 'Cập nhật', icon: 'far fa-edit', command: () => {
        let url = this.router.serializeUrl(this.router.createUrlTree(['/project/detail', { projectId: rowData.projectId }]));
        localStorage.setItem("Project_ID", rowData.projectId);
        window.open(url, '_blank');
      }
    }

    let deleteProject: MenuItem = {
      id: '3', label: 'Xóa', icon: 'pi pi-trash', command: () => {
        this.deleteProject(rowData.projectId, "DEL");
      }
    }

    let stopProject: MenuItem = {
      id: '4', label: 'Tạm dừng', icon: 'far fa-stop-circle', command: () => {
        this.changeProjectStatus(rowData.projectId, "STO", 'Bạn có chắc chắn muốn tạm dừng dự án?');
      }
    }

    let completeProject: MenuItem = {
      id: '4', label: 'Hoàn thành', icon: 'pi pi-check-square', command: () => {
        if (rowData.projectManagerId == null || rowData.projectManagerId == this.emptyGuild || rowData.projectManagerId == undefined) {
          this.confirmationService.confirm({
            message: 'Dự án hiện chưa có người quản lý, bạn phải chọn quản lý mới có thể tiếp thục hành động này?',
            accept: () => {
              let url = this.router.serializeUrl(this.router.createUrlTree(['/project/detail', { projectId: rowData.projectId }]));
              window.open(url, '_blank');
            }
          });
        }
        else {
          this.changeProjectStatus(rowData.projectId, "COM", 'Bạn có chắc chắn muốn hoàn thành dự án này?');
        }
      }
    }

    let closeProject: MenuItem = {
      id: '5', label: 'Đóng', icon: 'far fa-check-circle', command: () => {
        if (rowData.projectManagerId == null || rowData.projectManagerId == this.emptyGuild || rowData.projectManagerId == undefined) {
          this.confirmationService.confirm({
            message: 'Dự án hiện chưa có người quản lý, bạn phải chọn quản lý mới có thể tiếp thục hành động này?',
            accept: () => {
              let url = this.router.serializeUrl(this.router.createUrlTree(['/project/detail', { projectId: rowData.projectId }]));
              window.open(url, '_blank');
            }
          });
        }
        else {
          this.changeProjectStatus(rowData.projectId, "CLO", 'Bạn có chắc chắn muốn tạm đóng dự án?');
        }
      }
    }

    let cancelProject: MenuItem = {
      id: '6', label: 'Hủy', icon: 'far fa-times-circle', command: () => {
        this.changeProjectStatus(rowData.projectId, "HUY", 'Bạn có chắc chắn muốn hủy?');
      }
    }

    let openProject: MenuItem = {
      id: '7', label: 'Mở lại', icon: 'pi pi-replay', command: () => {
        if (rowData.projectManagerId == null || rowData.projectManagerId == this.emptyGuild || rowData.projectManagerId == undefined) {
          this.confirmationService.confirm({
            message: 'Dự án hiện chưa có người quản lý, bạn phải chọn quản lý mới có thể tiếp thục hành động này?',
            accept: () => {
              let url = this.router.serializeUrl(this.router.createUrlTree(['/project/detail', { projectId: rowData.projectId }]));
              window.open(url, '_blank');
            }
          });
        }
        else {
          if (rowData.actualEndDate) {
            let today = new Date();
            let endDate = new Date(rowData.actualEndDate);
            if (endDate <= today) {
              this.changeProjectStatus(rowData.projectId, "OPE", 'Dự án này thực tế đã kết thúc, nếu mở lại thông tin thời gian kết thúc thực tế sẽ bị xóa?');
            }
            else {
              this.changeProjectStatus(rowData.projectId, "OPE", 'Bạn có chắc chắn muốn mở lại dự án?');
            }
          }
          else {
            this.changeProjectStatus(rowData.projectId, "OPE", 'Bạn có chắc chắn muốn mở lại dự án?');
          }
        }
      }
    }

    if (rowData.projectStatusCode == 'MOI') {
      this.actions.push(startProject);
      this.actions.push(updateProject);
      this.actions.push(deleteProject);
    }

    if (rowData.projectStatusCode == 'DTK') {
      this.actions.push(updateProject);
      this.actions.push(completeProject);
      this.actions.push(stopProject);
    }

    if (rowData.projectStatusCode == 'TDU') {
      this.actions.push(openProject);
      this.actions.push(closeProject);
      this.actions.push(cancelProject);
    }

    if (rowData.projectStatusCode == 'HTH') {
      this.actions.push(updateProject);
      this.actions.push(closeProject);
    }

    if (rowData.projectStatusCode == 'DON') {
      this.actions.push(updateProject);
      this.actions.push(openProject);
    }

    if (rowData.projectStatusCode == 'HUY') {
      this.actions.push(openProject);
    }
  }

  changeProjectStatus(projectId, status, stringMessage) {
    this.confirmationService.confirm({
      message: stringMessage,
      accept: () => {
        this.loading = true;
        this.projectService.changeProjectStatus(projectId, status).subscribe(response => {
          this.loading = false;
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            var statusId = result.statusId;
            if (statusId) {
              let tmplistProject = this.listProject;
              let project = tmplistProject.find(e => e.projectId == projectId);

              let status = this.listStatus.find(st => st.categoryId == statusId)
              if (status) {
                // project.projectStatus = statusId;
                // project.projectStatusName = status.categoryName;
                // project.projectStatusCode = status.categoryCode;

                this.searchProject();

                let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Cập nhật dự án thành công' };
                this.showMessage(mgs);
              }
            }
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
            this.showMessage(mgs);
          }
        }, error => { this.loading = false; });
      }
    });
  }

  deleteProject(projectId, status) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.projectService.changeProjectStatus(projectId, status).subscribe(response => {
          this.loading = false;
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.listProject = this.listProject.filter(e => e.projectId != projectId);
            this.listProjectDTK = this.listProject.filter(x => x.projectStatusCode == 'MOI' || x.projectStatusCode == 'HTH' || x.projectStatusCode == 'DTK');
            this.listProjectHUY = this.listProject.filter(x => x.projectStatusCode == 'DON' || x.projectStatusCode == 'HUY' || x.projectStatusCode == 'TDU');
            let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Xóa dự án thành công' };
            this.showMessage(mgs);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
            this.showMessage(mgs);
          }
        }, error => { this.loading = false; });
      }
    });
  }
  searchProject() {
    this.loading = true;
    let projectStartS = this.projectStartS;
    if (projectStartS) {
      projectStartS.setHours(0, 0, 0, 0);
      projectStartS = convertToUTCTime(projectStartS);
    }
    let projectStartE = this.projectStartE;
    if (projectStartE) {
      projectStartE.setHours(0, 0, 0, 0);
      projectStartE = convertToUTCTime(projectStartE);
    }
    let projectEndS = this.projectEndS;
    if (projectEndS) {
      projectEndS.setHours(0, 0, 0, 0);
      projectEndS = convertToUTCTime(projectEndS);
    }
    let projectEndE = this.projectEndE;
    if (projectEndE) {
      projectEndE.setHours(0, 0, 0, 0);
      projectEndE = convertToUTCTime(projectEndE);
    }
    let actualStartS = this.actualStartS;
    if (actualStartS) {
      actualStartS.setHours(0, 0, 0, 0);
      actualStartS = convertToUTCTime(actualStartS);
    }
    let actualStartE = this.actualStartE;
    if (actualStartE) {
      actualStartE.setHours(0, 0, 0, 0);
      actualStartE = convertToUTCTime(actualStartE);
    }
    let actualEndS = this.actualEndS;
    if (actualEndS) {
      actualEndS.setHours(0, 0, 0, 0);
      actualEndS = convertToUTCTime(actualEndS);
    }
    let actualEndE = this.actualEndE;
    if (actualEndE) {
      actualEndE.setHours(0, 0, 0, 0);
      actualEndE = convertToUTCTime(actualEndE);
    }


    var projectType = this.selectedProjectType.map(p => p.categoryId);
    var employee = this.selectedEmployee.map(p => p.employeeId);
    var statusProject = this.selectedStatus.map(p => p.categoryId);
    this.projectService.searchProject(this.projectCode, this.projectName, projectStartS, projectStartE, projectEndS, projectEndE, actualStartS, actualStartE, actualEndS, actualEndE,
      statusProject, projectType, employee, this.estimateCompleteTimeS, this.estimateCompleteTimeE, this.completeRateS, this.completeRateE).subscribe(response => {
        let result: any = response;

        this.loading = false;
        if (result.statusCode == 200) {
          if (this.listStatus.length == 0) {
            this.listStatus = result.listStatus;
          }
          this.listProjectType = result.listProjectType;
          this.listProject = result.listProject;
          this.listProjectDTK = this.listProject.filter(x => x.projectStatusCode == 'MOI' || x.projectStatusCode == 'HTH' || x.projectStatusCode == 'DTK');
          this.listProjectHUY = this.listProject.filter(x => x.projectStatusCode == 'DON' || x.projectStatusCode == 'HUY' || x.projectStatusCode == 'TDU');
          this.listEmployee = result.listEmployee;
          this.isShowFilterLeft = false;
          this.leftColNumber = 12;
          this.rightColNumber = 0;
          if (this.listProject.length == 0) {
            let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy dự án nào!' };
            this.showMessage(mgs);
          } else {
            let listProjectCancel = [];

            this.listProject.forEach((item, index) => {
              item.completeRate = item.taskComplate + '%';

              if (item.projectStatusCode == 'HUY') {
                listProjectCancel.push(item);
                this.listProject.splice(index, 1);
              }
            });

            listProjectCancel.forEach(item => this.listProject.push(item));
          }
        } else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
        }
      });
  }

  resetTable() {
    if (this.myTableDtk) {
      this.myTableDtk.reset();
    }
    if (this.myTableHuy) {
      this.myTableHuy.reset();
    }
  }

  // Refresh parameter search
  refreshFilter() {
    this.selectedProjectType = [];
    this.selectedStatus = [];
    this.selectedEmployee = [];
    this.projectCode = '';
    this.projectName = '';
    this.projectStartS = null;
    this.projectStartE = null;
    this.projectEndS = null;
    this.projectEndE = null;
    this.actualStartS = null;
    this.actualStartE = null;
    this.actualEndS = null;
    this.actualEndE = null;
    this.isGlobalFilter = '';
    this.estimateCompleteTimeS = 0;
    this.estimateCompleteTimeE = 0;
    this.completeRateS = 0;
    this.completeRateE = 0;

    this.resetTable();

    this.searchProject();
  }

  pageChange(event: any) {
  }

  leftColNumber: number = 12;
  rightColNumber: number = 2;
  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.rightColNumber = 4;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }
  exportExcel() {
    if (this.listProject.length > 0) {
      if (this.selectedColumns.length > 0) {
        let title = `Danh sách dự án`;

        let workBook = new Workbook();
        let worksheet = workBook.addWorksheet(title);

        let colA = worksheet.getColumn('A');
        colA.width = 42;

        let colB = worksheet.getColumn('B');
        colB.width = 18.57;

        let colC = worksheet.getColumn('C');
        colC.width = 38.71;

        let colD = worksheet.getColumn('D');
        colD.width = 20.57;

        let colE = worksheet.getColumn('E');
        colE.width = 12.42;

        let colF = worksheet.getColumn('F');
        colF.width = 12.42;

        let colG = worksheet.getColumn('G');
        colG.width = 12.42;

        let colH = worksheet.getColumn('H');
        colH.width = 10.42;

        let colI = worksheet.getColumn('I');
        colI.width = 20.71;

        let dataHeaderMain = "Danh sách dự án".toUpperCase();
        let headerMain = worksheet.addRow([dataHeaderMain]);
        headerMain.font = { size: 18, bold: true };
        worksheet.mergeCells(`A${1}:I${1}`);
        headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        worksheet.addRow([]);

        /* Header row */
        let buildColumnExcel = this.buildColumnExcel();
        let dataHeaderRow = buildColumnExcel.map(x => x.name);
        let headerRow = worksheet.addRow(dataHeaderRow);
        headerRow.font = { name: 'Times New Roman', size: 10, bold: true };
        dataHeaderRow.forEach((item, index) => {
          headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          headerRow.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: '8DB4E2' }
          };
        });
        headerRow.height = 40;

        /* Data table */
        let data: Array<any> = [];
        this.listProject.forEach(item => {
          let row: Array<any> = [];
          buildColumnExcel.forEach((_item, _index) => {
            if (_item.code == 'projectCode') {
              row[_index] = item.projectCode;
            }
            else if (_item.code == 'projectName') {
              row[_index] = item.projectName;
            }
            else if (_item.code == 'projectTypeName') {
              row[_index] = item.projectTypeName;
            }
            else if (_item.code == 'employeeName') {
              row[_index] = item.employeeName;
            }
            else if (_item.code == 'projectStatusName') {
              row[_index] = item.projectStatusName;
            }
            else if (_item.code == 'projectStartDate') {
              row[_index] = this.datePipe.transform(item.projectStartDate, 'dd/MM/yyyy');
            }
            else if (_item.code == 'projectEndDate') {
              row[_index] = this.datePipe.transform(item.projectEndDate, 'dd/MM/yyyy');
            }
            else if (_item.code == 'actualStartDate') {
              row[_index] = this.datePipe.transform(item.actualStartDate, 'dd/MM/yyyy');
            }
            else if (_item.code == 'actualEndDate') {
              row[_index] = this.datePipe.transform(item.actualEndDate, 'dd/MM/yyyy');
            }
            else if (_item.code == 'butget') {
              row[_index] = item.butget;
            }
            else if (_item.code == 'completeRate') {
              row[_index] = item.completeRate;
            }
          });

          data.push(row);
        });

        data.forEach((el, index, array) => {
          let row = worksheet.addRow(el);
          row.font = { name: 'Times New Roman', size: 11 };

          buildColumnExcel.forEach((_item, _index) => {
            row.getCell(_index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
            if (_item.code == 'projectName' || _item.code == 'projectTypeName' || _item.code == 'employeeName') {
              row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'left' };
            } else if (_item.code == 'projectStartDate' || _item.code == 'projectEndDate' || _item.code == 'actualStartDate' || _item.code == 'estimateCompleteTime' || _item.code == 'completeRate') {
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
        this.showToast('warn', 'Thông báo', 'Không có dự án nào');
      }
    }
    else {
      this.showToast('warn', 'Thông báo', 'Không có dự án nào');
    }
  }

  buildColumnExcel(): Array<ColumnExcel> {
    let listColumn: Array<ColumnExcel> = [];

    this.selectedColumns.forEach(item => {
      let column = new ColumnExcel();
      column.code = item.field;
      column.name = item.header;

      if (item.field == 'projectCode') {
        column.width = 30;
      }
      else if (item.field == 'projectName') {
        column.width = 30;
      }
      else if (item.field == 'projectTypeName') {
        column.width = 30;
      }
      else if (item.field == 'employeeName') {
        column.width = 15;
      }
      else if (item.field == 'projectStart') {
        column.width = 15;
      }
      else if (item.field == 'projectStatusName') {
        column.width = 20;
      }
      else if (item.field == 'projectEnd') {
        column.width = 15;
      }
      else if (item.field == 'actualStart') {
        column.width = 15;
      }
      else if (item.field == 'actualEnd') {
        column.width = 15;
      }
      else if (item.field == 'butget') {
        column.width = 20;
      }
      else if (item.field == 'completeRate') {
        column.width = 20;
      }
      listColumn.push(column);
    });

    return listColumn;
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  dateFieldFormat: string = 'DD/MM/YYYY';
  customSort(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];

      /**Customize sort date */
      if (event.field == 'quoteDate') {
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
  }

  setDafaultStartDate(): Date {
    let date = new Date();
    date.setDate(1);

    return date;
  }

  goToCreateProject() {
    this.router.navigate(['/project/create']);
  }
  goToDetail(id: string) {
    let url = this.router.serializeUrl(this.router.createUrlTree(['/project/dashboard', { projectId: id }]));
    window.open(url, '_blank');
  }
  ngOnDestroy() {
    if (this.emitParamUrl) {
      this.emitParamUrl.unsubscribe();
    }
  }

  getDefaultApplicationName() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "ApplicationName") ?.systemValueString;
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
