import { Component, OnInit, ElementRef, HostListener, ViewChild, Renderer2 } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import * as $ from 'jquery';
import { Location } from '@angular/common';
import { NoteModel } from '../../../shared/models/note.model';

import { MessageService, ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission'
import { ProjectService } from '../../services/project.service';
import { ProjectModel } from '../../models/project.model';
import { runInThisContext } from 'vm';
import { ProjectMilestoneModel } from '../../models/milestone.model';

@Component({
  selector: 'app-create-or-update-milestone-dialog',
  templateUrl: './create-or-update-milestone-dialog.component.html',
  styleUrls: ['./create-or-update-milestone-dialog.component.css']
})
export class CreateOrUpdateMilestoneDialogComponent implements OnInit {
  auth = JSON.parse(localStorage.getItem("auth"));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();
  //START: BIẾN ĐIỀU KIỆN
  actionAdd: boolean = true;
  fixed: boolean = false;
  withFiexd: string = "";
  isOpenNotifiError: boolean = false;
  isInvalidForm: boolean = false;
  //END : BIẾN ĐIỀU KIỆN

  /*START: FORM CONTROL*/
  createOrUpdateMilestoneFrom: FormGroup;

  projectControl: FormControl;
  nameControl: FormControl;
  descriptionControl: FormControl;
  endTimeControl: FormControl;
  /*END: FORM CONTROL*/

  projectMilestoneId: string = this.emptyGuid;
  projectId: string = this.emptyGuid;
  isCreate: boolean = true;

  /*BIẾN LƯU DỮ LIỆU TRẢ VỀ*/
  listProject: Array<ProjectModel> = [];
  projectMilestone: ProjectMilestoneModel = null;
  /*END*/

  tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink']
  };
  isApproveProject: boolean = false;

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    public builder: FormBuilder,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private renderer: Renderer2,
    private projectService: ProjectService,
  ) {
    this.translate.setDefaultLang('vi');
    this.projectMilestoneId = this.config.data.projectMilestoneId;
    this.projectId = this.config.data.projectId;
    this.isCreate = this.config.data.isCreate;
    this.isApproveProject = this.config.data.isApproveProject;
  }

  ngOnInit(): void {
    this.initForm();
    this.getMasterData();
  }

  initForm() {
    this.projectControl = new FormControl(null, [Validators.required]);
    this.nameControl = new FormControl('', [Validators.required]);
    this.endTimeControl = new FormControl(null, [Validators.required]);
    this.descriptionControl = new FormControl('');

    this.createOrUpdateMilestoneFrom = new FormGroup({
      projectControl: this.projectControl,
      nameControl: this.nameControl,
      descriptionControl: this.descriptionControl,
      endTimeControl: this.endTimeControl
    });
  }

  getMasterData() {
    this.loading = true;
    this.projectService.getMasterDataCreateOrUpdateMilestone(this.projectMilestoneId, this.projectId, this.auth.UserId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listProject = result.listProject;
        this.listProject.forEach(item => {
          item.projectStartDate = new Date(item.projectStartDate);
          item.projectEndDate = new Date(item.projectEndDate);
        });
        this.projectMilestone = result.projectMilestone;
        if (!this.isCreate) {
          this.mappingDataToForm(result.projectMilestone);
        } else {
          this.setDefaultValue();
        }
      }
    });
  }

  setDefaultValue() {
    let project = this.listProject.find(c => c.projectId == this.projectId);
    if (project) {
      this.projectControl.setValue(project);
    }
  }

  mappingDataToForm(projectMilestone: ProjectMilestoneModel) {
    this.projectMilestoneId = projectMilestone.projectMilestonesId;

    let project = this.listProject.find(c => c.projectId == projectMilestone.projectId);
    if (project) {
      this.projectControl.setValue(project);
    }

    this.nameControl.setValue(projectMilestone.name);
    this.endTimeControl.setValue(new Date(projectMilestone.endTime));
    this.descriptionControl.setValue(projectMilestone.description);
  }

  save() {
    if (!this.createOrUpdateMilestoneFrom.valid) {
      Object.keys(this.createOrUpdateMilestoneFrom.controls).forEach(key => {
        if (!this.createOrUpdateMilestoneFrom.controls[key].valid) {
          this.createOrUpdateMilestoneFrom.controls[key].markAsTouched();
        }
      });
    } else {
      let projectMilestone = this.mappingDataFormToModel();
      this.loading = true;
      this.projectService.createOrUpdateMilestone(projectMilestone, this.auth.UserId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.ref.close(true);
        } else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      });
    }
  }

  mappingDataFormToModel(): ProjectMilestoneModel {
    var entity = new ProjectMilestoneModel();
    if (!this.isCreate) {
      entity.projectMilestonesId = this.projectMilestoneId;
    }
    entity.projectId = this.projectControl.value ? this.projectControl.value.projectId : null;
    entity.name = this.nameControl.value ? this.nameControl.value.trim() : '';
    entity.endTime = this.endTimeControl.value ? convertToUTCTime(this.endTimeControl.value) : null;
    entity.description = this.descriptionControl.value ? this.descriptionControl.value.trim() : null;

    return entity;
  }

  cancel() {
    this.ref.close();
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ key: "popup", severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
