import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DialogService, MessageService, TreeNode } from 'primeng';
import { forkJoin } from 'rxjs';
import { tap } from 'rxjs/internal/operators/tap';
import { CategoryEntityModel } from '../../model/category';
import { OptionCategory } from '../../model/option-category';
import { ListOptions, OptionsEntityModel } from '../../model/options';
import { OptionByIdModel } from '../../model/product-option-model';
import { ProductOptionService } from '../../service/product-option.service';
import { DialogCommonComponent } from '../dialog-common/dialog-common.component';
import { GetPermission } from '../../../../../../../src/app/shared/permission/get-permission';

@Component({
  selector: 'app-info',
  templateUrl: './info.component.html',
  styleUrls: ['./info.component.css']
})
export class InfoComponent implements OnInit {
  loading: boolean = false;
  listCategory: CategoryEntityModel[] = [];
  inforProductOption: OptionByIdModel = new OptionByIdModel();
  listProductOption: OptionByIdModel[] = [];
  listOptionCategory: OptionCategory[] = [];
  listOptionCategoryUnit: OptionCategory[] = [];
  categoryEntity: CategoryEntityModel;
  optionsEntityModel: OptionsEntityModel = new OptionsEntityModel();
  optionsEntityModelAddChild: OptionsEntityModel = new OptionsEntityModel();
  id: string = null;
  optionCategory: OptionCategory;
  optionCategoryUnit: OptionCategory;
  showError: boolean = false;
  isCreate: boolean = true;
  showModal: boolean = false;
  showModalAdd: boolean = false;
  listOptionTrees: ListOptions[] = [];
  cols: any[];
  listFolderTreeNode: TreeNode[] = [];
  /**Viewchild*/
  isDialog: boolean = false
  @ViewChild('child') childrent: DialogCommonComponent;
  /*Form Control*/
  priceVNDControl: FormControl;
  // formCtrl: FormGroup;
  isShowErrorRangeNumber: boolean = false;
  constructor(
    private _productService: ProductOptionService,
    private messageService: MessageService,
    private _activatedRoute: ActivatedRoute,
    private router: Router,
    public dialogService: DialogService,
    private getPermission: GetPermission,
  ) { }

  async ngOnInit() {

    let resource = "sal/product/product-option-detail";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }

    this._activatedRoute.params.subscribe(params => {
      this.id = params['optionId'];
      this.initData();
    });
  }

  initData(): void {
    forkJoin([this._productService.getListServiceType(), this._productService.getOptionCategory(), this._productService.getOptionCategoryUnit()])
      .subscribe(result => {
        this.optionCategory = result[0].listProductCategory;
        this.listOptionCategory = result[1].optionCategory;
        this.listOptionCategoryUnit = result[2].optionCategory;
        if (this.id != null) this.getOptionById(this.id);
      });
  }

  showToast(severity: string, summary: string, detail: string): void {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  getListServiceType(): void {
    this.loading = true;
    this._productService.getListServiceType()
      .pipe(
        tap(() => {
          this.loading = false;
        })
      )
      .subscribe(result => {
        this.optionCategory = result.listProductCategory;
      })
  }

  getOptionById(id: string): void {
    this.loading = true;
    this._productService.getOptionById(id)
      .pipe(
        tap(() => {
          this.loading = false;
        })
      ).subscribe(res => {
        this.optionsEntityModel = res.optionsEntityModel;
        this.optionCategory = this.listOptionCategory.find(x => x.categoryId == res.optionsEntityModel.categoryId);
        this.optionCategoryUnit = this.listOptionCategoryUnit.find(x => x.categoryId == res.optionsEntityModel.categoryUnitId);
      })
  }

  getOptionCategory(): void {
    this.loading = true;
    this._productService.getOptionCategory()
      .pipe(
        tap(() => {
          this.loading = false;
        })
      ).subscribe(res => {
        this.listOptionCategory = res.optionCategory;
      })
  }

  changeServiceType(event: CategoryEntityModel): void {
    this.optionsEntityModel.categoryId = event.categoryId;
    this.checkValidateDropdown();
  }

  changeCategoryUnit(event: CategoryEntityModel): void {
    this.optionsEntityModel.categoryUnitId = event.categoryId;
    this.checkValidateDropdown();
  }

  setForm() {
    this.priceVNDControl = new FormControl('0', [Validators.required]);
  }

  save() {
    if (!this.checkValidate()) {
      this.loading = true;
      if (this.id) {
        this.optionsEntityModel.categoryId = this.optionCategory.categoryId;
        this._productService.createOrUpdateOptions(this.optionsEntityModel).pipe(
          tap(() => {
            this.loading = false;
          })
        )
          .subscribe(result => {
            if (result.statusCode == 200) {
              this.showToast('success', 'Thông báo', 'Sửa thành công');
            } else {
              this.showToast('error', 'Thông báo', 'Sửa thất bại');
            }
          })
      }
      else {
        this.loading = true
        if (this.optionCategory.categoryId == null) {
          this.showToast('error', 'Thông báo', 'Vui lòng chọn loại dịch vụ');
          this.loading = false
        } else {
          this._productService.createOrUpdateOptions(this.optionsEntityModel).pipe(
            tap(() => {
              this.loading = false;
            })
          )
            .subscribe(result => {
              if (result.statusCode == 200) {
                this.showToast('success', 'Thông báo', 'Thêm thành công');
                this.router.navigate(['/product/product-option-list']);
              } else {
                this.showToast('error', 'Thông báo', 'Thêm thất bại');
              }
            })
        }

      }
    }

  }

  goToDetail(id: string) {
    this.router.navigate(['/product/product-option-detail', { optionId: id }]);
  }

  checkValidate(): boolean {
    if (this.optionCategory == undefined) {
      return this.showError = true;
    } else {
      return this.showError = false;
    }
  }

  checkValidateDropdown(): boolean {
    if (this.optionsEntityModel.categoryId == undefined) {
      return this.showError = true;
    } else {
      return this.showError = false;
    }
  }


  close(): void {
    this.showModal = false;
  }

  closeAdd(): void {
    this.showModalAdd = false;
  }

  openDialog(id: string) {
    this.optionsEntityModelAddChild.parentId = id;
    this.showModal = true;
  }

  onClickButtonCreate(id: string) {
    this.showModal = true;
    this.optionsEntityModelAddChild.parentId = id;
  }

  checkRangeNumber(x: number) {
    if (x < 0 || x > 100) {
      return this.isShowErrorRangeNumber = true;
    } else {
      return this.isShowErrorRangeNumber = false;
    }
  }
}
