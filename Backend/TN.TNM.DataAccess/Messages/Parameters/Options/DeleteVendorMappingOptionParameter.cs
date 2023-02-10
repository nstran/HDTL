using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Options
{
    public class DeleteVendorMappingOptionParameter: BaseParameter
    {
        public Guid VendorId { get; set; }
        public Guid OptionId { get; set; }
    }
}
