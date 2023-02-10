using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.BusinessLogic.Messages.Responses.Task
{
    public class GetProjectScopeByProjectIdResponse : BaseResponse
    {
        public List<ProjectScopeModel> ListProjectScrope { get; set; }
    }
}
