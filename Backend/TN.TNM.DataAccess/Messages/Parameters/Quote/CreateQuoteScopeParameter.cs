using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class CreateQuoteScopeParameter : BaseParameter
    {
        public Guid? ScopeId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? QuoteId { get; set; }
    }
}
