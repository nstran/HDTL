using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class EditEmployeeInsuranceRequest : BaseRequest<EditEmployeeInsuranceParameter>
    {
        public EmployeeInsuranceEntityModel EmployeeInsurance { get; set; }
        public override EditEmployeeInsuranceParameter ToParameter()
        {
            return new EditEmployeeInsuranceParameter(){
                EmployeeInsurance = EmployeeInsurance,
                UserId = UserId
            };
        }
    }
}
