using System;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateProductionOrderEspeciallyRequest : BaseRequest<UpdateProductionOrderEspeciallyParameter>
    {
        public Guid ProductionOrderId { get; set; }
        public bool Especially { get; set; }
        public override UpdateProductionOrderEspeciallyParameter ToParameter()
        {
            return new UpdateProductionOrderEspeciallyParameter()
            {
                ProductionOrderId = ProductionOrderId,
                Especially = Especially
            };
        }
    }
}
