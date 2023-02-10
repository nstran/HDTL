import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxLoadingModule } from 'ngx-loading';

import { GetPermission } from '../shared/permission/get-permission';
import { ManufactureService } from "./services/manufacture.service";
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';

import { ManufactureRouting } from './manufacture.routing';
import { ManufactureComponent } from './manufacture.component';
import { ProductionOrderCreateComponent } from './component/production-order-create/production-order-create.component';
import { ProductionOrderListComponent } from './component/production-order-list/production-order-list.component';
import { TotalProductionOrderCreateComponent } from './component/total-production-order-create/total-production-order-create.component';
import { TotalProductionOrderListComponent } from './component/total-production-order-list/total-production-order-list.component';
import { TotalProductionOrderDetailComponent } from './component/total-production-order-detail/total-production-order-detail.component';
import { ProductionOrderDetailComponent } from './component/production-order-detail/production-order-detail.component';
import { ProductionTrackingComponent } from './component/production-tracking/production-tracking.component';
import { ProductOrderWorkflowCreateComponent } from './component/product-order-workflow-create/product-order-workflow-create.component';
import { ProductOrderWorkflowDetailComponent } from './component/product-order-workflow-detail/product-order-workflow-detail.component';
import { ProductOrderWorkflowListComponent } from './component/product-order-workflow-list/product-order-workflow-list.component';
import { TechniqueRequestCreateComponent } from './component/technique-request-create/technique-request-create.component';
import { TechniqueRequestDetailComponent } from './component/technique-request-detail/technique-request-detail.component';
import { TechniqueRequestListComponent } from './component/technique-request-list/technique-request-list.component';
import { NoteService } from '../shared/services/note.service';
import { AddProductionOrderDialogComponent } from './component/add-production-order-dialog/add-production-order-dialog.component';
import { TrackProductionComponent } from './component/track-production/track-production.component';
import { DescriptionErrorDialogComponent } from './component/description-error-dialog/description-error-dialog.component';
import { ReportQuanlityControlComponent } from './component/report-quanlity-control/report-quanlity-control.component';
import { ViewListItemDialogComponent } from './component/view-list-item-dialog/view-list-item-dialog.component';
import { ReportManufactureComponent } from './component/report-manufacture/report-manufacture.component';
import { ExportTrackProductionDialogComponent } from './component/export-track-production-dialog/export-track-production-dialog.component';
import { ViewRememberItemDialogComponent } from './component/view-remember-item-dialog/view-remember-item-dialog.component';
import { DialogService } from 'primeng/dynamicdialog';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    ManufactureRouting,
    FormsModule,
    ReactiveFormsModule,
    NgxLoadingModule.forRoot({})
  ],
  declarations: [
    ManufactureComponent, 
    ProductionOrderCreateComponent, 
    ProductionOrderListComponent, 
    TotalProductionOrderCreateComponent, 
    TotalProductionOrderListComponent, 
    TotalProductionOrderDetailComponent, 
    ProductionOrderDetailComponent, 
    ProductionTrackingComponent, 
    ProductOrderWorkflowCreateComponent, 
    ProductOrderWorkflowDetailComponent, 
    ProductOrderWorkflowListComponent, 
    TechniqueRequestCreateComponent, 
    TechniqueRequestDetailComponent, 
    TechniqueRequestListComponent, 
    AddProductionOrderDialogComponent, 
    TrackProductionComponent, 
    DescriptionErrorDialogComponent, 
    ReportQuanlityControlComponent, 
    ViewListItemDialogComponent, 
    ReportManufactureComponent, 
    ExportTrackProductionDialogComponent, 
    ViewRememberItemDialogComponent
  ],
  entryComponents: [
    AddProductionOrderDialogComponent,
    DescriptionErrorDialogComponent,
    ViewListItemDialogComponent,
    ExportTrackProductionDialogComponent,
    ViewRememberItemDialogComponent
  ],
  providers: [
    ManufactureComponent,
    ManufactureService,
    GetPermission,
    MessageService,
    ConfirmationService,
    DialogService,
    NoteService
  ],
})
export class ManufactureModule { }
