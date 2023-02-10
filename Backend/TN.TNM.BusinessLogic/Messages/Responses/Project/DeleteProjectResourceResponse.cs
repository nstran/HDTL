using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Task
{
    public class DeleteProjectResourceResponse : BaseResponse
    {
       public Guid ProjectResourceId { get; set; }
    }
}
