import * as $ from 'jquery';
import { Component, OnInit, ElementRef, HostListener, Renderer2, ChangeDetectorRef, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { DatePipe } from '@angular/common';
import 'moment/locale/pt-br';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { Table } from 'primeng/table';
import { QuanLyCongTacService } from '../../../../../../app/employee/services/quan-ly-cong-tac/quan-ly-cong-tac.service';
import { FileUpload, Paginator } from 'primeng';
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
  selector: 'app-ho-so-cong-tac-list',
  templateUrl: './ho-so-cong-tac-list.component.html',
  styleUrls: ['./ho-so-cong-tac-list.component.css'],
  providers: [
    DatePipe,
  ]
})
export class HoSoCongTacListComponent implements OnInit {
  today: string = '';
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  @ViewChild('paginator', { static: true }) paginator: Paginator
  @ViewChild('myTable') myTable: Table;
  /* End */
  /*Khai báo biến*/
  auth: any = JSON.parse(localStorage.getItem("auth"));
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;

  /** Action */
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  /**End */

  /* Form */
  taoHoSoCongTacForm: FormGroup;

  theoQuyetDinhCTControl: FormControl;
  donViDenCTControl: FormControl;
  ngayBatDauControl: FormControl;
  ngayKetThucControl: FormControl;
  diaDiemCTControl: FormControl;
  phuongTienControl: FormControl;
  nhiemVuControl: FormControl;
  /* End */

  isShow: boolean = true;

  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();

  // Dữ liệu masterdata
  listHoSoCongTac: Array<any> = new Array<any>();
  listEmployee: Array<EmployeeModel> = new Array<EmployeeModel>();

  deXuatCTModel: any;

  // notification
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;

  colsList: any[];

  // Tài liệu liên quan
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];
  colsFile: any[];
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  employeeModel: EmployeeModel = new EmployeeModel();
  isUpdate: boolean = false;

  /* Form searcg */
  searchForm: FormGroup;
  maHoSoControl: FormControl;
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
      id: 0, name: 'Mới'
    },
    {
      id: 1, name: 'Hoàn thành'
    }
  ];

  constructor(
    private quanLyCongtacService: QuanLyCongTacService,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private dialogService: DialogService,
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
    let resource = "hrm/employee/danh-sach-ho-so-cong-tac/";
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
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }

      this.getMasterData();
      this.setDefaultPaidDate();
    }
  }

  setForm() {
    this.maHoSoControl = new FormControl(null); // Mã tài sản
    this.trangThaiControl = new FormControl(null); // Hiện trạng tài sản

    this.searchForm = new FormGroup({
      maHoSoControl: this.maHoSoControl,
      trangThaiControl: this.trangThaiControl
    });
  }

  setTable() {
    this.colsList = [
      { field: 'maHoSoCongTac', header: 'Mã hồ sơ', width: '100px', textAlign: 'left', color: '#f44336' },
      { field: 'noiCongTac', header: 'Nơi công tác', width: '200px', textAlign: 'center', color: '#f44336' },
      { field: 'thoiGian', header: 'Thời gian', width: '200px', textAlign: 'left', color: '#f44336' },
      { field: 'trangThaiString', header: 'Trạng thái', width: '100px', textAlign: 'left', color: '#f44336' },
    ];
    this.selectedColumns = this.colsList;
  }

  delHoSo(rowData) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa hồ sơ công tác?',
      accept: () => {
        this.quanLyCongtacService.xoaHoSoCongTac(rowData.hoSoCongTacId).subscribe(response => {
          var result = <any>response;
          if (result.statusCode == 200) {
            this.listHoSoCongTac = this.listHoSoCongTac.filter(c => c != rowData);
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Xóa hồ sơ công tác thành công." };
            this.showMessage(mgs);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
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
    this.quanLyCongtacService.getAllHoSoCongTacList().subscribe(response => {
      var result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listHoSoCongTac = result.listHoSoCongTac;
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
    this.searchForm.reset();
    if (this.listHoSoCongTac.length > 0) {
      this.myTable.reset();
    }
    this.searchHoSoCongTac();
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
    this.router.navigate(['/employee/tao-ho-so-cong-tac']);
  }

  // Tìm kiếm yêu cầu cấp phát tài sản
  async searchHoSoCongTac() {
    let selectedNguoiYCId: Array<string> = [];
    let hoSo = this.searchForm.get('maHoSoControl').value;
    let trangThai = this.searchForm.get('trangThaiControl').value == null ? null : this.searchForm.get('trangThaiControl').value.id;

    if (this.selectedNguoiYC.length > 0) {
      this.selectedNguoiYC.forEach(item => {
        selectedNguoiYCId.push(item.employeeId);
      });
    }

    this.loading = true;
    let result: any = await this.quanLyCongtacService.searchHoSoCongTac(hoSo, selectedNguoiYCId, trangThai);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listHoSoCongTac = result.listHoSoCongTac;
      this.ref.detectChanges();
      if (this.listHoSoCongTac.length == 0) {
        let mgs = { severity: 'warn', summary: 'Thông báo', detail: 'Không tìm thấy yêu cầu cấp phát nào!' };
        this.showMessage(mgs);
      }
    }
  }

  resetTable() {
    this.filterGlobal = '';
  }

  goToDetail(rowData: any) {

    this.router.navigate(['/employee/chi-tiet-ho-so-cong-tac', { hoSoCongTacId: this.encrDecrService.set(rowData.hoSoCongTacId) }]);
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

}
