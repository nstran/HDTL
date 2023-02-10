using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetInventoryNumberResponse : BaseResponse
    {
        public decimal? InventoryNumber { get; set; }
    }
}
