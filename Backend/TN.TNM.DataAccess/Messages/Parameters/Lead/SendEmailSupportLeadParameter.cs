using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Queue;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class SendEmailSupportLeadParameter: BaseParameter
    {
        //public Queue Queue { get; set; }
        public List<QueueEntityModel> ListQueue { get; set; }
    }
}
