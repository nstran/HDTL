using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class CheckIsDefaultProductOrderWorkflowResponse : BaseResponse
    {
        public bool IsDefaultExists { get; set; }
    }
}
