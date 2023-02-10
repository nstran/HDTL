using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class CreateOrUpdateProjectResourceResult : BaseResult    {
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }
        public Guid ProjectResourceId { get; set; }
    }
}
