import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { ProjectService } from '../../services/project.service';
import { MessageService, TreeNode, ConfirmationService } from 'primeng/api';
import { ProjectModel } from '../../models/project.model';
import { DatePipe } from '@angular/common';
import { ProjectScopeModel } from '../../models/ProjectScope.model';
import { ProjectTaskModel } from '../../models/ProjectTask.model';
import { AddProjectScopeDialogComponent } from '../add-project-scope-dialog/add-project-scope-dialog.component';
import { DynamicDialogRef, DialogService } from 'primeng/dynamicdialog';
import { ProjectResourceModel } from '../../models/ProjectResource.model';
import { MenuItem } from 'primeng/api';
import { AddTaskScopeDialogComponent } from '../add-task-scope-dialog/add-task-scope-dialog.component';
import { Router, ActivatedRoute } from '@angular/router';
import { NoteService } from '../../../shared/services/note.service';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { NoteModel } from '../../../shared/models/note.model';
import { FileUpload, Table } from 'primeng';
import * as $ from 'jquery';
import { Paginator } from 'primeng/paginator';
import { GetPermission } from '../../../shared/permission/get-permission';
import { async } from 'q';

class Project {
  projectId: string;
  projectCode: string;
  projectName: string;
  projectCodeName: string;
  projectStatusPlan: boolean;
}


class Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}

class PhanTram {
  label: string;
  value: number;
}

interface CategoryTypeModel {
  categoryTypeId: string,
  categoryTypeName: string;
  categoryTypeCode: string;
  categoryList: Array<CategoryModel>;
  active: boolean,
}

interface CategoryModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  categoryTypeId: CategoryTypeModel["categoryTypeId"];
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: string;
  active: Boolean;
  isEdit: Boolean;
  isDefault: Boolean;
  statusName: string;
  potentialName: string;
  categoryTypeName: string;
  listEmployee: Array<string>;
}

class Employee {
  public employeeId: string;
  public employeeName: string;
  public employeeCode: string;
  public active: boolean
}

class ProjectScopeFillter {
  public projectSopeId: string;
  public projectScopeName: string;
}

class Note {
  active: boolean;
  createdById: string;
  createdDate: Date;
  description: string;
  noteDocList: Array<NoteDocument>;
  noteId: string;
  noteTitle: string;
  objectId: string;
  objectType: string;
  responsibleAvatar: string;
  responsibleName: string;
  type: string;
  updatedById: string;
  updatedDate: Date;
  constructor() {
    this.noteDocList = [];
  }
}

class NoteDocument {
  active: boolean;
  base64Url: string;
  createdById: string;
  createdDate: Date;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  noteDocumentId: string;
  noteId: string;
  updatedById: string;
  updatedDate: Date;
}
@Component({
  selector: 'app-project-scope',
  templateUrl: './project-scope.component.html',
  styleUrls: ['./project-scope.component.css'],
  providers: [
    DatePipe
  ]
})

export class ProjectScopeComponent implements OnInit {
  constructor(
    private projectService: ProjectService,
    public ref: DynamicDialogRef,
    private router: Router,
    private route: ActivatedRoute,
    private noteService: NoteService,
    private imageService: ImageUploadService,
    private getPermission: GetPermission,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService) {
    this.innerWidth = window.innerWidth;
  }
  @ViewChild('fileUpload') fileUpload: FileUpload;
  @ViewChild('fileUploadNote') fileUploadNote: FileUpload;
  listResourceType: Array<CategoryModel> = [];
  loading: boolean = false;
  project: ProjectModel = new ProjectModel();
  isShow: boolean = true;
  data: TreeNode[];
  dataCopy: TreeNode[];
  selectedColumns: any[];
  selectedNode: TreeNode;
  projectId: string = '8A765839-5692-4CEB-9EEF-912FEE254A41';
  listProjectScopes: Array<ProjectScopeModel> = [];
  listProjectTasks: Array<ProjectTaskModel> = [];
  listProjectTasksDefault: Array<ProjectTaskModel> = [];
  listEmployee: Array<ProjectTaskModel> = [];

  selectedScope: any;
  selectedScopeName: string;
  selectedScopeCode: string;
  selectedScopeId: string;
  scopeDescription: string;
  /*Get Current EmployeeId*/
  auth = JSON.parse(localStorage.getItem('auth'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  /*End*/

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionDelete: boolean = true;
  actionEdit: boolean = true;
  actionDeleteScope: boolean = false;
  actionEditScope: boolean = false;
  actionEditNote: boolean = true;
  actionCopyScope: boolean = true;
  /*End*/
  totalPercent: string = "0"; // Tiến độ dự án
  listAction: MenuItem[]; // Them hang muc hay workpackage
  listVendor: Array<any> = []; // Nha thau
  parentName: string;// Trực thuộc hạng mục
  listStatus: Array<CategoryModel> = [];
  listResourceScope: Array<any> = [];
  isEdit: boolean = false;
  totalEstimateTime: number = 0;

  /* Search value */
  listWorkpackage: Array<ProjectScopeFillter> = [];
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  selectedStatus: Array<Category> = [];
  selectedWorkpackage: Array<ProjectScopeFillter> = [];
  selectedPic: Array<Employee> = [];
  startFromDate: Date = null;
  startToDate: Date = null;
  endToDate: Date = null;
  endFromDate: Date = null;
  fromPercent: number = 0;
  toPercent: number = 0;

  /* FORM CONTROL */
  createTaskScopeForm: FormGroup;
  descriptionControl: FormControl;
  scopeProjectForm: FormGroup;
  projectScopeForm: FormGroup;
  projectScopeDescriptionControl: FormControl;
  projectScopeNameControl: FormControl;
  resourceTypeControl: FormControl;
  /*END: FORM CONTROL*/
  cloneProjectScope: FormGroup;
  projectControl: FormControl;
  oldProjectId: string = null;
  newProjectId: string = null;
  listProject: Array<any> = [];
  displayCopyScope: boolean = false;

  listProjectTitle: Array<Project> = [];
  selectedProject: Project = null;
  isApproveProject: boolean = false;

  /*START: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  listResource: Array<ProjectResourceModel> = [];
  projectScope: ProjectScopeModel = null;
  //START: BIẾN ĐIỀU KIỆN
  fixed: boolean = false;
  //END : BIẾN ĐIỀU KIỆN
  leftColNumber: number = 12;
  rightColNumber: number = 0;
  lstNode: Array<any> = []; // Cây hạng mục ở dialog
  isOpenNotifiError: boolean = false;
  isInvalidForm: boolean = false;
  isDelete: boolean = false;
  noteHistory: Array<Note> = [];

  /*NOTE*/
  noteContent: string = '';
  noteId: string;
  listUpdateNoteDocument: Array<NoteDocument> = [];
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  /*END*/
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  listNoteDocumentModel: Array<NoteDocumentModel> = [];

  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find((systemParameter: { systemKey: string; }) => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  isSendEmailControl: FormControl;
  levelScope: number = 0;
  totalRecordsNote: number = 0;
  @ViewChild('paginator', { static: true }) paginator: Paginator

  tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink']
  };

  isReadonly: boolean = true;
  thoiGianUocLuong: number = 0;

  async ngOnInit() {
    this.initForm();
    this.setForm();
    this.route.params.subscribe(async params => {
      let resource = "pro/project/scope/";
      this.projectId = params['projectId'];

      let permission: any = await this.getPermission.getPermission(resource);
      if (permission.status == false) {
        this.router.navigate(['/home']);
      }
      else {
        let listCurrentActionResource = permission.listCurrentActionResource;
        if (listCurrentActionResource.indexOf("editnote") == -1) {
          this.actionEditNote = false;
        }
        if (listCurrentActionResource.indexOf("copyscope") == -1) {
          this.actionCopyScope = false;
        }

        await this.getMasterData();
      }

      this.setTable();
    });
  }

  // lấy thông tin gói công việc trực thuộc
  async getListWorkPackage(): Promise<void> {
    // Gói công việc trực thuộc
    if (this.listProjectScopes.length > 0)
      this.listProjectScopes.forEach(item => {
        var obj = new ProjectScopeFillter();
        obj.projectSopeId = item.projectScopeId;
        obj.projectScopeName = item.projectScopeName;
        this.listWorkpackage.push(obj);
      });
  }

  // Lấy dữ liệu form
  async getMasterData() {
    this.loading = true;
    let result: any = await this.projectService.getProjectScope(this.projectId);
    this.loading = false;
    if (result.statusCode == 200) {
      this.project = result.project;

      this.listProjectScopes = result.listProjectScope || [];
      this.listProjectTasks = result.listProjectTask || [];
      this.listProjectTasksDefault = result.listProjectTask || [];
      this.listResourceType = result.listResource || [];
      this.listStatus = result.listStatus || [];
      this.listVendor = result.listVendor;

      this.listProjectTitle = result.listProject;
      this.listProjectTitle.forEach(item => {
        item.projectCodeName = `[${item.projectCode}]: ${item.projectName}`;
      });
      this.selectedProject = this.listProjectTitle.find(c => c.projectId == this.projectId);
      this.isApproveProject = this.selectedProject.projectStatusPlan;

      this.handleEmployeeContent();
      this.isSendEmailControl.setValue(true);
      // Tiến độ dự án
      this.totalPercent = this.project.taskComplate.toFixed(2);
      this.totalEstimateTime = this.project.estimateCompleteTime;

      this.totalRecordsNote = result.totalRecordsNote;
      this.noteHistory = result.listNote;
      this.data = this.list_to_tree();
      this.handleNoteContent();
      this.isDelete = false;
      this.isEdit = false;
      this.loading = false;
    } else {
      this.loading = false;
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  initForm() {
    this.projectScopeNameControl = new FormControl(null, [Validators.required]);
    this.projectScopeDescriptionControl = new FormControl;
    this.resourceTypeControl = new FormControl(null, [Validators.required]);
    this.descriptionControl = new FormControl('');
    this.isSendEmailControl = new FormControl(false);

    this.projectScopeForm = new FormGroup({
      isSendEmailControl: this.isSendEmailControl
    });

    this.scopeProjectForm = new FormGroup({
      resourceTypeControl: this.resourceTypeControl,
      projectScopeNameControl: this.projectScopeNameControl,
      projectScopeDescriptionControl: this.projectScopeDescriptionControl,
    });
    this.disableControls();

    this.projectControl = new FormControl(null);
    this.cloneProjectScope = new FormGroup({
      projectControl: this.projectControl,

    });
  }

  // HIển thị dialog bộ lọc cho tab danh sách công việc
  async showFilter() {
    // if (this.innerWidth < 1024) {
    //   this.isShowFilterTop = !this.isShowFilterTop;
    // } else {
    await this.getListWorkPackage().then(res => {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 9;
        this.rightColNumber = 3;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
    );
    //}
  }
  // Thiết lập form
  setForm(type: any = 0) {
    this.listAction = [];
    let optionTask = {
      label: 'Công việc', icon: 'pi pi-tasks', command: () => {
        this.add_Task();
      }
    };
    let optionWorkpackage = {
      label: 'Gói công việc', icon: 'pi pi-project-diagram', command: () => {
        this.addProjectScope();
      }
    };

    if (type == 0) {
      this.listAction.push(optionTask);
      this.listAction.push(optionWorkpackage);
    }
    else {
      // Không phải là node cuối
      this.listAction.push(optionWorkpackage);
    }
  }

  setTable() {
    this.selectedColumns = [
      { field: 'taskName', header: 'Tên công việc', width: '20%', textAlign: 'left', color: '#f44336' },
      { field: 'projectScopeName', header: 'Gói công việc trực thuộc', width: '13%', textAlign: 'left', color: '#f44336' },
      { field: 'employee', header: 'Người phụ trách', width: '24%', textAlign: 'left', color: '#f44336' },
      { field: 'statusName', header: 'Trạng thái', width: '13%', textAlign: 'center', color: '#f44336' },
      { field: 'taskComplate', header: '% hoàn thành', width: '5%', textAlign: 'center', color: '#f44336' },
      { field: 'planStartTime', header: 'Ngày bắt đầu', width: '10%', textAlign: 'center', color: '#f44336' },
      { field: 'planEndTime', header: 'Ngày kết thúc', width: '10%', textAlign: 'center', color: '#f44336' }
    ];
  }

  // Build cây hạng mục công việc
  list_to_tree(projectId: string = null) {
    let list: Array<TreeNode> = [];
    var map = {}, node, roots = [], i;
    if (this.listProjectScopes !== null && this.listProjectScopes.length > 0) {
      this.listProjectScopes.filter(x => projectId !== null ? x.projectId == projectId : true).forEach(item => {

        let node: TreeNode = {
          label: item.projectScopeCode == null || item.projectScopeCode == "" ? item.projectScopeName : (item.projectScopeCode + '. ' + item.projectScopeName),
          expanded: true,
          data: {
            'projectScopeId': item.projectScopeId,
            'parentId': item.parentId,
            'projectScopeName': item.projectScopeName,
            'description': item.description,
            'projectId': item.projectId,
            'projectScopeCode': item.projectScopeCode,
            'ischange': false,
            'resourceType': item.resourceType,
            'level': item.level,
            'colorScope': item.styleClass
          },
          styleClass: item.styleClass,
          children: []
        };

        list = [...list, node];
      });
      for (i = 0; i < list.length; i += 1) {
        map[list[i].data.projectScopeId] = i; // initialize the map
        list[i].children = []; // initialize the children
      }

      for (i = 0; i < list.length; i += 1) {
        node = list[i];
        if (node.data.parentId !== undefined && node.data.parentId !== null) {
          // if you have dangling branches check that map[node.parentId] exists
          list[map[node.data.parentId]].children.push(node);
        } else {

          roots.push(node);
        }
      }
      return roots;
    }
  }

  // Đệ quy lấy danh sách công việc thuộc node cha và các node con
  recursiveNodeScope = (data) => {
    this.lstNode.push(data.data.projectScopeId);
    if (data.children != null) {
      data.children.forEach(chil => {
        this.recursiveNodeScope(chil);
      });
    }

  }

  // Xóa công việc
  delete_Task(rowData: any) {


    if (rowData.statusName !== "Mới") {
      this.clearToast();
      this.showToast('error', 'Thông báo', 'Chỉ được xóa công việc ở trọng thái Mới');
    }
    else {
      this.confirmationService.confirm({
        header: "Xác nhận",
        message: 'Bạn có chắc muốn xóa?',
        accept: () => {
          this.deleteTask(rowData.taskId);
        }
      })
    }
  }

  async deleteTask(taskId: any) {
    this.loading = true;
    await this.projectService.deleteTask(taskId).subscribe(async response => {
      let result = <any>response;

      this.loading = false;
      if (result.statusCode === 200 || result.statusCode === 202) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa công việc thành công!' };
        this.showMessage(msg);
        await this.reloadProjectTask();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
      this.loading = false;
    });
  }

  // Thêm mới công việc
  add_Task() {
    let width = '';
    if (window.innerWidth < 500) {
      width = '100%';
    } else {
      width = '80%';
    }
    // Nếu không chọn node nào thì mặc định node đầu tiên
    let ref = this.dialogService.open(AddTaskScopeDialogComponent, {
      data: {
        projectId: this.projectId,
        projectScope: this.selectedScope == null || this.selectedScope == undefined ? this.data[0] : this.selectedScope
      },
      header: 'Thêm công việc',
      width: width,
      height: '100%',
      baseZIndex: 1030,
      contentStyle: {
        "overflow-x": "hidden",
        "min-height": "300px",
        "max-height": "800px",
        "overflow": "true",
        "overflow-y": "auto"
      }
    });
    ref.onClose.subscribe(async (result: any) => {
      if (result) {
        if (result.status === true) {
          this.isDelete = false;
          this.ref.close();
          await this.reloadProjectTask();
        }
      }
    });
  }

  async reloadProjectTask() {
    let result: any = await this.projectService.getProjectScope(this.projectId);
    this.loading = false;
    if (result.statusCode == 200) {
      this.listProjectTasks = result.listProjectTask || [];
      this.listProjectTasksDefault = result.listProjectTask || [];
      if (this.listProjectTasksDefault.length > 0) {
        if (this.lstNode.length > 0) {
          // Danh sách công việc thuộc gói node và con của node
          this.listProjectTasks = this.listProjectTasksDefault.filter(x => this.lstNode.find(a => a == x.projectScopeId));
        }
      }
      this.handleEmployeeContent();
      this.noteHistory = result.listNote;
      this.handleNoteContent();
      this.totalPercent = result.project.taskComplate.toFixed(2);
      this.totalEstimateTime = result.project.estimateCompleteTime;
      this.listProjectScopes = result.listProjectScope || [];
      this.data = this.list_to_tree();
      this.loading = false;
    }
  }

  // Chỉnh sửa công việc
  edit_Task(rowData: any) {
    let ref = this.dialogService.open(AddTaskScopeDialogComponent, {
      data: {
        taskId: rowData.taskId,
        projectId: this.projectId,
        projectScope: this.listProjectScopes.filter(x => x.projectScopeId == rowData.projectScopeId)[0]
      },
      header: 'Chỉnh sửa công việc',
      width: '80%',
      height: '100%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px",
        "max-height": "800px",
        "overflow-x": "hidden",
        "overflow": "true",
        "overflow-y": "auto"
      }
    });
    ref.onClose.subscribe(async (result: any) => {
      if (result) {
        if (result.status === true) {
          this.ref.close();
          await this.reloadProjectTask();
        }
      }
    });
  }

  //#region  Thao tác cho hạng mục

  // Xóa hạng mục
  delete_projectResource() {
    this.confirmationService.confirm({
      header: "Xác nhận",
      message: 'Bạn có chắc chắn muốn xóa hạng mục dự án này?',
      accept: () => {
        this.deleteProjectScope();
      }
    })
  }


  deleteProjectScope() {
    this.projectService.deleteProjectScope(this.selectedNode.data.projectScopeId, this.projectId, this.data[0].data.projectScopeId).subscribe(async response => {
      let result: any = response;

      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa hạng mục thành công' };
        this.showMessage(msg);
        await this.getMasterData();
        this.getListWorkPackage();
        this.actionDeleteScope = false;
        this.actionEditScope = false;
        this.selectedNode = null;
        this.clearData();
        //
        // this.noteHistory.forEach(note => {
        //   var des = $.parseHTML(note.description);
        //   note.description = des[0].innerText;
        // });
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  // Chỉnh sửa hạng mục
  editProjectScope() {
    if (this.selectedScopeId != undefined && this.selectedScopeId != null) {
      // check xem da co nguon luc voi node da chon hay chua
      // Chưa có nguồn lực thì enable và bắt require
      if (this.selectedScope.resourceType == null || this.selectedScope.resourceType == undefined) {
        this.enableControls(true);
      }
      else {
        this.enableControls();
      }
      this.isEdit = true;
    }
    else {
      this.clearToast();
      this.showToast('error', 'Thông báo', "Vui lòng chọn hạng mục/ gói công việc");
    }
  }

  // Lưu Thay doi thong tin hang muc
  saveProjectScope() {
    if (this.scopeProjectForm.touched) {
      let projectScopes = new Array<ProjectScopeModel>();
      projectScopes = this.listProjectScopes.filter(p => p.ischange == true);

      if (this.selectedScopeId !== undefined || projectScopes.length > 0) {
        if (!this.scopeProjectForm.valid) {
          Object.keys(this.scopeProjectForm.controls).forEach(key => {
            if (!this.scopeProjectForm.controls[key].valid) {
              this.scopeProjectForm.controls[key].markAsTouched();
              this.scopeProjectForm.controls["resourceTypeControl"].markAsTouched();
            }
          });
          this.isInvalidForm = true;
          this.isOpenNotifiError = true;
        }
        else {
          this.isInvalidForm = false;
          this.isOpenNotifiError = false;
          if (this.selectedScopeId !== null) {
            this.selectedScope.projectId = this.projectId;
            this.selectedScope.description = this.scopeProjectForm.get('projectScopeDescriptionControl').value;
            this.selectedScope.projectScopeName = this.scopeProjectForm.get('projectScopeNameControl').value;
            let sourceType = this.scopeProjectForm.get('resourceTypeControl').value;
            if (sourceType != null)
              this.selectedScope.resourceType = sourceType.categoryId;
            projectScopes[0] = this.selectedScope;
          }
        }
      }

      if (projectScopes.length > 0) {

        let projectScopeName = this.projectScopeNameControl.value;

        this.loading = true;

        this.projectService.updateProjectScope(projectScopes[0], this.auth.UserId, this.data[0].data.projectScopeId).subscribe(response => {
          let result: any = response;

          if (result.statusCode === 202 || result.statusCode === 200) {
            this.noteHistory = result.listNote;
            this.selectedNode.label = projectScopeName;
            this.listProjectScopes = result.listProjectScope;
            this.data = this.list_to_tree();
            this.handleNoteContent();
            this.isDelete = false;
            this.loading = false;
            this.isEdit = false;

            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật hạng mục/ gói công việc thành công' };
            this.showMessage(msg);
          }
        });
        this.disableControls();
      }
    }
  }

  // disable control Hạng mục
  disableControls() {
    this.scopeProjectForm.get('projectScopeNameControl').disable();
    this.isReadonly = true
    this.scopeProjectForm.get('resourceTypeControl').disable();
  }

  // enable control Hạng mục
  enableControls(isExitsResource: boolean = false) {
    if (isExitsResource)
      this.scopeProjectForm.get('resourceTypeControl').enable();
    else
      this.scopeProjectForm.get('resourceTypeControl').disable();
    this.scopeProjectForm.get('projectScopeNameControl').enable();
    this.isReadonly = false
  }
  //#endregion

  createGUID() {
    var u = '', i = 0;
    while (i++ < 36) {
      var c = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'[i - 1], r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      u += (c == '-' || c == '4') ? c : v.toString(16)
    }
    return u;
  }

  //#region Tác vụ xử lý trên cây Hạng mục
  // Thêm mới hạng mục
  async addProjectScope() {

    var parentId: any;
    var parentName: any;
    var parentCode: any;
    // Check xem có lựa chọn node nào hay chưa
    if (this.selectedNode == undefined || this.selectedScope == null) {
    }
    else {
      parentId = this.selectedScope == null || this.selectedNode == undefined ? null : this.selectedScope.projectScopeId;

      if (parentId == null) {
        parentName = this.listProjectScopes.filter(x => x.projectScopeId == this.selectedScope.projectScopeId)[0].projectScopeName;
        parentCode = this.listProjectScopes.filter(x => x.projectScopeId == this.selectedScope.projectScopeId)[0].projectScopeCode;
      }
      else {
        parentName = this.listProjectScopes.filter(x => x.projectScopeId == parentId)[0].projectScopeName;
      }
    }

    let ref = this.dialogService.open(AddProjectScopeDialogComponent, {
      data: {
        parentId: parentId,
        listResourceType: this.listResourceType,
        parentName: parentName,
        listVendor: this.listVendor,
        projectScopeCode: parentCode,
        level: this.levelScope,
        projectId: this.projectId
      },
      header: 'Thông tin gói công việc',
      width: '80%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "400px",
        "max-height": "700px",
        "overflow-x": "hidden"
      }
    });

    ref.onClose.subscribe(async (result: any) => {

      if (result) {
        if (result.status === true) {
          this.loading = true;
          let newScope = result.scopeModel;

          let newProjectScope: ProjectScopeModel = new ProjectScopeModel();
          let count = this.listProjectScopes.filter(p => p.parentId == newScope.parentScopeId).length + 1;
          newProjectScope.parentId = newScope.parentScopeId;
          newProjectScope.projectId = this.projectId;
          //newProjectScope.projectScopeId = this.createGUID();
          newProjectScope.projectScopeName = newScope.projectScopeName;

          if (newProjectScope.parentId == undefined) newProjectScope.projectScopeCode = '';
          else newProjectScope.projectScopeCode = newScope.parentScopeCode == '' || newScope.parentScopeCode == undefined ? count.toString() : newScope.parentScopeCode + '.' + count;
          newProjectScope.description = newScope.projectScopeDescription;
          newProjectScope.ischange = true;
          newProjectScope.level = newScope.levelScope;
          newProjectScope.resourceType = newScope.resourceType;

          if (newScope.isExternal == 1) {
            newProjectScope.supplyId = newScope.supplyId;
            newProjectScope.listTask = [];
          }
          else
            newProjectScope.listTask = newScope.listTaskId;

          // Lưu vào DB
          await this.projectService.updateProjectScope(newProjectScope, this.auth.UserId, this.data[0].data.projectScopeId).subscribe(async response => {
            this.loading = false;
            let result = <any>response;

            if (result.statusCode === 202 || result.statusCode === 200) {
              this.clearToast();
              this.isEdit = false;

              await this.getMasterData();
              this.selectedNode = null;
              this.selectedScope = null;
              this.showToast('success', 'Thông báo', 'Thêm hạng mục/ gói công việc thành công');
            }
            else {
              this.clearToast();
              this.showToast('error', 'Thông báo', result.messageCode);
            }
            this.loading = false;
          }, error => { this.loading = false; });
        }
      }
      else
        this.loading = false;
    });
    // }
    this.clearData();
  }


  // Lựa chọn 1 node trong cây hạng mục
  async onNodeSelect(event) {
    this.actionEditScope = true;
    this.actionDeleteScope = event.node.children.length == 0;
    this.selectedScope = event.node.data;
    if (this.selectedScopeId != this.selectedScope.projectScopeId)
      this.isEdit = false;
    if (this.selectedScope.parentId == null || this.selectedScope.parentId == undefined)
      this.isDelete = false;

    this.selectedScopeId = this.selectedScope.projectScopeId;
    this.projectScopeNameControl.setValue(this.selectedScope.projectScopeName);

    let toSelectResource: CategoryModel = this.listResourceType.find(x => x.categoryId == this.selectedScope.resourceType);
    this.resourceTypeControl.setValue(toSelectResource);
    this.projectScopeDescriptionControl.setValue(this.selectedScope.description);
    this.parentName = this.listProjectScopes.find(x => x.projectScopeId == this.selectedScope.parentId) == null ? "" : this.listProjectScopes.find(x => x.projectScopeId == this.selectedScope.parentId).projectScopeName;
    this.listResourceScope = this.listProjectScopes.find(x => x.projectScopeId == this.selectedScopeId).listEmployee;

    this.levelScope = event.node.data.level;

    this.listWorkpackage = [];
    if (this.listProjectTasksDefault.length > 0) {

      this.lstNode = [];
      // Danh sách công việc thuộc con của node đấy
      //this.recursiveNodeScope(event.node);
      this.lstNode.push(event.node.data.projectScopeId);
      let listScopeId: Array<string> = this.listProjectScopes.filter(x => x.parentId == event.node.data.projectScopeId).map(c => c.projectScopeId);

      while (listScopeId.length != 0) {
        this.lstNode.push.apply(this.lstNode, listScopeId);
        listScopeId = this.listProjectScopes.filter(x => listScopeId.includes(x.parentId)).map(c => c.projectScopeId);
      }

      // Danh sách công việc thuộc gói node và con của node
      this.listProjectTasks = this.listProjectTasksDefault.filter(x => this.lstNode.find(a => a == x.projectScopeId));
    }
    this.handleEmployeeContent();
    this.listProjectTasks.forEach(item => {
      this.listProjectScopes.filter(x => x.projectScopeId == item.projectScopeId).forEach(scope => {
        var obj = new ProjectScopeFillter();
        obj.projectSopeId = scope.projectScopeId;
        obj.projectScopeName = scope.projectScopeName;
        this.listWorkpackage.push(obj);
      });
    });

    // Check xem node có con hay không
    // Node cuối hiển thị 2 lựa chọn
    if (this.listProjectScopes.filter(x => x.parentId == this.selectedScope.projectScopeId).length == 0 && this.listProjectTasks.length == 0 && this.selectedScope.parentId != null && this.selectedScope.parentId != undefined)
      this.isDelete = true;
    else {
      // chỉ hiển thị thêm gói công việc
      this.isDelete = false;
    }
    if (this.listProjectScopes.filter(x => x.parentId == this.selectedScope.projectScopeId).length == 0)
      this.setForm(0);
    else
      this.setForm(1);
    this.disableControls();

    //Tính thời gian ước lượng công việc của Hạng mục
    let result: any = await this.projectService.getThoiGianUocLuongHangMuc(this.selectedScope.projectScopeId);
    if (result.statusCode != 200) {
      this.clearToast();
      this.showToast('error', 'Thông báo', result.messageCode);
    }

    this.thoiGianUocLuong = result.thoiGianUocLuong;
  }

  // Bỏ chọn node trong cây hạng mục
  onNodeUnselect(event) {

    this.selectedNode = null;
    this.parentName = null;
    this.selectedScopeId = null;
    this.actionDeleteScope = false;
    this.actionEditScope = false;
    this.selectedScopeName = null;
    this.isEdit = false;
    this.scopeProjectForm.reset();
    this.listWorkpackage = [];
    this.listWorkpackage = [];
    this.selectedScope = null;
    this.listResourceScope = [];
    this.thoiGianUocLuong = 0;
    this.setForm(0);
    this.isDelete = false;
    if (this.listProjectTasksDefault.length > 0) {
      this.lstNode = [];
      // Danh sách công việc thuộc con của node đấy
      //this.recursiveNodeScope(this.data[0]);
      this.lstNode.push(this.data[0].data.projectScopeId);
      // Danh sách công việc thuộc gói node và con của node
      this.listProjectTasks = this.listProjectTasksDefault.filter(x => this.lstNode.find(a => a == x.projectScopeId));
    }

    this.listProjectTasks.forEach(item => {
      this.listProjectScopes.filter(x => x.projectScopeId == item.projectScopeId).forEach(scope => {
        var obj = new ProjectScopeFillter();
        obj.projectSopeId = scope.projectScopeId;
        obj.projectScopeName = scope.projectScopeName;
        this.listWorkpackage.push(obj);
      });
    });

    this.disableControls();
  }
  //#endregion


  // Clear dữ liệu có trong các ô nhập liệu
  clearData() {
    this.selectedNode = null;
    this.parentName = null;
    this.selectedScopeId = null;
    this.actionDeleteScope = false;
    this.actionEditScope = false;
    this.isEdit = false;
    this.scopeProjectForm.reset();
    this.listWorkpackage = [];
    this.selectedScope = null;
    this.listResourceScope = [];
    this.isDelete = false;
  }

  // Nút thoát
  cancel() {
    this.confirmationService.confirm({
      message: 'Bạn có muốn lưu các thay đổi?',
      accept: () => {
        // Chuyển về thông tin dự án
        this.router.navigate(['/project/detail', { 'projectId': this.projectId }]);
      }
    });
  }

  onRowSelect(dataRow) {
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clearToast() {
    this.messageService.clear();
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  // TÌm kiếm bộ lọc
  searchTask() {
    let listStatusId: Array<string> = [];
    if (this.selectedStatus) {
      listStatusId = this.selectedStatus.map(c => c.categoryId);
    }
    let listWorkpackageId: Array<string> = [];
    if (this.selectedWorkpackage) {
      listWorkpackageId = this.selectedWorkpackage.map(c => c.projectSopeId);
    }

    let ListEmployeeId: Array<string> = [];
    if (this.selectedPic) {
      ListEmployeeId = this.selectedPic.map(c => c.employeeId);
    }

    let fromPercent = this.fromPercent == null ? 0 : this.fromPercent;
    let toPercent = this.toPercent == null ? 0 : this.toPercent;

    let fromStartDate = this.startFromDate;
    if (fromStartDate) {
      fromStartDate.setHours(0, 0, 0, 0);
      fromStartDate = convertToUTCTime(fromStartDate);
    }
    let toStartDate: Date = this.startToDate;
    if (toStartDate) {
      toStartDate.setHours(23, 59, 59, 999);
      toStartDate = convertToUTCTime(toStartDate);
    }

    let fromEndDate = this.endFromDate;
    if (fromEndDate) {
      fromEndDate.setHours(0, 0, 0, 0);
      fromEndDate = convertToUTCTime(fromEndDate);
    }
    let toEndDate: Date = this.endToDate;
    if (toEndDate) {
      toEndDate.setHours(23, 59, 59, 999);
      toEndDate = convertToUTCTime(toEndDate);
    }
    this.loading = true;
    this.projectService.searchTaskFromProjectScope(this.projectId, listStatusId, listWorkpackageId, ListEmployeeId, fromStartDate, toStartDate,
      fromEndDate, toEndDate, fromPercent, toPercent, this.auth.UserId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.listProjectTasks = result.listTask || [];

          if (this.listProjectTasksDefault.length > 0) {
            if (this.lstNode.length > 0) {
              // Danh sách công việc thuộc gói node và con của node
              this.listProjectTasks = this.listProjectTasks.filter(x => this.lstNode.find(a => a == x.projectScopeId));
            }
          }
          this.handleEmployeeContent();

          if (this.listProjectTasks.length == 0) {
            this.clearToast();
            this.showToast('warn', 'Thông báo', "Không tìm thấy bản ghi nào!");
          }
        } else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      });
  }


  /*Event thay đổi nội dung ghi chú*/
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  /*Xử lý và hiển thị lại nội dung ghi chú*/
  async handleNoteContent() {
    this.loading = true;
    this.noteHistory.forEach(element => {
      setTimeout(() => {
        let count = 0;
        if (element.description == null) {
          element.description = "";
        }

        let des = $.parseHTML(element.description);
        let newTextContent = '';
        for (let i = 0; i < des.length; i++) {
          count += des[i].textContent.length;
          newTextContent += des[i].textContent;
        }

        if (count > 250) {
          newTextContent = newTextContent.substr(0, 250) + '<b>...</b>';
          $('#' + element.noteId).find('.short-content').append($.parseHTML(newTextContent));
        } else {
          $('#' + element.noteId).find('.short-content').append($.parseHTML(element.description));
        }

        $('#' + element.noteId).find('.full-content').append($.parseHTML(element.description));
      }, 1000);
    });
    this.loading = false;
  }
  /*End*/

  /*Xử lý và hiển thị lại người phụ trách*/
  handleEmployeeContent() {
    if (this.listProjectTasks.length > 0) {
      this.listProjectTasks.forEach(element => {
        setTimeout(() => {
          if (element.employee == null) {
            element.employee = "";
          }

          let des = element.employee.split(',');
          let content = '';
          let newTextContent = '';
          for (let i = 0; i < des.length; i++) {
            content = des[i].trimStart();

            if (i === des.length - 1) {
              if (des[i].length > 30)
                newTextContent += content.substr(0, 30) + '<b>...</b>' + "</br>";
              else
                newTextContent += content;
            }
            else {
              if (des[i].length > 30)
                newTextContent += content.substr(0, 30) + '<b>...</b>' + "," + "</br>";
              else {
                newTextContent += content + "," + "</br>";
              }
            }
          }
          $('#' + element.taskId).text('');
          $('#' + element.taskId).append($.parseHTML(newTextContent));
          this.loading = false;
        }, 1000);
      });
    }
  }

  /*End*/

  /*Event Sửa ghi chú*/
  onClickEditNote(noteId: string, noteDes: string) {
    this.noteContent = noteDes;
    this.noteId = noteId;
    this.listUpdateNoteDocument = [];//this.noteHistory.find(x => x.noteId == this.noteId).noteDocList;
    this.isEditNote = true;
  }
  /*End*/

  /*Event Xóa ghi chú*/
  onClickDeleteNote(noteId: string) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa ghi chú này?',
      accept: () => {
        this.loading = true;
        this.noteService.disableNote(noteId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            let note = this.noteHistory.find(x => x.noteId == noteId);
            let index = this.noteHistory.lastIndexOf(note);
            this.noteHistory.splice(index, 1);
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Xóa ghi chú thành công');
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    });
  }
  /*End*/

  /*Kiểm tra noteText > 250 ký tự hoặc noteDocument > 3 thì ẩn đi một phần nội dung*/
  tooLong(note): boolean {
    // if (note.noteDocList.length > 3) return true;
    var des = $.parseHTML(note.description);
    var count = 0;
    for (var i = 0; i < des.length; i++) {
      count += des[i].textContent.length;
      if (count > 250) return true;
    }
    return false;
  }

  /*Event Mở rộng/Thu gọn nội dung của ghi chú*/
  toggle_note_label: string = 'Mở rộng';
  trigger_node(nodeid: string, event) {
    // noteContent
    let shortcontent_ = $('#' + nodeid).find('.short-content');
    let fullcontent_ = $('#' + nodeid).find('.full-content');
    if (shortcontent_.css("display") === "none") {
      fullcontent_.css("display", "none");
      shortcontent_.css("display", "block");
    } else {
      fullcontent_.css("display", "block");
      shortcontent_.css("display", "none");
    }
    // noteFile
    let shortcontent_file = $('#' + nodeid).find('.short-content-file');
    let fullcontent_file = $('#' + nodeid).find('.full-content-file');
    let continue_ = $('#' + nodeid).find('.continue')
    if (shortcontent_file.css("display") === "none") {
      continue_.css("display", "block");
      fullcontent_file.css("display", "none");
      shortcontent_file.css("display", "block");
    } else {
      continue_.css("display", "none");
      fullcontent_file.css("display", "block");
      shortcontent_file.css("display", "none");
    }
    let curr = $(event.target);

    if (curr.attr('class').indexOf('pi-chevron-right') != -1) {
      this.toggle_note_label = 'Thu gọn';
      curr.removeClass('pi-chevron-right');
      curr.addClass('pi-chevron-down');
    } else {
      this.toggle_note_label = 'Mở rộng';
      curr.removeClass('pi-chevron-down');
      curr.addClass('pi-chevron-right');
    }
  }
  /*End */

  /*Hủy sửa ghi chú*/
  cancelNote() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn hủy ghi chú này?',
      accept: () => {
        this.noteId = null;
        this.noteContent = null;
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.listUpdateNoteDocument = [];
        this.isEditNote = false;
      }
    });
  }

  async uploadFilesAsync(files: File[]) {
    await this.imageService.uploadFileAsync(files);
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
  /*Lưu file và ghi chú vào Db*/
  async saveNote() {

    this.loading = true;
    this.listNoteDocumentModel = [];
    /*Upload file mới nếu có*/
    if (this.uploadedFiles.length > 0) {
      await this.uploadFilesAsync(this.uploadedFiles);
      for (var x = 0; x < this.uploadedFiles.length; ++x) {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = this.uploadedFiles[x].name;
        noteDocument.DocumentSize = this.uploadedFiles[x].size.toString();
        this.listNoteDocumentModel.push(noteDocument);
      }
    }
    let noteModel = new NoteModel();
    if (!this.noteId) {
      /*Tạo mới ghi chú*/
      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'NEW';
      noteModel.ObjectId = this.projectId;
      noteModel.ObjectType = 'PROSCOPE';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.auth.UserId;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/
      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'EDT';
      noteModel.ObjectId = this.projectId;
      noteModel.ObjectType = 'PROSCOPE';
      noteModel.NoteTitle = 'đã sửa ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.auth.UserId;
      noteModel.CreatedDate = new Date();
      //Thêm file cũ đã lưu nếu có
      // this.listUpdateNoteDocument.forEach(item => {
      //   let noteDocument = new NoteDocumentModel();
      //   noteDocument.DocumentName = item.documentName;
      //   noteDocument.DocumentSize = item.documentSize;
      //   this.listNoteDocumentModel.push(noteDocument);
      // });
    }
    this.noteService.createNoteForProjectScope(noteModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.noteContent = null;
        this.listUpdateNoteDocument = [];
        this.noteId = null;
        this.isEditNote = false;
        this.paginator.changePage(0);
        this.clearToast();
        this.showToast('success', 'Thông báo', "Lưu ghi chú thành công!");
        //this.fileUploadNote.clear();
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  showProjectInfor() {
    this.isShow = !this.isShow;
    if (this.isShow) {
      window.scrollTo(0, 0)
    }
  }

  copyProjectResource() {
    /* check xem hạng mục hiện tại có task hay chưa
    Nếu chưa thì cho phép copy
    Nếu có task rồi thì show thông báo có rồi
*/

    if (this.listProjectTasks.length > 0) {
      this.clearToast();
      this.showToast('warn', 'Thông báo', "Hạng mục có công việc không thể copy từ hạng mục khác!");
    }
    else {

      this.loading = true;
      this.projectControl = new FormControl(null);
      this.cloneProjectScope = new FormGroup({
        projectControl: this.projectControl,

      });
      this.newProjectId = null;
      this.dataCopy = null;
      this.displayCopyScope = true;
      this.getCopyScopeMasterData();
      this.loading = false;
    }
  }

  getCopyScopeMasterData() {
    this.projectService.getMasterDataListCloneProjectScope().subscribe(response => {
      let result: any = response;
      if (result.statusCode == 200) {
        this.listProjectScopes = result.listProjectScope;
        this.listProject = result.listProject.filter(x => x.projectId !== this.projectId);
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  async saveCopy() {
    // check permission có được xóa hay không

    // show confirm có chắc chắn muốn copy
    this.confirmationService.confirm({
      message: 'Tất cả hạng mục hiện tại sẽ bị xóa bỏ, bạn có chắc muốn tiếp tục copy',

      accept: async () => {
        // save hạng mục vào dự án mới

        this.loading = true;
        let resultClone: any = await this.projectService.cloneProjectScope(this.newProjectId, this.projectId, this.auth.UserId);
        if (resultClone.statusCode == 200) {
          this.confirmationService.close();
          await this.getMasterData();
          this.displayCopyScope = false;
        }
        else {
          this.confirmationService.close();
        }
        this.ref.close();
        this.loading = false;
      },
      reject: () => {
        this.confirmationService.close();
      }
    });
  }

  changeProject(event: any) {

    if (event.value != null) {
      this.newProjectId = event.value.projectId;
      this.dataCopy = this.list_to_tree(event.value.projectId);
    }
    else {
      this.newProjectId = null;
      this.dataCopy = null;
    }
  }

  cancelCopy() {
    this.confirmationService.confirm({
      message: 'Bạn có muốn hủy bỏ các thay đổi?',
      accept: () => {
        this.displayCopyScope = false;
      }
    });
  }

  async paginate(event) {
    //event.first = Index of the first record
    //event.rows = Number of rows to display in new page
    //event.page = Index of the new page
    //event.pageCount = Total number of pages

    let result: any = await this.projectService.pagingProjectNote(this.projectId, event.rows, event.page, "PROSCOPE");
    if (result.statusCode == 200) {

      this.noteHistory = result.noteEntityList;
      this.totalRecordsNote = result.totalRecordsNote;
      await this.handleNoteContent();
    }
  }

  onChangeProject(projectId: string) {
    this.router.navigate(['/project/scope', { 'projectId': projectId }]);
  }
}


function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
