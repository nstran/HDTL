import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { EmployeeService } from "../../../services/employee.service";
import { MessageService } from 'primeng/api';
import { isBuffer } from 'util';
import { TeacherSalaryListComponent } from '../../employee-salary/teacher-salary-list/teacher-salary-list.component';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { Router, ActivatedRoute } from '@angular/router';

class MucDanhGiaDanhGiaMappingEntityModel {
  mucDanhGiaDanhGiaMappingId: string;
  MucDanhGiaMasterDataId: string;
  DiemTu: number;
  DiemDen: number;
  DiemDanhGia: number;
  TenMucDanhGia: string;;
}

@Component({
  selector: 'app-quy-luong',
  templateUrl: './quy-luong.component.html',
  styleUrls: ['./quy-luong.component.css']
})

export class QuyLuongComponent implements OnInit {

  bienTam: any

  date2: any
  loading: boolean = true;
  isShow1: boolean = false
  isShow2: boolean = false
  isShowChinhSua1: boolean = false
  isShowChinhSua2: boolean = false
  id: any
  items: any
  isShowConfirmQuyLuong: boolean = false
  isShowConfirmDanhGia: boolean = false
  mucDanhGiaDanhGiaMappingId: Array<any>

  quyLuongData: Array<any>

  mucDanhGiaNhanVien: Array<any>
  mucDanhGiaNhanVien1: Array<any>

  listQuyLuongHeader: Array<any>
  listQuyLuong: Array<any>

  mucDanhGiaValue: any

  listMucDanhGiaHeader: Array<any>
  listMucDanhGia: Array<any>

  selectedColumns: Array<any>
  listValue: Array<any>

  //khai báo form
  quyLuongForm: FormGroup;
  namControl: FormControl;
  quyLuongControl: FormControl;
  diemToiDa: FormControl;
  tenThangDiem: FormControl;
  mucDanhGia: FormControl;
  giaTri1: FormControl;
  giaTri2: FormControl;
  dieuKienDanhGia1: FormControl
  thangDiem!: FormGroup;

  rowGroupMetadata: any;

  actionEdit: boolean = true;
  actionAdd: boolean = true;
  actionDelete: boolean = true;

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService,
    private messageService: MessageService,
    private getPermission: GetPermission,
    private router: Router,
  ) { }

  async ngOnInit() {
    this.setForm()
    let resource = "hrm/employee/quy-luong/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập' };
      this.showMessage(msg);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;

      if (listCurrentActionResource.indexOf("view") == -1) {
        this.router.navigate(['/home']);
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
    }


    await this.getMasterData()
    this.initTable()
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.employeeService.getMasterDataCreateCauHinhDanhGia();
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
      this.showMessage(msg);
      this.loading = false;
      return;
    }
    this.loading = false;
    this.quyLuongData = result.listQuyLuong
    this.mucDanhGiaNhanVien = result.listMucDanhGia
    this.listMucDanhGia = result.listThangDiemDanhGia

    this.rowGroupMetadata = {};
    //chỉnh gộp dòng

    let rowIndex = 1;
    if (this.listMucDanhGia) {
      this.listMucDanhGia.forEach((item, index) => {
        let brand = item.mucDanhGiaId;
        if (index == 0) {
          this.rowGroupMetadata[brand] = { index: 0, size: 1, rowIndex: rowIndex };
          rowIndex++;
        } else {
          let previousRowData = this.listMucDanhGia[index - 1];
          let previousRowGroup = previousRowData.mucDanhGiaId;

          if (brand === previousRowGroup) {
            this.rowGroupMetadata[brand].size++;
          }
          else {
            this.rowGroupMetadata[brand] = { index: index, size: 1, rowIndex: rowIndex };
            rowIndex++;
          }
        }
      });
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  setForm() {
    this.namControl = new FormControl(null, [Validators.required]);
    this.quyLuongControl = new FormControl(null, [Validators.required]);
    this.diemToiDa = new FormControl(0, [Validators.required]);
    this.tenThangDiem = new FormControl(null, [Validators.required]);
    this.quyLuongForm = new FormGroup({
      namControl: this.namControl,
      quyLuongControl: this.quyLuongControl
    })
    this.thangDiem = new FormGroup({
      diemToiDa: this.diemToiDa,
      tenThangDiem: this.tenThangDiem,
    })
  }

  initTable() {
    this.listQuyLuongHeader = [
      { field: 'nam', header: 'Năm' },
      { field: 'quyLuong', header: 'Quỹ lương (VNĐ)' },
    ]

    //ĐIều kiện đnáh giá
    this.listMucDanhGiaHeader = [
      { field: 'tenMucDanhGia', header: 'Tên thăng điểm', textAlign: "center", width: "30%" },
      { field: 'diemDanhGia', header: 'Điểm tối đa', textAlign: "center", width: "20%" },
      { field: 'mucDanhGia', header: 'Mức đánh giá', textAlign: "center", width: "50%" },
    ]

    this.selectedColumns = this.listQuyLuongHeader


    this.items = [];
    for (let i = 1000; i < 3000; i++) {
      this.items.push({ label: 'Năm ' + i, value: i });
    }
  }

  // Màn Hình 1

  isShowConfirmDeleteDanhGia(data: any) {
    this.id = {
      mucDanhGiaId: data.mucDanhGiaId
    }
    this.isShowConfirmDanhGia = true
  }

  //khởi tạo form array
  danhgia() {

  }
  danhGiaChiTiet = new FormArray([
    this.creatForm()
  ]);

  creatForm() {
    this.mucDanhGia = new FormControl(null, [Validators.required])
    this.giaTri1 = new FormControl(null, [Validators.required])
    this.giaTri2 = new FormControl(null, [Validators.required])
    return this.fb.group({
      mucDanhGia: this.mucDanhGia,
      giaTri1: this.giaTri1,
      giaTri2: this.giaTri2
    })
  }

  addMucDanhGia() {
    let form = this.creatForm();
    // form.reset();
    this.danhGiaChiTiet.push(form);
  }

  reMoveMucDanhGia(group, index) {
    this.danhGiaChiTiet.removeAt(index);
  }

  async submitAll() {
    var dataThangDiem = [];

    let isValid = true;

    if (!this.thangDiem.valid) {
      Object.keys(this.thangDiem.controls).forEach(key => {
        if (!this.thangDiem.controls[key].valid) {
          this.thangDiem.controls[key].markAsTouched();
        }
      });
      isValid = false;
    }

    for (let i = 0; i < this.danhGiaChiTiet.controls.length; i++) {
      let currentForm = this.danhGiaChiTiet.controls[i] as FormGroup;
      if (!currentForm.valid) {
        Object.keys(currentForm.controls).forEach(key => {
          if (!currentForm.controls[key].valid) {
            currentForm.controls[key].markAsTouched();
          }
        });
        isValid = false;
      }
    }

    if (!isValid) return;

    var thangDiem = {
      diemToiDa: this.thangDiem.get('diemToiDa') ?.value,
      tenThangDiem: this.thangDiem.get('tenThangDiem') ?.value,
    }


    for (let i = 0; i < this.danhGiaChiTiet.length; i++) {
      if (this.danhGiaChiTiet.value[i].giaTri2 == null) return;
      var e = {
        mucDanhGia: this.danhGiaChiTiet.value[i].mucDanhGia.categoryId,
        giaTri1: this.danhGiaChiTiet.value[i].giaTri1,
        giaTri2: this.danhGiaChiTiet.value[i].giaTri2,
      }
      dataThangDiem.push(e)
    }

    let listMucDanhGia = [];
    if (dataThangDiem.length > 0 && thangDiem != null) {
      this.isShow1 = true;
      for (let i = 0; i < dataThangDiem.length; i++) {
        var newObj = new MucDanhGiaDanhGiaMappingEntityModel();
        newObj.MucDanhGiaMasterDataId = dataThangDiem[i].mucDanhGia;
        newObj.DiemDanhGia = thangDiem.diemToiDa;
        newObj.TenMucDanhGia = thangDiem.tenThangDiem;
        newObj.DiemTu = dataThangDiem[i].giaTri1;
        newObj.DiemDen = dataThangDiem[i].giaTri2;
        listMucDanhGia.push(newObj);
      }
      let result: any = await this.employeeService.createCauHinhDanhGia(listMucDanhGia);

      if (result.statusCode != 200) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
        this.showMessage(msg);
        this.loading = false
        return;
      } else {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Thêm mới đánh giá thành công' };
        this.showMessage(msg);
        this.isShow1 = false;
      }
      await this.getMasterData()
    }
  }

  showAddDanhGia() {
    this.danhGiaChiTiet.clear();
    this.thangDiem.reset();
    this.diemToiDa.setValue(0);
    this.tenThangDiem.setValue('');
    this.isShow1 = !this.isShow1;

  }

  chinhSuaDanhGia(data: any) {
    this.danhGiaChiTiet.clear()
    this.isShowChinhSua1 = true
    this.mucDanhGiaDanhGiaMappingId = []


    var allData = []
    allData = this.listMucDanhGia.filter(value => {
      if (value.mucDanhGiaId == data.mucDanhGiaId) {
        return value
      }
    })
    this.id = allData[0].mucDanhGiaId


    this.diemToiDa.setValue(allData[0].diemDanhGia)
    this.tenThangDiem.setValue(allData[0].tenMucDanhGia)
    for (let i = 0; i < allData.length; i++) {
      this.mucDanhGiaDanhGiaMappingId.push(allData[i].mucDanhGiaDanhGiaMappingId)
      this.addMucDanhGia()
      this.danhGiaChiTiet.at(i).patchValue({
        giaTri1: allData[i].diemTu,
        giaTri2: allData[i].diemDen,
        mucDanhGia: this.mucDanhGiaNhanVien.find(x => x.categoryId == allData[i].mucDanhGiaMasterDataId)
      })
    }
  }

  async luuChinhSuaDanhGia() {
    let isValid = true;

    if (!this.thangDiem.valid) {
      Object.keys(this.thangDiem.controls).forEach(key => {
        if (!this.thangDiem.controls[key].valid) {
          this.thangDiem.controls[key].markAsTouched();
        }
      });
      isValid = false;
    }

    for (let i = 0; i < this.danhGiaChiTiet.controls.length; i++) {
      let currentForm = this.danhGiaChiTiet.controls[i] as FormGroup;
      if (!currentForm.valid) {
        Object.keys(currentForm.controls).forEach(key => {
          if (!currentForm.controls[key].valid) {
            currentForm.controls[key].markAsTouched();
          }
        });
        isValid = false;
      }
    }

    if (!isValid) return;

    this.isShowChinhSua1 = false
    var dataThangDiem = []

    for (let i = 0; i < this.danhGiaChiTiet.length; i++) {
      if (this.danhGiaChiTiet.value[i].giaTri2 == null) return;
      var e = {
        mucDanhGia: this.danhGiaChiTiet.value[i].mucDanhGia.categoryId,
        giaTri1: this.danhGiaChiTiet.value[i].giaTri1,
        giaTri2: this.danhGiaChiTiet.value[i].giaTri2,
      }
      dataThangDiem.push(e)
    }

    let listMucDanhGia = [];
    let mucDanhGiaId = this.id


    if (dataThangDiem.length > 0) {
      this.isShow1 = false
      for (let i = 0; i < dataThangDiem.length; i++) {
        var newObj = new MucDanhGiaDanhGiaMappingEntityModel();
        newObj.mucDanhGiaDanhGiaMappingId = this.mucDanhGiaDanhGiaMappingId[i]
        newObj.MucDanhGiaMasterDataId = dataThangDiem[i].mucDanhGia;
        newObj.DiemDanhGia = this.diemToiDa.value,
          newObj.TenMucDanhGia = this.tenThangDiem.value,
          newObj.DiemTu = dataThangDiem[i].giaTri1;
        newObj.DiemDen = dataThangDiem[i].giaTri2;
        listMucDanhGia.push(newObj);
      }
      let result: any = await this.employeeService.updateCauHinhDanhGia(listMucDanhGia, mucDanhGiaId);

      if (result.statusCode != 200) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.Message };
        this.showMessage(msg);
        this.loading = false;
        return;
      } else {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật thang điểm đánh giá thành công' };
        this.showMessage(msg);
      }
      await this.getMasterData()
      this.loading = false
    }
  }

  async xoaThangDiemDanhGia() {
    this.isShowConfirmDanhGia = false
    let result: any = await this.employeeService.deleteCauHinhDanhGia(this.id.mucDanhGiaId)
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
      this.showMessage(msg);
      this.loading = false;
      return;
    } else {
      let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa thang điểm đánh giá thành công' };
      this.showMessage(msg);
    }
    await this.getMasterData()
    this.loading = false
  }


  checkMucDanhGia(data: any) {
    var check = this.mucDanhGiaNhanVien.filter(value => {
      if (value.categoryId != data.value.categoryId) return value
    })
    this.mucDanhGiaNhanVien = check
  }

  checkGiaTri() {
    for (let i = 0; i < this.danhGiaChiTiet.length; i++) {
      this.danhGiaChiTiet.at(i).get('giaTri2').setValidators([Validators.required, Validators.max(this.diemToiDa.value),])
      this.danhGiaChiTiet.at(i).get('giaTri2').updateValueAndValidity();

      this.danhGiaChiTiet.at(i).get('giaTri1').setValidators([Validators.required, Validators.max(this.danhGiaChiTiet.at(i).get('giaTri2').value)])
      this.danhGiaChiTiet.at(i).get('giaTri1').updateValueAndValidity();



      // this.giaTri2.setValidators([Validators.required, Validators.max(this.diemToiDa.value)]);
      // this.giaTri2.updateValueAndValidity();
      // this.giaTri1.setValidators([Validators.required, Validators.max(this.giaTri2.value)]);
      // this.giaTri1.updateValueAndValidity();

    }
  }

  // Màn hình 2
  showAdd() {

    this.quyLuongControl.reset();
    this.namControl.reset();

    this.quyLuongControl.setValue(null)
    this.namControl.setValue(null)
    this.isShow2 = !this.isShow2;
  }

  showAdd2() {
    this.isShow2 = !this.isShow2;
  }

  capNhatQuyLuong(data: any) {
    var a = this.items.find(x => x.value == data.nam)
    this.isShowChinhSua2 = true
    this.id = data.quyLuongId
    this.namControl.setValue(a.value)
    this.quyLuongControl.setValue(data.quyLuong)
  }

  async xoaQuyLuong() {
    this.isShowConfirmQuyLuong = false
    var deleteData = {
      QuyLuongId: this.id.quyLuongId,
      Nam: this.id.nam,
      QuyLuong: this.id.quyLuong
    }
    this.id = null
    let result: any = await this.employeeService.deleteQuyLuong(deleteData);

    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
      this.showMessage(msg);
      return;
    } else {
      let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa quỹ lương thành công' };
      this.showMessage(msg);
    }
    await this.getMasterData()

    this.loading = false
  }

  async luuCapNhatQuyLuong() {
    this.isShowChinhSua2 = false
    var data = {
      QuyLuongId: this.id,
      Nam: this.namControl.value,
      QuyLuong: this.quyLuongControl.value
    }
    this.id = ''
    this.loading = true;
    let result: any = await this.employeeService.updateQuyLuong(data);
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
      this.showMessage(msg);
      this.loading = false;
      return;
    } else {
      let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật quỹ lương thành công!' };
      this.showMessage(msg);
      this.loading = false;
    }
    await this.getMasterData()
  }

  async submit() {
    if (!this.quyLuongForm.valid) {
      Object.keys(this.quyLuongForm.controls).forEach(key => {
        if (!this.quyLuongForm.controls[key].valid) {
          this.quyLuongForm.controls[key].markAsTouched();
        }
      });
      return;
    }

    var data = {
      Nam: this.namControl.value,
      QuyLuong: this.quyLuongControl.value
    }
    let result: any = await this.employeeService.taoQuyLuong(data)
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.message };
      this.showMessage(msg);
      this.loading = false
      return;
    } else {
      let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Thêm mới quỹ lương thành công' };
      this.showMessage(msg);
    }
    await this.getMasterData()
    this.loading = false
    this.isShow2 = false

  }

  isShowConfirmDeleteQuyLuong(data: any) {
    this.id = {
      quyLuongId: data.quyLuongId,
      nam: data.nam,
      quyLuong: data.quyLuong,
    }
    this.isShowConfirmQuyLuong = true
  }
}



