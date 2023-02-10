import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { EmployeeService } from '../../../../app/employee/services/employee.service';
import { TemplateEmailPopupComponent } from '../template-email-popup/template-email-popup.component'

interface ResultDialog {
  status: boolean
  message: string
  listInvalidEmail: Array<string>
  screenType: string
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
class emailPVModel {
  listCandidate: Array<candidateEmailPVModel> = []; //  email Ứng viên
  vancaciesName: string; // Vị trí tuyển dụng
  personInChagerName: string; // tên người phụ trách
  personInChagerPhone: string; // SĐT người phụ trách
  workplace: string;
  subject: string;
  sendContent: string;
}

class candidateEmailPVModel {
  email: string;
  fullName: string;
  interviewTime: Date;
  addressOrLink: string;
  interviewScheduleType: number;
  listInterviewerEmail: Array<string> = []; //danh sách email Người PV
}

@Component({
  selector: 'app-template-vacancies-email',
  templateUrl: './template-vacancies-email.component.html',
  styleUrls: ['./template-vacancies-email.component.css'],
  providers: [DialogService]
})
export class TemplateVacanciesEmailComponent implements AfterViewInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  fromTo: string = localStorage.getItem('UserEmail');
  userId = this.auth.UserId;
  loading: boolean = false;
  awaitResult: boolean = false;
  disableEmail: boolean = true;
  showdialog: boolean = false;
  file: File[];
  name: string = "{{name}}";
  hotline: string = "{{hotline}}";
  address: string = "{{address}}";

  /*Form send quick email*/
  formVacanciesEmail: FormGroup;
  emailControl: FormControl;
  ccEmailControl: FormControl;
  bccEmailControl: FormControl;
  titleControl: FormControl;
  contentControl: FormControl;
  /*End*/

  selectedColumns: any = [];
  listEmailTemplateName: Array<any> = [];

  @ViewChild('autofocus', { static: true }) private autofocus: ElementRef;

  /*Get Global Parameter*/
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  /*End*/

  /**Gửi Mail Attchement */
  uploadedFiles: any[] = [];
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  attchementName: Array<any> = [];
  /**END */

  emailData = new emailPVModel()
  emailType: string = '';
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
  screenType: string;
  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    private dialogService: DialogService,
  ) {
    this.disableEmail = this.config.data.disableEmail;
    this.emailData = this.config.data.emailData;
    this.emailType = this.config.data.emailType;
    this.screenType = this.config.data.screenType;
  }

  ngOnInit() {
    let emailPattern = '(([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?)([; ]?)){1,64}';
    // Hẹn phỏng vấn

    // danh sách email người nhận
    let emailStr = ''
    this.emailData.listCandidate.forEach(interview => {
      emailStr = emailStr + interview.email + ','
    });
    if (emailStr.length > 0) {
      emailStr = emailStr.slice(0, -1);
    }

    this.emailControl = new FormControl({ value: emailStr, disabled: true }, [Validators.required, Validators.pattern(emailPattern)]);

    let title = (this.emailType == "PHONG_VAN" ? "GREENKA - THƯ MỜI THAM DỰ PHỎNG VẤN " : "GREENKA - THÔNG BÁO KẾT QUẢ PHỎNG VẤN VỊ TRÍ ") + this.emailData.vancaciesName.toLocaleUpperCase();

    this.titleControl = new FormControl(title, [Validators.required, forbiddenSpaceText]);
    this.contentControl = new FormControl('', [Validators.required]);

    this.formVacanciesEmail = new FormGroup({
      emailControl: this.emailControl,
      titleControl: this.titleControl,
      contentControl: this.contentControl
    });
  }

  ngAfterViewInit() {
    //this.autofocus.nativeElement.focus();
  }

  async sendEmail() {
    if (!this.formVacanciesEmail.valid) {
      Object.keys(this.formVacanciesEmail.controls).forEach(key => {
        if (!this.formVacanciesEmail.controls[key].valid) {
          this.formVacanciesEmail.controls[key].markAsTouched();
        }
      });
    } else if (this.currentTextChange.trim() == "") {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Nội dung email không được để trống" };
      this.showMessage(msg);
    } else {

      let sendTo = this.emailControl.value == null ? "" : this.emailControl.value.trim();
      // let sendToCC = this.ccEmailControl.value == null ? "" : this.ccEmailControl.value.trim();
      // let sendToBCC = this.bccEmailControl.value == null ? "" : this.bccEmailControl.value.trim();

      if (!sendTo) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Chưa có email nhận. Đề nghị bổ sung trước khi gửi" };
        this.showMessage(msg);
      } else if (sendTo.trim() == "") {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Chưa có email nhận. Đề nghị bổ sung trước khi gửi" };
        this.showMessage(msg);
      } else {
        this.awaitResult = true;

        // let listFileUploadModel: Array<FileUploadModel> = [];
        // this.uploadedFiles.forEach(item => {
        //   let fileUpload: FileUploadModel = new FileUploadModel();
        //   fileUpload.FileInFolder = new FileInFolder();
        //   fileUpload.FileInFolder.active = true;
        //   let index = item.name.lastIndexOf(".");
        //   let name = item.name.substring(0, index);
        //   fileUpload.FileInFolder.fileName = name;
        //   fileUpload.FileInFolder.fileExtension = item.name.substring(index, item.name.length - index);
        //   fileUpload.FileInFolder.size = item.size;
        //   fileUpload.FileInFolder.objectType = 'CANDIDATE';
        //   fileUpload.FileSave = item;
        //   listFileUploadModel.push(fileUpload);
        // });

        this.file = new Array<File>();
        this.uploadedFiles.forEach(item => {
          this.file.push(item);
        });

        this.uploadedFiles = [];
        this.loading = true;

        // Content của email
        this.emailData.sendContent = this.contentControl.value;

        // Title mới của email
        this.emailData.subject = this.titleControl.value;

        this.employeeService.sendEmailInterview(this.emailData, this.file, 'CANDIDATE').subscribe(response => {
          let result = <any>response;
          this.loading = false;

          if (result.statusCode == 200) {
            let resultDialog: ResultDialog = {
              status: result.statusCode,
              message: result.messageCode,
              listInvalidEmail: result.listInvalidEmail,
              screenType: this.screenType
            };

            this.ref.close(resultDialog);
          } else {
            this.awaitResult = false;
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
    this.currentTextChange = this.contentControl.value;
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
