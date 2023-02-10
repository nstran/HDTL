using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class DeleteOrderOptionByCusParameter: BaseParameter
    {
        public bool IsExtend { get; set; }
        public Guid Id { get; set; }
    }
}
