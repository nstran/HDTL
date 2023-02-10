
import { Component, OnInit, ViewChild, ChangeDetectorRef, ElementRef } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService, MenuItem, MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { FileUpload } from 'primeng/fileupload';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { NoteModel } from '../../../shared/models/note.model';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { NoteService } from '../../../shared/services/note.service';
import { EmployeeService } from '../../services/employee.service';
import { Vacancies } from '../../models/recruitment-vacancies.model';
import { Table } from 'primeng/table';
import { EmployeeModel, CreateEmployeeModel } from '../../models/employee.model';
import { ContactModel } from '../../../../app/shared/models/contact.model';
import { UserModel } from '../../../../app/shared/models/user.model';
import { saveAs } from "file-saver";
import { Workbook } from 'exceljs';
import * as XLSX from 'xlsx';
import { CandidateImportDetailComponent } from '../candidate-import-detail/candidate-import-detail.component';
import { GetPermission } from '../../../shared/permission/get-permission';
import { TemplateVacanciesEmailComponent } from './../../../shared/components/template-vacancies-email/template-vacancies-email.component';

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
}

class EmployeeCandidateModel {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  employeeCodeName: string;
  phone: string;
}


class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}
class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileFullName: string;
  fileName: string;
  objectId: string;
  objectNumber: number;
  fileUrl: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
  createdById: string;
  createdDate: Date;
  uploadByName: string;
}

class CandidateModel {
  candidateId: string;
  vacanciesId: string;
  statusId: string;
  recruitmentCampaignId: string;
  fullName: string;
  dateOfBirth: Date;
  sex: number;
  address: string;
  phone: string;
  email: string;
  recruitmentChannel: string;
  applicationDate: Date;
}
interface FileNameExists {
  oldFileName: string;
  newFileName: string
}

class InterviewSheduleModel {
  interviewScheduleId: string;
  vacanciesId: string;
  candidateId: string;
  fullName: string;
  interviewTitle: string;
  interviewDate: Date;
  address: string;
  email: string;
  interviewScheduleType: number;
  listEmployeeId: Array<any> = [];
}

class SaleBiddingDetail {
  saleBiddingDetailId: string
  saleBiddingId: string;
  category: string;
  content: string;
  note: string;
  file: File[];
  listFile: Array<FileInFolder>;
  index: number;
}

class SaleBidding {
  saleBiddingId: string;
  saleBiddingName: string;
  saleBiddingCode: string;
  leadId: string;
  customerId: string;
  valueBid: number;
  startDate: Date;
  address: string;
  bidStartDate: Date;
  personInChargeId: string;
  effecTime: number;
  endDate: Date;
  typeContractId: string;
  formOfBid: string;
  currencyUnitId: string;
  ros: number;
  provisionalGrossProfit: number;
  typeContractName: string;
  slowDay: number;
  contactId: string;
  employeeId: string; // Nhân viên bán hàng
  statusId: string;
  leadName: string;
  leadCode: string;
  updatedById: string;
}

class InforExportExcel {
  companyName: string;
  address: string;
  phone: string;
  website: string;
  email: string;
  textTotalMoney: string
}

interface Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  employeeCodeName: string;
  isManager: boolean;
  positionId: string;
  organizationId: string;
}

class emailPVModel {
  listCandidate: Array<candidateSenEmailPVModel> = []; // danh sách email Ứng viên
  vancaciesName: string; // Vị trí tuyển dụng
  personInChagerName: string = ''; // tên người phụ trách
  personInChagerPhone: string = null; // SĐT người phụ trách
  workplace: string;
  subject: string;
  sendContent: string;
}
class candidateSenEmailPVModel {
  email: string;
  fullName: string;
  interviewTime: Date;
  addressOrLink: string;
  interviewScheduleType: number;
  listInterviewerEmail: Array<string> = []; //danh sách email Người PV
}

class CategoryModel {
  categoryId: string;
  categoryName: string;
  constructor() { }
}

class importCustomerByExcelModel {
  //candidateId: string;
  candidateName: string;
  dateOfBirth: Date;
  phone: string;
  sex: number;
  chanelCode: string;
  email: string;
  address: string;
  applicationDate: Date;
}

class ColumnExcel {
  code: string;
  name: string;
  width: number;
}

@Component({
  selector: 'app-tuyen-dung-detail',
  templateUrl: './tuyen-dung-detail.component.html',
  styleUrls: ['./tuyen-dung-detail.component.css']
})
export class TuyenDungDetailComponent implements OnInit {

  @ViewChild('fileNoteUpload') fileNoteUpload: FileUpload;
  loading: boolean = false;
  today: Date = new Date();
  statusCode: string = null;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  currentEmployeeCodeName = localStorage.getItem('EmployeeCodeName');
  auth = JSON.parse(localStorage.getItem('auth'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  viTriTuyenDungId: string = '';

  /*Valid Form*/
  isInvalidForm: boolean = false;
  isFromTo: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('fileUpload') fileUpload: FileUpload;

  selectedChienDich: any = null;
  // Danh sach
  actions: MenuItem[] = [];
  colsCandidate: any[];
  selectedInterviewColumns: any[];
  listCandidate: Array<any> = [];
  selectedCandidate: Array<any> = [];
  listPhuTrachViTriTuyenDung: Array<EmployeeCandidateModel> = [];
  listEmployeeInterview: Array<EmployeeCandidateModel> = [];
  listChienDich: Array<any> = [];
  listKinhNghiem: Array<any> = [];
  listMucUuTien: Array<any> = [
    {
      name: 'Cao',
      value: 1,
    },
    {
      name: 'Trung bình',
      value: 2,
    },
    {
      name: 'Thấp',
      value: 3,
    },
  ];
  listLoaiTienTe: Array<any> = [
    {
      name: 'VND',
      value: 'VND',
    },
    {
      name: 'USD',
      value: 'USD',
    },
  ];
  listLoaiCongViec: Array<any> = []
  listKieuLuong: Array<any> = [
    {
      name: 'Trong khoảng',
      value: 1,
    },
    {
      name: 'Thỏa thuận',
      value: 2,
    },
  ]
  listChanel: Array<CategoryModel> = []
  // Tài liệu liên quan
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];
  colsFile: any[];

  listCandidatePV: Array<InterviewSheduleModel> = [];
  // FORM
  thongTinViTriTuyenDungFormGroup: FormGroup;
  tenViTriTuyenDungFormControl: FormControl;
  soLuongFormControl: FormControl;
  mucUuTienFormControl: FormControl;
  nguoiPhuTrachFormControl: FormControl;
  loaiCongViecFormControl: FormControl;
  noiLamViecFormControl: FormControl;
  kinhNghiemFormControl: FormControl;
  loaiTienTeFormControl: FormControl;
  kieuLuongFormControl: FormControl;
  tuFormControl: FormControl;
  denFormControl: FormControl;
  viTriTuyenDungFormControl: FormControl;
  yeuCauChuyenMonFormControl: FormControl;
  quyenLoiUngVienFormControl: FormControl;

  //Note
  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  listUpdateNoteDocument: Array<NoteDocument> = [];
  noteHistory: Array<Note> = [];

  isAprovalQuote: boolean = false;
  noteId: string = null;
  noteContent: string = '';
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  uploadedNoteFiles: any[] = [];
  /*End : Note*/
  recruitmentCampaignId: string = '00000000-0000-0000-0000-000000000000';
  vacanciesId: string = '00000000-0000-0000-0000-000000000000';

  viTriTuyenDung: Vacancies = new Vacancies();
  displayAddCandidate: boolean = false;
  isUpdate: boolean = false;
  candidateViewDialogId: string = '00000000-0000-0000-0000-000000000000';
  statusIdDialog: string = '00000000-0000-0000-0000-000000000000';
  displayAddInterviewSche: boolean = false;

  // Dialog Thêm ứng viên
  createCandidateForm: FormGroup;
  vacanciesControl: FormControl;
  fullNameControl: FormControl;
  dateOfBirthControl: FormControl;
  phoneControl: FormControl;
  genderControl: FormControl;
  addressControl: FormControl;
  emailControl: FormControl;
  chanelControl: FormControl;
  applicationDateCandidateControl: FormControl;
  listDeleteNoteDocument: Array<NoteDocument> = [];
  // Lịch PV
  interViewForm: FormGroup;
  titleFormControl: FormControl;
  applicationDateControl: FormControl;
  addressOrLinkControl: FormControl;
  interViewNameControl: FormControl;

  emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
  genderModel: string = "1";
  filterCandidateInterviewGlobal: string = '';
  filterCandidateGlobal: string = '';

  @ViewChild('myTable') myTableCandidate: Table;
  @ViewChild('myTable') myTableInterview: Table;
  listInterviewType: Array<any> = [
    {
      categoryName: 'Trực tiếp',
      categoryId: 1,
    },
    {
      categoryName: 'Online',
      categoryId: 2,
    },
  ];

  // Show dialog gửi email
  displayOfferEmail: boolean = false;
  displayRejectEmail: boolean = false;
  password: string = this.systemParameterList.find(x => x.systemKey == "DefaultUserPassword").systemValueString;

  userModel: UserModel = {
    UserId: null, Password: this.password, UserName: '', EmployeeId: '', EmployeeCode: '', Disabled: false, CreatedById: 'DE2D55BF-E224-4ADA-95E8-7769ECC494EA', CreatedDate: null, UpdatedById: null, UpdatedDate: null, Active: true
  };
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới
  employeeModel = new CreateEmployeeModel()
  contactModel = new ContactModel()

  salebiddingModel: SaleBidding = new SaleBidding();
  month: string = '';
  year: number = (new Date()).getFullYear();
  inforExportExcel: InforExportExcel;
  listFileResult: Array<FileInFolder> = [];
  listFile: Array<FileInFolder> = [];
  headerPopupCandidate: string = 'Thêm nhanh ứng viên';
  fileName: string = '';
  displayChooseFileImportDialog: boolean = false;
  importFileExcel: any = null;
  showDialog: boolean = false;

  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  actionEdit: boolean = true;

  actionAddUngVien: boolean = true;

  isManagerOfHR: boolean = false;

  isGD: boolean = false;
  isNguoiPhuTrach: boolean = false;

  // Import ứng viên
  importCandidateForm: FormGroup;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private noteService: NoteService,
    public dialogService: DialogService,
    private messageService: MessageService,
    private imageService: ImageUploadService,
    private confirmationService: ConfirmationService,
    private employeeService: EmployeeService,
    private def: ChangeDetectorRef,
    private getPermission: GetPermission,

  ) { }

  async ngOnInit() {
    this.setForm();
    this.setTable();
    this.setControlDialog();

    let resource = "rec/employee/chi-tiet-cong-viec-tuyen-dung/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
      this.showMessage('warn', 'Bạn không có quyền truy cập');
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;

      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
    }

    //Cho tạo ứng viên
    let resource2 = "rec/employee/tao-ung-vien/";
    let permission2: any = await this.getPermission.getPermission(resource2);
    if (permission2.status == false) {
      this.actionAddUngVien = false;
    }
    else {
      let listCurrentActionResource = permission2.listCurrentActionResource;

      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAddUngVien = false;
      }
    }

    this.route.params.subscribe(params => {
      this.recruitmentCampaignId = params['recruitmentCampaignId'];
      this.vacanciesId = params['vacanciesId'];
    });

    this.getMasterData();
  }

  setTable() {
    this.colsFile = [
      { field: 'fileFullName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', color: '#f44336' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'right', color: '#f44336' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'center', color: '#f44336' },
      { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'left', color: '#f44336' },
    ];

    this.colsCandidate = [
      { field: 'index', header: '#', width: '5%', textAlign: 'center', color: '#f44336', excelWidth: 23 },
      { field: 'fullName', header: 'Tên', width: '20%', textAlign: 'left', color: '#f44336', excelWidth: 30 },
      { field: 'applicationDate', header: 'Ngày ứng tuyển', width: '10%', textAlign: 'center', color: '#f44336', excelWidth: 20 },
      { field: 'email', header: 'Email', width: '15%', textAlign: 'left', color: '#f44336', excelWidth: 30 },
      { field: 'phone', header: 'SĐT', width: '15%', textAlign: 'left', color: '#f44336', excelWidth: 20 },
      { field: 'recruitmentChannelName', header: 'Nguồn đăng tuyển', width: '15%', textAlign: 'left', color: '#f44336', excelWidth: 50 },
      { field: 'statusName', header: 'Trạng thái', width: '10%', textAlign: 'center', color: '#f44336', excelWidth: 20 },
      { field: 'thaoTac', header: 'Thao tác', width: '10%', textAlign: 'center', color: '#f44336', excelWidth: 10 },
    ];
    this.selectedInterviewColumns = [
      { field: 'index', header: 'STT', width: '10%', textAlign: 'center', color: '#f44336' },
      { field: 'fullName', header: 'Tên', width: '20%', textAlign: 'left', color: '#f44336' },
      { field: 'interViewName', header: 'Người phỏng vấn', width: '18%', textAlign: 'left', color: '#f44336' },
      { field: 'interviewType', header: 'Loaị phỏng vấn', width: '12%', textAlign: 'center', color: '#f44336' },
      { field: 'address', header: 'Địa chỉ/ Link', width: '25%', textAlign: 'left', color: '#f44336' },
      { field: 'interviewDate', header: 'Ngày phỏng vấn', width: '15%', textAlign: 'left', color: '#f44336' },
    ];
  }

  setForm() {
    this.tenViTriTuyenDungFormControl = new FormControl(null, [Validators.required]);
    this.soLuongFormControl = new FormControl(null);
    this.mucUuTienFormControl = new FormControl(null, [Validators.required]);
    this.nguoiPhuTrachFormControl = new FormControl(null);
    this.loaiCongViecFormControl = new FormControl(null);
    this.noiLamViecFormControl = new FormControl(null);
    this.kinhNghiemFormControl = new FormControl(null);
    this.loaiTienTeFormControl = new FormControl(null);
    this.kieuLuongFormControl = new FormControl(null);
    this.tuFormControl = new FormControl(null);
    this.denFormControl = new FormControl(null);
    this.viTriTuyenDungFormControl = new FormControl(null);
    this.yeuCauChuyenMonFormControl = new FormControl(null);
    this.quyenLoiUngVienFormControl = new FormControl(null);

    this.thongTinViTriTuyenDungFormGroup = new FormGroup({
      tenViTriTuyenDungFormControl: this.tenViTriTuyenDungFormControl,
      soLuongFormControl: this.soLuongFormControl,
      mucUuTienFormControl: this.mucUuTienFormControl,
      nguoiPhuTrachFormControl: this.nguoiPhuTrachFormControl,
      loaiCongViecFormControl: this.loaiCongViecFormControl,
      noiLamViecFormControl: this.noiLamViecFormControl,
      kinhNghiemFormControl: this.kinhNghiemFormControl,
      loaiTienTeFormControl: this.loaiTienTeFormControl,
      kieuLuongFormControl: this.kieuLuongFormControl,
      tuFormControl: this.tuFormControl,
      denFormControl: this.denFormControl,
      viTriTuyenDungFormControl: this.viTriTuyenDungFormControl,
      yeuCauChuyenMonFormControl: this.yeuCauChuyenMonFormControl,
      quyenLoiUngVienFormControl: this.quyenLoiUngVienFormControl,
    });
  }
  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }
  setControlDialog() {
    this.vacanciesControl = new FormControl('');
    this.fullNameControl = new FormControl('', Validators.required);
    this.phoneControl = new FormControl('', [Validators.required, Validators.pattern(this.getPhonePattern())]);
    this.dateOfBirthControl = new FormControl(null, Validators.required);
    this.genderControl = new FormControl(1, [Validators.required]);
    this.addressControl = new FormControl('');
    this.emailControl = new FormControl(null);
    this.chanelControl = new FormControl(null);
    this.applicationDateCandidateControl = new FormControl(null);

    this.titleFormControl = new FormControl('', Validators.required);
    this.addressOrLinkControl = new FormControl('', Validators.required);
    this.applicationDateControl = new FormControl(null, Validators.required);
    this.interViewNameControl = new FormControl(null, Validators.required);

    this.createCandidateForm = new FormGroup({
      vacanciesControl: this.vacanciesControl,
      fullNameControl: this.fullNameControl,
      phoneControl: this.phoneControl,
      dateOfBirthControl: this.dateOfBirthControl,
      genderControl: this.genderControl,
      addressControl: this.addressControl,
      emailControl: this.emailControl,
      chanelControl: this.chanelControl,
      applicationDateCandidateControl: this.applicationDateCandidateControl
    });

    this.interViewForm = new FormGroup({
      interViewNameControl: this.interViewNameControl,
      titleFormControl: this.titleFormControl,
      addressOrLinkControl: this.addressOrLinkControl,
      applicationDateControl: this.applicationDateControl,
    });
  }

  async getMasterData() {
    this.loading = true;

    let result: any = await this.employeeService.getMasterDataVacancies(this.vacanciesId);
    if (result.statusCode == 200) {

      this.listPhuTrachViTriTuyenDung = result.listEmployeePTTD;
      this.listChienDich = result.listEmployeeRecruit;
      this.listKinhNghiem = result.listKinhNghiem;
      this.listLoaiCongViec = result.listLoaiCV;
      this.noteHistory = result.listNote;
      this.listChanel = result.listChanel;
      this.listEmployeeInterview = result.listAllEmployee;
      this.viTriTuyenDung = result.viTriTuyenDung;

      this.isManagerOfHR = result.isManagerOfHR;
      this.isGD = result.isGD;
      this.isNguoiPhuTrach = result.isNguoiPhuTrach;

      this.listCandidate = result.listCandidate;
      this.setColorOfStatusCandidate();
      this.inforExportExcel = result.inforExportExcel;
      this.listFileResult = result.listFileResult;
      this.setDefaultValueForm();
      this.handleNoteContent();
      this.loading = false;
    }
    else
      this.loading = false;
  }

  setColorOfStatusCandidate() {
    this.listCandidate.forEach(item => {
      switch (item.statusCode) {
        case "UVMOI":
          item.backgroundColorForStatus = '#c4c3d0';
          break;
        case "UVHPV":
          item.backgroundColorForStatus = '#87cefa';
          break;
        case "UVDPV":
          item.backgroundColorForStatus = '#29ab87';
          break;
        case "UVGOF":
          item.backgroundColorForStatus = '#4cbb17';
          break;
        case "UVTC":
          item.backgroundColorForStatus = '#ee6363';
          break;
        case "UVKD":
          item.backgroundColorForStatus = '#FF0000';
          break;
      }
    });
  }

  async setDefaultValueForm() {

    // set dữ liệu Chiến dịch
    let reCamp = this.listChienDich.find(e => e.recruitmentCampaignId == this.recruitmentCampaignId);
    if (reCamp) {
      this.selectedChienDich = reCamp;
    }
    // tên vị trí
    this.tenViTriTuyenDungFormControl.setValue(this.viTriTuyenDung.vacanciesName);

    // Số lượng tuyển dụng
    this.soLuongFormControl.setValue(this.viTriTuyenDung.quantity);

    // Mức ưu tiên
    let uuTien = this.listMucUuTien.find(e => e.value == this.viTriTuyenDung.priority);
    if (uuTien) {
      this.mucUuTienFormControl.setValue(uuTien);
    }

    // Loai tiền tệ
    let loaiTien = this.listLoaiTienTe.find(e => e.value == this.viTriTuyenDung.currency);
    if (loaiTien) {
      this.loaiTienTeFormControl.setValue(loaiTien);
    }

    // Người phụ trách
    let nguoiPT = this.listPhuTrachViTriTuyenDung.find(e => e.employeeId == this.viTriTuyenDung.personInChargeId);
    if (nguoiPT) {
      this.nguoiPhuTrachFormControl.setValue(nguoiPT);
    }
    // Loại CV
    let loaiCV = this.listLoaiCongViec.find(e => e.categoryId == this.viTriTuyenDung.typeOfWork);
    if (loaiCV) {
      this.loaiCongViecFormControl.setValue(loaiCV);
    }
    // Kiểu lương
    let kieuLuong = this.listKieuLuong.find(e => e.value == this.viTriTuyenDung.salarType);
    if (kieuLuong) {
      this.kieuLuongFormControl.setValue(kieuLuong);
      if (kieuLuong.value == 1) {
        // Khoảng tiền
        this.tuFormControl.setValue(this.viTriTuyenDung.salaryFrom);
        this.denFormControl.setValue(this.viTriTuyenDung.salaryTo);
        this.isFromTo = true;
      }
    }
    // Nơi làm việc
    this.noiLamViecFormControl.setValue(this.viTriTuyenDung.placeOfWork);

    // Kinh nghiệm
    let kingNghiem = this.listKinhNghiem.find(e => e.categoryId == this.viTriTuyenDung.experienceId);
    if (kingNghiem) {
      this.kinhNghiemFormControl.setValue(kingNghiem);
    }

    // Vị trí tuyển dụng
    this.viTriTuyenDungFormControl.setValue(this.viTriTuyenDung.vacanciesDes);
    // Yêu cầu chuyển môn
    this.yeuCauChuyenMonFormControl.setValue(this.viTriTuyenDung.professionalRequirements);
    // Quyền lợi ứng viên
    this.quyenLoiUngVienFormControl.setValue(this.viTriTuyenDung.candidateBenefits);

    // Tài liệu liên quan

    // this.listCandidate.forEach(item => {
    //   this.actions = [];

    // });
  }

  changeSalary(event: any) {

    if (event.value.value == "1")
      this.isFromTo = true;
    else
      this.isFromTo = false;
  }


  goBackToList() {
    this.router.navigate(['/employee/danh-sach-cong-viec-tuyen-dung']);
  }

  goDetailCandidate(rowData) {
    let url = this.router.serializeUrl(this.router.createUrlTree(['/employee/chi-tiet-ung-vien', { candidateId: rowData.candidateId }]));
    window.open(url, '_blank');
    // this.headerPopupCandidate = "Thông tin ứng viên";
    // // show popup chi tiết ứng viên
    // this.displayAddCandidate = true;
    // this.isUpdate = true;
    // this.createCandidateForm.reset();
    // this.candidateViewDialogId = rowData.candidateId;
    // this.statusIdDialog = rowData.status;
    // this.vacanciesControl.setValue(this.viTriTuyenDung.vacanciesName);
    // this.fullNameControl.setValue(rowData.fullName);
    // this.dateOfBirthControl.setValue(rowData.dateOfBirth ? new Date(rowData.dateOfBirth) : null);
    // this.phoneControl.setValue(rowData.phone);

    // this.genderControl.setValue(rowData.sex);
    // // Kênh
    // let kenhtd = this.listChanel.find(x => x.categoryId == rowData.recruitmentChannelId);
    // this.chanelControl.setValue(kenhtd ? kenhtd : null);

    // this.emailControl.setValue(rowData.email);

    // this.addressControl.setValue(rowData.address);
    // // Ngày ứng tuyển
    // this.applicationDateCandidateControl.setValue(rowData.applicationDate ? new Date(rowData.applicationDate) : null);
  }

  btn_updateVacancies() {

    let viTriTyenDung: Vacancies = this.mapDataToModel();
    this.updateVacancies(viTriTyenDung);
  }


  updateVacancies(viTriTyenDung: any) {
    this.employeeService.updateVacancies(viTriTyenDung).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        let messageCode = "Cập nhập chiến dịch thành công";
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: messageCode };
        this.showMessageN(mgs);
        this.getMasterData();
      } else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessageN(mgs);
      }
    });
  }

  mapDataToModel(): Vacancies {
    let viTriTD = new Vacancies();

    viTriTD.vacanciesId = this.vacanciesId;
    // Mã chiến dịch
    viTriTD.recruitmentCampaignId = this.recruitmentCampaignId;

    viTriTD.vacanciesName = this.tenViTriTuyenDungFormControl.value ?.trim();

    // Người phụ trách tuyển dụng
    let personInCharge: Employee = this.nguoiPhuTrachFormControl.value != null ? this.nguoiPhuTrachFormControl.value : null;
    if (personInCharge) {
      viTriTD.personInChargeId = personInCharge.employeeId;
    } else {
      viTriTD.personInChargeId = this.emptyGuid;
    }

    // Loại công việc
    let loaiCV = this.loaiCongViecFormControl.value != null ? this.loaiCongViecFormControl.value : null;
    if (loaiCV) {
      viTriTD.typeOfWork = loaiCV.categoryId;
    } else {
      viTriTD.typeOfWork = this.emptyGuid;
    }

    // Kinh Nghiệm
    let kinhNghiem = this.kinhNghiemFormControl.value != null ? this.kinhNghiemFormControl.value : null;
    if (kinhNghiem) {
      viTriTD.experienceId = kinhNghiem.categoryId;
    } else {
      viTriTD.experienceId = this.emptyGuid;
    }

    viTriTD.quantity = this.soLuongFormControl == null ? 0 : ParseStringToFloat(this.soLuongFormControl.value);
    viTriTD.placeOfWork = this.noiLamViecFormControl.value;

    // Mức ưu tiên
    let mucUuTien = this.mucUuTienFormControl.value != null ? this.mucUuTienFormControl.value : null;
    if (mucUuTien) {
      viTriTD.priority = mucUuTien.value;
    } else {
      viTriTD.priority = 0;
    }

    // Loại tiền tệ
    let loaiTien = this.loaiTienTeFormControl.value != null ? this.loaiTienTeFormControl.value : null;
    if (loaiTien) {
      viTriTD.currency = loaiTien.value;
    } else {
      viTriTD.currency = '';
    }

    // Kiểu lương
    viTriTD.salarType = this.kieuLuongFormControl.value != null ? this.kieuLuongFormControl.value.value : null;

    let luongTu = this.tuFormControl.value != null ? ParseStringToFloat(this.tuFormControl.value) : 0;
    viTriTD.salaryFrom = luongTu;

    let luongDen = this.denFormControl.value != null ? ParseStringToFloat(this.denFormControl.value) : 0;
    viTriTD.salaryTo = luongDen;

    viTriTD.vacanciesDes = this.thongTinViTriTuyenDungFormGroup.get('viTriTuyenDungFormControl').value != null ? this.thongTinViTriTuyenDungFormGroup.get('viTriTuyenDungFormControl').value : null;

    viTriTD.professionalRequirements = this.thongTinViTriTuyenDungFormGroup.get('yeuCauChuyenMonFormControl').value
      != null ? this.thongTinViTriTuyenDungFormGroup.get('yeuCauChuyenMonFormControl').value : null;

    viTriTD.candidateBenefits = this.thongTinViTriTuyenDungFormGroup.get('quyenLoiUngVienFormControl').value
      != null ? this.thongTinViTriTuyenDungFormGroup.get('quyenLoiUngVienFormControl').value : null;

    return viTriTD;
  }

  //#region   GHI CHÚ
  /*Event Lưu các file được chọn*/
  handleFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;
      if (size <= this.defaultLimitedFileSize) {
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
  deleteFile(file: any) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.employeeService.deleteFile(file.fileInFolderId).subscribe(res => {
          let result: any = res;
          if (result.statusCode == 200) {
            this.listFileResult = this.listFileResult.filter(x => x.fileInFolderId != file.fileInFolderId);

            let msg = { severity: 'success', summary: 'Thông báo', detail: 'Xóa file thành công' };
            this.showMessageN(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
            this.showMessageN(msg);
          }
        })
      }
    });
  }

  /*Event upload list file*/
  myUploader(event) {

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
      fileUpload.FileInFolder.objectId = this.vacanciesId;
      fileUpload.FileInFolder.objectType = 'VACANCIES';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    this.employeeService.uploadFile("VACANCIES", listFileUploadModel, this.vacanciesId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.uploadedFiles = [];

        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }

        this.listFileResult = result.listFileInFolder;
        let msg = { severity: 'success', summary: 'Thông báo', detail: "Thêm file thành công" };
        this.showMessageN(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessageN(msg);
      }
    });
  }


  // Event thay đổi nội dung ghi chú
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
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

            this.showMessage('success', 'Xóa ghi chú thành công');
          } else {
            this.showMessage('error', result.messageCode);
          }
        });
      }
    });
  }

  /* Event thêm file dược chọn vào list file note */
  handleNoteFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= this.defaultLimitedFileSize) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedNoteFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedNoteFiles.push(file);
          }
        }
      }
    }
  }

  /*Event khi click xóa từng file */
  removeNoteFile(event) {
    let index = this.uploadedNoteFiles.indexOf(event.file);
    this.uploadedNoteFiles.splice(index, 1);
  }
  /*End*/

  /*Event khi click xóa toàn bộ file */
  clearAllNoteFile() {
    this.uploadedNoteFiles = [];
  }

  cancelNote() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn hủy ghi chú này?',
      accept: () => {
        this.noteId = null;
        this.noteContent = null;
        this.uploadedNoteFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();
        }
        this.listUpdateNoteDocument = [];
        this.isEditNote = false;
      }
    });
  }

  /*Lưu file và ghi chú vào Db*/
  async saveNote() {
    let objectType = 'VACANCIES';
    this.loading = true;
    this.listNoteDocumentModel = [];

    let listFileUploadModel: Array<FileUploadModel> = [];
    this.uploadedNoteFiles.forEach(item => {
      let fileUpload: FileUploadModel = new FileUploadModel();
      fileUpload.FileInFolder = new FileInFolder();
      fileUpload.FileInFolder.active = true;
      let index = item.name.lastIndexOf(".");
      let name = item.name.substring(0, index);
      fileUpload.FileInFolder.fileName = name;
      fileUpload.FileInFolder.fileExtension = item.name.substring(index + 1);
      fileUpload.FileInFolder.size = item.size;
      fileUpload.FileInFolder.objectType = objectType;
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    let noteModel = new NoteModel();
    if (!this.noteId) {
      /*Tạo mới ghi chú*/
      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.vacanciesId;
      noteModel.ObjectType = objectType;
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/
      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.vacanciesId;
      noteModel.ObjectType = objectType;
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    }
    if (noteModel.Description == "" && this.listNoteDocumentModel.length == 0) {

      this.loading = false;
      return;
    }

    this.listDeleteNoteDocument.forEach(item => {
      // Xóa file vật lý
      let result: any = this.imageService.deleteFileForOptionAsync(objectType, item.documentName);

      // xóa file trong DB
      this.noteService.deleteNoteDocument(item.noteId, item.noteDocumentId).subscribe(response => {
        let result: any = response;
        // this.loading = false;
        if (result.statusCode != 200) {
          let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
          this.showMessageN(msg);
        }
      });
    })

    this.noteHistory = [];
    this.noteService.createNoteForAllRecruitmentCampaign(noteModel, objectType, listFileUploadModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedNoteFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.noteContent = null;
        this.listUpdateNoteDocument = [];
        this.noteId = null;
        this.isEditNote = false;

        /*Reshow Time Line*/
        this.noteHistory = result.listNote;
        this.handleNoteContent();

        this.showMessage('success', 'Thêm ghi chú thành công');
      } else {
        this.showMessage('error', result.messageCode);
      }
    });
  }

  /** Xử lý và hiển thị lại nội dung ghi chú */
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

  //#endregion

  //#region   DANH SÁCH ỨNG VIÊN

  onChangeAction(rowData: any) {
    this.actions = [];

    // Hẹn PV
    let item1: MenuItem = {
      id: '1', label: 'Hẹn phỏng vấn', command: () => {
        this.addInterviewSchedule(rowData);
      }
    }

    // Gửi offer
    let item2: MenuItem = {
      id: '2', label: 'Gửi offer', command: () => {
        this.listCandidatePV = []
        this.listCandidatePV.push(rowData);

        let emailData = new emailPVModel();
        emailData = this.getInfomationContentEmail();

        let ref = this.dialogService.open(TemplateVacanciesEmailComponent, {
          data: {
            emailType: 'OFFER',
            emailData: emailData
          },
          header: 'Gửi email offer',
          width: '65%',
          baseZIndex: 1030,
          contentStyle: {
            "min-height": "190px",
            "max-height": "800px",
            "overflow": "auto"
          }
        });

        ref.onClose.subscribe(async (result: any) => {
          if (result) {
            if (result.status) {
              this.loading = true;
              let lstCandidateId: Array<string> = []

              lstCandidateId.push(rowData.candidateId);
              let result: any = await this.employeeService.updateStatusCandidateFromVacancies(lstCandidateId, 4, this.vacanciesId);
              if (result.statusCode == 200) {
                this.listCandidate = result.listCandidate;
                this.setColorOfStatusCandidate();
                this.loading = false;
                this.showMessage('success', 'Cập nhập trạng thái thành công.');
              }
              else
                this.loading = false;
            }
          }
          this.listCandidatePV = []
        });
      }
    }

    // Từ chối offer
    let item3: MenuItem = {
      id: '3', label: 'Từ chối offer', command: async () => {
        await this.updateCandidate(rowData, 5);
      }
    }

    // Không đạt
    let item4: MenuItem = {
      id: '4', label: 'Không đạt', command: async () => {
        this.listCandidatePV.push(rowData)
        let emailData = new emailPVModel();
        emailData = this.getInfomationContentEmail();

        let ref = this.dialogService.open(TemplateVacanciesEmailComponent, {
          data: {
            emailType: 'FAIL',
            emailData: emailData
          },
          header: 'Gửi email thông báo kết quả - Không đạt',
          width: '65%',
          baseZIndex: 1030,
          contentStyle: {
            "min-height": "190px",
            "max-height": "800px",
            "overflow": "auto"
          }
        });

        ref.onClose.subscribe(async (result: any) => {
          if (result) {
            if (result.status) {
              this.loading = true;
              let lstCandidateId: Array<string> = []
              lstCandidateId.push(rowData.candidateId);
              let result: any = await this.employeeService.updateStatusCandidateFromVacancies(lstCandidateId, 6, this.vacanciesId);
              if (result.statusCode == 200) {
                this.listCandidate = result.listCandidate;
                this.setColorOfStatusCandidate();
                this.loading = false;
                this.showMessage('success', 'Cập nhập trạng thái thành công.');
              }
              else
                this.loading = false;
            }
            else
              this.loading = false;
          }
          this.listCandidatePV = []
        });
      }
    }

    // Chuyển thành NV
    let item5: MenuItem = {
      id: '5', label: 'Chuyển thành NV', command: async () => {
        this.convertToEmployee(rowData);
      }
    }

    // Xóa
    let item6: MenuItem = {
      id: '6', label: 'Xóa', command: async () => {
        this.deleteCandidate(rowData);
      }
    }

    // Đạt PV
    let item7: MenuItem = {
      id: '7', label: 'Đạt phỏng vấn', command: async () => {
        this.updateCandidate(rowData, 3);
      }
    }

    // Trạng thái của từng dòng
    switch (rowData.status) {
      case 1:// Mới
        this.actions.push(item1);// hẹn PV
        this.actions.push(item5);// Chuyển NV
        this.actions.push(item6);// Xóa
        break;
      case 2:  // Hẹn phỏng vấn
        this.actions.push(item1);// hẹn PV
        this.actions.push(item7);// Đạt PV
        this.actions.push(item4);// Không Đạt
        this.actions.push(item2);// Gửi offer
        break;

      case 3: // Đạt phỏng vấn
        this.actions.push(item1);//hẹn PV
        this.actions.push(item2);// Gửi offer
        this.actions.push(item4);// Không Đạt
        this.actions.push(item5);// Chuyển NV
        break;

      case 4:// Gửi offer
        this.actions.push(item1);//hẹn PV
        this.actions.push(item2);// Gửi offer
        this.actions.push(item3);// Từ chối Offer
        this.actions.push(item5);// Chuyển NV
        break;

      case 5: // Từ chối offer
        this.actions.push(item1);//hẹn PV
        this.actions.push(item2);// Gửi offer
        this.actions.push(item5);// Chuyển NV
        break;
      case 6: // Khoogn đạt
        this.actions.push(item2);// Gửi offer
        this.actions.push(item7);// Đạt PV
        break;
    }

    //Nếu là người phụ trách hoặc trưởng nhan sự mới được thao tác với ứng viên
    if (this.isManagerOfHR || this.isNguoiPhuTrach) {
    } else {
      this.actions = [];
    }
  }

  // Tạo lịch PV cho 1 hoặc nhiều -- Nút trên header
  addInterviewSchedule(rowData: any) {

    this.listCandidatePV = []
    let interview = new InterviewSheduleModel();
    this.interViewForm.reset();
    // Tiêu đề phỏng vấn
    let title = 'Phỏng vấn ' + this.viTriTuyenDung.vacanciesName;
    this.titleFormControl.setValue(title);
    if (rowData !== null && rowData != undefined) {
      this.displayAddInterviewSche = true;
      interview.candidateId = rowData.candidateId;
      interview.fullName = rowData ?.fullName;
      interview.email = rowData ?.email;
      interview.interviewScheduleType = 1; // 1 Trực tiếp -- 2 Online
      this.listCandidatePV.push(interview)
    }
    else {
      if (this.selectedCandidate.length > 0) {
        this.displayAddInterviewSche = true;
        this.selectedCandidate.forEach(can => {
          interview = new InterviewSheduleModel();
          interview.candidateId = can.candidateId;
          interview.fullName = can ?.fullName;
          interview.email = can ?.email;
          interview.interviewScheduleType = 1; // 1 Trực tiếp -- 2 Online
          this.listCandidatePV.push(interview)
        });
      }
      else {
        if (this.listCandidate.filter(x => x.statusCode == "UVMOI").length > 0) {
          this.displayAddInterviewSche = true;
          this.listCandidate.filter(x => x.statusCode == "UVMOI").forEach(candi => {
            interview = new InterviewSheduleModel();
            interview.candidateId = candi.candidateId;
            interview.fullName = candi ?.fullName;
            interview.email = candi ?.email;
            interview.interviewScheduleType = 1; // 1 Trực tiếp -- 2 Online
            this.listCandidatePV.push(interview)
          });
        }
        else
          this.showMessage('error', 'Vui lòng chọn ứng viên có trạng thái mới.');
      }
    }
  }

  getInfomationContentEmail(emailType: string = null) {

    let emailData = new emailPVModel();
    let candi = new candidateSenEmailPVModel();
    this.listCandidatePV.forEach(item => {
      if (item.email !== null) {
        candi = new candidateSenEmailPVModel()
        candi.email = item.email;
        candi.fullName = item.fullName;
        candi.interviewTime = item.interviewDate;
        candi.addressOrLink = item.address;
        candi.interviewScheduleType = item.interviewScheduleType;
        if (emailType == "PHONG_VAN")
          item.listEmployeeId.forEach(emp => {
            candi.listInterviewerEmail.push(emp.email);
          });
        emailData.listCandidate.push(candi)
      }
    });

    // Vị trí tuyên dụng
    emailData.vancaciesName = this.selectedChienDich.recruitmentCampaignName;
    let nguoiPT = this.listPhuTrachViTriTuyenDung.find(e => e.employeeId == this.viTriTuyenDung.personInChargeId);
    if (nguoiPT != undefined) {
      // Người phụ trách
      emailData.personInChagerName = nguoiPT.employeeName;
      emailData.personInChagerPhone = nguoiPT.phone;
    }

    emailData.workplace = this.viTriTuyenDung.placeOfWork;

    this.listCandidatePV = []
    return emailData;
  }

  // TẠO LỊCH PV SAU GỬI EMAIL CHO 1 HOẶC NHIỀU NGƯỜI
  async saveAddIntervivewSheDialog() {

    if (!this.interViewForm.valid) {
      Object.keys(this.interViewForm.controls).forEach(key => {
        if (this.interViewForm.controls[key].valid == false) {
          this.interViewForm.controls[key].markAsTouched();
        }
      });
      this.showMessage('error', 'Vui lòng nhập đầy đủ thông tin các trường dữ liệu.');
    }
    else {
      let lstInterviewModel: Array<InterviewSheduleModel> = this.mappingInterviewSheduleFormToModel();

      // let emailData = new emailPVModel();
      // emailData = this.getInfomationContentEmail("PHONG_VAN");
      // // show popup gửi email
      // let ref = this.dialogService.open(TemplateVacanciesEmailComponent, {
      //   data: {
      //     emailData: emailData,
      //     emailType: 'PHONG_VAN'
      //   },
      //   header: 'Gửi email mời tham dự phỏng vấn',
      //   width: '65%',
      //   baseZIndex: 1030,
      //   contentStyle: {
      //     "min-height": "190px",
      //     "max-height": "800px",
      //     "overflow": "auto"
      //   }
      // });

      // ref.onClose.subscribe(async (result: any) => {

      //   if (result) {
      //     if (result.status) {
      this.loading = true;
      // Tạo lịch PV
      let result: any = await this.employeeService.createInterviewSchedule(lstInterviewModel);

      if (result.statusCode == 200) {
        this.loading = false;
        this.displayAddInterviewSche = false;
        this.listCandidate = []
        this.listCandidate = result.listCandidate;
        this.setColorOfStatusCandidate();
        this.showMessage('success', 'Tạo lịch phỏng vấn thành công.');
      }

      this.month = ((new Date).getMonth() + 1).toString();
      this.year = (new Date).getFullYear();

      let listInvalidEmail: Array<string> = result.listInvalidEmail;
      if (listInvalidEmail != null && listInvalidEmail != undefined) {
        let message = `Gửi email thành công. Có <strong>${listInvalidEmail.length} email</strong> không hợp lệ:<br/>`
        listInvalidEmail.forEach(item => {
          message += `<div style="padding-left: 30px;"> -<strong>${item}</strong></div>`
        });
        if (listInvalidEmail.length > 0) {
          this.confirmationService.confirm({
            message: message,
            rejectVisible: false,
          });
        }
      }
      let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Gửi email thành công' };
      this.showMessageN(msg);
      //     }
      //   }
      // });
      //}
      // else {
      //   this.loading = false;
      //   this.showMessage('error', 'Tạo lịch phỏng vấn thất bại.');
      // }
    }
  }

  mappingInterviewSheduleFormToModel() {

    let lstInterviewModel = new Array<InterviewSheduleModel>();
    this.listCandidatePV.forEach(item => {
      let interviewModel = new InterviewSheduleModel();
      interviewModel.interviewScheduleId = this.emptyGuid;
      interviewModel.vacanciesId = this.vacanciesId;
      interviewModel.candidateId = item.candidateId;

      interviewModel.interviewTitle = this.interViewForm.get('titleFormControl').value == null ? null : this.interViewForm.get('titleFormControl').value;
      interviewModel.interviewDate = item.interviewDate;
      interviewModel.address = item.address;
      interviewModel.interviewScheduleType = item.interviewScheduleType;

      item.listEmployeeId.forEach(emp => {
        interviewModel.listEmployeeId.push(emp.employeeId);
      });
      lstInterviewModel.push(interviewModel);
    });
    return lstInterviewModel;
  }

  cancelAddIntervivewSheDialog() {
    this.confirmationService.confirm({
      message: 'Bạn có muốn thoát không?',
      accept: async () => {
        this.displayAddInterviewSche = false;
      }
    });
  }

  // CHUYỂN THÀNH NV / THỬ VIỆC
  convertToEmployee(rowData: any) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn chuyển ứng viên thành nhân viên không?"',
      accept: () => {

        // Split tên Ứng viên
        let tenKhongDau = cleanAccents(rowData.fullName);
        let splitTenKD = tenKhongDau.split(" ");
        let name = '';
        let ho = '';
        let tenDem = '';
        let chuCaiDau = '';
        let chuCaiSau = '';

        if (splitTenKD.length == 1)
          name = splitTenKD[0];
        else {
          name = splitTenKD[splitTenKD.length - 1];
          ho = splitTenKD[0];
          chuCaiDau = ho.charAt(0);
          if (splitTenKD.length > 2) {
            tenDem = splitTenKD[1];
            chuCaiSau = tenDem.charAt(0);
          }
        }

        let accountName = name + chuCaiDau + chuCaiSau;
        let lstCandidateId: Array<string> = [];
        lstCandidateId.push(rowData.candidateId);

        // Lấy giá trị cho employee model
        this.employeeModel.HoTenTiengAnh = rowData.fullName.trim();
        this.employeeModel.CreatedDate = new Date();
        this.employeeModel.QuocTich = '';
        this.employeeModel.DanToc = '';
        this.employeeModel.TonGiao = '';
        this.employeeModel.StartDateMayChamCong = new Date();
        this.employeeModel.PositionId = this.emptyGuid;
        let isAccessable = true;

        // Lấy giá trị cho contact employee
        this.contactModel.ContactId = this.emptyGuid;
        this.contactModel.ObjectId = this.emptyGuid;

        this.contactModel.CreatedDate = new Date();
        this.contactModel.CreatedById = this.auth.UserId;
        this.contactModel.FirstName = rowData.fullName.trim()
        this.contactModel.LastName = '';
        this.contactModel.Email = rowData.email;
        this.contactModel.Phone = rowData.phone ?.value;
        this.contactModel.Gender = rowData.sex;
        this.contactModel.CreatedById = this.auth.UserId;
        this.contactModel.CreatedDate = new Date();

        // Accoutn đăng nhập
        this.userModel.UserName = accountName;
        //this.userModel.Password = "1";

        //List Phòng ban
        let listPhongBanId: Array<any> = [];

        let fileBase64 = {
          "Extension": null, /*Định dạng ảnh (jpg, png,...)*/
          "Base64": null /*Định dạng base64 của ảnh*/
        }

        this.loading = true;
        this.awaitResult = true;

        this.employeeService.createEmployee(this.employeeModel, this.contactModel, this.userModel, isAccessable, listPhongBanId, true, fileBase64, rowData.candidateId).subscribe(response => {
          let result = <any>response;
          this.loading = false;
          if (result.statusCode === 202 || result.statusCode === 200) {
            // remove ứng viên khỏi danh sách
            this.listCandidate = this.listCandidate.filter(e => e != rowData);
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: 'Chuyển đổi NV thành công.' };
            this.showMessageN(mgs);
          }
          else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessageN(mgs);
            this.awaitResult = false;
          };
        });

        this.displayRejectEmail = true;
      }
    });
  }

  // UPDATE TRẠNG THÁI
  async updateCandidate(rowData: any, status: number) {
    let mes = '';
    switch (status) {
      case 3: //Đạt
        mes = "Xác nhận PV đạt cho ứng viên?";
        break;
      case 6: // Không đạt
        mes = "Xác nhận PV không đạt ứng viên?";
        break;
      case 5:// Từ chối OFFER
        mes = "Xác nhận Từ chối offer ứng viên?";
        break;
    }

    this.confirmationService.confirm({
      message: mes,
      accept: async () => {
        let lstCandidateId: Array<string> = []
        lstCandidateId.push(rowData.candidateId);
        let result: any = await this.employeeService.updateStatusCandidateFromVacancies(lstCandidateId, status, this.vacanciesId);
        if (result.statusCode == 200) {
          this.listCandidate = result.listCandidate;
          this.setColorOfStatusCandidate();
          this.showMessage('success', 'Cập nhập trạng thái thành công.');
        }
      }
    });
  }

  // Thêm nhanh ứng viên
  addCandidate() {
    this.displayAddCandidate = true;
    this.createCandidateForm.reset();
    this.vacanciesControl.setValue(this.viTriTuyenDung.vacanciesName);
  }

  // Đóng dialog thêm UV
  cancelAddCandidateDialog() {

    if (this.createCandidateForm.dirty) {
      this.confirmationService.confirm({
        message: 'Bạn có muốn hủy bỏ các thay đổi?',
        accept: () => {
          this.displayAddCandidate = false;
          this.statusIdDialog = this.emptyGuid;
          this.isUpdate = false;
        }
      });
    }
    else {
      this.displayAddCandidate = false;
      this.statusIdDialog = this.emptyGuid;
      this.isUpdate = false;
    }
  }

  // Thêm nhanh ứng viên
  async saveAddCandidateDialog() {

    if (!this.createCandidateForm.valid) {
      Object.keys(this.createCandidateForm.controls).forEach(key => {
        if (!this.createCandidateForm.controls[key].valid) {
          this.createCandidateForm.controls[key].markAsTouched();
        }
      });
    }
    else {
      let candidateModel: CandidateModel = this.mappingCandidateFormToModel();
      this.loading = true;
      if (this.isUpdate) {
        candidateModel.statusId = this.statusIdDialog;
        this.employeeService.updateCandidate(candidateModel, this.vacanciesId, null, []).subscribe(res => {
          let result = <any>res;
          if (result.statusCode == 200) {
            this.statusIdDialog = this.emptyGuid;
            this.isUpdate = false;
            this.displayAddCandidate = false;
            this.listCandidate = result.listCandidate;
            this.setColorOfStatusCandidate();
            this.showMessage('success', "Cập nhập ứng viên thành công");
          }
        });
      }
      else
        this.employeeService.createCandidate(candidateModel, this.vacanciesId, null, []).subscribe(res => {
          let result = <any>res;

          if (result.statusCode == 200) {
            this.loading = false;
            this.statusIdDialog = this.emptyGuid;
            this.displayAddCandidate = false;
            this.candidateViewDialogId = this.emptyGuid;
            this.listCandidate = result.listCandidate;
            this.setColorOfStatusCandidate();
            this.showMessage('success', "Tạo ứng viên thành công");
          }
          else {
            this.loading = false;
            this.showMessage('error', "Tạo ứng viên thất bại");
          }
        });
      this.loading = false;
    }
  }

  mappingCandidateFormToModel() {
    let candidateModel = new CandidateModel();
    candidateModel.candidateId = this.isUpdate == true ? this.candidateViewDialogId : this.emptyGuid;
    candidateModel.vacanciesId = this.vacanciesId;
    candidateModel.recruitmentCampaignId = this.recruitmentCampaignId;

    candidateModel.fullName = this.createCandidateForm.get('fullNameControl').value == null ? null : this.createCandidateForm.get('fullNameControl').value;

    candidateModel.dateOfBirth = this.createCandidateForm.get('dateOfBirthControl').value == "" || this.createCandidateForm.get('dateOfBirthControl').value == null ? null : convertToUTCTime(this.createCandidateForm.get('dateOfBirthControl').value);

    candidateModel.sex = this.createCandidateForm.get('genderControl').value == null ? null : this.createCandidateForm.get('genderControl').value;

    candidateModel.address = this.createCandidateForm.get('addressControl').value == null ? '' : this.createCandidateForm.get('addressControl').value;

    candidateModel.phone = this.createCandidateForm.get('phoneControl').value;

    candidateModel.email = this.emailControl.value ? this.emailControl.value : '';

    candidateModel.recruitmentChannel = this.createCandidateForm.get('chanelControl').value == null ? null : this.createCandidateForm.get('chanelControl').value.categoryId;

    candidateModel.applicationDate = this.createCandidateForm.get('applicationDateCandidateControl').value == "" || this.createCandidateForm.get('applicationDateCandidateControl').value == null ? null : convertToUTCTime(this.createCandidateForm.get('applicationDateCandidateControl').value);
    return candidateModel;
  }

  // delete ứng viên 1 hoặc nhiều
  async deleteCandidate(rowData: any) {

    let selectedCandidateId = [];
    if (rowData != null) {
      selectedCandidateId.push(rowData.candidateId);
    }
    else {
      this.selectedCandidate.forEach(item => {
        selectedCandidateId.push(item.candidateId);
      });
    }

    if (selectedCandidateId.length > 0) {
      this.confirmationService.confirm({
        message: 'Bạn có muốn xóa ứng viên?',
        accept: async () => {
          this.loading = true;
          let result: any = await this.employeeService.deleteCandidates(selectedCandidateId, this.vacanciesId);
          if (result.statusCode == 200) {
            this.noteHistory = result.listNote;
            this.listCandidate = result.listCandidate;
            this.setColorOfStatusCandidate();
            this.handleNoteContent();
            this.loading = false;
            this.showMessage('success', "Xóa thành công ứng viên.");
          }
          else {
            this.loading = false;
            this.showMessage('error', "Tồn tại ứng viên trạng thái khác Mới.");
          }
        }
      });
    }
    else
      this.showMessage('error', "Vui lòng chọn ứng viên để xóa.");
  }

  //#endregion

  //#region XUẤT EXCEL
  exportExcel() {
    let title = 'DANH SÁCH ỨNG VIÊN ' + this.viTriTuyenDung.vacanciesName.toUpperCase();
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('DS_UNGVIEN');

    let imgBase64 = this.getBase64Logo();

    /* Image */
    var imgLogo = workbook.addImage({
      base64: imgBase64,
      extension: 'png',
    });

    worksheet.addImage(imgLogo, {
      tl: { col: 0, row: 0 },
      ext: { width: 155, height: 100 }
    });


    let dataRow1 = [];
    dataRow1[2] = this.inforExportExcel.companyName.toUpperCase();  //Tên công ty
    let row1 = worksheet.addRow(dataRow1);
    row1.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`B${row1.number}:E${row1.number}`);
    row1.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow2 = [];
    dataRow2[2] = 'Địa chỉ: ' + this.inforExportExcel.address;  //Địa chỉ
    let row2 = worksheet.addRow(dataRow2);
    row2.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`B${row2.number}:E${row2.number}`);
    row2.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow3 = [];
    dataRow3[2] = 'Điện thoại: ' + this.inforExportExcel.phone;  //Số điện thoại
    let row3 = worksheet.addRow(dataRow3);
    row3.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`B${row3.number}:E${row3.number}`);
    row3.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow4 = [];
    dataRow4[2] = 'Email: ' + this.inforExportExcel.email;
    let row4 = worksheet.addRow(dataRow4);
    row4.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`B${row4.number}:E${row4.number}`);
    row4.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow5 = [];
    dataRow5[2] = 'Website dịch vụ: ' + this.inforExportExcel.website;  //Địa chỉ website
    let row5 = worksheet.addRow(dataRow5);
    row5.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`B${row5.number}:E${row5.number}`);
    row5.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    worksheet.addRow([]);
    worksheet.addRow([]);
    worksheet.addRow([]);

    /* title */
    let titleRow = worksheet.addRow([title]);
    titleRow.font = { family: 4, size: 16, bold: true };
    titleRow.height = 25;
    titleRow.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A${titleRow.number}:F${titleRow.number}`);
    /* subtitle */
    //let filterData = this.getDataFilterForm();

    /* header */
    const header = this.colsCandidate.map(e => e.header);
    let headerRow = worksheet.addRow(header);
    headerRow.font = { bold: true };
    header.forEach((item, index) => {
      headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });

    /* data table */
    let data = this.listCandidate.map((item, index) => {
      let row = [];

      row.push(index + 1);
      row.push(item.fullName);
      row.push(new Date(item.applicationDate).getDate() + '/' + new Date(item.applicationDate).getMonth() + '/' + new Date(item.applicationDate).getUTCFullYear());
      row.push(item.email);
      row.push(item.phone);
      row.push(item.recruitmentChannelName);
      row.push(item.statusName);

      return row;
    });

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      this.colsCandidate.forEach((item, index) => {
        row.getCell(index + 1).alignment = { vertical: 'bottom', horizontal: item.textAlign, wrapText: true };
        row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      });
    });

    /* set width */
    this.colsCandidate.forEach((item, index) => {
      worksheet.getColumn(index + 1).width = item.excelWidth;
    });
    worksheet.addRow([]);
    /* ký tên */
    let footer1 = [];
    footer1[4] = 'Ngày.....tháng.....năm..........';
    let rowFooter1 = worksheet.addRow(footer1);
    worksheet.mergeCells(`H${rowFooter1.number}:F${rowFooter1.number}`);
    rowFooter1.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    let footer2 = [];
    footer2[4] = 'NGƯỜI LẬP';
    let rowFooter2 = worksheet.addRow(footer2);
    worksheet.mergeCells(`H${rowFooter2.number}:F${rowFooter2.number}`);
    rowFooter2.font = { bold: true };
    rowFooter2.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    let footer3 = [];
    footer3[4] = '(Ký, họ tên)';
    let rowFooter3 = worksheet.addRow(footer3);
    worksheet.mergeCells(`H${rowFooter3.number}:F${rowFooter3.number}`);
    rowFooter3.font = { italic: true };
    rowFooter3.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    this.exportToExel(workbook, title);
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    });
  }

  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo ?.systemValueString;
  }

  //#endregion

  //#region   IMPORT EXCEL

  chooseFile(event: any) {
    this.fileName = event.target ?.files[0] ?.name;
    this.importFileExcel = event.target;
  }

  importCandidate() {
    this.displayChooseFileImportDialog = true;
  }

  // Tải file excel ứng viên lên Grid
  importExcel() {
    if (this.fileName == "") {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Chưa chọn file cần nhập" };
      this.showMessageN(mgs);
      return;
    }

    const targetFiles: DataTransfer = <DataTransfer>(this.importFileExcel);
    const reader: FileReader = new FileReader();
    reader.readAsBinaryString(targetFiles.files[0]);
    reader.onload = (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary', cellDates: true });
      let code = 'UngVien';
      if (!workbook.Sheets[code]) {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: "File không hợp lệ" };
        this.showMessageN(mgs);
        return;
      }

      //lấy data từ file excel
      const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[code];
      /* save data */
      let listCandidateRawData: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
      /* remove header */
      listCandidateRawData = listCandidateRawData.filter((e, index) => index != 0);
      /* nếu không nhập  trường required thì loại bỏ */
      listCandidateRawData = listCandidateRawData.filter(e => (e[0] && e[1]));
      /* chuyển từ raw data sang model */
      let listCandidateDataImport: Array<importCustomerByExcelModel> = [];

      listCandidateRawData ?.forEach(_rawData => {
        /*
        Mã công lao động
        Tên công lao động
        Loại công việc
        Model xe
        Tính chất
       */
        let candidate = new importCustomerByExcelModel();
        candidate.candidateName = _rawData[1] ? _rawData[1].toString().trim() : '';
        candidate.dateOfBirth = _rawData[2] ? _rawData[2].toString().trim() : null;
        candidate.phone = _rawData[3] ? _rawData[3].toString().trim() : '';
        candidate.sex = _rawData[4] ? _rawData[4].toString().trim() : 1;
        candidate.chanelCode = _rawData[5] ? _rawData[5].toString().trim() : '';
        candidate.email = _rawData[6] ? _rawData[6].toString().trim() : '';
        candidate.address = _rawData[7] ? _rawData[7].toString().trim() : '';
        candidate.applicationDate = _rawData[8] ? _rawData[8] : null;

        listCandidateDataImport = [...listCandidateDataImport, candidate];
      });
      /* tắt dialog import file, bật dialog chi tiết khách hàng import */
      this.displayChooseFileImportDialog = false;
      this.openDetailImportDialog(listCandidateDataImport);
    }
  }

  // Show thông tin danh sách ứng viên trong excel lên dialog
  openDetailImportDialog(listCandidateDataImport) {

    let ref = this.dialogService.open(CandidateImportDetailComponent, {
      data: {
        listCandidateDataImport: listCandidateDataImport,
        vacanciesId: this.vacanciesId,
        listCandidateAdd: this.listCandidate
      },
      header: 'Nhập excel danh sách Ứng viên cho vị trí ' + this.viTriTuyenDung.vacanciesName,
      width: '85%',
      baseZIndex: 1050,
      contentStyle: {
        "max-height": "800px",
        // "over-flow": "hidden"
      }
    });
    ref.onClose.subscribe((result: any) => {
      if (result ?.status) {
        this.listCandidate = result.listCandidateReturn;
        this.setColorOfStatusCandidate();
      }
    });

  }

  onClickImportBtn(event: any) {
    /* clear value của file input */
    event.target.value = ''
  }



  closeChooseFileImportDialog() {
    this.cancelFile();
  }

  cancelFile() {
    let fileInput = $("#importFileProduct")
    fileInput.replaceWith(fileInput.val('').clone(true));
    this.fileName = "";
  }

  async downloadTemplateExcel() {
    let result: any = await this.employeeService.downloadTemplateImportCandidate();

    if (result.templateExcel != null && result.statusCode === 200) {
      const binaryString = window.atob(result.templateExcel);
      const binaryLen = binaryString.length;
      const bytes = new Uint8Array(binaryLen);
      for (let idx = 0; idx < binaryLen; idx++) {
        const ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      const fileName = result.fileName + ".xls";
      link.download = fileName;
      link.click();
    } else {
      this.displayChooseFileImportDialog = false;
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Download file thất bại' };
      this.showMessageN(msg);
    }
  }


  //#endregion

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
      if ((window.navigator as any) && (window.navigator as any).msSaveOrOpenBlob) {
        (window.navigator as any).msSaveOrOpenBlob(file);
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

  downloadFile(fileInfor: FileInFolder) {

    this.imageService.downloadFile(fileInfor.fileName, fileInfor.fileUrl).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;
      var name = fileInfor.fileFullName;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if ((window.navigator as any) && (window.navigator as any).msSaveOrOpenBlob) {
        (window.navigator as any).msSaveOrOpenBlob(file);
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
    });
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }
  showMessageN(msg: any) {
    this.messageService.add(msg);
  }
  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  chonTuKhoUngVien() {

  }
}
function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

const cleanAccents = (str: string): string => {
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
  // Combining Diacritical Marks
  str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // huyền, sắc, hỏi, ngã, nặng
  str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // mũ â (ê), mũ ă, mũ ơ (ư)

  return str;
}
function ParseStringToFloat(str: string) {
  if (str === "" || str == null) return 0;
  str = str.toString().replace(/,/g, '');
  return parseFloat(str);
}
