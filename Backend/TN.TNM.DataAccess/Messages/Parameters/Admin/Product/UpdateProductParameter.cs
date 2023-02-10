using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.ProductAttributeCategory;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class UpdateProductParameter : BaseParameter
    {
        public ProductEntityModel Product { get; set; }
        public List<ProductVendorMappingEntityModel> ListProductVendorMapping { get; set; }
        public List<ProductBillOfMaterialsEntityModel> ListProductBillOfMaterials { get; set; }
        public List<ProductImageEntityModel> ListProductImage { get; set; }
    }
}
