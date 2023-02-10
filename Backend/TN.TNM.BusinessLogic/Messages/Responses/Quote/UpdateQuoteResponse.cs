using System;
using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class UpdateQuoteResponse:BaseResponse
    {
        public Guid QuoteID { get; set; }

        public List<string> listInvalidEmail { get; set; }
    }
}
