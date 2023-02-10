using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Lead;
using TN.TNM.BusinessLogic.Messages.Requests.Lead;
using TN.TNM.BusinessLogic.Messages.Responses.Leads;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Lead;
using TN.TNM.DataAccess.Messages.Results.Lead;

/// <summary>
/// A short description about this controller
/// 
/// 1/ A short description about 1st function
/// 2/ A short description about 2nd function
/// 3/ ...
/// 
/// Author: thanhhh@tringhiatech.vn
/// Date: 13/06/2016
/// </summary>
namespace TN.TNM.Api.Controllers
{
    public class LeadSearchController : Controller
    {
        private readonly ILeadSearch iLead;
        private readonly ILeadSearchDataAccess iLeadDataAccess;

        public LeadSearchController(ILeadSearch _iLead , ILeadSearchDataAccess _iLeadDataAccess)
        {
            this.iLead = _iLead;
            iLeadDataAccess = _iLeadDataAccess;

        }

        /// <summary>
        /// Search lead by parameter
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/getAllLead")]
        [Authorize(Policy = "Member")]
        public GetAllLeadResult GetAllLead([FromBody]GetAllLeadParameter request)
        {
            return this.iLeadDataAccess.GetAllLead(request);
        }

        /// <summary>
        /// Create a new lead
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/lead/create")]
        [Authorize(Policy = "Member")]
        public CreateLeadResult CreateLead([FromBody]CreateLeadParameter request)
        {
            return this.iLeadDataAccess.CreateLead(request);
        }

        /// <summary>
        /// GetLeadById
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/lead/getLeadById")]
        [Authorize(Policy = "Member")]
        public GetLeadByIdResult GetLeadById([FromBody] GetLeadByIdParamater request)
        {
            return this.iLeadDataAccess.GetLeadById(request);
        }

        /// <summary>
        /// EditLeadById
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/lead/editLeadById")]
        [Authorize(Policy = "Member")]
        public EditLeadByIdResult EditLeadById([FromBody]EditLeadByIdParameter request)
        {
            return this.iLeadDataAccess.EditLeadById(request);
        }
        /// <summary>
        /// GetNoteHistory
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/lead/getNoteHistory")]
        [Authorize(Policy = "Member")]
        public GetNoteHistoryResult GetNoteHistory([FromBody] GetNoteHistoryParameter request)
        {
            return this.iLeadDataAccess.GetNoteHistory(request);
        }

        /// <summary>
        /// Search lead by parameter
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/getLeadByStatus")]
        [Authorize(Policy = "Member")]
        public GetLeadByStatusResult GetLeadByStatus([FromBody]GetLeadByStatusParameter request)
        {
            return this.iLeadDataAccess.GetLeadByStatus(request);
        }

        /// <summary>
        /// Search lead by parameter
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/getLeadByName")]
        [Authorize(Policy = "Member")]
        public GetLeadByNameResult GetLeadByName([FromBody]GetLeadByNameParameter request)
        {
            return this.iLeadDataAccess.GetLeadByName(request);
        }

        /// <summary>
        /// Get all employee which in the same organization of current user
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/getEmployeeWithNotificationPermisison")]
        [Authorize(Policy = "Member")]
        public GetEmployeeWithNotificationPermisisonResult GetEmployeeWithNotificationPermisison([FromBody]GetEmployeeWithNotificationPermisisonParameter request)
        {
            return this.iLeadDataAccess.GetEmployeeWithNotificationPermisison(request);
        }

        /// <summary>
        /// Change status to Unfollow
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/changeLeadStatusToUnfollow")]
        [Authorize(Policy = "Member")]
        public ChangeLeadStatusToUnfollowResult ChangeLeadStatusToUnfollow([FromBody]ChangeLeadStatusToUnfollowParameter request)
        {
            return this.iLeadDataAccess.ChangeLeadStatusToUnfollow(request);
        }

        /// <summary>
        /// Approve or Reject change status to Unfollow
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/approveRejectUnfollowLead")]
        [Authorize(Policy = "Member")]
        public ApproveRejectUnfollowLeadResult ApproveRejectUnfollowLead([FromBody]ApproveRejectUnfollowLeadParameter request)
        {
            return this.iLeadDataAccess.ApproveRejectUnfollowLead(request);
        }

        /// <summary>
        /// Get all Employee's manager
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/getEmployeeManager")]
        [Authorize(Policy = "Member")]
        public GetEmployeeManagerResult GetEmployeeManager([FromBody]GetEmployeeManagerParameter request)
        {
            return this.iLeadDataAccess.GetEmployeeManager(request);
        }

        /// <summary>
        /// Send Email Lead
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/sendEmailLead")]
        [Authorize(Policy = "Member")]
        public SendEmailLeadResult SendEmailLead([FromBody]SendEmailLeadParameter request)
        {
            return this.iLeadDataAccess.SendEmailLead(request);
        }
        
        /// <summary>
        /// Send SMS Lead
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/sendSMSLead")]
        [Authorize(Policy = "Member")]
        public SendSMSLeadResult SendSMSLead([FromBody]SendSMSLeadParameter request)
        {
            return this.iLeadDataAccess.SendSMSLead(request);
        }
        /// <summary>
        /// Send SMS Lead
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/importLead")]
        [Authorize(Policy = "Member")]
        public ImportLeadResult ImportLead(ImportLeadParameter request)
        {
            return this.iLeadDataAccess.ImportLead(request);
        }
        /// <summary>
        /// UpdateLeadDuplicate
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/updateLeadDuplicate")]
        [Authorize(Policy = "Member")]
        public UpdateLeadDuplicateResult UpdateLeadDuplicate([FromBody]UpdateLeadDuplicateParameter request)
        {
            return this.iLeadDataAccess.UpdateLeadDuplicate(request);
        }

        /// <summary>
        /// DownloadTemplateCustomer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/downloadTemplateLead")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateLeadResult DownloadTemplateCustomer([FromBody]DownloadTemplateLeadParameter request)
        {
            return this.iLeadDataAccess.DownloadTemplateLead(request);
        }

        /// <summary>
        /// ChangeLeadStatusToDelete
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/changeLeadStatusToDelete")]
        [Authorize(Policy = "Member")]
        public ChangeLeadStatusToDeleteResult ChangeLeadStatusToDelete([FromBody]ChangeLeadStatusToDeleteParameter request)
        {
            return this.iLeadDataAccess.ChangeLeadStatusToDelete(request);
        }

        /// <summary>
        /// CheckEmailLead
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/checkEmailLead")]
        [Authorize(Policy = "Member")]
        public CheckEmailLeadResult CheckEmailLead([FromBody]CheckEmailLeadParameter request)
        {
            return this.iLeadDataAccess.CheckEmailLead(request);
        }

        /// <summary>
        /// CheckPhoneLead
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/checkPhoneLead")]
        [Authorize(Policy = "Member")]
        public CheckPhoneLeadResult CheckPhoneLead([FromBody]CheckPhoneLeadParameter request)
        {
            return this.iLeadDataAccess.CheckPhoneLead(request);
        }

        /// <summary>
        /// Get Person In Charge
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/getPersonInCharge")]
        [Authorize(Policy = "Member")]
        public GetPersonInChargeResult GetPersonInCharge([FromBody]GetPersonInChargeParameter request)
        {
            return this.iLeadDataAccess.GetPersonInCharge(request);
        }

        /// <summary>
        /// Edit Person In Charge
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/editPersonInCharge")]
        [Authorize(Policy = "Member")]
        public EditPersonInChargeResult EditPersonInCharge([FromBody]EditPersonInChargeParameter request)
        {
            return this.iLeadDataAccess.EditPersonInCharge(request);
        }

        /// <summary>
        /// Edit Lead Status By Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/editLeadStatusById")]
        [Authorize(Policy = "Member")]
        public EditLeadStatusByIdResult EditLeadStatusById([FromBody]EditLeadStatusByIdParameter request)
        {
            return this.iLeadDataAccess.EditLeadStatusById(request);
        }

        [HttpPost]
        [Route("api/lead/getDataCreateLead")]
        [Authorize(Policy = "Member")]
        public GetDataCreateLeadResult GetDataCreateLead([FromBody]GetDataCreateLeadParameter request)
        {
            return this.iLeadDataAccess.GetDataCreateLead(request);
        }

        [HttpPost]
        [Route("api/lead/getListCustomerByType")]
        [Authorize(Policy = "Member")]
        public GetListCustomerByTypeResult GetListCustomerByType([FromBody]GetListCustomerByTypeParameter request)
        {
            return this.iLeadDataAccess.GetListCustomerByType(request);
        }

        [HttpPost]
        [Route("api/lead/getDatEditLead")]
        [Authorize(Policy = "Member")]
        public GetDataEditLeadResult GetDataEditLead([FromBody]GetDataEditLeadParameter request)
        {
            return this.iLeadDataAccess.GetDataEditLead(request);
        }

        [HttpPost]
        [Route("api/lead/getDataSearchLead")]
        [Authorize(Policy = "Member")]
        public GetDataSearchLeadResult GetDataSearchLead([FromBody]GetDataSearchLeadParameter request)
        {
            return this.iLeadDataAccess.GetDataSearchLead(request);
        }

        [HttpPost]
        [Route("api/lead/approveOrRejectLeadUnfollow")]
        [Authorize(Policy = "Member")]
        public ApproveOrRejectLeadUnfollowResult ApproveOrRejectLeadUnfollow([FromBody]ApproveOrRejectLeadUnfollowParameter request)
        {
            return this.iLeadDataAccess.ApproveOrRejectLeadUnfollow(request);
        }

        [HttpPost]
        [Route("api/lead/setPersonalInChange")]
        [Authorize(Policy = "Member")]
        public SetPersonalInChangeResult SetPersonalInChange([FromBody]SetPersonalInChangeParameter request)
        {
            return this.iLeadDataAccess.SetPersonalInChange(request);
        }

        [HttpPost]
        [Route("api/lead/unfollowListLead")]
        [Authorize(Policy = "Member")]
        public UnfollowListLeadResult UnfollowListLead([FromBody] UnfollowListLeadParamerter request)
        {
            return this.iLeadDataAccess.UnfollowListLead(request);
        }

        [HttpPost]
        [Route("api/lead/importLeadDetail")]
        [Authorize(Policy = "Member")]
        public ImportLeadDetailResult ImportLeadDetail([FromBody]ImportLeadDetailParameter request)
        {
            return this.iLeadDataAccess.ImportLeadDetail(request);
        }

        [HttpPost]
        [Route("api/lead/importListLead")]
        [Authorize(Policy = "Member")]
        public ImportListLeadResult ImportListLead([FromBody]ImportListLeadParameter request)
        {
            return this.iLeadDataAccess.ImportListLead(request);
        }

        [HttpPost]
        [Route("api/lead/getDataLeadProductDialog")]
        [Authorize(Policy = "Member")]
        public GetDataLeadProductDialogResult GetDataLeadProductDialog([FromBody]GetDataLeadProductDialogParameter request)
        {
            return this.iLeadDataAccess.GetDataLeadProductDialog(request);
        }

        [HttpPost]
        [Route("api/lead/sendEmailSupportLead")]
        [Authorize(Policy = "Member")]
        public SendEmailSupportLeadResult SendEmailSupportLead([FromBody]SendEmailSupportLeadParameter request)
        {
            return this.iLeadDataAccess.SendEmailSupportLead(request);
        }

        [HttpPost]
        [Route("api/lead/sendSMSSupportLead")]
        [Authorize(Policy = "Member")]
        public SendSMSSupportLeadResult SendSMSSupportLead([FromBody]SendSMSSupportLeadParameter request)
        {
            return this.iLeadDataAccess.SendSMSSupportLead(request);
        }

        [HttpPost]
        [Route("api/lead/sendGiftSupportLead")]
        [Authorize(Policy = "Member")]
        public SendGiftSupportLeadResult SendGiftSupportLead([FromBody]SendGiftSupportLeadParameter request)
        {
            return this.iLeadDataAccess.SendGiftSupportLead(request);
        }

        [HttpPost]
        [Route("api/lead/createLeadMeeting")]
        [Authorize(Policy = "Member")]
        public CreateLeadMeetingResult CreateLeadMeeting([FromBody]CreateLeadMeetingParameter request)
        {
            return this.iLeadDataAccess.CreateLeadMeeting(request);
        }

        [HttpPost]
        [Route("api/lead/changeLeadStatus")]
        [Authorize(Policy = "Member")]
        public ChangeLeadStatusResult ChangeLeadStatus([FromBody]ChangeLeadStatusParameter request)
        {
            return this.iLeadDataAccess.ChangeLeadStatus(request);
        }

        [HttpPost]
        [Route("api/lead/getHistoryLeadCare")]
        [Authorize(Policy = "Member")]
        public GetHistoryLeadCareResult GetHistoryLeadCare([FromBody]GetHistoryLeadCareParameter request)
        {
            return this.iLeadDataAccess.GetHistoryLeadCare(request);
        }

        [HttpPost]
        [Route("api/lead/getDataPreviewLeadCare")]
        [Authorize(Policy = "Member")]
        public GetDataPreviewLeadCareResult GetDataPreviewLeadCare([FromBody]GetDataPreviewLeadCareParameter request)
        {
            return this.iLeadDataAccess.GetDataPreviewLeadCare(request);
        }

        [HttpPost]
        [Route("api/lead/getDataLeadCareFeedBack")]
        [Authorize(Policy = "Member")]
        public GetDataLeadCareFeedBackResult GetDataLeadCareFeedBack([FromBody]GetDataLeadCareFeedBackParameter request)
        {
            return this.iLeadDataAccess.GetDataLeadCareFeedBack(request);
        }

        [HttpPost]
        [Route("api/lead/saveLeadCareFeedBack")]
        [Authorize(Policy = "Member")]
        public SaveLeadCareFeedBackResult SaveLeadCareFeedBack([FromBody]SaveLeadCareFeedBackParameter request)
        {
            return this.iLeadDataAccess.SaveLeadCareFeedBack(request);
        }
       
        [HttpPost]
        [Route("api/lead/getHistoryLeadMeeting")]
        [Authorize(Policy = "Member")]
        public GetHistoryLeadMeetingResult GetHistoryLeadMeeting([FromBody]GetHistoryLeadMeetingParameter request)
        {
            return this.iLeadDataAccess.GetHistoryLeadMeeting(request);
        }

        [HttpPost]
        [Route("api/lead/getDataLeadMeetintById")]
        [Authorize(Policy = "Member")]
        public GetDataLeadMeetingByIdResult GetDataLeadMeetintById([FromBody]GetDataLeadMeetingByIdParameter request)
        {
            return this.iLeadDataAccess.GetDataLeadMeetingById(request);
        }

        [HttpPost]
        [Route("api/lead/cloneLead")]
        [Authorize(Policy = "Member")]
        public CloneLeadResult ReplyLead([FromBody]CloneLeadParameter request)
        {
            return this.iLeadDataAccess.CloneLead(request);
        }

        [HttpPost]
        [Route("api/lead/getEmployeeSeller")]
        [Authorize(Policy = "Member")]
        public GetEmployeeSellerResult GetEmployeeSeller([FromBody]GetEmployeeSellerParameter request)
        {
            return this.iLeadDataAccess.GetEmployeeSeller(request);
        }
        
        [HttpPost]
        [Route("api/lead/getVendorByProductId")]
        [Authorize(Policy = "Member")]
        public GetVendorByProductIdResult GetVendorByProductId([FromBody]GetVendorByProductIdParameter request)
        {
            return this.iLeadDataAccess.GetVendorByProductId(request);
        }

        //
        [HttpPost]
        [Route("api/lead/getEmployeeByPersonInCharge")]
        [Authorize(Policy = "Member")]
        public GetEmployeeByPersonInChargeResult GetEmployeeByPersonInCharge([FromBody]GetEmployeeByPersonInChargeParameter request)
        {
            return this.iLeadDataAccess.GetEmployeeByPersonInCharge(request);
        }

        [HttpPost]
        [Route("api/lead/reportLead")]
        [Authorize(Policy = "Member")]
        public ReportLeadResult ReportLead([FromBody]ReportLeadParameter request)
        {
            return this.iLeadDataAccess.ReportLead(request);
        }

        [HttpPost]
        [Route("api/lead/getMasterDataReportLead")]
        [Authorize(Policy = "Member")]
        public GetMasterDataReportLeadResult GetMasterDataReportLead([FromBody]GetMasterDataReportLeadParameter request)
        {
            return this.iLeadDataAccess.GetMasterDataReportLead(request);
        }

        //
        [HttpPost]
        [Route("api/lead/changeLeadStatusSupport")]
        [Authorize(Policy = "Member")]
        public ChangeLeadStatusSupportResult ChangeLeadStatusSupport([FromBody]ChangeLeadStatusSupportParameter request)
        {
            return this.iLeadDataAccess.ChangeLeadStatusSupport(request);
        }
    }
}
