import { TreeNode } from "primeng";
import { CauHinhQuyTrinh } from '../../../../src/app/admin/models/cau-hinh-quy-trinh.model';

export class ProductModel {
  ProductId: string;
  ProductCategoryId: string;
  ProductName: string;
  ProductCode: string;
  Price1: number;
  Price2: number;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: Boolean;
  Quantity: number;
  ProductUnitId: string;
  ProductDescription: string;
  Vat: Number;
  MinimumInventoryQuantity: Number;
  ProductMoneyUnitId: string;
  GuaranteeTime: Number;
  ProductCategoryName: string;
  ListVendorName: string;
  //ProductAttribute: Array<ProductAttributeModel>;
  //ProductVendorMapping: Array<ProductVendorMappingModel>;
}

export class ProductModel2 {
  ProductId: string;
  ProductCategoryId: string;
  ProductName: string;
  ProductCode: string;
  Price1: number;
  Price2: number;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: Boolean;
  Quantity: number;
  ProductUnitId: string;
  ProductDescription: string;
  Vat: Number;
  MinimumInventoryQuantity: Number;
  ProductMoneyUnitId: string;
  GuaranteeTime: Number;
  ProductCategoryName: string;
  ListVendorName: string;
  ExWarehousePrice: number;
  CountProductInformation: number;
  CalculateInventoryPricesId: string;
  PropertyId: string;
  WarehouseAccountId: string;
  RevenueAccountId: string;
  PayableAccountId: string;
  ImportTax: number;
  CostPriceAccountId: string;
  AccountReturnsId: string;
  FolowInventory: boolean;
  ManagerSerialNumber: boolean;
  LoaiKinhDoanh: string;

  constructor() {
    this.CountProductInformation = 0;
  }
}

export class DetailProductModel {
  productId: string;
  productCategoryId: string;
  productName: string;
  productCode: string;
  price1: number;
  price2: number;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  active: Boolean;
  quantity: number;
  productUnitId: string;
  productDescription: string;
  vat: Number;
  minimumInventoryQuantity: Number;
  productMoneyUnitId: string;
  guaranteeTime: Number;
  productCategoryName: string;
  listVendorName: string;
  exWarehousePrice: number;
  countProductInformation: number;
  calculateInventoryPricesId: string;
  propertyId: string;
  warehouseAccountId: string;
  revenueAccountId: string;
  payableAccountId: string;
  importTax: number;
  costPriceAccountId: string;
  accountReturnsId: string;
  folowInventory: boolean;
  managerSerialNumber: boolean;
  loaiKinhDoanh: string;
  //ProductAttribute: Array<ProductAttributeModel>;
  //ProductVendorMapping: Array<ProductVendorMappingModel>;
  constructor() {
    this.countProductInformation = 0;
  }
}


export class ProductAttributeCategory {
  public ProductAttributeCategoryId: string;
  public ProductAttributeCategoryName: string;
  public CreatedById: string;
  public CreatedDate: Date;
  public UpdatedById: string;
  public UpdatedDate: Date;
  public Active: boolean;
  public ProductAttributeCategoryValue: Array<ProductAttributeCategoryValueModel>;
  constructor() {
    this.ProductAttributeCategoryValue = [];
  }
}

export class ProductAttributeCategoryValueModel {
  public ProductAttributeCategoryValueId: string;
  public ProductAttributeCategoryValue1: string;
  public ProductAttributeCategoryId: string;
  public CreatedById: string;
  public CreatedDate: Date;
  public UpdatedById: string;
  public UpdatedDate: Date;
  public Active: boolean;
}

export class ProductVendorMappingModel {
  VendorId: string;
  VendorName: string;
  VendorCode: string;
  VendorProductName: string;
  MiniumQuantity: number;
  Price: number;
  MoneyUnitId: string;
  MoneyUnitName: string;
  FromDate: Date;
  ToDate: Date;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  OrderNumber: number;
  ExchangeRate: number;
}
export class ProductAttributeModel {
  ProductAttributeCategoryId: string;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
}

export class productMoneyUnitModel {
  public categoryId: string;
  public categoryCode: string;
  public categoryName: string;
  public isDefauld: string;
}

export class productUnitModel {
  public categoryId: string;
  public categoryCode: string;
  public categoryName: string;
  public isDefauld: string;
}
//loại hình kinh doanh của sản phẩm: chỉ mua vào, chỉ bán ra, cả 2
export class productLoaiHinhKinhDoanhModel {
  public categoryId: string;
  public categoryCode: string;
  public categoryName: string;
}

export class vendorModel {
  public vendorId: string;
  public vendorName: string;
  public vendorCode: string;
}

export class warehouseModel {
  public warehouseId: string;
  public warehouseName: string;
  public warehouseCode: string;
  public warehouseParent: string;
  public warehouseNameByLevel: string;
  public listWarehouseNameByLevel: Array<string>;//thêm mới label theo phân cấp
  constructor() {
    this.listWarehouseNameByLevel = [];
  }
}

export class ProductQuantityInWarehouseModel {
  public InventoryReportId: string;
  public WarehouseId: string;
  public ProductId: string;
  public Quantity: number;
  public QuantityMinimum: number;
  public QuantityMaximum: number;
  public Active: boolean;
  public CreatedDate: Date;
  public CreatedById: string;
  public UpdatedDate: Date;
  public UpdatedById: string;
  public StartQuantity: number;
  public OpeningBalance: number;
  public Note: string;
  public ListSerial: Array<Serial>;

  constructor() {
    this.InventoryReportId = '00000000-0000-0000-0000-000000000000';
    this.WarehouseId = '00000000-0000-0000-0000-000000000000';
    this.ProductId = '00000000-0000-0000-0000-000000000000';
    this.Quantity = 0;
    this.QuantityMinimum = 0;
    this.QuantityMaximum = null;
    this.StartQuantity = 0;
    this.OpeningBalance = 0;
    this.Note = null;
    this.ListSerial = [];
    this.Active = true;
    this.CreatedDate = new Date();
    this.CreatedById = '00000000-0000-0000-0000-000000000000';
  }
}

export class Serial {
  public SerialId: string;
  public SerialCode: string;
  public ProductId: string;
  public StatusId: string;
  public WarehouseId: string;
  public Active: boolean;
  public CreatedDate: Date;
  public CreatedById: string;
  public UpdatedDate: Date;
  public UpdatedById: string;

  constructor() {
    this.SerialId = '00000000-0000-0000-0000-000000000000';
    this.ProductId = '00000000-0000-0000-0000-000000000000';
    this.StatusId = '00000000-0000-0000-0000-000000000000';
    this.WarehouseId = '00000000-0000-0000-0000-000000000000';
    this.Active = true;
    this.CreatedDate = new Date();
    this.CreatedById = '00000000-0000-0000-0000-000000000000';
  }
}

export class ProductImageModel {
  public ProductImageId: string;
  public ProductId: string;
  public ImageName: string;
  public ImageSize: string;
  public ImageUrl: string;
  public Active: boolean;
  public CreatedById: string;
  public CreatedDate: Date;
  public UpdatedById: string;
  public UpdatedDate: Date;
}

export class InventoryReportModel {
  public InventoryReportId: string;
  public WarehouseId: string;
  public ProductId: string;
  public Quantity: number;
  public QuantityMinimum: number;
  public QuantityMaximum: number;
  public Note: string;
  public Active: boolean;
  public CreatedDate: Date;
  public CreatedById: string;
  public UpdatedDate: Date;
  public UpdatedById: string;
  public StartQuantity: number;
  public OpeningBalance: number;
}

export class PriceProductModel {
  public PriceProductId: string;
  public ProductId: string;
  public EffectiveDate: Date;
  public PriceVnd: number;
  public MinQuantity: number;
  public PriceForeignMoney: number;
  public CustomerGroupCategory: string;
  public Active: boolean;
  public CreatedDate: Date;
  public CreatedById: string;
  public UpdatedDate: Date;
  public UpdatedById: string;
  public TiLeChietKhau: number;
  public NgayHetHan: Date;
}

export class GetListServiceTypeResult {
  public listProductCategory: CategoryEntityModel[];
  public statusCode: number;
  public messageCode: string;
}

export class CategoryEntityModel {
  public categoryId: string;
  public categoryName: string;
  public categoryCode: string;
  public categoryTypeId: string;
  public categoryTypeName: string;
  public createdById: string;
  public createdDate: Date;
  public updatedDate: Date;
  public updatedById: string;
  public active: boolean;
  public isEdit: boolean;
  public isDefault: boolean;
  public categoryTypeCode: string;
  public countCategoryById: number;
  public sortOrder: number;
  public isDefauld: boolean;
  public tenantId: string;
}

export class GetListProductOptionResult {
  public listProductOption: CategoryEntityModel[];
  public statusCode: number;
  public messageCode: string;
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
  listOptionsEntityModel : OptionsEntityModel [];
  packetServiceId : string | null;
}


export class OptionsEntityModel {
  id: string;
  parentId: string;
  categoryId: string;
  name: string;
  nameCustom: string;
  description: string;
  note: string;
  enableNumber: boolean | null;
  isShowQuantity: boolean;
  isActive: boolean;
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string;
  tenantId: string;
  price: number;
  ServiceTypeName: string;
  servicePacketId: string;
  isExtend: boolean;
}

export class CreateProductOptionResult {
  statusCode: number;
  messageCode: string;
}

export class TakeListOptionResult {
  listOption: OptionsEntityModel[];
}

export class CreateOrEditProductResult {
  id : string;
  statusCode: number;
  messageCode: string;
}

export class ProductMappingOptionModel {
  id: string;
  productId: string;
  optionId: string;
  vendorId: string | null;
  price: number | null;
  amount: number | null;
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string;
  tenantId: string;
}

export class CreateOrEditProductParameter {
  productEntityModel : ProductEntityModel;
  listOptionsEntityModel : OptionsEntityModel [];
}

export class TakeProductAndOptionsByIdResult {
  productEntityModel : ProductEntityModel;
  statusCode : number;
  messageCode : string;
}

export class TakeProductAndOptionsByIdParameter {
  productEntityModel : ProductEntityModel;
  statusCode : number;
  messageCode : string;
}

export class TakeListProductMappingOptionsByProductIdResult {
  listOptionsEntityModel : OptionsEntityModel [];
  statusCode : number;
  messageCode : string;
}

export class ServicePacketEntityModel {
  id: string;
  name: string;
  attributeName: string;
  message: string;
  description: string;
  locationId: string;
  status: string;
  productCategoryId: string;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  tenantId: string;
  provinceId: string;
  provinceName: string;
  stt: number;
  code: string;
  listPermissionConfigurationEntityModel : PermissionConfigurationEntityModel[];
  listRoleServicePacket : RoleEntityModel[];
  listServicePacketAttributeEntityModel : ServicePacketAttributeEntityModel[];
  listNotificationConfigurationEntityModel : NotificationConfigurationEntityModel[];

  constructor() {
    this.id = "00000000-0000-0000-0000-000000000000";
    this.createdById = "00000000-0000-0000-0000-000000000000";
    this.updatedById = "00000000-0000-0000-0000-000000000000";
    this.createdDate = new Date();
    this.updatedDate = new Date();
  }
}

export class DownloadServicePacketImageResponse{
  listServicePacketImageResponseModel: ServicePacketImageResponseModel[];
}

export class ServicePacketImageResponseModel {
  fileAsBase64: string;
  fileName: string;
  fileType: string;
  fileSize: number;
}

export class ServicePacketAttributeEntityModel {
  id: string;
  dataType: number;
  objectType: number;
  objectId: string;
  categoryId: string;
  categoryName: string;
  dataTypeName: string;
  constructor() {
    this.id = "00000000-0000-0000-0000-000000000000";
  }
}

export class ProductCategoryEntityModel {
  productCategoryId: string;
  productCategoryName: string;
  productCategoryLevel: number | null;
  productCategoryCode: string;
  description: string;
  parentId: string | null;
  createdById: string;
  createdDate: string;
  updatedById: string | null;
  updatedDate: string | null;
  active: boolean | null;
  tenantId: string | null;
}

export class AttrConfigure {
  value: number;
  name: string;
}

export class EmployeeEntityModel {
  employeeId: string;
  mission: string;
  employeeName: string;
  employeeCode: string;
  OrganizationId: string;
  PositionId: string;
  EmployeeCodeName: string;
  roleId : string | null;
}

export class ProvinceEntityModel {
  provinceId: string;
  provinceName: string;
  provinceCode: string;
  provinceType: string;
  active: boolean | null;
}

export class GetMasterDataCreateServicePacketResult {
  listDataTypeAttr: TrangThaiGeneral[];
  listRoleServicePacket: RoleEntityModel[];
  listAttrCategory: CategoryEntityModel[];
  listProductCategoryEntityModel: ProductCategoryEntityModel[];
  listEmpWithRole: EmployeeEntityModel[];
  listProvince : ProvinceEntityModel[];
  listStepServicePacketSelect : PermissionConfigurationEntityModel[];
  listServicePacketConfigurationPermissionModel: PermissionConfigurationEntityModel[];
  listOptionEntityModel: OptionsEntityModel[];
  listNotificationConfigurationEntityModel : NotificationConfigurationEntityModel[];
  statusCode: number;
  messageCode: string;
}

export class ServicePacketMappingProductEntityModel {
  id: string;
  productCategoryId: string;
  productId: string | null;
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string;
  tenantId: string;
}

export class PermissionConfigurationEntityModel {
  id: string;
  stepId: number | null;
  roleId: string | null;
  servicePacketId: string | null;
  employeeId: string | null;
  listEmployeeEntityModel: EmployeeEntityModel[];
  listEmployeeEntityModelByRoleId: EmployeeEntityModel[];
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string;
  tenantId: string;
  categoryId : string | null;
  categoryName : string | null;
  categoryCode : string | null;
  isEdit : boolean | null;
  objectRole : RoleEntityModel;
  stepServicePacketSelect : PermissionConfigurationEntityModel;
  constructor(data?: PermissionConfigurationEntityModel) {
    if (data) {
        for (var property in data) {
            if (data.hasOwnProperty(property))
                (<any>this)[property] = (<any>data)[property];
        }
    }
  }
}

export class ServicePacketImageEntityModel {
  id: string;
  servicePacketId: string;
  mainImage: string;
  data : string;
  type : string;
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string | null;
  tenantId: string;
  backgroundImage: string;
  icon: string;
}

export class NotificationConfigurationEntityModel {
  id: string;
  servicePacketId: string | null;
  categoryId: string | null;
  serviceRequestMaker: boolean | null;
  serviceManagement: boolean | null;
  serviceSupporter: boolean | null;
  supporter: boolean | null;
  reporter: boolean | null;
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string;
  tenantId: string;
  sortOrder: number | null;
  categoryName: string;
  listEmployeeEntityModel : EmployeeEntityModel[];
  listNotificationRecipient: NotificationConfigurationEntityModel;
  notificationRecipient: string = '';
}

export class CreateOrUpdateServicePacketParameter {
  servicePacketEntityModel: ServicePacketEntityModel;
  listServicePacketAttributeEntityModel: ServicePacketAttributeEntityModel[] = [];
  listServicePacketConfigurationPermissionModel: PermissionConfigurationEntityModel[] = [];
  servicePacketImageEntityModel : ServicePacketImageEntityModel;
  listNotificationConfigurationModel : NotificationConfigurationEntityModel[];
  cauHinhQuyTrinh : CauHinhQuyTrinh;
  listManagerId: Array<string>;
}

export class CreateOrUpdateServicePacketResult {
  statusCode: number;
  messageCode: string;
}

export class TrangThaiGeneral {
  value: number;
  valueText: string;
  name: string;
  objectRole : TrangThaiGeneral;
  listEmployeeEntityModel : EmployeeEntityModel[];
  isEdit : boolean | null;
  categoryId : string | null;
}

export class RoleEntityModel {
  roleId: string;
  roleValue: string;
}

export class GetListServicePacketResult {
  listServicePackageEntityModel : ServicePacketEntityModel[];
  statusCode: number;
  messageCode: string;
}

export class GetServicePacketByIdResult {
  servicePacketEntityModel : ServicePacketEntityModel;
  servicePacketImageEntityModel : ServicePacketImageEntityModel;
  cauHinhQuyTrinh : CauHinhQuyTrinh;
  listManager : Array<string>;
}

export class ImageModel {
  source: string;
  imageSize: string;
  imageName: string;
  imageType: string;
  imageUrl: string;
  alt: string;
  title: string;
}


export class AttributeConfigurationEntityModel {
  id: string | null;
  dataType: number | null;
  dataTypeName: string;
  categoryId: string | null;
  categoryName: string;
  createdById: string | null;
  createdDate: string | null;
  updatedById: string | null;
  updatedDate: string | null;
  tenantId: string | null;
  objectType: number | null;
  objectId: string | null;
  value: any;
}

export class CustomerOrderExtension {
  id: string | null;
  dataType: number | null;
  attributeConfigurationId: string;
  dataTypeName: string;
  categoryId: string | null;
  categoryName: string;
  createdById: string | null;
  createdDate: string | null;
  updatedById: string | null;
  updatedDate: string | null;
  tenantId: string | null;
  objectType: number | null;
  objectId: string | null;
  value: any;
  orderDetailId: any;
  servicePacketMappingOptionsId: any; 
}


export class ServicePacketMappingOptionsEntityModel {
  id: string | null;
  createdById: string | null;
  createdDate: string;
  updatedById: string | null;
  updatedDate: string;
  tenantId: string | null;
  parentId: string | null;
  name: string;
  nameCustom: string;
  price: number | null;
  vat: number | null;
  optionId: string | null;
  servicePacketId: string | null;
  sortOrder: number | null;
}

export interface NewTreeNode extends TreeNode {
  id : string;
  listAttr?: any;
  path: string;
  number: string;
  margin: string;
  sortOrder: number | null;
  optionId: string | null;
  price: number | null;
}

export class CreateServicePacketMappingOptionResult {
  statusCode: number;
  messageCode: string;
  message : string;
}

export class CreateServicePacketMappingOptionParameter {
  servicePacketMappingOptionsEntityModel : ServicePacketMappingOptionsEntityModel;
}

export class DeleteServicePacketMappingOptionResult {
  statusCode: number;
  messageCode: string;
  message : string;
}

export class EditServicePacketMappingOptionResult {
  statusCode: number;
  messageCode: string;
  message : string;
}

export class DeleteServicePacketMappingOptionParameter {
  id : string;
}

export class GetListEmployeeByRoleIdResult {
    listEmployeeEntityModel: EmployeeEntityModel[];
}


export class OrderProcessDetailEntityModel {
  id: string;
  stepId: number | null;
  roleId: string | null;
  servicePackageId: string | null;
  employeeId: string | null;
  listEmployeeEntityModel: EmployeeEntityModel[];
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string;
  tenantId: string;
  categoryId : string | null;
  categoryName : string | null;
  categoryCode : string | null;
  isEdit : boolean | null;
  objectRole : RoleEntityModel;
  stepServicePacketSelect : PermissionConfigurationEntityModel;
  status : Number;
  orderProcessId : string;
  statusName : string;

  orderId : string;
  orderActionId : string;
  listExtenOrder: Array<any> = []; //CustomerOrder
  isAction: Array<any> = [];  //Người đăng nhập có quyền thao tác tại bước này

}

export class OrderProcessEntityModel {
  id: string;
  servicePacketId: string | null;
  orderProcessCode: string;
  customerId: string | null;
  productCategoryId: string | null;
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string;
  tenantId: string;
  status: number | null;
  backgroundColorForStatus: string | null;
  isCreator: boolean;
}