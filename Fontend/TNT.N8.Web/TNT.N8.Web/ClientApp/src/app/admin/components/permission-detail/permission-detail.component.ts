import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { Router, ActivatedRoute } from '@angular/router';
import { SuccessComponent } from '../../../shared/toast/success/success.component';
import { FailComponent } from '../../../shared/toast/fail/fail.component';
import { PopupComponent } from '../../../shared/components/popup/popup.component';
import * as $ from 'jquery';
import { ngxLoadingAnimationTypes, NgxLoadingComponent } from 'ngx-loading';
import { EmployeeService } from '../../../employee/services/employee.service';
import { WarningComponent } from '../../../shared/toast/warning/warning.component';
import { GetPermission } from '../../../shared/permission/get-permission';

//SERVICES
import { PermissionService } from '../../../shared/services/permission.service';

import { MenuModule } from '../../../admin/models/menu-module.model';
import { MenuSubModule } from '../../../admin/models/menu-sub-module.model';
import { MenuPage } from '../../../admin/models/menu-page.model';
import { MenuBuild } from '../../../admin/models/menu-build.model';


@Component({
  selector: 'app-permission-detail',
  templateUrl: './permission-detail.component.html',
  styleUrls: ['./permission-detail.component.css']
})
export class PermissionDetailComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;

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
  loading: boolean = false;

  /*Create Permission Form*/
  createPermissionForm: FormGroup;
  roleValueControl: FormControl;
  roleDescriptionControl: FormControl;
  /*End*/

  /*Role value*/
  roleId: string = null;
  roleValue: string = null;
  roleDescription: string = null;
  /*End*/

  /*Tree data*/
  listActionResource: Array<any> = [];
  listTreeData: Array<any> = [];
  newListActionResource: Array<string> = [];
  listCurrentActionResource: Array<any> = [];
  listCurrentTreeData: Array<any> = [];
  /*End*/

  actionEdit: boolean = true;
  actionDelete: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };
  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };

  dialogPopup: MatDialogRef<PopupComponent>;

  listMenuModule: Array<MenuModule> = [];

  module_maping: Array<any> = [];
  resource_maping: Array<any> = [];
  action_maping: Array<any> = [];

  constructor(
    private permissisonService: PermissionService,
    private getPermission: GetPermission,
    private translate: TranslateService,
    public snackBar: MatSnackBar,
    private router: Router,
    public dialog: MatDialog,
    private employeeService: EmployeeService,
    private el: ElementRef,
    private route: ActivatedRoute,
  ) { }

  async ngOnInit() {
    this.setForm();
    let resource = "sys/admin/permission-detail/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.snackBar.openFromComponent(WarningComponent, { data: "Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ", ...this.warningConfig });
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      this.route.params.subscribe(params => {
        this.roleId = params['roleId'];
      });
      //this.setForm();
      this.getMasterData();
    }
  }

  setForm() {
    this.roleValueControl = new FormControl('', [Validators.required]);
    this.roleDescriptionControl = new FormControl('');

    this.createPermissionForm = new FormGroup({
      roleValueControl: this.roleValueControl,
      roleDescriptionControl: this.roleDescriptionControl
    });
  }

  getMasterData() {
    this.loading = true;

    this.permissisonService.getDetailPermission(this.roleId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.module_maping = result.module_Mapping;
        this.action_maping = result.action_Mapping;
        this.resource_maping = result.resource_Mapping;

        this.roleValue = result.role.roleValue;
        this.roleDescription = result.role.description;
        this.listActionResource = result.listActionResource;
        this.listCurrentActionResource = result.listCurrentActionResource;
        if (this.listActionResource.length > 0) {
          this.buildTreeDataActionResource();
        }
        if (this.listCurrentActionResource.length > 0) {
          this.buildTreeDataCurrentActionResource();
        }
        if (this.listCurrentActionResource.length > 0) {
          //Maping treeData and currentTreeData
          this.treeDataMaping();
        }

        /*Build tree Màn hình mặc định*/
        let listMenuBuild: Array<MenuBuild> = result.listMenuBuild;

        listMenuBuild.forEach(item => {
          if (item.level == 0) {
            let menuModule = new MenuModule();
            menuModule.code = item.code;
            menuModule.name = item.name;
            this.listMenuModule.push(menuModule);
          }
        });

        let listMenuSubModule: Array<MenuSubModule> = [];
        listMenuBuild.forEach(item => {
          if (item.level == 1) {
            let menuSubModule = new MenuSubModule();
            menuSubModule.indexOrder = item.indexOrder;
            menuSubModule.name = item.name;
            menuSubModule.code = item.code;
            menuSubModule.codeParent = item.codeParent;
            menuSubModule.path = item.path ?? '';

            listMenuSubModule.push(menuSubModule);
          }
        });

        let listMenuPage: Array<MenuPage> = [];
        listMenuBuild.forEach(item => {
          if (item.level == 2) {
            let menuPage = new MenuPage();
            menuPage.name = item.name;
            menuPage.codeParent = item.codeParent;
            menuPage.nameIcon = item.nameIcon;
            menuPage.path = item.path;

            //Nếu là màn hình chi tiết thì không hiển thị
            if (item.isPageDetail) {
              menuPage.isShow = false;
            }

            listMenuPage.push(menuPage);
          }
        });

        this.listMenuModule.forEach(menuModule => {
          menuModule.children = listMenuSubModule.filter(x => x.codeParent == menuModule.code);

          menuModule.children.forEach(subMenuModule => {
            subMenuModule.children = listMenuPage.filter(x => x.codeParent == subMenuModule.code);
          });
        });
      }
      else {
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
      }
    });
  }

  /*BEGIN: Build Tree Data Action Resource*/
  buildTreeDataActionResource() {
    this.listActionResource.forEach(item => {
      let option: any =
      {
        key: '',
        name: '',
        checked: false,
        indeterminate: false,
        children: []
      };

      let moduleKey = item.actionResource1.trim().slice(0, item.actionResource1.trim().indexOf('/'));
      let moduleName = this.module_maping.find(e => e.key == moduleKey).name;

      option.key = moduleKey;
      option.name = moduleName;
      option.children = [];

      if (this.listTreeData.length > 0) {
        if (this.checkModuleKey(moduleKey)) {
          //Nếu chưa tồn tại module
          this.listTreeData.push(option);

          //map resource
          let resourceKey = item.actionResource1.trim().slice(item.actionResource1.trim().indexOf('/') + 1, item.actionResource1.trim().lastIndexOf('/'));
          let resourceName = this.resource_maping.find(e => e.key == resourceKey).name;

          if (this.checkResourceKey(moduleKey, resourceKey)) {
            //Nếu chưa tồn tại resource
            let resource: any =
            {
              key: '',
              name: '',
              toggle: '',
              checked: false,
              indeterminate: false,
              children: []
            };
            resource.key = resourceKey;
            resource.name = resourceName;
            resource.toggle = resourceKey.replace(/\//g, '');

            this.listTreeData.find(e => e.key == moduleKey).children.push(resource);

            let actionKey = item.actionResource1.trim().slice(item.actionResource1.trim().lastIndexOf('/') + 1);
            let actionName = this.action_maping.find(e => e.key == actionKey).name;

            let action: any =
            {
              key: '',
              name: '',
              checked: false,
            };
            action.key = actionKey;
            action.name = actionName;

            this.listTreeData.find(e => e.key == moduleKey).children.find(f => f.key == resourceKey).children.push(action);
          }
          else {
            //Nếu đã tồn tại resource
            let actionKey = item.actionResource1.trim().slice(item.actionResource1.trim().lastIndexOf('/') + 1);
            let actionName = this.action_maping.find(e => e.key == actionKey).name;

            let action: any =
            {
              key: '',
              name: '',
              checked: false,
            };
            action.key = actionKey;
            action.name = actionName;

            this.listTreeData.find(e => e.key == moduleKey).children.find(f => f.key == resourceKey).children.push(action);
          }
        }
        else {
          //Nếu đã tồn tại module
          //map resource
          let resourceKey = item.actionResource1.trim().slice(item.actionResource1.trim().indexOf('/') + 1, item.actionResource1.trim().lastIndexOf('/'));
          if (!this.resource_maping.find(e => e.key == resourceKey)) {

          }
          if (this.resource_maping.find(e => e.key == resourceKey) == null || this.resource_maping.find(e => e.key == resourceKey) == undefined) {

          }

          if(!this.resource_maping.find(e => e.key == resourceKey)){
          }
          let resourceName = this.resource_maping.find(e => e.key == resourceKey).name;

          if (this.checkResourceKey(moduleKey, resourceKey)) {
            //Nếu chưa tồn tại resource
            let resource: any =
            {
              key: '',
              name: '',
              checked: false,
              indeterminate: false,
              children: []
            };
            resource.key = resourceKey;
            resource.name = resourceName;
            resource.toggle = resourceKey.replace(/\//g, '');

            this.listTreeData.find(e => e.key == moduleKey).children.push(resource);

            let actionKey = item.actionResource1.trim().slice(item.actionResource1.trim().lastIndexOf('/') + 1);
            let actionName = this.action_maping.find(e => e.key == actionKey).name;

            let action: any =
            {
              key: '',
              name: '',
              checked: false,
            };
            action.key = actionKey;
            action.name = actionName;

            this.listTreeData.find(e => e.key == moduleKey).children.find(f => f.key == resourceKey).children.push(action);
          }
          else {
            //Nếu đã tồn tại resource
            let actionKey = item.actionResource1.trim().slice(item.actionResource1.trim().lastIndexOf('/') + 1);
            let actionName = this.action_maping.find(e => e.key == actionKey).name;

            let action: any =
            {
              key: '',
              name: '',
              checked: false,
            };
            action.key = actionKey;
            action.name = actionName;

            this.listTreeData.find(e => e.key == moduleKey).children.find(f => f.key == resourceKey).children.push(action);
          }
        }
      }
      else {
        //Nếu là bản ghi đầu tiên của module
        this.listTreeData.push(option);

        let resourceKey = item.actionResource1.trim().slice(item.actionResource1.trim().indexOf('/') + 1, item.actionResource1.trim().lastIndexOf('/'));
        let resourceName = this.resource_maping.find(e => e.key == resourceKey).name;

        //Nếu chưa tồn tại resource
        let resource: any =
        {
          key: '',
          name: '',
          checked: false,
          indeterminate: false,
          children: []
        };
        resource.key = resourceKey;
        resource.name = resourceName;
        resource.toggle = resourceKey.replace(/\//g, '');

        this.listTreeData.find(e => e.key == moduleKey).children.push(resource);

        let actionKey = item.actionResource1.trim().slice(item.actionResource1.trim().lastIndexOf('/') + 1);
        let actionName = this.action_maping.find(e => e.key == actionKey).name;

        let action: any =
        {
          key: '',
          name: '',
          checked: false,
        };
        action.key = actionKey;
        action.name = actionName;

        this.listTreeData.find(e => e.key == moduleKey).children.find(f => f.key == resourceKey).children.push(action);
      }
    });
  }

  checkModuleKey(moduleKey) {
    let result = true;  //Chưa tồn tại module này trong mảng
    this.listTreeData.forEach(item => {
      if (item.key == moduleKey) {
        result = false; //Đã tồn tại module này trong mảng
      }
    });
    return result;
  }

  checkResourceKey(moduleKey, resourceKey) {
    let result = true; //Chưa tồn tại resource này trong mảng
    this.listTreeData.forEach(item => {
      if (item.key == moduleKey) {
        item.children.forEach(resource => {
          if (resource.key == resourceKey) {
            result = false;
          }
        });
      }
    });
    return result;
  }
  /*END: Build Tree Data Action Resource*/

  /*BEGIN: Build Tree Data Current Action Resource*/
  buildTreeDataCurrentActionResource() {
    this.listCurrentActionResource.forEach(item => {
      let option: any =
      {
        key: '',
        name: '',
        checked: false,
        indeterminate: false,
        children: []
      };

      let moduleKey = item.actionResource1.trim().slice(0, item.actionResource1.trim().indexOf('/'));
      let moduleName = this.module_maping.find(e => e.key == moduleKey).name;

      option.key = moduleKey;
      option.name = moduleName;
      option.children = [];

      if (this.listCurrentTreeData.length > 0) {
        if (this.checkModuleKeyForCurrentTreeData(moduleKey)) {
          //Nếu chưa tồn tại module
          this.listCurrentTreeData.push(option);

          //map resource
          let resourceKey = item.actionResource1.trim().slice(item.actionResource1.trim().indexOf('/') + 1, item.actionResource1.trim().lastIndexOf('/'));
          let resourceName = this.resource_maping.find(e => e.key == resourceKey).name;

          if (this.checkResourceKeyForCurrentTreeData(moduleKey, resourceKey)) {
            //Nếu chưa tồn tại resource
            let resource: any =
            {
              key: '',
              name: '',
              toggle: '',
              checked: false,
              indeterminate: false,
              children: []
            };
            resource.key = resourceKey;
            resource.name = resourceName;
            resource.toggle = resourceKey.replace(/\//g, '');

            this.listCurrentTreeData.find(e => e.key == moduleKey).children.push(resource);

            let actionKey = item.actionResource1.trim().slice(item.actionResource1.trim().lastIndexOf('/') + 1);
            let actionName = this.action_maping.find(e => e.key == actionKey).name;

            let action: any =
            {
              key: '',
              name: '',
              checked: false,
            };
            action.key = actionKey;
            action.name = actionName;

            this.listCurrentTreeData.find(e => e.key == moduleKey).children.find(f => f.key == resourceKey).children.push(action);
          }
          else {
            //Nếu đã tồn tại resource
            let actionKey = item.actionResource1.trim().slice(item.actionResource1.trim().lastIndexOf('/') + 1);
            let actionName = this.action_maping.find(e => e.key == actionKey).name;

            let action: any =
            {
              key: '',
              name: '',
              checked: false,
            };
            action.key = actionKey;
            action.name = actionName;

            this.listCurrentTreeData.find(e => e.key == moduleKey).children.find(f => f.key == resourceKey).children.push(action);
          }
        }
        else {
          //Nếu đã tồn tại module
          //map resource
          let resourceKey = item.actionResource1.trim().slice(item.actionResource1.trim().indexOf('/') + 1, item.actionResource1.trim().lastIndexOf('/'));
          let resourceName = this.resource_maping.find(e => e.key == resourceKey).name;

          if (this.checkResourceKeyForCurrentTreeData(moduleKey, resourceKey)) {
            //Nếu chưa tồn tại resource
            let resource: any =
            {
              key: '',
              name: '',
              checked: false,
              indeterminate: false,
              children: []
            };
            resource.key = resourceKey;
            resource.name = resourceName;
            resource.toggle = resourceKey.replace(/\//g, '');

            this.listCurrentTreeData.find(e => e.key == moduleKey).children.push(resource);

            let actionKey = item.actionResource1.trim().slice(item.actionResource1.trim().lastIndexOf('/') + 1);
            let actionName = this.action_maping.find(e => e.key == actionKey).name;

            let action: any =
            {
              key: '',
              name: '',
              checked: false,
            };
            action.key = actionKey;
            action.name = actionName;

            this.listCurrentTreeData.find(e => e.key == moduleKey).children.find(f => f.key == resourceKey).children.push(action);
          }
          else {
            //Nếu đã tồn tại resource
            let actionKey = item.actionResource1.trim().slice(item.actionResource1.trim().lastIndexOf('/') + 1);
            let actionName = this.action_maping.find(e => e.key == actionKey).name;

            let action: any =
            {
              key: '',
              name: '',
              checked: false,
            };
            action.key = actionKey;
            action.name = actionName;

            this.listCurrentTreeData.find(e => e.key == moduleKey).children.find(f => f.key == resourceKey).children.push(action);
          }
        }
      }
      else {
        //Nếu là bản ghi đầu tiên của module
        this.listCurrentTreeData.push(option);

        let resourceKey = item.actionResource1.trim().slice(item.actionResource1.trim().indexOf('/') + 1, item.actionResource1.trim().lastIndexOf('/'));
        let resourceName = this.resource_maping.find(e => e.key == resourceKey).name;

        //Nếu chưa tồn tại resource
        let resource: any =
        {
          key: '',
          name: '',
          checked: false,
          indeterminate: false,
          children: []
        };
        resource.key = resourceKey;
        resource.name = resourceName;
        resource.toggle = resourceKey.replace(/\//g, '');

        this.listCurrentTreeData.find(e => e.key == moduleKey).children.push(resource);

        let actionKey = item.actionResource1.trim().slice(item.actionResource1.trim().lastIndexOf('/') + 1);
        let actionName = this.action_maping.find(e => e.key == actionKey).name;

        let action: any =
        {
          key: '',
          name: '',
          checked: false,
        };
        action.key = actionKey;
        action.name = actionName;

        this.listCurrentTreeData.find(e => e.key == moduleKey).children.find(f => f.key == resourceKey).children.push(action);
      }
    });
  }

  checkModuleKeyForCurrentTreeData(moduleKey) {
    let result = true;  //Chưa tồn tại module này trong mảng
    this.listCurrentTreeData.forEach(item => {
      if (item.key == moduleKey) {
        result = false; //Đã tồn tại module này trong mảng
      }
    });
    return result;
  }

  checkResourceKeyForCurrentTreeData(moduleKey, resourceKey) {
    let result = true; //Chưa tồn tại resource này trong mảng
    this.listCurrentTreeData.forEach(item => {
      if (item.key == moduleKey) {
        item.children.forEach(resource => {
          if (resource.key == resourceKey) {
            result = false;
          }
        });
      }
    });
    return result;
  }
  /*END: Build Tree Data Current Action Resource*/

  treeDataMaping() {
    this.listTreeData.forEach(moduleTree => {
      this.listCurrentTreeData.forEach(moduleCurrentTree => {
        if (moduleTree.key == moduleCurrentTree.key) {
          moduleTree.children.forEach(resourceTree => {
            moduleCurrentTree.children.forEach(resourceCurrentTree => {
              if (resourceTree.key == resourceCurrentTree.key) {
                resourceTree.children.forEach(actionTree => {
                  resourceCurrentTree.children.forEach(actionCurrentTree => {
                    if (actionTree.key == actionCurrentTree.key) {
                      actionTree.checked = true;
                    }
                  });
                });
              }
            });
          });
        }
      });
    });

    //set trạng thái của resource
    this.listTreeData.forEach(moduleTree => {
      moduleTree.children.forEach(resource => {
        this.setStatusResourceAfterChangeAction(moduleTree.key, resource.key);
      });
    });

    //set trạng thái của module
    this.listTreeData.forEach(moduleTree => {
      this.setStatusModuleAfterCheckResource(moduleTree.key);
    });
  }

  changeIcon(event: any) {
    let curText = $(event.target).text().trim();
    if (curText === 'arrow_right' || curText === 'arrow_drop_down') $(event.target).text(curText === 'arrow_right' ? 'arrow_drop_down' : 'arrow_right');
    if (curText === 'add' || curText === 'remove') $(event.target).text(curText === 'add' ? 'remove' : 'add');
  }

  changeModule(elementModule) {
    //Sau khi click chọn vào module
    //Kiểm tra trạng thái trước khi được click của module là gì
    let beforeChecked = !elementModule.checked;
    if (beforeChecked) {
      //Module đang ở trạng thái check
      //Chuyển module sang trạng thái uncheck
      this.unSelectedModule(elementModule.key);
    }
    else {
      //Chuyển module sang trạng thái check
      this.selectedModule(elementModule.key);
    }
  }

  changeResource(moduleKey, resourceKey) {
    //Sau khi click kiểm tra trạng thái trước khi được click của resource là gì
    let beforeChecked = !this.listTreeData.find(e => e.key == moduleKey).children.find(f => f.key == resourceKey).checked;
    if (beforeChecked) {
      //Resource đang ở trạng thái check
      //Chuyển Resource sang trạng thái uncheck
      this.unSelectedResource(moduleKey, resourceKey);

      //Thay đổi trạng thái của Module
      this.setStatusModuleAfterCheckResource(moduleKey);
    }
    else {
      //Resource đang ở trạng thái uncheck
      this.selectedResource(moduleKey, resourceKey);
      //Thay đổi trạng thái của Module
      this.setStatusModuleAfterCheckResource(moduleKey);
    }
  }

  changeAction(moduleKey, resourceKey, actionKey) {
    //Thay đổi trạng thái của resource
    this.setStatusResourceAfterChangeAction(moduleKey, resourceKey);
    //Thay đổi trạng thái của Module
    this.setStatusModuleAfterCheckResource(moduleKey);
  }

  selectedModule(moduleKey) {
    this.listTreeData.forEach(item => {
      if (item.key == moduleKey) {
        item.checked = true;
        item.indeterminate = false;
        item.children.forEach(resource => {
          resource.checked = true;
          resource.indeterminate = false;
          resource.children.forEach(action => {
            action.checked = true;
          });
        });
      }
    });
  }

  unSelectedModule(moduleKey) {
    this.listTreeData.forEach(item => {
      if (item.key == moduleKey) {
        item.checked = false;
        item.indeterminate = false;
        item.children.forEach(resource => {
          resource.checked = false;
          resource.indeterminate = false;
          resource.children.forEach(action => {
            action.checked = false;
          });
        });
      }
    });
  }

  selectedResource(moduleKey, resourceKey) {
    this.listTreeData.forEach(item => {
      if (item.key == moduleKey) {
        item.children.forEach(resource => {
          if (resource.key == resourceKey) {
            resource.checked = true;
            resource.indeterminate = false;
            resource.children.forEach(action => {
              action.checked = true;
            });
          }
        });
      }
    });
  }

  unSelectedResource(moduleKey, resourceKey) {
    this.listTreeData.forEach(item => {
      if (item.key == moduleKey) {
        item.children.forEach(resource => {
          if (resource.key == resourceKey) {
            resource.checked = false;
            resource.indeterminate = false;
            resource.children.forEach(action => {
              action.checked = false;
            });
          }
        });
      }
    });
  }

  checkSelectedResource(moduleKey) {
    let totalResource = this.listTreeData.find(e => e.key == moduleKey).children.length;
    let unChecked = 0;
    let checked = 0;
    let indeterminate = 0;

    this.listTreeData.forEach(item => {
      if (item.key == moduleKey) {
        item.children.forEach(resource => {
          if (resource.checked == false && resource.indeterminate == false) {
            unChecked++;
          }
          else if (resource.checked == false && resource.indeterminate == true) {
            indeterminate++;
          }
          else if (resource.checked == true) {
            checked++;
          }
        });
      }
    });

    if (indeterminate > 0 && indeterminate <= totalResource) {
      //Có resource đang ở trạng thái indeterminate
      return 2;
    }

    if (checked == totalResource) {
      //Tất cả resource đều ở trạng thái check
      return 1;
    }
    else if (checked > 0 && checked < totalResource) {
      //Có cả resource đang ở trạng thái check và resource đang ở trạng thái uncheck
      return 2;
    }
    else if (unChecked == totalResource) {
      //Tất cả resource đều ở trạng thái uncheck
      return 0;
    }
  }

  setStatusModuleAfterCheckResource(moduleKey) {
    let statusModule = this.checkSelectedResource(moduleKey);
    if (statusModule == 0) {
      //Tất cả resource đều ở trạng thái uncheck
      //uncheck module
      this.unSelectedModule(moduleKey);
    }
    else if (statusModule == 1) {
      //Tất cả resource đều được check
      //check module
      this.selectedModule(moduleKey);
    }
    else if (statusModule == 2) {
      //Có resource đang ở trạng thái check và uncheck, indeterminate
      //Đổi trạng thái của module thành indeterminate
      this.listTreeData.find(e => e.key == moduleKey).checked = false;
      this.listTreeData.find(e => e.key == moduleKey).indeterminate = true;
    }
  }

  checkSelectedAction(moduleKey, resourceKey) {
    let totalAction = this.listTreeData.find(e => e.key == moduleKey).children.find(f => f.key == resourceKey).children.length;
    let unChecked = 0;
    let checked = 0;

    this.listTreeData.forEach(item => {
      if (item.key == moduleKey) {
        item.children.forEach(resource => {
          if (resource.key == resourceKey) {
            resource.children.forEach(action => {
              if (action.checked == false) {
                unChecked++;
              }
              else if (action.checked == true) {
                checked++;
              }
            });
          }
        });
      }
    });

    if (checked == totalAction) {
      //Tất cả resource đều ở trạng thái check
      return 1;
    }
    else if (checked > 0 && checked < totalAction) {
      //Có resource đang ở trạng thái check và uncheck
      return 2;
    }
    else if (unChecked == totalAction) {
      //Tất cả resource đều ở trạng thái uncheck
      return 0;
    }
  }

  setStatusResourceAfterChangeAction(moduleKey, resourceKey) {
    let statusResource = this.checkSelectedAction(moduleKey, resourceKey);
    if (statusResource == 0) {
      //Tất cả action đều ở trạng thái uncheck
      //uncheck resource
      this.unSelectedResource(moduleKey, resourceKey);
    }
    else if (statusResource == 1) {
      //Tất cả action đều được check
      //check resource
      this.selectedResource(moduleKey, resourceKey);
    }
    else if (statusResource == 2) {
      //Có action đang ở trạng thái check và uncheck, indeterminate
      //Đổi trạng thái của module thành indeterminate
      this.listTreeData.find(e => e.key == moduleKey).children.find(f => f.key == resourceKey).checked = false;
      this.listTreeData.find(e => e.key == moduleKey).children.find(f => f.key == resourceKey).indeterminate = true;
    }
  }

  btnSaveClick() {
    if (!this.createPermissionForm.valid) {
      Object.keys(this.createPermissionForm.controls).forEach(key => {
        if (!this.createPermissionForm.controls[key].valid) {
          this.createPermissionForm.controls[key].markAsTouched();
        }
      });
    }
    else {
      let _title = "XÁC NHẬN";
      let _content = "Bạn có chắc chắn muốn sửa nhóm quyền này?";

      this.dialogPopup = this.dialog.open(PopupComponent,
        {
          width: '500px',
          height: 'auto',
          autoFocus: false,
          data: { title: _title, content: _content }
        });

      this.dialogPopup.afterClosed().subscribe(resultDialog => {
        if (resultDialog) {
          this.newListActionResource = [];
          this.listTreeData.forEach(item => {
            if (item.checked == true && item.indeterminate == false) {
              //Nếu là chọn tất cả resource con bên trong thì list ra
              this.getListActionResourceByModuleKey(item.key);
            }
            else if (item.checked == false && item.indeterminate == true) {
              //Nếu là chọn một số phần tử resource bên trong
              //Kiểm tra tiếp trong resource
              item.children.forEach(resource => {
                if (resource.checked == true && resource.indeterminate == false) {
                  //Nếu là chọn tất cả action con bên trong thì list ra
                  this.getListActionResourceByResource(item.key, resource.key);
                }
                else if (resource.checked == false && resource.indeterminate == true) {
                  //Nếu là chọn một số action bên trong
                  //Kiểm tra tiếp trong action
                  resource.children.forEach(action => {
                    if (action.checked == true) {
                      //Nếu action được check thì list ra
                      let actionResource = item.key + '/' + resource.key + '/' + action.key;
                      this.newListActionResource.push(actionResource);
                    }
                  });
                }
              });
            }
          });

          /*Lấy các sub menu module và đường dẫn mặc định đã chọn*/
          let listMenuBuild: Array<MenuBuild> = [];
          this.listMenuModule.forEach(menuModule => {
            menuModule.children.forEach(_subMenuModule => {
              if (_subMenuModule.path.trim() != '') {
                let menuBuild = new MenuBuild();
                menuBuild.code = _subMenuModule.code;
                menuBuild.path = _subMenuModule.path;

                listMenuBuild.push(menuBuild);
              }
            });
          });
          /*End*/

          this.loading = true;
          this.permissisonService.editRoleAndPermission(this.roleId, this.roleValue, this.roleDescription, this.newListActionResource, listMenuBuild).subscribe(response => {
            let result: any = response;
            this.loading = false;
            if (result.statusCode == 200) {
              this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
              this.router.navigate(['/admin/permission']);
            }
            else {
              this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
            }
          });
        }
      });
    }
  }

  getListActionResourceByModuleKey(moduleKey) {
    this.listTreeData.forEach(item => {
      if (item.key == moduleKey) {
        item.children.forEach(resource => {
          resource.children.forEach(action => {
            let actionResource = moduleKey + '/' + resource.key + '/' + action.key;
            this.newListActionResource.push(actionResource);
          });
        });
      }
    });
  }

  getListActionResourceByResource(moduleKey, resourceKey) {
    this.listTreeData.forEach(item => {
      if (item.key == moduleKey) {
        item.children.forEach(resource => {
          if (resource.key == resourceKey) {
            resource.children.forEach(action => {
              let actionResource = moduleKey + '/' + resource.key + '/' + action.key;
              this.newListActionResource.push(actionResource);
            });
          }
        });
      }
    });
  }

  btnCancel() {
    this.router.navigate(['/admin/permission']);
  }

  btnDelete() {
    let _title = "XÁC NHẬN";
    let _content = "Bạn có chắc chắn muốn xóa nhóm quyền này?";

    this.dialogPopup = this.dialog.open(PopupComponent,
      {
        width: '500px',
        height: 'auto',
        autoFocus: false,
        data: { title: _title, content: _content }
      });

    this.dialogPopup.afterClosed().subscribe(resultDialog => {
      if (resultDialog) {
        this.loading = true;
        this.permissisonService.deleteRole(this.roleId, this.userId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
            this.router.navigate(['/admin/permission']);
          }
          else {
            this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
          }
        });
      }
    });
  }
}
