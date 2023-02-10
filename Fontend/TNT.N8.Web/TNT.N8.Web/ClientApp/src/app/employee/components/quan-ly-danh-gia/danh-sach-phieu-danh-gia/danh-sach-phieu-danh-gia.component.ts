import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { EmployeeService } from '../../../services/employee.service';
import { DialogService } from 'primeng';
import { EmployeeListService } from '../../../services/employee-list/employee-list.service';
import { EncrDecrService } from '../../../../shared/services/encrDecr.service';


@Component({
  selector: 'app-danh-sach-phieu-danh-gia',
  templateUrl: './danh-sach-phieu-danh-gia.component.html',
  styleUrls: ['./danh-sach-phieu-danh-gia.component.css']
})


export class DanhSachPhieuDanhGiaComponent implements OnInit {
  auth = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  messages: any;

  nguoiDeXuatId = null;
  thoiGianDeXuat = null;
  trangThai = null;

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

  listPhieuDanhGia: any[] = [];

  today = new Date();

  listTrangThai = [
    { value: 1, name: "Mới" },
    { value: 2, name: "Chờ phê duyệt" },
    { value: 3, name: "Đã duyệt" },
    { value: 4, name: "Từ chối" },
  ]

  listTinhTheo = [
    { value: 1, name: "Tổng điểm thành phần*Trọng số" },
    { value: 2, name: "Trung bình cộng điểm thành phần*Trọng số" },
  ];

  actionDelete: boolean = true;
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    private def: ChangeDetectorRef,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private ref: ChangeDetectorRef,
    private employeeListService: EmployeeListService,
    private encrDecrService: EncrDecrService,
    private getPermission: GetPermission,
  ) { }


  goToCreate() {
    this.router.navigate(['/employee/tao-phieu-danh-gia']);
  }

  async ngOnInit() {
    //Danh sach
    let resource0 = "hrm/employee/danh-sach-phieu-danh-gia/";
    let permission0: any = await this.getPermission.getPermission(resource0);
    if (permission0.status == false) {
      this.router.navigate(['/home']);
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập' };
      this.showMessage(msg);
    }
    else {
      let listCurrentActionResource = permission0.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập' };
        this.showMessage(msg);
      }
    }

    //Chi tiết
    let resource = "hrm/employee/chi-tiet-phieu-danh-gia/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.actionDelete = false;
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
    }


    this.getMasterData();
    this.initTable();
  }

  initTable() {
    this.colsList = [
      { field: 'maPhieuDanhGia', header: 'Mã phiếu', textAlign: 'left', display: 'table-cell' },
      { field: 'tenPhieuDanhGia', header: 'Tên phiếu đánh giá', textAlign: 'left', display: 'table-cell' },
      { field: 'createdDate', header: 'Ngày tạo', textAlign: 'left', display: 'table-cell' },
      { field: 'trangThaiPhieuDanhGia', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
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
    let result: any = await this.employeeListService.danhSachPhieuDanhGia();
    this.loading = false;
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Lấy thông tin không thành công' };
      this.showMessage(msg);
      return;
    }
    this.listPhieuDanhGia = result.listPhieuDanhGia.reverse();
  }



  deletePhieuDanhGia(rowData: any) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.employeeListService.deletePhieuDanhGia(rowData.phieuDanhGiaId).subscribe(response => {
          let result = <any>response;
          this.loading = false;
          if (result.statusCode == 200) {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: result.message };
            this.listPhieuDanhGia = this.listPhieuDanhGia.filter(item => item.phieuDanhGiaId != rowData.phieuDanhGiaId);
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
            this.showMessage(msg);
          }
        }, error => { });
      }
    });
  }

  onViewDetail(rowData: any) {
    this.router.navigate(['/employee/chi-tiet-phieu-danh-gia', { phieuDanhGiaId: this.encrDecrService.set(rowData.phieuDanhGiaId) }]);
  }

  async refreshFilter() {
    this.loading = true
    this.myTable.reset();
    this.filterGlobal = '';
    await this.getMasterData()
    this.loading = false
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
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
