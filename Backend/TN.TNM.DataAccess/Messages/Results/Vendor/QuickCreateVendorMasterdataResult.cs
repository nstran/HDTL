using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class QuickCreateVendorMasterdataResult : BaseResult
    {
        public List<CategoryEntityModel> ListVendorCategory { get; set; }
        public List<string> ListVendorCode { get; set; }
    }
}
