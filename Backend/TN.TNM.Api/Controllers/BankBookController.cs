using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.BankBook;
using TN.TNM.BusinessLogic.Messages.Requests.BankBook;
using TN.TNM.BusinessLogic.Messages.Responses.BankBook;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.BankBook;
using TN.TNM.DataAccess.Messages.Results.BankBook;

namespace TN.TNM.Api.Controllers
{
    public class BankBookController : Controller
    {
        private readonly IBankBook iBankBook;
        private readonly IBankBookDataAccess iBankBookDataAccess;
        public BankBookController(IBankBook _iBankBook, IBankBookDataAccess _iBankBookDataAccess)
        {
            this.iBankBook = _iBankBook;
            iBankBookDataAccess = _iBankBookDataAccess;
        }

        /// <summary>
        /// Search Bank Book
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankbook/searchBankBook")]
        [Authorize(Policy = "Member")]
        public SearchBankBookResult SearchBankBook([FromBody]SearchBankBookParameter request)
        {
            return iBankBookDataAccess.SearchBankBook(request);
        }

        [HttpPost]
        [Route("api/bankbook/getMasterDataSearchBankBook")]
        [Authorize(Policy = "Member")]
        public GetMaterDataSearchBankBookResult GetMasterDataSearchBankBook([FromBody]GetMasterDataSearchBankBookParameter request)
        {
            return iBankBookDataAccess.GetMasterDataSearchBankBook(request);
        }
    }
}