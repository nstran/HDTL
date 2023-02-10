using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetListItemChangeParameter : BaseParameter
    {
        public Guid ProductionOrderId { get; set; }
    }
}
