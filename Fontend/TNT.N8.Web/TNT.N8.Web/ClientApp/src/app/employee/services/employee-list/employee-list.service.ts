
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EmployeeModel, TakeListOrganizationToFilterEmployeeResult } from "../../models/employee.model";
import { ContactModel } from "../../../shared/models/contact.model";
import { UserModel } from '../../../shared/models/user.model';

@Injectable()
export class EmployeeListService {

  constructor(private httpClient: HttpClient) { }

  searchEmployeeFromList(
    isManager: boolean, employeeId: string, firstName: string, lastName: string, userName: string, identityId: string, position: Array<any>, organizationId: string,
    fromContractExpiryDate: Date, toContractExpiryDate: Date, fromBirthDay: Date, toBirthDay: Date, isQuitWork: boolean, organizationIds: string[]) {

    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/searchFromList';
    return this.httpClient.post(url, {
      IsManager: isManager,
      EmployeeId: employeeId,
      FirstName: firstName,
      LastName: lastName,
      UserName: userName,
      IdentityId: identityId,
      ListPosition: position,
      OrganizationId: organizationId,
      FromContractExpiryDate: fromContractExpiryDate,
      ToContractExpiryDate: toContractExpiryDate,
      FromBirthDay: fromBirthDay,
      ToBirthDay: toBirthDay,
      isQuitWork: isQuitWork,
      UserId: JSON.parse(currentUser).UserId,
      OrganizationIds : organizationIds
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  disableEmployee(employeeId: string) {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/disableEmployee';
    return this.httpClient.post(url, {
      EmployeeId: employeeId,
      UserId: JSON.parse(currentUser).UserId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }


  listDeXuatTangLuong(nguoiDeXuatId, thoiGianDeXuat, trangThai) {
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

  deleteDeXuatTangLuong(deXuatId) {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/DeleteDeXuatTangLuong';
    return this.httpClient.post(url, {
      DeXuatId: deXuatId,
      UserId: JSON.parse(currentUser).UserId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  deleteDeXuatChucVu(deXuatId) {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/DeleteDeXuatChucVu';
    return this.httpClient.post(url, {
      DeXuatThayDoiChucVuId: deXuatId,
      UserId: JSON.parse(currentUser).UserId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }



  listDeXuatChucVu(nguoiDeXuatId, thoiGianDeXuat, trangThai) {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/ListDeXuatChucVu';
    return this.httpClient.post(url, {
      NguoiDeXuatId: nguoiDeXuatId,
      ThoiGianDeXuat: thoiGianDeXuat,
      TrangThai: trangThai,
      UserId: JSON.parse(currentUser).UserId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }


  getListDeXuatCongTac(nguoiDeXuatId, thoiGianDeXuat, trangThai) {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/GetListDeXuatCongTac';
    return this.httpClient.post(url, {
      NguoiDeXuatId: nguoiDeXuatId,
      ThoiGianDeXuat: thoiGianDeXuat,
      TrangThaiDeXuat: trangThai,
      UserId: JSON.parse(currentUser).UserId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }
  deleteDeXuatCongTac(deXuatId) {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/DeleteDeXuatCongTac';
    return this.httpClient.post(url, {
      DeXuatCongTacId: deXuatId,
      UserId: JSON.parse(currentUser).UserId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  danhSachPhieuDanhGia() {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/danhSachPhieuDanhGia';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: JSON.parse(currentUser).UserId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  deletePhieuDanhGia(phieuDanhGiaId) {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/deletePhieuDanhGia';
    return this.httpClient.post(url, {
      PhieuDanhGiaId: phieuDanhGiaId,
      UserId: JSON.parse(currentUser).UserId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  takeListOrganizationToFilterEmployee() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/takeListOrganizationToFilterEmployee';
    return this.httpClient.post(url, {})
    .pipe(
      map((response: TakeListOrganizationToFilterEmployeeResult) => {
        return response;
    }));
  }

}
