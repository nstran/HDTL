using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Options
{
    public class GetMasterDataAddVendorToOptionParameter: BaseParameter
    {
        public Guid OptionId { get; set; }
    }
}
