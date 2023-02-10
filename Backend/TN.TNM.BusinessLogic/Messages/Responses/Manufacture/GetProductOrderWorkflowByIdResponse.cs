using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetProductOrderWorkflowByIdResponse : BaseResponse
    {
        public ProductOrderWorkflowModel ProductOrderWorkflow { get; set; }
        public List<OrderTechniqueMappingModel> ListOrderTechniqueMapping { get; set; }
        public List<string> ListCode { get; set; }
        public List<TechniqueRequestModel> ListTechniqueRequest { get; set; }
        public List<TechniqueRequestModel> ListIgnoreTechniqueRequest { get; set; }
    }
}
