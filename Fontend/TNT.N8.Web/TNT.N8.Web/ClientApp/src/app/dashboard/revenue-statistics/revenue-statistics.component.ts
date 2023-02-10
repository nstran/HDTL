import { Component, OnInit } from '@angular/core';
import { DashBoardService } from '../dashboard.service';
import { RevenueStatisticModel } from '../dashboard.model';

@Component({
  selector: 'app-revenue-statistics',
  templateUrl: './revenue-statistics.component.html',
  styleUrls: ['./revenue-statistics.component.css']
})
export class RevenueStatisticsComponent implements OnInit {
  revenueStatisticModel : RevenueStatisticModel = new RevenueStatisticModel();
  constructor(
    private _dashBoardService : DashBoardService
  ) 
  { 
    this.revenueStatisticModel.revenueOfYesterday = 0;
    this.revenueStatisticModel.revenueOfLastWeek = 0;
    this.revenueStatisticModel.revenueOfLastMonth = 0;
    this.revenueStatisticModel.revenueOfLastQuarter = 0;
    this.revenueStatisticModel.revenueOfLastYear = 0;
  }

  ngOnInit(): void {
    this.takeRevenueStatisticDashboard();
  }

  takeRevenueStatisticDashboard(): void {
    this._dashBoardService.takeRevenueStatisticDashboard()
      .subscribe(result => {
        this.revenueStatisticModel = result.revenueStatisticModel;
      })
  }

}
