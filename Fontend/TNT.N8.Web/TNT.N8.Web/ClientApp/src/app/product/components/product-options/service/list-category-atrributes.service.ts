import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs-compat/Observable';
import { map } from 'rxjs/operators';
import { AttributeConfigureResult, ListCategoryAttributeModel } from '../model/list-category-attributes';

@Injectable({
  providedIn: 'root'
})
export class ListCategoryAtrributesService {

  constructor(private httpClient: HttpClient) { }
  getListCategoryAttribute() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/options/getCategoryAttributes';
    return this.httpClient.get(url, {}).pipe(
      map((response: ListCategoryAttributeModel) => {
        return <ListCategoryAttributeModel>response;
      }));
  }
  getListAttribute() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/options/getAllAttributes';
    return this.httpClient.get(url, {}).pipe(
      map((response: ListCategoryAttributeModel) => {
        return <ListCategoryAttributeModel>response;
      }));
  }
  getListCategoryAttributesById(id: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/options/getCategoryAttributeById';
    return this.httpClient.post(url, {
      Id: id
    }).pipe(
      map((response: any) => {
        return response;
      }));
  }
  deleteAttributeConfigure(id: string): Observable<AttributeConfigureResult>{
    const url = localStorage.getItem('ApiEndPoint') + '/api/options/deleteAttributeConfigure';
    return this.httpClient.post(url, { 
      Id : id
     }).pipe(
      map((response: AttributeConfigureResult) => {
        return <AttributeConfigureResult>response;
      }));
  }
}
