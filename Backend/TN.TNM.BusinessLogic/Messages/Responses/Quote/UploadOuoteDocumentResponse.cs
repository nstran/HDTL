using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class UploadOuoteDocumentResponse: BaseResponse
    {
        public List<QuoteDocumentModel> ListQuoteDocument { get; set; }
    }
}
