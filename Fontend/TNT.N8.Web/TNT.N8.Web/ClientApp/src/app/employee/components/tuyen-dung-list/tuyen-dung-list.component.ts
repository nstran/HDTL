import { Component, OnInit, ViewChild } from '@angular/core';
import { Table } from 'primeng/table';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Vacancies } from '../../models/recruitment-vacancies.model';
import { Router } from '@angular/router';
import { MessageService, ConfirmationService, DialogService } from 'primeng';
import { EmployeeService } from '../../services/employee.service';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import * as XLSX from 'xlsx';
import { CategoryModel } from '../../../../app/shared/models/category.model';

interface Category {
  categoryId: string,
  categoryName: string,
  categoryCode: string
}
class ColumnExcel {
  code: string;
  name: string;
  width: number;
}
class CandidateModel {
  candidateId: string;
  vacanciesId: string;
  recruitmentCampaignId: string;
  fullName: string;
  dateOfBirth: Date;
  sex: number;
  address: string;
  phone: string;
  email: string;
  recruitmentChannel: string;
  applicationDate: Date;
}
@Component({
  selector: 'app-tuyen-dung-list',
  templateUrl: './tuyen-dung-list.component.html',
  styleUrls: ['./tuyen-dung-list.component.css']
})
export class TuyenDungListComponent implements OnInit {

  @ViewChild('myTable') myTable: Table;
  today: Date = new Date();
  filterGlobal: string = '';
  loading: boolean = false;
  fileName: string = '';
  currentYear: number = (new Date()).getFullYear();
  awaitResult: boolean = false;
  // Column
  selectedColumns: any[];
  colsListViTri: any[];
  // Excel
  displayChooseFileImportDialog: boolean = false;
  importFileExcel: any = null;

  //responsive
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  leftColNumber: number = 12;
  rightColNumber: number = 2;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  // List giá trị trả về
  listViTriTD: Array<any> = [];
  listChienDich: Array<any> = [];
  listMucUuTien: Array<any> = [
    {
      name: 'Cao',
      value: 1,
    },
    {
      name: 'Trung bình',
      value: 2,
    },
    {
      name: 'Thấp',
      value: 3,
    },
  ];

  listViTriTuyenDung: Array<Vacancies> = []; // danh sách vị trí tuyển dụng
  displayAddCandidate: boolean = false;
  // Điều kiện tìm kiếm
  selectedViTri: Array<any> = [];
  selectedChienDich: Array<any> = [];
  selectedMucDoUT: Array<any> = [];
  selectedKinhNghiem: Array<any> = [];
  selectedLoaiCV: Array<Category> = [];
  selectedNguoiPT: Array<any> = [];
  startSalary: string = '';
  endSalary: string = '';

  listExperience: Array<any> = [];
  listLoaiCV: Array<any> = [];
  listNguoiPT: Array<any> = [];
  listViTriFilter: Array<any> = [];

  createCandidateForm: FormGroup;
  vacanciesControl: FormControl;
  fullNameControl: FormControl;
  dateOfBirthControl: FormControl;
  phoneControl: FormControl;
  genderControl: FormControl;
  addressControl: FormControl;
  emailControl: FormControl;
  chanelControl: FormControl;
  applicationDateCandidateControl: FormControl;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  recruitmentCampaignId: string = '00000000-0000-0000-0000-000000000000';
  vacanciesId: string = '00000000-0000-0000-0000-000000000000';
  listChanel: Array<CategoryModel> = []

  constructor(
    private router: Router,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private employeeService: EmployeeService,
  ) {
    this.innerWidth = window.innerWidth;
  }

  ngOnInit(): void {
    this.initTable();
    this.setControlDialog();
    this.getMasterData();
  }

  initTable() {
    this.colsListViTri = [
      { field: 'vacanciesName', header: 'Tên vị trí', textAlign: 'left', display: 'table-cell', width: '16%' },
      { field: 'recruitmentCampaignName', header: 'Tên chiến dịch', textAlign: 'left', display: 'table-cell', width: '15%' },
      { field: 'priorityName', header: 'Ưu tiên', textAlign: 'center', display: 'table-cell', width: '5%' },
      { field: 'quantity', header: 'Số lượng', textAlign: 'center', display: 'table-cell', width: '6%' },
      { field: 'salary', header: 'Lương', textAlign: 'center', display: 'table-cell', width: '14%' },
      { field: 'experienceName', header: 'Kinh Nghiệm', textAlign: 'left', display: 'table-cell', width: '13%' },
      { field: 'typeOfWorkName', header: 'Loại công việc', textAlign: 'center', display: 'table-cell', width: '10%' },
      { field: 'personInChargeName', header: 'Người phụ trách', textAlign: 'left', display: 'table-cell', width: '13%' },
      { field: 'action', header: 'Thao tác', textAlign: 'center', display: 'table-cell', width: '8%' },
    ];

    this.selectedColumns = this.colsListViTri;
  }
  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.employeeService.getAllVacancies();
    if (result.statusCode == 200) {
      this.listChienDich = result.listChienDich;
      this.listExperience = result.listExperience;
      this.listLoaiCV = result.listLoaiCV;
      this.listNguoiPT = result.listNguoiPT;
      this.listViTriFilter = result.listViTriFilter;
      this.listViTriTuyenDung = result.listViTriTuyenDung;
      this.listChanel = result.listChanel;
      this.listViTriTuyenDung.forEach(item => {
        switch (item.priority) {
          case 1:
            item.priorityName = 'Cao';
            break;
          case 2:
            item.priorityName = 'Trung Bình';
            break;
          case 3:
            item.priorityName = 'Thấp';
            break;
        }

        if (item.salarType == 1)
          item.salary = item.salaryFrom.toLocaleString('vi-VN') + ' - ' + item.salaryTo.toLocaleString('vi-VN');
        if (item.salarType == 2)
          item.salary = "Thỏa thuận"
      });

      this.loading = false;
    } else {
      this.loading = false;
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  setControlDialog() {
    this.vacanciesControl = new FormControl('');
    this.fullNameControl = new FormControl('', Validators.required);
    this.phoneControl = new FormControl('', [Validators.required, Validators.pattern(this.getPhonePattern())]);
    this.dateOfBirthControl = new FormControl(null, Validators.required);
    this.genderControl = new FormControl(1, [Validators.required]);
    this.addressControl = new FormControl('');
    this.emailControl = new FormControl(null);
    this.chanelControl = new FormControl(null);
    this.applicationDateCandidateControl = new FormControl(null);

    this.createCandidateForm = new FormGroup({
      vacanciesControl: this.vacanciesControl,
      fullNameControl: this.fullNameControl,
      phoneControl: this.phoneControl,
      dateOfBirthControl: this.dateOfBirthControl,
      genderControl: this.genderControl,
      addressControl: this.addressControl,
      emailControl: this.emailControl,
      chanelControl: this.chanelControl,
      applicationDateCandidateControl: this.applicationDateCandidateControl
    });
  }
  exportExcel() {
    if (this.listViTriTuyenDung.length > 0) {
      if (this.listViTriTuyenDung.length > 0) {
        this.loading = true

        let title = `Danh sách vị trí tuyển dụng`;

        let workBook = new Workbook();
        let worksheet = workBook.addWorksheet(title);

        let dataHeaderMain = "Danh sách vị trí tuyển dụng".toUpperCase();
        let headerMain = worksheet.addRow([dataHeaderMain]);
        headerMain.font = { size: 18, bold: true };
        worksheet.mergeCells(`A${1}:F${1}`);
        headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        worksheet.addRow([]);

        /* Header row */
        let buildColumnExcel = this.buildColumnExcel();
        let dataHeaderRow = buildColumnExcel.map(x => x.name);
        let headerRow = worksheet.addRow(dataHeaderRow);
        headerRow.font = { name: 'Times New Roman', size: 12, bold: true };
        dataHeaderRow.forEach((item, index) => {
          headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          headerRow.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: '8DB4E2' }
          };
        });
        headerRow.height = 40;

        /* Data table */
        let data: Array<any> = [];
        this.listViTriTuyenDung.forEach(item => {
          let row: Array<any> = [];

          buildColumnExcel.forEach((_item, _index) => {
            if (_item.code != 'action') {

              if (_item.code == 'vacanciesName') row[_index] = item.vacanciesName;
              if (_item.code == 'recruitmentCampaignName') row[_index] = item.recruitmentCampaignName;
              if (_item.code == 'priorityName') row[_index] = item.priorityName;
              if (_item.code == 'quantity') row[_index] = item.quantity;
              if (_item.code == 'salary') row[_index] = item.salary;
              if (_item.code == 'experienceName') row[_index] = item.experienceName;
              if (_item.code == 'typeOfWorkName') row[_index] = item.typeOfWorkName;
              if (_item.code == 'personInChargeName') row[_index] = item.personInChargeName;
            }
          });
          data.push(row);
        });

        //Tô đậm khung cho các ô
        data.forEach((el, index, array) => {
          let row = worksheet.addRow(el);
          row.font = { name: 'Times New Roman', size: 12 };

          buildColumnExcel.forEach((_item, _index) => {
            row.getCell(_index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
            if (_item.code == 'tinhchat') {
              row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
            }
            else {
              row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
            }
          });
        });

        /* fix with for column */
        buildColumnExcel.forEach((item, index) => {
          worksheet.getColumn(index + 1).width = item.width;
        });

        this.exportToExel(workBook, title);
      }
      else {
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn phải chọn ít nhất 1 cột' };
        this.showMessage(msg);
      }
    }
    else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn chưa chọn báo giá' };
      this.showMessage(msg);
    }
    this.loading = false
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
      let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Xuất excel thành công" };
      this.showMessage(mgs);
    })
  }

  buildColumnExcel(): Array<ColumnExcel> {
    let listColumn: Array<ColumnExcel> = [];

    this.selectedColumns.forEach(item => {
      if (item.field != 'action') {
        let column = new ColumnExcel();
        column.code = item.field;
        column.name = item.header;
        if (item.field == 'vacanciesName') column.width = 40;
        if (item.field == 'recruitmentCampaignName') column.width = 40;
        if (item.field == 'priorityName') column.width = 15;
        if (item.field == 'quantity') column.width = 10;
        if (item.field == 'salary') column.width = 25;
        if (item.field == 'experienceName') column.width = 30;
        if (item.field == 'typeOfWorkName') column.width = 25;
        if (item.field == 'personInChargeName') column.width = 30;
        listColumn.push(column);
      }
    });

    return listColumn;
  }

  refreshFilter() {

    this.selectedChienDich = [];
    this.selectedViTri = [];
    this.selectedKinhNghiem = [];
    this.selectedLoaiCV = [];
    this.selectedMucDoUT = [];
    this.selectedNguoiPT = [];
    this.startSalary = null;
    this.endSalary = null;
    this.filterGlobal = '';
    this.filterGlobal = null;

    if (this.listViTriTuyenDung.length > 0) {
      this.myTable.reset();
    }
    this.filterVacancies();
  }


  importCustomer() {
    this.displayChooseFileImportDialog = true;
  }

  chooseFile(event: any) {
    this.fileName = event.target ?.files[0] ?.name;
    this.importFileExcel = event.target;
  }

  importExcel() {
    if (this.fileName == "") {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Chưa chọn file cần nhập" };
      this.showMessage(mgs);
      return;
    }
    this.loading = true

    const targetFiles: DataTransfer = <DataTransfer>(this.importFileExcel);
    const reader: FileReader = new FileReader();
    reader.readAsBinaryString(targetFiles.files[0]);
    reader.onload = (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary', cellDates: true });
      let code = 'Danh mục KH';
      if (!workbook.Sheets[code]) {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: "File không hợp lệ" };
        this.showMessage(mgs);
        return;
      }

      //lấy data từ file excel
      const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[code];
      /* save data */
      let listCustomerRawData: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
      /* remove header */
      listCustomerRawData = listCustomerRawData.filter((e, index) => index != 0);
      /* nếu không nhập  trường required thì loại bỏ */
      listCustomerRawData = listCustomerRawData.filter(e => (e[0] && e[1]));
      /* chuyển từ raw data sang model */
      // let listCustomerImport: Array<importCustomerByExcelModel> = [];
      // listCustomerRawData ?.forEach(_rawData => {
      //   let customer = new importCustomerByExcelModel();
      //   customer.TenKhachHang = _rawData[0] ? _rawData[0].toString().trim() : '';
      //   customer.LoaiKH = _rawData[1] ? _rawData[1] : 2;
      //   customer.DiaChi = _rawData[2] ? _rawData[2].toString().trim() : '';
      //   customer.MaSoThue = _rawData[3] ? _rawData[3].toString().trim() : '';
      //   customer.SDT = _rawData[4] ? _rawData[4].toString().trim() : '';
      //   customer.email = _rawData[5] ? _rawData[5].toString().trim() : '';
      //   customer.TenLH = _rawData[6] ? _rawData[6].toString().trim() : '';
      //   customer.SDTLH = _rawData[7] ? _rawData[7].toString().trim() : '';
      //   customer.MuaOGreenKa = _rawData[8] == 1 ? true : false;
      //   customer.bienSo = _rawData[9] ? _rawData[9].toString().trim() : '';
      //   customer.modelCar = _rawData[10] ? _rawData[10].toString().trim() : '';
      //   customer.soKhung = _rawData[11] ? _rawData[11] : '';
      //   customer.TenLienLac = _rawData[12] ? _rawData[12].toString().trim() : '';
      //   customer.SDTLC = _rawData[13] ? _rawData[13].toString().trim() : '';
      //   customer.BDSC = _rawData[14] = 1 ? true : false;
      //   listCustomerImport = [...listCustomerImport, customer];
      // });
      /* tắt dialog import file, bật dialog chi tiết khách hàng import */
      this.displayChooseFileImportDialog = false;
      this.loading = false
      //this.openDetailImportDialog(listCustomerImport);
    }
  }

  goToCreate() {
    this.router.navigate(['/employee/tao-cong-viec-tuyen-dung']);
  }

  openDetailImportDialog(listCustomerImport) {
    // let ref = this.dialogService.open(XeVaoXuongImportComponent, {
    //   data: {
    //     isPotentialCustomer: false,
    //     listCustomerImport: listCustomerImport,
    //     listCarInWorkShop: this.listViTriTuyenDung
    //   },
    //   header: 'Nhập excel danh sách xe vào xưởng',
    //   width: '85%',
    //   baseZIndex: 1050,
    //   contentStyle: {
    //     "max-height": "800px",
    //     // "over-flow": "hidden"
    //   }
    // });
    // ref.onClose.subscribe((result: any) => {
    //   if (result ?.status) {
    //     this.getMasterData();
    //   }
    // });

  }
  //#region BỘ LỌC
  async filterVacancies() {
    this.loading = true;

    let selectedViTriId: Array<string> = [];
    let selectedChienDichId: Array<string> = [];
    let selectedMucDoUTId: Array<number> = [];
    let selectedKinhNghiemId: Array<string> = [];
    let selectedLoaiCVId: Array<string> = [];
    let selectedNguoiPTId: Array<string> = [];

    if (this.selectedKinhNghiem.length > 0) {
      this.selectedKinhNghiem.forEach(item => {
        selectedKinhNghiemId.push(item.categoryId);
      });
    }
    if (this.selectedLoaiCV.length > 0) {
      this.selectedLoaiCV.forEach(item => {
        selectedLoaiCVId.push(item.categoryId);
      });
    }
    if (this.selectedNguoiPT.length > 0) {
      this.selectedNguoiPT.forEach(item => {
        selectedNguoiPTId.push(item.employeeId);
      });
    }
    if (this.selectedViTri.length > 0) {
      this.selectedViTri.forEach(item => {
        selectedViTriId.push(item.categoryId);
      })
    }
    if (this.selectedChienDich.length > 0) {
      this.selectedChienDich.forEach(item => {
        selectedChienDichId.push(item.recruitmentCampaignId);
      });
    }
    if (this.selectedMucDoUT.length > 0) {
      this.selectedMucDoUT.forEach(item => {
        selectedMucDoUTId.push(item.value);
      });
    }

    await this.employeeService.filterVacancies(selectedViTriId, selectedChienDichId, selectedMucDoUTId, selectedKinhNghiemId, selectedLoaiCVId, selectedNguoiPTId, ParseStringToFloat(this.startSalary), ParseStringToFloat(this.endSalary)).subscribe(response => {

      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listViTriTuyenDung = result.listViTriTuyenDung;
        this.listViTriTuyenDung.forEach(item => {
          switch (item.priority) {
            case 1:
              item.priorityName = 'Cao';
              break;
            case 2:
              item.priorityName = 'Trung Bình';
              break;
            case 3:
              item.priorityName = 'Thấp';
              break;
          }
          item.salary = item.salaryFrom.toString() + ' - ' + item.salaryTo.toString();
        });
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
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
  //#endregion

  onClickImportBtn(event: any) {
    /* clear value của file input */
    event.target.value = ''
  }
  closeChooseFileImportDialog() {
    this.cancelFile();
  }
  cancelFile() {
    let fileInput = $("#importFileVacancies")
    fileInput.replaceWith(fileInput.val('').clone(true));
    this.fileName = "";
  }
  async downloadTemplateExcel() {
    // let result: any = await this.employeeService.downloadTemplateImportXeVaoXuong();
    // if (result.templateExcel != null && result.statusCode === 200) {
    //   const binaryString = window.atob(result.templateExcel);
    //   const binaryLen = binaryString.length;
    //   const bytes = new Uint8Array(binaryLen);
    //   for (let idx = 0; idx < binaryLen; idx++) {
    //     const ascii = binaryString.charCodeAt(idx);
    //     bytes[idx] = ascii;
    //   }
    //   const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    //   const link = document.createElement('a');
    //   link.href = window.URL.createObjectURL(blob);
    //   const fileName = result.fileName + ".xls";
    //   link.download = fileName;
    //   link.click();
    // } else {
    //   this.displayChooseFileImportDialog = false;
    //   let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Download file thất bại' };
    //   this.showMessage(msg);
    // }
  }

  goToDetail(rowData: any) {

    this.router.navigate(['/employee/chi-tiet-cong-viec-tuyen-dung', { vacanciesId: rowData.vacanciesId, recruitmentCampaignId: rowData.recruitmentCampaignId }]);
  }

  editData() { }
  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  removeData(rowData) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn xóa vị trí tuyển dụng này không?',
      accept: async () => {
        rowData.active = false;

        let result: any = await this.employeeService.deleteVacanciesById(rowData.vacanciesId);
        if (result.statusCode === 200) {
          this.listViTriTuyenDung = this.listViTriTuyenDung.filter(e => e != rowData);
          this.showToast('success', 'Thông báo', "Bạn đã xóa vị trí tuyển dụng thành công")

        } else {
          this.showToast('error', 'Thông báo', result.message)
        }
      }
    });
  }

  // Thêm nhanh ứng viên
  addCandidate(rowData) {

    this.createCandidateForm.reset();
    this.vacanciesId = rowData.vacanciesId;
    this.recruitmentCampaignId = rowData.recruitmentCampaignId;
    this.displayAddCandidate = true;
    this.vacanciesControl.setValue(rowData.vacanciesName);
  }

  // Đóng dialog thêm UV
  cancelAddCandidateDialog() {
    this.confirmationService.confirm({
      message: 'Bạn có muốn hủy bỏ các thay đổi?',
      accept: () => {
        this.displayAddCandidate = false;
      }
    });
  }

  // Thêm nhanh ứng viên
  async saveAddCandidateDialog() {
    if (!this.createCandidateForm.valid) {
      Object.keys(this.createCandidateForm.controls).forEach(key => {
        if (!this.createCandidateForm.controls[key].valid) {
          this.createCandidateForm.controls[key].markAsTouched();
        }
      });
    }
    else {
      this.loading = true;
      this.awaitResult = true;
      let candidateModel: CandidateModel = this.mappingCandidateFormToModel();
      this.employeeService.createCandidate(candidateModel, this.vacanciesId, null, []).subscribe(res => {
        let result = <any>res;
        this.loading = false;
        this.awaitResult = false;
        if (result.statusCode == 200) {
          this.displayAddCandidate = false;
          // this.listViTriTuyenDung = result.listViTriTuyenDung;

          // this.listViTriTuyenDung ?.forEach(item => {
          //   switch (item.priority) {
          //     case 1:
          //       item.priorityName = 'Cao';
          //       break;
          //     case 2:
          //       item.priorityName = 'Trung Bình';
          //       break;
          //     case 3:
          //       item.priorityName = 'Thấp';
          //       break;
          //   }
          //   item.salary = item.salaryFrom.toString() + ' - ' + item.salaryTo.toString();
          // });
          let msg = { severity: 'success', summary: 'Thông báo', detail: 'Thêm ứng viên thành công' };
          this.showMessageN(msg);
        }
        else {
          this.loading = false;
          let msg = { severity: 'error', summary: 'Thông báo', detail: 'Thêm ứng viên thất bại' };
          this.showMessageN(msg);
        }
      });
      this.loading = false;
      this.awaitResult = false;
    }
  }

  showMessageN(msg: any) {
    this.messageService.add(msg);
  }

  mappingCandidateFormToModel() {
    let candidateModel = new CandidateModel();
    candidateModel.candidateId = this.emptyGuid;
    candidateModel.vacanciesId = this.vacanciesId;
    candidateModel.recruitmentCampaignId = this.recruitmentCampaignId;

    candidateModel.fullName = this.createCandidateForm.get('fullNameControl').value == null ? null : this.createCandidateForm.get('fullNameControl').value;

    candidateModel.dateOfBirth = this.createCandidateForm.get('dateOfBirthControl').value == "" || this.createCandidateForm.get('dateOfBirthControl').value == null ? null : convertToUTCTime(this.createCandidateForm.get('dateOfBirthControl').value);

    candidateModel.sex = this.createCandidateForm.get('genderControl').value == null ? null : this.createCandidateForm.get('genderControl').value;

    candidateModel.address = this.createCandidateForm.get('addressControl').value == null ? null : this.createCandidateForm.get('addressControl').value;

    candidateModel.phone = this.createCandidateForm.get('phoneControl').value == null ? null : this.createCandidateForm.get('phoneControl').value;

    candidateModel.email = this.emailControl.value ? this.emailControl.value : '';

    candidateModel.recruitmentChannel = this.createCandidateForm.get('chanelControl').value == null ? null : this.createCandidateForm.get('chanelControl').value.categoryId;

    candidateModel.applicationDate = this.createCandidateForm.get('applicationDateCandidateControl').value == "" || this.createCandidateForm.get('applicationDateCandidateControl').value == null ? null : convertToUTCTime(this.createCandidateForm.get('applicationDateCandidateControl').value);
    return candidateModel;
  }

}

function ParseStringToFloat(str: string) {

  if (str === "" || str == null) return null;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
