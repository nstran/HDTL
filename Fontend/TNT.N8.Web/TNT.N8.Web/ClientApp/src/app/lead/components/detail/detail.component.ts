import { Component, OnInit, ElementRef, ViewChild, HostListener, AfterViewChecked, ChangeDetectorRef } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, AbstractControl, ValidatorFn } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { SendEmailModel } from '../../../admin/models/sendEmail.model';
import { EmailConfigService } from '../../../admin/services/email-config.service';
import { ForderConfigurationService } from '../../../admin/components/folder-configuration/services/folder-configuration.service';

import { NoteService } from '../../../shared/services/note.service';
import { LeadService } from '../../services/lead.service';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { EmailService } from '../../../shared/services/email.service';
import { TranslateService } from '@ngx-translate/core';

import { LeadModel, leadByIdModel, contactLeadByIdModel } from '../../models/lead.model';
import { ContactModel, contactModel } from '../../../shared/models/contact.model';
import { CategoryModel } from '../../../shared/models/category.model';
import { EmployeeModel } from '../../../shared/models/employee.model';
import { CompanyModel } from '../../../shared/models/company.model';
import { NoteModel } from '../../../shared/models/note.model';
import { SelectionModel } from '@angular/cdk/collections';

import * as $ from 'jquery';
import { GetPermission } from '../../../shared/permission/get-permission';

import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { MessageService, ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { FileUpload } from 'primeng/fileupload';

import { LeadDetailDialogComponent } from '../lead-detail-dialog/lead-detail-dialog.component';
import { CreatContactLeadDialogComponent } from './../creat-contact-lead-dialog/creat-contact-lead-dialog.component';
import { LeadDetailModel, LeadProductDetailProductAttributeValue, leadDetailModel, leadProductDetailProductAttributeValue } from './../../models/leadDetail.Model';
import { LeadTemplateQuickSmsComponent } from '../lead-template-quick-sms/lead-template-quick-sms.component';
import { LeadTemplateQuickEmailComponent } from '../lead-template-quick-email/lead-template-quick-email.component';
import { LeadTemplateQuickGiftComponent } from '../lead-template-quick-gift/lead-template-quick-gift.component';
import { LeadMeetingDialogComponent } from '../lead-meeting-dialog/lead-meeting-dialog.component';
import { QuoteService } from '../../../customer/services/quote.service';
import { CustomerOrder } from '../../../order/models/customer-order.model';

/* MODELS */
class companyModel {
  companyId: string;
  companyName: string;
}

class customerContactModel {
  customerId: string;
  customerFullName: string;
  email: string;
  phone: string;
  address: string;
  addressWard: string;
}

class genderModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class interestedGroupModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class leadTypeModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  employeeCodeName: string;
  isManager: boolean;
  positionId: string;
  organizationId: string;
}

class potentialModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class statusLeadModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class Note {
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
  constructor() {
    this.noteDocList = [];
  }
}

class linkOfDocumentResponse {
  linkOfDocumentId: string;
  linkName: string;
  linkValue: string;
  objectId: string;
  objectType: string;
  active: boolean;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: string;
  createdByName: string;
  typeDocument: string; //"DOC": tài liệu; "LINK": liên kết
  name: string;
  isNewLink: boolean; // phân biệt link mới hoặc link cũ
}

class NoteDocument {
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

class visitCardInforModel {
  leadName: string;
  leadCompany: string;
  leadGender: string;
  customerName: string;
  picName: string;
  potenialName: string;
  statusName: string;
  leadTypeName: string;
  interestedGroupName: string;
  probabilityName: string;
}

class leadContactViewModel {
  phone: string;
  email: string;
  fullAddress: string;
}

class leadGroup {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

interface DateInMonth {
  startDateWeek1: Date,
  endDateWeek1: Date,
  startDateWeek2: Date,
  endDateWeek2: Date,
  startDateWeek3: Date,
  endDateWeek3: Date,
  startDateWeek4: Date,
  endDateWeek4: Date,
  startDateWeek5: Date,
  endDateWeek5: Date,
}

class LeadCareInfor {
  employeeName: string;
  employeePosition: string;
  employeeCharge: string;
  week1: Array<LeadCareForWeek>;
  week2: Array<LeadCareForWeek>;
  week3: Array<LeadCareForWeek>;
  week4: Array<LeadCareForWeek>;
  week5: Array<LeadCareForWeek>;
}

class LeadCareForWeek {
  leadCareId: string;
  employeeCharge: string;
  title: string;
  type: number;
  feedBackStatus: number;
  background: string;
  subtitle: string;
  activeDate: Date
}

class businessType {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class investFund {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class linkOfDocumentRequest {
  LinkOfDocumentId: string;
  LinkName: string;
  LinkValue: string;
  ObjectId: string;
  ObjectType: string;
  Active: boolean;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: string;
  CreatedByName: string;
  IsNewLink: boolean; // phân biệt link mới hoặc link cũ
}

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
  createdById: string;
  createdByName: string;
  createdDate: Date;
  dumpId: any;
  typeDocument: string; //"DOC": tài liệu; "LINK": liên kết
  linkName: string;
  linkValue: string;
  name: string;
  isNewLink: boolean; // phân biệt link mới hoặc link cũ
  linkOfDocumentId: string;
}

class leadReferenceCustomer {
  customerCode: string;
  customerStatus: string;
  customerId: string;
  customerName: string;
  customerType: number;
  personInChargeId: string;
  phone: string;
  address: string;
  email: string;
  workEmail: string;
  investmentFundId: string;
}

class probability {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

interface ResultDetailDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  leadDetailModel: LeadDetailModel,
}

class leadContactConfigDataModel {
  isEdit: boolean; //tạo mới: false; chỉnh sửa: true
  contact: contactModel
}

class resultContactDialog {
  status: boolean;
  contact: contactModel;
}

interface PreviewLeadCare {
  effecttiveFromDate: Date,
  effecttiveToDate: Date,
  sendDate: Date,
  statusName: string,
  previewEmailContent: string,
  previewEmailName: string,
  previewEmailTitle: string,
  previewSmsPhone: string,
  previewSmsContent: string
}

interface LeadCareFeedBack {
  leadCareFeedBackId: string,
  feedBackFromDate: Date,
  feedBackToDate: Date,
  feedBackType: string,
  feedBackCode: string,
  feedBackContent: string,
  leadId: string,
  leadCareId: string,
}

interface DataDialogLeadFeedBack {
  name: string,
  fromDate: Date,
  toDate: Date,
  typeName: string,
  feedBackCode: string,
  feedbackContent: string
}

interface Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefault: boolean;
}

interface LeadMeetingInfor {
  employeeName: string,
  employeePosition: string,
  employeeId: string,
  week1: Array<LeadMeetingForWeek>,
  week2: Array<LeadMeetingForWeek>,
  week3: Array<LeadMeetingForWeek>,
  week4: Array<LeadMeetingForWeek>,
  week5: Array<LeadMeetingForWeek>
}

interface LeadMeetingForWeek {
  leadMeetingId: string,
  employeeId: string,
  title: string,
  subTitle: string,
  background: string,
  startDate: Date,
  startHours: Date
}

class quoteModel {
  leadId: string;
  quoteId: string;
  quoteDate: Date;
  effectiveQuoteDate: number;
  expirationDate: Date;
  amount: number;
  statusId: string;
  quoteStatusName: number;
  note: string;
}

class saleBiddingModel {
  saleBiddingId: string;
  saleBiddingName: string;
  customerName: string;
  valueBid: number;
  personInChargeName: string
}

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

class GeographicalArea {
  geographicalAreaId: string;
  geographicalAreaCode: string;
  geographicalAreaName: string;
}

class StatusSupport {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  children: Array<Category>;
  isComplete: boolean;
  isCurrent: boolean;
  isActive: boolean;

  constructor() {
    this.children = [];
    this.isCurrent = false;
    this.isComplete = false;
    this.isActive = false;
  }
}

class ViewModel {
  tenCoHoi: string;
  customerType: number; //Mã loại khách hàng: 1 (KHDN), 2 (KHCN)
  loaiKhachHang: string; //Khách hàng cá nhân or Khách hàng doanh nghiệp
  tenKhachHang: string; //Tên khách hàng
  phone: string;
  email: string;
  diaChi: string;
  nguoiPhuTrach: string;
  nguonTiemNang: string; //Nguồn tiềm năng
  nhuCauSp: string; //Nhu cầu sp/dv
  loaiHinhDn: string; //Loại hình doanh nghiệp
  xacSuatThang: string; //Xác xuất thắng
  khuVuc: string;
  nhomKhachHang: string;
  trangThai: string;
  giaTriMongDoi: number;
  xacSuat: number;
  doanhSoDuBao: number;
  chiTietYeuCau: string;
  mucDoTiemNang: string;
}

@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class LeadDetailComponent implements OnInit, AfterViewChecked {
  @ViewChild('fileUpload') fileUpload: FileUpload;
  emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  appName = this.systemParameterList.find(x => x.systemKey == 'ApplicationName').systemValueString;
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  userFullName: any = localStorage.getItem("UserFullName");
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  listNoteDocumentModel: Array<NoteDocumentModel> = [];

  fixed: boolean = false;
  withFiexd: string = "";
  withFiexdCol: string = "";
  withColCN: number = 0;
  withCol: number = 0;
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
      var colT = 0;
      if (this.withColCN != width) {
        colT = this.withColCN - width;
        this.withColCN = width;
        this.withCol = $('#parentTH').width();
      }
      this.withFiexdCol = (this.withCol) + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
      this.withCol = $('#parentTH').width();
      this.withColCN = $('#parent').width();
      this.withFiexdCol = "";
    }
  }

  //master data
  listCompany: Array<companyModel> = [];
  listCustomerContact: Array<customerContactModel> = [];
  listEmailLead: Array<string> = [];
  listGender: Array<genderModel> = [];
  listInterestedGroup: Array<interestedGroupModel> = [];
  listLeadType: Array<any> = [{name: 'Khách hàng doanh nghiệp', value: 1}, {name: 'Khách hàng cá nhân', value: 2}];
  listLeadGroup: Array<leadGroup> = [];
  listPersonalInChange: Array<Employee> = [];
  listPersonalInChangeCus: Array<Employee> = [];
  listEmployee: Array<Employee> = [];
  listPhoneLead: Array<string> = [];
  listPotential: Array<potentialModel> = [];
  listStatusLead: Array<statusLeadModel> = [];
  listLeadInterestedGroupMappingId: Array<string> = [];
  listQuoteById: Array<quoteModel> = [];
  listOrder: Array<CustomerOrder> = [];
  listArea: Array<GeographicalArea> = [];
  selectedColumnsLead: Array<quoteModel> = [];
  totalAmountQuote: number = 0;
  listSaleBiddingById: Array<saleBiddingModel> = [];
  totalAmountSaleBidding: number = 0;
  //new
  listBusinessType: Array<businessType> = [];
  listInvestFund: Array<investFund> = [];
  listProbability: Array<probability> = [];
  listLeadReferenceCustomer: Array<leadReferenceCustomer> = [];
  listCurrentReferenceCustomer: Array<leadReferenceCustomer> = [];
  noteHistory: Array<Note> = []
  canCreateSaleBidding: boolean = true;
  canDelete: boolean = true;

  //form
  editLeadForm: FormGroup;
  addLinkForm: FormGroup;

  feedbackForm: FormGroup;
  feedbackCodeControl: FormControl;
  feedbackContentControl: FormControl;
  isGetNotification: FormControl;

  //create company
  isCreateCompany: boolean = false;
  //upload
  uploadedFiles: any[] = [];
  listUpdateNoteDocument: Array<NoteDocument> = [];
  colsFile: any;
  //lead model by id
  leadByIdModel: leadByIdModel = new leadByIdModel();
  contactLeadByIdModel: contactLeadByIdModel = new contactLeadByIdModel()
  //view models
  //view toogle varriable
  lastName: string = '';
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';

  //CSKH
  startDateWeek1: Date = null;
  endDateWeek1: Date = null;
  startDateWeek2: Date = null;
  endDateWeek2: Date = null;
  startDateWeek3: Date = null;
  endDateWeek3: Date = null;
  startDateWeek4: Date = null;
  endDateWeek4: Date = null;
  startDateWeek5: Date = null;
  endDateWeek5: Date = null;
  previewEmail: boolean = false;
  previewSMS: boolean = false;

  monthMeeting: string = '';
  yearMeeting: number = (new Date()).getFullYear();
  startDateWeek1Meeting: Date = null;
  endDateWeek1Meeting: Date = null;
  startDateWeek2Meeting: Date = null;
  endDateWeek2Meeting: Date = null;
  startDateWeek3Meeting: Date = null;
  endDateWeek3Meeting: Date = null;
  startDateWeek4Meeting: Date = null;
  endDateWeek4Meeting: Date = null;
  startDateWeek5Meeting: Date = null;
  endDateWeek5Meeting: Date = null;

  previewLeadCare: PreviewLeadCare = {
    effecttiveFromDate: new Date(),
    effecttiveToDate: new Date(),
    sendDate: new Date(),
    statusName: '',
    previewEmailName: '',
    previewEmailTitle: '',
    previewEmailContent: '',
    previewSmsContent: '',
    previewSmsPhone: '',
  }
  feedback: boolean = false;
  dataDialogLeadFeedBack: DataDialogLeadFeedBack = {
    name: '',
    fromDate: new Date(),
    toDate: new Date(),
    typeName: '',
    feedBackCode: '',
    feedbackContent: ''
  }

  listFeedBackCode: Array<Category> = [];
  leadCareFeedBack: LeadCareFeedBack = {
    leadCareFeedBackId: this.emptyGuid,
    feedBackFromDate: new Date(),
    feedBackToDate: new Date(),
    feedBackType: '',
    feedBackCode: '',
    feedBackContent: '',
    leadId: '',
    leadCareId: '',
  }

  leadMeetingInfor: LeadMeetingInfor = {
    employeeId: '',
    employeeName: '',
    employeePosition: '',
    week1: [],
    week2: [],
    week3: [],
    week4: [],
    week5: []
  }

  //Khai bao cac variable de upload file
  accept = 'image/*, video/*, audio/*, .zip, .rar, .pdf, .xls, .xlsx, .doc, .docx, .ppt, .pptx, .txt';
  files: File[] = [];
  progress: number;
  hasBaseDropZoneOver = false;
  lastFileAt: Date;
  //httpEmitter: Subscription;
  sendableFormData: any;
  maxSize: number = 11000000;
  lastInvalids: any;
  baseDropValid: any;
  dragFiles: any;
  awaitSaveFeedBack: boolean = false;
  //list action in page
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionImport: boolean = true;
  //viewMode
  /* 3 trạng thái view mode:
    1. mặc định: trạng thái Nháp. Code : "DRAFT"
    2. xác nhận: trạng thái Xác nhận. Code : "APPR"
    3. Hủy: trạng thái Hủy. Code "CANC"
  */
  viewModeCode: string = null;

  statusCode: string = null;
  isModeView: boolean = true;
  viewModel = new ViewModel();

  isShowButtonCancel: boolean = false;  //true: Hiện button Hủy; false: Ẩn button Hủy
  isShowButtonDelete: boolean = false;  //true: Hiện button Xóa; false: Ẩn button Xóa
  isShowButtonConfirm: boolean = false; //true: Hiện button Xác nhận; false: Ẩn button Xác nhận
  isShowButtonCreateQuote: boolean = false; //true: Hiện button Tạo báo giá; false: Ẩn button Tạo báo giá
  isShowButtonCreateHst: boolean = false; //true: Hiện button Tạo hồ sơ thầu; false: Ẩn button Tạo hồ sơ thầu
  isShowButtonCreateEdit: boolean = false;  //true: Hiện button Sửa; false: Ẩn button Sửa
  isShowButtonDvn: boolean = false; //true: Hiện button Đặt về nháp; false: Ẩn button Đặt về nháp

  isNotReference: boolean = false; //true: không có object nào gắn với cơ hội; false: có ít nhất 1 object gắn với cơ hội

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  //Khai bao cac variable binding du lieu
  potentialCode = 'MTN';
  interestedCode = 'NHU';
  paymentCode = 'PTO';
  editing: boolean = false;
  expand: boolean = false;
  expandsearch: boolean = false;
  editingInfo: boolean = false;
  editingCompany: boolean = false;
  isEditNote: boolean = false;
  isKCL: boolean = false;
  leadId: string;
  interestedGroupName: string;
  potentialName: string;
  role: string;
  statusName: string;
  fullAddress: string;
  noteContent: string = '';
  noteId: string;
  statusColor: string;
  selectedPic: string;
  selectedPotential: string;
  selectedStatus: string;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  searchNoteKeyword: string;
  searchNoteFromDate: Date;
  searchNoteToDate: Date;
  employees: Array<any> = [];
  potentials: Array<string> = [];
  statuss: Array<string> = [];
  interestedGroups: Array<string> = [];
  paymentMethods: Array<string> = [];
  genderCategory: string = "GENDER";
  genders = [{ categoryCode: 'NAM', categoryName: 'Nam' }, { categoryCode: 'NU', categoryName: 'Nữ' }];
  isManager: boolean = null;
  employeeId: string = '00000000-0000-0000-0000-000000000000';

  //Khai bao cac permission variable
  userPermission: any = localStorage.getItem("UserPermission").split(",");
  auth: any = JSON.parse(localStorage.getItem("auth"));
  currentEmployeeId = this.auth.EmployeeId;
  deleteNotePermission: string = "lead/deletenote";
  editPermission: string = "lead/edit";
  employeeWithSamePermission: Array<any>;
  SelectedEmployeeWithNotiPermissionId: Array<string> = [];

  districts: Array<any> = [];
  wards: Array<any> = [];
  selectedDistricts: Array<any> = [];
  selectedWards: Array<any> = [];
  countLead: number = 0;
  statusCheckDelete: any;
  statusSaleBiddingAndQuote: number;
  isCreateSaleBiddingButton: boolean = true;
  isCreateQuoteButton: boolean = true;
  //table
  cols: any[];
  selectedColumns: any[];
  colsContacts: any[];
  selectedColumnsContact: any[];
  listContact: Array<contactModel> = [];
  listContactReply: Array<contactModel> = [];
  //bảng danh sách báo giá theo cơ hội
  selectedColumnsQuote: any[];
  selectedColumnsOrder: any[];
  colsQuote: any[];
  colsOrder: any[];
  //bảng danh sách file tải lên
  colsDocument: any = [];
  //bảng danh sách hồ sơ thầu
  colsSaleBidding: any[];
  selectedColumnsSaleBidding: any[];
  //popup
  leadDetail: LeadDetailModel = new LeadDetailModel();
  listLeadDetailData: Array<leadDetailModel> = [];
  listLeadDetail: Array<LeadDetailModel> = [];
  listLeadDetailReply: Array<LeadDetailModel> = [];
  listLeadDetailModelOrderBy: Array<LeadDetailModel> = [];

  //file session
  listDocument: Array<FileInFolder> = [];
  listDocumentIdNeedRemove: Array<string> = [];
  listLinkOfDocument: Array<linkOfDocumentResponse> = [];
  file: File[];
  listFile: Array<FileInFolder> = [];

  //Config editor
  editorConfig: any = {
    'editable': this.actionEdit,
    'height': '220px',
    'minHeight': '220px',
    'width': 'auto',
    'minWidth': '0',
    'translate': 'yes',
    'enableToolbar': true,
    'showToolbar': true,
    'placeholder': 'Nhập ghi chú...',
    'toolbar': [
      ['bold', 'italic', 'underline'],
      ['fontName', 'fontSize', 'color'],
      ['justifyLeft', 'justifyCenter', 'justifyRight'],
      ['link', 'unlink'],
    ]
  };
  // Declare employee model
  employeeModel = new EmployeeModel();

  // Declare lead model
  leadModel = new LeadModel();

  // Declare contact model
  contactModel = new ContactModel();

  // Declare contact model
  categoryModel = new CategoryModel();

  // Declare company model
  companyModel = new CompanyModel();

  // Delare note model
  noteModel = new NoteModel();

  // Declare SelectionModel
  selection: SelectionModel<EmployeeModel>;

  loading: boolean = false;
  statusOld: string = '';

  selectedObjectType: string = 'cus';
  listCustomer: Array<leadReferenceCustomer> = [];  //list khách hàng
  listCurrentCustomer: Array<leadReferenceCustomer> = []; //list Khách hàng tiềm năng
  currentCustomer: leadReferenceCustomer;

  /* dialog varriable */
  displayAttachFile: boolean = false;
  displayAttachLink: boolean = false;

  /* Tổng hợp cơ hội */
  SumAmount: number = 0;
  SumProduct: number = 0;

  activeState: boolean[] = [false, false, false, false, false, false, false, false];

  //Trạng thái phụ
  listStatusSupport: Array<Category> = [];
  listTempStatusSupport: Array<Category> = [];
  selectedTempStatusSupport: Category = null;
  listFormatStatusSupport: Array<StatusSupport> = [];

  constructor(
    private translate: TranslateService,
    private leadService: LeadService,
    private getPermission: GetPermission,
    private noteService: NoteService,
    private imageService: ImageUploadService,
    private route: ActivatedRoute,
    private folderService: ForderConfigurationService,
    private router: Router,
    public builder: FormBuilder,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private el: ElementRef,
    private dialogService: DialogService,
    public ref: ChangeDetectorRef
  ) {
    translate.setDefaultLang('vi');
  }

  /*Function chay khi page load*/
  async ngOnInit() {
    let isManager = localStorage.getItem('IsManager');
    this.isManager = isManager == 'true' ? true : false;
    //Check permission
    let resource = "crm/lead/detail";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
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
      if (listCurrentActionResource.indexOf("import") == -1) {
        this.actionImport = false;
      }
      this.initForm();
      this.setTable();

      this.route.params.subscribe(async params => {
        this.leadId = params['leadId'];

        await this.getMasterdata();
        this.displayDistroyButton();
      });
    }
  }

  ngAfterViewChecked() {
    this.ref.detectChanges();
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  initForm() {
    if (this.appName == 'VNS') {
      this.editLeadForm = new FormGroup({
        'LeadName': new FormControl('', [Validators.required, forbiddenSpaceText]),
        // 'Potential': new FormControl(null, [Validators.required]),
        'Pic': new FormControl(null),
        'LeadType': new FormControl(null),
        'Status': new FormControl(null),
        'Interested': new FormControl([], Validators.required),
        'DetailInterested': new FormControl(''),
        'PaymentMethod': new FormControl(null),
        'Group': new FormControl(null),
        'Email': new FormControl('', [Validators.pattern(this.emailPattern)]),
        'Phone': new FormControl('', [Validators.pattern(this.getPhonePattern())]),
        'IdentityId': new FormControl(null),
        'DetailAddress': new FormControl(''),
        //new
        'BusinessType': new FormControl(null), //loai hinh doanh nghiep
        'InvestFund': new FormControl(null, [Validators.required]), //Nguon tiem nang
        'ExpectedSale': new FormControl('0'),
        'Probability': new FormControl(null, [Validators.required]), //xac suat
        'RefCustomer': new FormControl(null),// khach hang,
        'Area': new FormControl(null),
        'Percent': new FormControl('0'), // xắc suất
        'ForecastSales': new FormControl('0') // Doanh số dự báo
      });
    }
    else {
      this.editLeadForm = new FormGroup({
        'LeadName': new FormControl('', [Validators.required, forbiddenSpaceText]),
        'Potential': new FormControl(null, [Validators.required]),
        'Pic': new FormControl(null),
        'LeadType': new FormControl(null),
        'Status': new FormControl(null, [Validators.required]),
        'Interested': new FormControl([], Validators.required),
        'DetailInterested': new FormControl(''),
        'PaymentMethod': new FormControl(null),
        'Group': new FormControl(null),
        'Email': new FormControl('', [Validators.pattern(this.emailPattern)]),
        'Phone': new FormControl('', [Validators.pattern(this.getPhonePattern()), Validators.required]),
        'IdentityId': new FormControl(null),
        'DetailAddress': new FormControl(''),
        //new
        'BusinessType': new FormControl(null), //loai hinh doanh nghiep
        'InvestFund': new FormControl(null, [Validators.required]), //Nguon tiem nang
        'ExpectedSale': new FormControl('0'),
        'Probability': new FormControl(null, [Validators.required]), //xac suat
        'RefCustomer': new FormControl(null),// khach hang,
        'Area': new FormControl(null),
      });
    }

    /*FORM ADD FILE*/
    this.addLinkForm = new FormGroup({
      'NameOfLink': new FormControl('', [Validators.required]),
      'Link': new FormControl('', [Validators.required]),
    });

    /*FORM FEED BACK*/
    this.feedbackCodeControl = new FormControl(null, [Validators.required]);
    this.feedbackContentControl = new FormControl(null, [Validators.required]);

    this.feedbackForm = new FormGroup({
      feedbackCodeControl: this.feedbackCodeControl,
      feedbackContentControl: this.feedbackContentControl
    });
    /*END*/

    this.isGetNotification = new FormControl(null);

    /*Table*/
    this.colsFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left' },
      { field: 'documentSize', header: 'Kích thước tài liệu', width: '50%', textAlign: 'left' },
    ];
  }

  setTable() {
    this.cols = [
      { field: 'ProductCode', header: 'Mã sản phẩm', width: '70px', textAlign: 'left', color: '#f44336' },
      { field: 'ExplainStr', header: 'Tên sản phẩm', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'NameVendor', header: 'Nhà cung cấp', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'Quantity', header: 'Số lượng', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'ProductNameUnit', header: 'Đơn vị tính', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'UnitPrice', header: 'Đơn giá', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'NameMoneyUnit', header: 'Đơn vị tiền', width: '30px', textAlign: 'left', color: '#f44336' },
      { field: 'ExchangeRate', header: 'Tỷ giá', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'Vat', header: 'Thuế GTGT (%)', width: '30px', textAlign: 'right', color: '#f44336' },
      { field: 'DiscountValue', header: 'Chiết khấu', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'SumAmount', header: 'Thành tiền (VND)', width: '60px', textAlign: 'right', color: '#f44336' },
      { field: 'Delete', header: 'Thao tác', width: '60px', textAlign: 'center', color: '#f44336' }
    ];
    this.selectedColumns = this.cols.filter(e => e.field == "move" || e.field == "ProductCode" || e.field == "ExplainStr" || e.field == "Quantity"
      || e.field == "UnitPrice" || e.field == "SumAmount" || e.field == "Delete");

    this.colsContacts = [
      { field: 'firstName', header: 'Tên liên hệ', textAlign: 'left', color: '#f44336' },
      { field: 'genderDisplay', header: 'Giới tính', textAlign: 'center', color: '#f44336' },
      { field: 'dateOfBirth', header: 'Ngày sinh', textAlign: 'right', color: '#f44336' },
      { field: 'role', header: 'Vị trí công việc', textAlign: 'left', color: '#f44336' },
      { field: 'relationShip', header: 'Mối liên hệ', textAlign: 'left', color: '#f44336' },
      { field: 'phone', header: 'Số di động', textAlign: 'right', color: '#f44336' },
      { field: 'email', header: 'Email', textAlign: 'left', color: '#f44336' },
      { field: 'note', header: 'Ghi chú', textAlign: 'left', color: '#f44336' },
      { field: 'Delete', header: 'Thao tác', width: '60px', textAlign: 'center', color: '#f44336' }
    ];
    this.selectedColumnsContact = this.colsContacts.filter(e => e.field == "firstName" || e.field == "genderDisplay" || e.field == "dateOfBirth"
      || e.field == "role" || e.field == "phone" || e.field == "Delete");

    if (this.appName == 'VNS') {
      this.colsQuote = [
        { field: 'quoteCode', header: 'Số báo giá', width: '70px', textAlign: 'left', color: '#f44336' },
        { field: 'quoteDate', header: 'Ngày báo giá', width: '70px', textAlign: 'right', color: '#f44336' },
        { field: 'expirationDate', header: 'Hiệu lực đến ngày', width: '70px', textAlign: 'right', color: '#f44336' },
        { field: 'totalAmountAfterVat', header: 'Tổng tiền sau thuế', width: '70px', textAlign: 'right', color: '#f44336' },
        { field: 'quoteStatusName', header: 'Tình trạng', width: '70px', textAlign: 'center', color: '#f44336' },
        { field: 'note', header: 'Mô tả', width: '70px', textAlign: 'left', color: '#f44336' },
      ];
    } else {
      this.colsQuote = [
        { field: 'quoteCode', header: 'Số báo giá', width: '70px', textAlign: 'left', color: '#f44336' },
        { field: 'quoteDate', header: 'Ngày báo giá', width: '70px', textAlign: 'right', color: '#f44336' },
        { field: 'expirationDate', header: 'Hiệu lực đến ngày', width: '70px', textAlign: 'right', color: '#f44336' },
        { field: 'totalAmount', header: 'Tổng tiền', width: '70px', textAlign: 'right', color: '#f44336' },
        { field: 'quoteStatusName', header: 'Tình trạng', width: '70px', textAlign: 'center', color: '#f44336' },
        { field: 'note', header: 'Mô tả', width: '70px', textAlign: 'left', color: '#f44336' },
      ];
    }

    this.selectedColumnsQuote = this.colsQuote;

    this.colsOrder = [
      { field: 'orderCode', header: 'Số đơn hàng', width: '70px', textAlign: 'left', color: '#f44336' },
      { field: 'orderDate', header: 'Ngày đặt hàng', width: '70px', textAlign: 'right', color: '#f44336' },
      { field: 'customerName', header: 'Khách hàng', width: '70px', textAlign: 'right', color: '#f44336' },
      { field: 'amount', header: 'Giá trị đơn hàng', width: '70px', textAlign: 'right', color: '#f44336' },
      { field: 'orderStatusName', header: 'Trạng thái', width: '70px', textAlign: 'center', color: '#f44336' },
      { field: 'sellerName', header: 'Người phụ trách', width: '70px', textAlign: 'left', color: '#f44336' },
    ];

    this.selectedColumnsOrder = this.colsOrder;

    this.colsSaleBidding = [
      { field: 'saleBiddingName', header: 'Tên gói thầu', width: '70px', textAlign: 'left', color: '#f44336' },
      { field: 'customerName', header: 'Bên mời thầu', width: '70px', textAlign: 'left', color: '#f44336' },
      { field: 'valueBid', header: 'Giá trị thầu', width: '70px', textAlign: 'right', color: '#f44336' },
      { field: 'personInChargeName', header: 'Người phụ trách', width: '70px', textAlign: 'left', color: '#f44336' },
    ]

    this.selectedColumnsSaleBidding = this.colsSaleBidding;

    this.colsDocument = [
      { field: 'name', header: 'Tên tài liệu', width: '25%', textAlign: 'left', type: 'string' },
      { field: 'createdByName', header: 'Người đính kèm', width: '25%', textAlign: 'left', type: 'string' },
      { field: 'createdDate', header: 'Ngày đính kèm', width: '25%', textAlign: 'left', type: 'date' },
      { field: 'size', header: 'Dung lượng', width: '25%', textAlign: 'right', type: 'number' },
    ];

  }

  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch (unit) {
      case -1: {
        result = result;
        break;
      }
      case 0: {
        result = Math.round(result);
        break;
      }
      case 1: {
        result = Math.round(number * 10) / 10;
        break;
      }
      case 2: {
        result = Math.round(number * 100) / 100;
        break;
      }
      case 3: {
        result = Math.round(number * 1000) / 1000;
        break;
      }
      case 4: {
        result = Math.round(number * 10000) / 10000;
        break;
      }
      default: {
        result = result;
        break;
      }
    }
    return result;
  }

  async getMasterdata() {
    this.isModeView = true;
    this.loading = true;
    let result: any = await this.leadService.getDatEditLead(this.leadId, this.auth.UserId);
    this.loading = false;

    if (result.statusCode != 200) {
      this.clearToast();
      this.showToast('error', 'Thông báo', result.messageCode);
      return;
    }
    this.isShowButtonConfirm = result.isShowButtonConfirm;
    this.canCreateSaleBidding = result.canCreateSaleBidding;
    this.canDelete = result.canDelete;
    this.listCompany = result.listCompany;
    this.listCustomerContact = result.listCustomerContact;
    this.listEmailLead = result.listEmailLead;
    this.listLeadInterestedGroupMappingId = result.listLeadInterestedGroupMappingId;
    this.listGender = result.listGender;
    this.listInterestedGroup = result.listInterestedGroup;
    this.listLeadGroup = result.listLeadGroup;
    this.listLinkOfDocument = result.listLinkOfDocument || [];
    this.listFile = result.listFile || [];
    this.listPersonalInChange = result.listPersonalInChange;
    this.listArea = result.listArea;
    this.isShowButtonCancel = result.isShowButtonCancel;
    this.isShowButtonDelete = result.isShowButtonDelete;
    this.isShowButtonCreateQuote = result.isShowButtonCreateQuote;
    this.isShowButtonCreateHst = result.isShowButtonCreateHst;
    this.isShowButtonCreateEdit = result.isShowButtonCreateEdit;
    this.isNotReference = result.isNotReference;
    this.isShowButtonDvn = result.isShowButtonDvn;

    if (!this.isNotReference) {
      this.editLeadForm.disable();
    }
    else {
      this.editLeadForm.enable();
    }

    //Build trạng thái phụ
    this.listStatusSupport = result.listStatusSupport;
    this.listTempStatusSupport = this.listStatusSupport.filter(x => x.categoryCode.indexOf('E') != -1);
    this.buildListStatusSupport(result.statusSupportId);

    if (this.listPersonalInChangeCus.length == 0) {
      this.listPersonalInChangeCus = this.listPersonalInChange;
    }

    this.listEmployee = result.listEmployee;
    this.listEmployee.forEach(item => {
      item.employeeName = item.employeeCode + ' - ' + item.employeeName;
    });
    this.listPhoneLead = result.listPhoneLead;
    this.listPotential = result.listPotential;
    //new
    this.listBusinessType = result.listBusinessType;
    this.listInvestFund = result.listInvestFund;
    this.listProbability = result.listProbability;
    this.listLeadReferenceCustomer = result.listLeadReferenceCustomer;
    this.statusSaleBiddingAndQuote = result.statusSaleBiddingAndQuote;
    this.listQuoteById = result.listQuoteById;
    this.listQuoteById.forEach(quote => {
      this.totalAmountQuote += Number(quote.amount)
    });
    this.listOrder = result.listOrder;
    this.listSaleBiddingById = result.listSaleBiddingById;
    this.listSaleBiddingById.forEach(sale => {
      this.totalAmountSaleBidding += Number(sale.valueBid)
    });
    this.listLeadDetailData = result.listLeadDetail;
    this.listLeadDetailData.forEach(x => {
      this.SumAmount += x.sumAmount;
      x.sumAmountLabor = x.unitLaborNumber * x.unitLaborPrice * x.exchangeRate;
    })
    this.SumProduct = this.listLeadDetailData.length;
    this.listContact = result.listLeadContact;
    this.listContactReply = result.listLeadContact;

    //Map data list sản phẩm dịch vụ
    this.patchLeadDetailData();

    //Map data list liên hệ của Cơ hội
    this.patchLeadContactData();

    this.listDocument = [];

    //Map data list liên kết
    this.patchDefaultDocumentLink();

    //Map data list file đính kèm
    this.patchDefaultDocumentFile();

    this.listStatusLead = result.listLeadStatus;
    this.noteHistory = result.listNote;

    //Xử lý và hiển thị lại nội dung ghi chú
    this.handleNoteContent();

    this.editLeadForm.updateValueAndValidity();

    //lấy thông tin lead và patch data vào form
    this.leadByIdModel = result.leadModel;
    this.contactLeadByIdModel = result.leadContactModel;

    let emp = this.listPersonalInChangeCus.find(e => e.employeeId == this.leadByIdModel.personInChargeId);
    this.editLeadForm.controls['Pic'].setValue(emp);

    //set lead view mode
    let status = this.listStatusLead.find(e => e.categoryId == this.leadByIdModel.statusId);
    if (status) {
      this.statusCode = status.categoryCode;
      this.viewModeCode = status.categoryCode;
    }
    //Nếu status không xác định
    else {
      this.viewModeCode = "DRAFT";
    }

    let customer = this.listLeadReferenceCustomer.find(x => x.customerId == this.leadByIdModel.customerId);
    if (customer) {
      if (customer.customerStatus == 'HDO') {
        await this.changeObjectType('cus', false);
        this.selectedObjectType = 'cus';
      } else if (customer.customerStatus == 'MOI') {
        await this.changeObjectType('lea', false);
        this.selectedObjectType = 'lea';
      }
    } else {
      await this.changeObjectType('cus', false);
      this.selectedObjectType = 'cus';
    }

    this.patchLeadInforToForm(this.leadByIdModel, this.contactLeadByIdModel);
    /*Map data to viewModel*/
    this.viewModel.tenCoHoi = this.contactLeadByIdModel.firstName;
    this.viewModel.customerType = result.customerType;
    this.viewModel.loaiKhachHang = this.listLeadType.find(x => x.value == result.customerType)?.name;
    this.viewModel.phone = this.contactLeadByIdModel.phone;
    this.viewModel.email = this.contactLeadByIdModel.email;
    this.viewModel.diaChi = this.contactLeadByIdModel.address;
    this.viewModel.nguoiPhuTrach = this.listPersonalInChange.find(e => e.employeeId == this.leadByIdModel.personInChargeId)?.employeeCodeName;
    this.viewModel.nguonTiemNang = this.listInvestFund.find(e => e.categoryId == this.leadByIdModel.investmentFundId)?.categoryName;
    this.viewModel.nhuCauSp = this.listInterestedGroup.filter(e => this.listLeadInterestedGroupMappingId.includes(e.categoryId))?.map(x => x.categoryName)?.join(", ");
    this.viewModel.loaiHinhDn = this.listBusinessType.find(e => e.categoryId == this.leadByIdModel.businessTypeId)?.categoryName;
    this.viewModel.xacSuatThang = this.listProbability.find(e => e.categoryId == this.leadByIdModel.probabilityId)?.categoryName;
    this.viewModel.khuVuc = this.listArea.find(c => c.geographicalAreaId == this.leadByIdModel.geographicalAreaId)?.geographicalAreaName;
    this.viewModel.nhomKhachHang = this.listLeadGroup.find(e => e.categoryId == this.leadByIdModel.leadGroupId)?.categoryName;
    this.viewModel.trangThai = this.listStatusLead.find(e => e.categoryId == this.leadByIdModel.statusId)?.categoryName;
    this.viewModel.giaTriMongDoi = this.leadByIdModel.expectedSale;
    this.viewModel.xacSuat = this.leadByIdModel.percent;
    this.viewModel.doanhSoDuBao = this.leadByIdModel.forecastSales;
    this.viewModel.chiTietYeuCau = this.leadByIdModel.requirementDetail;
    this.viewModel.mucDoTiemNang = this.listPotential.find(x => x.categoryId == this.leadByIdModel.potentialId)?.categoryName;
    /*End*/
  }

  goToOrderDetail(orderId: string) {
    let url = this.router.serializeUrl(this.router.createUrlTree(['/order/order-detail', { customerOrderID: orderId }]));
    window.open(url, '_blank');
    // this.router.navigate(['/order/order-detail', { customerOrderID: orderId }]);
  }

  /*Map data list liên kết*/
  patchDefaultDocumentLink() {
    this.listLinkOfDocument.forEach(link => {
      link.name = link.linkName;
      link.typeDocument = "LINK";
      link.isNewLink = false;
      (this.listDocument as any) = [...this.listDocument, link];
    })
  }

  /*Map data list file đính kèm*/
  patchDefaultDocumentFile() {
    /* patch file to table */
    this.listFile.forEach(file => {
      (file as any).name = file.fileName;
      this.listDocument = [...this.listDocument, file];
    });
  }

  goToCustomerDetail(event: any) {
    var customer = this.editLeadForm.get('RefCustomer').value;
    if (customer.customerStatus == 'HDO') {
      var url = this.router.serializeUrl(
        this.router.createUrlTree(['/customer/detail', { customerId: customer.customerId }])
      );
      window.open(url, '_blank');
    }
    else {
      var url = this.router.serializeUrl(
        this.router.createUrlTree(['/customer/potential-customer-detail', { customerId: customer.customerId }])
      );
      window.open(url, '_blank');
    }
  }

  /*Event chuyển loại khách hàng (Khách hàng hoặc Khách hàng tiềm năng)*/
  async changeObjectType(objecType: any, isReset: boolean) {
    if (objecType == 'cus') {
      var customerType = 'HDO';
      let result: any = await this.leadService.getListCustomerByType(this.auth.UserId, customerType);
      this.listCustomer = result.listCustomerByType;
      this.listCustomer.forEach(x => {
        x.customerName = x.customerCode + ' - ' + x.customerName;
      });
      this.listCurrentCustomer = this.listCustomer;
    }
    else if (objecType == 'lea') {
      var customerType = 'MOI';
      let result: any = await this.leadService.getListCustomerByType(this.auth.UserId, customerType);
      this.listCustomer = result.listCustomerByType;
      this.listCustomer = result.listCustomerByType;
      this.listCustomer.forEach(x => {
        x.customerName = x.customerCode + ' - ' + x.customerName;
      });
      this.listCurrentCustomer = this.listCustomer;
    }

    if (isReset) {
      this.editLeadForm.get('RefCustomer').reset();
      this.editLeadForm.get('LeadType').reset();
    }
    else {
      let customerTypeTow = this.listCurrentCustomer.find(x => x.customerId == this.leadByIdModel.customerId);
      this.editLeadForm.get('RefCustomer').setValue(customerTypeTow);
      this.viewModel.tenKhachHang = customerTypeTow?.customerName;

      //loại khách hàng
      let leadType = this.listLeadType.find(e => e.value == customerTypeTow?.customerType);
      this.editLeadForm.get('LeadType').patchValue(leadType ? leadType : null);
      //list khách hàng
      if (leadType) {
        switch (leadType.value) {
          case 2:
            //khach hang ca nhan
            this.listCurrentCustomer = this.listCustomer.filter(e => e.customerType === 2);
            break;
          case 1:
            //khach hang doanh nghiep
            this.listCurrentCustomer = this.listCustomer.filter(e => e.customerType === 1);
            break;
          default:
            break;
        }
      }
    }
  }

  patchLeadInforToForm(leadByIdModel: leadByIdModel, contactLeadByIdModel: contactLeadByIdModel) {
    //tên cơ hội
    this.editLeadForm.get('LeadName').patchValue(contactLeadByIdModel.firstName);
    //người phụ trách
    let personalInChange = this.listPersonalInChange.find(e => e.employeeId == leadByIdModel.personInChargeId);
    this.editLeadForm.get('Pic').patchValue(personalInChange ? personalInChange : null);
    let area = this.listArea.find(c => c.geographicalAreaId == leadByIdModel.geographicalAreaId);
    this.editLeadForm.get('Area').patchValue(area ? area : null);
    let refCustomer = this.listCurrentCustomer.find(e => e.customerId == leadByIdModel.customerId);
    this.editLeadForm.get('RefCustomer').setValue(refCustomer ? refCustomer : null);
    //nhóm khách hàng
    let leadGroup = this.listLeadGroup.find(e => e.categoryId == leadByIdModel.leadGroupId);
    this.editLeadForm.get('Group').patchValue(leadGroup ? leadGroup : null);
    //nhu cầu sản phẩm
    let listInterestedGroup = this.listInterestedGroup.filter(e => this.listLeadInterestedGroupMappingId.includes(e.categoryId));
    this.editLeadForm.get('Interested').patchValue(listInterestedGroup);
    //chi tiết yêu cầu
    let requirementDetail = leadByIdModel.requirementDetail;
    this.editLeadForm.get('DetailInterested').patchValue(requirementDetail ? requirementDetail : '');
    //loại hình doanh nghiệp
    let businessType = this.listBusinessType.find(e => e.categoryId == leadByIdModel.businessTypeId);
    this.editLeadForm.get('BusinessType').patchValue(businessType ? businessType : null);
    //nguồn tiềm năng
    let investFund = this.listInvestFund.find(e => e.categoryId == leadByIdModel.investmentFundId);
    this.editLeadForm.get('InvestFund').patchValue(investFund ? investFund : null);
    //doanh thu mong đợi
    let expectedSale = leadByIdModel.expectedSale;
    this.editLeadForm.get('ExpectedSale').patchValue(expectedSale ? expectedSale : '0');
    // xác suất
    let percent = leadByIdModel.percent;
    this.editLeadForm.get('Percent')?.patchValue(percent ? percent : '0');
    // Doanh số dự báo
    let forecastSales = leadByIdModel.forecastSales;
    this.editLeadForm.get('ForecastSales')?.patchValue(forecastSales ? forecastSales : '0');
    //xác suất thắng
    let probability = this.listProbability.find(e => e.categoryId == leadByIdModel.probabilityId);
    this.editLeadForm.get('Probability').patchValue(probability ? probability : null);
    //mức độ tiềm năng
    let potential = this.listPotential.find(e => e.categoryId == leadByIdModel.potentialId);
    this.editLeadForm.get('Potential')?.patchValue(potential ? potential : null);
    //trạng thái
    let status = this.listStatusLead.find(e => e.categoryId == leadByIdModel.statusId);
    this.editLeadForm.get('Status').patchValue(status ? status : null);
    /* thông tin cá nhân */
    // email
    let email = contactLeadByIdModel.email;
    this.editLeadForm.get('Email').patchValue(email ? email : '');
    //phone
    let phone = contactLeadByIdModel.phone;
    this.editLeadForm.get('Phone').patchValue(phone ? phone : '');
    //cmnd
    let identityId = contactLeadByIdModel.identityId;
    this.editLeadForm.get('IdentityId').patchValue(identityId ? identityId : '');
    //địa chỉ
    let address = contactLeadByIdModel.address;
    this.editLeadForm.get('DetailAddress').patchValue(address ? address : '');
  }

  confirmEditLead() {

    // enable form de submit
    this.editLeadForm.enable();
    if (!this.editLeadForm.valid) {
      Object.keys(this.editLeadForm.controls).forEach(key => {
        if (!this.editLeadForm.controls[key].valid) {
          this.editLeadForm.controls[key].markAsTouched();
        }
      });
      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else {
      let leadModel: LeadModel = this.mapFormToLeadModel();
      let contactLeadModel: ContactModel = this.mapFormToLeadContactModel();
      let listInterestedId: Array<string> = this.getListInterestedId();
      let listDocumentLink: Array<linkOfDocumentRequest> = this.getListOfDocumentRequest();
      let isGetNoti: boolean = this.isGetNotification.value;
      this.editLead(leadModel, contactLeadModel, listInterestedId, listDocumentLink, isGetNoti);
    }
  }

  async cloneLead() {

    this.loading = true;
    let result: any = await this.leadService.cloneLeadAsync(this.leadId, this.auth.UserId);
    this.loading = false;
    if (result.statusCode == 200) {
      let note: NoteModel = new NoteModel();
      note.Type = 'NEW';
      if (result.picName) {
        note.Description = 'Trạng thái - <b>' + result.statusName + '</b>, người phụ trách - <b>' + result.picName;
      } else {
        note.Description = 'Trạng thái - <b>' + result.statusName + '</b>, chưa có người phụ trách';
      }
      this.noteService.createNote(note, result.leadId, null, this.auth.UserId).subscribe(response => {
        var _noteresult = <any>response;
        if (_noteresult.statusCode !== 200) {
          this.clearToast();
          this.showToast('error', 'Thông báo', _noteresult.messageCode);
        }
        this.router.navigate(['/lead/detail', { leadId: result.leadId }]);
        this.leadId = result.leadId;
        this.getMasterdata();

        let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Nhân bản thành công' };
        this.showMessage(msg);
      });
    } else {
      let msg = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  async editLead(leadModel: LeadModel, contactLeadModel: ContactModel,
    listInterestedId: Array<string>, listDocumentLink: Array<linkOfDocumentRequest>,
    isGetNoti: boolean) {
    //valid
    let listContact: Array<ContactModel> = this.getListContactFromTable();
    this.loading = true;
    await this.uploadFlileToServer(leadModel);
    this.leadService.editLeadById(leadModel, contactLeadModel,
      listInterestedId, listContact,
      this.listLeadDetail,
      this.listDocumentIdNeedRemove, listDocumentLink,
      isGetNoti).subscribe(response => {
        const result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.loading = false;

          this.listDocument = [];
          this.listFile = result.listFile || [];
          this.listLinkOfDocument = result.listLinkOfDocument || [];
          this.patchDefaultDocumentFile();
          this.patchDefaultDocumentLink();

          if (result.isChangePic) {
            let _type = 'EDT';
            let _noteTitle = '';
            let _noteBody = result.picName == null ? " đã chỉnh sửa cơ hội này không còn Người phụ trách" : "đã chỉnh sửa Người phụ trách thành " + "<b>" + result.picName + "</b>";
            this.createNoteAfterEdit(_type, _noteTitle, _noteBody);
          }
          if (result.isChangeStatus) {
            let _type = 'EDT';
            let _noteTitle = '';
            let _noteBody = "đã chỉnh sửa Trạng thái thành " + "<b>" + result.statusName + "</b>";
            this.createNoteAfterEdit(_type, _noteTitle, _noteBody);
          }
          if (result.isChangePotential) {
            let _type = 'EDT';
            let _noteTitle = '';
            let _noteBody = "đã chỉnh sửa Mức độ tiềm năng thành " + "<b>" + result.potential + "</b>";
            this.createNoteAfterEdit(_type, _noteTitle, _noteBody);
          }
          let msg = { severity: 'success', summary: 'Thông báo', detail: "Chỉnh sửa cơ hội thành công" };
          this.showMessage(msg);

          this.getMasterdata();
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
  }

  getListContactFromTable(): Array<ContactModel> {
    let result = new Array<ContactModel>();
    this.listContact.forEach(_contact => {
      let newContact = new ContactModel();
      //data
      newContact.FirstName = _contact.firstName;
      newContact.LastName = _contact.lastName;
      newContact.Gender = _contact.gender;
      newContact.DateOfBirth = _contact.dateOfBirth;
      newContact.Role = _contact.role;
      newContact.RelationShip = _contact.relationShip;
      newContact.Phone = _contact.phone;
      newContact.Email = _contact.email;
      newContact.Note = _contact.note;
      //default value
      newContact.ContactId = this.emptyGuid;
      newContact.ObjectId = this.emptyGuid;
      newContact.ObjectType = "LEA_CON";
      newContact.Active = true;
      newContact.CreatedById = this.auth.UserId;
      newContact.CreatedDate = new Date();

      result = [newContact, ...result];
    });

    return result;
  }

  mapFormToLeadModel(): LeadModel {

    let lead = new LeadModel();
    lead.LeadId = this.leadId;

    let detailInterested = this.editLeadForm.get('DetailInterested').value;
    let potential: potentialModel = this.editLeadForm.get('Potential')?.value; //Mức độ tiền năng
    let personInChargeId: Employee = this.editLeadForm.get('Pic').value;
    let leadType: leadTypeModel = this.editLeadForm.get('LeadType').value;
    let group: leadGroup = this.editLeadForm.get('Group').value;
    let customer: leadReferenceCustomer = this.editLeadForm.get('RefCustomer').value;
    let businessType: businessType = this.editLeadForm.get('BusinessType').value;
    let investmentFund: investFund = this.editLeadForm.get('InvestFund').value;
    let probability: probability = this.editLeadForm.get('Probability').value;
    let expectedSale: string = this.editLeadForm.get('ExpectedSale').value;
    let percent: string = this.editLeadForm.get('Percent')?.value;
    let forecastSales: string = this.editLeadForm.get('ForecastSales')?.value;
    let _area = this.editLeadForm.get('Area').value;

    lead.RequirementDetail = detailInterested ? detailInterested : "";
    lead.PotentialId = null;
    lead.InterestedGroupId = null;
    lead.PersonInChargeId = personInChargeId ? personInChargeId.employeeId : null;
    lead.LeadTypeId = leadType ? leadType.categoryId : null;
    let status = this.listStatusLead.find(e => e.categoryCode == this.statusCode);
    lead.StatusId = status ? status.categoryId : null;

    lead.PotentialId = potential?.categoryId;
    lead.LeadGroupId = group ? group.categoryId : null;
    lead.LeadCode = this.leadByIdModel.leadCode;
    lead.CompanyId = null;
    lead.PaymentMethodId = null;
    lead.CustomerId = customer ? customer.customerId : null;
    lead.BusinessTypeId = businessType ? businessType.categoryId : null;
    lead.InvestmentFundId = investmentFund ? investmentFund.categoryId : null;
    lead.ProbabilityId = probability ? probability.categoryId : null;
    lead.ExpectedSale = ParseStringToFloat(expectedSale);
    lead.Percent = percent ? ParseStringToFloat(percent) : null;
    lead.ForecastSales = forecastSales ? ParseStringToFloat(forecastSales) : null;
    lead.GeographicalAreaId = _area ? _area.geographicalAreaId : null;

    lead.CreatedById = this.auth.UserId;
    lead.CreatedDate = new Date();
    lead.UpdatedById = this.auth.UserId;
    lead.UpdatedDate = new Date();
    lead.Active = true;
    lead.Role = null;
    lead.WaitingForApproval = false;

    return lead;
  }

  mapFormToLeadContactModel(): ContactModel {
    let phone = this.editLeadForm.get('Phone').value != null ? this.editLeadForm.get('Phone').value.trim() : null;
    let email = this.editLeadForm.get('Email').value != null ? this.editLeadForm.get('Email').value.trim() : null;
    let address = this.editLeadForm.get('DetailAddress').value != null ? this.editLeadForm.get('DetailAddress').value.trim() : null;
    let identityID = this.editLeadForm.get('IdentityId').value;
    let leadName = this.editLeadForm.get('LeadName').value != null ? this.editLeadForm.get('LeadName').value.trim() : null;

    let contact = new ContactModel();
    contact.ObjectId = this.leadId;
    contact.FirstName = leadName ? leadName : "";
    contact.ObjectType = "LEA";
    contact.DateOfBirth = null;
    contact.Phone = phone ? phone : "";
    contact.WorkPhone = null;
    contact.OtherPhone = null;
    contact.Email = email ? email : "";
    contact.WorkEmail = null;
    contact.OtherEmail = null;
    contact.IdentityID = identityID ? identityID : null;
    contact.AvatarUrl = null;
    contact.Address = address ? address : "";
    contact.ProvinceId = null;
    contact.DistrictId = null;
    contact.WardId = null;
    contact.CreatedById = this.auth.UserId;
    contact.CreatedDate = new Date();
    contact.UpdatedById = this.auth.UserId;
    contact.UpdatedDate = new Date();
    contact.Active = true;
    return contact;
  }

  async uploadFlileToServer(leadModel: LeadModel) {
    if (this.uploadedFiles.length == 0) return;
    let listFileUploadModel: Array<FileUploadModel> = [];
    this.uploadedFiles.forEach(item => {
      let fileUpload: FileUploadModel = new FileUploadModel();
      fileUpload.FileInFolder = new FileInFolder();
      fileUpload.FileInFolder.active = true;
      let index = item.name.lastIndexOf(".");
      let name = item.name.substring(0, index);
      fileUpload.FileInFolder.fileName = name;
      fileUpload.FileInFolder.fileExtension = item.name.substring(index, item.name.length - index);
      fileUpload.FileInFolder.size = item.size;
      fileUpload.FileInFolder.objectId = leadModel.LeadId;
      fileUpload.FileInFolder.objectType = 'QLCH';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    let result: any = await this.folderService.uploadFileeByFolderTypeAsync("QLCH", listFileUploadModel, leadModel.LeadId);
    this.uploadedFiles = [];
  }

  getListInterestedId(): Array<string> {
    let interestedGroup: Array<interestedGroupModel> = this.editLeadForm.get('Interested').value;
    return interestedGroup.map(e => e.categoryId);
  }

  getInforLeadAfterEdit(): leadByIdModel {
    let detailInterested = this.editLeadForm.get('DetailInterested').value;
    let potential: potentialModel = this.editLeadForm.get('Potential')?.value; // mức độ tiền năng
    let personInChargeId: Employee = this.editLeadForm.get('Pic').value;
    let leadType: leadTypeModel = this.editLeadForm.get('LeadType').value;
    let group: leadGroup = this.editLeadForm.get('Group').value;
    let customer: leadReferenceCustomer = this.editLeadForm.get('RefCustomer').value;
    let businessType: businessType = this.editLeadForm.get('BusinessType').value;
    let investmentFund: investFund = this.editLeadForm.get('InvestFund').value;
    let probability: probability = this.editLeadForm.get('Probability').value;
    let expectedSale: string = this.editLeadForm.get('ExpectedSale').value;
    let status = this.listStatusLead.find(e => e.categoryCode == this.viewModeCode);

    let lead = new leadByIdModel();
    lead.leadId = this.leadId;
    lead.requirementDetail = detailInterested ? detailInterested : null;
    lead.potentialId = null;
    lead.interestedGroupId = null;
    lead.statusId = status ? status.categoryId : null;
    lead.leadTypeId = leadType ? leadType.categoryId : null;
    lead.personInChargeId = personInChargeId ? personInChargeId.employeeId : null;
    lead.paymentMethodId = null;
    lead.leadGroupId = group ? group.categoryId : null;
    lead.customerId = customer ? customer.customerId : null;
    lead.businessTypeId = businessType ? businessType.categoryId : null;
    lead.investmentFundId = investmentFund ? investmentFund.categoryId : null;
    lead.probabilityId = probability ? probability.categoryId : null;
    lead.expectedSale = expectedSale ? ParseStringToFloat(expectedSale) : 0;
    lead.createdById = this.auth.UserId;
    lead.createdDate = new Date();
    lead.updatedById = this.auth.UserId;
    lead.updatedDate = new Date();
    lead.active = true;
    lead.role = null;
    lead.waitingForApproval = false;
    return lead;
  }

  /*Event thay đổi nội dung ghi chú*/
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  /*Event Lưu các file được chọn*/
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

  /*Thêm sản phẩm dịch vụ*/
  addCustomerOrderDetail() {
    let ref = this.dialogService.open(LeadDetailDialogComponent, {
      data: {
        isCreate: true,
        type: 'SALE',
        viewModeCode: this.viewModeCode,
      },
      header: 'Thêm sản phẩm dịch vụ',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: ResultDetailDialog) => {
      if (result) {
        if (result.status) {
          this.leadDetail = result.leadDetailModel;

          //set orderNumber cho sản phẩm/dịch vụ mới thêm
          this.leadDetail.OrderNumber = this.listLeadDetail.length + 1;

          this.listLeadDetail.push(this.leadDetail);
        }
      }
    });
  }

  /*Xóa một sản phẩm dịch vụ*/
  deleteItem(dataRow) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listLeadDetail = this.listLeadDetail.filter(e => e != dataRow)

        //Đánh lại số OrderNumber
        this.listLeadDetail.forEach((item, index) => {
          item.OrderNumber = index + 1;
        });
      }
    });
  }

  deleteContact(dataRow) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listContact = this.listContact.filter(e => e != dataRow)
      }
    });
  }

  /*Sửa một sản phẩm dịch vụ*/
  onRowSelect(dataRow) {
    // if (this.viewModeCode != "DRAFT") return;
    //Nếu có quyền sửa thì mới cho sửa
    if (this.isNotReference) {
      var index = this.listLeadDetail.indexOf(dataRow);
      var OldArray = this.listLeadDetail[index];

      let ref = this.dialogService.open(LeadDetailDialogComponent, {
        data: {
          isCreate: false,
          leadDetailModel: OldArray,
          viewModeCode: this.viewModeCode,
          action: this.actionEdit,
          editMode: this.isModeView,
        },
        header: 'Sửa sản phẩm dịch vụ',
        width: '70%',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "280px",
          "max-height": "600px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: ResultDetailDialog) => {
        if (result) {
          if (result.status) {
            this.listLeadDetail[index] = result.leadDetailModel;

            //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
            this.listLeadDetail.sort((a, b) =>
              (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));
          }
          this.calTotalAmountAndSumProduct(this.listLeadDetail);
        }
      });
    }
  }

  addLeadContact() {
    let leadContactConfigData = new leadContactConfigDataModel();
    leadContactConfigData.isEdit = false; //tao moi lien he
    leadContactConfigData.contact = null;

    let ref = this.dialogService.open(CreatContactLeadDialogComponent, {
      data: {
        leadContactConfigData: leadContactConfigData
      },
      header: 'Thêm liên hệ',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px"
      }
    });

    ref.onClose.subscribe((result: resultContactDialog) => {
      if (result) {
        if (result.status === true) {
          this.listContact = [result.contact, ...this.listContact];
        }
      }
    });
  }

  editContact(rowData: contactModel) {
    if (this.isModeView) return;
    let configData = new leadContactConfigDataModel();
    configData.isEdit = true;
    configData.contact = rowData;

    let ref = this.dialogService.open(CreatContactLeadDialogComponent, {
      data: {
        leadContactConfigData: configData
      },
      header: 'Thêm liên hệ',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px"
      }
    });

    ref.onClose.subscribe((result: resultContactDialog) => {
      if (result) {
        if (result.status === true) {
          //update
          let editItemIndex = this.listContact.findIndex(e => e === rowData);
          this.listContact[editItemIndex] = result.contact;
        }
      }
    });
  }

  /*Event khi download 1 file đã lưu trên server*/
  downloadFile(fileInfor: NoteDocument) {
    // this.imageService.downloadFile(fileInfor.documentName, fileInfor.documentUrl).subscribe(response => {
    //   var result = <any>response;
    //   var binaryString = atob(result.fileAsBase64);
    //   var fileType = result.fileType;

    //   var binaryLen = binaryString.length;
    //   var bytes = new Uint8Array(binaryLen);
    //   for (var idx = 0; idx < binaryLen; idx++) {
    //     var ascii = binaryString.charCodeAt(idx);
    //     bytes[idx] = ascii;
    //   }
    //   var file = new Blob([bytes], { type: fileType });
    //   if (window.navigator && window.navigator.msSaveOrOpenBlob) {
    //     window.navigator.msSaveOrOpenBlob(file);
    //   } else {
    //     var fileURL = URL.createObjectURL(file);
    //     if (fileType.indexOf('image') !== -1) {
    //       window.open(fileURL);
    //     } else {
    //       var anchor = document.createElement("a");
    //       anchor.download = name;
    //       anchor.href = fileURL;
    //       anchor.click();
    //     }
    //   }
    // });
  }

  patchLeadDetailData() {
    this.listLeadDetail = [];
    this.listLeadDetailReply = [];
    this.listLeadDetailData.forEach((_detail, index) => {
      let leadDetail = new LeadDetailModel();

      leadDetail.CurrencyUnit = _detail.currencyUnit;
      leadDetail.Description = _detail.description;
      leadDetail.DiscountType = _detail.discountType;
      leadDetail.DiscountValue = _detail.discountValue;
      leadDetail.ExchangeRate = _detail.exchangeRate;
      leadDetail.IncurredUnit = _detail.incurredUnit;

      leadDetail.LeadDetailId = _detail.leadDetailId;
      leadDetail.LeadId = _detail.leadId;
      leadDetail.OrderDetailType = _detail.orderDetailType;
      leadDetail.ProductId = _detail.productId;
      leadDetail.Quantity = _detail.quantity;
      leadDetail.UnitId = _detail.unitId;
      leadDetail.UnitPrice = _detail.unitPrice;
      leadDetail.Vat = _detail.vat;
      leadDetail.VendorId = _detail.vendorId;
      leadDetail.ProductCategory = _detail.productCategory;

      leadDetail.NameMoneyUnit = _detail.nameMoneyUnit;
      leadDetail.NameVendor = _detail.nameVendor;
      leadDetail.ProductCode = _detail.productCode;
      leadDetail.ProductName = _detail.productName;
      leadDetail.ExplainStr = _detail.productName;
      leadDetail.ProductNameUnit = _detail.productNameUnit;

      leadDetail.AmountDiscount = _detail.amountDiscount;

      leadDetail.UnitLaborPrice = _detail.unitLaborPrice;
      leadDetail.UnitLaborNumber = _detail.unitLaborNumber;

      leadDetail.SumAmount = _detail.sumAmount;

      leadDetail.OrderNumber = _detail.orderNumber ? _detail.orderNumber : index + 1;

      leadDetail.Active = _detail.active;

      leadDetail.LeadProductDetailProductAttributeValue = [];
      _detail.leadProductDetailProductAttributeValue.forEach(attr => {
        let attri = new LeadProductDetailProductAttributeValue();
        attri.LeadDetailId = attr.leadDetailId;
        attri.LeadProductDetailProductAttributeValue1 = attr.leadProductDetailProductAttributeValue1;
        attri.ProductAttributeCategoryId = attr.productAttributeCategoryId;
        attri.ProductAttributeCategoryValueId = attr.productAttributeCategoryValueId;
        attri.ProductId = attr.productId;

        leadDetail.LeadProductDetailProductAttributeValue = [attri, ...leadDetail.LeadProductDetailProductAttributeValue];
      });

      this.listLeadDetail = [leadDetail, ...this.listLeadDetail];
      this.listLeadDetailReply = [leadDetail, ...this.listLeadDetailReply];
    });
    this.listLeadDetail = this.listLeadDetail.sort((a, b) => (a.OrderNumber > b.OrderNumber) ? 1 : -1);
  }

  patchLeadContactData() {
    this.listContact.forEach(e => {
      if (e.gender) {
        e.genderDisplay = this.listGender.find(x => x.categoryCode == e.gender).categoryName;
      }
    });
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
        $('#' + element.noteId).find('.full-content').append($.parseHTML(element.description));
      }, 1000);
    });
  }
  /*End*/

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
      noteModel.ObjectId = this.leadId;
      noteModel.ObjectType = 'LEA';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/
      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'EDT';
      noteModel.ObjectId = this.leadId;
      noteModel.ObjectType = 'LEA';
      noteModel.NoteTitle = 'đã sửa ghi chú';
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
    this.noteService.createNoteForLeadDetail(noteModel, this.listNoteDocumentModel).subscribe(response => {
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

  async createNoteAfterEdit(type: string, noteTitle: string, noteBody: string) {
    let noteModel = new NoteModel();
    noteModel.NoteId = this.emptyGuid;
    noteModel.Description = noteBody;
    noteModel.Type = 'ADD';
    noteModel.ObjectId = this.leadId;
    noteModel.ObjectType = 'LEA';
    noteModel.NoteTitle = noteTitle;
    noteModel.Active = true;
    noteModel.CreatedById = this.emptyGuid;
    noteModel.CreatedDate = new Date();
    this.noteService.createNoteForLeadDetail(noteModel, []).subscribe(response => {
      let result: any = response;
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
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  async uploadFilesAsync(files: File[]) {
    await this.imageService.uploadFileAsync(files);
  }

  backToList() {
    this.router.navigate(['/lead/list']);
  }

  deleteLead() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa cơ hội này?',
      accept: () => {
        this.leadService.changeLeadStatusToDelete(this.leadId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.router.navigate(['/lead/list']);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }

  checkDuplicateCustomer(type: string, formControl: FormControl | AbstractControl): boolean {
    if (formControl.invalid) return false;
    let isDuplicate: boolean = false;
    let value = formControl.value;
    switch (type) {
      case 'email':
        let duplicateEmailCustomer: customerContactModel = this.listCustomerContact.filter(e => e.email != null && e.email != "").find(e => e.email.trim() == value);
        if (duplicateEmailCustomer) {
          isDuplicate = true;
          this.confirmationService.confirm({
            message: `Đã tồn tại khách hàng có email này trong hệ thống. Bạn có muốn cập nhật thông tin khách hàng này không?`,
            accept: () => {
              this.router.navigate(['/customer/detail', { customerId: duplicateEmailCustomer.customerId }]);
            }
          });
        }
        break;
      case 'phone':
        let duplicatePhoneCustomer: customerContactModel = this.listCustomerContact.filter(e => e.phone != null).find(e => e.phone.trim() == value);
        if (duplicatePhoneCustomer) {
          isDuplicate = true;
          this.confirmationService.confirm({
            message: `Đã tồn tại khách hàng có số điện thoại này trong hệ thống. Bạn có muốn cập nhật thông tin khách hàng này không?`,
            accept: () => {
              this.router.navigate(['/customer/detail', { customerId: duplicatePhoneCustomer.customerId }]);
            }
          });
        }
        break;
      default:
        break;
    }
    return isDuplicate;
  }

  // End
  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clearMessage() {
    this.messageService.clear();
  }
  // Get current date
  getDate() {
    return new Date();
  }
  // End

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

  /* Xác định điều kiện ẩn hiện button: Tạo Báo giá và Tạo Hồ sơ thầu */
  displayDistroyButton() {
    if (this.statusSaleBiddingAndQuote === 1 || this.statusSaleBiddingAndQuote === 3) {
      this.isCreateSaleBiddingButton = true;
      this.isCreateQuoteButton = false;
    } else if (this.statusSaleBiddingAndQuote === 2 || this.statusSaleBiddingAndQuote === 4) {
      this.isCreateQuoteButton = true;
      this.isCreateSaleBiddingButton = false;
    }
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  changeTypeLead(event: any) {
    this.listCurrentReferenceCustomer = [];

    if (event.value === null) {
      this.listCurrentCustomer = this.listCustomer;
      return false;
    };
    let currentType: leadTypeModel = event.value;
    switch (currentType.categoryCode) {
      case "KPL":
        //khach hang ca nhan
        this.listCurrentCustomer = this.listCustomer.filter(e => e.customerType === 2);
        this.editLeadForm.get('RefCustomer').reset();
        break;
      case "KCL":
        //khach hang doanh nghiep
        this.listCurrentCustomer = this.listCustomer.filter(e => e.customerType === 1);
        this.editLeadForm.get('RefCustomer').reset();
        break;
      default:
        break;
    }
  }

  async setViewMode(code: string) {
    let isAppr = true;
    let viewModeCode = code;
    let status: statusLeadModel = this.listStatusLead.find(e => e.categoryCode == viewModeCode);
    let statusId = status ? status.categoryId : null;


    switch (viewModeCode) {
      case "DRAFT": //Trạng thái nháp
        this.loading = true;
        let draftResult: any = await this.leadService.changeLeadStatus(this.leadId, statusId);
        if (draftResult.statusCode == 200) {
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Sửa trạng thái thành công' };
          this.viewModeCode = viewModeCode;
          this.statusCode = viewModeCode;
          this.updateNewStatus(status);
          this.showMessage(msg);
          this.getMasterdata();
        } else {
          this.loading = false;
          let msg = { severity: 'error', summary: 'Thông báo:', detail: draftResult.messageCode };
          this.showMessage(msg);
        }

        break;

      case "APPR": //Trạng thái xác nhận
        //chuyen trang thai lead thanh xac nhan
        if (!this.editLeadForm.get('Pic').value) {
          let msg = { severity: 'error', summary: 'Thông báo', detail: "Chưa có người phụ trách!" };
          this.showMessage(msg);
          isAppr = false;
        }

        if (isAppr) {
          this.loading = true;
          let result: any = await this.leadService.changeLeadStatus(this.leadId, statusId);
          this.loading = false;
          if (result.statusCode == 200) {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Sửa trạng thái thành công' };
            this.viewModeCode = viewModeCode;
            this.statusCode = viewModeCode;
            this.updateNewStatus(status);
            this.confirmEditLead();
            this.showMessage(msg);
            this.getMasterdata();
          } else {
            this.loading = false;
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        }

        break;

      case "CANC": //Trạng thái hủy
        if (this.statusSaleBiddingAndQuote == 1 || this.statusSaleBiddingAndQuote == 2) {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Cơ hội đã gắn với hồ sơ thầu hoặc báo giá! Cần hủy hồ sơ thầu hoặc báo giá trước' };
          this.showMessage(msg);
          return;
        }
        this.loading = true;
        let cancelResult: any = await this.leadService.changeLeadStatus(this.leadId, statusId);
        this.loading = false;
        if (cancelResult.statusCode == 200) {
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Sửa trạng thái thành công' };
          this.viewModeCode = viewModeCode;
          this.statusCode = viewModeCode;
          this.updateNewStatus(status);
          this.showMessage(msg);
          this.getMasterdata();
        } else {
          this.loading = false;
          let msg = { severity: 'error', summary: 'Thông báo:', detail: cancelResult.messageCode };
          this.showMessage(msg);
        }
        break;

      default:
        break;
    }
  }

  /*Event thay đổi Khách hàng*/
  changeCustomer(event: any) {

    //Nếu bỏ chọn Khách hàng
    if (event.value == null) {
      this.editLeadForm.controls['Email'].setValue(null);
      this.editLeadForm.controls['Phone'].setValue(null);
      this.editLeadForm.controls['DetailAddress'].setValue(null);
      this.getListPersonInChange(this.emptyGuid, this.auth.UserId);
      return false;
    }
    if (event.value.customerType == 1) {
      this.editLeadForm.controls['Email'].setValue(event.value.workEmail ? event.value.workEmail : null);
    } else if (event.value.customerType == 2) {
      this.editLeadForm.controls['Email'].setValue(event.value.email ? event.value.email : null);
    }


    this.editLeadForm.controls['Phone'].setValue(event.value.phone);
    let address = event.value.address;
    this.editLeadForm.controls['DetailAddress'].setValue(address);

    //nếu khách hàng không có người phụ trách thì kết quả = có người phụ trách là nhân viên đang đăng nhập
    this.getListPersonInChange(event.value.personInChargeId, this.auth.UserId)
  }

  getListPersonInChange(empId: any, userId: any) {
    let personInChargeId = empId;
    if (personInChargeId) {
      this.leadService.getEmployeeByPersonInCharge(personInChargeId, userId, this.leadByIdModel.personInChargeId).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.listPersonalInChangeCus = result.listEmployee || [];
          let emp = this.listPersonalInChangeCus.find(e => e.employeeId == personInChargeId);
          this.editLeadForm.controls['Pic'].setValue(emp);
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
    else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Khách hàng này không có người phụ trách' };
      this.showMessage(msg);
    }
  }

  updateNewStatus(status: statusLeadModel) {
    this.editLeadForm.get('Status').setValue(status ? status : null);
  }

  /* Chuyển item lên một cấp */
  moveUp(data: LeadDetailModel) {
    let currentOrderNumber = data.OrderNumber;
    let preOrderNumber = currentOrderNumber - 1;
    let pre_data = this.listLeadDetail.find(x => x.OrderNumber == preOrderNumber);

    //Đổi số OrderNumber của 2 item
    pre_data.OrderNumber = currentOrderNumber;
    data.OrderNumber = preOrderNumber;

    //Xóa 2 item
    this.listLeadDetail = this.listLeadDetail.filter(x =>
      x.OrderNumber != preOrderNumber && x.OrderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listLeadDetail = [...this.listLeadDetail, pre_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listLeadDetail.sort((a, b) =>
      (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));
  }

  /* Chuyển item xuống một cấp */
  moveDown(data: LeadDetailModel) {
    let currentOrderNumber = data.OrderNumber;
    let nextOrderNumber = currentOrderNumber + 1;
    let next_data = this.listLeadDetail.find(x => x.OrderNumber == nextOrderNumber);

    //Đổi số OrderNumber của 2 item
    next_data.OrderNumber = currentOrderNumber;
    data.OrderNumber = nextOrderNumber;

    //Xóa 2 item
    this.listLeadDetail = this.listLeadDetail.filter(x =>
      x.OrderNumber != nextOrderNumber && x.OrderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listLeadDetail = [...this.listLeadDetail, next_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listLeadDetail.sort((a, b) =>
      (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));
  }

  resetLinkForm() {
    this.addLinkForm.reset();
    this.addLinkForm.patchValue({
      'NameOfLink': '',
      'Link': ''
    });
  }

  openAddLinkDialog() {
    this.resetLinkForm();
    this.displayAttachLink = true;
  }

  openAddFileDialog() {
    this.displayAttachFile = true;
  }

  closeAddFileDialog() {
    this.displayAttachFile = false;
  }

  closeAddLinkDialog() {
    this.displayAttachLink = false;
  }

  addFileToTable() {
    if (this.uploadedFiles) {
      this.uploadedFiles.forEach((file, index) => {
        file.createdById = this.auth.UserId;
        file.createdByName = this.userFullName;
        file.createdDate = new Date();
        file.dumpId = index;
        file.typeDocument = "DOC";
        file.fileInFolderId = this.emptyGuid;
        this.listDocument = [...this.listDocument, file];
      });
    }

    //upload file
    this.displayAttachFile = false;
    this.fileUpload.files = []; //primeNg control model
  }

  addLinkToTable() {
    if (!this.addLinkForm.valid) {
      Object.keys(this.addLinkForm.controls).forEach(key => {
        if (!this.addLinkForm.controls[key].valid) {
          this.addLinkForm.controls[key].markAsTouched();
        }
      });
      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
      return;
    }

    let linkName = this.addLinkForm.get('NameOfLink').value || '';
    let link = this.addLinkForm.get('Link').value || '';

    let item = new FileInFolder();
    item.linkOfDocumentId = this.emptyGuid;
    item.linkName = linkName ? linkName.trim() : "";
    item.linkValue = link ? link.trim() : "";
    item.typeDocument = "LINK";

    item.name = item.linkName;
    item.createdById = this.auth.UserId;
    item.createdByName = this.userFullName;
    item.createdDate = new Date();
    item.isNewLink = true;
    this.listDocument = [...this.listDocument, item];

    this.displayAttachLink = false;
  }

  getListOfDocumentRequest(): Array<linkOfDocumentRequest> {
    let result: Array<linkOfDocumentRequest> = [];
    this.listDocument.forEach(doc => {
      if (doc.typeDocument == "LINK") {
        let newRecord = new linkOfDocumentRequest();
        newRecord.LinkOfDocumentId = doc.linkOfDocumentId ? doc.linkOfDocumentId : this.emptyGuid;
        newRecord.LinkName = doc.linkName;
        newRecord.LinkValue = doc.linkValue;
        newRecord.ObjectId = this.emptyGuid;
        newRecord.ObjectType = '';
        newRecord.Active = true;
        newRecord.CreatedById = this.emptyGuid;
        newRecord.CreatedDate = new Date();
        newRecord.IsNewLink = doc.isNewLink;
        result.push(newRecord);
      }
    });
    return result;
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  openDocument(rowData: FileInFolder) {
    if (rowData.typeDocument == "LINK") {
      //mo lien ket
      window.open(rowData.linkValue, "_blank");
      return;
    }
  }

  deleteDocument(rowData: FileInFolder) {
    this.listDocument = this.listDocument.filter(file => file != rowData);
    if (rowData.fileInFolderId) {
      //file cu~
      this.listDocumentIdNeedRemove.push(rowData.fileInFolderId);
    }
  }

  async downloadDocument(rowData: FileInFolder) {
    if (rowData.fileInFolderId) {
      this.loading = true;
      this.folderService.downloadFile(rowData.fileInFolderId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
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
              anchor.download = rowData.fileName;
              anchor.href = fileURL;
              anchor.click();
            }
          }
        }
        else {
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      });
    }
  }

  createSaleBidding() {
    if (this.leadByIdModel.customerId) {
      this.router.navigate(['/sale-bidding/create', { leadId: this.leadId }]);
    }
    else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Không thể tạo Hồ sơ thầu khi Cơ hội không gắn với khách hàng' };
      this.showMessage(msg);
    }
  }

  /* Tạo báo giá từ Cơ hội */
  createQuote() {
    if (this.leadByIdModel.customerId) {
      this.router.navigate(['/customer/quote-create', { occasionId: this.leadId }]);
    }
    //Nếu cơ hội không có khách hàng
    else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Không thể tạo Báo giá khi Cơ hội không gắn với khách hàng' };
      this.showMessage(msg);
    }
  }

  changeActive(item: StatusSupport) {
    this.listFormatStatusSupport.forEach(_item => {
      _item.isActive = false;
    });

    item.isActive = true;
  }

  buildListStatusSupport(statusSupportId: string) {
    //Format list status support
    this.listFormatStatusSupport = [];

    this.listStatusSupport.forEach(item => {

      if (item.categoryCode.indexOf('E') == -1) {
        let _item = new StatusSupport();

        _item.categoryId = item.categoryId;
        _item.categoryCode = item.categoryCode;
        _item.categoryName = item.categoryName;

        this.listFormatStatusSupport.push(_item);
      }
      else {
        if (!this.listFormatStatusSupport.find(x => x.categoryCode == 'TEMP')) {
          let _item = new StatusSupport();
          _item.categoryCode = 'TEMP';
          _item.children = this.listTempStatusSupport;
          this.listFormatStatusSupport.push(_item);
        }
      }
    });

    //Tìm trạng thái phụ hiện tại (nếu có)
    let statusSupportCode = this.listStatusSupport.find(x => x.categoryId == statusSupportId)?.categoryCode;

    //Nếu trạng thái hiện tại là Chốt đơn không thành công thì giá trị mặc định của droplist là Chốt đơn không thành công
    if (statusSupportCode == "E2") {
      this.selectedTempStatusSupport = this.listTempStatusSupport.find(x => x.categoryCode == statusSupportCode);
    }
    //Nếu chưa có trạng thái phụ hoặc trạng thái phụ là Chốt đơn thành công thì giá trị mặc định của droplist là Chốt đơn thành công
    else {
      this.selectedTempStatusSupport = this.listTempStatusSupport.find(x => x.categoryCode == "E1");
    }

    //Nếu có trạng thái phụ
    if (statusSupportId) {
      if (statusSupportCode == "E1" || statusSupportCode == "E2") {
        let activeStep = this.listFormatStatusSupport.find(x => x.categoryCode == "TEMP");
        activeStep.isActive = true;
        activeStep.isCurrent = true;

        //Lấy các step sếp sau nó
        let activeStepIndex = this.listFormatStatusSupport.indexOf(activeStep);
        let listIndex: Array<number> = [];

        for (let i = 0; i < this.listFormatStatusSupport.length; i++) {
          if (i < activeStepIndex) {
            listIndex.push(i);
          }
        }

        this.listFormatStatusSupport.forEach((item, index) => {
          if (listIndex.includes(index)) {
            item.isComplete = true;
          }
        });
      }
      else {
        //Lấy step đang được ative
        let activeStep = this.listFormatStatusSupport.find(x => x.categoryCode == statusSupportCode);
        activeStep.isActive = true;
        activeStep.isCurrent = true;

        //Lấy các step sếp sau nó
        let activeStepIndex = this.listFormatStatusSupport.indexOf(activeStep);
        let listIndex: Array<number> = [];

        for (let i = 0; i < this.listFormatStatusSupport.length; i++) {
          if (i < activeStepIndex) {
            listIndex.push(i);
          }
        }

        this.listFormatStatusSupport.forEach((item, index) => {
          if (listIndex.includes(index)) {
            item.isComplete = true;
          }
        });
      }
    }
    //Nếu chưa có trạng thái phụ
    else {
      //Mặc định là mới tạo
      this.listFormatStatusSupport.forEach(item => {
        if (item.categoryCode == "A") {
          item.isActive = true;
          item.isCurrent = true;
        }
      });
    }
  }

  changeStatusSupport() {
    //Lấy statusSupportId
    let statusSupportId = '';
    let statusSupport = this.listFormatStatusSupport.find(x => x.isActive == true);

    if (statusSupport.categoryCode == "TEMP") {
      statusSupportId = this.selectedTempStatusSupport.categoryId;
    }
    else {
      statusSupportId = statusSupport.categoryId;
    }

    this.leadService.changeLeadStatusSupport(this.leadId, statusSupportId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.showToast('success', 'Thông báo', 'Chuyển trạng thái thành công');
        this.buildListStatusSupport(statusSupportId);
      }
      else {
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  /*Sửa: Chuyển mode edit*/
  toModeEdit() {
    this.isModeView = false;
    let status = this.listStatusLead.find(e => e.categoryId == this.leadByIdModel.statusId)?.categoryCode;
    if(status == 'APPR'){
      this.editLeadForm.disable();
      this.editLeadForm.get('Pic').enable();
    }else if(status == 'DRAFT'){
      this.editLeadForm.enable();
    }
  }

  /*Hủy sửa: Chuyển mode view*/
  toModeView() {
    this.getMasterdata();
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  toggle(index: number) {
    this.activeState[index] = !this.activeState[index];
  }

  calTotalAmountAndSumProduct(listLeadDetail: Array<LeadDetailModel>) {
    this.SumAmount = 0;
    listLeadDetail.forEach(x => {
      this.SumAmount += x.SumAmount;
    })
    this.SumProduct = listLeadDetail.length;
  }

  calForecastSales(){
    let _expectedSale = ParseStringToFloat(this.editLeadForm.get('ExpectedSale').value);
    let _percent = ParseStringToFloat(this.editLeadForm.get('Percent')?.value);
    let _forecastSales = (_expectedSale * (_percent || 0)) / 100;
    this.editLeadForm.get('ForecastSales')?.setValue(_forecastSales);
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
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.toString().replace(/,/g, '');
  return parseFloat(str);
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
