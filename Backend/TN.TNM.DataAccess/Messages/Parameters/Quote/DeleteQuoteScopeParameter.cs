using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class DeleteQuoteScopeParameter : BaseParameter
    {
        public Guid? ScopeId { get; set; }
    }
}
