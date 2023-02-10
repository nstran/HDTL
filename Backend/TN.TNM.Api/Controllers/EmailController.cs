using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Email;
using TN.TNM.BusinessLogic.Messages.Requests.Email;
using TN.TNM.BusinessLogic.Messages.Responses.Email;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Email;
using TN.TNM.DataAccess.Messages.Results.Email;

namespace TN.TNM.Api.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailDataAccess iEmailDataAccess;
        public EmailController( IEmailDataAccess _iEmailDataAccess)
        {
            iEmailDataAccess = _iEmailDataAccess;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmail")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailResult SendEmail([FromBody]SendEmailParameter request)
        {
            return iEmailDataAccess.SendEmail(request);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailAfterEditPic")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailAfterEditPicResult SendEmailAfterEditPic([FromBody]SendEmailAfterEditPicParameter request)
        {
            return iEmailDataAccess.SendEmailAfterEditPic(request);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailAfterCreatedLead")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailAfterCreatedLeadResult SendEmailAfterCreatedLead([FromBody]SendEmailAfterCreatedLeadParameter request)
        {
            return iEmailDataAccess.SendEmailAfterCreatedLead(request);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailAfterCreateNote")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailAfterCreateNoteResult SendEmailAfterCreateNote([FromBody]SendEmailAfterCreateNoteParameter request)
        {
            return iEmailDataAccess.SendEmailAfterCreateNote(request);
        }

        /// <summary>
        /// SendEmailEmployeePayslip
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailEmployeePayslip")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailEmployeePayslipResult SendEmailEmployeePayslip([FromBody]SendEmailEmployeePayslipParameter request)
        {
            return iEmailDataAccess.SendEmailEmployeePayslip(request);
        }

        /// <summary>
        /// SendEmailTeacherPayslip
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailTeacherPayslip")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailTeacherPayslipResult SendEmailTeacherPayslip([FromBody]SendEmailTeacherPayslipParameter request)
        {
            return iEmailDataAccess.SendEmailTeacherPayslip(request);
        }
        /// <summary>
        /// SendEmailAssistantPayslip
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailAssistantPayslip")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailAssistantPayslipResult SendEmailAssistantPayslip([FromBody]SendEmailAssistantPayslipParameter request)
        {
            return iEmailDataAccess.SendEmailAssistantPayslip(request);
        }

        [Route("api/email/sendEmailVendorOrder")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailVendorOrderResult SendEmailVendorOrder([FromBody]SendEmailVendorOrderParameter request)
        {
            return iEmailDataAccess.SendEmailVendorOrder(request);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailForgotPass")]
        [HttpPost]
        [AllowAnonymous]
        public SendEmailForgotPassResult SendEmailForgotPass([FromBody]SendEmailForgotPassParameter request)
        {
            return iEmailDataAccess.SendEmailForgotPass(request);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailCustomerOrder")]
        [HttpPost]
        [AllowAnonymous]
        public SendEmailCustomerOrderResult sendEmailCustomerOrder([FromBody]SendEmailCustomerOrderParameter request)
        {
            return iEmailDataAccess.SendEmailCustomerOrder(request);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailPersonApprove")]
        [HttpPost]
        [AllowAnonymous]
        public SendEmailPersonApproveResult SendEmailPersonApprove([FromBody]SendEmailPersonApproveParameter request)
        {
            return iEmailDataAccess.SendEmailPersonApprove(request);
        }
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailPersonCreate")]
        [HttpPost]
        [AllowAnonymous]
        public SendEmailPersonCreateResult SendEmailPersonCreate([FromBody]SendEmailPersonCreateParameter request)
        {
            return iEmailDataAccess.SendEmailPersonCreate(request);
        }
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailPersonNotify")]
        [HttpPost]
        [AllowAnonymous]
        public SendEmailPersonNotifyResult SendEmailPersonNotify([FromBody]SendEmailPersonNotifyParameter request)
        {
            return iEmailDataAccess.SendEmailPersonNotify(request);
        }
    }
}