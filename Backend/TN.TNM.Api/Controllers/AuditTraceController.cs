using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TN.TNM.BusinessLogic.Interfaces.Admin;
using TN.TNM.BusinessLogic.Messages.Requests.Admin;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.AuditTrace;
using TN.TNM.BusinessLogic.Messages.Responses.Admin;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.AuditTrace;
using TN.TNM.DataAccess.Messages.Results.Admin.AuditTrace;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.AuditTrace;

namespace TN.TNM.Api.Controllers
{
    public class AuditTraceController : Controller
    {
        private readonly IAuditTrace _iAuditTrace;
        private readonly IAuditTraceDataAccess _iAuditTraceDataAccess;

        public AuditTraceController(IAuditTrace iAuditTrace, IAuditTraceDataAccess iAuditTraceDataAccess)
        {
            _iAuditTrace = iAuditTrace;
            _iAuditTraceDataAccess = iAuditTraceDataAccess;
        }

        [HttpPost]
        [Route("api/admin/getMasterDataTrace")]
        [Authorize(Policy = "Member")]
        public GetMasterDataTraceResult GetMasterDataTrace([FromBody]GetMasterDataTraceParameter request)
        {
            return this._iAuditTraceDataAccess.GetMasterDataTrace(request);
        }


        [HttpPost]
        [Route("api/admin/searchTrace")]
        [Authorize(Policy = "Member")]
        public SearchTraceResult SearchTrace([FromBody]SearchTraceParameter request)
        {
            return this._iAuditTraceDataAccess.SearchTrace(request);
        }

    }
}
