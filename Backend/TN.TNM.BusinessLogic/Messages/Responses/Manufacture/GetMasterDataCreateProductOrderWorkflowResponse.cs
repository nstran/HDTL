using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetMasterDataCreateProductOrderWorkflowResponse : BaseResponse
    {
        public List<string> ListCode { get; set; }
    }
}
