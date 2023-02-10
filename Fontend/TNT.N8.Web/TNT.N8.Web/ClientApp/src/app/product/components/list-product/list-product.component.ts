import { Component, OnInit, Injector } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission';
import { Product } from './list-product-model';
import { debounceTime, startWith, switchMap, tap } from 'rxjs/operators';
import { AbstractBase } from '../../../shared/abstract-base.component';

@Component({
  selector: 'app-list-product',
  templateUrl: './list-product.component.html',
  styleUrls: ['./list-product.component.css']
})
export class ListProductComponent extends AbstractBase implements OnInit {
  first: number = 0;
  loading: boolean = false;
  actionAdd: boolean = true;
  actionDelete: boolean = true;
  listProduct: Product[] = [];
  rows: number = 10;
  cols: any[];
  filterText: string;
  queryControl = new FormControl("");

  constructor(
    injector : Injector,
    private translate: TranslateService,
    private getPermission: GetPermission,
    private productService: ProductService,
    private router: Router,
  ) {
    super(injector);
    translate.setDefaultLang('vi');
  }
  async ngOnInit() {
    //Check permission
    let resource = "sal/product/list";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
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
      this.initTable();
      this.getListProduct();
    }
  }

  initTable(): void {
    this.cols = [
      { field: 'categoryName', header: 'Loại dịch vụ', textAlign: 'left', display: 'table-cell' },
      { field: 'productName', header: 'Tên dịch vụ', textAlign: 'left', display: 'table-cell' },
    ];
  }

  getListProduct(): void {
    this.queryControl.valueChanges
    .pipe(
      debounceTime(1000),
      tap(() => { this.loading = true; }),
      startWith(this.filterText != "" ? this.filterText : ""),
      switchMap((query: string) =>
        this.productService.getListProduct(
          query
        ).pipe(tap(() => {
          this.loading = false;
          this.filterText = query;
        }))
      )
    )
    .subscribe((result) => {
      this.listProduct = result.productEntityModel;
      if (result.statusCode != 200 ) {
        let mgs = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(mgs);
      } 
    });
  }

  goToDetail(productId: string): void {
    this.router.navigate(['/product/createOrEdit', { productId: productId }]);
  }

  deleteProduct(productId: string): void {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.productService.updateActiveProduct(productId).subscribe(response => {
          this.loading = false;
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.listProduct = this.listProduct.filter(e => e.productId !== productId);
            let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Xóa sản phẩm thành công' };
            this.showMessage(mgs);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
            this.showMessage(mgs);
          }
        }, () => this.loading = false);
      }
    });
  }

}