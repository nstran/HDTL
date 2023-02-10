import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import Highcharts from 'highcharts';

require('highcharts/modules/exporting')(Highcharts);
require('highcharts/modules/export-data')(Highcharts);
require('highcharts/modules/accessibility')(Highcharts);

@Component({
  selector: 'app-bao-cao-chi-phi',
  templateUrl: './bao-cao-chi-phi.component.html',
  styleUrls: ['./bao-cao-chi-phi.component.css']
})
export class BaoCaoChiPhiComponent implements OnInit {

  loading: boolean = false
  searchKhuVucLamViec: any

  thongTinSearch: FormGroup;
  public thoiGianTu: FormControl;
  public thoiGianDen: FormControl;
  searchLoaiBieuDo: FormControl;
  tiGia: FormControl;

  showChart1: boolean = false
  showChart2: boolean = false
  showChart3: boolean = false
  showChart4: boolean = false
  showChart5: boolean = false
  showChart6: boolean = false
  showChart7: boolean = false
  showChart8: boolean = false
  showChart9: boolean = false
  showChart10: boolean = false
  showChart11: boolean = false
  showChart12: boolean = false

  loaiBieuDo: Array<any> = []

  categoriesChart2: any
  seriesChart2: any

  categoriesChart3: any
  seriesChart3: any

  categoriesChart4: any
  seriesChart4: any

  categoriesChart5: any
  seriesChart5: any
  tuNamChart5: any
  denNamChart5: any

  categoriesChart6: any
  seriesChart6: any
  tuNamChart6: any
  denNamChart6: any

  categoriesChart7: any
  seriesChart7: any

  categoriesChart8: any
  seriesChart8: any

  categoriesChart9: any
  seriesChart9: any
  tuNamChart9: any
  denNamChart9: any

  categoriesChart10: any
  seriesChart10: any
  tuNamChart10: any
  denNamChart10: any

  categoriesChart11: any
  seriesChart11: any
  tuNamChart11: any
  denNamChart11: any

  categoriesChart12: any
  seriesChart12: any
  tuNamChart12: any
  denNamChart12: any

  constructor() { }

  ngOnInit(): void {
    this.thoiGianTu = new FormControl(null, [Validators.required]);
    this.thoiGianDen = new FormControl(null, [Validators.required]);
    this.searchLoaiBieuDo = new FormControl(null, [Validators.required]);
    this.tiGia = new FormControl(null, [Validators.required]);

    this.thongTinSearch = new FormGroup({
      thoiGianTu: this.thoiGianTu,
      thoiGianDen: this.thoiGianDen,
      searchLoaiBieuDo: this.searchLoaiBieuDo,
      tiGia : this.tiGia,
    });
    this.getMasterData()
  }

  getMasterData() {
    this.loaiBieuDo = [
      { key: 'chart1', name: 'Chart 1' },
      { key: 'chart2', name: 'Chart 2' },
      { key: 'chart3', name: 'Chart 3' },
      { key: 'chart4', name: 'Chart 4' },
      { key: 'chart5', name: 'Chart 5' },
      { key: 'chart6', name: 'Chart 6' },
      { key: 'chart7', name: 'Chart 7' },
      { key: 'chart8', name: 'Chart 8' },
      { key: 'chart9', name: 'Chart 9' },
      { key: 'chart10', name: 'Chart 10' },
      { key: 'chart11', name: 'Chart 11' },
      { key: 'chart12', name: 'Chart 12' },
    ]
  }

  thayDoiBieuDo(data: any) {
    this.thoiGianTu.reset()
    this.thoiGianDen.reset()
  }

  showBieuDo() {
    if (!this.thongTinSearch.valid) {
      Object.keys(this.thongTinSearch.controls).forEach(key => {
        if (!this.thongTinSearch.controls[key].valid) {
          this.thongTinSearch.controls[key].markAsTouched();
        }
      });
    } else {
      this.showChart1 = false
      this.showChart2 = false
      this.showChart3 = false
      this.showChart4 = false
      this.showChart5 = false
      this.showChart6 = false
      this.showChart7 = false
      this.showChart8 = false
      this.showChart9 = false
      this.showChart10 = false
      this.showChart11 = false
      this.showChart12 = false

      if (this.searchLoaiBieuDo) {
        var data = this.loaiBieuDo.find(x => x.key == this.searchLoaiBieuDo.value.key)
        if (data.key == 'chart12') {
          //data chart 12
          this.categoriesChart12 = ['tháng 1 năm 2021', 'tháng 1 năm 2022']
          this.seriesChart12 = [{
            name: '>110',
            data: [0, 7]
          }, {
            name: '>132',
            data: [5, 7]
          }, {
            name: '>144',
            data: [5, 7]
          }]
          this.tuNamChart12 = 'Jan 2021'
          this.denNamChart12 = 'Jan 2022'
          var chart12 = Highcharts.chart('chart12', {
            chart: {
              type: 'column',
              backgroundColor: '#f2f4f8',
            },
            title: {
              text: `Incentive_Team LD_${this.tuNamChart12} vs ${this.denNamChart12}`
            },
            xAxis: {
              categories: this.categoriesChart12
            },
            yAxis: {
              title: {
                text: ''
              }
            },
            legend: {
              layout: 'vertical',
              align: 'right',
              verticalAlign: 'middle'
            },
            credits: {
              enabled: false
            },
            series: this.seriesChart12,
            plotOptions: {
              series: {
                dataLabels: {
                  enabled: true,
                  format: '{point.y}'
                }
              }
            },
            exporting: {
              buttons: {
                contextButton: {
                  menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                },
              },
              enabled: true,
            },

          });
          this.showChart12 = true

        } else if (data.key == 'chart11') {
          this.categoriesChart11 = ['tháng 1 năm 2021', 'tháng 1 năm 2022']
          this.seriesChart11 = [{
            name: '>132',
            data: [0, 7]
          }, {
            name: '>144',
            data: [5, 7]
          }]
          this.tuNamChart11 = 'Jan 2021'
          this.denNamChart11 = 'Jan 2022'
          var chart11 = Highcharts.chart('chart11', {
            chart: {
              type: 'column',
              backgroundColor: '#f2f4f8'
            },
            title: {
              text: `Incentive_Team_${this.tuNamChart11} vs ${this.denNamChart11}`
            },
            xAxis: {
              categories: this.categoriesChart11
            },
            yAxis: {
              title: {
                text: ''
              }
            },
            legend: {
              layout: 'vertical',
              align: 'right',
              verticalAlign: 'middle'
            },
            credits: {
              enabled: false
            },
            series: this.seriesChart11,
            plotOptions: {
              series: {
                dataLabels: {
                  enabled: true,
                  format: '{point.y}'
                }
              }
            },
            exporting: {
              buttons: {
                contextButton: {
                  menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                },
              },
              enabled: true,
            },

          });
          this.showChart11 = true
        } else if (data.key == 'chart10') {
          this.categoriesChart10 = ['tháng 1 năm 2021', 'tháng 1 năm 2022']
          this.seriesChart10 = [{
            name: '>100',
            data: [6, 7]
          }, {
            name: '>110',
            data: [5, 7]
          }, {
            name: '>120',
            data: [2, 1]
          }]
          this.tuNamChart10 = 'Jan 2021'
          this.denNamChart10 = 'Jan 2022'
          var chart10 = Highcharts.chart('chart10', {
            chart: {
              type: 'column',
              backgroundColor: '#f2f4f8'
            },
            title: {
              text: `Incentive_QA_${this.tuNamChart10} vs ${this.denNamChart10}`
            },
            xAxis: {
              categories: this.categoriesChart10
            },
            yAxis: {
              title: {
                text: ''
              }
            },
            legend: {
              layout: 'vertical',
              align: 'right',
              verticalAlign: 'middle'
            },
            credits: {
              enabled: false
            },
            series: this.seriesChart10,
            plotOptions: {
              series: {
                dataLabels: {
                  enabled: true,
                  format: '{point.y}'
                }
              }
            },
            exporting: {
              buttons: {
                contextButton: {
                  menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                },
              },
              enabled: true,
            },

          });
          this.showChart10 = true
        } else if (data.key == 'chart9') {
          this.categoriesChart9 = ['tháng 1 năm 2021', 'tháng 1 năm 2022']
          this.seriesChart9 = [{
            name: '>100',
            data: [6, 7]
          }, {
            name: '>110',
            data: [5, 7]
          }, {
            name: '>120',
            data: [2, 1]
          }, {
            name: '>132',
            data: [6, 3]
          }, {
            name: '>144',
            data: [10, 7]
          }, {
            name: '>158',
            data: [3, 4]
          }
            , {
            name: '>172',
            data: [5, 5]
          }
            , {
            name: '>200',
            data: [9, 9]
          }]
          this.tuNamChart9 = 'Jan 2021'
          this.denNamChart9 = 'Jan 2022'
          var chart9 = Highcharts.chart('chart9', {
            chart: {
              type: 'column',
              backgroundColor: '#f2f4f8'
            },
            title: {
              text: `Incentive_3D Modeler_${this.tuNamChart9} vs ${this.denNamChart9}`
            },
            xAxis: {
              categories: this.categoriesChart9
            },
            yAxis: {
              title: {
                text: ''
              }
            },
            legend: {
              layout: 'vertical',
              align: 'right',
              verticalAlign: 'middle'
            },
            credits: {
              enabled: false
            },
            series: this.seriesChart9,
            plotOptions: {
              series: {
                dataLabels: {
                  enabled: true,
                  format: '{point.y}'
                }
              }
            },
            exporting: {
              buttons: {
                contextButton: {
                  menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                },
              },
              enabled: true,
            },

          });
          this.showChart9 = true
        } else if (data.key == 'chart8') {
          this.categoriesChart8 = ['tháng 1 năm 2021', 'tháng 2 năm 2021', 'tháng 1 năm 2022', 'tháng 2 năm 2022']
          this.seriesChart8 = [{
            name: 'COS-HN',
            data: [7425, 7100, 7100, 7210]
          }, {
            name: 'COS-DN',
            data: [2014, 1092, 1857, 1200]
          }, {
            name: 'G&A-HN',
            data: [2322, 7450, 1043, 6512]
          }, {
            name: 'G&A-DN',
            data: [2412, 3150, 1415, 1736]
          }, {
            name: 'Ops-DN',
            data: [2122, 3334, 4446, 5511]
          }, {
            name: 'Ops-DN',
            data: [1232, 4811, 6420, 5410]
          }]
          var chart8 = Highcharts.chart('chart8', {
            chart: {
              type: 'column',
              backgroundColor: '#f2f4f8'
            },
            title: {
              text: `UC_SHUI_TU`
            },
            xAxis: {
              categories: this.categoriesChart8
            },
            yAxis: {
              title: {
                text: ''
              }
            },
            legend: {
              layout: 'vertical',
              align: 'right',
              verticalAlign: 'middle'
            },
            credits: {
              enabled: false
            },
            series: this.seriesChart8,
            plotOptions: {
              series: {
                dataLabels: {
                  enabled: true,
                  format: '{point.y}'
                }
              }
            },
            exporting: {
              buttons: {
                contextButton: {
                  menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                },
              },
              enabled: true,
            },

          });
          this.showChart8 = true
        } else if (data.key == 'chart7') {
          this.categoriesChart7 = ['tháng 1 năm 2021', 'tháng 2 năm 2021', 'tháng 1 năm 2022', 'tháng 2 năm 2022']
          this.seriesChart7 = [{
            name: 'COS-HN',
            data: [2425, 3100, 5100, 3210]
          }, {
            name: 'COS-DN',
            data: [2014, 1092, 1857, 1200]
          }, {
            name: 'G&A-HN',
            data: [2322, 7450, 1043, 6512]
          }, {
            name: 'G&A-DN',
            data: [2412, 3150, 1415, 1736]
          }, {
            name: 'Ops-DN',
            data: [2122, 3334, 4446, 5511]
          }, {
            name: 'Ops-DN',
            data: [1232, 4811, 6420, 5410]
          }]
          var chart7 = Highcharts.chart('chart7', {
            chart: {
              type: 'column',
              backgroundColor: '#f2f4f8'
            },
            title: {
              text: `UC_Loftcare`
            },
            xAxis: {
              categories: this.categoriesChart7
            },
            yAxis: {
              title: {
                text: ''
              }
            },
            legend: {
              layout: 'vertical',
              align: 'right',
              verticalAlign: 'middle'
            },
            credits: {
              enabled: false
            },
            series: this.seriesChart7,
            plotOptions: {
              series: {
                dataLabels: {
                  enabled: true,
                  format: '{point.y}'
                }
              }
            },
            exporting: {
              buttons: {
                contextButton: {
                  menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                },
              },
              enabled: true,
            },

          });

          this.showChart7 = true
        } else if (data.key == 'chart6') {
          this.categoriesChart6 = ['tháng 1 năm 2021', 'tháng 2 năm 2021', 'tháng 1 năm 2022', 'tháng 2 năm 2022']
          this.seriesChart6 = [{
            name: 'COS-HN',
            data: [0, 0, 0, 0]
          }, {
            name: 'COS-DN',
            data: [0, 0, 0, 0]
          }, {
            name: 'G&A-HN',
            data: [0, 0, 0, 0]
          }, {
            name: 'G&A-DN',
            data: [0, 0, 0, 0]
          }, {
            name: 'Ops-DN',
            data: [0, 0, 0, 0]
          }, {
            name: 'Ops-DN',
            data: [0, 0, 0, 0]
          }]
          this.tuNamChart6 = 2021
          this.denNamChart6 = 2022
          var chart6 = Highcharts.chart('chart6', {
            chart: {
              type: 'column',
              backgroundColor: '#f2f4f8'
            },
            title: {
              text: `UC_Monthly Income  ${this.tuNamChart6} - ${this.denNamChart6}`
            },
            xAxis: {
              categories: this.categoriesChart6
            },
            yAxis: {
              title: {
                text: ''
              }
            },
            legend: {
              layout: 'vertical',
              align: 'right',
              verticalAlign: 'middle'
            },
            credits: {
              enabled: false
            },
            series: this.seriesChart6,
            plotOptions: {
              series: {
                dataLabels: {
                  enabled: true,
                  format: '{point.y}'
                }
              }
            },
            exporting: {
              buttons: {
                contextButton: {
                  menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                },
              },
              enabled: true,
            },

          });
          this.showChart6 = true
        } else if (data.key == 'chart5') {
          this.categoriesChart5 = ['tháng 1 năm 2021', 'tháng 2 năm 2021', 'tháng 1 năm 2022', 'tháng 2 năm 2022']
          this.seriesChart5 = [{
            name: 'COS-HN',
            data: [0, 0, 5000, 6000]
          }, {
            name: 'COS-DN',
            data: [0, 0, 0, 0]
          }, {
            name: 'G&A-HN',
            data: [0, 0, 0, 0]
          }, {
            name: 'G&A-DN',
            data: [0, 0, 0, 0]
          }, {
            name: 'Ops-DN',
            data: [0, 0, 0, 0]
          }, {
            name: 'Ops-DN',
            data: [0, 0, 0, 0]
          }]
          this.tuNamChart5 = 2021
          this.denNamChart5 = 2022
          var chart5 = Highcharts.chart('chart5', {
            chart: {
              type: 'column',
              backgroundColor: '#f2f4f8'
            },
            title: {
              text: `Total Incentive ${this.tuNamChart5} - ${this.denNamChart5}`
            },
            xAxis: {
              categories: this.categoriesChart5
            },
            yAxis: {
              title: {
                text: ''
              }
            },
            legend: {
              layout: 'vertical',
              align: 'right',
              verticalAlign: 'middle'
            },
            credits: {
              enabled: false
            },
            series: this.seriesChart5,
            plotOptions: {
              series: {
                dataLabels: {
                  enabled: true,
                  format: '{point.y}'
                }
              }
            },
            exporting: {
              buttons: {
                contextButton: {
                  menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                },
              },
              enabled: true,
            },

          });
          this.showChart5 = true
        } else if (data.key == 'chart4') {
          this.categoriesChart4 = ['tháng 1 năm 2021', 'tháng 2 năm 2021', 'tháng 1 năm 2022', 'tháng 2 năm 2022']
          this.seriesChart4 = [{
            name: 'COS-HN',
            data: [9874, 321, 414, 6210]
          }, {
            name: 'COS-DN',
            data: [2014, 1092, 1857, 1200]
          }, {
            name: 'G&A-HN',
            data: [2322, 7450, 0, 0]
          }, {
            name: 'G&A-DN',
            data: [2412, 3150, 200, 141]
          }, {
            name: 'Ops-DN',
            data: [2122, 3334, 2111, 451]
          }, {
            name: 'Ops-DN',
            data: [1232, 4811, 6420, 5410]
          }]
          var chart4 = Highcharts.chart('chart4', {
            chart: {
              type: 'column',
              backgroundColor: '#f2f4f8'
            },
            title: {
              text: `UC_Allowances`
            },
            xAxis: {
              categories: this.categoriesChart4
            },
            yAxis: {
              title: {
                text: ''
              }
            },
            legend: {
              layout: 'vertical',
              align: 'right',
              verticalAlign: 'middle'
            },
            credits: {
              enabled: false
            },
            series: this.seriesChart4,
            plotOptions: {
              series: {
                dataLabels: {
                  enabled: true,
                  format: '{point.y}'
                }
              }
            },
            exporting: {
              buttons: {
                contextButton: {
                  menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                },
              },
              enabled: true,
            },

          });
          this.showChart4 = true
        } else if (data.key == 'chart3') {
          this.categoriesChart3 = ['tháng 1 năm 2021', 'tháng 2 năm 2021', 'tháng 1 năm 2022', 'tháng 2 năm 2022']
          this.seriesChart3 = [{
            name: 'COS-HN',
            data: [3456, 6543, 1243, 1454]
          }, {
            name: 'COS-DN',
            data: [2014, 1092, 1857, 1200]
          }, {
            name: 'G&A-HN',
            data: [2322, 7450, 1043, 6512]
          }, {
            name: 'G&A-DN',
            data: [2412, 3150, 1415, 1736]
          }, {
            name: 'Ops-DN',
            data: [2122, 3334, 4446, 5511]
          }, {
            name: 'Ops-DN',
            data: [1232, 4811, 6420, 5410]
          }]
          var chart3 = Highcharts.chart('chart3', {
            chart: {
              type: 'column',
              backgroundColor: '#f2f4f8'
            },
            title: {
              text: `UC_Monthly Income`
            },
            xAxis: {
              categories: this.categoriesChart3
            },
            yAxis: {
              title: {
                text: ''
              }
            },
            legend: {
              layout: 'vertical',
              align: 'right',
              verticalAlign: 'middle'
            },
            credits: {
              enabled: false
            },
            series: this.seriesChart3,
            plotOptions: {
              series: {
                dataLabels: {
                  enabled: true,
                  format: '{point.y}'
                }
              }
            },
            exporting: {
              buttons: {
                contextButton: {
                  menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                },
              },
              enabled: true,
            },

          });
          this.showChart3 = true
        } else if (data.key == 'chart2') {
          this.categoriesChart2 = ['tháng 1 năm 2021', 'tháng 2 năm 2021', 'tháng 1 năm 2022', 'tháng 2 năm 2022']
          this.seriesChart2 = [{
            name: 'COS-HN',
            data: [1425, 3100, 4100, 3210]
          }, {
            name: 'COS-DN',
            data: [2014, 1092, 1857, 1200]
          }, {
            name: 'G&A-HN',
            data: [2322, 7450, 1043, 6512]
          }, {
            name: 'G&A-DN',
            data: [2412, 3150, 1415, 1736]
          }, {
            name: 'Ops-DN',
            data: [2122, 3334, 4446, 5511]
          }, {
            name: 'Ops-DN',
            data: [1232, 4811, 6420, 5410]
          }]
          var chart2 = Highcharts.chart('chart2', {
            chart: {
              type: 'column',
              backgroundColor: '#f2f4f8'
            },
            title: {
              text: `UC_Monthly Total Payment`
            },
            xAxis: {
              categories: this.categoriesChart2
            },
            yAxis: {
              title: {
                text: ''
              }
            },
            legend: {
              layout: 'vertical',
              align: 'right',
              verticalAlign: 'middle'
            },
            credits: {
              enabled: false
            },
            series: this.seriesChart2,
            plotOptions: {
              series: {
                dataLabels: {
                  enabled: true,
                  format: '{point.y}'
                }
              }
            },
            exporting: {
              buttons: {
                contextButton: {
                  menuItems: ["viewFullscreen", "printChart", "downloadPNG", "downloadJPEG", "downloadPDF", "downloadSVG", "downloadCSV"]
                },
              },
              enabled: true,
            },

          });
          this.showChart2 = true
        } else if (data.key == 'chart1') {
          this.showChart1 = true
        }
      }
    }
  }
}

