import { Component, OnInit } from '@angular/core';
import { MessageService} from 'primeng/api';
import { FormGroup, FormControl, Validators} from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';

import { EmployeeService } from './../../../services/employee.service';
import { FormatDateService } from './../../../../shared/services/formatDate.services';

@Component({
  selector: 'app-them-moi-cauhinh-checklist',
  templateUrl: './them-moi-cauhinh-checklist.component.html',
  styleUrls: ['./them-moi-cauhinh-checklist.component.css']
})
export class ThemMoiCauhinhChecklistComponent implements OnInit {
  loading: boolean = false;
  isAdd: boolean = false;
  taiLieu: any;

  submitted: boolean = false;
  taiLieuForm: FormGroup;

  error: any = {
    tenTaiLieu: ''
  }

  constructor(
    public messageService: MessageService,
    private employeeService: EmployeeService,
    public data: DynamicDialogConfig,
    private ref: DynamicDialogRef,
    private formatDateService: FormatDateService
  ) { }

  ngOnInit(): void {
    this.isAdd = this.data.data.isAdd;
    this.taiLieu = this.data.data.taiLieu;

    this.initForm();
    if(!this.isAdd) {
      this.setForm();
    }
  }

  initForm() {
    this.taiLieuForm = new FormGroup({
      tenTaiLieu: new FormControl('', [Validators.required, forbiddenSpaceText]),
      active: new FormControl(false),
    });
  }

  setForm() {
    this.taiLieuForm.setValue({
      tenTaiLieu: this.taiLieu?.tenTaiLieu,
      active: this.taiLieu?.active,
    });
  }

  async save() {
    this.submitted = true;
    if (!this.taiLieuForm.valid) {
      if (this.taiLieuForm.get('tenTaiLieu').errors?.required || this.taiLieuForm.get('tenTaiLieu').errors?.forbiddenSpaceText) {
        this.error['tenTaiLieu'] = 'Không được để trống';
      }

      return;
    }

    let formData = this.taiLieuForm.value;

    let dataInput = {
      cauHinhChecklistId: (!this.isAdd) ? this.taiLieu.cauHinhChecklistId : null,
      tenTaiLieu: formData.tenTaiLieu.trim(),
      active: formData.active
    };

    this.loading = true;
    let result: any = await this.employeeService.createOrUpdateCauHinhChecklist(dataInput);
    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }

    this.showMessage('success', this.isAdd ? 'Thêm mới tài liệu thành công' : 'Chỉnh sửa tài liệu thành công');
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
