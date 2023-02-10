import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class DanhSachOtService {

  constructor(
    private httpClient: HttpClient
  ) { }
  getListOT(): Observable<any> {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + 'api/employee/getMasterSearchKeHoachOt';
    return this.httpClient.post(url, {
      UserId: JSON.parse(currentUser).UserId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }



}
