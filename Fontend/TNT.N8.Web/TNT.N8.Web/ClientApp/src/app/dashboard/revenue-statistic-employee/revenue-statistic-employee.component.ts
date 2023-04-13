import { Component, ElementRef, Injector, OnInit, ViewChild } from '@angular/core';
import { DashBoardService } from '../dashboard.service';
import Highcharts from 'highcharts';
import { AbstractBase } from '../../shared/abstract-base.component';

@Component({
  selector: 'app-revenue-statistic-employee',
  templateUrl: './revenue-statistic-employee.component.html',
  styleUrls: ['./revenue-statistic-employee.component.css']
})
export class RevenueStatisticEmployeeComponent extends AbstractBase implements OnInit {
  @ViewChild("container", { read: ElementRef, static: true }) container: ElementRef;
  loading: boolean = false;
  startDate: Date = new Date(new Date().setDate(new Date().getDate() - 7));
  endDate: Date = new Date();
  count : number = 5;
  listCount = [
    {value : 5, text : 'Top 5'},
    {value : 10, text : 'Top 10'},
    {value : 15, text : 'Top 15'},
    {value : 20, text : 'Top 20'},
    {value : 0, text : 'Tất cả'}
  ];
  countSelected = {value : 5, text : 'Top 5'};
  chart: Highcharts.ChartObject;

  constructor(
    injector: Injector,
    private _dashBoardService : DashBoardService
  ) {
    super(injector)
   }

  ngOnInit(): void {
    this.takeRevenueStatisticEmployeeDashboard();
  }

  selectCount(event): void {
    this.count = event.value;
    this.countSelected = this.listCount.find(x => x.value == event.value);
    this.takeRevenueStatisticEmployeeDashboard();
  }

  onChangeFromDate(event): void {
    this.takeRevenueStatisticEmployeeDashboard();
  }

  onChangeToDate(event): void {
    this.takeRevenueStatisticEmployeeDashboard();
  }

  refresh(): void {
    this.loading = true;
    this.chart.showLoading();
    setTimeout(() => {
      this.takeRevenueStatisticEmployeeDashboard();
      this.chart.hideLoading();
      this.loading = false;
    }, 1000);
  }

  takeRevenueStatisticEmployeeDashboard(): void {
    this._dashBoardService.takeRevenueStatisticEmployeeDashboard(this.startDate, this.endDate, this.count)
      .subscribe(result => {
        if(result.listRevenueStatisticEmployeeModel && result.listRevenueStatisticEmployeeModel.length > 0){
          let data : Array<[string, number]> = [];
          result.listRevenueStatisticEmployeeModel.forEach(x => {
            data.push([x.employeeName, x.amount])
          });
  
          this.chart = Highcharts.chart(this.container.nativeElement,{
            chart: {
              type: 'column'
            },
            credits: {
              enabled: false
            },
            title: {
              text: null
            },
            xAxis: {
              type: 'category',
              labels: {
                style: {
                  fontSize: '13px',
                  fontFamily: 'Verdana, sans-serif'
                }
              }
            },
            yAxis: {
              min: 0,
              title: {
                text: 'VNĐ'
              }
            },
            legend: {
              enabled: false
            },
            plotOptions: {
              column: {
                dataLabels: {
                  enabled: true,
                  format: '{point.y:,.0f}'
                }
              }
            },
            tooltip: {
              pointFormat: 'Doanh thu: <b>{point.y:,.0f} VNĐ</b>'
            },
            series: [
              {
                name :'Gói dịch vụ',
                data : data
              }
            ]
          })
        }
      })
  }

}
