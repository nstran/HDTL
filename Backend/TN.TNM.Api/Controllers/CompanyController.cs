using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.Company;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Company;
using TN.TNM.BusinessLogic.Messages.Requests.CompanyConfig;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Company;
using TN.TNM.BusinessLogic.Messages.Responses.CompanyConfig;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Company;
using TN.TNM.DataAccess.Messages.Parameters.CompanyConfig;
using TN.TNM.DataAccess.Messages.Results.Admin.Category;
using TN.TNM.DataAccess.Messages.Results.Admin.Company;
using TN.TNM.DataAccess.Messages.Results.CompanyConfig;

namespace TN.TNM.Api.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompany iCompany;
        private readonly ICompanyDataAccess iCompanyDataDataAccess;
        public CompanyController(ICompany _iCompany, ICompanyDataAccess _iCompanyDataAccess)
        {
            this.iCompany = _iCompany;
            this.iCompanyDataDataAccess = _iCompanyDataAccess;
        }

        /// <summary>
        /// Get all company info
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/getAllCompany")]
        [Authorize(Policy = "Member")]
        public GetAllCompanyResult GetAllCompany(GetAllCompanyParameter request)
        {
            return this.iCompanyDataDataAccess.GetAllCompany(request);
        }
        /// <summary>
        /// Get all company info
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/getCompanyConfig")]
        [Authorize(Policy = "Member")]
        public GetCompanyConfigResults GetCompanyConfig(GetCompanyConfigParameter request)
        {
            return this.iCompanyDataDataAccess.GetCompanyConfig(request);
        }
        /// <summary>
        /// Edit Company Config
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/editCompanyConfig")]
        [Authorize(Policy = "Member")]
        public EditCompanyConfigResults EditCompanyConfig([FromBody]EditCompanyConfigParameter request)
        {
            return this.iCompanyDataDataAccess.EditCompanyConfig(request);
        }
        /// <summary>
        /// Get All System Parameter
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/getAllSystemParameter")]
        [Authorize(Policy = "Member")]
        public GetAllSystemParameterResult GetAllSystemParameter([FromBody]GetAllSystemParameterParameter request)
        {
            return this.iCompanyDataDataAccess.GetAllSystemParameter(request);
        }
        /// <summary>
        /// Change System Parameter
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/changeSystemParameter")]
        [Authorize(Policy = "Member")]
        public ChangeSystemParameterResult ChangeSystemParameter([FromBody]ChangeSystemParameterParameter request)
        {
            return this.iCompanyDataDataAccess.ChangeSystemParameter(request);
        }
    }
}