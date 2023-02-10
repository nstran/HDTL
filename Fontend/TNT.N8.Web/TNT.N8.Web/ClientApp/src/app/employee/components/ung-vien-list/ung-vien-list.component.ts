import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmationService, MenuItem, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { EmployeeService } from '../../services/employee.service';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { DialogService } from 'primeng';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { EmployeeModel, CreateEmployeeModel } from '../../models/employee.model';
import { ContactModel } from '../../../../app/shared/models/contact.model';
import { UserModel } from '../../../../app/shared/models/user.model';
import { TemplateVacanciesEmailComponent } from './../../../shared/components/template-vacancies-email/template-vacancies-email.component';

class EmployeeCandidateModel {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  employeeCodeName: string;
  phone: string;
}

class ColumnExcel {
  code: string;
  name: string;
  width: number;
}

class InterviewSheduleModel {
  interviewScheduleId: string;
  vacanciesId: string;
  candidateId: string;
  fullName: string;
  interviewTitle: string;
  interviewDate: Date;
  address: string;
  email: string;
  interviewScheduleType: number;
  listEmployeeId: Array<any> = [];
}

class emailPVModel {
  listCandidate: Array<candidateSenEmailPVModel> = []; // danh sách email Ứng viên
  vancaciesName: string; // Vị trí tuyển dụng
  personInChagerName: string = ''; // tên người phụ trách
  personInChagerPhone: string = null; // SĐT người phụ trách
  workplace: string;
  subject: string;
  sendContent: string;
}

class candidateSenEmailPVModel {
  email: string;
  fullName: string;
  interviewTime: Date;
  addressOrLink: string;
  interviewScheduleType: number;
  listInterviewerEmail: Array<string> = []; //danh sách email Người PV
}

@Component({
  selector: 'app-ung-vien-list',
  templateUrl: './ung-vien-list.component.html',
  styleUrls: ['./ung-vien-list.component.css'],
  providers: [DatePipe]
})
export class UngVienListComponent implements OnInit {
  @ViewChild('myTable') myTable: Table;

  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  today: Date = new Date();
  auth = JSON.parse(localStorage.getItem('auth'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  filterGlobal: string = '';
  innerWidth: number = 0; //number window size first

  leftColNumber: number = 12;
  rightColNumber: number = 0;

  //Search
  fullName: string = '';
  selectedRecruitmentCampaign: Array<any> = [];
  selectedVacancies: Array<any> = [];
  ApplicationDateFrom: Date = null;
  ApplicationDateTo: Date = null;
  email: string = '';
  phone: string = '';
  selectdeRecruitmentChannel: Array<any> = [];
  selectedStatus: Array<any> = [];
  // danh sach
  actions: MenuItem[] = [];
  listCandidate: Array<any> = [];
  listRecruitmentCampaign: Array<any> = [];
  listVacancies: Array<any> = [];
  listRecruitmentChannel: Array<any> = [];
  listStatus: Array<any> = [
    {
      value: 1,
      lable: 'Mới',
    },
    {
      value: 2,
      lable: 'Hẹn phỏng vấn',
    },
    {
      value: 3,
      lable: 'Đạt phỏng vấn',
    },
    {
      value: 4,
      lable: 'Gửi offer',
    },
    {
      value: 5,
      lable: 'Từ chối offer',
    },
    {
      value: 6,
      lable: 'Không đạt',
    },
    {
      value: 7,
      lable: 'NV/Thử việc',
    },
  ];
  selection: Array<any> = [];

  colsList: any[];
  selectedColumns: any[];
  listCandidatePV: Array<InterviewSheduleModel> = [];
  selectedCandidate: any = null;
  // Lịch PV
  interViewForm: FormGroup;
  titleFormControl: FormControl;
  applicationDateControl: FormControl;
  addressOrLinkControl: FormControl;
  interViewNameControl: FormControl;

  displayAddInterviewSche: boolean = false;
  month: string = '';
  year: number = (new Date()).getFullYear();

  employeeModel = new CreateEmployeeModel();
  contactModel = new ContactModel();
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  password: string = this.systemParameterList.find(x => x.systemKey == "DefaultUserPassword").systemValueString;
  userModel: UserModel = {
    UserId: null, Password: this.password, UserName: '', EmployeeId: '', EmployeeCode: '', Disabled: false, CreatedById: 'DE2D55BF-E224-4ADA-95E8-7769ECC494EA', CreatedDate: null, UpdatedById: null, UpdatedDate: null, Active: true
  };

  displayRejectEmail: boolean = false;
  selectedInterviewColumns: any[];
  listEmployeeInterview: Array<EmployeeCandidateModel> = [];
  listInterviewType: Array<any> = [
    {
      categoryName: 'Trực tiếp',
      categoryId: 1,
    },
    {
      categoryName: 'Online',
      categoryId: 2,
    },
  ];

  constructor(
    private router: Router,
    private datePipe: DatePipe,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    private confirmationService: ConfirmationService,
    public dialogService: DialogService,
  ) {
    this.innerWidth = window.innerWidth;
  }

  ngOnInit(): void {
    this.setTable();
    this.setControlDialog();
    this.getMasterData();
  }

  setTable() {
    this.colsList = [
      { field: 'index', header: '#', width: '20px', textAlign: 'center' },
      { field: 'fullName', header: 'Tên ứng viên', width: '150px', textAlign: 'left' },
      { field: 'recruitmentCampaignName', header: 'Tên chiến dịch', width: '150px', textAlign: 'left' },
      { field: 'vacanciesName', header: 'Tên vị trí', width: '100px', textAlign: 'left' },
      { field: 'applicationDate', header: 'Ngày ứng tuyển', width: '100px', textAlign: 'right' },
      { field: 'email', header: 'Email', width: '100px', textAlign: 'left' },
      { field: 'phone', header: 'Số điện thoại', width: '100px', textAlign: 'right' },
      { field: 'recruitmentChannelName', header: 'Kênh ứng tuyển', width: '100px', textAlign: 'left' },
      { field: 'statusName', header: 'Trạng thái', width: '100px', textAlign: 'center' },
      { field: 'action', header: 'Thao tác', width: '50px', textAlign: 'center' },
    ];
    this.selectedColumns = this.colsList;
    this.selectedInterviewColumns = [
      { field: 'index', header: 'STT', width: '10%', textAlign: 'center', color: '#f44336' },
      { field: 'fullName', header: 'Tên', width: '20%', textAlign: 'left', color: '#f44336' },
      { field: 'interViewName', header: 'Người phỏng vấn', width: '18%', textAlign: 'left', color: '#f44336' },
      { field: 'interviewType', header: 'Loaị phỏng vấn', width: '12%', textAlign: 'center', color: '#f44336' },
      { field: 'address', header: 'Địa chỉ/ Link', width: '25%', textAlign: 'left', color: '#f44336' },
      { field: 'interviewDate', header: 'Ngày phỏng vấn', width: '15%', textAlign: 'left', color: '#f44336' },
    ];
  }

  async getMasterData() {
    this.loading = true;
    this.employeeService.getMasterSearchCandidate().subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.listRecruitmentCampaign = result.listRecruitmentCampaign;
        this.listVacancies = result.listVacancies;
        this.listRecruitmentChannel = result.listRecruitmentChannel;
        this.searchCandidate();

      }
      else {
        this.showMessage('error', result.messageCode);
        this.awaitResult = false;
      };
    });
  }
  setControlDialog() {
    this.titleFormControl = new FormControl('', Validators.required);
    this.addressOrLinkControl = new FormControl('', Validators.required);
    this.applicationDateControl = new FormControl(null, Validators.required);
    this.interViewNameControl = new FormControl(null, Validators.required);

    this.interViewForm = new FormGroup({
      interViewNameControl: this.interViewNameControl,
      titleFormControl: this.titleFormControl,
      addressOrLinkControl: this.addressOrLinkControl,
      applicationDateControl: this.applicationDateControl,
    });
  }

  refreshFilter() {
    if (this.filterGlobal != '') {
      this.myTable.reset();
    }
    this.filterGlobal = '';
    this.fullName = '';
    this.selectedRecruitmentCampaign = [];
    this.selectedVacancies = [];
    this.ApplicationDateFrom = null;
    this.ApplicationDateTo = null;
    this.email = '';
    this.phone = '';
    this.selectdeRecruitmentChannel = [];
    this.selectedStatus = [];

    this.searchCandidate();
  }

  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 9;
        this.rightColNumber = 3;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }


  setColorOfStatusCandidate() {
    this.listCandidate.forEach(item => {
      switch (item.statusCode) {
        case "UVMOI":
          item.backgroundColorForStatus = '#c4c3d0';
          break;
        case "UVHPV":
          item.backgroundColorForStatus = '#87cefa';
          break;
        case "UVDPV":
          item.backgroundColorForStatus = '#29ab87';
          break;
        case "UVGOF":
          item.backgroundColorForStatus = '#4cbb17';
          break;
        case "UVTC":
          item.backgroundColorForStatus = '#ee6363';
          break;
        case "UVKD":
          item.backgroundColorForStatus = '#FF0000';
          break;
      }
    });
  }

  onChangeAction(rowData: any) {
    this.actions = [];
    this.selectedCandidate = null;
    this.selectedCandidate = rowData;
    // Hẹn PV
    let item1: MenuItem = {
      id: '1', label: 'Hẹn phỏng vấn', command: () => {
        this.addInterviewSchedule(rowData);
      }
    }

    // Gửi offer
    let item2: MenuItem = {
      id: '2', label: 'Gửi offer', command: () => {
        this.listCandidatePV = []
        this.listCandidatePV.push(rowData);

        let emailData = new emailPVModel();
        emailData = this.getInfomationContentEmail();
        let ref = this.dialogService.open(TemplateVacanciesEmailComponent, {
          data: {
            emailType: 'OFFER',
            emailData: emailData
          },
          header: 'Gửi email offer',
          width: '65%',
          baseZIndex: 1030,
          contentStyle: {
            "min-height": "190px",
            "max-height": "800px",
            "overflow": "auto"
          }
        });

        ref.onClose.subscribe(async (result: any) => {
          if (result) {
            if (result.status) {
              this.loading = true;
              let lstCandidateId: Array<string> = []

              lstCandidateId.push(rowData.candidateId);
              let result: any = await this.employeeService.updateStatusCandidateFromVacancies(lstCandidateId, 4, rowData.vacanciesId);
              if (result.statusCode == 200) {
                //this.listCandidate = result.listCandidate;
                this.searchCandidate();
                this.setColorOfStatusCandidate();
                this.loading = false;
                this.showMessage('success', 'Cập nhập trạng thái thành công.');
              }
              else
                this.loading = false;
            }
          }
          this.listCandidatePV = []
        });
      }
    }

    // Từ chối offer
    let item3: MenuItem = {
      id: '3', label: 'Từ chối offer', command: async () => {
        await this.updateCandidate(rowData, 5);
      }
    }

    // Không đạt
    let item4: MenuItem = {
      id: '4', label: 'Không đạt', command: async () => {
        this.listCandidatePV.push(rowData)
        let emailData = new emailPVModel();
        emailData = this.getInfomationContentEmail();
        let ref = this.dialogService.open(TemplateVacanciesEmailComponent, {
          data: {
            emailType: 'FAIL',
            emailData: emailData
          },
          header: 'Gửi email thông báo kết quả - Không đạt',
          width: '65%',
          baseZIndex: 1030,
          contentStyle: {
            "min-height": "190px",
            "max-height": "800px",
            "overflow": "auto"
          }
        });

        ref.onClose.subscribe(async (result: any) => {
          if (result) {
            if (result.status) {
              this.loading = true;

              let lstCandidateId: Array<string> = []
              lstCandidateId.push(rowData.candidateId);
              let result: any = await this.employeeService.updateStatusCandidateFromVacancies(lstCandidateId, 6, rowData.vacanciesId);
              if (result.statusCode == 200) {
                //                this.listCandidate = result.listCandidate;
                this.searchCandidate();
                this.setColorOfStatusCandidate();
                this.loading = false;
                this.showMessage('success', 'Cập nhập trạng thái thành công.');
              }
              else
                this.loading = false;
            }
            else
              this.loading = false;
          }
          this.listCandidatePV = []
        });
      }
    }

    // Chuyển thành NV
    let item5: MenuItem = {
      id: '5', label: 'Chuyển thành NV', command: async () => {
        this.convertToEmployee(rowData);
      }
    }

    // Xóa
    let item6: MenuItem = {
      id: '6', label: 'Xóa', command: async () => {
        this.deleteCandidate(rowData);
      }
    }

    // Đạt PV
    let item7: MenuItem = {
      id: '7', label: 'Đạt phỏng vấn', command: async () => {
        this.updateCandidate(rowData, 3);
      }
    }

    // Trạng thái của từng dòng
    switch (rowData.statusId) {
      case 1:// Mới
        this.actions.push(item1);// hẹn PV
        this.actions.push(item5);// Chuyển NV
        this.actions.push(item6);// Xóa
        break;
      case 2:  // Hẹn phỏng vấn
        this.actions.push(item1);// hẹn PV
        this.actions.push(item7);// Đạt PV
        this.actions.push(item4);// Không Đạt
        this.actions.push(item2);// Gửi offer
        break;

      case 3: // Đạt phỏng vấn
        this.actions.push(item1);//hẹn PV
        this.actions.push(item2);// Gửi offer
        this.actions.push(item4);// Không Đạt
        this.actions.push(item5);// Chuyển NV
        break;

      case 4:// Gửi offer
        this.actions.push(item1);//hẹn PV
        this.actions.push(item2);// Gửi offer
        this.actions.push(item3);// Từ chối Offer
        this.actions.push(item5);// Chuyển NV
        break;

      case 5: // Từ chối offer
        this.actions.push(item1);//hẹn PV
        this.actions.push(item2);// Gửi offer
        this.actions.push(item5);// Chuyển NV
        break;
      case 6: // Khoogn đạt
        this.actions.push(item2);// Gửi offer
        this.actions.push(item7);// Đạt PV
        break;
    }
  }

  // delete ứng viên 1 hoặc nhiều
  async deleteCandidate(rowData: any) {
    this.confirmationService.confirm({
      message: 'Bạn có muốn xóa ứng viên?',
      accept: async () => {
        this.loading = true;
        this.employeeService.deleteCandidate(rowData.candidateId, rowData.vacanciesId).subscribe(response => {
          let result = <any>response;
          this.loading = false;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.showMessage('success', 'Xóa ứng viên thành công');
            setTimeout(() => {
              this.searchCandidate();
            }, 500)
            this.awaitResult = false;
          }
          else {
            this.showMessage('error', result.messageCode);
            this.awaitResult = false;
          };
        });
      }
    });
  }

  // Tạo lịch PV
  addInterviewSchedule(rowData: any) {
    this.listCandidatePV = [];

    let interview = new InterviewSheduleModel();
    this.interViewForm.reset();
    // Tiêu đề phỏng vấn
    let title = 'Phỏng vấn ' + rowData.vacanciesName;
    this.titleFormControl.setValue(title);

    this.displayAddInterviewSche = true;
    interview.candidateId = rowData.candidateId;
    interview.fullName = rowData ?.fullName;
    interview.email = rowData ?.email;
    interview.interviewScheduleType = 1; // 1 Trực tiếp -- 2 Online
    this.listCandidatePV.push(interview);
  }

  getInfomationContentEmail(emailType: string = null) {
    let emailData = new emailPVModel();
    let candi = new candidateSenEmailPVModel();
    this.listCandidatePV.forEach(item => {
      if (item.email !== null) {
        candi = new candidateSenEmailPVModel()
        candi.email = item.email;
        candi.fullName = item.fullName;
        candi.interviewTime = item.interviewDate;
        candi.addressOrLink = item.address;
        candi.interviewScheduleType = item.interviewScheduleType;
        if (emailType == "PHONG_VAN")
          item.listEmployeeId.forEach(emp => {
            candi.listInterviewerEmail.push(emp.email);
          });
        emailData.listCandidate.push(candi)
      }
    });

    // Vị trí tuyên dụng
    emailData.vancaciesName = this.selectedCandidate.recruitmentCampaignName;
    emailData.personInChagerName = this.selectedCandidate.personInChargeName;
    emailData.personInChagerPhone = this.selectedCandidate.personInChargePhone;
    emailData.workplace = this.selectedCandidate.placeOfWork;
    this.listCandidatePV = []
    return emailData;
  }

  // TẠO LỊCH PV SAU GỬI EMAIL CHO 1 HOẶC NHIỀU NGƯỜI
  async saveAddIntervivewSheDialog() {
    if (!this.interViewForm.valid) {
      Object.keys(this.interViewForm.controls).forEach(key => {
        if (this.interViewForm.controls[key].valid == false) {
          this.interViewForm.controls[key].markAsTouched();
        }
      });
      this.showMessage('error', 'Vui lòng nhập đầy đủ thông tin các trường dữ liệu.');
    }
    else {
      let lstInterviewModel: Array<InterviewSheduleModel> = this.mappingInterviewSheduleFormToModel();

      // let emailData = new emailPVModel();
      // emailData = this.getInfomationContentEmail("PHONG_VAN");
      // // show popup gửi email
      // let ref = this.dialogService.open(TemplateVacanciesEmailComponent, {
      //   data: {
      //     emailData: emailData,
      //     emailType: 'PHONG_VAN'
      //   },
      //   header: 'Gửi email mời tham dự phỏng vấn',
      //   width: '65%',
      //   baseZIndex: 1030,
      //   contentStyle: {
      //     "min-height": "190px",
      //     "max-height": "800px",
      //     "overflow": "auto"
      //   }
      // });
      // ref.onClose.subscribe(async (result: any) => {
      //   if (result) {
      //     if (result.status) {
      this.loading = true;
      // Tạo lịch PV
      let result: any = await this.employeeService.createInterviewSchedule(lstInterviewModel, "UNGVIEN");

      if (result.statusCode == 200) {
        this.loading = false;
        this.displayAddInterviewSche = false;
        this.listCandidate = []
        //this.listCandidate = result.listCandidate;
        this.searchCandidate();
        this.setColorOfStatusCandidate();
        this.showMessage('success', 'Tạo lịch phỏng vấn thành công.');
      }

      this.month = ((new Date).getMonth() + 1).toString();
      this.year = (new Date).getFullYear();

      let listInvalidEmail: Array<string> = result.listInvalidEmail;
      if (listInvalidEmail != null && listInvalidEmail != undefined) {
        let message = `Gửi email thành công. Có <strong>${listInvalidEmail.length} email</strong> không hợp lệ:<br/>`
        listInvalidEmail.forEach(item => {
          message += `<div style="padding-left: 30px;"> -<strong>${item}</strong></div>`
        });
        if (listInvalidEmail.length > 0) {
          this.confirmationService.confirm({
            message: message,
            rejectVisible: false,
          });
        }
      }
      this.showMessage('success', 'Gửi email thành công');
      //     }
      //   }
      // });
    }
  }

  mappingInterviewSheduleFormToModel() {

    let lstInterviewModel = new Array<InterviewSheduleModel>();
    this.listCandidatePV.forEach(item => {
      let interviewModel = new InterviewSheduleModel();
      interviewModel.interviewScheduleId = this.emptyGuid;
      // đang test
      interviewModel.vacanciesId = this.emptyGuid;
      interviewModel.candidateId = item.candidateId;

      interviewModel.interviewTitle = this.interViewForm.get('titleFormControl').value == null ? null : this.interViewForm.get('titleFormControl').value;
      interviewModel.interviewDate = item.interviewDate;
      interviewModel.address = item.address;
      interviewModel.interviewScheduleType = item.interviewScheduleType;

      item.listEmployeeId.forEach(emp => {
        interviewModel.listEmployeeId.push(emp.employeeId);
      });
      lstInterviewModel.push(interviewModel);
    });
    return lstInterviewModel;
  }

  cancelAddIntervivewSheDialog() {
    this.confirmationService.confirm({
      message: 'Bạn có muốn thoát không?',
      accept: async () => {
        this.displayAddInterviewSche = false;
        this.searchCandidate = null;
      }
    });
  }

  // CHUYỂN THÀNH NV / THỬ VIỆC
  convertToEmployee(rowData: any) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn chuyển ứng viên thành nhân viên không?',
      accept: () => {

        // Split tên Ứng viên
        let tenKhongDau = cleanAccents(rowData.fullName);
        let splitTenKD = tenKhongDau.split(" ");
        let name = '';
        let ho = '';
        let tenDem = '';
        let chuCaiDau = '';
        let chuCaiSau = '';

        if (splitTenKD.length == 1)
          name = splitTenKD[0];
        else {
          name = splitTenKD[splitTenKD.length - 1];
          ho = splitTenKD[0];
          chuCaiDau = ho.charAt(0);
          if (splitTenKD.length > 2) {
            tenDem = splitTenKD[1];
            chuCaiSau = tenDem.charAt(0);
          }
        }

        let accountName = name + chuCaiDau + chuCaiSau;
        let lstCandidateId: Array<string> = [];
        lstCandidateId.push(rowData.candidateId);

        // Lấy giá trị cho employee model
        this.employeeModel.HoTenTiengAnh = rowData.fullName.trim();
        this.employeeModel.CreatedDate = new Date();
        this.employeeModel.QuocTich = '';
        this.employeeModel.DanToc = '';
        this.employeeModel.TonGiao = '';
        this.employeeModel.StartDateMayChamCong = new Date();
        this.employeeModel.PositionId = this.emptyGuid;
        let isAccessable = true;

        // Lấy giá trị cho contact employee
        this.contactModel.ContactId = this.emptyGuid;
        this.contactModel.ObjectId = this.emptyGuid;

        this.contactModel.CreatedDate = new Date();
        this.contactModel.CreatedById = this.auth.UserId;
        this.contactModel.FirstName = rowData.fullName.trim()
        this.contactModel.LastName = '';
        this.contactModel.Email = rowData.email;
        this.contactModel.Phone = rowData.phone ?.value;
        this.contactModel.Gender = rowData.sex;
        this.contactModel.CreatedById = this.auth.UserId;
        this.contactModel.CreatedDate = new Date();

        // Accoutn đăng nhập
        this.userModel.UserName = accountName;

        //List Phòng ban
        let listPhongBanId: Array<any> = [];

        this.loading = true;
        this.awaitResult = true;

        let fileBase64 = {
          "Extension": null, /*Định dạng ảnh (jpg, png,...)*/
          "Base64": null /*Định dạng base64 của ảnh*/
        }
        this.employeeService.createEmployee(this.employeeModel, this.contactModel, this.userModel, isAccessable, listPhongBanId, true, fileBase64, rowData.candidateId).subscribe(response => {
          let result = <any>response;
          this.loading = false;
          if (result.statusCode === 202 || result.statusCode === 200) {
            // remove ứng viên khỏi danh sách
            this.listCandidate = this.listCandidate.filter(e => e != rowData);
            this.showMessage('success', 'Chuyển đổi NV thành công.');
          }
          else {
            this.showMessage('error', result.message);
            this.awaitResult = false;
          };
        });

        this.displayRejectEmail = true;
      }
    });
  }

  // UPDATE TRẠNG THÁI
  async updateCandidate(rowData: any, status: number) {
    let mes = '';
    switch (status) {
      case 3: //Đạt
        mes = "Xác nhận PV đạt cho ứng viên?";
        break;
      case 6: // Không đạt
        mes = "Xác nhận PV không đạt ứng viên?";
        break;
      case 5:// Từ chối OFFER
        mes = "Xác nhận Từ chối offer ứng viên?";
        break;
    }

    this.confirmationService.confirm({
      message: mes,
      accept: async () => {
        let lstCandidateId: Array<string> = []
        lstCandidateId.push(rowData.candidateId);
        let result: any = await this.employeeService.updateStatusCandidateFromVacancies(lstCandidateId, status, rowData.vacanciesId);
        if (result.statusCode == 200) {
          //          this.listCandidate = result.listCandidate;
          this.searchCandidate();
          this.setColorOfStatusCandidate();
          this.showMessage('success', 'Cập nhập trạng thái thành công.');
        }
      }
    });
  }

  exportExcel() {
    if (this.selection.length > 0) {
      if (this.selectedColumns.length > 0) {
        let title = `Danh sách ứng viên`;

        let workBook = new Workbook();
        let worksheet = workBook.addWorksheet(title);

        let dataHeaderMain = "Danh sách ứng viên".toUpperCase();
        let headerMain = worksheet.addRow([dataHeaderMain]);
        headerMain.font = { size: 18, bold: true };
        worksheet.mergeCells(`A${1}:I${1}`);
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
        headerRow.height = 22.50;

        /* Data table */
        let data: Array<any> = [];
        this.selection.forEach((item, index) => {
          let row: Array<any> = [];
          buildColumnExcel.forEach((_item, _index) => {

            if (_item.code == 'index') {
              row[_index] = index + 1;
            }
            else if (_item.code == 'fullName') {
              row[_index] = item.fullName;
            }
            else if (_item.code == 'recruitmentCampaignName') {
              row[_index] = item.recruitmentCampaignName;
            }
            else if (_item.code == 'vacanciesName') {
              row[_index] = item.vacanciesName;
            }
            else if (_item.code == 'applicationDate') {
              row[_index] = this.datePipe.transform(item.applicationDate, 'dd/MM/yyyy');
            }
            else if (_item.code == 'email') {
              row[_index] = item.email;
            }
            else if (_item.code == 'phone') {
              row[_index] = item.phone;
            }
            else if (_item.code == 'recruitmentChannelName') {
              row[_index] = item.recruitmentChannelName;
            }
            else if (_item.code == 'statusName') {
              row[_index] = item.statusName;
            }
          });

          data.push(row);
        });

        data.forEach((el, index, array) => {
          let row = worksheet.addRow(el);
          row.font = { name: 'Times New Roman', size: 11 };

          buildColumnExcel.forEach((_item, _index) => {
            row.getCell(_index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
            if (_item.code == 'fullName' || _item.code == 'recruitmentCampaignName' || _item.code == 'vacanciesName' || _item.code == 'email' || _item.code == 'recruitmentChannelName') {
              row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'left' };
            }
            else if (_item.code == 'applicationDate' || _item.code == 'phone') {
              row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'right' };
            }
            else {
              row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'center' };
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
        this.showMessage('warn', 'Bạn phải chọn ít nhất 1 cột');
      }
    }
    else {
      this.showMessage('warn', 'Bạn chưa chọn ứng viên');
    }
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  buildColumnExcel(): Array<ColumnExcel> {
    let listColumn: Array<ColumnExcel> = [];
    let cols = this.selectedColumns.filter(x => x.field != 'action');
    cols.forEach(item => {
      let column = new ColumnExcel();
      column.code = item.field;
      column.name = item.header;

      if (item.field == 'index') {
        column.width = 7.14;
      }
      else if (item.field == 'fullName') {
        column.width = 36.71;
      }
      else if (item.field == 'recruitmentCampaignName') {
        column.width = 40.71;
      }
      else if (item.field == 'vacanciesName') {
        column.width = 30.71;
      }
      else if (item.field == 'applicationDate') {
        column.width = 20.71;
      }
      else if (item.field == 'email') {
        column.width = 35.71;
      }
      else if (item.field == 'phone') {
        column.width = 17.71;
      }
      else if (item.field == 'recruitmentChannelName') {
        column.width = 25.71;
      }
      else if (item.field == 'statusName') {
        column.width = 20.71;
      }

      listColumn.push(column);
    });

    return listColumn;
  }


  goToCreate() {
    this.router.navigate(['/employee/tao-ung-vien']);
  }

  onViewDetail(rowData) {
    this.router.navigate(['/employee/chi-tiet-ung-vien', { candidateId: rowData.candidateId }]);
  }

  deleteRow(rowData) {
    this.confirmationService.confirm({
      message: 'Bạn có muốn xác nhận xóa?',
      accept: () => {
        this.loading = true;
        this.employeeService.deleteRecruitmentCampaign(rowData.recruitmentCampaignId).subscribe(response => {
          let result = <any>response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.listRecruitmentCampaign = this.listRecruitmentCampaign.filter(x => x.recruitmentCampaignId != rowData.recruitmentCampaignId);
            this.showMessage('success', 'Xóa chiến dịch thành công.');
          } else {
            this.showMessage('error', result.messageCode);
          }
        });
      }
    });
  }

  searchCandidate() {

    let selectedRecruitmentCampaignId = [];
    if (this.selectedRecruitmentCampaign.length > 0) {
      selectedRecruitmentCampaignId = this.selectedRecruitmentCampaign.map(x => x.recruitmentCampaignId);
    }

    let selectedVacanciesId = [];
    if (this.selectedVacancies.length > 0) {
      selectedVacanciesId = this.selectedVacancies.map(x => x.vacanciesId);
    }

    let ApplicationDateFromUTC = null;
    if (this.ApplicationDateFrom) {
      ApplicationDateFromUTC = this.ApplicationDateFrom;
      ApplicationDateFromUTC.setHours(0, 0, 0, 0);
      ApplicationDateFromUTC = convertToUTCTime(ApplicationDateFromUTC);
    }

    let ApplicationDateToUTC = null;
    if (this.ApplicationDateTo) {
      ApplicationDateToUTC = this.ApplicationDateTo;
      ApplicationDateToUTC.setHours(0, 0, 0, 0);
      ApplicationDateToUTC = convertToUTCTime(ApplicationDateToUTC);
    }

    let selectdeRecruitmentChannelId = [];
    if (this.selectdeRecruitmentChannel.length > 0) {
      selectdeRecruitmentChannelId = this.selectdeRecruitmentChannel.map(x => x.categoryId);
    }

    let selectedStatusId = [];
    if (this.selectedStatus.length > 0) {
      selectedStatusId = this.selectedStatus.map(x => x.value);
    }

    this.loading = true;
    this.employeeService.SearchCandidate(this.fullName, selectedRecruitmentCampaignId, selectedVacanciesId,
      ApplicationDateFromUTC, ApplicationDateToUTC, this.email, this.phone, selectdeRecruitmentChannelId, selectedStatusId).subscribe(response => {
        let result = <any>response;
        this.loading = false;
        if (result.statusCode == 200) {

          this.listCandidate = result.listCandidate;
          this.listEmployeeInterview = result.listAllEmployee;
          this.setColorOfStatusCandidate();
          if (this.listRecruitmentCampaign.length === 0) {
            this.showMessage('warn', 'Không tìm thấy nhân viên nào!');
          }
        } else {
          this.showMessage('error', result.messageCode);
        }
      });
  };

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

  updateStatus(candidateId: string, status: number) {
    this.loading = true;
    this.awaitResult = true;
    this.employeeService.updateCandidateStatus(candidateId, status).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.showMessage('success', 'Cập nhật trạng thái ứng viên thành công');
        setTimeout(() => {
          this.searchCandidate();
        }, 1000)
        this.awaitResult = false;
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage('error', result.messageCode);
        this.awaitResult = false;
      };
    });
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function ParseStringToFloat(str: string) {
  if (str === "") return null;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
const cleanAccents = (str: string): string => {
  str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
  str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
  str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
  str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
  str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
  str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
  str = str.replace(/đ/g, "d");
  str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
  str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
  str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
  str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
  str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
  str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
  str = str.replace(/Đ/g, "D");
  // Combining Diacritical Marks
  str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // huyền, sắc, hỏi, ngã, nặng
  str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // mũ â (ê), mũ ă, mũ ơ (ư)

  return str;
}
