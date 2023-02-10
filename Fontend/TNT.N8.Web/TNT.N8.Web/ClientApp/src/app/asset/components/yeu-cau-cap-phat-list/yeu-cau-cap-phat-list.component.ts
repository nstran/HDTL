import * as $ from 'jquery';
import { Component, OnInit, ViewChild, ElementRef, HostListener, Renderer2, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { DatePipe } from '@angular/common';
import 'moment/locale/pt-br';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { AssetService } from '../../services/asset.service';
import { Table } from 'primeng/table';
import { YeuCauCapPhatModel } from '../../models/yeu-cau-cap-phat';
import { EncrDecrService } from '../../../shared/services/encrDecr.service';


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
  selector: 'app-yeu-cau-cap-phat-list',
  templateUrl: './yeu-cau-cap-phat-list.component.html',
  styleUrls: ['./yeu-cau-cap-phat-list.component.css'],
  providers: [
    DatePipe,
  ]
})
export class DanhSachCapPhatListComponent implements OnInit {
  fixed: boolean = false;
  withFiexd: string = "";

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  auth: any = JSON.parse(localStorage.getItem('auth'));
  //get system parameter
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  loading: boolean = false;

  listYeuCauCapPhat: Array<YeuCauCapPhatModel> = []; // Danh sách yêu cầu
  listEmployee: Array<EmployeeModel> = new Array<EmployeeModel>();
  taiSanDetail: YeuCauCapPhatModel = new YeuCauCapPhatModel();
  selectedColumns: any[];
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  isUpdate: boolean = false;
  isDisplayName: boolean = false;

  /*START : FORM PriceList*/
  searchForm: FormGroup;
  /*END : FORM PriceList*/

  //responsive
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  leftColNumber: number = 12;
  rightColNumber: number = 2;
  today: string = '';
  colsList: any[];

  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('myTable') myTable: Table;
  filterGlobal: string = '';
  isShowDetail: boolean = true;

  listHienTrangTS = [
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
    },
    {
      id: 5, name: 'Hoàn thành cấp phát'
    }
  ];

  /* Form searcg */
  searchYeuCauCPForm: FormGroup;
  maYeuCauControl: FormControl;
  trangThaiControl: FormControl;
  selectedNguoiYC: Array<any> = [];

  constructor(
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private renderer: Renderer2,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    public dialogService: DialogService,
    private router: Router,
    private assetService: AssetService,
    private datePipe: DatePipe,
    private ref: ChangeDetectorRef,
    private encrDecrService: EncrDecrService,
  ) {
    this.innerWidth = window.innerWidth;
    this.renderer.listen('window', 'click', (e: Event) => {
      /**
       * Only run when toggleButton is not clicked
       * If we don't check this, all clicks (even on the toggle button) gets into this
       * section which in the result we might never see the menu open!
       * And the menu itself is checked here, and it's where we check just outside of
       * the menu and button the condition abbove must close the menu
       */
      if (this.toggleButton && this.notifi) {
        if (!this.toggleButton.nativeElement.contains(e.target) &&
          !this.notifi.nativeElement.contains(e.target) &&
          !this.save.nativeElement.contains(e.target)) {
          this.isOpenNotifiError = false;
        }
      }
    });
  }

  async ngOnInit() {
    this.setForm();
    this.setTable();
    let resource = "ass/asset/danh-sach-yeu-cau-cap-phat/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    } else {

      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("view") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionEdit = false;
      }
      // this.setForm();
      // this.setTable();
      this.getMasterData();
      this.setDefaultPaidDate();
    }
  }

  setForm() {
    this.maYeuCauControl = new FormControl(null); // Mã tài sản
    this.trangThaiControl = new FormControl(null); // Hiện trạng tài sản

    this.searchYeuCauCPForm = new FormGroup({
      maYeuCauControl: this.maYeuCauControl,
      trangThaiControl: this.trangThaiControl
    });
  }

  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }

  setDefaultPaidDate() {
    var datePipe = new DatePipe("en-US");
    var _today = new Date();
    this.today = datePipe.transform(_today, 'dd-MM-yyyy');
  }

  setTable() {
    this.colsList = [
      { field: 'maYeuCau', header: 'Mã yêu cầu', width: '100px', textAlign: 'left', color: '#f44336' },
      { field: 'soLuong', header: 'Số lượng', width: '80px', textAlign: 'center', color: '#f44336' },
      { field: 'nguoiDeXuat', header: 'Người yêu cầu', width: '200px', textAlign: 'left', color: '#f44336' },
      { field: 'phongBan', header: 'Phòng ban', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'trangThaiString', header: 'Trạng thái', width: '120px', textAlign: 'center', color: '#f44336' },
    ];
    this.selectedColumns = this.colsList;
  }

  delYeuCau(rowData) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa yêu cầu?',
      accept: () => {
        this.assetService.xoaYeuCauCapPhat(rowData.yeuCauCapPhatTaiSanId).subscribe(response => {
          var result = <any>response;
          if (result.statusCode == 200) {
            this.listYeuCauCapPhat = this.listYeuCauCapPhat.filter(c => c != rowData);
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa thành công.' };
            this.showMessage(mgs);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.message };
            this.showMessage(mgs);
          }
        });
      }
    });
  }

  getMasterData() {
    this.loading = true;
    this.assetService.getAllYeuCauCapPhatTSList().subscribe(response => {
      var result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listYeuCauCapPhat = result.listYeuCauCapPhatTaiSan;
        this.listEmployee = result.listEmployee;
      }
    });
  }

  checkEnterPress(event: any) {
    if (event.code === "Enter") {

    }
  }

  refreshFilter() {
    this.filterGlobal = '';
    this.filterGlobal = null;
    this.selectedNguoiYC = [];
    this.searchYeuCauCPForm.reset();
    if (this.listYeuCauCapPhat.length > 0) {
      this.myTable.reset();
    }
    this.searchYeuCauCP();
  }

  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.rightColNumber = 4;
        this.isShowDetail = false;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
        this.isShowDetail = true;
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
    this.router.navigate(['/asset/yeu-cau-cap-phat-tai-san']);
  }

  // Tìm kiếm yêu cầu cấp phát tài sản
  async searchYeuCauCP() {
    let selectedNguoiYCId: Array<string> = [];
    let maYeuCau = this.searchYeuCauCPForm.get('maYeuCauControl').value;
    let trangThai = this.searchYeuCauCPForm.get('trangThaiControl').value == null ? null : this.searchYeuCauCPForm.get('trangThaiControl').value.id;

    if (this.selectedNguoiYC.length > 0) {
      this.selectedNguoiYC.forEach(item => {
        selectedNguoiYCId.push(item.employeeId);
      });
    }

    this.loading = true;
    let result: any = await this.assetService.searchYeuCauCapPhatAsync(maYeuCau, selectedNguoiYCId, trangThai);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listYeuCauCapPhat = result.listYeuCauCapPhatTaiSan;
      this.ref.detectChanges();
      if (this.listYeuCauCapPhat.length == 0) {
        let mgs = { severity: 'warn', summary: 'Thông báo', detail: 'Không tìm thấy yêu cầu cấp phát nào!' };
        this.showMessage(mgs);
      }
    }
  }

  resetTable() {
    this.filterGlobal = '';
  }

  goToDetail(rowData: any) {
    this.router.navigate(['/asset/chi-tiet-yeu-cau-cap-phat', { requestId: this.encrDecrService.set(rowData.yeuCauCapPhatTaiSanId) }]);
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

}
