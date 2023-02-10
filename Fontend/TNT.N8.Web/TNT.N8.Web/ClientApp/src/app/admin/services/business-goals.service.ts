import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/internal/operators/map';
import { BusinessGoals, BusinessGoalsDetail } from '../models/business-goals.model';


@Injectable({
    providedIn: 'root'
})
export class BusinessGoalsService {

    constructor(private httpClient: HttpClient) { }

    getMasterDataBusinessGoals(organizationId: string, year: string, type: boolean, userId: string) {
        const url = localStorage.getItem('ApiEndPoint') + '/api/billSale/getMasterDataBusinessGoals';
        return this.httpClient.post(url, {
            OrganizationId: organizationId,
            Year: year,
            Type: type,
            UserId: userId
        }).pipe(
            map((response: Response) => {
                return response;
            }));
    }


    createOrUpdateBusinessGoals(businessGoals: BusinessGoals, listBusinessGoalsDetailSales: Array<BusinessGoalsDetail>, listBusinessGoalsDetailRevenue: Array<BusinessGoalsDetail>, userId: string) {
        const url = localStorage.getItem('ApiEndPoint') + '/api/billSale/createOrUpdateBusinessGoals';
        return this.httpClient.post(url, {
            BusinessGoals: businessGoals,
            ListBusinessGoalsDetailSales: listBusinessGoalsDetailSales,
            ListBusinessGoalsDetailRevenue: listBusinessGoalsDetailRevenue,
            UserId: userId
        }).pipe(
            map((response: Response) => {
                return response;
            }));
    }
}
