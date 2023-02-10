using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.PayableInvoice;
using TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice;
using TN.TNM.BusinessLogic.Messages.Responses.PayableInvoice;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;
using TN.TNM.DataAccess.Messages.Results.PayableInvoice;

namespace TN.TNM.Api.Controllers
{
    public class PayableInvoiceController
    {
        private readonly IPayableInvoiceDataAccess _iPayableInvoiceDataAccess;
        public PayableInvoiceController(IPayableInvoiceDataAccess iPayableInvoiceDataAccess)
        {
            _iPayableInvoiceDataAccess = iPayableInvoiceDataAccess;
        }

        [HttpPost]
        [Route("api/payableInvoice/create")]
        [Authorize(Policy = "Member")]
        public CreatePayableInvoiceResult CreatePayableInvoice([FromBody]CreatePayableInvoiceParameter request)
        {
            return this._iPayableInvoiceDataAccess.CreatePayableInvoice(request);
        }
        
        [HttpPost]
        [Route("api/payableInvoice/getPayableInvoiceById")]
        [Authorize(Policy = "Member")]
        public GetPayableInvoiceByIdResult GetPayableInvoiceById([FromBody] GetPayableInvoiceByIdParameter request)
        {
            return this._iPayableInvoiceDataAccess.GetPayableInvoiceById(request);
        }
        
        [HttpPost]
        [Route("api/payableInvoice/searchPayableInvoice")]
        [Authorize(Policy = "Member")]
        public SearchPayableInvoiceResult SearchPayableInvoice([FromBody] SearchPayableInvoiceParameter request)
        {
            return this._iPayableInvoiceDataAccess.SearchPayableInvoice(request);
        }
        
        [HttpPost]
        [Route("api/payableInvoice/createBankPayableInvoice")]
        [Authorize(Policy = "Member")]
        public CreateBankPayableInvoiceResult CreateBankPayableInvoice([FromBody] CreateBankPayableInvoiceParameter request)
        {
            return this._iPayableInvoiceDataAccess.CreateBankPayableInvoice(request);
        }
        
        [HttpPost]
        [Route("api/payableInvoice/searchBankPayableInvoice")]
        [Authorize(Policy = "Member")]
        public SearchBankPayableInvoiceResult SearchBankPayableInvoice([FromBody]SearchBankPayableInvoiceParameter request)
        {
            return this._iPayableInvoiceDataAccess.SearchBankPayableInvoice(request);
        }
        
        [HttpPost]
        [Route("api/payableInvoice/getBankPayableInvoiceById")]
        [Authorize(Policy = "Member")]
        public GetBankPayableInvoiceByIdResult GetBankPayableInvoiceById([FromBody] GetBankPayableInvoiceByIdParameter request)
        {
            return this._iPayableInvoiceDataAccess.GetBankPayableInvoiceById(request);
        }
        
        [HttpPost]
        [Route("api/payableInvoice/exportBankPayableInvoice")]
        [Authorize(Policy = "Member")]
        public ExportBankPayableInvoiceResult ExportBankPayableInvoice([FromBody] ExportBankPayableInvoiceParameter request)
        {
            return this._iPayableInvoiceDataAccess.ExportBankPayableInvoice(request);
        }
        
        [HttpPost]
        [Route("api/payableInvoice/exportPayableInvoice")]
        [Authorize(Policy = "Member")]
        public ExportPayableInvoiceResult ExportPayableInvoice([FromBody] ExportPayableInvoiceParameter request)
        {
            return this._iPayableInvoiceDataAccess.ExportPayableInvoice(request);
        }
        
        [HttpPost]
        [Route("api/payableInvoice/searchBankBookPayableInvoice")]
        [Authorize(Policy = "Member")]
        public SearchBankBookPayableInvoiceResult SearchBankBookPayableInvoice([FromBody]SearchBankBookPayableInvoiceParameter request)
        {
            return this._iPayableInvoiceDataAccess.SearchBankBookPayableInvoice(request);
        }
        
        [HttpPost]
        [Route("api/payableInvoice/searchCashBookPayableInvoice")]
        [Authorize(Policy = "Member")]
        public SearchCashBookPayableInvoiceResult SearchCashBookPayableInvoice([FromBody]SearchCashBookPayableInvoiceParameter request)
        {
            return this._iPayableInvoiceDataAccess.SearchCashBookPayableInvoice(request);
        }
        
        [HttpPost]
        [Route("api/payableInvoice/getMasterDataPayableInvoice")]
        [Authorize(Policy = "Member")]
        public GetMasterDataPayableInvoiceResult GetMasterDataPayableInvoice ([FromBody]GetMasterDataPayableInvoiceParameter request)
        {
            return this._iPayableInvoiceDataAccess.GetMasterDataPayableInvoice(request);
        }
        
        [HttpPost]
        [Route("api/payableInvoice/getMasterDataPayableInvoiceSearch")]
        [Authorize(Policy = "Member")]
        public GetMasterDataPayableInvoiceSearchResult GetMasterDataPayableInvoiceSearch(
            [FromBody]GetMasterDataPayableInvoiceSearchParameter request)
        {
            return this._iPayableInvoiceDataAccess.GetMasterDataPayableInvoiceSearch(request);
        }

        [HttpPost]
        [Route("api/payableInvoice/getMasterDataBankPayableInvoice")]
        [Authorize(Policy = "Member")]
        public GetMasterDataBankPayableInvoiceResult GetMasterDataBankPayableInvoice(
            [FromBody] GetMasterDataBankPayableInvoiceParameter request)
        {
            return this._iPayableInvoiceDataAccess.GetMasterDataBankPayableInvoice(request);
        }

        [HttpPost]
        [Route("api/payableInvoice/getMasterDataSearchBankPayableInvoice")]
        [Authorize(Policy ="Member")]
        public GetMasterDataSearchBankPayableInvoiceResult GetMasterDataSearchBankPayableInvoice(
            [FromBody] GetMasterDataSearchBankPayableInvoiceParameter request)
        {
            return this._iPayableInvoiceDataAccess.GetMasterDataSearchBankPayableInvoice(request);
        }
    }
}
