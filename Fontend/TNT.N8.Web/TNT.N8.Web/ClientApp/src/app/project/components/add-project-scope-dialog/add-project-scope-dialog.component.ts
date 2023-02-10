import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
/* Primeng API */
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService, TreeNode } from 'primeng/api';
/* end PrimeNg API */

import { ProjectScopeModel } from '../../models/ProjectScope.model';
import { ProjectService } from '../../services/project.service';
import { TaskModel } from '../../models/ProjectTask.model';
import { CategoryModel } from '../../../../app/shared/models/category.model';
import { TreeTable } from 'primeng';

interface DialogResult {
  status: boolean;
  scopeModel: projectScopeDialogModel
}

class projectScopeDialogModel {
  projectScopeId: string;
  projectId: string;
  parentScopeId: string;
  parentScopeCode: string;
  resourceType: number;
  projectScopeName: string;
  projectScopeDescription: string;
  supplyId: string;
  listTaskId: Array<any>;
  isExternal: number;
  levelScope: number;
}


@Component({
  selector: 'app-add-project-scope-dialog',
  templateUrl: './add-project-scope-dialog.component.html',
  styleUrls: ['./add-project-scope-dialog.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class AddProjectScopeDialogComponent implements OnInit {
  loading: boolean = false;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  isExternal: number = 0;
  projectScope: ProjectScopeModel = null;
  parentId: string;
  listTaskOfScope: Array<TaskModel> = [];
  projectId: string;
  columns: Array<any> = [];
  selectedColumns: Array<any> = [];
  selectedTask: Array<TaskModel> = [];
  listResourceType: Array<CategoryModel> = [];
  display: boolean = false;
  columnsTree: any;
  selectedNode: TreeNode;
  @ViewChild('treeTable') treeTable: TreeTable;
  listNoteProjectScope: TreeNode[];
  listProjectScope: Array<ProjectScopeModel> = [];

  createProjectScopeForm: FormGroup;
  toProjectScopeControl: FormControl;
  resourceTypeControl: FormControl;
  scopeNameControl: FormControl;
  scopeControl: FormControl; // Hạng mục
  parentName: string;
  listVendor: Array<any> = [];
  parentCode: string;
  levelScope: number;

  tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink']
  };

  constructor(
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    public ref: DynamicDialogRef,
    private projectService: ProjectService,
    public config: DynamicDialogConfig) {
    this.parentId = this.config.data.parentId;

    this.listResourceType = this.config.data.listResourceType;
    this.parentName = this.config.data.parentName;
    this.listVendor = this.config.data.listVendor;
    this.parentCode = this.config.data.parentCode;
    this.levelScope = this.config.data.level;
    this.projectId = this.config.data.projectId;
  }
  ngOnInit(): void {
    this.initForm();
    this.initTable();
    this.scopeControl.setValue(this.parentName);
  }
  getMasterData() {

    this.projectService.getAllTaskByProjectScopeId(this.projectId, this.projectScope == null ? this.parentId : this.projectScope.projectScopeId, this.auth.UserId).subscribe(response => {
      let result: any = response;

      this.loading = true;
      if (result.statusCode == 200) {
        this.listTaskOfScope = result.listTask;
      }
      this.loading = false;
    });
  }
  initTable() {
    this.columns = [
      { field: 'taskCode', header: 'Mã', width: '5%', textAlign: 'center', color: '#f44336' },
      { field: 'taskName', header: 'Tên công việc', width: '35%', textAlign: 'left', color: '#f44336' },
      { field: 'taskTyeName', header: 'Loại công việc', width: '20%', textAlign: 'center', color: '#f44336' },
      { field: 'statusName', header: 'Trạng thái', width: '15%', textAlign: 'center', color: '#f44336' },
      { field: 'employee', header: 'Người phụ trách', width: '25%', textAlign: 'left', color: '#f44336' }
    ];
    this.selectedColumns = this.columns;
  }

  initForm() {
    this.scopeControl = new FormControl(null, [Validators.required]);
    this.resourceTypeControl = new FormControl(null, [Validators.required]);
    this.scopeNameControl = new FormControl(null, [Validators.required]);
    this.toProjectScopeControl = new FormControl(null);
    this.createProjectScopeForm = new FormGroup({
      scopeControl: this.scopeControl,
      'supplyControl': new FormControl(null),
      'scopeDescriptionControl': new FormControl(null),
      resourceTypeControl: this.resourceTypeControl,
      scopeNameControl: this.scopeNameControl,
      toProjectScopeControl: this.toProjectScopeControl
    });
  }
  // CHọn hạng mục
  async openProjetScopeDialog() {
    this.display = true;

    this.initTableProjectResource();
    await this.getMasterDataProjectResource();
  }

  cancel() {
    this.confirmationService.confirm({
      message: 'Hành động này không thể được hoàn tác, bạn có chắc chắn muốn hủy?',
      accept: () => {
        this.ref.close();
      }
    });
  }

  // Thay đổi nguồn lực
  changeResource(event: any) {

    if (event.value.categoryCode == "TN")
      this.isExternal = 1;
    else {
      // Nguon luc noi bo
      this.isExternal = 2;
      //Load danh sách công việc của scope
      this.getMasterData();
    }
  }

  // Tạo mới hạng mục công việc ( có kèm theo các task nếu có )
  save() {

    if (!this.createProjectScopeForm.valid) {
      Object.keys(this.createProjectScopeForm.controls).forEach(key => {
        if (!this.createProjectScopeForm.controls[key].valid) {
          this.createProjectScopeForm.controls[key].markAsTouched();
        }
      });
    } else {
      //valid
      let scopeModel: projectScopeDialogModel = this.mapFormToTaskModel();
      let result: DialogResult = {
        status: true,
        scopeModel: scopeModel
      }
      this.ref.close(result);
    }
  }


  clearMessenger() {
    this.messageService.clear();
  }

  async getMasterDataProjectResource() {
    this.loading = true;

    await this.projectService.getProjectScopeByProjectId(this.projectId, this.auth.UserId).subscribe(response => {
      let result: any = response;

      this.loading = false;
      if (result.statusCode === 200) {
        this.listProjectScope = result.listProjectScrope;
        this.listNoteProjectScope = this.list_to_tree();
      } else {
        this.clearMessenger();
        this.showMessenger('error', 'Thông báo', result.Message);
      }
    });
    this.loading = false;
  }

  showMessenger(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  list_to_tree() {
    let list: Array<TreeNode> = [];
    this.listProjectScope.forEach(item => {
      let node: TreeNode = {
        label: item.projectScopeName,
        expanded: true,
        data: {
          'Id': item.projectScopeId,
          'ParentId': item.parentId,
          "Name": item.projectScopeName,
          "Level": item.level
        },
        children: []
      };

      list = [...list, node];
    });

    var map = {}, node: TreeNode, roots = [], i: number;

    for (i = 0; i < list.length; i += 1) {
      map[list[i].data.Id] = i; // initialize the map
      list[i].children = []; // initialize the children
    }

    for (i = 0; i < list.length; i += 1) {
      node = list[i];
      if (node.data.ParentId !== null) {
        // if you have dangling branches check that map[node.ParentId] exists
        list[map[node.data.ParentId]].children.push(node);
      } else {
        roots.push(node);
      }
    }
    return roots;
  }

  initTableProjectResource() {
    this.columns = [
      { field: 'Name', header: 'Hạng mục', textAlign: 'left' },
    ];
  }

  selectedScope(rowData: any) {

    let projectScope = this.listProjectScope.find(c => c.projectScopeId == rowData.Id);
    this.projectScope = projectScope;
    this.parentId = this.projectScope.projectScopeId;
    this.scopeControl.setValue(this.projectScope.projectScopeName);
    this.levelScope = rowData.Level;

    this.display = false;
    if (this.isExternal == 2)
      this.getMasterData();
  }

  mapFormToTaskModel(): projectScopeDialogModel {
    let targetDialogModel: projectScopeDialogModel = new projectScopeDialogModel();
    // Them goi cong viec

    targetDialogModel.parentScopeId = this.parentId;
    targetDialogModel.parentScopeCode = this.projectScope == null ? this.parentCode : this.projectScope.projectScopeCode;
    targetDialogModel.resourceType = this.createProjectScopeForm.get('resourceTypeControl').value.categoryId;
    targetDialogModel.projectScopeName = this.createProjectScopeForm.get('scopeNameControl').value;
    targetDialogModel.projectScopeDescription = this.createProjectScopeForm.get('scopeDescriptionControl')
      .value;
    targetDialogModel.levelScope = this.levelScope + 1;

    if (this.isExternal == 1) {
      targetDialogModel.isExternal = 1;
      targetDialogModel.supplyId = this.createProjectScopeForm.get('supplyControl').value ?.vendorId;
    }
    else {
      targetDialogModel.listTaskId = this.selectedTask.map(x => x.taskId);
      targetDialogModel.isExternal = 2;
    }
    return targetDialogModel;
  }
}

