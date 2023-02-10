using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetEmployeeListByOrganizationIdResult : BaseResult
    {
        public List<dynamic> employeeList { get; set; }
        public List<dynamic> lstResult { get; set; }
        public int? levelMaxProductCategory { get; set; }
        public List<CustomerOrderEntityModel> lstOrderInventoryDelivery { get; set; }
        public List<CustomerOrderEntityModel> lstOrderBill { get; set; }
        public List<dynamic> statusOrderList { get; set; }
        public List<dynamic> monthOrderList { get; set; }
    }
}
