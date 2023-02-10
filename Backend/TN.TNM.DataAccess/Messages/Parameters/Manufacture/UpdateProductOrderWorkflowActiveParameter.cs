using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class UpdateProductOrderWorkflowActiveParameter : BaseParameter
    {
        public Guid ProductOrderWorkflowId { get; set; }
        public bool Active { get; set; }
    }
}
