using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class QuotePaymentTerm
    {
        public Guid PaymentTermId { get; set; }
        public Guid QuoteId { get; set; }
        public string OrderNumber { get; set; }
        public string Milestone { get; set; }
        public string PaymentPercentage { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
