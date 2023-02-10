using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateProductOrderWorkflowRequest : BaseRequest<UpdateProductOrderWorkflowParameter>
    {
        public ProductOrderWorkflowModel ProductOrderWorkflow { get; set; }
        public List<OrderTechniqueMappingModel> ListOrderTechniqueMapping { get; set; }
        public override UpdateProductOrderWorkflowParameter ToParameter()
        {
            var _parameter = new UpdateProductOrderWorkflowParameter()
            {
                UserId = UserId,
                ProductOrderWorkflow = ProductOrderWorkflow.ToEntity(),
                ListOrderTechniqueMapping = new List<OrderTechniqueMappingEntityModel>()
            };

            ListOrderTechniqueMapping.ForEach(item =>
            {
                _parameter.ListOrderTechniqueMapping.Add(item.ToEntity());
            });

            return _parameter;
        }
    }
}
