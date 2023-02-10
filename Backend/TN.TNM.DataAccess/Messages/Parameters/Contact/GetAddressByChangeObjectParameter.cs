using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Contact
{
    public class GetAddressByChangeObjectParameter : BaseParameter
    {
        public Guid ObjectId { get; set; }
        public int ObjectType { get; set; }
    }
}
