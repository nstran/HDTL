using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetInventoryNumberParameter : BaseParameter
    {
        public Guid? WareHouseId {get; set;}
        public Guid? ProductId { get; set; }
    }
}
