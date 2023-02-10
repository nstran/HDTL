using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.ProductCategory;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetListProductResult: BaseResult
    {
        public List<ProductEntityModel> ProductEntityModel { get; set; }
        public List<ProductCategoryEntityModel> ListProductCategory { get; set; }
        public List<VendorEntityModel> ListVendor { get; set; }
        public List<CategoryEntityModel> ListUnit { get; set; }
        public List<CategoryEntityModel> ListProperty { get; set; }
        public List<CategoryEntityModel> ListPriceInventory { get; set; }
        public List<CategoryEntityModel> ListLoaiHinh { get; set; }

    }
}
