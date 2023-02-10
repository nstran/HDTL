import { HttpClient } from '@angular/common/http';
import { Identifiers } from '@angular/compiler';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { GetListServiceTypeResult } from '../model/category';
import { OptionCategoryTable } from '../model/option-category';
import { CreateOrUpdateOptionsResult, OptionsEntityModel, OptionsEntityTable, SearchOptionTreeResult } from '../model/options';
import { OptionByIdResult, OptionByIdTable, ProductOptionModel } from '../model/product-option-model';

@Injectable({
  providedIn: 'root'
})
export class ProductOptionService {

  constructor(private httpClient: HttpClient) { }
  searchProductOption(listCategoryId: Array<string>, listProductOption: ProductOptionModel) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/options/searchOptions';
    return this.httpClient.post(url, {
      ProductOptionModel: listProductOption,
      ListCategoryId: listCategoryId
    }).pipe(
      map((response: ProductOptionModel) => {
        return response;
      }));
  }
  searchProductOptionAsync(categoryName: string, optionName: string){
    const url = localStorage.getItem('ApiEndPoint') + '/api/options/searchOptions';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { CategoryName: categoryName, OptionName: optionName}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  getListServiceType(): Observable<GetListServiceTypeResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/getListServiceType';
    return this.httpClient.get(url).pipe(
      map((response: GetListServiceTypeResult) => {
        return <GetListServiceTypeResult>response;
      }));
  }
  getOptionById(id: string): Observable<OptionsEntityTable> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/options/getOptionById';
    return this.httpClient.post(url, {
      id: id
    }).pipe(
      map((response: OptionsEntityTable) => {
        return <OptionsEntityTable>response;
      }));
  }
  getOptionCategory(): Observable<OptionCategoryTable> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/options/getOptionCategory';
    return this.httpClient.get(url).pipe(
      map((response: OptionCategoryTable) => {
        return <OptionCategoryTable>response;
      }));
  }
  createOrUpdateOptions(optionsEntityModel: OptionsEntityModel): Observable<CreateOrUpdateOptionsResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/options/createOrUpdateOptions';
    return this.httpClient.post(url, {
      OptionsEntityModel: optionsEntityModel
    }).pipe(
      map((response: CreateOrUpdateOptionsResult) => {
        return <CreateOrUpdateOptionsResult>response;
      }));
  }
  deleteOption(id: string): Observable<CreateOrUpdateOptionsResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/options/deleteOption';
    return this.httpClient.post(url, {
      Id: id
    }).pipe(
      map((response: CreateOrUpdateOptionsResult) => {
        return <CreateOrUpdateOptionsResult>response;
      }));
  }
  getAllOptionTree(OptionId: string): Observable<SearchOptionTreeResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/options/searchOptionsTree';
    return this.httpClient.post(url, {
      OptionId: OptionId
    }).pipe(
      map((response: SearchOptionTreeResult) => {
        return <SearchOptionTreeResult>response;
      }));
  }
}
