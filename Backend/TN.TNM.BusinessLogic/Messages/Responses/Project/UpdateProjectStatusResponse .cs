using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class UpdateProjectStatusResponse : BaseResponse    {
        public Guid? StatusId { get; set; }
    }
}
