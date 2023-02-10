using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class UpdateQuoteParameter:BaseParameter
    {
        public QuoteEntityModel Quote { get; set; }
        public List<QuoteDetailEntityModel> QuoteDetail { get; set; }
        public int TypeAccount { get; set; }
        public List<QuoteDocumentEntityModel> FileList { get; set; }
    }
}
