using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.OrderProcessMappingEmployee;
using TN.TNM.DataAccess.Models.User;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class TakeListEvaluateResult : BaseResult
    {
        public List<OrderProcessMappingEmployeeEntityModel> ListOrderProcessMappingEmployee { get; set; }
    }
}
