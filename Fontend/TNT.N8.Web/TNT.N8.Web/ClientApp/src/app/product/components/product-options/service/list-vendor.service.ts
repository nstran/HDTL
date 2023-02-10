import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { VendorListModel, VendorModel } from '../model/list-vendor';

@Injectable({
  providedIn: 'root'
})
export class ListVendorService {

  constructor(private httpClient: HttpClient) { }
  getAllVendorToPay() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getAllVendor';
    return this.httpClient.post(url, {}).pipe(
      map((response: VendorModel) => {
        return <VendorModel>response;
      }));
  }
  searchVendorAsync(vendorName: string, vendorCode: string, vendorGroupIdList: Array<string>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/searchVendor';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { VendorName: vendorName, VendorCode: vendorCode, VendorGroupIdList: vendorGroupIdList, UserId: userId }).toPromise()
        .then((response: VendorListModel) => {
          resolve(response);
        });
    });
  }
  getDataSearchVendor(useId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getDataSearchVendor';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: useId
      }).toPromise()
        .then((response: VendorListModel) => {
          resolve(response);
        });
    });
  }


  getMasterDataAddVendorToOption(optionId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/options/getMasterDataAddVendorToOption';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        OptionId: optionId,
      }).toPromise()
        .then((response) => {
          resolve(response);
        });
    });
  }

  addVendorToOption(vendorId: string, optionId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/options/addVendorToOption';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        VendorId: vendorId,
        OptionId: optionId,
      }).toPromise()
        .then((response) => {
          resolve(response);
        });
    });
  }

  
  deleteVendorMappingOption(vendorId: string, optionId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/options/deleteVendorMappingOption';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        VendorId: vendorId,
        OptionId : optionId,
      }).toPromise()
        .then((response) => {
          resolve(response);
        });
    });
  }

  quickCreateVendor(
    VendorGroup, VendorName,
    MST, Phone,
    Email, Website,
    Address, Description,

    Bank, Account,
    AccountName, Branch,

    vendorId, optionId,
    
    ) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/options/quickCreateVendor';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        VendorId: vendorId,
        OptionId : optionId,
      }).toPromise()
        .then((response) => {
          resolve(response);
        });
    });
  }

}
