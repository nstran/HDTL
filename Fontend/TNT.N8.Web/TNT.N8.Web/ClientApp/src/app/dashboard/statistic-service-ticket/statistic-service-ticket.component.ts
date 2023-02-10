import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import * as  Highcharts from 'highcharts';
import { DashBoardService } from '../dashboard.service';
import Drilldown from 'highcharts/modules/drilldown';
Drilldown(Highcharts);

@Component({
  selector: 'app-statistic-service-ticket',
  templateUrl: './statistic-service-ticket.component.html',
  styleUrls: ['./statistic-service-ticket.component.css']
})
export class StatisticServiceTicketComponent implements OnInit {
  @ViewChild("container", { read: ElementRef, static: true }) container: ElementRef;
  loading: boolean = false;
  chart: Highcharts.ChartObject;

  constructor(
    private _dashBoardService : DashBoardService
    ) { }

  ngOnInit(): void {
    this.takeStatisticServiceTicketDashboard();
  }

  refresh(): void {
    this.loading = true;
    this.chart.showLoading();
    setTimeout(() => {
      this.takeStatisticServiceTicketDashboard();
      this.chart.hideLoading();
      this.loading = false;
    }, 1000);
  }

  takeStatisticServiceTicketDashboard(): void {
    this._dashBoardService.takeStatisticServiceTicketDashboard()
      .subscribe(result => {
        let drilldownSeries: Highcharts.IndividualSeriesOptions[] = [];
        let listNewStatus: Highcharts.DataPoint[] = [];
        let listProgressStatus: Highcharts.DataPoint[] = [];
        let listDoneStatus: Highcharts.DataPoint[] = [];
        let listCancelStatus: Highcharts.DataPoint[] = [];
        // Mới
        if(result.listNewStatus &&  result.listNewStatus.length > 0){
          result.listNewStatus.forEach(x => {
            let objData : Highcharts.DataPoint = {
              name : x.productCategoryName,
              y: x.count,
              drilldown: 'Mới ' + x.productCategoryName
            }
            listNewStatus.push(objData);
  
            let arrStr : Array<[string, number]> = [];
            x.listStatisticServicePacketNewStatus.forEach(z => {
              arrStr.push([z.servicePacketName, z.count]);
            });
  
            let objDrill : Highcharts.IndividualSeriesOptions = {
              name : 'Mới',
              id:  'Mới ' + x.productCategoryName,
              data: arrStr
            }
            drilldownSeries.push(objDrill);
          });
        }
        // Đang thực hiện
        if(result.listProgressStatus &&  result.listProgressStatus.length > 0){
          result.listProgressStatus.forEach(x => {
            let objData : Highcharts.DataPoint = {
              name : x.productCategoryName,
              y: x.count,
              drilldown: 'Đang thực hiện ' + x.productCategoryName
            }
            listProgressStatus.push(objData);
  
            let arrStr : Array<[string, number]> = [];
            x.listStatisticServicePacketProgressStatus.forEach(z => {
              arrStr.push([z.servicePacketName, z.count]);
            });
  
            let objDrill : Highcharts.IndividualSeriesOptions = {
              name : 'Đang thực hiện',
              id:  'Đang thực hiện ' + x.productCategoryName,
              data: arrStr
            }
            drilldownSeries.push(objDrill);
          });
        }
        //Hoàn thành
        if(result.listDoneStatus &&  result.listDoneStatus.length > 0){
          result.listDoneStatus.forEach(x => {
            let objData : Highcharts.DataPoint = {
              name : x.productCategoryName,
              y: x.count,
              drilldown: 'Hoàn thành ' + x.productCategoryName
            }
            listDoneStatus.push(objData);
  
            let arrStr : Array<[string, number]> = [];
            x.listStatisticServicePacketDoneStatus.forEach(z => {
              arrStr.push([z.servicePacketName, z.count]);
            });
  
            let objDrill : Highcharts.IndividualSeriesOptions = {
              name : 'Hoàn thành',
              id:  'Hoàn thành ' + x.productCategoryName,
              data: arrStr
            }
            drilldownSeries.push(objDrill);
          });
        }
        // Hủy
        if(result.listCancelStatus &&  result.listCancelStatus.length > 0){
          result.listCancelStatus.forEach(x => {
            let objData : Highcharts.DataPoint = {
              name : x.productCategoryName,
              y: x.count,
              drilldown: 'Hủy ' + x.productCategoryName
            }
            listCancelStatus.push(objData);
  
            let arrStr : Array<[string, number]> = [];
            x.listStatisticServicePacketCancelStatus.forEach(z => {
              arrStr.push([z.servicePacketName, z.count]);
            });
  
            let objDrill : Highcharts.IndividualSeriesOptions = {
              name : 'Hủy',
              id:  'Hủy ' + x.productCategoryName,
              data: arrStr
            }
            drilldownSeries.push(objDrill);
          });
        }

        //Chart
        this.chart = Highcharts.chart(this.container.nativeElement, 
          {
            chart: {
              type: 'column',
            },
            credits: {
              enabled: false
            },
            title: {
              text: 'Thống kê phiếu đặt dịch vụ',
              align: 'left'
            },
            xAxis: [{
              type: "category"
            }],
            yAxis: [
              {
                min: 0,
                title: {
                  text: 'Số lượng'
                }
              }
            ],
            plotOptions: {
              column: {
                borderWidth: 0,
                dataLabels: {
                  enabled: true,
                  format: '{y}'
                }
              }
            },
            series: [
              {
                name: 'Mới',
                data: listNewStatus
              }, 
              {
                name: 'Đang thực hiện',
                data: listProgressStatus,
                color: '#007bff'
              },
              {
                name: 'Hoàn thành',
                data: listDoneStatus,
                color: '#28a745'
              }, 
              {
                name: 'Hủy',
                data: listCancelStatus,
                color: '#dc3545'
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
              series: drilldownSeries
            }
        });
      })
  }
}
