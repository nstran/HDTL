import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators';
import { CreateOptionAndAttrModel, CreateOptionAndAttrResult, GetGiaTriThuocTinhModel } from '../model/data-type';
import { AttributeConfigurationModel } from '../model/options';

@Injectable({
  providedIn: 'root'
})
export class DataTypeService {

  constructor(private httpClient: HttpClient) { }
  getListDataType() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/options/getDataType';
    return this.httpClient.get(url, {}).pipe(
      map((response: GetGiaTriThuocTinhModel) => {
        return <GetGiaTriThuocTinhModel>response;
      }));
  }
  createOptionAndAttrModel(attributeConfigurationModel: AttributeConfigurationModel): Observable<CreateOptionAndAttrResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/options/createAttrConfig';
    return this.httpClient.post(url, {
      AttributeConfigurationModel : attributeConfigurationModel,
    }).pipe(
      map((response: CreateOptionAndAttrResult) => {
        return <CreateOptionAndAttrResult>response;
      }));
  }
}
