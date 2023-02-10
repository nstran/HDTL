import { Component, OnInit, ElementRef, Inject } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { CategoryService } from "../../../shared/services/category.service";
import { CategoryModel } from "../../../shared/models/category.model";
import { TranslateService } from '@ngx-translate/core';
import * as $ from 'jquery';

//PrimeNg
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';

export interface IDialogData {
  selectedCategoryId: string;
  selectedCategoryName: string;
  selectedCategoryCode: string;
  sortOrder: number;
  selectedCategoryType: string;
  selectedCategoryTypeId: string;
  categoryCreated: boolean;
  categoryCreatedId: string;
  categoryEdited: boolean;
  mode: string;
  message: string;
  isDefault: boolean;
}

@Component({
  selector: 'app-masterdata-dialog',
  templateUrl: './masterdata-dialog.component.html',
  styleUrls: ['./masterdata-dialog.component.css']
})
export class MasterdataDialogComponent implements OnInit {
  categoryModel: CategoryModel = {
    CategoryId: '',
    CategoryName: '',
    CategoryCode: '',
    SortOrder: 0,
    CategoryTypeId: '',
    CreatedById: '',
    CreatedDate: null,
    UpdatedById: '',
    UpdatedDate: null,
    Active: true,
    IsDefault: true,
    IsEdit: true,
    StatusName: '',
    PotentialName: '',
    CategoryTypeName: ''
  };

  dialogData: IDialogData = {
    selectedCategoryId: "",
    selectedCategoryName: "",
    selectedCategoryCode: "",
    selectedCategoryType: "",
    selectedCategoryTypeId: "",
    categoryCreated: null,
    categoryCreatedId: "",
    categoryEdited: null,
    mode: "",
    message: "",
    sortOrder: 0,
    isDefault: false
  }

  masterDataForm: FormGroup;
  formMasterDataName: FormControl;
  formMasterDataCode: FormControl;
  formMasterDataOrder: FormControl;

  constructor(
    private el: ElementRef,
    private categoryService: CategoryService,
    private messageService: MessageService,
    private translate: TranslateService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
  ) {
    this.translate.setDefaultLang('vi');
    this.dialogData = this.config.data;
  }

  ngOnInit() {

    this.filterData();

    let pattern = '^[a-zA-Z0-9]*$';
    this.formMasterDataCode = new FormControl(this.categoryModel.CategoryCode, [Validators.required, Validators.pattern(pattern), Validators.maxLength(10)]);
    this.formMasterDataName = new FormControl(this.categoryModel.CategoryName, [Validators.required, Validators.maxLength(600)]);

    this.formMasterDataOrder = new FormControl(this.categoryModel.SortOrder, [Validators.required, Validators.pattern(pattern), Validators.maxLength(2)]);

    this.masterDataForm = new FormGroup({
      formMasterDataName: this.formMasterDataName,
      formMasterDataCode: this.formMasterDataCode,
      formMasterDataOrder: this.formMasterDataOrder
    });
    if (this.dialogData.mode === "create")
      this.formMasterDataCode.enable();
    else
      this.formMasterDataCode.disable();

    // Nếu sửa giá trị mặc định thì không cho sửa tên
    if (this.dialogData.isDefault)
      this.formMasterDataName.disable();
    else
      this.formMasterDataName.enable();
  }

  //filter các thông tin dữ liệu vào text box khi sửa
  filterData() {

    if (this.dialogData.mode === "create") {
      this.dialogData.selectedCategoryType = this.dialogData.selectedCategoryType;
      this.dialogData.selectedCategoryTypeId = this.dialogData.selectedCategoryTypeId;
      this.categoryModel.CategoryName = '';
      this.categoryModel.CategoryCode = '';
      this.categoryModel.SortOrder = 0;
    } else {
      this.categoryModel.CategoryId = this.dialogData.selectedCategoryId;
      this.categoryModel.CategoryTypeId = this.dialogData.selectedCategoryTypeId;
      this.categoryModel.CategoryName = this.dialogData.selectedCategoryName;
      this.categoryModel.CategoryCode = this.dialogData.selectedCategoryCode;
      this.categoryModel.SortOrder = this.dialogData.sortOrder == null ? 0 : this.dialogData.sortOrder;
    }
  }

  createMasterData() {
    if (!this.masterDataForm.valid) {
      Object.keys(this.masterDataForm.controls).forEach(key => {
        if (this.masterDataForm.controls[key].valid === false) {
          this.masterDataForm.controls[key].markAsTouched();
        }
      });

      let target;
      target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else {
      this.categoryService
        .createCategory(this.formMasterDataName.value ?.trim(), this.formMasterDataCode.value ?.trim(), this.dialogData.selectedCategoryTypeId, this.formMasterDataOrder.value)
        .subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.dialogData.categoryCreated = true;
            this.dialogData.message = result.messageCode;
            this.ref.close(this.dialogData);
          } else {
            this.messageService.add({
              severity: 'error', summary: 'Thông báo', detail: result.messageCode
            });
          }
        });
    }
  }

  editMasterData() {
    if (!this.masterDataForm.valid) {
      Object.keys(this.masterDataForm.controls).forEach(key => {
        if (this.masterDataForm.controls[key].valid === false) {
          this.masterDataForm.controls[key].markAsTouched();
        }
      });

      let target;
      target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else {

      this.categoryService.editCategoryById(this.dialogData.selectedCategoryId, this.formMasterDataName.value ?.trim(), this.formMasterDataCode.value ?.trim(), this.formMasterDataOrder.value).subscribe(response => {
        let result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.dialogData.categoryEdited = true;
          this.dialogData.message = result.messageCode;
          this.ref.close(this.dialogData);
        } else {
          this.messageService.add({ severity: 'error', summary: 'Thông báo', detail: result.messageCode });
        }
      });
    }
  }

  onSaveClick() {
    if (this.dialogData.mode === 'create') {
      this.createMasterData();
    } else {
      this.editMasterData();
    }
  }

  onCancelClick() {
    this.ref.close();
  }
}
