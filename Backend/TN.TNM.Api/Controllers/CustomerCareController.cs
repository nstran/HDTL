using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.CustomerCare;
using TN.TNM.BusinessLogic.Messages.Requests.CustomerCare;
using TN.TNM.BusinessLogic.Messages.Responses.CustomerCare;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;
using TN.TNM.DataAccess.Messages.Results.CustomerCare;

namespace TN.TNM.Api.Controllers
{
    public class CustomerCareController : Controller
    {
        private readonly ICustomerCareDataAccess _iCustomerCareDataAccess;
        public CustomerCareController(ICustomerCareDataAccess iCustomerCareDataAccess)
        {
            this._iCustomerCareDataAccess = iCustomerCareDataAccess;
        }

        /// <summary>
        /// Create CustomerCare
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/createCustomerCare")]
        [Authorize(Policy = "Member")]
        public CreateCustomerCareResult CreateCustomerCare([FromBody] CreateCustomerCareParameter request)
        {
            return this._iCustomerCareDataAccess.CreateCustomerCare(request);
        }
        /// <summary>
        /// Update CustomerCare
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/updateCustomerCare")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerCareResult UpdateCustomerCare([FromBody] UpdateCustomerCareParameter request)
        {
            return this._iCustomerCareDataAccess.UpdateCustomerCare(request);
        }
        /// <summary>
        /// Get CustomerCare ById
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getCustomerCareById")]
        [Authorize(Policy = "Member")]
        public GetCustomerCareByIdResult GetCustomerCareById([FromBody] GetCustomerCareByIdParameter request)
        {
            return this._iCustomerCareDataAccess.GetCustomerCareById(request);
        }
        /// <summary>
        /// Create CustomerCare FeedBack
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/createCustomerCareFeedBack")]
        [Authorize(Policy = "Member")]
        public CreateCustomerCareFeedBackResult CreateCustomerCareFeedBack([FromBody] CreateCustomerCareFeedBackParameter request)
        {
            return this._iCustomerCareDataAccess.CreateCustomerCareFeedBack(request);
        }
        /// <summary>
        /// Update CustomerCareFeedBack
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/updateCustomerCareFeedBack")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerCareFeedBackResult UpdateCustomerCareFeedBack([FromBody] UpdateCustomerCareFeedBackParameter request)
        {
            return this._iCustomerCareDataAccess.UpdateCustomerCareFeedBack(request);
        }

        /// <summary>
        /// FilterCustomer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/filterCustomer")]
        [Authorize(Policy = "Member")]
        public FilterCustomerResult FilterCustomer([FromBody] FilterCustomerParameter request)
        {
            return this._iCustomerCareDataAccess.FilterCustomer(request);
        }

        /// <summary>
        /// SearchCustomerCare
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/searchCustomerCare")]
        [Authorize(Policy = "Member")]
        public SearchCustomerCareResult SearchCustomerCare([FromBody] SearchCustomerCareParameter request)
        {
            return this._iCustomerCareDataAccess.SearchCustomerCare(request);
        }

        /// <summary>
        /// Update Status Customer Care Customer By Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/updateStatusCustomerCareCustomerById")]
        [Authorize(Policy = "Member")]
        public UpdateStatusCustomerCareCustomerByIdResult UpdateStatusCustomerCareCustomerById([FromBody] UpdateStatusCustomerCareCustomerByIdParameter request)
        {
            return this._iCustomerCareDataAccess.UpdateStatusCustomerCareCustomerById(request);
        }

        /// <summary>
        /// Get TimeLine Customer Care By CustomerId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getTimeLineCustomerCareByCustomerId")]
        [Authorize(Policy = "Member")]
        public GetTimeLineCustomerCareByCustomerIdResult GetTimeLineCustomerCareByCustomerId([FromBody] GetTimeLineCustomerCareByCustomerIdParameter request)
        {
            return this._iCustomerCareDataAccess.GetTimeLineCustomerCareByCustomerId(request);
        }

        /// <summary>
        /// Get CustomerCareFeedBack By Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getCustomerCareFeedBackByCusIdAndCusCareId")]
        [Authorize(Policy = "Member")]
        public GetCustomerCareFeedBackByCusIdAndCusCareIdResult GetCustomerCareFeedBackByCusIdAndCusCareId([FromBody] GetCustomerCareFeedBackByCusIdAndCusCareIdParameter request)
        {
            return this._iCustomerCareDataAccess.GetCustomerCareFeedBackByCusIdAndCusCareId(request);
        }

        /// <summary>
        /// Send Quick Email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/sendQuickEmail")]
        [Authorize(Policy = "Member")]
        public SendQuickEmailResult SendQuickEmail([FromForm] SendQuickEmailParameter request)
        {
            return this._iCustomerCareDataAccess.SendQuickEmail(request);
        }

        /// <summary>
        /// Send Quick SMS
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/sendQuickSMS")]
        [Authorize(Policy = "Member")]
        public SendQuickSMSResult SendQuickSMS([FromBody] SendQuickSMSParameter request)
        {
            return this._iCustomerCareDataAccess.SendQuickSMS(request);
        }

        /// <summary>
        /// Send Quick Gift
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/SendQuickGift")]
        [Authorize(Policy = "Member")]
        public SendQuickGiftResult SendQuickGift([FromBody] SendQuickGiftParameter request)
        {
            return this._iCustomerCareDataAccess.SendQuickGift(request);
        }

        /// <summary>
        /// Update Status Customer Care
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/updateStatusCustomerCare")]
        [Authorize(Policy = "Member")]
        public UpdateStatusCustomerCareResult UpdateStatusCustomerCare([FromBody] UpdateStatusCustomerCareParameter request)
        {
            return this._iCustomerCareDataAccess.UpdateStatusCustomerCare(request);
        }

        /// <summary>
        /// GetTotalInteractive
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getTotalInteractive")]
        [Authorize(Policy = "Member")]
        public GetTotalInteractiveResult GetTotalInteractive([FromBody] GetTotalInteractiveParameter request)
        {
            return this._iCustomerCareDataAccess.GetTotalInteractive(request);
        }

        /// <summary>
        /// Update Status Customer Care
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getCustomerBirthDay")]
        [Authorize(Policy = "Member")]
        public GetCustomerBirthDayResult GetCustomerBirthDay([FromBody] GetCustomerBirthDayParameter request)
        {
            return this._iCustomerCareDataAccess.GetCustomerBirthDay(request);
        }

        /// <summary>
        /// GetCustomerNewCS
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getCustomerNewCS")]
        [Authorize(Policy = "Member")]
        public GetCustomerNewCSResult GetCustomerNewCS([FromBody] GetCustomerNewCSParameter request)
        {
            return this._iCustomerCareDataAccess.GetCustomerNewCS(request);
        }

        /// <summary>
        /// GetCustomerCareActive
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getCustomerCareActive")]
        [Authorize(Policy = "Member")]
        public GetCustomerCareActiveResult GetCustomerCareActive([FromBody] GetCustomerCareActiveParameter request)
        {
            return this._iCustomerCareDataAccess.GetCustomerCareActive(request);
        }

        /// <summary>
        /// GetCustomerCareActive
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getCharCustomerCS")]
        [Authorize(Policy = "Member")]
        public GetCharCustomerCSResult GetCharCustomerCS([FromBody] GetCharCustomerCSParameter request)
        {
            return this._iCustomerCareDataAccess.GetCharCustomerCS(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getMasterDataCustomerCareList")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCustomerCareListResult GetMasterDataCustomerCareList([FromBody] GetMasterDataCustomerCareListParameter request)
        {
            return this._iCustomerCareDataAccess.GetMasterDataCustomerCareList(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/updateStatusCusCare")]
        [Authorize(Policy = "Member")]
        public UpdateStatusCusCareResult UpdateStatusCusCare([FromBody] UpdateStatusCusCareParameter request)
        {
            return this._iCustomerCareDataAccess.UpdateStatusCusCare(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/updateCustomerMeeting")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerMeettingResult UpdateCustomerMeeting([FromBody] UpdateCustomerMettingParameter request)
        {
            return this._iCustomerCareDataAccess.UpdateCustomerMeeting(request);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/removeCustomerMeeting")]
        [Authorize(Policy = "Member")]
        public RemoveCustomerMeetingResult RemoveCustomerMeeting([FromBody] RemoveCustomerMettingParameter request)
        {
            return this._iCustomerCareDataAccess.RemoveCustomerMeeting(request);
        }
    }
}