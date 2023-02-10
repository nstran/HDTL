using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Contract;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Asset;
using TN.TNM.DataAccess.Messages.Results.CustomerCare;
using TN.TNM.DataAccess.Messages.Results.DataType;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.Api.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployee _iEmployee;
        private readonly IEmployeeDataAccess _iEmployeeDataAccess;

        public EmployeeController(IEmployee iEmployee, IEmployeeDataAccess iEmployeeDataAccess)
        {
            this._iEmployee = iEmployee;
            _iEmployeeDataAccess = iEmployeeDataAccess;
        }

        /// <summary>
        /// Create a new employee
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/create")]
        [Authorize(Policy = "Member")]
        public CreateEmployeeResult CreateEmployee([FromBody]CreateEmployeeParameter request)
        {
            return this._iEmployeeDataAccess.CreateEmployee(request);
        }

        /// <summary>
        /// Search employee
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/search")]
        [Authorize(Policy = "Member")]
        public SearchEmployeeResult SearchEmployee([FromBody]SearchEmployeeParameter request)
        {
            return this._iEmployeeDataAccess.SearchEmployee(request);
        }

        /// <summary>
        /// Search employee
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getAllEmployee")]
        [Authorize(Policy = "Member")]
        public GetAllEmployeeResult GetAllEmployee([FromBody]GetAllEmployeeParameter request)
        {
            return this._iEmployeeDataAccess.GetAllEmployee(request);
        }

        /// <summary>
        /// Get employee by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getEmployeeById")]
        [Authorize(Policy = "Member")]
        public GetEmployeeByIdResult GetEmployeeById([FromBody]GetEmployeeByIdParameter request)
        {
            return this._iEmployeeDataAccess.GetEmployeeById(request);
        }

        /// <summary>
        /// Edit employee by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/editEmployeeById")]
        [Authorize(Policy = "Member")]
        public EditEmployeeByIdResult EditEmployeeById([FromBody]EditEmployeeByIdParameter request)
        {
            return this._iEmployeeDataAccess.EditEmployeeById(request);
        }

        /// <summary>
        /// Get All Emp Account
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getAllEmpAccount")]
        [Authorize(Policy = "Member")]
        public GetAllEmpAccountResult GetAllEmpAccount([FromBody]GetAllEmpAccountParameter request)
        {
            return this._iEmployeeDataAccess.GetAllEmpAccount(request);
        }

        /// <summary>
        /// Get All Employee Account
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getAllEmployeeAccount")]
        [Authorize(Policy = "Member")]
        public GetAllEmployeeAccountResult GetAllEmployeeAccount()
        {
            return this._iEmployeeDataAccess.GetAllEmployeeAccount();
        }

        /// <summary>
        /// Get All Emp Account
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getAllEmpIdentity")]
        [Authorize(Policy = "Member")]
        public GetAllEmpIdentityResult GetAllEmpIdentity([FromBody]GetAllEmpIdentityParameter request)
        {
            return this._iEmployeeDataAccess.GetAllEmpIdentity(request);
        }

        /// <summary>
        /// Get All Emp Account
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/editEmployeeDataPermission")]
        [Authorize(Policy = "Member")]
        public EditEmployeeDataPermissionResult EditEmployeeDataPermission([FromBody]EditEmployeeDataPermissionParameter request)
        {
            return this._iEmployeeDataAccess.EditEmployeeDataPermission(request);
        }

        /// <summary>
        /// Grand employee module permisison
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/employeePermissionMapping")]
        [Authorize(Policy = "Member")]
        public EmployeePermissionMappingResult EmployeePermissionMapping([FromBody]EmployeePermissionMappingParameter request)
        {
            return this._iEmployeeDataAccess.EmployeePermissionMapping(request);
        }

        /// <summary>
        /// GetEmployeeByPositionCode
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getEmployeeByPositionCode")]
        [Authorize(Policy = "Member")]
        public GetEmployeeByPositionCodeResult GetEmployeeByPositionCode([FromBody]GetEmployeeByPositionCodeParameter request)
        {
            return this._iEmployeeDataAccess.GetEmployeeByPositionCode(request);
        }

        /// <summary>
        /// GetEmployeeApprove
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getEmployeeApprove")]
        [Authorize(Policy = "Member")]
        public GetEmployeeHighLevelByEmpIdResult GetEmployeeHighLevelByEmpId([FromBody]GetEmployeeHighLevelByEmpIdParameter request)
        {
            return this._iEmployeeDataAccess.GetEmployeeHighLevelByEmpId(request);
        }

        /// <summary>
        /// GetEmployeeByOrganizationId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getEmployeeByOrganizationId")]
        [Authorize(Policy = "Member")]
        public GetEmployeeByOrganizationIdResult GetEmployeeByOrganizationId([FromBody]GetEmployeeByOrganizationIdParameter request)
        {
            return this._iEmployeeDataAccess.GetEmployeeByOrganizationId(request);
        }

        /// <summary>
        /// GetEmployeeByTopRevenue
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getEmployeeByTopRevenue")]
        [Authorize(Policy = "Member")]
        public GetEmployeeByTopRevenueResult GetEmployeeByTopRevenue([FromBody]GetEmployeeByTopRevenueParameter request)
        {
            return this._iEmployeeDataAccess.GetEmployeeByTopRevenue(request);
        }

        /// <summary>
        /// ExportEmployeeRevenue
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/exportEmployeeRevenue")]
        [Authorize(Policy = "Member")]
        public ExportEmployeeRevenueResult ExportEmployeeRevenue([FromBody]ExportEmployeeRevenueParameter request)
        {
            return this._iEmployeeDataAccess.ExportEmployeeRevenue(request);
        }

        /// <summary>
        /// GetStatisticForEmpDashBoard
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getStatisticForEmpDashBoard")]
        [Authorize(Policy = "Member")]
        public GetStatisticForEmpDashBoardResult GetStatisticForEmpDashBoard([FromBody]GetStatisticForEmpDashBoardParameter request)
        {
            return this._iEmployeeDataAccess.GetStatisticForEmpDashBoard(request);
        }

        /// <summary>
        /// GetEmployeeCareStaff
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getEmployeeCareStaff")]
        [Authorize(Policy = "Member")]
        public GetEmployeeCareStaffResult GetEmployeeCareStaff([FromBody]GetEmployeeCareStaffParameter request)
        {
            return this._iEmployeeDataAccess.GetEmployeeCareStaff(request);
        }

        /// <summary>
        /// Search employee from list
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/searchFromList")]
        [Authorize(Policy = "Member")]
        public SearchEmployeeFromListResult SearchEmployeeFromList([FromBody]SearchEmployeeFromListParameter request)
        {
            return this._iEmployeeDataAccess.SearchEmployeeFromList(request);
        }

        /// <summary>
        /// Get All Employee Account Identity
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getAllEmpAccIdentity")]
        [Authorize(Policy = "Member")]
        public GetAllEmpAccIdentityResult GetAllEmpAccIdentity([FromBody]GetAllEmpAccIdentityParameter request)
        {
            return this._iEmployeeDataAccess.GetAllEmpAccIdentity(request);
        }

        /// <summary>
        /// Disable Employee
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/disableEmployee")]
        [Authorize(Policy = "Member")]
        public DisableEmployeeResult DisableEmployee([FromBody]DisableEmployeeParameter request)
        {
            return this._iEmployeeDataAccess.DisableEmployee(request);
        }

        /// <summary>
        /// CheckAdminLogin
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/checkAdminLogin")]
        [Authorize(Policy = "Member")]
        public CheckAdminLoginResult CheckAdminLogin([FromBody]CheckAdminLoginParameter request)
        {
            return this._iEmployeeDataAccess.CheckAdminLogin(request);
        }

        //
        [HttpPost]
        [Route("api/employee/getMasterDataEmployeeDetail")]
        [Authorize(Policy = "Member")]
        public GetMasterDataEmployeeDetailResult GetMasterDataEmployeeDetail([FromBody] GetMasterDataEmployeeDetailParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataEmployeeDetail(request);
        }

        //
        [HttpPost]
        [Route("api/employee/getThongTinCaNhanThanhVien")]
        [Authorize(Policy = "Member")]
        public GetThongTinCaNhanThanhVienResult GetThongTinCaNhanThanhVien([FromBody] GetThongTinCaNhanThanhVienParameter request)
        {
            return this._iEmployeeDataAccess.GetThongTinCaNhanThanhVien(request);
        }

        //
        [HttpPost]
        [Route("api/employee/getThongTinChungThanhVien")]
        [Authorize(Policy = "Member")]
        public GetThongTinChungThanhVienResult GetThongTinChungThanhVien([FromBody] GetThongTinChungThanhVienParameter request)
        {
            return this._iEmployeeDataAccess.GetThongTinChungThanhVien(request);
        }

        //
        [HttpPost]
        [Route("api/employee/saveThongTinChungThanhVien")]
        [Authorize(Policy = "Member")]
        public SaveThongTinChungThanhVienResult SaveThongTinChungThanhVien([FromBody] SaveThongTinChungThanhVienParameter request)
        {
            return this._iEmployeeDataAccess.SaveThongTinChungThanhVien(request);
        }

        //
        [HttpPost]
        [Route("api/employee/saveThongTinCaNhanThanhVien")]
        [Authorize(Policy = "Member")]
        public SaveThongTinCaNhanThanhVienResult SaveThongTinCaNhanThanhVien([FromBody] SaveThongTinCaNhanThanhVienParameter request)
        {
            return this._iEmployeeDataAccess.SaveThongTinCaNhanThanhVien(request);
        }

        //
        [HttpPost]
        [Route("api/employee/getCauHinhPhanQuyen")]
        [Authorize(Policy = "Member")]
        public GetCauHinhPhanQuyenResult GetCauHinhPhanQuyen([FromBody] GetCauHinhPhanQuyenParameter request)
        {
            return this._iEmployeeDataAccess.GetCauHinhPhanQuyen(request);
        }

        //
        [HttpPost]
        [Route("api/employee/saveCauHinhPhanQuyen")]
        [Authorize(Policy = "Member")]
        public SaveCauHinhPhanQuyenResult SaveCauHinhPhanQuyen([FromBody] SaveCauHinhPhanQuyenParameter request)
        {
            return this._iEmployeeDataAccess.SaveCauHinhPhanQuyen(request);
        }

        //
        [HttpPost]
        [Route("api/employee/getThongTinNhanSu")]
        [Authorize(Policy = "Member")]
        public GetThongTinNhanSuResult GetThongTinNhanSu([FromBody] GetThongTinNhanSuParameter request)
        {
            return this._iEmployeeDataAccess.GetThongTinNhanSu(request);
        }

        //
        [HttpPost]
        [Route("api/employee/saveThongTinNhanSu")]
        [Authorize(Policy = "Member")]
        public SaveThongTinNhanSuResult SaveThongTinNhanSu([FromBody] SaveThongTinNhanSuParameter request)
        {
            return this._iEmployeeDataAccess.SaveThongTinNhanSu(request);
        }

        //
        [HttpPost]
        [Route("api/employee/getThongTinLuongVaTroCap")]
        [Authorize(Policy = "Member")]
        public GetThongTinLuongVaTroCapResult GetThongTinLuongVaTroCap([FromBody] GetThongTinLuongVaTroCapParameter request)
        {
            return this._iEmployeeDataAccess.GetThongTinLuongVaTroCap(request);
        }

        //
        [HttpPost]
        [Route("api/employee/saveThongTinLuongVaTroCap")]
        [Authorize(Policy = "Member")]
        public SaveThongTinLuongVaTroCapResult SaveThongTinLuongVaTroCap([FromBody] SaveThongTinLuongVaTroCapParameter request)
        {
            return this._iEmployeeDataAccess.SaveThongTinLuongVaTroCap(request);
        }

        //
        [HttpPost]
        [Route("api/employee/getThongTinGhiChu")]
        [Authorize(Policy = "Member")]
        public GetThongTinGhiChuResult GetThongTinGhiChu([FromBody] GetThongTinGhiChuParameter request)
        {
            return this._iEmployeeDataAccess.GetThongTinGhiChu(request);
        }

        //
        [HttpPost]
        [Route("api/employee/resetPassword")]
        [Authorize(Policy = "Member")]
        public ResetPasswordResult ResetPassword([FromBody] ResetPasswordParameter request)
        {
            return this._iEmployeeDataAccess.ResetPassword(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterDataDashboard")]
        [Authorize(Policy = "Member")]
        public GetMasterDataDashboardResult GetMasterDataDashboard([FromBody] GetMasterDataDashboardParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataDashboard(request);
        }

        [HttpPost]
        [Route("api/employee/createRecruitmentCampaign")]
        [Authorize(Policy = "Member")]
        public CreateRecruitmentCampaignResult CreateRecruitmentCampaign([FromForm] CreateRecruitmentCampaignParameter request)
        {
            return this._iEmployeeDataAccess.CreateRecruitmentCampaign(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterSearchRecruitmentCampaign")]
        [Authorize(Policy = "Member")]
        public GetMasterSearchRecruitmentCampaignResult GetMasterSearchRecruitmentCampaign([FromBody] GetMasterSearchRecruitmentCampaignParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterSearchRecruitmentCampaign(request);
        }

        [HttpPost]
        [Route("api/employee/deleteRecruitmentCampaign")]
        [Authorize(Policy = "Member")]
        public DeleteRecruitmentCampaignResult DeleteRecruitmentCampaign([FromBody] DeleteRecruitmentCampaignParameter request)
        {
            return this._iEmployeeDataAccess.DeleteRecruitmentCampaign(request);
        }

        [HttpPost]
        [Route("api/employee/searchRecruitmentCampaign")]
        [Authorize(Policy = "Member")]
        public SearchRecruitmentCampaignResult SearchRecruitmentCampaign([FromBody] SearchRecruitmentCampaignParameter request)
        {
            return this._iEmployeeDataAccess.SearchRecruitmentCampaign(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterRecruitmentCampaignDetail")]
        [Authorize(Policy = "Member")]
        public GetMasterRecruitmentCampaignDetailResult GetMasterRecruitmentCampaignDetail([FromBody] GetMasterRecruitmentCampaignDetailParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterRecruitmentCampaignDetail(request);
        }

        [HttpPost]
        [Route("api/employee/uploadFile")]
        [AllowAnonymous]
        public UploadFileVacanciesResult UploadFile([FromForm] UploadFileParameter request)
        {
            return this._iEmployeeDataAccess.UploadFile(request);
        }

        [HttpPost]
        [Route("api/employee/createVacancies")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateVacanciesResult CreateOrUpdateVacancies([FromForm] CreateOrUpdateVacanciesParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrUpdateVacancies(request);
        }

        [HttpPost]
        [Route("api/employee/createCandidate")]
        [Authorize(Policy = "Member")]
        public CreateCandidateResult CreateCandidate([FromForm] CreateCandidateParameter request)
        {
            return this._iEmployeeDataAccess.CreateCandidate(request);
        }

        [HttpPost]
        [Route("api/employee/getAllVacancies")]
        [Authorize(Policy = "Member")]
        public GetAllVacanciesResult GetAllVacancies([FromBody] GetAllVacanciesParameter request)
        {
            return this._iEmployeeDataAccess.GetAllVacancies(request);
        }


        [HttpPost]
        [Route("api/employee/deleteVacanciesById")]
        [Authorize(Policy = "Member")]
        public DeleteVacanciesByIdResult DeleteVacanciesById([FromBody] DeleteVacanciesByIdParameter request)
        {
            return this._iEmployeeDataAccess.DeleteVacanciesById(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterDataRecruitmentCampaignInformation")]
        [Authorize(Policy = "Member")]
        public GetMasterDataRecruitmentCampaignInformationResult GetMasterDataRecruitmentCampaignInformation([FromBody] GetMasterDataRecruitmentCampaignInformationParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataRecruitmentCampaignInformation(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterDataBaoCaoTuyenDung")]
        [Authorize(Policy = "Member")]
        public GetMasterDataBaoCaoTuyenDungResult GetMasterDataBaoCaoTuyenDung([FromBody] GetMasterDataBaoCaoTuyenDungParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataBaoCaoTuyenDung(request);

        }

        [HttpPost]
        [Route("api/employee/getBaoCaoBaoCaoTuyenDung")]
        [Authorize(Policy = "Member")]
        public GetBaoCaoBaoCaoTuyenDungResult GetBaoCaoBaoCaoTuyenDung([FromBody] GetBaoCaoBaoCaoTuyenDungParameter request)
        {
            return this._iEmployeeDataAccess.GetBaoCaoBaoCaoTuyenDung(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterDataVacanciesCreate")]
        [Authorize(Policy = "Member")]
        public GetMasterDataVacanciesCreateResult GetMasterDataVacanciesCreate([FromBody] GetMasterDataVacanciesCreateParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataVacanciesCreate(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterDataVacanciesDetail")]
        [Authorize(Policy = "Member")]
        public GetMasterDataVacanciesDetailResult GetMasterDataVacanciesDetail([FromBody] GetMasterDataVacanciesDetailParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataVacanciesDetail(request);
        }

        [HttpPost]
        [Route("api/employee/updateVacancies")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateVacanciesResult UpdateVacancies([FromBody] CreateOrUpdateVacanciesParameter request)
        {
            return this._iEmployeeDataAccess.UpdateVacancies(request);
        }

        [HttpPost]
        [Route("api/employee/updateStatusCandidateFromVacancies")]
        [Authorize(Policy = "Member")]
        public UpdateCandidateStatusFromVacanciesResult UpdateStatusCandidateFromVacancies([FromBody] UpdateCandidateStatusFromVacanciesParameter request)
        {
            return this._iEmployeeDataAccess.UpdateStatusCandidateFromVacancies(request);
        }

        [HttpPost]
        [Route("api/employee/createInterviewSchedule")]
        [AllowAnonymous]
        public CreateInterviewScheduleResult CreateInterviewSchedule([FromForm] CreateInterviewScheduleParameter request)
        {
            return this._iEmployeeDataAccess.CreateInterviewSchedule(request);
        }

        [HttpPost]
        [Route("api/employee/updateCandidate")]
        [AllowAnonymous]
        public UpdateCandidateResult UpdateCandidate([FromForm] UpdateCandidateParameter request)
        {
            return this._iEmployeeDataAccess.UpdateCandidate(request);
        }

        [HttpPost]
        [Route("api/employee/deleteCandidates")]
        [AllowAnonymous]
        public DeleteCandidatesResult DeleteCandidates([FromBody] DeleteCandidatesParameter request)
        {
            return this._iEmployeeDataAccess.DeleteCandidates(request);
        }

        [HttpPost]
        [Route("api/employee/downloadTemplateImportCandidate")]
        [AllowAnonymous]
        public DownloadTemplateImportResult DownloadTemplateImportCandidate([FromBody] DownloadTemplateImportParameter request)
        {
            return this._iEmployeeDataAccess.DownloadTemplateImportCandidate(request);
        }

        [HttpPost]
        [Route("api/employee/filterVacancies")]
        [Authorize(Policy = "Member")]
        public GetAllVacanciesResult FilterVacancies([FromBody] FilterVacanciesParameter request)
        {
            return this._iEmployeeDataAccess.FilterVacancies(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterCreateCandidate")]
        [Authorize(Policy = "Member")]
        public GetMasterCreateCandidateResult GetMasterCreateCandidate([FromBody] GetMasterCreateCandidateParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterCreateCandidate(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterCandidateDetail")]
        [Authorize(Policy = "Member")]
        public GetMasterCandidateDetailResult GetMasterCandidateDetail([FromBody] GetMasterCandidateDetailParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterCandidateDetail(request);
        }

        [HttpPost]
        [Route("api/employee/deleteCandidate")]
        [Authorize(Policy = "Member")]
        public DeleteCandidateResult DeleteCandidate([FromBody] DeleteCandidateParameter request)
        {
            return this._iEmployeeDataAccess.DeleteCandidate(request);
        }

        [HttpPost]
        [Route("api/employee/updateCandidateStatus")]
        [Authorize(Policy = "Member")]
        public UpdateCandidateStatusResult UpdateCandidateStatus([FromBody] UpdateCandidateStatusParameter request)
        {
            return this._iEmployeeDataAccess.UpdateCandidateStatus(request);
        }


        [HttpPost]
        [Route("api/employee/deleteCandidateAssessment")]
        [Authorize(Policy = "Member")]
        public DeleteCandidateAssessmentResult DeleteCandidateAssessment([FromBody] DeleteCandidateAssessmentParameter request)
        {
            return this._iEmployeeDataAccess.DeleteCandidateAssessment(request);
        }

        [HttpPost]
        [Route("api/employee/createOrUpdateCandidateAssessment")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCandidateAssessmentResult CreateOrUpdateCandidateAssessment([FromBody] CreateOrUpdateCandidateAssessmentParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrUpdateCandidateAssessment(request);
        }

        [HttpPost]
        [Route("api/employee/createOrUpdateCandidateDetailInfor")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCandidateDetailInforResult CreateOrUpdateCandidateDetailInfor([FromBody] CreateOrUpdateCandidateDetailInforParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrUpdateCandidateDetailInfor(request);
        }

        [HttpPost]
        [Route("api/employee/updateInterviewSchedule")]
        [Authorize(Policy = "Member")]
        public UpdateInterviewScheduleResult UpdateInterviewSchedule([FromBody] UpdateInterviewScheduleParameter request)
        {
            return this._iEmployeeDataAccess.UpdateInterviewSchedule(request);
        }

        [HttpPost]
        [Route("api/employee/createOrUpdateQuiz")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateQuizResult CreateOrUpdateQuiz([FromBody] CreateOrUpdateQuizParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrUpdateQuiz(request);
        }

        [HttpPost]
        [Route("api/employee/deleteCandidateDetailInfor")]
        [Authorize(Policy = "Member")]
        public DeleteCandidateDetailInforResult DeleteCandidateDetailInfor([FromBody] DeleteCandidateDetailInforParameter request)
        {
            return this._iEmployeeDataAccess.DeleteCandidateDetailInfor(request);
        }

        [HttpPost]
        [Route("api/employee/deleteInterviewSchedule")]
        [Authorize(Policy = "Member")]
        public DeleteInterviewScheduleResult DeleteInterviewSchedule([FromBody] DeleteInterviewScheduleParameter request)
        {
            return this._iEmployeeDataAccess.DeleteInterviewSchedule(request);
        }

        [HttpPost]
        [Route("api/employee/deleteQuiz")]
        [Authorize(Policy = "Member")]
        public DeleteQuizResult DeleteQuiz([FromBody] DeleteQuizParameter request)
        {
            return this._iEmployeeDataAccess.DeleteQuiz(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterSearchCandidate")]
        [Authorize(Policy = "Member")]
        public GetMasterSearchCandidateResult GetMasterSearchCandidate([FromBody] GetMasterSearchCandidateParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterSearchCandidate(request);
        }

        [HttpPost]
        [Route("api/employee/searchCandidate")]
        [Authorize(Policy = "Member")]
        public SearchCandidateResult SearchCandidate([FromBody] SearchCandidateParameter request)
        {
            return this._iEmployeeDataAccess.SearchCandidate(request);
        }

        [HttpPost]
        [Route("api/employee/sendEmailInterview")]
        [Authorize(Policy = "Member")]
        public SendEmailInterviewResult SendEmailInterview([FromForm] SendEmailInterviewParameter request)
        {
            return this._iEmployeeDataAccess.SendEmailInterview(request);
        }


        [HttpPost]
        [Route("api/employee/getCandidateImportDetai")]
        [Authorize(Policy = "Member")]
        public GetCandidateImportDetaiResult GetCandidateImportDetai([FromBody] GetCandidateImportDetaiParameter request)
        {
            return this._iEmployeeDataAccess.GetCandidateImportDetai(request);
        }

        [HttpPost]
        [Route("api/employee/importListCandidate")]
        [Authorize(Policy = "Member")]
        public ImportListCandidateResult ImportListCandidate([FromBody] ImportListCandidateParameter request)
        {
            return this._iEmployeeDataAccess.ImportListCandidate(request);
        }










        [HttpPost]
        [Route("api/employee/GetMasterDataCreateEmployee")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateEmployeeResult GetMasterDataCreateEmployee([FromBody] GetMasterDataCreateEmployeeParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataCreateEmployee(request);
        }

        [HttpPost]
        [Route("api/employee/SaveThongTinKhac")]
        [Authorize(Policy = "Member")]
        public SaveThongTinKhacResult SaveThongTinKhac([FromBody] SaveThongTinKhacParameter request)
        {
            return this._iEmployeeDataAccess.SaveThongTinKhac(request);
        }

        [HttpPost]
        [Route("api/employee/GetThongTinKhac")]
        [Authorize(Policy = "Member")]
        public GetThongTinKhacResult GetThongTinKhac([FromBody] GetThongTinKhacParameter request)
        {
            return this._iEmployeeDataAccess.GetThongTinKhac(request);
        }

        [HttpPost]
        [Route("api/employee/GetThongTinGiaDinh")]
        [Authorize(Policy = "Member")]
        public GetThongTinGiaDinhResult GetThongTinGiaDinh([FromBody] GetThongTinGiaDinhParameter request)
        {
            return this._iEmployeeDataAccess.GetThongTinGiaDinh(request);
        }

        [HttpPost]
        [Route("api/employee/CreateOrUpdateThongTinGiaDinh")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateThongTinGiaDinhResult CreateOrUpdateThongTinGiaDinh([FromBody] CreateOrUpdateThongTinGiaDinhParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrUpdateThongTinGiaDinh(request);
        }

        [HttpPost]
        [Route("api/employee/GetCheckListTaiLieu")]
        [Authorize(Policy = "Member")]
        public GetCheckListTaiLieuResult GetCheckListTaiLieu([FromBody] GetCheckListTaiLieuParameter request)
        {
            return this._iEmployeeDataAccess.GetCheckListTaiLieu(request);
        }

        [HttpPost]
        [Route("api/employee/GetMasterDataCreateDeXuatTangLuong")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateDeXuatTangLuongResult GetMasterDataCreateDeXuatTangLuong([FromBody] GetMasterDataCreateDeXuatTangLuongParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataCreateDeXuatTangLuong(request);
        }

        [HttpPost]
        [Route("api/employee/TaoDeXuatTangLuong")]
        [Authorize(Policy = "Member")]
        public TaoDeXuatTangLuongResult TaoDeXuatTangLuong([FromForm] TaoDeXuatTangLuongParameter request)
        {
            return this._iEmployeeDataAccess.TaoDeXuatTangLuong(request);
        }

        [HttpPost]
        [Route("api/employee/createHopDongNhanSu")]
        [Authorize(Policy = "Member")]
        public CreateHopDongNhanSuResult CreateHopDongNhanSu([FromBody] CreateHopDongNhanSuParameter request)
        {
            return this._iEmployeeDataAccess.CreateHopDongNhanSu(request);
        }

        [HttpPost]
        [Route("api/employee/getHopDongNhanSuById")]
        [Authorize(Policy = "Member")]
        public GetHopDongNhanSuByIdResult GetHopDongNhanSuById([FromBody] GetHopDongNhanSuByIdParameter request)
        {
            return this._iEmployeeDataAccess.GetHopDongNhanSuById(request);
        }

        [HttpPost]
        [Route("api/employee/getListHopDongNhanSu")]
        [Authorize(Policy = "Member")]
        public GetListHopDongNhanSuResult GetListHopDongNhanSu([FromBody] GetListHopDongNhanSuParameter request)
        {
            return this._iEmployeeDataAccess.GetListHopDongNhanSu(request);
        }

        [HttpPost]
        [Route("api/employee/updateHopDongNhanSu")]
        [Authorize(Policy = "Member")]
        public UpdateHopDongNhanSuResult UpdateHopDongNhanSu([FromBody] UpdateHopDongNhanSuParameter request)
        {
            return this._iEmployeeDataAccess.UpdateHopDongNhanSu(request);
        }

        [HttpPost]
        [Route("api/employee/deleteHopDongNhanSuById")]
        [Authorize(Policy = "Member")]
        public DeleteHopDongNhanSuByIdResult DeleteHopDongNhanSuById([FromBody] DeleteHopDongNhanSuByIdParameter request)
        {
            return this._iEmployeeDataAccess.DeleteHopDongNhanSuById(request);
        }

        [HttpPost]
        [Route("api/employee/getLichSuHopDongNhanSu")]
        [Authorize(Policy = "Member")]
        public GetLichSuHopDongNhanSuResult GetLichSuHopDongNhanSu([FromBody] GetLichSuHopDongNhanSuParameter request)
        {
            return this._iEmployeeDataAccess.GetLichSuHopDongNhanSu(request);
        }

        [HttpPost]
        [Route("api/employee/DeXuatTangLuongDetail")]
        [Authorize(Policy = "Member")]
        public DeXuatTangLuongDetailResult DeXuatTangLuongDetail([FromBody] DeXuatTangLuongDetailParameter request)
        {
            return this._iEmployeeDataAccess.DeXuatTangLuongDetail(request);
        }

        [HttpPost]
        [Route("api/employee/ListDeXuatTangLuong")]
        [Authorize(Policy = "Member")]
        public ListDeXuatTangLuongResult ListDeXuatTangLuong([FromBody] ListDeXuatTangLuongParameter request)
        {
            return this._iEmployeeDataAccess.ListDeXuatTangLuong(request);
        }


        [HttpPost]
        [Route("api/employee/getLichSuThayDoiChucVu")]
        [Authorize(Policy = "Member")]
        public GetLichSuThayDoiChucVuResult GetLichSuThayDoiChucVu([FromBody] GetLichSuThayDoiChucVuParameter request)
        {
            return this._iEmployeeDataAccess.GetLichSuThayDoiChucVu(request);
        }

        [HttpPost]
        [Route("api/employee/getThongTinBaoHiemVaThue")]
        [Authorize(Policy = "Member")]
        public GetThongTinBaoHiemVaThueResult GetThongTinBaoHiemVaThue([FromBody] GetThongTinBaoHiemVaThueParameter request)
        {
            return this._iEmployeeDataAccess.GetThongTinBaoHiemVaThue(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterCauHinhBaoHiem")]
        [Authorize(Policy = "Member")]
        public GetMasterCauHinhBaoHiemResult GetMasterCauHinhBaoHiem([FromBody] GetMasterCauHinhBaoHiemParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterCauHinhBaoHiem(request);
        }

        [HttpPost]
        [Route("api/employee/createOrUpdateCauHinhBaoHiemLoftCare")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCauHinhBaoHiemLoftCareResult CreateOrUpdateCauHinhBaoHiemLoftCare([FromBody] CreateOrUpdateCauHinhBaoHiemLoftCareParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrUpdateCauHinhBaoHiemLoftCare(request);
        }

        [HttpPost]
        [Route("api/employee/getCauHinhBaoHiemLoftCareById")]
        [Authorize(Policy = "Member")]
        public GetCauHinhBaoHiemLoftCareByIdResult GetCauHinhBaoHiemLoftCareById([FromBody] GetCauHinhBaoHiemLoftCareByIdParameter request)
        {
            return this._iEmployeeDataAccess.GetCauHinhBaoHiemLoftCareById(request);
        }

        [HttpPost]
        [Route("api/employee/deleteCauHinhBaoHiemLoftCare")]
        [Authorize(Policy = "Member")]
        public DeleteCauHinhBaoHiemLoftCareResult DeleteCauHinhBaoHiemLoftCare([FromBody] DeleteCauHinhBaoHiemLoftCareParameter request)
        {
            return this._iEmployeeDataAccess.DeleteCauHinhBaoHiemLoftCare(request);
        }

        [HttpPost]
        [Route("api/employee/createOrUpdateCauHinhBaoHiem")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCauHinhBaoHiemResult CreateOrUpdateCauHinhBaoHiem([FromBody] CreateOrUpdateCauHinhBaoHiemParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrUpdateCauHinhBaoHiem(request);
        }

        [HttpPost]
        [Route("api/employee/getCauHinhBaoHiemById")]
        [Authorize(Policy = "Member")]
        public GetCauHinhBaoHiemByIdResult GetCauHinhBaoHiemById([FromBody] GetCauHinhBaoHiemByIdParameter request)
        {
            return this._iEmployeeDataAccess.GetCauHinhBaoHiemById(request);
        }

        [HttpPost]
        [Route("api/employee/saveThongTinBaoHiemVaThue")]
        [Authorize(Policy = "Member")]
        public SaveThongTinBaoHiemVaThueResult SaveThongTinBaoHiemVaThue([FromBody] SaveThongTinBaoHiemVaThueParameter request)
        {
            return this._iEmployeeDataAccess.SaveThongTinBaoHiemVaThue(request);
        }

        [HttpPost]
        [Route("api/employee/getListTaiLieuNhanVien")]
        [Authorize(Policy = "Member")]
        public GetListTaiLieuNhanVienResult GetListTaiLieuNhanVien([FromBody] GetListTaiLieuNhanVienParameter request)
        {
            return this._iEmployeeDataAccess.GetListTaiLieuNhanVien(request);
        }

        [HttpPost]
        [Route("api/employee/createTaiLieuNhanVien")]
        [Authorize(Policy = "Member")]
        public CreateTaiLieuNhanVienResult CreateTaiLieuNhanVien([FromBody] CreateTaiLieuNhanVienParameter request)
        {
            return this._iEmployeeDataAccess.CreateTaiLieuNhanVien(request);
        }

        [HttpPost]
        [Route("api/employee/tuChoiCheckListTaiLieu")]
        [Authorize(Policy = "Member")]
        public TuChoiCheckListTaiLieuResult TuChoiCheckListTaiLieu([FromBody] TuChoiCheckListTaiLieuParameter request)
        {
            return this._iEmployeeDataAccess.TuChoiCheckListTaiLieu(request);
        }

        [HttpPost]
        [Route("api/employee/xacNhanCheckListTaiLieu")]
        [Authorize(Policy = "Member")]
        public XacNhanCheckListTaiLieuResult XacNhanCheckListTaiLieu([FromBody] XacNhanCheckListTaiLieuParameter request)
        {
            return this._iEmployeeDataAccess.XacNhanCheckListTaiLieu(request);
        }

        [HttpPost]
        [Route("api/employee/yeuCauXacNhanCheckListTaiLieu")]
        [Authorize(Policy = "Member")]
        public YeuCauXacNhanCheckListTaiLieuResult YeuCauXacNhanCheckListTaiLieu([FromBody] YeuCauXacNhanCheckListTaiLieuParameter request)
        {
            return this._iEmployeeDataAccess.YeuCauXacNhanCheckListTaiLieu(request);
        }

        [HttpPost]
        [Route("api/employee/deleteTaiLieuNhanVienById")]
        [Authorize(Policy = "Member")]
        public DeleteTaiLieuNhanVienByIdResult DeleteTaiLieuNhanVienById([FromBody] DeleteTaiLieuNhanVienByIdParameter request)
        {
            return this._iEmployeeDataAccess.DeleteTaiLieuNhanVienById(request);
        }

        [HttpPost]
        [Route("api/employee/updateTaiLieuNhanVien")]
        [Authorize(Policy = "Member")]
        public UpdateTaiLieuNhanVienResult UpdateTaiLieuNhanVien([FromBody] UpdateTaiLieuNhanVienParameter request)
        {
            return this._iEmployeeDataAccess.UpdateTaiLieuNhanVien(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterCreateKeHoachOt")]
        [Authorize(Policy = "Member")]
        public GetMasterCreateKeHoachOtResult GetMasterCreateKeHoachOt([FromBody] GetMasterCreateKeHoachOtParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterCreateKeHoachOt(request);
        }

        [HttpPost]
        [Route("api/employee/createOrUpdateKeHoachOt")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateKeHoachOtResult createOrUpdateKeHoachOt([FromForm] CreateOrUpdateKeHoachOtParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrUpdateKeHoachOt(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterSearchKeHoachOt")]
        [Authorize(Policy = "Member")]
        public GetMasterSearchKeHoachOtResult getMasterSearchKeHoachOt([FromBody] GetMasterSearchKeHoachOtParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterSearchKeHoachOt(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterDataKeHoachOtDetail")]
        [Authorize(Policy = "Member")]
        public GetMasterDataKeHoachOtDetailResult getMasterDataKeHoachOtDetail([FromBody] GetMasterDataKeHoachOtDetailParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataKeHoachOtDetail(request);
        }

        [HttpPost]
        [Route("api/employee/GetMasterDataKeHoachOtPheDuyet")]
        [Authorize(Policy = "Member")]
        public GetMasterDataKeHoachOtPheDuyetResult getMasterDataKeHoachOtPheDuyet([FromBody] GetMasterDataKeHoachOtPheDuyetParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataKeHoachOtPheDuyet(request);
        }

        [HttpPost]
        [Route("api/employee/updateTrinhDoHocVanTuyenDung")]
        [Authorize(Policy = "Member")]
        public UpdateTrinhDoHocVanTuyenDungResult UpdateTrinhDoHocVanTuyenDung([FromBody] UpdateTrinhDoHocVanTuyenDungParameter request)
        {
            return this._iEmployeeDataAccess.UpdateTrinhDoHocVanTuyenDung(request);
        }

        [HttpPost]
        [Route("api/employee/getTrinhDoHocVanTuyenDung")]
        [Authorize(Policy = "Member")]
        public GetTrinhDoHocVanTuyenDungResult GetTrinhDoHocVanTuyenDung([FromBody] GetTrinhDoHocVanTuyenDungParameter request)
        {
            return this._iEmployeeDataAccess.GetTrinhDoHocVanTuyenDung(request);
        }

        [HttpPost]
        [Route("api/employee/deleteThongTinGiaDinhById")]
        [Authorize(Policy = "Member")]
        public DeleteThongTinGiaDinhByIdResult DeleteThongTinGiaDinhById([FromBody] DeleteThongTinGiaDinhByIdParameter request)
        {
            return this._iEmployeeDataAccess.DeleteThongTinGiaDinhById(request);
        }

        [HttpPost]
        [Route("api/employee/DeleteDeXuatTangLuong")]
        [Authorize(Policy = "Member")]
        public DeleteDeXuatTangLuongResult DeleteDeXuatTangLuong([FromBody] DeleteDeXuatTangLuongParameter request)
        {
            return this._iEmployeeDataAccess.DeleteDeXuatTangLuong(request);
        }

        [HttpPost]
        [Route("api/employee/UpdateDeXuatTangLuong")]
        [Authorize(Policy = "Member")]
        public UpdateDeXuatTangLuongResult UpdateDeXuatTangLuong([FromForm] UpdateDeXuatTangLuongParameter request)
        {
            return this._iEmployeeDataAccess.UpdateDeXuatTangLuong(request);
        }

        [HttpPost]
        [Route("api/employee/DatVeMoiDeXuatTangLuong")]
        [Authorize(Policy = "Member")]
        public DatVeMoiDeXuatTangLuongResult DatVeMoiDeXuatTangLuong([FromBody] DatVeMoiDeXuatTangLuongParameter request)
        {
            return this._iEmployeeDataAccess.DatVeMoiDeXuatTangLuong(request);
        }

        [HttpPost]
        [Route("api/employee/tuChoiOrPheDuyetNhanVienDeXuatTL")]
        [Authorize(Policy = "Member")]
        public TuChoiOrPheDuyetNhanVienDeXuatTLResult TuChoiOrPheDuyetNhanVienDeXuatTL([FromBody] TuChoiOrPheDuyetNhanVienDeXuatTLParameter request)
        {
            return this._iEmployeeDataAccess.TuChoiOrPheDuyetNhanVienDeXuatTL(request);
        }

        [HttpPost]
        [Route("api/employee/downloadTemplateImportDXTL")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateImportDXTLResult DownloadTemplateImportDXTL([FromBody] DownloadTemplateImportDXTLParameter request)
        {
            return this._iEmployeeDataAccess.DownloadTemplateImportDXTL(request);
        }

        [HttpPost]
        [Route("api/employee/GetMasterDataCreateDeXuatChucVu")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateDeXuatChucVuResult GetMasterDataCreateDeXuatChucVu([FromBody] GetMasterDataCreateDeXuatChucVuParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataCreateDeXuatChucVu(request);
        }

        [HttpPost]
        [Route("api/employee/taoDeXuatChucVu")]
        [Authorize(Policy = "Member")]
        public TaoDeXuatChucVuResult TaoDeXuatChucVu([FromForm] TaoDeXuatChucVuParameter request)
        {
            return this._iEmployeeDataAccess.TaoDeXuatChucVu(request);
        }

        [HttpPost]
        [Route("api/employee/downloadTemplateImportDXCV")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateImportDXCVResult DownloadTemplateImportDXCV([FromBody] DownloadTemplateImportDXCVParameter request)
        {
            return this._iEmployeeDataAccess.DownloadTemplateImportDXCV(request);
        }


        [HttpPost]
        [Route("api/employee/ListDeXuatChucVu")]
        [Authorize(Policy = "Member")]
        public ListDeXuatChucVuResult ListDeXuatChucVu([FromBody] ListDeXuatChucVuParameter request)
        {
            return this._iEmployeeDataAccess.ListDeXuatChucVu(request);
        }

        [HttpPost]
        [Route("api/employee/DeleteDeXuatChucVu")]
        [Authorize(Policy = "Member")]
        public DeleteDeXuatChucVuResult DeleteDeXuatChucVu([FromBody] DeleteDeXuatChucVuParameter request)
        {
            return this._iEmployeeDataAccess.DeleteDeXuatChucVu(request);
        }

        [HttpPost]
        [Route("api/employee/DeXuatChucVuDetail")]
        [Authorize(Policy = "Member")]
        public DeXuatChucVuDetailResult DeXuatChucVuDetail([FromBody] DeXuatChucVuDetailParameter request)
        {
            return this._iEmployeeDataAccess.DeXuatChucVuDetail(request);
        }

        [HttpPost]
        [Route("api/employee/DatVeMoiDeXuatChucVu")]
        [Authorize(Policy = "Member")]
        public DatVeMoiDeXuatChucVuResult DatVeMoiDeXuatChucVu([FromBody] DatVeMoiDeXuatChucVuParameter request)
        {
            return this._iEmployeeDataAccess.DatVeMoiDeXuatChucVu(request);
        }

        [HttpPost]
        [Route("api/employee/TuChoiOrPheDuyetNhanVienDeXuatCV")]
        [Authorize(Policy = "Member")]
        public TuChoiOrPheDuyetNhanVienDeXuatCVResult TuChoiOrPheDuyetNhanVienDeXuatCV([FromBody] TuChoiOrPheDuyetNhanVienDeXuatCVParameter request)
        {
            return this._iEmployeeDataAccess.TuChoiOrPheDuyetNhanVienDeXuatCV(request);
        }

        [HttpPost]
        [Route("api/employee/updateDeXuatChucVu")]
        [Authorize(Policy = "Member")]
        public UpdateDeXuatChucVuResult UpdateDeXuatChucVu([FromForm] UpdateDeXuatChucVuParameter request)
        {
            return this._iEmployeeDataAccess.UpdateDeXuatChucVu(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterDataHoSoCTForm")]
        [Authorize(Policy = "Member")]
        public GetMasterDataHoSoCTFormResult GetMasterDataHoSoCTForm([FromBody] GetMasterDataHoSoCTFormParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataHoSoCTForm(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterDeXuatCongTacDetail")]
        [Authorize(Policy = "Member")]
        public GetMasterDeXuatCongTacDetailResult GetMasterDeXuatCongTacDetail([FromBody] GetMasterDeXuatCongTacDetailParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDeXuatCongTacDetail(request);
        }

        [HttpPost]
        [Route("api/employee/createOrUpdateHoSoCT")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateHoSoCTResult CreateOrUpdateHoSoCT([FromForm] CreateOrUpdateHoSoCTParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrUpdateHoSoCT(request);
        }

        [HttpPost]
        [Route("api/employee/getAllHoSoCongTacList")]
        [Authorize(Policy = "Member")]
        public GetAllHoSoCongTacListResult GetAllHoSoCongTacList([FromBody] GetAllHoSoCongTacListParameter request)
        {
            return this._iEmployeeDataAccess.GetAllHoSoCongTacList(request);
        }

        [HttpPost]
        [Route("api/employee/xoaHoSoCongTac")]
        [Authorize(Policy = "Member")]
        public XoaHoSoCongTacResult XoaHoSoCongTac([FromBody] XoaHoSoCongTacParameter request)
        {
            return this._iEmployeeDataAccess.XoaHoSoCongTac(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterKeHoachOTDetail")]
        [Authorize(Policy = "Member")]
        public GetMasterKeHoachOTDetailResult GetMasterKeHoachOTDetail([FromBody] GetMasterKeHoachOTDetailParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterKeHoachOTDetail(request);
        }

        [HttpPost]
        [Route("api/employee/datVeMoiKeHoachOt")]
        [Authorize(Policy = "Member")]
        public DatVeMoiKeHoachOtResult DatVeMoiKeHoachOtOrDangKyOT([FromBody] DatVeMoiKeHoachOtParameter request)
        {
            return this._iEmployeeDataAccess.DatVeMoiKeHoachOt(request);
        }

        [HttpPost]
        [Route("api/employee/dangKyOTOrHuyDangKyOT")]
        [Authorize(Policy = "Member")]
        public DangKyOTOrHuyDangKyOTResult DangKyOTOrHuyDangKyOT([FromBody] DangKyOTOrHuyDangKyOTParameter request)
        {
            return this._iEmployeeDataAccess.DangKyOTOrHuyDangKyOT(request);
        }

        [HttpPost]
        [Route("api/employee/pheDuyetNhanSuDangKyOT")]
        [Authorize(Policy = "Member")]
        public PheDuyetNhanSuDangKyOTResult PheDuyetNhanSuDangKyOT([FromBody] PheDuyetNhanSuDangKyOTParameter request)
        {
            return this._iEmployeeDataAccess.PheDuyetNhanSuDangKyOT(request);
        }

        [HttpPost]
        [Route("api/employee/tuChoiNhanVienOTTongPheDuyet")]
        [Authorize(Policy = "Member")]
        public TuChoiNhanVienOTTongPheDuyetResult TuChoiNhanVienOTTongPheDuyet([FromBody] TuChoiNhanVienOTTongPheDuyetParameter request)
        {
            return this._iEmployeeDataAccess.TuChoiNhanVienOTTongPheDuyet(request);
        }


        [HttpPost]
        [Route("api/employee/giaHanThemKeHoachOT")]
        [Authorize(Policy = "Member")]
        public GiaHanThemKeHoachOTResult GiaHanThemKeHoachOT([FromBody] GiaHanThemKeHoachOTParameter request)
        {
            return this._iEmployeeDataAccess.GiaHanThemKeHoachOT(request);
        }


        [HttpPost]
        [Route("api/employee/deleteKehoachOT")]
        [Authorize(Policy = "Member")]
        public DeleteKehoachOTResult DeleteKehoachOT([FromBody] DeleteKehoachOTParameter request)
        {
            return this._iEmployeeDataAccess.DeleteKehoachOT(request);
        }


        [HttpPost]
        [Route("api/employee/getDataHoSoCTDetail")]
        [Authorize(Policy = "Member")]
        public GetDataHoSoCTDetailResult GetDataHoSoCTDetail([FromBody] GetDataDetailParameter request)
        {
            return this._iEmployeeDataAccess.GetDataHoSoCTDetail(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterDataDeNghiForm")]
        [Authorize(Policy = "Member")]
        public GetMasterDataDeNghiFormResult GetMasterDataDeNghiForm()
        {
            return this._iEmployeeDataAccess.GetMasterDataDeNghiForm();
        }

        [HttpPost]
        [Route("api/employee/createOrUpdateDeNghiTamHoanUng")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateDeNghiTamHoanUngResult CreateOrUpdateDeNghiTamHoanUng([FromForm] CreateOrUpdateDeNghiTamHoanUngParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrUpdateDeNghiTamHoanUng(request);
        }

        [HttpPost]
        [Route("api/employee/getDataDeNghiDetailForm")]
        [Authorize(Policy = "Member")]
        public GetDataDeNghiDetailFormResult GetDataDeNghiDetailForm([FromBody] GetDataDetailParameter request)
        {
            return this._iEmployeeDataAccess.GetDataDeNghiDetailForm(request);
        }

        [HttpPost]
        [Route("api/employee/updateDeNghiTamHoanUngChiTiet")]
        [Authorize(Policy = "Member")]
        public UpdateDeNghiTamHoanUngChiTietResult UpdateDeNghiTamHoanUngChiTiet([FromBody] UpdateDeNghiTamHoanUngChiTietParameter request)
        {
            return this._iEmployeeDataAccess.UpdateDeNghiTamHoanUngChiTiet(request);
        }

        [HttpPost]
        [Route("api/employee/deleteDeNghiTamHoanUngChiTiet")]
        [Authorize(Policy = "Member")]
        public DeleteDeNghiTamHoanUngChiTietResult DeleteDeNghiTamHoanUngChiTiet([FromBody] DeleteDeNghiTamHoanUngChiTietParameter request)
        {
            return this._iEmployeeDataAccess.DeleteDeNghiTamHoanUngChiTiet(request);
        }

        [HttpPost]
        [Route("api/employee/datVeMoiDeNghiTamHoanUng")]
        [Authorize(Policy = "Member")]
        public DatVeMoiDeNghiTamHoanUngResult DatVeMoiDeNghiTamHoanUng([FromBody] DatVeMoiDeNghiTamHoanUngParameter request)
        {
            return this._iEmployeeDataAccess.DatVeMoiDeNghiTamHoanUng(request);
        }

        [HttpPost]
        [Route("api/employee/xoaDeNghiTamHoanUng")]
        [Authorize(Policy = "Member")]
        public XoaDeNghiTamHoanUngResult XoaDeNghiTamHoanUng([FromBody] XoaDeNghiTamHoanUngParameter request)
        {
            return this._iEmployeeDataAccess.XoaDeNghiTamHoanUng(request);
        }

        [HttpPost]
        [Route("api/employee/getAllDenghiTamUngList")]
        [Authorize(Policy = "Member")]
        public GetAllDenghiTamUngListResult GetAllDenghiTamUngList([FromBody] GetAllDenghiTamUngListParameter request)
        {
            return this._iEmployeeDataAccess.GetAllDenghiTamUngList(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterDataCreateCauHinhDanhGia")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateCauHinhDanhGiaResult GetMasterDataCreateCauHinhDanhGia([FromBody] GetMasterDataCreateCauHinhDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataCreateCauHinhDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/updateCauHinhDanhGia")]
        [Authorize(Policy = "Member")]
        public UpdateCauHinhDanhGiaResult UpdateCauHinhDanhGia([FromBody] UpdateCauHinhDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.UpdateCauHinhDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/deleteCauHinhDanhGia")]
        [Authorize(Policy = "Member")]
        public DeleteCauHinhDanhGiaResult DeleteCauHinhDanhGia([FromBody] DeleteCauHinhDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.DeleteCauHinhDanhGia(request);
        }


        [HttpPost]
        [Route("api/employee/createCauHinhDanhGia")]
        [Authorize(Policy = "Member")]
        public CreateCauHinhDanhGiaResult CreateCauHinhDanhGia([FromBody] CreateCauHinhDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.CreateCauHinhDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/taoQuyLuong")]
        [Authorize(Policy = "Member")]
        public TaoQuyLuongResult TaoQuyLuong([FromBody] TaoQuyLuongParameter request)
        {
            return this._iEmployeeDataAccess.TaoQuyLuong(request);
        }

        [HttpPost]
        [Route("api/employee/updateQuyLuong")]
        [Authorize(Policy = "Member")]
        public TaoQuyLuongResult UpdateQuyLuong([FromBody] TaoQuyLuongParameter request)
        {
            return this._iEmployeeDataAccess.UpdateQuyLuong(request);
        }

        [HttpPost]
        [Route("api/employee/deleteQuyLuong")]
        [Authorize(Policy = "Member")]
        public TaoQuyLuongResult DeleteQuyLuong([FromBody] TaoQuyLuongParameter request)
        {
            return this._iEmployeeDataAccess.DeleteQuyLuong(request);
        }


        [HttpPost]
        [Route("api/employee/getMasterDataDeXuatCongTac")]
        [Authorize(Policy = "Member")]
        public GetMasterDataDeXuatCongTacResult GetMasterDataDeXuatCongTac([FromBody] GetMasterDataHoSoCTFormParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataDeXuatCongTac(request);
        }

        [HttpPost]
        [Route("api/employee/getListCauHinhChecklist")]
        [Authorize(Policy = "Member")]
        public GetListCauHinhChecklistResult GetListCauHinhChecklist([FromBody] GetListCauHinhChecklistParameter request)
        {
            return this._iEmployeeDataAccess.GetListCauHinhChecklist(request);
        }

        [HttpPost]
        [Route("api/employee/createOrUpdateCauHinhChecklist")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCauHinhChecklistResult CreateOrUpdateCauHinhChecklist([FromBody] CreateOrUpdateCauHinhChecklistParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrUpdateCauHinhChecklist(request);
        }

        [HttpPost]
        [Route("api/employee/deleteCauHinhChecklistById")]
        [Authorize(Policy = "Member")]
        public DeleteCauHinhChecklistByIdResult DeleteCauHinhChecklistById([FromBody] DeleteCauHinhChecklistByIdParameter request)
        {
            return this._iEmployeeDataAccess.DeleteCauHinhChecklistById(request);
        }

        [HttpPost]
        [Route("api/employee/createOrUpdateDeXuatCT")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateDeXuatCTResult CreateOrUpdateDeXuatCT([FromForm] CreateOrUpdateDeXuatCTParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrUpdateDeXuatCT(request);
        }

        [HttpPost]
        [Route("api/employee/xoaDeXuatCongTac")]
        [Authorize(Policy = "Member")]
        public XoaYeuCauCapPhatResult XoaDeXuatCongTac([FromBody] XoaDeXuatCongTacParameter request)
        {
            return this._iEmployeeDataAccess.XoaDeXuatCongTac(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterDataCreatePhieuDanhGia")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreatePhieuDanhGiaResult GetMasterDataCreatePhieuDanhGia([FromBody] GetMasterDataCreatePhieuDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataCreatePhieuDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/taoPhieuDanhGia")]
        [Authorize(Policy = "Member")]
        public TaoPhieuDanhGiaResult TaoPhieuDanhGia([FromForm] TaoPhieuDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.TaoPhieuDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/danhSachPhieuDanhGia")]
        [Authorize(Policy = "Member")]
        public DanhSachPhieuDanhGiaResult DanhSachPhieuDanhGia([FromBody] DanhSachPhieuDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.DanhSachPhieuDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/deletePhieuDanhGia")]
        [Authorize(Policy = "Member")]
        public DeletePhieuDanhGiaResult DeletePhieuDanhGia([FromBody] DeletePhieuDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.DeletePhieuDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/getAllDeXuatCongTac")]
        [Authorize(Policy = "Member")]
        public GetAllDeXuatCongTacResult GetAllDeXuatCongTac([FromBody] GetAllDeXuatCongTacParameter request)
        {
            return this._iEmployeeDataAccess.GetAllDeXuatCongTac(request);
        }

        [HttpPost]
        [Route("api/employee/danhSachKyDanhGia")]
        [Authorize(Policy = "Member")]
        public DanhSachKyDanhGiaResult DanhSachKyDanhGia([FromBody] DanhSachKyDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.DanhSachKyDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/phieuDanhGiaDetail")]
        [Authorize(Policy = "Member")]
        public PhieuDanhGiaDetailResult PhieuDanhGiaDetail([FromBody] PhieuDanhGiaDetailParameter request)
        {
            return this._iEmployeeDataAccess.PhieuDanhGiaDetail(request);
        }

        [HttpPost]
        [Route("api/employee/capNhatPhieuDanhGia")]
        [Authorize(Policy = "Member")]
        public CapNhatPhieuDanhGiaResult CapNhatPhieuDanhGia([FromForm] CapNhatPhieuDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.CapNhatPhieuDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/hoanThanhOrUpdateStatusPhieuDanhGia")]
        [Authorize(Policy = "Member")]
        public HoanThanhOrUpdateStatusPhieuDanhGiaResult HoanThanhOrUpdateStatusPhieuDanhGia([FromBody] HoanThanhOrUpdateStatusPhieuDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.HoanThanhOrUpdateStatusPhieuDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterDataTaoKyDanhGia")]
        [Authorize(Policy = "Member")]
        public GetMasterDataTaoKyDanhGiaResult GetMasterDataTaoKyDanhGia([FromBody] GetMasterDataTaoKyDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDataTaoKyDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/createOrUpdateChiTietDeXuatCongTac")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateChiTietDeXuatCongTacResult CreateOrUpdateChiTietDeXuatCongTac([FromBody] CreateOrUpdateChiTietDeXuatCongTacParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrUpdateChiTietDeXuatCongTac(request);
        }

        [HttpPost]
        [Route("api/employee/xoaDeXuatCongTacChiTiet")]
        [Authorize(Policy = "Member")]
        public XoaYeuCauCapPhatResult XoaDeXuatCongTacChiTiet([FromBody] XoaDeXuatCongTacChiTietParameter request)
        {
            return this._iEmployeeDataAccess.XoaDeXuatCongTacChiTiet(request);
        }

        [HttpPost]
        [Route("api/employee/datVeMoiDeXuatCongTac")]
        [Authorize(Policy = "Member")]
        public DeleteDeXuatCongTacResult DatVeMoiDeXuatCongTac([FromBody] DeleteDeXuatCongTacParameter request)
        {
            return this._iEmployeeDataAccess.DatVeMoiDeXuatCongTac(request);
        }

        [HttpPost]
        [Route("api/employee/getListCapPhatTaiSan")]
        [Authorize(Policy = "Member")]
        public GetListCapPhatTaiSanResult GetListCapPhatTaiSan([FromBody] GetListCapPhatTaiSanParameter request)
        {
            return this._iEmployeeDataAccess.GetListCapPhatTaiSan(request);
        }

        [HttpPost]
        [Route("api/employee/capNhatLyDoPheDuyetOrTuChoiDeXuatNV")]
        [Authorize(Policy = "Member")]
        public CapNhatLyDoPheDuyetOrTuChoiDeXuatNVResult CapNhatLyDoPheDuyetOrTuChoiDeXuatNV([FromBody] CapNhatLyDoPheDuyetOrTuChoiDeXuatNVParameter request)
        {
            return this._iEmployeeDataAccess.CapNhatLyDoPheDuyetOrTuChoiDeXuatNV(request);
        }

        [HttpPost]
        [Route("api/employee/capNhatNgayApDungDeXuat")]
        [Authorize(Policy = "Member")]
        public CapNhatNgayApDungDeXuatResult CapNhatNgayApDungDeXuat([FromBody] CapNhatNgayApDungDeXuatParameter request)
        {
            return this._iEmployeeDataAccess.CapNhatNgayApDungDeXuat(request);
        }

        [HttpPost]
        [Route("api/employee/taoKyDanhGia")]
        [Authorize(Policy = "Member")]
        public TaoKyDanhGiaResult TaoKyDanhGia([FromForm] TaoKyDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.TaoKyDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/kyDanhGiaDetail")]
        [Authorize(Policy = "Member")]
        public KyDanhGiaDetailResult KyDanhGiaDetail([FromBody] KyDanhGiaDetailParameter request)
        {
            return this._iEmployeeDataAccess.KyDanhGiaDetail(request);
        }

        [HttpPost]
        [Route("api/employee/xoaPhieuDanhGiaCuaKy")]
        [Authorize(Policy = "Member")]
        public XoaPhieuDanhGiaCuaKyResult XoaPhieuDanhGiaCuaKy([FromBody] XoaPhieuDanhGiaCuaKyParameter request)
        {
            return this._iEmployeeDataAccess.XoaPhieuDanhGiaCuaKy(request);
        }

        [HttpPost]
        [Route("api/employee/luuPhieuDanhGiaCuaKy")]
        [Authorize(Policy = "Member")]
        public LuuPhieuDanhGiaCuaKyResult LuuPhieuDanhGiaCuaKy([FromBody] LuuPhieuDanhGiaCuaKyParameter request)
        {
            return this._iEmployeeDataAccess.LuuPhieuDanhGiaCuaKy(request);
        }

        [HttpPost]
        [Route("api/employee/capNhatKyDanhGia")]
        [Authorize(Policy = "Member")]
        public CapNhatKyDanhGiaResult CapNhatKyDanhGia([FromForm] CapNhatKyDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.CapNhatKyDanhGia(request);
        }


        [HttpPost]
        [Route("api/employee/xoaPhongBanKyDanhGia")]
        [Authorize(Policy = "Member")]
        public XoaPhongBanKyDanhGiaResult XoaPhongBanKyDanhGia([FromBody] XoaPhongBanKyDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.XoaPhongBanKyDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/createOrAddPhongBanKyDanhGia")]
        [Authorize(Policy = "Member")]
        public CreateOrAddPhongBanKyDanhGiaResult CreateOrAddPhongBanKyDanhGia([FromBody] CreateOrAddPhongBanKyDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrAddPhongBanKyDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/hoanThanhkyDanhGia")]
        [Authorize(Policy = "Member")]
        public HoanThanhkyDanhGiaResult HoanThanhkyDanhGia([FromBody] HoanThanhkyDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.HoanThanhkyDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/updateNguoiDanhGiaNhanVienKy")]
        [Authorize(Policy = "Member")]
        public UpdateNguoiDanhGiaNhanVienKyResult UpdateNguoiDanhGiaNhanVienKy([FromBody] UpdateNguoiDanhGiaNhanVienKyParameter request)
        {
            return this._iEmployeeDataAccess.UpdateNguoiDanhGiaNhanVienKy(request);
        }

        [HttpPost]
        [Route("api/employee/taoPhieuTuDanhGiaNhanVien")]
        [Authorize(Policy = "Member")]
        public TaoPhieuTuDanhGiaNhanVienResult TaoPhieuTuDanhGiaNhanVien([FromBody] TaoPhieuTuDanhGiaNhanVienParameter request)
        {
            return this._iEmployeeDataAccess.TaoPhieuTuDanhGiaNhanVien(request);
        }

        [HttpPost]
        [Route("api/employee/thucHienDanhGiaDetail")]
        [Authorize(Policy = "Member")]
        public ThucHienDanhGiaDetailResult ThucHienDanhGiaDetail([FromBody] ThucHienDanhGiaDetailParameter request)
        {
            return this._iEmployeeDataAccess.ThucHienDanhGiaDetail(request);
        }

        [HttpPost]
        [Route("api/employee/luuOrHoanThanhDanhGia")]
        [Authorize(Policy = "Member")]
        public LuuOrHoanThanhDanhGiaResult LuuOrHoanThanhDanhGia([FromBody] LuuOrHoanThanhDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.LuuOrHoanThanhDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/ganMucDanhGiaChung")]
        [Authorize(Policy = "Member")]
        public GanMucDanhGiaChungResult GanMucDanhGiaChung([FromBody] GanMucDanhGiaChungParameter request)
        {
            return this._iEmployeeDataAccess.GanMucDanhGiaChung(request);
        }

        [HttpPost]
        [Route("api/employee/capNhatDanhGiaNhanVienRow")]
        [Authorize(Policy = "Member")]
        public CapNhatDanhGiaNhanVienRowResult CapNhatDanhGiaNhanVienRow([FromBody] CapNhatDanhGiaNhanVienRowParameter request)
        {
            return this._iEmployeeDataAccess.CapNhatDanhGiaNhanVienRow(request);
        }

        [HttpPost]
        [Route("api/employee/taoDeXuatTangLuongKyDanhGia")]
        [Authorize(Policy = "Member")]
        public TaoDeXuatTangLuongKyDanhGiaResult TaoDeXuatTangLuongKyDanhGia([FromBody] TaoDeXuatTangLuongKyDanhGiaParameter request)
        {
            return this._iEmployeeDataAccess.TaoDeXuatTangLuongKyDanhGia(request);
        }

        [HttpPost]
        [Route("api/employee/createOrUpdateLichSuThanhToanBaoHiem")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateLichSuThanhToanBaoHiemResult CreateOrUpdateLichSuThanhToanBaoHiem([FromBody] CreateOrUpdateLichSuThanhToanBaoHiemParameter request)
        {
            return this._iEmployeeDataAccess.CreateOrUpdateLichSuThanhToanBaoHiem(request);
        }
        [HttpPost]
        [Route("api/employee/getMasterDateImportEmployee")]
        [Authorize(Policy = "Member")]
        public GetMasterDateImportEmployeeResult GetMasterDateImportEmployee([FromBody] GetMasterDateImportEmployeeParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDateImportEmployee(request);
        }

        [HttpPost]
        [Route("api/employee/deleteLichSuThanhToanBaoHiemById")]
        [Authorize(Policy = "Member")]
        public DeleteLichSuThanhToanBaoHiemByIdResult DeleteLichSuThanhToanBaoHiemById([FromBody] DeleteLichSuThanhToanBaoHiemByIdParameter request)
        {
            return this._iEmployeeDataAccess.DeleteLichSuThanhToanBaoHiemById(request);
        }


        [HttpPost]
        [Route("api/employee/getAllVacanciesForOther")]
        [Authorize(Policy = "Member")]
        public GetAllVacanciesForOtherResult GetAllVacanciesForOther()
        {
            return this._iEmployeeDataAccess.GetAllVacanciesForOther();
        }


        [HttpPost]
        [Route("api/employee/importEmployee")]
        [Authorize(Policy = "Member")]
        public ImportEmployeeResult ImportEmployee([FromBody] ImportEmployeeParameter request)
        {
            return this._iEmployeeDataAccess.ImportEmployee(request);
        }

        [HttpPost]
        [Route("api/employee/downloadTemplateImportEmployee")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateImportResult DownloadTemplateImportEmployee([FromBody] DownloadTemplateImportParameter request)
        {
            return this._iEmployeeDataAccess.DownloadTemplateImportEmployee(request);
        }

        [HttpPost]
        [Route("api/employee/getDataDashboardHome")]
        [Authorize(Policy = "Member")]
        public GetDataDashboardHomeResult GetDataDashboardHome([FromBody] GetDataDashboardHomeParameter request)
        {
            return this._iEmployeeDataAccess.GetDataDashboardHome(request);
        }

        [HttpPost]
        [Route("api/employee/synchronizeCandidateDataFromCMS")]
        [Authorize(Policy = "Member")]
        public SynchronizeCandidateDataFromCMSResult SynchronizeCandidateDataFromCMS([FromBody] SynchronizeCandidateDataFromCMSParameter request)
        {
            return this._iEmployeeDataAccess.SynchronizeCandidateDataFromCMS(request);
        }

        [HttpPost]
        [Route("api/employee/dashboardHomeViewDetail")]
        [Authorize(Policy = "Member")]
        public DashboardHomeViewDetailResult DashboardHomeViewDetail([FromBody] DashboardHomeViewDetailParameter request)
        {
            return this._iEmployeeDataAccess.DashboardHomeViewDetail(request);
        }

        [HttpPost]
        [Route("api/employee/getListBaoCaoNhanSu")]
        [Authorize(Policy = "Member")]
        public GetListBaoCaoNhanSuResult GetListBaoCaoNhanSu([FromBody] GetListBaoCaoNhanSuParameter request)
        {
            return this._iEmployeeDataAccess.GetListBaoCaoNhanSu(request);
        }

        [HttpPost]
        [Route("api/employee/getBieuDoThongKeNhanSu")]
        [Authorize(Policy = "Member")]
        public GetBieuDoThongKeNhanSuResult GetBieuDoThongKeNhanSu([FromBody] GetBieuDoThongKeNhanSuParameter request)
        {
            return this._iEmployeeDataAccess.GetBieuDoThongKeNhanSu(request);
        }

        [HttpPost]
        [Route("api/employee/giaHanPheDuyetKeHoachOT")]
        [Authorize(Policy = "Member")]
        public GiaHanPheDuyetKeHoachOTResult GiaHanPheDuyetKeHoachOT([FromBody] GiaHanPheDuyetKeHoachOTParameter request)
        {
            return this._iEmployeeDataAccess.GiaHanPheDuyetKeHoachOT(request);
        }

        [HttpPost]
        [Route("api/employee/hoanThanhHoSoCongTac")]
        [Authorize(Policy = "Member")]
        public HoanThanhHoSoCongTacResult HoanThanhHoSoCongTac([FromBody] HoanThanhHoSoCongTacParameter request)
        {
            return this._iEmployeeDataAccess.HoanThanhHoSoCongTac(request);
        }

        [HttpPost]
        [Route("api/employee/saveGhiChuNhanVienKeHoachOT")]
        [Authorize(Policy = "Member")]
        public SaveGhiChuNhanVienKeHoachOTResult SaveGhiChuNhanVienKeHoachOT([FromBody] SaveGhiChuNhanVienKeHoachOTParameter request)
        {
            return this._iEmployeeDataAccess.SaveGhiChuNhanVienKeHoachOT(request);
        }

        [HttpPost]
        [Route("api/employee/layNhanVienCungCapVaCapDuoiOrg")]
        [Authorize(Policy = "Member")]
        public LayNhanVienCungCapVaCapDuoiOrgResult LayNhanVienCungCapVaCapDuoiOrg([FromBody] LayNhanVienCungCapVaCapDuoiOrgParameter request)
        {
            return this._iEmployeeDataAccess.LayNhanVienCungCapVaCapDuoiOrg(request);
        }

        [HttpPost]
        [Route("api/employee/hoanThanhDanhGiaPhongBan")]
        [Authorize(Policy = "Member")]
        public HoanThanhDanhGiaPhongBanResult HoanThanhDanhGiaPhongBan([FromBody] HoanThanhDanhGiaPhongBanParameter request)
        {
            return this._iEmployeeDataAccess.HoanThanhDanhGiaPhongBan(request);
        }

        [HttpPost]
        [Route("api/employee/downloadTemplateImportHDNS")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateImportHDNSResult DownloadTemplateImportHDNS([FromBody] DownloadTemplateImportHDNSParameter request)
        {
            return this._iEmployeeDataAccess.DownloadTemplateImportHDNS(request);
        }

        [HttpPost]
        [Route("api/employee/getMasterDateImportHDNS")]
        [Authorize(Policy = "Member")]
        public GetMasterDateImportHDNSResult GetMasterDateImportHDNS([FromBody] GetMasterDateImportHDNSParameter request)
        {
            return this._iEmployeeDataAccess.GetMasterDateImportHDNS(request);
        }

        [HttpPost]
        [Route("api/employee/importHDNS")]
        [Authorize(Policy = "Member")]
        public ImportHDNSResult ImportHDNS([FromBody] ImportHDNSParameter request)
        {
            return this._iEmployeeDataAccess.ImportHDNS(request);
        }
        [HttpGet]
        [Route("api/employee/getBenefitType")]
        [Authorize(Policy = "Member")]
        public Task<GetBenefitType> GetBenefitType()
        {
            return this._iEmployeeDataAccess.GetBenefitType();
        }

        [HttpPost]
        [Route("api/employee/takeListEmployeeByUserName")]
        [Authorize(Policy = "Member")]
        public TakeListEmployeeByUserNameResult TakeListEmployeeByUserName([FromBody] TakeListEmployeeByUserNameParameter request)
        {
            return this._iEmployeeDataAccess.TakeListEmployeeByUserName(request);
        }
    }
}

