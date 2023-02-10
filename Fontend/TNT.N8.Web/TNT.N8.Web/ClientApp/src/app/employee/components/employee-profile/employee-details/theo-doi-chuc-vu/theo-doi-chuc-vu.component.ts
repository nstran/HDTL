import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Table } from 'primeng/table';
import { MessageService } from 'primeng/api';

import { EmployeeService } from './../../../../services/employee.service';

@Component({
  selector: 'app-theo-doi-chuc-vu',
  templateUrl: './theo-doi-chuc-vu.component.html',
  styleUrls: ['./theo-doi-chuc-vu.component.css']
})
export class TheoDoiChucVuComponent implements OnInit {
  loading: boolean = false;
  @Input() actionEdit: boolean;
  @Input() employeeId: string;

  cols: Array<any> = [];
  listChucVu: Array<any> = [];

  @ViewChild('myTable') myTable: Table;
  constructor(
    private messageService: MessageService,
    private employeeService: EmployeeService,
  ) { }

  ngOnInit(): void {
    this.initTable();
    this.getLichSuThayDoiChucVu();
  }

  initTable() {
    this.cols = [
      { field: 'stt', header: '#', textAlign: 'center', colWith: '3vw' },
      { field: 'ngayBatDau', header: 'Ngày bắt đầu', textAlign: 'center', colWith: '8vw' },
      { field: 'ngayKetThuc', header: 'Ngày kết thúc', textAlign: 'center', colWith: '8vw' },
      { field: 'positionName', header: 'Chức vụ', textAlign: 'left', colWith: '8vw' }
    ];
  }

  async getLichSuThayDoiChucVu() {
    this.loading = true;
    let result: any = await this.employeeService.getLichSuThayDoiChucVu(this.employeeId);
    this.loading = false;

    if (result.statusCode == 200) {
      this.listChucVu = result.listData;
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}
