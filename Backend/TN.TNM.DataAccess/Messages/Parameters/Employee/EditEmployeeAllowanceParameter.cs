using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class EditEmployeeAllowanceParameter : BaseParameter
    {
        public EmployeeAllowanceEntityModel EmployeeAllowance { get; set; }
    }
}
