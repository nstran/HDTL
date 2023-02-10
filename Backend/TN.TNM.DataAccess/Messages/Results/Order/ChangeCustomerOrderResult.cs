using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.CustomerOrder;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class ChangeCustomerOrderResult: BaseResult
    {
        public List<CustomerOrderDetailEntityModel> ListOrderDetail { get; set; }
        public List<CustomerOrderDetailExtenEntityModel> ListOrderDetailExten { get; set; }
        public List<CustomerOrderExtensionEntityModel> ListAtrrOption { get; set; }
        public string CusName { get; set; }
        public string CusAddress { get; set; }
        public string CusPhone { get; set; }
        public string CusNote { get; set; }
        public DateTime? CusOrderDate { get; set; }

    }
}
