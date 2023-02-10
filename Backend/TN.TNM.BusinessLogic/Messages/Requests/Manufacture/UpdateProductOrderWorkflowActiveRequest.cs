using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateProductOrderWorkflowActiveRequest : BaseRequest<UpdateProductOrderWorkflowActiveParameter>
    {
        public Guid ProductOrderWorkflowId { get; set; }
        public bool Active { get; set; }
        public override UpdateProductOrderWorkflowActiveParameter ToParameter()
        {
            return new UpdateProductOrderWorkflowActiveParameter()
            {
                UserId = UserId,
                ProductOrderWorkflowId = ProductOrderWorkflowId,
                Active = Active
            };
        }
    }
}
