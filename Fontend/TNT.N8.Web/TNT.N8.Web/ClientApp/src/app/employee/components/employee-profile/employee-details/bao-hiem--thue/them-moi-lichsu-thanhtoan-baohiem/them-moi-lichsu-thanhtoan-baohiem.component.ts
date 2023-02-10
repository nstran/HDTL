import { Component, OnInit } from '@angular/core';
import { MessageService} from 'primeng/api';
import { FormGroup, FormControl, Validators} from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';

import { EmployeeService } from './../../../../../services/employee.service';
import { DataService } from './../../../../../../shared/services/data.service';
import { FormatDateService } from './../../../../../../shared/services/formatDate.services';

import { LichSuThanhToanBaoHiemModel } from './../../../../../models/lich-su-thanh-toan-bao-hiem.model';

@Component({
  selector: 'app-them-moi-lichsu-thanhtoan-baohiem',
  templateUrl: './them-moi-lichsu-thanhtoan-baohiem.component.html',
  styleUrls: ['./them-moi-lichsu-thanhtoan-baohiem.component.css']
})
export class ThemMoiLichsuThanhtoanBaohiemComponent implements OnInit {
  loading: boolean = false;
  listLoaiBaoHiem = [];
  employeeId: any;
  isAdd: boolean = false;
  lichSuThanhToanBaoHiem: any;

  submitted: boolean = false;
  lichSuThanhToanForm: FormGroup;

  error: any = {
    ngayThanhToan: '',
    loaiBaoHiem: '',
    soTienThanhToan: ''
  }

  constructor(
    public messageService: MessageService,
    private employeeService: EmployeeService,
    public data: DynamicDialogConfig,
    private ref: DynamicDialogRef,
    private dataService: DataService,
    private formatDateService: FormatDateService
  ) { }

  ngOnInit(): void {
    this.listLoaiBaoHiem = this.data.data.listLoaiBaoHiem;
    this.employeeId = this.data.data.employeeId;
    this.isAdd = this.data.data.isAdd;
    this.lichSuThanhToanBaoHiem = this.data.data.lichSuThanhToanBaoHiem;

    this.initForm();
    if(!this.isAdd) {
      this.setForm();
    }
  }

  initForm() {
    this.lichSuThanhToanForm = new FormGroup({
      ngayThanhToan: new FormControl('', [Validators.required]),
      loaiBaoHiem: new FormControl('', [Validators.required]),
      soTienThanhToan: new FormControl(0, [Validators.required, Validators.min(1)]),
      ghiChu: new FormControl(''),
    });
  }

  setForm() {
    this.lichSuThanhToanForm.setValue({
      ngayThanhToan: this.lichSuThanhToanBaoHiem?.ngayThanhToan ? new Date(this.lichSuThanhToanBaoHiem?.ngayThanhToan) : null,
      loaiBaoHiem: this.lichSuThanhToanBaoHiem?.loaiBaoHiem ? this.listLoaiBaoHiem.find(x => x.value == this.lichSuThanhToanBaoHiem.loaiBaoHiem) : null,
      soTienThanhToan: this.lichSuThanhToanBaoHiem?.soTienThanhToan,
      ghiChu: this.lichSuThanhToanBaoHiem.ghiChu
    })
  }

  async save() {
    this.submitted = true;
    if (!this.lichSuThanhToanForm.valid) {
      if (this.lichSuThanhToanForm.get('ngayThanhToan').errors?.required) {
        this.error['ngayThanhToan'] = 'Không được để trống';
      }

      if (this.lichSuThanhToanForm.get('loaiBaoHiem').errors?.required) {
        this.error['loaiBaoHiem'] = 'Không được để trống';
      }

      if (this.lichSuThanhToanForm.get('soTienThanhToan').errors?.required) {
        this.error['soTienThanhToan'] = 'Không được để trống';
      } else if(this.lichSuThanhToanForm.get('soTienThanhToan').errors?.min) {
        this.error['soTienThanhToan'] = 'Số tiền thanh toán phải lớn hơn 0';
      }

      return;
    }


    let formData = this.lichSuThanhToanForm.value;

    let dataInput: LichSuThanhToanBaoHiemModel = {
      ngayThanhToan: formData.ngayThanhToan ? this.formatDateService.convertToUTCTime(formData.ngayThanhToan) : null,
      loaiBaoHiem: formData.loaiBaoHiem.value,
      soTienThanhToan: formData.soTienThanhToan,
      ghiChu: formData.ghiChu,
      employeeId: this.employeeId
    };

    if(!this.isAdd) {
      dataInput = {...dataInput, lichSuThanhToanBaoHiemId: this.lichSuThanhToanBaoHiem.lichSuThanhToanBaoHiemId}
    }

    this.loading = true;
    let result: any = await this.employeeService.createOrUpdateLichSuThanhToanBaoHiem(dataInput);
    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }

    this.showMessage('success', result.messageCode);
    this.cancel();
  }

  cancel() {
    this.ref.close();
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}
