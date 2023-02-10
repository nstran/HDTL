using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class QuoteApproveHistory
    {
        public Guid Id { get; set; }
        public Guid QuoteId { get; set; }
        public string QuoteCode { get; set; }
        public string QuoteName { get; set; }
        public DateTime? SendApproveDate { get; set; }
        public Guid? SendApproveById { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountPriceInitial { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? AmountPriceProfit { get; set; }
        public Guid? TenantId { get; set; }
    }
}
