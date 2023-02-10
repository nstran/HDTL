using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetEmployeeManagerResult : BaseResult
    {
        public List<EmployeeEntityModel> ManagerList { get; set; }
    }
}
