using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class UpdateProjectMilestoneResponse : BaseResponse
    {
        public Guid MilestoneId { get; set; }
    }
}
