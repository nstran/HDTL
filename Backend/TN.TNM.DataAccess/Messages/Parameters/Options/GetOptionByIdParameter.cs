using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Options
{
    public class GetOptionByIdParameter : BaseParameter
    {
        public Guid Id { get; set; }
    }
}
