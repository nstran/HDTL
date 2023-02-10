import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, FormArray, FormBuilder, Validators } from '@angular/forms'

//SERVICES
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';

import { EmployeeService } from '../../../../../services/employee.service';
import { CommonService } from '../../../../../../shared/services/common.service';




class importHopDongModel {
  EmployeeCode: string;
  LoaiHopDong: string;
  LoaiHopDongId: any;
  SoHopDong: string;
  SoPhuLuc: string;
  NgayKyHD: Date;
  NgayBDLamViec: Date;
  NgayKetThucHD: Date;
  ChucVu: string;
  ChucVuId: any;
  MucLuong: number;

  listStatus = [];
  isValid: boolean;
}


interface ResultDialog {
  status: boolean,
  statusImport: boolean
}

class Note {
  public code: string;
  public name: string;
}

class HopDongNhanSuModel {
  EmployeeCode: string;
  LoaiHopDongId: string;
  SoHopDong: string;
  SoPhuLuc: string;
  NgayKyHopDong: Date;

  NgayBatDauLamViec: Date;
  NgayKetThucHopDong: Date;
  PositionId: string;
  MucLuong: string;

}


@Component({
  selector: 'app-hop-dong-import-detail',
  templateUrl: './hop-dong-import-detail.component.html',
  styleUrls: ['./hop-dong-import-detail.component.css']
})
export class HopDongImportDetailComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;
  @ViewChild('myTable') myTable: Table;

  today: Date = new Date();

  listNote: Array<Note> = [
    /* required fields */
    { code: "required_EmployeeCode", name: "Nhập mã nhân viên!" },
    { code: "required_LoaiHopDong", name: "Nhập loại hợp đồng!" },
    { code: "required_SoHopDong", name: "Nhập số hợp đồng!" },
    { code: "required_SoPhuLuc", name: "Nhập số phụ lục!" },
    { code: "required_NgayKyHD", name: "Nhập ngày ký HĐ!" },
    { code: "required_NgayBDLamViec", name: "Nhập ngày bắt đầu làm việc!" },
    { code: "required_NgayKetThucHD", name: "Nhập ngày kết thục HĐ!" },
    { code: "required_ChucVu", name: "Nhập chức vụ!" },
    { code: "required_MucLuong", name: "Nhập mức lương!" },

    // check mã code trong DB
    { code: "existSoHopDong_inDB", name: "Số hợp đồng đã tồn tại trên hệ thống!" },
    { code: "existSoPhuLuc_inDB", name: "Số phụ lục đã tồn tại trên hệ thống!" },
    { code: "existEmployeeCode_inDB", name: "Mã nhân viên không tồn tại trong hệ thống!" },


    //Check mã tài sản trong list
    { code: "existSoHopDong_inList", name: "Số hợp đồng đã tồn tại trên danh sách nhập!" },
    { code: "existSoPhuLuc_inList", name: "Số phụ lục đã tồn tại trên danh sách nhập!" },

    //Check số lơn hơn 0
    { code: "mucLuong_positive", name: "Mức lương phải lơn hơn 0" },


    { code: "wrong_ChucVuName", name: "Chức vụ không đúng!" },
    { code: "wrong_LoaiHDName", name: "Loại hợp đồng không đúng!" },

  ]

  listTaiSanImport: Array<importHopDongModel> = [];

  //table
  rows: number = 10;
  columns: Array<any> = [];
  selectedEmployeeImport: Array<importHopDongModel> = [];
  listChucVu: Array<any> = [];
  listLoaiHopDong: Array<any> = [];
  listHopDong: Array<any> = [];
  employeeId: string = null;

  // inlist
  soHDInList: Array<any> = [];
  EmployeeCodeInList: Array<any> = [];
  soPhuLucInList: Array<any> = [];
  // inDb
  soHDInDB: Array<any> = [];
  soPhuLucInDB: Array<any> = [];
  employeeCodeInDB: Array<any> = [];

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private messageService: MessageService,
    public employeeService: EmployeeService,
    public commonService: CommonService,
  ) {
    if (this.config.data) {
      this.listTaiSanImport = this.config.data.listTaiSanImport;
      this.employeeId = this.config.data.employeeId;
    }
  }

  async ngOnInit() {
    this.initTable();
    await this.getMasterdata();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  initTable() {
    // numberInt
    // text
    this.columns = [
      { field: 'EmployeeCode', header: 'Mã nhân viên', textAlign: 'left', display: 'table-cell', width: '100px', type: 'text', isRequired: true, isList: true },
      { field: 'LoaiHopDong', header: 'Loại hợp đồng', textAlign: 'left', display: 'table-cell', width: '200px', type: 'text', isRequired: true, isList: true },
      { field: 'SoHopDong', header: 'Số hợp đồng', textAlign: 'left', display: 'table-cell', width: '150px', type: 'text', isRequired: true, isList: false },
      { field: 'SoPhuLuc', header: 'Số phụ lục', textAlign: 'left', display: 'table-cell', width: '150px', type: 'text', isRequired: true, isList: false },
      { field: 'NgayKyHD', header: 'Ngày ký hợp đồng', textAlign: 'center', display: 'table-cell', width: '150px', type: 'date', isRequired: true, isList: false },
      { field: 'NgayBDLamViec', header: 'Ngày bắt đầu làm việc', textAlign: 'center', display: 'table-cell', width: '150px', type: 'date', isRequired: true, isList: false },
      { field: 'NgayKetThucHD', header: 'Ngày kết thúc hợp đồng', textAlign: 'center', display: 'table-cell', width: '150px', type: 'date', isRequired: true, isList: false },
      { field: 'ChucVu', header: 'Chức vụ', textAlign: 'center', display: 'table-cell', width: '150px', type: 'text', isRequired: true, isList: true },
      { field: 'MucLuong', header: 'Mức lương', textAlign: 'left', display: 'table-cell', width: '150px', type: 'number', isRequired: false, isList: false },
      { field: 'listStatus', header: 'Trạng thái', textAlign: 'left', display: 'table-cell', width: '250px', type: 'listStatus' },
    ];
  }

  async getMasterdata() {
    let result: any = await this.employeeService.getMasterDateImportHDNS();
    this.loading = false;
    if (result.statusCode == 200) {
      this.listChucVu = result.listChucVu;
      this.listHopDong = result.listHopDong;
      this.listLoaiHopDong = result.listLoaiHopDong;

      this.soHDInList = this.listTaiSanImport.map(x => x.SoHopDong);
      this.EmployeeCodeInList = this.listTaiSanImport.map(x => x.EmployeeCode);
      this.soPhuLucInList = this.listTaiSanImport.map(x => x.SoPhuLuc);
      this.soHDInDB = this.listHopDong.map(x => x.soHopDong);
      this.soPhuLucInDB = this.listHopDong.map(x => x.soPhuLuc);
      this.employeeCodeInDB = result.listEmployeeCode

      this.checkStatus(true);
    }
    else {
      let mgs = { severity: 'error', summary: 'Thông báo', detail: result.message };
      this.showMessage(mgs);
    }
  }

  selectRow(checkValue) {
    if (checkValue) {
      this.selectedEmployeeImport = this.listTaiSanImport.filter(e => e.isValid);
    } else {
      this.selectedEmployeeImport = [];
    }
  }

  checkStatus(autoAdd: boolean) {
    this.listTaiSanImport.forEach(employee => {
      employee.listStatus = [];
      employee.isValid = true;

      employee.ChucVu = employee.ChucVuId != null ? employee.ChucVuId.positionName : employee.ChucVu;
      employee.LoaiHopDong = employee.LoaiHopDongId != null ? employee.LoaiHopDongId.categoryName : employee.LoaiHopDong;

      /* required fields */
      if (!employee?.EmployeeCode.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_EmployeeCode")];
        employee.isValid = false;
      }

      if (!employee?.LoaiHopDong.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_LoaiHopDong")];
        employee.isValid = false;
      }

      if (!employee?.SoHopDong.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_SoHopDong")];
        employee.isValid = false;
      }

      if (!employee?.SoPhuLuc.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_SoPhuLuc")];
        employee.isValid = false;
      }

      if (!employee?.NgayKyHD) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_NgayKyHD")];
        employee.isValid = false;
      }

      if (!employee?.NgayBDLamViec) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_NgayBDLamViec")];
        employee.isValid = false;
      }

      if (!employee?.NgayKetThucHD) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_NgayKetThucHD")];
        employee.isValid = false;
      }

      if (!employee?.ChucVu.trim()) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_ChucVu")];
        employee.isValid = false;
      }

      if (!employee?.MucLuong) {
        employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "required_MucLuong")];
        employee.isValid = false;
      }

      //Check duplicate in list
      if (employee.EmployeeCode) {
        let check = this.EmployeeCodeInList.filter(x => x.toUpperCase().trim() == employee.EmployeeCode.toUpperCase().trim());
        if (check.length > 1) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "existEmployeeCode_inDB")];
          employee.isValid = false;
        }
      }

      if (employee.SoHopDong) {
        let check = this.soHDInList.filter(x => x.toUpperCase().trim() == employee.SoHopDong.toUpperCase().trim());
        if (check.length > 1) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "existSoHopDong_inList")];
          employee.isValid = false;
        }
      }

      if (employee.SoPhuLuc) {
        let check = this.soPhuLucInList.filter(x => x.toUpperCase().trim() == employee.SoPhuLuc.toUpperCase().trim());
        if (check.length > 1) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "existSoPhuLuc_inList")];
          employee.isValid = false;
        }
      }

      //Check duplicate in DB
      if (employee.EmployeeCode) {
        let check = this.employeeCodeInDB.filter(x => x.toUpperCase().trim() == employee.EmployeeCode.toUpperCase().trim());
        if (check.length == 0) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "existEmployeeCode_inDB")];
          employee.isValid = false;
        }
      }

      if (employee.SoHopDong) {
        let check = this.soHDInDB.filter(x => x.toUpperCase().trim() == employee.SoHopDong.toUpperCase().trim());
        if (check.length != 0) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "existSoHopDong_inDB")];
          employee.isValid = false;
        }
      }

      if (employee.SoPhuLuc) {
        let check = this.soPhuLucInDB.filter(x => x.toUpperCase().trim() == employee.SoPhuLuc.toUpperCase().trim());
        if (check.length != 0) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "existSoPhuLuc_inDB")];
          employee.isValid = false;
        }
      }

      //check chức vụ
      if (employee.ChucVu) {
        let chucVu = this.listChucVu.find(x => x.positionName.toLowerCase().trim() == employee.ChucVu.toLowerCase().trim());
        if (!chucVu) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "wrong_ChucVuName")];
          employee.isValid = false;
        } else {
          employee.ChucVuId = chucVu;
        }
      }

      //check loại HĐ
      if (employee.LoaiHopDong) {
        let loaiHD = this.listLoaiHopDong.find(x => x.categoryName.toLowerCase().trim() == employee.LoaiHopDong.toLowerCase().trim());
        if (!loaiHD) {
          employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "wrong_LoaiHDName")];
          employee.isValid = false;
        } else {
          employee.LoaiHopDongId = loaiHD;
        }
      }

      //Số phải lớn hơn 0
      if (employee.MucLuong) {
        if (Number(employee.MucLuong) != NaN) {
          if (Number(employee.MucLuong) < 0) {
            employee.listStatus = [...employee.listStatus, this.listNote.find(e => e.code == "mucLuong_positive")];
            employee.isValid = false;
          }
        }
      }
    })
  }

  onCancel() {
    let result: ResultDialog = {
      status: false,
      statusImport: false
    };
    this.ref.close(result);
  }

  async importCustomer() {
    /* check valid list selected */
    if (this.selectedEmployeeImport.length == 0) {
      let msg = { severity: 'warn', summary: 'Thông báo', detail: 'Chọn danh sách cần import' };
      this.showMessage(msg);
      return;
    }
    let inValidRecord = this.selectedEmployeeImport.find(e => !e.isValid);
    if (inValidRecord) {
      let msg = { severity: 'error', summary: 'Thông báo', detail: 'Danh sách không hợp lệ' };
      this.showMessage(msg);
      return;
    }
    this.checkStatus(false);
    this.standardizedListCustomer();
    let listEmpAdditional: Array<HopDongNhanSuModel> = [];
    this.selectedEmployeeImport.forEach(item => {
      var newEmp = this.mapFormToEmployeeModel(item);
      listEmpAdditional.push(newEmp);
    });
    console.log('listEmpAdditional', listEmpAdditional)
    this.loading = true;
    let result: any = await this.employeeService.importHDNS(listEmpAdditional);
    this.loading = false;
    if (result.statusCode === 200) {
      let mgs = { severity: 'success', summary: 'Thông báo', detail: result.message };
      this.showMessage(mgs);
      this.ref.close(result);
    } else {
      let mgs = { severity: 'error', summary: 'Thông báo', detail: result.message };
      this.showMessage(mgs);
      this.getMasterdata();
      this.checkStatus(true);
    }
  }

  standardizedListCustomer() {
    this.listTaiSanImport.forEach(customer => {
      // customer.materialID = customer.materialID?.trim() ?? "";
    });
  }

  mapFormToEmployeeModel(hopDong): HopDongNhanSuModel {
    let hopDongModel = new HopDongNhanSuModel();
    hopDongModel.EmployeeCode = hopDong.EmployeeCode;
    hopDongModel.LoaiHopDongId = hopDong.LoaiHopDongId.categoryId;
    hopDongModel.SoHopDong = hopDong.SoHopDong;
    hopDongModel.SoPhuLuc = hopDong.SoPhuLuc;
    hopDongModel.NgayKyHopDong = hopDong.NgayKyHD;
    hopDongModel.NgayBatDauLamViec = hopDong.NgayBDLamViec;
    hopDongModel.NgayKetThucHopDong = hopDong.NgayKetThucHD;
    hopDongModel.PositionId = hopDong.ChucVuId.positionId;
    hopDongModel.MucLuong = hopDong.MucLuong;
    return hopDongModel;
  }
  //end
}

function validateString(str: string) {
  if (str === undefined) return "";
  return str.trim();
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

