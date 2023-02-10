using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetAllEmployeeResult : BaseResult
    {
        public List<EmployeeEntityModel> EmployeeList { get; set; }
    }
}
