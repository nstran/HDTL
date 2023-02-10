using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class SearchProductOrderWorkflowResult : BaseResult
    {
        public List<ProductOrderWorkflowEntityModel> ProductOrderWorkflowList { get; set; }
    }
}
