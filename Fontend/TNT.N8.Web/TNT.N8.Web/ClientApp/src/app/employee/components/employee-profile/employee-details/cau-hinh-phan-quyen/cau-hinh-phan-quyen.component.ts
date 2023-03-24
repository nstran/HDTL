import { Component, OnInit, SimpleChanges, Input, OnChanges } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { MessageService, ConfirmationService } from 'primeng/api';
import { EmployeeService } from './../../../../services/employee.service';
import { ChonNhieuDvDialogComponent } from './../../../../../shared/components/chon-nhieu-dv-dialog/chon-nhieu-dv-dialog.component';
import { DialogService } from 'primeng/dynamicdialog';

class ThanhVienPhongBan {
  id: string;
  employeeId: string;
  organizationId: string;
  isManager: number; //0: nhân viên, 1: Trưởng bộ phận, 2: Người theo dõi
  organizationName: string;
}

class ThongTinChung {
  employeeId: string;
  employeeCode: string;
  avatarUrl: string;
  userName: string; //Tên đăng nhập
  trangThaiId: number;
  organizationId: string;
  organizationName: string;
  dateOfBirth: Date;
  phone: string;
  workEmail: string;
  firstName: string;
  lastName: string;
  gioiTinh: string;
  mission: string;
  benefit: number;
  percenBenefit: number;
  employeeType: number;
}
@Component({
  selector: 'app-cau-hinh-phan-quyen',
  templateUrl: './cau-hinh-phan-quyen.component.html',
  styleUrls: ['./cau-hinh-phan-quyen.component.css']
})
export class CauHinhPhanQuyenComponent implements OnInit, OnChanges {
  loading: boolean = false;
  isEdit: boolean = false;
  @Input() listOrganization: Array<any>;
  @Input() listRole: Array<any>;
  @Input() actionEdit: boolean;
  @Input() employeeId: string;

  listNhomQuyen = [
    { label: 'Trưởng phòng', value: 0 },
    { label: 'Quản lý', value: 1 },
    { label: 'Nhân viên', value: 2 },
    { label: 'HSNS', value: 3 },
  ]

  cauHinhPhanQuyenForm: FormGroup;
  nhomQuyenControl: FormControl;
  phanQuyenDuLieuControl: FormControl;
  phanQuyenChucNangControl: FormControl;

  isManager: string = 'false';
  listSelectedDonVi: Array<ThanhVienPhongBan> = [];
  listSelectedDonViClone: Array<ThanhVienPhongBan> = [];
  cols: Array<any> = [];

  isShowButtonSua: boolean = true;
  thongTinChung = new ThongTinChung();

  constructor(
    private messageService: MessageService,
    public employeeService: EmployeeService,
    public dialogService: DialogService,
    private confirmationService: ConfirmationService
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.initTable();
  }

  initTable() {
    this.cols = [
      { field: 'organizationName', header: 'Đơn vị', textAlign: 'left', display: 'table-cell' },
      { field: 'truongBoPhan', header: 'Trưởng bộ phận', textAlign: 'center', display: 'table-cell' },
      // { field: 'nguoiTheoDoi', header: 'Người theo dõi', textAlign: 'center', display: 'table-cell' },
      { field: 'nhanVien', header: 'Nhân viên', textAlign: 'center', display: 'table-cell' },
      // { field: 'actions', header: 'Thao tác', textAlign: 'center', display: 'table-cell' },
    ];
  }

  initForm() {
    this.nhomQuyenControl = new FormControl(null);
    this.phanQuyenDuLieuControl = new FormControl(null);
    this.phanQuyenChucNangControl = new FormControl(null);

    this.cauHinhPhanQuyenForm = new FormGroup({
      nhomQuyenControl: this.nhomQuyenControl,
      phanQuyenDuLieuControl: this.phanQuyenDuLieuControl,
      phanQuyenChucNangControl: this.phanQuyenChucNangControl
    });

    this.cauHinhPhanQuyenForm.disable();
  }

  /* Nếu Input có thay đổi */
  ngOnChanges() {
    if (this.listOrganization?.length > 0)
      this.getCauHinhPhanQuyen();
      this.getThongTinChungThanhVien();
  }

  async getCauHinhPhanQuyen() {
    this.loading = true;
    let result: any = await this.employeeService.getCauHinhPhanQuyen(this.employeeId);
    this.loading = false;

    if (result.statusCode == 200) {
      this.listSelectedDonVi = result.listSelectedDonVi;
      this.mapDataToForm(result.roleId, result.isManager);
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  enabledForm() {
    this.isEdit = true;
    this.cauHinhPhanQuyenForm.enable();
  }

  async saveForm() {
    if (!this.cauHinhPhanQuyenForm.valid) {
      Object.keys(this.cauHinhPhanQuyenForm.controls).forEach(key => {
        if (this.cauHinhPhanQuyenForm.controls[key].valid == false) {
          this.cauHinhPhanQuyenForm.controls[key].markAsTouched();
        }
      });

      return;
    }

    if (this.listSelectedDonVi ?.length == 0) {
      this.showMessage('warn', 'Nhân viên phải thuộc ít nhất một đơn vị');
      return;
    }

    let isManager = this.phanQuyenDuLieuControl.value === 'true';
    let roleId = this.phanQuyenChucNangControl.value ?.roleId;

    this.loading = true;
    let result: any = await this.employeeService.saveCauHinhPhanQuyen(this.employeeId, isManager, roleId, this.listSelectedDonVi);
    this.loading = false;

    if (result.statusCode == 200) {
      this.isEdit = false;
      this.cauHinhPhanQuyenForm.disable();
      this.showMessage('success', result.messageCode);
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  disabledForm() {
    this.isEdit = false;
    this.cauHinhPhanQuyenForm.disable();
    this.getCauHinhPhanQuyen();
  }

  mapDataToModel() {

  }

  mapDataToForm(roleId: string, isManager: boolean) {
    //Phân quyền dữ liệu
    this.isManager = isManager.toString().toLowerCase();
    this.phanQuyenDuLieuControl.setValue(isManager.toString().toLowerCase());

    //Phân quyền chức năng
    let quyenChucNang = this.listRole.find(x => x.roleId == roleId);
    if (quyenChucNang)
      this.phanQuyenChucNangControl.setValue(quyenChucNang);
  }

  openOrgPopup() {
    let listSelectedId = this.listSelectedDonVi.map(item => item.organizationId);
    this.listSelectedDonViClone = this.listSelectedDonVi;

    let ref = this.dialogService.open(ChonNhieuDvDialogComponent, {
      data: {
        mode: 1, //1: Multiple choice, 2: Single choice
        listSelectedId: listSelectedId
      },
      header: 'Chọn đơn vị',
      width: '40%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "350px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result ?.length > 0) {
          let listSelectedId = result.map(x => x.organizationId);

          let listOld = this.listSelectedDonViClone.filter(x => listSelectedId.includes(x.organizationId));
          let listOldId = listOld.map(y => y.organizationId);
          let listNewId: Array<any> = listSelectedId.filter(x => !listOldId.includes(x));

          if (listNewId.length > 0) {
            listNewId.forEach(id => {
              let donVi = this.listOrganization.find(x => x.organizationId == id);
              let newItem = new ThanhVienPhongBan();
              newItem.organizationId = id;
              newItem.organizationName = donVi.organizationName;
              newItem.employeeId = this.employeeId;
              newItem.isManager = 0;

              listOld = [...listOld, newItem];
            });
          }

          this.listSelectedDonVi = listOld;
        }
        else {
          this.listSelectedDonVi = [];
        }
      }
    });
  }

  xoaDonVi(rowData) {
    if (this.listSelectedDonVi.length == 1) {
      this.showMessage('warn', 'Nhân viên phải thuộc ít nhất một đơn vị');
      return;
    }

    this.listSelectedDonVi = this.listSelectedDonVi.filter(x => x != rowData);
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

  async getThongTinChungThanhVien() {
    this.loading = true;
    let result: any = await this.employeeService.getThongTinChungThanhVien(this.employeeId);
    this.loading = false;

    if (result.statusCode == 200) {
      this.thongTinChung = result.thongTinChung;
    }
  }

}
