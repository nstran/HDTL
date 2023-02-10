import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import * as  Highcharts from 'highcharts';
import { DashBoardService } from '../dashboard.service';
import Drilldown from 'highcharts/modules/drilldown';
import { RevenueStatisticWaitPaymentModel } from '../dashboard.model';
Drilldown(Highcharts);

@Component({
  selector: 'app-revenue-statistic-waiting-payment',
  templateUrl: './revenue-statistic-waiting-payment.component.html',
  styleUrls: ['./revenue-statistic-waiting-payment.component.css']
})
export class RevenueStatisticWaitingPaymentComponent implements OnInit {
  @ViewChild("container", { read: ElementRef, static: true }) container: ElementRef;
  loading : boolean = false;
  chart : Highcharts.ChartObject;
  listRevenueStatisticWaitPaymentModel: RevenueStatisticWaitPaymentModel[] = [];
  totalAmount : number;
  constructor(
    private _dashBoardService : DashBoardService
  ) {}

  ngOnInit(): void {
    this.takeRevenueStatisticWaitPaymentDashboard();
  }

  refresh(): void {
    this.loading = true;
    this.chart.showLoading();
    setTimeout(() => {
      this.takeRevenueStatisticWaitPaymentDashboard();
      this.chart.hideLoading();
      this.loading = false;
    }, 1000);
  }

  takeRevenueStatisticWaitPaymentDashboard(): void {
    this._dashBoardService.takeRevenueStatisticWaitPaymentDashboard()
      .subscribe(result => {
        let data : Highcharts.DataPoint[] = [];
        let drilldownSeries : Highcharts.IndividualSeriesOptions[] = [];
        if(result.listRevenueStatisticWaitPaymentModel && result.listRevenueStatisticWaitPaymentModel.length > 0){
          this.listRevenueStatisticWaitPaymentModel = result.listRevenueStatisticWaitPaymentModel;
          this.totalAmount = 0;
          this.listRevenueStatisticWaitPaymentModel.forEach(e => {
            this.totalAmount += e.amount;
          });
          result.listRevenueStatisticWaitPaymentModel.forEach(x => {
            let objData : Highcharts.DataPoint = {
              name : x.productCategoryName,
              y: x.percent,
              drilldown: x.productCategoryName,
              description : x.amount.toString(),
            }
            let arrStr : Array<[string, number]> = [];
            x.listRevenueStatisticWaitPaymentModel.forEach(z => {
              arrStr.push([z.servicePacketName, z.percentOfServicePacket]);
            });
            let objDrill : Highcharts.IndividualSeriesOptions = {
              name: x.listRevenueStatisticWaitPaymentModel.find(y => y.productCategoryName == x.productCategoryName).productCategoryName,
              id: x.listRevenueStatisticWaitPaymentModel.find(y => y.productCategoryName == x.productCategoryName).productCategoryName,
              data: arrStr
            }
            drilldownSeries.push(objDrill);
            data.push(objData);
          });
          //Chart
          this.chart = Highcharts.chart(this.container.nativeElement, 
            {
              chart: {
                type: 'pie',
                events: {
                  drillup: () => {
                    this.totalAmount = 0;
                    this.listRevenueStatisticWaitPaymentModel = result.listRevenueStatisticWaitPaymentModel;
                    this.listRevenueStatisticWaitPaymentModel.forEach(e => {
                      this.totalAmount += e.amount;
                    });
                  },
                }
              },
              credits: {
                enabled: false
              },
              title: {
                text: 'Thống kê doanh thu chờ thanh toán',
                align: 'left'
              },
              plotOptions: {
                series: {
                  dataLabels: {
                    enabled: true,
                    format: '{point.name}: {point.y:.1f}%'
                  },
                  events: {
                    click: (event) => {
                      this.listRevenueStatisticWaitPaymentModel = this.listRevenueStatisticWaitPaymentModel.find(x => x.productCategoryName == event.point.name).listRevenueStatisticWaitPaymentModel;
                      if(this.listRevenueStatisticWaitPaymentModel.length > 0){
                        this.totalAmount = 0;
                        this.listRevenueStatisticWaitPaymentModel.forEach(e => {
                          this.totalAmount += e.amount;
                        });  
                      }
                    },
                  },
                }
              },
              tooltip: {
                headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
                pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.2f}%</b><br/>'
              },
              colors : ['#6B9422','#B64C49','#487ECE','#745CB1','#CDAD51','#73302F','#00A9D7','#FFA400','#1CD0BB'],
              series: [
                {
                  name : 'Chi tiết',
                  data: data
                }
              ],
              lang: {
                drillUpText: 'Tổng quát'
              },
              drilldown: {
                drillUpButton: {
                  relativeTo: 'spacingBox',
                  position: {
                    y: 0,
                    x: 0
                  },
                  theme: {
                    fill: '#0f62fe',
                    'stroke-width': 1,
                    stroke: 'silver',
                    r: 4,
                    style : {
                      color: '#fff',
                      border: false
                    },
                    states: {
                      hover: {
                        fill: '#0f62fe'
                      },
                    }
                  }
                },
                series: drilldownSeries,
              }
            }
          );
        }
      })
  }

  getDataListDrillDown(name : string): void {
    this.listRevenueStatisticWaitPaymentModel = this.listRevenueStatisticWaitPaymentModel.filter(x => x.productCategoryName == name);
  }
}
