using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Task
{
    public class CreateOrUpdateTaskResponse : BaseResponse
    {
       public Guid? TaskId { get; set; }
    }
}
