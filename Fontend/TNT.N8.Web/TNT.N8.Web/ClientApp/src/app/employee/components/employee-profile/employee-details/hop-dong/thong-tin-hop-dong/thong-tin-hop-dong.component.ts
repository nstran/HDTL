import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { ConfirmationService } from 'primeng/api';

import { ThemMoiHopDongComponent } from '../them-moi-hop-dong/them-moi-hop-dong.component';
import { ChiTietHopDongComponent } from '../chi-tiet-hop-dong/chi-tiet-hop-dong.component';

import { HopDongNhanSuModel } from './../../../../../models/hop-dong-nhan-su.model';

import { EmployeeService } from './../../../../../services/employee.service';
import { HopDongImportDetailComponent } from '../hop-dong-import-detail/hop-dong-import-detail.component';
import * as XLSX from 'xlsx';


class importHopDongModel {
  LoaiHopDong: string;
  LoaiHopDongId: string;

  SoHopDong: string;
  SoPhuLuc: string;

  NgayKyHD: Date;
  NgayBDLamViec: Date;
  NgayKetThucHD: Date;

  ChucVu: string;
  ChucVuId: string;

  MucLuong: number;
}


@Component({
  selector: 'app-thong-tin-hop-dong',
  templateUrl: './thong-tin-hop-dong.component.html',
  styleUrls: ['./thong-tin-hop-dong.component.css']
})
export class ThongTinHopDongComponent implements OnInit {
  loading: boolean = false;
  @Input() actionEdit: boolean;
  @Input() employeeId: string;

  isShowButton: boolean = false;
  displayChooseFileImportDialog: boolean = false;

  fileName: string = '';
  importFileExcel: any = null;

  listPosition = [];
  listLoaiHopDong = [];

  cols: Array<any> = [];
  listHopDong: Array<HopDongNhanSuModel> = [];

  @ViewChild('myTable') myTable: Table;
  constructor(
    public dialogService: DialogService,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    public confirmationService: ConfirmationService,
  ) { }

  ngOnInit(): void {
    this.initTable();
    this.getListHopDongNhanSu();
  }

  initTable() {
    this.cols = [
      { field: 'stt', header: '#', textAlign: 'center', colWith: '3vw' },
      { field: 'soHopDong', header: 'Số HĐ', textAlign: 'left', colWith: '8vw' },
      { field: 'soPhuLuc', header: 'Số phụ lục', textAlign: 'left', colWith: '8vw' },
      { field: 'loaiHopDong', header: 'Loại hợp đồng', textAlign: 'left', colWith: '10vw' },
      { field: 'ngayKyHopDong', header: 'Ngày ký', textAlign: 'center', colWith: '8vw' },
      { field: 'ngayBatDauLamViec', header: 'Ngày bắt đầu làm việc', textAlign: 'center', colWith: '8vw' },
      { field: 'ngayKetThucHopDong', header: 'Ngày kết thúc', textAlign: 'center', colWith: '8vw' },
      { field: 'positionName', header: 'Chức vụ', textAlign: 'left', colWith: '8vw' },
      { field: 'mucLuong', header: 'Mức lương', textAlign: 'right', colWith: '8vw' },
      { field: 'action', header: 'Thao tác', textAlign: 'center', colWith: '150px' }
    ];
  }

  async getListHopDongNhanSu() {
    this.loading = true;
    let result: any = await this.employeeService.getListHopDongNhanSu(this.employeeId);
    this.loading = false;

    if (result.statusCode == 200) {
      this.listHopDong = result.listHopDongNhanSu;
      this.listPosition = result.listChucVu;
      this.listLoaiHopDong = result.listLoaiHopDongNhanSu;

      this.isShowButton = result.isShowButton;
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  addHopDong(rowData = null) {
    let isAdd: boolean = true;
    if (rowData) {
      isAdd = false;
    }
    let ref = this.dialogService.open(ThemMoiHopDongComponent, {
      data: {
        isAdd: isAdd,
        employeeId: this.employeeId,
        listPosition: this.listPosition,
        listLoaiHopDong: this.listLoaiHopDong,
        hopDongNhanSu: rowData
      },
      header: 'Thông tin hợp đồng',
      width: '690px',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "350px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      this.getListHopDongNhanSu();
    });
  }

  viewHopDong(data) {
    let ref = this.dialogService.open(ChiTietHopDongComponent, {
      data: {
        employeeId: this.employeeId,
        listSelectedHopDong: [data],
        dataHopDong: this.listHopDong
      },
      header: 'Lịch sử tăng lương',
      width: '690px',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "350px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      this.getListHopDongNhanSu();
    });
  }

  deleteHopDong(data) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa, dữ liệu sẽ không thể hoàn tác?',
      accept: async () => {
        try {
          this.loading = true;
          let result: any = await this.employeeService.deleteHopDongNhanSuById(data.hopDongNhanSuId);
          this.loading = false;

          if (result.statusCode != 200) {
            this.showMessage('error', result.messageCode);
            return;
          }
          this.showMessage('success', "Xóa dữ liệu thành công");
          this.getListHopDongNhanSu();
        } catch (error) {
          this.showMessage('error', error);
        }
      }
    });
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

  importHopDong() {
    if (this.fileName == "") {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Chưa chọn file cần nhập" };
      this.messageService.add(mgs);
      return;
    }

    const targetFiles: DataTransfer = <DataTransfer>(this.importFileExcel);
    const reader: FileReader = new FileReader();
    reader.readAsBinaryString(targetFiles.files[0]);
    reader.onload = (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary', cellText: true });
      let code = 'ImpotHDNS';
      if (!workbook.Sheets[code]) {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: "File không hợp lệ" };
        this.messageService.add(mgs);
        return;
      }
      let columns = [
        { field: 'LoaiHopDong', type: 'text', isRequired: true },
        { field: 'SoHopDong', type: 'text', isRequired: true },
        { field: 'SoPhuLuc', type: 'text', isRequired: true },
        { field: 'NgayKyHD', type: 'date', isRequired: true },
        { field: 'NgayBDLamViec', type: 'date', isRequired: true },
        { field: 'NgayKetThucHD', type: 'date', isRequired: true },
        { field: 'ChucVu', type: 'text', isRequired: true },
        { field: 'MucLuong', type: 'text', isRequired: false },
      ];

      //lấy data từ file excel
      const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[code];
      /* save data */
      let listTaiSanRawData: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
      /* remove header */
      listTaiSanRawData = listTaiSanRawData.filter((e, index) => index != 0);
      /* nếu không nhập 2 trường required: tên + mã khách hàng thì loại bỏ */
      listTaiSanRawData = listTaiSanRawData.filter(e => (e[1] && e[4]));
      /* chuyển từ raw data sang model */
      let listTaiSanRawImport: Array<importHopDongModel> = [];
      listTaiSanRawData?.forEach(_rawData => {
        let customer = new importHopDongModel();
        columns.forEach((item, index) => {
          if (item.type == 'text') customer[item.field] = _rawData[index] ? _rawData[index].toString().trim() : '';
          if (item.type == 'date') customer[item.field] = _rawData[index] ? my_date(_rawData[index].toString().trim()) : null;
        });
        listTaiSanRawImport = [...listTaiSanRawImport, customer];
      });
      /* tắt dialog import file, bật dialog chi tiết khách hàng import */
      this.displayChooseFileImportDialog = false;
      console.log('listTaiSanRawImport', listTaiSanRawImport)
      this.openDetailImportDialog(listTaiSanRawImport);
    }
  }

  openDetailImportDialog(listTaiSanRawImport) {
    let ref = this.dialogService.open(HopDongImportDetailComponent, {
      data: {
        listTaiSanImport: listTaiSanRawImport,
        employeeId: this.employeeId
      },
      header: 'Import hợp đồng nhân viên',
      width: '85%',
      baseZIndex: 1050,
      contentStyle: {
        "max-height": "800px",
        // "over-flow": "hidden"
      }
    });
    ref.onClose.subscribe((result: any) => {
      if (result?.statusCode == 200) {
        this.getListHopDongNhanSu();
      }
    });
  }


  closeChooseFileImportDialog() {
    this.cancelFile();
  }

  cancelFile() {
    let fileInput = $("#importFileProduct")
    fileInput.replaceWith(fileInput.val('').clone(true));
    this.fileName = "";


  }

  chooseFile(event: any) {
    this.fileName = event.target?.files[0]?.name;
    this.importFileExcel = event.target;
  }

  onpenDialogChoseFileExcel() {
    this.displayChooseFileImportDialog = true;
  }

  async downloadTemplateExcel() {
    this.loading = true;
    let result: any = await this.employeeService.downloadTemplateImportHDNS();
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
      const fileName = result.fileName + ".xlsx";
      link.download = fileName;
      link.click();
    } else {
      this.displayChooseFileImportDialog = false;
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Download file thất bại' };
      this.messageService.add(msg);
    }
  }

  onClickImportBtn(event: any) {
    /* clear value của file input */
    event.target.value = ''
  }

}

function my_date(date_string) {
  var date_components = date_string.split("/");
  if (date_components.length == 0) return null;
  var day = date_components[0];
  var month = date_components[1];
  var year = date_components[2];
  return new Date(year, month - 1, day);
}

