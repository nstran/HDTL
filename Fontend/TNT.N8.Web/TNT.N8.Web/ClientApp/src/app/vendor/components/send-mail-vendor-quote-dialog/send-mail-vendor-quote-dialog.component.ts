import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

//MODELS
import { QueueModel } from '../../models/queue.model';

//SERVICES
import { CompanyService } from '../../../shared/services/company.service';
import { VendorService } from '../../services/vendor.service';
import { getuid } from 'process';

//PRIMENG
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { TemplateEmailPopupComponent } from '../../../customer/components/template-email-popup/template-email-popup.component';

@Component({
  selector: 'app-send-mail-vendor-quote-dialog',
  templateUrl: './send-mail-vendor-quote-dialog.component.html',
  styleUrls: ['./send-mail-vendor-quote-dialog.component.css'],
  providers: [DialogService]
})
export class SendMailVendorQuoteDialogComponent implements OnInit {
  /*Get Global Parameter*/
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  /*End*/

  auth: any = JSON.parse(localStorage.getItem('auth'));
  fromTo: string = localStorage.getItem('UserEmail');
  userId = this.auth.UserId;
  loading: boolean = false;
  awaitResult: boolean = false;
  base64Pdf: any;
  emptyId = "00000000-0000-0000-0000-000000000000";
  emailPattern = '(([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?)([; ]?)){1,64}';


  queueModel: QueueModel = new QueueModel();
  sendTo: string = '';
  vendorId: string = this.emptyId;
  vendorCode: string = '';
  vendorName: string = '';
  suggestedSupplierQuoteId: string = this.emptyId;

  name: string = "{{name}}";
  hotline: string = "{{hotline}}";
  address: string = "{{address}}";

  /*Form send email*/
  formQuickEmail: FormGroup;

  emailControl: FormControl;
  ccEmailControl: FormControl;
  bccEmailControl: FormControl;
  titleControl: FormControl;
  contentControl: FormControl;
  /*End*/

  /**Gửi Mail Attchement */
  uploadedFiles: any[] = [];
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  attchementName: Array<any> = [];
  file: File[];
  /**END */

  @ViewChild('autofocus', { static: true }) private autofocus: ElementRef;

  tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink', 'Image', '|', 'ClearFormat', 'Print',
      'SourceCode', 'FullScreen', '|', 'Undo', 'Redo']
  };

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private companyService: CompanyService,
    private vendorService: VendorService,
    private messageService: MessageService,
    private dialogService: DialogService,
  ) {
    this.sendTo = this.config.data.sendTo;
    this.vendorId = this.config.data.vendorId;
    this.vendorCode = this.config.data.vendorCode;
    this.vendorName = this.config.data.vendorName;
    this.base64Pdf = this.config.data.base64;
    this.suggestedSupplierQuoteId = this.config.data.suggestedSupplierQuoteId;
  }


  ngOnInit() {

    this.setFrom();
    this.emailControl.setValue(this.sendTo);
    if (this.vendorName !== null && this.vendorName.trim() !== "") {
      this.titleControl.setValue('[' + this.vendorName + ']' + " - Đề nghị báo giá");
    }
    else {
      this.titleControl.setValue("Đề nghị báo giá");
    }
    // this.companyService.getCompanyConfig().subscribe(response => {
    //   let result = <any>response;
    //   if (result.statusCode === 202 || result.statusCode === 200) {
    //     let strContent = `<p>Kính gửi quý khách hàng,</p><p><br></p><p>Qua tìm hiểu, công ty chúng tôi biết Quý công ty đang cung cấp một số sản phẩm, dịch vụ mà 
    //     chúng tôi đang cần. Vì vậy, nhờ quý công ty báo giá cho chúng tôi về giá của một số sản phẩm như file đính kèm.</p><p><br></p><p>Xin trân trọng cảm ơn!</p>`
    //     this.contentControl.setValue(strContent);
    //   }
    // }, error => { });
  }

  sendEmail() {
    if (!this.formQuickEmail.valid) {
      Object.keys(this.formQuickEmail.controls).forEach(key => {
        if (!this.formQuickEmail.controls[key].valid) {
          this.formQuickEmail.controls[key].markAsTouched();
        }
      });
    } else {
      var listEmail = [];
      var emailErr = "";

      var lstMail = this.emailControl.value.trim().split(";");
      lstMail.forEach(item => {
        if (item.trim() !== "") {
          let isPatterm = /([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]+([" +"]?){2,64}/.test(item.trim());
          if (isPatterm) {
            listEmail.push(item.trim());
          }
          else {
            emailErr = emailErr === "" ? item.trim() : (emailErr + ", " + item.trim());
          }
        }
      })

      var listEmailCC = [];
      var lstTmpMailCC = this.ccEmailControl.value != null ? this.ccEmailControl.value.split(";") : [];
      lstTmpMailCC.forEach(item => {
        if (item.trim() !== "") {
          let isPatterm = /([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]+([" +"]?){2,64}/.test(item.trim());
          if (isPatterm) {
            listEmailCC.push(item.trim());
          }
        }
      })

      var listEmailBcc = [];
      var lstTmpMailBcc = this.bccEmailControl.value != null ? this.bccEmailControl.value.split(";") : [];
      lstTmpMailBcc.forEach(item => {
        if (item.trim() !== "") {
          let isPatterm = /([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]+([" +"]?){2,64}/.test(item.trim());
          if (isPatterm) {
            listEmailBcc.push(item.trim());
          }
          else {
            emailErr = emailErr === "" ? item.trim() : (emailErr + ", " + item.trim());
          }
        }
      })


      if (emailErr !== "") {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Email: " + emailErr + " sai định dạng." };
        this.showMessage(msg);
      }



      if (listEmail.length !== 0) {

        this.file = new Array<File>();
        this.uploadedFiles.forEach(item => {
          this.file.push(item);
        });

        // this.vendorService.sendEmailVendorQuote(listEmail, listEmailCC, listEmailBcc, this.titleControl.value.trim(), this.contentControl.value, this.suggestedSupplierQuoteId, this.base64Pdf).subscribe(response => {
        //   let result = <any>response;
        //   if (result.statusCode == 200) {
        //     let msg = { key: 'popup', severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        //     this.showMessage(msg);
        //     if (emailErr == "") {
        //       this.ref.close(result);
        //     }
        //   }
        //   else {
        //     let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Email sai định dạng." };
        //     this.showMessage(msg);
        //   }
        // });
        this.loading = true;
        this.vendorService.sendEmailVendorQuote(listEmail, listEmailCC, listEmailBcc, this.titleControl.value.trim(), this.contentControl.value, this.suggestedSupplierQuoteId, this.file, this.userId).subscribe(response => {
          let result = <any>response;
          this.loading = false;
          if (result.statusCode == 200) {
            let msg = { key: 'popup', severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
            if (emailErr == "") {
              this.ref.close(result);
            }
          }
          else {
            let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    }
  }

  setFrom() {
    this.emailControl = new FormControl(null, [Validators.required, Validators.pattern(this.emailPattern)]);
    this.ccEmailControl = new FormControl(null, [Validators.pattern(this.emailPattern)]);
    this.bccEmailControl = new FormControl(null, [Validators.pattern(this.emailPattern)]);
    this.titleControl = new FormControl('', [Validators.required, Validators.maxLength(200), forbiddenSpaceText]);
    this.contentControl = new FormControl('', [Validators.required]);

    this.formQuickEmail = new FormGroup({
      emailControl: this.emailControl,
      ccEmailControl: this.ccEmailControl,
      bccEmailControl: this.bccEmailControl,
      titleControl: this.titleControl,
      contentControl: this.contentControl
    });
  }

  ngAfterViewInit() {
    this.autofocus.nativeElement.focus();
  }
  trimspace() {
    var trim = this.emailControl.value.trim();
    this.emailControl.setValue(trim);
  }
  cancel() {
    this.ref.close();
  }

  replate_token(token: string) {
    let newContent = (this.contentControl.value != null ? this.contentControl.value : "") + token;
    this.contentControl.setValue(newContent);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  /*Event thay đổi nội dung ghi chú*/
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  /*Event Lưu các file được chọn*/
  handleFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;
      if (size <= this.defaultLimitedFileSize) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedFiles.push(file);
          }
        }
      }
    }
  }

  /*Event Khi click xóa từng file*/
  removeFile(event) {
    let index = this.uploadedFiles.indexOf(event.file);
    this.uploadedFiles.splice(index, 1);
  }

  /*Event Khi click xóa toàn bộ file*/
  clearAllFile() {
    this.uploadedFiles = [];
  }

  selectEmailTemplate() {
    let ref = this.dialogService.open(TemplateEmailPopupComponent, {
      data: {},
      header: 'Danh sách mẫu email',
      width: '1200px',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px",
        "max-height": "1500px",
        'overflow-x': 'hidden'
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        this.contentControl.setValue(result);
      }
    });
  }
}

function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
      }
    }
  }
  return null;
}
function formatNumber(num) {
  return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
}