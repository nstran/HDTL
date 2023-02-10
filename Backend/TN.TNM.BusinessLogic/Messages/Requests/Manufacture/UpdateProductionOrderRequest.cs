using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateProductionOrderRequest:BaseRequest<UpdateProductionOrderParameter>
    {
        public ProductionOrderModel ProductionOrder { get; set; }

        public override UpdateProductionOrderParameter ToParameter()
        {
            return new UpdateProductionOrderParameter()
            {
                UserId = UserId,
                ProductionOrder = ProductionOrder.ToEntity()
            };
        }
    }
}
