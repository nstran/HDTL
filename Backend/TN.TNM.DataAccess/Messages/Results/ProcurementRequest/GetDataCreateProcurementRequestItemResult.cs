using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.ProcurementRequest
{
    public class GetDataCreateProcurementRequestItemResult: BaseResult
    {
        public List<Models.Vendor.VendorEntityModel> ListVendor { get; set; }
        public List<Models.Product.ProductEntityModel> ListProduct { get; set; }
        public List<CategoryEntityModel> ListMoneyUnit { get; set; }
        public List<DataAccess.Databases.Entities.ProcurementPlan> ListProcurementPlan { get; set; }

        public List<WareHouseEntityModel> ListWarehouse { get; set; }
    }
}
