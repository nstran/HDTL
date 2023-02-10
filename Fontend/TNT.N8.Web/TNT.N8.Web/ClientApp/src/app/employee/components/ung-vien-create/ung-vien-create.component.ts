import { Component, OnInit, ChangeDetectorRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { GetPermission } from '../../../shared/permission/get-permission';
import { EmployeeService } from '../../services/employee.service';

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

class CandidateEntityModel {
  candidateId: string;
  fullName: string;
  dateOfBirth: Date;
  phone: string;
  address: string;
  avatar: string;
  email: string;
  recruitmentChannel: number;
  sex: number;
  statusId: string;
  applicationDate: Date;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
}

@Component({
  selector: 'app-ung-vien-create',
  templateUrl: './ung-vien-create.component.html',
  styleUrls: ['./ung-vien-create.component.css']
})
export class UngVienCreateComponent implements OnInit {

  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới
  today: Date = new Date();
  statusCode: string = null;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  @ViewChild('fileUpload') fileUpload: FileUpload;
  selectedChienDich: any = null;
  selectedViTri: any = null;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  //Danh sach
  listChienDich: Array<any> = [];
  listViTriTuyenDung: Array<any> = [];
  listAllViTri: Array<any> = [];
  listKenhTuyenDung: Array<any> = [];
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];
  colsFile: any[];

  // FORM GROUP
  thongTinUngVIenFormGroup: FormGroup;
  hoTenFormControl: FormControl;
  ngaySinhFormControl: FormControl;
  soDienThoaiFormControl: FormControl;
  chienDichUngTuyenFormControl: FormControl;
  viTriUngTuyenFormControl: FormControl;
  gioiTinhFormControl: FormControl;
  diaChiFormControl: FormControl;
  emailFormControl: FormControl;
  kenhTuyenDungFormControl: FormControl;
  ngayUngTuyenFormControl: FormControl;

  constructor(
    private router: Router,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    private confirmationService: ConfirmationService,
    private def: ChangeDetectorRef,
    private getPermission: GetPermission,

  ) { }

  async ngOnInit() {
    this.setForm();
    this.setTable();
    let resource = "rec/employee/tao-ung-vien/";
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

      this.getMasterData();
    }
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  setForm() {
    this.hoTenFormControl = new FormControl(null, [Validators.required]);
    this.ngaySinhFormControl = new FormControl(null, [Validators.required]);
    this.soDienThoaiFormControl = new FormControl('', [Validators.required, Validators.pattern(this.getPhonePattern())]);
    this.viTriUngTuyenFormControl = new FormControl(null, [Validators.required]);
    this.chienDichUngTuyenFormControl = new FormControl(null, [Validators.required]);
    this.gioiTinhFormControl = new FormControl(1, [Validators.required]);
    this.diaChiFormControl = new FormControl(null);
    this.emailFormControl = new FormControl(null);
    this.kenhTuyenDungFormControl = new FormControl(null);
    this.ngayUngTuyenFormControl = new FormControl(null);

    this.thongTinUngVIenFormGroup = new FormGroup({
      hoTenFormControl: this.hoTenFormControl,
      ngaySinhFormControl: this.ngaySinhFormControl,
      soDienThoaiFormControl: this.soDienThoaiFormControl,
      chienDichUngTuyenFormControl: this.chienDichUngTuyenFormControl,
      viTriUngTuyenFormControl: this.viTriUngTuyenFormControl,
      gioiTinhFormControl: this.gioiTinhFormControl,
      diaChiFormControl: this.diaChiFormControl,
      emailFormControl: this.emailFormControl,
      kenhTuyenDungFormControl: this.kenhTuyenDungFormControl,
      ngayUngTuyenFormControl: this.ngayUngTuyenFormControl,
    });
  }

  setTable() {
    this.colsFile = [
      { field: 'fileName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'right', type: 'number' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'right', type: 'date' },
      { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'left', type: 'string' },
    ];
  }

  getMasterData() {
    this.loading = true;
    this.employeeService.getMasterCreateCandidate().subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {

        this.listChienDich = result.listRecruitmentCampaign;
        this.listAllViTri = result.listVacancies;
        this.listViTriTuyenDung = this.listAllViTri;
        this.listKenhTuyenDung = result.listRecruitmentChannel;

      } else {
        this.showMessage('error', result.messageCode);
      }
    });
  }

  goBackToList() {
    this.router.navigate(['/employee/danh-sach-ung-vien']);
  }

  create(isSaveNew) {
    if (!this.thongTinUngVIenFormGroup.valid) {
      Object.keys(this.thongTinUngVIenFormGroup.controls).forEach(key => {
        if (this.thongTinUngVIenFormGroup.controls[key].valid == false) {
          this.thongTinUngVIenFormGroup.controls[key].markAsTouched();
        }
      });
    } else {
      let candidateModel = new CandidateEntityModel();
      candidateModel.fullName = this.hoTenFormControl.value ? this.hoTenFormControl.value : '';

      let dob = this.ngaySinhFormControl.value ? convertToUTCTime(this.ngaySinhFormControl.value) : null;
      candidateModel.dateOfBirth = dob;

      candidateModel.sex = this.gioiTinhFormControl.value ? this.gioiTinhFormControl.value : 1;

      candidateModel.phone = this.soDienThoaiFormControl.value ? this.soDienThoaiFormControl.value : '';

      candidateModel.email = this.emailFormControl.value ? this.emailFormControl.value : '';

      let ngayUngTuye = this.ngayUngTuyenFormControl.value ? convertToUTCTime(this.ngayUngTuyenFormControl.value) : null;
      candidateModel.applicationDate = ngayUngTuye;

      candidateModel.address = this.diaChiFormControl.value ? this.diaChiFormControl.value : '';

      let recruitmentChannel = this.kenhTuyenDungFormControl.value ? this.kenhTuyenDungFormControl.value.categoryId : this.emptyGuid;
      candidateModel.recruitmentChannel = recruitmentChannel;

      let vacancies = this.viTriUngTuyenFormControl.value.vacanciesId;

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
        fileUpload.FileInFolder.objectType = 'CANDIDATE';
        fileUpload.FileSave = item;
        listFileUploadModel.push(fileUpload);
      });


      this.loading = true;
      this.awaitResult = true;
      this.employeeService.createCandidate(candidateModel, vacancies, "CANDIDATE", listFileUploadModel).subscribe(response => {
        let result = <any>response;
        this.loading = false;
        if (result.statusCode === 202 || result.statusCode === 200) {

          this.showMessage('success', 'Tạo ứng viên thành công');
          //Lưu và thêm mới
          if (isSaveNew) {
            this.awaitResult = false;
          }
          //Lưu
          else {
            setTimeout(() => {
              this.router.navigate(['/employee/chi-tiet-ung-vien', { candidateId: result.candidateId }]);
            }, 1000)
            this.awaitResult = false;
          }
          this.resetFieldValue();
        }
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage('error', result.messageCode);
          this.awaitResult = false;
        };
      });
    }
  }

  onChangeChienDich(chienDichId: string) {
    this.listViTriTuyenDung = this.listAllViTri.filter(x => x.recruitmentCampaignId == chienDichId);
    // this.router.navigate(['/project/create-project-task', { 'projectId': projectId }]);
  }

  onChangeViTri(viTriId: string) {
    // this.router.navigate(['/project/create-project-task', { 'projectId': projectId }]);
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
        let index = this.arrayDocumentModel.indexOf(file);
        this.arrayDocumentModel.splice(index, 1);
      }
    });
  }

  //Function reset toàn bộ các value đã nhập trên form
  resetFieldValue() {
    this.fileUpload.clear();
    this.thongTinUngVIenFormGroup.reset();
    this.gioiTinhFormControl.setValue(1);
    this.def.detectChanges();
  }
  //Kết thúc

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};