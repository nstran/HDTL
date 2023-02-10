using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetDetailOrderProcessParameter: BaseParameter
    {
        public Guid Id { get; set; }
    }
}
