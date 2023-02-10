import { Component, OnInit } from '@angular/core';
import { Data } from '@angular/router';
import { MessageService } from 'primeng/api';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CustomerService } from '../../../customer/services/customer.service';
import { ImageUploadService } from '../../services/imageupload.service';
// import { ForderConfigurationService } from '../../../admin/components/folder-configuration/folder-configuration.component';

interface PreviewCustomerCare {
  effecttiveFromDate: Date,
  effecttiveToDate: Date,
  sendDate: Date,
  statusName: string,
  previewEmailTo: string,
  previewEmailCC: string,
  previewEmailBcc: string,
  previewEmailContent: string,
  previewEmailName: string,
  previewEmailTitle: string,
}

interface PreviewFileEmail {
  active: boolean
  createdById: string
  createdByName: string
  createdDate: Data
  fileExtension: string
  fileFullName: string
  fileInFolderId: string
  fileName: string
  fileUrl: string
  folderId: string
  objectId: string
  objectType: string
  size: string
  updatedById: string
  updatedDate: string
  uploadByName: string
  fileNameShow: string
}

@Component({
  selector: 'app-template-preview-email',
  templateUrl: './template-preview-email.component.html',
  styleUrls: ['./template-preview-email.component.css']
})
export class TemplatePreviewEmailComponent implements OnInit {

  previewCustomerCare: PreviewCustomerCare = {
    effecttiveFromDate: new Date(),
    effecttiveToDate: new Date(),
    sendDate: new Date(),
    statusName: '',
    previewEmailName: '',
    previewEmailTitle: '',
    previewEmailContent: '',
    previewEmailTo: '',
    previewEmailCC: '',
    previewEmailBcc: '',
  }
  customerCareId: string;
  customerId: string;

  /*Table*/
  previewFileCols: any;
  /*End*/

  listPreviewFileEmail: Array<PreviewFileEmail>

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
    private customerService: CustomerService,
    private imageService: ImageUploadService,
    // private folderService: ForderConfigurationService
  ) {
    this.customerId = this.config.data.customerId;
    this.customerCareId = this.config.data.customerCareId;
  }

  ngOnInit(): void {

    // preview File đính kèm email
    this.previewFileCols = [
      { field: 'fileNameShow', header: 'Tên file', width: '15%', textAlign: 'left' },
      { field: 'createdDate', header: 'Ngày gửi', width: '10%', textAlign: 'right' },
      { field: 'createdByName', header: 'Người gửi', width: '15%', textAlign: 'left' },
      { field: 'size', header: 'Kích thước', width: '7%', textAlign: 'right' },
      { field: 'action', header: 'Thao tác', width: '7%', textAlign: 'center' },
    ];

    this.customerService.getDataPreviewCustomerCare('Email', this.customerId, this.customerCareId).subscribe(response => {
      let result: any = response;
      if (result.statusCode == 200) {
        this.previewCustomerCare.effecttiveFromDate = result.effecttiveFromDate;
        this.previewCustomerCare.effecttiveToDate = result.effecttiveToDate;
        this.previewCustomerCare.sendDate = new Date(result.sendDate);
        this.previewCustomerCare.statusName = result.statusName;
        this.previewCustomerCare.previewEmailTitle = result.previewEmailTitle;
        this.previewCustomerCare.previewEmailContent = result.previewEmailContent;
        this.previewCustomerCare.previewEmailTo = result.previewEmailTo;
        this.previewCustomerCare.previewEmailCC = result.previewEmailCC;
        this.previewCustomerCare.previewEmailBcc = result.previewEmailBcc;
        this.listPreviewFileEmail = result.listPreviewFile;
        this.listPreviewFileEmail.forEach(item => {
          let fileName = item.fileName.substring(0, item.fileName.lastIndexOf('_'));
          item.fileNameShow = `${fileName}.${item.fileExtension}`;
        })
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  /*Event khi download 1 file đã lưu trên server*/
  downloadFile(fileInfor: any) {

    this.imageService.downloadFile(fileInfor.fileNameShow, fileInfor.fileUrl).subscribe(response => {
      var result = <any>response;
      if (result.statusCode == 200) {
        var binaryString = atob(result.fileAsBase64);
        var fileType = result.fileType;
        var name = fileInfor.fileNameShow;

        var binaryLen = binaryString.length;
        var bytes = new Uint8Array(binaryLen);
        for (var idx = 0; idx < binaryLen; idx++) {
          var ascii = binaryString.charCodeAt(idx);
          bytes[idx] = ascii;
        }
        var file = new Blob([bytes], { type: fileType });
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
          window.navigator.msSaveOrOpenBlob(file);
        } else {
          var fileURL = URL.createObjectURL(file);
          if (fileType.indexOf('image') !== -1) {
            window.open(fileURL);
          } else {
            var anchor = document.createElement("a");
            anchor.download = name;
            anchor.href = fileURL;
            anchor.click();
          }
        }
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  cancel() {
    this.ref.close();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }
}
