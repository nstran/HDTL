import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { TakeRatingStatistictDashboardResult, TakeRevenueStatisticDashboardResult, TakeRevenueStatisticEmployeeDashboardResult, TakeRevenueStatisticServicePacketDashboardResult, TakeRevenueStatisticWaitPaymentDashboardResult, TakeStatisticServiceTicketDashboardResult } from "./dashboard.model";

@Injectable()
export class DashBoardService {
    constructor(private httpClient: HttpClient) { }

    takeRevenueStatisticDashboard(): Observable<TakeRevenueStatisticDashboardResult> {
        const url = localStorage.getItem('ApiEndPoint') + '/api/Dashboard/takeRevenueStatisticDashboard';
        return this.httpClient.post(url, {}).pipe(
            map((response: TakeRevenueStatisticDashboardResult) => {
                return <TakeRevenueStatisticDashboardResult>response;
            }));
    }

    takeRevenueStatisticWaitPaymentDashboard(): Observable<TakeRevenueStatisticWaitPaymentDashboardResult> {
        const url = localStorage.getItem('ApiEndPoint') + '/api/Dashboard/takeRevenueStatisticWaitPaymentDashboard';
        return this.httpClient.post(url, {}).pipe(
            map((response: TakeRevenueStatisticWaitPaymentDashboardResult) => {
                return <TakeRevenueStatisticWaitPaymentDashboardResult>response;
            }));
    }

    takeStatisticServiceTicketDashboard(): Observable<TakeStatisticServiceTicketDashboardResult> {
        const url = localStorage.getItem('ApiEndPoint') + '/api/Dashboard/takeStatisticServiceTicketDashboard';
        return this.httpClient.post(url, {}).pipe(
            map((response: TakeStatisticServiceTicketDashboardResult) => {
                return <TakeStatisticServiceTicketDashboardResult>response;
            }));
    }

    takeRevenueStatisticEmployeeDashboard(startDate: Date, endDate: Date, count?: number): Observable<TakeRevenueStatisticEmployeeDashboardResult> {
        const url = localStorage.getItem('ApiEndPoint') + '/api/Dashboard/takeRevenueStatisticEmployeeDashboard';
        return this.httpClient.post(url, {StartDate : startDate, EndDate : endDate, Count: count}).pipe(
            map((response: TakeRevenueStatisticEmployeeDashboardResult) => {
                return <TakeRevenueStatisticEmployeeDashboardResult>response;
            }));
    }

    takeRevenueStatisticServicePacketDashboard(startDate: Date, endDate: Date, count?: number): Observable<TakeRevenueStatisticServicePacketDashboardResult> {
        const url = localStorage.getItem('ApiEndPoint') + '/api/Dashboard/takeRevenueStatisticServicePacketDashboard';
        return this.httpClient.post(url, {StartDate : startDate, EndDate : endDate, Count: count}).pipe(
            map((response: TakeRevenueStatisticServicePacketDashboardResult) => {
                return <TakeRevenueStatisticServicePacketDashboardResult>response;
            }));
    }

    takeRatingStatisticDashboard(startDate: Date, endDate: Date, count?: number): Observable<TakeRatingStatistictDashboardResult> {
        const url = localStorage.getItem('ApiEndPoint') + '/api/Dashboard/takeRatingStatisticDashboard';
        return this.httpClient.post(url, {StartDate : startDate, EndDate : endDate, Count: count}).pipe(
            map((response: TakeRatingStatistictDashboardResult) => {
                return <TakeRatingStatistictDashboardResult>response;
            }));
    }
}