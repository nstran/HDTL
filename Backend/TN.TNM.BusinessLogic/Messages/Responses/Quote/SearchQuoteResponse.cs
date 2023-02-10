using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class SearchQuoteResponse : BaseResponse
    {
        public List<QuoteEntityModel> ListQuote { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
    }
}
