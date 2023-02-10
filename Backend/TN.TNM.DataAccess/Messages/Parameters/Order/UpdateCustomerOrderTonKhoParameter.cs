using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class UpdateCustomerOrderTonKhoParameter:BaseParameter
    {
        public CustomerOrderEntityModel CustomerOrder { get; set; }
        public List<CustomerOrderDetailEntityModel> CustomerOrderDetail { get; set; }
        public List<OrderCostDetailEntityModel> OrderCostDetail { get; set; }
        public int TypeAccount { get; set; }
        public string StatusType { get; set; }
    }
}
