using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class UpdateItemInProductionParameter : BaseParameter
    {
        public ProductionOrderMappingEntityModel ProductItem { get; set; }
    }
}
