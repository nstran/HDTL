using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetMasterDataSearchTechniqueRequestRequest : BaseRequest<GetMasterDataSearchTechniqueRequestParameter>
    {
        public override GetMasterDataSearchTechniqueRequestParameter ToParameter()
        {
            return new GetMasterDataSearchTechniqueRequestParameter()
            {
                UserId = UserId
            };
        }
    }
}
