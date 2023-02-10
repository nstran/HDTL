import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { GetPermission } from '../../../../../../app/shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { QuanLyCongTacService } from '../../../../services/quan-ly-cong-tac/quan-ly-cong-tac.service';
import { DeNghiTamHoanUngModel } from '../../../../../../app/employee/models/de-nghi-tam-hoan-ung';
import { EmployeeService } from '../../../../../../app/employee/services/employee.service';
import { QuyTrinhService } from '../../../../../../app/admin/services/quy-trinh.service';
import { TreeNode } from 'primeng/api';
import { ExportFileWordService } from '../../../../../shared/services/exportFileWord.services';
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

class CategoryModel {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
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
  deNghiTamHoanUngChiTietId: number;
  deNghiTamHoanUngId: number;
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

class ThongTinDeNghi {
  deNghiTamHoanUngId: number;
  maDeNghi: string;
  ngayDeNghi: Date;
  hoSoCongTacId: number;
  lyDo: string;
  nguoiThuHuongId: string;
  loaiDeNghi: number;
  tienTamUng: number;
  trangThai: number;
  nguoiDeXuatId: string;
  nguoiDeXuat: string;

  constructor() {
    this.nguoiDeXuat = null;
  }
}

@Component({
  selector: 'app-chi-tiet-de-nghi-tam-hoan-ung',
  templateUrl: './chi-tiet-de-nghi-tam-hoan-ung.component.html',
  styleUrls: ['./chi-tiet-de-nghi-tam-hoan-ung.component.css']
})
export class ChiTietDeNghiTamHoanUngComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;
  today: Date = new Date();

  /*Khai báo biến*/
  auth: any = JSON.parse(localStorage.getItem("auth"));
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';

  /** Action */
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  /**End */

  deNghiTamHoanUngId: number = 0;
  hoSoCongTacId: number = 0;
  maNV: any

  // Dữ liệu masterdata
  listHoSoCT: Array<any> = new Array<any>();
  listHoSoCTFull: Array<any> = new Array<any>();
  listEmployee: Array<EmployeeModel> = new Array<EmployeeModel>();
  listHinhThucTT: Array<CategoryModel> = new Array<CategoryModel>();
  employeeModel: EmployeeModel = new EmployeeModel();
  listNoiDungTT: Array<any> = new Array<any>();
  listData: Array<TreeNode> = [];

  thongTinDeNghiModel: ThongTinDeNghi = new ThongTinDeNghi();
  loaiDeNghi: number = 0;

  colsList: any[];
  selectedColumns: any[];

  // Tài liệu liên quan
  @ViewChild('fileUpload') fileUpload: FileUpload;
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];
  colsFile: any[];
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';

  // Thông tin thanh toán
  deNghiThanhToanForm: FormGroup;
  ngayDeNghiControl: FormControl;
  theoHoSoCTControl: FormControl;
  lyDoControl: FormControl;
  nhanVienThuHuongControl: FormControl;

  tongTien: number = 0;
  tienTamUng: number = 0;
  tongTienConLaiCTT: number = 0;

  clonedData: { [s: string]: any; } = {}

  //#region   QUY TRÌNH PHÊ DUYỆT
  listFormatStatusSupport: Array<any> = []; // Thanh trạng thái
  showLyDoTuChoi: boolean = false;
  tuChoiForm: FormGroup;
  lyDoTuChoiControl: FormControl;
  //#endregion

  /*Điều kiện hiển thị các button*/
  isShowGuiPheDuyet: boolean = false;
  isShowPheDuyet: boolean = false;
  isShowTuChoi: boolean = false;
  isShowLuu: boolean = false;
  isShowXoa: boolean = false;
  isShowDatVeMoi: boolean = false;
  /*End*/

  refreshNote: number = 0;

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
    private employeeService: EmployeeService,
    private quyTrinhService: QuyTrinhService,
    public exportFileWordService: ExportFileWordService,
    private encrDecrService: EncrDecrService,
  ) {

  }

  async ngOnInit() {
    this.setForm();
    let resource = "hrm/employee/chi-tiet-de-nghi-tam-hoan-ung/";
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
      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }

      this.route.params.subscribe(params => {
        this.deNghiTamHoanUngId = Number(this.encrDecrService.get(params['deNghiTamHoanUngId']));
        this.hoSoCongTacId = Number(this.encrDecrService.get(params['hoSoCongTacId']));
      });

      this.setTable();
      this.getMasterData();
    }
  }

  setForm() {
    this.ngayDeNghiControl = new FormControl(null, [Validators.required]);
    this.theoHoSoCTControl = new FormControl(null, [Validators.required]);
    this.lyDoControl = new FormControl(null, [Validators.required]);
    this.nhanVienThuHuongControl = new FormControl(null, [Validators.required]);

    this.deNghiThanhToanForm = new FormGroup({
      ngayDeNghiControl: this.ngayDeNghiControl,
      theoHoSoCTControl: this.theoHoSoCTControl,
      lyDoControl: this.lyDoControl,
      nhanVienThuHuongControl: this.nhanVienThuHuongControl,
    });

    this.lyDoTuChoiControl = new FormControl(null, [Validators.required]);
    this.tuChoiForm = new FormGroup({
      lyDoTuChoiControl: this.lyDoTuChoiControl,
    });
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

    this.colsFile = [
      { field: 'fileFullName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', color: '#f44336' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'right', color: '#f44336' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'center', color: '#f44336' },
      { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'left', color: '#f44336' },
    ];
  }

  async getMasterData() {
    this.loading = true;
    this.awaitResult = true;
    let [result, resultDetail]: any = await Promise.all([
      this.quanLyCongtacService.getMasterDataDeNghiForm(),
      this.quanLyCongtacService.getDataDeNghiDetailForm(this.deNghiTamHoanUngId)
    ]);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showToast('error', 'Thông báo:', result.messageCode);
      return;
    }

    if (resultDetail.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showToast('error', 'Thông báo:', resultDetail.messageCode);
      return;
    }
    this.listHoSoCT = result.listHoSoCongTac;
    this.listHoSoCTFull = result.listHoSoCongTacFull;
    this.listHinhThucTT = result.listHinhThucTT;

    this.mapDataDeNghiToModel(resultDetail);

    await this.getDuLieuQuyTrinh();

    this.arrayDocumentModel = resultDetail.listFileInFolder;
    this.ref.detectChanges();
    this.loading = false;
    this.awaitResult = false;
  }

  mapDataDeNghiToModel(resultDetail: any) {
    this.isShowGuiPheDuyet = resultDetail.isShowGuiPheDuyet;
    this.isShowPheDuyet = resultDetail.isShowPheDuyet;
    this.isShowTuChoi = resultDetail.isShowTuChoi;
    this.isShowLuu = resultDetail.isShowLuu;
    this.isShowXoa = resultDetail.isShowXoa;
    this.isShowDatVeMoi = resultDetail.isShowDatVeMoi;

    this.thongTinDeNghiModel = resultDetail.deNghiTamHoanUng;

    if (this.thongTinDeNghiModel.trangThai != 1) {
      this.deNghiThanhToanForm.disable();
    }
    else {
      this.deNghiThanhToanForm.enable();
    }

    let hoSo = this.listHoSoCTFull.find(x => x.hoSoCongTacId == this.thongTinDeNghiModel.hoSoCongTacId);
    if (hoSo != null && hoSo != undefined) {
      var check = this.listHoSoCT.find(e => e.hoSoCongTacId == hoSo.hoSoCongTacId)
      if (!check) {
        this.listHoSoCT.push(hoSo)
      }
      this.theoHoSoCTControl.setValue(hoSo);
      this.listEmployee = hoSo.listNhanVienCT;
      let nhanVienThuHuong = this.listEmployee.find(x => x.employeeId == this.thongTinDeNghiModel.nguoiThuHuongId);
      this.maNV = (nhanVienThuHuong as any).employeeCode
      this.nhanVienThuHuongControl.setValue(nhanVienThuHuong)
      this.mappingModelToFrom(nhanVienThuHuong);
    }

    this.ngayDeNghiControl.setValue(new Date(this.thongTinDeNghiModel.ngayDeNghi));
    this.loaiDeNghi = this.thongTinDeNghiModel.loaiDeNghi;
    this.tienTamUng = this.thongTinDeNghiModel.tienTamUng;
    this.lyDoControl.setValue(this.thongTinDeNghiModel.lyDo);

    // Thông tin thanh toán
    this.listNoiDungTT = resultDetail.listNoiDungTT;

    // Hoàn ứng
    if (this.loaiDeNghi == 1) {
      this.listNoiDungTT.forEach((item, index) => {
        item.id = index + 1;
        let hinhThucThanhToan = this.listHinhThucTT.find(c => c.categoryId == item.hinhThucThanhToan);
        item.selectedHinhThucThanhToan = hinhThucThanhToan;

        item.ngayThang = item.ngayThang ? new Date(item.ngayThang) : null;
      });
    }
    // Tạm ứng
    else if (this.loaiDeNghi == 0) {
      this.listNoiDungTT.forEach(item => {
        item.id = item.deNghiTamHoanUngChiTietId;
      });
      this.listData = this.list_to_tree(this.listNoiDungTT);
    }

    this.tinhToanTongGiaTriTT();

    // TẠM ỨNG
    if (this.loaiDeNghi == 0)
      this.selectedColumns = [
        { field: 'noiDung', header: 'Tóm tắt mục đích chi', width: '40%', textAlign: 'left', color: '#f44336' },
        { field: 'maPhongBan', header: 'Mã phòng ban', width: '15%', textAlign: 'center', color: '#f44336' },
        { field: 'tongTienTruocVat', header: 'Số tiền trước VAT', width: '15%', textAlign: 'right', color: '#f44336' },
        { field: 'vat', header: 'VAT', width: '5%', textAlign: 'right', color: '#f44336' },
        { field: 'tienSauVat', header: 'Số tiền sau VAT', width: '15%', textAlign: 'right', color: '#f44336' },
        { field: 'action', header: 'Thao tác', width: '10%', textAlign: 'center', color: '#f44336' },
      ];
    // HOÀN ỨNG
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
    if (event) {
      this.nhanVienThuHuongControl.setValue(event)
      this.mappingModelToFrom(event);
      this.ref.detectChanges();
    }
  }

  mappingModelToFrom(model: any) {
    if (model) {
      this.employeeModel.bankName = model.bankAddress;
      this.employeeModel.bankOwnerName = model.bankOwnerName;
      this.employeeModel.organizationName = model.organizationName;
      this.employeeModel.bankAccount = model.bankAccount;
      this.employeeModel.bankCode = model.bankName;

    }
    else {
      this.employeeModel = new EmployeeModel();
    }
    this.ref.detectChanges();
  }

  addRow() {
    // TẠM ỨNG
    if (this.loaiDeNghi == 0) {
      let listId = this.getListId(this.listData, []);

      let maxId = 0;
      if (listId.length > 0) maxId = Math.max(...listId.map(x => x));

      let item = new NoiDungTamUngModel();
      item.id = maxId + 1;

      this.listNoiDungTT = [...this.listNoiDungTT, item];
      this.listData = this.list_to_tree(this.listNoiDungTT);
    }
    // HOÀN ỨNG
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

  onRowEditSave(rowData: any) {
    delete this.clonedData[rowData.id];
  }

  onRowEditInit(rowData: any) {
    this.clonedData[rowData.id] = { ...rowData };
    this.ref.detectChanges();
  }

  onRowEditCancel(rowData: any, index: number) {
    this.listNoiDungTT[index] = this.clonedData[rowData.id];
    delete this.clonedData[rowData.id];
    this.tinhToanTongGiaTriTT();
  }

  async onRowRemove(rowData: any) {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn xóa dòng này?`,
      accept: async () => {
        delete this.clonedData[rowData.id];
        this.listNoiDungTT = this.listNoiDungTT.filter(e => e != rowData);
        this.tinhToanTongGiaTriTT();
      }
    });
  }

  tinhToanTongGiaTriTT(rowData = null) {
    this.tongTienConLaiCTT = 0;
    this.tongTien = 0;

    //Hoàn ứng
    if (this.loaiDeNghi == 1) {
      this.listNoiDungTT.forEach(item => {
        let vat = (item.vat == null || item.vat == '') ? 0 : ParseStringToFloat(item.vat);
        if (vat > 100) vat = 100;

        let vanChuyenXm = item.vanChuyenXm == null ? 0 : ParseStringToFloat(item.vanChuyenXm.toString());
        let tienDonHnnb = item.tienDonHnnb == null ? 0 : ParseStringToFloat(item.tienDonHnnb.toString());
        let tienDonDn = item.tienDonDn == null ? 0 : ParseStringToFloat(item.tienDonDn.toString());
        let khachSan = item.khachSan == null ? 0 : ParseStringToFloat(item.khachSan.toString());
        let chiPhiKhac = item.chiPhiKhac == null ? 0 : ParseStringToFloat(item.chiPhiKhac.toString());

        let truocVAT = vanChuyenXm + tienDonHnnb + tienDonDn + khachSan + chiPhiKhac;

        item.vat = vat;
        item.vanChuyenXm = vanChuyenXm;
        item.tienDonHnnb = tienDonHnnb;
        item.tienDonDN = tienDonDn;
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

  // Thêm dòng con thuộc nội dung cha
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

  checkValid(): boolean {
    if (!this.deNghiThanhToanForm.valid) {
      Object.keys(this.deNghiThanhToanForm.controls).forEach(key => {
        if (!this.deNghiThanhToanForm.controls[key].valid) {
          this.deNghiThanhToanForm.controls[key].markAsTouched();
        }
      });

      return false;
    }

    let isError = false;

    //Nếu là tạm ứng
    if (this.loaiDeNghi == 0) {
      if (this.listNoiDungTT.length == 0 || this.tongTien == 0) {
        this.showToast('warn', 'Thông báo', "Bạn cần nhập đủ thông tin thanh toán.");
        return false;
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

        let tong = item.vanChuyenXM + item.tienDonHNNB + item.tienDonDN + item.khachSan + item.chiPhiKhac;
        if (tong == 0) isError = true;

        item.hinhThucThanhToan = item.selectedHinhThucThanhToan?.categoryId;
      });
    }

    if (isError) {
      this.showToast('warn', 'Thông báo', "Bạn cần nhập đủ thông tin thanh toán.");
      return false;
    }

    return true;
  }

  async update() {
    // Thông tin chung tạm hoàn ứng
    let ngayDeNghi = this.ngayDeNghiControl.value ? convertToUTCTime(this.ngayDeNghiControl.value) : null;

    let deNghiTamHoanUngModel = new DeNghiTamHoanUngModel();
    deNghiTamHoanUngModel.ngayDeNghi = ngayDeNghi;
    deNghiTamHoanUngModel.deNghiTamHoanUngId = this.deNghiTamHoanUngId;
    deNghiTamHoanUngModel.lyDo = this.lyDoControl.value;
    deNghiTamHoanUngModel.hoSoCongTacId = this.theoHoSoCTControl.value.hoSoCongTacId;
    deNghiTamHoanUngModel.tienTamUng = this.tienTamUng;
    deNghiTamHoanUngModel.tongTienThanhToan = this.tongTien;
    deNghiTamHoanUngModel.nguoiThuHuongId = this.nhanVienThuHuongControl.value.employeeId;
    deNghiTamHoanUngModel.loaiDeNghi = this.loaiDeNghi;
    // --------------

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.quanLyCongtacService.createOrUpdateDeNghiTamHoanUng(deNghiTamHoanUngModel, this.listNoiDungTT, 'DENGHITHU', [], this.auth.UserId);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showToast('error', 'Thông báo', result.messageCode);
      return {
        status: false,
        message: result.messageCode
      };
    }

    return {
      status: true,
      message: 'Lưu thành công'
    };
  }

  async capNhatDeNghi() {
    if (!this.checkValid()) {
      return;
    }

    let rersult = await this.update();
    if (!rersult.status) {
      return;
    }

    this.showToast('success', 'Thông báo', rersult.message);
    await this.getMasterData();
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
      message: 'Bạn chắc chắn muốn xóa tài liệu?',
      accept: () => {
        this.employeeService.deleteFile(file.fileInFolderId).subscribe(res => {
          let result: any = res;
          if (result.statusCode == 200) {
            let index = this.arrayDocumentModel.indexOf(file);
            this.arrayDocumentModel.splice(index, 1);
            this.showToast('success', 'Thông báo', 'Xóa file thành công');
          } else {
            this.showToast('success', 'Thông báo', result.messageCode);
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
      fileUpload.FileInFolder.objectNumber = this.deNghiTamHoanUngId;
      fileUpload.FileInFolder.objectType = 'DENGHITHU';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    this.quanLyCongtacService.uploadFile("DENGHITHU", listFileUploadModel, this.deNghiTamHoanUngId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.uploadedFiles = [];

        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }

        this.arrayDocumentModel = result.listFileInFolder;
        this.ref.detectChanges();
        this.showToast('success', 'Thông báo', "Thêm file thành công");
      } else {
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  async getDuLieuQuyTrinh() {
    // 21: TẠM ỨNG ---- 22: HOÀN ỨNG
    this.quyTrinhService.getDuLieuQuyTrinh(this.emptyGuid, this.loaiDeNghi == 0 ? 21 : 22, this.deNghiTamHoanUngId).subscribe(res => {
      let result: any = res;
      if (result.statusCode == 200) {
        this.listFormatStatusSupport = result.listDuLieuQuyTrinh;
      }
      else {
        this.showToast('error', 'Thông báo:', result.messageCode);
      }
    });
  }

  // Chuyển về trạng thái mới sau khi bị từ chối phê duyệt
  async datVeMoi() {
    let mes = this.loaiDeNghi == 0 ? "tạm ứng" : "hoàn ứng";
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn sử dụng lại đề nghị ' + mes + ' này?',
      accept: async () => {
        let DoiTuongApDung = this.loaiDeNghi == 0 ? 21 : 22;
        this.loading = true;
        let res: any = await this.quanLyCongtacService.datVeMoiDeNghiTamHoanUng(this.deNghiTamHoanUngId, DoiTuongApDung);
        if (res.statusCode === 200) {
          this.showToast('success', 'Thông báo:', "Cập nhật trạng thái đề nghị thành công.");
          this.getMasterData();
        } else {
          this.showToast('success', 'Thông báo:', res.messageCode);
        }
      }
    });
  }

  // Gửi phê duyệt
  async guiPheDuyet() {
    if (!this.checkValid()) {
      return;
    }

    let rersultUpdate = await this.update();
    if (!rersultUpdate.status) {
      return;
    }

    // 21: TẠM ỨNG ---- 22: HOÀN ỨNG
    this.quyTrinhService.guiPheDuyet(this.emptyGuid, this.loaiDeNghi == 0 ? 21 : 22, this.deNghiTamHoanUngId).subscribe(async res => {
      let result: any = res;

      if (result.statusCode != 200) {
        this.loading = false;
        this.awaitResult = false;
        this.showToast('error', 'Thông báo:', result.messageCode);
        return;
      }

      this.showToast('success', 'Thông báo:', "Gửi phê duyệt thành công.");
      await this.getMasterData();
      this.ref.detectChanges();
    });
  }

  // Phê duyệt
  async pheDuyet() {
    if (this.listNoiDungTT.length == 0) {
      this.showToast('error', 'Thông báo:', 'Vui lòng nhập nội dung thanh toán.');
      return;
    }

    this.confirmationService.confirm({
      message: `Bạn có chắn chắn muốn phê duyệt đề nghị ${this.loaiDeNghi == 0 ? 'tạm ứng' : 'hoàn ứng'} này?`,
      accept: async () => {
        this.loading = true;
        // 21: TẠM ỨNG ---- 22: HOÀN ỨNG
        this.quyTrinhService.pheDuyet(this.emptyGuid, this.loaiDeNghi == 0 ? 21 : 22, '', this.deNghiTamHoanUngId).subscribe(res => {
          let result: any = res;

          if (result.statusCode == 200) {
            this.getMasterData();
            this.ref.detectChanges();
          }
          else {
            this.loading = false;
            this.showToast('error', 'Thông báo:', result.messageCode);
          }
        });
      }
    });
  }

  // Từ chối phê duyệt
  async tuChoi() {
    if (!this.tuChoiForm.valid) {
      Object.keys(this.tuChoiForm.controls).forEach(key => {
        if (!this.tuChoiForm.controls[key].valid) {
          this.tuChoiForm.controls[key].markAsTouched();
        }
      });
      return;
    }

    this.loading = true;
    // 21: TẠM ỨNG ---- 22: HOÀN ỨNG
    this.quyTrinhService.tuChoi(this.emptyGuid, this.loaiDeNghi == 0 ? 21 : 22, this.lyDoTuChoiControl.value, this.deNghiTamHoanUngId).subscribe(async res => {
      let result: any = res;

      if (result.statusCode != 200) {
        this.loading = false;
        this.showToast('error', 'Thông báo:', result.messageCode);
      }

      this.showToast('success', 'Thông báo:', result.messageCode);
      await this.getMasterData();
      this.showLyDoTuChoi = false;
    });
  }

  xoa() {
    this.confirmationService.confirm({
      message: 'Dữ liệu không thể hoàn tác, bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.quanLyCongtacService.xoaDeNghiTamHoanUng(this.deNghiTamHoanUngId).subscribe(response => {
          let result = <any>response;

          if (result.statusCode != 200) {
            this.showToast('error', 'Thông báo:', result.messageCode);
            return;
          }

          this.showToast('success', 'Thông báo:', 'Xóa thành công');
          setTimeout(() => {
            if (this.loaiDeNghi == 1) this.router.navigate(['/employee/danh-sach-de-nghi-hoan-ung']);
            else this.router.navigate(['/employee/danh-sach-de-nghi-tam-ung']);
          }, 1000);
        });
      }
    });
  }

  openDialogReject() {
    this.showLyDoTuChoi = true;
    this.tuChoiForm.reset();
  }

  closeDialog() {
    this.showLyDoTuChoi = false;
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  cancel() {
    if (this.hoSoCongTacId != 0 && this.hoSoCongTacId != undefined) {
      this.router.navigate(['/employee/chi-tiet-ho-so-cong-tac', { hoSoCongTacId: this.encrDecrService.set(this.hoSoCongTacId) }]);
    }
    else {
      if (this.loaiDeNghi == 1)
        this.router.navigate(['/employee/danh-sach-de-nghi-hoan-ung']);
      else
        this.router.navigate(['/employee/danh-sach-de-nghi-tam-ung']);
    }
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  xuatFile() {
    var dataParent: Array<any> = []
    var dataChild: Array<any> = []
    var parent: any
    var child: any
    var fileDetail: any

    var file: Array<any> = []

    this.arrayDocumentModel.forEach(e => {
      fileDetail = {
        fileName: e.fileFullName.substring(0, e.fileFullName.lastIndexOf('_')),
        fileExtension: e.fileExtension
      }
      file.push(fileDetail)
    })

    this.listData.forEach(e => {
      e.children.forEach(i => {
        child = {
          maPhongBan: i.data.maPhongBan,
          noiDung: i.data.noiDung,
          tienSauVat: i.data.tienSauVat,
          tongTienTruocVat: i.data.tongTienTruocVat,
          vat: i.data.vat,
          parentId: i.data.parentId
        }
        dataChild.push(child)
      });
      parent = {
        id: e.data.id,
        noiDung: e.data.noiDung,
      }
      dataParent.push(parent)
    });

    if (this.loaiDeNghi == 0) {
      let data = {
        template: 5,
        nguoiDeNghi: (this.nhanVienThuHuongControl.value).employeeName,
        tenTaiKhoan: this.employeeModel.bankOwnerName,
        nganHang: this.employeeModel.bankName,
        phong: this.employeeModel.organizationName,
        maNganHang: this.employeeModel.bankCode,
        soTaiKhoan: this.employeeModel.bankAccount,
        liDo: this.lyDoControl.value,
        tongTien: this.tongTien,
        tongTienConLaiCTT: this.tongTienConLaiCTT,
        date: convertDate(new Date(this.ngayDeNghiControl.value)),
        dataChild: dataChild,
        dataParent: dataParent,
        fileTamUng: file,
      }

      this.exportFileWordService.saveFileWord(data, `Đề nghị tạm ứng - ${this.maNV}_${data.nguoiDeNghi} .docx`)
    } else {
      var listData: Array<any> = []
      this.listNoiDungTT.forEach(e => {
        let data = {
          chiPhiKhac: e.chiPhiKhac,
          date: convertDate(new Date(e.ngayThang)),
          soPhieu: e.soHoaDon,
          tomTat: e.noiDung,
          hinhThucTT: e.selectedHinhThucThanhToan.categoryName,
          maPhongBan: e.chiPhiKhac,
          xeMay: e.vanChuyenXm,
          tienHN: e.tienDonHnnb,
          tienDN: e.tienDonDn,
          khachSan: e.khachSan,
          tienTruoc: e.tongTienTruocVat,
          tienSau: e.tienSauVat,
          dinhKem: e.dinhKemCt,
        }
        listData.push(data)
      });

      let data = {
        template: 6,
        hoVaTen: (this.nhanVienThuHuongControl.value).employeeName,
        maNganHang: this.employeeModel.bankCode,
        tenNganHang: this.employeeModel.bankName,
        phongBan: this.employeeModel.organizationName,
        soTaiKhoan: this.employeeModel.bankAccount,
        tenTaiKhoan: this.employeeModel.bankOwnerName,
        tongTien: this.tongTien,
        tongTienTT: this.tongTienConLaiCTT,
        listData: listData
        // dataParent: dataParent,
      }

      this.exportFileWordService.saveFileWord(data, `Đề nghị hoàn ứng - ${this.maNV}_${data.hoVaTen}.docx`)
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

function convertDate(time: any) {
  let ngay = time.getDate()
  let thang = time.getMonth() + 1
  let nam = time.getFullYear()
  return `${ngay}/${thang}/${nam}`
};
