import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { ConfirmationService } from 'primeng/api';

import { TaiLieuNhanVienModel } from './../../../../../models/tai-lieu-nhan-vien.model';

import { EmployeeService } from './../../../../../services/employee.service';
import { DataService } from './../../../../../../shared/services/data.service';

import { ThemMoiTaiLieuComponent } from '../them-moi-tai-lieu/them-moi-tai-lieu.component';

@Component({
  selector: 'app-tai-lieu',
  templateUrl: './tai-lieu.component.html',
  styleUrls: ['./tai-lieu.component.css']
})
export class TaiLieuComponent implements OnInit {
  loading: boolean = false;
  @Input() actionEdit: boolean;
  @Input() employeeId: string;

  cols: Array<any> = [];
  listTaiLieu: Array<TaiLieuNhanVienModel> = [];

  isShowButtonTuChoi: boolean = false;
  isShowButtonXacNhan: boolean = false;
  isShowButtonYeuCauXacNhan: boolean = false;
  isShowButtonThemMoi: boolean = false;
  isShowButtonSua: boolean = false;
  isShowButtonXoa: boolean = false;

  displayTuChoi: boolean = false;
  lyDoTuChoi = null;

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
    this.getListTaiLieuNhanVien();
  }

  initTable() {
    this.cols = [
      { field: 'stt', header: '#', textAlign: 'center', colWith: '3vw' },
      { field: 'tenTaiLieu', header: 'Tài liệu', textAlign: 'left', colWith: '' },
      { field: 'ngayNop', header: 'Ngày nộp', textAlign: 'center', colWith: '8vw' },
      { field: 'ngayHen', header: 'Ngày hẹn nộp', textAlign: 'center', colWith: '8vw' },
      { field: 'action', header: 'Thao tác', textAlign: 'center', colWith: '150px' }
    ];
  }

  async getListTaiLieuNhanVien() {
    this.loading = true;
    let result: any = await this.employeeService.getListTaiLieuNhanVien(this.employeeId);
    this.loading = false;

    if (result.statusCode == 200) {
      this.listTaiLieu = result.listTaiLieu;
      this.isShowButtonTuChoi = result.isShowButtonTuChoi;
      this.isShowButtonXacNhan = result.isShowButtonXacNhan;
      this.isShowButtonYeuCauXacNhan = result.isShowButtonYeuCauXacNhan;
      this.isShowButtonThemMoi = result.isShowButtonThemMoi;
      this.isShowButtonSua = result.isShowButtonSua;
      this.isShowButtonXoa = result.isShowButtonXoa;
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  addTaiLieu(rowData = null) {
    let isAdd = rowData.taiLieuNhanVienId ? false : true;
    let ref = this.dialogService.open(ThemMoiTaiLieuComponent, {
      data: {
        isAdd: isAdd,
        employeeId: this.employeeId,
        taiLieu: rowData
      },
      header: isAdd ? 'Thêm mới tài liệu' : 'Chỉnh sửa tài liệu',
      width: '690px',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "250px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      this.getListTaiLieuNhanVien();
    });
  }

  deleteTaiLieu(data) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa, dữ liệu sẽ không thể hoàn tác?',
      accept: async () => {
        try {
          this.loading = true;
          let result: any = await this.employeeService.deleteTaiLieuNhanVienById(data.taiLieuNhanVienId);
          this.loading = false;

          if (result.statusCode != 200) {
            this.showMessage('error', result.messageCode);
            return;
          }
          this.showMessage('success', "Xóa dữ liệu thành công");
          this.getListTaiLieuNhanVien();
          this.dataService.changeMessage("Update success"); //thay đổi message để call lại api getListNote trong component NoteTimeline
        } catch (error) {
          this.showMessage('error', error);
        }
      }
    });
  }

  tuChoi() {
    this.displayTuChoi = true;
  }

  async saveTuChoi() {
    if(!this.lyDoTuChoi || !this.lyDoTuChoi.trim()) {
      this.showMessage('error', 'Vui lòng nhập lý do từ chối');
      return;
    }

    try {
      this.loading = true;
      let result: any = await this.employeeService.tuChoiCheckListTaiLieu(this.employeeId, this.lyDoTuChoi.trim());
      this.loading = false;

      if (result.statusCode != 200) {
        this.showMessage('error', result.messageCode);
        return;
      }
      this.showMessage('success', result.messageCode);
      this.getListTaiLieuNhanVien();
      this.dataService.changeMessage("Update success"); //thay đổi message để call lại api getListNote trong component NoteTimeline
    } catch (error) {
      this.showMessage('error', error);
    }

    this.displayTuChoi = false;
  }

  async xacNhan() {
    try {
      this.loading = true;
      let result: any = await this.employeeService.xacNhanCheckListTaiLieu(this.employeeId);
      this.loading = false;

      if (result.statusCode != 200) {
        this.showMessage('error', result.messageCode);
        return;
      }
      this.showMessage('success', result.messageCode);
      this.getListTaiLieuNhanVien();
      this.dataService.changeMessage("Update success"); //thay đổi message để call lại api getListNote trong component NoteTimeline
    } catch (error) {
      this.showMessage('error', error);
    }
  }

  async yeuCauXacNhan() {
    try {
      this.loading = true;
      let result: any = await this.employeeService.yeuCauXacNhanCheckListTaiLieu(this.employeeId);
      this.loading = false;

      if (result.statusCode != 200) {
        this.showMessage('error', result.messageCode);
        return;
      }
      this.showMessage('success', result.messageCode);
      this.getListTaiLieuNhanVien();
      this.dataService.changeMessage("Update success"); //thay đổi message để call lại api getListNote trong component NoteTimeline
    } catch (error) {
      this.showMessage('error', error);
    }
  }

  cancel() {
    this.displayTuChoi = false;
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}
