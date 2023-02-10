using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Messages.Responses.Task
{
    public class GetMasterDataCreateOrUpdateTaskResponse : BaseResponse
    {
        public ProjectEntityModel Project { get; set; }
        public List<CategoryEntityModel> ListTaskType { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<ProjectMilestoneEntityModel> ListMilestone { get; set; }
        public List<ProjectResourceEntityModel> ListProjectResource { get; set; }
        public List<TaskDocumentEntityModel> ListTaskDocument { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public ProjectScopeModel Scope { get; set; }
        public TaskEntityModel Task { get; set; }
        public List<TaskConstraintEntityModel> ListTaskConstraintBefore { get; set; }
        public List<TaskConstraintEntityModel> ListTaskConstraintAfter { get; set; }

        public List<ProjectEntityModel> listProject { get; set; }

        public bool IsManager { get; set; }
        public bool IsPresident { get; set; }
    }
}
