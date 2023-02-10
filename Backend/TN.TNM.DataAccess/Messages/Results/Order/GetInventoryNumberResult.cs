using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetInventoryNumberResult : BaseResult
    {
        public decimal? InventoryNumber { get; set; }
    }
}
