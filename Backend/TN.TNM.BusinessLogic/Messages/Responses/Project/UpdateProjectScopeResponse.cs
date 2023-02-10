using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Project;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class UpdateProjectScopeResponse : BaseResponse    {
        public List<ProjectScopeModel> ListProjectScope { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
    }

    public class CreateProjectScopeResponse : BaseResponse
    {
        public List<ProjectScopeModel> ListProjectScope { get; set; }
    }
}
