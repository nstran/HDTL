using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Vendor
{
    public class VendorEntityModel : BaseModel<Databases.Entities.Vendor>
    {
        public Guid VendorId { get; set; }
        public Guid? ContactId { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public Guid VendorGroupId { get; set; }
        public Guid PaymentId { get; set; }
        public string PaymentName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal? TotalPurchaseValue { get; set; }
        public decimal? TotalPayableValue { get; set; }
        public DateTime? NearestDateTransaction { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public string VendorGroupName { get; set; }
        public int CountVendorInformation { get; set; }
        public bool CanDelete { get; set; }
        public List<Guid> ListProductId { get; set; }
        public string VendorCodeName { get; set; }
        public bool ExitsAccount { get; set; }
        public decimal? Price { get; set; }
        public List<Guid> ListoptionId { get; set; }

        public VendorEntityModel()
        {
            this.ListProductId = new List<Guid>();
        }

        public VendorEntityModel(Databases.Entities.Vendor model)
        {
            Mapper(model, this);
        }

        public override Databases.Entities.Vendor ToEntity()
        {
            var entity = new Databases.Entities.Vendor();
            Mapper(this, entity);
            return entity;
        }
    }
}
