using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class GetMasterMilestoneParameter : BaseParameter
    {
        public Guid ProjectId { get; set; }
    }
}
