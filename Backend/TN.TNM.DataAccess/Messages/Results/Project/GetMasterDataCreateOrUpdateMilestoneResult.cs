using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetMasterDataCreateOrUpdateMilestoneResult : BaseResult
    {
        public List<ProjectEntityModel> ListProject { get; set; }
        public ProjectMilestoneEntityModel ProjectMilestone { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
    }
}
