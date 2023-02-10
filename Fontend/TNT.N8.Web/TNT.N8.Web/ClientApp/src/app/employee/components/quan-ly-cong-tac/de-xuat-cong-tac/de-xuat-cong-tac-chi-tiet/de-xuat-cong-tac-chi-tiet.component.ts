import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageService, ConfirmationService } from 'primeng/api';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ImageUploadService } from '../../../../../shared/services/imageupload.service';
import { FileUpload } from 'primeng/fileupload';
import { FileUploadModel, Note } from '../../../../models/dexuat-congtac.Model';
import { FileInFolder, NoteDocumentModel } from '../../../../models/dexuat-congtac.Model';
import { QuyTrinhService } from '../../../../../admin/services/quy-trinh.service';
import { QuanLyCongTacService } from '../../../../../employee/services/quan-ly-cong-tac/quan-ly-cong-tac.service';
import { GetPermission } from '../../../../../../app/shared/permission/get-permission';
import { DeXuatCongTacModel } from '../../../../../../app/employee/models/de-xuat-cong-tac';
import { DeXuatCongTacChiTietModel } from '../../../../../../app/employee/models/de-xuat-cong-tac-chi-tiet';
import { EmployeeService } from '../../../../../../app/employee/services/employee.service';
import { NoteService } from '../../../../../../app/shared/services/note.service';
import { ExportFileWordService } from '../../../../../shared/services/exportFileWord.services';
import { EncrDecrService } from '../../../../../shared/services/encrDecr.service';



class EmployeeModel {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  employeeLastname: string;
  employeeFirstname: string;
  startedDate: Date;
  organizationId: string;
  positionId: string;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  active: Boolean;
  username: string;
  identity: string;
  organizationName: string;
  avatarUrl: string;
  positionName: string;
  contactId: string;
  isManager: boolean;
  permissionSetId: string;
  probationEndDate: Date;
  probationStartDate: Date;
  trainingStartDate: Date;
  contractType: string;
  contractEndDate: Date;
  isTakeCare: boolean;
  isXuLyPhanHoi: boolean;
  organizationLevel: number;
  dateOfBirth: Date;
  constructor() {
    this.organizationName = "";
    this.dateOfBirth = null;
    this.employeeCode = "";
    this.positionName = "";
    this.identity = "";
  }
}

class NoteDocument {
  active: boolean;
  base64Url: string;
  createdById: string;
  createdDate: Date;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  noteDocumentId: string;
  noteId: string;
  updatedById: string;
  updatedDate: Date;
}

class NoteModel {
  NoteId: string;
  NoteTitle: string;
  Description: string;
  Type: string;
  ObjectId: string;
  ObjectNumber: number;
  ObjectType: string;
  Active: Boolean;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  constructor() { }
}
@Component({
  selector: 'app-de-xuat-cong-tac-chi-tiet',
  templateUrl: './de-xuat-cong-tac-chi-tiet.component.html',
  styleUrls: ['./de-xuat-cong-tac-chi-tiet.component.css']
})


export class DeXuatCongTacChiTietComponent implements OnInit {
  /*Điều kiện hiển thị các button*/
  isShowGuiPheDuyet: boolean = false;
  isShowPheDuyet: boolean = false;
  isShowTuChoi: boolean = false;
  isShowLuu: boolean = false;
  isShowXoa: boolean = false;
  isShowDatVeMoi: boolean = false;
  trangThai: number = null;
  trangThaiString: string = null;
  /*End*/

  /* Action*/
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  /*END*/

  listFileResult: Array<FileInFolder> = [];
  @ViewChild('fileUpload') fileUpload: FileUpload;
  currentEmployeeCodeName = localStorage.getItem('EmployeeCodeName');
  auth = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;

  uploadedFiles: any[] = [];
  colsFile: any[];
  arrayDocumentModel: Array<any> = [];
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';

  //#region   QUY TRÌNH PHÊ DUYỆT
  listFormatStatusSupport: Array<any> = []; // Thanh trạng thái
  showLyDoTuChoi: boolean = false;
  tuChoiForm: FormGroup;
  lyDoTuChoiControl: FormControl;

  //#endregion
  colsListEmp: any[];
  selectedColumns: any[];
  listEmpDefault: any[];
  listNhanVienDeXuatCongTac: any[]; // List nhân viên chọn để thêm đề xuất
  listEmp: any[]; // Danh sách nhân viên trả về
  isUpdate: boolean = false;
  emptyGuid = '00000000-0000-0000-0000-000000000000';
  deXuatCongTacId: number = 0;
  chiTietDeXuatCongTacId: number = 0;
  today = new Date();

  employeeModel: EmployeeModel = new EmployeeModel();
  deXuatCongTac: DeXuatCongTacModel = new DeXuatCongTacModel();
  listDeXuatCongTacChiTietTemp: Array<DeXuatCongTacChiTietModel> = new Array<DeXuatCongTacChiTietModel>();
  awaitResult: boolean = false;

  //Form thông tin đề xuất
  thongTinDeXuatFormGroup: FormGroup;
  tenDeXuatFormControl: FormControl;
  ngayDeXuatFormControl: FormControl;
  nguoiDeXuatFormControl: FormControl;

  //Form của phiếu đề xuất công tác
  deXuatCongTacFormGroup: FormGroup;
  donViFormControl: FormControl;
  diaDiemFormControl: FormControl;
  phuongTienFormControl: FormControl;
  nhiemVuFormControl: FormControl;
  thoiGianBatDauFormControl: FormControl;
  thoiGianKetThucFormControl: FormControl;

  //Ds nhân viên được đề xuất
  nhanVienFormGroup: FormGroup;
  nhanVienControl: FormControl;
  lyDoControl: FormControl;

  //Note
  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  listUpdateNoteDocument: Array<NoteDocument> = [];
  noteHistory: Array<Note> = [];
  isAprovalQuote: boolean = false;
  noteId: string = null;
  noteContent: string = '';
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  uploadedNoteFiles: any[] = [];
  @ViewChild('fileNoteUpload') fileNoteUpload: FileUpload;
  filterGlobal: string = '';

  refreshNote = 0;

  constructor(
    private imageService: ImageUploadService,
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private def: ChangeDetectorRef,
    private confirmationService: ConfirmationService,
    private quyTrinhService: QuyTrinhService,
    private quanLyCongtacService: QuanLyCongTacService,
    private getPermission: GetPermission,
    private employeeService: EmployeeService,
    private noteService: NoteService,
    public exportFileWordService: ExportFileWordService,
    private encrDecrService: EncrDecrService,
  ) { }

  async ngOnInit() {
    this.setForm();
    this.setTable();
    let resource = "hrm/employee/de-xuat-cong-tac-chi-tiet/";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.showToast('warn', 'Thông báo', 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ');
    this.router.navigate(['/home']);
    } else {

      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("view") == -1) {
        this.showToast('warn', 'Thông báo', 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ');
        this.router.navigate(['/home']);
      }
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
    }

    this.route.params.subscribe(params => {
      this.deXuatCongTacId = Number(this.encrDecrService.get(params['deXuatCongTacId']));
    });
    this.getMasterData();
    //  }
  }

  setForm() {
    this.tenDeXuatFormControl = new FormControl(null, [Validators.required]);
    this.ngayDeXuatFormControl = new FormControl(null, [Validators.required]);
    this.nguoiDeXuatFormControl = new FormControl({ value: null, disabled: true });

    this.thongTinDeXuatFormGroup = new FormGroup({
      tenDeXuatFormControl: this.tenDeXuatFormControl,
      ngayDeXuatFormControl: this.ngayDeXuatFormControl,
      nguoiDeXuatFormControl: this.nguoiDeXuatFormControl
    });

    this.nhanVienControl = new FormControl(null, [Validators.required]);
    this.lyDoControl = new FormControl(null, [Validators.required]);

    this.nhanVienFormGroup = new FormGroup({
      nhanVienControl: this.nhanVienControl,
      lyDoControl: this.lyDoControl,
    });

    this.lyDoTuChoiControl = new FormControl(null, [Validators.required]);
    this.tuChoiForm = new FormGroup({
      lyDoTuChoiControl: this.lyDoTuChoiControl,
    });

    this.donViFormControl = new FormControl(null, [Validators.required]);
    this.diaDiemFormControl = new FormControl(null);
    this.phuongTienFormControl = new FormControl(null);
    this.nhiemVuFormControl = new FormControl(null);
    this.thoiGianBatDauFormControl = new FormControl(null, [Validators.required]);
    this.thoiGianKetThucFormControl = new FormControl(null, [Validators.required]);

    this.deXuatCongTacFormGroup = new FormGroup({
      donViFormControl: this.donViFormControl,
      diaDiemFormControl: this.diaDiemFormControl,
      phuongTienFormControl: this.phuongTienFormControl,
      nhiemVuFormControl: this.nhiemVuFormControl,
      thoiGianBatDauFormControl: this.thoiGianBatDauFormControl,
      thoiGianKetThucFormControl: this.thoiGianKetThucFormControl,
    });

    this.ngayDeXuatFormControl.setValue(new Date());
  }

  setTable() {
    this.colsListEmp = [
      { field: 'maNV', header: 'Mã nhân viên', textAlign: 'left', display: 'table-cell', width: "125px" },
      { field: 'tenNhanVien', header: 'Họ tên', textAlign: 'left', display: 'table-cell', width: "150px" },
      { field: 'phongBan', header: 'Phòng ban', textAlign: 'left', display: 'table-cell', width: '200px' },
      { field: 'positionName', header: 'Chức vụ', textAlign: 'center', display: 'table-cell', width: '95px' },
      { field: 'dateOfBirth', header: 'Ngày sinh', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'identity', header: 'CMTND', textAlign: 'left', display: 'table-cell', width: '160px' },
      { field: 'lyDo', header: 'Lý do', textAlign: 'left', display: 'table-cell', width: '160px' },
    ];
    this.selectedColumns = this.colsListEmp;
    this.colsFile = [
      { field: 'fileFullName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'center', type: 'number' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'center', type: 'date' },
      { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'center', type: 'string' },
    ];
  }

  async getMasterData() {

    this.loading = true;
    this.awaitResult = true;
    await this.getDuLieuQuyTrinh();

    let result: any = await this.quanLyCongtacService.getMasterDeXuatCongTacDetail(this.deXuatCongTacId);
    this.loading = false;
    this.awaitResult = false;

    if (result.statusCode != 200) {
      this.showToast('error', 'Thông báo', result.messageCode);
      return;
    }

    if (result.statusCode == 200) {
      this.listEmp = result.listEmployee;
      this.listEmpDefault = result.listEmployee;

      this.mapDataFromModelToForm(result);
      this.arrayDocumentModel = result.listFileInFolder;
      this.def.detectChanges();
    }

  }

  mapDataFromModelToForm(resultDetail: any) {
    this.isShowGuiPheDuyet = resultDetail.isShowGuiPheDuyet;
    this.isShowPheDuyet = resultDetail.isShowPheDuyet;
    this.isShowTuChoi = resultDetail.isShowTuChoi;
    this.isShowLuu = resultDetail.isShowLuu;
    this.isShowXoa = resultDetail.isShowXoa;
    this.isShowDatVeMoi = resultDetail.isShowDatVeMoi;

    this.trangThai = resultDetail.deXuatCongTac.trangThai;
    this.trangThaiString = resultDetail.deXuatCongTac.trangThaiString;

    // Tên đề xuất
    this.deXuatCongTac.tenDeXuat = resultDetail.deXuatCongTac.tenDeXuat;
    this.tenDeXuatFormControl.setValue(this.deXuatCongTac.tenDeXuat)

    // Người đề xuất
    let nguoiDeXuat = this.listEmp.find(x => x.employeeId == resultDetail.deXuatCongTac.nguoiDeXuatId);
    if (nguoiDeXuat != null && nguoiDeXuat != undefined) {
      this.deXuatCongTac.nguoiDeXuat = nguoiDeXuat.employeeName;
      this.deXuatCongTac.nguoiDeXuatId = nguoiDeXuat.employeeId;

      this.nguoiDeXuatFormControl.setValue(nguoiDeXuat)
    }

    // Mã yêu cầu
    this.deXuatCongTac.maDeXuat = resultDetail.deXuatCongTac.maDeXuat;

    // Ngày đề xuất
    let ngayDeXuat = resultDetail.deXuatCongTac.ngayDeXuat ? new Date(resultDetail.deXuatCongTac.ngayDeXuat) : null;
    this.deXuatCongTac.ngayDeXuat = ngayDeXuat;
    this.ngayDeXuatFormControl.setValue(ngayDeXuat)

    // Đề xuất công tác
    this.deXuatCongTac.deXuatCongTacId = resultDetail.deXuatCongTac.deXuatCongTacId;

    // Danh sách tài sản yêu cầu
    this.listDeXuatCongTacChiTietTemp = resultDetail.deXuatCongTac.listDeXuatCongTacChiTiet;

    // Thông tin đơn vị công tác
    this.donViFormControl.setValue(resultDetail.deXuatCongTac.donVi);
    this.diaDiemFormControl.setValue(resultDetail.deXuatCongTac.diaDiem);
    this.phuongTienFormControl.setValue(resultDetail.deXuatCongTac.phuongTien);
    this.nhiemVuFormControl.setValue(resultDetail.deXuatCongTac.nhiemVu);

    let ngayBatDau = resultDetail.deXuatCongTac.thoiGianBatDau ? new Date(resultDetail.deXuatCongTac.thoiGianBatDau) : null;
    this.thoiGianBatDauFormControl.setValue(ngayBatDau)

    let ngayKetThuc = resultDetail.deXuatCongTac.thoiGianKetThuc ? new Date(resultDetail.deXuatCongTac.thoiGianKetThuc) : null;
    this.thoiGianKetThucFormControl.setValue(ngayKetThuc)

    if (this.trangThai != 1) {
      this.nhanVienFormGroup.disable();
      this.deXuatCongTacFormGroup.disable();
      this.thongTinDeXuatFormGroup.disable();
    }
    if (this.trangThai == 1) {
      this.nhanVienFormGroup.enable();
      this.deXuatCongTacFormGroup.enable();
      this.thongTinDeXuatFormGroup.enable();
    }

    this.def.detectChanges();
  }

  thayDoiNhanVien(event: any) {
    let nhanVien = this.listEmp.find(x => x.employeeId == event.value.employeeId);
    this.employeeModel = nhanVien;
    this.employeeModel.dateOfBirth = new Date(this.employeeModel.dateOfBirth)
  }

  refreshForm() {
    this.chiTietDeXuatCongTacId = 0;
    this.nhanVienFormGroup.reset();
    this.employeeModel = new EmployeeModel();
    this.isUpdate = false;
  }

  reShowDeXuatChiTiet(rowData) {
    this.isUpdate = true;

    this.chiTietDeXuatCongTacId = rowData.chiTietDeXuatCongTacId;

    // Mô tả
    this.nhanVienFormGroup.controls['lyDoControl'].setValue(rowData.lyDo);

    // Mã nhân viên
    let nhanVien = this.listEmpDefault.find(c => c.employeeId == rowData.employeeId);
    this.listEmp.push(nhanVien)
    this.def.detectChanges();

    this.employeeModel = nhanVien;

    this.nhanVienFormGroup.controls['nhanVienControl'].updateValueAndValidity();
    this.nhanVienFormGroup.controls['lyDoControl'].updateValueAndValidity();
    this.nhanVienFormGroup.controls['nhanVienControl'].setValue(nhanVien);
  }

  mapFormToChiTietDXCongTacModelTemp(isUpdate: boolean = false): DeXuatCongTacChiTietModel {
    let deXuatChiTietModel = new DeXuatCongTacChiTietModel();
    deXuatChiTietModel.chiTietDeXuatCongTacId = this.chiTietDeXuatCongTacId;
    if (!isUpdate) {
      const maxValueOfY = Math.max(...this.listDeXuatCongTacChiTietTemp.map(o => o.chiTietDeXuatCongTacId), 0);
      deXuatChiTietModel.chiTietDeXuatCongTacId = maxValueOfY + 1;
    }

    // Nhân viên
    let nhanVien = this.nhanVienFormGroup.get('nhanVienControl').value
    if (nhanVien != null && nhanVien != undefined) {
      deXuatChiTietModel.employeeId = nhanVien ? nhanVien.employeeId : this.emptyGuid;
      deXuatChiTietModel.tenNhanVien = nhanVien.employeeName;
      deXuatChiTietModel.phongBan = nhanVien.organizationName;
      deXuatChiTietModel.viTriLamViec = nhanVien.positionName;
      deXuatChiTietModel.identity = nhanVien.identity;
      deXuatChiTietModel.dateOfBirth = nhanVien.dateOfBirth;
    }

    // Lý do
    deXuatChiTietModel.lyDo = this.lyDoControl.value;
    deXuatChiTietModel.createdById = this.auth.UserId;
    deXuatChiTietModel.createdDate = convertToUTCTime(new Date());
    deXuatChiTietModel.updatedById = null;
    deXuatChiTietModel.updatedDate = null;

    return deXuatChiTietModel;
  }

  async themNhanVienCongTac() {
    if (!this.nhanVienFormGroup.valid) {
      Object.keys(this.nhanVienFormGroup.controls).forEach(key => {
        if (!this.nhanVienFormGroup.controls[key].valid) {
          this.nhanVienFormGroup.controls[key].markAsTouched();
        }
      });
      return;
    }

    let deXuatCTModel = this.mapFormToChiTietDXCongTacModelTemp();

    let exists = this.listDeXuatCongTacChiTietTemp.find(x => x.employeeId == deXuatCTModel.employeeId);
    if (exists) {
      this.showToast('error', 'Thông báo', 'Nhân viên này đã được thêm trong danh sách');
      return;
    }

    deXuatCTModel.chiTietDeXuatCongTacId = 0;
    deXuatCTModel.deXuatCongTacId = this.deXuatCongTacId;

    this.loading = true;
    let result: any = await this.quanLyCongtacService.createOrUpdateChiTietDeXuatCongTac(deXuatCTModel, this.auth.UserId);
    this.loading = false;

    if (result.statusCode === 200) {
      this.refreshForm();
      this.listDeXuatCongTacChiTietTemp = result.listDeXuatCongTacChiTietTemp;
      this.showToast('success', 'Thông báo', "Thêm mới thành công");
      this.def.detectChanges();
    }
    else
      this.showToast('error', 'Thông báo', result.messageCode);
  }

  async capNhatNhanVienCongTac() {
    if (!this.nhanVienFormGroup.valid) {
      Object.keys(this.nhanVienFormGroup.controls).forEach(key => {
        if (!this.nhanVienFormGroup.controls[key].valid) {
          this.nhanVienFormGroup.controls[key].markAsTouched();
        }
      });
    }
    else {
      let deXuatCTModel = this.mapFormToChiTietDXCongTacModelTemp(true);
      deXuatCTModel.deXuatCongTacId = this.deXuatCongTacId;

      this.loading = true;
      let result: any = await this.quanLyCongtacService.createOrUpdateChiTietDeXuatCongTac(deXuatCTModel, this.auth.UserId);
      this.loading = false;

      if (result.statusCode === 200) {
        this.refreshForm();
        this.listDeXuatCongTacChiTietTemp = result.listDeXuatCongTacChiTietTemp;
        this.def.detectChanges();
        this.showToast('success', 'Thông báo', "Cập nhật thành công");
      }
      else
        this.showToast('error', 'Thông báo', result.messageCode);
    }
  }

  delNhanVienTemp(rowData: any) {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn xóa dòng này?`,
      accept: async () => {
        this.loading = true;
        let result: any = await this.quanLyCongtacService.xoaDeXuatCongTacChiTiet(rowData.chiTietDeXuatCongTacId);
        this.loading = false;
        if (result.statusCode === 200) {
          this.listDeXuatCongTacChiTietTemp = this.listDeXuatCongTacChiTietTemp.filter(x => x != rowData);
          this.refreshForm();
          this.showToast('success', 'Thông báo', 'Xóa dữ liệu thành công');
        }
        else
          this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  checkValid(): boolean {

    if (!this.thongTinDeXuatFormGroup.valid) {
      Object.keys(this.thongTinDeXuatFormGroup.controls).forEach(key => {
        if (this.thongTinDeXuatFormGroup.controls[key].valid == false) {
          this.thongTinDeXuatFormGroup.controls[key].markAsTouched();
        }
      });
      this.showToast('warn', 'Thông báo:', 'Vui lòng nhập đầy đủ thông tin các trường dữ liệu.');
      return false;
    }

    if (!this.deXuatCongTacFormGroup.valid) {
      Object.keys(this.deXuatCongTacFormGroup.controls).forEach(key => {
        if (this.deXuatCongTacFormGroup.controls[key].valid == false) {
          this.deXuatCongTacFormGroup.controls[key].markAsTouched();
        }
      });
      this.showToast('warn', 'Thông báo:', 'Vui lòng nhập đầy đủ thông tin các trường dữ liệu.');
      return false;
    }

    if (this.listDeXuatCongTacChiTietTemp.length == 0) {
      this.showToast('warn', 'Thông báo:', 'Hãy chọn nhân viên được đề xuất!');
      return false;
    }

    return true;
  }

  async updateDeXuat() {
    let deXuatModel: DeXuatCongTacModel = this.mapDataFormToModel();
    deXuatModel.deXuatCongTacId = this.deXuatCongTacId;

    this.loading = true;
    this.awaitResult = true;
    let result: any = await this.quanLyCongtacService.createOrUpdateDeXuatCT(deXuatModel, [], 'DEXUATCT', [], this.auth.UserId);

    if (result.statusCode == 200) {
      return {
        status: true,
        message: 'Cập nhật đề xuất công tác thành công'
      }
    }
    else {
      return {
        status: false,
        message: result.messageCode
      }
    }
  }

  async clickUpdate() {
    if (!this.checkValid()) {
      return;
    }

    let result: any = await this.updateDeXuat();

    if (!result.status) {
      this.showToast('error', 'Thông báo:', result.message);
      return;
    }

    if (result.status) {
      this.showToast('success', 'Thông báo:', result.message);
      await this.getMasterData();
    }
  }

  mapDataFormToModel(): DeXuatCongTacModel {
    let formDataModel = new DeXuatCongTacModel();

    formDataModel.deXuatCongTacId = this.deXuatCongTacId;
    formDataModel.tenDeXuat = this.tenDeXuatFormControl.value;
    let ngayDeXuat = this.thongTinDeXuatFormGroup.get('ngayDeXuatFormControl').value ? convertToUTCTime(this.thongTinDeXuatFormGroup.get('ngayDeXuatFormControl').value) : null;
    formDataModel.ngayDeXuat = ngayDeXuat;

    formDataModel.nguoiDeXuatId = this.nguoiDeXuatFormControl.value == null ? this.auth.EmployeeId : this.nguoiDeXuatFormControl.value.employeeId;

    formDataModel.donVi = this.donViFormControl.value;
    formDataModel.diaDiem = this.diaDiemFormControl.value;
    formDataModel.phuongTien = this.phuongTienFormControl.value;
    formDataModel.nhiemVu = this.nhiemVuFormControl.value;

    let ngayBatDau = this.deXuatCongTacFormGroup.get('thoiGianBatDauFormControl').value ? convertToUTCTime(this.deXuatCongTacFormGroup.get('thoiGianBatDauFormControl').value) : null;
    formDataModel.thoiGianBatDau = ngayBatDau;

    let ngayKetThuc = this.deXuatCongTacFormGroup.get('thoiGianKetThucFormControl').value ? convertToUTCTime(this.deXuatCongTacFormGroup.get('thoiGianKetThucFormControl').value) : null;
    formDataModel.thoiGianKetThuc = ngayKetThuc;

    formDataModel.createdById = this.auth.UserId;
    formDataModel.createdDate = convertToUTCTime(new Date());
    formDataModel.updatedById = null;
    formDataModel.updatedDate = null;
    return formDataModel;
  }

  refreshFilter() {
    this.filterGlobal = '';
  }

  taoHoSoCongTac() {
    this.router.navigate(['/employee/tao-ho-so-cong-tac', { deXuatCTId: this.encrDecrService.set(this.deXuatCongTacId) }]);
  }
  //#region QUY TRÌNH PHÊ DUYỆT

  async getDuLieuQuyTrinh() {
    this.quyTrinhService.getDuLieuQuyTrinh(this.emptyGuid, 30, this.deXuatCongTacId).subscribe(res => {
      let result: any = res;
      if (result.statusCode == 200) {

        this.listFormatStatusSupport = result.listDuLieuQuyTrinh;
      }
      else {
        this.showToast('error', 'Thông báo:', result.messageCode);
      }
    });
  }

  // Chuyển yêu cầu về trạng thái mới sau khi bị từ chối phê duyệt
  async datVeMoi() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn sử dụng lại Đề xuất công tác này?',
      accept: async () => {

        let res: any = await this.quanLyCongtacService.datVeMoiDeXuatCongTac(this.deXuatCongTacId);
        if (res.statusCode === 200) {
          this.showToast('success', 'Thông báo:', "Cập nhật đề xuất công tác thành công.");
          this.getMasterData();
        } else {
          this.showToast('error', 'Thông báo:', res.messageCode);
        }
      }
    });
  }

  // Gửi phê duyệt
  async guiPheDuyet() {
    if (!this.checkValid()) {
      return;
    }

    let resultUpdate: any = await this.updateDeXuat();

    if (!resultUpdate.status) {
      this.showToast('error', 'Thông báo:', resultUpdate.message);
      return;
    }

    if (resultUpdate.status) {
      this.loading = true;
      this.awaitResult = true;
      this.quyTrinhService.guiPheDuyet(this.emptyGuid, 30, this.deXuatCongTacId).subscribe(async res => {
        let result: any = res;
        if (result.statusCode == 200) {
          this.showToast('success', 'Thông báo:', "Gửi phê duyệt thành công.");
          await this.getMasterData();
          this.def.detectChanges();
        }
        else {
          this.loading = false;
          this.showToast('error', 'Thông báo:', result.messageCode);
        }
      });
    }
  }

  // Phê duyệt
  async pheDuyet() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắc muốn phê duyệt đề xuất công tác này?',
      accept: async () => {
        this.loading = true;

        this.quyTrinhService.pheDuyet(this.emptyGuid, 30, '', this.deXuatCongTacId).subscribe(res => {
          let result: any = res;

          if (result.statusCode == 200) {
            this.getMasterData();
            this.def.detectChanges();
          }
          else {
            this.loading = false;
            this.showToast('error', 'Thông báo:', result.messageCode);
          }
        });
      }
    });
  }

  openDialogReject() {
    this.showLyDoTuChoi = true;
    this.tuChoiForm.reset();
  }

  // Từ chối phê duyệt
  async tuChoi() {
    if (!this.tuChoiForm.valid) {
      Object.keys(this.tuChoiForm.controls).forEach(key => {
        if (!this.tuChoiForm.controls[key].valid) {
          this.tuChoiForm.controls[key].markAsTouched();
        }
      });
    }
    else {
      this.loading = true;
      this.quyTrinhService.tuChoi(this.emptyGuid, 30, this.lyDoTuChoiControl.value, this.deXuatCongTacId).subscribe(res => {
        let result: any = res;

        if (result.statusCode == 200) {
          this.showToast('success', 'Thông báo:', result.messageCode);

          this.getMasterData();
        }
        else {
          this.loading = false;
          this.showToast('error', 'Thông báo:', result.messageCode);
        }
      });
      this.showLyDoTuChoi = false;
    }
  }

  kiemTraSoLuong(rowdata: any) {
    let soLuongPD = 0;
    soLuongPD = rowdata.soLuongPheDuyet ? parseFloat(rowdata.soLuongPheDuyet.replace(/,/g, '')) : null;

    if (soLuongPD > rowdata.soLuong || soLuongPD <= 0 || soLuongPD == null) {
      rowdata.error = true;
      this.showToast('error', 'Thông báo:', 'Số lượng thực xuất cần lớn hơn 0 và nhỏ hơn hoặc bằng số lượng cần xuất');
      return;
    }
    else {
      rowdata.error = false;
    }
  }

  closeDialog() {
    this.showLyDoTuChoi = false;
  }

  //#endregion

  /*Event Lưu các file được chọn*/
  handleFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;
      if (size <= this.defaultLimitedFileSize) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedFiles.push(file);
          }
        }
      }
    }
  }

  /*Event Khi click xóa từng file*/
  removeFile(event) {
    let index = this.uploadedFiles.indexOf(event.file);
    this.uploadedFiles.splice(index, 1);
  }

  /*Event Khi click xóa toàn bộ file*/
  clearAllFile() {
    this.uploadedFiles = [];
  }

  /*Event upload list file*/
  myUploader(event) {
    let listFileUploadModel: Array<FileUploadModel> = [];
    this.uploadedFiles.forEach(item => {
      let fileUpload: FileUploadModel = new FileUploadModel();
      fileUpload.FileInFolder = new FileInFolder();
      fileUpload.FileInFolder.active = true;
      let index = item.name.lastIndexOf(".");
      let name = item.name.substring(0, index);
      fileUpload.FileInFolder.fileName = name;
      fileUpload.FileInFolder.fileExtension = item.name.substring(index, item.name.length - index);
      fileUpload.FileInFolder.size = item.size;
      fileUpload.FileInFolder.objectNumber = this.deXuatCongTacId;
      fileUpload.FileInFolder.objectType = 'DEXUATCT';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    this.employeeService.uploadFile("DEXUATCT", listFileUploadModel, null, this.deXuatCongTacId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.uploadedFiles = [];

        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }

        this.arrayDocumentModel = result.listFileInFolder;

        this.showToast('success', 'Thông báo', "Thêm file thành công");
      } else {
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: any) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa tài liệu?',
      accept: () => {
        this.employeeService.deleteFile(file.fileInFolderId).subscribe(res => {
          let result: any = res;
          if (result.statusCode == 200) {
            let index = this.arrayDocumentModel.indexOf(file);
            this.arrayDocumentModel.splice(index, 1);
            this.showToast('success', 'Thông báo', 'Xóa file thành công');
          } else {
            this.showToast('success', 'Thông báo', result.messageCode);
          }
        })
      }
    });
  }

  downloadFile(fileInfor: FileInFolder) {

    this.imageService.downloadFile(fileInfor.fileName, fileInfor.fileUrl).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;
      var name = fileInfor.fileFullName;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if ((window.navigator as any) && (window.navigator as any).msSaveOrOpenBlob) {
        (window.navigator as any).msSaveOrOpenBlob(file);
      } else {
        var fileURL = URL.createObjectURL(file);
        if (fileType.indexOf('image') !== -1) {
          window.open(fileURL);
        } else {
          var anchor = document.createElement("a");
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    });
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  cancel() {
    this.router.navigate(['/employee/danh-sach-de-xuat-cong-tac']);
  }


  async exportWord() {
    this.loading = true;

    let result: any = await this.quanLyCongtacService.getMasterDeXuatCongTacDetail(this.deXuatCongTacId);
    this.loading = false;
    if (result.statusCode == 200) {
      let thongTin = result.deXuatCongTac
      let thoiGianBatDau = convertDate(new Date(thongTin.thoiGianBatDau))
      let thoiGianKetThuc = convertDate(new Date(thongTin.thoiGianKetThuc))

      for (let i = 0; i < result.deXuatCongTac.listDeXuatCongTacChiTiet.length; i++) {
        let data = {
          template: 1,
          tenNhanVien: result.deXuatCongTac.listDeXuatCongTacChiTiet[i].tenNhanVien,
          maNV: result.deXuatCongTac.listDeXuatCongTacChiTiet[i].maNV,
          viTriLamViec: result.deXuatCongTac.listDeXuatCongTacChiTiet[i].viTriLamViec,
          dateOfBirth: convertDate(new Date(result.deXuatCongTac.listDeXuatCongTacChiTiet[i].dateOfBirth)),
          identity: result.deXuatCongTac.listDeXuatCongTacChiTiet[i].identity,
          identityIddateOfIssue: convertDate(new Date(result.deXuatCongTac.listDeXuatCongTacChiTiet[i].identityIddateOfIssue)),
          identityIdplaceOfIssue: result.deXuatCongTac.listDeXuatCongTacChiTiet[i].identityIdplaceOfIssue,
          donVi: thongTin.donVi,
          diaDiem: thongTin.diaDiem,
          thoiGianBatDau: thoiGianBatDau,
          thoiGianKetThuc: thoiGianKetThuc,
          phuongTien: thongTin.phuongTien,
          nhiemVu: thongTin.nhiemVu,
          date: new Date().getDate(),
          month: new Date().getMonth() + 1,
          year: new Date().getFullYear(),
        }
        this.exportFileWordService.saveFileWord(data, `Đề xuất công tác - ${data.maNV}_${data.tenNhanVien}.docx`)
      }
    }

  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function convertDate(time: any) {
  let ngay = time.getDate()
  let thang = time.getMonth() + 1
  let nam = time.getFullYear()
  return `${ngay}/${thang}/${nam}`
};

