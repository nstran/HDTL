import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { FileUpload } from 'primeng/fileupload';
import { NoteService } from '../../../shared/services/note.service';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { NoteModel } from '../../../shared/models/note.model';
import { EmployeeService } from '../../services/employee.service';
import { ContractService } from '../../../sales/services/contract.service';
import { ForderConfigurationService } from '../../../admin/components/folder-configuration/services/folder-configuration.service';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { DatePipe } from '@angular/common';
import { Table } from 'primeng/table';
import { Vacancies } from '../../models/recruitment-vacancies.model';
import { GetPermission } from '../../../shared/permission/get-permission';
import * as $ from 'jquery';

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

class EmployeeModel {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  employeeCodeName: string;
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

interface FileNameExists {
  oldFileName: string;
  newFileName: string
}

class RecruitmentCampaignEntityModel {
  recruitmentCampaignId: string;
  recruitmentCampaignName: string;
  startDate: Date;
  endDateDate: Date;
  statusName: string;
  personInChargeId: string;
  personInChargeName: string;
  personInChargeCode: string;
  personInChargeCodeName: string;
  recruitmentCampaignDes: string;
  recruitmentQuantity: number;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
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

class ColumnExcel {
  code: string;
  name: string;
  width: number;
}
class CategoryModel {
  categoryId: string;
  categoryName: string;
  constructor() { }
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
@Component({
  selector: 'app-chien-dich-detail',
  templateUrl: './chien-dich-detail.component.html',
  styleUrls: ['./chien-dich-detail.component.css'],
  providers: [DatePipe]
})
export class ChienDichDetailComponent implements OnInit {

  @ViewChild('fileNoteUpload') fileNoteUpload: FileUpload;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  @ViewChild('myTable') myTableDocument: Table;
  @ViewChild('myTable') myTableVacancies: Table;
  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới
  today: Date = new Date();
  statusCode: string = null;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  currentEmployeeCodeName = localStorage.getItem('EmployeeCodeName');
  auth = JSON.parse(localStorage.getItem('auth'));
  chienDichId: string = '';
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  recruitmentCampaignModel: RecruitmentCampaignEntityModel;
  displayAddCandidate: boolean = false;
  genderModel: string = "1";
  selectedVacanciesId: string = '00000000-0000-0000-0000-000000000000';
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
  file: File[];
  listFile: Array<FileInFolder> = [];
  isFromTo: boolean = false;
  displayAddVacancies: boolean = false;
  // FORM GROUP
  thongTinChienDichFormGroup: FormGroup;
  tenChienDichFormControl: FormControl;
  ngayBatDauFormControl: FormControl;
  ngayKetThucFormControl: FormControl;
  phuTrachViTriTuyenDungFormControl: FormControl;
  phuTrachChienDichFormControl: FormControl;
  NoiDungChienDichFormControl: FormControl;
  trangThaiFormControl: FormControl;
  KeHoachTuyenDungFormControl: FormControl;

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

  // Dialog Thêm vị trí
  createVacanciesForm: FormGroup;
  tenViTriTuyenDungFormControl: FormControl;
  soLuongFormControl: FormControl;
  mucUuTienFormControl: FormControl;
  loaiTienTeFormControl: FormControl;
  nguoiPhuTrachFormControl: FormControl;
  loaiCongViecFormControl: FormControl;
  kieuLuongFormControl: FormControl;
  noiLamViecFormControl: FormControl;
  kinhNghiemFormControl: FormControl;
  tuFormControl: FormControl;
  denFormControl: FormControl;

  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionDownLoad: boolean = true;
  actionUpLoad: boolean = true;


  isManagerOfHR: boolean = false;
  isGD: boolean = false;
  isNguoiPhuTrach: boolean = false;

  actionAddViTriTuyenDung: boolean = true;
  actionAddUngVien: boolean = true;
  //Danh sach
  listPhuTrachViTriTuyenDung: Array<EmployeeModel> = [];
  listViTriUongTuyen: Array<any> = [];
  selectedCustomers: Array<any> = [];
  uploadedFiles: any[] = [];
  colsViTriUongTuyen: any[];
  colsFile: any[];
  folderType = 'RE_CAMP';
  listChanel: Array<CategoryModel> = []
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
  constructor(
    private router: Router,
    private datePipe: DatePipe,
    private route: ActivatedRoute,
    private noteService: NoteService,
    public dialogService: DialogService,
    private messageService: MessageService,
    private contractService: ContractService,
    private employeeService: EmployeeService,
    private imageService: ImageUploadService,
    private folderService: ForderConfigurationService,
    private confirmationService: ConfirmationService,
    private def: ChangeDetectorRef,
    private getPermission: GetPermission,
  ) { }

  async ngOnInit() {

    //Cho chi tiet chien dich
    let resource = "rec/employee/chi-tiet-chien-dich/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
      this.showMessage('warn', 'Bạn không có quyền truy cập');
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;

      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }

      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }

      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      //tải tài liệu
      if (listCurrentActionResource.indexOf("download") == -1) {
        this.actionDownLoad = false;
      }
      //upload tài liệu
      if (listCurrentActionResource.indexOf("upload") == -1) {
        this.actionUpLoad = false;
      }
    }

    //Cho vi tri tuyen dung
    let resource1 = "rec/employee/tao-cong-viec-tuyen-dung/";
    let permission1: any = await this.getPermission.getPermission(resource1);
    if (permission1.status == false) {
      this.actionAddViTriTuyenDung = false;
    }
    else {
      let listCurrentActionResource = permission1.listCurrentActionResource;

      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAddViTriTuyenDung = false;
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
      this.chienDichId = params['recruitmentCampaignId'];
      this.setForm();
      this.setTable();
      this.getMasterdata();

    });
  }

  setForm() {
    this.tenChienDichFormControl = new FormControl(null, [Validators.required]);
    this.ngayBatDauFormControl = new FormControl(null, [Validators.required]);
    this.ngayKetThucFormControl = new FormControl(null, [Validators.required]);
    this.phuTrachViTriTuyenDungFormControl = new FormControl(null);
    this.phuTrachChienDichFormControl = new FormControl(null);
    this.NoiDungChienDichFormControl = new FormControl(null);
    this.trangThaiFormControl = new FormControl(null);
    this.KeHoachTuyenDungFormControl = new FormControl('0');

    this.thongTinChienDichFormGroup = new FormGroup({
      tenChienDichFormControl: this.tenChienDichFormControl,
      ngayBatDauFormControl: this.ngayBatDauFormControl,
      ngayKetThucFormControl: this.ngayKetThucFormControl,
      phuTrachViTriTuyenDungFormControl: this.phuTrachViTriTuyenDungFormControl,
      phuTrachChienDichFormControl: this.phuTrachChienDichFormControl,
      NoiDungChienDichFormControl: this.NoiDungChienDichFormControl,
      trangThaiFormControl: this.trangThaiFormControl,
      KeHoachTuyenDungFormControl: this.KeHoachTuyenDungFormControl,
    });
    this.vacanciesControl = new FormControl('');
    this.fullNameControl = new FormControl('', Validators.required);
    this.phoneControl = new FormControl('', [Validators.required, Validators.pattern(this.getPhonePattern())]);
    this.dateOfBirthControl = new FormControl(null, Validators.required);
    this.genderControl = new FormControl("1");
    this.addressControl = new FormControl('');
    this.emailControl = new FormControl(null);
    this.chanelControl = new FormControl(null);
    this.applicationDateCandidateControl = new FormControl(null);

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
    this.phuTrachChienDichFormControl.setValue(this.currentEmployeeCodeName);
    this.phuTrachChienDichFormControl.disable();
    this.trangThaiFormControl.disable();
    this.KeHoachTuyenDungFormControl.disable();

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

    this.createVacanciesForm = new FormGroup({
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
    });
  }
  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }
  setTable() {
    this.colsFile = [
      { field: 'fileName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'right', type: 'number' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'right', type: 'date' },
      { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'left', type: 'string' },
    ];

    this.colsViTriUongTuyen = [
      { field: 'index', header: '#', width: '5%', textAlign: 'center', type: 'string' },
      { field: 'vacanciesName', header: 'Tên', width: '20%', textAlign: 'left', type: 'string' },
      { field: 'priorityName', header: 'Ưu tiên', width: '10%', textAlign: 'left', type: 'number' },
      { field: 'quantity', header: 'Số lượng', width: '5%', textAlign: 'right', type: 'date' },
      { field: 'salaryLable', header: 'Lương', width: '13%', textAlign: 'left', type: 'string' },
      { field: 'experienceName', header: 'Kinh nghiệm', width: '17%', textAlign: 'left', type: 'string' },
      { field: 'typeOfWorkName', header: 'Loại công việc', width: '15%', textAlign: 'left', type: 'string' },
      { field: 'personInChargeName', header: 'Người phụ trách', width: '15%', textAlign: 'left', type: 'string' },
    ];
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.employeeService.getMasterRecruitmentCampaignDetailAsync(this.chienDichId);
    this.loading = false;
    this.recruitmentCampaignModel = this.mappingRecruitmentCampaignModel(result.recruitmentCampaign);
    this.mappingModelToFrom();
    this.noteHistory = result.listNote;
    this.listChanel = result.listChanel;
    this.handleNoteContent();
    this.listViTriUongTuyen = result.listVacancies;

    this.listPhuTrachViTriTuyenDung = result.listEmployeePTTD;
    this.listKinhNghiem = result.listKinhNghiem;
    this.listLoaiCongViec = result.listLoaiCongViec;

    this.isManagerOfHR = result.isManagerOfHR;
    this.isGD = result.isGD;
    this.isNguoiPhuTrach = result.isNguoiPhuTrach;

    this.listViTriUongTuyen.forEach(item => {
      switch (item.priority) {
        case 1:
          item.priorityName = 'Cao';
          break;
        case 2:
          item.priorityName = 'Trung Bình';
          break;
        case 3:
          item.priorityName = 'Thấp';
          break;
      }

      if (item.salarType == 1)
        item.salaryLable = item.salaryFrom.toLocaleString('vi-VN') + ' - ' + item.salaryTo.toLocaleString('vi-VN');
      if (item.salarType == 2)
        item.salaryLable = "Thỏa thuận"
    });
    this.listFile = result.listFileInFolder;

  }

  mappingModelToFrom() {

    // ten
    this.tenChienDichFormControl.setValue(this.recruitmentCampaignModel.recruitmentCampaignName);

    // ngay bat dau
    let startDate = this.recruitmentCampaignModel.startDate ? new Date(this.recruitmentCampaignModel.startDate) : null;
    this.ngayBatDauFormControl.setValue(startDate);

    //tran thai
    this.trangThaiFormControl.setValue(this.recruitmentCampaignModel.statusName);

    //nguoi phu trach
    this.phuTrachChienDichFormControl.setValue(this.recruitmentCampaignModel.personInChargeCodeName);

    //ngay ket thuc
    let endDate = this.recruitmentCampaignModel.endDateDate ? new Date(this.recruitmentCampaignModel.endDateDate) : null;
    this.ngayKetThucFormControl.setValue(endDate);

    // ke hoach
    this.KeHoachTuyenDungFormControl.setValue(this.recruitmentCampaignModel.recruitmentQuantity);

    // noi dung
    this.NoiDungChienDichFormControl.setValue(this.recruitmentCampaignModel.recruitmentCampaignDes);

  }

  mappingRecruitmentCampaignModel(model: any): RecruitmentCampaignEntityModel {
    let recruitmentCampaign = new RecruitmentCampaignEntityModel();
    recruitmentCampaign.recruitmentCampaignId = model.recruitmentCampaignId;
    recruitmentCampaign.recruitmentCampaignName = model.recruitmentCampaignName;
    recruitmentCampaign.startDate = model.startDate;
    recruitmentCampaign.endDateDate = model.endDateDate;
    recruitmentCampaign.statusName = model.statusName;
    recruitmentCampaign.personInChargeId = model.personInChargeId;
    recruitmentCampaign.personInChargeName = model.personInChargeName;
    recruitmentCampaign.personInChargeCode = model.personInChargeCode;
    recruitmentCampaign.personInChargeCodeName = model.personInChargeCodeName;
    recruitmentCampaign.recruitmentCampaignDes = model.recruitmentCampaignDes;
    recruitmentCampaign.createdDate = model.createdDate;
    recruitmentCampaign.createdById = model.createdById;
    recruitmentCampaign.recruitmentQuantity = model.recruitmentQuantity;

    return recruitmentCampaign;
  }

  goBackToList() {
    this.router.navigate(['/employee/danh-sach-chien-dich']);
  }

  resetEndDate() {
    this.ngayKetThucFormControl.reset();
  }

  goDetailVacanciesName(rowData) {
    let url = this.router.serializeUrl(this.router.createUrlTree(['/employee/chi-tiet-cong-viec-tuyen-dung', { vacanciesId: rowData.vacanciesId, recruitmentCampaignId: this.recruitmentCampaignModel.recruitmentCampaignId }]));
    window.open(url, '_blank');
  }
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
  changeSalary(event: any) {

    if (event.value.value == "1")
      this.isFromTo = true;
    else
      this.isFromTo = false;
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
  // deleteFile(file: any) {
  //   this.confirmationService.confirm({
  //     message: 'Bạn chắc chắn muốn xóa?',
  //     accept: () => {
  //       let index = this.listFile.indexOf(file);
  //       this.listFile.splice(index, 1);
  //     }
  //   });
  // }

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: FileInFolder) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.contractService.deleteFile(file.fileInFolderId).subscribe(res => {
          let result: any = res;

          if (result.statusCode == 200) {
            this.listFile = this.listFile.filter(x => x.fileInFolderId != file.fileInFolderId);

            this.showMessage('success', 'Xóa file thành công');
          } else {
            this.showMessage('error', result.messageCode);
          }
        })
        // let index = this.listFile.indexOf(file);
        // this.listFile.splice(index, 1);
      }
    });
  }

  downloadFile(data: FileInFolder) {
    this.folderService.downloadFile(data.fileInFolderId).subscribe(response => {
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
        if ((window.navigator as any) && (window.navigator as any).msSaveOrOpenBlob) {
          (window.navigator as any).msSaveOrOpenBlob(file);
        } else {
          var fileURL = URL.createObjectURL(file);
          if (fileType.indexOf('image') !== -1) {
            window.open(fileURL);
          } else {
            var anchor = document.createElement("a");
            anchor.download = data.fileName.substring(0, data.fileName.lastIndexOf('_')) + "." + data.fileExtension;
            anchor.href = fileURL;
            anchor.click();
          }
        }
      }
      else {
        this.showMessage('error', result.messageCode);
      }
    });
  }

  /*Event upload list file*/
  myUploader(event: any) {
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
      fileUpload.FileInFolder.objectId = this.recruitmentCampaignModel.recruitmentCampaignId;
      fileUpload.FileInFolder.objectType = this.folderType;
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    this.employeeService.uploadFile(this.folderType, listFileUploadModel, this.recruitmentCampaignModel.recruitmentCampaignId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedFiles = [];
        this.fileUpload.clear();
        this.def.detectChanges();
        this.listFile = result.listFileInFolder;
        this.showMessage('success', "Thêm file thành công");
      } else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage('error', result.message);
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
      fileUpload.FileInFolder.objectType = this.folderType;
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    let noteModel = new NoteModel();
    if (!this.noteId) {
      /*Tạo mới ghi chú*/
      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.chienDichId;
      noteModel.ObjectType = this.folderType;
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/
      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.chienDichId;
      noteModel.ObjectType = this.folderType;
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
      let result: any = this.imageService.deleteFileForOptionAsync(this.folderType, item.documentName);

      // xóa file trong DB
      this.noteService.deleteNoteDocument(item.noteId, item.noteDocumentId).subscribe(response => {
        let result: any = response;
        // this.loading = false;
        if (result.statusCode != 200) {
          this.showMessage('error', 'Xóa file thất bại');
        }
      });
    })

    this.noteHistory = [];
    this.noteService.createNoteForAllRecruitmentCampaign(noteModel, this.folderType, listFileUploadModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedNoteFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.noteContent = null;
        this.noteId = null;
        this.isEditNote = false;

        /*Reshow Time Line*/
        this.noteHistory = result.listNote;
        this.handleNoteContent();
        // this.createOrUpdate(false);
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

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

  exportExcel() {
    if (this.selectedCustomers.length > 0) {
      let title = `Danh sách vị trí tuyển dụng`;

      let workBook = new Workbook();
      let worksheet = workBook.addWorksheet(title);

      let dataHeaderMain = "Danh sách vị trí tuyển dụng".toUpperCase();
      let headerMain = worksheet.addRow([dataHeaderMain]);
      headerMain.font = { size: 18, bold: true };
      worksheet.mergeCells(`A${1}:G${1}`);
      headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      worksheet.addRow([]);

      /* Header row */
      let buildColumnExcel = this.buildColumnExcel();
      let dataHeaderRow = buildColumnExcel.map(x => x.name);
      let headerRow = worksheet.addRow(dataHeaderRow);
      headerRow.font = { name: 'Times New Roman', size: 12, bold: true };
      dataHeaderRow.forEach((item, index) => {
        headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      });
      headerRow.height = 22.50;

      /* Data table */
      let data: Array<any> = [];
      this.selectedCustomers.forEach((item, index) => {
        let row: Array<any> = [];
        buildColumnExcel.forEach((_item, _index) => {

          if (_item.code == 'index') {
            row[_index] = index + 1;
          }
          else if (_item.code == 'vacanciesName') {
            row[_index] = item.vacanciesName;
          }
          else if (_item.code == 'priorityName') {
            row[_index] = item.priorityName;
          }
          else if (_item.code == 'quantity') {
            row[_index] = item.quantity;
          }
          else if (_item.code == 'salaryLable') {
            row[_index] = item.salaryLable;
          }
          else if (_item.code == 'experienceName') {
            row[_index] = item.experienceName;
          }
          else if (_item.code == 'typeOfWorkName') {
            row[_index] = item.typeOfWorkName;
          }
          else if (_item.code == 'placeOfWork') {
            row[_index] = item.placeOfWork;
          }
          else if (_item.code == 'personInChargeName') {
            row[_index] = item.personInChargeName;
          }
        });

        data.push(row);
      });

      data.forEach((el, index, array) => {
        let row = worksheet.addRow(el);
        row.font = { name: 'Times New Roman', size: 11 };

        buildColumnExcel.forEach((_item, _index) => {
          row.getCell(_index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          if (_item.code == 'recruitmentCampaignName' || _item.code == 'personInChargeName') {
            row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'left' };
          }
          else if (_item.code == 'startDate' || _item.code == 'endDateDate') {
            row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'right' };
          }
          else {
            row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'center' };
          }
        });
      });

      /* fix with for column */
      buildColumnExcel.forEach((item, index) => {
        worksheet.getColumn(index + 1).width = item.width;
      });

      this.exportToExel(workBook, title);
    }
    else {
      this.showMessage('warn', 'Bạn chưa chọn khách hàng');
    }
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  buildColumnExcel(): Array<ColumnExcel> {
    let listColumn: Array<ColumnExcel> = [];
    this.colsViTriUongTuyen.forEach(item => {
      let column = new ColumnExcel();
      column.code = item.field;
      column.name = item.header;

      if (item.field == 'index') {
        column.width = 7.14;
      }
      else if (item.field == 'vacanciesName') {
        column.width = 30.71;
      }
      else if (item.field == 'priorityName') {
        column.width = 30.71;
      }
      else if (item.field == 'quantity') {
        column.width = 30.71;
      }
      else if (item.field == 'salaryLable') {
        column.width = 35.71;
      }
      else if (item.field == 'experienceName') {
        column.width = 20.71;
      }
      else if (item.field == 'typeOfWorkName') {
        column.width = 25.71;
      }
      else if (item.field == 'placeOfWork') {
        column.width = 25.71;
      }
      else if (item.field == 'personInChargeName') {
        column.width = 25.71;
      }

      listColumn.push(column);
    });

    return listColumn;
  }

  importExcel() {

  }
  //#region   Thêm nhanh ứng viên
  addVancanciesDialog() {

    this.displayAddVacancies = true;
    this.createVacanciesForm.reset();
  }

  cancelAddVacanciesDialog() {
    if (this.createVacanciesForm.dirty) {
      this.confirmationService.confirm({
        message: 'Bạn có muốn hủy bỏ các thay đổi?',
        accept: () => {
          this.displayAddVacancies = false;
        }
      });
    }
    else {
      this.displayAddVacancies = false;
    }
  }

  // Thêm nhanh ứng viên
  async saveAddVacanciesDialog() {
    if (!this.createVacanciesForm.valid) {
      Object.keys(this.createVacanciesForm.controls).forEach(key => {
        if (!this.createVacanciesForm.controls[key].valid) {
          this.createVacanciesForm.controls[key].markAsTouched();
        }
      });
    }
    else {
      let viTriTyenDung: Vacancies = this.mapDataVacanciesToModel();
      this.loading = true;

      this.employeeService.createVacancies(viTriTyenDung, "VACANCIES", []).subscribe(res => {
        let result = <any>res;
        if (result.statusCode == 200) {
          this.loading = false;
          let totalUV: number = 0;
          this.listViTriUongTuyen = result.listVacancies;
          this.listViTriUongTuyen.forEach(item => {
            switch (item.priority) {
              case 1:
                item.priorityName = 'Cao';
                break;
              case 2:
                item.priorityName = 'Trung Bình';
                break;
              case 3:
                item.priorityName = 'Thấp';
                break;
            }

            if (item.salarType == 1)
              item.salary = item.salaryFrom.toLocaleString('vi-VN') + ' - ' + item.salaryTo.toLocaleString('vi-VN');
            if (item.salarType == 2)
              item.salary = "Thỏa thuận"

            totalUV += item.quantity;
          });

          this.KeHoachTuyenDungFormControl.setValue(totalUV);
          this.displayAddVacancies = false;
          this.showMessage('success', "Tạo vị trí tuyển dụng thành công");
          // tính lại SL tuyển

          this.def.detectChanges();
        }
        else {
          this.loading = false;
          this.showMessage('error', "Tạo vị trí tuyển dụng thất bại");
        }
      });
      this.loading = false;
    }
  }


  mapDataVacanciesToModel(): Vacancies {
    let viTriTD = new Vacancies();

    viTriTD.vacanciesId = this.emptyGuid;
    // Mã chiến dịch
    viTriTD.recruitmentCampaignId = this.recruitmentCampaignModel.recruitmentCampaignId;

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

    return viTriTD;
  }

  //#endregion


  //#region   Thêm nhanh ứng viên
  themUngVienDialog(rowData) {

    this.displayAddCandidate = true;
    this.createCandidateForm.reset();
    this.vacanciesControl.setValue(rowData.vacanciesName);
    this.selectedVacanciesId = rowData.vacanciesId;
  }
  // Đóng dialog thêm UV
  cancelAddCandidateDialog() {
    if (this.createCandidateForm.dirty) {
      this.confirmationService.confirm({
        message: 'Bạn có muốn hủy bỏ các thay đổi?',
        accept: () => {
          this.displayAddCandidate = false;
        }
      });
    }
    else {
      this.displayAddCandidate = false;
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
      this.employeeService.createCandidate(candidateModel, this.selectedVacanciesId, null, []).subscribe(res => {
        let result = <any>res;
        if (result.statusCode == 200) {
          this.loading = false;

          this.displayAddCandidate = false;
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
    candidateModel.candidateId = this.emptyGuid;
    candidateModel.vacanciesId = this.selectedVacanciesId;
    candidateModel.recruitmentCampaignId = this.recruitmentCampaignModel.recruitmentCampaignId;

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
  //#endregion

  deleteRow(rowData) {

    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn xóa vị trí tuyển dụng này không?',
      accept: async () => {
        rowData.active = false;

        let result: any = await this.employeeService.deleteVacanciesById(rowData.vacanciesId);

        if (result.statusCode === 200) {

          this.listViTriUongTuyen = this.listViTriUongTuyen.filter(e => e != rowData);
          this.showMessage('success', "Bạn đã xóa vị trí tuyển dụng thành công")
          this.def.detectChanges();
        } else {
          this.showMessage('error', result.message)
        }
      }
    });
  }

  update() {
    this.loading = true;
    this.awaitResult = true;
    if (!this.thongTinChienDichFormGroup.valid) {
      Object.keys(this.thongTinChienDichFormGroup.controls).forEach(key => {
        if (this.thongTinChienDichFormGroup.controls[key].valid == false) {
          this.thongTinChienDichFormGroup.controls[key].markAsTouched();
        }
      });
      this.loading = false;
      this.awaitResult = false;
    } else {

      //ten chien dich
      this.recruitmentCampaignModel.recruitmentCampaignName = this.tenChienDichFormControl.value ? this.tenChienDichFormControl.value : '';

      //ngay bat dau
      let startDate = this.ngayBatDauFormControl.value ? convertToUTCTime(this.ngayBatDauFormControl.value) : null;
      this.recruitmentCampaignModel.startDate = startDate;

      //ngay ket thuc
      let endDate = this.ngayKetThucFormControl.value ? convertToUTCTime(this.ngayKetThucFormControl.value) : null;
      this.recruitmentCampaignModel.endDateDate = endDate;

      // noi dung
      this.recruitmentCampaignModel.recruitmentCampaignDes = this.NoiDungChienDichFormControl.value == null ? '' : this.NoiDungChienDichFormControl.value;

      let listFileUploadModel: Array<FileUploadModel> = [];
      this.uploadedFiles.forEach(item => {
        let fileUpload: FileUploadModel = new FileUploadModel();
        fileUpload.FileInFolder = new FileInFolder();
        fileUpload.FileInFolder.active = true;
        let index = item.name.lastIndexOf(".");
        let name = item.name.substring(0, index);
        fileUpload.FileInFolder.fileName = name;
        fileUpload.FileInFolder.fileExtension = item.name.substring(index + 1);
        fileUpload.FileInFolder.size = item.size;
        fileUpload.FileInFolder.objectId = '';
        fileUpload.FileInFolder.objectType = this.folderType;
        fileUpload.FileSave = item;
        listFileUploadModel.push(fileUpload);
      });
      this.employeeService.createRecruitmentCampaign(this.recruitmentCampaignModel, this.folderType, listFileUploadModel).subscribe(response => {
        let result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.showMessage('success', 'Cập nhật chiến dịch thành công');
          this.resetFieldValue();
          setTimeout(async () => {
            await this.getMasterdata();
            this.loading = false;
          }, 500)
          this.awaitResult = false;
        }
        else {
          this.loading = false;
          this.showMessage('error', result.messageCode);
          this.awaitResult = false;
        };
      });
    }
  }

  deleteChienDich() {
    this.confirmationService.confirm({
      message: 'Bạn có muốn xác nhận xóa chiến dịch?',
      accept: () => {
        this.loading = true;
        this.employeeService.deleteRecruitmentCampaign(this.recruitmentCampaignModel.recruitmentCampaignId).subscribe(response => {
          let result = <any>response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.showMessage('success', 'Xóa chiến dịch thành công.');
            setTimeout(() => {
              this.router.navigate(['/employee/danh-sach-chien-dich']);
            }, 1000);
          } else {
            this.showMessage('error', 'Chiến dịch đã tồn tại vị trí tuyển dụng. Không thể xóa!');
          }
        });
      }
    });
  }

  //Function reset toàn bộ các value đã nhập trên form
  resetFieldValue() {
    this.uploadedFiles = [];
    this.thongTinChienDichFormGroup.reset();
  }
  //Kết thúc

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function ParseStringToFloat(str: string) {
  if (str === "" || str == null) return 0;
  str = str.toString().replace(/,/g, '');
  return parseFloat(str);
}
