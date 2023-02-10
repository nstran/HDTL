using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class OrderProcess
    {
        public Guid Id { get; set; }
        public Guid? ServicePacketId { get; set; }
        public string OrderProcessCode { get; set; }
        public int? RateStar { get; set; }
        public string RateContent { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public Guid? ParentId { get; set; }
        public int? Status { get; set; }
    }
}
