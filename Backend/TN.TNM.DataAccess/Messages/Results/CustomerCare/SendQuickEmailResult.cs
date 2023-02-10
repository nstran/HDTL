using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class SendQuickEmailResult : BaseResult
    {
        public Guid QueueId { get; set; }

        public List<string> listInvalidEmail { get; set; }
    }
}
