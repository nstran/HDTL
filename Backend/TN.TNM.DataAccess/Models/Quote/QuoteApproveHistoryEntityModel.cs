using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Quote
{
    public class QuoteApproveHistoryEntityModel
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
        public decimal? AmountIncreaseDecrease { get; set; }
    }
}
