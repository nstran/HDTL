import { CommonService } from './../../../../shared/services/common.service';
import { CauHinhOtCaNgay } from './../../../../salary/models/cau-hinh-ot-ca-ngay.model';
import { Component, OnInit, ChangeDetectorRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { EmployeeService } from '../../../services/employee.service';
import { KeHoachOtModel } from '../../../models/keHoach-Ot.model';
import { ChonNhieuDvDialogComponent } from '../../../../shared/components/chon-nhieu-dv-dialog/chon-nhieu-dv-dialog.component';
import { DialogService } from 'primeng/dynamicdialog';
import { EncrDecrService } from '../../../../shared/services/encrDecr.service';

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

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

class KeHoachOtPhongBanEntityModel {
  id: number;
  OrganizationId: SVGStringList;
  KeHoachOtId: number;
}

@Component({
  selector: 'app-kehoach-ot-create',
  templateUrl: './kehoach-ot-create.component.html',
  styleUrls: ['./kehoach-ot-create.component.css']
})
export class KeHoachOtCreateComponent implements OnInit {
  toDay = new Date();
  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới  
  statusCode: string = null;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  @ViewChild('fileUpload') fileUpload: FileUpload;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  newKeHoachOtId: number = 0;
  keHoachOtModel: KeHoachOtModel = new KeHoachOtModel();
  currentEmployeeCodeName = localStorage.getItem('EmployeeCodeName');
  auth = JSON.parse(localStorage.getItem('auth'));

  cauHinhOtCaNgay: CauHinhOtCaNgay = null;
  gioBatDau: string = null;
  gioKetThuc: string = null;
  listLoaiOt: Array<any> = [];
  listLoaiCaOt: Array<any> = [];
  listPhongBan: Array<any> = [];
  listAllViTri: Array<any> = [];
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];
  colsFile: any[];

  // FORM GROUP
  keHoachOtFormGroup: FormGroup;
  loaiOtFormControl: FormControl;
  tenKeHoachFormControl: FormControl;
  ngayDeXuatFormControl: FormControl;
  nguoiDeXuatIdFormControl: FormControl;
  donViIdFormControl: FormControl;
  donViNameIdFormControl: FormControl;
  diaDiemFormControl: FormControl
  ngayBatDauFormControl: FormControl;
  ngayKetThucFormControl: FormControl;
  gioBatDauFormControl: FormControl;
  gioKetThucFormControl: FormControl;
  lyDoFormControl: FormControl;
  hanPheDuyetKeHoachFormControl: FormControl;
  hanDangKyOTFormControl: FormControl;
  hanPheDuyetDangKyFormControl: FormControl;
  loaiCaOtControl: FormControl;

  listSelectedDonVi: any = [];
  listEmp: any = [];
  actionAdd:boolean = true;

  constructor(
    private router: Router,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    private confirmationService: ConfirmationService,
    private def: ChangeDetectorRef,
    private getPermission: GetPermission,
    public dialogService: DialogService,
    private commonService: CommonService,
    private encrDecrService: EncrDecrService,
  ) { }

  async ngOnInit() {
    this.setForm();
    this.setTable();
    let resource = "hrm/employee/kehoach-ot-create/";
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
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
    }
    this.getMasterData();
    // }
  }

  setForm() {
    this.loaiOtFormControl = new FormControl(null, [Validators.required]);
    this.tenKeHoachFormControl = new FormControl(null, [Validators.required]);
    this.ngayDeXuatFormControl = new FormControl(null, [Validators.required]);
    this.nguoiDeXuatIdFormControl = new FormControl(null, [Validators.required]);
    this.donViIdFormControl = new FormControl(null, [Validators.required]);
    this.donViNameIdFormControl = new FormControl(null, [Validators.required]);
    this.diaDiemFormControl = new FormControl(null, [Validators.required]);
    this.ngayBatDauFormControl = new FormControl(null, [Validators.required]);
    this.ngayKetThucFormControl = new FormControl(null, [Validators.required]);
    this.gioBatDauFormControl = new FormControl(null, [Validators.required]);
    this.gioKetThucFormControl = new FormControl(null, [Validators.required]);
    this.lyDoFormControl = new FormControl(null, [Validators.required]);
    this.hanPheDuyetKeHoachFormControl = new FormControl(null, [Validators.required]);
    this.hanDangKyOTFormControl = new FormControl(null, [Validators.required]);
    this.hanPheDuyetDangKyFormControl = new FormControl(null, [Validators.required]);
    this.loaiCaOtControl = new FormControl(null, [Validators.required]);

    this.keHoachOtFormGroup = new FormGroup({
      loaiOtFormControl: this.loaiOtFormControl,
      tenKeHoachFormControl: this.tenKeHoachFormControl,
      ngayDeXuatFormControl: this.ngayDeXuatFormControl,
      nguoiDeXuatIdFormControl: this.nguoiDeXuatIdFormControl,
      donViIdFormControl: this.donViIdFormControl,
      donViNameIdFormControl: this.donViNameIdFormControl,
      diaDiemFormControl: this.diaDiemFormControl,
      ngayBatDauFormControl: this.ngayBatDauFormControl,
      ngayKetThucFormControl: this.ngayKetThucFormControl,
      gioBatDauFormControl: this.gioBatDauFormControl,
      gioKetThucFormControl: this.gioKetThucFormControl,
      lyDoFormControl: this.lyDoFormControl,
      hanPheDuyetKeHoachFormControl: this.hanPheDuyetKeHoachFormControl,
      hanDangKyOTFormControl: this.hanDangKyOTFormControl,
      hanPheDuyetDangKyFormControl: this.hanPheDuyetDangKyFormControl,
      loaiCaOtControl: this.loaiCaOtControl
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
    this.employeeService.getMasterCreateKeHoachOt(this.auth.UserId).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.cauHinhOtCaNgay = result.cauHinhOtCaNgay;
        this.listLoaiOt = result.listLoaiOt;
        this.listLoaiCaOt = result.listLoaiCaOt;
        this.listPhongBan = result.listOrganization;
        this.listEmp = result.currentEmp;
        this.nguoiDeXuatIdFormControl.setValue(this.listEmp[0]);
        this.ngayDeXuatFormControl.setValue(this.toDay);
        this.nguoiDeXuatIdFormControl.disable();
      }
      else {
        this.showMessage('error', result.messageCode);
      }
    });
  }

  changeLoaiCaOt() {
    let loaiCaOt = this.loaiCaOtControl.value;

    if (!loaiCaOt && !this.cauHinhOtCaNgay) return;

    //Nếu là ca tối
    if (loaiCaOt.value == 3) {
      this.gioBatDauFormControl.setValidators([Validators.required]);
      this.gioKetThucFormControl.setValidators([Validators.required]);
    }
    //Nếu không phải ca tối
    else {
      this.gioBatDauFormControl.setValidators([]);
      this.gioKetThucFormControl.setValidators([]);
    }

    this.gioBatDauFormControl.updateValueAndValidity();
    this.gioKetThucFormControl.updateValueAndValidity();

    switch (loaiCaOt.value) {
      //Ca sáng 
      case 1:
        this.gioBatDau = this.cauHinhOtCaNgay?.gioVaoSang.slice(0, this.cauHinhOtCaNgay?.gioVaoSang.lastIndexOf(':'));
        this.gioKetThuc = this.cauHinhOtCaNgay?.gioRaSang.slice(0, this.cauHinhOtCaNgay?.gioRaSang.lastIndexOf(':'));
        break;

      //Ca chiều
      case 2:
        this.gioBatDau = this.cauHinhOtCaNgay?.gioVaoChieu.slice(0, this.cauHinhOtCaNgay?.gioVaoChieu.lastIndexOf(':'));
        this.gioKetThuc = this.cauHinhOtCaNgay?.gioRaChieu.slice(0, this.cauHinhOtCaNgay?.gioRaChieu.lastIndexOf(':'));
        break;

      //Ca tối
      case 3:
        this.gioBatDau = null;
        this.gioKetThuc = null;
        break;

      //Cả ngày
      case 4:
        this.gioBatDau = this.cauHinhOtCaNgay?.gioVaoSang.slice(0, this.cauHinhOtCaNgay?.gioVaoSang.lastIndexOf(':'));
        this.gioKetThuc = this.cauHinhOtCaNgay?.gioRaChieu.slice(0, this.cauHinhOtCaNgay?.gioRaChieu.lastIndexOf(':'));
        break;

      default:
        this.gioBatDau = null;
        this.gioKetThuc = null;
        break;
    }
  }

  openOrgPopup() {
    let listSelectedId = this.listSelectedDonVi.map(item => item.organizationId);

    let ref = this.dialogService.open(ChonNhieuDvDialogComponent, {
      data: {
        mode: 1,
        listSelectedId: listSelectedId,
        type: 'KeHoachOt',
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
          this.donViIdFormControl.setValue(result);
          this.donViNameIdFormControl.setValue(listSelectedTenDonVi);
        }
        else {
          this.listSelectedDonVi = [];
          this.donViIdFormControl.setValue(null);
        }
      }
    });
  }

  goBackToList() {
    this.router.navigate(['/employee/danh-sach-de-xuat-ot']);
  }

  create(isSaveNew) {
    if (!this.keHoachOtFormGroup.valid) {
      Object.keys(this.keHoachOtFormGroup.controls).forEach(key => {
        if (this.keHoachOtFormGroup.controls[key].valid == false) {
          this.keHoachOtFormGroup.controls[key].markAsTouched();
        }
      });
      return;
    }

    let loaiCaOt = this.loaiCaOtControl.value;

    let gioBatDau = null;
    let gioKetThuc = null;

    //Nếu là ca tối
    if (loaiCaOt.value == 3) {
      gioBatDau = this.commonService.convertDateToTimeSpan(this.gioBatDauFormControl.value);
      gioKetThuc = this.commonService.convertDateToTimeSpan(this.gioKetThucFormControl.value);

      if (this.gioBatDauFormControl.value.getTime() > this.gioKetThucFormControl.value.getTime()) {
        this.showMessage('warn', 'Giờ bắt đầu không được lớn hơn giờ kết thúc');
        return;
      }
    }
    //Nếu không phải ca tối
    else {
      gioBatDau = this.gioBatDau;
      gioKetThuc = this.gioKetThuc;
    }

    let keHoachOtModel = new KeHoachOtModel();
    keHoachOtModel.DiaDiem = this.diaDiemFormControl.value ? this.diaDiemFormControl.value : '';
    keHoachOtModel.GioBatDau = gioBatDau;
    keHoachOtModel.GioKetThuc = gioKetThuc;
    keHoachOtModel.KeHoachOtId = 0;
    keHoachOtModel.LoaiOtId = this.loaiOtFormControl.value.categoryId;
    keHoachOtModel.LyDo = this.lyDoFormControl.value;
    keHoachOtModel.NgayBatDau = this.ngayBatDauFormControl.value;
    keHoachOtModel.NgayDeXuat = this.ngayDeXuatFormControl.value;
    keHoachOtModel.NgayKetThuc = this.ngayKetThucFormControl.value;
    keHoachOtModel.NguoiDeXuatId = this.nguoiDeXuatIdFormControl.value.employeeId;
    keHoachOtModel.TenKeHoach = this.tenKeHoachFormControl.value;
    keHoachOtModel.TrangThai = 1;
    keHoachOtModel.HanPheDuyetKeHoach = this.hanPheDuyetKeHoachFormControl.value;
    keHoachOtModel.HanDangKy = this.hanDangKyOTFormControl.value;
    keHoachOtModel.HanPheDuyetDangKy = this.hanPheDuyetDangKyFormControl.value;
    keHoachOtModel.LoaiCaId = this.loaiCaOtControl.value.value;

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
      fileUpload.FileInFolder.objectNumber = 0;
      fileUpload.FileInFolder.objectType = 'KEHOACHOT';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    let listPhongBan: Array<KeHoachOtPhongBanEntityModel> = [];
    if (this.donViIdFormControl.value) {
      this.donViIdFormControl.value.forEach(item => {
        var newObj = new KeHoachOtPhongBanEntityModel();
        newObj.OrganizationId = item.organizationId;
        listPhongBan.push(newObj);
      });
    }

    this.loading = true;
    this.awaitResult = true;
    this.employeeService.createOrUpdateKeHoachOt(keHoachOtModel, this.newKeHoachOtId, "KEHOACHOT", listFileUploadModel, listPhongBan).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode === 202 || result.statusCode === 200) {

        this.showMessage('success', 'Tạo kế hoạch OT thành công');
        //Lưu và thêm mới
        if (isSaveNew) {
          this.awaitResult = false;
        }
        //Lưu
        else {
          setTimeout(() => {
            this.router.navigate(['/employee/kehoach-ot-detail', { deXuatOTId: this.encrDecrService.set(result.keHoachOtId) }]);
          }, 1000)
          this.awaitResult = false;
        }
        this.resetFieldValue();
      }
      else {
        this.showMessage('error', result.messageCode);
        this.awaitResult = false;
      };
    });
  }

  //   onChangeChienDich(chienDichId: string) {
  //     this.listViTriTuyenDung = this.listAllViTri.filter(x => x.recruitmentCampaignId == chienDichId);
  //     // this.router.navigate(['/project/create-project-task', { 'projectId': projectId }]);
  //   }

  //   onChangeViTri(viTriId: string) {
  //     // this.router.navigate(['/project/create-project-task', { 'projectId': projectId }]);
  //   }

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
    this.keHoachOtFormGroup.reset();
    this.nguoiDeXuatIdFormControl.setValue(this.currentEmployeeCodeName);
    this.def.detectChanges();
  }
  //Kết thúc

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

  changeTuNgay() {
    this.hanPheDuyetKeHoachFormControl.reset();
    this.hanDangKyOTFormControl.reset();
    this.hanPheDuyetDangKyFormControl.reset();
  }

  changeHanPheDuyetKH() {
    this.hanDangKyOTFormControl.reset();
    this.hanPheDuyetDangKyFormControl.reset();
  }

  changeHanDangKy() {
    this.hanPheDuyetDangKyFormControl.reset();
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};