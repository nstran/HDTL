import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { TakeDataFireBaseByUserIdResult } from '../models/fire-base.model';

@Injectable({
  providedIn: 'root'
})
export class FireBaseService {
    constructor(private httpClient: HttpClient) {
    }

    getDataFireBase() {
        let url = localStorage.getItem('ApiEndPoint') + "/api/FireBase/takeDataFireBaseByUserId";
        return this.httpClient.post(url, { }).pipe(
          map((response: TakeDataFireBaseByUserIdResult) => {
            return response;
          }));
    }

}