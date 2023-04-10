using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetMasterDataCreateOrderActionResult: BaseResult
    {
        public List<CustomerOrderEntityModel> ListCustomerOrder { get; set; }
        public List<EmployeeEntityModel> ListEmployeeEntityModel { get; set; }

        public string CreatorName { get; set; }
    }
}
