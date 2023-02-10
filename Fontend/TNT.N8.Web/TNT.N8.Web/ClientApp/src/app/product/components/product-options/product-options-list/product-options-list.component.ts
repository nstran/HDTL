import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, NgModel, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService, TreeNode } from 'primeng';
import { Table } from 'primeng/table';
import { forkJoin } from 'rxjs';
import { tap } from 'rxjs/internal/operators/tap';
import { CategoryEntityModel } from '../model/category';
import { OptionCategory } from '../model/option-category';
import { ListOptions, OptionsEntityModel } from '../model/options';
import { ListOptionModel, ProductOptionModel } from '../model/product-option-model';
import { ProductOptionService } from '../service/product-option.service';
import { GetPermission } from '../../../../../../src/app/shared/permission/get-permission';

@Component({
  selector: 'app-product-options-list',
  templateUrl: './product-options-list.component.html',
  styleUrls: ['./product-options-list.component.css']
})
export class ProductOptionsListComponent implements OnInit {
  @ViewChild('table') table: Table;
  loading = false;
  isValidation = false;
  isInvalidForm = false;
  displayDialog = false;
  colsListProduct = [];
  cols: any[];
  rows = 10;
  optionModel: ProductOptionModel = new ProductOptionModel();
  dataTable: ListOptionModel[] = [];
  filterGlobal: ""
  searchOptionForm: FormGroup;
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  leftColNumber: number = 12;
  rightColNumber: number = 2;
  first: number = 0;
  listOptionCategory: OptionCategory[] = [];
  listCategory: OptionCategory[] = [];
  optionsEntityModel: OptionsEntityModel = new OptionsEntityModel();
  optionCategory: OptionCategory;
  // listFolderTreeNode: TreeNode[] = [];
  listOption: ListOptionModel[] = [];
  CategoryName = '';
  fromPrice: number;
  toPrice: number;
  today = new Date();
  /*Ation*/
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  /*Form Control*/
  priceProductForm: FormGroup;
  priceVNDControl: FormControl;
  constructor(
    private optionList: ProductOptionService,
    private messageService: MessageService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private getPermission: GetPermission,
  ) {
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    this.initTable();

    let resource = "sal/product/product-option-list";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }



    this.loadCategory();
  }

  initTable() {
    this.loading = false;
    this.cols = [
      { field: 'categoryName', header: 'Loại dịch vụ', textAlign: 'left', display: 'table-cell', colWidth: 'auto' },
      { field: 'optionName', header: 'Tên tùy chọn dịch vụ', textAlign: 'left', display: 'table-cell', colWidth: 'auto' },
      { field: 'price', header: 'Đơn giá(VNĐ)', textAlign: 'right', display: 'table-cell', colWidth: 'auto' },
      { field: 'description', header: 'Mô tả', textAlign: 'left', display: 'table-cell', colWidth: 'auto' }
    ];
    this.searchOptionData();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  goToDetail(id: string) {
    this.router.navigate(['/product/product-option-detail', { optionId: id }]);
  }

  addNew() {
    this.router.navigate(['/product/product-option-detail']);
  }

  refreshFilter() {
    this.filterGlobal = "";
    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.listCategory = [];
    this.fromPrice = null;
    this.toPrice = null;
    this.initTable()
  }

  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.rightColNumber = 4;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

  resetTable() {
    this.dataTable = [];
    this.filterGlobal = '';
    this.first = 0;
    this.CategoryName = ''
    // this.myTable.reset();
  }

  checkEnterPress(event: any) {
    if (event.code === "Enter") {
      this.searchOption();
    }
  }
  searchOptionData() {
    this.loading = true;
    if (this.CategoryName) {
      this.CategoryName = this.CategoryName.trim();
    }
    let listCategoryId: Array<string> = [];
    listCategoryId = this.listCategory.map(c => c.categoryId);
    this.optionList.searchProductOption(listCategoryId, this.optionModel).subscribe(response => {
      this.loading = false;
      // this.listFolderTreeNode = [];
      this.listOption = response.listOptions;
      // let listDataRoot = this.listOptionTrees.filter((x) => x.parentId == null);
      if (this.fromPrice != null) {
        this.listOption = this.listOption.filter(x => x.price >= this.fromPrice)
        if (this.toPrice != null) {
          this.listOption = this.listOption.filter(x => x.price <= this.toPrice)
        }
      }
      this.listOption = this.listOption

      // this.listOptionTrees.forEach((item) => {
      //   let nodeRoot: TreeNode = {
      //     data: item,
      //     children: this.mapDataTreeNode(this.listOptionTrees, item),
      //   };
      //   nodeRoot.expanded = true;
      //   this.listFolderTreeNode.push(nodeRoot);
      // });
      // this.listFolderTreeNode = [...this.listFolderTreeNode];
    });
  }

  async searchOption() {
    let categoryName = this.searchOptionForm.get('categoryName').value;
    let optionName = this.searchOptionForm.get('optionName').value;
    this.loading = true;
    let result: any = await this.optionList.searchProductOptionAsync(categoryName, optionName);
    this.loading = false;
    if (result.statusCode === 200) {
      this.resetTable(); //reset state of table
      this.listOption = result.listOptions;
    } else {
    }
  }

  deleteOption(id: string) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.optionList.deleteOption(id).subscribe(res => {
          if (res.statusCode == 200) {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Gửi phê duyệt thành công' };
            this.showMessage(msg);
            this.initTable();
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: res.messageCode };
            this.showMessage(msg);
          }
        })
      }
    })
  }

  changeServiceType(event: CategoryEntityModel): void {
    this.CategoryName = event.categoryName;
  }

  loadCategory(){
    forkJoin([this.optionList.getListServiceType(), this.optionList.getOptionCategory()])
    .subscribe(result => {
      this.optionCategory = result[0].listProductCategory;
      this.listOptionCategory = result[1].optionCategory;
    })
  }

  // getAllOptionTree(id: string): void {
  //   this.loading = false;
  //   this.optionList.getAllOptionTree(id)
  //     .pipe(
  //       tap(() => {
  //         this.loading = false;
  //       })
  //     ).subscribe(res => {
  //       this.listOptionTrees = res.listOptions;
  //       this.cols = [
  //         { field: 'optionName', header: 'Tên tùy chọn dịch vụ' },
  //         { field: 'price', header: 'Đơn giá dịch vụ', textAlign: 'right', display: 'table-cell', colWidth: 'auto' },
  //         { field: 'description', header: 'Mô tả' },
  //       ];

  //       this.listFolderTreeNode = [];
  //       let listDataRoot = this.listOptionTrees.filter((x) => x.parentId == null);
  //       listDataRoot.forEach((item) => {
  //         let nodeRoot: TreeNode = {
  //           data: item,
  //           children: this.mapDataTreeNode(this.listOptionTrees, item),
  //         };
  //         nodeRoot.expanded = true;
  //         this.listFolderTreeNode.push(nodeRoot);
  //       });

  //       this.listFolderTreeNode = [...this.listFolderTreeNode];
  //     })
  // }
  // mapDataTreeNode(listOptionTrees: any, option): Array<TreeNode> {
  //   let result: Array<TreeNode> = [];
  //   let listChildren = this.listOptionTrees.filter((x) => x.parentId == option.optionId);
  //   listChildren.forEach((item) => {
  //     let dataRoot = item;
  //     let listChildrenNode: Array<TreeNode> = [];
  //     if (listOptionTrees.find((x) => x.parentId == item.id)) {
  //       listChildrenNode = this.mapDataTreeNode(this.listOptionTrees, item);
  //     }
  //     let node: TreeNode = { data: dataRoot, children: listChildrenNode };
  //     node.expanded = true;
  //     result.push(node);
  //   });
  //   return result;
  // }
  setForm(){
    this.priceVNDControl = new FormControl('0', [Validators.required]);
    this.priceProductForm = new FormGroup({
      priceVNDControl: this.priceVNDControl,
    });
  }

  onPropertyChange(toPrice: NgModel){
  }
}
