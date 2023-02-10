using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.CashBook;
using TN.TNM.BusinessLogic.Messages.Requests.CashBook;
using TN.TNM.BusinessLogic.Messages.Responses.CashBook;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.CashBook;
using TN.TNM.DataAccess.Messages.Results.CashBook;

namespace TN.TNM.Api.Controllers
{
    public class CashBookController : Controller
    {
        private readonly ICashBook iCashBook;
        private readonly ICashBookDataAccess iCashBookDataAccess;
        public CashBookController(ICashBook _iCashBook, ICashBookDataAccess _iCashBookDataAccess)
        {
            this.iCashBook = _iCashBook;
            this.iCashBookDataAccess = _iCashBookDataAccess;
        }

        /// <summary>
        /// getSurplusCashBookPerMonth
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/cashbook/getSurplusCashBookPerMonth")]
        [Authorize(Policy = "Member")]
        public GetSurplusCashBookPerMonthResult GetSurplusCashBookPerMonth([FromBody]GetSurplusCashBookPerMonthParameter request)
        {
            return iCashBookDataAccess.GetSurplusCashBookPerMonth(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/cashbook/getDataSearchCashBook")]
        [Authorize(Policy = "Member")]
        public GetDataSearchCashBookResult GetDataSearchCashBook([FromBody]GetDataSearchCashBookParameter request)
        {
            return iCashBookDataAccess.GetDataSearchCashBook(request);
        }
    }
}