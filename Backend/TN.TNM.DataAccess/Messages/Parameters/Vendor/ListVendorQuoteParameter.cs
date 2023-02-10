using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class ListVendorQuoteParameter : BaseParameter
    {
        public SuggestedSupplierQuotesEntityModel SuggestedSupplierQuotes { get; set; }
        public List<SuggestedSupplierQuotesDetailEntityModel> SuggestedSupplierQuoteDetailList { get; set; }
        public int Index { get; set; }
    }
}
