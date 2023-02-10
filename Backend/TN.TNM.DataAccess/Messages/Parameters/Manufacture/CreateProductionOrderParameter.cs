using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class CreateProductionOrderParameter : BaseParameter
    {
        public ProductionOrderEntityModel ProductionOrder { get; set; }
        public List<ProductionOrderMappingEntityModel> ListProduct { get; set; }
        public List<ProductionOrderMappingEntityModel> ListProductChildren { get; set; }
        public List<ProductionOrderMappingEntityModel> ListProductChildrenChildren { get; set; }
    }
}
