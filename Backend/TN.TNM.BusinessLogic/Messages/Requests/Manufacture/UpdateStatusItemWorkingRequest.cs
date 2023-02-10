using System;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateStatusItemWorkingRequest : BaseRequest<UpdateStatusItemWorkingParameter>
    {
        public Guid ProductionOrderMappingId { get; set; }
        public override UpdateStatusItemWorkingParameter ToParameter()
        {
            return new UpdateStatusItemWorkingParameter()
            {
                ProductionOrderMappingId = ProductionOrderMappingId,
                UserId = UserId
            };
        }
    }
}
