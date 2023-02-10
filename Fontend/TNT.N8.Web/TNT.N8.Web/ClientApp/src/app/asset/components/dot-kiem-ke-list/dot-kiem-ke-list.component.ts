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
import { EmployeeService } from '../../../employee/services/employee.service';
import { EncrDecrService } from '../../../shared/services/encrDecr.service';
import { companyConfigModel } from '../../models/bao-cao.model';

@Component({
  selector: 'app-dot-kiem-ke-list',
  templateUrl: './dot-kiem-ke-list.component.html',
  styleUrls: ['./dot-kiem-ke-list.component.css'],
  providers: [
    DatePipe,
  ]
})
export class DotKiemKeListComponent implements OnInit {
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  auth: any = JSON.parse(localStorage.getItem('auth'));
  //get system parameter
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  loading: boolean = false;

  listDotKiemKe: Array<any> = []; // Danh sách tài sản
  selectedColumns: any[];
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  isUpdate: boolean = false;
  isDisplayName: boolean = false;

  //Form của đợt kiểm kê
  dotKiemKeFormGroup: FormGroup;
  tenDotKiemKeControl: FormControl;
  ngayBatDauControl: FormControl;
  ngayKetThucControl: FormControl;

  //Search
  tenDotKiemKe: string = null;
  ngayBatDau: Date = null;
  ngayKetThuc: Date = null;
  listTrangThai: Array<any> = [];

  //responsive
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  leftColNumber: number = 12;
  leftColGridNumber: number = 12;
  rightColNumber: number = 2;
  colsList: any[];
  todayDate: Date = new Date();

  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('save') save: ElementRef;
  @ViewChild('myTable') myTable: Table;
  filterGlobal: string = '';
  taiSanId: number = 0;
  companyConfig = new companyConfigModel();


  showTaoMoi: boolean = false;
  listTrangThaiKiemKe: Array<any> = [];
  constructor(
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private el: ElementRef,
    private renderer: Renderer2,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    public dialogService: DialogService,
    private router: Router,
    private assetService: AssetService,
    private datePipe: DatePipe,
    private ref: ChangeDetectorRef,
    private employeeService: EmployeeService,
    private encrDecrService: EncrDecrService,
  ) {
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    this.setForm();
    this.setTable();
    let resource = "ass/asset/danh-sach-dot-kiem-ke";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    } 
    else {
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
    }
    
    this.getMasterData();
  }

  setForm() {
    this.tenDotKiemKeControl = new FormControl(null, [Validators.required]);
    this.ngayBatDauControl = new FormControl(null, [Validators.required]);
    this.ngayKetThucControl = new FormControl(null, [Validators.required]);

    this.dotKiemKeFormGroup = new FormGroup({
      tenDotKiemKeControl: this.tenDotKiemKeControl,
      ngayBatDauControl: this.ngayBatDauControl,
      ngayKetThucControl: this.ngayKetThucControl
    });
  }

  setTable() {
    this.colsList = [
      { field: 'tenDoiKiemKe', header: 'Tên đợt kiểm kê', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'ngayBatDau', header: 'Ngày bắt đầu', width: '200px', textAlign: 'left', color: '#f44336' },
      { field: 'ngayKetThuc', header: 'Ngày kết thúc', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'soLuongTaiSan', header: 'Số lượng tài sản kiểm kê', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'trangThaiId', header: 'Trạng thái', width: '100px', textAlign: 'center', color: '#f44336' },
    ];
    this.selectedColumns = this.colsList;
  }

  deleteDotKiemKe(rowData) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa đợt kiểm kê?',
      accept: () => {
        this.assetService.deleteDotKiemKe(rowData.dotKiemKeId).subscribe(response => {
          var result = <any>response;
          if (result.statusCode == 200) {
            this.listDotKiemKe = this.listDotKiemKe.filter(c => c != rowData);
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.message };
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
    let tenDotKiemKe = this.tenDotKiemKe;
    let ngayBatDau = this.ngayBatDau ? convertToUTCTime(new Date(this.ngayBatDau)) : null;
    let ngayKetThuc = this.ngayKetThuc ? convertToUTCTime(new Date(this.ngayKetThuc)) : null;
    let listTrangThai = this.listTrangThai ? this.listTrangThai.map(x => x.value) : [];
    this.assetService.dotKiemKeSearch(tenDotKiemKe, ngayBatDau, ngayKetThuc, listTrangThai).subscribe(response => {
      var result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listDotKiemKe = result.listDotKiemKe;
        this.companyConfig = result.companyConfig;
        this.listTrangThaiKiemKe = result.listTrangThaiKiemKe;
      }
    });
  }

  goToDetail(rowData) {
    this.router.navigate(['/asset/chi-tiet-dot-kiem-ke', { dotKiemKeId: this.encrDecrService.set(rowData.dotKiemKeId) }]);
  }

  refreshFilter() {
    this.filterGlobal = '';
    this.filterGlobal = null;
    this.leftColGridNumber = 12;
    if (this.listDotKiemKe.length > 0) {
      this.myTable.reset();
    }
    this.tenDotKiemKe = null;
    this.ngayBatDau = null;
    this.ngayKetThuc = null;
    this.listTrangThai = null;
    this.getMasterData();
  }

  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.leftColGridNumber = 12;
        this.rightColNumber = 4;
      } else {
        this.leftColNumber = 12;
        this.leftColGridNumber = 12;
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

  // Tìm kiếm tài sản
  async searchAsset(type: any) {
  }

  resetTable() {
    this.filterGlobal = '';
  }

  TaoDotKiemKe() {
    if (!this.dotKiemKeFormGroup.valid) {
      Object.keys(this.dotKiemKeFormGroup.controls).forEach(key => {
        if (this.dotKiemKeFormGroup.controls[key].valid == false) {
          this.dotKiemKeFormGroup.controls[key].markAsTouched();
        }
      });
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Vui lòng nhập đầy đủ thông tin các trường dữ liệu.' };
      this.showMessage(msg);
      return;
    }
    let tenDotKiemKe = this.tenDotKiemKeControl.value;
    let ngayBatDau = convertToUTCTime(new Date(this.ngayBatDauControl.value));
    let ngayKetThuc = convertToUTCTime(new Date(this.ngayKetThucControl.value));
    if (ngayBatDau > ngayKetThuc) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Ngày bắt đầu không được lớn hơn ngày kết thúc!' };
      this.showMessage(msg);
      return;
    }
    this.assetService.taoDotKiemKe(tenDotKiemKe, ngayBatDau, ngayKetThuc).subscribe(response => {
      var result = <any>response;
      if (result.statusCode == 200) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.message };
        this.showMessage(mgs);
        this.getMasterData();
        this.showTaoMoi = false;
      } else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.message };
        this.showMessage(mgs);
      }
    });
  }
}

function monthDiff(d1, d2) {
  var months;
  months = (d2.getFullYear() - d1.getFullYear()) * 12;
  months -= d1.getMonth();
  months += d2.getMonth();
  return months <= 0 ? 0 : months;
}

function getCountMonth(startDate, endDate) {
  if (startDate == null || startDate == undefined)
    return 0;
  else
    return (
      endDate.getMonth() -
      startDate.getMonth() +
      12 * (endDate.getFullYear() - startDate.getFullYear())
    );
}


function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
