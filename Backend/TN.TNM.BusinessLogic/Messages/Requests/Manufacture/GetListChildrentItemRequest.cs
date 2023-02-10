using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetListChildrentItemRequest : BaseRequest<GetListChildrentItemParameter>
    {
        public Guid ProductionOrderMappingId { get; set; }
        public override GetListChildrentItemParameter ToParameter()
        {
            return new GetListChildrentItemParameter()
            {
                ProductionOrderMappingId = ProductionOrderMappingId,
                UserId = UserId
            };
        }
    }
}
