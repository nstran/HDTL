using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class CreateQuoteScopeResquest : BaseRequest<CreateQuoteScopeParameter>
    {
        public Guid? ScopeId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? QuoteId { get; set; }
        public override CreateQuoteScopeParameter ToParameter()
        {
            return new CreateQuoteScopeParameter
            {
                ScopeId = ScopeId,
                Category = Category,
                Description = Description,
                Level = Level,
                ParentId = ParentId,
                QuoteId = QuoteId
            };
        }
    }
}
