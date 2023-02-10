using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.BusinessLogic.Messages.Responses.Task
{
    public class DeleteProjectScopeResponse : BaseResponse
    {
       public Guid ProjectScopeId { get; set; }
        public List<ProjectScopeModel> ListProjectScope { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
    }
}
