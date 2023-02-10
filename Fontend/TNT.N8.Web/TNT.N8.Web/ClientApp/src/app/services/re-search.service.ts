import { map } from 'rxjs/operators';
import { Pipe, Injectable, EventEmitter, Output, Directive } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { ListOrderSearch } from '../shared/models/re-search/list-order-search.model';
import { ListCustomerSearch } from '../shared/models/re-search/list-customer-search.model';

@Injectable()
export class ReSearchService {
//   private listOrderSearch$ = new BehaviorSubject<ListOrderSearch>(new ListOrderSearch());
//   listOrderSearch = this.listOrderSearch$.asObservable();

  constructor() { }

  //Lưu localStore
  updatedSearchModel(key: string, data: any) {
    // this.listOrderSearch$.next(data);

    //Lưu localStorage
    localStorage.setItem(key, JSON.stringify(data));
  }

  //Reset bộ lọc về mặc định
  resetSearchModel(path: string) {
    switch(path) 
    {
      case '/order/list':
        this.updatedSearchModel('listOrderSearch', new ListOrderSearch());
      break;

      case '/customer/list':
        this.updatedSearchModel('listCustomerSearch', new ListCustomerSearch());
      break;

      default:
      break;
    }
  }
}