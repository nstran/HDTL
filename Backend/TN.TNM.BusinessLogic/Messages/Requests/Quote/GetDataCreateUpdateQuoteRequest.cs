﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetDataCreateUpdateQuoteRequest : BaseRequest<GetDataCreateUpdateQuoteParameter>
    {
        public Guid? QuoteId { get; set; }
        public override GetDataCreateUpdateQuoteParameter ToParameter()
        {
            return new GetDataCreateUpdateQuoteParameter()
            {
                QuoteId = QuoteId,
                UserId = UserId
            };
        }
    }
}
