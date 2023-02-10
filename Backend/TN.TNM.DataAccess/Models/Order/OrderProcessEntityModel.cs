using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Order
{
    public class OrderProcessEntityModel
    {
        public Guid? Id { get; set; }
        public Guid? ServicePacketId { get; set; }
        public string ServicePacketName { get; set; }
        public string OrderProcessCode { get; set; }
        public int? RateStar { get; set; }
        public string RateContent { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public List<Guid> ListPersonInCharge { get; set; }
        public bool? IsCreator { get; set; }

    }
}
