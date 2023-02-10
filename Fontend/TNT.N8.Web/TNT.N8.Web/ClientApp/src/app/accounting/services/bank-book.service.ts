
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class BankBookService {

  constructor(private httpClient: HttpClient) { }

  searchBankBook(userId: string, fromPaidDate: Date, toPaidDate: Date, bankAccountId: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/bankbook/searchBankBook';

    return this.httpClient.post(url, {
      UserId: userId, FromPaidDate: fromPaidDate, ToPaidDate: toPaidDate, BankAccountId: bankAccountId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
