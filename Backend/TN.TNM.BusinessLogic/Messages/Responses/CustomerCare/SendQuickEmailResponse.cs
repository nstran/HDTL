using System;
using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class SendQuickEmailResponse : BaseResponse
    {
        public Guid QueueId { get; set; }

        public List<string> listInvalidEmail { get; set; }
    }
}
