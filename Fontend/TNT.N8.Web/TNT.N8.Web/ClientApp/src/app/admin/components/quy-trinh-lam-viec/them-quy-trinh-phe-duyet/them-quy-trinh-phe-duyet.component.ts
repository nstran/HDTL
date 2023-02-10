import { OrganizationService } from './../../../../shared/services/organization.service';
import { Component, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { CauHinhQuyTrinh } from './../../../models/cau-hinh-quy-trinh.model';
import { ChonNhieuDvDialogComponent } from './../../../../shared/components/chon-nhieu-dv-dialog/chon-nhieu-dv-dialog.component';
import { CacBuocQuyTrinh } from '../../../models/cac-buoc-quy-trinh.model';
import { PhongBanTrongCacBuocQuyTrinh } from '../../../models/phong-ban-trong-cac-buoc-quy-trinh.model';

@Component({
  selector: 'app-them-quy-trinh-phe-duyet',
  templateUrl: './them-quy-trinh-phe-duyet.component.html',
  styleUrls: ['./them-quy-trinh-phe-duyet.component.css'],
  providers: [DialogService]
})
export class ThemQuyTrinhPheDuyetComponent implements OnInit {
  cauHinhQuyTrinh = new CauHinhQuyTrinh();

  formTenCauHinh: FormGroup;
  tenCauHinhControl: FormControl;

  form: FormGroup;

  get quyTrinh() {
    return this.form.get('quyTrinh') as FormArray;
  }

  listLoaiPheDuyet = [
    { name: 'Phê duyệt trưởng bộ phận', value: 1 },
    { name: 'Phòng ban phê duyệt', value: 2 }
  ];

  listOrganization: Array<any> = [];
  
  constructor(
    private messageService: MessageService,
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
    private fb: FormBuilder,
    private dialogService: DialogService,
    private organizationService: OrganizationService
  ) {
    this.cauHinhQuyTrinh = this.config.data?.cauHinhQuyTrinh;
  }

  ngOnInit(): void {
    this.tenCauHinhControl = new FormControl(null, [Validators.required]);

    this.formTenCauHinh = new FormGroup({
      tenCauHinhControl: this.tenCauHinhControl
    });

    this.form = this.fb.group({
      quyTrinh: this.fb.array([]),
    });

    this.organizationService.getAllOrganization().subscribe(response => {
      let result = <any>response;
      
      this.listOrganization = result.listAll;
      this.mapDataToForm(this.cauHinhQuyTrinh);
    });
  }

  themBuoc() {
    this.quyTrinh.push(this.addForm());
  }

  xoaBuoc(index: number) {
    this.quyTrinh.removeAt(index);
  }

  addForm() {
    return this.fb.group({
      loaiPheDuyet: [null, Validators.required],
      phongBan: [{value: [], disabled: true}],
      phongBanId: [[]]
    });
  }

  /* Thay đổi loại phê duyệt */
  changeLoaiPheDuyet(index: number) {
    this.quyTrinh.controls[index].get('phongBanId').setValue([]);
    this.quyTrinh.controls[index].get('phongBan').setValue([]);
  }

  /* Chọn đơn vị */
  openPopup(index: number) {
    let mode = 2;
    let loaiPheDuyet = this.quyTrinh.controls[index].get('loaiPheDuyet').value.value;
    if (loaiPheDuyet == 2) {
      mode = 1;
    }
    
    let listSelectedId = this.quyTrinh.controls[index].get('phongBanId').value;
    let selectedId = listSelectedId.length == 1 ? listSelectedId[0] : null;
    
    let ref = this.dialogService.open(ChonNhieuDvDialogComponent, {
      data: {
        mode: mode, //1: Multiple choice, 2: Single choice
        listSelectedId: listSelectedId,
        selectedId: selectedId
      },
      header: 'Chọn đơn vị',
      width: '40%',
      baseZIndex: 10001,
      contentStyle: {
        "min-height": "350px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result?.length > 0) {
          this.quyTrinh.controls[index].get('phongBanId').setValue(result.map(x => x.organizationId));
          this.quyTrinh.controls[index].get('phongBan').setValue(result.map(x => x.organizationName))
        }
        else {
          this.quyTrinh.controls[index].get('phongBanId').setValue([]);
          this.quyTrinh.controls[index].get('phongBan').setValue([]);
        }
      }
    });
  }

  dong() {
    this.ref.close();
  }

  save() {
    if (!this.formTenCauHinh.valid) {
      if (!this.tenCauHinhControl.valid)
        this.tenCauHinhControl.markAsTouched();
      return;
    }
    
    if (this.quyTrinh.length == 0) {
      this.showMessage('warn', 'Bạn cần thêm quy trình');
      return;
    }

    if (!this.quyTrinh.valid) {
      this.quyTrinh.controls.forEach(abstractControl => {
        if (!abstractControl.valid) {
          abstractControl.get('loaiPheDuyet').markAsTouched();
        }
      });
      return;
    }
    
    //Kiểm tra nếu Loại phê duyệt là Phòng ban phê duyệt hoặc Phòng ban xác nhận => Đã chọn phòng ban chưa?
    let error = false;
    this.quyTrinh.controls.forEach(abstractControl => {
      if (abstractControl.get('loaiPheDuyet').value.value != 1) {
        let listPhongBanId = abstractControl.get('phongBanId').value;
        if (listPhongBanId.length == 0) {
          error = true;
        }
      }
    });

    if (error) {
      this.showMessage('warn', 'Bạn chưa chọn phòng ban phê duyệt');
      return;
    }
    
    let cauHinhQuyTrinh = this.mapDataToModel(this.cauHinhQuyTrinh);
    this.ref.close(cauHinhQuyTrinh);
  }

  mapDataToModel(cauHinhQuyTrinh: CauHinhQuyTrinh) {
    cauHinhQuyTrinh.tenCauHinh = this.tenCauHinhControl.value?.trim();
    cauHinhQuyTrinh.listCacBuocQuyTrinh = [];

    let listQuyTrinh: Array<string> = [];
    this.quyTrinh.controls.forEach((abstractControl, index) => {
      //Phê duyệt trưởng bộ phận
      if (abstractControl.get('loaiPheDuyet').value.value == 1) {
        listQuyTrinh.push('Trưởng bộ phận');
      }
      //Phòng ban phê duyệt hoặc Phòng ban xác nhận
      else {
        let listPhongBan: Array<string> = abstractControl.get('phongBan').value;
        listQuyTrinh.push(listPhongBan.join(", "));
      }

      let buoc = new CacBuocQuyTrinh();
      buoc.stt = index + 1;
      buoc.loaiPheDuyet = abstractControl.get('loaiPheDuyet').value.value;
      
      let listCacPhongBanTrongBuoc: Array<string> = abstractControl.get('phongBanId').value;
      listCacPhongBanTrongBuoc.forEach(phongBanId => {
        let phongBan = new PhongBanTrongCacBuocQuyTrinh();
        phongBan.organizationId = phongBanId;

        buoc.listPhongBanTrongCacBuocQuyTrinh.push(phongBan);
      });

      cauHinhQuyTrinh.listCacBuocQuyTrinh.push(buoc);
    });

    cauHinhQuyTrinh.quyTrinh = listQuyTrinh.join(" --> ");

    return cauHinhQuyTrinh;
  }

  mapDataToForm(cauHinhQuyTrinh: CauHinhQuyTrinh) {
    this.tenCauHinhControl.setValue(cauHinhQuyTrinh.tenCauHinh);

    //Show lại các bước
    cauHinhQuyTrinh.listCacBuocQuyTrinh.forEach(buoc => {
      let newForm = this.addForm();

      let loaiPheDuyet = this.listLoaiPheDuyet.find(x => x.value == buoc.loaiPheDuyet);
      newForm.get('loaiPheDuyet').setValue(loaiPheDuyet);

      //Phòng ban phê duyệt hoặc Phòng ban xác nhận
      if (loaiPheDuyet.value != 1) {
        let listPhongBan: Array<string> = []; //chip
        let listPhongBanId: Array<string> = []; //Id
        buoc.listPhongBanTrongCacBuocQuyTrinh.forEach(phongBan => {
          let _phongBan = this.listOrganization.find(x => x.organizationId == phongBan.organizationId);
          listPhongBan.push(_phongBan.organizationName);
          listPhongBanId.push(_phongBan.organizationId);
        });

        newForm.get('phongBan').setValue(listPhongBan);
        newForm.get('phongBanId').setValue(listPhongBanId);
      }

      this.quyTrinh.push(newForm);
    });
  }

  showMessage(severity: string, detail: string) {
    this.messageService.add({key: 'popup', severity: severity, summary: 'Thông báo:', detail: detail});
  }

}
