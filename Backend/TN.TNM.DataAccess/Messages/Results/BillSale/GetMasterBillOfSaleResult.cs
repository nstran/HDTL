using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.BillSale;
using TN.TNM.DataAccess.Models.Category;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.BillSale
{
    public class GetMasterBillOfSaleResult : BaseResult
    {
        public List<ProductEntityModel> ListProduct { get; set; }
        public List<CategoryModel> ListStatus { get; set; }
    }
}
