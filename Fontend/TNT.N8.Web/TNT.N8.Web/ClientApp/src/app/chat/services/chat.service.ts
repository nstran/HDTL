import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { TakeDataFireBaseByUserIdResult, TakeListEmployeeByUserNameParameter } from '../models/chat.models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
    constructor(private httpClient: HttpClient) {
    }

    getDataFireBase() {
      let url = localStorage.getItem('ApiEndPoint') + "/api/FireBase/takeDataFireBaseByUserId";
      return this.httpClient.post(url, { }).pipe(
        map((response: TakeDataFireBaseByUserIdResult) => {
          return response;
        }));
    }

    createDataFireBase(roomName: string, otherId: string) {
      let url = localStorage.getItem('ApiEndPoint') + "/api/FireBase/createDataFireBase";
        return new Promise((resolve, reject) => {
          return this.httpClient.post(url, { RoomName: roomName, OtherId: otherId}).toPromise()
            .then((response) => {
                resolve(response);
            });
        });
    }

    getUserProfileByRoomName(listRoomName: string[]) {
      let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getUserProfileByRoomName';
      return new Promise((resolve, reject) => {
        return this.httpClient.post(url, {listRoomName : listRoomName}).toPromise()
            .then((response: Response) => {
                resolve(response);
            });
      });
    }

    takeListEmployeeByUsername(filterText : string): Observable<TakeListEmployeeByUserNameParameter> {
      let url = localStorage.getItem('ApiEndPoint') + '/api/employee/takeListEmployeeByUsername';
      return this.httpClient.post(url, {FilterText : filterText}).pipe(
        map((response: TakeListEmployeeByUserNameParameter) => {
          return response;
        }));
    }
}