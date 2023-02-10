using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Messages.Responses.Task
{
    public class SearchTaskFromProjectScopeResponse : BaseResponse
    {
        public List<TaskEntityModel> ListProjectScrope { get; set; }
    }
}
