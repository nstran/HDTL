import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Table } from 'primeng/table';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { MessageService, ConfirmationService } from 'primeng/api';
import { EncrDecrService } from '../../../../shared/services/encrDecr.service';
import { SalaryService } from '../../../services/salary.service';
import { CommonService } from '../../../../shared/services/common.service';
import { FormatDateService } from '../../../../shared/services/formatDate.services';


@Component({
  selector: 'app-phieu-luong-list',
  templateUrl: './phieu-luong-list.component.html',
  styleUrls: ['./phieu-luong-list.component.css']
})
export class PhieuLuongListComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;
  nowDate = new Date();

  filterGlobal: string = null;
  isShowFilterRight: boolean = false;
  leftColNumber: number = 12;
  rightColNumber: number = 4;

  listPhieuLuong: Array<any> = [];
  listKyLuong: Array<any> = [];
  listEmployee: Array<any> = [];

  listSelectedKyLuong: Array<any> = [];
  listSelectedEmployee: Array<any> = [];

  @ViewChild('myTable') myTable: Table;
  colsList: any;

  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private encrDecrService: EncrDecrService,
    private salaryService: SalaryService,
    private commonService: CommonService,
    private formatDateService: FormatDateService,
  ) { }

  async ngOnInit() {
    this.initTable();

    await this._getPermission();
    
    this.search();
  }

  initTable() {
    this.colsList = [
      { field: 'phieuLuongCode', header: 'Mã phiếu lương', textAlign: 'left', display: 'table-cell' },
      { field: 'tenKyLuong', header: 'Tên kỳ lương', textAlign: 'left', display: 'table-cell' },
      { field: 'employeeCode', header: 'Mã nhân viên', textAlign: 'left', display: 'table-cell' },
      { field: 'employeeName', header: 'Họ tên', textAlign: 'left', display: 'table-cell' },
      { field: 'positionName', header: 'Chức vụ', textAlign: 'left', display: 'table-cell' },
      { field: 'subCode1', header: 'Phòng ban', textAlign: 'left', display: 'table-cell' },
    ]
  }

  async _getPermission() {
    let resource = "salary/salary/phieu-luong-list/";
    this.loading = true;
    let permission: any = await this.getPermission.getPermission(resource);
    this.loading = false;

    if (permission.status == false) {
      this.router.navigate(["/home"]);
      return;
    }
  }

  async search() {
    let listKyLuongId = this.listSelectedKyLuong.map(x => x.kyLuongId);
    let listEmployeeId = this.listSelectedEmployee.map(x => x.employeeId);

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.getListPhieuLuong(listKyLuongId, listEmployeeId);
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.listKyLuong = result.listKyLuong;
    this.listEmployee = result.listEmployee;
    this.listPhieuLuong = result.listPhieuLuong;
  }

  goToDetailPhieuLuong(data: any) {
    this.router.navigate(['/salary/phieu-luong-detail', { phieuLuongId: this.encrDecrService.set(data.phieuLuongId) }]);
  }

  goToDetailKyLuong(data: any) {
    this.router.navigate(['/salary/ky-luong-detail', { kyLuongId: this.encrDecrService.set(data.kyLuongId) }]);
  }

  goToDetailNhanVien(data: any) {
    this.router.navigate(['/employee/detail', { employeeId: data.employeeId }]);
  }

  refreshFilter() {
    this.filterGlobal = null;
    this.myTable?.reset();
    this.listSelectedKyLuong = [];
    this.listSelectedEmployee = [];
    this.search();
  }

  showFilter() {
    this.isShowFilterRight = !this.isShowFilterRight;

    if (this.isShowFilterRight) this.leftColNumber = 8;
    else this.leftColNumber = 12;
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }
}
