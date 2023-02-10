import { Component, ElementRef, Injector, OnInit, ViewChild } from '@angular/core';
import * as  Highcharts from 'highcharts';
import { DashBoardService } from '../dashboard.service';
import { DatePipe } from '@angular/common';
import { RatingStatisticServicePacketModel, RatingStatisticStarServicePacketModel } from '../dashboard.model';
import { AbstractBase } from '../../shared/abstract-base.component';

@Component({
  selector: 'app-rating-statistics',
  templateUrl: './rating-statistics.component.html',
  styleUrls: ['./rating-statistics.component.css']
})
export class RatingStatisticsComponent extends AbstractBase implements OnInit {
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
  listDetail: RatingStatisticServicePacketModel[] = [];
  totalRate: number = 0;
  totalRateByServicePacketName: number = 0;
  chart: Highcharts.ChartObject;
  listRatingStatisticServicePacketModelByServicePacket: RatingStatisticServicePacketModel[] = [];
  listRatingStatisticStarServicePacketModel: RatingStatisticStarServicePacketModel[] = [];
  listRatingStatisticStarServicePacketModelByServicePacketName: RatingStatisticStarServicePacketModel[] = [];
  productCategoryName: string;
  servicePacketName: string;

  constructor(
    injector: Injector,
    private _datepipe: DatePipe,
    private _dashBoardService : DashBoardService
  ) {
    super(injector)
   }

  ngOnInit(): void {
    this.takeRatingStatisticDashboard();
  }

  selectCount(event: { value: number; }): void {
    this.count = event.value;
    this.countSelected = this.listCount.find(x => x.value == event.value);
    this.takeRatingStatisticDashboard();
  }

  onChangeFromDate(event): void {
    this.takeRatingStatisticDashboard();
  }

  onChangeToDate(event): void {
    this.takeRatingStatisticDashboard();
  }

  refresh(): void {
    this.loading = true;
    this.chart.showLoading();
    setTimeout(() => {
      this.takeRatingStatisticDashboard();
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

  takeRatingStatisticDashboard(): void {
    this.productCategoryName = '';
    this.servicePacketName = '';
    this.listRatingStatisticStarServicePacketModelByServicePacketName = [];
    this._dashBoardService.takeRatingStatisticDashboard(this.convertToUTCTime(this.startDate), this.convertToUTCTime(this.endDate), this.count)
      .subscribe(result => {
        if(result.listRatingStatisticServicePacketModel && result.listRatingStatisticServicePacketModel.length > 0){
          this.listRatingStatisticServicePacketModelByServicePacket = result.listRatingStatisticServicePacketModelByServicePacket;
          this.listRatingStatisticStarServicePacketModel = result.listRatingStatisticStarServicePacketModel;
          this.totalRate = 0;
          let series : Highcharts.IndividualSeriesOptions[] = [];
          this.listDetail = result.listRatingStatisticServicePacketModel;
          this.listDetail.forEach(x => {
            x.rate = x.listRate.reduce((a, b) => a + b, 0);
            this.totalRate += x.rate;
          });
          result.listRatingStatisticServicePacketModel.forEach(x => {
            let objData : Highcharts.IndividualSeriesOptions = {
              name : x.productCategoryName,
              data : x.listRate
            }
            series.push(objData);
          });
          this.chart = Highcharts.chart(this.container.nativeElement, 
            {
              chart: {
                type: 'line'
              },
              credits: {
                enabled: false
              },
              title: {
                text: 'Thống kê đánh giá',
                align: 'Left'
              },
              xAxis: {
                categories: this.dateRange(this.startDate, this.endDate)
              },
              yAxis: {
                title: {
                  text: 'Số lượng'
                }
              },
              plotOptions: {
                line: {
                  dataLabels: {
                    enabled: true
                  }
                }
              },
              colors : ['#6B9422','#B64C49','#487ECE','#745CB1','#CDAD51','#73302F','#00A9D7','#FFA400','#1CD0BB'],
              series: series
          });
        } else {
          this.chart = Highcharts.chart(this.container.nativeElement, {
            title: {
              text: 'Thống kê đánh giá',
              align: 'Left'
            }
          });
        }
      })
  }

  loadDashBoardByProductCategoryName(name: string): void {
    this.productCategoryName = name;
    this.servicePacketName = '';
    this.listRatingStatisticStarServicePacketModelByServicePacketName = [];
    let series : Highcharts.IndividualSeriesOptions[] = [];
    this.totalRate = 0;
    this.listDetail = this.listRatingStatisticServicePacketModelByServicePacket.filter(x => x.productCategoryName == name);
    this.listDetail.forEach(x => {
      x.rate = x.listRate.reduce((a, b) => a + b, 0);
      this.totalRate += x.rate;
    });
    this.listDetail.forEach(x => {
      let objData : Highcharts.IndividualSeriesOptions = {
        name : x.servicePacketName,
        data : x.listRate
      }
      series.push(objData);
    });

    this.chart = Highcharts.chart(this.container.nativeElement, 
      {
        chart: {
          type: 'line'
        },
        credits: {
          enabled: false
        },
        title: {
          text: 'Thống kê đánh giá',
          align: 'Left'
        },
        xAxis: {
          categories: this.dateRange(this.startDate, this.endDate)
        },
        yAxis: {
          title: {
            text: 'Số lượng'
          }
        },
        plotOptions: {
          line: {
            dataLabels: {
              enabled: true
            }
          }
        },
        colors : ['#6B9422','#B64C49','#487ECE','#745CB1','#CDAD51','#73302F','#00A9D7','#FFA400','#1CD0BB'],
        series: series
    });
  }

  loadDashBoardByServicePacketName(name: string): void {
    this.servicePacketName = name;
    let data : Array<[string, number]> = [];
    this.listRatingStatisticStarServicePacketModelByServicePacketName = this.listRatingStatisticStarServicePacketModel.filter(x => x.servicePacketName == name);
    this.totalRateByServicePacketName = 0;
    this.listRatingStatisticStarServicePacketModelByServicePacketName.sort((a, b) => (b.rateStar > a.rateStar) ? 1 : -1).forEach(x => {
      data.push([this.getSatisfactionLevel(x.rateStar), x.count])
      this.totalRateByServicePacketName += x.count;
    });

    this.chart = Highcharts.chart(this.container.nativeElement,{
      chart: {
        type: 'column',
        events: {
          load: function() {
            let column1 = this.series[0].points[0];
            if(column1){
              column1.update({
                color: '#017EFA'
              });
            }
            let column2 = this.series[0].points[1];
            if(column2) {
              column2.update({
                color: '#51CBFF'
              });
            }
            let column3 = this.series[0].points[2];
            if(column3){
              column3.update({
                color: '#DDDD3B'
              });
            }
            let column4 = this.series[0].points[3];
            if(column4){
              column4.update({
                color: '#FDC5CD'
              });
            }
            let column5 = this.series[0].points[4];
            if(column5){
              column5.update({
                color: '#FFA8A7'
              });
            }
          }
      }
      },
      credits: {
        enabled: false
      },
      title: {
        text: 'Thống kê đánh giá',
        align: 'left'
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
          text: 'Số lượng'
        }
      },
      legend: {
        enabled: false
      },
      tooltip: {
        pointFormat: '<b>{point.y:.1f}</b>'
      },
      series: [
        {
          name :'Tên dịch vụ',
          data : data
        }
      ]
    })
  }

  getSatisfactionLevel(star : number) : string {
    let name : string = ''
    switch (star) {
      case 1:
        name = 'Tệ'
      break;

      case 2:
        name = 'Chưa hài lòng'
      break;

      case 3:
        name = 'Bình thường'
        break;

      case 4:
        name = 'Hài lòng'
      break;

      case 5:
        name = 'Tuyệt vời'
      break;

      default:
        break;
    }

    return name;
  }

}
