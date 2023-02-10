import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxLoadingModule } from 'ngx-loading';
import { TreeTableModule } from 'primeng/treetable';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { DragDropModule } from 'primeng/dragdrop';
import { DataViewModule } from 'primeng/dataview';
import { FullCalendarModule } from 'primeng/fullcalendar';

import { ProjectRouting } from './project.routing';
import { ProjectComponent } from './project.component';
import { ProjectListComponent } from '../project/components/project-list/project-list.component';
import { ContactService } from '../shared/services/contact.service';
import { ProjectService } from '../project/services/project.service';
import { GetPermission } from '../shared/permission/get-permission';
import { SystemParameterService } from '../admin/services/system-parameter.service';
import { CurrencyPipe } from '@angular/common';
import { EmailConfigService } from '../admin/services/email-config.service';
import { NoteService } from '../shared/services/note.service';
import { ImageUploadService } from '../shared/services/imageupload.service';
import { ProductCategoryService } from '../admin/components/product-category/services/product-category.service';

import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ProjectCreateComponent } from './components/project-create/project-create.component';
import { AddProjectTargetDialogComponent } from './components/add-project-target-dialog/add-project-target-dialog.component';
import { ProjectDetailComponent } from './components/project-detail/project-detail.component';
import { ProjectResourceComponent } from './components/project-resource/project-resource.component';
import { ProjectScopeComponent } from './components/project-scope/project-scope.component';
import { AddProjectScopeDialogComponent } from './components/add-project-scope-dialog/add-project-scope-dialog.component';
import { CreateProjectTaskComponent } from './components/create-project-task/create-project-task.component';
import { DetailProjectTaskComponent } from './components/detail-project-task/detail-project-task.component';
import { AddProjectResourceDialogComponent } from './components/add-project-resource-dialog/add-project-resource-dialog.component';
import { TimeSheetDialogComponent } from './components/time-sheet-dialog/time-sheet-dialog.component';
import { TaskProjectScopeDialogComponent } from './components/task-project-scope-dialog/task-project-scope-dialog.component';
import { TopHeaderProjectComponent } from './components/top-header-project/top-header-project.component';
import { MenuLeftProjectComponent } from './components/menu-left-project/menu-left-project.component';
import { ListTaskComponent } from './components/list-task/list-task.component';
import { AddTaskScopeDialogComponent } from './components/add-task-scope-dialog/add-task-scope-dialog.component';
import { AddConstaintDialogComponent } from './components/add-constaint-dialog/add-constaint-dialog.component';
import { ProjectMilestoneComponent } from './components/project-milestone/project-milestone.component';
import { CreateOrUpdateMilestoneDialogComponent } from './components/create-or-update-milestone-dialog/create-or-update-milestone-dialog.component';
import { AddOrRemoveTaskMilestoneDialogComponent } from './components/add-or-remove-task-milestone-dialog/add-or-remove-task-milestone-dialog.component';
import { ProjectDocumentComponent } from './components/project-document/project-document.component';
import { ProjectHeaderComponent } from './components/project-header/project-header.component';
import { SearchTimeSheetComponent } from './components/search-time-sheet/search-time-sheet.component';
import { SetIconTaskType } from './setIcon/setIcon';
import { DashboardProjectComponent } from './components/dashboard-project/dashboard-project.component';
import { ProjectInformationPanelComponent } from './components/project-information-panel/project-information-panel.component';
import { BaoCaoSuDungNguonLucComponent } from './components/bao-cao-su-dung-nguon-luc/bao-cao-su-dung-nguon-luc.component';
import { BaoCaoTongHopCacDuAnComponent } from './components/bao-cao-tong-hop-cac-du-an/bao-cao-tong-hop-cac-du-an.component';
import { AddRelateDialogComponent } from './components/add-relate-dialog/add-relate-dialog.component';


@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    ProjectRouting,
    FormsModule,
    ReactiveFormsModule,
    TreeTableModule,
    OverlayPanelModule,
    DragDropModule,
    DataViewModule,
    FullCalendarModule,
    NgxLoadingModule.forRoot({}),
  ],
  declarations: [
    ProjectComponent,
    ProjectCreateComponent,
    ProjectListComponent,
    AddProjectTargetDialogComponent,
    ProjectDetailComponent,
    ProjectResourceComponent,
    ProjectScopeComponent,
    AddProjectScopeDialogComponent,
    CreateProjectTaskComponent,
    DetailProjectTaskComponent,
    AddProjectResourceDialogComponent,
    TimeSheetDialogComponent,
    TaskProjectScopeDialogComponent,
    TopHeaderProjectComponent,
    MenuLeftProjectComponent,
    ListTaskComponent,
    AddTaskScopeDialogComponent,
    ProjectMilestoneComponent,
    CreateOrUpdateMilestoneDialogComponent,
    AddOrRemoveTaskMilestoneDialogComponent,
    AddConstaintDialogComponent,
    ProjectDocumentComponent,
    ProjectHeaderComponent,
    SearchTimeSheetComponent,
    DashboardProjectComponent,
    ProjectInformationPanelComponent,
    BaoCaoSuDungNguonLucComponent,
    BaoCaoTongHopCacDuAnComponent,
    AddRelateDialogComponent
  ],
  entryComponents: [
    AddProjectTargetDialogComponent,
    TimeSheetDialogComponent,
    TaskProjectScopeDialogComponent,
    AddConstaintDialogComponent,
    CreateOrUpdateMilestoneDialogComponent,
    AddProjectResourceDialogComponent,
    AddTaskScopeDialogComponent,
    AddProjectScopeDialogComponent,
    AddOrRemoveTaskMilestoneDialogComponent,
    AddRelateDialogComponent
  ],
  providers: [
    ContactService,
    ProjectService,
    GetPermission,
    SystemParameterService,
    CurrencyPipe,
    EmailConfigService,
    MessageService,
    ConfirmationService,
    DialogService,
    NoteService,
    ImageUploadService,
    ProductCategoryService,
    SetIconTaskType
  ],
})
export class ProjectModule { }
