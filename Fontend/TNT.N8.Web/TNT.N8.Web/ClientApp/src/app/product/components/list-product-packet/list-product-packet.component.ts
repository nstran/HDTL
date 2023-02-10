import { Component, Injector, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { debounceTime, startWith, switchMap, tap } from 'rxjs/operators';
import { AbstractBase } from '../../../shared/abstract-base.component';
import { GetListServicePacketResult, ServicePacketEntityModel } from '../../models/product.model';
import { ProductService } from '../../services/product.service';
import { GetPermission } from '../../../../../src/app/shared/permission/get-permission';

@Component({
  selector: 'app-list-product-packet',
  templateUrl: './list-product-packet.component.html',
  styleUrls: ['./list-product-packet.component.css']
})
export class ListProductPacketComponent extends AbstractBase implements OnInit {
  loading: boolean = false;
  rows: number = 10;
  first: number = 0;
  cols: any[];
  filterText: string;
  queryControl = new FormControl("");
  listServicePacket: ServicePacketEntityModel[] = [];

  constructor(
    injector: Injector,
    private _router: Router,
    private _productService: ProductService,
    private getPermission: GetPermission,
    private router: Router,
  ) {
    super(injector);
  }

  async ngOnInit() {
    this.initTable();

    let resource = "sal/product/list-product-packet";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }


    this.takeListProductPacket();
  }

  initTable(): void {
    this.cols = [
      { field: 'productCategoryName', header: 'Loại gói dịch vụ', textAlign: 'left', display: 'table-cell' },
      { field: 'name', header: 'Tên gói dịch vụ', textAlign: 'left', display: 'table-cell' },
      { field: 'provinceName', header: 'Khu vực áp dụng', textAlign: 'left', display: 'table-cell' },
    ];
  }

  takeListProductPacket(): void {
    this.queryControl.valueChanges
      .pipe(
        debounceTime(1000),
        tap(() => { this.loading = true; }),
        startWith(this.filterText != "" ? this.filterText : ""),
        switchMap((query: string) =>
          this._productService.getListServicePacket(
            query
          ).pipe(tap(() => {
            this.loading = false;
            this.filterText = query;
          }))
        )
      )
      .subscribe((result) => {
        this.listServicePacket = result.listServicePackageEntityModel;
      });
  }

  goToDetail(id: string): void {
    this._router.navigate(['/product/product-packet-createOrUpdate', { id: id }]);
  }

  deleteProductPacket(id: string): void {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this._productService.deleteServicePacket(id)
          .pipe(
            tap(() => { this.loading = false; }),
          )
          .subscribe(response => {
            let result = <GetListServicePacketResult>response;
            if (result.statusCode === 202 || result.statusCode === 200) {
              this.takeListProductPacket();
              let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Xóa thành công' };
              this.showMessage(mgs);
            } else {
              let mgs = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
              this.showMessage(mgs);
            }
          });
      }
    });
  }

  changeStt(rowData: ServicePacketEntityModel) {
    if(!rowData.stt){
      let mgs = { severity: 'error', summary: 'Thông báo', detail: "Hãy nhập số thứ tự!" };
      this.showMessage(mgs);
      return;
    }
    this.loading = true;
    this._productService.changeOrderServicePack(rowData.id, rowData.stt)
      .pipe(
        tap(() => { this.loading = false; }),
      )
      .subscribe(response => {
        let result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.takeListProductPacket();
          let mgs = { severity: 'success', summary: 'Thông báo', detail: result.messageCode };
          this.showMessage(mgs);
        } else {
          let mgs = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
          this.showMessage(mgs);
        }
      });

  }
}
