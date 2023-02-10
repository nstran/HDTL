using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetListOrderOfCusParameter: BaseParameter
    {
        public Guid CustomerId { get; set; }
    }
}
