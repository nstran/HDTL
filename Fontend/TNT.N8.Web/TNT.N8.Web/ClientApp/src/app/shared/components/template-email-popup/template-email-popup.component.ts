import { Component, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { EmailConfigService } from '../../../admin/services/email-config.service';

@Component({
  selector: 'app-template-email-popup',
  templateUrl: './template-email-popup.component.html',
  styleUrls: ['./template-email-popup.component.css']
})
export class TemplateEmailPopupComponent implements OnInit {

  emailContent: string = '';
  loading: boolean = false;
  selectedColumns: any = [];
  listEmailTemplateName: Array<any> = [];

  tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink', '|', 'ClearFormat', '|', 'Undo', 'Redo']
  };

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private emailConfigService: EmailConfigService,
    private messageService: MessageService,
  ) { }

  ngOnInit(): void {
    // preview File đính kèm email
    this.selectedColumns = [
      { field: 'emailTemplateName', header: 'Tên mẫu email', width: '100%' },
    ];

    this.getMasterData();
  }

  async getMasterData() {
    this.loading = true;
    let [masterdataResponse, initSearchResponse]: any = await Promise.all([
      this.emailConfigService.searchEmailConfigMasterdata(),
      this.emailConfigService.searchEmailTemplate('', '', [], [])
    ]);
    this.loading = false;

    if (masterdataResponse.statusCode === 200 && initSearchResponse.statusCode === 200) {
      //search response
      this.listEmailTemplateName = initSearchResponse.listEmailTemplateModel;
      this.listEmailTemplateName = this.listEmailTemplateName.filter(x => x.emailTemplateStatusCode == 'CHL');
      if (this.listEmailTemplateName.length === 0) {
        let msg = { key: 'popup', severity: 'warn', summary: 'Thông báo:', detail: 'Không có dữ liệu' };
        this.showMessage(msg);
      }
    }
    else if (initSearchResponse.statusCode !== 200) {
      let msg = { key: 'popup', severity: 'warn', summary: 'Thông báo:', detail: initSearchResponse.messageCode };
      this.showMessage(msg);
    }
  }

  showEmailContent(rowData: any) {
    this.emailContent = rowData.emailTemplateContent;
  }

  cancel() {
    this.ref.close();
  }

  saveTemplate() {
    this.ref.close(this.emailContent)
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }
}
