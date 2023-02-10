using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetVendorByCostIdParameter : BaseParameter
    {
        public Guid CostId { get; set; }
        public decimal SoLuong { get; set; }
    }

}
