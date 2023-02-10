import { DatePipe } from '@angular/common';
import { Component, ElementRef, Injector, OnInit, ViewChild } from '@angular/core';
import { DashBoardService } from '../dashboard.service';
import { AbstractBase } from '../../shared/abstract-base.component';
import { RevenueStatisticServicePacketModel } from '../dashboard.model';
import { IndividualSeriesOptions } from 'highcharts';
import Highcharts from 'highcharts';

@Component({
  selector: 'app-revenue-statistic-service-packet',
  templateUrl: './revenue-statistic-service-packet.component.html',
  styleUrls: ['./revenue-statistic-service-packet.component.css']
})
export class RevenueStatisticServicePacketComponent extends AbstractBase implements OnInit {
  @ViewChild("container", { read: ElementRef, static: true }) container: ElementRef;
  loading : boolean = false;
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
  chart : Highcharts.ChartObject;
  listDetail : RevenueStatisticServicePacketModel[] = [];
  totalAmount : number = 0;
  listRevenueStatisticServicePacketModelByServicePacket : RevenueStatisticServicePacketModel[] = [];
  servicePacketName: string;

  constructor(
    injector: Injector,
    private _datepipe: DatePipe,
    private _dashBoardService : DashBoardService
  ) { 
    super(injector);
  }

  ngOnInit(): void {
    this.takeRevenueStatisticServicePacketDashboard();
  }

  selectCount(event: { value: number; }): void {
    this.count = event.value;
    this.countSelected = this.listCount.find(x => x.value == event.value);
    this.takeRevenueStatisticServicePacketDashboard();
  }

  onChangeFromDate(event): void {
    this.takeRevenueStatisticServicePacketDashboard();
  }

  onChangeToDate(event): void {
    this.takeRevenueStatisticServicePacketDashboard();
  }

  refresh(): void {
    this.loading = true;
    this.chart.showLoading();
    setTimeout(() => {
      this.takeRevenueStatisticServicePacketDashboard();
      this.chart.hideLoading();
      this.loading = false;
    }, 1000);
  }

  dateRange(startDate: Date, endDate: Date, steps = 1): string[] {
    const dateArray: string[] = [];
    let currentDate = new Date(startDate);
    while (currentDate <= new Date(endDate)) {
      dateArray.push(this._datepipe.transform(new Date(currentDate), 'dd/MM'));
      currentDate.setUTCDate(currentDate.getUTCDate() + steps);
    }
    return dateArray;
  }

  takeRevenueStatisticServicePacketDashboard(): void {
    this.servicePacketName = '';
    this._dashBoardService.takeRevenueStatisticServicePacketDashboard(this.convertToUTCTime(this.startDate), this.convertToUTCTime(this.endDate), this.count)
      .subscribe(result => {
        if(result.listRevenueStatisticServicePacketModel && result.listRevenueStatisticServicePacketModel.length > 0){
          this.listRevenueStatisticServicePacketModelByServicePacket = result.listRevenueStatisticServicePacketModelByServicePacket;
          this.totalAmount = 0;
          this.listDetail = result.listRevenueStatisticServicePacketModel;
          this.listDetail.forEach(x => {
            x.amount = x.listAmount.reduce((a, b) => a + b, 0);
            this.totalAmount += x.amount
          });
  
          let series : IndividualSeriesOptions[] = []
          result.listRevenueStatisticServicePacketModel.forEach(x => {
            let objData : IndividualSeriesOptions = {
              name : x.productCategoryName,
              data : x.listAmount 
            }
            series.push(objData)
          });
          this.chart = Highcharts.chart(this.container.nativeElement,{
            chart: {
              type: 'line'
            },
            credits: {
              enabled: false
            },
            title: {
              text: 'Thống kê doanh thu theo loại gói dịch vụ',
              align: 'Left'
            },
            xAxis: {
              categories: this.dateRange(this.startDate, this.endDate)
            },
            yAxis: {
              title: {
                text: 'VNĐ'
              }
            },
            plotOptions: {
              line: {
                dataLabels: {
                  enabled: true,
                  format: '{point.y:,.0f}'
                },
                enableMouseTracking: false
              }
            },
            colors : ['#6B9422','#B64C49','#487ECE','#745CB1','#CDAD51','#73302F','#00A9D7','#FFA400','#1CD0BB'],
            series: series
          });
        } else {
          this.chart = Highcharts.chart(this.container.nativeElement, {
            title: {
              text: 'Thống kê doanh thu theo loại gói dịch vụ',
              align: 'Left'
            }
          });
        }
      })
  }

  loadDashBoardByProductCategoryName(name: string): void {
    this.servicePacketName = name;
    let series : IndividualSeriesOptions[] = [];
    this.listDetail = this.listRevenueStatisticServicePacketModelByServicePacket.filter(x => x.productCategoryName == name);
    this.totalAmount = 0;
    this.listDetail.forEach(x => {
      x.amount = x.listAmount.reduce((a, b) => a + b, 0);
      this.totalAmount += x.amount
    });
    this.listDetail.forEach(x => {
      let objData : IndividualSeriesOptions = {
        name : x.servicePacketName,
        data : x.listAmount,
      }
      series.push(objData)
    });
    this.chart = Highcharts.chart(this.container.nativeElement,{
      chart: {
        type: 'line'
      },
      credits: {
        enabled: false
      },
      title: {
        text: 'Thống kê doanh thu theo loại gói dịch vụ',
        align: 'Left'
      },
      xAxis: {
        categories: this.dateRange(this.startDate, this.endDate)
      },
      yAxis: {
        title: {
          text: 'VNĐ'
        }
      },
      plotOptions: {
        line: {
          dataLabels: {
            enabled: true
          },
          enableMouseTracking: false
        }
      },
      colors : ['#6B9422','#B64C49','#487ECE','#745CB1','#CDAD51','#73302F','#00A9D7','#FFA400','#1CD0BB'],
      series: series
    });
  }

}
