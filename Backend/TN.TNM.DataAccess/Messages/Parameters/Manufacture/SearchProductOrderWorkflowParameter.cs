using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class SearchProductOrderWorkflowParameter : BaseParameter
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? Active { get; set; }
        public string Description { get; set; }
    }
}
