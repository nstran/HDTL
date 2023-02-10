import { Component, OnInit } from '@angular/core';
import { MessageService} from 'primeng/api';
import { FormGroup, FormControl, Validators} from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';

import { EmployeeService } from './../../../../../services/employee.service';
import { FormatDateService } from './../../../../../../shared/services/formatDate.services';
import { DataService } from './../../../../../../shared/services/data.service';

import { TaiLieuNhanVienModel } from './../../../../../models/tai-lieu-nhan-vien.model';

@Component({
  selector: 'app-them-moi-tai-lieu',
  templateUrl: './them-moi-tai-lieu.component.html',
  styleUrls: ['./them-moi-tai-lieu.component.css']
})
export class ThemMoiTaiLieuComponent implements OnInit {
  loading: boolean = false;
  employeeId: any;
  isAdd: boolean = false;
  taiLieu: any;

  submitted: boolean = false;
  taiLieuForm: FormGroup;

  error: any = {
    tenTaiLieu: '',
    ngayNop: '',
    ngayHen: ''
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
    this.taiLieu = this.data.data.taiLieu;

    this.initForm();
    this.setForm();
  }

  initForm() {
    this.taiLieuForm = new FormGroup({
      tenTaiLieu: new FormControl({value: '', disabled: true}),
      ngayNop: new FormControl(''),
      ngayHen: new FormControl('')
    });
  }

  setForm() {
    this.taiLieuForm.setValue({
      tenTaiLieu: this.taiLieu?.tenTaiLieu,
      ngayNop: this.taiLieu?.ngayNop ? new Date(this.taiLieu?.ngayNop) : null,
      ngayHen: this.taiLieu?.ngayHen ? new Date(this.taiLieu?.ngayHen) : null
    });
  }

  async save() {
    this.submitted = true;
    if (!this.taiLieuForm.valid) {
      return;
    }

    let formData = this.taiLieuForm.value;

    let dataInput: TaiLieuNhanVienModel = {
      ngayNop: formData.ngayNop ? this.formatDateService.convertToUTCTime(formData.ngayNop) : null,
      ngayHen: formData.ngayHen ? this.formatDateService.convertToUTCTime(formData.ngayHen) : null,
      cauHinhChecklistId: this.taiLieu.cauHinhChecklistId,
      employeeId: this.employeeId
    };

    if(!this.isAdd) {
      dataInput = {...dataInput, taiLieuNhanVienId: this.taiLieu.taiLieuNhanVienId}
    }

    this.loading = true;
    let result: any;
    if(this.isAdd) {
      result = await this.employeeService.createTaiLieuNhanVien(dataInput);
    } else {
      result = await this.employeeService.updateTaiLieuNhanVien(dataInput);
    }
    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }

    this.showMessage('success', this.isAdd ? 'Thêm mới tài liệu thành công' : 'Chỉnh sửa tài liệu thành công');
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
