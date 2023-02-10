using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateProductOrderWorkflowDefaultRequest : BaseRequest<UpdateProductOrderWorkflowDefaultParameter>
    {
        public Guid ProductOrderWorkflowId { get; set; }
        public bool IsDefault { get; set; }
        public override UpdateProductOrderWorkflowDefaultParameter ToParameter()
        {
            return new UpdateProductOrderWorkflowDefaultParameter()
            {
                UserId = UserId,
                ProductOrderWorkflowId = ProductOrderWorkflowId,
                IsDefault = IsDefault
            };
        }
    }
}
