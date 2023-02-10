import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/internal/operators/map';


@Injectable({
  providedIn: 'root'
})
export class AuditTraceService {

  constructor(private httpClient: HttpClient) { }

  GetMasterDataTrace() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/admin/getMasterDataTrace';
    return this.httpClient.post(url, { }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchTrace(lstUserId: Array<any>, lstEmpId: Array<any>, 
    lstStatus: Array<any>, lstAction: Array<any>, 
    lstObject: Array<any>, searchDate: Date, 
    isSelectedLoginAudit: boolean, isSelectedAuditTrace: boolean,
    pageSize: number, pageIndex: number){
    const url = localStorage.getItem('ApiEndPoint') + '/api/admin/searchTrace';
    return this.httpClient.post(url, { 
      ListUserId: lstUserId,
      ListEmployeeId: lstEmpId,
      ListStatus: lstStatus,
      ListActionName: lstAction,
      ListObjectType: lstObject,
      SearchDate: searchDate,
      IsSelectedLoginAudit: isSelectedLoginAudit,
      IsSelectedAuditTrace: isSelectedAuditTrace,
      PageSize: pageSize,
      PageIndex: pageIndex,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
