import { Component, OnInit, ViewChild, ElementRef, HostListener, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { CustomerImportComponent } from '../customer-import/customer-import.component';
import { CustomerDownloadTemplateComponent } from '../customer-download-template/customer-download-template.component';
import { SendEmailDialogComponent } from '../../../shared/components/send-email-dialog/send-email-dialog.component';
import { SendSmsDialogComponent } from '../../../shared/components/send-sms-dialog/send-sms-dialog.component';
import { CustomerImportDetailComponent } from '../customer-import-detail/customer-import-detail.component';
import { EmployeeService } from "../../../employee/services/employee.service";
import { TranslateService } from '@ngx-translate/core';
import { CategoryService } from "../../../shared/services/category.service";
import { CustomerService } from '../../services/customer.service';
import { CustomerModel } from "../../models/customer.model";
import { GetPermission } from '../../../shared/permission/get-permission';

import { ngxLoadingAnimationTypes, NgxLoadingComponent } from "ngx-loading";
import { SortEvent } from 'primeng/api';
import { MenuItem } from "primeng/primeng";
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DialogService } from 'primeng/dynamicdialog';
import * as $ from 'jquery';
import * as XLSX from 'xlsx';
import { AddCustomerDialogComponent } from '../add-customer-dialog/add-customer-dialog.component';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { DatePipe, formatNumber } from '@angular/common';
import { ReSearchService } from '../../../services/re-search.service';
import { ListCustomerSearch } from '../../../shared/models/re-search/list-customer-search.model';
import { TemplateQuickEmailComponent } from '../template-quick-email/template-quick-email.component';

declare var google: any; //new added line

class NewCustomerModel {
  public index: number;
  public customerId: string;
  public contactId: string;
  public customerName: string;
  public customerCode: string;
  public picName: string;
  public customerEmail: string;
  public customerPhone: string;
  public countCustomerInfo: number;
  public statusName: string;
  public statusCareName: string;
  public backgroupStatus: string;
  public backgroundStatusCare: string;
  public colorStatusCare: string;
  public longitude: number;
  public latitude: number;
  public createdDate: Date;
  public updatedDate: Date;
}

class CustomerLevel {
  public customerServiceLevelId: string;
  public customerServiceLevelName: string;
}

class Employee {
  public employeeId: string;
  public employeeName: string;
  public employeeCode: string;
  public active: boolean
}

class CustomerGroup {
  public categoryId: string;
  public categoryName: string;
  public categoryCode: string;
}

class ColumnExcel {
  code: string;
  name: string;
  width: number;
}

class SearchCustomerModel {
  public NoPic: boolean;
  public IsBusinessCus: boolean;
  public IsPersonalCus: boolean;
  public IsAgentCus: boolean;
  public StatusCareId: string;
  public CustomerGroupIdList: Array<string>;
  public PersonInChargeIdList: Array<string>;
  public NhanVienChamSocId: string; //vns
  public AreaId: string;
  public FromDate: Date;
  public ToDate: Date;
  public FirstName: string;
  public LastName: string;
  public Phone: string;
  public Email: string;
  public Address: string;
  public KhachDuAn: boolean; //vns
  public KhachBanLe: boolean; //vns

  constructor() {
    this.CustomerGroupIdList = [];
    this.PersonInChargeIdList = [];
  }
}

class importCustomerByExcelModel {
  firstName: string;
  lastName: string;
  isEmployee: boolean;
  isVendor: boolean;
  customerCode: string;
  isCompany: boolean;
  companyName: string;
  address: string;
  taxCode: string;
  phone: string;
  email: string;

  fullNameOfContact: string;
  genderOfContact: boolean;
  addressOfContact: string;
  phoneOfContact: string;
  emailOfContact: string;
  note: string;
}

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
}

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.css'],
  providers: [DialogService, DatePipe]
})
export class CustomerListComponent implements OnInit, AfterViewInit {
  @ViewChild('table') table: Table;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  applicationName = this.systemParameterList.find(x => x.systemKey == 'ApplicationName').systemValueString;

  filterGlobal: string;
  innerWidth: number = 0; //number window size first
  loadingComponent: boolean = false;
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  customerCode: string = '';
  startDate: Date = new Date();
  minYear: number = 2015;
  currentYear: number = (new Date()).getFullYear();
  endDate: Date = new Date();
  maxEndDate: Date = new Date();
  selectedColumns: any[];
  colsCustomer: any[];
  listCustomer: Array<NewCustomerModel> = [];
  rows = 10;
  selectedCustomers: Array<NewCustomerModel> = [];
  actions: MenuItem[] = [];
  filterForm: FormGroup;
  //master data
  customerGroups: Array<CustomerGroup> = [];
  serviceLevels: Array<CustomerLevel> = [];
  employees: Array<Employee> = [];
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';

  isManager: boolean = null;
  employeeId: string = '';

  userPermission: any = localStorage.getItem("UserPermission").split(',');
  createPermission: string = 'customer/create';
  importPermission: string = 'import/create';
  exportPermission: string = 'export/create';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  selection: SelectionModel<CustomerModel>;
  selectedCustomerIdList: Array<string> = [];

  customerGroupCode: string = 'NHA';
  pics: Array<any> = [];
  filteredServiceLevel: Observable<string[]>;
  filteredCustomerGroup: Observable<string[]>;
  filteredPic: Observable<string[]>;
  serviceLevelCtrl = new FormControl();
  customerGroupCtrl = new FormControl();
  picCtrl = new FormControl();
  selectedServiceLevel: Array<any> = [];
  selectedCustomerGroup: Array<any> = [];

  customerGroup: string = '';
  serviceLevel: string = '';
  taxCode: string = '';

  /* Action*/
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  actionDelete: boolean = true;
  actionEdit: boolean = true;
  actionImport: boolean = true;
  actionSMS: boolean = true;
  actionEmail: boolean = true;
  /*END*/

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  paramUrl: any;
  noPic: boolean = false;
  businessOnly: boolean = true;
  personalOnly: boolean = true;
  hkd: boolean = true;
  customerLevelName: string;

  //map
  currentMode: boolean = true;
  options: any = {};
  overlays: any[] = [];
  map: google.maps.Map;
  zoom: number = 16;
  displayCustomerDetailDialog: boolean = false;
  selectedCustomerOnMap: NewCustomerModel = new NewCustomerModel();

  //import varriables
  displayChooseFileImportDialog: boolean = false;
  fileName: string = '';
  importFileExcel: any = null;

  listStatusCare: Array<any> = [];
  listSource: Array<any> = [];
  listProvince: Array<any> = [];

  @ViewChild('ngxLoading') ngxLoadingComponent: NgxLoadingComponent;
  public ngxLoadingAnimationTypes = ngxLoadingAnimationTypes;
  loadingConfig: any = {
    'animationType': ngxLoadingAnimationTypes.circle,
    'backdropBackgroundColour': 'rgba(0,0,0,0.1)',
    'backdropBorderRadius': '4px',
    'primaryColour': '#ffffff',
    'secondaryColour': '#999999',
    'tertiaryColour': '#ffffff'
  }
  selectedCustomerList: Array<any> = [];

  constructor(
    private ref: ChangeDetectorRef,
    private dialogService: DialogService,
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private translate: TranslateService,
    private customerService: CustomerService,
    private employeeService: EmployeeService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private datePipe: DatePipe,
    public reSearchService: ReSearchService
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    //Kiểm tra nhân viên hoặc quản lý
    this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
    this.employeeId = JSON.parse(localStorage.getItem('auth')).EmployeeId;

    //Check permission
    let resource = "crm/customer/list";
    let permission: any = await this.getPermission.getPermission(resource);

    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("download") == -1) {
        this.actionDownload = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("import") == -1) {
        this.actionImport = false;
      }
      if (listCurrentActionResource.indexOf("sms") == -1) {
        this.actionSMS = false;
      }
      if (listCurrentActionResource.indexOf("email") == -1) {
        this.actionEmail = false;
      }
      this.initTable();
      this.initForm();
      this.getMasterData();
    }
  }

  ngAfterViewInit() {
    this.ref.detectChanges();
  }

  setMap(event) {
    this.map = event.map;
  }

  initTable() {
    this.selectedColumns = [
      { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left', display: 'table-cell' },
      { field: 'customerEmail', header: 'Email', textAlign: 'left', display: 'table-cell' },
      { field: 'customerPhone', header: 'Số điện thoại', textAlign: 'left', display: 'table-cell' },
      { field: 'provinceName', header: 'Khu vực', textAlign: 'left', display: 'table-cell' },
      { field: 'createdDate', header: 'Ngày tạo', textAlign: 'left', display: 'table-cell' },
      { field: 'updatedDate', header: 'Ngày cập nhật', textAlign: 'left', display: 'table-cell' },
    ];
    this.colsCustomer = this.selectedColumns;
  }

  initForm() {
    this.filterForm = new FormGroup({
      'Name': new FormControl(''),
      'Phone': new FormControl(''),
      'Email': new FormControl(''),
      'Address': new FormControl(''),
      'FromDate': new FormControl(null),
      'ToDate': new FormControl(null),
      'Area': new FormControl(null),
      'Status': new FormControl(null),
      'Group': new FormControl(null),
      'Pic': new FormControl(null),
      'Care': new FormControl(null),
      'HavePic': new FormControl(false),
      'IsCompanyCustomer': new FormControl(this.applicationName == 'VNS' ? false : true),
      'IsPersonalCustomer': new FormControl(this.applicationName == 'VNS' ? false : true),
      'IsAgentCustomer': new FormControl(this.applicationName == 'VNS' ? false : true),
      'khachDuAn': new FormControl(false),
      'khachBanLe': new FormControl(false),
    });
  }

  resetTable() {
    this.filterGlobal = '';
    if (!this.table) return;
    this.table.sortOrder = 0;
    this.table.sortField = '';
    this.table.reset();
  }

  resetForm() {
    this.filterForm.patchValue({
      'Name': '',
      'Phone': '',
      'Email': '',
      'Address': '',
      'FromDate': null,
      'ToDate': null,
      'Area': null,
      'Status': null,
      'Group': null,
      'Pic': null,
      'Care': null,
      'HavePic': false,
      'IsCompanyCustomer': this.applicationName == 'VNS' ? false : true,
      'IsPersonalCustomer': this.applicationName == 'VNS' ? false : true,
      'IsAgentCustomer': this.applicationName == 'VNS' ? false : true,
      'khachDuAn': false,
      'khachBanLe': false,
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  searchCustomer() {
    //reset table
    this.resetTable();

    let searchCustomerModel = this.mappingFormToModel();

    this.saveParameterSearch(searchCustomerModel);

    this.loadingComponent = true;
    this.customerService.searchCustomer(
      searchCustomerModel.AreaId,
      convertToUTCTime(searchCustomerModel.FromDate),
      convertToUTCTime(searchCustomerModel.ToDate),
      searchCustomerModel.FirstName,
      searchCustomerModel.LastName,
      searchCustomerModel.Phone,
      searchCustomerModel.Email,
      searchCustomerModel.Address,
      searchCustomerModel.KhachDuAn,
      searchCustomerModel.KhachBanLe,
    ).subscribe(response => {
      this.loadingComponent = false;
      let result = <any>response;
      if (result.statusCode === 200) {
        this.listCustomer = result.listCustomer;

        this.setIndex();
        if (this.listCustomer.length === 0) {
          let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy khách hàng nào!' };
          this.showMessage(mgs);
        }
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }

      $(".ui-button-text-empty").css("display", "none")
      $(".ui-button-icon-left").removeClass("pi pi-chevron-down")
      $(".ui-button-icon-left").addClass("pi pi-cog")
    });
  }

  saveParameterSearch(searchCustomerModel: SearchCustomerModel)
  {
    var listCustomerSearch = new ListCustomerSearch();
    listCustomerSearch.noPic = searchCustomerModel.NoPic;
    listCustomerSearch.isBusinessCus = searchCustomerModel.IsBusinessCus;
    listCustomerSearch.isPersonalCus = searchCustomerModel.IsPersonalCus;
    listCustomerSearch.isAgentCus = searchCustomerModel.IsAgentCus;
    listCustomerSearch.statusCareId = searchCustomerModel.StatusCareId;
    listCustomerSearch.customerGroupIdList = searchCustomerModel.CustomerGroupIdList;
    listCustomerSearch.personInChargeId = searchCustomerModel.PersonInChargeIdList.length == 1 ?
      searchCustomerModel.PersonInChargeIdList[0] : null;
    listCustomerSearch.nhanVienChamSocId = searchCustomerModel.NhanVienChamSocId;
    listCustomerSearch.areaId = searchCustomerModel.AreaId;
    listCustomerSearch.fromDate = searchCustomerModel.FromDate ?
      searchCustomerModel.FromDate.toISOString().split('T')[0] : null;
    listCustomerSearch.toDate = searchCustomerModel.ToDate ?
      searchCustomerModel.ToDate.toISOString().split('T')[0] : null;
    listCustomerSearch.firstName = searchCustomerModel.FirstName;
    listCustomerSearch.phone = searchCustomerModel.Phone;
    listCustomerSearch.email = searchCustomerModel.Email;
    listCustomerSearch.adddress = searchCustomerModel.Address;

    this.reSearchService.updatedSearchModel('listCustomerSearch', listCustomerSearch);
  }

  setIndex() {
    this.listCustomer.forEach((customer, index) => {
      customer.index = index + 1;
    });
  }

  async getMasterData() {
    this.loadingComponent = true;
    let [getListCustomerAsyncResponse, getEmployeeCareStaffAsyncResponse]: any = await Promise.all([
      this.customerService.getListCustomerAsync("NHA"),
      this.employeeService.getEmployeeCareStaffAsyc(this.isManager, this.employeeId)
    ]);
    //loai khach hang portal
    this.listProvince = getListCustomerAsyncResponse.listProvince;
    this.listStatusCare = getListCustomerAsyncResponse.listStatusCareModel;
    this.listSource = getListCustomerAsyncResponse.listSourceModel;
    this.customerGroups = getListCustomerAsyncResponse.listCategoryModel.filter(e => e.categoryCode !== "POR");
    this.serviceLevels = getListCustomerAsyncResponse.listCustomerServiceLevelModel;
    this.employees = getEmployeeCareStaffAsyncResponse.employeeList;
    let listCustomerSearch: ListCustomerSearch = JSON.parse(localStorage.getItem("listCustomerSearch"));
    if (listCustomerSearch)
    {
      this.mapDataToForm(listCustomerSearch);
    }

    this.searchCustomer();
  }

  /* Hiển thị lại giá trị bộ lọc */
  mapDataToForm(listCustomerSearch: ListCustomerSearch) {
 
    let selectedArea = listCustomerSearch.areaId ?
      this.listProvince.find(x => x.areaId == listCustomerSearch.areaId) : null;
    this.filterForm.get('Area').setValue(selectedArea);

    this.filterForm.get('FromDate').setValue(listCustomerSearch.fromDate ?
      new Date(listCustomerSearch.fromDate) : null);

    this.filterForm.get('ToDate').setValue(listCustomerSearch.toDate ?
      new Date(listCustomerSearch.toDate) : null);

    this.filterForm.get('Name').setValue(listCustomerSearch.firstName);
    this.filterForm.get('Phone').setValue(listCustomerSearch.phone);
    this.filterForm.get('Email').setValue(listCustomerSearch.email);
    this.filterForm.get('Address').setValue(listCustomerSearch.adddress);
  }

  switchMode(event: any) {
    this.ref.detectChanges();
    if (!event.checked) {
      /* chuyển sang mode bản đồ */
      /* lấy những bản ghi có tọa độ hợp lệ */
      let listCustomerPosition = [];
      this.listCustomer.forEach(customer => {
        if (customer.longitude && customer.latitude) listCustomerPosition.push({
          lat: customer.latitude,
          lng: customer.longitude,
          title: customer.customerName,
          customerId: customer.customerId
        });
      });
      this.overlays = [];
      listCustomerPosition.forEach(pos => {
        this.overlays = [...this.overlays, new google.maps.Marker({ position: { lat: pos.lat, lng: pos.lng }, title: pos.title, customerId: pos.customerId })]
      });

      let bounds = new google.maps.LatLngBounds();
      this.overlays.forEach(marker => {
        bounds.extend(marker.getPosition());
      });
      setTimeout(() => { // map will need some time to load
        this.map.setCenter(new google.maps.LatLng(17.429441, 106.376713));
        this.map.setZoom(6);
      }, 1000);
    }
  }

  handleOverlayClick(event) {
    let customerId = event.overlay.customerId;

    let selectedCustomer = this.listCustomer.find(cus => cus.customerId == customerId);
    if (!selectedCustomer) return;

    this.displayCustomerDetailDialog = true;
    this.selectedCustomerOnMap = selectedCustomer;
  }

  mappingFormToModel(): SearchCustomerModel {
    let searchCustomer = new SearchCustomerModel();
    searchCustomer.FirstName = this.filterForm.get('Name').value;
    searchCustomer.LastName = '';
    searchCustomer.Phone = this.filterForm.get('Phone').value;
    searchCustomer.Email = this.filterForm.get('Email').value;
    searchCustomer.Address = this.filterForm.get('Address').value;
    searchCustomer.KhachDuAn = this.filterForm.get('khachDuAn').value;
    searchCustomer.KhachBanLe = this.filterForm.get('khachBanLe').value;
   
    //get khu vuc
    let area = this.filterForm.get('Area').value;
    searchCustomer.AreaId = area != null ? area.map(x => x.provinceId) : null  ;

    // get thoi gian tao
    searchCustomer.FromDate = this.filterForm.get('FromDate').value;
    searchCustomer.ToDate = this.filterForm.get('ToDate').value;

    return searchCustomer;
  }

  goToCreate() {
    this.router.navigate(['/customer/create']);
  }

  openAddCustomerDialog() {
    let width = '';
    if (window.innerWidth < 500) {
      width = '100%';
    } else {
      width = '70%';
    }

    let ref = this.dialogService.open(AddCustomerDialogComponent, {
      data: {
        isCreate: true,
        orderDate: new Date()
      },
      header: 'Thêm khách hàng',
      width: width,
      baseZIndex: 1030,
      closeOnEscape: false,
      closable: false,
      contentStyle: {
        "min-height": "300px",
        "max-height": "700px",
        "overflow": "auto !important",
      }
    });

    ref.onClose.subscribe(async (result: ResultDialog) => {
      if (result) {
        if (result.status) {
          let msg1 = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo khách hàng thành công' };
          this.showMessage(msg1);
          this.searchCustomer();
        } else {
          this.searchCustomer();
        }
      }
    });
  }

  //redirect đến trang tạo báo giá
  goToNewCustomerQuote() {
    this.router.navigate(['/customer/quote-create']);
  }

  goToCustomerQuote(customerId: string) {
    this.router.navigate(['/customer/quote-create', {
      customerId: customerId
    }]);
  }

  goToLead(customerId: string) {
    this.router.navigate(['/lead/create-lead', {
      customerId: customerId
    }]);
  }

  //redirect đến trang tạo Don Hang
  goToNewCustomerOrder() {
    this.router.navigate(['/order/create']);
  }


  goToCustomerOrder(customerId: string) {
    this.router.navigate(['/order/create', {
      customerId: customerId
    }]);
  }



  importCustomer() {
    this.displayChooseFileImportDialog = true;
  }

  del_customer(customerId: string) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loadingComponent = true;
        this.customerService.changeCustomerStatusToDelete(customerId, this.auth.UserId).subscribe(response => {
          this.loadingComponent = false;
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            //remove from list
            this.listCustomer = this.listCustomer.filter(e => e.customerId != customerId);
            let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Xóa khách hàng thành công' };
            this.showMessage(mgs);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
            this.showMessage(mgs);
          }
        }, error => { this.loadingComponent = false; });
      }
    });
  }

  downloadTemplate() {
    let ref = this.dialogService.open(CustomerDownloadTemplateComponent, {
      header: 'Tải mẫu Excel',
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "200px",
        "max-height": "600px"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          if (result.message == "success") {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tải thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        }
      }
    });
  }

  //apply new UI
  refreshFilter() {
    this.resetForm();
    this.searchCustomer();
  }

  leftColNumber: number = 12;
  rightColNumber: number = 0;
  showFilter() {
    if (this.innerWidth < 768) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.rightColNumber = 4;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

  pageChange(event: any) {
  }

  goToDetail(rowData: any) {
    this.router.navigate(['/customer/detail', { customerId: rowData.customerId, contactId: rowData.contactId }]);
  }

  checkEnterPress(event: any) {
    if (event.code === "Enter") {
      this.searchCustomer();
    }
  }

  closeChooseFileImportDialog() {
    this.cancelFile();
  }

  cancelFile() {
    let fileInput = $("#importFileProduct")
    fileInput.replaceWith(fileInput.val('').clone(true));
    this.fileName = "";
  }

  async downloadTemplateExcel() {
    this.loadingComponent = true;
    let result: any = await this.customerService.downloadTemplateImportCustomer(this.auth.UserId);
    this.loadingComponent = false;

    if (result.templateExcel != null && result.statusCode === 200) {
      const binaryString = window.atob(result.templateExcel);
      const binaryLen = binaryString.length;
      const bytes = new Uint8Array(binaryLen);
      for (let idx = 0; idx < binaryLen; idx++) {
        const ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      const fileName = result.fileName + ".xls";
      link.download = fileName;
      link.click();
    } else {
      this.displayChooseFileImportDialog = false;
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Download file thất bại' };
      this.showMessage(msg);
    }
  }

  chooseFile(event: any) {
    this.fileName = event.target?.files[0]?.name;
    this.importFileExcel = event.target;
  }

  onClickImportBtn(event: any) {
    /* clear value của file input */
    event.target.value = ''
  }

  importExcel() {
    if (this.fileName == "") {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Chưa chọn file cần nhập" };
      this.showMessage(mgs);
      return;
    }

    const targetFiles: DataTransfer = <DataTransfer>(this.importFileExcel);
    const reader: FileReader = new FileReader();
    reader.readAsBinaryString(targetFiles.files[0]);
    reader.onload = (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });
      let code = 'Danh mục KH';
      if (!workbook.Sheets[code]) {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: "File không hợp lệ" };
        this.showMessage(mgs);
        return;
      }

      //lấy data từ file excel
      const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[code];
      /* save data */
      let listCustomerRawData: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
      /* remove header */
      listCustomerRawData = listCustomerRawData.filter((e, index) => index != 0);
      /* nếu không nhập 2 trường required: tên + mã khách hàng thì loại bỏ */
      listCustomerRawData = listCustomerRawData.filter(e => (e[1] && e[4]));
      /* chuyển từ raw data sang model */
      let listCustomerImport: Array<importCustomerByExcelModel> = [];

      listCustomerRawData?.forEach(_rawData => {
        /*
      0: "Họ đệm "
      1: "Tên *"
      2: "Là nhân viên"
      3: "Là nhà cung cấp"
      4: "Mã khách hàng (*)"
      5: "Là doanh nghiệp?"
      6: "Thuộc Công Ty"
      7: "Địa chỉ"
      8: "Mã số thuế"
      9: "Điện thoại"
      10: "Email"
      11: "Họ và tên NLH"
      12: "Giới tính NLH"
      13: "Địa chỉ người liên hệ"
      14: "ĐT di động NLH"
      15: "Email người liên hệ"
      16: "Ghi chú"
       */
        let customer = new importCustomerByExcelModel();
        customer.firstName = _rawData[0] ? _rawData[0].toString().trim() : '';
        customer.lastName = _rawData[1] ? _rawData[1].toString().trim() : '';
        customer.isEmployee = _rawData[2] == "1" ? true : false;
        customer.isVendor = _rawData[3] == "1" ? true : false;
        customer.customerCode = _rawData[4] ? _rawData[4].toString().trim() : '';
        customer.isCompany = _rawData[5] == "1" ? true : false;
        customer.companyName = _rawData[6] ? _rawData[6].toString().trim() : '';
        customer.address = _rawData[7] ? _rawData[7].toString().trim() : '';
        customer.taxCode = _rawData[8] ? _rawData[8].toString().trim() : '';
        customer.phone = _rawData[9] ? _rawData[9].toString().trim() : '';
        customer.email = _rawData[10] ? _rawData[10].toString().trim() : '';
        customer.fullNameOfContact = _rawData[11] ? _rawData[11].toString().trim() : '';
        customer.genderOfContact = _rawData[12] ? _rawData[12].toString().trim() : '';
        customer.addressOfContact = _rawData[13] ? _rawData[13].toString().trim() : '';
        customer.phoneOfContact = _rawData[14] ? _rawData[14].toString().trim() : '';
        customer.emailOfContact = _rawData[15] ? _rawData[15].toString().trim() : '';
        customer.note = _rawData[16] ? _rawData[16].toString().trim() : '';

        listCustomerImport = [...listCustomerImport, customer];
      });
      /* tắt dialog import file, bật dialog chi tiết khách hàng import */
      this.displayChooseFileImportDialog = false;
      this.openDetailImportDialog(listCustomerImport);
    }
  }

  openDetailImportDialog(listCustomerImport) {
    let ref = this.dialogService.open(CustomerImportDetailComponent, {
      data: {
        isPotentialCustomer: false,
        listCustomerImport: listCustomerImport
      },
      header: 'Import Khách hàng',
      width: '85%',
      baseZIndex: 1050,
      contentStyle: {
        "max-height": "800px",
        // "over-flow": "hidden"
      }
    });
    ref.onClose.subscribe((result: any) => {
      if (result?.status) {
        this.getMasterData();
      }
    });
  }

  exportExcel() {
    if (this.selectedCustomers.length > 0) {
      if (this.selectedCustomers.length > 0) {
        let title = `Danh sách khách hàng`;

        let workBook = new Workbook();
        let worksheet = workBook.addWorksheet(title);

        let dataHeaderMain = "Danh sách khách hàng".toUpperCase();
        let headerMain = worksheet.addRow([dataHeaderMain]);
        headerMain.font = { size: 18, bold: true };
        worksheet.mergeCells(`A${1}:G${1}`);
        headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        worksheet.addRow([]);

        /* Header row */
        let buildColumnExcel = this.buildColumnExcel();
        let dataHeaderRow = buildColumnExcel.map(x => x.name);
        let headerRow = worksheet.addRow(dataHeaderRow);
        headerRow.font = { name: 'Times New Roman', size: 12, bold: true };
        dataHeaderRow.forEach((item, index) => {
          headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          headerRow.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: '8DB4E2' }
          };
        });
        headerRow.height = 40;

        /* Data table */
        let data: Array<any> = [];
        this.selectedCustomers.forEach(item => {
          let row: Array<any> = [];
          buildColumnExcel.forEach((_item, _index) => {
            if (_item.code == 'customerName')
            {
              row[_index] = item.customerName;
            }
            else if (_item.code == 'customerEmail')
            {
              row[_index] = item.customerEmail;
            }
            else if (_item.code == 'customerPhone')
            {
              row[_index] = item.customerPhone;
            }
            else if (_item.code == 'createdDate')
            {
              row[_index] = this.datePipe.transform(item.createdDate, 'dd/MM/yyyy');
            }
            else if (_item.code == 'updatedDate')
            {
              row[_index] = this.datePipe.transform(item.updatedDate, 'dd/MM/yyyy');
            }
            else if (_item.code == 'statusCareName')
            {
              row[_index] = item.statusCareName;
            }
            else if (_item.code == 'picName')
            {
              row[_index] = item.picName;
            }
          });
          data.push(row);
        });

        data.forEach((el, index, array) => {
          let row = worksheet.addRow(el);
          row.font = { name: 'Times New Roman', size: 12 };

          buildColumnExcel.forEach((_item, _index) => {
            row.getCell(_index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
            if (_item.code == 'statusCareName') {
              row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
            }
            else {
              row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
            }
          });
        });

        /* fix with for column */
        buildColumnExcel.forEach((item, index) => {
          worksheet.getColumn(index + 1).width = item.width;
        });

        this.exportToExel(workBook, title);
      }
      else {
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn phải chọn ít nhất 1 cột' };
        this.showMessage(msg);
      }
    }
    else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn chưa chọn khách hàng' };
      this.showMessage(msg);
    }
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  buildColumnExcel(): Array<ColumnExcel> {
    let listColumn: Array<ColumnExcel> = [];

    this.selectedColumns.forEach(item => {
      let column = new ColumnExcel();
      column.code = item.field;
      column.name = item.header;

      if (item.field == 'customerName') {
        column.width = 35.71;
      }
      else if (item.field == 'customerEmail') {
        column.width = 30.71;
      }
      else if (item.field == 'customerPhone') {
        column.width = 20.71;
      }
      else if (item.field == 'createdDate') {
        column.width = 25.71;
      }
      else if (item.field == 'updatedDate') {
        column.width = 15.71;
      }
      else if (item.field == 'statusCareName') {
        column.width = 20.71;
      }
      else if (item.field == 'picName') {
        column.width = 35.71;
      }

      listColumn.push(column);
    });

    return listColumn;
  }

}

function convertToUTCTime(time: any) {
  if (time)
    return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
  else
    return null;
};
