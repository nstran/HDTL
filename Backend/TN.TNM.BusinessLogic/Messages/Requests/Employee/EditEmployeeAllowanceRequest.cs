using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class EditEmployeeAllowanceRequest : BaseRequest<EditEmployeeAllowanceParameter>
    {
        public EmployeeAllowanceEntityModel EmployeeAllowance { get; set; }
        public override EditEmployeeAllowanceParameter ToParameter()
        {
            return new EditEmployeeAllowanceParameter()
            {
                EmployeeAllowance = EmployeeAllowance,
                UserId = UserId
            };
        }
    }
}
