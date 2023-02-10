using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class CreateTotalProductionOrderRequest : BaseRequest<CreateTotalProductionOrderParameter>
    {
        public TotalProductionOrderModel TotalProductionOrder { get; set; }
        public List<Guid> ListProductionOrderId { get; set; }
        public override CreateTotalProductionOrderParameter ToParameter()
        {
            return new CreateTotalProductionOrderParameter()
            {
                TotalProductionOrder = TotalProductionOrder.ToEntity(),
                ListProductionOrderId = ListProductionOrderId,
                UserId = UserId
            };
        }
    }
}
