using System;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetMasterDataDialogListProductOrderRequest : BaseRequest<GetMasterDataDialogListProductOrderParameter>
    {
        public Guid ProductionOrderId { get; set; }
        public override GetMasterDataDialogListProductOrderParameter ToParameter()
        {
            return new GetMasterDataDialogListProductOrderParameter()
            {
                ProductionOrderId = ProductionOrderId
            };
        }
    }
}
