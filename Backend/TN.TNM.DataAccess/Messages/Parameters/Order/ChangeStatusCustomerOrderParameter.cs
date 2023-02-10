using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class ChangeStatusCustomerOrderParameter: BaseParameter
    {
        public Guid OrderId { get; set; }
        public int StatusOrder { get; set; }
        public Guid? PaymentMethod { get; set; }
    }
}
