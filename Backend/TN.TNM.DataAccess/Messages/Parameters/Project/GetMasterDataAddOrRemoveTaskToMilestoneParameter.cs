using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class GetMasterDataAddOrRemoveTaskToMilestoneParameter : BaseParameter
    {
        public Guid ProjectMilestoneId { get; set; }
        public Guid ProjectId { get; set; }
        // 0 - Add; 1 - Remove
        public int Type { get; set; }
    }
}
