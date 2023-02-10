using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetEmployeeRequestByEmpIdResult : BaseResult
    {
        public List<EmployeeRequestEntityModel> ListEmployeeRequest { get; set; }
        public double amountAbsentWithPermission { get; set; }
        public double amountAbsentWithoutPermission { get; set; }
    }
}

