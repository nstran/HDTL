import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng';
import { MessageService } from 'primeng/api';

import { ManufactureService } from '../../services/manufacture.service';

@Component({
  selector: 'app-view-list-item-dialog',
  templateUrl: './view-list-item-dialog.component.html',
  styleUrls: ['./view-list-item-dialog.component.css']
})
export class ViewListItemDialogComponent implements OnInit {
  loading: boolean = false;

  productionOrderId: string;

  cols: Array<any> = [];
  listItem: Array<any> = [];

  constructor(
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private manufactureService: ManufactureService
  ) { 
    this.productionOrderId = this.config.data.productionOrderId;
  }

  ngOnInit() {
    this.initTable();

    this.loading = true;
    this.manufactureService.getMasterDataListItemDialog(this.productionOrderId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listItem = result.listItem;
      } else {

      }
    });
  }

  initTable() {
    this.cols = [
      { field: 'productName', header: 'Chủng loại', textAlign: 'left', display: 'table-cell' },
      { field: 'productColor', header: 'Màu sắc', textAlign: 'left', display: 'table-cell' },
      { field: 'productThickness', header: 'Độ dày (mm)', textAlign: 'right', display: 'table-cell' },
      { field: 'productLength', header: 'Chiều dài (mm)', textAlign: 'right', display: 'table-cell' },
      { field: 'productWidth', header: 'Chiều rộng (mm)', textAlign: 'right', display: 'table-cell' },
      { field: 'quantity', header: 'Số tấm', textAlign: 'right', display: 'table-cell' },
      { field: 'totalArea', header: 'Tổng số m2', textAlign: 'right', display: 'table-cell' },
      { field: 'completeQuantity', header: 'Số tấm đã làm', textAlign: 'right', display: 'table-cell' },
      { field: 'completeTotalArea', header: 'Tổng số m2 đã làm', textAlign: 'right', display: 'table-cell' },
      { field: 'techniqueDescription', header: 'Mã hiệu', textAlign: 'left', display: 'table-cell' },
      { field: 'workflowName', header: 'Tên quy trình', textAlign: 'left', display: 'table-cell' }
    ];
  }

  cancel() {
    this.ref.close();
  }

}
