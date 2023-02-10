import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { Router } from '@angular/router';

import { EmployeeService } from './../../../../services/employee.service';

@Component({
  selector: 'app-cap-phat-tai-san',
  templateUrl: './cap-phat-tai-san.component.html',
  styleUrls: ['./cap-phat-tai-san.component.css']
})
export class CapPhatTaiSanComponent implements OnInit {
  loading: boolean = false;
  @Input() actionEdit: boolean;
  @Input() employeeId: string;

  cols: Array<any> = [];
  listCapPhatTaiSan: Array<any> = [];

  isShowButton: boolean = false;

  @ViewChild('myTable') myTable: Table;
  constructor(
    private messageService: MessageService,
    private employeeService: EmployeeService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.initTable();
    this.getListCapPhatTaiSan();
  }

  initTable() {
    this.cols = [
      { field: 'stt', header: '#', textAlign: 'center', colWith: '8%' },
      { field: 'ngayBatDau', header: 'Ngày bắt đầu', textAlign: 'center', colWith: '15%' },
      { field: 'ngayKetThuc', header: 'Ngày kết thúc', textAlign: 'center', colWith: '15%' },
      { field: 'maTaiSan', header: 'Mã tài sản', textAlign: 'left', colWith: '15%' },
      { field: 'tenTaiSan', header: 'Tên tài sản', textAlign: 'left', colWith: '15%' },
      { field: 'mucDichSuDung', header: 'Mục đích sử dụng', textAlign: 'left', colWith: '15%' },
      { field: 'moTa', header: 'Mô tả', textAlign: 'left', colWith: '' },
    ];
  }

  async getListCapPhatTaiSan() {
    this.loading = true;
    let result: any = await this.employeeService.getListCapPhatTaiSan(this.employeeId);
    this.loading = false;

    if (result.statusCode == 200) {
      this.listCapPhatTaiSan = result.listCapPhatTaiSan;
      this.isShowButton = result.isShowButton;
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  capPhat() {
    this.router.navigate(['/asset/phan-bo-tai-san', {employeeId: this.employeeId}])
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}
