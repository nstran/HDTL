export class CategoryEntityModel {
    public categoryId : string;
    public categoryName : string;
    public categoryCode : string;
    public categoryTypeId : string;
    public categoryTypeName : string;
    public createdById : string;
    public createdDate : Date;
    public updatedDate : Date;
    public updatedById : string;
    public active : boolean;
    public isEdit : boolean;
    public isDefault : boolean;
    public categoryTypeCode : string;
    public countCategoryById : number;
    public sortOrder : number;
    public isDefauld : boolean;
    public tenantId : string;
  }
  export class ProductEntityModel {
    productId: string;
    productCategoryId: string;
    productName: string;
    productCode: string;
    price1: number | null;
    price2: number | null;
    createdById: string;
    createdDate: string;
    updatedById: string | null;
    updatedDate: string | null;
    active: boolean | null;
    quantity: number | null;
    productUnitId: string | null;
    productUnitName: string;
    productDescription: string;
    vat: number | null;
    minimumInventoryQuantity: number | null;
    productMoneyUnitId: string | null;
    productCategoryName: string;
    listVendorName: string;
    guarantee: number | null;
    guaranteeTime: number | null;
    countProductInformation: number;
    exWarehousePrice: number | null;
    calculateInventoryPricesId: string | null;
    propertyId: string | null;
    warehouseAccountId: string | null;
    revenueAccountId: string | null;
    payableAccountId: string | null;
    importTax: number | null;
    costPriceAccountId: string | null;
    accountReturnsId: string | null;
    folowInventory: boolean | null;
    managerSerialNumber: boolean | null;
    productCodeName: string;
    propertyName: string;
    calculateInventoryPricesName: string;
    fixedPrice: number | null;
    loaiKinhDoanh: string | null;
    loaiKinhDoanhCode: string;
    serviceTypeId : string;
  }
  export class GetListServiceTypeResult {
    public listProductCategory : CategoryEntityModel;
    public statusCode : number;
    public messageCode : string;
  }