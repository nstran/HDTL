using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetMasterDataVendorDialogResult : BaseResult
    {
        public List<VendorEntityModel> ListVendor { get; set; }
        public List<CategoryEntityModel> ListProductMoneyUnit { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
        public List<SuggestedSupplierQuotesEntityModel> ListSuggestedSupplierQuote { get; set; }
    }
}
