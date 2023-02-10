using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class MinusQuantityForItemRequest : BaseRequest<MinusQuantityForItemParameter>
    {
        public Guid ProductionOrderMappingId { get; set; }
        public int MinusType { get; set; }
        public double Quantity { get; set; }

        public override MinusQuantityForItemParameter ToParameter()
        {
            return new MinusQuantityForItemParameter()
            {
                UserId = UserId,
                ProductionOrderMappingId = ProductionOrderMappingId,
                MinusType = MinusType,
                Quantity = Quantity
            };
        }
    }
}
