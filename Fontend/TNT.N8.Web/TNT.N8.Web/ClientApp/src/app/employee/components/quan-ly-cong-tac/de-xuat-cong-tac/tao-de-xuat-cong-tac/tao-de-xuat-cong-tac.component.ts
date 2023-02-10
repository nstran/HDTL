import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../../../../../src/app/shared/permission/get-permission';
import { MessageService, ConfirmationService } from 'primeng/api';
import { EmployeeService } from '../../../../services/employee.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DialogService, FileUpload, Table } from 'primeng';
import { DeXuatCongTacModel } from '../../../../../employee/models/de-xuat-cong-tac';
import { QuanLyCongTacService } from '../../../../../employee/services/quan-ly-cong-tac/quan-ly-cong-tac.service';
import { DeXuatCongTacChiTietModel } from '../../../../../employee/models/de-xuat-cong-tac-chi-tiet';
import { EncrDecrService } from '../../../../../shared/services/encrDecr.service';


class EmployeeModel {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  employeeLastname: string;
  employeeFirstname: string;
  startedDate: Date;
  organizationId: string;
  positionId: string;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  active: Boolean;
  username: string;
  identity: string;
  organizationName: string;
  avatarUrl: string;
  positionName: string;
  contactId: string;
  isManager: boolean;
  permissionSetId: string;
  probationEndDate: Date;
  probationStartDate: Date;
  trainingStartDate: Date;
  contractType: string;
  contractEndDate: Date;
  isTakeCare: boolean;
  isXuLyPhanHoi: boolean;
  organizationLevel: number;
  dateOfBirth: Date;
  constructor() {
    this.organizationName = "";
    this.dateOfBirth = null;
    this.employeeCode = "";
    this.positionName = "";
    this.identity = "";
  }
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

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

@Component({
  selector: 'app-tao-de-xuat-cong-tac',
  templateUrl: './tao-de-xuat-cong-tac.component.html',
  styleUrls: ['./tao-de-xuat-cong-tac.component.css']
})

export class TaoDeXuatCongTacComponent implements OnInit {
  auth = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  colsListEmp: any[];
  selectedColumns: any[];

  listEmpDefault: any[];
  listEmp: any[];
  //listDeXuatCongTacChiTietTemp: Array<EmployeeInterface> = new Array<EmployeeInterface>();
  today = new Date();

  //Form thông tin đề xuất
  thongTinDeXuatFormGroup: FormGroup;
  tenDeXuatFormControl: FormControl;
  ngayDeXuatFormControl: FormControl;
  nguoiDeXuatFormControl: FormControl;

  //Form của phiếu đề xuất công tác
  deXuatCongTacFormGroup: FormGroup;
  donViFormControl: FormControl;
  diaDiemFormControl: FormControl;
  phuongTienFormControl: FormControl;
  nhiemVuFormControl: FormControl;
  thoiGianBatDauFormControl: FormControl;
  thoiGianKetThucFormControl: FormControl;

  //Ds nhân viên được đề xuất
  nhanVienFormGroup: FormGroup;
  nhanVienControl: FormControl;
  lyDoControl: FormControl;
  filterGlobal: string = '';

  actionAdd: boolean = true;
  employeeModel: EmployeeModel = new EmployeeModel();
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;

  // Tài liệu liên quan
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];
  colsFile: any[];
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  isUpdate: boolean = false;

  listDeXuatCongTacChiTietTemp: Array<DeXuatCongTacChiTietModel> = new Array<DeXuatCongTacChiTietModel>();
  chiTietDeXuatCongTacId: number = 0;
  deXuatCTChiTietModel: DeXuatCongTacChiTietModel = new DeXuatCongTacChiTietModel();
  nguoiDeXuatId: string = '00000000-0000-0000-0000-000000000000';
  @ViewChild('myTable') myTable: Table;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    private def: ChangeDetectorRef,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private getPermission: GetPermission,
    private quanLyCongtacService: QuanLyCongTacService,
    private encrDecrService: EncrDecrService,
  ) { }

  async ngOnInit() {
    this.setForm();
    this.setCols();
    let resource = "hrm/employee/tao-de-xuat-cong-tac/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(msg);
    this.router.navigate(['/home']);
    } else {

      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("view") == -1) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
        this.showMessage(msg);
      }
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
    }

    this.getMasterData();
    //}
  }

  setForm() {
    this.tenDeXuatFormControl = new FormControl(null, [Validators.required]);
    this.ngayDeXuatFormControl = new FormControl(null, [Validators.required]);
    this.nguoiDeXuatFormControl = new FormControl({ value: null, disabled: true });

    this.thongTinDeXuatFormGroup = new FormGroup({
      tenDeXuatFormControl: this.tenDeXuatFormControl,
      ngayDeXuatFormControl: this.ngayDeXuatFormControl,
      nguoiDeXuatFormControl: this.nguoiDeXuatFormControl
    });

    this.nhanVienControl = new FormControl(null, [Validators.required]);
    this.lyDoControl = new FormControl(null, [Validators.required]);

    this.nhanVienFormGroup = new FormGroup({
      nhanVienControl: this.nhanVienControl,
      lyDoControl: this.lyDoControl,
    });

    this.donViFormControl = new FormControl(null, [Validators.required]);
    this.diaDiemFormControl = new FormControl(null);
    this.phuongTienFormControl = new FormControl(null);
    this.nhiemVuFormControl = new FormControl(null);
    this.thoiGianBatDauFormControl = new FormControl(null, [Validators.required]);
    this.thoiGianKetThucFormControl = new FormControl(null, [Validators.required]);

    this.deXuatCongTacFormGroup = new FormGroup({
      donViFormControl: this.donViFormControl,
      diaDiemFormControl: this.diaDiemFormControl,
      phuongTienFormControl: this.phuongTienFormControl,
      nhiemVuFormControl: this.nhiemVuFormControl,
      thoiGianBatDauFormControl: this.thoiGianBatDauFormControl,
      thoiGianKetThucFormControl: this.thoiGianKetThucFormControl,
    });

    this.ngayDeXuatFormControl.setValue(new Date());
  }

  setCols() {
    this.colsListEmp = [
      { field: 'tenNhanVien', header: 'Họ tên', textAlign: 'left', display: 'table-cell', width: "250px" },
      { field: 'phongBan', header: 'Phòng ban', textAlign: 'left', display: 'table-cell', width: '200px' },
      { field: 'viTriLamViec', header: 'Chức vụ', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'dateOfBirth', header: 'Ngày sinh', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'identity', header: 'Số CMT/CCCD', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'lyDo', header: 'Lý do', textAlign: 'center', display: 'table-cell', width: '150px' },
    ];

    this.selectedColumns = this.colsListEmp;
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.employeeService.getMasterDataDeXuatCongTac();
    this.loading = false;
    if (result.statusCode == 200) {
      this.listEmpDefault = result.listEmp;
      this.listEmp = result.listEmp;

      let nguoiDeXuat = this.listEmpDefault.find(x => x.employeeId == result.employeeId);
      this.nguoiDeXuatFormControl.setValue(nguoiDeXuat);
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  refreshFilter() {
    this.filterGlobal = '';
    if (this.myTable) {
      this.myTable.reset();
    }
  }

  thoat() {
    this.router.navigate(['/employee/danh-sach-de-xuat-cong-tac']);
  }

  thayDoiNhanVien(event: any) {
    let nhanVien = this.listEmpDefault.find(x => x.employeeId == event.value.employeeId);
    this.employeeModel = nhanVien;
    this.employeeModel.dateOfBirth = new Date(this.employeeModel.dateOfBirth)
  }

  themNhanVien() {
    if (!this.nhanVienFormGroup.valid) {
      Object.keys(this.nhanVienFormGroup.controls).forEach(key => {
        if (this.nhanVienFormGroup.controls[key].valid == false) {
          this.nhanVienFormGroup.controls[key].markAsTouched();
        }
      });
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Vui lòng nhập đầy đủ thông tin các trường dữ liệu.' };
      this.showMessage(msg);
      return;
    }

    let nhanVienModel = this.mapFormToChiTietDXCongTacModelTemp();

    let exists = this.listDeXuatCongTacChiTietTemp.find(x => x.employeeId == nhanVienModel.employeeId);
    if (exists) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Nhân viên này đã được thêm trong danh sách' };
      this.showMessage(msg);
      return;
    }

    this.listDeXuatCongTacChiTietTemp = [...this.listDeXuatCongTacChiTietTemp, nhanVienModel];
    this.nhanVienControl.reset();
    this.def.detectChanges();
    this.refreshForm();
  }

  mapFormToChiTietDXCongTacModelTemp(isUpdate: boolean = false): DeXuatCongTacChiTietModel {
    let deXuatChiTietModel = new DeXuatCongTacChiTietModel();
    if (!isUpdate) {
      const maxValueOfY = Math.max(...this.listDeXuatCongTacChiTietTemp.map(o => o.chiTietDeXuatCongTacId), 0);
      deXuatChiTietModel.chiTietDeXuatCongTacId = maxValueOfY + 1;
    }

    // Nhân viên
    let nhanVien = this.nhanVienFormGroup.get('nhanVienControl').value;
    if (nhanVien != null && nhanVien != undefined) {
      deXuatChiTietModel.employeeId = nhanVien ? nhanVien.employeeId : this.emptyGuid;
      deXuatChiTietModel.tenNhanVien = nhanVien.employeeName;
      deXuatChiTietModel.phongBan = nhanVien.organizationName;
      deXuatChiTietModel.viTriLamViec = nhanVien.positionName;
      deXuatChiTietModel.identity = nhanVien.identity;
      deXuatChiTietModel.dateOfBirth = nhanVien.dateOfBirth;
    }

    // Lý do
    deXuatChiTietModel.lyDo = this.lyDoControl.value;

    deXuatChiTietModel.createdById = this.auth.UserId;
    deXuatChiTietModel.createdDate = convertToUTCTime(new Date());
    deXuatChiTietModel.updatedById = null;
    deXuatChiTietModel.updatedDate = null;

    return deXuatChiTietModel;
  }

  refreshForm() {
    this.def.detectChanges();
    this.chiTietDeXuatCongTacId = 0;
    this.nhanVienFormGroup.reset();
    this.employeeModel = new EmployeeModel();
    this.deXuatCTChiTietModel = new DeXuatCongTacChiTietModel();
    this.isUpdate = false;
  }

  resetFieldValue() {
    this.listDeXuatCongTacChiTietTemp = [];
    this.nhanVienFormGroup.reset();
    this.deXuatCongTacFormGroup.reset();
    this.thongTinDeXuatFormGroup.reset();
    this.clearAllFile();
    this.arrayDocumentModel = [];
    this.ngayDeXuatFormControl.setValue(new Date());
  }

  async capNhatNhanVien() {
    if (!this.nhanVienFormGroup.valid) {
      Object.keys(this.nhanVienFormGroup.controls).forEach(key => {
        if (!this.nhanVienFormGroup.controls[key].valid) {
          this.nhanVienFormGroup.controls[key].markAsTouched();
        }
      });
    }
    else {
      let deXuatModelUpdate = this.mapFormToChiTietDXCongTacModelTemp(true);

      const index = this.listDeXuatCongTacChiTietTemp.findIndex(obj => obj.chiTietDeXuatCongTacId === this.chiTietDeXuatCongTacId);
      this.listDeXuatCongTacChiTietTemp[index] = deXuatModelUpdate;
      this.def.detectChanges();
      this.refreshForm();
    }
  }

  removeEmp(rowData) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        if (this.isUpdate && this.employeeModel.employeeId == rowData.employeeId) {
          this.isUpdate = false;
        }
        this.listDeXuatCongTacChiTietTemp = this.listDeXuatCongTacChiTietTemp.filter(x => x.chiTietDeXuatCongTacId != rowData.chiTietDeXuatCongTacId);
      }
    });
  }

  reShowDeXuatChiTiet(rowData) {
    this.isUpdate = true;
    this.chiTietDeXuatCongTacId = rowData.chiTietDeXuatCongTacId;

    // Mô tả
    this.nhanVienFormGroup.controls['lyDoControl'].setValue(rowData.lyDo);

    // Mã nhân viên
    let nhanVien = this.listEmpDefault.find(c => c.employeeId == rowData.employeeId);
    this.def.detectChanges();

    this.employeeModel = nhanVien;

    this.nhanVienFormGroup.controls['nhanVienControl'].updateValueAndValidity();
    this.nhanVienFormGroup.controls['lyDoControl'].updateValueAndValidity();
    this.nhanVienFormGroup.controls['nhanVienControl'].setValue(nhanVien);
  }

  async taoDeXuatCongTac(isAddNew) {
    if (!this.thongTinDeXuatFormGroup.valid) {
      Object.keys(this.thongTinDeXuatFormGroup.controls).forEach(key => {
        if (this.thongTinDeXuatFormGroup.controls[key].valid == false) {
          this.thongTinDeXuatFormGroup.controls[key].markAsTouched();
        }
      });
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Vui lòng nhập đầy đủ thông tin các trường dữ liệu.' };
      this.showMessage(msg);
      return;
    }

    if (!this.deXuatCongTacFormGroup.valid) {
      Object.keys(this.deXuatCongTacFormGroup.controls).forEach(key => {
        if (this.deXuatCongTacFormGroup.controls[key].valid == false) {
          this.deXuatCongTacFormGroup.controls[key].markAsTouched();
        }
      });
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Vui lòng nhập đầy đủ thông tin các trường dữ liệu.' };
      this.showMessage(msg);
      return;
    }

    if (this.listDeXuatCongTacChiTietTemp.length == 0) {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Vui lòng chọn viên được đề xuất!' };
      this.showMessage(msg);
      return;
    }

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
      fileUpload.FileInFolder.objectType = 'DEXUATCT';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    let deXuatModel: DeXuatCongTacModel = this.mapDataFormToModel();

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.quanLyCongtacService.createOrUpdateDeXuatCT(deXuatModel, this.listDeXuatCongTacChiTietTemp, 'DEXUATCT', listFileUploadModel, this.auth.UserId);
    this.loading = false;
    this.awaitResult = false;
    if (result.statusCode == 200) {
      let msg = { severity: 'success', summary: 'Thông báo:', detail: "Tạo đề xuất công tác thành công" };
      this.showMessage(msg);

      //Lưu và thêm mới
      if (isAddNew) {
        this.resetFieldValue();
        this.listDeXuatCongTacChiTietTemp = new Array<DeXuatCongTacChiTietModel>();
      }
      //Lưu
      else {
        this.resetFieldValue();
        setTimeout(() => {
          this.router.navigate(['/employee/de-xuat-cong-tac-chi-tiet', { deXuatCongTacId: this.encrDecrService.set(result.deXuatCongTacId) }]);
        }, 1000)
      }
    }
    else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
      this.showMessage(msg);
    }
  }

  mapDataFormToModel(): DeXuatCongTacModel {
    let formDataModel = new DeXuatCongTacModel();

    formDataModel.tenDeXuat = this.tenDeXuatFormControl.value;
    let ngayDeXuat = this.thongTinDeXuatFormGroup.get('ngayDeXuatFormControl').value ? convertToUTCTime(this.thongTinDeXuatFormGroup.get('ngayDeXuatFormControl').value) : null;
    formDataModel.ngayDeXuat = ngayDeXuat;

    formDataModel.nguoiDeXuatId = this.nguoiDeXuatFormControl.value == null ? this.auth.EmployeeId : this.nguoiDeXuatFormControl.value.employeeId;

    formDataModel.donVi = this.donViFormControl.value;
    formDataModel.diaDiem = this.diaDiemFormControl.value ?? '';
    formDataModel.phuongTien = this.phuongTienFormControl.value;
    formDataModel.nhiemVu = this.nhiemVuFormControl.value;

    let ngayBatDau = this.deXuatCongTacFormGroup.get('thoiGianBatDauFormControl').value ? convertToUTCTime(this.deXuatCongTacFormGroup.get('thoiGianBatDauFormControl').value) : null;
    formDataModel.thoiGianBatDau = ngayBatDau;

    let ngayKetThuc = this.deXuatCongTacFormGroup.get('thoiGianKetThucFormControl').value ? convertToUTCTime(this.deXuatCongTacFormGroup.get('thoiGianKetThucFormControl').value) : null;
    formDataModel.thoiGianKetThuc = ngayKetThuc;

    formDataModel.createdById = this.auth.UserId;
    formDataModel.createdDate = convertToUTCTime(new Date());
    formDataModel.updatedById = null;
    formDataModel.updatedDate = null;
    return formDataModel;
  }

  //#region TÀI LIỆU LIÊN QUAN

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
  //#endregion
}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
