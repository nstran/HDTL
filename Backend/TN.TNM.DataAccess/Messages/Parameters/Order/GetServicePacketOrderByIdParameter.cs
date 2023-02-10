using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetServicePacketOrderByIdParameter: BaseParameter
    {
        public Guid ServicePacketId { get; set; }
    }
}
