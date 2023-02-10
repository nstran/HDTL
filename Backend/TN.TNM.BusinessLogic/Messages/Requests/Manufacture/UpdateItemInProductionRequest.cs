using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateItemInProductionRequest : BaseRequest<UpdateItemInProductionParameter>
    {
        public ProductionOrderMappingModel ProductItem { get; set; }
        public override UpdateItemInProductionParameter ToParameter()
        {
            return new UpdateItemInProductionParameter()
            {
                UserId = UserId,
                ProductItem = ProductItem.ToEntity()
            };
        }
    }
}
