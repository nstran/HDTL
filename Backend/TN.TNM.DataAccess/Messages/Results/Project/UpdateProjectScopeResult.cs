using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class UpdateProjectScopeResult : BaseResult    {
        public List<ProjectScopeModel> ListProjectScope { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
    }
}
