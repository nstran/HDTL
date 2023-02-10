import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { removeData } from 'jquery';
import { ConfirmationService, MessageService, Table } from 'primeng';
import { forkJoin } from 'rxjs';
import { tap } from 'rxjs/operators';
import { AttributeConfigurationModel, CreateOptionAndAttrModel, GetGiaTriThuocTinh, GetGiaTriThuocTinhModel } from '../../model/data-type';
import { ListCategoryAttribute } from '../../model/list-category-attributes';
import { OptionsEntityModel } from '../../model/options';
import { DataTypeService } from '../../service/data-type.service';
import { ListCategoryAtrributesService } from '../../service/list-category-atrributes.service';

@Component({
  selector: 'app-properties',
  templateUrl: './properties.component.html',
  styleUrls: ['./properties.component.css']
})
export class PropertiesComponent implements OnInit {
  showModal: boolean = false;
  @ViewChild('dt') table: Table;
  loading = false;
  cols: any[];
  rows = 10;
  listCategoryAtrributes: ListCategoryAttribute[] = [];
  listAtrributes: ListCategoryAttribute[] = [];
  listAtrributesOld: ListCategoryAttribute[] = [];
  listDatatype: GetGiaTriThuocTinh[] = [];
  createOptionAndAttrModel: CreateOptionAndAttrModel = new CreateOptionAndAttrModel();
  categoryAttribute: ListCategoryAttribute = new ListCategoryAttribute();
  getGiaTriThuocTinhModel: GetGiaTriThuocTinhModel = new GetGiaTriThuocTinhModel()
  // createOrUpdateAttrConfig: 
  clonedProducts: { [s: string]: ListCategoryAttribute; } = {};
  id: string;

  createForm: FormGroup;
  rowDataControl: FormControl;
  attrControl: FormControl;
  dataTypeControl: FormControl;

  constructor(
    private categoryAtrributesService: ListCategoryAtrributesService,
    private messageService: MessageService,
    private dataTypeService: DataTypeService,
    private _activatedRoute: ActivatedRoute,
    private confirmationService: ConfirmationService,

  ) { }

  ngOnInit(): void {
    this._activatedRoute.params.subscribe(params => {
      this.id = params['optionId'];
      this.loadTable();
      this.getListCategoryAttributesById(this.id);
    });
    this.createOptionAndAttrModel.attributeConfigurationModel = new AttributeConfigurationModel();
    this.createOptionAndAttrModel.optionsEntityModel = new OptionsEntityModel();
  }

  loadTable() {
    this.cols = [
      { field: 'categoryName', header: 'Tên thuộc tính' },
      { field: 'dataType', header: 'Giá trị' },
    ];

    this.rowDataControl = new FormControl(null);
    this.attrControl = new FormControl(null, [Validators.required]);
    this.dataTypeControl = new FormControl(null, [Validators.required]);

    this.createForm = new FormGroup({
      rowDataControl: this.rowDataControl,
      attrControl: this.attrControl,
      dataTypeControl: this.dataTypeControl,
    });

  }

  open(): void {
    this.showModal = true;
    this.rowDataControl.setValue(null);
    this.attrControl.setValue(null);
    this.dataTypeControl.setValue(null);
    this.createForm.reset();
  }

  close(): void {
    this.showModal = false;
  }

  edit(): void {
    this.showModal = true;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  showToast(severity: string, summary: string, detail: string): void {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  save() {
    if (!this.createForm.valid) {
      Object.keys(this.createForm.controls).forEach(key => {
        if (this.createForm.controls[key].valid == false) {
          this.createForm.controls[key].markAsTouched();
        }
      });
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Vui lòng nhập đầy đủ thông tin!" };
      this.showMessage(msg);
      return;
    }
    this.loading = true;
    var object = new AttributeConfigurationModel();
    object.id = this.rowDataControl.value != null ? this.rowDataControl.value.id : null;
    object.dataType = this.dataTypeControl.value.value;
    object.categoryId = this.attrControl.value.categoryId;
    object.objectType = 1;//Dành cho tùy chọn
    object.objectId = this.id; //id của tùy chọn
    this.dataTypeService.createOptionAndAttrModel(object).pipe(
      tap(() => {
        this.loading = false;
      })
    )
      .subscribe(result => {
        if (result.statusCode == 200) {
          this.showToast('success', 'Thông báo', result.messageCode);
          this.close();
          this.getListCategoryAttributesById(this.id);
        } else {
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      })
  }


  deleteAttributeConfigure(id: string) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.categoryAtrributesService.deleteAttributeConfigure(id).subscribe(res => {
          if (res.statusCode == 200) {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa thuộc tính thành công!' };
            this.showMessage(msg);
            this.getListCategoryAttributesById(this.id);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: res.messageCode };
            this.showMessage(msg);
          }
        })
      }
    })
  }

  onRowEditInit(rowData: ListCategoryAttribute) {
    this.showModal = true;
    this.rowDataControl.setValue(rowData);
    this.attrControl.setValue(this.listAtrributes.find(x => x.categoryId == rowData.categoryId));
    this.dataTypeControl.setValue(this.listDatatype.find(x => x.value == rowData.dataTypeValue));
  }


  getListCategoryAttributesById(id: string): void {
    this.loading = false;
    this.categoryAtrributesService.getListCategoryAttributesById(id)
      .pipe(
        tap(() => {
          this.loading = false;
        })
      ).subscribe(res => {
        this.listCategoryAtrributes = res.listCategoryAttributes;
        this.listAtrributes = res.listAttr;
        this.listDatatype = res.listDataType;
      })
  }

  onRowEditSave(categoryAttribute: ListCategoryAttribute) {
    delete this.clonedProducts[categoryAttribute.id];
  }

  onRowEditCancel(categoryAttribute: ListCategoryAttribute, index: number) {
    this.listCategoryAtrributes[index] = this.clonedProducts[categoryAttribute.id];
    delete this.clonedProducts[categoryAttribute.id];
  }

}
