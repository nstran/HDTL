using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class GetVendorByCostIdResult : BaseResult
    {
        public decimal PriceCost { get; set; }
        public bool IsHetHan { get; set; }
    }

}
