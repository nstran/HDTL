import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { GetPermission } from '../../../../../../app/shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { Paginator, Table } from 'primeng';
import { QuanLyCongTacService } from '../../../../services/quan-ly-cong-tac/quan-ly-cong-tac.service';
import { DeNghiTamHoanUngModel } from '../../../../../../app/employee/models/de-nghi-tam-hoan-ung';
import { CategoryModel } from '../../../../../../app/shared/models/category.model';
import { TreeNode } from 'primeng/api';
import { EncrDecrService } from '../../../../../shared/services/encrDecr.service';

export class EmployeeModel {
  employeeId: string;
  bankOwnerName: string; //Tên TK
  bankAccount: string;  // Số TK
  bankName: string;     // Ngân hàng
  bankCode: string; // Mã Ngân hàng
  bmployeeName: string;
  organizationName: string;
  constructor() { }
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

class NoiDungTamUngModel {
  id: number;
  deNghiTamHoanUngChiTietId: number;
  parentId: number;
  noiDung: string;
  tongTienTruocVat: number;
  vat: number;
  tienSauVat: number;
  level: number;

  constructor() {
    this.id = null,
      this.parentId = null;
    this.deNghiTamHoanUngChiTietId = null;
    this.noiDung = null;
    this.tongTienTruocVat = 0;
    this.vat = 0;
    this.tienSauVat = 0;
    this.level = 0;
  }
}

class NoiDungHoanUngModel {
  id: number;
  ngayThang: Date;
  soHoaDon: string;
  noiDung: string;
  hinhThucThanhToan: string;
  vanChuyenXm: any;
  tienDonHnnb: any;
  tienDonDn: any;
  khachSan: any;
  chiPhiKhac: any;
  tongTienTruocVat: any;
  vat: any;
  tienSauVat: any;
  dinhKemCt: string;
  ghiChu: string;
  parentId: number;
  level: number;

  selectedHinhThucThanhToan: any;

  constructor() {
    this.id = 0;
    this.vanChuyenXm = 0;
    this.tienDonHnnb = 0;
    this.tienDonDn = 0;
    this.khachSan = 0;
    this.chiPhiKhac = 0;
    this.tongTienTruocVat = 0;
    this.vat = 0;
    this.tienSauVat = 0;
    this.selectedHinhThucThanhToan = null;
    this.level = 0;
  }
}

@Component({
  selector: 'app-tao-de-nghi-tam-hoan-ung',
  templateUrl: './tao-de-nghi-tam-hoan-ung.component.html',
  styleUrls: ['./tao-de-nghi-tam-hoan-ung.component.css']
})
export class TaoDeNghiTamHoanUngComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;
  today: Date = new Date();
  @ViewChild('fileUpload') fileUpload: FileUpload;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  employeeCodeName: string = localStorage.getItem('EmployeeCodeName');
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  actionAdd: boolean = true;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';

  // Dữ liệu masterdata
  listHoSoCT: Array<any> = new Array<any>();
  listEmployee: Array<EmployeeModel> = new Array<EmployeeModel>();
  listHinhThucTT: Array<CategoryModel> = new Array<CategoryModel>();

  colsList: any[];
  selectedColumns: any[];
  uploadedFiles: any[] = [];
  colsFile: any[];
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  employeeModel: EmployeeModel = new EmployeeModel();

  // Thông tin thanh toán
  deNghiThanhToanForm: FormGroup;
  ngayDeNghiControl: FormControl;
  theoHoSoCTControl: FormControl;
  lyDoControl: FormControl;
  nhanVienThuHuongControl: FormControl;

  tongTien: number = 0;
  tienTamUng: number = 0;
  tongTienConLaiCTT: number = 0;

  listNoiDungTT: Array<any> = new Array<any>();

  clonedData: { [s: string]: any; } = {}
  editing: boolean = false;
  hoSoCongTacId: number = 0;
  loaiDeNghi: number = 0;

  listData: Array<TreeNode> = [];

  constructor(
    private quanLyCongtacService: QuanLyCongTacService,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private getPermission: GetPermission,
    private el: ElementRef,
    private ref: ChangeDetectorRef,
    private encrDecrService: EncrDecrService,
  ) {

  }

  async ngOnInit() {
    this.setForm();
    let resource = "hrm/employee/tao-de-nghi-tam-hoan-ung";
    let permission: any = await this.getPermission.getPermission(resource);

    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }
      this.route.params.subscribe(params => {
        if (params['hoSoCongTacId']) this.hoSoCongTacId = Number(this.encrDecrService.get(params['hoSoCongTacId']));
        this.loaiDeNghi = params['loaiDeNghi'];
      });

      this.setTable();
      this.getMasterData();
    }
  }

  setForm() {
    this.ngayDeNghiControl = new FormControl(new Date, [Validators.required]);
    this.theoHoSoCTControl = new FormControl(null, [Validators.required]);
    this.lyDoControl = new FormControl(null, [Validators.required]);
    this.nhanVienThuHuongControl = new FormControl(null, [Validators.required]);

    this.deNghiThanhToanForm = new FormGroup({
      ngayDeNghiControl: this.ngayDeNghiControl,
      theoHoSoCTControl: this.theoHoSoCTControl,
      lyDoControl: this.lyDoControl,
      nhanVienThuHuongControl: this.nhanVienThuHuongControl,
    });

    this.ngayDeNghiControl.setValue(this.today);
  }

  async getMasterData() {
    this.loading = true;
    // Check nếu đi từ màn hình Hồ sơ công tác thì danh sách nhân viên thụ hưởng sẽ theo hồ sơ công tác
    let result: any = await this.quanLyCongtacService.getMasterDataDeNghiForm();
    this.loading = false;
    if (result.statusCode == 200) {
      this.listHoSoCT = result.listHoSoCongTac;
      this.listHinhThucTT = result.listHinhThucTT;

      if (this.hoSoCongTacId != 0 && this.hoSoCongTacId) {
        let hoSo = this.listHoSoCT.find(x => x.hoSoCongTacId == this.hoSoCongTacId);

        if (hoSo != null && hoSo != undefined) {
          this.theoHoSoCTControl.setValue(hoSo);
          this.thayDoiHoSoCT(hoSo);
        }
      }
      this.ref.detectChanges();
    }
  }

  setTable() {
    this.colsList = [
      { field: 'index', header: 'STT', width: '50px', textAlign: 'center', color: '#f44336' },
      { field: 'ngayThang', header: 'Ngày', width: '200px', textAlign: 'center', color: '#f44336' },
      { field: 'soHoaDon', header: 'Số hóa đơn/số phiếu', width: '120px', textAlign: 'left', color: '#f44336' },
      { field: 'noiDung', header: 'Tóm tắt mục đích chi', width: '200px', textAlign: 'left', color: '#f44336' },
      { field: 'maPhongBan', header: 'Mã phòng ban', width: '120px', textAlign: 'center', color: '#f44336' },
      { field: 'hinhThucThanhToan', header: 'Hình thức thanh toán', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'vanChuyenXm', header: 'Vận chuyển xe máy', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'tienDonHnnb', header: 'Tiễn đón HN-NB', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'tienDonDn', header: 'Tiễn đón ĐN', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'khachSan', header: 'Khách sạn', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'chiPhiKhac', header: 'Chi phí khác', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'tongTienTruocVat', header: 'Số tiền trước VAT', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'vat', header: 'VAT', width: '80px', textAlign: 'right', color: '#f44336' },
      { field: 'tienSauVat', header: 'Số tiền sau VAT', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'dinhKemCt', header: 'Đính kèm chứng từ', width: '120px', textAlign: 'left', color: '#f44336' },
      { field: 'ghiChu', header: 'Ghi chú', width: '120px', textAlign: 'left', color: '#f44336' },
      { field: 'action', header: 'Thao tác', width: '100px', textAlign: 'center', color: '#f44336' },
    ];

    // Tạm ứng
    if (this.loaiDeNghi == 0)
      this.selectedColumns = [
        { field: 'noiDung', header: 'Tóm tắt mục đích chi', width: '40%', textAlign: 'left', color: '#f44336' },
        { field: 'maPhongBan', header: 'Mã phòng ban', width: '15%', textAlign: 'center', color: '#f44336' },
        { field: 'tongTienTruocVat', header: 'Số tiền trước VAT', width: '15%', textAlign: 'right', color: '#f44336' },
        { field: 'vat', header: 'VAT', width: '5%', textAlign: 'right', color: '#f44336' },
        { field: 'tienSauVat', header: 'Số tiền sau VAT', width: '15%', textAlign: 'right', color: '#f44336' },
        { field: 'action', header: 'Thao tác', width: '10%', textAlign: 'center', color: '#f44336' },
      ];
    // Hoàn ứng
    else
      this.selectedColumns = this.colsList.filter(e =>
        e.field == "index" || e.field == "ngayThang" || e.field == "soHoaDon" || e.field == "maPhongBan"
        || e.field == "noiDung" || e.field == "hinhThucThanhToan"
        || e.field == "vanChuyenXm" || e.field == "tienDonHnnb" || e.field == "tienDonDn"
        || e.field == "khachSan" || e.field == "chiPhiKhac" || e.field == "tongTienTruocVat"
        || e.field == "vat" || e.field == "tienSauVat" || e.field == "dinhKemCt"
        || e.field == "ghiChu" || e.field == "action"
      );
  }

  thayDoiHoSoCT(event: any) {
    this.nhanVienThuHuongControl.setValue(null);
    this.employeeModel = new EmployeeModel();
    if (event) {
      this.listEmployee = event.listNhanVienCT;
      this.ref.detectChanges();
    }
  }

  thayDoiNhanVienThuHuong(event: any) {
    if (event.value) {
      this.nhanVienThuHuongControl.setValue(event.value)
      this.mappingModelToFrom(event.value);
      this.ref.detectChanges();
    }
  }

  mappingModelToFrom(model: any) {
    this.employeeModel.bankName = model.bankAddress;
    this.employeeModel.bankOwnerName = model.bankOwnerName;
    this.employeeModel.organizationName = model.organizationName;
    this.employeeModel.bankAccount = model.bankAccount;
    this.employeeModel.bankCode = model.bankName;
    this.ref.detectChanges();
  }

  // Thêm nội dung cha
  addRow() {
    //Tạm ứng
    if (this.loaiDeNghi == 0) {
      let listId = this.getListId(this.listData, []);

      let maxId = 0;
      if (listId.length > 0) maxId = Math.max(...listId.map(x => x));

      let item = new NoiDungTamUngModel();
      item.id = maxId + 1;

      this.listNoiDungTT = [...this.listNoiDungTT, item];
      this.listData = this.list_to_tree(this.listNoiDungTT);
    }
    //Hoàn ứng
    else {
      let maxId = 0;

      if (this.listNoiDungTT.length > 0) {
        maxId = Math.max(...this.listNoiDungTT.map(x => x.id))
      }

      let noiDUngTT = new NoiDungHoanUngModel();
      noiDUngTT.id = maxId + 1;

      this.listNoiDungTT = [...this.listNoiDungTT, noiDUngTT];
    }

    this.ref.detectChanges();
  }

  getListId(listData = [], listResult = []) {
    listData.forEach(item => {
      listResult.push(item.data.id);

      if (item.children.length > 0) {
        let listChildren = item.children;
        this.getListId(listChildren, listResult);
      }
    });

    return listResult;
  }

  addRowChild(rowData: any) {
    let listId = this.getListId(this.listData, []);

    let maxId = 0;
    if (listId.length > 0) maxId = Math.max(...listId.map(x => x));

    let item = new NoiDungTamUngModel();
    item.id = maxId + 1;
    item.parentId = rowData.id;
    item.level = rowData.level + 1;

    this.listNoiDungTT = [...this.listNoiDungTT, item];
    this.listData = this.list_to_tree(this.listNoiDungTT);

    this.tinhToanTongGiaTriTT();
  }

  removeRow(rowData: any) {
    let listChildrenId = this.getListChildId(rowData.id, this.listData, []);

    this.listNoiDungTT = this.listNoiDungTT.filter(x => !listChildrenId.includes(x.id));
    this.listData = this.list_to_tree(this.listNoiDungTT);

    this.tinhToanTongGiaTriTT();
  }

  getNode(id, listData = [], listResult = []) {
    let item = listData.find(x => x.data.id == id);
    if (item) {
      listResult.push(item);
    }

    if (listResult.length == 0) {
      listData.forEach(_item => {
        let listChildren = _item.children;
        this.getNode(id, listChildren, listResult);
      });
    }

    return listResult;
  }

  getListChildId(id: any, listData = [], listResult = []) {
    listResult.push(id);

    let list = this.getNode(id, listData);
    let item = list[0];

    if (item.children.length > 0) {
      let listChildren = item.children;

      listChildren.forEach(_item => {
        this.getListChildId(_item.data.id, listChildren, listResult);
      });
    }

    return listResult;
  }

  onRowEditSave(rowData: any) {
    delete this.clonedData[rowData.id];
  }

  onRowEditInit(rowData: any) {
    this.clonedData[rowData.id] = { ...rowData };
    this.ref.detectChanges();
  }

  onRowRemove(rowData: any) {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn xóa dòng này?`,
      accept: () => {
        delete this.clonedData[rowData.id];
        this.listNoiDungTT = this.listNoiDungTT.filter(e => e != rowData);
        this.tinhToanTongGiaTriTT();
      }
    });
  }

  onRowEditCancel(rowData: any, index: number) {
    this.listNoiDungTT[index] = this.clonedData[rowData.id];
    delete this.clonedData[rowData.id];
    this.tinhToanTongGiaTriTT();
  }

  tinhToanTongGiaTriTT(rowData = null) {
    this.tongTienConLaiCTT = 0;
    this.tongTien = 0;

    //Hoàn ứng
    if (this.loaiDeNghi == 1) {
      this.listNoiDungTT.forEach(item => {
        let vat = (item.vat == null || item.vat == '') ? 0 : ParseStringToFloat(item.vat);
        if (vat > 100) vat = 100;

        //Giá trị = Vận chuyển xe máy + Tiễn đón HN_NB +  Tiễn đón ĐN + Khách sạn + Chi phí khác
        let vanChuyenXm = item.vanChuyenXm == null ? 0 : ParseStringToFloat(item.vanChuyenXm.toString());
        let tienHNNB = item.tienDonHnnb == null ? 0 : ParseStringToFloat(item.tienDonHnnb.toString());
        let tienDonDn = item.tienDonDn == null ? 0 : ParseStringToFloat(item.tienDonDn.toString());
        let khachSan = item.khachSan == null ? 0 : ParseStringToFloat(item.khachSan.toString());
        let chiPhiKhac = item.chiPhiKhac == null ? 0 : ParseStringToFloat(item.chiPhiKhac.toString());

        let truocVAT = vanChuyenXm + tienHNNB + tienDonDn + khachSan + chiPhiKhac;

        item.vat = vat;
        item.vanChuyenXm = vanChuyenXm;
        item.tienDonHnnb = tienHNNB;
        item.tienDonDn = tienDonDn;
        item.khachSan = khachSan;
        item.chiPhiKhac = chiPhiKhac;
        item.tongTienTruocVat = truocVAT;
        item.tienSauVat = truocVAT * (100 + vat) / 100;

        this.tongTien += item.tienSauVat;
      });
    }
    //Tạm ứng
    else if (this.loaiDeNghi == 0) {
      //Sửa giá trị số trong grid
      if (rowData) {
        let data = this.listNoiDungTT.find(x => x.id == rowData.id);

        data.noiDung = rowData.noiDung;

        let vat = (rowData.vat == null || rowData.vat == '') ? 0 : ParseStringToFloat(rowData.vat);
        if (vat > 100) vat = 100;

        let truocVAT = rowData.tongTienTruocVat == null ? 0 : ParseStringToFloat(rowData.tongTienTruocVat.toString());

        rowData.vat = vat;
        rowData.tongTienTruocVat = truocVAT;
        rowData.tienSauVat = truocVAT * (100 + vat) / 100;

        data.vat = vat;
        data.tongTienTruocVat = truocVAT;
        data.tienSauVat = rowData.tienSauVat;
      }

      this.listNoiDungTT.forEach(item => {
        item.tienSauVat = item.tienSauVat == null ? 0 : ParseStringToFloat(item.tienSauVat.toString());

        this.tongTien += item.tienSauVat;
      });
    }

    this.tongTienConLaiCTT = this.tongTien - (this.tienTamUng == null ? 0 : ParseStringToFloat(this.tienTamUng.toString()));
    this.ref.detectChanges();
  }

  cancel() {
    if (this.hoSoCongTacId != 0 && this.hoSoCongTacId !== undefined) {
      this.router.navigate(['/employee/chi-tiet-ho-so-cong-tac', { hoSoCongTacId: this.encrDecrService.set(this.hoSoCongTacId) }]);
    }
    else {
      if (this.loaiDeNghi == 1)
        this.router.navigate(['/employee/danh-sach-de-nghi-hoan-ung']);
      else
        this.router.navigate(['/employee/danh-sach-de-nghi-tam-ung']);
    }
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

  async taoMoiDeNghi() {
    if (!this.deNghiThanhToanForm.valid) {
      Object.keys(this.deNghiThanhToanForm.controls).forEach(key => {
        if (!this.deNghiThanhToanForm.controls[key].valid) {
          this.deNghiThanhToanForm.controls[key].markAsTouched();
        }
      });
      return;
    }

    let isError = false;

    //Nếu là tạm ứng
    if (this.loaiDeNghi == 0) {
      if (this.listNoiDungTT.length == 0 || this.tongTien == 0) {
        this.showToast('warn', 'Thông báo', "Bạn cần nhập đủ thông tin thanh toán.");
        return;
      }

      this.listNoiDungTT.forEach(item => {
        let listNode = this.getNode(item.id, this.listData, []);

        item.noiDung = listNode[0].data.noiDung;
      });

      this.listNoiDungTT.forEach(item => {
        if (!item.noiDung || item.noiDung?.trim() == '') isError = true;
      });
    }

    //Nếu là hoàn ứng
    if (this.loaiDeNghi == 1) {
      if (this.listNoiDungTT.length == 0) {
        isError = true;
      }

      this.listNoiDungTT.forEach(item => {
        if (!item.ngayThang) isError = true;
        else if (!item.soHoaDon || item.soHoaDon?.trim() == '') isError = true;
        else if (!item.noiDung || item.noiDung?.trim() == '') isError = true;
        else if (!item.selectedHinhThucThanhToan) isError = true;

        let tong = item.vanChuyenXm + item.tienDonHnnb + item.tienDonDn + item.khachSan + item.chiPhiKhac;
        if (tong == 0) isError = true;

        item.hinhThucThanhToan = item.selectedHinhThucThanhToan?.categoryId;
      });
    }

    if (isError) {
      this.showToast('warn', 'Thông báo', "Bạn cần nhập đủ thông tin thanh toán.");
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
      fileUpload.FileInFolder.objectType = 'DENGHITHU';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    // Thông tin chung tạm hoàn ứng
    let ngayDeNghi = convertToUTCTime(this.ngayDeNghiControl.value);

    let deNghiTamHoanUngModel = new DeNghiTamHoanUngModel();
    deNghiTamHoanUngModel.ngayDeNghi = ngayDeNghi;
    deNghiTamHoanUngModel.lyDo = this.lyDoControl.value;
    deNghiTamHoanUngModel.hoSoCongTacId = this.theoHoSoCTControl.value.hoSoCongTacId;
    deNghiTamHoanUngModel.tienTamUng = ParseStringToFloat(this.tienTamUng.toString());
    deNghiTamHoanUngModel.tongTienThanhToan = this.tongTien;
    deNghiTamHoanUngModel.nguoiThuHuongId = this.nhanVienThuHuongControl.value.employeeId;
    deNghiTamHoanUngModel.loaiDeNghi = this.loaiDeNghi;
    // --------------

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.quanLyCongtacService.createOrUpdateDeNghiTamHoanUng(deNghiTamHoanUngModel, this.listNoiDungTT, 'DENGHITHU', listFileUploadModel, this.auth.UserId)
    this.loading = false;

    if (result.statusCode != 200) {
      this.awaitResult = false;
      this.clearToast();
      this.showToast('error', 'Thông báo', result.message);
      return;
    }

    this.showToast('success', 'Thông báo', "Tạo đề nghị thành công");

    setTimeout(() => {
      this.router.navigate(['/employee/chi-tiet-de-nghi-tam-hoan-ung', { deNghiTamHoanUngId: this.encrDecrService.set(result.deNghiTamHoanUngId) }]);
    }, 1000);
  }

  list_to_tree(listData) {
    let list: Array<TreeNode> = [];
    listData.forEach(item => {
      let node: TreeNode = {
        label: item.name,
        expanded: true,
        data: {
          'id': item.id,
          'parentId': item.parentId,
          'noiDung': item.noiDung,
          'maPhongBan': null,
          'tongTienTruocVat': item.tongTienTruocVat,
          'vat': item.vat,
          'tienSauVat': item.tienSauVat,
          'level': item.level
        },
        children: []
      };

      list = [...list, node];
    });

    var map = {}, node, roots = [], i;

    for (i = 0; i < list.length; i += 1) {
      map[list[i].data.id] = i; // initialize the map
      list[i].children = []; // initialize the children
    }

    for (i = 0; i < list.length; i += 1) {
      node = list[i];
      if (node.data.parentId !== null) {
        // if you have dangling branches check that map[node.parentId] exists
        list[map[node.data.parentId]].children.push(node);
      } else {
        roots.push(node);
      }
    }
    return roots;
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
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
