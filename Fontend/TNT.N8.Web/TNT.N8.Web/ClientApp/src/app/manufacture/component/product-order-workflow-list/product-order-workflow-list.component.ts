import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ManufactureService } from '../../services/manufacture.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DialogService } from 'primeng';

class ProductOrderWorkflow {
  productOrderWorkflowId: string;
  code: string;
  name: string;
  isDefault: boolean;
  description: string;
  active: boolean;
}

class Active {
  name: string;
  value: boolean;
}

@Component({
  selector: 'app-product-order-workflow-list',
  templateUrl: './product-order-workflow-list.component.html',
  styleUrls: ['./product-order-workflow-list.component.css']
})
export class ProductOrderWorkflowListComponent implements OnInit {
  loading: boolean = false;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionEdit: boolean = true;

  cols: any;
  selectedColumns: any[];

  @ViewChild('myTable') myTable: Table;

  productOrderWorkflowList: Array<ProductOrderWorkflow> = [];

  code: string = null;
  name: string = null;
  description: string = null;

  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  innerWidth: number = 0; //number window size first

  listActive: Array<Active> = [
    {
      name: 'Hoạt động',
      value: true
    },
    {
      name: 'Không hoạt động',
      value: false
    }
  ];
  selectedActive: Active  = null;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private manufactureService: ManufactureService,
    private dialogService: DialogService,
    private messageService: MessageService,
  ) {
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    let resource = "man/manufacture/product-order-workflow/list";
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

      this.initTable();
      this.getMasterData();
    }
  }

  initTable() {
    this.cols = [
      { field: 'code', header: 'Mã quy trình', textAlign: 'left', display: 'table-cell' },
      { field: 'name', header: 'Tên quy trình', textAlign: 'left', display: 'table-cell' },
      { field: 'isDefault', header: 'Mặc định', textAlign: 'center', display: 'table-cell' },
      // { field: 'active', header: 'Đang hoạt động', textAlign: 'center', display: 'table-cell' },
      { field: 'description', header: 'Mô tả', textAlign: 'left', display: 'table-cell' }
    ];
    this.selectedColumns = this.cols;
  }

  getMasterData() {
    this.manufactureService.getMasterDataSearchProductOrderWorkflow().subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {

        this.searchProductOrderWorkflow();
      } else {
        let msg = { severity:'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  searchProductOrderWorkflow() {
    let active = this.selectedActive == null ? null : this.selectedActive.value;

    this.loading = true;
    this.manufactureService.searchProductOrderWorkflow(this.code, this.name, active, this.description).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.productOrderWorkflowList = result.productOrderWorkflowList;
      } else {
        let msg = { severity:'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  refreshFilter() {
    this.code = null;
    this.name = null;
    this.description = null;
    this.searchProductOrderWorkflow();
  }

  goToCreate() {
    this.router.navigate(['/manufacture/product-order-workflow/create']);
  }

  goToDetail(id: string) {
    //this.router.navigate(['/manufacture/product-order-workflow/detail', {productOrderWorkflowId: id}]);
    let url = '/manufacture/product-order-workflow/detail' + ';productOrderWorkflowId=' + id;
    window.open(url, "_blank");
  }

  leftColNumber: number = 12;
  rightColNumber: number = 0;
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

  updateDefault(productOrderWorkflowId: string, isDefault: boolean) {
    if (isDefault) {
      this.productOrderWorkflowList.forEach(item => {
        if (item.productOrderWorkflowId != productOrderWorkflowId) {
          item.isDefault = false;
        }
      })
    }

    this.loading = true;
    this.manufactureService.updateProductOrderWorkflowDefault(productOrderWorkflowId, isDefault).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        let msg = { severity:'success', summary: 'Thông báo:', detail: 'Cập nhật thành công' };
        this.showMessage(msg);
      } else {
        let msg = { severity:'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  updateActive(productOrderWorkflowId: string, active: boolean) {
    this.loading = true;
    this.manufactureService.updateProductOrderWorkflowActive(productOrderWorkflowId, active).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        let msg = { severity:'success', summary: 'Thông báo:', detail: 'Cập nhật thành công' };
        this.showMessage(msg);
      } else {
        let msg = { severity:'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

}
