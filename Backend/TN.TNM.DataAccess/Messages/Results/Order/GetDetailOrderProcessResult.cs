using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetDetailOrderProcessResult: BaseResult
    {
        public OrderProcessEntityModel OrderProcess { get; set; }
        public List<OrderProcessDetailEntityModel> ListOrderProcessDetail { get; set; }
    }
}
