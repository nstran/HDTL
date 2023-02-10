import { CauHinhQuyTrinh } from './../../../models/cau-hinh-quy-trinh.model';
import { QuyTrinh } from './../../../models/quy-trinh.model';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Router, ActivatedRoute } from '@angular/router';
import { ThemQuyTrinhPheDuyetComponent } from './../them-quy-trinh-phe-duyet/them-quy-trinh-phe-duyet.component';
import { DialogService } from 'primeng/dynamicdialog';
import { QuyTrinhService } from './../../../services/quy-trinh.service';
import {
  ToolbarService, LinkService, ImageService,
  HtmlEditorService, TableService, RichTextEditorComponent,
  ImageSettingsModel
} from '@syncfusion/ej2-angular-richtexteditor';

@Component({
  selector: 'app-tao-moi-quy-trinh',
  templateUrl: './tao-moi-quy-trinh.component.html',
  styleUrls: ['./tao-moi-quy-trinh.component.css'],
  providers: [ToolbarService, LinkService, ImageService, HtmlEditorService, TableService]
})
export class TaoMoiQuyTrinhComponent implements OnInit {
  loading: boolean = false;
  awaitRes: boolean = false;
  defaultNumberType = 2;
  listDoiTuongApDung: Array<any> = [
    { name: 'Cơ hội', value: 1 },
    { name: 'Hồ sơ thầu', value: 2 },
    { name: 'Báo giá', value: 3 },
    { name: 'Hợp đồng', value: 4 },
    { name: 'Đơn hàng bán', value: 5 },
    { name: 'Hóa đơn', value: 6 },
    { name: 'Đề xuất mua hàng', value: 7 },
    { name: 'Đơn hàng mua', value: 8 },
    { name: 'Đề xuất xin nghỉ', value: 9 },
    { value: 10, name: "Đề xuất tăng lương" },
    { value: 11, name: "Đề xuất chức vụ" },
    { value: 12, name: "Đề xuất kế hoạch OT" },
    { value: 13, name: "Đăng ký OT" },
    { value: 14, name: "Kỳ lương" },
    { value: 20, name: "Yêu cầu cấp phát" },
    { value: 21, name: "Đề nghị tạm ứng" },
    { value: 22, name: "Đề nghị hoàn ứng" },
    { value: 30, name: "Đề xuất công tác" },
    { value: 31, name: "Phê duyệt phát sinh" },
    { value: 32, name: "Phê duyệt yêu cầu bổ sung" },
  ];
  nowDate = new Date();
  nguoiTao: string = localStorage.getItem("EmployeeCodeName");
  listCauHinhQuyTrinh: Array<CauHinhQuyTrinh> = [];
  cols: Array<any> = [];

  /* #region: Editor */
  @ViewChild('templateRTE') rteEle: RichTextEditorComponent;
  tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink', 'Image', '|', 'ClearFormat', 'Print',
      'SourceCode', 'FullScreen', '|', 'Undo', 'Redo', 'CreateTable']
  };
  insertImageSettings: ImageSettingsModel = { 
    allowedTypes: ['.jpeg', '.jpg', '.png'], 
    display: 'inline', width: 'auto', 
    height: 'auto', 
    saveFormat: 'Base64', 
    saveUrl: null, 
    path: null, 
  };
  /* #endregion */

  quyTrinhForm: FormGroup;
  tenQuyTrinhControl: FormControl;
  doiTuongApDungControl: FormControl;
  hoatDongControl: FormControl;
  moTaControl: FormControl;

  warnQuyTrinh: boolean = false;

  constructor(
    public location: Location,
    public messageService: MessageService,
    public confirmationService: ConfirmationService,
    public router: Router,
    public route: ActivatedRoute,
    private dialogService: DialogService,
    private quyTrinhService: QuyTrinhService
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.initTable();
  }

  initForm() {
    this.tenQuyTrinhControl = new FormControl(null, [Validators.required]);
    this.doiTuongApDungControl = new FormControl(null, [Validators.required]);
    this.hoatDongControl = new FormControl(false);
    this.moTaControl = new FormControl(null);

    this.quyTrinhForm = new FormGroup({
      tenQuyTrinhControl: this.tenQuyTrinhControl,
      doiTuongApDungControl: this.doiTuongApDungControl,
      hoatDongControl: this.hoatDongControl,
      moTaControl: this.moTaControl
    });
  }

  initTable() {
    this.cols = [
      { field: 'stt', header: 'STT', textAlign: 'center', display: 'table-cell' },
      { field: 'soTienTu', header: 'Số tiền từ', textAlign: 'right', display: 'table-cell' },
      { field: 'tenCauHinh', header: 'Tên cấu hình', textAlign: 'center', display: 'table-cell' },
      { field: 'quyTrinh', header: 'Quy trình', textAlign: 'center', display: 'table-cell' },
      { field: 'actions', header: 'Thao tác', textAlign: 'center', display: 'table-cell' },
    ];
  }

  themCauHinhQuyTrinh() {
    let newCauHinh = new CauHinhQuyTrinh();
    newCauHinh.soTienTu = '0';
    
    this.listCauHinhQuyTrinh = [...this.listCauHinhQuyTrinh, newCauHinh];
    this.checkSoTien();
  }

  themQuyTrinhPheDuyet(rowData: CauHinhQuyTrinh) {
    let ref = this.dialogService.open(ThemQuyTrinhPheDuyetComponent, {
      data: {
        cauHinhQuyTrinh: rowData
      },
      header: 'Thêm quy trình phê duyệt',
      width: '60%',
      baseZIndex: 10001,
      contentStyle: {
        "min-height": "350px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        rowData = result;
      }
    });
  }

  xoaCauHinhQuyTrinh(rowData) {
    this.listCauHinhQuyTrinh = this.listCauHinhQuyTrinh.filter(x => x != rowData);
    this.checkSoTien();
  }

  /*Lấy list Price có giá trị đã được nhập nhiều hơn 1 lần giống nhau*/
  checkSoTien() {
    let listPrice = this.listCauHinhQuyTrinh.map(x => x.soTienTu.toString());

    let uniq = listPrice
      .map((price) => {
        return {
          count: 1,
          price: price
        }
      })
      .reduce((a, b) => {
        a[b.price] = (a[b.price] || 0) + b.count
        return a
      }, {});

    let duplicates = Object.keys(uniq).filter((a) => uniq[a] > 1);

    this.listCauHinhQuyTrinh.forEach(item => {
      if (duplicates.includes(item.soTienTu.toString())) {
        item.error = true;
      }
      else {
        item.error = false;
      }
    });
  }

  async save() {
    if (!this.quyTrinhForm.valid) {
      Object.keys(this.quyTrinhForm.controls).forEach(key => {
        if (!this.quyTrinhForm.controls[key].valid) {
          this.quyTrinhForm.controls[key].markAsTouched();
        }
      });
      return;
    }

    if (this.listCauHinhQuyTrinh.find(x => x.error == true)) {
      this.showMessage('warn', 'Cấu hình phê duyệt không hợp lệ');
      return;
    }

    //Tự động bỏ đi những cấu hình chưa được thêm quy trình
    this.listCauHinhQuyTrinh = this.listCauHinhQuyTrinh.filter(x => x.tenCauHinh != null);

    if (this.listCauHinhQuyTrinh.length == 0) {
      this.showMessage('warn', 'Bạn cần thêm Cấu hình phê duyệt');
      return;
    }

    let quyTrinh = this.mapDataToModel_QuyTrinh();
    let listCauHinhQuyTrinh = this.mapDataToModel_CauHinhQuyTrinh();

    //Nếu trạng thái là Hoạt động thì check
    if (quyTrinh.hoatDong) {
      let result: any = await this.quyTrinhService.checkTrangThaiQuyTrinh(quyTrinh.doiTuongApDung, quyTrinh.id);
      
      if (result.statusCode == 200) {
        //Nếu đối tượng đã có quy trình đang đc áp dụng
        if (result.exists) {
          this.warnQuyTrinh = true;
        }
        //Nếu đối tượng chưa có quy trình áp dụng => Tạo quy trình
        else {
          this.createQT(quyTrinh, listCauHinhQuyTrinh, quyTrinh.hoatDong);
        }
      }
      else {
        this.showMessage('error', result.messageCode);
      }
    }
    //Nếu trạng thái là Không hoạt động thì => Tạo mới quy trình
    else {
      this.createQT(quyTrinh, listCauHinhQuyTrinh, quyTrinh.hoatDong);
    }
  }

  /* Tạo quy trình với trạng thái Hoạt động */
  chapNhan() {
    this.warnQuyTrinh = false;

    let quyTrinh = this.mapDataToModel_QuyTrinh();
    let listCauHinhQuyTrinh = this.mapDataToModel_CauHinhQuyTrinh();
    this.createQT(quyTrinh, listCauHinhQuyTrinh, quyTrinh.hoatDong);
  }

  /* Tạo quy trình với trạng thái Không hoạt động */
  khongChapNhan() {
    this.warnQuyTrinh = false;

    let quyTrinh = this.mapDataToModel_QuyTrinh();
    let listCauHinhQuyTrinh = this.mapDataToModel_CauHinhQuyTrinh();
    this.createQT(quyTrinh, listCauHinhQuyTrinh, false);
  }

  createQT(quyTrinh: QuyTrinh, listCauHinhQuyTrinh: Array<CauHinhQuyTrinh>, hoatDong: boolean) {
    quyTrinh.hoatDong = hoatDong;

    this.loading = true;
    this.awaitRes = true;
    this.quyTrinhService.createQuyTrinh(quyTrinh, listCauHinhQuyTrinh).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.showMessage('success', result.messageCode);

        setTimeout(() => {
          this.router.navigate(['admin/chi-tiet-quy-trinh', { Id: result.id }]);
        }, 1000);
      }
      else {
        this.showMessage('error', result.messageCode);
      }
    });
  }

  mapDataToModel_QuyTrinh() {
    let quyTrinh = new QuyTrinh();
    quyTrinh.tenQuyTrinh = this.tenQuyTrinhControl.value?.trim();
    quyTrinh.doiTuongApDung = this.doiTuongApDungControl.value.value;
    quyTrinh.hoatDong = this.hoatDongControl.value;
    quyTrinh.moTa = this.moTaControl.value;

    return quyTrinh;
  }

  mapDataToModel_CauHinhQuyTrinh() {
    let listCauHinhQuyTrinh = this.listCauHinhQuyTrinh.map(item => Object.assign({}, item));
    listCauHinhQuyTrinh.forEach(item => {
      item.soTienTu = ParseStringToFloat(item.soTienTu.toString());
    });
    
    return listCauHinhQuyTrinh;
  }

  goBack() {
    this.location.back();
  }

  showMessage(severity: string, detail: string) {
    this.messageService.add({severity: severity, summary: 'Thông báo:', detail: detail});
  }
}

function ParseStringToFloat(str: any) {
  if (str === "") return 0;
  str = String(str).replace(/,/g, '');
  return parseFloat(str);
}
