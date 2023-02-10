using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Results.Task
{
    public class DeleteProjectScopeResult : BaseResult
    {
        public Guid ProjectScopeId { get; set; }
        public List<ProjectScopeModel> ListProjectScope { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
    }
}
