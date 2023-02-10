using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.Api.Controllers
{
    public class EmployeeInsuranceController : Controller
    {
        private readonly IEmployeeInsurance _iEmployeeInsurance;
        private readonly IEmployeeInsuranceDataAccess _iEmployeeInsuranceDataAccess;
        public EmployeeInsuranceController(IEmployeeInsurance iEmployeeInsurance, IEmployeeInsuranceDataAccess iEmployeeInsuranceDataAccess)
        {
            this._iEmployeeInsurance = iEmployeeInsurance;
            this._iEmployeeInsuranceDataAccess = iEmployeeInsuranceDataAccess;
        }

        /// <summary>
        /// Create a new employee Insurance
        /// </summary>
        /// <param name="Insurance">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeInsurance/create")]
        //[Authorize(Policy = "Member")]
        public CreateEmployeeInsuranceResult CreateEmployeeInsurance([FromBody]CreateEmployeeInsuranceParameter request)
        {
            return this._iEmployeeInsuranceDataAccess.CreateEmployeeInsurance(request);
        }
        /// <summary>
        /// Edit a new employee Insurance
        /// </summary>
        /// <param name="Insurance">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeInsurance/edit")]
        //[Authorize(Policy = "Member")]
        public EditEmployeeInsuranceResult EditEmployeeInsurance([FromBody]EditEmployeeInsuranceParameter request)
        {
            return this._iEmployeeInsuranceDataAccess.EditEmployeeInsurance(request);
        }
        /// <summary>
        /// Search a new employee Insurance
        /// </summary>
        /// <param name="Insurance">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeInsurance/search")]
        //[Authorize(Policy = "Member")]
        public SearchEmployeeInsuranceResult SearchEmployeeInsurance([FromBody]SearchEmployeeInsuranceParameter request)
        {
            return this._iEmployeeInsuranceDataAccess.SearchEmployeeInsurance(request);
        }
    }
}