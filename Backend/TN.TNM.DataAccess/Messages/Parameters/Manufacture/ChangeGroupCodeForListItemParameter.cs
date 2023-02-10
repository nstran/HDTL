using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class ChangeGroupCodeForListItemParameter : BaseParameter
    {
        public Guid ProductionOrderId { get; set; }
        public string Code_11 { get; set; }
        public string Code_111 { get; set; }
        public string Code_112 { get; set; }
        public string Code_12 { get; set; }
        public string Code_121 { get; set; }
        public string Code_122 { get; set; }
    }
}
