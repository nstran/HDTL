using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.Product;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Product;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Product;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Results.Admin.Product;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;
using System.Threading.Tasks;
using TN.TNM.DataAccess.Models.Options;
using System;
using TN.TNM.DataAccess.Databases.Entities;
using System.IO;
using System.Net;
using TN.TNM.Common;
using TN.TNM.BusinessLogic.Messages.Responses.File;
using TN.TNM.BusinessLogic.Messages.Requests.File;
using Microsoft.AspNetCore.Hosting;

namespace TN.TNM.Api.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductDataAccess iProductDataAccess;
        private IHostingEnvironment _hostingEnvironment;

        public ProductController(IProductDataAccess _iProductDataAccess, IHostingEnvironment hostingEnvironment)
        {
            this.iProductDataAccess = _iProductDataAccess;
            _hostingEnvironment = hostingEnvironment;
        }
        /// <summary>
        /// search product
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/searchProduct")]
        [Authorize(Policy = "Member")]
        public SearchProductResult  SearchProduct([FromBody]SearchProductParameter request)
        {
            return this.iProductDataAccess.SearchProduct(request);
        }
        /// <summary>
        /// create product
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/createProduct")]
        [Authorize(Policy = "Member")]
        public CreateProductResult CreateProduct([FromBody]CreateProductParameter request)
        {
            return this.iProductDataAccess.CreateProduct(request);
        }
        /// <summary>
        /// GetProductByID product
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/GetProductByID")]
        [Authorize(Policy = "Member")]
        public GetProductByIDResult GetProductByID([FromBody]GetProductByIDParameter request)
        {
            return this.iProductDataAccess.GetProductByID(request);
        }

        /// <summary>
        /// UpdateProduct
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/UpdateProduct")]
        [Authorize(Policy = "Member")]
        public UpdateProductResult UpdateProduct([FromBody]UpdateProductParameter request)
        {
            return this.iProductDataAccess.UpdateProduct(request);
        }

        /// <summary>
        /// UpdateActiveProduct
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/updateActiveProduct")]
        [Authorize(Policy = "Member")]
        public UpdateActiveProductResult UpdateActiveProduct([FromBody]UpdateActiveProductParameter request)
        {
            return this.iProductDataAccess.UpdateActiveProduct(request);
        }

        /// <summary>
        /// GetProductByVendorID
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/getProductByVendorID")]
        [Authorize(Policy = "Member")]
        public GetProductByVendorIDResult GetProductByVendorID([FromBody]GetProductByVendorIDParameter request)
        {
            return this.iProductDataAccess.GetProductByVendorID(request);
        }

        /// <summary>
        /// GetProductAttributeByProductID
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/getProductAttributeByProductID")]
        [Authorize(Policy = "Member")]
        public GetProductAttributeByProductIDResult GetProductAttributeByProductID([FromBody]GetProductAttributeByProductIDParameter request)
        {
            return this.iProductDataAccess.GetProductAttributeByProductID(request);
        }

        /// <summary>
        /// Get All Product Code
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/getAllProductCode")]
        [Authorize(Policy = "Member")]
        public GetAllProductCodeResult GetAllProductCode([FromBody]GetAllProductCodeParameter request)
        {
            return this.iProductDataAccess.GetAllProductCode(request);
        }

        /// <summary>
        /// Get List Product
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/getListProduct")]
        [Authorize(Policy = "Member")]
        public GetListProductResult GetListProduct([FromBody]GetListProductParameter request)
        {
            return this.iProductDataAccess.GetListProduct(request);
        }

        /// <summary>
        /// Get Masterdata Create Product
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/getMasterdataCreateProduct")]
        [Authorize(Policy = "Member")]
        public GetMasterdataCreateProductResult GetMasterdataCreateProduct([FromBody]GetMasterdataCreateProductParameter request)
        {
            return this.iProductDataAccess.GetMasterdataCreateProduct(request);
        }

        /// <summary>
        /// Add Serial Number
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/dddSerialNumber")]
        [Authorize(Policy = "Member")]
        public AddSerialNumberResult AddSerialNumber([FromBody]AddSerialNumberParameter request)
        {
            return this.iProductDataAccess.AddSerialNumber(request);
        }

        /// <summary>
        /// Get master data vendor dialog
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/getMasterDataVendorDialog")]
        [Authorize(Policy = "Member")]
        public GetMasterDataVendorDialogResult GetMasterDataVendorDialog([FromBody]GetMasterDataVendorDialogParameter request)
        {
            return this.iProductDataAccess.GetMasterDataVendorDialog(request);
        }

        /// <summary>
        /// Get master data vendor dialog
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/downloadTemplateProductService")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateProductServiceResult DownloadTemplateProductService([FromBody]DownloadTemplateProductServiceParameter request)
        {
            return this.iProductDataAccess.DownloadTemplateProductService(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/importProduct")]
        [Authorize(Policy = "Member")]
        public ImportProductResult ImportProduct([FromBody]ImportProductParameter request)
        {
            return this.iProductDataAccess.ImportProduct(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/getMasterDataPriceList")]
        [Authorize(Policy = "Member")]
        public GetMasterDataPriceProductResult GetMasterDataPriceList([FromBody]GetMasterDataPriceProductParameter request)
        {
            return this.iProductDataAccess.GetMasterDataPriceList(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/createOrUpdatePriceProduct")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdatePriceProductResult CreateOrUpdatePriceProduct([FromBody]CreateOrUpdatePriceProductParameter request)
        {
            return this.iProductDataAccess.CreateOrUpdatePriceProduct(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/deletePriceProduct")]
        [Authorize(Policy = "Member")]
        public DeletePriceProductResult DeletePriceProduct([FromBody]DeletePriceProductParameter request)
        {
            return this.iProductDataAccess.DeletePriceProduct(request);
        }

        [HttpPost]
        [Route("api/Product/getDataQuickCreateProduct")]
        [Authorize(Policy = "Member")]
        public GetDataQuickCreateProductResult GetDataQuickCreateProduct([FromBody]GetDataQuickCreateProductParameter request)
        {
            return this.iProductDataAccess.GetDataQuickCreateProduct(request);
        }

        [HttpPost]
        [Route("api/Product/getDataCreateUpdateBOM")]
        [Authorize(Policy = "Member")]
        public GetDataCreateUpdateBOMResult GetDataCreateUpdateBOM([FromBody]GetDataCreateUpdateBOMParameter request)
        {
            return this.iProductDataAccess.GetDataCreateUpdateBOM(request);
        }


        [HttpPost]
        [Route("api/Product/downloadPriceProductTemplate")]
        [Authorize(Policy = "Member")]
        public DownloadPriceProductTemplateResult DownloadPriceProductTemplate([FromBody] DownloadPriceProductTemplateParameter request)
        {
            return this.iProductDataAccess.DownloadPriceProductTemplate(request);
        }

        [HttpPost]
        [Route("api/Product/importPriceProduct")]
        [Authorize(Policy = "Member")]
        public ImportPriceProductResult ImportPriceProduct([FromBody] ImportPriceProductParamter request)
        {
            return this.iProductDataAccess.ImportPriceProduct(request);
        }

        [HttpPost]
        [Route("api/Product/createThuocTinhSanPham")]
        [Authorize(Policy = "Member")]
        public CreateThuocTinhSanPhamResult CreateThuocTinhSanPham([FromBody] CreateThuocTinhSanPhamParameter request)
        {
            return this.iProductDataAccess.CreateThuocTinhSanPham(request);
        }

        //
        [HttpPost]
        [Route("api/Product/deleteThuocTinhSanPham")]
        [Authorize(Policy = "Member")]
        public DeleteThuocTinhSanPhamResult DeleteThuocTinhSanPham([FromBody] DeleteThuocTinhSanPhamParameter request)
        {
            return this.iProductDataAccess.DeleteThuocTinhSanPham(request);
        }

        [HttpGet]
        [Route("api/Product/getListServiceType")]
        [Authorize(Policy = "Member")]
        public Task<GetListServiceTypeResult> GetListServiceType()
        {
            return this.iProductDataAccess.GetListServiceType();
        }

        [HttpPost]
        [Route("api/Product/takeListOption")]
        [Authorize(Policy = "Member")]
        public Task<TakeListOptionResult> TakeListOption([FromBody] TakeListOptionsParameter request)
        {
            return this.iProductDataAccess.TakeListOption(request);
        }

        [HttpPost]
        [Route("api/Product/createProductOption")]
        [Authorize(Policy = "Member")]
        public Task<CreateProductOptionResult> CreateProductOption([FromBody] CreateProductOptionParameter request)
        {
            return this.iProductDataAccess.CreateProductOption(request);
        }

        [HttpPost]
        [Route("api/Product/createOrEditProduct")]
        [Authorize(Policy = "Member")]
        public Task<CreateOrEditProductResult> CreateOrEditProduct([FromBody] CreateOrEditProductParameter request)
        {
            return this.iProductDataAccess.CreateOrEditProduct(request);
        }

        [HttpPost]
        [Route("api/Product/takeProductAndOptionsById")]
        [Authorize(Policy = "Member")]
        public Task<TakeProductAndOptionsByIdResult> TakeProductAndOptionsById([FromBody] TakeProductAndOptionsByIdParameter request)
        {
            return this.iProductDataAccess.TakeProductAndOptionsById(request);
        }

        [HttpGet]
        [Route("api/Product/getMasterDataCreateServicePacket")]
        [Authorize(Policy = "Member")]
        public Task<GetMasterDataCreateServicePacketResult> GetMasterDataCreateServicePacket([FromBody] GetMasterDataCreateServicePacketParameter request)
        {
            return this.iProductDataAccess.GetMasterDataCreateServicePacket(request);
        }

        [HttpPost]
        [Route("api/Product/createOrUpdateServicePacket")]
        [Authorize(Policy = "Member")]
        public Task<CreateOrUpdateServicePacketResult> CreateOrUpdateServicePacket([FromBody] CreateOrUpdateServicePacketParameter request)
        {
            return this.iProductDataAccess.CreateOrUpdateServicePacket(request);
        }

        [HttpPost]
        [Route("api/Product/getListEmployeeByRoleId")]
        [Authorize(Policy = "Member")]
        public GetListEmployeeByRoleIdResult GetListEmployeeByRoleId([FromBody] GetListEmployeeByRoleIdParameter request)
        {
            return this.iProductDataAccess.GetListEmployeeByRoleId(request);
        }

        [HttpPost]
        [Route("api/Product/getListServicePacket")]
        public Task<GetListServicePacketResult> GetListServicePacket([FromBody] GetListServicePacketParameter request)
        {
            return this.iProductDataAccess.GetListServicePacket(request);
        }

        [HttpPost]
        [Route("api/Product/deleteServicePacket")]
        [Authorize(Policy = "Member")]
        public Task<DeleteServicePacketResult> DeleteServicePacket([FromBody] DeleteServicePacketParameter request)
        {
            return this.iProductDataAccess.DeleteServicePacket(request);
        }

        [HttpPost]
        [Route("api/Product/getServicePacketById")]
        [Authorize(Policy = "Member")]
        public Task<GetServicePacketByIdResult> GetServicePacketById([FromBody] GetServicePacketByIdParameter request)
        {
            return this.iProductDataAccess.GetServicePacketById(request);
        }

        [HttpPost]
        [Route("/api/Product/deleteServicePacketMappingOption")]
        [Authorize(Policy = "Member")]
        public DeleteServicePacketMappingOptionResult DeleteServicePacketMappingOption([FromBody] DeleteServicePacketMappingOptionParameter request)
        {
            return this.iProductDataAccess.DeleteServicePacketMappingOption(request);
        }

        [HttpPost]
        [Route("/api/Product/createServicePacketMappingOption")]
        [Authorize(Policy = "Member")]
        public CreateServicePacketMappingOptionResult CreateServicePacketMappingOption([FromBody] CreateServicePacketMappingOptionParameter request)
        {
            return this.iProductDataAccess.CreateServicePacketMappingOption(request);
        }

        [HttpPost]
        [Route("/api/Product/changeOrderServicePack")]
        [Authorize(Policy = "Member")]
        public ChangeOrderServicePackResult ChangeOrderServicePack([FromBody] ChangeOrderServicePackParameter request)
        {
            return this.iProductDataAccess.ChangeOrderServicePack(request);
        }

        [HttpPost]
        [Route("api/Product/uploadServicePacketImage")]
        [Authorize(Policy = "Member")]
        public UploadFileResponse UploadProductImage(UploadFileRequest request)
        {
            try
            {
                if (request.FileList != null && request.FileList.Count > 0)
                {
                    string folderName = "ServicePacketImage";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }

                    foreach (IFormFile item in request.FileList)
                    {
                        if (item.Length > 0)
                        {
                            string fileName = item.FileName.Trim();
                            string fullPath = Path.Combine(newPath, fileName);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                item.CopyTo(stream);
                            }
                        }
                    }

                    return new UploadFileResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = CommonMessage.FileUpload.UPLOAD_SUCCESS,
                    };
                }
                else
                {
                    return new UploadFileResponse()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.FileUpload.NO_FILE
                    };
                }
            }
            catch (Exception ex)
            {
                return new UploadFileResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }
    }
}