using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetEmployeeAllowanceByEmpIdResult : BaseResult
    {
        public EmployeeAllowanceEntityModel EmployeeAllowance { get; set; }
    }
}
