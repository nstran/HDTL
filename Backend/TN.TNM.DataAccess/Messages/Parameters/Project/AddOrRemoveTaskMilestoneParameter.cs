using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class AddOrRemoveTaskMilestoneParameter : BaseParameter
    {
        public List<Guid> ListTaskId { get; set; }
        public Guid ProjectMilestoneId { get; set; }
        public int Type { get; set; }
    }
}
