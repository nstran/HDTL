import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { EncrDecrService } from '../../../../shared/services/encrDecr.service';
import { ExportFileWordService } from '../../../../shared/services/exportFileWord.services';
import { SalaryService } from '../../../services/salary.service';
import { MessageService, ConfirmationService } from 'primeng/api';
import { DecimalPipe } from '@angular/common';
import { GetPermission } from '../../../../shared/permission/get-permission';

@Component({
  selector: 'app-phieu-luong-detail',
  templateUrl: './phieu-luong-detail.component.html',
  styleUrls: ['./phieu-luong-detail.component.css'],
  providers: [DecimalPipe]
})
export class PhieuLuongDetailComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;

  phieuLuongId: number = 0;
  data: any = null;
  titleName: Array<any> = [];
  engTitleName: Array<any> = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    public exportFileWordService: ExportFileWordService,
    private getPermission: GetPermission,
    private encrDecrService: EncrDecrService,
    private salaryService: SalaryService,
    private messageService: MessageService,
    private decimalPipe: DecimalPipe
  ) { }

  async ngOnInit() {
    this.route.params.subscribe(params => {
      this.phieuLuongId = Number(this.encrDecrService.get(params['phieuLuongId']));
    });

    await this._getPermission();

    this.getMasterData();
  }

  async _getPermission() {
    let resource = "salary/salary/phieu-luong-detail/";
    this.loading = true;
    let permission: any = await this.getPermission.getPermission(resource);
    this.loading = false;

    if (permission.status == false) {
      this.router.navigate(["/home"]);
      return;
    }
  }

  async getMasterData() {
    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.getPhieuLuongById(this.phieuLuongId);
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.data = result.phieuLuong;
    this.initTable();
  };

  initTable() {
    this.titleName = [
      'Nhân viên-Học viên/',
      'Mã lưu hồ sơ/',
      'Email:',
      'Ngày tiêu chuẩn/',
      `Lương cơ bản trước điều chỉnh/`,
      `Lương cơ bản sau điều chỉnh (nếu có)/`,
      'Mức điều chỉnh/',
      'Số ngày thực tế làm việc/ học việc trong tháng/',
      'Số ngày nghỉ phép trong tháng/',
      'Số ngày nghỉ lễ/ Nghỉ hưởng nguyên lương trong tháng/',
      'Số ngày nghỉ không lương (Có phép)/',
      'Số ngày trừ đi muộn/ về sớm không làm đủ công/',
      'Số ngày không được tính thưởng chuyên cần/',
      'Bottom 10% - 10% KPI thấp nhất công ty không được hỗ trợ ĐT, xăng, nhà ở',
      `Số lượng người đăng ký giảm trừ gia cảnh (${this.decimalPipe.transform(this.data?.cauHinhGiamTruNguoiPhuThuoc)} triệu/ người)/`,
      `Giảm trừ gia cảnh bản thân & người thân (${this.decimalPipe.transform(this.data?.cauHinhGiamTruCaNhan)} triệu/ NV)/`,
      'I. CHI TIẾT THU NHẬP TRƯỚC THUẾ_VND/',
      '1. Lương theo ngày làm việc-học việc tạm tính trong tháng/',
      '2. Hỗ trợ xăng xe (Chỉ tính ngày đi làm), điều chỉnh/',
      '3. Hỗ trợ điện thoại (Chỉ tính ngày đi làm), điều chỉnh/',
      '4. Hỗ trợ ăn trưa (Tính theo ngày làm thực tế), điều chỉnh/',
      '5. Hỗ trợ nhà ở (Chỉ tính ngày đi làm), điều chỉnh/',
      '6. Thưởng chuyên cần (Không tính ngày nghỉ, muộn/sớm), điều chỉnh/',
      `7. Thưởng KPI tháng ${this.data?.thangTruoc} năm ${this.data?.namTheoThangTruoc}/`,
      `8. Thưởng thành tích cuối năm/`,
      '9. Phụ cấp trách nhiệm (nếu có)/',
      '10. Trợ cấp học việc (nếu có)/',
      '11.1. OT tính thuế TNCN/',
      '11.2. OT không tính thuế TNCN/',
      `12. Các khoản thu nhập tính thuế TNCN tháng ${this.data?.thangKetThucKyLuong}.${this.data?.namKetThucKyLuong} không cộng vào lương/`,
      '- Lương tháng 13/',
      '- Quà bốc thăm và các thu nhập tính thuế đã chỉ bằng tiền mặt hoặc hiện vật/',
      'II. CÁC KHOẢN TRỪ_VND/',
      '1. Tổng Thuế TNCN trong tháng/',
      `2. Bảo hiểm Xã hội, Y tế và Thất nghiệp (${this.decimalPipe.transform(this.data?.phanTramBaoHiemNld)}%)/`,
      'III.THU NHẬP THỰC NHẬN/',
      'IV. CÁC KHOẢN CÔNG TY PHẢI TRẢ/',
      `1. Bảo hiểm Xã hội, Y tế và Thất nghiệp (${this.decimalPipe.transform(this.data?.phanTramBaoHiemCty)}%)/`,
      `2. Kinh phí công đoàn ${this.decimalPipe.transform(this.data?.phanTramKinhPhiCongDoanCty)}% tháng này/`,
      'V. TỔNG CHI PHÍ NHÂN VIÊN-HỌC VIÊN/',
    ];

    this.engTitleName = [
      'Employee \'s name',
      'HR filing code',
      null,
      'Standard days',
      `Contract gross income`,
      `Contract gross income (if any)`,
      'Balance',
      'Working day of the month',
      'AL of the month',
      'Other paid leave',
      'Unpaid leave',
      'Leave early and come late days w/o pay',
      'Deduction days for attendance',
      null,
      'No. of dependent',
      'Deduction for family circumstance',
      'DETAIL GROSS INCOME',
      'GROSS income on working days',
      'Gasoline supporting fee and adjust if any',
      'Gell phone allowance and adjust if any',
      'Lunch allowance and adjust if any',
      'Housing allowance and adjust if any',
      'Bonus for compliance of regulation',
      `Bonus KPI in ${this.data?.thangTruocTiengAnh} ${this.data?.namTheoThangTruoc}`,
      `Year End rewards`,
      'Responsibility allowance (If any)',
      'Apprenticeship allowance (If any)',
      'Taxable income O.T',
      'Tax exempt O.T',
      'Other add to calculated',
      '13th monthy salary',
      'Tet gifts, Cash gift receiving during year-end meeting, Cash Tet bonus',
      'TOTAL DEDUCTION',
      'PIT',
      `SI, HI, UI (${this.decimalPipe.transform(this.data?.phanTramBaoHiemNld)}%)`,
      'NET INCOME TRANSFER TO EMPLOYEE',
      'CONTRIBUTION BY COMPANY',
      `SI, HI, UI (${this.decimalPipe.transform(this.data?.phanTramBaoHiemCty)}% from Jul 2017)`,
      `Union fee ${this.decimalPipe.transform(this.data?.phanTramKinhPhiCongDoanCty)}%`,
      'TOTAL DIRECT COST TO EMPLOYEE',
    ];
  }

  goBackToList() {
    this.router.navigate(['/salary/phieu-luong-list']);
  };

  exportFile() {
    let data = {
      template: 4,
      data: Object.assign({}, this.data, {
        baoHiem: this.decimalPipe.transform(this.data.baoHiem),
        cauHinhGiamTruCaNhan: this.decimalPipe.transform(this.data.cauHinhGiamTruCaNhan),
        cauHinhGiamTruNguoiPhuThuoc: this.decimalPipe.transform(this.data.cauHinhGiamTruNguoiPhuThuoc),
        cauHinhTroCapCc: this.decimalPipe.transform(this.data.cauHinhTroCapCc),
        cauHinhTroCapDmvs: this.decimalPipe.transform(this.data.cauHinhTroCapDmvs),
        ctyTraBh: this.decimalPipe.transform(this.data.ctyTraBh),
        giamTruGiaCanh: this.decimalPipe.transform(this.data.giamTruGiaCanh),
        kinhPhiCongDoan: this.decimalPipe.transform(this.data.kinhPhiCongDoan),
        luongCoBan: this.decimalPipe.transform(this.data.luongCoBan),
        luongCoBanSau: this.decimalPipe.transform(this.data.luongCoBanSau),
        luongThang13: this.decimalPipe.transform(this.data.luongThang13),
        luongTheoNgayHocViec: this.decimalPipe.transform(this.data.luongTheoNgayHocViec),
        mucDieuChinh: this.decimalPipe.transform(this.data.mucDieuChinh),
        ngayDmvs: this.decimalPipe.transform(this.data.ngayDmvs),
        ngayKhongHuongChuyenCan: this.decimalPipe.transform(this.data.ngayKhongHuongChuyenCan),
        ngayLamViecThucTe: this.decimalPipe.transform(this.data.ngayLamViecThucTe),
        ngayNghiKhongLuong: this.decimalPipe.transform(this.data.ngayNghiKhongLuong),
        ngayNghiLe: this.decimalPipe.transform(this.data.ngayNghiLe),
        ngayNghiPhep: this.decimalPipe.transform(this.data.ngayNghiPhep),
        nghiKhongPhep: this.decimalPipe.transform(this.data.nghiKhongPhep),
        otKhongTinhThue: this.decimalPipe.transform(this.data.otKhongTinhThue),
        otTinhThue: this.decimalPipe.transform(this.data.otTinhThue),
        phanTramBaoHiemCty: this.decimalPipe.transform(this.data.phanTramBaoHiemCty),
        phanTramBaoHiemNld: this.decimalPipe.transform(this.data.phanTramBaoHiemNld),
        phanTramKinhPhiCongDoanCty: this.decimalPipe.transform(this.data.phanTramKinhPhiCongDoanCty),
        quaBocTham: this.decimalPipe.transform(this.data.quaBocTham),
        soLanTruChuyenCan: this.decimalPipe.transform(this.data.soLanTruChuyenCan),
        soLuongDkGiamTruGiaCanh: this.decimalPipe.transform(this.data.soLuongDkGiamTruGiaCanh),
        soNgayLamViec: this.decimalPipe.transform(this.data.soNgayLamViec),
        thucNhan: this.decimalPipe.transform(this.data.thucNhan),
        thuongKpi: this.decimalPipe.transform(this.data.thuongKpi),
        tongChiPhiNhanVien: this.decimalPipe.transform(this.data.tongChiPhiNhanVien),
        tongSoNgayKhongTinhLuong: this.decimalPipe.transform(this.data.tongSoNgayKhongTinhLuong),
        tongThueTncn: this.decimalPipe.transform(this.data.tongThueTncn),
        troCapAnTrua: this.decimalPipe.transform(this.data.troCapAnTrua),
        troCapCcncTheoNgayLvtt: this.decimalPipe.transform(this.data.troCapCcncTheoNgayLvtt),
        troCapChuyenCan: this.decimalPipe.transform(this.data.troCapChuyenCan),
        troCapChuyenCanDmvs: this.decimalPipe.transform(this.data.troCapChuyenCanDmvs),
        troCapDiLai: this.decimalPipe.transform(this.data.troCapDiLai),
        troCapDienThoai: this.decimalPipe.transform(this.data.troCapDienThoai),
        troCapHocViec: this.decimalPipe.transform(this.data.troCapHocViec),
        troCapNhaO: this.decimalPipe.transform(this.data.troCapNhaO),
        troCapTrachNhiem: this.decimalPipe.transform(this.data.troCapTrachNhiem),
      })
    };
    this.exportFileWordService.saveFileWord(data, `Phiếu Lương - ${this.data.employeeCode}_${this.data.employeeName}.docx`)
  };

  numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
  };

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }
}
