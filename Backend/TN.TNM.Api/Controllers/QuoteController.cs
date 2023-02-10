using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Quote;
using TN.TNM.BusinessLogic.Messages.Requests.Quote;
using TN.TNM.BusinessLogic.Messages.Responses.Quote;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Quote;
using TN.TNM.DataAccess.Messages.Results.Quote;

namespace TN.TNM.Api.Controllers
{
    public class QuoteController : ControllerBase
    {
        private readonly IQuoteDataAccess _iQuoteDataAccess;
        private readonly IQuote _iQuote;
        public QuoteController(IQuote iQuote, IQuoteDataAccess iQuoteDataAccess)
        {
            this._iQuote = iQuote;
            _iQuoteDataAccess = iQuoteDataAccess;
        }
        /// <summary>
        /// Create Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/createQuote")]
        [Authorize(Policy = "Member")]
        public CreateQuoteResult CreateQuote([FromBody]CreateQuoteParameter request)
        {
            return this._iQuoteDataAccess.CreateQuote(request);
        }

        [HttpPost]
        [Route("api/quote/uploadQuoteDocument")]
        [Authorize(Policy = "Member")]
        public UploadOuoteDocumentResult UploadQuoteDocument([FromBody] UploadOuoteDocumentParameter request)
        {
            return this._iQuoteDataAccess.UploadOuoteDocument(request);
        }

        /// <summary>
        /// Get All Quote 
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getAllQuote")]
        [Authorize(Policy = "Member")]
        public GetAllQuoteResult GetAllQuote([FromBody]GetAllQuoteParameter request)
        {
            return this._iQuoteDataAccess.GetAllQuote(request);
        }

        /// <summary>
        /// Get Top3 Quotes Overdue 
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getTop3QuotesOverdue")]
        [Authorize(Policy = "Member")]
        public GetTop3QuotesOverdueResult GetTop3QuotesOverdue([FromBody]GetTop3QuotesOverdueParameter request)
        {
            return this._iQuoteDataAccess.GetTop3QuotesOverdue(request);
        }

        /// <summary>
        /// Get Top3 Week Quotes Overdue 
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getTop3WeekQuotesOverdue")]
        [Authorize(Policy = "Member")]
        public GetTop3WeekQuotesOverdueResult GetTop3WeekQuotesOverdue([FromBody]GetTop3WeekQuotesOverdueParameter request)
        {
            return this._iQuoteDataAccess.GetTop3WeekQuotesOverdue(request);
        }

        /// <summary>
        /// Get Top3 Week Quotes Overdue 
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getTop3PotentialCustomers")]
        [Authorize(Policy = "Member")]
        public GetTop3PotentialCustomersResult GetTop3PotentialCustomers([FromBody]GetTop3PotentialCustomersParameter request)
        {
            return this._iQuoteDataAccess.GetTop3PotentialCustomers(request);
        }

        /// <summary>
        /// update Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/updateQuote")]
        [Authorize(Policy = "Member")]
        public UpdateQuoteResult UpdateQuote([FromBody]UpdateQuoteParameter request)
        {
            return this._iQuoteDataAccess.UpdateQuote(request);
        }
        /// <summary>
        /// get Quote ByID
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getQuoteByID")]
        [Authorize(Policy = "Member")]
        public GetQuoteByIDResult GetQuoteByID([FromBody]GetQuoteByIDParameter request)
        {
            return this._iQuoteDataAccess.GetQuoteByID(request);
        }

        /// <summary>
        /// Get Total Amount Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getTotalAmountQuote")]
        [Authorize(Policy = "Member")]
        public GetTotalAmountQuoteResult GetTotalAmountQuote([FromBody]GetTotalAmountQuoteParameter request)
        {
            return this._iQuoteDataAccess.GetTotalAmountQuote(request);
        }
        /// <summary>
        /// Get DashBoard Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getDashBoardQuote")]
        [Authorize(Policy = "Member")]
        public GetDashBoardQuoteResult GetDashBoardQuote([FromBody]GetDashBoardQuoteParameter request)
        {
            return this._iQuoteDataAccess.GetDashBoardQuote(request);
        }

        /// <summary>
        /// update active Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/updateActiveQuote")]
        [Authorize(Policy = "Member")]
        public UpdateActiveQuoteResult UpdateActiveQuote([FromBody]UpdateActiveQuoteParameter request)
        {
            return this._iQuoteDataAccess.UpdateActiveQuote(request);
        }

        /// <summary>
        /// GetDataQuoteToPieChart
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getDataQuoteToPieChart")]
        [Authorize(Policy = "Member")]
        public GetDataQuoteToPieChartResult GetDataQuoteToPieChart([FromBody]GetDataQuoteToPieChartParameter request)
        {
            return this._iQuoteDataAccess.GetDataQuoteToPieChart(request);
        }

        /// <summary>
        /// Search Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/searchQuote")]
        [Authorize(Policy = "Member")]
        public SearchQuoteResult SearchQuote([FromBody]SearchQuoteParameter request)
        {
            return this._iQuoteDataAccess.SearchQuote(request);
        }


        /// <summary>
        /// Get Data Create/Update Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getDataCreateUpdateQuote")]
        [Authorize(Policy = "Member")]
        public GetDataCreateUpdateQuoteResult GetDataCreateUpdateQuote([FromBody]GetDataCreateUpdateQuoteParameter request)
        {
            return this._iQuoteDataAccess.GetDataCreateUpdateQuote(request);
        }


        /// <summary>
        /// Get Data Quote Add Edit Product Dialog
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getDataQuoteAddEditProductDialog")]
        [Authorize(Policy = "Member")]
        public GetDataQuoteAddEditProductDialogResult GetDataQuoteAddEditProductDialog([FromBody]GetDataQuoteAddEditProductDialogParameter request)
        {
            return this._iQuoteDataAccess.GetDataQuoteAddEditProductDialog(request);
        }

        //
        /// <summary>
        /// Get Vendor By ProductId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getVendorByProductId")]
        [Authorize(Policy = "Member")]
        public GetVendorByProductIdResult GetVendorByProductId([FromBody]GetVendorByProductIdParameter request)
        {
            return this._iQuoteDataAccess.GetVendorByProductId(request);
        }

        [HttpPost]
        [Route("api/quote/getDataExportExcelQuouter")]
        [Authorize(Policy ="Member")]
        public GetDataExportExcelQuoteResult GetDataExportExcelQuote([FromBody]GetDataExportExcelQuoteParameter request)
        {
            return this._iQuoteDataAccess.GetDataExportExcelQuote(request);
        }

        [HttpPost]
        [Route("api/quote/getEmployeeSale")]
        [Authorize(Policy = "Member")]
        public GetEmployeeSaleResult GetEmployeeSale([FromBody]GetEmployeeSaleParameter request)
        {
            return this._iQuoteDataAccess.GetEmployeeSale(request);
        }

        [HttpPost]
        [Route("api/quote/downloadTemplateProduct")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateProductResult DownloadTemplateProduct([FromBody]DownloadTemplateProductParameter request)
        {
             return this._iQuoteDataAccess.DownloadTemplateProduct(request);
        }

        [HttpPost]
        [Route("api/quote/createCost")]
        [Authorize(Policy = "Member")]
        public CreateCostResult CreateCost([FromBody]CreateCostParameter request)
        {
            return _iQuoteDataAccess.CreateCost(request);
        }

        /// <summary>
        /// get master data create cost
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getMasterDataCreateCost")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateCostResult GetMasterDataCreateCost([FromBody]GetMasterDataCreateCostParameter request)
        {
            return this._iQuoteDataAccess.GetMasterDataCreateCost(request);
        }

        /// <summary>
        /// UpdateCost
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/updateCost")]
        [Authorize(Policy = "Member")]
        public UpdateCostResult UpdateCost([FromBody]UpdateCostParameter request)
        {
            return this._iQuoteDataAccess.UpdateCost(request);
        }

        [HttpPost]
        [Route("api/quote/deleteCost")]
        [Authorize(Policy = "Member")]
        public DeleteCostResult DeleteCost([FromBody] DeleteCostParameter request)
        {
            return this._iQuoteDataAccess.DeleteCost(request);
        }

        /// <summary>
        /// UpdateCost
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/updateStatusQuote")]
        [Authorize(Policy = "Member")]
        public UpdateQuoteResult UpdateStatusQuote([FromBody]GetQuoteByIDParameter request)
        {
            return this._iQuoteDataAccess.UpdateStatusQuote(request);
        }

        /// <summary>
        /// Search Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/searchQuoteApproval")]
        [Authorize(Policy = "Member")]
        public SearchQuoteResult SearchQuoteAprroval([FromBody]SearchQuoteParameter request)
        {
            return this._iQuoteDataAccess.SearchQuoteAprroval(request);
        }

        /// <summary>
        /// UpdateCost
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/approvalOrRejectQuote")]
        [Authorize(Policy = "Member")]
        public UpdateQuoteResult ApprovalOrRejectQuote([FromBody]ApprovalOrRejectQuoteParameter request)
        {
            return this._iQuoteDataAccess.ApprovalOrRejectQuote(request);
        }

        /// <summary>
        /// UpdateCost
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/sendEmailCustomerQuote")]
        [Authorize(Policy = "Member")]
        public UpdateQuoteResult SendEmailCustomerQuote([FromForm]SendEmailCustomerQuoteParameter request)
        {
            return this._iQuoteDataAccess.SendEmailCustomerQuote(request);
        }

        //
        [HttpPost]
        [Route("api/quote/getMasterDataCreateQuote")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateQuoteResult GetMasterDataCreateQuote([FromBody]GetMasterDataCreateQuoteParameter request)
        {
            return this._iQuoteDataAccess.GetMasterDataCreateQuote(request);
        }

        //
        [HttpPost]
        [Route("api/quote/getEmployeeByPersonInCharge")]
        [Authorize(Policy = "Member")]
        public GetEmployeeByPersonInChargeResult GetEmployeeByPersonInCharge([FromBody]GetEmployeeByPersonInChargeParameter request)
        {
            return this._iQuoteDataAccess.GetEmployeeByPersonInCharge(request);
        }

        //
        [HttpPost]
        [Route("api/quote/getMasterDataUpdateQuote")]
        [Authorize(Policy = "Member")]
        public GetMasterDataUpdateQuoteResult GetMasterDataUpdateQuote([FromBody]GetMasterDataUpdateQuoteParameter request)
        {
            return this._iQuoteDataAccess.GetMasterDataUpdateQuote(request);
        }

        //
        [HttpPost]
        [Route("api/quote/createScope")]
        [Authorize(Policy = "Member")]
        public CreateQuoteScopeResult CreateScope([FromBody] CreateQuoteScopeParameter request)
        {
            return this._iQuoteDataAccess.CreateScope(request);
        }

        [HttpPost]
        [Route("api/quote/deleteScope")]
        [Authorize(Policy = "Member")]
        public DeleteQuoteScopeResult DeleteScope([FromBody] DeleteQuoteScopeParameter request)
        {
            return this._iQuoteDataAccess.DeleteScope(request);
        }

        //
        [HttpPost]
        [Route("api/quote/getMasterDataSearchQuote")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchQuoteResult GetMasterDataSearchQuote([FromBody] GetMasterDataSearchQuoteParameter request)
        {
            return this._iQuoteDataAccess.GetMasterDataSearchQuote(request);
        }
        [HttpPost]
        [Route("api/quote/getVendorByCostId")]
        [Authorize(Policy = "Member")]
        public GetVendorByCostIdResult GetVendorByCostId([FromBody] GetVendorByCostIdParameter request)
        {
            return this._iQuoteDataAccess.GetVendorByCostId(request);
        }

    }
}