import { Component, OnInit, ViewChild, ElementRef, Renderer2, HostListener, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import * as $ from 'jquery';

//MODELS
import { Quote } from '../../models/quote.model';
import { QuoteDetail } from "../../models/quote-detail.model";
import { QuoteDocument } from "../../models/quote-document.model";
import { QuoteProductDetailProductAttributeValue } from "../../models/quote-product-detail-product-attribute-value.model";
import { QuotePlanModel } from '../../models/quote-plan.model';
import { QuoteScopeModel } from '../../models/QuoteScopeModel';

//SERVICES
import { BankService } from '../../../shared/services/bank.service';
import { QuoteService } from '../../services/quote.service';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { PromotionService } from '../../../promotion/services/promotion.service';
import { ForderConfigurationService } from '../../../admin/components/folder-configuration/services/folder-configuration.service';
import { QuyTrinhService } from './../../../admin/services/quy-trinh.service';

//DIALOG COMPONENT
import { AddEditProductDialogComponent } from '../add-edit-product-dialog/add-edit-product-dialog.component';
import { GetPermission } from '../../../shared/permission/get-permission';

//
import { MessageService, TreeNode } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { MenuItem } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { ConfirmationService } from 'primeng/api';

import { saveAs } from "file-saver";
import { Workbook, Worksheet } from 'exceljs';
import { DecimalPipe, DatePipe, Location } from '@angular/common';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { NoteModel } from '../../../shared/models/note.model';
import { NoteService } from '../../../shared/services/note.service';
import { ProductService } from '../../../product/services/product.service';

import * as XLSX from 'xlsx';
import * as pdfMake from 'pdfmake/build/pdfmake.js';
import * as pdfFonts from 'pdfmake/build/vfs_fonts.js';
import { SendEmailQuoteComponent } from '../send-mail-popup-quote/send-mail-popup-quote.component';
import { PopupAddEditCostQuoteDialogComponent } from '../../../shared/components/add-edit-cost-quote/add-edit-cost-quote.component';
import { QuoteCostDetail } from '../../models/quote-cost-detail.model';
import { AddQuoteVendorDialogComponent } from '../add-quote-vendor-dialog/add-quote-vendor-dialog.component';
pdfMake.vfs = pdfFonts.pdfMake.vfs;
import { PromotionApplyPopupComponent } from '../../../shared/components/promotion-apply-popup/promotion-apply-popup.component';
import { PromotionObjectApply } from '../../../promotion/models/promotion-object-apply.model';
import { PromotionApply } from '../../../promotion/models/promotion-apply.model';
import { ContactService } from '../../../shared/services/contact.service'
import { QuotePaymentTerm } from '../../models/quote-payment-term.model';
import { LichSuPheDuyetComponent } from './../../../shared/components/lich-su-phe-duyet/lich-su-phe-duyet.component';
import { OverlayPanel } from 'primeng/overlaypanel';

interface Customer {
  customerId: string;
  customerCode: string;
  customerEmail: string;
  customerEmailWork: string;
  customerEmailOther: string;
  customerPhone: string;
  customerCompany: string;
  fullAddress: string;
  maximumDebtDays: number;
  maximumDebtValue: number;
  personInChargeId: string;
  customerGroupId: string;
  customerCodeName: string;
}

interface Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  employeeCodeName: string;
  isManager: boolean;
  positionId: string;
  organizationId: string;
}

interface Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefault: boolean;
}

interface DiscountType {
  name: string;
  code: string;
  value: boolean;
}

interface BankAccount {
  bankAccountId: string;
  bankName: string; //Ngân hàng
  accountNumber: string;  //Số tài khoản
  branchName: string; //Chi nhánh
  accountName: string; //Chủ tài khoản
  labelShow: string; //Text hiển thị trên giao diện
}

interface QuoteDocumentResponse {
  quoteDocumentId: string;
  quoteId: string,
  documentName: string,
  documentSize: string,
  documentUrl: string,
  createdById: string,
  createdDate: Date,
  updatedById: string,
  updatedDate: Date,
  active: boolean,
}

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  quoteDetailModel: QuoteDetail,
}

interface ResultCostDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  quoteDetailModel: QuoteCostDetail,
}

interface AdditionalInformation {
  ordinal: number,
  additionalInformationId: string,
  objectId: string,
  objectType: string,
  title: string,
  content: string,
  active: boolean,
  createdDate: Date,
  createdById: string,
  updatedDate: Date,
  updatedById: string,
  orderNumber: number,
}

interface InforExportExcel {
  companyName: string,
  address: string,
  phone: string,
  website: string,
  email: string,
  textTotalMoney: string
}

interface NoteDocument {
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

interface Note {
  active: boolean;
  createdById: string;
  createdDate: Date;
  description: string;
  noteDocList: Array<NoteDocument>;
  noteId: string;
  noteTitle: string;
  objectId: string;
  objectType: string;
  responsibleAvatar: string;
  responsibleName: string;
  type: string;
  updatedById: string;
  updatedDate: Date;
}

interface FileNameExists {
  oldFileName: string;
  newFileName: string
}

interface QuoteDetailExcel {
  STT: string,
  ProductCode: string,
  ProductName: string,
  Quantity: number,
  UnitPrice: number,
  CurrencyUnit: string,
  Amount: number,
  Tax: number,
  TaxAmount: number,
  DiscountType: boolean,
  DiscountValue: number,
  TotalAmount: number
}

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
  createdById: string;
  createdDate: Date;
  uploadByName: string;
}

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

class StatusSupport {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  children: Array<Category>;
  isComplete: boolean;
  isCurrent: boolean;
  isActive: boolean;

  constructor() {
    this.children = [];
    this.isCurrent = false;
    this.isComplete = false;
    this.isActive = false;
  }
}

@Component({
  selector: 'app-customer-quote-detail',
  templateUrl: './customer-quote-detail.component.html',
  styleUrls: ['./customer-quote-detail.component.css'],
  providers: [
    DecimalPipe,
    DatePipe
  ]
})
export class CustomerQuoteDetailComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới
  innerWidth: number = 0; //number window size first

  fixed: boolean = false;
  withFiexd: string = "";
  withFiexdCol: string = "";
  withColCN: number = 0;
  withCol: number = 0;
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
      var colT = 0;
      if (this.withColCN != width) {
        colT = this.withColCN - width;
        this.withColCN = width;
        this.withCol = $('#parentTH').width();
      }
      this.withFiexdCol = (this.withCol) + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
      this.withCol = $('#parentTH').width();
      this.withColCN = $('#parent').width();
      this.withFiexdCol = "";
    }
  }

  /*Get Global Parameter*/
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  appName = this.systemParameterList.find(x => x.systemKey == 'ApplicationName').systemValueString;
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  isManager: boolean = localStorage.getItem('IsManager') == "true" ? true : false;
  minimumProfit = this.systemParameterList.find(x => x.systemKey == 'MinimumProfitExpect').systemValueString;
  /*End*/

  /*Get Current EmployeeId*/
  auth = JSON.parse(localStorage.getItem('auth'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  /*End*/

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionImport: boolean = true;
  actionDelete: boolean = true;
  actionEdit: boolean = true;
  actionDeleteScope: boolean = false;
  actionEditScope: boolean = false;
  /*End*/

  /*Form Báo giá*/
  scopeCreateForm: FormGroup;
  quoteForm: FormGroup;
  objectControl: FormControl; //Đối tượng báo giá
  paymentMethodControl: FormControl;  //Phương thức thanh toán
  bankAccountControl: FormControl;  //Tài khoản ngân hàng
  daysAreOwedControl: FormControl;  //Số ngày được nợ
  maxDebtControl: FormControl;  //Số nợ tối đa
  quoteStatusControl: FormControl;  //Trạng thái
  intendedDateControl: FormControl; //Ngày gửi dự kiến
  nameQuoteControl: FormControl; //tên báo giá
  sendDateControl: FormControl; //Ngày gửi
  effectiveDateControl: FormControl; //Ngày hiệu lực
  personInChargeControl: FormControl; //Người phụ trách
  hoSoThauControl: FormControl; //Người phụ trách
  coHoiControl: FormControl; //Người phụ trách
  kenhControl: FormControl; //Người phụ trách
  descriptionControl: FormControl;  //Diễn giải
  noteControl: FormControl; //Ghi chú
  discountTypeControl: FormControl; //Loại chiết khấu (% - Số tiền)
  discountValueControl: FormControl; //Giá trị tổng chiết khấu của báo giá
  participantControl: FormControl; //Người tham gia
  vatControl: FormControl;
  percentAdvanceControl: FormControl; //% Tạm ứng
  percentAdvanceTypeControl: FormControl; //Loại Tạm ứng
  createByControl: FormControl; // nguoi tao
  constructionTimeControl: FormControl; // thowi gian thi cong
  scopeCategoryControl: FormControl;
  scopeDescriptionControl: FormControl;
  /*End*/

  /*Valid Form*/
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  @ViewChild('fileNoteUpload') fileNoteUpload: FileUpload;
  /*End*/

  /*Biến điều kiện*/
  colLeft: number = 8;
  isShow: boolean = true;
  displayRejectQuote: boolean = false;
  quoteId: string = null;
  statusCode: string = '';
  createQuoteFollow: number = 0;  //0: Tạo mới bình thường, 1: Tạo mới cho khách hàng, 2: Tạo mới cho khách hàng tiềm năng
  selectedObjectType: string = 'cus';
  isShowBankAccount: boolean = false;
  customerContactCode: string = 'CUS';
  rejectReason: string = 'EMP';
  uploadedFiles: any[] = [];
  cols: any[];
  colsCost: any[];
  colsPlan: any = [];
  colsPaymentTerm: any = [];
  listQuotePlans: Array<QuotePlanModel> = [];
  listQuoteScopes: Array<QuoteScopeModel> = [];
  listQuotePaymentTerm: Array<QuotePaymentTerm> = [];

  data: TreeNode[];
  selectedNode: TreeNode;
  colsScope: any = [];
  selectedColumnsDetail: any[];
  selectedColumnsHistory: any[];
  selectedColumns: any[];
  selectedColumnsCost: any[];
  selectedItem: any;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  colsFile: any[];
  colsNote: any[];
  colsNoteFile: any[];
  maxOrdinal: number = 0;
  AIUpdate: AdditionalInformation = {
    ordinal: null,
    additionalInformationId: this.emptyGuid,
    objectId: this.emptyGuid,
    objectType: '',
    title: '',
    content: '',
    active: true,
    createdDate: new Date(),
    createdById: this.emptyGuid,
    updatedDate: null,
    updatedById: null,
    orderNumber: null,
  };
  listAdditionalInformation: Array<AdditionalInformation> = [];
  titleText: string = '';
  contentText: string = '';
  isUpdateAI: boolean = false;

  listAction: MenuItem[];
  /*End*/

  /*Biến lưu giá trị trả về*/
  listIdCus: Array<string> = [];
  display: boolean = false;
  descriptionReject: string = '';
  workFollowQuote: MenuItem[];
  activeIndex: number = 0;
  isShowWorkFollowQuote: boolean = true;
  objectList = <any>[];
  objCustomer: Customer = null;
  leadObj: any = null;
  listPersonInCharge: Array<Employee> = []; //list người phụ trách
  listHoSoThau: Array<any> = []; //list hồ sơ thầu
  listCoHoi: Array<any> = []; //list cơ hội
  listKenh: Array<any> = []; //list kênh bán hàng
  listAdditionalInformationTemplate: Array<Category> = []; // Thông tin bổ sung mẫu được lưu trong category
  email: string = '';
  minDate: Date = new Date();
  phone: string = '';
  fullAddress: string = '';
  personInChargeId: string = '';
  listBankAccount: Array<BankAccount> = [];

  discountTypeList: Array<DiscountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];
  arrayQuoteDocumentModel: Array<QuoteDocument> = [];
  listQuoteDetailModel: Array<QuoteDetail> = [];
  listQuoteCostDetailModelOrderBy: Array<QuoteCostDetail> = [];
  listQuoteCostDetailModel: Array<QuoteCostDetail> = [];
  listQuoteDetailExcelModel: Array<QuoteDetailExcel> = [];
  listQuoteDetailModelOrderBy: Array<QuoteDetail> = [];
  arrayOrderProductDetailProductAttributeValueModel: Array<QuoteProductDetailProductAttributeValue> = [];
  inforExportExcel: InforExportExcel = {
    companyName: '',
    address: '',
    phone: '',
    website: '',
    email: '',
    textTotalMoney: ''
  }

  messageSendQuote: string = null;
  fileName: string = '';
  importFileExcel: any = null;
  messErrFile: any = [];
  cellErrFile: any = [];
  productCodeSystemList: string[] = [];
  listProductUnitName: string[] = [];
  listProduct: any[] = [];
  listUnitProduct: any[] = [];
  listUnitMoney: Array<Category> = [];
  displayDialog: boolean = false;

  /*End*/

  /*MODELS*/
  quoteModel: Quote = new Quote();

  quoteCostDetailModel: QuoteCostDetail = new QuoteCostDetail();

  quoteDetailModel = new QuoteDetail();

  /*END*/

  /*NOTE*/
  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  listUpdateNoteDocument: Array<NoteDocument> = [];
  noteHistory: Array<Note> = [];
  listFile: Array<FileInFolder> = [];

  isApprovalQuote: boolean = false;
  noteId: string = null;
  noteContent: string = '';
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  uploadedNoteFiles: any[] = [];
  /*End : Note*/

  /* Dialog Phê duyệt */
  displayReasonApprove: boolean = false;
  descriptionApprove: string = '';
  awaitResponseApprove: boolean = false;
  /*End: Dialog Phê duyệt */

  /* Mới */
  personInChargeIdOfQuote: string = null;
  listInvestFund: Array<Category> = []; //list kênh bán hàng
  listPaymentMethod: Array<Category> = []; //list Phương thức thanh toán
  listQuoteStatus: Array<Category> = []; //list Trạng thái của báo giá
  listEmployee: Array<Employee> = [];  //list nhân viên bán hàng phân quyền dữ liệu theo người đang đăng nhập
  listEmpSale: Array<Employee> = []; //list nhân viên bán hàng (có thể thay đổi)
  listCustomer: Array<Customer> = [];  //list khách hàng định danh
  listCustomerNew: Array<Customer> = []; //list Khách hàng tiềm năng
  listAllLead: Array<any> = []; //list Hồ sơ thầu phân quyền dữ liệu theo người đang đăng nhập
  listLead: Array<any> = []; //list cơ hội (có thể thay đổi)
  listAllSaleBidding: Array<any> = []; //list Hồ sơ thầu phân quyền dữ liệu theo người đang đăng nhập
  listSaleBidding: Array<any> = []; //list hồ sơ thầu (có thể thay đổi)
  listParticipant: Array<Employee> = []; //list người tham gia
  tooltipSelectedParticipant: string = null; //tooltip: danh sách nhân viên bán hàng đang được chọn
  /* End */

  /* Biến lưu thông tin báo giá */
  quoteCode: string = null;
  quoteDate: Date = null;
  expirationDate: Date = null; //Ngày hết hiệu lực của báo giá
  statusId: string = null;
  isSendQuote: boolean = false;
  isAllowDeleteFile: boolean = true;
  /* End */

  amount: number = 0; //Tổng giá trị hàng hóa bán ra
  amountPriceInitial: number = 0; //Tổng giá trị hàng hóa đầu vào
  amountPriceCost: number = 0; //Tổng chi phí
  amountPriceCostNotInclude: number = 0; //Tổng chi phí
  totalAmountVat: number = 0; //Tổng thuế VAT
  totalAmountAfterVat: number = 0; //Tổng tiền sau thuế
  totalAmountBeforeVat: number = 0; //Tổng tiền trước thuế
  totalAmountPromotion: number = 0; //Tổng tiền khuyến mại
  totalAmountDiscount: number = 0; //Tổng thành tiền chiết khấu
  customerOrderAmountAfterDiscount: number = 0; //Tổng thanh toán
  amountPriceProfit: number = 0; //Lợi nhuận tạm tính
  valuePriceProfit: number = 0; //% lợi nhuận tạm tính
  amountAdvance: number = 0; //Tiền tạm ứng
  totalSumAmountLabor: number = 0; //Tổng thành tiền nhân công
  vatMessage: string = ''; //Tồn tại sản phẩm có thuế

  isParticipant: boolean = null;
  isUserSendAproval: boolean = false; // có phải người gửi phê duyệt không

  /*Chương trình khuyến mại*/
  isPromotionCustomer: boolean = false;
  isPromotionAmount: boolean = false;
  colsPromotion: any[];
  listPromotionApply: Array<PromotionObjectApply> = [];
  /*End*/

  /*Điều kiện hiển thị các button*/
  isShowGuiPheDuyet: boolean = false;
  isShowPheDuyet: boolean = false;
  isShowTuChoi: boolean = false;
  /*End*/

  popupNhapLyDoTuChoi: boolean = false; //Mở popup nhập lý do từ chối
  lyDoTuChoi: string = null; //Lý do từ chối

  listDuLieuQuyTrinh: Array<any> = [];

  tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink']
  };

  isDisableDescriptionControl: boolean = false;

  isShowDialog: boolean = false
  listQuoteApproveHistory: Array<any> = [];
  listAllQuoteApproveDetailHistory: Array<any> = [];
  listQuoteApproveDetailHistory: Array<any> = [];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private productService: ProductService,
    private bankService: BankService,
    private imageService: ImageUploadService,
    private quoteService: QuoteService,
    private getPermission: GetPermission,
    private messageService: MessageService,
    private dialogService: DialogService,
    private renderer: Renderer2,
    private confirmationService: ConfirmationService,
    private decimalPipe: DecimalPipe,
    private datePipe: DatePipe,
    private noteService: NoteService,
    private promotionService: PromotionService,
    private contactService: ContactService,
    private forderService: ForderConfigurationService,
    private location: Location,
    private quyTrinhService: QuyTrinhService,
    private CDRef: ChangeDetectorRef
  ) {
    this.renderer.listen('window', 'click', (e: Event) => {
      /**
      * Only run when toggleButton is not clicked
      * If we don't check this, all clicks (even on the toggle button) gets into this
      * section which in the result we might never see the menu open!
      * And the menu itself is checked here, and it's where we check just outside of
      * the menu and button the condition abbove must close the menu
      */
      if (this.toggleButton && this.notifi) {
        if (this.saveAndCreate) {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target) &&
            !this.saveAndCreate.nativeElement.contains(e.target)) {
            this.isOpenNotifiError = false;
          }
        } else {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target)) {
            this.isOpenNotifiError = false;
          }
        }
      }
    });

    this.innerWidth = window.innerWidth;
    if (this.innerWidth < 768) {
      this.isShowWorkFollowQuote = false;
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  async ngOnInit() {
    this.setForm();
    /*Tạo báo giá cho Khách hàng, Khách hàng tiềm năng, hay xem lại chi tiết báo giá*/
    this.route.params.subscribe(async params => {
      this.quoteId = params['quoteId'];

      /*Check permission*/
      let resource = "crm/customer/quote-detail";
      let permission: any = await this.getPermission.getPermission(resource);
      if (permission.status == false) {
        let msg = {
          severity: 'warn',
          summary: 'Thông báo:',
          detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ'
        };
        setTimeout(() => {
          this.showMessage(msg);
          setTimeout(() => {
            this.router.navigate(['/home']);
          }, 1000);
        }, 0);
      } else {
        let listCurrentActionResource = permission.listCurrentActionResource;
        if (listCurrentActionResource.indexOf("add") == -1) {
          this.actionAdd = false; //Thêm sản phẩm dịch vụ, Tạo đơn hàng
        }
        if (listCurrentActionResource.indexOf("import") == -1) {
          this.actionImport = false;  //import file upload
        }
        if (listCurrentActionResource.indexOf("delete") == -1) {
          this.actionDelete = false;  //Xóa sản phẩm dịch vụ, xóa file upload đã lưu
        }
        if (listCurrentActionResource.indexOf("edit") == -1) {
          //Sửa báo giá, sửa sản phẩm dịch vụ
          this.actionEdit = false;
        } if (listCurrentActionResource.indexOf("delete-file") == -1) {
          this.isAllowDeleteFile = false;
        }
        this.setTable();
        this.getDataDefault();
      }
    });
    /*End*/
  }

  ngAfterViewInit() {
    this.CDRef.detectChanges();
  }

  ngOnChanges() {
    this.CDRef.detectChanges();
  }

  setForm() {
    /*Nếu là VNS thì không có tính năng xuất PDF*/
    this.listAction = [];

    let optionExportExcel = {
      label: 'Xuất excel', icon: 'pi pi-file', command: () => {
        this.exportExcel("EXCEL");
      }
    };

    let optionExportPdf = {
      label: 'Xuất PDF', icon: 'pi pi-file', command: () => {
        this.exportPdf('download');
      }
    };

    this.listAction.push(optionExportExcel);

    if (this.appName != 'VNS') {
      this.listAction.push(optionExportPdf);
    }
    /* END */

    this.objectControl = new FormControl(null, [Validators.required]);
    this.paymentMethodControl = new FormControl(null);
    this.bankAccountControl = new FormControl(null);
    this.daysAreOwedControl = new FormControl('0');
    this.maxDebtControl = new FormControl('0');
    this.quoteStatusControl = new FormControl(null);
    this.nameQuoteControl = new FormControl(null, [Validators.required]);
    this.intendedDateControl = new FormControl(new Date());
    this.sendDateControl = new FormControl(null);
    this.effectiveDateControl = new FormControl('30', [Validators.required, ageRangeValidator(1, 365)]);
    this.personInChargeControl = new FormControl(null, [Validators.required]);
    this.hoSoThauControl = new FormControl(null);
    this.coHoiControl = new FormControl(null);
    this.kenhControl = new FormControl(null);
    this.descriptionControl = new FormControl('');
    this.noteControl = new FormControl('');
    this.discountTypeControl = new FormControl(this.discountTypeList.find(x => x.code == "PT"));
    this.discountValueControl = new FormControl('0');
    this.participantControl = new FormControl(null);
    this.vatControl = new FormControl('0');
    this.percentAdvanceControl = new FormControl('0');
    this.percentAdvanceTypeControl = new FormControl(this.discountTypeList.find(x => x.code == "PT"));
    this.createByControl = new FormControl(null);
    this.constructionTimeControl = new FormControl(null);
    this.scopeCategoryControl = new FormControl('');
    this.scopeDescriptionControl = new FormControl('');

    this.quoteForm = new FormGroup({
      objectControl: this.objectControl,
      paymentMethodControl: this.paymentMethodControl,
      bankAccountControl: this.bankAccountControl,
      daysAreOwedControl: this.daysAreOwedControl,
      maxDebtControl: this.maxDebtControl,
      quoteStatusControl: this.quoteStatusControl,
      intendedDateControl: this.intendedDateControl,
      nameQuoteControl: this.nameQuoteControl,
      sendDateControl: this.sendDateControl,
      effectiveDateControl: this.effectiveDateControl,
      personInChargeControl: this.personInChargeControl,
      hoSoThauControl: this.hoSoThauControl,
      coHoiControl: this.coHoiControl,
      kenhControl: this.kenhControl,
      descriptionControl: this.descriptionControl,
      noteControl: this.noteControl,
      discountTypeControl: this.discountTypeControl,
      discountValueControl: this.discountValueControl,
      participantControl: this.participantControl,
      vatControl: this.vatControl,
      percentAdvanceControl: this.percentAdvanceControl,
      percentAdvanceTypeControl: this.percentAdvanceTypeControl,
      createByControl: this.createByControl,
      constructionTimeControl: this.constructionTimeControl
    });

    this.scopeCreateForm = new FormGroup({
      scopeCategoryControl: this.scopeCategoryControl,
      scopeDescriptionControl: this.scopeDescriptionControl
    });
  }

  setTable() {
    this.cols = [
      { field: 'move', header: '#', width: '95px', textAlign: 'center', color: '#f44336' },
      // { field: 'gift', header: 'Quà KM', width: '170px', textAlign: 'center', color: '#f44336' },
      { field: 'productCode', header: 'Mã sản phẩm/dịch vụ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'productName', header: 'Tên sản phẩm dịch vụ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'nameVendor', header: 'Nhà cung cấp', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', width: '80px', textAlign: 'right', color: '#f44336' },
      { field: 'productNameUnit', header: 'Đơn vị tính', width: '95px', textAlign: 'left', color: '#f44336' },
      { field: 'unitPrice', header: 'Đơn giá', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'unitLaborPrice', header: 'Nhân công', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'sumAmountLabor', header: 'Thành tiền nhân công', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'nameMoneyUnit', header: 'Đơn vị tiền', width: '90px', textAlign: 'left', color: '#f44336' },
      { field: 'exchangeRate', header: 'Tỷ giá', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'vat', header: 'Thuế GTGT (%)', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'discountValue', header: 'Chiết khấu', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'sumAmount', header: 'Thành tiền (VND)', width: '130px', textAlign: 'right', color: '#f44336' },
      { field: 'delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];

    this.selectedColumns = [
      { field: 'move', header: '#', width: '95px', textAlign: 'center', color: '#f44336' },
      // { field: 'gift', header: 'Quà KM', width: '170px', textAlign: 'center', color: '#f44336' },
      { field: 'productCode', header: 'Mã sản phẩm/dịch vụ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'productName', header: 'Tên sản phẩm dịch vụ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', width: '80px', textAlign: 'right', color: '#f44336' },
      { field: 'productNameUnit', header: 'Đơn vị tính', width: '95px', textAlign: 'left', color: '#f44336' },
      { field: 'unitPrice', header: 'Đơn giá', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'unitLaborPrice', header: 'Nhân công', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'sumAmountLabor', header: 'Thành tiền nhân công', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'nameMoneyUnit', header: 'Đơn vị tiền', width: '90px', textAlign: 'left', color: '#f44336' },
      { field: 'exchangeRate', header: 'Tỷ giá', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'sumAmount', header: 'Thành tiền (VND)', width: '130px', textAlign: 'right', color: '#f44336' },
      { field: 'delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];

    this.colsFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'documentSize', header: 'Kích thước', width: '50%', textAlign: 'left', type: 'number' },
      { field: 'updatedDate', header: 'Ngày tạo', width: '50%', textAlign: 'left', type: 'date' },
      { field: 'uploadByName', header: 'Người Upload', width: '50%', textAlign: 'left', type: 'string' },
    ];

    this.colsNote = [
      { field: 'title', header: 'Tiêu đề', width: '30%', textAlign: 'left' },
      { field: 'content', header: 'Nội dung', width: '70%', textAlign: 'left' },
    ];

    this.colsNoteFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left' },
      { field: 'documentSize', header: 'Kích thước', width: '50%', textAlign: 'left' },
      { field: 'updatedDate', header: 'Ngày tạo', width: '50%', textAlign: 'left' },
    ];

    this.colsCost = [
      { field: 'costCode', header: 'Mã chi phí', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'costName', header: 'Tên chi phí', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'unitPrice', header: 'Đơn giá', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'sumAmount', header: 'Thành tiền (VND)', width: '60px', textAlign: 'right', color: '#f44336' },
      { field: 'isInclude', header: 'Bao gồm trong giá bán', width: '195px', textAlign: 'center', color: '#f44336' },
      { field: 'delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];
    this.selectedColumnsCost = this.colsCost;

    this.colsPromotion = [
      { field: 'promotionName', header: 'CTKM', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'promotionProductName', header: 'Tên sản phẩm dịch vụ', width: '160px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'productUnitName', header: 'Đơn vị tính', width: '110px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'soLuongTang', header: 'Số lượng', width: '100px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'giaTri', header: 'Giá trị', width: '140px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'loaiGiaTri', header: 'Loại giá trị', width: '100px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'amount', header: 'Thành tiền', width: '100px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
    ];
    this.colsPlan = [
      { field: 'tt', header: 'TT', width: '50px', textAlign: 'left', type: 'string' },
      { field: 'finished', header: 'Mốc hoàn thành', width: '300px', textAlign: 'left', type: 'string' },
      { field: 'execTime', header: 'Thời gian thực hiện', width: '200px', textAlign: 'left', type: 'string' },
      { field: 'sumExecTime', header: 'Tổng thời gian thực hiện', width: '200px', textAlign: 'left', type: 'string' },
    ];
    this.colsScope = [
      { field: 'tt', header: 'Thứ tự', width: '10%', textAlign: 'left', type: 'string' },
      { field: 'category', header: 'Tên hạng mục', width: '30%', textAlign: 'left', type: 'string' },
      { field: 'description', header: 'Mô tả chi tiết', width: '50%', textAlign: 'left', type: 'string' },
    ];
    this.colsPaymentTerm = [
      { field: 'orderNumber', header: 'STT', width: '50px', textAlign: 'center', display: 'table-cell' },
      { field: 'milestone', header: 'Mốc', width: '300px', textAlign: 'left', display: 'table-cell' },
      { field: 'paymentPercentage', header: '%Thanh toán', width: '200px', textAlign: 'center', display: 'table-cell' },
    ];

    this.selectedColumnsHistory = [
      { field: 'quoteName', header: 'Tên báo giá', width: '200px', textAlign: 'left', color: '#f44336' },
      { field: 'sendApproveDate', header: 'Ngày gửi phê duyệt', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'amountQuote', header: 'Giá trị báo giá', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'discountValue', header: 'Giá trị chiết khấu', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'amountPriceInitial', header: 'Giá trị đầu vào', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'amountPriceProfit', header: 'Lợi nhuận tạm tính', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'amountIncreaseDecrease', header: 'Tăng giảm giá trị so với phiên bản trước', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'action', header: 'Danh sách sản phẩm', width: '100px', textAlign: 'center', color: '#f44336' }
    ];

    this.selectedColumnsDetail = [
      { field: 'productCode', header: 'Mã sản phẩm', width: '90px', textAlign: 'left', color: '#f44336' },
      { field: 'productName', header: 'Tên sản phẩm', width: '100px', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'unitPrice', header: 'Đơn giá', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'exchangeRate', header: 'Tỷ giá', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'sumAmount', header: 'Thành tiền (VND)', width: '130px', textAlign: 'right', color: '#f44336' },
    ];
  }

  selectData(rowData: any) {
    this.listQuoteApproveDetailHistory = this.listAllQuoteApproveDetailHistory.filter(x => x.quoteApproveHistoryId == rowData.id);
    this.isShowDialog = true;
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  getDataDefault() {
    this.loading = true;
    this.quoteService.getMasterDataUpdateQuote(this.quoteId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        // this.getDuLieuQuyTrinh();

        this.listInvestFund = result.listInvestFund;  //list kênh bán hàng
        this.listPaymentMethod = result.listPaymentMethod; //list Phương thức thanh toán
        this.listQuoteStatus = result.listQuoteStatus; //list Trạng thái của báo giá
        this.listEmployee = result.listEmployee;  //list nhân viên bán hàng
        this.listCustomer = result.listCustomer;  //list khách hàng định danh
        this.listCustomerNew = result.listCustomerNew; //list Khách hàng tiềm năng
        this.listAllLead = result.listAllLead; //list Hồ sơ thầu
        this.listAllSaleBidding = result.listAllSaleBidding; //list Hồ sơ thầu
        this.listParticipant = result.listParticipant; //list người tham gia
        this.isApprovalQuote = result.isApproval;
        this.isParticipant = result.isParticipant;  //có phải người tham gia hay không
        this.listPromotionApply = result.listPromotionObjectApply //quà khuyến mại
        this.listQuotePlans = result.listQuotePlans || [];

        this.listQuoteScopes = result.listQuoteScopes || [];// Phạm vi công việc
        this.listQuotePaymentTerm = result.listQuotePaymentTerm || [];
        this.isShowGuiPheDuyet = result.isShowGuiPheDuyet;
        this.isShowPheDuyet = result.isShowPheDuyet;
        this.isShowTuChoi = result.isShowTuChoi;

        this.listQuoteApproveHistory = result.listQuoteApproveHistory;
        this.listAllQuoteApproveDetailHistory = result.listQuoteApproveDetailHistory;

        this.listQuoteApproveHistory.forEach((item, index) => {
          item.amountQuote = item.amount;

          if (index != 0) {
            let preQuoteAmount = this.listQuoteApproveHistory[index - 1].amount;

            item.amountIncreaseDecrease = item.amountQuote - preQuoteAmount;
          }

          if (item.amountIncreaseDecrease > 0) {
            item.isIncrease = true;
          }
          else if (item.amountIncreaseDecrease < 0) {
            item.isIncrease = false;
          }
          else {
            item.isIncrease = null;
          }

          item.amountIncreaseDecrease = Math.abs(item.amountIncreaseDecrease);

        });

        this.listAllQuoteApproveDetailHistory.forEach(item => {
          let price = item.quantity * item.unitPrice * item.exchangeRate;

          /*Thành tiền chiết khấu*/
          if (item.discountType == true) {
            item.amountDiscount = (price * item.discountValue) / 100;
          }
          else {
            item.amountDiscount = item.discountValue;
          }
          /*End*/

          /*Thành tiền thuế VAT*/
          let amountVAT = ((price - item.amountDiscount) * item.vat) / 100;
          /*End*/

          /*Thành tiền*/
          item.sumAmount = price - item.amountDiscount + amountVAT;
          /*End*/
        });
        //Lấy người phụ trách hiện tại của báo giá
        this.personInChargeIdOfQuote = result.quote.personInChargeId;

        /* Convert data to type TreeNode */
        this.data = this.list_to_tree();
        this.setDefaultValueForm(
          result.quote, //báo giá
          result.listQuoteDetail, //list sản phẩm
          result.listQuoteCostDetail, //list chi phí
          result.listQuoteDocument, //list file đính kèm
          result.listAdditionalInformation, //list thông tin bổ sung
          result.listNote, //list ghi chú (dòng thời gian)
          result.listParticipantId //list người tham gia của báo giá hiện tại
        );
        if (this.auth.UserId == result.quote.updatedById) {
          this.isUserSendAproval = true;
        }
        this.createByControl.disable();
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  async setDefaultValueForm(quote: Quote, listQuoteDetail: Array<QuoteDetail>, listQuoteCostDetail: Array<QuoteCostDetail>,
    listQuoteDocument: Array<QuoteDocument>, listAdditionalInformation: Array<AdditionalInformation>, listNote: Array<Note>,
    listParticipantId: Array<string>) {
    //Kiểm tra xem khách hàng của Cơ hội là Định danh hay Tiềm năng
    let customerType = this.getCustomerType(quote.objectTypeId);

    //Khách hàng Định danh
    if (customerType == 1) {
      this.selectedObjectType = 'cus';
      this.objectList = this.listCustomer;
      let selectedCustomer = this.listCustomer.find(x => x.customerId == quote.objectTypeId);
      await this.mapDataToQuote(selectedCustomer);
    }
    //Khách hàng Tiềm năng
    else if (customerType == 0) {
      this.selectedObjectType = 'lea';
      this.objectList = this.listCustomerNew;
      let selectedCustomer = this.listCustomerNew.find(x => x.customerId == quote.objectTypeId);
      await this.mapDataToQuote(selectedCustomer);
    }
    else {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không xác định được khách hàng của Báo giá' };
      this.showMessage(mgs);
    }

    //Nếu đã xác định được khách hàng
    if (customerType == 0 || customerType == 1) {
      //Kiểm tra khách hàng có nhận được CTKM hay không
      let _result: any = await this.promotionService.checkPromotionByCustomer(quote.objectTypeId);
      this.loading = false;
      if (_result.statusCode == 200) {
        this.isPromotionCustomer = _result.isPromotionCustomer;
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: _result.messageCode };
        this.showMessage(mgs);
      }

      //Phương thức thanh toán
      let toSelectPaymentMethod: Category = this.listPaymentMethod.find(x => x.categoryId == quote.paymentMethod);
      this.paymentMethodControl.setValue(toSelectPaymentMethod);
      if (toSelectPaymentMethod) {
        if (toSelectPaymentMethod.categoryCode == "BANK") {
          this.isShowBankAccount = true;
          this.bankService.getAllBankAccountByObject(quote.objectTypeId, this.customerContactCode).subscribe(response => {
            var result = <any>response;

            this.listBankAccount = result.bankAccountList;

            if (this.listBankAccount.length > 0) {
              let toSelectBankAccount = this.listBankAccount.find(x => x.bankAccountId == quote.bankAccountId);
              if (toSelectBankAccount) {
                this.bankAccountControl.setValue(toSelectBankAccount);
              }
            }
          });
        } else if (toSelectPaymentMethod.categoryCode == "CASH") {
          this.isShowBankAccount = false;
          this.listBankAccount = [];
          this.bankAccountControl.setValue(null);
        }
      } else {
        const toSelectPaymentMethod = this.listPaymentMethod.find(c => c.isDefault === true);
        this.paymentMethodControl.setValue(toSelectPaymentMethod);
        this.isShowBankAccount = false;
        this.listBankAccount = [];
        this.bankAccountControl.setValue(null);
      }

      //Mã báo giá
      this.quoteCode = quote.quoteCode;

      //Tên báo giá
      this.nameQuoteControl.setValue(quote.quoteName);

      //Trạng thái
      let selectedStatus = this.listQuoteStatus.find(x => x.categoryId == quote.statusId);
      if (selectedStatus) {
        this.statusCode = selectedStatus.categoryCode;
        this.quoteStatusControl.setValue(selectedStatus);

        if (this.statusCode != 'MTA' || this.isParticipant) {
          this.nameQuoteControl.disable();
        }
      }

      this.configWorkflowSteps(selectedStatus);

      //Ngày tạo
      this.quoteDate = new Date(quote.quoteDate);

      //Ngày gửi dự kiến
      let intendedDate = quote.intendedQuoteDate ? new Date(quote.intendedQuoteDate) : null;
      this.intendedDateControl.setValue(intendedDate);

      //Ngày báo giá
      let sendDate = quote.sendQuoteDate ? new Date(quote.sendQuoteDate) : null;
      this.sendDateControl.setValue(sendDate);

      //Số ngày hiệu lực
      this.effectiveDateControl.setValue(quote.effectiveQuoteDate.toString());
      if (this.isParticipant) {
        this.effectiveDateControl.disable();
      }

      //Nhân viên bán hàng
      let personInCharge = this.listEmpSale.find(x => x.employeeId == quote.personInChargeId);
      if (personInCharge) {
        this.personInChargeControl.setValue(personInCharge);
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không thể hiển thị lại Nhân viên bán hàng, kiểm tra phân quyền dữ liệu' };
        this.showMessage(mgs);
      }

      //Người tham gia
      let listSelectedParticipant = this.listParticipant.filter(x => listParticipantId.includes(x.employeeId));
      this.participantControl.setValue(listSelectedParticipant);
      if (listSelectedParticipant?.length > 0) {
        this.tooltipSelectedParticipant = listSelectedParticipant.map(x => x.employeeCode).join(', ');
      }

      //Lấy list Hồ sơ thầu theo khách hàng được chọn
      this.listSaleBidding = this.listAllSaleBidding.filter(x => x.customerId == quote.objectTypeId);

      /*Set value cho Hồ sơ thầu*/
      if (quote.saleBiddingId) {
        let selectedSalebidding = this.listSaleBidding.find(x => x.saleBiddingId == quote.saleBiddingId);
        if (selectedSalebidding) {
          this.hoSoThauControl.setValue(selectedSalebidding);
        }
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không thể hiển thị lại Hồ sơ thầu, kiểm tra phân quyền dữ liệu' };
          this.showMessage(mgs);
        }
      }

      //Lấy list Cơ hội theo khách hàng được chọn
      this.listLead = this.listAllLead.filter(x => x.customerId == quote.objectTypeId);

      /*Set value cho Cơ hội*/
      if (quote.leadId) {
        let selectedLead = this.listLead.find(x => x.leadId == quote.leadId);
        if (selectedLead) {
          this.coHoiControl.setValue(selectedLead);
        }
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không thể hiển thị lại Cơ hội, kiểm tra phân quyền dữ liệu' };
          this.showMessage(mgs);
        }
      }

      //Kênh bán hàng
      if (quote.investmentFundId) {
        let investmentFund = this.listInvestFund.find(s => s.categoryId == quote.investmentFundId);
        this.kenhControl.setValue(investmentFund);
      }

      //Diễn giải
      this.descriptionControl.setValue(quote.description);
      if (this.statusCode != 'MTA' || this.isParticipant) {
        // this.descriptionControl.disable();
        this.isDisableDescriptionControl = true;
      }

      //Ghi chú
      this.noteControl.setValue(quote.note);
      if (this.statusCode != 'MTA' || this.isParticipant) {
        this.noteControl.disable();
      }

      /* List File đính kèm: Quote Document */
      listQuoteDocument.forEach(item => {
        let quoteDocument = new QuoteDocument();
        quoteDocument.quoteDocumentId = item.quoteDocumentId;
        quoteDocument.quoteId = item.quoteId;
        quoteDocument.documentName = item.documentName;
        quoteDocument.documentSize = item.documentSize;
        quoteDocument.documentUrl = item.documentUrl;
        quoteDocument.createdById = item.createdById;
        quoteDocument.createdDate = item.createdDate;
        quoteDocument.updatedById = item.updatedById;
        quoteDocument.updatedDate = item.updatedDate;
        quoteDocument.uploadByName = item.uploadByName;
        quoteDocument.active = item.active;

        this.arrayQuoteDocumentModel = [...this.arrayQuoteDocumentModel, quoteDocument];
        // Sắp xếp các file upload theo descending thời gian
        this.arrayQuoteDocumentModel.sort((a, b) => new Date(b.updatedDate).getTime() - new Date(a.updatedDate).getTime());
      });
      /*End: Quote Document*/

      /*List Sản phẩm*/
      this.listQuoteDetailModel = [];
      listQuoteDetail.forEach((item, index) => {
        let quoteDetailModel = new QuoteDetail();
        quoteDetailModel.quoteDetailId = item.quoteDetailId;
        quoteDetailModel.vendorId = item.vendorId;
        quoteDetailModel.quoteId = item.quoteId;
        quoteDetailModel.productId = item.productId;
        quoteDetailModel.quantity = item.quantity;
        quoteDetailModel.unitPrice = item.unitPrice;
        quoteDetailModel.currencyUnit = item.currencyUnit;
        quoteDetailModel.exchangeRate = item.exchangeRate;
        quoteDetailModel.vat = item.vat;
        quoteDetailModel.discountType = item.discountType;
        quoteDetailModel.discountValue = item.discountValue;
        quoteDetailModel.description = item.description;
        quoteDetailModel.orderDetailType = item.orderDetailType;
        quoteDetailModel.createdById = item.createdById;
        quoteDetailModel.createdDate = item.createdDate;
        quoteDetailModel.updatedById = item.updatedById;
        quoteDetailModel.updatedDate = item.updatedDate;
        quoteDetailModel.active = item.active;
        quoteDetailModel.unitId = item.unitId;
        quoteDetailModel.incurredUnit = item.incurredUnit;
        quoteDetailModel.productCode = item.productCode;
        quoteDetailModel.nameVendor = item.nameVendor;
        quoteDetailModel.productNameUnit = item.productNameUnit;
        quoteDetailModel.nameMoneyUnit = item.nameMoneyUnit;
        quoteDetailModel.isPriceInitial = item.isPriceInitial;
        quoteDetailModel.priceInitial = item.priceInitial;
        quoteDetailModel.productName = item.productName;
        quoteDetailModel.orderNumber = item.orderNumber ? item.orderNumber : index + 1;
        quoteDetailModel.unitLaborPrice = item.unitLaborPrice;
        quoteDetailModel.unitLaborNumber = item.unitLaborNumber;

        if (this.appName == 'VNS') {
          quoteDetailModel.sumAmount = item.sumAmount;
          quoteDetailModel.sumAmountLabor = item.unitLaborPrice * item.quantity;
        }
        else {
          quoteDetailModel.sumAmount = item.sumAmount + item.sumAmountLabor;
          quoteDetailModel.sumAmountLabor = item.unitLaborPrice * item.unitLaborNumber * item.exchangeRate;
        }

        quoteDetailModel.guaranteeTime = item.guaranteeTime;
        quoteDetailModel.productCategoryId = item.productCategoryId;


        var arrayAttributeValue = item.quoteProductDetailProductAttributeValue;
        if (arrayAttributeValue !== null) {
          arrayAttributeValue.forEach(_attr => {
            let orderProductDetailProductAttributeValue = new QuoteProductDetailProductAttributeValue();

            orderProductDetailProductAttributeValue.productId = _attr.productId;
            orderProductDetailProductAttributeValue.productAttributeCategoryId = _attr.productAttributeCategoryId;
            orderProductDetailProductAttributeValue.productAttributeCategoryValueId = _attr.productAttributeCategoryValueId;

            quoteDetailModel.quoteProductDetailProductAttributeValue.push(orderProductDetailProductAttributeValue);
          });
        }

        this.listQuoteDetailModel = [...this.listQuoteDetailModel, quoteDetailModel];
      });
      /*End*/

      /*List Thông tin chi phí*/
      this.listQuoteCostDetailModel = [];
      listQuoteCostDetail.forEach(item => {
        let quoteCostDetailModel = new QuoteCostDetail();
        quoteCostDetailModel.quoteCostDetailId = item.quoteCostDetailId;
        quoteCostDetailModel.costId = item.costId;
        quoteCostDetailModel.quoteId = item.quoteId;
        quoteCostDetailModel.quantity = item.quantity;
        quoteCostDetailModel.unitPrice = item.unitPrice;
        quoteCostDetailModel.costName = item.costName;
        quoteCostDetailModel.costCode = item.costCode;
        quoteCostDetailModel.createdById = item.createdById;
        quoteCostDetailModel.createdDate = item.createdDate;
        quoteCostDetailModel.updatedById = item.updatedById;
        quoteCostDetailModel.updatedDate = item.updatedDate;
        quoteCostDetailModel.active = item.active;
        quoteCostDetailModel.sumAmount = item.quantity * item.unitPrice;
        quoteCostDetailModel.isInclude = item.isInclude;

        this.listQuoteCostDetailModel = [...this.listQuoteCostDetailModel, quoteCostDetailModel];
      });
      /*End*/

      /*List thông tin bổ sung*/
      this.listAdditionalInformation = [];
      listAdditionalInformation.forEach(item => {
        this.maxOrdinal++;
        let additionalInformation: AdditionalInformation = {
          ordinal: this.maxOrdinal,
          additionalInformationId: item.additionalInformationId,
          objectId: item.objectId,
          objectType: item.objectType,
          title: item.title,
          content: item.content,
          active: true,
          createdDate: new Date(),
          createdById: this.emptyGuid,
          updatedDate: null,
          updatedById: null,
          orderNumber: null,
        };
        this.listAdditionalInformation = [...this.listAdditionalInformation, additionalInformation];
      });
      /*End*/

      /*List ghi chú (dòng thời gian)*/
      this.noteHistory = listNote;
      this.handleNoteContent();
      /*End*/

      /*Loại Chiết khấu*/
      let discountType = this.discountTypeList.find(x => x.value == quote.discountType);
      this.discountTypeControl.setValue(discountType);

      this.discountValueControl.setValue(quote.discountValue.toString());
      /*End*/

      /*Thuế VAT*/
      this.vatControl.setValue(quote.vat.toString());
      /*End*/

      if (this.appName == 'VNS') {
        /* Loại tạm ứng */
        let type = this.discountTypeList.find(x => x.value == quote.percentAdvanceType);
        this.percentAdvanceTypeControl.setValue(type);
        /* End */

        /* thời gian thi cong*/
        this.constructionTimeControl.setValue(quote.constructionTime != null ? quote.constructionTime : null);
        /* End */

        /* Người tạo */
        this.createByControl.setValue(quote.createdByEmp?.toString());
        /* End */
      }

      /*% tạm ứng*/
      this.percentAdvanceControl.setValue(quote.percentAdvance.toString());
      /*End*/

      this.calculatorAmount();
    }
  }

  /*Kiểm tra loại khách hàng*/
  getCustomerType(customerId: string): number {
    let object_1 = this.listCustomer.find(x => x.customerId == customerId);
    let object_2 = this.listCustomerNew.find(x => x.customerId == customerId);

    //Là khách hàng Định danh
    if (object_1 && !object_2) {
      return 1;
    }
    //Là khách hàng Tiềm năng
    else if (!object_1 && object_2) {
      return 0;
    }
    //khách hàng không xác định
    else {
      return -1;
    }
  }

  /*Mapdata vào Báo giá*/
  async mapDataToQuote(customer: Customer) {
    //Nếu khách hàng có trong tập khách hàng của người đang đăng nhập
    if (customer) {
      this.objectControl.setValue(customer);

      this.objCustomer = customer;
      this.email = customer.customerEmail;
      this.phone = customer.customerPhone;

      /** Lấy địa chỉ của khách hàng */
      this.contactService.getAddressByObject(customer.customerId, "CUS").subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.fullAddress = result.address;
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
      /** End */

      //Số ngày được nợ
      let daysAreOwed = customer.maximumDebtDays ? customer.maximumDebtDays : 0;
      this.daysAreOwedControl.setValue(daysAreOwed);

      //Số nợ tối đa
      let maxAmount = customer.maximumDebtValue ? customer.maximumDebtValue : 0;
      this.maxDebtControl.setValue(maxAmount);

      //Lấy list nhân viên bán hàng mới (phân quyền dữ liệu theo người phụ trách của khách hàng)
      this.listEmpSale = [];
      this.loading = true;
      let result: any = await this.quoteService.getEmployeeByPersonInCharge(customer.personInChargeId, this.personInChargeIdOfQuote);
      this.loading = false;
      if (result.statusCode == 200) {
        this.listEmpSale = result.listEmployee;
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    }
    //Nếu khách hàng không có trong tập khách hàng của người đang đăng nhập
    else {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Khách hàng này không thuộc quyền phụ trách của bạn' };
      this.showMessage(mgs);
    }
  }

  /*Tính các giá trị của Tổng hợp báo giá*/
  calculatorAmount() {
    if (this.appName != 'VNS') {
      let defaultNumberType = parseInt(this.defaultNumberType, 10);

      this.amount = 0; //Tổng giá trị hàng hóa bán ra
      this.amountPriceInitial = 0; //Tổng giá trị hàng hóa đầu vào
      this.amountPriceCost = 0; //Tổng chi phí
      this.amountPriceCostNotInclude = 0; //Tổng chi phí
      this.totalAmountVat = 0; //Tổng thuế VAT
      this.totalAmountAfterVat = 0; //Tổng tiền sau thuế
      this.totalAmountBeforeVat = 0; // Tổng tiền trước thuế

      this.totalAmountPromotion = 0; //Tổng tiền khuyến mại
      this.totalAmountDiscount = 0; //Tổng thành tiền chiết khấu
      this.customerOrderAmountAfterDiscount = 0; //Tổng thanh toán
      this.amountPriceProfit = 0; //Lợi nhuận tạm tính
      this.valuePriceProfit = 0; //% lợi nhuận tạm tính

      //Danh sách sản phẩm dịch vụ
      this.listQuoteDetailModel.forEach(item => {
        let price = item.quantity * item.unitPrice * item.exchangeRate;
        let laborPrice = item.unitLaborPrice * item.unitLaborNumber * item.exchangeRate;

        /*Thành tiền chiết khấu*/
        if (item.discountType) {
          item.amountDiscount = ((price + laborPrice) * item.discountValue) / 100;
        }
        else {
          item.amountDiscount = item.discountValue;
        }
        /*End*/

        /*Thành tiền thuế VAT*/
        let amountVAT = ((price + laborPrice - item.amountDiscount) * item.vat) / 100;
        /*End*/

        /*Tổng thuế VAT*/
        this.totalAmountVat += amountVAT;
        /*End*/

        /*Thành tiền*/
        item.sumAmount = price + laborPrice - item.amountDiscount + amountVAT;
        /*End*/

        /*Tiền vốn*/
        let amountInitial = item.quantity * item.priceInitial * item.exchangeRate;
        /*End*/

        /*Tổng giá trị hàng hóa bán ra*/
        this.amount += this.roundNumber((price + laborPrice - item.amountDiscount), defaultNumberType);
        /*End*/

        /*Tổng giá trị hàng hóa đầu vào*/
        this.amountPriceInitial += this.roundNumber(amountInitial, defaultNumberType);
        /*End*/
      });

      //Danh sách chi phí
      this.listQuoteCostDetailModel.forEach(item => {
        if (!item.isInclude) {
          this.amountPriceCostNotInclude += this.roundNumber(item.quantity * item.unitPrice, defaultNumberType);
        }
        this.amountPriceCost += this.roundNumber(item.quantity * item.unitPrice, defaultNumberType);
      });

      /*Tổng tiền sau thuế*/
      this.totalAmountAfterVat = this.amount + this.totalAmountVat + this.amountPriceCostNotInclude;

      //Kiểm tra có đủ điều kiện nhận quà khuyến mại không
      this.promotionService.checkPromotionByAmount(this.totalAmountAfterVat).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.isPromotionAmount = result.isPromotionAmount;
        }
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
        }
      });
      /*End*/

      //Nếu không đủ điều kiện nhận khuyến mại thì xóa các quà khuyến mại đó
      let ignoreList = this.listPromotionApply.filter(x => x.conditionsType == 3 && x.soTienTu > this.totalAmountAfterVat);
      this.listPromotionApply = this.listPromotionApply.filter(x => !ignoreList.includes(x));

      /*Tính lại thành tiền của Tab khuyến mại*/
      this.listPromotionApply.forEach(item => {
        //Nếu là phiếu giảm giá
        if (!item.productId) {
          //Nếu là số tiền
          if (!item.loaiGiaTri) {
            item.amount = item.giaTri * item.soLuongTang;
          }
          //Nếu là %
          else {
            item.amount = this.roundNumber((this.totalAmountAfterVat * item.giaTri / 100) * item.soLuongTang, defaultNumberType);
          }
        }
        //Nếu là sản phẩm
        else {
          item.amount = item.giaTri * item.soLuongTang;
        }
      });
      /*End*/

      /*Tổng tiền khuyến mại*/
      let totalAmountPhieuGiamGia = 0;  //Tổng thành tiền của tất cả phiếu giảm giá trong tab khuyến mại
      this.listPromotionApply.forEach(item => {
        //Chỉ tính với phiếu giảm giá
        if (!item.productId) {
          totalAmountPhieuGiamGia += item.amount;
          this.totalAmountPromotion += item.amount;
        }
      });
      /*End*/

      /*Tổng thành tiền chiết khấu*/
      let discountType: DiscountType = this.discountTypeControl.value;
      let discountValue = ParseStringToFloat(this.discountValueControl.value);

      if (discountType.value) {
        this.totalAmountDiscount = this.totalAmountAfterVat * discountValue / 100;
      }
      else {
        this.totalAmountDiscount = discountValue;
      }
      /*End*/
      //
      /*Tổng tiền trước thuế*/
      this.totalAmountBeforeVat = this.amount;
      /*Tổng thanh toán*/
      this.customerOrderAmountAfterDiscount = this.roundNumber(this.totalAmountAfterVat - this.totalAmountDiscount - totalAmountPhieuGiamGia, defaultNumberType);
      // Tổng
      let total = this.roundNumber(this.totalAmountBeforeVat, defaultNumberType);
      /*End*/

      /*Nếu Tổng thanh toán không âm và khác 0*/
      if (total > 0) {
        /*Lợi nhuận tạm tính*/
        this.amountPriceProfit = this.roundNumber(this.customerOrderAmountAfterDiscount - this.amountPriceInitial - this.amountPriceCost, defaultNumberType);
        /*End*/
        /*% lợi nhuận tạm tính*/
        this.valuePriceProfit = this.roundNumber((this.amountPriceProfit / this.customerOrderAmountAfterDiscount) * 100, defaultNumberType);
        /*End*/
      }
      //Nếu Tổng thanh toán âm hoặc bằng 0
      else {
        this.customerOrderAmountAfterDiscount = 0;
        this.amountPriceProfit = 0;
        this.valuePriceProfit = 0;
      }
      /*End*/
    }
    else if (this.appName == 'VNS') {
      let defaultNumberType = parseInt(this.defaultNumberType, 10);

      this.amount = 0; //Tổng giá trị hàng hóa bán ra
      this.amountPriceInitial = 0; //Tổng giá trị hàng hóa đầu vào
      this.amountPriceCost = 0; //Tổng chi phí
      this.totalAmountVat = 0; //Tổng thuế VAT
      this.totalAmountAfterVat = 0; //Tổng tiền sau thuế
      this.totalAmountPromotion = 0; //Tổng tiền khuyến mại
      this.totalAmountDiscount = 0; //Tổng thành tiền chiết khấu
      this.customerOrderAmountAfterDiscount = 0; //Tổng thanh toán
      this.amountPriceProfit = 0; //Lợi nhuận tạm tính
      this.valuePriceProfit = 0; //% lợi nhuận tạm tính
      this.totalSumAmountLabor = 0; //Tổng thành tiền nhân công
      this.amountAdvance = 0; //Tiền tạm ứng

      //Danh sách sản phẩm dịch vụ
      this.listQuoteDetailModel.forEach(item => {
        let price = item.quantity * item.unitPrice * item.exchangeRate;

        /*Tổng giá trị hàng hóa bán ra*/
        this.amount += this.roundNumber(price, defaultNumberType);
        /*End*/

        /*Tiền vốn*/
        let amountInitial = item.quantity * item.priceInitial * item.exchangeRate;
        /*End*/

        /*Tổng giá trị hàng hóa đầu vào*/
        this.amountPriceInitial += this.roundNumber(amountInitial, defaultNumberType);
        /*End*/

        /*Tiền chiết khấu*/
        if (item.discountType) {
          item.amountDiscount = price * item.discountValue / 100;
        }
        else {
          item.amountDiscount = item.discountValue;
        }
        /*End*/

        /*Tổng thành tiền chiết khấu*/
        this.totalAmountDiscount += item.amountDiscount;
        /*End*/

        /*Tiền thuế VAT*/
        let amountVAT = (price - item.amountDiscount + item.sumAmountLabor) * item.vat / 100;
        /*End*/

        /*Tổng thuế VAT*/
        this.totalAmountVat += amountVAT;
        /*End*/

        /*Thành tiền*/
        item.sumAmount = price - item.amountDiscount + amountVAT + item.sumAmountLabor;
        /*End*/

        /*Tổng thành tiền nhân công*/
        this.totalSumAmountLabor += item.sumAmountLabor;
        /*End*/
      });

      //Danh sách chi phí
      this.listQuoteCostDetailModel.forEach(item => {
        let price = item.quantity * item.unitPrice;

        /*Tổng chi phí*/
        this.amountPriceCost += this.roundNumber(price, defaultNumberType);
        /*End*/
      });

      //Kiểm tra có đủ điều kiện nhận quà khuyến mại không
      this.promotionService.checkPromotionByAmount(this.amount).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.isPromotionAmount = result.isPromotionAmount;
        }
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
        }
      });
      /*End*/

      //Nếu không đủ điều kiện nhận khuyến mại thì xóa các quà khuyến mại đó
      let ignoreList = this.listPromotionApply.filter(x => x.conditionsType == 3 && x.soTienTu > this.amount);
      this.listPromotionApply = this.listPromotionApply.filter(x => !ignoreList.includes(x));

      /*Tính lại thành tiền của Tab khuyến mại*/
      this.listPromotionApply.forEach(item => {
        //Nếu là phiếu giảm giá
        if (!item.productId) {
          //Nếu là số tiền
          if (!item.loaiGiaTri) {
            item.amount = item.giaTri * item.soLuongTang;
          }
          //Nếu là %
          else {
            item.amount = this.roundNumber((this.amount * item.giaTri / 100) * item.soLuongTang, defaultNumberType);
          }
        }
        //Nếu là sản phẩm
        else {
          item.amount = item.giaTri * item.soLuongTang;
        }
      });
      /*End*/

      /*Tổng tiền khuyến mại*/
      this.listPromotionApply.forEach(item => {
        //Chỉ tính với phiếu giảm giá
        if (!item.productId) {
          this.totalAmountPromotion += item.amount;
        }
      });
      /*End*/

      /*Kiểm tra nếu có ít nhất 1 sản phẩm có chiết khấu > 0 hoặc vat > 0*/
      let hasDiscount = false;
      let hasVat = false;
      this.listQuoteDetailModel.forEach(item => {
        if (item.discountValue > 0) {
          hasDiscount = true;
        }

        if (item.vat > 0) {
          hasVat = true;
        }
      });

      if (hasDiscount) {
        //Loại chiết khấu: Theo tiền
        this.discountTypeControl.setValue(this.discountTypeList.find(x => x.value == false));

        //Tổng thành tiền chiết khấu
        this.discountValueControl.setValue(this.totalAmountDiscount);
      }
      else {
        /*Tổng thành tiền chiết khấu*/
        let discountType: DiscountType = this.discountTypeControl.value;
        let discountValue = ParseStringToFloat(this.discountValueControl.value);

        if (discountType.value) {
          this.totalAmountDiscount = this.amount * discountValue / 100;
        }
        else {
          this.totalAmountDiscount = discountValue;
        }
        /*End*/
      }
      /*End*/

      if (hasVat) {
        this.vatMessage = 'Tồn tại sản phẩm có thuế';
        this.vatControl.setValue('0');
      }
      else {
        this.vatMessage = '';
        let vat = ParseStringToFloat(this.vatControl.value);
        this.totalAmountVat = (this.amount + this.totalSumAmountLabor - this.totalAmountDiscount - this.totalAmountPromotion) * vat / 100;
      }

      /*Tổng tiền sau thuế
      = Tổng GTHH bán ra + Tổng thành tiền nhân công - Tổng chiết khấu - Tổng khuyến mại + Tổng thuế VAT + Tổng chi phí
      */
      this.totalAmountAfterVat = this.amount + this.totalSumAmountLabor
        - this.totalAmountDiscount - this.totalAmountPromotion + this.totalAmountVat + this.amountPriceCost;

      /*Tiền tạm ứng*/
      let type = this.percentAdvanceTypeControl.value
      let percentAdvance = ParseStringToFloat(this.percentAdvanceControl.value.trim());
      if (type.value) {
        this.amountAdvance = this.totalAmountAfterVat * percentAdvance / 100;
      } else {
        this.amountAdvance = percentAdvance;
      }


      /*End*/

      /*Nếu Tổng thanh toán không âm và khác 0*/
      if (this.totalAmountAfterVat > 0) {
        /*Lợi nhuận tạm tính*/
        this.amountPriceProfit = this.roundNumber(this.totalAmountAfterVat - this.amountPriceInitial, defaultNumberType);
        /*End*/

        /*% lợi nhuận tạm tính*/
        this.valuePriceProfit = this.roundNumber((this.amountPriceProfit / this.totalAmountAfterVat) * 100, defaultNumberType);
        /*End*/
      }
      //Nếu Tổng thanh toán âm hoặc bằng 0
      else {
        this.totalAmountAfterVat = 0;
        this.amountPriceProfit = 0;
        this.valuePriceProfit = 0;
      }
      /*End*/
    }
  }

  /*Event chuyển loại khách hàng (Khách hàng hoặc Khách hàng tiềm năng)*/
  changeObjectType(objecType: any) {
    if (objecType == 'cus') {
      this.objectList = this.listCustomer;
    } else if (objecType == 'lea') {
      this.objectList = this.listCustomerNew;
    }

    //Ẩn Hộp quà CTKM
    this.isPromotionCustomer = false;

    //Xóa những sản phẩm khuyến mại theo khách hàng
    this.listPromotionApply = this.listPromotionApply.filter(x => x.conditionsType != 1);

    if (!this.objectList.find(x => x == this.objectControl.value)) {
      //Nếu khách hàng hiện tại đang được chọn không thuộc objectList thì reset các trường
      this.objCustomer = null;
      this.objectControl.setValue(null);
      this.email = '';
      this.phone = '';
      this.fullAddress = '';

      //Reset Phương thức thanh toán
      const toSelectPaymentMethod = this.listPaymentMethod.find(c => c.isDefault === true);
      this.paymentMethodControl.setValue(toSelectPaymentMethod);
      //End

      this.daysAreOwedControl.setValue('0');  //Reset Số ngày được nợ
      this.maxDebtControl.setValue('0');  //Reset Số nợ tối đa

      //Reset Tài khoản ngân hàng
      this.isShowBankAccount = false;
      this.listBankAccount = [];
      this.bankAccountControl.setValue(null);
      //End

      //Xóa list sản phẩm dịch vụ
      this.listQuoteDetailModel = [];

      //bỏ chọn nhân viên bán hàng
      this.personInChargeControl.reset();

      //reset list nhân viên bán hàng
      this.listEmpSale = this.listEmployee;

      //bỏ chọn Cơ hội
      this.coHoiControl.reset();

      //reset list Cơ hội
      // this.listLead = this.listAllLead;
      this.listLead = [];

      //bỏ chọn Hồ sơ thầu
      this.hoSoThauControl.reset();

      //reset list Hồ sơ thầu
      // this.listSaleBidding = this.listAllSaleBidding;
      this.listSaleBidding = [];

      this.calculatorAmount();
    }
  }

  /*Event thay đổi khách hàng*/
  async changeCustomer(value: Customer) {
    this.objCustomer = null;
    this.email = '';
    this.phone = '';
    this.fullAddress = '';

    //Reset Phương thức thanh toán
    const toSelectPaymentMethod = this.listPaymentMethod.find(c => c.isDefault === true);
    this.paymentMethodControl.setValue(toSelectPaymentMethod ?? null);
    //End

    this.daysAreOwedControl.setValue('0');  //Reset Số ngày được nợ
    this.maxDebtControl.setValue('0');  //Reset Số nợ tối đa

    //Reset Tài khoản ngân hàng
    this.listBankAccount = [];
    this.bankAccountControl.reset();
    if (toSelectPaymentMethod?.categoryCode == "CASH") {
      this.isShowBankAccount = false;
    }
    else if (toSelectPaymentMethod?.categoryCode == "BANK") {
      this.isShowBankAccount = true;
    }
    else {
      this.isShowBankAccount = false;
    }

    //reset nhân viên bán hàng (người phụ trách)
    this.listEmpSale = this.listEmployee;
    this.personInChargeControl.reset();

    //reset cơ hội
    // this.listLead = this.listAllLead;
    this.listLead = [];
    this.coHoiControl.reset();

    //reset hồ sơ thầu
    // this.listSaleBidding = this.listAllSaleBidding;
    this.listSaleBidding = [];
    this.hoSoThauControl.reset();

    //Danh sách sản phẩm dịch vụ
    this.listQuoteDetailModel = [];

    //Ẩn Hộp quà CTKM
    this.isPromotionCustomer = false;

    //Xóa những sản phẩm khuyến mại theo khách hàng
    this.listPromotionApply = this.listPromotionApply.filter(x => x.conditionsType != 1);

    //Tính lại phần Tổng hợp báo giá
    this.calculatorAmount();

    /*Nếu chọn khách hàng*/
    if (value) {
      this.objCustomer = value;
      this.email = value.customerEmail;
      this.phone = value.customerPhone;

      /** Lấy địa chỉ của khách hàng */
      this.contactService.getAddressByObject(value.customerId, "CUS").subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.fullAddress = result.address;
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
      /** End */

      //Số ngày được nợ
      let daysAreOwed = value.maximumDebtDays ? value.maximumDebtDays : 0;
      this.daysAreOwedControl.setValue(daysAreOwed);

      //Số nợ tối đa
      let maxAmount = value.maximumDebtValue ? value.maximumDebtValue : 0;
      this.maxDebtControl.setValue(maxAmount);

      //Nếu Phương thức thanh toán là Chuyển khoản thì lấy ra list account bank
      if (this.isShowBankAccount == true) {
        this.bankService.getAllBankAccountByObject(value.customerId, this.customerContactCode).subscribe(response => {
          var result = <any>response;

          this.listBankAccount = result.bankAccountList;
          if (this.listBankAccount.length > 0) {
            let toSelectBankAccount = this.listBankAccount[0];
            this.bankAccountControl.setValue(toSelectBankAccount);
          }
        });
      }

      //Lấy list Cơ hội theo khách hàng được chọn
      this.listLead = this.listAllLead.filter(x => x.customerId == value.customerId);

      //Lấy list Hồ sơ thầu theo khách hàng được chọn
      this.listSaleBidding = this.listAllSaleBidding.filter(x => x.customerId == value.customerId);

      //Lấy list nhân viên bán hàng mới (phân quyền dữ liệu theo người phụ trách của khách hàng)
      this.listEmpSale = [];
      this.loading = true;
      let result: any = await this.quoteService.getEmployeeByPersonInCharge(value.personInChargeId, this.personInChargeIdOfQuote);
      this.loading = false;
      if (result.statusCode == 200) {
        this.listEmpSale = result.listEmployee;

        //set default value là người phụ trách của khách hàng được chọn
        let emp = this.listEmpSale.find(e => e.employeeId == value.personInChargeId);
        this.personInChargeControl.setValue(emp);
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }

      let _result: any = await this.promotionService.checkPromotionByCustomer(value.customerId);
      this.loading = false;
      if (_result.statusCode == 200) {
        this.isPromotionCustomer = _result.isPromotionCustomer;
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: _result.messageCode };
        this.showMessage(mgs);
      }
    }
  }

  /*Event thay đổi phương thức thanh toán*/
  changeMethodControl(value: Category) {
    if (value.categoryCode == "BANK") {
      let customer: Customer = this.objectControl.value;

      if (customer && this.selectedObjectType == 'cus') {
        //Nếu đã chọn khách hàng
        this.isShowBankAccount = true;
        let customerId = customer.customerId;

        this.bankService.getAllBankAccountByObject(customerId, this.customerContactCode).subscribe(response => {
          var result = <any>response;

          this.listBankAccount = result.bankAccountList;
          if (this.listBankAccount.length > 0) {
            let toSelectBankAccount = this.listBankAccount[0];
            this.bankAccountControl.setValue(toSelectBankAccount);
          }
        });
      }
    } else {
      this.isShowBankAccount = false;
      this.listBankAccount = [];
      this.bankAccountControl.setValue(null);
    }
  }

  /*Event thay đổi cơ hội*/
  changeLead(value: any) {
    this.hoSoThauControl.reset();
    this.personInChargeControl.reset();
    this.listQuoteDetailModel = [];

    if (value) {
      /*Khách hàng sẽ là khách hàng của Hồ sơ thầu*/
      let customerType = this.getCustomerType(value.customerId);
      //Là khách hàng Định danh
      if (customerType == 1) {
        this.objectList = this.listCustomer;
        let customer = this.listCustomer.find(x => x.customerId == value.customerId);

        if (customer) {
          this.setDataFormCustomer(customer);
        }
      }
      //Là khách hàng Tiềm năng
      else if (customerType == 0) {
        this.selectedObjectType = 'lea';
        this.objectList = this.listCustomerNew;
        let customerNew = this.listCustomerNew.find(x => x.customerId == value.customerId);

        if (customerNew) {
          this.setDataFormCustomer(customerNew);
        }
      }
      //khách hàng không xác định
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không xác định được khách hàng của Cơ hội' };
        this.showMessage(mgs);
      }
      /*End*/

      let defaultNumberType = parseInt(this.defaultNumberType, 10);
      this.listSaleBidding = this.listAllSaleBidding.filter(h => h.leadId == value.leadId);

      let personInChargeId = value.personInChargeId ?? null;
      if (personInChargeId) {
        let personInCharge: Employee = this.listEmpSale.find(x => x.employeeId == personInChargeId);
        this.personInChargeControl.setValue(personInCharge);
      }

      /*Thêm list sản phẩm dịch vụ của Cơ hội vào list sản phẩm dịch vụ của Báo giá*/
      value.listLeadDetail.forEach((element, index) => {
        let obj: QuoteDetail = new QuoteDetail;
        obj.productCategoryId = element.productCategory;
        obj.vendorId = element.vendorId;
        obj.productId = element.productId;
        obj.quantity = element.quantity;
        obj.unitPrice = element.unitPrice;
        obj.currencyUnit = element.currencyUnit;
        obj.exchangeRate = element.exchangeRate;
        obj.vat = element.vat;
        obj.discountType = element.discountType;
        obj.discountValue = element.discountValue;
        obj.description = element.description;
        obj.orderDetailType = element.orderDetailType;
        obj.unitId = element.unitId;
        obj.incurredUnit = element.incurredUnit;
        obj.productCode = element.productCode;
        obj.nameMoneyUnit = element.nameMoneyUnit;
        obj.nameVendor = element.nameVendor;
        obj.productNameUnit = element.productNameUnit;
        obj.productName = element.productName;
        obj.unitLaborPrice = element.unitLaborPrice;
        obj.unitLaborNumber = element.unitLaborNumber;
        obj.orderNumber = index + 1;

        let sumprice = element.quantity * element.unitPrice * element.exchangeRate;
        if (obj.discountType) {
          obj.amountDiscount = sumprice * element.discountValue / 100;
        }
        else {
          obj.amountDiscount = sumprice - element.discountValue;
        }
        let vatAmount = (sumprice - obj.amountDiscount) * element.vat / 100;
        obj.sumAmount = this.roundNumber(sumprice - obj.amountDiscount + vatAmount, defaultNumberType);

        element.leadProductDetailProductAttributeValue.forEach(item => {
          let attributeValue: QuoteProductDetailProductAttributeValue = new QuoteProductDetailProductAttributeValue;
          attributeValue.productId = item.productId;
          attributeValue.productAttributeCategoryId = item.productAttributeCategoryId;
          attributeValue.productAttributeCategoryValueId = item.productAttributeCategoryValueId;
          obj.quoteProductDetailProductAttributeValue.push(attributeValue);
        });

        this.listQuoteDetailModel = [...this.listQuoteDetailModel, obj];
      });
      /*End*/
    }
    else {
      //Nếu có khách hàng đang được chọn thì list Hồ sơ thầu sẽ lấy theo khách hàng đang được chọn
      if (this.objCustomer) {
        this.listSaleBidding = this.listAllSaleBidding.filter(x => x.customerId == this.objCustomer.customerId);
      }
      //Nếu không có khách hàng đang được chọn thì list Hồ sơ thầu sẽ lấy theo người đang đăng nhập
      else {
        this.listSaleBidding = this.listAllSaleBidding;
      }
    }

    this.calculatorAmount();
  }

  /*Hiển thị lại Khách hàng, Email, Sđt, Địa chỉ, Số ngày được nợ, Số nợ tối đa theo CustomerId*/
  setDataFormCustomer(customer: Customer) {
    this.objectControl.setValue(customer);
    this.objCustomer = customer;
    this.email = customer.customerEmail;
    this.phone = customer.customerPhone;
    this.fullAddress = customer.fullAddress;

    //Số ngày được nợ
    let daysAreOwed = customer.maximumDebtDays ? customer.maximumDebtDays : 0;
    this.daysAreOwedControl.setValue(daysAreOwed);

    //Số nợ tối đa
    let maxAmount = customer.maximumDebtValue ? customer.maximumDebtValue : 0;
    this.maxDebtControl.setValue(maxAmount);

    //Reset Tài khoản ngân hàng
    this.isShowBankAccount = false;
    this.listBankAccount = [];
    this.bankAccountControl.setValue(null);
    //End

    //Phương thức thanh toán
    const toSelectPaymentMethod = this.listPaymentMethod.find(c => c.isDefault === true);
    this.paymentMethodControl.setValue(toSelectPaymentMethod);

    if (toSelectPaymentMethod?.categoryCode == "CASH") {
      this.isShowBankAccount = false;
    }
    else if (toSelectPaymentMethod?.categoryCode == "BANK") {
      this.isShowBankAccount = true;
    }
    else {
      this.isShowBankAccount = false;
    }

    //Nếu Phương thức thanh toán là Chuyển khoản thì lấy ra list account bank
    if (this.isShowBankAccount == true) {
      this.bankService.getAllBankAccountByObject(customer.customerId, this.customerContactCode).subscribe(response => {
        var result = <any>response;

        this.listBankAccount = result.bankAccountList;
        if (this.listBankAccount.length > 0) {
          let toSelectBankAccount = this.listBankAccount[0];
          this.bankAccountControl.setValue(toSelectBankAccount);
        }
      });
    }
  }

  /*Event thay đổi Hồ sơ thầu*/
  changeSaleBidding(value: any) {
    //list sản phẩm dịch vụ của báo giá sẽ bị xóa đi
    this.listQuoteDetailModel = [];

    //list Cơ hội sẽ bị xóa đi
    this.listLead = [];
    this.coHoiControl.reset();

    this.personInChargeControl.reset();

    //Nếu chọn Hồ sơ thầu
    if (value) {
      /*Khách hàng sẽ là khách hàng của Hồ sơ thầu*/
      let customerType = this.getCustomerType(value.customerId);
      //Là khách hàng Định danh
      if (customerType == 1) {
        this.objectList = this.listCustomer;
        let customer = this.listCustomer.find(x => x.customerId == value.customerId);

        if (customer) {
          this.setDataFormCustomer(customer);
        }
      }
      //Là khách hàng Tiềm năng
      else if (customerType == 0) {
        this.selectedObjectType = 'lea';
        this.objectList = this.listCustomerNew;
        let customerNew = this.listCustomerNew.find(x => x.customerId == value.customerId);

        if (customerNew) {
          this.setDataFormCustomer(customerNew);
        }
      }
      //khách hàng không xác định
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không xác định được khách hàng của Cơ hội' };
        this.showMessage(mgs);
      }
      /*End*/

      //Chọn Cơ hội gắn với Hồ sơ thầu
      let leadObj = this.listAllLead.find(l => l.leadId == value.leadId);
      if (leadObj) {
        this.listLead.push(leadObj);
        this.coHoiControl.setValue(leadObj);
      }

      //Nhân viên bán hàng (người phụ trách) của báo giá sẽ là người phụ trách của Hồ sơ thầu
      if (value.personInChargeId !== null) {
        let emp = this.listEmpSale.find(e => e.employeeId == value.personInChargeId);
        this.personInChargeControl.setValue(emp);
      }

      /*
      * Thêm các sản phẩm dịch vụ trong Hồ sơ vào list sản phẩm dịch vụ của Báo giá
      * Nhân viên bán hàng sẽ thay đổi là nhân viên phụ trách Hồ sơ thầu
      */
      value.saleBiddingDetail.forEach((element, index) => {
        let obj: QuoteDetail = new QuoteDetail;
        obj.productCategoryId = element.productCategory;
        obj.vendorId = element.vendorId;
        obj.productId = element.productId;
        obj.quantity = element.quantity;
        obj.unitPrice = element.unitPrice;
        obj.currencyUnit = element.currencyUnit;
        obj.exchangeRate = element.exchangeRate;
        obj.vat = element.vat;
        obj.discountType = element.discountType;
        obj.discountValue = element.discountValue;
        obj.description = element.description;
        obj.orderDetailType = element.orderDetailType;
        obj.unitId = element.unitId;
        obj.incurredUnit = element.incurredUnit;
        obj.productCode = element.productCode;
        obj.productName = element.productName;
        obj.nameMoneyUnit = element.nameMoneyUnit;
        obj.nameVendor = element.nameVendor;
        obj.productNameUnit = element.productNameUnit;
        obj.unitLaborPrice = element.unitLaborPrice;
        obj.unitLaborNumber = element.unitLaborNumber;
        obj.orderNumber = index + 1;

        let sumprice = element.quantity * element.unitPrice * element.exchangeRate;
        if (obj.discountType) {
          obj.amountDiscount = sumprice * element.discountValue / 100;
        }
        else {
          obj.amountDiscount = sumprice - element.discountValue;
        }
        let vatAmount = (sumprice - obj.amountDiscount) * element.vat / 100;
        obj.sumAmount = sumprice - obj.amountDiscount + vatAmount;

        element.saleBiddingDetailProductAttribute.forEach(item => {
          let attributeValue: QuoteProductDetailProductAttributeValue = new QuoteProductDetailProductAttributeValue;
          attributeValue.productId = item.productId;
          attributeValue.productAttributeCategoryId = item.productAttributeCategoryId;
          attributeValue.productAttributeCategoryValueId = item.productAttributeCategoryValueId;
          obj.quoteProductDetailProductAttributeValue.push(attributeValue);
        });

        this.listQuoteDetailModel = [...this.listQuoteDetailModel, obj];
      });
    }
    //Nếu bỏ chọn Hồ sơ thầu
    else {
      //Nếu có khách hàng đang được chọn thì list Cơ hội sẽ lấy theo khách hàng đang được chọn
      if (this.objCustomer) {
        this.listLead = this.listAllLead.filter(x => x.customerId == this.objCustomer.customerId);
      }
      //Nếu không có khách hàng đang được chọn thì list Cơ hội sẽ lấy theo người đang đăng nhập
      else {
        this.listLead = this.listAllLead;
      }
    }

    this.calculatorAmount();
  }

  /* Cập nhật báo giá */
  updateQuote() {
    if (!this.quoteForm.valid) {
      Object.keys(this.quoteForm.controls).forEach(key => {
        if (this.quoteForm.controls[key].valid == false) {
          this.quoteForm.controls[key].markAsTouched();
        }
      });
      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.quoteForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    } else if (this.listQuoteDetailModel.length == 0) {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Phải có ít nhất một sản phẩm dịch vụ được chọn' };
      this.showMessage(mgs);
    } else {
      if (this.uploadedFiles.length > 0) {
        this.uploadFiles(this.uploadedFiles);
        this.converToArrayQuoteDocument(this.uploadedFiles);

        // Xóa file trong control
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();
        }
      }

      /*Binding data for quoteModel*/
      let quote = this.mapDataToModel();

      //Lấy list người tham gia
      let listSelectedParticipant: Array<Employee> = this.participantControl.value;
      let listParticipant = listSelectedParticipant?.map(x => x.employeeId) ?? [];

      let minProfitExpect = parseFloat(this.minimumProfit.replace(/%/g, ''));
      let minProfit = this.valuePriceProfit;

      if (minProfit < minProfitExpect) {
        this.confirmationService.confirm({
          message: 'Mức lợi nhuận tạm tính thấp hơn mức kỳ vọng (' + minProfitExpect + '%), bạn có muốn tiếp tục không?',
          accept: () => {
            this.loading = true;
            this.UpdateQuote(quote, this.listQuoteDetailModel, 1,
              this.arrayQuoteDocumentModel, this.listAdditionalInformation,
              this.listQuoteCostDetailModel, false, this.emptyGuid, listParticipant, this.listPromotionApply, this.listQuotePlans, this.listQuoteScopes, this.listQuotePaymentTerm);
          },
        });
      } else {
        this.loading = true;
        this.UpdateQuote(quote, this.listQuoteDetailModel, 1,
          this.arrayQuoteDocumentModel, this.listAdditionalInformation,
          this.listQuoteCostDetailModel, false, this.emptyGuid, listParticipant, this.listPromotionApply, this.listQuotePlans, this.listQuoteScopes, this.listQuotePaymentTerm);
      }
    }
  }

  UpdateQuote(quote: Quote, listQuoteDetailModel: Array<QuoteDetail>, typeAccount: number, arrayQuoteDocumentModel: Array<QuoteDocument>,
    listAdditionalInformation: Array<any>, listQuoteCostDetailModel: Array<QuoteCostDetail>, isClone: boolean, quoteIdClone: string,
    listParticipant: string[], listPromotionApply: Array<PromotionObjectApply>, listQuotePlans: Array<QuotePlanModel>,
    listQuoteScopes: Array<QuoteScopeModel>, listQuotePaymentTerm: Array<QuotePaymentTerm>) {

    this.quoteService.CreateQuote(quote, listQuoteDetailModel, typeAccount, arrayQuoteDocumentModel,
      listAdditionalInformation, listQuoteCostDetailModel,
      isClone, quoteIdClone, listParticipant, listPromotionApply, listQuotePlans, listQuoteScopes, listQuotePaymentTerm).subscribe(response => {

        let result = <any>response;
        this.loading = false;

        if (result.statusCode == 200) {
          let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Lưu báo giá thành công" };
          this.showMessage(mgs);
          this.resetForm();
          this.getDataDefault();
        } else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
        }
      });
  }

  mapDataToModel(): Quote {
    let quote = new Quote();

    quote.quoteId = this.quoteId;
    quote.quoteCode = this.quoteCode;
    quote.quoteName = this.nameQuoteControl.value?.trim();
    quote.quoteDate = convertToUTCTime(new Date);
    let sendQuoteDate = (this.sendDateControl != null ? this.sendDateControl.value : null);
    if (sendQuoteDate) {
      sendQuoteDate = convertToUTCTime(sendQuoteDate);
      quote.sendQuoteDate = sendQuoteDate;
    } else {
      quote.sendQuoteDate = null;
    }

    let personInCharge: Employee = this.personInChargeControl.value != null ? this.personInChargeControl.value : null;
    if (personInCharge) {
      quote.personInChargeId = personInCharge.employeeId;
    } else {
      quote.personInChargeId = null;
    }
    quote.seller = quote.personInChargeId;

    let effectiveQuoteDate = parseFloat(this.effectiveDateControl.value.replace(/,/g, ''));
    quote.effectiveQuoteDate = effectiveQuoteDate;

    quote.expirationDate = this.expirationDate;

    let description = this.descriptionControl.value != null ? this.descriptionControl.value.trim() : '';
    quote.description = description;

    let note = this.noteControl.value != null ? this.noteControl.value.trim() : '';
    quote.note = note;

    quote.objectType = 'CUSTOMER';

    let object: Customer = this.objectControl.value;
    quote.objectTypeId = object.customerId;

    let paymentMethod: Category = this.paymentMethodControl.value != null ? this.paymentMethodControl.value : null;
    if (paymentMethod) {
      quote.paymentMethod = paymentMethod.categoryId;
    } else {
      quote.paymentMethod = null;
    }

    let discountType: DiscountType = this.discountTypeControl.value;
    quote.discountType = discountType.value;

    let bankAccount: BankAccount = this.bankAccountControl.value;
    if (bankAccount) {
      quote.bankAccountId = bankAccount.bankAccountId;
    } else {
      quote.bankAccountId = null;
    }

    let daysAreOwed = this.daysAreOwedControl.value != null ? parseFloat(this.daysAreOwedControl.value.replace(/,/g, '')) : 0;
    quote.daysAreOwed = daysAreOwed;

    let maxDebt = this.maxDebtControl.value != null ? parseFloat(this.maxDebtControl.value.replace(/,/g, '')) : 0;
    quote.maxDebt = maxDebt;

    quote.receivedDate = convertToUTCTime(new Date());
    quote.receivedHour = null;
    quote.recipientName = '';
    quote.locationOfShipment = '';
    quote.shippingNote = '';
    quote.recipientPhone = '';
    quote.recipientEmail = '';
    quote.placeOfDelivery = '';

    let intendedQuoteDate = this.intendedDateControl.value;
    quote.intendedQuoteDate = convertToUTCTime(intendedQuoteDate);

    quote.amount = this.amount;

    let discountValue = this.discountValueControl.value != null ? this.discountValueControl.value : '0';
    discountValue = parseFloat(discountValue.replace(/,/g, ''));
    quote.discountValue = discountValue;

    let statusQuote: Category = this.quoteStatusControl.value;
    quote.statusId = statusQuote.categoryId;

    let lead: any = this.coHoiControl.value != null ? this.coHoiControl.value : null;
    if (lead) {
      quote.leadId = lead.leadId;
    } else {
      quote.leadId = null;
    }

    let saleBillding: any = this.hoSoThauControl.value != null ? this.hoSoThauControl.value : null;
    if (saleBillding) {
      quote.saleBiddingId = saleBillding.saleBiddingId;
    } else {
      quote.saleBiddingId = null;
    }

    let investmentFundId: any = this.kenhControl.value != null ? this.kenhControl.value : null;
    if (investmentFundId) {
      quote.investmentFundId = investmentFundId.categoryId;
    } else {
      quote.investmentFundId = null;
    }

    quote.vat = ParseStringToFloat(this.vatControl.value.toString());
    quote.percentAdvance = ParseStringToFloat(this.percentAdvanceControl.value.toString());

    if (this.appName == 'VNS') {
      let type: DiscountType = this.percentAdvanceTypeControl.value;
      quote.percentAdvanceType = type.value;
      quote.constructionTime = this.constructionTimeControl.value != null ? this.constructionTimeControl.value : null;
    }

    quote.active = true;
    quote.createdById = this.emptyGuid;
    quote.createdDate = convertToUTCTime(new Date());
    quote.updatedById = this.emptyGuid;
    quote.updatedDate = convertToUTCTime(new Date());

    return quote;
  }

  cloneQuote() {
    let objClone = this.mapDataToModel();
    objClone.quoteId = this.emptyGuid;

    //Lấy list người tham gia
    let listSelectedParticipant: Array<Employee> = this.participantControl.value;
    let listParticipant = listSelectedParticipant.map(x => x.employeeId);

    this.quoteService.CreateQuote(objClone, this.listQuoteDetailModel, 1, this.arrayQuoteDocumentModel,
      this.listAdditionalInformation, this.listQuoteCostDetailModel,
      true, this.quoteId, listParticipant, this.listPromotionApply, this.listQuotePlans, this.listQuoteScopes, this.listQuotePaymentTerm).subscribe(response => {
        let result = <any>response;
        this.loading = false;

        if (result.statusCode == 200) {
          let messageCode = "Nhân bản báo giá thành công";
          let mgs = { severity: 'success', summary: 'Thông báo:', detail: messageCode };
          this.showMessage(mgs);
          this.quoteId = result.quoteID;
          this.router.navigate(['/customer/quote-detail', { quoteId: result.quoteID }]);
        } else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
        }
      });
  }

  resetForm() {
    this.objCustomer = null;
    this.selectedObjectType = 'cus';  //Loại khách hàng (Khách hàng tiềm năng hoặc Khách hàng)
    this.objectList = this.listCustomer;
    this.objectControl.reset();
    this.phone = '';
    this.email = '';
    this.fullAddress = '';
    this.paymentMethodControl.setValue(this.listPaymentMethod.find(x => x.isDefault == true));
    this.daysAreOwedControl.setValue('0');
    this.maxDebtControl.setValue('0');
    this.listBankAccount = [];
    this.bankAccountControl.setValue(null);
    this.isShowBankAccount = false;
    this.quoteStatusControl.setValue(this.listQuoteStatus.find(x => x.isDefault == true));
    this.intendedDateControl.setValue(new Date());
    this.sendDateControl.setValue(null);
    this.sendDateControl.setValidators(null);
    this.sendDateControl.updateValueAndValidity();
    this.effectiveDateControl.setValue('30');
    this.expirationDate = null;
    this.personInChargeControl.reset();
    this.hoSoThauControl.reset();
    this.coHoiControl.reset();
    this.kenhControl.reset();
    this.nameQuoteControl.reset();
    this.descriptionControl.setValue('');
    this.noteControl.setValue('');
    this.uploadedFiles = [];
    if (this.fileUpload) {
      this.fileUpload.clear();  //Xóa toàn bộ file trong control
    }
    this.titleText = '';
    this.contentText = '';
    this.isUpdateAI = false;
    this.listAdditionalInformation = [];
    this.arrayQuoteDocumentModel = [];
    this.listQuoteDetailModel = [];
    this.listQuoteCostDetailModel = [];
    this.discountTypeControl.setValue(this.discountTypeList.find(x => x.code == "PT"));
    this.discountValueControl.setValue('0');
    this.amount = 0;
    this.customerOrderAmountAfterDiscount = 0;
    this.amountPriceInitial = 0;
    this.amountPriceCost = 0;
    this.amountPriceProfit = 0;
    this.valuePriceProfit = 0;
  }

  quoteStatusBeforeClear: string = null;  //Biến lưu mã trạng thái báo giá trước khi compareDate() được thực hiện
  /*Event thay đổi Ngày gửi dự kiến và ngày hiệu lực*/
  compareDate() {
    let sendDate: Date = this.sendDateControl.value;
    let effectiveDate = this.effectiveDateControl.value;
    if (effectiveDate == null || effectiveDate == '') {
      this.effectiveDateControl.setValue('30');
      effectiveDate = 30;
    } else {
      effectiveDate = parseFloat(this.effectiveDateControl.value.replace(/,/g, ''));
    }

    let quoteStatus: Category = this.quoteStatusControl.value;
    this.quoteStatusBeforeClear = quoteStatus.categoryCode; //Lưu lại mã trạng thái
    if (sendDate) {
      //Tính: ngày hết hạn của báo giá = ngày gửi + ngày hiệu lực
      let current_miliseconds = sendDate.getTime();
      let result_miliseconds = current_miliseconds + effectiveDate * 1000 * 60 * 60 * 24;
      this.expirationDate = convertToUTCTime(new Date(result_miliseconds));
    }
  }

  /*Event Khi xóa ngày gửi (event này luôn được gọi sau event onBlur và onSelect)
  * Xử dụng event này trong trường hợp trạng thái của báo giá không thay đổi khi nó thuộc một trong các
  * trạng thái được quy định
  */
  clearSendDate() {
    //Xóa ngày hết hạn của báo giá vì ngày gửi hiện tại đã null
    this.expirationDate = null;
  }

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

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: QuoteDocument) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.arrayQuoteDocumentModel.indexOf(file);
        this.arrayQuoteDocumentModel.splice(index, 1);
      }
    });
  }

  /*Event upload list file*/
  myUploader(event: any) {
    if (this.uploadedFiles.length > 0) {
      this.uploadFiles(this.uploadedFiles);
      this.converToArrayQuoteDocument(this.uploadedFiles);

      // Xóa file trong control
      this.uploadedFiles = [];
      if (this.fileUpload) {
        this.fileUpload.clear();
      }
    }

    this.quoteService.UploadQuoteDocument(this.quoteId, this.arrayQuoteDocumentModel).subscribe(response => {
      var result = <any>response;
      if (result.statusCode == 200) {
        this.arrayQuoteDocumentModel = [];
        let listDocument = result.listQuoteDocument;
        listDocument.forEach(item => {
          let quoteDocument = new QuoteDocument();
          quoteDocument.quoteDocumentId = item.quoteDocumentId;
          quoteDocument.quoteId = item.quoteId;
          quoteDocument.documentName = item.documentName;
          quoteDocument.documentSize = item.documentSize;
          quoteDocument.documentUrl = item.documentUrl;
          quoteDocument.createdById = item.createdById;
          quoteDocument.createdDate = item.createdDate;
          quoteDocument.updatedById = item.updatedById;
          quoteDocument.updatedDate = item.updatedDate;
          quoteDocument.uploadByName = item.uploadByName;
          quoteDocument.active = item.active;

          this.arrayQuoteDocumentModel = [...this.arrayQuoteDocumentModel, quoteDocument];
          // Sắp xếp các file upload theo descending thời gian
          this.arrayQuoteDocumentModel.sort((a, b) => new Date(b.updatedDate).getTime() - new Date(a.updatedDate).getTime());
        });
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      } else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    })
  }

  /*Event khi xóa 1 file trong comment đã lưu trên server*/
  deleteNoteFile(file: NoteDocument) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listUpdateNoteDocument.indexOf(file);
        this.listUpdateNoteDocument.splice(index, 1);
      }
    });
  }

  /*Event khi download 1 file đã lưu trên server*/
  downloadFile(fileInfor: QuoteDocument) {
    this.imageService.downloadFile(fileInfor.documentName, fileInfor.documentUrl).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;
      var name = fileInfor.documentName;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(file);
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

  /* Tải file ở ghi chú (dòng thời gian) */
  downloadNoteFile(fileInfor: NoteDocument) {
    this.imageService.downloadFile(fileInfor.documentName, fileInfor.documentUrl).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;
      var name = fileInfor.documentName;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(file);
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

  /*Thêm một thông tin bổ sung*/
  addAI() {
    if (this.titleText == null || this.titleText.trim() == '') {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Tiêu đề không được để trống' };
      this.showMessage(msg);
    } else if (this.contentText == null || this.contentText.trim() == '') {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Nội dung không được để trống' };
      this.showMessage(msg);
    } else {
      this.maxOrdinal++;
      let note: AdditionalInformation = {
        ordinal: this.maxOrdinal,
        additionalInformationId: this.emptyGuid,
        objectId: this.emptyGuid,
        objectType: '',
        title: this.titleText.trim(),
        content: this.contentText.trim(),
        active: true,
        createdDate: new Date(),
        createdById: this.emptyGuid,
        updatedDate: null,
        updatedById: null,
        orderNumber: null
      };

      //Kiểm tra xem title đã tồn tại chưa
      let check = this.listAdditionalInformation.find(x => x.title == note.title);
      if (check) {
        //Nếu tồn tại rồi thì không cho thêm và hiển thị cảnh báo
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Tiêu đề này đã tồn tại' };
        this.showMessage(msg);
      } else {
        this.listAdditionalInformation.push(note);

        this.titleText = '';
        this.contentText = '';
      }
    }
  }

  /*Hiển thị lại thông tin bổ sung*/
  reShowNote(event: any) {
    let rowData: AdditionalInformation = event.data;
    this.isUpdateAI = true;
    this.AIUpdate.ordinal = rowData.ordinal;
    this.AIUpdate.title = rowData.title;

    this.titleText = rowData.title;
    this.contentText = rowData.content;
  }

  /*Hủy cập nhật thông tin bổ sung*/
  cancelAI() {
    this.isUpdateAI = false;
    this.AIUpdate = {
      ordinal: null,
      additionalInformationId: this.emptyGuid,
      objectId: this.emptyGuid,
      objectType: '',
      title: null,
      content: null,
      active: true,
      createdDate: new Date(),
      createdById: this.emptyGuid,
      updatedDate: null,
      updatedById: null,
      orderNumber: null
    };
    this.titleText = '';
    this.contentText = '';
  }

  /*Cập nhật thông tin bổ sung*/
  updateContentAI() {
    if (this.titleText == null || this.titleText.trim() == '') {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Tiêu đề không được để trống' };
      this.showMessage(msg);
    } else if (this.contentText == null || this.contentText.trim() == '') {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Nội dung không được để trống' };
      this.showMessage(msg);
    } else {
      var check = this.listAdditionalInformation.find(x => x.title == this.AIUpdate.title);
      if (check) {
        //Kiểm tra xem title đã tồn tại chưa
        let checkDublicate = this.listAdditionalInformation.find(x => x.title == this.titleText.trim() && x.ordinal != this.AIUpdate.ordinal);

        if (checkDublicate) {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Tiêu đề này đã tồn tại' };
          this.showMessage(msg);
        } else {
          this.listAdditionalInformation.forEach(item => {
            if (item.title == this.AIUpdate.title) {
              item.title = this.titleText.trim();
              item.content = this.contentText.trim();
            }
          });

          //reset form
          this.isUpdateAI = false;
          this.AIUpdate = {
            ordinal: null,
            additionalInformationId: this.emptyGuid,
            objectId: this.emptyGuid,
            objectType: '',
            title: null,
            content: null,
            active: true,
            createdDate: new Date(),
            createdById: this.emptyGuid,
            updatedDate: null,
            updatedById: null,
            orderNumber: null
          };
          this.titleText = '';
          this.contentText = '';
        }
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Không tồn tại thông tin bổ sung này' };
        this.showMessage(msg);
      }
    }
  }

  /*Xóa thông tin bổ sung*/
  deleteAI(rowData: AdditionalInformation) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listAdditionalInformation.indexOf(rowData);
        this.listAdditionalInformation.splice(index, 1);
      }
    });
  }

  /*Tạo báo giá NCC*/
  async showDialogQuoteVendor() {
    if (this.statusCode === 'MTA') {
      await this.getInforDetailQuote();

      var listQuoteVendor: Array<QuoteDetail> = [];
      this.listQuoteDetailModel.forEach(item => {
        //Hiện tại chỉ báo giá ncc đối với sản phẩm
        if (item.orderDetailType == 0) {
          let obj: QuoteDetail = new QuoteDetail();
          obj.quoteDetailId = item.quoteDetailId;
          obj.vendorId = item.vendorId;
          obj.quoteId = item.quoteId;
          obj.productId = item.productId;
          obj.quantity = item.quantity;
          obj.unitPrice = item.unitPrice;
          obj.currencyUnit = item.currencyUnit;
          obj.exchangeRate = item.exchangeRate;
          obj.vat = item.vat;
          obj.discountType = item.discountType;
          obj.discountValue = item.discountValue;
          obj.description = item.description;
          obj.orderDetailType = item.orderDetailType;
          obj.unitId = item.unitId;
          obj.active = item.active;
          obj.createdById = item.createdById;
          obj.createdDate = item.createdDate;
          obj.updatedById = item.updatedById;
          obj.updatedDate = item.updatedDate;
          obj.quoteProductDetailProductAttributeValue = item.quoteProductDetailProductAttributeValue;
          obj.productCode = item.productCode;
          obj.productName = item.productName;
          obj.nameVendor = item.nameVendor;
          obj.productNameUnit = item.productNameUnit;
          obj.nameMoneyUnit = item.nameMoneyUnit;
          obj.sumAmount = item.sumAmount;
          obj.amountDiscount = item.amountDiscount;
          obj.priceInitial = item.priceInitial;
          obj.isPriceInitial = item.isPriceInitial;

          obj.productCode = this.listProduct.find(p => p.productId == item.productId).productCode;

          listQuoteVendor.push(obj);
        }
      });

      let ref = this.dialogService.open(AddQuoteVendorDialogComponent, {
        data: {
          listQuoteDetailModel: listQuoteVendor,
          quoteId: this.quoteId
        },
        header: 'Tạo đề nghị báo giá nhà cung cấp',
        width: '70%',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "280px",
          "max-height": "600px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: ResultDialog) => {
        if (result) {
          if (result.status) {

          }
        }
      });
    }
  }

  /*Thêm sản phẩm dịch vụ*/
  addCustomerOrderDetail() {
    if (this.statusCode === 'MTA') {
      let cusGroupId = null;
      if (this.objCustomer !== null && this.objCustomer !== undefined) cusGroupId = this.objCustomer.customerGroupId;
      let ref = this.dialogService.open(AddEditProductDialogComponent, {
        data: {
          isCreate: true,
          cusGroupId: cusGroupId,
          orderDate: this.quoteDate,
          type: 'SALE',
          statusCode: this.statusCode,
        },
        header: 'Thêm sản phẩm dịch vụ',
        width: '70%',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "280px",
          "max-height": "600px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe(async (result: ResultDialog) => {
        if (result) {
          if (result.status) {
            let quoteDetailModel: QuoteDetail = result.quoteDetailModel;

            //set orderNumber cho sản phẩm/dịch vụ mới thêm
            quoteDetailModel.orderNumber = this.listQuoteDetailModel.length + 1;

            //Kiểm tra sản phẩm có đủ điều kiện nhận quà KM?
            if (quoteDetailModel.productId != null && quoteDetailModel.productId != this.emptyGuid) {
              let response: any = await this.promotionService.checkPromotionByProduct(quoteDetailModel.productId, quoteDetailModel.quantity);
              quoteDetailModel.isPromotionProduct = response.isPromotionProduct;
            }

            this.listQuoteDetailModel = [...this.listQuoteDetailModel, quoteDetailModel];
            this.calculatorAmount();
          }
        }
      });
    }
  }

  /*Sửa một sản phẩm dịch vụ*/
  onRowSelect(dataRow) {
    //Nếu có quyền sửa thì mới cho sửa
    var index = this.listQuoteDetailModel.indexOf(dataRow);
    var OldArray = this.listQuoteDetailModel[index];

    let cusGroupId = null;
    if (this.objCustomer !== null && this.objCustomer !== undefined) cusGroupId = this.objCustomer.customerGroupId;
    let ref = this.dialogService.open(AddEditProductDialogComponent, {
      data: {
        isCreate: false,
        cusGroupId: cusGroupId,
        quoteDetailModel: OldArray,
        orderDate: this.quoteDate,
        statusCode: this.statusCode,
        actionEdit: this.actionEdit,
      },
      header: 'Sửa sản phẩm dịch vụ',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe(async (result: ResultDialog) => {
      if (result) {
        if (result.status) {
          this.listQuoteDetailModel.splice(index, 1);
          let quoteDetailModel = result.quoteDetailModel;

          //Kiểm tra sản phẩm có đủ điều kiện nhận quà KM?
          if (quoteDetailModel.productId != null && quoteDetailModel.productId != this.emptyGuid) {
            let response: any = await this.promotionService.checkPromotionByProduct(quoteDetailModel.productId, quoteDetailModel.quantity);
            quoteDetailModel.isPromotionProduct = response.isPromotionProduct;
          }

          this.listQuoteDetailModel = [...this.listQuoteDetailModel, quoteDetailModel];
          this.reOrderListQuoteDetail();

          this.calculatorAmount();
        }
      }
    });
  }

  /*Xóa một sản phẩm dịch vụ*/
  deleteItem(dataRow) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listQuoteDetailModel = this.listQuoteDetailModel.filter(e => e != dataRow);
        //Đánh lại số OrderNumber
        this.listQuoteDetailModel.forEach((item, index) => {
          item.orderNumber = index + 1;
        });

        this.calculatorAmount();
      }
    });
  }

  /*Thêm một chi phí*/
  addCostQuote() {
    let ref = this.dialogService.open(PopupAddEditCostQuoteDialogComponent, {
      data: {
        isCreate: true
      },
      header: 'Thêm chi phí',
      width: '30%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: ResultCostDialog) => {
      if (result) {
        if (result.status) {
          let quoteCostDetailModel: QuoteCostDetail = result.quoteDetailModel;
          this.listQuoteCostDetailModel = [...this.listQuoteCostDetailModel, quoteCostDetailModel];

          this.calculatorAmount();
        }
      }
    });
  }

  /*Sửa một chi phí*/
  onRowCostSelect(dataRow) {
    //Nếu có quyền sửa thì mới cho sửa
    if (this.actionEdit && this.statusCode === 'MTA') {
      var index = this.listQuoteCostDetailModel.indexOf(dataRow);
      var OldArray = this.listQuoteCostDetailModel[index];

      let titlePopup = 'Sửa chi phí';

      let ref = this.dialogService.open(PopupAddEditCostQuoteDialogComponent, {
        data: {
          isCreate: false,
          quoteDetailModel: OldArray
        },
        header: titlePopup,
        width: '30%',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "280px",
          "max-height": "600px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: ResultCostDialog) => {
        if (result) {
          if (result.status) {
            let quoteCostDetailModel: QuoteCostDetail = result.quoteDetailModel;
            this.listQuoteCostDetailModel[index] = quoteCostDetailModel;

            this.calculatorAmount();
          }
        }
      });
    }
  }

  /*Xóa một chi phí*/
  deleteCostItem(dataRow) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listQuoteCostDetailModel.indexOf(dataRow);
        this.listQuoteCostDetailModel.splice(index, 1);
        this.listQuoteCostDetailModel = [...this.listQuoteCostDetailModel];

        this.calculatorAmount();
      }
    });
  }

  showTotalQuote() {
    this.isShow = !this.isShow;
    this.colLeft = this.isShow ? 8 : 12;
    if (this.isShow) {
      window.scrollTo(0, 0)
    }
  }

  /*Event khi thay đổi loại chiết khấu: Theo % hoặc Theo số tiền*/
  changeDiscountType(value: DiscountType) {
    this.discountValueControl.setValue('0');

    if (this.appName == 'VNS') {
      //Danh sách sản phẩm dịch vụ: Reset chiết khấu của tất cả sản phẩm về 0
      this.listQuoteDetailModel.forEach(item => {
        item.discountValue = 0;
      });
    }

    this.calculatorAmount();
  }

  /*Event khi thay đổi giá trị chiết khấu*/
  changeDiscountValue() {
    if (this.appName == 'VNS') {
      //Danh sách sản phẩm dịch vụ: Reset chiết khấu của tất cả sản phẩm về 0
      this.listQuoteDetailModel.forEach(item => {
        item.discountValue = 0;
      });
    }

    let discountValue = 0;
    if (this.discountValueControl.value.trim() == '') {
      discountValue = 0;
      this.discountValueControl.setValue('0');
    } else {
      discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
    }

    let discountType = this.discountTypeControl.value;
    let codeDiscountType = discountType.code;
    //Nếu loại chiết khấu là theo % thì giá trị discountValue không được lớn hơn 100%
    if (codeDiscountType == "PT") {
      if (discountValue > 100) {
        discountValue = 100;
        this.discountValueControl.setValue('100');
      }
    }

    this.calculatorAmount();

    /*
    * Nếu số thành tiền âm (vì nếu loại chiết khấu là % thì giá trị chiết khấu lớn nhất là 100%
    * nên số thành tiền không thể âm, vậy nếu số thành tiền âm thì chỉ có trường hợp giá trị
    * chiết khấu lúc này là Số tiền)
    */
    if (this.customerOrderAmountAfterDiscount < 0) {
      this.customerOrderAmountAfterDiscount = 0;
      this.discountValueControl.setValue(this.amount.toString());
    }
    /*End*/

    this.calculatorAmount();
  }

  uploadFiles(files: File[]) {
    this.imageService.uploadFile(files).subscribe(response => { });
  }

  goDetailLead() {
    this.router.navigate(['/lead/detail', { leadId: this.leadObj.leadId }]);
  }

  cancel() {
    // this.router.navigate(['/customer/quote-list']);
    this.location.back();
  }

  createOrder() {
    this.router.navigate(['/order/create', { quoteID: this.quoteId }]);
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  gotoCreateContract() {
    this.router.navigate(['/sales/contract-create', { quoteId: this.quoteId }]);
  }

  async exportExcel(type) {
    if (this.appName == 'VNS') {
      this.quoteService.getDataExportExcelQuote(this.quoteId).subscribe(response => {
        let result: any = response;
        if (result.statusCode == 200) {
          // Láy tên của khách hàng hoặc khách hàng tiềm năng
          let cusName: string = '';
          if (this.selectedObjectType == 'cus') {
            let customer: any = this.objectList.find(x => x.customerId == this.objCustomer.customerId);
            cusName = customer.customerName;
          } else if (this.selectedObjectType == 'lea') {
            let customer: any = this.objectList.find(x => x.customerId == this.objCustomer.customerId);
            cusName = customer.customerName;
          }

          let DateCHO = "";
          let status = this.listQuoteStatus.find(s => s.categoryId == result.quote.statusId);
          if (status.categoryCode == 'CHO') {
            DateCHO = this.datePipe.transform(result.quote.updatedDate, 'dd/MM/yyyy');
          }
          // convert tên khách hàng trên tên báo giá theo format : ten_khách_hàng báo giá ngày xuất file
          let cusNameExport = convertStringCusName(cusName);

          let dateUTC = new Date();

          let title = cusNameExport + ' báo giá ' + dateUTC.getDate() + '_' + (dateUTC.getMonth() + 1) + '_' + dateUTC.getUTCFullYear();
          let workbook = new Workbook();
          let worksheet = workbook.addWorksheet(title);
          worksheet.pageSetup.margins = {
            left: 0.85, right: 0.25,
            top: 0.75, bottom: 0.75,
            header: 0.3, footer: 0.3
          };
          worksheet.pageSetup.paperSize = 9;  //A4 : 8

          let logo1 = this.logo1();
          let imgBase64 = this.getBase64Logo();

          /* Image */
          var imgLogo1 = workbook.addImage({
            base64: logo1,
            extension: 'jpeg',
          });

          worksheet.addImage(imgLogo1, {
            tl: { col: 0, row: 0 },
            br: { col: 2.5, row: 2 },
            ext: { width: 155, height: 95 }
          });

          /* Image */
          var imgLogo2 = workbook.addImage({
            base64: imgBase64,
            extension: 'png',
          });

          worksheet.addImage(imgLogo2, {
            tl: { col: 8.6, row: 0 },
            br: { col: 10, row: 2 },
            ext: { width: 155, height: 95 }
          });

          worksheet.addRow([]);
          worksheet.addRow([]);

          let dataRow1 = [];
          dataRow1[6] = 'Nhà phân phối neolith tại việt nam'.toUpperCase() + '';  //Tên công ty
          let row1 = worksheet.addRow(dataRow1);
          row1.font = { name: 'Arial', size: 8, bold: true }; //color: { argb: '97A8B4' },
          row1.alignment = { vertical: 'top', horizontal: 'right' };
          worksheet.mergeCells(`F${row1.number}:J${row1.number}`);

          let dataRow2 = [];
          dataRow2[6] = 'KCN Trường An, An Khánh, Hoài Đức, Hà Nội 📍';
          let row2 = worksheet.addRow(dataRow2);
          row2.font = { name: 'Arial', size: 8 };
          row2.alignment = { vertical: 'top', horizontal: 'right' };
          worksheet.mergeCells(`F${row2.number}:J${row2.number}`);

          let dataRow3 = [];
          dataRow3[6] = '(+84) 24 6259 7010 | 093 504 3355 ✆';
          let row3 = worksheet.addRow(dataRow3);
          row3.font = { name: 'Arial', size: 8 };
          row3.alignment = { vertical: 'top', horizontal: 'right' };
          worksheet.mergeCells(`F${row3.number}:J${row3.number}`);

          let dataRow4 = [];
          dataRow4[6] = 'neolith.com | neolith.vn 🌐';
          let row4 = worksheet.addRow(dataRow4);
          row4.font = { name: 'Arial', size: 8 };
          row4.alignment = { vertical: 'top', horizontal: 'right' };
          worksheet.mergeCells(`F${row4.number}:J${row4.number}`);

          worksheet.addRow([]);
          worksheet.addRow([]);

          let dataRow5 = [];
          dataRow5[1] = 'BÁO GIÁ';
          let row5 = worksheet.addRow(dataRow5);
          row5.font = { name: 'Arial', size: 12, bold: true };
          row5.alignment = { vertical: 'top', horizontal: 'left' };

          worksheet.addRow([]);

          let dataRow6 = [];
          dataRow6[1] = 'KHÁCH HÀNG: ' + cusName.toUpperCase();
          let row6 = worksheet.addRow(dataRow6);
          row6.font = { name: 'Arial', size: 8, bold: true };
          worksheet.mergeCells(`A${row6.number}:E${row6.number}`);
          row6.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
          row6.height = 24;

          let dataRow7 = [];
          dataRow7[1] = 'ĐỊA CHỈ: ' + this.fullAddress?.toUpperCase();
          dataRow7[7] = 'Mã số đơn hàng';
          dataRow7[9] = this.quoteCode;
          let row7 = worksheet.addRow(dataRow7);
          row7.font = { name: 'Arial', size: 8 };
          worksheet.mergeCells(`A12:E13`);
          worksheet.mergeCells(`G${row7.number}:H${row7.number}`);
          worksheet.mergeCells(`I${row7.number}:J${row7.number}`);
          row7.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
          worksheet.getCell('G12').font = { name: 'Arial', size: 8, bold: true };

          worksheet.getCell('G13').value = 'Ngày';
          worksheet.mergeCells(`G13:H13`);
          worksheet.getCell('G13').font = { name: 'Arial', size: 8 };
          worksheet.getCell('G13').alignment = { vertical: "middle", horizontal: 'left', wrapText: true };

          worksheet.getCell('I13').value = this.datePipe.transform(this.quoteDate, 'dd/MM/yyyy');
          worksheet.mergeCells(`I13:J13`);
          worksheet.getCell('I13').font = { name: 'Arial', size: 8 };
          worksheet.getCell('I13').alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

          let dataRow8 = [];
          dataRow8[1] = 'SĐT: ' + this.phone;
          dataRow8[7] = 'Người phụ trách';
          dataRow8[9] = result.quote.sellerName;
          let row8 = worksheet.addRow(dataRow8);
          row8.font = { name: 'Arial', size: 8 };
          worksheet.mergeCells(`A${row8.number}:E${row8.number}`);
          worksheet.mergeCells(`G${row8.number}:H${row8.number}`);
          worksheet.mergeCells(`I${row8.number}:J${row8.number}`);
          row8.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

          let dataRow9 = [];
          dataRow9[1] = 'EMAIL: ' + this.email;
          let row9 = worksheet.addRow(dataRow9);
          row9.font = { name: 'Arial', size: 8 };
          worksheet.mergeCells(`A${row9.number}:E${row9.number}`);
          row9.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

          /* add border */
          worksheet.getCell('A11').border = { left: { style: "thin" }, top: { style: "thin" }, right: { style: "thin" } };
          worksheet.getCell('E13').border = { left: { style: "thin" }, right: { style: "thin" } };
          worksheet.getCell('E14').border = { left: { style: "thin" }, right: { style: "thin" } };
          worksheet.getCell('A15').border = { bottom: { style: "thin" } };
          worksheet.getCell('B15').border = { bottom: { style: "thin" } };
          worksheet.getCell('C15').border = { bottom: { style: "thin" } };
          worksheet.getCell('D15').border = { bottom: { style: "thin" } };
          worksheet.getCell('E15').border = { left: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

          worksheet.getCell('G11').border = { left: { style: "thin" }, top: { style: "thin" } };
          worksheet.getCell('H11').border = { top: { style: "thin" } };
          worksheet.getCell('I11').border = { top: { style: "thin" } };
          worksheet.getCell('J11').border = { top: { style: "thin" } };
          worksheet.getCell('J11').border = { top: { style: "thin" }, right: { style: "thin" } };
          worksheet.getCell('G12').border = { left: { style: "thin" } };
          worksheet.getCell('J12').border = { right: { style: "thin" } };
          worksheet.getCell('G13').border = { left: { style: "thin" } };
          worksheet.getCell('J13').border = { right: { style: "thin" } };
          worksheet.getCell('G14').border = { left: { style: "thin" } };
          worksheet.getCell('J14').border = { right: { style: "thin" } };
          worksheet.getCell('G15').border = { left: { style: "thin" }, bottom: { style: "thin" } };
          worksheet.getCell('H15').border = { bottom: { style: "thin" } };
          worksheet.getCell('I15').border = { bottom: { style: "thin" } };
          worksheet.getCell('J15').border = { bottom: { style: "thin" } };
          worksheet.getCell('J15').border = { bottom: { style: "thin" }, right: { style: "thin" } };

          worksheet.addRow([]);

          /* Header row */
          let dataHeaderRow = ['STT', 'Tên sản phẩm', '', '', '', 'ĐVT', 'Khối lượng', 'Đơn giá', 'Nhân công', 'Thành tiền'];
          let headerRow = worksheet.addRow(dataHeaderRow);
          headerRow.font = { name: 'Arial', size: 8, bold: true };
          dataHeaderRow.forEach((item, index) => {
            headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

            if (index == 1) {
              headerRow.getCell(index + 1).worksheet.mergeCells(
                `${headerRow.getCell(index + 1).$col$row}:${headerRow.getCell(index + 4).$col$row}`
              );
              headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
            }
            else if (index != 1 && index != 2 && index != 3 && index != 4) {
              headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
            }
          });

          /* Data table */
          let data: Array<any> = [];

          let tong = 0; //Tổng
          let chietKhau = this.totalAmountDiscount; //Chiết khấu
          let tongGiaSauChietKhau = 0; //Tổng giá sau chiết khấu
          let thueVat = this.totalAmountVat; //Thuế VAT
          let tongGiaSauThue = this.totalAmountAfterVat; //Tổng giá sau thuế
          let tamUng = this.amountAdvance; //Tạm ứng

          this.listQuoteDetailModel.forEach((item, index) => {
            let row: Array<any> = [];
            row[0] = index + 1;
            row[1] = item.productName;
            row[2] = '';
            row[3] = '';
            row[4] = '';
            row[5] = item.productNameUnit;
            row[6] = this.decimalPipe.transform(item.quantity.toString());
            row[7] = this.decimalPipe.transform(item.unitPrice.toString());
            row[8] = this.decimalPipe.transform(item.unitLaborPrice.toString());

            let sumAmount = item.quantity * (item.unitPrice + item.unitLaborPrice);
            row[9] = this.decimalPipe.transform(sumAmount.toString());

            data.push(row);

            /* Phần tổng kết */
            tong += sumAmount;
            /* End */
          });

          tongGiaSauChietKhau = tong - chietKhau;

          let count = data.length ?? 0;
          let countRow = 0;

          data.forEach((el, index, array) => {
            let row = worksheet.addRow(el);
            row.font = { name: 'Arial', size: 8 };
            row.height = 15.71;

            if (count == index + 1) {
              row.getCell(1).border = { left: { style: "thin" }, right: { style: "thin" } };
              row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };

              row.getCell(2).border = { left: { style: "thin" }, right: { style: "thin" } };
              row.getCell(2).worksheet.mergeCells(
                `${row.getCell(2).$col$row}:${row.getCell(5).$col$row}`
              );
              row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

              row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
              row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
              row.getCell(5).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

              row.getCell(6).border = { left: { style: "thin" }, right: { style: "thin" } };
              row.getCell(6).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

              row.getCell(7).border = { left: { style: "thin" }, right: { style: "thin" } };
              row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

              row.getCell(8).border = { left: { style: "thin" }, right: { style: "thin" } };
              row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

              row.getCell(9).border = { left: { style: "thin" }, right: { style: "thin" } };
              row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

              row.getCell(10).border = { left: { style: "thin" }, right: { style: "thin" } };
              row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true }; 1

              // them dong trang
              countRow = 15 - this.listQuoteDetailModel.length;
              if (countRow > 0) {
                while (countRow > 0) {
                  let emptyRow = worksheet.addRow([]);
                  emptyRow.getCell(1).border = { left: { style: "thin" }, right: { style: "thin" } };
                  emptyRow.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };

                  emptyRow.getCell(2).border = { left: { style: "thin" }, right: { style: "thin" } };
                  emptyRow.getCell(2).worksheet.mergeCells(
                    `${emptyRow.getCell(2).$col$row}:${emptyRow.getCell(5).$col$row}`
                  );
                  emptyRow.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

                  emptyRow.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
                  emptyRow.getCell(4).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
                  emptyRow.getCell(5).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

                  emptyRow.getCell(6).border = { left: { style: "thin" }, right: { style: "thin" } };
                  emptyRow.getCell(6).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

                  emptyRow.getCell(7).border = { left: { style: "thin" }, right: { style: "thin" } };
                  emptyRow.getCell(7).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

                  emptyRow.getCell(8).border = { left: { style: "thin" }, right: { style: "thin" } };
                  emptyRow.getCell(8).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

                  emptyRow.getCell(9).border = { left: { style: "thin" }, right: { style: "thin" } };
                  emptyRow.getCell(9).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

                  emptyRow.getCell(10).border = { left: { style: "thin" }, right: { style: "thin" } };
                  emptyRow.getCell(10).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
                  countRow--;
                }
              }

              // them mot dong trang cuoi bang
              let rowBotton1 = worksheet.addRow([]);
              rowBotton1.getCell(1).border = { left: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              rowBotton1.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };

              rowBotton1.getCell(2).border = { left: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              rowBotton1.getCell(2).worksheet.mergeCells(
                `${rowBotton1.getCell(2).$col$row}:${rowBotton1.getCell(5).$col$row}`
              );
              rowBotton1.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

              rowBotton1.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
              rowBotton1.getCell(4).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
              rowBotton1.getCell(5).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

              rowBotton1.getCell(6).border = { left: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              rowBotton1.getCell(6).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

              rowBotton1.getCell(7).border = { left: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              rowBotton1.getCell(7).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

              rowBotton1.getCell(8).border = { left: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              rowBotton1.getCell(8).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

              rowBotton1.getCell(9).border = { left: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              rowBotton1.getCell(9).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

              rowBotton1.getCell(10).border = { left: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              rowBotton1.getCell(10).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            }
            else {
              row.getCell(1).border = { left: { style: "thin" }, right: { style: "thin" } };
              row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };

              row.getCell(2).border = { left: { style: "thin" }, right: { style: "thin" } };
              row.getCell(2).worksheet.mergeCells(
                `${row.getCell(2).$col$row}:${row.getCell(5).$col$row}`
              );
              row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

              row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
              row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
              row.getCell(5).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

              row.getCell(6).border = { left: { style: "thin" }, right: { style: "thin" } };
              row.getCell(6).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

              row.getCell(7).border = { left: { style: "thin" }, right: { style: "thin" } };
              row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

              row.getCell(8).border = { left: { style: "thin" }, right: { style: "thin" } };
              row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

              row.getCell(9).border = { left: { style: "thin" }, right: { style: "thin" } };
              row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

              row.getCell(10).border = { left: { style: "thin" }, right: { style: "thin" } };
              row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            }
          });

          //Điều khoản thanh toán
          let dataTempRow1 = [];
          dataTempRow1[1] = 'Điều khoản thanh toán';

          let type = this.percentAdvanceTypeControl.value
          if (type.value) {
            dataTempRow1[3] = '- Tạm ứng trước ' + this.percentAdvanceControl.value + '% giá trị đơn hàng';
          } else {
            dataTempRow1[3] = '- Tạm ứng trước ' + this.percentAdvanceControl.value + 'đ giá trị đơn hàng';
          }

          dataTempRow1[7] = `Tổng`;
          dataTempRow1[10] = this.decimalPipe.transform(tong.toString());
          let tempRow1 = worksheet.addRow(dataTempRow1);

          let dataTempRow2 = [];
          dataTempRow2[3] = '- Quyết toán phần còn lại sau khi nghiệm thu công trình.';
          dataTempRow2[7] = 'Chiết khấu';
          dataTempRow2[10] = this.decimalPipe.transform(chietKhau.toString());
          let tempRow2 = worksheet.addRow(dataTempRow2);

          let dataTempRow3 = [];
          dataTempRow3[7] = 'Tổng giá sau chiết khấu';
          dataTempRow3[10] = this.decimalPipe.transform(tongGiaSauChietKhau.toString());
          let tempRow3 = worksheet.addRow(dataTempRow3);

          tempRow1.font = { name: 'Arial', size: 8 };
          tempRow2.font = { name: 'Arial', size: 8 };
          tempRow3.font = { name: 'Arial', size: 8 };

          worksheet.mergeCells(`${tempRow1.getCell(1).$col$row}:${tempRow3.getCell(2).$col$row}`);
          worksheet.mergeCells(`${tempRow1.getCell(3).$col$row}:${tempRow1.getCell(5).$col$row}`);
          worksheet.mergeCells(`${tempRow1.getCell(7).$col$row}:${tempRow1.getCell(9).$col$row}`);
          worksheet.mergeCells(`${tempRow2.getCell(3).$col$row}:${tempRow3.getCell(5).$col$row}`);
          worksheet.mergeCells(`${tempRow2.getCell(7).$col$row}:${tempRow2.getCell(9).$col$row}`);
          worksheet.mergeCells(`${tempRow3.getCell(7).$col$row}:${tempRow3.getCell(9).$col$row}`);

          tempRow1.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
          tempRow2.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
          tempRow3.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
          worksheet.getCell(tempRow1.getCell(10).$col$row).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          worksheet.getCell(tempRow2.getCell(10).$col$row).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          worksheet.getCell(tempRow3.getCell(10).$col$row).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

          let dataTempRow4 = [];
          dataTempRow4[1] = 'Chú ý';
          dataTempRow4[3] = `Đối với những đơn hàng dưới 10.000.000đ (Mười triệu đồng) thêm phí hỗ trợ v/c là 500.000đ (Năm trăm nghìn đồng)`;
          dataTempRow4[7] = 'Thuế VAT';
          dataTempRow4[10] = this.decimalPipe.transform(thueVat.toString());
          let tempRow4 = worksheet.addRow(dataTempRow4);

          let dataTempRow5 = [];
          dataTempRow5[7] = 'Tổng giá sau thuế';
          dataTempRow5[10] = this.decimalPipe.transform(tongGiaSauThue.toString());
          let tempRow5 = worksheet.addRow(dataTempRow5);

          let dataTempRow6 = [];
          dataTempRow6[7] = 'Tạm ứng';
          dataTempRow6[10] = this.decimalPipe.transform(tamUng.toString());
          let tempRow6 = worksheet.addRow(dataTempRow6);

          tempRow4.font = { name: 'Arial', size: 8 };
          tempRow5.font = { name: 'Arial', size: 8 };
          tempRow6.font = { name: 'Arial', size: 8 };

          worksheet.mergeCells(`${tempRow4.getCell(1).$col$row}:${tempRow6.getCell(2).$col$row}`);
          worksheet.mergeCells(`${tempRow4.getCell(3).$col$row}:${tempRow6.getCell(5).$col$row}`);
          worksheet.mergeCells(`${tempRow4.getCell(7).$col$row}:${tempRow4.getCell(9).$col$row}`);
          worksheet.mergeCells(`${tempRow5.getCell(7).$col$row}:${tempRow5.getCell(9).$col$row}`);
          worksheet.mergeCells(`${tempRow6.getCell(7).$col$row}:${tempRow6.getCell(9).$col$row}`);

          tempRow4.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
          tempRow5.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
          tempRow6.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
          worksheet.getCell(tempRow4.getCell(10).$col$row).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          worksheet.getCell(tempRow5.getCell(10).$col$row).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          worksheet.getCell(tempRow5.getCell(10).$col$row).font = { name: 'Arial', size: 8, bold: true };
          worksheet.getCell(tempRow6.getCell(10).$col$row).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
          worksheet.getCell(tempRow6.getCell(10).$col$row).font = { name: 'Arial', size: 8, bold: true };

          //border
          tempRow1.getCell(1).border = { left: { style: "thin" } };
          tempRow1.getCell(3).border = { right: { style: "thin" } };
          tempRow1.getCell(7).border = { left: { style: "thin" } };
          tempRow1.getCell(10).border = { left: { style: "thin" }, right: { style: "thin" } };

          tempRow2.getCell(3).border = { right: { style: "thin" } };
          tempRow2.getCell(7).border = { left: { style: "thin" } };
          tempRow2.getCell(10).border = { left: { style: "thin" }, right: { style: "thin" } };

          tempRow3.getCell(7).border = { left: { style: "thin" } };
          tempRow3.getCell(10).border = { left: { style: "thin" }, right: { style: "thin" } };

          tempRow4.getCell(1).border = { left: { style: "thin" }, bottom: { style: "thin" } };
          tempRow4.getCell(3).border = { right: { style: "thin" }, bottom: { style: "thin" } };
          tempRow4.getCell(7).border = { left: { style: "thin" } };
          tempRow4.getCell(10).border = { left: { style: "thin" }, right: { style: "thin" } };

          tempRow5.getCell(7).border = { left: { style: "thin" } };
          tempRow5.getCell(10).border = { left: { style: "thin" }, right: { style: "thin" } };

          tempRow6.getCell(6).border = { bottom: { style: "thin" } };
          tempRow6.getCell(7).border = { left: { style: "thin" }, bottom: { style: "thin" } };
          tempRow6.getCell(10).border = { left: { style: "thin" }, right: { style: "thin" }, bottom: { style: "thin" } };

          worksheet.addRow([]);

          if (countRow < 0) {
            let currentRow = tempRow6.number + 2;
            let tmp = 53 - currentRow;
            if (tmp > 0) {
              while (tmp > 0) {
                worksheet.addRow([]);
                tmp--;
              }
            }
          }

          let dataTempRow7 = [];
          dataTempRow7[1] = 'Chuyển khoản qua STK: 68388368 - Nguyễn Thị Lan Hương - Ngân hàng Á Châu (ACB) - Chi nhánh Hà Nội';
          let tempRow7 = worksheet.addRow(dataTempRow7);
          tempRow7.font = { name: 'Arial', size: 8 };

          let dataTempRow8 = [];
          let tempRow8 = worksheet.addRow(dataTempRow8);
          worksheet.getCell(tempRow8.getCell(1).$col$row).value = {
            'richText': [
              { 'font': { 'size': 8, 'name': 'Arial' }, 'text': 'Khi chuyển khoản, quý khách hàng vui lòng ghi nội dung chuyển khoản theo cú pháp: "' },
              { 'font': { 'size': 8, 'name': 'Arial', 'italic': true }, 'text': 'CKĐH chị/anh A' },
              { 'font': { 'size': 8, 'name': 'Arial' }, 'text': '"' },
            ]
          };
          tempRow8.font = { name: 'Arial', size: 8 };

          let dataTempRow9 = [];
          dataTempRow9[1] = 'Thời gian thi công ' + (this.constructionTimeControl.value != null ? this.constructionTimeControl.value : '.....') + ' ngày, không kể ngày lễ và chủ nhật, tính từ ngày ký hợp đồng.';
          let tempRow9 = worksheet.addRow(dataTempRow9);
          tempRow9.font = { name: 'Arial', size: 8 };

          let dataTempRow10 = [];
          dataTempRow10[1] = 'Lưu ý : Giá trị tạm ứng của đơn hàng phải được xác nhận qua chuyển khoản hoặc qua phiếu thu do Công ty Cổ phần Đá tự nhiên VNS phát hành.';
          let tempRow10 = worksheet.addRow(dataTempRow10);
          tempRow10.font = { name: 'Arial', size: 8, italic: true };
          tempRow10.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
          tempRow10.height = 29;
          worksheet.mergeCells(`A${tempRow10.number}:J${tempRow10.number}`);

          worksheet.addRow([]);

          let dataTempRow11 = [];
          dataTempRow11[2] = 'Khách hàng'.toUpperCase();
          dataTempRow11[5] = 'Kinh doanh'.toUpperCase();
          dataTempRow11[8] = 'Giám đốc'.toUpperCase();
          let tempRow11 = worksheet.addRow(dataTempRow11);
          tempRow11.font = { name: 'Arial', size: 8, bold: true };
          tempRow11.alignment = { vertical: "middle", horizontal: "center", wrapText: true };
          worksheet.mergeCells(`B${tempRow11.number}:D${tempRow11.number}`);
          worksheet.mergeCells(`E${tempRow11.number}:G${tempRow11.number}`);
          worksheet.mergeCells(`H${tempRow11.number}:J${tempRow11.number}`);

          let dataTempRow12 = [];
          dataTempRow12[2] = '(Ký tên và ghi rõ họ tên)';
          dataTempRow12[5] = '(Ký tên và ghi rõ họ tên)';
          dataTempRow12[8] = '(Ký tên, đóng dấu)';
          let tempRow12 = worksheet.addRow(dataTempRow12);
          tempRow12.font = { name: 'Arial', size: 8, italic: true };
          tempRow12.alignment = { vertical: "middle", horizontal: "center", wrapText: true };
          worksheet.mergeCells(`B${tempRow12.number}:D${tempRow12.number}`);
          worksheet.mergeCells(`E${tempRow12.number}:G${tempRow12.number}`);
          worksheet.mergeCells(`H${tempRow12.number}:J${tempRow12.number}`);

          worksheet.headerFooter.oddFooter = "&L&\"Arial\"&8NHÀ PHÂN PHỐI NEOLITH TẠI VIỆT NAM &C&\"Arial\"&8&P &R&\"Arial\"&8neolith.vn | neolith.com | stonex.vn";

          //Sang page 2
          tempRow12.addPageBreak();

          let dataRowPageTemp1 = [];
          dataRowPageTemp1[1] = 'Thông tin bảo hành'.toUpperCase();
          let rowPageTemp1 = worksheet.addRow(dataRowPageTemp1);
          rowPageTemp1.font = { name: 'Arial', size: 11, bold: true };
          rowPageTemp1.alignment = { vertical: "middle", horizontal: "center", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp1.number}:J${rowPageTemp1.number}`);

          let dataRowPageTemp2 = [];
          dataRowPageTemp2[1] = '1. bảo hành mặt bàn và mặt bếp'.toUpperCase();
          let rowPageTemp2 = worksheet.addRow(dataRowPageTemp2);
          rowPageTemp2.font = { name: 'Arial', size: 8, bold: true };

          let dataRowPageTemp3 = [];
          dataRowPageTemp3[1] = '- Thời gian bảo hành: 25 năm';
          let rowPageTemp3 = worksheet.addRow(dataRowPageTemp3);
          rowPageTemp3.font = { name: 'Arial', size: 8 };

          let dataRowPageTemp4 = [];
          dataRowPageTemp4[1] = '- Áp dụng độ dày: 12 mm và 20 mm';
          let rowPageTemp4 = worksheet.addRow(dataRowPageTemp4);
          rowPageTemp4.font = { name: 'Arial', size: 8 };

          let dataRowPageTemp5 = [];
          dataRowPageTemp5[1] = '- Chế độ bảo hành sẽ được áp dụng cho việc sửa chữa hoặc thay thế đá bị lỗi bao gồm mặt bàn, mặt bếp được lắp đặt cố định tại nơi ở của người sử dụng.';
          let rowPageTemp5 = worksheet.addRow(dataRowPageTemp5);
          rowPageTemp5.font = { name: 'Arial', size: 8 };
          rowPageTemp5.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          rowPageTemp5.height = 29;
          worksheet.mergeCells(`A${rowPageTemp5.number}:J${rowPageTemp5.number}`);

          let dataRowPageTemp6 = [];
          dataRowPageTemp6[1] = '2. Bảo hành đá ốp bề mặt ngoài công trình'.toUpperCase();
          let rowPageTemp6 = worksheet.addRow(dataRowPageTemp6);
          rowPageTemp6.font = { name: 'Arial', size: 8, bold: true };

          let dataRowPageTemp7 = [];
          dataRowPageTemp7[1] = '- Thời gian bảo hành: 10 năm';
          let rowPageTemp7 = worksheet.addRow(dataRowPageTemp7);
          rowPageTemp7.font = { name: 'Arial', size: 8 };
          rowPageTemp7.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp7.number}:J${rowPageTemp7.number}`);

          let dataRowPageTemp8 = [];
          dataRowPageTemp8[1] = '- Áp dụng độ dày: 6mm và 12mm';
          let rowPageTemp8 = worksheet.addRow(dataRowPageTemp8);
          rowPageTemp8.font = { name: 'Arial', size: 8 };
          rowPageTemp8.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp8.number}:J${rowPageTemp8.number}`);

          let dataRowPageTemp9 = [];
          dataRowPageTemp9[1] = '- Áp dụng bề mặt: Satin, Silk và Riverwashed';
          let rowPageTemp9 = worksheet.addRow(dataRowPageTemp9);
          rowPageTemp9.font = { name: 'Arial', size: 8 };
          rowPageTemp9.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp9.number}:J${rowPageTemp9.number}`);

          let dataRowPageTemp10 = [];
          dataRowPageTemp10[1] = '- Chế độ bảo hành sẽ được áp dụng cho việc sửa chữa hoặc thay thế đá ốp bề mặt ngoài của công trình bị lỗi được lắp đặt cố định.';
          let rowPageTemp10 = worksheet.addRow(dataRowPageTemp10);
          rowPageTemp10.font = { name: 'Arial', size: 8 };
          rowPageTemp10.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp10.number}:J${rowPageTemp10.number}`);

          let dataRowPageTemp11 = [];
          dataRowPageTemp11[1] = '3. BẢO HÀNH THI CÔNG'.toUpperCase();
          let rowPageTemp11 = worksheet.addRow(dataRowPageTemp11);
          rowPageTemp11.font = { name: 'Arial', size: 8, bold: true };

          let dataRowPageTemp12 = [];
          dataRowPageTemp12[1] = '- Thời gian bảo hành: 2 năm';
          let rowPageTemp12 = worksheet.addRow(dataRowPageTemp12);
          rowPageTemp12.font = { name: 'Arial', size: 8 };
          rowPageTemp12.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp12.number}:J${rowPageTemp12.number}`);

          let dataRowPageTemp13 = [];
          dataRowPageTemp13[1] = '- Áp dụng độ dày 3 mm, 6 mm và 12 mm';
          let rowPageTemp13 = worksheet.addRow(dataRowPageTemp13);
          rowPageTemp13.font = { name: 'Arial', size: 8 };
          rowPageTemp13.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp13.number}:J${rowPageTemp13.number}`);

          let dataRowPageTemp14 = [];
          dataRowPageTemp14[1] = '- Các vấn đề thi công được bảo hành bao gồm: bong bột, gãy vỡ do chất lượng thi công.';
          let rowPageTemp14 = worksheet.addRow(dataRowPageTemp14);
          rowPageTemp14.font = { name: 'Arial', size: 8 };
          rowPageTemp14.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp14.number}:J${rowPageTemp14.number}`);

          let dataRowPageTemp15 = [];
          dataRowPageTemp15[1] = '4. TRƯỜNG HỢP KHÔNG ÁP DỤNG BẢO HÀNH'.toUpperCase();
          let rowPageTemp15 = worksheet.addRow(dataRowPageTemp15);
          rowPageTemp15.font = { name: 'Arial', size: 8, bold: true };

          let dataRowPageTemp16 = [];
          let rowPageTemp16 = worksheet.addRow(dataRowPageTemp16);
          worksheet.getCell(rowPageTemp16.getCell(1).$col$row).value = {
            'richText': [
              { 'font': { 'size': 8, 'name': 'Arial' }, 'text': 'Hư hỏng do lạm dụng hành động vật lý (những tác động mạnh bất thường), hóa học (các chất tẩy rửa hóa học) và cơ học hoặc sử dụng không đúng mục đích (được đề cập trong tài liệu ' },
              { 'font': { 'size': 8, 'name': 'Arial', 'bold': true }, 'text': '"Hướng dẫn sử dụng đá Neolith bề mặt bóng"' },
              { 'font': { 'size': 8, 'name': 'Arial' }, 'text': ' và ' },
              { 'font': { 'size': 8, 'name': 'Arial', 'bold': true }, 'text': '"Sổ tay hướng dẫn sử dụng chăm sóc bề mặt đá Neolith"' },
              { 'font': { 'size': 8, 'name': 'Arial' }, 'text': ').' },
            ]
          };
          rowPageTemp16.font = { name: 'Arial', size: 8 };
          rowPageTemp16.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp16.number}:J${rowPageTemp16.number}`);
          rowPageTemp16.height = 43;

          let dataRowPageTemp17 = [];
          dataRowPageTemp17[1] = '- Hư hỏng do các thay đổi về cấu trúc, điều kiện môi trường, thiên tai.';
          let rowPageTemp17 = worksheet.addRow(dataRowPageTemp17);
          rowPageTemp17.font = { name: 'Arial', size: 8 };
          rowPageTemp17.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp17.number}:J${rowPageTemp17.number}`);

          let dataRowPageTemp18 = [];
          dataRowPageTemp18[1] = '- Bề mặt đã được di chuyển khỏi vị trí lắp đặt ban đầu, tính chất bị thay đổi so với nguyên bản theo bất kì cách nào.';
          let rowPageTemp18 = worksheet.addRow(dataRowPageTemp18);
          rowPageTemp18.font = { name: 'Arial', size: 8 };
          rowPageTemp18.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp18.number}:J${rowPageTemp18.number}`);

          let dataRowPageTemp19 = [];
          dataRowPageTemp19[1] = '- Sử dụng sản phẩm cho các khu vực dành cho việc vận chuyển người hoặc máy móc.';
          let rowPageTemp19 = worksheet.addRow(dataRowPageTemp19);
          rowPageTemp19.font = { name: 'Arial', size: 8 };
          rowPageTemp19.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp19.number}:J${rowPageTemp19.number}`);

          let dataRowPageTemp20 = [];
          dataRowPageTemp20[1] = '- Không tuân thủ các thông số kỹ thuật của sản phẩm, thiết kế và sử dụng theo cách bất thường và/hoặc trong điều kiện vượt quá giới hạn độ giãn được nêu trong các thông số kỹ thuật.';
          let rowPageTemp20 = worksheet.addRow(dataRowPageTemp20);
          rowPageTemp20.font = { name: 'Arial', size: 8 };
          rowPageTemp20.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp20.number}:J${rowPageTemp20.number}`);
          rowPageTemp20.height = 29;

          let dataRowPageTemp21 = [];
          dataRowPageTemp21[1] = '- Bề mặt có sự xuất hiện của kết cấu hoặc đường nối, chất kết dính, bột bả và/hoặc các vật liệu phụ kiện khác.';
          let rowPageTemp21 = worksheet.addRow(dataRowPageTemp21);
          rowPageTemp21.font = { name: 'Arial', size: 8 };
          rowPageTemp21.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp21.number}:J${rowPageTemp21.number}`);

          let dataRowPageTemp22 = [];
          dataRowPageTemp22[1] = '- Hư hỏng do việc lắp đặt các thiết bị gia dụng, sử dụng hoặc bảo trì không đúng cách.';
          let rowPageTemp22 = worksheet.addRow(dataRowPageTemp22);
          rowPageTemp22.font = { name: 'Arial', size: 8 };
          rowPageTemp22.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp22.number}:J${rowPageTemp22.number}`);

          let dataRowPageTemp23 = [];
          dataRowPageTemp23[1] = '- Các ý kiến chủ quan của khách hàng về thẩm mỹ sau khi sản phẩm được sản xuất hoặc lắp đặt.';
          let rowPageTemp23 = worksheet.addRow(dataRowPageTemp23);
          rowPageTemp23.font = { name: 'Arial', size: 8 };
          rowPageTemp23.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp23.number}:J${rowPageTemp23.number}`);

          let dataRowPageTemp24 = [];
          dataRowPageTemp24[1] = '- Sửa chữa bổ sung hoặc thay đổi hình dạng khi cần để lắp đặt lại các hệ thống (hệ thống ống nước, điện, công trình xây dựng, v.v.).';
          let rowPageTemp24 = worksheet.addRow(dataRowPageTemp24);
          rowPageTemp24.font = { name: 'Arial', size: 8 };
          rowPageTemp24.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp24.number}:J${rowPageTemp24.number}`);

          let dataRowPageTemp25 = [];
          let rowPageTemp25 = worksheet.addRow(dataRowPageTemp25);
          worksheet.getCell(rowPageTemp25.getCell(1).$col$row).value = {
            'richText': [
              { 'font': { 'size': 8, 'name': 'Arial' }, 'text': '- ' },
              { 'font': { 'size': 8, 'name': 'Arial', 'bold': true }, 'text': 'Đặc biệt đối với bề mặt Polished, ' },
              { 'font': { 'size': 8, 'name': 'Arial' }, 'text': 'Neolith không áp dụng bảo hành cho bất kỳ vết bẩn nào do tiếp xúc trực tiếp với thuốc tẩy, clo hoặc các tác nhân hóa học khác có độ pH trên 11 hoặc vết xước do sử dụng không đúng hướng dẫn của Neolith.' },
            ]
          };
          rowPageTemp25.font = { name: 'Arial', size: 8 };
          rowPageTemp25.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp25.number}:J${rowPageTemp25.number}`);
          rowPageTemp25.height = 29;

          let dataRowPageTemp26 = [];
          dataRowPageTemp26[1] = '- Tùy theo lô sản xuất, màu sắc của các sản phẩm cùng loại có thể có những sai khác nhất định, Neolith không bảo hành cho trường hợp này.';
          let rowPageTemp26 = worksheet.addRow(dataRowPageTemp26);
          rowPageTemp26.font = { name: 'Arial', size: 8 };
          rowPageTemp26.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp26.number}:J${rowPageTemp26.number}`);

          let dataRowPageTemp27 = [];
          dataRowPageTemp27[1] = '*Lưu ý:';
          let rowPageTemp27 = worksheet.addRow(dataRowPageTemp27);
          rowPageTemp27.font = { name: 'Arial', size: 8, italic: true };
          rowPageTemp27.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp27.number}:J${rowPageTemp27.number}`);

          let dataRowPageTemp28 = [];
          dataRowPageTemp28[1] = '- Việc sửa chữa hoặc thay thế sẽ phụ thuộc vào mẫu mã, màu sắc và bề mặt có sẵn tại thời điểm bảo hành.';
          let rowPageTemp28 = worksheet.addRow(dataRowPageTemp28);
          rowPageTemp28.font = { name: 'Arial', size: 8, italic: true };
          rowPageTemp28.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp28.number}:J${rowPageTemp28.number}`);

          let dataRowPageTemp29 = [];
          dataRowPageTemp29[1] = '- Thời hạn bảo hành sẽ không được chuyển nhượng và sẽ không có hiệu lực nếu các sản phẩm trên chưa được thanh toán đầy đủ.';
          let rowPageTemp29 = worksheet.addRow(dataRowPageTemp29);
          rowPageTemp29.font = { name: 'Arial', size: 8, italic: true };
          rowPageTemp29.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp29.number}:J${rowPageTemp29.number}`);

          let dataRowPageTemp30 = [];
          dataRowPageTemp30[1] = '- Đối với bề mặt Polished, không dùng miếng rửa bát thô, dung dịch kiềm, chất tẩy rửa ăn mòn hoặc các sản phẩm có độ pH trên 11 để làm sạch. Tránh để tiếp xúc trực tiếp với thuốc tẩy hoặc clo, không cắt thái trực tiếp thực phẩm trên bề mặt, không chà sát.';
          let rowPageTemp30 = worksheet.addRow(dataRowPageTemp30);
          rowPageTemp30.font = { name: 'Arial', size: 8, italic: true };
          rowPageTemp30.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp30.number}:J${rowPageTemp30.number}`);
          rowPageTemp30.height = 43;

          let dataRowPageTemp31 = [];
          let rowPageTemp31 = worksheet.addRow(dataRowPageTemp31);
          worksheet.getCell(rowPageTemp31.getCell(1).$col$row).value = {
            'richText': [
              { 'font': { 'size': 8, 'name': 'Arial', 'italic': true }, 'text': '- Việc sử dụng và chăm sóc mặt đá được khuyến cáo sử dụng theo chỉ dẫn tài liệu ' },
              { 'font': { 'size': 8, 'name': 'Arial', 'italic': true, 'bold': true }, 'text': '"Hướng dẫn sử dụng đá Neolith bề mặt bóng"' },
              { 'font': { 'size': 8, 'name': 'Arial', 'italic': true }, 'text': ' và ' },
              { 'font': { 'size': 8, 'name': 'Arial', 'italic': true, 'bold': true }, 'text': '"Sổ tay hướng dẫn sử dụng chăm sóc bề mặt đá Neolith"' },
              { 'font': { 'size': 8, 'name': 'Arial', 'italic': true }, 'text': ' đi kèm.' },
            ]
          };
          rowPageTemp31.font = { name: 'Arial', size: 8, italic: true };
          rowPageTemp31.alignment = { vertical: "middle", horizontal: "left", wrapText: true };
          worksheet.mergeCells(`A${rowPageTemp31.number}:J${rowPageTemp31.number}`);
          rowPageTemp31.height = 29;

          worksheet.getColumn(1).width = 5;
          worksheet.getColumn(5).width = 11;
          worksheet.getColumn(6).width = 6;
          worksheet.getColumn(10).width = 11;

          this.exportToExel(workbook, title);
        }
      });
    }
    else {
      await this.getInforDetailQuote();
      this.quoteService.getDataExportExcelQuote(this.quoteId).subscribe(response => {
        let result: any = response;
        if (result.statusCode == 200) {
          var dateNow = new Date();
          // Láy tên của khách hàng hoặc khách hàng tiềm năng
          let cusName: string = '';
          if (this.selectedObjectType == 'cus') {
            let customer: any = this.objectList.find(x => x.customerId == this.objCustomer.customerId);
            cusName = customer.customerName;
          } else if (this.selectedObjectType == 'lea') {
            let customer: any = this.objectList.find(x => x.customerId == this.objCustomer.customerId);
            cusName = customer.customerName;
          }

          let DateCHO = "";
          let status = this.listQuoteStatus.find(s => s.categoryId == result.quote.statusId);
          if (status.categoryCode == 'CHO') {
            DateCHO = this.datePipe.transform(result.quote.updatedDate, 'dd/MM/yyyy');
          }

          let intendedQuoteDate = this.datePipe.transform(result.quote.intendedQuoteDate, 'dd/MM/yyyy');

          let imgBase64 = this.getBase64Logo();

          // lay ten bao gia
          let quoteName = result.quote.quoteName;
          // convert tên báo giá theo format : ten_báo_giá
          let quoteNameExport = convertStringCusName(quoteName)

          let titleQuote = quoteNameExport;
          let workbook = new Workbook();
          let worksheet = workbook.addWorksheet(titleQuote, { views: [{ showGridLines: false }] });
          worksheet.pageSetup.margins = {
            top: 0.25, bottom: 0.75,
            left: 0.25, right: 0.5,
            header: 0.3, footer: 0.05
          };
          worksheet.pageSetup.paperSize = 9;  //A4 : 9

          let colA = worksheet.getColumn('A');
          colA.width = 3

          let colB = worksheet.getColumn('B');
          colB.width = 5.42;

          let colC = worksheet.getColumn('C');
          colC.width = 21.14;

          let colD = worksheet.getColumn('D');
          colD.width = 8.57;

          let colE = worksheet.getColumn('E');
          colE.width = 11.85;

          let colF = worksheet.getColumn('F');
          colF.width = 14.71;

          let colG = worksheet.getColumn('G');
          colG.width = 10.86;

          let colH = worksheet.getColumn('H');
          colH.width = 9.42;

          let colI = worksheet.getColumn('I');
          colI.width = 14.71;

          /* Image */
          var imgLogo = workbook.addImage({
            base64: imgBase64,
            extension: 'png',
          });

          worksheet.addImage(imgLogo, {
            tl: { col: 1, row: 2 },
            ext: { width: 153, height: 76 }
          });

          let dataRow1 = result.inforExportExcel.companyName.toUpperCase();  //Tên công ty
          let row1 = worksheet.getRow(3);
          row1.getCell('D').value = dataRow1;
          row1.font = { name: 'Times New Roman', size: 13, bold: true };
          worksheet.mergeCells(`D${row1.number}:I${row1.number}`);
          row1.alignment = { vertical: 'top', horizontal: 'center', wrapText: true };
          row1.height = 33.75;

          let dataRow2 = 'Địa chỉ: ' + result.inforExportExcel.address;  //Địa chỉ
          let row2 = worksheet.getRow(4);
          row2.getCell('D').value = dataRow2;
          row2.font = { name: 'Times New Roman', size: 13, color: { argb: '003366' } };
          worksheet.mergeCells(`D${row2.number}:I${row2.number}`);
          row2.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
          row2.height = 35.25;

          let dataRow3 = 'Điện thoại: ' + result.inforExportExcel.phone;  //Số điện thoại
          let row3 = worksheet.getRow(5);
          row3.getCell('D').value = dataRow3
          row3.font = { name: 'Times New Roman', size: 13, color: { argb: '003366' } };
          worksheet.mergeCells(`D${row3.number}:I${row3.number}`);
          row3.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

          let dataRow4 = 'Website: ' + result.inforExportExcel.website;  //Địa chỉ website
          let row4 = worksheet.getRow(6);
          row4.getCell('D').value = dataRow4
          row4.font = { name: 'Times New Roman', size: 13, color: { argb: '003366' } };
          worksheet.mergeCells(`D${row4.number}:I${row4.number}`);
          row4.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

          let dataRow5 = 'Email: ' + result.inforExportExcel.email; // email
          let row5 = worksheet.getRow(7);
          row5.getCell('D').value = dataRow5;
          row5.font = { name: 'Times New Roman', size: 13, color: { argb: '003366' } };
          worksheet.mergeCells(`D${row5.number}:I${row5.number}`);
          row5.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

          let dataRow6 = quoteName.toUpperCase();
          let row6 = worksheet.getRow(8);
          row6.getCell('C').value = dataRow6;
          row6.font = { name: 'Times New Roman', size: 13, bold: true };
          row6.height = 40;
          worksheet.mergeCells(`C${row6.number}:I${row6.number}`);
          row6.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

          let dataRow7 = [];
          dataRow7[3] = 'Kính gửi: ' + cusName.toUpperCase();
          let row7 = worksheet.addRow(dataRow7);
          row7.font = { name: 'Times New Roman', size: 13, bold: true };
          worksheet.mergeCells(`C${row7.number}:I${row7.number}`);
          row7.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

          worksheet.addRow([]);

          let dataRow8 = [];
          dataRow8[2] = result.inforExportExcel.companyName + ' gửi đến quý công ty báo giá ' + quoteName + ' với các nội dung sau:';
          let row8 = worksheet.addRow(dataRow8);
          row8.font = { name: 'Times New Roman', size: 13 };
          row8.height = 40;
          worksheet.mergeCells(`B${row8.number}:I${row8.number}`);
          row8.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

          let dataRow9 = [];
          dataRow9[2] = 'I. Danh sách sản phẩm/dịch vụ';
          let row9 = worksheet.addRow(dataRow9);
          row9.font = { name: 'Times New Roman', size: 13, underline: 'single', bold: true };
          row9.height = 33;
          worksheet.mergeCells(`B${row9.number}:I${row9.number}`);
          row9.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

          let dataRow10 = [];
          dataRow10[2] = "Đơn vị tiền: VND";
          let row10 = worksheet.addRow(dataRow10);
          row10.font = { name: 'Times New Roman', size: 13 };
          worksheet.mergeCells(`B${row10.number}:I${row10.number}`);
          row10.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

          /* Header row */
          let dataHeaderRow = ['', 'STT', 'Tên sản phẩm/dịch vụ', 'Đơn vị', 'Số lượng', 'Đơn giá', 'Chiết khấu', 'VAT(%)', 'Thành tiền'];
          let headerRow = worksheet.addRow(dataHeaderRow);
          headerRow.font = { name: 'Times New Roman', size: 13, bold: true };
          dataHeaderRow.forEach((item, index) => {
            if (item != '') {
              headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
              headerRow.getCell(index + 1).fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: '8DB4E2' }
              };
            }
          });
          headerRow.height = 40;

          /* Data table */
          let data: Array<any> = []; //[1, 'Dịch vụ CNTT', 'Gói', '2', '6.000.000', '12.000.000']

          let totalNotVat = 0;  //Số tiền chưa VAT
          let totalVat = 0; // Tổng VÁT của tất cả sản phẩm

          // Lấy dữ liệu trực tiếp từ data ko cần mapping
          for (var index = 0; index < result.listQuoteDetail.length; ++index) {

            let totalNotVatTemp = result.listQuoteDetail[index].quantity * result.listQuoteDetail[index].unitPrice * result.listQuoteDetail[index].exchangeRate + result.listQuoteDetail[index].unitLaborPrice * result.listQuoteDetail[index].unitLaborNumber * result.listQuoteDetail[index].exchangeRate;

            let row: Array<any> = [];
            row[0] = '';
            row[1] = index + 1;
            row[2] = (result.listQuoteDetail[index].nameProduct == null || result.listQuoteDetail[index].nameProduct == '') ? result.listQuoteDetail[index].description : result.listQuoteDetail[index].nameProduct;
            row[3] = (result.listQuoteDetail[index].discountType == 0) ? result.listQuoteDetail[index].nameProductUnit : result.listQuoteDetail[index].nameProductUnit;
            row[4] = this.decimalPipe.transform(result.listQuoteDetail[index].quantity).toString();
            row[5] = this.decimalPipe.transform((result.listQuoteDetail[index].unitPrice).toString());
            row[6] = this.decimalPipe.transform(result.listQuoteDetail[index].discountValue).toString();
            row[7] = this.decimalPipe.transform(result.listQuoteDetail[index].vat).toString();

            // Thành tiền từng sản phẩm sau chiết khấu và VAT
            if (result.listQuoteDetail[index].discountType) {
              // Thành tiền từng sản phẩm
              row[8] = this.decimalPipe.transform(((totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100)) +
                (totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100)) * (result.listQuoteDetail[index].vat / 100)).toString());

              totalVat += (totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100)) * (result.listQuoteDetail[index].vat / 100);
              totalNotVat += totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100);

            } else if (!result.listQuoteDetail[index].discountType) {
              // Thành tiền từng sản phẩm
              row[8] = this.decimalPipe.transform(((totalNotVatTemp - result.listQuoteDetail[index].discountValue)) +
                ((totalNotVatTemp - result.listQuoteDetail[index].discountValue) * (result.listQuoteDetail[index].vat / 100))).toString();

              totalVat += (totalNotVatTemp - result.listQuoteDetail[index].discountValue) * (result.listQuoteDetail[index].vat / 100);
              totalNotVat += totalNotVatTemp - result.listQuoteDetail[index].discountValue;
            }

            data.push(row);
          }

          data.forEach((el, index, array) => {
            if (el != '') {
              let row = worksheet.addRow(el);
              row.font = { name: 'Times New Roman', size: 13 };
              row.height = 38.71;

              row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };


              row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

              row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

              row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              row.getCell(5).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

              row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              row.getCell(6).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

              row.getCell(7).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

              row.getCell(8).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

              row.getCell(9).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
            }
          });

          /* VAT */
          let dataVatRow = ['', 'Tổng số tiền chưa VAT', '', '', '', '', '', '', this.decimalPipe.transform(totalNotVat.toString())];
          let vatRow = worksheet.addRow(dataVatRow);
          worksheet.mergeCells(`B${vatRow.number}:H${vatRow.number}`);
          dataVatRow.forEach((item, index) => {
            if (index != 0) {
              vatRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              if (index + 1 < 9) {
                vatRow.getCell(2).alignment = { vertical: 'middle', horizontal: 'right' };
              } else {
                vatRow.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' };
              }
            }
          });
          vatRow.font = { name: 'Times New Roman', size: 13, bold: true };
          vatRow.height = 25.71;

          /* value VAT  */
          let dataValueVatRow = ['', 'VAT', '', '', '', '', '', '', this.decimalPipe.transform(totalVat.toString())];
          let valueVatRow = worksheet.addRow(dataValueVatRow);
          worksheet.mergeCells(`B${valueVatRow.number}:H${valueVatRow.number}`);
          dataValueVatRow.forEach((item, index) => {
            if (index != 0) {
              valueVatRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              if (index + 1 < 9) {
                valueVatRow.getCell(2).alignment = { vertical: 'middle', horizontal: 'right' };
              } else {
                valueVatRow.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' };
              }
            }
          });
          valueVatRow.font = { name: 'Times New Roman', size: 13, bold: true };
          valueVatRow.height = 25.71;

          let totalNotDicount = totalNotVat + totalVat;
          /* Số tiền chết khấu*/
          let discountMoney = 0;
          if (result.quote.discountType) {
            discountMoney = totalNotDicount * result.quote.discountValue / 100;
          } else {
            discountMoney = result.quote.discountValue;
          }
          let dataDiscountQouteMoney = ['', 'Chiết khấu theo báo giá', '', '', '', '', '', '', this.decimalPipe.transform(discountMoney.toString())];
          let discountQouteMoney = worksheet.addRow(dataDiscountQouteMoney);
          worksheet.mergeCells(`B${discountQouteMoney.number}:H${discountQouteMoney.number}`);
          dataDiscountQouteMoney.forEach((item, index) => {
            if (index != 0) {
              discountQouteMoney.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              if (index + 1 < 9) {
                discountQouteMoney.getCell(2).alignment = { vertical: 'middle', horizontal: 'right' };
              } else {
                discountQouteMoney.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' };
              }
            }
          });
          discountQouteMoney.font = { name: 'Times New Roman', size: 13, bold: true };
          discountQouteMoney.height = 25.71;

          /* Số tiền phải thanh toán */
          let total = totalNotDicount - discountMoney;
          let dataTotalMoney = ['', 'Tổng số tiền phải thanh toán', '', '', '', '', '', '', this.decimalPipe.transform(total.toString())];
          let totalMoney = worksheet.addRow(dataTotalMoney);
          worksheet.mergeCells(`B${totalMoney.number}:H${totalMoney.number}`);
          dataTotalMoney.forEach((item, index) => {
            if (index != 0) {
              totalMoney.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              if (index + 1 < 9) {
                totalMoney.getCell(2).alignment = { vertical: 'middle', horizontal: 'right' };
              } else {
                totalMoney.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' };
              }
            }
          });
          totalMoney.font = { name: 'Times New Roman', size: 13, bold: true };
          totalMoney.height = 25.71;

          /* Số tiền bằng chữ */
          let dataStringMoney = ['', 'Số tiền bằng chữ: ' + result.inforExportExcel.textTotalMoney + './.', '', '', '', '', '', '', ''];
          let stringMoney = worksheet.addRow(dataStringMoney);
          worksheet.mergeCells(`B${stringMoney.number}:I${stringMoney.number}`);
          dataStringMoney.forEach((item, index) => {
            if (index != 0) {
              stringMoney.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
              stringMoney.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'right' };
            }
          });
          stringMoney.font = { name: 'Times New Roman', size: 13 };
          stringMoney.height = 25.71;

          worksheet.addRow([]);

          let rowFooterTable1 = [];
          rowFooterTable1[2] = 'Thời gian có hiệu lực:';
          rowFooterTable1[4] = this.effectiveDateControl.value?.toString() + ' ngày';
          rowFooterTable1[5] = 'Kể từ ngày:';
          rowFooterTable1[6] = intendedQuoteDate;
          let rowfooterTable1 = worksheet.addRow(rowFooterTable1);
          rowfooterTable1.font = { name: 'Times New Roman', size: 13 };
          rowfooterTable1.height = 20.71;
          worksheet.mergeCells(`B${rowfooterTable1.number}:C${rowfooterTable1.number}`);
          rowfooterTable1.alignment = { vertical: 'top', horizontal: 'left' };

          worksheet.addRow([]);

          result.listAdditionalInformation.forEach((item, index) => {
            let dataInfo1 = [];
            let title = index + 2 + '. ' + item.title
            dataInfo1[2] = convertStringTitle(title);
            let infor1 = worksheet.addRow(dataInfo1);
            infor1.font = { name: 'Times New Roman', size: 13, underline: 'single', bold: true };
            infor1.height = 33;
            worksheet.mergeCells(`B${infor1.number}:I${infor1.number}`);
            infor1.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
            if (item.content.trim().includes('table')) {
              let spilitArray = splitStringContent(item.content.trim());

              spilitArray.forEach(element => {
                if (element.includes('table')) {
                  let itemContentArray = convertStringContentToTable(element);
                  let tableHeader = itemContentArray[0].split("  ");
                  let tableContent = [];

                  itemContentArray.forEach((content, index) => {
                    if (index != 0) {
                      tableContent.push(content);
                    }
                  });

                  let columnNameArray = ['']
                  tableHeader.forEach((element, index) => {
                    if (element != '') {
                      columnNameArray.push(element);
                    }
                  });;

                  /* Header row */
                  let dataHeaderRow = columnNameArray;
                  let headerRow = worksheet.addRow(dataHeaderRow);
                  headerRow.font = { name: 'Times New Roman', size: 13, bold: true };
                  dataHeaderRow.forEach((item, index) => {
                    if (item != '' && index != 2) {
                      headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
                      headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
                      headerRow.getCell(index + 1).fill = {
                        type: 'pattern',
                        pattern: 'solid',
                        fgColor: { argb: '8DB4E2' }
                      };
                    } else if (item != '' && index == 2) {
                      headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
                      headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
                      headerRow.getCell(index + 1).fill = {
                        type: 'pattern',
                        pattern: 'solid',
                        fgColor: { argb: '8DB4E2' }
                      };
                    }
                  });
                  headerRow.height = 40;

                  /* table content row */
                  let data: Array<any> = [];

                  // Lấy dữ liệu trực tiếp từ data ko cần mapping
                  for (var index = 0; index < tableContent.length; ++index) {
                    let rowContent = tableContent[index].split("  ");
                    let row: Array<any> = [];
                    rowContent.forEach((item, index) => {
                      if (item != '' && item != ' ') {
                        row[index + 2] = item;
                      }
                    });
                    data.push(row);
                  }

                  data.forEach((el, index, array) => {
                    if (el != '') {
                      let row = worksheet.addRow(el);
                      row.font = { name: 'Times New Roman', size: 13 };
                      row.height = 38.71;

                      el.forEach((element, index) => {
                        if (element != '' && index != 3) {
                          row.getCell(index).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
                          row.getCell(index).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
                        }
                        else if (element != '' && index == 3) {
                          row.getCell(index).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
                          row.getCell(index).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
                        }
                      });
                    }
                  });

                }
                else {
                  let itemContentArray = element.split('\n');

                  itemContentArray.forEach((content, index) => {
                    let dataInfo2 = [];
                    dataInfo2[2] = content
                    if (index != 0) {
                      dataInfo2[2] = content
                    }
                    let infor2 = worksheet.addRow(dataInfo2);
                    if (item.title.toLowerCase().includes('ghi chú')) {
                      infor2.font = { name: 'Times New Roman', size: 13, italic: true };
                    } else {
                      infor2.font = { name: 'Times New Roman', size: 13 };
                    }
                    infor2.height = 20.71;
                    worksheet.mergeCells(`B${infor2.number}:I${infor2.number}`);
                    infor2.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
                  });
                }
              });
            }
            else {
              let itemContentArray = item.content.trim().split('\n');
              itemContentArray.forEach((element, index) => {
                let dataInfo2 = [];
                dataInfo2[2] = element;
                if (index != 0) {
                  dataInfo2[2] = element;
                }
                let infor2 = worksheet.addRow(dataInfo2);
                if (item.title.toLowerCase().includes('ghi chú')) {
                  infor2.font = { name: 'Times New Roman', size: 13, italic: true };
                } else {
                  infor2.font = { name: 'Times New Roman', size: 13 };
                }
                if (item.title.toLowerCase().includes('phạm vi')) {
                  worksheet.getCell(`B${infor2.number}`).value = { text: element, hyperlink: `${titleQuote}.xlsx - \'Phạm vi công viêc\'!A1` };
                }
                worksheet.mergeCells(`B${infor2.number}:I${infor2.number}`);
                infor2.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
              });
            }

            worksheet.addRow([]);
          });

          if (result.listQuotePlan.length > 0) {
            let indexNum = result.listAdditionalInformation.length + 1 + 1;

            let dataRow = [];
            dataRow[2] = romanize(indexNum) + '. Kế Hoạch Triển Khai (Schedule)';
            let row = worksheet.addRow(dataRow);
            row.font = { name: 'Times New Roman', size: 13, underline: 'single', bold: true };
            row.height = 33;
            worksheet.mergeCells(`B${row.number}:I${row.number}`);
            row.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

            worksheet.addRow([]);

            /* Header row */
            let dataHeaderRow2 = [];
            dataHeaderRow2[2] = 'STT';
            dataHeaderRow2[3] = 'Mốc hoàn thành';
            dataHeaderRow2[6] = 'Thời gian';
            dataHeaderRow2[8] = 'Tổng lũy kế';
            let headerRow2 = worksheet.addRow(dataHeaderRow2);
            headerRow2.font = { name: 'Times New Roman', size: 13, bold: true };
            worksheet.mergeCells(`C${headerRow2.number}:E${headerRow2.number}`);
            worksheet.mergeCells(`F${headerRow2.number}:G${headerRow2.number}`);
            worksheet.mergeCells(`H${headerRow2.number}:I${headerRow2.number}`);
            dataHeaderRow2.forEach((item, index) => {
              if (item != '') {
                headerRow2.getCell(index).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
                headerRow2.getCell(index).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

                headerRow2.getCell(index).fill = {
                  type: 'pattern',
                  pattern: 'solid',
                  fgColor: { argb: 'f2f2f2' }
                };
              }
            });
            headerRow2.height = 33.71;

            result.listQuotePlan.forEach((item, index) => {
              /* Data table */
              let data: Array<any> = [];

              // Lấy dữ liệu trực tiếp từ data ko cần mapping
              let row: Array<any> = [];
              row[2] = index + 1;
              row[3] = item.finished.toString().trim();
              row[6] = item.execTime.toString().trim();
              row[8] = item.sumExecTime.toString().trim();

              data.push(row);

              data.forEach((el, index, array) => {
                if (el != '') {
                  let row = worksheet.addRow(el);
                  row.font = { name: 'Times New Roman', size: 13 };
                  row.height = 37.46;
                  worksheet.mergeCells(`C${row.number}:E${row.number}`);
                  worksheet.mergeCells(`F${row.number}:G${row.number}`);
                  worksheet.mergeCells(`H${row.number}:I${row.number}`);

                  row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
                  row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };


                  row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
                  row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

                  row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
                  row.getCell(6).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

                  row.getCell(8).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
                  row.getCell(8).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
                }
              });
            });

            worksheet.addRow([]);
          }

          if (result.listQuotePaymentTerm.length > 0) {
            let indexNum = result.listQuotePaymentTerm.length + 2;

            let dataRow = [];
            dataRow[2] = romanize(indexNum) + '. Điều khoản thanh toán (Payment Term)';
            let row = worksheet.addRow(dataRow);
            row.font = { name: 'Times New Roman', size: 13, underline: 'single', bold: true };
            row.height = 33;
            worksheet.mergeCells(`B${row.number}:I${row.number}`);
            row.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

            let dataRow1 = [];
            dataRow1[2] = 'Các mốc thanh toán như sau: ';
            let row1 = worksheet.addRow(dataRow1);
            row1.font = { name: 'Times New Roman', size: 13 };
            row1.height = 33;
            worksheet.mergeCells(`B${row1.number}:I${row1.number}`);
            row1.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

            worksheet.addRow([]);

            /* Header row */
            let dataHeaderRow2 = [];
            dataHeaderRow2[2] = 'STT';
            dataHeaderRow2[3] = 'Mốc';
            dataHeaderRow2[6] = '%Thanh toán';
            let headerRow2 = worksheet.addRow(dataHeaderRow2);
            headerRow2.font = { name: 'Times New Roman', size: 13, bold: true };
            worksheet.mergeCells(`C${headerRow2.number}:E${headerRow2.number}`);
            worksheet.mergeCells(`F${headerRow2.number}:G${headerRow2.number}`);
            worksheet.mergeCells(`H${headerRow2.number}:I${headerRow2.number}`);
            dataHeaderRow2.forEach((item, index) => {
              if (item != '') {
                headerRow2.getCell(index).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
                headerRow2.getCell(index).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

                headerRow2.getCell(index).fill = {
                  type: 'pattern',
                  pattern: 'solid',
                  fgColor: { argb: 'f2f2f2' }
                };
              }
            });
            headerRow2.height = 33.71;

            result.listQuotePaymentTerm.forEach((item, index) => {
              /* Data table */
              let data: Array<any> = [];

              // Lấy dữ liệu trực tiếp từ data ko cần mapping
              let row: Array<any> = [];
              row[2] = item.orderNumber;
              row[3] = item.milestone.toString().trim();
              row[6] = item.paymentPercentage.toString().trim();

              data.push(row);

              data.forEach((el, index, array) => {
                if (el != '') {
                  let row = worksheet.addRow(el);
                  row.font = { name: 'Times New Roman', size: 13 };
                  row.height = 37.46;
                  worksheet.mergeCells(`C${row.number}:E${row.number}`);
                  worksheet.mergeCells(`F${row.number}:G${row.number}`);
                  worksheet.mergeCells(`H${row.number}:I${row.number}`);

                  row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
                  row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center' };


                  row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
                  row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

                  row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
                  row.getCell(6).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
                }
              });
            });

            worksheet.addRow([]);
          }

          let indexNum = result.listQuotePaymentTerm.length + 3;

          let dataRow = [];
          dataRow[2] = romanize(indexNum) + '. Phạm vi công việc';
          let row = worksheet.addRow(dataRow);
          row.font = { name: 'Times New Roman', size: 13, underline: 'single', bold: true };
          row.height = 33;
          worksheet.mergeCells(`B${row.number}:I${row.number}`);
          row.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

          let dataRow11 = [];
          dataRow11[2] = 'Các chức năng có tại sheet "Phạm vi công việc"';
          let row11 = worksheet.addRow(dataRow11);
          row11.font = { name: 'Times New Roman', size: 13 };
          worksheet.mergeCells(`B${row11.number}:I${row11.number}`);
          row11.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

          worksheet.addRow([]);

          let dataFooter1 = [];
          dataFooter1[6] = 'Trân trọng cảm ơn Quý khách hàng';
          let footer1 = worksheet.addRow(dataFooter1);
          footer1.font = { name: 'Times New Roman', size: 13 };
          worksheet.mergeCells(`F${footer1.number}:I${footer1.number}`);
          footer1.alignment = { vertical: 'middle', horizontal: 'center' };

          let dataFooter2 = [];
          let date = (new Date()).getDate();
          let month = (new Date()).getMonth() + 1;
          let fullYear = (new Date()).getFullYear();
          dataFooter2[6] = 'Hà Nội, Ngày ' + date + ' tháng ' + month + ' năm ' + fullYear;
          let footer2 = worksheet.addRow(dataFooter2);
          footer2.font = { name: 'Times New Roman', size: 13, bold: true };
          worksheet.mergeCells(`F${footer2.number}:I${footer2.number}`);
          footer2.alignment = { vertical: 'middle', horizontal: 'center' };

          let dataFooter3 = [];
          dataFooter3[6] = result.inforExportExcel.companyName.toUpperCase();
          let footer3 = worksheet.addRow(dataFooter3);
          footer3.font = { name: 'Times New Roman', size: 13, bold: true };
          worksheet.mergeCells(`F${footer3.number}:I${footer3.number}`);
          footer3.alignment = { vertical: 'bottom', horizontal: 'center', wrapText: true };
          footer3.height = 51;

          if (this.listQuoteScopes.length > 0) {
            // thêm sheet wbs
            let worksheet2 = workbook.addWorksheet('Phạm vi công việc', { views: [{ showGridLines: false }] });
            worksheet2.pageSetup.margins = {
              left: 0.25, right: 0.5,
              top: 0.25, bottom: 0.75,
              header: 0.3, footer: 0.05
            };
            worksheet2.pageSetup.paperSize = 9;  //A4 : 9

            this.buildExcelScope(worksheet2);
          }

          if (type === "EXCEL") {
            /*Export file excel*/
            this.exportToExel(workbook, titleQuote);
          }
          else {
            /*Export file excel*/
            this.exportToPdf(workbook, titleQuote);
          }
        }
      });
    }
  }

  logo1() {
    let logo = 'data:image/jpeg;base64,/9j/2wCEAAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQECAgICAgICAgICAgMDAwMDAwMDAwMBAQEBAQEBAgEBAgICAQICAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDA//dAAQA1//uAA5BZG9iZQBkwAAAAAH/wAARCAJcBrIDABEAAREBAhEB/8QBHwABAAMBAAIDAQEAAAAAAAAAAAkKCwgGBwEEBQMCAQEAAgICAwEAAAAAAAAAAAAACAkHCgUGAQIEAxAAAQMEAQECBgoJDwYICAcRAAMEBQECBgcICQoREhMUFSE5FhcxSViHiLjI8BkaQVFheJi31hgiV2lxgZGhp6ixwdHX6CMlKDKX2CRCUmJocnXhJjM4SHN0wvEnKTQ2N0NTgpKmNURHVFlkZ3eDk5aissMRAAEDAgMDAwYTDw0OBQQCAwABAgMEBQYHEQgSIQkTMRQiOEG18BUWGDI3RUdRVWGDhIWGlLO0xMUXGSM2QldxdXaBkZXS1NUkM1ZnoaOkpaax0+TlJUNSU1RiZXJzkpPB0eE0Y4KisiY1ZMIndPFEw//aAAwDAAABEQIRAD8Az/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Qz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Rz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Sz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Tz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Uz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Vz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Wz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Xz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Qz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Rz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Sz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Tz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA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AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Vz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Wz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Xz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//Qz/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA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AAAAAAAAAAAAAAAAAAAAAAAAAA//1M/8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA//1c/8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA//1s/8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA//18/8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA//0M/8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA//0c/8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA//0s/8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA//2Q=='
    return logo;
  }

  exportPdf(type: string) {
    if (this.appName == 'VNS') {
      this.quoteService.getDataExportExcelQuote(this.quoteId).subscribe(response => {
        let result: any = response;
        if (result.statusCode == 200) {
          let DateCHO = "";
          let status = this.listQuoteStatus.find(s => s.categoryId == result.quote.statusId);
          if (status.categoryCode == 'CHO') {
            DateCHO = this.datePipe.transform(result.quote.updatedDate, 'dd/MM/yyyy');
          }
          let dateNow = new Date();
          let imgBase64 = this.getBase64Logo();

          let customer: any = this.objectList.find(x => x.customerId == this.objCustomer.customerId);
          let cusName = customer.customerName;

          let totalNotVat = 0;  //Số tiền chưa VAT
          let totalVat = 0; // Tổng VÁT của tất cả sản phẩm

          // Lấy dữ liệu trực tiếp từ data ko cần mapping
          for (var index = 0; index < result.listQuoteDetail.length; ++index) {
            let totalNotVatTemp = result.listQuoteDetail[index].quantity * result.listQuoteDetail[index].unitPrice * result.listQuoteDetail[index].exchangeRate;

            // Thành tiền từng sản phẩm sau chiết khấu và VAT
            if (result.listQuoteDetail[index].discountType) {
              totalVat += (totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100)) * (result.listQuoteDetail[index].vat / 100);
              totalNotVat += totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100);
            } else if (!result.listQuoteDetail[index].discountType) {
              totalVat += (totalNotVatTemp - result.listQuoteDetail[index].discountValue) * (result.listQuoteDetail[index].vat / 100);
              totalNotVat += totalNotVatTemp - result.listQuoteDetail[index].discountValue;
            }
          }

          let totalNotDicount = totalNotVat + totalVat;
          /* Số tiền chết khấu*/
          let discountMoney = 0;
          if (result.quote.discountType) {
            discountMoney = totalNotDicount * result.quote.discountValue / 100;
          } else {
            discountMoney = result.quote.discountValue;
          }

          /* Số tiền phải thanh toán */
          let total = totalNotDicount - discountMoney;

          let documentDefinition: any = {
            pageSize: 'A4',
            pageMargins: [20, 20, 20, 20],
            content: [
              {
                table: {
                  widths: ['*', '400'],
                  body: [
                    [
                      {
                        stack: [
                          {
                            image: imgBase64, width: 100, height: 60
                          }
                        ],
                      },
                      {
                        stack: [
                          {
                            text: "" + result.inforExportExcel.companyName.toUpperCase(),
                            style: 'timer',
                            alignment: 'left'
                          },
                          {
                            text: '',
                            margin: [0, 2, 0, 2]
                          },
                          {
                            text: 'Địa chỉ: ' + result.inforExportExcel.address,
                            style: 'timer',
                            alignment: 'left'
                          },
                          {
                            text: '',
                            margin: [0, 2, 0, 2]
                          },
                          {
                            text: 'Điện thoại: ' + result.inforExportExcel.phone,
                            style: 'timer',
                            alignment: 'left'
                          },
                          {
                            text: '',
                            margin: [0, 2, 0, 2]
                          },
                          {
                            text: 'Email: ' + result.inforExportExcel.email,
                            style: 'timer',
                            alignment: 'left'
                          },
                          {
                            text: '',
                            margin: [0, 2, 0, 2]
                          },
                          {
                            text: 'Website dịch vụ: ' + result.inforExportExcel.website,
                            style: 'timer',
                            alignment: 'left'
                          },
                        ],
                      }
                    ],
                  ]
                },
                layout: {
                  defaultBorder: false
                },
                lineHeight: 0.75
              },
              {
                text: '',
                margin: [0, 10, 0, 10]
              },
              {
                text: 'BÁO GIÁ',
                style: 'header',
                alignment: 'center'
              },
              {
                text: 'Ngày ' + dateNow.getDate() + ' tháng ' + (dateNow.getMonth() + 1) + ' năm ' + dateNow.getFullYear(),
                style: 'timer',
                alignment: 'center'
              },
              {
                text: 'Số phiếu:  ' + this.quoteCode,
                style: 'timer',
                alignment: 'right',
                margin: [0, 10, 0, 10]
              },
              {
                text: 'Kính gửi: ' + cusName,
                style: 'timer',
                alignment: 'left',
                margin: [0, 2, 0, 2]
              },
              {
                text: 'Địa chỉ: ' + this.fullAddress,
                style: 'timer',
                alignment: 'left',
                margin: [0, 2, 0, 2]
              },
              {
                text: 'SĐT: ' + this.phone,
                style: 'timer',
                alignment: 'left',
                margin: [0, 2, 0, 2]
              },
              {
                text: 'Đơn vị tiền: VND',
                style: 'timer',
                alignment: 'right'
              },
              {
                style: 'table',
                table: {
                  widths: [20, 168, 49, 49, 49, 49, 45, 54],
                  headerRows: 1,
                  dontBreakRows: true,
                  body: [
                    [
                      { text: 'STT', style: 'tableHeader', alignment: 'center' },
                      { text: 'Tên sản phẩm/dịch vụ', style: 'tableHeader', alignment: 'center' },
                      { text: 'Đơn vị', style: 'tableHeader', alignment: 'center' },
                      { text: 'Số lượng', style: 'tableHeader', alignment: 'center' },
                      { text: 'Đơn giá', style: 'tableHeader', alignment: 'center' },
                      { text: 'Chiết khấu', style: 'tableHeader', alignment: 'center' },
                      { text: 'VAT(%)', style: 'tableHeader', alignment: 'center' },
                      { text: 'Thành tiền', style: 'tableHeader', alignment: 'center' },
                    ],
                  ]
                },
                layout: {
                  defaultBorder: true,
                  paddingTop: function (i, node) { return 2; },
                  paddingBottom: function (i, node) { return 2; }
                }
              },
              {
                text: '',
                margin: [0, -15.5, 0, -15.5]
              },
              {
                style: 'table',
                table: {
                  widths: [483, 54],
                  headerRows: 1,
                  dontBreakRows: true,
                  body: [
                    [
                      { text: 'Tổng số tiền chưa VAT:', style: 'tableHeader', alignment: 'right' },
                      { text: this.decimalPipe.transform(totalNotVat.toString()), style: { fontSize: 9, bold: true }, alignment: 'right' },
                    ],
                    [
                      { text: 'VAT:', style: 'tableHeader', alignment: 'right' },
                      { text: this.decimalPipe.transform(totalVat.toString()), style: { fontSize: 9, bold: true }, alignment: 'right' },
                    ],
                    [
                      { text: 'Chiết khấu theo báo giá:', style: 'tableHeader', alignment: 'right' },
                      { text: this.decimalPipe.transform(discountMoney.toString()), style: { fontSize: 9, bold: true }, alignment: 'right' },
                    ],
                    [
                      { text: 'Tổng số tiền phải thanh toán:', style: 'tableHeader', alignment: 'right' },
                      { text: this.decimalPipe.transform(total.toString()), style: { fontSize: 9, bold: true }, alignment: 'right' },
                    ],
                  ]
                },
                layout: {
                  defaultBorder: true,
                  paddingTop: function (i, node) { return 2; },
                  paddingBottom: function (i, node) { return 2; }
                }
              },
              {
                text: '',
                margin: [0, -15.5, 0, -15.5]
              },
              {
                style: 'table',
                table: {
                  widths: [546],
                  headerRows: 1,
                  dontBreakRows: true,
                  body: [
                    [
                      { text: "Số tiền bằng chữ: " + result.inforExportExcel.textTotalMoney, style: { fontSize: 10, bold: false, italic: true }, alignment: 'right' },
                    ],
                  ]
                },
                layout: {
                  defaultBorder: true,
                  paddingTop: function (i, node) { return 2; },
                  paddingBottom: function (i, node) { return 2; }
                }
              },
              {
                text: 'Thời gian có hiệu lực:  ' + this.effectiveDateControl.value.toString() + "    " + " Kể từ ngày: " + DateCHO,
                style: 'timer',
                alignment: 'left'
              },
              {
                style: 'table',
                table: {
                  widths: ["auto"],
                  headerRows: 1,
                  dontBreakRows: true,
                  body: [
                  ]
                },
                layout: {
                  defaultBorder: false,
                }
              },
              {
                columns: [
                  {
                    width: '55%',
                    text: '',
                    style: { fontSize: 10, bold: true },
                    alignment: 'right'
                  },
                  {
                    width: '45%',
                    text: 'Trân trọng cảm ơn Quý khách hàng',
                    style: { fontSize: 10, bold: false },
                    alignment: 'center'
                  }
                ]
              },
              {
                columns: [
                  {
                    width: '55%',
                    text: '',
                    style: { fontSize: 10, bold: true },
                    alignment: 'right'
                  },
                  {
                    width: '45%',
                    text: 'Hà Nội, Ngày ' + dateNow.getDate() + ' tháng ' + (dateNow.getMonth() + 1) + ' năm ' + dateNow.getFullYear(),
                    style: { fontSize: 10, bold: false },
                    alignment: 'center'
                  }
                ]
              },
              {
                columns: [
                  {
                    width: '55%',
                    text: '',
                    style: { fontSize: 10, bold: true },
                    alignment: 'right'
                  },
                  {
                    width: '45%',
                    text: result.inforExportExcel.companyName.toUpperCase(),
                    style: { fontSize: 10, bold: true },
                    alignment: 'center'
                  }
                ]
              },
            ],
            styles: {
              header: {
                fontSize: 18.5,
                bold: true
              },
              timer: {
                fontSize: 10,
                italics: true
              },
              table: {
                margin: [0, 15, 0, 15]
              },
              tableHeader: {
                fontSize: 10,
                bold: true
              },
              tableLine: {
                fontSize: 10,
              },
              tableLines: {
                fontSize: 9,
              },
              tableLiness: {
                fontSize: 7,
              },
              StyleItalics: {
                italics: true
              }
            }
          };

          result.listQuoteDetail.forEach((item, index) => {
            let col7 = "";
            let totalNotVatTemp = result.listQuoteDetail[index].quantity * result.listQuoteDetail[index].unitPrice * result.listQuoteDetail[index].exchangeRate;
            if (result.listQuoteDetail[index].discountType) {
              col7 = this.decimalPipe.transform(((totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100)) +
                (totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100)) * (result.listQuoteDetail[index].vat / 100)).toString());
            } else if (!result.listQuoteDetail[index].discountType) {
              // Thành tiền từng sản phẩm
              col7 = this.decimalPipe.transform(((totalNotVatTemp - result.listQuoteDetail[index].discountValue)) +
                ((totalNotVatTemp - result.listQuoteDetail[index].discountValue) * (result.listQuoteDetail[index].vat / 100))).toString();
            }
            let option = [
              { text: index + 1, style: 'tableLines', alignment: 'center' },
              {
                text: (item.nameGene == null || item.nameGene == '') ? item.description : item.nameGene,
                style: 'tableLines',
                alignment: 'left'
              },
              {
                text: (item.discountType == 0) ? item.nameProductUnit : item.nameProductUnit,
                style: 'tableLines',
                alignment: 'center'
              },
              {
                text: this.decimalPipe.transform(item.quantity).toString(),
                style: 'tableLines',
                alignment: 'right'
              },
              {
                text: this.decimalPipe.transform((item.unitPrice).toString()),
                style: 'tableLines',
                alignment: 'right'
              },
              {
                text: this.decimalPipe.transform(item.discountValue).toString(),
                style: 'tableLines',
                alignment: 'right'
              },
              {
                text: this.decimalPipe.transform(item.vat).toString(),
                style: 'tableLines',
                alignment: 'right'
              },
              { text: col7, style: 'tableLines', alignment: 'right' },
            ];
            documentDefinition.content[9].table.body.push(option);
          });

          if (result.listAdditionalInformation.length > 0) {
            result.listAdditionalInformation.forEach((item, index) => {
              let option1 =
                [
                  {
                    text: item.title,
                    style: { fontSize: 10, bold: true },
                    alignment: 'left'
                  }
                ]
              let option2 =
                [
                  {
                    text: item.content,
                    style: 'timer',
                    alignment: 'left'
                  }
                ]
              documentDefinition.content[15].table.body.push(option1);
              documentDefinition.content[15].table.body.push(option2);
            });
          } else {
            documentDefinition.content.splice(15, 1);
          }

          let cusNameExport = convertStringCusName(cusName);
          let title = cusNameExport + ' báo giá ' + dateNow.getDate() + '_' + (dateNow.getMonth() + 1) + '_' + dateNow.getUTCFullYear();

          if (type === 'download') {
            pdfMake.createPdf(documentDefinition).download(title + '.pdf');
          }
          else {
            pdfMake.createPdf(documentDefinition).getBase64(async function (encodedString) {
              localStorage.setItem('base64PDFQuote', encodedString);
            });
          }
        }
      });
    }
    else {
      this.quoteService.getDataExportExcelQuote(this.quoteId).subscribe(response => {
        let result: any = response;
        if (result.statusCode == 200) {
          let quoteName = result.quote.quoteName;
          let dateSend = this.sendDateControl.value ? convertToUTCTime(this.sendDateControl.value) : null;

          let intendedQuoteDate = this.datePipe.transform(result.quote.intendedQuoteDate, 'dd/MM/yyyy');

          let dateNow = new Date();
          let imgBase64 = this.getBase64Logo();

          let customer: any = this.objectList.find(x => x.customerId == this.objCustomer.customerId);
          let cusName = customer.customerName;

          let totalNotVat = 0;  //Số tiền chưa VAT
          let totalVat = 0; // Tổng VÁT của tất cả sản phẩm

          // Lấy dữ liệu trực tiếp từ data ko cần mapping
          for (var index = 0; index < result.listQuoteDetail.length; ++index) {
            let totalNotVatTemp = result.listQuoteDetail[index].quantity * result.listQuoteDetail[index].unitPrice * result.listQuoteDetail[index].exchangeRate + result.listQuoteDetail[index].unitLaborPrice * result.listQuoteDetail[index].unitLaborNumber * result.listQuoteDetail[index].exchangeRate;

            // Thành tiền từng sản phẩm sau chiết khấu và VAT
            if (result.listQuoteDetail[index].discountType) {
              totalVat += (totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100)) * (result.listQuoteDetail[index].vat / 100);
              totalNotVat += totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100);
            } else if (!result.listQuoteDetail[index].discountType) {
              totalVat += (totalNotVatTemp - result.listQuoteDetail[index].discountValue) * (result.listQuoteDetail[index].vat / 100);
              totalNotVat += totalNotVatTemp - result.listQuoteDetail[index].discountValue;
            }
          }

          let totalCost = 0;
          for (var index = 0; index < result.listQuoteCostDetail.length; ++index) {
            if (!result.listQuoteCostDetail[index].isInclude) {
              totalCost += (result.listQuoteCostDetail[index].quantity * result.listQuoteCostDetail[index].unitPrice);
            }
          }

          let totalNotDicount = totalNotVat + totalVat + totalCost;
          /* Số tiền chết khấu*/
          let discountMoney = 0;
          if (result.quote.discountType) {
            discountMoney = totalNotDicount * result.quote.discountValue / 100;
          } else {
            discountMoney = result.quote.discountValue;
          }

          /* Số tiền phải thanh toán */
          let total = totalNotDicount - discountMoney;
          let indexNum = result.listAdditionalInformation.length + 1 + 1;

          let breakline =
          {
            text: '',
            margin: [0, 10, 0, 10]
          };

          let documentDefinition: any = {
            pageSize: 'A4',
            pageMargins: [30, 20, 20, 30],
            content: [
              {
                table: {
                  widths: ['*', '400'],
                  body: [
                    [
                      {
                        stack: [
                          {
                            image: imgBase64, width: 143, height: 66
                          }
                        ],
                      },
                      {
                        stack: [
                          {
                            text: "" + result.inforExportExcel.companyName.toUpperCase(),
                            style: 'timer',
                            alignment: 'left'
                          },
                          {
                            text: '',
                            margin: [0, 2, 0, 2]
                          },
                          {
                            text: 'Địa chỉ: ' + result.inforExportExcel.address,
                            style: 'timer',
                            alignment: 'left'
                          },
                          {
                            text: '',
                            margin: [0, 2, 0, 2]
                          },
                          {
                            text: 'Điện thoại: ' + result.inforExportExcel.phone,
                            style: 'timer',
                            alignment: 'left'
                          },
                          {
                            text: '',
                            margin: [0, 2, 0, 2]
                          },
                          {
                            text: 'Email: ' + result.inforExportExcel.email,
                            style: 'timer',
                            alignment: 'left'
                          },
                          {
                            text: '',
                            margin: [0, 2, 0, 2]
                          },
                          {
                            text: 'Website dịch vụ: ' + result.inforExportExcel.website,
                            style: 'timer',
                            alignment: 'left'
                          },
                        ],
                      }
                    ],
                  ]
                },
                layout: {
                  defaultBorder: false
                },
                lineHeight: 0.75
              },
              {
                text: '',
                margin: [0, 10, 0, 10]
              },
              {
                text: 'BÁO GIÁ' + " " + quoteName.toUpperCase(),
                style: 'header',
                alignment: 'center'
              },
              {
                text: 'Ngày ' + dateNow.getDate() + ' tháng ' + (dateNow.getMonth() + 1) + ' năm ' + dateNow.getFullYear(),
                style: 'timer',
                alignment: 'center'
              },
              {
                text: 'Số phiếu:  ' + this.quoteCode,
                style: 'timer',
                alignment: 'right',
                margin: [0, 10, 0, 10]
              },
              {
                text: 'Kính gửi: ' + cusName,
                style: 'timer',
                alignment: 'left',
                margin: [0, 2, 0, 2]
              },
              {
                text: 'Địa chỉ: ' + this.fullAddress,
                style: 'timer',
                alignment: 'left',
                margin: [0, 2, 0, 2]
              },
              {
                text: 'SĐT: ' + this.phone,
                style: 'timer',
                alignment: 'left',
                margin: [0, 2, 0, 2]
              },
              {
                text: '',
                margin: [0, 2, 0, 2]
              },
              {
                text: result.inforExportExcel.companyName + ' gửi đến quý công ty báo giá ' + quoteName + ' với các nội dung sau:',
                style: 'timer',
                alignment: 'left',
                margin: [0, 2, 0, 2]
              },
              {
                text: 'Đơn vị tiền: VND',
                style: 'timer',
                alignment: 'right'
              },
              {
                text: 'I. Danh sách sản phẩm',
                style: 'headers',
                alignment: 'Left',
              },
              {
                style: 'table',
                table: {
                  widths: [25, 100, 40, 45, 85, 45, 45, 85],
                  headerRows: 1,
                  dontBreakRows: true,
                  body: [
                    [
                      { text: 'STT', style: 'tableHeader', alignment: 'center' },
                      { text: 'Tên sản phẩm/dịch vụ', style: 'tableHeader', alignment: 'center' },
                      { text: 'Đơn vị', style: 'tableHeader', alignment: 'center' },
                      { text: 'Số lượng', style: 'tableHeader', alignment: 'center' },
                      { text: 'Đơn giá', style: 'tableHeader', alignment: 'center' },
                      { text: 'Chiết khấu', style: 'tableHeader', alignment: 'center' },
                      { text: 'VAT(%)', style: 'tableHeader', alignment: 'center' },
                      { text: 'Thành tiền', style: 'tableHeader', alignment: 'center' },
                    ],
                  ]
                },
                layout: {
                  defaultBorder: true,
                  paddingTop: function (i, node) { return 2; },
                  paddingBottom: function (i, node) { return 2; }
                }
              },
              {
                text: '',
                margin: [0, -15.5, 0, -15.5]
              },
              {
                style: 'table',
                table: {
                  widths: [439, 85],
                  headerRows: 1,
                  dontBreakRows: true,
                  body: [
                    [
                      { text: 'Tổng số tiền chưa VAT:', style: 'tableHeader', alignment: 'right' },
                      { text: this.decimalPipe.transform(totalNotVat.toString()), style: { fontSize: 13, bold: true }, alignment: 'right' },
                    ],
                    [
                      { text: 'VAT:', style: 'tableHeader', alignment: 'right' },
                      { text: this.decimalPipe.transform(totalVat.toString()), style: { fontSize: 13, bold: true }, alignment: 'right' },
                    ],
                    [
                      { text: 'Chiết khấu theo báo giá:', style: 'tableHeader', alignment: 'right' },
                      { text: this.decimalPipe.transform(discountMoney.toString()), style: { fontSize: 13, bold: true }, alignment: 'right' },
                    ],
                    [
                      { text: 'Tổng số tiền phải thanh toán:', style: 'tableHeader', alignment: 'right' },
                      { text: this.decimalPipe.transform(total.toString()), style: { fontSize: 13, bold: true }, alignment: 'right' },
                    ],
                  ]
                },
                layout: {
                  defaultBorder: true,
                  paddingTop: function (i, node) { return 2; },
                  paddingBottom: function (i, node) { return 2; }
                }
              },
              {
                text: '',
                margin: [0, -15.5, 0, -15.5]
              },
              {
                style: 'table',
                table: {
                  widths: [533],
                  headerRows: 1,
                  dontBreakRows: true,
                  body: [
                    [
                      { text: "Số tiền bằng chữ: " + result.inforExportExcel.textTotalMoney, style: { fontSize: 13, bold: false, italic: true }, alignment: 'right' },
                    ],
                  ]
                },
                layout: {
                  defaultBorder: true,
                  paddingTop: function (i, node) { return 2; },
                  paddingBottom: function (i, node) { return 2; }
                }
              },
              {
                text: 'Thời gian có hiệu lực:  ' + this.effectiveDateControl.value.toString() + " ngày" + "    " + " Kể từ ngày: " + intendedQuoteDate,
                style: 'timer',
                alignment: 'left'
              },
            ],
            styles: {
              header: {
                fontSize: 14,
                bold: true
              },
              timer: {
                fontSize: 13
              },
              table: {
                margin: [0, 15, 0, 15]
              },
              tableHeader: {
                fontSize: 13,
                bold: true
              },
              tableLine: {
                fontSize: 13,
              },
              tableLines: {
                fontSize: 13,
              },
              tableLiness: {
                fontSize: 13,
              },
              StyleItalics: {
                italics: true
              },
              headers: {
                fontSize: 13,
                bold: true,
                decoration: 'underline'
              }
            }
          };

          result.listQuoteDetail.forEach((item, index) => {
            let option = [
              { text: index + 1, style: 'tableLines', alignment: 'center' },
              {
                text: (item.nameProduct == null || item.nameProduct == '') ? item.description : item.nameProduct,
                style: 'tableLines',
                alignment: 'left'
              },
              {
                text: (item.discountType == 0) ? item.nameProductUnit : item.nameProductUnit,
                style: 'tableLines',
                alignment: 'center'
              },
              {
                text: this.decimalPipe.transform(item.quantity).toString(),
                style: 'tableLines',
                alignment: 'right'
              },
              {
                text: this.decimalPipe.transform((item.unitPrice).toString()),
                style: 'tableLines',
                alignment: 'right'
              },
              {
                text: this.decimalPipe.transform(item.discountValue).toString(),
                style: 'tableLines',
                alignment: 'right'
              },
              {
                text: this.decimalPipe.transform(item.vat).toString(),
                style: 'tableLines',
                alignment: 'right'
              },
              {
                text: this.decimalPipe.transform(item.sumAmount).toString(),
                style: 'tableLines',
                alignment: 'right'
              },
            ];
            documentDefinition.content[12].table.body.push(option);
          });

          // thong tin bo xung
          if (result.listAdditionalInformation.length > 0) {

            result.listAdditionalInformation.forEach((item, index) => {
              let title = index + 2 + '. ' + item.title
              let option1 =
              {
                text: convertStringTitle(title),
                style: 'headers',
                alignment: 'left',
                margin: [0, 10, 0, 10],
              }

              documentDefinition.content.push(option1);

              let itemContentArray = convertStringContent(item.content.trim());
              itemContentArray.forEach(element => {
                let option2 =
                {
                  text: element,
                  style: 'timer',
                  alignment: 'left'
                }

                documentDefinition.content.push(option2);
              });
              documentDefinition.content.push(breakline);
            });
          }

          // ke hoach trien khai
          if (result.listQuotePlan.length > 0) {

            let scheduleHearder =
            {
              text: romanize(indexNum) + '. Kế Hoạch Triển Khai (Schedule)',
              style: 'headers',
              alignment: 'left',
              margin: [0, 10, 0, 10],
            };
            documentDefinition.content.push(scheduleHearder);
            indexNum++;

            let scheduleText =
            {
              text: 'Dự án được thực hiện theo kế hoạch thời gian như sau.',
              style: 'timer',
              alignment: 'left',
              margin: [0, 2, 0, 2]
            };
            documentDefinition.content.push(scheduleText);

            let scheduleTable =
            {
              style: 'table',
              table: {
                widths: ['auto', 'auto', 'auto', 'auto'],
                headerRows: 1,
                dontBreakRows: true,
                body: [
                  [
                    { text: 'STT', style: 'tableHeader', alignment: 'center' },
                    { text: 'Mốc hoàn thành', style: 'tableHeader', alignment: 'left' },
                    { text: 'Thời gian thực hiện', style: 'tableHeader', alignment: 'center' },
                    { text: 'Tổng thời gian thực hiện', style: 'tableHeader', alignment: 'center' },
                  ],
                ]
              },
              layout: {
                defaultBorder: true,
                paddingTop: function (i, node) { return 2; },
                paddingBottom: function (i, node) { return 2; }
              }
            };
            documentDefinition.content.push(scheduleTable);
            documentDefinition.content.push(breakline);

            result.listQuotePlan.forEach((item, index) => {
              let tableContent =
                [
                  { text: index + 1, style: 'tableLines', alignment: 'center' },
                  {
                    text: (item.finished == null || item.finished == '') ? item.finished : item.finished,
                    style: 'tableLines',
                    alignment: 'left'
                  },
                  {
                    text: (item.execTime == null) ? item.execTime : item.execTime,
                    style: 'tableLines',
                    alignment: 'center'
                  },
                  {
                    text: (item.sumExecTime == null) ? item.sumExecTime : item.sumExecTime,
                    style: 'tableLines',
                    alignment: 'center'
                  },
                ]
              documentDefinition.content[documentDefinition.content.length - 1 - 1].table.body.push(tableContent);
            });
          }

          // dieu khoan thanh toan
          if (result.listQuotePaymentTerm.length > 0) {

            let paymentTermHeard =
            {
              text: romanize(indexNum) + '. Điều khoản thanh toán (Payment Term)',
              style: 'headers',
              alignment: 'left',
              margin: [0, 10, 0, 10],
            }
            documentDefinition.content.push(paymentTermHeard);
            indexNum++;


            let paymentTermText =
            {
              text: 'Các mốc thanh toán như sau: ',
              style: 'timer',
              alignment: 'left',
              margin: [0, 2, 0, 2]
            };
            documentDefinition.content.push(paymentTermText);

            let paymentTermTable =
            {
              style: 'table',
              table: {
                widths: ['auto', 'auto', 'auto'],
                headerRows: 1,
                dontBreakRows: true,
                body: [
                  [
                    { text: 'STT', style: 'tableHeader', alignment: 'center' },
                    { text: 'Mốc', style: 'tableHeader', alignment: 'center' },
                    { text: '% Thanh toán', style: 'tableHeader', alignment: 'center' },
                  ],
                ]
              },
              layout: {
                defaultBorder: true,
                paddingTop: function (i, node) { return 2; },
                paddingBottom: function (i, node) { return 2; }
              }
            }
            documentDefinition.content.push(paymentTermTable);

            result.listQuotePaymentTerm.forEach(item => {
              let tableContent =
                [
                  { text: item.orderNumber, style: 'tableLines', alignment: 'center' },
                  {
                    text: item.milestone,
                    style: 'tableLines',
                    alignment: 'left'
                  },
                  {
                    text: item.paymentPercentage,
                    style: 'tableLines',
                    alignment: 'center'
                  },
                ]
              documentDefinition.content[documentDefinition.content.length - 1].table.body.push(tableContent);
            });
          }

          // pham vi cong viec
          if (this.listQuoteScopes.length > 1) {

            let quoteScopeHeader =
            {
              text: romanize(indexNum) + '. Phạm vi công việc',
              style: 'headers',
              alignment: 'left',
              margin: [0, 10, 0, 10],
            };
            documentDefinition.content.push(quoteScopeHeader);
            indexNum++;

            let quoteScopeText =
            {
              text: 'Các chức năng có tại phụ lục số 01.',
              style: 'timer',
              alignment: 'left',
              margin: [0, 2, 0, 2]
            }
            documentDefinition.content.push(quoteScopeText);
          }

          let footer = [
            {
              text: '',
              margin: [0, 2, 0, 2]
            },
            {
              columns: [
                {
                  width: '55%',
                  text: '',
                  style: { fontSize: 13, bold: true },
                  alignment: 'right'
                },
                {
                  width: '45%',
                  text: 'Trân trọng cảm ơn Quý khách hàng',
                  style: { fontSize: 13, bold: false },
                  alignment: 'center'
                }
              ]
            },
            {
              columns: [
                {
                  width: '55%',
                  text: '',
                  style: { fontSize: 13, bold: true },
                  alignment: 'right'
                },
                {
                  width: '45%',
                  text: 'Hà Nội, Ngày ' + dateNow.getDate() + ' tháng ' + (dateNow.getMonth() + 1) + ' năm ' + dateNow.getFullYear(),
                  style: { fontSize: 13, bold: false },
                  alignment: 'center'
                }
              ]
            },
            {
              columns: [
                {
                  width: '55%',
                  text: '',
                  style: { fontSize: 13, bold: true },
                  alignment: 'right'
                },
                {
                  width: '45%',
                  text: result.inforExportExcel.companyName.toUpperCase(),
                  style: { fontSize: 13, bold: true },
                  alignment: 'center'
                }
              ]
            },
          ]
          documentDefinition.content.push(footer);

          // pham vi cong viec bang - phu luc I
          if (this.listQuoteScopes.length > 1) {

            let newPageHeader =
            {
              text: 'PHỤ LỤC SỐ 01: PHẠM VI CÔNG VIỆC',
              pageBreak: 'before',
              style: 'header',
              alignment: 'center',
              margin: [0, 15, 0, 10],
              pageMargins: [30, 20, 20, 30],
            }
            documentDefinition.content.push(newPageHeader);

            let newPageDescription =
            {
              text: 'Danh sách phạm vi công việc ' + this.listQuoteScopes.find(x => x.tt == '')?.category,
              style: 'timer',
              alignment: 'center',
              margin: [0, 10, 0, 10],
            }
            documentDefinition.content.push(newPageDescription);
            documentDefinition.content.push(breakline);

            let newPageTable =
            {
              style: 'table',
              table: {
                widths: ['auto', 170, 300],
                headerRows: 1,
                dontBreakRows: true,
                body: [
                  [
                    { text: 'STT', style: 'tableHeader', alignment: 'left' },
                    { text: 'Hạng mục', style: 'tableHeader', alignment: 'left' },
                    { text: 'Mô tả', style: 'tableHeader', alignment: 'center' },
                  ],
                ]
              },
              layout: {
                defaultBorder: true,
                paddingTop: function (i, node) { return 2; },
                paddingBottom: function (i, node) { return 2; }
              }
            }
            documentDefinition.content.push(newPageTable);

            this.listQuoteScopes.forEach(item => {
              if (item.tt != '' || item.level != 0) {
                if (item.level == 1) {
                  let quoteScope = [
                    { text: item.tt, style: 'tableLines', alignment: 'left' },
                    {
                      text: (item.category == null || item.category == '') ? item.category : item.category,
                      style: { fontSize: 13, bold: true },
                      alignment: 'left',
                    },
                    {
                      text: (item.description == null) ? item.description : item.description,
                      style: 'tableLines',
                      alignment: 'left'
                    },
                  ]

                  documentDefinition.content[documentDefinition.content.length - 1].table.body.push(quoteScope);
                } else {
                  let quoteScope = [
                    { text: item.tt, style: 'tableLines', alignment: 'left' },
                    {
                      text: (item.category == null || item.category == '') ? item.category : item.category,
                      style: 'tableLines',
                      alignment: 'left'
                    },
                    {
                      text: (item.description == null) ? item.description : item.description,
                      style: 'tableLines',
                      alignment: 'left'
                    },
                  ]

                  documentDefinition.content[documentDefinition.content.length - 1].table.body.push(quoteScope);
                }
              }
            });
          }


          let cusNameExport = convertStringCusName(quoteName);
          let title = cusNameExport;
          if (type === 'download') {
            pdfMake.createPdf(documentDefinition).download(title + '.pdf');
          }
          else {
            pdfMake.createPdf(documentDefinition).getBase64(async function (encodedString) {
              localStorage.setItem('base64PDFQuote', encodedString);
            });
          }
        }
      });
    }
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  exportToPdf(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/pdf' });
      saveAs.saveAs(blob, fileName);
    })
  }

  converToArrayQuoteDocument(fileList: File[]) {
    for (var x = 0; x < fileList.length; ++x) {
      let quoteDocument = new QuoteDocument();
      quoteDocument.documentName = fileList[x].name;
      quoteDocument.documentSize = fileList[x].size + '';
      quoteDocument.createdDate = convertToUTCTime(new Date());
      quoteDocument.updatedDate = convertToUTCTime(new Date());

      this.arrayQuoteDocumentModel = [...this.arrayQuoteDocumentModel, quoteDocument];
    }
  }

  // Event thay đổi nội dung ghi chú
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  cancelNote() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn hủy ghi chú này?',
      accept: () => {
        this.noteId = null;
        this.noteContent = null;
        this.uploadedNoteFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();
        }
        this.listUpdateNoteDocument = [];
        this.isEditNote = false;
      }
    });
  }

  /*Lưu file và ghi chú vào Db*/
  async saveNote() {
    this.loading = true;
    this.listNoteDocumentModel = [];

    /*Upload file mới nếu có*/
    if (this.uploadedNoteFiles.length > 0) {
      let listFileNameExists: Array<FileNameExists> = [];
      let result: any = await this.imageService.uploadFileForOptionAsync(this.uploadedNoteFiles, 'Quote');

      listFileNameExists = result.listFileNameExists;

      for (var x = 0; x < this.uploadedNoteFiles.length; ++x) {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = this.uploadedNoteFiles[x].name;
        let fileExists = listFileNameExists.find(f => f.oldFileName == this.uploadedNoteFiles[x].name);
        if (fileExists) {
          noteDocument.DocumentName = fileExists.newFileName;
        }
        noteDocument.DocumentSize = this.uploadedNoteFiles[x].size.toString();
        this.listNoteDocumentModel.push(noteDocument);
      }
    }
    let noteModel = new NoteModel();
    if (!this.noteId) {
      /*Tạo mới ghi chú*/
      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.quoteId;
      noteModel.ObjectType = 'QUOTE';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/
      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.quoteId;
      noteModel.ObjectType = 'QUOTE';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    }
    if (noteModel.Description == "" && this.listNoteDocumentModel.length == 0) {

      this.loading = false;
      return;
    }
    this.noteHistory = [];
    this.noteService.createNoteForQuoteDetail(noteModel, this.listNoteDocumentModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedNoteFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.noteContent = null;
        this.listUpdateNoteDocument = [];
        this.noteId = null;
        this.isEditNote = false;

        /*Reshow Time Line*/
        this.noteHistory = result.listNote;
        this.handleNoteContent();
        let messageCode = "Thêm ghi chú thành công";
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: messageCode };
        this.showMessage(mgs);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Xử lý và hiển thị lại nội dung ghi chú*/
  handleNoteContent() {
    this.noteHistory.forEach(element => {
      setTimeout(() => {
        let count = 0;
        if (element.description == null) {
          element.description = "";
        }

        let des = $.parseHTML(element.description);
        let newTextContent = '';
        for (let i = 0; i < des.length; i++) {
          count += des[i].textContent.length;
          newTextContent += des[i].textContent;
        }

        if (count > 250) {
          newTextContent = newTextContent.substr(0, 250) + '<b>...</b>';
          $('#' + element.noteId).find('.short-content').append($.parseHTML(newTextContent));
        } else {
          $('#' + element.noteId).find('.short-content').append($.parseHTML(element.description));
        }

        $('#' + element.noteId).find('.full-content').append($.parseHTML(element.description));
      }, 1000);
    });
  }
  /*End*/

  /*Event Mở rộng/Thu gọn nội dung của ghi chú*/
  toggle_note_label: string = 'Mở rộng';
  trigger_node(nodeid: string, event) {
    // noteContent
    let shortcontent_ = $('#' + nodeid).find('.short-content');
    let fullcontent_ = $('#' + nodeid).find('.full-content');
    if (shortcontent_.css("display") === "none") {
      fullcontent_.css("display", "none");
      shortcontent_.css("display", "block");
    } else {
      fullcontent_.css("display", "block");
      shortcontent_.css("display", "none");
    }
    // noteFile
    let shortcontent_file = $('#' + nodeid).find('.short-content-file');
    let fullcontent_file = $('#' + nodeid).find('.full-content-file');
    let continue_ = $('#' + nodeid).find('.continue')
    if (shortcontent_file.css("display") === "none") {
      continue_.css("display", "block");
      fullcontent_file.css("display", "none");
      shortcontent_file.css("display", "block");
    } else {
      continue_.css("display", "none");
      fullcontent_file.css("display", "block");
      shortcontent_file.css("display", "none");
    }
    let curr = $(event.target);

    if (curr.attr('class').indexOf('pi-chevron-right') != -1) {
      this.toggle_note_label = 'Thu gọn';
      curr.removeClass('pi-chevron-right');
      curr.addClass('pi-chevron-down');
    } else {
      this.toggle_note_label = 'Mở rộng';
      curr.removeClass('pi-chevron-down');
      curr.addClass('pi-chevron-right');
    }
  }
  /*End */

  /*Kiểm tra noteText > 250 ký tự hoặc noteDocument > 3 thì ẩn đi một phần nội dung*/
  tooLong(note): boolean {
    if (note.noteDocList.length > 3) return true;
    var des = $.parseHTML(note.description);
    var count = 0;
    for (var i = 0; i < des.length; i++) {
      count += des[i].textContent.length;
      if (count > 250) return true;
    }
    return false;
  }

  openItem(name, url) {
    this.imageService.downloadFile(name, url).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(file);
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
    }, error => { });
  }

  /*Event Sửa ghi chú*/
  onClickEditNote(noteId: string, noteDes: string) {
    this.noteContent = noteDes;
    this.noteId = noteId;
    this.listUpdateNoteDocument = this.noteHistory.find(x => x.noteId == this.noteId).noteDocList;
    this.isEditNote = true;
  }
  /*End*/

  /*Event Xóa ghi chú*/
  onClickDeleteNote(noteId: string) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa ghi chú này?',
      accept: () => {
        this.loading = true;
        this.noteService.disableNote(noteId).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            let note = this.noteHistory.find(x => x.noteId == noteId);
            let index = this.noteHistory.lastIndexOf(note);
            this.noteHistory.splice(index, 1);

            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa ghi chú thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }
  /*End*/

  /* Event thêm file dược chọn vào list file note */
  handleNoteFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= 10000000) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedNoteFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedNoteFiles.push(file);
          }
        }
      }
    }
  }

  /*Event khi click xóa từng file */
  removeNoteFile(event) {
    let index = this.uploadedNoteFiles.indexOf(event.file);
    this.uploadedNoteFiles.splice(index, 1);
  }

  /*Event khi click xóa toàn bộ file */
  clearAllNoteFile() {
    this.uploadedNoteFiles = [];
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  ngOnDestroy() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
    }
  }

  addPlan() {
    let item = new QuotePlanModel();
    item.tt = this.listQuotePlans.length + 1;
    item.quoteId = this.quoteId;
    this.listQuotePlans = [...this.listQuotePlans, item];
  }

  deletePlan(rowData: QuotePlanModel) {
    this.listQuotePlans = this.listQuotePlans.filter(e => e != rowData);
    var index = 1;
    this.listQuotePlans.forEach(item => { item.tt = index; index++; })
  }

  awaitResponse: boolean = false;
  displayModal: boolean = false;
  isEditScope: boolean = false;
  textHeaderDialog: string = 'Thêm mới hạng mục';
  /* Mở popup Thêm mới phạm vi công việc */
  addScope() {

    if (this.selectedNode || this.listQuoteScopes.length == 0) {
      this.awaitResponse = false;
      this.displayModal = true;
      this.textHeaderDialog = 'Thêm mới hạng mục';
    } else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn chưa chọn vị trí hạng mục' };
      this.showMessage(msg);
    }
  }

  detailScope() {
    if (this.selectedNode) {
      this.awaitResponse = false;
      this.displayModal = true;
      this.isEditScope = true;
      this.textHeaderDialog = 'Chi tiết hạng mục';
      this.scopeCategoryControl.setValue(this.selectedNode.data.category);
      this.scopeDescriptionControl.setValue(this.selectedNode.data.description);
    } else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn chưa chọn vị trí hạng mục' };
      this.showMessage(msg);
    }
  }

  closePopup() {
    this.displayModal = false;
    this.selectedNode = null;
    this.actionDeleteScope = false;
    this.actionEditScope = false;
    this.isEditScope = false;
    this.scopeCreateForm.reset();
  }

  list_to_tree() {
    let list: Array<TreeNode> = [];
    if (this.listQuoteScopes.length > 0) {
      this.listQuoteScopes.forEach(item => {
        let node: TreeNode = {
          label: item.category,
          expanded: true,
          data: {
            'scopeId': item.scopeId,
            'parentId': item.parentId,
            'category': item.category,
            'description': item.description,
            'quoteId': item.quoteId,
            'level': item.level
          },
          children: []
        };

        list = [...list, node];
      });
    }

    var map = {}, node, roots = [], i;

    for (i = 0; i < list.length; i += 1) {
      map[list[i].data.scopeId] = i; // initialize the map
      list[i].children = []; // initialize the children
    }

    for (i = 0; i < list.length; i += 1) {
      node = list[i];
      if (node.data.level !== 0 && node.data.level !== null) {
        // if you have dangling branches check that map[node.parentId] exists
        list[map[node.data.parentId]]?.children.push(node);
      } else {
        roots.push(node);
      }
    }
    return roots;
  }

  onNodeSelect(event) {
    this.actionEditScope = true;
    this.actionDeleteScope = event.node.children.length == 0;
  }

  onNodeUnselect(event) {

    this.selectedNode = null;
    this.actionDeleteScope = false;
    this.actionEditScope = false;
    this.scopeCreateForm.reset();
  }

  deleteScope() {

    this.quoteService.deleteScope(this.selectedNode.data.scopeId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa thành công' };
        this.showMessage(msg);

        //this.getAllOrganization();
        this.listQuoteScopes = result.listQuoteScopes || [];
        /* Convert data to type TreeNode */
        this.data = this.list_to_tree();
        this.actionDeleteScope = false;
        this.actionEditScope = false;
        this.selectedNode = null;
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  createScope() {

    let level = (this.selectedNode === undefined || this.selectedNode === null) ? 0 : this.selectedNode.data.level + 1;
    let parentId = (this.selectedNode === undefined || this.selectedNode === null) ? null : this.selectedNode.data.scopeId;
    let category = this.scopeCategoryControl.value;
    let description = this.scopeDescriptionControl.value;
    let scopeId = this.isEditScope ? this.selectedNode.data.scopeId : this.emptyGuid;

    this.awaitResponse = true;
    this.quoteService.createScope(scopeId, category, description, level, parentId, this.quoteId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.closePopup();

        let msg = { severity: 'success', summary: 'Thông báo:', detail: (this.isEditScope ? 'Sửa thành công' : 'Thêm thành công') };
        this.showMessage(msg);

        //this.getAllOrganization();
        this.listQuoteScopes = result.listQuoteScopes || [];
        /* Convert data to type TreeNode */
        this.data = this.list_to_tree();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });

    this.closePopup();
  }

  exportScope() {
    let dateUTC = new Date();
    let title = "Danh sách phạm vi công việc " + dateUTC.getDate() + '_' + (dateUTC.getMonth() + 1) + '_' + dateUTC.getUTCFullYear();

    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Danh sách phạm vi công việc');

    worksheet.pageSetup.margins = {
      left: 0.25, right: 0.25,
      top: 0.75, bottom: 0.75,
      header: 0.3, footer: 0.3
    };
    worksheet.pageSetup.paperSize = 9;

    this.buildExcelScope(worksheet);

    this.exportToExel(workbook, title);
  }

  exportExcelProduct() {
    let dateUTC = new Date();
    let title = "Danh sách sản phẩm dịch vụ " + dateUTC.getDate() + '_' + (dateUTC.getMonth() + 1) + '_' + dateUTC.getUTCFullYear();
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('BOM Lines');
    worksheet.pageSetup.margins = {
      left: 0.25, right: 0.25,
      top: 0.75, bottom: 0.75,
      header: 0.3, footer: 0.3
    };
    worksheet.pageSetup.paperSize = 9;  //A4 : 9

    let dataRow1 = [];
    dataRow1[1] = `    `
    let row1 = worksheet.addRow(dataRow1);
    row1.font = { name: 'Arial', size: 18, bold: true };
    row1.alignment = { vertical: 'bottom', horizontal: 'center', wrapText: true };

    let dataRow2 = [];
    dataRow2[1] = `    `
    dataRow2[5] = `Danh sách BOM hàng hóa
    (BOM Line)`
    let row2 = worksheet.addRow(dataRow2);
    row2.font = { name: 'Arial', size: 18, bold: true };
    worksheet.mergeCells(`E${row2.number}:H${row2.number}`);
    row2.alignment = { vertical: 'bottom', horizontal: 'center', wrapText: true };

    worksheet.addRow([]);

    let dataRow4 = [];
    dataRow4[2] = `- Các cột màu đỏ là các cột bắt buộc nhập
    - Các cột có ký hiệu (*) là các cột bắt buộc nhập theo điều kiện`
    let row4 = worksheet.addRow(dataRow4);
    row4.font = { name: 'Arial', size: 11, color: { argb: 'ff0000' } };
    row4.alignment = { vertical: 'bottom', horizontal: 'left', wrapText: true };

    worksheet.addRow([]);

    /* Header row */
    let dataHeaderRow = ['STT', 'Mã sản phẩm', 'Tên sản phẩm/Mô tả', 'Số lượng', 'Đơn giá', 'Đơn vị tính', 'Thành tiền (VND)', `Thuế suất`, `Tiền thuế`, 'Loại chiết khấu', 'Chiết khấu', 'Tổng tiền'];
    let headerRow = worksheet.addRow(dataHeaderRow);
    headerRow.font = { name: 'Arial', size: 10, bold: true };
    dataHeaderRow.forEach((item, index) => {
      headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      if (index + 1 == 4 || index + 1 == 5) {
        headerRow.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'ff0000' }
        };
      }
      else {
        headerRow.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      }
    });
    headerRow.height = 40;

    this.listQuoteDetailModel.forEach((item, index) => {
      let productCode = "";
      let productName = "";
      let productQuantity = item.quantity;
      let productPriceAmount = item.unitPrice;
      let productUnit = item.productNameUnit;
      let productAmount = productQuantity * productPriceAmount;
      let productVat = item.vat;
      let productAmountVat = (productAmount * productVat) / 100;
      let productDiscountType = item.discountType ? "%" : "Tiền";
      let productDiscountValue = item.discountValue;
      let productSumAmount = item.sumAmount;
      if (item.productId !== null) {
        productCode = this.listProduct.find(p => p.productId == item.productId)?.productCode;
        productName = item.productName;
      }
      else {
        productName = item.description;
        productUnit = item.incurredUnit;
      }

      /* Header row */
      let dataHeaderRowIndex = [index + 1, productCode, productName, productQuantity, productPriceAmount, productUnit, productAmount, productVat, productAmountVat, productDiscountType, productDiscountValue, productSumAmount];
      let headerRowIndex = worksheet.addRow(dataHeaderRowIndex);
      headerRowIndex.font = { name: 'Arial', size: 10 };
      dataHeaderRowIndex.forEach((item, index) => {
        headerRowIndex.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        if (index == 1 || index == 2 || index == 5 || index == 9) {
          headerRowIndex.getCell(index + 1).alignment = { vertical: 'bottom', horizontal: 'left' };
        }
        if (index == 3 || index == 4 || index == 6 || index == 7 || index == 8 || index == 10 || index == 11) {
          headerRowIndex.getCell(index + 1).alignment = { vertical: 'bottom', horizontal: 'right' };
        }
      });
    })

    worksheet.addRow([]);
    worksheet.getRow(2).height = 47;
    worksheet.getRow(4).height = 70;
    worksheet.getColumn(1).width = 5;
    worksheet.getColumn(2).width = 25;
    worksheet.getColumn(3).width = 25;
    worksheet.getColumn(4).width = 25;
    worksheet.getColumn(5).width = 25;
    worksheet.getColumn(6).width = 25;
    worksheet.getColumn(7).width = 25;
    worksheet.getColumn(8).width = 25;
    worksheet.getColumn(9).width = 25;
    worksheet.getColumn(10).width = 25;
    worksheet.getColumn(11).width = 25;
    worksheet.getColumn(12).width = 25;

    worksheet.getColumn(5).numFmt = '#,##0.00';
    worksheet.getColumn(7).numFmt = '#,##0.00';
    worksheet.getColumn(9).numFmt = '#,##0.00';
    worksheet.getColumn(11).numFmt = '#,##0.00';
    worksheet.getColumn(12).numFmt = '#,##0.00';

    this.exportToExel(workbook, title);
  }

  validateFile(data) {
    /* function kiểm tra tính hợp lệ của data từ file excel */
    this.messErrFile = [];
    this.cellErrFile = [];

    data.forEach((row, i) => {
      if (i > 4) {
        if ((row[1] === null || row[1] === undefined || row[1].toString().trim() == "") && (row[2] === null || row[2] === undefined || row[2].toString().trim() == "")) {
          this.messErrFile.push('Dòng { ' + (i + 2) + ' } chưa nhập Mã sản phẩm hoặc Tên sản phẩm!');
        }
        if (row[3] === null || row[3] === undefined || row[3] == "") {
          this.messErrFile.push('Cột số lượng tại dòng ' + (i + 2) + ' không được để trống');
        }
        else {
          let isPattermSL = /^\d+$/.test(row[3].toString().trim());

          if (!isPattermSL) {
            this.messErrFile.push('Cột số lượng tại dòng ' + (i + 2) + ' sai định dạng');
          }
        }
        if (row[4] === null || row[4] === undefined || row[4] == "") {
          this.messErrFile.push('Cột đơn giá tại dòng ' + (i + 2) + ' không được để trống');
        }
        else {
          let isPattermDG = /^\d+$/.test(row[4].toString().trim());

          if (!isPattermDG) {
            this.messErrFile.push('Cột đơn giá tại dòng ' + (i + 2) + ' sai định dạng');
          }
        }
      }
    });

    if (this.messErrFile.length != 0) return true;
    else return false;
  }

  downloadTemplateExcel() {
    this.quoteService.downloadTemplateProduct().subscribe(response => {
      this.loading = false;
      const result = <any>response;
      if (result.templateExcel != null && result.statusCode === 202 || result.statusCode === 200) {
        const binaryString = window.atob(result.templateExcel);
        const binaryLen = binaryString.length;
        const bytes = new Uint8Array(binaryLen);
        for (let idx = 0; idx < binaryLen; idx++) {
          const ascii = binaryString.charCodeAt(idx);
          bytes[idx] = ascii;
        }
        const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        const fileName = result.fileName + ".xls";
        link.download = fileName;
        link.click();
      }
    }, error => { this.loading = false; });
  }

  chooseFile(event: any) {
    this.fileName = event.target.files[0].name;
    this.importFileExcel = event.target;
  }

  cancelFile() {
    $("#importFileProduct").val("")
    this.fileName = "";
  }

  async showDialogImport() {
    await this.getInforDetailQuote();
    this.displayDialog = true;
  }

  async getInforDetailQuote() {
    let result: any = await this.productService.getMasterdataCreateProduct();
    if (result.statusCode === 200) {
      this.productCodeSystemList = result.listProductCode;
      this.listProductUnitName = result.listProductUnitName;
    };

    let result_2: any = await this.quoteService.getDataQuoteAddEditProductDialogAsync();
    if (result_2.statusCode == 200) {
      this.listProduct = result_2.listProduct;
      this.listUnitMoney = result_2.listUnitMoney;
      this.listUnitProduct = result_2.listUnitProduct;
    }
  }

  importExcel() {
    if (this.fileName == "") {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Chưa chọn file cần nhập" };
      this.showMessage(mgs);
    }
    else {
      const targetFiles: DataTransfer = <DataTransfer>(this.importFileExcel);
      const reader: FileReader = new FileReader();
      reader.readAsBinaryString(targetFiles.files[0]);

      reader.onload = (e: any) => {
        /* read workbook */
        const bstr: string = e.target.result;

        const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });

        // kiểm tra form value và file excel có khớp mã với nhau hay không
        let sheetName = 'BOM Lines';
        if (workbook.Sheets[sheetName] === undefined) {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: "File không hợp lệ" };
          this.showMessage(mgs);
          return;
        }

        //lấy data từ file excel của khách hàng doanh nghiệp
        const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[sheetName];
        /* save data */
        let listProductImport: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
        //remove header row
        listProductImport.shift();
        let productCodeList: string[] = [];
        let productUnitList: string[] = [];

        let isValidation = this.validateFile(listProductImport);
        if (isValidation) {
          this.isInvalidForm = true;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = true;  //Hiển thị message lỗi
        }
        else {
          var messCodeErr = [];
          var messUnitErr = [];
          this.isInvalidForm = false;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = false;  //Hiển thị message lỗi

          listProductImport.forEach((row, i) => {
            // Lấy giá trị bản ghi trong excel bắt đầu từ line 6
            if (i > 4 && row.length != 0) {
              if (row[1] !== null && row[1] !== undefined && row[1].trim() != "") {
                let rowObj = productCodeList.filter(p => p.trim().toUpperCase() == row[1].trim().toUpperCase());
                if (rowObj.length === 0) {
                  productCodeList.push(row[1].toLowerCase().trim());
                }
                let check = this.productCodeSystemList.find(d => d.toLowerCase().trim() == row[1].trim().toLowerCase());
                if (check === undefined || check === null) {
                  messCodeErr.push(i + 2);
                }
              };
              if (row[5] !== null && row[5] !== undefined && row[5].trim() != "" &&
                (row[1] !== null && row[1] !== undefined && row[1].trim() != "")) {
                let rowObj = productUnitList.filter(p => p.trim().toUpperCase() == row[5].trim().toUpperCase());
                if (rowObj.length === 0) {
                  productUnitList.push(row[5].toLowerCase().trim());
                  let check = this.listProductUnitName.find(d => d.toLowerCase().trim() == row[5].trim().toLowerCase());
                  if (check === undefined || check === null) {
                    messUnitErr.push(i + 2);
                  }
                }
              };
            }
          });

          let countCode = this.productCodeSystemList.filter(c => productCodeList.includes(c.toLowerCase().trim()));
          let countUnit = this.listProductUnitName.filter(u => productUnitList.includes(u.toLowerCase().trim()));

          //chuẩn hóa mảng
          productCodeList = [...new Set(productCodeList.map(e => Boolean(e) === true && e.trim().toLowerCase()))];
          countCode = [...new Set(countCode.map(e => Boolean(e) === true && e.trim().toLowerCase()))];
          countUnit = [...new Set(countUnit.map(e => Boolean(e) === true && e.trim().toLowerCase()))];

          if (countCode.length == productCodeList.length && countUnit.length == productUnitList.length) {
            this.listQuoteDetailExcelModel = [];

            listProductImport.forEach((row, i) => {
              // Lấy giá trị bản ghi trong excel bắt đầu từ line 6
              if (i > 4 && row.length != 0 && (
                (row[1] !== null && row[1] !== undefined && row[1].trim() != "") || (row[2] !== null && row[2] !== undefined && row[2].trim() != "") || (row[5] !== null && row[5] !== undefined && row[5].trim() != "")
              )
              ) {
                let newCustomer: QuoteDetailExcel = {
                  STT: row[0],
                  ProductCode: row[1],
                  ProductName: row[2],
                  Quantity: (row[3] === null || row[3] === undefined || row[3] == "") ? 0 : row[3],
                  UnitPrice: (row[4] === null || row[4] === undefined || row[4] == "") ? 0 : row[4],
                  CurrencyUnit: row[5],
                  Amount: (row[6] === null || row[6] === undefined || row[6] == "") ? 0 : row[6],
                  Tax: (row[7] === null || row[7] === undefined || row[7] == "") ? 0 : row[7],
                  TaxAmount: (row[8] === null || row[8] === undefined || row[8] == "") ? 0 : row[8],
                  DiscountType: row[9] == "%" ? true : false,
                  DiscountValue: (row[10] === null || row[10] === undefined || row[10] == "" || row[9] === null || row[9] === undefined || row[9].trim() == "") ? 0 : row[10],
                  TotalAmount: (row[11] === null || row[11] === undefined || row[11] == "") ? 0 : row[11]
                }
                this.listQuoteDetailExcelModel.push(newCustomer);
              }
            });
            // lấy tiền VND
            var moneyUnit = this.listUnitMoney.find(c => c.categoryCode == "VND");

            this.listQuoteDetailExcelModel.forEach(item => {
              let detailProduct = new QuoteDetail();
              if (item.ProductCode == null || item.ProductCode.trim() == "" || item.ProductCode == undefined) {
                /* dịch vụ */

                detailProduct.orderDetailType = 1;
                detailProduct.active = true;
                detailProduct.currencyUnit = moneyUnit.categoryId;
                detailProduct.discountType = item.DiscountType;
                detailProduct.exchangeRate = 1;
                detailProduct.incurredUnit = item.CurrencyUnit;
                detailProduct.nameMoneyUnit = moneyUnit.categoryCode;
                detailProduct.description = item.ProductName;
                detailProduct.productName = item.ProductName;
                detailProduct.quantity = item.Quantity;
                detailProduct.unitPrice = item.UnitPrice;
                detailProduct.vat = item.Tax;
                detailProduct.discountValue = item.DiscountValue;
                detailProduct.sumAmount = item.TotalAmount;
                detailProduct.priceInitial = 0;
                detailProduct.isPriceInitial = false;

                this.listQuoteDetailModel = [...this.listQuoteDetailModel, detailProduct];
                this.reOrderListQuoteDetail();
              }
              else {
                /* sản phẩm, hàng hóa */

                detailProduct.orderDetailType = 0;
                detailProduct.active = true;
                detailProduct.currencyUnit = moneyUnit.categoryId;
                detailProduct.discountType = item.DiscountType;
                detailProduct.exchangeRate = 1;
                detailProduct.incurredUnit = "CCCCC";
                detailProduct.nameMoneyUnit = moneyUnit.categoryCode;
                detailProduct.description = "";

                detailProduct.quantity = item.Quantity;
                detailProduct.unitPrice = item.UnitPrice;
                detailProduct.vat = item.Tax;
                detailProduct.discountValue = item.DiscountValue;
                detailProduct.sumAmount = item.TotalAmount;
                detailProduct.priceInitial = 0;
                detailProduct.isPriceInitial = false;

                let product = this.listProduct.find(p => p.productCode.trim() == item.ProductCode.trim());
                detailProduct.productId = product.productId;
                detailProduct.productCode = product.productCode;
                detailProduct.productName = item.ProductName;

                //Lấy đơn vị tính
                let productUnitId = product.productUnitId;
                let productUnitName = this.listUnitProduct.find(x => x.categoryId == productUnitId).categoryName;
                detailProduct.productNameUnit = productUnitName;
                detailProduct.unitId = productUnitId;

                this.listQuoteDetailModel = [...this.listQuoteDetailModel, detailProduct];
                this.reOrderListQuoteDetail();
              }
            });

            //set lại orderNumber
            this.listQuoteDetailModel.forEach((item, index) => {
              item.orderNumber = index + 1;
            });

            this.calculatorAmount();

            this.cancelFile();
            this.isInvalidForm = false;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = false;  //Hiển thị message lỗi
          }
          if (countCode.length != productCodeList.length) {
            this.isInvalidForm = true;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = true;  //Hiển thị message lỗi
            messCodeErr.forEach(item => {
              this.messErrFile.push('Mã sản phẩm tại dòng ' + item + ' không tồn tại trong hệ thống')
            })
          }
          if (countUnit.length != productUnitList.length) {
            this.isInvalidForm = true;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = true;  //Hiển thị message lỗi
            messUnitErr.forEach(item => {
              this.messErrFile.push('Đơn vị tính tại dòng ' + item + ' không tồn tại trong hệ thống')
            })
          }
        }
        this.displayDialog = false;
      }
    }
  }

  /* Xóa báo giá */
  delQuote() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.quoteService.updateActiveQuote(this.quoteId).subscribe(response => {
          let result: any = response;
          if (result.statusCode == 200) {
            setTimeout(() => {
              this.router.navigate(['/customer/quote-list']);
            }, 500);
          }
        });
      }
    });
  }

  /* Thay đổi trạng thái báo giá */
  updateStatusQuoteAprroval(objectTyppe: string) {
    this.messageSendQuote = null;

    if (objectTyppe == 'APPROVAL_QUOTE' && this.selectedObjectType == 'lea') {
      this.confirmationService.confirm({
        message: 'Bạn cần chuyển khách hàng tiềm năng ' + this.objCustomer.customerCodeName + ' thành Khách hàng. Bạn có muốn chuyển đổi thành Khách hàng không?',
        accept: () => {
          this.loading = true;
          this.quoteService.updateStatusQuote(this.quoteId, this.auth.UserId, objectTyppe, this.amountPriceInitial, this.amountPriceProfit, this.customerOrderAmountAfterDiscount, this.totalAmountDiscount).subscribe(response => {
            let result: any = response;
            if (result.statusCode == 200) {
              this.resetForm();
              this.getDataDefault();

              let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(mgs);
            }
            else {
              let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(mgs);
            }
          });
        }
      });
    }
    else {
      this.loading = true;
      this.isInvalidForm = false;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = false;  //Hiển thị message lỗi
      this.quoteService.updateStatusQuote(this.quoteId, this.auth.UserId, objectTyppe, this.amountPriceInitial, this.amountPriceProfit, this.customerOrderAmountAfterDiscount, this.totalAmountDiscount).subscribe(response => {
        let result: any = response;
        if (result.statusCode == 200) {
          this.resetForm();
          this.getDataDefault();
          if (objectTyppe !== 'NEW_QUOTE') {
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          }
        }
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
        }
      });
    }
  }

  /* Gửi báo giá cho khách hàng */
  sendQuote() {
    let base64 = localStorage.getItem('base64PDFQuote');
    let emailList = null;
    let objectType: Customer = this.objectControl.value;
    if (objectType.customerEmail !== null && objectType.customerEmail !== undefined && objectType.customerEmail.trim() != "") {
      emailList = objectType.customerEmail + "; ";
    }
    if (objectType.customerEmailOther !== null && objectType.customerEmailOther !== undefined && objectType.customerEmailOther.trim() != "") {
      emailList = emailList + objectType.customerEmailOther + "; ";
    }
    if (objectType.customerEmailWork !== null && objectType.customerEmailWork !== undefined && objectType.customerEmailWork.trim() != "") {
      emailList = emailList + objectType.customerEmailWork + "; ";
    }

    let ref = this.dialogService.open(SendEmailQuoteComponent, {
      data: {
        sendTo: emailList,
        quoteId: this.quoteId,
        customerId: this.objectControl.value.customerId,
        quoteCode: this.quoteCode,
        quoteMoney: this.customerOrderAmountAfterDiscount,
        customerCompany: objectType.customerCompany,
        base64: base64
      },
      header: 'Gửi Email',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "190px",
        "max-height": "800px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        let listInvalidEmail: Array<string> = result.listInvalidEmail;
        let message = `Gửi email thành công. Có <strong>${listInvalidEmail.length} email</strong> không hợp lệ:<br/>`
        listInvalidEmail.forEach(item => {
          message += `<div style="padding-left: 30px;"> -<strong>${item}</strong></div>`
        })

        if (listInvalidEmail.length > 0) {
          this.confirmationService.confirm({
            message: message,
            rejectVisible: false,
          });
        }

        let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Gửi email thành công" };
        this.showMessage(mgs);
      }
    });
  }

  /* Confirm trước khi Phê duyệt hoặc Từ chối */
  approveOrReject(isApprove) {
    this.listIdCus.push(this.quoteId);
    if (isApprove !== null) {
      if (isApprove) {
        this.confirmationService.confirm({
          message: 'Bạn có chắc chắn muốn phê duyệt báo giá này?',
          accept: () => {
            this.displayReasonApprove = true;
            this.awaitResponseApprove = false;
          }
        });
      }
      else {
        this.rejectReason = "EMP";
        this.confirmationService.confirm({
          message: 'Bạn có chắc chắn muốn từ chối báo giá này?',
          accept: () => {
            this.display = true;
          }
        });
      }
    }
  }

  // Hủy báo giá ở trạng thái đã duyệt
  rejectQuote() {
    this.listIdCus.push(this.quoteId);
    this.displayRejectQuote = true;
  }

  /* Xác nhận nguồn từ chối: Quản lý từ chối hay Khách hàng từ chối */
  confirmRejectOrder(agree: boolean) {
    //Chấp nhận từ chối
    if (agree) {
      if (this.rejectReason == 'EMP') {
        this.lyDoTuChoi = null; //Clear lý do từ chối
        this.popupNhapLyDoTuChoi = true; //Mở popup nhập lý do từ chối (do Quản lý hoặc Người phê duyệt)
      }
      else if (this.rejectReason == 'CUS') {
        this.display = true; //Mở popup nhập lý do từ chối (nguồn Khách hàng)
      }

      this.displayRejectQuote = false; //Đóng popup chọn nguồn từ chối
    }
    //Hủy từ chối
    else {
      this.displayRejectQuote = false; //Đóng popup chọn nguồn từ chối
      this.rejectReason = 'EMP';
    }
  }

  /* Khách hàng Từ chối báo giá */
  rejectOrder() {
    this.loading = true;
    this.quoteService.approvalOrRejectQuote(this.listIdCus, false, this.auth.UserId, this.descriptionReject, this.rejectReason).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      this.listIdCus = [];
      this.descriptionReject = '';

      if (result.statusCode === 202 || result.statusCode === 200) {
        this.resetForm();
        this.getDataDefault();

        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      } else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
      this.display = false;
    });
  }

  /* Phê duyệt báo giá */
  approveOrder() {
    this.loading = true;
    this.awaitResponseApprove = true;
    this.quoteService.approvalOrRejectQuote(this.listIdCus, true, this.auth.UserId, this.descriptionApprove, "").subscribe(response => {
      let result = <any>response;
      this.loading = false;
      this.listIdCus = [];
      this.descriptionApprove = '';

      if (result.statusCode === 202 || result.statusCode === 200) {
        this.resetForm();
        this.getDataDefault();
        this.displayReasonApprove = false;

        let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Phê duyệt báo giá thành công" };
        this.showMessage(mgs);
      } else {
        this.displayReasonApprove = false;
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  /* Đóng dialog lý do phê duyệt báo giá */
  closeDialogReasonApprove() {
    this.descriptionApprove = '';
  }

  /* Chuyển item lên một cấp */
  moveUp(data: QuoteDetail) {
    let currentOrderNumber = data.orderNumber;
    let preOrderNumber = currentOrderNumber - 1;
    let pre_data = this.listQuoteDetailModel.find(x => x.orderNumber == preOrderNumber);

    //Đổi số OrderNumber của 2 item
    pre_data.orderNumber = currentOrderNumber;
    data.orderNumber = preOrderNumber;

    //Xóa 2 item
    this.listQuoteDetailModel = this.listQuoteDetailModel.filter(x =>
      x.orderNumber != preOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listQuoteDetailModel = [...this.listQuoteDetailModel, pre_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listQuoteDetailModel.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }

  /* Chuyển item xuống một cấp */
  moveDown(data: QuoteDetail) {
    let currentOrderNumber = data.orderNumber;
    let nextOrderNumber = currentOrderNumber + 1;
    let next_data = this.listQuoteDetailModel.find(x => x.orderNumber == nextOrderNumber);

    //Đổi số OrderNumber của 2 item
    next_data.orderNumber = currentOrderNumber;
    data.orderNumber = nextOrderNumber;

    //Xóa 2 item
    this.listQuoteDetailModel = this.listQuoteDetailModel.filter(x =>
      x.orderNumber != nextOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listQuoteDetailModel = [...this.listQuoteDetailModel, next_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listQuoteDetailModel.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }

  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo?.systemValueString;
  }

  reOrderListQuoteDetail() {
    this.listQuoteDetailModel.forEach((item, index) => item.orderNumber = index + 1);
    this.listQuoteDetailModel.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }

  configWorkflowSteps(currentStatus: Category) {
    /* workflow ứng với luồng trạng thái phê duyệt
       Mới tạo(MTA) => Chờ duyệt(DLY) => Đã duyệt(CHO) => Báo giá(DTH) => Đóng(DON)
    */
    let listStatusCodeWorkflow1 = ["MTA", "DLY", "CHO", "DTH", "DON"];
    /* workflow ứng với luồng trạng thái từ chối
       Mới tạo(MTA) => Chờ duyệt(DLY) => Từ chối(TUCHOI) => Hủy(HUY)
    */
    let listStatusCodeWorkflow2 = ["MTA", "DLY", "TUCHOI", "HUY"];

    let listStatusCodeWorkflow3 = ["MTA", "DLY", "CHO", "DTR"];

    let listStatusCodeWorkflow = [];

    if (listStatusCodeWorkflow1.includes(currentStatus.categoryCode)) {
      listStatusCodeWorkflow = listStatusCodeWorkflow1;
    } else if (listStatusCodeWorkflow2.includes(currentStatus.categoryCode)) {
      listStatusCodeWorkflow = listStatusCodeWorkflow2;
    } else if (listStatusCodeWorkflow3.includes(currentStatus.categoryCode)) {
      listStatusCodeWorkflow = listStatusCodeWorkflow3;
    }

    const listStatusWorkflow1 = this.listQuoteStatus.filter(e => listStatusCodeWorkflow.includes(e.categoryCode)).sort((x, y) => {
      //sắp xếp theo index của mảng listStatusCodeWorkflow1
      let xIndex = listStatusCodeWorkflow.findIndex(_statusCode => _statusCode == x.categoryCode);
      let yIndex = listStatusCodeWorkflow.findIndex(_statusCode => _statusCode == y.categoryCode);
      return xIndex - yIndex;
    });
    this.workFollowQuote = [];

    listStatusWorkflow1.forEach(e => {
      let newStep: any = { label: e.categoryName };
      this.workFollowQuote = [...this.workFollowQuote, newStep];
    });

    this.activeIndex = listStatusCodeWorkflow.findIndex(e => e == currentStatus.categoryCode) || 0;
  }

  /* reset workflow nếu nhấn lưu và thêm mới: ??? */
  resetWorkFollowQuote() {
    let listStatusCodeWorkflow1 = [];
    /* workflow ứng với trạng thái Mới tạo, Chờ duyệt, Đã duyệt
      Mới tạo(MTA) => Chờ duyệt(DLY) => Đã duyệt(CHO) => Báo giá(DTH) => Đóng(DON) => Hủy(HUY)
     */
    listStatusCodeWorkflow1 = ["MTA", "DLY", "CHO", "DTH", "DON", "HUY"];
    const listStatusWorkflow1 = this.listQuoteStatus.filter(e => listStatusCodeWorkflow1.includes(e.categoryCode)).sort((x, y) => {
      //sắp xếp theo index của mảng listStatusCodeWorkflow1
      let xIndex = listStatusCodeWorkflow1.findIndex(_statusCode => _statusCode == x.categoryCode);
      let yIndex = listStatusCodeWorkflow1.findIndex(_statusCode => _statusCode == y.categoryCode);
      return xIndex - yIndex;
    });
    this.workFollowQuote = [];
    listStatusWorkflow1.forEach(e => {
      let newStep: any = { label: e.categoryName };
      this.workFollowQuote = [...this.workFollowQuote, newStep];
    });
    this.activeIndex = 0;
  }

  /* Thay đổi người tham gia */
  changeParticipant() {
    if (this.participantControl.value?.length > 0) {
      this.tooltipSelectedParticipant = this.participantControl.value?.map(x => x.employeeCode).join(', ');
    }
    else {
      this.tooltipSelectedParticipant = null;
    }
  }

  gotoViewCustomer() {
    this.router.navigate(['/customer/detail', { customerId: this.objCustomer.customerId }]);
  }

  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch (unit) {
      case -1: {
        result = result;
        break;
      }
      case 0: {
        result = Math.round(result);
        break;
      }
      case 1: {
        result = Math.round(number * 10) / 10;
        break;
      }
      case 2: {
        result = Math.round(number * 100) / 100;
        break;
      }
      case 3: {
        result = Math.round(number * 1000) / 1000;
        break;
      }
      case 4: {
        result = Math.round(number * 10000) / 10000;
        break;
      }
      default: {
        result = result;
        break;
      }
    }
    return result;
  }

  /*Show popup các CTKM có thể áp dụng*/
  showPromotion(conditionsType: number, productId?: string, quantity?: number, productName?: string) {
    let titleHeader = '';
    if (conditionsType == 1) {
      titleHeader = 'khuyến mại dành cho ' + this.objCustomer.customerCodeName;
    }
    else if (conditionsType == 2) {
      titleHeader = 'khuyến mại dành cho ' + productName;
    }
    else if (conditionsType == 3) {
      titleHeader = 'khuyến mại dành cho Tổng giá trị sản phẩm';
    }

    let ref = this.dialogService.open(PromotionApplyPopupComponent, {
      data: {
        conditionsType: conditionsType,
        customerId: this.objCustomer?.customerId,
        listReshowPromotionApply: this.listPromotionApply,
        totalAmountAfterVat: this.totalAmountAfterVat,
        productId: productId,
        quantity: quantity
      },
      header: titleHeader,
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe(result => {
      if (result) {
        let selectedPromotionApply: Array<PromotionApply> = result;

        if (conditionsType == 1) {
          this.listPromotionApply = this.listPromotionApply.filter(x => x.conditionsType != 1);

          selectedPromotionApply.forEach(item => {
            item.selectedPromotionProductApply.forEach(product => {
              let promotionApply = new PromotionObjectApply();
              promotionApply.promotionId = item.promotionId;
              promotionApply.promotionName = item.promotionName;
              promotionApply.conditionsType = item.conditionsType;
              promotionApply.propertyType = item.propertyType;
              promotionApply.propertyTypeName = item.propertyTypeName;
              promotionApply.notMultiplition = item.notMultiplition;
              promotionApply.promotionMappingId = product.promotionMappingId;
              promotionApply.productId = product.productId;
              promotionApply.productUnitName = product.productUnitName;
              promotionApply.promotionProductName = product.promotionProductName;
              promotionApply.promotionProductNameConvert = product.promotionProductNameConvert;
              promotionApply.soLuongTang = product.soLuongTang;
              promotionApply.loaiGiaTri = product.loaiGiaTri;
              promotionApply.giaTri = product.giaTri;
              promotionApply.soTienTu = product.soTienTu;

              this.listPromotionApply = [...this.listPromotionApply, promotionApply];
            });
          });
        }
        else if (conditionsType == 3) {
          this.listPromotionApply = this.listPromotionApply.filter(x => x.conditionsType != 3);

          selectedPromotionApply.forEach(item => {
            item.selectedPromotionProductApply.forEach(product => {
              let promotionApply = new PromotionObjectApply();
              promotionApply.promotionId = item.promotionId;
              promotionApply.promotionName = item.promotionName;
              promotionApply.conditionsType = item.conditionsType;
              promotionApply.propertyType = item.propertyType;
              promotionApply.propertyTypeName = item.propertyTypeName;
              promotionApply.notMultiplition = item.notMultiplition;
              promotionApply.promotionMappingId = product.promotionMappingId;
              promotionApply.productId = product.productId;
              promotionApply.productUnitName = product.productUnitName;
              promotionApply.promotionProductName = product.promotionProductName;
              promotionApply.promotionProductNameConvert = product.promotionProductNameConvert;
              promotionApply.soLuongTang = product.soLuongTang;
              promotionApply.loaiGiaTri = product.loaiGiaTri;
              promotionApply.giaTri = product.giaTri;
              promotionApply.soTienTu = product.soTienTu;

              this.listPromotionApply = [...this.listPromotionApply, promotionApply];
            });
          });
        }
        else if (conditionsType == 2) {
          this.listPromotionApply = this.listPromotionApply.filter(x => x.conditionsType != 2);

          selectedPromotionApply.forEach(item => {
            if (item.propertyType == 3) {
              item.selectedPromotionProductApply.forEach(product => {
                let promotionApply = new PromotionObjectApply();
                promotionApply.promotionId = item.promotionId;
                promotionApply.promotionName = item.promotionName;
                promotionApply.conditionsType = item.conditionsType;
                promotionApply.propertyType = item.propertyType;
                promotionApply.propertyTypeName = item.propertyTypeName;
                promotionApply.notMultiplition = item.notMultiplition;
                promotionApply.promotionMappingId = product.promotionMappingId;
                promotionApply.productId = product.productId;
                promotionApply.productUnitName = product.productUnitName;
                promotionApply.promotionProductName = product.promotionProductName;
                promotionApply.promotionProductNameConvert = product.promotionProductNameConvert;
                promotionApply.soLuongTang = product.soLuongTang;
                promotionApply.loaiGiaTri = product.loaiGiaTri;
                promotionApply.giaTri = product.giaTri;
                promotionApply.soTienTu = product.soTienTu;

                this.listPromotionApply = [...this.listPromotionApply, promotionApply];
              });
            }
            else {

            }
          });
        }

        this.calculatorAmount();
      }
    });
  }

  /*Thay đổi vat của cả đơn hàng*/
  changeVatValue() {
    if (this.appName == 'VNS') {
      //Danh sách sản phẩm dịch vụ: Reset vat của tất cả sản phẩm về 0
      this.listQuoteDetailModel.forEach(item => {
        item.vat = 0;
      });
    }

    let value = this.vatControl.value;

    if (value == '') {
      this.vatControl.setValue('0');
    }
    else {
      let vat = ParseStringToFloat(value);

      if (vat > 100) {
        this.vatControl.setValue('100');
      }
    }

    this.calculatorAmount();
  }

  /*Event khi thay đổi loại tạm ứng: Theo % hoặc Theo số tiền*/
  changePercentAdvanceType(value: DiscountType) {
    this.percentAdvanceControl.setValue('0');

    this.calculatorAmount();
  }

  /* Thay đổi % tạm ứng */
  changePercentAdvance() {
    if (this.appName == 'VNS') {
      let value = this.percentAdvanceControl.value;

      if (value == '') {
        this.percentAdvanceControl.setValue('0');
      }
      else {
        let type = this.percentAdvanceTypeControl.value;
        if (type.value) {
          let percentAdvance = ParseStringToFloat(this.percentAdvanceControl.value);

          if (percentAdvance > 100) {
            this.percentAdvanceControl.setValue('100');
          }
        }
      }
    }
    else {
      let value = this.percentAdvanceControl.value;

      if (value == '') {
        this.percentAdvanceControl.setValue('0');
      }
      else {
        let percentAdvance = ParseStringToFloat(this.percentAdvanceControl.value);

        if (percentAdvance > 100) {
          this.percentAdvanceControl.setValue('100');
        }
      }
    }

    this.calculatorAmount();
  }

  buildExcelScope(worksheet: Worksheet) {

    let colA = worksheet.getColumn('A');
    colA.width = 6.28;

    let colB = worksheet.getColumn('B');
    colB.width = 18;

    let colC = worksheet.getColumn('C');
    colC.width = 58.42;

    let colD = worksheet.getColumn('D');
    colD.width = 68.71;

    let scopeName = '';

    if (this.listQuoteScopes.length > 0) {
      scopeName = this.listQuoteScopes.find(x => x.tt == '').category;
    }

    let dataRow1 = 'Danh sách phạm vi công việc ' + scopeName;
    let row1 = worksheet.getRow(3);
    row1.getCell('B').value = dataRow1;
    row1.font = { name: 'Times New Roman', size: 13, bold: true };
    worksheet.mergeCells(`B${row1.number}:D${row1.number}`);
    row1.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    row1.height = 33.75;

    /* Header row */
    let dataHeaderRow = ['', 'STT', 'Hạng mục', 'Mô tả'];
    let headerRow = worksheet.addRow(dataHeaderRow);
    headerRow.font = { name: 'Times New Roman', size: 13, bold: true };
    worksheet.mergeCells(`B${headerRow.number}:B${headerRow.number + 1}`);
    worksheet.mergeCells(`C${headerRow.number}:C${headerRow.number + 1}`);
    worksheet.mergeCells(`D${headerRow.number}:D${headerRow.number + 1}`);

    dataHeaderRow.forEach((item, index) => {
      if (item != '') {
        headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

        headerRow.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      }
    });

    let data: Array<any> = [];
    this.listQuoteScopes.forEach(item => {
      let row: Array<any> = [];
      row[2] = item.tt;
      row[3] = item.category;
      row[4] = item.description;
      data.push(row);
    })

    data.forEach((item, index) => {
      if (item != '') {
        if (item[2] != '') {
          let row = worksheet.addRow(item);
          if (item[2].includes('.')) {
            row.font = { name: 'Times New Roman', size: 13 };
            row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', indent: item[2].split('.').length };
            row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true, indent: item[2].split('.').length };
          }
          else {
            row.font = { name: 'Times New Roman', size: 13, bold: true };
            row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left' };
            row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
          }

          row.height = 20.71;

          row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

          row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };

          row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
        }
      }
    })

    worksheet.addRow([]);
  }

  addPaymentTerm() {
    let item = new QuotePaymentTerm();
    item.orderNumber = this.listQuotePaymentTerm.length + 1;
    this.listQuotePaymentTerm = [...this.listQuotePaymentTerm, item];
  }

  deletePaymentTerm(rowData: QuotePaymentTerm) {
    this.listQuotePaymentTerm = this.listQuotePaymentTerm.filter(e => e != rowData);
    var index = 1;
    this.listQuotePaymentTerm.forEach(item => { item.orderNumber = index; index++; })
  }

  guiPheDuyet() {
    this.quyTrinhService.guiPheDuyet(this.quoteId, 3).subscribe(res => {
      let result: any = res;

      if (result.statusCode == 200) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);

        this.resetForm();
        this.getDataDefault();
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  huyYeuCauPheDuyet() {
    this.quyTrinhService.huyYeuCauPheDuyet(this.quoteId, 3).subscribe(res => {
      let result: any = res;

      if (result.statusCode == 200) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);

        this.resetForm();
        this.getDataDefault();
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  /* Mở popup xác nhận */
  popupXacNhan(value: boolean) {
    //Phê duyệt
    if (value) {
      this.confirmationService.confirm({
        message: 'Bạn có chắc chắn muốn phê duyệt báo giá này?',
        accept: () => {
          this.display = true;
        }
      });
    }
    //Từ chối
    else {
      this.lyDoTuChoi = null; //Clear lý do từ chối

      this.confirmationService.confirm({
        message: 'Bạn có chắc chắn muốn từ chối báo giá này?',
        accept: () => {
          this.popupNhapLyDoTuChoi = true;
        }
      });
    }
  }

  pheDuyet() {
    this.displayReasonApprove = false; //Đóng popup

    this.loading = true;
    this.quyTrinhService.pheDuyet(this.quoteId, 3, this.descriptionApprove).subscribe(res => {
      let result: any = res;

      if (result.statusCode == 200) {
        this.descriptionApprove = '';

        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);

        this.resetForm();
        this.getDataDefault();
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  tuChoi() {
    this.popupNhapLyDoTuChoi = false; //Đóng popup

    this.loading = true;
    this.quyTrinhService.tuChoi(this.quoteId, 3, this.lyDoTuChoi).subscribe(res => {
      let result: any = res;

      if (result.statusCode == 200) {
        this.lyDoTuChoi = null;

        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);

        this.resetForm();
        this.getDataDefault();
      }
      else {
        this.loading = false;
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  xemLichSuPheDuyet() {
    let ref = this.dialogService.open(LichSuPheDuyetComponent, {
      data: {
        objectId: this.quoteId,
        doiTuongApDung: 3
      },
      header: 'Lịch sử phê duyệt',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "190px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {

      }
    });
  }

  getDuLieuQuyTrinh() {
    this.quyTrinhService.getDuLieuQuyTrinh(this.quoteId, 3).subscribe(res => {
      let result: any = res;

      if (result.statusCode == 200) {
        this.listDuLieuQuyTrinh = result.listDuLieuQuyTrinh;
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  changeActive(item) {

  }

  handleChange(event) {
    this.CDRef.detectChanges();
  }
}

function splitStringContent(str: string) {
  // split string thanh 3 phan
  // arr[0] - phan dau: doan html truoc bang
  // arr[1] - phan giua: doan html bang
  // arr[2] - phan cuoi: doan html sau bang
  let arr = [];

  arr[0] = str.substring(0, str.indexOf('<table'));

  arr[1] = str.substring(str.indexOf('<table'), str.indexOf('</table>') + 8);

  arr[2] = str.substring(str.lastIndexOf('</table>') + 8)

  return arr;
}

function filterByValue(array, value) {
  return array.filter((data) => JSON.stringify(data).toLowerCase().indexOf(value.toLowerCase()) !== -1);
}

function convertStringContentToTable(str: string) {
  let strTmp = str.substring(str.indexOf('<tr>'), str.lastIndexOf('</tr>') + 5);
  let check = true;
  let result = [];
  let index = 0;

  while (check) {
    if (strTmp != '' && strTmp != null && strTmp != undefined) {
      result[index] = strTmp.substring(strTmp.indexOf('<tr>'), strTmp.indexOf('</tr>') + 5).replace(/(<([^>]+)>)/gi, " ").trim();
      strTmp = strTmp.substring((strTmp.substring(strTmp.indexOf('<tr>'), strTmp.indexOf('</tr>') + 5)).length);
      index++;
    } else {
      check = false;
    }
  }

  return result;
}

function convertStringContent(str: string) {
  let strTmp = '';
  let result = [];
  let check = true;
  let index = 0;

  while (check) {
    if (str != '' && str != null && str != undefined) {
      if (str.includes('</p>')) {
        strTmp = str.substring(0, str.indexOf('</p>'));
        result[index] = strTmp.replace(/(<([^>]+)>)/gi, " ").trim();
        index++;
      }
      else {
        result[0] = str;
        check = false;
      }
    } else {
      check = false;
    }
    str = str.substring(strTmp.length + 4);
  }

  return result;
}

function convertStringCusName(str: string) {
  if (str.includes("-", 0)) {
    str = str.replace("-", " ");
  }
  while (str.includes("  ", 0)) {
    str = str.replace("  ", " ");
  }
  while (str.includes(" ", 0)) {
    str = str.replace(" ", "_");
  }
  return str;
}

function convertStringTitle(title: string) {
  var index = title.substring(0, title.indexOf('.'));
  var content = title.substring(title.indexOf('.'));
  var indexNumber = parseFloat(index.replace(/,/g, ''));
  var result = romanize(indexNumber) + content;
  return result;
}

function romanize(num) {
  var lookup = { M: 1000, CM: 900, D: 500, CD: 400, C: 100, XC: 90, L: 50, XL: 40, X: 10, IX: 9, V: 5, IV: 4, I: 1 },
    roman = '',
    i;
  for (i in lookup) {
    while (num >= lookup[i]) {
      roman += i;
      num -= lookup[i];
    }
  }
  return roman;
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

//So sánh giá trị nhập vào có thuộc khoảng xác định hay không?
function ageRangeValidator(min: number, max: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } | null => {
    if (control.value !== undefined && (isNaN(control.value) ||
      parseFloat(control.value.replace(/,/g, '')) < min ||
      parseFloat(control.value.replace(/,/g, '')) > max)) {
      return { 'ageRange': true };
    }
    return null;
  };
}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
