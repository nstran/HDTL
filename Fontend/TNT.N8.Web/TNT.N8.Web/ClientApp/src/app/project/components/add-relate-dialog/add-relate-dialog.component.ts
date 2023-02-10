import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { ProjectService } from '../../services/project.service';
/* Primeng API */
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
/* end PrimeNg API */
import { TaskRelate } from '../../models/TaskRelate.model';

class Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}

class Task {
  taskId : string;
  taskCode : string;
  taskTypeId : string;
  taskName : string;
  parentId: string;
  status : string;
}

interface DialogResult {
  status: boolean;
}
@Component({
  selector: 'app-add-relate-dialog',
  templateUrl: './add-relate-dialog.component.html',
  styleUrls: ['./add-relate-dialog.component.css']
})
export class AddRelateDialogComponent implements OnInit {
  loading: boolean = false;
  projectId: string = null;
  taskId: string = null;
  isCreate: boolean = false;

  auth: any = JSON.parse(localStorage.getItem("auth"));

  /*FORM CONTROL*/
  createRelateForm: FormGroup;
  taskTypeControl: FormControl;
  taskRelateControl: FormControl;
  /*END*/

  /*BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  listRelateTask: Array<Task> = [];
  listRelateTaskShow: Array<Task> = [];
  listTaskType: Array<Category> = [];
  /*END*/

  listRelateTaskReturn: Array<any> = [];

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
    this.taskTypeControl = new FormControl(null, [Validators.required]);
    this.taskRelateControl = new FormControl(null, [Validators.required]);

    this.createRelateForm = new FormGroup({
      taskTypeControl: this.taskTypeControl,
      taskRelateControl: this.taskRelateControl,
   
    });
  }

  getMasterData() {
    this.loading = true;
    this.projectService.GetMasterDataCreateRelateTask(this.taskId, this.projectId, this.auth.UserId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listRelateTask = result.listRelateTask;

        this.listTaskType = result.listTaskType;
        this.listRelateTask.forEach(item => {
          item.taskName = `${item.taskCode}-${item.taskName}`;
        });

        this.listRelateTaskShow = this.listRelateTask;

        this.taskTypeControl.setValue(this.listTaskType.find(x => x.categoryCode == "Loi"));

        let currentTaskType = this.taskTypeControl.value.categoryId;
        this.listRelateTaskShow = this.listRelateTask.filter( x => x.taskTypeId == currentTaskType);
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  changeTaskType(){
    let currentTaskType = this.taskTypeControl.value.categoryId;
    this.listRelateTaskShow = this.listRelateTask.filter( x => x.taskTypeId == currentTaskType);
  }
  cancel() {
    let taskRelate: TaskRelate = this.mappingFormToModel();
    this.ref.close(taskRelate);
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ key: "popupp", severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  save() {
    if (!this.createRelateForm.valid) {
      Object.keys(this.createRelateForm.controls).forEach(key => {
        if (!this.createRelateForm.controls[key].valid) {
          this.createRelateForm.controls[key].markAsTouched();
        }
      });
    } else {
      if(this.taskId == null){
        let taskRelate: TaskRelate = this.mappingFormToModel();
        this.ref.close(taskRelate);
      }else{
        this.loading = true;
        let taskRelate: TaskRelate = this.mappingFormToModel();
        this.projectService.createRelateTask(taskRelate, this.auth.UserId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.ref.close(result.taskRelate);
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    }
  }

  mappingFormToModel(): TaskRelate {
    let model = new TaskRelate();
    model.TaskId = this.taskId;
    model.relateTaskId = this.taskRelateControl.value ? this.taskRelateControl.value.taskId : null;
    model.ProjectId = this.projectId;
    model.taskName = this.taskRelateControl.value ? this.taskRelateControl.value.taskName : null;
    model.taskCode = this.taskRelateControl.value ? this.taskRelateControl.value.taskCode : null;
    model.expectedStartDate = this.taskRelateControl.value ? this.taskRelateControl.value.planStartTime : null;
    model.expectedEndDate = this.taskRelateControl.value ? this.taskRelateControl.value.planEndTime : null;
    model.statusName = this.taskRelateControl.value ? this.taskRelateControl.value.statusName : null;
    model.statusCode = this.taskRelateControl.value ? this.taskRelateControl.value.statusCode : null;
    model.backgroundColorForStatus = this.taskRelateControl.value ? this.taskRelateControl.value.backgroundColorForStatus : null;
    return model;
  }

}
function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
