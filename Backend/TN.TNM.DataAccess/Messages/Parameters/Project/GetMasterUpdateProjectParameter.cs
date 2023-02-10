using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class GetMasterUpdateProjectParameter : BaseParameter
    {
        public Guid ProjectId { get; set; }
    }
}
