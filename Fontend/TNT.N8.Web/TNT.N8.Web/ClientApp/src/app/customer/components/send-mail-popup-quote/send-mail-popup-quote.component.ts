import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';

//MODELS
import { QueueModel } from '../../models/queue.model';

//SERVICES
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { CompanyService } from '../../../shared/services/company.service';
import { QuoteService } from '../../services/quote.service';
import { FileUpload } from 'primeng/fileupload';
import { TemplateEmailPopupComponent } from '../template-email-popup/template-email-popup.component';

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
  createdById: string;
  createdDate: Date;
}

@Component({
  selector: 'app-send-mail-popup-quote',
  templateUrl: './send-mail-popup-quote.component.html',
  styleUrls: ['./send-mail-popup-quote.component.css'],
  providers: [DialogService]
})
export class SendEmailQuoteComponent implements AfterViewInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  fromTo: string = localStorage.getItem('UserEmail');
  userId = this.auth.UserId;
  loading: boolean = false;
  awaitResult: boolean = false;
  base64Pdf: any;

  queueModel: QueueModel = new QueueModel();
  sendTo: string = '';
  quoteId: string = '';
  customerCompany: string = '';
  customerId: string = '';
  quoteCode: string = '';
  quoteMoney: number = 0;
  isCreateBylead: boolean = false;

  name: string = "{{name}}";
  hotline: string = "{{hotline}}";
  address: string = "{{address}}";

  /*Form send quick email*/
  formQuickEmail: FormGroup;

  emailControl: FormControl;
  ccEmailControl: FormControl;
  bccEmailControl: FormControl;
  titleControl: FormControl;
  contentControl: FormControl;
  /*End*/

  /*Get Global Parameter*/
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
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
    private quoteService: QuoteService,
    private messageService: MessageService,
    private dialogService: DialogService,
  ) {
    this.sendTo = this.config.data.sendTo;
    this.customerId = this.config.data.customerId;
    this.quoteId = this.config.data.quoteId;
    this.quoteMoney = this.config.data.quoteMoney;
    this.quoteCode = this.config.data.quoteCode;
    this.base64Pdf = this.config.data.base64;
    this.customerCompany = this.config.data.customerCompany;
    if (this.config.data.isCreateBylead) {
      this.isCreateBylead = true;
    }
  }

  ngOnInit() {
    let emailPattern = '(([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?)([; ]?)){1,64}';

    this.emailControl = new FormControl(null, [Validators.required, Validators.pattern(emailPattern)]);
    this.ccEmailControl = new FormControl(null, [Validators.pattern(emailPattern)]);
    this.bccEmailControl = new FormControl(null, [Validators.pattern(emailPattern)]);
    this.titleControl = new FormControl('', [Validators.required, Validators.maxLength(200), forbiddenSpaceText]);
    this.contentControl = new FormControl('', [Validators.required]);

    this.formQuickEmail = new FormGroup({
      emailControl: this.emailControl,
      ccEmailControl: this.ccEmailControl,
      bccEmailControl: this.bccEmailControl,
      titleControl: this.titleControl,
      contentControl: this.contentControl
    });
    this.emailControl.setValue(this.sendTo);
    if (this.customerCompany !== null && this.customerCompany.trim() !== "") {
      this.titleControl.setValue(this.customerCompany + " - Bảng báo giá sản phẩm/dịch vụ");
    }
    else {
      this.titleControl.setValue("Bảng báo giá sản phẩm/dịch vụ");
    }
    // this.companyService.getCompanyConfig().subscribe(response => {
    //   let result = <any>response;
    //   if (result.statusCode === 202 || result.statusCode === 200) {
    //     let strContent = `<p>Kính gửi quý khách hàng,</p><p><br></p><p>Đây là bản báo giá sản phẩm dịch vụ của chúng tôi ${this.quoteCode}, với số tiền ${formatNumber(this.quoteMoney)} VND, chi tiết xem file đính kèm.</p><p>Nếu quý khách hàng có bất kỳ thắc mắc nào xin hãy liên hệ lại với chúng tôi theo số điện thoại ${result.companyConfig.phone} .</p><p><br></p><p>Xin trân trọng cảm ơn!</p>`
    //     this.contentControl.setValue(strContent);
    //   }
    // }, error => { });
  }

  ngAfterViewInit() {
    this.autofocus.nativeElement.focus();
  }

  trimspace() {
    var trim = this.emailControl.value.trim();
    this.emailControl.setValue(trim);
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

      let ListEmailCC = [];
      let ListTmpEmailCC = this.ccEmailControl.value != null ? this.ccEmailControl.value.split(';') : [];
      ListTmpEmailCC.forEach(element => {
        if (element.trim() != "") {
          ListEmailCC.push(element.trim());
        }
      });

      let ListEmailBcc = [];
      let ListTmpEmailBcc = this.bccEmailControl.value != null ? this.bccEmailControl.value.split(';') : [];
      
      ListTmpEmailBcc.forEach(element => {
        if (element.trim() != "") {
          ListEmailBcc.push(element.trim());
        }
      });

      if (emailErr !== "") {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Email: " + emailErr + " sai định dạng." };
        this.showMessage(msg);
      }

      if (listEmail.length !== 0) {
        this.file = new Array<File>();
        this.uploadedFiles.forEach(item => {
          this.file.push(item);
        });

        this.loading = true;
        this.quoteService.sendEmailCustomerQuote(listEmail, ListEmailCC, ListEmailBcc, this.titleControl.value.trim(), this.contentControl.value, this.quoteId, this.file, this.userId).subscribe(response => {
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
