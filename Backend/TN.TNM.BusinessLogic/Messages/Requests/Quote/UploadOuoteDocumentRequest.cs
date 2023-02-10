using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class UploadOuoteDocumentRequest: BaseRequest<UploadOuoteDocumentParameter>
    {
        public Guid QuoteId { get; set; }

        public List<QuoteDocumentModel> FileList { get; set; }

        public override UploadOuoteDocumentParameter ToParameter()
        {
            List<QuoteDocument> ListQuoteDocument = new List<QuoteDocument>();
            FileList.ForEach(item =>
            {
                ListQuoteDocument.Add(item.ToEntity());
            });

            return new UploadOuoteDocumentParameter
            {
                QuoteId = QuoteId,
                //QuoteDocument = ListQuoteDocument,
            };
        }
    }
}
