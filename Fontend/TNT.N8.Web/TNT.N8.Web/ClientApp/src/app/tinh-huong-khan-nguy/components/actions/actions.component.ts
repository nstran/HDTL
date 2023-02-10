import { CustomerService } from './../../../customer/services/customer.service';
import { Component, OnInit } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-actions',
  templateUrl: './actions.component.html',
  styleUrls: ['./actions.component.css'],
  providers: [MessageService, ConfirmationService]
})
export class ActionsComponent implements OnInit {
  loading: boolean = false;
  listPhone: string = null;
  noiDung: string = null;
  listData: Array<any> = [];
  listDataDetail: Array<any> = [];
  cols1: Array<any> = [];
  cols2: Array<any> = [];
  display: boolean = false;

  constructor(
    private customerService: CustomerService,
    private messageService: MessageService,
  ) { }

  ngOnInit(): void {
    this.cols1 = [
      { field: 'stt', header: 'STT', textAlign: 'left', width: '10%' },
      { field: 'noiDung', header: 'Nội dung', textAlign: 'left', width: '30%' },
      { field: 'thoiDiemKichHoat', header: 'Thời điểm kích hoạt', textAlign: 'left', width: '20%' },
      { field: 'soLuongNguoi', header: 'Số lượng người', textAlign: 'left', width: '30%' },
      { field: 'action', header: 'Thao tác', textAlign: 'right', width: '10%' },
    ];

    this.cols2 = [
      { field: 'stt', header: 'STT', textAlign: 'left', width: '10%' },
      { field: 'sdt', header: 'Số điện thoại', textAlign: 'left', width: '50%' },
      // { field: 'noiDung', header: 'Nội dung', textAlign: 'left', width: '30%' },
      { field: 'phanHoi', header: 'Phản hồi', textAlign: 'right', width: '10%' },
    ];

    this.getList();
  }

  async getList() {
    this.loading = true;
    let result: any = await this.customerService.getListTinhHuong();
    this.loading = false;
    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }
   
    this.listData = result.listData;
    this.listData.forEach((item, index) => {
      item.stt = index + 1;
    });
  }

  async kichHoat() {
    if (this.listPhone == null) {
      this.showMessage('warn', 'Bạn cần nhập tổ khẩn nguy');
      return;
    }

    this.loading = true;
    let result: any = await this.customerService.kichHoatTinhHuong(this.listPhone, this.noiDung);
    this.loading = false;
    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }

    this.showMessage('success', result.messageCode);
    this.getList();
  }

  async view(data: any) {
    this.listDataDetail = [];
    
    this.loading = true;
    let result: any = await this.customerService.getChiTietTinhHuong(data.id);
    this.loading = false;
    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }

    this.listDataDetail = result.listDataDetail;
    this.listDataDetail.forEach((item, index) => {
      item.stt = index + 1;
    });
    this.display = true;
  }

  showMessage(severity: string, detail: string) {
    this.messageService.add({severity: severity, summary: 'Thông báo:', detail: detail});
  }
}
