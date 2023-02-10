import { FormatDateService } from './../../../../shared/services/formatDate.services';
import { ChonNhieuDvDialogComponent } from './../../../../shared/components/chon-nhieu-dv-dialog/chon-nhieu-dv-dialog.component';
import { EmployeeRequestService } from './../../../services/employee-request/employee-request.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { SortEvent } from 'primeng/api';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { EncrDecrService } from '../../../../shared/services/encrDecr.service';

@Component({
  selector: 'app-list-employee-request',
  templateUrl: './list-employee-request.component.html',
  styleUrls: ['./list-employee-request.component.css'],
  providers: [
  ]
})
export class ListEmployeeRequestComponent implements OnInit {
  loading: boolean = false;
  nowDate: Date = new Date();

  isShowOrganization: boolean = false;
  organizationId: string = null;
  organizationName: string = null;
  listStatus: Array<any> = [];
  listSelectedStatus: Array<any> = [];
  listKyHieuChamCong: Array<any> = [];
  listSelectedKyHieuChamCong: Array<any> = [];

  code = "";
  employeeCode = "";
  employeeName = "";
  currentOrganizationId: string = null;
  currentOrganizationName: string = null;

  listDeXuatXinNghi: Array<any> = [];

  actionAdd: boolean = true;

  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();
  filterGlobal: string;

  leftColNumber: number = 12;
  rightColNumber: number = 0;

  @ViewChild('myTable') myTable: Table;
  colsList: any;

  constructor(
    private getPermission: GetPermission,
    private employeeRequestService: EmployeeRequestService,
    private router: Router,
    private messageService: MessageService,
    private dialogService: DialogService,
    private encrDecrService: EncrDecrService
  ) {
    this.innerWidth = window.innerWidth;
  }

  goToCreate() {
    this.router.navigate(['/employee/request/create']);
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }

  async ngOnInit() {
    this.initTable();

    let resource = "hrm/employee/request/list/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
      return;
    }

    let listCurrentActionResource = permission.listCurrentActionResource;
    if (listCurrentActionResource.indexOf("add") == -1) {
      this.actionAdd = false;
    }

    await this.getMasterData();
  }

  initTable() {
    this.colsList = [
      { field: 'code', header: 'Mã đề xuất', width: '130px', textAlign: 'left', display: 'table-cell' },
      { field: 'employeeCodeName', header: 'Tên NV được đề xuất', width: '250px', textAlign: 'left', display: 'table-cell' },
      { field: 'organizationName', header: 'Đơn vị', textAlign: 'left', display: 'table-cell' },
      { field: 'statusName', header: 'Trạng thái', width: '150px', textAlign: 'center', display: 'table-cell' },
      { field: 'createdDate', header: 'Ngày tạo', width: '120px', textAlign: 'center', display: 'table-cell' },
      // { field: 'nguoiPheDuyet', header: 'Người phê duyệt', textAlign: 'left', display: 'table-cell' },
      { field: 'tenLoaiDeXuat', header: 'Loại đề xuất', width: '250px', textAlign: 'left', display: 'table-cell' },
    ];
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.employeeRequestService.getDataSearchEmployeeRequest();
    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.isShowOrganization = result.isShowOrganization;
    this.organizationId = result.organizationId;
    this.organizationName = result.organizationName;
    this.listStatus = result.listStatus;
    this.listKyHieuChamCong = result.listKyHieuChamCong;

    this.currentOrganizationId = result.organizationId;
    this.currentOrganizationName = result.organizationName;

    this.searchEmployeeRequest();
  }

  async searchEmployeeRequest() {
    if (this.code) {
      this.code = this.code.trim();
    }
    if (this.employeeCode) {
      this.employeeCode = this.employeeCode.trim();
    }
    if (this.employeeName) {
      this.employeeName = this.employeeName.trim();
    }

    let listStatusId: Array<string> = [];
    listStatusId = this.listSelectedStatus.map(x => x.value);

    let listLoaiDeXuatId: Array<string> = [];
    listLoaiDeXuatId = this.listSelectedKyHieuChamCong.map(x => x.value);

    this.loading = true;
    let result: any = await this.employeeRequestService.searchEmployeeRequest(this.code, this.employeeCode, this.employeeName,
      this.currentOrganizationId, listLoaiDeXuatId, listStatusId);
    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.listDeXuatXinNghi = result.listDeXuatXinNghi;

    if (!this.listDeXuatXinNghi.length) {
      this.showMessage('warn', 'Không tìm thấy đề xuất xin nghỉ nào!');
    }
  }

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

  //hàm refesh các input trong hàm search
  refreshFilter() {
    this.code = null;
    this.employeeCode = null;
    this.employeeName = null;
    this.currentOrganizationId = this.organizationId;
    this.currentOrganizationName = this.organizationName;
    this.listSelectedKyHieuChamCong = [];
    this.listSelectedStatus = [];

    this.filterGlobal = '';
    this.myTable ?.reset();
    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.listDeXuatXinNghi = [];
    this.searchEmployeeRequest();
  }

  openOrgPopup() {
    let ref = this.dialogService.open(ChonNhieuDvDialogComponent, {
      data: {
        mode: 2, //Chọn 1
        selectedId: this.currentOrganizationId
      },
      header: 'Chọn đơn vị',
      width: '40%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "350px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        //Nếu chọn
        if (result.length > 0) {
          this.currentOrganizationId = result[0].organizationId;
          this.currentOrganizationName = result[0].organizationName;
        }
        //Nếu bỏ chọn
        else {
          this.currentOrganizationId = null;
          this.currentOrganizationName = null;
        }
      }
    });
  }

  onViewDetail(rowData: any) {
    this.router.navigate(['/employee/detail', { employeeId: rowData.employeeId }]);
  }

  goToRequest(rowData: any) {
    this.router.navigate(['/employee/request/detail', { deXuatXinNghiId: this.encrDecrService.set(rowData.deXuatXinNghiId) }]);
  }
}







