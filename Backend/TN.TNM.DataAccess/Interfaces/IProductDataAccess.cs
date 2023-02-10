using System;
using System.Threading.Tasks;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;
using TN.TNM.DataAccess.Messages.Results.Admin.Product;
using TN.TNM.DataAccess.Models.Options;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IProductDataAccess
    {
        SearchProductResult SearchProduct(SearchProductParameter parameter);
        CreateProductResult CreateProduct(CreateProductParameter parameter);
        GetProductByIDResult GetProductByID(GetProductByIDParameter parameter);
        UpdateProductResult UpdateProduct(UpdateProductParameter parameter);
        GetProductByVendorIDResult GetProductByVendorID(GetProductByVendorIDParameter parameter);
        GetProductAttributeByProductIDResult GetProductAttributeByProductID(GetProductAttributeByProductIDParameter parameter);
        GetAllProductCodeResult GetAllProductCode(GetAllProductCodeParameter parameter);
        UpdateActiveProductResult UpdateActiveProduct(UpdateActiveProductParameter parameter);
        GetListProductResult GetListProduct(GetListProductParameter parameter);
        GetMasterdataCreateProductResult GetMasterdataCreateProduct(GetMasterdataCreateProductParameter parameter);
        AddSerialNumberResult AddSerialNumber(AddSerialNumberParameter parameter);
        GetMasterDataVendorDialogResult GetMasterDataVendorDialog(GetMasterDataVendorDialogParameter parameter);
        DownloadTemplateProductServiceResult DownloadTemplateProductService(DownloadTemplateProductServiceParameter parameter);
        ImportProductResult ImportProduct(ImportProductParameter parameter);
        GetMasterDataPriceProductResult GetMasterDataPriceList(GetMasterDataPriceProductParameter parameter);
        CreateOrUpdatePriceProductResult CreateOrUpdatePriceProduct(CreateOrUpdatePriceProductParameter parameter);
        DeletePriceProductResult DeletePriceProduct(DeletePriceProductParameter parameter);
        GetDataQuickCreateProductResult GetDataQuickCreateProduct(GetDataQuickCreateProductParameter parameter);
        GetDataCreateUpdateBOMResult GetDataCreateUpdateBOM(GetDataCreateUpdateBOMParameter parameter);
        DownloadPriceProductTemplateResult DownloadPriceProductTemplate(DownloadPriceProductTemplateParameter parameter);
        ImportPriceProductResult ImportPriceProduct(ImportPriceProductParamter parameter);
        CreateThuocTinhSanPhamResult CreateThuocTinhSanPham(CreateThuocTinhSanPhamParameter parameter);
        DeleteThuocTinhSanPhamResult DeleteThuocTinhSanPham(DeleteThuocTinhSanPhamParameter parameter);

        Task<GetListServiceTypeResult> GetListServiceType();

        Task<CreateProductOptionResult> CreateProductOption(CreateProductOptionParameter parameter);

        Task<TakeListOptionResult> TakeListOption(TakeListOptionsParameter parameter);

        Task<CreateOrEditProductResult> CreateOrEditProduct(CreateOrEditProductParameter parameter);

        Task<TakeProductAndOptionsByIdResult> TakeProductAndOptionsById(TakeProductAndOptionsByIdParameter parameter);

        Task<GetMasterDataCreateServicePacketResult> GetMasterDataCreateServicePacket(GetMasterDataCreateServicePacketParameter parameter);

        GetListEmployeeByRoleIdResult GetListEmployeeByRoleId(GetListEmployeeByRoleIdParameter parameter);

        Task<CreateOrUpdateServicePacketResult> CreateOrUpdateServicePacket(CreateOrUpdateServicePacketParameter parameter);

        Task<GetListServicePacketResult> GetListServicePacket(GetListServicePacketParameter parameter);

        Task<DeleteServicePacketResult> DeleteServicePacket(DeleteServicePacketParameter parameter);

        Task<GetServicePacketByIdResult> GetServicePacketById(GetServicePacketByIdParameter parameter);

        CreateServicePacketMappingOptionResult CreateServicePacketMappingOption(CreateServicePacketMappingOptionParameter parameter);

        DeleteServicePacketMappingOptionResult DeleteServicePacketMappingOption(DeleteServicePacketMappingOptionParameter parameter);
        ChangeOrderServicePackResult ChangeOrderServicePack(ChangeOrderServicePackParameter parameter);

        
    }
}
