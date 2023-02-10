using System;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateStatusItemStopRequest : BaseRequest<UpdateStatusItemStopParameter>
    {
        public Guid ProductionOrderMappingId { get; set; }

        public override UpdateStatusItemStopParameter ToParameter()
        {
            return new UpdateStatusItemStopParameter()
            {
                UserId = UserId,
                ProductionOrderMappingId = ProductionOrderMappingId
            };
        }
    }
}
