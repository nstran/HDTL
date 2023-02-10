import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { CustomerOrder } from '../../models/customer-order.model';
import { CustomerOrderDetail } from '../../models/customer-order-detail.model';
import { OrderProductDetailProductAttributeValue } from '../../models/order-product-detail-product-attribute-value.model';
import { ContactModel } from '../../../shared/models/contact.model';
import { CustomerService } from '../../../customer/services/customer.service';
import { CustomerOrderService } from '../../services/customer-order.service';
import { ContactService } from '../../../shared/services/contact.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import * as $ from 'jquery';
import * as pdfMake from 'pdfmake/build/pdfmake.js';
import * as pdfFonts from 'pdfmake/build/vfs_fonts.js';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { OrderDetailDialogComponent } from '../order-detail-dialog/order-detail-dialog.component';
import { PopupComponent } from '../../../shared/components/popup/popup.component';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { NoteModel } from '../../../shared/models/note.model';
import { NoteService } from '../../../shared/services/note.service';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { PopupAddEditCostQuoteDialogComponent } from '../../../shared/components/add-edit-cost-quote/add-edit-cost-quote.component';
import { OrderCostDetail } from '../../models/customer-order-cost-detail.model';
import { QuoteService } from '../../../customer/services/quote.service';
import { MenuItem } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';
import { Workbook } from 'exceljs';
pdfMake.vfs = pdfFonts.pdfMake.vfs;
import { saveAs } from "file-saver";
import { ForderConfigurationService } from '../../../admin/components/folder-configuration/services/folder-configuration.service';
import { ContractService } from '../../../sales/services/contract.service';

interface ResultDialog {
  status: boolean,
  customerOrderDetailModel: CustomerOrderDetail,
}

class Folder {
  folderId: string;
  parentId: string;
  name: string;
  url: string;
  isDelete: boolean;
  active: boolean;
  hasChild: boolean;
  listFile: Array<FileInFolder>;
  folderType: string;
  numberFile: number;
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

interface ResultCostDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  quoteDetailModel: OrderCostDetail,
}

interface DiscountType {
  name: string;
  code: string;
  value: boolean;
}

interface OrderStatus {
  orderStatusId: string;
  orderStatusCode: string;
  orderStatusName: string;
  description: string;
}

interface Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  active: boolean;
}

interface Customer {
  customerId: string;
  customerCode: string;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  fullAddress: string;
  paymentId: string;
  maximumDebtDays: number;
  maximumDebtValue: number;
  taxCode: string;
  personInChargeId: string;
}

interface Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}

interface BankAccount {
  bankAccountId: string;
  bankName: string; //Ngân hàng
  accountNumber: string;  //Số tài khoản
  branchName: string; //Chi nhánh
  accountName: string; //Chủ tài khoản
  objectId: string;
}

interface CustomerType {
  typeValue: number;
  typeName: string;
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

class TonKhoTheoSanPham {
  productId: string;
  tonKho: number;
  warehouseId: string;
  warehouseName: string;
}

@Component({
  selector: 'app-order-detail',
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.css'],
})
export class OrderDetailComponent implements OnInit {
 
  constructor(private translate: TranslateService,
    private router: Router,
    private location: Location,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private customerService: CustomerService,
    private customerOrderService: CustomerOrderService,
    private contactService: ContactService,
    public cdRef: ChangeDetectorRef,
    private dialogService: DialogService,
    public dialog: MatDialog,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private renderer: Renderer2,
    private noteService: NoteService,
    private imageService: ImageUploadService,
    private quoteService: QuoteService,
    private folderService: ForderConfigurationService,
    private contractService: ContractService,
  ) {
   
  }

  async ngOnInit() {
    
    
  }
  getMasterData() {
  
    // this.customerOrderService.getMasterDataOrderDetail(this.orderId).subscribe(response => {
    //   let result: any = response;
    //   this.loading = false;
    //   if (result.statusCode == 200) {
    //     this.listAllOrderStatus = result.listOrderStatus;
    //     this.listEmployee = result.listEmployee;
    //     this.listPersonInCharge = result.listEmployee;
    //     this.listCustomer = result.listCustomer;
    //     this.listPaymentMethod = result.listPaymentMethod;
    //     this.listCustomerBankAccount = result.listCustomerBankAccount;
    //     this.listCustomerGroup = result.listCustomerGroup;
    //     this.listCustomerCode = result.listCustomerCode;
    //     this.listQuote = result.listQuote;
    //     this.listWare = result.listWare;
    //     this.listWarehouseLevel0 = this.listWare.filter(e => e.warehouseParent == null); //lấy kho nút 0
    //     this.getListWarehouseStartQuantity();
    //     this.listProduct = result.listProduct;
    //     this.isManager = result.isManager;
    //     this.listInventoryDeliveryVoucher = result.listInventoryDeliveryVoucher;
    //     this.listBill = result.listBillSale;
    //     this.listOrderContract = result.listContract;
    //     this.listPaymentInfor = result.listPaymentInformationEntityModel;

    //     this.listFile = result.listFile;
    //     this.listTonKhoTheoSanPham = result.listTonKhoTheoSanPham;

    //     /* Map data Order */
    //     let customerOrderObject = result.customerOrderObject;

    //     this.typeAccount = result.customerOrderObject.typeAccount;

    //     this.customerOrderModel.OrderId = customerOrderObject.orderId;
    //     this.customerOrderModel.OrderCode = customerOrderObject.orderCode;
    //     this.customerOrderModel.OrderDate = new Date(customerOrderObject.orderDate);
    //     this.customerOrderModel.Seller = customerOrderObject.seller;
    //     this.customerOrderModel.Description = customerOrderObject.description;
    //     this.customerOrderModel.Note = customerOrderObject.note;
    //     this.customerOrderModel.CustomerId = customerOrderObject.customerId;
    //     this.customerOrderModel.CustomerContactId = customerOrderObject.customerContactId;
    //     this.customerOrderModel.PaymentMethod = customerOrderObject.paymentMethod;
    //     this.customerOrderModel.DiscountType = customerOrderObject.discountType;
    //     this.customerOrderModel.BankAccountId = customerOrderObject.bankAccountId;
    //     this.customerOrderModel.DaysAreOwed = customerOrderObject.daysAreOwed;
    //     this.customerOrderModel.MaxDebt = customerOrderObject.maxDebt;
    //     this.customerOrderModel.ReceivedDate = customerOrderObject.receivedDate == null ? null : new Date(customerOrderObject.receivedDate);
    //     this.customerOrderModel.ReceivedHour = customerOrderObject.receivedHour;
    //     this.customerOrderModel.RecipientName = customerOrderObject.recipientName;
    //     this.customerOrderModel.LocationOfShipment = customerOrderObject.locationOfShipment;
    //     this.customerOrderModel.ShippingNote = customerOrderObject.shippingNote;
    //     this.customerOrderModel.RecipientPhone = customerOrderObject.recipientPhone;
    //     this.customerOrderModel.RecipientEmail = customerOrderObject.recipientEmail;
    //     this.customerOrderModel.PlaceOfDelivery = customerOrderObject.placeOfDelivery;
    //     this.customerOrderModel.Amount = customerOrderObject.amount;
    //     this.customerOrderModel.DiscountValue = customerOrderObject.discountValue;
    //     this.customerOrderModel.ReceiptInvoiceAmount = customerOrderObject.receiptInvoiceAmount;
    //     this.customerOrderModel.StatusId = customerOrderObject.statusId;
    //     this.customerOrderModel.CreatedById = customerOrderObject.createdById;
    //     this.customerOrderModel.CreatedDate = customerOrderObject.createdDate == null ? new Date() : new Date(customerOrderObject.createdDate);
    //     this.customerOrderModel.CustomerName = customerOrderObject.customerName;
    //     this.customerOrderModel.CustomerAddress = customerOrderObject.customerAddress;
    //     this.customerOrderModel.WarehouseId = customerOrderObject.warehouseId;
    //     this.customerOrderModel.QuoteId = customerOrderObject.quoteId;
    //     this.customerOrderModel.OrderContractId = customerOrderObject.orderContractId;
    //     this.customerOrderModel.UpdatedById = customerOrderObject.updatedById;
    //     this.customerOrderModel.UpdatedDate = customerOrderObject.updatedDate;
    //     this.statusOld = customerOrderObject.statusId;
    //     this.autoGenValue = customerOrderObject.isAutoGenReceiveInfor;
    //     /* End */

    //     if (this.auth.UserId == this.customerOrderModel.UpdatedById) {
    //       this.isUserSendAproval = true;
    //     }

    //     /*Reshow Time Line */
    //     this.noteHistory = result.listNote;
    //     this.handleNoteContent();

    //     this.checkIsDraft();

    //     this.checkLicensed();

    //     this.filterOrderStatus(this.customerOrderModel.StatusId);

    //     this.mapDataToForm();

    //     /* Map data list Order Detail */
    //     this.mapDataToTableProduct(result.listCustomerOrderDetail);
    //     /* End */
    //     /* Map data list Order COST */
    //     this.mapDataToTableCost(result.listCustomerOrderCostDetail);
    //     /* End */

    //     this.calculatorAll();
    //   } else {
    //     this.loading = false;
    //     let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
    //     this.showMessage(msg);
    //   }
    // });
  }

  

}

