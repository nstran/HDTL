using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.CustomerServiceLevel;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.CustomerServiceLevel;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.CustomerServiceLevel;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.CustomerServiceLevel;
using TN.TNM.DataAccess.Messages.Results.Admin.CustomerServiceLevel;

namespace TN.TNM.Api.Controllers
{
    public class CustomerServiceLevelController
    {
        private readonly ICustomerServiceLevelDataAccess _iCustomerServiceLevelDataAccess;
        public CustomerServiceLevelController(ICustomerServiceLevelDataAccess iCustomerServiceLevelDataAccess)
        {
            this._iCustomerServiceLevelDataAccess = iCustomerServiceLevelDataAccess;
        }

        /// <summary>
        /// AddLevelCustomer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/admin/customerservicelevel")]
        [Authorize(Policy = "Member")]
        public AddLevelCustomerResult AddLevelCustomer([FromBody]AddLevelCustomerParameter request)
        {
            return this._iCustomerServiceLevelDataAccess.AddLevelCustomer(request);
        }
        /// <summary>
        /// GetConfigCustomerServiceLevel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/admin/getCustomerservicelevel")]
        [Authorize(Policy = "Member")]
        public GetConfigCustomerServiceLevelResult GetConfigCustomerServiceLevel([FromBody]GetConfigCustomerServiceLevelParameter request)
        {
            return this._iCustomerServiceLevelDataAccess.GetConfigCustomerServiceLevel(request);
        }
        /// <summary>
        /// UpdateConfigCustomer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/admin/updateConfigCustomer")]
        [Authorize(Policy = "Member")]
        public UpdateConfigCustomerResults UpdateConfigCustomer([FromBody]UpdateConfigCustomerParameter request)
        {
            return this._iCustomerServiceLevelDataAccess.UpdateConfigCustomer(request);
        }
    }
}
