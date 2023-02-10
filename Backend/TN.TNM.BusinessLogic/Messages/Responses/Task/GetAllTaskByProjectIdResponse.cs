using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Project;


namespace TN.TNM.BusinessLogic.Messages.Responses.Task
{
    public class GetAllTaskByProjectIdResponse : BaseResponse
    {
       public List<TaskModel> ListTask { get; set; }
    }
}
