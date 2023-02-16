import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { ChangePasswordResult, ForgotPassModel } from '../../../models/forgotPass.model';
import { Observable } from 'rxjs';

@Injectable()
export class ForgotPassCheckService {
  constructor(private httpClient: HttpClient) { }

  check_user(user_name: ForgotPassModel) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getCheckUserName';
    return this.httpClient.post(url, { UserName: user_name.UserName }).pipe(
      map((response: Response) => {
        let result = <any>response;
        return result;
      }));
  }

  send_email(user_info: ForgotPassModel) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/email/sendEmailForgotPass';
    return this.httpClient.post(url, { EmailAddress: user_info.EmailAddress } ).pipe(
      map((response: Response) => {
        let result = <any>response;
        return result;
      }))
  }

  changePasswordForgot(code: string, userName: string, newPassword: string, confirmPassword: string): Observable<ChangePasswordResult> {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/changePasswordForgot';
    return this.httpClient.post(url, { Code: code, UserName: userName, NewPassword: newPassword, confirmPassword: confirmPassword } ).pipe(
      map((response: ChangePasswordResult) => {
        return <ChangePasswordResult>response;
      }));
  }
}
