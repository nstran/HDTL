import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { MessageService, ConfirmationService } from 'primeng/api';
import { EmployeeService } from '../../../services/employee.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import * as XLSX from 'xlsx';
import { DialogService } from 'primeng';
import { ImportNvDeXuatTangLuongComponent } from '../importNv-de-xuat-tang-luong/importNv-de-xuat-tang-luong.component';
import { FileUpload } from 'primeng/fileupload';
import { EncrDecrService } from '../../../../shared/services/encrDecr.service';

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

class importNVByExcelModel {
  empId: number;
  empCode: string;
  empName: string;
  oranganizationName: string;
  positionName: string;
  mucLuongCu: number;
  luongDeXuat: number;
  luongChenhLech: number;
  lydo: string;
}

class EmployeeInterface {
  employeeId: number;
  employeeName: string;
  chucVuId: string;
  organizationName: string;
  employeeCode: string;
  PhongBanId: string;
  positionName: string;
  DateOfBirth: string;
  lyDoDeXuat: string;
  luongDeXuat: number;
  luongHienTai: number;
  mucChechLech: number;
  trangThai: number;
}

class DeXuatTangLuongModel {
  TenDeXuat: string;
  LoaiDeXuat: number;
  NgayDeXuat: Date;
  NguoiDeXuatId: string;
  TrangThai: number;
}


@Component({
  selector: 'app-de-xuat-tang-luong',
  templateUrl: './de-xuat-tang-luong.component.html',
  styleUrls: ['./de-xuat-tang-luong.component.css']
})
export class DeXuatTangLuongComponent implements OnInit {
  auth = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;

  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  arrayDocumentModel: Array<any> = [];
  statusCode: string = null;

  displayChooseFileImportDialog: boolean = false;
  fileName: string = '';
  importFileExcel: any = null;

  colsListEmp: any[];
  selectedColumns: any[];

  listEmpAdd: any[]; // List nhân viên chọn để thêm đề xuất
  listEmpSelected: Array<EmployeeInterface> = new Array<EmployeeInterface>();  // List nhân viên được chọn thêm đề xuất
  colsFile: any[];

  today = new Date();

  //Form của phiếu đề xuất tăng lương
  deXuatTangLuongFormGroup: FormGroup;
  tenDeXuatFormControl: FormControl;
  phanLoaiDeXuatFormControl: FormControl;
  ngayDeXuatFormControl: FormControl;
  nguoiDeXuatFormControl: FormControl;

  //Ds nhân viên được đề xuất
  nhanVienFormGroup: FormGroup;
  EmployeeIdFormControl: FormControl;
  EmployeeNameFormControl: FormControl;
  OranganizationIdFormControl: FormControl;
  OranganizationNameFormControl: FormControl;
  MucLuongDeXuatFormControl: FormControl;
  EmployeeCodeFormControl: FormControl;
  PositisionIdFormControl: FormControl;
  PositisionNameFormControl: FormControl;
  DateOfBirthFormControl: FormControl;
  MucLuongCuFormControl: FormControl;
  LyDoFormControl: FormControl;

  IsEditNV: boolean = false;

  loginEmpId: string = '';

  filterGlobal: string = '';

  //list phân loại đều xuất có 2 loại => fixx cứng
  listLoaiDeXuat = [
    { value: 1, Name: "Đề xuất tăng lương adhoc" },
    { value: 2, Name: "Đề xuất tăng lương sau đánh giá" },
  ];

  tongTang: number = 0;
  actionAdd:Boolean = true;
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    private def: ChangeDetectorRef,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private encrDecrService: EncrDecrService,
    private getPermission: GetPermission,
  ) { }

  async ngOnInit() {
    this.setForm();
    this.setCols();
    await this._getPermission();
    this.getMasterData();
  }

  setForm() {
    this.tenDeXuatFormControl = new FormControl(null, [Validators.required]);
    this.phanLoaiDeXuatFormControl = new FormControl(null, [Validators.required]);
    this.ngayDeXuatFormControl = new FormControl(null, [Validators.required]);
    this.nguoiDeXuatFormControl = new FormControl(null, [Validators.required]);

    this.deXuatTangLuongFormGroup = new FormGroup({
      tenDeXuatFormControl: this.tenDeXuatFormControl,
      phanLoaiDeXuatFormControl: this.phanLoaiDeXuatFormControl,
      ngayDeXuatFormControl: this.ngayDeXuatFormControl,
      nguoiDeXuatFormControl: this.nguoiDeXuatFormControl
    });

    this.phanLoaiDeXuatFormControl.disable();

    this.EmployeeIdFormControl = new FormControl(null, [Validators.required]);
    this.EmployeeNameFormControl = new FormControl(null);
    this.OranganizationIdFormControl = new FormControl();
    this.OranganizationNameFormControl = new FormControl(null);
    this.MucLuongCuFormControl = new FormControl(null);
    this.MucLuongDeXuatFormControl = new FormControl(null, [Validators.required, Validators.min(this.MucLuongCuFormControl.value)]);
    this.EmployeeCodeFormControl = new FormControl(null);
    this.PositisionIdFormControl = new FormControl(null);
    this.PositisionNameFormControl = new FormControl(null);
    this.DateOfBirthFormControl = new FormControl(null);
    this.LyDoFormControl = new FormControl(null, [Validators.required]);

    this.nhanVienFormGroup = new FormGroup({
      EmployeeIdFormControl: this.EmployeeIdFormControl,
      EmployeeNameFormControl: this.EmployeeNameFormControl,
      OranganizationIdFormControl: this.OranganizationIdFormControl,
      OranganizationNameFormControl: this.OranganizationNameFormControl,
      MucLuongDeXuatFormControl: this.MucLuongDeXuatFormControl,
      EmployeeCodeFormControl: this.EmployeeCodeFormControl,
      PositisionIdFormControl: this.PositisionIdFormControl,
      PositisionNameFormControl: this.PositisionNameFormControl,
      DateOfBirthFormControl: this.DateOfBirthFormControl,
      MucLuongCuFormControl: this.MucLuongCuFormControl,
      LyDoFormControl: this.LyDoFormControl,
    });
    this.ngayDeXuatFormControl.setValue(new Date());
    this.phanLoaiDeXuatFormControl.setValue(this.listLoaiDeXuat[0]);
  }

  setCols() {
    this.colsListEmp = [
      { field: 'employeeCode', header: 'Mã nhân viên', textAlign: 'left', display: 'table-cell', width: "125px" },
      { field: 'employeeName', header: 'Họ tên', textAlign: 'left', display: 'table-cell', width: "150px" },
      { field: 'organizationName', header: 'Phòng ban', textAlign: 'left', display: 'table-cell', width: '200px' },
      { field: 'positionName', header: 'Chức vụ', textAlign: 'center', display: 'table-cell', width: '95px' },
      { field: 'luongHienTai', header: 'Mức lương cũ', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'luongDeXuat', header: 'Mức đề xuất tăng', textAlign: 'left', display: 'table-cell', width: '160px' },
      { field: 'mucChechLech', header: 'Mức chênh lệch', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'trangThai', header: 'Trạng thái', textAlign: 'left', display: 'table-cell', width: '100px' },
    ];

    this.selectedColumns = this.colsListEmp;

    this.colsFile = [
      { field: 'fileName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'right', type: 'number' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'right', type: 'date' },
      { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'left', type: 'string' },
    ];
  }

  async _getPermission() {
    let resource = "hrm/employee/tao-de-xuat-tang-luong/";
    let permission: any = await this.getPermission.getPermission(resource);

    if (permission.status == false) { 
      this.router.navigate(["/home"]);
      return;
    }

    let listCurrentActionResource = permission.listCurrentActionResource;
    if (listCurrentActionResource.indexOf("view") == -1) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(msg);
    }
    if (listCurrentActionResource.indexOf("add") == -1) {
      this.actionAdd = false;
    }

  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.employeeService.getMasterDataCreateDeXuatTangLuong();
    this.loading = false;
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Lấy thông tin không thành công' };
      this.showMessage(msg);
      return;
    }
    this.listEmpAdd = result.listEmp;

    if (this.listEmpAdd.length != 0) {
      this.setDefaultValue(this.listEmpAdd[0]);
      this.nguoiDeXuatFormControl.setValue(this.listEmpAdd.find(x => x.employeeId == result.loginEmployeeID));
      this.nguoiDeXuatFormControl.disable();
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  refreshFilter() {
    this.filterGlobal = '';
  }

  thoat() {
    this.router.navigate(['/employee/danh-sach-de-xuat-tang-luong']);
  }

  async TaiFileMauDeXuatTL() {
    let result: any = await this.employeeService.downloadTemplateImportDXTL();

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
      this.showMessage(msg);
    }
  }

  changeEmplyee(emp) {
    //check xem nhân viên có tồn tại trong list chưa
    var checkExist = this.listEmpSelected.find(x => x.employeeId == emp.value.employeeId);
    if (checkExist) {
      this.updateEmp(checkExist);
    } else {
      this.IsEditNV = false;
      this.EmployeeIdFormControl.setValue(emp.value);
      this.OranganizationIdFormControl.setValue(emp.value.organizationId);
      this.OranganizationNameFormControl.setValue(emp.value.organizationName);
      this.MucLuongDeXuatFormControl.setValue("");
      this.EmployeeCodeFormControl.setValue(emp.value.employeeCode);
      this.PositisionIdFormControl.setValue(emp.value.positionId);
      this.PositisionNameFormControl.setValue(emp.value.positionName);
      this.DateOfBirthFormControl.setValue(new Date(emp.value.dateOfBirth));
      this.LyDoFormControl.setValue("");
      this.MucLuongCuFormControl.setValue(emp.value.mucLuongHienTai);
      this.MucLuongDeXuatFormControl.setValidators([Validators.required, Validators.min(this.MucLuongCuFormControl.value)]);
      this.MucLuongDeXuatFormControl.updateValueAndValidity();
    }
  }

  setDefaultValue(emp) {
    // this.EmployeeIdFormControl.setValue(emp);
    // this.OranganizationIdFormControl.setValue(emp.organizationId);
    // this.OranganizationNameFormControl.setValue(emp.organizationName);
    this.MucLuongDeXuatFormControl.setValue("");
    // this.EmployeeCodeFormControl.setValue(emp.employeeCode);
    // this.PositisionIdFormControl.setValue(emp.positionId);
    // this.PositisionNameFormControl.setValue(emp.positionName);
    // this.DateOfBirthFormControl.setValue(emp.dateOfBirth);
    // this.MucLuongCuFormControl.setValue(emp.mucLuongHienTai);
    this.LyDoFormControl.setValue("");
    this.MucLuongDeXuatFormControl.setValidators([Validators.required, Validators.min(this.MucLuongCuFormControl.value)]);
    this.MucLuongDeXuatFormControl.updateValueAndValidity();
  }

  AddEmp() {
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

    let newEmp = new EmployeeInterface();

    newEmp.employeeId = this.EmployeeIdFormControl.value.employeeId;
    newEmp.employeeName = this.EmployeeIdFormControl.value.employeeName;
    newEmp.PhongBanId = this.OranganizationIdFormControl.value;
    newEmp.organizationName = this.OranganizationNameFormControl.value;
    newEmp.employeeCode = this.EmployeeCodeFormControl.value;
    newEmp.chucVuId = this.PositisionIdFormControl.value;
    newEmp.positionName = this.PositisionNameFormControl.value;
    newEmp.lyDoDeXuat = this.LyDoFormControl.value;
    newEmp.luongDeXuat = this.MucLuongDeXuatFormControl.value;
    newEmp.luongHienTai = this.MucLuongCuFormControl.value //sau đổi sau
    newEmp.mucChechLech = this.MucLuongDeXuatFormControl.value - this.MucLuongCuFormControl.value;
    newEmp.trangThai = 1; //trạng thái mới
    var checkExist = this.listEmpSelected.filter(x => x.employeeId == this.EmployeeIdFormControl.value.employeeId);
    if (checkExist.length != 0) {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Nhân viên đã tồn tại trong danh sách đề xuất' };
      this.showMessage(msg);
      return;
    }

    this.listEmpSelected.push(newEmp);
    this.def.detectChanges();
    this.clearEmp();
    this.tinhTongTang();
  }

  async EditEmp() {
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
    this.IsEditNV = false;

    let newEmp = new EmployeeInterface();
    newEmp.employeeId = this.EmployeeIdFormControl.value.employeeId;
    newEmp.employeeName = this.EmployeeIdFormControl.value.employeeName;
    newEmp.PhongBanId = this.OranganizationIdFormControl.value;
    newEmp.organizationName = this.OranganizationNameFormControl.value;
    newEmp.employeeCode = this.EmployeeCodeFormControl.value;
    newEmp.chucVuId = this.PositisionIdFormControl.value;
    newEmp.positionName = this.PositisionNameFormControl.value;
    newEmp.lyDoDeXuat = this.LyDoFormControl.value;
    newEmp.luongDeXuat = this.MucLuongDeXuatFormControl.value;
    newEmp.luongHienTai = this.MucLuongCuFormControl.value //sau đổi sau
    newEmp.mucChechLech = this.MucLuongDeXuatFormControl.value - this.MucLuongCuFormControl.value;
    newEmp.trangThai = 1; // trạng thái mới
    this.listEmpSelected = await this.listEmpSelected.filter(x => x.employeeId != newEmp.employeeId);
    this.listEmpSelected.push(newEmp);
    this.def.detectChanges();
    this.clearEmp();
    this.tinhTongTang();
  }

  removeEmp(rowData) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.listEmpSelected = this.listEmpSelected.filter(x => x.employeeId != rowData.employeeId);
        this.loading = false;
      }
    });
  }

  updateEmp(rowData) {
    this.IsEditNV = true;
    this.EmployeeIdFormControl.setValue(this.listEmpAdd.find(x => x.employeeId == rowData.employeeId));
    this.OranganizationIdFormControl.setValue(rowData.OrganizationId);
    this.OranganizationNameFormControl.setValue(rowData.organizationName);
    this.MucLuongDeXuatFormControl.setValue(rowData.luongDeXuat);
    this.EmployeeCodeFormControl.setValue(rowData.employeeCode);
    this.PositisionIdFormControl.setValue(rowData.PositionId);
    this.PositisionNameFormControl.setValue(rowData.positionName);
    this.DateOfBirthFormControl.setValue(rowData.DateOfBirth);
    this.MucLuongCuFormControl.setValue(rowData.luongHienTai);
    this.LyDoFormControl.setValue(rowData.lyDoDeXuat);
    this.MucLuongDeXuatFormControl.setValidators([Validators.required, Validators.min(this.MucLuongCuFormControl.value)]);
    this.MucLuongDeXuatFormControl.updateValueAndValidity();
  }

  clearEmp() {
    this.IsEditNV = false;
    this.nhanVienFormGroup.reset();
  }

  async taoMoiDeXuatTangLuong() {
    if (!this.deXuatTangLuongFormGroup.valid) {
      Object.keys(this.deXuatTangLuongFormGroup.controls).forEach(key => {
        if (this.deXuatTangLuongFormGroup.controls[key].valid == false) {
          this.deXuatTangLuongFormGroup.controls[key].markAsTouched();
        }
      });
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Vui lòng nhập đầy đủ thông tin các trường dữ liệu.' };
      this.showMessage(msg);
      return;
    }
    if (this.listEmpSelected.length == 0) {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Hãy chọn nhân viên được đề xuất tăng lương!' };
      this.showMessage(msg);
      return;
    }

    let deXuatTangLuong = new DeXuatTangLuongModel();
    deXuatTangLuong.TenDeXuat = this.tenDeXuatFormControl.value;
    deXuatTangLuong.LoaiDeXuat = this.phanLoaiDeXuatFormControl.value.value;
    deXuatTangLuong.NgayDeXuat = this.ngayDeXuatFormControl.value;
    deXuatTangLuong.NguoiDeXuatId = this.nguoiDeXuatFormControl.value.employeeId;
    deXuatTangLuong.TrangThai = 1;

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
      fileUpload.FileInFolder.objectType = 'DXTL';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    let result: any = await this.employeeService.taoDeXuatTangLuong(deXuatTangLuong, this.listEmpSelected, listFileUploadModel, "DXTL");
    this.loading = false;
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
      this.showMessage(msg);
      return;
    }
    let msg = { severity: 'success', summary: 'Thông báo:', detail: result.message };
    this.showMessage(msg);

    this.router.navigate(['/employee/de-xuat-tang-luong-detail', { deXuatTLId: this.encrDecrService.set(result.deXuatId) }]);
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
      const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary', cellDates: true });
      let sheetName = 'TangLuongNV_DeXuat';
      if (!workbook.Sheets[sheetName]) {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: "File không hợp lệ" };
        this.showMessage(mgs);
        return;
      }

      //lấy data từ file excel
      const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[sheetName];
      /* save data */
      let listCustomerRawData: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
      /* remove header */
      listCustomerRawData = listCustomerRawData.filter((e, index) => index != 0 && index != 1);
      /* nếu không nhập  trường required thì loại bỏ */
      listCustomerRawData = listCustomerRawData.filter(e => (e[0] && e[1]));
      /* chuyển từ raw data sang model */
      let listEmpImport: Array<importNVByExcelModel> = [];
      listCustomerRawData?.forEach(_rawData => {
        /*
        empCode: string;
        empName: string;
        oranganizationName: string;
        positionName: string;
        mucLuongCu: string;
        luongDeXuat: string;
       */
        let customer = new importNVByExcelModel();
        customer.empCode = _rawData[0] ? _rawData[0].toString().trim() : '';
        customer.luongDeXuat = _rawData[1] ? _rawData[1] : null;
        customer.lydo = _rawData[2] ? _rawData[2] : "";
        listEmpImport = [...listEmpImport, customer];
      });
      /* tắt dialog import file, bật dialog chi tiết khách hàng import */
      this.displayChooseFileImportDialog = false;
      this.openDetailImportDialog(listEmpImport);
    }
  }

  importEmp() {
    this.displayChooseFileImportDialog = true;
  }

  openDetailImportDialog(listCustomerImport) {
    let ref = this.dialogService.open(ImportNvDeXuatTangLuongComponent, {
      data: {
        listEmpImport: listCustomerImport,
        listAdded: this.listEmpSelected,
        listAllEmp: this.listEmpAdd
      },
      header: 'Nhập excel danh sách nhân viên đề xuất thay tăng lương',
      width: '85%',
      baseZIndex: 1050,
      contentStyle: {
        "max-height": "800px",
        // "over-flow": "hidden"
      }
    });
    ref.onClose.subscribe((result: any) => {
      if (result.status) {
        let listImport = this.mapLuongCu(result.returnList);
        this.listEmpSelected = this.listEmpSelected.concat(listImport);
        this.tinhTongTang();
        this.refreshFilter();
      }
    });
  }

  //map mức lương cũ với nhân viên
  mapLuongCu(returnList) {
    returnList.forEach((item) => {
      let empInfor = this.listEmpAdd.find(x => x.employeeCode == item.employeeCode);
      if (empInfor) {
        item.luongHienTai = empInfor.mucLuongHienTai != null ? empInfor.mucLuongHienTai : 0;
        item.mucChechLech = item.luongDeXuat - item.luongHienTai;
      }
    });
    return returnList;
  }

  closeChooseFileImportDialog() {
    this.cancelFile();
  }

  cancelFile() {
    let fileInput = $("#importFileProduct")
    fileInput.replaceWith(fileInput.val('').clone(true));
    this.fileName = "";
  }

  onClickImportBtn(event: any) {
    /* clear value của file input */
    event.target.value = ''
  }

  chooseFile(event: any) {
    this.fileName = event.target?.files[0]?.name;
    this.importFileExcel = event.target;
  }

  tinhTongTang() {
    this.tongTang = 0;
    this.listEmpSelected.forEach(item => {
      this.tongTang = this.tongTang + item.mucChechLech;
    });
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

}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
