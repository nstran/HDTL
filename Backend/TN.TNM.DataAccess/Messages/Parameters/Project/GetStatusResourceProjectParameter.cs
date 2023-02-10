using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class GetStatusResourceProjectParameter : BaseParameter
    {
        public Guid? ProjectResourceId { get; set; }
    }
}
