import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { ContactService } from '../../../shared/services/contact.service';

//PrimeNg
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';

class OtherContact {
  contactId: string;
  objectId: string;
  objectType: string;
  firstName: string;
  lastName: string;
  contactName: string;
  gender: string;
  genderName: string;
  dateOfBirth: Date;
  address: string;
  role: string; //Chức vụ
  phone: string;
  email: string;
  other: string;  //Thông tin khác
  provinceId: string;
  districtId: string;
  wardId: string;
}

interface Province {
  provinceId: string;
  provinceName: string;
}

interface District {
  districtId: string;
  districtName: string;
  provinceId: string;
}

interface Ward {
  wardId: string;
  wardName: string;
  districtId: string;
}

interface ResultDialog {
  status: boolean,  //TRUE: Lưu thành công, FALSE: Lưu thất bại
  listContact: Array<OtherContact>  //list câu hỏi và câu trả lời sau khi thêm/sửa
}

@Component({
  selector: 'app-contactpopup',
  templateUrl: './contactpopup.component.html',
  styleUrls: ['./contactpopup.component.css']
})
export class ContactpopupComponent implements OnInit {
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  auth: any = JSON.parse(localStorage.getItem("auth"));
  userId = this.auth.UserId;
  loading: boolean = false;

  listGenders = [{ code: 'NAM', name: 'Nam' }, { code: 'NU', name: 'Nữ' }];
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  contact: OtherContact = new OtherContact();

  createContactForm: FormGroup;

  genderControl: FormControl;
  firstNameControl: FormControl;
  lastNameControl: FormControl;
  emailContactControl: FormControl;
  phoneContactControl: FormControl;
  roleControl: FormControl;
  otherControl: FormControl;
  dateOfBirthControl: FormControl;
  addressControl: FormControl;
  provinceControl: FormControl;
  districtControl: FormControl;
  wardControl: FormControl;

  listProvince: Array<Province> = [];
  listDistrict: Array<District> = [];
  listWard: Array<Ward> = [];

  constructor(
    private translate: TranslateService,
    private contactService: ContactService,
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
    private messageService: MessageService) 
  {
    this.contact = this.config.data.contact;
  }

  ngOnInit() {
    let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';

    this.genderControl = new FormControl(null);
    this.firstNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText, Validators.maxLength(100)]);
    this.lastNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText, Validators.maxLength(50)]);
    this.roleControl = new FormControl(null, [Validators.maxLength(100)]);
    this.emailContactControl = new FormControl(null, [Validators.pattern(emailPattern), Validators.maxLength(100)]);
    this.phoneContactControl = new FormControl(null, [Validators.required, Validators.pattern(this.getPhonePattern()), Validators.maxLength(50)]);
    this.otherControl = new FormControl(null, [Validators.maxLength(500)]);
    this.dateOfBirthControl = new FormControl(null);
    this.addressControl = new FormControl(null);
    this.provinceControl = new FormControl(null);
    this.districtControl = new FormControl(null);
    this.wardControl = new FormControl(null);

    this.createContactForm = new FormGroup({
      genderControl: this.genderControl,
      firstNameControl: this.firstNameControl,
      lastNameControl: this.lastNameControl,
      emailContactControl: this.emailContactControl,
      phoneContactControl: this.phoneContactControl,
      roleControl: this.roleControl,
      otherControl: this.otherControl,
      dateOfBirthControl: this.dateOfBirthControl,
      addressControl: this.addressControl,
      provinceControl: this.provinceControl,
      districtControl: this.districtControl,
      wardControl: this.wardControl
    });

    this.contactService.getAddressByContactId(this.contact.contactId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listProvince = result.listProvince;
        this.listDistrict = result.listDistrict;
        this.listWard = result.listWard;

        if (this.contact.contactId) {
          //Cập nhật
          let toSelectedGender = this.listGenders.find(x => x.code == this.contact.gender);
          if (toSelectedGender) {
            this.genderControl.setValue(toSelectedGender);
          }
          
          this.firstNameControl.setValue(this.contact.firstName);
          this.lastNameControl.setValue(this.contact.lastName);
          this.emailContactControl.setValue(this.contact.email);
          this.phoneContactControl.setValue(this.contact.phone);
          this.roleControl.setValue(this.contact.role);
          this.otherControl.setValue(this.contact.other);
          this.dateOfBirthControl.setValue(this.contact.dateOfBirth == null ? null : new Date(this.contact.dateOfBirth));
          this.addressControl.setValue(this.contact.address);
          let province = this.listProvince.find(x => x.provinceId == this.contact.provinceId);
          this.provinceControl.setValue(province);
          let district = this.listDistrict.find(x => x.districtId == this.contact.districtId);
          this.districtControl.setValue(district);
          let ward = this.listWard.find(x => x.wardId == this.contact.wardId);
          this.wardControl.setValue(ward);
        }
      }
      else {
        let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: result.messageCode};
        this.showMessage(msg);
      }
    });
  }

  //Thêm hoặc Cập nhật người liên hệ
  awaitResponse: boolean = false;
  save() {
    if (!this.createContactForm.valid) {
      Object.keys(this.createContactForm.controls).forEach(key => {
        if (this.createContactForm.controls[key].valid === false) {
          this.createContactForm.controls[key].markAsTouched();
        }
      });
    } else {
      this.contact.firstName = this.firstNameControl.value.trim();
      this.contact.lastName = this.lastNameControl.value.trim();
      let toSelectedGender = this.genderControl.value;
      if (toSelectedGender) {
        this.contact.gender = toSelectedGender.code;
      } else {
        this.contact.gender = null;
      }
      
      if (this.emailContactControl.value) {
        this.contact.email = this.emailContactControl.value.trim();
      } else {
        this.contact.email = null;
      }

      this.contact.phone = this.phoneContactControl.value.trim();

      if (this.roleControl.value) {
        this.contact.role = this.roleControl.value.trim();
      } else {
        this.contact.role = null;
      }

      if (this.otherControl.value) {
        this.contact.other = this.otherControl.value.trim();
      } else {
        this.contact.other = null;
      }

      let dateOfBirth = this.dateOfBirthControl.value;
      if (dateOfBirth) {
        dateOfBirth = convertToUTCTime(dateOfBirth);
      }
      this.contact.dateOfBirth = dateOfBirth;

      this.contact.address = this.addressControl.value?.trim();

      this.contact.provinceId = this.provinceControl.value?.provinceId;
      this.contact.districtId = this.districtControl.value?.districtId;
      this.contact.wardId = this.wardControl.value?.wardId;

      this.awaitResponse = true;
      this.contactService.createContact(this.contact).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          let resultDialog: ResultDialog = {
            status: true,
            listContact: result.listContact
          };

          this.ref.close(resultDialog);
        } else {
          this.awaitResponse = false;
          let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: result.messageCode};
          this.showMessage(msg);
        }
      });
    }
  }

  cancel() {
    this.ref.close();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType"); 
    return phonePatternObj.systemValueString;
  }

  changeProvince() {
    let province: Province = this.provinceControl.value;
    
    if (province) {
      this.listDistrict = [];
      this.listWard = [];

      this.contactService.getAddressByChangeObject(province.provinceId, 1).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.listDistrict = result.listDistrict;
        }
        else {
          let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: result.messageCode};
          this.showMessage(msg);
        }
      });
    }
    else {
      this.listDistrict = [];
      this.listWard = [];
    }  
  }

  changeDistrict() {
    let district: District = this.districtControl.value;

    if (district) {
      this.listWard = [];

      this.contactService.getAddressByChangeObject(district.districtId, 2).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.listWard = result.listWard;
        }
        else {
          let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: result.messageCode};
          this.showMessage(msg);
        }
      });
    }
    else {
      this.listWard = [];
    }
  }

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

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
