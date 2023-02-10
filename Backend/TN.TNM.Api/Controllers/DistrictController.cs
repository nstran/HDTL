using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.District;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.District;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.District;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.District;
using TN.TNM.DataAccess.Messages.Results.Admin.District;

namespace TN.TNM.Api.Controllers
{
    public class DistrictController : Controller
    {
        private readonly IDistrictDataAccess iDistrictDataAccess;

        public DistrictController(IDistrictDataAccess _iDistrictDataAccess)
        {
            this.iDistrictDataAccess = _iDistrictDataAccess;
        }

        /// <summary>
        /// Get all District info
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/district/getAllDistrictByProvinceId")]
        [Authorize(Policy = "Member")]
        public GetAllDistrictByProvinceIdResult GetAllDistrictByProvinceId([FromBody]GetAllDistrictByProvinceIdParameter request)
        {
            return this.iDistrictDataAccess.GetAllDistrictByProvinceId(request);
        }
    }
}