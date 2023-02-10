import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Component, OnInit, Input } from '@angular/core';
import { MessageService } from 'primeng/api';

import { EmployeeService } from './../../../../services/employee.service';
import { DataService } from './../../../../../shared/services/data.service';

import { HocVanTuyenDungModel } from './../../../../models/hoc-van-tuyen-dung.model';

@Component({
  selector: 'app-hoc-van-va-tuyen-dung',
  templateUrl: './hoc-van-va-tuyen-dung.component.html',
  styleUrls: ['./hoc-van-va-tuyen-dung.component.css']
})
export class HocVanVaTuyenDungComponent implements OnInit {
  loading: boolean = false;

  @Input() actionEdit: boolean;
  @Input() employeeId: string;

  isShowButtonSua: boolean = false;
  isEdit: boolean = false;
  hocVanForm: FormGroup;

  trinhDoHocVanTuyenDung: HocVanTuyenDungModel;
  trinhDoHocVanTuyenDungClone: HocVanTuyenDungModel;

  listBangCap = [];
  listPhuongThucTuyenDung = [];
  listNguonTuyenDung = [];
  listKyNangTayNghe = [];

  constructor(
    public messageService: MessageService,
    private employeeService: EmployeeService,
    private dataService: DataService,
  ) { }

  ngOnInit(): void {
    this.getTrinhDoHocVanTuyenDung();

    this.initForm();
  }

  initForm() {
    this.hocVanForm = new FormGroup({
      TenTruong: new FormControl(''),
      ChuyenNganh: new FormControl(''),
      BangCap: new FormControl(''),
      KyNang: new FormControl(''),
      HocVan: new FormControl(''),
      KinhNghiem: new FormControl(''),
      PhuongThucTuyenDung: new FormControl(''),
      ChuyenNganhHoc: new FormControl(''),
      MucPhi: new FormControl('a'),
      NguonTuyenDung: new FormControl('')
    });

    this.hocVanForm.disable();
  }

  async getTrinhDoHocVanTuyenDung() {
    this.loading = true;
    let result: any = await this.employeeService.getTrinhDoHocVanTuyenDung(this.employeeId);
    this.loading = false;

    if (result.statusCode == 200) {
      this.isShowButtonSua = result.isShowButtonSua;

      this.trinhDoHocVanTuyenDung = result.trinhDoHocVanTuyenDung;
      this.listBangCap = result.trinhDoHocVanTuyenDung.listBangCap;
      this.listNguonTuyenDung = result.trinhDoHocVanTuyenDung.listNguonTuyenDung;
      this.listPhuongThucTuyenDung = result.trinhDoHocVanTuyenDung.listPhuongThucTuyenDung;
      this.listKyNangTayNghe = result.trinhDoHocVanTuyenDung.listKyNangTayNghe;

      this.trinhDoHocVanTuyenDungClone = result.trinhDoHocVanTuyenDung;

      this.setForm(this.trinhDoHocVanTuyenDung);
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  setForm(trinhDoHocVanTuyenDung: HocVanTuyenDungModel) {
    this.hocVanForm.setValue({

      TenTruong: trinhDoHocVanTuyenDung ?.tenTruongHocCaoNhat,
      ChuyenNganh: trinhDoHocVanTuyenDung ?.chuyenNganhHoc,
      BangCap: trinhDoHocVanTuyenDung ?.bangCapCaoNhatDatDuocId ? this.listBangCap.find(x => x.categoryId == trinhDoHocVanTuyenDung ?.bangCapCaoNhatDatDuocId) : null,
      KyNang: trinhDoHocVanTuyenDung ?.kyNangTayNghes ? this.listKyNangTayNghe.find(x => x.value == trinhDoHocVanTuyenDung ?.kyNangTayNghes) : null,
      HocVan: trinhDoHocVanTuyenDung ?.tomTatHocVan,
      KinhNghiem: trinhDoHocVanTuyenDung ?.kinhNghiemLamViec,
      PhuongThucTuyenDung: trinhDoHocVanTuyenDung ?.phuongThucTuyenDungId ? this.listPhuongThucTuyenDung.find(x => x.categoryId == trinhDoHocVanTuyenDung ?.phuongThucTuyenDungId) : null,
      ChuyenNganhHoc: trinhDoHocVanTuyenDung ?.loaiChuyenNganhHoc,
      MucPhi: trinhDoHocVanTuyenDung ?.mucPhi,
      NguonTuyenDung: trinhDoHocVanTuyenDung ?.nguonTuyenDungId ? this.listNguonTuyenDung.find(x => x.categoryId == trinhDoHocVanTuyenDung ?.nguonTuyenDungId) : null,
    });
  }

  enabledForm() {
    this.isEdit = true;
    this.hocVanForm.enable();
  }

  disabledForm() {
    this.isEdit = false;
    this.setForm(this.trinhDoHocVanTuyenDungClone);
    this.hocVanForm.disable();
  }

  async saveForm() {
    let formData = this.hocVanForm.value;
    let hocVanTuyenDung: HocVanTuyenDungModel = {
      employeeId: this.employeeId,
      tenTruongHocCaoNhat: formData ?.TenTruong ? formData ?.TenTruong.trim() : null,
      chuyenNganhHoc: formData ?.ChuyenNganh ? formData ?.ChuyenNganh.trim() : null,
      bangCapCaoNhatDatDuocId: formData.BangCap ?.categoryId,
      kyNangTayNghes: formData ?.KyNang ? formData ?.KyNang.value : null,
      tomTatHocVan: formData ?.HocVan ? formData ?.HocVan.trim() : null,
      kinhNghiemLamViec: formData ?.KinhNghiem ? formData ?.KinhNghiem.trim() : null,
      phuongThucTuyenDungId: formData.PhuongThucTuyenDung ?.categoryId,
      loaiChuyenNganhHoc: formData ?.ChuyenNganhHoc ? formData ?.ChuyenNganhHoc.trim() : null,
      mucPhi: formData ?.MucPhi,
      nguonTuyenDungId: formData.NguonTuyenDung ?.categoryId,
    };

    this.loading = true;
    let result: any = await this.employeeService.updateTrinhDoHocVanTuyenDung(hocVanTuyenDung);
    this.loading = false;

    if (result.statusCode == 200) {
      this.isEdit = false;
      this.hocVanForm.disable();
      this.getTrinhDoHocVanTuyenDung();
      this.showMessage('success', result.messageCode);
      this.dataService.changeMessage("Update success"); //thay đổi message để call lại api getListNote trong component NoteTimeline
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}
