using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.AuditTrace
{
    public class TraceEntityModel
    {
        public Guid UserId { get; set; }
        
        public Guid EmployeeId { get; set; }

        public string UserName { get; set; }

        public string EmployeeName { get; set; }

        public string ActionName { get; set; }
        
        public string ActionType { get; set; }

        public Guid ObjectId { get; set; }

        public string ObjectName { get; set; }
        
        public string ObjectCode { get; set; }

        public DateTime CreateDate { get; set; }
        
        public string Description { get; set; }

        public string BackgroundColor { get; set; }

        public string Color { get; set; }
    }
}
