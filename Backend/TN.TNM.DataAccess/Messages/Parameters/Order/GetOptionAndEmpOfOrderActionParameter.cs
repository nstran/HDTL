using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetOptionAndEmpOfOrderActionParameter: BaseParameter
    {
        public Guid OrderActionId { get; set; }
        public Guid? ServicePacketMappingOptionsId { get; set; }
        
    }
}
