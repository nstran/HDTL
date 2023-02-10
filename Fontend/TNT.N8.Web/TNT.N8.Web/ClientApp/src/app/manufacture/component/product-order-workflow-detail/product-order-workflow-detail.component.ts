import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { ManufactureService } from '../../services/manufacture.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import * as $ from 'jquery';
import { DialogService } from 'primeng';

class ProductOrderWorkflow {
  productOrderWorkflowId: string;
  code: string;
  name: string;
  isDefault: boolean;
  description: string;
  active: boolean;
  createdDate: Date;
  createdById: string;

  constructor() {
    this.productOrderWorkflowId = '00000000-0000-0000-0000-000000000000';
    this.code = null;
    this.name = null;
    this.description = null;
    this.isDefault = false;
    this.active = true;
    this.createdDate = convertToUTCTime(new Date());
    this.createdById = '00000000-0000-0000-0000-000000000000';
  }
}

class TechniqueRequest {
  techniqueRequestId: string;
  parentId: string;
  parentName: string;
  organizationId: string;
  organizationName: string;
  techniqueName: string;
  description: string;
  active: boolean;
}

class OrderTechniqueMapping {
  orderTechniqueMappingId: string;
  productOrderWorkflowId: string;
  techniqueRequestId: string;
  techniqueOrder: number;
  rate: any;
  isDefault: boolean;
  createdDate: Date;
  createdById: string;
  techniqueName: string;

  constructor() {
    this.orderTechniqueMappingId = '00000000-0000-0000-0000-000000000000';
    this.productOrderWorkflowId = null;
    this.techniqueRequestId = null;
    this.techniqueOrder = null;
    this.rate = null;
    this.isDefault = false;
    this.createdDate = convertToUTCTime(new Date());
    this.createdById = '00000000-0000-0000-0000-000000000000';
    this.techniqueName = null;
  }
}

@Component({
  selector: 'app-product-order-workflow-detail',
  templateUrl: './product-order-workflow-detail.component.html',
  styleUrls: ['./product-order-workflow-detail.component.css']
})
export class ProductOrderWorkflowDetailComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(sessionStorage.getItem("auth"));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;

  productOrderWorkflowId: string = null;
  productOrderWorkflow: ProductOrderWorkflow = new ProductOrderWorkflow();
  listCode: Array<string> = [];
  listTechniqueRequest: Array<TechniqueRequest> = [];
  selectedTechniqueRequest: TechniqueRequest = null;
  listIgnoreTechniqueRequest: Array<TechniqueRequest> = [];
  cols: Array<any> = [];
  listAdd: Array<OrderTechniqueMapping> = [];

  /* FORM */
  productOrderWorkflowForm: FormGroup;
  codeControl: FormControl;
  nameControl: FormControl;
  descriptionControl: FormControl;
  isDefaultControl: FormControl;
  activeControl: FormControl;
  /* END FORM */

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private manufactureService: ManufactureService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private renderer: Renderer2,
    private confirmationService: ConfirmationService
  ) { }

  async  ngOnInit() {
    this.setForm()
    let resource = "man/manufacture/product-order-workflow/detail";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
    }

    this.route.params.subscribe(params => { this.productOrderWorkflowId = params['productOrderWorkflowId'] });

    //this.setForm();
    this.initTable();

    this.manufactureService.getProductOrderWorkflowById(this.productOrderWorkflowId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listCode = result.listCode;
        this.productOrderWorkflow = result.productOrderWorkflow;
        this.listAdd = result.listOrderTechniqueMapping;
        this.listTechniqueRequest = result.listTechniqueRequest;
        this.listIgnoreTechniqueRequest = result.listIgnoreTechniqueRequest;
        this.mapDataToForm();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setForm() {
    this.codeControl = new FormControl(null);
    this.nameControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.descriptionControl = new FormControl(null);
    this.isDefaultControl = new FormControl(false);
    this.activeControl = new FormControl(true);

    this.productOrderWorkflowForm = new FormGroup({
      codeControl: this.codeControl,
      nameControl: this.nameControl,
      descriptionControl: this.descriptionControl,
      isDefaultControl: this.isDefaultControl,
      activeControl: this.activeControl
    });
  }

  initTable() {
    this.cols = [
      { field: 'techniqueOrder', header: 'Thứ tự', textAlign: 'left', display: 'table-cell' },
      { field: 'techniqueName', header: 'Tiến trình', textAlign: 'left', display: 'table-cell' },
      // { field: 'rate', header: 'Hệ số', textAlign: 'center', display: 'table-cell' },
      // { field: 'isDefault', header: 'Mặc định', textAlign: 'center', display: 'table-cell' },
    ];
  }

  mapDataToForm() {
    this.codeControl.setValue(this.productOrderWorkflow.code);
    this.nameControl.setValue(this.productOrderWorkflow.name);
    this.descriptionControl.setValue(this.productOrderWorkflow.description);
    this.isDefaultControl.setValue(this.productOrderWorkflow.isDefault);
    this.activeControl.setValue(this.productOrderWorkflow.active);

    this.setValidate();
  }

  setValidate() {
    this.codeControl.setValidators([Validators.required, checkDuplicateCode(this.listCode), forbiddenSpaceText]);
    this.codeControl.updateValueAndValidity();
  }

  checkIsDefault() {
    let isDefault = this.isDefaultControl.value;
    if (isDefault == true) {
      this.loading = true;
      this.manufactureService.checkIsDefaultProductOrderWorkflow().subscribe(response => {
        let result: any = response;
        this.loading = false;

        if (result.statusCode == 200) {
          if (result.isDefaultExists) {
            this.confirmationService.confirm({
              message: 'Đã tồn tại quy trình mặc định bạn có muốn đặt quy trình này là mặc định?',
              accept: () => {
                //this.isDefaultControl.setValue(true);
              },
              reject: () => {
                this.isDefaultControl.setValue(false);
              }
            });
          }
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  addTechniqueRequest() {
    if (this.selectedTechniqueRequest) {
      let orderTechniqueMapping = new OrderTechniqueMapping();
      let techniqueRequest = this.selectedTechniqueRequest;

      orderTechniqueMapping.productOrderWorkflowId = this.productOrderWorkflowId;
      orderTechniqueMapping.techniqueRequestId = techniqueRequest.techniqueRequestId;
      orderTechniqueMapping.techniqueOrder = 0;
      orderTechniqueMapping.rate = '1';
      orderTechniqueMapping.isDefault = true;
      orderTechniqueMapping.techniqueName = techniqueRequest.techniqueName;

      this.listAdd = [...this.listAdd, orderTechniqueMapping];

      this.listIgnoreTechniqueRequest.push(this.selectedTechniqueRequest);
      this.listTechniqueRequest = this.listTechniqueRequest.filter(x => x != this.selectedTechniqueRequest);
      this.selectedTechniqueRequest = null;
    }
  }

  deleteItem(item: OrderTechniqueMapping) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listAdd = this.listAdd.filter(x => x.techniqueRequestId != item.techniqueRequestId);

        let techniqueRequest = this.listIgnoreTechniqueRequest.find(x => x.techniqueRequestId == item.techniqueRequestId);
        this.listTechniqueRequest = [...this.listTechniqueRequest, techniqueRequest];
      }
    });
  }

  cancel() {
    this.router.navigate(['/manufacture/product-order-workflow/list']);
  }

  updateProductOrderWorkflow() {
    if (!this.productOrderWorkflowForm.valid) {
      Object.keys(this.productOrderWorkflowForm.controls).forEach(key => {
        if (this.productOrderWorkflowForm.controls[key].valid == false) {
          this.productOrderWorkflowForm.controls[key].markAsTouched();
        }
      });
    } else {
      this.productOrderWorkflow.code = this.codeControl.value.trim();
      this.productOrderWorkflow.name = this.nameControl.value.trim();
      this.productOrderWorkflow.description = this.descriptionControl.value == null ? null : this.descriptionControl.value.trim();
      this.productOrderWorkflow.isDefault = this.isDefaultControl.value == null ? false : this.isDefaultControl.value;
      this.productOrderWorkflow.active = true;

      this.listAdd.forEach((item, index) => {
        item.techniqueOrder = index + 1;
        if (item.rate != null) {
          if (typeof (item.rate) == "string") {
            if (item.rate == '') {
              item.rate = 1;
            } else {
              item.rate = ParseStringToFloat(item.rate);
            }
          }
        } else {
          item.rate = 1;
        }
      });

      this.loading = true;
      this.manufactureService.updateProductOrderWorkflow(this.productOrderWorkflow, this.listAdd).subscribe(response => {
        let result: any = response;
        this.loading = false;

        if (result.statusCode == 200) {
          this.listCode = result.listCode;
          this.setValidate();
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Lưu thành công' };
          this.showMessage(msg);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

}

function checkDuplicateCode(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null) {
      if (array.indexOf(control.value.toLowerCase()) !== -1 && control.value.toLowerCase() !== "") {
        return { 'duplicateCode': true };
      }
      return null;
    }
  }
};

function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
      }
    }
  }
  return null;
};

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
