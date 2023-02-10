using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Vendor
    {
        public Vendor()
        {
            Inventory = new HashSet<Inventory>();
            ProcurementRequestItem = new HashSet<ProcurementRequestItem>();
            ProductVendorMapping = new HashSet<ProductVendorMapping>();
            VendorOrderDetail = new HashSet<VendorOrderDetail>();
        }

        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public Guid VendorGroupId { get; set; }
        public Guid PaymentId { get; set; }
        public decimal? TotalPurchaseValue { get; set; }
        public decimal? TotalPayableValue { get; set; }
        public DateTime? NearestDateTransaction { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public Guid? TenantId { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Mst { get; set; }
        public string AccountNumberInfor { get; set; }
        public string ContactName { get; set; }
        public bool? Gender { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; }
        public Guid? ProductMappingOptionId { get; set; }
        public string Description { get; set; }

        public Category Payment { get; set; }
        public Category VendorGroup { get; set; }
        public ICollection<Inventory> Inventory { get; set; }
        public ICollection<ProcurementRequestItem> ProcurementRequestItem { get; set; }
        public ICollection<ProductVendorMapping> ProductVendorMapping { get; set; }
        public ICollection<VendorOrderDetail> VendorOrderDetail { get; set; }
    }
}
