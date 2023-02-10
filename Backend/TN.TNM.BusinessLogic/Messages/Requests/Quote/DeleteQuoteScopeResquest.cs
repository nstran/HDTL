using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class DeleteQuoteScopeResquest : BaseRequest<DeleteQuoteScopeParameter>
    {
        public Guid? ScopeId { get; set; }
        public override DeleteQuoteScopeParameter ToParameter()
        {
            return new DeleteQuoteScopeParameter
            {
                ScopeId = ScopeId
            };
        }
    }
}
