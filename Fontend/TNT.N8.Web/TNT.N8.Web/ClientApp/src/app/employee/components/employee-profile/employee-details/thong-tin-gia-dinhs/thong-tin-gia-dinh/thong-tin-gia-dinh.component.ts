import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { ConfirmationService } from 'primeng/api';

import { ThongTinGiaDinhModel } from './../../../../../models/thong-tin-gia-dinh.model';

import { EmployeeService } from './../../../../../services/employee.service';
import { DataService } from './../../../../../../shared/services/data.service';

import { ThemMoiGiaDinhComponent } from '../them-moi-gia-dinh/them-moi-gia-dinh.component';

@Component({
  selector: 'app-thong-tin-gia-dinh',
  templateUrl: './thong-tin-gia-dinh.component.html',
  styleUrls: ['./thong-tin-gia-dinh.component.css']
})
export class ThongTinGiaDinhComponent implements OnInit {
  loading: boolean = false;
  @Input() actionEdit: boolean;
  @Input() employeeId: string;

  isManager: boolean = false;
  listQuanHe = [];

  cols: Array<any> = [];
  listThanhVienGiaDinh: Array<ThongTinGiaDinhModel> = [];

  @ViewChild('myTable') myTable: Table;
  constructor(
    public dialogService: DialogService,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    public confirmationService: ConfirmationService,
    private dataService: DataService,
  ) { }

  ngOnInit(): void {
    this.initTable();
    this.getThongTinGiaDinh();
  }

  initTable() {
    this.cols = [
      { field: 'stt', header: '#', textAlign: 'center', colWith: '3vw' },
      { field: 'tenQuanHe', header: 'Quan hệ', textAlign: 'left', colWith: '8vw' },
      { field: 'fullName', header: 'Họ và tên', textAlign: 'left', colWith: '' },
      { field: 'dateOfBirth', header: 'Ngày sinh', textAlign: 'center', colWith: '8vw' },
      { field: 'phone', header: 'Điện thoại', textAlign: 'left', colWith: '8vw' },
      { field: 'email', header: 'Email', textAlign: 'left', colWith: '15vw' },
      { field: 'phuThuoc', header: 'Phụ thuộc', textAlign: 'center', colWith: '8vw' },
      { field: 'action', header: 'Thao tác', textAlign: 'center', colWith: '100px' }
    ];
  }

  async getThongTinGiaDinh() {
    this.loading = true;
    let result: any = await this.employeeService.getThongTinGiaDinh(this.employeeId);
    this.loading = false;

    if (result.statusCode == 200) {
      this.listThanhVienGiaDinh = result.listThanhVienGiaDinh;
      this.isManager = result.isManager;
      this.listQuanHe = result.listQuanHe;
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  addThongTinGiaDinh(rowData = null) {
    let isAdd: boolean = true;
    if (rowData) {
      isAdd = false;
    }
    let ref = this.dialogService.open(ThemMoiGiaDinhComponent, {
      data: {
        isAdd: isAdd,
        employeeId: this.employeeId,
        listQuanHe: this.listQuanHe,
        thanhVien: rowData
      },
      header: isAdd ? 'Thêm mới thông tin gia đình' : 'Chỉnh sửa thông tin gia đình',
      width: '690px',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "250px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      this.getThongTinGiaDinh();
    });
  }

  deleteThongTinGiaDinh(data) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa, dữ liệu sẽ không thể hoàn tác?',
      accept: async () => {
        try {
          this.loading = true;
          let result: any = await this.employeeService.deleteThongTinGiaDinhById(data.contactId);
          this.loading = false;

          if (result.statusCode != 200) {
            this.showMessage('error', result.messageCode);
            return;
          }
          this.showMessage('success', "Xóa dữ liệu thành công");
          this.getThongTinGiaDinh();
          this.dataService.changeMessage("Update success"); //thay đổi message để call lại api getListNote trong component NoteTimeline
        } catch (error) {
          this.showMessage('error', error);
        }
      }
    });
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}
