using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetTotalProductionOrderByIdParameter : BaseParameter
    {
        public Guid TotalProductionOrderId { get; set; }
    }
}
