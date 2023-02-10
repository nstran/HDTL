using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetAllEmployeeResponse : BaseResponse
    {
        public List<EmployeeEntityModel> EmployeeList { get; set; }
    }
}
