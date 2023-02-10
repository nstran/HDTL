using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetTotalProductionOrderByIdRequest : BaseRequest<GetTotalProductionOrderByIdParameter>
    {
        public Guid TotalProductionOrderId { get; set; }
        public override GetTotalProductionOrderByIdParameter ToParameter()
        {
            return new GetTotalProductionOrderByIdParameter()
            {
                UserId = UserId,
                TotalProductionOrderId = TotalProductionOrderId
            };
        }
    }
}
