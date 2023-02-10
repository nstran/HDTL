using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetEmployeeHighLevelByEmpIdResult : BaseResult
    {
        public List<CategoryEntityModel> ListReasons { get; set; }
        public List<EmployeeEntityModel> ListEmployeeToApprove { get; set; }
        public List<EmployeeEntityModel> ListEmployeeToNotify { get; set; }
    }
}
