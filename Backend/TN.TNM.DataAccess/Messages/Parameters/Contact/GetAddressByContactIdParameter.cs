using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Contact
{
    public class GetAddressByContactIdParameter : BaseParameter
    {
        public Guid? ContactId { get; set; }
    }
}
