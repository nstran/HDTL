using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetMasterDataSearchQuoteRequest : BaseRequest<GetMasterDataSearchQuoteParameter>
    {
        public override GetMasterDataSearchQuoteParameter ToParameter()
        {
            return new GetMasterDataSearchQuoteParameter()
            {
                UserId = UserId
            };
        }
    }
}
