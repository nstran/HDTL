import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormArray, FormBuilder, Validators } from '@angular/forms'

//SERVICES
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';

//COMPONENTs
import { EmployeeService } from '../../services/employee.service'

interface ResultDialog {
  status: boolean,
  statusImport: boolean,
  listCandidateReturn: Array<any>
}

class Note {
  public code: string;
  public name: string;
}


class importCandidateByExcelModel {
  candidateId: string;
  candidateName: string;
  dateOfBirth: Date;
  phone: string;
  sex: number;
  chanelCode: string;
  email: string;
  address: string;
  applicationDate: Date;
  isValid: boolean;
  listStatus: Array<Note>;
}

class candidateList {
  candidateId: string;
  fullName: string;
  dateOfBirth: Date;
  phone: string;
  sex: number;
  recruitmentChannelCode: string;
  email: string;
  address: string;
  applicationDate: Date = null;
}

@Component({
  selector: 'app-candidate-import-detail',
  templateUrl: './candidate-import-detail.component.html',
  styleUrls: ['./candidate-import-detail.component.css']
})
export class CandidateImportDetailComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;

  listCandidateReturn: Array<any> = [];
  listNote: Array<Note> = [
    /* required fields */
    { code: "required_candidateName", name: "Nhập tên ứng viên" },
    { code: "required_dateOfBirth", name: "Nhập ngày sinh" },
    { code: "required_phone", name: "Nhập số điện thoại" },

    /* check exist in database */
    { code: "exist_code", name: "Đã tồn tại ứng viên" },

    /* duplicate in form */
    { code: "duplicate_phone", name: "Trùng số điện thoại" },

    //check mã kênh , giới tính
    { code: "exist_chanelCode", name: "Không tồn tại kênh tuyển dụng" },
    { code: "exist_sexCode", name: "Không tồn tại mã giới tính" },

    //Check tồn tại trong list ứng viên
    { code: "exist_CandiDate", name: "Ứng viên đã tồn tại trong danh sách" },

    //Check tồn tại trong những ứng viên không đạt
    { code: "exist_CandiDateKhongDat", name: "Ứng viên không đạt ở đợt tuyển dụng [Tên chiến dịch tuyển dụng]" },

    //Check tồn tại trong list nhân viên
    { code: "exist_CandiDateVsEmp", name: "Ứng viên đang là nhân viên thuộc phòng ban [Tên phòng ban]" },

    //Check tồn tại trong list nhân viên
    { code: "exist_CandiDateVsEmpNghiViec", name: "Ứng viên là nhân viên thuộc phòng ban [Tên phòng ban] đã nghỉ việc " },

  ]

  //dialog data
  listCandidateDataImport: boolean = false;
  listCandidateImport: Array<importCandidateByExcelModel> = [];

  //table
  rows: number = 10;
  columns: Array<any> = [];
  selectedColumns: Array<any> = [];
  selectedCandidateImport: Array<importCandidateByExcelModel> = [];
  candidateForm: FormGroup;



  //master data
  vacanciesId: string = '00000000-0000-0000-0000-000000000000';
  listCandidate: Array<any> = [];
  listCandidateAdd: Array<any> = [];

  listEmp: Array<any> = [];
  listEmpNghiViec: Array<any> = [];

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private formBuilder: FormBuilder,
    private employeeService: EmployeeService,

  ) {
    this.listCandidateImport = this.config.data.listCandidateDataImport;
    this.vacanciesId = this.config.data.vacanciesId;
    this.listCandidateAdd = this.config.data.listCandidateAdd;
  }

  async ngOnInit() {
    this.initTable();
    this.getDataFromCandidateImportComponent();
    await this.getMasterdata();
    this.checkStatus(true);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  getDataFromCandidateImportComponent() {
    this.listCandidateImport.forEach(item => {
      let date = new Date(item.applicationDate)
      if (item.applicationDate != null) item.applicationDate = date;

      let dateOfBirth = new Date(item.dateOfBirth)
      if (item.dateOfBirth != null) item.dateOfBirth = dateOfBirth;
    })
  }

  initTable() {
    this.columns = [
      { field: 'candidateName', header: 'Tên ứng viên', textAlign: 'center', display: 'table-cell', width: '30%' },
      { field: 'dateOfBirth', header: 'Ngày sinh', textAlign: 'center', display: 'table-cell', width: '10%' },
      { field: 'phone', header: 'Số điện thoại', textAlign: 'center', display: 'table-cell', width: '10%' },
      { field: 'sex', header: 'Giới tính', textAlign: 'center', display: 'table-cell', width: '5%' },
      { field: 'chanelCode', header: 'Kênh tuyển dụng', textAlign: 'center', display: 'table-cell', width: '5%' },
      { field: 'email', header: 'Email', textAlign: 'center', display: 'table-cell', width: '15%' },
      { field: 'address', header: 'Địa chỉ', textAlign: 'center', display: 'table-cell', width: '15%' },
      { field: 'applicationDate', header: 'Ngày ứng tuyển', textAlign: 'center', display: 'table-cell', width: '10%' },
      { field: 'listStatus', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '30%' },
    ];
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.employeeService.getCandidateImportDetai();
    if (result.statusCode === 200) {
      this.listCandidate = result.listCandidate;
      //loại bỏ các ứng viên trong dsach ứng viên của vị trí và đã có trạng thái trượt ứng tuyển: (6: Không đạt)
      let listUngVienId = this.listCandidateAdd.map(x => x.candidateId);
      this.listCandidate = this.listCandidate.filter(x => listUngVienId.indexOf(x.candidateId) == -1 && x.status == 6);

      this.listEmp = result.listEmp;
      this.listEmpNghiViec = result.listEmpNghiViec;
    }
    this.loading = false;
  }

  checkStatus(autoAdd: boolean) {

    this.listCandidateImport.forEach(candidate => {
      candidate.isValid = true;
      candidate.listStatus = [];
      /* required fields */
      if (!candidate.candidateName ?.trim()) {
        candidate.listStatus = [...candidate.listStatus, this.listNote.find(e => e.code == "required_candidateName")];
        candidate.isValid = false;
      }

      if (candidate.dateOfBirth == null) {
        candidate.listStatus = [...candidate.listStatus, this.listNote.find(e => e.code == "required_dateOfBirth")];
        candidate.isValid = false;
      }
      if (!candidate.phone ?.trim()) {
        candidate.listStatus = [...candidate.listStatus, this.listNote.find(e => e.code == "required_phone")];
        candidate.isValid = false;
      }

      var addErrExist_CandiDate = false;
      //Check SDT vs Email với list ứng viên trong dsach
      if (candidate.email) {
        var checkEmail = this.listCandidateAdd.find(e => e.email.trim().toLowerCase() == candidate.email.trim().toLowerCase());
        if (checkEmail) {
          candidate.listStatus = [...candidate.listStatus, this.listNote.find(e => e.code == "exist_CandiDate")];
          candidate.isValid = false;
          addErrExist_CandiDate = true;
        }
      }

      if (candidate.phone) {
        var checkSDT = this.listCandidateAdd.find(e => e.phone.trim().toLowerCase() == candidate.phone.trim().toLowerCase());
        if (checkSDT && addErrExist_CandiDate == false) {
          candidate.listStatus = [...candidate.listStatus, this.listNote.find(e => e.code == "exist_CandiDate")];
          candidate.isValid = false;
          addErrExist_CandiDate = true;
        }
      }

      if (candidate.candidateName) {
        var checkHoTen = this.listCandidateAdd.find(e => e.fullName.trim().toLowerCase() == candidate.candidateName.trim().toLowerCase());
        if (checkHoTen && addErrExist_CandiDate == false) {
          candidate.listStatus = [...candidate.listStatus, this.listNote.find(e => e.code == "exist_CandiDate")];
          candidate.isValid = false;
          addErrExist_CandiDate = true;
        }
      }

      var addErr_CandiDateKhongDat = false;
      //Check SDT vs Email với list ứng viên trong không đạt
      if (candidate.email) {
        var checkEmail = this.listCandidate.find(e => e.email.trim().toLowerCase() == candidate.email.trim().toLowerCase());
        if (checkEmail) {
          var errMess = this.listNote.find(e => e.code == "exist_CandiDateKhongDat");
          errMess.name = errMess.name.replace('[Tên chiến dịch tuyển dụng]', checkEmail.recruitmentCampaignName);
          candidate.listStatus = [...candidate.listStatus, errMess];
          candidate.isValid = false;
          addErr_CandiDateKhongDat = true;
        }
      }

      if (candidate.phone) {
        var checkSDT = this.listCandidate.find(e => e.phone.trim().toLowerCase() == candidate.phone.trim().toLowerCase());
        if (checkSDT && addErr_CandiDateKhongDat == false) {
          var errMess = this.listNote.find(e => e.code == "exist_CandiDateKhongDat");
          errMess.name = errMess.name.replace('[Tên chiến dịch tuyển dụng]', checkEmail.recruitmentCampaignName);
          candidate.listStatus = [...candidate.listStatus, errMess];
          candidate.isValid = false;
          addErr_CandiDateKhongDat = true;
        }
      }

      if (candidate.candidateName) {
        var checkHoTen = this.listCandidate.find(e => e.candidateName.trim().toLowerCase() == candidate.candidateName.trim().toLowerCase());
        if (checkHoTen && addErr_CandiDateKhongDat == false) {
          var errMess = this.listNote.find(e => e.code == "exist_CandiDateKhongDat");
          errMess.name = errMess.name.replace('[Tên chiến dịch tuyển dụng]', checkEmail.recruitmentCampaignName);
          candidate.listStatus = [...candidate.listStatus, errMess];
          candidate.isValid = false;
          addErr_CandiDateKhongDat = true;
        }
      }

      var addErr_Emp = false;
      //Check SDT vs Email với list nhân viên
      if (candidate.email) {
        var checkEmail = this.listEmp.find(e => e.email.trim().toLowerCase() == candidate.email.trim().toLowerCase());
        if (checkEmail) {
          var errMess = this.listNote.find(e => e.code == "exist_CandiDateVsEmp");
          errMess.name = errMess.name.replace('[Tên phòng ban]', checkEmail.organizationName);
          candidate.listStatus = [...candidate.listStatus, errMess];
          candidate.isValid = false;
          addErr_Emp = true;
        }
      }

      if (candidate.phone) {
        var checkSDT = this.listEmp.find(e => e.phone.trim().toLowerCase() == candidate.phone.trim().toLowerCase());
        if (checkSDT && addErr_Emp == false) {
          var errMess = this.listNote.find(e => e.code == "exist_CandiDateVsEmp");
          errMess.name = errMess.name.replace('[Tên phòng ban]', checkEmail.organizationName);
          candidate.listStatus = [...candidate.listStatus, errMess];
          candidate.isValid = false;
          addErr_Emp = true;
        }
      }

      if (candidate.candidateName) {
        var checkHoTen = this.listEmp.find(e => e.employeeName.trim().toLowerCase() == candidate.candidateName.trim().toLowerCase());
        if (checkHoTen && addErr_Emp == false) {
          var errMess = this.listNote.find(e => e.code == "exist_CandiDateVsEmp");
          errMess.name = errMess.name.replace('[Tên phòng ban]', checkEmail.organizationName);
          candidate.listStatus = [...candidate.listStatus, errMess];
          candidate.isValid = false;
          addErr_Emp = true;
        }
      }


      var addErr_EmpNghiViec = false;
      //Check SDT vs Email với list nhân viên nghỉ việc
      if (candidate.email) {
        var checkEmail = this.listEmpNghiViec.find(e => e.email.trim().toLowerCase() == candidate.email.trim().toLowerCase());
        if (checkEmail) {
          var errMess = this.listNote.find(e => e.code == "exist_CandiDateVsEmpNghiViec");
          errMess.name = errMess.name.replace('[Tên phòng ban]', checkEmail.organizationName);
          candidate.listStatus = [...candidate.listStatus, errMess];
          candidate.isValid = false;
          addErr_EmpNghiViec = true;
        }
      }

      if (candidate.phone) {
        var checkSDT = this.listEmpNghiViec.find(e => e.phone.trim().toLowerCase() == candidate.phone.trim().toLowerCase());
        if (checkSDT && addErr_EmpNghiViec == false) {
          var errMess = this.listNote.find(e => e.code == "exist_CandiDateVsEmpNghiViec");
          errMess.name = errMess.name.replace('[Tên phòng ban]', checkEmail.organizationName);
          candidate.listStatus = [...candidate.listStatus, errMess];
          candidate.isValid = false;
          addErr_EmpNghiViec = true;
        }
      }

      if (candidate.candidateName) {
        var checkHoTen = this.listEmpNghiViec.find(e => e.employeeName.trim().toLowerCase() == candidate.candidateName.trim().toLowerCase());
        if (checkHoTen && addErr_EmpNghiViec == false) {
          var errMess = this.listNote.find(e => e.code == "exist_CandiDateVsEmpNghiViec");
          errMess.name = errMess.name.replace('[Tên phòng ban]', checkEmail.organizationName);
          candidate.listStatus = [...candidate.listStatus, errMess];
          candidate.isValid = false;
          addErr_EmpNghiViec = true;
        }
      }

    });
    /* auto add to valid list */
    if (autoAdd) this.selectedCandidateImport = this.listCandidateImport.filter(e => e.isValid);
  }

  onCancel() {
    let result: ResultDialog = {
      status: false,
      statusImport: false,
      listCandidateReturn: []
    };
    this.ref.close(result);
  }

  // ĐẨY DỮ LIỆU TỪ GRID LÊN SERVER
  async importCandidate() {
    /* check valid list selected */
    if (this.selectedCandidateImport.length == 0) {
      let msg = { severity: 'warn', summary: 'Thông báo', detail: 'Chọn danh sách cần import' };
      this.showMessage(msg);
      return;
    }

    let inValidRecord = this.selectedCandidateImport.find(e => !e.isValid);
    if (inValidRecord) {
      let msg = { severity: 'error', summary: 'Thông báo', detail: 'Danh sách không hợp lệ' };
      this.showMessage(msg);
      return;
    }

    //this.checkStatus(false);

    this.standardizedListCandidate();
    let listCandidateAdditional = []

    this.selectedCandidateImport.forEach(o => {
      let newCandidate = new candidateList();
      newCandidate.candidateId = this.emptyGuid;
      newCandidate.fullName = o.candidateName;
      newCandidate.dateOfBirth = convertToUTCTime(new Date(o.dateOfBirth));
      newCandidate.phone = o.phone;
      newCandidate.sex = o.sex;
      newCandidate.recruitmentChannelCode = o.chanelCode;
      newCandidate.email = o.email;
      newCandidate.address = o.address;
      newCandidate.applicationDate = o.applicationDate == null ? null : convertToUTCTime(new Date(o.applicationDate));

      listCandidateAdditional.push(newCandidate);
    });

    this.loading = true;
    let result: any = await this.employeeService.importListCandidate(listCandidateAdditional, this.vacanciesId);
    this.loading = false;
    if (result.statusCode === 200) {
      let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Nhập excel thành công' };
      this.showMessage(mgs);
      let resultReturn: ResultDialog = {
        status: true,
        statusImport: true,
        listCandidateReturn: result.listCandidate
      };
      this.ref.close(resultReturn);
    } else {
      let mgs = { severity: 'error', summary: 'Thông báo', detail: 'Nhập excel thất bại' };
      this.showMessage(mgs);
      let result: ResultDialog = {
        status: false,
        statusImport: false,
        listCandidateReturn: []
      };
      this.ref.close(result);
    }
  }

  standardizedListCandidate() {
    this.listCandidateImport.forEach(candidate => {
      candidate.candidateId = candidate.candidateId ?.trim() ?? "";
      candidate.candidateName = candidate.candidateName ?.trim() ?? "";
      candidate.dateOfBirth = candidate.dateOfBirth;
      candidate.phone = candidate.phone;
      candidate.sex = candidate.sex;
      candidate.chanelCode = candidate.chanelCode ?.trim() ?? "";
      candidate.email = candidate.email ?.trim() ?? "";
      candidate.address = candidate.address ?.trim() ?? "";

      candidate.applicationDate = candidate.applicationDate == null ? null : candidate.applicationDate;
    });
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

