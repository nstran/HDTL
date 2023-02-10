using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetEmployeeWithNotificationPermisisonResult : BaseResult
    {
        public List<EmployeeEntityModel> EmployeeList { get; set; }
    }
}
