using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetMasterdataCreateProductResult: BaseResult
    {
        public List<VendorEntityModel> ListVendor { get; set; }
        public List<CategoryEntityModel> ListProductMoneyUnit { get; set; }
        public List<CategoryEntityModel> ListProperty { get; set; }
        public List<CategoryEntityModel> ListPriceInventory { get; set; }
        public List<CategoryEntityModel> ListProductUnit { get; set; }
        public List<WareHouseEntityModel> ListWarehouse { get; set; }
        public List<string> ListProductCode { get; set; }
        public List<string> ListProductUnitName { get; set; }
        public List<CategoryEntityModel> ListLoaiHinh { get; set; }

    }
}
