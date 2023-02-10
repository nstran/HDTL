using System;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class DeleteItemInProductionRequest : BaseRequest<DeleteItemInProductionParameter>
    {
        public Guid ProductionOrderMappingId { get; set; }

        public override DeleteItemInProductionParameter ToParameter()
        {
            return new DeleteItemInProductionParameter()
            {
                UserId = UserId,
                ProductionOrderMappingId = ProductionOrderMappingId
            };
        }
    }
}
