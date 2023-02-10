using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class GetDataMilestoneByIdResponse : BaseResponse
    {
        public List<TaskEntityModel> ListTaskNew { get; set; }
        public List<TaskEntityModel> ListTaskInProgress { get; set; }
        public List<TaskEntityModel> ListTaskComplete { get; set; }
        public List<TaskEntityModel> ListTaskClose { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public bool IsContainResource { get; set; }
        public ProjectMilestoneEntityModel ProjectMilestone { get; set; }
    }
}
