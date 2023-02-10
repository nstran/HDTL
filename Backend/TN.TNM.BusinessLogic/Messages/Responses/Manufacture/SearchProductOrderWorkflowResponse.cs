using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class SearchProductOrderWorkflowResponse : BaseResponse
    {
        public List<ProductOrderWorkflowModel> ProductOrderWorkflowList { get; set; }
    }
}
