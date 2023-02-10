using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class UpdateProjectMilestoneParameter : BaseParameter
    {
        public ProjectMilestoneEntityModel ProjectMilestone { get; set; }
    }
}
