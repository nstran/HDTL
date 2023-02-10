using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.ReceiptInvoice;
using TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice;
using TN.TNM.BusinessLogic.Messages.Responses.ReceiptInvoice;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;
using TN.TNM.DataAccess.Messages.Results.ReceiptInvoice;

namespace TN.TNM.Api.Controllers
{
    public class ReceiptInvoiceController
    {
        private readonly IReceiptInvoiceDataAccess _iReceiptInvoiceDataAccess;
        public ReceiptInvoiceController(IReceiptInvoiceDataAccess iReceiptInvoiceDataAccess)
        {
            _iReceiptInvoiceDataAccess = iReceiptInvoiceDataAccess;
        }

        [HttpPost]
        [Route("api/receiptInvoice/create")]
        [Authorize(Policy = "Member")]
        public CreateReceiptInvoiceResult CreateReceiptInvoice([FromBody]CreateReceiptInvoiceParameter request)
        {
            return this._iReceiptInvoiceDataAccess.CreateReceiptInvoice(request);
        }

        [HttpPost]
        [Route("api/receiptInvoice/confirm")]
        [Authorize(Policy = "Member")]
        public ConfirmPaymentResult ConfirmPayment([FromBody] ConfirmPaymentParameter request)
        {
            return this._iReceiptInvoiceDataAccess.ConfirmPayment(request);
        }

        [HttpPost]
        [Route("api/receiptInvoice/getReceiptInvoiceById")]
        [Authorize(Policy = "Member")]
        public GetReceiptInvoiceByIdResult GetReceiptInvoiceById([FromBody] GetReceiptInvoiceByIdParameter request)
        {
            return this._iReceiptInvoiceDataAccess.GetReceiptInvoiceById(request);
        }

        [HttpPost]
        [Route("api/receiptInvoice/createBankReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public CreateBankReceiptInvoiceResult CreateBankReceiptInvoice([FromBody] CreateBankReceiptInvoiceParameter request)
        {
            return this._iReceiptInvoiceDataAccess.CreateBankReceiptInvoice(request);
        }

        [HttpPost]
        [Route("api/receiptInvoice/searchReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public SearchReceiptInvoiceResult SearchReceiptInvoice([FromBody] SearchReceiptInvoiceParameter request)
        {
            return this._iReceiptInvoiceDataAccess.SearchReceiptInvoice(request);
        }

        [HttpPost]
        [Route("api/receiptInvoice/searchBankReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public SearchBankReceiptInvoiceResult SearchBankReceiptInvoice([FromBody]SearchBankReceiptInvoiceParameter request)
        {
            return this._iReceiptInvoiceDataAccess.SearchBankReceiptInvoice(request);
        }

        [HttpPost]
        [Route("api/receiptInvoice/getBankReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public GetBankReceiptInvoiceByIdResult GetBankReceiptInvoiceById([FromBody]GetBankReceiptInvoiceByIdParameter request)
        {
            return this._iReceiptInvoiceDataAccess.GetBankReceiptInvoiceById(request);
        }

        [HttpPost]
        [Route("api/receiptInvoice/exportPdfReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public ExportReceiptinvoiceResult ExportPdfReceiptInvoice([FromBody] ExportReceiptInvoiceParameter request)
        {
            return this._iReceiptInvoiceDataAccess.ExportPdfReceiptInvoice(request);
        }

        [HttpPost]
        [Route("api/receiptInvoice/exportBankReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public ExportBankReceiptInvoiceResult ExportBankReceiptInvoice([FromBody]ExportBankReceiptInvoiceParameter request)
        {
            return this._iReceiptInvoiceDataAccess.ExportBankReceiptInvoice(request);
        }

        [HttpPost]
        [Route("api/receiptInvoice/searchBankBookReceipt")]
        [Authorize(Policy = "Member")]
        public SearchBankBookReceiptResult SearchBankBookReceipt([FromBody]SearchBankBookReceiptParameter request)
        {
            return this._iReceiptInvoiceDataAccess.SearchBankBookReceipt(request);
        }

        [HttpPost]
        [Route("api/receiptInvoice/searchCashBookReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public SearchCashBookReceiptInvoiceResult SearchCashBookReceiptInvoice(
            [FromBody] SearchCashBookReceiptInvoiceParameter request)
        {
            return this._iReceiptInvoiceDataAccess.SearchCashBookReceiptInvoice(request);
        }

        [HttpPost]
        [Route("api/receiptInvoice/getOrderByCustomerId")]
        [Authorize(Policy = "Member")]
        public GetOrderByCustomerIdResult GetOrderByCustomerId([FromBody] GetOrderByCustomerIdParameter request)
        {
            return this._iReceiptInvoiceDataAccess.GetOrderByCustomerId(request);
        }

        [HttpPost]
        [Route("api/receiptInvoice/getMasterDateSearchBankBookReceipt")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchBankReceiptInvoiceResult GetMasterDataSearchBankReceiptInvoice(
            [FromBody] GetMasterDataSearchBankReceiptInvoiceParameter request)
        {
            return this._iReceiptInvoiceDataAccess.GetMasterDataSearchBankReceiptInvoice(request);
        }
        
        [HttpPost]
        [Route("api/receiptInvoice/getMasterDataBookReceipt")]
        [Authorize(Policy = "Member")]
        public GetMasterDataReceiptInvoiceResult GetMasterDataReceiptInvoice(
            [FromBody]GetMasterDataReceiptInvoiceParameter request)
        {
            return this._iReceiptInvoiceDataAccess.GetMasterDataReceiptInvoice(request);
        }

        [HttpPost]
        [Route("api/receiptInvoice/GetMasterDataSearchReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchReceiptInvoiceResult GetMasterDataSearchReceiptInvoice(
            [FromBody]GetMasterDataSearchReceiptInvoiceParameter request)
        {
            return this._iReceiptInvoiceDataAccess.GetMasterDataSearchReceiptInvoice(request);
        }
    }
}
