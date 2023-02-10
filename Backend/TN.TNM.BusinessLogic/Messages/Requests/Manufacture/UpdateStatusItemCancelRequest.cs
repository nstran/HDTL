using System;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateStatusItemCancelRequest : BaseRequest<UpdateStatusItemCancelParameter>
    {
        public Guid ProductionOrderMappingId { get; set; }

        public override UpdateStatusItemCancelParameter ToParameter()
        {
            return new UpdateStatusItemCancelParameter()
            {
                UserId = UserId,
                ProductionOrderMappingId = ProductionOrderMappingId
            };
        }
    }
}
