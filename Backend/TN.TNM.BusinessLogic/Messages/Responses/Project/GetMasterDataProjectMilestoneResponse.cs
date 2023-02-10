using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class GetMasterDataProjectMilestoneResponse : BaseResponse
    {
        public List<ProjectMilestoneEntityModel> ListProjectMilestoneInProgress { get; set; }
        public List<ProjectMilestoneEntityModel> ListProjectMilestoneComplete { get; set; }
        public ProjectEntityModel Project { get; set; }
        public decimal ProjectTaskComplete { get; set; }
        public decimal TotalEstimateHour { get; set; }

        public List<NoteEntityModel> ListNote { get; set; }
        public int TotalRecordsNote { get; set; }

        public List<ProjectEntityModel> ListProject { get; set; }
    }
}
