import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { Row, Workbook, Worksheet } from 'exceljs';
import { saveAs } from "file-saver";
import { FormControl, FormGroup, Validators } from '@angular/forms';
import * as XLSX from 'xlsx';
import { DialogService, FileUpload } from 'primeng';
import { EncrDecrService } from '../../../../../src/app/shared/services/encrDecr.service';
import { AssetService } from '../../services/asset.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-dot-kiem-ke-detail',
  templateUrl: './dot-kiem-ke-detail.component.html',
  styleUrls: ['./dot-kiem-ke-detail.component.css']
})

export class DotKiemKeDetailComponent implements OnInit {
  auth = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  filterGlobal: string = '';
  displayChooseFileImportDialog: boolean = false;
  fileName: string = '';
  selectedColumns: any[];
  cols: any[];
  listTaiSan: any[] = [];
  today = new Date();

  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  leftColNumber: number = 12;
  rightColNumber: number = 0;

  //Form của đợt kiểm kê
  dotKiemKeFormGroup: FormGroup;
  tenDotKiemKeControl: FormControl;
  ngayBatDauControl: FormControl;
  ngayKetThucControl: FormControl;

  loginEmpId: string = '';
  dotKiemKeId: number = 0;

  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  companyConfig: any; // Thông tin vê cty
  isShowWorkFollowContract: boolean = true;
  emptyGuid = '00000000-0000-0000-0000-000000000000';
  defaultAvatar: string = '/assets/images/no-avatar.png';
  @ViewChild('fileNoteUpload') fileNoteUpload: FileUpload;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  @ViewChild('myTable') myTable: Table;

  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionDownLoad: boolean = true;
  actionUpLoad: boolean = true;
  isManagerOfHR: boolean = false;
  isGD: boolean = false;
  viewNote: boolean = true;
  viewTimeline: boolean = true;
  statusCode: string = null;
  pageSize = 20;
  trangThaiDeXuat: number = 1;

  listProvince: Array<any> = [];
  listPhanLoaiTaiSan: Array<any> = [];
  listHienTrangTaiSan: Array<any> = [];
  listNguoiKiemKe: Array<any> = [];

  ngayKiemKeSearch: any = null;
  provinceSearch: any = null;
  phanLoaiTaiSanSearch: any = null;
  hienTrangTaiSanSearch: any = null;
  nguoiKiemKeSearch: any = null;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private def: ChangeDetectorRef,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private ref: ChangeDetectorRef,
    private encrDecrService: EncrDecrService,
    private assetService: AssetService,
    private getPermission: GetPermission,
    private datePipe: DatePipe,
  ) {
    this.innerWidth = window.innerWidth;
    if (this.innerWidth < 768) {
      this.isShowWorkFollowContract = false;
    }
  }

  async ngOnInit() {
    this.setForm();

    this.route.params.subscribe(params => {
      if (params['dotKiemKeId']) {
        this.dotKiemKeId = Number(this.encrDecrService.get(params['dotKiemKeId']));
      }
    });

    let resource = "ass/asset/chi-tiet-dot-kiem-ke";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      //if (listCurrentActionResource.indexOf("add") == -1) {
      //  this.actionAdd = false;
      //}
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }
    }

    this.getMasterData();

  }


  setForm() {

    this.tenDotKiemKeControl = new FormControl(null, Validators.required);
    this.ngayBatDauControl = new FormControl(null, Validators.required);
    this.ngayKetThucControl = new FormControl(null, Validators.required);

    this.dotKiemKeFormGroup = new FormGroup({
      tenDotKiemKeControl: this.tenDotKiemKeControl,
      ngayBatDauControl: this.ngayBatDauControl,
      ngayKetThucControl: this.ngayKetThucControl
    });
  }

  setCols() {
    this.cols = [
      { field: 'createdDate', header: 'Ngày kiểm kê', textAlign: 'center', display: 'table-cell', width: "150px" },
      { field: 'nguoiKiemKeName', header: 'Người kiểm kê', textAlign: 'left', display: 'table-cell', width: '200px' },
      { field: 'maTaiSan', header: 'Mã tài sản', textAlign: 'center', display: 'table-cell', width: '110px' },
      { field: 'tenTaiSan', header: 'Tên tài sản', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'khuVucName', header: 'Khu vực', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'tinhTrangName', header: 'Tình trạng', textAlign: 'center', display: 'table-cell', width: '110px' },
      { field: 'giaTriTinhKhauHao', header: 'Giá trị tính khấu hao', textAlign: 'right', display: 'table-cell', width: '170px' },
      { field: 'khauHaoLuyKe', header: 'Khấu hao lũy kế', textAlign: 'right', display: 'table-cell', width: '150px' },
      { field: 'giaTriConLai', header: 'Giá trị còn lại', textAlign: 'right', display: 'table-cell', width: '150px' },
      { field: 'moTaTaiSan', header: 'Mô tả tài sản', textAlign: 'left', display: 'table-cell', width: '150px' },
    ];
    this.selectedColumns = this.cols;
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  async getMasterData() {
    this.loading = true;
    let ngayKiemKe = this.ngayKiemKeSearch != null ? convertToUTCTime(new Date(this.ngayKiemKeSearch)) : null;
    let provincecId = this.provinceSearch != null ? this.provinceSearch.map(x => x.provinceId) : null;
    let phanLoaiTaiSanId = this.phanLoaiTaiSanSearch != null ? this.phanLoaiTaiSanSearch.map(x => x.categoryId) : null;
    let hienTrangTaiSan = this.hienTrangTaiSanSearch != null ? this.hienTrangTaiSanSearch.value : null;
    let nguoiKiemKeId = this.nguoiKiemKeSearch != null ? this.nguoiKiemKeSearch.map(x => x.employeeId) : null;
    this.assetService.dotKiemKeDetail(this.dotKiemKeId, provincecId, phanLoaiTaiSanId, hienTrangTaiSan, ngayKiemKe, nguoiKiemKeId).subscribe(response => {
      var result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.setDefaultValue(result);
      }
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  refreshFilter() {
    this.filterGlobal = '';
    this.provinceSearch = null;
    this.phanLoaiTaiSanSearch = null;
    this.listHienTrangTaiSan = null;
    this.nguoiKiemKeSearch = null;
    this.ngayKiemKeSearch = null;
    if (this.listTaiSan.length > 0) {
      this.myTable.reset();
    }
    this.getMasterData();
  }

  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.rightColNumber = 4;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }



  setDefaultValue(result) {

    this.trangThaiDeXuat = result.dotKiemKe.trangThaiId;
    this.tenDotKiemKeControl.setValue(result.dotKiemKe.tenDoiKiemKe);
    this.ngayBatDauControl.setValue(new Date(result.dotKiemKe.ngayBatDau));
    this.ngayKetThucControl.setValue(new Date(result.dotKiemKe.ngayKetThuc));
    this.listTaiSan = result.listDotKiemKeChiTiet;

    this.listProvince = result.listAllProvince;
    this.listPhanLoaiTaiSan = result.listPhanLoaiTaiSan;
    this.listHienTrangTaiSan = result.listHienTrangTaiSan;
    this.listNguoiKiemKe = result.listAllNguoiKiemKe;
    if (this.trangThaiDeXuat == 1) {
      this.tenDotKiemKeControl.enable();
      this.ngayBatDauControl.enable();
      this.ngayKetThucControl.enable();
    }
    if (this.trangThaiDeXuat == 2) {
      this.tenDotKiemKeControl.disable();
      this.ngayBatDauControl.disable();
      this.ngayKetThucControl.enable();
    }

    if (this.trangThaiDeXuat == 3) {
      this.tenDotKiemKeControl.disable();
      this.ngayBatDauControl.disable();
      this.ngayKetThucControl.disable();
    }

    this.setCols();
  }

  thoat() {
    this.router.navigate(['/asset/danh-sach-dot-kiem-ke']);
  }

  capNhatDoiKiemKe() {
    if (!this.dotKiemKeFormGroup.valid) {
      Object.keys(this.dotKiemKeFormGroup.controls).forEach(key => {
        if (this.dotKiemKeFormGroup.controls[key].valid == false) {
          this.dotKiemKeFormGroup.controls[key].markAsTouched();
        }
      });
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Vui lòng nhập đầy đủ thông tin các trường dữ liệu.' };
      this.showMessage(msg);
      return;
    }
    let tenDotKiemKe = this.tenDotKiemKeControl.value;
    let ngayBatDau = convertToUTCTime(new Date(this.ngayBatDauControl.value));
    let ngayKetThuc = convertToUTCTime(new Date(this.ngayKetThucControl.value));
    if (ngayBatDau > ngayKetThuc) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Ngày bắt đầu không được lớn hơn ngày kết thúc!' };
      this.showMessage(msg);
      return;
    }
    this.assetService.taoDotKiemKe(tenDotKiemKe, ngayBatDau, ngayKetThuc, this.dotKiemKeId).subscribe(response => {
      var result = <any>response;
      if (result.statusCode == 200) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.message };
        this.showMessage(mgs);
        this.getMasterData();
      } else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.message };
        this.showMessage(mgs);
      }
    });
  }

  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo ?.systemValueString;
  }

  exportExcel() {
    let title = this.tenDotKiemKeControl.value;
    let workBook = new Workbook();
    let worksheet = workBook.addWorksheet(title);
    worksheet.addRow([]);
    /* title */
    let headerMain = worksheet.addRow(['', "Danh sách tài sản kiểm kê".toUpperCase()]);
    headerMain.font = { size: 18, bold: true };
    worksheet.mergeCells(`B${2}:K${2}`);
    headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    worksheet.addRow([]);

    let dataHeaderRow1: Array<string> = [];
    dataHeaderRow1 = ["", "Ngày kiểm kê", "Người kiểm kê", "Mã tài sản", "Tên tài sản", "khu vực", "Tình trạng", "Giá trị khấu hao", "Khấu hao lũy kế", "Giá trị còn lại", "Mô tả tài sản"];
    let headerRow1 = worksheet.addRow(dataHeaderRow1);
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };

    //merge header column
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

    var datePipe = new DatePipe("en-US");
    let data: Array<any> = [];
    this.listTaiSan ?.forEach((item, index) => {
      let row: Array<any> = [];
      row[0] = '';
      row[1] = datePipe.transform(new Date(item.createdDate), 'dd-MM-yyyy');
      row[2] = item.nguoiKiemKeName;
      row[3] = item.maTaiSan;
      row[4] = item.tenTaiSan;
      row[5] = item.khuVucName;
      row[6] = item.tinhTrangName;
      row[7] = item.giaTriTinhKhauHao;
      row[8] = item.khauHaoLuyKe;
      row[9] = item.giaTriConLai;
      row[10] = item.moTaTaiSan;
      data.push(row);
    });

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman', size: 11 };

      row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(5).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(6).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(7).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(7).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(8).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(8).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(9).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(9).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(10).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(10).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(11).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(11).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    });

    /* fix with for column */
    worksheet.getColumn(1).width = 15;
    worksheet.getColumn(2).width = 15;
    worksheet.getColumn(3).width = 15;
    worksheet.getColumn(4).width = 15;
    worksheet.getColumn(5).width = 15;
    worksheet.getColumn(6).width = 15;
    worksheet.getColumn(7).width = 15;
    worksheet.getColumn(8).width = 15;
    worksheet.getColumn(9).width = 15;
    worksheet.getColumn(10).width = 15;
    worksheet.getColumn(11).width = 15;
    this.exportToExel(workBook, title);
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

}

function convertDate(time: any) {
  let ngay = time.getDate()
  let thang = time.getMonth() + 1
  let nam = time.getFullYear()
  return `${ngay}/${thang}/${nam}`
};

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
