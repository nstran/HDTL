using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.AuditTrace
{
    public class LoginTraceEntityModel
    {
        public Guid UserId { get; set; }

        public Guid? EmployeeId { get; set; }


        public string UserName { get; set; }

        public DateTime? LoginDate { get; set; }
        
        public int? StatusCode { get; set; } 

        public string BackgroundColor { get; set; }

        public string Color { get; set; }

        public string EmployeeCode { get; set; }
        
        public string EmployeeName { get; set; }
    }
}
