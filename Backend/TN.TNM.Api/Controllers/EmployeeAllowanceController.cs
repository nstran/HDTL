using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.Api.Controllers
{

    public class EmployeeAllowanceController : Controller
    {
        private readonly IEmployeeAllowanceDataAccess _iEmployeeAllowanceDataAccess;
        public EmployeeAllowanceController( IEmployeeAllowanceDataAccess iEmployeeAllowanceDataAccess)
        {
            this._iEmployeeAllowanceDataAccess = iEmployeeAllowanceDataAccess;
        }

        /// <summary>
        /// getEmployeeRequestById
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeAllowance/getEmployeeAllowanceById")]
        [Authorize(Policy = "Member")]
        public GetEmployeeAllowanceByEmpIdResult GetEmployeeRequestById([FromBody]GetEmployeeAllowanceByEmpIdParameter request)
        {
            return this._iEmployeeAllowanceDataAccess.GetEmployeeAllowanceByEmpId(request);
        }
        /// <summary>
        /// getEmployeeRequestById
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeAllowance/editEmployeeAllowance")]
        [Authorize(Policy = "Member")]
        public EditEmployeeAllowanceResult EditEmployeeAllowance([FromBody]EditEmployeeAllowanceParameter request)
        {
            return this._iEmployeeAllowanceDataAccess.EditEmployeeAllowance(request);
        }
    }
}