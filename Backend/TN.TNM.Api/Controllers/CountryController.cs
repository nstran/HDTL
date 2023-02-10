using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.Country;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Country;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Country;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Country;
using TN.TNM.DataAccess.Messages.Results.Admin.Country;

namespace TN.TNM.Api.Controllers
{
    public class CountryController : Controller
    {
        private readonly ICountryDataAccess _iCountry;
        public CountryController(ICountryDataAccess iCountry)
        {
            this._iCountry = iCountry;
        }

        /// <summary>
        /// getAllCountry
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/country/getAllCountry")]
        [Authorize(Policy = "Member")]
        public GetAllCountryResult GetAllCountry([FromBody]GetAllCountryParameter request)
        {
            return this._iCountry.GetAllCountry(request);
        }
    }
}