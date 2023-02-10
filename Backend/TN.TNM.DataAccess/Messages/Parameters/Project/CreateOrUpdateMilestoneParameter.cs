using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class CreateOrUpdateMilestoneParameter : BaseParameter
    {
        public ProjectMilestoneEntityModel ProjectMilestone { get; set; }
    }
}
