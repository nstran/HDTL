
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
@Injectable()
export class EmployeeTimesheetService {

  constructor(private httpClient: HttpClient) { }

  uploadFile(fileList: File[], month: number, year: number, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employeeSalary/employeeTimeSheetImport';
    let formData: FormData = new FormData();
    for (var i = 0; i < fileList.length; i++) {
      formData.append('fileList', fileList[i]);
    }
    formData.append('Month', month.toString());
    formData.append('Year', year.toString());
    formData.append('UserId', userId);
    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
