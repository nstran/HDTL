import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng';
import { MessageService } from 'primeng/api';
import { ManufactureService } from '../../services/manufacture.service';

class ResultDialog {
  isError: boolean;
  isErrorPre: boolean;
  quantityUnitErr: number;
  description: string;
  idChildrent: string;
  isChildren: boolean;
  listItemId: Array<string>;

  constructor() {
    this.isError = null;
    this.isErrorPre = null;
    this.quantityUnitErr = 1;
    this.description = null;
    this.idChildrent = null;
    this.isChildren = false;
    this.listItemId = [];
  }
}

@Component({
  selector: 'app-description-error-dialog',
  templateUrl: './description-error-dialog.component.html',
  styleUrls: ['./description-error-dialog.component.css']
})
export class DescriptionErrorDialogComponent implements OnInit {

  rate: number = null;
  techniqueOrder: number = null;

  productionOrderMappingId: string = null;
  isError: string = 'a';
  isErrorPre: string = 'bb';
  showIsErrorPre: boolean = true;
  productName: string = null;
  productLength: number = 0;
  productWidth: number = 0;
  productThickness: number = 0;
  type: number = null;
  showIsErrorPrePart = false; //Lỗi tấm quy trình bán thành phẩm
  organizationCode: string = null;
  showError: boolean = true;

  listProductionOrderMapping: Array<any> = [];

  descriptionErrorForm: FormGroup;
  quantityUnitErrControl: FormControl;
  childrentControl: FormControl;
  descriptionControl: FormControl;

  constructor(
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private manufactureService: ManufactureService,
  ) { 
    this.productionOrderMappingId = this.config.data.productionOrderMappingId;
    this.rate = this.config.data.rate;
    this.techniqueOrder = this.config.data.techniqueOrder;
    this.productName = this.config.data.productName;
    this.productLength = this.config.data.productLength;
    this.productWidth = this.config.data.productWidth;
    this.productThickness = this.config.data.productThickness;
    this.type = this.config.data.type;
    this.organizationCode = this.config.data.organizationCode;

    if (this.techniqueOrder == 1) {
      //Nếu là tiến trình đầu tiên thì ẩn lỗi tấm trước đi
      this.showIsErrorPre = false;

      //Nếu là lỗi ở item cha (trường hợp ví dụ: Dán + Dán => Dán)
      if (this.type == 1) {
        //Ẩn lỗi tấm quy trình này
        this.showError = false;

        //Hiển thị lỗi tấm bán thành phẩm
        this.showIsErrorPrePart = true;

        //Mặc định tích vào tấm bán thành phẩm
        this.isErrorPre = 'cc';

        this.productName = '';
        this.productThickness = null;
      }
    } else {
      //Nếu không phải tiến trình đầu tiên
      //Ẩn lỗi tấm quy trình này
      this.showError = false;

      //Mặc định tích vào tấm quy trình trước
      this.isErrorPre = 'aa';

      //Nếu là lỗi ở item cha
      if (this.type == 1) {
        //Hiển thị lỗi tấm bán thành phẩm
        this.showIsErrorPrePart = true;

        //Mặc định tích vào tấm bán thành phẩm
        this.isErrorPre = 'cc';

        this.productName = '';
        this.productThickness = null;
      }
    }

    // if (this.type == 1) {
    //   this.showIsErrorPrePart = true;
    // }

    // //Nếu không phải tổ Cắt
    // if (this.organizationCode != 'TCAT') {
    //   //Ẩn lỗi tấm quy trình này
    //   this.showError = false;
    //   //Mặc định tích vào tấm quy trình trước
    //   this.isErrorPre = 'aa';

    //   //Nếu là lỗi ở item cha
    //   if (this.type == 1) {
    //     //Mặc định tích vào tấm bán thành phẩm
    //     this.isErrorPre = 'cc';

    //     this.productName = '';
    //     this.productThickness = null;
    //   }
    // }
  }

  ngOnInit() {
    this.initForm();

    if (this.type == 1) {
      this.manufactureService.getListChildrentItem(this.productionOrderMappingId).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.listProductionOrderMapping = result.listProductionOrderMapping;
        } else {
          let msg = { key: 'popup', severity:'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  initForm() {
    // this.quantityUnitErrControl = new FormControl(1);
    this.descriptionControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.childrentControl = new FormControl(null);

    this.descriptionErrorForm = new FormGroup({
      // quantityUnitErrControl: this.quantityUnitErrControl,
      descriptionControl: this.descriptionControl,
      childrentControl: this.childrentControl
    });
  }

  cancel() {
    this.ref.close();
  }

  changeType() {
    if (this.isErrorPre == 'aa') {
      this.childrentControl.setValue(null);
      this.childrentControl.setValidators(null);
      this.childrentControl.updateValueAndValidity();

      this.productName = this.config.data.productName;
      this.productLength = this.config.data.productLength;
      this.productWidth = this.config.data.productWidth;
      this.productThickness = this.config.data.productThickness;
    } else if (this.isErrorPre == 'bb') {
      this.childrentControl.setValue(null);
      this.childrentControl.setValidators(null);
      this.childrentControl.updateValueAndValidity();

      this.productName = this.config.data.productName;
      this.productLength = this.config.data.productLength;
      this.productWidth = this.config.data.productWidth;
      this.productThickness = this.config.data.productThickness;
    } else if (this.isErrorPre == 'cc') {
      this.childrentControl.setValidators([Validators.required]);
      this.childrentControl.updateValueAndValidity();

      this.productName = '';
      this.productWidth = null;
      this.productLength = null;
      this.productThickness = null;
    }
  }

  /*Event: Khi chọn Bán thành phẩm*/
  choosePartItem() {
    let listChidren: Array<any> = this.childrentControl.value;

    if (listChidren.length == 0) {
      this.productName = '';
      this.productThickness = null;
    } else {
      if (listChidren.length == 1) {
        this.productName = this.childrentControl.value[0].productName;
        this.productThickness = this.childrentControl.value[0].productThickness;
      } else {
        this.productName = '';
        this.productThickness = null;
      }
    }
  }

  // changeQuantityUnitErr() {
  //   if (this.quantityUnitErrControl.value == '' || this.quantityUnitErrControl.value == '0') {
  //     this.quantityUnitErrControl.setValue('1');
  //   } else {
  //     let quantity = ParseStringToFloat(this.quantityUnitErrControl.value);

  //     if (quantity > this.rate) {
  //       let msg = { key: 'popup', severity:'warn', summary: 'Thông báo:', detail: 'Số tấm hỏng không được vượt quá hệ số/lần hỏng' };
  //       this.showMessage(msg);
  //     }
  //   }
  // }

  save() {
    let result = new ResultDialog();

    if (this.isError == 'a') {
      if (!this.descriptionErrorForm.valid) {
        Object.keys(this.descriptionErrorForm.controls).forEach(key => {
          if (this.descriptionErrorForm.controls[key].valid == false) {
            this.descriptionErrorForm.controls[key].markAsTouched();
          }
        });
      } else {
        result.isError = true;
        result.description = this.descriptionControl.value.trim();

        if (this.isErrorPre == 'aa') {
          //Tấm quy trình trước
          result.isErrorPre = true;
          result.quantityUnitErr = 1; //ParseStringToFloat(this.quantityUnitErrControl.value);
          result.idChildrent = null;

          if (result.quantityUnitErr > this.rate) {
            let msg = { key: 'popup', severity:'warn', summary: 'Thông báo:', detail: 'Số tấm hỏng không được vượt quá hệ số/lần hỏng' };
            this.showMessage(msg);
          } else {
            this.ref.close(result);
          }
        } else if (this.isErrorPre == 'bb') {
          //Tấm quy trình này
          result.isErrorPre = false;
          result.quantityUnitErr = 1;
          result.idChildrent = null;

          this.ref.close(result);
        } else if (this.isErrorPre == 'cc') {
          //Tấm bán thành phẩm
          result.isErrorPre = true;
          result.quantityUnitErr = 1;
          result.idChildrent = null;
          result.isChildren = true;
          result.listItemId = this.childrentControl.value.map(x => x.productionOrderMappingId);

          this.ref.close(result);
        }
      }
    } else {
      result.isError = false;
      result.description = null;

      this.ref.close(result);
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

}

function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
      }
    }
  }
  return null;
};

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
};