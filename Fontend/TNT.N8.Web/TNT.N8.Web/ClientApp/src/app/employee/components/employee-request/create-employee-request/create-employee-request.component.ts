import { QuyTrinhService } from './../../../../admin/services/quy-trinh.service';
import { FormatDateService } from './../../../../shared/services/formatDate.services';
import { ValidaytorsService } from './../../../../shared/services/validaytors.service';
import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { EmployeeRequestService } from '../../../services/employee-request/employee-request.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { MessageService } from "primeng/api";
import { EncrDecrService } from '../../../../shared/services/encrDecr.service';

@Component({
  selector: 'app-create-employee-request',
  templateUrl: './create-employee-request.component.html',
  styleUrls: ['./create-employee-request.component.css'],
})
export class CreateEmployeeRequestComponent implements OnInit {
  loading: boolean = false;
  awaitResponse: boolean = false;
  emptyGuid = '00000000-0000-0000-0000-000000000000';
  actionAdd: boolean = true;

  nowDate = new Date();
  employeeName: string = null;
  organizationName: string = null;
  positionName: string = null;

  dxxnForm: FormGroup;
  loaiDeXuatControl: FormControl;
  ngayNghiControl: FormControl;
  tuCaControl: FormControl;
  caControl: FormControl;
  denCaControl: FormControl;
  lyDoControl: FormControl;

  listKyHieuChamCong: Array<any> = [];
  listLoaiCaLamViec: Array<any> = [];

  isShowTuDen: boolean = true;
  tongNgayNghi: number = 0;
  soNgayPhepConLai: number = 0;

  constructor(
    private getPermission: GetPermission,
    private router: Router,
    private route: ActivatedRoute,
    private employeeRequestService: EmployeeRequestService,
    private messageService: MessageService,
    private validaytorsService: ValidaytorsService,
    private formatDateService: FormatDateService,
    private quyTrinhService: QuyTrinhService,
    private encrDecrService: EncrDecrService
  ) { }

  async ngOnInit() {
    this.setForm();

    let resource = "hrm/employee/request/create/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
      return;
    }

    let listCurrentActionResource = permission.listCurrentActionResource;
    if (listCurrentActionResource.indexOf("add") == -1) {
      this.actionAdd = false;
    }

    this.getMasterData();
  }

  setForm() {
    this.loaiDeXuatControl = new FormControl(null, [Validators.required]);
    this.ngayNghiControl = new FormControl(null, [Validators.required]);
    this.tuCaControl = new FormControl(null, [Validators.required]);
    this.denCaControl = new FormControl(null, [Validators.required]);
    this.lyDoControl = new FormControl(null, [Validators.required, this.validaytorsService.forbiddenSpaceText]);
    this.caControl = new FormControl(null, [Validators.required]);

    this.dxxnForm = new FormGroup({
      loaiDeXuatControl: this.loaiDeXuatControl,
      ngayNghiControl: this.ngayNghiControl,
      tuCaControl: this.tuCaControl,
      denCaControl: this.denCaControl,
      lyDoControl: this.lyDoControl,
      caControl: this.caControl
    });
  }

  async getMasterData() {
    this.loading = true;
    this.awaitResponse = true;
    let result: any = await this.employeeRequestService.getMasterCreateEmpRequest();
    this.loading = false;
    this.awaitResponse = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.listKyHieuChamCong = result.listKyHieuChamCong;
    this.listLoaiCaLamViec = result.listLoaiCaLamViec;
    this.employeeName = result.employeeName;
    this.organizationName = result.organizationName;
    this.positionName = result.positionName;
    this.soNgayPhepConLai = result.soNgayPhepConLai;
  }

  changeLoaiDeXuat() {
    let loaiDeXuat = this.loaiDeXuatControl.value;

    //Nếu loại đề xuất là Đi muộn hoặc Về sớm
    if (loaiDeXuat.value == 12 || loaiDeXuat.value == 13) {
      this.isShowTuDen = false;
      this.caControl.setValidators([Validators.required]);
      this.tuCaControl.setValidators([]);
      this.denCaControl.setValidators([]);
    }
    else {
      this.isShowTuDen = true;
      this.caControl.setValidators([]);
      this.tuCaControl.setValidators([Validators.required]);
      this.denCaControl.setValidators([Validators.required]);
    }

    this.caControl.updateValueAndValidity();
    this.tuCaControl.updateValueAndValidity();
    this.denCaControl.updateValueAndValidity();

    this.tinhSoNgayNghi();
  }

  tinhSoNgayNghi() {
    this.tongNgayNghi = 0;

    let loaiDeXuat = this.loaiDeXuatControl.value;
    let listDate: Array<Date> = this.ngayNghiControl.value;

    if (!loaiDeXuat) {
      return;
    }

    if (!listDate) {
      return;
    }

    //Sắp xếp lại list ngày theo thứ tự lớn dần
    listDate.sort(function (a, b) {
      return +(new Date(b)) - +(new Date(a));
    });
    listDate.reverse();

    //Nếu loại đề xuất là Đi muộn hoặc Về sớm
    if (loaiDeXuat.value == 12 || loaiDeXuat.value == 13) {
      if (!this.caControl.value) {
        return;
      }
      this.tongNgayNghi = listDate.length;
    }
    else {
      if (!this.tuCaControl.value || !this.denCaControl.value) {
        return;
      }

      //Nếu chỉ chọn 1 ngày
      if (listDate.length == 1) {
        if (this.tuCaControl.value.value > this.denCaControl.value.value) {
          return;
        }

        if (this.tuCaControl.value == this.denCaControl.value) {
          this.tongNgayNghi = 0.5;
        }
        else {
          this.tongNgayNghi = 1;
        }
      }
      //Nếu chọn nhiều hơn 1 ngày
      else if (listDate.length > 1) {
        for (let i = 0; i < listDate.length; i++) {
          //Nếu là ngày đầu tiên
          if (i == 0) {
            //Nếu bắt đầu từ Ca sáng
            if (this.tuCaControl.value.value == 1) this.tongNgayNghi = 1;
            //Nếu bắt đầu từ Ca chiều
            else if (this.tuCaControl.value.value == 2) this.tongNgayNghi = 0.5;
          }
          //Nếu là ngày cuối cùng
          else if (i == listDate.length - 1) {
            //Nếu kết thúc đến Ca sáng
            if (this.denCaControl.value.value == 1) this.tongNgayNghi += 0.5;
            //Nếu kết thúc đến Ca chiều
            else if (this.denCaControl.value.value == 2) this.tongNgayNghi += 1;
          }
          //Nếu không phải ngày đầu tiên hoặc ngày cuối cùng
          else {
            this.tongNgayNghi += 1;
          }
        }
      }
    }
  }

  async taoDeXuat(isSend: boolean) {
    if (!this.dxxnForm.valid) {
      Object.keys(this.dxxnForm.controls).forEach(key => {
        if (!this.dxxnForm.controls[key].valid) {
          this.dxxnForm.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    if (this.loaiDeXuatControl.value.value == 1 && this.tongNgayNghi > this.soNgayPhepConLai) {
      this.showMessage('warn', 'Bạn không có đủ số ngày nghỉ phép');
      return;
    }

    let listDate: Array<Date> = this.ngayNghiControl.value.map(item => {
      item = this.formatDateService.convertToUTCTime(item);
      return item;
    });

    let deXuatXinNghi = {
      loaiDeXuatId: this.loaiDeXuatControl.value.value,
      lyDo: this.lyDoControl.value,
      listDate: listDate,
      tuCa: this.tuCaControl.value ?.value,
      denCa: this.denCaControl.value ?.value,
      ca: this.caControl.value ?.value
    };

    this.loading = true;
    this.awaitResponse = true;
    let result: any = await this.employeeRequestService.createEmployeeRequest(deXuatXinNghi, false);
    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      this.awaitResponse = false;
      return;
    }

    //Nếu tạo mới
    if (!isSend) {
      this.showMessage("success", result.messageCode);

      setTimeout(() => {
        this.router.navigate(['/employee/request/detail', { deXuatXinNghiId: this.encrDecrService.set(result.deXuatXinNghiId) }]);
      }, 1500);
    }
    //Nếu tạo mới và gửi phê duyệt
    else {
      this.quyTrinhService.guiPheDuyet(this.emptyGuid, 9, result.deXuatXinNghiId).subscribe(async res => {
        let _result: any = res;

        //Nếu gửi phê duyệt thất bại
        if (_result.statusCode != 200) {
          //Xóa đề xuất mới tạo
          let resultDelete: any = await this.employeeRequestService.deleteDeXuatXinNghiById(result.deXuatXinNghiId);
          this.loading = false;
          this.awaitResponse = false;

          if (resultDelete.statusCode != 200) {
            this.showMessage("error", resultDelete.messageCode);
            return;
          }

          this.showMessage("error", _result.messageCode);
          return;
        }

        this.showMessage("success", _result.messageCode);

        setTimeout(() => {
          this.router.navigate(['/employee/request/detail', { deXuatXinNghiId: this.encrDecrService.set(result.deXuatXinNghiId) }]);
        }, 1500);
      });
    }
  }

  back() {
    this.router.navigate(['/employee/request/list']);
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }
}
