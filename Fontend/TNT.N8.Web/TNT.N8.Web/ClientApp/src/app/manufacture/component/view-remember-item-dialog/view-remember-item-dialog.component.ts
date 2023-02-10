import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng';
import { MessageService } from 'primeng/api';

import { ManufactureService } from '../../services/manufacture.service';

class RememberItem {
  rememberItemId: string;
  productionOrderId: string;
  productionOrderMappingId: string;
  quantity: number;
  description: string;
  isCheck: boolean;
  createdDate: Date;
  createdById: string;

  constructor() {
    this.rememberItemId = '00000000-0000-0000-0000-000000000000';
    this.productionOrderId = null;
    this.productionOrderMappingId = null;
    this.quantity = null;
    this.description = null;
    this.isCheck = false;
    this.createdDate = new Date();
    this.createdById = '00000000-0000-0000-0000-000000000000';
  }
}

@Component({
  selector: 'app-view-remember-item-dialog',
  templateUrl: './view-remember-item-dialog.component.html',
  styleUrls: ['./view-remember-item-dialog.component.css']
})
export class ViewRememberItemDialogComponent implements OnInit {
  loading: boolean = false;

  cols: Array<any> = [];
  listRememberItem: Array<any> = [];

  productionOrderCode: string = null;
  productName: string = null;
  productThickness: string = null;
  fromDate: Date = this.findFromDate();
  toDate: Date = new Date;

  constructor(
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private manufactureService: ManufactureService
  ) { }

  ngOnInit() {
    this.initTable();
    this.search();
  }

  initTable() {
    this.cols = [
      { field: 'productionOrderCode', header: 'Lệnh số', textAlign: 'left', display: 'table-cell' },
      { field: 'productName', header: 'Chủng loại', textAlign: 'left', display: 'table-cell' },
      { field: 'productThickness', header: 'Độ dày', textAlign: 'center', display: 'table-cell' },
      { field: 'productLength', header: 'Chiều dài', textAlign: 'center', display: 'table-cell' },
      { field: 'productWidth', header: 'Chiều rộng', textAlign: 'center', display: 'table-cell' },
      { field: 'quantity', header: 'Số tấm lỗi', textAlign: 'center', display: 'table-cell' },
      { field: 'description', header: 'Ghi chú', textAlign: 'left', display: 'table-cell' },
      { field: 'isCheck', header: 'Cắt bù', textAlign: 'left', display: 'table-cell' }
    ];
  }

  refreshFilter() {
    this.productionOrderCode = null;
    this.productName = null;
    this.productThickness = null;
    this.fromDate = this.findFromDate();
    this.toDate = new Date;

    this.search();
  }

  search() {
    //Convert data
    let productionOrderCode = null;
    if (this.productionOrderCode) {
      productionOrderCode = this.productionOrderCode.trim();
    }

    let productName = null;
    if (this.productName) {
      productName = this.productName.trim();
    }

    let productThickness = null;
    if (this.productThickness != '' && this.productThickness != null) {
      productThickness = ParseStringToFloat(this.productThickness);
    }

    let fromDate = null;
    if (this.fromDate) {
      fromDate = this.fromDate;
      fromDate.setHours(0);
      fromDate.setMinutes(0);
      fromDate.setSeconds(0);
      fromDate = convertToUTCTime(fromDate);
    }

    let toDate = null;
    if (this.toDate) {
      toDate = this.toDate;
      toDate.setHours(23);
      toDate.setMinutes(59);
      toDate.setSeconds(59);
      toDate = convertToUTCTime(toDate);
    }

    this.loading = true;
    this.manufactureService.getMasterDataViewRememberItemDialog(productionOrderCode, productName, 
        productThickness, fromDate, toDate).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listRememberItem = result.listRememberItem;
      } else {

      }
    });
  }

  changeItem(data: any) {
    let rememberItem: RememberItem = new RememberItem();
    rememberItem.rememberItemId = data.rememberItemId;
    rememberItem.productionOrderId = data.productionOrderId;
    rememberItem.productionOrderMappingId = data.productionOrderMappingId;
    rememberItem.description = data.description;
    rememberItem.isCheck = data.isCheck;

    let quantity = null;
    if (data.quantity != null && data.quantity != '') {
      if (typeof(data.quantity) == 'string') {
        quantity = ParseStringToFloat(data.quantity);
      } else if (typeof(data.quantity) == 'number') {
        quantity = data.quantity;
      }
    }

    rememberItem.quantity = quantity;

    this.manufactureService.updateRememberItem(rememberItem).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        
      } else {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  findFromDate() {
    //Từ ngày sẽ là từ 3 ngày trước đến thời điểm hiện tại
    let milisecon = 2 * 24 * 60 * 60 * 1000;
    let nowDate = new Date();
    let miliseconFromDate = nowDate.getTime() - milisecon;
    let fromDate = new Date(miliseconFromDate);

    return fromDate;
  }

  cancel() {
    this.ref.close();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
};
