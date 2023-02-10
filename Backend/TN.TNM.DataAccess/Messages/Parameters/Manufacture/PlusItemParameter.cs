using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class PlusItemParameter : BaseParameter
    {
        public ProductionOrderHistoryEntityModel ProductionOrderHistory { get; set; }
    }
}
