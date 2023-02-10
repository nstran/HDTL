using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class UploadOuoteDocumentResult: BaseResult
    {
        public Guid QuoteID { get; set; }
        public List<QuoteDocumentEntityModel> ListQuoteDocument { get; set; }
    }
}
