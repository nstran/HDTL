using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class InventoryDeliveryVoucherFilterVendorOrderResult:BaseResult
    {
        public List<VendorOrderEntityModel> listVendorOrder { get; set; }

    }
}
