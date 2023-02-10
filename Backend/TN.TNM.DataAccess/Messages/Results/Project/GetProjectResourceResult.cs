using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetProjectResourceResult : BaseResult
    {   
        public ProjectEntityModel Project { get; set; }
        public List<ProjectResourceEntityModel> ListProjectResource { get; set; }
        public List<CategoryEntityModel> ListPaymentMethod { get; set; }
        public List<TaskEntityModel> ListProjectTask { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public int TotalRecordsNote { get; set; }

        public List<ProjectEntityModel> listProject { get; set; }
    }
}
