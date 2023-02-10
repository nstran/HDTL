using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Options
{
    public class GetMasterDataAddVendorToOptionResult: BaseResult
    {
        public List<VendorEntityModel> VendorList { get; set; }
        public List<CategoryEntityModel> ListVendorGroup { get; set; }
        public List<CategoryEntityModel> ListBank { get; set; }
        public List<VendorEntityModel> ListVendorMappingOption { get; set; }
    }
}
