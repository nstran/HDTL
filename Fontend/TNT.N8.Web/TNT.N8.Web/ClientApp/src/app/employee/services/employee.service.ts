import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DeleteEmployeeResult, EmployeeModel, TakeListEvaluateResult } from "../models/employee.model";
import { CreateEmployeeModel } from "../models/employee.model";
import { ContactModel } from "../../shared/models/contact.model";
import { UserModel } from '../../shared/models/user.model';
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';
import { CandidateAssessment } from '../models/candidate-assessment.model';
import { HopDongNhanSuModel } from './../models/hop-dong-nhan-su.model';
import { Observable } from 'rxjs/Observable';
import { GetBenefitTypeModel} from '../models/employee-type.model';

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


@Injectable()
export class EmployeeService {

  constructor(private httpClient: HttpClient) { }

  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;

  getBenefitType() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getBenefitType';
    return this.httpClient.get(url, {}).pipe(
      map((response: GetBenefitTypeModel) => {
        return <GetBenefitTypeModel>response;
      }));
  }
  
  createEmployee(emp: CreateEmployeeModel, contact: ContactModel, user: UserModel, isAccessable: Boolean, listPhongBanId: Array<string>, isAuto: Boolean = false, fileBase64: any = null, candidateId: string = null) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/create";
    return this.httpClient.post(url, {
      Employee: emp,
      Contact: contact,
      User: user,
      IsAccessable: isAccessable,
      ListPhongBanId: listPhongBanId,
      IsAuto: isAuto,
      FileBase64: fileBase64,
      CandidateId: candidateId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  createCusEmployee(emp: CreateEmployeeModel, contact: ContactModel, user: UserModel, listPhongBanId: Array<string>, isAuto: Boolean = false) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/create";
    return this.httpClient.post(url, {
      Employee: emp,
      Contact: contact,
      User: user,
      ListPhongBanId: listPhongBanId,
      IsAuto: isAuto,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  searchEmployee(firstName: string, lastName: string, userName: string, identityId: string, position: Array<any>, organizationId: string) {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/search';
    return this.httpClient.post(url, {
      FirstName: firstName,
      LastName: lastName,
      UserName: userName,
      IdentityId: identityId,
      ListPosition: position,
      OrganizationId: organizationId,
      UserId: JSON.parse(currentUser).UserId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllEmployee() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllEmployee";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllEmployeeAsync() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllEmployee";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getEmployeeById(employeeId: string, contactId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getEmployeeById";
    return this.httpClient.post(url, { EmployeeId: employeeId, ContactId: contactId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getEmployeeByIdAsync(employeeId: string, contactId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getEmployeeById";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { EmployeeId: employeeId, ContactId: contactId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  editEmployeeById(employee: EmployeeModel, contact: ContactModel, user: UserModel, isReset: boolean) {

    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/editEmployeeById";
    return this.httpClient.post(url, { Employee: employee, Contact: contact, User: user, IsResetPass: isReset }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllEmpAccount() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllEmpAccount";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  //getAllEmployeeAccount() {
  //  let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllEmployeeAccount";
  //  return new Promise((resolve, reject) => { return this.httpClient.post(url, {}).toPromise()
  //    .then((response: Response) => {
  //      resolve(response);
  //    });
  //  });
  //}
  getAllEmployeeAccount() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllEmployeeAccount";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
    //return new Promise((resolve, reject) => {
    //  return this.httpClient.post(url, {}).toPromise()
    //    .then((response: Response) => {
    //      resolve(response);
    //    });
    //});
  }

  getAllEmpIdentity(currentEmpId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllEmpIdentity";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { CurrentEmpId: currentEmpId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getAllEmpAccIdentity(employeeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllEmpAccIdentity";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { EmployeeId: employeeId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  sendEmail(email: string, fullName: string, userName: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/email/sendEmail";
    return this.httpClient.post(url, { EmailAddress: email, FullName: fullName, UserName: userName }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  editEmployeeDataPermission(empId: string, isManager: boolean) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/editEmployeeDataPermission";
    return this.httpClient.post(url, { EmployeeId: empId, IsManager: isManager }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  employeePermissionMapping(perSetId: string, empId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/employeePermissionMapping';
    return this.httpClient.post(url, { PermissionSetId: perSetId, EmployeeId: empId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getEmployeeByPositionCode(positionCode: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/getEmployeeByPositionCode';
    return this.httpClient.post(url, { PositionCode: positionCode }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getStatisticForEmpDashBoard(firstOfWeek, lastOfWeek, keyname, userId) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/getStatisticForEmpDashBoard';
    return this.httpClient.post(url, {
      FirstOfWeek: firstOfWeek, LastOfWeek: lastOfWeek, KeyName: keyname, UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getEmployeeToApprove(empId: string, moduleCode: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/getEmployeeApprove';
    return this.httpClient.post(url, { EmployeeId: empId, ModuleCode: moduleCode }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  //Lấy danh sách nhân viên chăm sóc khách hàng theo quyền của người đang đăng nhập
  getEmployeeCareStaff(isManager: boolean, employeeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getEmployeeCareStaff";
    return this.httpClient.post(url, { IsManager: isManager, EmployeeId: employeeId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getEmployeeCareStaffAsyc(isManager: boolean, employeeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getEmployeeCareStaff";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { IsManager: isManager, EmployeeId: employeeId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  fileType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
  fileExtension = '.xlsx';

  public exportExcel(jsonData: any[], fileName: string): void {

    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(jsonData);
    const wb: XLSX.WorkBook = { Sheets: { 'Danh sách nhân viên': ws }, SheetNames: ['Danh sách nhân viên'] };
    const excelBuffer: any = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
    this.saveExcelFile(excelBuffer, fileName);
  }

  private saveExcelFile(buffer: any, fileName: string): void {
    const data: Blob = new Blob([buffer], { type: this.fileType });
    FileSaver.saveAs(data, fileName + this.fileExtension);
  }

  checkAdminLogin() {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/checkAdminLogin';
    return this.httpClient.post(url, {
      UserId: JSON.parse(currentUser).UserId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  checkAdminLoginAsync() {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/checkAdminLogin';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { UserId: JSON.parse(currentUser).UserId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });

  }

  getMasterDataEmployeeDetail() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterDataEmployeeDetail';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getThongTinCaNhanThanhVien(EmployeeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getThongTinCaNhanThanhVien';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { EmployeeId: EmployeeId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getThongTinChungThanhVien(EmployeeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getThongTinChungThanhVien';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { EmployeeId: EmployeeId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveThongTinChungThanhVien(ThongTinChungThanhVien: any, listPhongBanId: any, fileBase64: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/saveThongTinChungThanhVien';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url,
        {
          ThongTinChungThanhVien: ThongTinChungThanhVien,
          ListPhongBanId: listPhongBanId,
          FileBase64: fileBase64
        }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveThongTinCaNhanThanhVien(ThongTinCaNhanThanhVien: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/saveThongTinCaNhanThanhVien';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { ThongTinCaNhanThanhVien: ThongTinCaNhanThanhVien }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getCauHinhPhanQuyen(EmployeeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getCauHinhPhanQuyen';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { EmployeeId: EmployeeId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveCauHinhPhanQuyen(EmployeeId: string, IsManager: boolean, RoleId: string, ListThanhVienPhongBan: Array<any>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/saveCauHinhPhanQuyen';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmployeeId: EmployeeId,
        IsManager: IsManager,
        RoleId: RoleId,
        ListThanhVienPhongBan: ListThanhVienPhongBan
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getThongTinNhanSu(EmployeeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getThongTinNhanSu';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmployeeId: EmployeeId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveThongTinNhanSu(ThongTinNhanSu: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/saveThongTinNhanSu';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ThongTinNhanSu: ThongTinNhanSu
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getThongTinLuongVaTroCap(EmployeeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getThongTinLuongVaTroCap';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmployeeId: EmployeeId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveThongTinLuongVaTroCap(ThongTinLuongVaTroCap: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/saveThongTinLuongVaTroCap';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ThongTinLuongVaTroCap: ThongTinLuongVaTroCap
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateThongTinGiaDinh(ThongTinGiaDinh: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/CreateOrUpdateThongTinGiaDinh';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ThongTinGiaDinh: ThongTinGiaDinh
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getThongTinGiaDinh(EmployeeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/GetThongTinGiaDinh';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmployeeId: EmployeeId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveThongTinKhac(ThongTinKhac: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/SaveThongTinKhac';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ThongTinKhac: ThongTinKhac
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getThongTinKhac(EmployeeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/GetThongTinKhac';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmployeeId: EmployeeId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getThongTinGhiChu(EmployeeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getThongTinGhiChu';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmployeeId: EmployeeId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  resetPassword(EmployeeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/resetPassword';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmployeeId: EmployeeId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataDashboard(timeType: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getMasterDataDashboard";
    return this.httpClient.post(url, { TimeType: timeType }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createRecruitmentCampaign(recruitmentCampaignModel: any, folderType: string, listFile: Array<FileUploadModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/createRecruitmentCampaign';

    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("UserId", this.userId);

    formData.append("RecruitmentCampaignModel.RecruitmentCampaignId", recruitmentCampaignModel.recruitmentCampaignId);
    formData.append("RecruitmentCampaignModel.RecruitmentCampaignName", recruitmentCampaignModel.recruitmentCampaignName);
    formData.append("RecruitmentCampaignModel.StartDate", recruitmentCampaignModel.startDate == null ? null : new Date(recruitmentCampaignModel.startDate).toUTCString());
    formData.append("RecruitmentCampaignModel.EndDateDate", recruitmentCampaignModel.endDateDate == null ? null : new Date(recruitmentCampaignModel.endDateDate).toUTCString());
    formData.append("RecruitmentCampaignModel.PersonInChargeId", recruitmentCampaignModel.personInChargeId);
    formData.append("RecruitmentCampaignModel.RecruitmentCampaignDes", recruitmentCampaignModel.recruitmentCampaignDes);
    formData.append("RecruitmentCampaignModel.CreatedById", recruitmentCampaignModel.createdById);
    formData.append("RecruitmentCampaignModel.CreatedDate", recruitmentCampaignModel.createdDate == null ? null : new Date(recruitmentCampaignModel.createdDate).toUTCString());
    formData.append("RecruitmentCampaignModel.UpdatedById", recruitmentCampaignModel.updatedById);
    formData.append("RecruitmentCampaignModel.UpdatedDate", recruitmentCampaignModel.updatedDate == null ? null : new Date(recruitmentCampaignModel.updatedDate).toUTCString());

    var index = 0;
    for (var pair of listFile) {
      formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
      index++;
    }

    return this.httpClient.post(url,
      formData
    ).pipe(map((response: Response) => {
      return response;
    }));
  }

  getMasterSearchRecruitmentCampaignAsync() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterSearchRecruitmentCampaign';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteRecruitmentCampaign(recruitmentCampaignId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/deleteRecruitmentCampaign';
    return this.httpClient.post(url, {
      RecruitmentCampaignId: recruitmentCampaignId,
    }).pipe(
      map((response: Response) => {
        return response;
      })
    );
  }

  searchRecruitmentCampaign(recruitmentCampaignName: string, startDateFrom: Date, startDateTo: Date, endDateFrom: Date, endDateTo: Date, listPersonInchangeId: Array<string>, recruitmentQuantityFrom: number, recruitmentQuantityTo: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/searchRecruitmentCampaign';
    return this.httpClient.post(url, {
      RecruitmentCampaignName: recruitmentCampaignName,
      StartDateFrom: startDateFrom,
      StartDateTo: startDateTo,
      EndDateFrom: endDateFrom,
      EndDateTo: endDateTo,
      ListPersonInchangeId: listPersonInchangeId,
      RecruitmentQuantityFrom: recruitmentQuantityFrom,
      RecruitmentQuantityTo: recruitmentQuantityTo,
    }).pipe(
      map((response: Response) => {
        return response;
      })
    );
  }

  getMasterDataRecruitmentCampaignInformation(recruitmentCampaignId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterDataRecruitmentCampaignInformation';
    return this.httpClient.post(url, {
      RecruitmentCampaignId: recruitmentCampaignId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  //LẤY MASTER DATA ĐỂ CHECK IMPORT DATA: UNG VIEN
  getCandidateImportDetai() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getCandidateImportDetai";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  importListCandidate(listCandidate: any, vacanciesId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/importListCandidate";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListCandidate: listCandidate,
        VacanciesId: vacanciesId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  getMasterDataBaoCaoTuyenDung() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterDataBaoCaoTuyenDung';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getBaoCaoBaoCaoTuyenDung(thang: number, nam: number, listRecumId: Array<string>, listVacanciesId: Array<string>, listEmployeeId: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getBaoCaoBaoCaoTuyenDung';

    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Thang: thang,
        Nam: nam,
        ListRecruitId: listRecumId,
        ListEmployeeId: listEmployeeId,
        ListVacanciesId: listVacanciesId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  getMasterDataVacanciesCreate() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getMasterDataVacanciesCreate";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createVacancies(viTriTD: any, folderType: string, listFile: Array<any>) {

    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/createVacancies';

    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("UserId", this.userId);

    formData.append("ViTriTuyenDung.VacanciesId", viTriTD.vacanciesId);
    formData.append("ViTriTuyenDung.RecruitmentCampaignId", viTriTD.recruitmentCampaignId);
    formData.append("ViTriTuyenDung.VacanciesName", viTriTD.vacanciesName);
    formData.append("ViTriTuyenDung.Quantity", viTriTD.quantity);
    formData.append("ViTriTuyenDung.Priority", viTriTD.priority.toString());
    formData.append("ViTriTuyenDung.PersonInChargeId", viTriTD.personInChargeId);
    formData.append("ViTriTuyenDung.TypeOfWork", viTriTD.typeOfWork);
    formData.append("ViTriTuyenDung.PlaceOfWork", viTriTD.placeOfWork == null ? '' : viTriTD.placeOfWork);
    formData.append("ViTriTuyenDung.ExperienceId", viTriTD.experienceId);
    formData.append("ViTriTuyenDung.Currency", viTriTD.currency.toString());
    formData.append("ViTriTuyenDung.SalarType", viTriTD.salarType == null ? null : viTriTD.salarType.toString());
    formData.append("ViTriTuyenDung.SalaryFrom", viTriTD.salaryFrom.toString());
    formData.append("ViTriTuyenDung.SalaryTo", viTriTD.salaryTo.toString());
    formData.append("ViTriTuyenDung.VacanciesDes", viTriTD.vacanciesDes == null ? '' : viTriTD.vacanciesDes);
    formData.append("ViTriTuyenDung.ProfessionalRequirements", viTriTD.professionalRequirements == null ? '' : viTriTD.professionalRequirements);
    formData.append("ViTriTuyenDung.CandidateBenefits", viTriTD.candidateBenefits == null ? '' : viTriTD.candidateBenefits);

    formData.append("ViTriTuyenDung.CreatedById", viTriTD.createdById);
    formData.append("ViTriTuyenDung.CreatedDate", viTriTD.createdDate == null ? null : new Date(viTriTD.createdDate).toString());
    formData.append("ViTriTuyenDung.UpdatedById", viTriTD.updatedById);
    formData.append("ViTriTuyenDung.UpdatedDate", viTriTD.updatedDate == null ? null : new Date(viTriTD.updatedDate).toString());

    var index = 0;

    for (var pair of listFile) {
      formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
      index++;
    }

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataVacancies(vacanciesId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getMasterDataVacanciesDetail";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        VacanciesId: vacanciesId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateVacancies(viTriTD: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/updateVacancies';
    return this.httpClient.post(url, { ViTriTuyenDung: viTriTD }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteFile(fileInFolderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/contract/deleteFile';
    return this.httpClient.post(url, {
      FileInFolderId: fileInFolderId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  uploadFile(folderType: string, listFile: Array<FileUploadModel>, objectId: string, objectNumber: number = 0) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/uploadFile';

    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("ObjectId", objectId);
    formData.append("ObjectNumber", objectNumber.toString());
    formData.append("UserId", this.userId);
    var index = 0;
    for (var pair of listFile) {
      formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
      index++;
    }
    return this.httpClient.post(url,
      formData
    ).pipe(map((response: Response) => {
      return response;
    }));
  }

  updateStatusCandidateFromVacancies(lstCandidateId: Array<string>, status: number, vacanciesId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/updateStatusCandidateFromVacancies';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListCandidate: lstCandidateId,
        Status: status,
        VacanciesId: vacanciesId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createInterviewSchedule(listInterviewSchedule: Array<any>, screenType: string = null) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/createInterviewSchedule";

    let formData: FormData = new FormData();
    formData.append("UserId", this.userId);
    formData.append("ScreenType", screenType);
    var index = 0;
    for (var interView of listInterviewSchedule) {

      formData.append("ListInterviewSchedule[" + index + "].InterviewScheduleId", interView.interviewScheduleId);
      formData.append("ListInterviewSchedule[" + index + "].VacanciesId", interView.vacanciesId);
      formData.append("ListInterviewSchedule[" + index + "].CandidateId", interView.candidateId);
      formData.append("ListInterviewSchedule[" + index + "].InterviewTitle", interView.interviewTitle);
      formData.append("ListInterviewSchedule[" + index + "].InterviewDate", interView.interviewDate == null ? null : interView.interviewDate.toUTCString());
      formData.append("ListInterviewSchedule[" + index + "].Address", interView.address);
      formData.append("ListInterviewSchedule[" + index + "].InterviewScheduleType", interView.interviewScheduleType);
      formData.append("ListInterviewSchedule[" + index + "].TypeOfInterview", interView.typeOfInterview);
      var indexEmp = 0;
      for (var empId of interView.listEmployeeId) {
        formData.append("ListInterviewSchedule[" + index + "].ListEmployeeId[" + indexEmp + "]", empId);
        indexEmp++;
      }
      index++;
    }

    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateCandidate(candidateModel: any, vacanciesId: string, folderType: string, listFile: Array<FileUploadModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/updateCandidate';

    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("VacanciesId", vacanciesId);
    formData.append("UserId", this.userId);

    formData.append("CandidateModel.CandidateId", candidateModel.candidateId);
    formData.append("CandidateModel.FullName", candidateModel.fullName);
    formData.append("CandidateModel.DateOfBirth", candidateModel.dateOfBirth == null ? null : new Date(candidateModel.dateOfBirth).toUTCString());
    formData.append("CandidateModel.Phone", candidateModel.phone);
    formData.append("CandidateModel.Email", candidateModel.email);
    formData.append("CandidateModel.Address", candidateModel.address);
    formData.append("CandidateModel.RecruitmentChannel", candidateModel.recruitmentChannel);
    formData.append("CandidateModel.Sex", candidateModel.sex == null ? null : candidateModel.sex.toString());
    formData.append("CandidateModel.StatusId", candidateModel.statusId == null ? null : candidateModel.statusId.toString());
    formData.append("CandidateModel.ApplicationDate", candidateModel.applicationDate == null ? null : new Date(candidateModel.applicationDate).toUTCString());

    formData.append("CandidateModel.CreatedById", candidateModel.createdById);
    formData.append("CandidateModel.CreatedDate", candidateModel.createdDate == null ? null : new Date(candidateModel.createdDate).toUTCString());
    formData.append("CandidateModel.UpdatedById", candidateModel.updatedById);
    formData.append("CandidateModel.UpdatedDate", candidateModel.updatedDate == null ? null : new Date(candidateModel.updatedDate).toUTCString());

    formData.append("CandidateModel.PhuongThucTuyenDungId", candidateModel.phuongThucTuyenDungId);
    formData.append("CandidateModel.MucPhi", candidateModel.mucPhi);
    formData.append("CandidateModel.TomTatHocVan", candidateModel.tomTatHocVan);

    var index = 0;
    for (var pair of listFile) {
      formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
      index++;
    }

    // index = 0;
    // for (var overviewCandidate of listOverviewCandidate) {
    //   formData.append("ListOverviewCandidate[" + index + "].OverviewCandidateId", overviewCandidate.overviewCandidateId);
    //   formData.append("ListOverviewCandidate[" + index + "].CandidateId", overviewCandidate.candidateId);
    //   formData.append("ListOverviewCandidate[" + index + "].EducationAndWorkExpName", overviewCandidate.educationAndWorkExpName);
    //   formData.append("ListOverviewCandidate[" + index + "].CertificatePlace", overviewCandidate.certificatePlace);
    //   formData.append("ListOverviewCandidate[" + index + "].SpecializedTraining", overviewCandidate.specializedTraining);
    //   formData.append("ListOverviewCandidate[" + index + "].JobDescription", overviewCandidate.jobDescription);
    //   formData.append("ListOverviewCandidate[" + index + "].StartDate", overviewCandidate.startDate == null ? null : new Date(overviewCandidate.startDate).toUTCString());
    //   formData.append("ListOverviewCandidate[" + index + "].EndDate", overviewCandidate.endDate == null ? null : new Date(overviewCandidate.endDate).toUTCString());
    //   formData.append("ListOverviewCandidate[" + index + "].Phone", overviewCandidate.phone);
    //   formData.append("ListOverviewCandidate[" + index + "].Type", overviewCandidate.type);
    //   formData.append("ListOverviewCandidate[" + index + "].ProficiencyLevel", overviewCandidate.proficiencyLevel);

    //   formData.append("ListOverviewCandidate[" + index + "].CreatedById", overviewCandidate.createdById);
    //   formData.append("ListOverviewCandidate[" + index + "].CreatedDate", overviewCandidate.createdDate == null ? null : new Date(candidateModel.createdDate).toUTCString());
    //   formData.append("ListOverviewCandidate[" + index + "].UpdatedById", overviewCandidate.updatedById);
    //   formData.append("ListOverviewCandidate[" + index + "].UpdatedDate", overviewCandidate.updatedDate == null ? null : new Date(candidateModel.updatedDate).toUTCString());
    //   index++;
    // }

    // index = 0;
    // for (var interviewSchedule of listInterviewSchedule) {
    //   formData.append("ListInterviewSchedule[" + index + "].InterviewScheduleId", interviewSchedule.interviewScheduleId);
    //   formData.append("ListInterviewSchedule[" + index + "].VacanciesId", interviewSchedule.vacanciesId);
    //   formData.append("ListInterviewSchedule[" + index + "].CandidateId", interviewSchedule.candidateId);
    //   formData.append("ListInterviewSchedule[" + index + "].InterviewTitle", interviewSchedule.interviewTitle);
    //   formData.append("ListInterviewSchedule[" + index + "].Address", interviewSchedule.Address);
    //   formData.append("ListInterviewSchedule[" + index + "].InterviewTime", interviewSchedule.interviewTime == null ? null : interviewSchedule.interviewTime.toString());
    //   formData.append("ListInterviewSchedule[" + index + "].InterviewDate", interviewSchedule.interviewDate == null ? null : new Date(interviewSchedule.interviewDate).toUTCString());
    //   formData.append("ListInterviewSchedule[" + index + "].SortOrder", interviewSchedule.sortOrder == null ? null : interviewSchedule.sortOrder.toString());
    //   formData.append("ListInterviewSchedule[" + index + "].Status", interviewSchedule.status == null ? null : interviewSchedule.status.toString());
    //   formData.append("ListInterviewSchedule[" + index + "].TypeOfInterview", interviewSchedule.typeOfInterview);

    //   formData.append("ListInterviewSchedule[" + index + "].CreatedById", interviewSchedule.createdById);
    //   formData.append("ListInterviewSchedule[" + index + "].CreatedDate", interviewSchedule.createdDate == null ? null : new Date(candidateModel.createdDate).toUTCString());
    //   formData.append("ListInterviewSchedule[" + index + "].UpdatedById", interviewSchedule.updatedById);
    //   formData.append("ListInterviewSchedule[" + index + "].UpdatedDate", interviewSchedule.updatedDate == null ? null : new Date(candidateModel.updatedDate).toUTCString());
    //   index++;
    // }

    // index = 0;
    // for (var quiz of listQuiz) {
    //   formData.append("ListQuiz[" + index + "].QuizId", quiz.quizId);
    //   formData.append("ListQuiz[" + index + "].VacanciesId", quiz.vacanciesId);
    //   formData.append("ListQuiz[" + index + "].CandidateId", quiz.candidateId);
    //   formData.append("ListQuiz[" + index + "].QuizName", quiz.quizName);
    //   formData.append("ListQuiz[" + index + "].Score", quiz.score == null ? null : quiz.score.toString());
    //   formData.append("ListQuiz[" + index + "].Status", quiz.status == null ? null : quiz.status.toString());

    //   formData.append("ListQuiz[" + index + "].CreatedById", quiz.createdById);
    //   formData.append("ListQuiz[" + index + "].CreatedDate", quiz.createdDate == null ? null : new Date(quiz.createdDate).toUTCString());
    //   formData.append("ListQuiz[" + index + "].UpdatedById", quiz.updatedById);
    //   formData.append("ListQuiz[" + index + "].UpdatedDate", quiz.updatedDate == null ? null : new Date(quiz.updatedDate).toUTCString());
    //   index++;
    // }

    return this.httpClient.post(url,
      formData
    ).pipe(map((response: Response) => {
      return response;
    }));
  }

  deleteCandidates(listCandidateId: Array<string>, vacanciesId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/deleteCandidates";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        ListCandidateId: listCandidateId,
        VacanciesId: vacanciesId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  //File mẫu ứng viên
  downloadTemplateImportCandidate() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/downloadTemplateImportCandidate';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createCandidate(candidateModel: any, vacanciesId: string, folderType: string, listFile: Array<FileUploadModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/createCandidate';

    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("VacanciesId", vacanciesId);
    formData.append("UserId", this.userId);

    formData.append("CandidateModel.CandidateId", candidateModel.candidateId);
    formData.append("CandidateModel.FullName", candidateModel.fullName);
    formData.append("CandidateModel.DateOfBirth", candidateModel.dateOfBirth == null ? null : new Date(candidateModel.dateOfBirth).toUTCString());
    formData.append("CandidateModel.Phone", candidateModel.phone);
    formData.append("CandidateModel.Email", candidateModel.email);
    formData.append("CandidateModel.Address", candidateModel.address);
    formData.append("CandidateModel.RecruitmentChannel", candidateModel.recruitmentChannel);
    formData.append("CandidateModel.Sex", candidateModel.sex == null ? null : candidateModel.sex.toString());
    formData.append("CandidateModel.StatusId", candidateModel.statusId == null ? null : candidateModel.statusId.toString());

    formData.append("CandidateModel.ApplicationDate", candidateModel.applicationDate == null ? null : new Date(candidateModel.applicationDate).toUTCString());

    formData.append("CandidateModel.CreatedById", candidateModel.createdById);
    formData.append("CandidateModel.CreatedDate", candidateModel.createdDate == null ? null : new Date(candidateModel.createdDate).toUTCString());
    formData.append("CandidateModel.UpdatedById", candidateModel.updatedById);
    formData.append("CandidateModel.UpdatedDate", candidateModel.updatedDate == null ? null : new Date(candidateModel.updatedDate).toUTCString());


    var index = 0;
    for (var pair of listFile) {
      formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
      index++;
    }

    return this.httpClient.post(url,
      formData
    ).pipe(map((response: Response) => {
      return response;
    }));
  }

  sendEmailInterview(emailData: any, file: File[], folderType: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/sendEmailInterview";

    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("UserId", this.userId);

    var index1 = 0;
    for (var candi of emailData.listCandidate) {

      formData.append("EmailInterview.ListCandidate[" + index1 + "].FullName", candi.fullName);
      formData.append("EmailInterview.ListCandidate[" + index1 + "].InterviewTime", candi.interviewTime == null ? null : candi.interviewTime.toUTCString());
      formData.append("EmailInterview.ListCandidate[" + index1 + "].AddressOrLink", candi.addressOrLink);
      formData.append("EmailInterview.ListCandidate[" + index1 + "].Email", candi.email);
      formData.append("EmailInterview.ListCandidate[" + index1 + "].InterviewScheduleType", candi.interviewScheduleType);
      var index2 = 0;
      for (var email of candi.listInterviewerEmail) {

        formData.append("EmailInterview.ListCandidate[" + index1 + "].ListInterviewerEmail[" + index2 + "]", email.email);
        index2++;
      }
      index1++;
    }

    formData.append("EmailInterview.WorkPlace", emailData.workplace);
    formData.append("EmailInterview.VancaciesName", emailData.vancaciesName);
    formData.append("EmailInterview.PersonInChagerName", emailData.personInChagerName);
    formData.append("EmailInterview.PersonInChagerPhone", emailData.personInChagerPhone);
    formData.append("EmailInterview.SendContent", emailData.sendContent);
    formData.append("EmailInterview.Subject", emailData.subject);

    // var index = 0;
    // for (var pair of attchement) {
    //   formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
    //   formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
    //   formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
    //   formData.append("ListFile[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
    //   formData.append("ListFile[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
    //   formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
    //   formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
    //   formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
    //   index++;
    // }
    for (let i = 0; i < file.length; i++) {
      formData.append("ListFormFile", file[i]);
    }
    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  sendEmailTest(nameMailSent: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/sentmail";
    let formData: FormData = new FormData();
    formData.append("NameMailSent", nameMailSent);
    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getAllVacancies() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/getAllVacancies';

    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  filterVacancies(selectedViTriId: Array<string>, selectedChienDichId: Array<string>, selectedMucDoUTId: Array<number>, selectedKinhNghiemId: Array<string>, selectedLoaiCVId: Array<string>, selectedNguoiPTId: Array<string>, startMoney: number, endMoney: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/filterVacancies';
    return this.httpClient.post(url, {
      SelectedViTriId: selectedViTriId,
      SelectedChienDichId: selectedChienDichId,
      SelectedMucDoUTId: selectedMucDoUTId,
      SelectedKinhNghiemId: selectedKinhNghiemId,
      SelectedLoaiCVId: selectedLoaiCVId,
      SelectedNguoiPTId: selectedNguoiPTId,
      StartMoney: startMoney,
      EndMoney: endMoney
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteVacanciesById(vacanciesId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/deleteVacanciesById';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        VacanciesId: vacanciesId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  getMasterCreateCandidate() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterCreateCandidate';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      })
    );
  }

  deleteCandidate(candidateId: string, vacanciesId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/deleteCandidate";
    return this.httpClient.post(url, { CandidateId: candidateId, VacanciesId: vacanciesId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createOrUpdateCandidateAssessment(candidateAssessment: CandidateAssessment, listCandiAssetDetail: Array<any>) {
    // const url = localStorage.getItem('ApiEndPoint') + '/api/employee/createOrUpdateCandidateAssessment';
    // return new Promise((resolve, rejecr) => {
    //   return this.httpClient.post(url, {
    //     CandidateAssessment: candidateAssessment,
    //     CandidateAssessmentDetail: listCandiAssetDetail
    //   }).toPromise()
    //     .then((response: Response) => {
    //       resolve(response);
    //     });
    // });

    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/createOrUpdateCandidateAssessment';
    return this.httpClient.post(url, {
      CandidateAssessment: candidateAssessment,
      CandidateAssessmentDetail: listCandiAssetDetail
    }).pipe(map((response: Response) => {
      return response;
    }));
  }


  createOrUpdateCandidateDetailInfor(overviewCandidate: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/createOrUpdateCandidateDetailInfor";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        OverviewCandidate: overviewCandidate,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateInterviewScheduleAsync(interviewSchedule: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/updateInterviewSchedule';
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        InterviewSchedule: interviewSchedule,
        // RecruitmentCampaignId: recruitmentCampaignId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateQuiz(quizModel: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/createOrUpdateQuiz';
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        QuizModel: quizModel,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteCandidateDetailInfor(overviewCandidateId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/deleteCandidateDetailInfor";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        OverviewCandidateId: overviewCandidateId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteInterviewScheduleAsync(interviewScheduleId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/deleteInterviewSchedule';
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        InterviewScheduleId: interviewScheduleId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteQuiz(quizId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/deleteQuiz';
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        QuizId: quizId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteCandidateAssessment(candidateAssessmentId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/deleteCandidateAssessment';
    // return new Promise((resolve, rejecr) => {
    //   return this.httpClient.post(url, {
    //     CandidateAssessmentId: candidateAssessmentId
    //   }).toPromise()
    //     .then((response: Response) => {
    //       resolve(response);
    //     });
    // });
    return this.httpClient.post(url, {
      CandidateAssessmentId: candidateAssessmentId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }


  updateCandidateStatus(candidateId: string, status: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/updateCandidateStatus";
    return this.httpClient.post(url, { CandidateId: candidateId, Status: status }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterCreateCandidateAsync() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterCreateCandidate';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterCandidateDetailAsync(candidateId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterCandidateDetail';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { CandidateId: candidateId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterSearchCandidate() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterSearchCandidate';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      })
    );
  }


  SearchCandidate(fullName: string, listRecruitmentCampaignId: Array<string>,
    listVacanciesId: Array<string>, applicationDateFrom: Date, applicationDateTo: Date, email: string, phone: string,
    listRecruitmentChannelId: Array<string>, listStatus: Array<number>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/searchCandidate';
    return this.httpClient.post(url, {
      FullName: fullName,
      ListRecruitmentCampaignId: listRecruitmentCampaignId,
      ListVacanciesId: listVacanciesId,
      ApplicationDateFrom: applicationDateFrom,
      ApplicationDateTo: applicationDateTo,
      Email: email,
      Phone: phone,
      ListRecruitmentChannelId: listRecruitmentChannelId,
      ListStatus: listStatus,
    }).pipe(
      map((response: Response) => {
        return response;
      })
    );
  }

  getMasterRecruitmentCampaignDetailAsync(recruitmentCampaignId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterRecruitmentCampaignDetail';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        RecruitmentCampaignId: recruitmentCampaignId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }








  getMasterCongLuongCauHinh() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterCongLuongCauHinh';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  getDetailCongLuongCauHinh() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getDetailCongLuongCauHinh';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }





  getMasterDataCreateEmployee() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/GetMasterDataCreateEmployee";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createHopDongNhanSu(hopDongNhanSu: HopDongNhanSuModel) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/createHopDongNhanSu";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        HopDongNhanSu: hopDongNhanSu,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getHopDongNhanSuById(hopDongNhanSuId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getHopDongNhanSuById";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        Id: hopDongNhanSuId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getListHopDongNhanSu(employeeId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getListHopDongNhanSu";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        EmployeeId: employeeId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateHopDongNhanSu(hopDongNhanSu: HopDongNhanSuModel) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/updateHopDongNhanSu";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        HopDongNhanSu: hopDongNhanSu,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteHopDongNhanSuById(hopDongNhanSuId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/deleteHopDongNhanSuById";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        Id: hopDongNhanSuId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getLichSuHopDongNhanSu(employeeId: any, listId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getLichSuHopDongNhanSu";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        EmployeeId: employeeId,
        ListId: listId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterCauHinhBaoHiem() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterCauHinhBaoHiem';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateCauHinhBaoHiemLoftCare(Type: number, CauHinhBaoHiemLoftCare: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/createOrUpdateCauHinhBaoHiemLoftCare";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        Type: Type,
        CauHinhBaoHiemLoftCare: CauHinhBaoHiemLoftCare,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  createOrUpdateCauHinhBaoHiem(CauHinhBaoHiem: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/createOrUpdateCauHinhBaoHiem";
    return this.httpClient.post(url, {
      CauHinhBaoHiem: CauHinhBaoHiem,
    }).pipe(map((response: Response) => {
      return response;
    }));
  }
  deleteCauHinhBaoHiemLoftCare(Type: number, Id: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/DeleteCauHinhBaoHiemLoftCare";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        Type: Type,
        Id: Id
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  getMasterDataCreateDeXuatTangLuong() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/GetMasterDataCreateDeXuatTangLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  taoDeXuatTangLuong(dexuat, nhanviendexuat, listFile, folderType) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/TaoDeXuatTangLuong';
    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("UserId", this.userId);

    formData.append("DeXuatTangLuong.TenDeXuat", dexuat.TenDeXuat);
    formData.append("DeXuatTangLuong.LoaiDeXuat", dexuat.LoaiDeXuat);
    formData.append("DeXuatTangLuong.NgayDeXuat", dexuat.NgayDeXuat == null ? null : new Date(dexuat.NgayDeXuat).toUTCString());
    formData.append("DeXuatTangLuong.NguoiDeXuatId", dexuat.NguoiDeXuatId);
    formData.append("DeXuatTangLuong.TrangThai", dexuat.TrangThai);

    var index1 = 0;
    for (var nvien of nhanviendexuat) {
      formData.append("NhanVienDuocDeXuats[" + index1 + "].EmployeeId", nvien.employeeId);
      formData.append("NhanVienDuocDeXuats[" + index1 + "].PhongBanId", nvien.PhongBanId);
      formData.append("NhanVienDuocDeXuats[" + index1 + "].ChucVuId", nvien.chucVuId);
      formData.append("NhanVienDuocDeXuats[" + index1 + "].LuongHienTai", nvien.luongHienTai);
      formData.append("NhanVienDuocDeXuats[" + index1 + "].LuongDeXuat", nvien.luongDeXuat);
      formData.append("NhanVienDuocDeXuats[" + index1 + "].LyDoDeXuat", nvien.lyDoDeXuat);
      formData.append("NhanVienDuocDeXuats[" + index1 + "].Active", nvien.active);
      formData.append("NhanVienDuocDeXuats[" + index1 + "].TrangThai", nvien.trangThai);
      index1++;
    }


    var indexFile = 0;
    for (var file of listFile) {
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileInFolderId", file.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FolderId", file.FileInFolder.folderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileName", file.FileInFolder.fileName);
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectNumber", file.FileInFolder.objectNumber.toString());
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectType", file.FileInFolder.objectType);
      formData.append("ListFile[" + indexFile + "].FileInFolder.Size", file.FileInFolder.size);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileExtension", file.FileInFolder.fileExtension);
      formData.append("ListFile[" + indexFile + "].FileSave", file.FileSave);
      indexFile++;
    }


    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, formData).toPromise().then((response: Response) => {
        resolve(response);
      });
    });
  }

  deXuatTangLuongDetail(deXuatTLId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/DeXuatTangLuongDetail';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DeXuatTLId: deXuatTLId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getLichSuThayDoiChucVu(employeeId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getLichSuThayDoiChucVu";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        EmployeeId: employeeId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getThongTinBaoHiemVaThue(employeeId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getThongTinBaoHiemVaThue";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        EmployeeId: employeeId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getCauHinhBaoHiemLoftCareById() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getCauHinhBaoHiemLoftCareById";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  getMasterCreateKeHoachOt(UserId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterCreateKeHoachOt';
    return this.httpClient.post(url, { UserId: this.userId }).pipe(
      map((response: Response) => {
        return response;
      })
    );
  }

  createOrUpdateKeHoachOt(keHoachOt: any, keHoachOtId: number, folderType: string, listFile: Array<FileUploadModel>, listKeHoachOtPhongBan: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/createOrUpdateKeHoachOt';

    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("UserId", this.userId);

    formData.append("KeHoachOt.KeHoachOtId", keHoachOt.KeHoachOtId);
    formData.append("KeHoachOt.LoaiOtId", keHoachOt.LoaiOtId);
    formData.append("KeHoachOt.DiaDiem", keHoachOt.DiaDiem);
    formData.append("KeHoachOt.GioBatDau", keHoachOt.GioBatDau);
    formData.append("KeHoachOt.GioKetThuc", keHoachOt.GioKetThuc);
    formData.append("KeHoachOt.LoaiOt", keHoachOt.LoaiOt);
    formData.append("KeHoachOt.LyDo", keHoachOt.LyDo);
    formData.append("KeHoachOt.NgayBatDau", keHoachOt.NgayBatDau == null ? null : new Date(keHoachOt.NgayBatDau).toUTCString());
    formData.append("KeHoachOt.NgayDeXuat", keHoachOt.NgayDeXuat == null ? null : new Date(keHoachOt.NgayDeXuat).toUTCString());
    formData.append("KeHoachOt.NgayKetThuc", keHoachOt.NgayKetThuc == null ? null : new Date(keHoachOt.NgayKetThuc).toUTCString());
    formData.append("KeHoachOt.NguoiDeXuatId", keHoachOt.NguoiDeXuatId);
    formData.append("KeHoachOt.TenKeHoach", keHoachOt.TenKeHoach);
    formData.append("KeHoachOt.TrangThai", keHoachOt.TrangThai);
    formData.append("KeHoachOt.HanPheDuyetKeHoach", keHoachOt.HanPheDuyetKeHoach == null ? null : new Date(keHoachOt.HanPheDuyetKeHoach).toUTCString());
    formData.append("KeHoachOt.HanDangKy", keHoachOt.HanDangKy == null ? null : new Date(keHoachOt.HanDangKy).toUTCString());
    formData.append("KeHoachOt.HanPheDuyetDangKy", keHoachOt.HanPheDuyetDangKy == null ? null : new Date(keHoachOt.HanPheDuyetDangKy).toUTCString());
    formData.append("KeHoachOt.LoaiCaId", keHoachOt.LoaiCaId)

    var index1 = 0;
    for (var phongBan of listKeHoachOtPhongBan) {
      formData.append("ListKeHoachOtPhongBan[" + index1 + "].Id", phongBan.id);
      formData.append("ListKeHoachOtPhongBan[" + index1 + "].OrganizationId", phongBan.OrganizationId);
      formData.append("ListKeHoachOtPhongBan[" + index1 + "].KeHoachOtId", phongBan.keHoachOtId);
      formData.append("ListKeHoachOtPhongBan[" + index1 + "].TrangThai", phongBan.TrangThai);
      index1++;
    }

    var indexFile = 0;
    for (var file of listFile) {
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileInFolderId", file.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FolderId", file.FileInFolder.folderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileName", file.FileInFolder.fileName);
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectNumber", file.FileInFolder.objectNumber.toString());
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectType", file.FileInFolder.objectType);
      formData.append("ListFile[" + indexFile + "].FileInFolder.Size", file.FileInFolder.size);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileExtension", file.FileInFolder.fileExtension);
      formData.append("ListFile[" + indexFile + "].FileSave", file.FileSave);
      indexFile++;
    }
    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }


  getCauHinhBaoHiemById(Id: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getCauHinhBaoHiemById";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        Id: Id
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }



  updateDeXuatTangLuong(dexuat, nhanviendexuat, listFile, folderType) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/UpdateDeXuatTangLuong';
    return new Promise((resolve, reject) => {

      let formData: FormData = new FormData();
      formData.append("FolderType", folderType);
      formData.append("UserId", this.userId);

      formData.append("DeXuatTangLuong.DeXuatTangLuongId", dexuat.DeXuatTangLuongId);
      formData.append("DeXuatTangLuong.TenDeXuat", dexuat.TenDeXuat);
      formData.append("DeXuatTangLuong.LoaiDeXuat", dexuat.LoaiDeXuat);
      formData.append("DeXuatTangLuong.NgayDeXuat", dexuat.NgayDeXuat == null ? null : new Date(dexuat.NgayDeXuat).toUTCString());
      formData.append("DeXuatTangLuong.NguoiDeXuatId", dexuat.NguoiDeXuatId);
      formData.append("DeXuatTangLuong.TrangThai", dexuat.TrangThai);

      var index1 = 0;
      for (var nvien of nhanviendexuat) {
        formData.append("NhanVienDuocDeXuats[" + index1 + "].EmployeeId", nvien.employeeId);
        formData.append("NhanVienDuocDeXuats[" + index1 + "].PhongBanId", nvien.PhongBanId);
        formData.append("NhanVienDuocDeXuats[" + index1 + "].ChucVuId", nvien.chucVuId);
        formData.append("NhanVienDuocDeXuats[" + index1 + "].LuongHienTai", nvien.luongHienTai);
        formData.append("NhanVienDuocDeXuats[" + index1 + "].LuongDeXuat", nvien.luongDeXuat);
        formData.append("NhanVienDuocDeXuats[" + index1 + "].LyDoDeXuat", nvien.lyDoDeXuat);
        formData.append("NhanVienDuocDeXuats[" + index1 + "].Active", nvien.active);
        formData.append("NhanVienDuocDeXuats[" + index1 + "].TrangThai", nvien.trangThai);
        index1++;
      }


      var indexFile = 0;
      for (var file of listFile) {
        formData.append("ListFile[" + indexFile + "].FileInFolder.FileInFolderId", file.FileInFolder.fileInFolderId);
        formData.append("ListFile[" + indexFile + "].FileInFolder.FolderId", file.FileInFolder.folderId);
        formData.append("ListFile[" + indexFile + "].FileInFolder.FileName", file.FileInFolder.fileName);
        formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectNumber", file.FileInFolder.objectNumber.toString());
        formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectType", file.FileInFolder.objectType);
        formData.append("ListFile[" + indexFile + "].FileInFolder.Size", file.FileInFolder.size);
        formData.append("ListFile[" + indexFile + "].FileInFolder.FileExtension", file.FileInFolder.fileExtension);
        formData.append("ListFile[" + indexFile + "].FileSave", file.FileSave);
        indexFile++;
      }


      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  datVeMoiDeXuatTangLuong(dexuat: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/DatVeMoiDeXuatTangLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DeXuatTangLuongId: dexuat
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  tuChoiOrPheDuyetNhanVienDeXuatTL(listEmp: Array<any>, DeXuatTangLuongId: number, IsXacNhan, lyDoTuChoi) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/tuChoiOrPheDuyetNhanVienDeXuatTL';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListNV: listEmp,
        DeXuatTangLuongId: DeXuatTangLuongId,
        IsXacNhan: IsXacNhan,
        LyDoTuChoi: lyDoTuChoi,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveThongTinBaoHiemVaThue(employeeId: any, BaoHiemXaHoi: any, BaoHiemLoftCare: any, ThongTinThueVaGiamTru: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/saveThongTinBaoHiemVaThue";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        EmployeeId: employeeId,
        BaoHiemXaHoi,
        BaoHiemLoftCare,
        ThongTinThueVaGiamTru
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getListTaiLieuNhanVien(employeeId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getListTaiLieuNhanVien";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        EmployeeId: employeeId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createTaiLieuNhanVien(TaiLieu: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/createTaiLieuNhanVien";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        TaiLieu
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  tuChoiCheckListTaiLieu(employeeId: any, lyDo: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/tuChoiCheckListTaiLieu";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        EmployeeId: employeeId,
        LyDo: lyDo
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  xacNhanCheckListTaiLieu(employeeId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/xacNhanCheckListTaiLieu";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        EmployeeId: employeeId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  yeuCauXacNhanCheckListTaiLieu(employeeId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/yeuCauXacNhanCheckListTaiLieu";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        EmployeeId: employeeId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteTaiLieuNhanVienById(Id: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/deleteTaiLieuNhanVienById";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        Id: Id
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateTaiLieuNhanVien(TaiLieu: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/updateTaiLieuNhanVien";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        TaiLieu
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getTrinhDoHocVanTuyenDung(employeeId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getTrinhDoHocVanTuyenDung";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        EmployeeId: employeeId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateTrinhDoHocVanTuyenDung(TrinhDoHocVanTuyenDung: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/updateTrinhDoHocVanTuyenDung";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        TrinhDoHocVanTuyenDung
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteThongTinGiaDinhById(ContactId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/deleteThongTinGiaDinhById";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        ContactId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  getMasterSearchKeHoachOtAsync(listSearchTrangThai) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/GetMasterSearchKeHoachOt';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListTrangThai: listSearchTrangThai,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  getMasterCreateOrUpdateDeXuatCongTac() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/GetMasterCreateOrUpdateDeXuatCongTac';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  createOrUpdateDeXuatCongTacResult(dexuat, nhanviendexuat, folderType: string, listFile: Array<FileUploadModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/CreateOrUpdateDeXuatCongTac';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DeXuatCongTac: dexuat,
        ListChiTietDeXuatCongTac: nhanviendexuat,
        FolderType: folderType,
        ListFile: listFile,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  downloadTemplateImportDXTL() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/downloadTemplateImportDXTL';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  getMasterDataCreateDeXuatChucVu() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/GetMasterDataCreateDeXuatChucVu';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  taoDeXuatChucVu(dexuat: any, nhanviendexuat: any, listFile, folderType) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/taoDeXuatChucVu';
    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("UserId", this.userId);


    formData.append("DeXuatChucVu.TenDeXuat", dexuat.tenDeXuat);
    formData.append("DeXuatChucVu.NgayDeXuat", dexuat.ngayDeXuat == null ? null : new Date(dexuat.ngayDeXuat).toUTCString());
    formData.append("DeXuatChucVu.NguoiDeXuatId", dexuat.nguoiDeXuatId);

    var index1 = 0;
    for (var nvien of nhanviendexuat) {
      formData.append("NhanVienDuocDeXuats[" + index1 + "].EmployeeId", nvien.employeeId);
      formData.append("NhanVienDuocDeXuats[" + index1 + "].LyDoDeXuat", nvien.lyDoDeXuat);
      formData.append("NhanVienDuocDeXuats[" + index1 + "].ChucVuHienTaiId", nvien.chucVuHienTaiId);
      formData.append("NhanVienDuocDeXuats[" + index1 + "].ChucVuDeXuatId", nvien.chucVuDeXuatId);
      index1++;
    }


    var indexFile = 0;
    for (var file of listFile) {
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileInFolderId", file.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FolderId", file.FileInFolder.folderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileName", file.FileInFolder.fileName);
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectNumber", file.FileInFolder.objectNumber.toString());
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectType", file.FileInFolder.objectType);
      formData.append("ListFile[" + indexFile + "].FileInFolder.Size", file.FileInFolder.size);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileExtension", file.FileInFolder.fileExtension);
      formData.append("ListFile[" + indexFile + "].FileSave", file.FileSave);
      indexFile++;
    }
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  downloadTemplateImportDXCV() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/downloadTemplateImportDXCV';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deXuatChucVuDetail(deXuatTLId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/DeXuatChucVuDetail';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DeXuatTLId: deXuatTLId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  datVeMoiDeXuatChucVu(dexuat: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/DatVeMoiDeXuatChucVu';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DeXuatChucVuId: dexuat
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  tuChoiOrPheDuyetNhanVienDeXuatCV(listEmp: Array<any>, DeXuatChucVuId: number, IsXacNhan, lyDoTuChoi) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/tuChoiOrPheDuyetNhanVienDeXuatCV';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListNV: listEmp,
        DeXuatChucVuId: DeXuatChucVuId,
        IsXacNhan: IsXacNhan,
        LyDoTuChoi: lyDoTuChoi,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateDeXuatChucVu(dexuat, nhanviendexuat, listFile, folderType) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/updateDeXuatChucVu';
    return new Promise((resolve, reject) => {

      let formData: FormData = new FormData();
      formData.append("FolderType", folderType);
      formData.append("UserId", this.userId);
      formData.append("DeXuatChucVu.DeXuatThayDoiChucVuId", dexuat.deXuatThayDoiChucVuId);
      formData.append("DeXuatChucVu.TenDeXuat", dexuat.tenDeXuat);
      formData.append("DeXuatChucVu.LoaiDeXuat", dexuat.loaiDeXuat);
      formData.append("DeXuatChucVu.NgayDeXuat", dexuat.ngayDeXuat == null ? null : new Date(dexuat.ngayDeXuat).toUTCString());
      formData.append("DeXuatChucVu.NguoiDeXuatId", dexuat.nguoiDeXuatId);
      formData.append("DeXuatChucVu.TrangThai", dexuat.trangThai);



      var index1 = 0;
      for (var nvien of nhanviendexuat) {
        formData.append("NhanVienDuocDeXuats[" + index1 + "].EmployeeId", nvien.employeeId);
        formData.append("NhanVienDuocDeXuats[" + index1 + "].ChucVuDeXuatId", nvien.chucVuDeXuatId);
        formData.append("NhanVienDuocDeXuats[" + index1 + "].ChucVuHienTaiId", nvien.chucVuHienTaiId);
        formData.append("NhanVienDuocDeXuats[" + index1 + "].LyDoDeXuat", nvien.lyDoDeXuat);
        formData.append("NhanVienDuocDeXuats[" + index1 + "].TrangThai", nvien.trangThai);
        index1++;
      }


      var indexFile = 0;
      for (var file of listFile) {
        formData.append("ListFile[" + indexFile + "].FileInFolder.FileInFolderId", file.FileInFolder.fileInFolderId);
        formData.append("ListFile[" + indexFile + "].FileInFolder.FolderId", file.FileInFolder.folderId);
        formData.append("ListFile[" + indexFile + "].FileInFolder.FileName", file.FileInFolder.fileName);
        formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectNumber", file.FileInFolder.objectNumber.toString());
        formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectType", file.FileInFolder.objectType);
        formData.append("ListFile[" + indexFile + "].FileInFolder.Size", file.FileInFolder.size);
        formData.append("ListFile[" + indexFile + "].FileInFolder.FileExtension", file.FileInFolder.fileExtension);
        formData.append("ListFile[" + indexFile + "].FileSave", file.FileSave);
        indexFile++;
      }


      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });


  }

  getMasterKeHoachOTDetail(deXuatOTId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterKeHoachOTDetail';
    return this.httpClient.post(url, {
      DeXuatOTId: deXuatOTId
    }).pipe(
      map((response: Response) => {
        return response;
      })
    );
  }

  datVeMoiKeHoachOt(dexuat: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/datVeMoiKeHoachOt';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KeHoachOtId: dexuat,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  dangKyOTOrHuyDangKyOT(dexuat: number, type: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/dangKyOTOrHuyDangKyOT';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KeHoachOtId: dexuat,
        Type: type
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  pheDuyetNhanSuDangKyOT(listIdPheDuyet, ListAllNV: any, OrganizationId: any, KeHoachOtId: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/pheDuyetNhanSuDangKyOT';
    return this.httpClient.post(url, {
      ListNvPheDuyetID: listIdPheDuyet,
      ListNv: ListAllNV,
      OrganizationId: OrganizationId,
      KeHoachOtId: KeHoachOtId
    }).pipe(
      map((response: Response) => {
        return response;
      })
    );
  }


  tuChoiNhanVienOTTongPheDuyet(listIdTuChoi: any, ListAllNV: any, KeHoachOtId: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/tuChoiNhanVienOTTongPheDuyet';
    return this.httpClient.post(url, {
      ListIdTuChoi: listIdTuChoi,
      KeHoachOtId: KeHoachOtId,
      ListNv: ListAllNV,
    }).pipe(
      map((response: Response) => {
        return response;
      })
    );
  }


  giaHanThemKeHoachOT(dexuat: number, type: number, giaHanThemType: string, giaHanDangKyOT, giaHanPheDuyetDangKyOT) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/giaHanThemKeHoachOT';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KeHoachOtId: dexuat,
        Type: type,
        GiaHanThemType: giaHanThemType,
        GiaHanDangKyOT: giaHanDangKyOT,
        GiaHanPheDuyetDangKyOT: giaHanPheDuyetDangKyOT
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  deleteKehoachOT(KeHoachOtId: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/deleteKehoachOT';
    return this.httpClient.post(url, {
      KeHoachOtId: KeHoachOtId,
    }).pipe(
      map((response: Response) => {
        return response;
      })
    );
  }

  createOrUpdateFileUpload(folderType: string, listFile: Array<FileUploadModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/CreateOrUpdateFileUpload';
    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("UserId", this.userId);


    var index = 0;
    for (var pair of listFile) {
      formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
      index++;
    }
    return this.httpClient.post(url,
      formData
    ).pipe(map((response: Response) => {
      return response;
    }));
  }
  GetMasterDeXuatCongTacDetail(deXuatCongTacId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/GetMasterDeXuatCongTacDetail";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DeXuatCongTacId: deXuatCongTacId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  GetlistDeXuatCongTac(nguoiDeXuatId, thoiGianDeXuat, trangThai) {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/ListDeXuatTangLuong';
    return this.httpClient.post(url, {
      NguoiDeXuatId: nguoiDeXuatId,
      ThoiGianDeXuat: thoiGianDeXuat,
      TrangThai: trangThai,
      UserId: JSON.parse(currentUser).UserId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }
  datVeMoiDeXuatCongTac(dexuat: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/DatVeMoiDeXuatCongTac';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DeXuatCongTacId: dexuat
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  getMasterDataCreateCauHinhDanhGia() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterDataCreateCauHinhDanhGia';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  taoQuyLuong(data: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/taoQuyLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        QuyLuongId: null,
        Nam: data.Nam,
        QuyLuong: data.QuyLuong,

      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateQuyLuong(data: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/updateQuyLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        QuyLuongId: data.QuyLuongId,
        Nam: data.Nam,
        QuyLuong: data.QuyLuong,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteQuyLuong(data: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/deleteQuyLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        QuyLuongId: data.QuyLuongId,
        Nam: data.Nam,
        QuyLuong: data.QuyLuong,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createCauHinhDanhGia(data: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/createCauHinhDanhGia';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListThangDiemDanhGia: data
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  getMasterDataDeXuatCongTac() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterDataDeXuatCongTac';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getListCauHinhChecklist() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getListCauHinhChecklist';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateCauHinhChecklist(CauHinhChecklist: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/createOrUpdateCauHinhChecklist';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CauHinhChecklist: CauHinhChecklist
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteCauHinhDanhGia(data: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/deleteCauHinhDanhGia';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        MucDanhGiaId: data
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteCauHinhChecklistById(Id: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/deleteCauHinhChecklistById';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Id: Id
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataCreatePhieuDanhGia() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterDataCreatePhieuDanhGia';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  taoPhieuDanhGia(phieuDanhGia, cauHoiNV, cauHoiQL, listFile, folderType) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/taoPhieuDanhGia';
    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("UserId", this.userId);


    formData.append("PhieuDanhGia.TenPhieuDanhGia", phieuDanhGia.TenPhieuDanhGia);
    formData.append("PhieuDanhGia.HoatDong", phieuDanhGia.HoatDong);
    formData.append("PhieuDanhGia.ThangDiemDanhGiaId", phieuDanhGia.ThangDiemDanhGiaId);
    formData.append("PhieuDanhGia.CachTinhTong", phieuDanhGia.CachTinhTong);


    //NV
    var index1 = 0;
    for (var cauHoi of cauHoiNV) {
      formData.append("CauHoiNV[" + index1 + "].NoiDungCauHoi", cauHoi.noiDungCauHoi);
      formData.append("CauHoiNV[" + index1 + "].TiLe", cauHoi.tiLe);
      formData.append("CauHoiNV[" + index1 + "].ParentId", cauHoi.parentId);
      formData.append("CauHoiNV[" + index1 + "].NguoiDanhGia", cauHoi.nguoiDanhGia);
      formData.append("CauHoiNV[" + index1 + "].IsFarther", cauHoi.isFarther);
      formData.append("CauHoiNV[" + index1 + "].Stt", cauHoi.stt);
      formData.append("CauHoiNV[" + index1 + "].CauHoiPhieuDanhGiaMappingId", cauHoi.cauHoiPhieuDanhGiaMappingId);
      formData.append("CauHoiNV[" + index1 + "].PhieuDanhGiaId", cauHoi.phieuDanhGiaId);


      if (cauHoi.parentId != 0) {  //Nếu là thư mục con

        formData.append("CauHoiNV[" + index1 + "].CauTraLoi.Value", cauHoi.cauTraLoi.value);
        formData.append("CauHoiNV[" + index1 + "].CauTraLoi.Name", cauHoi.cauTraLoi.name);
        formData.append("CauHoiNV[" + index1 + "].CauTraLoi.ValueText", cauHoi.cauTraLoi.valueText);

        if (cauHoi.cauTraLoi.value == 2) {
          let indexDanhMuc = 0;
          cauHoi.danhSachItem.forEach(item => {
            formData.append("CauHoiNV[" + index1 + "].DanhSachItem[" + indexDanhMuc + "].CategoryId", item.categoryId);
            formData.append("CauHoiNV[" + index1 + "].DanhSachItem[" + indexDanhMuc + "].categoryCode", item.categoryCode);
            formData.append("CauHoiNV[" + index1 + "].DanhSachItem[" + indexDanhMuc + "].CategoryTypeName", item.categoryTypeName);
            indexDanhMuc++;
          });
        }
      }

      index1++;
    }

    //QL
    var index2 = 0;
    for (var cauHoi of cauHoiQL) {
      formData.append("CauHoiQL[" + index2 + "].NoiDungCauHoi", cauHoi.noiDungCauHoi);
      formData.append("CauHoiQL[" + index2 + "].TiLe", cauHoi.tiLe);
      formData.append("CauHoiQL[" + index2 + "].ParentId", cauHoi.parentId);
      formData.append("CauHoiQL[" + index2 + "].NguoiDanhGia", cauHoi.nguoiDanhGia);
      formData.append("CauHoiQL[" + index2 + "].IsFarther", cauHoi.isFarther);
      formData.append("CauHoiQL[" + index2 + "].Stt", cauHoi.stt);
      formData.append("CauHoiQL[" + index2 + "].CauHoiPhieuDanhGiaMappingId", cauHoi.cauHoiPhieuDanhGiaMappingId);
      formData.append("CauHoiQL[" + index2 + "].PhieuDanhGiaId", cauHoi.phieuDanhGiaId);


      if (cauHoi.parentId != 0) {  //Nếu là thư mục con

        formData.append("CauHoiQL[" + index2 + "].CauTraLoi.Value", cauHoi.cauTraLoi.value);
        formData.append("CauHoiQL[" + index2 + "].CauTraLoi.Name", cauHoi.cauTraLoi.name);
        formData.append("CauHoiQL[" + index2 + "].CauTraLoi.ValueText", cauHoi.cauTraLoi.valueText);

        if (cauHoi.cauTraLoi.value == 2) {
          let indexDanhMuc = 0;
          cauHoi.danhSachItem.forEach(item => {
            formData.append("CauHoiQL[" + index2 + "].DanhSachItem[" + indexDanhMuc + "].CategoryId", item.categoryId);
            formData.append("CauHoiQL[" + index2 + "].DanhSachItem[" + indexDanhMuc + "].CategoryCode", item.categoryCode);
            formData.append("CauHoiQL[" + index2 + "].DanhSachItem[" + indexDanhMuc + "].CategoryTypeName", item.categoryTypeName);
            indexDanhMuc++;
          });
        }
      }

      index2++;
    }


    var indexFile = 0;
    for (var file of listFile) {
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileInFolderId", file.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FolderId", file.FileInFolder.folderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileName", file.FileInFolder.fileName);
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectNumber", file.FileInFolder.objectNumber.toString());
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectType", file.FileInFolder.objectType);
      formData.append("ListFile[" + indexFile + "].FileInFolder.Size", file.FileInFolder.size);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileExtension", file.FileInFolder.fileExtension);
      formData.append("ListFile[" + indexFile + "].FileSave", file.FileSave);
      indexFile++;
    }


    return this.httpClient.post(url, formData).pipe(map((response: Response) => {
      return response;
    }));
  }
  updateCauHinhDanhGia(listMucDanhGia: any, mucDanhGiaId: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/updateCauHinhDanhGia';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListThangDiemDanhGia: listMucDanhGia,
        MucDanhGiaId: mucDanhGiaId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getListCapPhatTaiSan(employeeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getListCapPhatTaiSan';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmployeeId: employeeId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  phieuDanhGiaDetail(phieuDanhGiaId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/phieuDanhGiaDetail';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        PhieuDanhGiaId: phieuDanhGiaId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  capNhatPhieuDanhGia(phieuDanhGia, cauHoiNV, cauHoiQL, listFile, folderType) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/capNhatPhieuDanhGia';
    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("UserId", this.userId);


    formData.append("PhieuDanhGia.PhieuDanhGiaId", phieuDanhGia.PhieuDanhGiaId);
    formData.append("PhieuDanhGia.TenPhieuDanhGia", phieuDanhGia.TenPhieuDanhGia);
    formData.append("PhieuDanhGia.TrangThaiPhieuDanhGia", phieuDanhGia.HoatDong);
    formData.append("PhieuDanhGia.ThangDiemDanhGiaId", phieuDanhGia.ThangDiemDanhGiaId);
    formData.append("PhieuDanhGia.CachTinhTong", phieuDanhGia.CachTinhTong);


    //NV
    var index1 = 0;
    for (var cauHoi of cauHoiNV) {
      formData.append("CauHoiNV[" + index1 + "].NoiDungCauHoi", cauHoi.noiDungCauHoi);
      formData.append("CauHoiNV[" + index1 + "].TiLe", cauHoi.tiLe);
      formData.append("CauHoiNV[" + index1 + "].ParentId", cauHoi.parentId);
      formData.append("CauHoiNV[" + index1 + "].NguoiDanhGia", cauHoi.nguoiDanhGia);
      formData.append("CauHoiNV[" + index1 + "].IsFarther", cauHoi.isFarther);
      formData.append("CauHoiNV[" + index1 + "].Stt", cauHoi.stt);
      formData.append("CauHoiNV[" + index1 + "].CauHoiPhieuDanhGiaMappingId", cauHoi.cauHoiPhieuDanhGiaMappingId);
      formData.append("CauHoiNV[" + index1 + "].PhieuDanhGiaId", cauHoi.phieuDanhGiaId);


      if (cauHoi.parentId != 0) {  //Nếu là thư mục con

        formData.append("CauHoiNV[" + index1 + "].CauTraLoi.Value", cauHoi.cauTraLoi.value);
        formData.append("CauHoiNV[" + index1 + "].CauTraLoi.Name", cauHoi.cauTraLoi.name);
        formData.append("CauHoiNV[" + index1 + "].CauTraLoi.ValueText", cauHoi.cauTraLoi.valueText);

        if (cauHoi.cauTraLoi.value == 2) {
          let indexDanhMuc = 0;
          cauHoi.danhSachItem.forEach(item => {
            formData.append("CauHoiNV[" + index1 + "].DanhSachItem[" + indexDanhMuc + "].CategoryId", item.categoryId);
            formData.append("CauHoiNV[" + index1 + "].DanhSachItem[" + indexDanhMuc + "].categoryCode", item.categoryCode);
            formData.append("CauHoiNV[" + index1 + "].DanhSachItem[" + indexDanhMuc + "].CategoryTypeName", item.categoryTypeName);
            indexDanhMuc++;
          });
        }
      }

      index1++;
    }

    //QL
    var index2 = 0;
    for (var cauHoi of cauHoiQL) {
      formData.append("CauHoiQL[" + index2 + "].NoiDungCauHoi", cauHoi.noiDungCauHoi);
      formData.append("CauHoiQL[" + index2 + "].TiLe", cauHoi.tiLe);
      formData.append("CauHoiQL[" + index2 + "].ParentId", cauHoi.parentId);
      formData.append("CauHoiQL[" + index2 + "].NguoiDanhGia", cauHoi.nguoiDanhGia);
      formData.append("CauHoiQL[" + index2 + "].IsFarther", cauHoi.isFarther);
      formData.append("CauHoiQL[" + index2 + "].Stt", cauHoi.stt);
      formData.append("CauHoiQL[" + index2 + "].CauHoiPhieuDanhGiaMappingId", cauHoi.cauHoiPhieuDanhGiaMappingId);
      formData.append("CauHoiQL[" + index2 + "].PhieuDanhGiaId", cauHoi.phieuDanhGiaId);


      if (cauHoi.parentId != 0) {  //Nếu là thư mục con

        formData.append("CauHoiQL[" + index2 + "].CauTraLoi.Value", cauHoi.cauTraLoi.value);
        formData.append("CauHoiQL[" + index2 + "].CauTraLoi.Name", cauHoi.cauTraLoi.name);
        formData.append("CauHoiQL[" + index2 + "].CauTraLoi.ValueText", cauHoi.cauTraLoi.valueText);

        if (cauHoi.cauTraLoi.value == 2) {
          let indexDanhMuc = 0;
          cauHoi.danhSachItem.forEach(item => {
            formData.append("CauHoiQL[" + index2 + "].DanhSachItem[" + indexDanhMuc + "].CategoryId", item.categoryId);
            formData.append("CauHoiQL[" + index2 + "].DanhSachItem[" + indexDanhMuc + "].CategoryCode", item.categoryCode);
            formData.append("CauHoiQL[" + index2 + "].DanhSachItem[" + indexDanhMuc + "].CategoryTypeName", item.categoryTypeName);
            indexDanhMuc++;
          });
        }
      }

      index2++;
    }



    var indexFile = 0;
    for (var file of listFile) {
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileInFolderId", file.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FolderId", file.FileInFolder.folderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileName", file.FileInFolder.fileName);
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectNumber", file.FileInFolder.objectNumber.toString());
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectType", file.FileInFolder.objectType);
      formData.append("ListFile[" + indexFile + "].FileInFolder.Size", file.FileInFolder.size);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileExtension", file.FileInFolder.fileExtension);
      formData.append("ListFile[" + indexFile + "].FileSave", file.FileSave);
      indexFile++;
    }


    return this.httpClient.post(url, formData).pipe(map((response: Response) => {
      return response;
    }));
  }


  hoanThanhOrUpdateStatusPhieuDanhGia(phieuDanhGiaId, trangThai, type) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/hoanThanhOrUpdateStatusPhieuDanhGia';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        PhieuDanhGiaId: phieuDanhGiaId,
        TrangThaiPhieuDanhGia: trangThai,
        Type: type,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }



  capNhatLyDoPheDuyetOrTuChoiDeXuatNV(deXuatNhanVienId: number, ghiChu: string, loaiDeXuat: number, nghiaVu: string = null) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/capNhatLyDoPheDuyetOrTuChoiDeXuatNV";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        DeXuatNhanVienId: deXuatNhanVienId,
        GhiChu: ghiChu,
        NghiaVu: nghiaVu,
        LoaiDeXuat: loaiDeXuat,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  capNhatNgayApDungDeXuat(ngayApDung: Date, deXuatId: number, loaiDeXuat: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/capNhatNgayApDungDeXuat";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        NgayApDung: ngayApDung,
        DeXuatId: deXuatId,
        LoaiDeXuat: loaiDeXuat,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  getMasterDataTaoKyDanhGia() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterDataTaoKyDanhGia';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  danhSachKyDanhGia() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/danhSachKyDanhGia';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  taoKyDanhGia(kyDanhGia, listNoiDungKyDanhGia, listNhanVienKyDanhGia, listFile, folderType) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/taoKyDanhGia';
    return new Promise((resolve, reject) => {
      let formData: FormData = new FormData();
      formData.append("FolderType", folderType);
      formData.append("UserId", this.userId);

      formData.append("KyDanhGia.TenKyDanhGia", kyDanhGia.TenKyDanhGia);
      formData.append("KyDanhGia.ThoiGianBatDau", kyDanhGia.ThoiGianBatDau == null ? null : new Date(kyDanhGia.ThoiGianBatDau).toUTCString());
      formData.append("KyDanhGia.ThoiGianKetThuc", kyDanhGia.ThoiGianKetThuc == null ? null : new Date(kyDanhGia.ThoiGianKetThuc).toUTCString());
      formData.append("KyDanhGia.LyDo", kyDanhGia.LyDo);

      var index1 = 0;
      for (var obj of listNoiDungKyDanhGia) {
        formData.append("NoiDungKyDanhGia[" + index1 + "].PhieuDanhGiaId", obj.phieuDanhGiaId);
        formData.append("NoiDungKyDanhGia[" + index1 + "].PositionId", obj.positionId);
        index1++;
      }

      var index2 = 0;
      for (var obj of listNhanVienKyDanhGia) {
        formData.append("NhanVienKyDanhGia[" + index2 + "].ParentId", obj.parentId);
        formData.append("NhanVienKyDanhGia[" + index2 + "].Level", obj.level);
        formData.append("NhanVienKyDanhGia[" + index2 + "].KyDanhGia", obj.kyDanhGia);
        formData.append("NhanVienKyDanhGia[" + index2 + "].NguoiDanhGiaId", obj.nguoiDanhGiaId);
        formData.append("NhanVienKyDanhGia[" + index2 + "].NguoiDuocDanhGiaId", obj.nguoiDuocDanhGiaId);
        formData.append("NhanVienKyDanhGia[" + index2 + "].OrganizationId", obj.organizationId);
        formData.append("NhanVienKyDanhGia[" + index2 + "].QuyLuong", obj.quyLuong);
        formData.append("NhanVienKyDanhGia[" + index2 + "].XemLuong", obj.xemLuong);
        index2++;
      }

      var indexFile = 0;
      for (var file of listFile) {
        formData.append("ListFile[" + indexFile + "].FileInFolder.FileInFolderId", file.FileInFolder.fileInFolderId);
        formData.append("ListFile[" + indexFile + "].FileInFolder.FolderId", file.FileInFolder.folderId);
        formData.append("ListFile[" + indexFile + "].FileInFolder.FileName", file.FileInFolder.fileName);
        formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectNumber", file.FileInFolder.objectNumber.toString());
        formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectType", file.FileInFolder.objectType);
        formData.append("ListFile[" + indexFile + "].FileInFolder.Size", file.FileInFolder.size);
        formData.append("ListFile[" + indexFile + "].FileInFolder.FileExtension", file.FileInFolder.fileExtension);
        formData.append("ListFile[" + indexFile + "].FileSave", file.FileSave);
        indexFile++;
      }

      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteKyDanhGia(kyDanhGiaid: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/deleteKyDanhGia';
    return this.httpClient.post(url, {
      KyDanhGiaId: kyDanhGiaid
    }).pipe(map((response: Response) => {
      return response;
    }));
  }


  kyDanhGiaDetail(kyDanhGiaId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/kyDanhGiaDetail';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        kyDanhGiaId: kyDanhGiaId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  xoaPhieuDanhGiaCuaKy(kyDanhGiaId, phieuDanhGiaId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/xoaPhieuDanhGiaCuaKy';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        kyDanhGiaId: kyDanhGiaId,
        PhieuDanhGiaId: phieuDanhGiaId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  luuPhieuDanhGiaCuaKy(kyDanhGiaId, noiDungKyDanhGia) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/luuPhieuDanhGiaCuaKy';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KyDanhGiaId: kyDanhGiaId,
        NoiDungKyDanhGia: noiDungKyDanhGia,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  capNhatKyDanhGia(kyDanhGia, listFile, folderType) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/capNhatKyDanhGia';
    return new Promise((resolve, reject) => {
      let formData: FormData = new FormData();
      formData.append("FolderType", folderType);
      formData.append("UserId", this.userId);

      formData.append("KyDanhGia.KyDanhGiaId", kyDanhGia.KyDanhGiaId);
      formData.append("KyDanhGia.TenKyDanhGia", kyDanhGia.TenKyDanhGia);
      formData.append("KyDanhGia.ThoiGianBatDau", kyDanhGia.ThoiGianBatDau == null ? null : new Date(kyDanhGia.ThoiGianBatDau).toUTCString());
      formData.append("KyDanhGia.ThoiGianKetThuc", kyDanhGia.ThoiGianKetThuc == null ? null : new Date(kyDanhGia.ThoiGianKetThuc).toUTCString());
      formData.append("KyDanhGia.LyDo", kyDanhGia.LyDo);
      formData.append("KyDanhGia.TrangThaiDanhGia", kyDanhGia.TrangThaiDanhGia);

      var indexFile = 0;
      for (var file of listFile) {
        formData.append("ListFile[" + indexFile + "].FileInFolder.FileInFolderId", file.FileInFolder.fileInFolderId);
        formData.append("ListFile[" + indexFile + "].FileInFolder.FolderId", file.FileInFolder.folderId);
        formData.append("ListFile[" + indexFile + "].FileInFolder.FileName", file.FileInFolder.fileName);
        formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectNumber", file.FileInFolder.objectNumber.toString());
        formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectType", file.FileInFolder.objectType);
        formData.append("ListFile[" + indexFile + "].FileInFolder.Size", file.FileInFolder.size);
        formData.append("ListFile[" + indexFile + "].FileInFolder.FileExtension", file.FileInFolder.fileExtension);
        formData.append("ListFile[" + indexFile + "].FileSave", file.FileSave);
        indexFile++;
      }

      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  xoaPhongBanKyDanhGia(kyDanhGiaId, organizationId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/xoaPhongBanKyDanhGia';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KyDanhGiaId: kyDanhGiaId,
        OrganizationId: organizationId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrAddPhongBanKyDanhGia(kyDanhGiaId, organizationId, quyLuong) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/createOrAddPhongBanKyDanhGia';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KyDanhGiaId: kyDanhGiaId,
        OrganizationId: organizationId,
        QuyLuong: quyLuong,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  hoanThanhkyDanhGia(kyDanhGiaId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/hoanThanhkyDanhGia';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KyDanhGiaId: kyDanhGiaId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateNguoiDanhGiaNhanVienKy(kyDanhGiaId, listNVKyDanhGia,) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/updateNguoiDanhGiaNhanVienKy';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KyDanhGiaId: kyDanhGiaId,
        ListNVKyDanhGia: listNVKyDanhGia,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  thucHienDanhGiaDetail(danhGiaNhanVienId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/thucHienDanhGiaDetail';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DanhGiaNhanVienId: danhGiaNhanVienId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  taoPhieuTuDanhGiaNhanVien(nhanVienKyDanhGiaId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/taoPhieuTuDanhGiaNhanVien';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        nhanVienKyDanhGiaId: nhanVienKyDanhGiaId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  luuOrHoanThanhDanhGia(danhGiaNhanVien, listCauTraLoi, isClickHoanThanh: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/luuOrHoanThanhDanhGia';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DanhGiaNhanVien: danhGiaNhanVien,
        ListCauTraLoi: listCauTraLoi,
        IsClickHoanThanh: isClickHoanThanh
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  capNhatDanhGiaNhanVienRow(danhGiaNhanVienId, mucLuongDeXuatQuanLy) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/capNhatDanhGiaNhanVienRow';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DanhGiaNhanVienId: danhGiaNhanVienId,
        MucLuongDeXuatQuanLy: mucLuongDeXuatQuanLy
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  ganMucDanhGiaChung(listNv, mucDanhGiaMasterDataId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/ganMucDanhGiaChung';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListNhanVien: listNv,
        MucDanhGiaMasterDataId: mucDanhGiaMasterDataId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  taoDeXuatTangLuongKyDanhGia(kyDanhGiaId, orgId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/taoDeXuatTangLuongKyDanhGia';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KyDanhGiaId: kyDanhGiaId,
        OrgId: orgId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateLichSuThanhToanBaoHiem(LichSuThanhToanBaoHiem) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/createOrUpdateLichSuThanhToanBaoHiem';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        LichSuThanhToanBaoHiem
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteLichSuThanhToanBaoHiemById(lichSuThanhToanBaoHiemId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/deleteLichSuThanhToanBaoHiemById';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        LichSuThanhToanBaoHiemId: lichSuThanhToanBaoHiemId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDateImportEmployee(type: number, listEmpId = null) {
    // 1: Nhập
    // 2: Xuất
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterDateImportEmployee';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Type: type,
        ListEmpId: listEmpId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  importEmployee(ListEmp) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/importEmployee';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListEmp: ListEmp
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  downloadTemplateImportEmployee() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/downloadTemplateImportEmployee';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getListBaoCaoNhanSu() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/getListBaoCaoNhanSu';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getBieuDoThongKeNhanSu(Type, FromDate: any = null, ToDate: any = null) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getBieuDoThongKeNhanSu';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Type: Type,
        FromDate: FromDate,
        ToDate: ToDate
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  giaHanThemPheDuyetKHOT(keHoachOtId, hanPheDuyetKhot, hanDangKyOt, hanPheDuyetDangKyOt) {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/giaHanPheDuyetKeHoachOT';
    return this.httpClient.post(url, {
      UserId: JSON.parse(currentUser).UserId,
      KeHoachOtId: keHoachOtId,
      HanPheDuyetKeHoachOt: hanPheDuyetKhot,
      HanDangKyKeHoachOT: hanDangKyOt,
      HanPheDuyetDangKyOt: hanPheDuyetDangKyOt
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  saveGhiChuNhanVienKeHoachOT(ThanhVienOtId, GhiChu) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/saveGhiChuNhanVienKeHoachOT';
    return this.httpClient.post(url, {
      GhiChu: GhiChu,
      ThanhVienOtId: ThanhVienOtId
    }).pipe(
      map((response: Response) => {
        return response;
      })
    );
  }


  layNhanVienCungCapVaCapDuoiOrg(phongBanId, trangThai, kyDanhGiaId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/layNhanVienCungCapVaCapDuoiOrg';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        OrgId: phongBanId,
        TrangThai: trangThai,
        KyDanhGiaId: kyDanhGiaId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  hoanThanhDanhGiaPhongBan(kyDanhGiaId, orgId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/hoanThanhDanhGiaPhongBan';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KyDanhGiaId: kyDanhGiaId,
        OrgId: orgId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  downloadTemplateImportHDNS() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/downloadTemplateImportHDNS";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDateImportHDNS() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getMasterDateImportHDNS";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  importHDNS(listHopDong) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/importHDNS';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListHopDong: listHopDong,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteEmployee(listEmployeeId: string[]): Observable<DeleteEmployeeResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/deleteEmployee';
    return this.httpClient.post(url, {
      ListEmployeeId: listEmployeeId
    }).pipe(
      map((response: DeleteEmployeeResult) => {
        return response;
      })
    );
  }

  takeListEvaluateForObjectId(employeeId: string): Observable<TakeListEvaluateResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/takeListEvaluateForObjectId';
    return this.httpClient.post(url, {
      EmployeeId: employeeId
    }).pipe(
      map((response: TakeListEvaluateResult) => {
        return response;
      })
    );
  }
  
}


