import { Component, OnInit, ViewChild, ElementRef, Renderer2 } from '@angular/core';
import { DatePipe } from '@angular/common';

import { Router, ActivatedRoute } from '@angular/router';
import { CompanyConfigModel } from '../shared/models/companyConfig.model';
import { BreadCrumMenuModel } from '../shared/models/breadCrumMenu.model';
import { ChangepasswordComponent } from '../shared/components/changepassword/changepassword.component';
import { UserprofileComponent } from "../userprofile/userprofile.component"
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { EventEmitterService } from '../shared/services/event-emitter.service';
import { NotificationService } from '../shared/services/notification.service';
import { DashboardHomeService } from '../shared/services/dashboard-home.service';
import { AuthenticationService } from '../shared/services/authentication.service';
// Full canlendar
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { ConfirmationService, MessageService } from 'primeng/api';
import { MenuItem } from 'primeng/api';

import * as $ from 'jquery';
import { GetPermission } from '../shared/permission/get-permission';
import { FullCalendar } from 'primeng/fullcalendar';
import { MeetingDialogComponent } from '../customer/components/meeting-dialog/meeting-dialog.component';
import { DialogService } from 'primeng';
import { ReSearchService } from '../services/re-search.service';
import { FireBaseService } from '../shared/services/fire-base.service';
import * as firebase from 'firebase';

class delayProductionOrder {
  productionOrderId: string;
  productionOrderCode: string;
  customerName: string;
  areaRemain: number;
  endDate: Date;
}

class totalProductionOrderInDashBoard {
  customerName: string;
  endDate: Date;
  statusCode: string;
  listTotalQuantityByTechniqueRequest: Array<totalQuantityByTechniqueRequestModel>;
  productionOrderCode: string;
  productionOrderId: string;
  statusName: string;
  isDeplay: boolean;
  constructor() {
    this.listTotalQuantityByTechniqueRequest = [];
    this.isDeplay = true;
  }
}

class totalQuantityByTechniqueRequestModel {
  techniqueRequestId: string;
  totalQuantityCompleted: number;
  totalQuantityHaveToComplete: number;
  statusName: string;//trạng thái hiển thị trên table
}

class techniqueRequest {
  techniqueRequestId: string;
  techniqueName: string;
  techniqueRequestCode: string;
  completeAreaInDay: number;
}

interface Quote {
  quoteId: string;
  quoteCode: string;
  amount: number;
  objectTypeId: string;
  objectType: string;
  customerName: string;
  customerContactId: string;
  seller: string;
  sellerName: string;
  quoteDate: Date;
}

interface Customer {
  customerId: string;
  customerName: string;
  customerPhone: string;
  customerEmail: string;
  picName: string;
  picContactId: string;
  dateOfBirth: Date;
}

interface Order {
  orderId: string;
  orderCode: string;
  customerId: string;
  customerName: string;
  amount: number;
  seller: string;
  sellerName: string;
  sellerContactId: string;
}

interface CustomerMeeting {
  customerMeetingId: string;
  customerId: string;
  employeeId: string;
  startDate: Date;
  endDate: Date;
  title: string;
  content: string;
  locationMeeting: string;
  customerName: string;
  employeeName: string;
  createByName: string;
  participants: string;
  isCreateByUser: boolean;
}

interface LeadMeeting {
  leadMeetingId: string;
  leadId: string;
  employeeId: string;
  startDate: Date;
  endDate: Date;
  title: string;
  content: string;
  locationMeeting: string;
  leadName: string;
  employeeName: string;
  createByName: string;
  isShowLink: boolean;
}

interface Employee {
  employeeId: string;
  employeeName: string;
  organizationId: string;
  organizationName: string;
  positionId: string;
  positionName: string;
  dateOfBirth: Date;
  contactId: string;
}

class Calendar {
  id: string;
  title: string;
  start: Date;
  end: Date;
  backgroundColor: string;
  participants: string;
  isCreateByUser: boolean;
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  providers: [
    DatePipe
  ]
})
export class HomeComponent implements OnInit {
  @ViewChild('toggleNotifi') toggleNotifi: ElementRef;
  isOpenNotifi: boolean = false;

  @ViewChild('toggleConfig') toggleConfig: ElementRef;
  isOpenConfig: boolean = false;

  @ViewChild('toggleCreateElement') toggleCreateElement: ElementRef;
  isOpenCreateElement: boolean = false;

  @ViewChild('toggleUser') toggleUser: ElementRef;
  isOpenUser: boolean = false;

  @ViewChild('dropdownMenus') dropdownMenus: ElementRef;

  @ViewChild('calendar') calendar: FullCalendar;
  /**/
  listPermissionResource: Array<string> = localStorage.getItem('ListPermissionResource').split(',');
  listPermissionResourceActive: string = localStorage.getItem("ListPermissionResource");
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  applicationName = this.getDefaultApplicationName();
  listModule: Array<string> = [];
  listResource: Array<string> = [];
  lstBreadCrumLeftMenu: Array<BreadCrumMenuModel> = [];
  lstBreadCrum: Array<BreadCrumMenuModel> = [];
  listParticipants: Array<Employee> = [];
  /*Module name*/
  moduleCrm = 'crm'; //Module Quản trị quan hệ khách hàng
  moduleSal = 'sal'; //Module Quản lý bán hàng
  moduleBuy = 'buy'; //Module Quản lý mua hàng
  moduleAcc = 'acc'; //Module Quản lý tài chính
  moduleRec = 'rec'; //Module Quản lý tuyển dụng
  moduleHrm = 'hrm'; //Module Quản trị nhân sự
  moduleSalary = 'salary'; //Module Quản lý lương
  moduleAss = 'ass'; //Module Quản lý tài sản
  moduleSys = 'sys'; //Module Quản trị hệ thống

  /*End*/

  isCustomer = false;
  isSales = false;
  isShopping = false;
  isAccounting = false;
  isHrm = false;
  isWarehouse = false;
  isProject = false;
  companyConfigModel = new CompanyConfigModel();
  isToggleCick: Boolean = false;

  notificationNumber: number = 0;
  NotificationContent: string;
  notificationList: Array<any> = [];
  auth: any = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;

  username: string;
  userAvatar: string;
  userFullName: string;
  userEmail: string;
  dialogRef: MatDialogRef<ChangepasswordComponent>;
  dialogPopup: MatDialogRef<UserprofileComponent>;

  // full calendar
  events: Array<Calendar> = [];
  options: any;

  lstSubmenuLevel3: Array<BreadCrumMenuModel> = [
    //Quan ly he thong
    { Name: "Cấu hình mobile", Path: "/admin/mobile-app-config", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "settings", IsDefault: true, CodeParent: "sys_chttc", LstChildren: [], Display: "none", Code: '' },
    // { Name: "Cấu hình thông tin chung", Path: "/admin/company-config", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "settings", IsDefault: true, CodeParent: "sys_chttc", LstChildren: [], Display: "none", Code: '' },
    // { Name: "Cấu hình thư mục", Path: "/admin/folder-config", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "settings", IsDefault: true, CodeParent: "sys_chtm", LstChildren: [], Display: "none", Code: '' },
    // { Name: "Quản lý thông báo", Path: "/admin/notifi-setting-list", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "settings", IsDefault: true, CodeParent: "sys_tb", LstChildren: [], Display: "none", Code: '' },
    { Name: "Tham số hệ thống", Path: "/admin/system-parameter", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "settings_applications", IsDefault: true, CodeParent: "sys_tsht", LstChildren: [], Display: "none", Code: '' },
    // { Name: "Quản lý mẫu Email", Path: "/admin/email-configuration", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "device_hub", IsDefault: true, CodeParent: "Systems_QLE", LstChildren: [], Display: "none", Code: '' },
    // { Name: "Quản lý sơ đồ tổ chức", Path: "/admin/organization", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "device_hub", IsDefault: true, CodeParent: "sys_sdtc", LstChildren: [], Display: "none", Code: '' },
    { Name: "Quản lý dữ liệu danh mục", Path: "/admin/masterdata", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "category", IsDefault: true, CodeParent: "sys_dldm", LstChildren: [], Display: "none", Code: '' },
    { Name: "Quản lý nhóm quyền", Path: "/admin/permission", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "format_list_bulleted", IsDefault: true, CodeParent: "sys_nq", LstChildren: [], Display: "none", Code: '' },
    // { Name: "Quản lý quy trình làm việc", Path: "/admin/danh-sach-quy-trinh", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "swap_horiz", IsDefault: true, CodeParent: "sys_qtlv", LstChildren: [], Display: "none", Code: '' },
  ];

  /*Data chart revenue employee*/
  dataRevenueEmployee: any;
  optionsRevenueEmployee: any;
  /*End*/

  /*Data chart Chance*/
  dataChance: any;
  optionsDataChance: any;
  /*End*/

  isManager: boolean = false;

  colsQuote: Array<any> = [];
  colsCustomer: Array<any> = [];
  colsOrder: Array<any> = [];
  colsCustomerMeeting: Array<any> = [];
  colsCusBirthdayOfWeek: Array<any> = [];
  colsEmployeeBirthDayOfWeek: Array<any> = [];
  selectedColsCustomerMeeting: Array<any> = [];
  colsLeadMeeting: Array<any> = [];
  selectedColsLeadMeeting: Array<any> = [];

  totalSalesOfWeek: number = 0;
  totalSalesOfMonth: number = 0;
  totalSalesOfQuarter: number = 0;
  totalSalesOfYear: number = 0;
  chiTieuDoanhThuTuan: number = 0;
  chiTieuDoanhThuThang: number = 0;
  chiTieuDoanhThuQuy: number = 0;
  chiTieuDoanhThuNam: number = 0;

  totalSalesOfWeekPress: number = 0;
  totalSalesOfMonthPress: number = 0;
  totalSalesOfQuarterPress: number = 0;
  totalSalesOfYearPress: number = 0;

  salesOfWeek: number = 0;
  salesOfMonth: number = 0;
  salesOfQuarter: number = 0;
  salesOfYear: number = 0;

  listQuote: Array<Quote> = [];
  listCustomer: Array<Customer> = [];
  listOrderNew: Array<Order> = [];
  listCustomerMeeting: Array<CustomerMeeting> = [];
  listLeadMeeting: Array<LeadMeeting> = [];
  listCusBirthdayOfWeek: Array<Customer> = [];
  listEmployeeBirthDayOfWeek: Array<Employee> = [];
  lstSubmenuLevel1: Array<BreadCrumMenuModel> = [
    //Module Nhân sự
    {
      Name: "Nhân sự", Path: "", ObjectType: "hrm", LevelMenu: 1, Active: false, nameIcon: "fa-users", IsDefault: false, CodeParent: "Employee_Module", Display: "none",
      LstChildren: [
        {
          Name: "Nhân viên", Path: "", ObjectType: "employee", LevelMenu: 2, Active: false, nameIcon: "fa-user-circle", IsDefault: false, CodeParent: "hrm_nv", Display: "none", LstChildren: [
            { Name: "Tạo nhân viên", Path: "/employee/create", ObjectType: "HRM", LevelMenu: 3, Active: false, nameIcon: "person_add", IsDefault: false, CodeParent: "hrm_nv", LstChildren: [], Display: "none", Code: '' },
            { Name: "Danh sách nhân viên", Path: "/employee/list", ObjectType: "HRM", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: false, CodeParent: "hrm_nv", LstChildren: [], Display: "none", Code: 'HRM_EmployeeTK' },
            // { Name: "Danh sách nhân viên đã nghỉ việc", Path: "/employee/employee-quit-work", ObjectType: "HRM", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: false, CodeParent: "hrm_nv", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
        {
          Name: "Sơ đồ tổ chức", Path: "", ObjectType: "employee", LevelMenu: 2, Active: false, nameIcon: "fas fa-users", IsDefault: false, CodeParent: "hrm_nv", Display: "none", LstChildren: [
            { Name: "Sơ đồ tổ chức", Path: "/employee/organization", ObjectType: "HRM", LevelMenu: 3, Active: false, nameIcon: "person_add", IsDefault: false, CodeParent: "hrm_nv", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
          
        },
      ],
       Code: ''
    },
    //Module CRM
    {
      Name: "Quản trị khách hàng", Path: "", ObjectType: "crm", LevelMenu: 1, Active: false, nameIcon: "fa-street-view", IsDefault: false, CodeParent: "Lead_QLKHTN_Module", Display: "none",
      LstChildren: [
        {
          Name: "Khách hàng", Path: "", ObjectType: "customer", LevelMenu: 2, Active: false, nameIcon: "fa-user", IsDefault: false, CodeParent: "crm_kh", Display: "none",
          LstChildren: [
            // { Name: "Dashboard", Path: "/customer/dashboard", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u39.png", IsDefault: true, CodeParent: "crm_kh", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tạo mới", Path: "/customer/create", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u43.png", IsDefault: false, CodeParent: "crm_kh", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm", Path: "/customer/list", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "crm_kh", LstChildren: [], Display: "none", Code: 'Customer_TK' },
          ], Code: ''
        },

      ], Code: ''
    },
    
   // Module nhà cung cấp
   {
    Name: "Quản lý nhà cung cấp", Path: "", ObjectType: "buy", LevelMenu: 1, Active: false, nameIcon: "fa-shopping-cart", IsDefault: false, CodeParent: "Shopping_Module", Display: "none",
    LstChildren: [

      {
        Name: "Nhà cung cấp", Path: "", ObjectType: "shopping", LevelMenu: 2, Active: false, nameIcon: "fa-binoculars", IsDefault: false, CodeParent: "buy_ncc", Display: "none",
        LstChildren: [
          { Name: "Tạo mới nhà cung cấp", Path: "/vendor/create", ObjectType: "shop", LevelMenu: 3, Active: false, nameIcon: "fa fa-plus-square-o", IsDefault: false, CodeParent: "Shopping_QLNCC", LstChildren: [], Display: "none", Code: '' },
          { Name: "Tìm kiếm nhà cung cấp", Path: "/vendor/list", ObjectType: "shop", LevelMenu: 3, Active: false, nameIcon: "fa fa-search", IsDefault: true, CodeParent: "Shopping_QLNCC", LstChildren: [], Display: "none", Code: 'Shop_Vendor_TK' },
        ], Code: ''
      }
    ], Code: ''
  },


    //Gói dịch vụ
    {
      Name: "Quản lý dịch vụ", Path: "", ObjectType: "sal", LevelMenu: 1, Active: false, nameIcon: "fas fa-window-restore", IsDefault: false, CodeParent: "Lead_QLKHTN_Module", Display: "none",
      LstChildren: [
        {
          Name: "Quản lý loại gói dịch vụ", Path: "", ObjectType: "product", LevelMenu: 2, Active: false, nameIcon: "", IsDefault: false, CodeParent: "productCategory", Display: "none",
          LstChildren: [
            { Name: "Tìm kiếm", Path: "/admin/list-product-category", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "fa-book", IsDefault: false, CodeParent: "productCategory", LstChildren: [], Display: "none", Code: 'Customer_TK' },
          ], Code: ''
        },
        {
          Name: "Tùy chọn dịch vụ", Path: "", ObjectType: "product", LevelMenu: 2, Active: false, nameIcon: "", IsDefault: false, CodeParent: "options", Display: "none",
          LstChildren: [
            { Name: "Tìm kiếm", Path: "/product/product-option-list", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "fa-book", IsDefault: false, CodeParent: "options", LstChildren: [], Display: "none", Code: 'Customer_TK' },
            { Name: "Tạo mới", Path: "/product/product-option-detail", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "note_add", IsDefault: false, CodeParent: "options", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
        {
          Name: "Gói dịch vụ", Path: "", ObjectType: "product", LevelMenu: 2, Active: false, nameIcon: "", IsDefault: false, CodeParent: "servicePacket", Display: "none",
          LstChildren: [
            { Name: "Tìm kiếm", Path: "/product/list-product-packet", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "fa-book", IsDefault: false, CodeParent: "servicePacket", LstChildren: [], Display: "none", Code: 'Customer_TK' },
            { Name: "Tạo mới", Path: "/product/product-packet-createOrUpdate", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "note_add", IsDefault: false, CodeParent: "servicePacket", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
      ], Code: ''
    },
    //Module quản lý đặt dịch vụ
    {
      Name: "Quản lý đặt dịch vụ", Path: "", ObjectType: "cusOrder", LevelMenu: 1, Active: false, nameIcon: "fa-university", IsDefault: false, CodeParent: "Sale_Module", Display: "none",
      LstChildren: [
        {
          Name: "Quản lý đặt dịch vụ", Path: "", ObjectType: "customerOrder", LevelMenu: 2, Active: false, nameIcon: "", IsDefault: false, CodeParent: "customerOrder_orderProcess", Display: "none",
          LstChildren: [
            { Name: "Tìm kiếm", Path: "/order/orderProcessList", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: false, CodeParent: "customerOrder_orderProcess", LstChildren: [], Display: "none", Code: 'Sale_Order_TK' },
            { Name: "Tạo mới", Path: "/order/orderProcess", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "note_add", IsDefault: false, CodeParent: "customerOrder_orderProcess", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
        {
          Name: "Phiếu yêu cầu dịch vụ", Path: "", ObjectType: "customerOrder", LevelMenu: 2, Active: false, nameIcon: "", IsDefault: false, CodeParent: "customerOrder_order", Display: "none",
          LstChildren: [
            { Name: "Tìm kiếm", Path: "/order/list", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: false, CodeParent: "customerOrder_order", LstChildren: [], Display: "none", Code: 'Sale_Order_TK' },
          ], Code: ''
        },
        {
          Name: "Phiếu hỗ trợ dịch vụ", Path: "", ObjectType: "customerOrder", LevelMenu: 2, Active: false, nameIcon: "", IsDefault: false, CodeParent: "customerOrder_orderAction", Display: "none",
          LstChildren: [
            { Name: "Tìm kiếm", Path: "/order/orderActionList", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: false, CodeParent: "customerOrder_orderAction", LstChildren: [], Display: "none", Code: 'Sale_Order_TK' },
          ], Code: ''
        },
      ], Code: ''
    }
  ];

  items: MenuItem[] = [
    {
      label: 'CRM',
      items: [
        { label: 'Tạo Khách hàng', routerLink: '/customer/create' },
      ]
    },
    {
      label: 'Bán hàng',
      items: [
        { label: 'Tạo sản phẩm', routerLink: '/product/createOrEdit' },
        // { label: 'Tạo hợp đồng bán', routerLink: '/sales/contract-create' },
        { label: 'Tạo mới', routerLink: '/order/create' },
        // { label: 'Tạo hóa đơn', routerLink: '/bill-sale/create' },
      ]
    },
    {
      label: 'Mua hàng',
      items: [
        { label: 'Tạo nhà cung cấp', routerLink: '/vendor/create' },
        { label: 'Tạo đề xuất mua hàng', routerLink: '/procurement-request/create' },
      ]
    },
    {
      label: 'Nhân sự',
      items: [
        { label: 'Nhân viên', routerLink: '/employee/create' },
      ]
    },
  ];

  listEmployee: Array<any> = [];
  selectedEmployee: Array<any> = [];
  totalEvents: Array<Calendar> = [];
  rooms: any[];
  nickname: string = localStorage.getItem('EmployeeName');
  countMessageUnread: number;
  listRoomHaveMessageUnread: any[] = [];
  userId = this.auth.UserId;
  
  constructor(
    private router: Router,
    private eventEmitterService: EventEmitterService,
    private getPermission: GetPermission,
    private dashboardHomeService: DashboardHomeService,
    private authenticationService: AuthenticationService,
    public dialog: MatDialog,
    private messageService: MessageService,
    private renderer: Renderer2,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private datePipe: DatePipe,
    public reSearchService: ReSearchService
  ) {
    $("body").addClass('sidebar-collapse');
    this.renderer.listen('window', 'click', (e: Event) => {
      if (this.toggleNotifi) {
        //ẩn hiện khi click Thông báo
        if (this.toggleNotifi.nativeElement.contains(e.target)) {
          this.isOpenNotifi = !this.isOpenNotifi;
        } else {
          this.isOpenNotifi = false;
        }

        //ẩn hiện khi click Tạo mới
        // if (this.toggleCreateElement.nativeElement.contains(e.target)) {
        //   this.isOpenCreateElement = !this.isOpenCreateElement;
        // } else {
        //   this.isOpenCreateElement = false;
        // }

        //ẩn hiện khi click Config
        if (this.toggleConfig.nativeElement.contains(e.target)) {
          this.isOpenConfig = !this.isOpenConfig;
        } else {
          this.isOpenConfig = false;
        }

        //ẩn hiện khi click User
        if (this.toggleUser.nativeElement.contains(e.target)) {
          this.isOpenUser = !this.isOpenUser;
        } else {
          this.isOpenUser = false;
        }
      }

      if (this.dropdownMenus) {
        // ẩn hiện khi click menu items Tạo mới
        if (this.dropdownMenus.nativeElement.contains(e.target)) {
          this.isOpenCreateElement = !this.isOpenCreateElement;
        } else {
          this.isOpenCreateElement = false;
        }
      }
    });
  }

  ngOnInit() {
    this.getPemistion();
    this.getListModuleAndResource();
    this.getNotification();
    this.getLastNotification();
    this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
    this.username = localStorage.getItem("Username");
    // this.userAvatar = localStorage.getItem("UserAvatar");
    this.userAvatar = '';
    this.userFullName = localStorage.getItem("UserFullName");
    this.userEmail = localStorage.getItem("UserEmail");

    //var leftMenu = localStorage.getItem('lstBreadCrumLeftMenu');

    var leftMenu = JSON.stringify(this.lstSubmenuLevel1);

    this.lstBreadCrumLeftMenu = [];
    this.lstBreadCrumLeftMenu = JSON.parse(leftMenu);

    var X = localStorage.getItem('menuMapPath');
    this.lstBreadCrum = JSON.parse(X);

    //kiem tra xem co toggle ko
    if ($("body").hasClass("sidebar-collapse")) {
      this.isToggleCick = true;
    }
    else {
      this.isToggleCick = false;
    }

    if (localStorage.getItem('IsAdmin') == 'true') {
      localStorage.setItem('menuIndex', 'admin');
    }

    if (this.eventEmitterService.subsVar == undefined) {
      this.eventEmitterService.subsVar = this.eventEmitterService.
        invokeFirstComponentFunction.subscribe((name: string) => {
          this.updateLeftMenu();
        });
    }

    //Call Update IsToggle với eventEmitterService
    if (this.eventEmitterService.subsVar2 == undefined) {
      this.eventEmitterService.subsVar2 = this.eventEmitterService.
        invokeUpdateIsToggleFunction.subscribe((name: string) => {
          this.updateLeftIsToggle();
        });
    }

    //setting full calendar
    this.options = {
      plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
      defaultDate: new Date(),
      header: {
        left: 'prev,next',
        center: 'title',
        right: 'dayGridMonth,timeGridWeek,timeGridDay'
      },
      editable: true,
      buttonText: {
        dayGridMonth: 'Tháng',
        timeGridWeek: 'Tuần',
        timeGridDay: 'Ngày',
      },
      eventClick: this.handleEventClick.bind(this),
      eventDrop: (event) => {
        // if (event.event._def.extendedProps.isCreateByUser) {
        // } else {
        //   let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền sửa lịch hẹn này' };
        //   this.showMessage(msg);
        // }
        this.changeEventFullCalendar(event);
      },
      eventResize: (event) => {
        // if (event.event._def.extendedProps.isCreateByUser) {
        // } else {
        //   let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn không có quyền sửa lịch hẹn này' };
        //   this.showMessage(msg);
        // }
        this.changeEventFullCalendar(event);
      },
      selectOverlap: true,
    }
    this.getMessageUnread();
  }

  

  changeEventFullCalendar(els: any) {
    if (els) {
      let startDate = null;
      if (els.event.start) {
        startDate = convertToUTCTime(els.event.start)
      }
      let endDate = null;
      if (els.event.end) {
        endDate = convertToUTCTime(els.event.end);
      }
      let message: string = '';
      if (els.event.end) {
        message = "Bạn có muốn chỉnh sửa thời gian lịch hẹn bắt đầu từ : " + this.datePipe.transform(els.event.start, "h:mm dd/MM/yyyy ") + " đến " + this.datePipe.transform(els.event.end, "h:mm dd/MM/yyyy");
      } else {
        message = "Bạn có muốn chỉnh sửa thời gian lịch hẹn bắt đầu từ : " + this.datePipe.transform(els.event.start, "h:mm dd/MM/yyyy ");
      }
      this.confirmationService.confirm({
        message: message,
        accept: () => {
          this.loading = true;
          this.dashboardHomeService.updateCustomerMeeting(els.event.id, startDate, endDate, this.auth.UserId).subscribe(response => {
            let result: any = response;
            this.loading = false;
            if (result.statusCode == 200) {
              let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật lịch hẹn thành công' };
              this.showMessage(msg);
            } else {
              let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
          });
        },
        reject: () => {
        }
      });

    }

  }

  handleEventClick(eventClick) {
    if (eventClick) {
      let id = eventClick.event.id;
      let ref = this.dialogService.open(MeetingDialogComponent, {
        data: {
          // customerId: this.customerId,
          customerMeetingId: id,
          listParticipants: this.listParticipants
        },
        header: 'Cập nhật lịch hẹn',
        width: '800px',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "300px",
          "max-height": "900px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: any) => {
        if (result) {
          if (result.status) {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật lịch hẹn thành công' };
            this.showMessage(msg);
          }
        }
      });
    }
  }

  setCalendar() {
    this.events = [];
    if (this.listCustomerMeeting) {
      this.listCustomerMeeting.forEach(item => {
        let meeting = new Calendar();
        meeting.id = item.customerMeetingId;
        meeting.title = item.customerName;
        meeting.start = item.startDate;
        meeting.end = item.endDate;
        meeting.participants = item.participants;
        meeting.isCreateByUser = item.isCreateByUser;

        if (meeting.start < new Date()) {
          meeting.backgroundColor = "#DD0000";
        }
        this.events = [...this.events, meeting];
      });
    }
  

    this.totalEvents = this.events;
  }


  // getPemistionMenu2() {
  //   this.lstSubmenuLevel3Create.forEach(element => {
  //     let resource = element.ObjectType + element.Path;
  //     let permission: any = this.getPermission.getPermission(this.listPermissionResourceActive, resource);
  //     if (permission.status == false) {
  //       element.Active = true;
  //     }
  //     else {
  //       element.Active = false;
  //     }
  //   });
  // }
  
  getPemistion() {
    this.lstSubmenuLevel1.forEach(item => {
      item.LstChildren.forEach(element => {
        element.LstChildren.forEach(role => {
          let resource = item.ObjectType + role.Path;
          let permission: any = this.getPermission.getPermissionLocal(this.listPermissionResourceActive, resource);
          if (permission.__zone_symbol__value.status == false) {
            role.Active = true;
          }
          else {
            role.Active = false;
          }
        });
        let countElement = element.LstChildren.filter(f => f.Active == true);
        if (countElement.length == element.LstChildren.length) {
          element.Active = true;
        }
        else element.Active = false;
      });
      let countItem = item.LstChildren.filter(f => f.Active == true);
      if (countItem.length == item.LstChildren.length) {
        item.Active = true;
      }
      else item.Active = false;
    });
  }


  setStepSize(data) {
    //Chia nhiều nhất là 10 mốc dữ liệu
    return this.formatRoundNumber(data[0] / 10);
  }

  //Làm tròn số
  formatRoundNumber(number) {
    number = number.toString();
    let stt = number.length;
    let first_number = number.slice(0, 1);
    let result: number;
    switch (first_number) {
      case '1':
        result = this.addZero(2, stt);
        break;
      case '2':
        result = this.addZero(3, stt);
        break;
      case '3':
        result = this.addZero(4, stt);
        break;
      case '4':
        result = this.addZero(5, stt);
        break;
      case '5':
        result = this.addZero(6, stt);
        break;
      case '6':
        result = this.addZero(7, stt);
        break;
      case '7':
        result = this.addZero(8, stt);
        break;
      case '9':
        result = this.addZero(9, stt);
        break;
      default:
        break;
    }
    return result;
  }

  //Thêm số chữ số 0 vào sau một ký tự
  addZero(tmp: number, stt: number) {
    if (tmp == 9) {
      stt = stt + 1;
      tmp = 1;
    }
    let num = tmp.toString();
    for (let i = 0; i < stt - 1; i++) {
      num += "0";
    }
    return Number(num);
  }

  //Lấy ra list module của user
  getListModuleAndResource() {
    if (this.listPermissionResource.length > 0) {
      this.listPermissionResource.forEach(item => {
        let moduleName = item.slice(0, item.indexOf('/'));
        if (this.listModule.indexOf(moduleName) == -1) {
          this.listModule.push(moduleName);
        }

        let resourceName = item.slice(item.indexOf('/') + 1, item.lastIndexOf('/'));
        if (this.listResource.indexOf(resourceName) == -1) {
          this.listResource.push(resourceName);
        }
      });
    }
  }

  //Kiểm tra user được quyền thấy các module nào trên trang home:
  checkModule(moduleName) {
    let result = false;
    if (this.listModule.indexOf(moduleName) !== -1) {
      result = true;
    }
    return result;
  }

  //Kiểm tra user có được quyền nhìn thấy các resource nào trên menu:
  checkUserResource(resourceName) {
    let result = false;
    if (this.listResource.indexOf(resourceName) !== -1) {
      result = true;
    }
    return result;
  }

  // checkUserResourceModule(resourceName: string[]) {
  //   let result = false;
  //   for (var i = 0; i < resourceName.length; i++) {
  //     if (this.listResource.indexOf(resourceName[i]) !== -1) {
  //       result = true;
  //       return result;
  //     }
  //   }
  //   return result;
  // }

  updateLeftMenu() {
    var leftMenu = localStorage.getItem('lstBreadCrumLeftMenu');
    this.lstBreadCrumLeftMenu = [];
    this.lstBreadCrumLeftMenu = JSON.parse(leftMenu);
  }

  updateLeftIsToggle() {
    this.isToggleCick = JSON.parse(localStorage.getItem('isToggleCick'));
  }

  openMenuLevel2(resource, resourceParent: BreadCrumMenuModel) {
    //Kiểm tra reset bộ lọc
    this.resetSearchModel(resource.Path);
    this.router.navigate([resource.Path]);
  }

  openMenuLevel3(resource, resourceParent: BreadCrumMenuModel) {
    if (resource.LstChildren.length == 1) {
      this.router.navigate([resource.LstChildren[0].Path]);
    } 
    else if (resource.LstChildren.length == 0) {
      //Kiểm tra reset bộ lọc
      this.resetSearchModel(resource.Path);
      this.router.navigate([resource.Path]);
    }
  }

  openMenuLevel4(resource, resourceParent: BreadCrumMenuModel) {
    //Kiểm tra reset bộ lọc
    // this.resetSearchModel(resource.Path);
    this.router.navigate([resource.Path]);
  }

  getNotification(): void {
    let size = 5;
    firebase.database().ref('notification/').child(this.auth.EmployeeId).orderByChild("status").equalTo(false).once('value', resp => {
      const listNotification = this.snapshotToArray(resp).sort((a, b): any => { return b.date.localeCompare(a.date) });
      this.notificationNumber = listNotification.length;
      this.notificationList = listNotification.splice(0, size)
    })
  }

  getLastNotification(): void {
    firebase.database().ref('notification/').child(this.auth.EmployeeId).limitToLast(1).on('value', resp => {
      this.getNotification();
    })
  }

  goToNotiUrl(item: any): void {
    if(item.url) {
      window.location.href = item.url;
      const notificationRef = firebase.database().ref('notification').child(this.auth.EmployeeId + '/' + item.key);
      notificationRef.update({status: true});
    }
  }

  goToNotification() {
    this.router.navigate(['/chat/notificaton-list']);
  }

  logout() {
    this.authenticationService.logout();
    this.router.navigate(['/login']);
  }

  // Mở giao diện đổi Password
  openChangePassword() {
    let account = this.username;
    let _name = this.userFullName;
    let _email = this.userEmail;
    let _avatar = this.userAvatar;
    this.dialogRef = this.dialog.open(ChangepasswordComponent,
      {
        data: { accountName: account, name: _name, email: _email, avatar: _avatar }
      });
    this.dialogRef.afterClosed().subscribe(result => {
    });
  }
  //Ket thuc

  // Mo giao dien UserProfile
  goToViewProfile() {
    this.router.navigate(['/userprofile']);
  }

  goToUrlSysConfig(Path) {
    this.router.navigate([Path]);
  }

  goListQuote() {
    this.router.navigate(['/customer/quote-list']);
  }

  onViewQuoteDetail(id: string) {
    this.router.navigate(['/customer/quote-detail', { quoteId: id }]);
  }

  goListCustomer() {
    this.router.navigate(['/customer/list']);
  }

  onViewCustomerDetail(id: string) {
    this.router.navigate(['/customer/detail', { customerId: id }]);
  }

  onViewObjectDetail(id: string, contactId: string, type: string) {
    if (type == 'CUSTOMER') {
      this.router.navigate(['/customer/detail', { customerId: id }]);
    } else if (type = 'LEAD') {
      this.router.navigate(['/lead/detail', { leadId: id }]);
    }
  }

  goListOrder() {
    this.router.navigate(['/order/list']);
  }

  onViewOrderDetail(id: string) {
    this.router.navigate(['/order/order-detail', { customerOrderID: id }]);
  }

  onViewEmployeeDetail(employeeId: string, contactId: string) {
    this.router.navigate(['/employee/detail', { employeeId: employeeId, contactId: contactId }]);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  isHomepage() {
    if (this.router.url === '/home') {
      return true;
    } else {
      return false;
    }
  }

  rowclick: number = -1;
  rowclickParent: number = -1;
  rowclickGrandparent: number = -1;
  active: boolean = true;
  activeParent: boolean = true;
  activeGrandparent: boolean = true;
  countLengthParrent: number = 0;
  countLengthGrandparent: number = 0;

  addRemoveIcon(index) {
    for (let i = 0; i < this.lstBreadCrumLeftMenu.length; i++) {
      $(".module-remove" + i).hide();
      $(".module-add" + i).show();
    }
    if (this.rowclick !== index) {
      $(".module-remove" + index).show();
      $(".module-add" + index).hide();
      this.active = true;

      for (let i = 0; i < this.countLengthParrent; i++) {
        $(".module-remove-parent" + i).hide();
        $(".module-add-parent" + i).show();
      }
      this.activeParent = true;
    }
    else {
      if (!this.active) {
        $(".module-remove" + index).show();
        $(".module-add" + index).hide();
      }
      else {
        $(".module-remove" + index).hide();
        $(".module-add" + index).show();
      }
      this.active = !this.active;
    }

    this.rowclick = index;
  }

  addRemoveIconParren(index, countLength) {
    this.countLengthParrent = countLength;
    for (let i = 0; i < countLength; i++) {
      $(".module-remove-parent" + i).hide();
      $(".module-add-parent" + i).show();
    }
    if (this.rowclickParent !== index) {
      $(".module-remove-parent" + index).show();
      $(".module-add-parent" + index).hide();
      this.activeParent = true;

      for (let i = 0; i < this.countLengthGrandparent; i++) {
        $(".module-remove-grandparent" + i).hide();
        $(".module-add-grandparent" + i).show();
      }
      this.activeGrandparent = true;
    }
    else {
      if (!this.activeParent) {
        $(".module-remove-parent" + index).show();
        $(".module-add-parent" + index).hide();
      }
      else {
        $(".module-remove-parent" + index).hide();
        $(".module-add-parent" + index).show();
      }
      this.activeParent = !this.activeParent;
    }

    this.rowclickParent = index;
  }

  addRemoveIconGrandparent(index, countLength) {
    this.countLengthGrandparent = countLength;
    for (let i = 0; i < countLength; i++) {
      $(".module-remove-grandparent" + i).hide();
      $(".module-add-grandparent" + i).show();
    }
    if (this.rowclickGrandparent !== index) {
      $(".module-remove-grandparent" + index).show();
      $(".module-add-grandparent" + index).hide();
      this.activeGrandparent = true;
    }
    else {
      if (!this.activeGrandparent) {
        $(".module-remove-grandparent" + index).show();
        $(".module-add-grandparent" + index).hide();
      }
      else {
        $(".module-remove-grandparent" + index).hide();
        $(".module-add-grandparent" + index).show();
      }
      this.activeGrandparent = !this.activeGrandparent;
    }

    this.rowclickGrandparent = index;
  }

  getDefaultApplicationName() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "ApplicationName")?.systemValueString;
  }

  searchMeetingForEmployee() {
    this.events = this.totalEvents;
    let listEmployeeId = this.selectedEmployee.map(x => x.employeeId);

    if (listEmployeeId.length > 0) {
      let currentEvent: Array<any> = [];
      this.events.forEach(item => {
        let listParticipants = item.participants.split(';');

        let flag = false;
        listEmployeeId.forEach(_item => {
          if (listParticipants.includes(_item)) {
            flag = true;
          }
        });

        if (flag) {
          currentEvent.push(item);
        }
      });

      this.events = [];
      this.events = [...currentEvent];
    }
    else {
      this.events = this.totalEvents;
    }
  }

  //Kiểm tra reset bộ lọc
  resetSearchModel(path) {
    this.reSearchService.resetSearchModel(path);
  }

  gotoProductionDetailDelay(rowData: delayProductionOrder) {
    this.router.navigate(['/manufacture/production-order/detail', { productionOrderId: rowData.productionOrderId }]);
  }

  gotoProductionDetail(rowData: totalProductionOrderInDashBoard) {
    this.router.navigate(['/manufacture/production-order/detail', { productionOrderId: rowData.productionOrderId }]);
  }

  goToListRoom(): void {
    this.router.navigate(['/chat/room-list']);
  }

  getMessageUnread(): void {
    let fb = firebase.database();
    fb.ref('rooms/').on('value', resp => {
      this.rooms = [];
      this.rooms = this.snapshotToArrayRoomName(resp);
      this.countMessageUnread = 0;
      this.rooms.forEach(r => {
        fb.ref('chats/').child(r.roomname).limitToLast(1).on('value', resp => {
          let listMessageUnread = this.snapshotToArrayMessage(resp, r.roomname);
          if (listMessageUnread.length > 0 && !this.listRoomHaveMessageUnread.includes(r.roomname)) {
            this.listRoomHaveMessageUnread.push(r.roomname)
          }
          this.countMessageUnread = this.listRoomHaveMessageUnread.length;
        });
      })
    });
  }

  snapshotToArray(snapshot: any): any[] {
    const returnArr = [];
    snapshot.forEach((childSnapshot: any) => {
      const item = childSnapshot.val();
      item.key = childSnapshot.key;
      returnArr.push(item);
    });
    return returnArr;
  };

  snapshotToArrayRoomName(snapshot: any): any[] {
    const returnArr = [];
    snapshot.forEach((childSnapshot: any) => {
        const item = childSnapshot.val();
        item.key = childSnapshot.key;
        if ((item.receiver == this.userId || item.userCreate == this.userId)) {
          returnArr.push(item);
        }
    });
    return returnArr;
  };

  snapshotToArrayMessage(snapshot: any, roomName: string): any[] {
    const returnArr = [];
    snapshot.forEach((childSnapshot: any) => {
      const item = childSnapshot.val();
      item.key = childSnapshot.key;
      if (item.roomname == roomName && item.senderId != this.userId && item.isSeen == false) {
        returnArr.push(item);
      }
    });
    return returnArr;
  };

  goToHome() {
    this.router.navigate(['/home']);
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
