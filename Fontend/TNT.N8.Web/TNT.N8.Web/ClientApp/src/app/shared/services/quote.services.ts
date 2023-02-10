import {map} from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Pipe({ name: 'QuoteService' })

@Injectable()
export class QuoteService {

    constructor(private httpClient: HttpClient) { }

    getMasterDataCreateCost(userId) {
        const url = localStorage.getItem('ApiEndPoint') + "/api/quote/getMasterDataCreateCost";
        return this.httpClient.post(url, {UserId: userId}).pipe(
          map((response: Response) => {
            return response;
          }));
      }
      getVendorByCostId(costId:string,soLuong:number) {
        const url = localStorage.getItem('ApiEndPoint') + "/api/quote/getVendorByCostId";
        return this.httpClient.post(url, {
          CostId:costId,
          SoLuong:soLuong
        }).pipe(
          map((response: Response) => {
            return response;
          }));
      }
}