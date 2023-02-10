using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class GetMasterDataProjectMilestoneParameter : BaseParameter
    {
        public Guid ProjectId { get; set; }
    }
}
