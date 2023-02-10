using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetMasterDataCreateProductOrderWorkflowRequest : BaseRequest<GetMasterDataCreateProductOrderWorkflowParameter>
    {
        public override GetMasterDataCreateProductOrderWorkflowParameter ToParameter()
        {
            return new GetMasterDataCreateProductOrderWorkflowParameter()
            {
                UserId = this.UserId
            };
        }
    }
}
