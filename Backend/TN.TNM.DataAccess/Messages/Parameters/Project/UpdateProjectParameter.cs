using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.ProjectObjective;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class UpdateProjectParameter : BaseParameter
    {
        public ProjectEntityModel Project { get; set; }
        public List<ProjectObjectiveEntityModel> ListProjectTarget { get; set; }
    }
}
