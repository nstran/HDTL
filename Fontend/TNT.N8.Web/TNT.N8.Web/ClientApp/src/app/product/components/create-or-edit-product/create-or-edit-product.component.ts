import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { tap } from 'rxjs/operators';
import { AbstractBase } from '../../../shared/abstract-base.component';
import { CategoryEntityModel, CreateOrEditProductParameter, OptionsEntityModel, ProductEntityModel } from '../../models/product.model';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-create-or-edit-product',
  templateUrl: './create-or-edit-product.component.html',
  styleUrls: ['./create-or-edit-product.component.css']
})
export class CreateOrEditProductComponent extends AbstractBase implements OnInit {
  productEntityModel : ProductEntityModel = new ProductEntityModel();
  optionsEntityModel : OptionsEntityModel = new OptionsEntityModel();
  loading : boolean = false;
  showModal : boolean = false;
  showErrorCategoryEntity : boolean = false;
  showErrorDropdownModal : boolean = false;
  listCategory : CategoryEntityModel[] = [];
  targetOption : OptionsEntityModel[] = [];
  options : OptionsEntityModel[] = [];
  categoryEntity : CategoryEntityModel;
  productId : string;

  constructor(
    injector : Injector,
    private _productService : ProductService,
    private _activatedRoute: ActivatedRoute
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getListServiceType();

    this._activatedRoute.params.subscribe(params => {
      this.productId = params['productId'];
      if(this.productId){
        this.takeProductAndOptionsById(this.productId);
      }
    });
  }

  openDialog(): void {
    this.showModal = true;
  }
  
  closeDialog(): void {
    this.showModal = false;
    this.optionsEntityModel = new OptionsEntityModel();
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
      this.listCategory = result.listProductCategory;
    })
  }

  changeServiceType(event : CategoryEntityModel): void {
    this.loading = true;
    this.targetOption = [];
    this.options = [];
    this.categoryEntity = event;
    this.productEntityModel.serviceTypeId = event.categoryId;
    this.getListProductOptions(event.categoryId);
  }

  changeServiceTypeDiaLog(event : CategoryEntityModel): void {
    this.optionsEntityModel.categoryId = event.categoryId;
    this.checkValidateDropdown();
  }

  getListProductOptions(categoryId : string): void {
    this.loading = true; 
    this._productService.getListProductOptions(categoryId)
    .pipe(
      tap(() => {
          this.loading = false; 
        })
      )
      .subscribe(result => {
        this.options = result.listOption;
        this.targetOption.forEach(t => {
          this.options = this.options.filter(x => x.id != t.id);
        });
    })
  }

  takeProductAndOptionsById(productId : string): void { 
    this.loading = true; 
    this._productService.takeProductAndOptionsById(productId)
    .pipe(
      tap(() => {
          this.loading = false; 
        })
      )
      .subscribe(result => {
        this.productEntityModel = result.productEntityModel;
        this.categoryEntity = this.listCategory.find(x => x.categoryId == result.productEntityModel.serviceTypeId);
        this.targetOption = result.productEntityModel.listOptionsEntityModel;
        this.getListProductOptions(result.productEntityModel.serviceTypeId);
    })
  }

  checkValidate(): boolean {
    if(this.categoryEntity == undefined){
      return this.showErrorCategoryEntity = true;
    } else {
      return this.showErrorCategoryEntity = false;
    }
  }

  save(): void {
    if(!this.checkValidate()){
      this.loading = true;

      let createOrEditProductParameter = new CreateOrEditProductParameter();
      createOrEditProductParameter.productEntityModel = this.productEntityModel;
      createOrEditProductParameter.listOptionsEntityModel = this.targetOption;
      this._productService.createOrEditProduct(createOrEditProductParameter)
      .pipe(
        tap(() => {
          this.loading = false; 
        })
      )
      .subscribe(result => {
        if (result.statusCode == 200) {
          this.showToast('success', 'Thông báo', 'Lưu thành công');
          this.closeDialog();
          this.getListProductOptions(this.productEntityModel.serviceTypeId);
        } else {
          this.showToast('error', 'Thông báo', 'Lưu thất bại');
        }
      })
    }
  }

  checkValidateDropdown(): boolean {
    if(this.optionsEntityModel.categoryId == undefined){
      return this.showErrorDropdownModal = true;
    } else {
      return this.showErrorDropdownModal = false;
    }
  }

  saveProductOptions(): void {
    if(!this.checkValidateDropdown()){
      this.loading = true;
      this._productService.createProductOption(this.optionsEntityModel)
      .pipe(
        tap(() => {
          this.loading = false; 
        })
      )
      .subscribe(result => {
        if (result.statusCode == 200) {
          this.showToast('success', 'Thông báo', 'Thêm tùy chọn thành công');
          this.getListProductOptions(this.productEntityModel.serviceTypeId);
          this.closeDialog();
        } else {
          this.showToast('error', 'Thông báo', 'Thêm tùy chọn thất bại');
        }
      })
    }
  }

}
