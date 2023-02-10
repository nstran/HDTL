using System;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetMasterDataProductionOrderDetailRequest : BaseRequest<GetMasterDataProductionOrderDetailParameter>
    {
        public Guid ProductionOrderId { get; set; }
        public override GetMasterDataProductionOrderDetailParameter ToParameter()
        {
            return new GetMasterDataProductionOrderDetailParameter()
            {
                ProductionOrderId = ProductionOrderId
            };
        }
    }
}
