using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetDataSearchVendorResult: BaseResult
    {
        public List<CategoryEntityModel> ListVendorGroup { get; set; }
    }
}
