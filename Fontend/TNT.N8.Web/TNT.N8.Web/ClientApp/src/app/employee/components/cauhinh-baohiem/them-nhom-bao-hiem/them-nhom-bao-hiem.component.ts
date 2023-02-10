import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, FormArray } from "@angular/forms";
import { MessageService } from "primeng/api";
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';

import { EmployeeService } from "../../../services/employee.service";
import { ValidaytorsService } from './../../../../shared/services/validaytors.service';
import { CommonService } from '../../../../shared/services/common.service';

import { CauHinhBaoHiemLoftCareModel, NhomBaoHiemLoftCareModel, ChucVuBaoHiemLoftCareModel } from '../../../models/cauhinh-baohiem.model';

@Component({
  selector: 'app-them-nhom-bao-hiem',
  templateUrl: './them-nhom-bao-hiem.component.html',
  styleUrls: ['./them-nhom-bao-hiem.component.css']
})
export class ThemNhomBaoHiemComponent implements OnInit {
  loading: boolean = false;
  submitted: boolean = false;

  isAdd: boolean = false;
  cauHinhBaoHiemLoftCareId: any;
  nhomBaoHiem: NhomBaoHiemLoftCareModel;
  listPosition = [];
  listDataChucVu = [];
  colsChucVu = [];

  error = null;
  nhomBaoHiemForm: FormGroup;
  constructor(
    private employeeService: EmployeeService,
    private messageService: MessageService,
    private validaytorsService: ValidaytorsService,
    private ref: DynamicDialogRef,
    public data: DynamicDialogConfig,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.isAdd = this.data.data.isAdd;
    this.listPosition = this.data.data.listPosition;
    this.cauHinhBaoHiemLoftCareId = this.data.data ?.cauHinhBaoHiemLoftCareId;
    this.nhomBaoHiem = this.data.data ?.nhomBaoHiem ?? new NhomBaoHiemLoftCareModel();

    this.colsChucVu = [
      { field: 'positionName', header: 'Chức vụ', textAlign: 'left', width: '40%' },
      { field: 'soNamKinhNghiem', header: 'Số năm kinh nghiệm', textAlign: 'center', width: '40%' },
      { field: 'action', header: 'Thao tác', textAlign: 'center', width: '20%' },
    ];

    this.nhomBaoHiemForm = new FormGroup({
      tenNhom: new FormControl(null, [Validators.required, this.validaytorsService.forbiddenSpaceText]),
      chucVu: new FormControl(null)
    });

    if (!this.isAdd) {
      this.mapDataToFormBaoHiemLoftCare();
    }

  }

  mapDataToFormBaoHiemLoftCare() {
    if (!this.isAdd) {
      this.nhomBaoHiemForm.setValue({
        tenNhom: this.nhomBaoHiem.tenNhom,
        chucVu: null
      });
    }
    this.nhomBaoHiem.listChucVuBaoHiemLoftCare.forEach(cv => {
      let chucVu = this.listPosition.find(x => x.positionId == cv.positionId);
      cv.positionName = chucVu ?.positionName;
    });
  }

  themChucVu() {
    let selectedPosition = this.nhomBaoHiemForm.controls.chucVu.value;
    if (selectedPosition) {
      let listChucVuBaoHiemLoftCare = this.nhomBaoHiem.listChucVuBaoHiemLoftCare;

      //Chỉ thêm những chức vụ chưa được thêm
      if (!listChucVuBaoHiemLoftCare.map(y => y.positionId).includes(selectedPosition.positionId)) {
        let chucVu = new ChucVuBaoHiemLoftCareModel();
        chucVu.positionId = selectedPosition.positionId;
        chucVu.positionName = selectedPosition.positionName;
        chucVu.soNamKinhNghiem = 0;

        listChucVuBaoHiemLoftCare.push(chucVu);
      }
    }

  }

  xoaChucVu(rowData: any) {
    let listChucVuBaoHiemLoftCare = this.nhomBaoHiem.listChucVuBaoHiemLoftCare;
    this.nhomBaoHiem.listChucVuBaoHiemLoftCare = listChucVuBaoHiemLoftCare.filter(x => x.positionId != rowData.positionId);
  }

  async save() {
    this.submitted = true;
    if (!this.nhomBaoHiemForm.valid) {
      if (this.nhomBaoHiemForm.get('tenNhom').errors ?.required || this.nhomBaoHiemForm.get('tenNhom').errors ?.forbiddenSpaceText) {
        this.error = 'Không được để trống';
      }
      return;
    }

    this.nhomBaoHiem.listChucVuBaoHiemLoftCare.forEach(item => {
      item.soNamKinhNghiem = this.commonService.convertStringToNumber(item.soNamKinhNghiem.toString());
    });

    if (!this.nhomBaoHiem.listChucVuBaoHiemLoftCare.length) {
      this.showMessage('warn', 'Bạn chưa thêm chức vụ được hưởng');
      return;
    }

    let dataInput: CauHinhBaoHiemLoftCareModel = {
      nhomBaoHiemLoftCare: {
        nhomBaoHiemLoftCareId: this.nhomBaoHiem ?.nhomBaoHiemLoftCareId ?? null,
        cauHinhBaoHiemLoftCareId: this.cauHinhBaoHiemLoftCareId,
        tenNhom: this.nhomBaoHiemForm.get('tenNhom').value.trim(),
        listChucVuBaoHiemLoftCare: this.nhomBaoHiem.listChucVuBaoHiemLoftCare
      }
    };

    this.loading = true;
    let result: any = await this.employeeService.createOrUpdateCauHinhBaoHiemLoftCare(2, dataInput);

    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }

    this.showMessage('success', result.messageCode);
    this.cancel();
    this.ref.close({ status: true });
  }

  cancel() {
    this.ref.close();
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}
