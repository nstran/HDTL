using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class UpdateProjectResponse : BaseResponse
    {
        public Guid ProjectId { get; set; }
    }
}
