import { Component, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';

import { EmployeeService } from './../../../../../services/employee.service';
import { FormatDateService } from './../../../../../../shared/services/formatDate.services';
import { DataService } from './../../../../../../shared/services/data.service';

import { ThongTinGiaDinhModel } from './../../../../../models/thong-tin-gia-dinh.model';

@Component({
  selector: 'app-them-moi-gia-dinh',
  templateUrl: './them-moi-gia-dinh.component.html',
  styleUrls: ['./them-moi-gia-dinh.component.css']
})
export class ThemMoiGiaDinhComponent implements OnInit {
  loading: boolean = false;
  employeeId: any;
  isAdd: boolean = false;
  listQuanHe = [];
  thanhVien: any;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));

  submitted: boolean = false;
  thanhVienForm: FormGroup;

  maxDate = new Date(Date.now() - (1000 * 60 * 60 * 24));

  error: any = {
    quanHe: '',
    fullName: '',
    dateOfBirth: '',
    phone: '',
    email: '',
    phuThuocTuNgay: ''
  }

  constructor(
    public messageService: MessageService,
    private employeeService: EmployeeService,
    public data: DynamicDialogConfig,
    private ref: DynamicDialogRef,
    private formatDateService: FormatDateService,
    private dataService: DataService,
  ) { }

  ngOnInit(): void {
    this.employeeId = this.data.data.employeeId;
    this.isAdd = this.data.data.isAdd;
    this.listQuanHe = this.data.data.listQuanHe;
    this.thanhVien = this.data.data.thanhVien;

    this.initForm();
    if (!this.isAdd) {
      this.setForm();
    }
  }

  initForm() {
    let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
    this.thanhVienForm = new FormGroup({
      quanHe: new FormControl('', [Validators.required]),
      fullName: new FormControl('', [Validators.required, forbiddenSpaceText]),
      dateOfBirth: new FormControl('', [Validators.required]),
      phone: new FormControl('', [Validators.pattern(this.getPhonePattern())]),
      email: new FormControl('', [Validators.pattern(emailPattern)]),
      phuThuoc: new FormControl(false),
      phuThuocTuNgay: new FormControl({ value: null, disabled: true }),
      phuThuocDenNgay: new FormControl({ value: null, disabled: true }),
    });
  }

  setForm() {
    this.thanhVienForm.setValue({
      quanHe: this.thanhVien ?.quanHeId ? this.listQuanHe.find(x => x.categoryId == this.thanhVien.quanHeId) : null,
      fullName: this.thanhVien ?.fullName,
      dateOfBirth: this.thanhVien ?.dateOfBirth ? new Date(this.thanhVien ?.dateOfBirth) : null,
      phone: this.thanhVien ?.phone ?? null,
      email: this.thanhVien ?.email ?? null,
      phuThuoc: this.thanhVien ?.phuThuoc ?? false,
      phuThuocTuNgay: this.thanhVien ?.phuThuocTuNgay ? new Date(this.thanhVien ?.phuThuocTuNgay) : null,
      phuThuocDenNgay: this.thanhVien ?.phuThuocDenNgay ? new Date(this.thanhVien ?.phuThuocDenNgay) : null
    });

    this.changePhuThuoc();
  }

  changePhuThuoc() {
    let phuThuoc = this.thanhVienForm.get('phuThuoc').value;
    if (phuThuoc) {
      this.thanhVienForm.get('phuThuocTuNgay').setValidators([Validators.required]);
      this.thanhVienForm.get('phuThuocTuNgay').enable();
      this.thanhVienForm.get('phuThuocDenNgay').enable();
    } else {
      this.thanhVienForm.get('phuThuocTuNgay').setValidators([]);
      this.thanhVienForm.get('phuThuocTuNgay').disable();
      this.thanhVienForm.get('phuThuocDenNgay').disable();
    }

    this.thanhVienForm.get('phuThuocTuNgay').updateValueAndValidity();
  }

  async save() {
    this.submitted = true;
    if (!this.thanhVienForm.valid) {
      if (this.thanhVienForm.get('quanHe').errors ?.required) {
        this.error['quanHe'] = 'Không được để trống';
      }

      if (this.thanhVienForm.get('fullName').errors ?.required || this.thanhVienForm.get('fullName').errors ?.forbiddenSpaceText) {
        this.error['fullName'] = 'Không được để trống';
      }

      if (this.thanhVienForm.get('dateOfBirth').errors ?.required) {
        this.error['dateOfBirth'] = 'Không được để trống';
      }

      if (this.thanhVienForm.get('phone').errors ?.pattern) {
        this.error['phone'] = 'Không đúng định dạng';
      }

      if (this.thanhVienForm.get('email').errors ?.pattern) {
        this.error['email'] = 'Không đúng định dạng';
      }

      if (this.thanhVienForm.get('phuThuocTuNgay').errors ?.required) {
        this.error['phuThuocTuNgay'] = 'Không được để trống';
      }

      return;
    }

    let phuThuocTuNgay = this.thanhVienForm.get('phuThuocTuNgay').value ? (this.thanhVienForm.get('phuThuocTuNgay').value as Date).getTime() : null;
    let phuThuocDenNgay = this.thanhVienForm.get('phuThuocDenNgay').value ? (this.thanhVienForm.get('phuThuocDenNgay').value as Date).getTime() : null;

    if (phuThuocDenNgay && phuThuocDenNgay <= phuThuocTuNgay) {
      this.showMessage('error', 'Yêu cầu Phụ thuộc từ ngày phải lớn hơn Phụ thuộc đến ngày');
      return;
    }

    let formData = this.thanhVienForm.getRawValue();

    let dataInput: ThongTinGiaDinhModel = {
      quanHeId: formData.quanHe.categoryId,
      fullName: formData.fullName.trim(),
      dateOfBirth: formData.dateOfBirth ? this.formatDateService.convertToUTCTime(formData.dateOfBirth) : null,
      phone: formData ?.phone,
      email: formData ?.email,
      employeeId: this.employeeId,
      phuThuoc: formData ?.phuThuoc,
      phuThuocTuNgay: formData.phuThuocTuNgay ? this.formatDateService.convertToUTCTime(formData.phuThuocTuNgay) : null,
      phuThuocDenNgay: formData.phuThuocDenNgay ? this.formatDateService.convertToUTCTime(formData.phuThuocDenNgay) : null,
    };

    if (!this.isAdd) {
      dataInput = { ...dataInput, contactId: this.thanhVien.contactId }
    }

    this.loading = true;
    let result: any = await this.employeeService.createOrUpdateThongTinGiaDinh(dataInput);
    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }

    this.showMessage('success', this.isAdd ? 'Thêm mới thông tin gia đình thành công' : 'Chỉnh sửa thông tin gia đình thành công');
    this.cancel();
    this.dataService.changeMessage("Update success"); //thay đổi message để call lại api getListNote trong component NoteTimeline
  }

  cancel() {
    this.ref.close();
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

}

function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
      }
    }
  }
  return null;
}
