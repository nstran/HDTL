import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { AppRouting } from './app.routing';
import { SharedModule } from './shared/shared.module';
import { UserprofileComponent } from "./userprofile/userprofile.component";
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { CommonService } from './shared/services/common.service';
import { LocationStrategy, PathLocationStrategy } from '@angular/common';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { QueryBuilderModule } from "angular2-query-builder";
import { PanelMenuModule } from 'primeng/panelmenu';
import { CustomerModule } from './customer/customer.module';
import { MeetingDialogComponent } from './customer/components/meeting-dialog/meeting-dialog.component';
import { TemplateVacanciesEmailComponent } from './customer/components/template-vacancies-email/template-vacancies-email.component';
import { RevenueStatisticsComponent } from './dashboard/revenue-statistics/revenue-statistics.component';
import { DashBoardService } from './dashboard/dashboard.service';
import { RevenueStatisticEmployeeComponent } from './dashboard/revenue-statistic-employee/revenue-statistic-employee.component';
import { RevenueStatisticWaitingPaymentComponent } from './dashboard/revenue-statistic-waiting-payment/revenue-statistic-waiting-payment.component';
import { RatingStatisticsComponent } from './dashboard/rating-statistics/rating-statistics.component';
import { RevenueStatisticServicePacketComponent } from './dashboard/revenue-statistic-service-packet/revenue-statistic-service-packet.component';
import { StatisticServiceTicketComponent } from './dashboard/statistic-service-ticket/statistic-service-ticket.component';
import { environment } from '../environments/environment';

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

export function create(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/appconfig', '.json');
}

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    UserprofileComponent,
    RevenueStatisticsComponent,
    StatisticServiceTicketComponent,
    RevenueStatisticEmployeeComponent,
    RevenueStatisticWaitingPaymentComponent,
    RatingStatisticsComponent,
    RevenueStatisticServicePacketComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    AppRouting,
    SharedModule,
    NgMultiSelectDropDownModule.forRoot(),
    QueryBuilderModule,
    PanelMenuModule,
    CustomerModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: (createTranslateLoader),
        deps: [HttpClient],
      }
    })
  ],
  providers: [
    CommonService,
    DashBoardService,
    { provide: LocationStrategy, useClass: PathLocationStrategy}
  ],
  entryComponents: [
    MeetingDialogComponent,
    TemplateVacanciesEmailComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(private commonService: CommonService, private httpClient: HttpClient) {
    this.httpClient.get('./assets/appconfig.json').subscribe((config : any) => {
      localStorage.setItem('ApiEndPoint', config.ApiEndPoint);
      localStorage.setItem('Version', config.Version);
    });

    // commonService.getApiEndPoint().subscribe(result => {
    //   if (result.value !== localStorage.getItem('ApiEndPoint')) {
    //     localStorage.setItem('ApiEndPoint', result.value);
    //   }
    // });
    // commonService.getVersion().subscribe(result => {
    //   if (result.value !== localStorage.getItem('Version')) {
    //     localStorage.setItem('Version', result.value);
    //   }
    // });

    
  }
}
