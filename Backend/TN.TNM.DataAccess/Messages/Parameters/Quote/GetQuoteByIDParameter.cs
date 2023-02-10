using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetQuoteByIDParameter:BaseParameter
    {
        public Guid QuoteId { get; set; }

        public string ObjectType { get; set; }
        
        public decimal? AmountPriceInitial { get; set; }
        
        public decimal? AmountPriceProfit { get; set; }
        
        public decimal? CustomerOrderAmountAfterDiscount { get; set; }
        
        public decimal? TotalAmountDiscount { get; set; }

    }
}
