import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { AdvertisementConfigurationEntityModel, CreateOrEditMobileAppConfigurationResult, MobileAppConfigurationEntityModel, PaymentMethodConfigure, TakeMobileAppConfigurationResult } from '../models/mobile-app-configuraton.models';

@Injectable()
export class MobileAppConfigurationService {
  constructor(private httpClient: HttpClient) { }

    createOrEditMobileAppConfiguration(mobileAppConfigurationEntityModel: MobileAppConfigurationEntityModel) {
        const url = localStorage.getItem('ApiEndPoint') + '/api/MobileAppConfiguration/createOrEditMobileAppConfiguration';
            return this.httpClient.post(url, { MobileAppConfigurationEntityModel : mobileAppConfigurationEntityModel })
            .pipe(
                map((response: CreateOrEditMobileAppConfigurationResult) => {
                    return response;
                })
            );
    }

    takeMobileAppConfiguration() {
        const url = localStorage.getItem('ApiEndPoint') + '/api/MobileAppConfiguration/takeMobileAppConfiguration';
            return this.httpClient.post(url, { })
            .pipe(
                map((response: TakeMobileAppConfigurationResult) => {
                    return response;
                })
            );
    }

    deletePaymentMethod(rowData: PaymentMethodConfigure) {
        const url = localStorage.getItem('ApiEndPoint') + '/api/MobileAppConfiguration/deletePaymentMethod';
            return this.httpClient.post(url, { Payment : rowData })
            .pipe(
                map((response: any) => {
                    return response;
                })
            );
    }
    
    createOrUpdatePaymentMethod(rowData: PaymentMethodConfigure) {
        const url = localStorage.getItem('ApiEndPoint') + '/api/MobileAppConfiguration/createOrUpdatePaymentMethod';
            return this.httpClient.post(url, { Payment : rowData })
            .pipe(
                map((response: any) => {
                    return response;
                })
            );
    }

    createOrEditAdvertisementConfiguration(advertisementConfigurationEntityModel: AdvertisementConfigurationEntityModel) {
        const url = localStorage.getItem('ApiEndPoint') + '/api/MobileAppConfiguration/createOrEditAdvertisementConfiguration';
            return this.httpClient.post(url, { AdvertisementConfigurationEntityModel : advertisementConfigurationEntityModel })
            .pipe(
                map((response: CreateOrEditMobileAppConfigurationResult) => {
                    return response;
                })
            );
    }

    deleteAdvertisementConfiguration(id : string) {
        const url = localStorage.getItem('ApiEndPoint') + '/api/MobileAppConfiguration/deleteAdvertisementConfiguration';
            return this.httpClient.post(url, { Id : id })
            .pipe(
                map((response: CreateOrEditMobileAppConfigurationResult) => {
                    return response;
                })
            );
    }
    
}