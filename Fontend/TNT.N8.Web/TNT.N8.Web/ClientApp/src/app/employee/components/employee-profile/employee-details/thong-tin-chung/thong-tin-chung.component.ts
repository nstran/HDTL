import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Component, OnInit, Input, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Time, DatePipe } from '@angular/common';
import { DomSanitizer } from '@angular/platform-browser';
import { DialogService } from 'primeng/dynamicdialog';

import { EmployeeService } from './../../../../services/employee.service';
import { HandleFileService } from './../../../../../shared/services/handleFile.service';
import { FormatDateService } from './../../../../../shared/services/formatDate.services';
import { DataService } from './../../../../../shared/services/data.service';
import { CommonService } from '../../../../../shared/services/common.service';

import { ChonNhieuDvDialogComponent } from './../../../../../shared/components/chon-nhieu-dv-dialog/chon-nhieu-dv-dialog.component';
import { CategoryEntityModel } from '../../../../../../../src/app/product/models/product.model';
import { GetListBenefitType } from '../../../../models/employee-type.model';

class ThongTinChung {
  employeeId: string;
  employeeCode: string;
  avatarUrl: string;
  userName: string; //Tên đăng nhập
  trangThaiId: number;
  organizationId: string;
  organizationName: string;
  dateOfBirth: Date;
  phone: string;
  workEmail: string;
  firstName: string;
  lastName: string;
  gioiTinh: string;
  mission: string;
  benefit: number;
  percenBenefit: number;
  employeeType: number;
}

@Component({
  selector: 'app-thong-tin-chung',
  templateUrl: './thong-tin-chung.component.html',
  styleUrls: ['./thong-tin-chung.component.css'],
  providers: [DatePipe]
})
export class ThongTinChungComponent implements OnInit {
  loading = false;
  isEdit = false;
  @Input() listPosition: Array<any>;
  @Input() actionEdit: boolean;
  @Input() employeeId: string;
  @Input() listLoaiHopDong: Array<any>;
  @Input() listOrganization: Array<any>;

  isShowButtonSua = true;

  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  apiEndPoint = localStorage.getItem('ApiEndPoint');

  submitted: boolean = false;

  listGioiTinh = [
    { code: 'NAM', name: 'Nam' },
    { code: 'NU', name: 'Nữ' },
  ];

  listTrangThai = [
    {
      id: 1, text: 'Đang hoạt động - Được phép truy cập'
    },
    {
      id: 2, text: 'Đang hoạt động - Không được phép truy cập'
    },
    {
      id: 3, text: 'Ngừng hoạt động'
    }
  ];

  /* Tạo cộng tác viên */
  checked: boolean = false;
  listBenefitType: GetListBenefitType[] = [];
  isShowPercenBenefit: boolean = false;

  defaultLimitedFileSize = 2000000; //2MB
  uploadedFiles = [];
  currentLogoUrl: any = '/assets/images/no-avatar.png';
  newLogoUrl: any = null;
  extLogo: any = null;

  thongTinChung = new ThongTinChung();
  thongTinChungClone = new ThongTinChung();

  thongTinChungForm: FormGroup;

  listSelectedDonVi: Array<any> = [];

  listMission: Array<CategoryEntityModel> = [];
  error: any = {
    TrangThai: '',
    FirstName: '',
    LastName: '',
    Gender: '',
    DateOfBirth: '',
    OrganizationId: '',
    mission: '',
    Phone: '',
    WorkEmail: '',
    Benefit: 1,
    PercenBenefit: null
  };

  constructor(
    public messageService: MessageService,
    public employeeService: EmployeeService,
    public datePipe: DatePipe,
    public dialogService: DialogService,
    public changeRef: ChangeDetectorRef,
    private handleFileService: HandleFileService,
    private domSanitizer: DomSanitizer,
    private formatDateService: FormatDateService,
    private dataService: DataService,
    private commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.getBenefitType();
  }

  initForm() {
    let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
    this.thongTinChungForm = new FormGroup({
      TrangThai: new FormControl('', [Validators.required]),
      FirstName: new FormControl('', [Validators.required, Validators.maxLength(250), forbiddenSpaceText]),
      LastName: new FormControl('', [Validators.required, Validators.maxLength(250), forbiddenSpaceText]),
      Gender: new FormControl(''),
      DateOfBirth: new FormControl('', [Validators.required]),
      OrganizationId: new FormControl({ value: null, disabled: true }, [Validators.required]),
      mission: new FormControl(''),
      Phone: new FormControl('', [Validators.required, Validators.pattern(this.getPhonePattern()), forbiddenSpaceText]),
      WorkEmail: new FormControl('', [Validators.required, Validators.pattern(emailPattern)]),
      Benefit: new FormControl(null),
      PercenBenefit: new FormControl(null),
      EmployeeType: new FormControl(null),
    });

    this.thongTinChungForm.disable();
  }

  async getThongTinChungThanhVien() {
    this.loading = true;
    let result: any = await this.employeeService.getThongTinChungThanhVien(this.employeeId);
    this.loading = false;

    if (result.statusCode == 200) {
      this.thongTinChung = result.thongTinChung;
      this.thongTinChungClone = result.thongTinChung;
      this.listMission = result.listMission;


      this.currentLogoUrl = this.thongTinChung.avatarUrl ? this.apiEndPoint + '/' + this.thongTinChung.avatarUrl : '/assets/images/no-avatar.png';
      this.listSelectedDonVi = this.thongTinChung.organizationId ? [{ organizationId: this.thongTinChung.organizationId, organizationName: this.thongTinChung.organizationName }] : [];

      this.mapDataToForm(this.thongTinChung);
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  /* Nếu Input có thay đổi */
  ngOnChanges(changes: SimpleChanges) {
    if (this.listLoaiHopDong?.length > 0) {
      this.getThongTinChungThanhVien();
    }
  }

  mapDataToForm(thongTinChung: ThongTinChung) {
    this.thongTinChungForm.setValue({
      TrangThai: thongTinChung?.trangThaiId ? this.listTrangThai.find(x => x.id == thongTinChung.trangThaiId) : null,
      FirstName: thongTinChung?.firstName ?? null,
      LastName: thongTinChung?.lastName ?? null,
      Gender: thongTinChung?.gioiTinh ?? null,
      DateOfBirth: thongTinChung?.dateOfBirth ? new Date(thongTinChung.dateOfBirth) : null,
      OrganizationId: thongTinChung?.organizationName ?? null,
      mission: thongTinChung?.mission ? this.listMission.find(x => x.categoryId == thongTinChung.mission) : null,
      Phone: thongTinChung?.phone ?? null,
      WorkEmail: thongTinChung?.workEmail ?? null,
      Benefit: thongTinChung?.benefit != null ? this.listBenefitType.find(x => x.value == thongTinChung?.benefit) : null,
      PercenBenefit: thongTinChung?.percenBenefit ?? null,
      EmployeeType: thongTinChung?.employeeType == 1 ? true : false
    });
    this.checkCongTacVien(thongTinChung?.employeeType);
    if (thongTinChung?.benefit == 2) this.isShowPercenBenefit = true;
    if (thongTinChung?.benefit == 1) this.isShowPercenBenefit = false;
    this.checked = thongTinChung?.employeeType == 1 ? true : false;
  }

  enabledForm() {
    this.isEdit = true;
    this.thongTinChungForm.enable();
  }

  uploadImage() {
    document.getElementById('imageProfile').click();
  }

  transform(url) {
    return this.domSanitizer.bypassSecurityTrustResourceUrl(url);
  }

  changeFile(file) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result);
      reader.onerror = error => reject(error);
    });
  }

  async handleFileUpload(event) {
    if (event.target.files.length > 0) {
      if (event.target.files[0].size > this.defaultLimitedFileSize) {
        this.showMessage('error', 'Kích thước ảnh quá lớn');
        return;
      }
      this.newLogoUrl = await this.changeFile(event.target.files[0]);
      let index = event.target.files[0].name.lastIndexOf('.');
      this.extLogo = event.target.files[0].name.substring(index + 1);

      this.changeRef.detectChanges();
      setTimeout(() => {
        this.currentLogoUrl = this.newLogoUrl;
        this.uploadedFiles = this.handleFileService.convertFileName(event.target);
      }, 500)
    }
    else {
      this.currentLogoUrl = '/assets/images/no-avatar.png';
    }
  }


  async saveForm() {
    this.submitted = true;

    let OrganizationId = this.thongTinChungForm.get('OrganizationId').value;

    if (!this.thongTinChungForm.valid || !OrganizationId) {
      if (this.thongTinChungForm.get('FirstName').errors?.required || this.thongTinChungForm.get('FirstName').errors?.forbiddenSpaceText) {
        this.error['FirstName'] = 'Không được để trống';
      } else if (this.thongTinChungForm.get('FirstName').errors?.maxlength) {
        this.error['FirstName'] = 'Không vượt quá 250 kí tự';
      }

      if (this.thongTinChungForm.get('LastName').errors?.required || this.thongTinChungForm.get('LastName').errors?.forbiddenSpaceText) {
        this.error['LastName'] = 'Không được để trống';
      } else if (this.thongTinChungForm.get('LastName').errors?.maxlength) {
        this.error['LastName'] = 'Không vượt quá 250 kí tự';
      }


      if (this.thongTinChungForm.get('DateOfBirth').errors?.required) {
        this.error['DateOfBirth'] = 'Không được để trống';
      }

      if (this.thongTinChungForm.get('Phone').errors?.required) {
        this.error['Phone'] = 'Không được để trống';
      } else if (this.thongTinChungForm.get('Phone').errors?.pattern) {
        this.error['Phone'] = 'Không đúng định dạng';
      }

      if (this.thongTinChungForm.get('WorkEmail').errors?.required) {
        this.error['WorkEmail'] = 'Không được để trống';
      } else if (this.thongTinChungForm.get('WorkEmail').errors?.pattern) {
        this.error['WorkEmail'] = 'Không đúng định dạng';
      }


      if (this.thongTinChungForm.get('TrangThai').errors?.required) {
        this.error['TrangThai'] = 'Không được để trống';
      }

      if (!OrganizationId) {
        this.error['OrganizationId'] = 'Không được để trống';
      } else {
        this.error['OrganizationId'] = null;
      }


      return;
    }

    let employeeType = null;
    let benefit = null;
    let benefitPercent = null;
    if (!this.checked) {
      employeeType = 2;
    } else {
      employeeType = 1;
      let benefitFind = this.thongTinChungForm.get('Benefit').value;
      if (benefitFind == null) {
        this.showMessage('error', "Chọn mức hưởng");
        return;
      }else {
        benefit = benefitFind.value;
        if(benefit == 2){
          benefitPercent = this.thongTinChungForm.get('PercenBenefit').value;
          if (!benefitPercent) {
            this.showMessage('error', "Nhập mức hưởng");
            return;
          }
          benefitPercent = benefitPercent;
        }
      }
    }

    let data = this.thongTinChungForm.value;
    // Lấy giá trị cho employee model
    let thongTinChung = {
      employeeId: this.employeeId,
      firstName: data.FirstName.trim(),
      lastName: data.LastName.trim(),
      gioiTinh: data.Gender,
      dateOfBirth: data.DateOfBirth ? this.formatDateService.convertToUTCTime(data.DateOfBirth) : null,
      phone: data.Phone.trim(),
      workEmail: data.WorkEmail.trim(),
      trangThaiId: data.TrangThai?.id,
      mission: data.mission?.categoryId,
      employeeType: employeeType,
      benefit: benefit,
      percenBenefit: benefitPercent,
    }

    //List Phòng ban
    let listPhongBanId = this.listSelectedDonVi.map(item => item.organizationId);

    //file image avatar
    let firstIndexOf = this.newLogoUrl ? this.newLogoUrl.indexOf(",") : null;
    let imageBase64 = firstIndexOf ? this.newLogoUrl.substring(firstIndexOf + 1) : null;
    let fileBase64 = {
      "Extension": this.extLogo, /*Định dạng ảnh (jpg, png,...)*/
      "Base64": imageBase64 /*Định dạng base64 của ảnh*/
    }
    this.loading = true;
    let result: any = await this.employeeService.saveThongTinChungThanhVien(thongTinChung, listPhongBanId, fileBase64);
    this.loading = false;

    if (result.statusCode == 200) {
      this.isEdit = false;
      this.thongTinChungForm.disable();
      this.showMessage('success', result.messageCode);
      this.getThongTinChungThanhVien();
      this.dataService.changeMessage("Update success"); //thay đổi message để call lại api getListNote trong component NoteTimeline
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }


  checkCongTacVien(e) {
    this.checked = e.checked;
  }

  changeBenefitDataType(event: GetListBenefitType): void {
    if (event.value == 2) {
      this.isShowPercenBenefit = true;
      // this.thongTinChungForm.get('Benefit').setValue(2);
    }
    else {
      this.isShowPercenBenefit = false;
      // this.thongTinChungForm.get('Benefit').setValue(1);
    }
  }

  getBenefitType() {
    this.employeeService.getBenefitType().subscribe(result => {
      this.listBenefitType = result.getListBenefitType;
    })
  }



  disabledForm() {
    this.isEdit = false;
    this.mapDataToForm(this.thongTinChungClone);
    this.thongTinChungForm.disable();
    this.error = {
      TrangThai: '',
      FirstName: '',
      LastName: '',
      Gender: '',
      DateOfBirth: '',
      OrganizationId: '',
      PositionId: '',
      mission: '',
      Phone: '',
      WorkEmail: '',
    }
  }

  convertDateToTime(date: any) {
    if (date) {
      return this.datePipe.transform(new Date(date), 'HH:mm:ss');
    }
    else {
      return null;
    }
  }

  convertTimeToDate(time: string) {
    if (time) {
      let listTime = time.split(':');
      let hour = parseInt(listTime[0]);
      let minute = parseInt(listTime[1]);
      let second = parseInt(listTime[2]);

      let newDate = new Date();
      newDate.setHours(hour);
      newDate.setMinutes(minute);
      newDate.setSeconds(second);

      return newDate
    }
    else {
      return null;
    }
  }

  openOrgPopup() {
    let listSelectedId = this.listSelectedDonVi.map(item => item.organizationId);
    let selectedId = null;
    if (listSelectedId.length > 0) {
      selectedId = listSelectedId[0]
    }

    let ref = this.dialogService.open(ChonNhieuDvDialogComponent, {
      data: {
        mode: 2,
        selectedId: selectedId
      },
      header: 'Chọn đơn vị',
      width: '40%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "350px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result?.length > 0) {
          this.listSelectedDonVi = result;
          let listSelectedTenDonVi = this.listSelectedDonVi.map(x => x.organizationName);
          this.thongTinChungForm.controls.OrganizationId.patchValue(listSelectedTenDonVi);
          this.error['OrganizationId'] = null;
        }
        else {
          this.listSelectedDonVi = [];
          this.thongTinChungForm.controls.OrganizationId.patchValue(null);
          this.error['OrganizationId'] = 'Không được để trống';
        }
      }
    });
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
};
