import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthGuard } from '../shared/guards/auth.guard';

import { ProjectComponent } from './project.component';
import { ProjectListComponent } from '../project/components/project-list/project-list.component';
import { ProjectCreateComponent } from '../project/components/project-create/project-create.component';
import { ProjectDetailComponent } from './components/project-detail/project-detail.component';
import { ProjectResourceComponent } from './components/project-resource/project-resource.component';
import { ProjectScopeComponent } from './components/project-scope/project-scope.component';
import { CreateProjectTaskComponent } from './components/create-project-task/create-project-task.component';
import { DetailProjectTaskComponent } from './components/detail-project-task/detail-project-task.component';
// import { ProjectLayoutComponent } from './components/project-layout/project-layout.component';
import { ListTaskComponent } from './components/list-task/list-task.component';
import { ProjectMilestoneComponent } from './components/project-milestone/project-milestone.component';
import { ProjectDocumentComponent } from './components/project-document/project-document.component';
import { SearchTimeSheetComponent } from './components/search-time-sheet/search-time-sheet.component';
import { DashboardProjectComponent } from './components/dashboard-project/dashboard-project.component';
import { BaoCaoSuDungNguonLucComponent } from './components/bao-cao-su-dung-nguon-luc/bao-cao-su-dung-nguon-luc.component';
import { BaoCaoTongHopCacDuAnComponent } from './components/bao-cao-tong-hop-cac-du-an/bao-cao-tong-hop-cac-du-an.component';

// import { ProjectMilestoneComponent } from './components/project-milestone/project-milestone.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: ProjectComponent,
        children: [
          {
            path: 'list',
            component: ProjectListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'create',
            component: ProjectCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'bc-su-dung-nguon-luc',
            component: BaoCaoSuDungNguonLucComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'bc-tong-hop-cac-du-an',
            component: BaoCaoTongHopCacDuAnComponent,
            canActivate: [AuthGuard]
          },
        ]
      },
      {
        path: 'detail',
        component: ProjectDetailComponent,
        children: []
      },
      {
        path: 'create-project-task',
        component: CreateProjectTaskComponent,
        children: []
      },
      {
        path: 'detail-project-task',
        component: DetailProjectTaskComponent,
        children: []
      },
      {
        path: 'list-project-task',
        component: ListTaskComponent,
        children: []
      },
      {
        path: 'resource',
        component: ProjectResourceComponent,
        children: []
      },
      {
        path: 'scope',
        component: ProjectScopeComponent,
        children: []
      },
      {
        path: 'milestone',
        component: ProjectMilestoneComponent,
        children: []
      },
      {
        path: 'document',
        component: ProjectDocumentComponent,
        children: []
      },
      {
        path: 'search-time-sheet',
        component: SearchTimeSheetComponent,
        children: []
      },
      {
        path: 'dashboard',
        component: DashboardProjectComponent,
        children: []
      },
      // {
      //   path: 'bc-su-dung-nguon-luc',
      //   component: BaoCaoSuDungNguonLucComponent,
      //   children: []
      // },
      // {
      //   path: 'bc-tong-hop-cac-du-an',
      //   component: BaoCaoTongHopCacDuAnComponent,
      //   children: []
      // },
    ])
  ],
  exports: [
    RouterModule
  ]
})
export class ProjectRouting { }
