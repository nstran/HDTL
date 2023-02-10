using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetMasterDataSearchTotalProductionOrderRequest : BaseRequest<GetMasterDataSearchTotalProductionOrderParameter>
    {
        public override GetMasterDataSearchTotalProductionOrderParameter ToParameter()
        {
            return new GetMasterDataSearchTotalProductionOrderParameter()
            {
                UserId = UserId
            };
        }
    }
}
