import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Table } from 'primeng/table';
import { QuyTrinhService } from './../../../services/quy-trinh.service';
import { MessageService, ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-danh-sach-quy-trinh',
  templateUrl: './danh-sach-quy-trinh.component.html',
  styleUrls: ['./danh-sach-quy-trinh.component.css']
})
export class DanhSachQuyTrinhComponent implements OnInit {
  innerWidth: number = 0; //number window size first
  nowDate = new Date();
  @ViewChild('myTable') myTable: Table;
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  loading: boolean = false;
  filterGlobal: string = null;

  listEmployee: Array<any> = [];
  listSelectedEmp: Array<any> = [];
  tenQuyTrinh: string = null;
  maQuyTrinh: string = null;
  createdDateFrom: Date = null;
  createdDateTo: Date = null;
  listTrangThai = [
    { name: 'Hoạt động', value: true },
    { name: 'Không hoạt động', value: false },
  ];
  listSelectedTrangThai = [];

  cols: any[];
  listQuyTrinh: Array<any> = [];
  
  constructor(
    public router: Router,
    public route: ActivatedRoute,
    private quyTrinhService: QuyTrinhService,
    public messageService: MessageService
  ) { 
    this.innerWidth = window.innerWidth;
  }

  ngOnInit(): void {
    this.initTable();
    this.quyTrinhService.getMasterDataSearchQuyTrinh().subscribe(res => {
      let result: any = res;

      if (result.statusCode == 200) {
        this.listEmployee = result.listEmployee;
        this.search();
      }
      else {
        this.showMessage('error', result.messageCode);
      }
    });
  }

  initTable() {
    this.cols = [
      { field: 'maQuyTrinh', header: 'Mã quy trình', textAlign: 'left' },
      { field: 'tenQuyTrinh', header: 'Tên quy trình', textAlign: 'left' },
      { field: 'tenDoiTuongApDung', header: 'Đối tượng áp dụng', textAlign: 'left' },
      { field: 'nguoiTao', header: 'Người tạo', textAlign: 'left' },
      { field: 'ngayTao', header: 'Ngày tạo', textAlign: 'center' },
      { field: 'hoatDong', header: 'Hoạt động', textAlign: 'center' },
    ];
  }

  refreshFilter() {
    this.filterGlobal = null;
    this.myTable.reset();
    this.listSelectedEmp = [];
    this.tenQuyTrinh = null;
    this.maQuyTrinh = null;
    this.createdDateFrom = null;
    this.createdDateTo = null;
    this.listSelectedTrangThai = [];
    this.search();
  }

  goToCreate() {
    this.router.navigate(['admin/tao-moi-quy-trinh']);
  }

  leftColNumber: number = 12;
  rightColNumber: number = 2;

  showFilter() {
    if (this.innerWidth < 1423) {
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

  goDetail(id) {
    this.router.navigate(['admin/chi-tiet-quy-trinh', { Id: id }]);
  }

  search() {
    let listEmployeeId = this.listSelectedEmp.map(x => x.employeeId);
    let listTrangThai = this.listSelectedTrangThai.map(x => x.value);

    this.loading = true;
    this.quyTrinhService.searchQuyTrinh(
      listEmployeeId, 
      this.tenQuyTrinh?.trim(),
      this.maQuyTrinh?.trim(),
      convertToUTCTime(this.createdDateFrom),
      convertToUTCTime(this.createdDateTo),
      listTrangThai
    )
    .subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listQuyTrinh = result.listQuyTrinh;
      }
      else {
        this.showMessage('error', result.messageCode);
      }
    });
  }

  showMessage(severity: string, detail: string) {
    this.messageService.add({severity: severity, summary: 'Thông báo:', detail: detail});
  }
}

function convertToUTCTime(time: any) {
  if (time) 
    return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
  else 
    return null;
};
