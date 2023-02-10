using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetMasterDataCreateTotalProductionOrderRequest : BaseRequest<GetMasterDataCreateTotalProductionOrderParameter>
    {
        public override GetMasterDataCreateTotalProductionOrderParameter ToParameter()
        {
            return new GetMasterDataCreateTotalProductionOrderParameter()
            {
                UserId = UserId
            };
        }
    }
}
