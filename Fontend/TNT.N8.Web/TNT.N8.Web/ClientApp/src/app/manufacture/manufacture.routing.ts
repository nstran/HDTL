import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthGuard } from '../shared/guards/auth.guard';

import { ManufactureComponent } from './manufacture.component';
import { ProductionOrderCreateComponent } from './component/production-order-create/production-order-create.component';
import { ProductionOrderListComponent } from './component/production-order-list/production-order-list.component';
import { ProductionOrderDetailComponent } from './component/production-order-detail/production-order-detail.component';
import { ProductOrderWorkflowCreateComponent } from './component/product-order-workflow-create/product-order-workflow-create.component';
import { ProductOrderWorkflowDetailComponent } from './component/product-order-workflow-detail/product-order-workflow-detail.component';
import { ProductOrderWorkflowListComponent } from './component/product-order-workflow-list/product-order-workflow-list.component';
import { TechniqueRequestCreateComponent } from './component/technique-request-create/technique-request-create.component';
import { TechniqueRequestDetailComponent } from './component/technique-request-detail/technique-request-detail.component';
import { TechniqueRequestListComponent } from './component/technique-request-list/technique-request-list.component';
import { TotalProductionOrderCreateComponent } from './component/total-production-order-create/total-production-order-create.component';
import { TotalProductionOrderListComponent } from './component/total-production-order-list/total-production-order-list.component';
import { TotalProductionOrderDetailComponent } from './component/total-production-order-detail/total-production-order-detail.component';
import { TrackProductionComponent } from './component/track-production/track-production.component';
import { ReportQuanlityControlComponent } from './component/report-quanlity-control/report-quanlity-control.component';
import { ReportManufactureComponent } from './component/report-manufacture/report-manufacture.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: ManufactureComponent,
        children: [
          {
            path: 'production-order/list',
            component: ProductionOrderListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'production-order/create',
            component: ProductionOrderCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'production-order/detail',
            component: ProductionOrderDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'product-order-workflow/create',
            component: ProductOrderWorkflowCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'product-order-workflow/detail',
            component: ProductOrderWorkflowDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'product-order-workflow/list',
            component: ProductOrderWorkflowListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'technique-request/create',
            component: TechniqueRequestCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'technique-request/detail',
            component: TechniqueRequestDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'technique-request/list',
            component: TechniqueRequestListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'total-production-order/create',
            component: TotalProductionOrderCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'total-production-order/detail',
            component: TotalProductionOrderDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'total-production-order/list',
            component: TotalProductionOrderListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'track-production/track',
            component: TrackProductionComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'report/quanlity-control',
            component: ReportQuanlityControlComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'report/manufacture',
            component: ReportManufactureComponent,
            canActivate: [AuthGuard]
          }
        ]
      }
    ])
  ],
  exports: [
    RouterModule
  ]
})
export class ManufactureRouting { }
