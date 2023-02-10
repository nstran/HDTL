using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetDataCreateVendorResult: BaseResult
    {
        public List<CategoryEntityModel> ListVendorGroup { get; set; }
        public List<string> ListVendorCode { get; set; }
        public List<Models.Address.ProvinceEntityModel> ListProvince { get; set; }
        public List<Models.Address.DistrictEntityModel> ListDistrict { get; set; }
        public List<Models.Address.WardEntityModel> ListWard { get; set; }
    }
}
