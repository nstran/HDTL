import { Component, OnInit, Input, SimpleChanges } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MessageService, ConfirmationService } from 'primeng/api';
import { EmployeeService } from './../../../../services/employee.service';
import { FormatDateService } from './../../../../../shared/services/formatDate.services';
import { DataService } from './../../../../../shared/services/data.service';

class ThongTinNhanSu {
  employeeId: string;
  codeMayChamCong: string; // Code máy chấm công
  tenMayChamCong: string; // Họ tên trên máy chấm công
  subCode1Value: Number;
  subCode2Value: Number;
  deptCodeValue: Number;
  capBacId: string;
  maTest: Date;
  diemTest: Date;
  identityId: string; //Số cmnd
  identityIddateOfIssue: Date; //Ngày cấp cmnd
  identityIdplaceOfIssue: Date; //Nơi cấp cmnd tiếng việt
  noiCapCmndtiengAnh: string; //Nơi cấp cmnd tiếng anh
  nguyenQuan: Date;
  noiSinh: Date;
  hoKhauThuongTruTv: string; //Hộ khẩu thường trú tiếng việt
  hoKhauThuongTruTa: string; //Hộ khẩu thường trú tiếng anh
  address: string; //Nơi ở hiện tại
  addressTiengAnh: string; //Nơi ở hiện tại - tiếng anh
  provinceId: string; //Địa điểm làm việc
}

@Component({
  selector: 'app-thong-tin-nhan-su',
  templateUrl: './thong-tin-nhan-su.component.html',
  styleUrls: ['./thong-tin-nhan-su.component.css']
})
export class ThongTinNhanSuComponent implements OnInit {
  loading = false;
  isEdit = false;
  @Input() actionEdit: boolean;
  @Input() employeeId: string;
  @Input() listQuocGia: Array<any>;
  @Input() listNganHang: Array<any>;
  filteredNganHang: Array<any> = [];

  isShowButtonSua: boolean = false;

  submitted: boolean = false;

  listSubCode1 = [];
  listSubCode2 = [];
  listDeptCode = [];
  listCapBac = [];
  listProvince = [];

  thongTinNhanSu = new ThongTinNhanSu();
  thongTinNhanSuClone = new ThongTinNhanSu();

  thongTinNhanSuForm: FormGroup;

  error: any = {
    codeMayChamCong: '',
    tenMayChamCong: '',
    subCode1: '',
    subCode2: '',
    deptCode: '',
    identityId: '',
    identityIddateOfIssue: '',
    identityIdplaceOfIssue: '',
    noiCapCmndtiengAnh: '',
    nguyenQuan: '',
    noiSinh: '',
    hoKhauThuongTruTv: '',
    hoKhauThuongTruTa: '',
    address: '',
    addressTiengAnh: '',
    provinceId: ''
  }


  constructor(
    public messageService: MessageService,
    public employeeService: EmployeeService,
    private formatDateService: FormatDateService,
    private dataService: DataService,
  ) { }

  ngOnInit(): void {
    this.initForm();
  }

  initForm() {
    this.thongTinNhanSuForm = new FormGroup({
      codeMayChamCong: new FormControl(null, [Validators.required, forbiddenSpaceText]),
      tenMayChamCong: new FormControl(null, [Validators.required, forbiddenSpaceText]),
      subCode1: new FormControl(null, [Validators.required]),
      subCode2: new FormControl(null, [Validators.required]),
      deptCode: new FormControl(null, [Validators.required]),
      capBac: new FormControl(null),
      maTest: new FormControl(null),
      diemTest: new FormControl(null),
      identityId: new FormControl(null, [Validators.required, forbiddenSpaceText]),
      identityIddateOfIssue: new FormControl(null, [Validators.required]),
      identityIdplaceOfIssue: new FormControl(null, [Validators.required, forbiddenSpaceText]),
      noiCapCmndtiengAnh: new FormControl(null, [Validators.required, forbiddenSpaceText]),
      nguyenQuan: new FormControl(null, [Validators.required, forbiddenSpaceText]),
      noiSinh: new FormControl(null, [Validators.required, forbiddenSpaceText]),
      hoKhauThuongTruTv: new FormControl(null, [Validators.required, forbiddenSpaceText]),
      hoKhauThuongTruTa: new FormControl(null, [Validators.required, forbiddenSpaceText]),
      address: new FormControl(null, [Validators.required, forbiddenSpaceText]),
      addressTiengAnh: new FormControl(null, [Validators.required, forbiddenSpaceText]),
      provinceId: new FormControl(null, [Validators.required]),
    });

    this.thongTinNhanSuForm.disable();
  }

  /* Nếu Input có thay đổi */
  ngOnChanges(changes: SimpleChanges) {
    if (this.listQuocGia ?.length > 0)
      this.getThongTinNhanSu();
  }

  async getThongTinNhanSu() {
    this.loading = true;
    let result: any = await this.employeeService.getThongTinNhanSu(this.employeeId);
    this.loading = false;

    if (result.statusCode == 200) {
      this.thongTinNhanSu = result.thongTinNhanSu;
      this.thongTinNhanSuClone = result.thongTinNhanSu;
      this.listSubCode1 = result.listSubCode1;
      this.listSubCode2 = result.listSubCode2;
      this.listDeptCode = result.listDeptCode;
      this.listCapBac = result.listCapBac;
      this.listProvince = result.listProvince;

      this.isShowButtonSua = result.isShowButtonSua;

      this.mapDataToForm(this.thongTinNhanSu);
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  enabledForm() {
    this.isEdit = true;
    this.thongTinNhanSuForm.enable();
  }

  async saveForm() {
    this.submitted = true;

    if (!this.thongTinNhanSuForm.valid) {
      if (this.thongTinNhanSuForm.get('codeMayChamCong').errors ?.required || this.thongTinNhanSuForm.get('codeMayChamCong').errors ?.forbiddenSpaceText) {
        this.error['codeMayChamCong'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('tenMayChamCong').errors ?.required || this.thongTinNhanSuForm.get('tenMayChamCong').errors ?.forbiddenSpaceText) {
        this.error['tenMayChamCong'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('subCode1').errors ?.required) {
        this.error['subCode1'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('subCode2').errors ?.required) {
        this.error['subCode2'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('deptCode').errors ?.required) {
        this.error['deptCode'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('identityId').errors ?.required || this.thongTinNhanSuForm.get('identityId').errors ?.forbiddenSpaceText) {
        this.error['identityId'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('identityIddateOfIssue').errors ?.required) {
        this.error['identityIddateOfIssue'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('identityIdplaceOfIssue').errors ?.required || this.thongTinNhanSuForm.get('identityIdplaceOfIssue').errors ?.forbiddenSpaceText) {
        this.error['identityIdplaceOfIssue'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('noiCapCmndtiengAnh').errors ?.required || this.thongTinNhanSuForm.get('noiCapCmndtiengAnh').errors ?.forbiddenSpaceText) {
        this.error['noiCapCmndtiengAnh'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('nguyenQuan').errors ?.required || this.thongTinNhanSuForm.get('nguyenQuan').errors ?.forbiddenSpaceText) {
        this.error['nguyenQuan'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('noiSinh').errors ?.required || this.thongTinNhanSuForm.get('noiSinh').errors ?.forbiddenSpaceText) {
        this.error['noiSinh'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('hoKhauThuongTruTv').errors ?.required || this.thongTinNhanSuForm.get('hoKhauThuongTruTv').errors ?.forbiddenSpaceText) {
        this.error['hoKhauThuongTruTv'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('hoKhauThuongTruTa').errors ?.required || this.thongTinNhanSuForm.get('hoKhauThuongTruTa').errors ?.forbiddenSpaceText) {
        this.error['hoKhauThuongTruTa'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('address').errors ?.required || this.thongTinNhanSuForm.get('address').errors ?.forbiddenSpaceText) {
        this.error['address'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('addressTiengAnh').errors ?.required || this.thongTinNhanSuForm.get('addressTiengAnh').errors ?.forbiddenSpaceText) {
        this.error['addressTiengAnh'] = 'Không được để trống';
      }

      if (this.thongTinNhanSuForm.get('provinceId').errors ?.required) {
        this.error['provinceId'] = 'Không được để trống';
      }

      return;
    }

    let thongTinNhanSu = this.mapDataToModel();
    this.loading = true;
    let result: any = await this.employeeService.saveThongTinNhanSu(thongTinNhanSu);
    this.loading = false;

    if (result.statusCode == 200) {
      this.isEdit = false;
      this.thongTinNhanSuForm.disable();
      this.getThongTinNhanSu();
      this.showMessage('success', result.messageCode);
      this.dataService.changeMessage("Update success"); //thay đổi message để call lại api getListNote trong component NoteTimeline
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  disabledForm() {
    this.isEdit = false;
    this.mapDataToForm(this.thongTinNhanSuClone);
    this.thongTinNhanSuForm.disable();
    this.error = {
      codeMayChamCong: '',
      tenMayChamCong: '',
      subCode1: '',
      subCode2: '',
      deptCode: '',
      identityId: '',
      identityIddateOfIssue: '',
      identityIdplaceOfIssue: '',
      noiCapCmndtiengAnh: '',
      nguyenQuan: '',
      noiSinh: '',
      hoKhauThuongTruTv: '',
      hoKhauThuongTruTa: '',
      address: '',
      addressTiengAnh: '',
      provinceId: ''
    }
  }

  mapDataToForm(thongTinNhanSu: ThongTinNhanSu) {
    this.thongTinNhanSuForm.setValue({
      codeMayChamCong: thongTinNhanSu ?.codeMayChamCong ?? null,
      tenMayChamCong: thongTinNhanSu ?.tenMayChamCong ?? null,
      subCode1: thongTinNhanSu ?.subCode1Value ? this.listSubCode1.find(x => x.value == thongTinNhanSu.subCode1Value) : null,
      subCode2: thongTinNhanSu ?.subCode2Value ? this.listSubCode2.find(x => x.value == thongTinNhanSu.subCode2Value) : null,
      deptCode: thongTinNhanSu ?.deptCodeValue ? this.listDeptCode.find(x => x.value == thongTinNhanSu.deptCodeValue) : null,
      capBac: thongTinNhanSu ?.capBacId ? this.listCapBac.find(x => x.categoryId == thongTinNhanSu.capBacId) : null,
      maTest: thongTinNhanSu ?.maTest ?? null,
      diemTest: thongTinNhanSu ?.diemTest ?? null,
      identityId: thongTinNhanSu ?.identityId ?? null,
      identityIddateOfIssue: thongTinNhanSu ?.identityIddateOfIssue ? new Date(thongTinNhanSu ?.identityIddateOfIssue) : null,
      identityIdplaceOfIssue: thongTinNhanSu ?.identityIdplaceOfIssue ?? null,
      noiCapCmndtiengAnh: thongTinNhanSu ?.noiCapCmndtiengAnh ?? null,
      nguyenQuan: thongTinNhanSu ?.nguyenQuan ?? null,
      noiSinh: thongTinNhanSu ?.noiSinh ?? null,
      hoKhauThuongTruTv: thongTinNhanSu ?.hoKhauThuongTruTv ?? null,
      hoKhauThuongTruTa: thongTinNhanSu ?.hoKhauThuongTruTa ?? null,
      address: thongTinNhanSu ?.address ?? null,
      addressTiengAnh: thongTinNhanSu ?.addressTiengAnh ?? null,
      provinceId: thongTinNhanSu ?.provinceId ? this.listProvince.find(x => x.provinceId == thongTinNhanSu.provinceId) : null,
    });
  }

  mapDataToModel(): ThongTinNhanSu {
    let result = new ThongTinNhanSu();
    let data = this.thongTinNhanSuForm.value;

    result.employeeId = this.employeeId;
    result.codeMayChamCong = data.codeMayChamCong.trim();
    result.tenMayChamCong = data.tenMayChamCong.trim();
    result.subCode1Value = data ?.subCode1 ? data.subCode1.value : null;
    result.subCode2Value = data ?.subCode2 ? data.subCode2.value : null;
    result.deptCodeValue = data ?.deptCode ? data.deptCode.value : null;
    result.capBacId = data ?.capBac ? data.capBac.categoryId : null;
    result.maTest = data.maTest ? data.maTest.trim() : null;
    result.diemTest = data.diemTest ? data.diemTest.trim() : null;
    result.identityId = data.identityId.trim();
    result.identityIddateOfIssue = data.identityIddateOfIssue ? this.formatDateService.convertToUTCTime(data.identityIddateOfIssue) : null;;
    result.identityIdplaceOfIssue = data.identityIdplaceOfIssue.trim();
    result.noiCapCmndtiengAnh = data.noiCapCmndtiengAnh.trim();
    result.nguyenQuan = data.nguyenQuan.trim();
    result.noiSinh = data.noiSinh.trim();
    result.hoKhauThuongTruTv = data.hoKhauThuongTruTv.trim();
    result.hoKhauThuongTruTa = data.hoKhauThuongTruTa.trim();
    result.address = data.address.trim();
    result.addressTiengAnh = data.addressTiengAnh.trim();
    result.provinceId = data.provinceId ? data.provinceId.provinceId : null;

    return result;
  }

  filterTenNganHang(event) {
    let filtered: any[] = [];
    let query = event.query;
    for (let i = 0; i < this.listNganHang.length; i++) {
      let nganHang = this.listNganHang[i];
      if (nganHang.categoryName.toLowerCase().indexOf(query.toLowerCase()) != -1) {
        filtered.push(nganHang);
      }
    }

    this.filteredNganHang = filtered;
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
};
