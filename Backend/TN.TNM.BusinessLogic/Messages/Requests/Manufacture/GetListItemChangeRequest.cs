using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetListItemChangeRequest : BaseRequest<GetListItemChangeParameter>
    {
        public Guid ProductionOrderId { get; set; }

        public override GetListItemChangeParameter ToParameter()
        {
            return new GetListItemChangeParameter()
            {
                UserId = UserId,
                ProductionOrderId = ProductionOrderId
            };
        }
    }
}
