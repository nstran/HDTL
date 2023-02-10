using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class GetProductCategoryGroupByLevelParameter : BaseParameter
    {
        public DateTime? VendorOrderDateStart { get; set; }
        public DateTime? VendorOrderDateEnd { get; set; }
        public Guid OrganizationId { get; set; }
        public int ProductCategoryLevel { get; set; }
    }
}
