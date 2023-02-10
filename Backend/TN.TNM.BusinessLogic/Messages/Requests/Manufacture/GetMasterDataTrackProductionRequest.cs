using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetMasterDataTrackProductionRequest : BaseRequest<GetMasterDataTrackProductionParameter>
    {
        public override GetMasterDataTrackProductionParameter ToParameter()
        {
            return new GetMasterDataTrackProductionParameter()
            {
                UserId = UserId
            };
        }
    }
}
