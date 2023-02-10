using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetMasterDataPriceProductResult : BaseResult
    {
        public List<ProductEntityModel> ListProduct { get; set; }
        public List<CategoryEntityModel> ListCategory { get; set; }
        public List<PriceProductEntityModel> ListPrice { get; set; }
    }
}
