using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.Api.Controllers
{
    public class EmployeeRequestController : Controller
    {
        private readonly IEmployeeRequestDataAccess _iEmployeeRequestDataAccess;
        public EmployeeRequestController(IEmployeeRequestDataAccess iEmployeeRequestDataAccess)
        {
            _iEmployeeRequestDataAccess = iEmployeeRequestDataAccess;
        }

        [HttpPost]
        [Route("api/employeeRequest/create")]
        [Authorize(Policy = "Member")]
        public CreateEmployeeRequestResult CreateEmployeeRequest([FromBody] CreateEmplyeeRequestParameter request)
        {
            return this._iEmployeeRequestDataAccess.CreateEmployeeRequest(request);
        }

        [HttpPost]
        [Route("api/employeeRequest/search")]
        [Authorize(Policy = "Member")]
        public SearchEmployeeRequestResult SearchEmployeeRequest([FromBody] SearchEmployeeRequestParameter request)
        {
            return this._iEmployeeRequestDataAccess.SearchEmployeeRequest(request);
        }

        [HttpPost]
        [Route("api/employeeRequest/getEmployeeRequestById")]
        [Authorize(Policy = "Member")]
        public GetEmployeeRequestByIdResult GetEmployeeRequestById([FromBody] GetEmployeeRequestByIdParameter request)
        {
            return this._iEmployeeRequestDataAccess.GetEmployeeRequestById(request);
        }

        [HttpPost]
        [Route("api/employeeRequest/editEmployeeRequestById")]
        [Authorize(Policy = "Member")]
        public EditEmployeeRequestByIdResult EditEmployeeRequestById([FromBody] EditEmployeeRequestByIdParameter request)
        {
            return this._iEmployeeRequestDataAccess.EditEmployeeRequestById(request);
        }

        [HttpPost]
        [Route("api/employeeRequest/getDataSearchEmployeeRequest")]
        [Authorize(Policy = "Member")]
        public GetDataSearchEmployeeRequestResult GetDataSearchEmployeeRequest([FromBody] GetDataSearchEmployeeRequestParameter request)
        {
            return this._iEmployeeRequestDataAccess.GetDataSearchEmployeeRequest(request);
        }

        [HttpPost]
        [Route("api/employeeRequest/getMasterCreateEmpRequest")]
        [Authorize(Policy = "Member")]
        public GetMasterCreateEmpRequestResult GetMasterCreateEmpRequest([FromBody] GetMasterCreateEmpRequestParameter request)
        {
            return this._iEmployeeRequestDataAccess.GetMasterCreateEmpRequest(request);
        }

        [HttpPost]
        [Route("api/employeeRequest/deleteDeXuatXinNghiById")]
        [Authorize(Policy = "Member")]
        public DeleteDeXuatXinNghiByIdResult DeleteDeXuatXinNghiById([FromBody] DeleteDeXuatXinNghiByIdParameter request)
        {
            return this._iEmployeeRequestDataAccess.DeleteDeXuatXinNghiById(request);
        }

        [HttpPost]
        [Route("api/employeeRequest/datVeMoiDeXuatXinNghi")]
        [Authorize(Policy = "Member")]
        public DatVeMoiDeXuatXinNghiResult DatVeMoiDeXuatXinNghi([FromBody] DatVeMoiDeXuatXinNghiParameter request)
        {
            return this._iEmployeeRequestDataAccess.DatVeMoiDeXuatXinNghi(request);
        }
    }
}