import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { EmployeeService } from '../../services/employee.service';
import { GetPermission } from '../../../shared/permission/get-permission';

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

class RecruitmentCampaignEntityModel {
  recruitmentCampaignId: string;
  recruitmentCampaignName: string;
  startDate: Date;
  endDateDate: Date;
  personInChargeId: string;
  recruitmentCampaignDes: string;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
}

@Component({
  selector: 'app-chien-dich-create',
  templateUrl: './chien-dich-create.component.html',
  styleUrls: ['./chien-dich-create.component.css']
})
export class ChienDichCreateComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới
  today: Date = new Date();
  statusCode: string = null;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  currentEmployeeCodeName = localStorage.getItem('EmployeeCodeName');
  auth = JSON.parse(localStorage.getItem('auth'));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  // FORM GROUP
  thongTinChienDichFormGroup: FormGroup;
  tenChienDichFormControl: FormControl;
  ngayBatDauFormControl: FormControl;
  ngayKetThucFormControl: FormControl;
  phuTrachViTriTuyenDungFormControl: FormControl;
  phuTrachChienDichFormControl: FormControl;
  NoiDungChienDichFormControl: FormControl;

  //Danh sach
  listPhuTrachViTriTuyenDung: Array<EmployeeModel> = [];
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];
  colsFile: any[];

  constructor(
    private router: Router,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    private confirmationService: ConfirmationService,
    private getPermission: GetPermission,
  ) { }

  async ngOnInit() {
    this.setForm();
    this.setTable();
    let resource = "rec/employee/tao-chien-dich/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
      this.showMessage("warn", 'Bạn không có quyền truy cập');
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;

      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }
    }
  }

  setForm() {
    this.tenChienDichFormControl = new FormControl(null, [Validators.required]);
    this.ngayBatDauFormControl = new FormControl(null, [Validators.required]);
    this.ngayKetThucFormControl = new FormControl(null, [Validators.required]);
    this.phuTrachViTriTuyenDungFormControl = new FormControl(null);
    this.phuTrachChienDichFormControl = new FormControl(null);
    this.NoiDungChienDichFormControl = new FormControl(null);

    this.thongTinChienDichFormGroup = new FormGroup({
      tenChienDichFormControl: this.tenChienDichFormControl,
      ngayBatDauFormControl: this.ngayBatDauFormControl,
      ngayKetThucFormControl: this.ngayKetThucFormControl,
      phuTrachViTriTuyenDungFormControl: this.phuTrachViTriTuyenDungFormControl,
      phuTrachChienDichFormControl: this.phuTrachChienDichFormControl,
      NoiDungChienDichFormControl: this.NoiDungChienDichFormControl,
    });

    this.phuTrachChienDichFormControl.setValue(this.currentEmployeeCodeName);
    this.phuTrachChienDichFormControl.disable();
  }

  setTable() {
    this.colsFile = [
      { field: 'fileName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'right', type: 'number' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'right', type: 'date' },
      { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'left', type: 'string' },
    ];
  }



  goBackToList() {
    this.router.navigate(['/employee/danh-sach-chien-dich']);
  }

  resetEndDate() {
    this.ngayKetThucFormControl.reset();
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

  /*Event upload list file*/
  /*Event upload list file*/
  myUploader(event: any) {
    // let listFileUploadModel: Array<FileUploadModel> = [];
    // this.uploadedFiles.forEach(item => {
    //   let fileUpload: FileUploadModel = new FileUploadModel();
    //   fileUpload.FileInFolder = new FileInFolder();
    //   fileUpload.FileInFolder.active = true;
    //   let index = item.name.lastIndexOf(".");
    //   let name = item.name.substring(0, index);
    //   fileUpload.FileInFolder.fileName = name;
    //   fileUpload.FileInFolder.fileExtension = item.name.substring(index, item.name.length - index);
    //   fileUpload.FileInFolder.size = item.size;
    //   fileUpload.FileInFolder.objectId = this.contractId;
    //   fileUpload.FileInFolder.objectType = 'QLHD';
    //   fileUpload.FileSave = item;
    //   listFileUploadModel.push(fileUpload);
    // });

    // this.contractService.uploadFile("QLHD", listFileUploadModel, this.contractId).subscribe(response => {
    //   let result: any = response;
    //   this.loading = false;
    //   if (result.statusCode == 200) {
    //     this.uploadedFiles = [];
    //     if (this.fileUpload) {
    //       this.fileUpload.clear();  //Xóa toàn bộ file trong control
    //     }

    //     this.listFile = result.listFileInFolder;

    //     let msg = { severity: 'success', summary: 'Thông báo', detail: "Thêm file thành công" };
    //     this.showMessage(msg);
    //   } else {
    //     let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
    //     this.showMessage(msg);
    //   }
    // });
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

  create(isSaveNew) {
    if (!this.thongTinChienDichFormGroup.valid) {
      Object.keys(this.thongTinChienDichFormGroup.controls).forEach(key => {
        if (this.thongTinChienDichFormGroup.controls[key].valid == false) {
          this.thongTinChienDichFormGroup.controls[key].markAsTouched();
        }
      });
    } else {
      let recruitmentCampaignModel = new RecruitmentCampaignEntityModel();
      recruitmentCampaignModel.recruitmentCampaignName = this.tenChienDichFormControl.value ? this.tenChienDichFormControl.value : '';

      let startDate = this.ngayBatDauFormControl.value ? convertToUTCTime(this.ngayBatDauFormControl.value) : null;
      recruitmentCampaignModel.startDate = startDate;

      let endDate = this.ngayKetThucFormControl.value ? convertToUTCTime(this.ngayKetThucFormControl.value) : null;
      recruitmentCampaignModel.endDateDate = endDate;

      recruitmentCampaignModel.personInChargeId = this.auth.EmployeeId;
      recruitmentCampaignModel.recruitmentCampaignDes = this.NoiDungChienDichFormControl.value ? this.NoiDungChienDichFormControl.value : '';

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
        fileUpload.FileInFolder.objectType = 'RE_CAMP';
        fileUpload.FileSave = item;
        listFileUploadModel.push(fileUpload);
      });


      this.loading = true;
      this.awaitResult = true;
      this.employeeService.createRecruitmentCampaign(recruitmentCampaignModel, "RE_CAMP", listFileUploadModel).subscribe(response => {
        let result = <any>response;
        this.loading = false;
        if (result.statusCode === 202 || result.statusCode === 200) {

          this.showMessage('success', 'Tạo chiến dịch thành công');
          //Lưu và thêm mới
          if (isSaveNew) {
            this.resetFieldValue();
            this.awaitResult = false;
          }
          //Lưu
          else {
            this.resetFieldValue();
            setTimeout(() => {
              this.router.navigate(['/employee/chi-tiet-chien-dich', { recruitmentCampaignId: result.recruitmentCampaignId }]);
            }, 1000)
            this.awaitResult = false;
          }
        }
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage('error', result.messageCode);
          this.awaitResult = false;
        };
      });
    }
  }

  //Function reset toàn bộ các value đã nhập trên form
  resetFieldValue() {
    this.uploadedFiles = [];
    this.thongTinChienDichFormGroup.reset();
  }
  //Kết thúc
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
