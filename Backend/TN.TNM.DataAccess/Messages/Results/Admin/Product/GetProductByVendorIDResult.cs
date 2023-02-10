using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetProductByVendorIDResult : BaseResult
    {
        public List<ProductEntityModel> lstProduct { get; set; }
    }
}
