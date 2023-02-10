using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class CreateProductOrderWorkflowResponse : BaseResponse
    {
        public Guid ProductOrderWorkflowId { get; set; }
        public List<string> ListCode { get; set; }
    }
}
