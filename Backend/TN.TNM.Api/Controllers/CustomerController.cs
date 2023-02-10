using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Customer;
using TN.TNM.BusinessLogic.Messages.Requests.Customer;
using TN.TNM.BusinessLogic.Messages.Responses.Customer;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Customer;
using TN.TNM.DataAccess.Messages.Results.Customer;

namespace TN.TNM.Api.Controllers
{
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerDataAccess _iCustomerDataAccess;
        private readonly ICustomer _iCustomer;
        public CustomerController(ICustomer iCustomer, ICustomerDataAccess iCustomerDataAccess)
        {
            this._iCustomer = iCustomer;
            _iCustomerDataAccess = iCustomerDataAccess;
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/createCustomer")]
        [Authorize(Policy = "Member")]
        public CreateCustomerResult CreateCustomer([FromBody] CreateCustomerParameter request)
        {
            return this._iCustomerDataAccess.CreateCustomer(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/searchCustomer")]
        [Authorize(Policy = "Member")]
        public SearchCustomerResult SearchCustomer([FromBody] SearchCustomerParameter request)
        {
            return this._iCustomerDataAccess.SearchCustomer(request);
        }

        /// <summary>
        /// Get Customer From Order Create
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getCustomerFromOrderCreate")]
        [Authorize(Policy = "Member")]
        public GetCustomerFromOrderCreateResult GetCustomerFromOrderCreate([FromBody] GetCustomerFromOrderCreateParameter request)
        {
            return this._iCustomerDataAccess.GetCustomerFromOrderCreate(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getAllCustomerServiceLevel")]
        [Authorize(Policy = "Member")]
        public GetAllCustomerServiceLevelResult GetAllCustomerServiceLevel([FromBody] GetAllCustomerServiceLevelParameter request)
        {
            return this._iCustomerDataAccess.GetAllCustomerServiceLevel(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getCustomerById")]
        [Authorize(Policy = "Member")]
        public GetCustomerByIdResult GetCustomerById([FromBody] GetCustomerByIdParameter request)
        {
            return this._iCustomerDataAccess.GetCustomerById(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/editCustomerById")]
        [Authorize(Policy = "Member")]
        public EditCustomerByIdResult EditCustomerById([FromBody] EditCustomerByIdParameter request)
        {
            return this._iCustomerDataAccess.EditCustomerById(request);
        }

        /// <summary>
        /// CustomerList
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getAllCustomer")]
        [Authorize(Policy = "Member")]
        public GetAllCustomerResult GetAllCustomer([FromBody] GetAllCustomerParameter request)
        {
            return this._iCustomerDataAccess.GetAllCustomer(request);
        }

        /// <summary>
        /// CustomerList
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/quickCreateCustomer")]
        [Authorize(Policy = "Member")]
        public QuickCreateCustomerResult QuickCreateCustomer([FromBody] QuickCreateCustomerParameter request)
        {
            return this._iCustomerDataAccess.QuickCreateCustomer(request);
        }

        /// <summary>
        /// CustomerCodeList
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getAllCustomerCode")]
        [Authorize(Policy = "Member")]
        public GetAllCustomerCodeResult GetAllCustomerCode(GetAllCustomerCodeParameter request)
        {
            return this._iCustomerDataAccess.GetAllCustomerCode(request);
        }
        /// <summary>
        /// ImportCustomer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/importCustomer")]
        [Authorize(Policy = "Member")]
        public ImportCustomerResult ImportCustomer(ImportCustomerParameter request)
        {
            return this._iCustomerDataAccess.ImportCustomer(request);
        }
        /// <summary>
        /// DownloadTemplateCustomer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/downloadTemplateCustomer")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateCustomerResult DownloadTemplateCustomer([FromBody] DownloadTemplateCustomerParameter request)
        {
            return this._iCustomerDataAccess.DownloadTemplateCustomer(request);
        }
        /// <summary>
        /// UpdateCustomerDuplicate
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/updateCustomerDuplicate")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerDuplicateResult UpdateCustomerDuplicate([FromBody] UpdateCustomerDuplicateParameter request)
        {
            return this._iCustomerDataAccess.UpdateCustomerDuplicate(request);
        }
        /// <summary>
        /// getStatisticCustomerForDashboard
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getStatisticCustomerForDashboard")]
        [Authorize(Policy = "Member")]
        public GetStatisticCustomerForDashboardResult GetStatisticCustomerForDashboard([FromBody] GetStatisticCustomerForDashboardParameter request)
        {
            return this._iCustomerDataAccess.GetStatisticCustomerForDashboard(request);
        }/// <summary>
         /// getListCustomeSaleToprForDashboard
         /// </summary>
         /// <param name="request"></param>
         /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getListCustomeSaleToprForDashboard")]
        [Authorize(Policy = "Member")]
        public GetListCustomeSaleToprForDashboardResult GetListCustomeSaleToprForDashboard([FromBody] GetListCustomeSaleToprForDashboardParameter request)
        {
            return this._iCustomerDataAccess.GetListCustomeSaleToprForDashboard(request);
        }
        /// <summary>
        /// getStatisticCustomerForDashboard
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/checkDuplicateCustomerPhoneOrEmail")]
        [Authorize(Policy = "Member")]
        public CheckDuplicateCustomerResult CheckDuplicateCustomerPhoneOrEmail([FromBody] CheckDuplicateCustomerLeadParameter request)
        {
            return this._iCustomerDataAccess.CheckDuplicateCustomerPhoneOrEmail(request);
        }
        /// <summary>
        /// getStatisticCustomerForDashboard
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/checkDuplicateCustomer")]
        [Authorize(Policy = "Member")]
        public CheckDuplicateCustomerResult CheckDuplicateCustomer([FromBody] CheckDuplicateCustomerParameter request)
        {
            return this._iCustomerDataAccess.CheckDuplicateCustomer(request);
        }
        /// <summary>
        /// Check Duplicate Personal Customer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/customer/checkDuplicatePersonalCustomer")]
        [Authorize(Policy = "Member")]
        public CheckDuplicatePersonalCustomerResult CheckDuplicatePersonalCustomer([FromBody] CheckDuplicatePersonalCustomerParameter request)
        {
            return this._iCustomerDataAccess.CheckDuplicatePersonalCustomer(request);
        }
        /// <summary>
        /// Check Duplicate Personal Customer By Phone Or Email
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/customer/checkDuplicatePersonalCustomerByEmailOrPhone")]
        [Authorize(Policy = "Member")]
        public CheckDuplicatePersonalCustomerByEmailOrPhoneResult CheckDuplicatePersonalCustomerByEmailOrPhone([FromBody] CheckDuplicatePersonalCustomerByEmailOrPhoneParameter request)
        {
            return this._iCustomerDataAccess.CheckDuplicatePersonalCustomerByEmailOrPhone(request);
        }
        /// <summary>
        /// Get All Customer Additional By CustomerId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getAllCustomerAdditionalByCustomerId")]
        [Authorize(Policy = "Member")]
        public GetAllCustomerAdditionalByCustomerIdResult GetAllCustomerAdditionalByCustomerId([FromBody] GetAllCustomerAdditionalByCustomerIdParameter request)
        {
            return this._iCustomerDataAccess.GetAllCustomerAdditionalByCustomerId(request);
        }

        /// <summary>
        /// Create Customer Additional
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/createCustomerAdditional")]
        [Authorize(Policy = "Member")]
        public CreateCustomerAdditionalResult CreateCustomerAdditional([FromBody] CreateCustomerAdditionalParameter request)
        {
            return this._iCustomerDataAccess.CreateCustomerAdditional(request);
        }

        /// <summary>
        /// Delete Customer Additional
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/deleteCustomerAdditional")]
        [Authorize(Policy = "Member")]
        public DeleteCustomerAdditionalResult DeleteCustomerAdditional([FromBody] DeleteCustomerAdditionalParameter request)
        {
            return this._iCustomerDataAccess.DeleteCustomerAdditional(request);
        }

        /// <summary>
        /// Edit Customer Additional
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/editCustomerAdditional")]
        [Authorize(Policy = "Member")]
        public EditCustomerAdditionalResult EditCustomerAdditional([FromBody] EditCustomerAdditionalParameter request)
        {
            return this._iCustomerDataAccess.EditCustomerAdditional(request);
        }

        /// <summary>
        /// Create List Question
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/createListQuestion")]
        [Authorize(Policy = "Member")]
        public CreateListQuestionResult CreateListQuestion([FromBody] CreateListQuestionParameter request)
        {
            return this._iCustomerDataAccess.CreateListQuestion(request);
        }

        /// <summary>
        /// Get List Question Answer By Search
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getListQuestionAnswerBySearch")]
        [Authorize(Policy = "Member")]
        public GetListQuestionAnswerBySearchResult GetListQuestionAnswerBySearch([FromBody] GetListQuestionAnswerBySearchParameter request)
        {
            return this._iCustomerDataAccess.GetListQuestionAnswerBySearch(request);
        }

        /// <summary>
        /// Get All History Product By CustomerId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getAllHistoryProductByCustomerId")]
        [Authorize(Policy = "Member")]
        public GetAllHistoryProductByCustomerIdResult GetAllHistoryProductByCustomerId([FromBody] GetAllHistoryProductByCustomerIdParameter request)
        {
            return this._iCustomerDataAccess.GetAllHistoryProductByCustomerId(request);
        }
        /// <summary>
        /// Create Customer From Protal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/createCustomerFromProtal")]
        [AllowAnonymous]
        public CreateCustomerFromProtalResult CreateCustomerFromProtal([FromBody] CreateCustomerFromProtalParameter request)
        {
            return this._iCustomerDataAccess.CreateCustomerFromProtal(request);
        }

        /// <summary>
        /// Change Customer Status To Delete
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/changeCustomerStatusToDelete")]
        [Authorize(Policy = "Member")]
        public ChangeCustomerStatusToDeleteResult ChangeCustomerStatusToDelete([FromBody] ChangeCustomerStatusToDeleteParameter request)
        {
            return this._iCustomerDataAccess.ChangeCustomerStatusToDelete(request);
        }

        //
        /// <summary>
        /// Get DashBoard Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getDashBoardCustomer")]
        [Authorize(Policy = "Member")]
        public GetDashBoardCustomerResult GetDashBoardCustomer([FromBody] GetDashBoardCustomerParameter request)
        {
            return this._iCustomerDataAccess.GetDashBoardCustomer(request);
        }

        /// <summary>
        /// Change Customer Status To Delete
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getListCustomer")]
        [Authorize(Policy = "Member")]
        public GetListCustomerResult GetListCustomer([FromBody] GetListCustomerParameter request)
        {
            return this._iCustomerDataAccess.GetListCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/createCustomerMasterData")]
        [Authorize(Policy = "Member")]
        public CreateCustomerMasterDataResult CreateCustomerMasterData([FromBody] CreateCustomerMasterDataParameter request)
        {
            return this._iCustomerDataAccess.CreateCustomerMasterData(request);
        }

        [HttpPost]
        [Route("api/customer/checkDuplicateCustomerAllType")]
        [Authorize(Policy = "Member")]
        public CheckDuplicateCustomerAllTypeResult CheckDuplicateCustomerAllType([FromBody] CheckDuplicateCustomerAllTypeParameter request)
        {
            return this._iCustomerDataAccess.CheckDuplicateCustomerAllType(request);
        }

        //
        /// <summary>
        /// Update Customer By Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/updateCustomerById")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerByIdResult UpdateCustomerById([FromBody] UpdateCustomerByIdParameter request)
        {
            return this._iCustomerDataAccess.UpdateCustomerById(request);
        }

        /// <summary>
        /// Get Customer Import Detail
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getCustomerImportDetai")]
        [Authorize(Policy = "Member")]
        public GetCustomerImportDetailResult GetCustomerImportDetail([FromBody] GetCustomerImportDetailParameter request)
        {
            return this._iCustomerDataAccess.GetCustomerImportDetail(request);
        }

        /// <summary>
        /// Import List Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/importListCustomer")]
        [Authorize(Policy = "Member")]
        public ImportListCustomerResult ImportListCustomer([FromBody] ImportListCustomerParameter request)
        {
            return this._iCustomerDataAccess.ImportListCustomer(request);
        }

        //
        [HttpPost]
        [Route("api/customer/deleteListCustomerAdditional")]
        [Authorize(Policy = "Member")]
        public DeleteListCustomerAdditionalResult DeleteListCustomerAdditional([FromBody] DeleteListCustomerAdditionalParameter request)
        {
            return this._iCustomerDataAccess.DeleteListCustomerAdditional(request);
        }

        //
        [HttpPost]
        [Route("api/customer/getHistoryCustomerCare")]
        [Authorize(Policy = "Member")]
        public GetHistoryCustomerCareResult GetHistoryCustomerCare([FromBody] GetHistoryCustomerCareParameter request)
        {
            return this._iCustomerDataAccess.GetHistoryCustomerCare(request);
        }

        //
        [HttpPost]
        [Route("api/customer/getDataPreviewCustomerCare")]
        [Authorize(Policy = "Member")]
        public GetDataPreviewCustomerCareResult GetDataPreviewCustomerCare([FromBody] GetDataPreviewCustomerCareParameter request)
        {
            return this._iCustomerDataAccess.GetDataPreviewCustomerCare(request);
        }

        //
        [HttpPost]
        [Route("api/customer/getDataCustomerCareFeedBack")]
        [Authorize(Policy = "Member")]
        public GetDataCustomerCareFeedBackResult GetDataCustomerCareFeedBack([FromBody] GetDataCustomerCareFeedBackParameter request)
        {
            return this._iCustomerDataAccess.GetDataCustomerCareFeedBack(request);
        }

        //
        [HttpPost]
        [Route("api/customer/saveCustomerCareFeedBack")]
        [Authorize(Policy = "Member")]
        public SaveCustomerCareFeedBackResult SaveCustomerCareFeedBack([FromBody] SaveCustomerCareFeedBackParameter request)
        {
            return this._iCustomerDataAccess.SaveCustomerCareFeedBack(request);
        }

        //
        [HttpPost]
        [Route("api/customer/getDataCustomerMeetingById")]
        [Authorize(Policy = "Member")]
        public GetDataCustomerMeetingByIdResult GetDataCustomerMeetingById([FromBody] GetDataCustomerMeetingByIdParameter request)
        {
            return this._iCustomerDataAccess.GetDataCustomerMeetingById(request);
        }

        //
        [HttpPost]
        [Route("api/customer/createCustomerMeeting")]
        [Authorize(Policy = "Member")]
        public CreateCustomerMeetingResult CreateCustomerMeeting([FromBody] CreateCustomerMeetingParameter request)
        {
            return this._iCustomerDataAccess.CreateCustomerMeeting(request);
        }

        //
        [HttpPost]
        [Route("api/customer/getHistoryCustomerMeeting")]
        [Authorize(Policy = "Member")]
        public GetHistoryCustomerMeetingResult GetHistoryCustomerMeeting([FromBody] GetHistoryCustomerMeetingParameter request)
        {
            return this._iCustomerDataAccess.GetHistoryCustomerMeeting(request);
        }

        //
        [HttpPost]
        [Route("api/customer/sendApprovalCustomer")]
        [Authorize(Policy = "Member")]
        public SendApprovalResult SendApproval([FromBody] SendApprovalParameter request)
        {
            return this._iCustomerDataAccess.SendApproval(request);
        }

        /// <summary>
        /// search Customer Approval
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/searchCustomerApproval")]
        [Authorize(Policy = "Member")]
        public SearchCustomerResult GetListCustomerRequestApproval([FromBody] GetListCustomerRequestApprovalParameter request)
        {
            return this._iCustomerDataAccess.GetListCustomerRequestApproval(request);
        }

        /// <summary>
        /// Approval Or RejectCustomer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/approvalOrRejectCustomer")]
        [Authorize(Policy = "Member")]
        public SendApprovalResult ApprovalOrRejectCustomer([FromBody] ApprovalOrRejectCustomerParameter request)
        {
            return this._iCustomerDataAccess.ApprovalOrRejectCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/getDataCreatePotentialCustomer")]
        [Authorize(Policy = "Member")]
        public GetDataCreatePotentialCustomerResult GetDataCreatePotentialCustomer([FromBody] GetDataCreatePotentialCustomerParameter request)
        {
            return this._iCustomerDataAccess.GetDataCreatePotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/getDataDetailPotentialCustomer")]
        [Authorize(Policy = "Member")]
        public GetDataDetailPotentialCustomerResult GetDataDetailPotentialCustomer([FromBody] GetDataDetailPotentialCustomerParameter request)
        {
            return this._iCustomerDataAccess.GetDataDetailPotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/updatePotentialCustomer")]
        [Authorize(Policy = "Member")]
        public UpdatePotentialCustomerResult UpdatePotentialCustomer([FromBody] UpdatePotentialCustomerParameter request)
        {
            return this._iCustomerDataAccess.UpdatePotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/getDataSearchPotentialCustomer")]
        [Authorize(Policy = "Member")]
        public GetDataSearchPotentialCustomerResult GetDataSearchPotentialCustomer([FromBody] GetDataSearchPotentialCustomerParameter request)
        {
            return this._iCustomerDataAccess.GetDataSearchPotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/searchPotentialCustomer")]
        [Authorize(Policy = "Member")]
        public SearchPotentialCustomerResult SearchPotentialCustomer([FromBody] SearchPotentialCustomerParameter request)
        {
            return this._iCustomerDataAccess.SearchPotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/getDataDashboardPotentialCustomer")]
        [Authorize(Policy = "Member")]
        public GetDataDashboardPotentialCustomerResult GetDataDashboardPotentialCustomer([FromBody] GetDataDashboardPotentialCustomerParameter request)
        {
            return this._iCustomerDataAccess.GetDataDashboardPotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/convertPotentialCustomer")]
        [Authorize(Policy = "Member")]
        public ConvertPotentialCustomerResult ConvertPotentialCustomer([FromBody] ConvertPotentialCustomerParameter request)
        {
            return this._iCustomerDataAccess.ConvertPotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/downloadTemplatePotentialCustomer")]
        [Authorize(Policy = "Member")]
        public DownloadTemplatePotentialCustomerResult DownloadTemplatePotentialCustomer([FromBody] DownloadTemplatePotentialCustomerParameter request)
        {
            return this._iCustomerDataAccess.DownloadTemplatePotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/getDataImportPotentialCustomer")]
        [Authorize(Policy = "Member")]
        public GetDataImportPotentialCustomerResult GetDataImportPotentialCustomer([FromBody] GetDataImportPotentialCustomerParameter request)
        {
            return this._iCustomerDataAccess.GetDataImportPotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/downloadTemplateImportCustomer")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateImportCustomerResult DownloadTemplateImportCustomer([FromBody] DownloadTemplateImportCustomerParameter request)
        {
            return this._iCustomerDataAccess.DownloadTemplateImportCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/searchContactCustomer")]
        [Authorize(Policy = "Member")]
        public SearchContactCustomerResult SearchContactCustomer([FromBody] SearchContactCustomerParameter request)
        {
            return this._iCustomerDataAccess.SearchContactCustomer(request);
        }

        //
        [HttpPost]
        [Route("api/customer/checkDuplicateInforCustomer")]
        [Authorize(Policy = "Member")]
        public CheckDuplicateInforCustomerResult CheckDuplicateInforCustomer([FromBody] CheckDuplicateInforCustomerParameter request)
        {
            return this._iCustomerDataAccess.CheckDuplicateInforCustomer(request);
        }

        //
        [HttpPost]
        [Route("api/customer/changeStatusSupport")]
        [Authorize(Policy = "Member")]
        public ChangeStatusSupportResult ChangeStatusSupport([FromBody] ChangeStatusSupportParameter request)
        {
            return this._iCustomerDataAccess.ChangeStatusSupport(request);
        }

        [HttpPost]
        [Route("api/customer/createPotentialCustomerFromWeb")]
        [AllowAnonymous]
        public CreatePotentialCutomerFromWebResult CreatePotentialCustomerFromWeb([FromBody] CreatePotentialCutomerFromWebParameter request)
        {                                             
            return this._iCustomerDataAccess.CreatePotentialCutomerFromWeb(request);
        }

        //
        [HttpPost]
        [Route("api/customer/kichHoatTinhHuong")]
        [AllowAnonymous]
        public KichHoatTinhHuongResult KichHoatTinhHuong([FromBody] KichHoatTinhHuongParameter request)
        {
            return this._iCustomerDataAccess.KichHoatTinhHuong(request);
        }

        //
        [HttpPost]
        [Route("api/customer/getListTinhHuong")]
        [AllowAnonymous]
        public GetListTinhHuongResult GetListTinhHuong([FromBody] GetListTinhHuongParameter request)
        {
            return this._iCustomerDataAccess.GetListTinhHuong(request);
        }

        //
        [HttpPost]
        [Route("api/customer/getChiTietTinhHuong")]
        [AllowAnonymous]
        public Task<GetChiTietTinhHuongResult> GetChiTietTinhHuong([FromBody] GetChiTietTinhHuongParameter request)
        {
            return this._iCustomerDataAccess.GetChiTietTinhHuong(request);
        }
    }
}