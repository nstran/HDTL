import * as $ from 'jquery';
import { Component, OnInit, ViewChild, ElementRef, HostListener, Renderer2 } from '@angular/core';
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
import { columnModel } from '../../models/bao-cao.model';
import { companyConfigModel } from '../../models/bao-cao.model';
import { baoCaoKhauHaoModel } from '../../models/bao-cao.model';
import { Table } from 'primeng/table';
import { saveAs } from "file-saver";


@Component({
  selector: 'app-asset-list',
  templateUrl: './bao-cao-khau-hao.component.html',
  styleUrls: ['./bao-cao-khau-hao.component.css'],
  providers: [
    DatePipe,
  ]
})
export class BaoCaoKhauHaoComponent implements OnInit {
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

  listAsset: Array<baoCaoKhauHaoModel> = []; // Danh sách tài sản
  listBaoCaoPhanBoAll: Array<baoCaoKhauHaoModel> = [];
  taiSanDetail: TaiSanModel = new TaiSanModel();
  selectedColumns: any[];
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  isUpdate: boolean = false;
  isDisplayName: boolean = false;

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
  listHienTrangTS = [
    {
      id: 1, name: 'Đang sử dụng'
    },
    {
      id: 0, name: 'Không sử dụng'
    }];
  listPhanLoaiTaiSan: any[];
  listOrganization: any[];
  listEmployee: any[];
  /* Form searcg */
  searchAssetForm: FormGroup;
  phanLoaiTaiSanControl: FormControl;
  thoiDiemKTKhauHaoControl: FormControl;
  hienTrangTaiSanControl: FormControl;

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
    this.setForm();
    this.setTable();
    let resource = "ass/asset/bao-cao-khau-hao/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;

      if (listCurrentActionResource.indexOf("view") == -1) {
        let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
        this.showMessage(mgs);
        this.router.navigate(['/home']);
      }

      this.getMasterData();
      this.setDefaultPaidDate();
      this.isShowQRCode = false;
    }
  }

  setForm() {
    this.phanLoaiTaiSanControl = new FormControl(null);

    this.thoiDiemKTKhauHaoControl = new FormControl(null);
    this.hienTrangTaiSanControl = new FormControl(null); // Hiện trạng tài sản
    this.searchAssetForm = new FormGroup({
      phanLoaiTaiSanControl: this.phanLoaiTaiSanControl,
      thoiDiemKTKhauHaoControl: this.thoiDiemKTKhauHaoControl,
      hienTrangTaiSanControl: this.hienTrangTaiSanControl
    });
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
      { field: 'maTaiSan', header: 'Mã tài sản', width: '100px', textAlign: 'left', color: '#f44336' },
      { field: 'tenTaiSan', header: 'Tên tài sản', width: '250px', textAlign: 'left', color: '#f44336' },
      { field: 'loaiTaiSanStr', header: 'Loại tài sản', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'ngayVaoSo', header: 'Ngày vào sổ', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'giaTriNguyenGia', header: 'Nguyên giá', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'giaTriTinhKhauHao', header: 'Giá trị tính khấu hao', width: '150px', textAlign: 'center', color: '#f44336' },
      { field: 'tiLeKhauHaoTheoThang', header: 'TL khấu hao theo tháng (%)', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'tiLeKhauHaoTheoNam', header: 'Tỷ lệ khấu hao theo năm (%)', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'giaTriKhauHaoTheoThang', header: 'Giá trị khấu hao theo tháng', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'giaTriKhauHaoTheoNam', header: 'Giá trị khấu hao theo năm', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'thoiDiemKTKhauHao', header: 'Thời gian tính khấu hao đến', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'giaTriKhauHaoLuyKe', header: 'Giá trị khấu hao lũy kế', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'giaTriConLai', header: 'Giá trị còn lai', width: '100px', textAlign: 'center', color: '#f44336' },
    ];
    this.selectedColumns = this.colsList;
  }

  async getMasterData() {
    this.loading = true;
    let listPhanLoaiTaiSanId = [];
    let thoigianKhauHaoDen: Date = null;
    let listHienTrang = [];
    let result: any = await this.assetService.baoCaoKhauHao(thoigianKhauHaoDen, listPhanLoaiTaiSanId, listHienTrang);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listAsset = result.listAsset;
      this.listBaoCaoPhanBoAll = result.listAsset;
      this.listPhanLoaiTaiSan = result.listPhanLoaiTaiSan;
      this.resetTable(); //reset state of table
      if (this.listAsset.length == 0) {
        let mgs = { severity: 'warn', summary: 'Thông báo', detail: 'Không tìm thấy tài sản nào!' };
        this.showMessage(mgs);
      }
    }
  }

  checkEnterPress(event: any) {
    if (event.code === "Enter") {

    }
  }

  refreshFilter() {
    this.filterGlobal = '';
    this.isShowQRCode = false;
    this.isShowDetail = false;
    this.leftColGridNumber = 12;
    this.searchAssetForm.reset();
  }

  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.leftColGridNumber = 12;
        this.rightColNumber = 4;
        this.isShowDetail = false;
      }
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  // Tìm kiếm tài sản
  async searchAsset() {
    this.loading = true;
    // Cần xử lý theo kiểu này để tối ưu hơn
    // let lstEmpId = this.employeeNameControl.value ? this.employeeNameControl.value.map(x => x.employeeId) : [];
    // let listPhanLoaiTaiSanId=this.phanLoaiTaiSanControl.value? this.phanLoaiTaiSanControl.value.map(x=>x.categoryId):[];
    // let listOrganizationId=this.organizationControl.value? this.organizationControl.value.map(x=>x.organizationId):[];
    // let listHienTrangTaiSan=this.hienTrangTaiSanControl.value? this.hienTrangTaiSanControl.value.map(x=>x.id):[];
    // let listEmployeeId=this.employeeControl.value? this.employeeControl.value.map(x=>x.employeeId):[];

    let thoigianKhauHaoDen: Date = null;
    let listPhanLoaiTaiSanId: Array<string> = [];
    let listHienTrang: Array<number> = [];
    // Xử lý kiểu củ chuối cho kịp deadline
    if ((this.phanLoaiTaiSanControl.value !== null) && this.phanLoaiTaiSanControl.value.length > 0) {
      this.phanLoaiTaiSanControl.value.forEach(item => {
        listPhanLoaiTaiSanId.push(item.categoryId);
      });
    }
    if ((this.hienTrangTaiSanControl.value !== null) && this.hienTrangTaiSanControl.value.length > 0) {
      this.hienTrangTaiSanControl.value.forEach(item => {
        listHienTrang.push(item.id);
      });
    }
    thoigianKhauHaoDen = this.thoiDiemKTKhauHaoControl.value;
    let result: any = await this.assetService.baoCaoKhauHao(thoigianKhauHaoDen, listPhanLoaiTaiSanId, listHienTrang);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listAsset = result.listAsset;
      this.resetTable(); //reset state of table
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
    var qrCodeAsset = this.taiSanDetail.maTaiSan;
    this.qrCode = qrCodeAsset;
    this.taiSanId = rowData.taiSanId;

    this.leftColGridNumber = 8;
    this.isShowDetail = true
  }
  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo ?.systemValueString;
  }

  // Tạo tiêu đề cho các cột
  getListExcelColumns(): Array<columnModel> {
    let listColumns: Array<columnModel> = [];
    let columGroup_0 = new columnModel();
    columGroup_0.column1 = "STT";
    let columGroup_1 = new columnModel();
    columGroup_1.column1 = "Mã tài sản";
    let columGroup_2 = new columnModel();
    columGroup_2.column1 = "Tên tài sản";
    let columGroup_3 = new columnModel();
    columGroup_3.column1 = "Loại tài sản";
    let columGroup_4 = new columnModel();
    columGroup_4.column1 = "Ngày vào sổ";
    let columGroup_5 = new columnModel();
    columGroup_5.column1 = "Nguyên giá";
    let columGroup_6 = new columnModel();
    columGroup_6.column1 = "Giá trị tính khấu hao";
    let columGroup_7 = new columnModel();
    columGroup_7.column1 = "TL khấu hao theo tháng (%)";
    let columGroup_8 = new columnModel();
    columGroup_8.column1 = "Tỷ lệ khấu hao theo năm (%)";
    let columGroup_9 = new columnModel();
    columGroup_9.column1 = "Giá trị khấu hao theo tháng";
    let columGroup_10 = new columnModel();
    columGroup_10.column1 = "Giá trị khấu hao theo năm";
    let columGroup_11 = new columnModel();
    columGroup_11.column1 = "Thời gian tính khấu hao đến";
    let columGroup_12 = new columnModel();
    columGroup_12.column1 = "Giá trị khấu hao lũy kế";
    let columGroup_13 = new columnModel();
    columGroup_13.column1 = "Giá trị còn lai";
    listColumns = [columGroup_0, columGroup_1, columGroup_2, columGroup_3, columGroup_4, columGroup_5
      , columGroup_6, columGroup_7, columGroup_8, columGroup_9, columGroup_10, columGroup_11
      , columGroup_12, columGroup_13];

    return listColumns;
  }
  //Map dữ liệu từ dữ liệu lấy về vào model để xuất excel
  getDataExportExcel(listAsset: Array<baoCaoKhauHaoModel>) {
    let result = [];
    listAsset ?.forEach(asset => {
      let newItem = new baoCaoKhauHaoModel();
      newItem.maTaiSan = asset.maTaiSan;
      newItem.tenTaiSan = asset.tenTaiSan;
      newItem.loaiTaiSanStr = asset.loaiTaiSanStr;
      newItem.ngayVaoSo = this.datePipe.transform(asset.ngayVaoSo, 'dd/MM/yyyy');
      newItem.giaTriNguyenGia = asset.giaTriNguyenGia;
      newItem.giaTriTinhKhauHao = asset.giaTriTinhKhauHao;
      newItem.tiLeKhauHaoTheoThang = asset.tiLeKhauHaoTheoThang;
      newItem.tiLeKhauHaoTheoNam = asset.tiLeKhauHaoTheoNam;
      newItem.giaTriKhauHaoTheoThang = asset.giaTriKhauHaoTheoThang;
      newItem.giaTriKhauHaoTheoNam = asset.giaTriKhauHaoTheoNam;
      newItem.thoiDiemKTKhauHao = this.datePipe.transform(asset.thoiDiemKTKhauHao, 'dd/MM/yyyy');
      newItem.giaTriKhauHaoLuyKe = asset.giaTriKhauHaoLuyKe;
      newItem.giaTriConLai = asset.giaTriConLai;

      result = [...result, newItem];
    });
    return result;
  }

  exportExcel() {
    let title = `Báo cáo khấu hao tài sản`;

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
    let dataHeaderMain = "Báo cáo khấu hao tài sản".toUpperCase();
    let headerMain = worksheet.addRow([dataHeaderMain]);
    headerMain.font = { size: 18, bold: true };
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
    // Format tiêu đề của bảng dữ liệu
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
    dataHeaderRow1.forEach((item, index) => {
      if (index + 1 < dataHeaderRow1.length + 1) {
        headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow1.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      }
    });

    let data: Array<any> = [];
    let dataExportExcel: Array<baoCaoKhauHaoModel> = this.getDataExportExcel(this.listAsset);
    // Gán dữ liệu cho các cột
    dataExportExcel ?.forEach((item, index) => {
      let row: Array<any> = [];
      row[0] = index + 1;
      row[1] = item.maTaiSan;
      row[2] = item.tenTaiSan;
      row[3] = item.loaiTaiSanStr;
      row[4] = item.ngayVaoSo;
      row[5] = item.giaTriNguyenGia;
      row[6] = item.giaTriTinhKhauHao;
      row[7] = item.tiLeKhauHaoTheoThang;
      row[8] = item.tiLeKhauHaoTheoNam;
      row[9] = item.giaTriKhauHaoTheoThang;
      row[10] = item.giaTriKhauHaoTheoNam;
      row[11] = item.thoiDiemKTKhauHao;
      row[12] = item.giaTriKhauHaoLuyKe;
      row[13] = item.giaTriConLai;

      data.push(row);
    });

    //Format các cột. Có time cần tối ưu lại đoạn này
    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman', size: 11 };

      row.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

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
    worksheet.getColumn(1).width = 10;
    worksheet.getColumn(2).width = 15;
    worksheet.getColumn(3).width = 20;
    worksheet.getColumn(4).width = 20;
    worksheet.getColumn(5).width = 15;
    worksheet.getColumn(6).width = 10;
    worksheet.getColumn(7).width = 10;
    worksheet.getColumn(8).width = 10;
    worksheet.getColumn(9).width = 10;
    worksheet.getColumn(10).width = 10;
    worksheet.getColumn(11).width = 10;
    worksheet.getColumn(12).width = 15;
    worksheet.getColumn(13).width = 10;
    worksheet.getColumn(13).width = 10;

    this.exportToExel(workBook, title);

  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

}
function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
      }
    }
  }
  return null;
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};


function ExcelDateToJSDate(serial) {
  var utc_days = Math.floor(serial - 25569);
  var utc_value = utc_days * 86400;
  var date_info = new Date(utc_value * 1000);

  var fractional_day = serial - Math.floor(serial) + 0.0000001;

  var total_seconds = Math.floor(86400 * fractional_day);

  var seconds = total_seconds % 60;

  total_seconds -= seconds;

  var hours = Math.floor(total_seconds / (60 * 60));
  var minutes = Math.floor(total_seconds / 60) % 60;

  return new Date(date_info.getFullYear(), date_info.getMonth(), date_info.getDate(), hours, minutes, seconds);
}
function ParseStringToFloat(str: any) {
  if (str === "") return 0;
  str = str.toString().replace(/,/g, '');
  return parseFloat(str);
}
