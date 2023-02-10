import { Component, OnInit, ViewChild } from '@angular/core';
import { MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';

import { EmployeeService } from './../../../../../services/employee.service';

@Component({
  selector: 'app-chi-tiet-hop-dong',
  templateUrl: './chi-tiet-hop-dong.component.html',
  styleUrls: ['./chi-tiet-hop-dong.component.css']
})
export class ChiTietHopDongComponent implements OnInit {
  loading: boolean = false;
  employeeId: any;
  listSelectedHopDong: any = [];
  dataHopDong: any = [];

  
  listId: any = [];

  cols: Array<any> = [];
  listHopDong = [];

  @ViewChild('myTable') myTable: Table;
  constructor(
    private messageService: MessageService,
    private employeeService: EmployeeService,
    public data: DynamicDialogConfig,
    private ref: DynamicDialogRef,
  ) { }

  ngOnInit(): void {
    this.employeeId = this.data.data.employeeId;
    this.listSelectedHopDong = this.data.data.listSelectedHopDong;
    this.dataHopDong = this.data.data.dataHopDong;

    this.listId = this.listSelectedHopDong.map(x => x.hopDongNhanSuId);

    this.initTable();
    this.getLichSuHopDongNhanSu();
  }

  initTable() {
    this.cols = [
      { field: 'stt', header: '#', textAlign: 'center', colWith: '3vw' },
      { field: 'soHopDong', header: 'Số HĐ', textAlign: 'left', colWith: '8vw' },
      { field: 'ngayBatDau', header: 'Ngày bắt đầu', textAlign: 'center', colWith: '8vw' },
      { field: 'mucLuong', header: 'Mức lương', textAlign: 'right', colWith: '8vw' }
    ];
  }

  async getLichSuHopDongNhanSu() {
    this.loading = true;
    let result: any = await this.employeeService.getLichSuHopDongNhanSu(this.employeeId, this.listId);
    this.loading = false;

    if (result.statusCode == 200) {
      this.listHopDong = result.listData;
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  onChangeSelectedHopDong() {
    this.listId = this.listSelectedHopDong.map(x => x.hopDongNhanSuId);
    this.getLichSuHopDongNhanSu();
  }

  cancel() {
    this.ref.close();
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}
