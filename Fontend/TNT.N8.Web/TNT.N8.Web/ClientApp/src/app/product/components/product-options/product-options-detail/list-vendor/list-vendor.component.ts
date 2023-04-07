import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Table } from 'primeng/table';
import { VendorGroupModel, VendorListModel } from '../../model/list-vendor';
import { ListVendorService } from '../../service/list-vendor.service';
import { AbstractBase } from '../../../../../shared/abstract-base.component';
import { RegexConst } from '../../../../../shared/regex-const';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-list-vendor',
  templateUrl: './list-vendor.component.html',
  styleUrls: ['./list-vendor.component.css']
})
export class ListVendorComponent extends AbstractBase implements OnInit {
  @ViewChild('dt') table: Table;
  @Input() optionId: string;


  listVendorGroup: VendorGroupModel[] = [];
  listBank: VendorGroupModel[] = [];
  vendorList: VendorListModel[] = [];


  listVendorMappingOption: VendorListModel[] = [];


  loading = false;
  cols: any[];
  rows = 10;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  showModal: boolean = false;
  createVendorForm: FormGroup;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  emailPattern = RegexConst.emailPattern;

  vendorAdd: VendorListModel = null;

  constructor(
    injector: Injector,
    private vendorService: ListVendorService,
    private _activatedRoute: ActivatedRoute,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._activatedRoute.params.subscribe(params => {
      this.optionId = params['optionId'];
    });
    this.loadTable();
    this.initForm();
  }

  clearToast() {
    this.message.clear();
  }

  async loadTable() {
    this.loading = false;
    this.cols = [
      { field: 'vendorGroupName', header: 'Nhóm nhà cung cấp' },
      { field: 'vendorName', header: 'Tên nhà cung cấp' },
      { field: 'email', header: 'Email' },
      { field: 'phoneNumber', header: 'Số điện thoại' },
      { field: 'address', header: 'Địa chỉ' },
      { field: 'price', header: 'Đơn giá' }
    ];
    let [result]: any = await Promise.all([
      this.vendorService.getMasterDataAddVendorToOption(this.optionId)
    ]);

    this.loading = false;
    if (result.statusCode != 200) {
      this.clearToast();
      this.showToast('error', 'Thông báo', result.messageCode);
      return;
    }
    this.listVendorGroup = result.listVendorGroup;
    this.listBank = result.listBank;
    this.vendorList = result.vendorList;
    this.listVendorMappingOption = result.listVendorMappingOption;
  }

  open(): void {
    this.showModal = true;
  }

  close(): void {
    this.showModal = false;
  }

  initForm() {
    this.createVendorForm = new FormGroup({
      'VendorGroup': new FormControl(null, [Validators.required]),
      'VendorName': new FormControl('', [Validators.required, this.checkBlankString()]),
      'MST': new FormControl(''),
      'Phone': new FormControl(''),
      'Email': new FormControl(''),
      'Website': new FormControl(''),
      'Address': new FormControl(''),
      'Description': new FormControl(''),

      'Bank': new FormControl(null, [Validators.required]),
      'Account': new FormControl(null, [Validators.required, this.checkBlankString()]),
      'AccountName': new FormControl(null, [Validators.required, this.checkBlankString()]),
      'Branch': new FormControl(null, [Validators.required, this.checkBlankString()]),

      'ContactName': new FormControl(""),
      'ContactGender': new FormControl("NAM"),
      'ContactEmail': new FormControl(""),
      'ContactPhone': new FormControl(""),
    });

  }

  checkBlankString(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: boolean } => {
      if (control.value !== null && control.value !== undefined) {
        if (control.value.trim() === "") {
          return { 'blankString': true };
        }
      }
      return null;
    }
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  resetForm() {
    this.createVendorForm.reset();
    this.createVendorForm.get('VendorGroup').patchValue(null);
    this.createVendorForm.get('MST').patchValue('');
    this.createVendorForm.get('VendorName').patchValue('');
    this.createVendorForm.get('Email').patchValue('');
    this.createVendorForm.get('Phone').patchValue('');
    this.createVendorForm.get('Address').patchValue('');
  }

  async addVendorToOption() {
    if (!this.vendorAdd) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: "Vui lòng chọn nhà cung cấp!" };
      this.showMessage(msg);
      return;
    }

    this.loading = true;
    let result: any = await this.vendorService.addVendorToOption(this.vendorAdd.vendorId, this.optionId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.vendorAdd = null;
      this.loadTable();
    } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', result.messageCode);
    }
  }


  async deleteVendorMappingOption(vendorId) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: async () => {
        this.loading = true;
        let result: any = await this.vendorService.deleteVendorMappingOption(vendorId, this.optionId);
        this.loading = false;
        if (result.statusCode === 200) {
          this.clearToast();
          this.showToast('success', 'Thông báo', result.messageCode);
          this.loadTable();
        } else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      }
    })
  }

  quickCreateVendor() {
    if (!this.createVendorForm.valid) {
      Object.keys(this.createVendorForm.controls).forEach(key => {
        if (!this.createVendorForm.controls[key].valid) {
          this.createVendorForm.controls[key].markAsTouched();
        }
      });
      this.showToast('success', 'Thông báo', "Vui lòng nhập đầy đủ thông tin!");
      this.loadTable();
      return;
    }

    // this.loading = true;
    // let result: any = await this.vendorService.quickCreateVendor(vendorId, this.optionId);
    // this.loading = false;
    // if (result.statusCode === 200) {
    //   this.clearToast();
    //   this.showToast('success', 'Thông báo', result.messageCode);
    //   this.loadTable();
    // } else {
    //   this.clearToast();
    //   this.showToast('error', 'Thông báo', result.messageCode);
    // }
  }




}
