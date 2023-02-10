using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetProductOrderWorkflowByIdParameter : BaseParameter
    {
        public Guid ProductOrderWorkflowId { get; set; }
    }
}
