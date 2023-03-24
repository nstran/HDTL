import { Timestamp } from "rxjs";
import { CustomerOrderDetail } from "./customer-order-detail.model";
var emptyGuid: string = '00000000-0000-0000-0000-000000000000';
export class CustomerOrder {
  OrderId: string;
  OrderCode: string;
  OrderDate: Date;
  Seller: string;
  Description: string;
  Note: string;
  CustomerId: string;
  CustomerContactId: string;
  PaymentMethod: string;
  DiscountType: boolean;
  BankAccountId: string;
  DaysAreOwed: number;
  MaxDebt: number;
  ReceivedDate: Date;
  ReceivedHour: Date;
  RecipientName: string;
  LocationOfShipment: string;
  ShippingNote: string;
  RecipientPhone: string;
  RecipientEmail: string;
  PlaceOfDelivery: string;
  Amount: number;
  DiscountValue: number;
  ReceiptInvoiceAmount: number;
  StatusId: string;//Guid?
  Active: boolean;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  QuoteId: string;
  ReasonCancel: string;
  IsAutoGenReceiveInfor: boolean;
  CustomerName: string;
  CustomerAddress: string;
  OrderContractId: string;
  WarehouseId: string;
  StatusOrder: number;
  OrderType: number;
  ObjectId: string;
  PaymentMethodNote: string;
  PaymentMethodOrder: number;
  PaymentContent: string;


  constructor() {
    this.OrderId = emptyGuid,
      this.OrderCode = null,
      this.OrderDate = null,
      this.Seller = null,
      this.Description = null,
      this.Note = null,
      this.CustomerId = null,
      this.CustomerContactId = null,
      this.PaymentMethod = null,
      this.DiscountType = true,
      this.BankAccountId = null,
      this.DaysAreOwed = null,
      this.MaxDebt = null,
      this.ReceivedDate = null,
      this.ReceivedHour = null,
      this.RecipientName = null,
      this.LocationOfShipment = null,
      this.ShippingNote = null,
      this.RecipientPhone = null,
      this.RecipientEmail = null,
      this.PlaceOfDelivery = null,
      this.Amount = 0,
      this.DiscountValue = 0,
      this.ReceiptInvoiceAmount = null,
      this.StatusId = null,
      this.Active = true,
      this.CreatedById = emptyGuid,
      this.CreatedDate = new Date(),
      this.UpdatedById = null,
      this.UpdatedDate = null,
      this.QuoteId = null,
      this.ReasonCancel = null,
      this.CustomerName = null;
    this.CustomerAddress = null;
    this.OrderContractId = null;
    this.WarehouseId = null;
    this.StatusOrder = null;
    this.OrderType = 1;
    this.ObjectId = "";
  }
}

export class CustomerOrderExtension {
  id: string;
  attributeConfigurationId: string | null;
  objectId: string | null;
  objectType: string;
  value: string;
  dataType: string;
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string;
  tenantId: string;
}



export class CustomerOrderTaskEntityModel {
  id: string;
  servicePacketMappingOptionsId: string | null;
  optionId: string;
  optionName: string;
  path: string;
  vendorId: string | null;
  vendorName: string;
  empId: string | null;
  listEmpId: Array<string> | null;
  listEmpName: string | null;
  empName: string;
  mission: string;
  note: string;
  servicePacketId: string;
  isExtend: boolean;
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string;
  tenantId: string;
  isPersonInCharge: boolean;
  orderActionId: string;
}

export class ReportPointEntityModel {
  id: string | null;
  name: string;
  optionId: string | null;
  optionName: string;
  orderActionId: string | null;
  order: number | null;
  empId: string | null;
  empName: string;
  deadline: Date;
  isCusView: boolean | null;
  content: string;
  status: number | null;
  statusName: string;
  createdById: string | null;
  createdDate: string | null;
  updatedById: string | null;
  isExtend: boolean | null;
  updatedDate: string | null;
  servicePacketMappingOptionsId: string;
  isReporter: string;
}

export interface SelectItem {
  label: string;
  value: any;
  disabled: boolean;
}