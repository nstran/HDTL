import { Component, Injector, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ChangePasswordModel, ForgotPassModel } from '../../models/forgotPass.model';
import { ForgotPassCheckService } from './services/forgot-pass-check.service';
import { tap } from 'rxjs/operators';
import { AbstractBase } from '../../../shared/abstract-base.component';

@Component({
  selector: 'app-forgot-pass-check',
  templateUrl: './forgot-pass-check.component.html',
  styleUrls: ['./forgot-pass-check.component.css']
})
export class ForgotPassCheckComponent extends AbstractBase implements OnInit {
  model: ForgotPassModel = {
    UserId: '',
    UserName: '',
    FullName: '',
    EmailAddress: '',
    Password: '',
    Re_password: ''
  };
  loading = false;
  state: number = 1;
  success: boolean = false;
  forgotPassMessageCode: string = '';
  capcharLogo: string;
  capcharInput: string = '';
  capcharCheck: string = '';
  isCheckCaptcha: boolean = false;
  validateCaptcha: boolean = false;
  changePasswordModel = new ChangePasswordModel();

  constructor(
    injector : Injector,
    private route: ActivatedRoute,
    private router: Router,
    private translate: TranslateService,
    private forgotPassCheckService: ForgotPassCheckService
  ) {
    super(injector);
    translate.setDefaultLang('vi');
  }

  ngOnInit() {
    this.capcharText();
  }

  capcharText() {
    if (this.capcharCheck == null || this.capcharCheck === '' || this.capcharCheck === undefined) this.capcharCheck = '1234567890';
    var random = this.capcharCheck[Math.floor(Math.random() * this.capcharCheck.length)];
    for (let i = 0; i < 3; i++) {
      random = random + this.capcharCheck[Math.floor(Math.random() * this.capcharCheck.length)];
    }
    this.capcharLogo = random;
  }

  resolved(captchaResponse: string) {
    if (captchaResponse != null) {
      this.isCheckCaptcha = true;
      this.validateCaptcha = false;
    }
    else {
      this.isCheckCaptcha = false;
      this.validateCaptcha = true;
    }

  }

  re_send() {
    this.loading = true;
    this.send()
      .then((result) => {
        this.loading = false;
      })
      .catch((error) => {
        this.loading = false;
        this.forgotPassMessageCode = error.messageCode;
      })
  }

  send() {

    if (this.isCheckCaptcha) {
      this.validateCaptcha = true;
      this.loading = false;
      return;
    }

    return new Promise((resolve, reject) => {
      this.forgotPassCheckService.send_email(this.model)
        .subscribe(res => {
          let result = <any>res;
          if (result && result.statusCode === 200) {
            resolve(result);
            this.changePasswordModel.userName = result.userName;
            this.success = true;
          }
          else {
            reject(result);
            this.success = false;
          }
        })
    })
  }

  onKey(event: any) {
    this.state = 1;
  }

  redirectLogin() {
    this.router.navigate(['login']);
  }

  changePassword(): void {
    this.loading = true;
    this.forgotPassCheckService.changePasswordForgot(this.changePasswordModel.code, this.changePasswordModel.userName, this.changePasswordModel.newPassword, this.changePasswordModel.confirmPassword)
    .pipe(tap(() => {this.loading = false;}))
    .subscribe(result => {
      if (result && result.statusCode === 200) {
        let mgs = { severity: 'success', summary: 'Thông báo', detail: result.message };
        this.showMessage(mgs);
        this.router.navigate(['login']);
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo', detail: result.message };
        this.showMessage(mgs);
      }
    })
  }

}
