using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.Email;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Email;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Email;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Email;
using TN.TNM.DataAccess.Messages.Results.Admin.Email;

namespace TN.TNM.Api.Controllers
{
    public class EmailConfigController : Controller
    {
        private readonly IEmailConfigurationDataAccess iEmailConfigurationDataAccess;
        public EmailConfigController( IEmailConfigurationDataAccess _iEmailConfigurationDataAccess)
        {
            this.iEmailConfigurationDataAccess = _iEmailConfigurationDataAccess;
        }

        [HttpPost]
        [Route("api/emailConfig/createUpdateEmailTemplateMasterdata")]
        [Authorize(Policy = "Member")]
        public CreateUpdateEmailTemplateMasterdataResult CreateUpdateEmailTemplateMasterdata([FromBody]CreateUpdateEmailTemplateMasterdataParameter request)
        {
            return this.iEmailConfigurationDataAccess.CreateUpdateEmailTemplateMasterdata(request);
        }

        [HttpPost]
        [Route("api/emailConfig/createUpdateEmailTemplate")]
        [Authorize(Policy = "Member")]
        public CreateUpdateEmailTemplateResult CreateUpdateEmailTemplate([FromBody]CreateUpdateEmailTemplateParameter request)
        {
            return this.iEmailConfigurationDataAccess.CreateUpdateEmailTemplate(request);
        }

        [HttpPost]
        [Route("api/emailConfig/searchEmailConfigMasterdata")]
        [Authorize(Policy = "Member")]
        public SearchEmailConfigMasterdataResult SearchEmailConfigMasterdata([FromBody]SearchEmailConfigMasterdataParameter request)
        {
            return this.iEmailConfigurationDataAccess.SearchEmailConfigMasterdata(request);
        }

        [HttpPost]
        [Route("api/emailConfig/searchEmailTemplate")]
        [Authorize(Policy = "Member")]
        public SearchEmailTemplateResult SearchEmailTemplate([FromBody]SearchEmailTemplateParameter request)
        {
            return this.iEmailConfigurationDataAccess.SearchEmailTemplate(request);
        }

        [HttpPost]
        [Route("api/emailConfig/sendEmail")]
        [Authorize(Policy = "Member")]
        public SendEmailResult SendEmail([FromBody]SendEmailParameter request)
        {
            return this.iEmailConfigurationDataAccess.SendEmail(request);
        }
        
        [HttpPost]
        [Route("api/emailConfig/getTokenForEmailTypeId")]
        [Authorize(Policy = "Member")]
        public GetTokenForEmailTypeIdResult GetTokenForEmailTypeId([FromBody]GetTokenForEmailTypeIdParameter request)
        {
            return this.iEmailConfigurationDataAccess.GetTokenForEmailTypeId(request);
        }
    }
}