import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, FormArray } from "@angular/forms";
import { MessageService } from "primeng/api";
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';

import { EmployeeService } from "../../../services/employee.service";
import { ValidaytorsService } from './../../../../shared/services/validaytors.service';
import { CommonService } from '../../../../shared/services/common.service';

import { CauHinhBaoHiemLoftCareModel, QuyenLoiBaoHiemLoftCareModel, MucHuongBaoHiemLoftCareModel } from '../../../models/cauhinh-baohiem.model';

@Component({
  selector: 'app-them-quyen-loi-bao-hiem',
  templateUrl: './them-quyen-loi-bao-hiem.component.html',
  styleUrls: ['./them-quyen-loi-bao-hiem.component.css']
})
export class ThemQuyenLoiBaoHiemComponent implements OnInit {
  loading: boolean = false;
  submitted: boolean = false;

  isAdd: boolean = false;
  nhomBaoHiemLoftCareId: any;
  quyenLoiBaoHiem: QuyenLoiBaoHiemLoftCareModel;
  listDonVi = [];
  listDoiTuong = [];
  cols = [];

  error = null;
  quyenLoiBaoHiemForm: FormGroup;
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
    this.listDonVi = this.data.data.listDonVi;
    this.listDoiTuong = this.data.data.listDoiTuong;
    this.nhomBaoHiemLoftCareId = this.data.data?.nhomBaoHiemLoftCareId;
    this.quyenLoiBaoHiem = this.data.data?.quyenLoiBaoHiem ?? new QuyenLoiBaoHiemLoftCareModel();

    this.cols = [
      { field: 'tenDoiTuong', header: 'Đối tượng', textAlign: 'left' },
      { field: 'mucHuong', header: 'Mức hưởng', textAlign: 'right' },
      { field: 'donVi', header: 'Đơn vị', textAlign: 'center' },
      { field: 'lePhi', header: 'Lệ phí (%)', textAlign: 'right' },
      { field: 'phiCoDinh', header: 'Phí cố định', textAlign: 'right' },
      { field: 'phiTheoLuong', header: 'Phí theo lương (%)', textAlign: 'right' },
      { field: 'mucGiam', header: 'Mức giảm (%)', textAlign: 'right' },
    ];

    this.quyenLoiBaoHiemForm = new FormGroup({
      tenQuyenLoi: new FormControl(null, [Validators.required, this.validaytorsService.forbiddenSpaceText])
    });

    if (this.isAdd) {
      let mucHuongBanThan = new MucHuongBaoHiemLoftCareModel();
      mucHuongBanThan.doiTuongHuong = 1;
      let banThan = this.listDoiTuong.find(x => x.value == mucHuongBanThan.doiTuongHuong);
      mucHuongBanThan.tenDoiTuong = banThan?.name;

      let mucHuongNguoiThan = new MucHuongBaoHiemLoftCareModel();
      mucHuongNguoiThan.doiTuongHuong = 2;
      let nguoiThan = this.listDoiTuong.find(x => x.value == mucHuongNguoiThan.doiTuongHuong);
      mucHuongNguoiThan.tenDoiTuong = nguoiThan?.name;

      this.quyenLoiBaoHiem.listMucHuongBaoHiemLoftCare.push(mucHuongBanThan);
      this.quyenLoiBaoHiem.listMucHuongBaoHiemLoftCare.push(mucHuongNguoiThan);
    }

    this.mapDataToFormBaoHiemLoftCare();

  }

  mapDataToFormBaoHiemLoftCare() {
    if(!this.isAdd) {
      this.quyenLoiBaoHiemForm.setValue({
        tenQuyenLoi: this.quyenLoiBaoHiem.tenQuyenLoi
      });
    }
    this.quyenLoiBaoHiem.listMucHuongBaoHiemLoftCare.forEach(mucHuong => {
      let donVi = this.listDonVi.find(x => x.value == mucHuong.donVi);
      mucHuong.selectedDonVi = donVi;
    });
  }

  async save() {
    this.submitted = true;
    if (!this.quyenLoiBaoHiemForm.valid) {
      if (this.quyenLoiBaoHiemForm.get('tenQuyenLoi').errors?.required || this.quyenLoiBaoHiemForm.get('tenQuyenLoi').errors?.forbiddenSpaceText) {
        this.error = 'Không được để trống';
      }
      return;
    }

    let isError = false;
    this.quyenLoiBaoHiem.listMucHuongBaoHiemLoftCare.forEach(item => {
      if (item.selectedDonVi) item.donVi = item.selectedDonVi.value;

      item.lePhi = this.commonService.convertStringToNumber(item.lePhi.toString());
      item.phiTheoLuong = this.commonService.convertStringToNumber(item.phiTheoLuong.toString());
      item.mucGiam = this.commonService.convertStringToNumber(item.mucGiam.toString());

      if (item.mucHuong == null || item.lePhi == null || item.phiCoDinh == null ||
        item.phiTheoLuong == null || item.mucGiam == null) {
        isError = true;
      }
    });

    if (isError) {
      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let isErrorPercent = false;
    this.quyenLoiBaoHiem.listMucHuongBaoHiemLoftCare.forEach(item => {
      if (item.lePhi > 100 || item.phiTheoLuong > 100 || item.mucGiam > 100) {
        isErrorPercent = true;
      }
    });

    if (isErrorPercent) {
      this.showMessage('warn', 'Số phần trăm không được vượt quá 100%');
      return;
    }

    let dataInput: CauHinhBaoHiemLoftCareModel = {
      quyenLoiBaoHiemLoftCare: {
        quyenLoiBaoHiemLoftCareId: this.quyenLoiBaoHiem?.quyenLoiBaoHiemLoftCareId ?? null,
        nhomBaoHiemLoftCareId: this.nhomBaoHiemLoftCareId,
        tenQuyenLoi: this.quyenLoiBaoHiemForm.get('tenQuyenLoi').value.trim(),
        listMucHuongBaoHiemLoftCare: this.quyenLoiBaoHiem.listMucHuongBaoHiemLoftCare
      }
    };

    this.loading = true;
    let result: any = await this.employeeService.createOrUpdateCauHinhBaoHiemLoftCare(3, dataInput);

    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }

    this.showMessage('success', result.messageCode);
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
