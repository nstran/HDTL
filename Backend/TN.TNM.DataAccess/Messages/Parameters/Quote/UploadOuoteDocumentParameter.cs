using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class UploadOuoteDocumentParameter: BaseParameter
    {
        public Guid QuoteId { get; set; }

        public List<QuoteDocumentEntityModel> FileList { get; set; }
    }
}
