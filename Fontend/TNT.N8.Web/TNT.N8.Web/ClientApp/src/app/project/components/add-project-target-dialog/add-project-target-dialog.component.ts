import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
/* Primeng API */
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
/* end PrimeNg API */

interface ConfigData {
  isEdit: boolean;
  target: projectTargetDialogModel
}
interface DialogResult {
  status: boolean;
  targetModel: projectTargetDialogModel
}

class projectTargetDialogModel {
  projectTargetId: string;
  projectId: string;
  orderNumber: number;
  projectObjectType: string;
  targetTypeDisplay: string;
  projectObjectName: string;
  projectObjectUnit: string;
  targetUnitDisplay: string;
  projectObjectValue: number;
  projectObjectDescription: string;
}

@Component({
  selector: 'app-add-project-target-dialog',
  templateUrl: './add-project-target-dialog.component.html',
  styleUrls: ['./add-project-target-dialog.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class AddProjectTargetDialogComponent implements OnInit {
  loading: boolean = false;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  listTargetType: Array<any> = [];
  listTargetUnit: Array<any> = [];
  createProjectTargetForm: FormGroup;
  statusCode: string = '';
  isApproveProject: boolean = false;

  tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink']
  };

  constructor(private messageService: MessageService,
    private confirmationService: ConfirmationService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig) {
    this.statusCode = this.config.data.statusCode;
    this.isApproveProject = this.config.data.isApproveProject;
    this.listTargetType = this.config.data.listTargetType;
    this.listTargetUnit = this.config.data.listTargetUnit;
  }

  ngOnInit(): void {

    this.initForm();
    if (this.config.data) {
      let configData: ConfigData = this.config.data;
      if (configData.isEdit === true) this.patchDataToForm(configData.target);
    }
    // if (this.statusCode == 'TDU' || this.statusCode == 'HUY' || this.isApproveProject) {
    //   this.createProjectTargetForm.disable();
    // } else {
    //   this.createProjectTargetForm.enable();
    // }
    if (this.statusCode == 'TDU' || this.statusCode == 'HUY') {
      this.createProjectTargetForm.disable();
    } else {
      this.createProjectTargetForm.enable();
    }
  }

  initForm() {
    this.createProjectTargetForm = new FormGroup({
      'targetTypeControl': new FormControl(null, [Validators.required]),
      'targetUnitControl': new FormControl(null),
      'targetValueControl': new FormControl(null),
      'targetNameControl': new FormControl(null, [Validators.required]),
      'targetDescriptionControl': new FormControl(null),
    });
  }

  cancel() {
    // this.confirmationService.confirm({
    //   message: 'Hành động này không thể được hoàn tác, bạn có chắc chắn muốn hủy?',
    //   accept: () => {
    //   }
    // });
    this.ref.close();
  }

  save() {
    if (!this.createProjectTargetForm.valid) {
      Object.keys(this.createProjectTargetForm.controls).forEach(key => {
        if (!this.createProjectTargetForm.controls[key].valid) {
          this.createProjectTargetForm.controls[key].markAsTouched();
        }
      });
    } else {
      //valid
      let targetModel: projectTargetDialogModel = this.mapFormToTargetModel();
      let result: DialogResult = {
        status: true,
        targetModel: targetModel
      }
      this.ref.close(result);
    }
  }

  mapFormToTargetModel(): projectTargetDialogModel {
    let targetDialogModel: projectTargetDialogModel = new projectTargetDialogModel();
    targetDialogModel.projectObjectType = this.createProjectTargetForm.get('targetTypeControl').value?.categoryId;
    targetDialogModel.targetTypeDisplay = this.createProjectTargetForm.get('targetTypeControl').value?.categoryName;
    targetDialogModel.projectObjectUnit = this.createProjectTargetForm.get('targetUnitControl').value?.categoryId;
    targetDialogModel.targetUnitDisplay = this.createProjectTargetForm.get('targetUnitControl').value?.categoryName;
    targetDialogModel.projectObjectName = this.createProjectTargetForm.get('targetNameControl').value;
    targetDialogModel.projectObjectValue = this.createProjectTargetForm.get('targetValueControl').value;
    targetDialogModel.projectObjectDescription = this.createProjectTargetForm.get('targetDescriptionControl').value;
    return targetDialogModel;
  }
  patchDataToForm(target: projectTargetDialogModel) {
    this.createProjectTargetForm.get('targetNameControl').setValue(target.projectObjectName);
    this.createProjectTargetForm.get('targetValueControl').setValue(target.projectObjectValue);
    this.createProjectTargetForm.get('targetDescriptionControl').setValue(target.projectObjectDescription);
    let _type = this.listTargetType.find(e => e.categoryId == target.projectObjectType);
    this.createProjectTargetForm.get('targetTypeControl').setValue(_type ? _type : null);
    let _unit = this.listTargetUnit.find(e => e.categoryId == target.projectObjectUnit);
    this.createProjectTargetForm.get('targetUnitControl').setValue(_unit ? _unit : null);
  }
}
