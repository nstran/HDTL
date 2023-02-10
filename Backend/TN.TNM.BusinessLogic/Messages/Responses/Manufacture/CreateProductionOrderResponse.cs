using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class CreateProductionOrderResponse : BaseResponse
    {
        public Guid ProductionOrderId { get; set; }
    }
}
