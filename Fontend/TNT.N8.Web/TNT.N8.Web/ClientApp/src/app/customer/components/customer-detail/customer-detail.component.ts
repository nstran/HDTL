import { Component, OnInit, ElementRef, ViewChild, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { Location } from '@angular/common';
import * as $ from 'jquery';
import { TranslateService } from '@ngx-translate/core';
import { CategoryService } from "../../../shared/services/category.service";
import { CustomerService } from "../../services/customer.service";
import { ContactService } from "../../../shared/services/contact.service";
import { WardService } from '../../../shared/services/ward.service';
import { ProvinceService } from '../../../shared/services/province.service';
import { DistrictService } from '../../../shared/services/district.service';
import { ImageUploadService } from '../../../shared/services/imageupload.service';

import { CustomerModel } from "../../models/customer.model";
import { ContactModel } from "../../../shared/models/contact.model";
import { NoteModel } from '../../../shared/models/note.model';
import { NoteService } from '../../../shared/services/note.service';
import { BankService } from '../../../shared/services/bank.service';
import { EmployeeService } from "../../../employee/services/employee.service";
import { CustomerCareService } from '../../services/customer-care.service';
import { GetPermission } from '../../../shared/permission/get-permission';

import { MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { ConfirmationService } from 'primeng/api';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { SendEmailModel } from '../../../admin/models/sendEmail.model';
import { DialogService } from 'primeng/dynamicdialog';
import { EmailConfigService } from '../../../admin/services/email-config.service';
import { GoogleService } from '../../../shared/services/google.service';

import { WarningComponent } from '../../../shared/toast/warning/warning.component';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { TemplatePreviewEmailComponent } from '../../../shared/components/template-preview-email/template-preview-email.component';

declare var google: any;

interface Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefault: boolean;
}

interface Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
}

interface Province {
  provinceId: string;
  provinceName: string;
}

interface Note {
  active: boolean;
  createdById: string;
  createdDate: Date;
  description: string;
  noteDocList: Array<NoteDocument>;
  noteId: string;
  noteTitle: string;
  objectId: string;
  objectType: string;
  responsibleAvatar: string;
  responsibleName: string;
  type: string;
  updatedById: string;
  updatedDate: Date;
}

interface NoteDocument {
  active: boolean;
  base64Url: string;
  createdById: string;
  createdDate: Date;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  noteDocumentId: string;
  noteId: string;
  updatedById: string;
  updatedDate: Date;
}


@Component({
  selector: 'app-customer-detail',
  templateUrl: './customer-detail.component.html',
  styleUrls: ['./customer-detail.component.css'],
  providers: [GoogleService]
})
export class CustomerDetailComponent implements OnInit {
  /*Khai báo biến*/
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  auth: any = JSON.parse(localStorage.getItem("auth"));

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionSMS: boolean = true;
  actionEmail: boolean = true;
  actionImport: boolean = true;
  /*END*/

  defaultNumberType = this.getDefaultNumberType();
  loading: boolean = false;
  customerTypeString: string = '';
  listProvince: Array<Province> = [];
  noteContent: string = '';
  customerNameLabel: string = '';
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  noteDocumentModel: NoteDocumentModel = new NoteDocumentModel();
  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  listUpdateNoteDocument: Array<NoteDocument> = [];
  noteHistory: Array<Note> = [];
  listGenders = [{ categoryCode: 'NAM', categoryName: 'Nam' }, { categoryCode: 'NU', categoryName: 'Nữ' }];
  customerId: string = '';
  noteId: string = null;
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  customerModel: CustomerModel = new CustomerModel();
  contactModel: ContactModel = new ContactModel();


  customerCodeOld: string = "";
  countCustomer: any;
  isExist: boolean = true;
  listCustomerCode: any = []
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  listSelect: Array<boolean> = [true, false, false, false, false, false, false, false];

  @ViewChild('fileUpload') fileUpload: FileUpload;

  /*Form thông tin cơ bản*/
  customerBaseForm: FormGroup;

  cusFirstNameControl: FormControl;
  cusCodeControl: FormControl;
  cusLastNameControl: FormControl;
  /*End*/

  /*Form thông tin chung KH Cá nhân*/
  customerPersonalInforForm: FormGroup;

  genderControl: FormControl;
  birthDayControl: FormControl;
  phoneControl: FormControl;
  emailControl: FormControl;
  provinceControl: FormControl;
  addressControl: FormControl;
  /*End*/

  /*Table*/
  colsFile: any;
  /*End*/

  /* create valiable add class scroll */
  fixed: boolean = false;
  withFiexd: string = "";

  //map
  options: any = {};
  overlays: any[] = [];
  map: google.maps.Map;
  zoom: number = 16;

  point: number = 0; //điểm tích lũy
  payPoint: number = 0; //điểm đã thanh toán

  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };

  validEmailCustomer: boolean = true;

  validPhoneCustomer: boolean = true;

  listCustomerLead: Array<any> = [];

  listCustomerQuote: Array<any> = [];

  applicationName = this.systemParameterList.find(x => x.systemKey == 'ApplicationName').systemValueString;
  isEdit: boolean = false;
  cusGroupLableName: string = '';
  cusStatusLableName: string = '';
  cusStatusCareLableName: string = '';
  personInChargeLableName: string = '';
  careStaffLableName: string = '';
  provinceLableName: string = '';
  districtLableName: string = '';
  wardLableName: string = '';
  areaLableName: string = '';
  cusLocalTypeBusinessLableName: string = '';
  cusBusinessSizeLableName: string = '';
  cusPaymentMethodLableName: string = '';
  cusBusinessTypeLableName: string = '';
  cusMainBusinessSectorLableName: string = '';
  cusTypeOfBusinessLableName: string = '';
  cusPerGenderLableName: string = '';
  cusPerCountryLableName: string = '';
  cusPerMaritalStatusLableName: string = '';
  cusPerPositionLableName: string = '';

  khachDuAn: boolean = false;

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private customerService: CustomerService,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    private wardService: WardService,
    private districtService: DistrictService,
    private provinceService: ProvinceService,
    private employeeService: EmployeeService,
    private customerCareService: CustomerCareService,
    private contactService: ContactService,
    private bankService: BankService,
    private router: Router,
    private fb: FormBuilder,
    private imageService: ImageUploadService,
    private noteService: NoteService,
    private el: ElementRef,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private emailConfigService: EmailConfigService,
    private googleService: GoogleService,
    private dialogService: DialogService,
    private snackBar: MatSnackBar,
    public location: Location
  ) {
    this.translate.setDefaultLang('vi');
  }

  async ngOnInit() {
    this.setForm();
    //Check permission
    let resource = "crm/customer/detail";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
      this.snackBar.openFromComponent(WarningComponent, { data: 'Bạn không có quyền truy cập', ... this.warningConfig });
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      if (listCurrentActionResource.indexOf("sms") == -1) {
        this.actionSMS = false;
      }
      if (listCurrentActionResource.indexOf("email") == -1) {
        this.actionEmail = false;
      }
      if (listCurrentActionResource.indexOf("import") == -1) {
        this.actionImport = false;
      }

      this.route.params.subscribe(params => {
        this.customerId = params['customerId'];
      });

      this.getMasterData();
    }
  }


  getMasterData() {
    this.loading = true;
    this.customerService.getCustomerById(this.customerId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listProvince = result.listProvince;

        this.mapDataResponse(result.customer, result.contact, false);

        /*Reshow Time Line */
        this.noteHistory = result.listNote;
        this.handleNoteContent();

        // this.getListCustomerValidate();
        this.listCustomerCode = result.customerCode;
        this.customerBaseForm.get('cusCodeControl').setValidators([Validators.required, checkDuplicateCustomeCode(this.listCustomerCode, this.customerCodeOld)]);
        this.customerBaseForm.updateValueAndValidity();

      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }



  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }

  setForm() {
    /*Table*/
    this.colsFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left' },
      { field: 'documentSize', header: 'Kích thước tài liệu', width: '50%', textAlign: 'left' },
    ];


    this.cusFirstNameControl = new FormControl(null, [Validators.maxLength(100)]);
    this.cusCodeControl = new FormControl(null, [Validators.required, checkDuplicateCode(this.listCustomerCode), forbiddenSpaceText]);
    this.cusLastNameControl = new FormControl(null, [Validators.maxLength(50), Validators.required, forbiddenSpaceText]);

    this.customerBaseForm = new FormGroup({
      cusFirstNameControl: this.cusFirstNameControl,
      cusCodeControl: this.cusCodeControl,
      cusLastNameControl: this.cusLastNameControl,
    });

    let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';

    /*FORM KHÁCH HÀNG CÁ NHÂN*/
    let prc_1 = '[a-zA-Z0-9]+$';

    this.genderControl = new FormControl(null);
    this.birthDayControl = new FormControl(null);
    this.phoneControl = new FormControl(null, [Validators.required, Validators.maxLength(20), Validators.pattern(prc_1)]);
    this.emailControl = new FormControl(null, [Validators.pattern(emailPattern)]);
    this.provinceControl = new FormControl(null);
    this.addressControl = new FormControl(null);

    this.customerPersonalInforForm = new FormGroup({
      genderControl: this.genderControl,
      birthDayControl: this.birthDayControl,
      phoneControl: this.phoneControl,
      emailControl: this.emailControl,
      provinceControl: this.provinceControl,
      addressControl: this.addressControl
    });
    /*END*/

  }

  mapDataResponse(customer: any, contact: any, isEdit: boolean) {
    this.khachDuAn = customer.khachDuAn;

    if (!isEdit) {
      //Điểm tích lũy
      this.point = customer.point;

      this.countCustomer = customer.countCustomerInfo;
      this.customerCodeOld = customer.customerCode;

      this.customerModel = <CustomerModel>({
        CustomerId: this.customerId,
        CustomerCode: customer.customerCode,
        CustomerGroupId: customer.customerGroupId,
        CustomerName: customer.customerName,
        CustomerCareStaff: customer.customerCareStaff,
        StatusId: customer.statusId,
        CustomerServiceLevelId: customer.customerServiceLevelId,
        CustomerServiceLevelName: customer.customerServiceLevelName,
        PersonInChargeId: customer.personInChargeId,
        CustomerType: customer.customerType,
        PaymentId: customer.paymentId,
        MaximumDebtValue: customer.maximumDebtValue,
        MaximumDebtDays: customer.maximumDebtDays,
        MainBusinessSector: customer.mainBusinessSector,
        FieldId: customer.fieldId,
        ScaleId: customer.scaleId,
        TotalSaleValue: customer.totalSaleValue,
        TotalReceivable: customer.totalReceivable,
        NearestDateTransaction: customer.nearestDateTransaction,
        BusinessRegistrationDate: customer.businessRegistrationDate,
        EnterpriseType: customer.enterpriseType,
        BusinessScale: customer.businessScale,
        BusinessType: customer.businessType,
        TotalEmployeeParticipateSocialInsurance: customer.totalEmployeeParticipateSocialInsurance,
        TotalCapital: customer.totalCapital,
        TotalRevenueLastYear: customer.totalRevenueLastYear,
        CreatedById: customer.createdById,
        CreatedDate: customer.createdDate,
        UpdatedById: customer.updatedById,
        UpdatedDate: customer.updatedDate,
        Active: customer.active,
        IsGraduated: customer.isGraduated,
        IsApproval: customer.isApproval,
        ApprovalStep: customer.approvalStep,
        StatusCareId: customer.statusCareId,
        KhachDuAn: customer.khachDuAn
      });

      //Thông tin chi tiết khách hàng
      this.contactModel = <ContactModel>({
        ContactId: contact.contactId,
        ObjectId: contact.objectId,
        ObjectType: contact.objectType,
        FirstName: contact.firstName != null ? contact.firstName.trim() : "",
        LastName: contact.lastName != null ? contact.lastName.trim() : "",
        Phone: contact.phone != null ? contact.phone.trim() : "",
        Email: contact.email != null ? contact.email.trim() : "",
        Address: contact.address != null ? contact.address.trim() : "",
        Gender: contact.gender,
        DateOfBirth: contact.dateOfBirth != null ? new Date(contact.dateOfBirth) : null,
        WorkPhone: contact.workPhone,
        OtherPhone: contact.otherPhone,
        WorkEmail: contact.workEmail,
        OtherEmail: contact.otherEmail,
        IdentityID: contact.identityId,
        AvatarUrl: contact.avatarUrl,
        CountryId: contact.countryId,
        DistrictId: contact.districtId,
        ProvinceId: contact.provinceId,
        WebsiteUrl: contact.websiteUrl,
        WardId: contact.wardId,
        MaritalStatusId: contact.maritalStatusId,
        PostCode: contact.postCode,
        Note: contact.note,
        Role: contact.role,
        TaxCode: contact.taxCode,
        Job: contact.job,
        Agency: contact.agency,
        CompanyName: contact.companyName,
        CompanyAddress: contact.companyAddress,
        CustomerPosition: contact.customerPosition,
        Birthplace: contact.birthplace,
        CreatedById: contact.createdById,
        CreatedDate: contact.createdDate,
        UpdatedById: contact.updatedById,
        UpdatedDate: contact.updatedDate,
        Active: contact.active,
        Other: contact.other,
        Longitude: contact.longitude,
        Latitude: contact.latitude,
        AreaId: contact.areaId,
      });
    }

    //customerType: Khách hàng doanh nghiệp, khách hàng cá nhân, hộ kinh doanh
    switch (this.customerModel.CustomerType) {
      case 1:
        this.customerTypeString = "Khách hàng doanh nghiệp";
        break;

      case 2:
        this.customerTypeString = "Khách hàng cá nhân";
        break;

      case 3:
        this.customerTypeString = "Khách hàng đại lý";
        break;

      default:
        break;
    }

    //Full Name
    this.customerNameLabel = this.customerModel.CustomerName ?? null;

    //Mã khách hàng
    this.cusCodeControl.setValue(this.customerModel.CustomerCode);

    //Họ và Tên đệm
    this.cusFirstNameControl.setValue(this.contactModel.FirstName);

    //Tên
    this.cusLastNameControl.setValue(this.contactModel.LastName);


    /*Khách hàng Cá nhân*/
    if (this.customerModel.CustomerType != 1) {
      //Giới tính
      let toSelectedGender = this.listGenders.find(x => x.categoryCode == this.contactModel.Gender);
      if (toSelectedGender) {
        this.genderControl.setValue(toSelectedGender);
        this.cusPerGenderLableName = toSelectedGender.categoryName;
      }

      /*Thông tin liên hệ*/
      this.birthDayControl.setValue(this.contactModel.DateOfBirth);
      this.phoneControl.setValue(this.contactModel.Phone);
      this.emailControl.setValue(this.contactModel.Email);
      this.genderControl.setValue(this.listGenders.find(x => x.categoryCode == this.contactModel.Gender));
      this.addressControl.setValue(this.contactModel.Address);
      let toSelectedProvince: Province = this.listProvince.find(x => x.provinceId == this.contactModel.ProvinceId);
      this.provinceControl.setValue(toSelectedProvince)

      this.provinceLableName = toSelectedProvince.provinceName;
      /*End*/
    }
    /*End*/
  }



  /*Event thay đổi nội dung ghi chú*/
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  /*Event Thêm các file được chọn vào list file*/
  handleFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= 10000000) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedFiles.push(file);
          }
        }
      }
    }
  }

  /*Event Khi click xóa từng file*/
  removeFile(event) {
    let index = this.uploadedFiles.indexOf(event.file);
    this.uploadedFiles.splice(index, 1);
  }

  /*Event Khi click xóa toàn bộ file*/
  clearAllFile() {
    this.uploadedFiles = [];
  }

  /*Hủy sửa ghi chú*/
  cancelNote() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn hủy ghi chú này?',
      accept: () => {
        this.noteId = null;
        this.noteContent = null;
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.listUpdateNoteDocument = [];
        this.isEditNote = false;
      }
    });
  }

  /*Lưu file và ghi chú vào Db*/
  async saveNote() {
    this.loading = true;
    this.listNoteDocumentModel = [];

    /*Upload file mới nếu có*/
    if (this.uploadedFiles.length > 0) {
      await this.uploadFilesAsync(this.uploadedFiles);

      for (var x = 0; x < this.uploadedFiles.length; ++x) {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = this.uploadedFiles[x].name;
        noteDocument.DocumentSize = this.uploadedFiles[x].size.toString();

        this.listNoteDocumentModel.push(noteDocument);
      }
    }

    let noteModel = new NoteModel();

    if (!this.noteId) {
      /*Tạo mới ghi chú*/

      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.customerId;
      noteModel.ObjectType = 'CUS';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/

      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.customerId;
      noteModel.ObjectType = 'CUS';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();

      //Thêm file cũ đã lưu nếu có
      this.listUpdateNoteDocument.forEach(item => {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = item.documentName;
        noteDocument.DocumentSize = item.documentSize;

        this.listNoteDocumentModel.push(noteDocument);
      });
    }

    this.noteService.createNoteForCustomerDetail(noteModel, this.listNoteDocumentModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.noteContent = null;
        this.listUpdateNoteDocument = [];
        this.noteId = null;
        this.isEditNote = false;

        /*Reshow Time Line */
        this.noteHistory = result.listNote;
        this.handleNoteContent();

        let msg = { severity: 'success', summary: 'Thông báo:', detail: "Lưu ghi chú thành công" };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  async uploadFilesAsync(files: File[]) {
    await this.imageService.uploadFileAsync(files);
  }

  // Upload file to server
  uploadFiles(files: File[]) {
    this.imageService.uploadFile(files).subscribe(response => { });
  }

  /*Xử lý và hiển thị lại nội dung ghi chú*/
  handleNoteContent() {

    this.noteHistory.forEach(element => {
      setTimeout(() => {
        let count = 0;
        if (element.description == null) {
          element.description = "";
        }

        let des = $.parseHTML(element.description);
        let newTextContent = '';
        for (let i = 0; i < des.length; i++) {
          count += des[i].textContent.length;
          newTextContent += des[i].textContent;
        }

        if (count > 250) {
          newTextContent = newTextContent.substr(0, 250) + '<b>...</b>';
          $('#' + element.noteId).find('.short-content').append($.parseHTML(newTextContent));
        } else {
          $('#' + element.noteId).find('.short-content').append($.parseHTML(element.description));
        }

        // $('#' + element.noteId).find('.note-title').append($.parseHTML(element.noteTitle));
        $('#' + element.noteId).find('.full-content').append($.parseHTML(element.description));
      }, 1000);
    });
  }
  /*End*/

  /*Event Mở rộng/Thu gọn nội dung của ghi chú*/
  toggle_note_label: string = 'Mở rộng';
  trigger_node(nodeid: string, event) {
    // noteContent
    let shortcontent_ = $('#' + nodeid).find('.short-content');
    let fullcontent_ = $('#' + nodeid).find('.full-content');
    if (shortcontent_.css("display") === "none") {
      fullcontent_.css("display", "none");
      shortcontent_.css("display", "block");
    } else {
      fullcontent_.css("display", "block");
      shortcontent_.css("display", "none");
    }
    // noteFile
    let shortcontent_file = $('#' + nodeid).find('.short-content-file');
    let fullcontent_file = $('#' + nodeid).find('.full-content-file');
    let continue_ = $('#' + nodeid).find('.continue')
    if (shortcontent_file.css("display") === "none") {
      continue_.css("display", "block");
      fullcontent_file.css("display", "none");
      shortcontent_file.css("display", "block");
    } else {
      continue_.css("display", "none");
      fullcontent_file.css("display", "block");
      shortcontent_file.css("display", "none");
    }
    let curr = $(event.target);

    if (curr.attr('class').indexOf('pi-chevron-right') != -1) {
      this.toggle_note_label = 'Thu gọn';
      curr.removeClass('pi-chevron-right');
      curr.addClass('pi-chevron-down');
    } else {
      this.toggle_note_label = 'Mở rộng';
      curr.removeClass('pi-chevron-down');
      curr.addClass('pi-chevron-right');
    }
  }
  /*End */

  /*Kiểm tra noteText > 250 ký tự hoặc noteDocument > 3 thì ẩn đi một phần nội dung*/
  tooLong(note): boolean {
    if (note.noteDocList.length > 3) return true;
    var des = $.parseHTML(note.description);
    var count = 0;
    for (var i = 0; i < des.length; i++) {
      count += des[i].textContent.length;
      if (count > 250) return true;
    }
    return false;
  }

  openItem(name, url) {
    this.imageService.downloadFile(name, url).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(file);
      } else {
        var fileURL = URL.createObjectURL(file);
        if (fileType.indexOf('image') !== -1) {
          window.open(fileURL);
        } else {
          var anchor = document.createElement("a");
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    }, error => { });
  }

  /*Event Sửa ghi chú*/
  onClickEditNote(noteId: string, noteDes: string) {
    this.noteContent = noteDes;
    this.noteId = noteId;
    this.listUpdateNoteDocument = this.noteHistory.find(x => x.noteId == this.noteId).noteDocList;
    this.isEditNote = true;
  }
  /*End*/

  /*Event Xóa ghi chú*/
  onClickDeleteNote(noteId: string) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa ghi chú này?',
      accept: () => {
        this.loading = true;
        this.noteService.disableNote(noteId).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            let note = this.noteHistory.find(x => x.noteId == noteId);
            let index = this.noteHistory.lastIndexOf(note);
            this.noteHistory.splice(index, 1);

            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa ghi chú thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }
  /*End*/

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: NoteDocument) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listUpdateNoteDocument.indexOf(file);
        this.listUpdateNoteDocument.splice(index, 1);
      }
    });
  }

  /*Event khi download 1 file đã lưu trên server*/
  downloadFile(fileInfor: NoteDocument) {
    this.imageService.downloadFile(fileInfor.documentName, fileInfor.documentUrl).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(file);
      } else {
        var fileURL = URL.createObjectURL(file);
        if (fileType.indexOf('image') !== -1) {
          window.open(fileURL);
        } else {
          var anchor: any = document.createElement("a");
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    });
  }

  /*Lưu thông tin khách hàng*/
  async saveCustomer() {
    if (!this.customerBaseForm.valid) {
      Object.keys(this.customerBaseForm.controls).forEach(key => {
        if (!this.customerBaseForm.controls[key].valid) {
          this.customerBaseForm.controls[key].markAsTouched();
        }
      });
      this.showToast('error', 'Thông báo', 'Hãy nhập đầy đủ thông tin!');
      return;
    }

    if (!this.customerPersonalInforForm.valid) {
      Object.keys(this.customerPersonalInforForm.controls).forEach(key => {
        if (!this.customerPersonalInforForm.controls[key].valid) {
          this.customerPersonalInforForm.controls[key].markAsTouched();
        }
      });
      this.showToast('error', 'Thông báo', 'Hãy nhập đầy đủ thông tin!');
      return;
    }

    /*Form chung*/
    let fullName = '';
    let firstName = this.cusFirstNameControl.value != null ? this.cusFirstNameControl.value.trim() : "";
    let lastName = this.cusLastNameControl.value != null ? this.cusLastNameControl.value.trim() : "";
    fullName = (firstName + " " + lastName).trim();

    this.customerModel.CustomerName = fullName;
    this.customerModel.CustomerCode = this.cusCodeControl.value != null ? this.cusCodeControl.value.trim() : "";

    this.contactModel.FirstName = firstName;
    this.contactModel.LastName = lastName;


    /*End Form chung*/
    if (this.customerModel.CustomerType == 2) {
      /*Khách hàng cá nhân*/
      this.contactModel.Gender = this.genderControl.value.categoryCode;
      this.contactModel.DateOfBirth = this.birthDayControl.value != null ? convertToUTCTime(new Date(this.birthDayControl.value)) : null;
      this.contactModel.Phone = this.phoneControl.value;
      this.contactModel.Email = this.emailControl.value;
      this.contactModel.ProvinceId = this.provinceControl.value != null ? this.provinceControl.value.provinceId : null;
      this.contactModel.Address = this.addressControl.value;

    }

    this.loading = true;
    this.customerService.updateCustomerById(this.customerModel, this.contactModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: "Cập nhập khách hàng thành công" };
        this.showMessage(msg);
        //send email after edit
        let sendMailModel: SendEmailModel = result.sendEmailEntityModel;
        this.emailConfigService.sendEmail(6, sendMailModel).subscribe(reponse => { });
        this.isEdit = false;
        this.getMasterData();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });

  }

  del_customer() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa khách hàng này?',
      accept: () => {
        this.customerService.changeCustomerStatusToDelete(this.customerId, this.auth.UserId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.router.navigate(['/customer/list']);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }


  changeCustomerName() {
    let firstName = this.cusFirstNameControl.value != null ? this.cusFirstNameControl.value.trim() : "";
    let lastName = this.cusLastNameControl.value != null ? this.cusLastNameControl.value.trim() : "";
    this.customerNameLabel = (firstName + " " + lastName).trim();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  customerCodePattern(event: any) {
    const pattern = /^[a-zA-Z0-9-]$/;
    const inputChar = String.fromCharCode(event.charCode);
    if (!pattern.test(inputChar)) {
      event.preventDefault();
    }
  }



  showMessageErr() {
    //Nếu là Email
    if (!this.validEmailCustomer) {
      this.showToast('error', 'Thông báo', 'Email khách hàng đã tồn tại trên hệ thống');
    }
    //Nếu là Số điện thoại
    if (!this.validPhoneCustomer) {
      this.showToast('error', 'Thông báo', 'Số điện thoại khách hàng đã tồn tại trên hệ thống');
    }
  }

  /* Disable Form */
  isEditCustomer() {
    this.isEdit = true;
  }

  /* Enable Form */
  isCancelEditCustomer() {
    this.isEdit = false;
  }

  /** CẮT CHUỖI VÀ THÊM '...' */
  formaterStr(str: string) {
    // nếu chuỗi khác undefined
    if (str != undefined && str != null && str != '') {
      let strArray = str.split(' ');
      let result = '';
      let i = 0;
      // nếu chuỗi là link
      if (str.search('/') > 1) {
        // nếu link dfài hơn 40 kí tự
        if (str.length > 40) {
          result = str.substring(0, str.lastIndexOf('/')) + '/' + '...';
          return result;
        }
        // nếu link ngắn hơn 40 kí tự
        else {
          result = str;
          return result;
        }
      }
      // nếu chuỗi là một chuỗi liền dài hơn 20 kí tự
      else if (str.lastIndexOf(' ') === -1 && str.length > 20) {
        result = str.substring(0, 20)
      }
      // nếu chuỗi ngắn hơn 20 từ
      else if (str.lastIndexOf(' ') <= 20) {
        result = str
      }
      else {
        // nếu chuỗi ngắn hơn hoặc bằng 20 từ
        if (strArray.length <= 20) {
          result = str
        }
        else {
          while (i < 20) {
            result += strArray[i] + ' ';
            if (result.indexOf('.') == result.length - 2) {
              result = result.trim() + '..';
              return result;
            } else if (result.indexOf(',') == result.length - 2) {
              result = result.substring(0, result.indexOf(',')) + '...';
              return result;
            }
            i++;
          }
          result = result.trim();

          if (result.charAt(result.lastIndexOf(' ') - 1) == '.') {
            result = result.substring(0, str.lastIndexOf(' ')) + '..';
          }
          else if (result.charAt(result.lastIndexOf(' ') - 1) == ',') {
            result = result.substring(0, str.lastIndexOf(' ') - 1) + '...'
          } else {
            result = result + '...'
          }
        }
      }
      return result;
    } else {
      return '';
    }
  }

  goBack() {
    this.location.back();
  }

}

function checkDuplicateCode(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null) {
      if (array.indexOf(control.value.toLowerCase()) !== -1 && control.value.toLowerCase() !== "") {
        return { 'checkDuplicateCode': true };
      }
      return null;
    }
  }
}
function checkDuplicateCustomeCode(array: Array<any>, customerCode: any): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined && customerCode !== null && customerCode !== undefined) {
      if (customerCode.toLowerCase() !== control.value.toLowerCase()) {
        if (control.value.trim() !== "") {
          let duplicateCode = array.find(e => e === control.value.trim());
          if (duplicateCode !== undefined) {
            return { 'duplicateCode': true };
          }
        }
      }
    }
    return null;
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

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
