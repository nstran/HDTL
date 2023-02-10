using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class CreateProductOrderWorkflowRequest : BaseRequest<CreateProductOrderWorkflowParameter>
    {
        public ProductOrderWorkflowModel ProductOrderWorkflow { get; set; }
        public override CreateProductOrderWorkflowParameter ToParameter()
        {
            return new CreateProductOrderWorkflowParameter()
            {
                UserId = UserId,
                ProductOrderWorkflow = ProductOrderWorkflow.ToEntity()
            };
        }
    }
}
