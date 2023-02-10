using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models
{
    public class MailModel
    {
        public bool UsingDefaultReceiverEmail { get; set; }
        public string DefaultReceiverEmail { get; set; }
        public string SmtpEmailAccount { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpSsl { get; set; }
    }
}
