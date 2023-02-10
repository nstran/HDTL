using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Results;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class TakeListOptionsParameter : BaseParameter
    {
        public Guid CategoryId { get; set; }
    }
}
