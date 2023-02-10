using System;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetMasterDataCreateProductionOrderRequest:BaseRequest<GetMasterDataCreateProductionOrderParameter>
    {
        public Guid OrderId { get; set; }
        public override GetMasterDataCreateProductionOrderParameter ToParameter()
        {
            return new GetMasterDataCreateProductionOrderParameter()
            {
                OrderId = OrderId
            };
        }
    }
}
