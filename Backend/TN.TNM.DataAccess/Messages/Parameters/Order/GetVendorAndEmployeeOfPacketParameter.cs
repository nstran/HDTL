using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetVendorAndEmployeeOfPacketParameter: BaseParameter
    {
        public Guid? PacketServiceId { get; set; }
        public bool IsExten { get; set; }
    }
}
