import { FormGroup, FormControl, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { Component, OnInit, Input, SimpleChanges } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { EmployeeService } from './../../../../services/employee.service';

class ThongTinLuongVaTroCap {
  employeeId: string;
  listEmployeeSalary: Array<any>;
  employeeSalaryBase: number;
  effectiveDate: Date;
  lunchAllowance: number;
  maternityAllowance: number;
  fuelAllowance: number;
  phoneAllowance: number;
  otherAllownce: number;
  socialInsuranceSalary: number;
  socialInsuranceSupportPercent: number;
  socialInsurancePercent: number;
  healthInsuranceSupportPercent: number;
  unemploymentinsuranceSupportPercent: number;
  healthInsurancePercent: number;
  unemploymentinsurancePercent: number;
  chiPhiTheoGio: number;
}

@Component({
  selector: 'app-luong-va-tro-cap',
  templateUrl: './luong-va-tro-cap.component.html',
  styleUrls: ['./luong-va-tro-cap.component.css']
})
export class LuongVaTroCapComponent implements OnInit {
  loading: boolean = false;
  isEdit: boolean = false;
  @Input() actionEdit: boolean;
  @Input() employeeId: string;

  thongTinLuongVaTroCap = new ThongTinLuongVaTroCap();

  thongTinLuongVaTroCapForm: FormGroup;

  luongHienTai: number;
  mucLuongKyHopDong: number;

  display: boolean = false;
  cols: Array<any> = [];
  listTroCap = [];

  constructor(
    public messageService: MessageService,
    public employeeService: EmployeeService,
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.initTable();
  }

  initForm() {
    this.thongTinLuongVaTroCapForm = new FormGroup({
      luongHienTai: new FormControl(null),
      mucLuongKyHopDong: new FormControl(null)
    });

    this.thongTinLuongVaTroCapForm.disable();
  }

  initTable() {
    this.cols = [
      { field: 'tenTroCap', header: 'Tên trợ cấp', textAlign: 'left' },
      { field: 'mucTroCap', header: 'Mức trợ cấp (VNĐ)', textAlign: 'right' },
    ];
  }

  /* Nếu Input có thay đổi */
  ngOnChanges(changes: SimpleChanges) {
    if (this.actionEdit)
      this.getThongTinLuongVaTroCap();
  }

  async getThongTinLuongVaTroCap() {
    this.loading = true;
    let result: any = await this.employeeService.getThongTinLuongVaTroCap(this.employeeId);
    this.loading = false;

    if (result.statusCode == 200) {
      this.luongHienTai = result.luongHienTai;
      this.mucLuongKyHopDong = result.mucLuongKyHopDong;
      this.listTroCap = result.listTroCap;

      this.setForm();

    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  setForm() {
    this.thongTinLuongVaTroCapForm.setValue({
      luongHienTai: this.luongHienTai ?? 0,
      mucLuongKyHopDong: this.mucLuongKyHopDong ?? 0
    });
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}