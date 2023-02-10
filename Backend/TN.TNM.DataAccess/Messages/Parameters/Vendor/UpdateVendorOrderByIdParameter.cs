using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class UpdateVendorOrderByIdParameter : BaseParameter
    {
        public VendorOrderEntityModel VendorOrder { get; set; }
        public List<VendorOrderDetailEntityModel> VendorOrderDetail { get; set; }
        public bool IsSendApproval { get; set; }
        public List<VendorOrderProcurementRequestMappingEntityModel> ListVendorOrderProcurementRequestMapping { get; set; }
        public List<VendorOrderCostDetailEntityModel> ListVendorOrderCostDetail { get; set; }
    }
}
