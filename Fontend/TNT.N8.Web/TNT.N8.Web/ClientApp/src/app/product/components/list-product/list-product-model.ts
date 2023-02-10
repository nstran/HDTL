import { ProductEntityModel } from "../../models/product.model";

export class ProductCategory {
    public productCategoryId: string;
    public productCategoryName: string;
}

export class Vendor {
    public vendorId: string;
    public vendorName: string;
}

export class Product {
    public productId: string;
    public productCode: string;
    public productName: string;
    public productCategoryName: string;
    public listVendorName: string;
    public productUnitName: string;
    public propertyName: string;
    public calculateInventoryPricesName: string;
}

export class GetListProductResult {
    productEntityModel : ProductEntityModel [];
    statusCode : number;
    messageCode : string;
}