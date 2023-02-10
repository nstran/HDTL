import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormArray, FormBuilder, Validators } from '@angular/forms'

//SERVICES
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';

//COMPONENTs

interface ResultDialog {
  status: boolean,
  returnList: Array<importNVByExcelModel>;
}

class Note {
  public code: string;
  public name: string;
}


class importNVByExcelModel {
  empId: number;
  empCode: string;
  empName: string;
  oranganizationName: string;
  phongBanId: string;
  chucVuId: string;
  positionName: string;
  mucLuongCu: number;
  luongDeXuat: number;
  luongChenhLech: number;
  lydo: string;

  listStatus: Array<Note>;
  isValid: boolean;
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

@Component({
  selector: 'app-importNv-de-xuat-tang-luong',
  templateUrl: './importNv-de-xuat-tang-luong.component.html',
  styleUrls: ['./importNv-de-xuat-tang-luong.component.css']
})
export class ImportNvDeXuatTangLuongComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;


  listNote: Array<Note> = [
    /* required fields */
    { code: "required_empCode", name: "Nhập mã nhân viên" },
    { code: "required_luongDeXuat", name: "Nhập lương đề xuất" },
    { code: "required_lyDo", name: "Nhập lý do" },

    /* check exist in database */
    { code: "notExist_empCode", name: "Mã nhân viên không tồn tại trong hệ thống" },

    /* duplicate in form */
    { code: "duplicate_code", name: "Đã tồn tại mã nhân viên" },

    { code: "duplicate_empInListAdded", name: "Đã tồn tại nhân viên trong danh sách đề xuất" },

    { code: "luongDeXuat", name: "Lương đề xuất phải lướn hơn 0" },
  ]

  //dialog data
  isPotentialCustomer: boolean = false;
  listEmpImport: Array<importNVByExcelModel> = [];

  listAdded: Array<any> = [];
  listAllEmp: Array<any> = [];

  //table
  rows: number = 10;
  columns: Array<any> = [];
  selectedColumns: Array<any> = [];
  selectedEmpImport: Array<importNVByExcelModel> = [];
  customerForm: FormGroup;
  //master data
  listMaterialCode: Array<any> = [];
  listMaterialTypeCode: Array<any> = [];
  listMaterialGroupCode: Array<any> = [];
  listCarModelCode: Array<any> = [];
  listJobCode: Array<any> = [];
  listUnitCode: Array<any> = [];
  listVendorCode: Array<any> = [];

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private formBuilder: FormBuilder,

  ) {
  }

  async ngOnInit() {
    this.initTable();
    this.getDataFromCustomerImportComponent();
    this.checkStatus(true);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  getDataFromCustomerImportComponent() {
    this.listEmpImport = this.config.data.listEmpImport;
    this.listAdded = this.config.data.listAdded;
    this.listAllEmp = this.config.data.listAllEmp;
  }

  initTable() {
    this.columns = [
      { field: 'empCode', header: 'Mã nhân viên', textAlign: 'left', display: 'table-cell', width: '100px' },
      { field: 'luongDeXuat', header: 'Lương đề xuất', textAlign: 'right', display: 'table-cell', width: '100px' },
      { field: 'lydo', header: 'Lý do', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'listStatus', header: 'Trạng thái', textAlign: 'left', display: 'table-cell', width: '200px' },
    ];
  }

  checkStatus(autoAdd: boolean) {

    this.listEmpImport.forEach(customer => {
      //Danh sách nhân viên đã được add trong đề xuất
      let listAddedEmpCode = this.listAdded.map(item => item.employeeCode);
      //Danh sách toàn bộ nhân viên
      let listAllEmpCode = this.listAllEmp.map(item => item.employeeCode);

      customer.listStatus = [];
      customer.isValid = true;
      /* required fields */
      if (!customer.empCode ?.trim()) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "required_empCode")];
        customer.isValid = false;
      }

      if (!customer.luongDeXuat.toString() ?.trim()) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "required_luongDeXuat")];
        customer.isValid = false;
      }

      if (!customer.lydo ?.trim()) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "required_lyDo")];
        customer.isValid = false;
      }


      /* duplicate in form */
      let listOtherCustomer = this.listEmpImport.filter(e => e != customer); /* lấy danh sách khách hàng trừ bản ghi đang duyệt */
      let duplicateCode = listOtherCustomer.find(e => (Boolean(customer.empCode ?.trim() ?.toLowerCase()) && (e.empCode ?.trim() ?.toLowerCase() === customer.empCode ?.trim() ?.toLowerCase())));
      if (duplicateCode) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "duplicate_code")];
        customer.isValid = false;
      }

      if (customer.luongDeXuat < 0) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "luongDeXuat")];
        customer.isValid = false;
      }

      //Check mã nhân viên có tồn tại trong hệ thống
      var checkExsitInDB = listAllEmpCode.find(i => i == customer.empCode);
      if (!checkExsitInDB) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "notExist_empCode")];
        customer.isValid = false;
      }

      //Check mã nhân viên có tồn tại trong list đề xuất đã thêm chưa
      var checkExsitInListAdded = listAddedEmpCode.find(i => i == customer.empCode);
      if (checkExsitInListAdded) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "duplicate_empInListAdded")];
        customer.isValid = false;
      }


      var emp = this.listAllEmp.find(i => i.employeeCode == customer.empCode);
      if (emp) {
        customer.empId = emp.employeeId;
        customer.empName = emp.employeeName;
        customer.positionName = emp.positionName;
        customer.oranganizationName = emp.organizationName;
        customer.phongBanId = emp.organizationId;
        customer.chucVuId = emp.positionId;
      }
    });
    /* auto add to valid list */
    if (autoAdd) this.selectedEmpImport = this.listEmpImport.filter(e => e.isValid);
  }


  async importCustomer() {
    /* check valid list selected */
    if (this.selectedEmpImport.length == 0) {
      let msg = { severity: 'warn', summary: 'Thông báo', detail: 'Chọn danh sách cần import' };
      this.showMessage(msg);
      return;
    }

    let inValidRecord = this.selectedEmpImport.find(e => !e.isValid);
    if (inValidRecord) {
      let msg = { severity: 'error', summary: 'Thông báo', detail: 'Danh sách không hợp lệ' };
      this.showMessage(msg);
      return;
    }

    this.checkStatus(false);
    // this.standardizedListCustomer();
    let listEmpAdditional = []
    this.selectedEmpImport.forEach(o => {
      let newEmp = new EmployeeInterface();
      newEmp.employeeId = o.empId;
      newEmp.employeeCode = o.empCode;
      newEmp.employeeName = o.empName;
      newEmp.luongDeXuat = Number(o.luongDeXuat.toString().split(",").join(''));
      newEmp.organizationName = o.oranganizationName;
      newEmp.positionName = o.positionName;

      newEmp.PhongBanId = o.phongBanId;
      newEmp.chucVuId = o.chucVuId;

      newEmp.lyDoDeXuat = o.lydo;
      newEmp.trangThai = 1;//trạng thái mới
      listEmpAdditional.push(newEmp);
    });

    let result: ResultDialog = {
      status: true,
      returnList: listEmpAdditional,
    };
    this.ref.close(result);
  }

  onCancel() {
    let result: ResultDialog = {
      status: false,
      returnList: null
    };
    this.ref.close(result);
  }

  // standardizedListCustomer() {
  //   this.listEmpImport.forEach(customer => {
  //     customer.empId = customer.empId.trim();
  //     customer.empCode = customer.empCode?.trim();
  //     customer.materialGroup = customer.materialGroup?.trim() ?? "";
  //     customer.unit = customer.unit?.trim() ?? "";
  //     customer.jobType = customer.jobType?.trim() ?? "";
  //     customer.modelCar = customer.modelCar?.trim() ?? "";
  //     customer.vendorId = customer.vendorId?.trim() ?? "";
  //     customer.retailPrice = customer.retailPrice;
  //     customer.costPrice = customer.costPrice;
  //     if (!customer.retailPrice) customer.retailPrice = 0;
  //     if (!customer.costPrice) customer.costPrice = 0;
  //   });
  // }
  //end
}

function validateString(str: string) {
  if (str === undefined) return "";
  return str.trim();
}
function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

