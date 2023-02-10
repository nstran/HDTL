using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.CustomerOrder;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Options;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetOptionAndEmpOfOrderActionResult : BaseResult
    {
        public List<EmployeeEntityModel> ListSupporter { get; set; }
        public List<CustomerOrderTaskEntityModel> ListOption { get; set; }
    }
}
