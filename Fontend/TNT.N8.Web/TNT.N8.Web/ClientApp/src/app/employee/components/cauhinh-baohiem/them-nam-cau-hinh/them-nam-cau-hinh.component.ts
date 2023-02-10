import { Component, OnInit} from '@angular/core';
import { FormControl, Validators, FormGroup } from "@angular/forms";
import { MessageService } from "primeng/api";
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';

import { EmployeeService } from "../../../services/employee.service";
import { ValidaytorsService } from './../../../../shared/services/validaytors.service';

import { CauHinhBaoHiemLoftCareModel } from "../../../models/cauhinh-baohiem.model";

@Component({
  selector: 'app-them-nam-cau-hinh',
  templateUrl: './them-nam-cau-hinh.component.html',
  styleUrls: ['./them-nam-cau-hinh.component.css']
})
export class ThemNamCauHinhComponent implements OnInit {
  loading: boolean = false;
  submitted: boolean = false;

  isAdd: boolean = false;
  cauHinhBaoHiem: CauHinhBaoHiemLoftCareModel;

  error = null;
  cauHinhBaoHiemForm: FormGroup;

  constructor(
    private employeeService: EmployeeService,
    private messageService: MessageService,
    private validaytorsService: ValidaytorsService,
    private ref: DynamicDialogRef,
    public data: DynamicDialogConfig,
  ) { }

  ngOnInit(): void {
    this.isAdd = this.data.data.isAdd;
    this.cauHinhBaoHiem = this.data.data?.cauHinhBaoHiem;

    this.cauHinhBaoHiemForm = new FormGroup({
      namCauHinh: new FormControl(null, [Validators.required, Validators.pattern(this.validaytorsService.regex.number)])
    });

    if(!this.isAdd) {
      this.cauHinhBaoHiemForm.setValue({
        namCauHinh: this.cauHinhBaoHiem.namCauHinh
      });
    }
  }

  async save() {
    this.submitted = true;
    if (!this.cauHinhBaoHiemForm.valid) {
      if (this.cauHinhBaoHiemForm.get('namCauHinh').errors?.required) {
        this.error = 'Không được để trống';
      } else if(this.cauHinhBaoHiemForm.get('namCauHinh').errors?.pattern) {
        this.error = 'Không đúng định dạng';
      }
      return;
    }

    let dataInput: CauHinhBaoHiemLoftCareModel = {
      cauHinhBaoHiemLoftCareId: this.cauHinhBaoHiem?.cauHinhBaoHiemLoftCareId ?? null,
      namCauHinh: this.cauHinhBaoHiemForm.get('namCauHinh').value.trim()
    };

    this.loading = true;
    let result: any = await this.employeeService.createOrUpdateCauHinhBaoHiemLoftCare(1, dataInput);

    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }

    this.showMessage('success', this.isAdd ? 'Thêm mới năm cấu hình thành công' : 'Cập nhật năm cấu hình thành công');
    this.ref.close({status: true});
  }

  cancel() {
    this.ref.close();
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}
