using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class GetDataAddVendorOrderDetailParameter: BaseParameter
    {
        public Guid? VendorId { get; set; }
    }
}
