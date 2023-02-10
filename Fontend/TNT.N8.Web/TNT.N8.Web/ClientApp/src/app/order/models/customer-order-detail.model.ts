import { OrderProductDetailProductAttributeValue } from "./order-product-detail-product-attribute-value.model"
export class CustomerOrderDetail {
  orderDetailId: string;
  vendorId: string;
  orderId: string;
  productId: string;
  quantity: number;
  unitPrice: number;
  currencyUnit: string;
  exchangeRate: number;
  vat: number;
  discountType: boolean;
  discountValue: number;
  description: string;
  orderDetailType: number;
  unitId: string;
  incurredUnit: string;
  active: boolean;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  UpdatedDate: Date;
  OrderProductDetailProductAttributeValue: Array<OrderProductDetailProductAttributeValue>;
  ExplainStr: string;
  NameVendor: string;
  ProductNameUnit: string;
  NameMoneyUnit: string;
  SumAmount: number;
  GuaranteeTime: number;
  GuaranteeDatetime: Date;
  AmountDiscount: number; //add by dung
  ExpirationDate: Date;
  WarehouseId: string;
  priceInitial: number;
  IsPriceInitial: boolean;
  WarrantyPeriod: number;
  ActualInventory: number;
  BusinessInventory: number;
  ProductCode: string;
  ProductName: string;
  WareCode: string;
  OrderNumber: number;
  UnitLaborPrice: number;
  UnitLaborNumber: number;
  SumAmountLabor: number;
  FolowInventory: boolean;
  ProductCategoryId: string;
  optionId: string;
  servicePacketId: string;
  optionName: string;

  constructor() {
    this.orderDetailId = '00000000-0000-0000-0000-000000000000',
      this.vendorId = '',
      this.orderId = '00000000-0000-0000-0000-000000000000',
      this.productId = '',
      this.ProductCategoryId = '00000000-0000-0000-0000-000000000000',
      this.quantity = 0,
      this.unitPrice = 0,
      this.currencyUnit = '00000000-0000-0000-0000-000000000000',
      this.exchangeRate = 1,
      this.vat = 0,
      this.discountType = true,
      this.discountValue = 0,
      this.description = '',
      this.orderDetailType = 0,
      this.unitId = '',
      this.incurredUnit = '',
      this.active = true,
      this.createdById = '00000000-0000-0000-0000-000000000000',
      this.createdDate = new Date(),
      this.updatedById = null,
      this.UpdatedDate = null,
      this.OrderProductDetailProductAttributeValue = [],
      this.ExplainStr = '',
      this.NameVendor = '',
      this.ProductNameUnit = '',
      this.NameMoneyUnit = '',
      this.SumAmount = 0,
      this.GuaranteeTime = null;
    this.GuaranteeDatetime = null;
    this.AmountDiscount = 0;
    this.ExpirationDate = null;
    this.WarehouseId = null;
    this.priceInitial = null;
    this.IsPriceInitial = false;
    this.WarrantyPeriod = null;
    this.ActualInventory = null;
    this.BusinessInventory = null;
    this.OrderNumber = 0;
    this.UnitLaborPrice = 0;
    this.UnitLaborNumber = 0;
    this.SumAmountLabor = 0;
    this.FolowInventory = false;
  }
}

export class CustomerOrderExtension {
  id: string;
  attributeConfigurationId: string | null;
  objectId: string | null;
  objectType: string;
  value: string;
  dataType: number;
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string;
  tenantId: string;
  attrName: string;
  servicePacketMappingOptionsId: string;
}


export class CustomerOrderDetailExten {
  id: string;
  orderId: string;
  name: string | null;
  quantity: number | null;
  price: number;
  statusObject: any;
  status: number;
  statusName: string;
  edit: boolean;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  tenantId: string;

  constructor() {
    this.id = '00000000-0000-0000-0000-000000000000',
      this.orderId = '00000000-0000-0000-0000-000000000000',
      this.name = '',
      this.quantity = 1,
      this.price = null,
      this.status = 2,
      this.statusName = null,
      this.statusObject = null,
      this.createdById = '00000000-0000-0000-0000-000000000000',
      this.createdDate = new Date(),
      this.updatedById = null,
      this.updatedDate = null
  }

}

