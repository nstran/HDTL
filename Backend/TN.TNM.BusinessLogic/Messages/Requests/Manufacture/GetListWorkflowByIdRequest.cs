using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetListWorkflowByIdRequest : BaseRequest<GetListWorkflowByIdParameter>
    {
        public Guid ProductOrderWorkflowId { get; set; }

        public override GetListWorkflowByIdParameter ToParameter()
        {
            return new GetListWorkflowByIdParameter()
            {
                UserId = UserId,
                ProductOrderWorkflowId = ProductOrderWorkflowId
            };
        }
    }
}
