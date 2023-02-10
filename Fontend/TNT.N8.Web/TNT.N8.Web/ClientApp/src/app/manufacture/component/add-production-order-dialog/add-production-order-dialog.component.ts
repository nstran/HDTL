import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng';
import { MessageService } from 'primeng/api';

import { ManufactureService } from '../../services/manufacture.service';

class ResultDialog {
  listAddProductionOrder: Array<ProductionOrder>;

  constructor() {
    this.listAddProductionOrder = [];
  }
}

class ProductionOrder {
  productionOrderId: string;
  productionOrderCode: string;
  customerName: string;
  remainQuantity: number;
  remainTotalArea: number;
  endDate: Date;
  statusId: string;
  statusName: string;
  noteTechnique: string;
  especially: boolean;
  isDelete: boolean;
  parentId: string;
  isIgnore: boolean;
}

@Component({
  selector: 'app-add-production-order-dialog',
  templateUrl: './add-production-order-dialog.component.html',
  styleUrls: ['./add-production-order-dialog.component.css']
})
export class AddProductionOrderDialogComponent implements OnInit {
  loading: boolean = false;

  listIgnore: Array<string> = [];

  cols: Array<any> = [];
  listProductionOrder: Array<ProductionOrder> = [];
  selectedProductionOrder: Array<ProductionOrder> = [];

  constructor(
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private manufactureService: ManufactureService
  ) { 
    this.listIgnore = this.config.data.listIgnore;
  }

  ngOnInit() {
    this.initTable();

    this.loading = true;
    this.manufactureService.getMasterDataAddProductionOrderDialog(this.listIgnore).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listProductionOrder = result.listProductionOrder;
      } else {

      }
    });
  }

  initTable() {
    this.cols = [
      { field: 'productionOrderCode', header: 'Lệnh số', textAlign: 'left', display: 'table-cell' },
      { field: 'customerName', header: 'Khách hàng', textAlign: 'left', display: 'table-cell' },
      { field: 'remainQuantity', header: 'Số tấm', textAlign: 'center', display: 'table-cell' },
      { field: 'remainTotalArea', header: 'Số m2', textAlign: 'center', display: 'table-cell' },
      { field: 'endDate', header: 'Ngày trả', textAlign: 'right', display: 'table-cell' },
      { field: 'statusName', header: 'Trạng thái SX', textAlign: 'left', display: 'table-cell' },
      { field: 'noteTechnique', header: 'Ghi chú KT', textAlign: 'left', display: 'table-cell' }
    ];
  }

  addItem() {
    let result: ResultDialog = new ResultDialog();
    result.listAddProductionOrder = this.selectedProductionOrder;

    this.ref.close(result);
  }

  cancel() {
    this.ref.close();
  }

}
