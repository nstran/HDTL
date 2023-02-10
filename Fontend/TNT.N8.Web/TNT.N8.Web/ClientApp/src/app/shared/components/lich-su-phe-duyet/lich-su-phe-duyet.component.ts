import { QuyTrinhService } from './../../../admin/services/quy-trinh.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-lich-su-phe-duyet',
  templateUrl: './lich-su-phe-duyet.component.html',
  styleUrls: ['./lich-su-phe-duyet.component.css']
})
export class LichSuPheDuyetComponent implements OnInit {
  objectId: string = null;
  doiTuongApDung: number = null;
  listLichSuPheDuyet: Array<any> = [];
  cols: Array<any> =  [];
  @ViewChild('myTable') myTable: Table;

  constructor(
    public quyTrinhService: QuyTrinhService,
    public messageService: MessageService,
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
  ) { 
    this.objectId = this.config.data.objectId;
    this.doiTuongApDung = this.config.data.doiTuongApDung;
  }

  ngOnInit(): void {
    this.initTable();

    this.quyTrinhService.getLichSuPheDuyet(this.objectId, this.doiTuongApDung).subscribe(res => {
      let result: any = res;

      if (result.statusCode == 200) {
        this.listLichSuPheDuyet = result.listLichSuPheDuyet;
      }
      else {
        this.showMessage('error', result.messageCode);
      }
    });
  }

  initTable() {
    this.cols = [
      { field: 'ngayTaoString', header: 'Ngày' },
      { field: 'tenTrangThai', header: 'Phê duyệt/Từ chối' },
      { field: 'nguoiPheDuyet', header: 'Người phê duyệt' },
      { field: 'tenDonVi', header: 'Đơn vị' },
      { field: 'lyDo', header: 'Lý do' },
    ];
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}
