import { Component, OnInit} from '@angular/core';
import { FormGroup, FormControl, Validators} from "@angular/forms";
import { MessageService } from "primeng/api";

import { CauHinhBaoHiemModel} from "../../../models/cauhinh-baohiem.model";

import { EmployeeService } from "../../../services/employee.service";
import { CommonService } from '../../../../shared/services/common.service';

@Component({
  selector: 'app-bao-hiem-xa-hoi',
  templateUrl: './bao-hiem-xa-hoi.component.html',
  styleUrls: ['./bao-hiem-xa-hoi.component.css']
})
export class BaoHiemXaHoiComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới

  id: any;
  cauHinhBaoHiemModel: any;
  listLoaiDong: any;

  cauHinhBaoHiemClone = new CauHinhBaoHiemModel();

  //Bảo hiểm xã hội
  baoHiemForm: FormGroup;
  loaiDongControl: FormControl;
  mucDongControl: FormControl;
  mucDongToiDaControl: FormControl;
  mucLuongCoSoControl: FormControl;
  bhxhControl: FormControl;
  bhytControl: FormControl;
  bhtnControl: FormControl;
  bhtnnnControl: FormControl;
  bhxhCtyControl: FormControl;
  bhytCtyControl: FormControl;
  bhtnCtyControl: FormControl;
  bhtnnnCtyControl: FormControl;

  constructor(
    private messageService: MessageService,
    private employeeService: EmployeeService,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.getMasterData();
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.employeeService.getMasterCauHinhBaoHiem();
    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    if (result.isExists) this.id = result.cauHinhBaoHiemModel.cauHinhBaoHiemId;
    this.cauHinhBaoHiemClone = result.cauHinhBaoHiemModel;
    this.listLoaiDong = result.listLoaiDong;
    
    this.mapDataToForm(result.cauHinhBaoHiemModel);
  }

  //Bảo hiểm xã hội
  initForm() {
    this.loaiDongControl = new FormControl(null, [Validators.required]);
    this.mucDongControl = new FormControl(0, [Validators.required]);
    this.mucDongToiDaControl = new FormControl(null, [Validators.required]);
    this.mucLuongCoSoControl = new FormControl(null, [Validators.required]);
    this.bhxhControl = new FormControl(null, [Validators.required]);
    this.bhytControl = new FormControl(null, [Validators.required]);
    this.bhtnControl = new FormControl(null, [Validators.required]);
    this.bhtnnnControl = new FormControl(null, [Validators.required]);
    this.bhxhCtyControl = new FormControl(null, [Validators.required]);
    this.bhytCtyControl = new FormControl(null, [Validators.required]);
    this.bhtnCtyControl = new FormControl(null, [Validators.required]);
    this.bhtnnnCtyControl = new FormControl(null, [Validators.required]);

    this.baoHiemForm = new FormGroup({
      loaiDongControl: this.loaiDongControl,
      mucDongControl: this.mucDongControl,
      mucDongToiDaControl: this.mucDongToiDaControl,
      mucLuongCoSoControl: this.mucLuongCoSoControl,
      bhxhControl: this.bhxhControl,
      bhytControl: this.bhytControl,
      bhtnControl: this.bhtnControl,
      bhtnnnControl: this.bhtnnnControl,
      bhxhCtyControl: this.bhxhCtyControl,
      bhytCtyControl: this.bhytCtyControl,
      bhtnCtyControl: this.bhtnCtyControl,
      bhtnnnCtyControl: this.bhtnnnCtyControl,
    });

    this.baoHiemForm.disable();
  }

  mapDataToForm(cauHinhBaoHiemModel: CauHinhBaoHiemModel) {
    let loaiDong = this.listLoaiDong.find(x => x.value == cauHinhBaoHiemModel.loaiDong);
    this.loaiDongControl.setValue(loaiDong ? loaiDong : null);
    this.mucDongControl.setValue(cauHinhBaoHiemModel.mucDong);
    this.mucDongToiDaControl.setValue(cauHinhBaoHiemModel.mucDongToiDa);
    this.mucLuongCoSoControl.setValue(cauHinhBaoHiemModel.mucLuongCoSo);

    this.bhxhControl.setValue(
      cauHinhBaoHiemModel.tiLePhanBoMucDongBhxhcuaNld
    );
    this.bhytControl.setValue(
      cauHinhBaoHiemModel.tiLePhanBoMucDongBhytcuaNld
    );
    this.bhtnControl.setValue(
      cauHinhBaoHiemModel.tiLePhanBoMucDongBhtncuaNld
    );
    this.bhtnnnControl.setValue(
      cauHinhBaoHiemModel.tiLePhanBoMucDongBhtnnncuaNld
    );
    this.bhxhCtyControl.setValue(
      cauHinhBaoHiemModel.tiLePhanBoMucDongBhxhcuaNsdld
    );
    this.bhytCtyControl.setValue(
      cauHinhBaoHiemModel.tiLePhanBoMucDongBhytcuaNsdld
    );
    this.bhtnCtyControl.setValue(
      cauHinhBaoHiemModel.tiLePhanBoMucDongBhtncuaNsdld
    );
    this.bhtnnnCtyControl.setValue(
      cauHinhBaoHiemModel.tiLePhanBoMucDongBhtnnncuaNsdld
    );

    this.changeLoaiDong();
  }

  changeLoaiDong() {
    let loaiDong = this.loaiDongControl.value;

    //Nếu không chọn hoặc chọn loại đóng là Full lương
    if (!loaiDong || loaiDong.value == 1) {
      this.mucDongControl.setValidators([]);
    }
    //Nếu chọn loại đóng là Mức đóng
    else {
      this.mucDongControl.setValidators([Validators.required]);
    }

    this.mucDongControl.updateValueAndValidity();
  }

  suaCauHinhBaoHiem() {
    this.baoHiemForm.enable();
  }

  luuCauHinhBaoHiem() {
    if (!this.baoHiemForm.valid) {
      Object.keys(this.baoHiemForm.controls).forEach(key => {
        if (!this.baoHiemForm.controls[key].valid) {
          this.baoHiemForm.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    let bhxhModel = new CauHinhBaoHiemModel();
    bhxhModel.cauHinhBaoHiemId = this.id;
    bhxhModel.loaiDong = this.loaiDongControl.value.value;

    if (bhxhModel.loaiDong == 1) bhxhModel.mucDong = 0;
    else bhxhModel.mucDong = this.commonService.convertStringToNumber(this.mucDongControl.value.toString());

    bhxhModel.mucDongToiDa = this.commonService.convertStringToNumber(this.mucDongToiDaControl.value.toString());
    bhxhModel.mucLuongCoSo = this.commonService.convertStringToNumber(this.mucLuongCoSoControl.value.toString());

    bhxhModel.tiLePhanBoMucDongBhxhcuaNld = this.commonService.convertStringToNumber(this.bhxhControl.value.toString());
    bhxhModel.tiLePhanBoMucDongBhytcuaNld = this.commonService.convertStringToNumber(this.bhytControl.value.toString());
    bhxhModel.tiLePhanBoMucDongBhtncuaNld = this.commonService.convertStringToNumber(this.bhtnControl.value.toString());
    bhxhModel.tiLePhanBoMucDongBhtnnncuaNld = this.commonService.convertStringToNumber(this.bhtnnnControl.value.toString());

    bhxhModel.tiLePhanBoMucDongBhxhcuaNsdld = this.commonService.convertStringToNumber(this.bhxhCtyControl.value.toString());
    bhxhModel.tiLePhanBoMucDongBhytcuaNsdld = this.commonService.convertStringToNumber(this.bhytCtyControl.value.toString());
    bhxhModel.tiLePhanBoMucDongBhtncuaNsdld = this.commonService.convertStringToNumber(this.bhtnCtyControl.value.toString());
    bhxhModel.tiLePhanBoMucDongBhtnnncuaNsdld = this.commonService.convertStringToNumber(this.bhtnnnCtyControl.value.toString());

    if (bhxhModel.tiLePhanBoMucDongBhxhcuaNld > 100 ||
      bhxhModel.tiLePhanBoMucDongBhytcuaNld > 100 ||
      bhxhModel.tiLePhanBoMucDongBhtncuaNld > 100 ||
      bhxhModel.tiLePhanBoMucDongBhtnnncuaNld > 100 ||
      bhxhModel.tiLePhanBoMucDongBhxhcuaNsdld > 100 ||
      bhxhModel.tiLePhanBoMucDongBhytcuaNsdld > 100 ||
      bhxhModel.tiLePhanBoMucDongBhtncuaNsdld > 100 ||
      bhxhModel.tiLePhanBoMucDongBhtnnncuaNsdld > 100) {
      this.showMessage("error", 'Số phần trăm không được vượt quá 100%');
      return;
    }

    this.loading = true;
    this.awaitResult = true;
    this.employeeService.createOrUpdateCauHinhBaoHiem(bhxhModel).subscribe(async response => {
      let result = <any>response;

      if (result.statusCode != 200) {
        this.loading = false;
        this.awaitResult = false;
        this.showMessage("error", result.messageCode);
        return;
      }

      this.id = result.cauHinhBaoHiemId;
      let _result: any = await this.employeeService.getCauHinhBaoHiemById(this.id);
      this.loading = false;
      this.awaitResult = false;

      if (_result.statusCode != 200) {
        this.showMessage("error", _result.messageCode);
        return;
      }

      this.cauHinhBaoHiemClone = _result.cauHinhBaoHiem;
      this.baoHiemForm.disable();
      this.showMessage("success", result.messageCode);
    });
  }

  huySuaCauHinhBaoHiem() {
    this.mapDataToForm(this.cauHinhBaoHiemClone);
    this.baoHiemForm.disable();
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }

}
