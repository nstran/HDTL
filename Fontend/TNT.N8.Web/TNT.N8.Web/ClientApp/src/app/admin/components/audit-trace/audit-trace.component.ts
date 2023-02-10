import { Component, OnInit } from '@angular/core';
import { AuditTraceService } from '../../services/audit-trace.service';
import { EmployeeService } from '../../../employee/services/employee.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';

interface loginTrace {
  userId: string;
  userName: string;
  loginDate: Date;
  status: string;
  statusCode: number;
  backgroundColor: string;
  color: string;
  employeeCode: string;
  employeeName: string;
  employeeCodeName: string;
}

interface auditTrace {
  userId: string;
  userName: string;
  employeeName: string;
  actionName: string;
  objectId: string;
  objectName: string;
  objectCode: string;
  createDate: Date;
  description: string;
  backgroundColor: string;
  color: string;
  object: string;
}

class user {
  userId: string;
  userName: string;
}

class employee {
  employeeId: string;
  employeeName: string;
  employeeCodeName: string;
}

enum ObjectType {
  POTENTIAL_CUSTOMER = 'POTENTIAL_CUSTOMER',
  CUSTOMER = 'CUSTOMER',
  LEAD = 'LEAD',
  SALE_BIDDING = 'SALE_BIDDING',
  QUOTE = 'QUOTE',
  CUSTOMER_CARE = 'CUSTOMER_CARE',
  PROMOTION = 'PROMOTION',
  CONTRACT = 'CONTRACT',
  CUSTOMER_ORDER = 'CUSTOMER_ORDER',
  BILL_SALE = 'BILL_SALE',
  PROCUREMENT_REQUEST = 'PROCUREMENT_REQUEST',
  VENDOR_ORDER = 'VENDOR_ORDER',
  PRODUCT = 'PRODUCT',
}

const POTENTIAL_CUSTOMER = 'Tiềm năng';
const CUSTOMER = 'Khách hàng';
const LEAD = 'Cơ hội';
const SALE_BIDDING = 'Hồ sơ thầu';
const QUOTE = 'Báo giá';
const CUSTOMER_CARE = 'Chương trình chăm sóc';
const PROMOTION = 'Chương trình khuyến mại';
const CONTRACT = 'Hợp đồng bán';
const CUSTOMER_ORDER = 'Đơn hàng';
const BILL_SALE = 'Hóa đơn';
const PROCUREMENT_REQUEST = 'Đề xuất mua hàng';
const VENDOR_ORDER = 'Đơn hàng mua';
const PRODUCT = 'Sản phẩm dịch vụ';

const ACTION_NAME = [
  {
    type: 'CREATE',
    name: 'Tạo mới'
  },
  {
    type: 'UPDATE',
    name: 'Sửa'
  },
  {
    type: 'DELETE',
    name: 'Xóa'
  },
]

const OBJECT_NAME = [
  {
    type: 'POTENTIAL_CUSTOMER',
    name: 'Tiềm năng',
  },
  {
    type: 'CUSTOMER',
    name: 'Khách hàng',
  },
  {
    type: 'LEAD',
    name: 'Cơ hội',
  },
  {
    type: 'SALE_BIDDING',
    name: 'Hồ sơ thầu',
  },
  {
    type: 'QUOTE',
    name: 'Báo giá',
  },
  {
    type: 'CUSTOMER_CARE',
    name: 'Chương trình chăm sóc',
  },
  {
    type: 'PROMOTION',
    name: 'Chương trình khuyến mại',
  },
  {
    type: 'CONTRACT',
    name: 'Hợp đồng bán',
  },
  {
    type: 'CUSTOMER_ORDER',
    name: 'Đơn hàng',
  },
  {
    type: 'BILL_SALE',
    name: 'Hóa đơn',
  },
  {
    type: 'PROCUREMENT_REQUEST',
    name: 'Đề xuất mua hàng',
  },
  {
    type: 'VENDOR_ORDER',
    name: 'Đơn hàng mua',
  },
  {
    type: 'PRODUCT',
    name: 'Sản phẩm dịch vụ',
  },
]

const STATUS = [
  {
    id: 1,
    name: 'Thành công',
  },
  {
    id: 0,
    name: 'Thất bại',
  },
]

@Component({
  selector: 'app-audit-trace',
  templateUrl: './audit-trace.component.html',
  styleUrls: ['./audit-trace.component.css']
})
export class AuditTraceComponent implements OnInit {

  // commont
  auth: any = JSON.parse(localStorage.getItem("auth"));
  userId: string = this.auth.UserId;
  loading: boolean = false;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  listLoginTrace: Array<loginTrace> = [];
  listAuditTrace: Array<auditTrace> = [];
  listEmployee: Array<employee> = [];
  listUser: Array<user> = [];
  lstAction: Array<any> = [];
  lstObject: Array<any> = [];
  lstStatus: Array<any> = [];

  colHeaderLoginTrace: any[];
  colHeaderTrace: any[];

  isSelectedLoginAudit: boolean = true;
  isSelectedAuditTrace: boolean = false;

  /** SEARCH FORM */
  searchForm: FormGroup;
  userNameControl: FormControl;
  userNameLoginControl: FormControl;
  employeeNameControl: FormControl;
  employeeCodeNameControl: FormControl;
  actionNameControl: FormControl;
  objectNameControl: FormControl;
  statusControl: FormControl;
  loginDateControl: FormControl;
  dateControl: FormControl;
  /** END */

  currentYear: number = (new Date()).getFullYear();
  today: Date = new Date();

  /** PAGING */
  pageNumber: number; //Số page
  pageSize: number = 10; // Số lượng bản ghi một page
  pageIndex: number = 1; // page hiện tại, mặng định là 1
  totalRecordLoginTrace: number; // Tổng các bản ghi danh sách nhật ký đăng nhập
  totalRecordAuditTrace: number; // Tổng các bản ghi danh sách nhật ký hệ thống
  /** PAGING */

  constructor(
    private auditTraceService: AuditTraceService,
    private getPermission: GetPermission,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
  ) { }

  async  ngOnInit() {
    /* #region  init table header */
    this.colHeaderLoginTrace = [
      { field: 'userName', header: 'Tên tài khoản', textAlign: 'left', display: 'table-cell', colWith: '50px' },
      { field: 'employeeCodeName', header: 'Tên nhân viên', textAlign: 'left', display: 'table-cell', colWith: '100px' },
      { field: 'loginDate', header: 'Ngày đăng nhập', textAlign: 'right', display: 'table-cell', colWith: '100px' },
      { field: 'status', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', colWith: '100px' },
    ];

    this.colHeaderTrace = [
      { field: 'userName', header: 'Tên tài khoản', textAlign: 'left', display: 'table-cell', colWith: '100px' },
      { field: 'employeeName', header: 'Tên Nhân Viên', textAlign: 'left', display: 'table-cell', colWith: '100px' },
      { field: 'actionName', header: 'Hành động', textAlign: 'left', display: 'table-cell', colWith: '100px' },
      { field: 'object', header: 'Đối tượng', textAlign: 'left', display: 'table-cell', colWith: '100px' },
      { field: 'createDate', header: 'Ngày', textAlign: 'right', display: 'table-cell', colWith: '100px' },
      { field: 'description', header: 'Mô tả', textAlign: 'left', display: 'table-cell', colWith: '100px' },
    ];
    /* #endregion */

    this.lstAction = ACTION_NAME;
    this.lstObject = OBJECT_NAME;
    this.lstStatus = STATUS;

    let resource = "sys/admin/audit-trace/";
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
      this.getMasterData();
    }
  }

  getMasterData() {
    this.loading = true;
    this.auditTraceService.GetMasterDataTrace().subscribe(response => {
      let result = <any>response;
      this.loading = false;
      this.listEmployee = result.listEmp;
      this.listUser = result.listUser;
      this.search(1);
    });
  }

  initForm() {
    this.userNameControl = new FormControl('');
    this.employeeNameControl = new FormControl('');
    this.employeeCodeNameControl = new FormControl('');
    this.actionNameControl = new FormControl('');
    this.objectNameControl = new FormControl('');
    this.statusControl = new FormControl('');
    this.dateControl = new FormControl(null);

    this.searchForm = new FormGroup({
      userNameControl: this.userNameControl,
      employeeNameControl: this.employeeNameControl,
      employeeCodeNameControl: this.employeeCodeNameControl,
      actionNameControl: this.actionNameControl,
      objectNameControl: this.objectNameControl,
      statusControl: this.statusControl,
      dateControl: this.dateControl,
    });
  }

  handleChange() {
    this.isSelectedLoginAudit = !this.isSelectedLoginAudit;
    this.isSelectedAuditTrace = !this.isSelectedAuditTrace;
  }

  masterToggleDdefault(type: string) {
    if (type === 'login') {
      this.isSelectedLoginAudit = true;
      this.isSelectedAuditTrace = false;
      this.userNameControl.reset();
      this.employeeNameControl.reset();
      this.actionNameControl.reset();
      this.objectNameControl.reset();
      this.dateControl.reset();
    }
    if (type === 'trace') {
      this.isSelectedLoginAudit = false;
      this.isSelectedAuditTrace = true;
      this.userNameControl.reset();
      this.employeeNameControl.reset();
      this.dateControl.reset();
      this.statusControl.reset();
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  refresh() {
    this.getMasterData();
    this.searchForm.reset();
  }

  search(pageIndex) {
    if (pageIndex == 1) {
      this.loading = true;
    } else {
      this.loading = false;
    }


    let searchDate = this.dateControl.value;
    if (searchDate) {
      searchDate.setHours(0, 0, 0, 0);
      searchDate = convertToUTCTime(searchDate);
    }

    let lstUserId = this.userNameControl.value ? this.userNameControl.value.map(x => x.userId) : [];
    let lstEmpId = this.employeeNameControl.value ? this.employeeNameControl.value.map(x => x.employeeId) : [];
    let ListStatus = this.statusControl.value ? this.statusControl.value.map(x => x.id) : [];
    let ListAction = this.actionNameControl.value ? this.actionNameControl.value.map(x => x.type) : [];
    let ListObjectType = this.objectNameControl.value ? this.objectNameControl.value.map(x => x.type) : [];

    this.auditTraceService.searchTrace(lstUserId, lstEmpId,
      ListStatus, ListAction,
      ListObjectType, searchDate,
      this.isSelectedLoginAudit, this.isSelectedAuditTrace,
      this.pageSize, pageIndex).subscribe(res => {
        let result: any = res;
        this.loading = false;

        if (result.statusCode == 200) {
          this.listLoginTrace = result.listLoginTrace;
          this.listAuditTrace = result.listAuditTrace;
          this.totalRecordAuditTrace = result.totalRecordsAuditTrace;
          this.totalRecordLoginTrace = result.totalRecordsLoginTrace;

          if (this.listLoginTrace.length == 0) {
            let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy.' };
            this.showMessage(mgs);
          } else {
            this.listLoginTrace.forEach(item => {
              if (item.statusCode === 1) {
                item.status = 'Đăng nhập thành công';
                item.backgroundColor = '#0f0';
                item.color = '#000';
              }
              else if (item.statusCode === 0) {
                item.status = 'Đăng nhập thất bại';
                item.backgroundColor = '#f00';
                item.color = '#fff';
              }
              item.employeeCodeName = item.employeeCode + ' - ' + item.employeeName;
            })
          }

          if (this.listAuditTrace.length == 0) {
            let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy.' };
            this.showMessage(mgs);
          } else {
            this.listAuditTrace.forEach(item => {
              switch (item.objectName) {
                case ObjectType.POTENTIAL_CUSTOMER: {
                  item.object = POTENTIAL_CUSTOMER;
                  break;
                }
                case ObjectType.CUSTOMER: {
                  item.object = CUSTOMER;
                  break;
                }
                case ObjectType.LEAD: {
                  item.object = LEAD;
                  break;
                }
                case ObjectType.QUOTE: {
                  item.object = QUOTE;
                  break;
                }
                case ObjectType.SALE_BIDDING: {
                  item.object = SALE_BIDDING;
                  break;
                }
                case ObjectType.CUSTOMER_CARE: {
                  item.object = CUSTOMER_CARE;
                  break;
                }
                case ObjectType.PROMOTION: {
                  item.object = PROMOTION;
                  break;
                }
                case ObjectType.CONTRACT: {
                  item.object = CONTRACT;
                  break;
                }
                case ObjectType.CUSTOMER_ORDER: {
                  item.object = CUSTOMER_ORDER;
                  break;
                }
                case ObjectType.BILL_SALE: {
                  item.object = BILL_SALE;
                  break;
                }
                case ObjectType.PROCUREMENT_REQUEST: {
                  item.object = PROCUREMENT_REQUEST;
                  break;
                }
                case ObjectType.VENDOR_ORDER: {
                  item.object = VENDOR_ORDER;
                  break;
                }
                case ObjectType.PRODUCT: {
                  item.object = PRODUCT;
                  break;
                }
                default: {
                  item.object = '';
                  break;
                }
              }
            })
          }
        } else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
        }
      });
  }

  paginate(event: any){
    this.search(event.page + 1);
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
