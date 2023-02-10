import { Component, OnInit, ViewChild, ElementRef, HostListener, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission';

import { SelectItem, SortEvent } from 'primeng/api';
import { MenuItem } from "primeng/primeng";
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DialogService } from 'primeng/dynamicdialog';
import * as $ from 'jquery';

import { ReSearchService } from '../../../services/re-search.service';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { BusinessGoalsService } from '../../services/business-goals.service';
import { OrganizationDialogComponent } from '../../../shared/components/organization-dialog/organization-dialog.component';
import { BusinessGoals, BusinessGoalsDetail } from '../../models/business-goals.model';

class Year {
  label: string;
  value: number;
}

class ProductCategory {
  productCategoryId: string;
  productCategoryCode: string;
  productCategoryName: string;
}

@Component({
  selector: 'app-business-goals',
  templateUrl: './business-goals.component.html',
  styleUrls: ['./business-goals.component.css']
})
export class BusinessGoalsComponent implements OnInit {
  @ViewChild('myTable') myTable: Table;
  auth = JSON.parse(localStorage.getItem("auth"));
  filterGlobal: string;
  innerWidth: number = 0; //number window size first
  loading: boolean = false;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  currentYear: number = (new Date()).getFullYear();
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  defaultNumberType = this.systemParameterList.find((systemParameter: { systemKey: string; }) => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  today: Date = new Date();

  isManager: boolean = null;
  isGlobalFilter: string = '';

  //START: BIẾN ĐIỀU KIỆN
  fixed: boolean = false;
  withFiexd: string = "";
  //END : BIẾN ĐIỀU KIỆN

  /* Action*/
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  /*END*/

  /*TABLE*/
  selectedColumns: any[];
  cols: any[];
  rowGroupMetadataDoanhSo: any;
  rowGroupMetadataDoanhThu: any;
  /*END*/

  /*FORM CONTROL*/
  businessGoalsFrom: FormGroup;

  yearControl: FormControl;
  organizationControl: FormControl;
  /*END*/

  /*BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  organizationId: string = this.emptyGuid;
  organizationName: string = '';
  listYear: Array<Year> = [];
  listProductCategory: Array<ProductCategory> = [];
  listBusinessGoalsRevenueDetail: Array<BusinessGoalsDetail> = []; // Doanh thu
  listBusinessGoalsSalesDetail: Array<BusinessGoalsDetail> = []; // Doanh số

  listBusinessGoalsRevenueDetailChild: Array<BusinessGoalsDetail> = []; // Doanh thu phòng con
  listBusinessGoalsSalesDetailChild: Array<BusinessGoalsDetail> = []; // Doanh số phòng con
  listOrganization: any = [];
  /*END*/


  /*Total variable*/
  totalSalesT1: number = 0;
  totalSalesT2: number = 0;
  totalSalesT3: number = 0;
  totalSalesT4: number = 0;
  totalSalesT5: number = 0;
  totalSalesT6: number = 0;
  totalSalesT7: number = 0;
  totalSalesT8: number = 0;
  totalSalesT9: number = 0;
  totalSalesT10: number = 0;
  totalSalesT11: number = 0;
  totalSalesT12: number = 0;

  totalRevenueT1: number = 0;
  totalRevenueT2: number = 0;
  totalRevenueT3: number = 0;
  totalRevenueT4: number = 0;
  totalRevenueT5: number = 0;
  totalRevenueT6: number = 0;
  totalRevenueT7: number = 0;
  totalRevenueT8: number = 0;
  totalRevenueT9: number = 0;
  totalRevenueT10: number = 0;
  totalRevenueT11: number = 0;
  totalRevenueT12: number = 0;


  constructor(
    private ref: ChangeDetectorRef,
    private dialogService: DialogService,
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private translate: TranslateService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    public reSearchService: ReSearchService,
    private businessGoalsService: BusinessGoalsService,
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
    this.setDefaultYear();
  }

  async  ngOnInit() {
    let resource = "sys/admin/business-goals/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.messageService.add({ severity: 'warn', summary: 'Thông báo', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' });
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }

      this.initForm();
      this.initTable();
      this.getMasterData(true);
    }
  }

  initForm() {
    let year = this.listYear.find(c => c.value == this.currentYear);
    this.yearControl = new FormControl(year, [Validators.required]);
    this.organizationControl = new FormControl('', [Validators.required]);

    this.businessGoalsFrom = new FormGroup({
      yearControl: this.yearControl,
      organizationControl: this.organizationControl,
    });
  }

  initTable() {
    this.cols = [
      { field: 'productCategoryName', header: 'Chỉ tiêu', textAlign: 'center', textAlignContent: "left", display: 'table-cell', width: '150px', excelWidth: 20, },
      { field: 'january', header: 'T1', textAlign: 'center', textAlignContent: "right", display: 'table-cell', width: '80px', excelWidth: 30, },
      { field: 'february', header: 'T2', textAlign: 'center', textAlignContent: "right", display: 'table-cell', width: '80px', excelWidth: 25, },
      { field: 'march', header: 'T3', textAlign: 'center', textAlignContent: "right", display: 'table-cell', width: '80px', excelWidth: 25, },
      { field: 'april', header: 'T4', textAlign: 'center', textAlignContent: "right", display: 'table-cell', width: '80px', excelWidth: 20, },
      { field: 'may', header: 'T5', textAlign: 'center', textAlignContent: "right", display: 'table-cell', width: '80px', excelWidth: 30, },
      { field: 'june', header: 'T6', textAlign: 'center', textAlignContent: "right", display: 'table-cell', width: '80px', excelWidth: 25, },
      { field: 'july', header: 'T7', textAlign: 'center', textAlignContent: "right", display: 'table-cell', width: '80px', excelWidth: 25, },
      { field: 'august', header: 'T8', textAlign: 'center', textAlignContent: "right", display: 'table-cell', width: '80px', excelWidth: 25, },
      { field: 'september', header: 'T9', textAlign: 'center', textAlignContent: "right", display: 'table-cell', width: '80px', excelWidth: 25, },
      { field: 'october', header: 'T10', textAlign: 'center', textAlignContent: "right", display: 'table-cell', width: '80px', excelWidth: 25, },
      { field: 'november', header: 'T11', textAlign: 'center', textAlignContent: "right", display: 'table-cell', width: '80px', excelWidth: 25, },
      { field: 'december', header: 'T12', textAlign: 'center', textAlignContent: "right", display: 'table-cell', width: '80px', excelWidth: 25, },
    ];
    this.selectedColumns = this.cols;
  }

  getMasterData(type: boolean) {
    let year = this.yearControl.value.value;
    this.loading = true;
    this.businessGoalsService.getMasterDataBusinessGoals(this.organizationId, year, type, this.auth.UserId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listProductCategory = result.listProductCategory;
        this.listBusinessGoalsSalesDetail = result.listBusinessGoalsSalesDetail;
        this.listBusinessGoalsRevenueDetail = result.listBusinessGoalsRevenueDetail;
        this.listBusinessGoalsSalesDetailChild = result.listBusinessGoalsSalesDetailChild;

        this.updateRowGroupMetaDataDoanhSo();
        this.updateRowGroupMetaDataDoanhThu();

        if (type == true) {
          let organization = result.listOrganization.find(c => c.parentId == null);
          this.organizationId = organization.organizationId;
          this.organizationControl.setValue(organization.organizationName);
        }

        this.listBusinessGoalsSalesDetailChild.forEach(item => {
          item.organizationName = this.buildOrganizationName(result.listOrganization, item.organizationId);
        });

        this.listBusinessGoalsRevenueDetailChild.forEach(item => {
          item.organizationName = this.buildOrganizationName(result.listOrganization, item.organizationId);
        });

        this.calculate();
        this.ref.detectChanges();
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  buildOrganizationName(listOrganization: Array<any>, organizationId: string): string {
    let result = '';
    let listOrganizationName = []
    let organization = listOrganization.find(x => x.organizationId == organizationId);
    let currentOrganization = listOrganization.find(x => x.organizationId == this.organizationId);
    listOrganizationName.push(organization.organizationName);

    while (organization.level > currentOrganization.level + 1) {
      organization = listOrganization.find(x => x.organizationId == organization.parentId);
      listOrganizationName.push(organization.organizationName);
    }

    return result = listOrganizationName.reverse().join(' > ');
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  openOrgPopup() {
    let ref = this.dialogService.open(OrganizationDialogComponent, {
      data: {
        chooseFinancialIndependence: false //Nếu chỉ chọn đơn vị độc lập tài chính
      },
      header: 'Chọn đơn vị',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "350px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.organizationId = result.selectedOrgId;
          this.organizationControl.setValue(result.selectedOrgName);
          this.getMasterData(false);
        }
      }
    });
  }

  setDefaultYear() {
    let i = 0;
    while (i < 6) {
      let year = new Year();
      year.value = this.currentYear + i;
      year.label = year.value.toString();

      this.listYear.push(year);
      i++;
    }
  }

  deleteBusinessGoalProductCategory(rowData: any, type: string) {
    if (type == "SALES") {
      this.listBusinessGoalsSalesDetail = this.listBusinessGoalsSalesDetail.filter(c => c != rowData);
    } else {
      this.listBusinessGoalsRevenueDetail = this.listBusinessGoalsRevenueDetail.filter(c => c != rowData);
    }

    this.calculate();
  }

  changeProductCategorySales(event: any, rowData: BusinessGoalsDetail, rowIndex: number) {
    let productCategory = event.value;
    this.listBusinessGoalsSalesDetail.forEach((x, index) => {
      if (rowIndex == index) {
        x.productCategoryId = productCategory.productCategoryId;
        x.productCategoryName = productCategory.productCategoryName;
      }
    });

  }

  changeProductCategoryRevenue(event: any, rowData: BusinessGoalsDetail, rowIndex: number) {
    let productCategory = event.value;
    this.listBusinessGoalsRevenueDetail.forEach((x, index) => {
      if (rowIndex == index) {
        x.productCategoryId = productCategory.productCategoryId;
        x.productCategoryName = productCategory.productCategoryName;
      }
    });
  }

  addProductCategoryBusinessGoalsSales() {
    let businessGoalsDetail = new BusinessGoalsDetail();
    this.listBusinessGoalsSalesDetail.push(businessGoalsDetail);
  }
  addProductCategoryBusinessGoalsRevenue() {
    let businessGoalsDetail = new BusinessGoalsDetail();
    this.listBusinessGoalsRevenueDetail.push(businessGoalsDetail);
  }

  save() {

    if (!this.businessGoalsFrom.valid) {
      Object.keys(this.businessGoalsFrom.controls).forEach(key => {
        if (!this.businessGoalsFrom.controls[key].valid) {
          this.businessGoalsFrom.controls[key].markAsTouched();
        }
      });
    } else {
      this.loading = true;
      let countSales = this.listBusinessGoalsSalesDetail.filter(c => c.productCategoryId == this.emptyGuid).length;
      let countRevenue = this.listBusinessGoalsRevenueDetail.filter(c => c.productCategoryId == this.emptyGuid).length;
      let businessGoals = this.mappingDataFormToModel();
      let lstSales = this.mappingListBusinessGoalsSale();
      let letRevenue = this.mappingListBusinessGoalsRevenue();


      if (countSales > 0) {
        this.loading = false;
        this.totalSalesT1 = 0;
        this.totalSalesT2 = 0;
        this.totalSalesT3 = 0;
        this.totalSalesT4 = 0;
        this.totalSalesT5 = 0;
        this.totalSalesT6 = 0;
        this.totalSalesT7 = 0;
        this.totalSalesT8 = 0;
        this.totalSalesT9 = 0;
        this.totalSalesT10 = 0;
        this.totalSalesT11 = 0;
        this.totalSalesT12 = 0;
        this.clearToast();
        this.showToast('error', 'Thông báo', "Chưa chọn sản phẩm cho doanh số");
      }
      else if (countRevenue > 0) {
        this.loading = false;
        this.totalRevenueT1 = 0;
        this.totalRevenueT2 = 0;
        this.totalRevenueT3 = 0;
        this.totalRevenueT4 = 0;
        this.totalRevenueT5 = 0;
        this.totalRevenueT6 = 0;
        this.totalRevenueT7 = 0;
        this.totalRevenueT8 = 0;
        this.totalRevenueT9 = 0;
        this.totalRevenueT10 = 0;
        this.totalRevenueT11 = 0;
        this.totalRevenueT12 = 0;
        this.clearToast();
        this.showToast('error', 'Thông báo', "Chưa chọn sản phẩm cho doanh thu");
      }
      else {
        this.businessGoalsService.createOrUpdateBusinessGoals(businessGoals, lstSales, letRevenue, this.auth.UserId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode === 200) {
            this.clearToast();
            this.showToast('success', 'Thông báo', "Chỉnh sửa dữ liệu thành công!");
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    }
  }

  mappingDataFormToModel(): BusinessGoals {
    let businessModel = new BusinessGoals();
    businessModel.organizationId = this.organizationId;
    businessModel.year = this.yearControl.value.value;

    return businessModel;
  }

  mappingListBusinessGoalsSale(): Array<BusinessGoalsDetail> {
    let lstSales: Array<BusinessGoalsDetail> = [];
    this.listBusinessGoalsSalesDetail = this.listBusinessGoalsSalesDetail.filter(c => c.productCategoryId != this.emptyGuid);
    this.listBusinessGoalsSalesDetail.forEach(item => {
      let businessDetail = new BusinessGoalsDetail();
      businessDetail.productCategoryId = item.productCategoryId;
      businessDetail.businessGoalsType = "SALES";
      businessDetail.january = ParseStringToFloat(item.january.toString());
      businessDetail.february = ParseStringToFloat(item.february.toString());
      businessDetail.march = ParseStringToFloat(item.march.toString());
      businessDetail.april = ParseStringToFloat(item.april.toString());
      businessDetail.may = ParseStringToFloat(item.may.toString());
      businessDetail.june = ParseStringToFloat(item.june.toString());
      businessDetail.july = ParseStringToFloat(item.july.toString());
      businessDetail.august = ParseStringToFloat(item.august.toString());
      businessDetail.september = ParseStringToFloat(item.september.toString());
      businessDetail.october = ParseStringToFloat(item.october.toString());
      businessDetail.november = ParseStringToFloat(item.november.toString());
      businessDetail.december = ParseStringToFloat(item.december.toString());
      lstSales.push(businessDetail);
    });

    return lstSales;
  }

  mappingListBusinessGoalsRevenue(): Array<BusinessGoalsDetail> {
    let lstRevenue: Array<BusinessGoalsDetail> = [];
    this.listBusinessGoalsRevenueDetail = this.listBusinessGoalsRevenueDetail.filter(c => c.productCategoryId != this.emptyGuid);
    this.listBusinessGoalsRevenueDetail.forEach(item => {
      let businessDetail = new BusinessGoalsDetail();
      businessDetail.productCategoryId = item.productCategoryId;
      businessDetail.businessGoalsType = "REVENUE";
      businessDetail.january = ParseStringToFloat(item.january.toString());
      businessDetail.february = ParseStringToFloat(item.february.toString());
      businessDetail.march = ParseStringToFloat(item.march.toString());
      businessDetail.april = ParseStringToFloat(item.april.toString());
      businessDetail.may = ParseStringToFloat(item.may.toString());
      businessDetail.june = ParseStringToFloat(item.june.toString());
      businessDetail.july = ParseStringToFloat(item.july.toString());
      businessDetail.august = ParseStringToFloat(item.august.toString());
      businessDetail.september = ParseStringToFloat(item.september.toString());
      businessDetail.october = ParseStringToFloat(item.october.toString());
      businessDetail.november = ParseStringToFloat(item.november.toString());
      businessDetail.december = ParseStringToFloat(item.december.toString());
      lstRevenue.push(businessDetail);
    });

    return lstRevenue;
  }

  changeYear() {
    this.getMasterData(false);
  }

  updateRowGroupMetaDataDoanhSo() {
    this.rowGroupMetadataDoanhSo = {};
    if (this.listBusinessGoalsSalesDetailChild) {
      for (let i = 0; i < this.listBusinessGoalsSalesDetailChild.length; i++) {
        let rowData = this.listBusinessGoalsSalesDetailChild[i];

        let organizationId = rowData.organizationId;
        if (i == 0) {
          this.rowGroupMetadataDoanhSo[organizationId] = { index: 0, size: 1 };
        }
        else {
          let previousRowData = this.listBusinessGoalsSalesDetailChild[i - 1];
          let previousRowGroup = previousRowData.organizationId;
          if (organizationId === previousRowGroup)
            this.rowGroupMetadataDoanhSo[organizationId].size++;
          else
            this.rowGroupMetadataDoanhSo[organizationId] = { index: i, size: 1 };
        }
      }
    }
  }

  updateRowGroupMetaDataDoanhThu() {
    this.rowGroupMetadataDoanhThu = {};
    if (this.listBusinessGoalsRevenueDetailChild) {
      for (let i = 0; i < this.listBusinessGoalsRevenueDetailChild.length; i++) {
        let rowData = this.listBusinessGoalsRevenueDetailChild[i];
        let organizationId = rowData.organizationId;
        if (i == 0) {
          this.rowGroupMetadataDoanhThu[organizationId] = { index: 0, size: 1 };
        }
        else {
          let previousRowData = this.listBusinessGoalsRevenueDetailChild[i - 1];
          let previousRowGroup = previousRowData.organizationId;
          if (organizationId === previousRowGroup)
            this.rowGroupMetadataDoanhThu[organizationId].size++;
          else
            this.rowGroupMetadataDoanhThu[organizationId] = { index: i, size: 1 };
        }
      }
    }
  }

  calculate() {
    // Doanh số
    this.totalSalesT1 = 0;
    this.totalSalesT2 = 0;
    this.totalSalesT3 = 0;
    this.totalSalesT4 = 0;
    this.totalSalesT5 = 0;
    this.totalSalesT6 = 0;
    this.totalSalesT7 = 0;
    this.totalSalesT8 = 0;
    this.totalSalesT9 = 0;
    this.totalSalesT10 = 0;
    this.totalSalesT11 = 0;
    this.totalSalesT12 = 0;

    this.listBusinessGoalsSalesDetail.forEach(item => {
      this.totalSalesT1 += ParseStringToFloat(item.january.toString());
      this.totalSalesT2 += ParseStringToFloat(item.february.toString());
      this.totalSalesT3 += ParseStringToFloat(item.march.toString());
      this.totalSalesT4 += ParseStringToFloat(item.april.toString());
      this.totalSalesT5 += ParseStringToFloat(item.may.toString());
      this.totalSalesT6 += ParseStringToFloat(item.june.toString());
      this.totalSalesT7 += ParseStringToFloat(item.july.toString());
      this.totalSalesT8 += ParseStringToFloat(item.august.toString());
      this.totalSalesT9 += ParseStringToFloat(item.september.toString());
      this.totalSalesT10 += ParseStringToFloat(item.october.toString());
      this.totalSalesT11 += ParseStringToFloat(item.november.toString());
      this.totalSalesT12 += ParseStringToFloat(item.december.toString());
    });

    this.listBusinessGoalsSalesDetailChild.forEach(item => {
      this.totalSalesT1 += ParseStringToFloat(item.january.toString());
      this.totalSalesT2 += ParseStringToFloat(item.february.toString());
      this.totalSalesT3 += ParseStringToFloat(item.march.toString());
      this.totalSalesT4 += ParseStringToFloat(item.april.toString());
      this.totalSalesT5 += ParseStringToFloat(item.may.toString());
      this.totalSalesT6 += ParseStringToFloat(item.june.toString());
      this.totalSalesT7 += ParseStringToFloat(item.july.toString());
      this.totalSalesT8 += ParseStringToFloat(item.august.toString());
      this.totalSalesT9 += ParseStringToFloat(item.september.toString());
      this.totalSalesT10 += ParseStringToFloat(item.october.toString());
      this.totalSalesT11 += ParseStringToFloat(item.november.toString());
      this.totalSalesT12 += ParseStringToFloat(item.december.toString());
    });

    //Doanh thu
    this.totalRevenueT1 = 0;
    this.totalRevenueT2 = 0;
    this.totalRevenueT3 = 0;
    this.totalRevenueT4 = 0;
    this.totalRevenueT5 = 0;
    this.totalRevenueT6 = 0;
    this.totalRevenueT7 = 0;
    this.totalRevenueT8 = 0;
    this.totalRevenueT9 = 0;
    this.totalRevenueT10 = 0;
    this.totalRevenueT11 = 0;
    this.totalRevenueT12 = 0;

    this.listBusinessGoalsRevenueDetail.forEach(item => {
      this.totalRevenueT1 += ParseStringToFloat(item.january.toString());
      this.totalRevenueT2 += ParseStringToFloat(item.february.toString());
      this.totalRevenueT3 += ParseStringToFloat(item.march.toString());
      this.totalRevenueT4 += ParseStringToFloat(item.april.toString());
      this.totalRevenueT5 += ParseStringToFloat(item.may.toString());
      this.totalRevenueT6 += ParseStringToFloat(item.june.toString());
      this.totalRevenueT7 += ParseStringToFloat(item.july.toString());
      this.totalRevenueT8 += ParseStringToFloat(item.august.toString());
      this.totalRevenueT9 += ParseStringToFloat(item.september.toString());
      this.totalRevenueT10 += ParseStringToFloat(item.october.toString());
      this.totalRevenueT11 += ParseStringToFloat(item.november.toString());
      this.totalRevenueT12 += ParseStringToFloat(item.december.toString());
    });
    this.listBusinessGoalsRevenueDetailChild.forEach(item => {
      this.totalRevenueT1 += ParseStringToFloat(item.january.toString());
      this.totalRevenueT2 += ParseStringToFloat(item.february.toString());
      this.totalRevenueT3 += ParseStringToFloat(item.march.toString());
      this.totalRevenueT4 += ParseStringToFloat(item.april.toString());
      this.totalRevenueT5 += ParseStringToFloat(item.may.toString());
      this.totalRevenueT6 += ParseStringToFloat(item.june.toString());
      this.totalRevenueT7 += ParseStringToFloat(item.july.toString());
      this.totalRevenueT8 += ParseStringToFloat(item.august.toString());
      this.totalRevenueT9 += ParseStringToFloat(item.september.toString());
      this.totalRevenueT10 += ParseStringToFloat(item.october.toString());
      this.totalRevenueT11 += ParseStringToFloat(item.november.toString());
      this.totalRevenueT12 += ParseStringToFloat(item.december.toString());
    });
  }
}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
