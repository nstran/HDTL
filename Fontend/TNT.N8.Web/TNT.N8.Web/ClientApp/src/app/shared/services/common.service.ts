
import { map } from 'rxjs/operators';
import { Pipe, Injectable, EventEmitter, Output, Directive } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Pipe({ name: 'CommonService' })
@Injectable()
export class CommonService {
  constructor(private http: HttpClient) { }

  testToken(): Observable<any> {
    let currentUser = <any>localStorage.getItem('auth');
    let headers = new HttpHeaders({
      'Authorization': 'Bearer ' + JSON.parse(currentUser).token
    });

    return this.http.post('http://localhost:5001/api/auth/testToken', { UserId: JSON.parse(currentUser).UserId }, { headers: headers }).pipe(
      map((response: Response) => {
        var result = <any>response.json();
        return result;
      }));
  }

  getApiEndPoint(): any {
    let url = '/api/shared/getkey';
    return this.http.get(url + '?key=API_ENDPOINT').pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getVersion(): any {
    let url = '/api/shared/getkey';
    return this.http.get(url + '?key=VERSION').pipe(
      map((response: Response) => {
        return response;
      }));
  }

  @Output() change: EventEmitter<string> = new EventEmitter();
  click(value: string) {
    this.change.emit(value);
  }

  convertDateToTimeSpan(date: Date) {
    if (!date) return null;
    else {
      let hour = date.getHours();
      let minute = date.getMinutes();

      return hour.toString() + ":" + minute.toString();
    }
  }

  convertTimeSpanToDate(timeSpan: string) {
    if (!timeSpan) return null;
    else {
      let listTime = timeSpan.split(':');

      let now = new Date();

      if (listTime.length == 2) {
        now.setHours(parseInt(listTime[0]));
        now.setMinutes(parseInt(listTime[1]));
      }
      else if (listTime.length == 3) {
        now.setHours(parseInt(listTime[0]));
        now.setMinutes(parseInt(listTime[1]));
        now.setSeconds(parseInt(listTime[2]));
      }

      return now;
    }
  }

  convertStringToNumber(str: string) {
    if (!str || str.trim() == '') return null;
    else {
      return parseFloat(str.replace(/,/g, ''));
    }
  }

  convertStringToDate(dateString: string, format: string) {
    if (!dateString || dateString.trim() == '') {
      return null;
    }

    let now = new Date();

    if (format == 'dd/MM/yyyy') {
      let listTime = dateString.split('/');

      now.setDate(parseInt(listTime[0]));
      now.setMonth(parseInt(listTime[1]) - 1);
      now.setFullYear(parseInt(listTime[2]));
    }
    else if (format == 'MM/dd/yyyy') {
      let listTime = dateString.split('/');

      now.setDate(parseInt(listTime[1]));
      now.setMonth(parseInt(listTime[0]) - 1);
      now.setFullYear(parseInt(listTime[2]));
    }

    return new Date(
      Date.UTC(
        now.getFullYear(),
        now.getMonth(),
        now.getDate(),
        now.getHours(),
        now.getMinutes(),
        now.getSeconds()
      )
    );
  }

}
