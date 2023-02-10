using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class CheckIsDefaultProductOrderWorkflowRequest : BaseRequest<CheckIsDefaultProductOrderWorkflowParameter>
    {
        public override CheckIsDefaultProductOrderWorkflowParameter ToParameter()
        {
            return new CheckIsDefaultProductOrderWorkflowParameter()
            {
                UserId = UserId
            };
        }
    }
}
