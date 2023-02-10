using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class CreateOrUpdateCustomerOrderProcessParameter: BaseParameter
    {
        public OrderProcessEntityModel OrderProcess { get; set; }
    }
}
