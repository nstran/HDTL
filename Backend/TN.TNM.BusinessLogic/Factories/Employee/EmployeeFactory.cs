using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;
using GetEmployeeByIdResponse = TN.TNM.BusinessLogic.Messages.Responses.Employee.GetEmployeeByIdResponse;

namespace TN.TNM.BusinessLogic.Factories.Employee
{
    public class EmployeeFactory : BaseFactory, IEmployee
    {
        private IEmployeeDataAccess iEmployeeDataAccess;
        public EmployeeFactory(IEmployeeDataAccess _iEmployeeDataAccess, ILogger<EmployeeFactory> _logger)
        {
            iEmployeeDataAccess = _iEmployeeDataAccess;
            logger = _logger;
        }

        public CreateEmployeeResponse CreateEmployee(CreateEmployeeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.CreateEmployee(parameter);
                var response = new CreateEmployeeResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    EmployeeId = result.EmployeeId,
                    ContactId = result.ContactId,
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreateEmployeeResponse()
                {
                    MessageCode = CommonMessage.Employee.CREATE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SearchEmployeeResponse SearchEmployee(SearchEmployeeRequest request)
        {
            try
            {
                logger.LogInformation("Search Employee");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.SearchEmployee(parameter);
                var response = new SearchEmployeeResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    EmployeeList = new List<EmployeeModel>()
                };
                result.EmployeeList.ForEach(employeeEntity =>
                {
                    response.EmployeeList.Add(new EmployeeModel(employeeEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchEmployeeResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetAllEmployeeResponse GetAllEmployee(GetAllEmployeeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetAllEmployee(parameter);
                var response = new GetAllEmployeeResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    EmployeeList = result.EmployeeList
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetAllEmployeeResponse
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetEmployeeByIdResponse GetEmployeeById(GetEmployeeByIdRequest request)
        {
            try
            {
                logger.LogInformation("Get Employee by Id");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetEmployeeById(parameter);
                var response = new GetEmployeeByIdResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    Employee = new EmployeeModel(result.Employee),
                    Contact = new ContactModel(result.Contact),
                    User = new UserModel(result.User)
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetEmployeeByIdResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public EditEmployeeByIdResponse EditEmployeeById(EditEmployeeByIdRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.EditEmployeeById(parameter);
                var response = new EditEmployeeByIdResponse();
                if (!result.Status)
                {
                    response.StatusCode = HttpStatusCode.ExpectationFailed;
                    response.MessageCode = CommonMessage.Employee.EDIT_FAIL;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.MessageCode = result.Message;
                }

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EditEmployeeByIdResponse
                {
                    MessageCode = CommonMessage.Employee.EDIT_FAIL,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetAllEmpAccountResponse GetAllEmpAccount(GetAllEmpAccountRequest request)
        {
            try
            {
                logger.LogInformation("Get all Employee Account");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetAllEmpAccount(parameter);
                var response = new GetAllEmpAccountResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    EmpAccountList = result.Status ? result.EmpAccountList : null
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllEmpAccountResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetAllEmpIdentityResponse GetAllEmpIdentity(GetAllEmpIdentityRequest request)
        {
            try
            {
                logger.LogInformation("Get all Employee Identity");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetAllEmpIdentity(parameter);
                var response = new GetAllEmpIdentityResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    EmpIdentityList = result.Status ? result.EmpIdentityList : null
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllEmpIdentityResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public EditEmployeeDataPermissionResponse EditEmployeeDataPermission(EditEmployeeDataPermissionRequest request)
        {
            try
            {
                logger.LogInformation("Edit employee data permission");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.EditEmployeeDataPermission(parameter);
                return new EditEmployeeDataPermissionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EditEmployeeDataPermissionResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public EmployeePermissionMappingResponse EmployeePermissionMapping(EmployeePermissionMappingRequest request)
        {
            try
            {
                logger.LogInformation("Edit employee module permission");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.EmployeePermissionMapping(parameter);
                return new EmployeePermissionMappingResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EmployeePermissionMappingResponse
                {
                    MessageCode = CommonMessage.Employee.GRAND_PERMISSION_FAIL,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetAllEmployeeAccountResponse GetAllEmployeeAccount()
        {
            try
            {
                logger.LogInformation("Get all Employee Account");
                var result = iEmployeeDataAccess.GetAllEmployeeAccount();
                var response = new GetAllEmployeeAccountResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    EmployeeAccounts = result.EmployeeAcounts?.Select(eM => new EmployeeModel(eM)).ToList()
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllEmployeeAccountResponse
                {
                    MessageCode = CommonMessage.Employee.GET_FAIL,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetEmployeeByPositionCodeResponse GetEmployeeByPositionCode(GetEmployeeByPositionCodeRequest request)
        {
            try
            {
                var result = iEmployeeDataAccess.GetEmployeeByPositionCode(request.ToParameter());
                var response = new GetEmployeeByPositionCodeResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    EmployeeList = result.EmployeeList?.Select(e => new EmployeeModel(e)).ToList()
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetEmployeeByPositionCodeResponse
                {
                    MessageCode = CommonMessage.Employee.GET_FAIL,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }
        public GetEmployeeHighLevelByEmpIdResponse GetEmployeeHighLevelByEmpId(GetEmployeeHighLevelByEmpIdRequest request)
        {
            try
            {
                var result = iEmployeeDataAccess.GetEmployeeHighLevelByEmpId(request.ToParameter());
                var response = new GetEmployeeHighLevelByEmpIdResponse
                {
                    ListEmployeeToApprove = new List<EmployeeModel>(),
                    ListEmployeeToNotify = new List<EmployeeModel>(),
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                result.ListEmployeeToApprove.ForEach(emp =>
                {
                    response.ListEmployeeToApprove.Add(new EmployeeModel(emp));
                });
                result.ListEmployeeToNotify.ForEach(emp =>
                {
                    response.ListEmployeeToNotify.Add(new EmployeeModel(emp));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetEmployeeHighLevelByEmpIdResponse
                {
                    MessageCode = CommonMessage.Employee.GET_FAIL,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetEmployeeByOrganizationIdResponse GetEmployeeByOrganizationId(GetEmployeeByOrganizationIdRequest request)
        {
            try
            {
                logger.LogInformation("Get Employee By OrganizationId");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetEmployeeByOrganizationId(parameter);
                var response = new GetEmployeeByOrganizationIdResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    listEmployee = result.listEmployee
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetEmployeeByOrganizationIdResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetEmployeeByTopRevenueResponse GetEmployeeByTopRevenue(GetEmployeeByTopRevenueRequest request)
        {
            try
            {
                logger.LogInformation("Get Employee By Top Revenue");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetEmployeeByTopRevenue(parameter);
                var response = new GetEmployeeByTopRevenueResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    listEmployee = result.listEmployee
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetEmployeeByTopRevenueResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public ExportEmployeeRevenueResponse ExportEmployeeRevenue(ExportEmployeeRevenueRequest request)
        {
            try
            {
                logger.LogInformation("Export Employee Revenue");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.ExportEmployeeRevenue(parameter);
                var response = new ExportEmployeeRevenueResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    ExcelFile = result.ExcelFile
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new ExportEmployeeRevenueResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }
        public GetStatisticForEmpDashBoardResponse GetStatisticForEmpDashBoard(GetStatisticForEmpDashBoardRequest request)
        {
            try
            {
                logger.LogInformation("Get Statistic For Emp DashBoard");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetStatisticForEmpDashBoard(parameter);
                var response = new GetStatisticForEmpDashBoardResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    ListEmployee = new List<EmployeeModel>(),
                    ListRequestInsideWeek = new List<EmployeeRequestModel>(),
                    ListEmpNearestBirthday = new List<EmployeeModel>(),
                    ListEmpEndContract = new List<EmployeeModel>(),
                    IsManager = result.IsManager
                };
                result.ListEmployee.ForEach(l => response.ListEmployee.Add(new EmployeeModel(l)));
                result.ListRequestInsideWeek.ForEach(l => response.ListRequestInsideWeek.Add(new EmployeeRequestModel(l)));
                result.ListEmpNearestBirthday.ForEach(l => response.ListEmpNearestBirthday.Add(new EmployeeModel(l)));
                result.ListEmpEndContract.ForEach(l => response.ListEmpEndContract.Add(new EmployeeModel(l)));
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetStatisticForEmpDashBoardResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetEmployeeCareStaffResponse GetEmployeeCareStaff(GetEmployeeCareStaffRequest request)
        {
            try
            {
                logger.LogInformation("Get Employee Care Staff");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetEmployeeCareStaff(parameter);
                var response = new GetEmployeeCareStaffResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    employeeList = result.employeeList
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetEmployeeCareStaffResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public SearchEmployeeFromListResponse SearchEmployeeFromList(SearchEmployeeFromListRequest request)
        {
            try
            {
                logger.LogInformation("Search Employee");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.SearchEmployeeFromList(parameter);
                var response = new SearchEmployeeFromListResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CurrentOrganizationId = result.CurrentOrganizationId,
                    EmployeeList = new List<EmployeeModel>()
                };
                result.EmployeeList.ForEach(employeeEntity =>
                {
                    response.EmployeeList.Add(new EmployeeModel(employeeEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchEmployeeFromListResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetAllEmpAccIdentityResponse GetAllEmpAccIdentity(GetAllEmpAccIdentityRequest request)
        {
            try
            {
                logger.LogInformation("Get All Employee Account Identity");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetAllEmpAccIdentity(parameter);
                var response = new GetAllEmpAccIdentityResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListAccEmployee = result.ListAccEmployee
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllEmpAccIdentityResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public DisableEmployeeResponse DisableEmployee(DisableEmployeeRequest request)
        {
            try
            {
                logger.LogInformation("Disable Employee");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.DisableEmployee(parameter);
                var response = new DisableEmployeeResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = CommonMessage.Employee.DELETE_SUCCESS,
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new DisableEmployeeResponse
                {
                    MessageCode = CommonMessage.Employee.DELETE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
        public CheckAdminLoginResponse CheckAdminLogin(CheckAdminLoginRequest request)
        {
            try
            {
                logger.LogInformation("Check Admin Login");
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.CheckAdminLogin(parameter);
                if (result.IsAdmin == true)
                {
                    return new CheckAdminLoginResponse()
                    {
                        IsAdmin = result.IsAdmin,
                        StatusCode = System.Net.HttpStatusCode.OK,
                        MessageCode = result.Message,
                    };
                }
                else
                {
                    return new CheckAdminLoginResponse()
                    {
                        IsAdmin = result.IsAdmin,
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        MessageCode = result.Message,
                    };
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CheckAdminLoginResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataEmployeeDetailResponse GetMasterDataEmployeeDetail(GetMasterDataEmployeeDetailRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetMasterDataEmployeeDetail(parameter);
                var response = new GetMasterDataEmployeeDetailResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListOrganization = result.ListOrganization,
                    ListPosition = result.ListPosition,
                    ListRole = result.ListRole,
                    ListLoaiHopDong = result.ListLoaiHopDong,
                    ListQuocGia = result.ListQuocGia,
                    ListNganHang = result.ListNganHang
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataEmployeeDetailResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetThongTinCaNhanThanhVienResponse GetThongTinCaNhanThanhVien(GetThongTinCaNhanThanhVienRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetThongTinCaNhanThanhVien(parameter);
                var response = new GetThongTinCaNhanThanhVienResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ThongTinCaNhan = result.ThongTinCaNhan
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetThongTinCaNhanThanhVienResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetThongTinChungThanhVienResponse GetThongTinChungThanhVien(GetThongTinChungThanhVienRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetThongTinChungThanhVien(parameter);
                var response = new GetThongTinChungThanhVienResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ThongTinChung = result.ThongTinChung
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetThongTinChungThanhVienResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SaveThongTinChungThanhVienResponse SaveThongTinChungThanhVien(SaveThongTinChungThanhVienRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.SaveThongTinChungThanhVien(parameter);
                var response = new SaveThongTinChungThanhVienResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new SaveThongTinChungThanhVienResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SaveThongTinCaNhanThanhVienResponse SaveThongTinCaNhanThanhVien(SaveThongTinCaNhanThanhVienRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.SaveThongTinCaNhanThanhVien(parameter);
                var response = new SaveThongTinCaNhanThanhVienResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new SaveThongTinCaNhanThanhVienResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetCauHinhPhanQuyenResponse GetCauHinhPhanQuyen(GetCauHinhPhanQuyenRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetCauHinhPhanQuyen(parameter);
                var response = new GetCauHinhPhanQuyenResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    RoleId = result.RoleId,
                    IsManager = result.IsManager,
                    ListSelectedDonVi = result.ListSelectedDonVi
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetCauHinhPhanQuyenResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SaveCauHinhPhanQuyenResponse SaveCauHinhPhanQuyen(SaveCauHinhPhanQuyenRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.SaveCauHinhPhanQuyen(parameter);
                var response = new SaveCauHinhPhanQuyenResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new SaveCauHinhPhanQuyenResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetThongTinNhanSuResponse GetThongTinNhanSu(GetThongTinNhanSuRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetThongTinNhanSu(parameter);
                var response = new GetThongTinNhanSuResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ThongTinNhanSu = result.ThongTinNhanSu
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetThongTinNhanSuResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SaveThongTinNhanSuResponse SaveThongTinNhanSu(SaveThongTinNhanSuRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.SaveThongTinNhanSu(parameter);
                var response = new SaveThongTinNhanSuResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new SaveThongTinNhanSuResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SaveThongTinLuongVaTroCapResponse SaveThongTinLuongVaTroCap(SaveThongTinLuongVaTroCapRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.SaveThongTinLuongVaTroCap(parameter);
                var response = new SaveThongTinLuongVaTroCapResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new SaveThongTinLuongVaTroCapResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetThongTinGhiChuResponse GetThongTinGhiChu(GetThongTinGhiChuRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.GetThongTinGhiChu(parameter);
                var response = new GetThongTinGhiChuResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListNote = result.ListNote
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetThongTinGhiChuResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public ResetPasswordResponse ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeDataAccess.ResetPassword(parameter);
                var response = new ResetPasswordResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new ResetPasswordResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }
    }
}
