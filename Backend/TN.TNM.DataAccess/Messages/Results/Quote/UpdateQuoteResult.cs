using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class UpdateQuoteResult:BaseResult
    {
        public Guid QuoteID { get; set; }
        //public Guid? VendorOrderID { get; set; }
        public List<string> listInvalidEmail { get; set; }

    }
}
