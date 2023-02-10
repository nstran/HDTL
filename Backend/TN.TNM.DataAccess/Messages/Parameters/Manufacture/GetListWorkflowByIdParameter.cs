using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetListWorkflowByIdParameter : BaseParameter
    {
        public Guid ProductOrderWorkflowId { get; set; }
    }
}
