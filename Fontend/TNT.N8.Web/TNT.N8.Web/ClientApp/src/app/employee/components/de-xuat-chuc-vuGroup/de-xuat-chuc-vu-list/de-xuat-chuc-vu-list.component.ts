import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { EncrDecrService } from '../../../../shared/services/encrDecr.service';
import { TranslateService } from '@ngx-translate/core';
import { EmployeeService } from "../../../services/employee.service";
import { EmployeeListService } from '../../../services/employee-list/employee-list.service';
import { OrganizationService } from "../../../../shared/services/organization.service";
import { EmployeeModel } from "../../../models/employee.model";
import { UserModel } from '../../../../shared/models/user.model';
import * as $ from 'jquery';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { OrganizationDialogComponent } from '../../../../shared/components/organization-dialog/organization-dialog.component';

import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { SortEvent } from 'primeng/api';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';

interface Employee {
  index: number,
  employeeId: string,
  employeeName: string,
  organizationId: string,
  organizationName: string,
  employeeCode: string,
  positionId: string,
  createdById: string,
  createdDate: string,
  active: boolean,
  username: string,
  contactId: string,
}

@Component({
  selector: 'app-de-xuat-chuc-vu-list',
  templateUrl: './de-xuat-chuc-vu-list.component.html',
  styleUrls: ['./de-xuat-chuc-vu-list.component.css']
})

export class DeXuatChucVuListComponent implements OnInit {

  excel: any = {
    'STT': '',
    'Tên': '',
    'Tên tài khoản': '',
    'Mã': '',
    'Đơn vị': '',
  };
  excelExport: Array<any> = [];
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  createPermission: string = 'employee/create';


  messages: any;

  selection: Array<any>;

  nguoiDeXuatId = null;
  thoiGianDeXuat = null;
  trangThai = null;

  listEmpAdd: any;

  loading: boolean = false;
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  filterGlobal: string;

  leftColNumber: number = 12;
  rightColNumber: number = 0;

  nowDate: Date = new Date();
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  @ViewChild('myTable') myTable: Table;
  colsList: any;
  selectedColumns: any[];

  listDeXuatChucVu: any[] = [];

  today = new Date();

  listTrangThai = [
    { value: 1, name: "Mới" },
    { value: 2, name: "Chờ phê duyệt" },
    { value: 3, name: "Đã duyệt" },
    { value: 4, name: "Từ chối" },
  ]

  actionAdd:boolean = true;
  actionDelete:boolean = true;

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private organizationService: OrganizationService,
    private employeeListService: EmployeeListService,
    private employeeService: EmployeeService,
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService,
    public dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private encrDecrService: EncrDecrService,
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  goToCreate() {
    this.router.navigate(['/employee/tao-de-xuat-chuc-vu']);
  }

  async ngOnInit() {
    this.initTable();

    let resource = "hrm/employee/danh-sach-de-xuat-chuc-vu/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    } else {

      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("view") == -1) {
        let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
        this.showMessage(mgs);
        this.router.navigate(['/home']);      
      }
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
    }

    this.getMasterData();

    this.searchDeXuatTangLuong();
  }

  initTable() {
    this.colsList = [
      { field: 'tenDeXuat', header: 'Tên đề xuất', textAlign: 'left', display: 'table-cell' },
      { field: 'ngayDeXuat', header: 'Ngày để xuất', textAlign: 'left', display: 'table-cell' },
      { field: 'nguoiDeXuatName', header: 'Người đề xuất', textAlign: 'left', display: 'table-cell' },
      { field: 'trangThai', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
      { field: 'soNhanSuDuocDeXuat', header: 'Số nhân sự được đề xuất', textAlign: 'center', display: 'table-cell' },
    ];

    this.selectedColumns = this.colsList;
  }
  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.employeeService.getMasterDataCreateDeXuatTangLuong();
    this.loading = false;
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Lấy thông tin không thành công' };
      this.showMessage(msg);
      return;
    }
    this.listEmpAdd = result.listEmp;

  }

  searchDeXuatTangLuong() {
    let nguoiDeXuat = null;
    let tgianDeXuat = null;
    let trangThai = null;
    if (this.nguoiDeXuatId != null) nguoiDeXuat = this.nguoiDeXuatId.employeeId;
    if (this.thoiGianDeXuat != null) tgianDeXuat = convertToUTCTime(this.thoiGianDeXuat);
    if (this.trangThai != null) trangThai = this.trangThai.value;
    this.loading = true;
    this.employeeListService.listDeXuatChucVu(nguoiDeXuat, tgianDeXuat, trangThai).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listDeXuatChucVu = result.listDeXuatChucVu;
        if (this.listDeXuatChucVu.length === 0) {
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy đề xuất thay đổi chức vụ nào' };
          this.showMessage(msg);
        }
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
        this.showMessage(msg);
      }

      this.setIndex();
    }, error => { });
  };


  deleteDeXuat(rowData: any) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.employeeListService.deleteDeXuatChucVu(rowData.deXuatThayDoiChucVuId).subscribe(response => {
          let result = <any>response;
          this.loading = false;
          if (result.statusCode == 200) {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: result.message };
            this.listDeXuatChucVu = this.listDeXuatChucVu.filter(item => item.deXuatThayDoiChucVuId != rowData.deXuatThayDoiChucVuId);
            this.showMessage(msg);
          } else {

            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
            this.showMessage(msg);
          }
          this.setIndex();
        }, error => { });
      }
    });
  }



  setIndex() {
    this.listDeXuatChucVu.forEach((item, index) => {
      item.index = index + 1;
    });
  }
  onViewDetail(rowData: any) {
    this.router.navigate(['/employee/de-xuat-chuc-vu-detail', { deXuatTLId: this.encrDecrService.set(rowData.deXuatThayDoiChucVuId) }]);
  }

  refreshFilter() {
    this.myTable?.reset();

    this.nguoiDeXuatId = null;
    this.thoiGianDeXuat = null;
    this.trangThai = null;

    this.filterGlobal = '';
    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;


    this.searchDeXuatTangLuong();
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

  // exportExcel() {
  //   let listCheck = [];

  //   if (this.selection.length > 0) {
  //     listCheck = this.selection;
  //     this.excelExport = [];
  //   }
  //   for (var i = 0; i < listCheck.length; i++) {
  //     this.excel = {
  //       'STT': i + 1,
  //       'Tên': listCheck[i].employeeName,
  //       'Tên tài khoản': listCheck[i].username,
  //       'Mã': listCheck[i].employeeCode,
  //       'Đơn vị': listCheck[i].organizationName,
  //     }
  //     this.excelExport.push(this.excel);
  //   }
  //   if (this.excelExport.length == 0){
  //     this.excelExport.push(this.excel);
  //   }
  //   this.employeeService.exportExcel(this.excelExport, 'Excel_Danh sách nhân viên');
  // }
  /*End*/
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

