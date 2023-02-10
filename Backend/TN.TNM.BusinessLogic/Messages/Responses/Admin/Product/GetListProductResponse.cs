using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class GetListProductResponse: BaseResponse
    {
        public List<Models.Admin.ProductCategoryModel> ListProductCategory { get; set; }
        public List<Models.Vendor.VendorModel> ListVendor { get; set; }
        public List<Models.Category.CategoryModel> ListUnit { get; set; }
        public List<Models.Category.CategoryModel> ListProperty { get; set; }
        public List<Models.Category.CategoryModel> ListPriceInventory { get; set; }
        public List<CategoryEntityModel> ListLoaiHinh { get; set; }

    }
}
