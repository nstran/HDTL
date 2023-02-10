import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { ProjectService } from '../../services/project.service';
import { ProjectResourceModel } from '../../models/ProjectResource.model';
/* Primeng API */
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
/* end PrimeNg API */
import { EmployeeService } from '../../../employee/services/employee.service';
import { EmailConfigService } from '../../../../app/admin/services/email-config.service';
import { ProjectTaskModel, TaskModel } from '../../models/ProjectTask.model';
import { TaskContraint } from '../../models/TaskConstraint.model';

class Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}

interface DialogResult {
  status: boolean;
}
@Component({
  selector: 'app-add-constaint-dialog',
  templateUrl: './add-constaint-dialog.component.html',
  styleUrls: ['./add-constaint-dialog.component.css']
})
export class AddConstaintDialogComponent implements OnInit {
  loading: boolean = false;
  projectId: string = null;
  taskId: string = null;
  isCreate: boolean = false;

  auth: any = JSON.parse(localStorage.getItem("auth"));

  /*FORM CONTROL*/
  createConstraintForm: FormGroup;
  taskControl: FormControl;
  delayTimeControl: FormControl;
  constraintRequriedControl: FormControl;
  constraintTypeControl: FormControl;
  /*END*/

  /*BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  listConstraint: Array<Category> = [];
  listTask: Array<TaskModel> = [];
  /*END*/

  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  constructor(
    public ref: DynamicDialogRef,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private projectService: ProjectService,
    public config: DynamicDialogConfig,
  ) {
    this.projectId = this.config.data.projectId;
    this.taskId = this.config.data.taskId;
    this.isCreate = this.config.data.isCreate;
  }

  ngOnInit(): void {
    this.initForm();
    this.getMasterData();
  }

  initForm() {
    this.taskControl = new FormControl(null, [Validators.required]);
    this.constraintTypeControl = new FormControl(null, [Validators.required]);
    this.delayTimeControl = new FormControl('0');
    this.constraintRequriedControl = new FormControl(true);

    this.createConstraintForm = new FormGroup({
      taskControl: this.taskControl,
      constraintTypeControl: this.constraintTypeControl,
      delayTimeControl: this.delayTimeControl,
      constraintRequriedControl: this.constraintRequriedControl
    });
  }

  getMasterData() {
    this.loading = true;
    this.projectService.getMasterDataCreateConstraint(this.taskId, this.projectId, this.auth.UserId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listConstraint = result.listConstraint;
        this.listTask = result.listTask;
        this.listTask.forEach(item => {
          item.taskName = `${item.taskCode}-${item.taskName}`;
        })
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  cancel() {
    this.ref.close();
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ key: "popupp", severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  save() {
    if (!this.createConstraintForm.valid) {
      Object.keys(this.createConstraintForm.controls).forEach(key => {
        if (!this.createConstraintForm.controls[key].valid) {
          this.createConstraintForm.controls[key].markAsTouched();
        }
      });
    } else {
      this.loading = true;
      let taskContraint: TaskContraint = this.mappingFormToModel();
      this.projectService.createConstraintTask(taskContraint, this.auth.UserId).subscribe(response => {
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

  mappingFormToModel(): TaskContraint {
    let model = new TaskContraint();
    model.taskId = this.taskId;
    model.projectId = this.projectId;
    model.parentId = this.taskControl.value ? this.taskControl.value.taskId : null;
    model.constraintType = this.constraintTypeControl.value ? this.constraintTypeControl.value.categoryId : null;
    model.constraingRequired = this.constraintRequriedControl.value;
    model.delayTime = this.delayTimeControl.value ? ParseStringToFloat(this.delayTimeControl.value) : 0;

    return model;
  }

}
function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
