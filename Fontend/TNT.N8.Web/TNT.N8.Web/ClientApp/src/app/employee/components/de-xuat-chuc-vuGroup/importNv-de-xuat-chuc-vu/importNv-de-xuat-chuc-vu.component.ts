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
  empId: string;
  empCode: string;
  empName: string;
  oranganizationName: string;
  phongBanId: string;
  chucVuId: any;
  positionName: string;
  chucVuDeXuat: string;
  chucVuDeXuatId: any;
  lydo: string;
  listStatus: Array<Note>;
  isValid: boolean;
}

class EmployeeInterface {
  nhanVienDeXuatThayDoiChucVuId: number;
  deXuatThayDoiChucVuId: number;
  employeeId: string;
  employeeName: string;
  organizationName: string;
  PhongBanId: string;
  employeeCode: string;
  DateOfBirth: Date;
  lyDoDeXuat: string;
  positionName: string;
  chucVuHienTaiId: string;
  chucVuDeXuatId: string;
  positionNameDx: string;
  trangThai: number;
}

@Component({
  selector: 'app-importNv-de-xuat-chuc-vu',
  templateUrl: './importNv-de-xuat-chuc-vu.component.html',
  styleUrls: ['./importNv-de-xuat-chuc-vu.component.css']
})
export class ImportNvDeXuatChucVuComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;


  listNote: Array<Note> = [
    /* required fields */
    { code: "required_empCode", name: "Nhập mã nhân viên" },
    { code: "required_chucVuDeXuat", name: "Nhập chức vụ đề xuất" },
    { code: "required_lyDoDeXuat", name: "Nhập lý do đề xuất" },


    /* check exist in database */
    { code: "notExist_empCode", name: "Mã nhân viên không tồn tại trong hệ thống" },

    /* duplicate in form */
    { code: "duplicate_code", name: "Đã tồn tại mã nhân viên" },

    { code: "duplicate_empInListAdded", name: "Đã tồn tại nhân viên trong danh sách đề xuất" },

    //Chức vụ có tồn tại trong hệ thống
    { code: "notExistPosition", name: "Chức vụ không tồn tại trên hệ thống" },

  ]

  //dialog data
  isPotentialCustomer: boolean = false;
  listEmpImport: Array<importNVByExcelModel> = [];

  listAdded: Array<any> = [];
  listAllEmp: Array<any> = [];

  listPosition: Array<any> = [];


  //table
  rows: number = 10;
  columns: Array<any> = [];
  selectedColumns: Array<any> = [];
  selectedEmpImport: Array<importNVByExcelModel> = [];
  customerForm: FormGroup;
  //master data

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
    this.listPosition = this.config.data.listPosition;
  }

  initTable() {
    this.columns = [
      { field: 'empCode', header: 'Mã nhân viên', textAlign: 'left', display: 'table-cell', width: '100px' },
      { field: 'chucVuDeXuat', header: 'Chức vụ đề xuất', textAlign: 'right', display: 'table-cell', width: '120px' },
      { field: 'lydo', header: 'Lý do', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'listStatus', header: 'Trạng thái', textAlign: 'left', display: 'table-cell', width: '200px' },
    ];
  }

  checkStatus(autoAdd: boolean) {
    this.listEmpImport.forEach(customer => {
      customer.chucVuDeXuat = customer.chucVuDeXuatId != null ? customer.chucVuDeXuatId.positionName : customer.chucVuDeXuat;

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

      if (!customer.chucVuDeXuat ?.trim()) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "required_chucVuDeXuat")];
        customer.isValid = false;
      }

      if (!customer.lydo ?.trim()) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "required_lyDoDeXuat")];
        customer.isValid = false;
      }

      /* duplicate in form */
      let listOtherCustomer = this.listEmpImport.filter(e => e != customer); /* lấy danh sách khách hàng trừ bản ghi đang duyệt */
      let duplicateCode = listOtherCustomer.find(e => (Boolean(customer.empCode ?.trim() ?.toLowerCase()) && (e.empCode ?.trim() ?.toLowerCase() === customer.empCode ?.trim() ?.toLowerCase())));
      if (duplicateCode) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "duplicate_code")];
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

      if (customer.chucVuDeXuat) {
        let ChucVu = this.listPosition.find(x => x.positionName.toLowerCase().trim() == customer.chucVuDeXuat.toLowerCase().trim());
        if (!ChucVu) {
          customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "notExistPosition")];
          customer.isValid = false;
        } else {
          customer.chucVuDeXuatId = ChucVu;
        }
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
    let listEmpAdditional = []
    this.selectedEmpImport.forEach(o => {
      let newEmp = new EmployeeInterface();
      newEmp.employeeId = o.empId;
      newEmp.employeeCode = o.empCode;
      newEmp.employeeName = o.empName;
      newEmp.positionNameDx = o.chucVuDeXuat;
      newEmp.chucVuDeXuatId = o.chucVuDeXuatId.positionId;
      newEmp.organizationName = o.oranganizationName;
      newEmp.positionName = o.positionName;

      newEmp.PhongBanId = o.phongBanId;
      newEmp.chucVuHienTaiId = o.chucVuId;

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

