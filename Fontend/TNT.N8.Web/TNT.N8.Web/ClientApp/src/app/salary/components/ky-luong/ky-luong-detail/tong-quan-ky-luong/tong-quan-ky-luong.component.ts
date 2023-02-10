import { QuyTrinhService } from './../../../../../admin/services/quy-trinh.service';
import { FormatDateService } from './../../../../../shared/services/formatDate.services';
import { ValidaytorsService } from './../../../../../shared/services/validaytors.service';
import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../../../shared/permission/get-permission';
import { MessageService, ConfirmationService } from "primeng/api";
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { EncrDecrService } from '../../../../../shared/services/encrDecr.service';
import { SalaryService } from '../../../../services/salary.service';
import { KyLuong } from '../../../../models/ky-luong.model';
import { CommonService } from '../../../../../shared/services/common.service';
import { Table } from 'primeng/table';
import { DialogService } from 'primeng/dynamicdialog';
import { UpdateDieuKienTroCapCoDinhComponent } from '../update-dieu-kien-tro-cap-co-dinh/update-dieu-kien-tro-cap-co-dinh.component';
import { UpdateDieuKienTroCapKhacComponent } from '../update-dieu-kien-tro-cap-khac/update-dieu-kien-tro-cap-khac.component';
import * as XLSX from 'xlsx';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";

class columnModel {
  column1: string;
  column2: string;
}

@Component({
  selector: 'app-tong-quan-ky-luong',
  templateUrl: './tong-quan-ky-luong.component.html',
  styleUrls: ['./tong-quan-ky-luong.component.css']
})
export class TongQuanKyLuongComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;
  emptyGuid = '00000000-0000-0000-0000-000000000000';
  isEdit: boolean = false;

  actionEdit: boolean = false;
  actionDelete: boolean = false;
  actionComplete: boolean = false;

  kyLuongId: number = 0;
  kyLuong: KyLuong = null;
  kyLuongClone: KyLuong = null;
  listDuLieuQuyTrinh: Array<any> = [];

  @ViewChild('myTable1') myTable1: Table;
  @ViewChild('myTable2') myTable2: Table;
  @ViewChild('myTable3') myTable3: Table;
  @ViewChild('myTable4') myTable4: Table;
  @ViewChild('myTable5') myTable5: Table;
  @ViewChild('myTable6') myTable6: Table;
  @ViewChild('myTable7') myTable7: Table;
  @ViewChild('myTable8') myTable8: Table;
  @ViewChild('myTable9') myTable9: Table;
  @ViewChild('myTable10') myTable10: Table;
  @ViewChild('myTable11') myTable11: Table;
  @ViewChild('myTable12') myTable12: Table;

  frozenCols1: Array<any> = [];
  frozenCols2: Array<any> = [];
  frozenCols3: Array<any> = [];
  frozenCols4: Array<any> = [];
  frozenCols5: Array<any> = [];
  frozenCols6: Array<any> = [];
  frozenCols7: Array<any> = [];
  frozenCols8: Array<any> = [];
  frozenCols9: Array<any> = [];
  frozenCols10: Array<any> = [];
  frozenCols11: Array<any> = [];
  frozenCols12: Array<any> = [];

  cols1: Array<any> = [];
  cols2: Array<any> = [];
  cols3: Array<any> = []; //Trợ cấp cố định
  cols4: Array<any> = []; //Trợ cấp khác
  cols5: Array<any> = []; //Trợ cấp OT
  cols6: Array<any> = []; //Thu nhập chỉ đưa vào tính thuế
  cols7: Array<any> = []; //Bảo hiểm
  cols8: Array<any> = []; //Giảm trừ trước thuế
  cols9: Array<any> = []; //Giảm trừ sau thuế
  cols10: Array<any> = []; //Hoàn lại sau thuế
  cols11: Array<any> = []; //Cty đóng
  cols12: Array<any> = []; //Khác

  listTongHopChamCong: Array<any> = [];
  listLuongTongHop: Array<any> = [];
  listLuongCtThuNhapTinhThue: Array<any> = [];
  listLuongCtBaoHiem: Array<any> = [];
  listLuongCtGiamTruTruocThue: Array<any> = [];
  listLuongCtGiamTruSauThue: Array<any> = [];
  listLuongCtHoanLaiSauThue: Array<any> = [];
  listLuongCtCtyDong: Array<any> = [];
  listLuongCtOther: Array<any> = [];

  trRowTroCapOt: Array<any> = [];
  trRowTroCapOtFrozen: Array<any> = [];
  listDataTroCapOt: Array<any> = [];

  trRowTroCapCoDinh: Array<any> = [];
  trRowTroCapCoDinhFrozen: Array<any> = [];
  listDataTroCapCoDinh: Array<any> = [];

  trRowTroCapKhac: Array<any> = [];
  trRowTroCapKhacFrozen: Array<any> = [];
  listDataTroCapKhac: Array<any> = [];

  listLoaiTroCapKhac: Array<any> = [];
  listSelectedLoaiTroCapKhac: Array<any> = [];

  isShowSua: boolean = false;
  isShowXoa: boolean = false;
  isShowGuiPheDuyet: boolean = false;
  isShowTuChoi: boolean = false;
  isShowPheDuyet: boolean = false;
  isShowDatVeMoi: boolean = false;

  kyluongForm: FormGroup;
  tenKyLuongControl: FormControl;
  soNgayLamViecControl: FormControl;
  tuNgayControl: FormControl;
  denNgayControl: FormControl;

  showLyDoTuChoi: boolean = false;

  lyDoTuChoiForm: FormGroup;
  lyDoTuChoiControl: FormControl;

  refreshNote = 0;
  tabIndex = 0;

  @ViewChild('attachment') attachment: any;
  fileExcelImport: any = null;
  displayDialogImport: boolean = false;
  fileName: string = '';

  typeImport = 1; //1: import trợ cấp khác; 2: import lương chi tiết
  displayErrorDialog: boolean = false;
  messEmpHeThong: string = null;
  messEmpBangLuong: string = null;
  messLoaiTroCap: string = null;

  constructor(
    private getPermission: GetPermission,
    private router: Router,
    private route: ActivatedRoute,
    private salaryService: SalaryService,
    private messageService: MessageService,
    private validaytorsService: ValidaytorsService,
    private formatDateService: FormatDateService,
    private confirmationService: ConfirmationService,
    private quyTrinhService: QuyTrinhService,
    private encrDecrService: EncrDecrService,
    private commonService: CommonService,
    private dialogService: DialogService
  ) { }

  async ngOnInit() {
    this.setForm();
    this.initTable();

    this.route.params.subscribe(params => {
      this.kyLuongId = Number(this.encrDecrService.get(params['kyLuongId']));
    });

    await this._getPermission();

    this.getData();
  }

  setForm() {
    this.tenKyLuongControl = new FormControl(null, [Validators.required]);
    this.soNgayLamViecControl = new FormControl(null, [Validators.required]);
    this.tuNgayControl = new FormControl(null, [Validators.required]);
    this.denNgayControl = new FormControl(null, [Validators.required]);

    this.kyluongForm = new FormGroup({
      tenKyLuongControl: this.tenKyLuongControl,
      soNgayLamViecControl: this.soNgayLamViecControl,
      tuNgayControl: this.tuNgayControl,
      denNgayControl: this.denNgayControl,
    });

    this.kyluongForm.disable();

    this.lyDoTuChoiControl = new FormControl(null, [Validators.required, this.validaytorsService.forbiddenSpaceText]);

    this.lyDoTuChoiForm = new FormGroup({
      lyDoTuChoiControl: this.lyDoTuChoiControl
    });
  }

  initTable() {
    this.cols1 = [
      { field: "ngayLamViecThucTe", header: "Ngày làm việc thực tế", width: "130px", textAlign: "right", height: "80px" },
      { field: "congTac", header: "Công tác", width: "130px", textAlign: "right", height: "80px" },
      { field: "daoTaoHoiThao", header: "Đào tạo/hội thảo", width: "130px", textAlign: "right", height: "80px" },
      { field: "nghiPhep", header: "Nghỉ phép", width: "130px", textAlign: "right", height: "80px" },
      { field: "nghiLe", header: "Nghỉ lễ", width: "130px", textAlign: "right", height: "80px" },
      { field: "nghiCheDo", header: "Nghỉ chế độ", width: "130px", textAlign: "right", height: "80px" },
      { field: "nghiHuongLuongKhac", header: "Nghỉ hưởng lương khác", width: "130px", textAlign: "right", height: "80px" },
      { field: "nghiBu", header: "Nghỉ bù", width: "130px", textAlign: "right", height: "80px" },
      { field: "nghiHuongBhxh", header: "Nghỉ hưởng BHXH", width: "130px", textAlign: "right", height: "80px" },
      { field: "nghiKhongPhep", header: "Tự ý nghỉ không hưởng lương", width: "130px", textAlign: "right", height: "80px" },
      { field: "nghiKhongLuong", header: "Nghỉ không lương", width: "130px", textAlign: "right", height: "80px" },
      { field: "tongNgayDmvs", header: "Tổng ngày đi muộn/về sớm không được trả lương", width: "160px", textAlign: "right", height: "80px" },
      { field: "soLanTruChuyenCan", header: "Số lần trừ chuyên cần", width: "130px", textAlign: "right", height: "80px" },
      { field: "tongSoNgayKhongTinhLuong", header: "Tổng số ngày không tính lương", width: "130px", textAlign: "right", height: "80px" },
      { field: "tongSoNgayTinhLuong", header: "Tổng số ngày tính lương", width: "130px", textAlign: "right", height: "80px" },
      { field: "tongNgayNghiTinhTroCapChuyenCan", header: "Tổng ngày nghỉ tính trợ cấp chuyên cần", width: "150px", textAlign: "right", height: "80px" },
    ];

    this.frozenCols1 = [
      { field: "stt", header: "#", width: "40px", textAlign: "center", height: "80px" },
      { field: "employeeCode", header: "Mã nhân viên", width: "150px", textAlign: "left", height: "80px" },
      { field: "subCode1", header: "Mã phòng ban", width: "90px", textAlign: "left", height: "80px" },
      { field: "employeeName", header: "Họ tên", width: "200px", textAlign: "left", height: "80px" }
    ]

    this.cols2 = [
      { field: "tongNgayTinhLuong", header: "Total payable day for this month, including Holiday (Mon-Fri)(Tổng ngày tính lương)", width: "180px", textAlign: "right" },
      { field: "mucLuongCu", header: "GROSS SALARY LAST MONTH/ VOCATIONAL TRAINING/ PROBATION(Mức lương cũ)", width: "150px", textAlign: "right" },
      { field: "soNgayMucLuongCu", header: "Actual working day with old salary (Số ngày tính theo mức lương cũ)", width: "160px", textAlign: "right" },
      { field: "mucLuongHienTai", header: "NEW GROSS MI TO SIGN LABOR CONTRACT THIS MONTH (Mức lương hiện tại)", width: "150px", textAlign: "right" },
      { field: "luongThucTe", header: "NEW GROSS BASED ON ACTUAL WORKING DAY (Lương thực tế)", width: "150px", textAlign: "right" },
      { field: "khoanBuTruThangTruoc", header: "PAYBACK - GROSS BASIC IF ANY (Khoản bù trừ tháng trước)", width: "150px", textAlign: "right" },
      { field: "luongOtTinhThue", header: "Taxable income O.T (Lương OT tính thuế)", width: "150px", textAlign: "right" },
      { field: "luongOtKhongTinhThue", header: "Tax exempt O.T (Lương OT không tính thuế)", width: "150px", textAlign: "right" },
      { field: "tongTroCapCoDinh", header: "TOTAL GROSS ALLOWANCE (Tổng trợ cấp cố định)", width: "150px", textAlign: "right" },
      { field: "tongTroCapKhac", header: "TOTAL BONUS (Tổng trợ cấp khác)", width: "150px", textAlign: "right" },
      { field: "troCapKhacKhongTinhThue", header: "ADDED NET  ALLOWANCE - TAX EXAMPTION FOR EMPLOYEE/ TAXABLE BY COMPANY Trợ cấp khác không tính thuế", width: "180px", textAlign: "right" },
      { field: "khauTruHoanLaiTruocThue", header: "DEDUCTION FROM EMPLOYEE/ Refund company before Tax (Over paid AL) (Khấu trừ/Hoàn lại trước thuế)", width: "180px", textAlign: "right" },
      { field: "tongThuNhapBaoGomThueVaKhongThue", header: "GRAND TOTAL GROSS (BF deduct OT Examption & Severance allowance yet) (Tổng thu nhập bao gồm cả các khoản tính thuế và không tính thuế)", width: "200px", textAlign: "right" },
      { field: "tongThuNhapSauKhiBoCacKhoanKhongTinhThue", header: "GRAND TOTAL GROSS (After deduct OT Examption, Severance allowance) (Tổng thu nhập sau khi đã loại bỏ các khoản không chịu thuế)", width: "200px", textAlign: "right" },
      { field: "giamTruTruocThue", header: "OTHER DEDUCTION BEFORE TAX (TAX EXAMPTION) (Giảm trừ trước thuế)", width: "150px", textAlign: "right" },
      { field: "thuNhapChiDuaVaoTinhThue", header: "OTHER ADDED TO CALCULATED TAX ONLY, none of those benefits be paid through payroll (Thu nhập chỉ đưa vào tính thuế)", width: "190px", textAlign: "right" },
      { field: "tongThuNhapChiuThueSauGiamTru", header: "TOTAL GROSS SALARY AFTER DEDUCT TAX EXAMPTION (Tổng thu nhập chịu thuế sau giảm trừ)", width: "180px", textAlign: "right" },
      { field: "tongThueTncnCtyVaNld", header: "TOTAL PIT PAYMENT PAID BY COMPANY AND EMPLOYEE (Tổng thuế TNCN phải trả bởi công ty và NLĐ)", width: "180px", textAlign: "right" },
      { field: "thueTncnNld", header: "TOTAL PIT PAYMENT_Borne by employee (Thuế TNCN NLĐ phải trả)", width: "150px", textAlign: "right" },
      { field: "luongCoBanDongBh", header: "Basic salary for SI, AI & HI, Trade Union from 1.7.2019 (Lương cơ bản đóng BHXH, BHYT, BHNN, KPCĐ)", width: "180px", textAlign: "right" },
      { field: "tongTienBhNldPhaiDong", header: "Total employee_SI, AI, UI and HI _10.5% (Tổng tiền BH NLĐ phải đóng)", width: "150px", textAlign: "right" },
      { field: "tongTienBhCtyPhaiDong", header: "Total Company_SI, AI, UI and HI _21.5% (Tổng tiền BH Công ty phải đóng)", width: "150px", textAlign: "right" },
      { field: "thuNhapThucNhanTruocKhiBuTruThue", header: "Net Income BEFORE ADD/ DEDUCT AFTER TAX (Thu nhập thực nhận trước khi bù trừ thuế nếu có)", width: "180px", textAlign: "right" },
      { field: "cacKhoanGiamTruSauThue", header: "Other deduction after Tax (Các khoản giảm trừ sau thuế)", width: "150px", textAlign: "right" },
      { field: "cacKhoanHoanLaiSauThue", header: "Other Additional after Tax (Các khoản hoàn lại sau thuế)", width: "150px", textAlign: "right" },
      { field: "thuNhapThucNhan", header: "TOTAL NET INCOME PAYABLE (Thu nhập thực nhận)", width: "150px", textAlign: "right" },
      { field: "luongTamUng", header: "Paid - 1st time if any Đã trả lần 1 nếu có (hoặc tạm ứng)", width: "150px", textAlign: "right" },
      { field: "luongConLai", header: "NET RECEIVE THIS MONTH (Thực nhận trong tháng)", width: "150px", textAlign: "right" },
      { field: "cacKhoanCtyPhaiTraKhac", header: "Other company payable (Các khoản công ty phải trả khác)", width: "150px", textAlign: "right" },
      { field: "tongChiPhiCtyPhaiTra", header: "Tổng chi phí công ty phải trả", width: "150px", textAlign: "right" },
    ];

    this.frozenCols2 = [
      { field: "stt", header: "#", width: "40px", textAlign: "center" },
      { field: "employeeCode", header: "Mã nhân viên", width: "150px", textAlign: "left" },
      { field: "subCode1", header: "Mã phòng ban", width: "90px", textAlign: "left" },
      { field: "employeeName", header: "Họ tên", width: "200px", textAlign: "left" },
      { field: "positionName", header: "Chức vụ", width: "130px", textAlign: "left" },
    ]

    this.cols6 = [
      { field: "netToGross", header: "Convert from Net to Gross to pay Tax (Chuyển từ net sang gross)", width: "150px", textAlign: "right" },
      { field: "month13", header: "13th monthy salary (Lương tháng 13)", width: "150px", textAlign: "right" },
      { field: "gift", header: "Tet gifts, Cash gift receiving during year-end meeting, Cash Tet bonus (Quà tết)", width: "180px", textAlign: "right" },
      { field: "other", header: "Other Net to pay PIT/ Gross salary and bonus (Khoản khác)", width: "150px", textAlign: "right" },
      { field: "sum", header: "Tổng", width: "150px", textAlign: "right" },
    ];

    this.frozenCols6 = [
      { field: "stt", header: "#", width: "40px", textAlign: "center" },
      { field: "employeeCode", header: "Mã nhân viên", width: "150px", textAlign: "left" },
      { field: "subCode1", header: "Mã phòng ban", width: "90px", textAlign: "left" },
      { field: "employeeName", header: "Họ tên", width: "200px", textAlign: "left" },
      { field: "positionName", header: "Chức vụ", width: "130px", textAlign: "left" },
    ]

    this.cols7 = [
      { field: "baseBhxh", header: "Lương cơ bản đóng BHXH", width: "150px", textAlign: "right" },
      { field: "bhxh", header: "Social Ins (BHXH)", width: "150px", textAlign: "right" },
      { field: "bhyt", header: "H.I (BHYT)", width: "150px", textAlign: "right" },
      { field: "bhtn", header: "Unemployment Ins (BHTN)", width: "150px", textAlign: "right" },
      { field: "bhtnnn", header: "Accident Ins (BHTNNN)", width: "150px", textAlign: "right" },
      { field: "other", header: "Additional SI, AI, HI, UI payback ….Paid in this month if any (Các khoản vể BH bắt buộc bù trừ tháng trước phải trả vào tháng này nếu có)", width: "250px", textAlign: "right" },
      { field: "sum", header: "Tổng", width: "150px", textAlign: "right" },
    ];

    this.frozenCols7 = [
      { field: "stt", header: "#", width: "40px", textAlign: "center" },
      { field: "employeeCode", header: "Mã nhân viên", width: "150px", textAlign: "left" },
      { field: "subCode1", header: "Mã phòng ban", width: "90px", textAlign: "left" },
      { field: "employeeName", header: "Họ tên", width: "200px", textAlign: "left" },
      { field: "positionName", header: "Chức vụ", width: "130px", textAlign: "left" },
    ]

    this.cols8 = [
      { field: "giamTruCaNhan", header: "Personal Deduction (Giảm trừ cá nhân)", width: "150px", textAlign: "right" },
      { field: "giamTruNguoiPhuThuoc", header: "Dependant Amount (Giảm trừ người phụ thuộc)", width: "150px", textAlign: "right" },
      { field: "tienDongBaoHiem", header: "Obligation 10.5%_Employee's side (Số tiền đóng bảo hiểm)", width: "150px", textAlign: "right" },
      { field: "giamTruKhac", header: "Other deduction (Các khoản giảm trừ trước thuế khác)", width: "150px", textAlign: "right" },
      { field: "sum", header: "Tổng", width: "150px", textAlign: "right" },
    ];

    this.frozenCols8 = [
      { field: "stt", header: "#", width: "40px", textAlign: "center" },
      { field: "employeeCode", header: "Mã nhân viên", width: "150px", textAlign: "left" },
      { field: "subCode1", header: "Mã phòng ban", width: "90px", textAlign: "left" },
      { field: "employeeName", header: "Họ tên", width: "200px", textAlign: "left" },
      { field: "positionName", header: "Chức vụ", width: "130px", textAlign: "left" },
    ]

    this.cols9 = [
      { field: "kinhPhiCongDoan", header: "T.U_Employee (KPCĐ)", width: "150px", textAlign: "right" },
      { field: "quyetToanThueTncn", header: "PIT liquidation (Quyết toán thuế TNCN)", width: "150px", textAlign: "right" },
      { field: "other", header: "Other deduction after Tax (Các khoản giảm trừ sau thuế khác)", width: "150px", textAlign: "right" },
      { field: "sum", header: "Tổng", width: "150px", textAlign: "right" },
    ];

    this.frozenCols9 = [
      { field: "stt", header: "#", width: "40px", textAlign: "center" },
      { field: "employeeCode", header: "Mã nhân viên", width: "150px", textAlign: "left" },
      { field: "subCode1", header: "Mã phòng ban", width: "90px", textAlign: "left" },
      { field: "employeeName", header: "Họ tên", width: "200px", textAlign: "left" },
      { field: "positionName", header: "Chức vụ", width: "130px", textAlign: "left" },
    ]

    this.cols10 = [
      { field: "thueTncn", header: "PIT Refund PIT last month (Thuế TNCN hoàn lại nếu có)", width: "150px", textAlign: "right" },
      { field: "other", header: "Other add after Tax (Các khoản cộng thêm sau thuế khác)", width: "150px", textAlign: "right" },
      { field: "sum", header: "Tổng", width: "150px", textAlign: "right" },
    ];

    this.frozenCols10 = [
      { field: "stt", header: "#", width: "40px", textAlign: "center" },
      { field: "employeeCode", header: "Mã nhân viên", width: "150px", textAlign: "left" },
      { field: "subCode1", header: "Mã phòng ban", width: "90px", textAlign: "left" },
      { field: "employeeName", header: "Họ tên", width: "200px", textAlign: "left" },
      { field: "positionName", header: "Chức vụ", width: "130px", textAlign: "left" },
    ]

    this.cols11 = [
      { field: "baseBhxh", header: "Lương cơ bản đóng BHXH", width: "150px", textAlign: "right" },
      { field: "bhxh", header: "Social Ins (BHXH)", width: "150px", textAlign: "right" },
      { field: "bhyt", header: "H.I (BHYT)", width: "150px", textAlign: "right" },
      { field: "bhtn", header: "Unemployment Ins (BHTN)", width: "150px", textAlign: "right" },
      { field: "bhtnnn", header: "Accident Ins (BHTNNN)", width: "150px", textAlign: "right" },
      { field: "kinhPhiCongDoan", header: "T.U_Company (KPCĐ)", width: "150px", textAlign: "right" },
      { field: "other", header: "Additional SI, AI, HI, UI payback …. Paid in this month if any (Các khoản vể BH bắt buộc bù trừ tháng trước phải trả vào tháng này công ty phải đóng nếu có)", width: "250px", textAlign: "right" },
      { field: "fundOct", header: "Fund_Oct", width: "150px", textAlign: "right" },
      { field: "sum", header: "Tổng", width: "150px", textAlign: "right" },
    ];

    this.frozenCols11 = [
      { field: "stt", header: "#", width: "40px", textAlign: "center" },
      { field: "employeeCode", header: "Mã nhân viên", width: "150px", textAlign: "left" },
      { field: "subCode1", header: "Mã phòng ban", width: "90px", textAlign: "left" },
      { field: "employeeName", header: "Họ tên", width: "200px", textAlign: "left" },
      { field: "positionName", header: "Chức vụ", width: "130px", textAlign: "left" },
    ]

    this.cols12 = [
      { field: "khoanBuTruThangTruoc", header: "PAYBACK - GROSS BASIC IF ANY (Khoản bù trừ tháng trước)", width: "150px", textAlign: "right" },
      { field: "troCapKhacKhongTinhThue", header: "ADDED NET ALLOWANCE - TAX EXAMPTION FOR EMPLOYEE/ TAXABLE BY COMPANY (Trợ cấp khác không tính thuế)", width: "200px", textAlign: "right" },
      { field: "khauTruHoanLaiTruocThue", header: "DEDUCTION FROM EMPLOYEE/ Refund company before Tax (Over paid AL) (Khấu trừ/Hoàn lại trước thuế)", width: "200px", textAlign: "right" },
      { field: "luongTamUng", header: "Paid - 1st time if any Đã trả lần 1 nếu có (hoặc tạm ứng)", width: "150px", textAlign: "right" },
    ];

    this.frozenCols12 = [
      { field: "stt", header: "#", width: "40px", textAlign: "center" },
      { field: "employeeCode", header: "Mã nhân viên", width: "150px", textAlign: "left" },
      { field: "subCode1", header: "Mã phòng ban", width: "90px", textAlign: "left" },
      { field: "employeeName", header: "Họ tên", width: "200px", textAlign: "left" },
      { field: "positionName", header: "Chức vụ", width: "130px", textAlign: "left" },
    ]
  }

  async _getPermission() {
    let resource = "salary/salary/ky-luong-detail/";
    this.loading = true;
    let permission: any = await this.getPermission.getPermission(resource);
    this.loading = false;

    if (permission.status == false) {
      this.router.navigate(["/home"]);
      return;
    }

    if (permission.listCurrentActionResource.indexOf("complete") != -1) {
      this.actionComplete = true;
    }

    if (permission.listCurrentActionResource.indexOf("edit") != -1) {
      this.actionEdit = true;
    }

    if (permission.listCurrentActionResource.indexOf("delete") != -1) {
      this.actionDelete = true;
    }
  }

  async getData() {
    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.getKyLuongById(this.kyLuongId);
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.kyLuong = result.kyLuong;
    this.kyLuongClone = result.kyLuong;
    this.listTongHopChamCong = result.listTongHopChamCong;
    this.listLuongTongHop = result.listLuongTongHop;
    this.listLuongCtThuNhapTinhThue = result.listLuongCtThuNhapTinhThue;
    this.listLuongCtBaoHiem = result.listLuongCtBaoHiem;
    this.listLuongCtGiamTruTruocThue = result.listLuongCtGiamTruTruocThue;
    this.listLuongCtGiamTruSauThue = result.listLuongCtGiamTruSauThue;
    this.listLuongCtHoanLaiSauThue = result.listLuongCtHoanLaiSauThue;
    this.listLuongCtCtyDong = result.listLuongCtCtyDong;
    this.listLuongCtOther = result.listLuongCtOther;
    this.trRowTroCapOt = result.listDataHeaderTroCapOt;
    this.trRowTroCapOtFrozen = [result.listDataHeaderTroCapOt[0].filter(item => item.columnKey == 'stt' || item.columnKey == 'employeeCode' || item.columnKey == 'subCode1' || item.columnKey == 'employeeName' || item.columnKey == 'positionName')];
    this.trRowTroCapOt[0] = this.trRowTroCapOt[0].filter(item => item.columnKey != 'stt' && item.columnKey != 'employeeCode' && item.columnKey != 'subCode1' && item.columnKey != 'employeeName' && item.columnKey != 'positionName')
    this.buildData(result.listDataTroCapCoDinh, 2);
    this.buildData(result.listDataTroCapOt, 1);
    this.trRowTroCapCoDinh = result.listDataHeaderTroCapCoDinh;
    this.trRowTroCapCoDinhFrozen = [result.listDataHeaderTroCapCoDinh[0].filter(item => item.columnKey == 'stt' || item.columnKey == 'employeeCode' || item.columnKey == 'subCode1' || item.columnKey == 'employeeName' || item.columnKey == 'positionName')];
    this.trRowTroCapCoDinh[0] = this.trRowTroCapCoDinh[0].filter(item => item.columnKey != 'stt' && item.columnKey != 'employeeCode' && item.columnKey != 'subCode1' && item.columnKey != 'employeeName' && item.columnKey != 'positionName')
    this.buildData(result.listDataTroCapCoDinh, 2);
    this.listLoaiTroCapKhac = result.listLoaiTroCapKhac;
    this.listSelectedLoaiTroCapKhac = result.listSelectedLoaiTroCapKhac;
    this.trRowTroCapKhac = result.listDataHeaderTroCapKhac;
    this.trRowTroCapKhacFrozen = [result.listDataHeaderTroCapKhac[0].filter(item => item.columnKey == 'stt' || item.columnKey == 'employeeCode' || item.columnKey == 'subCode1' || item.columnKey == 'employeeName' || item.columnKey == 'positionName')];
    this.trRowTroCapKhac[0] = this.trRowTroCapKhac[0].filter(item => item.columnKey != 'stt' && item.columnKey != 'employeeCode' && item.columnKey != 'subCode1' && item.columnKey != 'employeeName' && item.columnKey != 'positionName')
    this.buildData(result.listDataTroCapKhac, 3);
    this.isShowGuiPheDuyet = result.isShowGuiPheDuyet;
    this.isShowPheDuyet = result.isShowPheDuyet;
    this.isShowTuChoi = result.isShowTuChoi;
    this.isShowDatVeMoi = result.isShowDatVeMoi;
    this.isShowXoa = result.isShowXoa;
    this.isShowSua = result.isShowSua;

    if (!this.isShowSua) {
      this.kyluongForm.disable();
      this.isEdit = false;
    }

    this.mapDataToForm(this.kyLuong);

    this.getDuLieuQuyTrinh();
  }

  mapDataToForm(kyLuong: KyLuong) {
    this.tenKyLuongControl.setValue(kyLuong.tenKyLuong);
    this.soNgayLamViecControl.setValue(kyLuong.soNgayLamViec);
    this.tuNgayControl.setValue(new Date(kyLuong.tuNgay));
    this.denNgayControl.setValue(new Date(kyLuong.denNgay));
  }

  async save() {
    if (!this.kyluongForm.valid) {
      Object.keys(this.kyluongForm.controls).forEach(key => {
        if (!this.kyluongForm.controls[key].valid) {
          this.kyluongForm.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    this.confirmationService.confirm({
      message: `Bạn chắc chắn đã lưu thông tin chỉnh sửa ở các tab chi tiết?`,
      accept: async () => {
        let kyLuong = new KyLuong();
        kyLuong.kyLuongId = this.kyLuongId;
        kyLuong.tenKyLuong = this.tenKyLuongControl.value.trim();
        kyLuong.soNgayLamViec = this.commonService.convertStringToNumber(this.soNgayLamViecControl.value.toString());
        kyLuong.tuNgay = this.formatDateService.convertToUTCTime(this.tuNgayControl.value);
        kyLuong.denNgay = this.formatDateService.convertToUTCTime(this.denNgayControl.value);

        this.loading = true;
        this.awaitResult = true;
        let result: any = await this.salaryService.createOrUpdateKyLuong(kyLuong);

        if (result.statusCode != 200) {
          this.loading = false;
          this.awaitResult = false;
          this.showMessage("error", result.messageCode);
          return;
        }

        this.isEdit = false;
        this.kyluongForm.disable();
        this.showMessage("success", result.messageCode);
        this.getData();
      },
    });
  }

  async xoa() {
    this.confirmationService.confirm({
      message: `Dữ liệu không thể hoàn tác, bạn chắc chắn muốn xóa?`,
      accept: async () => {
        this.loading = true;
        this.awaitResult = true;
        let result: any = await this.salaryService.deleteKyLuong(this.kyLuongId);

        if (result.statusCode != 200) {
          this.loading = false;
          this.awaitResult = false;
          this.showMessage("error", result.messageCode);
          return;
        }

        this.showMessage("success", result.messageCode);
        setTimeout(() => {
          this.router.navigate(['/salary/ky-luong-list']);
        }, 1500);
      },
    });
  }

  guiPheDuyet() {
    this.loading = true;
    this.awaitResult = true;
    this.quyTrinhService.guiPheDuyet(this.emptyGuid, 14, this.kyLuongId).subscribe(res => {
      let result: any = res;

      if (result.statusCode != 200) {
        this.loading = false;
        this.awaitResult = false;
        this.showMessage("error", result.messageCode);
        return;
      }

      this.showMessage("success", result.messageCode);
      this.getData();
      this.refreshNote++;
    });
  }

  showLyDoTuChoiDialog() {
    this.showLyDoTuChoi = true;
    this.lyDoTuChoiForm.reset();
  }

  closeLyDoTuChoi() {
    this.showLyDoTuChoi = false;
  }

  tuChoi() {
    if (!this.lyDoTuChoiForm.valid) {
      Object.keys(this.lyDoTuChoiForm.controls).forEach(key => {
        if (!this.lyDoTuChoiForm.controls[key].valid) {
          this.lyDoTuChoiForm.controls[key].markAsTouched();
        }
      });

      this.showMessage('warn', 'Bạn chưa nhập đủ thông tin');
      return;
    }

    this.loading = true;
    this.awaitResult = true;
    this.quyTrinhService.tuChoi(this.emptyGuid, 14, this.lyDoTuChoiControl.value?.trim(), this.kyLuongId).subscribe(async res => {
      let result: any = res;

      if (result.statusCode != 200) {
        this.loading = false;
        this.awaitResult = false;
        this.showMessage("error", result.messageCode);
        return;
      }

      this.showMessage("success", result.messageCode);
      await this.getData();
      this.refreshNote++;
      this.showLyDoTuChoi = false;
    });
  }

  pheDuyet() {
    this.loading = true;
    this.awaitResult = true;
    this.quyTrinhService.pheDuyet(this.emptyGuid, 14, null, this.kyLuongId).subscribe(res => {
      let result: any = res;

      if (result.statusCode != 200) {
        this.loading = false;
        this.awaitResult = false;
        this.showMessage("error", result.messageCode);
        return;
      }

      this.showMessage("success", result.messageCode);
      this.getData();
      this.refreshNote++;
    });
  }

  async datVeMoi() {
    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.datVeMoiKyLuong(this.kyLuongId);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showMessage("success", result.messageCode);
    this.getData();
    this.refreshNote++;
  }

  sua() {
    this.isEdit = true;
    this.kyluongForm.enable();
  }

  huySua() {
    this.mapDataToForm(this.kyLuongClone);
    this.isEdit = false;
    this.kyluongForm.disable();
  }

  getDuLieuQuyTrinh() {
    this.quyTrinhService.getDuLieuQuyTrinh(this.emptyGuid, 14, this.kyLuongId).subscribe(res => {
      let result: any = res;

      if (result.statusCode != 200) {
        this.showMessage("error", result.messageCode);
        return;
      }

      this.listDuLieuQuyTrinh = result.listDuLieuQuyTrinh;
    });
  }

  tinhTongThuNhapTinhThue() {
    this.listLuongCtThuNhapTinhThue.forEach(item => {
      let netToGross = 0;
      if (item.netToGross != null && item.netToGross != '') {
        netToGross = this.commonService.convertStringToNumber(item.netToGross.toString());
      }
      item.netToGross = netToGross;

      let month13 = 0;
      if (item.month13 != null && item.month13 != '') {
        month13 = this.commonService.convertStringToNumber(item.month13.toString());
      }
      item.month13 = month13;

      let gift = 0;
      if (item.gift != null && item.gift != '') {
        gift = this.commonService.convertStringToNumber(item.gift.toString());
      }
      item.gift = gift;

      let other = 0;
      if (item.other != null && item.other != '') {
        other = this.commonService.convertStringToNumber(item.other.toString());
      }
      item.other = other;

      item.sum = item.netToGross + item.month13 + item.gift + item.other;
    });
  }

  async saveThuNhapTinhThue() {
    this.loading = true;
    this.awaitResult = true;
    this.tinhTongThuNhapTinhThue();
    let result: any = await this.salaryService.saveThuNhapTinhThue(this.listLuongCtThuNhapTinhThue);
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showMessage("success", result.messageCode);
  }

  tinhTongBaoHiem() {
    this.listLuongCtBaoHiem.forEach(item => {
      let other = 0;
      if (item.other != null && item.other != '') {
        other = this.commonService.convertStringToNumber(item.other.toString());
      }
      item.other = other;

      item.sum = item.bhxh + item.bhyt + item.bhtn + item.bhtnnn + item.other;
    });
  }

  async saveBaoHiem() {
    this.loading = true;
    this.awaitResult = true;
    this.tinhTongBaoHiem();
    let result: any = await this.salaryService.saveBaoHiem(this.listLuongCtBaoHiem);
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showMessage("success", result.messageCode);
  }

  tinhTongGiamTruTruocThue() {
    this.listLuongCtGiamTruTruocThue.forEach(item => {
      let giamTruCaNhan = 0;
      if (item.giamTruCaNhan != null && item.giamTruCaNhan != '') {
        giamTruCaNhan = this.commonService.convertStringToNumber(item.giamTruCaNhan.toString());
      }
      item.giamTruCaNhan = giamTruCaNhan;

      let giamTruNguoiPhuThuoc = 0;
      if (item.giamTruNguoiPhuThuoc != null && item.giamTruNguoiPhuThuoc != '') {
        giamTruNguoiPhuThuoc = this.commonService.convertStringToNumber(item.giamTruNguoiPhuThuoc.toString());
      }
      item.giamTruNguoiPhuThuoc = giamTruNguoiPhuThuoc;

      let tienDongBaoHiem = 0;
      if (item.tienDongBaoHiem != null && item.tienDongBaoHiem != '') {
        tienDongBaoHiem = this.commonService.convertStringToNumber(item.tienDongBaoHiem.toString());
      }
      item.tienDongBaoHiem = tienDongBaoHiem;

      let giamTruKhac = 0;
      if (item.giamTruKhac != null && item.giamTruKhac != '') {
        giamTruKhac = this.commonService.convertStringToNumber(item.giamTruKhac.toString());
      }
      item.giamTruKhac = giamTruKhac;

      item.sum = item.giamTruCaNhan + item.giamTruNguoiPhuThuoc + item.tienDongBaoHiem + item.giamTruKhac;
    });
  }

  async saveGiamTruTruocThue() {
    this.loading = true;
    this.awaitResult = true;
    this.tinhTongGiamTruTruocThue();
    let result: any = await this.salaryService.saveGiamTruTruocThue(this.listLuongCtGiamTruTruocThue);
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showMessage("success", result.messageCode);
  }

  tinhTongGiamTruSauThue() {
    this.listLuongCtGiamTruSauThue.forEach(item => {
      let kinhPhiCongDoan = 0;
      if (item.kinhPhiCongDoan != null && item.kinhPhiCongDoan != '') {
        kinhPhiCongDoan = this.commonService.convertStringToNumber(item.kinhPhiCongDoan.toString());
      }
      item.kinhPhiCongDoan = kinhPhiCongDoan;

      let quyetToanThueTncn = 0;
      if (item.quyetToanThueTncn != null && item.quyetToanThueTncn != '') {
        quyetToanThueTncn = this.commonService.convertStringToNumber(item.quyetToanThueTncn.toString());
      }
      item.quyetToanThueTncn = quyetToanThueTncn;

      let other = 0;
      if (item.other != null && item.other != '') {
        other = this.commonService.convertStringToNumber(item.other.toString());
      }
      item.other = other;

      item.sum = item.kinhPhiCongDoan + item.quyetToanThueTncn + item.other;
    });
  }

  async saveGiamTruSauThue() {
    this.loading = true;
    this.awaitResult = true;
    this.tinhTongGiamTruSauThue();
    let result: any = await this.salaryService.saveGiamTruSauThue(this.listLuongCtGiamTruSauThue);
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showMessage("success", result.messageCode);
  }

  tinhTongHoanLaiSauThue() {
    this.listLuongCtHoanLaiSauThue.forEach(item => {
      let thueTncn = 0;
      if (item.thueTncn != null && item.thueTncn != '') {
        thueTncn = this.commonService.convertStringToNumber(item.thueTncn.toString());
      }
      item.thueTncn = thueTncn;

      let other = 0;
      if (item.other != null && item.other != '') {
        other = this.commonService.convertStringToNumber(item.other.toString());
      }
      item.other = other;

      item.sum = item.thueTncn + item.other;
    });
  }

  async saveHoanLaiSauThue() {
    this.loading = true;
    this.awaitResult = true;
    this.tinhTongHoanLaiSauThue();
    let result: any = await this.salaryService.saveHoanLaiSauThue(this.listLuongCtHoanLaiSauThue);
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showMessage("success", result.messageCode);
  }

  tinhTongCtyDong() {
    this.listLuongCtCtyDong.forEach(item => {
      let bhxh = 0;
      if (item.bhxh != null && item.bhxh != '') {
        bhxh = this.commonService.convertStringToNumber(item.bhxh.toString());
      }
      item.bhxh = bhxh;

      let bhyt = 0;
      if (item.bhyt != null && item.bhyt != '') {
        bhyt = this.commonService.convertStringToNumber(item.bhyt.toString());
      }
      item.bhyt = bhyt;

      let bhtn = 0;
      if (item.bhtn != null && item.bhtn != '') {
        bhtn = this.commonService.convertStringToNumber(item.bhtn.toString());
      }
      item.bhtn = bhtn;

      let bhtnnn = 0;
      if (item.bhtnnn != null && item.bhtnnn != '') {
        bhtnnn = this.commonService.convertStringToNumber(item.bhtnnn.toString());
      }
      item.bhtnnn = bhtnnn;

      let kinhPhiCongDoan = 0;
      if (item.kinhPhiCongDoan != null && item.kinhPhiCongDoan != '') {
        kinhPhiCongDoan = this.commonService.convertStringToNumber(item.kinhPhiCongDoan.toString());
      }
      item.kinhPhiCongDoan = kinhPhiCongDoan;

      let other = 0;
      if (item.other != null && item.other != '') {
        other = this.commonService.convertStringToNumber(item.other.toString());
      }
      item.other = other;

      let fundOct = 0;
      if (item.fundOct != null && item.fundOct != '') {
        fundOct = this.commonService.convertStringToNumber(item.fundOct.toString());
      }
      item.fundOct = fundOct;

      item.sum = item.bhxh + item.bhyt + item.bhtn + item.bhtnnn + item.kinhPhiCongDoan + item.other + item.fundOct;
    });
  }

  async saveCtyDong() {
    this.loading = true;
    this.awaitResult = true;
    this.tinhTongCtyDong();
    let result: any = await this.salaryService.saveCtyDong(this.listLuongCtCtyDong);
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showMessage("success", result.messageCode);
  }

  tinhTongOther() {
    this.listLuongCtOther.forEach(item => {
      let khoanBuTruThangTruoc = 0;
      if (item.khoanBuTruThangTruoc != null && item.khoanBuTruThangTruoc != '') {
        khoanBuTruThangTruoc = this.commonService.convertStringToNumber(item.khoanBuTruThangTruoc.toString());
      }
      item.khoanBuTruThangTruoc = khoanBuTruThangTruoc;

      let troCapKhacKhongTinhThue = 0;
      if (item.troCapKhacKhongTinhThue != null && item.troCapKhacKhongTinhThue != '') {
        troCapKhacKhongTinhThue = this.commonService.convertStringToNumber(item.troCapKhacKhongTinhThue.toString());
      }
      item.troCapKhacKhongTinhThue = troCapKhacKhongTinhThue;

      let khauTruHoanLaiTruocThue = 0;
      if (item.khauTruHoanLaiTruocThue != null && item.khauTruHoanLaiTruocThue != '') {
        khauTruHoanLaiTruocThue = this.commonService.convertStringToNumber(item.khauTruHoanLaiTruocThue.toString());
      }
      item.khauTruHoanLaiTruocThue = khauTruHoanLaiTruocThue;

      let luongTamUng = 0;
      if (item.luongTamUng != null && item.luongTamUng != '') {
        luongTamUng = this.commonService.convertStringToNumber(item.luongTamUng.toString());
      }
      item.luongTamUng = luongTamUng;

      item.sum = item.khoanBuTruThangTruoc + item.troCapKhacKhongTinhThue + item.khauTruHoanLaiTruocThue + item.luongTamUng;
    });
  }

  async saveOther() {
    this.loading = true;
    this.awaitResult = true;
    this.tinhTongOther();
    let result: any = await this.salaryService.saveOther(this.listLuongCtOther);
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.showMessage("success", result.messageCode);
  }

  buildData(list: Array<any>, type: number) {
    //Trợ cấp OT
    if (type == 1) {
      this.cols5 = [];
      this.listDataTroCapOt = [];

      if (list.length) {
        list[0].forEach(item => {
          if (item.isShow) {
            let col = {
              field: item.columnKey,
              header: item.columnKey,
              width: item.width,
              textAlign: item.textAlign
            };
            if (item.columnKey == 'stt' || item.columnKey == 'employeeCode' || item.columnKey == 'subCode1' || item.columnKey == 'employeeName' || item.columnKey == 'positionName') {
              this.frozenCols5.push(col);
            } else {
              this.cols5.push(col);
            }
          }
        });

        list.forEach(row => {
          let dataRow = {};
          row.forEach(item => {
            //Nếu là number
            if (item.valueType == 1) {
              dataRow[item.columnKey] = this.commonService.convertStringToNumber(item.columnValue);
            }
            else {
              dataRow[item.columnKey] = item.columnValue;
            }
          });

          this.listDataTroCapOt = [...this.listDataTroCapOt, dataRow];
        });
      }
    }
    //Trợ cấp cố định
    else if (type == 2) {
      this.cols3 = [];
      this.listDataTroCapCoDinh = [];
      this.frozenCols3 = [];
      if (list.length) {
        list[0].forEach(item => {
          if (item.isShow) {
            let col = {
              field: item.columnKey,
              header: item.columnKey,
              width: item.width,
              textAlign: item.textAlign
            };
            if (item.columnKey == 'stt' || item.columnKey == 'employeeCode' || item.columnKey == 'subCode1' || item.columnKey == 'employeeName' || item.columnKey == 'positionName') {
              this.frozenCols3.push(col);
            } else {
              this.cols3.push(col);
            }

          }
        });
        list.forEach(row => {
          let dataRow = {};
          row.forEach(item => {
            //Nếu là number
            if (item.valueType == 1) {
              dataRow[item.columnKey] = this.commonService.convertStringToNumber(item.columnValue);
            }
            else {
              dataRow[item.columnKey] = item.columnValue;
            }
          });

          this.listDataTroCapCoDinh = [...this.listDataTroCapCoDinh, dataRow];
        });
      }
    }
    else if (type == 3) {
      this.cols4 = [];
      this.listDataTroCapKhac = [];

      if (list.length) {
        list[0].forEach(item => {
          if (item.isShow) {
            let col = {
              field: item.columnKey,
              header: item.columnKey,
              width: item.width,
              textAlign: item.textAlign
            };
            if (item.columnKey == 'stt' || item.columnKey == 'employeeCode' || item.columnKey == 'subCode1' || item.columnKey == 'employeeName' || item.columnKey == 'positionName') {
              this.frozenCols4.push(col);
            } else {
              this.cols4.push(col);
            }
          }
        });

        list.forEach(row => {
          let dataRow = {};
          row.forEach(item => {
            //Nếu là number
            if (item.valueType == 1) {
              dataRow[item.columnKey] = this.commonService.convertStringToNumber(item.columnValue);
            }
            //Nếu là boolean
            else if (item.valueType == 2) {
              dataRow[item.columnKey] = item.columnValue == 'true' ? true : false;
            }
            else {
              dataRow[item.columnKey] = item.columnValue;
            }
          });

          this.listDataTroCapKhac = [...this.listDataTroCapKhac, dataRow];
        });
      }
    }
  }

  async showDieuKienTroCapCoDinh(data, key) {
    //Nếu click vào các ô giá trị của mức trợ cấp
    if (key.includes('index_')) {
      let luongCtLoaiTroCapCoDinhId = data['loaiTroCapCoDinhId_' + key];

      this.loading = true;
      this.awaitResult = true;
      let result: any = await this.salaryService.getListDieuKienTroCapCoDinh(luongCtLoaiTroCapCoDinhId);
      this.loading = false;
      this.awaitResult = false;

      if (result.statusCode != 200) {
        this.showMessage("error", result.messageCode);
        return;
      }

      if (!result.listLuongCtDieuKienTroCapCoDinh.length) {
        this.showMessage("warn", "Loại trợ cấp này không có điều kiện hưởng hoặc nhân viên không đủ điều kiện hưởng trợ cấp");
        return;
      }

      let ref = this.dialogService.open(UpdateDieuKienTroCapCoDinhComponent, {
        header: 'Điều kiện nhận trợ cấp',
        width: '40%',
        baseZIndex: 1030,
        closable: false,
        contentStyle: {
          "min-height": "auto",
          "max-height": "600px",
        },
        data: {
          trangThai: this.kyLuong.trangThai,
          employeeName: data.employeeName,
          loaiTroCap: result.loaiTroCap,
          listLuongCtDieuKienTroCapCoDinh: result.listLuongCtDieuKienTroCapCoDinh
        }
      });

      ref.onClose.subscribe((_result: any) => {
        this.getData();
        this.refreshNote++;
      });
    }
  }

  async themTroCapKhac() {
    let listLoaiTroCapKhacId = this.listSelectedLoaiTroCapKhac.map(x => x.categoryId);

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.saveLuongCtTroCapKhac(this.kyLuongId, listLoaiTroCapKhacId);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showMessage("error", result.messageCode);
      return;
    }

    await this.getData();
    this.showMessage("success", result.messageCode);
  }

  async showDieuKienTroCapKhac(data, key) {
    //Nếu click vào các ô giá trị của mức trợ cấp
    if (key.includes('index_')) {
      let luongCtLoaiTroCapKhacId = data['loaiTroCapKhacId_' + key];

      this.loading = true;
      this.awaitResult = true;
      let result: any = await this.salaryService.getListDieuKienTroCapKhac(luongCtLoaiTroCapKhacId);
      this.loading = false;
      this.awaitResult = false;

      if (result.statusCode != 200) {
        this.showMessage("error", result.messageCode);
        return;
      }

      let isCoCauHinhLoaiTroCap = result.isCoCauHinhLoaiTroCap;

      let ref = this.dialogService.open(UpdateDieuKienTroCapKhacComponent, {
        header: 'Điều kiện nhận trợ cấp',
        width: '40%',
        baseZIndex: 1030,
        closable: false,
        contentStyle: {
          "min-height": "auto",
          "max-height": "600px",
        },
        data: {
          luongCtLoaiTroCapKhacId: luongCtLoaiTroCapKhacId,
          trangThai: this.kyLuong.trangThai,
          employeeId: data.employee_id,
          employeeName: data.employeeName,
          loaiTroCap: result.loaiTroCap,
          mucTroCap: result.mucTroCap,
          listLuongCtDieuKienTroCapKhac: result.listLuongCtDieuKienTroCapKhac,
          isCoCauHinhLoaiTroCap: isCoCauHinhLoaiTroCap
        }
      });

      ref.onClose.subscribe((_result: any) => {
        this.getData();
        this.refreshNote++;
      });
    }
  }

  importBangLuongChiTiet() {
    this.displayDialogImport = true;
    this.typeImport = 2;
  }

  importTroCapKhac() {
    this.displayDialogImport = true;
    this.typeImport = 1;
  }

  chooseFileTroCapKhac(event: any) {
    this.fileName = event.target.files[0].name;
    this.fileExcelImport = event.target;
  }

  cancelFileTroCapKhac() {
    this.attachment.nativeElement.value = '';
    this.fileName = null;
  }

  importExcel() {
    if (!this.fileName) {
      this.showMessage("error", 'Bạn chưa chọn file');
      return;
    }

    const targetFiles: DataTransfer = <DataTransfer>(this.fileExcelImport);
    const reader: FileReader = new FileReader();
    reader.readAsBinaryString(targetFiles.files[0]);

    reader.onload = async (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });

      let code = workbook.SheetNames[0];

      // Lấy data từ file excel
      const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[code];

      //Trợ cấp khác
      if (this.typeImport == 1) {
        let dataPoint: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
        dataPoint.shift();

        let listDataHeader = dataPoint[0];
        listDataHeader.shift();

        dataPoint.shift();

        let listData = [];
        dataPoint.forEach((row, index) => {
          if (row.length != 0) {
            let data = {
              employeeCode: row[0],
              listDataLoaiTroCap: []
            };

            listDataHeader.forEach((tenLoaiTroCap, _index) => {
              let troCap = {
                tenLoaiTroCap: tenLoaiTroCap,
                mucTroCap: row[_index + 1]
              };

              data.listDataLoaiTroCap.push(troCap)
            });

            listData.push(data);
          }
        });

        if (!listData.length) {
          this.showMessage("error", 'File import không có dữ liệu');
          this.displayDialogImport = false;
          return;
        }

        this.loading = true;
        this.awaitResult = true;
        let result: any = await this.salaryService.importTroCapKhac(listData, this.kyLuongId);

        if (result.statusCode != 200) {
          this.loading = false;
          this.awaitResult = false;
          this.showMessage("error", result.messageCode);
          return;
        }

        //Nếu có lỗi
        if (result.isError) {
          this.loading = false;
          this.awaitResult = false;

          this.messEmpHeThong = result.messEmpHeThong;
          this.messEmpBangLuong = result.messEmpBangLuong;
          this.messLoaiTroCap = result.messLoaiTroCap;

          this.displayErrorDialog = true;
        }
        //Nếu ko có lỗi
        else {
          await this.getData();
          this.refreshNote++;

          this.showMessage("success", result.messageCode);
          this.displayDialogImport = false;
        }
      }
      //Lương chi tiết
      else {
        let dataPoint: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
        dataPoint.shift();
        dataPoint.shift();

        let listData = [];
        dataPoint.forEach((row, index) => {
          if (row.length != 0) {
            let data = {
              kyLuongId: this.kyLuongId,
              employeeCode: row[0],
              thuNhapTinhThueNetToGross: row[1],
              thuNhapTinhThueMonth13: row[2],
              thuNhapTinhThueGift: row[3],
              thuNhapTinhThueOther: row[4],
              baoHiemOther: row[5],
              giamTruTruocThueGiamTruKhac: row[6],
              giamTruSauThueQuyetToanThueTncn: row[7],
              giamTruSauThueOther: row[8],
              hoanLaiSauThueThueTncn: row[9],
              hoanLaiSauThueOther: row[10],
              ctyDongOther: row[11],
              ctyDongFundOct: row[12],
              otherKhoanBuTruThangTruoc: row[13],
              otherTroCapKhacKhongTinhThue: row[14],
              otherKhauTruHoanLaiTruocThue: row[15],
              otherLuongTamUng: row[16],
            };

            listData.push(data);
          }
        });

        if (!listData.length) {
          this.showMessage("error", 'File import không có dữ liệu');
          this.displayDialogImport = false;
          return;
        }

        this.loading = true;
        this.awaitResult = true;
        let result: any = await this.salaryService.importLuongChiTiet(listData);

        if (result.statusCode != 200) {
          this.loading = false;
          this.awaitResult = false;
          this.showMessage("error", result.messageCode);
          return;
        }

        //Nếu có lỗi
        if (result.isError) {
          this.loading = false;
          this.awaitResult = false;

          this.messEmpHeThong = result.messEmpHeThong;
          this.messEmpBangLuong = result.messEmpBangLuong;

          this.displayErrorDialog = true;
        }
        //Nếu ko có lỗi
        else {
          await this.getData();
          this.refreshNote++;

          this.showMessage("success", result.messageCode);
          this.displayDialogImport = false;
        }
      }
    }
  }

  closeErrorDialog() {
    this.displayErrorDialog = false;
    this.messEmpHeThong = null;
    this.messEmpBangLuong = null;
    this.messLoaiTroCap = null;
  }

  async complete() {
    this.confirmationService.confirm({
      message: `Bạn chắc chắc xác nhận hoàn thành kỳ lương và gửi payslip cho nhân viên?`,
      accept: async () => {
        this.loading = true;
        this.awaitResult = true;
        let result: any = await this.salaryService.hoanThanhKyLuong(this.kyLuongId);

        if (result.statusCode != 200) {
          this.loading = false;
          this.awaitResult = false;
          this.showMessage("error", result.messageCode);
          return;
        }

        await this.getData();
        this.refreshNote++;
        this.showMessage("success", result.messageCode);
      },
    });
  }

  async xuatBaoCao() {
    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.exportBaoCaoKyLuong(this.kyLuongId);
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    let title = 'In Comparison with forecast';
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet(title);

    /* header table */
    let headerBaoCao1 = result.listHeaderBaoBao;

    let headerRow1 = worksheet.addRow(headerBaoCao1);

    headerRow1.height = 70;
    headerRow1.font = { name: 'Times New Roman', size: 11, bold: true };

    headerBaoCao1.forEach((item, index) => {
      headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

      if (index == 4 || index == 5) {
        headerRow1.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'FF0000' }
        };
      }
      else {
        headerRow1.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'FABF8F' }
        };
      }
    });

    /* data table */
    let dataBaoCao1 = result.listDataBaoCao;

    let data = dataBaoCao1.map((item, index) => {
      let row = [];

      if (index == 4) {
        row.push('');
        row.push('');
        row.push(item.cdActual1);
        row.push(item.forecast);
        row.push(item.cdActual2);
        row.push(item.vsActual);
        row.push(item.vsForecast);
        row.push('');
      } else {
        row.push(index + 1);
        row.push(item.payment);
        row.push(item.cdActual1);
        row.push(item.forecast);
        row.push(item.cdActual2);
        row.push(item.vsActual);
        row.push(item.vsForecast);
        row.push('');
      }

      return row;
    });

    data.forEach((el, index) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman' };
      if (index == 4) {
        row.font = { bold: true };
        row.height = 30;
      }

      for (let i = 0; i < el.length; i++) {
        row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

        if (i == 0) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        }
        else if (i == 1 || i == 7) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
        }
        else {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          row.getCell(i + 1).numFmt = '#,##0';
        }

        if (i == 5) {
          row.getCell(i + 1).value = { formula: `=E${row.number}-C${row.number}`, date1904: false };
        }
        else if (i == 6) {
          row.getCell(i + 1).value = { formula: `=D${row.number}-E${row.number}`, date1904: false };
        }
      }
    });
    worksheet.mergeCells('A6:B6');

    /* set width */
    worksheet.getColumn(1).width = 10;
    worksheet.getColumn(2).width = 35;
    worksheet.getColumn(8).width = 100;

    for (let i = 1; i <= headerBaoCao1.length; i++) {
      if (i > 2 && i < 8) {
        worksheet.getColumn(i).width = 15;
      }
    }

    /*---------------Begin: Sheet 2---------------*/

    let titleSheet2 = 'Cost Center_Loft';
    let worksheet2 = workbook.addWorksheet(titleSheet2);

    /* title table 2 */
    let titleTable2Row = worksheet2.addRow(['SUMMARY BY CODE CENTER - LOFT']);
    titleTable2Row.font = { name: 'Times New Roman', family: 4, size: 30, bold: true };
    titleTable2Row.height = 40;
    worksheet2.mergeCells(`A${titleTable2Row.number}:K${titleTable2Row.number}`)
    titleTable2Row.alignment = { vertical: 'middle', horizontal: 'left' };

    titleTable2Row = worksheet2.addRow([`PAYROL OF ${result.thangNamBatDauKyLuongTiengAnh}`.toUpperCase()]);
    titleTable2Row.font = { name: 'Times New Roman', family: 4, size: 15, bold: true };
    titleTable2Row.height = 20;
    worksheet2.mergeCells(`A${titleTable2Row.number}:K${titleTable2Row.number}`)
    titleTable2Row.alignment = { vertical: 'middle', horizontal: 'left' };

    /* header table 2 */
    let headerTable2 = [
      'Dept',
      'No of staff',
      'Basic Gross salary',
      'Bonus/ Allowances/ Others_AV',
      'Trade Union',
      'PIT',
      'PIT (Tax)',
      'Obligation insurance',
      'Net payable',
      'Total company gross payable before off-set',
      'Total company gross payable after off-set'
    ];

    let headerTable2Row = worksheet2.addRow(headerTable2);
    headerTable2Row.height = 80;
    headerTable2Row.font = { name: 'Times New Roman', size: 15, bold: true };

    headerTable2.forEach((item, index) => {
      headerTable2Row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerTable2Row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

      if (index == 9) {
        headerTable2Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'FFFF00' }
        };
      }
      else if (index >= 4 && index <= 8) {
        headerTable2Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'EBF1DE' }
        };
        headerTable2Row.getCell(index + 1).font = {
          name: 'Times New Roman', size: 15, bold: true, color: { argb: 'C00000' }
        }
      }
      else {
        headerTable2Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'EBF1DE' }
        };
      }
    });

    /* data table 2 */
    let dataTable2 = result.listDataBaoCao2Bang1.map((item, index) => {
      let row = [];

      row.push(item.dept);
      row.push(item.noOfStaff);
      row.push(item.basicGrossSalary);
      row.push(item.bonus);
      row.push(item.tradeUnion);
      row.push(item.pit);
      row.push(item.pitTax);
      row.push(item.obligation);
      row.push(item.netPayable);
      row.push(item.totalCompBefore);
      row.push(item.totalCompAfter);

      return row;
    });

    dataTable2.forEach((el, index) => {
      let row = worksheet2.addRow(el);
      row.height = 40;
      row.font = { name: 'Times New Roman', size: 13, bold: true };

      for (let i = 0; i < el.length; i++) {
        row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

        if (i == 0) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        }
        else if (i >= 4 && i <= 8) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          row.getCell(i + 1).font = { name: 'Times New Roman', size: 13, bold: true, color: { argb: 'C00000' } };
          row.getCell(i + 1).numFmt = '#,##0';
        }
        else {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          row.getCell(i + 1).numFmt = '#,##0';
        }
      }
    });

    let dataTable2Sum = ['TOTAL VND', '', '', '', '', '', '', '', '', '', ''];
    let dataTable2SumRow = worksheet2.addRow(dataTable2Sum);
    dataTable2SumRow.height = 40;
    dataTable2SumRow.font = { name: 'Times New Roman', size: 13, bold: true, underline: true };
    for (let i = 0; i < dataTable2Sum.length; i++) {
      dataTable2SumRow.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

      if (i == 0) {
        dataTable2SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      }
      else if (i >= 4 && i <= 8) {
        dataTable2SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        dataTable2SumRow.getCell(i + 1).font = { name: 'Times New Roman', size: 13, bold: true, underline: true, color: { argb: 'C00000' } };
        dataTable2SumRow.getCell(i + 1).numFmt = '#,##0';
      }
      else {
        dataTable2SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        dataTable2SumRow.getCell(i + 1).numFmt = '#,##0';
      }

      if (i > 0 && i < 10) {
        dataTable2SumRow.getCell(i + 1).value = { formula: `=SUM(${this.getHeaderColumn(dataTable2SumRow.getCell(i + 1).$col$row)}${dataTable2SumRow.number - dataTable2.length}:${this.getHeaderColumn(dataTable2SumRow.getCell(i + 1).$col$row)}${dataTable2SumRow.number - 1})`, date1904: false };
      }
    }

    let dataTable2Check = ['CHECK', '', '', '', '', '', '', '', '', '', ''];
    let dataTable2CheckRow = worksheet2.addRow(dataTable2Check);
    dataTable2CheckRow.height = 40;
    dataTable2CheckRow.font = { name: 'Times New Roman', size: 13, bold: true };
    for (let i = 0; i < dataTable2Sum.length; i++) {
      dataTable2CheckRow.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

      if (i == 0) {
        dataTable2CheckRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        dataTable2CheckRow.getCell(i + 1).font = { name: 'Times New Roman', size: 13, bold: true, underline: true };
      }
      else {
        dataTable2CheckRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        dataTable2CheckRow.getCell(i + 1).numFmt = '#,##0';
      }
    }

    for (var i = 0; i < 5; i++) {          // cach khoang 5 dong
      let space = worksheet2.addRow(['']);
      space.height = 30;
    }

    /* title table 3 */
    let titleTable3Row = worksheet2.addRow(['SUMMARY BY GRADE - 3D & QA']);
    titleTable3Row.font = { name: 'Times New Roman', family: 4, size: 30, bold: true };
    titleTable3Row.height = 40;
    worksheet2.mergeCells(`A${titleTable3Row.number}:K${titleTable3Row.number}`)
    titleTable3Row.alignment = { vertical: 'middle', horizontal: 'left' };

    /* header table 3 */
    let headerTable3 = [
      'Dept',
      'No of staff',
      'Gross salary',
      'Bonus/ Allowances/ Others_AV',
      'Trade Union ER',
      'PIT',
      'PIT (Tax)',
      'Obligation insurance',
      'Net payroll',
      'Total company gross payable_DF',
    ];
    let headerTable3Row = worksheet2.addRow(headerTable3);

    headerTable3Row.height = 70;
    headerTable3Row.font = { name: 'Times New Roman', size: 15, bold: true };

    headerTable3.forEach((item, index) => {
      headerTable3Row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerTable3Row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

      if (index == 10) {
        headerTable3Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'FFFF00' }
        };
      } else {
        headerTable3Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'EBF1DE' }
        };
      }
    });

    /* data table 3*/
    let dataTable3 = result.listDataBaoCao2Bang2.map((item, index) => {
      let row = [];

      row.push(item.dept);
      row.push(item.noOfStaff);
      row.push(item.grossSalary);
      row.push(item.bonus);
      row.push(item.tradeUnion);
      row.push(item.pit);
      row.push(item.pitTax);
      row.push(item.obligation);
      row.push(item.netPayroll);
      row.push(item.totalComp);

      return row;
    });

    dataTable3.forEach((el, index) => {
      let row = worksheet2.addRow(el);
      row.height = 30;
      row.font = { name: 'Times New Roman', size: 13, bold: true };

      for (let i = 0; i < el.length; i++) {
        row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

        if (i == 0) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        } else {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          row.getCell(i + 1).numFmt = '#,##0';
        }
      }
    });

    let dataTable3Sum = ['TOTAL VND', '', '', '', '', '', '', '', '', ''];
    let dataTable3SumRow = worksheet2.addRow(dataTable3Sum);
    dataTable3SumRow.height = 40;
    dataTable3SumRow.font = { name: 'Times New Roman', size: 13, bold: true, color: { argb: 'C00000' } };
    for (let i = 0; i < dataTable3Sum.length; i++) {
      dataTable3SumRow.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

      if (i == 0) {
        dataTable3SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      }
      else {
        dataTable3SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        dataTable3SumRow.getCell(i + 1).numFmt = '#,##0';
      }

      //Tính theo công thức
      if (i > 0) {
        dataTable3SumRow.getCell(i + 1).value = { formula: `=SUM(${this.getHeaderColumn(dataTable3SumRow.getCell(i + 1).$col$row)}${dataTable3SumRow.number - dataTable3.length}:${this.getHeaderColumn(dataTable3SumRow.getCell(i + 1).$col$row)}${dataTable3SumRow.number - 1})`, date1904: false };
      }
    }

    let dataTable3Check = ['CHECK', '', '', '', '', '', '', '', '', ''];
    let dataTable3CheckRow = worksheet2.addRow(dataTable3Check);
    dataTable3CheckRow.height = 40;
    dataTable3CheckRow.font = { name: 'Times New Roman', size: 13, bold: true };
    for (let i = 0; i < dataTable3Sum.length; i++) {
      dataTable3CheckRow.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

      if (i == 0) {
        dataTable3CheckRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        dataTable3CheckRow.getCell(i + 1).font = { name: 'Times New Roman', size: 13, bold: true, underline: true };
      }
      else {
        dataTable3CheckRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        dataTable3CheckRow.getCell(i + 1).numFmt = '#,##0';
      }
    }

    worksheet2.addRow(['']);
    worksheet2.addRow(['']);
    worksheet2.addRow(['']);
    worksheet2.addRow(['']);
    worksheet2.addRow(['']);
    worksheet2.addRow(['']);

    /* title table 4 */
    let titleTable4Row = worksheet2.addRow(['COMPARISION - BASIC PAYOUT VS TARGET']);
    titleTable4Row.font = { name: 'Times New Roman', family: 4, size: 14, bold: true };
    titleTable4Row.height = 20;
    worksheet2.mergeCells(`A${titleTable4Row.number}:K${titleTable4Row.number}`)
    titleTable4Row.alignment = { vertical: 'middle', horizontal: 'left' };

    /* header table 4 */
    let headerTable4_1 = [
      '(1)',
      '(2)',
      '(3)',
      '(4)',
      '(5)',
      '(6)',
      '(7)',
      '(8)',
      '(9)',
      '(10)',
    ];
    let headerTable4_1Row = worksheet2.addRow(headerTable4_1);

    headerTable4_1Row.height = 33;
    headerTable4_1Row.font = { name: 'Times New Roman', size: 10, bold: true };

    headerTable4_1.forEach((item, index) => {
      headerTable4_1Row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      headerTable4_1Row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });

    let headerTable4_2 = [
      'Grade',
      'Number of employees',
      'GROSS BASE SALARY',
      'AVERAGE-GROSS BASE SALARY',
      'TARGET Average BASIC SALARY STRUCTURE',
      '',
      'Actual vs Target',
      '',
      '',
      '',
    ];
    let headerTable4_2Row = worksheet2.addRow(headerTable4_2);

    headerTable4_2Row.height = 70;
    headerTable4_2Row.font = { name: 'Times New Roman', size: 11, bold: true };

    headerTable4_2.forEach((item, index) => {
      if (index < 5 || index == 6) {
        headerTable4_2Row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerTable4_2Row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerTable4_2Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'EBF1DE' }
        };
      }
    });

    /* data table 4*/
    let dataTable4 = result.listDataBaoCao2Bang3.map((item, index) => {
      let row = [];

      row.push(item.grade);
      row.push(item.numberOfEmployees);
      row.push(item.grossBase);
      row.push(item.averageGrossBase);
      row.push(item.targetAverage);
      row.push(item.empty1);
      row.push(item.actual);
      row.push(item.empty2);
      row.push(item.empty3);
      row.push(item.empty4);

      return row;
    });

    dataTable4.forEach((el, index) => {
      let row = worksheet2.addRow(el);
      row.height = 30;
      row.font = { name: 'Times New Roman', size: 10 };

      for (let i = 0; i < el.length; i++) {
        if (i < 5 || i == 6) {
          row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        }

        if (i == 0) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
        }
        else if (i >= 1 && i <= 4) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          row.getCell(i + 1).numFmt = '#,##0';
        }
        else if (i == 6) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          row.getCell(i + 1).numFmt = '0%';
        }
        else if (i == 7) {
          row.getCell(i + 1).numFmt = '_(* #,##0_);_(* (#,##0);_(* "-"??_);_(@_)';
        }
        else if (i == 8 || i == 9) {
          row.getCell(i + 1).numFmt = '_($* #,##0.0_);_($* (#,##0.0);_($* "-"??_);_(@_)';
        }

        //Tính theo công thức
        if (i == 3) {
          row.getCell(i + 1).value = { formula: `=C${row.number}/B${row.number}`, date1904: false };
        }
        else if (i == 6) {
          row.getCell(i + 1).value = { formula: `=D${row.number}/E${row.number}`, date1904: false };
        }
        else if (index == 1 && i == 7) {
          row.getCell(i + 1).value = { formula: `=AVERAGE(D31:D32)`, date1904: false };
        }
        else if (index == 1 && i == 8) {
          row.getCell(i + 1).value = { formula: `=H31/23000`, date1904: false };
        }
        else if (index == 1 && i == 9) {
          row.getCell(i + 1).value = { formula: `=I31*12`, date1904: false };
        }
        else if (index == 2 && i == 7) {
          row.getCell(i + 1).value = { formula: `=H31*40`, date1904: false };
        }
        else if (index == 2 && i == 8) {
          row.getCell(i + 1).value = { formula: `=I31*40`, date1904: false };
        }
      }
    });

    let dataTable4Sum = ['Total', '', '', '', '', '', '', '', '', ''];
    let dataTable4SumRow = worksheet2.addRow(dataTable4Sum);
    dataTable4SumRow.height = 30;
    dataTable4SumRow.font = { name: 'Times New Roman', size: 10, bold: true, color: { argb: 'C00000' } };
    for (let i = 0; i < dataTable4Sum.length; i++) {
      if (i < 5 || i == 6) {
        dataTable4SumRow.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }

      if (i == 0) {
        dataTable4SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      }
      else if (i >= 1 && i <= 4) {
        dataTable4SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        dataTable4SumRow.getCell(i + 1).numFmt = '#,##0';
      }
      else if (i == 6) {
        dataTable4SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        dataTable4SumRow.getCell(i + 1).numFmt = '0%';
      }

      //Tính theo công thức
      if (i == 1 || i == 2) {
        dataTable4SumRow.getCell(i + 1).value = { formula: `=SUM(${this.getHeaderColumn(dataTable4SumRow.getCell(i + 1).$col$row)}${dataTable4SumRow.number - dataTable4.length}:${this.getHeaderColumn(dataTable4SumRow.getCell(i + 1).$col$row)}${dataTable4SumRow.number - 1})`, date1904: false };
      }
      else if (i == 3) {
        dataTable4SumRow.getCell(i + 1).value = { formula: `=C${dataTable4SumRow.number}/B${dataTable4SumRow.number}`, date1904: false };
      }
      else if (i == 4) {
        dataTable4SumRow.getCell(i + 1).value = { formula: `=AVERAGE(${this.getHeaderColumn(dataTable4SumRow.getCell(i + 1).$col$row)}${dataTable4SumRow.number - 2}:${this.getHeaderColumn(dataTable4SumRow.getCell(i + 1).$col$row)}${dataTable4SumRow.number - 1})`, date1904: false };
      }
      else if (i == 6) {
        dataTable4SumRow.getCell(i + 1).value = { formula: `=D${dataTable4SumRow.number}/E${dataTable4SumRow.number}`, date1904: false };
      }
    }

    worksheet2.addRow(['']);
    worksheet2.addRow(['']);

    /* title table 5 */
    let titleTable5Row = worksheet2.addRow(['COMPARISION - TOTAL GROSS BASE + BENEFITS PAYOUT VS TARGET']);
    titleTable5Row.font = { name: 'Times New Roman', family: 4, size: 14, bold: true };
    titleTable5Row.height = 20;
    worksheet2.mergeCells(`A${titleTable5Row.number}:K${titleTable5Row.number}`)
    titleTable5Row.alignment = { vertical: 'middle', horizontal: 'left' };

    /* header table 5 */
    let headerTable5_1 = [
      '(1)',
      '(2)',
      '(3)',
      '(4)',
      '(5)',
      '(6)',
      '(7)',
      '(8)',
      '(9)',
      '(10)',
    ];
    let headerTable5_1Row = worksheet2.addRow(headerTable5_1);

    headerTable5_1Row.height = 33;
    headerTable5_1Row.font = { name: 'Times New Roman', size: 10, bold: true };

    headerTable5_1.forEach((item, index) => {
      headerTable5_1Row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      headerTable5_1Row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });

    let headerTable5_2 = [
      'Grade',
      'Number of employees',
      'GROSS BASE + OTHER ALLOWANCE',
      'AVERAGE-GROSS BASE + OTHER ALLOWANCE',
      'TARGET_Average GROSS + OTHER ALLOWANCE',
      '',
      'Actual vs Target',
      '',
      '',
      '',
    ];
    let headerTable5_2Row = worksheet2.addRow(headerTable5_2);

    headerTable5_2Row.height = 70;
    headerTable5_2Row.font = { name: 'Times New Roman', size: 11, bold: true };

    headerTable5_2.forEach((item, index) => {
      if (index < 5 || index == 6) {
        headerTable5_2Row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerTable5_2Row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerTable5_2Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'EBF1DE' }
        };
      }
    });

    /* data table 5 */
    let dataTable5 = result.listDataBaoCao2Bang4.map((item, index) => {
      let row = [];

      row.push(item.grade);
      row.push(item.numberOfEmployees);
      row.push(item.grossBase);
      row.push(item.averageGrossBase);
      row.push(item.targetAverage);
      row.push(item.empty1);
      row.push(item.actual);
      row.push(item.empty2);
      row.push(item.empty3);
      row.push(item.empty4);

      return row;
    });

    dataTable5.forEach((el, index) => {
      let row = worksheet2.addRow(el);
      row.height = 30;
      row.font = { name: 'Times New Roman', size: 10 };

      for (let i = 0; i < el.length; i++) {
        if (i < 5 || i == 6) {
          row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        }

        if (i == 0) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
        }
        else if (i >= 1 && i <= 4) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          row.getCell(i + 1).numFmt = '_(* #,##0_);_(* (#,##0);_(* "-"??_);_(@_)';
        }
        else if (i == 6) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          row.getCell(i + 1).numFmt = '0%';
        }

        //Tính theo công thức
        if (i == 2) {
          row.getCell(i + 1).value = { formula: `=C${row.number - 23}+D${row.number - 23}`, date1904: false };
        }
        else if (i == 3) {
          row.getCell(i + 1).value = { formula: `=C${row.number}/B${row.number}`, date1904: false };
        }
        else if (i == 6) {
          row.getCell(i + 1).value = { formula: `=D${row.number}/E${row.number}`, date1904: false };
        }
      }
    });

    let dataTable5Sum = ['Total', '', '', '', '', '', '', '', '', ''];
    let dataTable5SumRow = worksheet2.addRow(dataTable5Sum);
    dataTable5SumRow.height = 30;
    dataTable5SumRow.font = { name: 'Times New Roman', size: 10, bold: true, color: { argb: 'C00000' } };
    for (let i = 0; i < dataTable5Sum.length; i++) {
      if (i < 5 || i == 6) {
        dataTable5SumRow.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }

      if (i == 0) {
        dataTable5SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      }
      else if (i >= 1 && i <= 4) {
        dataTable5SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        dataTable5SumRow.getCell(i + 1).numFmt = '_(* #,##0_);_(* (#,##0);_(* "-"??_);_(@_)';
      }
      else if (i == 6) {
        dataTable5SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        dataTable5SumRow.getCell(i + 1).numFmt = '0%';
      }

      //Tính theo công thức
      if (i == 1 || i == 2) {
        dataTable5SumRow.getCell(i + 1).value = { formula: `=SUM(${this.getHeaderColumn(dataTable5SumRow.getCell(i + 1).$col$row)}${dataTable5SumRow.number - dataTable5.length}:${this.getHeaderColumn(dataTable5SumRow.getCell(i + 1).$col$row)}${dataTable5SumRow.number - 1})`, date1904: false };
      }
      else if (i == 3) {
        dataTable5SumRow.getCell(i + 1).value = { formula: `=C${dataTable5SumRow.number}/B${dataTable5SumRow.number}`, date1904: false };
      }
      else if (i == 4) {
        dataTable5SumRow.getCell(i + 1).value = { formula: `=AVERAGE(${this.getHeaderColumn(dataTable5SumRow.getCell(i + 1).$col$row)}${dataTable5SumRow.number - 2}:${this.getHeaderColumn(dataTable5SumRow.getCell(i + 1).$col$row)}${dataTable5SumRow.number - 1})`, date1904: false };
      }
      else if (i == 6) {
        dataTable5SumRow.getCell(i + 1).value = { formula: `=D${dataTable5SumRow.number}/E${dataTable5SumRow.number}`, date1904: false };
      }
    }

    worksheet2.addRow(['']);
    worksheet2.addRow(['']);

    /* title table 6 */
    let titleTable6Row = worksheet2.addRow(['COMPARISION - TOTAL LUMPSUM GROSS (Net + PIT, Government fee) VS TARGET']);
    titleTable6Row.font = { name: 'Times New Roman', family: 4, size: 14, bold: true };
    titleTable6Row.height = 20;
    worksheet2.mergeCells(`A${titleTable6Row.number}:K${titleTable6Row.number}`)
    titleTable6Row.alignment = { vertical: 'middle', horizontal: 'left' };

    /* header table 6 */
    let headerTable6_1 = [
      '(1)',
      '(2)',
      '(3)',
      '(4)',
      '(5)',
      '(6)',
      '(7)',
      '(8)',
      '(9)',
      '(10)',
    ];
    let headerTable6_1Row = worksheet2.addRow(headerTable6_1);

    headerTable6_1Row.height = 33;
    headerTable6_1Row.font = { name: 'Times New Roman', size: 10, bold: true };

    headerTable6_1.forEach((item, index) => {
      headerTable6_1Row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      headerTable6_1Row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });

    let headerTable6_2 = [
      'Grade',
      'Number of employees',
      'TOTAL LUMPSUM GROSS PAYOUT',
      'AVERAGE TOTAL LUMPSUM GROSS PAYOUT',
      'TARGET_Average GROSS + OTHER ALLOWANCE',
      '',
      'Actual vs Target',
      '',
      '',
      '',
    ];
    let headerTable6_2Row = worksheet2.addRow(headerTable6_2);

    headerTable6_2Row.height = 70;
    headerTable6_2Row.font = { name: 'Times New Roman', size: 11, bold: true };

    headerTable6_2.forEach((item, index) => {
      if (index < 5 || index == 6) {
        headerTable6_2Row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerTable6_2Row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerTable6_2Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'EBF1DE' }
        };
      }
    });

    /* data table 6 */
    let dataTable6 = result.listDataBaoCao2Bang5.map((item, index) => {
      let row = [];

      row.push(item.grade);
      row.push(item.numberOfEmployees);
      row.push(item.grossBase);
      row.push(item.averageGrossBase);
      row.push(item.targetAverage);
      row.push(item.empty1);
      row.push(item.actual);
      row.push(item.empty2);
      row.push(item.empty3);
      row.push(item.empty4);

      return row;
    });

    dataTable6.forEach((el, index) => {
      let row = worksheet2.addRow(el);
      row.height = 30;
      row.font = { name: 'Times New Roman', size: 10 };

      for (let i = 0; i < el.length; i++) {
        if (i < 5 || i == 6) {
          row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        }

        if (i == 0) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
        }
        else if (i >= 1 && i <= 4) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          row.getCell(i + 1).numFmt = '_(* #,##0_);_(* (#,##0);_(* "-"??_);_(@_)';
        }
        else if (i == 6) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          row.getCell(i + 1).numFmt = '0%';
        }

        //Tính theo công thức
        if (i == 2) {
          row.getCell(i + 1).value = { formula: `=J${row.number - 32}`, date1904: false };
        }
        else if (i == 3) {
          row.getCell(i + 1).value = { formula: `=C${row.number}/B${row.number}`, date1904: false };
        }
        else if (i == 6) {
          row.getCell(i + 1).value = { formula: `=D${row.number}/E${row.number}`, date1904: false };
        }
      }
    });

    let dataTable6Sum = ['Total', '', '', '', '', '', '', '', '', ''];
    let dataTable6SumRow = worksheet2.addRow(dataTable6Sum);
    dataTable6SumRow.height = 30;
    dataTable6SumRow.font = { name: 'Times New Roman', size: 10, bold: true, color: { argb: 'C00000' } };
    for (let i = 0; i < dataTable6Sum.length; i++) {
      if (i < 5 || i == 6) {
        dataTable6SumRow.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }

      if (i == 0) {
        dataTable6SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      }
      else if (i >= 1 && i <= 4) {
        dataTable6SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        dataTable6SumRow.getCell(i + 1).numFmt = '_(* #,##0_);_(* (#,##0);_(* "-"??_);_(@_)';
      }
      else if (i == 6) {
        dataTable6SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        dataTable6SumRow.getCell(i + 1).numFmt = '0%';
      }

      //Tính theo công thức
      if (i == 1 || i == 2) {
        dataTable6SumRow.getCell(i + 1).value = { formula: `=SUM(${this.getHeaderColumn(dataTable6SumRow.getCell(i + 1).$col$row)}${dataTable6SumRow.number - dataTable5.length}:${this.getHeaderColumn(dataTable6SumRow.getCell(i + 1).$col$row)}${dataTable6SumRow.number - 1})`, date1904: false };
      }
      else if (i == 3) {
        dataTable6SumRow.getCell(i + 1).value = { formula: `=C${dataTable6SumRow.number}/B${dataTable6SumRow.number}`, date1904: false };
      }
      else if (i == 4) {
        dataTable6SumRow.getCell(i + 1).value = { formula: `=AVERAGE(${this.getHeaderColumn(dataTable6SumRow.getCell(i + 1).$col$row)}${dataTable6SumRow.number - 2}:${this.getHeaderColumn(dataTable6SumRow.getCell(i + 1).$col$row)}${dataTable6SumRow.number - 1})`, date1904: false };
      }
      else if (i == 6) {
        dataTable6SumRow.getCell(i + 1).value = { formula: `=D${dataTable6SumRow.number}/E${dataTable6SumRow.number}`, date1904: false };
      }
    }

    worksheet2.addRow(['']);
    worksheet2.addRow(['']);
    worksheet2.addRow(['']);

    let labelSheet2 = worksheet2.addRow(['PREPARED BY:']);
    labelSheet2.font = { name: 'Times New Roman', family: 4, size: 11, bold: true };
    labelSheet2.alignment = { vertical: 'middle', horizontal: 'left' };

    worksheet2.addRow(['']);
    worksheet2.addRow(['']);
    worksheet2.addRow(['']);
    worksheet2.addRow(['']);

    let nameSheet2 = worksheet2.addRow(['NGUYEN THI TRANG']);
    nameSheet2.font = { name: 'Times New Roman', family: 4, size: 11, bold: true };
    nameSheet2.alignment = { vertical: 'middle', horizontal: 'left' };

    /*---------------End: Sheet 2---------------*/

    /*---------------Begin: Sheet 3---------------*/

    let titleSheet3 = 'Cost center_RSM';
    let worksheet3 = workbook.addWorksheet(titleSheet3);

    /* title */
    let titleSheet = worksheet3.addRow(['', `PAYROL OF ${result.thangNamBatDauKyLuongTiengAnh}`.toUpperCase()]);
    titleSheet.font = { name: 'Times New Roman', family: 4, size: 30, bold: true };
    titleSheet.height = 50;
    worksheet3.mergeCells(`B${titleSheet.number}:T${titleSheet.number}`)
    titleSheet.alignment = { vertical: 'middle', horizontal: 'left' };

    /* Đánh số thứ tự */
    let space = worksheet3.addRow([]);
    for (var i = 1; i < 45; i++) {
      space.getCell(i + 1).value = i;
    }
    space.height = 30;
    space.font = { name: 'Times New Roman', size: 15, bold: true };
    space.alignment = { vertical: 'middle', horizontal: 'center' };

    /* title table 7 */
    let titleTable7Row = worksheet3.addRow(['', 'SUMMARY BY CODE CENTER - RSM']);
    titleTable7Row.font = { name: 'Times New Roman', family: 4, size: 30, bold: true };
    titleTable7Row.height = 50;
    worksheet3.mergeCells(`B${titleTable7Row.number}:K${titleTable7Row.number}`);
    titleTable7Row.alignment = { vertical: 'middle', horizontal: 'left' };

    /* header table 7 */
    let headerTable7_1 = [
      "No.",
      "HR Filing Code",
      "Dept Code",
      "No. of staff",
      "Group Health Care",
      "Monthly Bonus/ Incentive Accrual Local Currency",
      "Actual working days",
      "Actual working days in vocational / Probation",
      "AL",
      "Holiday",
      "Other paid leave with full paid",
      "Unpaid Leave, paid by social innurance",
      "Unpaid leave without reason",
      "Unpaid leave",
      "Deduction days for attendance/ Deduction days for non- compliance of regulation",
      "Total payable day for this month, including Holiday (Mon-Fri)",
      "PAYBACK - GROSS BASIC IF ANY",
      "NEW GROSS BASED ON ACTUAL WORKING DAY",
      "GROSS OT",
      "GROSS ALLOWANCE (Cell phone, Housing allowance, lunch, attendance reward)",
      "GROSS BONUS",
      "NET SEVERANCE ALLOWANCE",
      "DEDUCTION FROM EMPLOYEE/ Refund company before Tax (Over paid AL) Các khoản giảm trừ trước thuế nếu có",
      "TOTAL PIT PAYMENT_Borne by employee",
      "PIT born by company &employee after off-set PIT liquidation",
      "UI",
      "UI",
      "SI",
      "SI",
      "HI",
      "HI",
      "Other HI - Additional",
      "Total company and employee_SI, AI, UI and HI _32%",
      "Total company and employee_SI, AI, UI and HI _32%",
      "Total company and employee_SI, AI, UI and HI _32%",
      "T.U_Company ",
      "Other deduction after Tax",
      "Other Additional after Tax for employee",
      "TOTAL NET INCOME PAYABLE",
      "Net pay before New Year",
      "Net_Paid 26th Jul",
      "TOTAL GROSS PAYABLE - BEFORE OFFSET PIT IF ANY",
      "TOTAL GROSS PAYABLE-AFTER OFFSET PIT IF ANY",
      "Bonus accrual from Jan-Dec",
      "Bonus accrual from Jan-Dec",
    ];

    let headerTable7_1Row = worksheet3.addRow(headerTable7_1);
    let headerTable7_2Row = worksheet3.addRow([]);

    headerTable7_1Row.height = 50;
    headerTable7_1Row.font = { name: 'Times New Roman', size: 12, bold: true };
    headerTable7_2Row.height = 140;
    headerTable7_2Row.font = { name: 'Times New Roman', size: 12, bold: true };

    let excelColumn = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'AA', 'AB', 'AC', 'AD', 'AE', 'AF', 'AG', 'AH', 'AI', 'AJ', 'AK', 'AL', 'AM', 'AN', 'AO', 'AP', 'AQ', 'AR', 'AS', 'AT', 'AU', 'AV', 'AW', 'AX', 'AY', 'AZ'];

    for (var i = 0; i <= 24; i++) {
      worksheet3.mergeCells(`${excelColumn[i]}4:${excelColumn[i]}5`);
    }

    for (var i = 25; i <= 29; i += 2) {
      worksheet3.mergeCells(`${excelColumn[i]}4:${excelColumn[i + 1]}4`);
    }

    worksheet3.mergeCells(`${excelColumn[32]}4:${excelColumn[34]}4`);

    for (var i = 35; i <= 44; i++) {
      worksheet3.mergeCells(`${excelColumn[i]}4:${excelColumn[i]}5`);
    }

    headerTable7_2Row.getCell(26).value = 'Company';
    headerTable7_2Row.getCell(27).value = 'Employee';
    headerTable7_2Row.getCell(28).value = 'Company';
    headerTable7_2Row.getCell(29).value = 'Employee';
    headerTable7_2Row.getCell(30).value = 'Company';
    headerTable7_2Row.getCell(31).value = 'Employee';
    headerTable7_2Row.getCell(32).value = 'Employee';
    headerTable7_2Row.getCell(33).value = 'Company';
    headerTable7_2Row.getCell(34).value = 'Employee';
    headerTable7_2Row.getCell(35).value = 'TOTAL';

    headerTable7_1.forEach((item, index) => {
      headerTable7_1Row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerTable7_1Row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

      if (index == 6 || index == 7 || index == 8 || index == 13 || index == 14) {
        headerTable7_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'F2DCDB' }
        };
      }
      else if (index == 1 || index == 15 || index == 18 || index == 24 || index == 36 || index == 37 || index == 42) {
        headerTable7_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'E4DFEC' }
        };
      }
      else if (index == 9 || index == 10) {
        headerTable7_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'F2F2F2' }
        };
      }
      else if (index == 11) {
        headerTable7_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'DAEEF3' }
        };
      }
      else if (index == 12) {
        headerTable7_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'FDE9D9' }
        };
      }
      else {
        headerTable7_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'EBF1DE' }
        };
      }

      if (index >= 25 && index <= 34) {
        headerTable7_2Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'EBF1DE' }
        };
        headerTable7_2Row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerTable7_2Row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }
    });

    let headerTable7_3Row = worksheet3.addRow([null, null, null, null, null, 'Day', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'AG', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'BY', 'BZ', 'CB,CE', 'CC,CF', 'CI', 'CJ', null, 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'DE', 'DF', 'VND', 'VND', 'VND', 'VND']);
    headerTable7_3Row.height = 40;
    headerTable7_3Row.font = { name: 'Times New Roman', size: 15, bold: true };
    headerTable7_3Row.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    for (var i = 1; i <= 45; i++) {
      headerTable7_3Row.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    }

    /* data table 7 */
    let dataTable7 = result.listDataBaoCao3Bang1.map((item, index) => {
      let row = [];

      row.push(item.col1);
      row.push(item.col2);
      row.push(item.col3);
      row.push(item.col4);
      row.push(item.col5);
      row.push(item.col6);
      row.push(item.col7);
      row.push(item.col8);
      row.push(item.col9);
      row.push(item.col10);
      row.push(item.col11);
      row.push(item.col12);
      row.push(item.col13);
      row.push(item.col14);
      row.push(item.col15);
      row.push(item.col16);
      row.push(item.col17);
      row.push(item.col18);
      row.push(item.col19);
      row.push(item.col20);
      row.push(item.col21);
      row.push(item.col22);
      row.push(item.col23);
      row.push(item.col24);
      row.push(item.col25);
      row.push(item.col26);
      row.push(item.col27);
      row.push(item.col28);
      row.push(item.col29);
      row.push(item.col30);
      row.push(item.col31);
      row.push(item.col32);
      row.push(item.col33);
      row.push(item.col34);
      row.push(item.col35);
      row.push(item.col36);
      row.push(item.col37);
      row.push(item.col38);
      row.push(item.col39);
      row.push(item.col40);
      row.push(item.col41);
      row.push(item.col42);
      row.push(item.col43);
      row.push(item.col44);
      row.push(item.col45);

      return row;
    });

    dataTable7.forEach((el, index) => {
      let row = worksheet3.addRow(el);
      row.height = 30;
      row.font = { name: 'Times New Roman', size: 15 };

      for (let i = 0; i < el.length; i++) {
        row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

        if (i == 0) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        }
        else if (i == 1 || i == 2 || i == 3) {
          row.getCell(i + 1).font = { name: 'Times New Roman', size: 20, bold: true };
        }

        if (i >= 3) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          row.getCell(i + 1).numFmt = '_(* #,##0_);_(* (#,##0);_(* "-"??_);_(@_)';
        }

        //Tính theo công thức
        if (i == 32) {
          row.getCell(i + 1).value = { formula: `=Z${row.number}+AB${row.number}+AD${row.number}`, date1904: false };
        }
        else if (i == 33) {
          row.getCell(i + 1).value = { formula: `=AA${row.number}+AC${row.number}+AE${row.number}+AF${row.number}`, date1904: false };
        }
        else if (i == 34) {
          row.getCell(i + 1).value = { formula: `=AH${row.number}+AG${row.number}`, date1904: false };
        }
      }
    });

    let empty = [];
    for (let i = 0; i <= 44; i++) {
      empty.push('');
    }
    let emptyRow = worksheet3.addRow(empty);
    emptyRow.height = 30;
    emptyRow.font = { name: 'Times New Roman', size: 15 };
    for (let i = 0; i < empty.length; i++) {
      emptyRow.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    }

    let dataTable7Sum = [];
    for (let i = 0; i <= 44; i++) {
      if (i == 1) dataTable7Sum.push('TOTAL');
      else dataTable7Sum.push('');
    }

    let dataTable7SumRow = worksheet3.addRow(dataTable7Sum);
    dataTable7SumRow.height = 35;
    dataTable7SumRow.font = { name: 'Times New Roman', size: 16, bold: true, color: { argb: 'C00000' } };
    for (let i = 0; i < dataTable7Sum.length; i++) {
      dataTable7SumRow.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      dataTable7SumRow.getCell(i + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'FDE9D9' }
      };

      if (i == 1) {
        dataTable7SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      }
      else if (i > 1) {
        dataTable7SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        dataTable7SumRow.getCell(i + 1).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';
      }

      //Tính theo công thức
      if (i > 2) {
        dataTable7SumRow.getCell(i + 1).value = { formula: `=SUM(${this.getHeaderColumn(dataTable7SumRow.getCell(i + 1).$col$row)}7:${this.getHeaderColumn(dataTable7SumRow.getCell(i + 1).$col$row)}10)`, date1904: false };
      }
    }

    let HcNote = ['', 'HC note:', 'Staffs'];
    let HcNoteRow = worksheet3.addRow(HcNote);
    HcNoteRow.height = 30;
    HcNoteRow.font = { name: 'Times New Roman', size: 18, bold: true };
    HcNoteRow.getCell(3).font = { name: 'Times New Roman', size: 18, bold: true, color: { argb: 'C00000' } };

    worksheet3.addRow([]);

    /* header table 8 */
    let headerTable8 = ['', 'Dept Code', 'No. of HC'];
    for (let i = 0; i < 15; i++) {
      headerTable8.push('');
    }
    headerTable8.push('Description');
    headerTable8.push('Before PIT adjustment');
    headerTable8.push('PIT overpaid/under paid');
    headerTable8.push('After PIT adjustment');
    headerTable8.push('Diff');

    let headerTable8Row = worksheet3.addRow(headerTable8);
    headerTable8Row.height = 53;
    headerTable8Row.font = { name: 'Times New Roman', size: 13, bold: true };
    headerTable8Row.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    for (let i = 0; i < headerTable8.length; i++) {
      if (i == 1 || i == 2 || i == 18 || i == 19 || i == 20 || i == 21 || i == 22) {
        headerTable8Row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerTable8Row.getCell(i + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'EBF1DE' }
        }
      }
    }

    let row15 = worksheet3.addRow([]);
    row15.font = { name: 'Times New Roman', size: 13, bold: true };
    row15.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row15.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row15.getCell(19).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row15.getCell(20).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row15.getCell(21).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row15.getCell(22).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row15.getCell(23).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

    row15.getCell(3).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    row15.getCell(20).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    row15.getCell(22).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

    row15.getCell(19).value = 'Total gross-income';
    row15.getCell(20).value = { formula: `=SUM(Q11:V11)-W11`, date1904: false };
    row15.getCell(22).value = { formula: `=T15+U15`, date1904: false };

    let row16 = worksheet3.addRow([]);
    row16.font = { name: 'Times New Roman', size: 13, bold: true };
    row16.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row16.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row16.getCell(19).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row16.getCell(20).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row16.getCell(21).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row16.getCell(22).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row16.getCell(23).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

    row16.getCell(3).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    row16.getCell(20).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    row16.getCell(22).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

    row16.getCell(2).value = 'G&A';
    row16.getCell(3).value = { formula: `=D7`, date1904: false };
    row16.getCell(19).value = 'PIT of the month (EE)';
    row16.getCell(20).value = { formula: `=X11`, date1904: false };
    row16.getCell(22).value = { formula: `=T16+U16`, date1904: false };

    let row17 = worksheet3.addRow([]);
    row17.font = { name: 'Times New Roman', size: 13, bold: true };
    row17.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row17.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row17.getCell(19).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row17.getCell(20).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row17.getCell(21).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row17.getCell(22).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row17.getCell(23).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

    row17.getCell(3).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    row17.getCell(20).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    row17.getCell(22).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

    row17.getCell(2).value = 'OPS';
    row17.getCell(3).value = { formula: `=D8`, date1904: false };
    row17.getCell(19).value = 'PIT of the month (ER)';
    row17.getCell(20).value = { formula: `=Y11-X11`, date1904: false };
    row17.getCell(22).value = { formula: `=T17+U17`, date1904: false };

    let row18 = worksheet3.addRow([]);
    row18.font = { name: 'Times New Roman', size: 13, bold: true };
    row18.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row18.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row18.getCell(19).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row18.getCell(20).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row18.getCell(21).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row18.getCell(22).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row18.getCell(23).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

    row18.getCell(3).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    row18.getCell(20).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    row18.getCell(22).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

    row18.getCell(2).value = 'COS';
    row18.getCell(3).value = { formula: `=D9`, date1904: false };
    row18.getCell(19).value = 'SHUI (EE)';
    row18.getCell(20).value = { formula: `=AH11`, date1904: false };
    row18.getCell(22).value = { formula: `=T18+U18`, date1904: false };

    let row19 = worksheet3.addRow([]);
    row19.font = { name: 'Times New Roman', size: 13, bold: true };

    row19.getCell(2).fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'EBF1DE' }
    }
    row19.getCell(3).fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'EBF1DE' }
    }
    row19.getCell(19).fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'EBF1DE' }
    }
    row19.getCell(20).fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'EBF1DE' }
    }
    row19.getCell(21).fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'EBF1DE' }
    }
    row19.getCell(22).fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'EBF1DE' }
    }
    row19.getCell(23).fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'EBF1DE' }
    }

    row19.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row19.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row19.getCell(19).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row19.getCell(20).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row19.getCell(21).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row19.getCell(22).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    row19.getCell(23).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

    row19.getCell(2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    row19.getCell(3).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    row19.getCell(19).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
    row19.getCell(20).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    row19.getCell(22).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

    row19.getCell(2).value = 'Total HC';
    row19.getCell(3).value = { formula: `=C18+C17+C16+C15`, date1904: false };
    row19.getCell(19).value = 'Net receiving';
    row19.getCell(20).value = { formula: `=T15+AL11-T16-T18`, date1904: false };
    row19.getCell(21).value = { formula: `=SUM(U15:U18)`, date1904: false };
    row19.getCell(22).value = { formula: `=T19+U19`, date1904: false };
    row19.getCell(23).value = { formula: `=SUM(W15:W18)`, date1904: false };

    let row20 = worksheet3.addRow([]);
    row20.alignment = { vertical: 'middle', wrapText: true };
    row20.getCell(19).value = 'Diff';
    row20.getCell(19).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
    row20.getCell(19).font = { name: 'Times New Roman', size: 13, bold: true, color: { argb: 'C00000' } };
    row20.getCell(20).value = { formula: `=T19-AM11`, date1904: false };
    row20.getCell(20).font = { name: 'Times New Roman', size: 13, bold: true, color: { argb: 'C00000' } };

    row16.getCell(2).numFmt = '_(* #,##0_);_(* (#,##0);_(* "-"??_);_(@_)';
    row16.getCell(20).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';
    row16.getCell(22).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';

    row17.getCell(2).numFmt = '_(* #,##0_);_(* (#,##0);_(* "-"??_);_(@_)';
    row17.getCell(20).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';
    row17.getCell(22).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';

    row18.getCell(2).numFmt = '_(* #,##0_);_(* (#,##0);_(* "-"??_);_(@_)';
    row18.getCell(20).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';
    row18.getCell(22).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';

    row19.getCell(2).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';
    row19.getCell(20).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';
    row19.getCell(21).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';
    row19.getCell(22).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';
    row19.getCell(23).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';

    row20.getCell(20).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';

    worksheet3.addRow([]);

    let row21 = worksheet3.addRow(['', 'BỎ TỪ DÒNG 21 TRỞ ĐI TRƯỚC KHI GỬI']);
    row21.font = { name: 'Times New Roman', size: 30, bold: true, color: { argb: 'C00000' } };
    row21.height = 47;

    let row22 = worksheet3.addRow(['', 'SUMMARY BY SUB-COST CENTER']);
    row22.font = { name: 'Times New Roman', size: 30, bold: true };
    row22.height = 47;

    let headerTable9_1 = [
      "No.",
      "HR Filing Code",
      "Dept Code",
      "No. of staff",
      "Group Health Care",
      "Monthly Bonus/ Incentive Accrual Local Currency",
      "Actual working days",
      "Actual working days in vocational / Probation",
      "AL",
      "Holiday",
      "Other paid leave with full paid",
      "Unpaid Leave, paid by social innurance",
      "Unpaid leave without reason",
      "Unpaid leave",
      "Deduction days for attendance/ Deduction days for non- compliance of regulation",
      "Total payable day for this month, including Holiday (Mon-Fri)",
      "PAYBACK - GROSS BASIC IF ANY",
      "NEW GROSS BASED ON ACTUAL WORKING DAY",
      "GROSS OT",
      "GROSS ALLOWANCE (Cell phone, Housing allowance, lunch, attendance reward)",
      "GROSS BONUS",
      "NET SEVERANCE ALLOWANCE",
      "DEDUCTION FROM EMPLOYEE/ Refund company before Tax (Over paid AL) Các khoản giảm trừ trước thuế nếu có",
      "TOTAL PIT PAYMENT_Borne by employee",
      "PIT born by company &employee after off-set PIT liquidation",
      "UI",
      "UI",
      "SI",
      "SI",
      "HI",
      "HI",
      "Other HI - Additional",
      "Total company and employee_SI, AI, UI and HI _32%",
      "Total company and employee_SI, AI, UI and HI _32%",
      "Total company and employee_SI, AI, UI and HI _32%",
      "T.U_Company ",
      "Other deduction after Tax",
      "Other Additional after Tax",
      "TOTAL NET INCOME PAYABLE",
      "Net pay before New Year",
      "Net_Paid 10th Feb",
      "TOTAL GROSS PAYABLE - BEFORE OFFSET PIT LIQUIDATION",
      "TOTAL GROSS PAYABLE-AFTER OFFSET PIT LIQUIDATION",
    ];

    let headerTable9_1Row = worksheet3.addRow(headerTable9_1);
    let headerTable9_2Row = worksheet3.addRow([]);

    headerTable9_1Row.height = 50;
    headerTable9_1Row.font = { name: 'Times New Roman', size: 15, bold: true };
    headerTable9_2Row.height = 140;
    headerTable9_2Row.font = { name: 'Times New Roman', size: 15, bold: true };

    for (var i = 0; i <= 24; i++) {
      worksheet3.mergeCells(`${excelColumn[i]}24:${excelColumn[i]}25`);
    }
    for (var i = 25; i <= 29; i += 2) {
      worksheet3.mergeCells(`${excelColumn[i]}24:${excelColumn[i + 1]}24`);
    }
    worksheet3.mergeCells(`${excelColumn[32]}24:${excelColumn[34]}24`);
    for (var i = 35; i <= 42; i++) {
      worksheet3.mergeCells(`${excelColumn[i]}24:${excelColumn[i]}25`);
    }

    headerTable9_2Row.getCell(26).value = 'Company';
    headerTable9_2Row.getCell(27).value = 'Employee';
    headerTable9_2Row.getCell(28).value = 'Company';
    headerTable9_2Row.getCell(29).value = 'Employee';
    headerTable9_2Row.getCell(30).value = 'Company';
    headerTable9_2Row.getCell(31).value = 'Employee';
    headerTable9_2Row.getCell(32).value = 'Employee';
    headerTable9_2Row.getCell(33).value = 'Company';
    headerTable9_2Row.getCell(34).value = 'Employee';
    headerTable9_2Row.getCell(35).value = 'TOTAL';

    headerTable9_1.forEach((item, index) => {
      headerTable9_1Row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerTable9_1Row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

      if (index == 6 || index == 7 || index == 8 || index == 13 || index == 14) {
        headerTable9_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'F2DCDB' }
        };
      }
      else if (index == 1 || (index >= 17 && index <= 20) || (index >= 23 && index <= 30) || (index >= 35 && index <= 38) || index == 41 || index == 42) {
        headerTable9_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'E4DFEC' }
        };
      }
      else if (index == 9 || index == 10) {
        headerTable9_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'F2F2F2' }
        };
      }
      else if (index == 11) {
        headerTable9_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'DAEEF3' }
        };
      }
      else if (index == 12) {
        headerTable9_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'FDE9D9' }
        };
      }
      else {
        headerTable9_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'EBF1DE' }
        };
      }

      if (index >= 25 && index <= 34) {
        if (index >= 25 && index <= 30) {
          headerTable9_2Row.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'E4DFEC' }
          };
        }
        else {
          headerTable9_2Row.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
        }

        headerTable9_2Row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerTable9_2Row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }
    });

    let headerTable9_3Row = worksheet3.addRow([null, null, null, null, null, 'Day', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', null, 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', null, null, 'VND', 'VND']);
    headerTable9_3Row.height = 40;
    headerTable9_3Row.font = { name: 'Times New Roman', size: 15, bold: true };
    headerTable9_3Row.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    for (var i = 1; i <= 43; i++) {
      headerTable9_3Row.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    }

    /* data table 9 */
    let dataTable9 = result.listDataBaoCao3Bang2.map((item, index) => {
      let row = [];

      row.push(item.col1);
      row.push(item.col2);
      row.push(item.col3);
      row.push(item.col4);
      row.push(item.col5);
      row.push(item.col6);
      row.push(item.col7);
      row.push(item.col8);
      row.push(item.col9);
      row.push(item.col10);
      row.push(item.col11);
      row.push(item.col12);
      row.push(item.col13);
      row.push(item.col14);
      row.push(item.col15);
      row.push(item.col16);
      row.push(item.col17);
      row.push(item.col18);
      row.push(item.col19);
      row.push(item.col20);
      row.push(item.col21);
      row.push(item.col22);
      row.push(item.col23);
      row.push(item.col24);
      row.push(item.col25);
      row.push(item.col26);
      row.push(item.col27);
      row.push(item.col28);
      row.push(item.col29);
      row.push(item.col30);
      row.push(item.col31);
      row.push(item.col32);
      row.push(item.col33);
      row.push(item.col34);
      row.push(item.col35);
      row.push(item.col36);
      row.push(item.col37);
      row.push(item.col38);
      row.push(item.col39);
      row.push(item.col40);
      row.push(item.col41);
      row.push(item.col42);
      row.push(item.col43);

      return row;
    });

    dataTable9.forEach((el, index) => {
      let row = worksheet3.addRow(el);
      row.height = 30;
      row.font = { name: 'Times New Roman', size: 15 };

      for (let i = 0; i < el.length; i++) {
        row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

        if (i == 0) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        }
        else if (i == 1 || i == 2 || i == 3) {
          row.getCell(i + 1).font = { name: 'Times New Roman', size: 20, bold: true };
        }

        if (i >= 3) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          row.getCell(i + 1).numFmt = '_(* #,##0_);_(* (#,##0);_(* "-"??_);_(@_)';
        }

        //Tính theo công thức
        if (i == 32) {
          row.getCell(i + 1).value = { formula: `=Z${row.number}+AB${row.number}+AD${row.number}`, date1904: false };
        }
        else if (i == 33) {
          row.getCell(i + 1).value = { formula: `=AA${row.number}+AC${row.number}+AE${row.number}+AF${row.number}`, date1904: false };
        }
        else if (i == 34) {
          row.getCell(i + 1).value = { formula: `=AH${row.number}+AG${row.number}`, date1904: false };
        }
      }
    });

    let empty2 = [];
    for (let i = 0; i <= 42; i++) {
      empty2.push('');
    }
    let empty2Row = worksheet3.addRow(empty2);
    empty2Row.height = 30;
    empty2Row.font = { name: 'Times New Roman', size: 15 };
    for (let i = 0; i < empty2.length; i++) {
      empty2Row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    }

    let dataTable9Sum = [];
    for (let i = 0; i <= 42; i++) {
      if (i == 1) dataTable9Sum.push('TOTAL');
      else dataTable9Sum.push('');
    }

    let dataTable9SumRow = worksheet3.addRow(dataTable9Sum);
    dataTable9SumRow.height = 35;
    dataTable9SumRow.font = { name: 'Times New Roman', size: 16, bold: true, color: { argb: 'C00000' } };

    for (let i = 0; i < dataTable9Sum.length; i++) {
      dataTable9SumRow.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      dataTable9SumRow.getCell(i + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'FDE9D9' }
      };

      if (i == 1) {
        dataTable9SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      }
      else if (i > 1) {
        dataTable9SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        dataTable9SumRow.getCell(i + 1).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';
      }

      //Tính theo công thức
      if (i > 2) {
        dataTable9SumRow.getCell(i + 1).value = { formula: `=SUM(${this.getHeaderColumn(dataTable9SumRow.getCell(i + 1).$col$row)}27:${this.getHeaderColumn(dataTable9SumRow.getCell(i + 1).$col$row)}36)`, date1904: false };
      }
    }

    worksheet3.addRow([]);
    let row39 = worksheet3.addRow(['', 'SUMMARY BY GRADE - 3D']);
    row39.font = { name: 'Times New Roman', size: 30, bold: true };
    row39.height = 47;

    /* header table 10 */
    let headerTable10_1 = headerTable9_1;
    let headerTable10_1Row = worksheet3.addRow(headerTable10_1);
    let headerTable10_2Row = worksheet3.addRow([]);

    headerTable10_1Row.height = 50;
    headerTable10_1Row.font = { name: 'Times New Roman', size: 15, bold: true };
    headerTable10_2Row.height = 140;
    headerTable10_2Row.font = { name: 'Times New Roman', size: 15, bold: true };

    for (var i = 0; i <= 24; i++) {
      worksheet3.mergeCells(`${excelColumn[i]}40:${excelColumn[i]}41`);
    }
    for (var i = 25; i <= 29; i += 2) {
      worksheet3.mergeCells(`${excelColumn[i]}40:${excelColumn[i + 1]}40`);
    }
    worksheet3.mergeCells(`${excelColumn[32]}40:${excelColumn[34]}40`);
    for (var i = 35; i <= 42; i++) {
      worksheet3.mergeCells(`${excelColumn[i]}40:${excelColumn[i]}41`);
    }

    headerTable10_2Row.getCell(26).value = 'Company';
    headerTable10_2Row.getCell(27).value = 'Employee';
    headerTable10_2Row.getCell(28).value = 'Company';
    headerTable10_2Row.getCell(29).value = 'Employee';
    headerTable10_2Row.getCell(30).value = 'Company';
    headerTable10_2Row.getCell(31).value = 'Employee';
    headerTable10_2Row.getCell(32).value = 'Employee';
    headerTable10_2Row.getCell(33).value = 'Company';
    headerTable10_2Row.getCell(34).value = 'Employee';
    headerTable10_2Row.getCell(35).value = 'TOTAL';

    headerTable10_1.forEach((item, index) => {
      headerTable10_1Row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerTable10_1Row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

      if (index == 6 || index == 7 || index == 8 || index == 13 || index == 14) {
        headerTable10_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'F2DCDB' }
        };
      }
      else if (index == 1 || (index >= 17 && index <= 20) || (index >= 23 && index <= 30) || (index >= 35 && index <= 38) || index == 41 || index == 42) {
        headerTable10_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'E4DFEC' }
        };
      }
      else if (index == 9 || index == 10) {
        headerTable10_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'F2F2F2' }
        };
      }
      else if (index == 11) {
        headerTable10_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'DAEEF3' }
        };
      }
      else if (index == 12) {
        headerTable10_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'FDE9D9' }
        };
      }
      else {
        headerTable10_1Row.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'EBF1DE' }
        };
      }

      if (index >= 25 && index <= 34) {
        if (index >= 25 && index <= 30) {
          headerTable10_2Row.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'E4DFEC' }
          };
        }
        else {
          headerTable10_2Row.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'EBF1DE' }
          };
        }

        headerTable10_2Row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerTable10_2Row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      }
    });

    let headerTable10_3Row = worksheet3.addRow([null, null, null, null, null, 'Day', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', null, 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', 'VND', null, null, 'VND', 'VND']);
    headerTable10_3Row.height = 40;
    headerTable10_3Row.font = { name: 'Times New Roman', size: 15, bold: true };
    headerTable10_3Row.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    for (var i = 1; i <= 43; i++) {
      headerTable10_3Row.getCell(i).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    }

    /* data table 9 */
    let dataTable10 = result.listDataBaoCao3Bang3.map((item, index) => {
      let row = [];

      row.push(item.col1);
      row.push(item.col2);
      row.push(item.col3);
      row.push(item.col4);
      row.push(item.col5);
      row.push(item.col6);
      row.push(item.col7);
      row.push(item.col8);
      row.push(item.col9);
      row.push(item.col10);
      row.push(item.col11);
      row.push(item.col12);
      row.push(item.col13);
      row.push(item.col14);
      row.push(item.col15);
      row.push(item.col16);
      row.push(item.col17);
      row.push(item.col18);
      row.push(item.col19);
      row.push(item.col20);
      row.push(item.col21);
      row.push(item.col22);
      row.push(item.col23);
      row.push(item.col24);
      row.push(item.col25);
      row.push(item.col26);
      row.push(item.col27);
      row.push(item.col28);
      row.push(item.col29);
      row.push(item.col30);
      row.push(item.col31);
      row.push(item.col32);
      row.push(item.col33);
      row.push(item.col34);
      row.push(item.col35);
      row.push(item.col36);
      row.push(item.col37);
      row.push(item.col38);
      row.push(item.col39);
      row.push(item.col40);
      row.push(item.col41);
      row.push(item.col42);
      row.push(item.col43);

      return row;
    });

    dataTable10.forEach((el, index) => {
      let row = worksheet3.addRow(el);
      row.height = 30;
      row.font = { name: 'Times New Roman', size: 15 };

      for (let i = 0; i < el.length; i++) {
        row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

        if (i == 0) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        }
        else if (i == 1 || i == 2 || i == 3) {
          row.getCell(i + 1).font = { name: 'Times New Roman', size: 20, bold: true };
        }

        if (i >= 3) {
          row.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          row.getCell(i + 1).numFmt = '_(* #,##0_);_(* (#,##0);_(* "-"??_);_(@_)';
        }

        //Tính theo công thức
        if (i == 32) {
          row.getCell(i + 1).value = { formula: `=Z${row.number}+AB${row.number}+AD${row.number}`, date1904: false };
        }
        else if (i == 33) {
          row.getCell(i + 1).value = { formula: `=AA${row.number}+AC${row.number}+AE${row.number}+AF${row.number}`, date1904: false };
        }
        else if (i == 34) {
          row.getCell(i + 1).value = { formula: `=AH${row.number}+AG${row.number}`, date1904: false };
        }
      }
    });

    let empty3 = [];
    for (let i = 0; i <= 42; i++) {
      empty3.push('');
    }
    let empty3Row = worksheet3.addRow(empty3);
    empty3Row.height = 30;
    empty3Row.font = { name: 'Times New Roman', size: 15 };
    for (let i = 0; i < empty3.length; i++) {
      empty3Row.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    }

    let dataTable10Sum = [];
    for (let i = 0; i <= 42; i++) {
      if (i == 1) dataTable10Sum.push('TOTAL');
      else dataTable10Sum.push('');
    }

    let dataTable10SumRow = worksheet3.addRow(dataTable10Sum);
    dataTable10SumRow.height = 35;
    dataTable10SumRow.font = { name: 'Times New Roman', size: 16, bold: true, color: { argb: 'C00000' } };

    for (let i = 0; i < dataTable10Sum.length; i++) {
      dataTable10SumRow.getCell(i + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      dataTable10SumRow.getCell(i + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'FDE9D9' }
      };

      if (i == 1) {
        dataTable10SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      }
      else if (i > 1) {
        dataTable10SumRow.getCell(i + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        dataTable10SumRow.getCell(i + 1).numFmt = '_(* #,##0.0_);_(* (#,##0.0);_(* "-"??_);_(@_)';
      }

      //Tính theo công thức
      if (i > 2) {
        dataTable10SumRow.getCell(i + 1).value = { formula: `=SUM(${this.getHeaderColumn(dataTable10SumRow.getCell(i + 1).$col$row)}43:${this.getHeaderColumn(dataTable10SumRow.getCell(i + 1).$col$row)}46)`, date1904: false };
      }
    }

    worksheet3.addRow(['']);

    let labelSheet3 = worksheet3.addRow(['', 'PREPARED BY:']);
    labelSheet3.font = { name: 'Times New Roman', family: 4, size: 20, bold: true };
    labelSheet3.alignment = { vertical: 'middle', horizontal: 'left' };

    worksheet3.addRow(['']);
    worksheet3.addRow(['']);
    worksheet3.addRow(['']);
    worksheet3.addRow(['']);
    worksheet3.addRow(['']);
    worksheet3.addRow(['']);

    let nameSheet3 = worksheet3.addRow(['', 'NGUYEN THI TRANG']);
    nameSheet3.font = { name: 'Times New Roman', family: 4, size: 20, bold: true };
    nameSheet3.alignment = { vertical: 'middle', horizontal: 'left' };

    /*---------------End: Sheet 3---------------*/

    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, 'Báo cáo tổng hợp');
    });
  }

  async xuatBaoCao1() {
    this.loading = true
    let result: any = await this.salaryService.exportExcelKyLuongMealAllowance(1);
    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }
    this.loading = false
    let title = 'Due date contract';
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet(title);

    /* header table */
    let headerBaoCao1 = result.listHeader[0]

    let headerRow1 = worksheet.addRow(headerBaoCao1);

    headerRow1.height = 70;
    headerRow1.font = { name: 'Times New Roman', size: 11, bold: true };

    headerBaoCao1.forEach((item, index) => {
      headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow1.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
    });

    let data: Array<any> = [];
    var server = result.data
    // [
    //   { code: 'RM01', name: 'Nguyen Van Binh', salary: 100000, salary1: 3000000 },
    //   { code: 'RM02', name: 'Nguyen Van Binh2', salary: 200000, salary1: 4000000 },
    //   { code: 'RM03', name: 'Nguyen Van Binh2', salary: 300000, salary1: 5000000 },
    //   { code: '', name: '', salary: 5000000, salary1: 50000002 },
    //   { code: '', name: '', salary: 50000001, salary1: 50000003 },
    // ]

    for (let i = 0; i < server.length; i++) {
      let row: Array<any> = [];
      if (i < server.length - 2) {
        row[0] = i + 1;
        row[1] = server[i].maNhanVien;
        row[2] = server[i].hoTen;
        row[3] = server[i].luongThuViec;
        row[4] = server[i].luongHopDong
        row[5] = '-'
      } else if (i < server.length - 1) {
        row[0] = 'TOTAL'
        row[1] = ''
        row[2] = ''
        row[3] = server[i].luongThuViec;
        row[4] = server[i].luongHopDong
        row[5] = ''
      }
      else {
        row[0] = 'AVERAGE'
        row[1] = ''
        row[2] = ''
        row[3] = server[i].luongThuViec;
        row[4] = server[i].luongHopDong
        row[5] = ''
      }
      data.push(row);
    }

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman', size: 11 };

      if (index + 3 > data.length) {
        row.font = { name: 'Times New Roman', size: 11, bold: true };
      }

      row.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(4).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      row.getCell(6).value = { formula: `=E${row.number}-D${row.number}`, date1904: false };

    });

    worksheet.mergeCells(`A${data.length}:C${data.length}`);
    worksheet.mergeCells(`A${data.length + 1}:C${data.length + 1}`);
    /* fix with for column */
    worksheet.getColumn(1).width = 10;
    worksheet.getColumn(2).width = 15;
    worksheet.getColumn(3).width = 20;
    worksheet.getColumn(4).width = 20;
    worksheet.getColumn(5).width = 20;
    worksheet.getColumn(6).width = 20;

    this.exportToExel(workbook, title);
  }

  async xuatBaoCao2() {
    this.loading = true
    let result: any = await this.salaryService.exportExcelKyLuongMealAllowance(2);
    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }
    this.loading = false

    let title = 'Meal Allowance';
    let workbook = new Workbook();

    let worksheet = workbook.addWorksheet(title);
    let titleTable2Row = worksheet.addRow([``]);
    worksheet.mergeCells(`A1:L1`)

    /* title table 2 */
    titleTable2Row = worksheet.addRow([`BẢNG TÍNH TIỀN ĂN TRƯA TỪ 16/04/2022 ĐẾN 15/05/2022`]);
    titleTable2Row.font = { name: 'Times New Roman', family: 4, size: 13, bold: true };
    titleTable2Row.height = 40;
    titleTable2Row.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A2:L2`)

    titleTable2Row = worksheet.addRow(['BẢNG CÔNG THÁNG 05 NĂM 2022']);
    titleTable2Row.font = { name: 'Times New Roman', family: 4, size: 13, bold: true };
    titleTable2Row.height = 40;
    titleTable2Row.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A3:L3`)
    titleTable2Row = worksheet.addRow([``]);
    worksheet.mergeCells(`A4:L4`)

    /* header table */
    let dataHeaderRow1 = result.listHeader[1]

    let headerRow1 = worksheet.addRow(dataHeaderRow1);
    headerRow1.height = 50;
    headerRow1.font = { name: 'Times New Roman', size: 11, bold: true };

    let dataHeaderRow2 = result.listHeader[2]

    let serverHeader = worksheet.addRow(dataHeaderRow2);
    serverHeader.height = 15;

    dataHeaderRow2.forEach((item, index) => {
      serverHeader.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      serverHeader.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      serverHeader.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
    });

    dataHeaderRow1.forEach((item, index) => {
      headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow1.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };

      if (item.length >= 30) {
        worksheet.getColumn(index + 1).width = 22;
      } else if (item.length <= 30) {
        worksheet.getColumn(index + 1).width = 15;
      }
    });

    worksheet.mergeCells(`A5:A6`)
    worksheet.mergeCells(`B5:B6`)
    worksheet.mergeCells(`C5:C6`)
    worksheet.mergeCells(`D5:D6`)
    worksheet.mergeCells(`E5:E6`)
    worksheet.mergeCells(`G5:G6`)
    worksheet.mergeCells(`K5:K6`)
    worksheet.mergeCells(`L5:L6`)

    let data: Array<any> = [];

    var server = result.data1
    // [
    //   { code: 'RM01', maPhong: 'G&A', name: 'Nguyen Van Binh1', duocTraTienAn: 'Yes', soTienAnTB: 20000, soNgayLamViec: 18.00, soTienAnDuocTra: 510001, soTienAnThieu: 410001, soTienAnThua: 310006, sumTienAnDuocTra: 510000, ghiChu: 'ghi chu 1' },
    //   { code: 'RM02', maPhong: 'G&A1', name: 'Nguyen Van Binh2', duocTraTienAn: 'Yes', soTienAnTB: 10000, soNgayLamViec: 28.00, soTienAnDuocTra: 510002, soTienAnThieu: 410001, soTienAnThua: 310006, sumTienAnDuocTra: 510001, ghiChu: 'ghi chu 2' },
    //   { code: 'RM03', maPhong: 'G&A2', name: 'Nguyen Van Binh3', duocTraTienAn: 'No', soTienAnTB: 30000, soNgayLamViec: 18.00, soTienAnDuocTra: 510003, soTienAnThieu: 410002, soTienAnThua: 310001, sumTienAnDuocTra: 510002, ghiChu: 'ghi chu 3' },
    //   { code: 'RM04', maPhong: 'G&A3', name: 'Nguyen Van Binh4', duocTraTienAn: 'Yes', soTienAnTB: 50000, soNgayLamViec: 28.00, soTienAnDuocTra: 510004, soTienAnThieu: 410002, soTienAnThua: 310006, sumTienAnDuocTra: 510003, ghiChu: 'ghi chu 4' },
    //   { code: 'RM05', maPhong: 'G&A4', name: 'Nguyen Van Binh5', duocTraTienAn: 'Yes', soTienAnTB: 70000, soNgayLamViec: 18.00, soTienAnDuocTra: 510005, soTienAnThieu: 410004, soTienAnThua: 310002, sumTienAnDuocTra: 510004, ghiChu: 'ghi chu 5' },
    // ]

    for (let i = 0; i < server.length; i++) {
      let row: Array<any> = [];
      if (i < server.length - 1) {
        row[0] = i + 1;
        row[1] = server[i].code;
        row[2] = server[i].maPhong;
        row[3] = server[i].name;
        row[4] = server[i].duocTraTienAn
        row[5] = server[i].soTienAnTB
        row[6] = server[i].soNgayLamViec
        row[7] = server[i].soTienAnDuocTra
        row[8] = server[i].soTienAnThieu
        row[9] = server[i].soTienAnThua
        row[10] = server[i].sumTienAnDuocTra
        row[11] = server[i].ghiChu
      }
      else {
        row[0] = '-';
        row[1] = '';
        row[2] = '';
        row[3] = '';
        row[4] = ''
        row[5] = server[i].soTienAnTB
        row[6] = server[i].soNgayLamViec
        row[7] = server[i].soTienAnDuocTra
        row[8] = server[i].soTienAnThieu
        row[9] = server[i].soTienAnThua
        row[10] = server[i].sumTienAnDuocTra
        row[11] = server[i].ghiChu
      }
      data.push(row);
    }

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman', size: 11 };

      if (index + 2 > data.length) {
        row.font = { name: 'Times New Roman', size: 11, bold: true };
      }

      el.forEach((element, index) => {
        row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      });

    });

    let signature = worksheet.addRow([]);
    signature = worksheet.addRow([]);

    signature = worksheet.addRow([`PREPARED BY:`]);
    signature.font = { name: 'Times New Roman', family: 4, size: 14, bold: true };
    signature.height = 40;

    signature = worksheet.addRow([`NGUYEN THI TRANG`]);
    signature.font = { name: 'Times New Roman', family: 4, size: 14, bold: true };
    signature.height = 40;

    this.exportToExel(workbook, title);
  }


  async xuatBaoCao3() {
    this.loading = true
    let result: any = await this.salaryService.exportExcelKyLuongMealAllowance(3);
    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }
    this.loading = false

    let title = 'Trợ cấp chuyên cần';
    let workbook = new Workbook();


    let worksheet = workbook.addWorksheet(title);

    /* title table 2 */
    let titleTable3Row = worksheet.addRow([`BẢNG CÔNG THÁNG 05 NĂM 2022`]);
    titleTable3Row.font = { name: 'Times New Roman', family: 4, size: 13, bold: true };
    titleTable3Row.height = 40;
    titleTable3Row.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A1:P1`)

    /* header table */
    let dataHeaderRow1 = result.listHeader[3]

    let headerRow1 = worksheet.addRow(dataHeaderRow1);
    // headerRow1.height = 50;
    headerRow1.font = { name: 'Times New Roman', size: 11, bold: true };


    let dataHeaderRow2 = result.listHeader[4]

    let headerRow2 = worksheet.addRow(dataHeaderRow2);
    // headerRow2.height = 15;
    headerRow2.font = { name: 'Times New Roman', size: 11, bold: true };


    let dataHeaderRow3 = result.listHeader[5]

    let headerRow3 = worksheet.addRow(dataHeaderRow3);
    // headerRow2.height = 15;
    headerRow3.font = { name: 'Times New Roman', size: 11, bold: true };

    dataHeaderRow3.forEach((item, index) => {
      headerRow3.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow3.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow3.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
      if (item.length > 0) {
        headerRow3.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      }
    });

    dataHeaderRow2.forEach((item, index) => {
      headerRow2.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow2.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
    });

    dataHeaderRow1.forEach((item, index) => {
      headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow1.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };

      if (item.length >= 30) {
        worksheet.getColumn(index + 1).width = 22;
      } else if (item.length <= 30) {
        worksheet.getColumn(index + 1).width = 15;
      }
    });

    //MERGER Header
    worksheet.mergeCells(`A2:A4`)
    worksheet.mergeCells(`B2:B4`)
    worksheet.mergeCells(`C2:C4`)
    worksheet.mergeCells(`D2:D4`)
    worksheet.mergeCells(`E2:E4`)
    worksheet.mergeCells(`F2:F4`)
    worksheet.mergeCells(`G2:K2`)
    worksheet.mergeCells(`L2:O2`)
    worksheet.mergeCells(`P2:P4`)

    worksheet.mergeCells(`G3:G4`)
    worksheet.mergeCells(`H3:H4`)
    worksheet.mergeCells(`I3:I4`)
    worksheet.mergeCells(`J3:J4`)
    worksheet.mergeCells(`L3:L4`)
    worksheet.mergeCells(`M3:M4`)
    worksheet.mergeCells(`N3:N4`)

    let data: Array<any> = [];

    var server = result.data2

    for (let i = 0; i < server.length; i++) {
      let row: Array<any> = [];
      if (i < server.length - 1) {
        row[0] = i + 1;
        row[1] = server[i].maNV;
        row[2] = server[i].maPhong;
        row[3] = server[i].name
        row[4] = server[i].typeOfContact
        row[5] = server[i].chuyenCan
        row[6] = server[i].ngayNghiDotXuat
        row[7] = server[i].ngayNghi
        row[8] = server[i].ngayLamViec
        row[9] = server[i].troCapTheoNgayLam
        row[10] = server[i].troCapChuyenCan
        row[11] = server[i].soLanDMVS
        row[12] = server[i].soNgayLamViec
        row[13] = server[i].troCapDMVS
        row[14] = server[i].troCapChuyenCanNgayCong
        row[15] = server[i].ghiChu
      }
      else {
        row[0] = '';
        row[1] = '';
        row[2] = '';
        row[3] = ''
        row[4] = ''
        row[5] = ''
        row[6] = server[i].ngayNghiDotXuat
        row[7] = server[i].ngayNghi
        row[8] = server[i].ngayLamViec
        row[9] = server[i].troCapTheoNgayLam
        row[10] = server[i].troCapChuyenCan
        row[11] = server[i].soLanDMVS
        row[12] = server[i].soNgayLamViec
        row[13] = server[i].troCapDMVS
        row[14] = server[i].troCapChuyenCanNgayCong
        row[15] = server[i].ghiChu
      }
      data.push(row);
    }

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman', size: 11 };

      if (index + 2 > data.length) {
        row.font = { name: 'Times New Roman', size: 11, bold: true };
      }

      el.forEach((element, index) => {
        row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      });

    });

    let signature = worksheet.addRow([]);
    signature = worksheet.addRow([]);

    signature = worksheet.addRow([`PREPARED BY:`]);
    signature.font = { name: 'Times New Roman', family: 4, size: 14, bold: true };


    signature = worksheet.addRow([`NGUYEN THI TRANG`]);
    signature.font = { name: 'Times New Roman', family: 4, size: 14, bold: true };

    /* fix with for column */
    worksheet.getColumn(1).width = 8;
    worksheet.getColumn(2).width = 10;
    worksheet.getColumn(3).width = 10;
    worksheet.getColumn(4).width = 20;
    worksheet.getColumn(5).width = 10;
    worksheet.getColumn(6).width = 10;

    worksheet.getColumn(7).width = 10;
    worksheet.getColumn(8).width = 10;
    worksheet.getColumn(9).width = 10;
    worksheet.getColumn(10).width = 10;
    worksheet.getColumn(11).width = 10;

    worksheet.getColumn(12).width = 10;
    worksheet.getColumn(13).width = 10;
    worksheet.getColumn(14).width = 10;
    worksheet.getColumn(15).width = 10;
    worksheet.getColumn(16).width = 15;


    this.exportToExel(workbook, title);
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  getHeaderColumn(colrow: string) {
    return colrow.slice(1, colrow.lastIndexOf('$'));
  }

  async capNhatBangLuong() {
    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.salaryService.capNhatBangLuong(this.kyLuongId);

    if (result.statusCode != 200) {
      this.loading = false;
      this.awaitResult = false;
      this.showMessage("error", result.messageCode);
      return;
    }

    await this.getData();
    this.showMessage("success", result.messageCode);
  }

  back() {
    this.router.navigate(['/salary/ky-luong-list']);
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: "Thông báo:", detail: detail };
    this.messageService.add(msg);
  }

}
