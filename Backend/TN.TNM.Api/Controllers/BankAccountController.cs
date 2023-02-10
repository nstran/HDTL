using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.BankAccount;
using TN.TNM.BusinessLogic.Messages.Requests.BankAccount;
using TN.TNM.BusinessLogic.Messages.Responses.BankAccount;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.BankAccount;
using TN.TNM.DataAccess.Messages.Results.BankAccount;

namespace TN.TNM.Api.Controllers
{
    public class BankAccountController : Controller
    {
        private readonly IBankAccount _iBankAccount;
        private readonly IBankAccountDataAccess _iBankAccountDataAccess;
        public BankAccountController(IBankAccount iBankAccount, IBankAccountDataAccess iBankAccountDataAccess)
        {
            this._iBankAccount = iBankAccount;
            this._iBankAccountDataAccess = iBankAccountDataAccess;
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankAccount/createBankAccount")]
        [Authorize(Policy = "Member")]
        public CreateBankAccountResult CreateBankAccount([FromBody] CreateBankAccountParameter request)
        {
            return this._iBankAccountDataAccess.CreateBankAccount(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankAccount/getBankAccountById")]
        [Authorize(Policy = "Member")]
        public GetBankAccountByIdResult GetBankAccountById([FromBody] GetBankAccountByIdParameter request)
        {
            return this._iBankAccountDataAccess.GetBankAccountById(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankAccount/getAllBankAccountByObject")]
        [Authorize(Policy = "Member")]
        public GetAllBankAccountByObjectResult GetAllBankAccountByObject([FromBody] GetAllBankAccountByObjectParameter request)
        {
            return this._iBankAccountDataAccess.GetAllBankAccountByObject(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankAccount/editBankAccount")]
        [Authorize(Policy = "Member")]
        public EditBankAccountResult EditBankAccount([FromBody] EditBankAccountParameter request)
        {
            return this._iBankAccountDataAccess.EditBankAccount(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankAccount/deleteBankAccount")]
        [Authorize(Policy = "Member")]
        public DeleteBankAccountByIdResult DeleteBankAccount([FromBody] DeleteBankAccountByIdParameter request)
        {
            return this._iBankAccountDataAccess.DeleteBankAccountById(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankAccount/getCompanyBankAccount")]
        [Authorize(Policy = "Member")]
        public GetCompanyBankAccountResult GetCompanyBankAccount(GetCompanyBankAccountParameter request)
        {
            return this._iBankAccountDataAccess.GetCompanyBankAccount(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankAccount/getMasterDataBankPopup")]
        [Authorize(Policy = "Member")]
        public GetMasterDataBankPopupResult GetMasterDataBankPopup(GetMasterDataBankPopupParameter request)
        {
            return this._iBankAccountDataAccess.GetMasterDataBankPopup(request);
        }
    }
}