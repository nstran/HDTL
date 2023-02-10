using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class GetMasterDataAddOrRemoveTaskToMilestoneResponse : BaseResponse
    {
        public List<TaskEntityModel> ListTask { get; set; }
    }
}
