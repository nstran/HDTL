import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { FormControl, FormGroup, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { ImageUploadService } from '../../../../app/shared/services/imageupload.service';
import { Vacancies } from '../../models/recruitment-vacancies.model';
import { EmployeeService } from '../../services/employee.service';
import { GetPermission } from '../../../shared/permission/get-permission';

class EmployeeModel {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  employeeCodeName: string;
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
  createdDate: Date;
  uploadByName: string;
}

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

@Component({
  selector: 'app-tuyen-dung-create',
  templateUrl: './tuyen-dung-create.component.html',
  styleUrls: ['./tuyen-dung-create.component.css']
})
export class TuyenDungCreateComponent implements OnInit {

  loading: boolean = false;
  today: Date = new Date();
  statusCode: string = null;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  currentEmployeeCodeName = localStorage.getItem('EmployeeCodeName');

  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  selectedChienDich: any = null;

  // Danh sach
  listPhuTrachViTriTuyenDung: Array<EmployeeModel> = [];
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

  // Tài liệu liên quan
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];
  colsFile: any[];

  // FORM
  thongTinChiTietVTTD: FormGroup;
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

  // Danh sách chiến dịch tuyển dụng
  listChienDich: Array<any> = [];

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
  /*End*/
  /*Get Current EmployeeId*/
  auth = JSON.parse(localStorage.getItem('auth'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  /*End*/
  isValidChienDich: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới
  soLuong: number = 0;

  constructor(
    private router: Router,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private employeeService: EmployeeService,
    private imageService: ImageUploadService,
    private getPermission: GetPermission,
    private def: ChangeDetectorRef,
  ) { }

  async ngOnInit() {
    this.setForm();
    this.setTable();
    let resource = "rec/employee/tao-cong-viec-tuyen-dung/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập' };
      this.showMessage(msg);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;

      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }

      this.getMasterdata();
    }
  }

  setTable() {
    this.colsFile = [
      { field: 'fileName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'right', type: 'number' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'right', type: 'date' },
      { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'left', type: 'string' },
    ];
  }

  async getMasterdata() {
    this.loading = true;

    let result: any = await this.employeeService.getMasterDataVacanciesCreate();
    if (result.statusCode == 200) {
      this.listPhuTrachViTriTuyenDung = result.listEmployee;
      this.listChienDich = result.listEmployeeRecruit;
      this.listKinhNghiem = result.listKinhNghiem;
      this.listLoaiCongViec = result.listLoaiCV;
      this.loading = false;
    }
    else
      this.loading = false;
  }

  setForm() {
    this.tenViTriTuyenDungFormControl = new FormControl(null, [Validators.required]);
    this.soLuongFormControl = new FormControl(null, [Validators.required]);
    this.mucUuTienFormControl = new FormControl(null, [Validators.required]);
    this.nguoiPhuTrachFormControl = new FormControl(null);
    this.loaiCongViecFormControl = new FormControl(null);
    this.noiLamViecFormControl = new FormControl(null);
    this.kinhNghiemFormControl = new FormControl(null);
    this.loaiTienTeFormControl = new FormControl(null);
    this.kieuLuongFormControl = new FormControl(null);
    this.tuFormControl = new FormControl(0);
    this.denFormControl = new FormControl(0);

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

  // Tạo chiến dịch
  btn_createVacancies(value: boolean) {
    if (this.selectedChienDich == null) {
      this.isValidChienDich = true;
    }
    else
      this.isValidChienDich = false;

    if (!this.thongTinViTriTuyenDungFormGroup.valid) {
      Object.keys(this.thongTinViTriTuyenDungFormGroup.controls).forEach(key => {
        if (this.thongTinViTriTuyenDungFormGroup.controls[key].valid == false) {
          this.thongTinViTriTuyenDungFormGroup.controls[key].markAsTouched();
        }
      });
      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.thongTinViTriTuyenDungFormGroup.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    }
    else {
      if (this.selectedChienDich == null) {
        this.isValidChienDich = true;
        this.isInvalidForm = true;  //Hiển thị icon-warning-active
        this.isOpenNotifiError = true;  //Hiển thị message lỗi
        setTimeout(() => {
          this.isValidChienDich = false;
          this.isInvalidForm = false;
          this.isOpenNotifiError = false;
        }, 4000);
      }
      else {
        if (this.soLuongFormControl.value == 0) {
          this.isInvalidForm = true;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = true;  //Hiển thị message lỗi
          return;
        }
        this.isInvalidForm = false;  //Hiển thị icon-warning-active
        this.isOpenNotifiError = false;  //Hiển thị message lỗi
        // File tài liệu
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
          fileUpload.FileInFolder.objectType = 'VACANCIES';
          fileUpload.FileSave = item;
          listFileUploadModel.push(fileUpload);
        });

        /*Binding data for vị trí tuyển dụng model*/
        let viTriTyenDung: Vacancies = this.mapDataToModel();
        this.awaitResult = false;
        //this.loading = true;
        this.createVacancies(viTriTyenDung, listFileUploadModel);
      }
    }
  }

  createVacancies(viTriTyenDung: any, listFileUploadModel: Array<FileUploadModel>) {
    this.employeeService.createVacancies(viTriTyenDung, "VACANCIES", listFileUploadModel).subscribe(response => {

      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        let messageCode = "Tạo vị trí chiến dịch thành công";
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: messageCode };
        this.showMessage(mgs);

        setTimeout(() => {
          this.router.navigate(['/employee/chi-tiet-cong-viec-tuyen-dung', { vacanciesId: result.vacanciesId, recruitmentCampaignId: this.selectedChienDich.recruitmentCampaignId }]);
        }, 1000);
      } else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  changeSalary(event: any) {
    if (event.value.value == "1")
      this.isFromTo = true;
    else
      this.isFromTo = false;
  }
  mapDataToModel(): Vacancies {
    let viTriTD = new Vacancies();

    viTriTD.vacanciesId = this.emptyGuid;
    // Mã chiến dịch
    viTriTD.recruitmentCampaignId = this.selectedChienDich.recruitmentCampaignId;

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
    viTriTD.placeOfWork = this.noiLamViecFormControl.value == null ? null : this.noiLamViecFormControl.value;

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

    viTriTD.vacanciesDes = this.viTriTuyenDungFormControl.value;

    viTriTD.professionalRequirements = this.yeuCauChuyenMonFormControl.value;

    viTriTD.candidateBenefits = this.quyenLoiUngVienFormControl.value;

    return viTriTD;
  }

  // Upload file to server
  uploadFiles(files: File[]) {
    this.imageService.uploadFile(files).subscribe(response => { });
  }

  goBackToList() {
    this.router.navigate(['/employee/danh-sach-cong-viec-tuyen-dung']);
  }

  create(isSaveNew) {
    this.router.navigate(['/employee/chi-tiet-cong-viec-tuyen-dung']);
  }

  changeFromMoney() {
    /*Bật validator kiểm tra Giá sản phẩm*/
    this.tuFormControl.setValidators([compareNumberValidator(ParseStringToFloat(this.denFormControl.value))]);
    this.tuFormControl.updateValueAndValidity();

    this.denFormControl.clearValidators();
    this.denFormControl.updateValueAndValidity();
    this.def.detectChanges();
  }

  changeToMoney() {
    this.denFormControl.setValidators([compareNumberValidatorLess(ParseStringToFloat(this.tuFormControl.value))]);
    this.denFormControl.updateValueAndValidity();

    this.tuFormControl.clearValidators();
    this.tuFormControl.updateValueAndValidity();

    this.def.detectChanges();
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

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }
  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
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
//So sánh giá trị nhập vào với một giá trị xác định
function compareNumberValidator(number: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } | null => {
    if (control.value !== undefined && (parseFloat(control.value.replace(/,/g, '')) >= number)) {
      return { 'numberInvalid': true };
    }
    return null;
  };
}

//So sánh giá trị nhập vào với một giá trị xác định
function compareNumberValidatorLess(number: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } | null => {
    if (control.value !== undefined && (parseFloat(control.value.replace(/,/g, '')) < number)) {
      return { 'numberInvalid': true };
    }
    return null;
  };
}
