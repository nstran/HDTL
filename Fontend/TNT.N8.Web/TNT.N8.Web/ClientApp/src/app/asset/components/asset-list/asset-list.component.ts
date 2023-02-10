import * as $ from 'jquery';
import { Component, OnInit, ViewChild, ElementRef, HostListener, Renderer2, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { DatePipe } from '@angular/common';
import 'moment/locale/pt-br';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { Workbook } from 'exceljs';
import { AssetService } from '../../services/asset.service';
import { TaiSanModel } from '../../models/taisan.model';
import { Table } from 'primeng/table';
import { saveAs } from "file-saver";
import * as XLSX from 'xlsx';
import { AssetImportDetailComponent } from '../asset-import-detail/asset-import-detail.component';
import { EmployeeService } from '../../../employee/services/employee.service';
import { EncrDecrService } from '../../../shared/services/encrDecr.service';


import * as pdfMake from "pdfmake/build/pdfmake.js";
import * as pdfFonts from "pdfmake/build/vfs_fonts.js";
pdfMake.vfs = pdfFonts.pdfMake.vfs;

export class CategoryModel {
  categoryId: string;
  categoryCode: string;
  categoryName: string;

  constructor() {
    this.categoryId = '00000000-0000-0000-0000-000000000000';
    this.categoryCode = '';
    this.categoryName = '';
  }
}

interface icard_infor {
  account: string;
  dept: string;
  serial: string;
  qr_code: string;
  moTa: string;
  maTaiSan: string;
}

class importTaiSanModel {
  maTaiSan: string;
  tenTaiSan: string;
  phanLoaiTaiSanId: string;
  donViTinhId: string;
  khuVucTaiSanId: string;
  ngayVaoSo: Date;
  moTa: string;
  soSerial: string;
  model: string;
  soHieu: string;
  thongTinNoiMua: string;
  namSx: number;
  nuocSxid: string;
  hangSxid: string;
  ngayMua: Date;
  thoiHanBaoHanh: number;
  baoDuongDinhKy: number;
  thongTinNoiBaoHanh: string;
  giaTriNguyenGia: number;
  giaTriTinhKhauHao: number;
  thoiGianKhauHao: number;
  thoiDiemBdtinhKhauHao: Date;

  nuocSxCode: string;
  hangSxCode: string;
  phanLoaiTaiSanCode: string;
  donViTinhCode: string;
  khuVucTaiSanCode: string;
}



//model dùng để build template excel
class columnModel {
  column1: string;
  column2: string;
}

class companyConfigModel {
  companyName: string;
  companyAddress: string;
  phone: string;
  email: string;
  website: string;
  constructor() {
    this.companyName = '';
    this.companyAddress = '';
    this.phone = '';
    this.email = '';
    this.website = '';
  }
}

class exportExcelModel {
  maTaiSan: string;
  tenTaiSan: string;
  ngayVaoSo: string;
  hienTrangTaiSanString: string;
  phanLoaiTaiSan: string;
  maNV: string;
  hoVaTen: string;
  phongBan: string;
  viTriLamViec: string;
  moTa: string;

  giaTriNguyenGia: string;
  giaTriTinhKhauHao: string;
  tiLeKhauHaoTheoThang: string;
  tiLeKhauHaoTheoNam: string;
  giaTriKhauHaoTheoThang: string;
  giaTriKhauHaoTheoNam: string;
  thoiGianTinhKhauHaoDen: string;
  giaTriKhauHaoLuyKe: string;
  giaTriConLai: string;
}

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
}

@Component({
  selector: 'app-asset-list',
  templateUrl: './asset-list.component.html',
  styleUrls: ['./asset-list.component.css'],
  providers: [
    DatePipe,
  ]
})
export class AssetListComponent implements OnInit {
  fixed: boolean = false;
  withFiexd: string = "";

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  auth: any = JSON.parse(localStorage.getItem('auth'));
  //get system parameter
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  loading: boolean = false;

  listAsset: Array<any> = []; // Danh sách tài sản
  taiSanDetail: TaiSanModel = new TaiSanModel();
  selectedColumns: any[];
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  isUpdate: boolean = false;
  isDisplayName: boolean = false;


  listAssetSelected: Array<any> = []; // Danh sách tài sản


  /*START : FORM PriceList*/
  searchForm: FormGroup;
  /*END : FORM PriceList*/

  //responsive
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  leftColNumber: number = 12;
  leftColGridNumber: number = 12;
  rightColNumber: number = 2;
  today: string = '';
  colsList: any[];
  todayDate: Date = new Date();
  // QRCode
  qrCode: any = "Loft3Di";
  isShowQRCode: boolean = false;

  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('myTable') myTable: Table;
  filterGlobal: string = '';
  taiSanId: number = 0;
  companyConfig = new companyConfigModel();
  isShowDetail: boolean = false;


  //import varriables
  displayChooseFileImportDialog: boolean = false;
  value: boolean = false;
  fileName: string = '';
  importFileExcel: any = null;


  listHienTrangTS = [
    {
      id: 1, name: 'Đang sử dụng'
    },
    {
      id: 0, name: 'Không sử dụng'
    }];

  // listLoaiBaoCao = [
  //   {
  //     id: 1, name: 'Không'
  //   },
  //   {
  //     id: 2, name: 'Báo cáo phân bổ'
  //   },
  //   {
  //     id: 3, name: 'Báo cáo khấu hao'
  //   }
  // ];
  viewbaoCao: number = 1;
  /* Form search */
  searchAssetForm: FormGroup;
  maTaiSanControl: FormControl;
  tenTaiSanControl: FormControl;
  //hienTrangTaiSanControl: FormControl;
  selectedNguoiSD: Array<any> = [];
  listEmployee: Array<EmployeeModel> = new Array<EmployeeModel>();

  selectedLoaiTS: Array<any> = [];
  listLoaiTaiSan: Array<CategoryModel> = new Array<CategoryModel>();

  listKhuVuc: Array<any> = new Array<any>();

  loaiBaoCaoControl: FormControl;

  selectedKhuVuc: Array<any> = [];


  // baoCaoKhauHaoForm: FormGroup;
  // ngayTinhKhauHaoDenControl: FormControl;
  displayKhauHao: boolean = false;
  selectedTrangThaiBaoCao: Array<any> = [];
  selectedTrangThai: Array<any> = [];
  thoiGianTinhKhauHaoDen: Date = new Date();
  selectedLoaiTSBaoCao: Array<any> = [];

  constructor(
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private el: ElementRef,
    private renderer: Renderer2,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    public dialogService: DialogService,
    private router: Router,
    private assetService: AssetService,
    private datePipe: DatePipe,
    private ref: ChangeDetectorRef,
    private employeeService: EmployeeService,
    private encrDecrService: EncrDecrService,
  ) {
    this.innerWidth = window.innerWidth;
    this.renderer.listen('window', 'click', (e: Event) => {
      /**
       * Only run when toggleButton is not clicked
       * If we don't check this, all clicks (even on the toggle button) gets into this
       * section which in the result we might never see the menu open!
       * And the menu itself is checked here, and it's where we check just outside of
       * the menu and button the condition abbove must close the menu
       */
      if (this.toggleButton && this.notifi) {
        if (!this.toggleButton.nativeElement.contains(e.target) &&
          !this.notifi.nativeElement.contains(e.target) &&
          !this.save.nativeElement.contains(e.target)) {
          this.isOpenNotifiError = false;
        }
      }
    });
  }

  async ngOnInit() {
    let resource = "ass/asset/list";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    } else {

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
      this.setForm();
      this.setTable();
      this.getMasterData();
      this.setDefaultPaidDate();
      this.isShowQRCode = false;
    }
  }

  setForm() {
    this.maTaiSanControl = new FormControl(null); // Mã tài sản
    this.tenTaiSanControl = new FormControl(null); // Tên tài sản
    //this.hienTrangTaiSanControl = new FormControl(null); // Hiện trạng tài sản
    this.loaiBaoCaoControl = new FormControl(null);
    // this.ngayTinhKhauHaoDenControl = new FormControl(null);
    this.searchAssetForm = new FormGroup({
      maTaiSanControl: this.maTaiSanControl,
      tenTaiSanControl: this.tenTaiSanControl,
      // hienTrangTaiSanControl: this.hienTrangTaiSanControl,
      loaiBaoCaoControl: this.loaiBaoCaoControl
    });

    // this.baoCaoKhauHaoForm = new FormGroup({
    //   ngayTinhKhauHaoDenControl: this.ngayTinhKhauHaoDenControl,
    // });
  }

  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }

  setDefaultPaidDate() {
    var datePipe = new DatePipe("en-US");
    var _today = new Date();
    this.today = datePipe.transform(_today, 'dd-MM-yyyy');
  }

  setTable() {
    this.colsList = [
      { field: 'maTaiSan', header: 'Mã tài sản', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'tenTaiSan', header: 'Tên tài sản', width: '200px', textAlign: 'left', color: '#f44336' },
      { field: 'ngayVaoSo', header: 'Ngày vào sổ', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'hienTrangTaiSanString', header: 'Trạng thái', width: '100px', textAlign: 'center', color: '#f44336' },

      { field: 'maNV', header: 'Mã NV sử dụng', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'hoVaTen', header: 'Họ tên', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'phongBan', header: 'Phòng ban', width: '100px', textAlign: 'left', color: '#f44336' },
      { field: 'viTriLamViec', header: 'Vị trí việc làm', width: '100px', textAlign: 'left', color: '#f44336' },
      { field: 'moTa', header: 'Mô tả', width: '100px', textAlign: 'left', color: '#f44336' },

      { field: 'giaTriNguyenGia', header: 'Nguyên giá', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'giaTriTinhKhauHao', header: 'Giá trị tính khấu hao', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'tiLeKhauHaoTheoThang', header: 'Tỉ lệ khấu hao theo tháng (%)', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'tiLeKhauHaoTheoNam', header: 'Tỉ lệ khấu hao theo năm (%)', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'giaTriKhauHaoTheoThang', header: 'Giá trị khấu hao theo tháng', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'giaTriKhauHaoTheoNam', header: 'Giá trị khấu hao theo năm', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'thoiGianTinhKhauHaoDen', header: 'Thời gian tính khấu hao đến', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'giaTriKhauHaoLuyKe', header: 'Giá trị khấu hao lũy kế', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'giaTriConLai', header: 'Giá trị còn lại', width: '100px', textAlign: 'right', color: '#f44336' },
    ];
    this.selectedColumns = this.colsList.filter(e =>
      e.field == "maTaiSan" || e.field == "tenTaiSan"
      || e.field == "ngayVaoSo" || e.field == "hienTrangTaiSanString"
      // || e.field == "maNV"
      // || e.field == "hoVaTen" || e.field == "phongBan" || e.field == "viTriLamViec"
    );
  }

  delAsset(rowData) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa tài sản?',
      accept: () => {
        this.assetService.deleteAsset(rowData.priceProductId).subscribe(response => {
          var result = <any>response;
          if (result.statusCode == 200) {
            this.listAsset = this.listAsset.filter(c => c != rowData);
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.message };
            this.showMessage(mgs);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.message };
            this.showMessage(mgs);
          }
        });
      }
    });
  }

  getMasterData() {
    this.loading = true;
    this.assetService.getAllAssetList().subscribe(response => {
      var result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        console.log("result", result)
        this.listAsset = result.listAsset;
        this.companyConfig = result.companyConfig;
        this.listEmployee = result.listEmployee;
        this.listLoaiTaiSan = result.listLoaiTaiSan;
        this.listKhuVuc = result.listKhuVuc;
      }
    });
  }

  checkEnterPress(event: any) {
    if (event.code === "Enter") {
      this.searchAsset(1);
    }
  }

  refreshFilter() {
    this.filterGlobal = '';
    this.filterGlobal = null;
    this.isShowQRCode = false;
    this.isShowDetail = false;
    this.leftColGridNumber = 12;
    this.selectedNguoiSD = [];
    this.selectedLoaiTS = [];
    this.viewbaoCao = 1;
    this.searchAssetForm.reset();

    if (this.listAsset.length > 0) {
      this.myTable.reset();
    }
    this.searchAsset(1);
  }

  showFilter() {
    this.isShowDetail = false;
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.leftColGridNumber = 12;
        this.rightColNumber = 4;
      } else {
        this.leftColNumber = 12;
        this.leftColGridNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  // Thu hồi tài sản
  thuHoiTaiSan() {
    this.router.navigate(['/asset/thu-hoi-tai-san']);
  }

  // Xem chi tiết tài sản
  goToCreate() {
    this.router.navigate(['/asset/create']);
  }

  // Phân bổ tài sản
  phanBoTaiSan() {
    this.router.navigate(['/asset/phan-bo-tai-san']);
  }

  xemChiTietTS() {
    if (this.taiSanId != 0)
      this.router.navigate(['/asset/detail', { assetId: this.encrDecrService.set(this.taiSanId) }]);
  }

  // Tìm kiếm tài sản
  async searchAsset(type: any) {
    let assetName = this.searchAssetForm.get('maTaiSanControl').value;
    let assetCode = this.searchAssetForm.get('tenTaiSanControl').value;
    //let trangThai = this.searchAssetForm.get('hienTrangTaiSanControl').value == null ? null : this.searchAssetForm.get('hienTrangTaiSanControl').value.id;
    let selectedNguoiSDId = this.selectedNguoiSD.map(p => p.employeeId);
    let selectedLoaiTaiSanId = this.selectedLoaiTS.map(p => p.categoryId);
    let trangThaiTS = this.selectedTrangThai.map(p => p.id);
    let thoiGianTinhKH = null;
    let khuVucId = this.selectedKhuVuc.map(x => x.provinceId);
    if (this.viewbaoCao == 3) {
      // Loại tài sản
      selectedLoaiTaiSanId = this.selectedLoaiTSBaoCao.map(p => p.categoryId);

      // Trạng thái
      trangThaiTS = this.selectedTrangThaiBaoCao.map(p => p.id);
    }

    if (type == 1) {
      this.viewbaoCao = 1;
      this.selectedColumns = this.colsList.filter(e =>
        e.field == "maTaiSan" || e.field == "tenTaiSan"
        || e.field == "ngayVaoSo" || e.field == "hienTrangTaiSanString"
      );
      this.ref.detectChanges();
    }
    // let thoiGianTinhKH = this.ngayTinhKhauHaoDenControl.value ? convertToUTCTime(this.ngayTinhKhauHaoDenControl.value) : null;

    // // View hiển thị báo cáo
    // this.viewbaoCao = this.searchAssetForm.get('loaiBaoCaoControl').value == null ? null : this.searchAssetForm.get('loaiBaoCaoControl').value.id;

    this.loading = true;
    let result: any = await this.assetService.searchAssetAsync(assetName, assetCode, trangThaiTS, selectedNguoiSDId, selectedLoaiTaiSanId, thoiGianTinhKH, khuVucId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listAsset = result.listAsset;
      if (this.viewbaoCao == 2) // Báo cáo phân bổ
      {
        this.selectedColumns = this.colsList.filter(e =>
          e.field == "maTaiSan" || e.field == "tenTaiSan"
          || e.field == "ngayVaoSo" || e.field == "hienTrangTaiSanString" || e.field == "maNV"
          || e.field == "hoVaTen" || e.field == "phongBan" || e.field == "viTriLamViec" || e.field == "moTa"
        );
      }
      if (this.viewbaoCao == 3) // Báo cáo khấu hao
      {
        this.selectedColumns = this.colsList.filter(e =>
          e.field == "maTaiSan" || e.field == "tenTaiSan"
          || e.field == "ngayVaoSo" || e.field == "giaTriNguyenGia" || e.field == "giaTriTinhKhauHao"
          || e.field == "tiLeKhauHaoTheoThang" || e.field == "tiLeKhauHaoTheoNam" || e.field == "giaTriKhauHaoTheoThang" || e.field == "giaTriKhauHaoTheoNam" || e.field == "thoiGianTinhKhauHaoDen" || e.field == "giaTriKhauHaoLuyKe" || e.field == "giaTriConLai"
        );

        // tính toán lại các giá trị
        this.listAsset.forEach(ass => {

          //Giá trị tính khấu hao
          let giaTriTinhKHao = ass.giaTriTinhKhauHao;

          if (ass.thoiDiemBDTinhKhauHao && this.thoiGianTinhKhauHaoDen && ass.thoiGianKhauHao) {
            // Thời gian khấu hao tính từ thời điểm bắt đầu tính khấu hao đến tháng lựa chọn => tính theo tháng
            // let thoiGianKhauHao = monthDiff(new Date(ass.thoiDiemBDTinhKhauHao), this.thoiGianTinhKhauHaoDen);

            ass.tiLeKhauHaoTheoThang = Number((100 / ass.thoiGianKhauHao).toFixed(2));
            //Giá trị khấu hao theo tháng = (Giá trị tính khấu hao* tỉ lệ khấu hao theo tháng)/100
            ass.giaTriKhauHaoTheoThang = Number(((giaTriTinhKHao * ass.tiLeKhauHaoTheoThang) / 100).toFixed(2));

            //Tỉ lệ khấu hao theo năm = (100 / (Thời gian khấu hao / 12))
            ass.tiLeKhauHaoTheoNam = Number((100 / (ass.thoiGianKhauHao / 12)).toFixed(2));

            //Giá trị khấu hao theo năm = (Giá trị tính khấu hao* tỉ lệ khấu hao theo năm)/100
            ass.giaTriKhauHaoTheoNam = Number(((giaTriTinhKHao * ass.tiLeKhauHaoTheoNam) / 100).toFixed(2));

            //Giá trị khấu hao lũy kế = [ giá trị khấu hao theo tháng * (tháng hiện tại - tháng bắt đầu tính khấu hao)]
            ass.giaTriKhauHaoLuyKe = Number((ass.giaTriKhauHaoTheoThang * getCountMonth(new Date(ass.thoiDiemBDTinhKhauHao), this.thoiGianTinhKhauHaoDen)).toFixed(2)) < 0 ? 0 : Number((ass.giaTriKhauHaoTheoThang * getCountMonth(new Date(ass.thoiDiemBDTinhKhauHao), this.thoiGianTinhKhauHaoDen)).toFixed(2));

            //Giá trị còn lại = Giá trị tính khấu hao - Giá trị khấu hao lũy kế
            ass.giaTriConLai = Number((giaTriTinhKHao - ass.giaTriKhauHaoLuyKe).toFixed(2)) < 0 ? 0 : Number((giaTriTinhKHao - ass.giaTriKhauHaoLuyKe).toFixed(2));

            ass.thoiGianTinhKhauHaoDen = this.thoiGianTinhKhauHaoDen
          }
        });
      }

      this.ref.detectChanges();
      if (this.listAsset.length == 0) {
        let mgs = { severity: 'warn', summary: 'Thông báo', detail: 'Không tìm thấy tài sản nào!' };
        this.showMessage(mgs);
      }
    }
  }

  resetTable() {
    this.filterGlobal = '';
  }

  goToDetail(rowData: any) {
    this.taiSanDetail = rowData;
    this.isShowQRCode = true;
    var qrCodeAsset = this.taiSanDetail.taiSanId;
    this.qrCode = qrCodeAsset.toString();
    this.taiSanId = rowData.taiSanId;
    if (this.isShowFilterLeft)
      this.isShowFilterLeft = !this.isShowFilterLeft;
    this.leftColNumber = 12;
    this.leftColGridNumber = 8;
    this.rightColNumber = 4;
    this.isShowDetail = true
  }

  inQRCode() {
    let printContent = document.getElementById("qrcode");
    // '', '', 'left=0,top=0,width=800,height=700,toolbar=0,scrollbars=0,status=0'
    const windowPrt = window.open();
    windowPrt.document.write(printContent.innerHTML);
    windowPrt.document.close();
    windowPrt.focus();
    windowPrt.print();
    windowPrt.close();
  }

  baoCaoPhanBo() {
    this.viewbaoCao = 2;
    this.searchAsset(2);
  }

  closeKhauHao() {
    this.displayKhauHao = false;
  }

  baoCaoKhauHao() {
    this.displayKhauHao = true;
    this.selectedLoaiTSBaoCao = []
    this.selectedTrangThaiBaoCao = []
    this.thoiGianTinhKhauHaoDen = new Date();
    //this.baoCaoKhauHaoForm.reset();
    this.ref.detectChanges();
  }

  xemBaoCao() {
    if (this.selectedTrangThaiBaoCao == null || this.selectedLoaiTSBaoCao == null || this.selectedTrangThaiBaoCao.length == 0 || this.selectedLoaiTSBaoCao.length == 0) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Hãy chọn đầy đủ thông tin bắt buộc!' };
      this.showMessage(msg);
      return;
    }
    this.viewbaoCao = 3;
    // Thời gian tính KH
    // this.thoiGianTinhKhauHaoDen = this.baoCaoKhauHaoForm.get('ngayTinhKhauHaoDenControl').value;
    this.searchAsset(3);
    this.displayKhauHao = false;
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo?.systemValueString;
  }

  getListExcelColumns(): Array<columnModel> {
    let listColumns: Array<columnModel> = [];
    let columGroup_0 = new columnModel();
    columGroup_0.column1 = "";

    let columGroup_1 = new columnModel();
    columGroup_1.column1 = "Mã tài sản";

    let columGroup_2 = new columnModel();
    columGroup_2.column1 = "Tên tài sản";

    let columGroup_3 = new columnModel();
    columGroup_3.column1 = "Ngày vào sổ";

    let columGroup_TrangThai = new columnModel();
    columGroup_TrangThai.column1 = "Trạng thái";

    if (this.viewbaoCao == 1) {
      listColumns = [columGroup_0, columGroup_1, columGroup_2, columGroup_3, columGroup_TrangThai];
    }
    if (this.viewbaoCao == 2) {

      let columGroup_4 = new columnModel();
      columGroup_4.column1 = "Mã NV sử dụng";

      let columGroup_5 = new columnModel();
      columGroup_5.column1 = "Họ tên";

      let columGroup_6 = new columnModel();
      columGroup_6.column1 = "Phòng ban";

      let columGroup_7 = new columnModel();
      columGroup_7.column1 = "Vị trí việc làm";

      let columGroup_8 = new columnModel();
      columGroup_8.column1 = "Mô tả";

      listColumns = [columGroup_0, columGroup_1, columGroup_2, columGroup_3, columGroup_4
        , columGroup_5, columGroup_6, columGroup_7, columGroup_8
      ];
    }
    if (this.viewbaoCao == 3) {
      let columGroup_18 = new columnModel();
      columGroup_18.column1 = "Loại tài sản";

      let columGroup_9 = new columnModel();
      columGroup_9.column1 = "Nguyên giá";

      let columGroup_10 = new columnModel();
      columGroup_10.column1 = "Giá trị tính khấu hao";

      let columGroup_11 = new columnModel();
      columGroup_11.column1 = "Tỉ lệ khấu hao theo tháng (%)";

      let columGroup_12 = new columnModel();
      columGroup_12.column1 = "Tỉ lệ khấu hao theo năm (%)";

      let columGroup_13 = new columnModel();
      columGroup_13.column1 = "Giá trị khấu hao theo tháng";

      let columGroup_14 = new columnModel();
      columGroup_14.column1 = "Giá trị khấu hao theo năm";

      let columGroup_15 = new columnModel();
      columGroup_15.column1 = "Thời gian tính khấu hao đến";

      let columGroup_16 = new columnModel();
      columGroup_16.column1 = "Giá trị khấu hao lũy kế";

      let columGroup_17 = new columnModel();
      columGroup_17.column1 = "Giá trị còn lại";

      listColumns = [columGroup_0, columGroup_1, columGroup_2, columGroup_3, columGroup_18, columGroup_9
        , columGroup_10, columGroup_11, columGroup_12, columGroup_13, columGroup_14
        , columGroup_15, columGroup_16, columGroup_17
      ];
    }

    return listColumns;
  }

  getDataExportExcel(listAsset: Array<TaiSanModel>) {
    let result = [];
    if (this.viewbaoCao == 1) {
      listAsset?.forEach(asset => {
        let newItem = new exportExcelModel();
        newItem.maTaiSan = asset.maTaiSan;
        newItem.tenTaiSan = asset.tenTaiSan;
        newItem.hienTrangTaiSanString = asset.hienTrangTaiSanString;
        newItem.ngayVaoSo = this.datePipe.transform(asset.ngayVaoSo, 'dd/MM/yyyy');
        result = [...result, newItem];
      });
    }
    if (this.viewbaoCao == 2) {
      listAsset?.forEach(asset => {
        let newItem = new exportExcelModel();
        newItem.maTaiSan = asset.maTaiSan;
        newItem.tenTaiSan = asset.tenTaiSan;
        // newItem.hienTrangTaiSanString = asset.hienTrangTaiSanString;
        newItem.ngayVaoSo = this.datePipe.transform(asset.ngayVaoSo, 'dd/MM/yyyy');

        newItem.maNV = asset.maNV;
        newItem.hoVaTen = asset.hoVaTen;
        newItem.phongBan = asset.phongBan;
        newItem.viTriLamViec = asset.viTriLamViec;
        newItem.moTa = asset.moTa;
        result = [...result, newItem];
      });
    }
    if (this.viewbaoCao == 3) {
      listAsset?.forEach(asset => {
        let newItem = new exportExcelModel();
        newItem.maTaiSan = asset.maTaiSan;
        newItem.tenTaiSan = asset.tenTaiSan;
        newItem.phanLoaiTaiSan = asset.phanLoaiTaiSan;
        newItem.ngayVaoSo = this.datePipe.transform(asset.ngayVaoSo, 'dd/MM/yyyy');
        newItem.giaTriNguyenGia = Intl.NumberFormat().format(asset.giaTriNguyenGia).toString();
        newItem.giaTriTinhKhauHao = Intl.NumberFormat().format(asset.giaTriTinhKhauHao).toString();

        newItem.tiLeKhauHaoTheoThang = asset.tiLeKhauHaoTheoThang ? Intl.NumberFormat().format(asset.tiLeKhauHaoTheoThang).toString() : "";
        newItem.tiLeKhauHaoTheoNam = asset.tiLeKhauHaoTheoNam ? Intl.NumberFormat().format(asset.tiLeKhauHaoTheoNam).toString() : "";
        newItem.giaTriKhauHaoTheoThang = asset.giaTriKhauHaoTheoThang ? Intl.NumberFormat().format(asset.giaTriKhauHaoTheoThang).toString() : "";
        newItem.giaTriKhauHaoTheoNam = asset.giaTriKhauHaoTheoNam ? Intl.NumberFormat().format(asset.giaTriKhauHaoTheoNam).toString() : "";
        newItem.giaTriKhauHaoLuyKe = asset.giaTriKhauHaoLuyKe ? Intl.NumberFormat().format(asset.giaTriKhauHaoLuyKe).toString() : "";
        newItem.giaTriConLai = asset.giaTriConLai ? Intl.NumberFormat().format(asset.giaTriConLai).toString() : "";
        newItem.thoiGianTinhKhauHaoDen = this.datePipe.transform(asset.thoiGianTinhKhauHaoDen, 'dd/MM/yyyy');
        result = [...result, newItem];
      });
    }

    return result;
  }

  exportExcel() {
    var datePipe = new DatePipe("en-US");
    let title = `Danh sách tài sản`;
    if (this.viewbaoCao == 2)
      title = `Báo cáo phân bổ tài sản`;
    if (this.viewbaoCao == 3)
      title = `Báo cáo khấu hao tài sản`;

    let workBook = new Workbook();
    let worksheet = workBook.addWorksheet(title);

    //LOGO - infor section
    let imgBase64 = this.getBase64Logo();
    var imgLogo = workBook.addImage({
      base64: imgBase64,
      extension: 'png',
    });
    worksheet.addImage(imgLogo, {
      tl: { col: 0.5, row: 0.5 },
      ext: { width: 140, height: 95 }
    });

    let dataInforRow_1 = `${this.companyConfig.companyName}`
    let dataInforRow_2 = `Địa chỉ: ${this.companyConfig.companyAddress}`;
    let dataInforRow_3 = `Điện thoại: ${this.companyConfig.phone}`;
    let dataInforRow_4 = `Email: ${this.companyConfig.email}`;
    let dataInforRow_5 = `Website dịch vụ: `

    let inforRow_1 = worksheet.addRow(['', '', dataInforRow_1]);
    let inforRow_2 = worksheet.addRow(['', '', dataInforRow_2]);
    let inforRow_3 = worksheet.addRow(['', '', dataInforRow_3]);
    let inforRow_4 = worksheet.addRow(['', '', dataInforRow_4]);
    let inforRow_5 = worksheet.addRow(['', '', dataInforRow_5]);

    inforRow_1.font = { size: 14, bold: true };
    worksheet.mergeCells(`C${1}:E${1}`);
    worksheet.mergeCells(`C${2}:${2}`);
    worksheet.mergeCells(`C${3}:E${3}`);
    worksheet.mergeCells(`C${4}:E${4}`);
    worksheet.mergeCells(`C${5}:E${5}`);

    worksheet.addRow([]);
    /* title */
    let dataHeaderMain = "Danh sách tài sản".toUpperCase();

    if (this.viewbaoCao == 2)
      dataHeaderMain = "Báo cáo phân bổ tài sản".toUpperCase();
    if (this.viewbaoCao == 3)
      dataHeaderMain = "Báo cáo khấu hao tài sản".toUpperCase();

    let headerMain = worksheet.addRow([dataHeaderMain]);
    headerMain.font = { size: 18, bold: true };
    if (this.viewbaoCao == 1)
      worksheet.mergeCells(`A${7}:G${7}`);

    if (this.viewbaoCao == 2)
      worksheet.mergeCells(`A${7}:I${7}`);

    if (this.viewbaoCao == 3)
      worksheet.mergeCells(`A${7}:N${7}`);

    headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    worksheet.addRow([]);

    let listExcelColumns = this.getListExcelColumns();
    let dataHeaderRow1: Array<string> = listExcelColumns.map(e => e.column1);
    let headerRow1 = worksheet.addRow(dataHeaderRow1);
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };

    //merge header column
    worksheet.mergeCells(`A${9}:A${9}`);
    worksheet.mergeCells(`B${9}:B${9}`);
    worksheet.mergeCells(`C${9}:C${9}`);
    worksheet.mergeCells(`D${9}:D${9}`);
    worksheet.mergeCells(`E${9}:E${9}`);
    if (this.viewbaoCao == 2 || this.viewbaoCao == 3) {
      worksheet.mergeCells(`F${9}:F${9}`);
      worksheet.mergeCells(`G${9}:G${9}`);
      worksheet.mergeCells(`H${9}:H${9}`);
      worksheet.mergeCells(`I${9}:I${9}`);
      if (this.viewbaoCao == 3) {
        worksheet.mergeCells(`K${9}:K${9}`);
        worksheet.mergeCells(`L${9}:L${9}`);
        worksheet.mergeCells(`M${9}:M${9}`);
        worksheet.mergeCells(`N${9}:N${9}`);
        worksheet.mergeCells(`O${9}:O${9}`);
        worksheet.mergeCells(`P${9}:P${9}`);
        worksheet.mergeCells(`Q${9}:Q${9}`);
        worksheet.mergeCells(`R${9}:R${9}`);
      }
    }

    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
    dataHeaderRow1.forEach((item, index) => {
      if (index + 2 < dataHeaderRow1.length + 1) {
        headerRow1.getCell(index + 2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow1.getCell(index + 2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow1.getCell(index + 2).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      }
    });

    let data: Array<any> = [];

    let dataExportExcel: Array<TaiSanModel> = this.getDataExportExcel(this.listAsset);

    if (this.viewbaoCao == 1) {
      dataExportExcel?.forEach((item, index) => {
        let row: Array<any> = [];
        row[0] = '';
        row[1] = item.maTaiSan;
        row[2] = item.tenTaiSan;
        row[3] = item.ngayVaoSo;
        row[4] = item.hienTrangTaiSanString;
        data.push(row);
      });

      data.forEach((el, index, array) => {
        let row = worksheet.addRow(el);
        row.font = { name: 'Times New Roman', size: 11 };

        row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

        row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(5).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      });

      /* fix with for column */
      worksheet.getColumn(1).width = 15;
      worksheet.getColumn(2).width = 15;
      worksheet.getColumn(3).width = 40;
      worksheet.getColumn(4).width = 20;
      worksheet.getColumn(5).width = 30;
    }

    if (this.viewbaoCao == 2) {

      dataExportExcel?.forEach((item, index) => {
        let row: Array<any> = [];
        row[0] = '';
        row[1] = item.maTaiSan;
        row[2] = item.tenTaiSan;
        row[3] = item.ngayVaoSo;
        row[4] = item.maNV;
        row[5] = item.hoVaTen;
        row[6] = item.phongBan;
        row[7] = item.viTriLamViec;
        row[8] = item.moTa;
        data.push(row);
      });

      data.forEach((el, index, array) => {
        let row = worksheet.addRow(el);
        row.font = { name: 'Times New Roman', size: 11 };

        row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

        row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(5).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

        row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(6).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        row.getCell(7).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(7).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        row.getCell(8).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(8).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        row.getCell(9).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(9).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      });

      /* fix with for column */
      worksheet.getColumn(1).width = 15;
      worksheet.getColumn(2).width = 15;
      worksheet.getColumn(3).width = 40;
      worksheet.getColumn(4).width = 15;
      worksheet.getColumn(5).width = 15;
      worksheet.getColumn(6).width = 30;
      worksheet.getColumn(7).width = 30;
      worksheet.getColumn(8).width = 15;
      worksheet.getColumn(9).width = 30;

    }

    // Báo cáo khấu hao
    if (this.viewbaoCao == 3) {
      dataExportExcel?.forEach((item, index) => {
        let row: Array<any> = [];
        row[0] = '';
        row[1] = item.maTaiSan;
        row[2] = item.tenTaiSan;
        row[3] = item.ngayVaoSo;
        row[4] = item.phanLoaiTaiSan;
        row[5] = item.giaTriNguyenGia;
        row[6] = item.giaTriTinhKhauHao;
        row[7] = item.tiLeKhauHaoTheoThang;
        row[8] = item.tiLeKhauHaoTheoNam;
        row[9] = item.giaTriKhauHaoTheoThang;
        row[10] = item.giaTriKhauHaoTheoNam;
        row[11] = item.thoiGianTinhKhauHaoDen;
        row[12] = item.giaTriKhauHaoLuyKe;
        row[13] = item.giaTriConLai;
        data.push(row);
      });

      data.forEach((el, index, array) => {
        let row = worksheet.addRow(el);
        row.font = { name: 'Times New Roman', size: 11 };

        row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(5).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

        row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

        row.getCell(7).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

        row.getCell(8).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

        row.getCell(9).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

        row.getCell(10).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

        row.getCell(11).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

        row.getCell(12).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(12).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

        row.getCell(13).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(13).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

        row.getCell(14).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(14).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      });

      /* fix with for column */
      worksheet.getColumn(1).width = 15;
      worksheet.getColumn(2).width = 15;
      worksheet.getColumn(3).width = 40;
      worksheet.getColumn(4).width = 20;
      worksheet.getColumn(5).width = 15;
      worksheet.getColumn(6).width = 15;
      worksheet.getColumn(7).width = 15;
      worksheet.getColumn(8).width = 15;
      worksheet.getColumn(9).width = 15;
      worksheet.getColumn(10).width = 15;
      worksheet.getColumn(11).width = 15;
      worksheet.getColumn(12).width = 15;
      worksheet.getColumn(13).width = 15;

    }

    this.exportToExel(workBook, title);

  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  onpenDialogChoseFileExcel() {
    this.displayChooseFileImportDialog = true;
  }

  async downloadTemplateExcel() {
    this.loading = true;
    let result: any = await this.assetService.downloadTemplateImportAsset();
    this.loading = false;
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
      this.showMessage(msg);
    }
  }

  inNhan() {
    //this.loading = true;
    // if (this.selection.selected.length > 0) {
    //   this.listQRCode = this.selection.selected;
    // }
    console.log(this.listAssetSelected);

    let logo_evn =
      "data:image/png;base64,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";

    let list_card: Array<icard_infor> = [];
    this.listAssetSelected.forEach((item) => {
      let card = {
        account: item.account ?? "",
        dept: item.dept ?? "",
        serial: item.soSerial,
        qr_code: item.taiSanId.toString(),
        moTa: item.moTa,
        maTaiSan: item.maTaiSan,
      };

      list_card.push(card);
    });

    let documentDefinition: any = {
      pageSize: "A4",
      pageMargins: [10, 20, 20, 20],
      pageOrientation: "landscape",
      content: [],
      styles: {
        table: {
          margin: [15, 15, 15, 15],
        },
        title_header: {
          fontSize: 10,
          bold: true,
          margin: [0, 15, 0, 5],
        },
        sub_title_header: {
          fontSize: 10,
          bold: true,
        },
        account: {
          fontSize: 10,
          bold: true,
        },
        dept: {
          fontSize: 10,
          bold: true,
        },
        serial: {
          fontSize: 10,
          bold: true,
        },
        qrcode: {
          fontSize: 10,
          bold: true,
          alignment: "center",
        },
        moTa: {
          fontSize: 10,
          alignment: "left",
        },
      },
    };

    list_card.forEach((card, key) => {
      let stack_option: any = {
        style: "table",
        table: {
          headerRows: 1,
          widths: [250],
          body: [
            [
              {
                width: 250,
                stack: [
                  {
                    columns: [
                      {
                        stack: [
                          {
                            image: logo_evn,
                            width: 150,
                            height: 42,
                          },
                        ],
                      },
                    ],
                  },
                  {
                    stack: [
                      {
                        columns: [
                          {
                            width: 150,
                            stack: [
                              {
                                text: "", //card_infor.account
                                style: "account",
                                margin: [0, 5, 0, 0],
                              },
                              {
                                text: "", //card_infor.dept
                                style: "dept",
                                margin: [0, 5, 0, 0],
                              },
                              {
                                text: "", //card_infor.serial
                                style: "serial",
                                margin: [0, 5, 0, 0],
                              },
                              {
                                text: "", //card_infor.moTa
                                style: "moTa",
                                margin: [0, 5, 0, 10],
                              },
                            ],
                          },
                          {
                            width: 80,
                            stack: [
                              {
                                qr: "", //card_infor.qr_code
                                fit: "70",
                                margin: [0, -30, 0, 0],
                              },
                              {
                                text: "", //card_infor.qr_code
                                margin: [0, 5, 0, 0],
                                style: "qrcode",
                                align: 'center'
                              },
                            ],
                          },
                        ],
                        columnGap: 30,
                        margin: [0, 0, 0, 0],
                      },
                    ],
                  },
                ],
              },
            ],
          ],
        },
        layout: {
          defaultBorder: true,
          paddingTop: function (i, node) {
            return 1;
          },
          paddingBottom: function (i, node) {
            return 1;
          },
        },
      };

      stack_option.table.body[0][0].stack[1].stack[0].columns[0].stack[0].text =
        "Account: " + card.account;
      stack_option.table.body[0][0].stack[1].stack[0].columns[0].stack[1].text =
        "Dept: " + card.dept;
      stack_option.table.body[0][0].stack[1].stack[0].columns[0].stack[2].text =
        "Serial: " + card.serial;
      stack_option.table.body[0][0].stack[1].stack[0].columns[0].stack[3].text =
        card.moTa;

      stack_option.table.body[0][0].stack[1].stack[0].columns[1].stack[0].qr =
        card.qr_code;
      stack_option.table.body[0][0].stack[1].stack[0].columns[1].stack[1].text =
        card.maTaiSan;

      if (key % 3 == 0) {
        let sub_column: any = {
          width: 250,
          stack: [],
        };
        sub_column.stack.push(stack_option);
        let column_otpion: any = {
          columns: [],
          columnGap: 20,
        };
        column_otpion.columns.push(sub_column);
        documentDefinition.content.push(column_otpion);
      } else {
        let sub_column: any = {
          width: 250,
          stack: [],
        };
        sub_column.stack.push(stack_option);
        //console.log(sub_column)
        let length = documentDefinition.content.length;
        documentDefinition.content[length - 1].columns.push(sub_column);
      }
    });

    pdfMake.createPdf(documentDefinition).open();
    this.listAssetSelected = [];
  }


  chooseFile(event: any) {
    this.fileName = event.target?.files[0]?.name;
    this.importFileExcel = event.target;
  }

  importExcel() {
    if (this.fileName == "") {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Chưa chọn file cần nhập" };
      this.showMessage(mgs);
      return;
    }

    const targetFiles: DataTransfer = <DataTransfer>(this.importFileExcel);
    const reader: FileReader = new FileReader();
    reader.readAsBinaryString(targetFiles.files[0]);
    reader.onload = (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });
      let code = 'Nhap_Tai_San';
      if (!workbook.Sheets[code]) {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: "File không hợp lệ" };
        this.showMessage(mgs);
        return;
      }

      //lấy data từ file excel
      const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[code];
      /* save data */
      let listTaiSanRawData: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
      /* remove header */
      listTaiSanRawData = listTaiSanRawData.filter((e, index) => index != 0);
      /* nếu không nhập 2 trường required: tên + mã khách hàng thì loại bỏ */
      listTaiSanRawData = listTaiSanRawData.filter(e => (e[1] && e[4]));
      /* chuyển từ raw data sang model */
      let listTaiSanRawImport: Array<importTaiSanModel> = [];

      listTaiSanRawData?.forEach(_rawData => {
        let customer = new importTaiSanModel();
        customer['maTaiSan'] = _rawData[0] ? _rawData[0].toString().trim() : '';
        customer.tenTaiSan = _rawData[1] ? _rawData[1].toString().trim() : '';
        customer.phanLoaiTaiSanCode = _rawData[2] ? _rawData[2].toString().trim() : '';
        customer.donViTinhCode = _rawData[3] ? _rawData[3].toString().trim() : '';
        customer.khuVucTaiSanCode = _rawData[4] ? _rawData[4].toString().trim() : '';
        customer.ngayVaoSo = _rawData[5] ? new Date(_rawData[5].toString().trim()) : null;
        customer.moTa = _rawData[6] ? _rawData[6].toString().trim() : '';
        customer.soSerial = _rawData[7] ? _rawData[7].toString().trim() : '';
        customer.model = _rawData[8] ? _rawData[8].toString().trim() : '';
        customer.soHieu = _rawData[9] ? _rawData[9].toString().trim() : '';
        customer.thongTinNoiMua = _rawData[10] ? _rawData[10].toString().trim() : '';
        customer.namSx = _rawData[11] ? _rawData[11].toString().trim() : '';
        customer.nuocSxCode = _rawData[12] ? _rawData[12].toString().trim() : '';
        customer.hangSxCode = _rawData[13] ? _rawData[13].toString().trim() : '';
        customer.ngayMua = _rawData[14] ? new Date(_rawData[14].toString().trim()) : null;
        customer.thoiHanBaoHanh = _rawData[15] ? parseInt(_rawData[15].toString().trim()) : null;
        customer.baoDuongDinhKy = _rawData[16] ? parseInt(_rawData[16].toString().trim()) : null;
        customer.thongTinNoiBaoHanh = _rawData[17] ? _rawData[17].toString().trim() : '';
        customer.giaTriNguyenGia = _rawData[18] ? _rawData[18].toString().trim() : '';
        customer.giaTriTinhKhauHao = _rawData[19] ? _rawData[19].toString().trim() : '';
        customer.thoiGianKhauHao = _rawData[20] ? _rawData[20].toString().trim() : '';
        customer.thoiDiemBdtinhKhauHao = _rawData[21] ? new Date(_rawData[21].toString().trim()) : null;
        listTaiSanRawImport = [...listTaiSanRawImport, customer];
      });
      /* tắt dialog import file, bật dialog chi tiết khách hàng import */
      this.displayChooseFileImportDialog = false;
      this.openDetailImportDialog(listTaiSanRawImport);
    }
  }

  openDetailImportDialog(listTaiSanRawImport) {
    let ref = this.dialogService.open(AssetImportDetailComponent, {
      data: {
        listTaiSanImport: listTaiSanRawImport
      },
      header: 'Import tài sản',
      width: '85%',
      baseZIndex: 1050,
      contentStyle: {
        "max-height": "800px",
        // "over-flow": "hidden"
      }
    });
    ref.onClose.subscribe((result: any) => {
      if (result?.status) {
        this.getMasterData();
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

}

function monthDiff(d1, d2) {
  var months;
  months = (d2.getFullYear() - d1.getFullYear()) * 12;
  months -= d1.getMonth();
  months += d2.getMonth();
  return months <= 0 ? 0 : months;
}

function getCountMonth(startDate, endDate) {
  if (startDate == null || startDate == undefined)
    return 0;
  else
    return (
      endDate.getMonth() -
      startDate.getMonth() +
      12 * (endDate.getFullYear() - startDate.getFullYear())
    );
}
