import { Component, OnInit, ElementRef, ViewChild, Renderer2 } from '@angular/core';
import { BreadCrumMenuModel } from '../../../shared/models/breadCrumMenu.model';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { ChangepasswordComponent } from '../../../shared/components/changepassword/changepassword.component';
import { UserprofileComponent } from '../../../userprofile/userprofile.component';
import { MenuItem } from 'primeng/api';
import { AuthenticationService } from '../../../shared/services/authentication.service';

@Component({
  selector: 'app-top-header-project',
  templateUrl: './top-header-project.component.html',
  styleUrls: ['./top-header-project.component.css']
})
export class TopHeaderProjectComponent implements OnInit {
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  applicationName = this.getDefaultApplicationName();
  // userAvatar: string = localStorage.getItem("UserAvatar");
  userAvatar: string = '';
  username = localStorage.getItem("Username");
  userFullName = localStorage.getItem("UserFullName");
  userEmail = localStorage.getItem("UserEmail");

  @ViewChild('toggleUser') toggleUser: ElementRef;
  isOpenUser: boolean = false;

  notificationNumber: number = 0;
  @ViewChild('toggleNotifi') toggleNotifi: ElementRef;
  isOpenNotifi: boolean = false;

  @ViewChild('toggleCreateElement') toggleCreateElement: ElementRef;
  isOpenCreateElement: boolean = false;

  @ViewChild('toggleConfig') toggleConfig: ElementRef;
  isOpenConfig: boolean = false;

  @ViewChild('dropdownMenus') dropdownMenus: ElementRef;

  dialogRef: MatDialogRef<ChangepasswordComponent>;
  dialogPopup: MatDialogRef<UserprofileComponent>;

  lstSubmenuLevel3: Array<BreadCrumMenuModel> = [
    //Quan ly he thong
    { Name: "Cấu hình thông tin chung", Path: "/admin/company-config", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "settings", IsDefault: true, CodeParent: "sys_chttc", LstChildren: [], Display: "none", Code: '' },
    { Name: "Cấu hình thư mục", Path: "/admin/folder-config", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "settings", IsDefault: true, CodeParent: "sys_chtm", LstChildren: [], Display: "none", Code: '' },
    { Name: "Quản lý thông báo", Path: "/admin/notifi-setting-list", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "settings", IsDefault: true, CodeParent: "sys_tb", LstChildren: [], Display: "none", Code: '' },
    { Name: "Tham số hệ thống", Path: "/admin/system-parameter", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "settings_applications", IsDefault: true, CodeParent: "sys_tsht", LstChildren: [], Display: "none", Code: '' },
    { Name: "Quản lý mẫu Email", Path: "/admin/email-configuration", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "device_hub", IsDefault: true, CodeParent: "Systems_QLE", LstChildren: [], Display: "none", Code: '' },
    { Name: "Quản lý sơ đồ tổ chức", Path: "/admin/organization", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "device_hub", IsDefault: true, CodeParent: "sys_sdtc", LstChildren: [], Display: "none", Code: '' },
    { Name: "Quản lý dữ liệu danh mục", Path: "/admin/masterdata", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "category", IsDefault: true, CodeParent: "sys_dldm", LstChildren: [], Display: "none", Code: '' },
    { Name: "Quản lý nhóm quyền", Path: "/admin/permission", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "format_list_bulleted", IsDefault: true, CodeParent: "sys_nq", LstChildren: [], Display: "none", Code: '' },
    // { Name: "Phân hạng khách hàng", Path: "/admin/config-level-customer", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "filter_list", IsDefault: true, CodeParent: "sys_phkh", LstChildren: [], Display: "none", Code: '' },
    { Name: "Quản lý quy trình làm việc", Path: "/admin/danh-sach-quy-trinh", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "swap_horiz", IsDefault: true, CodeParent: "sys_qtlv", LstChildren: [], Display: "none", Code: '' },
    // { Name: "Nhật ký hệ thống", Path: "/admin/audit-trace", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "menu_book", IsDefault: true, CodeParent: "sys_log", LstChildren: [], Display: "none", Code: '' },
  ];

  items: MenuItem[] = [
    {
      label: 'CRM',
      items: [
        { label: 'Tạo khách hàng tiềm năng', routerLink: '/customer/potential-customer-create' },
        { label: 'Tạo cơ hội', routerLink: '/lead/create-lead' },
        { label: 'Tạo báo giá', routerLink: '/customer/quote-create' },
        { label: 'Tạo Khách hàng', routerLink: '/customer/create' },
      ]
    },
    {
      label: 'Bán hàng',
      items: [
        { label: 'Tạo sản phẩm', routerLink: '/product/createOrEdit' },
        { label: 'Tạo hợp đồng bán', routerLink: '/sales/contract-create' },
        { label: 'Tạo đơn hàng', routerLink: '/order/create' },
        { label: 'Tạo hóa đơn', routerLink: '/bill-sale/create' },
      ]
    },
    {
      label: 'Mua hàng',
      items: [
        { label: 'Tạo nhà cung cấp', routerLink: '/vendor/create' },
        { label: 'Tạo đề xuất mua hàng', routerLink: '/procurement-request/create' },
      ]
    },
    // {
    //   label: 'Tài chính',
    //   items: [
    //     { label: 'Tạo phiếu thu', routerLink: '/accounting/cash-receipts-create' },
    //     { label: 'Tạo phiếu chi', routerLink: '/accounting/cash-payments-create' },
    //     { label: 'Tạo báo có', routerLink: '/accounting/bank-receipts-create' },
    //     { label: 'Tạo UNC', routerLink: '/accounting/bank-payments-create' },
    //   ]
    // },
    {
      label: 'Nhân sự',
      items: [
        { label: 'Nhân viên', routerLink: '/employee/create' },
      ]
    },
    // {
    //   label: 'Kho',
    //   items: [
    //     { label: 'Tạo phiếu nhập kho', routerLink: '/warehouse/inventory-receiving-voucher/create' },
    //     { label: 'Tạo phiếu xuất kho', routerLink: '/warehouse/inventory-delivery-voucher/create-update' }
    //   ]
    // }
  ];

  constructor(
    private renderer: Renderer2,
    private router: Router,
    public dialog: MatDialog,
    private authenticationService: AuthenticationService,
  ) {
    this.renderer.listen('window', 'click', (e: Event) => {
      if (this.toggleNotifi) {
        //ẩn hiện khi click Thông báo
        if (this.toggleNotifi.nativeElement.contains(e.target)) {
          this.isOpenNotifi = !this.isOpenNotifi;
        } else {
          this.isOpenNotifi = false;
        }
      }

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

      //ẩn hiện khi click Tạo mới
      if (this.toggleCreateElement.nativeElement.contains(e.target)) {
        this.isOpenCreateElement = !this.isOpenCreateElement;
      } else {
        this.isOpenCreateElement = false;
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

  ngOnInit(): void {

  }

  goToHome() {
    this.router.navigate(['/home']);
  }

  goToViewProfile() {
    this.router.navigate(['/userprofile']);
  }

  /* Mở giao diện đổi Password */
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

  logout() {
    this.authenticationService.logout();
    this.router.navigate(['/login']);
  }

  goToUrlSysConfig(Path) {
    this.router.navigate([Path]);
  }

  getDefaultApplicationName() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "ApplicationName") ?.systemValueString;
  }

}
