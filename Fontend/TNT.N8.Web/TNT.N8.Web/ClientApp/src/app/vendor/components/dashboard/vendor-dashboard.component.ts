import {
  Component,
  OnInit,
  ElementRef,
  ViewChild,
  AfterViewInit,
} from "@angular/core";
import { Router } from "@angular/router";
import { DialogService, MessageService } from "primeng";
import { DialogOrganizationPermissionsionComponent } from "../../../shared/components/dialog-organization-permissionsion/dialog-organization-permissionsion.component";
import { DateParameter } from "../../../sales/models/dateParameter.model";
import { ContractService } from "../../../sales/services/contract.service";
import { GetPermission } from "../../../shared/permission/get-permission";
import { VendorService } from "../../services/vendor.service";
import { Chart, ChartData } from "chart.js";
import * as Highcharts from "highcharts";

class VendorOrder {
  vendorOrderId: string;
  vendorOrderCode: string;
  vendorOrderDate: Date;
  amount: number;
}

class ProcurementRequest {
  procurementRequestId: string;
  procurementCode: string;
  createdDate: Date;
  totalMoney: number;
}

@Component({
  selector: "app-vendor-dashboard",
  templateUrl: "./vendor-dashboard.component.html",
  styleUrls: ["./vendor-dashboard.component.css"],
})
export class VendorDashboardComponent implements OnInit, AfterViewInit {
  ngAfterViewInit() {
    /*Sẽ comment để làm mới*/
    //if (this.isManager) {
    //  this.setDoughnut(this.current_percent, this.current_list_color, this.current_title);
    //  this.setBar(this.current_list_name, this.current_list_revenue);
    //} else {
    //  this.setDoughnut(this.current_percent, this.current_list_color, this.current_title);
    //}
    /*End*/
  }
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem(
    "ListPermissionResource"
  );
  loading: boolean = false;
  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;
  isManager: boolean =
    localStorage.getItem("IsManager") === "true" ? true : false;
  isRoot: boolean = false;
  orgNameDisplay: string = "";
  currentTimeString = this.convertTimeToString();
  model: DateParameter = {
    OrderStartDate: null,
    OrderEndDate: null,
    OrganizationId: null,
    OrganizationName: null,
  };
  list_color: any = [
    "#ff6384",
    "#ffcd56",
    "#dd4895",
    "#36a2eb",
    "#8beb7a",
    "#4bc0c0",
    "#ff9f40",
    "#e5e5e5",
  ];
  current_list_color: any = [];
  // list các tháng để xem doanh số giữa các tháng ( mục giá trị đơn hàng giữa các tháng )
  monthOrderName: any = [];
  // list doanh số của các tháng tại list trên
  monthOrderRevenue: any = [];
  listMonthOption: Array<any> = [];
  months: any;
  endDate: Date = new Date();

  /*========================START : Show Chart==========================*/
  chartShowByProductCategoryGorup: boolean = false;
  chartShowByVendor: boolean = false;
  chartBarShowMonth: boolean = true;
  chartBarShow3: boolean = true;
  /*========================END : Show Chart==========================*/

  /*========================START : Chart Variable==========================*/
  @ViewChild("canvas") canvas: ElementRef;
  @ViewChild("canvasVendor") canvasVendor: ElementRef;

  chart: any;
  chartVendor: any;
  /*========================END : Chart Variable==========================*/

  /*========================START :Data Chart==========================*/
  doughnutData: ChartData = {
    datasets: [],
    labels: [],
  };

  doughnutVendorData: ChartData = {
    datasets: [],
    labels: [],
  };
  /*========================END :Data Chart==========================*/

  /*Table*/
  colsVendorOrder: any;
  colsRequest: any;
  ////////////////
  current_money: any = [];
  current_percent: any = [];
  current_title: any = [];
  ////////////////
  LevelGroupProduct: number = 0;
  listLevelGroupProduct: Array<any> = [];
  month: number = new Date().getMonth() + 1;
  fromDate: Date;
  toDate: Date;

  /*START : RETURN DATA*/
  totalCost: number = 0;
  organization: any;
  listVendorOrderWaitingForApproval: Array<VendorOrder> = [];
  listProcurementWaitingForApproval: Array<ProcurementRequest> = [];
  /*END : RETURN DATA*/

  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private contractService: ContractService,
    private messageService: MessageService,
    private dialogService: DialogService,
    private vendorService: VendorService
  ) {
    Highcharts.setOptions({
      lang: {
        thousandsSep: ",",
      },
    });
  }
  async  ngOnInit() {
    this.listMonthOption = [
      { id: 3, name: "3 tháng" },
      { id: 6, name: "6 tháng" },
      { id: 12, name: "12 tháng" },
    ];
    this.months = this.listMonthOption[0];
    let resource = "buy/vendor/dashboard";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      this.setTable();
      var today = new Date();
      this.model.OrderEndDate = today;
      this.model.OrderStartDate = new Date(new Date().setDate(1));
      this.getMasterDataTotalCost();
    }
  }

  setTable() {
    this.colsVendorOrder = [
      {
        field: "vendorOrderCode",
        header: "Mã đơn hàng mua",
        textAlign: "left",
        display: "table-cell",
        width: "110px",
      },
      {
        field: "vendorName",
        header: "Nhà cung cấp",
        textAlign: "left",
        display: "table-cell",
        width: "150px",
      },
      {
        field: "amount",
        header: "Tổng giá trị",
        textAlign: "right",
        display: "table-cell",
        width: "100px",
      },
      {
        field: "vendorOrderDate",
        header: "Ngày đặt hàng",
        textAlign: "right",
        display: "table-cell",
        width: "100px",
      },
    ];

    this.colsRequest = [
      {
        field: "procurementCode",
        header: "Mã đề xuất",
        textAlign: "left",
        display: "table-cell",
        width: "110px",
      },
      // {
      //   field: "customerName",
      //   header: "Khách hàng",
      //   textAlign: "left",
      //   display: "table-cell",
      //   width: "150px",
      // },
      {
        field: "totalMoney",
        header: "Tổng giá trị",
        textAlign: "right",
        display: "table-cell",
        width: "100px",
      },
      {
        field: "createdDate",
        header: "Ngày đề xuất",
        textAlign: "right",
        display: "table-cell",
        width: "100px",
      },
    ];
  }
  goToCreateOrder() {
    this.router.navigate(["/vendor/create-order"]);
  }
  convertTimeToString(): string {
    let now = new Date();
    let day = now.getDay() + 1;
    let stringDay = "";
    let date = now.getDate();
    let stringDate = "";
    let month = now.getMonth() + 1;
    let stringMonth = "";
    let year = now.getFullYear();
    let stringYear = "";

    if (day == 1) {
      stringDay = "Chủ nhật";
    } else {
      stringDay = "Thứ " + day;
    }
    stringDate = "ngày " + date;
    stringMonth = "tháng " + month;
    stringYear = "năm " + year;

    return stringDay + ", " + stringDate + " " + stringMonth + " " + stringYear;
  }
  
  getMasterDataTotalCost() {
    this.loading = true;
    this.vendorService
      .getDashboardVendor(
        this.month,
        this.model.OrderStartDate,
        this.model.OrderEndDate,
        this.model.OrganizationId
      )
      .subscribe((response) => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode === 200) {
          this.totalCost = result.totalCost;
          this.isRoot = result.isRoot;
          if (
            this.model.OrganizationId == null ||
            this.model.OrganizationId == undefined ||
            this.model.OrganizationId === ""
          ) {
            this.model.OrganizationId = result.organization.organizationId;
            this.orgNameDisplay = result.organization.organizationName;
          }
          this.LevelGroupProduct = 0;
          this.listLevelGroupProduct = [];
          for (let i = 0; i <= result.levelMaxProductCategory; i++) {
            let item = {
              LevelGroupProduct: i,
              LevelGroupProductName: "Cấp " + i,
            };
            this.listLevelGroupProduct.push(item);
          }
          this.listVendorOrderWaitingForApproval = result.listVendorOrder;
          this.listProcurementWaitingForApproval = result.listRequest;
          this.setDataForVendorChart(result.listResultVendorChart);
          this.changeDoughnutManager(0);
          this.getMonthsOrder();
        }
      });
  }
  openOrgDialog() {
    let ref = this.dialogService.open(
      DialogOrganizationPermissionsionComponent,
      {
        data: {},
        header: "Chọn đơn vị",
        width: "40%",
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "350px",
          "max-height": "500px",
          overflow: "auto",
        },
      }
    );

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status == true) {
          this.model.OrganizationId =
            result.selectedOrganization.organizationId;
          this.orgNameDisplay = result.selectedOrganization.organizationName;
        } else {
          this.showToast(
            "error",
            "Thông báo",
            "Rất tiếc! Có lỗi xảy ra! Vui lòng thử lại!"
          );
        }
      }
    });
  }
  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({
      severity: severity,
      summary: summary,
      detail: detail,
    });
  }
  setDoughnut(
    current_percent: any,
    current_list_color: any,
    current_title: any,
    current_money: any
  ) {
    setTimeout(() => {
      if (this.chart) {
        this.chart.destroy();
      }
      if (this.canvas != null) {
        this.chart = new Chart(this.canvas.nativeElement.getContext("2d"), {
          type: "doughnut",
          data: {
            datasets: [
              {
                data: current_percent,
                data_total: current_money,
                backgroundColor: current_list_color,
                borderWidth: 1,
              },
            ],
            labels: current_title,
          },
          options: {
            cutoutPercentage: 70,
            responsive: true,
            legend: {
              display: false,
              position: "bottom",
            },
            tooltips: {
              enabled: true,
              callbacks: {
                label: function (tooltipItem, data) {
                  var label = data.labels[tooltipItem.index];
                  var datasetLabel = data.datasets[
                    tooltipItem.datasetIndex
                  ].data_total[tooltipItem.index]
                    .toFixed(2)
                    .replace(/\d(?=(\d{3})+\.)/g, "$&,");
                  return label + ": " + datasetLabel;
                },
              },
            },
          },
        });
        this.doughnutData = this.chart.data;
      }
    }, 100);
  }
  setDoughnutVendor(
    current_percent: any,
    current_list_color: any,
    current_title: any,
    current_money: any
  ) {
    setTimeout(() => {
      if (this.chartVendor) {
        this.chartVendor.destroy();
      }
      if (this.canvasVendor != null) {
        this.chartVendor = new Chart(
          this.canvasVendor.nativeElement.getContext("2d"),
          {
            type: "doughnut",
            data: {
              datasets: [
                {
                  data: current_percent,
                  data_total: current_money,
                  backgroundColor: current_list_color,
                  borderWidth: 1,
                },
              ],
              labels: current_title,
            },
            options: {
              cutoutPercentage: 70,
              responsive: true,
              legend: {
                display: false,
                position: "bottom",
              },
              tooltips: {
                enabled: true,
                callbacks: {
                  label: function (tooltipItem, data) {
                    var label = data.labels[tooltipItem.index];
                    var datasetLabel = data.datasets[
                      tooltipItem.datasetIndex
                    ].data_total[tooltipItem.index]
                      .toFixed(2)
                      .replace(/\d(?=(\d{3})+\.)/g, "$&,");
                    return label + ": " + datasetLabel;
                  },
                },
              },
            },
          }
        );
        this.doughnutVendorData = this.chartVendor.data;
      }
    }, 100);
  }
  changeDoughnutManager(productCategoryLevel: number) {
    this.vendorService
      .getProductCategoryGroupBy(
        this.model.OrderStartDate,
        this.model.OrderEndDate,
        this.model.OrganizationId,
        productCategoryLevel
      )
      .subscribe((response) => {
        let result2: any = response;
        if (result2.statusCode === 200) {
          // Khai báo biến
          let current_money: any = [];
          let current_percent: any = [];
          let current_title: any = [];
          let current_list_color: any = [];

          this.chartShowByProductCategoryGorup = true;
          if (result2.lstResult.length === 0) {
            this.chartShowByProductCategoryGorup = false;
          } else if (result2.lstResult.length <= 8) {
            //Nếu tổng số nhóm sản phẩm bé hơn 8 thì
            for (let i = 0; i < result2.lstResult.length; i++) {
              current_list_color[i] = this.list_color[i];
              current_percent[i] = result2.lstResult[i].Percent;
              current_money[i] = result2.lstResult[i].Total;
              current_title[i] = result2.lstResult[i].ProductCategoryName;
            }
            this.setDoughnut(
              current_percent,
              current_list_color,
              current_title,
              current_money
            );
          } else {
            //Nếu tổng số nhóm sản phẩm lớn hơn 8 thì
            let total_top_7 = 0;
            for (let i = 0; i < 8; i++) {
              if (i < 7) {
                current_list_color[i] = this.list_color[i];
                current_percent[i] = result2.lstResult[i].Percent;
                current_title[i] = result2.lstResult[i].ProductCategoryName;
                current_money[i] = result2.lstResult[i].Total;
                total_top_7 += result2.lstResult[i].Percent;
              } else if (i == 7) {
                current_list_color[i] = this.list_color[i];
                current_percent[i] = (100 - total_top_7).toFixed(1);
                current_money[i] = result2.lstResult[i].Total;
                current_title[i] = "Khác";
              }
            }
            this.setDoughnut(
              current_percent,
              current_list_color,
              current_title,
              current_money
            );
          }
        }
      });
  }
  setDataForVendorChart(data: any) {
    this.chartShowByVendor = true;

    let current_money: any = [];
    let current_percent: any = [];
    let current_title: any = [];
    let current_list_color: any = [];

    if (data.length === 0) {
      // Khai báo biến
      this.chartShowByVendor = false;
    } else if (data.length <= 8) {
      //Nếu tổng số nhóm sản phẩm bé hơn 8 thì
      for (let i = 0; i < data.length; i++) {
        current_list_color[i] = this.list_color[i];
        current_percent[i] = data[i].Percent;
        current_money[i] = data[i].Count;
        current_title[i] = data[i].VendorName;
      }
      this.setDoughnutVendor(
        current_percent,
        current_list_color,
        current_title,
        current_money
      );
    } else {
      data = data.sort(function(a, b){return b.Count - a.Count});
      //Nếu tổng số nhóm sản phẩm lớn hơn 8 thì
      let total_top_7 = 0;
      for (let i = 0; i < 8; i++) {
        if (i < 7) {
          current_list_color[i] = this.list_color[i];
          current_percent[i] = data[i].Percent;
          current_title[i] = data[i].VendorName;
          current_money[i] = data[i].Count;
          total_top_7 += data[i].Percent;
        } else if (i == 7) {
          current_list_color[i] = this.list_color[i];
          current_percent[i] = (100 - total_top_7).toFixed(1);
          current_money[i] = data[i].Count;
          current_title[i] = "Khác";
        }
      }
      this.setDoughnutVendor(
        current_percent,
        current_list_color,
        current_title,
        current_money
      );
    }
  }
  getMonthsOrder() {
    this.vendorService
      .getDataBarchartFollowMonth(
        this.endDate,
        this.months.id,
        this.model.OrganizationId
      )
      .subscribe((response) => {
        let result: any = response;
        if (result.statusCode === 200) {
          this.setChart3(result.monthOrderAndRequestList);
          this.setChart4(result.monthOrderList);
        }
      });
  }
  setChart3(data: any) {
    setTimeout(() => {
      if (data.length > 0) {
        this.chartBarShow3 = true;
        let listVendorOrderAmount: Array<number> = data.map(
          (c) => c.VendorOrderAmount
        );
        let listRequestAmount: Array<number> = data.map((c) => c.RequestAmount);
        let arrayCate: Array<string> = data.map((c) => c.DateStr);
        Highcharts.setOptions({
          chart: {
            style: {
              fontFamily: "Inter UI",
            },
          },
        });
        Highcharts.chart("container3", {
          chart: {
            type: "column",
          },
          title: {
            text: "",
          },
          subtitle: {
            text: "",
          },
          xAxis: {
            categories: arrayCate,
            crosshair: true,
            title: {
              text: "",
            },
          },
          yAxis: {
            min: 0,
            title: {
              text: "",
            },
          },
          tooltip: {
            headerFormat:
              '<span style="font-size:10px">{point.key}</span><table>',
            pointFormat:
              '<tr><td style="color:{series.color};padding:0;font-family:Inter UI">{series.name}: </td>' +
              '<td style="padding:0;font-family:Inter UI"><b>{point.y} VNĐ</b></td></tr>',
            footerFormat: "</table>",
            shared: true,
            useHTML: true,
          },
          plotOptions: {
            column: {
              pointPadding: 0.2,
              borderWidth: 0,
            },
          },
          series: [
            {
              name: "Mua hàng",
              data: listVendorOrderAmount,
              color: "#237118",
            },
            {
              name: "Đề xuất mua hàng",
              data: listRequestAmount,
              color: "#425A55",
            },
          ],
        });
      } else {
        this.chartBarShow3 = false;
      }
    }, 100);

  }
  setChart4(data: any) {
    setTimeout(() => {
      if (data.length > 0) {
        this.chartBarShowMonth = true;
        let listVendorOrderAmount: Array<number> = data.map(
          (c) => c.VendorOrderAmount
        );
        let listCusOrderAmount: Array<number> = data.map(
          (c) => c.CustomerOrderAmount
        );
        let arrayCate: Array<string> = data.map((c) => c.DateStr);
        Highcharts.setOptions({
          chart: {
            style: {
              fontFamily: "Inter UI",
            },
          },
        });
        Highcharts.chart("container", {
          chart: {
            type: "column",
          },
          title: {
            text: "",
          },
          subtitle: {
            text: "",
          },
          xAxis: {
            categories: arrayCate,
            crosshair: true,
            title: {
              text: "",
            },
          },
          yAxis: {
            min: 0,
            title: {
              text: "",
            },
          },
          tooltip: {
            headerFormat:
              '<span style="font-size:10px">{point.key}</span><table>',
            pointFormat:
              '<tr><td style="color:{series.color};padding:0;font-family:Inter UI">{series.name}: </td>' +
              '<td style="padding:0;font-family:Inter UI"><b>{point.y} VNĐ</b></td></tr>',
            footerFormat: "</table>",
            shared: true,
            useHTML: true,
          },
          plotOptions: {
            column: {
              pointPadding: 0.2,
              borderWidth: 0,
            },
          },
          series: [
            {
              name: "Mua hàng",
              data: listVendorOrderAmount,
              color: "#237118",
            },
            {
              name: "Bán hàng",
              data: listCusOrderAmount,
              color: "#44AACC",
            },
          ],
        });
      } else {
        this.chartBarShowMonth = false;
      }
    }, 100);
  }
  goToDetailVendorOrder(id: string) {
    this.router.navigate(["/vendor/detail-order", { vendorOrderId: id }]);
  }
  goToDetailRequest(value: any) {
    this.router.navigate(['/procurement-request/view', { id: value }])
  }
  goToRequestList() {
    this.router.navigate(['/procurement-request/list']);
  }

  goToVendorOrderList() {
    this.router.navigate(['/vendor/-order']);
  }
}
