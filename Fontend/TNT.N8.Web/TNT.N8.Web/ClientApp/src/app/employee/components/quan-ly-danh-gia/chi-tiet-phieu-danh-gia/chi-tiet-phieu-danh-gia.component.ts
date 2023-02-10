import { async } from '@angular/core/testing';
import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { EmployeeService } from '../../../services/employee.service';
import { Row, Workbook, Worksheet } from 'exceljs';
import { saveAs } from "file-saver";
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { interactionSettingsStore } from '@fullcalendar/core';
import * as XLSX from 'xlsx';
import { DialogService, FileUpload } from 'primeng';
import { max } from 'moment';
import { EncrDecrService } from '../../../../shared/services/encrDecr.service';
import { EmployeeListService } from '../../../services/employee-list/employee-list.service';



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

class CauHoiPhieuDanhGiaMapping {
  cauHoiPhieuDanhGiaMappingId: number;
  noiDungCauHoi: string;
  tiLe: number;
  danhSachItem: any;
  parentId: number;
  nguoiDanhGia: number;
  cauTraLoi: any;
  isValid: Boolean;
  stt: number;
}

class PhieuDanhGia {
  PhieuDanhGiaId: string;
  TenPhieuDanhGia: string;
  ThangDiemDanhGiaId: string;
  CachTinhTong: string;
  TrangThaiPhieuDanhGia: number;
}


@Component({
  selector: 'app-chi-tiet-phieu-danh-gia',
  templateUrl: './chi-tiet-phieu-danh-gia.component.html',
  styleUrls: ['./chi-tiet-phieu-danh-gia.component.css']
})



export class ChiTietPhieuDanhGiaComponent implements OnInit {
  auth = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;

  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];
  statusCode: string = null;

  colsBangDanhGia: any[];
  colsBangDanhGiaQL: any[];

  listEmpAdd: any[]; // List nhân viên chọn để thêm đề xuất

  listNoiDungDanhGiaNV: Array<any> = new Array<any>();
  listNoiDungDanhGiaQL: Array<any> = new Array<any>();

  today = new Date();
  clonedData: { [s: string]: any; } = {}

  //Form của phiếu đánh giá
  phieuDanhGiaFormGroup: FormGroup;
  tenPhieuDanhGiaFormControl: FormControl;
  ngayTaoFormControl: FormControl;
  nguoiTaoFormControl: FormControl;
  hoatDongControl: FormControl;
  thangDiemDanhGiaControl: FormControl;

  cachTinhFormGroup: FormGroup;
  cachTinhControl: FormControl;

  editing: boolean = false;

  colsFile: any[];

  phieuDanhGiaId: any;

  listTinhTheo = [
    { value: 1, name: "Tổng điểm thành phần*Trọng số" },
    { value: 2, name: "Trung bình cộng điểm thành phần*Trọng số" },
  ];

  listMucDanhGia = [];


  viewNote: boolean = true;
  viewTimeline: boolean = true;
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionDownLoad: boolean = true;
  actionUpLoad: boolean = true;
  pageSize = 20;

  listDangCauTraLoi = [];
  listDangCauTraLoiQl = [];
  listItemCauTraLoi = [];


  trangThaiPhieuDanhGia: number = 0; //0: Mới, 1: Có hiệu lực, 2: Hết hiệu lực

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    private def: ChangeDetectorRef,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private ref: ChangeDetectorRef,
    private encrDecrService: EncrDecrService,
    private getPermission: GetPermission,
    private employeeListService: EmployeeListService,
  ) { }

  async ngOnInit() {
    this.setForm();
    let resource = "hrm/employee/chi-tiet-phieu-danh-gia/";
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

      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
    }
    this.route.params.subscribe(params => {
      this.phieuDanhGiaId = Number(this.encrDecrService.get(params['phieuDanhGiaId']));
    });
    this.getMasterData();
  }


  setForm() {
    this.tenPhieuDanhGiaFormControl = new FormControl(null, [Validators.required]);
    this.ngayTaoFormControl = new FormControl(null, [Validators.required]);
    this.nguoiTaoFormControl = new FormControl(null, [Validators.required]);
    this.hoatDongControl = new FormControl(null);
    this.thangDiemDanhGiaControl = new FormControl(null, [Validators.required]);

    this.phieuDanhGiaFormGroup = new FormGroup({
      tenPhieuDanhGiaFormControl: this.tenPhieuDanhGiaFormControl,
      ngayTaoFormControl: this.ngayTaoFormControl,
      nguoiTaoFormControl: this.nguoiTaoFormControl,
      hoatDongControl: this.hoatDongControl,
      thangDiemDanhGiaControl: this.thangDiemDanhGiaControl,
    });

    this.thangDiemDanhGiaControl.setValue(this.listMucDanhGia[0]);

    this.cachTinhControl = new FormControl(null, [Validators.required]);

    this.cachTinhFormGroup = new FormGroup({
      cachTinhControl: this.cachTinhControl
    });
  }

  setCols() {
    this.colsBangDanhGia = [
      { field: 'stt', header: 'Stt', width: "20px", textAlign: 'left', color: '#f44336' },
      { field: 'noiDungCauHoi', header: 'Nội dung đánh giá', textAlign: 'left', display: 'table-cell', width: "125px" },
      { field: 'tiLe', header: 'Tỉ lệ (%)', textAlign: 'left', width: "80px" },
      { field: 'cauTraLoi', header: 'Câu trả lời', textAlign: 'left', display: 'table-cell', width: '200px' },
      { field: 'action', header: 'Thao tác', width: '60px', textAlign: 'center', color: '#f44336' },
    ];

    this.colsBangDanhGiaQL = this.colsBangDanhGia.filter(x => x.field != "tiLe");

    //hoàn thành thì ẩn cột thao tác của bảng cấu hình câu hỏi
    if (this.trangThaiPhieuDanhGia == 1) {
      this.colsBangDanhGia = this.colsBangDanhGia.filter(x => x.field != 'action');
      this.colsBangDanhGiaQL = this.colsBangDanhGiaQL.filter(x => x.field != 'action');
    }

    this.colsFile = [
      { field: 'fileName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'right', type: 'number' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'right', type: 'date' },
      { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'left', type: 'string' },
    ];


  }
  async getMasterData() {
    this.loading = true;
    let result: any = await this.employeeService.phieuDanhGiaDetail(this.phieuDanhGiaId);
    this.loading = false;
    console.log(result)
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Lấy thông tin không thành công' };
      this.showMessage(msg);
      return;
    }
    this.listDangCauTraLoi = result.listDangCauTraLoi;
    this.listDangCauTraLoiQl = result.listDangCauTraLoi.filter(x => x.value != 1 && x.value != 3);

    this.listEmpAdd = result.nguoiTaoModel;
    this.listItemCauTraLoi = result.listItemCauTraLoi;
    this.listMucDanhGia = result.listThangDiemDanhGia;
    this.arrayDocumentModel = result.listFileInFolder;

    this.setDefaultValue(result);
    this.setCols();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  thoat() {
    this.router.navigate(['/employee/danh-sach-de-xuat-chuc-vu']);
  }



  setDefaultValue(result) {
    //Map data phiếu
    this.tenPhieuDanhGiaFormControl.setValue(result.phieuDanhGia.tenPhieuDanhGia);
    this.ngayTaoFormControl.setValue(new Date(result.phieuDanhGia.createdDate));
    this.ngayTaoFormControl.disable();
    this.nguoiTaoFormControl.setValue(result.nguoiTao.employeeName);
    this.nguoiTaoFormControl.disable();
    this.hoatDongControl.setValue(true);
    this.thangDiemDanhGiaControl.setValue(this.listMucDanhGia.find(x => x.mucDanhGiaId == result.phieuDanhGia.thangDiemDanhGiaId));
    this.cachTinhControl.setValue(this.listTinhTheo.find(x => x.value == result.phieuDanhGia.cachTinhTong));

    this.trangThaiPhieuDanhGia = result.phieuDanhGia.trangThaiPhieuDanhGia;
    if (this.trangThaiPhieuDanhGia == 0) this.hoatDongControl.enable();
    if (this.trangThaiPhieuDanhGia != 0) {
      this.hoatDongControl.enable();
      if (result.phieuDanhGia.trangThaiPhieuDanhGia == 1) this.hoatDongControl.setValue(true); // có hiệu lựuc
      if (result.phieuDanhGia.trangThaiPhieuDanhGia == 2) this.hoatDongControl.setValue(false); // Hết hiệu lực
    }
    //Map câu hỏi
    this.setCauHoi(result, 1); //Nhân viên
    this.setCauHoi(result, 2); //Quản lý

    this.sapXepLaiDanhSachNoiDung('NV');
    this.sapXepLaiDanhSachNoiDung('QL');
  }




  setCauHoi(result, nguoiDanhGia) {
    let listNoiDungTTNew: Array<any> = new Array<any>();
    let indexParent = 1;
    result.listCauHoiMapping.filter(x => x.nguoiDanhGia == nguoiDanhGia && x.parentId == null)?.forEach(parent => {
      let cauHoiCha: CauHoiPhieuDanhGiaMapping = {
        cauHoiPhieuDanhGiaMappingId: indexParent,
        parentId: 0,
        tiLe: parent.tiLe,
        stt: parent.stt,
        noiDungCauHoi: parent.noiDungCauHoi,
        nguoiDanhGia: nguoiDanhGia,
        cauTraLoi: null,
        danhSachItem: null,
        isValid: true,
      }

      let oldIndexParent = parent.cauHoiPhieuDanhGiaMappingId;
      parent.cauHoiPhieuDanhGiaMappingId = indexParent;
      listNoiDungTTNew.push(cauHoiCha);
      let indexCon = 0.1; // đánh stt tăng dần cho index con

      result.listCauHoiMapping.filter(x => x.nguoiDanhGia == nguoiDanhGia && x.parentId == oldIndexParent)?.forEach(child => {
        let cauHoiCon: CauHoiPhieuDanhGiaMapping = {
          cauHoiPhieuDanhGiaMappingId: indexParent + indexCon,
          parentId: parent.cauHoiPhieuDanhGiaMappingId,
          tiLe: null,
          stt: parent.stt,
          noiDungCauHoi: child.noiDungCauHoi,
          nguoiDanhGia: nguoiDanhGia,
          cauTraLoi: this.listDangCauTraLoi.find(x => x.value == child.loaiCauTraLoiId),
          danhSachItem: child.danhMucItem,
          isValid: true,
        }
        indexCon += 0.1;
        listNoiDungTTNew.push(cauHoiCon);
      });
      indexParent++;
    });
    if (nguoiDanhGia == 1) this.listNoiDungDanhGiaNV = listNoiDungTTNew;
    else if (nguoiDanhGia == 2) this.listNoiDungDanhGiaQL = listNoiDungTTNew;
    this.ref.detectChanges();
  }


  async UpdateStatusPhieuDanhGia(number) {
    let trangThai = 0;
    if (number == 0) {//hoàn thành ( mới )
      trangThai = 0;
    }
    if (number == 1) {// có hiệu lực or hết
      trangThai = this.hoatDongControl.value == true ? 1 : 2
    }
    //Cập nhật hiệu lực phiếu đánh giá
    let result: any = await this.employeeService.hoanThanhOrUpdateStatusPhieuDanhGia(this.phieuDanhGiaId, trangThai, number);
    this.loading = false;
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Cập nhật thông tin không thành công' };
      this.showMessage(msg);
      return;
    }
    this.getMasterData();
  }

  async hoanThanhPhieuDanhGia() {
    if (!this.phieuDanhGiaFormGroup.valid) {
      Object.keys(this.phieuDanhGiaFormGroup.controls).forEach(key => {
        if (!this.phieuDanhGiaFormGroup.controls[key].valid) {
          this.phieuDanhGiaFormGroup.controls[key].markAsTouched();
        }
      });
      return false;
    }

    if (!this.cachTinhFormGroup.valid) {
      Object.keys(this.cachTinhFormGroup.controls).forEach(key => {
        if (!this.cachTinhFormGroup.controls[key].valid) {
          this.cachTinhFormGroup.controls[key].markAsTouched();
        }
      });
      return false;
    }

    let phieuDanhGia = new PhieuDanhGia();
    phieuDanhGia.PhieuDanhGiaId = this.phieuDanhGiaId;
    phieuDanhGia.TenPhieuDanhGia = this.tenPhieuDanhGiaFormControl.value;
    phieuDanhGia.TrangThaiPhieuDanhGia = 0; // cập nhật thì trạng thái luôn là 0: Mới
    phieuDanhGia.ThangDiemDanhGiaId = this.thangDiemDanhGiaControl.value.mucDanhGiaId;
    phieuDanhGia.CachTinhTong = this.cachTinhControl.value.value;

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
      fileUpload.FileInFolder.objectType = 'PHIEUDANHGIA';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    if (!(this.listNoiDungDanhGiaNV != null && this.listNoiDungDanhGiaNV.length > 0)) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Nhập thông tin câu hỏi tự đánh giá nhân viên" };
      this.showMessage(msg);
      return false;
    }



    let check = this.checkListCauHoi();
    if (!check) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Vui lòng nhập đầy đủ thông tin" };
      this.showMessage(msg);
      return false;
    }


    // Kiểm tra tổng tỉ lệ phần trăm
    let percent: number = 0;
    this.listNoiDungDanhGiaNV.filter(x => x.parentId == 0 || x.parentId == null)?.forEach(parent => {
      percent += parseFloat(parent.tiLe);
    });
    if (percent != 100) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Tổng tỷ lệ nội dung đánh giá phải bằng 100%" };
      this.showMessage(msg);
      return false;
    }

    this.employeeService.capNhatPhieuDanhGia(phieuDanhGia, this.listNoiDungDanhGiaNV, this.listNoiDungDanhGiaQL, listFileUploadModel, "PHIEUDANHGIA").subscribe(async response => {
      this.loading = false;
      let result = <any>response;
      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: result.message };
        this.showMessage(msg);
        await this.UpdateStatusPhieuDanhGia(0);
        await this.getMasterData();
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
        this.showMessage(msg);
        return false;
      }
    });
    return true;
  }



  nhanBanPhieuDanhGia() {
    if (!this.phieuDanhGiaFormGroup.valid) {
      Object.keys(this.phieuDanhGiaFormGroup.controls).forEach(key => {
        if (!this.phieuDanhGiaFormGroup.controls[key].valid) {
          this.phieuDanhGiaFormGroup.controls[key].markAsTouched();
        }
      });
      return;
    }

    if (!this.cachTinhFormGroup.valid) {
      Object.keys(this.cachTinhFormGroup.controls).forEach(key => {
        if (!this.cachTinhFormGroup.controls[key].valid) {
          this.cachTinhFormGroup.controls[key].markAsTouched();
        }
      });
      return;
    }

    let phieuDanhGia = new PhieuDanhGia();
    phieuDanhGia.TenPhieuDanhGia = this.tenPhieuDanhGiaFormControl.value;
    phieuDanhGia.ThangDiemDanhGiaId = this.thangDiemDanhGiaControl.value.mucDanhGiaId;
    phieuDanhGia.CachTinhTong = this.cachTinhControl.value.value;

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
      fileUpload.FileInFolder.objectType = 'PHIEUDANHGIA';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    if (!(this.listNoiDungDanhGiaNV != null && this.listNoiDungDanhGiaNV.length > 0)) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Nhập thông tin câu hỏi tự đánh giá nhân viên" };
      this.showMessage(msg);
      return;
    }


    let check = this.checkListCauHoi();
    if (!check) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Vui lòng nhập đầy đủ thông tin" };
      this.showMessage(msg);
      return;
    }


    this.employeeService.taoPhieuDanhGia(phieuDanhGia, this.listNoiDungDanhGiaNV, this.listNoiDungDanhGiaQL, listFileUploadModel, "PHIEUDANHGIA").subscribe(response => {
      this.loading = false;
      let result = <any>response;
      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Nhân bản phiếu đánh giá thành công!' };
        this.showMessage(msg);


        this.phieuDanhGiaId = result.phieuDanhGiaId;
        this.getMasterData();
        setTimeout(() => {
          this.router.navigate(['/employee/chi-tiet-phieu-danh-gia', { phieuDanhGiaId: this.encrDecrService.set(result.phieuDanhGiaId) }]);
        }, 500)

      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
        this.showMessage(msg);
      }
    });

  }


  async capNhatPhieuDanhGia() {
    if (!this.phieuDanhGiaFormGroup.valid) {
      Object.keys(this.phieuDanhGiaFormGroup.controls).forEach(key => {
        if (!this.phieuDanhGiaFormGroup.controls[key].valid) {
          this.phieuDanhGiaFormGroup.controls[key].markAsTouched();
        }
      });
      return false;
    }

    if (!this.cachTinhFormGroup.valid) {
      Object.keys(this.cachTinhFormGroup.controls).forEach(key => {
        if (!this.cachTinhFormGroup.controls[key].valid) {
          this.cachTinhFormGroup.controls[key].markAsTouched();
        }
      });
      return false;
    }

    let phieuDanhGia = new PhieuDanhGia();
    phieuDanhGia.PhieuDanhGiaId = this.phieuDanhGiaId;
    phieuDanhGia.TenPhieuDanhGia = this.tenPhieuDanhGiaFormControl.value;
    phieuDanhGia.TrangThaiPhieuDanhGia = 0; // cập nhật thì trạng thái luôn là 0: Mới
    phieuDanhGia.ThangDiemDanhGiaId = this.thangDiemDanhGiaControl.value.mucDanhGiaId;
    phieuDanhGia.CachTinhTong = this.cachTinhControl.value.value;

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
      fileUpload.FileInFolder.objectType = 'PHIEUDANHGIA';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    if (!(this.listNoiDungDanhGiaNV != null && this.listNoiDungDanhGiaNV.length > 0)) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Nhập thông tin câu hỏi tự đánh giá nhân viên" };
      this.showMessage(msg);
      return false;
    }



    let check = this.checkListCauHoi();
    if (!check) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Vui lòng nhập đầy đủ thông tin" };
      this.showMessage(msg);
      return false;
    }


    // Kiểm tra tổng tỉ lệ phần trăm
    let percent: number = 0;
    this.listNoiDungDanhGiaNV.filter(x => x.parentId == 0 || x.parentId == null)?.forEach(parent => {
      percent += parseFloat(parent.tiLe);
    });
    if (percent != 100) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Tổng tỷ lệ nội dung đánh giá phải bằng 100%" };
      this.showMessage(msg);
      return false;
    }

    this.employeeService.capNhatPhieuDanhGia(phieuDanhGia, this.listNoiDungDanhGiaNV, this.listNoiDungDanhGiaQL, listFileUploadModel, "PHIEUDANHGIA").subscribe(async response => {
      this.loading = false;
      let result = <any>response;
      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: result.message };
        this.showMessage(msg);
        await this.getMasterData();
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
        this.showMessage(msg);
        return false;
      }
    });
    return true;
  }

  checkListCauHoi() {
    //Check list câu hỏi nhân viên
    let check = false;
    let indexParent = 1;
    this.listNoiDungDanhGiaNV.filter(x => x.parentId == 0)?.forEach(parent => {
      let oldIndexParent = parent.cauHoiPhieuDanhGiaMappingId;
      parent.cauHoiPhieuDanhGiaMappingId = indexParent;
      let indexCon = 0; // đánh stt tăng dần cho index con
      if (parent.noiDungCauHoi == null || parent.noiDungCauHoi.trim() == ''
        || parent.tiLe > 100
      ) {
        // parent.isValid = false;
        check = true;
      }
      if (!this.listNoiDungDanhGiaNV.find(x => x.parentId == oldIndexParent)) {
        check = true;
      }

      this.listNoiDungDanhGiaNV.filter(x => x.parentId == oldIndexParent)?.forEach(child => {

        child.cauHoiPhieuDanhGiaMappingId = indexParent * 1000 + indexCon;
        child.parentId = parent.cauHoiPhieuDanhGiaMappingId;
        if (
          child.noiDungCauHoi == null || child.noiDungCauHoi.trim() == ''
          || child.cauTraLoi == null
        ) {
          // parent.isValid = false;
          check = true;

        }
        if (child.cauTraLoi != null) {
          if (child.cauTraLoi.value == 2 || child.cauTraLoi.value == 0) {
            if (child.danhSachItem == null || child.danhSachItem.length == 0) {
              check = true;
            }
          }
        }

        indexCon++
      });

      indexParent++;
    });

    //Check list câu hỏi quản lý
    let indexParentQL = 1;
    if (this.listNoiDungDanhGiaQL.length > 0) {
      this.listNoiDungDanhGiaQL.filter(x => x.parentId == 0)?.forEach(parent => {
        let oldIndexParent = parent.cauHoiPhieuDanhGiaMappingId;
        parent.cauHoiPhieuDanhGiaMappingId = indexParentQL;
        let indexCon = 0.1; // đánh stt tăng dần cho index con
        if (!this.listNoiDungDanhGiaQL.find(x => x.parentId == oldIndexParent)) {
          check = true;
        }

        this.listNoiDungDanhGiaQL.filter(x => x.parentId == oldIndexParent)?.forEach(child => {
          child.cauHoiPhieuDanhGiaMappingId = indexParentQL + indexCon;
          child.parentId = parent.cauHoiPhieuDanhGiaMappingId;
          if (
            child.noiDungCauHoi == null || child.noiDungCauHoi.trim() == ''
            || child.cauTraLoi == null
          ) {
            // parent.isValid = false;
            check = true;
          }
          if (child.cauTraLoi != null) {
            if (child.cauTraLoi.value == 2 || child.cauTraLoi.value == 0) {
              if (child.danhSachItem == null || child.danhSachItem.length == 0) check = true;
            }
          }

          indexCon += 0.1;
        });

        indexParentQL++;
      });
    }




    if (check) return false;

    return true;

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

  // Thêm dòng con thuộc nội dung cha
  addRowChild(rowData: any, type) {
    let noiDUngTT: CauHoiPhieuDanhGiaMapping = {
      cauHoiPhieuDanhGiaMappingId: 0,
      noiDungCauHoi: '',
      tiLe: 0,
      cauTraLoi: null,
      danhSachItem: null,
      isValid: true,
      parentId: rowData.cauHoiPhieuDanhGiaMappingId,
      stt: 0,
      nguoiDanhGia: type == 'NV' ? 1 : 2,
    }
    if (type == 'NV') this.listNoiDungDanhGiaNV.push(noiDUngTT);
    if (type == 'QL') this.listNoiDungDanhGiaQL.push(noiDUngTT);
    this.sapXepLaiDanhSachNoiDung(type);
  }


  // Thêm nội dung cha
  addRow(type) {
    let noiDUngTT: CauHoiPhieuDanhGiaMapping = {
      cauHoiPhieuDanhGiaMappingId: type == 'NV' ? this.listNoiDungDanhGiaNV.filter(x => x.parentId == 0).length + 1 : this.listNoiDungDanhGiaQL.filter(x => x.parentId == 0).length + 1,
      noiDungCauHoi: '',
      tiLe: 0,
      cauTraLoi: null,
      isValid: true,
      danhSachItem: null,
      parentId: 0,
      stt: 0,
      nguoiDanhGia: type == 'NV' ? 1 : 2,
    }
    if (type == 'NV') this.listNoiDungDanhGiaNV.push(noiDUngTT);
    if (type == 'QL') this.listNoiDungDanhGiaQL.push(noiDUngTT);
    this.sapXepLaiDanhSachNoiDung(type);
    this.ref.detectChanges();
  }

  // Sắp xếp lại vị trí thứ tự của list nội dung
  sapXepLaiDanhSachNoiDung(type) {
    if (type == 'NV') {
      let listNoiDungTTNew: Array<any> = new Array<any>();
      let indexParent = 1;
      this.listNoiDungDanhGiaNV.filter(x => x.parentId == 0)?.forEach(parent => {
        let oldIndexParent = parent.cauHoiPhieuDanhGiaMappingId;
        parent.cauHoiPhieuDanhGiaMappingId = indexParent;
        parent.stt = indexParent;
        listNoiDungTTNew.push(parent);
        let indexCon = 0.1; // đánh stt tăng dần cho index con
        this.listNoiDungDanhGiaNV.filter(x => x.parentId == oldIndexParent)?.forEach(child => {
          child.cauHoiPhieuDanhGiaMappingId = indexParent + indexCon;
          child.stt = indexParent + indexCon;
          child.parentId = parent.cauHoiPhieuDanhGiaMappingId;
          indexCon += 0.1;
          listNoiDungTTNew.push(child);
        });
        indexParent++;
      });
      this.listNoiDungDanhGiaNV = listNoiDungTTNew;
      this.ref.detectChanges();
    }
    if (type == 'QL') {
      let listNoiDungTTNew: Array<any> = new Array<any>();
      let indexParent = 1;
      this.listNoiDungDanhGiaQL.filter(x => x.parentId == 0)?.forEach(parent => {
        let oldIndexParent = parent.cauHoiPhieuDanhGiaMappingId;
        parent.cauHoiPhieuDanhGiaMappingId = indexParent;
        parent.stt = indexParent;
        listNoiDungTTNew.push(parent);

        let indexCon = 0.1; // đánh stt tăng dần cho index con
        this.listNoiDungDanhGiaQL.filter(x => x.parentId == oldIndexParent)?.forEach(child => {
          child.cauHoiPhieuDanhGiaMappingId = indexParent + indexCon;
          child.stt = indexParent + indexCon;
          child.parentId = parent.cauHoiPhieuDanhGiaMappingId;
          indexCon += 0.1;
          listNoiDungTTNew.push(child);
        });
        indexParent++;
      });
      this.listNoiDungDanhGiaQL = listNoiDungTTNew;
      this.ref.detectChanges();
    }

  }


  onRowEditInit(rowData: any) {
    this.clonedData[rowData.cauHoiPhieuDanhGiaMappingId] = { ...rowData };
    this.ref.detectChanges();
  }

  async onRowRemove(rowData: any, type) {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn xóa dòng này?`,
      accept: async () => {
        if (type == 'NV') this.listNoiDungDanhGiaNV = this.listNoiDungDanhGiaNV.filter(e => e != rowData);
        if (type == 'QL') this.listNoiDungDanhGiaQL = this.listNoiDungDanhGiaQL.filter(e => e != rowData);

      }
    });
  }

  onRowEditSave(rowData: any) {

  }


  //#region Thao tác chung
  onRowEditCancel(rowData: any) {
    Object.keys(this.clonedData).forEach(key => {
      if (key == rowData.cauHoiPhieuDanhGiaMappingId && rowData.cauHoiPhieuDanhGiaMappingId != 0) {
        rowData.cauHoiPhieuDanhGiaMappingId = this.clonedData[key].cauHoiPhieuDanhGiaMappingId;
        rowData.noiDungDanhGia = this.clonedData[key].noiDungDanhGia;
        rowData.tiLe = this.clonedData[key].tiLe;
        rowData.cauTraLoi = this.clonedData[key].cauTraLoi;
        rowData.parentId = this.clonedData[key].parentId;
      }
    });
  }

  /** Xử lý row con */
  onRowEditSaveChild(rowData: any) {

    this.editing = !this.editing;
  }

  onRowEditCancelChild(rowData: any) {
    this.editing = !this.editing;

    Object.keys(this.clonedData).forEach(key => {
      if (key == rowData.cauHoiPhieuDanhGiaMappingId && rowData.cauHoiPhieuDanhGiaMappingId != 0) {
        rowData.cauHoiPhieuDanhGiaMappingId = this.clonedData[key].cauHoiPhieuDanhGiaMappingId;
        rowData.noiDungDanhGia = this.clonedData[key].noiDungDanhGia;
        rowData.phongBan = this.clonedData[key].phongBan;
        rowData.tiLe = this.clonedData[key].tiLe;
        rowData.cauTraLoi = this.clonedData[key].cauTraLoi;
        rowData.parentId = this.clonedData[key].parentId;
      }
      // else {
      //   this.listNoiDungTT = this.listNoiDungTT.filter(e => e != rowData);
      // }
    });
  }

  async onRowRemoveChild(rowData: any, type) {

    this.confirmationService.confirm({
      message: `Bạn có chắc chắn xóa dòng này?`,
      accept: async () => {
        if (type == "NV") this.listNoiDungDanhGiaNV = this.listNoiDungDanhGiaNV.filter(e => e != rowData);
        if (type == "QL") this.listNoiDungDanhGiaQL = this.listNoiDungDanhGiaQL.filter(e => e != rowData);
        this.sapXepLaiDanhSachNoiDung(type);
      }
    });
  }

  onRowEditInitChild(rowData: any) {

    this.editing = !this.editing;

    this.clonedData[rowData.cauHoiPhieuDanhGiaMappingId] = { ...rowData };
    this.ref.detectChanges();
  }



  onRowEditCancelParent(rowData: any) {

    this.editing = !this.editing;

    Object.keys(this.clonedData).forEach(key => {
      if (key == rowData.cauHoiPhieuDanhGiaMappingId && rowData.cauHoiPhieuDanhGiaMappingId != 0) {
        rowData.cauHoiPhieuDanhGiaMappingId = this.clonedData[key].cauHoiPhieuDanhGiaMappingId;
        rowData.noiDungDanhGia = this.clonedData[key].noiDungDanhGia;
        rowData.tiLe = this.clonedData[key].tiLe;
        rowData.cauTraLoi = this.clonedData[key].cauTraLoi;
        rowData.parentId = this.clonedData[key].parentId;
      }
      // else {
      //   this.listNoiDungTT = this.listNoiDungTT.filter(e => e != rowData);
      // }
    });
  }



  /**Xử lý row cha */
  onRowEditSaveParent(rowData: any, event) {

    // if (
    //   !rowData.cauHoiPhieuDanhGiaMappingId || rowData.cauHoiPhieuDanhGiaMappingId == '' || rowData.cauHoiPhieuDanhGiaMappingId == 0 ||
    //   !rowData.tiLe || rowData.tiLe == '' || rowData.tiLe == 0 ||
    //   !rowData.noiDungDanhGia || rowData.noiDungDanhGia == '' || rowData.noiDungDanhGia == 0 ||
    //   !rowData.danhSachItem || rowData.danhSachItem == '' || rowData.danhSachItem == 0 ||
    //   !rowData.cauTraLoi || rowData.cauTraLoi == '' || rowData.cauTraLoi == 0 ||
    //   !rowData.cauTraLoiId || rowData.cauTraLoiId == '' || rowData.cauTraLoiId == 0 ||
    //   !rowData.parentId || rowData.parentId == '' || rowData.parentId == 0
    //   ) {

    //   let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Hãy nhập đầy đủ thông tin!' };
    //   this.showMessage(msg);
    //   event.preventDefault();
    //   return;
    // }
  }


  async onRowRemoveParent(rowData: any, type) {
    if (type == "NV") {
      // Kiểm tra xem có row con
      if (this.listNoiDungDanhGiaNV.filter(x => x.parentId == rowData.cauHoiPhieuDanhGiaMappingId).length > 0) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Tồn tại nội dung đánh giá con. Vui lòng kiểm tra lại' };
        this.showMessage(msg);
        return;
      }
    }

    if (type == "QL") {
      // Kiểm tra xem có row con
      if (this.listNoiDungDanhGiaQL.filter(x => x.parentId == rowData.cauHoiPhieuDanhGiaMappingId).length > 0) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Tồn tại nội dung đánh giá con. Vui lòng kiểm tra lại' };
        this.showMessage(msg);
        return;
      }
    }

    this.confirmationService.confirm({
      message: `Bạn có chắc chắn xóa dòng này?`,
      accept: async () => {
        if (type == "NV") this.listNoiDungDanhGiaNV = this.listNoiDungDanhGiaNV.filter(e => e != rowData);
        if (type == "QL") this.listNoiDungDanhGiaQL = this.listNoiDungDanhGiaQL.filter(e => e != rowData);
      }
    });
  }


  onRowEditInitParent(rowData: any) {

    this.editing = !this.editing;

    this.clonedData[rowData.cauHoiPhieuDanhGiaMappingId] = { ...rowData };
    this.ref.detectChanges();
  }


  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }


}


function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
