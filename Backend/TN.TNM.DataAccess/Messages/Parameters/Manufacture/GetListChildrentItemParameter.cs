using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetListChildrentItemParameter : BaseParameter
    {
        public Guid ProductionOrderMappingId { get; set; }
    }
}
