using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetMasterDataSearchProductOrderWorkflowRequest : BaseRequest<GetMasterDataSearchProductOrderWorkflowParameter>
    {
        public override GetMasterDataSearchProductOrderWorkflowParameter ToParameter()
        {
            return new GetMasterDataSearchProductOrderWorkflowParameter()
            {
                UserId = UserId
            };
        }
    }
}
