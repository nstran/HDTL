using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class UpdateProductOrderWorkflowResponse : BaseResponse
    {
        public List<string> ListCode { get; set; }
    }
}
