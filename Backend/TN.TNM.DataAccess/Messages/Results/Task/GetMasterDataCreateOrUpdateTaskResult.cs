using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Messages.Results.Task
{
    public class GetMasterDataCreateOrUpdateTaskResult : BaseResult
    {
        public ProjectEntityModel Project { get; set; }
        public List<CategoryEntityModel> ListTaskType { get; set; }
        public List<RelateTaskMappingEntityModel> ListRelateTaskParent { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<ProjectMilestoneEntityModel> ListMilestone { get; set; }
        public List<ProjectResourceEntityModel> ListProjectResource { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public List<TaskDocumentEntityModel> ListTaskDocument { get; set; }
        public TaskEntityModel Task { get; set; }
        public ProjectScopeModel Scope { get; set; }

        public List<TaskConstraintEntityModel> ListTaskConstraintBefore { get; set; }
        public List<TaskConstraintEntityModel> ListTaskConstraintAfter { get; set; }
        public int TotalRecordsNote { get; set; }

        public List<ProjectEntityModel> listProject { get; set; }
        public List<RelateTaskMappingEntityModel> ListRelateTask { get; set; }
        public bool IsManager { get; set; }
        public bool IsPresident { get; set; }
    }
}
