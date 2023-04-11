
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProductModel, ProductModel2, ProductQuantityInWarehouseModel, ProductAttributeCategory, ProductImageModel, InventoryReportModel, ProductVendorMappingModel, PriceProductModel, GetListServiceTypeResult, CreateProductOptionResult, OptionsEntityModel, GetListProductOptionResult, TakeListOptionResult, ProductEntityModel, CreateOrEditProductResult, CreateOrEditProductParameter, TakeProductAndOptionsByIdResult, GetListServicePacketResult, GetServicePacketByIdResult, CreateOrUpdateServicePacketParameter, CreateOrUpdateServicePacketResult, GetMasterDataCreateServicePacketResult, DownloadServicePacketImageResponse, CreateServicePacketMappingOptionResult, DeleteServicePacketMappingOptionResult, ServicePacketMappingOptionsEntityModel, EditServicePacketMappingOptionResult, GetListEmployeeByRoleIdResult, ServicePacketImage } from '../models/product.model';
import { ProductAttributeCategoryModel } from '../models/productAttributeCategory.model';
import { Observable } from 'rxjs';
import { GetListProductResult } from '../components/list-product/list-product-model';
@Injectable()
export class ProductService {

  constructor(private httpClient: HttpClient) { }

  searchProduct(productName: string, productCode: string, listProductCategory: Array<string>, listVendor: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/searchProduct';
    return this.httpClient.post(url, {
      ProductName: productName,
      ProductCode: productCode,
      ListProductCategory: listProductCategory,
      ListVendor: listVendor
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createProduct(product: ProductModel, LstVendor: Array<string>, LstProductAttributeCategory: Array<ProductAttributeCategoryModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/createProduct';
    return this.httpClient.post(url, {
      Product: product,
      lstVendor: LstVendor,
      lstProductAttributeCategory: LstProductAttributeCategory,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createProductAsync(
    product: ProductModel,
    ListProductVendorMapping: Array<ProductVendorMappingModel>,
    ListProductAttributeCategory: Array<ProductAttributeCategory>,
    ListProductBillOfMaterials: Array<any>,
    ListInventoryReport: Array<ProductQuantityInWarehouseModel>,
    ListProductImage: Array<ProductImageModel>,
    userId: string
  ) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/createProduct';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Product: product,
        ListProductVendorMapping: ListProductVendorMapping,
        ListProductAttributeCategory: ListProductAttributeCategory,
        ListProductBillOfMaterials: ListProductBillOfMaterials,
        ListInventoryReport: ListInventoryReport,
        ListProductImage: ListProductImage,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getProductByID(productId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/GetProductByID';
    return this.httpClient.post(url, {
      ProductId: productId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateProductAsync(product: ProductModel2,
    listProductVendorMapping: Array<ProductVendorMappingModel>,
    ListProductBillOfMaterials: Array<any>,
    ListProductImage: Array<ProductImageModel>,
    userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/UpdateProduct';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Product: product,
        ListProductVendorMapping: listProductVendorMapping,
        ListProductBillOfMaterials: ListProductBillOfMaterials,
        ListProductImage: ListProductImage,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateActiveProduct(productId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/updateActiveProduct';
    return this.httpClient.post(url, {
      ProductId: productId
    }).pipe(
      map((response: Response) => {
        return response;
      }));

  }

  getProductAttributeByProductID(productId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/getProductAttributeByProductID';
    return this.httpClient.post(url, {
      ProductId: productId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getProductByVendorID(vendorId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/getProductByVendorID';
    return this.httpClient.post(url, {
      VendorId: vendorId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getProductByVendorIDAsync(vendorId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/getProductByVendorID";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { VendorId: vendorId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  getListCodeAsync() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/getAllProductCode";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  searchProductAsync(productName: string, productCode: string, listProductCategory: Array<string>, listVendor: Array<string>, listKieuHinh) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/searchProduct";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ProductName: productName,
        ProductCode: productCode,
        ListProductCategory: listProductCategory,
        ListVendor: listVendor,
        ListKieuHinhKinhDoanh: listKieuHinh,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getListProduct(filterText : string) : Observable<GetListProductResult> {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/getListProduct";
    return this.httpClient.post(url, {FilterText : filterText}).pipe(
      map((response: GetListProductResult) => {
        return <GetListProductResult>response;
      }));
  }

  getMasterdataCreateProduct() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/getMasterdataCreateProduct";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  addSerialNumber(productId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/dddSerialNumber";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { ProductId: productId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getProductByIDAsync(productId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/GetProductByID';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ProductId: productId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  downloadProductImage(listImageUrl: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/file/downloadProductImage';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListFileUrl: listImageUrl
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataVendorDialog() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/getMasterDataVendorDialog";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  downloadTemplateProduct() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/downloadTemplateProductService';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  importProduct(listProduct: Array<ProductModel2>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/importProduct';
    return this.httpClient.post(url, { ListProduct: listProduct }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getMasterDataPriceList() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/getMasterDataPriceList';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  createOrUpdatePriceProduct(priceProduct: PriceProductModel) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/createOrUpdatePriceProduct';
    return this.httpClient.post(url, { PriceProduct: priceProduct }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  deletePriceProduct(priceProductId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/deletePriceProduct';
    return this.httpClient.post(url, { PriceProductId: priceProductId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getDataCreateUpdateBOM() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/getDataCreateUpdateBOM";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  downloadPriceProductTemplate() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/downloadPriceProductTemplate';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  importPriceProduct(list: Array<PriceProductModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/importPriceProduct';
    return this.httpClient.post(url, { ListPriceProduct: list }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  createThuocTinhSanPham(productId: string, thuocTinh: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/createThuocTinhSanPham";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ProductId: productId,
        ThuocTinh: thuocTinh
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteThuocTinhSanPham(id: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/deleteThuocTinhSanPham";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ProductAttributeCategoryId: id,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getListProductOption(): Observable<GetListProductOptionResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/getListProductOption';
    return this.httpClient.get(url).pipe(
      map((response: GetListProductOptionResult) => {
        return <GetListProductOptionResult>response;
      }));
  }

  getListServiceType(): Observable<GetListServiceTypeResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/getListServiceType';
    return this.httpClient.get(url).pipe(
      map((response: GetListServiceTypeResult) => {
        return <GetListServiceTypeResult>response;
      }));
  }

  createProductOption(optionsEntityModel: OptionsEntityModel): Observable<CreateProductOptionResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/createProductOption';
    return this.httpClient.post(url, { optionsEntityModel: optionsEntityModel }).pipe(
      map((response: CreateProductOptionResult) => {
        return <CreateProductOptionResult>response;
      }));
  }

  getListProductOptions(categoryId: string): Observable<TakeListOptionResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/takeListOption';
    return this.httpClient.post(url, { CategoryId: categoryId }).pipe(
      map((response: TakeListOptionResult) => {
        return <TakeListOptionResult>response;
      }));
  }

  createOrEditProduct(createOrEditProductParameter: CreateOrEditProductParameter): Observable<CreateOrEditProductResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/createOrEditProduct';
    return this.httpClient.post(url,
      {
        ProductEntityModel: createOrEditProductParameter.productEntityModel,
        listOptionsEntityModel: createOrEditProductParameter.listOptionsEntityModel
      })
      .pipe(
        map((response: CreateOrEditProductResult) => {
          return <CreateOrEditProductResult>response;
        }));
  }

  takeProductAndOptionsById(productId: string): Observable<TakeProductAndOptionsByIdResult>  {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/takeProductAndOptionsById';
    return this.httpClient.post(url, { ProductId: productId }).pipe(
      map((response: TakeProductAndOptionsByIdResult) => {
        return <TakeProductAndOptionsByIdResult>response;
      }));
  }

  getMasterDataCreateServicePacket(): Observable<GetMasterDataCreateServicePacketResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/getMasterDataCreateServicePacket';
    return this.httpClient.get(url).pipe(
      map((response: GetMasterDataCreateServicePacketResult) => {
        return <GetMasterDataCreateServicePacketResult>response;
      }));
  }
  
  createOrUpdateServicePacket(createOrEditProductParameter: CreateOrUpdateServicePacketParameter): Observable<CreateOrUpdateServicePacketResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/createOrUpdateServicePacket';
    return this.httpClient.post(url, 
      {
        ServicePacketEntityModel: createOrEditProductParameter.servicePacketEntityModel,
        ListServicePacketAttributeEntityModel: createOrEditProductParameter.listServicePacketAttributeEntityModel,
        ListServicePacketConfigurationPermissionModel: createOrEditProductParameter.listServicePacketConfigurationPermissionModel,
        ServicePacketImageEntityModel : createOrEditProductParameter.servicePacketImageEntityModel,
        ListNotificationConfigurationModel : createOrEditProductParameter.listNotificationConfigurationModel,
        CauHinhQuyTrinh : createOrEditProductParameter.cauHinhQuyTrinh, 
        ListManagerId: createOrEditProductParameter.listManagerId
      })
      .pipe(
        map((response: CreateOrUpdateServicePacketResult) => {
          return <CreateOrUpdateServicePacketResult>response;
      }));
  }

  getListServicePacket(filterText : string): Observable<GetListServicePacketResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/getListServicePacket';
    return this.httpClient.post(url, {FilterText : filterText}).pipe(
      map((response: GetListServicePacketResult) => {
        return <GetListServicePacketResult>response;
      }));
  }

  deleteServicePacket(id : string): Observable<GetListServicePacketResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/deleteServicePacket';
    return this.httpClient.post(url, {Id : id}).pipe(
      map((response: GetListServicePacketResult) => {
        return <GetListServicePacketResult>response;
      }));
  }

  getServicePacketById(id : string): Observable<GetServicePacketByIdResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/getServicePacketById';
    return this.httpClient.post(url, {Id : id}).pipe(
      map((response: GetServicePacketByIdResult) => {
        return <GetServicePacketByIdResult>response;
      }));
  }

  searchOptionOfPacketService(packetServiceId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/searchOptionOfPacketService';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        PacketServiceId: packetServiceId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteServicePacketMappingOption(id : string): Observable<DeleteServicePacketMappingOptionResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/deleteServicePacketMappingOption';
    return this.httpClient.post(url, {Id : id}).pipe(
      map((response: DeleteServicePacketMappingOptionResult) => {
        return <DeleteServicePacketMappingOptionResult>response;
      }));
  }

  editServicePacketMappingOption(id : string, name: string, sortOrder: number): Observable<EditServicePacketMappingOptionResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/editServicePacketMappingOption';
    return this.httpClient.post(url, {Id : id, Name : name, SortOrder : sortOrder}).pipe(
      map((response: EditServicePacketMappingOptionResult) => {
        return <EditServicePacketMappingOptionResult>response;
      }));
  }

  createServicePacketMappingOption(servicePacketMappingOptionsEntityModel : ServicePacketMappingOptionsEntityModel): Observable<CreateServicePacketMappingOptionResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/createServicePacketMappingOption';
    return this.httpClient.post(url, {ServicePacketMappingOptionsEntityModel : servicePacketMappingOptionsEntityModel}).pipe(
      map((response: CreateServicePacketMappingOptionResult) => {
        return <CreateServicePacketMappingOptionResult>response;
      }));
  }

  getListEmployeeByRoleId(roleId : string): Observable<GetListEmployeeByRoleIdResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/getListEmployeeByRoleId';
    return this.httpClient.post(url, {RoleId : roleId}).pipe(
      map((response: GetListEmployeeByRoleIdResult) => {
        return <GetListEmployeeByRoleIdResult>response;
      }));
  }

  
  changeOrderServicePack(id : string, stt: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/changeOrderServicePack';
    return this.httpClient.post(url, {Id : id, Stt: stt}).pipe(
      map((response) => {
        return response;
      }));
  }

  uploadServicePacketImage(fileList: File[]): Promise<Response> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/uploadServicePacketImage';
    let formData: FormData = new FormData();
    for (var i = 0; i < fileList.length; i++) {
      formData.append('fileList', fileList[i]);
    }
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
}
