using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class SendEmailInterviewResult : BaseResult
    {
        public List<string> listInvalidEmail { get; set; }
    }
}
