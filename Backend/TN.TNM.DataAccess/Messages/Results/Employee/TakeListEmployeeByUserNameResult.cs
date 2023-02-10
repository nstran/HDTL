using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.User;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class TakeListEmployeeByUserNameResult : BaseResult
    {
       public List<EmployeeEntityModel> ListEmployeeEntityModel { get; set; }
    }
}
