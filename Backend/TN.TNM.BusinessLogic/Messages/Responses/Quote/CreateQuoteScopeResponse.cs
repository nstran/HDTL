using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class CreateQuoteScopeResponse : BaseResponse
    {
        public List<QuoteScopeEntityModel> ListQuoteScopes { get; set; }
    }
}
