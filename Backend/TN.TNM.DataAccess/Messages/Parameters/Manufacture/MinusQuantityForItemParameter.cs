using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class MinusQuantityForItemParameter : BaseParameter
    {
        public Guid ProductionOrderMappingId { get; set; }
        public int MinusType { get; set; }
        public double Quantity { get; set; }
    }
}
