using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class CreateQuoteScopeResult : BaseResult
    {
        public List<QuoteScopeEntityModel> ListQuoteScopes { get; set; }
    }
}
