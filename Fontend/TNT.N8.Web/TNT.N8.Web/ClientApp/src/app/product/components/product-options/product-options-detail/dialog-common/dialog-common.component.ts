import { Component, Input, OnInit } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng';
import { OptionsEntityModel } from '../../model/options';

@Component({
  selector: 'app-dialog-common',
  templateUrl: './dialog-common.component.html',
  styleUrls: ['./dialog-common.component.css']
})
export class DialogCommonComponent implements OnInit {
  optionsEntityModel: OptionsEntityModel = new OptionsEntityModel();
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
  ) { }
  @Input() optionId: string
  @Input() isDialog: boolean
  ngOnInit(): void {
    this.optionId = this.config.data.optionId
    this.isDialog = this.config.data.isDialog
  }

}
