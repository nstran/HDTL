using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class CreateProductOrderWorkflowResult : BaseResult
    {
        public Guid ProductOrderWorkflowId { get; set; }
        public List<string> ListCode { get; set; }
    }
}
