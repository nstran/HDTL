using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.User;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class EditEmployeeByIdParameter : BaseParameter
    {
        public EmployeeEntityModel Employee { get; set; }
        public ContactEntityModel Contact { get; set; }
        public UserEntityModel User { get; set; }
        public bool IsResetPass { get; set; }
    }
}
