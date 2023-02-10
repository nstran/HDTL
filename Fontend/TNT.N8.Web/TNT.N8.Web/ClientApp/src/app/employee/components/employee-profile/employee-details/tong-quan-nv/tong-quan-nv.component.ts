import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { EmployeeService } from './../../../../services/employee.service';
import { GetPermission } from './../../../../../shared/permission/get-permission';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Router, ActivatedRoute } from '@angular/router';
import { NumberToStringPipe } from '../../../../../shared/ConvertMoneyToString/numberToString.pipe';
// import { ExportWordService } from '../../../../../../app/shared/services/exportWord.service';

class ThongTinCaNhan {
  employeeId: string;
  avatarUrl: string;
  employeeCode: string;
  gender: string;
  firstName: string;
  lastName: string;
  employeeName: string;
  dateOfBirth: Date;
  phone: string;
  email: string;
  address: string;
  soBuoiNghiCoPhep: number;
  soBuoiNghiKhongPhep: number;
  soBuoiPhepNam: number;
  soBuoiPhepCon: number;
}

@Component({
  selector: 'app-tong-quan-nv',
  templateUrl: './tong-quan-nv.component.html',
  styleUrls: ['./tong-quan-nv.component.css']
})
export class TongQuanNvComponent implements OnInit {
  loading: boolean = false;
  employeeId: string = null;
  tabIndex: number = 0;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionEdit: boolean = true;
  actionImport: boolean = true;
  actionDelete: boolean = true;
  isAllowReset: boolean = true;

  listGioiTinh = [
    { code: 'NAM', name: 'Nam' },
    { code: 'NU', name: 'Nữ' },
  ];
  listOrganization: Array<any> = [];
  listPosition: Array<any> = [];
  listRole: Array<any> = [];
  listLoaiHopDong: Array<any> = [];
  listQuocGia: Array<any> = [];
  listNganHang: Array<any> = [];

  thongTinCaNhan = new ThongTinCaNhan();
  thongTinCaNhanClone = new ThongTinCaNhan();
  isEdit: boolean = false;

  thongTinCaNhanForm: FormGroup;
  displayContractTypeDialog: boolean = false;

  // defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));

  maNhanVienControl: FormControl;
  ngaySinhControl: FormControl;
  soDienThoaiControl: FormControl;
  emailControl: FormControl;
  diaChiControl: FormControl;
  hoTenDemControl: FormControl;
  tenControl: FormControl;
  gioiTinhControl: FormControl;

  constructor(
    public location: Location,
    public employeeService: EmployeeService,
    public getPermission: GetPermission,
    public messageService: MessageService,
    public confirmationService: ConfirmationService,
    public router: Router,
    public route: ActivatedRoute,
    // private exportWordService: ExportWordService,
  ) { }

  async ngOnInit() {
    // this.initForm();
    let resource = "hrm/employee/detail/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("import") == -1) {
        this.actionImport = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      if (listCurrentActionResource.indexOf("re-pass") == -1) {
        this.isAllowReset = false;
      }

      this.route.params.subscribe(params => {
        this.employeeId = params['employeeId'];
      });

      this.getMasterData();
    }
  }

  initForm() {
    this.maNhanVienControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.ngaySinhControl = new FormControl(null);
    this.soDienThoaiControl = new FormControl(null);
    this.emailControl = new FormControl(null, [Validators.required, Validators.email, forbiddenSpaceText]);
    this.diaChiControl = new FormControl(null);
    this.hoTenDemControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.tenControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.gioiTinhControl = new FormControl(null);

    this.thongTinCaNhanForm = new FormGroup({
      maNhanVienControl: this.maNhanVienControl,
      ngaySinhControl: this.ngaySinhControl,
      soDienThoaiControl: this.soDienThoaiControl,
      emailControl: this.emailControl,
      diaChiControl: this.diaChiControl,
      hoTenDemControl: this.hoTenDemControl,
      tenControl: this.tenControl,
      gioiTinhControl: this.gioiTinhControl
    });

    this.thongTinCaNhanForm.disable();
  }

  // getDefaultNumberType() {
  //   return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  // }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.employeeService.getMasterDataEmployeeDetail();
    this.loading = false;

    if (result.statusCode == 200) {
      this.listOrganization = result.listOrganization;
      this.listPosition = result.listPosition;
      this.listRole = result.listRole;
      this.listLoaiHopDong = result.listLoaiHopDong;
      this.listQuocGia = result.listQuocGia;
      this.listNganHang = result.listNganHang;

      // this.getThongTinCaNhanThanhVien();
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  async getThongTinCaNhanThanhVien() {
    this.loading = true;
    let result: any = await this.employeeService.getThongTinCaNhanThanhVien(this.employeeId);
    this.loading = false;

    if (result.statusCode == 200) {
      this.thongTinCaNhan = result.thongTinCaNhan;
      this.thongTinCaNhanClone = result.thongTinCaNhan;

      this.mapDataToForm(this.thongTinCaNhan);
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  enabledForm() {
    this.isEdit = true;
    this.thongTinCaNhanForm.enable();
  }

  async saveForm() {
    if (!this.thongTinCaNhanForm.valid) {
      Object.keys(this.thongTinCaNhanForm.controls).forEach(key => {
        if (this.thongTinCaNhanForm.controls[key].valid == false) {
          this.thongTinCaNhanForm.controls[key].markAsTouched();
        }
      });

      return;
    }

    let thongTinCaNhan = this.mapDataToModel();
    this.loading = true;
    let result: any = await this.employeeService.saveThongTinCaNhanThanhVien(thongTinCaNhan);
    this.loading = false;

    if (result.statusCode == 200) {
      this.isEdit = false;
      this.thongTinCaNhanForm.disable();
      this.showMessage('success', result.messageCode);
      this.getThongTinCaNhanThanhVien();
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  disabledForm() {
    this.isEdit = false;
    this.mapDataToForm(this.thongTinCaNhanClone);
    this.thongTinCaNhanForm.disable();
  }

  mapDataToModel() {
    let result = new ThongTinCaNhan();

    result.employeeCode = this.maNhanVienControl.value ?.trim();
    result.employeeId = this.employeeId;
    result.firstName = this.hoTenDemControl.value ?.trim();
    result.lastName = this.tenControl.value ?.trim();
    result.gender = this.gioiTinhControl.value ?.code;
    result.dateOfBirth = convertToUTCTime(this.ngaySinhControl.value);
    result.phone = this.soDienThoaiControl.value;
    result.email = this.emailControl.value;
    result.address = this.diaChiControl.value;

    return result;
  }

  mapDataToForm(thongTinCaNhan: ThongTinCaNhan) {
    this.maNhanVienControl.setValue(thongTinCaNhan.employeeCode);
    this.hoTenDemControl.setValue(thongTinCaNhan.firstName);
    this.tenControl.setValue(thongTinCaNhan.lastName);

    let gioiTinh = this.listGioiTinh.find(x => x.code == thongTinCaNhan.gender);
    if (gioiTinh)
      this.gioiTinhControl.setValue(gioiTinh);
    else
      this.gioiTinhControl.setValue(null);

    this.ngaySinhControl.setValue(thongTinCaNhan.dateOfBirth ? new Date(thongTinCaNhan.dateOfBirth) : null);
    this.soDienThoaiControl.setValue(thongTinCaNhan.phone);
    this.emailControl.setValue(thongTinCaNhan.email);
    this.diaChiControl.setValue(thongTinCaNhan.address);
  }

  goBack() {
    this.location.back();
  }

  datLaiMatKhau() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn reset mật khẩu cho người dùng này không?',
      accept: async () => {
        this.loading = true;
        let result: any = await this.employeeService.resetPassword(this.employeeId);
        this.loading = false;

        if (result.statusCode == 200) {
          this.showMessage('success', result.messageCode);
        }
        else {
          this.showMessage('error', result.messageCode);
        }
      }
    });
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

  chooseContractType() {
    this.displayContractTypeDialog = true;
  }

  //xuất hợp đồng lao động
  async exportLaborContract(type: number) {

    // từ ngày dến ngày của loại hợp đồng
    const moneyPipe = new NumberToStringPipe();
    let [getThongTinChungThanhVien, getThongTinCaNhanThanhVien, cauHinhPhanQuyen, thongTinNhanSu, luongVaTroCap, thongTinDanhGia]: any = await Promise.all([
      this.employeeService.getThongTinChungThanhVien(this.employeeId),
      this.employeeService.getThongTinCaNhanThanhVien(this.employeeId),
      this.employeeService.getCauHinhPhanQuyen(this.employeeId),
      this.employeeService.getThongTinNhanSu(this.employeeId),
      this.employeeService.getThongTinLuongVaTroCap(this.employeeId),
      this.employeeService.getThongTinGhiChu(this.employeeId),
    ]);

    let thongTin0 = getThongTinChungThanhVien.thongTinChung
    let thongTin1 = getThongTinCaNhanThanhVien.thongTinCaNhan
    let thongTin2 = cauHinhPhanQuyen
    let thongTin3 = thongTinNhanSu.thongTinNhanSu
    let thongTin4 = luongVaTroCap.thongTinLuongVaTroCap
    let thongTin5 = thongTinDanhGia.thongTinDanhGia
    let capBac = this.listPosition.find(x => x.positionId == thongTin0.positionId);
    let loaiHD = this.listLoaiHopDong.find(x => x.categoryId == thongTin0.contractType);
    let quocGia = this.listQuocGia.find(x => x.countryId == thongTin3.countryId);
    let ngayCapCmtNV = thongTin3.identityIddateOfIssue ? new Date(thongTin3.identityIddateOfIssue) : null;
    let ngaySinhNV = thongTin1.dateOfBirth ? new Date(thongTin1.dateOfBirth) : null;

    //lấy thời hạn HD dựa vào loại hợp đồng
    let tuNgayDate
    let denNgayDate
    if (loaiHD !== undefined) {
      this.loading = true;
      if (loaiHD.categoryCode == 'TTA') { // thực tập
        tuNgayDate = new Date(thongTin0.trainingStartDate);
        let temp = new Date(thongTin0.trainingStartDate);
        denNgayDate = new Date(temp.setMonth(temp.getMonth() + 2));
      }
      if (loaiHD.categoryCode == 'TVI') { //thử việc
        tuNgayDate = new Date(thongTin0.probationStartDate);
        denNgayDate = new Date(thongTin0.probationEndDate);
      }
      if (loaiHD.categoryCode != 'TTA' && loaiHD.categoryCode != 'TVI') { // Hợp đồng có thời hạn và không thời hạn
        tuNgayDate = new Date(thongTin0.startedDate);
        let temp = new Date(thongTin0.startedDate);
        if (loaiHD.categoryCode == 'STH') denNgayDate = new Date(temp.setMonth(temp.getMonth() + 6)); // 6 tháng
        if (loaiHD.categoryCode == 'MNA') denNgayDate = new Date(temp.setMonth(temp.getMonth() + 12)); // 1 năm
        if (loaiHD.categoryCode == 'HNA') denNgayDate = new Date(temp.setMonth(temp.getMonth() + 24)); // 2 năm
        if (loaiHD.categoryCode == 'BNA') denNgayDate = new Date(temp.setMonth(temp.getMonth() + 36)); // 3 năm
        if (loaiHD.categoryCode == 'KHA') denNgayDate = new Date(temp.setMonth(temp.getMonth() + 2)); // Không thời hạn
      }
      let keTuNgay = tuNgayDate.getDate() + ' tháng ' + (tuNgayDate.getMonth() + 1) + ' năm ' + tuNgayDate.getFullYear();
      if (loaiHD.categoryCode != 'KHA') keTuNgay = keTuNgay + ' đến ngày ' + denNgayDate.getDate() + ' tháng ' + (denNgayDate.getMonth() + 1) + ' năm ' + denNgayDate.getFullYear() + ' ';
      //Mau_HTN_HDLD: template Number 5
      //Mau_HDLD_hhb: template Number 6
      //HDLD_GreenKa: template Number 7
      let now = new Date();
      let body = {
        'tenNV': thongTin1.employeeName ? thongTin1.employeeName : '',
        'ngaySinhNV': ngaySinhNV != null ? ngaySinhNV.toLocaleDateString('en-GB', { month: '2-digit', day: '2-digit', year: 'numeric' }).toString() : '',
        'diaChiNV': thongTin1.address ? thongTin1.address : '',
        'quocTichNV': quocGia ? quocGia.countryName : '',
        'cmtHoChieuNV': thongTin3.identityId ? thongTin3.identityId : '',
        'ngayCapCmtNV': ngayCapCmtNV != null ? ngayCapCmtNV.toLocaleDateString('en-GB', { month: '2-digit', day: '2-digit', year: 'numeric' }).toString() : '',
        'noiCapCmtNV': thongTin3.identityIdplaceOfIssue ? thongTin3.identityIdplaceOfIssue : '',
        'loaiHD': loaiHD ? loaiHD.categoryName : '',
        'capBacNV': capBac ? capBac.positionName : '',
        'luongChinhNV': thongTin4.employeeSalaryBase ? thongTin4.employeeSalaryBase.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,') : '',
        // 'luong9NVChu': thongTin4.employeeSalaryBase ? moneyPipe.transform(parseFloat(thongTin4.employeeSalaryBase.toString()), this.defaultNumberType) : '',
        'keTuNgay': keTuNgay,
        'ngay': now.getDate(),
        'thang': now.getMonth() + 1,
        'nam': now.getFullYear(),
        'diaDiemLamViec': thongTin0.address ? thongTin0.address : ''
      };

      //chọn loại template theo phòng ban
      //if(thongTin0.organizationCode == 'BGD')
      body['templateNumber'] = type;//5;
      // if(thongTin0.organizationCode == 'PKD1')
      body['templateNumber'] = type; //6;
      //if(thongTin0.organizationCode == 'PKD2')
      body['templateNumber'] = type; //7;

      let nameFile = 'Hợp đồng lao động_' + thongTin1.employeeName
      this.displayContractTypeDialog = false;
      this.loading = false;
      // this.exportWordService.saveFileWord(body, nameFile);
    }
    else {
      this.loading = false;
      this.displayContractTypeDialog = false;
      this.showMessage('warn', "Vui lòng cập nhập loại hợp đồng cho nhân viên trước khi xuất.");
    }
  }
}



function convertToUTCTime(time: any) {
  if (time)
    return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
  else
    return null;
};

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