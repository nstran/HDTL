using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetProductOrderWorkflowByIdRequest : BaseRequest<GetProductOrderWorkflowByIdParameter>
    {
        public Guid ProductOrderWorkflowId { get; set; }
        public override GetProductOrderWorkflowByIdParameter ToParameter()
        {
            return new GetProductOrderWorkflowByIdParameter()
            {
                UserId = UserId,
                ProductOrderWorkflowId = ProductOrderWorkflowId
            };
        }
    }
}
