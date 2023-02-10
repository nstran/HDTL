using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class SearchEmployeeInsuranceResult : BaseResult
    {
        public List<EmployeeInsuranceEntityModel> ListEmployeeInsurance { get; set; }
    }
}
