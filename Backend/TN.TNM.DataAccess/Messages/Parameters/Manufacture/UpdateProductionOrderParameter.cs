using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class UpdateProductionOrderParameter : BaseParameter
    {
        public ProductionOrderEntityModel ProductionOrder { get; set; }
    }
}
