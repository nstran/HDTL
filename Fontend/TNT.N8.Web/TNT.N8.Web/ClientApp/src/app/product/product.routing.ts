import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ProductComponent } from './product.component';
import { AuthGuard } from '../shared/guards/auth.guard';

import { ListProductComponent } from './components/list-product/list-product.component';
import { PriceListComponent } from './components/price-list/price-list.component';
import { CreateOrEditProductComponent } from './components/create-or-edit-product/create-or-edit-product.component';
import { ProductOptionsListComponent } from './components/product-options/product-options-list/product-options-list.component';
import { ProductOptionsDetailComponent } from './components/product-options/product-options-detail/product-options-detail.component';
import { CreateOrEditProductPacketComponent } from './components/create-or-edit-product-packet/create-or-edit-product-packet.component';
import { ListProductPacketComponent } from './components/list-product-packet/list-product-packet.component';
import { TreeProductCategoryComponent } from './components/tree-product-category/tree-product-category.component';
@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: ProductComponent,
        children: [
          {
            path: 'list',
            component: ListProductComponent,
            canActivate: [AuthGuard]
          }
          ,
          {
            path: 'createOrEdit',
            component: CreateOrEditProductComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'price-list',
            component: PriceListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'product-option-list',
            component: ProductOptionsListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'product-option-detail',
            component: ProductOptionsDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'list-product-packet',
            component: ListProductPacketComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'product-packet-createOrUpdate',
            component: CreateOrEditProductPacketComponent,
            canActivate: [AuthGuard]
          },
        ]
      }
    ])
  ],
  exports: [
    RouterModule
  ]
})
export class ProductRouting {
}
