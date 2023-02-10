using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetVendorByProductIdParameter : BaseParameter
    {
        public Guid ProductId { get; set; }
        public decimal SoLuong { get; set; }
        public Guid? CustomerGroupId { get; set; }
    }
}
