using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetMasterDataCreateTechniqueRequestRequest : BaseRequest<GetMasterDataCreateTechniqueRequestParameter>
    {
        public override GetMasterDataCreateTechniqueRequestParameter ToParameter()
        {
            return new GetMasterDataCreateTechniqueRequestParameter()
            {
                UserId = UserId
            };
        }
    }
}
