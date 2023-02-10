import { Component, OnInit, ViewChild } from '@angular/core';
import { MessageService, ConfirmationService, TreeNode } from 'primeng/api';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TranslateService } from '@ngx-translate/core';
import { ProjectService } from '../../services/project.service';
import { ProjectScopeModel } from '../../models/ProjectScope.model';
import { TreeTable } from 'primeng';

@Component({
  selector: 'app-task-project-scope-dialog',
  templateUrl: './task-project-scope-dialog.component.html',
  styleUrls: ['./task-project-scope-dialog.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class TaskProjectScopeDialogComponent implements OnInit {
  loading: boolean = false;
  auth = JSON.parse(localStorage.getItem("auth"));
  projectId: string = '';

  @ViewChild('treeTable') treeTable: TreeTable;
  listNoteProjectScope: TreeNode[];

  /*BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  listProjectScope: Array<ProjectScopeModel> = [];
  /*END*/

  columns: any;
  selectedColumns: any;
  selectedNode: TreeNode;

  constructor(
    private translate: TranslateService,
    private messageService: MessageService,
    private projectService: ProjectService,
    public config: DynamicDialogConfig,
    public ref: DynamicDialogRef,
  ) {
    this.translate.setDefaultLang('vi');
    this.projectId = this.config.data.projectId;
  }

  ngOnInit(): void {
    this.initTable();
    this.getMasterData();
  }

  initTable() {
    this.columns = [
      { field: 'Name', header: 'Hạng mục', textAlign: 'left' },
    ];
  }

  getMasterData() {
    this.loading = true;
    this.projectService.getProjectScopeByProjectId(this.projectId, this.auth.UserId).subscribe(response => {
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
  }

  showMessenger(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearMessenger() {
    this.messageService.clear();
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

  selectedScope(rowData: any) {
    let projectScope = this.listProjectScope.find(c => c.projectScopeId == rowData.Id);
    this.ref.close(projectScope);
  }
}
