import { Component, OnInit, ElementRef, HostListener, ViewChild, Renderer2, ChangeDetectorRef, AfterViewChecked } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

import { ContactModel } from '../../../../shared/models/contact.model';
import { UserModel } from '../../../../shared/models/user.model';
import { FormGroup, FormControl, FormBuilder, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { OrganizationService } from '../../../../shared/services/organization.service';
import { PositionService } from '../../../../shared/services/position.service';
import { EmployeeService } from '../../../services/employee.service';
import { EmailConfigService } from '../../../../admin/services/email-config.service';
import { ActivatedRoute, Router } from '@angular/router';
import { HandleFileService } from './../../../../shared/services/handleFile.service';
import { FormatDateService } from './../../../../shared/services/formatDate.services';
import { ValidaytorsService } from './../../../../shared/services/validaytors.service';

import { CreateEmployeeModel } from '../../../models/employee.model';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { DialogService } from 'primeng/dynamicdialog';

import { MessageService } from 'primeng/api';
import { ChonNhieuDvDialogComponent } from './../../../../shared/components/chon-nhieu-dv-dialog/chon-nhieu-dv-dialog.component';
import { GetListBenefitType } from '../../../models/employee-type.model';

@Component({
  selector: 'app-employee-create',
  templateUrl: './employee-create.component.html',
  styleUrls: ['./employee-create.component.css']
})
export class EmployeeCreateComponent implements OnInit, AfterViewChecked {
  /*Khai bao bien*/
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  password: string = this.systemParameterList.find(x => x.systemKey == "DefaultUserPassword").systemValueString;
  isManager: boolean = null;
  employeeId: string = null;
  currentOrganizationId: string = '';

  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới

  submitted: boolean = false;
  listOrganization: Array<any> = [];
  listPosition: Array<any> = [];
  genders = [{
    value: 'NAM', viewValue: 'Nam'
  },
  {
    value: 'NU', viewValue: 'Nữ'
  }];

  error: any = {
    EmployeeCode: '',
    IsAccessable: '',
    FirstName: '',
    LastName: '',
    TenTiengAnh: '',
    Gender: '',
    DateOfBirth: '',
    DanToc: '',
    TonGiao: '',
    OrganizationId: '',
    StartedDate: '',
    PositionId: '',
    ViTriLamViec: '',
    Phone: '',
    WorkEmail: '',
  }

  defaultLimitedFileSize = parseInt(this.systemParameterList.find(x => x.systemKey == "LimitedFileSize").systemValueString) * 1000000; //1MB = 1000000 byte
  uploadedFiles = [];
  currentLogoUrl: any = '/assets/images/no-avatar.png';
  newLogoUrl: any = null;
  extLogo: any = null;

  roles: Array<string> = [];
  isAccessable: Boolean = true;
  toastMessage: string;
  empAccountList: Array<any> = [];
  empIdentityList: Array<any> = [];
  orgNameDisplay: string;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';

  employeeModel = new CreateEmployeeModel()
  contactModel = new ContactModel()
  userModel: UserModel = {
    UserId: this.emptyGuid,
    Password: this.password,
    UserName: '',
    EmployeeCode: '',
    EmployeeId: '',
    Disabled: false,
    CreatedById: 'DE2D55BF-E224-4ADA-95E8-7769ECC494EA',
    CreatedDate: new Date(),
    UpdatedById: null,
    UpdatedDate: null,
    Active: true
  };

  createEmpForm: FormGroup;
  actionAdd: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  /*Ket thuc*/

  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  fixed: boolean = false;
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageXOffset;
    if (num > 100) {
      this.fixed = true;
    } else {
      this.fixed = false;
    }
  }

  listDonVi: Array<any> = [];
  listSelectedDonVi: Array<any> = [];
  /* Tạo cộng tác viên */
  checked: boolean = false;
  listBenefitType: GetListBenefitType[] = [];
  isShowPercenBenefit: boolean = false;
  /*Khai bao contructor*/
  constructor(private translate: TranslateService,
    private getPermission: GetPermission,
    private organizationService: OrganizationService,
    private positionService: PositionService,
    private employeeService: EmployeeService,
    private route: ActivatedRoute,
    private router: Router,
    public builder: FormBuilder,
    private emailConfigService: EmailConfigService,
    private renderer: Renderer2,
    private messageService: MessageService,
    public dialogService: DialogService,
    public changeRef: ChangeDetectorRef,
    private handleFileService: HandleFileService,
    private domSanitizer: DomSanitizer,
    private formatDateService: FormatDateService,
    private validaytorsService: ValidaytorsService
  ) {
    this.renderer.listen('window', 'click', (e: Event) => {
      /**
       * Only run when toggleButton is not clicked
       * If we don't check this, all clicks (even on the toggle button) gets into this
       * section which in the result we might never see the menu open!
       * And the menu itself is checked here, and it's where we check just outside of
       * the menu and button the condition abbove must close the menu
       */
      if (this.toggleButton && this.notifi) {
        if (!this.toggleButton.nativeElement.contains(e.target) &&
          !this.notifi.nativeElement.contains(e.target) &&
          !this.save.nativeElement.contains(e.target) &&
          !this.saveAndCreate.nativeElement.contains(e.target)) {
          this.isOpenNotifiError = false;
        }
      }
    });
    translate.setDefaultLang('vi');
  }
  /*Ket thuc*/

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Notification:', detail: detail };
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  /*Function chay khi page load*/
  async ngOnInit() {
    this.initForm();
    let resource = "hrm/employee/create/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.showMessage('warn', 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ');
      this.router.navigate(['/home']);
    }
    else {
      this.getBenefitType();
      this.getMasterDataEmployeeDetail();

      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
      this.employeeId = JSON.parse(localStorage.getItem('auth')).EmployeeId;
      // let result: any = await this.organizationService.getOrganizationByEmployeeId(this.employeeId);
      // this.currentOrganizationId = result.organization.organizationId;

      // this.contactModel.Gender = 'NAM';
      // this.organizationService.getAllOrganization().subscribe(response => {
      //   let result = <any>response;
      //   this.listDonVi = result.listAll;
      // });

      // this.positionService.getAllPosition().subscribe(response => {
      //   let result = <any>response;
      //   this.roles = result.listPosition;
      // });

      // await this.getAllEmpAccount();
      // await this.getAllEmpIdentity();
    }
  }

  async getMasterDataEmployeeDetail() {
    try {
      this.loading = true;
      let result: any = await this.employeeService.getMasterDataEmployeeDetail();
      this.loading = false;

      if (result.statusCode == 200) {
        this.listOrganization = result.listOrganization;
        this.listPosition = result.listPosition;
      }
      else {
        this.showMessage('error', result.messageCode);
      }
    }
    catch (e) {
      this.loading = false;
      this.showMessage('error', e);
    }
  }

  initForm() {
    let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
    this.createEmpForm = new FormGroup({
      EmployeeCode: new FormControl('', [Validators.required, Validators.pattern(this.validaytorsService.regex.username), checkDuplicateUsername(this.empAccountList), Validators.maxLength(250), forbiddenSpaceText]),
      IsAccessable: new FormControl('true'),
      FirstName: new FormControl('', [Validators.required, Validators.maxLength(250), forbiddenSpaceText]),
      LastName: new FormControl('', [Validators.required, Validators.maxLength(250), forbiddenSpaceText]),
      TenTiengAnh: new FormControl('', [Validators.maxLength(250), forbiddenSpaceText]),
      Gender: new FormControl('NAM'),
      DateOfBirth: new FormControl('', [Validators.required]),
      OrganizationId: new FormControl({ value: null, disabled: true }, [Validators.required]),
      Phone: new FormControl('', [Validators.required, Validators.pattern(this.getPhonePattern())]),
      WorkEmail: new FormControl('', [Validators.required, Validators.pattern(emailPattern)]),
      Benefit: new FormControl(null),
      PercenBenefit: new FormControl(null),
      EmployeeType: new FormControl(null),
    });
  }
  /*Ket thuc*/

  /*Quay tro lai Employee List*/
  goBack() {
    this.router.navigate(['/employee/list']);
  }
  /*Ket thuc*/

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

  getBenefitType() {
    this.employeeService.getBenefitType().subscribe(result => {
      this.listBenefitType = result.getListBenefitType;
    })
  }

  changeBenefitDataType(event: GetListBenefitType): void {
    if (event.value == 2) {
      this.isShowPercenBenefit = true;
      this.employeeModel.Benefit = 2
    }
    else {
      this.isShowPercenBenefit = false;
      this.employeeModel.Benefit = 1
    }
  }
  /*Tao Employee moi*/
  @HostListener('submit', ['$event'])
  createEmployee(value: boolean) {

    this.submitted = true;

    let OrganizationId = this.createEmpForm.get('OrganizationId').value;

    if (!this.createEmpForm.valid || !OrganizationId) {
      if (this.createEmpForm.get('EmployeeCode').errors?.required || this.createEmpForm.get('EmployeeCode').errors?.forbiddenSpaceText) {
        this.error['EmployeeCode'] = 'Không được để trống';
      } else if (this.createEmpForm.get('EmployeeCode').hasError('duplicateCode')) {
        this.error['EmployeeCode'] = 'Mã nhân viên đã tồn tại';
      } else if (this.createEmpForm.get('EmployeeCode').errors?.maxlength) {
        this.error['EmployeeCode'] = 'Không vượt quá 250 kí tự';
      } else if (this.createEmpForm.get('EmployeeCode').errors?.pattern) {
        this.error['EmployeeCode'] = 'Mã nhân viên không đúng định dạng';
      }

      if (this.createEmpForm.get('FirstName').errors?.required || this.createEmpForm.get('FirstName').errors?.forbiddenSpaceText) {
        this.error['FirstName'] = 'Không được để trống';
      } else if (this.createEmpForm.get('FirstName').errors?.maxlength) {
        this.error['FirstName'] = 'Không vượt quá 250 kí tự';
      }

      if (this.createEmpForm.get('LastName').errors?.required || this.createEmpForm.get('LastName').errors?.forbiddenSpaceText) {
        this.error['LastName'] = 'Không được để trống';
      } else if (this.createEmpForm.get('LastName').errors?.maxlength) {
        this.error['LastName'] = 'Không vượt quá 250 kí tự';
      }


      if (this.createEmpForm.get('DateOfBirth').errors?.required) {
        this.error['DateOfBirth'] = 'Không được để trống';
      }

      if (this.createEmpForm.get('Phone').errors?.required) {
        this.error['Phone'] = 'Không được để trống';
      } else if (this.createEmpForm.get('Phone').errors?.pattern) {
        this.error['Phone'] = 'Không đúng định dạng';
      }


      if (this.createEmpForm.get('WorkEmail').errors?.required) {
        this.error['WorkEmail'] = 'Không được để trống';
      } else if (this.createEmpForm.get('WorkEmail').errors?.pattern) {
        this.error['WorkEmail'] = 'Không đúng định dạng';
      }

      if (!OrganizationId) {
        this.error['OrganizationId'] = 'Không được để trống';
        return;
      } else {
        this.error['OrganizationId'] = null;
      }
      return;
    } else {
      if (!this.checked) {
        this.employeeModel.EmployeeType = 2;
      } else {
        this.employeeModel.EmployeeType = 1;
        var benefit = this.createEmpForm.get('Benefit').value;
        if (benefit == null) {
          this.showMessage('error', "Chọn mức hưởng");
          return;
        }
        this.employeeModel.Benefit = benefit.value;
        if (this.employeeModel.Benefit == 2) {
          var benefitPercent = this.createEmpForm.get('PercenBenefit').value;
          if (!benefitPercent) {
            this.showMessage('error', "Nhập mức hưởng");
            return;
          }
          this.employeeModel.PercenBenefit = benefitPercent;
        }

      }
      let data = this.createEmpForm.value;
      // Lấy giá trị cho employee model
      this.employeeModel.CreatedDate = new Date();
      this.isAccessable = data.IsAccessable == 'true' ? true : false;
      this.employeeModel.EmployeeCode = data.EmployeeCode?.trim();

      // Lấy giá trị cho contact employee
      this.contactModel.ContactId = this.emptyGuid;
      this.contactModel.ObjectId = this.emptyGuid;
      this.contactModel.FirstName = data.FirstName.trim();
      this.contactModel.LastName = data.LastName.trim();
      this.contactModel.Gender = data.Gender.trim();
      this.contactModel.DateOfBirth = data.DateOfBirth ? this.formatDateService.convertToUTCTime(data.DateOfBirth) : null;
      this.contactModel.Phone = data.Phone.trim();
      this.contactModel.WorkEmail = data.WorkEmail.trim();

      this.userModel.EmployeeCode = data.EmployeeCode.trim();

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
      this.awaitResult = true;
      this.employeeService.createEmployee(this.employeeModel, this.contactModel, this.userModel, this.isAccessable, listPhongBanId, false, fileBase64).subscribe(response => {
        let result = <any>response;
        this.loading = false;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.showMessage("success", "Tạo nhân viên thành công");

          //Lưu và thêm mới
          if (value) {
            this.resetFieldValue();
            this.awaitResult = false;
          }
          //Lưu
          else {
            this.resetFieldValue();
            this.router.navigate(['/employee/detail', { employeeId: result.employeeId, contactId: result.contactId }]);
            this.awaitResult = false;
          }
        }
        else {
          this.showMessage('error', result.messageCode);
          this.awaitResult = false;
        };
      });
    }
  }
  /*Ket thuc*/

  convert_vi_to_en(str) {
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/Đ/g, "D");
    str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, " ");
    str = str.replace(/  +/g, ' ');
    return str;
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
          this.createEmpForm.controls.OrganizationId.patchValue(listSelectedTenDonVi);
          this.error['OrganizationId'] = null;
        }
        else {
          this.listSelectedDonVi = [];
          this.createEmpForm.controls.OrganizationId.patchValue(null);
          this.error['OrganizationId'] = 'Không được để trống';
        }
      }
    });
  }

  //Function reset toàn bộ các value đã nhập trên form
  resetFieldValue() {
    // this.listSelectedDonVi = [];
    // this.createEmpForm.reset();
    // this.employeeModel.PositionId = '';
    // this.employeeModel.OrganizationId = '';
    // this.contactModel.Gender = 'NAM';
    // this.isAccessable = true;
    // this.createEmpForm.controls['formPositionId'].setValue('');
    // this.createEmpForm.controls['formOrganizationId'].setValue([]);
    // this.createEmpForm.controls['formGender'].setValue(this.genders[0]);
    // this.createEmpForm.controls['formIsAccess'].setValue(true);
  }
  //Kết thúc

  /*Function lay ra toan bo Employee Account de check Account trung lap*/
  async getAllEmpAccount() {
    var result: any = await this.employeeService.getAllEmpAccount();
    this.empAccountList = result.empAccountList;
  }
  /*Ket thuc*/

  /*Function lay ra toan bo Employee Account de check Account trung lap*/
  async getAllEmpIdentity() {
    var result: any = await this.employeeService.getAllEmpIdentity(this.emptyGuid);
    this.empIdentityList = result.empIdentityList;
  }

  /*Ket thuc*/
  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  ngOnDestroy() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
    }
  }

  cancel() {
    this.router.navigate(['/employee/list']);
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  ngAfterViewChecked() {
    this.changeRef.detectChanges();
  }

  checkCongTacVien(e) {
    this.checked = e.checked;
  }
}

function checkDuplicateUsername(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value != undefined) {
      if (array.indexOf(control.value.toLowerCase()) !== -1 && control.value.toLowerCase() !== "") {
        return { 'checkDuplicateUsername': true };
      }
      return null;
    }
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
