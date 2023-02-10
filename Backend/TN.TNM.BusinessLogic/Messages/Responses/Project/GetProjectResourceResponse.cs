using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Project;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class GetProjectResourceResponse : BaseResponse
    {      
        public ProjectModel Project { get; set; }
        public List<ProjectResourceModel> ListProjectResource { get; set; }
        public List<CategoryEntityModel> ListPaymentMethod { get; set; }
        public List<TaskEntityModel> ListProjectTask { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public int TotalRecordsNote { get; set; }

        public List<DataAccess.Models.Project.ProjectEntityModel> listProject { get; set; }
    }
}
