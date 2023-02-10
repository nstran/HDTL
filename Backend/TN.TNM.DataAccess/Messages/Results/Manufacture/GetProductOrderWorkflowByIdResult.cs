using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetProductOrderWorkflowByIdResult : BaseResult
    {
        public ProductOrderWorkflowEntityModel ProductOrderWorkflow { get; set; }
        public List<OrderTechniqueMappingEntityModel> ListOrderTechniqueMapping { get; set; }
        public List<string> ListCode { get; set; }
        public List<TechniqueRequestEntityModel> ListTechniqueRequest { get; set; }
        public List<TechniqueRequestEntityModel> ListIgnoreTechniqueRequest { get; set; }
    }
}
