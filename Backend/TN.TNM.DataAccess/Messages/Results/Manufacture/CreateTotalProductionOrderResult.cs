using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class CreateTotalProductionOrderResult : BaseResult
    {
        public Guid TotalProductionOrderId { get; set; }
    }
}
