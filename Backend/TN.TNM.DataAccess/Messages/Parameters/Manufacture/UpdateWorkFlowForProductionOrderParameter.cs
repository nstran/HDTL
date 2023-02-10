using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class UpdateWorkFlowForProductionOrderParameter : BaseParameter
    {
        public List<ProductionOrderMappingEntityModel> ListProductItem { get; set; }
    }
}