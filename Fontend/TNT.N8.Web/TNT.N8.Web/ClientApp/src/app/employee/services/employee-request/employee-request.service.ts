
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EmployeeRequestModel } from '../../models/employee-request.model';

@Injectable()
export class EmployeeRequestService {

  constructor(private httpClient: HttpClient) { }

  createEmployeeRequest(DeXuatXinNghi: any, IsGuiPheDuyet: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employeeRequest/create';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DeXuatXinNghi: DeXuatXinNghi,
        IsGuiPheDuyet: IsGuiPheDuyet
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  editEmployeeRequestById(DeXuatXinNghi: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeRequest/editEmployeeRequestById";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DeXuatXinNghi: DeXuatXinNghi,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getEmployeeRequestById(DeXuatXinNghiId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeRequest/getEmployeeRequestById";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DeXuatXinNghiId: DeXuatXinNghiId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterCreateEmpRequest() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employeeRequest/getMasterCreateEmpRequest';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {

      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataSearchEmployeeRequest() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employeeRequest/getDataSearchEmployeeRequest';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {

      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchEmployeeRequest(code: string, employeeCode: string, employeeName: string,
    organizationId: string, listLoaiDeXuatId: Array<any>, listStatusId: Array<any>) 
  {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employeeRequest/search';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        code: code,
        employeeCode: employeeCode,
        employeeName: employeeName,
        organizationId: organizationId,
        listLoaiDeXuatId: listLoaiDeXuatId,
        listStatusId: listStatusId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteDeXuatXinNghiById(Id: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employeeRequest/deleteDeXuatXinNghiById';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Id: Id
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  datVeMoiDeXuatXinNghi(Id: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employeeRequest/datVeMoiDeXuatXinNghi';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Id: Id
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
}
