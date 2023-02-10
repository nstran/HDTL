using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class CreateOrUpdateProjectResourceResponse : BaseResponse    {
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }
        public Guid ProjectResourceId { get; set; }
    }
}
