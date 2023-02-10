using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetMasterDataOrderActionDetailParameter: BaseParameter
    {
        public Guid OrderActionId { get; set; }
    }
}
