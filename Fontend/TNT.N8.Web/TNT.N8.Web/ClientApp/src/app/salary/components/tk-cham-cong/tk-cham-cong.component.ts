import { SalaryService } from './../../services/salary.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { GetPermission } from "../../../shared/permission/get-permission";
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from "primeng/api";
import { ChamCongThuongComponent } from './cham-cong-thuong/cham-cong-thuong.component';
import { ChamCongOtComponent } from './cham-cong-ot/cham-cong-ot.component';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-tk-cham-cong',
  templateUrl: './tk-cham-cong.component.html',
  styleUrls: ['./tk-cham-cong.component.css']
})
export class TkChamCongComponent implements OnInit {
  loading: boolean = false;
  awaitResponse: boolean = false;

  actionEdit: boolean = false;
  actionImport: boolean = false;
  actionDownload: boolean = false;

  tabIndex: number = 0;

  listEmployee: Array<any> = [];
  listKyHieuChamCong: Array<any> = [];

  @ViewChild('chamCongThuong') chamCongThuong: ChamCongThuongComponent;
  @ViewChild('chamCongOt') chamCongOt: ChamCongOtComponent;

  @ViewChild('attachment') attachment: any;
  fileExcelImport: any = null;
  displayDialogImport: boolean = false;
  fileName: string = '';

  displayErrorDialog: boolean = false;
  colsErrorDialog: Array<any> = [];
  listDataError: Array<any> = [];

  constructor(
    private getPermission: GetPermission,
    private router: Router,
    private messageService: MessageService,
    private salaryService: SalaryService,
    private confirmationService: ConfirmationService,
  ) { }

  async ngOnInit() {
    this.initTable();
    await this._getPermission();
    this.getMasterData();
  }

  initTable() {
    this.colsErrorDialog = [
      { field: 'index', header: '#', textAlign: 'center', width: '5%' },
      { field: 'name', header: 'Họ tên', textAlign: 'left', width: '30%' },
      { field: 'no', header: 'Code', textAlign: 'center', width: '5%' },
      { field: 'thoiGianChamCong', header: 'Thời gian chấm công', textAlign: 'left', width: '20%' },
      { field: 'noteError', header: 'Ghi chú', textAlign: 'left', width: '40%' },
    ];
  }

  async _getPermission() {
    let resource = "salary/salary/tk-cham-cong/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(["/home"]);
      return;
    }

    if (permission.listCurrentActionResource.indexOf("edit") != -1) {
      this.actionEdit = true;
    }

    if (permission.listCurrentActionResource.indexOf("import") != -1) {
      this.actionImport = true;
    }

    if (permission.listCurrentActionResource.indexOf("download") != -1) {
      this.actionDownload = true;
    }
  }

  async getMasterData() {
    let result: any = await this.salaryService.getMasterThongKeChamCong();

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.listEmployee = result.listEmployee;
    this.listKyHieuChamCong = result.listKyHieuChamCong.map((item) =>
      Object.assign({}, item, {
        name: item.name + `${item.valueText ? ` (${item.valueText})` : ''}`
      })
    )
  }

  showDialogImport() {
    this.displayDialogImport = true;
  }

  close() {
    this.displayErrorDialog = false;
  }

  chooseFile(event: any) {
    this.fileName = event.target.files[0].name;
    this.fileExcelImport = event.target;
  }

  cancelFile() {
    this.attachment.nativeElement.value = '';
    this.fileName = null;
  }

  importExcel() {
    if (!this.fileName) {
      this.showMessage("error", 'Bạn chưa chọn file');
      return;
    }

    const targetFiles: DataTransfer = <DataTransfer>(this.fileExcelImport);
    const reader: FileReader = new FileReader();
    reader.readAsBinaryString(targetFiles.files[0]);

    reader.onload = async (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });

      let code = workbook.SheetNames[0];

      // Lấy data từ file excel
      const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[code];

      /* save data */
      let dataPoint: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
      dataPoint.shift();

      let listData = [];
      dataPoint.forEach((row, index) => {
        if (row.length != 0) {
          let data = {
            index: index + 1,
            name: row[1],
            no: row[2] ? (row[2] != '' ? row[2] : null) : null,
            thoiGianChamCong: row[3] ? (row[3].trim() != '' ? row[3].trim() : null) : null
          };
          listData.push(data);
        }
      });

      if (!listData.length) {
        this.showMessage("error", 'File import không có dữ liệu');
        this.displayDialogImport = false;
        return;
      }

      this.confirmationService.confirm({
        message: `Nếu dữ liệu đã tồn tại trên hệ thống thì sẽ được cập nhật lại, bạn có chắc chắn muốn import?`,
        accept: async () => {
          this.awaitResponse = true;
          let result: any = await this.salaryService.importChamCong(listData);
          this.awaitResponse = false;

          if (result.statusCode == 409) {
            this.showMessage("error", result.messageCode);
            this.displayDialogImport = false;
            this.listDataError = [];
            this.listDataError = result.listDataError.filter(x => x.isError == true).map((m, index) => {
              m.index = index + 1;
              return m;
            });
            this.displayErrorDialog = true;
            return;
          }

          if (result.statusCode != 200) {
            this.showMessage("error", result.messageCode);
            return;
          }

          this.showMessage("success", result.messageCode);
          this.chamCongThuong.selectedRow = null;
          await this.chamCongThuong.getData();
          this.chamCongThuong.refreshNote++;
          this.chamCongOt.selectedRow = null;
          await this.chamCongOt.getData();
          this.displayDialogImport = false;
          return;
        },
      });
    }
  }

  exportExcel() {
    //Chấm công thường
    if (this.tabIndex == 0) {
      this.chamCongThuong.exportExcel();
    }
    //Chấm công OT
    else if (this.tabIndex == 1) {
      this.chamCongOt.exportExcel();
    }
  }

  refreshFilter() {
    this.chamCongThuong.refreshFilter();
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }
}
