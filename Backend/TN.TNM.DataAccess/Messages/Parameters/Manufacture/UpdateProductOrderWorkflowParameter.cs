using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class UpdateProductOrderWorkflowParameter : BaseParameter
    {
        public ProductOrderWorkflowEntityModel ProductOrderWorkflow { get; set; }
        public List<OrderTechniqueMappingEntityModel> ListOrderTechniqueMapping { get; set; }
    }
}
