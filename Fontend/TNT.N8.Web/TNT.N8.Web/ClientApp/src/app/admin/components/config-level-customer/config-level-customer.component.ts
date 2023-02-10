import { Component, OnInit } from '@angular/core';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { SuccessComponent } from '../../../shared/toast/success/success.component';
import { FailComponent } from '../../../shared/toast/fail/fail.component';
import { Observable } from 'rxjs';
import * as $ from 'jquery';
import { CustomerlevelService } from '../../../shared/services/customerlevel.service';
import { CustomerServiceLevelModel } from '../../../shared/models/customerservicelevel.model';
import { WarningComponent } from '../../../shared/toast/warning/warning.component';
import { PopupComponent } from '../../../shared/components/popup/popup.component';
import { EmployeeService } from '../../../employee/services/employee.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-config-level-customer',
  templateUrl: './config-level-customer.component.html',
  styleUrls: ['./config-level-customer.component.css'],
  providers: [MessageService]
})
export class ConfigLevelCustomerComponent implements OnInit {
  loading: boolean = false;
  customerLevelId: string;

  constructor(private translate: TranslateService,
    private getPermission: GetPermission,
    private router: Router,
    private employeeService: EmployeeService,
    private customerLevelService: CustomerlevelService,
    private snackBar: MatSnackBar,
    public dialog: MatDialog,
    private messageService: MessageService
  ) {
    translate.setDefaultLang('vi');
  }
  public isShow = false;
  public newAttribute: any = {
    MinimumSaleValue: 0,
    CustomerServiceLevelName: '',
    CustomerServiceLevelId: null,
    CustomerServiceLevelCode: null,
    CreatedById: null,
    CreatedDate: null,
    UpdatedById: null,
    UpdatedDate: null,
    Active: true
  };

  actionAdd: boolean = true;
  actionDelete: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  customerServiceLevelModel = new Array<CustomerServiceLevelModel>();
  newCustomerLevelModel = new Array<CustomerServiceLevelModel>();
  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };
  dialogPopup: MatDialogRef<PopupComponent>;

  async ngOnInit() {
    let resource = "sys/admin/config-level-customer/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.snackBar.openFromComponent(WarningComponent, { data: "Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ", ...this.warningConfig });
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      this.getConfigLevelCustomer();
    }
  }

  addLevel() {
    this.customerServiceLevelModel.push(this.newAttribute);
    this.newAttribute = {
      MinimumSaleValue: 0,
      CustomerServiceLevelName: '',
      CustomerServiceLevelId: null,
      CustomerServiceLevelCode: null,
      CreatedById: null,
      CreatedDate: null,
      UpdatedById: null,
      UpdatedDate: null,
      Active: true
    };
    this.isShow = true;
  }

  removeLevel(index, id) {
    this.dialogPopup = this.dialog.open(PopupComponent,
      {
        width: '500px',
        height: '250px',
        autoFocus: false,
        data: { title: 'XÓA', content: 'Bạn có chắc chắn muốn xóa?' }
      });
    this.dialogPopup.afterClosed().subscribe(result => {
      if (result) {
        if (id == null) {
          this.customerServiceLevelModel.splice(index, 1);
          this.newCustomerLevelModel = this.customerServiceLevelModel.filter(el => el.CustomerServiceLevelId === null);
          if (this.newCustomerLevelModel.length === 0) {
            this.isShow = false;
          }
        } else {
          this.loading = true;
          this.customerLevelService.updateLevelCustomer(id).subscribe(response => {
            const result = <any>response;
            this.loading = false;

            if (result.statusCode !== 200) {
              this.isShow = false;
              this.showMessage('error', result.messageCode);
              return;
            }

            this.showMessage('success', result.messageCode);
            this.getConfigLevelCustomer();
            this.isShow = false;
          });
        }
      }
    });
  }

  getConfigLevelCustomer() {
    this.loading = true;
    this.customerLevelService.getLevelCustomer().subscribe(response => {
      this.loading = false;
      const result = <any>response;

      if (result.statusCode != 200) {
        this.showMessage('error', result.messageCode);
        return;
      }

      if (result.customerServiceLevel.length === 0) {
        this.showMessage('warn', 'Không tìm thấy phân loại khách hàng nào!');
        return;
      }

      this.customerServiceLevelModel = [];
      for (let i = 0; i < result.customerServiceLevel.length; i++) {
        const item: CustomerServiceLevelModel = {
          MinimumSaleValue: result.customerServiceLevel[i].minimumSaleValue,
          CustomerServiceLevelName: result.customerServiceLevel[i].customerServiceLevelName,
          CustomerServiceLevelId: result.customerServiceLevel[i].customerServiceLevelId,
          CustomerServiceLevelCode: result.customerServiceLevel[i].customerServiceLevelCode,
          CreatedById: result.customerServiceLevel[i].createdById,
          CreatedDate: result.customerServiceLevel[i].createdDate,
          UpdatedById: result.customerServiceLevel[i].updatedById,
          UpdatedDate: result.customerServiceLevel[i].updatedDate,
          Active: result.customerServiceLevel[i].active,
        };
        this.customerServiceLevelModel.push(item);
      }
    });
  }

  cancel() {
    this.isShow = false;
  }

  save() {
    this.newCustomerLevelModel = this.customerServiceLevelModel.filter(el => el.CustomerServiceLevelId === null);
    this.loading = true;
    this.customerLevelService.addLevelCustomer(this.newCustomerLevelModel).subscribe(response => {
      this.loading = false;
      const result = <any>response;
      if (result.statusCode !== 200) {
        this.showMessage('error', result.messageCode);
        return;
      }

      this.showMessage('success', result.messageCode);
      this.getConfigLevelCustomer();
      this.isShow = false;
    });
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }
}
