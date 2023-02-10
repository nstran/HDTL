using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class CreateTotalProductionOrderParameter : BaseParameter
    {
        public TotalProductionOrderEntityModel TotalProductionOrder { get; set; }
        public List<Guid> ListProductionOrderId { get; set; }
    }
}
