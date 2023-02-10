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

@Component({
  selector: 'app-product-order-workflow-create',
  templateUrl: './product-order-workflow-create.component.html',
  styleUrls: ['./product-order-workflow-create.component.css']
})
export class ProductOrderWorkflowCreateComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(sessionStorage.getItem("auth"));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;

  fixed: boolean = false;
  withFiexd: string = "";
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }

  listCode: Array<string> = [];

  /* Valid Form */
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  /* End */

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
    private confirmationService: ConfirmationService,
  ) {
    this.renderer.listen('window', 'click', (e: Event) => {
      /**
       * Only run when toggleButton is not clicked
       * If we don't check this, all clicks (even on the toggle button) gets into this
       * section which in the result we might never see the menu open!
       * And the menu itself is checked here, and it's where we check just outside of
       * the menu and button the condition abbove must close the menu
       */
      if (this.toggleButton && this.notifi) {
        if (this.saveAndCreate) {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target) &&
            !this.saveAndCreate.nativeElement.contains(e.target)) {
            this.isOpenNotifiError = false;
          }
        } else {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target)) {
            this.isOpenNotifiError = false;
          }
        }
      }
    });
  }

  async ngOnInit() {
    this.setForm()
    let resource = "man/manufacture/product-order-workflow/create";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
    }

    // this.setForm();
    this.getMasterData();
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

  getMasterData() {
    this.loading = true;
    this.manufactureService.getMasterDataCreateProductOrderWorkflow().subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listCode = result.listCode;
        this.setDefaultValueAndValidate();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setDefaultValueAndValidate() {
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

  createProductOrderWorkflow(mode: boolean) {
    if (!this.productOrderWorkflowForm.valid) {
      Object.keys(this.productOrderWorkflowForm.controls).forEach(key => {
        if (this.productOrderWorkflowForm.controls[key].valid == false) {
          this.productOrderWorkflowForm.controls[key].markAsTouched();
        }
      });
    } else {
      let productOrderWorkflow: ProductOrderWorkflow = new ProductOrderWorkflow();
      productOrderWorkflow.code = this.codeControl.value.trim();
      productOrderWorkflow.name = this.nameControl.value.trim();
      productOrderWorkflow.description = this.descriptionControl.value == null ? null : this.descriptionControl.value.trim();
      productOrderWorkflow.isDefault = this.isDefaultControl.value == null ? false : this.isDefaultControl.value;
      productOrderWorkflow.active = true;

      this.loading = true;
      this.awaitResult = true;
      this.manufactureService.createProductOrderWorkflow(productOrderWorkflow).subscribe(response => {
        let result: any = response;
        this.loading = false;

        if (result.statusCode == 200) {
          if (mode) {
            //Lưu và Thêm mới
            this.listCode = result.listCode;
            this.setDefaultValueAndValidate();
            this.resetForm();
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo Quy trình thành công' };
            this.showMessage(msg);
          } else {
            //Lưu
            this.router.navigate(['/manufacture/product-order-workflow/detail', { productOrderWorkflowId: result.productOrderWorkflowId }]);
          }
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  resetForm() {
    this.codeControl.reset();
    this.nameControl.reset();
    this.descriptionControl.reset();
    this.isDefaultControl.setValue(false);
    this.activeControl.setValue(true);

    this.awaitResult = false;
  }

  cancel() {
    this.router.navigate(['/manufacture/product-order-workflow/list']);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  ngOnDestroy() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
    }
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

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
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
