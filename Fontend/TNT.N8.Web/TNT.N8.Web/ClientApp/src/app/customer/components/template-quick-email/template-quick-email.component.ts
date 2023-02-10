import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

//MODELS
import { QueueModel } from '../../models/queue.model';

//SERVICES
import { CustomerCareService } from '../../services/customer-care.service';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { ForderConfigurationService } from '../../../admin/components/folder-configuration/services/folder-configuration.service';
import { EmailConfigService } from '../../../admin/services/email-config.service';
import { TemplateEmailPopupComponent } from '../template-email-popup/template-email-popup.component';


//COMPONENT
// import { TemplateEmailDialogComponent } from '../template-email-dialog/template-email-dialog.component';
// import { TemplateSmsDialogComponent } from '../template-sms-dialog/template-sms-dialog.component';

interface ResultDialog {
  status: boolean
  message: string
  listInvalidEmail: Array<string>
}

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
  createdByName: string;
  createdDate: Date;
  dumpId: any;
  typeDocument: string; //"DOC": tài liệu; "LINK": liên kết
  linkName: string;
  linkValue: string;
  name: string;
  isNewLink: boolean; // phân biệt link mới hoặc link cũ
  linkOfDocumentId: string;
}

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

@Component({
  selector: 'app-template-quick-email',
  templateUrl: './template-quick-email.component.html',
  styleUrls: ['./template-quick-email.component.css'],
  providers: [DialogService]
})
export class TemplateQuickEmailComponent implements AfterViewInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  fromTo: string = localStorage.getItem('UserEmail');
  userId = this.auth.UserId;
  loading: boolean = false;
  awaitResult: boolean = false;
  disableEmail: boolean = true;
  showdialog: boolean = false;

  queueModel: QueueModel = new QueueModel();
  sendTo: string = '';
  customerId: string = '';
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

  selectedColumns: any = [];
  listEmailTemplateName: Array<any> = [];

  @ViewChild('autofocus', { static: true }) private autofocus: ElementRef;

  //dialogSelectTemplateEmail: MatDialogRef<TemplateEmailDialogComponent>;
  //dialogSelectTemplateSMS: MatDialogRef<TemplateSmsDialogComponent>;

  /*Get Global Parameter*/
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  /*End*/

  /**Gửi Mail Attchement */
  uploadedFiles: any[] = [];
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  attchementName: Array<any> = [];
  /**END */

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
    private messageService: MessageService,
    private customerCareService: CustomerCareService,
    private dialogService: DialogService,
    private emailConfigService: EmailConfigService,
    private folderService: ForderConfigurationService,
  ) {
    this.sendTo = this.config.data.sendTo;
    this.disableEmail = this.config.data.disableEmail;
    this.customerId = this.config.data.customerId;
    if (this.config.data.isCreateBylead) {
      this.isCreateBylead = true;
    }
  }

  ngOnInit() {

    // var emailPattern = '/([A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}(;?\s?))/';
    let emailPattern = '(([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?)([; ]?)){1,64}';

    this.emailControl = new FormControl({ value: this.sendTo, disabled: this.disableEmail }, [Validators.required, Validators.pattern(emailPattern)]);
    this.ccEmailControl = new FormControl({ value: '', disabled: this.disableEmail }, [Validators.pattern(emailPattern)]);
    this.bccEmailControl = new FormControl({ value: '', disabled: this.disableEmail }, [Validators.pattern(emailPattern)]);
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
    //this.autofocus.nativeElement.focus();
  }

  async sendEmail() {
    if (!this.formQuickEmail.valid) {
      Object.keys(this.formQuickEmail.controls).forEach(key => {
        if (!this.formQuickEmail.controls[key].valid) {
          this.formQuickEmail.controls[key].markAsTouched();
        }
      });
    } else if (!this.contentControl.value || this.contentControl.value == "") {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Nội dung email không được để trống" };
      this.showMessage(msg);
    } else {

      let sendTo = this.emailControl.value == null ? "" : this.emailControl.value.trim();
      let sendToCC = this.ccEmailControl.value == null ? "" : this.ccEmailControl.value.trim();
      let sendToBCC = this.bccEmailControl.value == null ? "" : this.bccEmailControl.value.trim();

      if (!sendTo) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Chưa có email nhận. Đề nghị bổ sung trước khi gửi" };
        this.showMessage(msg);
      } else if (sendTo.trim() == "") {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Chưa có email nhận. Đề nghị bổ sung trước khi gửi" };
        this.showMessage(msg);
      } else {
        this.queueModel.FromTo = this.fromTo != null ? this.fromTo.trim() : "";
        this.queueModel.SendTo = sendTo;
        this.queueModel.SendToCC = sendToCC;
        this.queueModel.SendToBCC = sendToBCC;
        this.queueModel.CustomerId = this.customerId;
        this.queueModel.Title = this.titleControl.value.trim();
        this.queueModel.SendContent = this.contentControl.value;
        this.queueModel.Method = "Email";
        this.queueModel.CreateById = this.userId;

        this.awaitResult = true;

        // if (this.isCreateBylead == true) {
        //   this.customerCareService.sendQuickLeadEmail(this.queueModel).subscribe(response => {
        //     let result = <any>response;
        //     if (result.statusCode == 200) {
        //       let resultDialog: ResultDialog = {
        //         status: true
        //       };

        //       this.ref.close(resultDialog);
        //     } else {
        //       this.awaitResult = false;
        //       let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: result.messageCode};
        //       this.showMessage(msg);
        //     }
        //   });
        // } else {

        // if (this.uploadedFiles.length == 0) return;
        let listFileUploadModel: Array<FileUploadModel> = [];
        this.uploadedFiles.forEach(item => {
          let fileUpload: FileUploadModel = new FileUploadModel();
          fileUpload.FileInFolder = new FileInFolder();
          fileUpload.FileInFolder.active = true;
          let index = item.name.lastIndexOf(".");
          let name = item.name.substring(0, index);
          fileUpload.FileInFolder.fileName = name;
          fileUpload.FileInFolder.fileExtension = item.name.substring(index, item.name.length - index);
          fileUpload.FileInFolder.size = item.size;
          fileUpload.FileInFolder.objectId = this.queueModel.QueueId;
          fileUpload.FileInFolder.objectType = 'QLKHCRM';
          fileUpload.FileSave = item;
          listFileUploadModel.push(fileUpload);
        });

        this.uploadedFiles = [];

        this.loading = true;
        this.customerCareService.sendQuickEmail(this.queueModel, listFileUploadModel, 'QLKHCRM').subscribe(response => {
          let result = <any>response;
          this.loading = false;
          if (result.statusCode == 200) {

            let resultDialog: ResultDialog = {
              status: result.statusCode,
              message: result.messageCode,
              listInvalidEmail: result.listInvalidEmail,
            };
            this.ref.close(resultDialog);
          } else {
            this.awaitResult = false;
            let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
        // }

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
    this.currentTextChange = event.value;
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
        this.currentTextChange = result;
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
