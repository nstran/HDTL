using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.ProcurementRequest;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetDashboardVendorResult : BaseResult
    {
        public decimal TotalCost { get; set; }
        public bool IsRoot { get; set; }
        public string OrganizationName { get; set; }
        public OrganizationEntityModel Organization { get; set; }
        public int LevelMaxProductCategory { get; set; }
        public List<dynamic> ListResultVendorChart { get; set; }
        public List<VendorOrderEntityModel> ListVendorOrder { get; set; }
        public List<ProcurementRequestEntityModel> ListRequest { get; set; }
    }
}
