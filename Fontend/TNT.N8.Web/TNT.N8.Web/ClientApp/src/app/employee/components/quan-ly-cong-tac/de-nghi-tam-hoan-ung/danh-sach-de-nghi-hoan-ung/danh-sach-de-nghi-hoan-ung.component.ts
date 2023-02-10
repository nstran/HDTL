
import { Component, OnInit, ElementRef, ChangeDetectorRef, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { DatePipe } from '@angular/common';
import 'moment/locale/pt-br';
import { FormControl, FormGroup } from '@angular/forms';
import { Table } from 'primeng/table';
import { QuanLyCongTacService } from '../../../../../../app/employee/services/quan-ly-cong-tac/quan-ly-cong-tac.service';
import { Paginator } from 'primeng';
import { EncrDecrService } from '../../../../../shared/services/encrDecr.service';

class EmployeeModel {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  employeeLastname: string;
  employeeFirstname: string;
  startedDate: Date;
  organizationId: string;
  positionId: string;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  active: Boolean;
  username: string;
  identity: string;
  organizationName: string;
  avatarUrl: string;
  positionName: string;
  contactId: string;
  isManager: boolean;
  permissionSetId: string;
  probationEndDate: Date;
  probationStartDate: Date;
  trainingStartDate: Date;
  contractType: string;
  contractEndDate: Date;
  isTakeCare: boolean;
  isXuLyPhanHoi: boolean;
  organizationLevel: number;
}

@Component({
  selector: 'app-danh-sach-de-nghi-hoan-ung',
  templateUrl: './danh-sach-de-nghi-hoan-ung.component.html',
  styleUrls: ['./danh-sach-de-nghi-hoan-ung.component.css'],
  providers: [
    DatePipe,
  ]
})
export class DanhSachDeNghiHoanUngComponent implements OnInit {
  today: string = '';
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('paginator', { static: true }) paginator: Paginator
  @ViewChild('myTable') myTable: Table;
  /* End */
  /*Khai báo biến*/
  auth: any = JSON.parse(localStorage.getItem("auth"));
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  /** Action */
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  /**End */
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;

  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();

  // Dữ liệu masterdata
  listDeNghiTamHoanUng: Array<any> = new Array<any>();
  listEmployee: Array<EmployeeModel> = new Array<EmployeeModel>();

  colsList: any[];

  /* Form searcg */
  searchForm: FormGroup;
  maDeNghiControl: FormControl;
  trangThaiControl: FormControl;
  selectedNguoiYC: Array<any> = [];
  selectedColumns: any[];
  filterGlobal: string = '';

  //responsive
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  leftColNumber: number = 12;
  rightColNumber: number = 2;

  listTrangThai = [
    {
      id: 1, name: 'Mới'
    },
    {
      id: 2, name: 'Chờ phê duyệt'
    },
    {
      id: 3, name: 'Đã duyệt'
    },
    {
      id: 4, name: 'Từ chối'
    }
  ];

  constructor(
    private quanLyCongtacService: QuanLyCongTacService,
    private messageService: MessageService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private getPermission: GetPermission,
    private el: ElementRef,
    private ref: ChangeDetectorRef,
    private encrDecrService: EncrDecrService,
  ) {
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    this.setForm();
    this.setTable();
    let resource = "hrm/employee/danh-sach-de-nghi-hoan-ung";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
      return;
    } 

    if (permission != undefined) {
      let listCurrentActionResource = permission.listCurrentActionResource;

      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }

      this.getMasterData();
      this.setDefaultPaidDate();
    }
    else { this.router.navigate(['/home']); }
  }

  setForm() {
    this.maDeNghiControl = new FormControl(null);
    this.trangThaiControl = new FormControl(null);

    this.searchForm = new FormGroup({
      maDeNghiControl: this.maDeNghiControl,
      trangThaiControl: this.trangThaiControl
    });
  }

  setTable() {
    this.colsList = [
      { field: 'maDeNghi', header: 'Mã đề nghị', width: '100px', textAlign: 'left', color: '#f44336' },
      { field: 'nguoiDeXuat', header: 'Người đề nghị', width: '200px', textAlign: 'left', color: '#f44336' },
      { field: 'nguoiThuHuong', header: 'Người thụ hưởng', width: '200px', textAlign: 'left', color: '#f44336' },
      { field: 'ngayDeNghi', header: 'Ngày đề nghị', width: '150px', textAlign: 'center', color: '#f44336' },
      { field: 'tienTamUng', header: 'Chi phí hoàn ứng', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'tongTienThanhToan', header: 'Tổng chi phí', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'trangThaiString', header: 'Trạng thái', width: '100px', textAlign: 'center', color: '#f44336' },
    ];
    this.selectedColumns = this.colsList;
  }

  delItem(rowData) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa đề nghị hoàn ứng?',
      accept: () => {
        this.quanLyCongtacService.xoaDeNghiTamHoanUng(rowData.deNghiTamHoanUngId).subscribe(response => {
          var result = <any>response;
          if (result.statusCode == 200) {
            this.listDeNghiTamHoanUng = this.listDeNghiTamHoanUng.filter(c => c != rowData);
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Xóa đề nghị hoàn ứng thành công." };
            this.showMessage(mgs);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.message };
            this.showMessage(mgs);
          }
        });
      }
    });
  }

  setDefaultPaidDate() {
    var datePipe = new DatePipe("en-US");
    var _today = new Date();
    this.today = datePipe.transform(_today, 'dd-MM-yyyy');
  }

  async getMasterData() {
    this.loading = true;
    this.quanLyCongtacService.getAllDenghiTamUngList(1).subscribe(response => {
      var result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listDeNghiTamHoanUng = result.listDeNghiTamHoanUng;
        this.listEmployee = result.listEmployee;
      }
    });
  }

  checkEnterPress(event: any) {
    if (event.code === "Enter") {
      this.searchDeNghiTamUng();
    }
  }

  refreshFilter() {
    this.filterGlobal = '';
    this.filterGlobal = null;
    this.selectedNguoiYC = [];
    this.searchForm.reset();
    if (this.listDeNghiTamHoanUng.length > 0) {
      this.myTable.reset();
    }
    this.searchDeNghiTamUng();
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

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  goToCreate() {
    this.router.navigate(['/employee/tao-de-nghi-tam-hoan-ung', { loaiDeNghi: 1 }]);
  }

  async searchDeNghiTamUng() {
    let selectedNguoiYCId: Array<string> = [];
    let maDenghi = this.searchForm.get('maDeNghiControl').value;
    let trangThai = this.searchForm.get('trangThaiControl').value == null ? null : this.searchForm.get('trangThaiControl').value.id;

    if (this.selectedNguoiYC.length > 0) {
      this.selectedNguoiYC.forEach(item => {
        selectedNguoiYCId.push(item.employeeId);
      });
    }

    this.loading = true;
    let result: any = await this.quanLyCongtacService.searchDeNghiTamUng(maDenghi, selectedNguoiYCId, trangThai, 1);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listDeNghiTamHoanUng = result.listDeNghiTamHoanUng;
      this.ref.detectChanges();
      if (this.listDeNghiTamHoanUng.length == 0) {
        let mgs = { severity: 'warn', summary: 'Thông báo', detail: 'Không tìm thấy đề nghị hoàn ứng nào!' };
        this.showMessage(mgs);
      }
    }
  }

  resetTable() {
    this.filterGlobal = '';
  }

  goToDetail(rowData: any) {
    this.router.navigate(['/employee/chi-tiet-de-nghi-tam-hoan-ung', { deNghiTamHoanUngId: this.encrDecrService.set(rowData.deNghiTamHoanUngId), loaiDeNghi: 1 }]);
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

}
