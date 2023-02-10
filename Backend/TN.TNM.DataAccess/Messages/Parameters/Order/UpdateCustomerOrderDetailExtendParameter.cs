using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class UpdateCustomerOrderDetailExtendParameter: BaseParameter
    {
        public Guid OrderId { get; set; }
        public List<CustomerOrderDetailExtenEntityModel> ListOrderDetailExten { get; set; }
    }
}
