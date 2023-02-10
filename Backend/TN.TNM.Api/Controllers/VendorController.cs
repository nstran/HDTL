using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Vendor;
using TN.TNM.BusinessLogic.Messages.Requests.Vendor;
using TN.TNM.BusinessLogic.Messages.Responses.Vendor;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;
using TN.TNM.DataAccess.Messages.Results.Vendor;

namespace TN.TNM.Api.Controllers
{
    public class VendorController : Controller
    {
        private readonly IVendor iVendor;
        private readonly IVendorDataAsccess iVendorDataAccess;
        public VendorController(IVendor _iVendor, IVendorDataAsccess _iVendorDataAccess)
        {
            this.iVendor = _iVendor;
            this.iVendorDataAccess = _iVendorDataAccess;
        }

        /// <summary>
        /// Create new Vendor
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vendor/createVendor")]
        [Authorize(Policy = "Member")]
        public CreateVendorResult CreateVendor([FromBody]CreateVendorParameter request)
        {
            return iVendorDataAccess.CreateVendor(request);
        }

        /// <summary>
        /// Search Vendor
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Vendor</returns>
        [HttpPost]
        [Route("api/vendor/searchVendor")]
        [Authorize(Policy = "Member")]
        public SearchVendorResult SearchVendor([FromBody]SearchVendorParameter request)
        {
            return iVendorDataAccess.SearchVendor(request);
        }

        /// <summary>
        /// Get Vendor by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>Get Vendor</returns>
        [HttpPost]
        [Route("api/vendor/getVendorById")]
        [Authorize(Policy = "Member")]
        public GetVendorByIdResult GetVendorById([FromBody]GetVendorByIdParameter request)
        {
            return iVendorDataAccess.GetVendorById(request);
        }

        /// <summary>
        /// Get Vendor by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>Get Vendor</returns>
        [HttpPost]
        [Route("api/vendor/getAllVendorCode")]
        [Authorize(Policy = "Member")]
        public GetAllVendorCodeResult GetAllVendorCode([FromBody]GetAllVendorCodeParameter request)
        {
            return iVendorDataAccess.GetAllVendorCode(request);
        }

        /// <summary>
        /// Update Vendor
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>Get Vendor</returns>
        [HttpPost]
        [Route("api/vendor/updateVendorById")]
        [Authorize(Policy = "Member")]
        public UpdateVendorByIdResult UpdateVendorById([FromBody]UpdateVendorByIdParameter request)
        {
            return iVendorDataAccess.UpdateVendorById(request);
        }

        /// <summary>
        /// Quick Create Vendor
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>Get Vendor</returns>
        [HttpPost]
        [Route("api/vendor/quickCreateVendor")]
        [Authorize(Policy = "Member")]
        public QuickCreateVendorResult QuickCreateVendor([FromBody]QuickCreateVendorParameter request)
        {
            return iVendorDataAccess.QuickCreateVendor(request);
        }

        /// <summary>
        /// Create Vendor Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>Get Vendor</returns>
        [HttpPost]
        [Route("api/vendor/createVendorOrder")]
        [Authorize(Policy = "Member")]
        public CreateVendorOrderResult CreateVendorOrder([FromBody]CreateVendorOrderParameter request)
        {
            return iVendorDataAccess.CreateVendorOrder(request);
        }

        /// <summary>
        /// Create Vendor Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>Get Vendor</returns>
        [HttpPost]
        [Route("api/vendor/searchVendorOrder")]
        [Authorize(Policy = "Member")]
        public SearchVendorOrderResult SearchVendorOrder([FromBody]SearchVendorOrderParameter request)
        {
            return iVendorDataAccess.SearchVendorOrder(request);
        }

        /// <summary>
        /// GetAllVendor
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vendor/getAllVendor")]
        [Authorize(Policy = "Member")]
        public GetAllVendorResult GetAllVendor([FromBody]GetAllVendorParameter request)
        {
            return iVendorDataAccess.GetAllVendor(request);
        }

        /// <summary>
        /// Get vendor order by id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vendor/getVendorOrderById")]
        [Authorize(Policy = "Member")]
        public GetVendorOrderByIdResult GetVendorOrderById([FromBody]GetVendorOrderByIdParameter request)
        {
            return iVendorDataAccess.GetVendorOrderById(request);
        }

        /// <summary>
        /// Update vendor order by id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vendor/updateVendorOrderById")]
        [Authorize(Policy = "Member")]
        public UpdateVendorOrderByIdResult UpdateVendorOrderById([FromBody]UpdateVendorOrderByIdParameter request)
        {
            return iVendorDataAccess.UpdateVendorOrderById(request);
        }

        /// <summary>
        /// Update Active Vendor
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>Get Vendor</returns>
        [HttpPost]
        [Route("api/vendor/updateActiveVendor")]
        [Authorize(Policy = "Member")]
        public UpdateActiveVendorResult UpdateActiveVendor([FromBody]UpdateActiveVendorParameter request)
        {
            return iVendorDataAccess.UpdateActiveVendor(request);
        }

        /// <summary>
        /// QuickCreateVendorMasterdata
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>QuickCreateVendorMasterdata</returns>
        [HttpPost]
        [Route("api/vendor/quickCreateVendorMasterdata")]
        [Authorize(Policy = "Member")]
        public QuickCreateVendorMasterdataResult QuickCreateVendorMasterdata([FromBody]QuickCreateVendorMasterdataParameter request)
        {
            return iVendorDataAccess.QuickCreateVendorMasterdata(request);
        }

        [HttpPost]
        [Route("api/vendor/getDataCreateVendor")]
        [Authorize(Policy = "Member")]
        public GetDataCreateVendorResult GetDataCreateVendor([FromBody]GetDataCreateVendorParameter request)
        {
            return iVendorDataAccess.GetDataCreateVendor(request);
        }

        [HttpPost]
        [Route("api/vendor/getDataSearchVendor")]
        [Authorize(Policy = "Member")]
        public GetDataSearchVendorResult GetDataSearchVendor([FromBody]GetDataSearchVendorParameter request)
        {
            return iVendorDataAccess.GetDataSearchVendor(request);
        }

        [HttpPost]
        [Route("api/vendor/getDataEditVendor")]
        [Authorize(Policy = "Member")]
        public GetDataEditVendorResult GetDataEditVendor([FromBody]GetDataEditVendorParameter request)
        {
            return iVendorDataAccess.GetDataEditVendor(request);
        }

        [HttpPost]
        [Route("api/vendor/createVendorContact")]
        [Authorize(Policy = "Member")]
        public CreateVendorContactResult CreateVendorContact([FromBody]CreateVendorContactParameter request)
        {
            return iVendorDataAccess.CreateVendorContact(request);
        }

        [HttpPost]
        [Route("api/vendor/getDataCreateVendorOrder")]
        [Authorize(Policy = "Member")]
        public GetDataCreateVendorOrderResult GetDataCreateVendorOrder([FromBody]GetDataCreateVendorOrderParameter request)
        {
            return iVendorDataAccess.GetDataCreateVendorOrder(request);
        }

        [HttpPost]
        [Route("api/vendor/getDataAddVendorOrderDetail")]
        [Authorize(Policy = "Member")]
        public GetDataAddVendorOrderDetailResult GetDataAddVendorOrderDetail([FromBody]GetDataAddVendorOrderDetailParameter request)
        {
            return iVendorDataAccess.GetDataAddVendorOrderDetail(request);
        }

        [HttpPost]
        [Route("api/vendor/getMasterDataSearchVendorOrder")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchVendorOrderResult GetGetMasterDataSearchVendorOrder([FromBody]GetMasterDataSearchVendorOrderParameter request)
        {
            return iVendorDataAccess.GetGetMasterDataSearchVendorOrder(request);
        }

        [HttpPost]
        [Route("api/vendor/getDataEditVendorOrder")]
        [Authorize(Policy = "Member")]
        public GetDataEditVendorOrderResult GetDataEditVendorOrder([FromBody]GetDataEditVendorOrderParameter request)
        {
            return iVendorDataAccess.GetDataEditVendorOrder(request);
        }

        [HttpPost]
        [Route("api/vendor/getDataSearchVendorQuote")]
        [Authorize(Policy = "Member")]
        public GetDataSearchVendorQuoteResult GetDataSearchVendorQuote([FromBody]GetDataSearchVendorQuoteParameter request)
        {
            return iVendorDataAccess.GetDataSearchVendorQuote(request);
        }

        [HttpPost]
        [Route("api/vendor/createVendorQuote")]
        [Authorize(Policy = "Member")]
        public CreateVendorQuoteResult CreateVendorQuote([FromBody]ListVendorQuoteParameter request)
        {
            return iVendorDataAccess.CreateVendorQuote(request);
        }

        [HttpPost]
        [Route("api/vendor/searchVendorProductPrice")]
        [Authorize(Policy = "Member")]
        public SearchVendorProductPriceResult SearchVendorProductPrice([FromBody]SearchVendorProductPriceParameter request)
        {
            return iVendorDataAccess.SearchVendorProductPrice(request);
        }

        [HttpPost]
        [Route("api/vendor/createVendorProductPrice")]
        [Authorize(Policy = "Member")]
        public CreateVendorProductPriceResult CreateVendorProductPrice([FromBody]CreateVendorProductPriceParameter request)
        {
            return iVendorDataAccess.CreateVendorProductPrice(request);
        }

        [HttpPost]
        [Route("api/vendor/deleteVendorProductPrice")]
        [Authorize(Policy = "Member")]
        public DeleteVendorProductPriceResult DeleteVendorProductPrice([FromBody]DeleteVendorProductPriceParameter request)
        {
            return iVendorDataAccess.DeleteVendorProductPrice(request);
        }

        [HttpPost]
        [Route("api/vendor/downloadTemplateVendorProductPrice")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateVendorProductPriceResult DownloadTemplateVendorProductPrice([FromBody]DownloadTemplateVendorProductPriceParameter request)
        {
            return iVendorDataAccess.DownloadTemplateVendorProductPrice(request);
        }

        [HttpPost]
        [Route("api/vendor/importVendorProductPrice")]
        [Authorize(Policy = "Member")]
        public ImportProductVendorPriceResult ImportVendorProductPrice([FromBody]ImportVendorProductPriceParameter request)
        {
            return iVendorDataAccess.ImportProductVendorPrice(request); 
        }

        [HttpPost]
        [Route("api/vendor/getMasterDataCreateSuggestedSupplierQuote")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateSuggestedSupplierQuoteResult GetMasterDataCreateSuggestedSupplierQuote([FromBody]GetMasterDataCreateSuggestedSupplierQuoteParameter request)
        {
            return iVendorDataAccess.GetMasterDataCreateSuggestedSupplierQuote(request);
        }

        [HttpPost]
        [Route("api/vendor/createOrUpdateSuggestedSupplierQuote")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateSuggestedSupplierQuoteResult CreateOrUpdateSuggestedSupplierQuote([FromBody]CreateOrUpdateSuggestedSupplierQuoteParameter request)
        {
            return iVendorDataAccess.CreateOrUpdateSuggestedSupplierQuote(request);
        }

        [HttpPost]
        [Route("api/vendor/deleteSuggestedSupplierQuoteRequest")]
        [Authorize(Policy = "Member")]
        public DeleteSuggestedSupplierQuoteRequestResult DeleteSuggestedSupplierQuoteRequest([FromBody]DeleteSugestedSupplierQuoteRequestParameter request)
        {
            return iVendorDataAccess.DeleteSuggestedSupplierQuoteRequest(request);
        }

        [HttpPost]
        [Route("api/vendor/changeStatusVendorQuote")]
        [Authorize(Policy = "Member")]
        public ChangeStatusVendorQuoteResult ChangeStatusVendorQuote([FromBody]ChangeStatusVendorQuoteParameter request)
        {
            return iVendorDataAccess.ChangeStatusVendorQuote(request);
        }
        //
        [HttpPost]
        [Route("api/vendor/getDataAddEditCostVendorOrder")]
        [Authorize(Policy = "Member")]
        public GetDataAddEditCostVendorOrderResult GetDataAddEditCostVendorOrder([FromBody]GetDataAddEditCostVendorOrderParameter request)
        {
            return iVendorDataAccess.GetDataAddEditCostVendorOrder(request);
        }

        [HttpPost]
        [Route("api/vendor/sendEmailVendorQuote")]
        [Authorize(Policy = "Member")]
        public SendEmailVendorQuoteResult SendEmailVendorQuote([FromForm]SendMailVendorQuoteParameter request)
        {
            return iVendorDataAccess.SendEmailVendorQuote(request);
        }

        
        [HttpPost]
        [Route("api/vendor/removeVendorOrder")]
        [Authorize(Policy = "Member")]
        public RemoveVendorOrderResult RemoveVendorOrder([FromBody]RemoveVendorOrderParameter request)
        {
            return iVendorDataAccess.RemoveVendorOrder(request);
        }

        //
        [HttpPost]
        [Route("api/vendor/cancelVendorOrder")]
        [Authorize(Policy = "Member")]
        public CancelVendorOrderResult CancelVendorOrder([FromBody]CancelVendorOrderParameter request)
        {
            return iVendorDataAccess.CancelVendorOrder(request);
        }

        //
        [HttpPost]
        [Route("api/vendor/draftVendorOrder")]
        [Authorize(Policy = "Member")]
        public DraftVendorOrderResult DraftVendorOrder([FromBody]DraftVendorOrderParameter request)
        {
            return iVendorDataAccess.DraftVendorOrder(request);
        }

        //
        [HttpPost]
        [Route("api/vendor/getMasterDataVendorOrderReport")]
        [Authorize(Policy = "Member")]
        public GetMasterDataVendorOrderReportResult GetMasterDataVendorOrderReport([FromBody]GetMasterDataVendorOrderReportParameter request)
        {
            return iVendorDataAccess.GetMasterDataVendorOrderReport(request);
        }

        //
        [HttpPost]
        [Route("api/vendor/searchVendorOrderReport")]
        [Authorize(Policy = "Member")]
        public SearchVendorOrderReportResult SearchVendorOrderReport([FromBody]SearchVendorOrderReportParameter request)
        {
            return iVendorDataAccess.SearchVendorOrderReport(request);
        }

        //
        [HttpPost]
        [Route("api/vendor/approvalOrRejectVendorOrder")]
        [Authorize(Policy = "Member")]
        public ApprovalOrRejectVendorOrderResult ApprovalOrRejectVendorOrder(
            [FromBody]ApprovalOrRejectVendorOrderParameter request)
        {
            return iVendorDataAccess.ApprovalOrRejectVendorOrder(request);
        }

        [HttpPost]
        [Route("api/vendor/getQuantityApproval")]
        [Authorize(Policy = "Member")]
        public GetQuantityApprovalResult GetQuantityApproval([FromBody] GetQuantityApprovalParameter request)
        {
            return iVendorDataAccess.GetQuantityApproval(request);
        }
        
        
        [HttpPost]
        [Route("api/vendor/getDashboardVendor")]
        [Authorize(Policy = "Member")]
        public GetDashboardVendorResult GetDashboardVendor([FromBody] GetDashboardVendorParamter request)
        {
            return iVendorDataAccess.GetDashboardVendor(request);
        }
        
        [HttpPost]
        [Route("api/vendor/getProductCategoryGroupByLevel")]
        [Authorize(Policy = "Member")]
        public GetProductCategoryGroupByLevelResult GetProductCategoryGroupByLevel([FromBody] GetProductCategoryGroupByLevelParameter request)
        {
            return iVendorDataAccess.GetProductCategoryGroupByLevel(request);
        }
        
        [HttpPost]
        [Route("api/vendor/getDataBarchartFollowMonth")]
        [Authorize(Policy = "Member")]
        public GetDataBarchartFollowMonthResult GetDataBarchartFollowMonth([FromBody] GetDataBarchartFollowMonthParameter request)
        {
            return iVendorDataAccess.GetDataBarchartFollowMonth(request);
        }
    }
}