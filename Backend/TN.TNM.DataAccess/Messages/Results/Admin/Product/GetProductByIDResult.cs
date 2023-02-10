using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.ProductAttributeCategory;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetProductByIDResult : BaseResult
    {
        public ProductEntityModel Product { get; set; }
        public List<ProductVendorMappingEntityModel> LstProductVendorMapping { get; set; }
        public List<ProductAttributeCategoryEntityModel> lstProductAttributeCategory { get; set; }
        public List<CustomerOrderEntityModel> lstCustomerOrder { get; set; }
        public List<ProductImageEntityModel> ListProductImage { get; set; }
        public List<Models.Product.InventoryReportByProductIdEntityModel> ListInventory { get; set; }
        public bool CanDelete { get; set; }
        public List<Models.Product.ProductBillOfMaterialsEntityModel> ListProductBillOfMaterials { get; set; }
    }
}
