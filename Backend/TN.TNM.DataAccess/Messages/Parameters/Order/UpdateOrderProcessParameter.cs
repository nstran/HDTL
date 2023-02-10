using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class UpdateOrderProcessParameter: BaseParameter
    {
        public Guid OrderProcessId { get; set; }
        public Guid? OrderProcessDetailId { get; set; }
    }
}
